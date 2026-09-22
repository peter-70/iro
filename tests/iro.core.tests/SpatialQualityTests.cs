using Iro.Core.Analysis;
namespace Iro.Core.Tests;

public class SpatialQualityTests
{
    private static RgbFrame Gradient(int variation, bool texture = false)
    {
        const int width = 120, height = 90;
        var data = new byte[width * height * 3];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            byte value = (byte)(110 + (texture ? ((x + y) % 2 == 0 ? 0 : variation) : variation * x / (width - 1)));
            for (int c = 0; c < 3; c++) data[(y * width + x) * 3 + c] = value;
        }
        return new(width, height, width * 3, data);
    }

    [Fact]
    public void CoherentGradientIsRejectedAlthoughOverallDispersionPasses()
    {
        var frame = Gradient(20);
        var old = RegionSampler.Measure(frame, new(0, 0, 120, 90), new() { MaximumSpatialDeltaE = 200 });
        var current = RegionSampler.Measure(frame, new(0, 0, 120, 90), new());
        Assert.True(old.IsUsable);
        Assert.True(current.SpatialDeltaE > 2);
        Assert.False(current.IsUsable); Assert.Null(current.Lab);
        Assert.Contains("räumlich ungleichmäßig", current.Reason);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(10, true)]
    public void UniformGentleAndFineTexturedSurfacesRemainUsable(int variation, bool texture)
    {
        var result = RegionSampler.Measure(Gradient(variation, texture), new(0, 0, 120, 90), new());
        Assert.True(result.IsUsable, result.Reason);
        Assert.True(result.SpatialDeltaE <= 2);
    }

    [Fact]
    public void InvalidQualityThresholdsAreRejected()
    {
        Assert.Throws<ArgumentException>(() => new AnalysisOptions { MaximumSpatialDeltaE = double.NaN }.Validate());
        Assert.Throws<ArgumentException>(() => new AnalysisOptions { MaximumSpatialDeltaE = 0 }.Validate());
        Assert.Throws<ArgumentException>(() => new AnalysisOptions { SurfaceMargin = .3 }.Validate());
    }
}
