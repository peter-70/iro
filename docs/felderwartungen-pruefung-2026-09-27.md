# Feldbezogene Erwartungen und Verdeckungskorrektur

27. September 2026 · Plan 1.49 · Analyse 0.5.14. Technisch gezielt geprüft, keine Abnahme.

## Verdeckung: Ursache und begrenzte Korrektur

Der extern gemeldete Test `TestPlanTests.OccluderIsNotReleasedAsAColorField` (Medium) gehörte **nicht** zu den zuvor gemeldeten 27 Integrationsprüfungen. Deren damaliger Filter lautete:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~StraighteningTests|FullyQualifiedName~AnalysisIntegrationTests|FullyQualifiedName~AndroidPresentationTests|FullyQualifiedName~ReflectionPngTests|FullyQualifiedName~ReviewDiagnosticsTests' --verbosity minimal
```

Dieser historische Befehl wurde in der aktuellen Arbeit nicht wiederholt. Die extern berichteten 249/249 Kern- und 145/146 Generatorprüfungen sind eine Nutzer-Nachprüfung, kein hier erneut ausgeführter Gesamtlauf.

Lokale Reproduktion: 3 Verdeckungsvarianten bestanden, Medium gab 0 statt 4 Felder frei. Die geometrisch erkannte Fremdfläche überlappte ein benachbartes Farbfeld; dessen große räumliche Farbabweichung löste die aufnahmeweite Flächensperre aus. In Analyse 0.5.14 werden die inkonsistente Fremdfläche und von ihr überlappte Felder gesperrt. Ihre räumliche Variation begründet keine Sperre der übrigen intakten Felder. Die Wandprüfung, aufnahmeweite Unschärfe- und Kanalanschlagsperren bleiben erhalten. Medium liefert wieder vier echte Felder; die vorhandenen Erwartungen wurden nicht gelockert.

Der erste Versuch nahm zusätzlich solche Flächen aus der Unschärfeprüfung aus. Eine gezielte Kernprüfung erkannte die dadurch verursachte Regression. Dieser Teil wurde zurückgenommen; danach bestand derselbe komplette Filter. Keine Behauptung einer allgemeinen Verdeckungsfreigabe oder eines historisch exakt bestimmten Einführungscommits.

## Felderwartungen im Testwerkzeug

Erwartungsformat 2 ergänzt eine vollständige Liste der erwarteten Generatorfelder mit Freigabe, minimaler geometrischer Überdeckung und optional ausdrücklich geprüftem ΔE00 samt absoluter Toleranz. Die Felderliste muss zur geometrischen Testbeschreibung passen. Mehrdeutige/doppelte Zuordnung, unerwartete freigegebene Flächen und fehlende erwartete Freigaben bestehen nicht. Ein erwartbar gesperrtes Feld darf unerkannt bleiben; das ist ausdrücklich **kein** Nachweis seiner erfolgreichen Erkennung. Die Zuordnung bezieht sich ausschließlich auf das aktuelle Bild.

Die Messwertprüfung liest die tatsächlichen freigegebenen Analysewerte. Nominalwerte und daraus abgeleitete Differenzen sind keine Ersatz-Sollwerte. Verifikation und Grundlage sind Pflicht für eine positive Bewertung; die Software kann die Wahrheit der fachlichen Grundlage nicht selbst beweisen. Keine neue Bildbearbeitung. Der Analyzer erhält weiterhin ausschließlich PNG-Pixel und Analyseoptionen.

Ergebnislauf und Testplan behalten ihre äußere Formatversion 2; die verschachtelte Erwartung hat eine eigene Version. Version 1 bleibt lesbar und enthält weiterhin nur bildbezogene Kriterien. Dialog und Export zeigen Feldname, erwartetes Verhalten und Soll/Ist mit Toleranz in Klartext. Historische Läufe werden nicht rückwirkend geprüft. Nicht für jedes Feld ist eine numerische Erwartung vorgeschrieben; ohne sie wird auch keine Farbgenauigkeit bestätigt.

[Ladbarer Plan](../iro-gen/testplans/feldzuordnung-und-messwerte.json): saubere senkrechte, saubere waagerechte und fest leicht verrauschte Aufnahme. Neutrale sRGB-Felder 80/128/180 gegen Wand 140. Unabhängige achromatische CIEDE2000-Rechnung: 23,338282 / 4,356546 / 12,267977. Saubere Toleranz 0,001 deckt die gerundeten RGB-Matrixkoeffizienten ab; 0,15 beim festen bekannten Rauschfall ist eine synthetische Regressionsgrenze, keine physikalische Gerätegenauigkeit und keine allgemeine Rauschzusage. Referenzrechnung importiert weder Iro noch Generatormethoden.

Ausführung durch den Nutzer: aktuellen Release-IroGen starten, Plan laden, Erzeugung abwarten, **An Iro-Tests senden**, Analyse abwarten, Ergebnisse auswerten und exportieren. Erwartet: drei erfüllte Erwartungen, jeweils drei korrekt zugeordnete messbare Felder.

## Tatsächlich ausgeführte Prüfungen

Alle Befehle vom Projektstamm `D:\Source\iro`. Kein vollständiger Sammellauf. Keine übersprungenen Tests in den ausgewählten Läufen.

1. Fehlerreproduktion, vor der Korrektur zweimal (zweiter Lauf mit ausführlicherer Fehlermeldung): jeweils 3 bestanden / 1 fehlgeschlagen (Medium).

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TestPlanTests.OccluderIsNotReleasedAsAColorField --verbosity minimal
```

2. Erster Korrekturversuch: 6 bestanden; erst die breitere Kernprüfung unten deckte dessen Nebenwirkung auf.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~TestPlanTests.OccluderIsNotReleasedAsAColorField|FullyQualifiedName~IlluminationPlanTests|FullyQualifiedName~BlurPlanTests' --verbosity minimal
```

3. Schutzprüfungen: zunächst 127 bestanden / 1 fehlgeschlagen (`MeasurementRuleTests.DetectedUnusableMotionBlurStopsOtherSharpFieldsToo`); nach Rücknahme der Unschärfeausnahme **128/128 bestanden**.

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~GeometrySafetyTests|FullyQualifiedName~MeasurementRuleTests|FullyQualifiedName~IlluminationEvidenceTests|FullyQualifiedName~BlurSafetyTests|FullyQualifiedName~ReflectionEvidenceTests|FullyQualifiedName~CrossPerspectiveTests|FullyQualifiedName~ContourMaskTests|FullyQualifiedName~RotatedInteriorSafetyTests' --verbosity minimal
```

4. Korrigierte Verdeckung und verpflichtender Gegencheck vor Beginn der Erwartungserweiterung: **7/7 bestanden** (vier Verdeckungsvarianten, Beleuchtungsplan, Unschärfeplan, Zwei-Bilder-Test).

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~TestPlanTests.OccluderIsNotReleasedAsAColorField|FullyQualifiedName~IlluminationPlanTests|FullyQualifiedName~BlurPlanTests|FullyQualifiedName~TwoImageCheckpointTests' --verbosity minimal
```

5. Neue Felderwartungen und vorhandene globale Erwartungen: **15/15 bestanden**. Dazu gehören vertauschte Werte/Felder, doppelte oder unsichere Zuordnung, zusätzliche Freigaben, fehlende oder ungeprüfte Daten, widersprüchliche Verträge und nichtendliche Werte. Ende-zu-Ende: PNG-Analyse, geänderte Erwartungen ohne Analyseänderung, gespeicherter Bericht auch ohne ursprüngliches Eingabepaket.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~FieldExpectationTests|FullyQualifiedName~ExpectationTests' --verbosity minimal
```

6. Erste Vertrags-/Exportprüfung: **35/35 bestanden**. Die zwei zusätzlich genannten Filterteile `ReviewWorkflowTests` und `TestReviewExportTests` treffen keine existierenden Klassen; Exportprüfungen liegen unter anderem in `TestPlanTests`, `ExpectationTests` und `FieldExpectationTests`. Sie werden nicht als zusätzliche Tests gezählt.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~FieldExpectationTests|FullyQualifiedName~ExpectationTests|FullyQualifiedName~TestPlanTests|FullyQualifiedName~ReviewDiagnosticsTests|FullyQualifiedName~ReviewWorkflowTests|FullyQualifiedName~TestReviewExportTests|FullyQualifiedName~AnalysisWorkflowTests&FullyQualifiedName!~CompleteCoverageSeriesRunsThroughPixelApi' --verbosity minimal
```

7. Angrenzende Originalpixel-, Flächen- und Beschnittverträge: **56/56 bestanden**.

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~OriginalPixelContractTests|FullyQualifiedName~SpatialQualityTests|FullyQualifiedName~CropSafetyRegressionTests|FullyQualifiedName~CropAcceptanceTests' --verbosity minimal
```

8. Abschließende Vertrags-/Exportprüfung nach Ausschluss einer reinen Anzeigeneigenschaft vom JSON-Format: **35/35 bestanden**.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~FieldExpectationTests|FullyQualifiedName~ExpectationTests|FullyQualifiedName~TestPlanTests|FullyQualifiedName~ReviewDiagnosticsTests|FullyQualifiedName~AnalysisWorkflowTests&FullyQualifiedName!~CompleteCoverageSeriesRunsThroughPixelApi' --verbosity minimal
```

9. Danach verpflichtender Gegencheck: **1/1 Test mit zwei neu erzeugten Bildern bestanden**. Saubere und bekannte leicht verrauschte Aufnahme: drei freigegebene richtige Felder, keine zusätzliche Fläche, mindestens 90 % geometrische Überdeckung, Werte innerhalb der vorher festgelegten Grenzen und gleiche nächste Farbe.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --verbosity minimal
```

10. Unabhängige Sollwertrechnung sowie Schemagegenprüfung: bestanden. Drei Testpläne, zwei bewusst ungültige Gegenbeispiele und letzter gespeicherter Drei-Bilder-Ergebnislauf. Die Schemata prüfen Struktur; semantische Widersprüche prüft zusätzlich der C#-Vertrag.

```powershell
python docs/verify-field-expectations.py
& tests/schemas/Test-FieldExpectations.ps1
```

Zuvor zusätzlich direkt erfolgreich ausgeführte Schema-Prüfung desselben Ergebnisstands:

```powershell
$latest=Get-ChildItem tests/adjustments/felderwartungen-20260927 -Directory | Sort-Object LastWriteTime -Descending | Select-Object -First 1
& tests/iro.gen.tests/Test-AnalysisRuns.ps1 -Directory $latest.FullName
$p='iro-gen/testplans/feldzuordnung-und-messwerte.json'
if (-not (Test-Json -Json (Get-Content $p -Raw) -SchemaFile tests/schemas/irogen-testplan-v2.schema.json)) { throw 'Schemafehler Testplan' }
if (-not (Test-Json -Json (Get-Content iro-gen/testplans/zwischenkontrolle-zwei-bilder.json -Raw) -SchemaFile tests/schemas/irogen-testplan-v2.schema.json)) { throw 'Schemafehler Altplan' }
```

11. Kunden-App: Android-Release-Build bestanden: 0 Fehler, 8 bekannte XAML-Bindungswarnungen (XC0022).

```powershell
dotnet build src/iro.app/Iro.App.csproj -f net10.0-android --no-restore -c Release --verbosity minimal
```

12. Formatprüfung bestanden:

```powershell
git diff --check 2>$null
```

## Bewusst nicht ausgeführt

Kernprojekt: `AnalysisTests`, `PixelAnalysisTests`, `MeasurementStabilityInvestigationTests`, `CropEvidenceTests`, `UnderexposureEvidenceTests`. Die übrigen oben genannten zwölf Kernklassen wurden gezielt ausgeführt; zusammen 184 unterschiedliche Tests, keine Aussage über alle 249 Tests.

Generatorprojekt: `AnalysisIntegrationTests`, `AndroidPresentationTests`, `BatchTests`, `DelayedBusyIndicatorTests`, `GeneratorTests`, `LightingQualityTests`, `PerspectiveMaskPngTests`, `PngAdapterTests`, `ReflectionPngTests`, `SpatialSceneTests`, `StraighteningTests`, `UnderexposurePlanTests`, `VisualSmokeTests`. Innerhalb `AnalysisWorkflowTests` wurden die beiden großen 100-/500-Bilder-Fälle von `CompleteCoverageSeriesRunsThroughPixelApi` ausgelassen. Alle anderen oben ausgewählten Tests liefen. Überlappende Wiederholungen nicht addieren: 38 unterschiedliche Generator-/Integrationstests einschließlich Gegencheck.

Kein Emulator-, Kamera- oder Gerätetest; keine vollständige Solution-Prüfung. Der abschließende Sammellauf und die Abnahmen bleiben späteren Schritten vorbehalten.

## Nachweise und Grenzen

- [Finaler Feldvergleich samt absichtlich falschen Gegenproben](../tests/adjustments/felderwartungen-20260927/iro-run-8641284c6d414ab999d1b033a0384cf7/bericht.md). Drei korrekte Fälle erfüllt, drei absichtlich manipulierte Erwartungen nicht erfüllt; Pixelanalyse dabei identisch.
- [Gegencheck nach Verdeckungskorrektur](../tests/adjustments/zwischenkontrolle-20260923/iro-run-69c969b939c04a78a286d58ed8b10969/bericht.md).
- [Abschließender Gegencheck](../tests/adjustments/zwischenkontrolle-20260923/iro-run-5874f48d71324f67bdd1968318bf906a/bericht.md).

Die zwölf bekannten unerkannten Freigaben unter gleichmäßiger Reflexüberlagerung sind weiterhin offen. Kein synthetischer Test beweist reale Messgenauigkeit. Der vollständige unabhängige Datensatz, weitere Qualitätsgründe und Abnahmen fehlen weiterhin.

Nächster offener Teilpunkt im Arbeitsplan „Diagnose und verständliche Anzeige“: Unschärfe, Kanalanschlag, ungleichmäßige Flächen/Beleuchtung und weitere tatsächlich erkannte Ablehnungsursachen eindeutig maschinenlesbar ausweisen und mit geprüften Erwartungen kontrollieren. Der bisherige Sammelcode „anderer Qualitätshinweis“ kann diese Ursachen nicht unterscheiden. Dabei nur vorhandene nachgewiesene Gründe benennen; keine unbekannte Ursache behaupten und keine Sperren lockern.


Nachtrag zur Wiederholbarkeit (27. September 2026): Die obigen direkten Schemaaufrufe erfolgten in Codex-PowerShell 7.6.5, nicht Windows PowerShell 5.1. [Konkreter Runtimepfad, geprüfter 5.1-Einstieg und tatsächliche Ausgabe](qualitaetsgruende-pruefung-2026-09-27.md). Keine neue Ausführung der hier historischen Gesamtfilter durch diesen Nachtrag.
