using System.IO;
using Iro.Analysis;
using Iro.Core.Analysis;

namespace IroGenTests;

public class AndroidPresentationTests
{
    [Fact]
    public async Task OriginalBlurredImageReplacesValidValuesWithActualAnalyzerHint()
    {
        var presentation = new AnalysisPresentation();
        Task<RgbFrame> Load(string name, CancellationToken token) => Task.FromResult(
            PngAnalysisApi.Decode(File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, name)), token));
        await presentation.AnalyzeAsync(t => Load("test-normal.png", t));
        Assert.Equal(7, presentation.Fields.Count);
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));

        var loading = new TaskCompletionSource<RgbFrame>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pending = presentation.AnalyzeAsync(_ => loading.Task);
        Assert.True(presentation.IsBusy);
        Assert.Empty(presentation.Fields);
        loading.SetResult(await Load("test-blur.png", default));
        await pending;
        Assert.False(presentation.IsBusy);
        Assert.Equal("Keine zuverlässige Messung", presentation.Heading);
        Assert.Equal("Bild unscharf. Kamera ruhig halten und neu fokussieren.", presentation.Message);
        Assert.NotEmpty(presentation.Fields);
        Assert.All(presentation.Fields, f => Assert.Equal("–", f.Value));

        await presentation.AnalyzeAsync(t => Load("test-normal.png", t));
        Assert.Equal("Farbvergleich", presentation.Heading);
        Assert.All(presentation.Fields, f => Assert.NotEqual("–", f.Value));
    }

    [Fact]
    public async Task CancelledOlderImageCannotOverwriteANewerResult()
    {
        var presentation = new AnalysisPresentation();
        var old = new TaskCompletionSource<RgbFrame>(TaskCreationOptions.RunContinuationsAsynchronously);
        var pending = presentation.AnalyzeAsync(_ => old.Task);
        presentation.Cancel();
        await presentation.AnalyzeAsync(t => Task.FromResult(PngAnalysisApi.Decode(
            File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "test-normal.png")), t)));
        old.SetResult(new RgbFrame(10, 10, 30, new byte[300]));
        await pending;
        Assert.Equal("Farbvergleich", presentation.Heading);
        Assert.Equal(7, presentation.Fields.Count);
        Assert.False(presentation.IsBusy);
    }

    [Fact]
    public async Task LoadingFailureClearsPreviousMeasurements()
    {
        var presentation = new AnalysisPresentation();
        await presentation.AnalyzeAsync(t => Task.FromResult(PngAnalysisApi.Decode(
            File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "test-normal.png")), t)));
        await presentation.AnalyzeAsync(_ => throw new IOException("Test"));
        Assert.Empty(presentation.Fields);
        Assert.Equal("Bild nicht auswertbar", presentation.Heading);
        Assert.False(presentation.IsBusy);
    }
}
