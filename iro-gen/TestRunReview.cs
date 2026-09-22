using System.IO;
using System.Text.Json;
using Iro.Analysis;
using Iro.Core.Analysis;

namespace IroGen;

/// <summary>
/// Liest die gespeicherten Analyseläufe unter tests/runs und ordnet ihnen die Szene des zugehörigen
/// Testauftrags zu. Die Bewertung ist eine Entwicklungsdiagnose mit offengelegter Regel, kein
/// Abnahmeurteil: Für gestörte Bilder ist im Testauftrag kein geprüftes Sollverhalten hinterlegt.
/// </summary>
public enum ReviewVerdict { Erreicht, FalscherMesswert, Teilweise, Abgewiesen, Fehler }

public sealed record TestRunRow(
    string RunId, string RequestId, DateTime? CreatedUtc, string Scene, string Disturbances,
    ReviewVerdict Verdict, string Finding, string Message, int Expected, int Measured,
    double? MaxDeviation, int Unexpected, string Folder)
{
    public string? CaptureId { get; init; }
    public string? CaseName { get; init; }
    public string? Seed { get; init; }
    public string? AnalyzerVersion { get; init; }
    public string? GeneratorVersion { get; init; }
    public string? OptionsText { get; init; }
    public string? AnalysisProfile { get; init; }
    public string? RunStatus { get; init; }
    public int RequestedCount { get; init; }
    public int ProcessedCount { get; init; }
    public string TimeText => CreatedUtc is { } t ? t.ToLocalTime().ToString("dd.MM. HH:mm") : "–";
    public string VerdictText => Verdict switch
    {
        ReviewVerdict.Erreicht => "Ziel erreicht",
        ReviewVerdict.FalscherMesswert => "Ziel nicht erreicht – falscher Messwert",
        ReviewVerdict.Teilweise => "Teilweise gemessen",
        ReviewVerdict.Abgewiesen => "Messung abgewiesen",
        _ => "Lauf nicht lesbar"
    };
    public string DeviationText => MaxDeviation is { } d ? d.ToString("0.0") : "–";
}

public static class TestRunReview
{
    /// <summary>Störungsoptionen des Generators. Ein Wert gilt als gesetzt, wenn er nicht neutral ist.</summary>
    private static readonly (string Key, string Label, string Neutral)[] Disturbances =
    [
        ("blur", "Unschärfe", "None"), ("motionBlur", "Bewegungsunschärfe", "None"),
        ("noise", "Rauschen", "None"), ("haze", "Schleier", "None"),
        ("glare", "Reflexe", "None"), ("shadows", "Schatten", "None"),
        ("vignette", "Randabfall", "None"), ("texture", "Oberflächenstruktur", "None"),
        ("occlusion", "Verdeckung", "None"), ("dirt", "Verschmutzung", "None"),
        ("perspective", "Perspektive", "None"), ("distance", "Abstand", "Normal"),
        ("rotationDegrees", "Drehung", "0"), ("exposureStops", "Belichtung", "0")
    ];

    public static IReadOnlyList<TestRunRow> Load(string projectRoot, double tolerance, CancellationToken token = default, string? runId = null)
    {
        var scenes = ReadScenes(Path.Combine(projectRoot, "tests", "requests"), token);
        string runsFolder = Path.Combine(projectRoot, "tests", "runs");
        if (!Directory.Exists(runsFolder)) return [];
        var rows = new List<TestRunRow>();
        foreach (string folder in Directory.EnumerateDirectories(runsFolder, "iro-run-*"))
        {
            token.ThrowIfCancellationRequested();
            if (runId != null && Path.GetFileName(folder) != runId) continue;
            string file = Path.Combine(folder, "results.json");
            if (!File.Exists(file)) continue;
            try
            {
                var run = JsonSerializer.Deserialize<AnalysisRun>(File.ReadAllText(file), AnalysisRunner.JsonOptions);
                if (run == null || run.Kind != "iro-analysis-run") continue;
                foreach (var capture in run.Captures) rows.Add(Build(run, capture, scenes, tolerance, folder));
            }
            catch (Exception error)
            {
                rows.Add(new(Path.GetFileName(folder), "–", null, "–", "–", ReviewVerdict.Fehler,
                    "Ergebnisdatei nicht lesbar", error.Message, 0, 0, null, 0, folder));
            }
        }
        return [.. rows.OrderBy(r => r.CreatedUtc ?? DateTime.MaxValue).ThenBy(r => r.RunId, StringComparer.Ordinal)];
    }

    private static TestRunRow Build(AnalysisRun run, CaptureAnalysis capture,
        IReadOnlyDictionary<string, SceneInfo> scenes, double tolerance, string folder)
    {
        DateTime.TryParse(run.CreatedAtUtc, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var created);
        scenes.TryGetValue(capture.CaptureId, out var scene);
        string sceneText = scene?.Scene ?? "Testauftrag nicht mehr vorhanden";
        string disturbanceText = scene?.Disturbances ?? "unbekannt";

        if (capture.Error != null)
            return new(run.RunId, run.RequestId, created, sceneText, disturbanceText, ReviewVerdict.Fehler,
                "Bild konnte nicht verarbeitet werden", capture.Error, 0, 0, null, 0, folder) { CaptureId = capture.CaptureId, CaseName = scene?.CaseName, Seed = scene?.Seed, GeneratorVersion = scene?.GeneratorVersion, OptionsText = scene?.OptionsText, RunStatus = run.Status, RequestedCount = run.RequestedCount, ProcessedCount = run.ProcessedCount };

        var comparisons = capture.Comparisons;
        int expected = comparisons.Count;
        int measured = comparisons.Count(c => c.Status == "measured");
        var deviations = comparisons.Where(c => c.DifferenceFromNominal is { } d && double.IsFinite(d))
            .Select(c => Math.Abs(c.DifferenceFromNominal!.Value)).ToList();
        double? maxDeviation = deviations.Count > 0 ? deviations.Max() : null;
        int unexpected = capture.UnexpectedDetectedFields;

        var verdict = measured == 0 ? ReviewVerdict.Abgewiesen
            : maxDeviation > tolerance ? ReviewVerdict.FalscherMesswert
            : measured < expected || unexpected > 0 ? ReviewVerdict.Teilweise
            : ReviewVerdict.Erreicht;

        string finding = measured == 0
            ? $"Kein Farbfeld gemessen (Status {StatusText(capture.Analysis?.Status)})"
            : $"{measured} von {expected} Feldern gemessen, größte Abweichung {(maxDeviation is { } m ? m.ToString("0.00") : "–")} ΔE00"
              + (unexpected > 0 ? $", {unexpected} zusätzlich erkannte Fläche(n)" : string.Empty);

        return new(run.RunId, run.RequestId, created, sceneText, disturbanceText, verdict, finding,
            Message(capture, verdict, maxDeviation, tolerance), expected, measured, maxDeviation, unexpected, folder)
        { CaptureId = capture.CaptureId, CaseName = scene?.CaseName, Seed = scene?.Seed, AnalyzerVersion = capture.Analysis?.AnalyzerVersion, GeneratorVersion = scene?.GeneratorVersion, OptionsText = scene?.OptionsText,
          AnalysisProfile = capture.Analysis == null ? null : JsonSerializer.Serialize(capture.Analysis.Options, new JsonSerializerOptions(AnalysisRunner.JsonOptions) { WriteIndented = false }),
          RunStatus = run.Status, RequestedCount = run.RequestedCount, ProcessedCount = run.ProcessedCount };
    }

    private static string Message(CaptureAnalysis capture, ReviewVerdict verdict, double? maxDeviation, double tolerance)
    {
        var analysis = capture.Analysis;
        if (analysis == null) return "Keine Analyse gespeichert.";
        if (verdict == ReviewVerdict.FalscherMesswert)
            return $"Messwerte freigegeben, obwohl sie bis zu {maxDeviation!.Value:0.0} ΔE00 vom nominalen Sollabstand "
                 + $"abweichen (Diagnosegrenze {tolerance:0.0}). App: {analysis.Hint} " + string.Join(" | ", analysis.Fields.Select(f => f.Hint).Where(h => !string.IsNullOrWhiteSpace(h)).Distinct());
        var reasons = analysis.Fields
            .SelectMany(f => new[] { f.Measurement?.Reason, f.Reference?.Reason, f.Hint })
            .Concat([verdict == ReviewVerdict.Erreicht ? null : analysis.Hint])
            .Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();
        return reasons.Count > 0 ? string.Join(" | ", reasons) : string.Empty;
    }

    private static string StatusText(AnalysisStatus? status) => status switch
    {
        AnalysisStatus.Measured => "gemessen",
        AnalysisStatus.PartiallyMeasured => "teilweise gemessen",
        AnalysisStatus.NoPattern => "kein Muster erkannt",
        AnalysisStatus.AmbiguousPattern => "mehrdeutiges Muster",
        AnalysisStatus.UnsuitableGeometry => "ungeeignete Geometrie",
        AnalysisStatus.InvalidReference => "Referenzfläche ungeeignet",
        AnalysisStatus.InvalidFields => "Messflächen ungeeignet",
        _ => "unbekannt"
    };

    private sealed record SceneInfo(string Scene, string Disturbances, string? CaseName, string? Seed, string? GeneratorVersion, string OptionsText);

    private static Dictionary<string, SceneInfo> ReadScenes(string requestsFolder, CancellationToken token)
    {
        var scenes = new Dictionary<string, SceneInfo>(StringComparer.Ordinal);
        if (!Directory.Exists(requestsFolder)) return scenes;
        foreach (string request in Directory.EnumerateDirectories(requestsFolder, "iro-request-*"))
        {
            token.ThrowIfCancellationRequested();
            string inputs = Path.Combine(request, "inputs");
            if (!Directory.Exists(inputs)) continue;
            foreach (string file in SafeEnumerate(inputs))
            {
                if (Path.GetFileName(file) == "series.json") continue;
                try
                {
                    using var document = JsonDocument.Parse(File.ReadAllText(file));
                    var root = document.RootElement;
                    if (!root.TryGetProperty("captureId", out var id) || id.GetString() is not { } captureId) continue;
                    if (!root.TryGetProperty("conditions", out var conditions)
                        || !conditions.TryGetProperty("generator", out var generator)
                        || !generator.TryGetProperty("parameters", out var parameters)
                        || !parameters.TryGetProperty("options", out var options)) continue;
                                        string? Text(string key) => options.TryGetProperty(key, out var value) ? value.ToString() : null;
                    var compact = System.Text.Json.Nodes.JsonNode.Parse(options.GetRawText())!.AsObject();
                    compact.Remove("colors"); compact.Remove("seed"); compact.Remove("testCaseName");
                    scenes[captureId] = new(Scene(options), DisturbanceText(options), Text("testCaseName"), Text("seed"),
                        generator.TryGetProperty("version", out var version) ? version.GetString() : null, compact.ToJsonString());
                }
                catch (JsonException) { }
                catch (IOException) { }
            }
        }
        return scenes;
    }

    private static IEnumerable<string> SafeEnumerate(string folder)
    {
        try { return Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories); }
        catch (IOException) { return []; }
        catch (UnauthorizedAccessException) { return []; }
    }

    private static string Scene(JsonElement options)
    {
        string Text(string key) => options.TryGetProperty(key, out var value)
            ? value.ValueKind switch
            {
                JsonValueKind.String => value.GetString() ?? "",
                JsonValueKind.True => "ja",
                JsonValueKind.False => "nein",
                JsonValueKind.Number => value.ToString(),
                _ => ""
            } : "";
        var parts = new List<string>();
        string orientation = Text("orientation"), position = Text("position"), count = Text("fieldCount");
        if (orientation.Length > 0) parts.Add(orientation == "Vertical" ? "senkrecht" : "waagerecht");
        if (position.Length > 0) parts.Add("Streifen " + (position switch
        {
            "Right" => "rechts", "Left" => "links", "Top" => "oben", "Bottom" => "unten", "Center" => "mittig", _ => position
        }));
        if (count.Length > 0) parts.Add(count + " Felder");
        if (Text("wallDifference") is { Length: > 0 } difference)
            parts.Add("Wand " + (difference == "Exact" ? "gleich Bezugsfeld" : difference.ToLowerInvariant()));
        return string.Join(", ", parts);
    }

    private static string DisturbanceText(JsonElement options)
    {
        var set = new List<string>();
        foreach (var (key, label, neutral) in Disturbances)
        {
            if (!options.TryGetProperty(key, out var value)) continue;
            string text = value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.ToString();
            if (text.Length == 0 || text == neutral || text == "null") continue;
            set.Add($"{label}: {Strength(text)}");
        }
        return set.Count > 0 ? string.Join(", ", set) : "keine Störung";
    }

    private static string Strength(string value) => value switch
    {
        "Light" => "leicht", "Medium" => "mittel", "Strong" => "stark",
        "Near" => "nahe", "Nearer" => "näher", "TooClose" => "ganz nahe",
        "Far" => "weit", "Farther" => "weiter", "TooFar" => "sehr weit",
        "1" => "+1 Blende", "-1" => "−1 Blende",
        _ => value
    };
}
