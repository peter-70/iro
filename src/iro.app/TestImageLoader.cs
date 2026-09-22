using Android.Graphics;
using Iro.Core.Analysis;

namespace Iro.App;

internal static class TestImageLoader
{
    // Bundled, opaque sRGB PNG fixtures. Not a general camera/color-profile converter.
    public static async Task<RgbFrame> LoadAsync(string asset, CancellationToken token)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(asset);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory, token);
        token.ThrowIfCancellationRequested();
        using var options = new BitmapFactory.Options { InScaled = false, InPreferredConfig = Bitmap.Config.Argb8888 };
        using var bitmap = BitmapFactory.DecodeByteArray(memory.ToArray(), 0, checked((int)memory.Length), options)
            ?? throw new InvalidDataException("Testbild nicht lesbar.");
        int width = bitmap.Width, height = bitmap.Height;
        if ((long)width * height > 12_000_000) throw new InvalidDataException("Testbild zu groß.");
        var pixels = new int[width * height];
        bitmap.GetPixels(pixels, 0, width, 0, 0, width, height);
        var rgb = new byte[pixels.Length * 3];
        for (int y = 0; y < height; y++)
        {
            token.ThrowIfCancellationRequested();
            for (int x = 0; x < width; x++)
            {
                int i = y * width + x, color = pixels[i];
                if ((uint)color >> 24 != 255) throw new InvalidDataException("Testbild enthält transparente Pixel.");
                rgb[i * 3] = (byte)(color >> 16); rgb[i * 3 + 1] = (byte)(color >> 8); rgb[i * 3 + 2] = (byte)color;
            }
        }
        return new(width, height, width * 3, rgb);
    }
}
