# IRO – MASTERPLAN

**Status:** Verbindlich  
**Rolle:** Einzige Quelle für Produktfachlichkeit und Entwicklungsrichtung (*Single Source of Truth*)  
**Gültig für:** Codex (Coder) und Claude (Reviewer)  
**Gültig ab:** Reset des Iro-Projekts am 27.09.2026

---

# 0. Verbindlichkeit dieses Dokuments

Dieses Dokument ist die **einzige verbindliche Quelle für Produktfachlichkeit und Entwicklungsrichtung von Iro**, einschließlich Ziel, Produktprinzipien und fachlicher Testanforderungen.

`AGENTS.md` regelt ausschließlich die operative Arbeitsweise von Codex und Claude. Fachliche Regeln werden dort möglichst durch Verweise auf diesen Masterplan eingebunden, nicht parallel definiert.

Bei Widerspruch gilt `MASTERPLAN.md`. Offene Produktentscheidungen werden in `OPEN.md` dokumentiert und ausschließlich vom Nutzer entschieden.

Für Codex und Claude gilt zwingend:

1. Frühere Markdown-Dokumente, frühere Testverträge, alte Agentenantworten, alte Zwischenentscheidungen und Git-Historie sind **keine aktuelle Anforderungsquelle**.
2. Bestehender C#-Code ist **IST-Zustand**, nicht automatisch fachliche Wahrheit.
3. Bestehende Implementierungen dürfen nur dann als weiterhin gültig angesehen werden, wenn sie diesem Masterplan entsprechen.
4. Frühere Logik darf nicht allein deshalb erhalten bleiben, weil sie bereits implementiert ist.
5. Frühere Logik darf nicht allein deshalb wiederhergestellt werden, weil sie in Git auffindbar ist.
6. Kein Agent darf eine offene Produktfrage selbst entscheiden.
7. Bei Widerspruch zwischen Code und diesem Dokument gilt **dieses Dokument**.
8. Bei fachlicher Unsicherheit wird gestoppt und dem Nutzer die konkrete Produktentscheidungsfrage vorgelegt. Mehrere technische Implementierungswege für dasselbe bereits definierte Produktverhalten sind normale technische Entscheidungen und benötigen nicht automatisch eine Nutzerfreigabe.

**Historie darf erklären. Sie darf nicht befehlen.**

---

# 1. Ziel und Sinn von Iro

Iro soll einem normalen Anwender ermöglichen, mit einem Smartphone zu bestimmen, welches Farbfeld eines realen Farbstreifens zur Farbe einer realen Wand passt.

Der Benutzer legt bzw. hält den Farbstreifen an oder vor die Wand und erstellt **ein einziges Foto**, auf dem gleichzeitig zu sehen sind:

* die Wand,
* der Farbstreifen,
* die einzelnen Farbfelder.

Iro analysiert dieses **eine gemeinsame Bild** und vergleicht die Wandfarbe relativ mit den Farben der Farbfelder.

Das Produktziel ist **nicht**, die objektiv wahre Farbe der Wand unabhängig vom Foto zu rekonstruieren.

Das Produktziel ist:

> **Wand und Farbfelder innerhalb derselben Aufnahme so miteinander zu vergleichen, dass das für die Wand am besten passende Farbfeld zuverlässig bestimmt werden kann.**

Später kann zusätzlich entschieden werden, ob Iro auch explizit feststellen soll:

> **Keines der Felder passt ausreichend gut.**

Diese zweite Produktfrage ist derzeit noch offen und darf nicht eigenmächtig durch eine Schwelle beantwortet werden.

---

# 2. Das unverrückbare Grundprinzip

## 2.1 Wand und Farbstreifen sind eine Einheit

Wand und Farbstreifen sind für die Farbauswertung **eine unteilbare gemeinsame Aufnahme**.

Sie dürfen niemals fachlich so behandelt werden, als wären sie zwei voneinander unabhängige Farbmessungen.

Der Hauptgewinn der App besteht gerade darin, dass beide unter denselben Aufnahmebedingungen im selben Bild vorhanden sind.

## 2.2 Relative statt absolute Farbmessung

Iro soll nicht fragen:

> „Welche objektive Farbe hat die Wand?“

und anschließend unabhängig:

> „Welche objektive Farbe hat das Farbfeld?“

Iro soll fragen:

> **„Wie verhalten sich Wand und Farbfeld innerhalb derselben Aufnahme relativ zueinander?“**

Für bereits im aufgenommenen Bild vorhandene gemeinsame Aufnahmeeffekte sowie generatorseitig erzeugte Teststörungen folgt daraus:

* gemeinsame Belichtungsfehler sind nicht automatisch schädlich,
* gemeinsamer Weißabgleich ist nicht automatisch schädlich,
* gemeinsamer Farbstich ist nicht automatisch schädlich,
* gemeinsame Aufhellung oder Abdunklung ist nicht automatisch schädlich,
* gemeinsame leichte Entsättigung ist nicht automatisch schädlich,
* gemeinsame globale Transformationen sind nicht allein deshalb ein Qualitätsfehler.

Relevant wird eine Beeinträchtigung erst dann, wenn sie die **relative Beziehung zwischen Wand und Farbfeld** so verändert, dass die fachliche Antwort unzuverlässig wird.

## 2.3 Wand und Farbstreifen dürfen nicht auseinandergerissen werden

Ohne ausdrückliche neue Nutzerentscheidung sind insbesondere verboten:

* getrennte Belichtungskorrektur von Wand und Farbstreifen,
* getrennte Weißabgleiche,
* getrennte Sättigungs- oder Kontrastkorrekturen,
* getrennte Farbnormalisierung,
* getrennte Rekonstruktion vermeintlich „wahrer“ Farben,
* unterschiedliche Farbtransformationen für Wand und Farbstreifen,
* Logik, die Wand und Farbstreifen als unabhängige Farbmessquellen behandelt.

Für geometrische Verarbeitung gelten die Nachweispflichten aus Abschnitt 2.6. Sie ist keine Erlaubnis, Farbwerte für die Messung zu ersetzen.

## 2.4 Unveränderte Originalpixel als Messgrundlage

**Iro darf die Farbwerte des aufgenommenen Fotos nicht nachträglich verändern.** Für die Farbmessung werden ausschließlich unveränderte Originalpixel verwendet.

Verboten sind insbesondere:

* Aufhellen und Abdunkeln,
* Schärfen und Weichzeichnen des aufgenommenen Fotos,
* lokale oder globale Farbkorrekturen,
* getrennte Korrekturen von Wand und Farbstreifen,
* Erzeugen neuer Farbwerte, die anschließend anstelle der Originalpixel als Messgrundlage dienen.

Auch eine gemeinsame Farbverbesserung des gesamten Fotos ist verboten. Die Ausnahme für geometrische Hilfsdarstellungen ist ausschließlich in Abschnitt 2.6 geregelt.

## 2.5 Robuste statistische Messung

**Originalpixel dürfen nicht verändert werden. Aus Originalpixeln dürfen aber Messwerte statistisch abgeleitet werden.**

Eine erkannte Wand- oder Farbfeldfläche darf aus unterschiedlichen RGB-Pixeln bestehen. Aus ihren unveränderten Originalpixeln darf und soll ein robuster repräsentativer Farbwert bestimmt werden. Insbesondere die **Medianbildung ist ausdrücklich erlaubt** und keine unerlaubte Bildmanipulation.

Der abgeleitete Messwert ist ein statistisches Ergebnis; er ersetzt keine Bildpixel und wird nicht als nachbearbeitetes Bild erneut zur Messgrundlage.

## 2.6 Geometrische Hilfsverarbeitung und Nachweispflicht

Geometrische Hilfsdarstellungen dürfen abgeleitete Bilddaten verwenden, um Streifen zu lokalisieren, Orientierung zu bestimmen oder Konturen und Grenzen zu finden. Sie dürfen vorerst nur bestehen bleiben, wenn technisch belastbar nachgewiesen ist, dass:

1. ihre abgeleiteten Farbwerte keine Farbwerte für die Messung ersetzen,
2. die spätere Farbmessung ausschließlich unveränderte Originalpixel verwendet,
3. die Auswahl der Messflächen nicht unvertretbar verfälscht wird.

Es reicht nicht, allein den Zugriff auf Originalpixel in der späteren Farbmessung nachzuweisen. Zu prüfen sind auch systematisch falsche oder verschobene Messflächen und dadurch veränderte Pixelmengen, insbesondere im Vergleich zur Erkennung ohne Glättung.

Fließen geglättete Farbwerte direkt als Messgrundlage ein oder verfälschen sie indirekt über die Messflächenauswahl die Farbmessung unvertretbar, ist dieser Einfluss verboten und zu entfernen. Änderungen erfolgen ausschließlich im freigegebenen Auftrag; ein Prüfauftrag ist keine Implementierungsfreigabe.

**Geometrische Hilfsverarbeitung ist nur zulässig, wenn sie keine Farbwerte für die Messung ersetzt und die Auswahl der Messflächen nicht unvertretbar verfälscht.**

---

# 3. Zweites unverrückbares Prinzip: Ein Foto = eine Messung

Der reale Benutzer erstellt ein Foto.

Iro bekommt dieses Foto.

Dieses Foto muss für sich allein analysiert werden.

Daraus folgt:

* keine Mehrbildfusion,
* keine zeitliche Mittelung über mehrere Bilder,
* keine Kippserie als Voraussetzung für eine Farbaussage,
* keine Bestätigung eines Ergebnisses durch vorherige oder nachfolgende Frames,
* keine bildübergreifende Farbhistorie als Voraussetzung für die Antwort.

Ein Analyseergebnis darf nicht von einem vorherigen Foto abhängig sein.

Der technische Ablauf lautet:

> **ein Bild → eine Analyse → eine Auswertung → eine Antwort**

---

# 4. Was Iro fachlich beantworten soll

## 4.1 Bereits festes Ziel

Iro muss bestimmen können:

> **Welches erkannte Farbfeld liegt farblich am nächsten an der Wandreferenz?**

## 4.2 Noch offene Produktfrage

Noch nicht entschieden ist:

> **Soll Iro zusätzlich entscheiden, ob überhaupt ein Feld ausreichend gut passt?**

Bis zur ausdrücklichen Entscheidung des Nutzers gilt:

* keine ΔE-Match-Schwelle,
* kein implizites „passt“, nur weil ein Feld `IsNearest` ist,
* keine aus alten Tests rekonstruierte Match-Grenze,
* keine automatische No-Match-Entscheidung.

Codex und Claude dürfen diese Frage untersuchen und Messdaten vorlegen, aber nicht entscheiden.

## 4.3 Teilweise auswertbarer Farbstreifen – entschieden

`PartiallyMeasured` ist **kein automatischer Ablehnungszustand** und keine offene Produktfrage.

Wenn nur ein Teil des Farbstreifens zuverlässig erfasst und gemessen werden kann, arbeitet Iro mit den vorhandenen Informationen:

* erkannte und auswertbare Felder messen,
* vorhandene Messwerte anzeigen,
* unter den auswertbaren Feldern den nächstliegenden Kandidaten bestimmen,
* fehlende oder nicht messbare Felder nicht erraten,
* keine hypothetischen Aussagen über nicht sichtbare Felder treffen.

Die UI muss die eingeschränkte Aussagekraft transparent machen, zum Beispiel:

> Der Farbstreifen wurde wahrscheinlich nicht vollständig erfasst. Die angezeigten Ergebnisse beziehen sich nur auf die erkannten und auswertbaren Felder.

Die Freigabe einer Teilauswertung setzt weiterhin voraus, dass die verwendeten Messdaten zuverlässig sind. Die Qualitätsregeln aus Abschnitt 12 gelten weiter. Die Entscheidung zu `PartiallyMeasured` trifft keine Match-/No-Match-Entscheidung gemäß Abschnitt 4.2.

---

# 5. Aktueller IST-Zustand

Der vorhandene C#-Code bleibt zunächst bestehen und wird als technischer IST-Bestand behandelt.

Bekannt bzw. bereits weitgehend vorhanden sind:

* Core-Analyse,
* Farbkonvertierung und CIEDE2000,
* robuste Flächenmessung,
* Streifenerkennung,
* Felderkennung,
* beide Streifenrichtungen,
* Geraderichten bzw. geometrische Hilfslogik,
* Analyzer-/Presentation-Struktur,
* IroGen als Generator- und Testhilfswerkzeug,
* Android-/MAUI-Prototyp,
* Analyse-/Runner-Hilfsstruktur.

Der aktuelle bereinigte Arbeitsbestand besteht im Wesentlichen aus:

* `iro-gen`
* `src/iro.analysis`
* `src/iro.app`
* `src/iro.core`
* `tools/iro.testgen`

Es existiert derzeit bewusst **keine alte aktive Testsuite** und keine alte Dokumentationslandschaft.

---

# 6. Bekannte Altlasten im Code

Der vorhandene Code kann weiterhin Logik enthalten, die aus früheren, inzwischen verworfenen oder widersprüchlichen Produktannahmen entstanden ist.

Besonders zu prüfen sind:

## 6.1 Kanalendpunkt-/Clipping-Logik

Verdacht:

Absolute Kanalwerte `0` oder `255` können pauschal als Qualitätsfehler behandelt werden.

Prüffrage:

> Ist durch diesen Zustand tatsächlich relative Farbinformation verloren gegangen oder handelt es sich lediglich um eine legitime reale Farbe?

Jede Sperre braucht einen nachgewiesenen Zusammenhang mit einem unvertretbaren Fehlrisiko im relativen Vergleich. Die vorhandene Kanalendpunktregel bleibt ein Prüfpunkt in Phase 1 und wird bis zur gesonderten Entscheidung bzw. Validierung nicht eigenmächtig geändert; für ihren vorläufigen Bestand gilt Abschnitt 12.

## 6.2 Reflex-/Reflexschleier-Logik

Eine gleichmäßige globale Veränderung von Wand und Farbstreifen ist nicht automatisch ein Fehler.

Zu prüfen sind ausschließlich Fälle, in denen:

* Wand und Streifen unterschiedlich beeinflusst werden,
* Materialreflexionen unterschiedlich wirken,
* die relative Farbbeziehung tatsächlich verfälscht wird.

Keine pauschale Reflexschleier-Sperre ohne Nachweis.

## 6.3 Helligkeits-/Dunkelheitsregeln

Absolute Dunkelheit oder Helligkeit allein rechtfertigt keine Ablehnung.

Entscheidend ist, ob die relative Farbmessung unzuverlässig wird.

## 6.4 Alte Mehrbild-/Zeitlogik

Jegliche verbliebene Logik, die mehrere Bilder zusammenführt oder zeitliche Bestätigung voraussetzt, widerspricht dem Einzelbildprinzip und ist zu entfernen, sofern sie tatsächlich noch wirksam ist.

## 6.5 Alte Test- und Erwartungsinfrastruktur im Produktbestand

Besonders kritisch zu prüfen sind Klassen und Konzepte, deren Namen auf frühere Prüfverträge hindeuten, unter anderem:

* `iro-gen/IroTestHandoff.cs`
* `iro-gen/TestPlan.cs`
* `iro-gen/TestResultsWindow.xaml.cs`
* `iro-gen/TestReviewExport.cs`
* `iro-gen/TestRunReview.cs`
* `src/iro.analysis/BehaviorExpectation.cs`
* `src/iro.analysis/RankingExpectation.cs`
* `src/iro.analysis/AnalysisRunContracts.cs`

Diese Dateien werden **nicht blind gelöscht**.

Claude prüft zuerst, ob sie:

1. fachlich neutral und weiterhin sinnvoll,
2. nur Hilfsinfrastruktur,
3. teilweise Altlast,
4. vollständig durch den alten Testvertrag geprägt

sind.

Codex ändert oder entfernt sie erst nach einem klaren Auftrag.

## 6.6 Technische Inkonsistenzen nach der Dokumentenbereinigung

In Phase 1 sind folgende technische Altlasten nach klarer Aufgabenformulierung zu bereinigen:

1. `iro-gen/IroTestHandoff.cs` darf zur Erkennung des Projektstamms nicht mehr von der entfernten Datei `IRO-KONSOLIDIERTER-PLAN.md` abhängen.
2. Verweise auf nicht mehr vorhandene Testprojekte sind aus `iro.slnx` zu entfernen. Historische Testprojekte werden dafür nicht rekonstruiert.

Dies sind technische Bereinigungen, keine offenen Produktentscheidungen. Codex darf sie im entsprechend formulierten Auftrag umsetzen. Der aktuelle Auftrag zur Dokumentationsanpassung und StripDetector-Prüfung umfasst diese Umsetzung noch nicht.

---

# 7. Rollen

# 7.1 Claude = Reviewer

Claude ist die fachlich-technische Kontrollinstanz.

Claude darf:

* Code lesen und analysieren,
* Architektur und Logik gegen diesen Masterplan prüfen,
* Widersprüche identifizieren,
* Altlasten benennen,
* Risiken erklären,
* Testlücken aufzeigen,
* konkrete Arbeitspakete für Codex formulieren,
* Änderungen von Codex überprüfen,
* Testauswertungen bewerten,
* offene Produktentscheidungen klar an den Nutzer eskalieren.

Claude darf nicht:

* Produktcode selbst verändern,
* Produktentscheidungen treffen,
* neue Schwellenwerte erfinden,
* neue Qualitätsregeln eigenmächtig festlegen,
* alte Git-Historie als verbindliche Spezifikation verwenden,
* Codex anweisen, alte Tests oder alte Regeln wiederherzustellen, nur weil sie früher existierten.

Claude muss vor jeder Empfehlung prüfen:

> **Welcher konkrete Schaden am relativen Wand-/Farbfeldvergleich wird damit verhindert?**

Wenn diese Frage nicht klar beantwortet werden kann, ist keine neue Produktlogik zu empfehlen.

# 7.2 Codex = Coder

Codex ist die Implementierungsinstanz.

Codex darf:

* freigegebene Arbeitspakete implementieren,
* Code refaktorieren, soweit das fachliche Verhalten nicht eigenmächtig verändert wird,
* neue Tests für gültige Anforderungen schreiben,
* alte Code-Altlasten nach Freigabe entfernen,
* Build- und Testläufe ausführen,
* technische Befunde dokumentieren.

Codex darf nicht:

* Produktanforderungen erfinden,
* offene Fragen selbst schließen,
* Schwellenwerte selbst wählen,
* neue Sperrlogik „vorsichtshalber“ ergänzen,
* alte Tests rekonstruieren, weil sie früher existierten,
* Git-Historie nach alten Produktregeln durchsuchen, sofern dies nicht ausdrücklich beauftragt wurde,
* Code nur deshalb ändern, weil ein früherer Mechanismus bekannt ist,
* Wand und Farbstreifen getrennt farblich behandeln.

Codex muss vor jeder Produktcodeänderung angeben:

1. welche Masterplan-Regel betroffen ist,
2. welches konkrete Problem behoben wird,
3. wie das Problem reproduziert wurde,
4. warum die Änderung den relativen Vergleich verbessert oder schützt,
5. welche Dateien geändert werden sollen.

---

# 8. Zusammenarbeit zwischen Claude und Codex

Die operative Ausführung und die Abschlusskriterien nach Aufgabentyp regelt `AGENTS.md`. Für Produktcodeänderungen gilt folgender Arbeitszyklus:

1. **Masterplan lesen**
2. Claude analysiert einen klar abgegrenzten Punkt.
3. Claude beschreibt:

   * IST-Zustand,
   * Problem,
   * Relevanz zum Grundprinzip,
   * konkrete Änderung,
   * benötigte Tests.
4. Nutzer entscheidet, sofern eine Produktentscheidung betroffen ist.
5. Codex implementiert ausschließlich den freigegebenen Auftrag.
6. Codex führt die dazugehörigen Tests aus.
7. Codex dokumentiert:

   * geänderte Dateien,
   * ausgeführte Befehle,
   * Testergebnisse,
   * bewusst nicht ausgeführte Tests,
   * verbleibende Unsicherheiten.
8. Claude überprüft Änderung und Ergebnis erneut gegen diesen Masterplan.
9. Erst danach folgt der nächste Punkt.

Keine parallele fachliche Ausweitung ohne Auftrag.

---

# 9. Teststrategie nach dem Reset

Alle Tests vor dem Reset gelten als historisch und sind nicht automatisch wiederherzustellen.

Neue Tests entstehen ausschließlich aus:

1. diesem Masterplan,
2. einer ausdrücklichen neuen Nutzerentscheidung,
3. einem nach dem Reset reproduzierten echten Fehler.

## 9.1 Reihenfolge

Für jede relevante Produktcodeänderung gilt:

> Produktregel → reproduzierbarer Test → Implementierung → Regressionstest

Nicht:

> alter Test → Code verbiegen → Begründung nachträglich suchen

## 9.2 Testklassen

Neue Tests werden gedanklich in vier Gruppen unterschieden:

### Contract Tests

Beweisen aktuell gültige Produktregeln.

### Regression Tests

Sichern konkret behobene Fehler ab.

### Diagnostic Tests

Messen und untersuchen, definieren aber keine Produktregel.

### Acceptance Tests

Nutzen einen getrennten Abnahmesatz und prüfen das reale Produktverhalten.

## 9.3 Grundlegende neue Contract Tests

Nach Bereinigung des Codes sollen zuerst wenige, fundamentale Tests entstehen.

Mindestens:

1. **Gemeinsame Bildbasis**  
Wand und Farbfelder werden aus derselben Aufnahmebasis ausgewertet.
2. **Unveränderte Originalpixel**  
Die Farbmessung verwendet ausschließlich unveränderte Originalpixel; lokale, globale oder getrennte Farbverbesserung ist verboten. Statistisch abgeleitete Messwerte, insbesondere der Median, sind gemäß Abschnitt 2.5 erlaubt. Für geometrische Hilfsdarstellungen gelten zusätzlich die Nachweispflichten aus Abschnitt 2.6.
3. **Identische Ausgangsfarben bleiben identisch**  
Identische Ausgangsfarben von Wand und Farbfeld bleiben bei derselben deterministischen Transformation identisch. Daraus folgt kein allgemeiner Vertrag zur Rangfolge verschiedener, nicht identischer Farben.
4. **Gemeinsame Transformation ist für sich kein Fehler**  
Eine globale gemeinsame Transformation ist nicht allein deshalb ein Fehler. Es gelten die Anforderungen aus Abschnitt 2 und 12.
5. **Einzelbildprinzip**  
Das Ergebnis eines Bildes ist unabhängig von vorherigen Bildern.
6. **CIEDE2000-Referenzwerte**  
Die mathematische Farbabstandsberechnung wird mit unabhängigen Referenzwerten geprüft.
7. **Beide Streifenrichtungen**  
Horizontal und vertikal müssen unterstützt werden.

Weitere Tests entstehen erst aus konkreten Anforderungen oder reproduzierten Fehlern.

Die gemeinsamen Transformationen in den Punkten 3 und 4 beziehen sich auf Testbild-Erzeugung bzw. bereits vorhandene Aufnahmeeffekte. Sie erlauben keine nachträgliche Farbkorrektur durch Iro.

## 9.4 Diagnostische Untersuchung der Rangfolge

Ob eine konkrete gemeinsame Transformation bei der Testbild-Erzeugung die Rangfolge verschiedener, nicht identischer Farben ausreichend erhält, muss für diese Transformation untersucht werden. Diese Untersuchung erlaubt keine nachträgliche Farbverbesserung im Analyzer. Eine allgemeine Rangfolgestabilität ist **kein mathematischer Grundvertrag**.

Solche Rangfolgetests sind zunächst **Diagnostic Tests**. Sie liefern Messdaten und begründen für sich keine Produktanforderung. Erst wenn der Nutzer auf Grundlage der Messergebnisse eine Produktanforderung ausdrücklich bestätigt, dürfen daraus entsprechende Contract Tests entstehen.

---

# 10. Generatorregeln

IroGen ist ein Werkzeug zur reproduzierbaren Erzeugung und Untersuchung definierter Szenarien.

Generatorseitig erzeugte Störungen für Testzwecke sind vom Verbot nachträglicher Farbverbesserung durch Iro nicht betroffen: Sie sind bereits vor Beginn der Analyzer-Auswertung Bestandteil des synthetischen Eingabebildes. Ab dieser Eingabe gelten die Regeln zu unveränderten Originalpixeln aus Abschnitt 2.

Ground Truth darf und soll vom Generator/Testsystem erzeugt, gespeichert und für die spätere Ergebnisprüfung verwendet werden.

Verboten ist ausschließlich, dass Generatorwissen in die Analyzerentscheidung gelangt.

Insbesondere dürfen Generatorparameter wie:

* Störungsart,
* Störungsstärke, einschließlich Schleierstärke,
* Sollfeld,
* Ground Truth,
* interne Szenenmetadaten

dem Analyzer nicht übergeben und von ihm nicht für seine Entscheidung verwendet werden.

Der Analyzer erhält nur das Bild und die im realen Produkt ebenfalls verfügbaren Informationen.

Generator und Analyzer sind logisch strikt getrennt.

---

# 11. Reale versus synthetische Tests

Synthetische Generatorbilder sind wichtig für:

* reproduzierbare Regressionen,
* kontrollierte Variation einzelner Störungen,
* Grenzuntersuchungen,
* Diagnose.

Sie sind kein vollständiger Ersatz für reale Smartphone-Aufnahmen.

Reale Tests müssen später insbesondere abdecken:

* verschiedene Wandfarben,
* helle und dunkle Farben,
* gering gesättigte Farben,
* unterschiedliche Materialien,
* reale Beleuchtung,
* reale Reflexion,
* Bewegung,
* Defokus,
* unterschiedliche Abstände,
* horizontale und vertikale Streifen,
* verschiedene Smartphones bzw. Kamerapipelines.

Neue Schutzlogik soll bevorzugt aus **nachgewiesenen realen oder realistisch reproduzierbaren Fehlerfällen** entstehen, nicht aus hypothetischen Befürchtungen.

---

# 12. Qualitätsregeln und Sperren

Die vorhandenen Qualitätsablehnungen können vorläufig grundsätzlich bestehen bleiben. Dies ist keine Bestätigung ihrer Produktreife und kein Auftrag zur Änderung ihrer Grenzen.

Es gilt:

* keine Sperre allein aufgrund theoretischer Vorsicht,
* jede Sperre braucht einen nachgewiesenen Zusammenhang mit einem unvertretbaren Fehlrisiko,
* experimentelle Grenzwerte bleiben experimentell,
* vorhandene Ablehnungen müssen gegen reale Daten validiert werden,
* Produktreife ist erst nach realen Gerätetests gegeben.

Die Kanalendpunktregel bleibt ausdrücklich ein Prüfpunkt in Phase 1. Bis zur gesonderten Entscheidung bzw. Validierung dürfen Match-/No-Match-Logik, ΔE-Grenzwerte, Qualitätsgrenzen, Kanalendpunkt-Schwellen und weitere Bildqualitätsheuristiken nicht eigenmächtig verändert werden.

Eine neue Sperre oder Warnung darf nur eingebaut werden, wenn alle folgenden Bedingungen erfüllt sind:

1. Es gibt einen reproduzierbaren Fehlerfall.
2. Der Fehler führt zu einer fachlich falschen oder unvertretbar unsicheren Antwort.
3. Das relevante Merkmal ist aus dem einzelnen Bild erkennbar.
4. Das Merkmal verwechselt legitime reale Szenen nicht in unvertretbarem Umfang mit Fehlerfällen.
5. Der Nutzen der Sperre ist durch Tests belegt.
6. Der Nutzer hat die Produktregel freigegeben.

Keine Sperre nur aufgrund von:

* absoluter Helligkeit,
* absolutem RGB-Endpunkt,
* „blassem“ Erscheinungsbild,
* ungewöhnlichem Weißabgleich,
* globalem Farbstich,
* globaler gemeinsamer Transformation,

solange nicht gezeigt wurde, dass dadurch der relative Vergleich tatsächlich unzuverlässig wird.

---

# 13. Keine nachträgliche Farbverbesserung

Iro darf das aufgenommene Foto weder lokal noch global zur Verbesserung seiner Farbe nachbearbeiten. Ein behaupteter oder nachgewiesener Nutzen erlaubt keine Ausnahme von Abschnitt 2.4.

Robuste statistische Auswertung unveränderter Originalpixel, insbesondere Medianbildung, ist gemäß Abschnitt 2.5 ausdrücklich erlaubt. Für geometrische Hilfsdarstellungen gilt ausschließlich Abschnitt 2.6 mit seiner technischen Nachweispflicht.

Generatorseitige Teststörungen gemäß Abschnitt 10 sind davon nicht betroffen.

---

# 14. Entwicklungsphasen ab jetzt

**Aktuelle Phase: PHASE 1 – Code-Altlasten bereinigen.**

Dieser Status gilt, bis der Nutzer einen Phasenwechsel ausdrücklich freigibt. Die Phasenangabe ist kein eigenständiger Auftrag, mit einer Codeänderung zu beginnen.

# PHASE 1 – Code-Altlasten bereinigen

**Ziel:** Der vorhandene Code enthält nur noch Logik, die mit diesem Masterplan vereinbar ist.

Vorgehen:

1. Claude prüft den bestehenden C#-Bestand blockweise.
2. Für jede verdächtige Logik:

   * Fundstelle,
   * aktuelle Wirkung,
   * frühere mutmaßliche Annahme,
   * Konflikt oder Übereinstimmung mit diesem Masterplan,
   * Empfehlung.
3. Noch keine Änderung bei offener Produktfrage.
4. Codex entfernt oder korrigiert nur freigegebene Altlasten.
5. Für jede Entfernung/Korrektur entstehen neue, aktuelle Tests.

**Priorität der Prüfung:**

1. Kanalendpunkt-/Clipping-Logik
2. Reflex-/Reflexschleier-Sperren
3. Dunkel-/Hell-Sperren
4. Mehrbild-/Zeitlogik
5. alte Erwartungs-/Testinfrastruktur in `iro.analysis` und `iro-gen`
6. Beschnitt-/Sichtbarkeitslogik
7. sonstige Qualitätsheuristiken

Zusätzlich ist die Glättung im `StripDetector` gezielt gegen Abschnitt 2.6 zu prüfen: direkter Messwerteinfluss, Einfluss auf Messflächen und Originalpixelauswahl sowie systematische Verfälschungen. Nach dem aktuellen Prüfauftrag wird der Befund mit Fundstellen vorgelegt; erst danach entscheidet der Nutzer über Änderungen dort.

Die technischen Bereinigungen aus Abschnitt 6.6 gehören ebenfalls zu Phase 1 und werden separat als klare Aufgaben umgesetzt.

# PHASE 2 – Kleine neue Baseline-Testsuite

Nach der Codebereinigung werden nur die fundamentalen, aktuell legitimierten Tests aufgebaut.

Ziel:

* klein,
* verständlich,
* direkt auf Masterplan-Regeln zurückführbar,
* keine historische Testmasse.

Erst wenn diese Baseline grün ist, wird weitergebaut.

# PHASE 3 – End-to-End-Testmodus

Der vollständige App-Ablauf muss ohne echte Kamera testbar sein:

1. Testbild laden,
2. Wandreferenz sichtbar und nachvollziehbar,
3. Streifen erkennen,
4. Felder erkennen,
5. Farben messen,
6. nächsten Kandidaten bestimmen,
7. Ergebnis sichtbar im Bild markieren,
8. Fehler-/Unsicherheitszustände anzeigen,
9. keine veralteten Ergebnisse,
10. streng kontrollierter Analyseablauf.

# PHASE 4 – Kamera

Erst nach stabilem End-to-End-Testmodus:

* Android-Kamera anbinden,
* notwendige Kameraeinstellungen bereitstellen,
* ein Foto pro Analyse,
* keine versteckte Mehrbildlogik,
* keine nachträgliche lokale, globale oder getrennte Farbverbesserung; es gelten Abschnitt 2.4–2.6.

# PHASE 5 – Reale Gerätetests

Danach reale Testserien.

Ergebnisse werden gegen die Produktfrage bewertet:

> Gibt Iro dem Benutzer die richtige Antwort?

Nicht:

> Sieht das Bild technisch perfekt aus?

# PHASE 6 – Match/No-Match-Entscheidung

Wenn genügend reale und synthetische Messdaten vorliegen, wird die offene Frage entschieden:

> Reicht „nächstes Feld“, oder soll Iro zusätzlich „kein Feld passt“ erkennen?

Bis dahin keine implizite Match-Schwelle.

---

# 15. Dokumentationsregeln und Markdown-Lebenszyklus

Im aktiven Projekt bleiben dauerhaft nur:

* `MASTERPLAN.md` – einzige Quelle für Produktfachlichkeit und Entwicklungsrichtung,
* `AGENTS.md` – operative Arbeitsweise von Codex und Claude,
* `OPEN.md` – offene Produktentscheidungen,
* optional ein kurzes `README.md` als technische Projektübersicht.

Weitere Markdown-Dateien dürfen für größere Teilaufgaben **temporär** entstehen. Sie sind als temporäre Arbeitsunterlagen oder Berichte zu kennzeichnen und dürfen keine parallele Produktspezifikation bilden.

**Nach Abschluss, erforderlichen Tests und Abnahme der jeweiligen Teilaufgabe müssen ihre temporären Markdown-Dateien wieder gelöscht werden.** Welche Tests erforderlich sind, richtet sich nach dem Aufgabentyp gemäß AGENTS, Abschnitt 19; für reine Dokumentationsänderungen ohne ausführbares Verhalten sind keine Tests erforderlich.

Diese Lebenszyklusregel gilt auch für temporäre Markdown-Testberichte. Eine dauerhafte zusätzliche Spezifikations- oder Berichtswelt ist nicht zulässig. Git bleibt die Historie; es wird kein dauerhaftes Markdown-Archiv neben den Steuerdokumenten aufgebaut.

Verbindliche, ausdrücklich freigegebene Produktentscheidungen gehören in diesen Masterplan; noch offene Produktentscheidungen in `OPEN.md`. Berichte und Logs sind Berichte, keine Produktverträge.

Dieses Dokument darf nur auf ausdrücklichen Auftrag des Nutzers geändert werden.

---

# 16. OPEN.md

Offene Produktentscheidungen werden nicht in verstreuten Berichten geführt, sondern zentral in `OPEN.md`.

Jeder Eintrag enthält:

* ID,
* Frage,
* warum offen,
* bekannte Optionen,
* Messdaten/Befunde,
* ausdrücklich verbotene Vorentscheidung.

Ein Agent darf aus einem offenen Punkt keinen eigenen Produktvertrag machen.

---

# 17. Umgang mit Git-Historie

Git bleibt die technische Historie.

Aber:

> Git-Historie ist kein Anforderungenarchiv.

Agenten dürfen alte Stände nur untersuchen, wenn der Nutzer oder der aktuelle Auftrag dies ausdrücklich verlangt.

Historische Entscheidungen dürfen nicht automatisch wieder aktiviert werden.

---

# 18. Definition of Done nach Aufgabentyp

Die operativen Abschlusskriterien stehen zentral in `AGENTS.md`, Abschnitt 19:

* **Produktcodeänderungen:** vollständige Definition of Done einschließlich Reproduktion, Tests und Claude-Review.
* **Reine Analyse-/Review-Aufträge:** gelesener Masterplan, Befund mit Fundstellen, Einordnung gegen den Masterplan, keine versteckte Produktentscheidung und keine Codeänderung.
* **Reine Dokumentationsänderungen:** keine Änderung der Produktfachlichkeit ohne Nutzerfreigabe und Konsistenz mit diesem Masterplan; keine Tests erforderlich, sofern kein ausführbares Verhalten betroffen ist.

---

# 19. Stop-Regeln

Claude oder Codex müssen die Arbeit stoppen und den Nutzer fragen, wenn:

* mehrere fachlich unterschiedliche Produktverhalten möglich sind,
* ein Grenzwert benötigt wird, der nicht im Masterplan festgelegt ist,
* eine neue Sperre oder Warnung notwendig erscheint,
* eine Änderung Wand und Farbstreifen unterschiedlich behandeln würde,
* ein alter Codepfad nicht eindeutig als Altlast oder gültige Funktion klassifizierbar ist,
* eine Änderung den grundlegenden Produktansatz verändern würde,
* „nearest“ und „match“ vermischt werden,
* reale Anforderungen aus altem Git rekonstruiert werden müssten.

Mehrere technische Implementierungswege für dasselbe bereits definierte Produktverhalten sind normale technische Entscheidungen und benötigen nicht automatisch eine Nutzerfreigabe.

---

# 20. Oberste Prüffrage

Vor **jeder** neuen fachlichen Änderung, jedem Test und jeder Auswertung ist diese Frage zu beantworten:

> **Welchen nachgewiesenen Schaden verhindert diese Maßnahme am relativen Vergleich von Wand und Farbfeld innerhalb desselben Fotos?**

Wenn darauf keine konkrete, überprüfbare Antwort existiert:

> **nicht implementieren.**

---

# 21. Ziel des Resets

Der Reset soll Iro nicht zurück auf Null setzen.

Er soll den brauchbaren technischen Kern retten und gleichzeitig:

* widersprüchliche Altverträge entfernen,
* alte Tests entmachten,
* übervorsichtige Schutzlogik prüfen,
* den relativen Vergleich wieder zum Mittelpunkt machen,
* den Produktcode verständlich machen,
* eine kleine, belastbare neue Testsuite aufbauen,
* den tatsächlichen App-Ablauf fertigstellen,
* reale Probleme priorisieren,
* künstliche Endlosschleifen aus Agentenentscheidungen verhindern.

Der technische Bestand ist Material.

**Dieser Masterplan ist die Richtung.**
