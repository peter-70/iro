using System.Text;
using Iro.Core.Analysis;

namespace Iro.Core.Tests;

/// <summary>Offline investigation only; no temporal release policy is installed in the app.</summary>
public class MeasurementStabilityInvestigationTests
{
    private const int Width = 800, Height = 600, FrameCount = 12;
    private static readonly string[] Scenarios =
        ["Unverändert", "Pixelrauschen ±2", "Pixelrauschen ±6", "Wechselnder Offset ±6", "Fester Offset +6", "Belichtungswechsel 0 bis −2 EV"];
    private sealed record Series(string Scenario, bool Horizontal, byte Wall, ImageAnalysis[] Frames);
    private static double Median(IEnumerable<double> source)
    {
        var values = source.Order().ToArray();
        return values.Length % 2 == 0 ? (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2 : values[values.Length / 2];
    }
    private static double Span(double[] values) => values.Max() - values.Min();

    [Fact]
    public void ControlledSequencesSeparateRepeatabilityFromSystematicChanges()
    {
        var series = new List<Series>();
        foreach (bool horizontal in new[] { false, true })
        foreach (byte wall in new byte[] { 10, 140 })
        foreach (string scenario in Scenarios)
        {
            var frames = Enumerable.Range(0, FrameCount)
                .Select(index => new ImageAnalyzer().Analyze(CreateFrame(horizontal, wall, scenario, index), new())).ToArray();
            // Mild, bounded disturbances deliberately preserve geometry; check the real detector too.
            Assert.All(frames, frame =>
            {
                Assert.Equal(AnalysisStatus.Measured, frame.Status);
                Assert.Equal(3, frame.Fields.Count);
                Assert.All(frame.Fields, field => Assert.True(field.MeasurementAllowed));
            });
            series.Add(new(scenario, horizontal, wall, frames));
        }
        var report = new StringBuilder("# Messstabilität: kontrollierte Bildfolgen\n\n");
        report.AppendLine("Analyse " + ImageAnalyzer.Version + "; 24 Folgen × 12 Frames = 288 Einzelbildanalysen. Keine Kamera-, Genauigkeits- oder Abnahmeprüfung.");
        report.AppendLine("Pro Frame werden Wand und Feld gemeinsam vom produktiven Analyzer gemessen. Feldzuordnung nur für diese feste Geometrie nach Lage; kein allgemeines Tracking.");
        report.AppendLine("Median, unskalierte MAD und Spannweite beziehen sich auf ΔE00 je Feld über zwölf Frames. Basisdifferenz = Betrag der Differenz zum Median derselben unveränderten digitalen Szene, kein realer Farbfehler.");
        report.AppendLine("Alle Felder aller Frames müssen freigegeben sein, sonst schlägt dieser begrenzte Versuch fehl; fehlende Werte werden weder als Null ersetzt noch still aussortiert. Keine universellen Freigabeschwellen.");
        report.AppendLine();
        report.AppendLine("| Folge | Richtung | Wand RGB | Feld | Werte | Median ΔE00 | MAD | Spannweite | Basisdifferenz |");
        report.AppendLine("|---|---|---:|---:|---:|---:|---:|---:|---:|");
        foreach (var current in series)
        {
            var baseline = series.Single(s => s.Scenario == Scenarios[0] && s.Horizontal == current.Horizontal && s.Wall == current.Wall);
            for (int field = 0; field < 3; field++)
            {
                var values = Values(current, field);
                double median = Median(values), deviation = Math.Abs(median - Median(Values(baseline, field)));
                double mad = Median(values.Select(v => Math.Abs(v - median)));
                report.AppendLine(FormattableString.Invariant($"| {current.Scenario} | {(current.Horizontal ? "waagerecht" : "senkrecht")} | {current.Wall} | {field + 1} | {values.Length} | {median:F6} | {mad:F6} | {Span(values):F6} | {deviation:F6} |"));
                if (current.Scenario == Scenarios[0]) Assert.Equal(0, Span(values));
                if (current.Scenario == Scenarios[4])
                {
                    Assert.Equal(0, Span(values));
                    Assert.True(deviation > 1e-4, "Stabile systematische Veränderung muss gegenüber der ursprünglichen digitalen Szene sichtbar sein.");
                }
                if (current.Scenario == Scenarios[3] || current.Scenario == Scenarios[5])
                    Assert.True(Span(values) > 1e-4, "Gemeinsame Änderung muss im Vergleich sichtbar bleiben können.");
            }
        }
        report.AppendLine();
        report.AppendLine("Feste gemeinsame Offsets sind zeitlich vollkommen stabil, verändern aber die Vergleiche. Gemeinsame Veränderungen heben sich in ΔE00 nicht allgemein auf.");
        report.AppendLine("Die Störmodelle sind kontrollierte digitale Eingaben: unabhängiges gleichverteiltes RGB-Codewertrauschen, gemeinsame additive Offsets bzw. lineare Belichtungsänderung. Keine kalibrierten Sensor-/ISP-Modelle.");
        report.AppendLine();
        report.AppendLine("## Fünfermedian als Offline-Versuch");
        report.AppendLine();
        report.AppendLine("Acht vollständige Fenster pro Folge, Ausgabe ab Frame 5. Vergleich mit den Rohwerten derselben Frames 5–12; kein Vorfüllen. Kein produktiver Glätter oder Freigabekriterium.");
        report.AppendLine();
        report.AppendLine("| Folge | Richtung | Wand RGB | Feld | Roh-MAD ab Frame 5 | Median-MAD | Maximale Abweichung vom aktuellen Rohwert |");
        report.AppendLine("|---|---|---:|---:|---:|---:|---:|");
        foreach (var current in series)
        for (int field = 0; field < 3; field++)
        {
            var values = Values(current, field);
            var raw = values.Skip(4).ToArray();
            var smooth = Enumerable.Range(4, values.Length - 4).Select(i => Median(values.Skip(i - 4).Take(5))).ToArray();
            double rawCenter = Median(raw), smoothCenter = Median(smooth);
            double rawMad = Median(raw.Select(v => Math.Abs(v - rawCenter)));
            double smoothMad = Median(smooth.Select(v => Math.Abs(v - smoothCenter)));
            double lag = raw.Zip(smooth, (a, b) => Math.Abs(a - b)).Max();
            if (current.Scenario == Scenarios[4]) Assert.Equal(raw, smooth);
            if (current.Scenario == Scenarios[5]) Assert.True(lag > 1e-4);
            report.AppendLine(FormattableString.Invariant($"| {current.Scenario} | {(current.Horizontal ? "waagerecht" : "senkrecht")} | {current.Wall} | {field + 1} | {rawMad:F6} | {smoothMad:F6} | {lag:F6} |"));
        }
        report.AppendLine();
        report.AppendLine("Der Median beseitigt den festen Offset nicht. Bei Belichtungsänderung weicht sein Ergebnis vom aktuellen Frame ab; diese Verzögerung darf nicht als erfolgreiche Stabilisierung gewertet werden. Änderungen tatsächlicher Aufnahmeparameter brauchen weiterhin einen Historienwechsel.");
        string project = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(project, "iro.slnx")))
            project = Directory.GetParent(project)?.FullName ?? throw new InvalidOperationException("Projektstamm fehlt.");
        string folder = Path.Combine(project, "tests", "adjustments", "messstabilitaet-20260923");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "bericht.md"), report.ToString());
    }

    private static double[] Values(Series series, int field) => series.Frames.Select(frame =>
        // Fixed geometry allows spatial association independent of detected field IDs.
        frame.Fields.OrderBy(f => series.Horizontal ? f.Bounds.X : f.Bounds.Y).ElementAt(field).DeltaE00!.Value).ToArray();

    [Fact]
    public void SameFramePairingPreservesEqualityWhileShiftedPairingInventsDifferences()
    {
        var pairs = Enumerable.Range(0, FrameCount).Select(index =>
        {
            byte value = (byte)(index % 2 == 0 ? 20 : 40);
            var frame = new RgbFrame(160, 80, 480, Enumerable.Repeat(value, 160 * 80 * 3).ToArray());
            // Fixed masks isolate pairing from detection; identical areas contain no detectable strip.
            var wall = RegionSampler.Measure(frame, new(0, 0, 80, 80), new());
            var field = RegionSampler.Measure(frame, new(80, 0, 80, 80), new());
            Assert.True(wall.IsUsable && field.IsUsable);
            return (Wall: wall.Lab!.Value, Field: field.Lab!.Value);
        }).ToArray();
        Assert.All(pairs, pair => Assert.Equal(0, ColorMath.DeltaE00(pair.Wall, pair.Field)));
        for (int index = 1; index < pairs.Length; index++)
            Assert.True(ColorMath.DeltaE00(pairs[index - 1].Wall, pairs[index].Field) > 1);
    }

    [Fact]
    public void InvalidFrameAndRecoveryDoNotReuseEarlierSingleFrameValues()
    {
        var analyzer = new ImageAnalyzer();
        var before = analyzer.Analyze(CreateFrame(false, 140, Scenarios[0], 0), new());
        var invalid = analyzer.Analyze(new(Width, Height, Width * 3, new byte[Width * Height * 3]), new());
        Assert.DoesNotContain(invalid.Fields, field => field.MeasurementAllowed || field.DeltaE00.HasValue || field.IsNearest);
        var after = analyzer.Analyze(CreateFrame(false, 140, Scenarios[0], 0), new());
        Assert.Equal(before.Fields.Select(f => f.DeltaE00), after.Fields.Select(f => f.DeltaE00));
        var presentation = new AnalysisPresentation(analyzer);
        presentation.Present(before);
        Assert.Equal(3, presentation.Fields.Count);
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));
        presentation.Present(invalid);
        Assert.Empty(presentation.Fields);
        presentation.Present(after);
        Assert.Equal(3, presentation.Fields.Count);
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));
        // Product presentation state is covered; no temporal smoothing/history implementation is claimed.
    }

    private static RgbFrame CreateFrame(bool horizontal, byte wall, string scenario, int index)
    {
        var data = Enumerable.Repeat(wall, Width * Height * 3).ToArray();
        RgbColor[] colors = [new(60, 80, 110), new(90, 110, 140), new(120, 140, 170)];
        for (int field = 0; field < 3; field++)
        for (int y = 60 + field * 144; y < 190 + field * 144; y++)
        for (int x = 550; x < 710; x++)
        {
            int offset = (y * Width + x) * 3;
            data[offset] = colors[field].R; data[offset + 1] = colors[field].G; data[offset + 2] = colors[field].B;
        }
        var random = new Random(12345 + index);
        int noise = scenario == Scenarios[1] ? 2 : scenario == Scenarios[2] ? 6 : 0;
        int shift = scenario == Scenarios[3] ? (index % 2 == 0 ? -6 : 6) : scenario == Scenarios[4] ? 6 : 0;
        double exposure = scenario == Scenarios[5] ? Math.Pow(2, -2d * index / (FrameCount - 1)) : 1;
        for (int p = 0; p < data.Length; p++)
        {
            double value = data[p];
            if (exposure != 1)
            {
                // Independent forward transform; no original colors or exposure supplied to analyzer.
                double s = value / 255;
                double linear = (s <= .04045 ? s / 12.92 : Math.Pow((s + .055) / 1.055, 2.4)) * exposure;
                value = 255 * (linear <= .0031308 ? 12.92 * linear : 1.055 * Math.Pow(linear, 1 / 2.4) - .055);
            }
            value += shift + (noise == 0 ? 0 : random.Next(-noise, noise + 1));
            Assert.InRange(value, 1, 254);
            data[p] = (byte)Math.Round(value);
        }
        if (!horizontal) return new(Width, Height, Width * 3, data);
        var rotated = new byte[data.Length];
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
            Array.Copy(data, (y * Width + x) * 3, rotated, ((Width - 1 - x) * Height + y) * 3, 3);
        // x'=y, y'=Width-1-x; therefore long-axis order is increasing x.
        return new(Height, Width, Height * 3, rotated);
    }
}