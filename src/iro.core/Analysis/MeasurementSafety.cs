namespace Iro.Core.Analysis;

// Safety policy versioned with ImageAnalyzer.Version. These are conservative 8-bit
// trial limits, not validated camera exposure thresholds. Inspect retained original
// pixels: small excluded print/glints do not automatically invalidate a whole frame.
internal static class MeasurementSafety
{
    internal const double MaximumRetainedEndpointFraction = .02;
    internal const string ChannelLimitHint = "Farbkanal an der Messgrenze. Beleuchtung und Belichtung prüfen; keine zuverlässige Messung möglich.";
    internal const string BlurHint = "Bild unscharf. Kamera ruhig halten und neu fokussieren.";

    // Image-space trial criterion, not an estimate of a physical camera angle.
    // A coherent cross-width change is different from varying field lengths or
    // one occluded field. Require at least three regions and a well-supported trend.
    internal const double MaximumCoherentWidthChange = .15;
    internal const double MinimumTaperFit = .90;
    internal const double MinimumWithinFieldWidthChange = .025;
    internal static bool HasStrongCoherentTaper(Detection detection)
    {
        if (detection.Fields.Count < 3 || detection.Orientation == "single") return false;
        bool vertical = detection.Orientation == "vertical";
        var points = detection.Fields.Select(r => (
            Position: vertical ? r.Y + r.Height / 2d : r.X + r.Width / 2d,
            Width: (double)(vertical ? r.Width : r.Height))).OrderBy(p => p.Position).ToArray();
        double meanX = points.Average(p => p.Position), meanY = points.Average(p => p.Width);
        double xx = points.Sum(p => Math.Pow(p.Position - meanX, 2));
        double yy = points.Sum(p => Math.Pow(p.Width - meanY, 2));
        if (xx == 0 || yy == 0) return false;
        double xy = points.Sum(p => (p.Position - meanX) * (p.Width - meanY));
        double fit = xy * xy / (xx * yy);
        double relativeChange = Math.Abs(xy / xx) * (points[^1].Position - points[0].Position) / points.Max(p => p.Width);
        if (fit < MinimumTaperFit || relativeChange <= MaximumCoherentWidthChange
            || detection.WidthChanges == null) return false;
        int supportingContours = detection.Fields.Count(field =>
        {
            if (!detection.WidthChanges.TryGetValue(field, out var changes)) return false;
            double change = vertical ? changes.Vertical : changes.Horizontal;
            return Math.Abs(change) >= MinimumWithinFieldWidthChange && Math.Sign(change) == Math.Sign(xy);
        });
        return supportingContours >= Math.Max(2, (detection.Fields.Count + 1) / 2);
    }
    internal static bool HasCroppedContinuation(Detection detection)
    {
        if (detection.BorderRegions == null || detection.Fields.Count == 0) return false;
        bool Matches(bool vertical, PixelRect field, PixelRect border)
        {
            double crossField = vertical ? field.Width : field.Height;
            double crossBorder = vertical ? border.Width : border.Height;
            double centerField = vertical ? field.X + field.Width / 2d : field.Y + field.Height / 2d;
            double centerBorder = vertical ? border.X + border.Width / 2d : border.Y + border.Height / 2d;
            int gap = vertical ? Math.Max(border.Y - field.Bottom, field.Y - border.Bottom)
                : Math.Max(border.X - field.Right, field.X - border.Right);
            return gap >= -2 && gap <= Math.Max(crossField, crossBorder) * .85
                && Math.Min(crossField, crossBorder) / Math.Max(crossField, crossBorder) >= .70
                && Math.Abs(centerField - centerBorder) <= Math.Min(crossField, crossBorder) * .13;
        }
        return detection.BorderRegions.Any(border => detection.Fields.Any(field =>
            (detection.Orientation != "horizontal" && Matches(true, field, border)) ||
            (detection.Orientation != "vertical" && Matches(false, field, border))));
    }
}