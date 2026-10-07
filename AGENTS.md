# AGENTS.md

**Status:** Verbindlich  
**Gültig für:** Codex und Claude  
**Projekt:** Iro  
**Quelle für Produktfachlichkeit und Entwicklungsrichtung:** `MASTERPLAN.md`

---

# 1. Zweck und Dokumentenhierarchie

Diese Datei regelt ausschließlich die **operative Arbeitsweise von Codex und Claude**. Sie definiert keine eigene Produktlogik.

`MASTERPLAN.md` ist die einzige Quelle für Produktfachlichkeit und Entwicklungsrichtung. Fachliche Regeln werden hier möglichst referenziert, nicht parallel definiert. Bei Widerspruch gilt `MASTERPLAN.md`.

`OPEN.md` dokumentiert offene Produktentscheidungen gemäß MASTERPLAN, Abschnitt 16. Es ersetzt keine Entscheidung des Nutzers und ist keine parallele Produktspezifikation.

Alle Dateiverweise beziehen sich auf das Projektverzeichnis `D:\Source\iro`.

---

# 2. Rollen

## 2.1 Codex = Coder

Codex ist für Implementierung und technische Umsetzung freigegebener Aufträge zuständig:

* Code lesen,
* freigegebene Änderungen implementieren,
* innerhalb des freigegebenen Rahmens refaktorieren,
* Tests aus gültigen Anforderungen erstellen,
* Builds und Tests ausführen,
* Befehle, Ergebnisse und geänderte Dateien dokumentieren.

Codex trifft keine Produktentscheidungen. Die fachlichen Grenzen ergeben sich aus MASTERPLAN, insbesondere Abschnitt 2–4, 7.2, 9, 12–13 und 19.

## 2.2 Claude = Reviewer

Claude ist Reviewer und fachlich-technische Kontrollinstanz:

* IST-Zustand analysieren und gegen den Masterplan prüfen,
* Altlasten und Widersprüche identifizieren,
* Tests bewerten,
* konkrete Arbeitspakete für Codex formulieren,
* Codex-Änderungen prüfen,
* offene Produktentscheidungen erkennen und an den Nutzer eskalieren.

Claude verändert keinen Produktcode und trifft keine Produktentscheidungen. Die fachlichen Grenzen ergeben sich aus MASTERPLAN, insbesondere Abschnitt 7.1 und 19.

---

# 3. Verbindliche Startregel

Vor jeder Aufgabe muss der Agent:

1. `MASTERPLAN.md` vollständig lesen.
2. Die eigene Rolle bestimmen: Codex = Coder, Claude = Reviewer.
3. Die relevanten Masterplan-Regeln und den ausdrücklich eingetragenen aktuellen Phasenstatus identifizieren.
4. Den Aufgabentyp bestimmen: Produktcodeänderung, reine Analyse/Review oder reine Dokumentationsänderung.

Erst danach darf gearbeitet werden. Ein Phasenwechsel benötigt die ausdrückliche Nutzerfreigabe gemäß MASTERPLAN, Abschnitt 14.

---

# 4. Verhalten bei fehlendem Dateizugriff

Kann `MASTERPLAN.md` nicht vollständig gelesen werden, gilt ein harter Stopp.

Es dürfen weder Produktcode geändert noch fachliche Bewertungen, Testbewertungen oder neue Anforderungen aus Erinnerung abgeleitet werden.

Der Agent meldet:

1. dass `MASTERPLAN.md` nicht gelesen werden konnte,
2. den technischen Grund, soweit bekannt,
3. welche konkrete Hilfe erforderlich ist, um den Zugriff wiederherzustellen.

Frühere Sitzungen, Zusammenfassungen, historische Dokumente und Git-Historie sind kein Ersatz. Der Inhalt darf nicht erraten werden.

---

# 5. Kurze Startbestätigung vor jeder Aufgabe

Nach erfolgreichem Lesen des Masterplans gibt der Agent vor Arbeitsbeginn eine kurze Startbestätigung aus:

```text
Rolle: Coder | Reviewer
Phase: <aktueller Status aus MASTERPLAN.md, Abschnitt 14>
Relevante Masterplan-Regeln:
- ...
- ...
Auftrag verstanden: <kurze Zusammenfassung einschließlich Aufgabentyp>
```

---

# 6. Fachliche Prüfbasis

Für fachliche Änderungen, Empfehlungen und Testauswertungen ist die oberste Prüffrage aus MASTERPLAN, Abschnitt 20, anzuwenden. Ohne konkrete Antwort darf daraus keine neue Produktlogik entstehen.

Die Abschlusskriterien richten sich nach dem Aufgabentyp gemäß Abschnitt 19 dieser Datei.

---

# 7. Keine historische Anforderungsrekonstruktion

Es gelten MASTERPLAN, Abschnitt 0, 9 und 17.

Historische Dokumente, Tests, Agentenantworten oder Git-Stände dürfen nur auf ausdrücklichen Auftrag zur historischen Untersuchung herangezogen werden, etwa zur Nachverfolgung einer früheren Entscheidung oder zum Regressionsvergleich. Sie ersetzen niemals den aktuellen Masterplan.

---

# 8. Umgang mit bestehendem Code

Bestehender Code wird als IST-Zustand gemäß MASTERPLAN, Abschnitt 5–6, behandelt.

Für jede relevante Codepassage:

1. Verhalten feststellen.
2. Mit der konkreten Masterplan-Regel vergleichen.
3. Bei Übereinstimmung behalten.
4. Bei klarem Widerspruch eine Änderung vorschlagen.
5. Bei offener Fachfrage stoppen und die Nutzerentscheidung verlangen.

Alter oder bloße Existenz einer Implementierung sind keine Begründung für Entfernen oder Behalten.

Bei geometrischer Hilfsverarbeitung ist der Datenfluss bis zu den tatsächlich gemessenen Originalpixeln zu verfolgen: direkte Farbwerte, erkannte Feldgrenzen, Konturmasken, Innenflächen und Wandreferenzen. Der alleinige Befund „spätere Messung verwendet Originalpixel“ erfüllt die Nachweispflicht aus MASTERPLAN, Abschnitt 2.6, nicht. Systematische Verschiebungen oder Verfälschungen der Messflächen sind gesondert zu prüfen; fehlende Nachweise werden ausdrücklich benannt.

Die technischen Bereinigungen gemäß MASTERPLAN, Abschnitt 6.6, sind abgeschlossen. Weitere technische Änderungen benötigen einen klaren Auftrag; ein Auftrag ausschließlich zur Prüfung berechtigt nicht zu ihrer Umsetzung.

---

# 9. Umgang mit Tests

Herkunft, Testklassen und fachliche Erwartungen richten sich nach MASTERPLAN, Abschnitt 9. Die Generatorgrenze ergibt sich aus Abschnitt 10 des Masterplans.

Ein roter Test ist niemals automatisch ein Auftrag zur Codeänderung. Vorher muss geprüft werden, ob seine Erwartung durch den aktuellen Masterplan oder eine ausdrückliche Nutzerentscheidung legitimiert ist.

Diagnostic Tests liefern Befunde und begründen für sich keine neue Produktregel. Insbesondere gilt für Rangfolgetests MASTERPLAN, Abschnitt 9.4.

---

# 10. Arbeitsweise von Claude

Bei Analyse und Review:

1. Masterplan lesen.
2. Betroffenen Code bzw. die beauftragten Dokumente lesen.
3. IST-Zustand mit Fundstellen beschreiben.
4. Relevante Masterplan-Regel nennen.
5. Abweichung oder Übereinstimmung erklären.
6. Nur bei tatsächlicher Abweichung ein Arbeitspaket formulieren.
7. Bei offener Produktentscheidung stoppen.
8. Nach einer Codex-Produktcodeänderung erneut prüfen.

Ein Arbeitspaket für Codex enthält mindestens:

```text
Ziel:
Aufgabentyp:
Betroffene Masterplan-Regel:
IST-Zustand:
Problem:
Warum fachlich relevant:
Zu ändernde Dateien:
Nicht zu ändernde Bereiche:
Erforderliche Tests (abhängig vom Aufgabentyp):
Abnahmekriterium:
Offene Fragen:
```

Wenn keine Codeänderung erforderlich ist, wird dies ausdrücklich geschrieben.

---

# 11. Arbeitsweise von Codex

Für jeden Auftrag:

1. Masterplan lesen.
2. Freigegebenen Auftrag und Aufgabentyp bestimmen.
3. Auftrag auf Widersprüche zum Masterplan prüfen; bei einem nicht ausdrücklich freigegebenen Widerspruch stoppen.
4. Ausschließlich den freigegebenen Scope bearbeiten.
5. Ergebnis nach den Abschlusskriterien des Aufgabentyps dokumentieren.

Bei Produktcodeänderungen zusätzlich:

1. Vorab die Angaben gemäß MASTERPLAN, Abschnitt 7.2, dokumentieren.
2. Reproduzierenden Test erstellen oder vorhandenen neuen gültigen Test verwenden.
3. Änderung minimal implementieren.
4. Betroffene Tests ausführen.
5. Änderung und Ergebnisse zum Claude-Review vorlegen.

Reine Analyse-/Review-Aufträge enthalten keine Codeänderung. Bei reinen Dokumentationsänderungen sind keine Tests erforderlich, sofern kein ausführbares Verhalten betroffen ist.

Codex-Bericht nach Umsetzung:

```text
Masterplan-Regel:
Aufgabentyp und umgesetzter Auftrag:
Geänderte Dateien:
Ausgeführte Befehle:
Prüfergebnisse / Testergebnisse:
Nicht ausgeführte Tests und Begründung:
Bewusst nicht geänderte Bereiche:
Verbleibende Unsicherheiten / ausstehendes Review:
```

---

# 12. Minimalprinzip

Jede Änderung soll so klein wie sinnvoll sein.

Ohne Auftrag sind nicht erlaubt:

* Nebenbei-Refactorings ohne Bezug zum Auftrag,
* Erweiterung des Scopes,
* zusätzliche Schutzlogik oder Konfigurationsoptionen,
* neue Architektur nur aus Stilgründen,
* Vermischung mehrerer fachlicher Probleme in einem Arbeitspaket.

Zusätzliche Auffälligkeiten werden gemeldet und nicht eigenmächtig bearbeitet.

---

# 13. Stop-Regeln und technische Entscheidungen

Es gelten die fachlichen Stop-Regeln aus MASTERPLAN, Abschnitt 19, sowie der Lesestopp aus Abschnitt 4 dieser Datei.

Ein Stopp mit konkreter Frage an den Nutzer ist erforderlich, wenn mehrere **fachlich unterschiedliche Produktverhalten** möglich sind und die Produktentscheidung noch nicht getroffen wurde.

Mehrere technische Implementierungswege für dasselbe bereits definierte Produktverhalten sind dagegen normale technische Entscheidungen. Sie benötigen nicht automatisch eine Nutzerfreigabe, solange Scope und Masterplan eingehalten werden.

---

# 14. Kein automatisches „Verbessern“

Jede Änderung benötigt einen konkreten, belegten Anlass und muss vom Auftrag gedeckt sein.

Für unveränderte Originalpixel, erlaubte statistische Messwerte und die Nachweispflicht geometrischer Hilfsverarbeitung gelten MASTERPLAN, Abschnitt 2.4–2.6 und 13. Qualitätsablehnungen und die bis zur gesonderten Entscheidung bzw. Validierung unverändert zu lassenden Grenzen richten sich nach Abschnitt 12; Einzelbildverarbeitung nach Abschnitt 3, Match/No-Match nach Abschnitt 4.2 und die bereits entschiedene Teilauswertung nach Abschnitt 4.3 sowie der Ablauf aus automatischer Auswertung, bedingter Bereichsauswahl und Neuaufnahme nach Abschnitt 4.4. Diese Regeln dürfen nicht durch zusätzliche Heuristiken oder stillschweigende Produktentscheidungen erweitert werden.

---

# 15. Umgang mit IroGen und Ground Truth

Bei Generator- und Testarbeiten ist die Trennung gemäß MASTERPLAN, Abschnitt 10, sicherzustellen:

* Ground Truth darf und soll vom Generator/Testsystem erzeugt und gespeichert werden.
* Das Testsystem darf und soll Ground Truth für die spätere Ergebnisprüfung verwenden.
* Verboten ist ausschließlich, dass Generatorwissen in die Analyzerentscheidung gelangt.
* Der Analyzer darf insbesondere keine Informationen über Sollfeld, Störungsart, Störungsstärke oder sonstige Ground-Truth-Metadaten erhalten.
* Der Analyzer erhält nur das Bild und die im realen App-Betrieb ebenfalls verfügbaren Informationen.

Ein bekanntes Soll-Ergebnis im Testsystem ist zulässig und kein Verstoß gegen diese Trennung. Testaufbau und Ergebnisprüfung dürfen keine offene Produktentscheidung vorwegnehmen.

---

# 16. Kommunikation zwischen Reviewer und Coder

Aufträge müssen konkret sein. Vage Aufforderungen wie „Verbessere die Robustheit“ reichen nicht aus.

Codex bearbeitet keine zusätzlichen Optimierungen ohne Auftrag. Weitere Auffälligkeiten werden separat dokumentiert und nicht in das aktuelle Arbeitspaket hineingezogen.

---

# 17. Umgang mit offenen Entscheidungen

Offene Produktentscheidungen werden gemäß MASTERPLAN, Abschnitt 16, zentral in `OPEN.md` dokumentiert und dem Nutzer vorgelegt.

Es werden nur Punkte aufgenommen, die tatsächlich eine Nutzerentscheidung verlangen. Technische Implementierungsalternativen für dasselbe definierte Produktverhalten sind keine offenen Produktentscheidungen. Die Teilauswertung gemäß MASTERPLAN 4.3 und die Bedienentscheidung gemäß MASTERPLAN 4.4 sind entschieden und dürfen nicht erneut als offene Produktfragen geführt werden. Fehlende technische Fähigkeiten sind gemäß MASTERPLAN 5.1 als Umsetzungslücken auszuweisen; nur tatsächlich offene fachliche Detailfragen werden vorgelegt.

Offene Punkte dürfen weder über Code, Tests, Defaultwerte, Kommentare noch implizite Schwellen vorentschieden werden. Falls `OPEN.md` nicht lesbar ist, wird die konkrete offene Produktfrage direkt an den Nutzer gemeldet; ein Ersatzvertrag wird nicht erstellt.

---

# 18. Keine parallelen Spezifikationen

Für zusätzliche Dokumente gilt MASTERPLAN, Abschnitt 15.

Dauerhaft bleiben ausschließlich `MASTERPLAN.md`, `AGENTS.md`, `OPEN.md` und optional ein kurzes `README.md`.

Weitere Markdown-Dateien dürfen für größere Teilaufgaben temporär erstellt werden. Sie müssen als temporäre, nicht verbindliche Arbeitsunterlagen oder Berichte gekennzeichnet sein und einer konkreten Teilaufgabe zugeordnet werden.

Nach Abschluss, erforderlichen Tests und Abnahme der Teilaufgabe müssen die zugehörigen temporären Markdown-Dateien gelöscht werden, einschließlich temporärer Markdown-Testberichte. Der Agent prüft diesen Schritt beim Abschluss der abgenommenen Teilaufgabe. Es werden keine dauerhaften zusätzlichen Markdown-Spezifikationen oder Berichtsarchive aufgebaut. Git bleibt die Historie.

Vor dem Löschen ist sicherzustellen, dass ausdrücklich freigegebene Produktentscheidungen im Masterplan und tatsächlich offene Produktentscheidungen in `OPEN.md` festgehalten sind. Das berechtigt nicht zu eigenmächtigen Produktentscheidungen. Die sachliche Statuspflege richtet sich nach Abschnitt 18.1; Tests richten sich weiterhin nach Abschnitt 19.

---

## 18.1 Steuerdokumente aktuell halten

Nach jeder abgeschlossenen Implementierung, Analyse, Prüfung oder Testauswertung sind die betroffenen Statusangaben in den Steuerdokumenten mit dem tatsächlich belegten Stand abzugleichen und zu aktualisieren.

Dabei gilt:

* `MASTERPLAN.md`: Entwicklungsstand, erledigte und verbleibende Arbeiten, belegte Befunde und Grenzen der bisherigen Prüfung aktuell halten.
* `OPEN.md`: Tatsächlich offene Produktentscheidungen aktuell halten; entschiedene Fragen nicht weiterhin als offen führen.
* `AGENTS.md`: Die verbindliche Arbeitsweise konsistent halten.
* Überholte oder widersprüchliche Statusangaben korrigieren. Keine zusätzliche parallele Statusdokumentation aufbauen.
* Implementiert, getestet, geprüft und abgenommen klar unterscheiden. Synthetische Tests, statische Analysen und reale Geräteprüfungen nicht gleichsetzen.
* Nur tatsächlich durchgeführte Arbeiten und nachgewiesene Ergebnisse als erledigt dokumentieren. Verbleibende Einschränkungen ausdrücklich erhalten.
* Diese sachliche Statuspflege gehört zum jeweiligen Auftrag und benötigt keine erneute Nutzerfreigabe.
* Daraus entsteht keine Erlaubnis, Produktanforderungen, Grenzwerte, Prioritäten oder Entwicklungsphasen eigenmächtig zu ändern oder offene Produktentscheidungen selbst zu treffen.

Ein Auftrag ist erst vollständig dokumentiert, wenn die betroffenen Steuerdokumente den belegten Abschlussstand widerspruchsfrei wiedergeben.

# 19. Definition of Done nach Aufgabentyp

Für alle Aufgabentypen gehört die sachliche Statuspflege gemäß Abschnitt 18.1 zum Abschluss. Implementierungs-, Test-, Review- und Abnahmestand sind dabei getrennt auszuweisen.

## 19.1 Produktcodeänderungen

Eine Produktcodeänderung ist erst abgeschlossen, wenn:

1. `MASTERPLAN.md` vollständig gelesen wurde.
2. Die relevante Masterplan-Regel genannt wurde.
3. Das Problem reproduziert wurde.
4. Der Scope eingehalten wurde.
5. Passende neue, aktuell legitimierte Tests existieren.
6. Alle betroffenen Tests grün sind.
7. Keine offene Produktentscheidung versteckt getroffen wurde.
8. Die gemeinsame Farbauswertung gemäß MASTERPLAN, Abschnitt 2, eingehalten wird.
9. Codex die Umsetzung, geänderten Dateien, ausgeführten Befehle, Resultate und nicht ausgeführten Tests dokumentiert hat.
10. Claude die Änderung gegen den Masterplan geprüft hat.
11. Der Nutzer bei erforderlichen Produktentscheidungen zugestimmt hat.

## 19.2 Reine Analyse-/Review-Aufträge

Ein reiner Analyse-/Review-Auftrag ist abgeschlossen, wenn:

1. `MASTERPLAN.md` vollständig gelesen wurde.
2. Der Befund mit Fundstellen dokumentiert ist.
3. Der Befund gegen den Masterplan eingeordnet ist.
4. Keine versteckte Produktentscheidung getroffen wurde.
5. Kein Code geändert wurde.

## 19.3 Reine Dokumentationsänderungen

Eine reine Dokumentationsänderung ist abgeschlossen, wenn:

1. Die Startregel eingehalten wurde.
2. Keine Produktfachlichkeit ohne ausdrückliche Nutzerfreigabe geändert wurde.
3. Die Dokumente mit `MASTERPLAN.md` konsistent sind.
4. Der freigegebene Scope eingehalten und die Änderung dokumentiert wurde.

Es sind keine Tests erforderlich, sofern kein ausführbares Verhalten betroffen ist. Reproduktion, Produkttests und Claude-Review aus Abschnitt 19.1 sind keine Abschlussvoraussetzung für reine Dokumentationsänderungen.

---

# 20. Grundsatz für beide Agenten

Ziel ist ein klarer, belegbarer und widerspruchsfreier Projektzustand innerhalb des freigegebenen Auftrags.

`MASTERPLAN.md` bestimmt Produktfachlichkeit und Entwicklungsrichtung. `AGENTS.md` regelt die operative Arbeitsweise. Der Nutzer entscheidet offene Produktfragen. Claude reviewt. Codex implementiert.
