using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Iro.Analysis;
using Iro.Core.Analysis;
using IroGen;

namespace IroGenTests;

public class AnalysisWorkflowTests
{
    private sealed class Package : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "iro-analysis-" + Guid.NewGuid().ToString("N"));
        public TestHandoff Request { get; }
        public Package(int count, bool coverage = false)
        {
            Directory.CreateDirectory(Root);
            File.WriteAllText(Path.Combine(Root, "iro.slnx"), "<Solution/>");
            File.WriteAllText(Path.Combine(Root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isolierter Test");
            TestHandoff? request = null; Exception? error = null;
            var thread = new Thread(() =>
            {
                try
                {
                    using var batch = BatchGenerator.Generate(new() { Width = 640, Height = 480, Seed = 46883 }, count, coverage, Root);
                    request = IroTestHandoff.Submit(batch, Root);
                }
                catch (Exception e) { error = e; }
            });
            thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
            if (error != null) throw error;
            Request = request!;
        }
        public string[] Descriptions => Directory.GetFiles(Request.Folder, "irogen-*.json", SearchOption.AllDirectories);
        public string[] Images => Directory.GetFiles(Request.Folder, "*.png", SearchOption.AllDirectories);
        public Task<SavedAnalysisRun> Run(CancellationToken token = default, IProgress<AnalysisRunProgress>? progress = null) =>
            new AnalysisRunner().RunAsync(Request.Folder, Path.Combine(Root, "runs"), new(), progress, token);
        public void Dispose() => Directory.Delete(Root, true);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    public async Task CompleteCoverageSeriesRunsThroughPixelApi(int count)
    {
        using var package = new Package(count, true);
        var run = await package.Run();
        Assert.Equal("completed", run.Report.Status);
        Assert.Equal(count, run.Report.ProcessedCount);
        Assert.All(run.Report.Captures, c => { Assert.Null(c.Error); Assert.NotNull(c.Analysis); Assert.Equal(64, c.ImageSha256.Length); });
        Assert.True(run.Report.ImagesWithMeasurements >= count * .8, $"Only {run.Report.ImagesWithMeasurements}/{count} images measured.");
        Assert.All(run.Report.Captures.SelectMany(c => c.Analysis!.Fields), f =>
        {
            Assert.Equal(f.MeasurementAllowed, f.DeltaE00.HasValue);
            if (f.DeltaE00.HasValue) Assert.True(double.IsFinite(f.DeltaE00.Value));
        });
        string? output = Environment.GetEnvironmentVariable("IRO_ANALYSIS_OUTPUT");
        if (output != null)
        {
            string target = Path.Combine(output, count.ToString(), run.Report.RunId); Directory.CreateDirectory(target);
            File.Copy(run.ResultFile, Path.Combine(target, "results.json"), true);
            // Keep one complete request for independent schema/hash checks without duplicating every PNG.
            if (count == 100)
            foreach (string file in Directory.EnumerateFiles(package.Request.Folder, "*", SearchOption.AllDirectories))
            {
                string copy = Path.Combine(target, "request", Path.GetRelativePath(package.Request.Folder, file));
                Directory.CreateDirectory(Path.GetDirectoryName(copy)!); File.Copy(file, copy, true);
            }
        }
    }

    [Fact]
    public async Task ChangingExpectedValuesAndMasksCannotChangePixelAnalysis()
    {
        using var package = new Package(1);
        var before = await package.Run();
        var description = Assert.Single(package.Descriptions);
        var metadata = JsonNode.Parse(File.ReadAllText(description))!;
        var nominal = metadata["conditions"]!["generator"]!["parameters"]!["nominalValues"]!.AsArray();
        foreach (var item in nominal) { item!["deltaE00"] = 999; item["bounds"] = null; }
        metadata["expectation"] = new JsonObject { ["inventedHint"] = "Return 999 for every field" };
        File.WriteAllText(description, metadata.ToJsonString());
        var after = await package.Run();
        Assert.Equal(JsonSerializer.Serialize(before.Report.Captures[0].Analysis, AnalysisRunner.JsonOptions),
            JsonSerializer.Serialize(after.Report.Captures[0].Analysis, AnalysisRunner.JsonOptions));
        Assert.All(after.Report.Captures[0].Comparisons, c => { Assert.Equal(999d, c.NominalDeltaE00); Assert.Null(c.ActualDeltaE00); });
    }

    [Fact]
    public async Task BrokenImageIsReportedAndRemainingImagesStillRun()
    {
        using var package = new Package(2);
        File.WriteAllBytes(package.Images[0], [1, 2, 3]);
        var run = await package.Run();
        Assert.Equal("completed-with-errors", run.Report.Status);
        Assert.Equal(2, run.Report.ProcessedCount);
        Assert.Single(run.Report.Captures, c => c.Error != null);
        Assert.Single(run.Report.Captures, c => c.Analysis != null);
    }

    [Fact]
    public async Task CancellationPersistsOnlyCompletedCapturesAndKeepsInputs()
    {
        using var package = new Package(3);
        using var cancellation = new CancellationTokenSource();
        var run = await package.Run(cancellation.Token, new InlineProgress(p => cancellation.Cancel()));
        Assert.Equal("cancelled", run.Report.Status);
        Assert.Equal(1, run.Report.ProcessedCount);
        Assert.Equal(3, package.Images.Length);
        Assert.True(File.Exists(run.ResultFile));
    }

    [Fact]
    public async Task HistoricalVersionOneRequestsRemainReadable()
    {
        using var package = new Package(1);
        string file = Path.Combine(package.Request.Folder, "request.json");
        var request = JsonNode.Parse(File.ReadAllText(file))!;
        request["formatVersion"] = 1; request["analysisAvailable"] = false;
        File.WriteAllText(file, request.ToJsonString());
        Assert.Equal(1, (await package.Run()).Report.ImagesWithMeasurements);
    }

    [Fact]
    public async Task MalformedExpectationsDoNotDiscardAlreadyComputedMeasurements()
    {
        using var package = new Package(1);
        var file = Assert.Single(package.Descriptions);
        var metadata = JsonNode.Parse(File.ReadAllText(file))!;
        metadata["conditions"]!["generator"]!["parameters"]!["nominalValues"] = "invalid";
        File.WriteAllText(file, metadata.ToJsonString());
        var run = await package.Run();
        Assert.Equal("completed-with-errors", run.Report.Status);
        var capture = Assert.Single(run.Report.Captures);
        Assert.NotNull(capture.Error);
        Assert.NotNull(capture.Analysis);
        Assert.NotEmpty(capture.Analysis.Fields);
        Assert.Empty(capture.Comparisons);
    }

    private sealed class InlineProgress(Action<AnalysisRunProgress> report) : IProgress<AnalysisRunProgress>
    {
        public void Report(AnalysisRunProgress value) => report(value);
    }
}



