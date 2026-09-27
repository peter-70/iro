using Iro.Core.Analysis;

namespace Iro.Core.Tests;

public class BlurSafetyTests
{
    private const int W = 800, H = 600;
    private static byte[] Scene()
    {
        var data = Enumerable.Repeat((byte)140, W * H * 3).ToArray();
        for (int f = 0; f < 3; f++)
        for (int y = 60 + f * 144; y < 190 + f * 144; y++)
        for (int x = 550; x < 710; x++)
        {
            int p = (y * W + x) * 3;
            data[p] = (byte)(60 + 30 * f);
            data[p + 1] = (byte)(80 + 30 * f);
            data[p + 2] = (byte)(110 + 30 * f);
        }
        return data;
    }
    private static byte[] Blur(byte[] source, int radius, bool horizontal)
    {
        var output = new byte[source.Length];
        int lines = horizontal ? H : W, length = horizontal ? W : H;
        for (int line = 0; line < lines; line++)
        for (int c = 0; c < 3; c++)
        {
            int Offset(int p) => (horizontal ? line * W + p : p * W + line) * 3 + c;
            int sum = 0;
            for (int d = -radius; d <= radius; d++) sum += source[Offset(Math.Clamp(d, 0, length - 1))];
            for (int p = 0; p < length; p++)
            {
                output[Offset(p)] = (byte)(sum / (radius * 2 + 1));
                sum -= source[Offset(Math.Clamp(p - radius, 0, length - 1))];
                sum += source[Offset(Math.Clamp(p + radius + 1, 0, length - 1))];
            }
        }
        return output;
    }
    private static void AddNoise(byte[] data, int amplitude, int seed = 12345)
    {
        var random = new Random(seed);
        for (int p = 0; p < data.Length; p++)
            data[p] = (byte)Math.Clamp(data[p] + random.Next(-amplitude, amplitude + 1), 0, 255);
    }
    private static ImageAnalysis Analyze(byte[] data, bool turned)
    {
        if (!turned) return new ImageAnalyzer().Analyze(new(W, H, W * 3, data), new());
        var output = new byte[data.Length];
        for (int y = 0; y < H; y++)
        for (int x = 0; x < W; x++)
            Array.Copy(data, (y * W + x) * 3, output, (x * H + H - 1 - y) * 3, 3);
        return new ImageAnalyzer().Analyze(new(H, W, H * 3, output), new());
    }

    [Theory]
    [InlineData(false, false, 0)]
    [InlineData(false, false, 6)]
    [InlineData(false, true, 0)]
    [InlineData(false, true, 6)]
    [InlineData(true, false, 0)]
    [InlineData(true, false, 6)]
    [InlineData(true, true, 0)]
    [InlineData(true, true, 6)]
    public void StrongSmearingCannotBeHiddenByNoise(bool turned, bool defocus, int noise)
    {
        var data = Blur(Scene(), 25, true);
        if (defocus) data = Blur(data, 25, false);
        // Noise after optical smearing: independent of the generator's noise-before-blur order.
        if (noise > 0) AddNoise(data, noise);
        var result = Analyze(data, turned);
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00.HasValue || f.IsNearest);
        Assert.Contains("unscharf", result.Hint);
        Assert.Equal(AnalysisHintCode.UnusableBlur, result.HintCode);
    }

    [Theory]
    [InlineData(false, 0)]
    [InlineData(false, 6)]
    [InlineData(true, 0)]
    [InlineData(true, 6)]
    public void SharpEdgesWithTheSameNoiseRemainMeasurable(bool turned, int noise)
    {
        var data = Scene();
        if (noise > 0) AddNoise(data, noise);
        var result = Analyze(data, turned);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count(f => f.MeasurementAllowed));
    }
    [Fact]
    public void AdditionalIndependentNoiseSeedsCannotRestoreSmearedFrames()
    {
        foreach (int seed in new[] { 24680, 97531 })
        foreach (bool turned in new[] { false, true })
        foreach (bool defocus in new[] { false, true })
        {
            var data = Blur(Scene(), 25, true);
            if (defocus) data = Blur(data, 25, false);
            AddNoise(data, 6, seed);
            var result = Analyze(data, turned);
            Assert.True(result.Fields.All(f => !f.MeasurementAllowed && !f.DeltaE00.HasValue && !f.IsNearest),
                $"Fehlfreigabe bei Seed {seed}, gedreht={turned}, Defokus={defocus}.");
            // Pattern/area-quality gates may reject before a blur cause is established.
            Assert.False(string.IsNullOrWhiteSpace(result.Hint));
            Assert.DoesNotContain(result.Status, new[] { AnalysisStatus.Measured, AnalysisStatus.PartiallyMeasured });
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void SlightEdgeSofteningWithUnchangedInteriorsRemainsMeasurable(bool turned, bool defocus)
    {
        var data = Blur(Scene(), 1, true);
        if (defocus) data = Blur(data, 1, false);
        var result = Analyze(data, turned);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        foreach (var color in new RgbColor[] { new(60, 80, 110), new(90, 110, 140), new(120, 140, 170) })
            Assert.Contains(result.Fields, f => f.MeasurementAllowed &&
                ColorMath.DeltaE00(ColorMath.ToLab(color), f.Measurement.Lab!.Value) < 1e-9);
    }
}