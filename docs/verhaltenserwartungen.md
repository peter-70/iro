# Automatische Verhaltenserwartungen im Testwerkzeug

23. September 2026 · Plan 1.33 · IroGen 1.5.0 · Analyse 0.5.4.

## Umfang und Trennung

Geprüft werden bildbezogene Messfreigabe (mindestens ein Feld freigegeben oder vollständige Sperre), exakte Anzahl tatsächlich freigegebener Felder und der fachliche Hauptgrund des Aufnahmehinweises. Nicht enthalten: Farbgenauigkeit, Identität jedes Feldes, exakte Sollpolygone, frei formulierte Hinweise oder sämtliche feldbezogenen Warnungen. Diese erste Stufe ersetzt weder die allgemeine Bildqualitätsprüfung noch eine Nutzerabnahme.

Erwartungen gehen niemals an die PNG-Analyse-API. Nach der Pixelanalyse wertet der Serienläufer sie aus und speichert Erwartung, Grundlage und Befund im unveränderlichen Ergebnislauf. Nachträgliches Ändern oder Entfernen des Eingabepakets ändert diesen gespeicherten Prüfbefund nicht. Nominale Farbabstände bleiben eine separate Diagnose.

## Versionierte Verträge

- Testplanformat **2**: Jeder Fall darf zusätzlich expected enthalten. Format 1 bleibt lesbar, darf aber keine neuen Erwartungen enthalten.
- Erwartungsformat **1**: formatVersion, verification (proposed oder verified), basis, measurementAllowed, releasedFieldCount, requiredHint und forbiddenHint. Unbekannte Felder werden zurückgewiesen. Fehlende Einzelkriterien bedeuten unbekannt, nicht null Felder. Mindestens ein Kriterium erforderlich.
- verified erfordert eine nichtleere, nachvollziehbare Grundlage. Das Kennzeichen bestätigt die dokumentierte fachliche Vorprüfung des Falles, nicht die Nutzerabnahme. Software kann die Wahrheit eines Grundlage-Textes nicht beweisen.
- Die Erwartung wird im bestehenden erweiterbaren Generatorparameterbereich der Aufnahme als behaviorExpectation mit eigener Formatversion gespeichert. Aufnahmeformat 1 bleibt unverändert; dessen bisheriges expected für Feldgeometrie und Farbwerte wird nicht umgedeutet.
- Neue Ergebnisläufe verwenden **Analyseformat 2** mit expectation und evaluation je Aufnahme. Alte Ergebnisläufe v1 werden nicht rückwirkend bewertet.
- Die Analyse liefert zusätzlich einen nullable fachlichen Haupt-Hinweiscode: None (kein Qualitätshinweis), Other (anderer Qualitätshinweis), PerspectiveTaper (Verjüngung, frontalere Aufnahme erforderlich). Fehlender Code alter Analysen ist unbekannt und wird nicht als None interpretiert. Ein anderer UI-Wortlaut verändert die Prüfung nicht. Messalgorithmus und Freigabeschwellen wurden nicht verändert.
- Schemata: [Testplan v2](../tests/schemas/irogen-testplan-v2.schema.json), [Analyselauf v2](../tests/schemas/analyse-lauf-v2.schema.json). Semantische Widersprüche und Optionen prüft zusätzlich der Leser.

## Bewertung und Anzeige

- **Erwartung erfüllt:** Alle hinterlegten geprüften Kriterien passen.
- **Erwartung NICHT erfüllt:** Mindestens ein Kriterium widerspricht dem tatsächlichen Ergebnis. Beispielsweise: „Vollständige Sperre erwartet; tatsächlich 2 Felder freigegeben.“
- **Nicht bewertet:** Keine oder nur vorgeschlagene Erwartung, historischer Lauf ohne Prüfung oder fehlender fachlicher Hinweiscode, sofern kein anderes Kriterium bereits verletzt ist.
- **Prüffehler:** Ungültige Erwartung, fehlendes Ergebnis oder Verarbeitungsfehler.

Dialog: Testfall, Soll-Ist-Status, erwartetes Verhalten und Prüfbefund sind getrennte Spalten; oben stehen Gesamtzahlen. Eine erwartete vollständige Sperre zählt als erfüllt. Fehlgeschlagene Prüfungen bleiben im Problemfilter sichtbar, auch wenn der nominale Farbvergleich unauffällig ist. Nominale Farbauffälligkeiten bleiben zusätzlich sichtbar, auch bei erfüllter Verhaltenserwartung. Ohne geprüfte Erwartung bleibt die bisherige Diagnosefilterung erhalten.

Export: Gesamtzahlen und vollständige Soll-Ist-Tabelle für jede Aufnahme, einschließlich Grundlage, gefolgt von der bisherigen nominalen Diagnose. Der Export umfasst weiterhin alle eingelesenen Aufnahmen unabhängig vom Dialogfilter. Ein leeres Ergebnis enthält keine bestandenen Fälle; angeforderte und verarbeitete Bildzahl bleiben separat sichtbar.

## Nutzer-Gegenlauf

1. Neu gebauten IroGen starten: iro-gen/bin/Release/net10.0-windows/IroGen.exe.
2. [Aktualisierten Zehn-Bilder-Plan](../iro-gen/testplans/perspektivkorrektur-konturpruefung.json) über „Testplan laden“ laden.
3. Erzeugung abwarten, „An Iro-Tests senden“, Analyseabschluss abwarten.
4. „Testergebnisse auswerten“: aktuelle Serie mit zehn Bildern, IroGen 1.5.0 und Analyse 0.5.4.
5. Erwartet: **10 erfüllt, 0 nicht erfüllt, 0 nicht bewertet, 0 Prüffehler**. Die nominale Diagnose zeigt weiterhin sechs vollständig gemessene und vier abgewiesene Bilder.
6. Markdown-Bericht exportieren und zur Prüfung vorlegen.

Die Pixeloptionen dieses Plans bleiben unverändert; neu sind Formatversion und maschinenlesbare Erwartungen. Grundlage sind die vorab festgelegten Geometriefälle und der dokumentierte Nutzer-Gegenlauf unter Analyse 0.5.3.

## Prüfung und Abnahme

Gezielte Tests prüfen falsche Freigaben, falsche Feldanzahlen, falsche/fehlende Hinweise, geänderten UI-Wortlaut, ungeprüfte Erwartungen, ungültige Verträge, alte Planformate, Filter und Export. Ein Ende-zu-Ende-Test führt Erzeugung, PNG-Analyse, Persistenz, Dialog und Export des Zehn-Bilder-Plans aus; er liest das Ergebnis zusätzlich ohne ursprüngliches Eingabepaket. Vertragsschemata wurden am tatsächlichen Plan und Ergebnislauf geprüft.

[Entwicklungsbericht](../tests/adjustments/erwartungspruefung-20260923/bericht.md). Vollständiger Abschlussprüfstand wird im Arbeitsplan festgehalten.

- [x] Erste Stufe umgesetzt.
- [x] Gezielte technische Prüfung bestanden.
- [ ] Nutzer-Gegenlauf und ausdrückliche Abnahme.
- [ ] Teilaufgabe erledigt.

Abnahme in Klartext: „IroGen vergleicht die geprüften Freigabe-, Feldanzahl- und Hinweiserwartungen mit den tatsächlichen Ergebnissen. Abweichungen werden verständlich angezeigt und exportiert. Ungeprüfte und historische Fälle werden nicht automatisch als bestanden ausgegeben.“