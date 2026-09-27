using Iro.Core.Analysis;

namespace Iro.Core.Tests;

public class ContourMaskTests
{
    [Theory]
    [InlineData(false)] [InlineData(true)]
    public void MaskOnlySelectsOriginalPixelsAndDoesNotHideInteriorGradient(bool gradient)
    {
        const int w = 100, h = 80;
        PixelPoint[] mask = [new(10, 10), new(90, 16), new(90, 64), new(10, 70)];
        byte[] pixels = new byte[w * h * 3];
        int included = 0;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
        {
            // Independent inequalities for this specific quadrilateral.
            bool inside = x + .5 >= 10 && x + .5 <= 90 && y + .5 >= 10 + .075 * (x + .5 - 10) && y + .5 <= 70 - .075 * (x + .5 - 10);
            if (inside) included++;
            for (int channel = 0; channel < 3; channel++)
                pixels[(y * w + x) * 3 + channel] = inside ? (byte)(90 + channel * 15 + (gradient ? x / 2 : 0)) : (byte)255;
        }
        var original = pixels.ToArray();
        var frame = new RgbFrame(w, h, w * 3, pixels);
        var result = RegionSampler.Measure(frame, new(0, 0, w, h), new(), mask: mask);
        Assert.Equal(original, frame.Pixels.ToArray());
        Assert.Equal(included, result.SampleCount);
        if (gradient) { Assert.False(result.IsUsable); Assert.Null(result.Lab); Assert.True(result.SpatialDeltaE > 2); }
        else
        {
            Assert.True(result.IsUsable, result.Reason);
            var expected = ColorMath.LinearToLab(ColorMath.Decode(90), ColorMath.Decode(105), ColorMath.Decode(120));
            Assert.InRange(ColorMath.DeltaE00(expected, result.Lab!.Value), 0, 1e-8);
        }
    }
    [Fact]
    public void EmptyIntersectionCannotProduceAColor()
    {
        var source = new RgbFrame(80, 80, 240, Enumerable.Repeat((byte)140, 80 * 80 * 3).ToArray());
        PixelPoint[] mask = [new(90, 90), new(100, 90), new(100, 100), new(90, 100)];
        var result = RegionSampler.Measure(source, new(0, 0, 80, 80), new(), mask: mask);
        Assert.False(result.IsUsable); Assert.Null(result.Lab); Assert.Equal(0, result.SampleCount);
    }
}
