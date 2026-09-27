using Iro.Core.Analysis;

namespace Iro.Core.Tests;

public class RotatedInteriorSafetyTests
{
    const int Size = 800;
    static (double X, double Y) Local(double x, double y, double angle)
    {
        double c = Math.Cos(angle * Math.PI / 180), s = Math.Sin(angle * Math.PI / 180);
        return (c * (x - 400) + s * (y - 400) + 400, -s * (x - 400) + c * (y - 400) + 400);
    }
    static PixelRect Rect(int i, bool small, bool tiny = false)
    {
        int w = tiny ? 26 : small ? 64 : 140, h = tiny ? 22 : small ? 52 : 110;
        return new(480, 190 + i * (h + 18), w, h);
    }
    static bool Inside(double x, double y, PixelRect r, double margin = 0) => x >= r.X + margin && x < r.Right - margin && y >= r.Y + margin && y < r.Bottom - margin;
    static RgbColor Color(int i) => new((byte)(60 + 30 * i), (byte)(75 + 30 * i), (byte)(90 + 30 * i));
    static RgbFrame Make(double angle, bool small, string print, bool tiny = false, bool competitor = false)
    {
        var pixels = new byte[Size * Size * 3];
        for (int y = 0; y < Size; y++) for (int x = 0; x < Size; x++)
        {
            var p = Local(x + .5, y + .5, angle);
            var color = new RgbColor(140, 140, 140);
            for (int i = 0; i < 3; i++)
            {
                var r = Rect(i, small, tiny);
                // White separators/borders surround each true material rectangle.
                if (Inside(p.X, p.Y, new(r.X - 3, r.Y - 3, r.Width + 6, r.Height + 6))) color = new(245, 245, 245);
                if (!Inside(p.X, p.Y, r)) continue;
                color = Color(i);
                double u = p.X - r.X, v = p.Y - r.Y;
                bool ink = print == "Randdruck" ? u >= 4 && u < 8 && v >= 8 && v < r.Height - 8
                    : print == "Innendruck" && u >= r.Width / 2d - 2 && u < r.Width / 2d + 2 && v >= r.Height / 2d - 6 && v < r.Height / 2d + 6;
                if (ink) color = new(20, 20, 20);
            }
            // Independent horizontal row away from the main strip, not supplied as a mask.
            if (competitor && y >= 610 && y < 685 && x >= 120 && x < 354 && (x - 120) % 82 < 70)
                color = Color((x - 120) / 82);
            int offset = (y * Size + x) * 3;
            pixels[offset] = color.R; pixels[offset + 1] = color.G; pixels[offset + 2] = color.B;
        }
        return new(Size, Size, Size * 3, pixels);
    }
    static PixelPoint[] Polygon(PixelRect r) => [new(r.X, r.Y), new(r.Right, r.Y), new(r.Right, r.Bottom), new(r.X, r.Bottom)];
    static void CheckReleased(ImageAnalysis result, double angle, bool small, bool requireAll)
    {
        var allowed = result.Fields.Where(f => f.MeasurementAllowed).ToArray();
        if (requireAll)
        {
            Assert.True(result.Status == AnalysisStatus.Measured, $"{angle}/{small}: {result.Status}; {result.Hint}");
            Assert.Equal(3, allowed.Length);
        }
        var identities = new HashSet<int>();
        foreach (var f in allowed)
        {
            var polygon = f.InnerPolygon ?? Polygon(f.InnerBounds);
            var center = Local(polygon.Average(p => p.X), polygon.Average(p => p.Y), angle);
            int index = Enumerable.Range(0, 3).Single(i => Inside(center.X, center.Y, Rect(i, small)));
            Assert.True(identities.Add(index), "Ein echtes Feld darf nicht mehrfach freigegeben werden.");
            foreach (var point in polygon)
            {
                var local = Local(point.X, point.Y, angle);
                Assert.True(Inside(local.X, local.Y, Rect(index, small), 2), $"Messmaske verlässt sicheres Feldinneres: {angle}/{small}/{index}: {local}");
            }
            var color = Color(index);
            var expected = ColorMath.LinearToLab(ColorMath.Decode(color.R), ColorMath.Decode(color.G), ColorMath.Decode(color.B));
            var wall = ColorMath.LinearToLab(ColorMath.Decode(140), ColorMath.Decode(140), ColorMath.Decode(140));
            Assert.InRange(ColorMath.DeltaE00(expected, f.Measurement.Lab!.Value), 0, 1e-8);
            Assert.InRange(ColorMath.DeltaE00(wall, f.Reference!.Lab!.Value), 0, 1e-8);
            Assert.InRange(Math.Abs(f.DeltaE00!.Value - ColorMath.DeltaE00(expected, wall)), 0, 1e-8);
        }
        Assert.All(result.Fields.Where(f => !f.MeasurementAllowed), f => { Assert.Null(f.DeltaE00); Assert.False(f.IsNearest); });
    }
    [Theory]
    [InlineData(-80)] [InlineData(-45)] [InlineData(-27)] [InlineData(0)] [InlineData(13)] [InlineData(37)] [InlineData(44.5)] [InlineData(80)]
    public void CleanAndEdgePrintedFieldsKeepSafeMasksAndUnmixedColors(double angle)
    {
        foreach (bool small in new[] { false, true }) foreach (string print in new[] { "Kein Druck", "Randdruck" })
            CheckReleased(new ImageAnalyzer().Analyze(Make(angle, small, print), new()), angle, small, true);
    }
    [Theory]
    [InlineData(-80)] [InlineData(-45)] [InlineData(-27)] [InlineData(0)] [InlineData(13)] [InlineData(37)] [InlineData(44.5)] [InlineData(80)]
    public void InteriorInkIsExcludedOrMeasurementRejectedNeverMixedIntoColor(double angle)
    {
        foreach (bool small in new[] { false, true })
            CheckReleased(new ImageAnalyzer().Analyze(Make(angle, small, "Innendruck"), new()), angle, small, false);
    }
    [Theory]
    [InlineData(-45)] [InlineData(0)] [InlineData(37)] [InlineData(80)]
    public void TooSmallOriginalFieldsNeverGetReleased(double angle)
    {
        var result = new ImageAnalyzer().Analyze(Make(angle, true, "Kein Druck", tiny: true), new());
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
    }
    [Theory]
    [InlineData(-27)] [InlineData(0)] [InlineData(37)]
    public void CompetingRowsCannotBeSilentlyUsedAsWallOrChosenArbitrarily(double angle)
    {
        var result = new ImageAnalyzer().Analyze(Make(angle, false, "Kein Druck", competitor: true), new());
        Assert.Equal(AnalysisStatus.AmbiguousPattern, result.Status);
        Assert.Contains("Mehrere mögliche Muster", result.Hint);
        Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
    }
}
