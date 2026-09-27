# Gemeinsame Originalpixel: Vertrag und Absicherung

Stand: 25. September 2026. Analyse 0.5.11; verbindliche Planfassung 1.46.

## Unverhandelbare Regel

Jede photometrische Bildbearbeitung muss einheitlich das gesamte Bild betreffen. Niemals Wand, Farbfelder oder beliebige andere Teilflächen unabhängig bearbeiten, auch nicht in einer Erkennungskopie. Räumlich angepasste Aufhellung, Gamma-, Weißabgleich-, Kontrast- oder Filterregeln sind ausgeschlossen. Geometrische Auswahl und robuste statistische Auswertung unveränderter Pixel sind keine Bildkorrektur.

Geraderichten darf die Messfarben ebenfalls nicht verändern. Eine tatsächliche Rasterrotation könnte durch Interpolation Farben mischen. Iro benutzt stattdessen eine virtuelle geometrische Ansicht; gemessen werden rückzugeordnete Originalpixel, nicht interpolierte Ansichtspixel.

## Geprüfter aktueller Datenfluss

Der Windows-PNG-Adapter und Android-Testbildimport erzeugen jeweils einen RGB24-Puffer für das ganze Bild. Der Analyzer hält diese Aufnahme als gemeinsame Quelle. Drehung und Detektion liefern Geometrie. RegionSampler liest sowohl Feld- als auch Wandpixel über dieselbe Bildinstanz beziehungsweise deren direkte Originalquelle. Die verkleinerte Detektionsdarstellung und ihre Klassifikationswerte werden nicht als Messfarben zurückgegeben. Keine getrennte photometrische Korrektur im geprüften Produktpfad gefunden. Der Android-Import ist weiterhin ein Testbildimport, kein geprüfter realer Kamerapfad.

## Gefundene und behobene Vertragslücke

RgbFrame übernahm bisher ReadOnlyMemory des Aufrufers ohne eigene Kopie. Der Aufrufer konnte seinen ursprünglichen Array später verändern. Auch der ausgegebene ReadOnlyMemory gab über MemoryMarshal.TryGetArray Zugriff auf das interne Array. ReadOnlyMemory allein war deshalb keine Unveränderlichkeitsgarantie. Zwei gezielte Mutationsgegenproben scheiterten vor Änderung; dies belegt eine Vertragslücke, keine bereits beobachtete getrennte Bildbearbeitung im regulären App-Ablauf.

Seit Analyse 0.5.11 übernimmt RgbFrame einen eigenen Snapshot des relevanten Puffers einschließlich Stride. Pixels liefert eine abgetrennte Kopie. GetPixel liest intern direkt aus dem privaten Speicher, ohne pro Pixel zu kopieren. Eine gedrehte Ansicht hat weiterhin keinen exportierbaren Farbpuffer. Ihr Konstruktor verlangt eine direkte Originalquelle und übereinstimmende Quellabmessungen; falsche Größe und verschachtelte Ansichten werden abgewiesen. Die neue Prüfung ungültiger Zuordnung scheiterte ebenfalls vor der Änderung.

Das erzeugt eine zusätzliche vollständige RGB-Pufferkopie beim Import und bei explizitem Export. Bei zwölf Millionen dicht gepackten Pixeln entspricht eine solche Kopie 36 MB; Speicherbedarf und Laufzeit am Gerät sind noch zu prüfen. Eingabepuffer dürfen während des Snapshot-Kopiervorgangs nicht gleichzeitig verändert werden. Keine Garantie gegen absichtliche Speicherverletzungen, Reflection oder beliebige zukünftige Codeänderungen behaupten.

## Gezielte Nachweise

Zehn neue Originalpixel-Vertragstests:

- Zwei Mutationsgegenproben: Änderung am Eingabepuffer oder an einem exportierten Array verändert weder Originalpixel noch Messresultat; gepolsterter Stride berücksichtigt.
- Fünf Drehwinkel von −45 bis +45 Grad: unabhängiger Scan aller Originalpixel kontrolliert die Auswahl innerhalb der gedrehten Maske; exakte Pixelreihenfolge, keine mehrfachen Samples und Lab-Wert aus ursprünglichen RGB-Bytes. Kein interpolierter Puffer verfügbar.
- Falsche Quellgröße und verschachtelte Drehung zurückgewiesen.
- Leere Drehungsränder liefern keine schwarze Messfarbe, sondern eine ungeeignete Messfläche.
- Absichtlich ganzbildweit invertierte Arbeitskopie: echte Erkennung findet dieselben Feldgeometrien, aber andere Farben; die vollständige Analyse der Originalaufnahme bleibt unverändert. Die Invertierung ist ausschließlich ein Test, keine neue Produktkorrektur.

132 gezielte Kernprüfungen bestanden; danach der um die Arbeitskopie erweiterte Zehn-Test-Vertragssatz ebenfalls vollständig bestanden (133 unterschiedliche Kernprüfungen insgesamt). 27 Integrationsprüfungen für PNG-Analyse, Geraderichten, Reflexe, Anzeige und Bericht bestanden. Anschließend obligatorischer Zwei-Bilder-Gegencheck bestanden. Keine übersprungenen Tests.

## Geltungsbereich und offene Arbeit

Der vorhandene Originalpixelpfad ist gegen die konkret nachgewiesenen Zugriffs- und Zuordnungslücken abgesichert. Es gibt aktuell keine produktive photometrisch bearbeitete Erkennungskopie mit eigenem Übergabevertrag. Der Kopietest ist keine Implementierung eines solchen Pfads. Wird er künftig eingeführt, müssen Geometrieübergabe, eindeutiger Aufnahmebezug und Ausschluss bearbeiteter Messfarben erneut gezielt abgesichert werden. Schritt 4 bleibt deshalb insgesamt offen; keine zukünftige Korrekturfunktion oder gesamte Regelabdeckung automatisch abgenommen.

Reale Kamera-/Decoder-Farbtreue, Geräte-Speicherverhalten, abschließender Sammel-Testlauf und ausdrückliche Nutzerabnahme bleiben offen. Keine bildübergreifende Zuordnung oder Historie eingeführt.

## Reproduktion

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~OriginalPixelContractTests
```

Android-Release-Build zur Originalpixel-Absicherung am 25. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest und kein Schluss-Sammellauf.
