# Beschnitt: bedingte Messbarkeit und Grenzen

24. September 2026; Entscheidung in Planfassung 1.41. Untersuchung mit unveränderter Analyse 0.5.7.

## Neue Nutzerentscheidung

Beschnitt allein soll nicht dauerhaft pauschal zur Ablehnung führen. Ein sauberes Ergebnis darf bei nachgewiesener Eignung der tatsächlich sichtbaren Flächen zugelassen werden; bei zu großer oder nicht ausreichend begrenzbarer Unsicherheit ist abzulehnen. Diese Entscheidung ersetzt die frühere grundsätzliche Forderung nach einem vollständig sichtbaren Streifen. Sie legt noch keinen numerischen Grenzwert fest und erlaubt weder Rekonstruktion fehlender Farben noch getrennte Bildkorrekturen.

Sicherheit ist hier keine aus dem Bild belegte Prozentwahrscheinlichkeit. Die verlorene Gesamtfläche lässt sich ohne bekannte ursprüngliche Streifengeometrie nicht allgemein als Beschnittanteil bestimmen. Ein belastbarer Freigabebereich muss mehrere beobachtbare Bedingungen verbinden: sichere Feldzuordnung, genügend unverfälschte Originalpixel, sichere Innenabstände, geeignete gemeinsame Wandreferenz und bestandene Qualitätsprüfungen. Eine Rangfolge kann ausschließlich die tatsächlich messbaren sichtbaren Felder betreffen. Ein unsichtbares Farbfeld könnte besser passen; das darf nicht durch einen vermeintlich sicheren Gesamtgewinner kaschiert werden.

## Durchgeführte Gegenproben

64 unabhängige synthetische Bilder: acht sichtbare Resthöhen (6, 16, 24, 32, 40, 60, 90, 110 Pixel), zwei Richtungen und vier Oberflächen (homogen, Rauschen ±2 mit Seed 61927, deutlicher Farbverlauf, roter Kanal auf 255). Die Restbreite beträgt 160 Pixel; zwei weitere rechteckige Farbfelder sind vollständig sichtbar. Der Analyzer erhält nur Bildpixel. Zusätzlich wird eine unabhängig bekannte Fixture-Maske direkt mit RegionSampler geprüft, um Messbarkeit der Restfläche von der Erkennungs- und bisherigen Beschnittsperre zu trennen. Die bekannte Maske wird nicht in die App-Erkennung eingespeist.

Vorab geprüfte Erwartungen: Homogene/restlich leicht verrauschte Flächen ab 40 Pixeln Resthöhe müssen bei festgehaltener korrekter Maske messbar sein; Farbabweichung zum digitalen Original ohne Rauschen höchstens 1e-9 ΔE00, mit Rauschen höchstens 0,15 ΔE00. Sehr kleine Reste bis 16 Pixel und Kanalanschlag müssen als Messfläche ungeeignet bleiben. Größere Flächen mit deutlichem Verlauf müssen ungeeignet bleiben. Diese Toleranzen gelten nur für diese digitalen Fixtures und sind keine Gerätegenauigkeit.

Ergebnisse:

- Homogene Restflächen ab 40 Pixeln sowie die geprüften verrauschten Varianten liefern unter der bekannten Maske die erwarteten Farben. Auch 32 Pixel lieferten in dieser konkreten Geometrie lokal brauchbare Werte; daraus folgt keine allgemeine Freigabegrenze.
- Bei 24 Pixeln kann die größere Fläche geeignet erscheinen, während das nach innen versetzte Messfenster zu schmal ist. Bloße Gesamtpixelzahl reicht somit nicht.
- Sehr schmale Reste, starke Verläufe und Kanalanschlag bestehen die jeweils einschlägige direkte Qualitätsprüfung nicht.
- Die produktive Analyse sperrt erkannte passende homogene Randreste weiterhin entsprechend ihrer bisherigen Regel, auch wenn die direkt bekannte Restfläche messbar wäre. Die neue bedingte Freigabe ist noch nicht implementiert.
- Bei den Farbverläufen wird die passende Randfortsetzung nicht zuverlässig erkannt. Der Analyzer kann zwei freigegebene Felder mit Status Measured liefern, obwohl ein weiterer unbrauchbarer angeschnittener Feldrest vorliegt. Das ist keine nachgewiesene sichere Teilfreigabe und kein bestandener Zielzustand. Die Beobachtungen werden im Bericht ergebnisoffen aufgeführt.

Eine zweite Gegenprobe zeigt zwei unterschiedliche vollständige Szenen mit pixelidentischem sichtbarem Ausschnitt, aber unterschiedlicher Farbe des vollständig abgeschnittenen Felds. Deshalb lässt sich ein bester Treffer über alle ursprünglichen Felder aus diesem Ausschnitt nicht ableiten.

[Mess- und Erkennungsbefunde aller 64 Bilder](../tests/adjustments/beschnitt-20260924/bericht.md).

## Was damit belegt ist und was noch fehlt

Die bisherige Pauschalsperre ist strenger als die lokale Messbarkeit einzelner sauberer Restflächen erfordert. Damit ist die bedingte Freigabe plausibel untersuchbar, aber noch nicht Ende-zu-Ende abgesichert. Die schon vorhandenen Mindestwerte (40 Pixel Feldseite, 2500 Pixel Feldfläche, mindestens 600 Messproben sowie mindestens 20 Pixel innere Seitenlänge) sind Ausgangswerte für weitere Gegenproben, KEINE in dieser Untersuchung neu validierte Beschnitt-Sicherheitsgrenze.

Als nächste konkrete Arbeit die Erkennung gestörter Randreste und die Kennzeichnung eingeschränkter Vergleiche absichern; anschließend Eignungsgrenzen mit real erkannter Geometrie statt bekannten Fixture-Masken prüfen. Dazu gehören Beschnitt an allen Seiten, schräge Streifen, Unschärfe, Wandknappheit, unterschiedliche Größen und zurückgehaltene Vergleichsfälle. Erst ein belegter Freigabebereich rechtfertigt das Lockern der bestehenden produktiven Sperre. Keine Produktvariante anhand eines beliebigen Beschnittprozentsatzes aktivieren.

## Prüfstatus

Zwei neue Untersuchungstests bestanden, einschließlich 64 Einzelbildanalysen; kein fachlicher Gesamt-Erfolg aller beobachteten Analyzer-Ausgaben daraus abgeleitet. Anschließend Zwei-Bilder-Gegencheck bestanden. Produktcode unverändert; kein neuer App-Build erforderlich. Gesamtpunkt, Sammel-Testlauf, Gerätetests und ausdrückliche Abnahme bleiben offen.

## Fortschreibung: Randrestschutz

Die oben für 0.5.7 beobachteten Verlaufs-Randreste werden durch Analyse 0.5.8 in den geprüften Fällen vollständig gesperrt. [Korrektur und Teilmessungskennzeichnung](randrest-schutzpruefung.md). Vorherbefund separat gesichert; die Untersuchungsausgabe zeigt nun den aktuellen Stand. Der bedingte Freigabebereich bleibt offen.
