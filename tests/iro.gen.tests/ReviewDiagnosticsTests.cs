using System.IO;
using System.Text.Json;
using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;

namespace IroGenTests;

public class ReviewDiagnosticsTests
{
    [Theory]
    [InlineData(0, ReviewVerdict.NominalUnauffaellig)]
    [InlineData(4, ReviewVerdict.NominalAbweichend)]
    [InlineData(-1, ReviewVerdict.OhneSollvergleich)]
    public void NominalComparisonsDoNotClaimColorAccuracyOrAcceptance(double deviation, ReviewVerdict expected)
    {
        string root = Path.Combine(Path.GetTempPath(), "iro-review-rules-" + Guid.NewGuid().ToString("N"));
        string folder = Path.Combine(root, "tests", "runs", "iro-run-review");
        Directory.CreateDirectory(folder);
        try
        {
            var region = new RegionMeasurement(new(20, 20, 80, 80), true, null, new(50, 10, 5), 6400, 0, 0, 0);
            var field = new FieldAnalysis("field", region.Bounds, region.Bounds, region, region, 4, true, null, true);
            var analysis = new ImageAnalysis(ImageAnalyzer.Version, new(), 200, 200, AnalysisStatus.Measured, "Gemessen", [field], []);
            var comparisons = deviation < 0 ? Array.Empty<FieldComparison>() : new[]
                { new FieldComparison("expected", "field", 1, 4 - deviation, 4, deviation, "measured") };
            var run = new AnalysisRun(1, "iro-analysis-run", "iro-run-review", "request", ImageAnalyzer.Version,
                DateTime.UtcNow.ToString("O"), new(), "completed", 1, 1, 1, 1, "Nominale Diagnose",
                [new CaptureAnalysis("capture", "hash", null, analysis, comparisons, 0)]);
            File.WriteAllText(Path.Combine(folder, "results.json"), JsonSerializer.Serialize(run, AnalysisRunner.JsonOptions));
            var row = Assert.Single(TestRunReview.Load(root, 1));
            Assert.Equal(expected, row.Verdict);
            string report = TestReviewExport.Create([row], 1, false);
            Assert.DoesNotContain("falscher Messwert", report);
            Assert.DoesNotContain("Ziel erreicht", report);
            Assert.DoesNotContain("vollständig/genau", report);
            Assert.Contains("keine Abnahme", report);
            if (deviation < 0)
            {
                Assert.Contains("1 Felder freigegeben", row.Finding);
                Assert.DoesNotContain("Kein Farbfeld", row.Finding);
            }
        }
        finally { Directory.Delete(root, true); }
    }
}