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

## 2.7 Abschluss und Unveränderlichkeit einer Flächenmessung

Für jede Messfläche muss vor der eigentlichen Farbmessung eindeutig festgelegt sein:

- welche Originalpixel zur Messfläche gehören,
- welche Konturmaske bzw. Stichprobenauswahl gilt,
- welche Messmethode verwendet wird.

Aus dieser festgelegten Originalpixelmenge und dieser festgelegten Messmethode entsteht pro Messvorgang genau ein abschließender repräsentativer Farbwert.

Interne Rechenschritte, die zur festgelegten Messmethode gehören, sind zulässig. Dazu können beispielsweise Histogramme, Medianberechnung, MAD-Berechnung oder andere vorher definierte statistische Zwischenschritte gehören.

Sobald der abschließende repräsentative Farbwert erzeugt wurde, ist der Messvorgang für diese Fläche beendet. Dieser Wert ist unveränderlich.

Nach diesem Point of no Return ist insbesondere verboten:

- die zugrunde liegende Pixelmenge stillschweigend zu verändern,
- weitere Pixel hinzuzunehmen oder auszuschließen,
- den Farbwert erneut zu filtern,
- ihn nachträglich zu korrigieren oder zu normalisieren,
- ihn als Eingabe für eine neue Farbmessung zu verwenden,
- aus einem bereits abgeleiteten Farbwert einen vermeintlich besseren neuen Messwert zu erzeugen.

Zulässig bleiben fest definierte mathematische Ableitungen zur fachlichen Auswertung, insbesondere Farbraumumrechnungen, CIEDE2000-Berechnung, Rangfolge und Darstellungsformatierung.

Solche Ableitungen dürfen den ursprünglichen Messwert nicht überschreiben und nicht rückwirkend die Messfläche oder die Messmethode verändern.

Wird später festgestellt, dass eine Messung ungültig ist, wird dieses Messergebnis vollständig aus der fachlichen Ergebnisverwendung ausgeschlossen. Daraus bereits berechnete Farbabstände, Rangfolgen und Ergebniskennzeichnungen werden ebenfalls ungültig.

Fehlergrund, Geometrie und diagnostische Metadaten dürfen erhalten bleiben, begründen aber keine Farbaussage.

Betrifft die Ungültigkeit die Wandreferenz, werden alle davon abhängigen Vergleiche ungültig.

Betrifft sie nur ein einzelnes Farbfeld, dürfen andere gültige Felder gemäß der `PartiallyMeasured`-Regel weiter ausgewertet werden.

Eine neue Messung darf nur als neuer Messvorgang vom unveränderten Originalbild aus beginnen. Das gilt auch für eine bestätigte neue Bereichsauswahl gemäß Abschnitt 4.4: Messflächen und Referenzen werden innerhalb dieser Auswahl neu bestimmt; frühere Messwerte werden weder übernommen noch nachträglich angepasst.

Diese Regel definiert ausschließlich den Lebenszyklus eines Messwertes. Sie legt nicht fest, welche konkrete Medianform oder Ausreißerselektion verwendet wird.

---

# 3. Zweites unverrückbares Prinzip: Ein Originalfoto pro unabhängigem Analysevorgang

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

Für jeden unabhängigen Analysevorgang gilt:

> **ein Originalfoto → eine Analyse → eine Auswertung → eine Antwort**

Zuerst wird das übermittelte Foto automatisch untersucht. Eine bestätigte neue Bereichsauswahl gemäß Abschnitt 4.4 startet einen neuen, unabhängigen Analysevorgang aus demselben unveränderten Originalfoto. Das ist keine Mehrbildauswertung und keine zeitliche Bestätigung. Vorherige Messwerte werden nicht übernommen oder vermischt; veraltete Ergebnisse dürfen nicht als Ergebnis der neuen Auswahl erscheinen.

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

Ein nicht erkanntes oder unsicheres Feld darf für sich allein keine zuverlässigen Vergleiche anderer Felder verhindern. Sicher erkannte und zuverlässig messbare Felder werden im Bild gekennzeichnet und mit ihrem ΔE zur Wand angezeigt. Unsichere Kandidaten erhalten keinen ΔE-Wert und nehmen nicht an der Auswahl des nächstliegenden Feldes teil; eindeutig lokalisierbare Positionen werden als unsicher markiert. Nicht erkannte Felder und eine unbekannte Gesamtzahl werden nicht erfunden. Der Hinweis begrenzt die Aussage auf die markierten, zuverlässig ausgewerteten Felder. Eine ungültige Wandreferenz entwertet weiterhin alle abhängigen Vergleiche.

Die Zulassung eines angeschnittenen Randfeldes und die Auswertbarkeit der vollständigen Felder sind getrennt zu beurteilen. Randregionen bleiben auch ungemessen Ausschlussflächen für die Wandreferenz. Die Referenz wird mit der vorhandenen Auswahl- und Qualitätslogik vor der abschließenden Feldmessung festgelegt. Geometrische Unsicherheiten innerhalb des relevanten Bereichs dürfen die tatsächlich abhängigen Vergleiche verhindern. Eine bloße Auswahlfrage zwischen konkurrierenden Bildbereichen ist davon zu unterscheiden und wird gemäß Abschnitt 4.4 behandelt; sie begründet für sich keine Unbrauchbarkeit der Aufnahme.

Die Freigabe einer Teilauswertung setzt weiterhin voraus, dass die verwendeten Messdaten zuverlässig sind. Die Qualitätsregeln aus Abschnitt 12 gelten weiter. Die Entscheidung zu `PartiallyMeasured` trifft keine Match-/No-Match-Entscheidung gemäß Abschnitt 4.2.

## 4.4 Automatische Auswertung, Bereichsauswahl und Neuaufnahme – entschieden

**Automatische Auswertung ist der Normalfall.** Iro untersucht zuerst das übermittelte Foto automatisch als Ganzes. Sind zuverlässige Vergleiche möglich, werden sie gemäß Abschnitt 4.3 im Bild markiert und mit ΔE angezeigt, auch als zulässige Teilauswertung. Unsichere oder nicht erkannte Felder verhindern für sich allein keine unabhängigen zuverlässigen Vergleiche. Unsichere Kandidaten erhalten keinen Farbvergleich und nehmen nicht an der Bestimmung des nächstliegenden Feldes teil.

**Eine Bereichsauswahl wird ausschließlich bei unklarer Zuordnung angeboten:** Die Aufnahme ist grundsätzlich auswertbar, aber Iro kann nicht eindeutig bestimmen, welcher Bildbereich relevant ist. Ein zusätzliches Bild an der Wand kann beispielsweise konkurrierende Strukturen erzeugen. Nur für diese Auswahlfrage bietet Iro einen verschiebbaren und in der Größe veränderbaren Rahmen an. Der Hinweis lautet sinngemäß:

> Bitte markieren Sie den gewünschten Farbstreifen und ausreichend Wandfläche daneben.

Der Rahmen erscheint weder grundsätzlich vor jeder Analyse noch bei jeder Unsicherheit. Er bedeutet ausschließlich „Diesen Bereich meine ich“. Er ist keine manuelle Vorgabe von Feldgrenzen und keine Qualitätsfreigabe.

**Mehrdeutigkeit hat zwei unterschiedliche Bedeutungen:** Eine Auswahlfrage zwischen grundsätzlich untersuchbaren Bereichen kann der Anwender durch den Rahmen klären. Eine weiterhin unsichere Geometrie innerhalb des gewählten Bereichs wird dadurch nicht sicher. Iro darf keine Feldgrenzen erraten; verlässlich auswertbare, unabhängige Teilfelder bleiben gemäß Abschnitt 4.3 nutzbar. Weder jede Mehrdeutigkeit noch jeder fehlende Analysewert berechtigt automatisch zum Rahmenangebot oder zur Behauptung einer unbrauchbaren Aufnahme.

**Eine unbrauchbare Aufnahme erfordert eine Neuaufnahme.** Sind wegen Aufnahmequalität oder ungeeigneter Aufnahmebedingungen tatsächlich keine zuverlässigen Vergleiche möglich, erklärt Iro die Einschränkung verständlich und bittet um ein neues Foto. Der Rahmen wird nicht als Ausweg angeboten. Starke Verwacklung oder ungeeigneter Aufnahmeabstand sind Beispiele, keine neuen pauschalen Sperren. Hinweise wie „Smartphone ruhig halten“ oder „Abstand vergrößern“ benötigen einen entsprechend belegten Befund; aus einem unspezifischen Erkennungsfehler darf keine konkrete Ursache behauptet werden. Ein lokales Problem macht nicht automatisch die gesamte Aufnahme unbrauchbar.

**Die Auswahl ersetzt keine Qualitätsprüfung.** Nach Bestätigung analysiert Iro den gewählten Bereich mit denselben fachlichen Qualitätsanforderungen. Bleibt er unbrauchbar, wird keine Farbaussage erzwungen. Wand und Streifen stammen aus demselben Originalfoto. Der Rahmen beschränkt die untersuchten Originalpixel, verändert aber keine Farbwerte. Skalierte Anzeigen oder interpolierte Vorschaubilder dürfen nicht zur Messgrundlage werden.

Eine bestätigte neue Auswahl startet gemäß Abschnitt 3 einen neuen unabhängigen Analysevorgang vom Originalfoto aus. Alte Messwerte werden nicht übernommen oder vermischt; alte Ergebnisse und Bildmarkierungen dürfen nicht als Ergebnis der neuen Auswahl erscheinen.

Diese Bedienentscheidung ist verbindlich. Fehlende technische Nachweise ihrer zuverlässigen Umsetzung sind Umsetzungslücken gemäß Abschnitt 5.1, keine erneut offene Grundsatzentscheidung. Es werden hierfür weder eine Konfidenzzahl noch ein neuer Grenzwert festgelegt.

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

## 5.1 Umsetzungslücken zum entschiedenen Ablauf aus Abschnitt 4.4

**Grundlage: statischer Codeabgleich vom 04.10.2026 ohne neue Tests oder Prüfläufe; Status der Beschnitt-/Sichtbarkeitshinweise am 05.10.2026 und der Qualitätshinweise am 06.10.2026 nach Umsetzung und Prüfung aktualisiert.** Die Unterscheidung „relevanter Bereich unklar“ gegenüber „Aufnahme beziehungsweise verbleibende Vergleiche unbrauchbar“ ist im aktuellen Code noch nicht zuverlässig umgesetzt.

* In **StripDetector.Detect** werden unter anderem mehrere Gruppen, konkurrierende Fortsetzungen und eine zu große Kandidatenzahl zu demselben Ambiguous-Befund zusammengefasst. **ImageAnalyzer.Analyze** kann daraufhin bereits vor Referenz- und Flächenqualitätsprüfung mit AmbiguousPattern abbrechen. Auch der spätere Vergleich nach Geraderichtung verwendet diesen Status. Er belegt weder eine durch Auswahl lösbare Ursache noch die grundsätzliche Auswertbarkeit der Aufnahme.
* Die Statuswerte **NoPattern**, **UnsuitableGeometry**, **InvalidFields** und **InvalidReference** sowie die vorhandenen Hinweis-Codes unterscheiden einige Befunde, liefern aber keinen vollständigen Nachweis für die Bedienentscheidung aus 4.4. Insbesondere sind erfolglose Erkennung, unsichere Geometrie und nachgewiesene Aufnahmeunbrauchbarkeit nicht gleichzusetzen. Die Winkelzuversicht des Geraderichtens ist kein Ersatz für diese Ursachenunterscheidung.
* Die Mehrdeutigkeits- und Beschnitt-/Sichtbarkeitshinweise in **ImageAnalyzer** wurden am 05.10.2026 auf den belegten Befund begrenzt; die durchgängige Ursachenunterscheidung bleibt offen. Die unbelegten Ursachen- und Abhilfeaussagen der erreichbaren Qualitätshinweise wurden am 06.10.2026 korrigiert. Die anschließend am 06.10.2026 bereinigte Blur-Reichweite verwendet den breiten Kantenübergang ausschließlich diagnostisch, ohne lokale oder globale Messsperre (6.2). Der damit entfallene globale Ablehnungstext wurde entfernt; die durchgängige Ursachenunterscheidung aus 4.4 bleibt eine Phase-3-Umsetzungslücke.
* **AnalysisOptions/IImageAnalyzer** enthalten keinen ausdrücklichen Vertrag für eine bestätigte Bereichsauswahl mit Zuordnung zum Originalfoto. Originalpixelzugriff und geometrische Rückabbildung existieren, aber Auswahlbegrenzung, Referenzsuche und Bildkoordinaten für den neuen Ablauf sind noch durchgängig umzusetzen und zu prüfen.
* **MainPage** lädt mitgelieferte Testbilder. Das **AnalysisOverlayDrawable** und die eingabetransparente GraphicsView zeigen Ergebnisse, bieten jedoch keinen verschiebbaren/veränderbaren Auswahlrahmen. **AnalysisPresentation** besitzt bereits Abbruch-/Revisionsschutz für neue Analysen. Eine bestätigte Auswahl und deren eigener Analyseauftrag sind noch nicht angebunden oder geprüft.

Der am 05.10.2026 festgestellte begrenzte Korrekturbedarf an pauschalen Beschnitt-/Sichtbarkeitshinweisen ist umgesetzt, getestet und abschließend durch Codex geprüft. Die abgegrenzte Phase-1-Prüfung ist abgeschlossen; Nachweise und Grenzen stehen in Abschnitt 14. Die folgenden Phase-3-Umsetzungslücken bleiben bestehen.

Die Ermittlung geeigneter Bildbefunde, die Trennung der Ergebnisursachen, der Auswahlvertrag und die UI-Anbindung sind technische Umsetzungsarbeiten innerhalb der entschiedenen Produktgrenzen. Aus dem statischen Abgleich ergibt sich keine zusätzliche fachliche Detailentscheidung, die jetzt in OPEN.md aufzunehmen wäre. Falls sich bei der Umsetzung tatsächlich mehrere fachlich verschiedene Verhaltensweisen ergeben, ist ausschließlich diese konkrete Detailfrage vorzulegen; die Bedienentscheidung aus 4.4 bleibt geschlossen. Die Phasenzuordnung steht in Abschnitt 14.

---

# 6. Bekannte Altlasten im Code

Der vorhandene Code kann weiterhin Logik enthalten, die aus früheren, inzwischen verworfenen oder widersprüchlichen Produktannahmen entstanden ist.

Besonders zu prüfen sind:

## 6.1 Kanalendpunkt-/Clipping-Logik

**Kanalendpunkt-/Clipping-Altlast: bereinigt.**

Kanalwerte `0/255` und Near-Limit-Häufigkeiten sind diagnostische Merkmale und keine Ablehnungsgründe. Die frühere 2-%-Sperre und ihre globale Wirkung wurden entfernt. Regression: 51/51 Core-Tests grün. Tatsächliches informationszerstörendes Clipping bleibt ein separates, derzeit ungelöstes Untersuchungsproblem; daraus folgt aktuell keine Produktlogik.

## 6.2 Reflex-/Reflexschleier-Logik

Eine gleichmäßige globale Veränderung von Wand und Farbstreifen ist nicht automatisch ein Fehler.

Zu prüfen sind ausschließlich Fälle, in denen:

* Wand und Streifen unterschiedlich beeinflusst werden,
* Materialreflexionen unterschiedlich wirken,
* die relative Farbbeziehung tatsächlich verfälscht wird.

Keine pauschale Reflexschleier-Sperre ohne Nachweis.

**Globale `unevenSurface`-Sperre: bereinigt.**

* Die globale `unevenSurface`-Sperre wurde entfernt.
* Lokale räumliche Ungleichmäßigkeit wirkt nur noch auf die tatsächlich abhängigen Messungen.
* `PartiallyMeasured` bleibt für unabhängige gültige Vergleiche erhalten.
* Eine ungültige gemeinsame Wandreferenz entwertet weiterhin alle davon abhängigen Vergleiche.
* Regression des Umsetzungslaufs: 57/57 Core-Tests grün.
* Die lokalen Qualitätsgrenzen selbst bleiben experimentell und wurden durch diese Änderung nicht validiert.

Dieser Abschluss betrifft ausschließlich die globale `unevenSurface`-Sperrwirkung. Die globale Blur-Sperre wurde in jenem Arbeitspaket nicht verändert; ihr späterer Abschluss ist unten gesondert dokumentiert. Für den inzwischen bereinigten `unsafeCrop`-Pfad gilt der folgende Status.

**Globale `unsafeCrop`-Sperre: bereinigt.**

* Die globale `unsafeCrop`-Sperre wurde entfernt.
* Ein bereits geometrisch zugelassener Endbeschnitt verursacht bei einem lokalen Qualitätsproblem keine globale Ablehnung mehr.
* Die Kontrollfläche des angeschnittenen Feldes bleibt mit allen bisherigen Unbrauchbarkeitsgründen lokal wirksam.
* Lokale bzw. gemeinsame Referenzen wirken entsprechend ihrer tatsächlichen Abhängigkeiten.
* `PartiallyMeasured` bleibt für unabhängige gültige Vergleiche erhalten.
* Regression des Umsetzungslaufs: 68/68 Core-Tests grün.
* Geometrische Crop-Zulassung und Qualitätsgrenzen wurden nicht verändert oder validiert.

**Blur-Reichweite: abgegrenzte Phase-1-Bereinigung am 06.10.2026 implementiert, getestet und gemäß Nutzerfreigabe durch Codex abschließend selbst geprüft; abgeschlossen.**

* Die vorhandene Prüfung beobachtet den Farbverlauf an vier Feldkanten: Kanalspanne mindestens 10 und größte benachbarte Stufe relativ zur Spanne kleiner als 0,18. Ein positiver Befund beweist weder Blur als Ursache noch die Unbrauchbarkeit einer Messfläche oder sämtlicher Vergleiche. Die Berechnung und ihre bisherigen Diagnosegrenzen bleiben unverändert.
* Die allein daraus abgeleitete lokale Ungültigkeit und die anschließende globale Löschung aller Lab-/ΔE-Werte wurden entfernt. Ein feldbezogener Diagnoseeintrag bleibt erhalten. Keine pauschale lokale Ersatzsperre, neue Qualitätsheuristik oder Schwelle.
* Geometrie, Geraderichtung, Konturmasken, Originalpixelauswahl und Referenzauswahl bleiben unverändert. Bestehende Flächen-/Kontrollflächen- und Referenzprüfungen entscheiden weiterhin gemäß tatsächlichen Abhängigkeiten; ein Kantenbefund überstimmt sie nicht mehr.
* Grundlage: die vorhandenen 84 synthetischen Diagnoseläufe mit isolierten und feldübergreifenden Auswirkungen, ohne Wiederholung der Gesamtserie. Eine Rangumkehr unter weiterhin gültigen Nachbarfeldern war in dieser Serie nicht nachgewiesen. Bei `V-straight-shared-one-4` und seinem horizontalen Gegenstück sind alle drei Feldmessungen einschließlich der auffälligen Kante unverändert und werden nun ausgegeben; eine lokale Sperre wäre dort ebenfalls unbegründet.
* Gezielter aktueller Nachweis: rotierte, texturierte Szenen mit lokaler Mehrkantenunschärfe verändern Winkel, Referenzposition und Originalpixelmengen anderer Felder. Die geprüften freigegebenen Pixel liegen weiterhin im zugehörigen physischen Feld bzw. auf der Wand; Lab und ΔE stimmen mit der Messung dieser ausgewählten Originalpixel überein, der nächste Kandidat bleibt erhalten. Bei kleinen Feldern verwirft die vorhandene Geometrie-/Kontrollflächenprüfung das gestörte Feld lokal; unabhängige Nachbarn bleiben auswertbar. Eine veränderte Pixelmenge allein ist kein nachgewiesener falscher Vergleich.
* Globale Gegenprobe: die durch starken Blur unbrauchbare gemeinsame Referenz führt weiterhin zu keiner abhängigen Farbaussage und nun zu `InvalidReference`, ohne Überdeckung durch den pauschalen Blur-Status. Zusätzlich wurde eine gezielt unbrauchbare gemeinsame Referenz im isolierten Kantenfall geprüft. Beide Streifenrichtungen sowie gemeinsame und feldweise Referenzen sind in den gezielten Fällen berücksichtigt.
* Generatorwissen und ungestörte Baselines werden ausschließlich nachgelagert im Test zur Feld-/Pixelzuordnung und Ergebnisprüfung verwendet. Der Analyzer erhält unverändert nur das einzelne fertige RGB-Bild und normale Analyseoptionen; keine Bildveränderung durch Iro.
* Nachweise: Fehlverwerfung vor Korrektur reproduziert; danach 16/16 neue Blur-Reichweitentests, insgesamt 35/35 betroffene Tests und 123/123 Core-Tests grün. Zwei frühere Tests einer pauschalen globalen Blur-Sperre wurden durch konkrete Farbvergleichs-/Abhängigkeitsprüfungen ersetzt. 14 bereits vorhandene komprimierte Diagnose-Eingabebilder sind als Regressionseingaben übernommen; keine neue allgemeine Diagnoseserie. Selbstprüfung des Paketdiffs durchgeführt; kein gesondertes Claude-Review für dieses Paket erforderlich.
* Grenzen: Die vorhandenen Prüfungen behandeln die untersuchten Fälle ohne pauschale Blur-Sperre korrekt. Dies ist kein allgemeiner Nachweis, dass jede optische Unschärfe oder jeder Verlust relativer Farbinformation im Einzelbild erkannt wird. Gaussian-Testmodell und synthetische Szenen sind keine Smartphone-Optik-/Gerätevalidierung. Keine neuen Fehlergrenzen, keine allgemeine Rangfolgestabilität und keine Validierung der übrigen Qualitätsgrenzen. Gerätevalidierung bleibt Phase 5. Zum Abschluss des Blur-Pakets blieb die Verjüngungssperre separat offen; ihr späterer Abschluss vom 07.10.2026 steht in Abschnitt 14. Phase 1 bleibt bis zur Freigabe des Phasenwechsels aktiv.

## 6.3 Helligkeits-/Dunkelheitsregeln

Absolute Dunkelheit oder Helligkeit allein rechtfertigt keine Ablehnung.

Entscheidend ist, ob die relative Farbmessung unzuverlässig wird.

**Helligkeits-/Dunkelheitsregeln: geprüft, keine eigenständige Altlast gefunden.**

* Im aktuellen Analyzer existiert keine eigenständige absolute Hell-/Dunkel-Sperre.
* `NearLimitFraction` ist lediglich diagnostisch.
* Legitime homogene schwarze oder weiße Messflächen werden nicht allein wegen ihrer absoluten Helligkeit verworfen.
* Bestehende Schwellen für Regionserkennung, Geraderichten, Blur, MAD, Ausreißer und `SpatialDeltaE` sind keine absoluten Helligkeitssperren.
* Belichtungsänderungen können diese Mechanismen indirekt beeinflussen; ihre jeweiligen Regeln werden dadurch nicht automatisch validiert.
* Tatsächliche Über-/Unterbelichtung kann durch Quantisierung oder Begrenzung relative Farbinformation zerstören.
* Der aktuelle Einzelbild-Analyzer kann diesen Informationsverlust nicht zuverlässig von einer legitimen homogenen Endpunktfarbe unterscheiden.
* Daraus folgt aktuell weder eine neue Hell-/Dunkel-Sperre noch ein neuer Grenzwert.

## 6.4 Alte Mehrbild-/Zeitlogik

**Geprüft, keine fachliche Mehrbild-/Zeitlogik gefunden; statische Prüfung.**

Jegliche verbliebene Logik, die mehrere Bilder zusammenführt oder zeitliche Bestätigung voraussetzt, widerspricht dem Einzelbildprinzip und ist zu entfernen, sofern sie tatsächlich noch wirksam ist.

## 6.5 Alte Test- und Erwartungsinfrastruktur im Produktbestand

**Gezielte Bereinigung der 18 identifizierten Alt-Erwartungen: abgeschlossen.**

Die Bewertungswirksamkeit dieser Aufnahmen ist anhand ihrer konkreten Aufnahme-IDs aufgehoben. Der `AnalysisRunner` übergibt die Identität an den Evaluator; alte Erwartungen erzeugen damit `NotEvaluated` mit Begründung statt `Passed`/`Failed`. Ein anderes verified-Kennzeichen, ein neuer Begründungstext oder eine neuere Formatversion legitimieren diese Aufnahmen nicht erneut.

Bilder, Ground Truth, ursprüngliche Erwartungen, Analyseergebnisse und diagnostische Vergleiche bleiben erhalten. Allgemeine Bewertungsfunktionen bleiben nutzbar; keine alte Erwartung wurde erneut freigegeben. Die Identitätsliste ist kein allgemeines Freigabesystem für andere Pakete und ändert keine früher gespeicherten Berichte. Die technischen Punkte aus 6.6 blieben im damaligen Arbeitspaket unberührt; ihr aktueller Abschlussstand steht in Abschnitt 6.6.

Regression des Umsetzungslaufs: 7/7 Analyse-Tests und 68/68 Core-Tests grün. Die Analyse-Tests verwenden die 18 archivierten Bild-/Metadatenpaare mit temporären Transporthüllen und kontrollierten Analyzer-Antworten; sie validieren keine Qualitätsregel dieser Bilder.

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

**Beide technischen Bereinigungen: implementiert, getestet und im ausdrücklich beauftragten Codex-Review am 05.10.2026 abgenommen; Arbeitspaket abgeschlossen.**

1. **Projektstammerkennung repariert:** `iro-gen/IroTestHandoff.cs` erkennt den Stamm anhand von `iro.slnx`, `iro-gen/IroGen.csproj` und `src/iro.analysis/Iro.Analysis.csproj`. Die Abhängigkeit von der entfernten Datei `IRO-KONSOLIDIERTER-PLAN.md` wurde beseitigt; kein fest codierter Projektpfad und keine Wiederherstellung der alten Datei im Projektbestand.
2. **Solution bereinigt:** Der Verweis auf das nicht vorhandene `tests/iro.gen.tests/IroGen.Tests.csproj` samt zugehörigem Solution-Ordner wurde entfernt. Alle sechs gültigen Projektverweise bleiben erhalten; kein historisches Testprojekt wurde rekonstruiert.

**Nachweise des Umsetzungslaufs vom 04.10.2026:**

* Gezielte Regression vor der Korrektur: 4 von 5 Testfällen fehlgeschlagen; danach 5/5 bestanden. Geprüft wurden tatsächlicher Projektstamm, verwendete Startverzeichnisse, versetzte Projektstruktur und Zurückweisung ungültiger Strukturen.
* Automatisierter Übergabetest mit synthetischem Generatorbild und isoliertem temporärem Projektstamm: `BatchGenerator.Generate` → `IroTestHandoff.Submit` → realer `AnalysisRunner`/`PngAnalysisApi` → gespeichertes Ergebnis. Bildprüfsumme und Ergebnisgleichheit zur direkten PNG-Analyse geprüft; Generatorwissen bleibt in der nachgelagerten Ergebnisprüfung.
* Analyse-Regression 12/12 und Core-Regression 92/92 grün. `dotnet build iro.slnx` erfolgreich mit 0 Fehlern und 0 Warnungen, einschließlich IroGen und Android-App.

**Review und Grenzen:** Statisches Codex-Review von Code, Projektverweisen, Paketdiff und vorhandenen Testnachweisen ohne abnahmehindernden Befund. Kein gesondertes Claude-Review durchgeführt. Keine manuelle WPF-Klickprüfung und keine reale Geräteprüfung; der automatisierte Übergabetest ist kein solcher Nachweis. Keine fachlichen Nebenänderungen oder neue Produktentscheidungen. Zum damaligen Reviewzeitpunkt blieb Phase 1 insgesamt offen; der aktuelle Phasenabschlussstand steht in Abschnitt 14.

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

Bestehende experimentelle Qualitätsregeln sind als vorläufiger Implementierungsstand zu prüfen. Ihre Existenz bestätigt weder Produktreife noch die Ursache oder Reichweite eines Fehlers. Daraus darf insbesondere keine Ausnahme vom verbindlichen Ablauf in Abschnitt 4.4 abgeleitet werden. Bekannte Abweichungen bleiben ausdrücklich Umsetzungslücken, bis sie in einem freigegebenen Auftrag bereinigt sind; dieser Dokumentationsauftrag ändert weder Code noch Grenzen.

Eine durch Bereichsauswahl klärbare Auswahlfrage ist keine Qualitätsablehnung. Umgekehrt darf ein Qualitätsproblem nicht durch den Rahmen umgangen werden. Die Auswahl des Nutzerhinweises und die Aufforderung zur Neuaufnahme benötigen die in Abschnitt 4.4 beschriebene Befundgrundlage. Zuverlässige unabhängige Teilvergleiche bleiben gemäß Abschnitt 4.3 erhalten.

Es gilt:

* keine Sperre allein aufgrund theoretischer Vorsicht,
* jede Sperre braucht einen nachgewiesenen Zusammenhang mit einem unvertretbaren Fehlrisiko,
* experimentelle Grenzwerte bleiben experimentell,
* vorhandene Ablehnungen müssen gegen reale Daten validiert werden,
* Produktreife ist erst nach realen Gerätetests gegeben.

Für Kanalendpunkte gilt der abgeschlossene Stand aus Abschnitt 6.1. Bis zur gesonderten Entscheidung bzw. Validierung dürfen Match-/No-Match-Logik, ΔE-Grenzwerte, Qualitätsgrenzen und weitere Bildqualitätsheuristiken nicht eigenmächtig verändert werden.

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

**Abschlussstand vom 07.10.2026: Phase 1 ist im dokumentierten Umfang fachlich-technisch abgeschlossen und bereit für die ausdrückliche Nutzerfreigabe zum Phasenwechsel. Keine Aufgabe der aktuellen Phase-1-Restliste verbleibt. Phase 2 wurde nicht begonnen.**

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

1. Kanalendpunkt-/Clipping-Altlast: bereinigt; siehe Abschnitt 6.1
2. Reflex-/Reflexschleier-Sperren: lokale Abhängigkeiten bereinigt; Qualitätshinweise und Blur-Reichweite bereinigt; begrenzte Nachweise und verbleibende Validierungsgrenzen siehe 6.2
3. Dunkel-/Hell-Sperren: geprüft / keine eigenständige Bereinigung erforderlich; siehe Abschnitt 6.3
4. Mehrbild-/Zeitlogik: geprüft, keine fachliche Altlast gefunden; statische Prüfung, siehe Abschnitt 6.4
5. alte Erwartungs-/Testinfrastruktur: gezielte Bewertungsbereinigung der 18 identifizierten Alt-Erwartungen abgeschlossen; siehe Abschnitt 6.5
6. Beschnitt-/Sichtbarkeitslogik: abgegrenzte Phase-1-Prüfung einschließlich Hinweiskorrektur am 05.10.2026 abgeschlossen; begrenzte Nachweisreichweite und Phase-3-Lücken bleiben bestehen (Stand unten)
7. sonstige Qualitätsheuristiken: statisch eingeordnet; Qualitätshinweise und globale Verjüngungssperre bereinigt; begrenzte Nachweise und Validierungsgrenzen siehe Abschluss unten

**Restliste Phase 1 – Abschlussstand vom 07.10.2026: keine offenen Arbeiten innerhalb der abgegrenzten Phase-1-Prüfung.**

Die Bestandsaufnahme vom 05.10.2026 identifizierte Qualitätshinweise, Blur-Reichweite und globale Verjüngungssperre als Restarbeiten. Die ersten beiden Pakete wurden am 06.10.2026 abgeschlossen (6.2 und unten); der letzte Punkt ist mit folgendem Nachweis bereinigt. Frühere Bestandsaufnahme und Testzahlen bleiben zeitlich zugeordnete Nachweise, keine allgemeine Validierung experimenteller Grenzen.

**Globale Verjüngungssperre: am 07.10.2026 implementiert, getestet und gemäß Nutzerfreigabe durch Codex abschließend selbst geprüft; abgeschlossen.**

* Ausgangsbefund: `MeasurementSafety.HasStrongCoherentTaper` erkennt einen kohärenten Breitenverlauf (>15 %, Fit >=0,90, mindestens zwei unterstützende Konturen mit jeweils mindestens 2,5 % Breitenänderung). `ImageAnalyzer` brach damit vor Flächen- und Referenzmessung global ab. Der geometrische Befund bewies keine unzuverlässige Farbmessung sämtlicher Felder.
* Begrenzte Diagnose: trapezförmige, entlang des Streifens schmaler werdende Felder gegen einen Kontrollstreifen konstanter Breite; beide Streifenrichtungen, Bilddrehung 0° und 7°. Die vorhandene Geraderichtung wurde regulär angewendet, nicht durch Sollwinkel ersetzt. Die ermittelten Winkel (unter anderem 0,8° bei ungedrehter Verjüngung und 6,3° statt 7°) sind keine validierte Perspektivschätzung; im geprüften Fall bleiben die Messflächen dennoch korrekt zugeordnet.
* Tatsächliche Originalpixelmengen anhand erkannter Rechtecke, Konturmasken, vorhandener Stichprobenauswahl und unveränderter Referenzwahl nachverfolgt. Alle freigegebenen Feldpixel gehören zum richtigen physischen Feld, alle Referenzpixel zur Wand. Samplezahlen stimmen überein; Lab und ΔE entsprechen den bekannten homogenen Eingabefarben, ebenso der nächste Kandidat. Keine manuell günstig gewählten Ersatzflächen oder zusätzliche Weißreferenz.
* Gegenproben: Ein räumlich ungleichmäßiges Feld wird durch die vorhandene Flächenprüfung lokal ausgeschlossen (`PartiallyMeasured`); die zwei gültigen Nachbarn behalten korrekte Farbvergleiche. Eine unbrauchbare gemeinsame Referenz entwertet alle abhängigen Vergleiche (`InvalidReference`). Konkurrierende Geometrie bleibt `AmbiguousPattern` ohne Farbvergleiche. Beide Streifenrichtungen geprüft. Diese konkreten Nachweise ersetzen keine allgemeine Geometrievalidierung.
* Kleinste Korrektur: genau eine Anweisung in `ImageAnalyzer.cs` ersetzt den globalen Verjüngungsabbruch durch einen diagnostischen Geometrieeintrag. Die Befundberechnung und ihre Grenzen in `MeasurementSafety` bleiben unverändert. Keine lokale Ersatzsperre, neue Schwelle, Qualitätsheuristik, Änderung der Geraderichtung, Konturen, Messflächen, Messmethode oder Referenzauswahl.
* Vorher/Nachher: Eine temporäre Analyzer-Kopie ausschließlich im Testbereich erlaubte zunächst die Diagnose hinter dem Abbruch; nur dieser Verjüngungsabbruch war dort ausgelassen. 10/10 Diagnosefälle bestanden. Die Hilfe wurde vor der Produktregression entfernt. Am unveränderten Produktpfad scheiterten acht Fälle am vorzeitigen Abbruch (bei zwei Referenzaufbauten bereits vor dem Zugriff auf die Referenz); zwei Mehrdeutigkeitsfälle bestanden. Nach der Korrektur: 10/10 neue Verjüngungstests, 24/24 betroffene Tests und 133/133 Core-Tests grün. Die beiden früheren Verjüngungs-Hinweistests prüfen nun den Diagnoseeintrag bei erhaltenen Vergleichen, nicht die überholte Gesamtablehnung.
* Generatorgeometrie und Sollfarben dienen ausschließlich der nachgelagerten Testprüfung. Der Analyzer erhält nur das einzelne fertige Eingabebild und reguläre Optionen. Eingabepixel bleiben unverändert. Paketdiff selbst geprüft; andere Arbeitsänderungen erhalten, kein gesondertes Claude-Review erforderlich.
* Grenzen: keine allgemeine Perspektiv-, Material- oder Gerätevalidierung, keine neue Zusicherung für extreme Verjüngung oder beliebige Verdeckungen. Die vorhandenen lokalen Qualitäts- und Geometriegrenzen bleiben experimentell. Die geprüfte Fehlverwerfung ist behoben; es entstand keine zusätzliche offene Produktentscheidung.

**Phase-1-Abschluss:** Die dokumentierten Altlasten wurden geprüft, nachgewiesene Widersprüche in den freigegebenen Paketen bereinigt und verbleibende Grenzen ausdrücklich erhalten. Die aktuelle Restliste ist leer. Phase 1 ist damit fachlich-technisch abgeschlossen und bereit für die Nutzerfreigabe zum Phasenwechsel; dies ist keine Produktreife- oder allgemeine Gerätefreigabe. Phase 2 beginnt erst nach gesonderter Nutzerfreigabe.

**Sonstige Regeln eingeordnet, ohne neue Freigabe:** MAD >12, Ausreißeranteil >18 %, SpatialDeltaE >2 sowie Mindestgröße/Samplezahl wirken auf die jeweilige Messung; Referenzabhängigkeiten und Teilauswertung bleiben wie geprüft erhalten. Für diese lokalen Grenzwerte ist in dieser Bestandsaufnahme kein zusätzlicher konkreter Phase-1-Widerspruch nachgewiesen. Regionstoleranz, Füllgrad, Raster, Konturfit und Winkelzuversicht bleiben experimentelle Erkennungs-/Geometrieparameter; geringe Winkelzuversicht führt im `ImageStraightener` zur Analyse ohne Drehung, nicht allein zur Aufnahmeablehnung. Diese Einordnung validiert weder Zahlenwerte noch allgemeine Messflächengüte. Die untersuchten feldübergreifenden Blur-Effekte sind im abgeschlossenen begrenzten Paket aus 6.2 eingeordnet; dessen Nachweisgrenzen bleiben erhalten. Fehlende reale Kalibrierung allein ist kein zusätzlicher Phase-1-Blocker.

**Abgeschlossen bleiben:** StripDetector-Nachbarschaftsmittelung; Kanalendpunkt-Sperren; globale `unevenSurface`-/`unsafeCrop`-Wirkung; Hell-/Dunkel- und Mehrbild-Bestandsaufnahme ohne eigenständige Altlast; die gezielte Bereinigung der 18 Alt-Erwartungen; beide technischen Punkte aus 6.6; die abgegrenzte Beschnitt-/Sichtbarkeitsprüfung einschließlich Hinweisen. Kein neuer Gegenbefund zu diesen Paketen.

**Später bzw. geparkt:** Phase 2: grundlegende Baseline-Verträge; Phase 3: durchgängige Ursachenunterscheidung, Auswahlvertrag und interaktiver Ablauf aus 5.1; Phase 4: Kamera; Phase 5: reale Geräte-/Material-/Optikvalidierung; Phase 6: Match/No-Match. O-002 (Medianverfahren) bleibt bewusst geparkt und ist keiner neuen Phase zugewiesen. Tatsächliches informationszerstörendes Clipping bleibt ungelöst, ohne neue Produktlogik oder neuen Phase-1-Bereinigungsauftrag. Die zuvor offenen Phase-1-Restpunkte wurden durch die dokumentierten Korrekturen und Nachweise abgeschlossen, nicht durch Verschieben in spätere Phasen. Die ausdrücklich geparkten Entscheidungen und späteren Umsetzungsschritte bleiben bestehen.

**Hinweispaket „Unbelegte Qualitätsaussagen korrigieren“: am 06.10.2026 implementiert, getestet und gemäß ausdrücklicher Nutzerfreigabe durch Codex abschließend selbst geprüft; abgeschlossen.**

* Ausschließlich sechs erreichbare Stringliterale in `MeasurementSafety.cs`, `RegionSampler.cs` und `ImageAnalyzer.cs` korrigiert: breiter Kantenübergang, räumliche Farbunterschiede, Farbstreuung/Ausreißer, kleine Kandidaten und kohärent veränderliche Feldbreiten. Keine unbewiesene Ursache, pauschale Aufnahmeunbrauchbarkeit, Abhilfe oder noch nicht verfügbare Bereichsauswahl. Die globale Nichtausgabe wird als bestehende Ausgabeentscheidung beschrieben.
* Vorher: 10 von 11 gezielten Fällen scheiterten ausschließlich am alten Hinweis, ein bestehender Fall blieb grün. Nachher: 33/33 betroffene Hinweis-/Abhängigkeitstests und 107/107 Core-Tests grün. Synthetische Tests prüfen Feld-, Kontrollflächen-, lokale/gemeinsame Referenz- und globale Ergebnispfade einschließlich `AnalysisPresentation`; Taper-Hinweis in beiden Richtungen. Ein anfänglicher Test-Compilefehler durch einen falsch benannten Diagnosewert wurde vor der Reproduktion korrigiert.
* Abschlussprüfung: Paketdiff und Vergleich des Produktcodes nach Ausblenden der Stringliterale bestätigen unveränderte Analyseentscheidungen, Statuswerte, Sperren, Grenzwerte, Messverfahren, Messflächen und Referenzauswahl. Vorhandene fremde Änderungen erhalten. Kein gesondertes Claude-Review für dieses Paket erforderlich.
* Grenzen: keine manuelle App-/Geräteprüfung und keine neue Qualitätsvalidierung. Der unbenutzte `UnevenSurfaceHint` hat keinen Ausgabepfad und wurde nicht geändert. Zum Abschluss dieses reinen Hinweispakets blieben Blur-Reichweite und Verjüngungssperre offen; die Hinweistests legitimieren diese Sperren nicht. Der anschließend gesondert beauftragte Abschluss der Blur-Reichweite steht in 6.2. Die Verjüngungssperre wurde anschließend am 07.10.2026 gesondert bereinigt (Abschluss oben); Phase 1 bleibt bis zur Freigabe des Phasenwechsels aktiv.

**Stand der Beschnitt-/Sichtbarkeitslogik: abgegrenzte Phase-1-Prüfung abgeschlossen; begrenzte Teilauswertung und korrigierte Hinweise geprüft, keine allgemeine Beschnitt- oder Gerätevalidierung.**

Die folgenden Angaben beschreiben den bisherigen Implementierungs- und Prüfstand. Insbesondere bestätigen Tests bestehender Mehrdeutigkeitsablehnungen nicht deren Eignung als Neuaufnahmeentscheidung oder Rahmenauslöser. Maßgeblich ist die Unterscheidung aus Abschnitt 4.4; die technische Lücke steht in Abschnitt 5.1.

* Die gescheiterte Zulassung eines einzelnen lokalisierten Endrestes beendet nicht mehr automatisch die Auswertung vollständiger Innenfelder. Bei gültigen Vergleichen gilt PartiallyMeasured; der Rest bleibt ungemessen und wird separat als unsicher markiert.
* Die Fortsetzung ist auf einen horizontalen oder vertikalen Streifen ohne angewandte Geraderichtung begrenzt: mindestens zwei vollständige Anker, keine Mustermehrdeutigkeit oder inkonsistenten Innenfelder, genau eine normale Randregion und keine zusätzlichen schwachen Randreste. Die Quergrenzen aller vollständigen Felder müssen innerhalb der vorhandenen 2-px-Toleranz übereinstimmen. Vorhandene Endlage-, Größen-, Abstands- und Fortsetzungskriterien bleiben wirksam.
* Die Quergrenzen-Zulassung des Randfeldes selbst bleibt unverändert. Nur sein Ausschluss von der Messung ermöglicht die getrennte Fortsetzung; keine neue Schwelle und keine Sonderbehandlung anhand einer Verschiebung oder Testbildidentität.
* Die vorhandene Referenzauswahl verwendet den verbleibenden Feldverbund und schließt weiterhin sämtliche Randregionen aus. Ihre Position kann sich ändern. Ein Test mit gezielt unbrauchbarer neu gewählter Wandreferenz bestätigt, dass keine abhängigen Vergleiche freigegeben werden; die homogene Diagnosewand gilt nicht als allgemeiner Referenznachweis.
* Bildrahmen und Werteliste verwenden dieselben Feldidentitäten. Sichere Vergleiche werden nummeriert markiert; unsichere Randbereiche separat ohne ΔE oder Nächstliegend-Kennzeichnung. Die unbekannte Gesamtzahl wird nicht behauptet.
* Nachweis: 6-px-Fall zunächst in beiden Richtungen rot, danach grün. Core-Regression 92/92, Analyse-Regression 7/7 grün. Bestehende zugelassene Endfelder bleiben auswertbar; gemeinsame Mehrdeutigkeit, zusätzliche Randreste und angewandte Geraderichtung bleiben abgesichert.
* App-Build erfolgreich. Zeichenroutine an vier Crop-Bildern und beiden vorhandenen App-Testbildern über ein temporäres Desktop-Zeichenbackend geprüft; keine Android-Geräte-/Emulatorprüfung. Keine Smartphone- oder allgemeine Beschnittvalidierung.
* Im damaligen Arbeitspaket wurden Blur, Medianmethode, Match-/No-Match, sonstige Qualitätsgrenzen und Abschnitt 6.6 nicht geändert. Dies ist keine Freigabe widersprechender Bedien- oder Ablehnungsregeln; Abschnitt 4.4 gilt. Phase 1 bleibt aktiv.

**Abschließende Phase-1-Einordnung vom 05.10.2026 (statische Analyse, vorhandene Testnachweise; keine neuen Läufe):**

* **Ausreichend geprüft im begrenzten Anwendungsbereich:** Der nicht zugelassene einzelne Endrest bleibt ungemessen; vollständige Innenfelder behalten Originalpixelmessung, ΔE und Bildzuordnung. Normale und schwache erkannte Randregionen bleiben Referenzausschlüsse (`ImageAnalyzer.cs:55–88, 166–176`). Lokale Mess-/Kontrollflächen- und Referenzfehler betreffen ihre abhängigen Vergleiche; eine unbrauchbare gemeinsame Referenz gibt keine Vergleiche frei. Belegt durch `ExcludedEndCropTests` und `CropDependencyTests`; vorhandene Core-Nachweise vom 02. und 04.10.2026: jeweils 92/92 grün, darunter 35 Beschnitt-Testfälle. Dies bestätigt keine allgemeine Geometrie- oder Gerätevalidierung und validiert keine Blur-Regel.
* **Verbleibende Grenzen:** Rotation, zusätzliche/fragmentierte Reste, fehlende Anker und inkonsistente Innenfelder erfüllen die begrenzte Fortsetzung nicht (`MeasurementSafety.cs:55–101`). Die vorhandenen Negativtests belegen die Nichtfreigabe, nicht die Unbrauchbarkeit sämtlicher übriger Vergleiche. Aus den bisherigen Nachweisen ergibt sich außerhalb des bereits bereinigten Falls keine weitere konkret bewiesene Fehlverwerfung unabhängiger Farbvergleiche; ebenso wenig ein allgemeiner Nachweis für lokale Isolierbarkeit. Vollständig außerhalb des Bildes liegende Felder sind daraus nicht bestimmbar und werden nicht erfunden. Diese Grenzen bleiben ausdrücklich ungeklärt, ohne daraus eine neue globale Produktregel abzuleiten.
* **Hinweiswiderspruch zu 4.3/4.4 inzwischen bereinigt:** Die statische Prüfung identifizierte pauschale Vollständigkeits-/Ausschnittforderungen in `ImageAnalyzer.cs:39, 44, 104, 110, 131`. Im anschließenden freigegebenen Korrekturpaket wurden ausschließlich diese fünf Textstellen geändert: nicht eindeutige Musterzuordnung, nicht sicher zugeordnete Randgeometrie, abweichende Feldgrenzen und nicht gefundene Wandreferenz. Die Hinweise behaupten keine Aufnahmeunbrauchbarkeit oder unbewiesene Ursache und bieten keinen Auswahlrahmen an. Bestehende Hinweise bei Teilauswertung begrenzen die Aussage weiterhin auf die ausgewerteten Felder.
* **Phase 3 bleibt getrennt:** `StripDetector.cs:172–219` und `ImageAnalyzer.cs:39, 131` unterscheiden die verschiedenen Mehrdeutigkeitsursachen noch nicht durchgängig. Ursachenvertrag, zuverlässige Entscheidung über ein Rahmenangebot, Auswahlvertrag und UI-Ablauf bleiben die bereits zugeordneten Umsetzungslücken aus 5.1 und 14. Die vollständige Umsetzung ist kein zusätzlicher Abschlussauftrag für diese Phase-1-Prüfung. Reale Gerätevalidierung bleibt Phase 5.

**Abschluss des freigegebenen Hinweis-Korrekturpakets am 05.10.2026:**

* Reproduktion vor Änderung: sieben Hinweisfälle rot, zwei Teilauswertungsfälle bereits grün; zusätzlich der späte Mehrdeutigkeitspfad nach Geraderichtung am alten Hinweis rot.
* Nach Änderung: 10/10 gezielte `VisibilityHintTests` und 102/102 Core-Tests grün. Die Tests verwenden synthetische Eingabebilder und prüfen die tatsächlichen Ergebnispfade einschließlich Weitergabe an `AnalysisPresentation`; keine Geräteprüfung.
* Abschließende Selbstprüfung durch Codex gemäß ausdrücklicher Nutzerfreigabe durchgeführt, ohne weiteren belegten Widerspruch innerhalb dieses abgegrenzten Pakets. Kein gesondertes Claude-Review erforderlich. Produktdiff: ausschließlich fünf Stringliterale in `ImageAnalyzer.cs`; Zulassungen, Statusentscheidungen, Messflächen, Referenzauswahl, Freigaben und Grenzwerte unverändert.
* Die abgegrenzte Beschnitt-/Sichtbarkeitsprüfung in Phase 1 ist damit abgeschlossen. Die oben dokumentierten Grenzen für Rotation, weitere Randreste und andere nicht nachgewiesene Fälle sowie die offenen Phase-3-Umsetzungslücken bleiben ausdrücklich erhalten. Keine allgemeine Beschnitt-/Gerätevalidierung, keine Änderung oder Validierung von Blur oder anderen Qualitätsheuristiken. Phase 1 insgesamt bleibt aktiv.

**Status der StripDetector-Glättung:**

* Nachbarschaftsmittelung im `StripDetector` entfernt.
* Neue Originalpixel-/Erkennungstests vorhanden.
* Ergebnis des Umsetzungslaufs: 26/26 Tests grün.
* Punkt damit technisch abgeschlossen, sofern kein neuer Befund entsteht.

Die beiden technischen Bereinigungen aus Abschnitt 6.6 sind implementiert, getestet und im beauftragten Codex-Review abgenommen. Dieses Arbeitspaket ist abgeschlossen. Phase 1 ist inzwischen im dokumentierten Umfang abgeschlossen und bleibt bis zur ausdrücklichen Freigabe des Phasenwechsels die aktuelle Phase.

**Einordnung der Umsetzung aus Abschnitt 4.4:**

* In Phase 1 sind die vorhandenen Mehrdeutigkeits-/Ablehnungsregeln und Nutzerhinweise anhand der Ursachen und tatsächlichen Abhängigkeiten einzuordnen. Nachgewiesene Widersprüche sind in abgegrenzten Codeaufträgen zu bereinigen; fehlende Ursachenunterscheidung und verbleibende Unsicherheiten sind ausdrücklich festzuhalten. Es ist nicht erforderlich, jede denkbare Aufnahme oder jede Gerätegrenze bereits hier zu lösen.
* Die durchgängige technische Unterscheidung für das Rahmenangebot, der Originalfoto-/Auswahlvertrag und der interaktive Ablauf gehören zum End-to-End-Testmodus in Phase 3. Seine benötigten Core-Grundlagen und Nachweise sind dabei ausdrücklich Teil der Umsetzung; sie gelten nicht bereits als vorhanden.
* Phase 2 sichert vorhandene, aktuell entschiedene Grundverträge. Neue Verhaltenstests für Bereichsauswahl und Ergebniswechsel entstehen mit der zugehörigen Umsetzung gemäß Abschnitt 9.1.
* Geräteabhängige Zuverlässigkeit wird in Phase 5 validiert. Fehlende Gerätevalidierung ist keine Erlaubnis, Auswahl- und Qualitätsprobleme pauschal gleichzusetzen.

Diese Zuordnung erlaubt keinen Phasenwechsel und ist kein Implementierungsauftrag.

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

1. Testbild als unverändertes Originalfoto laden und zuerst automatisch im Ganzbild untersuchen.
2. Wandreferenz nachvollziehbar anzeigen; sichere Felder erkennen und aus Originalpixeln messen.
3. Zuverlässige Vergleiche einschließlich zulässiger Teilauswertung markieren, mit ΔE anzeigen und den nächstliegenden ausgewerteten Kandidaten bestimmen.
4. Die Ursachen aus Abschnitt 4.4 technisch unterscheiden; nur bei grundsätzlich auswertbarer Aufnahme und unklarem relevanten Bereich den verschiebbaren und veränderbaren Auswahlrahmen anbieten.
5. Nach bestätigter Auswahl einen neuen unabhängigen Analysevorgang innerhalb der ausgewählten Originalpixel starten: Referenz- und Feldbestimmung, dieselben Qualitätsanforderungen und eindeutige Rückzuordnung der Anzeige zum Originalfoto.
6. Bei nachgewiesener Unbrauchbarkeit ohne zuverlässige verbleibende Vergleiche verständlich zur Neuaufnahme auffordern; konkrete Aufnahmehinweise nur aus belegtem Befund. Kein Rahmen als Qualitätsumgehung und keine geratenen Feldgrenzen.
7. Ergebnisse, Markierungen und laufende Aufträge eindeutig dem jeweiligen Analysevorgang zuordnen; keine Übernahme alter Messwerte und keine veralteten Ergebnisse der vorherigen Auswahl.

Vorhandene automatische Analyse, Ergebnisanzeige und Revisionsschutz sind Ausgangspunkte. Die in Abschnitt 5.1 genannten fehlenden Fähigkeiten sind in dieser Phase umzusetzen und gezielt nachzuweisen; der aktuelle Stand ist noch kein vollständiger End-to-End-Nachweis.

# PHASE 4 – Kamera

Erst nach stabilem End-to-End-Testmodus:

* Android-Kamera anbinden,
* notwendige Kameraeinstellungen bereitstellen,
* ein Originalfoto pro unabhängigem Analysevorgang; eine bestätigte Bereichsauswahl auf demselben Foto folgt Abschnitt 3 und 4.4,
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

Die sachliche Statuspflege gemäß `AGENTS.md`, Abschnitt 18.1, gehört zum jeweiligen Auftrag und benötigt keine erneute Nutzerfreigabe. Änderungen an Produktanforderungen, Grenzwerten, Prioritäten oder Entwicklungsphasen sowie Entscheidungen offener Produktfragen benötigen weiterhin die ausdrückliche Nutzerfreigabe.

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
