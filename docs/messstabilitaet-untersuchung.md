# Messstabilität: kontrollierte Bildfolgen

**Aktualisierung, Planfassung 1.36:** Die nachstehend historisch vorgeschlagene Historien-/Mehrframe-Weiterarbeit ist durch die ausdrückliche Nutzerentscheidung vom 23. September 2026 aufgehoben. Jedes Bild wird unabhängig ausgewertet. Der Historienprototyp wurde entfernt; seine Wiederherstellung ist keine offene Aufgabe. Die Untersuchungsergebnisse bleiben als Entwicklungsbefund erhalten.

Stand: 23. September 2026. Ergänzender Untersuchungsnachweis zu Planfassung 1.34, insbesondere „Paarung vor Glättung“ sowie den Arbeitsplanpunkten Unterbelichtung und sichere Mehrframe-Auswertung. Keine neue Produktentscheidung, kein aktivierter Glätter und keine neue Messsperre. Produktanalyse bleibt 0.5.4.

## Fragestellung und Versuchsaufbau

Die Untersuchung prüft, was Wiederholbarkeit belegt und was nicht. Grundlage sind synthetische RGB-Originalbilder mit festen Flächen; keine rekonstruierten Farben und keine Generator-Sollwerte als Analyzer-Eingabe.

24 Folgen mit je zwölf Frames durchlaufen den produktiven ImageAnalyzer, einschließlich Streifensuche und Wandreferenzwahl: sechs Störmodelle, zwei Streifenrichtungen und zwei Wandfarben (RGB 10 und 140). Bildgröße 800 × 600 beziehungsweise gedreht 600 × 800. Die drei Feldfarben sind RGB (60,80,110), (90,110,140) und (120,140,170). Der Seed ist 12345 plus Frameindex. Jede Folge beginnt mit einem eigenen Versuchsaufbau; kein gemeinsamer historischer Zustand wird vorausgesetzt.

Alle 288 Frames lieferten jeweils drei freigegebene Felder. Das ist die überprüfte Vollständigkeit des begrenzten Versuchs, keine allgemeine Bestätigung der Aufnahmequalität. Felder werden hier ausschließlich anhand ihrer Lage in der festen Geometrie zugeordnet. Bewegte Streifen, wechselnde Feldanzahlen und echtes Tracking sind nicht geprüft.

Die Störmodelle gelten für das gesamte Bild:
- Unveränderte Wiederholung.
- Unabhängiges gleichverteiltes Pixelrauschen von minus bis plus zwei beziehungsweise sechs RGB-Codewerten je Kanal.
- Gemeinsamer, von Frame zu Frame wechselnder Offset minus/plus sechs Codewerte.
- Gemeinsamer konstanter Offset plus sechs Codewerte.
- Lineare Belichtungsänderung von null bis minus zwei Blenden über zwölf Frames, danach sRGB-Kodierung und Quantisierung.

Kein Kanal erreicht 0 oder 255. Die Modelle bilden kein reales Sensorrauschen, keine Kamera-Automatik und keine kamerainterne Rauschunterdrückung vollständig ab. Die Zahl zwölf ist eine Versuchslänge, keine vorgeschriebene Wartezeit.

## Gemessene Ergebnisse

Ausgewertet wird der Wand-Feld-Abstand innerhalb jedes Frames. Pro Feld und Folge werden Median, unskalierte MAD und Spannweite der zwölf Abstände berechnet. Die Basisdifferenz vergleicht den Median mit derselben unveränderten digitalen Szene; sie ist **kein gemessener realer Farbfehler**.

| Störung | Größte zeitliche Spannweite über alle Felder/Richtungen/Wandfarben | Größte Basisdifferenz |
|---|---:|---:|
| Unverändert | 0,00 ΔE00 | 0,00 ΔE00 |
| Pixelrauschen ±2 | 0,05 ΔE00 | 0,01 ΔE00 |
| Pixelrauschen ±6 | 0,13 ΔE00 | 0,01 ΔE00 |
| Wechselnder gemeinsamer Offset ±6 | 2,72 ΔE00 | 0,07 ΔE00 |
| Konstanter gemeinsamer Offset +6 | 0,00 ΔE00 | 1,32 ΔE00 |
| Belichtungswechsel 0 bis −2 EV | 22,88 ΔE00 | 13,54 ΔE00 |

Gerundet auf zwei Nachkommastellen. Vollständige mit sechs Nachkommastellen ausgegebene Einzelbefunde stehen im [automatisch erzeugten Bericht](../tests/adjustments/messstabilitaet-20260923/bericht.md).

**Befund:** Zufälliges Pixelrauschen wird in den großen homogenen Flächen bereits durch die bestehende räumliche Messung stark gemittelt. Ein konstanter gemeinsamer Versatz ist zeitlich vollkommen stabil, verändert aber die Vergleiche. Ein kleiner Medianunterschied bei wechselndem Offset kann gleichzeitig eine große zeitliche Schwankung verdecken. Weder „ruhig“ noch „im Mittel unverändert“ genügt als alleiniger Qualitätsnachweis.

## Fünfermedian getrennt untersucht

Der im Plan genannte Fünfermedian wurde ausschließlich offline auf bereits frameweise berechneten ΔE00-Werten untersucht. Acht vollständige Fenster je Folge; Ausgabe ab dem fünften Frame. Vergleich mit den Rohwerten derselben Frames fünf bis zwölf. Keine Fensterauffüllung mit Nullen, keine Vermischung von Wand- und Feldfarben aus verschiedenen Frames.

- Bei beiden Pixelrauschstufen sank die MAD in allen jeweils zwölf Feld-/Wand-/Richtungsvergleichen.
- Bei unveränderten Bildern und konstantem Offset bleibt das Ergebnis unverändert. Der feste Versatz wird nicht korrigiert.
- Beim wechselnden Offset bleibt die MAD gleich: Der Fünfermedian beseitigt diese alternierende Veränderung nicht.
- Beim Belichtungswechsel bleibt der Median hinter der aktuellen Änderung zurück. Die größte Abweichung vom aktuellen Rohwert beträgt 4,85 ΔE00; die MAD der betrachteten Median-Ausgaben ist hier sogar größer als die der zeitlich passenden Rohwerte. Das ist kein Beleg eines schlechteren Farbrechenkerns, sondern eine Folge der verschobenen Zeitfenster in einer nichtstationären Folge.

Damit ist der Fünfermedian ein belegter Kandidat gegen das untersuchte unabhängige Rauschen, aber kein Unterbelichtungsdetektor, kein Korrektor systematischer Veränderungen und noch kein produktiver Mehrframe-Nachweis. Die bereits beschlossene Historienrücksetzung bei relevanten Änderungen tatsächlicher Aufnahmeparameter bleibt erforderlich. Diese Untersuchung legt deren Toleranzen nicht fest.

## Gegenproben zur Zuordnung und Anzeige

Eine zweite Prüfung verwendet zwei gleiche Flächen pro Frame, abwechselnd RGB 20 und 40. Die gemeinsame Messung liefert in jedem Frame exakt null Abstand. Eine absichtlich falsche Paarung mit der Wand des vorherigen Frames erzeugt dagegen Abstände größer als eins. Hier werden feste Masken verwendet, um die Zeitzuordnung isoliert zu prüfen; es gibt bei identischen Flächen keinen erkennbaren Streifen. Dieser Test implementiert noch keine produktive Frame-ID-Verwaltung.

Eine dritte Prüfung führt geeignetes Bild → schwarzes Bild ohne erkennbares Muster → geeignetes Bild durch denselben Einzelbild-Analyzer und die produktive AnalysisPresentation. Beim ungültigen Bild bleiben keine alten Messwerte angezeigt; nach Wiederherstellung erscheinen wieder die drei aktuellen Werte. Asynchrone Konkurrenz, Rangbestätigung und eine zukünftige Glättungshistorie sind damit nicht erneut geprüft.

## Reproduktion und Status

~~~powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~MeasurementStabilityInvestigationTests --verbosity minimal
~~~

Drei Tests bestanden, keine übersprungen. Die erste Testmethode führt die 288 Matrixanalysen aus und erzeugt den Bericht. Die numerische Grenze 0,0001 in den Gegenproben trennt lediglich eine erkennbare Veränderung von Rechenrauschen; sie ist keine App-Freigabetoleranz. Die Beispiele sind Entwicklungsdaten, keine unabhängigen Abnahmefälle.

Kein Gesamttestlauf, kein Android-Build und keine Gerätetests in dieser Untersuchung. Produktcode unverändert.

## Konsequenz und nächste Arbeit

Die Unterbelichtungssperre bleibt offen. Ein zeitlich stabiles Signal kann weiterhin systematisch verändert oder bereits informationsarm sein. Aus dieser Untersuchung wird deshalb weder eine Helligkeitsschwelle noch eine Stabilitätsfreigabe abgeleitet.

Der nächste technisch abgrenzbare Schritt ist der Eingabe-/Historienvertrag für Bildfolgen: eindeutige Aufnahme- und Messgenerationszuordnung, stabile Feldzuordnung, Paarung innerhalb eines Frames sowie Entwertung bei ungültiger Aufnahme, Referenz-/Feldwechsel und erheblich veränderten Aufnahmebedingungen. Er soll mit künstlichen Bildfolgen geprüft werden, bevor eine Glättung die Kundenanzeige beeinflusst. Ungeklärte Gerätetoleranzen dürfen dabei nicht als beschlossen ausgegeben werden. Messstabilität, Genauigkeit und aktuelle Gültigkeit bleiben getrennte Aussagen.

- [x] Kontrollierte Bildfolgen und Offline-Medianvergleich erstellt.
- [x] Gezielte Prüfungen und kompakter Ergebnisbericht vorhanden.
- [ ] Produktiver Mehrframe-Vertrag und Historienverwaltung implementiert und geprüft.
- [ ] Belastbare Unterbelichtungsgrenze belegt.
- [ ] Abschließender Sammel-Testlauf und reale Gerätetests.
- [ ] Ausdrückliche Abnahme am Schluss.
## Nachfolgender Implementierungsstand

Der oben vorgeschlagene Eingabe-/Historienvertrag ist inzwischen als [isolierter Historienkern Version 1](bildfolgen-historienvertrag.md) umgesetzt und gezielt geprüft. Die vollständige Mehrframe-Verarbeitung bleibt offen. Vor und nach dieser Folgearbeit wurde die neu vorgeschriebene Zwei-Bilder-Kontrolle bestanden.
