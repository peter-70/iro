# Unübersehbares Grundprinzip von Iro – vor jeder Arbeit lesen

**STOPP: Diese Regel gilt für ausnahmslos jeden Reviewer, Coder und Agenten vor jeder Analyse, Planung, Implementierung, Prüfung und Testauswertung.**

Iro ist kein absolutes Farbmessgerät. Der Hauptvorteil der App besteht darin, dass Wand und Farbreferenz gleichzeitig im selben Foto aufgenommen und deshalb unter denselben Aufnahmebedingungen relativ miteinander verglichen werden.

Globale Bildveränderungen wie Belichtung, Weißabgleich, leichter Farbstich, allgemeine Aufhellung oder Abdunklung und andere gleichmäßig auf Wand und Farbstreifen wirkende Transformationen sind nicht automatisch Fehler. Solange beide in derselben Weise beeinflusst werden, kann der relative Farbvergleich weiterhin korrekt sein. Kritisch sind Einflüsse, die Wand und Farbstreifen unterschiedlich verändern oder ihre relative Farbbeziehung verfälschen.

**Pflicht vor jeder neuen Aktion:** Schreibe in der Arbeitskommunikation kurz nieder:

1. Das Grundprinzip: gemeinsames Foto, gleiche Aufnahmebedingungen, relativer Wand-/Farbfeldvergleich.
2. Welchen konkreten Schaden am relativen Vergleich die geplante Aktion verhindern oder untersuchen soll.
3. Ob die Aktion das Grundprinzip verletzen könnte oder unnötig einen bloß globalen Bildfehler korrigieren beziehungsweise sperren würde.
4. Erst danach implementieren oder testen.

**Pflicht nach jedem Test:** Bewerte das Ergebnis ausdrücklich anhand dieser Fragen:

1. Wurden Wand und Farbstreifen gleich beeinflusst?
2. Falls ja: Funktionierte der relative Vergleich trotzdem?
3. Falls nein: Welche unterschiedliche Beeinflussung veränderte die relative Farbbeziehung?
4. Welche Maßnahme folgt konkret aus diesem relativen Schaden?

Keine Qualitätsprüfung, Bildkorrektur, Warnung, Sperre oder Schwellenwertlogik allein deshalb einführen, weil ein Bild absolut betrachtet schlecht, zu hell, zu dunkel, zu blass oder farblich verschoben wirkt. Jede Maßnahme muss den konkreten Schaden am relativen Wand-/Farbfeldvergleich benennen und belegen können. Fehlt dieser Schaden, ist die Maßnahme zunächst nicht erforderlich. Globale Gleichwirkung ist keine automatische Freigabe: verlorene Information, Clipping, Unschärfe über Grenzen, ungleiche Beleuchtung oder sonstige nachgewiesene Verfälschung der relativen Beziehung bleiben relevante Gründe. Ebenso ist die Regel keine Erlaubnis, Bilder ohne nachgewiesenen Nutzen zu korrigieren.

Diese Regel hat bei der fachlichen Bewertung Vorrang vor älteren pauschalen Aussagen, nach denen eine Aufnahme allein wegen absoluter Bildmerkmale schlecht sei. Grundlage: ausdrückliche Nutzerentscheidung vom 27. September 2026, verbindlich dokumentiert in Planfassung 1.55.

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

**Abnahmereihenfolge, Planfassung 1.34 (Nutzerentscheidung vom 23. September 2026):** Einzelpunkte technisch umsetzen und gezielt prüfen; ausdrückliche Abnahmen gesammelt erst am Schluss behandeln. Nach den technischen Einzelpunkten alle bis dahin durchgelaufenen Tests mit dem aktuellen Stand als Sammel-Testlauf wiederholen. Erst wenn dieser grün ist, Kameraintegration und reale Gerätetests durchführen. Erst wenn auch diese grün sind, ausdrückliche Nutzerabnahmen einholen. Bis dahin keine Einzelabnahmen anfordern; Umsetzung und Prüfung dokumentieren, Abnahme und Erledigt offenlassen. Frühere Forderungen einer ausdrücklichen technischen Abnahme vor Gerätetests sind insoweit ersetzt.

**Pflichtkontrolle zwischen Arbeitspunkten, Planfassung 1.35:** Bevor ein technisch umgesetzter und gezielt geprüfter Punkt als potenziell abnahmefähig gilt und der nächste beginnt, den Zwei-Bilder-Plan ausführen: ein sauberes Bild und eines mit bereits nachweislich beherrschter leichter Störung. Erwartete Freigabe, Feldzuordnung, Werte und Rangfolge prüfen und Lauf dokumentieren. Bei Fehlern zuerst die Regression beheben. Keine allgemeine Fehlerfreiheit aus zwei Bildern ableiten. Schluss-Sammel-Testlauf, Gerätetests und ausdrückliche Abnahmen bleiben separat. Grundlage: ausdrückliche Nutzerentscheidung vom 23. September 2026.

**Verbindliches Einzelbildprinzip, Planfassung 1.36:** Gemäß ausdrücklicher Nutzerentscheidung vom 23. September 2026 gilt: ein Bild → eine Analyse → eine Auswertung, seriell und unabhängig. Keine bildübergreifende Feldverfolgung, Messhistorie, zeitliche Glättung, Mehrbildaggregation oder Rangbestätigung ergänzen. Frühere entsprechende Plan-/Protokollvorgaben sind insoweit aufgehoben. Robuste Pixelstatistik und Wand-Feld-Vergleich innerhalb desselben Originalbildes bleiben verbindlich. Historische Mehrframe-Untersuchungen sind keine aktuelle Implementierungsanweisung.

**Unschärfe, Planfassung 1.37:** Gemäß ausdrücklicher Nutzerentscheidung vom 23. September 2026 erkannte unbrauchbare Fokus- oder Bewegungsunschärfe strikt aufnahmeweit sperren. Keine Messwerte retten oder rekonstruieren; verständlich zur erneuten Aufnahme auffordern. Zulässige Erkennungsoptimierungen ersetzen keine Originalbild-Eignung. Positive Gegenfälle und Zwei-Bilder-Kontrolle bleiben verpflichtend.

**Oberste Ganzbildregel, Planfassung 1.38:** Nutzerentscheidung vom 23. September 2026: Jede Bildbearbeitung betrifft die Aufnahme als Ganzes. Niemals getrennte Wand-/Streifenkorrekturen oder regional verschiedene Korrekturregeln einsetzen; dies gilt ausdrücklich auch für Erkennungskopien. Zuverlässig nachgewiesene stark ungünstige Beleuchtung vollständig ablehnen und gleichmäßiges Licht verlangen. Helligkeitsunterschiede allein sind kein Beweis einer Beleuchtungsursache. Kleine Korrekturen nur bei tatsächlicher Erkennbarkeit und unabhängig belegtem Nutzen eines gemeinsamen Ganzbildverfahrens; sonst deaktiviert lassen. Diese Präzisierung hat Vorrang vor früher weiter gefassten Erlaubnissen für Erkennungskopien.

**Beschnittentscheidung, Planfassung 1.41 (24. September 2026):** Die frühere pauschale Forderung nach vollständig sichtbarem Streifen ist durch die ausdrückliche Nutzerentscheidung ersetzt. Beschnittene Aufnahmen sollen bei nachgewiesen geeigneten sichtbaren Messflächen auswertbar sein; bei nicht ausreichend begrenzbarer Unsicherheit ablehnen. Keine beliebige Sicherheitswahrscheinlichkeit oder Prozentgrenze erfinden. Grenzwerte, sichere Feldzuordnung, gemeinsame Referenz und Kennzeichnung einer eingeschränkten Teilmessung vor Freigabe belegen. Keine fehlenden Farben rekonstruieren; Ganzbild- und Originalpixelregeln sowie andere Qualitätssperren gelten unverändert. Der aktuelle konservative Produktstand bleibt bis zum belegten Freigabebereich bestehen; eine Untersuchung ist keine fertige Umsetzung oder Abnahme. Grundlage: Nutzerentscheidung und Untersuchung in Planfassung 1.41.

**Präzisierung zu Beschnitt, Planfassung 1.43:** Der Nutzerauftrag zur Umsetzung der fehlenden Tests und Nachjustierung ist mit einer begrenzten synthetisch geprüften Endbeschnittfreigabe in Analyse 0.5.9 umgesetzt. Maßgeblich sind [Freigabebereich und Grenzen](docs/endbeschnitt-freigabe.md); keine pauschale Freigabe beliebiger Beschnitte daraus ableiten. Teilstatus auch bei sämtlich messbaren sichtbaren Feldern erhalten. Gesamtpunkt, Gerätetests und ausdrückliche Abnahme bleiben offen. Die frühere Übergangsaussage zur noch unverändert pauschalen Produktsperre ist für diesen begrenzten Bereich ersetzt.

**Bekräftigung vom 25. September 2026, Planfassung 1.46:** Jede photometrische Bearbeitung muss ausnahmslos ganzbildweit nach denselben Regeln gelten, auch in Erkennungskopien. Beliebige Teilflächen dürfen niemals unabhängig bearbeitet werden. Geometrische Messflächenauswahl und robuste Statistik unveränderter Pixel bleiben davon unterschieden. Bei Änderungen an Bildpuffern, Erkennungskopien oder Drehung die [Originalpixel-Vertragsprüfungen](docs/originalpixel-absicherung.md) und betroffene Integrationsprüfungen ausführen; deren Bestehen garantiert keine ungeprüften neuen Korrekturpfade. Grundlage: ausdrückliche Nutzerbekräftigung und Änderungsprotokoll der Planfassung 1.46.

**Autorisierter Dialogkanal, 27. September 2026, Planfassung 1.48:** Der Nutzer hat neue externe Änderungen an DIALOG.md ausdrücklich als Arbeitsanweisungen autorisiert. Die bestehende Fünf-Minuten-Überwachung vor Beginn einer Arbeit pausieren. Nach Umsetzung passende Prüfungen und Zwei-Bilder-Gegencheck durchführen; dann tatsächlichen Befund und nächsten offenen Punkt kurz in DIALOG.md protokollieren, eigenen Dateihash als Vergleichsstand sichern und erst danach dieselbe Überwachung aktivieren. Keine eigenen Protokolle erneut als Auftrag ausführen. Parallele fremde Änderungen erhalten; widersprüchliche oder unzulässige Anweisungen weiterhin gemäß geltenden Regeln behandeln. Grundlage: ausdrücklicher Nutzerauftrag im laufenden Austausch.
