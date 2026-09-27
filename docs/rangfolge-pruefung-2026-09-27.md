# Rangfolge und Gleichstände im Einzelbild prüfen

27. September 2026 · Plan 1.51 · Analyse unverändert 0.5.15. Grundlage: autorisierte externe DIALOG-Anweisung. Umsetzung und gezielte Prüfung bestanden; keine Abnahme.

## Vertrag und Bedeutung

Erwartungsformat **3** ergänzt `ranking` mit `groups` (nächste zuerst) und ausdrücklich angegebenem `tieTolerance` in ΔE00. Beispiel: `[["field-2"],["field-1","field-3"]]` verlangt zuerst Feld 2, danach einen Gleichstand von Feld 1 und 3. Die Gruppen enthalten genau alle erwarteten freigegebenen Felder, jedes einmal. Gesperrte Felder gehören nicht hinein. Ungültige, doppelte, fehlende oder widersprüchliche Einträge und negative/nichtendliche Toleranzen werden zurückgewiesen. Die vollständigen geometrischen Felderwartungen bleiben Voraussetzung.

Die Prüfung nutzt ausschließlich nach der Pixelanalyse sicher zugeordnete tatsächliche Werte desselben Bildes. Nominalwerte und die Reihenfolge der Erkennungs-Liste bestimmen keine Ränge. Paarweise gilt:

- In derselben Ranggruppe: absoluter Abstandsunterschied kleiner oder gleich Prüftoleranz.
- In verschiedenen Gruppen: jedes frühere Feld muss um mehr als die Prüftoleranz näher sein als jedes spätere.
- Die Grenze selbst gehört zum Gleichstand. Kein Verkettungsautomatismus: A nahe B und B nahe C macht A/C nicht automatisch gleich nah. Nicht eindeutig gruppierbare Ketten erfüllen keine erfundene vollständige Rangordnung.

**Separate Kennzeichnungsprüfung:** Iros bestehende `IsNearest`-Regel bleibt unverändert: alle exakten kleinsten freigegebenen Werte und nur diese werden markiert. Bei einem Toleranzgleichstand können die Werte geringfügig verschieden sein; dann darf innerhalb der ersten Prüfgruppe allein das tatsächlich exakte Minimum markiert sein. Alle markierten Minima müssen zur erwarteten ersten Gruppe gehören. Falsche, fehlende oder veraltete Markierungen bleiben Fehler, auch innerhalb einer tolerierten Gruppe. Eine neue Produkt-Gleichstandsschwelle wird nicht eingeführt.

Fehlende Zuordnungen oder nichtendliche/fehlende Messwerte ergeben keine bestandene Rangprüfung. Ungeprüfte Erwartungen bleiben ungeprüft. Numerische Sollwertprüfung, geometrische Zuordnung, Rangprüfung und Gesamturteil bleiben unterscheidbar: ein separat erfüllter Rangvergleich kann einen anderen Fehler nicht überstimmen.

Gespeichertes Ergebnis: `evaluation.ranking` enthält Status, Klartextbefund, paarweise `expectedRelation`/`actualRelation` (`tied`, `before`, `after`), vorzeichenbehaftete Differenz und `passed` sowie `nearestFlagsCorrect`. Dialog und Markdown-Export zeigen erwartete Ranggruppen, Toleranz und Befund. Äußeres Testplan-/Analyseformat bleibt 2; die verschachtelte Erwartung wird ausdrücklich auf 3 versioniert. Erwartungsversionen 1 und 2 bleiben unverändert lesbar, dürfen keine Rangvorgabe aufnehmen. Leser ohne Format-3-Unterstützung müssen aktualisiert werden. Historische Ergebnisse werden nicht nachträglich bewertet.

Kein Kern-/App-Code geändert: keine neuen Messwerte, Bildkorrekturen, Qualitätsschwellen, Freigaben oder Sperren; keine zeitliche Bestätigung.

## Ladbarer Plan und Nachweise

[Rangfolge und Gleichstände](../iro-gen/testplans/rangfolge-und-gleichstaende.json): fünf Bilder mit eindeutiger Reihenfolge, exaktem Gleichstand senkrecht/waagerecht, nahen Werten innerhalb 0,5 ΔE00 Prüftoleranz und bekannter leichter Rauschstörung. Die Grauwert-Abstände und vorab festgelegten Gruppen werden zusätzlich unabhängig mit der achromatischen CIEDE2000-Formel nachgerechnet; kein Ableiten der Sollordnung aus dem aktuellen Analyseergebnis.

IroGen starten → Testplan laden → Erzeugung abwarten → **An Iro-Tests senden** → Analyse abwarten → auswerten. Erwartet: fünf erfüllte Erwartungen mit je drei Feldern. Synthetische Prüfung, keine Genauigkeitszusage für reale Kameras.

- [Finaler Ergebnisbericht](../tests/adjustments/rangfolge-20260927/iro-run-1204d581b9f1490ab9882315ca0ad450/bericht.md): fünf korrekte Fälle bestanden, fünf absichtlich manipulierte Erwartungen durchgefallen. Die Pixelanalysen beider Läufe sind identisch; nur die Testmetadaten wurden verändert. Persistenz und Bericht auch nach Entfernung der ursprünglichen Eingabemetadaten geprüft.
- [Zwei-Bilder-Gegencheck](../tests/adjustments/zwischenkontrolle-20260923/iro-run-4001ca8a88f940d988823931fabb5b9b/bericht.md): sauber und bekannt leicht gestört, Freigabe/Zuordnung/Werte/Rangfolge bestanden.

## Werkzeuge und tatsächlich ausgeführte Prüfungen

Vom Projektstamm `D:\Source\iro`. .NET SDK 10.0.401, Python 3.14.0, Codex-PowerShell 7.6.5. Die .NET-Befehle sind auch aus Windows PowerShell 5.1 aufrufbar. Das Schemaskript selbst braucht PowerShell 7; der unten verwendete 5.1-Einstieg startet sie ausdrücklich über die vorhandene Codex-Runtime. Keine Installation.

Erster gezielter Lauf:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~RankingExpectationTests|FullyQualifiedName~FieldExpectationTests' --verbosity minimal
```

```text
Bestanden!   : Fehler:     0, erfolgreich:    20, übersprungen:     0, gesamt:    20, Dauer: 2 s - IroGen.Tests.dll (net10.0)
```

Finaler Vertrags-/Exportlauf:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~ExpectationTests|FullyQualifiedName~TestPlanTests|FullyQualifiedName~ReviewDiagnosticsTests|FullyQualifiedName~AnalysisWorkflowTests&FullyQualifiedName!~CompleteCoverageSeriesRunsThroughPixelApi' --verbosity minimal
```

```text
Bestanden!   : Fehler:     0, erfolgreich:    50, übersprungen:     0, gesamt:    50, Dauer: 3 s - IroGen.Tests.dll (net10.0)
```

Der Teilfilter `ExpectationTests` umfasst `ExpectationTests`, `FieldExpectationTests`, `QualityHintExpectationTests` und `RankingExpectationTests`. Dazu `TestPlanTests`, `ReviewDiagnosticsTests` und `AnalysisWorkflowTests` ohne die beiden großen Serienfälle. Gegenproben umfassen vertauschte erste und spätere Plätze, exakte/nahe Gleichstände, beidseitige inklusive Toleranzgrenze, überschrittene Toleranz, versehentliche Gleichstände, nichttransitive Ketten, falsche/fehlende Minima, Teilmessungen, veraltete Markierungen gesperrter Felder, ungültige Verträge, JSON-Persistenz und unveränderte Bildanalyse nach Manipulation von Erwartungen.

Anschließender Pflicht-Gegencheck:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --verbosity minimal
```

```text
Bestanden!   : Fehler:     0, erfolgreich:     1, übersprungen:     0, gesamt:     1, Dauer: 798 ms - IroGen.Tests.dll (net10.0)
```

Zusammen 51 verschiedene ausgewählte Generator-/Integrationstests; die ersten 20 nicht zusätzlich addieren. Keine fehlgeschlagenen oder übersprungenen Tests in diesen Läufen.

Unabhängige Berechnung und Schemata:

```powershell
python docs/verify-field-expectations.py
& 'C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe' -NoProfile -File tests/schemas/Invoke-FieldExpectations.ps1
```

```text
Independent neutral values and configured ranking groups verified: 8 cases.
PowerShell 7.6.5
Analyseformat und Zusammenfassung gültig: 3 Bilder; 3 mit Messwerten; 9 gültige Felder.
Analyseformat und Zusammenfassung gültig: 5 Bilder; 5 mit Messwerten; 15 gültige Felder.
Sechs Pläne, vier ungültige Gegenbeispiele und zwei aktuelle Ergebnisläufe schema-geprüft.
```

Formatprüfung:

```powershell
git diff --check 2>$null
```

Exitcode 0, keine Ausgabe.

## Bewusst nicht ausgeführt

**Gesamtes Kern-Testprojekt** `tests/iro.core.tests/Iro.Core.Tests.csproj` in dieser Arbeit nicht erneut ausgeführt: unveränderter Messkern; die Erweiterung liegt ausschließlich im Windows-Testwerkzeug. Ebenso kein erneuter Android-/Solution-Build, Emulator-, Kamera- oder Gerätetest.

Im Generatorprojekt nicht ausgeführt: `AnalysisIntegrationTests`, `AndroidPresentationTests`, `BatchTests`, `BlurPlanTests`, `DelayedBusyIndicatorTests`, `GeneratorTests`, `IlluminationPlanTests`, `LightingQualityTests`, `PerspectiveMaskPngTests`, `PngAdapterTests`, `ReflectionPngTests`, `SpatialSceneTests`, `StraighteningTests`, `UnderexposurePlanTests`, `VisualSmokeTests`; außerdem die beiden 100-/500-Bilder-Fälle von `AnalysisWorkflowTests.CompleteCoverageSeriesRunsThroughPixelApi`.

Kein Abschluss-Sammellauf und keine Abnahme. Die externe erfolgreiche Nachprüfung der vorherigen 53/21/Gegencheck-Befehle bleibt als Nutzerbefund dokumentiert und wird nicht als hier wiederholte Prüfung gezählt. Zwölf bekannte unerkannt freigegebene Fälle gleichmäßiger Reflexüberlagerung und weitere bereits dokumentierte Grenzen bleiben offen.

## Nächster offener Teilpunkt

Den unabhängigen Prüfbestand für den abschließenden Sammellauf zusammenstellen: vorhandene Bildpläne, vorab geprüfte Erwartungen, bewusst unbewertete Fälle und bekannte Schutzlücken vollständig zuordnen. Insbesondere dürfen bekannte Reflex-Fehlfreigaben nicht hinter erfolgreichen Kontrollbildern verschwinden. Noch keinen Sammellauf oder Gerätefreigabe behaupten; zunächst eine ausführbare, nachvollziehbare Zusammenstellung und klare Liste der verbliebenen Lücken erstellen. Das ist ein Teil der Vergleichsdaten und Abschlussvorbereitung des bestehenden Arbeitsplans.
