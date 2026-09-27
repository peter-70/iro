# Unschärfe strikt ablehnen: rauschbedingte Schutzlücke

Stand: 23. September 2026. Planfassung 1.37; Analyse 0.5.5, IroGen 1.5.0.

## Verbindliche Anforderung

Erkannte unbrauchbare Fokus- oder Bewegungsunschärfe sperrt die gesamte Aufnahme: keine freigegebenen Abstände, keine nächste Farbe und keine stehen gebliebenen Messwerte. Keine Entschärfung, Rekonstruktion oder sonstige Rettung verlorener Farbinformation. Der Nutzer erhält:

> Bild unbrauchbar: zu unscharf. Bitte erneut aufnehmen. Kamera ruhig halten und neu fokussieren.

Die Eignung wird am Originalbild beurteilt. Geraderichten ist weiterhin zulässig; Aufhellung bleibt auf den Erkennungspfad beziehungsweise ein gesondert validiertes gemeinsames Modell beschränkt. Diese Entscheidung aktiviert keine neue Aufhellung oder Farbkorrektur.

## Vorher reproduzierter Fehler

Unabhängige RGB-Bilder mit drei Feldern wurden über eine 51 Pixel breite Boxmittelung horizontal verschmiert; für den Fokusfall zusätzlich vertikal. Anschließend wurde Pixelrauschen bis ±6 Codewerte hinzugefügt. Beide Streifenrichtungen wurden geprüft. Diese Reihenfolge modelliert optische Unschärfe mit anschließendem Rauschen; sie unterscheidet sich vom bestehenden Generator, der Rauschen vor der Unschärfe aufbringt.

Unter Analyse 0.5.4 scheiterten vier von zwölf ursprünglichen Gegenproben:
- Drei stark verschmierte Bilder mit Rauschen gaben noch Messwerte frei.
- Ein weiteres wurde bereits wegen ungleichmäßiger Messflächen gesperrt, erkannte aber die Unschärfe nicht.
- Vier verschmierte Bilder ohne Rauschen und vier scharfe Kontrollen bestanden.

Ursache: Die Kantenprüfung betrachtete eine einzelne Pixellinie. Rauschspitzen erzeugten große Nachbarsprünge und täuschten damit eine scharfe Kante vor.

## Gezielte Korrektur

Analyse 0.5.5 bildet das Kantenprofil aus einem schmalen Streifen parallel zur Kante, maximal 17 Originalpixel pro Profilposition. Dadurch wirken einzelne Rauschspitzen weniger stark. Quer zur Kante wird nicht gemittelt. Die bisherige Prüfung auf breite Übergänge verwendet weiterhin den Kontrast von mindestens zehn Codewerten und die bestehende Mindestkonzentration; eine erkannte unbrauchbare Kante sperrt weiterhin die ganze Aufnahme.

Diese Mittelung ist ausschließlich eine Qualitätsstatistik. Sie erzeugt kein bearbeitetes Messbild und verändert weder gemessene Wand- noch Feldfarben. Die vorhandene Originalpixelmessung bleibt erhalten.

Die Bandbreite ist ein begrenzter technischer Versuchswert, keine universelle Kameragrenze. Die Korrektur schließt die reproduzierte Rauschmaskierung, beweist jedoch nicht die Erkennung jeder denkbaren Unschärfe bei beliebigen Auflösungen, Texturen, Kontrasten oder Rauschmustern.

## Gezielte Prüfungen

17 neue Kern-Testfälle enthalten:
- Acht ursprüngliche Unschärfefälle mit/ohne Rauschen und beiden Richtungen: vollständige Sperre mit Unschärfehinweis.
- Vier scharfe Gegenfälle mit/ohne demselben Rauschen: jeweils drei freigegebene Felder.
- Vier Fälle mit lediglich drei Pixel breiter Kantenerweichung: weiterhin drei freigegebene Felder, deren gemessene Innenfarben mit den bekannten unveränderten Originalfarben übereinstimmen.
- Eine zusätzliche Testmethode mit acht Varianten aus zwei weiteren Rauschseeds, beiden Richtungen und beiden Unschärfemodellen: keine Messfreigabe. Einzelne Varianten scheitern bereits an Muster-/Flächeneignung; der Test verlangt dort keinen erfundenen Unschärfegrund.

Zusammen mit den angrenzenden Geometrie-, Einzelbild- und aufnahmeweiten Schutztests:
**63 Kernprüfungen bestanden**, keine übersprungen.

Zusätzlich **15 gezielte Generator-/Integrationsprüfungen** für Geraderichten, PNG-Analyse und die tatsächliche Android-Hinweisanzeige bestanden. Keine gesamte Test-Suite ausgeführt.

## Ladbarer IroGen-Nachweis

[unschaerfe-strenge-ablehnung.json](../iro-gen/testplans/unschaerfe-strenge-ablehnung.json) erzeugt zwölf Bilder:
- Vier scharfe Kontrollen, senkrecht/waagerecht, ohne/mit leichtem Rauschen.
- Acht stark unscharfe Bilder, Fokus-/Bewegungsunschärfe, beide Richtungen, ohne/mit leichtem Rauschen.

Vorab hinterlegte Erwartungen: Kontrollen jeweils drei freigegebene Felder; starke Unschärfe jeweils vollständige Sperre. **Zwölf Erwartungen erfüllt, null nicht erfüllt, null nicht bewertet, null Prüffehler.** [Kompakter Bericht](../tests/adjustments/unschaerfe-20260923/bericht.md).

Dieser Generatorplan ergänzt den unabhängigen Kerntest. Er reproduziert nicht die spezielle Reihenfolge „Unschärfe vor Rauschen“ der gefundenen Lücke. Diese ist durch BlurSafetyTests abgedeckt. Der maschinenlesbare Haupt-Hinweis für die Sperrfälle ist derzeit der allgemeine Code Other; die konkrete Unschärfeformulierung wird zusätzlich in Kern- und Anzeigeprüfungen kontrolliert.

Nutzerlauf: IroGen mit aktueller Analyse neu starten, JSON über „Testplan laden“ laden, an Iro-Tests senden, auswerten und den Bericht exportieren.

## Verpflichtende Zwischenkontrolle

Die Zwei-Bilder-Kontrolle nach der Änderung ist grün: sauberes und leicht verrauschtes Bild erfüllen Freigabe, Anzahl, Feldzuordnung, Werttoleranzen und Rangfolge. Der Lauf ist unter tests/adjustments/zwischenkontrolle-20260923 gesichert.

~~~powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter "FullyQualifiedName~BlurSafetyTests|FullyQualifiedName~MeasurementRuleTests|FullyQualifiedName~PixelAnalysisTests|FullyQualifiedName~GeometrySafetyTests" --verbosity minimal
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter "FullyQualifiedName~StraighteningTests|FullyQualifiedName~AndroidPresentationTests|FullyQualifiedName~AnalysisIntegrationTests" --verbosity minimal
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~BlurPlanTests --verbosity minimal
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --verbosity minimal
~~~

- [x] Reproduzierte Rauschmaskierung gezielt behoben.
- [x] Gezielte Kern-/Integrationsprüfungen und Zwölf-Bilder-Plan bestanden.
- [x] Verbindliche Zwei-Bilder-Kontrolle bestanden.
- [ ] Reale Unschärfegrenzen und vollständige Störklassenabdeckung geprüft.
- [ ] Schluss-Sammel-Testlauf und Gerätetests.
- [ ] Ausdrückliche Nutzerabnahme am Schluss.

Keine vollständige Abnahme des übergeordneten Bildqualitätspunkts.
Android-Release-Build nach der Korrektur erfolgreich: 0 Fehler, 8 bekannte XAML-Bindungswarnungen (XC0022). Kein Gerätetest.
[Zwei-Bilder-Bericht nach der Korrektur](../tests/adjustments/zwischenkontrolle-20260923/iro-run-c68eb262e96243d2b784115e12e3751b/bericht.md)
