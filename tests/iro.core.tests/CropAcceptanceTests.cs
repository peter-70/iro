using Iro.Core.Analysis;
namespace Iro.Core.Tests;
public class CropAcceptanceTests
{
    [Theory]
    [InlineData(40, false, false)] [InlineData(41, false, false)] [InlineData(60, false, false)] [InlineData(110, false, false)]
    [InlineData(40, true, false)] [InlineData(41, true, false)] [InlineData(60, true, false)] [InlineData(110, true, false)]
    [InlineData(40, false, true)] [InlineData(41, false, true)] [InlineData(60, false, true)] [InlineData(110, false, true)]
    [InlineData(40, true, true)] [InlineData(41, true, true)] [InlineData(60, true, true)] [InlineData(110, true, true)]
    public void SafeEndCropUsesOriginalColorsAndAlwaysStatesPartialScope(int visible, bool turn, bool reverse)
    {
        foreach (int noise in new[] { 0, 2 })
        {
            var result = Run(visible, turn, reverse, noise);
            Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
            Assert.Equal(3, result.Fields.Count);
            Assert.All(result.Fields, f => Assert.True(f.MeasurementAllowed, f.Hint));
            Assert.Contains("Angeschnittener Streifen", result.Hint);
            Assert.Contains("kein vollständiger Streifenvergleich", result.Hint);
            bool descending = turn != reverse;
            for (int index = 0; index < 3; index++)
            {
                int field = descending ? 2 - index : index;
                var expected = ColorMath.LinearToLab(ColorMath.Decode((byte)(60 + field * 30)), ColorMath.Decode((byte)(75 + field * 30)), ColorMath.Decode((byte)(90 + field * 30)));
                Assert.InRange(ColorMath.DeltaE00(expected, result.Fields[index].Measurement.Lab!.Value), 0, noise == 0 ? 1e-9 : .15);
                var wall = ColorMath.LinearToLab(ColorMath.Decode(140), ColorMath.Decode(140), ColorMath.Decode(140));
                Assert.InRange(Math.Abs(result.Fields[index].DeltaE00!.Value - ColorMath.DeltaE00(expected, wall)), 0, noise == 0 ? 1e-9 : .15);
            }
            var nearest = Assert.Single(result.Fields, f => f.IsNearest);
            Assert.Equal(result.Fields.Min(f => f.DeltaE00), nearest.DeltaE00);
            var display = new AnalysisPresentation(); display.Present(result);
            Assert.Equal("Teilweise auswertbar", display.Heading);
            Assert.Equal(result.Hint, display.Message);
        }
    }
    [Theory]
    [InlineData(false, false)] [InlineData(false, true)] [InlineData(true, false)] [InlineData(true, true)]
    public void SmallerRemaindersAndUnsafeCropColorsNeverRelease(bool turn, bool reverse)
    {
        foreach (int visible in new[] { 6, 16, 24, 32, 39 }) Reject(Run(visible, turn, reverse, 0));
        foreach (string defect in new[] { "gradient", "clipped", "offset", "flecks", "wallclipped", "blur", "oneanchor", "twocrops" }) Reject(Run(60, turn, reverse, 0, defect));
    }
    [Fact]
    public void CropPolicyHonorsStricterConfiguredGeometryButKeepsItsSafetyFloor()
    {
        Reject(Run(60, false, false, 0, options: new() { MinimumFieldSide = 80 }));
        Assert.Equal(AnalysisStatus.PartiallyMeasured, Run(90, false, false, 0, options: new() { MinimumFieldSide = 80 }).Status);
        Reject(Run(39, false, false, 0, options: new() { MinimumFieldSide = 8, MinimumFieldArea = 64 }));
    }    private static void Reject(ImageAnalysis result) => Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
    private static ImageAnalysis Run(int visible, bool turn, bool reverse, int noise, string defect = "", AnalysisOptions? options = null)
    {
        const int w = 800, h = 600;
        var pixels = Enumerable.Repeat((byte)140, w * h * 3).ToArray();
        if (defect == "wallclipped") for (int i = 0; i < pixels.Length; i += 3) pixels[i] = 255;
        var random = new Random(82931);
        for (int field = 0; field < (defect == "oneanchor" ? 2 : defect == "twocrops" ? 4 : 3); field++)
        {
            int top = field == 0 ? 0 : visible + 18 + (field - 1) * 128;
            if (defect == "twocrops" && field == 2) top = 296;
            if (field == 3) top = h - visible;
            int fieldHeight = field == 0 || field == 3 ? visible : defect == "twocrops" ? (field == 1 ? 200 : 226) : 110;
            int left = field == 0 && defect == "offset" ? 550 : 540;
            for (int y = top; y < top + fieldHeight; y++)
            for (int x = left; x < left + 160; x++)
            for (int c = 0; c < 3; c++)
            {
                int value = 60 + field * 30 + c * 15 + (noise == 0 ? 0 : random.Next(-noise, noise + 1));
                if (field == 0 && defect == "gradient") value += (x - left) * 35 / 159;
                if (field == 0 && defect == "flecks" && (x + y) % 4 == 0) value = 220;
                if (field == 0 && defect == "clipped" && c == 0) value = 255;
                pixels[((reverse ? h - 1 - y : y) * w + x) * 3 + c] = (byte)value;
            }
        }
        if (defect == "blur")
        {
            var blurred = new byte[pixels.Length];
            const int radius = 25;
            for (int y = 0; y < h; y++)
            for (int c = 0; c < 3; c++)
            {
                int sum = 0, count = 0;
                for (int x = 0; x <= radius; x++) { sum += pixels[(y * w + x) * 3 + c]; count++; }
                for (int x = 0; x < w; x++)
                {
                    blurred[(y * w + x) * 3 + c] = (byte)(sum / count);
                    if (x - radius >= 0) { sum -= pixels[(y * w + x - radius) * 3 + c]; count--; }
                    if (x + radius + 1 < w) { sum += pixels[(y * w + x + radius + 1) * 3 + c]; count++; }
                }
            }
            pixels = blurred;
        }        if (!turn) return new ImageAnalyzer().Analyze(new(w, h, w * 3, pixels), options ?? new());
        var rotated = new byte[pixels.Length];
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
            Array.Copy(pixels, (y * w + x) * 3, rotated, (x * h + h - 1 - y) * 3, 3);
        return new ImageAnalyzer().Analyze(new(h, w, h * 3, rotated), options ?? new());
    }
}
