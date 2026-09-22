using System.IO;
using System.Text.Json;
using IroGen;

namespace IroGenTests;

public class BatchTests
{
    private static void Sta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() => { try { action(); } catch (Exception e) { failure = e; } });
        thread.SetApartmentState(ApartmentState.STA); thread.Start(); thread.Join();
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    [Theory]
    [InlineData(100, 12345, 7, 5)]
    [InlineData(500, 748291, 7, 5)]
    [InlineData(100, 0, 20, 10)]
    [InlineData(100, -9982, 1, 0.5)]
    public void CoverageIncludesAllBandsWithUniquePalettes(int count, int seed, int fieldCount, double step)
    {
        var schedule = ColorCoverage.Schedule(count, seed);
        var palettes = new HashSet<string>();
        var actualBands = new HashSet<string>();
        for (int i = 0; i < count; i++)
        {
            int itemSeed = unchecked(seed + i * (int)0x9E3779B9u);
            var options = new GeneratorOptions { Seed = itemSeed, FieldCount = fieldCount, MatchingField = 1, ShadeStep = step };
            var colors = ColorCoverage.Create(options, schedule[i]);
            Assert.Equal(fieldCount, colors.Palette.Length);
            Assert.True(palettes.Add(string.Join("/", colors.Palette.Select(c => c.Hex))));
            var band = ColorCoverage.Bands[schedule[i]];
            double delta = ColorScience.DeltaE00(ColorScience.ToLab(colors.Wall), ColorScience.ToLab(colors.Palette[colors.AnchorField-1]));
            if (schedule[i] == 0) Assert.Equal(0, delta);
            else { Assert.True(delta > band.Minimum); if (band.Maximum != null) Assert.True(delta <= band.Maximum); }
            actualBands.Add(colors.CoverageBand);
            var again = ColorCoverage.Create(options, schedule[i]);
            Assert.Equal(colors.Wall, again.Wall); Assert.Equal(colors.Palette, again.Palette);
        }
        Assert.Equal(ColorCoverage.Bands.Length, actualBands.Count);
        Assert.All(Enumerable.Range(0, ColorCoverage.Bands.Length), band => Assert.InRange(schedule.Count(i => i == band), count / 11, count / 11 + 1));
    }

    [Fact]
    public void SeriesWritesAllImagesAndCanBeSavedAndSubmittedIndependently() => Sta(() =>
    {
        string root = Path.Combine(Path.GetTempPath(), "irogen-batchtest-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            File.WriteAllText(Path.Combine(root, "iro.slnx"), "<Solution />");
            File.WriteAllText(Path.Combine(root, "IRO-KONSOLIDIERTER-PLAN.md"), "Isoliertes Testprojekt");
            var options = new GeneratorOptions { Width=320, Height=320, RotationDegrees=7, Glare=Severity.Light };
            using var batch = BatchGenerator.Generate(options, 11, true, root);
            Assert.Equal(11, batch.Items.Count);
            foreach (var item in batch.Items)
            {
                Assert.Equal(7, item.Options.RotationDegrees); Assert.Equal(Severity.Light, item.Options.Glare);
                var scene = item.Load();
                Assert.Equal(320, scene.Image.PixelWidth);
                Assert.Equal(item.Options.Colors!.Wall, scene.Wall);
                Assert.Equal(item.Options.Colors.Palette, scene.Fields.Select(f => f.Color));
                using var description = JsonDocument.Parse(File.ReadAllText(Path.Combine(item.Folder, item.CaptureId+".json")));
                var restored = description.RootElement.GetProperty("conditions").GetProperty("generator").GetProperty("parameters").GetProperty("options").Deserialize<GeneratorOptions>(SceneExport.JsonOptions)!;
                var rerender = SceneGenerator.Generate(restored);
                var originalPixels = new byte[320*320*4]; var restoredPixels = new byte[originalPixels.Length];
                scene.Image.CopyPixels(originalPixels,320*4,0); rerender.Image.CopyPixels(restoredPixels,320*4,0);
                Assert.Equal(originalPixels,restoredPixels);
            }
            string saved = BatchGenerator.SaveAll(batch, Path.Combine(root,"saved"));
            var submitted = IroTestHandoff.Submit(batch,root);
            Assert.Contains("Bereit für die lokale Bildanalyse",submitted.Status);
            using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(saved,"series.json")));
            Assert.Equal(11,manifest.RootElement.GetProperty("actualCount").GetInt32());
            Assert.Equal(11,manifest.RootElement.GetProperty("captures").GetArrayLength());
            using var request = JsonDocument.Parse(File.ReadAllText(Path.Combine(submitted.Folder,"request.json")));
            Assert.True(request.RootElement.GetProperty("analysisAvailable").GetBoolean());
            string? handoffArtifacts = Environment.GetEnvironmentVariable("IROGEN_HANDOFF_OUTPUT");
            if (handoffArtifacts != null)
            {
                string target = Path.Combine(handoffArtifacts, submitted.RequestId);
                foreach (string file in Directory.EnumerateFiles(submitted.Folder,"*",SearchOption.AllDirectories))
                {
                    string copy = Path.Combine(target,Path.GetRelativePath(submitted.Folder,file));
                    Directory.CreateDirectory(Path.GetDirectoryName(copy)!);
                    File.Copy(file,copy);
                }
            }
            batch.Dispose();
            Assert.Equal(11,Directory.GetFiles(saved,"*.png",SearchOption.AllDirectories).Length);
            Assert.Equal(11,Directory.GetFiles(submitted.Folder,"*.png",SearchOption.AllDirectories).Length);
            Assert.Empty(Directory.GetDirectories(root,"*.incomplete",SearchOption.AllDirectories));
        }
        finally { Directory.Delete(root,true); }
    });

    [Fact]
    public void CancellationDoesNotLeavePartialSeriesOrExport() => Sta(() =>
    {
        string root = Path.Combine(Path.GetTempPath(),"irogen-cancel-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            using var cancellation = new CancellationTokenSource();
            var progress = new InlineProgress<BatchProgress>(p => { if (p.Completed == 2) cancellation.Cancel(); });
            Assert.Throws<OperationCanceledException>(() => BatchGenerator.Generate(new(){Width=320,Height=320},20,true,root,progress,cancellation.Token));
            Assert.Empty(Directory.GetDirectories(root));
            using var batch = BatchGenerator.Generate(new(){Width=320,Height=320},2,false,root);
            string destination = Path.Combine(root,"export");
            Assert.Throws<OperationCanceledException>(() => BatchGenerator.SaveAll(batch,destination,cancellation.Token));
            Assert.Empty(Directory.GetDirectories(destination));
            Assert.Equal(2,batch.Items.Count);
        }
        finally { Directory.Delete(root,true); }
    });

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    public void FullHundredAndFiveHundredImageBatchesAreComplete(int count) => Sta(() =>
    {
        string cache = Path.Combine(Path.GetTempPath(), "irogen-large-" + Guid.NewGuid().ToString("N"));
        string? artifacts = Environment.GetEnvironmentVariable("IROGEN_SERIES_OUTPUT");
        Directory.CreateDirectory(cache);
        try
        {
            using var batch = BatchGenerator.Generate(new() { Width = 320, Height = 320, Seed = 46883 }, count, true, cache);
            Assert.Equal(count, batch.Items.Count);
            Assert.Equal(count, Directory.GetFiles(batch.Root, "*.png", SearchOption.AllDirectories).Length);
            Assert.Equal(count, batch.Items.Select(i => string.Join("/",i.Fields.Select(f => f.Hex))).Distinct().Count());
            Assert.Equal(0, batch.Items.Min(i => i.MinimumDeltaE));
            Assert.True(batch.Items.Max(i => i.MaximumDeltaE) > 100);
            Assert.Equal(11,batch.Items.Select(i => i.Options.Colors!.CoverageBand).Distinct().Count());
            foreach(var item in batch.Items) Assert.Equal(320,item.Load().Image.PixelWidth);
            if (artifacts != null)
                BatchGenerator.SaveAll(batch, Path.Combine(artifacts, count.ToString()));
        }
        finally { Directory.Delete(cache,true); }
    });
    [Fact]
    public void CopyFailureDoesNotPublishAnIncompleteSeries() => Sta(() =>
    {
        string root = Path.Combine(Path.GetTempPath(),"irogen-copyfailure-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            using var batch = BatchGenerator.Generate(new(){Width=320,Height=320},2,false,root);
            Assert.Throws<ArgumentException>(() => BatchGenerator.SaveAll(batch,batch.Root));
            File.Delete(batch.Items[1].ImagePath);
            string destination = Path.Combine(root,"export");
            Assert.Throws<FileNotFoundException>(() => BatchGenerator.SaveAll(batch,destination));
            Assert.Empty(Directory.GetDirectories(destination));
        }
        finally { Directory.Delete(root,true); }
    });
    private sealed class InlineProgress<T>(Action<T> action) : IProgress<T> { public void Report(T value) => action(value); }
}




