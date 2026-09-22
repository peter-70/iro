using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Iro.Analysis;

namespace IroGenTests;

public class PngAdapterTests
{
    private static byte[] Png(byte alpha)
    {
        var pixels = new byte[8 * 8 * 4];
        for (int i = 3; i < pixels.Length; i += 4) pixels[i] = alpha;
        var source = BitmapSource.Create(8, 8, 96, 96, PixelFormats.Bgra32, null, pixels, 32);
        var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(source));
        using var stream = new MemoryStream(); encoder.Save(stream); return stream.ToArray();
    }

    [Fact]
    public async Task InvalidTransparentAndProfiledImagesAreExplicitlyRejected()
    {
        var api = new PngAnalysisApi();
        await Assert.ThrowsAsync<ArgumentException>(() => api.AnalyzeAsync(new byte[24], new()));
        await Assert.ThrowsAsync<ArgumentException>(() => api.AnalyzeAsync(Png(0), new()));
        var png = Png(255);
        // Reject unsupported profiles before decoding, instead of silently treating them as sRGB.
        byte[] profiled = png[..33].Concat(new byte[] { 0, 0, 0, 0, (byte)'i', (byte)'C', (byte)'C', (byte)'P', 0, 0, 0, 0 }).Concat(png[33..]).ToArray();
        var error = await Assert.ThrowsAsync<ArgumentException>(() => api.AnalyzeAsync(profiled, new()));
        Assert.Contains("ICC", error.Message);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => api.AnalyzeAsync(png, new(), new CancellationToken(true)));
    }
}
