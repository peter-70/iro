using System.Text;
using Iro.Core.Analysis;

namespace Iro.Core.Tests;

// Forward illumination fixtures, not a correction algorithm or a camera model.
public class IlluminationEvidenceTests
{
    private const int W = 800, H = 600;
    private static readonly string[] Cases = ["Gleichmäßig", "Gemeinsam halb so hell", "Sanfter Verlauf", "Starker Querverlauf", "Starker Längsverlauf", "Schattenkante durch Messflächen", "Farbiges Lichtgefälle", "Schatten zwischen Wand und Streifen"];

    private static byte Encode(double linear)
    {
        double s = linear <= .0031308 ? 12.92 * linear : 1.055 * Math.Pow(linear, 1 / 2.4) - .055;
        return (byte)Math.Clamp((int)Math.Round(s * 255), 0, 255);
    }
    private static double Decode(byte value)
    {
        double s = value / 255d;
        return s <= .04045 ? s / 12.92 : Math.Pow((s + .055) / 1.055, 2.4);
    }
    private static double Light(string scenario, int x, int y, int channel) => scenario switch
    {
        "Gemeinsam halb so hell" => .5,
        "Sanfter Verlauf" => .97 + .03 * x / (W - 1d),
        "Starker Querverlauf" => .2 + .8 * x / (W - 1d),
        "Starker Längsverlauf" => .2 + .8 * y / (H - 1d),
        "Schattenkante durch Messflächen" => y < 268 ? .35 : 1,
        "Farbiges Lichtgefälle" => channel == 0 ? .3 + .7 * x / (W - 1d) : channel == 2 ? 1 - .7 * x / (W - 1d) : .7,
        "Schatten zwischen Wand und Streifen" => x < 540 ? .35 : 1,
        _ => 1
    };
    private static byte[] Pixels(string scenario, int noise, int seed = 72931)
    {
        byte[] shades = [60, 90, 120];
        var data = new byte[W * H * 3];
        var random = new Random(seed);
        for (int y = 0; y < H; y++)
        for (int x = 0; x < W; x++)
        {
            int field = x >= 550 && x < 710 ? (y - 60) / 144 : -1;
            bool inside = y >= 60 && field >= 0 && field < 3 && y < 60 + field * 144 + 130;
            for (int c = 0; c < 3; c++)
            {
                byte material = inside ? (byte)(shades[field] + c * 15) : (byte)140;
                int value = Encode(Decode(material) * Light(scenario, x, y, c));
                data[(y * W + x) * 3 + c] = (byte)Math.Clamp(value + (noise == 0 ? 0 : random.Next(-noise, noise + 1)), 0, 255);
            }
        }
        return data;
    }
    private static RgbFrame Frame(byte[] data, bool horizontal)
    {
        if (!horizontal) return new(W, H, W * 3, data);
        var rotated = new byte[data.Length];
        for (int y = 0; y < H; y++)
        for (int x = 0; x < W; x++)
            Array.Copy(data, (y * W + x) * 3, rotated, (x * H + H - 1 - y) * 3, 3);
        return new(H, W, H * 3, rotated);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 2)]
    [InlineData(true, 0)]
    [InlineData(true, 2)]
    public void StrongCrossGradientNeverPublishesPartialMeasurements(bool horizontal, int noise)
    {
        var result = new ImageAnalyzer().Analyze(Frame(Pixels(Cases[3], noise), horizontal), new());
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
        Assert.False(string.IsNullOrWhiteSpace(result.Hint));
        // Some gradients prevent field detection before any surface can be measured.
        Assert.Equal(result.Fields.Count == 0 ? AnalysisHintCode.Other : AnalysisHintCode.UnevenSurface, result.HintCode);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 2)]
    [InlineData(true, 0)]
    [InlineData(true, 2)]
    public void UniformDifferentMaterialColorsAndGentleGradientRemainMeasurable(bool horizontal, int noise)
    {
        foreach (var scenario in new[] { Cases[0], Cases[2] })
        {
            var result = new ImageAnalyzer().Analyze(Frame(Pixels(scenario, noise), horizontal), new());
            Assert.Equal(AnalysisStatus.Measured, result.Status);
            Assert.Equal(3, result.Fields.Count);
            Assert.All(result.Fields, f => Assert.True(f.MeasurementAllowed));
        }
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DimNoisyHorizontalControlRetainsEveryField(bool straighten)
    {
        var result = new ImageAnalyzer().Analyze(Frame(Pixels(Cases[1], 2), true), new() { Straighten = straighten });
        Assert.True(result.Fields.Count == 3,
            $"Expected 3 fields, got {result.Fields.Count}; angle={result.StraighteningDegrees}; " + string.Join("; ", result.Fields.Select(f => f.Bounds)));
        Assert.All(result.Fields, f => Assert.True(f.MeasurementAllowed, f.Hint));
    }
    [Theory]
    [InlineData(false, 72931)]
    [InlineData(true, 72931)]
    [InlineData(false, 12345)]
    [InlineData(true, 12345)]
    [InlineData(false, 91763)]
    [InlineData(true, 91763)]
    [InlineData(false, 40117)]
    [InlineData(true, 40117)]
    public void DimControlsKeepRealGeometryAndOriginalColorsAcrossNoiseSeeds(bool horizontal, int seed)
    {
        foreach (int noise in new[] { 0, 1, 2 })
        {
            var data = Pixels(Cases[1], noise, seed);
            var original = data.ToArray();
            var result = new ImageAnalyzer().Analyze(Frame(data, horizontal), new());
            Assert.Equal(original, data);
            Assert.Equal(AnalysisStatus.Measured, result.Status);
            Assert.Equal(3, result.Fields.Count);
            for (int index = 0; index < 3; index++)
            {
                int materialIndex = horizontal ? 2 - index : index;
                var measured = result.Fields[index];
                var expected = horizontal ? new PixelRect(H - (60 + materialIndex * 144 + 130), 550, 130, 160)
                    : new PixelRect(550, 60 + materialIndex * 144, 160, 130);
                Assert.InRange(Math.Abs(measured.Bounds.X - expected.X), 0, 2);
                Assert.InRange(Math.Abs(measured.Bounds.Y - expected.Y), 0, 2);
                Assert.InRange(Math.Abs(measured.Bounds.Right - expected.Right), 0, 2);
                Assert.InRange(Math.Abs(measured.Bounds.Bottom - expected.Bottom), 0, 2);
                double Channel(int c) => Decode(Encode(Decode((byte)(60 + 30 * materialIndex + c * 15)) * .5));
                var expectedLab = ColorMath.LinearToLab(Channel(0), Channel(1), Channel(2));
                Assert.True(measured.MeasurementAllowed, measured.Hint);
                // Digital fixture tolerance, not a physical color accuracy promise.
                Assert.InRange(ColorMath.DeltaE00(expectedLab, measured.Measurement.Lab!.Value), 0, noise == 0 ? 1e-9 : .15);
            }
        }
    }
    [Fact]
    public void FullImageInvestigationReportsReleasesWithoutCallingThemLightingSuccess()
    {
        var report = new StringBuilder();
        var controlFailures = new List<string>();
        report.AppendLine("# Beleuchtung: unabhängige Vorwärtsmodelle");
        report.AppendLine();
        report.AppendLine("Analyse " + ImageAnalyzer.Version + "; 32 Einzelbilder: acht Lichtfelder, zwei Richtungen, ohne/mit Rauschen ±2; Seed 72931. Lineares RGB mal ortsabhängigem Lichtfaktor, dann sRGB und Quantisierung. Alle drei Materialfelder sind homogen. Keine Rekonstruktion, kein Kamera- oder Genauigkeitsnachweis.");
        report.AppendLine("Nur gleichmäßige Kontrollen und der konkret sanfte Verlauf besitzen hier eine vorab geforderte vollständige Freigabe. Gestörte Fälle werden ergebnisoffen untersucht; ein bestandener Untersuchungstest bedeutet ausdrücklich NICHT, dass deren Freigaben richtig sind. Detektion und Messung verwenden keinerlei Material- oder Licht-Sollwerte.");
        report.AppendLine();
        report.AppendLine("| Lichtfeld | Richtung | Rauschen | Erkannte Felder | Freigegeben | Status | Hinweis |");
        report.AppendLine("|---|---|---:|---:|---:|---|---|");
        foreach (var scenario in Cases)
        foreach (bool horizontal in new[] { false, true })
        foreach (int noise in new[] { 0, 2 })
        {
            var data = Pixels(scenario, noise);
            var original = data.ToArray();
            var result = new ImageAnalyzer().Analyze(Frame(data, horizontal), new());
            Assert.Equal(original, data);
            int released = result.Fields.Count(f => f.MeasurementAllowed);
            Assert.All(result.Fields.Where(f => !f.MeasurementAllowed), f => { Assert.Null(f.DeltaE00); Assert.False(f.IsNearest); });
            if (scenario == Cases[0] || scenario == Cases[1] || scenario == Cases[2])
            {
                if (result.Status != AnalysisStatus.Measured || released != 3)
                    controlFailures.Add($"{scenario}; horizontal={horizontal}; noise={noise}: {result.Status}, {released}/3");
            }
            report.AppendLine($"| {scenario} | {(horizontal ? "waagerecht" : "senkrecht")} | {noise} | {result.Fields.Count} | {released} | {result.Status} | {result.Hint} |");
        }
        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        string folder = Path.Combine(root, "tests", "adjustments", "felderkennung-20260924");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "bericht.md"), report.ToString());
        // Investigation includes unresolved findings; do not disguise these as passed expectations.
        File.AppendAllText(Path.Combine(folder, "bericht.md"), "\n## Nicht erfüllte Kontrollerwartungen\n\n" + (controlFailures.Count == 0 ? "Keine." : string.Join("\n", controlFailures.Select(f => "- " + f))) + "\n");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShadowBetweenUniformMaterialsIsIndistinguishableFromDifferentMaterialColors(bool horizontal)
    {
        var shadow = Pixels(Cases[7], 0);
        // Alternative physical interpretation: these observed colors ARE the materials,
        // under uniform light. No additional metadata reaches the analyzer.
        var alternative = new byte[shadow.Length];
        for (int i = 0; i < shadow.Length; i++) alternative[i] = Encode(Decode(shadow[i]) * 1);
        Assert.Equal(shadow, alternative);
        var a = new ImageAnalyzer().Analyze(Frame(shadow, horizontal), new());
        var b = new ImageAnalyzer().Analyze(Frame(alternative, horizontal), new());
        Assert.Equal(a.Status, b.Status);
        Assert.Equal(a.Hint, b.Hint);
        Assert.Equal(a.Fields.Select(f => f.DeltaE00), b.Fields.Select(f => f.DeltaE00));
        // No release assertion: uncertainty must never become a requirement to release.
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void StrongWithinSurfaceVariationIsRejectedWithoutDiagnosingItsCause(bool tinted)
    {
        const int width = 120, height = 90;
        var data = new byte[width * height * 3];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        for (int c = 0; c < 3; c++)
        {
            double factor = tinted && c == 2 ? 1 - .7 * x / (width - 1d) : .3 + .7 * x / (width - 1d);
            data[(y * width + x) * 3 + c] = Encode(Decode(140) * factor);
        }
        var result = RegionSampler.Measure(new(width, height, width * 3, data), new(0, 0, width, height), new());
        Assert.False(result.IsUsable);
        Assert.Null(result.Lab);
        Assert.True(result.SpatialDeltaE > 2);
        Assert.Contains("räumlich ungleichmäßig", result.Reason);
    }
}
