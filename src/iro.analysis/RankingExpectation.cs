using System.Text.Json.Serialization;
using Iro.Core.Analysis;

namespace Iro.Analysis;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RankingExpectation
{
    [JsonRequired] public IReadOnlyList<IReadOnlyList<string>> Groups { get; init; } = [];
    [JsonRequired] public double TieTolerance { get; init; }
    public void Validate(IReadOnlyList<FieldExpectation> fields)
    {
        if (!double.IsFinite(TieTolerance) || TieTolerance < 0 || Groups is not { Count: >= 1 and <= 20 }
            || Groups.Any(g => g is not { Count: >= 1 and <= 20 } || g.Any(string.IsNullOrWhiteSpace)))
            throw new ArgumentException("Rangfolge benötigt nichtleere Gruppen und eine endliche, nichtnegative Gleichstandstoleranz.");
        var ids = Groups.SelectMany(g => g).ToArray();
        if (ids.Distinct(StringComparer.Ordinal).Count() != ids.Length
            || !ids.ToHashSet(StringComparer.Ordinal).SetEquals(fields.Where(f => f.MeasurementAllowed).Select(f => f.FieldId)))
            throw new ArgumentException("Rangfolge muss jedes erwartete freigegebene Feld genau einmal enthalten; gesperrte Felder gehören nicht hinein.");
    }
    [JsonIgnore] public string Description => "Rangfolge (nächste zuerst): "
        + string.Join(" < ", Groups.Select(g => string.Join(" = ", g)))
        + $"; Gleichstand bis einschließlich {TieTolerance:0.####} ΔE00 Unterschied";
}

public sealed record RankingPairCheck(string FirstFieldId, string SecondFieldId, string ExpectedRelation,
    string ActualRelation, double Difference, bool Passed);
public sealed record RankingEvaluation(ExpectationStatus Status, string Details,
    IReadOnlyList<RankingPairCheck> Pairs, bool? NearestFlagsCorrect);

internal static class RankingEvaluator
{
    internal static RankingEvaluation Evaluate(RankingExpectation expected, ImageAnalysis actual,
        IReadOnlyDictionary<string, FieldAnalysis> assigned)
    {
        var ordered = expected.Groups.SelectMany((group, rank) => group.Select(id => (Id: id, Rank: rank))).ToArray();
        if (ordered.Any(item => !assigned.TryGetValue(item.Id, out var f) || !f.MeasurementAllowed
            || f.DeltaE00 is not double value || !double.IsFinite(value) || value < 0))
            return new(ExpectationStatus.NotEvaluated, "Rangfolge nicht vollständig prüfbar: sichere Zuordnung oder freigegebener endlicher Messwert fehlt.", [], null);
        var pairs = new List<RankingPairCheck>();
        var failures = new List<string>();
        for (int i = 0; i < ordered.Length; i++)
        for (int j = i + 1; j < ordered.Length; j++)
        {
            var first = ordered[i]; var second = ordered[j];
            double difference = assigned[second.Id].DeltaE00!.Value - assigned[first.Id].DeltaE00!.Value;
            string expectedRelation = first.Rank == second.Rank ? "tied" : "before";
            string actualRelation = Math.Abs(difference) <= expected.TieTolerance ? "tied" : difference > 0 ? "before" : "after";
            bool passed = expectedRelation == actualRelation;
            pairs.Add(new(first.Id, second.Id, expectedRelation, actualRelation, difference, passed));
            if (!passed) failures.Add($"{first.Id} / {second.Id}: erwartet {RelationText(expectedRelation)}, tatsächlich {RelationText(actualRelation)} (Differenz {difference:0.####} ΔE00).");
        }
        // Test tolerance does not alter the app contract: exact minima are marked.
        // Near ties may have one exact minimum; all exact minima must be marked.
        var released = actual.Fields.Where(f => f.MeasurementAllowed).ToArray();
        bool validValues = released.All(f => f.DeltaE00 is double value && double.IsFinite(value) && value >= 0);
        double minimum = validValues && released.Length > 0 ? released.Min(f => f.DeltaE00!.Value) : double.NaN;
        var bestIds = expected.Groups[0].Select(id => assigned[id].FieldId).ToHashSet();
        bool nearestCorrect = validValues && released.Length > 0
            && actual.Fields.All(f => f.IsNearest == (f.MeasurementAllowed && f.DeltaE00 == minimum))
            && actual.Fields.Where(f => f.IsNearest).All(f => bestIds.Contains(f.FieldId));
        if (!nearestCorrect) failures.Add("Kennzeichnung der nächsten Farbe falsch: alle exakten kleinsten freigegebenen Abstände und nur diese müssen markiert sein; sie müssen zur erwarteten ersten Gruppe gehören.");
        return new(failures.Count == 0 ? ExpectationStatus.Passed : ExpectationStatus.Failed,
            failures.Count == 0 ? "Rangfolge, Gleichstände innerhalb der Prüftoleranz und Kennzeichnung der nächsten Farbe erfüllt."
                : string.Join(" ", failures), pairs, nearestCorrect);
    }
    private static string RelationText(string value) => value switch
    { "tied" => "gleich nah innerhalb der Prüftoleranz", "before" => "erstes Feld näher", _ => "zweites Feld näher" };
}
