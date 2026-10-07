using System.Security.Cryptography;
using System.Text.Json;
using Iro.Analysis;
using Iro.Core.Analysis;
using Xunit;

namespace Iro.Analysis.Tests;

public sealed class LegacyExpectationTests
{
    private static readonly string Root = FindRoot();
    private const string GeometryRequest = "iro-request-eb213e24763d467eb259b89ee279d65b";
    private const string ShadowRequest = "iro-request-d7b212cf58df44478f2ae9ca859d8847";

    [Theory]
    [InlineData(GeometryRequest, 10, false)]
    [InlineData(GeometryRequest, 10, true)]
    [InlineData(ShadowRequest, 8, false)]
    [InlineData(ShadowRequest, 8, true)]
    public async Task IdentifiedPackagesAreNotEvaluatedAndKeepAnalysisAndDiagnostics(string requestId, int count, bool partial)
    {
        string request = Path.Combine(Root, "tests", "requests", requestId);
        var before = Directory.GetFiles(request, "*", SearchOption.AllDirectories)
            .ToDictionary(p => p, p => SHA256.HashData(File.ReadAllBytes(p)));
        using var output = new TemporaryFolder();
        var api = new FixedAnalysisApi(partial);
        string input = WrapArchivedInputs(request, output.Path, requestId);
        var saved = await new AnalysisRunner(api).RunAsync(input, Path.Combine(output.Path, "runs"), new());
        Assert.Equal(count, api.Calls);
        Assert.Equal(count, saved.Report.Captures.Count);
        foreach (var capture in saved.Report.Captures)
        {
            Assert.Null(capture.Error);
            Assert.Same(api.Result, capture.Analysis);
            Assert.Equal(partial ? AnalysisStatus.PartiallyMeasured : AnalysisStatus.InvalidFields, capture.Analysis!.Status);
            Assert.Equal("verified", capture.Expectation!.Verification);
            Assert.False(string.IsNullOrWhiteSpace(capture.Expectation.Basis));
            Assert.Equal(ExpectationStatus.NotEvaluated, capture.Evaluation!.Status);
            Assert.Contains("Reset", capture.Evaluation.Details);
            Assert.NotEmpty(capture.Comparisons);
            Assert.Contains(capture.Comparisons, c => c.NominalDeltaE00.HasValue);
            Assert.False(string.IsNullOrWhiteSpace(capture.ImageSha256));
            if (partial)
            {
                Assert.True(capture.Analysis.Fields[0].MeasurementAllowed);
                Assert.Equal(1d, capture.Analysis.Fields[0].DeltaE00);
                Assert.True(capture.Analysis.Fields[0].IsNearest);
            }
        }
        var stored = JsonSerializer.Deserialize<AnalysisRun>(File.ReadAllText(saved.ResultFile), AnalysisRunner.JsonOptions)!;
        Assert.Equal(JsonSerializer.Serialize(saved.Report, AnalysisRunner.JsonOptions),
            JsonSerializer.Serialize(stored, AnalysisRunner.JsonOptions));
        foreach (var (path, hash) in before) Assert.Equal(hash, SHA256.HashData(File.ReadAllBytes(path)));
    }

    [Fact]
    public async Task NewFormatAndNewBasisDoNotReauthorizeIdentifiedCaptures()
    {
        using var folder = new TemporaryFolder();
        string input = WrapArchivedInputs(Path.Combine(Root, "tests", "requests", ShadowRequest), folder.Path, ShadowRequest);
        foreach (string path in Directory.GetFiles(input, "irogen-*.json"))
        {
            var json = System.Text.Json.Nodes.JsonNode.Parse(File.ReadAllText(path))!;
            json["conditions"]!["generator"]!["parameters"]!["behaviorExpectation"] = JsonSerializer.SerializeToNode(
                new BehaviorExpectation { FormatVersion = 2, Verification = "verified", Basis = "New wording does not authorize an old capture",
                    MeasurementAllowed = true, ReleasedFieldCount = 1,
                    Fields = [new() { FieldId = "field-1", MeasurementAllowed = true }] }, AnalysisRunner.JsonOptions);
            File.WriteAllText(path, json.ToJsonString());
        }
        var saved = await new AnalysisRunner(new FixedAnalysisApi(true)).RunAsync(input, Path.Combine(folder.Path, "runs"), new());
        Assert.Equal(8, saved.Report.Captures.Count);
        Assert.All(saved.Report.Captures, capture =>
        {
            Assert.Null(capture.Error);
            Assert.Equal(2, capture.Expectation!.FormatVersion);
            Assert.Equal(ExpectationStatus.NotEvaluated, capture.Evaluation!.Status);
        });
    }
    // Current authorization: this task explicitly requires the generic evaluator to remain usable.
    // These artificial runner expectations test that mechanism only, not any old image or quality rule.
    [Theory]
    [InlineData(1, ExpectationStatus.Passed)]
    [InlineData(2, ExpectationStatus.Failed)]
    public async Task CurrentExplicitTestExpectationIsStillEvaluated(int wanted, ExpectationStatus status)
    {
        using var folder = new TemporaryFolder();
        var expected = new BehaviorExpectation { Verification = "verified", Basis = "Current user-authorized evaluator regression; synthetic result only.",
            MeasurementAllowed = true, ReleasedFieldCount = wanted };
        File.WriteAllBytes(Path.Combine(folder.Path, "image.png"), [1, 2, 3]);
        Write("description.json", new { formatVersion = 1, captureId = "current-evaluator-fixture", colorSpace = "sRGB", imageFile = "image.png", width = 1600, height = 1200,
            conditions = new { generator = new { parameters = new { behaviorExpectation = expected } } } });
        Write("series.json", new { formatVersion = 1, kind = "iro-image-series", batchId = "current-batch", actualCount = 1, requestedCount = 1,
            captures = new[] { new { captureId = "current-evaluator-fixture", descriptionFile = "description.json", imageFile = "image.png" } } });
        Write("request.json", new { formatVersion = 2, kind = "iro-test-request", requestId = "current-request", batchId = "current-batch", imageCount = 1, seriesFile = "series.json" });
        var saved = await new AnalysisRunner(new FixedAnalysisApi(true)).RunAsync(folder.Path, Path.Combine(folder.Path, "runs"), new());
        var capture = Assert.Single(saved.Report.Captures);
        Assert.Null(capture.Error);
        Assert.Equal(status, capture.Evaluation!.Status);
        void Write(string name, object value) => File.WriteAllText(Path.Combine(folder.Path, name), JsonSerializer.Serialize(value, AnalysisRunner.JsonOptions));
    }

    // The surviving archive contains image/description pairs, but no request/series envelopes.
    // Add only transport envelopes in a temporary copy; preserve all source bytes and expectations.
    private static string WrapArchivedInputs(string request, string output, string requestId)
    {
        string input = Path.Combine(output, "input");
        Directory.CreateDirectory(input);
        var captures = new List<object>();
        foreach (string description in Directory.GetFiles(request, "*.json", SearchOption.AllDirectories))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(description));
            var metadata = document.RootElement;
            if (!metadata.TryGetProperty("captureId", out var id)) continue;
            string image = metadata.GetProperty("imageFile").GetString()!;
            File.Copy(description, Path.Combine(input, Path.GetFileName(description)));
            File.Copy(Path.Combine(Path.GetDirectoryName(description)!, image), Path.Combine(input, image));
            captures.Add(new { captureId = id.GetString(), descriptionFile = Path.GetFileName(description), imageFile = image });
        }
        File.WriteAllText(Path.Combine(input, "series.json"), JsonSerializer.Serialize(new {
            formatVersion = 1, kind = "iro-image-series", batchId = "archive-fixture", actualCount = captures.Count,
            requestedCount = captures.Count, captures }, AnalysisRunner.JsonOptions));
        File.WriteAllText(Path.Combine(input, "request.json"), JsonSerializer.Serialize(new {
            formatVersion = 2, kind = "iro-test-request", requestId, batchId = "archive-fixture", imageCount = captures.Count,
            seriesFile = "series.json" }, AnalysisRunner.JsonOptions));
        return input;
    }
    private sealed class FixedAnalysisApi(bool partial) : IPngAnalysisApi
    {
        public int Calls { get; private set; }
        public ImageAnalysis Result { get; } = MakeResult(partial);
        public Task<ImageAnalysis> AnalyzeAsync(ReadOnlyMemory<byte> png, AnalysisOptions options, CancellationToken token = default)
        {
            Assert.False(png.IsEmpty);
            Calls++;
            return Task.FromResult(Result);
        }
        private static ImageAnalysis MakeResult(bool partial)
        {
            var region = new RegionMeasurement(new(0, 0, 40, 40), true, null, new(50, 0, 0), 1600, 0, 0, 0);
            FieldAnalysis[] fields = partial ? [new("detected-1", region.Bounds, region.Bounds, region, region, 1, true, null, true)] : [];
            return new("evaluator-fixture", new(), 1600, 1200, partial ? AnalysisStatus.PartiallyMeasured : AnalysisStatus.InvalidFields,
                "Controlled result, no claim about the archived images", fields, ["diagnostic-preserved"]) { HintCode = AnalysisHintCode.Other };
        }
    }
    private static string FindRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "MASTERPLAN.md"))) return dir.FullName;
        throw new InvalidOperationException("Run tests from the Iro checkout containing the identified input packages.");
    }
    private sealed class TemporaryFolder : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "iro-expectation-tests-" + Guid.NewGuid().ToString("N"));
        public TemporaryFolder() => Directory.CreateDirectory(Path);
        public void Dispose() => Directory.Delete(Path, true);
    }
}