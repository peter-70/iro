using System.Text;
using Iro.Core.Analysis;
namespace Iro.Core.Tests;

public class CropEvidenceTests
{
    [Fact]
    public void VisibleRemaindersSeparateUsablePixelsFromCurrentCropPolicy()
    {
        var report = new StringBuilder("# Beschnitt: Restfläche und bestehende Sperre\n\n");
        report.AppendLine("Analyse " + ImageAnalyzer.Version + ". 64 synthetische Bilder: acht Resthöhen, zwei Richtungen, vier Oberflächen. Der Analyzer bekommt nur Pixel. Eine separat bekannte Messmaske dient ausschließlich der Untersuchung lokaler Messbarkeit und wird NICHT in die produktive Erkennung eingespeist. Keine allgemeine Beschnittfreigabe.");
        report.AppendLine("| Resthöhe | Richtung | Oberfläche | Samples innen | Innen geeignet | Größere Fläche geeignet | Farbfehler ΔE00 | App-Freigaben | App-Status |");
        report.AppendLine("|---:|---|---|---:|---|---|---:|---:|---|");
        foreach (int visible in new[] { 6, 16, 24, 32, 40, 60, 90, 110 })
        foreach (bool horizontal in new[] { false, true })
        foreach (string surface in new[] { "Homogen", "Rauschen", "Verlauf", "Kanalanschlag" })
        {
            const int w = 800, h = 600;
            var data = Enumerable.Repeat((byte)140, w * h * 3).ToArray();
            var random = new Random(61927);
            for (int field = 0; field < 3; field++)
            {
                int top = field == 0 ? 0 : visible + 18 + (field - 1) * 128;
                for (int y = top; y < top + (field == 0 ? visible : 110); y++)
                for (int x = 540; x < 700; x++)
                for (int c = 0; c < 3; c++)
                {
                    int value = 60 + 30 * field + 15 * c;
                    if (field == 0)
                    {
                        if (surface == "Rauschen") value += random.Next(-2, 3);
                        if (surface == "Verlauf") value += (x - 540) * 35 / 159;
                        if (surface == "Kanalanschlag" && c == 0) value = 255;
                    }
                    data[(y * w + x) * 3 + c] = (byte)value;
                }
            }
            PixelRect bounds = new(540, 0, 160, visible);
            RgbFrame frame;
            if (horizontal)
            {
                var turned = new byte[data.Length];
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    Array.Copy(data, (y * w + x) * 3, turned, (x * h + h - 1 - y) * 3, 3);
                frame = new(h, w, h * 3, turned);
                bounds = new(h - visible, 540, visible, 160);
            }
            else frame = new(w, h, w * 3, data);
            var options = new AnalysisOptions();
            var inner = RegionSampler.Measure(frame, bounds.Inset(options.InnerMargin), options);
            var larger = RegionSampler.Measure(frame, bounds.Inset(options.SurfaceMargin), options);
            var result = new ImageAnalyzer().Analyze(frame, options);
            double? error = inner.Lab is { } lab ? ColorMath.DeltaE00(lab, ColorMath.LinearToLab(ColorMath.Decode(60), ColorMath.Decode(75), ColorMath.Decode(90))) : null;
            if ((surface == "Homogen" || surface == "Rauschen") && visible >= 40)
            {
                Assert.True(inner.IsUsable && larger.IsUsable);
                Assert.InRange(error!.Value, 0, surface == "Homogen" ? 1e-9 : .15);
            }
            if (visible <= 16 || surface == "Kanalanschlag") Assert.False(inner.IsUsable);
            if (surface == "Verlauf" && visible >= 40) Assert.False(larger.IsUsable);
            if (surface == "Verlauf") Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
            report.AppendLine(FormattableString.Invariant($"| {visible} | {(horizontal ? "waagerecht" : "senkrecht")} | {surface} | {inner.SampleCount} | {inner.IsUsable} | {larger.IsUsable} | {error:F6} | {result.Fields.Count(f => f.MeasurementAllowed)} | {result.Status} |"));
        }
        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        string folder = Path.Combine(root, "tests", "adjustments", "beschnitt-20260924");
        Directory.CreateDirectory(folder);
        File.WriteAllText(Path.Combine(folder, "bericht.md"), report.ToString());
    }

    [Fact]
    public void HiddenBestFieldCannotBeInferredFromIdenticalVisiblePixels()
    {
        const int w = 100, h = 180, removed = 60;
        byte[] Scene(byte hidden)
        {
            var data = Enumerable.Repeat((byte)140, w * h * 3).ToArray();
            for (int y = 10; y < 50; y++)
            for (int x = 20; x < 80; x++)
            for (int c = 0; c < 3; c++) data[(y * w + x) * 3 + c] = hidden;
            for (int field = 0; field < 2; field++)
            for (int y = 70 + field * 55; y < 110 + field * 55; y++)
            for (int x = 20; x < 80; x++)
            for (int c = 0; c < 3; c++) data[(y * w + x) * 3 + c] = (byte)(90 + field * 30);
            return data;
        }
        var a = Scene(140); var b = Scene(60);
        Assert.False(a.SequenceEqual(b));
        Assert.Equal(a.Skip(w * removed * 3), b.Skip(w * removed * 3));
    }
}
