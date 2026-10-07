using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

// Text/path regressions only; these tests do not validate the quality gates.
public sealed class QualityHintTests
{
    [Fact]
    public void SmallCandidatesDoNotClaimIncompletePatternOrPrescribeDistance()
    {
        var image = Image(300, 300, (x,y) => x >= 130 && x < 150 && y >= 130 && y < 150 ? (byte)60 : (byte)120);
        var detection = StripDetector.Detect(image, new(), default);
        Assert.True(detection.HadSmallRegions);
        Assert.Empty(detection.Fields);
        var result = new ImageAnalyzer().Analyze(image, new());
        Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Empty(result.Fields);
        AssertMessage(result, "Die erkannten kleinen Farbflächen reichen nicht für einen Farbvergleich aus.");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TaperObservationIsDiagnosticWithoutPrescribingCameraPosition(bool horizontal)
    {
        var image = Image(1440, 960, (x,y) =>
        {
            for (int i = 0; i < 3; i++)
                if (y >= 200 + 160*i && y < 320 + 160*i && Math.Abs(x - 800) < (280 - .2*y)/2)
                    return (byte)(40 + 20*i);
            return 160;
        }, horizontal);
        var options = new AnalysisOptions { Straighten = false };
        var detection = StripDetector.Detect(image, options, default);
        Assert.False(detection.Ambiguous);
        Assert.True(MeasurementSafety.HasStrongCoherentTaper(detection));
        var result = new ImageAnalyzer().Analyze(image, options);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, field => Assert.True(field.MeasurementAllowed));
        Assert.Contains("Die erkannten Feldbreiten verändern sich entlang des Streifens deutlich; diagnostischer Geometriebefund ohne eigene Messsperre.", result.Diagnostics);
        AssertMessage(result, "Farbabstand ΔE00 · kleiner = ähnlicher");
    }

    [Fact]
    public void OutlierReasonDoesNotClaimReflectionsOrLighting()
    {
        var image = Image(120, 120, (x,y) => (x+y)%4 == 0 ? (byte)140 : (byte)120);
        var options = new AnalysisOptions();
        var result = RegionSampler.Measure(image, new(0,0,120,120), options);
        Assert.True(result.RejectedFraction > options.MaximumOutlierFraction);
        Assert.True(result.ChannelMad <= options.MaximumChannelMad);
        Assert.True(result.SpatialDeltaE <= options.MaximumSpatialDeltaE);
        Assert.False(result.IsUsable);
        Assert.Null(result.Lab);
        Assert.Equal("Die Messfläche weist stark unterschiedliche Farbwerte auf und wird nicht ausgewertet.", result.Reason);
    }

    private static void AssertMessage(ImageAnalysis result, string expected)
    {
        Assert.Equal(expected, result.Hint);
        var presentation = new AnalysisPresentation(); presentation.Present(result);
        Assert.Equal(expected, presentation.Message);
    }

    private static RgbFrame Image(int width, int height, Func<int,int,byte> value, bool horizontal = false)
    {
        var data = new byte[width*height*3];
        int outputWidth = horizontal ? height : width;
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int offset = (horizontal ? x*outputWidth+y : y*width+x)*3;
            data[offset] = data[offset+1] = data[offset+2] = value(x,y);
        }
        return new(outputWidth, horizontal ? width : height, outputWidth*3, data);
    }
}
