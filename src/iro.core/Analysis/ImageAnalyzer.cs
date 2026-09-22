namespace Iro.Core.Analysis;

public sealed class ImageAnalyzer : IImageAnalyzer
{
    public const string Version = "0.2.0";
    public ImageAnalysis Analyze(RgbFrame image, AnalysisOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(image); ArgumentNullException.ThrowIfNull(options); options.Validate();
        cancellationToken.ThrowIfCancellationRequested();
        var detection = StripDetector.Detect(image, options, cancellationToken);
        var diagnostics = new List<string>
        {
            "Experimenteller Einzelbild-Testweg; keine Kamera- oder zeitliche Stabilitätsfreigabe.",
            "Ganzbildsuche; zusammenhängende Farbregionen und Streifengeometrie. Keine Generator-Sollwerte als Analyseinput.",
            "Gleichmäßige Beleuchtungsverschiebungen und materialabhängige Reflexe sind nicht allgemein aus einem Bild erkennbar."
        };
        if (detection.Ambiguous) return Finish(AnalysisStatus.AmbiguousPattern, "Mehrere mögliche Muster. Nur den gewünschten Streifen ins Bild nehmen.", []);
        if (detection.Fields.Count == 0) return Finish(detection.HadSmallRegions ? AnalysisStatus.UnsuitableGeometry : AnalysisStatus.NoPattern,
            detection.HadSmallRegions ? "Muster zu klein oder unvollständig. Abstand und Ausschnitt prüfen." : "Vergleichsmuster nicht erkannt.", []);

        var combined = Union(detection.Fields);
        var exclusions = detection.Fields.Concat(detection.BorderRegions ?? []).ToArray();
        RegionMeasurement? shared = null;
        if (options.ReferenceMode == ReferenceMode.SharedAutomaticTrial)
        {
            var referenceBounds = FindReference(image, combined, exclusions, detection.Orientation);
            if (referenceBounds != null) shared = RegionSampler.Measure(image, referenceBounds.Value, options, cancellationToken);
        }
        var fields = new List<FieldAnalysis>();
        foreach (var bounds in detection.Fields)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inner = bounds.Inset(options.InnerMargin);
            var measurement = RegionSampler.Measure(image, inner, options, cancellationToken);
            if (detection.InconsistentFields?.Contains(bounds) == true)
                measurement = measurement with { IsUsable = false, Lab = null,
                    Reason = "Feldgrenzen passen nicht zum Streifen. Mögliche Verdeckung; Muster vollständig sichtbar halten." };
            if (measurement.IsUsable && HasBlurredEdges(image, bounds, options.MinimumEdgeConcentration))
                measurement = measurement with { IsUsable = false, Lab = null, Reason = "Bild unscharf. Kamera ruhig halten und neu fokussieren." };
            RegionMeasurement? reference = shared;
            if (options.ReferenceMode == ReferenceMode.AdjacentPerFieldTrial)
            {
                var local = FindReference(image, bounds, exclusions, detection.Orientation);
                if (local != null) reference = RegionSampler.Measure(image, local.Value, options, cancellationToken);
            }
            bool allowed = measurement.IsUsable && reference?.IsUsable == true;
            string? hint = reference == null ? "Zu wenig freie Wandfläche. Muster und Wand vollständig ins Bild nehmen." :
                !reference.IsUsable ? "Referenzbereich auf eine ausreichend große, gleichmäßige Wandfläche setzen." : measurement.Reason;
            fields.Add(new($"detected-{fields.Count + 1}", bounds, inner, measurement, reference,
                allowed ? ColorMath.DeltaE00(measurement.Lab!.Value, reference!.Lab!.Value) : null, allowed, hint, false));
        }
        int valid = fields.Count(f => f.MeasurementAllowed);
        if (valid > 0)
        {
            // No guessed 'similar enough' threshold. Exact ties are represented as multiple minima.
            double minimum = fields.Where(f => f.DeltaE00.HasValue).Min(f => f.DeltaE00!.Value);
            for (int i = 0; i < fields.Count; i++) fields[i] = fields[i] with { IsNearest = fields[i].DeltaE00 == minimum };
        }
        bool missingReference = fields.All(f => f.Reference?.IsUsable != true);
        return Finish(valid == fields.Count ? AnalysisStatus.Measured : valid > 0 ? AnalysisStatus.PartiallyMeasured : missingReference ? AnalysisStatus.InvalidReference : AnalysisStatus.InvalidFields,
            valid == fields.Count ? "Farbabstand ΔE00 · kleiner = ähnlicher" : valid > 0 ? "Einzelne Messflächen ungeeignet; gültige Felder bleiben auswertbar." : fields[0].Hint ?? "Messung nicht möglich.", fields);

        ImageAnalysis Finish(AnalysisStatus status, string hint, IReadOnlyList<FieldAnalysis> results) =>
            new(Version, options, image.Width, image.Height, status, hint, results, diagnostics);
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
        return choices.Where((r, i) => orientation == "single" || (orientation == "vertical" ? i < 2 : i >= 2)).Where(r => r.X >= 0 && r.Y >= 0 && r.Right <= image.Width && r.Bottom <= image.Height && !exclusions.Any(r.Intersects))
            .OrderBy(r => Math.Pow(r.X + side / 2d - cx, 2) + Math.Pow(r.Y + side / 2d - cy, 2))
            .Select(r => (PixelRect?)r).FirstOrDefault();
    }

    private static bool HasBlurredEdges(RgbFrame image, PixelRect bounds, double minimumConcentration)
    {
        int poor = 0;
        foreach (var edge in new[] { (bounds.X, bounds.Y + bounds.Height / 2, true), (bounds.Right - 1, bounds.Y + bounds.Height / 2, true),
            (bounds.X + bounds.Width / 2, bounds.Y, false), (bounds.X + bounds.Width / 2, bounds.Bottom - 1, false) })
        {
            double maxStep = 0, range = 0; var colors = new List<RgbColor>();
            for (int d = -12; d <= 12; d++)
            {
                int x = edge.Item1 + (edge.Item3 ? d : 0), y = edge.Item2 + (edge.Item3 ? 0 : d);
                if (x >= 0 && y >= 0 && x < image.Width && y < image.Height) colors.Add(image.GetPixel(x, y));
            }
            for (int i = 0; i < colors.Count; i++)
            for (int j = i + 1; j < colors.Count; j++)
            {
                double difference = Math.Max(Math.Abs(colors[i].R - colors[j].R), Math.Max(Math.Abs(colors[i].G - colors[j].G), Math.Abs(colors[i].B - colors[j].B)));
                range = Math.Max(range, difference); if (j == i + 1) maxStep = Math.Max(maxStep, difference);
            }
            if (range >= 10 && maxStep / range < minimumConcentration) poor++;
        }
        return poor >= 2;
    }
}


