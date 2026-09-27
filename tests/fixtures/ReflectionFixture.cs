using Iro.Core.Analysis;

namespace Iro.TestFixtures;

// Forward-only synthetic reflection model; never used by production analysis.
public static class ReflectionFixture
{
    public const int W = 800, H = 600;
    public static readonly string[] Shapes = ["Punkt", "Mehrere Punkte", "Harter Fleck klein", "Harter Fleck groß", "Weicher Glanz", "Lichtband", "Farbiger Reflex", "Kanalanschlag", "Gleichmäßiger Schleier"];
    public static PixelRect Field(int i) => new(550, 60 + i * 144, 160, 130);
    public static byte Material(int i, int c, bool bright) => i < 0 ? (byte)140 : (byte)((bright ? 180 : 60) + i * (bright ? 20 : 30) + c * (bright ? 3 : 15));
    static byte Encode(double v) => (byte)Math.Clamp((int)Math.Round(255 * (v <= .0031308 ? 12.92 * v : 1.055 * Math.Pow(v, 1 / 2.4) - .055)), 0, 255);
    static byte UniformOverlay(byte material) => Encode(.6 * ColorMath.Decode(material) + .4);
    static bool Contains(PixelRect r, int x, int y) => x >= r.X && x < r.Right && y >= r.Y && y < r.Bottom;

    // Builds a genuinely homogeneous scene whose surface colors already equal the
    // colors produced by the synthetic uniform overlay. No overlay is drawn here.
    // This is the constructive counterexample for single-image distinguishability.
    public static RgbFrame MakeCleanEquivalentToUniformOverlay(string target, bool horizontal, int noise)
    {
        byte[,] surfaces = new byte[4, 3]; // row 0: wall; rows 1..3: fields
        for (int field = -1; field < 3; field++)
        for (int c = 0; c < 3; c++)
        {
            bool transformed = target == "Wand" ? field < 0 : target == "Beide" || field == 1;
            byte material = Material(field, c, false);
            surfaces[field + 1, c] = transformed ? UniformOverlay(material) : material;
        }

        byte[] pixels = new byte[W * H * 3];
        var random = new Random(924137);
        for (int y = 0; y < H; y++)
        for (int x = 0; x < W; x++)
        {
            int field = Enumerable.Range(0, 3).FirstOrDefault(i => Contains(Field(i), x, y), -1);
            for (int c = 0; c < 3; c++)
            {
                int value = surfaces[field + 1, c];
                pixels[(y * W + x) * 3 + c] = (byte)Math.Clamp(value + (noise == 0 ? 0 : random.Next(-noise, noise + 1)), 0, 255);
            }
        }
        if (!horizontal) return new(W, H, W * 3, pixels);
        byte[] rotated = new byte[pixels.Length];
        for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
            Array.Copy(pixels, (y * W + x) * 3, rotated, (x * H + H - 1 - y) * 3, 3);
        return new(H, W, H * 3, rotated);
    }
    public static RgbFrame Make(string shape, string target, bool horizontal, int noise, bool bright = false, bool isolatedBand = false)
    {
        byte[] pixels = new byte[W * H * 3];
        var random = new Random(924137);
        for (int y = 0; y < H; y++)
        for (int x = 0; x < W; x++)
        {
            int field = Enumerable.Range(0, 3).FirstOrDefault(i => Contains(Field(i), x, y), -1);
            if (isolatedBand && field != 1) field = -1;
            bool affected = shape != "Kontrolle" && (target == "Wand" ? field < 0 : target == "Beide" || field == 1);
            // The wall overlay covers the actual left reference area and surrounding wall,
            // not a privileged measurement mask supplied to Iro.
            var region = field >= 0 ? Field(field) : new PixelRect(390, 170, 150, 200);
            double u = (x - region.X) / (double)region.Width, v = (y - region.Y) / (double)region.Height;
            double radius = Math.Sqrt(Math.Pow(u - .5, 2) + Math.Pow(v - .5, 2));
            double alpha = !affected ? 0 : shape switch
            {
                "Punkt" => radius < .025 ? .85 : 0,
                "Mehrere Punkte" => Math.Pow((u * 5 % 1) - .5, 2) + Math.Pow((v * 5 % 1) - .5, 2) < .015 && u is >= 0 and <= 1 && v is >= 0 and <= 1 ? .85 : 0,
                "Harter Fleck klein" => radius < .18 ? .65 : 0,
                "Harter Fleck groß" => radius < .43 ? .65 : 0,
                "Weicher Glanz" => .8 * Math.Exp(-radius * radius / .11),
                "Lichtband" => u is > .35 and < .65 ? .75 : 0,
                "Farbiger Reflex" => .7 * Math.Exp(-radius * radius / .13),
                "Kanalanschlag" => radius < .43 ? 1 : 0,
                "Gleichmäßiger Schleier" => .4,
                _ => 0
            };
            for (int c = 0; c < 3; c++)
            {
                double light = shape == "Farbiger Reflex" ? (c == 0 ? 1 : c == 1 ? .4 : .08) : 1;
                int value = Encode((1 - alpha) * ColorMath.Decode(Material(field, c, bright)) + alpha * light);
                pixels[(y * W + x) * 3 + c] = (byte)Math.Clamp(value + (noise == 0 ? 0 : random.Next(-noise, noise + 1)), 0, 255);
            }
        }
        if (!horizontal) return new(W, H, W * 3, pixels);
        byte[] rotated = new byte[pixels.Length];
        for (int y = 0; y < H; y++) for (int x = 0; x < W; x++)
            Array.Copy(pixels, (y * W + x) * 3, rotated, (x * H + H - 1 - y) * 3, 3);
        return new(H, W, H * 3, rotated);
    }
}
