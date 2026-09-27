using IroGen;
using Iro.Analysis;
using System.IO;

namespace IroGenTests;

public class TwoImageCheckpointTests
{
    [Fact]
    public async Task CleanAndKnownMildNoiseRetainFieldsValuesAndRanking()
    {
        string project = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(project, "iro.slnx")))
            project = Directory.GetParent(project)?.FullName ?? throw new Exception("Projekt fehlt.");
        var plan = TestPlan.Parse(File.ReadAllText(Path.Combine(project,
            "iro-gen", "testplans", "zwischenkontrolle-zwei-bilder.json")));
        Assert.Equal(2, plan.Expand().Sum(c => c.Case.Count));
        Assert.Equal(2, plan.Cases.Count(c => c.Expected != null));
        Assert.Equal(Severity.None, plan.Expand()[0].Options.Noise);
        Assert.Equal(Severity.Light, plan.Expand()[1].Options.Noise);
        string root = Path.Combine(Path.GetTempPath(), "iro-checkpoint-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            using var batch = Sta(() => plan.Generate(root));
            File.WriteAllText(Path.Combine(root, "iro.slnx"), "<Solution/>");
            File.WriteAllText(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolierter Test");
            var request = IroTestHandoff.Submit(batch, root);
            var saved = await new AnalysisRunner().RunAsync(request.Folder, Path.Combine(root, "tests", "runs"), new());
            Assert.Equal(2, saved.Report.ProcessedCount);
            Assert.All(saved.Report.Captures, capture => Assert.Null(capture.Error));
            Assert.All(saved.Report.Captures.Where(c => c.Expectation != null),
                capture => Assert.Equal(ExpectationStatus.Passed, capture.Evaluation!.Status));

            foreach (var capture in saved.Report.Captures)
            {
                Assert.Equal(0, capture.UnexpectedDetectedFields);
                Assert.Equal(3, capture.Comparisons.Count);
                Assert.All(capture.Comparisons, comparison =>
                {
                    Assert.NotNull(comparison.DetectedFieldId);
                    Assert.NotNull(comparison.NominalDeltaE00);
                    Assert.True(comparison.IntersectionOverUnion >= .9);
                    // Fixed regression envelope, not a device accuracy promise:
                    // clean homogeneous pixels agree to 0.01; known bounded noise to 0.15 delta E.
                    double tolerance = capture == saved.Report.Captures[0] ? .01 : .15;
                    Assert.True(comparison.DifferenceFromNominal.HasValue);
                    Assert.InRange(Math.Abs(comparison.DifferenceFromNominal.Value), 0, tolerance);
                });
                var expectedNearest = capture.Comparisons.MinBy(c => c.NominalDeltaE00)!.DetectedFieldId;
                var nearest = Assert.Single(capture.Analysis!.Fields, f => f.IsNearest);
                Assert.Equal(expectedNearest, nearest.FieldId);
            }
            var review = TestRunReview.Load(root, 1);
            Assert.Equal(2, review.Count(r => r.CheckStatus == ExpectationStatus.Passed));
            Assert.DoesNotContain(review, r => r.CheckStatus != ExpectationStatus.Passed);
            string evidence = Path.Combine(project, "tests", "adjustments", "zwischenkontrolle-20260923");
            Directory.CreateDirectory(evidence);
            string runFolder = Path.Combine(evidence, saved.Report.RunId);
            Directory.CreateDirectory(runFolder);
            File.WriteAllText(Path.Combine(runFolder, "bericht.md"), TestReviewExport.Create(review, 1, false)
                + Environment.NewLine + "Zusätzliche automatisierte Regression: drei zugeordnete Felder, keine Fremdflächen, Überdeckung mindestens 90 %, gleiche nächste Farbe; nominale Abweichung sauber höchstens 0,01 / leichtes Rauschen höchstens 0,15 ΔE00. Keine Gerätegrenze.");
            File.Copy(Path.Combine(project, "iro-gen", "testplans", "zwischenkontrolle-zwei-bilder.json"),
                Path.Combine(runFolder, "testplan.json"));
            var hashes = new System.Text.StringBuilder();
            hashes.AppendLine("# Prüfbasis des Kontrolllaufs");
            hashes.AppendLine();
            foreach (string source in new[] { "src/iro.core/Analysis/ImageAnalyzer.cs", "tests/iro.gen.tests/TwoImageCheckpointTests.cs" })
            {
                string file = Path.Combine(project, source);
                if (File.Exists(file))
                    hashes.AppendLine(source + ": " + Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(file))));
            }
            File.WriteAllText(Path.Combine(runFolder, "pruefbasis.md"), hashes.ToString());
            foreach (string png in Directory.EnumerateFiles(request.Folder, "*.png", SearchOption.AllDirectories))
                File.Copy(png, Path.Combine(runFolder, Path.GetFileName(Path.GetDirectoryName(png)) + "-" + Path.GetFileName(png)));
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