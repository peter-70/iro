using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace IroGen;

public sealed record TestPlanCase(string Name, int Count, bool CoverRange, JsonObject Options)
{ public Iro.Analysis.BehaviorExpectation? Expected { get; init; } }
public sealed record TestPlan(int FormatVersion, string Kind, string Name, GeneratorOptions Defaults, List<TestPlanCase> Cases)
{
    public static TestPlan Parse(string text)
    {
        var plan = JsonSerializer.Deserialize<TestPlan>(text, SceneExport.JsonOptions)
            ?? throw new ArgumentException("Leerer Testplan.");
        _ = plan.Expand();
        return plan;
    }

    public IReadOnlyList<(TestPlanCase Case, GeneratorOptions Options)> Expand()
    {
        if (FormatVersion is not (1 or 2) || Kind != "irogen-test-plan" || string.IsNullOrWhiteSpace(Name)
            || Defaults == null || Cases == null || Cases.Count == 0)
            throw new ArgumentException("Testplan: Version, Art, Name, Grundoptionen oder Testfälle fehlen.");
        Defaults.Validate();
        long count = 0;
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<(TestPlanCase, GeneratorOptions)>();
        foreach (var entry in Cases)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.Name) || !names.Add(entry.Name)
                || entry.Count < 1 || entry.Options == null)
                throw new ArgumentException("Jeder Testfall benötigt einen eindeutigen Namen, Optionen und eine positive Bildanzahl.");
            if (FormatVersion == 1 && entry.Expected != null)
                throw new ArgumentException("Verhaltenserwartungen benötigen Testplanformat 2.");
            if (entry.Options.ContainsKey("testExpectation"))
                throw new ArgumentException("Erwartungen gehören in expected, nicht in Generatoroptionen.");
            entry.Expected?.Validate();
            count += entry.Count;
            if (count > 10000) throw new ArgumentException("Ein Testplan darf insgesamt höchstens 10000 Bilder enthalten.");
            var merged = JsonSerializer.SerializeToNode(Defaults, SceneExport.JsonOptions)!.AsObject();
            foreach (var option in entry.Options) merged[option.Key] = option.Value?.DeepClone();
            var options = merged.Deserialize<GeneratorOptions>(SceneExport.JsonOptions)! with { TestCaseName = entry.Name, TestExpectation = entry.Expected };
            options.Validate();
            result.Add((entry, options));
        }
        return result;
    }

    // Execute on the same STA worker as ordinary series generation. No shell commands in plans.
    public GeneratedBatch Generate(string cacheParent, IProgress<BatchProgress>? progress = null, CancellationToken token = default)
    {
        var cases = Expand();
        int total = cases.Sum(c => c.Case.Count);
        string id = "irogen-series-" + Guid.NewGuid().ToString("N");
        var result = new GeneratedBatch(Path.Combine(cacheParent, id), id, total, false);
        Directory.CreateDirectory(result.Root);
        try
        {
            foreach (var (entry, options) in cases)
            {
                token.ThrowIfCancellationRequested();
                int completed = result.Items.Count;
                using var part = BatchGenerator.Generate(options, entry.Count, entry.CoverRange, result.Root,
                    new ForwardProgress(p => progress?.Report(new(completed + p.Completed, total))), token);
                foreach (var item in part.Items)
                {
                    token.ThrowIfCancellationRequested();
                    string target = Path.Combine(result.Root, item.CaptureId);
                    Directory.Move(item.Folder, target);
                    result.Items.Add(item with { Number = result.Items.Count + 1, Folder = target });
                }
            }
            return result;
        }
        catch { result.Dispose(); throw; }
    }

    private sealed class ForwardProgress(Action<BatchProgress> report) : IProgress<BatchProgress>
    {
        public void Report(BatchProgress value) => report(value);
    }
}
