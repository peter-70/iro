# Fehlendes Farbfeld bei Abdunklung und leichtem Rauschen

Stand: 24. September 2026. Analyse 0.5.7, Planfassung 1.40. Begrenzte Fehlerkorrektur; keine allgemeine Vollständigkeits-, Beleuchtungs- oder Gerätefreigabe.

## Reproduzierte Ursache

Die unabhängige 800 × 600-Pixel-Szene enthält drei rechteckige homogene Farbfelder. Das gesamte Bild erhält im Vorwärtsmodell den linearen Lichtfaktor 0,5, anschließend digitales Rauschen ±2 mit Seed 72931. Bei waagerechter Ausrichtung erkannte Analyse 0.5.6 nur zwei Felder. Die feste Gegenprobe scheiterte vor der Korrektur sowohl mit als auch ohne Geraderichten; Winkel jeweils 0°.

Eine vorübergehende Diagnose direkt in der Regionssuche belegte die Ursache: Der erste Wandpixel mit RGB (100, 101, 100) wurde als Farbvergleich für die gesamte zusammenhängende Wandregion verwendet. Ein Pixel im fehlenden Feld mit RGB (87, 100, 109) lag innerhalb der bestehenden Kanal-Toleranz 14 und war bereits als Teil der Wandregion markiert. Dadurch verschwand das Feld vor der Geometrie- und Farbauswertung. Die Diagnoselogausgabe wurde anschließend vollständig entfernt.

Es war weder eine Sperre wegen Unterbelichtung noch eine Drehungs- oder Anzeigeursache. Der Status Measured bezog sich auf die zwei tatsächlich erkannten Felder und bewies keine Vollständigkeit des realen Streifens.

## Gezielte Änderung

StripDetector bestimmt den Startvergleich einer Region bei kleiner lokaler Streuung aus einer zusammenhängenden Pilotgruppe von 64 Erkennungspixeln. Die Gruppe wird mit der bestehenden Regions-Toleranz gesucht; bereits zugeordnete und ungültige Pixel bleiben ausgeschlossen. Nur wenn alle 64 Pixel verfügbar sind und die gesamte Spannweite jedes RGB-Kanals höchstens vier digitale Stufen beträgt, wird ihr gerundeter Mittelwert als fester Klassifikationswert verwendet. Andernfalls bleibt der bisherige Startwert erhalten. Der Vergleichswert wandert nicht mit dem weiteren Regionswachstum mit.

Die Größe begrenzt Aufwand und räumlichen Einfluss; die Spannweite deckt die geprüften kleinen digitalen Rauschstufen ab und verhindert eine Mittelwertübernahme aus einer deutlich gemischten Pilotgruppe. Das ist ein begrenzter experimenteller Erkennungsschutz, kein allgemeines physikalisches Rauschmodell. Ein beliebiger Material- oder Lichtverlauf ist damit nicht zuverlässig erkannt.

Keine Bildpixel werden überschrieben oder regional korrigiert. Dasselbe Verfahren gilt für sämtliche Regionsstarts, unabhängig von einer späteren Zuordnung zu Wand oder Streifen. Der Klassifikationswert fließt nicht in die Farbmessung ein; diese verwendet weiterhin Originalpixel. Regions-Toleranz, Mindestgeometrie und sämtliche Qualitäts-Sperrschwellen bleiben unverändert. Weder Feldanzahl noch Sollfarben werden dem Analyzer vorgegeben.

## Nachweise

- Ursprünglicher Fehler vor Änderung in zwei Gegenproben reproduziert; danach beide bestanden.
- 24 Varianten mit vier Seeds (72931, 12345, 91763, 40117), zwei Streifenrichtungen und Rauschen 0/±1/±2 bestanden. Jeweils drei reale Felder und Messfreigabe verlangt; alle vier Feldgrenzen höchstens zwei Pixel vom unabhängigen Rechteck-Soll entfernt. Gemessene Lab-Farbe mit der unabhängig vorwärtsberechneten digitalen Feldfarbe verglichen: ohne Rauschen höchstens 1e-9 ΔE00, mit Rauschen höchstens 0,15 ΔE00. Diese vorab gesetzten Fixture-Toleranzen sind keine Genauigkeitszusage für Geräte. Unveränderte Eingabepixel zusätzlich geprüft.
- Insgesamt 91 gezielte Kernprüfungen bestanden: neue Gegenproben, Beleuchtungsuntersuchung, Geometrie, Unschärfe, räumliche Qualität, Messregeln und Pixelanalyse. Keine übersprungenen Tests.
- 18 Generator-/Integrationsprüfungen bestanden: Geraderichten, Anzeige, Analyseübergabe sowie Beleuchtungs-, Unschärfe- und Unterbelichtungspläne. Die zwölf bisher nicht bewerteten Unterbelichtungsfälle bleiben nicht bewertet; aus erfolgreicher Testausführung wird keine Eignungsfreigabe abgeleitet.
- Zwei-Bilder-Zwischenkontrolle mit sauberem Bild und bekanntem leichtem Rauschen bestanden, einschließlich Zuordnung, Messwerten und Rangfolge.

Die 32-Bilder-Untersuchung wurde zusätzlich vor/nach der gezielten Änderung ausgeführt. Für den Vorher-Nachweis wurde ausschließlich die alte Startpixel-Klassifikation kontrolliert reaktiviert und danach wieder entfernt; Ausgabe als Analyse 0.5.6. [Vorher](../tests/adjustments/felderkennung-20260924/vorher.md), [nachher](../tests/adjustments/felderkennung-20260924/bericht.md). Der ursprüngliche Fehler erscheint im Vorherbericht; im Nachherbericht sind sämtliche zwölf Kontrollerwartungen erfüllt. Das vorherige Beleuchtungsprotokoll wurde auf denselben erneut reproduzierten Vorherstand gesichert.

**Unverändert offen:** Vier Schattenkonstellationen zwischen jeweils homogenen Messflächen werden weiterhin freigegeben; dafür fehlt ein belastbarer allgemeiner Erkennungsnachweis. Die behobene Felderkennung schließt diese Lichtlücke nicht. Unvollständige oder kontrastarme Muster sind nicht allgemein abgesichert. Die gesamten Arbeitspunkte zu Beleuchtung und Vollständigkeit bleiben offen.

Keine neue manuelle Iro-Gen-Konfiguration wird als exakte Reproduktion ausgegeben: Das verwendete Kernfixture mit Rauschen ±2 unterscheidet sich vom vorhandenen Generator-Regler leicht. Der genaue Fehler und seine Varianten sind durch automatisierte unabhängige Pixeltests abgesichert; bestehende PNG-Pläne prüfen angrenzende Abläufe.

## Status

- [x] Ursache reproduziert und eingegrenzt.
- [x] Gezielte Korrektur und betroffene automatisierte Prüfungen bestanden.
- [x] Verpflichtende Zwei-Bilder-Kontrolle bestanden.
- [ ] Schluss-Sammel-Testlauf und reale Gerätetests.
- [ ] Ausdrückliche Nutzerabnahme und Erledigt.

Android-Release-Build nach der Korrektur bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Keine Geräteausführung.
