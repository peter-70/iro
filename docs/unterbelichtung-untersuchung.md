# Unterbelichtung ohne Kanalanschlag: Untersuchung und Gegenproben
## Aktueller verbindlicher Stand – Planfassung 1.57

Iro bewertet Dunkelheit nicht absolut, sondern anhand des relativen Wand-/Farbfeldvergleichs im selben Bild. Eine pauschale Helligkeitssperre bleibt unzulässig. Der vorhandene IroGen-Plan prüft jetzt alle 16 Bilder verbindlich: vier dunkle Originalfarbkontrollen sowie zwölf ganzbildweit um −1, −2 oder −3 EV abgedunkelte Fälle in beiden Streifenrichtungen, jeweils ohne und mit leichtem Rauschen.

Für die zwölf Belichtungsfälle sind drei geometrisch sicher zugeordnete und freigegebene Felder sowie die vorab aus den expliziten neutralen Farben festgelegte Rangfolge `field-3 < field-2 < field-1` hinterlegt. Es werden keine ΔE00-Materialwerte vor der Belichtung als Sollwerte verwendet. Alle 16 Erwartungen sind erfüllt; keine Aufnahme bleibt unbewertet.

Der erste verschärfte Lauf war rot, weil die neue Erwartung irrtümlich annahm, `field-1` sei wandgleich. Der Plan setzt die Wand ausdrücklich auf sRGB 140 und die Felder auf 60, 100 und 160; deshalb ist `field-3` korrekt am nächsten. Nach Korrektur der Testannahme wurde Iro unverändert erneut geprüft. Das schützt davor, die App an einen falschen Test anzupassen.

Ausgeführt wurden:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~UnderexposurePlanTests --logger "console;verbosity=minimal"
```

Ergebnis nach korrigierter Erwartung: Fehler 0, erfolgreich 1, übersprungen 0, gesamt 1, Dauer 5 s; 16 PNG-Bilder erzeugt und analysiert, 16 Erwartungen erfüllt. Der vorausgehende rote Lauf meldete 12/16 verletzte Erwartungen und diente zur Korrektur der falschen Testannahme.

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~UnderexposureEvidenceTests --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 8, übersprungen 0, gesamt 8, Dauer 191 ms.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter "FullyQualifiedName~UnderexposurePlanTests|FullyQualifiedName~RankingExpectationTests|FullyQualifiedName~TestPlanTests" --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 22, übersprungen 0, gesamt 22, Dauer 5 s.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 1, übersprungen 0, gesamt 1, Dauer 877 ms.

Bewertung nach dem Grundprinzip: In den zwölf synthetischen Belichtungsfällen wurden Wand und Farbstreifen durch dieselbe Ganzbildtransformation beeinflusst; der relative Vergleich funktionierte. Die acht Kerngegenproben zeigen zugleich, dass Quantisierung Information ohne exakten Endpunkt vernichten kann, dieser Verlust aus identischen Einzelbildpixeln aber nicht allgemein von einer echten dunklen Farbe unterscheidbar ist. Deshalb folgt keine neue Korrektur oder Helligkeitsschwelle. Reale Kamera- und Informationsgrenzen bleiben für die Gerätephase offen.

Der nachstehende Text dokumentiert den historischen Untersuchungsstand vor dieser verbindlichen relativen Bewertung.

**Aktualisierung, Planfassung 1.36:** Die nachstehend historisch vorgeschlagene Historien-/Mehrframe-Weiterarbeit ist durch die ausdrückliche Nutzerentscheidung vom 23. September 2026 aufgehoben. Jedes Bild wird unabhängig ausgewertet. Der Historienprototyp wurde entfernt; seine Wiederherstellung ist keine offene Aufgabe. Die Untersuchungsergebnisse bleiben als Entwicklungsbefund erhalten.

Stand: 23. September 2026. Grundlage: Planfassung 1.34, Arbeitsplan „Clipping, Dunkelheit und starke Unschärfe zuverlässig sperren“. Produktanalyse weiterhin 0.5.4, IroGen weiterhin 1.5.0. Keine neue Messsperre oder Bildkorrektur aktiviert.

## Ergebnis und offene Grenze

Die gewünschte Unterbelichtungssperre ist noch nicht vollständig implementiert. Die Untersuchung belegt, warum eine bloße Helligkeits-, Histogramm- oder Anzahl-Tonwerte-Schwelle keine ausreichende fachliche Grundlage wäre:

- Vier unabhängige Szenen mit überwiegend dunkler Originalwand (RGB 10 beziehungsweise 30), drei klar getrennten homogenen Feldern und beiden Streifenrichtungen bleiben messbar. Die Wandmessung entspricht den unveränderten Originalpixeln.
- Homogene Originalflächen mit RGB 6, 10 und 20 bleiben bei der Flächenmessung unverändert und geeignet. Ein enger Tonwertumfang oder geringe Helligkeit allein belegen keine unbrauchbare Aufnahme.
- Ein unabhängiges Vorwärtsmodell mit minus acht Blenden quantisiert sRGB 100 und 101 auf denselben Codewert 2. Kein Kanal erreicht 0 oder 255. Diese Pixel sind zugleich identisch mit einer ursprünglich homogenen digitalen Fläche RGB 2. Der Analyzer erhält keine Information, aus welcher Vorgeschichte die Pixel stammen. Der Test verlangt gleiche Behandlung identischer Eingaben, nicht die dauerhafte Freigabe eines ungeeigneten Kamerabildes.
- Das Beispiel belegt einen Informationsverlust ohne Endpunkt. Es bestimmt weder einen praktischen Belichtungsgrenzwert noch eine sichere Klassifikation aller dunklen Bilder. Minus acht Blenden ist ein mathematisches Gegenbeispiel außerhalb der IroGen-Einstellung, kein neues Generator- oder Kameraprofil.

Bisher prüft RegionSampler Kanalendpunkte, absolute Kanalstreuung, Ausreißeranteil und räumliche Farbunterschiede. Ein eigener validierter Signal-/Rausch- oder Tonwertgütevertrag fehlt. Eine nominale ΔE-Abweichung gegenüber Farben vor dem Abdunkeln liefert diesen Vertrag nicht.

## Reproduzierbarer IroGen-Untersuchungsplan

[Unterbelichtung und dunkle Originalfarben](../iro-gen/testplans/unterbelichtung-und-dunkle-originalfarben.json) enthält 16 Bilder:

| Gruppe | Anzahl | Bewertung |
|---|---:|---|
| Dunkle homogene Originalwand RGB 10/30, senkrechter/waagerechter Streifen | 4 | Vorab geprüfte Erwartung: drei freigegebene Felder |
| Minus eine, zwei und drei Blenden, beide Streifenrichtungen, jeweils ohne/mit leichtem Rauschen | 12 | Untersuchungsfälle ohne bereits gesicherte Freigabe-/Sperrerwartung |

Palette, Seed, Geometrie und Bildgröße sind festgelegt. Die Generator-Belichtung arbeitet in linearem Licht, das Rauschen wird anschließend auf kodierte RGB-Kanäle addiert. Das ist kein kalibriertes Sensor-/ISP-Modell. Die Prüfung darf daraus keine universelle Dunkelheitsgrenze ableiten.

Der vollständige PNG-Übergabe-/Analyse-/Exportlauf ist automatisiert ausgeführt:
**16 verarbeitet, 4 Erwartungen erfüllt, 12 bewusst nicht bewertet, keine Prüf- oder Verarbeitungsfehler.**
Alle zwölf Untersuchungsbilder liefern derzeit jeweils drei Messwerte. Das ist ein dokumentierter Istbefund, keine Bestätigung ihrer Eignung und auch kein alleiniger Beweis einer Fehlfreigabe. Nominale Abweichungen bleiben getrennt.
[Gesicherter kompakter Bericht](../tests/adjustments/unterbelichtung-20260923/bericht.md).

Optionaler Nutzerlauf: IroGen starten, diese JSON über „Testplan laden“ laden, „An Iro-Tests senden“, anschließend „Testergebnisse auswerten“ und Bericht exportieren. Vier erfüllte und zwölf nicht bewertete Erwartungen sind hier beabsichtigt. Dieser Plan ist ein Untersuchungsplan, kein vollständig bewerteter Abnahmesatz. Zur Fortsetzung der Entwicklung ist jetzt kein zusätzlicher Nutzerlauf erforderlich.

## Ausgeführte gezielte Prüfungen

~~~powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~UnderexposureEvidenceTests --verbosity minimal
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~UnderexposurePlanTests --verbosity minimal
~~~

- Acht Kernprüfungen bestanden, keine übersprungen.
- Eine Integrationstestmethode mit 16 tatsächlich erzeugten und analysierten PNGs bestanden.
- Die Integration prüft vier bekannte Freigaben und die Nichtbewertung der zwölf ungeklärten Fälle. Sie schreibt letztere nicht anhand des aktuellen Analyzerverhaltens zu bestandenen Erwartungen um.
- Keine Gesamt-Testreihe und keine Gerätetests in dieser Aufgabe. Der abschließende Sammel-Testlauf bleibt am Ende.

## Weiterarbeit am offenen Schutzpunkt

Vor einer zusätzlichen Sperre müssen unabhängige Qualitätskriterien festgelegt und belegt werden: Welche Instabilität der gemessenen Farbe ist noch zulässig, welche Originalinformation dient als Referenz, und mit welchen Daten lassen sich geeignete dunkle Farben von unbrauchbaren Aufnahmen trennen? Ein bloßer EV-Wert, ein Mittelwert der Bildhelligkeit oder aus nominalen Generatorfarben zurückgerechnete „Wahrheit“ genügt nicht.

Als nächste technische Untersuchung eignen sich kontrollierte Bildfolgen mit gleichbleibender Geometrie und bekannten Störanteilen. Daran lässt sich die Wiederholbarkeit von Wand-Feld-Vergleichen prüfen, ohne Wand und Streifen aus verschiedenen Frames zu vermischen. Stabile, aber systematisch verfälschte oder bereits entrauschte Bilder werden durch Wiederholbarkeit allein nicht als richtig bewiesen. Reale Grenzwerte und kamerainterne Verarbeitung sind später am Gerät zu prüfen. Ein etwaiger zusätzlicher Referenz-/Metadatenvertrag ist vor Nutzung ausdrücklich zu dokumentieren; Testplanerwartungen bleiben außerhalb des Analyzers.

Diese Untersuchung schließt den Schutzpunkt nicht und ändert die Forderung nach einer Unterbelichtungssperre nicht. Sie verhindert eine unbelegte Sofortkorrektur, die dunkle Farben pauschal aussperrt.

- [x] Voruntersuchung und reproduzierbare Gegenproben erstellt.
- [x] Gezielte Kern- und PNG-Prüfungen bestanden.
- [x] Synthetischer relativer Unterbelichtungsvertrag bis −3 EV umgesetzt und geprüft; reale Geräte- und Informationsgrenzen bleiben offen.
- [ ] Schluss-Sammel-Testlauf und reale Gerätetests bestanden.
- [ ] Ausdrückliche Nutzerabnahme am Ende.
- [ ] Übergeordneter Schutzpunkt erledigt.
## Fortsetzung: Messstabilität untersucht

Die [kontrollierten Bildfolgen und der Offline-Medianvergleich](messstabilitaet-untersuchung.md) sind am 23. September 2026 durchgeführt: 288 Matrixanalysen plus Paarungs- und Anzeigegegenproben, drei gezielte Tests bestanden. Ruhige Werte beweisen keine unverfälschte Messung; eine Unterbelichtungsfreigabe wird daraus nicht abgeleitet. Nächster technischer Schritt ist der überprüfbare Eingabe-/Historienvertrag für Bildfolgen.
