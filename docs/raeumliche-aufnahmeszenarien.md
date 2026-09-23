# Räumliche Aufnahmeszenarien in IroGen

Stand: 22. September 2026. Generator 1.3.0, Analyse 0.3.0, Planfassung 1.25.
Grundlage: Nutzerauftrag zu zufälliger Streifenlage, Seiten-/Höhenblick, Wandabstand und schwierigen Freihandkombinationen.

## Bedienung

IroGen neu bauen und starten. Unter **Geometrie & Abstand** stehen:

- **Zufällige Position und Drehung:** ersetzt feste Position und Drehung; Winkel im gesamten Bereich −180 bis +180°. Die Streifenform bleibt durch die gewählte Grundausrichtung bestimmt. Soweit die projizierte Größe es erlaubt, bleibt der Streifen im Bild; übergroße Streifen dürfen angeschnitten sein.
- **Blick von der Seite:** aus, leicht, stark, sehr stark; linke/rechte Seite zufällig.
- **Blick von oben / unten:** aus, leicht, stark, sehr stark. Die Richtung ist standardmäßig zufällig, für reproduzierbare Hochhalte-/Bückfälle auch ausdrücklich von oben oder unten wählbar.
- **Streifen vor der Wand:** anliegend, kleiner Abstand unter 10 cm, größerer Abstand zwischen 10 und unter 20 cm, großer Abstand über 20 cm.
- **Zusätzliche Verjüngung:** bisherige Perspektivoption für ältere Versuche. Sie bleibt kombinierbar, ist im neuen Testplan ausgeschaltet.

Zufall folgt dem Seed. Ein eigener gemischter Zufallsstrom für die Geometrie verändert die bisherige Farbpalette und Bildstörungen nicht. Aufgelöste Winkel, Wandabstand und Projektionsursprung werden zusätzlich zu den Optionen in den Aufnahmemetadaten gespeichert. Der kompakte Bericht nennt bei seinen Beispielen die tatsächliche simulierte Lage.

## Vorbereiteter Testplan

[raumlage-perspektive-und-wandabstand.json](../iro-gen/testplans/raumlage-perspektive-und-wandabstand.json): **108 Bilder in 33 Fällen**.

Er enthält beide Streifenrichtungen, sechs ungestörte Kontrollbilder, zufällige Lagen, alle Perspektiv- und Abstandsstufen sowie ausdrücklich festgelegten Höhenblick. Die Fälle **Hochhalten** und **Bücken** kombinieren die Blickrichtung mit Wandabstand, wenig Licht, Rauschen und Bewegungsunschärfe in drei Stärken. Frontal aufgenommene dunkle Vergleichsbilder helfen, den Einfluss der Geometrie von dem der Beleuchtung zu unterscheiden.

Ablauf: **Testplan laden → Generierung abwarten → An Iro-Tests senden → Analyse abwarten → Testergebnisse auswerten → Kompakten Bericht exportieren.**
Alternativ startet **Testergebnisse auswerten** die noch fehlende Analyse der aktuellen Serie automatisch. Die Anzahl im neuen Lauf muss 108 sein. Der bisherige 113-Bilder-Testplan bleibt separat verfügbar.

## Modell und Grenzen

Die neuen Blickrichtungen verwenden eine invertierbare Lochkameraprojektion der ebenen Streifenfläche. Seitenblick verkürzt die horizontale Kameraachse, Höhenblick die vertikale; beide wirken auch bei gedrehten Streifen. Vorwärtsprojektion für Feldeckpunkte und Rückprojektion für das Raster verwenden dieselbe Geometrie. Die Kamera wird auf die gewählte Bildposition ausgerichtet, damit ein zusätzlicher Wandabstand nicht bloß den ganzen Streifen aus dem Bild schiebt.

Die drei Versuchswinkel sind **15°, 45° und 70°**. Die drei Abstandsklassen werden exemplarisch durch **5, 15 und 30 cm** repräsentiert. Das künstliche Kameramodell setzt eine Wandentfernung von 1,5 m und eine Brennweite von vier langen Bildseiten in Pixeln an. Diese Werte sind technische Generierungsannahmen, keine Iro-Grenzen und keine aus dem Bild bestimmten Entfernungen. Die vorhandenen Nah-/Fernmodi bleiben zusätzliche Skalierungsversuche und sind nicht als metrische Kameradistanzen kalibriert.

Ein Wandabstand verändert die scheinbare Streifengröße und die räumliche Projektion. Zusätzlich entsteht ein versetzter, mit dem Abstand weicherer Wandschatten. Dieser Schatten ist ein vereinfachtes Bildmodell mit fester Lichtrichtung. Wandtextur und weitere Bildstörungen bleiben synthetische Effekte im Bildraum; Materialreflexionen, reale Tiefenschärfe, Handbewegung im Raum und vollständige Lichtausbreitung werden nicht physikalisch simuliert.

Nominale Farbwerte beziehen sich weiterhin auf die ursprünglichen Farben vor Störungen. Die Kombinationen liefern Entwicklungsdiagnosen, keine automatisch geprüften Freigabe-/Sperrvorgaben. Die Geometrieinformationen gelangen ausschließlich in die nachgelagerte Auswertung, nicht als Hilfe in die Bildanalyse.

## Lokaler Prüfstand

Paketwiederherstellung im Locked-Modus erfolgreich; 57 Kern- und 116 Generator-/Ablauftests bestanden. Gesamtbuild in Release erfolgreich, ohne Fehler; acht bereits vorhandene XAML-Binding-Warnungen in der Android-Oberfläche.

- Neue Geometrieprüfungen: Vorwärts-/Rückprojektion einschließlich extremer Kombinationen, beide Streifenrichtungen, Verkürzungsrichtung und Richtungswechsel, reproduzierbare und verteilte Zufallspositionen, sichtbare Wandschatten, Feldpixel gegen projizierte Geometrie sowie Metadaten.
- Vollständige 108-Bilder-Erzeugung, Übergabe an die echte Analyse und kompakter Export ohne Verarbeitungsfehler.
- Ergebnis bei Diagnosegrenze 1 ΔE00: **49 vollständig/genau, 24 teilweise, 30 abgewiesen, 5 nominal abweichend**. Alle sechs ungestörten Kontrollen vollständig/genau.
- Die fünf nominal auffälligen Bilder stammen aus den frontal aufgenommenen Vergleichsfällen mit −2 EV und Rauschen, davon zwei zusätzlich mit Bewegungsunschärfe. Einheitliche Belichtungsänderungen sind aus einem einzelnen Bild nicht allgemein von anderen Materialfarben unterscheidbar. Deshalb keine unbelegte Verschärfung der Analysegrenzen.
- Der vorherige 113-Bilder-Vergleich bleibt durch die bestehenden Regressionstests abgesichert. Die räumliche Qualitätsprüfung aus Analyse 0.3.0 bleibt erhalten.
- Keine erneute Geräteabnahme und kein Nachweis einer universell zuverlässigen Erkennung stark gedrehter Streifen. Die derzeitige Suche bevorzugt annähernd achsenparallele Felder.

Der vollständige lokale Diagnoseexport liegt nach einem Testlauf mit gesetztem IROGEN_SPATIAL_OUTPUT im gewählten Artefaktordner. Beispiel:

~~~powershell
$env:IROGEN_SPATIAL_OUTPUT = 'D:\Source\iro\artifacts\raeumliche-szenarien'
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~SpatialSceneTests
Remove-Item Env:IROGEN_SPATIAL_OUTPUT
~~~
