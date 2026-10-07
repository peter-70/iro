using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

// User decision: channel endpoints alone are diagnostic, never a rejection reason.
public sealed class EndpointDiagnosticTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(0, 255)]
    [InlineData(1, 255)]
    [InlineData(2, 255)]
    public void HomogeneousEndpointColorRemainsMeasurable(int channel, int value)
    {
        var color = WithChannel(channel, (byte)value);
        var image = Solid(color);
        var before = image.Pixels.ToArray();
        var measurement = RegionSampler.Measure(image, new(0, 0, 40, 40), new());

        Assert.True(measurement.IsUsable, measurement.Reason);
        Assert.Null(measurement.Reason);
        Assert.NotNull(measurement.Lab);
        Assert.InRange(ColorMath.DeltaE00(ColorMath.ToLab(color), measurement.Lab.Value), 0, 1e-9);
        Assert.Equal(1d, measurement.NearLimitFraction);
        Assert.Equal(before, image.Pixels.ToArray());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void AdjacentHighValuesHaveTheSameReleaseDecision(int channel)
    {
        var below = RegionSampler.Measure(Solid(WithChannel(channel, 254)), new(0, 0, 40, 40), new());
        var endpoint = RegionSampler.Measure(Solid(WithChannel(channel, 255)), new(0, 0, 40, 40), new());

        Assert.True(below.IsUsable, below.Reason);
        Assert.Equal(below.IsUsable, endpoint.IsUsable);
        Assert.NotNull(endpoint.Lab);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(100)]
    public void EndpointFrequencyDoesNotControlRelease(int percent)
    {
        // Values differ by only one code value; all remain inside the existing MAD tolerance.
        var pixels = new byte[40 * 40 * 3];
        for (int i = 0; i < 1600; i++)
        {
            pixels[i * 3] = (byte)(i % 100 < percent ? 255 : 254);
            pixels[i * 3 + 1] = 120;
            pixels[i * 3 + 2] = 80;
        }
        var measurement = RegionSampler.Measure(new(40, 40, 120, pixels), new(0, 0, 40, 40), new());

        Assert.True(measurement.IsUsable, measurement.Reason);
        Assert.Equal(0d, measurement.RejectedFraction);
        Assert.Equal(1d, measurement.NearLimitFraction);
    }

    [Theory]
    [InlineData(0, ReferenceMode.SharedAutomaticTrial)]
    [InlineData(255, ReferenceMode.SharedAutomaticTrial)]
    [InlineData(0, ReferenceMode.AdjacentPerFieldTrial)]
    [InlineData(255, ReferenceMode.AdjacentPerFieldTrial)]
    public void EndpointFieldDoesNotBlockOtherComparisons(int value, ReferenceMode mode)
    {
        var image = Scene(new(120, 120, 120), new((byte)value, 120, 80));
        var result = new ImageAnalyzer().Analyze(image, new() { ReferenceMode = mode });

        AssertAllComparisonsReleased(result);
        Assert.Equal(1d, result.Fields[0].Measurement.NearLimitFraction);
        Assert.All(result.Fields.Skip(1), field => Assert.Equal(0d, field.Measurement.NearLimitFraction));
        Assert.True(result.Fields[1].IsNearest);
    }

    [Theory]
    [InlineData(0, ReferenceMode.SharedAutomaticTrial)]
    [InlineData(255, ReferenceMode.SharedAutomaticTrial)]
    [InlineData(0, ReferenceMode.AdjacentPerFieldTrial)]
    [InlineData(255, ReferenceMode.AdjacentPerFieldTrial)]
    public void EndpointWallRemainsAValidReference(int value, ReferenceMode mode)
    {
        var image = Scene(new((byte)value, 120, 80), new(116, 116, 116));
        var result = new ImageAnalyzer().Analyze(image, new() { ReferenceMode = mode });

        AssertAllComparisonsReleased(result);
        Assert.All(result.Fields, field => Assert.Equal(1d, field.Reference!.NearLimitFraction));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(255)]
    public void EndpointMetadataDoesNotOverrideInsufficientSampleRejection(int value)
    {
        var measurement = RegionSampler.Measure(Solid(new((byte)value, 120, 80)), new(0, 0, 20, 20), new());

        Assert.False(measurement.IsUsable);
        Assert.Equal("Zu wenig nutzbare Bildfläche.", measurement.Reason);
        Assert.Null(measurement.Lab);
        Assert.Equal(1d, measurement.NearLimitFraction);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(255)]
    public void TooSmallEndpointFieldDoesNotBlockValidNeighbors(int value)
    {
        var image = Scene(new(120, 120, 120), new((byte)value, 120, 80), firstHeight: 48);
        // The short field has fewer than 4000 samples; both full fields and the wall have more.
        var result = new ImageAnalyzer().Analyze(image, new() { MinimumSamples = 4000 });

        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        var small = result.Fields[0];
        Assert.False(small.MeasurementAllowed);
        Assert.Equal("Zu wenig nutzbare Bildfläche.", small.Measurement.Reason);
        Assert.Equal(1d, small.Measurement.NearLimitFraction);
        Assert.Null(small.DeltaE00);
        Assert.False(small.IsNearest);
        Assert.All(result.Fields.Skip(1), field =>
        {
            Assert.True(field.MeasurementAllowed, field.Hint);
            Assert.NotNull(field.DeltaE00);
        });
        Assert.True(result.Fields[1].IsNearest);
    }

    private static void AssertAllComparisonsReleased(ImageAnalysis result)
    {
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, field =>
        {
            Assert.True(field.MeasurementAllowed, field.Hint);
            Assert.True(field.Measurement.IsUsable);
            Assert.True(field.Reference?.IsUsable);
            Assert.NotNull(field.DeltaE00);
            Assert.True(double.IsFinite(field.DeltaE00.Value));
        });
        Assert.Contains(result.Fields, field => field.IsNearest);
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.All(presentation.Fields, field => Assert.NotEqual("–", field.Value));
    }

    private static RgbColor WithChannel(int channel, byte value) => channel switch
    {
        0 => new(value, 120, 80),
        1 => new(120, value, 80),
        _ => new(120, 80, value)
    };

    private static RgbFrame Solid(RgbColor color)
    {
        var pixels = new byte[40 * 40 * 3];
        for (int i = 0; i < 1600; i++)
        {
            pixels[i * 3] = color.R;
            pixels[i * 3 + 1] = color.G;
            pixels[i * 3 + 2] = color.B;
        }
        return new(40, 40, 120, pixels);
    }

    private static RgbFrame Scene(RgbColor wall, RgbColor firstField, int firstHeight = 110)
    {
        const int width = 1440, height = 960;
        var pixels = new byte[width * height * 3];
        void Paint(int x, int y, RgbColor color)
        {
            int offset = (y * width + x) * 3;
            pixels[offset] = color.R; pixels[offset + 1] = color.G; pixels[offset + 2] = color.B;
        }
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++) Paint(x, y, wall);
        RgbColor[] colors = [firstField, new(123, 123, 123), new(153, 153, 153)];
        for (int i = 0; i < colors.Length; i++)
        {
            int top = 230 + i * 150;
            int fieldHeight = i == 0 ? firstHeight : 110;
            for (int y = top - 7; y < top + fieldHeight + 7; y++)
            for (int x = 643; x < 837; x++)
                Paint(x, y, x >= 650 && x < 830 && y >= top && y < top + fieldHeight ? colors[i] : new(65, 65, 65));
        }
        return new(width, height, width * 3, pixels);
    }
}
