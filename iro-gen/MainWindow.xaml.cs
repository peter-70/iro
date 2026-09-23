using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace IroGen;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, FrameworkElement> editors = [];
    private GeneratedScene? current;
    private readonly CancellationTokenSource lifetime = new();
    private bool busy;

    private readonly string? analysisProjectRoot;
    public MainWindow() : this(null) { }
    public MainWindow(string? projectRoot)
    {
        analysisProjectRoot = projectRoot;
        InitializeComponent();
        Section("Bild & Zufall");
        Number(nameof(GeneratorOptions.Width), "Wand / Bildbreite (px)");
        Number(nameof(GeneratorOptions.Height), "Wand / Bildhöhe (px)");
        Number(nameof(GeneratorOptions.Seed), "Zufallsseed");
        Section("Farbstreifen");
        Choice(nameof(GeneratorOptions.Position), "Position", ["Rechts", "Links", "Unten", "Oben", "Mittig"]);
        Choice(nameof(GeneratorOptions.Orientation), "Ausrichtung", ["Vertikal", "Horizontal"]);
        Number(nameof(GeneratorOptions.FieldCount), "Anzahl Farbfelder (1–20)");
        Number(nameof(GeneratorOptions.StripWidthPercent), "Breite (% der Querachse, 5–95)");
        Number(nameof(GeneratorOptions.StripLengthPercent), "Länge (% der Längsachse, 10–98)");
        Number(nameof(GeneratorOptions.GapPercent), "Zwischenraum (% der Feldhöhe, 0–15)");
        Number(nameof(GeneratorOptions.MarginPercent), "Abstand zum Bildrand (%, 0–25)");
        Number(nameof(GeneratorOptions.ShadeStep), "Helligkeitsstufe (HSL-Prozentpunkte, 0,5–10)");
        Check(nameof(GeneratorOptions.RoundedTop), "Abgerundeter Streifenabschluss");
        Check(nameof(GeneratorOptions.VariableFieldHeights), "Unterschiedliche Feldhöhen");
        Check(nameof(GeneratorOptions.Labels), "Sollwerte auf Farbfelder drucken");
        Section("Wandfarbe");
        Number(nameof(GeneratorOptions.MatchingField), "Bezugsfeld (Nummer ab 1)");
        Choice(nameof(GeneratorOptions.WallDifference), "Abweichung vom Bezugsfeld", ["Exakt gleich", "Leicht (ca. ΔE00 1)", "Mittel (ca. ΔE00 4)", "Stark (ca. ΔE00 12)", "Andere Farbfamilie"]);
        Section("Geometrie & Abstand");
        Choice(nameof(GeneratorOptions.Distance), "Kameraabstand (Simulation)", ["Normal", "Zu nahe: nahe", "Zu nahe: näher", "Zu nahe: ganz nahe", "Zu weit: weit", "Zu weit: weiter", "Zu weit: sehr weit"]);
        editors[nameof(GeneratorOptions.Distance)].Tag = new[] { CameraDistance.Normal, CameraDistance.Near, CameraDistance.Nearer, CameraDistance.TooClose, CameraDistance.Far, CameraDistance.Farther, CameraDistance.TooFar };
        Number(nameof(GeneratorOptions.RotationDegrees), "Drehung im Bild (−80 bis 80°)");
        Check(nameof(GeneratorOptions.RandomPlacement), "Zufällige Position und Drehung");
        editors[nameof(GeneratorOptions.RandomPlacement)].ToolTip = "Ersetzt die feste Position und Drehung durch reproduzierbare Zufallswerte.";
        Choice(nameof(GeneratorOptions.SideView), "Blick von der Seite (Seite zufällig)", ["Aus", "Leicht", "Stark", "Sehr stark"]);
        Choice(nameof(GeneratorOptions.VerticalView), "Blick von oben / unten", ["Aus", "Leicht", "Stark", "Sehr stark"]);
        Choice(nameof(GeneratorOptions.VerticalDirection), "Blickrichtung oben / unten", ["Zufällig", "Von oben", "Von unten"]);
        Choice(nameof(GeneratorOptions.WallGap), "Streifen vor der Wand (Simulation)", ["Anliegend", "Kleiner Abstand (< 10 cm)", "Größerer Abstand (10–< 20 cm)", "Großer Abstand (> 20 cm)"]);
        Level(nameof(GeneratorOptions.Perspective), "Zusätzliche Verjüngung (bisherige Perspektive)");
        Section("Licht & Bildstörungen");
        Level(nameof(GeneratorOptions.Glare), "Glanzlicht auf dem Streifen");
        Level(nameof(GeneratorOptions.Dirt), "Verschmutzung / Flecken");
        Level(nameof(GeneratorOptions.Blur), "Unscharfer Fokus");
        Level(nameof(GeneratorOptions.MotionBlur), "Bewegungsunschärfe");
        Level(nameof(GeneratorOptions.Shadows), "Licht und Schatten");
        Level(nameof(GeneratorOptions.Texture), "Wandstruktur");
        Level(nameof(GeneratorOptions.Noise), "Bildrauschen");
        Level(nameof(GeneratorOptions.Vignette), "Abgedunkelte Bildränder");
        Level(nameof(GeneratorOptions.Occlusion), "Verdeckte Farbfelder");
        Level(nameof(GeneratorOptions.Haze), "Schleier / Kontrastverlust");
        Number(nameof(GeneratorOptions.ExposureStops), "Belichtung (−3 bis +3 EV)");
        SetOptions(new());
        Loaded += async (_, _) => await GenerateAsync();
        Closing += (_, e) =>
        {
            if (!busy) return;
            e.Cancel = true;
            closeAfterOperation = true;
            operation?.Cancel();
            Status.Text = "Vorgang wird beendet und temporäre Daten werden aufgeräumt …";
        };
        Closed += (_, _) => { lifetime.Cancel(); batch?.Dispose(); lifetime.Dispose(); };
        UpdateColorMode();
    }

    private void Section(string title) => OptionsPanel.Children.Add(new TextBlock { Text = title, FontSize = 17, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0, 20, 0, 12) });
    private void Add(string property, string title, FrameworkElement element)
    {
        var label = new Label { Content = title, Target = element, Padding = new Thickness(0, 0, 0, 5), FontSize = 12 };
        OptionsPanel.Children.Add(label);
        element.Margin = new Thickness(0, 0, 0, 12);
        AutomationProperties.SetName(element, title);
        OptionsPanel.Children.Add(element);
        editors[property] = element;
    }
    private void Number(string property, string title) => Add(property, title, new TextBox());
    private void Choice(string property, string title, string[] items) => Add(property, title, new ComboBox { ItemsSource = items, SelectedIndex = 0 });
    private void Level(string property, string title) => Choice(property, title, ["Aus", "Leicht", "Mittel", "Stark"]);
    private void Check(string property, string title) => Add(property, title, new CheckBox { Content = "Aktiviert" });

    private GeneratorOptions ReadOptions()
    {
        var options = new GeneratorOptions();
        foreach (var (name, control) in editors)
        {
            var property = typeof(GeneratorOptions).GetProperty(name)!;
            object value;
            if (control is CheckBox check) value = check.IsChecked == true;
            else if (control is ComboBox combo) value = combo.Tag is CameraDistance[] distances ? distances[combo.SelectedIndex] : Enum.ToObject(property.PropertyType, combo.SelectedIndex);
            else
            {
                string input = ((TextBox)control).Text.Trim().Replace(',', '.');
                if (property.PropertyType == typeof(int))
                {
                    if (!int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number))
                        throw new ArgumentException($"{AutomationProperties.GetName(control)}: Bitte eine ganze Zahl eingeben.");
                    value = number;
                }
                else
                {
                    if (!double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
                        throw new ArgumentException($"{AutomationProperties.GetName(control)}: Bitte eine Zahl eingeben.");
                    value = number;
                }
            }
            property.SetValue(options, value);
        }
        if (CoverRangeCheck.IsChecked == true && ReadImageCount() > 1)
            options = options with { MatchingField = Math.Min(options.MatchingField, options.FieldCount) };
        options.Validate();
        return options;
    }

    private void SetOptions(GeneratorOptions options)
    {
        foreach (var (name, control) in editors)
        {
            object value = typeof(GeneratorOptions).GetProperty(name)!.GetValue(options)!;
            if (control is CheckBox check) check.IsChecked = (bool)value;
            else if (control is ComboBox combo) combo.SelectedIndex = combo.Tag is CameraDistance[] distances ? Array.IndexOf(distances, (CameraDistance)value) : Convert.ToInt32(value);
            else ((TextBox)control).Text = Convert.ToString(value, CultureInfo.GetCultureInfo("de-DE"))!;
        }
    }

    private Task GenerateAsync() => GenerateSeriesAsync();

    private async void Generate_Click(object sender, RoutedEventArgs e) => await GenerateAsync();
    private async void Random_Click(object sender, RoutedEventArgs e)
    {
        ((TextBox)editors[nameof(GeneratorOptions.Seed)]).Text = RandomNumberGenerator.GetInt32(int.MaxValue).ToString(CultureInfo.InvariantCulture);
        await GenerateAsync();
    }
    private async void Export_Click(object sender, RoutedEventArgs e) => await ExportAllAsync();

    private void SaveOptions_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var options = ReadOptions();
            var dialog = new SaveFileDialog { Filter = "IroGen-Optionen (*.json)|*.json", FileName = "irogen-optionen.json" };
            if (dialog.ShowDialog(this) == true)
            {
                File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(new OptionsFile(2, SceneGenerator.Version, options, ReadImageCount(), CoverRangeCheck.IsChecked == true), SceneExport.JsonOptions));
                Status.Text = "Optionen gespeichert: " + dialog.FileName;
            }
        }
        catch (Exception error) { ShowError(error); }
    }
    private async void Load_Click(object sender, RoutedEventArgs e)
    {
        if (busy) return;
        var dialog = new OpenFileDialog { Filter = "IroGen-Optionen oder Aufnahme (*.json)|*.json" };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(dialog.FileName));
            var root = document.RootElement;
            GeneratorOptions options;
            if (root.TryGetProperty("conditions", out var conditions))
            {
                var generator = conditions.GetProperty("generator");
                if (root.GetProperty("formatVersion").GetInt32() != 1 || generator.GetProperty("name").GetString() != "IroGen" || generator.GetProperty("version").GetString() is not ("1.0.0" or "1.1.0" or "1.2.0" or SceneGenerator.Version))
                    throw new ArgumentException("Diese Aufnahme stammt nicht aus einer unterstützten IroGen-Version.");
                SeriesCount.Text = "1";
                options = generator.GetProperty("parameters").GetProperty("options").Deserialize<GeneratorOptions>(SceneExport.JsonOptions)!;
            }
            else
            {
                var file = root.Deserialize<OptionsFile>(SceneExport.JsonOptions) ?? throw new ArgumentException("Leere Optionsdatei.");
                if (file.FormatVersion is not (1 or 2) || file.GeneratorVersion is not ("1.0.0" or "1.1.0" or "1.2.0" or SceneGenerator.Version)) throw new ArgumentException("Nicht unterstützte Optionsversion.");
                options = file.Options;
                SeriesCount.Text = file.ImageCount.ToString(CultureInfo.InvariantCulture);
                CoverRangeCheck.IsChecked = file.CoverRange;
            }
            if (options == null) throw new ArgumentException("Die Datei enthält keine Optionen.");
            options.Validate(); SetOptions(options); await GenerateSeriesAsync(options);
        }
        catch (Exception error) { ShowError(error); }
    }
    private void ShowError(Exception error)
    {
        Status.Text = error.Message;
        MessageBox.Show(this, error.Message, "IroGen", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    private sealed record OptionsFile(int FormatVersion, string GeneratorVersion, GeneratorOptions Options, int ImageCount = 1, bool CoverRange = true);
}





