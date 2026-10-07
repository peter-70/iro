using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

// User decision: an isolated unadmitted end remainder must not erase sound comparisons.
// Scene construction and physical identities stay exclusively on this test side.
public sealed class ExcludedEndCropTests
{
    private static readonly AnalysisOptions Options = new();
    private static PixelRect Trans(PixelRect r, bool horizontal) =>
        horizontal ? new(r.Y, r.X, r.Height, r.Width) : r;

    [Theory]
    [InlineData(false, 6)]
    [InlineData(true, 6)]
    [InlineData(false, 8)]
    [InlineData(true, -6)]
    public void CompleteFieldsKeepOriginalPixelsAndCorrespondingPresentation(bool horizontal, int offset)
    {
        var image = CropDependencyTests.Scene(cropOffset: offset, horizontal: horizontal);
        var before = image.Pixels.ToArray();
        var baseline = new ImageAnalyzer().Analyze(CropDependencyTests.Scene(horizontal: horizontal), Options);
        var result = new ImageAnalyzer().Analyze(image, Options);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Equal(0, result.StraighteningDegrees);
        Assert.Equal(2, result.Fields.Count);
        var uncertain = Assert.Single(result.UncertainRegions);
        Assert.Equal(Trans(new(650 + offset, 0, 180, 150), horizontal), uncertain.Bounds);
        Assert.DoesNotContain(result.Fields, f => f.Bounds == uncertain.Bounds);
        var expectedReference = Trans(new(478, 248, 144, 144), horizontal);
        Assert.NotEqual(baseline.Fields[0].Reference!.Bounds, expectedReference);
        for (int i = 0; i < 2; i++)
        {
            var bounds = Trans(new(650, 190 + 150 * i, 180, 110), horizontal);
            var field = Assert.Single(result.Fields, f => f.Bounds == bounds);
            var previous = Assert.Single(baseline.Fields, f => f.Bounds == bounds);
            Assert.True(field.MeasurementAllowed);
            Assert.Equal(previous.InnerBounds, field.InnerBounds);
            Assert.Null(field.InnerPolygon);
            Assert.Equal(previous.Measurement, field.Measurement);
            Assert.Equal(previous.DeltaE00, field.DeltaE00);
            Assert.Equal(expectedReference, field.Reference!.Bounds);
            Assert.False(uncertain.Bounds.Intersects(field.Reference.Bounds));
            Assert.Equal(7980, field.Measurement.SampleCount);
            Assert.Equal(0, field.Measurement.RejectedFraction);
            byte c = (byte)(i == 0 ? 123 : 153);
            var pixels = image.Sample(field.InnerBounds, 1, default).ToArray();
            Assert.Equal(7980, pixels.Length);
            Assert.All(pixels, p => Assert.Equal(new RgbColor(c, c, c), p.Pixel));
            Assert.Equal(RegionSampler.Measure(image, field.InnerBounds, Options), field.Measurement);
            Assert.All(image.Sample(field.Reference.Bounds, 1, default),
                p => Assert.Equal(new RgbColor(120, 120, 120), p.Pixel));
        }
        Assert.Equal(result.Fields[0].FieldId, Assert.Single(result.Fields, f => f.IsNearest).FieldId);
        Assert.Equal(before, image.Pixels.ToArray());
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.Equal(3, presentation.Markers.Count);
        Assert.Contains("Gesamtzahl", presentation.Message);
        foreach (var field in result.Fields)
        {
            var marker = Assert.Single(presentation.Markers, m => m.RegionId == field.FieldId);
            var row = Assert.Single(presentation.Fields, r => r.FieldId == field.FieldId);
            Assert.Equal(field.Bounds, marker.Bounds);
            Assert.Equal(row.Label, marker.Label);
            Assert.Equal(row.Value, marker.Value);
            Assert.Equal(MarkerKind.Measured, marker.Kind);
            Assert.Equal(field.IsNearest, marker.IsNearest);
            Assert.NotEqual("–", marker.Value);
        }
        var uncertainMarker = Assert.Single(presentation.Markers, m => m.Kind == MarkerKind.Uncertain);
        Assert.Equal(uncertain.Bounds, uncertainMarker.Bounds);
        Assert.Equal("–", uncertainMarker.Value);
        Assert.False(uncertainMarker.IsNearest);
        Assert.Contains(presentation.Fields, row => row.FieldId == uncertain.RegionId && row.Value == "–");
        presentation.Cancel();
        Assert.Empty(presentation.Markers);
        Assert.Empty(presentation.Fields);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OppositeEndUsesTheSameBoundedContinuation(bool horizontal)
    {
        var baseline = new ImageAnalyzer().Analyze(CropDependencyTests.Scene(horizontal: horizontal, atEnd: true), Options);
        var result = new ImageAnalyzer().Analyze(CropDependencyTests.Scene(cropOffset: 6, horizontal: horizontal, atEnd: true), Options);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Equal(Trans(new(656, 810, 180, 150), horizontal), Assert.Single(result.UncertainRegions).Bounds);
        Assert.Equal(2, result.Fields.Count);
        Assert.All(result.Fields, field =>
        {
            var previous = Assert.Single(baseline.Fields, b => b.Bounds == field.Bounds);
            Assert.True(field.MeasurementAllowed);
            Assert.Equal(previous.Measurement, field.Measurement);
            Assert.Equal(previous.DeltaE00, field.DeltaE00);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NewlySelectedUnusableReferenceReleasesNoComparisons(bool horizontal)
    {
        // Disturb only the lower portion newly included by the moved reference.
        // Old reference ends at y=297; new one ends at 392.
        var image = CropDependencyTests.Scene((x, y, v) =>
            x >= 478 && x < 622 && y >= 300 && y < 392 ? (byte)130 : v,
            cropOffset: 6, horizontal: horizontal);
        var result = new ImageAnalyzer().Analyze(image, Options);
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.True(RegionSampler.Measure(image, Trans(new(478, 153, 144, 144), horizontal), Options).IsUsable);
        Assert.Equal(2, result.Fields.Count);
        Assert.Single(result.UncertainRegions);
        Assert.All(result.Fields, f =>
        {
            Assert.True(f.Measurement.IsUsable);
            Assert.False(f.Reference!.IsUsable);
            Assert.True(f.Reference.SpatialDeltaE > Options.MaximumSpatialDeltaE);
            Assert.Equal(Trans(new(478, 248, 144, 144), horizontal), f.Reference.Bounds);
            Assert.False(f.MeasurementAllowed); Assert.False(f.IsNearest); Assert.Null(f.DeltaE00);
        });
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.All(presentation.Fields, row => Assert.Equal("–", row.Value));
        Assert.DoesNotContain(presentation.Markers, m => m.Kind == MarkerKind.Measured || m.IsNearest);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LocalReferencesKeepTheirOwnDependency(bool horizontal)
    {
        var options = Options with { ReferenceMode = ReferenceMode.AdjacentPerFieldTrial };
        var image = CropDependencyTests.Scene((x,y,v) =>
            x >= 478 && x < 622 && y >= 183 && y < 327 ? (byte)(y < 255 ? 114 : 126) : v,
            cropOffset: 6, horizontal: horizontal);
        var result = new ImageAnalyzer().Analyze(image, options);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.False(result.Fields[0].Reference!.IsUsable);
        Assert.Null(result.Fields[0].DeltaE00);
        Assert.True(result.Fields[1].MeasurementAllowed);
        Assert.True(result.Fields[1].IsNearest);
    }

    [Theory]
    [InlineData("rotation")]
    [InlineData("ambiguous")]
    [InlineData("second-rest")]
    [InlineData("weak-rest")]
    [InlineData("missing-anchor")]
    [InlineData("inconsistent")]
    [InlineData("misaligned-interiors")]
    [InlineData("too-small")]
    public void UnsupportedGeometryCannotUseTheNewContinuation(string problem)
    {
        var image = CropDependencyTests.Scene(cropOffset: 6);
        var d = StripDetector.Detect(image, Options, default);
        Assert.True(MeasurementSafety.TryGetExcludedEndCrop(d, image.Width, image.Height, Options, false, out _));
        d = problem switch
        {
            "ambiguous" => d with { Ambiguous = true },
            "second-rest" => d with { BorderRegions = d.BorderRegions!.Append(new PixelRect(650, 850, 180, 110)).ToArray() },
            "weak-rest" => d with { WeakBorderRegions = [new(650, 850, 180, 110)] },
            "missing-anchor" => d with { Fields = [d.Fields[0]] },
            "inconsistent" => d with { InconsistentFields = [d.Fields[0]] },
            "misaligned-interiors" => d with { Fields = [d.Fields[0], d.Fields[1] with { X = 656 }] },
            "too-small" => d with { BorderRegions = [new(656, 0, 180, 20)] },
            _ => d
        };
        Assert.False(MeasurementSafety.TryGetExcludedEndCrop(d, image.Width, image.Height, Options, problem == "rotation", out _));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AdditionalRealBorderRemainderStillRejectsFrame(bool horizontal)
    {
        var image = CropDependencyTests.Scene((x,y,v) => x >= 650 && x < 830 && y >= 470 ? (byte)80 : v,
            cropOffset: 6, horizontal: horizontal);
        var result = new ImageAnalyzer().Analyze(image, Options);
        Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Empty(result.Fields);
        Assert.Empty(result.UncertainRegions);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(-3)]
    public void AppliedStraighteningDoesNotEnterTheNewContinuation(double degrees)
    {
        var source = CropDependencyTests.Scene(cropOffset: 6);
        var pixels = new byte[source.Width * source.Height * 3];
        double a = degrees * Math.PI / 180, c = Math.Cos(a), s = Math.Sin(a);
        for (int y = 0; y < source.Height; y++)
        for (int x = 0; x < source.Width; x++)
        {
            double dx = x - source.Width / 2d, dy = y - source.Height / 2d;
            int sx = (int)Math.Round(c * dx + s * dy + source.Width / 2d);
            int sy = (int)Math.Round(-s * dx + c * dy + source.Height / 2d);
            byte value = sx >= 0 && sx < source.Width && sy >= 0 && sy < source.Height
                ? source.GetPixel(sx, sy).R : (byte)120;
            int i = (y * source.Width + x) * 3;
            pixels[i] = pixels[i + 1] = pixels[i + 2] = value;
        }
        var result = new ImageAnalyzer().Analyze(new(source.Width, source.Height, source.Stride, pixels), Options);
        Assert.NotEqual(0, result.StraighteningDegrees);
        Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Empty(result.Fields);
        Assert.Empty(result.UncertainRegions);
    }

    [Fact]
    public void ActualCompetingPatternIsNotReleased()
    {
        var image = CropDependencyTests.Scene((x,y,v) =>
            y >= 650 && y < 830 && ((x >= 150 && x < 260) || (x >= 300 && x < 410)) ? (byte)80 : v,
            cropOffset: 6);
        var result = new ImageAnalyzer().Analyze(image, Options);
        Assert.Equal(AnalysisStatus.AmbiguousPattern, result.Status);
        Assert.Empty(result.Fields);
        Assert.Empty(result.UncertainRegions);
    }
}
