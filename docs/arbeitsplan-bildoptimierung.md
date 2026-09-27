# Arbeitsplan: Bildoptimierung ohne Verfälschung der Farbmessung

Stand: 27. September 2026. Grundlage: [konsolidierter Plan, Fassung 1.55](../IRO-KONSOLIDIERTER-PLAN.md) und das ausdrücklich angenommene [Entscheidungsprotokoll](entscheidung-bildoptimierung-2026-09-22.md). Dieser Plan konkretisiert die beschlossene Arbeit; er ersetzt keine fachliche Festlegung.

**Oberstes Grundprinzip vor jedem Arbeitsschritt:** Iro vergleicht Wand und Farbreferenz relativ im selben Foto. Eine auf beide gleich wirkende globale Veränderung ist nicht automatisch schädlich. Vor jeder Aktion den konkreten möglichen Schaden an dieser relativen Beziehung benennen; nach jedem Test Gleich- oder Ungleichwirkung und die tatsächlich erhaltene oder verfälschte relative Aussage dokumentieren. Keine Korrektur, Warnung, Sperre oder Schwelle allein aus einem absolut schlecht wirkenden Bild ableiten.

## Verbindliche Arbeits- und Statusregeln

Die nachstehenden Schritte in der angegebenen Reihenfolge bearbeiten. Unabhängige Arbeiten dürfen während einer ausstehenden Abnahme weitergehen; Voraussetzungen für abhängige Arbeiten müssen erfüllt sein. Kameraintegration und reale Prüfungen beginnen erst nach dem grünen technischen Sammel-Testlauf der Solution. Ausdrückliche Abnahmen werden gemäß Nutzerentscheidung vom 23. September 2026 erst nach ebenfalls grünen realen Gerätetests gebündelt behandelt; bis dahin keine Einzelabnahmen anfordern. Erlaubte Verfahren sind keine Pflicht, jedes Verfahren einzubauen: Bei bedingten Korrekturen zunächst Bedarf und Nutzen untersuchen; ohne Nachweis bleibt die Korrektur deaktiviert.

Jeder Schritt erhält vier getrennte Kontrollkästchen: Umsetzung, bestandene Prüfung, ausdrückliche Nutzerabnahme und Erledigt. **Erledigt darf erst markiert werden, wenn alle drei Voraussetzungen belegt sind.** Bei Dokumentationsaufgaben entspricht Umsetzung der Erstellung der Vereinbarung. Bei Untersuchungen ist ein belegtes Ergebnis „kein Nutzen, deshalb nicht aktivieren“ zulässig; das ist keine implementierte Korrekturfunktion. Ein leerer Testlauf, erfolgreicher Build oder übermittelter Testbericht ist keine fachliche Abnahme. Der Nutzer hat das Protokoll beschlossen, damit aber keine noch zu prüfende Implementierung abgenommen.

Pro Schritt festhalten: Änderungsdatum, betroffene Code-/Dokumentfassung, tatsächlich ausgeführte Prüfungen mit Ergebnis und Link, offene Einschränkungen, Abnahmeentscheidung mit Datum und Bezug auf die Nutzerantwort. Abnahmefragen verwenden die unten formulierten Verhaltensbeschreibungen, keine bloßen Kennungen. Bei späteren Änderungen betroffene Prüfungen und Abnahmen ausdrücklich wieder öffnen; alte Nachweise erhalten.

## Pflichtkontrolle vor dem nächsten Arbeitspunkt

**Verbindliche Zwischenkontrolle, Nutzerentscheidung vom 23. September 2026:** Nach Umsetzung und gezielter Prüfung eines Arbeitspunkts, bevor er als technisch potenziell abnahmefähig gilt und der nächste Punkt begonnen wird, zwei feste Kontrollbilder neu erzeugen und durch Iro auswerten: eines ohne Störungen und eines mit einer bereits nachweislich beherrschten leichten Störung. Vorab festgelegte Freigabe, Feldzuordnung, Messwerte und Rangfolge prüfen; Seeds, Softwarestand und Befund sichern. Nur bei grünem Ergebnis weitergehen, sonst die Regression zuerst beheben. Diese begrenzte Kontrolle ergänzt gezielte Tests und ersetzt weder den Schluss-Sammel-Testlauf noch reale Gerätetests oder ausdrückliche Abnahmen. [Ladbarer Plan](../iro-gen/testplans/zwischenkontrolle-zwei-bilder.json).

## Vorrang nach dem Entwicklungs- und Regelaudit

**Nutzerauftrag vom 22. September 2026:** Regelverstöße zuerst beheben, Schwerpunkt Kunden-App. [Ausführlicher Audit mit Nachweisen](regelaudit-2026-09-22.md). Die folgenden offenen Sicherheits- und Nachweislücken haben vor neuen Optimierungen Vorrang; sie verweisen auf Inhalte der Schritte unten und bilden kein zweites konkurrierendes Konzept.

| Vorrangige Arbeit in Klartext | Umsetzung | Prüfung | Nutzerabnahme | Erledigt |
|---|---|---|---|---|
| Reproduzierte Teilfreigaben bei Kanalanschlag, erkannter unbrauchbarer Unschärfe und erkanntem Bildbeschnitt beheben | umgesetzt in Analyse 0.5.0 | gezielte Regressionen bestanden | offen | nein |
| Nominale Diagnosen nicht als nachgewiesene Messfehler/Genauigkeit ausgeben | umgesetzt in IroGen-Auswertung | Diagnoseprüfungen bestanden | offen | nein |
| Starke Perspektive nach geprüften Eignungskriterien sperren | Teilprüfung deutlicher Verjüngung mit Konturbelegen in Analyse 0.5.2 | gezielte Gegenproben bestanden; allgemeine Perspektivprüfung offen | offen | nein |
| Unterbelichtung ohne Kanalanschlag relativ bewerten | synthetischer Freigabevertrag umgesetzt; keine pauschale Helligkeitssperre | 16/16 Bild-Erwartungen, acht Kerngegenproben und Zwei-Bilder-Kontrolle bestanden; reale Grenze offen | am Schluss | nein |
| Zuverlässig nachgewiesene stark ungünstige Beleuchtung erkennen und vollständig sperren | offen | offen | offen | nein |
| Verbleibende Lücken bei Vollständigkeit, Unschärfe und flächigen Reflexen schließen | kleine passende Randreste in Analyse 0.5.1 ergänzt; weitere Lücken offen | gezielte Beschnittprüfungen bestanden; insgesamt unvollständig | offen | nein |
| Verifizierte Freigabe-, Hinweis- und Farbwerterwartungen im Testwerkzeug auswerten | Bildbezogene Kriterien und feldbezogene geometrische Zuordnung sowie ausdrücklich geprüfte Messwerte umgesetzt; vollständiger Datensatz offen | Gezielte Gegenproben, Zehn-Bilder-Ablauf und neue Drei-Bilder-Felderwartung bestanden | offen | nein |

Keine zusätzlichen Erkennungsoptimierungen oder Farbkorrekturmodelle beginnen, solange diese Lücken nicht geschlossen oder ihre Grenzen ausdrücklich abgenommen sind. Unabhängige Fehlerbehebungen bleiben autorisiert. Die aktualisierte Reihenfolge lautet: technische Einzelprüfungen, abschließender Sammel-Testlauf, reale Gerätetests, ausdrückliche Abnahmen. Ein bestandener synthetischer Test wird nicht als Kundenfreigabe behandelt.

**Aktueller Umfang, Fassung 1.36:** Ein Bild → eine Analyse → eine Auswertung. Keine bildübergreifende Messhistorie, Feldverfolgung, Glättung oder Rangbestätigung. Die früheren Historienarbeiten sind aufgehoben, nicht als fehlende Produktarbeit weiterzuführen. Räumliche robuste Statistik innerhalb eines Bildes bleibt erhalten. Frühere Prüfvermerke unten beschreiben ausschließlich ihren damaligen Stand.

**Aktueller Code-/Planabgleich, 27. September 2026, Planfassung 1.58:** [Auditbericht](code-plan-abgleich-2026-09-27.md). Sechs rote vorhandene Endpunktfarben-Gegenfälle und widersprüchliche Sperrerwartungen belegen den nächsten technischen Handlungsbedarf; Kanalendpunkt allein beweist keinen relativen Schaden. 73 weitere ausgewählte Kernprüfungen und 25 Generator-/Integrationsprüfungen einschließlich Zwei-Bilder-Kontrolle bestanden. Inventarvorschau rot wegen veralteter Unterbelichtungszahlen; pauschale Reflexschleier-Sperrforderung im Inventar ebenfalls überholt. Danach vollständigen Android-Testmodus und unabhängigen Prüfbestand fertigstellen. Keine Produktänderung, kein Schluss-Sammellauf, kein Gerätetest, keine Abnahme. Frühere grüne Teilbefunde ersetzen diese aktuelle offene Bewertung nicht.

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

**Teilfortschritt vom 27. September 2026, Fassung 1.49:** Feldzuordnung innerhalb eines Bildes und explizit geprüfte ΔE00-Erwartungen sind im Testwerkzeug implementiert. Drei neue synthetische Kontrollen und gezielte fehlerhafte Gegenproben bestanden; nominale Materialdiagnosen bleiben getrennt. [Verträge, genaue Befehle und Grenzen](felderwartungen-pruefung-2026-09-27.md). Vollständiger Datensatz, Gesamtschritt und Abnahme bleiben offen.

- [ ] Umsetzung: Reproduzierbaren IroGen-Testplan für alle zwölf Verfahren einschließlich Kombinationen, beider Streifenrichtungen, passender und unterschiedlicher Farben sowie guter Gegenbeispiele erstellen. Bildbezogene Erwartungen vorab prüfen; Entwicklung und zurückgehaltene Abnahme trennen.
- [ ] Prüfung: Seeds, Generator-/Analyseversionen und Optionen reproduzieren; unveränderte Vergleichsläufe und kompakte Markdown-Exporte sichern. Feldtreffer, übersehene/falsche Felder, Fehlfreigaben, falsche Ablehnungen, Farbfehler, Rangfolge und Hinweise getrennt auswerten.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Die Testfälle enthalten passende und unbrauchbare Aufnahmen mit geprüften Erwartungen. Eine Verbesserung kann weder durch pauschale Ablehnung noch durch ungeprüfte Messwerte vorgetäuscht werden.“

Numerische Toleranzen vor der jeweiligen Abnahme anhand nachvollziehbarer Befunde festlegen. Die Diagnosegrenze eines alten Berichts ist keine neue Genauigkeitszusage. Die bestehende Pflicht zu mehreren hundert Bildqualitätsfällen bleibt bestehen; vorhandene Fälle dürfen nach Prüfung einbezogen werden.

## 4. Erkennung und gemeinsame Farbmessung technisch absichern

**Teilfortschritt vom 25. September 2026:** [Originalpixelvertrag abgesichert](originalpixel-absicherung.md), Analyse 0.5.11. Externe Pufferänderungen können das Original nicht mehr verändern; direkte Drehungsbindung und Messung unveränderter Originalpixel gezielt geprüft. Zehn neue Vertragstests, angrenzende Kern-/Integrationsprüfungen und Zwei-Bilder-Gegencheck bestanden. Ein zusätzlicher produktiver Erkennungskopien-Vertrag ist damit nicht implementiert; Gesamtpunkt und Abnahme offen.


- [ ] Umsetzung: Getrennte Rollen für Erkennungsbild und gemeinsames Messbild, eindeutigen Aufnahmebezug, unveränderte Originaldaten und nachvollziehbare Koordinatenübertragung absichern. Bearbeitete Erkennungspixel dürfen den Farbpfad nicht erreichen.
- [ ] Prüfung: Erkennungskopie gezielt farblich verändern und bei festgehaltenen Messmasken identische Messwerte aus dem Original nachweisen. Zusätzlich echte Erkennung mit veränderten Masken auf richtige Zuordnung prüfen. Andere Frames und ungültige Transformationen zurückweisen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Eine für die Erkennung aufgehellte oder geschärfte Kopie verändert unsere gemessenen Farben nicht. Wand und Farbstreifen werden aus derselben gemeinsamen Aufnahme gemessen.“

## 5. Geraderichten und innere Messflächen absichern

**Teilfortschritt vom 27. September 2026:** [55 unabhängige Bildkonstellationen](gedrehte-messflaechen-schutzpruefung.md) prüfen sichere Innenpolygone, Originalfarben, Druck, weiße Ränder, kleine Flächen und konkurrierende Muster. Zwei Mehrdeutigkeitslücken in Analyse 0.5.12 geschlossen. 117 gezielte Kern-/23 Integrationsprüfungen, anschließend Zwei-Bilder-Gegencheck bestanden. Erweiterte Real- und Grenzfallabdeckung sowie Gesamtpunkt und Abnahme bleiben offen.


- [ ] Umsetzung: Vorhandenes Geraderichten gegen das Protokoll prüfen; Kantenabstand, Originalpolygone, leere Rotationsränder und sichere Innenflächen vervollständigen.
- [ ] Prüfung: Horizontale/vertikale Streifen in verschiedenen Drehungen, kleine Felder, Texte, weiße Zwischenräume und mehrdeutige Kanten prüfen. Rückabbildung und Farbmessung unabhängig kontrollieren.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Verdrehte Streifen werden waagerecht oder senkrecht erkannt. Die Messung verwendet sichere Feldinneren und keine durch Drehung vermischten Randfarben.“

## 6. Moderate Perspektive auswerten und starke Perspektive ablehnen

**Teilfortschritt vom 27. September 2026:** [Geometrische Originalpixelmasken](perspektivmasken-schutzpruefung.md), Analyse 0.5.13, beseitigen die reproduzierte Fehlablehnung homogener Felder bei leichter Perspektive. 40 neue Perspektivanalysen einschließlich Drehung/Rauschen, direkte Maskentests und PNG-Gegenproben bestanden. Starke Gegenfälle bleiben gesperrt. Allgemeine Entzerrung und reale Eignungsgrenzen sowie Gesamtschritt/Abnahme offen.


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

**Aktuelle Entscheidung vom 27. September 2026, Fassung 1.56:** Die frühere pauschale Sperrforderung für jede gleichmäßige Reflexüberlagerung ist ersetzt. Wirkt eine globale Veränderung gleichmäßig auf Wand und Farbstreifen und bleibt der relative Vergleich nachweislich zuverlässig, ist sie nicht allein wegen des absolut veränderten Bildaussehens abzulehnen. Numerische ΔE00-Werte beschreiben dabei nur den relativen Abstand in der aktuellen Aufnahme, keine absolute Materialfarbe. Räumlich ungleiche Reflexe, Clipping, verlorene Information und nachgewiesene falsche Feldzuordnungen bleiben unbrauchbar. Keine Reflexursache aus einem ununterscheidbaren Einzelbild erraten und keine pauschale Helligkeits- oder Farbsperre ergänzen.

**Historischer Teilfortschritt vom 24. September 2026:** [108 Reflexbilder und begrenzte Korrektur](reflexe-schutzpruefung.md). Vier Lichtband-Fehlzuordnungen in Analyse 0.5.10 geschlossen; zwölf gleichmäßige Reflexüberlagerungen waren nach damaliger pauschaler Sperrforderung ungelöst. Die aktuelle relative Bewertung steht oben; räumlich ungleiche Reflexe und reale Gerätefälle bleiben offen.


- [ ] Umsetzung: Viele innere Originalpixel robust auswerten; auffällige Einzelpixel und kleine Glanzstellen ausschließen, Mindestfläche und Ausschlussanteil prüfen. Klassische Filter nur konservativ, bildweit mit denselben Regeln, ohne Kantenvermischung und mit Nutzennachweis erproben.
- [ ] Prüfung: Rauschen, Glanz, Text und kleine Felder gegenüber ungestörten Referenzen testen. Flächige Reflexe müssen zur Ablehnung oder einer nachweislich geeigneten alternativen Messfläche führen; keine regional unterschiedliche Filterung.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Einzelne Störpixel beeinflussen die Messung möglichst wenig. Große Reflexe oder zu wenig verbleibende Messfläche liefern keinen erfundenen Farbwert.“

## 9. Clipping, Dunkelheit und starke Unschärfe zuverlässig sperren

**Konkretisierung vom 23. September 2026:** Unbrauchbare Fokus- oder Bewegungsunschärfe strikt aufnahmeweit sperren; keine Rekonstruktion oder teilweise Rettung. Erneute Aufnahme verlangen. Analyse 0.5.5 schließt eine reproduzierte Rauschmaskierung der Kantenprüfung; [Nachweis und verbleibende Grenzen](unschaerfe-schutzpruefung.md). Der Gesamtpunkt bleibt offen.

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

**Entschieden am 23. September 2026:** Zuverlässig nachgewiesene stark ungünstige Beleuchtung strikt aufnahmeweit ablehnen, ohne Korrektur; auf gleichmäßige Beleuchtung hinweisen. Kleine Unterschiede nur bei tatsächlicher Erkennbarkeit und belegtem Nutzen eines gemeinsamen Ganzbildverfahrens als Korrekturkandidaten behandeln. Keine getrennte Nachbearbeitung von Wand und Streifen, auch nicht in Erkennungskopien. Offen sind belastbare Erkennungskriterien, keine pauschale Schattenursache aus Helligkeitsunterschieden erfinden. Siehe [Prüfstand](ganzbild-und-beleuchtung.md).

**Abnahme in Klartext:** „Sicher erkannte stark ungünstige Beleuchtung sperrt das gesamte Bild und fordert gleichmäßige Beleuchtung. Wand und Streifen werden niemals getrennt nachbearbeitet. Kleine Korrekturen sind nur gemeinsam und nachgewiesen zulässig.“

## 11. Gemeinsame Helligkeits- und Farbkorrektur nur bei belegtem Nutzen

- [ ] Umsetzung: Bedarf für globale Helligkeitskorrektur und gemeinsamen Weißabgleich/Farbkalibrierung untersuchen. Voraussetzungen, vertrauenswürdige Referenzquellen, Modell, Farbraum, Parameter und Geltungsbereich dokumentieren. Ohne Nutzenbeleg bleibt der Messpfad unkorrigiert.
- [ ] Prüfung: Dasselbe Modell auf das gesamte Bild anwenden; Reflex-/Schatten-/Clippingfälle und falsche Referenzen abweisen. Mit unabhängigen Farbpaaren und getrennten Kalibrier-/Prüfdaten unkorrigiert gegen korrigiert vergleichen; Nullvergleich allein genügt nicht. Farbabstände, Rangfolge, Fehlerverteilung und Verschlechterungen prüfen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Eine gemeinsame Korrektur wird nur im belegten Geltungsbereich verwendet. Sie behandelt Wand und Streifen gleich und verbessert die Farbmessung nachweislich. Andernfalls bleibt sie ausgeschaltet.“

Keine Sollfarben aus dem Testbericht als versteckte Analyzer-Eingaben verwenden. Eine zuverlässig bekannte Kalibrierreferenz braucht einen ausdrücklich dokumentierten Eingabevertrag. Reale Gültigkeit darf nicht allein aus synthetischen Versuchen abgeleitet werden; gegebenenfalls bleibt dieser Punkt bis zur Gerätephase offen. Keine ungefragte Endanwender-Kalibrierfunktion ergänzen.

## 12. Jedes Bild unabhängig auf Eignung prüfen und auswerten

- [ ] Umsetzung: Nachgewiesene Eignung sichtbarer Streifenflächen einschließlich möglichem Beschnitt, Perspektive, Schärfe, Clipping, Licht, vergleichbare Beleuchtung, Schatten/Reflexe, kurzfristige Ruhe und Messflächen vor Freigabe zusammen prüfen. Zuerst mit Testbildfolgen arbeiten; keine feste Feldzahl oder grundlose Bewegungssperre einführen.
- [ ] Prüfung: Angeschnittene Streifen gemäß Entscheidung vom 24. September nur im belegten Eignungsbereich freigeben, sonst ablehnen; wieder geeignete Frames freigeben. Wand und Feld ausschließlich innerhalb desselben Bildes vergleichen. Ein vorheriges Bild darf Werte, Rangfolge oder Freigabe des aktuellen Bildes nicht beeinflussen; keine bildübergreifende Feldzuordnung oder Messhistorie.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Iro wertet jedes eingehende Bild unabhängig aus. Ungeeignete Bilder zeigen keine unzulässigen Werte; geeignete Bilder liefern ihren eigenen Vergleich ohne Einfluss vorheriger Messungen.“

## 13. Hinweise, Diagnose und Datenverträge vervollständigen

- [ ] Umsetzung: Verwendeten Erkennungsweg, geometrische Transformation, gemeinsame Messgrundlage, gegebenenfalls Korrekturmodell/Version und Qualitätsgründe nachvollziehbar machen. Erforderliche maschinenlesbare Vertragserweiterungen vor Nutzung versionieren; kompakte Auswertung in Klartext ergänzen.
- [ ] Prüfung: API, Export, Wiederholung eines Laufs und Anzeige fehlender Werte kontrollieren. Kein Messwert ist nicht gleich Nullabstand. Alte oder abgelehnte Werte verschwinden; unbekannte Ursachen bleiben als unbekannt gekennzeichnet.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Ich erkenne im Bericht, was Iro bearbeitet hat, woraus gemessen wurde und warum eine Aufnahme abgelehnt wurde. Hinweise und Abnahmefragen sind ohne technische Kennungen verständlich.“

## 14. Abschließenden technischen Sammel-Testlauf mit unabhängigen Fällen durchführen

- [ ] Umsetzung: Übernommene Verfahren, Regeln, UI und Testmodus zusammenführen; kompakte Vorher-/Nachher-Übersicht mit Einschränkungen erstellen. Bedingte, nicht nachgewiesene Korrekturen bleiben deaktiviert.
- [ ] Prüfung: Alle bis dahin durchgelaufenen Tests mit dem gemeinsamen aktuellen Stand erneut ausführen: Kern-, Generator-, API-, Regressions- und Emulatorprüfungen sowie zurückgehaltenen Abnahmesatz. Fehlerquoten und Messgüte gegen vorab vereinbarte Kriterien prüfen; kein Nachjustieren am Abnahmesatz. Laufzeit und Abbruch prüfen.
- [ ] Abnahme.
- [ ] Erledigt.

**Abnahme in Klartext:** „Der vollständige technische Messablauf erfüllt die vereinbarten Kriterien auch bei unabhängigen Testfällen. Die verbleibenden Gerätenachweise sind ausdrücklich benannt.“

## 15. Kamera und reale Messgüte nach grünem Sammel-Testlauf prüfen

- [ ] Umsetzung: Erst nach grünem technischem Sammel-Testlauf Kamera anbinden und dieselben Regeln auf echte Frames anwenden; AE/AWB-Schalterentscheidung beibehalten. Reale Versuche mit dokumentierter Beleuchtung, Materialien, Geräteeinstellungen und Referenzen vorbereiten.
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

**Nächster auszuführender Schritt:** Die verbleibenden Schutzlücken der Einzelbildanalyse bearbeiten, insbesondere Unterbelichtung, ungleiche Beleuchtung und Vollständigkeit. Feldverfolgung, Glättung und Historienintegration entfallen durch die Nutzerentscheidung in Fassung 1.36. Die Zwei-Bilder-Kontrolle bleibt vor jedem nächsten Arbeitspunkt erforderlich.

**Gezielte Prüfanleitung vom 23. September 2026:** [Testplan für die korrigierte Perspektivsperre](../iro-gen/testplans/perspektivkorrektur-konturpruefung.md) mit 22 bestehenden Geometriefällen, Testbefehl, Berichtsausgabe und Abnahmecheckliste. Dokumentationsarbeit auf Grundlage von Planfassung 1.30; keine neue fachliche Anforderung, kein neuer Testlauf und keine Abnahme dadurch.

**Klargestellter Nutzerauftrag, 23. September 2026:** [Ladbarer IroGen-JSON-Plan](../iro-gen/testplans/perspektivkorrektur-konturpruefung.json) mit zehn Bildern umgesetzt. Feldbreitenoption ergänzt; PNG-Übergabe, Analyse und Export geprüft. Probelauf: neun Erwartungen erfüllt, waagerechte Verjüngung mit Abdunklung/Rauschen gibt fälschlich zwei Felder frei. Diese Schutzlücke hat weiter Vorrang. [Anleitung und Befunde](../iro-gen/testplans/perspektivkorrektur-konturpruefung.md). Nutzerabnahme und Erledigt bleiben offen.

**Nutzerlauf vom 23. September 2026 bestätigt:** [Übermittelter Bericht mit zehn Bildern](../iro-gen/testplans/iro-testbericht-20260923-114032.md), Generator 1.4.0 / Analyse 0.5.2, vollständig und ohne Verarbeitungsfehler. Vier unterschiedliche Rechteckbreiten und zwei Kontrollen jeweils 3/3 Felder freigegeben; drei Verjüngungsfälle korrekt gesperrt. Waagerechte Verjüngung mit Abdunklung/Rauschen weiterhin 2/3 freigegeben statt vollständiger Sperre: neun von zehn Erwartungen erfüllt. Keine Nutzerabnahme aus der Berichtsübermittlung ableiten. Als Nächstes diese reproduzierte Fehlfreigabe beheben; derselbe JSON-Plan bleibt die Gegenprobe.

**Aktueller Abschluss der gezielten Korrektur, 23. September 2026:** Analyse 0.5.3 beseitigt den übersprungenen Zwei-Feld-Verjüngungsschutz. 28 gezielte Geometriefälle und unveränderter IroGen-Plan mit 10/10 erfüllten Erwartungen; anschließend alle 96 Kern- und 123 Integrationstests bestanden. Schema-/Export-/Ergebnisvertragsprüfungen bestanden. [Nachweise und Grenzen](geometrie-schutzpruefung.md). Umsetzung und technische Prüfung dieses Teilpunkts abgeschlossen; Nutzerabnahme und Erledigt offen. Allgemeine Perspektiverkennung weiterhin unvollständig.

Abschließender Release-Build der gesamten Solution einschließlich Android bestanden: 0 Fehler, 8 bekannte XAML-Bindungswarnungen (XC0022). Keine neue Geräteprüfung. Für den Nutzer-Gegenlauf denselben JSON-Plan mit neu gestarteter IroGen-Instanz und Analyse 0.5.3 verwenden.

**Nutzer-Gegenlauf nach Korrektur, 23. September 2026:** [Übermittelter Bericht](../iro-gen/testplans/iro-testbericht-20260923-123744.md), Generator 1.4.0 / Analyse 0.5.3, 10/10 verarbeitet ohne Fehler. Alle zehn Erwartungen erfüllt: sechs geeignete Bilder jeweils 3/3 Felder freigegeben; vier Verjüngungsfälle vollständig gesperrt mit Frontalhinweis, einschließlich der zuvor fehlerhaften waagerechten Kombination. Umsetzung und Prüfung der begrenzten Korrektur damit auch im Nutzerlauf bestätigt. Ausdrückliche Nutzerabnahme und Erledigt bleiben offen; allgemeine Perspektivprüfung nicht abgeschlossen.

**Fortschritt zu Vergleichsdaten und verständlicher Diagnose, 23. September 2026:** Erste automatische Verhaltenserwartungsprüfung umgesetzt und gezielt geprüft; [Vertrag, Nachweise und Gegenlauf](verhaltenserwartungen.md). Schritte 3 und 13 bleiben insgesamt offen, weil der vollständige Datensatz, weitere Verträge und Abnahmen noch fehlen. Kein neuer Messalgorithmus und keine Genauigkeitsfreigabe.

**Abschlussprüfung der Erwartungsauswertung, 23. September 2026:** Nach Implementierung alle 96 Kern- und 129 Generator-/Integrationstests bestanden, keine übersprungen. Gezielt zuvor Gegenproben mit falscher Freigabe/Feldzahl/Hinweis, ungeprüften und alten Daten sowie echtem Dialogfilter geprüft. Neuer Testplan v2, neuer Ergebnislauf v2 und historische Ergebnisläufe schema-validiert; Aufnahmebeispiele einschließlich fünf ungültiger Gegenbeispiele bestanden. Der Zehn-Bilder-Lauf meldet automatisch 10 erfüllt / 0 nicht erfüllt / 0 nicht bewertet / 0 Prüffehler. Nutzer-Gegenlauf und Abnahme offen.

Abschließender vollständiger Release-Build zur Erwartungsauswertung einschließlich Android erfolgreich: 0 Fehler, 8 bekannte XAML-Bindungswarnungen. Keine neue Geräteprüfung. Ausgelieferter Prüfstand für den Gegenlauf: IroGen 1.5.0 / Analyse 0.5.4.

**Nutzer-Gegenlauf der automatischen Erwartungsprüfung, 23. September 2026:** [Neuester Bericht](../iro-gen/testplans/iro-testbericht-20260923-133659.md) mit IroGen 1.5.0 / Analyse 0.5.4: 10/10 verarbeitet, automatisch 10 erfüllt / 0 nicht erfüllt / 0 nicht bewertet / 0 Prüffehler. Die Einzelbefunde bestätigen sechs vollständig messbare Bilder mit jeweils drei Feldern sowie vier vollständige Sperren mit Frontalhinweis. Soll-Ist-Tabelle, Grundlagen und nominale Diagnose sind getrennt exportiert. Nutzer-Gegenlauf bestanden; ausdrückliche Abnahme bleibt offen. Dieser Lauf allein prüft nicht erneut die absichtlich fehlerhaften Gegenproben oder Altdatenkompatibilität.

### Aktualisierte Abnahmereihenfolge am 23. September 2026

Ausdrücklicher Nutzerauftrag: Abnahmen gesammelt am Schluss behandeln. Zuerst die Einzelpunkte technisch umsetzen und prüfen, dann alle bisherigen Tests in einem Sammel-Testlauf wiederholen, danach bei grünem Ergebnis reale Tests am Gerät. Erst wenn auch diese grün sind, die verständlich formulierten ausdrücklichen Abnahmen durchführen. Bei Fehlern Korrektur und betroffene Prüfungen wiederholen; Abnahme erst für den belegten Endstand. Geräteabhängige Arbeiten gehören zur Gerätephase und werden nicht als Voraussetzung ihrer eigenen Erprobung verlangt. Der Status „Erledigt“ bleibt bis zur Abnahme offen. Keine Einzelabnahmefragen während der weiteren Umsetzung.

**Historische Unterbelichtungs-Voruntersuchung vom 23. September 2026:** [Acht Kerngegenproben und damaliger 16-Bilder-PNG-Lauf](unterbelichtung-untersuchung.md) dokumentierten vier geprüfte Dunkelheitskontrollen und zwölf zunächst unbewertete Belichtungsfälle. Die zwölf Fälle sind mit Fassung 1.57 relativ bewertet; historische Ergebnisse bleiben erhalten.

**Messstabilität, Untersuchung am 23. September 2026:** [24 Folgen mit je zwölf Frames](messstabilitaet-untersuchung.md), gemeinsamer Wand-Feld-Vergleich, Rohwertstatistik und Offline-Fünfermedian ausgewertet. Drei gezielte Tests bestanden, einschließlich absichtlich falscher Frame-Paarung und produktiver Anzeige bei ungeeignetem Zwischenbild. Produktcode unverändert; keine neue Freigabeschwelle, kein aktivierter Glätter. Teilbeitrag zu Schritten 3, 9 und 12; Unterbelichtung, produktive Historienverwaltung und Gesamtprüfung bleiben offen.

**Zwischenkontrolle und Historienkern, 23. September 2026:** [Zwei feste Kontrollbilder](zwischenkontrolle-zwei-bilder.md) vor Beginn und nach Abschluss der neuen Kernkomponente bestanden. [Historienvertrag Version 1](bildfolgen-historienvertrag.md) implementiert; 17 neue und elf angrenzende Tests bestanden. Abgegrenzter Kernteil technisch geprüft und Zwischenkontrolle grün. Schritte 12 und 13 insgesamt weiterhin offen: automatische Zuordnung, Frame-Adapter, Rangbestätigung, Anzeigeintegration und Geräteparameter fehlen. Keine vorgezogene Abnahme.

Android-Release-Build nach Ergänzung des Historienkerns erfolgreich: 0 Fehler, 8 bekannte XAML-Bindungswarnungen. Keine Geräteausführung; Schluss-Sammel-Testlauf weiterhin ausstehend.

**Rückbau am 23. September 2026, Fassung 1.36:** Nach ausdrücklicher Nutzerentscheidung den neu ergänzten Historienkern und seine 17 Tests entfernt. Unabhängige Einzelbildanalyse bleibt bestehen. Früher genannte Tracker-, Historien- und Glättungsaufgaben sind aus dem Produktumfang gestrichen; keine Abnahme dieser entfallenen Funktionen erforderlich.

**Prüfung nach Rückbau, 23. September 2026:** 19 gezielte Einzelbild-/Schutz-/Anzeigeprüfungen bestanden; anschließend beide Kontrollbilder einschließlich Feldzuordnung, Werten und Rangfolge bestanden. Keine übersprungenen Tests. Kein Schluss-Sammel-Testlauf und keine neue Geräteprüfung.

**Unschärfekorrektur am 23. September 2026:** Drei Fehlfreigaben und ein fehlender Unschärfehinweis vor Korrektur reproduziert. Analyse 0.5.5; 63 gezielte Kernprüfungen und 15 angrenzende Generator-/Integrationsprüfungen bestanden. Neuer Zwölf-Bilder-Plan mit 12/12 erfüllten Erwartungen; Zwei-Bilder-Kontrolle danach grün. Keine vollständige Test-Suite oder Geräteprüfung vorgezogen. [Befund](unschaerfe-schutzpruefung.md).

Android-Release-Build zur Unschärfekorrektur bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein neuer Geräte- oder Sammel-Testlauf.

**Ganzbildregel und Beleuchtung, 23. September 2026:** Nutzerentscheidung in Fassung 1.38 übernommen. Aktuellen Originalpixel-, Erkennungs-, Drehungs- und Messpfad gezielt geprüft: keine getrennte photometrische Wand-/Streifenkorrektur gefunden. Die vorhandene räumliche Flächenprüfung beweist keine eindeutige Beleuchtungsursache. Keine neue automatische Korrektur oder unbelegte Beleuchtungssperre aktiviert. Schritt 10 bleibt in Umsetzung und Gesamtprüfung offen.

**Beleuchtungsuntersuchung und Teilkorrektur, 23. September 2026, Fassung 1.39:** [32 unabhängige Untersuchungsbilder und Gegenbeispiele](beleuchtung-untersuchung.md). Drei Teilfreigaben bei nachgewiesener starker Flächenungleichmäßigkeit vor Änderung reproduziert; Analyse 0.5.6 sperrt solche Aufnahmen vollständig. 81 gezielte Kernprüfungen, 16 Integrationsprüfungen einschließlich Acht-Bilder-Generatorplan sowie die Zwei-Bilder-Zwischenkontrolle bestanden. Untersuchung enthält ausdrücklich offene Befunde: vier unentdeckte Beleuchtungsunterschiede zwischen homogenen Flächen; eine gleichmäßig abgedunkelte, leicht verrauschte Kontrolle mit nur zwei statt drei erkannten Feldern. Diese Befunde bleiben unerfüllt; bestandene Untersuchungsdurchführung ist keine fachliche Freigabe. Nächste konkrete Fehleruntersuchung: fehlendes Feld in dieser Kontrolle. Keine Helligkeitssperre, keine automatische Lichtkorrektur. Schritt 10, Gesamtprüfungen und Abnahmen bleiben offen.

Android-Release-Build zur Beleuchtungs-Teilkorrektur bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Geräte- oder Schluss-Sammel-Testlauf.

**Nutzer-Gegenlauf am 24. September 2026:** [Übermittelter Acht-Bilder-Bericht](../iro-gen/testplans/iro-testbericht-20260924-111222.md), IroGen 1.5.0 / Analyse 0.5.6: 8/8 verarbeitet, acht Verhaltenserwartungen erfüllt, keine Prüf- oder Verarbeitungsfehler. Vier Kontrollen jeweils 3/3 Felder ohne Qualitätshinweis; vier starke Schattenfälle vollständig gesperrt mit Hinweis auf ungleichmäßige Messflächen. Beide Streifenrichtungen und jeweils ohne/mit leichtem Rauschen enthalten. Keine Teilfreigaben. Bestätigt den begrenzten Generator-Gegenlauf, nicht die allgemeine Beleuchtungserkennung oder die drei unabhängigen Querverlauf-Regressionen erneut. Die offene Schattenkonstellation zwischen homogenen Messflächen und das fehlende Feld bei gemeinsamer Abdunklung bleiben ungetestet durch diesen Bericht und weiterhin offen. Keine ausdrückliche Abnahme; keine neuen lokalen Tests allein für diese Dokumentation.

**Fehlende Felderkennung gezielt korrigiert, 24. September 2026:** [Ursache und Nachweise](felderkennung-rauschstart.md). Einzelner verrauschter Startpixel schlug ein Feld der Wand zu; in zwei Gegenproben vor Änderung reproduziert. Analyse 0.5.7 verwendet einen begrenzten gleichmäßigen Pilotbereich nur als Klassifikationswert. 24 Varianten mit geprüften Grenzen und Originalfarben, insgesamt 91 gezielte Kernprüfungen und 18 Integrationsprüfungen bestanden; abschließende Zwei-Bilder-Kontrolle grün. Die zwölf Kontrollerwartungen der 32-Bilder-Untersuchung sind jetzt erfüllt. Der frühere konkrete Folgepunkt fehlendes Feld bei gemeinsamer Abdunklung ist technisch bearbeitet und geprüft; keine Abnahme. Unentdeckte Beleuchtungsunterschiede zwischen homogenen Flächen sowie allgemeine Vollständigkeit bleiben offen. Kein Schluss-Sammellauf oder Gerätetest vorgezogen.
Android-Release-Build zur Felderkennungskorrektur bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Schluss-Sammellauf oder Gerätetest.

**Erneuter Gegencheck auf Nutzerwunsch, 24. September 2026:** Zwei Bilder neu erzeugt und mit dem aktuellen Stand geprüft: saubere Aufnahme und bekannte leichte Rauschstörung. Automatisierte Prüfung von Freigabe, Feldzuordnung, Messwerten und Rangfolge bestanden; keine übersprungenen Tests. [Bericht](../tests/adjustments/zwischenkontrolle-20260923/iro-run-ccbe0f19c24a487abbce2173392dac0a/bericht.md). Keine Abnahme oder vollständiger Sammel-Testlauf.

**Bedingte Messbarkeit bei Beschnitt, 24. September 2026, Fassung 1.41:** Nutzer ersetzt die dauerhafte Pauschalsperre durch nachgewiesene Eignung und konservative Unsicherheitsgrenzen. [64 synthetische Bilder untersucht](beschnitt-untersuchung.md): homogene Restflächen können unter bekannter korrekter Maske messbar sein; gestörte Randreste werden teilweise übersehen. Keine neue Freigabeschwelle aus den lokalen Messwerten abgeleitet, Produktcode unverändert. Zwei neue Untersuchungstests und anschließender Zwei-Bilder-Gegencheck bestanden. Schritt 12 in Umsetzung und Gesamtprüfung weiterhin offen. Als nächste konkrete Arbeit gestörte Randfortsetzungen und verständlich eingeschränkte Teilvergleiche absichern, danach Ende-zu-Ende-Freigabegrenzen prüfen. Frühere pauschale Beschnitt-Abnahmekriterien sind durch die neue Entscheidung überholt; historische Testergebnisse bleiben historische Nachweise.
**Gestörte Randreste und Teilmessung abgesichert, 24. September 2026:** [Ursache und Nachweise](randrest-schutzpruefung.md), Analyse 0.5.8. 16 reproduzierte Fehlfreigaben an vier Bildseiten behoben; vier Gegenfälle mit unabhängigen Randobjekten bleiben messbar. Teilmessungen nennen die auswertbaren/erkannten Felder und begrenzen Vergleich samt nächstem Treffer auf diese sichtbaren Felder. 108 gezielte Kern- und 19 Integrationsprüfungen bestanden, anschließend Zwei-Bilder-Gegencheck grün. Dieser begrenzte Teil technisch umgesetzt und geprüft; allgemeine Randrest-/Vollständigkeitserkennung, bedingte Beschnittfreigabe, Schritt 12, Schlussprüfungen und Nutzerabnahme bleiben offen. Nächste Arbeit: Freigabebereich geeigneter Beschnitte mit tatsächlicher Erkennung absichern; keine vorzeitige Lockerung.
Android-Release-Build zum Randrestschutz bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Schluss-Sammellauf oder Gerätetest.

**Begrenzte Endbeschnittfreigabe, 24. September 2026:** Analyse 0.5.9, [Regeln und Nachweise](endbeschnitt-freigabe.md). Genau ein ausreichend großer und eindeutig zugeordneter Endrest kann nach Originalqualitätsprüfung als Teilvergleich gemessen werden. Vier Fehlfreigaben bei gefleckten Randresten zusätzlich geschlossen. Iro-Gen erhält Teilstatus und Hinweis auch dann, wenn alle sichtbaren Felder zugeordnet sind. 21 neue Kern-Testfälle mit 87 Analysen, insgesamt 129 gezielte Kern- und 25 Integrationsprüfungen bestanden; anschließender Zwei-Bilder-Gegencheck grün. Dieser begrenzte Teil umgesetzt und geprüft. Allgemeine Beschnitt-/Vollständigkeitsprüfung, reale Grenzen, Schritt 12 insgesamt, Schluss-Sammellauf und ausdrückliche Abnahme bleiben offen.
Android-Release-Build zur begrenzten Endbeschnittfreigabe am 24. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest und kein Schluss-Sammellauf.

**Reflexe und Glanzstellen, 24. September 2026:** Breite Untersuchung mit 108 Störungsbildern, acht sauberen/hellen Kontrollen und zwei echten Rechteck-Gegenbeispielen. Vier reproduzierte mehrdeutige Lichtband-Freigaben gesperrt; zwölf gleichmäßige Überlagerungen ausdrücklich offen. 138 gezielte Kern- und 27 Integrationsprüfungen bestanden, anschließend Zwei-Bilder-Gegencheck grün. [Befund und Reproduktion](reflexe-schutzpruefung.md). Schritt 8 insgesamt nicht abgeschlossen. Beschnitt bleibt gemäß Nutzerentscheidung bis zur Gerätephase unverändert. Kein Schluss-Sammellauf oder Abnahme.

Android-Release-Build zur Reflex-Teilkorrektur am 24. September 2026 erfolgreich: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest oder Schluss-Sammellauf.

**Historischer Dokumentationsstand vom 24. September 2026:** Damals wurde eine strikte Ablehnung gleichmäßiger Reflexüberlagerung festgehalten. Diese pauschale Sperrforderung ist durch die relative Grundentscheidung in Fassung 1.56 ersetzt; der damalige Nachweis bleibt als Historie erhalten.

**Ganzbildregel am 25. September 2026:** Nutzer bekräftigt das Verbot jeder unabhängigen Teilbildbearbeitung. Original-Snapshot und abgetrennter Export implementiert; drei fehlschlagende Vertragsgegenproben behoben. 132 gezielte Kernprüfungen sowie erweiterter Zehn-Test-Vertragssatz, 27 Integrationsprüfungen und anschließender Zwei-Bilder-Gegencheck bestanden. [Grenzen und Nachweise](originalpixel-absicherung.md). Keine Geräteprüfung, kein Schluss-Sammellauf, keine Abnahme.

Android-Release-Build zur Originalpixel-Absicherung am 25. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest und kein Schluss-Sammellauf.

**Gedrehte Messflächen, 27. September 2026:** Die neuen geeigneten Rand-/Druckfälle benötigen keine Änderung der Farbstatistik. Zweite Musterrichtung wird nun geometrisch gegen das Original geprüft und bei erkannter Konkurrenz gesperrt. Qualitätsgründe behalten Vorrang. [Nachweise und offene Grenzen](gedrehte-messflaechen-schutzpruefung.md). Kein Schluss-Sammellauf, Gerätetest oder Abnahme.

Android-Release-Build zur Messflächen- und Mehrdeutigkeitsabsicherung am 27. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest oder Schluss-Sammellauf.

**Perspektivmasken, 27. September 2026:** 184 gezielte Kernprüfungen und erweiterter abschließender 25-Test-Satz bestanden (überschneidend; 191 unterschiedliche Prüfungen). 27 bestehende plus vier neue Integrationsprüfungen bestanden; anschließender Zwei-Bilder-Gegencheck grün. [Nachweise und Grenzen](perspektivmasken-schutzpruefung.md). Nächste unabhängige Arbeit: automatische Feldzuordnungs- und verifizierte Messwerterwartungen im Iro-Gen-Testbericht ergänzen, ohne nominale Materialabstände als nachgewiesene Sollwerte zu behandeln.

Android-Release-Build am 27. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest und kein Schluss-Sammellauf.

**Erwartungsprüfung und Verdeckung, 27. September 2026:** Die autorisierte DIALOG-Anweisung ist umgesetzt; [vollständiges Prüfprotokoll](felderwartungen-pruefung-2026-09-27.md). Analyse 0.5.14 behebt die reproduzierte Fehlablehnung bei mittlerer Verdeckung; Unschärfe-, Kanalanschlags- und Wandsperren bleiben aktiv. 184 verschiedene gezielte Kernprüfungen und 38 verschiedene Generator-/Integrationstests einschließlich Zwei-Bilder-Kontrolle bestanden; Wiederholungen nicht mehrfach gezählt. Kein Gesamtlauf. Android-Release-Build: 0 Fehler, 8 bekannte Warnungen. Technisch geprüfte Teilbeiträge, keine Abnahme.

**Historische nächste Arbeit nach Fassung 1.49 (inzwischen teilweise umgesetzt):** Im Schritt „Diagnose und verständliche Anzeige“ erkannte Ablehnungsgründe wie Unschärfe, Kanalanschlag und ungleichmäßige Flächen eindeutig maschinenlesbar unterscheiden und im Testwerkzeug gezielt prüfen. Der Sammelcode „anderer Qualitätshinweis“ genügt dafür nicht. Keine unbekannte Ursache als bewiesen ausgeben, keine Sperren lockern. Gleichmäßige Reflexüberlagerung mit zwölf bekannten unerkannten Freigaben sowie die übrigen dokumentierten Grenzen bleiben offen; der Abschluss-Sammellauf ist noch nicht erreicht.

**Präzise Hauptgründe, 27. September 2026, Fassung 1.50:** Teilbeitrag zu „Diagnose und verständliche Anzeige“ umgesetzt und gezielt geprüft. Unbrauchbare Unschärfe, Kanalanschlag und räumliche Ungleichmäßigkeit direkt aus bestehenden Befunden unterscheiden; unbekannte Ursachen bleiben unbestimmt. Keine geänderten Messentscheidungen. 53 Kernprüfungen, 21 Generator-/Erwartungsprüfungen und anschließende Zwei-Bilder-Kontrolle bestanden. Android: 0 Fehler, 8 bekannte Warnungen. [Genaue Befehle, tatsächliche Ausgaben, benötigte Shell und ausgelassene Tests](qualitaetsgruende-pruefung-2026-09-27.md). Gesamtpunkt und Abnahme bleiben offen.

**Historische nächste Arbeit nach Fassung 1.50 (inzwischen umgesetzt):** Erwartete Rangfolge und Gleichstände innerhalb eines einzelnen Bildes explizit im Iro-Gen-Bericht prüfen, als Teil der unabhängigen Vergleichsdaten. Keine bildübergreifende Bestätigung. Weitere offene Schutzgrenzen, Sammellauf und Gerätetests bleiben bestehen.

**Rangfolge und Gleichstände, 27. September 2026, Fassung 1.51:** Als Teil der Vergleichsdaten implementiert und gezielt geprüft: versionierte Ranggruppen samt Prüftoleranz, paarweise maschinenlesbare Befunde und separate Kennzeichnung exakter Minima. Fünf ladbare Bildfälle, unabhängige Referenzrechnung, gezielt falsche Gegenproben, Persistenz und Export bestanden; 50 finale Generator-/Vertragstests und anschließender Zwei-Bilder-Gegencheck grün. [Befehle, tatsächliche Ausgaben und bewusst ausgelassene Prüfungen](rangfolge-pruefung-2026-09-27.md). Keine Änderung des Messkerns; Gesamt-Datensatz und Abnahme weiterhin offen.

**Aktuell nächster offener Teilpunkt:** Den unabhängigen Prüfbestand für den späteren Sammellauf zusammenstellen: vorhandene Bildpläne, vorab geprüfte Erwartungen, bewusst unbewertete Fälle und bekannte Schutzlücken zuordnen. Eine ausführbare Zusammenstellung und ehrliche Restlückenliste erstellen; bekannte Reflex-Fehlfreigaben nicht überdecken. Noch keinen Abschluss-Sammellauf, keine Geräteprüfung oder Abnahme vorziehen.

**Prüfbestand, 27. September 2026, Fassung 1.52:** Alle neun IroGen-JSON-Pläne sind mit 277 Bildern vollständig erfasst: 44 Bilder mit geprüften Erwartungen, 233 diagnostisch oder bewusst unbewertet. Vorschau validiert Schema, reale Planzahlen, vollständige Erfassung, sechs spätere feste Befehle und sieben Restlücken, startet aber keinen Sammellauf. [Ausführbares Inventar und Grenzen](abschluss-pruefbestand-2026-09-27.md). Anschließender Zwei-Bilder-Gegencheck bestanden ([Bericht](../tests/adjustments/zwischenkontrolle-20260923/iro-run-31035ec8b94d4756ae22dc50c034da15/bericht.md)); Gesamtpunkt und Abnahme offen.

**Historische Untersuchung der gleichmäßigen Reflexüberlagerung, 27. September 2026, Fassung 1.53:** Für alle zwölf damaligen Fehlfreigaben wurde eine saubere Szene konstruiert, deren echte homogene Wand-/Feldfarben bytegenau dieselben RGB-Pixel liefern. Damit ist eine zuverlässige unterschiedliche Entscheidung aus dem einzelnen Bild für diese Klasse unmöglich. Keine Helligkeitssperre, Scheinsperre oder Farbkorrektur ergänzt. 23 gezielte Kernprüfungen, vier PNG-Integrationsprüfungen und der anschließende Zwei-Bilder-Gegencheck bestanden. [Beweis und Grenzen](reflexe-einzelbildgrenze-2026-09-27.md). Die damalige offene Produktstrategie ist mit Fassung 1.56 zugunsten des belegten relativen Vergleichs entschieden.

**Robustheit der Feldantwort bei Reflexschleier, 27. September 2026, Fassung 1.54:** IroGen 1.6.0 besitzt eine reproduzierbare ganzbildweite Testskala 0–10 und vier Farbprofile. 880 PNG-Einzelbilder ausgewertet: auf jeder Stufe 80/80 vollständig richtige Antworten, insgesamt keine falschen und keine übersehenen ähnlichsten Treffer; damit 97-%-Ziel bis Stufe 2 und auch Stufe 3 erfüllt. Keine fehlerhafte Stufe, daher keine Warnmerkmal-Untersuchung. Analyzer und Produktlogik unverändert. 76 gezielte Generator-/Testplan-/Berichtsprüfungen und anschließender Zwei-Bilder-Gegencheck bestanden. [Vollständiger Befund und Grenzen](reflexschleier-robustheit-2026-09-27.md).

**Relative Reflexbewertung, 27. September 2026, Fassung 1.56:** Die offene Entscheidung ist durch das oberste Grundprinzip beantwortet. Maßgeblich ist der zuverlässige relative Wand-/Farbfeldvergleich. Der 880-Bilder-Test ist nun ein echter Regressionsvertrag und schlägt bei falschem oder fehlendem passendsten Feld sowie bei Verarbeitungsfehlern fehl. Erneut bestanden: zwei Reflexschleier-Vertragstests mit 880 Bildanalysen, 23 Kernprüfungen und vier PNG-Integrationsprüfungen für räumlich ungleiche Reflexfälle sowie der verpflichtende Zwei-Bilder-Gegencheck. Keine Produktkorrektur, Reflexursachenerkennung oder absolute Genauigkeitszusage ergänzt. Reale Reflexe und numerische Gerätegenauigkeit bleiben für die Gerätephase offen.

**Unterbelichtung relativ abgesichert, 27. September 2026, Fassung 1.57:** Zwölf zuvor unbewertete Ganzbildabdunklungen von −1 bis −3 EV, beide Streifenrichtungen, jeweils ohne/mit leichtem Rauschen, besitzen nun geprüfte Feld- und Rangfolgeerwartungen. Zusammen mit vier dunklen Originalfarbkontrollen 16/16 erfüllt. Acht Kerngegenproben, 22 angrenzende Testplan-/Rangfolgeverträge und Zwei-Bilder-Gegencheck bestanden. Ein roter Erstlauf legte eine falsche neue Annahme zur expliziten Wandfarbe offen; die Erwartung wurde anhand der festgelegten RGB-Werte korrigiert, Iro nicht an den Test angepasst. Keine Produktlogik, Korrektur oder Helligkeitsschwelle ergänzt.

**Aktuell nächster offener Teilpunkt:** Die bestehende aufnahmeweite Kanalanschlags-/Clipping-Sperre am relativen Grundprinzip auditieren. Nachweisen, wann Endpunktpixel tatsächlich relative Farbinformation vernichten, und sicherstellen, dass echte sehr dunkle, helle oder gesättigte Farben nicht allein wegen ihres absoluten Farbwerts gesperrt werden. Keine Geräte- oder Prozentgrenze ohne belastbare Gegenfälle ändern.
