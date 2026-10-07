namespace Iro.Core.Analysis;

public sealed class ImageAnalyzer : IImageAnalyzer
{
    public const string Version = "0.5.15";
    public ImageAnalysis Analyze(RgbFrame image, AnalysisOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(image); ArgumentNullException.ThrowIfNull(options); options.Validate();
        cancellationToken.ThrowIfCancellationRequested();
        var source = image;
        var uncertainRegions = new List<UncertainRegion>();
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
        if (detection.Ambiguous) return Finish(AnalysisStatus.AmbiguousPattern, "Das Vergleichsmuster konnte nicht eindeutig zugeordnet werden. Es werden keine Farbvergleiche ausgegeben.", []);
        if (detection.Fields.Count == 0) return Finish(detection.HadSmallRegions ? AnalysisStatus.UnsuitableGeometry : AnalysisStatus.NoPattern,
            detection.HadSmallRegions ? "Die erkannten kleinen Farbflächen reichen nicht für einen Farbvergleich aus." : "Vergleichsmuster nicht erkannt.", []);

        PixelRect? admittedCrop = null;
        const string cropHint = "Die Feldgeometrie am Bildrand konnte nicht sicher zugeordnet werden. Es werden keine Farbvergleiche ausgegeben.";
        if (MeasurementSafety.HasCroppedContinuation(detection))
        {
            if (rotation != null) return Finish(AnalysisStatus.UnsuitableGeometry, cropHint, []);
            if (MeasurementSafety.TryGetSingleCropCandidate(detection, image.Width, image.Height, options, out var crop))
            {
                admittedCrop = crop;
                detection = detection with { Fields = detection.Fields.Append(crop)
                    .OrderBy(b => detection.Orientation == "vertical" ? b.Y : b.X).ToArray() };
                diagnostics.Add("Ein geometrisch eindeutiger Endbeschnitt wird auf Originalpixel-Eignung geprüft; nur sichtbare auswertbare Felder vergleichen.");
            }
            else if (MeasurementSafety.TryGetExcludedEndCrop(detection, image.Width, image.Height, options, rotation != null, out var excluded))
            {
                uncertainRegions.Add(new("uncertain-edge-1", excluded,
                    "Unsicherer Randbereich: Feldgrenzen nicht sicher zugeordnet; kein Farbvergleich."));
                diagnostics.Add("Ein lokalisierter Endrest bleibt ungemessen und von der Wandreferenz ausgeschlossen. Referenzwahl aus dem verbleibenden Feldverbund.");
            }
            else return Finish(AnalysisStatus.UnsuitableGeometry, cropHint, []);
        }

        if (MeasurementSafety.HasStrongCoherentTaper(detection))
            diagnostics.Add("Die erkannten Feldbreiten verändern sich entlang des Streifens deutlich; diagnostischer Geometriebefund ohne eigene Messsperre.");

        var combined = Union(detection.Fields);
        var exclusions = detection.Fields.Concat(detection.BorderRegions ?? []).Concat(detection.WeakBorderRegions).ToArray();
        RegionMeasurement? shared = null;
        if (options.ReferenceMode == ReferenceMode.SharedAutomaticTrial)
        {
            var referenceBounds = FindReference(image, combined, exclusions, detection.Orientation);
            if (referenceBounds != null) shared = RegionSampler.Measure(image, referenceBounds.Value, options, cancellationToken);
        }
        // A geometrically identified foreign surface is not a valid color field.
        // Any field overlapped by it is also unavailable. Edge observations are diagnostic only.
        var occluded = detection.Fields.Where(field => detection.InconsistentFields?.Any(foreign =>
            foreign == field || foreign.Intersects(field)) == true).ToHashSet();
        var fields = new List<FieldAnalysis>();
        foreach (var bounds in detection.Fields)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RegionMeasurement? reference = shared;
            if (options.ReferenceMode == ReferenceMode.AdjacentPerFieldTrial)
            {
                var local = FindReference(image, bounds, exclusions, detection.Orientation);
                if (local != null) reference = RegionSampler.Measure(image, local.Value, options, cancellationToken);
            }
            var inner = bounds.Inset(options.InnerMargin);
            detection.SurfacePolygons.TryGetValue(bounds, out var contour);
            var measurement = RegionSampler.Measure(image, inner, options, cancellationToken, contour);
            // Insets protect the color mean from print/borders, but must not hide a visible
            // illumination gradient across the larger detected field.
            var surface = RegionSampler.Measure(image, bounds.Inset(options.SurfaceMargin), options, cancellationToken, contour);
            if (measurement.IsUsable && surface.SpatialDeltaE > options.MaximumSpatialDeltaE)
                measurement = measurement with { IsUsable = false, Lab = null,
                    Reason = "Die Farbfläche weist räumlich unterschiedliche Farbwerte auf. Für dieses Feld wird kein Farbvergleich ausgegeben." };
            // An admitted crop keeps every control-surface quality check, locally.
            if (admittedCrop == bounds && measurement.IsUsable && !surface.IsUsable)
                measurement = measurement with { IsUsable = false, Lab = null, Reason = surface.Reason };
            if (occluded.Contains(bounds))
                measurement = measurement with { IsUsable = false, Lab = null,
                    Reason = "Die Feldgrenzen stimmen nicht mit der erkannten Streifengeometrie überein. Für dieses Feld ist kein Farbvergleich verfügbar." };
            if (HasBlurredEdges(image, bounds, options.MinimumEdgeConcentration))
                diagnostics.Add($"Breiter Farbübergang an mindestens einer Kante von Feld {fields.Count + 1}; diagnostischer Befund ohne eigene Messsperre.");
            bool allowed = measurement.IsUsable && reference?.IsUsable == true;
            string? hint = !measurement.IsUsable ? measurement.Reason : reference == null ? "Für dieses Feld wurde keine geeignete Wandreferenz gefunden. Ein Farbvergleich ist nicht verfügbar." :
                !reference.IsUsable ? reference.Reason ?? "Referenzbereich auf eine ausreichend große, gleichmäßige Wandfläche setzen." : measurement.Reason;
            fields.Add(new($"detected-{fields.Count + 1}", bounds, inner, measurement, reference,
                allowed ? ColorMath.DeltaE00(measurement.Lab!.Value, reference!.Lab!.Value) : null, allowed, hint, false) { InnerPolygon = measurement.Polygon, SurfaceSpatialDeltaE = surface.SpatialDeltaE });
        }
        if (competingPattern)
            return Finish(AnalysisStatus.AmbiguousPattern, "Das Vergleichsmuster konnte nicht eindeutig zugeordnet werden. Es werden keine Farbvergleiche ausgegeben.", []);
        int valid = fields.Count(f => f.MeasurementAllowed);
        if (valid > 0)
        {
            // No guessed 'similar enough' threshold. Exact ties are represented as multiple minima.
            double minimum = fields.Where(f => f.DeltaE00.HasValue).Min(f => f.DeltaE00!.Value);
            for (int i = 0; i < fields.Count; i++) fields[i] = fields[i] with { IsNearest = fields[i].DeltaE00 == minimum };
        }
        bool missingReference = fields.Any(f => f.Measurement.IsUsable) && fields.All(f => f.Reference?.IsUsable != true);
        return Finish(valid == fields.Count && admittedCrop == null && uncertainRegions.Count == 0 ? AnalysisStatus.Measured : valid > 0 ? AnalysisStatus.PartiallyMeasured : missingReference ? AnalysisStatus.InvalidReference : AnalysisStatus.InvalidFields,
            valid == fields.Count && admittedCrop == null && uncertainRegions.Count == 0 ? "Farbabstand ΔE00 · kleiner = ähnlicher" : valid > 0 && uncertainRegions.Count > 0 ? $"{valid} markierte Felder zuverlässig ausgewertet. Ergebnisse und nächstliegendes Feld beziehen sich ausschließlich auf diese Felder. Unsichere Randbereiche bleiben ohne Farbvergleich; die Gesamtzahl der Felder ist unbekannt." : valid > 0 ? $"{(admittedCrop != null ? "Angeschnittener Streifen. " : "")}{valid} von {fields.Count} erkannten Feldern auswertbar. Vergleich und ähnlichster Treffer gelten nur für die auswertbaren sichtbaren Felder; kein vollständiger Streifenvergleich." : fields[0].Hint ?? "Messung nicht möglich.", fields);

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
                { UncertainRegions = uncertainRegions.ToArray(), StraighteningDegrees = -alignment.Degrees, HintCode = hintCode ?? (status == AnalysisStatus.Measured ? AnalysisHintCode.None : AnalysisHintCode.Other) };
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
        // This observes a broad edge transition, not its cause, affected area,
        // or the reliability of the selected field/reference pixels.
        return poor >= 1;
    }
}


