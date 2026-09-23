namespace Iro.Core.Analysis;

public readonly record struct PixelPoint(double X, double Y);

/// <summary>Rotation around image centers, no scaling. Coordinates refer to pixel edges.</summary>
public sealed class ImageRotation
{
    public double Degrees { get; }
    public int SourceWidth { get; }
    public int SourceHeight { get; }
    public int Width { get; }
    public int Height { get; }
    private readonly double cos, sin;
    public ImageRotation(int sourceWidth, int sourceHeight, double degrees)
    {
        if (sourceWidth < 1 || sourceHeight < 1 || !double.IsFinite(degrees) || degrees is < -45 or > 45)
            throw new ArgumentException("Ungültige Bilddrehung.");
        SourceWidth = sourceWidth; SourceHeight = sourceHeight; Degrees = degrees;
        double radians = degrees * Math.PI / 180; cos = Math.Cos(radians); sin = Math.Sin(radians);
        Width = (int)Math.Ceiling(sourceWidth * Math.Abs(cos) + sourceHeight * Math.Abs(sin));
        Height = (int)Math.Ceiling(sourceWidth * Math.Abs(sin) + sourceHeight * Math.Abs(cos));
    }
    public PixelPoint ToSource(double x, double y) =>
        new((x - Width / 2d) * cos - (y - Height / 2d) * sin + SourceWidth / 2d,
            (x - Width / 2d) * sin + (y - Height / 2d) * cos + SourceHeight / 2d);
    public PixelPoint ToAligned(double x, double y) =>
        new((x - SourceWidth / 2d) * cos + (y - SourceHeight / 2d) * sin + Width / 2d,
            -(x - SourceWidth / 2d) * sin + (y - SourceHeight / 2d) * cos + Height / 2d);
    public PixelPoint[] ToSource(PixelRect rect) =>
        [ToSource(rect.X, rect.Y), ToSource(rect.Right, rect.Y), ToSource(rect.Right, rect.Bottom), ToSource(rect.X, rect.Bottom)];
    public PixelRect SourceBounds(PixelRect rect)
    {
        var p = ToSource(rect);
        int x = Math.Clamp((int)Math.Floor(p.Min(p => p.X)), 0, SourceWidth),
            y = Math.Clamp((int)Math.Floor(p.Min(p => p.Y)), 0, SourceHeight),
            right = Math.Clamp((int)Math.Ceiling(p.Max(p => p.X)), 0, SourceWidth),
            bottom = Math.Clamp((int)Math.Ceiling(p.Max(p => p.Y)), 0, SourceHeight);
        return new(x, y, right - x, bottom - y);
    }
}

internal sealed record AlignmentEstimate(double Degrees, double Confidence, bool Ambiguous);

internal static class ImageStraightener
{
    // Bounded edge search precedes any color-region/field detection.
    public static AlignmentEstimate Estimate(RgbFrame image, CancellationToken token)
    {
        double scale = Math.Max(1, Math.Max(image.Width, image.Height) / 480d);
        int w = (int)Math.Ceiling(image.Width / scale), h = (int)Math.Ceiling(image.Height / scale);
        if (w < 8 || h < 8) return new(0, 0, false);
        var colors = new RgbColor[w * h];
        for (int y = 0; y < h; y++)
        {
            token.ThrowIfCancellationRequested();
            for (int x = 0; x < w; x++)
                colors[y * w + x] = image.GetPixel(Math.Min(image.Width - 1, (int)((x + .5) * scale)),
                    Math.Min(image.Height - 1, (int)((y + .5) * scale)));
        }
        var magnitudes = new double[w * h]; var horizontal = new bool[w * h];
        for (int y = 1; y < h - 1; y++)
        for (int x = 1; x < w - 1; x++)
        {
            int index = y * w + x;
            double gradientBest = 0; bool direction = false;
            for (int channel = 0; channel < 3; channel++)
            {
                int C(int dx, int dy) { var c = colors[(y + dy) * w + x + dx]; return channel == 0 ? c.R : channel == 1 ? c.G : c.B; }
                double gx = C(1,-1) + 2*C(1,0) + C(1,1) - C(-1,-1) - 2*C(-1,0) - C(-1,1);
                double gy = C(-1,1) + 2*C(0,1) + C(1,1) - C(-1,-1) - 2*C(0,-1) - C(1,-1);
                double magnitude = gx * gx + gy * gy;
                if (magnitude > gradientBest) { gradientBest = magnitude; direction = Math.Abs(gx) >= Math.Abs(gy); }
            }
            magnitudes[index] = gradientBest; horizontal[index] = direction;
        }
        var edges = new List<PixelPoint>();
        for (int y = 2; y < h - 2; y++)
        for (int x = 2; x < w - 2; x++)
        {
            int index = y * w + x, offset = horizontal[index] ? 1 : w;
            double m = magnitudes[index];
            if (m >= 48 * 48 && m >= magnitudes[index - offset] && m > magnitudes[index + offset])
                edges.Add(new(x - w / 2d, y - h / 2d));
        }
        if (edges.Count < 60 || edges.Count > 20000) return new(0, 0, false);
        int radius = (int)Math.Ceiling(Math.Sqrt(w * w + h * h)) + 2;
        var first = new int[2 * radius + 1]; var second = new int[first.Length];
        double minimumLine = Math.Max(12, Math.Max(w, h) * .035);
        double Score(double angle)
        {
            token.ThrowIfCancellationRequested();
            Array.Clear(first); Array.Clear(second);
            double cos = Math.Cos(angle * Math.PI / 180), sin = Math.Sin(angle * Math.PI / 180);
            foreach (var p in edges)
            {
                first[radius + (int)Math.Round(p.X * cos + p.Y * sin)]++;
                second[radius + (int)Math.Round(-p.X * sin + p.Y * cos)]++;
            }
            double Peaks(int[] votes)
            {
                // Two orthogonal edge families are required; a single shadow edge is insufficient.
                double sum = 0;
                for (int n = 1; n < votes.Length - 1; n++)
                    if (votes[n] >= minimumLine && votes[n] >= votes[n-1] && votes[n] > votes[n+1])
                        sum += Math.Pow(votes[n] - minimumLine, 2);
                return sum;
            }
            return Math.Sqrt(Peaks(first) * Peaks(second));
        }
        var scores = Enumerable.Range(-45, 90).Select(angle => (Angle: (double)angle, Score: Score(angle))).ToArray();
        var best = scores.MaxBy(s => s.Score);
        if (best.Score < 100) return new(0, 0, false);
        double Distance(double angle) { double d = Math.Abs(angle - best.Angle); return Math.Min(d, 90 - d); }
        double alternative = scores.Where(s => Distance(s.Angle) >= 8).Max(s => s.Score);
        double confidence = Math.Clamp(1 - alternative / best.Score, 0, 1);
        if (confidence < .3) return new(0, confidence, true);
        double refinedAngle = best.Angle, refinedScore = best.Score;
        for (int step = -10; step <= 10; step++)
        {
            double angle = best.Angle + step / 10d, score = Score(angle);
            if (score > refinedScore) { refinedScore = score; refinedAngle = angle; }
        }
        if (refinedAngle < -45) refinedAngle += 90;
        if (refinedAngle >= 45) refinedAngle -= 90;
        return new(Math.Abs(refinedAngle) < .5 ? 0 : refinedAngle, confidence, false);
    }
}
