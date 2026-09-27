using Iro.Core.Analysis;

namespace Iro.Core.Tests;

// Independent pixel fixtures. Exposure history is deliberately NOT an analyzer input.
public class UnderexposureEvidenceTests
{
    [Theory]
    [InlineData(false, 10)]
    [InlineData(true, 10)]
    [InlineData(false, 30)]
    [InlineData(true, 30)]
    public void PredominantlyDarkSceneWithResolvedFieldsIsNotRejectedForBrightness(bool horizontal, byte wall)
    {
        const int width = 800, height = 600;
        var pixels = Enumerable.Repeat(wall, width * height * 3).ToArray();
        byte[] shades = [60, 100, 160];
        for (int field = 0; field < shades.Length; field++)
        for (int y = 60 + field * 144; y < 190 + field * 144; y++)
        for (int x = 550; x < 710; x++)
        {
            int offset = (y * width + x) * 3;
            pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = shades[field];
        }
        RgbFrame frame;
        if (horizontal)
        {
            var rotated = new byte[pixels.Length];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                Array.Copy(pixels, (y * width + x) * 3, rotated, (x * height + height - 1 - y) * 3, 3);
            frame = new(height, width, height * 3, rotated);
        }
        else frame = new(width, height, width * 3, pixels);
        var result = new ImageAnalyzer().Analyze(frame, new());
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, field =>
        {
            Assert.True(field.MeasurementAllowed);
            Assert.NotNull(field.DeltaE00);
            var expectedWall = ColorMath.LinearToLab(ColorMath.Decode(wall), ColorMath.Decode(wall), ColorMath.Decode(wall));
            Assert.InRange(ColorMath.DeltaE00(expectedWall, field.Reference!.Lab!.Value), 0, 1e-10);
        });
    }

    [Theory]
    [InlineData(6)]
    [InlineData(10)]
    [InlineData(20)]
    public void UniformDarkOriginalPixelsRetainTheirMeasuredColor(byte shade)
    {
        var pixels = Enumerable.Repeat(shade, 80 * 80 * 3).ToArray();
        var snapshot = pixels.ToArray();
        var measurement = RegionSampler.Measure(new(80, 80, 240, pixels), new(0, 0, 80, 80), new());
        Assert.True(measurement.IsUsable);
        Assert.Equal(0, measurement.ChannelMad);
        Assert.Equal(0, measurement.NearLimitFraction);
        double linear = ColorMath.Decode(shade);
        Assert.InRange(ColorMath.DeltaE00(ColorMath.LinearToLab(linear, linear, linear), measurement.Lab!.Value), 0, 1e-10);
        Assert.Equal(snapshot, pixels);
    }

    [Fact]
    public void ExposureQuantizationCanLoseDistinctionsWithoutAnyEndpoint()
    {
        // Independent forward sRGB model: not an attempted inverse correction.
        static byte Expose(byte value)
        {
            double s = value / 255d;
            double linear = s <= .04045 ? s / 12.92 : Math.Pow((s + .055) / 1.055, 2.4);
            linear /= 256; // minus eight stops, deliberately outside any claimed valid camera range
            double encoded = linear <= .0031308 ? 12.92 * linear : 1.055 * Math.Pow(linear, 1 / 2.4) - .055;
            return (byte)Math.Round(encoded * 255);
        }
        byte a = Expose(100), b = Expose(101);
        Assert.InRange(a, (byte)2, (byte)253);
        Assert.Equal(a, b);
        Assert.NotEqual(ColorMath.Decode(100), ColorMath.Decode(101));
        var exposedA = Enumerable.Repeat(a, 80 * 80 * 3).ToArray();
        var exposedB = Enumerable.Repeat(b, 80 * 80 * 3).ToArray();
        // An originally uniform dark digital patch can produce exactly the same input.
        var darkOriginal = Enumerable.Repeat((byte)2, 80 * 80 * 3).ToArray();
        Assert.Equal(darkOriginal, exposedA);
        Assert.Equal(exposedA, exposedB);
        var measuredA = RegionSampler.Measure(new(80, 80, 240, exposedA), new(0, 0, 80, 80), new());
        var measuredB = RegionSampler.Measure(new(80, 80, 240, darkOriginal), new(0, 0, 80, 80), new());
        // Characterize indistinguishability, NOT an obligation to release an unsuitable frame.
        Assert.Equal(measuredA, measuredB);
        Assert.Equal(0, measuredA.NearLimitFraction);
    }
}