# Räumliche Flächenprüfung – Analyse 0.3.0

22. September 2026, Planfassung 1.24. Grundlage: Nutzerauftrag zur Optimierung nach dem 113-Bilder-Bericht und der dortigen Analyse 0.2.0.

## Änderung

Die bisherige Prüfung aus globaler Kanalstreuung (MAD) und Ausreißeranteil konnte glatte Helligkeitsverläufe innerhalb einer Messfläche übersehen. Außerdem betrachtete die Qualitätsprüfung nur das stark eingerückte Messinnere; ein außerhalb davon sichtbarer Verlauf oder eine teilweise Verdeckung blieb unberücksichtigt.

Neue Prüfung: Die Fläche wird in 3 × 3 Teilflächen aufgeteilt. Je Farbkanal wird aus dem mittleren 50-%-Bereich der Histogrammwerte ein Mittel in linearem RGB gebildet. Diese robusten Teilflächenfarben dienen ausschließlich der Qualitätsprüfung. Der größte paarweise ΔE00-Abstand ist die räumliche Variation. Die eigentliche Messfarbe und ΔE00-Rechnung bleiben unverändert.

- Versuchsschwelle MaximumSpatialDeltaE = 2 ΔE00. Dies ist keine garantierte Messgenauigkeit, Materialtoleranz oder übernommene 1-ΔE00-Grenze des Berichts.
- Messinnere und tatsächliche Wandreferenz werden geprüft.
- Bei Farbfeldern wird zusätzlich eine größere Innenfläche mit SurfaceMargin = 5 % je Rand geprüft. Die Farbmessung verwendet weiterhin InnerMargin = 18 %.
- Einzelne ungeeignete Felder verlieren ihren Wert; eine ungeeignete gemeinsame Referenz sperrt ihre Vergleiche.
- Gleichmäßige Fläche und feine Struktur bleiben zulässig. Die Prüfung wählt keine hellere/dunklere Referenz und rekonstruiert keine Materialfarbe.
- Räumliche Variation erscheint als Diagnosedatum spatialDeltaE bzw. surfaceSpatialDeltaE im Ergebnis. Bei historischen Daten ist ein fehlender Wert unbekannt (null), nicht eine gemessene Null.
- Bei nachgewiesener Unschärfe bleibt der konkrete Unschärfehinweis vorrangig. Sonst nennt der neue Hinweis eine räumlich ungleichmäßige Fläche und empfiehlt gleichmäßigeres Licht bzw. eine einheitliche Fläche; keine sichere Behauptung der Ursache.
- Das Ergebnisschema lässt die neuen optionalen Eigenschaften zu. Die bereits mit 0.2.0 eingeführte Quergrenzentoleranz wurde im Schema nachgetragen.

## Vergleich mit dem bisherigen Verhalten

Die gleiche mitgelieferte Serie wurde neu erzeugt und zweimal analysiert. Im Vergleichslauf war ausschließlich die neue räumliche Sperre mit Schwelle 200 praktisch deaktiviert; diese reproduziert die bisherigen numerischen Gesamtergebnisse (33/20/34/26), ist aber kein separat gestartetes historisches Programm. Die neue Fassung verwendet Schwelle 2. Keine bestehenden Nutzer-results.json wurden eingelesen.

| Befund bei Diagnosegrenze 1 ΔE00 | Bisheriges Verhalten | Neue Prüfung |
|---|---:|---:|
| Vollständig, innerhalb nominaler Grenze | 33 | 33 |
| Freigegebene Werte außerhalb nominaler Grenze | 20 | 12 |
| Teilweise gemessen | 34 | 30 |
| Vollständig abgewiesen | 26 | 38 |
| Dateifehler | 0 | 0 |

Alle 17 ungestörten Kontrollen bleiben vollständig und innerhalb der Diagnosegrenze. Ebenfalls erhalten bleiben sämtliche vollständigen Fälle mit leichtem/mittlerem Rauschen, „weit“/„weiter“ sowie −1 EV.

Die drei stark verschatteten und fünf bislang mit abweichenden Werten freigegebenen Randabfallbilder liefern nun keine Werte. Das ist eine Verbesserung der Sperrprüfung, keine Wiederherstellung richtiger Materialfarben. Die neun Schleierfälle und drei +1-EV-Fälle bleiben nominal auffällig; gleichmäßige Verschiebungen sind mit dieser Prüfung nicht allgemein erkennbar.

### Kosten und Grenzen

Der Versuch ist vorsichtiger als bisher:
- Bei mittlerem Schatten werden die vorher 18 freigegebenen Felder mit kleiner nominaler Abweichung nun gesperrt.
- Bei leichtem Schatten bleiben sechs statt zwölf Felder über drei Bilder auswertbar.
- Bei mittlerer Verdeckung bleiben pro Bild vier statt fünf Felder, sowohl senkrecht als auch waagerecht. Das zusätzliche Feld ist in der kontrollierten Generatorgeometrie am Rand teilweise verdeckt: Die größere Prüfregion erkennt diesen Anteil, obwohl das Messinnere zuvor eine unveränderte Farbe lieferte.
- Die 2-ΔE00-Schwelle ist ein Entwicklungsparameter. Mehr Sperren dürfen nicht als allgemeine höhere Farbgenauigkeit ausgegeben werden. Reale Materialien, Drucktext, schwache Verläufe und Gerätebedingungen benötigen weitere Prüfung.
- Ein außerhalb der geprüften Flächen liegender Beleuchtungswechsel zwischen Feld und Referenz kann weiterhin unentdeckt bleiben.

## Prüfgrundlage

- Unabhängig gezeichnete gleichmäßige Flächen, schwache/starke Gradienten und feine Zweifarbstruktur im Kern.
- Bisherige Farbrechnung, Textausreißer, Verdeckung und lokale Sperren als Regressionen.
- Reproduktion der Schatten-/Randabfallfälle mit allen drei Seeds aus dem Nutzerbericht.
- Vollständiger Vorher-/Nachher-Durchlauf der 113 Bilder.
- Zusätzlich zwölf noch nicht zur Schwellenwahl verwendete positive Bilder: sechs weitere Seeds, beide Orientierungen, variable Feldhöhen, mittleres Rauschen und mittlere Wandstruktur. Alle zuvor gültigen Felder und ihre Werte bleiben erhalten. Dies ist ein begrenzter Generalisierungstest, kein unabhängiger Abnahmesatz.
- Android-Anzeigetests mit vorhandenen normalen/unscharfen PNGs prüfen weiterhin Entfernung ungültiger Werte und konkrete Hinweise.

## Abschließender Prüfstand

57 Kerntests und 107 Generator-/Analyse-/WPF-Tests bestanden. Gesamte Solution in Release erfolgreich gebaut; keine Fehler, acht bestehende XAML-Binding-Warnungen in der unveränderten Android-Oberfläche. Der vollständige 113-Bilder-Vergleich ist im Generator-Testsatz enthalten. Der kompakte Nachher-Bericht liegt lokal unter artifacts/beleuchtung-nachher.md; die dazugehörigen temporären Testpakete werden nach dem Test entfernt. Seeds und Optionen bleiben im Bericht und im versionierten Testplan nachvollziehbar. Keine reale Geräteprüfung in diesem Arbeitsschritt.

Die Änderungen liegen uncommittet im Arbeitsverzeichnis. Der Git-Ausgangsstand war sauber; kein bestehender Nutzer-Code wurde zurückgesetzt.
