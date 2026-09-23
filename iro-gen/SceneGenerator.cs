using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IroGen;

public sealed record PixelBounds(int X, int Y, int Width, int Height);
public sealed record FieldInfo(string FieldId, Rgb Color, double DeltaE00, Point[] Polygon, PixelBounds? Bounds)
{
    public string Hex => Color.Hex;
    public string ExpectedText => DeltaE00.ToString("F2", CultureInfo.GetCultureInfo("de-DE"));
}
public sealed record GeneratedScene(GeneratorOptions Options, Rgb Wall, IReadOnlyList<FieldInfo> Fields, BitmapSource Image)
{
    public SpatialSceneInfo? SpatialGeometry { get; init; }
}

public static class SceneGenerator
{
    public const string Version = "1.4.0";

    // WPF text rendering requires an STA. The caller runs this on a dedicated STA worker.
    public static GeneratedScene Generate(GeneratorOptions o, CancellationToken cancellationToken = default)
    {
        o.Validate();
        var random = new SeedRandom(o.Seed);
        double hue = random.Next() * 360, saturation = .25 + random.Next() * .40;
        double step = Math.Min(o.ShadeStep / 100, .76 / Math.Max(1, o.FieldCount - 1));
        double span = step * (o.FieldCount - 1), center = .12 + span / 2 + random.Next() * (.76 - span);
        Rgb[] colors = Enumerable.Range(0, o.FieldCount).Select(i => ColorScience.Hsl(hue, saturation, center + span / 2 - i * step)).ToArray();
        Rgb wall = colors[o.MatchingField - 1];
        if (o.WallDifference == WallDifference.Opposite) wall = ColorScience.Hsl(hue + 180, .65, .5);
        else if (o.WallDifference != WallDifference.Exact)
        {
            double target = o.WallDifference switch { WallDifference.Light => 1, WallDifference.Medium => 4, _ => 12 };
            var source = wall;
            var far = ColorScience.Hsl(hue + 180, .75, .5);
            // Find an approximately specified nominal distance after quantization to 8-bit sRGB.
            double lo = 0, hi = 1;
            for (int i = 0; i < 24; i++)
            {
                double mix = (lo + hi) / 2;
                wall = new(ColorScience.Byte(source.R + (far.R - source.R) * mix), ColorScience.Byte(source.G + (far.G - source.G) * mix), ColorScience.Byte(source.B + (far.B - source.B) * mix));
                if (ColorScience.DeltaE00(ColorScience.ToLab(source), ColorScience.ToLab(wall)) < target) lo = mix; else hi = mix;
            }
        }

        if (o.Colors is { } assigned)
        {
            colors = assigned.Palette;
            wall = assigned.Wall;
        }

        bool vertical = o.Orientation == StripOrientation.Vertical;
        var geometry = new SceneGeometry(o);
        Point Project(double u, double v) => geometry.Project(u, v);

        const int localWidth = 600;
        // Keep label rasterization independent of final camera distance.
        int localHeight = Math.Clamp((int)(600 * (vertical ? o.Height : o.Width) * o.StripLengthPercent / ((vertical ? o.Width : o.Height) * o.StripWidthPercent)), 600, 6000);
        double border = 9, gap = localHeight / (double)o.FieldCount * o.GapPercent / 100;
        double usable = localHeight - 2 * border - gap * (o.FieldCount - 1);
        double[] weights = Enumerable.Range(0, o.FieldCount).Select(i => o.VariableFieldHeights ? .75 + random.Next() * .5 : 1).ToArray();
        double totalWeight = weights.Sum();
        var fields = new List<FieldInfo>();
        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, localWidth, localHeight));
            double y = border;
            for (int i = 0; i < o.FieldCount; i++)
            {
                double h = usable * weights[i] / totalWeight;
                double fieldWidth = (localWidth - 2 * border) * (o.FieldWidthFactors?[i] ?? 1);
                var rect = new Rect((localWidth - fieldWidth) / 2, y, fieldWidth, h);
                var rgb = colors[i];
                double delta = ColorScience.DeltaE00(ColorScience.ToLab(wall), ColorScience.ToLab(rgb));
                dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(rgb.R, rgb.G, rgb.B)), null, rect);
                if (o.Labels)
                {
                    string label = $"{i + 1:00}   Soll ΔE00 {delta.ToString("F2", CultureInfo.GetCultureInfo("de-DE"))}";
                    double fontSize = Math.Min(27, h * .17);
                    var brush = ColorScience.ToLab(rgb).L > 55 ? Brushes.Black : Brushes.White;
                    var text = new FormattedText(label, CultureInfo.GetCultureInfo("de-DE"), FlowDirection.LeftToRight, new Typeface("Segoe UI"), fontSize, brush, 1);
                    dc.DrawText(text, new Point(rect.Left + 12, y + h - text.Height - Math.Min(10, h * .04)));
                }
                Point[] polygon = [Project(rect.Left / localWidth, rect.Top / localHeight), Project(rect.Right / localWidth, rect.Top / localHeight), Project(rect.Right / localWidth, rect.Bottom / localHeight), Project(rect.Left / localWidth, rect.Bottom / localHeight)];
                fields.Add(new($"field-{i + 1}", rgb, delta, polygon, Bounds(polygon, o.Width, o.Height)));
                y += h + gap;
            }
        }
        var local = new RenderTargetBitmap(localWidth, localHeight, 96, 96, PixelFormats.Pbgra32);
        local.Render(visual);
        byte[] strip = new byte[localWidth * localHeight * 4];
        local.CopyPixels(strip, localWidth * 4, 0);
        byte[] pixels = new byte[o.Width * o.Height * 4];
        double exposure = Math.Pow(2, o.ExposureStops);
        var dirt = Enumerable.Range(0, (int)o.Dirt * 18).Select(_ => (X: random.Next(), Y: random.Next(), Radius: .005 + random.Next() * .025)).ToArray();
        for (int y = 0; y < o.Height; y++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            for (int x = 0; x < o.Width; x++)
            {
                var uv = geometry.Unproject(x + .5, y + .5);
                double u = uv.X, v = uv.Y;
                bool inside = u >= 0 && u < 1 && v >= 0 && v < 1;
                if (inside && o.RoundedTop && v < .055)
                    inside = v >= .055 * Math.Pow(2 * u - 1, 2);
                double r = wall.R, g = wall.G, b = wall.B;
                if (inside)
                {
                    int sx = Math.Clamp((int)(u * localWidth), 0, localWidth - 1), sy = Math.Clamp((int)(v * localHeight), 0, localHeight - 1);
                    int si = (sy * localWidth + sx) * 4;
                    b = strip[si]; g = strip[si + 1]; r = strip[si + 2];
                }
                double nx = (x + .5) / o.Width, ny = (y + .5) / o.Height;
                double lighting = inside ? 1 : geometry.WallShadow(x + .5, y + .5, o.Width, o.Height);
                if (o.Shadows != Severity.None)
                    lighting *= 1 - (int)o.Shadows * .22 * (.5 + .5 * Math.Tanh((nx + .55 * ny - .85) * 35));
                if (o.Vignette != Severity.None)
                    lighting *= Math.Max(.1, 1 - (int)o.Vignette * .30 * (Math.Pow(2 * nx - 1, 2) + Math.Pow(2 * ny - 1, 2)));
                // Apply exposure and illumination in linear light.
                if (lighting != 1 || exposure != 1)
                {
                    r = ColorScience.Encode(ColorScience.Linear(r / 255) * lighting * exposure);
                    g = ColorScience.Encode(ColorScience.Linear(g / 255) * lighting * exposure);
                    b = ColorScience.Encode(ColorScience.Linear(b / 255) * lighting * exposure);
                }
                if (o.Texture != Severity.None && !inside)
                {
                    double texture = (random.Next() - .5) * (int)o.Texture * 10;
                    r += texture; g += texture; b += texture;
                }
                foreach (var spot in dirt)
                {
                    double d = Math.Pow((nx - spot.X) / spot.Radius, 2) + Math.Pow((ny - spot.Y) / spot.Radius, 2);
                    if (d < 1) { double a = .2 + .15 * (int)o.Dirt; r *= 1 - a; g *= 1 - a; b *= 1 - a; break; }
                }
                if (o.Occlusion != Severity.None && inside && v > .38 && v < .38 + .11 * (int)o.Occlusion && u > .15)
                { r = 164; g = 116; b = 82; }
                double glare = o.Glare == Severity.None ? 0 : Math.Min(.98, (int)o.Glare * .36 * Math.Exp(-Math.Pow((u - .55) / .36, 2) - Math.Pow((v - .38) / .23, 2)) * (inside ? 1 : .12));
                double haze = (int)o.Haze * .13;
                double whiteMix = 1 - (1 - glare) * (1 - haze);
                r += (255 - r) * whiteMix; g += (255 - g) * whiteMix; b += (255 - b) * whiteMix;
                if (o.Noise != Severity.None)
                {
                    r += (random.Next() - .5) * (int)o.Noise * 12;
                    g += (random.Next() - .5) * (int)o.Noise * 12;
                    b += (random.Next() - .5) * (int)o.Noise * 12;
                }
                int index = (y * o.Width + x) * 4;
                pixels[index] = ColorScience.Byte(b); pixels[index + 1] = ColorScience.Byte(g); pixels[index + 2] = ColorScience.Byte(r); pixels[index + 3] = 255;
            }
        }
        double scale = Math.Min(o.Width, o.Height) / 1200.0;
        if (o.Blur != Severity.None)
        {
            int radius = Math.Max(1, (int)Math.Round((o.Blur switch { Severity.Light => 2, Severity.Medium => 6, _ => 16 }) * scale));
            // Three separable box passes approximate a defocus/Gaussian blur in linear time.
            for (int pass = 0; pass < 3; pass++)
            {
                pixels = BoxBlur(pixels, o.Width, o.Height, radius, true, cancellationToken);
                pixels = BoxBlur(pixels, o.Width, o.Height, radius, false, cancellationToken);
            }
        }
        if (o.MotionBlur != Severity.None)
            pixels = BoxBlur(pixels, o.Width, o.Height, Math.Max(1, (int)((o.MotionBlur switch { Severity.Light => 7, Severity.Medium => 22, _ => 55 }) * scale)), true, cancellationToken);
        var image = BitmapSource.Create(o.Width, o.Height, 96, 96, PixelFormats.Bgra32, null, pixels, o.Width * 4);
        image.Freeze();
        return new(o, wall, fields.AsReadOnly(), image) { SpatialGeometry = geometry.Info };
    }

    public static PixelBounds? Bounds(Point[] polygon, int width, int height)
    {
        int x = Math.Clamp((int)Math.Floor(polygon.Min(p => p.X)), 0, width), y = Math.Clamp((int)Math.Floor(polygon.Min(p => p.Y)), 0, height);
        int right = Math.Clamp((int)Math.Ceiling(polygon.Max(p => p.X)), 0, width), bottom = Math.Clamp((int)Math.Ceiling(polygon.Max(p => p.Y)), 0, height);
        return right > x && bottom > y ? new(x, y, right - x, bottom - y) : null;
    }

    private static byte[] BoxBlur(byte[] source, int width, int height, int radius, bool horizontal, CancellationToken token)
    {
        byte[] result = new byte[source.Length];
        int lines = horizontal ? height : width, length = horizontal ? width : height, count = 2 * radius + 1;
        for (int line = 0; line < lines; line++)
        {
            token.ThrowIfCancellationRequested();
            int Index(int p) => horizontal ? (line * width + p) * 4 : (p * width + line) * 4;
            for (int channel = 0; channel < 3; channel++)
            {
                int sum = 0;
                for (int p = -radius; p <= radius; p++) sum += source[Index(Math.Clamp(p, 0, length - 1)) + channel];
                for (int p = 0; p < length; p++)
                {
                    result[Index(p) + channel] = (byte)((sum + count / 2) / count);
                    sum -= source[Index(Math.Clamp(p - radius, 0, length - 1)) + channel];
                    sum += source[Index(Math.Clamp(p + radius + 1, 0, length - 1)) + channel];
                }
            }
        }
        for (int i = 3; i < result.Length; i += 4) result[i] = 255;
        return result;
    }
}

