# Gemeinsame Aufnahmebeschreibung

Verbindlicher Einstieg: [Aufnahmeschema v1](aufnahme-v1.schema.json). Generator und Kameraaufzeichnung verwenden dieselben Feldnamen und Datentypen. Ein Aufnahme-Datensatz besteht aus einer PNG-Datei und einer JSON-Datei gleichen Basisnamens. IDs müssen innerhalb der zusammengeführten Datenbasis eindeutig sein; Beispiele sind keine produktive ID-Strategie.

## Bedeutung

- Unbekannte Werte werden als `null` angegeben, niemals durch erfundene Werte, Nullmessungen oder leere Texte ersetzt. `expected: null` bedeutet: kein Sollbefund bekannt. Eine leere Feldliste bedeutet ausdrücklich: keine Felder erwartet.
- `conditions.generator` enthält Erzeuger, Version, Seed und Parameter; bei Kameraaufnahmen ist es `null`. Bei Generatoraufnahmen ist entsprechend `conditions.camera` gleich `null`.
- Kameraangaben beschreiben tatsächlich angewandte Werte, soweit bekannt, nicht bloß angeforderte Einstellungen. Nanosekundenwerte stehen verlustfrei als Dezimalzeichenfolgen in JSON.
- `createdAtUtc` ist eine UTC-Zeitangabe; sie ersetzt nicht den Sensorzeitstempel. Fehlende Zeitbasis (`null`) und die von Android gemeldete unbekannte Zeitbasis (`unknown`) sind verschieden.
- Rechtecke beziehen sich auf Pixel der gespeicherten Datei: Ursprung links oben, x nach rechts, y nach unten, Breite und Höhe positiv. Der Leser muss zusätzlich prüfen, dass sie innerhalb des Bildes liegen.
- `expected` enthält ausschließlich Erwartungen. `proposed` ist ein ungeprüfter Vorschlag; erst `verified` mit dokumentierter Grundlage darf als verbindlicher Sollbefund dienen. Unbekannte Einzelangaben bleiben `null`.
- `measurementAllowed` benennt die erwartete Freigabe; feldbezogene Angaben ermöglichen teilweise Sperren. `requiredHint` beschreibt den erwarteten Hinweis sinngemäß, nicht einen sprachabhängigen exakten UI-Text.
- Tatsächliche Testergebnisse liegen separat unter `tests/runs/` und verweisen mindestens auf Aufnahme-ID, Datensatzversion, Formatversion und Testlauf-ID. Sie dürfen Erwartungen nicht überschreiben. Ihr eigenes Schema wird vor der Implementierung der Ergebnisausgabe ergänzt.
- Generatorparameter sind bewusst erzeugerspezifisch. Ihre Bedeutung wird mit Erzeugername und Version festgelegt; beide Komponenten dürfen keine alternativen Namen für gemeinsame Angaben erfinden.

## Umfang und Grenzen

Die beiden JSON-Beispiele sind **illustrative Metadaten**, keine vorhandenen Kameraaufnahmen oder abgenommenen Testbilder. Die referenzierten PNG-Dateien sind noch nicht vorhanden. Ein gemeinsamer Leser kann beide Beschreibungen einlesen; vollständige Bildwiedergabe und fachliche Prüfung folgen mit echten Eingaben vor Fertigstellung der erzeugenden Komponenten.

v1 beschreibt den gemeinsamen PNG-Einstieg. Originale Kamerapuffer, Strides, Farbkorrekturmetadaten, Empfangszeiten, Transformationen und Bildfolgen gemäß Plan sind damit noch nicht vollständig abgebildet. Vor Umsetzung der Live-Aufzeichnung wird der Vertrag dafür ausdrücklich versioniert erweitert. Eine PNG-Umwandlung allein ersetzt keinen Nachweis des Original-Pufferpfads.

Der Leser muss neben der Schemaprüfung Dateiexistenz, tatsächliche Bildgröße, eindeutige IDs, gültige Flächen und die belegte Richtigkeit der Sollbefunde prüfen. Schema-Gültigkeit beweist keine Farbgenauigkeit.

## Prüfung

Mit PowerShell 7 vom Projektstamm:

```powershell
./tests/schemas/Test-Examples.ps1
```

Der Prüfer verwendet denselben JSON-Leser und dasselbe Schema für beide Beispiele. Er prüft außerdem, dass fehlende Angaben, ungültige Bildgrößen und untergemischte Ist-Ergebnisse abgewiesen werden.

Struktur- oder Bedeutungsänderungen erhalten eine neue Formatversion und Schemadatei; vorhandene Datensätze werden nicht stillschweigend umgedeutet.

Grundlage: [JSON Schema – Objekte](https://json-schema.org/understanding-json-schema/reference/object) und [unbekannte Werte mit null](https://json-schema.org/understanding-json-schema/reference/null).

## Historisch: Serien und vorbereitete Testaufträge in Planfassung 1.18

[bildserie-v1.schema.json](bildserie-v1.schema.json) beschreibt eine vollständige IroGen-Serie mit Aufnahmeverweisen, Bildanzahl und nominaler Abstandsverteilung. [testauftrag-v1.schema.json](testauftrag-v1.schema.json) beschreibt die Übergabe solcher Eingaben an die spätere Iro-Testausführung. Die einzelnen Aufnahmen verwenden unverändert das Aufnahmeformat v1.

Der Auftrag kennzeichnet ausdrücklich `pending-analysis` und `analysisAvailable: false`. Das ist eine bereitgestellte Eingabesammlung, kein ausgeführter Test. Ein Ergebnisformat und die spätere Rückgabe/Anzeige von Analysedaten sind damit nicht vorweggenommen. Wie bei Einzelaufnahmen zusätzlich Dateiexistenz, übereinstimmende IDs/Anzahlen und sichere relative Pfade prüfen; eine reine Schemaprüfung genügt nicht.

## Aktuell: lokale Analyse, Planfassung 1.19

Neue Aufträge verwenden [Testauftrag v2](testauftrag-v2.schema.json) mit `analysisAvailable: true`. Der Status `pending-analysis` beschreibt die bereitgestellte Eingabe; ein Lauf verändert diese nicht. Historische Aufträge v1 sind lesbar, ohne ihre ursprüngliche Bedeutung umzuschreiben. [Analyselauf v1](analyse-lauf-v1.schema.json) definiert separate tatsächliche Ergebnisse einschließlich Aufnahme-ID, PNG-Hash, Algorithmus-/Profilversion, Messflächen, gültiger/fehlender Werte und nachträglicher nominaler Vergleiche. Fehlende Messwerte sind `null`, niemals Nullabstände.

Zusätzlich zum Schema zählt `tests/iro.gen.tests/Test-AnalysisRuns.ps1` die Einzelbefunde nach und prüft die Messgeometrie sowie den Abschlusszustand. Ein bestandenes Schema bestätigt keine fachliche Messgenauigkeit. Dokumentation: [API](../../docs/analyse-api.md), [Prüfstand](../../docs/analyse-pruefung.md).

## Bildoptimierung: verbindliche Semantik ab Planfassung 1.27

Für Aufnahme- und Analyseverträge gilt zusätzlich der [Messvertrag zur Bildoptimierung](../../docs/analyse-api.md#verbindlicher-messvertrag-zur-bildoptimierung--planfassung-127). Erkennungspixel und Messfarben sind getrennt zu halten; Wand und Streifen haben dieselbe Messgrundlage. Die neuen Aufnahme-Sperren sind in den Erwartungen und Ergebnissen nach ihrer Implementierung nachvollziehbar abzubilden. Geprüfte Erwartungen vorher festlegen; historische Läufe nicht umdeuten. Die konkrete Schemaerweiterung samt Kompatibilitätsprüfung ist ein offener Schritt im [Arbeitsplan](../../docs/arbeitsplan-bildoptimierung.md), keine mit dieser Dokumentationsänderung ausgelieferte Funktion.

Aktuell ab Plan 1.33: [automatische Verhaltenserwartungen](../../docs/verhaltenserwartungen.md). Neue Testpläne können Format 2 verwenden; neue Ergebnisläufe verwenden Format 2 mit gespeicherter Erwartung und Soll-Ist-Bewertung. Altformate bleiben lesbar, ohne rückwirkende Bewertung. Die erste Stufe prüft Freigabe, freigegebene Feldanzahl und fachlichen Haupt-Hinweis, keine Farbgenauigkeit.
