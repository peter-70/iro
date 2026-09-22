using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;

namespace IroGen;

public sealed record BatchProgress(int Completed, int Total);
public sealed record BatchItem(int Number, string CaptureId, string Folder, GeneratorOptions Options, Rgb Wall, FieldInfo[] Fields)
{
    public string ImagePath => Path.Combine(Folder, CaptureId + ".png");
    public string Description => $"{Options.TestCaseName ?? "Einzelserie"} · Bild {Number:000} · {Options.Colors?.CoverageBand ?? "Feste Wandoption"} · Seed {Options.Seed}";
    public double MinimumDeltaE => Fields.Min(f => f.DeltaE00);
    public double MaximumDeltaE => Fields.Max(f => f.DeltaE00);
    public double? AnchorDeltaE => Options.Colors is { } colors ? Fields[colors.AnchorField - 1].DeltaE00 : null;

    public GeneratedScene Load()
    {
        using var stream = File.OpenRead(ImagePath);
        var image = BitmapDecoder.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad).Frames[0];
        image.Freeze();
        return new(Options, Wall, Fields, image);
    }
}

public sealed class GeneratedBatch(string root, string id, int requestedCount, bool coverRange) : IDisposable
{
    public string Root { get; } = root;
    public string Id { get; } = id;
    public int RequestedCount { get; } = requestedCount;
    public bool CoverRange { get; } = coverRange;
    public List<BatchItem> Items { get; } = [];
    public string Summary => $"{Items.Count} Bilder · ΔE00 {Items.Min(i => i.MinimumDeltaE):F2}–{Items.Max(i => i.MaximumDeltaE):F2} · " +
        (Items.Any(i => i.Options.TestCaseName != null) ? $"{Items.Select(i => i.Options.TestCaseName).Distinct().Count()} Testfälle" : CoverRange ? $"{Items.Select(i => i.Options.Colors!.CoverageBand).Distinct().Count()}/{ColorCoverage.Bands.Length} Abstandsbereiche" : "feste Wandoption");

    public void Dispose()
    {
        // This object owns exactly one explicitly generated cache folder.
        string path = Path.GetFullPath(Root);
        if (Path.GetFileName(path) != Id || !Id.StartsWith("irogen-series-", StringComparison.Ordinal))
            throw new InvalidOperationException("Ungültiger Cachepfad.");
        if (Directory.Exists(path)) Directory.Delete(path, true);
    }
}

public static class BatchGenerator
{
    public static GeneratedBatch Generate(GeneratorOptions options, int count, bool coverRange, string cacheParent,
        IProgress<BatchProgress>? progress = null, CancellationToken token = default)
    {
        options.Validate();
        if (count is < 1 or > 10000) throw new ArgumentException("Anzahl Bilder: Bitte 1 bis 10000 eingeben.");
        string id = "irogen-series-" + Guid.NewGuid().ToString("N");
        var batch = new GeneratedBatch(Path.Combine(cacheParent, id), id, count, coverRange);
        Directory.CreateDirectory(batch.Root);
        try
        {
            var schedule = ColorCoverage.Schedule(count, options.Seed);
            var palettes = new HashSet<string>(StringComparer.Ordinal);
            int seedIndex = 0;
            for (int index = 0; index < count; index++)
            {
                token.ThrowIfCancellationRequested();
                GeneratedScene scene;
                GeneratorOptions itemOptions;
                int attempts = 0;
                do
                {
                    if (++attempts > 1000) throw new InvalidOperationException("Keine weitere eindeutige Farbpalette gefunden.");
                    // An odd step permutes all 32-bit seeds, including retries for quantized duplicates.
                    int seed = unchecked(options.Seed + seedIndex++ * (int)0x9E3779B9u);
                    itemOptions = options with { Seed = seed, Colors = count == 1 ? options.Colors : null };
                    if (coverRange) itemOptions = itemOptions with { Colors = ColorCoverage.Create(itemOptions, schedule[index], token) };
                    scene = SceneGenerator.Generate(itemOptions, token);
                } while (!palettes.Add(string.Join("/", scene.Fields.Select(f => f.Hex))));
                string folder = SceneExport.Save(scene, batch.Root);
                batch.Items.Add(new(index + 1, Path.GetFileName(folder), folder, itemOptions, scene.Wall, scene.Fields.ToArray()));
                progress?.Report(new(index + 1, count));
            }
            return batch;
        }
        catch { batch.Dispose(); throw; }
    }

    public static string SaveAll(GeneratedBatch batch, string parent, CancellationToken token = default)
    {
        string parentPath = Path.GetFullPath(parent);
        string cachePath = Path.GetFullPath(batch.Root);
        if (parentPath.Equals(cachePath, StringComparison.OrdinalIgnoreCase) || parentPath.StartsWith(cachePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Bitte einen Speicherort außerhalb des temporären Seriencaches wählen.");
        string destination = Path.Combine(parentPath, batch.Id + "-" + Guid.NewGuid().ToString("N"));
        string staging = destination + ".incomplete";
        Directory.CreateDirectory(staging);
        try
        {
            foreach (var item in batch.Items)
            {
                token.ThrowIfCancellationRequested();
                string target = Path.Combine(staging, item.CaptureId);
                Directory.CreateDirectory(target);
                File.Copy(item.ImagePath, Path.Combine(target, item.CaptureId + ".png"));
                File.Copy(Path.Combine(item.Folder, item.CaptureId + ".json"), Path.Combine(target, item.CaptureId + ".json"));
            }
            var manifest = new
            {
                formatVersion = 1, kind = "iro-image-series", batchId = batch.Id,
                generatorVersion = SceneGenerator.Version, requestedCount = batch.RequestedCount,
                actualCount = batch.Items.Count, coverRange = batch.CoverRange,
                minimumDeltaE00 = batch.Items.Min(i => i.MinimumDeltaE), maximumDeltaE00 = batch.Items.Max(i => i.MaximumDeltaE),
                coverage = ColorCoverage.Bands.Select(b => new { b.Name, b.Minimum, b.Maximum, count = batch.Items.Count(i => i.Options.Colors?.CoverageBand == b.Name) }),
                captures = batch.Items.Select(i => new
                {
                    i.Number, i.CaptureId, descriptionFile = i.CaptureId + "/" + i.CaptureId + ".json",
                    imageFile = i.CaptureId + "/" + i.CaptureId + ".png", i.AnchorDeltaE
                })
            };
            File.WriteAllText(Path.Combine(staging, "series.json"), JsonSerializer.Serialize(manifest, SceneExport.JsonOptions));
            token.ThrowIfCancellationRequested();
            Directory.Move(staging, destination);
            return destination;
        }
        catch { if (Directory.Exists(staging)) Directory.Delete(staging, true); throw; }
    }
}

