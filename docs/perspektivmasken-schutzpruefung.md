# Leichte Perspektive: geometrische Konturmasken auf Originalpixeln

Stand: 27. September 2026; Analyse 0.5.13, Planfassung 1.48.

## Befund vor Korrektur

Eine unabhängige projektive Abbildung der gesamten ebenen Szene erzeugt leichte beziehungsweise stärkere seitliche Verkürzung. Die Analyse erhält ausschließlich Bildpixel. Vier neue positive Testfälle scheiterten jeweils am ersten leichten Perspektivbild: Das größere rechteckige Qualitätsfenster enthielt außerhalb der schrägen Feldkante Wandpixel. Ein tatsächlich homogenes Feld wurde deshalb als räumlich ungleichmäßig gesperrt. Die vier starken Gegenfalltests bestanden bereits vor der Korrektur. Keine angeblich neue starke Perspektiverkennung daraus ableiten.

## Umsetzung

Der Detektor darf für begrenzt verformte Komponenten eine konservative Konturmaske liefern. Dafür müssen die vier Randverläufe im mittleren Bereich jeweils durch eine Gerade gestützt sein; jeder dort ausgewertete Konturpunkt muss bis auf einen Erkennungsrasterpixel passen. Mindestens 30 Rasterpixel Seitenlänge; Kantensteigung zwischen 0,02 (mindestens eine Kante) und maximal 0,4; Verhältnis kleinerer/größerer Randbreite und Randhöhe mindestens 0,85. Das sind begrenzende Bildgeometrie-Versuchswerte, keine physikalischen Kamerawinkel oder Gerätefreigaben.

Die Geraden begrenzen ein konvexes Polygon, nach innen um 1,5 Rasterpixel abgesichert. RegionSampler schneidet das bisherige Mess-/Qualitätsrechteck mit dieser Maske und liest nur tatsächlich enthaltene Originalpixel. Keine Pixel werden entzerrt, interpoliert, aufgehellt oder anderweitig korrigiert. Wand und Felder stammen unverändert aus derselben Aufnahme. Robuste Statistik, Mindestzahl verwendbarer Samples, Kanalgrenzen, Flächenungleichmäßigkeit, Unschärfe- und übrige Schutzprüfungen bleiben aktiv. Nicht zuverlässig passende Konturen erhalten keine Maske; der bisherige konservative Rechteckweg bleibt erhalten.

Die tatsächlich benutzten inneren Messpolygone werden auch nach Geraderichten auf Originalkoordinaten zurückgeführt. Die vorhandenen Polygonfelder des Ergebnisvertrags werden genutzt; keine neue Schemaversion. Im bisherigen Produktweg ist keine allgemeine perspektivische Bildentzerrung hinzugekommen: Für die nachgewiesenen leichten Fälle genügt sichere geometrische Pixelauswahl.

## Testumfang

Zwölf neue Perspektiv-Testfälle enthalten 40 Bildanalysen:

- 16 frontal/leicht: Projektivparameter 0 beziehungsweise 0,1, beide Streifenrichtungen, beide Blickrichtungen durch Spiegelung, ohne/mit Rauschen ±2. Alle drei Felder und Originalfarben müssen erhalten bleiben.
- 16 stärkere Konstellationen: Parameter 0,6 beziehungsweise 0,9, beide Richtungen und Spiegelungen, ohne/mit Rauschen. Keine freigegebenen Werte oder nächsten Treffer.
- Acht Kombinationen mit leichter Perspektive, Drehung −23° beziehungsweise 17° und Rauschen: beide Streifenrichtungen/Spiegelungen; alle drei Felder, unverfälschte Farben und übereinstimmende zurückgerechnete Messpolygone.

Die Bildparameter sind keine gemessenen Blickwinkel. Farbtoleranz ohne Rauschen 1e-8 ΔE00, mit Rauschen 0,15 ΔE00: ausschließlich synthetische Prüftoleranzen, keine Aussage realer Farbgenauigkeit. Seed 92741.

Drei direkte Maskentests prüfen unabhängige Polygonzugehörigkeit und Samplezahl, unveränderte Originalpixel, weiterhin gesperrten starken Farbverlauf innerhalb einer Maske sowie leere Schnittflächen ohne Messfarbe.

Vier neue PNG-Integrationstests enthalten 16 gespeicherte Bildbeispiele unter [perspektivmasken-20260927](../tests/adjustments/perspektivmasken-20260927). Geometrie, Werte, Status und Anzeige werden über den produktiven PNG-Adapter geprüft.

## Ausgeführte Prüfungen

- 184 gezielte Kernprüfungen bestanden. Danach erweiterter abschließender Satz von 25 Perspektiv-, Masken- und Originalpixel-Vertragsprüfungen bestanden. Die Sätze überschneiden sich; insgesamt 191 unterschiedliche Kernprüfungen, kein vollständiger Sammellauf.
- 27 bestehende Integrationsprüfungen und vier neue PNG-Prüfungen bestanden; zusammen 31.
- Anschließend Zwei-Bilder-Gegencheck bestanden: saubere Aufnahme und bekannte leichte Störung, Feldzuordnung, Werte und Rangfolge. Keine übersprungenen Tests.

## Grenzen und Status

Die beschränkte leichte Perspektivauswertung ist umgesetzt und geprüft. Allgemeine moderate Entzerrung, gleichmäßige Verkürzung ohne Konturhinweise, beliebige Blickwinkel, Wandabstand samt unterschiedlicher Beleuchtung und echte Kameras sind nicht allgemein abgesichert. Eine Maske beweist keine vergleichbare Beleuchtung. Gleichmäßige Reflexüberlagerung bleibt unverändert offen. Gesamtschritt, Schluss-Sammellauf, Gerätetests und ausdrückliche Abnahme bleiben offen.

Nächste unabhängige Arbeit: Die automatische Erwartungsprüfung im Iro-Gen-Bericht um die Zuordnung der Farbfelder innerhalb eines einzelnen Bilds und verifizierte Messwerterwartungen ergänzen. Nominale Materialabstände bleiben von geprüften Erwartungen für das tatsächlich analysierte Bild getrennt. Keine bildübergreifende Verfolgung.

Android-Release-Build am 27. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest und kein Schluss-Sammellauf.
