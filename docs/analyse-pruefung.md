# Prüfstand der ersten Iro-Einzelbildanalyse

19. September 2026 · Planfassung 1.19 · Analyzer 0.1.0 · Profil `synthetic-trial-1`.

Dies ist ein Entwicklungsnachweis für den ersten lokalen Analyseweg. Er ersetzt weder den vollständigen technischen Abnahmesatz noch Kamera-, Material- und Farbgenauigkeitsprüfungen am Gerät. Die hier verwendeten Daten sind Entwicklungsdaten, keine zurückgehaltenen Abnahmefälle.

## Tatsächlich ausgeführte Prüfungen

- Gesamte Solution mit festgelegten Paketen (`--locked-mode`) wiederhergestellt; serieller Build einschließlich Android erfolgreich, **0 Warnungen, 0 Fehler**. Wegen einer geöffneten bisherigen IroGen-Instanz wurde der separate Ausgabeordner `artifacts/analysis-build` verwendet. Die laufende Nutzeranwendung wurde nicht beendet.
- **50 Kernprüfungen bestanden**: alle 34 veröffentlichten CIEDE2000-Referenzpaare einschließlich Symmetrie/Identität, D65-Referenzfarben, Mittelung im linearen RGB, MAD-Nullfall und Ausreißer, Puffer/Stride, Abbruch und ungültige Parameter. Unabhängig von IroGen erzeugte RGB-Fixtures prüfen weiße/dunkle Felder, Einzelrechtecke, mehrere mehrdeutige Streifen, einzelne ungültige Felder und eine ungültige gemeinsame Wandreferenz.
- **88 Generator-/Integrationsprüfungen bestanden**: vollständiger Lauf mit 87 Tests, anschließend zusätzlich der neu ergänzte PNG-Grenztest sowie erneuter WPF-Test nach Layoutkorrektur. Die bestehenden 70 Generatorprüfungen bleiben enthalten. Die Analyseprüfungen umfassen vertikale Streifen links/rechts/mittig und horizontale oben/unten, normale Sollwerte, zu große/kleine Aufnahmeabstände, starke Unschärfe, PNG-Dekodierung, Pfadgrenzen und reale Testaufträge.
- Sollwerte und nominale Feldrechtecke nachträglich verändert: identische Pixelanalyse, veränderter beziehungsweise fehlender Sollvergleich. Generator-Erwartungen steuern die Analyse nicht. Fehlerhafte Erwartungsdaten verwerfen bereits errechnete Messwerte nicht.
- Defekte PNG-Datei protokolliert, weiteres Bild derselben Serie verarbeitet. Abbruch nach einem Bild speichert einen ausdrücklich abgebrochenen Lauf mit genau diesem fertigen Befund und erhält sämtliche Eingaben. Historische Testaufträge v1 bleiben lesbar.
- PNG-Adapter weist kaputte, transparente und mit einem nicht unterstützten ICC-Profil versehene Eingaben zurück; Abbruch vor Start geprüft.
- WPF: elf Bilder über die Oberfläche erzeugt, echten Analyse-Button ausgelöst, elf Bilder verarbeitet, Istwerttabelle beim Bildwechsel aktualisiert. Dateien liegen bei dieser Prüfung in einem isolierten Testprojekt. Ansichten bei 1340×900 und 1000×650 gerendert und visuell geprüft; ein scrollbarerer Inhaltsbereich erhält die Bildvorschau bei geringer Fensterhöhe. Schließen während einer neuen Generierung weiterhin geprüft.

## Durchlauf mit 100 und 500 Bildern

Je Serie neue Paletten, sieben Felder, 640×480 Pixel, Seed 46883, sämtliche elf nominalen Abstandsbereiche, keine Störeffekte. PNGs wurden tatsächlich gerendert, als vollständiger Auftrag bereitgestellt und über dieselbe API analysiert, die IroGen verwendet.

| Serie | Verarbeitet | Bilder mit Messwerten | Erkannte und gemessene Felder | Nicht erkannte Sollfelder |
|---|---:|---:|---:|---:|
| 100 Bilder | 100 | 100 | 694 / 700 | 6 |
| 500 Bilder | 500 | 500 | 3.456 / 3.500 | 44 |

Keine Datei-/Verarbeitungsfehler und keine zusätzlichen unerwarteten Felder in diesen Läufen. Bei den geometrisch zugeordneten gültigen Feldern betrug die maximale absolute Differenz zum nominalen ΔE00 rund **1,69 × 10⁻¹¹**. Das zeigt übereinstimmende Berechnung auf diesen homogenen synthetischen Farben, keine zugesicherte reale Messgenauigkeit. Die Nominalspannen der Serien betragen 0–109,662965 beziehungsweise 0–112,908922.

Die Erkennung ist noch nicht vollständig. Unter anderem können sehr helle Felder mit dem weißen Träger verschmelzen. Fehlende Felder werden im nachträglichen Vergleich ausdrücklich `not-detected`; die Analyse ergänzt sie nicht aus den bekannten Generator-Rechtecken. `Measured` im Pixelbefund heißt nur: alle erkannten Felder sind auswertbar. Es bestätigt keine vollständige Erkennung aller Sollfelder.

Ergebnisdateien beider Serien gegen [Analyselauf v1](../tests/schemas/analyse-lauf-v1.schema.json) geprüft; Einzelbefunde, Summen und Messgeometrie nachgezählt. Der vollständige 100-Bilder-Auftrag zusätzlich gegen Testauftrags-, Serien- und Aufnahmeschemata geprüft, einschließlich aller PNG-Abmessungen. Lokale Artefakte: `artifacts/analysis-trial/100/`, `artifacts/analysis-trial/500/`, `artifacts/analysis-ui/`. Vom 500-Bilder-Durchlauf bleibt der Ergebnisbericht erhalten; die temporären Eingaben wurden durch die Testbereinigung entfernt.

## Reproduzieren

```powershell
dotnet restore iro.slnx --locked-mode --artifacts-path artifacts/analysis-build
dotnet build iro.slnx --no-restore --artifacts-path artifacts/analysis-build -m:1
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore --artifacts-path artifacts/analysis-build
$env:IRO_ANALYSIS_OUTPUT = "$PWD/artifacts/analysis-trial"
$env:IROGEN_VISUAL_OUTPUT = "$PWD/artifacts/analysis-ui"
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore --artifacts-path artifacts/analysis-build
./tests/iro.gen.tests/Test-AnalysisRuns.ps1 -Directory artifacts/analysis-trial
$latest = Get-ChildItem artifacts/analysis-trial/100 -Directory -Filter iro-run-* | Sort-Object LastWriteTime -Descending | Select-Object -First 1
./tests/iro.gen.tests/Test-Exports.ps1 -Directory (Join-Path $latest.FullName request)
```

Jeder neue Artefaktlauf erhält einen eigenen Unterordner mit seiner Lauf-ID. Damit bleiben wiederholte Aufträge mit neuen Aufnahme-IDs getrennt. Ohne diese Umgebungsvariablen laufen alle Tests mit temporären Verzeichnissen und eigener Bereinigung.

## Noch offen

Vollständige geplante Profil-/Gradientenerkennung, robustere Behandlung von Schräglage/Perspektive, Weiß-auf-Weiß-Geometrie, schwierigen Lichtverläufen und Materialreflexen; zeitliche Stabilität und Zustandswechsel; Android-UI samt Puffer-/Kameraintegration; unabhängige fachliche Gesamtabnahme und reale Messvalidierung. Der aktuelle Versuch setzt keine endgültige Kamera-Suchzone, Referenzbedienung oder „ähnlich nah“-Schwelle fest. API-Vertrag und Fehlersemantik: [Analyse-API](analyse-api.md).


## Testbedienung, 21. September 2026 – Planfassung 1.20

Testübergabe, Einlesen, Analyse und Ergebnisspeicherung laufen vollständig auf einem Hintergrundtask. Die Oberfläche zeigt nach einer Sekunde weiterhin laufender Arbeit einen unbestimmten Fortschrittsbalken; vor Ablauf der Sekunde abgeschlossene Vorgänge zeigen ihn nie. Eine Abschlussmeldung außerhalb des scrollbaren Bildbereichs bietet direkten Explorer-Zugriff mit markierter Ergebnisdatei. Abgebrochene Läufe und Verarbeitungsfehler erhalten eigene Meldungen.

WPF-Solution erfolgreich gebaut, keine Warnungen/Fehler. Fünf gezielte Tests bestanden: schneller Lauf ohne spätere Indikatoranzeige, langsamer Erfolg, langsamer Fehler, früher Abbruch und vollständiger WPF-Test mit echter Analyse/Abschlussmeldung. Ansichten bei 1340×900 und 1000×650 gerendert; kleine Ansicht visuell geprüft. Der Explorer-Aufruf wird erst durch einen Nutzerklick ausgelöst; während der automatisierten Prüfung wurde kein Explorer-Fenster geöffnet. Die Farbrechnung wurde in diesem Schritt nicht geändert.
