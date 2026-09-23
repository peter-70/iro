namespace Iro.Core.Analysis;

/// <summary>Convex polygon overlap in original image coordinates, for geometric result assignment.</summary>
public static class PolygonGeometry
{
    public static double Area(IReadOnlyList<PixelPoint> points) => Math.Abs(SignedArea(points));
    private static double SignedArea(IReadOnlyList<PixelPoint> points)
    {
        double sum = 0;
        for (int n = 0; n < points.Count; n++)
        { var a = points[n]; var b = points[(n + 1) % points.Count]; sum += a.X * b.Y - b.X * a.Y; }
        return sum / 2;
    }
    public static PixelPoint[] Rectangle(PixelRect rect) =>
        [new(rect.X, rect.Y), new(rect.Right, rect.Y), new(rect.Right, rect.Bottom), new(rect.X, rect.Bottom)];
    public static PixelPoint[] Clip(IReadOnlyList<PixelPoint> subject, IReadOnlyList<PixelPoint> boundary)
    {
        var polygon = subject.ToList();
        double sign = Math.Sign(SignedArea(boundary));
        if (sign == 0) return [];
        for (int edge = 0; edge < boundary.Count && polygon.Count > 0; edge++)
        {
            var a = boundary[edge]; var b = boundary[(edge + 1) % boundary.Count];
            double Distance(PixelPoint p) => sign * ((b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X));
            var input = polygon; polygon = [];
            var previous = input[^1]; double previousDistance = Distance(previous);
            foreach (var current in input)
            {
                double distance = Distance(current);
                bool inside = distance >= -1e-9, wasInside = previousDistance >= -1e-9;
                if (inside != wasInside)
                {
                    double fraction = previousDistance / (previousDistance - distance);
                    polygon.Add(new(previous.X + fraction * (current.X - previous.X), previous.Y + fraction * (current.Y - previous.Y)));
                }
                if (inside) polygon.Add(current);
                previous = current; previousDistance = distance;
            }
        }
        return polygon.ToArray();
    }
    public static double IntersectionOverUnion(IReadOnlyList<PixelPoint> a, IReadOnlyList<PixelPoint> b, int width, int height)
    {
        var frame = Rectangle(new(0,0,width,height));
        var first = Clip(a, frame); var second = Clip(b, frame);
        double intersection = Area(Clip(first, second)), union = Area(first) + Area(second) - intersection;
        return union > 0 ? Math.Clamp(intersection / union, 0, 1) : 0;
    }
}
