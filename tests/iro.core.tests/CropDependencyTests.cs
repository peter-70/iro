using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

public sealed class CropDependencyTests
{
    private static readonly AnalysisOptions Options = new();
    private static readonly PixelRect Crop = new(650, 0, 180, 150);

    [Fact]
    public void UnusableCropDoesNotBlockCompleteFields()
    {
        var image = Scene((x, y, v) => Crop.Contains(x, y) ? (byte)(x < 740 ? 74 : 86) : v);
        AssertAdmitted(image);
        var result = Analyze(image);
        Assert.True(result.Fields[0].Measurement.SpatialDeltaE > Options.MaximumSpatialDeltaE);
        AssertLocalFailure(result, Analyze(Scene()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UnusableControlSurfaceRemainsLocallyEffective(bool nonSpatial)
    {
        var innerBounds = Crop.Inset(Options.InnerMargin);
        var image = Scene((x, y, v) => Crop.Contains(x, y) && !innerBounds.Contains(x, y)
            ? nonSpatial ? (byte)((x + y) % 2 == 0 ? 86 : 80) : (byte)68 : v);
        AssertAdmitted(image);
        var inner = RegionSampler.Measure(image, innerBounds, Options);
        var surface = RegionSampler.Measure(image, Crop.Inset(Options.SurfaceMargin), Options);
        Assert.True(inner.IsUsable, inner.Reason);
        Assert.False(surface.IsUsable);
        if (nonSpatial)
        {
            Assert.True(surface.SpatialDeltaE <= Options.MaximumSpatialDeltaE);
            Assert.True(surface.ChannelMad <= Options.MaximumChannelMad);
            Assert.True(surface.RejectedFraction > Options.MaximumOutlierFraction);
        }
        else Assert.True(surface.SpatialDeltaE > Options.MaximumSpatialDeltaE);
        var result = Analyze(image);
        AssertLocalFailure(result, Analyze(Scene()));
        Assert.False(result.Fields[0].Measurement.IsUsable);
        Assert.Null(result.Fields[0].Measurement.Lab);
        Assert.False(string.IsNullOrWhiteSpace(result.Fields[0].Measurement.Reason));
        if (nonSpatial) Assert.Equal(surface.Reason, result.Fields[0].Measurement.Reason);
    }

    [Fact]
    public void InvalidLocalReferenceOnlyBlocksCropComparison()
    {
        var options = Options with { ReferenceMode = ReferenceMode.AdjacentPerFieldTrial };
        var baseline = Analyze(Scene(), options);
        var reference = baseline.Fields[0].Reference!.Bounds;
        var image = Scene((x, y, v) => reference.Contains(x, y) ? (byte)(x < reference.X + reference.Width / 2 ? 114 : 126) : v);
        AssertAdmitted(image);
        var result = Analyze(image, options);
        Assert.False(result.Fields[0].Reference!.IsUsable);
        AssertLocalFailure(result, baseline);
    }

    [Fact]
    public void InvalidSharedReferenceHasReferenceStatusAndNoComparisons()
    {
        var baseline = Analyze(Scene());
        var reference = baseline.Fields[0].Reference!.Bounds;
        var image = Scene((x, y, v) => reference.Contains(x, y) ? (byte)(x < reference.X + reference.Width / 2 ? 114 : 126) : v);
        AssertAdmitted(image);
        var result = Analyze(image);
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, field =>
        {
            Assert.False(field.Reference!.IsUsable);
            Assert.False(field.MeasurementAllowed);
            Assert.Null(field.DeltaE00);
            Assert.False(field.IsNearest);
        });
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.All(presentation.Fields, field => Assert.Equal("–", field.Value));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void ValidEndCropRemainsPartiallyMeasured(bool horizontal, bool atEnd)
    {
        var image = Scene(horizontal: horizontal, atEnd: atEnd);
        AssertAdmitted(image);
        var result = Analyze(image);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, field =>
        {
            Assert.True(field.MeasurementAllowed, field.Hint);
            Assert.NotNull(field.DeltaE00);
        });
        Assert.Contains("Angeschnittener Streifen", result.Hint);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MisalignedCropKeepsCompleteComparisons(bool horizontal)
    {
        var image = Scene(cropOffset: 6, horizontal: horizontal);
        var detection = StripDetector.Detect(image, Options, default);
        Assert.True(MeasurementSafety.HasCroppedContinuation(detection));
        Assert.False(MeasurementSafety.TryGetSingleCropCandidate(detection, image.Width, image.Height, Options, out _));
        var result = Analyze(image);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Equal(2, result.Fields.Count);
        Assert.All(result.Fields, field => Assert.True(field.MeasurementAllowed));
    }

    [Fact]
    public void BroadEdgesDoNotBlockIndependentAdmittedCropComparisons()
    {
        var sharp = Scene();
        var pixels = sharp.Pixels.ToArray();
        for (int y = 190 - 24; y < 300 + 24; y++)
        for (int x = Crop.X; x < Crop.Right; x++)
        {
            int sum = 0;
            for (int dy = -8; dy <= 8; dy++) sum += sharp.GetPixel(x, y + dy).R;
            int offset = (y * sharp.Width + x) * 3;
            pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = (byte)(sum / 17);
        }
        var image = new RgbFrame(sharp.Width, sharp.Height, sharp.Stride, pixels);
        AssertAdmitted(image);
        var result = Analyze(image);
        Assert.Equal(3, result.Fields.Count);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Contains(result.Diagnostics, d => d.Contains("Breiter Farbübergang"));
        var baseline = Analyze(sharp);
        for (int i = 0; i < result.Fields.Count; i++)
        {
            Assert.True(result.Fields[i].MeasurementAllowed);
            Assert.Equal(baseline.Fields[i].DeltaE00!.Value, result.Fields[i].DeltaE00!.Value, 9);
            Assert.Equal(baseline.Fields[i].IsNearest, result.Fields[i].IsNearest);
        }
    }

    private static void AssertLocalFailure(ImageAnalysis result, ImageAnalysis baseline)
    {
        Assert.Equal(AnalysisStatus.PartiallyMeasured, baseline.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.False(result.Fields[0].MeasurementAllowed);
        Assert.Null(result.Fields[0].DeltaE00);
        Assert.False(result.Fields[0].IsNearest);
        for (int i = 1; i < 3; i++)
        {
            var field = result.Fields[i];
            Assert.True(field.MeasurementAllowed, field.Hint);
            Assert.Equal(baseline.Fields[i].Measurement.Lab, field.Measurement.Lab);
            Assert.Equal(baseline.Fields[i].Reference!.Lab, field.Reference!.Lab);
            Assert.Equal(baseline.Fields[i].DeltaE00, field.DeltaE00);
        }
        Assert.True(result.Fields[1].IsNearest);
        Assert.False(result.Fields[2].IsNearest);
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.Equal("–", presentation.Fields[0].Value);
        Assert.All(presentation.Fields.Skip(1), field => Assert.NotEqual("–", field.Value));
    }

    private static void AssertAdmitted(RgbFrame image)
    {
        var detection = StripDetector.Detect(image, Options, default);
        Assert.False(detection.Ambiguous);
        Assert.Equal(2, detection.Fields.Count);
        Assert.True(MeasurementSafety.HasCroppedContinuation(detection));
        Assert.True(MeasurementSafety.TryGetSingleCropCandidate(detection, image.Width, image.Height, Options, out _));
    }

    private static ImageAnalysis Analyze(RgbFrame image, AnalysisOptions? options = null)
        => new ImageAnalyzer().Analyze(image, options ?? Options);

    internal static RgbFrame Scene(Func<int, int, byte, byte>? transform = null, int cropOffset = 0,
        bool horizontal = false, bool atEnd = false)
    {
        const int width = 1440, height = 960;
        int outputWidth = horizontal ? height : width, outputHeight = horizontal ? width : height;
        var pixels = new byte[width * height * 3];
        PixelRect[] regions = [Crop with { X = Crop.X + cropOffset }, new(650, 190, 180, 110), new(650, 340, 180, 110)];
        byte[] colors = [80, 123, 153];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            byte value = 120;
            for (int i = 0; i < regions.Length; i++)
            {
                var r = regions[i];
                if (x >= r.X - 7 && x < r.Right + 7 && y >= r.Y - 7 && y < r.Bottom + 7)
                    value = r.Contains(x, y) ? colors[i] : i == 0 ? (byte)120 : (byte)65;
            }
            value = transform?.Invoke(x, y, value) ?? value;
            int sy = atEnd ? height - 1 - y : y;
            int offset = horizontal ? (x * outputWidth + sy) * 3 : (sy * outputWidth + x) * 3;
            pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = value;
        }
        return new(outputWidth, outputHeight, outputWidth * 3, pixels);
    }
}
