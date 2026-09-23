# Verständlich über Anforderungen und Entscheidungen sprechen

**Diese Regel gilt für jeden Agenten in diesem Projekt:** Wenn du mit dem Nutzer über Anforderungen, Entscheidungen, offene Fragen, Widersprüche oder Tests aus unseren Plänen sprichst, benenne den betreffenden Inhalt in einem kurzen, verständlichen Text. Kryptische Kennungen wie `CAA001`, `CR04.32`, `K4` oder bloße Abschnittsnummern ersetzen keine Erklärung.

- Beschreibe konkret, welches App-Verhalten oder welche Festlegung gemeint ist und was daran zu klären ist.
- Formuliere sinngemäß und knapp. Wenn der genaue Wortlaut entscheidend ist, zitiere die relevante Passage kurz.
- Verwende Kennungen nur bei Bedarf ergänzend zur verständlichen Beschreibung, niemals als alleinigen Bezug. Ein Dateilink kann die Fundstelle zusätzlich belegen.
- Der Nutzer soll die Aussage verstehen können, ohne Kennungen im Plan nachschlagen zu müssen.

**Nicht so:** „In CAA001 und CR04.32 wurde angegeben, dass …“

**Sondern so:** „Beim App-Verhalten für fehlerhafte Bilder haben wir festgelegt, dass unbrauchbare Bilder keine Messwerte liefern dürfen. Jetzt müssen wir klären, welchen Hinweis der Nutzer dabei erhält.“

Diese Vorgabe gilt auch für Rückfragen, Fortschrittsmeldungen, Prüfberichte und Zusammenfassungen. Technische Kennungen dürfen innerhalb der Pläne und im Code zur eindeutigen Zuordnung bestehen bleiben.

## Verbindliche Arbeitsgrundlage und Entscheidungen

Der [konsolidierte Konzept- und Entwicklungsplan](IRO-KONSOLIDIERTER-PLAN.md) ist die verbindliche inhaltliche Grundlage dieses Projekts. Lies vor der ersten Arbeit am Projekt den Plan und vor jeder Aufgabe die dafür relevanten Abschnitte. Bereits gelesene, unveränderte Inhalte müssen nicht wiederholt vollständig gelesen werden. Historische Entwürfe und Bewertungen anderer KI-Systeme ersetzen den aktuellen Plan nicht.

- **Beschlossene Anforderungen respektieren:** Setze bestätigte Anforderungen um. Öffne sie nicht allein wegen einer abweichenden KI-Empfehlung erneut und ändere sie nicht stillschweigend.
- **Offene Produktentscheidungen offen halten:** Wähle keine noch unentschiedene Produktvariante eigenmächtig als verbindliches App-Verhalten. Stelle die konkrete Entscheidungsfrage verständlich dar, sobald die Umsetzung davon abhängt; unabhängige Arbeiten können weitergehen.
- **Technische Umsetzung selbstständig voranbringen:** Implementierungsdetails, Fehlerkorrekturen und Prüfungen innerhalb der beschlossenen Anforderungen dürfen ohne erneute Grundsatzfreigabe bearbeitet werden. Als Versuch gekennzeichnete Ansätze dürfen untersucht werden; ein Versuchsergebnis ist noch keine automatisch beschlossene Produktänderung.
- **Nutzerentscheidungen haben Vorrang:** Berücksichtige ausdrückliche Entscheidungen und bereits erteilte Autorisierung aus dem laufenden Austausch. Hole dafür keine wiederholte Bestätigung ein. Bei einem tatsächlichen Widerspruch benenne die betroffene Anforderung und die Auswirkung verständlich.
- **Planänderungen nachvollziehbar halten:** Trage autorisierte Entscheidungen und belegte technische Erkenntnisse an der passenden Stelle ein. Kennzeichne Vorschläge weiterhin als Vorschläge; schließe offene Produktfragen nur aufgrund einer Nutzerentscheidung. Bei inhaltlichen Planänderungen die Fassung gemäß Plan erhöhen und die Änderung kurz erläutern. Keine parallele, widersprüchliche Konzeptfassung anlegen.

## Entscheidungen und Quellen dokumentieren

- Dokumentiere künftig jede neue oder geänderte Festlegung kurz im Änderungsprotokoll des verbindlichen Plans: **Datum, Entscheidung beziehungsweise Änderung, Begründung, betroffene Planfassung und Grundlage** (zum Beispiel ausdrückliche Nutzerentscheidung oder verlinkter Testbefund).
- Aktualisiere zugleich die betroffene Stelle im Plan. Das Änderungsprotokoll beschreibt die Entwicklung; maßgeblich bleibt die aktuelle Festlegung im jeweiligen Fachabschnitt.
- Kennzeichne historische Entwürfe, KI-Bewertungen und frühere Prüfaussagen ausdrücklich als historisch. Stelle sie nicht als aktuelle Vorgaben oder erneut geprüfte Befunde dar.
- Rekonstruiere frühere Änderungen nur, soweit sie durch vorhandene Fassungen, den verfügbaren Austausch oder andere nachvollziehbare Belege gestützt sind. Kennzeichne solche Einträge als nachträglich rekonstruiert und nenne ihre Grundlage. Unbekannte Daten, Gründe oder Versionszuordnungen nicht erfinden; Lücken ausdrücklich offenlassen.
- Änderungen an Agentenregeln ebenfalls mit Bezug zur geltenden Planfassung dokumentieren. Eine reine Agentenregeländerung verlangt keine erfundene fachliche Planänderung.

## Build, Tests und Start

Die folgenden PowerShell-Befehle gelten vom Projektstamm aus. SDK-Version und Paketstände werden durch `global.json`, Projektdateien und Paket-Lockdateien festgelegt.

```powershell
# Pakete in den festgelegten Versionen wiederherstellen
dotnet restore iro.slnx --locked-mode

# Gesamte Solution bauen
dotnet build iro.slnx --no-restore

# Fachliche Tests im plattformunabhängigen Kern ausführen
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore

# Testgenerator starten
dotnet run --project tools/iro.testgen/Iro.TestGen.csproj --no-restore

# Android-App auf einem bereits gestarteten Emulator oder verbundenen Gerät starten
dotnet build src/iro.app/Iro.App.csproj -t:Run -f net10.0-android --no-restore
```

Führe die zur Änderung passenden Prüfungen aus. Reine Dokumentationsänderungen benötigen keinen App-Build. Bei beabsichtigten Paketänderungen die Lockdateien gezielt aktualisieren; ein fehlgeschlagener Restore ist kein Grund, Versionen beiläufig zu ändern.

Ein erfolgreicher Build belegt keine fachliche Richtigkeit. Ein Testlauf ohne gefundene Tests gilt nicht als bestandene fachliche Prüfung. Berichte, was tatsächlich geprüft wurde und was noch aussteht. Der Testgenerator ist derzeit ein Gerüst; sein erfolgreicher Start weist keine Testdatenerzeugung nach.

Details zu Visual Studio, Emulator-Grafik und bisherigen Prüfungen stehen in [Entwicklungsumgebung und Prüfstand](docs/entwicklungsumgebung.md). Reale Kamera- und Farbgenauigkeit sind am Gerät zu prüfen; Emulatorprüfungen ersetzen diese Nachweise nicht.

## Verbindliches Protokoll zur Bildoptimierung und Arbeitsfortschritt

Für alle im Projekt verwendeten Agenten gilt die Nutzerentscheidung vom 22. September 2026 in [Planfassung 1.27, gemeinsame Messgrundlage](IRO-KONSOLIDIERTER-PLAN.md#64-verbindliche-bildoptimierung-und-gemeinsame-messgrundlage). Vor einschlägiger Arbeit das [angenommene Entscheidungsprotokoll](docs/entscheidung-bildoptimierung-2026-09-22.md) und den [Arbeitsplan](docs/arbeitsplan-bildoptimierung.md) lesen. Keine abweichenden Agentenvereinbarungen führen.

- Wand und Farbstreifen aus derselben gemeinsamen Messgrundlage auswerten. Keine voneinander unabhängige Helligkeits-, Gamma-, Kontrast-, Weißabgleich-, Farb- oder Filterkorrektur.
- Optimierte Erkennungskopien dürfen Geometrie liefern, aber keine bearbeiteten Farbwerte für die Messung. Koordinaten zuverlässig zurückführen; Qualitätsmängel der Originalaufnahme nicht durch verbesserte Optik kaschieren.
- Bedingt zulässige gemeinsame Korrekturen erst nach den vorgeschriebenen reproduzierbaren Nutzennachweisen einsetzen. Keine verlorene Farbinformation rekonstruieren oder erfinden.
- Arbeitsplan schrittweise pflegen. Umsetzung, bestandene Prüfung und ausdrückliche Nutzerabnahme getrennt mit Datum und Belegen dokumentieren. Erst wenn alle drei vorliegen, den Punkt als erledigt markieren. Alte Tests sind keine automatische Abnahme neuer Anforderungen; Schweigen oder die Übermittlung eines Berichts ist keine Abnahme.
- Abnahmepunkte, Rückfragen und Berichte beschreiben das konkrete Verhalten in Klartext. Unabhängige autorisierte Arbeit während ausstehender Abnahme fortsetzen; Abnahmen nicht selbst behaupten. Bei Änderungen betroffene Prüfungen und Abnahmen wieder öffnen.

Grundlage dieser Agentenregel: ausdrücklicher Nutzerauftrag zum verbindlichen Protokoll und schrittweisen Arbeitsplan; dokumentiert im Änderungsprotokoll der Planfassung 1.27.
**Aktueller Vorrang, Planfassung 1.28:** Gemäß Nutzerauftrag zuerst die im [Regelaudit](docs/regelaudit-2026-09-22.md) belegten Verstöße und offenen notwendigen Schutzprüfungen bearbeiten. Schwerpunkt ist die Kunden-App Iro. Zusätzliche Bildoptimierungen zurückstellen; Generator-Testzahlen sind keine App-Abnahme. Den Vorrang und offenen Status im Arbeitsplan pflegen.
