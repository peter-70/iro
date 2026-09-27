using IroGen;
using Iro.Analysis;
using System.IO;

namespace IroGenTests;

public class IlluminationPlanTests
{
    [Fact]
    public async Task StrongShadowCrossingsAreRejectedAndUniformControlsRetainAllFields()
    {
        string project = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(project, "iro.slnx")))
            project = Directory.GetParent(project)?.FullName ?? throw new Exception("Projekt fehlt.");
        var plan = TestPlan.Parse(File.ReadAllText(Path.Combine(project,
            "iro-gen", "testplans", "beleuchtung-schutzpruefung.json")));
        Assert.Equal(8, plan.Expand().Sum(c => c.Case.Count));
        Assert.Equal(8, plan.Cases.Count(c => c.Expected != null));
        string root = Path.Combine(Path.GetTempPath(), "iro-lighting-plan-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            using var batch = Sta(() => plan.Generate(root));
            File.WriteAllText(Path.Combine(root, "iro.slnx"), "<Solution/>");
            File.WriteAllText(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolierter Test");
            var request = IroTestHandoff.Submit(batch, root);
            var saved = await new AnalysisRunner().RunAsync(request.Folder, Path.Combine(root, "tests", "runs"), new());
            Assert.Equal(8, saved.Report.ProcessedCount);
            Assert.All(saved.Report.Captures, capture => Assert.Null(capture.Error));
            Assert.All(saved.Report.Captures.Where(c => c.Expectation != null),
                capture => Assert.Equal(ExpectationStatus.Passed, capture.Evaluation!.Status));
            Assert.All(saved.Report.Captures.Where(c => c.Expectation == null),
                capture => Assert.Equal(ExpectationStatus.NotEvaluated, capture.Evaluation!.Status));
            foreach (var capture in saved.Report.Captures.Where(c => c.Expectation!.MeasurementAllowed == false))
                Assert.All(capture.Analysis!.Fields, f => {
                    Assert.False(f.MeasurementAllowed);
                    Assert.Null(f.DeltaE00);
                    Assert.False(f.IsNearest);
                });
            var review = TestRunReview.Load(root, 1);
            Assert.Equal(8, review.Count(r => r.CheckStatus == ExpectationStatus.Passed));
            Assert.DoesNotContain(review, r => r.CheckStatus != ExpectationStatus.Passed);
            string evidence = Path.Combine(project, "tests", "adjustments", "beleuchtung-generator-20260923");
            Directory.CreateDirectory(evidence);
            File.WriteAllText(Path.Combine(evidence, "bericht.md"), TestReviewExport.Create(review, 1, false));
        }
        finally
        {
            // Only this test's freshly created, uniquely named temporary directory.
            Directory.Delete(root, true);
        }
    }

    private static T Sta<T>(Func<T> action)
    {
        T result = default!; Exception? error = null;
        var thread = new Thread(() => { try { result = action(); } catch (Exception e) { error = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
        return result;
    }
}