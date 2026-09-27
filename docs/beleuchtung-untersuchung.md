# Beleuchtung: Untersuchung und begrenzte Schutzkorrektur

Stand: 23. September 2026; Planfassung 1.39; Analyse 0.5.6. Keine Nutzerabnahme, keine allgemeine Beleuchtungsfreigabe.

## Fragestellung und vorab festgelegte Erwartungen

Starke räumliche Ungleichmäßigkeit innerhalb einer Messfläche macht den gemeinsamen Vergleich unsicher. Nach Nutzerentscheidung lieber ablehnen als eine Unsicherheit rechnerisch zu kaschieren. Die physische Ursache ist damit nicht bewiesen: Licht, Materialverlauf oder Reflex können ähnliche Pixel erzeugen. Große Unterschiede ZWISCHEN homogenen Materialfeldern sind dagegen kein hinreichender Ablehnungsgrund.

Unabhängige Kernfixtures erzeugen 800 × 600 Pixel mit drei homogenen Materialfeldern und einer grauen Wand. Für die Simulation wird lineares RGB mit einem ortsabhängigen Lichtfaktor multipliziert, dann in sRGB kodiert und gerundet. Das ist ausschließlich eine Vorwärtssimulation der Aufnahme, keine in Iro eingebaute Korrektur. Keine Sollfarben und keine Lichtparameter gelangen in den Analyzer. Ein farbkanalabhängiger Faktor dient als vereinfachtes farbiges Lichtfeld, nicht als spektrales Kameramodell.

Acht Szenarien, jeweils senkrecht/waagerecht und ohne/mit festem Rauschen ±2 (Seed 72931): gleichmäßig; gemeinsame Halbierung des Lichts; geringer Verlauf 0,97–1; Querverlauf 0,2–1; Längsverlauf 0,2–1; Schattenkante 0,35/1 durch die Messflächen; gegenläufige rote/blaue Lichtfaktoren; Schattenkante zwischen Wandreferenz und Streifen bei x=540. Insgesamt 32 Untersuchungsbilder. Gleichmäßige Kontrollen einschließlich gemeinsamer Abdunklung und der konkret geringe Verlauf sollen drei Felder liefern. Starke Querverläufe dürfen keine Teilfreigabe liefern. Weitere gestörte Konstellationen werden ausdrücklich ergebnisoffen protokolliert, ohne deren beobachtete Freigabe nachträglich zur Sollvorgabe zu machen.

Zusätzlich: zwei direkte Messflächen-Gegenproben für neutrale/farbige starke Verläufe und zwei Nachweise pixelidentischer Eingaben mit unterschiedlichen physikalischen Interpretationen. Helligkeit ist das Produkt aus Material und Beleuchtung; dieselben Pixel können durch andere Materialfarben bei gleichmäßigem Licht entstehen. Die automatische Messung kann diese Interpretationen ohne zusätzliche verlässliche Information nicht unterscheiden. Der Test verlangt daher identische Auswertung, NICHT deren Freigabe.

## Reproduzierter Fehler und Korrektur

Analyse 0.5.5 gab in drei von vier Querverlauf-Konstellationen noch zwei von drei Feldern frei, obwohl die vorhandene Flächenprüfung bereits eine starke Ungleichmäßigkeit festgestellt hatte. Drei eigens vor der Änderung ausgeführte Regressionsfälle scheiterten. Der vierte Fall wurde bereits wegen mehrdeutiger Geometrie gesperrt.

Analyse 0.5.6 erweitert die vorhandene aufnahmeweite Sperre: Überschreitet die räumliche Qualitätsstatistik eines Feldinneren, seiner größeren geprüften Innenfläche oder seiner Wandreferenz den bestehenden Grenzwert, gibt das gesamte Bild keine Vergleiche und keinen nächsten Farbtreffer frei. Keine Änderung der Pixel, keine neue Farbkorrektur, keine nach unten gedrehten Grenzwerte. Clipping und Unschärfe behalten die bisherige Hinweispriorität. Der bestehende experimentelle Grenzwert von 2 ΔE00 zwischen räumlichen Teilflächen ist KEINE Genauigkeitszusage und keine allgemein validierte Beleuchtungsgrenze.

Hinweis:

> Messflächen sind zu ungleichmäßig für einen zuverlässigen Vergleich. Bitte für gleichmäßige Beleuchtung und einheitliche Messflächen sorgen und erneut aufnehmen.

Die Sperre gilt bewusst auch bei einer starken Materialungleichmäßigkeit in einer Messfläche. Sie behauptet keine sicher erkannte Schattenursache. Andere Ablehnungsgründe und gute homogene Materialfarben werden dadurch nicht pauschal gesperrt.

## Ergebnisse und offen gebliebene Befunde

- Alle vier starken Querverläufe geben jetzt keine Messwerte frei; drei bisherige Teilfreigaben geschlossen.
- Acht Kontrollen ohne Abdunklung (gleichmäßig und geringer Verlauf, jeweils beide Richtungen und Rauschstufen) geben weiterhin drei Felder frei.
- Starker Längsverlauf, Schattenkante durch Messflächen und farbiges Lichtgefälle bleiben vollständig gesperrt.
- **Nicht gelöst:** Beim Schattenübergang zwischen den ausgewählten Flächen sind diese jeweils homogen. Alle vier Konstellationen geben weiterhin drei Felder frei, obwohl das bekannte Vorwärtsmodell unterschiedliche Beleuchtung erzeugt. Kein universeller Beleuchtungsschutz; keine automatische Korrektur oder pauschale Helligkeitssperre als scheinbare Lösung.
- **Zusätzlicher Erkennungsfehler:** Bei gemeinsamer Halbierung des Lichts, waagerechtem Streifen und Rauschen ±2 werden nur zwei statt drei Felder erkannt. Der Ergebnisstatus lautet dennoch Measured, weil er sich auf erkannte Felder bezieht. Vor und nach dieser Korrektur vorhanden. Die vorab erwarteten drei Felder bleiben im Bericht als nicht erfüllt ausgewiesen. Untersuchungstest bestanden bedeutet nicht, dass diese Kontrollerwartung erfüllt ist. Die Feldvollständigkeit ist als Folgearbeit offen.

Der [Vorherbericht der endgültigen Gegenproben](../tests/adjustments/beleuchtung-20260923/vorher-gezielte-gegenproben.md) und der [aktuelle Untersuchungsbericht](../tests/adjustments/beleuchtung-20260923/bericht.md) erlauben den Vergleich. Die zusätzliche Datei vorher.md ist ein früher explorativer Stand mit Schattenkante x=520, die noch die Wandmessfläche schneidet; sie ist für diesen Schattenfall kein identischer Vorhervergleich.

## Gezielte Prüfungen

- 81 Kernprüfungen bestanden: Beleuchtungsgegenproben, räumliche Qualität, Messregeln, Pixelanalyse, Unschärfe und Geometrieschutz. Darin 13 neue Beleuchtungs-Testfälle einschließlich der ergebnisoffenen 32-Bilder-Untersuchung. Keine übersprungenen Tests.
- 16 Generator-/Integrationsprüfungen bestanden: neuer Beleuchtungsplan, Anzeige, Analyseintegration und Geraderichten. Eine zunächst aufgedeckte Änderung des Status bei gleichzeitigem Unschärfe-/Referenzbefund wurde korrigiert; die bisherige Unschärfeanzeige bleibt erhalten.
- Verpflichtende Zwei-Bilder-Zwischenkontrolle nach der endgültigen Korrektur bestanden: sauberes Bild und bekanntes leichtes Rauschen, jeweils Freigabe, Zuordnung, Werte und Rangfolge.
- Android-Release-Build bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Keine Geräteausführung.
- Abschließender Gesamt-Testlauf, reale Gerätetests und ausdrückliche Abnahmen bleiben ausstehend. Schritt „Vergleichbare Beleuchtung prüfen“ bleibt insgesamt offen.

## Ladbarer Gegenlauf in Iro-Gen

[beleuchtung-schutzpruefung.json](../iro-gen/testplans/beleuchtung-schutzpruefung.json) enthält acht Bilder: zwei Richtungen × ohne/mit leichtem Rauschen × ohne/mit starkem Schattenübergang. Vier Kontrollen erwarten drei freigegebene Felder ohne Qualitätshinweis, vier Schattenfälle vollständige Sperre und Qualitätshinweis. Diese Erwartungen wurden vor dem Lauf festgelegt. Der bestehende Generator setzt einen diagonalen weichen Schattenübergang bis zum linearen Lichtfaktor 0,34; dieser Plan prüft weder alle acht unabhängigen Kernmodelle noch den unentdeckten Schatten zwischen homogenen Flächen.

Der echte PNG-/Übergabe-/Analyse-/Exportlauf erfüllte **8 von 8 Erwartungen**, siehe [Generatorbericht](../tests/adjustments/beleuchtung-generator-20260923/bericht.md). Das ist ein begrenzter Gegenlauf, keine umfassende Beleuchtungs- oder Farbgenauigkeitsabnahme.

Iro-Gen neu starten → „Testplan laden“ → diese JSON wählen → Generierung abwarten → „An Iro-Tests senden“ → Analyse abwarten → „Testergebnisse auswerten“ → exportieren. Im neuen Lauf muss Analyse 0.5.6 stehen. Die bekannten offenen Kernbefunde werden durch einen grünen Acht-Bilder-Lauf nicht erledigt.

**Nutzer-Gegenlauf am 24. September 2026:** [Übermittelter Acht-Bilder-Bericht](../iro-gen/testplans/iro-testbericht-20260924-111222.md), IroGen 1.5.0 / Analyse 0.5.6: 8/8 verarbeitet, acht Verhaltenserwartungen erfüllt, keine Prüf- oder Verarbeitungsfehler. Vier Kontrollen jeweils 3/3 Felder ohne Qualitätshinweis; vier starke Schattenfälle vollständig gesperrt mit Hinweis auf ungleichmäßige Messflächen. Beide Streifenrichtungen und jeweils ohne/mit leichtem Rauschen enthalten. Keine Teilfreigaben. Bestätigt den begrenzten Generator-Gegenlauf, nicht die allgemeine Beleuchtungserkennung oder die drei unabhängigen Querverlauf-Regressionen erneut. Die offene Schattenkonstellation zwischen homogenen Messflächen und das fehlende Feld bei gemeinsamer Abdunklung bleiben ungetestet durch diesen Bericht und weiterhin offen. Keine ausdrückliche Abnahme; keine neuen lokalen Tests allein für diese Dokumentation.

## Fortschreibung am 24. September 2026

Der oben für Analyse 0.5.6 dokumentierte konkrete Fehler mit nur zwei statt drei Feldern ist in Analyse 0.5.7 gezielt behoben; siehe [Ursache und Prüfungen](felderkennung-rauschstart.md). Der historische Befund bleibt erhalten. Die Schattenlücke zwischen homogenen Messflächen bleibt unverändert offen.
