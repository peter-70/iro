using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

public sealed class SpatialDependencyTests
{
    private static readonly AnalysisOptions Options = new();
    private static readonly PixelRect First = new(650, 230, 180, 110);

    [Fact]
    public void UnevenFieldPreservesNeighborDistances()
    {
        var baseline = Analyze(Scene());
        var result = Analyze(Scene((x, y, v) => First.Contains(x, y) ? (byte)(x < 740 ? 110 : 122) : v));
        Assert.True(result.Fields[0].Measurement.SpatialDeltaE > Options.MaximumSpatialDeltaE);
        AssertPartialWithUnchangedNeighbors(result, baseline);
        AssertFieldHint(result, "Die Messfläche weist räumlich unterschiedliche Farbwerte auf und wird nicht ausgewertet.");
    }

    [Fact]
    public void UnevenControlSurfaceOnlyInvalidatesItsField()
    {
        var image = Scene((x, y, v) => First.Contains(x, y) && !First.Inset(Options.InnerMargin).Contains(x, y) ? (byte)104 : v);
        var inner = RegionSampler.Measure(image, First.Inset(Options.InnerMargin), Options);
        Assert.True(inner.IsUsable, inner.Reason);
        var result = Analyze(image);
        Assert.True(result.Fields[0].SurfaceSpatialDeltaE > Options.MaximumSpatialDeltaE);
        Assert.True(result.Fields[0].Measurement.SpatialDeltaE <= Options.MaximumSpatialDeltaE);
        AssertPartialWithUnchangedNeighbors(result, Analyze(Scene()));
        AssertFieldHint(result, "Die Farbfläche weist räumlich unterschiedliche Farbwerte auf. Für dieses Feld wird kein Farbvergleich ausgegeben.");
    }

    [Fact]
    public void AlreadyUnusableFieldDoesNotPoisonNeighbors()
    {
        // Detection samples odd coordinates at scale 2. The measurement sees the original texture too.
        var image = Scene((x, y, v) => First.Contains(x, y)
            ? (byte)(x % 2 == 1 && y % 2 == 1 ? 116 : x < 740 ? 80 : 152) : v);
        var local = RegionSampler.Measure(image, First.Inset(Options.InnerMargin), Options);
        Assert.True(local.ChannelMad > Options.MaximumChannelMad);
        Assert.True(local.SpatialDeltaE > Options.MaximumSpatialDeltaE);
        Assert.False(local.IsUsable);
        AssertPartialWithUnchangedNeighbors(Analyze(image), Analyze(Scene()));
    }

    [Fact]
    public void UnevenLocalReferenceOnlyInvalidatesDependentComparison()
    {
        var options = Options with { ReferenceMode = ReferenceMode.AdjacentPerFieldTrial };
        var baseline = Analyze(Scene(), options);
        var reference = baseline.Fields[0].Reference!.Bounds;
        var image = Scene((x, y, v) => reference.Contains(x, y) ? (byte)(x < reference.X + reference.Width / 2 ? 114 : 126) : v);
        var result = Analyze(image, options);
        Assert.True(result.Fields[0].Reference!.SpatialDeltaE > Options.MaximumSpatialDeltaE);
        Assert.False(result.Fields[0].Reference!.IsUsable);
        AssertPartialWithUnchangedNeighbors(result, baseline);
        AssertFieldHint(result, "Die Messfläche weist räumlich unterschiedliche Farbwerte auf und wird nicht ausgewertet.");
    }

    [Fact]
    public void UnevenSharedReferenceStillInvalidatesAllDependentComparisons()
    {
        var baseline = Analyze(Scene());
        var reference = baseline.Fields[0].Reference!.Bounds;
        var result = Analyze(Scene((x, y, v) => reference.Contains(x, y) ? (byte)(x < reference.X + reference.Width / 2 ? 114 : 126) : v));
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.Equal(3, result.Fields.Count);
        AssertFieldHint(result, "Die Messfläche weist räumlich unterschiedliche Farbwerte auf und wird nicht ausgewertet.");
        Assert.Equal(result.Fields[0].Hint, result.Hint);
        Assert.All(result.Fields, field =>
        {
            Assert.True(field.Reference!.SpatialDeltaE > Options.MaximumSpatialDeltaE);
            Assert.False(field.Reference.IsUsable);
            Assert.False(field.MeasurementAllowed);
            Assert.Null(field.DeltaE00);
            Assert.False(field.IsNearest);
        });
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.All(presentation.Fields, field => Assert.Equal("–", field.Value));
    }

    [Fact]
    public void BroadEdgesPreserveUsableComparisonsAndRemainDiagnostic()
    {
        // Test-input blur: spread both the field/border and border/wall transitions.
        var sharp = Scene();
        var pixels = sharp.Pixels.ToArray();
        for (int y = First.Y; y < First.Bottom; y++)
        for (int x = First.X - 24; x < First.Right + 24; x++)
        {
            int sum = 0;
            for (int dx = -8; dx <= 8; dx++) sum += sharp.GetPixel(x + dx, y).R;
            int offset = (y * sharp.Width + x) * 3;
            pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = (byte)(sum / 17);
        }
        var image = new RgbFrame(sharp.Width, sharp.Height, sharp.Stride, pixels);
        var result = Analyze(image);
        Assert.Equal(3, result.Fields.Count);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Contains(result.Diagnostics, d => d.Contains("Breiter Farbübergang"));
        var baseline = Analyze(sharp);
        for (int i = 0; i < result.Fields.Count; i++)
        {
            Assert.True(result.Fields[i].MeasurementAllowed);
            Assert.Equal(baseline.Fields[i].DeltaE00!.Value, result.Fields[i].DeltaE00!.Value, 9);
            Assert.Equal(baseline.Fields[i].IsNearest, result.Fields[i].IsNearest);
        }
        var presentation = new AnalysisPresentation(); presentation.Present(result);
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));
    }

    [Fact]
    public void ColorDispersionHasNoClaimedCauseAndKeepsOtherComparisons()
    {
        var image = Scene((x,y,v) => First.Contains(x,y)
            ? (byte)(x % 2 == 1 && y % 2 == 1 ? 116 : x % 4 < 2 ? 80 : 152) : v);
        var result = Analyze(image);
        Assert.True(result.Fields[0].Measurement.ChannelMad > Options.MaximumChannelMad);
        Assert.True(result.Fields[0].Measurement.SpatialDeltaE <= Options.MaximumSpatialDeltaE);
        AssertPartialWithUnchangedNeighbors(result, Analyze(Scene()));
        AssertFieldHint(result, "Die Messfläche weist stark unterschiedliche Farbwerte auf und wird nicht ausgewertet.");
    }

    private static void AssertFieldHint(ImageAnalysis result, string expected)
    {
        Assert.Equal(expected, result.Fields[0].Hint);
        var presentation = new AnalysisPresentation(); presentation.Present(result);
        Assert.Equal(expected, presentation.Fields[0].Hint);
        Assert.Equal(result.Hint, presentation.Message);
    }

    private static void AssertPartialWithUnchangedNeighbors(ImageAnalysis result, ImageAnalysis baseline)
    {
        Assert.Equal(AnalysisStatus.Measured, baseline.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.False(result.Fields[0].MeasurementAllowed);
        Assert.Null(result.Fields[0].DeltaE00);
        Assert.False(result.Fields[0].IsNearest);
        for (int i = 1; i < 3; i++)
        {
            var field = result.Fields[i];
            Assert.True(field.Measurement.IsUsable, field.Measurement.Reason);
            Assert.True(field.Reference!.IsUsable, field.Reference.Reason);
            Assert.True(field.MeasurementAllowed, field.Hint);
            Assert.Equal(baseline.Fields[i].Measurement.Lab, field.Measurement.Lab);
            Assert.Equal(baseline.Fields[i].Reference!.Lab, field.Reference.Lab);
            Assert.Equal(baseline.Fields[i].DeltaE00, field.DeltaE00);
        }
        Assert.True(result.Fields[1].IsNearest);
        Assert.False(result.Fields[2].IsNearest);
        var presentation = new AnalysisPresentation();
        presentation.Present(result);
        Assert.Equal("–", presentation.Fields[0].Value);
        Assert.All(presentation.Fields.Skip(1), field => Assert.NotEqual("–", field.Value));
    }

    private static ImageAnalysis Analyze(RgbFrame image, AnalysisOptions? options = null)
        => new ImageAnalyzer().Analyze(image, options ?? Options);

    private static RgbFrame Scene(Func<int, int, byte, byte>? transform = null)
    {
        const int width = 1440, height = 960;
        var pixels = new byte[width * height * 3];
        byte[] colors = [116, 123, 153];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            byte value = 120;
            for (int i = 0; i < 3; i++)
            {
                int top = 230 + i * 150;
                if (x >= 643 && x < 837 && y >= top - 7 && y < top + 117)
                    value = x >= 650 && x < 830 && y >= top && y < top + 110 ? colors[i] : (byte)65;
            }
            value = transform?.Invoke(x, y, value) ?? value;
            int offset = (y * width + x) * 3;
            pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = value;
        }
        return new(width, height, width * 3, pixels);
    }
}

internal static class TestRectContainment
{
    internal static bool Contains(this PixelRect rect, int x, int y)
        => x >= rect.X && x < rect.Right && y >= rect.Y && y < rect.Bottom;
}
