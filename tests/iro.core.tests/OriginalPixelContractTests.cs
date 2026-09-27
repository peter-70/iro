using System.Runtime.InteropServices;
using Iro.Core.Analysis;

namespace Iro.Core.Tests;

public class OriginalPixelContractTests
{
    static byte[] Pattern(int w, int h, int stride)
    {
        var bytes = new byte[stride * h];
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            int i = y * stride + x * 3;
            bytes[i] = (byte)(100 + x % 3); bytes[i + 1] = (byte)(120 + y % 3); bytes[i + 2] = (byte)(140 + (x + y) % 3);
        }
        return bytes;
    }
    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void OriginalSnapshotCannotBeChangedThroughInputOrExport(bool throughExport)
    {
        byte[] input = Pattern(80, 60, 248);
        var original = input.ToArray();
        var frame = new RgbFrame(80, 60, 248, input);
        var before = RegionSampler.Measure(frame, new(0, 0, 80, 60), new());
        if (throughExport)
        {
            Assert.True(MemoryMarshal.TryGetArray(frame.Pixels, out var exposed));
            Array.Fill(exposed.Array!, (byte)230, exposed.Offset, exposed.Count / 2);
        }
        else Array.Fill(input, (byte)230, 0, input.Length / 2);
        Assert.Equal(original, frame.Pixels.ToArray());
        Assert.Equal(before, RegionSampler.Measure(frame, new(0, 0, 80, 60), new()));
    }
    [Theory]
    [InlineData(-45)] [InlineData(-27.3)] [InlineData(0)] [InlineData(13.7)] [InlineData(45)]
    public void RotatedMeasurementUsesEachOriginalPixelInsideMaskExactlyOnce(double degrees)
    {
        const int w = 120, h = 100, stride = 368;
        byte[] bytes = Pattern(w, h, stride);
        var source = new RgbFrame(w, h, stride, bytes);
        var rotation = new ImageRotation(w, h, degrees);
        var view = new RgbFrame(source, rotation);
        var bounds = new PixelRect(view.Width / 2 - 20, view.Height / 2 - 15, 40, 30);
        Assert.Throws<InvalidOperationException>(() => view.Pixels);
        var samples = view.Sample(bounds, 1, default).ToArray();
        // Independent full-original scan, not SourceBounds or RgbFrame.Sample.
        // Rotation around image centers determines membership; colors come directly
        // from the original byte buffer, never from transformed preview pixels.
        double c = Math.Cos(degrees * Math.PI / 180), s = Math.Sin(degrees * Math.PI / 180);
        var expected = new List<RgbColor>();
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            double ax = c * (x + .5 - w / 2d) + s * (y + .5 - h / 2d) + view.Width / 2d;
            double ay = -s * (x + .5 - w / 2d) + c * (y + .5 - h / 2d) + view.Height / 2d;
            if (ax >= bounds.X && ax < bounds.Right && ay >= bounds.Y && ay < bounds.Bottom)
            {
                int i = y * stride + x * 3;
                expected.Add(new(bytes[i], bytes[i + 1], bytes[i + 2]));
            }
        }
        Assert.Equal(expected, samples.Select(p => p.Pixel));
        Assert.Equal(samples.Length, samples.Select(p => (p.X, p.Y)).Distinct().Count());
        var measured = RegionSampler.Measure(view, bounds, new());
        Assert.True(measured.IsUsable, measured.Reason);
        Assert.Equal(expected.Count, measured.SampleCount);
        var lab = ColorMath.LinearToLab(expected.Average(p => ColorMath.Decode(p.R)), expected.Average(p => ColorMath.Decode(p.G)), expected.Average(p => ColorMath.Decode(p.B)));
        Assert.InRange(ColorMath.DeltaE00(lab, measured.Lab!.Value), 0, 1e-10);
    }
    [Fact]
    public void RotationRejectsWrongSizeAndNestedView()
    {
        var source = new RgbFrame(120, 100, 360, Pattern(120, 100, 360));
        Assert.Throws<ArgumentException>(() => new RgbFrame(source, new ImageRotation(121, 100, 20)));
        var rotated = new RgbFrame(source, new ImageRotation(120, 100, 20));
        Assert.Throws<ArgumentException>(() => new RgbFrame(rotated, new ImageRotation(rotated.Width, rotated.Height, 10)));
    }
    [Fact]
    public void GloballyAlteredWorkingCopyCannotChangeOriginalAnalysis()
    {
        var source = Iro.TestFixtures.ReflectionFixture.Make("Kontrolle", "Beide", false, 0);
        var expected = new ImageAnalyzer().Analyze(source, new());
        byte[] copy = source.Pixels.ToArray();
        // Identical inversion for every pixel: deliberate test perturbation only.
        for (int i = 0; i < copy.Length; i++) copy[i] = (byte)(255 - copy[i]);
        var changed = new ImageAnalyzer().Analyze(new(source.Width, source.Height, source.Stride, copy), new());
        Assert.Equal(AnalysisStatus.Measured, changed.Status);
        Assert.Equal(expected.Fields.Select(f => f.Bounds), changed.Fields.Select(f => f.Bounds));
        Assert.NotEqual(expected.Fields[0].Measurement.Lab, changed.Fields[0].Measurement.Lab);
        var actual = new ImageAnalyzer().Analyze(source, new());
        Assert.Equal(AnalysisStatus.Measured, actual.Status);
        Assert.Equal(expected.Fields, actual.Fields);
    }
    [Fact]
    public void EmptyRotationCornersNeverBecomeBlackMeasurements()
    {
        var source = new RgbFrame(120, 100, 360, Pattern(120, 100, 360));
        var view = new RgbFrame(source, new ImageRotation(120, 100, 30));
        var result = RegionSampler.Measure(view, new(0, 0, 25, 25), new());
        Assert.False(result.IsUsable);
        Assert.Null(result.Lab);
        Assert.Equal(0, result.SampleCount);
    }
}
