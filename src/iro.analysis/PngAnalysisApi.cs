using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Iro.Core.Analysis;

namespace Iro.Analysis;

/// <summary>Local .NET API used by IroGen. The Android app can use the same IImageAnalyzer with camera RGB buffers.</summary>
public interface IPngAnalysisApi
{
    Task<ImageAnalysis> AnalyzeAsync(ReadOnlyMemory<byte> png, AnalysisOptions options, CancellationToken token = default);
}

public sealed class PngAnalysisApi(IImageAnalyzer? analyzer = null) : IPngAnalysisApi
{
    private readonly IImageAnalyzer engine = analyzer ?? new ImageAnalyzer();
    public Task<ImageAnalysis> AnalyzeAsync(ReadOnlyMemory<byte> png, AnalysisOptions options, CancellationToken token = default) =>
        Task.Run(() => engine.Analyze(Decode(png, token), options, token), token);

    // Deliberately narrow contract: untagged images are declared sRGB by the caller.
    // Profile conversion and animated PNG are not part of this analysis adapter.
    private static void ValidateColorContract(ReadOnlySpan<byte> png)
    {
        bool srgb = false, otherColorMetadata = false, ended = false;
        int offset = 8;
        while (offset <= png.Length - 12)
        {
            int length = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.Slice(offset, 4));
            if (length < 0 || length > png.Length - offset - 12) throw new ArgumentException("Unvollständige PNG-Daten.");
            var type = png.Slice(offset + 4, 4);
            if (offset == 8 && (!type.SequenceEqual("IHDR"u8) || length != 13 || png[offset + 16] != 8))
                throw new ArgumentException("Die Analyse erwartet ein 8-Bit-PNG.");
            if (type.SequenceEqual("iCCP"u8)) throw new ArgumentException("ICC-Profile werden nicht umgerechnet. Bitte als sRGB-PNG exportieren.");
            if (type.SequenceEqual("acTL"u8)) throw new ArgumentException("Animierte PNG-Dateien werden nicht unterstützt.");
            if (type.SequenceEqual("sRGB"u8))
            {
                if (length != 1 || png[offset + 8] > 3) throw new ArgumentException("Ungültige sRGB-Kennzeichnung.");
                srgb = true;
            }
            otherColorMetadata |= type.SequenceEqual("gAMA"u8) || type.SequenceEqual("cHRM"u8);
            offset += length + 12;
            if (type.SequenceEqual("IEND"u8)) { ended = true; break; }
        }
        if (!ended || offset != png.Length) throw new ArgumentException("Unvollständige oder zusätzliche PNG-Daten.");
        if (otherColorMetadata && !srgb) throw new ArgumentException("Nicht eindeutig als sRGB gekennzeichnetes PNG. Bitte als sRGB exportieren.");
    }
    public static RgbFrame Decode(ReadOnlyMemory<byte> png, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        ReadOnlySpan<byte> signature = [137, 80, 78, 71, 13, 10, 26, 10];
        if (png.Length < 24 || png.Length > 64 * 1024 * 1024 || !png.Span[..8].SequenceEqual(signature))
            throw new ArgumentException("Erwartet wird eine PNG-Datei bis 64 MiB.");
        int width = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.Span.Slice(16, 4));
        int height = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.Span.Slice(20, 4));
        if (width < 1 || height < 1 || (long)width * height > 12_000_000) throw new ArgumentException("Bildgröße außerhalb des Analysebereichs.");
        ValidateColorContract(png.Span);
        using var stream = new MemoryStream(png.ToArray(), false);
        var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        if (decoder.Frames.Count != 1) throw new ArgumentException("Nur einzelne PNG-Bilder werden unterstützt.");
        var frame = decoder.Frames[0];
        if (frame.PixelWidth != width || frame.PixelHeight != height) throw new ArgumentException("Inkonsistente PNG-Abmessungen.");
        var bgra = new FormatConvertedBitmap(frame, PixelFormats.Bgra32, null, 0);
        var source = new byte[width * height * 4]; bgra.CopyPixels(source, width * 4, 0);
        var rgb = new byte[width * height * 3];
        for (int y = 0; y < height; y++)
        {
            token.ThrowIfCancellationRequested();
            for (int x = 0; x < width; x++)
            {
                int p = (y * width + x) * 4, q = (y * width + x) * 3;
                if (source[p + 3] != 255) throw new ArgumentException("Transparente Bilder besitzen keine eindeutige Wandfarbe.");
                rgb[q] = source[p + 2]; rgb[q + 1] = source[p + 1]; rgb[q + 2] = source[p];
            }
        }
        return new(width, height, width * 3, rgb);
    }
}

