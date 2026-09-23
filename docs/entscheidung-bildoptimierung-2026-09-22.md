# Entscheidungsprotokoll: Bildoptimierung vor der Farbanalyse

## 1. Ziel dieses Protokolls

Dieses Protokoll legt fest, welche Bildoptimierungen Iro vor der Analyse verwenden darf und welche Verfahren ausgeschlossen werden müssen.

Die wichtigste fachliche Grundlage lautet:

> Wand und Farbstreifen bilden eine gemeinsame fotografische Messeinheit. Sie dürfen für die Farbmessung niemals voneinander abgekoppelt oder unterschiedlich bearbeitet werden.

Der Sinn des Farbstreifens besteht gerade darin, die Wandfarbe und die Referenzfarben unter denselben realen Aufnahmebedingungen miteinander vergleichen zu können. Dazu gehören insbesondere:

* dieselbe Beleuchtung,
* dieselbe Belichtung,
* derselbe Weißabgleich,
* dieselbe Kamera,
* dieselbe kamerainterne Verarbeitung,
* dieselben Schatten- und Umgebungsbedingungen.

Werden Wand und Farbstreifen unterschiedlich bearbeitet, geht diese gemeinsame Bezugsbasis verloren. Das wäre kein kleiner Qualitätsmangel, sondern würde das Messprinzip der App beschädigen.

---

## 2. Verbindliche Grundentscheidung

Iro darf zwei technische Bildpfade verwenden:

### Pfad A: Erkennungsbild

Eine temporäre Arbeitskopie darf optimiert werden, um:

* den Farbstreifen zu finden,
* einzelne Farbfelder zu erkennen,
* Kanten und Konturen zu bestimmen,
* Drehung und Perspektive zu erkennen,
* geeignete Messbereiche festzulegen,
* die Bildqualität zu beurteilen.

Diese Arbeitskopie darf jedoch keine Farbwerte für das endgültige Messergebnis liefern.

### Pfad B: gemeinsames Messbild

Die Farbwerte von Wand und Farbstreifen müssen aus derselben gemeinsamen Bildgrundlage stammen.

Falls eine farbliche Korrektur erforderlich ist, muss sie:

* auf demselben mathematischen Modell beruhen,
* für das gesamte Bild gelten,
* Wand und Farbstreifen identisch behandeln,
* reproduzierbar und nachvollziehbar sein,
* auf beiden Bereichen dieselbe Transformation anwenden.

Unzulässig ist jede Optimierung, die Wand und Farbstreifen unabhängig voneinander verändert.

---

## 3. Wichtige begriffliche Unterscheidung

Eine getrennte Verarbeitungskopie zur **Erkennung** ist nicht dasselbe wie eine getrennte Bearbeitung zur **Farbmessung**.

### Zulässige Trennung

```text
Originalaufnahme
├── optimierte Arbeitskopie zur Positions- und Formerkennung
└── gemeinsames Messbild für Wand und Farbstreifen
```

Die Arbeitskopie bestimmt beispielsweise nur:

* Wo liegt der Farbstreifen?
* Wo befinden sich seine Felder?
* Welche Wandfläche soll gemessen werden?
* Ist das Bild scharf genug?
* Muss das Bild geometrisch gerichtet werden?

Anschließend werden die ermittelten Koordinaten auf die gemeinsame Messgrundlage übertragen.

### Unzulässige Trennung

```text
Wandbereich       → eigene Helligkeits- oder Farbkorrektur
Farbstreifen      → andere Helligkeits- oder Farbkorrektur
                   ↓
             anschließender Vergleich
```

Dieses Verfahren ist fachlich unzulässig. Nach der getrennten Bearbeitung ist nicht mehr feststellbar, ob ein gemessener Unterschied aus der realen Aufnahme oder aus den unterschiedlichen Bearbeitungen stammt.

---

# 4. Protokoll der einzelnen Optimierungen

## 4.1 Geraderichten

### Pro

Das Geraderichten ist sinnvoll, weil es die geometrische Lage korrigiert, ohne Wand und Farbstreifen photometrisch voneinander zu trennen.

Vorteile:

* stabilere Erkennung der Farbfelder,
* zuverlässigere Zuordnung der Felder,
* einfachere Bestimmung der Messflächen,
* weniger Fehler durch zufällige Kameradrehung.

### Kontra und Risiken

Jede Neuberechnung von Pixeln kann durch Interpolation geringfügige Farbänderungen erzeugen. Besonders an Kanten können Mischpixel entstehen.

### Entscheidung

Geraderichten ist zulässig und sinnvoll.

Für die Farbmessung sollen keine Randpixel der Farbfelder verwendet werden. Gemessen wird in ausreichend weit innen liegenden Bereichen, damit durch die geometrische Transformation entstandene Mischpixel das Ergebnis nicht beeinflussen.

---

## 4.2 Perspektivkorrektur

### Pro

Eine moderate Perspektivkorrektur kann den Farbstreifen in eine auswertbare Form bringen und die Lage seiner Felder stabilisieren.

### Kontra und Risiken

Bei einer starken Schrägaufnahme entstehen:

* erhebliche Interpolationen,
* unterschiedlich große Messflächen,
* mögliche Reflexionen,
* unterschiedliche Beleuchtungswinkel,
* Vermischung benachbarter Bereiche.

Eine geometrische Entzerrung kann diese verlorenen oder verfälschten Informationen nicht zurückholen.

### Entscheidung

Moderate Perspektivkorrektur ist zulässig. Stark perspektivische Aufnahmen sollen nicht künstlich „gerettet“, sondern abgelehnt werden.

---

## 4.3 Globale Helligkeitskorrektur

### Pro

Eine globale Aufhellung kann dunkle Bereiche für die Erkennung sichtbar machen. Wenn dieselbe Transformation auf das gesamte Bild angewendet wird, bleiben Wand und Farbstreifen zumindest innerhalb derselben Verarbeitung.

### Kontra und Risiken

Die Annahme, dass sich eine globale Aufhellung im anschließenden Vergleich vollständig aufhebt, ist nicht zuverlässig.

Gründe:

* RGB- und Lab-Farbräume sind nicht linear zueinander.
* ΔE00 reagiert nicht linear auf Helligkeitsveränderungen.
* Dunkle Farbkanäle enthalten verhältnismäßig mehr Rauschen.
* Kameras bearbeiten dunkle und helle Bereiche unterschiedlich.
* Beim Aufhellen können Rundungs- und Quantisierungsfehler verstärkt werden.
* Bereits verlorene Farbinformation kann nicht rekonstruiert werden.

### Entscheidung

Eine globale Aufhellung ist für die Erkennungskopie zulässig.

Für die Farbmessung ist sie nur dann zulässig, wenn:

* sie auf das gesamte Bild angewendet wird,
* Wand und Farbstreifen identisch behandelt werden,
* kein Kanal abgeschnitten ist,
* die Korrektur mathematisch festgelegt und getestet wurde,
* nachgewiesen ist, dass sie die Messergebnisse verbessert.

Eine automatische getrennte Aufhellung einzelner Bereiche ist verboten.

---

## 4.4 Weißabgleich und Farbkalibrierung

### Pro

Ein falscher Weißabgleich kann sämtliche Farben des Fotos verschieben. Wenn der Farbstreifen bekannte Referenzfarben besitzt, kann er möglicherweise zur Bestimmung eines gemeinsamen Korrekturmodells dienen.

Eine solche Kalibrierung kann sinnvoll sein, weil sie nicht willkürlich „verschönert“, sondern eine bekannte Abweichung der gesamten Aufnahme korrigiert.

### Kontra und Risiken

Eine lokale oder unabhängige Weißabgleichkorrektur würde Wand und Farbstreifen auf unterschiedliche Farbbasen stellen.

Auch eine gemeinsame Kalibrierung kann fehlerhaft sein, wenn:

* einzelne Referenzfelder spiegeln,
* Felder im Schatten liegen,
* das Licht räumlich stark ungleichmäßig ist,
* Farbkanäle bereits abgeschnitten sind,
* die Referenzfarben nicht zuverlässig bekannt sind.

### Entscheidung

Ein Weißabgleich beziehungsweise eine Farbkalibrierung ist nur als gemeinsames Modell zulässig.

Die aus dem Farbstreifen bestimmte Transformation muss auf Wand und Farbstreifen gleichermaßen angewendet werden. Eine getrennte Neutralisierung oder automatische Bereichsanpassung ist verboten.

---

## 4.5 Kontrastverstärkung

### Pro

Eine Kontrastverstärkung kann:

* Feldgrenzen deutlicher machen,
* die Streifenerkennung verbessern,
* schwache Kanten sichtbar machen.

### Kontra und Risiken

Eine Kontrastveränderung verändert die Farbabstände. Lokale Kontrastverfahren wie CLAHE oder automatische HDR-Optimierungen können benachbarte Bildbereiche unterschiedlich behandeln. Dadurch könnten Wand und Farbstreifen selbst innerhalb desselben Gesamtbildes verschiedene Transformationen erhalten.

### Entscheidung

Kontrastverstärkung ist für die Erkennungskopie zulässig.

Die kontrastverstärkte Kopie darf nicht als Grundlage der Farbmessung verwendet werden. Lokale Kontrastverfahren sind im Messpfad ausgeschlossen.

---

## 4.6 Entrauschen

### Pro

Leichtes Bildrauschen lässt sich durch robuste Messverfahren reduzieren:

* Mittelwert oder Median aus vielen Pixeln,
* Ausschluss auffälliger Einzelpixel,
* Auswertung mehrerer geeigneter Kameraframes,
* Messung innerhalb homogener Flächen.

Diese Verfahren können die Messung stabilisieren, ohne neue Farben zu erfinden.

### Kontra und Risiken

Starke Entrauschungsfilter können:

* Farbübergänge verwischen,
* kleine Felder verändern,
* Farben benachbarter Flächen vermischen,
* Texturen und Reflexionen fälschlich glätten.

Eine getrennte Entrauschung von Wand und Farbstreifen könnte beide Bereiche unterschiedlich verändern.

### Entscheidung

Bevorzugt wird keine klassische Bildverschönerung, sondern eine robuste statistische Farbmessung aus vielen inneren Pixeln.

Falls ein Filter eingesetzt wird, muss er:

* auf das gesamte Bild identisch angewendet werden,
* sehr konservativ arbeiten,
* Kanten nicht überschreiten,
* durch Vergleichstests nachweislich bessere Ergebnisse liefern.

---

## 4.7 Nachschärfen

### Pro

Leichtes Nachschärfen kann die Erkennung von Feldgrenzen verbessern.

### Kontra und Risiken

Nachschärfen rekonstruiert keine verlorenen Details. Es verstärkt vorhandene Kontraste und kann erzeugen:

* künstliche Farbsäume,
* Über- und Unterschwinger an Kanten,
* verstärktes Rauschen,
* scheinbar scharfe, aber falsche Strukturen.

### Entscheidung

Nachschärfen ist höchstens für die Erkennungskopie zulässig.

Geschärfte Pixel dürfen nicht für die Farbmessung verwendet werden. Ein unscharfes Messbild wird durch Nachschärfen nicht wieder messfähig.

---

## 4.8 Bewegungsunschärfe und starke Defokussierung

### Pro einer Reparatur

Eine rechnerische Entschärfung könnte das Bild optisch verständlicher erscheinen lassen.

### Kontra und Risiken

Bewegungsunschärfe vermischt die Farben verschiedener Bereiche bereits bei der Aufnahme. Beispielsweise können sich folgende Farben miteinander vermischen:

* benachbarte Farbfelder,
* Farbstreifen und Wand,
* Wand und Schatten,
* Feldfarbe und weißer Zwischenraum.

Eine Software kann nicht zuverlässig wissen, welche ursprünglichen Farben vor dieser Vermischung vorhanden waren. KI-Verfahren könnten glaubwürdige, aber erfundene Farben erzeugen.

### Entscheidung

Starke Bewegungs- oder Fokusunschärfe wird nicht repariert.

Die Aufnahme ist abzulehnen beziehungsweise bei laufender Kamera durch einen besseren Frame zu ersetzen. Eine künstliche Entschärfung wäre für eine farbmetrische Anwendung fachlich nicht vertretbar.

---

## 4.9 Überbelichtung und Clipping

### Pro einer Abdunklung

Ein überbelichtetes Bild kann durch Abdunklung optisch wieder angenehmer erscheinen.

### Kontra und Risiken

Wenn ein Farbkanal den Maximalwert erreicht hat, ist die ursprüngliche Information verloren. Aus einem abgeschnittenen Wert lässt sich nicht mehr bestimmen, wie hell oder farbig die reale Fläche tatsächlich war.

Eine Abdunklung macht aus Weiß lediglich Grau; sie stellt die verlorenen Farbkanäle nicht wieder her.

### Entscheidung

Messrelevantes Clipping führt zur Ablehnung der Aufnahme.

Es darf nicht durch nachträgliches Abdunkeln kaschiert werden.

---

## 4.10 Starke Unterbelichtung

### Pro einer Aufhellung

Eine Aufhellung kann vorhandene Strukturen sichtbar machen.

### Kontra und Risiken

Bei starker Unterbelichtung dominieren:

* Sensorrauschen,
* kamerainterne Rauschunterdrückung,
* geringe Tonwertauflösung,
* instabile Farbkanäle,
* Kompressionsartefakte.

Eine Aufhellung vergrößert diese Fehler lediglich.

### Entscheidung

Leicht dunkle Aufnahmen dürfen untersucht und gegebenenfalls über ein gemeinsames, validiertes Modell korrigiert werden.

Stark unterbelichtete Aufnahmen werden abgelehnt. Die Grenze muss durch Tests bestimmt werden.

---

## 4.11 Schatten und ungleichmäßige Beleuchtung

### Pro einer Korrektur

Ein räumliches Beleuchtungsmodell könnte Helligkeitsunterschiede teilweise ausgleichen.

### Kontra und Risiken

Ein Schatten ist nicht zwangsläufig eine reine Helligkeitsänderung. Er kann auch die spektrale Zusammensetzung des Lichts verändern.

Befindet sich beispielsweise:

* der Farbstreifen im Licht und die Wand im Schatten oder
* die Wand im Licht und der Farbstreifen im Schatten,

dann stehen beide nicht mehr unter denselben Aufnahmebedingungen. Eine einfache Aufhellung kann diesen Unterschied nicht zuverlässig beseitigen.

### Entscheidung

Iro soll prüfen, ob Wandmessfläche und Farbstreifen vergleichbar beleuchtet sind.

Deutliche Schattenunterschiede führen zu einer Warnung oder Ablehnung. Eine lokale Aufhellung nur des Schattenbereichs ist verboten.

---

## 4.12 Glanz und Reflexionen

### Pro einer Korrektur

Einzelne reflektierende Pixel könnten aus der Messfläche ausgeschlossen werden.

### Kontra und Risiken

Große Reflexionen überdecken die tatsächliche Oberflächenfarbe. Diese Farbe lässt sich aus den betroffenen Pixeln nicht wiederherstellen.

### Entscheidung

Kleine Glanzstellen dürfen durch robuste Pixelauswahl ausgeschlossen werden.

Sind wesentliche Teile eines Farbfeldes oder der Wandmessfläche betroffen, muss die Aufnahme abgelehnt oder eine andere Messfläche verwendet werden.

---

# 5. Als vernichtend einzustufende Verfahren

Folgende Verfahren widersprechen dem Grundprinzip der App und dürfen nicht implementiert werden:

1. Die Wand wird automatisch aufgehellt, der Farbstreifen jedoch nicht.
2. Der Farbstreifen erhält einen eigenen Weißabgleich.
3. Wand und Farbstreifen werden unabhängig farblich normalisiert.
4. Jeder Bereich erhält eine eigene Histogramm- oder Kontrastanpassung.
5. Die Farben des Farbstreifens werden auf bekannte Sollwerte gesetzt, ohne dieselbe daraus abgeleitete Transformation auf die Wand anzuwenden.
6. Wand und Farbstreifen werden mit unterschiedlichen Entrauschungs- oder Schärfungsverfahren bearbeitet.
7. Eine KI rekonstruiert unscharfe oder überbelichtete Farben und diese rekonstruierten Farben fließen in die Messung ein.
8. Eine optisch überzeugende Bildverbesserung wird ohne farbmetrischen Nachweis als Messgrundlage verwendet.

Diese Verfahren sind deshalb vernichtend, weil die App anschließend keine reale Beziehung mehr zwischen Wand und Farbstreifen misst. Sie vergleicht dann zwei unterschiedlich erzeugte Bildprodukte. Das Ergebnis kann plausibel aussehen, wäre aber fachlich nicht mehr abgesichert.

---

# 6. Bevorzugte Strategie

Die App soll schlechte Aufnahmen möglichst nicht reparieren, sondern frühzeitig erkennen und durch bessere Aufnahmen ersetzen.

Bei einer laufenden Kamera kann Iro mehrere Frames prüfen:

1. Ist der Farbstreifen vollständig sichtbar?
2. Ist die Perspektive zulässig?
3. Ist das Bild ausreichend scharf?
4. Sind die RGB-Kanäle ohne relevantes Clipping?
5. Ist genügend Licht vorhanden?
6. Sind Wand und Farbstreifen vergleichbar beleuchtet?
7. Gibt es starke Schatten oder Reflexionen?
8. Ist die Kamera kurzzeitig ruhig?
9. Sind geeignete Messflächen vorhanden?

Erst wenn diese Bedingungen erfüllt sind, wird ein Frame zur Messung verwendet.

Mehrere geeignete Frames können außerdem gemeinsam ausgewertet werden. Ein robuster Mittelwert über mehrere Aufnahmen kann Rauschen reduzieren, ohne eine künstliche Farbe zu erzeugen.

---

# 7. Technische Leitplanken für die Implementierung

Der lokale Agent soll folgende Regeln als verbindlich behandeln:

## Regel 1: Gemeinsame Photometrie

Wand und Farbstreifen müssen immer aus derselben photometrischen Bildgrundlage ausgewertet werden.

## Regel 2: Keine unabhängigen Optimierungen

Es darf keine separate Helligkeits-, Kontrast-, Gamma-, Weißabgleich- oder Farbkorrektur für Wand und Farbstreifen geben.

## Regel 3: Erkennung und Messung trennen

Optimierungen zur Erkennung sind erlaubt, sofern ihre Pixelwerte nicht in die Farbmessung eingehen.

## Regel 4: Gemeinsame Korrektur nur mit Nachweis

Eine gemeinsame Farbkorrektur ist nicht automatisch sicher. Sie muss durch reproduzierbare Tests nachweisen, dass sie gegenüber der unkorrigierten Messung eine Verbesserung darstellt.

## Regel 5: Keine erfundenen Informationen

Clipping, starke Unschärfe und starke Unterbelichtung dürfen nicht durch Rekonstruktion kaschiert werden.

## Regel 6: Ablehnung ist ein gültiges Ergebnis

Wenn die Aufnahmebedingungen keine belastbare Messung zulassen, ist „Aufnahme nicht geeignet“ fachlich besser als ein scheinbar präzises, aber falsches Farbergebnis.

## Regel 7: Innere Messflächen verwenden

Messpixel müssen mit ausreichendem Abstand zu:

* Kanten,
* Feldgrenzen,
* Schattenübergängen,
* Reflexionen,
* geometrisch interpolierten Randbereichen

ausgewählt werden.

---

# 8. Pro und Kontra der empfohlenen Gesamtarchitektur

## Pro

* Das Messprinzip der App bleibt erhalten.
* Wand und Farbstreifen behalten ihre gemeinsame Bezugsbasis.
* Die Erkennung darf trotzdem technisch verbessert werden.
* Schlechte Aufnahmen können frühzeitig erkannt werden.
* Geometrische und farbmetrische Verarbeitung bleiben nachvollziehbar getrennt.
* Korrekturen können einzeln getestet und validiert werden.
* Die App vermeidet glaubwürdige, aber künstlich erzeugte Messergebnisse.

## Kontra

* Die Architektur benötigt zwei klar getrennte Verarbeitungspfade.
* Koordinaten aus der Erkennungskopie müssen zuverlässig auf die Messgrundlage übertragen werden.
* Einige Bilder müssen abgelehnt werden, obwohl sie optisch noch brauchbar erscheinen.
* Gemeinsame Farbkorrekturen benötigen umfangreiche Vergleichstests.
* Die App kann nicht jedes schlechte Foto automatisch retten.

Diese Nachteile sind akzeptabel. Sie betreffen Implementierungsaufwand und Bedienkomfort. Die Alternative – getrennte Bearbeitung von Wand und Farbstreifen – betrifft dagegen die fachliche Gültigkeit des gesamten Ergebnisses.

---

# 9. Abschließende Entscheidung

Die App betrachtet die Aufnahme als ein zusammengehörendes Ganzes.

Wand und Farbstreifen dürfen für Erkennungszwecke als unterschiedliche Regionen lokalisiert werden. Für die farbliche Auswertung dürfen sie jedoch nicht unabhängig optimiert oder auf unterschiedliche Bildgrundlagen gestellt werden.

Die zentrale Festlegung lautet deshalb:

> Jede farbwirksame Verarbeitung muss die gemeinsame Beziehung zwischen Wand und Farbstreifen erhalten. Eine voneinander unabhängige Bildoptimierung beider Bereiche ist ausgeschlossen, weil sie das grundlegende Messprinzip von Iro zerstören würde.

Optimierungen sollen vorrangig dazu dienen, den Farbstreifen und geeignete Messflächen zu erkennen sowie ungeeignete Aufnahmen auszusortieren. Sie dürfen nicht dazu verwendet werden, fehlende oder verfälschte Farbinformation künstlich zu ersetzen.
