namespace Iro.Core.Analysis;

internal sealed record Detection(IReadOnlyList<PixelRect> Fields, bool Ambiguous, bool HadSmallRegions, string Orientation, IReadOnlyList<PixelRect>? BorderRegions = null, IReadOnlyList<PixelRect>? InconsistentFields = null, IReadOnlyDictionary<PixelRect, (double Vertical, double Horizontal)>? WidthChanges = null)
{
    public IReadOnlyDictionary<PixelRect, IReadOnlyList<PixelPoint>> SurfacePolygons { get; init; } = new Dictionary<PixelRect, IReadOnlyList<PixelPoint>>();
    public IReadOnlyList<PixelRect> WeakBorderRegions { get; init; } = [];
}

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
    // A single noisy start pixel must not classify a large wall as a nearby field.
    // Use a fixed small connected pilot only when it demonstrates a tight color cluster.
    // This is a classification reference, not a modified image or measured color.
    private static RgbColor StableSeed(RgbColor[] pixels, bool[] valid, bool[] seen,
        int width, int height, int start, double tolerance)
    {
        const int pilotSize = 64;
        var original = pixels[start];
        Span<int> pilot = stackalloc int[pilotSize];
        pilot[0] = start;
        int count = 1;
        for (int read = 0; read < count && count < pilotSize; read++)
        {
            int point = pilot[read], x = point % width, y = point / width;
            for (int direction = 0; direction < 4 && count < pilotSize; direction++)
            {
                int neighbor = direction switch
                {
                    0 => x > 0 ? point - 1 : -1,
                    1 => x + 1 < width ? point + 1 : -1,
                    2 => y > 0 ? point - width : -1,
                    _ => y + 1 < height ? point + width : -1
                };
                if (neighbor < 0 || !valid[neighbor] || seen[neighbor] || pilot[..count].Contains(neighbor)) continue;
                var p = pixels[neighbor];
                if (Math.Max(Math.Abs(p.R - original.R), Math.Max(Math.Abs(p.G - original.G), Math.Abs(p.B - original.B))) > tolerance) continue;
                pilot[count++] = neighbor;
            }
        }
        if (count < pilotSize) return original;
        Span<byte> red = stackalloc byte[pilotSize], green = stackalloc byte[pilotSize], blue = stackalloc byte[pilotSize];
        for (int i = 0; i < count; i++)
        {
            var p = pixels[pilot[i]];
            red[i] = p.R; green[i] = p.G; blue[i] = p.B;
        }
        red.Sort(); green.Sort(); blue.Sort();
        // At most +/-2 digital noise around one color. Do not infer a new seed
        // from mixed surfaces, a broad gradient or a strongly noisy pilot.
        if (red[^1] - red[0] > 4 || green[^1] - green[0] > 4 || blue[^1] - blue[0] > 4) return original;
        static byte Mean(ReadOnlySpan<byte> channel)
        {
            int sum = 0; foreach (byte value in channel) sum += value;
            return (byte)((sum + channel.Length / 2) / channel.Length);
        }
        return new(Mean(red), Mean(green), Mean(blue));
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
                // Detection geometry must be derived from unmodified source pixels.
                pixels[y * width + x] = image.GetPixel(px, py);
            }
        }
        var seen = new bool[pixels.Length]; var queue = new int[pixels.Length];
        var surfacePolygons = new Dictionary<PixelRect, IReadOnlyList<PixelPoint>>();
        var widthChanges = new Dictionary<PixelRect, (double Vertical, double Horizontal)>();
        var candidates = new List<PixelRect>(); var borderRegions = new List<PixelRect>(); var weakBorderRegions = new List<PixelRect>(); bool small = false;
        for (int start = 0; start < pixels.Length; start++)
        {
            if (seen[start] || !valid[start]) continue;
            token.ThrowIfCancellationRequested();
            var seed = StableSeed(pixels, valid, seen, width, height, start, options.RegionTolerance); int read = 0, write = 1;
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
            int ox = (int)Math.Floor(left * scale), oy = (int)Math.Floor(top * scale);
            int ex = Math.Min(image.Width, (int)Math.Ceiling((right + 1) * scale)), ey = Math.Min(image.Height, (int)Math.Ceiling((bottom + 1) * scale));
            var rect = new PixelRect(ox, oy, ex - ox, ey - oy);
            if (border)
            {
                if (image.Rotation != null && ((originalEdges & 5) == 5 || (originalEdges & 10) == 10)) continue;
                // Cropped color fields must not become a supposedly free wall reference.
                // A wall surrounding the strip spans both image axes; retain only bounded edge regions.
                if (!(left == 0 && right == width - 1) && !(top == 0 && bottom == height - 1)) (fill < options.MinimumFillRatio ? weakBorderRegions : borderRegions).Add(rect);
                continue;
            }
            if (fill < options.MinimumFillRatio) continue;
            // Small cropped fragments still carry evidence of an incomplete strip.
            // Apply measurement-size limits only after retaining border evidence.
            if (Math.Min(rect.Width, rect.Height) < options.MinimumFieldSide || rect.Area < options.MinimumFieldArea)
            { if (rect.Area > 200) small = true; continue; }
            widthChanges[rect] = (
                ComponentWidthChange(queue, write, width, top, bottom, true),
                ComponentWidthChange(queue, write, width, left, right, false));
            var contour = ContourMask.Fit(queue, write, width, left, top, right, bottom, scale);
            if (contour != null) surfacePolygons[rect] = contour;
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
            // A reflection can split one field into a perpendicular row of rectangles.
            // Intact rectangles beside that row support a competing strip direction.
            // Reject the ambiguity instead of measuring the reflection and using an
            // ignored intact field as the wall. No brightness/color inference is made.
            int gx = group.Fields.Min(r => r.X), gy = group.Fields.Min(r => r.Y);
            var envelope = new PixelRect(gx, gy, group.Fields.Max(r => r.Right) - gx, group.Fields.Max(r => r.Bottom) - gy);
            bool competingVertical = !group.Vertical;
            bool CompetingContinuation(PixelRect r)
            {
                if (group.Fields.Contains(r)) return false;
                double crossA = competingVertical ? envelope.Width : envelope.Height;
                double crossB = competingVertical ? r.Width : r.Height;
                double centerA = competingVertical ? envelope.X + envelope.Width / 2d : envelope.Y + envelope.Height / 2d;
                double centerB = competingVertical ? r.X + r.Width / 2d : r.Y + r.Height / 2d;
                int gap = competingVertical ? Math.Max(r.Y - envelope.Bottom, envelope.Y - r.Bottom)
                    : Math.Max(r.X - envelope.Right, envelope.X - r.Right);
                return gap >= -2 && gap <= Math.Max(crossA, crossB) * .85
                    && Math.Min(crossA, crossB) / Math.Max(crossA, crossB) >= .70
                    && Math.Abs(centerA - centerB) <= Math.Min(crossA, crossB) * .13;
            }
            if (candidates.Any(CompetingContinuation)) return new([], true, small, "unknown");
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
                group.Vertical ? "vertical" : "horizontal", borderRegions, inconsistent, widthChanges) { WeakBorderRegions = weakBorderRegions, SurfacePolygons = surfacePolygons };
        }
        return candidates.Count == 1 ? new(candidates, false, small, "single", borderRegions) { WeakBorderRegions = weakBorderRegions, SurfacePolygons = surfacePolygons } : new([], candidates.Count > 1, small, "unknown");
    }
}

