using Iro.Core.Analysis;
namespace Iro.Core.Tests;
public class CropSafetyRegressionTests
{
    [Theory]
    [InlineData(6, false, false)]
    [InlineData(16, false, false)]
    [InlineData(40, false, false)]
    [InlineData(110, false, false)]
    [InlineData(6, true, false)]
    [InlineData(16, true, false)]
    [InlineData(40, true, false)]
    [InlineData(110, true, false)]
    [InlineData(6, false, true)]
    [InlineData(16, false, true)]
    [InlineData(40, false, true)]
    [InlineData(110, false, true)]
    [InlineData(6, true, true)]
    [InlineData(16, true, true)]
    [InlineData(40, true, true)]
    [InlineData(110, true, true)]
    public void GradientAtAnyCropEdgeCannotDisappearFromSafetyDecision(int visible, bool turn, bool reverse)
    {
        var result = Analyze(visible, turn, reverse, 540);
        Assert.True(result.Fields.All(f => !f.MeasurementAllowed && f.DeltaE00 == null && !f.IsNearest),
            $"Cropped gradient released {result.Fields.Count(f => f.MeasurementAllowed)} fields; {result.Status}");
        Assert.Contains("vollständig", result.Hint);
    }
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void UnrelatedGradientAtBorderDoesNotInvalidateCompleteStrip(bool turn, bool reverse)
    {
        var result = Analyze(16, turn, reverse, 60);
        Assert.Equal(AnalysisStatus.Measured, result.Status);
        Assert.Equal(2, result.Fields.Count(f => f.MeasurementAllowed));
    }
    private static ImageAnalysis Analyze(int visible, bool turn, bool reverse, int borderX)
    {
        const int w = 800, h = 600;
        var data = Enumerable.Repeat((byte)140, w * h * 3).ToArray();
        for (int field = 0; field < 3; field++)
        {
            int top = field == 0 ? 0 : visible + 18 + (field - 1) * 128;
            int left = field == 0 ? borderX : 540;
            for (int y = top; y < top + (field == 0 ? visible : 110); y++)
            for (int x = left; x < left + 160; x++)
            for (int c = 0; c < 3; c++)
                data[((reverse ? h - 1 - y : y) * w + x) * 3 + c] = (byte)(60 + 30 * field + 15 * c + (field == 0 ? (x - left) * 35 / 159 : 0));
        }
        if (!turn) return new ImageAnalyzer().Analyze(new(w, h, w * 3, data), new());
        var rotated = new byte[data.Length];
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
            Array.Copy(data, (y * w + x) * 3, rotated, (x * h + h - 1 - y) * 3, 3);
        return new ImageAnalyzer().Analyze(new(h, w, h * 3, rotated), new());
    }
}
