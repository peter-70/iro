# Lokale Iro-Analyse-API

Stand 19. September 2026, Planfassung 1.19. Erster Einzelbildversuch, Analyzer **0.1.0**, Profil **synthetic-trial-1**. Die Schnittstelle arbeitet lokal im Prozess; es gibt keinen HTTP-Server und keinen Cloud-Aufruf.

## Verwendung aus IroGen oder einem Windows-Testwerkzeug

Projektverweis auf `src/iro.analysis/Iro.Analysis.csproj`, Ziel `net10.0-windows`:

```csharp
using Iro.Analysis;
using Iro.Core.Analysis;

byte[] png = await File.ReadAllBytesAsync(imagePath, cancellationToken);
ImageAnalysis result = await new PngAnalysisApi().AnalyzeAsync(
    png, new AnalysisOptions(), cancellationToken);

foreach (var field in result.Fields)
{
    // null bedeutet: kein zulässiger Messwert. Niemals durch 0 ersetzen.
    Console.WriteLine($"{field.FieldId}: {field.DeltaE00}; {field.Hint}");
}
```

Eingabe: einzelnes deckendes 8-Bit-sRGB-PNG, maximal 64 MiB und 12 Millionen Pixel. Nicht gekennzeichnete PNGs werden gemäß diesem API-Vertrag als sRGB behandelt. Andere ICC-Profile, nicht eindeutig als sRGB markierte Gamma-/Chromatizitätsangaben, transparente oder animierte Dateien werden zurückgewiesen; eine Farbraumkonvertierung ist nicht enthalten. Die Dekodierung und Analyse laufen außerhalb des UI-Threads. Ein Abbruchtoken wird durchgereicht.

Für vollständige vorbereitete IroGen-Aufträge:

```csharp
SavedAnalysisRun run = await new AnalysisRunner().RunAsync(
    requestFolder,
    Path.Combine(projectRoot, "tests", "runs"),
    new AnalysisOptions(),
    progress: new Progress<AnalysisRunProgress>(p =>
        Console.WriteLine($"{p.Completed}/{p.Total}")),
    token: cancellationToken);

Console.WriteLine(run.ResultFile);
```

Der Läufer akzeptiert Testaufträge v1 und v2 und liest die referenzierte Bildserie v1. Er prüft IDs, Anzahlen, Pfadgrenzen, Dateigrößen, Bildgrößen und den deklarierten Farbraum. Verknüpfungen innerhalb des Pakets werden abgewiesen. Jeder fertige Lauf erhält einen neuen Ordner und eine separate `results.json`. Eingaben werden nicht überschrieben. Ein PNG-Hash verbindet den Befund mit den tatsächlich analysierten Bytes.

`completed` bedeutet, dass alle Eingaben verarbeitet wurden; es bedeutet nicht, dass alle Felder erkannt wurden oder eine fachliche Abnahme bestanden wurde. `completed-with-errors` kennzeichnet Datei-/Beschreibungsfehler. Nach Abbruch enthält `cancelled` nur die vollständig verarbeiteten Aufnahmen; Abbruch vor dem Start wirft `OperationCanceledException`. Ein einzelner Dateifehler hält die übrige Serie nicht auf. Fehlerhafte Erwartungen nach abgeschlossener Pixelanalyse lassen die bereits berechnete Analyse erhalten, aber verhindern den Sollvergleich dieser Aufnahme.

## Plattformunabhängiger Kern

`src/iro.core` besitzt keine WPF-, Generator- oder Dateisystemabhängigkeit. Die spätere Android-Pufferanbindung kann dieselbe Schnittstelle verwenden:

```csharp
IImageAnalyzer analyzer = new ImageAnalyzer();
var frame = new RgbFrame(width, height, stride, rgb24);
ImageAnalysis result = analyzer.Analyze(frame, new AnalysisOptions(), cancellationToken);
```

RGB24 bedeutet drei Bytes je Pixel in der Reihenfolge R, G, B, mit kodierten sRGB-Werten. Zeilenpadding ist erlaubt. Bildkoordinaten beziehen sich auf das Originalbild. Der Aufrufer hält den Puffer während der synchronen Analyse unverändert. Kamera-YUV-Konvertierung, CaptureResult-Zuordnung und zeitliche Stabilisierung sind hier noch nicht implementiert.

## Auswertung und Grenzen

- Der Analyzer erhält ausschließlich Pixel und explizite Versuchsparameter. Er kennt weder Sollfarben noch Feldanzahl, Masken, nominale Abstände oder Aufdrucktexte.
- Eine gemeinsame geometrisch benachbarte Wandfläche gehört zu genau diesem Bild. Die experimentelle alternative Referenz pro Feld ist nur ein expliziter API-Versuchsparameter; IroGen verwendet die gemeinsame Referenz. Das endgültige Kamerareferenzmodell bleibt offen.
- Nach dem Erkennen werden innere Messflächen robust gefiltert, in linearem RGB gemittelt und über XYZ/Lab D65 in ΔE00 umgerechnet. Ungültige Flächen liefern `null` und einen Grund; gültige andere Felder bleiben auswertbar. Ein ungültiger gemeinsamer Referenzbereich sperrt sämtliche Vergleiche.
- `Measured` beschreibt die gültigen **erkannten** Felder. Die Pixelanalyse kann nicht wissen, wie viele Felder sie übersehen hat. Erst `comparisons` im Testlauf meldet erwartete, nicht erkannte Felder ausdrücklich als `not-detected`; zusätzliche Erkennungen werden ebenfalls gezählt.
- Nominale Felder werden erst anschließend mit erkannten Rechtecken zugeordnet (Intersection over Union mindestens 0,45; mehrdeutige Zuordnung erhält keinen geratenen Wert). `differenceFromNominal` ist Ist minus nominalem Soll. Bei Störeffekten sind nominale Materialfarben nicht automatisch die korrekten Sollwerte der veränderten Pixel.
- Aktuell ein begrenzter Regions-/Geometrieversuch für annähernd achsenparallele, getrennte Farbfelder. Nahezu weiße Felder am weißen Träger, starke Perspektive, Verdeckung und räumliche Lichtänderungen benötigen weitere Arbeit. Kein vollständiger Nachweis der geplanten Qualitäts-, Gradienten- oder Kamerapipeline.

Verträge: [Analyselauf v1](../tests/schemas/analyse-lauf-v1.schema.json), [Testauftrag v2](../tests/schemas/testauftrag-v2.schema.json). Reproduzierbare Prüfungen und tatsächliche Erkennungszahlen stehen im [Prüfstand](analyse-pruefung.md).

## Verbindlicher Messvertrag zur Bildoptimierung — Planfassung 1.27

Die [gemeinsame Messgrundlage](../IRO-KONSOLIDIERTER-PLAN.md#64-verbindliche-bildoptimierung-und-gemeinsame-messgrundlage) gilt für sämtliche Analyseaufrufe. Eine optimierte Erkennungskopie darf Geometrie und unterstützende Qualitätsmerkmale liefern, aber keine bearbeiteten Messfarben. Wand und Farbstreifen stammen aus demselben Frame und demselben gemeinsamen Messbild. Unabhängige Bereichskorrekturen sind verboten; gemeinsame Messkorrekturen benötigen das festgelegte Modell und den vorgeschriebenen Nutzennachweis.

Die neue Nutzerentscheidung verlangt aufnahmeweite Ablehnung bei messrelevantem Clipping, starker Unterbelichtung, starker Unschärfe und starker Perspektive sowie vollständige Streifensichtbarkeit. Diese Anforderungen haben Vorrang vor älteren allgemeinen Aussagen zur Teilfreigabe in dieser Dokumentation. Umsetzung und Nachweise sind im [Arbeitsplan](arbeitsplan-bildoptimierung.md) noch zu prüfen; dieser Abschnitt behauptet keine bereits vollständige Implementierung.

Aufnahmebezug, geometrische Rückabbildung, tatsächlich verwendete Messgrundlage und gegebenenfalls gemeinsames Korrekturmodell einschließlich Version/Parametern müssen nachvollziehbar sein. Erforderliche neue Vertragsfelder werden im entsprechenden Arbeitsschritt ausdrücklich versioniert; vorhandene Datensätze werden nicht stillschweigend umgedeutet. Sollwerte dürfen nicht als versteckte Analyseeingaben dienen. Die ältere API-Beschreibung oben bleibt ein historischer Implementierungsstand, soweit neuere Anforderungen oder Prüfberichte abweichen.
### Geprüfter Teilstand Analyse 0.5.0

Die [Regelaudit-Korrekturen](regelaudit-2026-09-22.md) sperren erkannte unbrauchbare Unschärfe und relevante Kanalendpunkte aufnahmeweit sowie geometrisch erkannten Bildbeschnitt. Originalpixelmessung bleibt erhalten. Die konservative Endpunktregel ist mit der Analyserversion gekennzeichnet; die Struktur des gespeicherten Ergebnisvertrags bleibt unverändert. Vollständige Perspektiv-, Unterbelichtungs-, Beleuchtungs- und Vollständigkeitsprüfung sind weiterhin nicht abgenommen. Kein allgemeines Freigabeversprechen aus diesem Teilstand ableiten.
