using Iro.Core.Analysis;

namespace Iro.Core.Tests;

// Hand-built RGB fixtures: no generator, labels, expected masks or palette implementation involved.
public class PixelAnalysisTests
{
    private const int Width = 800, Height = 600;
    private static byte[] Wall() => Enumerable.Repeat((byte)140, Width * Height * 3).ToArray();
    private static void Paint(byte[] pixels, PixelRect bounds, RgbColor color)
    {
        for (int y = bounds.Y; y < bounds.Bottom; y++)
        for (int x = bounds.X; x < bounds.Right; x++)
        {
            int p = (y * Width + x) * 3;
            pixels[p] = color.R; pixels[p + 1] = color.G; pixels[p + 2] = color.B;
        }
    }
    private static void Strip(byte[] pixels, int x = 550)
    {
        Paint(pixels, new(x, 60, 160, 130), new(255, 255, 255));
        Paint(pixels, new(x, 204, 160, 130), new(10, 10, 10));
        Paint(pixels, new(x, 348, 160, 130), new(170, 90, 110));
    }
    private static ImageAnalysis Analyze(byte[] pixels) => new ImageAnalyzer().Analyze(new(Width, Height, Width * 3, pixels), new());

    [Fact]
    public void WhiteAndVeryDarkFieldsAreNotDiscardedByColor()
    {
        var pixels = Wall(); Strip(pixels);
        var result = Analyze(pixels);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.InRange(result.Fields[0].Measurement.Lab!.Value.L, 99.99, 100.01);
        Assert.InRange(result.Fields[1].Measurement.Lab!.Value.L, 0, 3);
        Assert.All(result.Fields, f => Assert.Equal(result.Fields[0].Reference, f.Reference));
    }

    [Fact]
    public void MultipleStripsAreAmbiguousAndProduceNoValues()
    {
        var pixels = Wall(); Strip(pixels); Strip(pixels, 120);
        var result = Analyze(pixels);
        Assert.Equal(AnalysisStatus.AmbiguousPattern, result.Status);
        Assert.Empty(result.Fields);
    }

    [Fact]
    public void AContaminatedFieldDoesNotInvalidateOtherFields()
    {
        var pixels = Wall(); Strip(pixels);
        var first = Analyze(pixels);
        var inner = first.Fields[1].InnerBounds;
        // Small individual flecks preserve the geometric field, but contaminate 25% of its measurement area.
        for (int y = inner.Y; y < inner.Bottom; y++)
        for (int x = inner.X; x < inner.Right; x++)
            if ((x + y) % 4 == 0) Paint(pixels, new(x, y, 1, 1), new(220, 220, 220));
        var result = Analyze(pixels);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Null(result.Fields[1].DeltaE00);
        Assert.False(result.Fields[1].IsNearest);
        Assert.Equal(2, result.Fields.Count(f => f.MeasurementAllowed));
    }

    [Fact]
    public void InvalidSharedWallInvalidatesEveryComparison()
    {
        var pixels = Wall(); Strip(pixels);
        var reference = Analyze(pixels).Fields[0].Reference!.Bounds;
        for (int y = reference.Y; y < reference.Bottom; y++)
        for (int x = reference.X; x < reference.Right; x++)
            if ((x + y) % 3 == 0) Paint(pixels, new(x, y, 1, 1), new(250, 250, 250));
        var result = Analyze(pixels);
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.All(result.Fields, f => { Assert.Null(f.DeltaE00); Assert.False(f.MeasurementAllowed); });
    }

    [Fact]
    public void ASingleClearlyBoundedFieldIsPermitted()
    {
        var pixels = Wall(); Paint(pixels, new(550, 150, 150, 240), new(40, 70, 90));
        Assert.Equal(AnalysisStatus.Measured, Analyze(pixels).Status);
        Assert.Single(Analyze(pixels).Fields);
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OffsetOccluderIsRejectedWhileDifferentFieldHeightsRemainUsable(bool horizontal)
    {
        var pixels = Wall();
        Paint(pixels, new(550, 40, 160, 90), new(70, 100, 150));
        Paint(pixels, new(550, 145, 160, 120), new(80, 115, 170));
        Paint(pixels, new(550, 280, 160, 75), new(95, 130, 185));
        Paint(pixels, new(550, 370, 160, 105), new(110, 145, 200));
        Paint(pixels, new(550, 490, 160, 65), new(125, 160, 215));
        ImageAnalysis Run(byte[] source)
        {
            if (!horizontal) return Analyze(source);
            var rotated = new byte[source.Length];
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Array.Copy(source, (y * Width + x) * 3, rotated, (x * Height + Height - 1 - y) * 3, 3);
            return new ImageAnalyzer().Analyze(new(Height, Width, Height * 3, rotated), new());
        }
        Assert.Equal(5, Run(pixels).Fields.Count(f => f.MeasurementAllowed));
        Paint(pixels, new(574, 275, 136, 85), new(164, 116, 82));
        var result = Run(pixels);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Equal(4, result.Fields.Count(f => f.MeasurementAllowed));
        Assert.Contains(result.Fields, f => !f.MeasurementAllowed && f.Hint!.Contains("Feldgrenzen"));
    }
}
