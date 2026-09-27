using System.IO;
using Iro.Analysis;
using Iro.Core.Analysis;
using static Iro.TestFixtures.ProjectiveFixture;

namespace IroGenTests;

public class PerspectiveMaskPngTests
{
    [Theory]
    [InlineData(false, false)] [InlineData(true, false)] [InlineData(false, true)] [InlineData(true, true)]
    public async Task PerspectiveMasksAndSafetySurvivePng(bool horizontal, bool strong)
    {
        string root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(root, "iro.slnx"))) root = Directory.GetParent(root)!.FullName;
        string folder = Path.Combine(root, "tests", "adjustments", "perspektivmasken-20260927");
        Directory.CreateDirectory(folder);
        foreach (int noise in new[] { 0, 2 }) foreach (bool reverse in new[] { false, true })
        {
            var frame = Scene(strong ? .6 : .1, horizontal, noise, reverse: reverse);
            var png = ReflectionPngTests.Png(frame);
            var actual = await new PngAnalysisApi().AnalyzeAsync(png, new());
            var direct = new ImageAnalyzer().Analyze(frame, new());
            Assert.Equal(direct.Status, actual.Status);
            Assert.Equal(direct.Fields.Count, actual.Fields.Count);
            for (int i = 0; i < direct.Fields.Count; i++)
            {
                Assert.Equal(direct.Fields[i].DeltaE00, actual.Fields[i].DeltaE00);
                Assert.Equal(direct.Fields[i].InnerPolygon, actual.Fields[i].InnerPolygon);
                Assert.Equal(direct.Fields[i].Measurement.Polygon, actual.Fields[i].Measurement.Polygon);
            }
            var display = new AnalysisPresentation(); display.Present(actual);
            if (strong)
            {
                Assert.DoesNotContain(actual.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
                Assert.All(display.Fields, f => Assert.Equal("–", f.Value));
            }
            else
            {
                Assert.Equal(AnalysisStatus.Measured, actual.Status);
                Assert.Equal(3, actual.Fields.Count);
                Assert.All(actual.Fields, f => Assert.True(f.MeasurementAllowed));
                Assert.All(display.Fields, f => Assert.NotEqual("–", f.Value));
            }
            File.WriteAllBytes(Path.Combine(folder, $"{(strong ? "stark" : "leicht")}-{(horizontal ? "waagerecht" : "senkrecht")}-rauschen-{noise}-gespiegelt-{reverse}.png"), png);
        }
    }
}
