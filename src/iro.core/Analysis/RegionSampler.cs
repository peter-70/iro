namespace Iro.Core.Analysis;

public static class RegionSampler
{
    public static RegionMeasurement Measure(RgbFrame image, PixelRect bounds, AnalysisOptions options, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(image); ArgumentNullException.ThrowIfNull(options); options.Validate();
        if (bounds.X < 0 || bounds.Y < 0 || bounds.Width <= 0 || bounds.Height <= 0 || bounds.Right > image.Width || bounds.Bottom > image.Height)
            throw new ArgumentException("Messfläche liegt außerhalb des Bilds.");
        int step = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(bounds.Area / 40000d)));
        var samples = new List<RgbColor>();
        int[] r = new int[256], g = new int[256], b = new int[256];
        int nearLimits = 0;
        var tiles = Enumerable.Range(0, 9).Select(_ => new int[3 * 256]).ToArray();
        var tileCounts = new int[9];
        for (int y = bounds.Y; y < bounds.Bottom; y += step)
        {
            token.ThrowIfCancellationRequested();
            for (int x = bounds.X; x < bounds.Right; x += step)
            {
                var pixel = image.GetPixel(x, y); samples.Add(pixel); r[pixel.R]++; g[pixel.G]++; b[pixel.B]++;
                int tile = Math.Min(2, (y - bounds.Y) * 3 / bounds.Height) * 3 + Math.Min(2, (x - bounds.X) * 3 / bounds.Width);
                tiles[tile][pixel.R]++; tiles[tile][256 + pixel.G]++; tiles[tile][512 + pixel.B]++; tileCounts[tile]++;
                if (pixel.R is <= 1 or >= 254 || pixel.G is <= 1 or >= 254 || pixel.B is <= 1 or >= 254) nearLimits++;
            }
        }
        static int Median(int[] histogram, int count)
        {
            int cumulative = 0;
            for (int i = 0; i < histogram.Length; i++) { cumulative += histogram[i]; if (cumulative > count / 2) return i; }
            return 0;
        }
        int mr = Median(r, samples.Count), mg = Median(g, samples.Count), mb = Median(b, samples.Count);
        int[] deviations = new int[256];
        foreach (var p in samples) deviations[Math.Max(Math.Abs(p.R - mr), Math.Max(Math.Abs(p.G - mg), Math.Abs(p.B - mb)))]++;
        int mad = Median(deviations, samples.Count);
        // Explicit MAD=0 handling: a small quantization/noise floor, never division by zero.
        double tolerance = Math.Max(4, 3 * mad);
        int retained = 0;
        double lr = 0, lg = 0, lb = 0;
        foreach (var p in samples)
        {
            if (Math.Max(Math.Abs(p.R - mr), Math.Max(Math.Abs(p.G - mg), Math.Abs(p.B - mb))) > tolerance) continue;
            lr += ColorMath.Decode(p.R); lg += ColorMath.Decode(p.G); lb += ColorMath.Decode(p.B); retained++;
        }
        // Central histogram means tolerate print/outliers without the discontinuous
        // median jump of a balanced two-tone texture. Used only for quality, not the measured color.
        static double CentralMean(int[] histogram, int offset, int count)
        {
            double low = count * .25, high = count * .75, sum = 0;
            int cumulative = 0;
            for (int value = 0; value < 256; value++)
            {
                int next = cumulative + histogram[offset + value];
                double weight = Math.Max(0, Math.Min(next, high) - Math.Max(cumulative, low));
                sum += weight * ColorMath.Decode((byte)value);
                cumulative = next;
            }
            return sum / (high - low);
        }
        var tileColors = Enumerable.Range(0, 9).Where(i => tileCounts[i] >= 16)
            .Select(i => ColorMath.LinearToLab(CentralMean(tiles[i], 0, tileCounts[i]),
                CentralMean(tiles[i], 256, tileCounts[i]), CentralMean(tiles[i], 512, tileCounts[i]))).ToArray();
        double spatialDelta = 0;
        for (int a = 0; a < tileColors.Length; a++)
        for (int bIndex = a + 1; bIndex < tileColors.Length; bIndex++)
            spatialDelta = Math.Max(spatialDelta, ColorMath.DeltaE00(tileColors[a], tileColors[bIndex]));
        double rejected = 1 - retained / (double)samples.Count;
        string? reason = Math.Min(bounds.Width, bounds.Height) < 20 || retained < options.MinimumSamples ? "Zu wenig nutzbare Bildfläche." :
            spatialDelta > options.MaximumSpatialDeltaE ? "Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden." :
            mad > options.MaximumChannelMad || rejected > options.MaximumOutlierFraction ? "Messfläche durch Flecken, Reflexe oder ungleichmäßiges Licht gestört." : null;
        return new(bounds, reason == null, reason, reason == null ? ColorMath.LinearToLab(lr / retained, lg / retained, lb / retained) : null,
            samples.Count, rejected, mad, nearLimits / (double)samples.Count) { SpatialDeltaE = spatialDelta };
    }
}

