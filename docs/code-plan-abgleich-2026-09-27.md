# Code-/Planabgleich Iro und IroGen

Stand: 27. September 2026. Geprüfte fachliche Grundlage: Planfassung 1.57. Die Dokumentation dieses Audits wird in Fassung 1.58 verlinkt; damit werden keine Produktanforderungen geändert. Geprüft wurde der lokale Arbeitsstand einschließlich vorhandener uncommitteter Änderungen, nicht allein Commit `5befde7e8950778f4c3a1206294addb681e80ca6`. Analyseversion 0.5.15, IroGen 1.6.0. Keine Produktcode- oder Testcodeänderung durch dieses Audit.

## Ergebnis

Der Einzelbild-Messkern und das Windows-Testwerkzeug setzen wesentliche Teile des Plans um. Die Android-App ist dagegen noch ein Testbild-Prototyp. Der vollständige technische Testmodus ist nicht fertig; der Übergang zur Kamera ist deshalb noch nicht erreicht. Zusätzlich wurden eine zu pauschale Kanalendpunkt-Sperre, widersprüchliche vorhandene Tests und ein nicht mehr ausführbar validierbares Prüfinventar konkret nachgewiesen.

Grundlage jeder Bewertung: Wand und Farbreferenz stammen aus demselben Foto unter gleichen Aufnahmebedingungen. Entscheidend ist ihre relative Farbbeziehung. Globale Belichtungs- oder Farbänderungen sind allein kein Fehler. Unterschiedliche Beleuchtung, falsche Flächen, Grenzvermischung und nachgewiesener Informationsverlust können diesen Vergleich schädigen. Das Audit führt keine Bildkorrekturen, Schwellen oder Sperren ein.

## Abdeckung des Plans

| Bereich | IST-Zustand | Bewertung und Grenze |
|---|---|---|
| Architektur | MAUI/Android, unabhängiger C#-Kern, WPF-IroGen und lokale Windows-PNG-API vorhanden. | Entspricht der Aufteilung. Das alte Konsolen-Testgeneratorprojekt ist weiterhin nur ein Gerüst. |
| Farbrechnung | sRGB-Linearisierung, XYZ/Lab D65, CIEDE2000; robuste Auswahl und Mittelung im linearen RGB, anschließend Lab. | Planentsprechender Startansatz. Veröffentlichte Rechenreferenzen und Originalpixelverträge im gezielten Lauf bestanden; keine Aussage über physische Gerätegenauigkeit. |
| Gemeinsame Messgrundlage | `RgbFrame` übernimmt einen privaten Snapshot. Drehung liefert Geometrie; Messung greift auf Originalpixel zurück. Wand und Felder verwenden dieselbe Quelle und dieselben Statistikregeln. | In den geprüften Pfaden keine getrennte photometrische Wand-/Streifenkorrektur gefunden. Regionale Masken/Statistik sind keine Bildkorrektur. |
| Einzelbilder | Analyzer zustandslos; Serienläufer wartet jede Analyse ab. Keine produktive Messhistorie oder Glättung gefunden. | Inhaltlich planentsprechend. Strikte Serialisierung der App-Aufrufe ist separat noch nicht garantiert, siehe unten. |
| Erkennung | Beide Richtungen, Geraderichten, Konturmasken, Geometrie-/Mehrdeutigkeitsprüfung, begrenzter Endbeschnitt und Teilstatus vorhanden. | Fortgeschrittener Versuch, keine allgemeine Vollständigkeits-, Perspektiv- oder Verdeckungsgarantie. Ganzbildsuche und automatische Referenz sind ausdrücklich Versuchskonfigurationen. |
| Qualitätsregeln | Unschärfe, räumliche Flächenungleichmäßigkeit, Kanalendpunkte und Geometrie beeinflussen Freigabe; aufnahmeweite Sperren entfernen Werte und nächsten Treffer. | Teilweise belegt. Endpunktregel widerspricht inzwischen vorhandenen positiven Gegenfällen. Homogene, unterschiedlich beleuchtete Materialien sind nicht allgemein unterscheidbar. |
| Android-Bedienung | Zwei eingebettete PNGs, kleine Bildvorschau, Ergebnisliste, Hinweise und Berechtigungsdialog. | Vollständiger Testmodus fehlt: Overlay, Referenzmarkierung/-bedienung, nächster Treffer in der UI, App-Einstellungen und allgemeine Testeingabe. |
| Kamera | Keine Camera2-Session, kein ImageReader, kein YUV-Konverter und keine CaptureResult-Paarung im Quellbestand. | Bewusst spätere Phase gemäß Plan. Jetzt kein Anlass, die vereinbarte Reihenfolge zu überspringen. |
| IroGen | Reproduzierbare Paletten/Seeds, Serien, Geometrie-/Störmodelle, PNG/JSON-Export, Übergabe, lokale Analyse und Berichte. | Wesentlicher Funktionsumfang vorhanden. Synthetische Störmodelle sind Testeingaben, keine unerlaubten Korrekturen des Iro-Messpfads. |
| Soll-/Ist-Trennung | PNG-API erhält nur Bild und Analyseoptionen. Feldzuordnung, numerische Erwartungen und Ranggruppen werden außerhalb des Analyzers geprüft. | Planentsprechend. Nominale Materialwerte vor einer Störung bleiben Diagnosewerte und sind nicht automatisch Sollwerte der veränderten Aufnahme. |
| Testdaten/Abschluss | Neun JSON-Pläne, automatisierte Fachtests und umfangreiche historische Berichte. | Kein vollständiger unabhängiger Abnahmesatz nachgewiesen. `tests/datasets` enthält nur die README. Technischer Sammellauf und reale Validierung stehen aus. |

## Vorrangige Befunde

### 1. Kanalendpunkte sperren auch ungestörte Materialfarben

Fundstellen: `src/iro.core/Analysis/RegionSampler.cs:46,71`, `MeasurementSafety.cs:8`, `ImageAnalyzer.cs:105–124`.

Mehr als zwei Prozent robust behaltener Pixel mit irgendeinem Kanal exakt 0 oder 255 setzen `HasUnresolvedChannels`; dies sperrt anschließend die ganze Aufnahme. Die Regel prüft weder einen konkret verlorenen relativen Unterschied noch, ob der Endpunkt aus einer echten homogenen Farbe stammt.

Reproduziert: Vier bereits vorhandene Analyzer-Gegenfälle mit RGB (0,100,160), (255,100,160), Schwarz und Weiß erwarten Freigabe, erhalten aber `InvalidFields`. Zwei reine Flächengegenfälle für homogene Endpunkte scheitern ebenfalls. Fundstellen: `tests/iro.core.tests/MeasurementRuleTests.cs:95,143`. Es wurde dabei keine unterschiedliche Aufnahmebeeinflussung von Wand und Streifen erzeugt; der Vergleich wird durch die Endpunktregel gar nicht erst ausgegeben. Die Tests belegen Fehlablehnung der konstruierten Fälle, keine allgemeine Messbarkeit aller realen Endpunktbilder.

Der Testbestand ist außerdem intern widersprüchlich: `FrameWideFailureClearsPreviouslyDisplayedValues` verwendet dieselbe homogene Feldfarbe (255,100,160), erwartet aber gerade deshalb eine aufnahmeweite Sperre. Die kombinierte Unschärfeprüfung erwartet nach Einfügen eines homogenen Endpunktfeldes weiterhin den Kanalhinweis. Eine Änderung allein an der Messlogik kann diesen widersprüchlichen Anforderungen nicht zugleich genügen.

Nächste Arbeit: Die positiven Gegenfälle und tatsächliche Informationsverluste fachlich voneinander abgrenzen, widersprüchliche Erwartungen korrigieren und erst danach die Schutzregel gezielt überarbeiten. Keine unbelegte Änderung der Zwei-Prozent-Grenze. Auch ein homogenes Endpunktplateau kann aus Clipping stammen; Homogenität allein beweist keine sichere Freigabe. Erkennbarkeit und nicht unterscheidbare Fälle ausdrücklich dokumentieren.

### 2. Das Abschlussinventar ist veraltet und seine Vorschau bricht ab

Fundstellen: `tests/abschluss-pruefbestand.json:6,87,111`, `tests/Invoke-AbschlussPruefbestand.ps1:43,56`.

Aktuell enthalten die neun Pläne 277 Bilder, davon 56 mit als geprüft hinterlegten Erwartungen und 221 diagnostische/unbewertete Bilder. Das Inventar behauptet weiterhin 44 beziehungsweise 233. Ursache: Der Unterbelichtungsplan hat inzwischen 16 statt vier bewertete Bilder. Die Vorschau bricht genau an diesem Unterschied mit Exitcode 1 ab.

Zusätzlich fordert die Lückenliste weiterhin die vollständige Ablehnung von zwölf gleichmäßigen Reflexüberlagerungen. Das Skript erzwingt sogar das Vorhandensein dieses alten Eintrags. Das ist mit der relativen Bewertung aus Fassung 1.56 nicht mehr vereinbar. Eine reine Aktualisierung der Zahlen genügt deshalb nicht.

Nächste Arbeit: Inventar, fachliche Lückenbeschreibung und Skriptprüfung gemeinsam synchronisieren. Den alten Befund historisch erhalten, aber nicht als aktuelle pauschale Sperrforderung behandeln. Die Vorschau muss anschließend ohne Sammellauf erfolgreich sein.

### 3. Der geplante Android-Testablauf ist noch nicht implementiert

Fundstellen: `src/iro.app/MainPage.xaml:11–35`, `MainPage.xaml.cs:95–105`, `AppShell.xaml`, `src/iro.core/Analysis/AnalysisPresentation.cs:6,57`.

Die App zeichnet keine Werte auf die Felder, zeigt keine verwendete Wandfläche und überträgt `IsNearest` nicht in ihr Anzeigemodell. Ein Nutzer kann deshalb die automatische Referenzwahl und den nächsten Treffer nicht direkt im Bild nachvollziehen. Der vorhandene Einstellungsbutton öffnet Androids App-Einstellungen; es gibt noch keine Iro-Einstellungsseite mit getrennten, standardmäßig ausgeschalteten und gespeicherten AE-/AWB-Schaltern.

Auch die generische Testbild-/Metadatenquelle, ihre Transformationszuordnung und simulierte Einstellungsreaktionen fehlen. Die vorhandenen Windows-Tests des gemeinsamen Anzeigemodells prüfen keine MAUI-Overlays, Emulatorbedienung oder TalkBack-Bedienelemente.

Nächste Arbeit: Vollständigen Testmodus entsprechend dem Umsetzungskapitel herstellen, bevor der Abschluss-Sammellauf als Übergang zur Kamera gelten kann. Eine endgültige Referenzstrategie oder Grenze für „ähnlich nah“ dabei nicht eigenmächtig festlegen.

### 4. Abbruchschutz ist vorhanden, strikte Serialisierung der App nicht

Fundstelle: `src/iro.core/Analysis/AnalysisPresentation.cs:20–33`.

Ein neuer Aufruf fordert den Abbruch des alten an und startet sofort einen weiteren `Task.Run`. Das Ende des alten Aufrufs wird nicht abgewartet. Kooperative Cancellation kann daher kurzzeitig überlappende Analysen zulassen. Die Revisionsnummer schützt die Anzeige gegen alte Ergebnisse; sie stellt keine serielle Ausführung her. Der vorhandene Anzeigewechseltest bestätigt den Ausgabeschutz, nicht eine maximale Gleichzeitigkeit von eins.

Dies ist ein statischer Vertragsbefund; ein tatsächliches Überlappungsszenario wurde hier nicht zusätzlich ausgeführt. Vor der allgemeinen Test-/Framequelle einen seriellen Ausführungspfad mit passender Abbruch-/Bildwechselprüfung vorsehen. Keine Messwerte zwischen Bildern zusammenführen.

### 5. Viele Bilder sind noch kein umfassender Abnahmenachweis

Der 880-Bilder-Reflexschleiertest ist ein echter Regressionsvertrag, aber laut Code ausschließlich mit senkrechtem Streifen rechts und exakt passendem dritten Feld aufgebaut (`tests/iro.gen.tests/ReflectionVeilRobustnessTests.cs:95`). Er wurde in diesem Audit nicht erneut ausgeführt. Die früheren 880 erfolgreichen Bilder dürfen nicht als allgemeiner Beleg für nichtidentische Farbpaare, komplette Rangfolgen, beide Richtungen und numerische Genauigkeit gelesen werden.

Die 100-/500-Bilder-Serientests prüfen Erzeugung, Dateien, Paletten und Abstandsabdeckung; sie allein sind keine entsprechenden 100-/500-Iro-Messvergleiche. In den neun JSON-Plänen bleiben 221 Bilder ohne geprüfte Verhaltenserwartung. Diese Zahlen erfassen nur diese Pläne, nicht sämtliche in C# definierten Fachtests.

Nächste Arbeit: Unabhängig überprüfte Erwartungen über unterschiedliche Farbabstände, beide Richtungen und relevante Störungskombinationen vervollständigen; einen getrennten zurückgehaltenen Satz nachvollziehbar festlegen. Globale Störungen nur dann zum Ablehnungsfall erklären, wenn relativer Schaden belegt ist.

### 6. Der Plan enthält überholte Aussagen neben aktuellen Entscheidungen

Beispiele im geprüften Stand: Einleitung Zeile 26 führt die Mehrbildfrage noch als offen; Kapitel 13 erklärt sie entschieden. Zeile 425 fordert noch pauschale Reflexschleier-Ablehnung, später ausdrücklich aufgehoben. Zeile 451 verlangt zeitliche Beständigkeit; das Einzelbildprinzip schließt bildübergreifende Bestätigung aus. Zeile 545 nennt vollständige Streifensichtbarkeit, obwohl begrenzter Endbeschnitt zulässig ist.

Für dieses Audit gelten jeweils die neuesten ausdrücklichen Entscheidungen. Als Dokumentationsfolge die überholten Fachpassagen eindeutig historisch markieren oder auf die aktuelle Festlegung verweisen. Keine erneute Produktentscheidung ist für bereits beschlossene Inhalte nötig.

## Tatsächlich ausgeführte Prüfungen

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~AnalysisTests|FullyQualifiedName~OriginalPixelContractTests|FullyQualifiedName~MeasurementRuleTests' --logger 'console;verbosity=minimal' --logger 'trx;LogFileName=codeaudit-core.trx' --results-directory tests/adjustments/codeaudit-20260927

dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~AndroidPresentationTests|FullyQualifiedName~UnderexposurePlanTests|FullyQualifiedName~FieldExpectationTests|FullyQualifiedName~RankingExpectationTests|FullyQualifiedName~TwoImageCheckpointTests' --logger 'console;verbosity=minimal' --logger 'trx;LogFileName=codeaudit-integration.trx' --results-directory tests/adjustments/codeaudit-20260927

& ./tests/Invoke-AbschlussPruefbestand.ps1
```

| Prüfung | Tatsächliches Ergebnis | Bewertung am relativen Vergleich |
|---|---|---|
| Ausgewählte Kerntests | 79 gefunden, 73 bestanden, sechs fehlgeschlagen, null übersprungen | Sechs Fehlablehnungen bei homogenen Endpunktfarben ohne simulierte unterschiedliche Aufnahmebeeinflussung. Der relative Vergleich wird gesperrt. Übrige Prüfungen sichern u. a. gemeinsame Originalpixel, Rechenweg und vorhandene Schutzfälle. Rechen-/Pufferprüfungen allein belegen keine reale Aufnahmegüte. |
| Ausgewählte Generator-/Integrationsprüfungen | 25 gefunden, 25 bestanden, null übersprungen | Gemeinsame Abdunklung und leichtes bildweites Rauschen erhalten Feldzuordnung/Rangfolge in den geprüften Fällen. Unschärfe vermischt Farben über Grenzen; der ungeeignete Vergleich wird entfernt. Gezielte falsche Erwartungen werden erkannt. |
| Darin: Zwei-Bilder-Kontrolle | Bestanden, sauberes Bild und bekannte leichte Rauschstörung | Beide Messpartner unter gleichem Störmodell; Zuordnung, Werte und nächstes Feld bleiben innerhalb der vorgegebenen Prüftoleranzen korrekt. Daraus folgt keine zusätzliche Bildkorrektur oder Sperre. |
| Inventarvorschau | Exitcode 1, Abbruch beim Unterbelichtungsplan | Keine Bildanalyse: veraltete Zahlen verhindern die Validierung. Inventar korrigieren, keine Änderung der Bildfreigabe daraus ableiten. |

Maschinenlesbare Ergebnisse: [Kernlauf](../tests/adjustments/codeaudit-20260927/codeaudit-core.trx), [Integrationslauf](../tests/adjustments/codeaudit-20260927/codeaudit-integration.trx). Der Kernfilter trifft auch Klassennamen, die `AnalysisTests` enthalten. Die Zahlen bezeichnen Testfälle, nicht die gesamte Anzahl intern analysierter Bilder.

Kein vollständiger Testprojektlauf, kein Abschluss-Sammellauf, kein neuer Solution-/Android-Build, keine Emulator-/Geräteprüfung und keine Nutzerabnahme. Die Testbefehle bauen ihre benötigten Projekte. Vorhandene Tests erzeugen eigene Diagnoseberichte und einen neuen Zwei-Bilder-Nachweis; diese Nebenwirkungen sind keine Produktcodeänderungen.

## Empfohlene Reihenfolge

1. Kanalendpunkt-Prüfvertrag widerspruchsfrei machen und die belegten Fehlablehnungen samt Gegenfällen für Informationsverlust bearbeiten. Danach gezielte Tests und Zwei-Bilder-Kontrolle.
2. Prüfbestand und überholte aktuelle Planpassagen synchronisieren; Inventarvorschau wieder grün bekommen.
3. Vollständigen Android-Testmodus bauen: Overlay, sichtbare Referenz, nächster Treffer, App-Einstellungen, Testquelle, Transformations- und serielle Ausführungsverträge. Offene Entscheidungen zu Suchraum, Referenz und „ähnlich nah“ vor der jeweils verbindlichen Produktumsetzung behandeln; zulässige Versuche klar kennzeichnen.
4. Unabhängige Sollbefunde und fehlende technische UI-/Fehlerabläufe vervollständigen. Die nicht allgemein aus Einzelbildern erkennbaren physikalischen Grenzen ehrlich dokumentieren.
5. Erst nach Abschluss der technischen Punkte den vollständigen technischen Sammellauf durchführen. Das derzeitige Skript allein ersetzt keinen vollständigen Emulator-/Bediennachweis.
6. Nach grünem technischen Stand Camera2, YUV-/Metadatenpaarung und Gerätemessung integrieren; reale Validierung und erst danach gebündelte ausdrückliche Nutzerabnahmen.

Diese Reihenfolge ist eine Empfehlung innerhalb der bestehenden Planvorgaben; das Audit schließt keine offene Produktentscheidung und erklärt keinen Arbeitspunkt für abgenommen.
