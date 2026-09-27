using Iro.Core.Analysis;
using static Iro.TestFixtures.ProjectiveFixture;
using Xunit.Abstractions;

namespace Iro.Core.Tests;

public class CrossPerspectiveTests(ITestOutputHelper output)
{
    [Theory]
    [InlineData(false, 0)] [InlineData(false, 2)] [InlineData(true, 0)] [InlineData(true, 2)]
    public void StrongSidePerspectiveMustNotReleaseValues(bool horizontal, int noise)
    {
        foreach (double strength in new[] { .6, .9 }) foreach (bool reverse in new[] { false, true })
        {
            var frame = Scene(strength, horizontal, noise, reverse: reverse);
            var detection = StripDetector.Detect(frame, new(), default);
            var actual = new ImageAnalyzer().Analyze(frame, new());
            output.WriteLine($"k={strength}, reverse={reverse}, fields={detection.Fields.Count}, orientation={detection.Orientation}, changes={string.Join(';', detection.WidthChanges?.Values ?? [])}, status={actual.Status}");
            Assert.DoesNotContain(actual.Fields, f => f.MeasurementAllowed || f.DeltaE00 != null || f.IsNearest);
        }
    }
    [Theory]
    [InlineData(false, 0)] [InlineData(false, 2)] [InlineData(true, 0)] [InlineData(true, 2)]
    public void FrontalAndMildPerspectiveKeepOriginalColors(bool horizontal, int noise)
    {
        foreach (double strength in new[] { 0, .1 }) foreach (bool reverse in new[] { false, true })
        {
            var result = new ImageAnalyzer().Analyze(Scene(strength, horizontal, noise, reverse: reverse), new());
            Assert.True(result.Status == AnalysisStatus.Measured, $"{strength}/{horizontal}/{noise}/{reverse}: {result.Status}; {result.Hint}");
            Assert.Equal(3, result.Fields.Count);
            foreach (var f in result.Fields)
            {
                Assert.True(f.MeasurementAllowed);
                double error = Enumerable.Range(0, 3).Min(i => ColorMath.DeltaE00(f.Measurement.Lab!.Value, ColorMath.LinearToLab(ColorMath.Decode((byte)(60 + 30 * i)), ColorMath.Decode((byte)(75 + 30 * i)), ColorMath.Decode((byte)(90 + 30 * i)))));
                Assert.InRange(error, 0, noise == 0 ? 1e-8 : .15);
            }
        }
    }
    [Theory]
    [InlineData(false, -23)] [InlineData(false, 17)] [InlineData(true, -23)] [InlineData(true, 17)]
    public void MildPerspectiveAndRotationKeepOriginalColors(bool horizontal, double angle)
    {
        foreach (bool reverse in new[] { false, true })
        {
            var result = new ImageAnalyzer().Analyze(Scene(.1, horizontal, 2, angle, reverse), new());
            Assert.True(result.Status == AnalysisStatus.Measured, $"{horizontal}/{angle}/{reverse}: {result.Status} {result.Hint}");
            Assert.Equal(3, result.Fields.Count);
            foreach (var f in result.Fields)
            {
                Assert.True(f.MeasurementAllowed);
                Assert.NotNull(f.InnerPolygon);
                Assert.Equal(f.InnerPolygon, f.Measurement.Polygon);
                var wall = ColorMath.LinearToLab(ColorMath.Decode(140), ColorMath.Decode(140), ColorMath.Decode(140));
                Assert.InRange(ColorMath.DeltaE00(wall, f.Reference!.Lab!.Value), 0, .15);
                double error = Enumerable.Range(0, 3).Min(i => ColorMath.DeltaE00(f.Measurement.Lab!.Value, ColorMath.LinearToLab(ColorMath.Decode((byte)(60 + 30 * i)), ColorMath.Decode((byte)(75 + 30 * i)), ColorMath.Decode((byte)(90 + 30 * i)))));
                Assert.InRange(error, 0, .15);
            }
        }
    }}
