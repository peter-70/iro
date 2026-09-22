# IroGen – Implementierungs- und Prüfstand

Historischer Erststand: 19. September 2026. Grundlage: Planfassung 1.17 und ausdrücklicher Nutzerauftrag für eine WPF-Anwendung mit Optionen und Sollwertaufdruck.

## Umgebung

Lokal ausgelesen: Windows x64, .NET SDK 10.0.401, MSBuild 18.9.11, Windows-Desktop-Runtime 10.0.12 und Visual Studio Community 2026 18.9.3. IroGen verwendet `net10.0-windows` und WPF. Es wurden keine SDKs, Workloads oder Laufzeitpakete installiert und keine bestehenden Paketversionen geändert. Neue Projektabhängigkeiten besitzen Lockdateien; das Generator-Testprojekt verwendet dieselben Testpaketversionen wie das bestehende Testprojekt.

## Tatsächlich ausgeführte Prüfungen

- `dotnet restore iro.slnx --locked-mode`: erfolgreich.
- `dotnet build iro-gen/IroGen.csproj --no-restore`: erfolgreich, keine Warnungen oder Fehler.
- `dotnet build iro.slnx --no-restore -m:1`: gesamte Solution einschließlich Android erfolgreich, keine Warnungen oder Fehler. Der vorherige parallele Build scheiterte an einer durch einen anderen Prozess gesperrten Android-Zwischendatei unter `obj`; der serielle Wiederholungslauf war erfolgreich. Keine fachliche Änderung oder Paketänderung als Workaround.
- `dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore`: **61 bestanden**, keine übersprungenen oder fehlgeschlagenen Tests.
- Alle **34 veröffentlichten CIEDE2000-Paare** innerhalb 0,0001, zusätzlich Symmetrie und Identität. sRGB/D65-Konvertierung für Schwarz, Weiß und Primärfarben geprüft.
- Pixelwerte im ungestörten Wandbereich und in den Feldinneren einschließlich exaktem Nullvergleich für beide Streifenrichtungen geprüft. Gleicher Seed und gleiche Optionen erzeugen identische Pixel; anderer Seed ändert die Farben.
- Jeder Störeffekt in drei Stärken verändert die Bildpixel, während die nominalen Sollabstände unverändert bleiben. Beide Orientierungen, alle fünf Positionen und drei Abstandsmodi einschließlich Perspektive/Drehung durchlaufen; exportierte Begrenzungen bleiben im Bild.
- PNG-Dekodierung reproduziert die ursprünglichen Pixel. Optionen werden aus der JSON-Aufnahmebeschreibung wiederhergestellt und ergeben dasselbe Bild. Wiederholte Exporte verwenden verschiedene Paketordner. Ungültige Eingaben werden abgewiesen.
- WPF-Anwendung als echter Prozess gestartet; Bedienelemente, Vorschau, sieben Ergebniszeilen und Fertigstatus über Windows UI Automation ausgelesen. Der automatisierte WPF-Test öffnet das Fenster, wartet auf die berechnete Vorschau und rendert die Oberfläche bei 1340×900 und 1000×650.
- Sieben konkrete Beispielpakete erzeugt: normal, horizontal mit anderer Wandfarbfamilie, gedreht/perspektivisch, Glanz/Schatten, Unschärfe/Verschmutzung, nah und fern. Normale/horizontale/perspektivische Bilder und beide Fenstergrößen visuell geprüft.
- `tests/iro.gen.tests/Test-Exports.ps1`: sieben erzeugte PNG-/JSON-Paare gegen das gemeinsame Aufnahmeschema geprüft; eindeutige IDs, vorhandene Bilder, tatsächliche Bildgrößen und Feldbegrenzungen gültig.
- Bestehender `tests/schemas/Test-Examples.ps1`: Generator- und Kameraformatbeispiel gelesen; fünf ungültige Gegenbeispiele abgewiesen.

Lokale visuelle Prüfarbeitsdateien: `artifacts/irogen-final/`. Sie sind von Git ausgeschlossen und keine freigegebenen Iro-Abnahmedaten. Die normale Desktop-Screenshotfunktion war in der Ausführungssitzung nicht verwendbar; die visuellen Fensterprüfungen basieren auf direkt gerenderten WPF-Fenstern, ergänzt um den echten Prozessstart und Windows UI Automation.

## Grenzen dieses Nachweises

Die Tests belegen den Generator und seine Farbmathematik, nicht die Erkennung, Qualitätssperren oder Farbgenauigkeit der Android-App. Iro besitzt im vorhandenen Grundgerüst noch keinen vollständigen Bild-Testablauf. Weder eine Sammlung von mehreren hundert fachlich freigegebenen Testbildern noch die technische Iro-Gesamtabnahme ist hiermit erledigt. Reale Kameratests folgen gemäß Plan später.

Bei Störbildern sind die aufgedruckten nominalen Abstände keine automatisch verbindlichen Sollmesswerte der gestörten Pixel. Der fachliche Sollbefund bleibt zur Prüfung offen. Systemschrift-/Renderingänderungen können Textpixel beeinflussen; Original-PNGs für spätere Regressionstests aufbewahren.

## Historischer Prüfstand: Erweiterung Bildserien – Planfassung 1.18

Ebenfalls am 19. September 2026 nach ausdrücklichem Nutzerauftrag umgesetzt: Bildanzahl, neue Zufallspalette je Bild, systematische Abstandsverteilung, temporäre Festplattenablage, Fortschritt/Abbruch, Bildauswahl, Gesamtspeicherung und vorbereitete lokale Testübergabe. Generatorversion 1.1.0; frühere 1.0-Aufnahmen und Optionsdateien bleiben ladbar. Der Nutzer hat ausdrücklich entschieden, die Testübergabe jetzt vorzubereiten und die noch fehlende Iro-Analyse nicht in diesem Schritt zu entwickeln. Die spätere Analyseanzeige bleibt offen.

Aktueller Prüfstand:

- WPF-Solution mit `--locked-mode` wiederhergestellt und gebaut: keine Warnungen oder Fehler; keine neuen Pakete nötig.
- **70 automatisierte Tests bestanden**, einschließlich der bisherigen 61 Prüfungen und zusätzlicher Serienprüfungen. Die 100-/500-Palettenprüfungen testen jeden quantisierten Ankerabstand gegen seinen vorgesehenen Bereich, eindeutige Paletten, Reproduzierbarkeit und nahezu gleiche Belegung der elf Bereiche. Weitere Fälle prüfen ein Feld und zwanzig Felder mit kleinen/großen Abstufungen.
- Zwei vollständige Serien mit **100 und 500 PNG-Bildern bei 320×320 Pixeln** erzeugt, exportiert und jedes Bild erneut dekodiert. Startseed 46883, sieben Farbfelder. Alle elf Abstandsbereiche sind belegt; sämtliche Paletten innerhalb der jeweiligen Serie verschieden. Nominale Spannen: 0–109,662965 beziehungsweise 0–112,908922. Die Bereiche sind Testparameter, keine Genauigkeitsgrenzen.
- Beide Serienmanifeste und alle **600 PNG-/JSON-Paare** gegen die zentralen Schemata geprüft; Bildanzahlen, Dateiverweise, IDs, Bildgrößen und Begrenzungen gültig.
- Exportierte explizite Paletten erneut eingelesen und gerendert: identische Pixel. Gesamtspeicherung und Testübergabe ergeben unabhängige Kopien, die nach Entfernen des temporären Caches vollständig bleiben.
- Ein isolierter echter Auftrag aus `IroTestHandoff.Submit` mit elf Bildern gegen Testauftrags-, Serien- und Aufnahmeschema geprüft. Status korrekt `pending-analysis`, `analysisAvailable: false`; keine Ist-Ergebnisse erzeugt.
- Abbruch nach zwei erzeugten Bildern hinterlässt keine halbe neue Serie. Abgebrochener Export und ein Kopierfehler nach begonnenem Export veröffentlichen keinen unvollständigen Serienordner. Speichern in den eigenen temporären Cache wird abgewiesen, damit späteres Aufräumen keine vermeintlich dauerhaft gespeicherten Dateien entfernt.
- WPF-Interaktion: elf Bilder in Standardauflösung 1600×1200 über den Generieren-Button erzeugt, 11/11 Bereiche angezeigt, Auswahl auf das letzte Bild gewechselt; Speicher-/Übergabebuttons freigegeben. Layout bei 1340×900 und 1000×650 visuell geprüft. Schließen während einer weiteren laufenden Serie bricht diese zuerst ab und räumt den Cache auf.

Lokale Prüfartefakte liegen unter `artifacts/irogen-series/`, `artifacts/irogen-series-ui/` und `artifacts/irogen-handoff/`. Der Testauftrag wurde in einer isolierten Testumgebung erzeugt, nicht als vermeintlich produktiver Analyseauftrag eingeschleust. Die 600 erzeugten Bilder sind überprüfte Generatorausgaben, aber **kein fachlich abgenommener Iro-Datensatz**: Erkennung, Qualitätssperren, Analyseergebnisse und deren Darstellung sind damit nicht implementiert oder abgenommen.

## Aktueller Analyseweg – Planfassung 1.19

Der anschließend beauftragte Iro-Analyseweg ist inzwischen angebunden. Die oben stehenden Aussagen über fehlende Analyse dokumentieren die vorherigen Umsetzungsschritte. Aktueller Befund einschließlich 100-/500-Bilder-Pixelanalyse und vorläufiger WPF-Istwertanzeige: [Analyse-Prüfstand](analyse-pruefung.md). Die vollständige Android-/Kameraabnahme steht weiterhin aus.
