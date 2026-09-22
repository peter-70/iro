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
        for (int y = bounds.Y; y < bounds.Bottom; y += step)
        {
            token.ThrowIfCancellationRequested();
            for (int x = bounds.X; x < bounds.Right; x += step)
            {
                var pixel = image.GetPixel(x, y); samples.Add(pixel); r[pixel.R]++; g[pixel.G]++; b[pixel.B]++;
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
        double rejected = 1 - retained / (double)samples.Count;
        string? reason = Math.Min(bounds.Width, bounds.Height) < 20 || retained < options.MinimumSamples ? "Zu wenig nutzbare Bildfläche." :
            mad > options.MaximumChannelMad || rejected > options.MaximumOutlierFraction ? "Messfläche durch Flecken, Reflexe oder ungleichmäßiges Licht gestört." : null;
        return new(bounds, reason == null, reason, reason == null ? ColorMath.LinearToLab(lr / retained, lg / retained, lb / retained) : null,
            samples.Count, rejected, mad, nearLimits / (double)samples.Count);
    }
}

