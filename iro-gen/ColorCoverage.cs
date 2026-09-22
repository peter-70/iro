namespace IroGen;

public sealed record SceneColors(Rgb[] Palette, Rgb Wall, int AnchorField, double TargetDeltaE, string CoverageBand);
public sealed record CoverageBand(string Name, double Minimum, double? Maximum);

/// <summary>Stratified nominal distances, not percentages or perception thresholds.</summary>
public static class ColorCoverage
{
    public static readonly CoverageBand[] Bands =
    [
        new("Identisch", 0, 0), new("0–0,5", 0, .5), new("0,5–2", .5, 2),
        new("2–5", 2, 5), new("5–10", 5, 10), new("10–20", 10, 20),
        new("20–40", 20, 40), new("40–60", 40, 60), new("60–80", 60, 80),
        new("80–100", 80, 100), new("Über 100", 100, null)
    ];

    public static int[] Schedule(int count, int seed)
    {
        // For small batches include both ends; from 11 images onward cover every band.
        var indices = Enumerable.Range(0, count).Select(i => count < Bands.Length
            ? (int)Math.Round(i * (Bands.Length - 1.0) / Math.Max(1, count - 1)) : i % Bands.Length).ToArray();
        var random = new SeedRandom(seed);
        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = (int)(random.Next() * (i + 1));
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }
        return indices;
    }

    public static SceneColors Create(GeneratorOptions options, int bandIndex, CancellationToken token = default)
    {
        var band = Bands[bandIndex];
        var random = new SeedRandom(options.Seed);
        // Keep targets away from bin edges so 8-bit quantization cannot silently change coverage.
        double target = bandIndex == 0 ? 0 : bandIndex == Bands.Length - 1 ? 101 + 4 * random.Next()
            : band.Minimum + (band.Maximum!.Value - band.Minimum) * (.2 + .6 * random.Next());
        for (int attempt = 0; attempt < 12000; attempt++)
        {
            token.ThrowIfCancellationRequested();
            double hue = random.Next() * 360;
            double saturation = target >= 80 ? .85 + .15 * random.Next() : .15 + .85 * random.Next();
            double step = Math.Min(options.ShadeStep / 100, .96 / Math.Max(1, options.FieldCount - 1));
            double span = step * (options.FieldCount - 1);
            double center = .02 + span / 2 + random.Next() * (.96 - span);
            var palette = Enumerable.Range(0, options.FieldCount)
                .Select(i => ColorScience.Hsl(hue, saturation, center + span / 2 - i * step)).ToArray();
            int anchor = (int)(random.Next() * options.FieldCount);
            if (target == 0) return new(palette, palette[anchor], anchor + 1, 0, band.Name);
            var source = palette[anchor];
            var lab = ColorScience.ToLab(source);
            Rgb[] candidates = Enumerable.Range(0, 8).Select(i => new Rgb((byte)((i & 1) * 255), (byte)(((i >> 1) & 1) * 255), (byte)(((i >> 2) & 1) * 255)))
                .Concat(Enumerable.Range(0, 12).Select(_ => ColorScience.Hsl(random.Next() * 360, .3 + .7 * random.Next(), .03 + .94 * random.Next()))).ToArray();
            var suitable = candidates.Where(c => ColorScience.DeltaE00(lab, ColorScience.ToLab(c)) >= target).ToArray();
            if (suitable.Length == 0) continue;
            var far = suitable[(int)(random.Next() * suitable.Length)];
            double lo = 0, hi = 1;
            Rgb wall = source;
            // Bracket a crossing, without assuming global monotonicity of CIEDE2000 in RGB.
            for (int j = 0; j < 30; j++)
            {
                double t = (lo + hi) / 2;
                wall = new(ColorScience.Byte(source.R + (far.R - source.R) * t),
                    ColorScience.Byte(source.G + (far.G - source.G) * t), ColorScience.Byte(source.B + (far.B - source.B) * t));
                double value = ColorScience.DeltaE00(lab, ColorScience.ToLab(wall));
                if (value < target) lo = t; else hi = t;
            }
            double actual = ColorScience.DeltaE00(lab, ColorScience.ToLab(wall));
            if (actual > band.Minimum && (band.Maximum == null || actual <= band.Maximum))
                return new(palette, wall, anchor + 1, target, band.Name);
        }
        throw new InvalidOperationException($"Für den ΔE00-Bereich {band.Name} konnte mit diesen Farboptionen kein passendes Paar gefunden werden. Die Serie wird nicht als vollständig ausgegeben.");
    }
}
