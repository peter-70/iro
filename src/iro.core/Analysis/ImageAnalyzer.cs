namespace Iro.Core.Analysis;

public sealed class ImageAnalyzer : IImageAnalyzer
{
    public const string Version = "0.5.15";
    public ImageAnalysis Analyze(RgbFrame image, AnalysisOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(image); ArgumentNullException.ThrowIfNull(options); options.Validate();
        cancellationToken.ThrowIfCancellationRequested();
        var source = image;
        var alignment = options.Straighten ? ImageStraightener.Estimate(image, cancellationToken) : new AlignmentEstimate(0, 0, false);
        ImageRotation? rotation = alignment.Degrees == 0 ? null : new(image.Width, image.Height, alignment.Degrees);
        if (rotation != null) image = new RgbFrame(source, rotation);
        var detection = StripDetector.Detect(image, options, cancellationToken);
        bool competingPattern = false;
        // Straightening must not hide another already recognizable strip direction.
        // Compare geometry only; both detections use the same original pixels.
        if (rotation != null && !detection.Ambiguous && detection.Fields.Count > 0)
        {
            var originalDetection = StripDetector.Detect(source, options, cancellationToken);
            int outside = originalDetection.Fields.Count(field =>
            {
                var center = rotation.ToAligned(field.X + field.Width / 2d, field.Y + field.Height / 2d);
                return !detection.Fields.Any(aligned => center.X >= aligned.X && center.X < aligned.Right
                    && center.Y >= aligned.Y && center.Y < aligned.Bottom);
            });
            if (originalDetection.Ambiguous || outside >= 2)
                competingPattern = true;
        }
        var diagnostics = new List<string>
        {
            "Experimenteller Einzelbild-Testweg; keine Kamera- oder zeitliche Stabilitätsfreigabe.",
            "Ganzbildsuche; zusammenhängende Farbregionen und Streifengeometrie. Keine Generator-Sollwerte als Analyseinput.",
            "Gleichmäßige Beleuchtungsverschiebungen und materialabhängige Reflexe sind nicht allgemein aus einem Bild erkennbar."
        };
        diagnostics.Add($"Gerade richten: {-alignment.Degrees:F1}°; Winkelzuversicht {alignment.Confidence:F2}; Messung aus Originalpixeln.");
        if (alignment.Ambiguous) diagnostics.Add("Keine eindeutige Kantenausrichtung; Bild unverändert analysiert.");
        if (detection.Ambiguous) return Finish(AnalysisStatus.AmbiguousPattern, "Mehrere mögliche Muster. Nur den gewünschten Streifen ins Bild nehmen.", []);
        if (detection.Fields.Count == 0) return Finish(detection.HadSmallRegions ? AnalysisStatus.UnsuitableGeometry : AnalysisStatus.NoPattern,
            detection.HadSmallRegions ? "Muster zu klein oder unvollständig. Abstand und Ausschnitt prüfen." : "Vergleichsmuster nicht erkannt.", []);

        PixelRect? admittedCrop = null;
        const string cropHint = "Angeschnittener Streifen: sichere Auswertung nicht gewährleistet. Muster vollständig ins Bild nehmen.";
        if (MeasurementSafety.HasCroppedContinuation(detection))
        {
            if (rotation != null || !MeasurementSafety.TryGetSingleCropCandidate(detection, image.Width, image.Height, options, out var crop))
                return Finish(AnalysisStatus.UnsuitableGeometry, cropHint, []);
            admittedCrop = crop;
            detection = detection with { Fields = detection.Fields.Append(crop)
                .OrderBy(b => detection.Orientation == "vertical" ? b.Y : b.X).ToArray() };
            diagnostics.Add("Ein geometrisch eindeutiger Endbeschnitt wird auf Originalpixel-Eignung geprüft; nur sichtbare auswertbare Felder vergleichen.");
        }

        if (MeasurementSafety.HasStrongCoherentTaper(detection))
            return Finish(AnalysisStatus.UnsuitableGeometry, "Streifen verjüngt sich deutlich. Kamera möglichst frontal auf Muster und Wand ausrichten.", [], AnalysisHintCode.PerspectiveTaper);

        var combined = Union(detection.Fields);
        var exclusions = detection.Fields.Concat(detection.BorderRegions ?? []).Concat(detection.WeakBorderRegions).ToArray();
        RegionMeasurement? shared = null;
        if (options.ReferenceMode == ReferenceMode.SharedAutomaticTrial)
        {
            var referenceBounds = FindReference(image, combined, exclusions, detection.Orientation);
            if (referenceBounds != null) shared = RegionSampler.Measure(image, referenceBounds.Value, options, cancellationToken);
        }
        // A geometrically identified foreign surface is not a valid color field.
        // Any field overlapped by it is also unavailable; neither may establish
        // spatial color variation for intact fields. Blur and channel checks remain global.
        var occluded = detection.Fields.Where(field => detection.InconsistentFields?.Any(foreign =>
            foreign == field || foreign.Intersects(field)) == true).ToHashSet();
        var fields = new List<FieldAnalysis>();
        bool hasUnusableBlur = false, unsafeCrop = false;
        foreach (var bounds in detection.Fields)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inner = bounds.Inset(options.InnerMargin);
            detection.SurfacePolygons.TryGetValue(bounds, out var contour);
            var measurement = RegionSampler.Measure(image, inner, options, cancellationToken, contour);
            // Insets protect the color mean from print/borders, but must not hide a visible
            // illumination gradient across the larger detected field.
            var surface = RegionSampler.Measure(image, bounds.Inset(options.SurfaceMargin), options, cancellationToken, contour);
            if (measurement.IsUsable && surface.SpatialDeltaE > options.MaximumSpatialDeltaE)
                measurement = measurement with { IsUsable = false, Lab = null,
                    Reason = "Farbfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden." };
            if (occluded.Contains(bounds))
                measurement = measurement with { IsUsable = false, Lab = null,
                    Reason = "Feldgrenzen passen nicht zum Streifen. Mögliche Verdeckung; Muster vollständig sichtbar halten." };
            if (HasBlurredEdges(image, bounds, options.MinimumEdgeConcentration))
            {
                hasUnusableBlur = true;
                measurement = measurement with { IsUsable = false, Lab = null, Reason = MeasurementSafety.BlurHint };
            }
            RegionMeasurement? reference = shared;
            if (options.ReferenceMode == ReferenceMode.AdjacentPerFieldTrial)
            {
                var local = FindReference(image, bounds, exclusions, detection.Orientation);
                if (local != null) reference = RegionSampler.Measure(image, local.Value, options, cancellationToken);
            }
            bool allowed = measurement.IsUsable && reference?.IsUsable == true;
            if (admittedCrop == bounds && (!allowed || !surface.IsUsable)) unsafeCrop = true;
            string? hint = !measurement.IsUsable ? measurement.Reason : reference == null ? "Zu wenig freie Wandfläche. Muster und Wand vollständig ins Bild nehmen." :
                !reference.IsUsable ? reference.Reason ?? "Referenzbereich auf eine ausreichend große, gleichmäßige Wandfläche setzen." : measurement.Reason;
            fields.Add(new($"detected-{fields.Count + 1}", bounds, inner, measurement, reference,
                allowed ? ColorMath.DeltaE00(measurement.Lab!.Value, reference!.Lab!.Value) : null, allowed, hint, false) { InnerPolygon = measurement.Polygon, SurfaceSpatialDeltaE = surface.SpatialDeltaE });
        }
        // Frame-wide safety decisions precede ranking and every published value.
        bool unevenSurface = fields.Any(f => (!occluded.Contains(f.Bounds) && (f.Measurement.SpatialDeltaE > options.MaximumSpatialDeltaE
            || f.SurfaceSpatialDeltaE > options.MaximumSpatialDeltaE)) || f.Reference?.SpatialDeltaE > options.MaximumSpatialDeltaE);
        bool channelLimit = fields.Any(f => f.Measurement.HasUnresolvedChannels || f.Reference?.HasUnresolvedChannels == true);
        // Preserve the existing rejection precedence; classify only measured evidence.
        AnalysisHintCode? frameCode = channelLimit ? AnalysisHintCode.ChannelLimit
            : hasUnusableBlur ? AnalysisHintCode.UnusableBlur
            : unevenSurface ? AnalysisHintCode.UnevenSurface : null;
        string? frameFailure = channelLimit
            ? MeasurementSafety.ChannelLimitHint : hasUnusableBlur ? MeasurementSafety.BlurHint
            : unevenSurface ? MeasurementSafety.UnevenSurfaceHint : unsafeCrop ? cropHint : null;
        if (frameFailure != null)
        {
            for (int i = 0; i < fields.Count; i++) fields[i] = fields[i] with
            {
                MeasurementAllowed = false, DeltaE00 = null, IsNearest = false, Hint = frameFailure,
                Measurement = fields[i].Measurement with { IsUsable = false, Lab = null }
            };
            diagnostics.Add("Aufnahmeweite Messsperre: " + frameFailure);
            return Finish(fields.Any(f => f.Reference?.HasUnresolvedChannels == true || (frameFailure == MeasurementSafety.UnevenSurfaceHint && f.Reference?.SpatialDeltaE > options.MaximumSpatialDeltaE))
                ? AnalysisStatus.InvalidReference : AnalysisStatus.InvalidFields, frameFailure, fields, frameCode);
        }
        if (competingPattern)
            return Finish(AnalysisStatus.AmbiguousPattern, "Mehrere mögliche Muster. Nur den gewünschten Streifen ins Bild nehmen.", []);
        int valid = fields.Count(f => f.MeasurementAllowed);
        if (valid > 0)
        {
            // No guessed 'similar enough' threshold. Exact ties are represented as multiple minima.
            double minimum = fields.Where(f => f.DeltaE00.HasValue).Min(f => f.DeltaE00!.Value);
            for (int i = 0; i < fields.Count; i++) fields[i] = fields[i] with { IsNearest = fields[i].DeltaE00 == minimum };
        }
        bool missingReference = fields.Any(f => f.Measurement.IsUsable) && fields.All(f => f.Reference?.IsUsable != true);
        return Finish(valid == fields.Count && admittedCrop == null ? AnalysisStatus.Measured : valid > 0 ? AnalysisStatus.PartiallyMeasured : missingReference ? AnalysisStatus.InvalidReference : AnalysisStatus.InvalidFields,
            valid == fields.Count && admittedCrop == null ? "Farbabstand ΔE00 · kleiner = ähnlicher" : valid > 0 ? $"{(admittedCrop != null ? "Angeschnittener Streifen. " : "")}{valid} von {fields.Count} erkannten Feldern auswertbar. Vergleich und ähnlichster Treffer gelten nur für die auswertbaren sichtbaren Felder; kein vollständiger Streifenvergleich." : fields[0].Hint ?? "Messung nicht möglich.", fields);

        ImageAnalysis Finish(AnalysisStatus status, string hint, IReadOnlyList<FieldAnalysis> results, AnalysisHintCode? hintCode = null)
        {
            RegionMeasurement? Map(RegionMeasurement? region) => region == null || rotation == null ? region
                : region with { Bounds = rotation.SourceBounds(region.Bounds), Polygon = region.Polygon == null ? rotation.ToSource(region.Bounds) : region.Polygon.Select(p => rotation.ToSource(p.X, p.Y)).ToArray() };
            var mapped = rotation == null ? results : results.Select(f => f with
            {
                Bounds = rotation.SourceBounds(f.Bounds), InnerBounds = rotation.SourceBounds(f.InnerBounds),
                Polygon = rotation.ToSource(f.Bounds), InnerPolygon = f.InnerPolygon == null ? rotation.ToSource(f.InnerBounds) : f.InnerPolygon.Select(p => rotation.ToSource(p.X, p.Y)).ToArray(),
                Measurement = Map(f.Measurement)!, Reference = Map(f.Reference)
            }).ToArray();
            return new(Version, options, source.Width, source.Height, status, hint, mapped, diagnostics)
                { StraighteningDegrees = -alignment.Degrees, HintCode = hintCode ?? (status == AnalysisStatus.Measured ? AnalysisHintCode.None : AnalysisHintCode.Other) };
        }
    }
    private static PixelRect Union(IReadOnlyList<PixelRect> fields)
    {
        int x = fields.Min(r => r.X), y = fields.Min(r => r.Y);
        return new(x, y, fields.Max(r => r.Right) - x, fields.Max(r => r.Bottom) - y);
    }

    private static PixelRect? FindReference(RgbFrame image, PixelRect target, IReadOnlyList<PixelRect> exclusions, string orientation)
    {
        int side = Math.Max(24, (int)(Math.Min(image.Width, image.Height) * .15));
        int gap = Math.Max(6, (int)(Math.Min(image.Width, image.Height) * .03));
        int cx = target.X + target.Width / 2, cy = target.Y + target.Height / 2;
        PixelRect[] choices = [new(target.X - gap - side, cy - side / 2, side, side), new(target.Right + gap, cy - side / 2, side, side),
            new(cx - side / 2, target.Y - gap - side, side, side), new(cx - side / 2, target.Bottom + gap, side, side)];
        // Geometry only: never choose the brightest or darkest patch to bias the result.
        return choices.Where((r, i) => orientation == "single" || (orientation == "vertical" ? i < 2 : i >= 2)).Where(r => r.X >= 0 && r.Y >= 0 && r.Right <= image.Width && r.Bottom <= image.Height && image.ContainsArea(r) && !exclusions.Any(r.Intersects))
            .OrderBy(r => Math.Pow(r.X + side / 2d - cx, 2) + Math.Pow(r.Y + side / 2d - cy, 2))
            .Select(r => (PixelRect?)r).FirstOrDefault();
    }

    private static bool HasBlurredEdges(RgbFrame image, PixelRect bounds, double minimumConcentration)
    {
        int poor = 0;
        foreach (var edge in new[] { (bounds.X, bounds.Y + bounds.Height / 2, true), (bounds.Right - 1, bounds.Y + bounds.Height / 2, true),
            (bounds.X + bounds.Width / 2, bounds.Y, false), (bounds.X + bounds.Width / 2, bounds.Bottom - 1, false) })
        {
            double maxStep = 0, range = 0; var colors = new List<(double R, double G, double B)>();
            for (int d = -12; d <= 12; d++)
            {
                int x = edge.Item1 + (edge.Item3 ? d : 0), y = edge.Item2 + (edge.Item3 ? 0 : d);
                // Average parallel to the boundary, never across it. A single noisy
                // pixel must not make a broad smeared transition look sharp.
                // This is a quality statistic only; measurement still uses original pixels.
                int halfBand = Math.Min(8, (edge.Item3 ? bounds.Height : bounds.Width) / 6);
                double r = 0, g = 0, b = 0; int count = 0;
                for (int along = -halfBand; along <= halfBand; along++)
                {
                    int sx = x + (edge.Item3 ? 0 : along), sy = y + (edge.Item3 ? along : 0);
                    if (!image.IsValidPixel(sx, sy)) continue;
                    var pixel = image.GetPixel(sx, sy);
                    r += pixel.R; g += pixel.G; b += pixel.B; count++;
                }
                if (count > 0) colors.Add((r / count, g / count, b / count));
            }
            for (int i = 0; i < colors.Count; i++)
            for (int j = i + 1; j < colors.Count; j++)
            {
                double difference = Math.Max(Math.Abs(colors[i].R - colors[j].R), Math.Max(Math.Abs(colors[i].G - colors[j].G), Math.Abs(colors[i].B - colors[j].B)));
                range = Math.Max(range, difference); if (j == i + 1) maxStep = Math.Max(maxStep, difference);
            }
            if (range >= 10 && maxStep / range < minimumConcentration) poor++;
        }
        // A clearly smeared boundary already contaminates the axis-aligned interior
        // after straightening. Requiring two poor sides admitted narrow fields whose
        // second blurred boundary was outside the short sampling segment.
        return poor >= 1;
    }
}


