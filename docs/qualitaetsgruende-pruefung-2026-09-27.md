# Präzise erkannte Ablehnungsgründe

27. September 2026 · Plan 1.50 · Analyse 0.5.15. Grundlage: neue externe DIALOG-Anweisung. Umsetzung und gezielte Prüfung erfolgt; keine Nutzerabnahme.

## Verhalten und Vertrag

Der vorhandene Haupt-Hinweiscode unterscheidet zusätzlich `UnusableBlur` (erkannte unbrauchbare Unschärfe), `ChannelLimit` (Farbkanal an der Messgrenze) und `UnevenSurface` (räumlich ungleichmäßige Messfläche). Die Codes werden direkt aus den vorhandenen Qualitätsbefunden gesetzt, nicht aus Meldungstexten oder Generatoroptionen erraten. Bestehende Priorität unverändert: Kanalanschlag vor Unschärfe vor räumlicher Ungleichmäßigkeit. Es handelt sich um den Hauptgrund der Aufnahme; keine vollständige Liste sämtlicher Feldursachen. Originalpixel, Auswahl, Schwellen, Freigaben und Sperren unverändert. Alte Enumnummern bleiben erhalten; neue Codes sind angehängt. Analystand 0.5.15 kennzeichnet die Erweiterung; JSON-Struktur unverändert, Schemata und Leser erweitert. Alte gespeicherte Ergebnisse werden nicht umbewertet.

`Other` bleibt für nicht genauer klassifizierte Ablehnungen. Fehlender Code alter Läufe bleibt unbekannt. `UnevenSurface` behauptet ausdrücklich keine erwiesene Licht-/Schattenursache. Aufnahmen, deren Geometrie bereits scheitert, erhalten keine nachträglich erfundene Qualitätsdiagnose.

Die ladbaren Pläne `unschaerfe-strenge-ablehnung.json` und `beleuchtung-schutzpruefung.json` verlangen die präziseren Gründe nur bei tatsächlich belegten Prüfpfaden. Ein Unschärfeplanfall scheitert vorher an mehrdeutiger Geometrie, zwei Beleuchtungsplanfälle an fehlender Wandreferenz: dort bleibt `Other`. Keine bestehende Freigabe-/Sperrerwartung gelockert. Der erste zu pauschale Versuch einer genaueren Test-Erwartung schlug deshalb korrekt fehl; die App wurde nicht passend zu einer unbelegten Ursache geändert.

## Benötigte Werkzeuge

Tatsächlich verwendet: .NET SDK **10.0.401**, Python **3.14.0** für lokale Dateibearbeitung, PowerShell **7.6.5** (Core) aus Codex. `Test-FieldExpectations.ps1` verlangt ausdrücklich PowerShell **7.0 oder neuer** (`#requires`); Windows PowerShell 5.1 allein kann es nicht ausführen. PowerShell 7 war hier unter folgendem vollständigen Pfad vorhanden, ohne notwendige systemweite Installation oder `pwsh` im Suchpfad:

`C:\Users\Peter Breitkopf\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe`

Die neue Startdatei `Invoke-FieldExpectations.ps1` läuft auch unter Windows PowerShell 5.1 und startet diesen Codex-Pfad unter dem aktuellen Benutzerprofil. Fehlt die Runtime, meldet sie das verständlich; keine automatische Installation. Der genaue Pfad ist ein lokaler Runtimepfad, keine dauerhaft zugesagte Codex-Schnittstelle.

## Tatsächlich ausgeführte Befehle und Schlusszeilen

Vom Projektstamm `D:\Source\iro`; Testbefehle funktionieren auch in Windows PowerShell 5.1. Kein vollständiger Sammellauf.

### Kern: Qualitätsgründe und unveränderte Sperren

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~MeasurementRuleTests|FullyQualifiedName~BlurSafetyTests|FullyQualifiedName~IlluminationEvidenceTests' --verbosity minimal
```

Letzte Ausgabe des finalen Laufs:

```text
Bestanden!   : Fehler:     0, erfolgreich:    53, übersprungen:     0, gesamt:    53, Dauer: 4 s - Iro.Core.Tests.dll (net10.0)
```

Derselbe Befehl lief zuvor dreimal: zunächst 50 bestanden / 1 fehlgeschlagen wegen einer zu spezifischen Ursachenerwartung bei früher gescheiterter Geometrie; danach 51/51 bestanden. Beim Ergänzen beider Kanalendpunkte gab es einen Compilerfehler in der Testparametrisierung (CS0103, endpoint fehlte in der Signatur), somit keinen bestandenen Testlauf. Nach Korrektur 53/53. Geprüft: Feld-/Wandkanal bei 0 und 255, Unschärfe trotz anderer scharfer Felder, Kanalpriorität bei kombinierter Störung, Gradienten und geeignete Gegenfälle.

### Generator: Hinweiserwartungen, Serialisierung und Export

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~QualityHintExpectationTests|FullyQualifiedName~ExpectationTests|FullyQualifiedName~BlurPlanTests|FullyQualifiedName~IlluminationPlanTests' --verbosity minimal
```

Der Teilfilter `ExpectationTests` trifft auch `FieldExpectationTests` und `QualityHintExpectationTests`. Ausgeführt wurden diese drei Klassen sowie `BlurPlanTests` und `IlluminationPlanTests`. Neue Gegenproben: jeder neue Grund gegen alle anderen Codes, erforderlicher und verbotener Hinweis, verständlicher Export, unbekanntes Bild und alte Ergebnisse ohne Code. Die beiden PNG-Pläne umfassen 12 beziehungsweise 8 Bilder.

```text
Bestanden!   : Fehler:     0, erfolgreich:    21, übersprungen:     0, gesamt:    21, Dauer: 2 s - IroGen.Tests.dll (net10.0)
```

Erster Lauf desselben Befehls: 19 bestanden / 2 fehlgeschlagen, weil insgesamt drei Bildfälle bereits aus anderen Gründen abgelehnt waren. Die zuvor bewährten vollständigen Sperrerwartungen blieben erhalten; nur die neu zu pauschal angesetzten präzisen Gründe wurden auf die belegte Aussage begrenzt.

### Zwei-Bilder-Gegencheck

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --verbosity minimal
```

```text
Bestanden!   : Fehler:     0, erfolgreich:     1, übersprungen:     0, gesamt:     1, Dauer: 789 ms - IroGen.Tests.dll (net10.0)
```

Ein sauberer und ein bekannt leicht gestörter Fall: Freigabe, Feldzuordnung, Werte, Rangfolge bestanden. [Gesicherter Bericht](../tests/adjustments/zwischenkontrolle-20260923/iro-run-5b5858b432774beebc7a50721ee06b3d/bericht.md). Danach wurde nur die Testparametrisierung für zusätzliche Kanalendpunkte korrigiert; der Produktionsstand blieb unverändert.

### Schema: explizite Shell und Wiederholbarkeit aus Windows PowerShell

Beide folgenden Befehle wurden erfolgreich ausgeführt:

```powershell
& 'C:\Users\Peter Breitkopf\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe' -NoProfile -File tests/schemas/Test-FieldExpectations.ps1
& 'C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe' -NoProfile -File tests/schemas/Invoke-FieldExpectations.ps1
```

Tatsächliche Schlusszeilen beider Aufrufe:

```text
PowerShell 7.6.5
Analyseformat und Zusammenfassung gültig: 3 Bilder; 3 mit Messwerten; 9 gültige Felder.
Fünf Pläne, zwei ungültige Gegenbeispiele und aktueller Ergebnislauf schema-geprüft.
```

Damit ist der Aufruf aus der vorhandenen Windows PowerShell nachgewiesen. Keine Behauptung, dort selbst sei `Test-Json` vorhanden.

### Android und Format

```powershell
dotnet build src/iro.app/Iro.App.csproj -f net10.0-android --no-restore -c Release --verbosity minimal
git diff --check 2>$null
```

Android-Release-Build bestanden. Tatsächliche Schlusszeilen:

```text
    8 Warnung(en)
    0 Fehler

Verstrichene Zeit 00:00:53.71
```

Die acht bekannten Warnungen betreffen XAML-Bindungen (XC0022). Formatprüfung: Exitcode 0, keine Ausgabe.

## Bewusst nicht ausgeführte Tests

Kernprojekt: `AnalysisTests`, `PixelAnalysisTests`, `GeometrySafetyTests`, `MeasurementStabilityInvestigationTests`, `CropEvidenceTests`, `UnderexposureEvidenceTests`, `ContourMaskTests`, `CrossPerspectiveTests`, `CropSafetyRegressionTests`, `CropAcceptanceTests`, `OriginalPixelContractTests`, `SpatialQualityTests`, `RotatedInteriorSafetyTests`, `ReflectionEvidenceTests`.

Generatorprojekt: `AnalysisWorkflowTests` (einschließlich 100-/500-Bilder-Fällen), `AnalysisIntegrationTests`, `AndroidPresentationTests`, `BatchTests`, `DelayedBusyIndicatorTests`, `GeneratorTests`, `LightingQualityTests`, `PerspectiveMaskPngTests`, `PngAdapterTests`, `ReflectionPngTests`, `SpatialSceneTests`, `StraighteningTests`, `UnderexposurePlanTests`, `VisualSmokeTests`, `TestPlanTests`, `ReviewDiagnosticsTests`.

Kein vollständiger Solution-/Sammellauf, kein Emulator-/Kamera-/Gerätetest. Die vorherige externe Nachprüfung von 128/56/7/35 plus Gegencheck wurde als Nutzerbefund übernommen, nicht als erneute eigene Prüfung ausgegeben. Der vollständige Messpfad, zwölf bekannte gleichmäßige Reflex-Fehlfreigaben und reale Grenzen bleiben offen.

## Nächster offener Punkt

Im Arbeitsplan „Vergleichsdaten und unabhängige Abnahmekriterien“ erwartete **Rangfolge und Gleichstände** explizit maschinenlesbar im Iro-Gen-Bericht prüfen. Feldidentität und Messwerte sind bereits prüfbar; sie beweisen nicht automatisch eine korrekt ausgewiesene nächste Farbe bei gleichen oder sehr nahen Werten. Nur innerhalb eines Bildes, ohne zeitliche Bestätigung. Danach wieder gezielte Tests und Zwei-Bilder-Gegencheck. Abnahme bleibt gesammelt am Schluss.
