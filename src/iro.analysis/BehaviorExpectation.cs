using System.Text.Json.Serialization;
using Iro.Core.Analysis;

namespace Iro.Analysis;

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

    public void Validate()
    {
        if (FormatVersion != 1 || Verification is not ("proposed" or "verified")
            || (Verification == "verified" && string.IsNullOrWhiteSpace(Basis))
            || ReleasedFieldCount is < 0 or > 20
            || (RequiredHint.HasValue && !Enum.IsDefined(RequiredHint.Value))
            || (ForbiddenHint.HasValue && !Enum.IsDefined(ForbiddenHint.Value))
            || (RequiredHint.HasValue && RequiredHint == ForbiddenHint)
            || (MeasurementAllowed == false && ReleasedFieldCount > 0)
            || (MeasurementAllowed == true && ReleasedFieldCount == 0)
            || (MeasurementAllowed == null && ReleasedFieldCount == null && RequiredHint == null && ForbiddenHint == null))
            throw new ArgumentException("Ungültige Verhaltenserwartung: Version, Grundlage, Freigabe, Feldanzahl oder Hinweise prüfen.");
    }
}

public enum ExpectationStatus { NotEvaluated, Passed, Failed, Error }
public sealed record ExpectationEvaluation(ExpectationStatus Status, string Details);

public static class ExpectationEvaluator
{
    public static ExpectationEvaluation Evaluate(BehaviorExpectation? expected, ImageAnalysis? actual, string? error = null)
    {
        if (error != null) return new(ExpectationStatus.Error, "Prüfung nicht möglich: " + error);
        if (expected == null) return new(ExpectationStatus.NotEvaluated, "Keine Verhaltenserwartung hinterlegt.");
        try { expected.Validate(); }
        catch (ArgumentException e) { return new(ExpectationStatus.Error, e.Message); }
        if (expected.Verification != "verified")
            return new(ExpectationStatus.NotEvaluated, "Erwartung ist noch nicht geprüft.");
        if (actual == null) return new(ExpectationStatus.Error, "Kein Analyseergebnis vorhanden.");
        int count = actual.Fields.Count(f => f.MeasurementAllowed);
        var details = new List<string>();
        if (expected.MeasurementAllowed is bool allowed && allowed != (count > 0))
            details.Add(allowed ? "Messfreigabe erwartet; kein Feld freigegeben."
                : $"Vollständige Sperre erwartet; tatsächlich {count} Felder freigegeben.");
        if (expected.ReleasedFieldCount is int wanted && wanted != count)
            details.Add($"Erwartet: {wanted} freigegebene Felder; tatsächlich: {count}.");
        bool missingReason = (expected.RequiredHint != null || expected.ForbiddenHint != null) && actual.HintCode == null;
        if (!missingReason)
        {
            if (expected.RequiredHint is { } required && actual.HintCode != required)
                details.Add($"Erforderlicher Hinweis fehlt: {HintText(required)}.");
            if (expected.ForbiddenHint is { } forbidden && actual.HintCode == forbidden)
                details.Add($"Unerwarteter Hinweis: {HintText(forbidden)}.");
        }
        if (details.Count > 0) return new(ExpectationStatus.Failed, string.Join(" ", details));
        if (missingReason) return new(ExpectationStatus.NotEvaluated, "Hinweisprüfung nicht möglich: ältere Analyse ohne fachlichen Hinweiscode.");
        return new(ExpectationStatus.Passed, $"Alle hinterlegten Verhaltenserwartungen erfüllt; {count} Felder freigegeben. Keine Aussage zur Farbgenauigkeit.");
    }

    public static string HintText(AnalysisHintCode code) => code switch
    {
        AnalysisHintCode.PerspectiveTaper => "Streifen verjüngt sich; frontalere Aufnahme erforderlich",
        AnalysisHintCode.None => "kein Qualitätshinweis",
        _ => "anderer Qualitätshinweis"
    };
}