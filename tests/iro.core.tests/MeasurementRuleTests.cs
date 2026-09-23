using Iro.Core.Analysis;

namespace Iro.Core.Tests;

// Independent pixel fixtures, without generator metadata or expected colors as analyzer input.
public class MeasurementRuleTests
{
    private const int Width = 800, Height = 600;
    private static byte[] Scene(bool cropped = false)
    {
        var pixels = Enumerable.Repeat((byte)140, Width * Height * 3).ToArray();
        int start = cropped ? -40 : 60;
        Paint(pixels, new(550, start, 160, 130), new(70, 100, 160));
        Paint(pixels, new(550, start + 150, 160, 130), new(100, 140, 190));
        Paint(pixels, new(550, start + 300, 160, 130), new(120, 160, 220));
        return pixels;
    }
    private static void Paint(byte[] pixels, PixelRect rect, RgbColor color)
    {
        for (int y = Math.Max(0, rect.Y); y < Math.Min(Height, rect.Bottom); y++)
        for (int x = Math.Max(0, rect.X); x < Math.Min(Width, rect.Right); x++)
        {
            int offset = (y * Width + x) * 3;
            pixels[offset] = color.R; pixels[offset + 1] = color.G; pixels[offset + 2] = color.B;
        }
    }
    private static ImageAnalysis Analyze(byte[] pixels, bool horizontal = false)
    {
        if (!horizontal) return new ImageAnalyzer().Analyze(new(Width, Height, Width * 3, pixels), new());
        var turned = new byte[pixels.Length];
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
            Array.Copy(pixels, (y * Width + x) * 3, turned, (x * Height + Height - 1 - y) * 3, 3);
        return new ImageAnalyzer().Analyze(new(Height, Width, Height * 3, turned), new());
    }
    private static void AssertNoValues(ImageAnalysis result)
    {
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
        Assert.DoesNotContain(result.Status, new[] { AnalysisStatus.Measured, AnalysisStatus.PartiallyMeasured });
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CroppedStripDoesNotReleaseItsRemainingFields(bool horizontal)
    {
        var control = Analyze(Scene(), horizontal);
        Assert.Equal(AnalysisStatus.Measured, control.Status);
        Assert.Equal(3, control.Fields.Count);
        var result = Analyze(Scene(cropped: true), horizontal);
        AssertNoValues(result);
        Assert.Contains("vollständig", result.Hint);
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AClippedMeasurementChannelStopsTheWholeComparison(bool wall)
    {
        var pixels = Scene();
        var control = Analyze(pixels);
        Assert.Equal(AnalysisStatus.Measured, control.Status);
        if (wall)
        {
            var reference = control.Fields[0].Reference!.Bounds;
            Paint(pixels, reference, new(255, 120, 140));
        }
        else Paint(pixels, new(550, 60, 160, 130), new(255, 100, 160));
        var result = Analyze(pixels);
        Assert.NotEmpty(result.Fields);
        AssertNoValues(result);
        Assert.Contains("kanal", result.Hint);
    }
    [Fact]
    public void DetectedUnusableMotionBlurStopsOtherSharpFieldsToo()
    {
        var pixels = Scene();
        var source = (byte[])pixels.Clone();
        for (int y = 30; y < 220; y++)
        for (int x = 510; x < 750; x++)
        for (int c = 0; c < 3; c++)
        {
            int sum = 0;
            for (int dx = -25; dx <= 25; dx++) sum += source[(y * Width + x + dx) * 3 + c];
            pixels[(y * Width + x) * 3 + c] = (byte)(sum / 51);
        }
        var result = Analyze(pixels);
        Assert.Contains(result.Fields, f => f.Hint?.Contains("unscharf") == true);
        AssertNoValues(result);
        Assert.Contains("unscharf", result.Hint);
    }
    [Theory]
    [InlineData(250, 250, 250)]
    [InlineData(10, 10, 10)]
    [InlineData(240, 10, 10)]
    public void BrightDarkAndSaturatedColorsWithResolvableChannelsRemainMeasurable(byte r, byte g, byte b)
    {
        var pixels = Scene();
        Paint(pixels, new(550, 60, 160, 130), new(r, g, b));
        var result = Analyze(pixels);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count(f => f.MeasurementAllowed));
    }
    [Theory]
    [InlineData(0)]
    [InlineData(255)]
    public void EndpointPlateausAreRejectedButSparseExcludedPrintIsAllowed(byte endpoint)
    {
        var plateau = Enumerable.Repeat(endpoint, 60 * 60 * 3).ToArray();
        var result = RegionSampler.Measure(new(60, 60, 180, plateau), new(0, 0, 60, 60), new());
        Assert.False(result.IsUsable);
        Assert.Null(result.Lab);
        var printed = Enumerable.Repeat((byte)120, 60 * 60 * 3).ToArray();
        Array.Fill(printed, endpoint, 0, 100 * 3);
        var usable = RegionSampler.Measure(new(60, 60, 180, printed), new(0, 0, 60, 60), new());
        Assert.True(usable.IsUsable, usable.Reason);
        Assert.InRange(ColorMath.DeltaE00(ColorMath.ToLab(new(120, 120, 120)), usable.Lab!.Value), 0, 1e-9);
    }
    [Fact]
    public void FrameWideFailureClearsPreviouslyDisplayedValues()
    {
        var presentation = new AnalysisPresentation();
        presentation.Present(Analyze(Scene()));
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));
        var pixels = Scene();
        Paint(pixels, new(550, 60, 160, 130), new(255, 100, 160));
        presentation.Present(Analyze(pixels));
        Assert.All(presentation.Fields, f => Assert.Equal("–", f.Value));
        Assert.Contains("kanal", presentation.Message);
        presentation.Present(Analyze(Scene()));
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));
    }}