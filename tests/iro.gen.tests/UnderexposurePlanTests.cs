using IroGen;
using Iro.Analysis;
using System.IO;

namespace IroGenTests;

public class UnderexposurePlanTests
{
    [Fact]
    public async Task DarkControlsAndWholeImageExposureRetainVerifiedRelativeComparison()
    {
        string project = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(project, "iro.slnx")))
            project = Directory.GetParent(project)?.FullName ?? throw new Exception("Projekt fehlt.");
        var plan = TestPlan.Parse(File.ReadAllText(Path.Combine(project,
            "iro-gen", "testplans", "unterbelichtung-und-dunkle-originalfarben.json")));
        Assert.Equal(16, plan.Expand().Sum(c => c.Case.Count));
        Assert.Equal(16, plan.Cases.Count(c => c.Expected != null));
        string root = Path.Combine(Path.GetTempPath(), "iro-darkness-plan-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            using var batch = Sta(() => plan.Generate(root));
            File.WriteAllText(Path.Combine(root, "iro.slnx"), "<Solution/>");
            File.WriteAllText(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolierter Test");
            var request = IroTestHandoff.Submit(batch, root);
            var saved = await new AnalysisRunner().RunAsync(request.Folder, Path.Combine(root, "tests", "runs"), new());
            Assert.Equal(16, saved.Report.ProcessedCount);
            Assert.All(saved.Report.Captures, capture => Assert.Null(capture.Error));
            Assert.All(saved.Report.Captures, capture =>
            {
                Assert.Equal(ExpectationStatus.Passed, capture.Evaluation!.Status);
                Assert.Equal(0, capture.UnexpectedDetectedFields);
                Assert.Equal(3, capture.Comparisons.Count);
                Assert.All(capture.Comparisons, comparison =>
                {
                    Assert.Equal("measured", comparison.Status);
                    Assert.NotNull(comparison.DetectedFieldId);
                    Assert.True(comparison.IntersectionOverUnion >= .9);
                });
                string expectedNearestId = capture.Expectation?.Ranking?.Groups[0][0] ?? "field-1";
                var expectedNearest = capture.Comparisons.Single(c => c.ExpectedFieldId == expectedNearestId).DetectedFieldId;
                var nearest = Assert.Single(capture.Analysis!.Fields, field => field.IsNearest);
                Assert.Equal(expectedNearest, nearest.FieldId);
            });
            var review = TestRunReview.Load(root, 1);
            Assert.Equal(16, review.Count(r => r.CheckStatus == ExpectationStatus.Passed));
            Assert.DoesNotContain(review, r => r.CheckStatus != ExpectationStatus.Passed);
            string evidence = Path.Combine(project, "tests", "adjustments", "unterbelichtung-20260923");
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