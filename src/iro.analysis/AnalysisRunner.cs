using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Iro.Core.Analysis;

namespace Iro.Analysis;

public sealed class AnalysisRunner(IPngAnalysisApi? api = null)
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    private readonly IPngAnalysisApi engine = api ?? new PngAnalysisApi();

    public async Task<SavedAnalysisRun> RunAsync(string requestFolder, string runsFolder, AnalysisOptions options,
        IProgress<AnalysisRunProgress>? progress = null, CancellationToken token = default)
    {
        options.Validate(); token.ThrowIfCancellationRequested();
        string root = Path.GetFullPath(requestFolder);
        using var requestDocument = ReadJson(SafePath(root, "request.json"));
        var request = requestDocument.RootElement;
        if (request.GetProperty("formatVersion").GetInt32() is not (1 or 2) || request.GetProperty("kind").GetString() != "iro-test-request")
            throw new ArgumentException("Nicht unterstützter Iro-Testauftrag.");
        string requestId = RequiredId(request, "requestId");
        string seriesPath = SafePath(root, request.GetProperty("seriesFile").GetString()!);
        using var seriesDocument = ReadJson(seriesPath);
        var series = seriesDocument.RootElement;
        if (series.GetProperty("formatVersion").GetInt32() != 1 || series.GetProperty("kind").GetString() != "iro-image-series")
            throw new ArgumentException("Nicht unterstützte Bildserie.");
        var entries = series.GetProperty("captures").EnumerateArray().Select(e => e.Clone()).ToArray();
        if (entries.Length is < 1 or > 10000 || entries.Length != request.GetProperty("imageCount").GetInt32()
            || entries.Length != series.GetProperty("actualCount").GetInt32() || entries.Length != series.GetProperty("requestedCount").GetInt32()
            || request.GetProperty("batchId").GetString() != series.GetProperty("batchId").GetString())
            throw new ArgumentException("Testauftrag und Serie sind unvollständig oder widersprüchlich.");
        string seriesRoot = Path.GetDirectoryName(seriesPath)!;
        var ids = new HashSet<string>();
        foreach (var entry in entries)
        {
            if (!ids.Add(RequiredId(entry, "captureId"))) throw new ArgumentException("Doppelte Aufnahme-ID im Testauftrag.");
            _ = SafePath(seriesRoot, entry.GetProperty("descriptionFile").GetString()!);
            _ = SafePath(seriesRoot, entry.GetProperty("imageFile").GetString()!);
        }
        string runId = "iro-run-" + Guid.NewGuid().ToString("N");
        string destination = Path.Combine(Path.GetFullPath(runsFolder), runId);
        var captures = new List<CaptureAnalysis>(); bool cancelled = false;
        foreach (var entry in entries)
        {
            if (token.IsCancellationRequested) { cancelled = true; break; }
            string captureId = RequiredId(entry, "captureId"), hash = "";
            ImageAnalysis? measured = null;
            BehaviorExpectation? expectation = null;
            try
            {
                string descriptionPath = SafePath(seriesRoot, entry.GetProperty("descriptionFile").GetString()!);
                string imagePath = SafePath(seriesRoot, entry.GetProperty("imageFile").GetString()!);
                using var description = ReadJson(descriptionPath);
                var metadata = description.RootElement;
                if (metadata.GetProperty("formatVersion").GetInt32() != 1 || metadata.GetProperty("captureId").GetString() != captureId
                    || metadata.GetProperty("colorSpace").GetString() != "sRGB"
                    || SafePath(Path.GetDirectoryName(descriptionPath)!, metadata.GetProperty("imageFile").GetString()!) != imagePath)
                    throw new ArgumentException("Aufnahme-ID, Bildpfad oder sRGB-Farbraum stimmt nicht überein.");
                if (metadata.TryGetProperty("conditions", out var conditions)
                    && conditions.TryGetProperty("generator", out var generator) && generator.ValueKind == JsonValueKind.Object
                    && generator.TryGetProperty("parameters", out var parameters)
                    && parameters.TryGetProperty("behaviorExpectation", out var expectationJson)
                    && expectationJson.ValueKind != JsonValueKind.Null)
                {
                    var candidateExpectation = expectationJson.Deserialize<BehaviorExpectation>(JsonOptions);
                    candidateExpectation?.Validate();
                    expectation = candidateExpectation;
                }
                var info = new FileInfo(imagePath);
                if (info.Length > 64 * 1024 * 1024) throw new ArgumentException("PNG-Datei überschreitet das Eingabelimit.");
                byte[] png = await File.ReadAllBytesAsync(imagePath, token).ConfigureAwait(false);
                hash = Convert.ToHexString(SHA256.HashData(png)).ToLowerInvariant();
                // This API boundary receives no metadata, masks, expected colors or expected field count.
                var analysis = await engine.AnalyzeAsync(png, options, token).ConfigureAwait(false);
                measured = analysis;
                if (analysis.Width != metadata.GetProperty("width").GetInt32() || analysis.Height != metadata.GetProperty("height").GetInt32())
                    throw new ArgumentException("PNG-Abmessungen passen nicht zur Aufnahmebeschreibung.");
                var comparisons = CompareAfterAnalysis(metadata, analysis);
                int unmatched = analysis.Fields.Count(f => comparisons.All(c => c.DetectedFieldId != f.FieldId));
                captures.Add(new(captureId, hash, null, analysis, comparisons, unmatched) { Expectation = expectation, Evaluation = ExpectationEvaluator.Evaluate(expectation, analysis) });
            }
            catch (OperationCanceledException) { cancelled = true; break; }
            catch (Exception error) when (error is IOException or ArgumentException or JsonException or InvalidOperationException or NotSupportedException or KeyNotFoundException or FormatException or UnauthorizedAccessException or System.Runtime.InteropServices.COMException)
            { captures.Add(new(captureId, hash, error.Message, measured, [], measured?.Fields.Count ?? 0) { Expectation = expectation, Evaluation = ExpectationEvaluator.Evaluate(expectation, measured, error.Message) }); }
            progress?.Report(new(captures.Count, entries.Length, captureId));
        }
        var report = new AnalysisRun(2, "iro-analysis-run", runId, requestId, ImageAnalyzer.Version, DateTime.UtcNow.ToString("O"), options,
            cancelled ? "cancelled" : captures.Any(c => c.Error != null) ? "completed-with-errors" : "completed", entries.Length, captures.Count,
            captures.Count(c => c.Analysis?.Fields.Any(f => f.MeasurementAllowed) == true), captures.Sum(c => c.Analysis?.Fields.Count(f => f.MeasurementAllowed) ?? 0),
            "Istwerte aus PNG-Pixeln; nominale Generatorwerte erst nach Analyse geometrisch zugeordnet. Abweichungen unter Störungen sind Diagnosen, keine automatischen Bestehen-/Durchfallen-Urteile.", captures);
        Directory.CreateDirectory(destination);
        string result = Path.Combine(destination, "results.json"), temporary = result + ".incomplete";
        try
        {
            await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(report, JsonOptions)).ConfigureAwait(false);
            File.Move(temporary, result);
            return new(report, result);
        }
        catch { if (File.Exists(temporary)) File.Delete(temporary); throw; }
    }

    public static string SafePath(string root, string relative)
    {
        if (string.IsNullOrWhiteSpace(relative) || Path.IsPathRooted(relative) || relative.Contains(':')) throw new ArgumentException("Nur relative Dateipfade sind zulässig.");
        string basePath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        string path = Path.GetFullPath(Path.Combine(basePath, relative));
        if (!path.StartsWith(basePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Dateipfad verlässt das Testpaket.");
        for (string? part = path; part != null && part.Length >= basePath.Length; part = Path.GetDirectoryName(part))
            if ((File.Exists(part) || Directory.Exists(part)) && (File.GetAttributes(part) & FileAttributes.ReparsePoint) != 0)
                throw new ArgumentException("Verknüpfungen innerhalb von Testpaketen werden nicht unterstützt.");
        return path;
    }
    private static JsonDocument ReadJson(string path)
    {
        if (new FileInfo(path).Length > 8 * 1024 * 1024) throw new ArgumentException("JSON-Datei überschreitet das Eingabelimit.");
        return JsonDocument.Parse(File.ReadAllBytes(path));
    }
    private static string RequiredId(JsonElement element, string name)
    {
        string? id = element.GetProperty(name).GetString();
        return id != null && Regex.IsMatch(id, "^[A-Za-z0-9][A-Za-z0-9._-]{0,150}$") ? id : throw new ArgumentException("Ungültige Test-/Aufnahme-ID.");
    }
    private static List<FieldComparison> CompareAfterAnalysis(JsonElement metadata, ImageAnalysis analysis)
    {
        var comparisons = new List<FieldComparison>();
        if (!metadata.TryGetProperty("conditions", out var conditions) || !conditions.TryGetProperty("generator", out var generator) || generator.ValueKind != JsonValueKind.Object
            || !generator.GetProperty("parameters").TryGetProperty("nominalValues", out var nominal)) return comparisons;
        var used = new HashSet<string>();
        foreach (var expected in nominal.EnumerateArray())
        {
            string fieldId = RequiredId(expected, "fieldId");
            double? distance = expected.TryGetProperty("deltaE00", out var d) && d.TryGetDouble(out double v) && double.IsFinite(v) && v >= 0 ? v : null;
            PixelRect? bounds = null;
            if (expected.TryGetProperty("bounds", out var box) && box.ValueKind == JsonValueKind.Object)
                bounds = new(box.GetProperty("x").GetInt32(), box.GetProperty("y").GetInt32(), box.GetProperty("width").GetInt32(), box.GetProperty("height").GetInt32());
            PixelPoint[]? polygon = expected.TryGetProperty("projectedCorners", out var corners) && corners.ValueKind == JsonValueKind.Array
                ? corners.EnumerateArray().Select(p => new PixelPoint(p.GetProperty("x").GetDouble(), p.GetProperty("y").GetDouble())).ToArray() : null;
            bool usablePolygon = polygon is { Length: >= 3 } && polygon.All(p => double.IsFinite(p.X) && double.IsFinite(p.Y));
            var matches = bounds == null ? [] : analysis.Fields.Select(f => (Field: f, Overlap: usablePolygon && f.Polygon != null
                ? PolygonGeometry.IntersectionOverUnion(polygon!, f.Polygon, analysis.Width, analysis.Height)
                : IoU(bounds.Value, f.Bounds)))
                .Where(m => m.Overlap >= .45).OrderByDescending(m => m.Overlap).ToArray();
            if (matches.Length == 0) { comparisons.Add(new(fieldId, null, null, distance, null, null, "not-detected")); continue; }
            if (matches.Length > 1 || !used.Add(matches[0].Field.FieldId))
            { comparisons.Add(new(fieldId, null, null, distance, null, null, "ambiguous-assignment")); continue; }
            var match = matches[0];
            comparisons.Add(new(fieldId, match.Field.FieldId, match.Overlap, distance, match.Field.DeltaE00,
                match.Field.DeltaE00 - distance, match.Field.MeasurementAllowed ? "measured" : "rejected"));
        }
        return comparisons;
    }
    private static double IoU(PixelRect a, PixelRect b)
    {
        double area = Math.Max(0, Math.Min(a.Right, b.Right) - Math.Max(a.X, b.X)) * (double)Math.Max(0, Math.Min(a.Bottom, b.Bottom) - Math.Max(a.Y, b.Y));
        return a.Area > 0 && b.Area > 0 ? area / (a.Area + b.Area - area) : 0;
    }
}

