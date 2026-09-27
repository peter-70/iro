# Zwischenkontrolle: zwei feste Bilder

Stand: 23. September 2026; verbindlich gemäß Planfassung 1.35.

Vor dem Übergang vom technisch geprüften Arbeitspunkt zum nächsten werden beide Bilder neu erzeugt und analysiert. Bei Fehlern zuerst die Regression beheben. Die Kontrolle ergänzt die jeweils passenden Tests; sie beweist nicht, dass an keiner anderen Stelle ein Fehler entstanden ist.

## Festgelegte Gegenfälle

Ladbarer IroGen-Testplan: [zwischenkontrolle-zwei-bilder.json](../iro-gen/testplans/zwischenkontrolle-zwei-bilder.json).

Zwei Bilder mit identischer fester Palette, Geometrie und Seed 12345:
1. Ohne Störungen.
2. Mit leichtem Pixelrauschen (höchstens sechs RGB-Codewerte je Richtung im Generatormodell).

Drei Felder, senkrechter Streifen rechts, 1600 × 1200 Pixel; keine Beschriftung oder zusätzliche Störung. Saubere Geometriekontrollen und die vorherige Messstabilitätsuntersuchung mit Rauschen bis ±6 bilden die Grundlage. „Nachweislich beherrscht“ bezieht sich auf diese reproduzierbaren Fälle, nicht auf beliebige reale Aufnahmen mit der Bezeichnung „leicht“.

Vorab festgelegte Erwartungen: volle Messfreigabe, drei freigegebene Felder und kein Störungshinweis. Der automatisierte Integrationstest prüft zusätzlich:
- Drei zugeordnete Sollfelder, keine zusätzliche Fremdfläche.
- Mindestens 90 % geometrische Überdeckung je Feld.
- Dieselbe nächstliegende Farbe.
- Abweichung vom nominalen Abstand höchstens 0,01 ΔE00 beim sauberen Bild und 0,15 ΔE00 beim begrenzt verrauschten Bild.

Die Zahlen sind enge Regressionstoleranzen dieser festen digitalen Gegenfälle, keine realen Geräte- oder Genauigkeitsgrenzen. Sie werden nicht als allgemeine Sollwerte beliebiger Störbilder verwendet. IroGen prüft im Dialog Freigabe, Anzahl und Hinweis automatisch; die zusätzlichen Zahlen-, Zuordnungs- und Rangprüfungen laufen im Integrationstest.

## Ausführen

~~~powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --verbosity minimal
~~~

Die Prüfung erzeugt beide PNGs, übergibt sie durch den normalen Testauftragsweg an Iro und wertet den exportierten Bericht aus. Pro erfolgreichem Lauf bleiben Bericht und Bilder in einem eigenen Ordner unter tests/adjustments/zwischenkontrolle-20260923 erhalten. Ab dem zweiten Lauf werden zusätzlich eine Kopie des Testplans und Quelltextprüfsummen gesichert.

Für einen manuellen Gegenlauf in IroGen den JSON-Plan laden, an Iro-Tests senden und anschließend auswerten. Manuelles Laden allein ersetzt die zusätzlichen automatisierten Zahlen- und Rangprüfungen nicht.

## Durchgeführte Läufe

- Vor Beginn der Historienkomponente: zwei Erwartungen erfüllt; zusätzliche Zuordnungs-, Werte- und Rangprüfungen bestanden.
- Nach Implementierung und gezielter Prüfung der Historienkomponente: erneut zwei Erwartungen erfüllt und alle zusätzlichen Prüfungen bestanden.

Beide Läufe: IroGen 1.5.0 / Einzelbildanalyse 0.5.4. Die neue Historienkomponente besitzt separat den internen Vertrag Version 1; sie ändert den Einzelbildalgorithmus nicht.

Keine ausdrückliche Abnahme und kein Schluss-Sammel-Testlauf.
[Bericht vor der Erweiterung](../tests/adjustments/zwischenkontrolle-20260923/iro-run-c1b2abce9906484d9db3faa4f4e6a6af/bericht.md) · [Bericht nach der Erweiterung](../tests/adjustments/zwischenkontrolle-20260923/iro-run-70536f0090ec4755bbde068341ab49bd/bericht.md)

## Kontrolle nach Aufhebung der Mehrbild-Verarbeitung

Planfassung 1.36: Historienkern entfernt; unabhängige Einzelbildauswertung beibehalten. **Prüfung nach Rückbau, 23. September 2026:** 19 gezielte Einzelbild-/Schutz-/Anzeigeprüfungen bestanden; anschließend beide Kontrollbilder einschließlich Feldzuordnung, Werten und Rangfolge bestanden. Keine übersprungenen Tests. Kein Schluss-Sammel-Testlauf und keine neue Geräteprüfung.
[Bericht nach Rückbau](../tests/adjustments/zwischenkontrolle-20260923/iro-run-acacea92ec99484088d0855396693077/bericht.md)

## Kontrolle nach der Unschärfekorrektur

Analyse 0.5.5: beide Bilder einschließlich aller zusätzlichen Werte-/Zuordnungs-/Rangprüfungen bestanden. [Bericht](../tests/adjustments/zwischenkontrolle-20260923/iro-run-c68eb262e96243d2b784115e12e3751b/bericht.md).
