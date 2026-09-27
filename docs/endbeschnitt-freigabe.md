# Begrenzte Freigabe geeigneter Endbeschnitte

24. September 2026; Analyse 0.5.9; Planfassung 1.43. Umsetzung der Nutzerentscheidung zur bedingten Messbarkeit beschnittener Streifen. Keine Geräte- oder allgemeine Beschnittfreigabe.

## Freigabebereich und Grenzen

Ein erkannter Endbeschnitt kann als Teilvergleich ausgewertet werden, wenn alle folgenden Bedingungen erfüllt sind:

- Genau ein einzelner, geometrisch ausreichend gefüllter Randrest und mindestens zwei vollständig erkannte, geometrisch konsistente Felder als Zuordnungsanker. Keine aus mehreren Fragmenten zusammengesetzte Messfläche.
- Streifen waagerecht oder senkrecht ohne rechnerische Drehung. Der Rest berührt nur die passende Stirnseite des Bilds, nicht zusätzlich eine Querseite. Erkannter Beschnitt mit Rotation, zweiter verdächtiger Randfortsetzung oder uneindeutiger Zuordnung bleibt gesperrt.
- Beide Querbegrenzungen des Restes stimmen innerhalb von zwei Rasterpixeln mit den beiden äußeren intakten Ankern überein. Die bestehende Abstandsvorgabe (höchstens 0,85 der Anker-Querbreite, kein Überlappen) bleibt erhalten.
- Sichtbare Restfläche mindestens 40 Pixel je Seite und mindestens 2500 Pixel Fläche. Strengere Einstellungen für Mindestfeldseite/-fläche gelten zusätzlich; niedrigere Einstellungen unterschreiten diese Beschnitt-Untergrenzen nicht.
- Innere Originalmessfläche und größerer Flächenbereich sind geeignet; die gemeinsame beziehungsweise konfigurierte Wandreferenz ist geeignet. Bestehende Stichproben-, Innenabstands-, räumliche Qualitäts-, Kanalanschlag-, Unschärfe- und Perspektivprüfungen bleiben wirksam. Eine ungeeignete zugelassene Restfläche verhindert sämtliche Vergleiche der Aufnahme.

Die drei Beschnittkonstanten sind in MeasurementSafety benannt: MinimumCropSide, MinimumCropArea und MaximumCropEdgeDeviation. Die Eignungsgrenze ist eine Kombination dieser Geometriebedingungen mit den vorhandenen Qualitätsbedingungen, keine statistisch kalibrierte Sicherheitswahrscheinlichkeit oder Prozentzahl des verlorenen Streifens. Die Größenuntergrenzen übernehmen konservativ die bestehende Felderkennungsgröße. Für diese Grenze wurden Fälle unmittelbar darunter und darüber geprüft; daraus folgt keine universelle Gerätegrenze. Der Rest wird aus vorhandenen Pixeln gemessen; fehlende Pixel oder Farben werden nicht rekonstruiert.

## Darstellung und Bericht

Selbst wenn alle sichtbaren Felder messbar sind, lautet der Status PartiallyMeasured und der Hinweis beginnt mit Angeschnittener Streifen. Die Anzahl auswertbarer/erkannter Felder wird genannt. Vergleich und ähnlichster Treffer gelten ausdrücklich nur für die sichtbaren auswertbaren Felder, nie für den vollständigen ursprünglichen Streifen. Es werden keine unbekannten Gesamtfeldzahlen erfunden.

Der Iro-Gen-Bericht berücksichtigte bisher nur die Zahl nominal zugeordneter Felder und konnte einen solchen Teilvergleich als vollständig gemessen einstufen. Eine neue Gegenprobe scheiterte vor Änderung. Die Auswertung berücksichtigt nun zusätzlich den Teilmessungsstatus; Einschränkung und Hinweis bleiben im Dialog und Markdown-Export erhalten. Die Bildgenerierung selbst ist unverändert.

## Zusätzlich geschlossene Schutzlücke

Vier Gegenproben mit 25 Prozent hellen Störpixeln im Randrest wurden zunächst fälschlich freigegeben. Die Regionssuche hatte Komponenten mit zu geringer Flächenfüllung vor der Beschnittprüfung verworfen. Solche Randkomponenten bleiben jetzt als getrennte schwache Geometriehinweise erhalten. Sie sind niemals Freigabekandidaten und werden nicht als freie Wandreferenz genutzt. Bei der Schutzprüfung können sich ihre Rechtecke überlappen; entlang des Streifens müssen Anfang und Ende weiterhin bis auf zwei Pixel übereinstimmen. Die Vereinigung liefert nur ein Ablehnungsindiz, keine Messfarbe oder rekonstruierte Messfläche. Nicht zum Streifen passende Randobjekte werden weiter separat behandelt.

## Gegenproben und Ergebnisse

Vor Implementierung scheiterten alle 16 neuen Freigabetestfälle an der bisherigen Pauschalsperre. Die frühere Gegenprobe mit einem sauberen 90-Pixel-Endrest wurde ausdrücklich auf die neue Nutzerentscheidung umgestellt: drei sichtbare Werte und zwingend Teilstatus statt pauschaler Ablehnung. Die Tests für kleine oder unbrauchbare Reste wurden beibehalten.

CropAcceptanceTests enthält 21 Testfälle mit insgesamt 87 Analysen:

- 32 geeignete Bilder: vier Resthöhen 40/41/60/110 Pixel, alle vier Bildseiten, ohne/mit Rauschen ±2, Seed 82931. Gefordert werden drei echte freigegebene Felder, Teilstatus und Hinweis, ursprüngliche digitale Feldfarben und ΔE00 zur Wand. Toleranz ohne Rauschen 1e-9, mit Rauschen 0,15 ΔE00; ausschließlich digitale Fixture-Toleranzen. Nächster Treffer ausschließlich unter den freigegebenen sichtbaren Feldern.
- 52 Ablehnungsbilder: Resthöhen 6/16/24/32/39; Gradient, Kanalanschlag, seitlicher Versatz, Flecken, ungültige Wandreferenz, starke Bewegungsunschärfe, nur ein intakter Anker und zwei angeschnittene Enden, jeweils alle vier Seiten.
- Drei Konfigurationsgegenproben: strengere Mindestgröße wird eingehalten; eine abgesenkte allgemeine Feldgröße unterläuft die Beschnitt-Untergrenze nicht.

Insgesamt **129 gezielte Kernprüfungen bestanden**, einschließlich früherer Randrest-, Geometrie-, Messregel-, Beleuchtungs- und Unschärfegegenproben sowie der aktualisierten 64-Bilder-Beschnittuntersuchung. Letztere enthält weiterhin eine separate Messung unter bekannter Fixture-Maske; die neuen Freigabetests laufen dagegen ausschließlich über die tatsächliche Erkennung.

**25 gezielte Integrationsprüfungen bestanden**: PNG-Analyse, App-Anzeigeanbindung, Auswertung/Export, Geraderichten, Beleuchtungs- und Unschärfepläne. Vier PNG-Gegenproben prüfen geeigneten/gestörten Beschnitt in beiden Richtungen über den produktiven Adapter. Beispiele und Kurzberichte liegen unter tests/adjustments/endbeschnitt-20260924 (geeignet-senkrecht, geeignet-waagerecht, gestoert-senkrecht, gestoert-waagerecht).

Der obligatorische **Zwei-Bilder-Gegencheck bestand** danach: sauberes Bild und bekannte leichte Störung, einschließlich Feldzuordnung, Werten und Rangfolge. Keine übersprungenen Tests. Schluss-Sammellauf und reale Gerätetests bleiben ausstehend.

## Offene Grenzen und Status

Beliebiger schräger Beschnitt, fehlende oder vollständig unsichtbare Felder, verschiedene Kameras und Auflösungen, Texturen, Reflexe und reale Farbgenauigkeit sind nicht umfassend validiert. Die bereits dokumentierte Mehrdeutigkeit unterschiedlich beleuchteter homogener Flächen bleibt bestehen. Diese Implementierung ist eine begrenzte, synthetisch geprüfte Freigabe und kein Abschluss des gesamten Vollständigkeits-/Beleuchtungspunkts.

- [x] Begrenzte Endbeschnittfreigabe umgesetzt.
- [x] Gezielte positive und negative Gegenproben sowie Berichtsdarstellung geprüft.
- [x] Zwei-Bilder-Gegencheck bestanden.
- [ ] Schluss-Sammellauf, reale Gerätetests, ausdrückliche Nutzerabnahme und Erledigt.

Android-Release-Build zur begrenzten Endbeschnittfreigabe am 24. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest und kein Schluss-Sammellauf.
