# Historisch und aufgehoben: Bildfolgen-Historienvertrag

**Aufgehoben am 23. September 2026 durch die ausdrückliche Nutzerentscheidung in Planfassung 1.36.** Der unten beschriebene Prototyp und seine 17 Tests wurden entfernt. Keine offene Pflicht zur Wiederherstellung, Feldverfolgung oder Anzeigeintegration. Aktuell gilt ausschließlich die unabhängige Einzelbildauswertung. Der folgende Text bleibt als historischer Nachweis des damaligen Versuchs erhalten und ist kein gültiger API-Vertrag.

# Bildfolgen: Zuordnung und Historienkern

Stand: 23. September 2026. Planfassung 1.35. Interner .NET-Vertrag FrameMeasurementHistory, Version 1. Kein neues JSON-Format; bestehende PNG-API und Ergebnisdateien unverändert. Einzelbildanalyse weiterhin 0.5.4.

## Umgesetzter Teilpunkt

Der Kern enthält jetzt eine getrennte, noch nicht an die Kundenanzeige angeschlossene Historienkomponente. Sie verarbeitet ausschließlich ΔE00-Vergleiche, die zuvor innerhalb desselben Originalframes von Iro berechnet wurden. Sie verändert keine Bildfarben.

1. BeginFrame übernimmt einen Original-RGB-Frame und kopiert dessen Pixel, bevor der Erzeuger seinen Puffer wiederverwenden darf. Während des Aufrufs muss der Erzeuger den Puffer unverändert halten. Gedrehte Erkennungsansichten werden nicht als Original angenommen.
2. Die Arbeit erhält eine interne Besitzerkennung, Messgeneration und fortlaufende Frame-ID. PendingFrameAnalysis.Analyze analysiert genau diese Kopie mit den zugehörigen Optionen. Ein separat erzeugtes Analyseergebnis lässt sich nicht als Ergebnis eines anderen Frames einschleusen.
3. Complete akzeptiert ausschließlich das neueste ausgegebene, noch nicht übernommene Ergebnis derselben Sitzung und Generation. Alte, doppelte oder fremde Ergebnisse ändern den aktuellen Stand nicht.
4. Eine ausdrücklich bestätigte Zuordnung von erkannten zu stabilen Feld-IDs ist erforderlich. Unvollständige oder doppelte Zuordnung löscht die Historie und gibt keine Werte frei. Die pro Bild vergebenen Namen „detected-1“ usw. sind ausdrücklich keine stabilen Identitäten.
5. Je gültiger stabiler Feld-ID werden maximal die letzten fünf gepaarten Abstände samt Frame-IDs gespeichert. Der Rohwert und ein Versuchsmedian sind getrennt auslesbar. Bei weniger als fünf Werten wird der Median der vorhandenen Werte nur als Diagnose berechnet; daraus entsteht keine Freigabe- oder Wartezeitregel für die Kundenanzeige.
6. Ungültige Aufnahmen leeren alle Historien; ein ungültiges oder verlorenes einzelnes Feld verliert nur seine eigene Historie. Wieder auftauchende Felder beginnen neu.
7. Ein Quellen-, Referenz-, Transformations- oder Aufnahmebedingungswechsel beginnt eine neue Generation. Analyseoptions- und Bildgrößenwechsel werden ebenfalls erkannt. Reset verwirft laufende Arbeit und alle Historien, beispielsweise bei Abbruch oder Lebenszykluswechsel.

Die stabile Feldzuordnung und die Revision tatsächlich angewandter Aufnahmebedingungen sind **Eingabeverantwortung des künftigen Trackers beziehungsweise Frame-Adapters**. Die Komponente erkennt diese Änderungen nicht selbst aus Kameradaten und prüft nicht, ob der Aufrufer eine falsche Identität behauptet. Keine automatische Gleichsetzung anhand der Feldnummer. Die Tests nutzen ausschließlich geometrisch bekannte Szenen.

## Zeitvertrag

Alle Zeitwerte sind monotone Zeitspannen derselben Empfangsuhr, keine lokalen Kalenderzeiten.

- firstReceivedAt muss beim ersten Bildempfang erfasst und BeginFrame in Empfangsreihenfolge aufgerufen werden. BeginFrame ist die Aufnahmeannahme, nicht eine spätere Nachregistrierung aus einer Warteschlange.
- captureTimeInReceiptClock darf nur gesetzt werden, wenn die Aufnahmezeit zuverlässig in diese Zeitbasis übertragen wurde. Unbekannte beziehungsweise unvereinbare Kamerazeitbasen dürfen nicht als passend behauptet werden.
- Fehlt die zugeordnete Aufnahmezeit, wird das Alter ab erstem Empfang gezählt; dies beweist ausdrücklich nicht das tatsächliche Aufnahmealter.
- Bekannte Aufnahmezeiten müssen innerhalb derselben Generation streng steigen; doppelte und ältere Aufnahmen werden abgewiesen. Beim Empfangsalter sind keine Rückschritte der Uhr erlaubt.
- Der bereits im Plan vorhandene Versuchswert 500 ms wird angewandt: Ab diesem Alter liefert der Kern keinen aktuellen Wert mehr. Später Eingang frischt das Ergebnis nicht auf. Auch während laufender Arbeit kann die vorherige Historie ablaufen; sie wird nicht anschließend wiederverwendet.
- ReadCurrent prüft das Alter auch ohne neues Ergebnis. Ein künftiger UI-Adapter muss dies per monotonem Timer aufrufen und die Anzeige leeren. Der Kern besitzt **keinen UI-Timer**; ein einmal ausgelesener Snapshot ist keine unbefristete Anzeigeerlaubnis.

## Gezielte Prüfungen

17 neue Tests mit echten Pixelanalysen prüfen:
- Schutz vor wiederverwendeten Eingangspuffern.
- Fünferfenster aus gepaarten Abständen, unabhängig nachgerechneter Median und zugehörige Frame-IDs.
- Verspätete, doppelte, fremde und durch Reset überholte Ergebnisse.
- Quellen-, Referenz-, Transformations-, Bedingungs-, Analyseoptions- und Größenwechsel.
- Fehlende oder doppelte Feldzuordnung.
- Feldverlust/Wiederauftauchen, Teilfehler, ganze ungültige Aufnahme und Wiederfreigabe.
- Aufnahmealter, Empfangsalter als Rückfall, Ablauf ohne neuen Frame und Ablauf während einer laufenden Analyse.
- Doppelte/ältere Aufnahmezeitstempel und ungültige Zeit-/Kontextangaben.

~~~powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter "FullyQualifiedName~FrameMeasurementHistoryTests|FullyQualifiedName~MeasurementStabilityInvestigationTests|FullyQualifiedName~UnderexposureEvidenceTests" --verbosity minimal
~~~

Alle 28 ausgewählten Tests bestanden (17 Historienprüfungen, drei Stabilitätsprüfungen mit 288 Matrixanalysen, acht Dunkelheitsgegenproben). Danach die [verbindliche Zwei-Bilder-Kontrolle](zwischenkontrolle-zwei-bilder.md) erneut bestanden. Android-Release-Build bestanden: null Fehler, acht bekannte XAML-Bindungswarnungen (XC0022). Kein Gesamttestlauf und keine reale Geräteprüfung.

## Grenzen und nächste Arbeit

Dieser abgegrenzte Historienkern ist umgesetzt und gezielt geprüft; die vollständige Mehrframe-Verarbeitung der Kunden-App ist noch offen. Es fehlen automatische Feldverfolgung, verlässliche Aufnahmeinformationen aus einem Frame-Adapter, reale Toleranzen für relevante Parameteränderungen einschließlich schleichender Veränderungen, Rangbestätigung und UI-Timerintegration. Die Referenzstrategie bleibt in ihrem bislang beschlossenen Umfang unverändert.

Der Versuchsmedian ist noch nicht an die Kundenanzeige angeschlossen und ersetzt keine Qualitätsprüfung. Die Unterbelichtungssperre bleibt offen. Das Kopieren eines ganzen RGB-Frames schützt den Besitz, ist aber noch kein Laufzeit-/Speicheroptimierungsnachweis für die Kamera.

Als nächste Arbeit den Zuordnungsadapter mit bewegten Streifen, Feldverlusten und mehrdeutigen Zuordnungen prüfen; erst auf dieser Grundlage eine vollständige Historien-/Anzeigeintegration angehen.

- [x] Versionierter, isolierter Historienkern umgesetzt.
- [x] 17 gezielte neue Tests und angrenzende Gegenproben bestanden.
- [x] Zwei-Bilder-Kontrolle nach der Änderung bestanden.
- [ ] Vollständige Mehrframe-Verarbeitung inklusive Tracker und Kundenanzeige.
- [ ] Schluss-Sammel-Testlauf und reale Gerätetests.
- [ ] Ausdrückliche Abnahme am Schluss.