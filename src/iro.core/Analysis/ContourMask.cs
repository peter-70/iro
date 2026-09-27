namespace Iro.Core.Analysis;

// Geometry-only masks for modest, well-supported quadrilaterals. No pixel edits.
internal static class ContourMask
{
    internal static IReadOnlyList<PixelPoint>? Fit(int[] points, int count, int width,
        int left, int top, int right, int bottom, double scale)
    {
        int w = right - left + 1, h = bottom - top + 1;
        if (Math.Min(w, h) < 30) return null;
        var minX = Enumerable.Repeat(int.MaxValue, h).ToArray();
        var maxX = Enumerable.Repeat(int.MinValue, h).ToArray();
        var minY = Enumerable.Repeat(int.MaxValue, w).ToArray();
        var maxY = Enumerable.Repeat(int.MinValue, w).ToArray();
        for (int i = 0; i < count; i++)
        {
            int x = points[i] % width - left, y = points[i] / width - top;
            minX[y] = Math.Min(minX[y], x); maxX[y] = Math.Max(maxX[y], x);
            minY[x] = Math.Min(minY[x], y); maxY[x] = Math.Max(maxY[x], y);
        }
        static (double A, double B)? Line(int[] values)
        {
            int start = values.Length / 5, end = values.Length - start;
            if (Enumerable.Range(start, end - start).Any(i => values[i] is int.MaxValue or int.MinValue)) return null;
            double mx = (start + end - 1) / 2d, my = Enumerable.Range(start, end - start).Average(i => values[i]);
            double xx = 0, xy = 0;
            for (int i = start; i < end; i++) { xx += (i - mx) * (i - mx); xy += (i - mx) * (values[i] - my); }
            double a = xy / xx, b = my - a * mx;
            // Every fitted contour sample must support the line, not merely its average.
            if (Enumerable.Range(start, end - start).Any(i => Math.Abs(values[i] - (a * i + b)) > 1)) return null;
            return (a, b);
        }
        var l = Line(minX); var r = Line(maxX); var t = Line(minY); var b = Line(maxY);
        if (l == null || r == null || t == null || b == null) return null;
        var lines = new[] { l.Value, r.Value, t.Value, b.Value };
        if (lines.Max(p => Math.Abs(p.A)) > .4 || lines.Max(p => Math.Abs(p.A)) < .02) return null;
        double WidthAt(double y) => (r.Value.A - l.Value.A) * y + r.Value.B - l.Value.B + 1;
        double HeightAt(double x) => (b.Value.A - t.Value.A) * x + b.Value.B - t.Value.B + 1;
        bool Modest(double first, double last) => Math.Min(first, last) > 0 && Math.Min(first, last) / Math.Max(first, last) >= .85;
        if (!Modest(WidthAt(0), WidthAt(h - 1)) || !Modest(HeightAt(0), HeightAt(w - 1))) return null;
        IReadOnlyList<PixelPoint> polygon = [new(0, 0), new(w, 0), new(w, h), new(0, h)];
        // Inset supported edges by 1.5 detection pixels; fitted coordinates are pixel centers.
        polygon = Clip(polygon, p => p.X - (l.Value.A * (p.Y - .5) + l.Value.B + 2));
        polygon = Clip(polygon, p => r.Value.A * (p.Y - .5) + r.Value.B - 1 - p.X);
        polygon = Clip(polygon, p => p.Y - (t.Value.A * (p.X - .5) + t.Value.B + 2));
        polygon = Clip(polygon, p => b.Value.A * (p.X - .5) + b.Value.B - 1 - p.Y);
        return polygon.Count < 3 ? null : polygon.Select(p => new PixelPoint((p.X + left) * scale, (p.Y + top) * scale)).ToArray();
    }
    internal static IReadOnlyList<PixelPoint> Clip(IReadOnlyList<PixelPoint> polygon, Func<PixelPoint, double> distance)
    {
        var result = new List<PixelPoint>();
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i]; var b = polygon[(i + 1) % polygon.Count];
            double da = distance(a), db = distance(b);
            if (da >= 0) result.Add(a);
            if ((da >= 0) != (db >= 0)) { double t = da / (da - db); result.Add(new(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y))); }
        }
        return result;
    }
    internal static IReadOnlyList<PixelPoint> Intersect(IReadOnlyList<PixelPoint> polygon, PixelRect bounds)
    {
        polygon = Clip(polygon, p => p.X - bounds.X);
        polygon = Clip(polygon, p => bounds.Right - p.X);
        polygon = Clip(polygon, p => p.Y - bounds.Y);
        return Clip(polygon, p => bounds.Bottom - p.Y);
    }
    internal static bool Contains(IReadOnlyList<PixelPoint> polygon, double x, double y)
    {
        if (polygon.Count < 3) return false;
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i]; var b = polygon[(i + 1) % polygon.Count];
            if ((b.X - a.X) * (y - a.Y) - (b.Y - a.Y) * (x - a.X) < -1e-8) return false;
        }
        return true;
    }
}
