namespace Iro.Core.Analysis;

// Safety policy versioned with ImageAnalyzer.Version. These are conservative 8-bit
// trial limits, not validated camera exposure thresholds. Inspect retained original
// pixels: small excluded print/glints do not automatically invalidate a whole frame.
internal static class MeasurementSafety
{
    internal const double MaximumRetainedEndpointFraction = .02;
    internal const string ChannelLimitHint = "Farbkanal an der Messgrenze. Beleuchtung und Belichtung prüfen; keine zuverlässige Messung möglich.";
    internal const string BlurHint = "Bild unbrauchbar: zu unscharf. Bitte erneut aufnehmen. Kamera ruhig halten und neu fokussieren.";

    internal const string UnevenSurfaceHint = "Messflächen sind zu ungleichmäßig für einen zuverlässigen Vergleich. Bitte für gleichmäßige Beleuchtung und einheitliche Messflächen sorgen und erneut aufnehmen.";

    // Image-space trial criterion, not an estimate of a physical camera angle.
    // A coherent cross-width change is different from varying field lengths or
    // one occluded field. Two regions suffice only when BOTH contours independently support the trend.
    internal const double MaximumCoherentWidthChange = .15;
    internal const double MinimumTaperFit = .90;
    internal const double MinimumWithinFieldWidthChange = .025;
    internal static bool HasStrongCoherentTaper(Detection detection)
    {
        if (detection.Fields.Count < 2 || detection.Orientation == "single") return false;
        bool vertical = detection.Orientation == "vertical";
        var points = detection.Fields.Select(r => (
            Position: vertical ? r.Y + r.Height / 2d : r.X + r.Width / 2d,
            Width: (double)(vertical ? r.Width : r.Height))).OrderBy(p => p.Position).ToArray();
        double meanX = points.Average(p => p.Position), meanY = points.Average(p => p.Width);
        double xx = points.Sum(p => Math.Pow(p.Position - meanX, 2));
        double yy = points.Sum(p => Math.Pow(p.Width - meanY, 2));
        if (xx == 0 || yy == 0) return false;
        double xy = points.Sum(p => (p.Position - meanX) * (p.Width - meanY));
        // With two fields the fit is trivially perfect; the two independent
        // within-field contour checks below remain mandatory.
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
    internal const int MinimumCropSide = 40;
    internal const int MinimumCropArea = 2500;
    internal const int MaximumCropEdgeDeviation = 2;

    // First bounded crop policy: one end remainder, two intact anchors, no
    // synthesized fragments. Quality is still checked on original pixels later.
    internal static bool TryGetSingleCropCandidate(Detection detection, int width, int height,
        AnalysisOptions options, out PixelRect crop)
    {
        crop = default;
        if (detection.Fields.Count < 2 || detection.Orientation is not ("vertical" or "horizontal")
            || detection.InconsistentFields?.Count > 0 || detection.BorderRegions == null) return false;
        bool vertical = detection.Orientation == "vertical";
        var candidates = detection.BorderRegions.Where(border =>
        {
            if (Math.Min(border.Width, border.Height) < Math.Max(MinimumCropSide, options.MinimumFieldSide)
                || border.Area < Math.Max(MinimumCropArea, options.MinimumFieldArea)) return false;
            bool atStart = vertical ? border.Y == 0 && border.X > 0 && border.Right < width
                : border.X == 0 && border.Y > 0 && border.Bottom < height;
            bool atEnd = vertical ? border.Bottom == height && border.X > 0 && border.Right < width
                : border.Right == width && border.Y > 0 && border.Bottom < height;
            if (!atStart && !atEnd) return false;
            var anchor = atStart ? detection.Fields[0] : detection.Fields[^1];
            int gap = vertical ? (atStart ? anchor.Y - border.Bottom : border.Y - anchor.Bottom)
                : (atStart ? anchor.X - border.Right : border.X - anchor.Right);
            int cross = vertical ? anchor.Width : anchor.Height;
            if (gap < 0 || gap > cross * .85) return false;
            // Both transverse edges must agree with BOTH intact end anchors.
            return new[] { detection.Fields[0], detection.Fields[^1] }.All(field => vertical
                ? Math.Abs(field.X - border.X) <= MaximumCropEdgeDeviation && Math.Abs(field.Right - border.Right) <= MaximumCropEdgeDeviation
                : Math.Abs(field.Y - border.Y) <= MaximumCropEdgeDeviation && Math.Abs(field.Bottom - border.Bottom) <= MaximumCropEdgeDeviation);
        }).ToArray();
        if (candidates.Length != 1) return false;
        var candidate = candidates[0];
        // A second suspicious remainder (including fragmented gradients) cannot be ignored.
        if (HasCroppedContinuation(detection with { BorderRegions = detection.BorderRegions.Where(b => b != candidate).ToArray() })) return false;
        crop = candidate;
        return true;
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
        bool AlongOrientation(bool vertical)
        {
            // A gradient can split one cropped field into adjacent color components.
            // Combine geometry only, and only across aligned, touching fragments.
            // No extrapolation across missing pixels and no reconstructed color.
            var ordered = detection.BorderRegions.Concat(detection.WeakBorderRegions).OrderBy(b => vertical ? b.X : b.Y).ToArray();
            PixelRect? combined = null;
            bool MatchesAny(PixelRect border) => detection.Fields.Any(field => Matches(vertical, field, border));
            foreach (var border in ordered)
            {
                if (MatchesAny(border)) return true;
                if (combined is { } current)
                {
                    int crossGap = vertical ? border.X - current.Right : border.Y - current.Bottom;
                    bool aligned = vertical
                        ? Math.Abs(border.Y - current.Y) <= 2 && Math.Abs(border.Bottom - current.Bottom) <= 2
                        : Math.Abs(border.X - current.X) <= 2 && Math.Abs(border.Right - current.Right) <= 2;
                    if (aligned && crossGap <= 2)
                    {
                        int x = Math.Min(current.X, border.X), y = Math.Min(current.Y, border.Y);
                        combined = new(x, y, Math.Max(current.Right, border.Right) - x, Math.Max(current.Bottom, border.Bottom) - y);
                        if (MatchesAny(combined.Value)) return true;
                        continue;
                    }
                }
                combined = border;
            }
            return false;
        }
        return (detection.Orientation != "horizontal" && AlongOrientation(true))
            || (detection.Orientation != "vertical" && AlongOrientation(false));
    }
}
