using IroGen;
using Iro.Core.Analysis;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xunit.Abstractions;

namespace IroGenTests;

public class StraighteningTests(ITestOutputHelper output)
{
    private static GeneratedScene Generate(GeneratorOptions options)
    {
        GeneratedScene result = null!; Exception? error = null;
        var thread = new Thread(() => { try { result = SceneGenerator.Generate(options); } catch (Exception e) { error = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        return result;
    }
    private static RgbFrame Frame(GeneratedScene scene)
    {
        var bitmap = new FormatConvertedBitmap(scene.Image, PixelFormats.Rgb24, null, 0);
        byte[] bytes = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 3];
        bitmap.CopyPixels(bytes, bitmap.PixelWidth * 3, 0);
        return new(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.PixelWidth * 3, bytes);
    }
    [Fact]
    public void ClearStripsAtAllAnglesRemainMeasurable()
    {
        var failures = new List<string>();
        foreach (var orientation in Enum.GetValues<StripOrientation>())
        foreach (double angle in new[] { -80d, -60, -45, -30, -10, 0, 10, 30, 45, 60, 80 })
        {
            var scene = Generate(new() { Width=1000, Height=1000, Position=StripPosition.Center,
                StripLengthPercent=65, Orientation=orientation, RotationDegrees=angle });
            var result = new ImageAnalyzer().Analyze(Frame(scene), new());
            string detail = $"{orientation} {angle}: angle={result.StraighteningDegrees}, {result.Status}, {result.Fields.Count(f=>f.MeasurementAllowed)}/{result.Fields.Count}; {string.Join(" / ", result.Fields.Select(f=>f.Hint).Distinct())}";
            output.WriteLine(detail);
            if (result.Fields.Count(f=>f.MeasurementAllowed) != 7) failures.Add(detail);
            double expectedAngle = angle - 90 * Math.Floor((angle + 45) / 90);
            double difference = Math.Abs(result.StraighteningDegrees + expectedAngle);
            if (Math.Min(difference,90-difference) > .6) failures.Add("Wrong angle: "+detail);
            Assert.Equal(1000, result.Width); Assert.Equal(1000,result.Height);
            foreach (var field in result.Fields.Where(f=>f.MeasurementAllowed))
            {
                double cx = field.Polygon?.Average(p=>p.X) ?? field.Bounds.X+field.Bounds.Width/2d;
                double cy = field.Polygon?.Average(p=>p.Y) ?? field.Bounds.Y+field.Bounds.Height/2d;
                var nominal = scene.Fields.MinBy(f => Math.Pow(f.Polygon.Average(p=>p.X)-cx,2)+Math.Pow(f.Polygon.Average(p=>p.Y)-cy,2))!;
                Assert.InRange(Math.Abs(field.DeltaE00!.Value-nominal.DeltaE00),0,.15);
            }
        }
        Assert.True(failures.Count == 0, string.Join("\n", failures));
    }

    [Fact]
    public void RandomAnglesRecoverClearFieldsAndKeepSevereCombinationsRejected()
    {
        var plan = TestPlan.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory,
            "testplans","raumlage-perspektive-und-wandabstand.json")));
        foreach (var (entry, options) in plan.Expand().Where(e=>e.Options.RandomPlacement))
        for (int n=0;n<entry.Count;n++)
        {
            var scene = Generate(options with { Seed=unchecked(options.Seed+n*(int)0x9E3779B9u) });
            var result = new ImageAnalyzer().Analyze(Frame(scene),new());
            bool cropped = scene.Fields.Any(f => f.Polygon.Any(p => p.X < 0 || p.Y < 0 || p.X > scene.Options.Width || p.Y > scene.Options.Height));
            if (options.MotionBlur == Severity.None && !cropped)
            {
                Assert.InRange(result.Fields.Count(f => f.MeasurementAllowed), 5, 7);
                Assert.All(result.Fields.Where(f => f.MeasurementAllowed), f =>
                {
                    var polygon = f.Polygon!;
                    Assert.NotNull(polygon);
                    double cx = polygon.Average(p => p.X), cy = polygon.Average(p => p.Y);
                    var nominal = scene.Fields.MinBy(field => Math.Pow(field.Polygon.Average(p => p.X)-cx, 2) + Math.Pow(field.Polygon.Average(p => p.Y)-cy, 2))!;
                    Assert.InRange(Math.Abs(f.DeltaE00!.Value - nominal.DeltaE00), 0, .15);
                    Assert.Equal(f.InnerPolygon, f.Measurement.Polygon);
                });
            }
            else Assert.DoesNotContain(result.Fields, f => f.MeasurementAllowed);
            output.WriteLine($"{entry.Name} seed={scene.Options.Seed} sourceAngle={scene.SpatialGeometry!.RotationDegrees:F2} corrected={result.StraighteningDegrees} {result.Status} fields={result.Fields.Count(f=>f.MeasurementAllowed)}/{result.Fields.Count}; {string.Join(" / ",result.Fields.Select(f=>f.Hint).Distinct())}");
        }
    }
}
