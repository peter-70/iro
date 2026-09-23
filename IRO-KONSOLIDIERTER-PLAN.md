# IRO – Konsolidierter Konzept- und Entwicklungsplan

**Stand:** 23. September 2026 · **Fassung:** 1.32 – Verjüngungsschutz auch bei zwei belegten Feldkonturen
**App:** IRO · **Anzeigename:** Iro · **Dateisystemname:** iro · **Namensraum:** Iro  
**Vorgesehener Ablageort:** `D:\Source\iro\IRO-KONSOLIDIERTER-PLAN.md`

## 0. Auftrag, Ergebnis und Verbindlichkeit

Dieses Dokument führt die vorhandenen Konzepte und die gegenseitigen Bewertungen von Astra und Opus zusammen. Es ist als einzige inhaltliche Arbeitsgrundlage lesbar; zum Verständnis der Anforderungen, Arbeitspakete, Parameter und offenen Entscheidungen müssen die alten Planungsdateien nicht danebenliegen. Es enthält keine neue Produktidee und keine neu erfundene technische Lösung. Die Gliederung, einheitlichen Kennungen und Zuordnung zu Arbeitsschritten dienen ausschließlich der Zusammenführung.

**Das Fundament ist ausreichend konkret, um Umgebung, Kameraversuch, Farbrechenkern und Testwerkzeug aufzubauen.** Von den ursprünglich sechs Kernentscheidungen sind K2 (beide Streifenrichtungen) und K4 (Automatiken standardmäßig aus) durch den Nutzer verbindlich entschieden. K1, K3, K5 und K6 bleiben offen. Die Positionierung nahe der Bildmitte wird als technisch begründeter Ansatz zur Vermeidung von Einschränkungen der Kamerarandbereiche weiter besprochen, nicht als bloße Bedienpräferenz verworfen.

Daher gilt: Nutzerentscheidungen und der gemeinsame technische Teil bilden den Arbeitsplan. Bei weiterhin offenen Konflikten ist keine Variante stillschweigend freigegeben. Unabhängige Arbeiten können fortgesetzt werden; zwei vollständige Produktvarianten werden nicht parallel verlangt.

### 0.1 Historische Quellen und eindeutige Kürzel

**Historische Herkunft:** Die folgende Tabelle beschreibt die Quellen der ursprünglichen Konsolidierung. Die damaligen Upload- und ZIP-Pfade sind Herkunftsangaben, keine Zusage aktuell verfügbarer Projektdateien. Diese Quellen sind keine parallelen verbindlichen Arbeitsgrundlagen. Maßgeblich sind dieser fortgeschriebene Plan und ausdrückliche Nutzerentscheidungen. Die Namensdatei ist weiterhin im Projekt vorhanden.

Astra und Opus verwendeten „Plan A“ und „Plan B“ genau gegensätzlich. Die historischen Verweise verwenden deshalb folgende Kürzel:

| Kürzel | Historische Quelle | Bedeutung bei der Konsolidierung |
|---|---|---|
| **AS** | hochgeladenes `astra.md` | Astras gegenseitige Bewertung und Korrekturvorschläge, vorrangige Quelle |
| **OP** | hochgeladenes `opus.md` | Opus’ gegenseitige Bewertung und Korrekturvorschläge, vorrangige Quelle |
| **TK** | ZIP: `iro/docs/IRO-Technisches-Konzept.md` | Technischer Ausgangsplan von Astra, Version 1.0 |
| **KP** | ZIP: `iro/KONZEPT.md` | Technischer Ausgangsplan von Opus, Version 2 |
| **TP** | ZIP: `iro/output/pdf/IRO-Technisches-Konzept.pdf` | 14-seitige Satzfassung von TK, kein dritter eigenständiger Plan |
| **UR** | ZIP: `iro/IRO-Delta-E-Vergleich.pdf` | Dreiseitiger historischer Ausgangsentwurf |
| **NA** | ZIP: `iro/Namensgebung.txt` | Namenskonvention einschließlich IPA `/ˈaɪ.roʊ/` |
| **RW** | ZIP: `iro/docs/tools/render_concept.py` | PDF-Renderer für TK mit zwei erläuternden Diagrammen, kein weiteres Konzept |

**Überlieferter Prüfstand der Konsolidierung:** Die damalige Fassung berichtete Bytegleichheit der Analysefassungen sowie die Prüfung des bildbasierten PDF-Entwurfs, der Satzfassung und des Renderers. Diese Aussagen werden hier ausschließlich historisch überliefert; sie sind keine erneute Prüfung. Insbesondere ist die Bytegleichheit anhand der aktuell verfügbaren Projektdateien nicht nachgewiesen. Die damals beschriebenen Vorschau- und Pipeline-Diagramme begründen keine zusätzlichen Produktanforderungen.

### 0.2 Statusregeln

| Kennzeichnung | Verbindlichkeit |
|---|---|
| **Nutzerentscheidung** | Ausdrücklich festgelegte Anforderung; hat Vorrang vor widersprechenden KI-Vorschlägen. |
| **Gemeinsam** | Beide vertreten den Inhalt, oder die andere Bewertung übernimmt ihn ausdrücklich. Teil des Arbeitsplans. |
| **Start / Versuch** | In den Quellen vorhandener Ausgangswert oder Implementierungskandidat; keine nachgewiesene Geräteeigenschaft. Darf erprobt, aber nicht als erwiesene Messgüte dargestellt werden. |
| **Entscheidung offen** | Widerspruch oder noch nicht ausgearbeitete Entscheidung. Kein behaupteter Konsens. |
| **Erhalten, nicht beschlossen** | Einseitiger, nicht gemeinsam angenommener Vorschlag. Bleibt dokumentiert, erweitert aber nicht automatisch den Implementierungsumfang. |
| **Nicht übernehmen** | Von mindestens einer abschließenden Bewertung ausdrücklich beanstandeter Ansatz. Wenn die andere Seite daran festhält, steht der Konflikt zusätzlich in Kapitel 13. |

Eine Empfehlung zu einem Versuch ist nicht dasselbe wie Zustimmung zum späteren Produktverhalten. Schweigen gilt nicht als Zustimmung. Ausdrückliche Nutzerentscheidungen haben Vorrang vor den Quellenbewertungen. In Fassung 1.1 bestätigt der Nutzer horizontale und vertikale Streifen sowie zwei standardmäßig ausgeschaltete Kameraautomatiken; automatisches Einregeln mit anschließendem Sperren ist kein zulässiger Ersatzablauf.

**Historischer Stand der ursprünglichen Zusammenführung:** Diese war eine Dokumentenanalyse ohne eigene Rechner-, Geräte- oder Implementierungsprüfung. Spätere Einrichtungs- und Emulatorprüfungen sind in [Entwicklungsumgebung und Prüfstand](docs/entwicklungsumgebung.md) dokumentiert; sie sind von den übernommenen Ausgangsbefunden zu unterscheiden.

### 0.3 Orientierung im Dokument

- Kapitel 1–3: Produkt, Bedienung und Architektur.
- Kapitel 4–9: Projektstart, Kamera, Bildverarbeitung, Farbrechnung und Gültigkeit.
- Kapitel 10–12: Tests, vorhandene Vergleichsversuche und Umsetzungsschritte.
- Kapitel 13–15: verbleibende Entscheidungen, kleinere offene Punkte und Ausschlüsse.
- Kapitel 16–17: Abdeckungsnachweis und überlieferte Referenzen.

## 1. Produktziel und Grenzen

**Gemeinsam:** Iro ist eine kleine, vollständig lokal arbeitende Android-App für den kamerabasierten Vergleich einer Referenzfläche mit den Farbfeldern eines handelsüblichen Musterstreifens. Alle geeigneten erkannten Felder werden gleichzeitig bewertet. Auf jedem geeigneten Feld steht unmittelbar im Kamerabild sein Farbunterschied zur Referenz. Das ähnlichste der aktuell auswertbaren Felder wird durch eine Umrandung hervorgehoben.

Der typische Einsatz ist ein Baumarkt-Farbmuster an einer gestrichenen Wand. Die App unterstützt die Auswahl; der Nutzer liest einen Farbcode selbst ab oder markiert das Feld auf dem Papier. Als Zweck bleibt auch die Vergleichshilfe beim Nachmischen erhalten. Eine Mischrezeptur entsteht nicht. Ob die Güte dafür ausreicht, ist gesondert nachzuweisen.

Gemessen werden Flächen statt einzelner Pixel, um Rauschen, Oberflächenstruktur und einzelne Störpixel zu berücksichtigen. Die Feldanzahl ist nicht fest vorgegeben. Unterschiedliche Feldhöhen, weiße Zwischenräume, Drucktext und ein abgerundeter Streifenabschluss gehören zum vorhandenen Anforderungsbild. Die im historischen Entwurf genannten Streifenmaße von ungefähr 4 × 10–12 cm und häufig vier bis fünf Feldern sind Beispiele, keine Erkennungsgrenzen.

### 1.1 Aussage des Ergebnisses

Gemeinsame Rechen- und Anzeigebasis ist **CIEDE2000, ΔE00**. Kleinere Werte bedeuten einen kleineren berechneten Farbabstand. Es gibt keine Umrechnung auf 0–10, keine Prozentanzeige und keine allgemeine Sichtbarkeitstabelle oder Farbampel.

Das Ergebnis ist ein kamerabasierter Schätzwert. Gleiche Verarbeitung im selben Bild reduziert bestimmte Unterschiede der Aufnahmebedingungen, beweist aber weder richtige absolute Abstände noch eine immer richtige Rangfolge. Eine fixierte Kameraautomatik ist nicht automatisch eine richtige Farbmessung. Ein gerundeter Nullwert bedeutet nicht, dass zwei Materialien physikalisch identisch sind. „Nächste Farbe“ bedeutet nicht „ausreichend passende Farbe“.

### 1.2 Nicht-Ziele

| Nicht enthalten | Präzisierung |
|---|---|
| Farbcodes, OCR, Katalogabgleich | Keine RAL-/NCS-/Herstellerdatenbanken, keine Codeerkennung oder Codeverwaltung |
| Ergebnisverwaltung | Keine gespeicherten Messergebnislisten, Historie oder Ergebnisexporte |
| Rezeptberechnung | Keine Mischrezepte |
| Online-System | Kein Konto, Backend, Cloud-Dienst oder KI-Dienst im Messablauf; kein dafür benötigter Netzwerkzugriff |
| Weitere App-Plattformen | Android, kein iOS; kein Blazor-Projekt |
| Absolutmessgerät | Kein zugesicherter Spektralfotometer-Ersatz und keine zugesicherte geräteübergreifende Farbgenauigkeit |
| Komplexe Oberfläche | Keine verschachtelte Menüstruktur; Vorschau, knappe Hilfen, Einstellungen und Kurzanleitung |

„Keine Persistenz“ bezieht sich auf Messergebnisse: Lokale Einstellungen bleiben erhalten. Referenzposition und Werte gehören zur Sitzung. Im normalen Betrieb werden keine Bilder dauerhaft gespeichert. Gezielt gesammelte Entwickler-Testbilder und Messprotokolle sind davon getrennt. Eine künftig beschlossene Kalibrierung wäre ebenfalls Persistenz und dürfte nicht als „speichert nichts“ beschrieben werden.

### 1.3 Umfang, der noch abgegrenzt werden muss

Ein Streifen und Hochformat bilden die gemeinsame Startbasis. Mehrere Streifen gleichzeitig und Querformat sind spätere offene Erweiterungen, keine Aufgaben der ersten Version. **Nutzerentscheidung K2:** Horizontale und vertikale Streifen werden bereits in der ersten Version unterstützt. Das Hochformat der App schränkt die Streifenrichtung nicht ein.

Wandflächen sind gemeinsamer Anwendungsfall. Stoff, Holz, Kunststoff und andere genügend gleichmäßige Flächen sind in beiden Quellen vorhanden, werden aber unterschiedlich eingeordnet: TK/AS zählen sie bereits zum vorgesehenen Einsatz, KP führt sie als Produktfrage. Diese Einordnung bleibt unter O12 erhalten. Entsprechende Materialfälle sind bereits Teil der Testplanung.

*Herkunft: TK §1, §12; KP §1–2, §3.5, §9; AS §3, §4.5, §7, §9; OP §2.4, §2.6, §3.8.*

## 2. Gemeinsamer Bedienungs- und Darstellungskern

Die gemeinsame Abfolge lautet:

1. Kurze Handhabungshinweise und Kameraberechtigung bereitstellen; Kamera-Vorschau starten.
2. Die gespeicherten Schalter für automatische Belichtung und automatischen Weißabgleich bereits beim Kamerastart anwenden; beide sind standardmäßig aus. Unterstützung und tatsächliche Wirkung prüfen.
3. Einen Streifen erkennen, geeignete Feldinneren bestimmen und die tatsächlich verwendete Referenz sichtbar markieren.
4. Geometrie, Schärfe, Belichtung, Flächenqualität und zeitliche Beständigkeit prüfen.
5. Für jedes geeignete Feld aus demselben Analyseframe den Abstand zur zugehörigen Referenz berechnen.
6. Nach Stabilisierung gültige Werte direkt auf den Feldern und eine nachvollziehbare Rangmarkierung anzeigen.
7. Ungültigkeit und Ausfälle verständlich behandeln. Alte Werte dürfen nicht wie aktuelle Live-Messungen erscheinen.

Positionierung (K1), gemeinsame/lokale Referenz (K3) und Abschluss einer möglichen Kippmessreihe (K5) bleiben offen. Beide Streifenrichtungen (K2) und die Kamerastart-Semantik (K4) sind verbindlich festgelegt.

### 2.1 Anzeige

**Gemeinsam als Startansatz:** Eine Nachkommastelle in der normalen Anzeige. Opus empfiehlt sie; Astra bevorzugt sie inzwischen ausdrücklich als zu prüfenden Startwert. Intern bleibt die Rechnung ungerundet. Zwei Nachkommastellen bleiben für Entwicklerdiagnosen und den vorgesehenen Anzeigenvergleich erhalten.

Die Vorschau trägt den kurzen Hinweis **„Farbabstand ΔE00 · kleiner = ähnlicher“**. Hervorhebungen funktionieren durch Form, Zahl und Text, nicht allein durch Farbe. Ein neutraler kontrastreicher Rahmen und ein dunkler Zahlenhintergrund sind der vorhandene lesbare Startansatz. Die alternative Schriftfarbe nach Feld-L* bleibt eine Gestaltungsvariante; sie ersetzt keinen Nachweis der Lesbarkeit auf Text, Reflexen und unruhigen Hintergründen.

„Nächste Farbe“ bezeichnet das beste gültige Feld. Sind nicht alle Felder auswertbar, lautet die vorhandene Präzisierung **„nächste erkannte Farbe“**. Sehr nahe Spitzenwerte sollen als **„ähnlich nah“** erscheinen; die dafür verwendete Grenze ist ausdrücklich noch offen (K6). Die Rundung darf weder interne Rangfolge noch Qualitätsprüfung ersetzen.

**Nutzerentscheidung – Umfang der ersten Version:** Version 1 bietet eine gut lesbare visuelle Ergebnisanzeige und zugänglich beschriftete Bedienelemente. Eine sprachgestützte Durchführung und Ausgabe der Messung ist zunächst nicht vorgesehen.

- Zahlen mit kontrastreichem Hintergrund gut lesbar darstellen.
- Das beste aktuell auswertbare Feld durch Rahmen und Text hervorheben, nicht ausschließlich durch Farbe.
- Normale Bedienelemente, insbesondere Buttons und Einstellungsschalter, verständlich beschriften und für TalkBack zugänglich machen; bei Schaltern muss auch der aktuelle Zustand erkennbar sein.
- Keine automatische Ansage laufender Messwerte und keine vollständig sprachgeführte Messung vorsehen. Eine sprachgestützte Messausgabe bleibt eine mögliche spätere Erweiterung, keine Verpflichtung für Version 1.
- Diese Grenze beschreibt ausschließlich den vereinbarten Funktionsumfang. Sie wird nicht mit Annahmen darüber begründet, wer die App benutzen sollte. Zugängliche Bedienelemente allein sind kein Nachweis einer vollständig barrierefreien Messdurchführung.

### 2.2 Referenz und Anleitung

Die benutzte Referenzfläche muss sichtbar sein und darf kein erkanntes Musterfeld überlappen. Manuelles Verschieben per Antippen wird von beiden Analysen unterstützt; ob es der Normalfall oder eine Korrektur automatischer Platzierung ist, bleibt K3. Ein gestricheltes Rechteck ist der vorhandene Vorschlag für automatisch platzierte Referenzen.

Gemeinsame Hinweise: Streifen gerade halten; möglichst gleichmäßiges Licht und vergleichbare Oberflächenausrichtung herstellen; direkte Spiegelungen und eigenen Schatten vermeiden; Referenz auf eine ausreichend gleichmäßige Fläche setzen. Vorgaben zur Bildposition und eine Kippaufforderung sind erst nach K1/K5 endgültig formulierbar.

TK sieht beim ersten Start eine kurze Anleitung vor dem Berechtigungsdialog, danach direkte Vorschau und erneuten Zugang über Einstellungen vor. KP bevorzugt direkt eingeblendete kontextbezogene Hinweise ohne Tutorial-Strecke. Beide verlangen knappe Nutzerführung; deren endgültiger Wortlaut und genaue Erststartdarstellung bleiben unter O09 erhalten.

### 2.3 Meldungen aus den vorhandenen Plänen

| Auslöser | Vorhandener Text / Bindung |
|---|---|
| Kein geeignetes Muster | „Vergleichsmuster nicht erkannt.“ Der Zusatz „Streifen an einen Bildrand halten“ gehört ausschließlich zur noch offenen Randvariante. |
| Zu wenig Bildfläche | „Muster zu klein. Bitte näher herangehen.“ |
| Ungeeignete Achsenlage | „Streifen waagerecht oder senkrecht halten.“ Beide Richtungen sind verbindlich unterstützt. |
| Ungeeignete Referenz | „Referenzbereich auf eine gleichmäßige Fläche setzen.“ |
| Überlappung mit Muster | „Referenzbereich auf die Vergleichsfläche setzen.“ |
| Unbrauchbare Helligkeit / Reflexe | „Messung nicht möglich. Licht und Ausrichtung prüfen.“ |
| Instabilität | „Bitte kurz ruhig halten.“ |
| Kamerafunktion fehlt | „Diese Kamera unterstützt die gewählte Einstellung nicht.“ |

Technische Details wie ISO, YUV-Matrix und Schwellenwerte gehören in die Entwicklungsdiagnose, nicht in den normalen Messablauf.

*Herkunft: TK §1, §7–9; KP §2, §3.9, §4; AS §5.6, §6, §8 V5; OP §2.3–2.4, §3.1, §3.8.*

## 3. Architektur und Datenverträge

**Gemeinsam:** .NET MAUI, C# und XAML; Camera2 direkt über .NET-Android-Bindings (`Android.Hardware.Camera2`); eigener MAUI-Handler für eine native Vorschau; geräteunabhängiger C#-Kern. Kotlin ist kein zusätzliches Projekt. Die Bildanalyse läuft außerhalb des UI-Threads.

| Ablage / Baustein | Verantwortung |
|---|---|
| `src/iro.app` / `Iro.App` | MAUI-Oberfläche, kleines ViewModel, Vorschau, Overlay, Sitzungssteuerung, lokale Einstellungen und Berechtigungen |
| `src/iro.app/Platforms/Android/Camera` | Camera2-Session, Eigenschaften, Capture-Parameter, Metadaten, Framezugriff und Lebenszyklus |
| `src/iro.core` / `Iro.Core` | Bildpuffer, Geometrie, Konvertierung, ΔE, Erkennung, Flächenstatistik, Qualitätsregeln und zeitliche Stabilisierung |
| `tests/iro.core.tests` / `Iro.Core.Tests` | xUnit: Rechenreferenzen, Konverter, Erkennung, Geometrie, Zustände und Zeitverhalten |
| `tests/fixtures` | Kleiner reproduzierbarer Pflichtsatz und ausgewählte echte Aufnahmen |
| `tests/datasets` | Versionierte Testdatensätze samt Sollverhalten und Herkunft, getrennt für Entwicklung und unabhängige Abnahme |
| `tests/runs` | Unveränderte Ergebnisse und Diagnosepakete je Testlauf |
| `tests/adjustments` | Probleme, Feinjustierungen und verknüpfte Vorher-/Nachher-Auswertungen |
| `iro-gen` / `IroGen` | WPF-Testbildgenerator mit Optionen, Vorschau und PNG-/JSON-Export; eigene `IroGen.slnx`, zusätzlich in `iro.slnx` eingebunden |
| `tests/iro.gen.tests` / `IroGen.Tests` | Windows-Tests für Generator-Farbmathematik, Rendering, Export und WPF-Vorschau |
| `tools/iro.testgen` / `Iro.TestGen` | Historisches Konsolengerüst; keine implementierte Erzeugung oder Auswertung. Aktuelle Bildgenerierung erfolgt mit IroGen. |
| `docs` | Später entstehende Einrichtungs- und Geräteprotokolle, Messnachweise und begründete Entscheidungen |
| `iro.slnx`, `global.json` | Solution und festgeschriebener SDK-Stand |
| `IRO-KONSOLIDIERTER-PLAN.md` | Dieses Dokument als einzige konzeptionelle Quelle |

Die kleingeschriebenen Verzeichnisnamen übernehmen TK und die ausdrückliche Empfehlung OP §2.7; Projektdateien und Namensräume bleiben `Iro.*`. Der Generator aus KP wird in dieselbe Namenskonvention eingefügt. Nach der Bereinigung genügt zunächst diese Planungsdatei; Projektverzeichnisse entstehen erst bei der Umsetzung.

### 3.1 Plattformgrenze und Vorschau

Der Handler hostet eine native `TextureView` oder `SurfaceView`; die konkrete Wahl ist ein vorhandenes Implementierungsdetail. Die Vorschau geht an die native View, Analyseframes an den Kern. Eine darüberliegende MAUI-`GraphicsView` zeichnet Werte, Referenzrahmen und Gewinner-Markierung.

Android-Bitmaps und native Sessionobjekte bleiben im Plattformteil. Der Kern verwendet schlanke definierte Puffer mit Breite, Höhe, Stride und Pixeldaten sowie numerische Typen. Er ist ohne Gerät testbar. Das Overlay erhält Ergebnisse und liest keine Farben aus dem dargestellten Bildschirm zurück.

CameraX/`Camera2Interop` bleibt die schon genannte Alternative nur bei belegtem Vorteil des Prototyps; keine parallele Pflege zweier Kameraanbindungen. Start mit konventioneller Bildverarbeitung ohne zusätzliche native Bildbibliothek. Ein dauerhafter Ausschluss von OpenCV oder anderen Bibliotheken wird nicht als gemeinsame Vorgabe behandelt; spätere Änderungen benötigen einen konkret gemessenen Grund. Der eigene Handler hängt nicht von einer unbelegten pauschalen Aussage über alle Versionen der Toolkit-`CameraView` ab.

### 3.2 Schnittstellen

| Vertrag | Inhalt |
|---|---|
| `ICameraSession` | Fähigkeiten lesen, starten/stoppen, Einstellungen anwenden, Frames liefern |
| `IFrameConverter` | Puffer einschließlich Strides, Zuschnitt und Farbrauminformation in einen definierten RGB-Arbeitsraum überführen |
| `ISwatchDetector` | Geordnete Feldpolygone und Erkennungsgüte aus einem Frame liefern |
| `MeasurementEngine` | Referenz und Feldinneres bewerten, ΔE00 berechnen, gültige Ergebnisse ordnen |
| `MeasurementSnapshot` | Messgeneration, Frame-ID, Aufnahmezeitstempel und Zeitbasis, monotone erste Bildempfangszeit, Transformationsversion, Referenzgeometrie, Feld-IDs, Werte, Qualitätszustände und verwendete Kameraparameter zusammenhalten |

Ein Frame-Ergebnis ist unveränderlich und einem Aufnahmezeitpunkt zugeordnet. Zeitliche Glättung wird daraus abgeleitet; sie darf die Herkunft eines aktuellen Live-Werts nicht verschleiern. Austauschbare statistische und zeitliche Strategien sind vorhandene Testkandidaten, keine Formel- oder Expertenauswahl im Endanwenderbildschirm.

### 3.3 Puffer, Leistung und Lebenszyklus

- Neuesten verfügbaren Frame verarbeiten; keine Warteschlange alter Analysebilder aufbauen.
- `ImageReader`-Bilder auch bei Fehlern zeitnah schließen. Pufferbesitz und Freigabe eindeutig regeln.
- Wiederverwendete Puffer und `Span<T>`-Verarbeitung einsetzen, Allokationen messen. „Keine Allokationen“ und „C# ist schnell genug“ sind Ziele beziehungsweise zu prüfende Aussagen, keine vorliegenden Messergebnisse.
- Vorschau und Overlay flüssig halten; Zahlen dürfen zwischen Analysen nur innerhalb der Gültigkeitsregeln stehen bleiben.
- Im Hintergrund Kamera freigeben. Bei Rückkehr, Kamera-Neustart, relevanter Einstellung, Referenzwechsel und unklarer Feldzuordnung Messhistorien verwerfen.
- 1280 × 720 als verfügbarkeitsabhängiger Start für den Analyse-/Messstrom; etwa fünf Auswertungen pro Sekunde. Die tatsächliche Streamkombination protokollieren.
- Niedriger aufgelöste Suche, beispielsweise etwa 640 Pixel Breite, ist eine vorhandene Optimierungsvariante; die Farbmessflächen können aus demselben höher aufgelösten Frame stammen. Keine zeitlich oder geometrisch unverbundenen Bilder kombinieren.

Das Ziel von weniger als 300 ms gilt für den Weg vom ausgewerteten Frame zur sichtbaren Ausgabe. Es ist **keine** Zusage über die gesamte Reaktion nach einem Farbwechsel: Ein Fünferfenster umfasst bei fünf Analysen/s etwa 0,8 Sekunden; Rangbestätigung verzögert zusätzlich. Erstausgabe, Alter der Anzeige und sichtbarer Rangwechsel sind getrennt zu messen. Das alternative Ziel von 5–10 Analysen/s bleibt ein Leistungsversuch, keine erhöhte Abnahmepflicht.

*Herkunft: TK §3–4, §6, §9; KP §7; AS §5.5, §6, §7, §9; OP §3.1–3.3, §3.6, §6.*

## 4. Entwicklungsumgebung und reproduzierbarer Start

### 4.1 Übernommener Befund, noch kein hier geprüfter Rechnerstand

TK und KP berichten folgende Ausgangslage vom 18. September 2026:

| Bestandteil | Überlieferter Stand / notwendige Prüfung |
|---|---|
| Arbeitsordner | `D:\Source\iro`, noch kein App-Projekt; laut KP noch kein initialisiertes Git-Repository |
| SDK | .NET SDK `10.0.401`; KP nennt zusätzlich `9.0.307` und `10.0.102` |
| MAUI | SDK-Pack unter anderem `10.0.20`; vor Projektstart mit zusammenpassendem unterstütztem Stand abgleichen |
| Workloads | `android`, `maui-windows`, `ios`, `maccatalyst` über Visual Studio; KP nennt Android-Workload `36.1.69` |
| Android SDK | TK nennt installierte Plattformen 34/35/36; KP hatte SDK-Pfade noch nicht verifiziert |
| Java | TK fand im geprüften Verzeichnis `jdk-17.0.14`; weiterer Pfad oder Bereitstellung von JDK 21 zu prüfen |
| ADB | Laut TK im SDK vorhanden, nicht im geprüften PATH |
| Gerät | Laut TK Motorola vorhanden, Modell unbekannt; OP verlangt Bestätigung dieser Angabe. Samsung als spätere zweite Gerätefamilie vorgesehen. |
| IDE | Vorhandene Visual-Studio-Installation; .NET-10-Unterstützung prüfen. VS Code mit MAUI-Erweiterung als genannte Alternative. |

Nicht allein wegen einer fehlenden Workload-ID `maui-android` nachinstallieren. Maßgeblich sind passende Komponenten und ein erfolgreicher Android-Build. Bestehende Visual-Studio-Installation über ihren Installer pflegen.

**Übernommene Startbasis:** .NET 10 / MAUI 10, `net10.0-android`, Core und Tests `net10.0`, Nullable-Prüfung, JDK 21 und Android API 36 für den Build. API 24 ist nur die vorläufige Installationsuntergrenze; ihre Eignung bleibt zu prüfen. Keine Android-Version ersetzt die einzelne Kamerafähigkeitsprüfung.

TK nennt in seiner damaligen Supportbetrachtung MAUI-Patch `10.0.101` und den 11. Mai 2027 als Supportende. Diese Angaben werden hier zur Vollständigkeit überliefert, nicht erneut als aktueller Installationsbefehl bestätigt. Bei M0 einen kompatiblen unterstützten Gesamtstand prüfen und dokumentieren.

### 4.2 Einrichtungsschritte

Die folgenden PowerShell-Befehle sind ein Arbeitsplan aus TK, ergänzt um den von beiden übernommenen Generator und die dort geforderte Git-Initialisierung. Sie wurden für diese Zusammenführung nicht auf Windows ausgeführt. Nur in einem bereinigten Arbeitsverzeichnis ohne bereits angelegte gleichnamige Projekte ausführen; kein `--force` zum Überschreiben verwenden.

```powershell
Set-Location 'D:\Source\iro'
dotnet --info
dotnet workload list
dotnet new list maui

git init
dotnet new gitignore
dotnet new globaljson --sdk-version 10.0.401 --roll-forward latestPatch
dotnet new sln -n iro
dotnet new maui -n Iro.App -o src/iro.app --no-restore
dotnet new classlib -n Iro.Core -o src/iro.core -f net10.0 --no-restore
dotnet new xunit -n Iro.Core.Tests -o tests/iro.core.tests `
  -f net10.0 --no-restore
dotnet new console -n Iro.TestGen -o tools/iro.testgen -f net10.0 --no-restore
```

Bei zwischenzeitlich anderem geprüftem SDK dessen Version verwenden. In `global.json` zusätzlich `allowPrerelease: false` setzen. SDK-Pinning allein fixiert weder Workloads noch NuGet-Pakete. Nach erfolgreichem Erstbuild Paketstände/Lock-Dateien und Workloadinventar festhalten. `bin`, `obj`, Benutzerdateien und lokale SDK-Pfade nicht einchecken.

Das MAUI-Template auf ausschließlich Android beschränken: alle zusätzlichen `TargetFrameworks`-Erweiterungen und nicht passenden plattformspezifischen Bedingungen entfernen, benötigte Ressourcen und MAUI-Paketreferenzen erhalten.

```xml
<TargetFramework>net10.0-android</TargetFramework>
<SupportedOSPlatformVersion>24.0</SupportedOSPlatformVersion>
<ApplicationTitle>Iro</ApplicationTitle>
<ApplicationId>de.example.iro</ApplicationId>
```

`de.example.iro` ist nur die vorhandene Entwicklungskennung. Mindest-API und endgültige Kennung bleiben offen. Unter Android Kameraberechtigung und erforderliche Rückkamera deklarieren, Berechtigung zur Laufzeit anfordern. Der geplante Ablauf benötigt keine Mikrofon-, Standort- oder Speicherberechtigung.

```powershell
dotnet sln iro.slnx add src/iro.app/Iro.App.csproj `
  src/iro.core/Iro.Core.csproj `
  tests/iro.core.tests/Iro.Core.Tests.csproj `
  tools/iro.testgen/Iro.TestGen.csproj

dotnet add src/iro.app/Iro.App.csproj reference src/iro.core/Iro.Core.csproj
dotnet add tests/iro.core.tests/Iro.Core.Tests.csproj `
  reference src/iro.core/Iro.Core.csproj
dotnet add tools/iro.testgen/Iro.TestGen.csproj `
  reference src/iro.core/Iro.Core.csproj

dotnet workload restore src/iro.app/Iro.App.csproj
dotnet restore iro.slnx
dotnet build src/iro.app/Iro.App.csproj -f net10.0-android
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj
& 'C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe' devices
```

Der ADB-Pfad stammt aus TK und ist lokal zu bestätigen. Die `.slnx`-Ausgabe wurde laut OP mit dem dort installierten SDK geprüft. Workload-Restore kann fehlende Komponenten ergänzen; Installationsprobleme über den zuständigen Installer beheben. Template-Demo entfernen und leere Tests durch echte Referenztests ersetzen.

Motorola beziehungsweise bestätigtes Testgerät per USB verbinden, USB-Debugging freigeben, als IDE-Startziel wählen. Abnahme: Build, Tests, App-Start, Debugger und bedienbarer Kameraberechtigungsdialog. Ein Emulator genügt für Oberfläche und bestimmte Lebenszyklustests; Kamera- und Farbprüfung erfolgen am echten Gerät.

*Herkunft: TK §2–3; KP §7.5–8; AS §4.6, §6; OP §2.7, §3.6–3.7, §5.1, §7.*

## 5. Kamera und Aufnahmebedingungen

### 5.1 Gemeinsame Prüfung

Rückseitige Hauptkamera, zunächst kein digitaler Zoom und kein bewusster Objektivwechsel. Kamera-ID, Sensorausrichtung, Hardware-Level, Ausgabeformate, Streamkombinationen, Auflösungen, manuelle Fähigkeiten, unterstützte Lock-/AF-Modi und Parameterbereiche über `CameraCharacteristics` erfassen. Nicht aus Marke, Android-Version oder allein `LEGACY` auf Eignung schließen.

`CaptureResult` liefert den Gegencheck zu angeforderten Einstellungen: Belichtungszeit, ISO, Weißabgleichszustand, Farbkorrektur, Fokusinformationen soweit vorhanden und Zeitstempel je analysiertem Frame protokollieren. Auch bei verändertem Bildinhalt prüfen, ob als fest gewählte Werte tatsächlich fest bleiben.

Benötigte Fähigkeiten fehlen → betroffenen Messmodus sperren und verständlich informieren. Kein stiller Rückfall auf eine andere Automatik. Ein Warnschild allein macht einen nicht unterstützten Modus nicht funktionsfähig. Eine verfügbare Alternative muss ausdrücklich gewählt werden.

**Verbindlicher Bedienweg bei nicht unterstützten Kameraeinstellungen:**

1. Bereits beim Kamerastart die gemeldeten Fähigkeiten prüfen. Zeigt erst die Gegenprüfung der tatsächlich angewendeten Einstellungen ein Problem, denselben Bedienweg aus der laufenden Messung anbieten und betroffene Messwerte sperren.
2. Die Meldung benennt die konkrete nicht unterstützte oder nicht zuverlässig angewendete Einstellung, beispielsweise: „Diese Kamera unterstützt keinen festen Weißabgleich in der gewählten Kombination.“ Eine verfügbare Alternative darf mit Begründung vorgeschlagen werden; sie wird nicht automatisch aktiviert.
3. Der Button **„Kameraeinstellungen öffnen“** führt zu den Einstellungen innerhalb der App. Dort kann der Nutzer die betreffende Automatik bewusst einschalten. Beide Automatiken bleiben im Auslieferungszustand aus; ohne ausdrückliche Auswahl erfolgt keine Umschaltung.
4. Nach der Änderung zur Messansicht zurückführen, die gewählte Kombination prüfen und erst bei erfolgreicher Prüfung wieder Messwerte freigeben. Die bewusste Auswahl lokal für die betreffende Kamera speichern. Bei erneutem Start die Unterstützung prüfen, eine weiterhin geeignete gespeicherte Auswahl aber nicht erneut bestätigen lassen. Einstellungswechsel unterliegen den Regeln für neue Messgenerationen und geleerte Historien.
5. Ist insgesamt kein nutzbarer Messbetrieb möglich, die fehlende Voraussetzung verständlich benennen und erklären, dass mit dieser Kamera keine Messung möglich ist. Eine einzelne nicht unterstützte AE-/AWB-Einstellung bedeutet noch keine allgemeine Geräteuntauglichkeit.

### 5.2 Verbindliche Kameraautomatiken

**Nutzerentscheidung K4:** Zwei getrennte Schalter „Automatische Belichtung“ und „Automatischer Weißabgleich“, jeweils standardmäßig aus und lokal gespeichert. Die Einstellungen gelten bereits beim Kamerastart. Bei ausgeschaltetem Schalter wird die entsprechende Automatik nicht zunächst zum Einregeln aktiviert. Bei eingeschaltetem Schalter bleibt sie aktiv und wird nicht anschließend automatisch gesperrt.

| AE-Automatik | AWB-Automatik | Verbindliches Verhalten, am Gerät zu prüfen |
|---|---|---|
| Aus | Aus | Feste Belichtung, fester Weißabgleich |
| Ein | Aus | Nachregelnde Belichtung, fester Weißabgleich |
| Aus | Ein | Feste Belichtung, automatischer Weißabgleich, soweit Kombination funktioniert |
| Ein | Ein | Beide Automatiken aktiv |

`AE_MODE_OFF` braucht eigene Belichtungsparameter. `AWB_MODE_OFF` braucht passende Farbkorrekturparameter. Als Versuch nennt TK ISO 100 und 1/100 s innerhalb der unterstützten Bereiche; beim Weißabgleich zunächst einen unterstützten festen Tageslichtmodus, andernfalls überprüfte gerätespezifische Gains/Farbkorrektur. Diese Startwerte sind weder universell innenraumtauglich noch eine Kalibrierung. Tageslichtmodus und echte manuelle AWB-Gains sind getrennte technische Wege.

**Nicht übernehmen:** Erst automatisch einregeln lassen und anschließend AE/AWB sperren. Dieser frühere KP/OP-Vorschlag widerspricht der bestätigten Schaltersemantik und ist keine offene Produktalternative mehr.

Für echten Handbetrieb nennen die Analysen insbesondere `MANUAL_SENSOR` und `MANUAL_POST_PROCESSING`; die benötigten Fähigkeiten und tatsächlich angewendeten Parameter sind einzeln zu prüfen. Offen bleiben geeignete feste Werte und das Geräteverhalten (O18), nicht die Schaltersemantik. Fehlende Unterstützung führt zur Sperre des betroffenen Messmodus, nicht zu versteckter Einregelung.

### 5.3 Fokus und Kamera-Bildaufbereitung

Gemeinsam erforderlich ist die frühe Prüfung von Fokus, AE und AWB im Zusammenspiel. TK wollte zunächst Autofokus; KP wollte nach Fokussierung AF ausschalten. AS präzisiert: `AF_MODE_OFF` allein garantiert keinen gehaltenen Fokus. Passende Fokusdistanz oder unterstützten Lock-Zustand prüfen; auch AF bei ausgeschalteter AE ist geräteabhängig zu erproben. Schärfeverlust sperrt betroffene Messungen. Die konkrete Strategie wird nach dem Kameraversuch dokumentiert.

Gemeinsam ist das Vermeiden steuerbarer Effekte, Szenenmodi, HDR/Multiframe-Verarbeitung und automatischer Objektivwechsel. KP/OP nennen zusätzlich `CONTROL_EFFECT_MODE = OFF`, `CONTROL_SCENE_MODE = DISABLED` sowie `NOISE_REDUCTION_MODE` und `EDGE_MODE` möglichst `OFF`, ersatzweise `FAST`. AS bestätigt keine bestimmte NR-/Edge-Konfiguration als gemeinsame Vorgabe: Diese Werte bleiben unter O03 als erhaltene Kameraversuchskandidaten, nicht als garantierter Weg zu unverfälschten Farben.

### 5.4 Messgeometrie und Materialeinflüsse

Zu untersuchen sind Referenzabstand, Position im Bild, Objektivabschattung/Vignettierung, Beleuchtungsgradienten, Schatten durch Hand und Papierkante, unterschiedliche Reflexion, Blickwinkel und Struktur. Weder räumliche Nähe noch Bildmitte ist bereits als überlegen nachgewiesen. Die von OP genannte typische Größenordnung von 20–40 % Randabfall ist keine Messung am vorgesehenen Gerät und wird nicht zur Projektanforderung oder Fehlerkorrekturkonstante.

Die Referenz darf selbst glänzen, das Muster kann verschattet sein. Darum gilt kein allgemeiner Schluss „hellere Wandprobe ist richtig“ oder „dunklere Musterprobe ist Körperfarbe“. Unterschiedliche Materialien und Winkel werden als reale Störfälle berücksichtigt. Ein Kippverfahren bleibt K5, eine Flatfield-/`LENS_SHADING_MAP`-Korrektur bleibt O04.

*Herkunft: TK §5; KP §3.1, §3.4, §3.7, §9; AS §4.2–4.4, §4.6, §4.8, §5.7, §6; OP §2.1–2.2, §3.3, §4.1–4.2, §5–6.*

## 6. Bildpuffer, Farbraum und Koordinaten

### 6.1 Frame und Konverter

Vorschau und Analyse stammen aus derselben Kamerasitzung. Feld und Referenz eines Vergleichs stammen zwingend aus demselben Analyseframe. Eine einzige Auswertung darf keine getrennt ausgesuchten Farben verschiedener Aufnahmezeitpunkte zusammenstellen.

Startformat ist `YUV_420_888` über `ImageReader`. Y-, U- und V-Ebenen besitzen eigene `RowStride`/`PixelStride`; Zuschnitt, Puffergrenzen und UV-Reihenfolge sind zu beachten. Kein pauschales dicht gepacktes NV21 voraussetzen. YUV-Frames sind keine Sensor-RAW-Daten und umgehen die Kamera-Bildaufbereitung nicht.

Für den RGB-Arbeitsraum YUV-Matrix, Wertebereich und Übertragungsfunktion dokumentieren. Vorhandene Datenraum-Metadaten auswerten, fehlende Angaben als Annahmen kenntlich machen. BT.601 gegenüber BT.709 und voller gegenüber begrenztem Wertebereich mit Fixtures und echten Aufnahmen prüfen. KP nennt BT.601/full range als Startannahme; daraus wird keine vorab abgenommene gemeinsame Standardkonvertierung.

Die übrige Pipeline erwartet eindeutig gekennzeichnetes sRGB. Entwicklerdiagnosen dürfen offene Annahmen untersuchen, eine abgenommene Messpipeline darf sie nicht als geprüft ausgeben. Gleiche Konvertierung für beide Flächen hebt farbabhängige Fehler nicht automatisch auf. Auch linearisiertes sRGB bleibt eine Interpretation bereits aufbereiteter Kameradaten.

### 6.1.1 Verbindliche Zuordnung von Bild und Aufnahmeinformationen

1. **Exakte Paarung:** Bildzeitstempel (`Image.Timestamp`) und `SENSOR_TIMESTAMP` der Aufnahmeinformationen müssen innerhalb derselben Kamerasitzung übereinstimmen. Die Zuordnung verwendet Sitzung und Aufnahmezeitstempel; weder Ankunftsreihenfolge noch die zuletzt empfangenen Metadaten ersetzen diese Paarung. Der Analyseausgang ist so zu konfigurieren, dass der Bildzeitstempel dem Sensor-Aufnahmezeitstempel entspricht. [Android: SENSOR_TIMESTAMP](https://developer.android.com/reference/android/hardware/camera2/CaptureResult#SENSOR_TIMESTAMP)
2. **Begrenzt zwischenpuffern:** Unabhängig von der Ankunftsreihenfolge kurz auf das Gegenstück warten. Ohne vollständige Zuordnung samt benötigten Angaben keine freigegebene Messung. Wartezeit und Puffergröße begrenzen; konkrete Grenzen während der Implementierung festlegen. Bei Ablauf oder Verdrängung betroffene Daten verwerfen und Bildressourcen freigeben. Bei Sitzungswechsel Puffer leeren und verspätete Rückmeldungen der alten Sitzung verwerfen.
3. **Zeitbasis unterscheiden:** Bei `SENSOR_INFO_TIMESTAMP_SOURCE_REALTIME` lässt sich das Aufnahmealter mit `SystemClock.elapsedRealtimeNanos()` bestimmen. Andernfalls Wartezeiten über eine separat erfasste monotone Empfangszeit begrenzen. **Diese misst ausschließlich die Zeit seit Empfang, nicht das tatsächliche Aufnahmealter.** Beide Größen in Datenmodell und Diagnose unterscheiden; Empfangszeit nicht als Aufnahmezeit ausgeben. [Android: SENSOR_INFO_TIMESTAMP_SOURCE](https://developer.android.com/reference/android/hardware/camera2/CameraCharacteristics#SENSOR_INFO_TIMESTAMP_SOURCE)

**Festgelegt durch Nutzerentscheidung zu Prüfpunkt 2.** Die Kameraimplementierung in M1 und die Puffertests in M2 müssen diese Regeln erfüllen.

### 6.2 Eine gemeinsame Transformation

Analyse-/Sensorbild und Vorschau werden durch eine gemeinsame versionierte Transformation verbunden: Rotation, Zuschnitt, Seitenverhältnis, Skalierung und gegebenenfalls Spiegelung. Eine einfache proportionale Umrechnung allein anhand der Bildbreite genügt nicht.

Feldpolygone werden zur Vorschau transformiert, Referenztipps in Gegenrichtung. Overlay und Messwert gehören zur gleichen Transformationsversion. Nach Größenänderung und Kameraneustart Geometrien neu aufbauen. Rotation, Ausschnitt, Referenztipp und Overlay müssen in Tests dieselben Flächen treffen.

TK sieht für die manuelle Variante einen an der Bildschirmposition verbleibenden Referenzrahmen vor, kein Tracking derselben Wandstelle. KP bindet die Referenz an den Streifen. Das ist Bestandteil von K3, keine schon beschlossene gemeinsame Tracking-Lösung. **Nutzerentscheidung zu Prüfpunkt 6:** Bewegung allein setzt die Stabilisierung nicht zurück und sperrt keine Messung. Entscheidend sind die Auswertbarkeit des aktuellen Bildes und die eindeutige Zuordnung der Messflächen. Ein tatsächlicher Referenzwechsel oder eine nicht mehr gesicherte Zuordnung darf keine alten Messhistorien weiterverwenden; ein neues geeignetes Bild bleibt auswertbar.

### 6.3 Feldzuordnung

Erkannte Felder erneut bestimmen und über Überlappung, Reihenfolge und Abstand zeitlich zuordnen. Unklare Zuordnung → neue Feld-ID und neue Historie. Keine Vermischung verschiedener Felder in einem Glättungsfenster. Handzittern oder Bewegung sind für sich keine Ablehnungsgründe. Zahlen werden bei unbrauchbarer Bildqualität, verlorener Erkennung oder unklarer Zuordnung entfernt, nicht aufgrund der Bewegung allein. Für Rangfolgen sind zeitlich vergleichbare gültige Daten nötig; AS weist ausdrücklich auf gemeinsame Zeitfenster bei unterschiedlichen Feldhistorien hin.

*Herkunft: TK §4, §6; KP §3.2, §7.1–7.2; AS §4.1, §6, §9; OP §3.2–3.3, §3.8, §6.*

### 6.4 Verbindliche Bildoptimierung und gemeinsame Messgrundlage

**Nutzerentscheidung vom 22. September 2026, Fassung 1.27:** Das [vollständige Entscheidungsprotokoll zur Bildoptimierung](docs/entscheidung-bildoptimierung-2026-09-22.md) ist ausdrücklich angenommen und als unveränderte Quelle abgelegt. Die folgenden Regeln übernehmen es in die aktuelle Arbeitsgrundlage. Bei betroffenen älteren Vorgaben gilt diese neuere Entscheidung. Der [Arbeitsplan mit Abnahmen in Klartext](docs/arbeitsplan-bildoptimierung.md) konkretisiert die Umsetzung; er ist kein paralleles Konzept.

**Messprinzip:** Wand und Farbstreifen sind eine gemeinsame fotografische Messeinheit. Beide müssen aus demselben Frame und derselben photometrischen Bildgrundlage stammen. Gleiche Verarbeitung beweist nicht automatisch gleiche reale Beleuchtung oder richtige Farbabstände. Lokalisierte Regionen dürfen unterschiedliche Pixel enthalten, aber keine unabhängig bestimmten Farbtransformationen erhalten.

**Zwei Pfade:** Eine temporäre Erkennungskopie darf für Streifen, Felder, Konturen, Drehung, Perspektive, Messmasken und unterstützende Qualitätsanalyse optimiert werden. Ihre bearbeiteten Farbwerte dürfen niemals in das endgültige Messergebnis eingehen. Koordinaten sind nachvollziehbar auf das gemeinsame Messbild zurückzuführen. Die Eignung der Messinformation muss an der unverfälschten Aufnahme geprüft bleiben; verbesserte Kanten oder Helligkeit der Erkennungskopie sind kein Nachweis wiederhergestellter Messfähigkeit.

**Gemeinsames Messbild:** Ausgangspunkt ist die Originalaufnahme im definierten Farbraum. Eine zulässige farbwirksame Korrektur braucht ein gemeinsames, reproduzierbares, dokumentiertes Modell mit identischen Transformationsregeln für das gesamte Bild. Für Wand und Streifen unterschiedliche Helligkeits-, Kontrast-, Gamma-, Weißabgleich- oder Farbkorrekturen sind verboten. Messpfad-Korrekturen sind nur nach den unten genannten Nachweisen zulässig; die Entscheidung aktiviert noch kein ungetestetes Verfahren.

| Verfahren | Verbindliche Festlegung |
|---|---|
| Geraderichten | Zulässig, früh vor der Farbfeldsuche. Innere Messflächen mit ausreichendem Abstand zu Kanten und interpolierten Randbereichen verwenden; die bisherige Messung auf Originalpixeln erfüllt den gewählten technischen Ansatz. |
| Perspektivkorrektur | Moderate Entzerrung ist zulässig; starke Perspektive ablehnen. Entzerrung stellt weder verlorene Farbinformation noch gleiche Beleuchtungswinkel her. Grenzen durch Tests bestimmen. |
| Globale Aufhellung | In der Erkennungskopie zulässig. Im Messpfad nur bildweit identisch, ohne abgeschnittene Kanäle, mit festgelegtem und getestetem Modell sowie nachgewiesener Verbesserung. Keine getrennte Aufhellung. |
| Weißabgleich und Farbkalibrierung | Nur gemeinsames Modell; eine aus zuverlässig bekannten Referenzfarben abgeleitete Transformation gilt gleichermaßen für Wand und Streifen. Schatten, Reflexe, ungleichmäßiges Licht und Clipping auf Referenzfeldern berücksichtigen. Keine unabhängige Neutralisierung. |
| Kontrastverstärkung | Nur Erkennungskopie; deren Pixel dürfen nicht gemessen werden. Lokale Kontrastverfahren einschließlich CLAHE und lokaler HDR-Anpassungen sind im Messpfad ausgeschlossen. |
| Entrauschen | Robuste Statistik aus vielen inneren Pixeln bevorzugen. Ein Filter muss bildweit identisch, konservativ und ohne Vermischung über Kanten arbeiten; Nutzen durch Vergleichstests belegen. Regionale Pixelauswahl ist keine Erlaubnis für unterschiedliche Bildfilter. |
| Nachschärfen | Höchstens Erkennungskopie. Geschärfte Pixel nicht messen; Schärfung hebt Unschärfesperren nicht auf. |
| Starke Bewegungs- oder Fokusunschärfe | Nicht rekonstruieren; Aufnahme ablehnen beziehungsweise besseren Kameraframe verwenden. Keine KI-erfundenen Farben. |
| Messrelevantes Clipping | Aufnahme ablehnen; nachträgliches Abdunkeln darf Informationsverlust nicht kaschieren. Relevanz durch Tests bestimmen, nicht jeden hellen Pixel als Clipping behandeln. |
| Unterbelichtung | Leicht dunkle Bilder untersuchen; Korrektur nur über gemeinsames validiertes Modell. Starke Unterbelichtung ablehnen; Grenze durch Tests bestimmen. |
| Schatten und ungleichmäßiges Licht | Vergleichbare Beleuchtung von Wand und Streifen prüfen. Deutliche Unterschiede erfordern Warnung oder Ablehnung; konkrete Zuordnung ist noch festzulegen. Lokale Schattenaufhellung ist verboten. |
| Glanz und Reflexe | Kleine Glanzstellen robust ausschließen. Bei wesentlicher Betroffenheit Aufnahme ablehnen oder andere geeignete Messfläche verwenden. Keine verdeckte Farbe rekonstruieren. |

**Ausdrückliche Verbote:** Wand allein aufhellen; Streifen allein weißabgleichen; beide unabhängig normalisieren; eigene Histogramm-/Kontrastanpassung je Bereich; Referenzfarben auf Sollwerte setzen ohne dieselbe abgeleitete Transformation auf die Wand; unterschiedliche Entrauschungs-/Schärfungsverfahren; rekonstruierte unscharfe oder überbelichtete Farben messen; optische Verbesserung ohne farbmetrischen Nachweis als Messgrundlage verwenden.

**Bevorzugter Aufnahmeablauf:** Schlechte Aufnahmen früh erkennen und ersetzen. Vor Messfreigabe vollständige Sichtbarkeit des Streifens, zulässige Perspektive, ausreichende Schärfe, fehlendes relevantes Clipping, genügend Licht, vergleichbare Beleuchtung, fehlende starke Schatten/Reflexe, hinreichende kurzfristige Ruhe und geeignete Messflächen prüfen. Bewegung bleibt anhand ihrer Auswirkungen auf Qualität und Zuordnung zu beurteilen; keine willkürliche starre Stillhaltezeit ergänzen. Die neue Forderung nach vollständiger Sichtbarkeit ersetzt die frühere Freigabe angeschnittener Streifen. Sie verlangt keine fest vorgegebene Feldanzahl; die technische Erkennbarkeit der Vollständigkeit muss nachgewiesen werden. Teilmessungen vollständig sichtbarer Streifen bleiben nur zulässig, soweit keine aufnahmeweite Sperre dieses Protokolls greift. Messrelevantes Clipping, starke Unterbelichtung, starke Unschärfe und starke Perspektive sind aufnahmeweite Sperren.

**Mehrere Frames:** Nur geeignete, eindeutig zugeordnete Frames verwenden. Die bestehende Paarung innerhalb jedes Frames und anschließende robuste Aggregation bleibt verbindlich. Keine unabhängig ausgewählten Wand-/Streifenfarben aus verschiedenen Zeiten. Ein neuer pixelbasierter Mehrframe-Schätzer benötigt einen eigenen Nachweis; die Erlaubnis zur gemeinsamen Auswertung ist keine automatische Freigabe beliebiger Mittelung.

**Noch durch Arbeit und Tests festzulegen:** Numerische Qualitätsgrenzen, Erkennbarkeit vollständiger Streifen, Schattenwarnung gegenüber Ablehnung, konkrete Korrekturmodelle und ihr Geltungsbereich. Eine bekannte Referenzpalette darf nicht aus nominalen Test-Sollwerten heimlich in den Analyzer gelangen. Die Entscheidung beschließt keine Farbkatalogfunktion, keine Endanwender-Kalibrieroberfläche und keine geänderte AE-/AWB-Schaltersemantik. Reale Kamera- und Genauigkeitsnachweise folgen weiterhin erst nach technischer Gesamtabnahme.

## 7. Streifen erkennen und Messflächen bestimmen

### 7.1 Gemeinsamer erster Algorithmus

Der konkrete Profil-/Gradientenansatz aus KP wird mit den Geometrie-, Gültigkeits- und Fehlerregeln aus TK verbunden. Das Verfahren ist als erster Algorithmus angenommen und muss horizontale und vertikale Streifen unterstützen (K2). Die endgültige Suchzone bleibt K1.

1. Innerhalb des beschlossenen Suchraums lange, weitgehend achsenparallele Kanten anhand von Farb-/Helligkeitsprofilen suchen. Spalten- und Zeilenprofile entsprechend der Orientierung verwenden.
2. Mehrere plausible Begrenzungspaare anhand gemeinsamer Ausrichtung und räumlicher Folge prüfen. Die zwei stärksten Kanten können zu Türrahmen, Möbeln oder Schatten gehören und sind nicht automatisch der Streifen.
3. Innerhalb des Kandidaten Farbwechsel zwischen aufeinanderfolgenden Zeilen beziehungsweise Spalten bestimmen. Ähnliche Helligkeit benachbarter Farben erfordert Farbinformation; reine Graustufenkanten reichen nicht immer.
4. Feldbänder trennen und Geometrie, Mindestgröße und Flächenkonsistenz prüfen. Keine feste Feldanzahl oder gleiche Feldhöhe verlangen. Einzelne vollständig erkennbare Rechtecke sind zulässig; Mehrdeutigkeit liefert eine Hilfe statt geratenem Ergebnis.
5. Trennlinien anhand schmaler Geometrie, Nachbarschaft und wiederkehrender Struktur bewerten. Hohe Helligkeit und geringe Buntheit allein dürfen weiße, cremefarbene oder hellgraue Felder nicht entfernen.
6. Messflächen ins Feldinnere legen. Erkennungsfläche und Messfläche sind getrennt. Ränder, Trennlinien und Beschriftung möglichst ausschließen; verbleibende Störungen robust behandeln.
7. Zeitliche Beständigkeit und Qualitätszustand bestätigen, dann nur geeignete Felder auswerten.

Ein abgerundeter oberer Abschluss ist zulässig, wenn ein ausreichender einfarbiger Innenbereich verbleibt. Kleine Druckzeichen sind nicht garantiert extreme Ausreißer. Viel Text oder starke Textur darf nicht in eine angeblich homogene Farbe umgedeutet werden. Leichte Wand- oder Gewebestruktur ist nicht automatisch ungültig.

### 7.1.1 Geraderichten vor der Farbfeldsuche

**Nutzerentscheidung vom 22. September 2026:** Iro soll die Drehung des Streifens in der Bildebene früh erkennen und das Bild für die Geometriesuche auf die nächstgelegene waagerechte oder senkrechte Lage ausrichten. Dieser Schritt liegt vor der Suche nach den Farbvierecken. Analyse 0.4.0 setzt die Entscheidung als automatische Winkelschätzung mit höchstens 45° Korrektur um. Die geradgerichtete Ansicht dient der Erkennung; die Farbmessung verwendet unveränderte Originalpixel. Feld-, Innen- und Referenzflächen werden als Polygone in Originalkoordinaten zurückgeführt. Bei uneindeutiger Richtung wird keine Drehung erzwungen. Geraderichten ersetzt keine perspektivische Entzerrung und hebt Qualitätsprüfungen für Unschärfe, Beleuchtung oder unzureichende Flächen nicht auf. Verfahren, Grenzen und Vorher-/Nachher-Prüfung: [Gerade richten vor der Farbfeldsuche](docs/gerade-richten.md).
### 7.2 Übernommene Versuchsparameter

| Prüfung | Vorhandener Startwert | Einschränkung |
|---|---|---|
| Suchraum der Randvariante | Äußere 30 % jeder Bildseite; Kandidaten dürfen nach innen reichen | Nur K1-Randvariante, nicht stillschweigend gemeinsamer Suchraum |
| Suchraum der Mittelvariante | Mittiges Band | OP nennt keine numerische Breite; hier wird keine erfunden |
| Restabweichung der Geometriesuche nach dem Geraderichten | Etwa 10° | Vorläufig; das Geraderichten korrigiert Bilddrehung, keine starke Perspektive |
| Vollständiges Feld | Kurze Seite ≥ 40 Analysepixel und Fläche ≥ 2.500 Pixel | Auf unvergrößertes Analysebild bezogen |
| Feldinneres nach TK | Je 15 % vom Feldrand nach innen; ≥ 20 Pixel je Seite und ≥ 600 nutzbare Pixel | An Gerät und Textanteil prüfen |
| Feldinneres nach KP | Zentrale ca. 60 % der Feldfläche | Alternativer Parameter, nicht rechnerisch mit „15 % je Rand“ gleichsetzen |
| Manuelle Referenz nach TK | Seitenlänge 15 % der kürzeren Bildseite; gleiche Pixel-Mindestanforderungen | Nur betreffende Referenzvariante |
| Lokale Referenzbänder nach KP | Links/rechts, je ca. 40 % der Streifenbreite, auf Feldhöhe; Abstand ≥ 3 % der Streifenbreite | K3-Kandidat; Werte nicht validiert; keine automatische Hellerauswahl |
| Zeitliche Bestätigung | Mindestens drei aufeinanderfolgende gültige Auswertungen | Versuchsparameter |

Künstliches Hochskalieren erfüllt keine Mindestgröße. Bei niedriger Suchauflösung geometrische Grenzen passend normieren und tatsächliche Samplezahl/Innenfläche im Messpuffer zusätzlich prüfen. Alle Grenzen zentral und versioniert führen.

### 7.3 Fehlerverhalten

Zu kleine, stark schräge, verdeckte, abgeschnittene oder nicht trennbare Felder erhalten keinen Wert. Andere vollständig erkennbare Felder dürfen weiter ausgewertet werden. Keine globale Behauptung über nicht erkannte Felder. Überlappt die Referenz ein Feld, die betroffenen Vergleiche sperren; bei gemeinsamer Referenz alle.

Verhalten ohne Erkennung: TK schließt einen versteckten beliebigen Punktvergleich aus; KP hält zwei manuelle Messzonen als Fallback offen. Deshalb keine solche Funktion als Konsens implementieren. Zwei feste Diagnosezonen im Kameraversuch bleiben davon getrennt und sind gemeinsam vorgesehen. Die Fallback-Frage bleibt O05.

*Herkunft: TK §7, §9; KP §3.3–3.6, §9; AS §4.7, §5.2–5.3, §6; OP §2.5, §3.4–3.5, §3.8.*

## 8. Farbrechnung und zeitliche Verarbeitung

### 8.1 Gemeinsame Rechenkonvention

sRGB → lineares RGB → XYZ mit D65-Referenzweiß → CIELAB mit demselben D65-Weiß → CIEDE2000. Lab(D65) nicht ungeprüft mit Lab(D50)-Referenzdaten vermischen. sRGB-Übertragungsfunktion und XYZ-Matrix aus der dokumentierten Referenz verwenden.

Für ΔE00 zunächst `kL = kC = kH = 1`, intern `double`, erst bei Ausgabe runden. Die Implementierung gegen den veröffentlichten Sharma-/Wu-/Dalal-Prüfsatz mit 34 Paaren prüfen, einschließlich schwieriger Farbtonübergänge. Kein verkürzter Formel-Nachbau ohne Referenztest.

CIE76 und CIE94 sind in beiden Analysen als Entwicklervergleich vertretbar, nicht als Endanwenderauswahl. OP verlangt beide im Core; AS hält sie im Testwerkzeug für gegebenenfalls sinnvoll. Die feste Zusage ihrer Implementierung ist daher unter O06 erhalten, nicht Pflicht für den ΔE00-Kern. Der vorhandene CIE94-Vorschlag lautet Graphic Arts mit `kL = 1`, `K1 = 0,045`, `K2 = 0,015`; die vollständige Konvention wäre vor einem solchen Test zu dokumentieren.

### 8.2 Robuste Flächenfarbe

**Gemeinsamer implementierbarer Startkandidat:** Ausreißerbehandlung, Mittelwert der behaltenen Pixel im linearen RGB, erst danach XYZ/Lab. OP übernimmt diesen Weg ausdrücklich; AS hält daran als Kandidat fest, verlangt aber den Vergleich statt eines vorweggenommenen Siegers.

Ablauf:

1. Nur definierte innere Messmaske verwenden; Ränder, bekannte Textbereiche und ungültige Pixel ausschließen.
2. Mit dem geprüften Konverter in den definierten RGB-Raum überführen.
3. Ausreißer gegenüber der lokalen Verteilung bestimmen. Zulässigen Ausschlussanteil konfigurieren; zu viele verworfene Pixel machen die Fläche ungültig.
4. Behaltene Pixel im linearen RGB mitteln; danach einmal nach XYZ/Lab konvertieren. Gamma-/Mittelwertreihenfolge durch Test absichern.
5. Referenz und Feld desselben Frames vergleichen. In der gemeinsamen-Referenz-Variante wird deren Farbe einmal je Frame ermittelt und für alle Felder verwendet. Die Referenzzuordnung der Produktversion folgt K3.

Median/MAD aus KP ist ein konkreter Statistikbaustein. MAD bedeutet `median(abs(x - median(x)))`. Für MAD = 0 ist vor Verwendung ein definiertes Verhalten nötig; keine neue Ersatzregel wird hier erfunden. Der kanalweise Lab-Median nach Ausreißerverwerfung bleibt als AS-Testkandidat erhalten, wird von OP aber nicht als produktiver Flächenschätzer unterstützt (O07).

Clipping, starke Streuung und zu wenige verbleibende Pixel liefern einen Qualitätszustand statt eines Ersatzwerts. Weiß, Schwarz oder gesättigte Farben nicht allein nach Farbnamen verwerfen; prüfen, ob relevante Unterschiede im Signal auflösbar sind.

### 8.3 Paarung vor Glättung

**Gemeinsam und zwingend:** Zuerst gültige Referenz-/Feldpaare innerhalb eines Frames und deren ΔE00 berechnen. Erst danach zeitlich robust aggregieren. Keine getrennte Auswahl dunkler Muster- und heller Referenzproben verschiedener Frames.

AS liefert den Pflichtgegenfall: Beide neutralen Flächen besitzen je Frame dasselbe L*, über acht Frames zweimal 48, zweimal 49, zweimal 51 und zweimal 52; a* = b* = 0. Alle frameweisen Abstände sind null. Die getrennte Extremquartil-Auswahl setzt dagegen L* 48 und 52 zusammen und erzeugt ΔE00 = 4. Dieser Fehler ist unabhängig davon, wie ruhig das Endergebnis wirkt.

**Start / Versuch aus TK:** Median der letzten fünf gültigen ΔE00-Werte je stabiler Feld-ID. Referenz-/Feldwechsel, Kameraänderung und ungültiger Zustand leeren die Historie. Rohwerte bleiben in Entwicklerdiagnosen verfügbar. Rangwechsel erst nach drei aufeinanderfolgenden Bestätigungen. Aktualität und Verzögerung gemäß Kapitel 3 und 9 prüfen.

**Verbindliche Ergänzung – Nachregelung während der Messung:** Ändern sich die tatsächlich angewandten Aufnahmeparameter während der Messung erheblich, dürfen frühere Werte die aktuelle Anzeige nicht verfälschen. Die betroffene Glättungshistorie wird zurückgesetzt. Das gilt insbesondere bei eingeschalteter Belichtungs- oder Weißabgleichsautomatik auch dann, wenn der Nutzer keinen Schalter verändert hat. Maßgeblich sind die dem jeweiligen Analyseframe eindeutig zugeordneten Aufnahmeinformationen, nicht allein die angeforderten Einstellungen.

Geringfügige Schwankungen lösen keinen pauschalen Neustart aus. Die erforderlichen Toleranzen werden anhand realer Bildfolgen während der Entwicklung festgelegt und geprüft. Dabei sowohl abrupte Änderungen als auch eine über mehrere Frames anwachsende Veränderung berücksichtigen, damit viele kleine Schritte keine erhebliche Änderung gegenüber den noch verwendeten historischen Werten verdecken. Betroffene Rangbestätigungen ebenfalls neu aufbauen; bereits überholte Auswertungen dürfen die zurückgesetzte Historie oder Anzeige nicht wiederherstellen. Ein geeignetes aktuelles Bild bleibt auswertbar, sofern die übrigen Freigabebedingungen erfüllt sind.

OPs zusätzliche Gewichtung nach einem Glanzmaß ist ein nicht ausgearbeiteter Kandidat. Sie ist keine gemeinsam festgelegte Formel und gehört zur offenen Mehrwinkelfrage K5. Ein Mittelwert/Median über Frames allein ist kein belegter Reflexkorrektor.

*Herkunft: TK §8; KP §3.6–3.8, §6.1; AS §4.1, §5.3, §6, §8 V2–V3, §9; OP §2.6, §3.4, §5.1, §6.*

## 9. Zustände, Gültigkeit und Qualität

OP übernimmt das Zustandsmodell aus TK ausdrücklich. Es gilt für die Live-Anteile jeder beschlossenen Variante; ein festgehaltenes Abschlussresultat wäre gesondert zu kennzeichnen und ist noch nicht beschlossen.

| Zustand | Verhalten |
|---|---|
| Berechtigung fehlt | Begründung und erneuten Zugang zur Freigabe anbieten; keine Messung |
| Kamera startet | Vorschau, Fähigkeiten und Einstellungen aufbauen und prüfen; keine Werte |
| Muster gesucht | Referenz, soweit bestimmbar, und zur beschlossenen Geometrie passende Hilfe |
| Stabilisierung | Geeignete Felder vorhanden; kurz „Bitte ruhig halten“ |
| Messung gültig | Werte/Rangmarkierung anzeigen, fortlaufend weiter prüfen |
| Einzelnes Feld ungültig | Seine Zahl entfernen, übrige gültige Felder weiter auswerten |
| Referenz ungültig | Alle von dieser Referenz abhängigen Werte entfernen; bei gemeinsamer Referenz sämtliche Werte |
| Kamera nicht verfügbar | Sitzung sauber schließen, verständlichen Neustart anbieten |

Jeder Wert trägt Gültigkeit und Aufnahmebezug. „Keine Messung“ niemals als `0,0` oder `0,00` ausgeben. Ein ungültiger aktueller Frame entfernt betroffene Live-Werte sofort. Keine alten Zahlen über ein inzwischen anderes Kamerabild legen.

### 9.1 Veraltete Ergebnisse sperren

**Festgelegt durch Nutzerentscheidung zu Prüfpunkt 3:** Die Zuordnung von Bild und Metadaten nach §6.1.1 wird um den Schutz vor verspätet abgeschlossenen Berechnungen ergänzt.

- **Messgeneration kennzeichnen:** Bei Kameraneustart, Einstellungs- oder Referenzwechsel beginnt eine neue Generation. Ergebnisse älterer Generationen werden verworfen.
- **Reihenfolge sichern:** Ein älteres Ergebnis darf ein bereits angezeigtes neueres nicht ersetzen.
- **Alter vor Anzeige prüfen:** Ein verspätetes Ergebnis wird nicht durch seinen Eingang wieder „frisch“. Die vorläufigen 500 ms zählen bei bekannter Zeitbasis ab Aufnahme, andernfalls ab dem ersten Bildempfang – mit der bereits dokumentierten Einschränkung: Zeit seit Empfang ist nicht das tatsächliche Aufnahmealter.
- **Auch ohne neue Ergebnisse ausblenden:** Die Anzeige entfernt abgelaufene Werte selbstständig.

**Die 500 ms bleiben ein während der Entwicklung zu prüfender Startwert.**

### 9.2 Qualitätsprüfungen

**Aktuelle Ergänzung, Fassung 1.27:** Die aufnahmeweiten Sperren und die vollständige Streifensichtbarkeit aus §6.4 haben Vorrang vor früheren feldweisen Freigaben. Die folgenden Regeln für Teilmessungen gelten nur, soweit diese Sperren nicht greifen. Technische ältere Prüfberichte sind Nachweise ihres damaligen Umfangs, keine Abnahme des neuen Protokolls.

**Verbindlicher Ansatz:** Die App prüft jedes zu analysierende Bild auf seine Auswertbarkeit. Erkannte Bewegungsunschärfe, zu weicher Fokus, unbrauchbare Unter-/Überbelichtung, störende Reflexe und starkes Licht-Schatten-Spiel können die betroffenen Messbereiche ungültig machen. Betrifft der Fehler die gemeinsame Referenz oder das gesamte Bild, wird die gesamte davon abhängige Messung abgewiesen. Geeignete Bereiche dürfen weiter ausgewertet werden, sofern Zuordnung und Referenz gültig sind.

Erkennbare Qualitätsprobleme müssen dem Nutzer klar und handlungsorientiert gemeldet werden, beispielsweise „Bild unscharf. Kamera ruhig halten und neu fokussieren“, „Bild zu dunkel/zu hell. Beleuchtung prüfen“ oder „Reflexe oder starke Schatten. Licht und Ausrichtung anpassen“. Ist die Ursache nicht eindeutig unterscheidbar, einen passenden allgemeinen Qualitätshinweis statt einer geratenen Diagnose anzeigen. Sobald wieder geeignete Bilder vorliegen, muss die Auswertung wieder möglich sein. Keine pauschale Bewegungssperre.

Qualität umfasst:

- Geometrie: Größe, Achsenlage, sichtbares Feldinneres, nutzbare Pixel, Referenzüberlappung.
- Bildinhalt: übermäßige Streuung, lokale Helligkeitsunterschiede, Clipping und Reflexhinweise.
- Zeit: stabile Flächenidentität und plausible Beständigkeit; keine sichere Rangfolge bei instabiler Zuordnung behaupten.
- Fokus: Feldkanten und verfügbare Kameraangaben; homogene Wand ohne Kanten nicht allein deshalb als unscharf einstufen.

**Technische Weiterentwicklung vom 22. September 2026, Fassung 1.24:** Im Einzelbildversuch prüft Analyse 0.3.0 zusätzlich die räumliche Farbvariation zwischen neun Teilflächen. Sie bewertet das Messinnere, die tatsächliche Referenz und eine größere, nur um 5 % eingerückte Feldinnenfläche. Die vorläufige Sperrgrenze beträgt 2 ΔE00 zwischen robusten Teilflächenfarben; sie ist kein Genauigkeitsversprechen. Farbrechnung und geometrische Referenzwahl bleiben unverändert. Einzelne gestörte Felder werden gesperrt, eine ungeeignete gemeinsame Referenz sperrt ihre Vergleiche. Die Ursache wird als räumliche Uneinheitlichkeit beschrieben; vorhandene konkrete Unschärfehinweise bleiben erhalten. Begründung, Vorher-/Nachher-Zahlen, zusätzliche Sperren zuvor numerisch passender Teilmessungen und offene Grenzen stehen im [Prüfbericht zur räumlichen Flächenprüfung](docs/raeumliche-flaechenpruefung.md). Die offenen Produktentscheidungen zur endgültigen Referenzstrategie und Beleuchtungskorrektur werden damit nicht geschlossen.
### 9.3 Abstand, Blickwinkel und weitere Störfaktoren

**Nutzerergänzung:** Zu geringer Abstand, zu großer Abstand und ein zu flacher Blickwinkel gehören ausdrücklich zu den zu prüfenden Störfaktoren. „Zu flach“ bedeutet hier eine stark seitliche, streifende Sicht auf die Messfläche mit perspektivisch zusammengedrückten Feldern; dies ist von einer bloßen Drehung des Streifens innerhalb des Bildes zu unterscheiden. Horizontale und vertikale Streifen bleiben gleichermaßen zulässig.

| Störfaktor | Zu prüfendes Bildproblem und Reaktion |
|---|---|
| Zu nahe an Muster oder Referenz | Fehlende Schärfe, angeschnittene Felder oder zu wenig sichtbare Referenz prüfen. Ungeeignete Messungen sperren; je nach Befund „Etwas mehr Abstand halten“ oder „Muster und Referenz vollständig ins Bild nehmen“. |
| Zu weit entfernt | Zu kleine Feldinnenflächen, zu wenige nutzbare Pixel oder nicht mehr trennbare Felder prüfen. „Muster zu klein. Bitte näher herangehen.“ |
| Zu flacher Blickwinkel / starke perspektivische Verkürzung | Unzureichend erkennbare Feldgeometrie und zu schmale Messflächen zurückweisen. „Kamera möglichst frontal auf Muster und Referenz ausrichten.“ |
| Unterschiedliche Entfernung oder Ausrichtung von Muster und Referenz | Prüfen, ob beide Messbereiche gleichzeitig scharf und geeignet sind. Bei Problemen auf ähnliche Ausrichtung und Anlegen des Streifens an die Referenzfläche hinweisen. |
| Verdeckung durch Finger oder andere Gegenstände | Verdeckte Feldteile, Objektivbereiche und Referenzbereiche als Prüffälle aufnehmen. Keine Fremdfläche als Musterfarbe auswerten; „Muster und Referenz freihalten“, sofern die Verdeckung erkennbar ist. |
| Gebogener, geknickter oder beschädigter Streifen | Geometrie, Knickschatten, Reflexe und ausreichende ungestörte Innenflächen prüfen. Bei Bedarf „Streifen möglichst eben halten“. |
| Verschmutzte oder beschlagene Linse; Streulicht / Gegenlicht | Fälle mit Schleier, Kontrastverlust, Lichtflecken und Unschärfe prüfen. Bei erkennbar unbrauchbarer Qualität sperren; als Handlungshilfe „Linse und Lichteinfall prüfen“, ohne die Ursache als sicher erkannt zu behaupten. |
| Flackernde Beleuchtung, wandernde Helligkeitsbänder oder rasche Lichtwechsel | Einzelbilder und Bildfolgen auf gestörte Messbereiche beziehungsweise instabile Werte prüfen. Bei unbrauchbaren Bedingungen „Gleichmäßigere Beleuchtung verwenden“. |
| Mischlicht oder farbige Reflexionen aus der Umgebung | Räumlich und zeitlich unterschiedliche Farb-/Helligkeitsverteilungen als Testfälle aufnehmen. Erkennbare Störungen melden; eine gleichmäßige Farbverschiebung nicht als sicher automatisch erkennbar voraussetzen. |
| Stark gemusterte, fleckige, verschmutzte oder uneinheitliche Messfläche | Fehlenden repräsentativen Farbwert innerhalb der Messfläche erkennen, soweit anhand der Verteilung möglich. „Referenzbereich auf eine gleichmäßigere Stelle setzen“. Leichte zulässige Struktur nicht pauschal verwerfen. |
| Ähnliche Rechtecke im Hintergrund oder mehrere Musterkandidaten | Verwechslungen und Mehrdeutigkeit prüfen; keine willkürliche Auswahl und keine fremden Flächen als Farbfelder ausgeben. „Nur den gewünschten Streifen ins Bild nehmen“, wenn die Zuordnung unklar bleibt. |

Die Ablehnung richtet sich nach erkennbaren Auswirkungen auf Messfläche, Bildqualität und Zuordnung, nicht nach einer geratenen Entfernung in Zentimetern oder einem ungeprüften Winkelwert. Ein nahes, fernes oder leicht schräges Muster bleibt auswertbar, wenn es die Qualitäts- und Geometrieanforderungen erfüllt. Konkrete Grenzen erst anhand praktischer Versuche festlegen. Ist die Ursache uneindeutig, den nachgewiesenen Bildmangel melden und keine sichere Ursachendiagnose vortäuschen.

Numerische Grenzen für Clipping, Unschärfe, Textanteil, Ausschlussanteil und Streuung nach dem ersten Gerätdatensatz festlegen. Zentrale versionierte Analysekonfiguration statt verstreuter Konstanten. Nicht jede ungeeignete Beleuchtung ist automatisch erkennbar; vollständige Aufnahmekontrolle wird nicht versprochen.

*Herkunft: TK §6–9; AS §4.8, §6, §8 V5; OP §3.1–3.3, §3.8, §6.*

### 9.4 Regelaudit vor weiteren Bildoptimierungen

**Nutzerpriorisierung vom 22. September 2026, Fassung 1.28:** Zuerst den tatsächlichen Stand von Iro und IroGen gegen das neue Protokoll prüfen, Schwerpunkt Kunden-App. Regelverstöße und fehlende notwendige Schutzprüfungen vor neuen Optimierungen bearbeiten. Der [Regelaudit](docs/regelaudit-2026-09-22.md) trennt Codebefunde, reproduzierte Verstöße, Korrekturen und offene Abdeckung.

Analyse **0.5.0** behebt nachgewiesene Freigabefehler: Erkannte unbrauchbare Unschärfe sperrt die gesamte Aufnahme; geometrisch passende Randfortsetzungen sperren angeschnittene Streifen; mehr als 2 % der robust behaltenen Messpixel mit einem Kanal exakt 0 oder 255 sperren die Aufnahme wegen fehlender Kanalreserve. Diese zentral versionierte 8-Bit-Endpunktregel ist konservativ und noch keine abgenommene Gerätegrenze. Ausgeschlossene kleine Druck-/Glanzpixel zählen nicht automatisch als messrelevant. Echte Farben am Codewertanschlag können ebenfalls abgewiesen werden; aus dem Endpunkt allein wird keine sichere Über-/Unterbelichtungsursache behauptet. Die Messwerte stammen weiterhin aus Originalpixeln. Diagnosefelder und historische Läufe werden nicht umgedeutet.

IroGen kennzeichnet nominale Abweichungen als Entwicklungsbefunde statt als automatisch falsche Messwerte; nominal unauffällige Ergebnisse sind keine Genauigkeits- oder Abnahmezusage. Messungen ohne nominale Vergleichsbasis werden gesondert ausgewiesen. Die App zeigt entsprechend dem bestehenden Startansatz eine Nachkommastelle.

**Teilfortschritt in Analyse 0.5.1, Fassung 1.29:** Kleine passende Randreste bleiben auch unterhalb der Mindestmessgröße als Beschnitthinweise erhalten. Ab drei Feldern sperrt ein kohärenter Querbreitentrend mit Bestimmtheitsmaß mindestens 0,90 und geschätzter Breitenänderung über 15 % der maximalen Feldbreite die Aufnahme mit Aufforderung zur frontaleren Aufnahme. Dies sind technische Versuchswerte, keine Kamera-Winkelgrenzen. Zwölf unabhängige Pixelgegenproben belegen acht behobene Schutzfehler und vier weiterhin messbare Kontrollen. Gleichmäßige perspektivische Verkürzung, natürliche Breitenunterschiede und verbleibende Beschnittgrenzen sind damit nicht allgemein gelöst. [Prüfung und Grenzen](docs/geometrie-schutzpruefung.md); Nutzerabnahme offen.
**Aktueller Teilfortschritt in Analyse 0.5.2, Fassung 1.30:** Die Breitenprüfung aus 0.5.1 erzeugte in vier unabhängigen Gegenbeispielen falsche Perspektivablehnungen bei unterschiedlich breiten Rechtecken. Sie verlangt jetzt zusätzlich gleichgerichtete Konturverjüngung innerhalb mindestens zweier und mindestens der Hälfte der Felder (aufgerundet): mindestens zwei Erkennungsrasterpixel und 2,5 % Breitenänderung zwischen inneren Konturbändern. Technische Versuchswerte, keine abgenommene Winkelgrenze. Die Gegenbeispiele und vier Kombinationen mit Abdunklung/Störpixeln bestehen; Details und aktuelle Prüfergebnisse im [Geometrienachweis](docs/geometrie-schutzpruefung.md). Gleichmäßige Verkürzung ist allein anhand unbekannter Feldseitenverhältnisse nicht von tatsächlich schmalen Feldern unterscheidbar. Keine feste Feldform oder ungeprüfte Winkelschwelle daraus ableiten; die allgemeine Perspektivanforderung bleibt offen.
**IroGen-Nachweis vom 23. September 2026, Fassung 1.31:** Der Nutzer verlangt den Nachweis über einen ladbaren JSON-Testplan und den normalen Generator-/Analyse-/Exportweg. IroGen 1.4.0 ergänzt dafür optionale zentrierte Feldbreitenfaktoren (ein endlicher Faktor 0,3 bis 1 je Feld; ohne Angabe unveränderte Geometrie). Der [Plan mit zehn Bildern](iro-gen/testplans/perspektivkorrektur-konturpruefung.json) prüft unterschiedliche Rechteckbreiten, Kontrollen und deutliche Verjüngung mit/ohne Abdunklung und Rauschen. Unter Analyse 0.5.2 erfüllt der Probelauf neun Erwartungen; die waagerechte Kombination aus Verjüngung, Abdunklung und Rauschen gibt entgegen der Sperrerwartung zwei Felder frei. Historischer Befund unter 0.5.2; die gezielte Behebung unter 0.5.3 ist nachstehend dokumentiert. Der [Nutzerlauf vom 23. September 2026](iro-gen/testplans/iro-testbericht-20260923-114032.md) bestätigt dieselben neun erfüllten Erwartungen und dieselbe Fehlfreigabe; die Berichtsübermittlung ist keine Nutzerabnahme. [Anleitung, Sollverhalten und Probelauf](iro-gen/testplans/perspektivkorrektur-konturpruefung.md). Der Auswertungsdialog bewertet weiterhin nominale Diagnosen; maschinelle geprüfte Erwartungsauswertung im regulären Dialog bleibt eine eigene offene Arbeit.
**Aktuelle Korrektur in Analyse 0.5.3, Fassung 1.32:** Die kombinierte Fehlfreigabe entstand durch den vorzeitigen Abbruch der Verjüngungsprüfung bei nur zwei erkannten Feldern. Nun werden auch zwei Felder geprüft, sofern beide Konturen gleichgerichtete Verjüngung unabhängig belegen; die übrigen Versuchsschwellen bleiben unverändert. Rechteckkontrollen und einseitige Konturverjüngung lösen diese Sperre nicht aus. 28 gezielte Geometrieprüfungen und alle zehn Erwartungen des unveränderten IroGen-Plans bestanden. Abschließende Gesamttests und Grenzen im [Geometrienachweis](docs/geometrie-schutzpruefung.md). Die konkrete Lücke ist technisch behoben, Nutzerabnahme offen; keine allgemeine Perspektiv- oder Gerätefreigabe.
**Vorrangig offen:** belastbare starke-Perspektive-Sperre, Unterbelichtungsprüfung ohne Endpunktanschlag, vergleichbare Beleuchtung beider Messpartner, verbleibende Vollständigkeits-/Unschärfe-/Reflexerkennung und Ausführung geprüfter Freigabe-/Hinweis-Erwartungen im Testwerkzeug. Keine universellen Schwellen aus Generatorstufen ableiten. Warnung versus Ablehnung bei Schatten bleibt gemäß Nutzerprotokoll konkret festzulegen. Kein Erkennungsbild-Aufhellen, Nachschärfen oder neues Farbkorrekturmodell vor dem Schließen beziehungsweise ausdrücklich abgenommenen Abgrenzen dieser Lücken. Die noch fehlende Kamera ist eine spätere Entwicklungsphase, kein Anlass, die technische Abnahme zu überspringen.

## 10. Testwerkzeug, Datensätze und Abnahme

### 10.1 Getrennte Nachweise

| Ebene | Was sie belegt | Was sie nicht belegt |
|---|---|---|
| Farbmathematik | Implementierung der Konvertierung und ΔE-Formel | Richtigkeit der Kamera-Bildaufbereitung |
| Kontrollierte Puffer/Bilder | Strides, UV-Reihenfolge, Zuschnitt, Segmentierung, Feldzuordnung, Statistik und Overlay | Genauigkeit eines konkreten Smartphone-Sensors/ISP |
| Reale Aufnahme | Geräteverhalten, Wiederholbarkeit und Störanfälligkeit unter protokollierten Bedingungen | Ohne unabhängige Referenz keine allgemeine absolute Farbgenauigkeit |
| Unabhängige physische Referenzpaare | Fehler und Rangfolge gegenüber geeigneten Referenzmessungen | Keine unbegrenzte Übertragung auf andere Geräte, Lichtlagen oder Materialien |

**Verbindliche Nutzerentscheidung zur Reihenfolge:** Zuerst den vollständigen technischen Ablauf mit generierten Testbildern und Bildfolgen entwickeln und abnehmen: Hauptoberfläche, Einstellungen, Verdrahtung, Farbanalyse, Erkennung, Qualitätssperren und Hinweise. Erst nach bestandener technischer Abnahme folgen native Kameraintegration und reale Gerätetests. Frühere Empfehlungen für einen vorgezogenen Kameraversuch sind damit abgelöst. Die technische Abnahme ersetzt keine spätere Prüfung realer Kamera- und Messgenauigkeit.

### 10.2 Rechen- und Puffertests

- Sharma-Prüfsatz: alle 34 veröffentlichten ΔE00-Paare innerhalb `0,0001` der gerundeten Referenz; Identität und Symmetrie korrekt.
- sRGB/XYZ/Lab: Weiß, Schwarz, Grau und Primärfarben mit D65-kompatiblen Referenzen; Lab → XYZ → Lab als vorhandener Rundlauftest.
- Mittelwert im linearen RGB und Reihenfolge der Gamma-Verarbeitung prüfen.
- Verschiedene Row-/Pixel-Strides, UV-Reihenfolgen, Zuschnitte und Puffergrenzen testen.
- Exakte Bild-/Metadatenpaarung bei beiden Ankunftsreihenfolgen, fehlenden Gegenstücken oder benötigten Angaben, Pufferüberlauf, Wartezeitablauf und verspäteten Rückmeldungen nach Sitzungswechsel testen. REALTIME-Aufnahmealter und monotone Wartezeit seit Empfang getrennt prüfen.
- Rotation, Seitenverhältnis, Zuschnitt, Spiegelung soweit relevant, Referenztipps und versionierte Overlay-Transformation prüfen.
- Feld-IDs, neue Historien bei unklarer Zuordnung, Referenzwechsel, verlorene Felder, Gültigkeitsablauf und Lebenszyklus testen.
- Verspätet abgeschlossene Berechnungen nach Kamera-, Einstellungs- und Referenzwechsel verwerfen; vertauschte Ergebnisreihenfolge und weiterhin eintreffende, bereits abgelaufene Ergebnisse prüfen. Selbstständiges Ausblenden ohne neue Ergebnisse sowie die Altersgrenze ab Aufnahme beziehungsweise erstem Bildempfang getrennt testen.
- Gepaarte Zeitreihen einschließlich des Null-Gegenfalls aus Kapitel 8 sowie kurzer Reflexe, Schatten und gemeinsamer Helligkeitsänderung prüfen.
- Nachregelung bei unveränderten Nutzereinstellungen anhand realer Bildfolgen und zugehöriger Metadaten prüfen: erhebliche Sprünge sowie schrittweise anwachsende Änderungen setzen betroffene Glättung und Rangbestätigung zurück; geringfügige Schwankungen verursachen keine dauernden Neustarts. Verspätete Ergebnisse aus der Zeit vor dem Zurücksetzen dürfen nicht wieder einfließen. Toleranzen und beobachtete Auswirkungen dokumentieren.

Ein Generator darf nicht ausschließlich dieselbe potenziell fehlerhafte Routine wie die zu prüfende Anwendung als Sollwertquelle verwenden. Formeltests stützen sich auf veröffentlichte Referenzen.

### 10.3 IroGen und reproduzierbare Testbilder

**Nutzerentscheidung vom 22. September 2026, Fassung 1.25:** IroGen ergänzt zufällige Streifenposition und Drehung, seitlichen Blick sowie Blick von oben oder unten in den Stufen leicht/stark/sehr stark und einen getrennten Abstand zwischen Streifen und Wand unter 10 cm, zwischen 10 und unter 20 cm beziehungsweise über 20 cm. Höhenblick wird standardmäßig zufällig von oben oder unten gewählt; festgelegte Richtungen ermöglichen gezielte Hochhalte-/Bückversuche. Alle Effekte sind mit wenig Licht, Bewegungsunschärfe und den bestehenden Störungen kombinierbar. Generator 1.3.0 setzt dies als reproduzierbare Projektions- und Schattenversuche um; Beispielwinkel 15/45/70° und Wandabstände 5/15/30 cm sind Modellparameter, keine gemessenen Entfernungen oder App-Freigabegrenzen. Neue Metadaten und kompakte Berichte halten die tatsächlich ausgeloste Lage fest. Ein zusätzlicher Testplan enthält 108 Bilder in 33 Fällen mit positiven Kontrollen. Modellgrenzen, Ablauf und belegter Prüfstand: [Räumliche Aufnahmeszenarien](docs/raeumliche-aufnahmeszenarien.md). Bestehende Optionen und der bisherige 113-Bilder-Testplan bleiben verwendbar; offene Produktentscheidungen werden dadurch nicht geschlossen.


**Nutzerentscheidung vom 19. September 2026:** Der numerische Testbildgenerator wird als eigenständige WPF-Anwendung **IroGen** unter `D:\Source\iro\iro-gen` umgesetzt. Die Optionenoberfläche erzeugt Wand und einen Farbmusterstreifen mit zufällig gewählter Farbfamilie, leichten Abstufungen, wählbarer Feldzahl und kleinen Zwischenräumen. Einstellbar sind Position rechts/links/oben/unten/mittig, horizontale/vertikale Ausrichtung, Streifen- und Bildgröße sowie eine exakt passende, leicht/mittel/stark abweichende oder ganz andersfarbige Wand. Glanzlicht, Verschmutzung, Fokus- und Bewegungsunschärfe, Licht/Schatten, Nah-/Fernabstand, Drehung und perspektivische Verkürzung dienen als kombinierbare Störfälle. Weitere implementierte Varianten sind Wandstruktur, Rauschen, Vignettierung, Verdeckung, Schleier und Belichtung.

Die erwarteten nominalen Farbabstände zur Wand werden direkt auf die Farbfelder gedruckt; Iro benötigt dafür keine Texterkennung. **Die Bildanalyse und Soll-Ist-Auswertung erfolgen in Iro beziehungsweise dessen Testablauf.** IroGen soll erzeugte Serien übergeben und später zurückgelieferte Analyse-Daten anzeigen; die konkrete Darstellung bleibt ausdrücklich offen. Der Generator liefert Vorschau, reproduzierbaren Seed, Optionen, PNG und die gemeinsame JSON-Aufnahmebeschreibung. Feldfarben, nominale Abstände, Rangfolge und projizierte Geometrie bleiben maschinenlesbar. Die Umhüllungsrechtecke ersetzen keine inneren Messmasken.

**Technische Präzisierung in Fassung 1.17:** Aufdruck und nominale JSON-Werte beziehen sich auf die ursprünglichen quantisierten sRGB-Farben vor Störeffekten. Daraus folgt keine pauschale Erwartung, dass Iro bei Schatten oder Reflexen den ursprünglichen Materialabstand wiederherstellen müsse. Bildbezogene Sollabstände, Freigaben, Hinweise und Referenzplatzierung bleiben bis zur fachlichen Prüfung unbekannt; die Aufnahmebeschreibung wird als `proposed` gespeichert. Die nominalen Werte stehen getrennt in den Generatorparametern. Störstufen und die ungefähren Wandabweichungen ΔE00 1/4/12 sind Generatorparameter, keine neuen App-Grenzen. Offene Produktentscheidungen zu Suchraum und Referenz bleiben offen.

Geprüfte lokale Umsetzung: `net10.0-windows`, WPF, vorhandenes SDK 10.0.401 und Visual Studio Community 2026 18.9.3. Keine zusätzlichen Laufzeitpakete erforderlich. Bedienung, Modelle, Export und Prüfungen stehen in [IroGen](iro-gen/README.md). Das historische Konsolengerüst bleibt vorerst erhalten. Für rechnerisch kontrollierte Farbtestdaten dient IroGen; ergänzende KI-Störbilder nach §10.4.1 und die umfassenden späteren Testdatensätze bleiben vorgesehen. Der aktuelle Generator ist keine Behauptung einer bereits abgeschlossenen technischen Iro-Gesamtabnahme.

| Variation | Vorhandene Fälle |
|---|---|
| Geometrie | Streifenlage, Rotation/Verkippung, Feldzahl, Feldhöhen, Trennlinienbreite, abgerundetes Ende, angrenzende Felder ohne Trennlinien |
| Farben | Minimale, mittlere und große Wand-/Feldabweichungen, ähnliche Helligkeit, Weiß/Creme/Hellgrau, dunkle und gesättigte Farben |
| Beschriftung | Unterschiedliche Textanteile und Störpixel innerhalb und außerhalb der Messfläche |
| Licht und Optik | Helligkeits-/Farbgradienten, Vignettierung/Randabfall, Schatten, simulierte Reflexe |
| Signal und Oberfläche | Verschiedene Rauschstärken, Wandstruktur/Raufaser-artiger Hintergrund, strukturierte Flächen |
| Fehlerbilder | Zu kleine, verdeckte, abgeschnittene, stark schräge und mehrdeutige Streifen; leere Wand |

**Nutzerentscheidung zu Prüfpunkt 5:** Zunächst **mehrere hundert reproduzierbare Testbeispiele mit bekannten erwarteten Ergebnissen** erzeugen. Die Reihe deckt den untersuchten Ergebnisbereich **von ΔE00 = 0 bis N** ab: identische Farben, sehr kleine, mittlere und große Unterschiede sowie Grenzfälle und unterschiedliche Rangfolgen der Musterfelder. N bezeichnet die obere Grenze des jeweiligen Testbereichs, keine neue Obergrenze der Anzeige. Der Nullvergleich ist ein reguläres Testszenario dieser Reihe und kein alleiniger Genauigkeitsnachweis.

Für jedes Beispiel Eingabedaten, Sollfarben beziehungsweise Sollabstände, erwartete Feldzuordnung/Rangfolge und gegebenenfalls erwartete Sperren dokumentieren. Die Sollwerte müssen unabhängig überprüfbar sein; eine bloße Behauptung eines Bildgenerators genügt nicht. Die endliche Testreihe soll die relevanten Ergebnis- und Fehlerklassen systematisch abdecken, nicht eine vollständige Prüfung aller denkbaren Farbwerte behaupten.

**Verbindliche Fehlerauswertung:** Soll- und Ist-Ergebnis je Beispiel vergleichen. Zahlenabweichungen, falsche Feldzuordnungen, falsche Rangfolgen, unerwartete Ausfälle und fehlerhafte Freigaben getrennt protokollieren. Fehlerquoten mit Bezugsmenge und verwendeter Fehlertoleranz ausweisen, auch aufgeschlüsselt nach Testklasse und Farbabstand. So wird nachvollziehbar, wo die Erkennung oder Farbauswertung ungenau ist. Toleranzen werden während der Entwicklung praktisch festgelegt, nicht jetzt theoretisch vorgegeben.

**Verbesserungszyklus:** Fehlerfälle untersuchen, Erkennung und Farbauswertung gezielt verbessern und die Testreihe erneut ausführen. Verbesserungen und Regressionen dokumentieren; den zurückgehaltenen Abnahmesatz gemäß §10.4 unabhängig halten. Diese Auswertung belegt die Fehlerquote für die tatsächlich geprüften Fälle. Reale Kameraeigenschaften werden zusätzlich durch die Geräte- und Referenztests nach §10.5–10.6 geprüft.

Ein kleiner fester versionierter Pflichtsatz bleibt Teil dieser Teststrategie. Größere Sweeps in der überlieferten Größenordnung von **5.000 bis 500.000 Paaren** sind optional nach Laufzeit und Fragestellung skalierbar, keine pauschale Mindestabnahme.

AS präzisiert die Sollwerte: Nach einer simulierten Licht-/Reflexveränderung sind ursprüngliche Materialfarbe und tatsächlich gerenderter Bildwert verschieden. Geometriefehler, Fehler zur Bildfarbe und Robustheit gegen Störungen getrennt auswerten. Eine Pipeline ohne Beleuchtungsrekonstruktion muss nicht automatisch den ursprünglichen Materialabstand wiederherstellen. Diese Präzisierung verhindert, dass ein bestehender Test künstlich einen nicht vorgesehenen Produktumfang verlangt.

### 10.3.1 Bildserien und Testübergabe

**Nutzerentscheidung vom 19. September 2026, Fassung 1.18:** In IroGen die Anzahl der zu erzeugenden Bilder/Farbstreifen angeben können, insbesondere 100 oder 500. Jedes Bild enthält einen Streifen mit einer neuen zufälligen Farbpalette. Die gewählte Geometrie, Feldanzahl, Abstufung und Störungen bleiben erhalten. Wand- und Streifenfarben werden so variiert, dass die Serie identische, sehr kleine, mittlere, große und extreme nominale Farbabstände abdeckt. Nicht allein auf zufällig auftretende Abstände vertrauen.

**Technische Umsetzung:** Der standardmäßig eingeschaltete Abdeckungsmodus verteilt Serien ab elf Bildern möglichst gleichmäßig auf elf numerische Bereiche: exakt 0, (0;0,5], (0,5;2], (2;5], (5;10], (10;20], (20;40], (40;60], (60;80], (80;100] und über 100. Die Reihenfolge und Grundfarbtöne werden reproduzierbar zufällig gewählt. Für jedes Bild wird ein Bezugsfeld zufällig gewählt und seine Wandfarbe im gewünschten Bereich konstruiert; die tatsächlichen quantisierten Abstände werden geprüft. Doppelte Paletten werden neu erzeugt. Dies sind Testbereiche, keine Wahrnehmungs- oder App-Freigabegrenzen. ΔE00 besitzt keine allgemeine 100-Punkte-Skala; eine endliche Serie behauptet weder jeden reellen Abstand noch einen bewiesenen mathematischen Maximalwert abzudecken. Die tatsächlich erreichte Spanne und belegten Bereiche werden angezeigt und gespeichert. Kleinere Serien verteilen sich über den Bereich, können aber nicht alle elf Klassen enthalten.

Im Abdeckungsmodus werden die bislang feste Wandabweichung und das feste Bezugsfeld durch die Serienverteilung ersetzt; dies ist in der Oberfläche sichtbar. Wer gezielt nur eine feste Wandoption untersuchen will, kann den Abdeckungsmodus ausschalten. Einzelbilder behalten den bisherigen Bedienweg. Original-Paletten, Wandfarben, Seed, Abstandsbereich und Zielabstand werden für jedes Bild gespeichert; nominale Werte bleiben vor Störeffekten definiert. Optionen einschließlich Serienanzahl sind speicherbar, frühere Options- und Aufnahmedateien bleiben ladbar.

Die Bilder werden während der Erzeugung in einem eigenen temporären Serienordner abgelegt; die Oberfläche lädt nur die ausgewählte Vorschau. Fortschritt und Abbruch vorsehen. Eine neue Serie ersetzt die vorige erst nach vollständiger Erzeugung. **Alle speichern** kopiert sämtliche PNG-/JSON-Paare samt versioniertem Serienmanifest in einen neuen Ordner; weder alte Exporte überschreiben noch unvollständige Kopien als fertige Serie ausgeben.

**Historisch, Fassung 1.18:** Zunächst nur die Testübergabe vorbereiten; damals noch keine Analyse implementieren. **Aktueller Nutzerauftrag, Fassung 1.19:** Jetzt die Iro-Bildanalyse mit API für IroGen entwickeln, vor der ersten Android-Hauptoberfläche. **An Iro-Tests senden** stellt weiterhin einen vollständigen lokalen Auftrag mit unabhängigen Eingaben unter `tests/requests/` bereit und führt ihn anschließend über die lokale .NET-API aus. Messwerte, Ablehnungsgründe und nominale Vergleiche werden separat unter `tests/runs/` gespeichert. Eine kleine Diagnoseanzeige in IroGen zeigt Istwerte pro Bild; die ausführliche Darstellung bleibt später zu klären.

Gemeinsame Verträge: [Bildserie v1](tests/schemas/bildserie-v1.schema.json), [Testauftrag v2](tests/schemas/testauftrag-v2.schema.json), [Analyselauf v1](tests/schemas/analyse-lauf-v1.schema.json). Aufnahmeformat v1 bleibt unverändert. Historische Testaufträge v1 bleiben lesbar; ihre damalige Kennzeichnung fehlender Analyse wird nicht umgeschrieben.

**Nutzerentscheidung zur Testbedienung (21. September 2026):** Testübergabe und vollständige Analyse laufen asynchron außerhalb des UI-Threads. Erst wenn der Vorgang nach einer Sekunde noch läuft, erscheint ein Busy-Indikator; kürzere Läufe zeigen keinen Indikator. Abschluss, Fehler und Abbruch beenden die Aktivitätsanzeige. Nach einem vollständig verarbeiteten Lauf erscheint eine sichtbare Abschlussmeldung mit dem Button **„Testergebnisse in Windows-Explorer anzeigen“**. Dieser öffnet den zugehörigen Laufordner und markiert die `results.json`. Verarbeitungsfehler werden in der Meldung genannt; ein abgebrochener Lauf wird ausdrücklich als solcher mit gespeicherten Teilergebnissen bezeichnet. Der Zugriff öffnet genau den zu dieser Meldung gehörenden Lauf, nicht pauschal den neuesten Ordner.

**Nutzerentscheidung zur schrittweisen Hinweis-Anbindung (21. September 2026):** Analyseergebnisse bereits jetzt über eine gemeinsame Anzeige in Iro mit dem Anwender verbinden. Der erste Android-Testmodus wertet gespeicherte Original-Testbilder mit dem produktiven Einzelbildkern aus, ohne Kamera oder Kameraberechtigung zu benötigen. Analysehinweis und Feldhinweise werden aus dem Ergebnis übernommen, nicht anhand des Testbildnamens erfunden. Ein Ergebnis mit gesperrter Messung zeigt keine Messwerte; teilweise geeignete Ergebnisse behalten ausschließlich freigegebene Feldwerte. Beim Start einer neuen Analyse, bei Abbruch, Ladefehler und Verlassen/Hintergrundwechsel werden bisherige Werte entfernt; überholte Ergebnisse dürfen nicht nachträglich erscheinen. Ein klarer und der zuletzt untersuchte stark unscharfe Originalfall dienen als wiederholbare Entwicklungsfälle. Kameraanbindung, endgültige Hauptoberfläche und vollständige technische Abnahme bleiben Folgearbeiten. Der beim Unschärfetest gefundene Fehler der Wandreferenz wird durch die Hinweis-Anbindung nicht behoben.

**Nutzerentscheidung vom 22. September 2026, Fassung 1.22:** IroGen lädt im Dialog eine Testplan-Datei mit benannten Fällen, jeweiligen Generatoroptionen und Bildanzahlen. Eine vollständige Serie wird mit einem Bedienvorgang erzeugt und über den vorhandenen Button gemeinsam an die Iro-Tests übergeben. Fortschritt, Abbruch und Erhalt der vorherigen fertigen Serie bleiben erhalten. Der Auswertungsdialog bietet einen kompakten Markdown-Export mit Gesamtmengen, gruppierten Befunden, tatsächlichen Hinweisen sowie identifizierbaren Beispielen; die vollständigen ursprünglichen Ergebnisdateien bleiben unverändert. Der Export dient der Entwicklerauswertung und erweitert nicht die ausgeschlossene Ergebnisverwaltung im normalen App-Betrieb.

**Bedienkorrektur vom 22. September 2026, Fassung 1.23:** „Testergebnisse auswerten“ bezieht sich auf die aktuell erzeugte Serie. Falls für diese Serie noch keine Analyse vorliegt, führt der Button die Testübergabe und Analyse zunächst aus und öffnet danach genau diesen Lauf. Ein vorhandener Lauf derselben Serie wird ohne erneute Analyse geöffnet; nach neuer Generierung darf kein alter Lauf als aktuelles Ergebnis erscheinen. Bei fehlgeschlagener Übergabe wird kein historischer Ersatz geöffnet. Abgebrochene Analysen behalten ihren ausdrücklichen Teilstatus. Die Bildanzahl wird nach Testplan-Generierung korrekt übernommen; während laufender Arbeit ist der Auswertungsbutton gesperrt. Der Export umfasst alle eingelesenen Bilder des geöffneten Laufs, nicht andere historische Läufe. Grundlage ist der Nutzerfehlerbericht mit Screenshots; die bisherige Anzeige war nicht an die sichtbare Serie gebunden.
Die Abstandssimulation bietet **zu weit: weit / weiter / sehr weit** und **zu nahe: nahe / näher / ganz nahe** zusätzlich zur Normalstellung. Die Stufen sind gegenseitig ausschließende geometrische Simulationen, keine kalibrierten Entfernungen. Technische Skalierungen und Testplanformat stehen in [Testpläne und Verdeckung](docs/testplaene-und-verdeckung.md). Der vorbereitete Entwicklungsplan umfasst 113 Bilder in 35 Fällen; diese Anzahl ist keine neue Abnahmegrenze.
### 10.3.2 Erster Einzelbild-Analyseversuch und lokale API

**Nutzerantwort zur Bildzuordnung:** Ein gespeichertes Bild enthält einen Streifen mit seinen Feldern und seiner Wand; das nächste Bild besitzt seine eigene Wand. Farben oder Referenzen niemals zwischen Bildern vermischen. Für den ersten Testweg wird eine gemeinsame, automatisch geometrisch neben dem erkannten Streifen bestimmte Wandfläche pro Bild verwendet. Das ist ein gekennzeichnetes Versuchsprofil; die Antwort legt weder die endgültige Kamera-Platzierung noch eine Schattenstrategie fest. Die offene Produktentscheidung zum Referenzmodell bleibt bestehen.

`Iro.Core` erhält ausschließlich unvergrößerte sRGB-RGB24-Pixel samt expliziten Versuchsparametern. `Iro.Analysis` stellt dafür einen Windows-PNG-Adapter und einen Serienläufer bereit; IroGen verwendet diese lokale .NET-API ohne Netzwerkdienst. Generator-Feldanzahl, Paletten, Masken, Sollwerte oder aufgedruckter Text fließen nicht in die Erkennung oder Farbrechnung ein. Erst nach Abschluss der Pixelanalyse ordnet der Testläufer erkannte Rechtecke den nominalen Generatorfeldern geometrisch zu und protokolliert Differenzen. Diese sind unter Störungen keine automatische Abnahmeentscheidung.

**Technischer Versuch `synthetic-trial-1`, keine Änderung des beschlossenen Zielalgorithmus aus Kapitel 7:** Für kontrollierte Einzelbilder wird zunächst eine Ganzbildsuche nach zusammenhängenden Farbregionen mit anschließender Achsen-/Rechteckgruppierung erprobt. Dieses begrenzte Verfahren ist noch keine vollständige Profil-/Gradientenpipeline und ersetzt die vorgesehene Weiterentwicklung nicht. Messflächen liegen innerhalb der erkannten Felder; robuste Ausreißerbehandlung, Mittelung in linearem RGB, D65-Lab und ΔE00 werden im eigenständigen Iro-Kern berechnet. Zu kleine, mehrdeutige oder ungeeignete Flächen liefern fehlende Werte mit Grund; einzelne ungültige Felder sperren nicht automatisch die übrigen. Eine ungültige gemeinsame Wandreferenz sperrt alle Vergleiche des Bilds. Angeschnittene erkannte Randregionen sind als Wandreferenz ausgeschlossen. Profilparameter und Algorithmusversion werden mit jedem Ergebnis gespeichert.

Die aktuelle Suche bevorzugt deutlich getrennte, annähernd achsenparallele, homogene Felder. Fast weiße Felder können mit weißem Träger verschmelzen; deutliche Schräglage, Perspektive, Schatten und Reflexe erfordern weitere Entwicklung. Gleichmäßige Farbverschiebungen sind aus einem Einzelbild nicht allgemein als Beleuchtungsfehler erkennbar. Keine pauschale Farbgenauigkeits-, Qualitäts- oder Kamera-Freigabe behaupten. Zeitliche Stabilität, Kamerapuffer, endgültige Suchzone, Referenzbedienung und „ähnlich nah“ bleiben Folgearbeiten. Entwicklungsnachweise: [Analyse-Prüfstand](docs/analyse-pruefung.md).

Historische Generator-Prüfbefunde aus Fassung 1.18: 100 und 500 vollständige Bilder mit unterschiedlichen Paletten erzeugt, jeweils alle elf Bereiche abgedeckt; erreichte nominale Spannen bei Seed 46883 und sieben Feldern: 0–109,66 beziehungsweise 0–112,91. Diese Werte sind Ergebnisse der konkreten Serien, keine allgemeine Genauigkeits- oder Skalenfestlegung. Weitere Nachweise stehen in [IroGen-Prüfstand](docs/irogen-pruefung.md).
**Belegter technischer Befund in Fassung 1.22:** Neu erzeugte Verdeckungsfälle zeigen, dass die Fremdfläche selbst in die Streifengruppe gelangen und als Messfeld freigegeben werden konnte. Analyse 0.2.0 sperrt Kandidaten, deren beide Quergrenzen nicht zur Mehrheit der übrigen Felder passen, mit einem Hinweis auf mögliche Verdeckung. Feldhöhen und Farben werden dabei nicht gleichgesetzt. Dies ist eine begrenzte Korrektur im bestehenden Versuchsprofil, keine allgemeine Verdeckungserkennung oder Entscheidung zur endgültigen Suchzone. Reproduktion, Parameter und Grenzen: [Prüfbericht](docs/testplaene-und-verdeckung.md).
### 10.4 Pflichtsatz und zurückgehaltene Abnahme

Startvorschlag aus TK, ausdrücklich von OP unterstützt: **mindestens 30 positive und 30 negative fest abgelegte Bilder**, zusätzlich zu größeren Sweeps. Positive Fälle: variable Feldzahlen/-höhen, helle Felder, schwache Farbkanten, Text und abgerundete Abschlüsse. Negative Fälle: Nadelkopfgröße, leere Wand, starke Schräglage, abgeschnittene/verdeckte Felder, Reflexe, mehrere mehrdeutige Streifen.

**Herausforderung – Nutzerentscheidung zu Prüfpunkt 4:** Durch Tests nachweisen, dass die App sowohl bei gültigen als auch bei ungeeigneten Mustern korrekt reagiert. Das bloße Vermeiden falscher Messungen genügt nicht: Eine App, die grundsätzlich kein Muster erkennt, erfüllt die Anforderungen nicht.

| Testszenario | Qualitatives Abnahmekriterium |
|---|---|
| Gültiger, ausreichend großer Streifen unter geeigneten Aufnahmebedingungen, horizontal und vertikal | Die geeigneten Farbfelder werden vollständig und korrekt zugeordnet erkannt. Bei gültiger Referenz und erfüllten Messbedingungen erscheinen die zugehörigen Vergleichswerte. |
| Gültige Varianten mit unterschiedlichen Feldzahlen/-höhen, hellen Farben, Beschriftung, schwachen Farbkanten und abgerundetem Abschluss | Die Erkennung bewältigt die vorgesehenen Varianten; geeignete Felder werden weder übersehen noch mit Trennlinien, Beschriftungen oder Hintergrund verwechselt. |
| Kein Muster oder ungeeignetes Muster, etwa zu klein, stark schräg, verdeckt oder mehrdeutig | Keine freigegebene Messung für ungeeignete Bereiche; der zum Fehler passende Hinweis beziehungsweise die vorgesehene Sperre erscheint. |
| Angeschnittener und damit nicht vollständig sichtbarer Streifen | Gemäß der neueren Nutzerentscheidung in §6.4 keine Messfreigabe; zum vollständigen Aufnehmen von Streifen und Wand auffordern. |
| Vollständig sichtbarer Streifen mit einzeln ungeeigneter Messfläche | Teilmessung nur, wenn keine aufnahmeweite Sperre aus §6.4 vorliegt und Referenz sowie übrige Flächen geeignet sind. |

**Abnahme:** Positive und negative Fälle müssen jeweils das festgelegte Sollverhalten nachweisen. Treffer, übersehene und fälschlich erkannte Felder sowie Ausfälle protokollieren. Die Prüfung darf weder durch pauschales Verwerfen aller Muster noch durch ungeprüfte Ausgabe von Messwerten bestanden werden.

**Später konkretisieren:** Konkrete Testaufnahmen und Aufnahmebedingungen, Zeit bis zur Erkennung, Beobachtungszeitraum, zulässige Ausfälle und Erfolgsquote werden während der Entwicklung anhand praktischer Versuche festgelegt und vor der abschließenden Abnahme verbindlich dokumentiert. Jetzt werden dafür keine theoretischen Zahlenwerte vorgegeben. Die obigen Kriterien beschreiben das geforderte Verhalten und behaupten keine hundertprozentige Erkennung jedes einzelnen Kameraframes.

Abstimmungsdaten und zurückgehaltener Abnahmesatz bleiben getrennt. Synthetische und echte Fälle getrennt auswerten. Nicht Schwellenwerte am Abnahmesatz einstellen und danach dieselben Daten als unabhängigen Nachweis ausgeben. Eine Strategie darf auch nicht allein durch massenhaftes Verwerfen scheinbar genauer werden.

### 10.4.1 Verbindlicher Testschwerpunkt Bildqualität

**Nutzerentscheidung zu Prüfpunkt 6:** Eine gesondert ausgewertete Testreihe mit **mindestens mehreren hundert Testbildern** für die Erkennung und Behandlung von Bildqualitätsproblemen aufbauen. Sie ergänzt die Sollwert-Testreihe für Farbabstände; deren Umfang allein erfüllt diesen Testschwerpunkt nicht.

| Testszenario | Gefordertes Verhalten |
|---|---|
| Bewegungsunschärfe und zu weicher Fokus in unterschiedlichen Stärken | Unbrauchbare Messbereiche erkennen, Messung dort sperren und verständlichen Unschärfehinweis anzeigen. |
| Unterbelichtung, Überbelichtung und nicht mehr auflösbare Kanalwerte | Ungeeignete Bereiche zurückweisen und auf die Beleuchtung hinweisen; legitime dunkle beziehungsweise helle Farben nicht allein wegen ihrer Farbe verwerfen. |
| Starke oder zahlreiche Lichtreflexe, harte Schatten und ausgeprägte Licht-Schatten-Wechsel | Gestörte Messbereiche erkennen, zugehörige Messwerte sperren und auf Reflexe beziehungsweise Beleuchtungsprobleme hinweisen. |
| Zu geringer und zu großer Abstand; angeschnittene, unscharfe oder zu kleine Messflächen | Ungeeignete Fälle sperren und passende Abstand-/Ausschnitthinweise zeigen; geeignete Vergleichsfälle bei unterschiedlichen Abständen weiterhin auswerten. |
| Flacher Blickwinkel, starke perspektivische Verkürzung; horizontale und vertikale Streifen | Unbrauchbare Geometrie zurückweisen und auf frontalere Aufnahme hinweisen; beide Streifenrichtungen und zulässige leichte Perspektive als positive Gegenfälle prüfen. |
| Unterschiedliche Tiefenlagen/Ausrichtungen von Muster und Referenz; nur eine Fläche scharf | Beide Messpartner auf Eignung prüfen. Ein scharfes Muster darf eine unbrauchbare Referenz nicht ausgleichen und umgekehrt. |
| Finger-/Objektverdeckung, gebogene/geknickte Streifen, ähnliche Hintergrundrechtecke und mehrere Kandidaten | Verdeckte oder falsch zugeordnete Flächen nicht als gültige Messung freigeben; feldbezogene Sperren und eindeutige Nutzerhilfe prüfen. |
| Verschmutzte/beschlagene Linse, Streulicht, Gegenlicht und Kontrastschleier | Erkennbare Qualitätsmängel zurückweisen; Hinweise nach Bildbefund geben, ohne eine nicht nachweisbare Ursache als sicher auszugeben. |
| Lichtflackern, Helligkeitsbänder, rasche Lichtwechsel, Mischlicht und farbige Umgebungsreflexionen | Einzelbilder und zusätzliche Bildfolgen auf Fehler und Fehlfreigaben prüfen; Grenzen der Erkennbarkeit dokumentieren. |
| Stark strukturierte, fleckige oder uneinheitliche Referenzen gegenüber zulässiger leichter Wand-/Gewebestruktur | Ungeeignete Messflächen zurückweisen, aber auswertbare Struktur nicht übermäßig aussortieren. |
| Kombinationen der Störungen sowie lokal begrenzte Fehler | Sperre auf die tatsächlich betroffenen Messungen anwenden; ungültige Referenz sperrt alle davon abhängigen Vergleiche. |
| Geeignete Vergleichsbilder, einschließlich scharfer Bilder trotz Handzittern oder Positionsänderung | Auswertbare Bilder nicht pauschal abweisen; gültige Zuordnung vorausgesetzt, Vergleichswerte liefern. |
| Übergang von geeigneten zu ungeeigneten Bildern und zurück | Unbrauchbare Werte entfernen, passenden Hinweis anzeigen und bei wieder geeigneten Bildern die Auswertung fortsetzen. Hierfür zusätzlich kurze Bildfolgen testen. |

Für jedes Beispiel geeignete/ungeeignete Bereiche und das erwartete Freigabe-, Sperr- und Hinweisverhalten kennzeichnen. Alle Störklassen aus §9.3 mit unterschiedlichen Stärken, Grenzfällen und Kombinationen im Testplan abdecken, jeweils einschließlich geeigneter positiver Gegenbeispiele. Die mindestens mehreren hundert Qualitätstestbilder über diese Klassen verteilen; eine hohe Gesamtzahl ohne Abdeckung einzelner Klassen genügt nicht. Synthetisch erzeugte Fälle durch reale Kameraaufnahmen ergänzen und beide Gruppen getrennt auswerten. Die Einteilung und Abnahmetoleranzen werden während der Entwicklung konkretisiert; keine universelle Erkennbarkeit allein aus der bekannten Ursache eines Testbildes ableiten.

**Verbindliche KI-Bilderzeugung – Nutzerergänzung:** Eine bildgenerierende KI beauftragen, sämtliche Störszenarien aus §9.3 als Testbilder zu erzeugen, einschließlich unterschiedlicher Stärken, Kombinationen, Grenzfälle und geeigneter Gegenbeispiele. Die Bilder werden Teil der mindestens mehreren hundert Qualitätstestbilder. Für zeitliche Störungen ergänzend geeignete Bildfolgen erzeugen beziehungsweise zusammenstellen. Originaldateien, Testfall-ID, Generierungsauftrag und verfügbare Generierungsparameter ablegen, damit spätere Testläufe dieselben Eingaben verwenden.

**Erwartetes Ergebnis je Bild:** Zusammen mit jedem Testbild einen strukturierten Sollbefund festhalten: dargestellte Störung, betroffene Felder/Referenzbereiche, erwartete Erkennung und Zuordnung, Freigabe oder Sperre der jeweiligen Messungen sowie erwarteter Nutzerhinweis. Beispielsweise soll ein unbrauchbar unscharfes Muster keine freigegebenen Farbwerte liefern und einen Unschärfehinweis auslösen; bei nur einem verdeckten Feld sollen geeignete übrige Felder weiter auswertbar sein. Erwartete Meldungen nach ihrer Bedeutung bewerten, nicht ausschließlich nach einem bestimmten Wortlaut. Wo numerische Farbwerte geprüft werden sollen, zusätzlich unabhängig überprüfte Sollwerte bereitstellen.

**Sollbefund prüfen:** Die KI soll das erwartete Verhalten mitliefern. Vor Aufnahme in den verbindlichen Testsatz prüfen, ob das tatsächlich erzeugte Bild die beabsichtigte Störung enthält und der Sollbefund zu den sichtbaren Bildinformationen und den App-Anforderungen passt. Prompt und KI-Antwort allein belegen weder einen exakten Farbwert noch eine eindeutig erkennbare Störungsursache. Ungeeignete Generierungen korrigieren, neu erzeugen oder aus dem Abnahmesatz ausschließen. Den geprüften Sollbefund vor dem App-Test festschreiben und nicht nachträglich an ein fehlerhaftes App-Ergebnis anpassen.

**Soll-Ist-Vergleich und Verbesserung:** Die App beziehungsweise ihre Bildanalyse mit den abgelegten Testbildern ausführen und tatsächliche Erkennung, Messfreigaben/-sperren und Nutzerhinweise gegen den festgelegten Sollbefund vergleichen. Übereinstimmungen und konkrete Abweichungen je Testfall protokollieren. Übersehene Qualitätsprobleme, fälschlich abgewiesene gute Bilder sowie fehlende oder unzutreffende Hinweise getrennt auswerten. Bei Fehlinterpretationen Ursache untersuchen, das Verhalten gezielt verbessern und dieselben Fälle erneut prüfen. Korrigierte Fälle als Regressionstests erhalten; zusätzlich den getrennten, zurückgehaltenen Testsatz verwenden, um Verbesserungen über die bearbeiteten Beispiele hinaus zu überprüfen.

Konkrete Qualitätsgrenzen, zulässige Fehlerraten und Reaktionszeiten während der Entwicklung anhand dieser Versuche bestimmen und vor der abschließenden Abnahme festlegen. Die Erkennung ist anhand der geprüften Fälle nachzuweisen; aus einem einzelnen Bild nicht sicher unterscheidbare Ursachen werden nicht als sicher erkannt behauptet. Reale Kameraaufnahmen bleiben als ergänzende Prüfung vorgesehen.

### 10.5 Reale Versuche

Erst nach bestandener technischer Abnahme des vollständigen Testmodus, bestätigtem Testgerät und anschließender Kameraintegration beginnen. Rohwerte, Median, Streuung, Ausfallquote, Geräte-/Kameraparameter, Geometrie, Licht und Material dokumentieren.

Vorhandenes Wiederholungsprotokoll: pro Aufbau **zehn Sekunden messen**, **zehnmal neu positionieren**. Zuerst denselben matten Farbkörper an verschiedenen Bildpositionen, danach ähnliche und deutlich unterschiedliche Farbpaare. Muster und Referenz räumlich vertauschen. Nullvergleich und Wiederholbarkeit zunächst als Entwicklerdiagnose, nicht als vollständige Selbstkalibrierung.

Prüfbedingungen aus den Quellen:

- Tageslicht, bedeckter Himmel, Glühlampe, warme/kalte LED, Leuchtstoff und Mischlicht.
- Matt/matt, seidenmatt/matt, glänzend/matt und Stoff/Papier.
- Verschiedene Blickwinkel, Papier-/Handschatten, Reflexe und Lichtgradienten.
- Farbtarget mit bekannten Lab-Werten, beispielsweise ColorChecker, soweit tatsächlich verfügbar und mit passender Referenzkonvention.

Vorläufiges Entwicklungsziel aus TK: Bei ruhendem Aufbau und gleichmäßigem Licht ist die Differenz zwischen 95. und 5. Perzentil je Paar höchstens **0,5 ΔE00**. Das ist ein selbst gesetztes Stabilitätsziel, keine Norm und kein Nachweis absoluter Genauigkeit. Nach erster Messreihe bestätigen oder begründet anpassen.

**Verbindlicher Gerätetest: Lichtflimmern und feste Belichtungszeiten**

- Auf dem realen Android-Testgerät, zunächst dem bestätigten Motorola, verschiedene feste Belichtungszeiten unter LED- und Leuchtstofflicht sowie geeigneter möglichst flimmerarmer Vergleichsbeleuchtung untersuchen. Wenn verfügbar, gedimmte Lichtquellen einbeziehen. Emulator und generierte Bilder ergänzen diesen Versuch, ersetzen ihn aber nicht.
- Die Belichtungsautomatik bleibt im vorgesehenen Standardmodus ausgeschaltet. Mehrere vom Gerät unterstützte feste Belichtungszeiten einschließlich des bisherigen Versuchswerts 1/100 s prüfen. Keine universelle Belichtungszeit vorab festlegen und keine Automatik stillschweigend aktivieren.
- Bildfolgen mit eindeutig zugeordneten Aufnahmeinformationen speichern. Lichtquelle und gegebenenfalls Dimmerstellung, Gerät, Kamera, tatsächlich angewandte Belichtungszeit, ISO und Weißabgleich sowie Helligkeitsbänder, zeitliche Farb-/Helligkeitsschwankungen und Auswirkungen auf die Vergleichswerte dokumentieren. Nicht bekannte Eigenschaften der Lichtquelle ausdrücklich unbekannt lassen.
- Androids Antibanding-Einstellung wirkt bei ausgeschalteter Belichtungsautomatik nicht; sie ist daher keine Lösung für den manuellen Standardmodus. Geeignete feste Parameter müssen am Gerät ermittelt werden. [Android: CONTROL_AE_ANTIBANDING_MODE](https://developer.android.com/reference/android/hardware/camera2/CaptureRequest?authuser=3#CONTROL_AE_ANTIBANDING_MODE)
- Prüfen, ob verbleibende erkennbare Störungen die betroffenen Messbereiche zuverlässig sperren und einen verständlichen Hinweis auslösen, beispielsweise „Gleichmäßigere Beleuchtung verwenden“. Geeignete Gegenbeispiele dürfen nicht pauschal verworfen werden. Übersehene Störungen und fälschliche Sperren getrennt auswerten; nicht jede Form von Flimmern als sicher erkennbar voraussetzen.
- Geeignete Parameter, erkennbare Grenzen und Verbesserungsbedarf aus den Versuchen ableiten. Konkrete Qualitätsgrenzen und zulässige Fehler vor der abschließenden Abnahme anhand der Befunde festlegen. Ausgewählte reale Aufnahmen als wiederholbare Regressionstests übernehmen; unabhängige Abnahmedaten getrennt halten.

### 10.5.1 Verbindliche Testdaten-Persistenz auf Windows und Android

**Zweck und Abgrenzung:** Eine nachvollziehbare Datenbasis für Entwicklung, Fehleranalyse, Wiederholung und Abnahme aufbauen. Im normalen App-Betrieb werden weiterhin keine Bilder oder Messergebnisse dauerhaft gespeichert. Die folgende Aufzeichnung ist ein ausdrücklich gestarteter Testmodus der Entwicklungsversion, keine Ergebnisverwaltung für Endanwender.

**Windows – zentrale Ablage im Projekt `D:\Source\iro`:**

- `tests/datasets/`: Testbilder und Bildfolgen mit festem Testfallbezeichner, verständlicher Beschreibung, Herkunft, Aufnahme- beziehungsweise Erzeugungsbedingungen und geprüftem Sollverhalten. Entwicklungs- und unabhängige Abnahmedaten getrennt halten. Der kleine automatisierte Pflichtsatz unter `tests/fixtures/` bleibt eine über Bezeichner und Datensatzversion zugeordnete Auswahl; keine widersprüchlichen Sollbefunde doppelt pflegen.
- `tests/runs/`: Ein eigener Ordner je Testlauf mit Zeitpunkt, Laufbezeichner, App-/Codeversion, Datensatzversion, Gerätekonfiguration und Analyseparametern. Tatsächliche Ergebnisse, Ablehnungen, Hinweise und Soll-Ist-Abweichungen je Testfall maschinenlesbar als JSON ablegen; einen kurzen Markdown-Bericht ergänzen. Frühere Läufe nicht überschreiben.
- `tests/adjustments/`: Beobachtetes Problem, Zeitpunkt, vorgenommene Feinjustierung, Begründung, geänderte Parameter beziehungsweise Codeversion und verknüpfte Testläufe vor und nach der Änderung dokumentieren. Verbesserungen und Verschlechterungen sichtbar halten.

**Android – gezielt gestartete Live-Testaufzeichnung:**

- Testaufzeichnung in der Entwicklungsversion explizit starten und beenden. Geräte-/Kamera-ID, App-Version, Startzeit und gewählte Einstellungen festhalten.
- Für aufgezeichnete Analyseframes die tatsächlich verwendeten Bilddaten, zugehörigen Aufnahmeinformationen, Messflächen, Feldzuordnungen, Farbwerte, Ergebnisse oder Ablehnungsgründe zusammen speichern. Sitzung, Messgeneration und Aufnahmezeitstempel erhalten die eindeutige Zuordnung; menschlich lesbare Zeitangaben ersetzen nicht die technischen Zeitbasen für Altersprüfungen.
- Eingabedaten für die spätere Wiedergabe ausreichend vollständig erhalten: Bildformat, Abmessungen, bei Rohpuffern Strides und Farbraumannahmen sowie die verwendeten Transformationen dokumentieren. Ein Screenshot mit eingeblendeten Zahlen allein genügt nicht zur erneuten Analyse.
- Problematische Momente während der Aufzeichnung markieren und um eine kurze Beobachtung beziehungsweise das erwartete Verhalten ergänzen können. Diese Beobachtung ist zunächst ein zu prüfender Sollbefund, keine automatisch bestätigte Referenzmessung.
- Dateien zunächst im privaten App-Speicher ablegen. Aufzeichnungsdauer und Datenmenge begrenzen; konkrete Grenzen während der Implementierung festlegen. Bei erreichtem Limit oder Speicherfehler die Aufzeichnung verständlich beenden, ohne unbemerkt unvollständige Daten als vollständigen Testlauf auszugeben. Den Einfluss der Aufzeichnung auf die Laufzeit bei Leistungstests berücksichtigen.
- Das zusammengehörige Testpaket anschließend gezielt auf Windows übertragen, zunächst unter `tests/runs/` ablegen und auf Vollständigkeit prüfen. Geeignete Fälle nach Prüfung in die Entwicklungsdatensätze übernehmen. Der konkrete Übertragungsweg wird beim Aufbau des Entwicklerwerkzeugs festgelegt. Daten auf dem Gerät erst nach bestätigter Übertragung gezielt bereinigen; vor Deinstallation oder Löschen der App-Daten sichern.

**Nachvollziehbarkeit und Sicherung:** JSON-Daten erhalten eine Formatversion; Testfälle, Läufe und Feinjustierungen werden eindeutig miteinander verknüpft und zusätzlich verständlich beschrieben. Kleine feste Tests und Dokumentation können in Git liegen. Umfangreiche Bildsammlungen und Laufdaten werden separat gesichert und von normaler Git-Aufnahme ausgeschlossen; Projektablage allein ist kein Backup. Den konkreten Sicherungsort beim Aufbau der Testwerkzeuge festlegen. Ein zur Feinjustierung verwendeter Fall darf danach nicht mehr als unabhängiger Abnahmefall gelten.

**Prüfung des Testwerkzeugs:** Aufzeichnung, eindeutige Bild-/Metadatenzuordnung, Markierungen, Speichergrenzen, unterbrochene Aufzeichnungen, Übertragung und Wiedergabe am Rechner prüfen. Erwartetes Verhalten und tatsächliches Ergebnis müssen auch nach der Übertragung nachvollziehbar bleiben.

### 10.5.2 Gemeinsames versioniertes Datenformat

**Nutzerentscheidung:** Testbildgenerator und Kameraaufzeichnung verwenden dieselbe Beschreibung ihrer Bilder: dieselben gemeinsamen Angaben unter denselben Namen und in denselben Formaten. Pro Aufnahme eine Bilddatei und eine zugehörige JSON-Datei vorsehen. Die Beschreibung enthält Formatversion, eindeutige Aufnahme-ID, Bilddatei, Herkunft, Bildgröße, Aufnahme- beziehungsweise Erzeugungsbedingungen und erwartetes Verhalten, soweit bekannt. Unbekanntes bleibt ausdrücklich unbekannt; Kameraaufnahmen erhalten keine erfundenen Sollfarbwerte.

Die verbindliche maschinenlesbare Formatbeschreibung liegt unter [tests/schemas/](tests/schemas/README.md). Der erste gemeinsame PNG-Vertrag ist [aufnahme-v1.schema.json](tests/schemas/aufnahme-v1.schema.json). Ein Schema prüft Struktur und Datentypen, nicht die Richtigkeit eines behaupteten Sollbefunds. Die Überprüfung der Erwartung bleibt gemäß dem Testkonzept erforderlich.

**Strikte Trennung:** Tatsächliche Testergebnisse in separaten Ergebnisdateien speichern und über Aufnahme-ID sowie Datensatz- und Testlaufversion zuordnen. Ein Testlauf überschreibt niemals die vorab festgelegten Erwartungen. Das Schema der Ergebnisdateien vor deren Implementierung ebenfalls zentral festlegen.

**Gemeinsamer Leser und Beispiele:** Vor Implementierung beider erzeugender Komponenten je eine Generator- und Kamera-Beschreibung mit demselben Leser und Schema prüfen. Die angelegten Beispiele sind zunächst illustrative JSON-Dateien, keine echten Bilddatensätze. Vor Fertigstellung der Komponenten sind zugehörige erzeugte beziehungsweise reale Bilddateien und deren Wiedergabe zu prüfen.

**Vollständige Live-Aufzeichnung:** Das Einstiegsschema ersetzt nicht die Anforderungen an Originalpuffer, Kamera-Metadaten, Zeitbasen, Transformationen und Bildfolgen aus dem vorherigen Abschnitt. Diese Angaben vor Implementierung der Aufzeichnung ausdrücklich in einem versionierten Vertrag ergänzen. Änderungen an Struktur oder Bedeutung erhalten eine neue Formatversion; keine stillschweigende Umdeutung alter Datensätze.

### 10.6 Referenzgenauigkeit und „ähnlich nah“

Für die Freigabe als Hilfe beim Nachmischen unabhängig gemessene physische Farbpaare mit dokumentierter Beleuchtung, Messgeometrie und Farbraumkonvention verwenden. Mittleren Fehler, 95. Perzentil und Rangfolgefehler getrennt von der Wiederholbarkeit auswerten. Akzeptable Fehlertoleranzen und verfügbare Referenzen **vor** dieser Abnahme festlegen; die Quellen liefern dafür keine endgültigen Zahlen.

Gerätebezogene Farbkorrektur nur bei nachgewiesenem Bedarf und mit getrennten Kalibrier- und Prüfmustern. Eine Endanwender-Kalibrierfunktion ist für die erste Umsetzung nicht beschlossen. Der Nullvergleich ist Teil der verbindlichen Sollwert-Testreihe aus §10.3; er liefert allein keine allgemeine ±-Unsicherheit. Die konkrete Anbindung der Test- und Messbefunde an „ähnlich nah“ bleibt als spätere Ausgestaltung in K6 erhalten.

### 10.7 Betrieb und Leistung

**Anzeige und Bedienung der ersten Version prüfen:** Lesbarkeit der Zahlen und ihrer kontrastreichen Hintergründe auf hellen, dunklen und unruhigen Kamerabildern sowie bei vergrößerter Systemschrift prüfen. Die Hervorhebung des besten Feldes muss anhand von Rahmen und Text verständlich bleiben, ohne allein von Farbe abzuhängen. Auf einem realen Android-Gerät mit TalkBack prüfen, dass normale Bedienelemente verständliche Namen und gegebenenfalls ihren Schaltzustand liefern und bedienbar sind. Laufende Messwertänderungen dürfen keine automatischen Ansagen auslösen. Eine sprachgestützte Messdurchführung oder Messausgabe ist kein Abnahmekriterium der ersten Version.


Den Bedienweg aus §5.1 bei bereits gemeldeter fehlender Unterstützung und bei erst im Betrieb fehlgeschlagener Gegenprüfung testen: konkrete Meldung, erreichbare App-Einstellungen, keine eigenmächtige Umschaltung, bewusste Auswahl und Speicherung, Rückkehr zur Messansicht sowie erneute Freigabe ausschließlich nach erfolgreicher Prüfung. Ohne geeigneten Messbetrieb bleibt die Messung mit verständlichem Grund gesperrt.

Zehn Minuten Dauerbetrieb; mindestens 20 Hintergrund-/Vordergrundwechsel; verweigerte Berechtigung; alle unterstützten und ausdrücklich geprüften Kamera-Schalterkombinationen. Kein fortlaufend wachsender Speicher, keine blockierte Kamera wegen offener Bilder. Tatsächliche Analysefrequenz, Allokationen, Framealter und Verzögerungen dokumentieren.

Nach Motorola-Prototyp beziehungsweise dem bestätigten ersten Gerät folgt eine zweite Gerätefamilie, im Ausgangsplan Samsung. Die konkrete Geräteauswahl und Unterstützung bleiben offen. Aus einem erfolgreichen Gerätetest folgt keine pauschale Android-Freigabe.

*Herkunft: TK §10–12; KP §5–6, §8; AS §4.4, §4.8, §5.1, §5.7, §8; OP §3.4–3.5, §4.3, §6–8.*

## 11. Bereits vorgeschlagene Vergleichsversuche

AS formuliert sechs Versuche. Sie werden vollständig erhalten, aber nicht fälschlich als sechs von Opus ausdrücklich beschlossene Arbeitspakete ausgegeben. Gemeinsam geforderte Prüfgegenstände sind in Kapitel 10 enthalten; die spezielle Versuchsausgestaltung bleibt dort, wo nur AS sie nennt, **erhalten, nicht beschlossen**.

| Quelle / Versuch | Vorhandene Durchführung | Welche offene Aussage er betrifft |
|---|---|---|
| AS V1: Position und Referenznähe | Mitte/nah, Mitte/fern, Rand/nah, Rand/fern; dasselbe Paar und gleiche Einstellungen; danach Schatten/Gradienten hinzufügen; Wiederholbarkeit, Rangfolge, verfügbare Referenzabweichung messen | K1/K3; Position und Nähe nicht gleichzeitig ändern und alles der Mitte zuschreiben |
| AS V2: Flächenstatistik | Gleiche Puffer mit bekanntem Text-/Störanteil: robuster linearer Mittelwert gegenüber Lab-Median/MAD; MAD = 0, helle und strukturierte Flächen; Fehler und Ausfallquote | O07; keine Siegerbehauptung ohne Daten |
| AS V3: Zeitaggregation | Null-Gegenfall sowie gepaarte Reihen mit Reflexen/Schatten/gemeinsamer Helligkeitsänderung; ungepaarte Extremwahl gegen gültige Frame-Abstände | Gemeinsam abgelehnte Entpaarung als Regression absichern; keine Wiederzulassung ihrer Produktnutzung |
| AS V4: Kamerastart/Fokus | AE-/AWB-Schalterkombinationen gemäß K4, Fokuszustände und Capture-Parameter am echten Gerät; feste Werte unter vorgesehenen Lichtlagen prüfen. Der frühere Vergleich mit einem Einregel-/Lock-Start entfällt. | Umsetzung von K4, O18 und Fokusstrategie |
| AS V5: Anzeige/Reaktionszeit | Eine/zwei Nachkommastellen, Zahlenhintergrund/Markierung; abrupter Musterwechsel; erste gültige Anzeige, aktualisierte Zahl und Rangwechsel getrennt messen | Anzeige als gemeinsamer Versuch, konkrete Ausgestaltung aus AS |
| AS V6: Grenzen des Nullvergleichs | Nach gleichen Flächen verschiedene Farbpaare; absichtlich fehlerhafte Farbtransformation, die gleiche Farben weiterhin gleich abbildet | K6; Nullvergleich darf einen solchen Fehler nicht als allgemeine Genauigkeit legitimieren |

Ein Gerätetest hat hier noch nicht stattgefunden. Diese Versuche sind bestehende Vorschläge, keine nachträglich behaupteten Resultate und kein Ersatz für die offenen Produktentscheidungen.

## 12. Umsetzung in überprüfbaren Schritten

**Verbindliche Reihenfolge:** Nach aktueller Nutzerpriorisierung zuerst die Iro-Bildanalyse und die lokale API für IroGen entwickeln, anschließend die erste Android-Hauptoberfläche. Danach im Emulator technisch vollständig grün werden. Dazu gehören Haupt-UI, Einstellungen-UI, deren Verdrahtung und der funktionierende Messablauf mit generierten Testbildern und Bildfolgen. Danach erst native Kamera und reales Gerät. Die bisherigen Meilensteinkennungen bleiben für bestehende Verweise erhalten; die Reihenfolge dieser Tabelle ist maßgeblich, nicht ihre Nummerierung.

| Schritt | Arbeit und Ergebnis | Abnahme / Entscheidungsbindung |
|---|---|---|
| **M0 – Umgebung und Grundgerüst** | Android-Solution und Emulatorbetrieb; Git-Einrichtung übernimmt der Nutzer | App startet im Emulator; Build funktioniert. Leere Testläufe sind keine fachliche Abnahme. |
| **M2/M3 – Datenverträge, Testdaten und Rechenkern** | Aufnahme- und Ergebnisformate vervollständigen; generierte Bilder/Bildfolgen mit geprüften Erwartungen; Farbberechnung, Puffer, Erkennung beider Streifenrichtungen und Qualitätsprüfung implementieren | Referenztests, geeignete und ungeeignete Muster sowie Störfälle erfüllen vorher festgelegte Sollbefunde; unabhängige Abnahmedaten getrennt halten. |
| **M4 – Vollständiger Ablauf im Testmodus** | Hauptoberfläche, Einstellungen, Bildanzeige mit Overlay, Referenzzuordnung, Messwerte, Rangfolge, Sperren und Hinweise verbinden; Einstellungen speichern und im Testablauf anwenden | Der gesamte Ablauf funktioniert im Emulator mit der produktiven Analyse und Oberfläche. Kein bloßes Durchreichen vorgegebener Testergebnisse. Offene Produktentscheidungen vor der betroffenen verbindlichen Umsetzung klären. |
| **Technische Abnahme – Übergang zum realen Gerät** | Vollständige technische Testreihe einschließlich unabhängiger Fälle, UI-Abläufe, Einstellungswechsel, Bildfolgen und Regressionen ausführen | Alle vereinbarten technischen Abnahmekriterien erfüllt; keine offenen abnahmeverhindernden Fehler. Erst dann Kameraintegration beginnen. |
| **M1 – Kameraintegration und Kameraversuch** | Native Vorschau, ImageReader, Fähigkeiten, CaptureResult, Lebenszyklus und feste Diagnoseflächen; AE/AWB standardmäßig aus gemäß Nutzerentscheidung | Tatsächliches Geräteverhalten, Framezuordnung, Fokus, feste Parameter und Freigabe prüfen. Technische Testfälle weiter als Regressionen verwenden. |
| **M5 – Reale Messvalidierung und Freigabe** | Reale Beleuchtung einschließlich Flimmern, Materialien, Wiederholbarkeit, Referenzvergleiche, Leistung, Lebenszyklus und zweite Gerätefamilie prüfen | Reale Fehlergrenzen und Eignung belegt; erforderliche Korrekturen erneut technisch und am Gerät prüfen. Keine Produktfreigabe allein aufgrund synthetischer Tests. |

Die mögliche Kippmessreihe bleibt eine offene Produktentscheidung, keine zusätzlich beschlossene Funktion. Geräteabhängige Parameter und Genauigkeitsaussagen werden erst in der Gerätephase validiert. Soweit Positionierung, Referenzmodell oder „ähnlich nah“ reale Befunde benötigen, werden im Testmodus ausdrücklich gekennzeichnete Versuchskonfigurationen verwendet; sie schließen offene Produktfragen nicht stillschweigend.

### 12.1 Nächste konkrete Arbeitspakete

**Aktueller Arbeitsauftrag, Fassung 1.27:** Für Bildoptimierung gilt der [schrittweise Arbeitsplan](docs/arbeitsplan-bildoptimierung.md). Jeden Punkt getrennt als umgesetzt, geprüft und abgenommen führen; erst danach als erledigt markieren. Abnahmefragen und Berichte nennen das konkrete Verhalten in Klartext. Die nachstehenden allgemeinen Aufgaben bleiben als übergeordneter Rahmen erhalten.

1. Gemeinsames Datenformat für Aufnahmen, Bildfolgen, Erwartungen und separate Testergebnisse vervollständigen und validieren.
2. Testeingabequelle vorsehen, die Bilder und simulierte, klar gekennzeichnete Aufnahmeinformationen in denselben Analyseweg einspeist wie später die Kamera. Testmodus ohne Kamerazugriff oder Kameraberechtigung betreiben.
3. Zuerst den Einzelbild-Rechenkern und die lokale API für IroGen ausbauen und prüfen; danach Hauptoberfläche und Einstellungen aufbauen und mit diesem Ablauf verbinden. Die getrennten AE-/AWB-Schalter bleiben standardmäßig aus. Im Testmodus ihre Speicherung und Weitergabe sowie simulierte Reaktionen prüfen; keine tatsächliche Sensorwirkung behaupten.
4. Farbrechenkern, Streifenerkennung, Referenzzuordnung, Qualitätsbewertung, Bild-/Bereichsablehnung, Hinweise und zeitliches Verhalten anhand geprüfter Testdaten entwickeln. Automatische Beseitigung beliebiger Bildstörungen ist nicht zugesagt.
5. Technische Gesamtabnahme durchführen. Erst danach Kameraeigenschaften, native Vorschau und Framezugriff am realen Gerät integrieren und prüfen.

**„Technisch vollständig grün“:** Alle für die technische Phase vereinbarten Anforderungen bestehen ihre Prüfungen. Ein erfolgreicher Build, ein leerer Testlauf oder eine Oberfläche mit vorgegebenen Zahlen genügen nicht. Kriterien und Toleranzen vor der jeweiligen Abnahme festlegen; Fehlerfälle, gültige Gegenbeispiele und unabhängige Testdaten einbeziehen. Noch ausstehende physische Gerätenachweise getrennt dokumentieren, nicht als bereits bestanden ausweisen.

### 12.2 Was „fertig“ bedeutet

Für den Arbeitsplan zur Bildoptimierung gilt zusätzlich: Eine Aufgabe ist erst erledigt, wenn Umsetzung, bestandene Prüfung und ausdrückliche Nutzerabnahme mit Datum und Beleg dokumentiert sind. Die Annahme des Protokolls ist keine Abnahme einer Implementierung. Unabhängige Arbeit darf während ausstehender Abnahmen fortgesetzt werden.

Eine Funktion erfüllt Ablauf und Fehlerzustände und besitzt den passenden Nachweis: Mathematik durch Referenztests, UI und Overlay zunächst durch Testbilder und Bildfolgen im Emulator; Kamera und Übertragung des Ablaufs auf echte Frames anschließend durch Gerätetests. Bestätigte Parameter und begründete Änderungen in diesem Dokument beziehungsweise den daraus entstehenden Messprotokollen festhalten. Offene Punkte erst schließen, wenn Entscheidung oder Nachweis dokumentiert ist.

Nach M1 Capture-Befunde, Fokusstrategie, nutzbare Modi, Analyseauflösung und Startparameter festhalten. Fehlen nötige Funktionen auf dem ersten Gerät, ein zweites prüfen, bevor die Architektur unnötig erweitert wird. Die offene Produktsemantik nicht durch einen beiläufigen Workaround verändern.

## 13. Kernentscheidungen: zwei festgelegt, vier offen

**K2 und K4 sind durch den Nutzer verbindlich entschieden.** K1, K3, K5 und K6 bleiben zur weiteren Klärung erhalten. Die Kennungen bleiben für bestehende Verweise unverändert.

### K1 – Position und Suchraum

| Astra | Opus | Gemeinsamer verbleibender Kern |
|---|---|---|
| Vier Bildränder beibehalten; äußere 30 % als Suchraum. Referenznähe und zentrale Position als Versuch vergleichen. | Streifen direkt an die Messfläche nahe der Bildmitte; Suchraum als mittiges Band begrenzen. | Begrenzter Suchraum, einfache Geometrie, räumliche Licht-/Optikeinflüsse prüfen. |

TK sieht ungefähr eine halbe Bildfläche für die Referenzoberfläche vor, nicht einen halb so großen Messrahmen. OP verkürzt dies teilweise zu „halbbildgroßer Referenzfläche“; der konkrete TK-Messrahmen ist 15 % der kürzeren Bildseite. Diese Größen nicht verwechseln.

**Noch nicht abschließend entschieden:** Die Positionierung nahe der Bildmitte ist ein technisch begründeter Kandidat, keine festgelegte Vorgabe. Auch ein bestimmter zentraler Aufnahmebereich für Streifen und Referenz ist nicht beschlossen. **Nutzerentscheidung zum Vorgehen:** Positionierung und Suchraum erst während der Entwicklung anhand praktischer Versuche festlegen; jetzt keine rein theoretische Geometrie verbindlich vorgeben. AS V1 liefert einen möglichen Vergleich von Position und Referenznähe. Die Ergebnisse fließen vor der endgültigen Erkennungsgeometrie in M3 ein.

### K2 – Streifenrichtung der ersten Version – entschieden

**Nutzerentscheidung:** Horizontale und vertikale Streifen werden bereits in Version 1 unterstützt. Eine Beschränkung auf vertikale Streifen ist ausgeschlossen. Hochformat der App und Positionierung im Bild sind davon unabhängige Fragen. Erkennung und Abnahmesatz in M3 müssen beide Richtungen abdecken.

### K3 – Referenzmodell und Platzierung

| Astra | Opus | Gemeinsamer verbleibender Kern |
|---|---|---|
| Eine gemeinsame, zunächst mittige, verschiebbare Fläche; automatische nahe Vorschläge als Versuch | Automatisch neben dem Streifen, manuell korrigierbar; KP legt pro Feld flankierende Bänder an | Sichtbare Referenz, manuelle Korrektur und Überlappungsschutz; Nähe verdient Untersuchung. |

Eine einzige gemeinsame Referenz beantwortet „welches Feld passt zu dieser gewählten Fläche?“. Eigene lokale Referenzen beantworten „welches Feld passt zu seiner jeweiligen Nachbarfläche?“. Bei inhomogener Wand sind das verschiedene Rangfolgen. OPs abschließende Kurzform klärt nicht ausdrücklich, ob die KP-Referenzen je Feld durch eine gemeinsame ersetzt werden sollen. Deshalb nicht als bereits vereinheitlicht behandeln.

**Noch zu entscheiden:** Gemeinsame oder feldweise Referenz; Standardplatzierung und Bewegung; gegebenenfalls benötigte Homogenitätsprüfung. Keine Wahl allein nach größerem L*. **Vor produktiver Referenzzuordnung in M3/M4.**

### K4 – Kamerastart und Einstellungen – entschieden

**Nutzerentscheidung:** Automatische Belichtung und automatischer Weißabgleich sind separat einstellbar und standardmäßig ausgeschaltet. Die gespeicherten Einstellungen gelten bereits beim Kamerastart. Schaltet der Nutzer eine Automatik ein, bleibt sie aktiv. Zunächst automatisch einregeln und anschließend sperren ist ausdrücklich kein vorgesehener Startablauf.

**Technisch noch zu lösen:** Geeignete feste Parameter bei ausgeschalteten Automatiken sowie tatsächlich unterstützte Kombinationen und Lichtbedingungen (O18). ISO 100/1⁄100 s ist nur ein Versuchswert. Diese Umsetzungsfragen werden in M1 geprüft und stellen die festgelegte Schaltersemantik nicht erneut zur Entscheidung.

### K5 – Laufende Messung oder Kippmessreihe

| Astra | Opus | Gemeinsamer verbleibender Kern |
|---|---|---|
| Kontinuierliche Live-Werte, ungültige/alte Werte verschwinden; Kippen nur nach belegtem Nutzen optional erwägen | Kippaufforderung und Mehrwinkelreihe beibehalten, nun mit frameweisen ΔE-Paaren | Reflexe und Winkel ernst nehmen; erst innerhalb eines Frames vergleichen; danach robust aggregieren. |

KP sieht ungefähr 2–3 Sekunden, anschließend festgehaltenes Ergebnis bis Reset vor. OP unterstützt Kippen, löst aber das Verhältnis zwischen diesem Festhalten und dem übernommenen Live-Gültigkeitsmodell nicht ausdrücklich auf. Das ist kein schon beschlossener gemeinsamer Ablauf.

**Noch zu entscheiden:** Ob überhaupt Produkt-Messreihe, gegebenenfalls Start automatisch/per Button, Stabilitätsanzeige, Abschluss, sichtbare Unterscheidung vom Live-Bild und Reset. Die Gewichtung nach einem Glanzmaß ist nicht spezifiziert. **Vor endgültiger Ablaufsteuerung M4.**

### K6 – Testansatz festgelegt; „ähnlich nah“ später konkretisieren

**Nutzerentscheidung:** Zunächst mehrere hundert Testbeispiele mit bekannten Sollwerten über den untersuchten Bereich von 0 bis N erzeugen, Fehler systematisch auswerten und die Erkennung beziehungsweise Farbauswertung anhand der Befunde verbessern (§10.3). Der Nullvergleich ist einer dieser Fälle. Die Bewertung wird nicht auf ausschließlich identische Farbpaare gestützt; die frühere Gegenüberstellung der KI-Positionen ist damit für den Testansatz erledigt.

**Später konkretisieren:** Aus den Test- und realen Messbefunden Verfahren, Grenzen und Geltungsbedingungen für „ähnlich nah“ ableiten und validieren. Jetzt wird weder die frühere `0,20-ΔE00`-Grenze verbindlich gesetzt noch eine allgemeine ±-Unsicherheit aus einem einzelnen Nullvergleich behauptet. Ob eine weitere Kalibrierfunktion nötig ist, ergibt sich aus den Befunden. Die Gruppierungsregel ist vor ihrer endgültigen Umsetzung in M4, die Genauigkeitsbewertung vor der Freigabe in M5 festzulegen.

## 14. Weitere erhaltene offene Punkte

Diese Liste bewahrt kleinere Fragen und einseitige Ausarbeitungen. Sie sind nicht stillschweigend verworfen, aber auch keine zusätzlichen beschlossenen Funktionen.

| ID | Offener Inhalt / überlieferte Alternativen | Zeitpunkt / Herkunft |
|---|---|---|
| **O01** | Tatsächlich verfügbares Motorola-Modell, Android-Version, Hauptkamera und weiteres Samsung-Gerät bestätigen; Geräteunterstützung definieren | Vor M1, zweite Familie nach Prototyp. TK F01/F10; KP §9.2; OP §7 |
| **O02** | Kompatibler SDK/MAUI/JDK/Build-Tools-Stand, Mindest-Android-Version; API 24 nur Ausgangswert | M0/M1. TK §2/F01; KP §9.1; OP §2.7/§7 |
| **O03** | Fokusstrategie und unterstützte Fokusdistanz/Locks; NR/EDGE OFF oder FAST als KP/OP-Kandidat; tatsächlicher Effekt statt Zusage | M1. TK §5; KP §3.1; AS §4.8; OP §4.2 |
| **O04** | Bedarf und Verfahren von Flatfield-/`LENS_SHADING_MAP`-Korrektur; bisher kein ausgearbeitetes gemeinsames Verfahren | Nach Ortsprüfung, vor einer darauf beruhenden Korrektur. KP §9.4; OP §8.1; AS §8 V1/V6 |
| **O05** | Ohne Streifen nur Hinweis oder manuelle Zwei-Zonen-Funktion? TK schließt versteckten Punktfallback aus; KP lässt Wahl offen | Vor Produktfallback, kein Hindernis für Diagnoseflächen. TK §7; KP §9.8 |
| **O06** | CIE76/94 im Core verpflichtend oder nur bei Bedarf im Entwicklerwerkzeug? Keine Formelauswahl für Nutzer | Bei konkretem Testbedarf. AS §9; OP §2.6 |
| **O07** | Robuster linearer RGB-Mittelwert als gemeinsamer Startkandidat; Lab-Median/MAD als von AS gewünschter, von OP produktiv abgelehnter Vergleich; MAD-Nullfall und Ausschlussregeln konkretisieren | Vor Statistikabnahme M2/M3. AS §5.3/V2; OP §3.4 |
| **O08** | Endgültige Erkennungs-/Innenflächengrößen, Clipping-, Text-, Streuungs- und Schärfegrenzen; Felder ohne Trennlinien und durchgehende Verläufe | M3/M4 mit echten Aufnahmen. TK F05/F06; KP §9.7 |
| **O09** | Erststartanleitung, kontextbezogene Hinweise, endgültiger Wortlaut, Lesbarkeit, Icon und visuelle Gestaltung | M4/Feinschliff. UR §5; TK F07; KP §8 M8, §9.15 |
| **O10** | Geräteausrichtung während der Sitzung; später Querformat/mehrere Streifen; Display wachhalten; Start einer etwaigen Messreihe automatisch/per Button | Hochformat/ein Streifen bleiben Startbasis; übriges vor entsprechender Funktion. TK F12; KP §9.9–11 |
| **O11** | Endgültige App-ID, Signierung, Play Store oder Sideload/APK; bis dahin Debug-Build | Vor externer Verteilung. TK F11; KP §9.12 |
| **O12** | Einordnung von Stoff/Holz/Kunststoff in Version 1 und materialgerechte Hinweise; kein stiller Materialerkennungsumfang | Vor Produktfreigabe. TK §1; KP §9.16; AS §4.2 |
| **O13** | Nur Deutsch oder zusätzlich Englisch; Dark Mode | Vor UI-Abschluss, kein vorab entschiedener Zusatzumfang. KP §9.13–14 |
| **O14** | Konkrete Arbeitsweise mit KI-Werkzeugen des Entwicklers | Später festlegen; kein neues Agenten-/Orchestrierungssystem aus diesem Plan ableiten. UR §5; KP §9.17 |
| **O15** | Mischlicht: Fälle vorhanden, aber keine eigenständige ausformulierte Korrektur-/Sperrregel; Metamerie-Hinweis als OP-Vorschlag | Reale Tests/Nutzerführung. KP §6.3; OP §8.2–3 |
| **O16** | Sehr dunkle und gesättigte Farben: allgemeine Clipping-/Qualitätsregeln vorhanden, konkrete Grenzwerte fehlen | Gerätesatz M3/M5. TK §9; OP §8.5 |
| **O17** | OPs zusätzlicher Vorschlag „kein Feld passt ausreichend“ oberhalb einer Grenze | Nicht gemeinsam beschlossen; ohne absolute Eignungsgrenze keine solche Aussage. OP §8.4 gegenüber TK §8/AS §4.5 |
| **O18** | Richtiges YUV-/SDR-Profil, endgültige Auflösung, gegebenenfalls niedrig aufgelöste Suche; unterstützte Kamera-Kombinationen und feste Werte | M1/M2. TK F02–F04; KP §9.3/5; AS §6 |
| **O19** | Verfügbare unabhängige Referenzmessungen, akzeptabler Fehler beim Nachmischen, Bedarf und Pflege einer Geräte-Farbkorrektur | Vor M5-Freigabe. TK F08/F09; AS §4.4; OP §4.3 |

Die ursprünglichen TK-Fragen F01–F12 sind damit abgedeckt: F01→O01; F02/F03→K4/O18; F04→O18; F05/F06→O08; F07→Kapitel 2/K6/O09; F08→O19; F09→K6/O19; F10→O01; F11→O11; F12→O10. KP §9.1–17 ist über O01–O14, O18 und K2/K4 vollständig zugeordnet.

## 15. Nicht übernommene Ansätze und korrigierte Aussagen

Ein Ausschluss bedeutet hier: keine verbindliche positive Implementierungsvorgabe. Wo die andere KI daran festhält, bleiben Inhalt und Konflikt in Kapitel 13/14 erhalten. Die Tabelle trennt gemeinsame Ablehnung von einseitiger Kritik.

| Ansatz / Behauptung | Behandlung und Grund | Quellenlage |
|---|---|---|
| Version 1 unterstützt nur vertikale Streifen | Nicht übernehmen; horizontal und vertikal sind verbindlich | Nutzerentscheidung K2, Fassung 1.1 |
| Automatisches Einregeln und anschließendes Sperren als Kamerastart | Nicht übernehmen; gespeicherte Automatikschalter gelten ab Start, standardmäßig aus | Nutzerentscheidung K4, Fassung 1.1 |
| Farbcodes speichern, Ergebnisliste, OCR, Kataloge | Aus dem historischen Umfang entfernt; Nutzer liest/markiert selbst | Beide Ausgangspläne/Analysen |
| Nutzer wählt nur ein Farbfeld zum Vergleich | Durch gleichzeitige Bewertung aller geeigneten Felder ersetzt | Beide |
| Einzelpixel als normale Farbmessung | Durch innere Messflächen ersetzt | Beide |
| Skala 0–10, Prozent oder universelle Sichtbarkeitstabelle | Kein gemeinsamer sinnvoller Produktinhalt; ΔE00 direkt, relative Bedeutung | Beide; Skala 0–10 laut AS spätere Diskussion, nicht UR |
| Getrennt dunkelstes Musterquartil und hellstes Wandquartil | Verletzt Frame-Paarung, erzeugt künstliche Unterschiede | AS §4.1 und OP §5.1 stimmen in Korrektur überein |
| Formelauswahl CIE76/94/00 im Nutzerbildschirm | Fachlich unnötige Bedienkomplexität; ΔE00 fest | AS §3/§9 und OP §2.6 |
| Fehlende Workload-ID beweist fehlende MAUI-Installation | Fehlschluss; tatsächlichen Build und Installationsweg prüfen | AS §4.6 und OP §5.1 |
| `LEGACY`-Warnung ersetzt Fähigkeitenprüfung | Einzelne Fähigkeiten und Capture-Wirkung prüfen; ungeeigneten Modus sperren | AS §4.6; OP übernimmt Prüfung aus TK |
| Unveränderte 640-Pixel-Grenzen unabhängig von Feldgröße | Messauflösung an Mindestgrößen binden; kleinere Suche bleibt Versuch | AS §6; OP §2.5 |
| Helles, unbuntes Band ist immer Separator | Kann legitime helle Farbfelder entfernen; Geometrie/Struktur hinzunehmen | Explizite Korrektur AS §4.7; OP übernimmt TK-Erkennungsanforderungen |
| Zwei stärkste Kanten sind automatisch der Streifen | Mehrere plausible Kandidaten prüfen | Explizite Korrektur AS §4.7 |
| Höheres Referenz-L* oder dunkleres Muster ist allgemein richtiger | Keine automatische allgemeine Reflex-/Schattenkorrektur | AS §4.2–4.3 widerspricht KP/OP; K3/K5 bleiben offen |
| Einmaliger Nullvergleich liefert allgemeine Geräteunsicherheit und beweist Lock | Als vollständige Aussage nicht übernehmen; Diagnose bleibt | AS §4.4 widerspricht OP §4.3; K6 |
| Jeder Nullvergleich zeigt nur Systemfehler | Material, Licht und Geometrie können unterschiedlich sein; unterschiedliche Farbpaare zusätzlich nötig | AS §4.4; K6 |
| Gemeinsame Pipeline garantiert belastbare Abstände/Rangfolge | Nicht als Nachweis verwenden; reale unabhängige Validierung | AS §4.5 korrigiert KP §1.3 |
| Deckel bei 99,9 | AS lehnt stilles Abschneiden ab; keine gemeinsame Produktvorgabe | AS §3/§4.5 gegen KP §3.9 |
| Feste 0,20 als allgemeine Genauigkeitsgrenze | OP lehnt festen Wert ab; TK hatte ihn lediglich als UI-Stabilisierung vorgesehen | K6, nicht heimlich durch anderes erfundenes Maß ersetzt |
| Zwei Nachkommastellen als normale Präzisionszusage | Gemeinsamer neuer Startversuch: eine Stelle; Diagnose darf mehr zeigen | AS §5.6/§6 und OP §2.4 |
| ISO 100, 1/100 s oder universelle Gains als fertiger Alltagsstart | Nur gerätebezogener Versuch; Lichtlage und Unterstützung prüfen | AS §6 und OP §2.2 |
| `AF_MODE_OFF` allein hält sicher die vorherige Schärfe | Konkrete Fokusdistanz/Lock und Zusammenspiel prüfen | AS §4.8 |
| YUV als unverarbeitete Sensor-RAW-Daten | Begriff korrigiert; ISP-Einfluss bleibt | AS §9 |
| Pauschal immer BT.601/full range korrekt | Nur KP-Startannahme; verifizieren, nicht als geprüft liefern | TK §6, OP §6; AS §9 |
| Vorgezogener Kameraversuch vor vollständigem technischem Testmodus | Durch Nutzerentscheidung abgelöst: zunächst technische Abnahme mit generierten Daten, danach Kameraintegration und reale Tests | Nutzerentscheidung, Fassung 1.16 |
| Bibliothek sicher niemals nötig; C# vor Profiling bewiesen schnell | Keine Garantie; zunächst ohne Zusatzbibliothek, Leistung messen | AS §5.2/§5.5 |
| Toolkit-`CameraView` generell ohne brauchbare Fähigkeiten | Ohne Paket-/API-Version keine pauschale Begründung; eigener Handler bleibt gewählt | AS §9 |
| 300 ms gleich gesamte sichtbare Reaktionszeit | Frame-Latenz, Historienfenster und Rangwechsel getrennt messen | AS §6 |
| Ursprungs-PDF habe subjektive Nutzerkalibrierung verlangt | Historisch falsch: UR nennt rechnerisch erwartete Referenzwerte | AS §9; UR §3.5 |
| „Keine Persistenz“ schließt auch Einstellungen aus | Präzisiert: keine Ergebnisverwaltung; lokale Einstellungen zulässig | AS §9; TK §12 |

## 16. Abdeckungsnachweis der Zusammenführung

Dieser Nachweis ordnet auch abgelehnte, widersprüchliche und nur vorgeschlagene Inhalte zu. „Abgedeckt“ heißt nicht, dass jede Ursprungsbehauptung als richtig bestätigt oder jede Idee implementiert wird.

### 16.1 Ausgangspläne und weitere Dateien

| Quelle / Abschnitt | Inhalt in diesem Dokument |
|---|---|
| TK Vorspann, §1 | Kapitel 0–2; offene Konflikte K1/K3/K5; Entscheidungen K2/K4; Materialumfang O12 |
| TK §2–3 | Kapitel 3–4; Versionsangaben ausdrücklich als übernommener Befund |
| TK §4–6 | Kapitel 3, 5–6; K4 und O03/O18 |
| TK §7 | Kapitel 7; K1–K3; O05/O08 |
| TK §8 | Kapitel 8; K6/O06/O07; Ausschlüsse Kapitel 15 |
| TK §9 | Kapitel 2, 9 |
| TK §10 | Kapitel 10–11 |
| TK §11 | Kapitel 12; ursprüngliche DEV/CAM/COL-Kennungen erhalten |
| TK §12 / F01–F12 | Kapitel 1 und 13–14 samt expliziter F-Zuordnung |
| TK §13 | Kapitel 0, 17; keine behauptete neue externe Quellenprüfung |
| KP §1–2 | Kapitel 1–2; K1/K3–K5 |
| KP §3.1–3.2 | Kapitel 5–6; O03/O18; K4 |
| KP §3.3–3.6 | Kapitel 7–8; O07; K1–K3 |
| KP §3.7 | Kapitel 8, 10–11; K5; Ausschluss ungepaarter Quartile |
| KP §3.8–3.9 | Kapitel 2, 8; O06; Ausschlüsse Formelwahl/Schwellen/Deckel |
| KP §4–5 | Kapitel 2, 5, 10; K5/K6 |
| KP §6 | Kapitel 10–11; spätes Realtesten und allgemeine Selbstkalibrierung nicht übernommen |
| KP §7 | Kapitel 3–6; Workloadfehlschluss und pauschale Garantien in Kapitel 15 |
| KP §8 / M0–M8 | Kapitel 12; Mehrwinkel und Kalibrierfunktion an K5/K6 gebunden; Feinschliff erhalten |
| KP §9 / Punkte 1–17 | Kapitel 13–14 |
| KP §10 | Kapitel 15; historische Herkunft korrigiert |
| UR §1–2 | Ziel und gemeinsamer Aufnahmezeitpunkt erhalten; Code-/Speicherablauf und Einzelfeldauswahl ersetzt |
| UR §3.1–3.4 | Kamerakontrolle, ΔE, Erkennung und Nutzerführung in Kapitel 2, 5–8; Position/Orientierung als Konflikt erhalten |
| UR §3.5 | Referenzbasierte Tests und große künstliche Datensätze in Kapitel 10; KI-generierte Störbilder durch Nutzerergänzung in §10.4.1 verbindlich, numerisch kontrollierte Farbtests zusätzlich erhalten |
| UR §4–5 | Schlanke App, erheblicher realer Feinjustierungsbedarf, Nutzerführung und KI-Werkzeugfrage in Kapitel 1, 10, 12, O14 |
| NA | Kopf des Dokuments und Kapitel 0/3/4; IPA erhalten |
| TP/RW | Satzfassung/Renderer von TK; Diagramminhalte durch Bedienungs- und Datenflussbeschreibung in Kapitel 2–3 abgedeckt; keine zusätzliche Planungsdatei nötig |

### 16.2 Vorrangige Bewertungen

| Bewertung | Zuordnung |
|---|---|
| AS §1–3 / OP §0–2 | Quellenbereinigung, gemeinsames Fundament und Konflikte in Kapitel 0–3/13 |
| AS §4.1–4.3 | Paarung in Kapitel 8; Reflex-/Geometriefragen in Kapitel 5/13 |
| AS §4.4–4.6 | Nullvergleich Kapitel 10/K6, Aussagegrenzen Kapitel 1/15, Einrichtung/Fähigkeiten Kapitel 4–5 |
| AS §4.7–4.8 | Helle Felder/Profilverfahren Kapitel 7, Fokus/Realtests Kapitel 5/10 |
| AS §5.1–5.7 | Generator/Tests Kapitel 10, Statistik Kapitel 8/O07, Nähe K1/K3, Puffer Kapitel 3, Anzeige Kapitel 2, Materialien Kapitel 10 |
| AS §6 | Parameterstatus, Fokus, Latenz, Geometrie, Referenzwechsel und Reproduzierbarkeit in Kapitel 3–8/10 |
| AS §7–8 | Konsens/Konflikte Kapitel 13; alle sechs Versuche in Kapitel 11 |
| AS §9 | Historie, Persistenz, RAW, Formelvergleich, Toolkit-Aussage und fehlende Gerätemessung in Kapitel 0/1/6/8/15 |
| OP §3.1–3.8 | Zustände, Koordinaten, Lebenszyklus, Farbkern, Holdout, Projektstart, Kennzeichnung und Einzelanforderungen in Kapitel 0–10 |
| OP §4.1–4.4 | Reflexe/Schattierung Kapitel 5/10/K5; NR/EDGE O03; Unsicherheit K6; Generator und Nicht-Ziele Kapitel 1/10 |
| OP §5–6 | Korrigierte Quartile/Schwellen/Workloads in Kapitel 15; positive gemeinsame Teile im Fachplan, übrige Empfehlungen K1–K6/O03/O06/O07 |
| OP §7 | Geräte- und Umgebungsbefunde Kapitel 4, O01/O02 |
| OP §8.1–8.5 | Flatfield O04, Mischlicht/Metamerie O15, „kein Feld passt“ O17, dunkle/gesättigte Farben O16 |
| OP §9 | TK-Struktur als Grundlage, physikalische Prüfgegenstände erhalten; kein behaupteter Konsens bei weiterhin gegensätzlichen Empfehlungen |

## 17. Überlieferte technische Referenzen und Dokumentpflege

Die folgenden Links stammen aus den analysierten Quellen. Sie sind für die spätere Implementierungsprüfung erhalten. Sie wurden im Rahmen dieser rein quellengebundenen Zusammenführung nicht erneut auf ihren aktuellen Inhalt geprüft. Keine der hier verwendeten Projektgrenzen wird allein dadurch zur Normvorgabe.

| Referenz | Verwendung laut Ausgangsdokumenten |
|---|---|
| [Microsoft: MAUI-Installation](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation?view=net-maui-10.0) | Installation, Workload-IDs, SDK-/JDK-Wege |
| [Microsoft: MAUI-Support](https://dotnet.microsoft.com/en-us/platform/support/policy/maui) | Unterstützte Versionen und Patchstand bei M0 prüfen |
| [Microsoft: Neuerungen MAUI 10](https://learn.microsoft.com/en-us/dotnet/maui/whats-new/dotnet-10?view=net-maui-10.0) | Überlieferte Android-/JDK-Basis |
| [Android: CameraCharacteristics](https://developer.android.com/reference/android/hardware/camera2/CameraCharacteristics) | Fähigkeiten, Modi, Bereiche, Streamkombinationen |
| [Android: CaptureRequest](https://developer.android.com/reference/android/hardware/camera2/CaptureRequest) | AE/AWB/AF, manuelle Werte, Locks |
| [Android: ImageReader](https://developer.android.com/reference/android/media/ImageReader) | Framezugriff, Warteschlange, Freigabe |
| [Android: CaptureResult](https://developer.android.com/reference/android/hardware/camera2/CaptureResult) | Tatsächlich angewendete Aufnahmeparameter |
| [Android: YUV_420_888](https://developer.android.com/reference/android/graphics/ImageFormat#YUV_420_888) | Ebenen und Strides |
| [ICC/W3C: sRGB](https://www.w3.org/Graphics/Color/srgb) | Übertragungsfunktion, D65, XYZ-Transformation |
| [Sharma, Wu, Dalal: CIEDE2000](https://hajim.rochester.edu/ece/sites/gsharma/ciede2000/) | Formelhinweise und Referenztestpaare |
| [NIST: Unsicherheitsanalyse](https://www.itl.nist.gov/div898/handbook/mpc/section5/mpc52.htm) | AS-Abgrenzung von Wiederholbarkeit und Messunsicherheit |
| [NIST: Streuungsmaße](https://www.itl.nist.gov/div898/handbook/eda/section3/eda356.htm) | AS-Präzisierung der MAD-Definition |
| [CIE: Validity of Formulae for Predicting Small Colour Differences](https://www.cie.co.at/publications/validity-formulae-predicting-small-colour-differences) | AS-Abgrenzung allgemeiner Sichtbarkeitsschwellen |

### Pflege dieser Arbeitsgrundlage

Diese Markdown-Datei bleibt die inhaltliche Quelle. Spätere Entscheidungen werden an den bezeichneten Stellen eingetragen und die Fassung erhöht. Messnachweise dürfen als Arbeitsartefakte entstehen, aber kein neuer paralleler, widersprüchlicher Konzeptstand. Eine optionale spätere PDF wäre nur Satzfassung desselben Inhalts.

**Verbindliche Dokumentationsregel:** Jede neue oder geänderte Festlegung kurz im Änderungsprotokoll festhalten: Datum, Entscheidung beziehungsweise Änderung, Begründung, betroffene Planfassung und nachvollziehbare Grundlage. Historische Quellen und Prüfaussagen ausdrücklich als historisch kennzeichnen. Frühere Änderungen nur belegbar rekonstruieren, als nachträgliche Rekonstruktion markieren und verbleibende Lücken benennen. Die ausführlichen Arbeitsregeln stehen in [AGENTS.md](AGENTS.md).

**Konkreter Arbeitsbeginn:** Iro-Einzelbildanalyse und lokale API für IroGen vor der ersten Android-Hauptoberfläche umsetzen und prüfen; danach Testeingabequelle sowie Haupt- und Einstellungsoberfläche mit dem Analyseweg verbinden. Kameraintegration und reale Gerätetests beginnen erst nach bestandener technischer Gesamtabnahme gemäß Kapitel 12. Die Git-Einrichtung übernimmt der Nutzer.

## 18. Änderungsprotokoll

Dieses Protokoll wird ab Fassung 1.11 fortlaufend geführt. Für die Fassungen 1.0 bis 1.10 liegt hier noch kein vollständiges nachträglich geprüftes Änderungsprotokoll vor. Historische Versionsverweise im Dokument bleiben erhalten; fehlende Einträge werden nur auf belegbarer Grundlage rekonstruiert.

| Datum | Planfassung | Entscheidung / Änderung | Begründung | Grundlage |
|---|---|---|---|---|
| 18. September 2026 | 1.10 → 1.11 | Verbindliche Dokumentationsregel in AGENTS.md und Plan aufgenommen; Änderungsprotokoll begonnen; ursprüngliche Quellen und Prüfaussagen als historisch gekennzeichnet. | Entscheidungen, Gründe und betroffene Fassungen nachvollziehbar halten; überlieferte Aussagen nicht als aktuelle Prüfung darstellen. | Ausdrücklicher Nutzerauftrag im laufenden Austausch: künftig Entscheidung, Begründung und Planfassung dokumentieren; historische Quellen kennzeichnen; frühere Änderungen nur belegbar rekonstruieren. |
| 18. September 2026 | 1.11 → 1.12 | Gemeinsame versionierte Aufnahmebeschreibung für Generator und Kamera festgelegt; PNG-Einstiegsschema, illustrative JSON-Beispiele und Strukturprüfer angelegt. Sollbefunde und tatsächliche Ergebnisse bleiben getrennt. | Inkompatible Metadaten und Überschreiben von Erwartungen verhindern; unbekannte Angaben nicht erfinden. | Nutzerfestlegung im laufenden Austausch zur gemeinsamen Bild-/JSON-Beschreibung; beide Beispiele mit demselben Leser geprüft, fünf ungültige Varianten abgewiesen. Vollständige Bildwiedergabe steht aus. |
| 18. September 2026 | 1.12 → 1.13 | Expliziten Gerätetest für Lichtflimmern, feste Belichtungszeiten und die Erkennung verbleibender Störungen ergänzt. | Das Zusammenspiel realer Beleuchtung und Kamera lässt sich durch Emulator und generierte Bilder allein nicht nachweisen; geeignete Parameter sind praktisch zu bestimmen. | Nutzerfreigabe im laufenden Austausch zur Ergänzung realer Gerätetests; Android-Dokumentation zur fehlenden Antibanding-Wirkung bei manueller Belichtung. |
| 18. September 2026 | 1.13 → 1.14 | Zurücksetzen der betroffenen Glättungshistorie bei erheblichen Änderungen tatsächlich angewandter Aufnahmeparameter verbindlich festgelegt; passende Zeitreihentests ergänzt. | Alte Aufnahmebedingungen dürfen die aktuelle Anzeige auch bei automatischer Nachregelung ohne Schalterwechsel nicht verfälschen; kleine Schwankungen sollen die Anzeige nicht ständig zurücksetzen. | Ausdrückliche Nutzerfreigabe im laufenden Austausch; Toleranzen sind während der Entwicklung anhand realer Bildfolgen zu bestimmen. |
| 18. September 2026 | 1.14 → 1.15 | Gut lesbare visuelle Messanzeige, Hervorhebung durch Rahmen und Text sowie TalkBack-zugängliche normale Bedienelemente für Version 1 festgelegt; passende Prüfungen ergänzt. Sprachgestützte Messdurchführung und Messausgabe zunächst ausgeschlossen. | Einen klar begrenzten ersten Funktionsumfang mit lesbarer Anzeige und verständlicher Bedienung umsetzen; keine Einschränkung anhand vermuteter Nutzergruppen begründen. | Ausdrückliche Nutzerfestlegung im laufenden Austausch zur visuellen Anzeige und zugänglich beschrifteten Bedienelementen. |

| 18. September 2026 | 1.15 → 1.16 | Reihenfolge verbindlich geändert: vollständige Haupt- und Einstellungsoberfläche, Verdrahtung und Analyse im Testmodus technisch abnehmen; danach Kameraintegration und reale Gerätetests. Frühere Vorgaben für frühe Kameraarbeit ersetzt. | Zunächst den gesamten technischen Ablauf mit kontrollierten Eingaben nachweisen, bevor reale Aufnahmebedingungen hinzukommen. | Ausdrückliche Nutzerentscheidung im laufenden Austausch: zuerst technisch komplett grün einschließlich UI und Testmodus, danach reales Gerät. |

| 19. September 2026 | 1.16 → 1.17 | IroGen als WPF-Anwendung unter `iro-gen` mit Optionen, zufälligen Farbabstufungen, Wandvarianten, Störeffekten, Sollwertaufdruck und PNG-/JSON-Export festgelegt und implementiert. Auswertung bleibt im Iro-Testablauf. Nominale Materialabstände und noch ungeprüfte Bild-/Freigabeerwartungen getrennt. | Kontrollierte, wiederholbare Wand-/Streifenbilder interaktiv erstellen und mit Iro vergleichen können; Störbilder dürfen keine unbelegten Mess- oder Ablehnungsgrenzen definieren. | Ausdrücklicher Nutzerauftrag vom 19. September 2026 samt Bildvorlagen; lokale Umgebungsprüfung; Generator-Referenz-, Rendering- und Exporttests, dokumentiert in `iro-gen/README.md` und `docs/irogen-pruefung.md`. |

| 19. September 2026 | 1.17 → 1.18 | IroGen um wählbare Bildserien, neue Zufallspalette pro Bild, systematische nominale ΔE00-Abdeckung, Fortschritt/Abbruch, Gesamtspeicherung und vorbereitete lokale Iro-Testaufträge erweitert. Spätere Analyseanzeige in IroGen vorgesehen, Darstellung und Rückweg bleiben offen. | 100/500 Bilder sollen den Farbabstandsbereich nachvollziehbar abdecken und als vollständiger Satz für Iro bereitstehen; reine Zufallswahl könnte wichtige Unterschiede auslassen. Keine vorgetäuschte Analyse bei noch fehlendem Iro-Analyseweg. | Ausdrücklicher Nutzerauftrag und Antwort „Testübergabe jetzt vorbereiten“; automatisierte Serien-, Export-, Übergabe- und WPF-Prüfungen, siehe `docs/irogen-pruefung.md`. |

| 19. September 2026 | 1.18 → 1.19 | Iro-Einzelbildanalyse und lokale .NET-API vor die erste Android-UI priorisiert; gemeinsamer Wandreferenz-Versuch pro Bild, separater Ergebnisvertrag, echter Serienlauf und vorläufige Istwertdiagnose in IroGen ergänzt. | Vorbereitete Testaufträge sollen tatsächlich gegen Bildpixel ausgewertet werden. Eingaben/Sollwerte und Ergebnisse strikt trennen; Versuch nicht mit Kamera- oder Produktfreigabe verwechseln. | Ausdrücklicher Nutzerauftrag „Bildanalyse mit einem API für IroGen … wichtiger als die erste UI“ und Antwort „ein Farbstreifen … bekommt seine Wand“; begrenzter technischer Regionsversuch und Nachweise in `docs/analyse-pruefung.md`. |


| 21. September 2026 | 1.19 → 1.20 | Testübergabe und Analyse vollständig in den Hintergrund verlagert; Busy-Indikator erst nach einer Sekunde; sichtbare Abschlussmeldung mit Explorer-Zugriff auf den konkreten Lauf ergänzt. | Kurze Tests sollen nicht flackern, lange Tests den UI-Thread nicht blockieren; Ergebnisdateien ohne manuelle Suche erreichbar machen. | Ausdrücklicher Nutzerauftrag im laufenden Austausch; fünf gezielte Prüfungen für schnelle/langsame Läufe, Fehler, Abbruch und WPF-Abschlussanzeige bestanden. |

| 21. September 2026 | 1.20 → 1.21 | Gemeinsame Ergebnis-/Hinweisanzeige in der Android-App verdrahtet; echter Einzelbild-Testmodus mit klarem und stark unscharfem Originalbild, Sperre/Entfernung ungültiger oder überholter Messwerte ergänzt. | Bestätigte Analysefehler sollen schrittweise beim Anwender sichtbar werden, statt nur im Testbericht zu stehen. | Ausdrücklicher Nutzerauftrag „genau das jetzt so verdrahten“; Original-Störlauf `iro-run-e96672920cee46fda9b0e7997daa16ab`; technische Prüfungen in `docs/android-hinweise-pruefung.md`. |

| 22. September 2026 | 1.21 → 1.22 | Konfigurierbare Testpläne mit Falloptionen/Bildanzahlen, kompakter Diagnoseexport und je drei Nah-/Fernstufen ergänzt; Fehlfreigabe geometrisch abweichender Fremdflächen im Einzelbildversuch korrigiert; tatsächliche Hinweise im Bericht erhalten. | Wiederholbare Testserien ohne manuelles Umstellen, kleinere Auswertungsdateien und gezielte Korrektur eines nachgestellten Verdeckungsfehlers. | Ausdrücklicher Nutzerauftrag im laufenden Austausch; neu erzeugte Vergleichsbilder und automatisierte Prüfungen in [Testpläne und Verdeckung](docs/testplaene-und-verdeckung.md). |

| 22. September 2026 | 1.22 → 1.23 | Auswertung und Export an die aktuelle Bildserie gebunden; fehlende Analyse beim Auswerten durchführen; Bildanzahl aus geladenem Testplan anzeigen. | Alte 30 Ergebnisse konnten trotz neu erzeugter 113 Bilder angezeigt werden; der Auswertungsbutton hatte keinen Bezug zur sichtbaren Serie. | Nutzerfehlerbericht mit vier Screenshots; Codeprüfung und vollständiger WPF-Ablauftest mit dem 113-Bilder-Plan, siehe [Prüfbericht](docs/testplaene-und-verdeckung.md). |
| 22. September 2026 | 1.23 → 1.24 | Räumliche Qualitätsprüfung für Felder und Referenz im Einzelbildversuch ergänzt; größere Feldinnenfläche geprüft, Diagnoseparameter und Schema erweitert, Unschärfehinweis erhalten. | Globale Streuung übersah sichtbare Helligkeitsverläufe; acht nominal auffällige Schatten-/Randabfallbilder werden nun gesperrt, alle 17 ungestörten Kontrollen bleiben auswertbar. Zusätzliche Sperren und Grenzen ausdrücklich dokumentiert. | Ausdrücklicher Nutzerauftrag zur Bildanalyseoptimierung; [reproduzierbarer Vorher-/Nachher-Prüfbericht](docs/raeumliche-flaechenpruefung.md), Nutzerbericht mit 113 Bildern und zusätzliche Kern-/Generatorprüfungen. |
| 22. September 2026 | 1.24 → 1.25 | Zufällige Streifenlage, getrennte Seiten-/Höhenperspektive und Wandabstand in IroGen ergänzt; 108-Bilder-Testplan mit kombinierten Hochhalte-/Bückfällen und aufgelöster Geometrie im Diagnoseexport. | Reale schwierige Aufnahmesituationen gezielt und reproduzierbar untersuchen; Modellannahmen von App-Grenzen und realer Messgenauigkeit trennen. | Ausdrücklicher Nutzerauftrag im laufenden Austausch; [Modell und lokaler Prüfstand](docs/raeumliche-aufnahmeszenarien.md). |
| 22. September 2026 | 1.25 → 1.26 | Automatisches Geraderichten auf die nächstgelegene waagerechte oder senkrechte Achse vor der Farbfeldsuche ergänzt; Messung bleibt auf Originalpixeln, Ergebnisse werden als Polygone zurückgeführt. | Beliebig in der Bildebene gedrehte Streifen sollen vor der bislang achsenparallelen Farbfeldsuche normalisiert werden, ohne Farben durch Interpolation zu verändern. | Ausdrückliche Nutzerentscheidung im laufenden Austausch; [Verfahren und reproduzierbare Prüfung](docs/gerade-richten.md). |

| 22. September 2026 | 1.26 → 1.27 | Nutzerprotokoll zur Bildoptimierung verbindlich übernommen: gemeinsame Messgrundlage, getrennter Erkennungspfad, erlaubte/bedingte/verbotene Verfahren, aufnahmeweite Sperren und vollständige Sichtbarkeit. Arbeitsplan mit Umsetzung, Prüfung und ausdrücklicher Abnahme in Klartext angelegt; AGENTS.md, Claude-Einstieg und Vertragsdokumentation angebunden. | Messprinzip schützen, bedingt zulässige Korrekturen nicht ungeprüft aktivieren und Fortschritt für alle Agenten nachvollziehbar halten. Neuere Vollständigkeits- und Clippingregeln ersetzen insoweit frühere Teilfreigaben. | Ausdrücklicher Nutzerauftrag und unverändert archiviertes [Entscheidungsprotokoll](docs/entscheidung-bildoptimierung-2026-09-22.md); noch keine Abnahme der neuen Implementierungsanforderungen. |
| 22. September 2026 | 1.27 → 1.28 | Tiefenprüfung mit Schwerpunkt Kunden-App vor weitere Optimierungen priorisiert; Analyse 0.5.0 sperrt erkannte unbrauchbare Unschärfe und relevante Kanalendpunkte aufnahmeweit sowie erkannte angeschnittene Streifen. IroGen trennt nominale Diagnose von fachlichem Urteil; App-Anzeige auf eine Nachkommastelle angepasst. Verbleibende Qualitäts-/Abnahmelücken ausdrücklich offen. | Gemeinsame Originalpixelmessung ist vorhanden, aber reproduzierbare Freigabeverstöße und überschätzte Testaussagen müssen zuerst beseitigt werden. Keine Kundenfreigabe aus Generatorläufen ableiten. | Ausdrücklicher Nutzerauftrag zur tiefgründigen Prüfung und vorgelagerten Behebung; [Codebefunde, Vorher-/Nachher-Tests und Grenzen](docs/regelaudit-2026-09-22.md). || 22. September 2026 | 1.28 → 1.29 | Erste geometrische Schutzlücken mit Analyse 0.5.1 geschlossen: kleine Randreste berücksichtigen und deutliche kohärente Verjüngung sperren; allgemeine Perspektiv- und Abnahmeaufgaben bleiben offen. | Acht vorab reproduzierte Fehlfreigaben beheben und geeignete Gegenbeispiele erhalten; technische Versuchswerte nicht als Winkel- oder Messgütezusage ausgeben. | Nutzerauftrag zum Start des neuen Plans; [unabhängige Gegenproben und Prüfnachweis](docs/geometrie-schutzpruefung.md). |
| 22. September 2026 | 1.29 → 1.30 | Analyse 0.5.2 ergänzt Konturbelege vor einer Perspektivsperre; vier falsche Ablehnungen bei unterschiedlichen rechteckigen Feldbreiten behoben. Grenzen gleichmäßiger Verkürzung explizit dokumentiert. | Neue Gegenbeispiele verhindern eine Scheinsicherheit durch bloße Feldbreiten; keine unbekannte reale Feldform voraussetzen. | Nutzerauftrag zur strikten Planfortsetzung; [vorab formulierte Gegenproben und kombinierte Störungen](docs/geometrie-schutzpruefung.md). |
| 23. September 2026 | 1.30 (ergänzende Prüfdokumentation, fachlich unverändert) | [Markdown-Testplan](iro-gen/testplans/perspektivkorrektur-konturpruefung.md) für die Konturkorrektur erstellt und im Arbeitsplan verlinkt. | Reproduzierbare Durchführung der 22 bestehenden Gegenproben und verständliche getrennte Abnahme ermöglichen. Keine neue Produktfestlegung. | Ausdrücklicher Nutzerauftrag für einen neuen Markdown-Testplan; Abgleich mit GeometrySafetyTests und Geometrienachweis. Kein neuer Testlauf in dieser Dokumentationsaufgabe. |
| 23. September 2026 | 1.30 → 1.31 | Ladbaren IroGen-Testplan mit zehn Bildern und optionalen Feldbreitenfaktoren ergänzt; neun erfüllte Erwartungen und eine offene Fehlfreigabe im PNG-Probelauf dokumentiert. | Nutzer wollte Generatorbilder und exportierten Ergebnisbericht statt ausschließlich Kerntests; echte Generatorstörungen erweitern den Nachweis und zeigen eine verbleibende Lücke. | Klarstellung des Nutzerauftrags; [Probelauf](tests/adjustments/konturplan-20260923/bericht.md) und [Soll-Ist-Befunde](tests/adjustments/konturplan-20260923/soll-ist.md). |
| 23. September 2026 | 1.31 (ergänzender Prüfnachweis, Anforderungen unverändert) | Nutzerbericht mit zehn Bildern gegen Sollverhalten ausgewertet: neun Erwartungen erfüllt, bekannte kombinierte Fehlfreigabe reproduziert. | Extern ausgeführten Generatorlauf nachvollziehbar belegen; keine vollständige Abnahme behaupten. | [Vom Nutzer übermittelter Bericht](iro-gen/testplans/iro-testbericht-20260923-114032.md), Generator 1.4.0 / Analyse 0.5.2. |
| 23. September 2026 | 1.31 → 1.32 | Analyse 0.5.3 prüft Verjüngung auch bei zwei erkannten Feldern mit zwingenden Belegen beider Konturen. Ausnahme für kombinierte Störungen aus dem Ende-zu-Ende-Test entfernt; unveränderter Plan erfüllt 10/10 Erwartungen. | Gezielte Ursache beheben ohne Rechteckkontrollen zu sperren oder andere Schwellen zu lockern; vollständige Tests erst nach Korrektur. | Ausdrücklicher Nutzerauftrag; reproduzierte Zwei-Feld-Diagnose und [Vorher-/Nachher-Nachweise](docs/geometrie-schutzpruefung.md). |
