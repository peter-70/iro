using IroGen;
using Iro.Core.Analysis;
using System.Windows.Media;
using System.IO;
using System.Text.Json.Nodes;

namespace IroGenTests;

public class TestPlanTests
{
    private static T Sta<T>(Func<T> action)
    {
        T result = default!; Exception? error = null;
        var thread = new Thread(() => { try { result = action(); } catch (Exception e) { error = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        return result;
    }

    [Fact]
    public void PlansMergeOptionsAndRejectMistakesBeforeGenerating()
    {
        var plan = TestPlan.Parse("""{"formatVersion":1,"kind":"irogen-test-plan","name":"Test","defaults":{"width":800,"height":600},"cases":[{"name":"Kontrolle","count":2,"coverRange":false,"options":{}},{"name":"Verdeckung","count":2,"coverRange":false,"options":{"occlusion":"Medium"}}]}""");
        Assert.Equal(800, plan.Expand()[1].Options.Width);
        Assert.Equal(Severity.Medium, plan.Expand()[1].Options.Occlusion);
        Assert.ThrowsAny<Exception>(() => TestPlan.Parse("""{"formatVersion":1,"kind":"irogen-test-plan","name":"Test","defaults":{},"cases":[{"name":"X","count":1,"options":{"blurr":"Strong"}}]}"""));
        Assert.ThrowsAny<Exception>(() => (plan with { Cases = [plan.Cases[0], plan.Cases[0]] }).Expand());
        Assert.ThrowsAny<Exception>(() => (plan with { Cases = [plan.Cases[0] with { Count = 10001 }] }).Expand());
    }

    [Fact]
    public async Task PlanGeneratesAllCasesWithReproduciblePairedSeeds()
    {
        string root = Path.Combine(Path.GetTempPath(), "iro-plan-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var plan = new TestPlan(1, "irogen-test-plan", "Vergleich", new() { Width = 800, Height = 600 },
                [new("Kontrolle", 2, false, new()), new("Verdeckung", 2, false, new() { ["occlusion"] = "Medium" })]);
            using var batch = Sta(() => plan.Generate(root));
            Assert.Equal(4, batch.Items.Count);
            Assert.Equal(batch.Items[0].Options.Seed, batch.Items[2].Options.Seed);
            Assert.Equal(batch.Items[0].Fields.Select(f => f.Hex), batch.Items[2].Fields.Select(f => f.Hex));
            Assert.Equal("Verdeckung", batch.Items[2].Options.TestCaseName);
            Assert.All(batch.Items, i => Assert.True(File.Exists(i.ImagePath)));
            File.WriteAllText(Path.Combine(root, "iro.slnx"), "<Solution/>");
            File.WriteAllText(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolierter Test");
            var request = IroTestHandoff.Submit(batch, root);
            var run = await new Iro.Analysis.AnalysisRunner().RunAsync(request.Folder, Path.Combine(root, "tests", "runs"), new());
            var review = TestRunReview.Load(root, 1);
            Assert.Equal(4, review.Count);
            Assert.Contains(review, r => r.CaseName == "Verdeckung");
            Assert.All(review, r => { Assert.NotNull(r.CaptureId); Assert.Equal("completed", r.RunStatus); });
            Assert.Contains("Verdeckung", TestReviewExport.Create(review, 1, false));
            using var cancel = new CancellationTokenSource(); cancel.Cancel();
            Assert.Throws<OperationCanceledException>(() => Sta(() => plan.Generate(root, token: cancel.Token)));
            Assert.Single(Directory.GetDirectories(root, "irogen-series-*"));
        }
        finally { Directory.Delete(root, true); }
    }

    [Fact]
    public void DistanceStepsChangeProjectedSizeMonotonically()
    {
        double previous = 0;
        foreach (var distance in new[] { CameraDistance.TooFar, CameraDistance.Farther, CameraDistance.Far, CameraDistance.Normal, CameraDistance.Near, CameraDistance.Nearer, CameraDistance.TooClose })
        {
            var scene = Sta(() => SceneGenerator.Generate(new() { Width = 800, Height = 600, Distance = distance }));
            var field = scene.Fields[0];
            double width = field.Polygon[1].X - field.Polygon[0].X;
            Assert.True(width > previous); previous = width;
        }
    }

    [Theory]
    [InlineData(Severity.None)]
    [InlineData(Severity.Light)]
    [InlineData(Severity.Medium)]
    [InlineData(Severity.Strong)]
    public void OccluderIsNotReleasedAsAColorField(Severity severity)
    {
        var scene = Sta(() => SceneGenerator.Generate(new() { Occlusion = severity }));
        var bitmap = new System.Windows.Media.Imaging.FormatConvertedBitmap(scene.Image, PixelFormats.Rgb24, null, 0);
        var pixels = new byte[scene.Options.Width * scene.Options.Height * 3];
        bitmap.CopyPixels(pixels, scene.Options.Width * 3, 0);
        var analysis = new ImageAnalyzer().Analyze(new(scene.Options.Width, scene.Options.Height, scene.Options.Width * 3, pixels), new());
        var occluderLab = ColorMath.ToLab(new RgbColor(164, 116, 82));
        Assert.All(analysis.Fields.Where(f => f.MeasurementAllowed),
            f => Assert.True(ColorMath.DeltaE00(f.Measurement.Lab!.Value, occluderLab) > 1));
        if (severity == Severity.None) Assert.Equal(7, analysis.Fields.Count(f => f.MeasurementAllowed));
        if (severity is Severity.Light or Severity.Medium)
        {
            Assert.Equal(severity == Severity.Medium ? 4 : 5, analysis.Fields.Count(f => f.MeasurementAllowed));
            Assert.Contains(analysis.Fields, f => !f.MeasurementAllowed && f.Hint!.Contains("Feldgrenzen"));
        }
    }

    [Fact]
    public void ExportIncludesTotalsAndVersionButLimitsExamples()
    {
        var rows = Enumerable.Range(0, 100).Select(i => new TestRunRow("run", "request", null, "Szene", "Verdeckung",
            ReviewVerdict.FalscherMesswert, "Befund", "Hinweis", 7, 6, i, 0, "folder")
            { CaptureId = "capture-" + i, Seed = i.ToString(), AnalyzerVersion = "test-v1", CaseName = "Fall" }).ToArray();
        string report = TestReviewExport.Create(rows, 1, true);
        Assert.Contains("Bilder insgesamt: 100", report);
        Assert.Contains("99.00", report);
        Assert.Contains("capture-99", report);
        Assert.DoesNotContain("capture-0,", report);
        Assert.Contains("Analyse test-v1", report);
        Assert.Contains("ALLE", report);
    }
    [Fact]
    public void ShippedPlanContains113ImagesAndAllSixDistanceSteps()
    {
        var plan = TestPlan.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "testplans", "bildqualitaet-und-abstand.json")));
        var cases = plan.Expand();
        Assert.Equal(113, cases.Sum(c => c.Case.Count));
        Assert.Equal(35, cases.Count);
        Assert.Equal(7, cases.Select(c => c.Options.Distance).Distinct().Count());
    }
}
