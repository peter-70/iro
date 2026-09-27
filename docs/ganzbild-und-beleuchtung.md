# Einheitliche Ganzbildverarbeitung und Beleuchtungsablehnung

Stand: 23. September 2026. Nutzerentscheidung und Planfassung 1.38. Analyse 0.5.5 unverändert.

## Verbindliche Festlegung

Jede Bearbeitung einer Aufnahme gilt für das Bild als Ganzes. Wand und Farbstreifen dürfen unter keinen Umständen getrennt nachbearbeitet oder mit unterschiedlichen Korrekturregeln behandelt werden. Diese Regel gilt auch für eine Erkennungskopie. Die Originalpixel bleiben die Messgrundlage, solange kein gemeinsam angewandtes Korrekturmodell unabhängig validiert ist.

Sicher erkannte stark ungünstige Beleuchtung verlangt die vollständige Ablehnung ohne Reparatur. Der vorgesehene Hinweis lautet:

> Lichtverhältnisse wurden als sehr ungünstig erkannt. Bitte für gleichmäßige Beleuchtung sorgen.

Diese Formulierung behauptet eine erkannte Ursache und darf deshalb erst verwendet werden, wenn ein entsprechendes Kriterium ausreichend belegt ist. Helle und dunkle Bildhälften allein beweisen keinen Schatten; unterschiedliche Materialien können dasselbe Bildmuster erzeugen. Eine absolut zuverlässige Klassifikation beliebiger unbekannter Einzelbilder wird nicht versprochen.

Kleine Unterschiede dürfen nur dann als Korrekturkandidaten gelten, wenn sie tatsächlich als Störung erkennbar sind und ein gemeinsames Ganzbildverfahren nachweislich hilft. Die Entscheidung schaltet keine neue Korrektur ein. Eine globale Aufhellung beseitigt eine unterschiedliche Beleuchtung nicht automatisch und hebt sich in ΔE00 nicht allgemein auf.

## Geprüfter Codepfad

Gezielte Quellprüfung des aktuellen Einzelbildwegs:
- ImageAnalyzer verwendet eine gemeinsame Originalaufnahme für Wand und Felder.
- ImageStraightener schätzt eine Richtung; die gedrehte Ansicht benutzt eine gemeinsame geometrische Abbildung.
- RgbFrame.Sample führt die Messmasken in Originalkoordinaten zurück und liefert Originalpixel; kein interpolierter Farbkorrekturpuffer.
- StripDetector liest Pixel zur Regionssuche; keine separate photometrische Nachbearbeitung von Wand oder Streifen.
- RegionSampler wendet dieselben Messregeln auf innere Feld- und Wandflächen an: robuste Pixelauswahl, Mittelung im linearen RGB und Lab-Konvertierung. Die Bildpixel werden dabei nicht verändert.
- Das Kantenprofil für die Unschärfeprüfung ist eine Qualitätsstatistik; dessen gemittelte Werte werden nicht als Messfarben benutzt.

In diesen geprüften Pfaden wurde keine getrennte Helligkeits-, Gamma-, Kontrast-, Weißabgleich- oder Farbkorrektur gefunden. Das ist ein Befund dieses Codeumfangs, kein universeller Nachweis sämtlicher zukünftiger Verarbeitung.

Das Festlegen verschiedener Messmasken und die Messung ihrer Originalpixel sind notwendig, um Wand und Farbfelder überhaupt vergleichen zu können. Sie sind keine getrennte Nachbearbeitung des Fotos. Eine lokale Ersatzfarbe, Normalisierung oder Schattenaufhellung wäre dagegen unzulässig.

## Bestehende Schutzprüfung und offene Grenze

RegionSampler untersucht räumliche Ungleichmäßigkeit innerhalb der ausgewählten Messflächen; ImageAnalyzer prüft zusätzlich einen größeren Feldinnenbereich. Betroffene Bereiche werden gegebenenfalls abgewiesen, eine ungeeignete gemeinsame Wandreferenz verhindert sämtliche Vergleiche.

Diese Prüfung kann Beleuchtung, Materialverlauf, Textur und andere Ursachen nicht allgemein unterscheiden. Ein flacher dunkler Wandbereich neben einem flachen hellen Streifen kann derzeit weiterhin messbar erscheinen. Die neue Entscheidung rechtfertigt keine pauschale Sperre aufgrund dieses Helligkeitsunterschieds und keine Umbenennung vorhandener Flächenwarnungen in angeblich sicher erkannte Lichtverhältnisse.

Die Ursache „stark ungünstige Beleuchtung“ bleibt deshalb eine noch nicht vollständig implementierte Erkennungsaufgabe. Es wurde weder eine unbelegte Lichtklassifikation noch ein nachträglicher Schattenausgleich aktiviert. Vor jeder solchen Implementierung braucht es geeignete Lichtfälle und materialbedingte Gegenbeispiele mit unabhängig begründeten Erwartungen.

## Status

- [x] Nutzerregel in Fachplan, Arbeitsplan und Agentenregeln festgeschrieben.
- [x] Gemeinsamen Originalpixel- und Korrekturpfad gezielt geprüft.
- [ ] Belastbare Erkennung stark ungünstiger Beleuchtung samt aufnahmeweiter Sperre umgesetzt und nachgewiesen.
- [ ] Kleine gemeinsame Korrektur nur bei tatsächlichem Bedarf und unabhängigem Nutzenbeleg; bisher deaktiviert.
- [ ] Abschließender Sammel-Testlauf, Gerätetests und ausdrückliche Abnahmen.
## Prüfungen nach der Dokumentation

Am 23. September 2026 bestanden die fünf vorhandenen SpatialQualityTests. Die verpflichtende Zwei-Bilder-Zwischenkontrolle bestand ebenfalls: saubere Aufnahme und bekannte leichte Rauschstörung, einschließlich Verhaltenserwartungen, Feldzuordnung, Farbabweichung und Rangfolge. Diese Prüfungen belegen keine allgemeine Beleuchtungserkennung. Produktcode wurde für diese Festlegung nicht geändert; der abschließende Sammel-Testlauf bleibt ausstehend.

## Nachfolgende Untersuchung

Planfassung 1.39 / Analyse 0.5.6 ergänzt die [belegte aufnahmeweite Sperre bei starker Flächenungleichmäßigkeit](beleuchtung-untersuchung.md). Dieser ursprüngliche Audit dokumentiert den Stand 0.5.5. Allgemeine Beleuchtungserkennung bleibt offen; insbesondere können verschieden beleuchtete, jeweils homogene Flächen weiter unentdeckt bleiben.
