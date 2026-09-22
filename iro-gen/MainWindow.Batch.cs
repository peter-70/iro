using Iro.Analysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace IroGen;

public partial class MainWindow
{
    private string? lastResultFile;
    private string? analyzedBatchId;
    private string? resultProjectRoot;
    private GeneratedBatch? batch;
    private IReadOnlyDictionary<string, CaptureAnalysis> analyses = new Dictionary<string, CaptureAnalysis>();
    private bool closeAfterOperation;
    private CancellationTokenSource? operation;

    private async void LoadPlan_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        var dialog = new OpenFileDialog { Filter = "IroGen-Testplan (*.json)|*.json", Title = "Testplan laden und Bilder erzeugen" };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            if (new FileInfo(dialog.FileName).Length > 1024 * 1024) throw new ArgumentException("Testplan ist größer als 1 MB.");
            var plan = TestPlan.Parse(File.ReadAllText(dialog.FileName));
            await GenerateSeriesAsync(plan: plan);
        }
        catch (Exception error) { ShowError(error); }
    }
    private int ReadImageCount()
    {
        if (!int.TryParse(SeriesCount.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count) || count is < 1 or > 10000)
            throw new ArgumentException("Anzahl Bilder: Bitte eine ganze Zahl von 1 bis 10000 eingeben.");
        return count;
    }

    private void SetBusy(bool value)
    {
        busy = value;
        GenerateButton.IsEnabled = RandomButton.IsEnabled = OptionsPanel.IsEnabled = SeriesCount.IsEnabled = CoverRangeCheck.IsEnabled = !value;
        BatchPicker.IsEnabled = PlanLoadButton.IsEnabled = ReviewTestsButton.IsEnabled = !value;
        ExportButton.IsEnabled = SendTestsButton.IsEnabled = !value && batch != null;
        CancelButton.IsEnabled = value;
        if (!value)
        {
            UpdateColorMode();
            if (closeAfterOperation) Close();
        }
    }

    private void UpdateColorMode()
    {
        bool automatic = CoverRangeCheck?.IsChecked == true && int.TryParse(SeriesCount?.Text, out int count) && count > 1;
        foreach (string name in new[] { nameof(GeneratorOptions.WallDifference), nameof(GeneratorOptions.MatchingField) })
            if (editors.TryGetValue(name, out var editor)) editor.IsEnabled = !automatic;
        if (ColorModeHint != null) ColorModeHint.Text = automatic
            ? "Serie: Wandfarbe und Bezugsfeld werden für die Abstandsverteilung gewählt. Geometrie, Abstufung und Störungen bleiben erhalten."
            : "Einzelbild / feste Wandoption: Die gewählte Wandabweichung gilt für jedes Bild.";
    }

    private void SeriesSettings_Changed(object sender, RoutedEventArgs e) => UpdateColorMode();
    private void Cancel_Click(object sender, RoutedEventArgs e) => operation?.Cancel();

    private async Task GenerateSeriesAsync(GeneratorOptions? replayOptions = null, TestPlan? plan = null)
    {
        if (busy) return;
        GeneratedBatch? generated = null;
        try
        {
            int count = plan?.Expand().Sum(c => c.Case.Count) ?? ReadImageCount();
            bool coverage = count > 1 && CoverRangeCheck.IsChecked == true;
            var options = plan?.Defaults ?? replayOptions ?? ReadOptions();
            if (coverage) options = options with { MatchingField = Math.Min(options.MatchingField, options.FieldCount) };
            options.Validate();
            operation = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
            SetBusy(true);
            GenerationProgress.Minimum = 0; GenerationProgress.Maximum = count; GenerationProgress.Value = 0;
            Status.Text = $"{count} Testbilder werden erzeugt …";
            var progress = new Progress<BatchProgress>(p =>
            {
                GenerationProgress.Value = p.Completed;
                Status.Text = $"Bild {p.Completed} von {p.Total} erzeugt …";
            });
            var completion = new TaskCompletionSource<GeneratedBatch>(TaskCreationOptions.RunContinuationsAsynchronously);
            var token = operation.Token;
            var worker = new Thread(() =>
            {
                try { completion.SetResult(plan != null ? plan.Generate(Path.Combine(Path.GetTempPath(), "IroGen"), progress, token) : BatchGenerator.Generate(options, count, coverage, Path.Combine(Path.GetTempPath(), "IroGen"), progress, token)); }
                catch (Exception error) { completion.SetException(error); }
            }) { IsBackground = true, Name = "IroGen series" };
            worker.SetApartmentState(ApartmentState.STA); worker.Start();
            generated = await completion.Task;
            if (lifetime.IsCancellationRequested) { generated.Dispose(); return; }
            // Load before replacing the previous completed series; failures preserve it.
            var first = generated.Items[0].Load();
            var previous = batch;
            batch = generated; generated = null;
            analyses = new Dictionary<string, CaptureAnalysis>();
            analyzedBatchId = null; lastResultFile = null;
            if (plan != null) SeriesCount.Text = batch.Items.Count.ToString(CultureInfo.InvariantCulture);
            TestCompletion.Visibility = Visibility.Collapsed;
            BatchPicker.ItemsSource = batch.Items;
            BatchPicker.SelectedIndex = 0;
            DisplayScene(first);
            CoverageSummary.Text = batch.Summary;
            HandoffStatus.Text = "Noch kein Analyselauf für diese Serie.";
            previous?.Dispose();
            Status.Text = "Vorschau fertig. " + batch.Summary + ". Alle Bilder können gespeichert oder an Iro-Tests übergeben werden.";
        }
        catch (OperationCanceledException) { Status.Text = "Abgebrochen. Die zuvor fertige Serie bleibt verfügbar."; }
        catch (Exception error) { generated?.Dispose(); ShowError(error); }
        finally { operation?.Dispose(); operation = null; SetBusy(false); }
    }

    private void DisplayScene(GeneratedScene scene)
    {
        current = scene;
        Preview.Source = scene.Image; RefreshAnalysisRows();
        ImageInfo.Text = $"{scene.Options.Width} × {scene.Options.Height} px · Seed {scene.Options.Seed} · Wand {scene.Wall.Hex} · {scene.Options.FieldCount} Farbfelder";
    }

    private sealed record AnalysisRow(string FieldId, string Hex, string ExpectedText, string ActualText, string AnalysisHint);

    private void RefreshAnalysisRows()
    {
        if (current == null) return;
        CaptureAnalysis? capture = null;
        if (BatchPicker.SelectedItem is BatchItem item) analyses.TryGetValue(item.CaptureId, out capture);
        Results.ItemsSource = current.Fields.Select(field =>
        {
            var comparison = capture?.Comparisons.FirstOrDefault(c => c.ExpectedFieldId == field.FieldId);
            var measured = capture?.Analysis?.Fields.FirstOrDefault(f => f.FieldId == comparison?.DetectedFieldId);
            string value = comparison?.ActualDeltaE00?.ToString("F2", CultureInfo.GetCultureInfo("de-DE")) ?? "–";
            string hint = capture == null ? "Noch nicht analysiert" : capture.Error ?? (comparison?.Status switch
            {
                "measured" => $"Ist − Soll: {comparison.DifferenceFromNominal?.ToString("+0.00;-0.00;0.00", CultureInfo.GetCultureInfo("de-DE"))}",
                "rejected" => measured?.Hint ?? "Messfläche ungeeignet",
                "ambiguous-assignment" => "Zuordnung mehrdeutig",
                "not-detected" => "Farbfeld nicht erkannt",
                _ => capture.Analysis?.Hint ?? "Nicht erkannt"
            });
            return new AnalysisRow(field.FieldId, field.Hex, field.ExpectedText, value, hint);
        }).ToArray();
    }

    private void BatchPicker_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (BatchPicker.SelectedItem is not BatchItem item) return;
        try { DisplayScene(item.Load()); }
        catch (Exception error) { ShowError(error); }
    }

    private async Task ExportAllAsync()
    {
        if (batch == null || busy) return;
        var dialog = new OpenFolderDialog { Title = $"Alle {batch.Items.Count} Bilder als neue Serie speichern" };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            operation = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token); SetBusy(true);
            Status.Text = "Alle Bilder und Beschreibungen werden gespeichert …";
            var snapshot = batch; var token = operation.Token;
            string output = await Task.Run(() => BatchGenerator.SaveAll(snapshot, dialog.FolderName, token));
            Status.Text = $"Alle {snapshot.Items.Count} Bilder gespeichert: {output}";
        }
        catch (OperationCanceledException) { Status.Text = "Speichern abgebrochen. Die erzeugte Serie bleibt verfügbar."; }
        catch (Exception error) { ShowError(error); }
        finally { operation?.Dispose(); operation = null; SetBusy(false); }
    }

    private void OpenTestResults_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (lastResultFile == null || !File.Exists(lastResultFile))
                throw new IOException("Die Ergebnisdatei ist nicht mehr vorhanden.");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"),
                Arguments = "/select,\"" + Path.GetFullPath(lastResultFile) + "\"",
                UseShellExecute = true
            });
        }
        catch (Exception error) { ShowError(error); }
    }

    private async void ReviewTests_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        string? root = analysisProjectRoot ?? IroTestHandoff.FindProjectRoot(AppContext.BaseDirectory);
        if (root == null)
        {
            var folderDialog = new OpenFolderDialog { Title = "Iro-Projektstamm mit iro.slnx wählen" };
            if (folderDialog.ShowDialog(this) != true) return;
            root = folderDialog.FolderName;
        }
        try
        {
            // Never silently substitute historical results for the visible generated series.
            if (batch != null && analyzedBatchId != batch.Id)
                await SendTestsAsync();
            if (closeAfterOperation || (batch != null && analyzedBatchId != batch.Id)) return;
            string? runId = batch != null && lastResultFile != null ? Path.GetFileName(Path.GetDirectoryName(lastResultFile)) : null;
            new TestResultsWindow(resultProjectRoot ?? root, runId) { Owner = this }.ShowDialog();
        }
        catch (Exception error) { ShowError(error); }
    }

    private async void SendTests_Click(object sender, RoutedEventArgs e) => await SendTestsAsync();

    private async Task SendTestsAsync()
    {
        if (batch == null || busy) return;
        string? root = analysisProjectRoot ?? IroTestHandoff.FindProjectRoot(AppContext.BaseDirectory);
        if (root == null)
        {
            var dialog = new OpenFolderDialog { Title = "Iro-Projektstamm mit iro.slnx wählen" };
            if (dialog.ShowDialog(this) != true) return;
            root = dialog.FolderName;
        }
        try
        {
            operation = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token); SetBusy(true);
            Status.Text = "Vollständiger Testauftrag wird bereitgestellt …";
            var snapshot = batch; var token = operation.Token;

            var progress = new Progress<AnalysisRunProgress>(p =>
            {
                Status.Text = $"Iro analysiert Bild {p.Completed} von {p.Total} …";
            });
            TestCompletion.Visibility = Visibility.Collapsed;
            GenerationProgress.Visibility = Visibility.Collapsed;
            var run = await DelayedBusyIndicator.RunAsync(() => Task.Run(async () =>
            {
                var handoff = IroTestHandoff.Submit(snapshot, root, token);
                return await new AnalysisRunner().RunAsync(handoff.Folder, Path.Combine(root, "tests", "runs"), new(), progress, token);
            }), visible => TestBusyIndicator.Visibility = visible ? Visibility.Visible : Visibility.Collapsed, token);
            lastResultFile = run.ResultFile;
            resultProjectRoot = root;
            analyzedBatchId = snapshot.Id;
            if (!closeAfterOperation)
            {
                TestCompletionText.Text = run.Report.Status switch
                {
                    "cancelled" => $"Tests abgebrochen. {run.Report.ProcessedCount} von {run.Report.RequestedCount} Bildern verarbeitet; Teilergebnisse gespeichert.",
                    "completed-with-errors" => $"Tests vollständig durchgelaufen: {run.Report.ProcessedCount} Bilder. Bei {run.Report.Captures.Count(c => c.Error != null)} Bildern gab es Verarbeitungsfehler.",
                    _ => $"Tests vollständig durchgelaufen: {run.Report.ProcessedCount} Bilder, {run.Report.MeasuredFields} auswertbare Farbfelder."
                };
                TestCompletion.Visibility = Visibility.Visible;
            }
            analyses = run.Report.Captures.ToDictionary(c => c.CaptureId);
            RefreshAnalysisRows();
            string completion = run.Report.Status == "cancelled" ? "Analyse abgebrochen" : "Analyse abgeschlossen";
            HandoffStatus.Text = $"{completion}: {run.Report.ProcessedCount}/{run.Report.RequestedCount} Bilder verarbeitet, " +
                $"{run.Report.ImagesWithMeasurements} Bilder mit Messwerten, {run.Report.MeasuredFields} auswertbare Felder. " +
                $"{run.Report.Captures.Count(c => c.Error != null)} Dateifehler. Ergebnisdatei: {run.ResultFile}";
            Status.Text = "Experimenteller Iro-Testweg. Soll/Ist-Werte und Ablehnungsgründe stehen in der Ergebnisdatei; Details pro Feld erscheinen in der Tabelle; die ausführliche Auswertung folgt später.";
        }
        catch (OperationCanceledException) { Status.Text = "Testübergabe oder Analysestart abgebrochen. Bereits bereitgestellte Aufträge bleiben erhalten."; }
        catch (Exception error) { ShowError(error); }
        finally { TestBusyIndicator.Visibility = Visibility.Collapsed; GenerationProgress.Visibility = Visibility.Visible; operation?.Dispose(); operation = null; SetBusy(false); }
    }
}







