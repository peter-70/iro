using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
namespace IroGen;
public static class TestReviewExport
{
    private static string VerdictLabel(ReviewVerdict verdict) => verdict switch
    {
        ReviewVerdict.NominalUnauffaellig => "Vollständig gemessen; nominal unauffällig",
        ReviewVerdict.NominalAbweichend => "Nominale Abweichung; fachlich zu prüfen",
        ReviewVerdict.Teilweise => "Teilweise gemessen",
        ReviewVerdict.Abgewiesen => "Messung abgewiesen",
        ReviewVerdict.OhneSollvergleich => "Gemessen ohne nominalen Vergleich",
        _ => "Verarbeitungs- oder Lesefehler"
    };
    public static string Create(IReadOnlyList<TestRunRow> rows, double tolerance, bool onlyProblems)
    {
        static string Cell(object? value) => (Convert.ToString(value, CultureInfo.InvariantCulture) ?? "–")
            .Replace("|", "/").Replace("\r", " ").Replace("\n", " ");
        static string Number(double? value) => value?.ToString("0.00", CultureInfo.InvariantCulture) ?? "–";
        var text = new StringBuilder("# Iro – kompakter Testbericht\n\n");
        text.AppendLine($"Export UTC: {DateTime.UtcNow:O}");
        text.AppendLine($"Diagnosegrenze: {tolerance.ToString(CultureInfo.InvariantCulture)} ΔE00. Bilder insgesamt: {rows.Count}.");
        text.AppendLine($"Dialogfilter: {(onlyProblems ? "nur Probleme" : "alle")}. Der Export berücksichtigt immer ALLE eingelesenen Bilder.");
        text.AppendLine("Entwicklungsdiagnose, keine Abnahme. Nominale Materialabstände vor Störungen sind keine geprüften Bild-Sollwerte. Abweisung kann richtig sein. Befunde unten sind Beispiele, keine vollständige Einzelfallliste.\n");
        foreach (var verdict in Enum.GetValues<ReviewVerdict>())
            text.AppendLine($"- {VerdictLabel(verdict)}: {rows.Count(r => r.Verdict == verdict)}");
        text.AppendLine("\n## Zusammenfassung nach Testfall, Optionen und Softwareversion\n");
        foreach (var run in rows.GroupBy(r => r.RunId))
        {
            var row = run.First();
            text.AppendLine($"Lauf {Cell(row.RunId)}: {Cell(row.RunStatus)}, verarbeitet {row.ProcessedCount}/{row.RequestedCount}.");
        }
        var optionSets = rows.Where(r => r.OptionsText != null).Select(r => JsonNode.Parse(r.OptionsText!)!.AsObject()).ToArray();
        var common = optionSets.Length == 0 ? new JsonObject() : optionSets[0].DeepClone().AsObject();
        foreach (string key in common.Select(p => p.Key).ToArray())
            if (optionSets.Any(o => !o.ContainsKey(key) || !JsonNode.DeepEquals(o[key], common[key]))) common.Remove(key);
        text.AppendLine($"\nGemeinsame Generatoroptionen: {common.ToJsonString()}");
        var profiles = rows.Select(r => r.AnalysisProfile ?? "unbekannt").Distinct().ToList();
        for (int p = 0; p < profiles.Count; p++) text.AppendLine($"Analyseprofil {p + 1}: {profiles[p]}");
        int index = 0;
        foreach (var group in rows.GroupBy(r => new { r.CaseName, r.Scene, r.Disturbances, r.OptionsText, r.AnalyzerVersion, r.GeneratorVersion, r.AnalysisProfile }))
        {
            index++;
            text.AppendLine($"### {index}. {Cell(group.Key.CaseName ?? group.Key.Scene)}");
            text.AppendLine($"{Cell(group.Key.Scene)}; {Cell(group.Key.Disturbances)}. Generator {Cell(group.Key.GeneratorVersion)}, Analyse {Cell(group.Key.AnalyzerVersion)}.");
            text.AppendLine($"Bilder: {group.Count()}; vollständig/nominal unauffällig: {group.Count(r => r.Verdict == ReviewVerdict.NominalUnauffaellig)}; abweichend: {group.Count(r => r.Verdict == ReviewVerdict.NominalAbweichend)}; teilweise: {group.Count(r => r.Verdict == ReviewVerdict.Teilweise)}; abgewiesen: {group.Count(r => r.Verdict == ReviewVerdict.Abgewiesen)}; Lesefehler: {group.Count(r => r.Verdict == ReviewVerdict.Fehler)}; ohne nominalen Vergleich: {group.Count(r => r.Verdict == ReviewVerdict.OhneSollvergleich)}.");
            text.AppendLine($"Gemessene/erwartete Felder: {group.Sum(r => r.Measured)}/{group.Sum(r => r.Expected)}; zusätzliche Flächen: {group.Sum(r => r.Unexpected)}; größte nominale Abweichung: {Number(group.Max(r => r.MaxDeviation))} ΔE00.");
            var specific = group.Key.OptionsText == null ? new JsonObject() : JsonNode.Parse(group.Key.OptionsText)!.AsObject();
            foreach (string key in common.Select(p => p.Key)) specific.Remove(key);
            text.AppendLine($"Abweichende Generatoroptionen: {specific.ToJsonString()}; Analyseprofil {profiles.IndexOf(group.Key.AnalysisProfile ?? "unbekannt") + 1}.");
            var examples = group.GroupBy(r => r.Verdict).Select(g => g.OrderByDescending(r => r.MaxDeviation ?? -1).ThenBy(r => r.Measured).First()).OrderByDescending(r => r.Verdict == ReviewVerdict.NominalAbweichend)
                .ThenByDescending(r => r.MaxDeviation ?? -1).ThenBy(r => r.Measured).Take(3);
            text.AppendLine("\nBis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):");
            foreach (var row in examples)
            {
                text.AppendLine($"- Lauf {Cell(row.RunId)}, Aufnahme {Cell(row.CaptureId)}, Seed {Cell(row.Seed)}: {row.VerdictText}; {Cell(row.Finding)}");
                if (row.StraighteningDegrees != 0) text.AppendLine($"  Iro: um {row.StraighteningDegrees.ToString("0.0", CultureInfo.InvariantCulture)}° gerade gerichtet.");
                if (row.GeometryText != null) text.AppendLine($"  {Cell(row.GeometryText)}");
                text.AppendLine($"  Hinweis: {Cell(row.Message)}");
            }
            text.AppendLine();
        }
        return text.ToString();
    }
}
