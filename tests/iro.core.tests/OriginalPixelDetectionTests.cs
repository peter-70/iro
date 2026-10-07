using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

// New post-reset tests: MASTERPLAN 2.4–2.6 and the user's explicit removal decision.
// The scene geometry is test knowledge only; the analyzer receives just RGB pixels.
public sealed class OriginalPixelDetectionTests
{
    public static IEnumerable<object[]> RasterCases()
    {
        foreach (bool horizontal in new[] { false, true })
        foreach (int phase in new[] { 0, 1, 2, 3 })
            yield return [horizontal, phase];
    }

    [Theory]
    [MemberData(nameof(RasterCases))]
    public void SmallFieldsAreMeasuredWithoutSmoothingAmbiguity(bool horizontal, int phase)
    {
        var scene = Scene(48, phase, horizontal);
        var before = scene.Image.Pixels.ToArray();
        var result = new ImageAnalyzer().Analyze(scene.Image, new());

        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, field => Assert.True(field.MeasurementAllowed));
        Assert.True(result.Fields[1].IsNearest);
        Assert.Single(result.Fields, field => field.IsNearest);
        Assert.Equal(before, scene.Image.Pixels.ToArray());
    }

    [Theory]
    [MemberData(nameof(RasterCases))]
    public void FieldBoundsFollowOriginalEdgesAtTheDetectionGrid(bool horizontal, int phase)
    {
        var scene = Scene(180, phase, horizontal);
        var detection = StripDetector.Detect(scene.Image, new(), default);

        Assert.False(detection.Ambiguous);
        Assert.Equal(3, detection.Fields.Count);
        for (int i = 0; i < 3; i++)
        {
            // At scale 2, sample centers are odd coordinates. These even-sized
            // rectangles therefore quantize to the preceding even edge.
            var physical = scene.Fields[i];
            var expected = new PixelRect(physical.X & ~1, physical.Y & ~1,
                physical.Width, physical.Height);
            Assert.Equal(expected, detection.Fields[i]);
        }
    }

    [Theory]
    [MemberData(nameof(RasterCases))]
    public void FieldAndWallMeasurementsUseOriginalColors(bool horizontal, int phase)
    {
        var scene = Scene(180, phase, horizontal);
        var original = scene.Image.Pixels.ToArray();
        var result = new ImageAnalyzer().Analyze(scene.Image, new());

        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        for (int i = 0; i < 3; i++)
        {
            var field = result.Fields[i];
            Assert.True(field.MeasurementAllowed);
            Assert.NotNull(field.Measurement.Lab);
            Assert.NotNull(field.Reference);
            Assert.NotNull(field.Reference.Lab);
            AssertLab(ColorMath.ToLab(scene.Colors[i]), field.Measurement.Lab.Value);
            AssertLab(ColorMath.ToLab(new(120, 120, 120)), field.Reference.Lab.Value);
        }
        Assert.Equal(original, scene.Image.Pixels.ToArray());
    }

    [Theory]
    [InlineData(-17)]
    [InlineData(17)]
    public void RotationSamplesOriginalPixelsWithoutInterpolatedColors(double angle)
    {
        const int width = 40, height = 30;
        var bytes = new byte[width * height * 3];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int offset = (y * width + x) * 3;
            bytes[offset] = (byte)(x + 20);
            bytes[offset + 1] = (byte)(y + 30);
            bytes[offset + 2] = (byte)(x + 2 * y + 40);
        }
        var source = new RgbFrame(width, height, width * 3, bytes);
        var rotation = new ImageRotation(width, height, angle);
        var aligned = new RgbFrame(source, rotation);
        var selected = new HashSet<(int X, int Y)>();
        foreach (var sample in aligned.Sample(new(0, 0, aligned.Width, aligned.Height), 1, default))
        {
            var point = rotation.ToSource(sample.X, sample.Y);
            int x = (int)Math.Round(point.X - .5), y = (int)Math.Round(point.Y - .5);
            Assert.Equal(source.GetPixel(x, y), sample.Pixel);
            Assert.True(selected.Add((x, y)));
        }
        Assert.Equal(width * height, selected.Count);
        Assert.Equal(bytes, source.Pixels.ToArray());
    }

    private static void AssertLab(LabColor expected, LabColor actual)
    {
        // Floating-point accumulation tolerance only; not a product color threshold.
        Assert.InRange(Math.Abs(expected.L - actual.L), 0, 1e-9);
        Assert.InRange(Math.Abs(expected.A - actual.A), 0, 1e-9);
        Assert.InRange(Math.Abs(expected.B - actual.B), 0, 1e-9);
    }

    private static (RgbFrame Image, PixelRect[] Fields, RgbColor[] Colors) Scene(
        int fieldWidth, int phase, bool horizontal)
    {
        const int width = 1440, height = 960, fieldHeight = 110, gap = 40;
        int outputWidth = horizontal ? height : width;
        int outputHeight = horizontal ? width : height;
        var bytes = new byte[outputWidth * outputHeight * 3];
        Array.Fill(bytes, (byte)120);
        int x0 = 650 + phase, y0 = 230 + phase;
        byte[] values = [116, 123, 153];
        var fields = new PixelRect[3];
        for (int i = 0; i < 3; i++)
        {
            int fy = y0 + i * (fieldHeight + gap);
            for (int y = fy - 7; y < fy + fieldHeight + 7; y++)
            for (int x = x0 - 7; x < x0 + fieldWidth + 7; x++)
            {
                byte value = x >= x0 && x < x0 + fieldWidth && y >= fy && y < fy + fieldHeight
                    ? values[i] : (byte)65;
                int offset = horizontal ? (x * outputWidth + y) * 3 : (y * outputWidth + x) * 3;
                bytes[offset] = bytes[offset + 1] = bytes[offset + 2] = value;
            }
            fields[i] = horizontal ? new(fy, x0, fieldHeight, fieldWidth)
                : new(x0, fy, fieldWidth, fieldHeight);
        }
        return (new(outputWidth, outputHeight, outputWidth * 3, bytes), fields,
            values.Select(value => new RgbColor(value, value, value)).ToArray());
    }
}