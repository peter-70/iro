using Iro.Core.Analysis;
using Xunit;

namespace Iro.Core.Tests;

public sealed class VisibilityHintTests
{
    private const string Ambiguous = "Das Vergleichsmuster konnte nicht eindeutig zugeordnet werden. Es werden keine Farbvergleiche ausgegeben.";
    private const string Crop = "Die Feldgeometrie am Bildrand konnte nicht sicher zugeordnet werden. Es werden keine Farbvergleiche ausgegeben.";
    private const string Geometry = "Die Feldgrenzen stimmen nicht mit der erkannten Streifengeometrie überein. Für dieses Feld ist kein Farbvergleich verfügbar.";
    private const string Reference = "Für dieses Feld wurde keine geeignete Wandreferenz gefunden. Ein Farbvergleich ist nicht verfügbar.";

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RejectedEndRemaindersDescribeOnlyGeometry(bool horizontal)
    {
        var image = CropDependencyTests.Scene((x,y,v) => x >= 650 && x < 830 && y >= 470 ? (byte)80 : v,
            cropOffset: 6, horizontal: horizontal);
        var result = new ImageAnalyzer().Analyze(image, new());
        Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Empty(result.Fields);
        AssertMessage(result, Crop);
    }

    [Fact]
    public void CompetingPatternsDoNotRequestANewPhoto()
    {
        var image = CropDependencyTests.Scene((x,y,v) =>
            y >= 650 && y < 830 && ((x >= 150 && x < 260) || (x >= 300 && x < 410)) ? (byte)80 : v,
            cropOffset: 6);
        Assert.True(StripDetector.Detect(image, new(), default).Ambiguous);
        var result = new ImageAnalyzer().Analyze(image, new());
        Assert.Equal(AnalysisStatus.AmbiguousPattern, result.Status);
        AssertMessage(result, Ambiguous);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InconsistentFieldHasLocalGeometryReasonAndPreservesOtherComparisons(bool horizontal)
    {
        var image = Rectangles(960, 720, [new(400, 100, 180, 100), new(420, 250, 160, 100), new(400, 400, 180, 100)], horizontal);
        var result = new ImageAnalyzer().Analyze(image, new());
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        var bad = Assert.Single(result.Fields, f => !f.MeasurementAllowed);
        Assert.Equal(Geometry, bad.Hint);
        Assert.Null(bad.DeltaE00);
        Assert.False(bad.IsNearest);
        Assert.Equal(2, result.Fields.Count(f => f.MeasurementAllowed && f.DeltaE00.HasValue));
        Assert.Contains("nur für die auswertbaren sichtbaren Felder", result.Hint);
        var presentation = new AnalysisPresentation(); presentation.Present(result);
        Assert.Equal(Geometry, Assert.Single(presentation.Fields, f => f.FieldId == bad.FieldId).Hint);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MissingReferenceDoesNotClaimMissingWallOrDemandFullVisibility(bool horizontal)
    {
        var image = Rectangles(300, 720, [new(30, 100, 240, 100), new(30, 250, 240, 100), new(30, 400, 240, 100)], horizontal);
        var result = new ImageAnalyzer().Analyze(image, new());
        Assert.Equal(AnalysisStatus.InvalidReference, result.Status);
        Assert.Equal(3, result.Fields.Count);
        Assert.All(result.Fields, f =>
        {
            Assert.True(f.Measurement.IsUsable);
            Assert.Null(f.Reference); Assert.Null(f.DeltaE00);
            Assert.False(f.MeasurementAllowed); Assert.False(f.IsNearest);
            Assert.Equal(Reference, f.Hint);
        });
        AssertMessage(result, Reference);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PartialResultLimitsItsStatementToMeasuredFields(bool horizontal)
    {
        var result = new ImageAnalyzer().Analyze(CropDependencyTests.Scene(cropOffset: 6, horizontal: horizontal), new());
        Assert.Equal(AnalysisStatus.PartiallyMeasured, result.Status);
        Assert.Single(result.UncertainRegions);
        Assert.Equal(2, result.Fields.Count(f => f.MeasurementAllowed));
        Assert.Contains("Ergebnisse und nächstliegendes Feld beziehen sich ausschließlich auf diese Felder", result.Hint);
        Assert.Contains("Gesamtzahl der Felder ist unbekannt", result.Hint);
        AssertMessage(result, result.Hint);
    }

    [Fact]
    public void CompetingPatternAfterStraighteningUsesTheSameNeutralHint()
    {
        const int width = 1440, height = 960;
        var data = new byte[width * height * 3];
        double angle = 15 * Math.PI / 180, c = Math.Cos(angle), s = Math.Sin(angle);
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            double ax = c * (x - 850) + s * (y - 480) + 850;
            double ay = -s * (x - 850) + c * (y - 480) + 480;
            byte v = 120;
            for (int i = 0; i < 4; i++)
                if (ax >= 750 && ax < 950 && ay >= 150 + 170 * i && ay < 280 + 170 * i) v = (byte)(30 + 20 * i);
            if (y >= 300 && y < 480 && ((x >= 100 && x < 210) || (x >= 260 && x < 370))) v = 65;
            int offset = (y * width + x) * 3;
            data[offset] = data[offset + 1] = data[offset + 2] = v;
        }
        var image = new RgbFrame(width, height, width * 3, data);
        var alignment = ImageStraightener.Estimate(image, default);
        Assert.NotEqual(0, alignment.Degrees);
        var rotation = new ImageRotation(width, height, alignment.Degrees);
        var aligned = StripDetector.Detect(new RgbFrame(image, rotation), new(), default);
        Assert.False(aligned.Ambiguous);
        Assert.NotEmpty(aligned.Fields);
        var original = StripDetector.Detect(image, new(), default);
        Assert.True(original.Ambiguous || original.Fields.Count(f =>
        {
            var center = rotation.ToAligned(f.X + f.Width / 2d, f.Y + f.Height / 2d);
            return !aligned.Fields.Any(a => center.X >= a.X && center.X < a.Right && center.Y >= a.Y && center.Y < a.Bottom);
        }) >= 2);
        var result = new ImageAnalyzer().Analyze(image, new());
        Assert.Equal(AnalysisStatus.AmbiguousPattern, result.Status);
        Assert.Empty(result.Fields);
        AssertMessage(result, Ambiguous);
    }

    private static void AssertMessage(ImageAnalysis result, string expected)
    {
        Assert.Equal(expected, result.Hint);
        var presentation = new AnalysisPresentation(); presentation.Present(result);
        Assert.Equal(expected, presentation.Message);
    }

    private static RgbFrame Rectangles(int width, int height, PixelRect[] rectangles, bool horizontal)
    {
        var data = new byte[width * height * 3];
        int outputWidth = horizontal ? height : width;
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            byte v = 120;
            for (int i = 0; i < rectangles.Length; i++) if (rectangles[i].Contains(x,y)) v = (byte)(40 + 20 * i);
            int offset = (horizontal ? x * outputWidth + y : y * width + x) * 3;
            data[offset] = data[offset + 1] = data[offset + 2] = v;
        }
        return new(outputWidth, horizontal ? width : height, outputWidth * 3, data);
    }
}
