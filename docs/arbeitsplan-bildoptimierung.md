# Arbeitsplan: Bildoptimierung ohne Verfälschung der Farbmessung

Stand: 22. September 2026. Grundlage: [konsolidierter Plan, Fassung 1.31](../IRO-KONSOLIDIERTER-PLAN.md#64-verbindliche-bildoptimierung-und-gemeinsame-messgrundlage) und das ausdrücklich angenommene [Entscheidungsprotokoll](entscheidung-bildoptimierung-2026-09-22.md). Dieser Plan konkretisiert die beschlossene Arbeit; er ersetzt keine fachliche Festlegung.

## Verbindliche Arbeits- und Statusregeln

Die nachstehenden Schritte in der angegebenen Reihenfolge bearbeiten. Unabhängige Arbeiten dürfen während einer ausstehenden Abnahme weitergehen; Voraussetzungen für abhängige Arbeiten müssen erfüllt sein. Kameraintegration und reale Prüfungen beginnen erst nach der technischen Gesamtabnahme der Solution. Erlaubte Verfahren sind keine Pflicht, jedes Verfahren einzubauen: Bei bedingten Korrekturen zunächst Bedarf und Nutzen untersuchen; ohne Nachweis bleibt die Korrektur deaktiviert.

Jeder Schritt erhält vier getrennte Kontrollkästchen: Umsetzung, bestandene Prüfung, ausdrückliche Nutzerabnahme und Erledigt. **Erledigt darf erst markiert werden, wenn alle drei Voraussetzungen belegt sind.** Bei Dokumentationsaufgaben entspricht Umsetzung der Erstellung der Vereinbarung. Bei Untersuchungen ist ein belegtes Ergebnis „kein Nutzen, deshalb nicht aktivieren“ zulässig; das ist keine implementierte Korrekturfunktion. Ein leerer Testlauf, erfolgreicher Build oder übermittelter Testbericht ist keine fachliche Abnahme. Der Nutzer hat das Protokoll beschlossen, damit aber keine noch zu prüfende Implementierung abgenommen.

Pro Schritt festhalten: Änderungsdatum, betroffene Code-/Dokumentfassung, tatsächlich ausgeführte Prüfungen mit Ergebnis und Link, offene Einschränkungen, Abnahmeentscheidung mit Datum und Bezug auf die Nutzerantwort. Abnahmefragen verwenden die unten formulierten Verhaltensbeschreibungen, keine bloßen Kennungen. Bei späteren Änderungen betroffene Prüfungen und Abnahmen ausdrücklich wieder öffnen; alte Nachweise erhalten.

## Vorrang nach dem Entwicklungs- und Regelaudit

**Nutzerauftrag vom 22. September 2026:** Regelverstöße zuerst beheben, Schwerpunkt Kunden-App. [Ausführlicher Audit mit Nachweisen](regelaudit-2026-09-22.md). Die folgenden offenen Sicherheits- und Nachweislücken haben vor neuen Optimierungen Vorrang; sie verweisen auf Inhalte der Schritte unten und bilden kein zweites konkurrierendes Konzept.

| Vorrangige Arbeit in Klartext | Umsetzung | Prüfung | Nutzerabnahme | Erledigt |
|---|---|---|---|---|
| Reproduzierte Teilfreigaben bei Kanalanschlag, erkannter unbrauchbarer Unschärfe und erkanntem Bildbeschnitt beheben | umgesetzt in Analyse 0.5.0 | gezielte Regressionen bestanden | offen | nein |
| Nominale Diagnosen nicht als nachgewiesene Messfehler/Genauigkeit ausgeben | umgesetzt in IroGen-Auswertung | Diagnoseprüfungen bestanden | offen | nein |
| Starke Perspektive nach geprüften Eignungskriterien sperren | Teilprüfung deutlicher Verjüngung mit Konturbelegen in Analyse 0.5.2 | gezielte Gegenproben bestanden; allgemeine Perspektivprüfung offen | offen | nein |
| Unbrauchbare Unterbelichtung ohne Kanalanschlag erkennen | offen | offen | offen | nein |
| Vergleichbare Beleuchtung prüfen und Schattenwarnung/Sperre konkretisieren | offen | offen | offen | nein |
| Verbleibende Lücken bei Vollständigkeit, Unschärfe und flächigen Reflexen schließen | kleine passende Randreste in Analyse 0.5.1 ergänzt; weitere Lücken offen | gezielte Beschnittprüfungen bestanden; insgesamt unvollständig | offen | nein |
| Verifizierte Freigabe-, Hinweis- und Farbwerterwartungen im Testwerkzeug auswerten | offen | offen | offen | nein |

Keine zusätzlichen Erkennungsoptimierungen oder Farbkorrekturmodelle beginnen, solange diese Lücken nicht geschlossen oder ihre Grenzen ausdrücklich abgenommen sind. Unabhängige Fehlerbehebungen bleiben autorisiert. Die bisherige Kamera-Reihenfolge bleibt bestehen. Ein bestandener synthetischer Test wird nicht als Kundenfreigabe behandelt.

## Ausgangslage und bisherige Nachweise

Geraderichten, Rückabbildung auf Originalpolygone, robuste Flächenmessung und erste Qualitätsprüfungen sind als Vorarbeiten dokumentiert. Quellen: [Geraderichten](gerade-richten.md), [räumliche Flächenprüfung](raeumliche-flaechenpruefung.md), [Nutzerbericht mit 108 Bildern](../iro-gen/testplans/iro-testbericht-20260922-135106.md). Diese Nachweise sind nicht neu ausgeführt und decken das neue Protokoll nicht vollständig ab. Nominale Materialabstände vor einer Störung sind keine automatisch gültigen Sollwerte des gestörten Bildes. Deshalb wird kein Implementierungspunkt hier allein aufgrund früherer Gesamtzahlen als geprüft oder abgenommen markiert.

## 1. Entscheidung und Agentenverträge verbindlich verankern

- [x] Umsetzung: Originalprotokoll sichern; Fachplan auf Fassung 1.27 bringen; Agenteneinstiege und API-/Datenvertragsdokumentation verknüpfen.
- [x] Prüfung: Quelltreue, Verweise, Versionsangabe, Abdeckung aller zwölf Verfahren und Statusregeln kontrolliert; siehe Prüfvermerk am Ende.
- [ ] Abnahme: Nutzer bestätigt die dokumentierte Übernahme und den Arbeitsplan.
- [ ] Erledigt.

**Abnahme in Klartext:** „Das Protokoll ist vollständig und verbindlich übernommen. Alle Agenten finden dieselben Regeln; Umsetzung, Tests und meine Abnahme werden getrennt dokumentiert.“

## 2. Bestehenden Farb- und Qualitätspfad gegen die Entscheidung prüfen

- [x] Umsetzung: Datenfluss von Originalaufnahme über Erkennung, Masken, Farbmessung und Qualitätsfreigabe untersuchen; jede Abweichung mit Fundstelle diesem Plan zuordnen.
- [x] Prüfung: Nachvollziehbar belegen, aus welchem Bild jede gemessene Wand-/Feldfarbe stammt und welche Sperren derzeit fehlen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Wir können nachvollziehen, welche vorhandenen Funktionen die neuen Regeln bereits erfüllen und welche konkret geändert werden müssen.“

## 3. Vergleichsdaten und unabhängige Abnahmekriterien vorbereiten

- [ ] Umsetzung: Reproduzierbaren IroGen-Testplan für alle zwölf Verfahren einschließlich Kombinationen, beider Streifenrichtungen, passender und unterschiedlicher Farben sowie guter Gegenbeispiele erstellen. Bildbezogene Erwartungen vorab prüfen; Entwicklung und zurückgehaltene Abnahme trennen.
- [ ] Prüfung: Seeds, Generator-/Analyseversionen und Optionen reproduzieren; unveränderte Vergleichsläufe und kompakte Markdown-Exporte sichern. Feldtreffer, übersehene/falsche Felder, Fehlfreigaben, falsche Ablehnungen, Farbfehler, Rangfolge und Hinweise getrennt auswerten.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Die Testfälle enthalten passende und unbrauchbare Aufnahmen mit geprüften Erwartungen. Eine Verbesserung kann weder durch pauschale Ablehnung noch durch ungeprüfte Messwerte vorgetäuscht werden.“

Numerische Toleranzen vor der jeweiligen Abnahme anhand nachvollziehbarer Befunde festlegen. Die Diagnosegrenze eines alten Berichts ist keine neue Genauigkeitszusage. Die bestehende Pflicht zu mehreren hundert Bildqualitätsfällen bleibt bestehen; vorhandene Fälle dürfen nach Prüfung einbezogen werden.

## 4. Erkennung und gemeinsame Farbmessung technisch absichern

- [ ] Umsetzung: Getrennte Rollen für Erkennungsbild und gemeinsames Messbild, eindeutigen Aufnahmebezug, unveränderte Originaldaten und nachvollziehbare Koordinatenübertragung absichern. Bearbeitete Erkennungspixel dürfen den Farbpfad nicht erreichen.
- [ ] Prüfung: Erkennungskopie gezielt farblich verändern und bei festgehaltenen Messmasken identische Messwerte aus dem Original nachweisen. Zusätzlich echte Erkennung mit veränderten Masken auf richtige Zuordnung prüfen. Andere Frames und ungültige Transformationen zurückweisen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Eine für die Erkennung aufgehellte oder geschärfte Kopie verändert unsere gemessenen Farben nicht. Wand und Farbstreifen werden aus derselben gemeinsamen Aufnahme gemessen.“

## 5. Geraderichten und innere Messflächen absichern

- [ ] Umsetzung: Vorhandenes Geraderichten gegen das Protokoll prüfen; Kantenabstand, Originalpolygone, leere Rotationsränder und sichere Innenflächen vervollständigen.
- [ ] Prüfung: Horizontale/vertikale Streifen in verschiedenen Drehungen, kleine Felder, Texte, weiße Zwischenräume und mehrdeutige Kanten prüfen. Rückabbildung und Farbmessung unabhängig kontrollieren.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Verdrehte Streifen werden waagerecht oder senkrecht erkannt. Die Messung verwendet sichere Feldinneren und keine durch Drehung vermischten Randfarben.“

## 6. Moderate Perspektive auswerten und starke Perspektive ablehnen

- [ ] Umsetzung: Moderate Entzerrung für die Erkennung samt Rückabbildung entwickeln; ausreichende Originalflächen und Geometriegüte verlangen. Grenzen aus Versuchen bestimmen.
- [ ] Prüfung: Seitenblick sowie Blick von oben/unten mit und ohne Wandabstand, Drehung und Unschärfe prüfen. Starke Verkürzung darf trotz optisch entzerrter Ansicht keine Freigabe erhalten.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Leicht schräge Aufnahmen bleiben bei ausreichender Bildinformation messbar. Stark schräge Aufnahmen werden mit verständlichem Hinweis abgelehnt.“

## 7. Aufhellung, Kontrast und Schärfung der Erkennungskopie erproben

- [ ] Umsetzung: Verfahren einzeln und kombiniert als kontrollierbare Versuche untersuchen; nur belegbar nützliche Erkennungsverbesserungen übernehmen. Originale Qualitätsbewertung beibehalten.
- [ ] Prüfung: Gegen unveränderte Erkennung vergleichen; zusätzliche richtige und falsche Treffer, veränderte Messmasken, Laufzeit und Fehlfreigaben dokumentieren. Geschärfte Unschärfe und aufgehelltes Clipping bleiben ungeeignet.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Die gewählten Optimierungen helfen beim Finden der Flächen, ohne verbesserte Bildoptik mit besserer Messqualität zu verwechseln.“

## 8. Robuste Statistik, Rauschen und kleine Glanzstellen behandeln

- [ ] Umsetzung: Viele innere Originalpixel robust auswerten; auffällige Einzelpixel und kleine Glanzstellen ausschließen, Mindestfläche und Ausschlussanteil prüfen. Klassische Filter nur konservativ, bildweit mit denselben Regeln, ohne Kantenvermischung und mit Nutzennachweis erproben.
- [ ] Prüfung: Rauschen, Glanz, Text und kleine Felder gegenüber ungestörten Referenzen testen. Flächige Reflexe müssen zur Ablehnung oder einer nachweislich geeigneten alternativen Messfläche führen; keine regional unterschiedliche Filterung.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Einzelne Störpixel beeinflussen die Messung möglichst wenig. Große Reflexe oder zu wenig verbleibende Messfläche liefern keinen erfundenen Farbwert.“

## 9. Clipping, Dunkelheit und starke Unschärfe zuverlässig sperren

- [ ] Umsetzung: Aufnahmeweite Sperren bei messrelevantem Clipping, starker Unterbelichtung sowie starker Bewegungs-/Fokusunschärfe gemäß neuem Protokoll umsetzen. Keine Rekonstruktion oder kaschierende Aufhellung/Abdunklung.
- [ ] Prüfung: Stärkeabstufungen und Kombinationen testen; wirklich schwarze, weiße und gesättigte Flächen als Gegenfälle einbeziehen. Grenzwerte versionieren und begründen; Wiederfreigabe beim nächsten geeigneten Bild prüfen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Aufnahmen mit verlorener Farbinformation zeigen keine Messwerte und einen passenden Hinweis. Gute dunkle oder helle Farben werden nicht allein wegen ihrer Farbe abgelehnt.“

## 10. Vergleichbare Beleuchtung von Wand und Streifen prüfen

- [ ] Umsetzung: Schatten, Gradienten, Wandabstand und unterschiedliche Beleuchtung untersuchen; Hinweise und Freigaberegeln ableiten. Keine lokale Aufhellung nur eines Bereichs.
- [ ] Prüfung: Streifen im Licht/Wand im Schatten und umgekehrt, gleichmäßige Gegenfälle sowie farbiges Mischlicht prüfen. Nachweisbare Störung von bloß vermuteter Ursache unterscheiden.
- [ ] Abnahme.
- [ ] Erledigt.

**Noch zu entscheiden, bevor das betreffende Produktverhalten umgesetzt wird:** Bei welchen nachgewiesenen Beleuchtungsunterschieden genügt eine Warnung, und wann muss die Messung gesperrt werden? Das Nutzerprotokoll lässt beide Reaktionen zu; ein Agent darf diese Auswahl nicht als bereits beschlossen ausgeben.

**Abnahme in Klartext:** „Unterschiedliche Beleuchtung wird im nachgewiesenen Umfang erkannt und verständlich behandelt. Wand und Streifen werden nicht getrennt zurechtkorrigiert.“

## 11. Gemeinsame Helligkeits- und Farbkorrektur nur bei belegtem Nutzen

- [ ] Umsetzung: Bedarf für globale Helligkeitskorrektur und gemeinsamen Weißabgleich/Farbkalibrierung untersuchen. Voraussetzungen, vertrauenswürdige Referenzquellen, Modell, Farbraum, Parameter und Geltungsbereich dokumentieren. Ohne Nutzenbeleg bleibt der Messpfad unkorrigiert.
- [ ] Prüfung: Dasselbe Modell auf das gesamte Bild anwenden; Reflex-/Schatten-/Clippingfälle und falsche Referenzen abweisen. Mit unabhängigen Farbpaaren und getrennten Kalibrier-/Prüfdaten unkorrigiert gegen korrigiert vergleichen; Nullvergleich allein genügt nicht. Farbabstände, Rangfolge, Fehlerverteilung und Verschlechterungen prüfen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Eine gemeinsame Korrektur wird nur im belegten Geltungsbereich verwendet. Sie behandelt Wand und Streifen gleich und verbessert die Farbmessung nachweislich. Andernfalls bleibt sie ausgeschaltet.“

Keine Sollfarben aus dem Testbericht als versteckte Analyzer-Eingaben verwenden. Eine zuverlässig bekannte Kalibrierreferenz braucht einen ausdrücklich dokumentierten Eingabevertrag. Reale Gültigkeit darf nicht allein aus synthetischen Versuchen abgeleitet werden; gegebenenfalls bleibt dieser Punkt bis zur Gerätephase offen. Keine ungefragte Endanwender-Kalibrierfunktion ergänzen.

## 12. Geeignete Aufnahmen auswählen und mehrere Frames sicher auswerten

- [ ] Umsetzung: Vollständige Streifensichtbarkeit, Perspektive, Schärfe, Clipping, Licht, vergleichbare Beleuchtung, Schatten/Reflexe, kurzfristige Ruhe und Messflächen vor Freigabe zusammen prüfen. Zuerst mit Testbildfolgen arbeiten; keine feste Feldzahl oder grundlose Bewegungssperre einführen.
- [ ] Prüfung: Angeschnittene Streifen gemäß neuer Entscheidung ablehnen; wieder geeignete Frames freigeben. Paarung von Wand und Feld je Frame, Historienwechsel und zeitliche Zuordnung absichern. Robust aggregierte gültige Vergleiche dürfen keine Farben unterschiedlicher Aufnahmebedingungen vermischen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Iro wartet bei ungeeigneten Aufnahmen auf ein brauchbares Bild. Mehrere gute Bilder stabilisieren den Vergleich, ohne Wand und Streifen aus verschiedenen Situationen zusammenzumischen.“

## 13. Hinweise, Diagnose und Datenverträge vervollständigen

- [ ] Umsetzung: Verwendeten Erkennungsweg, geometrische Transformation, gemeinsame Messgrundlage, gegebenenfalls Korrekturmodell/Version und Qualitätsgründe nachvollziehbar machen. Erforderliche maschinenlesbare Vertragserweiterungen vor Nutzung versionieren; kompakte Auswertung in Klartext ergänzen.
- [ ] Prüfung: API, Export, Wiederholung eines Laufs und Anzeige fehlender Werte kontrollieren. Kein Messwert ist nicht gleich Nullabstand. Alte oder abgelehnte Werte verschwinden; unbekannte Ursachen bleiben als unbekannt gekennzeichnet.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Ich erkenne im Bericht, was Iro bearbeitet hat, woraus gemessen wurde und warum eine Aufnahme abgelehnt wurde. Hinweise und Abnahmefragen sind ohne technische Kennungen verständlich.“

## 14. Technische Gesamtabnahme mit unabhängigen Fällen durchführen

- [ ] Umsetzung: Übernommene Verfahren, Regeln, UI und Testmodus zusammenführen; kompakte Vorher-/Nachher-Übersicht mit Einschränkungen erstellen. Bedingte, nicht nachgewiesene Korrekturen bleiben deaktiviert.
- [ ] Prüfung: Relevante Kern-, Generator-, API-, Regressions- und Emulatorprüfungen sowie zurückgehaltenen Abnahmesatz ausführen. Fehlerquoten und Messgüte gegen vorab vereinbarte Kriterien prüfen; kein Nachjustieren am Abnahmesatz. Laufzeit und Abbruch prüfen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Der vollständige technische Messablauf erfüllt die vereinbarten Kriterien auch bei unabhängigen Testfällen. Die verbleibenden Gerätenachweise sind ausdrücklich benannt.“

## 15. Kamera und reale Messgüte nach technischer Abnahme prüfen

- [ ] Umsetzung: Erst nach technischer Gesamtabnahme Kamera anbinden und dieselben Regeln auf echte Frames anwenden; AE/AWB-Schalterentscheidung beibehalten. Reale Versuche mit dokumentierter Beleuchtung, Materialien, Geräteeinstellungen und Referenzen vorbereiten.
- [ ] Prüfung: Schärfe, Clipping, Dunkelheit, Mischlicht, Schatten, Glanz, Perspektive, Wandabstand, zeitliche Stabilität und reale Farbfehler prüfen. Gerätegrenzen und gegebenenfalls Korrekturmodelle unabhängig validieren.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Die Messung funktioniert auf den geprüften echten Geräten innerhalb dokumentierter Grenzen. Synthetische Tests werden nicht als Beleg realer Farbgenauigkeit ausgegeben.“

## Prüf- und Abnahmeprotokoll

| Datum | Arbeitsschritt in Klartext | Umsetzung / Prüfung | Abnahme und Einschränkung |
|---|---|---|---|
| 22. September 2026 | Entscheidung und Agentenverträge verbindlich verankern | Originalprotokoll unverändert im Repository gesichert; Fachplan 1.27, Arbeitsplan, AGENTS.md, CLAUDE.md, README sowie API-/Schemadokumentation verknüpft. Quellgleichheit, lokale Verweise, Verfahrensabdeckung und Statusstruktur geprüft. Reine Dokumentationsänderung, kein App-Build und keine neuen fachlichen Tests. | Protokoll vom Nutzer ausdrücklich beschlossen; Abnahme seiner dokumentierten Umsetzung noch offen. |
| 22. September 2026 | Bestehenden Farb- und Qualitätspfad gegen die Entscheidung prüfen | Codepfade von App, Core, Windows-Adapter und IroGen geprüft; fünf unabhängige Fehlergegenproben vor Korrektur; gezielte Korrekturen und 68 Kern-/121 Integrationstests bestanden. [Audit](regelaudit-2026-09-22.md) dokumentiert verbleibende Grenzen. | Abnahme offen; bestehende Regelabdeckung nicht vollständig. Keine Erweiterungsfreigabe daraus ableiten. |
| 22. September 2026 | Geometrische Eignung und unabhängige Freigabe-/Ablehnungserwartungen | Zwölf neue Pixelgegenproben; acht Fehler vor Korrektur reproduziert, danach alle bestanden. Analyse 0.5.1; insgesamt 80 Kern- und 121 Integrationstests bestanden. [Prüfung und Grenzen](geometrie-schutzpruefung.md). Teilbeitrag zu Vergleichsdaten, Perspektive und Vollständigkeit. | Nutzerabnahme offen; die übergeordneten Arbeitsschritte sind weiterhin nicht vollständig umgesetzt oder erledigt. |
| 22. September 2026 | Falsche Perspektivablehnung vermeiden und kombinierte Störungen prüfen | Vier Fehlablehnungen vor Korrektur reproduziert. Konturprüfung in Analyse 0.5.2 ergänzt; 22 gezielte Geometriefälle und insgesamt 90 Kern-/121 Integrationstests bestanden; Release-Build einschließlich Android erfolgreich. [Nachweise und Grenzen](geometrie-schutzpruefung.md). | Keine allgemeine Perspektivfreigabe; gleichmäßige Verkürzung ohne bekannte Feldform bleibt mehrdeutig. Nutzerabnahme offen. |

**Nächster auszuführender Schritt:** Die im Audit belegten offenen Schutz- und Nachweislücken vorrangig bearbeiten, als Nächstes mit der Ausführung geprüfter Freigabe-/Hinweis-Erwartungen im Testwerkzeug und weiteren geometrischen Gegenbeispielen. Die dokumentierte Mehrdeutigkeit gleichmäßiger Verkürzung bleibt offen. Die Bestandsprüfung ist durchgeführt; die Regelkonformität der gesamten App ist noch nicht erreicht. Keine neue Bildoptimierung und keine Erledigt-Markierung ohne Prüfung und Nutzerabnahme.

**Gezielte Prüfanleitung vom 23. September 2026:** [Testplan für die korrigierte Perspektivsperre](../iro-gen/testplans/perspektivkorrektur-konturpruefung.md) mit 22 bestehenden Geometriefällen, Testbefehl, Berichtsausgabe und Abnahmecheckliste. Dokumentationsarbeit auf Grundlage von Planfassung 1.30; keine neue fachliche Anforderung, kein neuer Testlauf und keine Abnahme dadurch.

**Klargestellter Nutzerauftrag, 23. September 2026:** [Ladbarer IroGen-JSON-Plan](../iro-gen/testplans/perspektivkorrektur-konturpruefung.json) mit zehn Bildern umgesetzt. Feldbreitenoption ergänzt; PNG-Übergabe, Analyse und Export geprüft. Probelauf: neun Erwartungen erfüllt, waagerechte Verjüngung mit Abdunklung/Rauschen gibt fälschlich zwei Felder frei. Diese Schutzlücke hat weiter Vorrang. [Anleitung und Befunde](../iro-gen/testplans/perspektivkorrektur-konturpruefung.md). Nutzerabnahme und Erledigt bleiben offen.

**Nutzerlauf vom 23. September 2026 bestätigt:** [Übermittelter Bericht mit zehn Bildern](../iro-gen/testplans/iro-testbericht-20260923-114032.md), Generator 1.4.0 / Analyse 0.5.2, vollständig und ohne Verarbeitungsfehler. Vier unterschiedliche Rechteckbreiten und zwei Kontrollen jeweils 3/3 Felder freigegeben; drei Verjüngungsfälle korrekt gesperrt. Waagerechte Verjüngung mit Abdunklung/Rauschen weiterhin 2/3 freigegeben statt vollständiger Sperre: neun von zehn Erwartungen erfüllt. Keine Nutzerabnahme aus der Berichtsübermittlung ableiten. Als Nächstes diese reproduzierte Fehlfreigabe beheben; derselbe JSON-Plan bleibt die Gegenprobe.
