using System.Text.Json.Serialization;
using Iro.Core.Analysis;

namespace Iro.Analysis;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record FieldExpectation
{
    [JsonRequired] public string FieldId { get; init; } = "";
    [JsonRequired] public bool MeasurementAllowed { get; init; }
    public double MinimumIntersectionOverUnion { get; init; } = .9;
    public double? DeltaE00 { get; init; }
    public double? AbsoluteTolerance { get; init; }
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(FieldId) || FieldId.Length > 150
            || !double.IsFinite(MinimumIntersectionOverUnion) || MinimumIntersectionOverUnion is < .45 or > 1
            || DeltaE00.HasValue != AbsoluteTolerance.HasValue
            || (DeltaE00 is double value && (!MeasurementAllowed || !double.IsFinite(value) || value < 0))
            || (AbsoluteTolerance is double tolerance && (!double.IsFinite(tolerance) || tolerance < 0)))
            throw new ArgumentException("Ungültige Felderwartung: Feldname, Freigabe, Überdeckung oder Messwert mit Toleranz prüfen.");
    }
    [JsonIgnore] public string Description => $"{FieldId}: " + (MeasurementAllowed ? "Freigabe" : "keine Freigabe")
        + $", Überdeckung mindestens {MinimumIntersectionOverUnion:P0}"
        + (DeltaE00 is double value ? $", geprüftes Soll ΔE00 {value:0.####} ± {AbsoluteTolerance:0.####}" : ", keine numerische Erwartung");
}

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BehaviorExpectation
{
    public int FormatVersion { get; init; } = 1;
    public string Verification { get; init; } = "proposed";
    public string? Basis { get; init; }
    public bool? MeasurementAllowed { get; init; }
    public int? ReleasedFieldCount { get; init; }
    public AnalysisHintCode? RequiredHint { get; init; }
    public AnalysisHintCode? ForbiddenHint { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<FieldExpectation>? Fields { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RankingExpectation? Ranking { get; init; }

    public void Validate()
    {
        if (FormatVersion is not (1 or 2 or 3) || Verification is not ("proposed" or "verified")
            || (Verification == "verified" && string.IsNullOrWhiteSpace(Basis))
            || ReleasedFieldCount is < 0 or > 20
            || (RequiredHint.HasValue && !Enum.IsDefined(RequiredHint.Value))
            || (ForbiddenHint.HasValue && !Enum.IsDefined(ForbiddenHint.Value))
            || (RequiredHint.HasValue && RequiredHint == ForbiddenHint)
            || (MeasurementAllowed == false && ReleasedFieldCount > 0)
            || (MeasurementAllowed == true && ReleasedFieldCount == 0)
            || (FormatVersion == 1 && Fields != null)
            || (FormatVersion < 3 && Ranking != null)
            || (FormatVersion == 3 && Ranking == null)
            || (FormatVersion >= 2 && Fields is not { Count: >= 1 and <= 20 })
            || (MeasurementAllowed == null && ReleasedFieldCount == null && RequiredHint == null && ForbiddenHint == null && Fields == null))
            throw new ArgumentException("Ungültige Verhaltenserwartung: Version, Grundlage, Freigabe, Feldanzahl oder Hinweise prüfen.");
        if (Fields != null)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (var field in Fields)
            {
                if (field == null || !ids.Add(field.FieldId)) throw new ArgumentException("Felderwartungen fehlen oder enthalten doppelte Feldnamen.");
                field.Validate();
            }
            int released = Fields.Count(f => f.MeasurementAllowed);
            if ((ReleasedFieldCount.HasValue && ReleasedFieldCount != released)
                || (MeasurementAllowed.HasValue && MeasurementAllowed != (released > 0)))
                throw new ArgumentException("Bildbezogene und feldbezogene Freigabeerwartungen widersprechen sich.");
            Ranking?.Validate(Fields);
        }
    }
}

public enum ExpectationStatus { NotEvaluated, Passed, Failed, Error }
public sealed record ExpectationEvaluation(ExpectationStatus Status, string Details)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RankingEvaluation? Ranking { get; init; }
}

public static class ExpectationEvaluator
{
    public static ExpectationEvaluation Evaluate(BehaviorExpectation? expected, ImageAnalysis? actual, string? error = null,
        IReadOnlyList<FieldComparison>? comparisons = null)
    {
        if (error != null) return new(ExpectationStatus.Error, "Prüfung nicht möglich: " + error);
        if (expected == null) return new(ExpectationStatus.NotEvaluated, "Keine Verhaltenserwartung hinterlegt.");
        try { expected.Validate(); }
        catch (ArgumentException e) { return new(ExpectationStatus.Error, e.Message); }
        if (expected.Verification != "verified")
            return new(ExpectationStatus.NotEvaluated, "Erwartung ist noch nicht geprüft.");
        if (actual == null) return new(ExpectationStatus.Error, "Kein Analyseergebnis vorhanden.");
        int count = actual.Fields.Count(f => f.MeasurementAllowed);
        var failures = new List<string>();
        var checks = new List<string>();
        var assigned = new Dictionary<string, FieldAnalysis>();
        if (expected.MeasurementAllowed is bool allowed && allowed != (count > 0))
            failures.Add(allowed ? "Messfreigabe erwartet; kein Feld freigegeben."
                : $"Vollständige Sperre erwartet; tatsächlich {count} Felder freigegeben.");
        if (expected.ReleasedFieldCount is int wanted && wanted != count)
            failures.Add($"Erwartet: {wanted} freigegebene Felder; tatsächlich: {count}.");
        bool missingReason = (expected.RequiredHint != null || expected.ForbiddenHint != null) && actual.HintCode == null;
        if (!missingReason)
        {
            if (expected.RequiredHint is { } required && actual.HintCode != required)
                failures.Add($"Erforderlicher Hinweis fehlt: {HintText(required)}.");
            if (expected.ForbiddenHint is { } forbidden && actual.HintCode == forbidden)
                failures.Add($"Unerwarteter Hinweis: {HintText(forbidden)}.");
        }
        bool missingAssignments = expected.Fields != null && comparisons == null;
        if (expected.Fields is { } fields && comparisons != null)
        {
            // Geometry comes from the test metadata AFTER analysis. Nominal distances
            // and their differences are deliberately never read for these assertions.
            if (comparisons.Count != fields.Count || comparisons.Select(c => c.ExpectedFieldId).Distinct().Count() != comparisons.Count
                || fields.Any(f => !comparisons.Any(c => c.ExpectedFieldId == f.FieldId)))
                return new(ExpectationStatus.Error, "Felderwartung und geometrische Testbeschreibung müssen dieselben eindeutigen Felder vollständig enthalten.");
            if (actual.Fields.Select(f => f.FieldId).Distinct().Count() != actual.Fields.Count)
                return new(ExpectationStatus.Error, "Analyse enthält doppelte Feldnamen; Zuordnung nicht prüfbar.");
            var used = new HashSet<string>();
            foreach (var field in fields)
            {
                var mapping = comparisons.Single(c => c.ExpectedFieldId == field.FieldId);
                if (mapping.Status == "ambiguous-assignment")
                { failures.Add($"{field.FieldId}: Feldzuordnung mehrdeutig."); continue; }
                if (mapping.DetectedFieldId == null)
                {
                    if (field.MeasurementAllowed) failures.Add($"{field.FieldId}: erwartetes messbares Feld nicht erkannt.");
                    else checks.Add($"{field.FieldId}: keine Freigabe, wie erwartet (nicht erkannt).");
                    continue;
                }
                if (!used.Add(mapping.DetectedFieldId))
                { failures.Add($"{field.FieldId}: dieselbe erkannte Fläche wurde mehrfach zugeordnet."); continue; }
                var measured = actual.Fields.SingleOrDefault(f => f.FieldId == mapping.DetectedFieldId);
                if (measured == null || mapping.IntersectionOverUnion is not double overlap || !double.IsFinite(overlap)
                    || overlap < field.MinimumIntersectionOverUnion || overlap > 1)
                { failures.Add($"{field.FieldId}: keine ausreichend gesicherte geometrische Zuordnung (gefordert {field.MinimumIntersectionOverUnion:P0})."); continue; }
                assigned.Add(field.FieldId, measured);
                if (measured.MeasurementAllowed != field.MeasurementAllowed)
                    failures.Add($"{field.FieldId}: erwartet {(field.MeasurementAllowed ? "freigegeben" : "gesperrt")}, tatsächlich {(measured.MeasurementAllowed ? "freigegeben" : "gesperrt")}.");
                else checks.Add($"{field.FieldId}: Zuordnung und {(field.MeasurementAllowed ? "Freigabe" : "Sperre")} erfüllt.");
                if (field.DeltaE00 is double target)
                {
                    bool valid = measured.MeasurementAllowed && measured.DeltaE00 is double value && double.IsFinite(value)
                        && value >= 0 && Math.Abs(value - target) <= field.AbsoluteTolerance!.Value;
                    string text = $"{field.FieldId}: geprüftes Soll ΔE00 {target:0.####} ± {field.AbsoluteTolerance:0.####}, Ist {measured.DeltaE00?.ToString("0.####") ?? "fehlt"}: {(valid ? "erfüllt" : "NICHT erfüllt")}.";
                    (valid ? checks : failures).Add(text);
                }
            }
            foreach (var extra in actual.Fields.Where(f => f.MeasurementAllowed && !used.Contains(f.FieldId)))
                failures.Add($"Unerwartete freigegebene Fläche: {extra.FieldId}.");
        }
        var ranking = expected.Ranking == null ? null : RankingEvaluator.Evaluate(expected.Ranking, actual, assigned);
        if (ranking?.Status == ExpectationStatus.Failed) failures.Add(ranking.Details);
        else if (ranking != null) checks.Add(ranking.Details);
        if (failures.Count > 0) return new(ExpectationStatus.Failed, string.Join(" ", failures.Concat(checks))) { Ranking = ranking };
        if (missingReason || missingAssignments || ranking?.Status == ExpectationStatus.NotEvaluated) return new(ExpectationStatus.NotEvaluated,
            "Prüfung unvollständig: " + (missingReason ? "fachlicher Hinweiscode fehlt. " : "") + (missingAssignments ? "Feldzuordnungen fehlen." : "") + (ranking?.Status == ExpectationStatus.NotEvaluated ? ranking.Details : "")) { Ranking = ranking };
        string scope = expected.Fields?.Any(f => f.DeltaE00.HasValue) == true
            ? "Numerische Prüfung nur für die ausdrücklich hinterlegten Felder und Toleranzen; keine Gerätegenauigkeitsfreigabe."
            : "Keine Aussage zur Farbgenauigkeit.";
        return new(ExpectationStatus.Passed, $"Alle hinterlegten Verhaltenserwartungen erfüllt; {count} Felder freigegeben. " + string.Join(" ", checks) + " " + scope) { Ranking = ranking };
    }

    public static string HintText(AnalysisHintCode code) => code switch
    {
        AnalysisHintCode.PerspectiveTaper => "Streifen verjüngt sich; frontalere Aufnahme erforderlich",
        AnalysisHintCode.None => "kein Qualitätshinweis",
        AnalysisHintCode.UnusableBlur => "unbrauchbare Unschärfe erkannt; erneut aufnehmen",
        AnalysisHintCode.ChannelLimit => "Farbkanal an der Messgrenze; Beleuchtung und Belichtung prüfen",
        AnalysisHintCode.UnevenSurface => "Messfläche räumlich ungleichmäßig; Ursache nicht bestimmt",
        _ => "anderer Qualitätshinweis"
    };
}
