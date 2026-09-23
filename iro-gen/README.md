# IroGen

WPF-Anwendung für numerisch kontrollierte Testbilder aus Wand und einem Farbmusterstreifen. Lokal, ohne Bildgenerierungsdienst oder zusätzliche Laufzeitpakete. Die Auswertung erfolgt jetzt über die lokale Iro-Analyse-API. Eine vorläufige Tabelle zeigt Istwerte und Diagnosen; die ausführliche Analyseoberfläche wird später festgelegt.

## Start

`D:\Source\iro\iro-gen\IroGen.slnx` in Visual Studio öffnen, **IroGen** als Startprojekt wählen und **F5** drücken. Das Projekt ist zusätzlich in der übergeordneten `iro.slnx` enthalten.

Vom Projektstamm:

```powershell
dotnet restore iro-gen/IroGen.slnx --locked-mode
dotnet run --project iro-gen/IroGen.csproj --no-restore
```

Geprüfte Umgebung am 19. September 2026: Windows x64, SDK **10.0.401**, Windows-Desktop-Runtime **10.0.12**, Visual Studio Community 2026 **18.9.3**. Ziel ist `net10.0-windows` mit WPF; das bestehende SDK-Pinning wird übernommen. Keine MAUI-Workloads für diese separate Solution erforderlich.

## Bedienung

1. **Anzahl Bilder** einstellen, beispielsweise 100 oder 500, und links Geometrie, Feldzahl, Farbabstufung sowie Störungen wählen.
2. **Bilder generieren** erzeugt die komplette Serie. **Neue Zufallsfarben** wählt einen neuen Startseed und erzeugt die Serie erneut. Fortschritt und Abbruch sind verfügbar.
3. Über die Bildauswahl oberhalb der Vorschau jedes erzeugte Bild betrachten. Die Zusammenfassung nennt Anzahl, tatsächlich erreichte nominale ΔE00-Spanne und belegte Abstandsbereiche.
4. **Alle speichern** kopiert alle PNG-/JSON-Paare und `series.json` in einen neuen Unterordner des gewählten Ziels. Es wird genau die fertig erzeugte Serie gespeichert; noch nicht angewendete Optionen ändern diese nicht.
5. **An Iro-Tests senden** stellt die vollständige Serie unter `tests/requests/` als lokalen Testauftrag bereit. Anschließend analysiert Iro jedes PNG aus seinen Pixeln. Die Tabelle zeigt für das ausgewählte Bild Soll-/Istwerte und Ablehnungsgründe. Ergebnisse werden separat unter `tests/runs/` gespeichert; Die Bedienoberfläche bleibt während der Testübergabe und Analyse reaktionsfähig; nach einer Sekunde erscheint bei noch laufender Arbeit ein Busy-Indikator. Kürzere Läufe zeigen keinen. Nach Abschluss erscheint eine Meldung mit **Testergebnisse in Windows-Explorer anzeigen**; der Button öffnet den Laufordner und markiert `results.json`. Fehler beziehungsweise gespeicherte Teilergebnisse nach Abbruch werden ausdrücklich bezeichnet. Auftrag und Ergebnisse bleiben unabhängig vom temporären Cache erhalten. Der erste Analyseweg ist experimentell; siehe [API und Grenzen](../docs/analyse-api.md).
6. Optionen einschließlich Serienanzahl und Abdeckungsmodus speichern und wieder laden. Eine einzelne exportierte Aufnahmebeschreibung lädt ihre exakte Farbpalette und erzeugt das betreffende Einzelbild erneut. Frühere IroGen-Dateien der Version 1.0 bleiben unterstützt.

Die erzeugte Serie wird vor dem dauerhaften Speichern in einem eigenen temporären Ordner gehalten. Nur die ausgewählte Vorschau wird geladen; nicht alle hochauflösenden Bilder gleichzeitig im Arbeitsspeicher halten. Beim normalen Schließen wird der Cache entfernt. Deshalb gewünschte Bilder vorher mit **Alle speichern** sichern oder als Testauftrag übergeben. Bei abgebrochener neuer Erzeugung bleibt die zuvor fertige Serie erhalten.

### Abdeckung der Farbabstände

Bei mehr als einem Bild ist standardmäßig **gesamten Abstandsbereich abdecken** eingeschaltet. Die Bilder verteilen sich möglichst gleichmäßig auf elf Bereiche: exakt 0; über 0 bis 0,5; bis 2; bis 5; bis 10; bis 20; bis 40; bis 60; bis 80; bis 100; über 100. Ab elf Bildern ist jeder Bereich vertreten, bei 100/500 entsprechend mehrfach. Kleine Serien enthalten eine Auswahl über die Spanne. Die Reihenfolge und Grundfarbtöne sind zufällig und über den Startseed reproduzierbar.

Je Bild wählt der Generator ein Bezugsfeld und konstruiert eine Wandfarbe mit passendem nominalem Abstand. Quantisierte Farbwerte werden gegen den vorgesehenen Bereich geprüft und doppelte Paletten neu erzeugt. Alle Felder erhalten ihren tatsächlichen nominalen ΔE00-Wert aufgedruckt. Die festen Einzelbild-Optionen für Bezugsfeld und Wandabweichung sind in diesem Modus deaktiviert; Geometrie, Feldanzahl, Helligkeitsabstufung und Störregler gelten weiter. Ausschalten des Abdeckungsmodus ermöglicht Serien mit der fest eingestellten Wandabweichung.

Diese elf Bereiche sind eine Testverteilung, keine Wahrnehmungsskala. ΔE00 ist nicht auf 100 begrenzt. Die Serie behauptet keinen universellen oberen Endwert; angezeigt wird der tatsächlich erreichte Bereich. Beim geprüften Beispiel mit 500 Bildern reicht er von 0 bis 112,91. Sehr große Abstände benötigen geeignete gesättigte Farbpaare; daher ist die Grundtonverteilung innerhalb eines Abstandsbereichs nicht zwangsläufig gleichverteilt über alle Farbtöne.

Wandgröße bedeutet die Pixelgröße der sichtbaren Wand beziehungsweise des gesamten Bilds. Streifenbreite bezieht sich auf die quer zum Streifen liegende Bildachse, Streifenlänge auf die Längsachse. Position und Ausrichtung sind unabhängig. Die Felder sind wie auf den Vorlagen rechteckige Farbflächen, keine zwingend geometrischen Quadrate. Eine Drehung kann den Streifen anschneiden; der Nahmodus erzeugt das absichtlich.

Einstellbar sind Feldzahl (1–20), Streifenmaße, Rand- und Feldabstände, gleichmäßige oder unterschiedliche Feldhöhen, abgerundeter Abschluss, Beschriftung und Farbabstufung. Sehr viele Felder begrenzen die HSL-Abstufung automatisch auf den verfügbaren Helligkeitsbereich, damit keine identischen Endfarben durch Clipping entstehen. Der Serien-Abdeckungsmodus nutzt für extreme Abstände einen größeren Helligkeits-/Sättigungsbereich als der bisherige Einzelbildmodus.

Die Wand übernimmt das gewählte Bezugsfeld exakt oder weicht ungefähr um ΔE00 1, 4 beziehungsweise 12 ab. Diese drei Zahlen beschreiben ausschließlich Generierungsziele, keine allgemeinen Wahrnehmungs- oder Freigabegrenzen. Der tatsächlich quantisierte Sollabstand steht auf dem Feld und in der JSON-Datei. „Andere Farbfamilie“ verschiebt den Farbton um 180°.

## Störungen

Glanzlicht, Verschmutzung, unscharfer Fokus, Bewegungsunschärfe, Licht/Schatten, Perspektive, Wandstruktur, Bildrauschen, Randabdunklung, Verdeckung und Schleier sind jeweils aus, leicht, mittel oder stark einstellbar. Zusätzlich gibt es Drehung, simulierten Nah-/Fernabstand und Belichtung von −3 bis +3 EV. Effekte lassen sich kombinieren.

- Fokusunschärfe: drei separierbare Boxfilter als Gauß-/Defokusnäherung. Bewegungsunschärfe: horizontaler Linienfilter. Beides skaliert mit der Bildauflösung und wirkt auch auf den gedruckten Text.
- Perspektive: projektive Verjüngung und seitliche Verkürzung; unabhängig von der Drehung in der Bildebene.
- Beleuchtung und Belichtung wirken im linearen RGB. Glanz, Schleier, Flecken, Verdeckung und Rauschen sind einfache synthetische Bildmodelle, keine physikalische Kamera- oder Materialsimulation.
- Nah und fern skalieren die Streifengeometrie um 3,4 beziehungsweise 0,18; keine Aussage über reale Zentimeter oder die sichere Erkennbarkeit durch Iro.
- Die üblichen Systemschriftdateien beeinflussen den Text. Für langfristige Regressionstests die erzeugten Original-PNGs aufbewahren; keine plattformübergreifende Pixelidentität des WPF-Schriftrasters behaupten.

## Bedeutung der Sollwerte und Export

Aufdruck und Tabelle zeigen **nominale ΔE00 zur ursprünglichen Wandfarbe vor Störungen**. Berechnung: quantisiertes 8-Bit-sRGB → lineares RGB → XYZ D65 → Lab D65 → CIEDE2000 mit kL = kC = kH = 1. Zwei Nachkommastellen im Aufdruck, ungerundete Werte in JSON. Die eigene Generatorimplementierung ist unabhängig vom späteren Iro-Rechenkern und gegen den veröffentlichten Sharma-/Wu-/Dalal-Prüfsatz getestet.

Ein Glanzlicht oder Schatten ändert Bildpixel. Der ursprüngliche Sollabstand ist dann kein automatisch gültiger Sollmesswert für die gestörte Aufnahme. Der Generator entscheidet deshalb nicht pauschal, welche Störstärke Iro ablehnen muss. Soll-Ist-Analyse und Festlegung der geprüften Freigaben erfolgen im Iro-Testablauf.

Ein Serienexport enthält zusätzlich `series.json` mit Aufnahmeverweisen und tatsächlicher Abstandsverteilung gemäß [Bildserie v1](../tests/schemas/bildserie-v1.schema.json). Ein Testauftrag ergänzt `request.json` gemäß [Testauftrag v2](../tests/schemas/testauftrag-v2.schema.json). Die Eingaben bleiben von den tatsächlichen [Analyseergebnissen v1](../tests/schemas/analyse-lauf-v1.schema.json) getrennt. Frühere Testaufträge v1 sind weiterhin lesbar.

PNG und JSON besitzen denselben Basisnamen und erfüllen [Aufnahmeformat v1](../tests/schemas/aufnahme-v1.schema.json). `conditions.generator.parameters` enthält sämtliche Optionen, Wand- und Feldfarben, nominale Abstände, Rangfolge sowie projizierte Feldeckpunkte. Die Rechtecke unter `bounds` sind zugeschnittene Umhüllungen, **keine inneren Messmasken**. Abrundung, Text und Verdeckung müssen beim späteren Festlegen einer Messmaske berücksichtigt werden.

`expected.verification` bleibt `proposed`; bildbezogene Sollabstände, Messfreigaben und Referenzplatzierung bleiben unbekannt (`null`), bis der Sollbefund geprüft wurde. Die numerischen Material-Sollwerte stehen ausdrücklich getrennt unter `nominalValues`. Es werden keine Ist-Ergebnisse angelegt oder frühere Exporte überschrieben. Abgebrochene Exporte entfernen ihr neu angelegtes Paket wieder, statt ein unvollständiges Paar zu hinterlassen.

## Prüfungen

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore
```

Für zusätzliche lokale visuelle Prüfarbeitsdateien:

```powershell
$env:IROGEN_VISUAL_OUTPUT = 'D:\Source\iro\artifacts\irogen-review'
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore --filter FullyQualifiedName~VisualSmokeTests
Remove-Item Env:IROGEN_VISUAL_OUTPUT
```

Die Tests prüfen veröffentlichte Farbpaare, Identität/Symmetrie, sRGB-Referenzfarben, reproduzierbare Pixel, gerenderte Feldinneren, Störwirkungen, Geometrie, Eingabegrenzen und PNG-/JSON-Wiedergabe. Ein WPF-Test öffnet die Oberfläche, wartet auf die echte Vorschau und rendert zwei Fenstergrößen. Die optionalen Prüfarbeitsdateien sind keine abgenommenen Iro-Datensätze.

Quelle der 34 CIEDE2000-Paare: [Sharma, Wu, Dalal – veröffentlichte Testdaten](https://hajim.rochester.edu/ece/sites/gsharma/ciede2000/dataNprograms/ciede2000testdata.txt). Lokale unveränderte Kopie unter `tests/iro.gen.tests/ciede2000testdata.txt`.

Zusätzliche Serien- und Übergabeprüfungen: vollständige Erzeugung von 100 und 500 Bildern, Verteilung auf alle elf Bereiche, unterschiedliche Paletten, Wiedergabe gespeicherter Paletten, Abbruchbereinigung und unabhängige Kopien für Speicherung/Testauftrag. Exporte einschließlich Serienmanifest und Testauftrag prüfen:

```powershell
./tests/iro.gen.tests/Test-Exports.ps1 -Directory 'D:\Pfad\zur\exportierten\Serie'
```

Die Tests können über `IROGEN_SERIES_OUTPUT` lokale Kopien der 100-/500-Bild-Serien und über `IROGEN_HANDOFF_OUTPUT` einen isolierten Beispielauftrag für die Schemaprüfung behalten. Dies sind Prüfartefakte, keine fachlich freigegebenen Iro-Abnahmedatensätze.



## Testpläne und kompakter Bericht

Über **Testplan laden** die Datei [bildqualitaet-und-abstand.json](testplans/bildqualitaet-und-abstand.json) auswählen: 113 Bilder in 35 Fällen. Anschließend **An Iro-Tests senden**, **Testergebnisse auswerten** und **Kompakten Bericht exportieren**. Format, Entfernungsstufen und Prüfstand: [Dokumentation](../docs/testplaene-und-verdeckung.md).

Seit der Ablaufkorrektur vom 22. September genügt nach **Testplan laden** der Button **Testergebnisse auswerten**. Er analysiert die aktuelle Serie bei Bedarf und öffnet genau deren Lauf; ältere gespeicherte Ergebnisse werden dabei nicht beigemischt.


## Räumliche Aufnahmen und Freihandkombinationen (Generator 1.3.0)

Neu sind zufällige Position und Drehung, Seitenblick sowie Blick von oben/unten (jeweils leicht/stark/sehr stark) und ein simulierter Abstand des Streifens zur Wand. Höhenblick kann zufällig oder ausdrücklich von oben/unten erfolgen. Effekte lassen sich mit wenig Licht, Bewegungsunschärfe und allen bisherigen Störungen kombinieren.

Der zusätzliche [Testplan für Raumlage, Perspektive und Wandabstand](testplans/raumlage-perspektive-und-wandabstand.json) erzeugt **108 Bilder in 33 Fällen**, einschließlich Hochhalten und Bücken. Über **Testplan laden** auswählen, danach an Iro-Tests senden und den kompakten Bericht exportieren. Frühere Optionsdateien der Generatorversionen 1.0.0 bis 1.2.0 bleiben lesbar.

Die Zentimeterklassen repräsentieren synthetische Szenarien, keine aus Bildern gemessenen Abstände. Modell, vollständiger Ablauf und Prüfstand: [Räumliche Aufnahmeszenarien](../docs/raeumliche-aufnahmeszenarien.md).
Gezielter Konturtest ab Generator 1.4.0: [JSON-Testplan](testplans/perspektivkorrektur-konturpruefung.json) und [Anleitung mit Sollbefunden](testplans/perspektivkorrektur-konturpruefung.md). Die optionale JSON-Einstellung fieldWidthFactors enthält je Farbfeld einen Faktor von 0,3 bis 1; Felder bleiben zentriert. Ohne Angabe bleibt die bisherige Breite erhalten. Die Einstellung wird mit Optionen und Metadaten gespeichert; ein eigener UI-Regler ist nicht vorgesehen. Der Probelauf ist wegen einer dokumentierten kombinierten Fehlfreigabe noch keine vollständige Abnahme.
