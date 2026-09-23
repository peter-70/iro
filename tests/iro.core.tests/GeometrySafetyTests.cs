using Iro.Core.Analysis;

namespace Iro.Core.Tests;

public class GeometrySafetyTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void RectangularFieldsWithDifferentWidthsDoNotProvePerspective(bool horizontal, bool reverse)
    {
        var data = Empty();
        for (int field = 0; field < 3; field++)
        {
            int width = 180 - field * 30;
            int row = reverse ? 400 - field * 130 : 50 + field * 130;
            Paint(data, 620 - width / 2, row, width, 110, (byte)(60 + 30 * field));
        }
        var result = Run(data, horizontal);
        Assert.DoesNotContain("frontal", result.Hint);
        Assert.NotEqual(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Contains(result.Fields, f => f.MeasurementAllowed);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NarrowRectanglesAloneDoNotProvePhysicalCameraAngle(bool horizontal)
    {
        var data = Empty();
        for (int field = 0; field < 3; field++)
            Paint(data, 580, 50 + field * 130, 60, 110, (byte)(60 + 30 * field));
        var result = Run(data, horizontal);
        // This image can describe genuinely narrow frontal fields OR uniform
        // foreshortening. Pixel geometry cannot establish their physical shape.
        Assert.DoesNotContain("frontal", result.Hint);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(3, result.Fields.Count(f => f.MeasurementAllowed));
    }
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void TwoFieldsRequireContourEvidenceToRejectTaper(bool horizontal, bool tapered)
    {
        var data = Empty();
        for (int y = 50; y < 290; y++)
        {
            int field = (y - 50) / 130;
            if ((y - 50) % 130 >= 110) continue;
            int width = tapered ? 180 - (y - 50) / 3 : 180 - field * 44;
            Paint(data, 620 - width / 2, y, width, 1, (byte)(60 + 30 * field));
        }
        var result = Run(data, horizontal);
        if (tapered)
        {
            Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
            Assert.Contains("frontal", result.Hint);
            Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed);
        }
        else
        {
            Assert.Equal(AnalysisStatus.Measured, result.Status);
            Assert.Equal(2, result.Fields.Count(f => f.MeasurementAllowed));
            Assert.DoesNotContain("frontal", result.Hint);
        }
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OneTaperedContourDoesNotEstablishTwoFieldPerspective(bool horizontal)
    {
        var data = Empty();
        for (int y = 50; y < 290; y++)
        {
            int field = (y - 50) / 130;
            if ((y - 50) % 130 >= 110) continue;
            int width = field == 0 ? 180 - (y - 50) / 3 : 136;
            Paint(data, 620 - width / 2, y, width, 1, (byte)(60 + 30 * field));
        }
        var result = Run(data, horizontal);
        Assert.DoesNotContain("frontal", result.Hint);
        Assert.Contains(result.Fields, f => f.MeasurementAllowed);
    }
    private const int W = 800, H = 600;
    private static byte[] Empty() => Enumerable.Repeat((byte)140, W * H * 3).ToArray();
    private static void Paint(byte[] pixels, int x, int y, int width, int height, byte shade)
    {
        for (int yy = Math.Max(0, y); yy < Math.Min(H, y + height); yy++)
        for (int xx = Math.Max(0, x); xx < Math.Min(W, x + width); xx++)
        {
            int p = (yy * W + xx) * 3;
            pixels[p] = shade; pixels[p + 1] = (byte)(shade + 20); pixels[p + 2] = (byte)(shade + 50);
        }
    }
    private static ImageAnalysis Run(byte[] data, bool horizontal)
    {
        if (!horizontal) return new ImageAnalyzer().Analyze(new(W, H, W * 3, data), new());
        var turned = new byte[data.Length];
        for (int y = 0; y < H; y++)
        for (int x = 0; x < W; x++)
            Array.Copy(data, (y * W + x) * 3, turned, (x * H + H - y - 1) * 3, 3);
        return new ImageAnalyzer().Analyze(new(H, W, H * 3, turned), new());
    }
    [Theory]
    [InlineData(false, 6)]
    [InlineData(false, 16)]
    [InlineData(true, 6)]
    [InlineData(true, 16)]
    public void SmallVisibleRemnantOfCroppedFieldStillStopsMeasurement(bool horizontal, int visibleHeight)
    {
        var data = Empty();
        Paint(data, 540, 0, 160, visibleHeight, 60);
        Paint(data, 540, visibleHeight + 18, 160, 100, 90);
        Paint(data, 540, visibleHeight + 136, 160, 110, 120);
        var result = Run(data, horizontal);
        Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Contains("vollständig", result.Hint);
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed);
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void UnrelatedSmallBorderPatchDoesNotInvalidateCompleteStrip(bool horizontal)
    {
        var data = Empty();
        Paint(data, 60, 0, 160, 12, 60);
        Paint(data, 540, 60, 160, 100, 90);
        Paint(data, 540, 178, 160, 110, 120);
        var result = Run(data, horizontal);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(2, result.Fields.Count(f => f.MeasurementAllowed));
    }
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, true, false)]
    [InlineData(false, false, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void StrongCoherentTaperStopsTheWholeStrip(bool horizontal, bool reverse, bool dimAndNoisy)
    {
        var data = Empty();
        // Three trapezoids on one continuously converging strip outline. No camera
        // angle or nominal palette is passed to the analyzer.
        for (int y = 50; y < 440; y++)
        {
            int field = (y - 50) / 130;
            if ((y - 50) % 130 >= 110) continue;
            int width = 180 - (y - 50) / 5;
            int row = reverse ? H - 1 - y : y;
            Paint(data, 620 - width / 2, row, width, 1, (byte)(60 + 30 * field));
        }
        if (dimAndNoisy)
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)((int)(data[i] * .65) + (i % 5) - 2);
        var result = Run(data, horizontal);
        Assert.Equal(AnalysisStatus.UnsuitableGeometry, result.Status);
        Assert.Contains("frontal", result.Hint);
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed);
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DifferentFieldLengthsAndSlightTaperRemainMeasurable(bool horizontal)
    {
        var data = Empty();
        int[] tops = [40, 120, 280, 390];
        int[] lengths = [60, 140, 90, 150];
        for (int field = 0; field < tops.Length; field++)
        {
            int width = 170 - field * 2;
            Paint(data, 620 - width / 2, tops[field], width, lengths[field], (byte)(50 + 30 * field));
        }
        var result = Run(data, horizontal);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(4, result.Fields.Count(f => f.MeasurementAllowed));
    }
}