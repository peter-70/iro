using System.Text;
using Iro.Core.Analysis;
using static Iro.TestFixtures.ReflectionFixture;

namespace Iro.Core.Tests;

// Synthetic forward reflection overlays. Material colors are evaluation-only, never analyzer inputs.
public class ReflectionEvidenceTests
{
    static PixelRect Bounds(int i, bool horizontal)
    {
        var r = Field(i);
        return horizontal ? new(H - r.Bottom, r.X, r.Height, r.Width) : r;
    }
    static double Iou(PixelRect a, PixelRect b)
    {
        double overlap = Math.Max(0, Math.Min(a.Right, b.Right) - Math.Max(a.X, b.X)) * Math.Max(0, Math.Min(a.Bottom, b.Bottom) - Math.Max(a.Y, b.Y));
        return overlap / (a.Area + b.Area - overlap);
    }
    static double ColorError(FieldAnalysis f, bool horizontal, bool bright)
    {
        int index = Enumerable.Range(0, 3).MaxBy(i => Iou(f.Bounds, Bounds(i, horizontal)));
        var expected = ColorMath.LinearToLab(ColorMath.Decode(Material(index, 0, bright)), ColorMath.Decode(Material(index, 1, bright)), ColorMath.Decode(Material(index, 2, bright)));
        var wall = ColorMath.LinearToLab(ColorMath.Decode(140), ColorMath.Decode(140), ColorMath.Decode(140));
        return Math.Max(ColorMath.DeltaE00(expected, f.Measurement.Lab!.Value), ColorMath.DeltaE00(wall, f.Reference!.Lab!.Value));
    }
    [Theory]
    [InlineData(false, 0)] [InlineData(false, 2)] [InlineData(true, 0)] [InlineData(true, 2)]
    public void CleanAndBrightControlsKeepAllOriginalColors(bool horizontal, int noise)
    {
        foreach (bool bright in new[] { false, true })
        {
            var result = new ImageAnalyzer().Analyze(Make("Kontrolle", "Beide", horizontal, noise, bright), new());
            Assert.Equal(AnalysisStatus.Measured, result.Status);
            Assert.Equal(3, result.Fields.Count);
            Assert.All(result.Fields, f => { Assert.True(f.MeasurementAllowed); Assert.InRange(ColorError(f, horizontal, bright), 0, noise == 0 ? 1e-8 : .15); });
        }
    }
    [Theory]
    [InlineData(false, 0)] [InlineData(false, 2)] [InlineData(true, 0)] [InlineData(true, 2)]
    public void ReflectionBandMustNotBecomeAPerpendicularColorStrip(bool horizontal, int noise)
    {
        var result = new ImageAnalyzer().Analyze(Make("Lichtband", "Feld", horizontal, noise), new());
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
        Assert.False(string.IsNullOrWhiteSpace(result.Hint));
    }
    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void SameThreeRectanglesWithoutCompetingContinuationRemainMeasurable(bool horizontal)
    {
        // The same three rectangles can genuinely be a complete perpendicular
        // strip. Brightness alone must not trigger the new ambiguity rule.
        var result = new ImageAnalyzer().Analyze(Make("Lichtband", "Feld", horizontal, 0, isolatedBand: true), new());
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, f => Assert.True(f.MeasurementAllowed));
    }
    [Theory]
    [InlineData("Feld", false, 0)] [InlineData("Feld", false, 2)]
    [InlineData("Feld", true, 0)] [InlineData("Feld", true, 2)]
    [InlineData("Wand", false, 0)] [InlineData("Wand", false, 2)]
    [InlineData("Wand", true, 0)] [InlineData("Wand", true, 2)]
    [InlineData("Beide", false, 0)] [InlineData("Beide", false, 2)]
    [InlineData("Beide", true, 0)] [InlineData("Beide", true, 2)]
    public void UniformOverlayIsPixelIdenticalToPlausibleCleanMaterials(string target, bool horizontal, int noise)
    {
        var overlaid = Make("Gleichmäßiger Schleier", target, horizontal, noise);
        var cleanMaterials = MakeCleanEquivalentToUniformOverlay(target, horizontal, noise);

        Assert.Equal(overlaid.Width, cleanMaterials.Width);
        Assert.Equal(overlaid.Height, cleanMaterials.Height);
        Assert.Equal(overlaid.Pixels.ToArray(), cleanMaterials.Pixels.ToArray());

        var overlaidResult = new ImageAnalyzer().Analyze(overlaid, new());
        var cleanResult = new ImageAnalyzer().Analyze(cleanMaterials, new());
        Assert.Equal(AnalysisStatus.Measured, cleanResult.Status);
        Assert.Equal(cleanResult.Status, overlaidResult.Status);
        Assert.Equal(cleanResult.Hint, overlaidResult.Hint);
        Assert.All(cleanResult.Fields, field => Assert.True(field.MeasurementAllowed));
        Assert.Equal(
            cleanResult.Fields.Select(field => (field.Bounds, field.MeasurementAllowed, field.DeltaE00, field.IsNearest)),
            overlaidResult.Fields.Select(field => (field.Bounds, field.MeasurementAllowed, field.DeltaE00, field.IsNearest)));
    }
    [Fact]
    public void BroadInvestigationExportsEveryUnsafeReleaseWithoutClaimingAcceptance()
    {
        var report = new StringBuilder("# Reflexuntersuchung – tatsächliche Erkennung und Originalmessung\n\n");
        report.AppendLine($"Analyse {ImageAnalyzer.Version}; 108 gestörte Einzelbilder, neun Formen × Wand/Feld/beide × zwei Richtungen × Rauschen 0/±2; Seed 924137. Zusätzlich acht saubere/helle Kontrollbilder in separaten Pflichtprüfungen. Lineare Mischung von Materiallicht und weißem/farbigem Reflexlicht, keine Kamera-Simulation und kein Nachweis physikalischer Farbgenauigkeit.");
        report.AppendLine("Untersuchungserfolg ist keine erfüllte Schutzanforderung. Offene Fehlfreigaben werden unten ausdrücklich gezählt. Die digitale Grenze 1 ΔE00 je gemessener Feld-/Wandfarbe dient nur zum Auffinden deutlicher Verfälschungen; sie ist keine Produkt- oder Gerätefreigabe. Gleichmäßiger Schleier ohne sonstige Indizien ist aus unbekannten Materialfarben nicht eindeutig unterscheidbar. Keine Sollwerte gehen in Iro ein.\n");
        report.AppendLine("| Form | Ziel | Richtung | Rauschen | Erkannte / freie Felder | Größter Farbfehler | Fremde Geometrie freigegeben | Befund | Hinweis |\n|---|---|---|---:|---|---:|---|---|---|");
        int unsafeImages = 0, rejected = 0, safe = 0;
        var unexpectedFailures = new List<string>();
        foreach (string shape in Shapes) foreach (string target in new[] { "Feld", "Wand", "Beide" })
        foreach (bool horizontal in new[] { false, true }) foreach (int noise in new[] { 0, 2 })
        {
            var result = new ImageAnalyzer().Analyze(Make(shape, target, horizontal, noise), new());
            var allowed = result.Fields.Where(f => f.MeasurementAllowed).ToArray();
            double error = allowed.Select(f => ColorError(f, horizontal, false)).DefaultIfEmpty(0).Max();
            bool wrongGeometry = allowed.Any(f => Enumerable.Range(0, 3).Max(i => Iou(f.Bounds, Bounds(i, horizontal))) < .8);
            bool unsafeRelease = allowed.Length > 0 && (error > 1 || wrongGeometry);
            if (shape != "Gleichmäßiger Schleier" && unsafeRelease)
                unexpectedFailures.Add($"{shape}/{target}/{horizontal}/{noise}: verfälschte Farbe oder fremde Geometrie");
            if ((shape == "Punkt" || shape == "Mehrere Punkte") && (allowed.Length != 3 || result.Status != AnalysisStatus.Measured))
                unexpectedFailures.Add($"{shape}/{target}/{horizontal}/{noise}: kleine Glanzstellen verlieren geeignete Felder");
            if (unsafeRelease) unsafeImages++; else if (allowed.Length == 0) rejected++; else safe++;
            Assert.All(result.Fields.Where(f => !f.MeasurementAllowed), f => { Assert.Null(f.DeltaE00); Assert.False(f.IsNearest); });
            report.AppendLine($"| {shape} | {target} | {(horizontal ? "waagerecht" : "senkrecht")} | {noise} | {result.Fields.Count} / {allowed.Length} | {error:F3} | {wrongGeometry} | {(unsafeRelease ? "OFFENE FEHLFREIGABE" : allowed.Length == 0 ? "Gesperrt" : "Sichtbare Freigaben unverfälscht; Vollständigkeit separat prüfen")} | {result.Hint} |");
        }
        report.AppendLine($"\n**Offene Fehlfreigaben: {unsafeImages}; ohne Werte gesperrt: {rejected}; übrige Freigaben innerhalb digitaler Prüftoleranz: {safe}.**\nKeine allgemeine Schutzfreigabe oder Abnahme.");
        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        var folder = Path.Combine(root, "tests", "adjustments", "reflexe-20260924");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, $"untersuchung-{ImageAnalyzer.Version}.md"), report.ToString());
        Assert.True(unexpectedFailures.Count == 0, string.Join(Environment.NewLine, unexpectedFailures));
    }
}
