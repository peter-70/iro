using System.Security.Cryptography;
using System.Text.Json;
using IroGen;
using Xunit;

namespace Iro.Analysis.Tests;

public sealed class HandoffTests
{
    private static string RepositoryRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "iro-gen", "IroGen.csproj"))) return dir.FullName;
        throw new InvalidOperationException("Repository not found.");
    }

    [Fact]
    public void ActualCallerDirectoriesFindCurrentRepositoryWithoutRetiredPlan()
    {
        string root = RepositoryRoot();
        Assert.False(File.Exists(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md")));
        string output = Path.GetDirectoryName(typeof(IroTestHandoff).Assembly.Location)!;
        foreach (string start in new[] { root, Path.Combine(root, "iro-gen"), Path.Combine(root, "iro-gen", "bin", "Debug", "net10.0-windows"), output, AppContext.BaseDirectory })
            Assert.Equal(root, IroTestHandoff.FindProjectRoot(start));
    }

    [Fact]
    public void RelocatedStructureWorksWithoutAnyPlanFile()
    {
        using var temp = new TemporaryProject();
        temp.CreateStructure();
        string output = Path.Combine(temp.Root, "iro-gen", "bin", "Debug", "net10.0-windows");
        Directory.CreateDirectory(output);
        Assert.Equal(temp.Root, IroTestHandoff.FindProjectRoot(output));
        Assert.Equal(temp.Root, IroTestHandoff.FindProjectRoot(temp.Root));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MissingStructureCannotBeMistakenForProjectRoot(bool obsoleteMarker)
    {
        using var temp = new TemporaryProject();
        Assert.Null(IroTestHandoff.FindProjectRoot(temp.Root));
        File.WriteAllText(Path.Combine(temp.Root, "iro.slnx"), "<Solution />");
        if (obsoleteMarker) File.WriteAllText(Path.Combine(temp.Root, "IRO-KONSOLIDIERTER-PLAN.md"), "old");
        Assert.Null(IroTestHandoff.FindProjectRoot(temp.Root));
        var batch = new GeneratedBatch(temp.Root, "unused", 0, false);
        Assert.Throws<ArgumentException>(() => IroTestHandoff.Submit(batch, temp.Root));
        Assert.False(Directory.Exists(Path.Combine(temp.Root, "tests")));
    }

    [Fact]
    public async Task GeneratedBatchTraversesSubmitAndRealAnalyzerWithoutMetadataInfluence()
    {
        using var temp = new TemporaryProject();
        temp.CreateStructure();
        // Same public pipeline as MainWindow.SendTestsAsync, with an isolated relocated root.
        using var batch = BatchGenerator.Generate(new GeneratorOptions
        {
            Width = 640, Height = 480, FieldCount = 3, MatchingField = 2, Labels = false
        }, 1, false, Path.Combine(temp.Root, "cache"));
        var handoff = IroTestHandoff.Submit(batch, temp.Root);
        var run = await new AnalysisRunner().RunAsync(handoff.Folder, Path.Combine(temp.Root, "tests", "runs"), new());
        Assert.Equal("completed", run.Report.Status);
        Assert.Equal(handoff.RequestId, run.Report.RequestId);
        Assert.Equal(1, run.Report.ProcessedCount);
        Assert.True(File.Exists(Path.Combine(handoff.Folder, "request.json")));
        Assert.True(File.Exists(run.ResultFile));
        var capture = Assert.Single(run.Report.Captures);
        Assert.Null(capture.Error);
        Assert.NotNull(capture.Analysis);
        Assert.NotEmpty(capture.Comparisons);
        byte[] png = File.ReadAllBytes(batch.Items[0].ImagePath);
        Assert.Equal(Convert.ToHexString(SHA256.HashData(png)).ToLowerInvariant(), capture.ImageSha256.ToLowerInvariant());
        var direct = await new PngAnalysisApi().AnalyzeAsync(png, new());
        Assert.Equal(JsonSerializer.Serialize(direct), JsonSerializer.Serialize(capture.Analysis));
        using var result = JsonDocument.Parse(File.ReadAllText(run.ResultFile));
        Assert.Equal(JsonValueKind.Object, result.RootElement.ValueKind);
    }

    private sealed class TemporaryProject : IDisposable
    {
        public string Root { get; } = Path.Combine(Path.GetTempPath(), "iro-handoff-test-" + Guid.NewGuid().ToString("N"));
        public TemporaryProject() => Directory.CreateDirectory(Root);
        public void CreateStructure()
        {
            string repository = RepositoryRoot();
            foreach (string file in new[] { "iro.slnx", "iro-gen/IroGen.csproj", "src/iro.analysis/Iro.Analysis.csproj" })
            {
                string destination = Path.Combine(Root, file);
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                File.Copy(Path.Combine(repository, file), destination);
            }
        }
        public void Dispose()
        {
            string full = Path.GetFullPath(Root);
            string parent = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetTempPath()));
            if (Path.GetDirectoryName(full) != parent || !Path.GetFileName(full).StartsWith("iro-handoff-test-"))
                throw new InvalidOperationException("Unexpected temporary path.");
            Directory.Delete(full, true);
        }
    }
}
