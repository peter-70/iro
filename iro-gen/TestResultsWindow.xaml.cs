using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace IroGen;

public partial class TestResultsWindow : Window
{
    private readonly string projectRoot;
    private readonly string? runId;
    private IReadOnlyList<TestRunRow> rows = [];

    public TestResultsWindow(string projectRoot, string? runId = null)
    {
        this.projectRoot = projectRoot;
        this.runId = runId;
        InitializeComponent();
        Loaded += (_, _) => Reload();
    }

    private double ReadTolerance() =>
        double.TryParse(Tolerance.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out double value)
        && double.IsFinite(value) && value >= 0 ? value : 1d;

    private void Reload()
    {
        try
        {
            rows = TestRunReview.Load(projectRoot, ReadTolerance(), runId: runId);
            Show(rows);
        }
        catch (Exception error)
        {
            Summary.Text = error.Message;
            MessageBox.Show(this, error.Message, "IroGen", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Show(IReadOnlyList<TestRunRow> all)
    {
        var visible = OnlyProblems.IsChecked == true
            ? all.Where(r => r.Verdict != ReviewVerdict.Erreicht).ToList()
            : [.. all];
        Rows.ItemsSource = visible;
        if (all.Count == 0)
        {
            Summary.Text = "Unter tests/runs liegen noch keine lesbaren Analyseläufe. "
                         + "Erst „An Iro-Tests senden“ ausführen.";
            return;
        }
        int ok = all.Count(r => r.Verdict == ReviewVerdict.Erreicht);
        int wrong = all.Count(r => r.Verdict == ReviewVerdict.FalscherMesswert);
        int partial = all.Count(r => r.Verdict == ReviewVerdict.Teilweise);
        int refused = all.Count(r => r.Verdict == ReviewVerdict.Abgewiesen);
        int broken = all.Count(r => r.Verdict == ReviewVerdict.Fehler);
        var parts = new List<string> { $"{all.Count} Bilder ausgewertet", $"{ok} vollständig und genau gemessen" };
        if (wrong > 0) parts.Add($"{wrong} mit freigegebenen, aber zu weit abweichenden Werten");
        if (partial > 0) parts.Add($"{partial} nur teilweise gemessen");
        if (refused > 0) parts.Add($"{refused} vollständig abgewiesen");
        if (broken > 0) parts.Add($"{broken} nicht lesbar");
        Summary.Text = (runId != null ? "Aktuelle Serie · " : "Gespeicherte Läufe · ") + string.Join(" · ", parts)
            + (visible.Count != all.Count ? $" — angezeigt: {visible.Count}" : string.Empty);
    }

    private void Tolerance_Changed(object sender, TextChangedEventArgs e)
    {
        if (!IsLoaded) return;
        rows = TestRunReview.Load(projectRoot, ReadTolerance(), runId: runId);
        Show(rows);
    }

    private void Filter_Changed(object sender, RoutedEventArgs e)
    {
        if (IsLoaded) Show(rows);
    }

    private void Reload_Click(object sender, RoutedEventArgs e) => Reload();

    private void OpenFolder_Click(object sender, RoutedEventArgs e)
    {
        if (Rows.SelectedItem is not TestRunRow row)
        {
            MessageBox.Show(this, "Bitte zuerst eine Zeile auswählen.", "IroGen",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        try
        {
            if (!Directory.Exists(row.Folder)) throw new IOException("Der Ordner des Laufs ist nicht mehr vorhanden.");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"),
                Arguments = "\"" + Path.GetFullPath(row.Folder) + "\"",
                UseShellExecute = true
            });
        }
        catch (Exception error)
        {
            MessageBox.Show(this, error.Message, "IroGen", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Kompakter Testbericht (*.md)|*.md",
                FileName = "iro-testbericht-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".md"
            };
            if (dialog.ShowDialog(this) == true)
                File.WriteAllText(dialog.FileName, TestReviewExport.Create(rows, ReadTolerance(), OnlyProblems.IsChecked == true));
        }
        catch (Exception error)
        {
            MessageBox.Show(this, error.Message, "IroGen", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
