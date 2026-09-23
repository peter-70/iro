namespace Iro.Core.Analysis;

internal sealed record Detection(IReadOnlyList<PixelRect> Fields, bool Ambiguous, bool HadSmallRegions, string Orientation, IReadOnlyList<PixelRect>? BorderRegions = null, IReadOnlyList<PixelRect>? InconsistentFields = null, IReadOnlyDictionary<PixelRect, (double Vertical, double Horizontal)>? WidthChanges = null);

internal static class StripDetector
{
    // Use component contours, not bounding-box differences between distinct fields.
    // Inner thirds avoid isolated tip/corner pixels. No color values are modified.
    private static double ComponentWidthChange(int[] points, int count, int imageWidth,
        int start, int end, bool vertical)
    {
        int length = end - start + 1;
        if (length < 9) return 0;
        var minimum = Enumerable.Repeat(int.MaxValue, length).ToArray();
        var maximum = Enumerable.Repeat(int.MinValue, length).ToArray();
        for (int i = 0; i < count; i++)
        {
            int x = points[i] % imageWidth, y = points[i] / imageWidth;
            int row = (vertical ? y : x) - start, cross = vertical ? x : y;
            minimum[row] = Math.Min(minimum[row], cross);
            maximum[row] = Math.Max(maximum[row], cross);
        }
        double AverageWidth(int from, int to)
        {
            double sum = 0; int samples = 0;
            for (int row = from; row < to; row++)
                if (maximum[row] >= minimum[row])
                { sum += maximum[row] - minimum[row] + 1; samples++; }
            return samples == 0 ? 0 : sum / samples;
        }
        double first = AverageWidth(length / 6, length / 3);
        double last = AverageWidth(2 * length / 3, 5 * length / 6);
        // Sub-pixel/one-pixel quantization cannot provide a taper signal.
        return first <= 0 || last <= 0 || Math.Abs(last - first) < 2
            ? 0 : (last - first) / Math.Max(first, last);
    }
    public static Detection Detect(RgbFrame image, AnalysisOptions options, CancellationToken token)
    {
        double scale = Math.Max(1, Math.Max(image.Width, image.Height) / (double)options.DetectionLongestSide);
        int width = (int)Math.Ceiling(image.Width / scale), height = (int)Math.Ceiling(image.Height / scale);
        var pixels = new RgbColor[width * height];
        var valid = new bool[pixels.Length];
        var sourceEdges = new byte[pixels.Length];
        for (int y = 0; y < height; y++)
        {
            token.ThrowIfCancellationRequested();
            for (int x = 0; x < width; x++)
            {
                int px = Math.Min(image.Width - 1, (int)((x + .5) * scale));
                int py = Math.Min(image.Height - 1, (int)((y + .5) * scale));
                if (!image.IsValidPixel(px, py)) continue;
                valid[y * width + x] = true;
                if (image.Rotation is { } rotation)
                {
                    var source = rotation.ToSource(px + .5, py + .5);
                    byte edges = 0;
                    if (source.X < 2 * scale) edges |= 1;
                    if (source.Y < 2 * scale) edges |= 2;
                    if (source.X > rotation.SourceWidth - 2 * scale) edges |= 4;
                    if (source.Y > rotation.SourceHeight - 2 * scale) edges |= 8;
                    sourceEdges[y * width + x] = edges;
                }
                // Symmetric small box average reduces isolated sensor noise, never enlarges evidence.
                int rr = 0, gg = 0, bb = 0, n = 0;
                int radius = scale >= 2 ? 1 : 0;
                for (int yy = Math.Max(0, py - radius); yy <= Math.Min(image.Height - 1, py + radius); yy++)
                for (int xx = Math.Max(0, px - radius); xx <= Math.Min(image.Width - 1, px + radius); xx++)
                { if (!image.IsValidPixel(xx, yy)) continue; var p = image.GetPixel(xx, yy); rr += p.R; gg += p.G; bb += p.B; n++; }
                pixels[y * width + x] = new((byte)(rr / n), (byte)(gg / n), (byte)(bb / n));
            }
        }
        var seen = new bool[pixels.Length]; var queue = new int[pixels.Length];
        var widthChanges = new Dictionary<PixelRect, (double Vertical, double Horizontal)>();
        var candidates = new List<PixelRect>(); var borderRegions = new List<PixelRect>(); bool small = false;
        for (int start = 0; start < pixels.Length; start++)
        {
            if (seen[start] || !valid[start]) continue;
            token.ThrowIfCancellationRequested();
            var seed = pixels[start]; int read = 0, write = 1;
            queue[0] = start; seen[start] = true;
            int left = start % width, right = left, top = start / width, bottom = top;
            bool border = false; byte originalEdges = 0;
            while (read < write)
            {
                int point = queue[read++], x = point % width, y = point / width;
                left = Math.Min(left, x); right = Math.Max(right, x); top = Math.Min(top, y); bottom = Math.Max(bottom, y);
                originalEdges |= sourceEdges[point];
                border |= x == 0 || y == 0 || x == width - 1 || y == height - 1;
                void Visit(int neighbor)
                {
                    if (!valid[neighbor]) { border = true; return; }
                    if (seen[neighbor]) return;
                    var p = pixels[neighbor];
                    if (Math.Max(Math.Abs(p.R - seed.R), Math.Max(Math.Abs(p.G - seed.G), Math.Abs(p.B - seed.B))) > options.RegionTolerance) return;
                    seen[neighbor] = true; queue[write++] = neighbor;
                }
                if (x > 0) Visit(point - 1); if (x + 1 < width) Visit(point + 1);
                if (y > 0) Visit(point - width); if (y + 1 < height) Visit(point + width);
            }
            if (write < 12) continue;
            double fill = write / (double)((right - left + 1) * (bottom - top + 1));
            if (fill < options.MinimumFillRatio) continue;
            int ox = (int)Math.Floor(left * scale), oy = (int)Math.Floor(top * scale);
            int ex = Math.Min(image.Width, (int)Math.Ceiling((right + 1) * scale)), ey = Math.Min(image.Height, (int)Math.Ceiling((bottom + 1) * scale));
            var rect = new PixelRect(ox, oy, ex - ox, ey - oy);
            if (border)
            {
                if (image.Rotation != null && ((originalEdges & 5) == 5 || (originalEdges & 10) == 10)) continue;
                // Cropped color fields must not become a supposedly free wall reference.
                // A wall surrounding the strip spans both image axes; retain only bounded edge regions.
                if (!(left == 0 && right == width - 1) && !(top == 0 && bottom == height - 1)) borderRegions.Add(rect);
                continue;
            }
            // Small cropped fragments still carry evidence of an incomplete strip.
            // Apply measurement-size limits only after retaining border evidence.
            if (Math.Min(rect.Width, rect.Height) < options.MinimumFieldSide || rect.Area < options.MinimumFieldArea)
            { if (rect.Area > 200) small = true; continue; }
            widthChanges[rect] = (
                ComponentWidthChange(queue, write, width, top, bottom, true),
                ComponentWidthChange(queue, write, width, left, right, false));
            candidates.Add(rect);
            if (candidates.Count > 256) return new([], true, small, "unknown");
        }

        var groups = new List<(List<PixelRect> Fields, bool Vertical)>();
        foreach (bool vertical in new[] { true, false })
        {
            var ordered = candidates.OrderBy(r => vertical ? r.Y : r.X).ToArray();
            var parents = Enumerable.Range(0, ordered.Length).ToArray();
            int Root(int i) { while (parents[i] != i) { parents[i] = parents[parents[i]]; i = parents[i]; } return i; }
            for (int a = 0; a < ordered.Length; a++)
            for (int b = a + 1; b < ordered.Length; b++)
            {
                var first = ordered[a]; var second = ordered[b];
                double crossA = vertical ? first.Width : first.Height, crossB = vertical ? second.Width : second.Height;
                double centerA = vertical ? first.X + first.Width / 2d : first.Y + first.Height / 2d;
                double centerB = vertical ? second.X + second.Width / 2d : second.Y + second.Height / 2d;
                int gap = vertical ? second.Y - first.Bottom : second.X - first.Right;
                if (gap >= -2 && gap <= Math.Max(crossA, crossB) * .85 && Math.Min(crossA, crossB) / Math.Max(crossA, crossB) >= .70
                    && Math.Abs(centerA - centerB) <= Math.Min(crossA, crossB) * .13)
                    parents[Root(b)] = Root(a);
            }
            foreach (var set in Enumerable.Range(0, ordered.Length).GroupBy(Root))
            {
                var fields = set.Select(i => ordered[i]).ToList();
                if (fields.Count >= 2) groups.Add((fields, vertical));
            }
        }
        if (groups.Count > 1) return new([], true, small, "unknown");
        if (groups.Count == 1)
        {
            var group = groups[0];
            var inconsistent = new List<PixelRect>();
            // A connected foreign rectangle can join the strip even though only one edge aligns.
            // Require agreement with BOTH transverse boundaries of a majority of intact fields.
            // Field heights/lengths and colors are deliberately not compared.
            if (group.Fields.Count >= 3)
            {
                double Median(IEnumerable<int> values) { var sorted = values.Order().ToArray(); return sorted[sorted.Length / 2]; }
                double start = Median(group.Fields.Select(r => group.Vertical ? r.X : r.Y));
                double end = Median(group.Fields.Select(r => group.Vertical ? r.Right : r.Bottom));
                double tolerance = Math.Max(2 * scale, (end - start) * options.MaximumCrossEdgeDeviation);
                bool Consistent(PixelRect r) =>
                    Math.Abs((group.Vertical ? r.X : r.Y) - start) <= tolerance &&
                    Math.Abs((group.Vertical ? r.Right : r.Bottom) - end) <= tolerance;
                if (group.Fields.Count(Consistent) > group.Fields.Count / 2)
                    inconsistent.AddRange(group.Fields.Where(r => !Consistent(r)));
            }
            return new(group.Fields.OrderBy(r => group.Vertical ? r.Y : r.X).ToArray(), false, small,
                group.Vertical ? "vertical" : "horizontal", borderRegions, inconsistent, widthChanges);
        }
        return candidates.Count == 1 ? new(candidates, false, small, "single", borderRegions) : new([], candidates.Count > 1, small, "unknown");
    }
}

