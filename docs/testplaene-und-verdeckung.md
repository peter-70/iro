# Testpläne, Ergebnisexport und Verdeckung

Stand: 22. September 2026. Entwicklungsnachweis zu den Planfassungen 1.22 und 1.23.

## Bedienung

1. Die neu gebaute Release-Fassung starten: iro-gen/bin/Release/net10.0-windows/IroGen.exe.
2. **Testplan laden** wählen und iro-gen/testplans/bildqualitaet-und-abstand.json öffnen.
3. IroGen erzeugt 113 Bilder in 35 benannten Fällen. Danach **Testergebnisse auswerten**: falls nötig läuft zunächst der vollständige Satz durch die lokale Iro-Analyse. **An Iro-Tests senden** bleibt als separater Analysestart verfügbar.
4. Im automatisch geöffneten Fenster mit **Aktuelle Serie · 113 Bilder ausgewertet** den Button **Kompakten Bericht exportieren** wählen. Die erzeugte Markdown-Datei kann im Chat bereitgestellt werden.

Der Export umfasst alle im Dialog eingelesenen Bilder des aktuellen Laufs, unabhängig vom Problemfilter. Er gruppiert nach Fall, Generatoroptionen und Software-/Analyseprofil; nennt Mengen, größte nominale Abweichung und bis zu drei nachvollziehbare Beispiele je Gruppe. Er ist bewusst keine vollständige Liste sämtlicher Einzelwerte. Lauf- und Aufnahme-ID sowie Seed ermöglichen gezielte Rückfragen. Alte Läufe bleiben auf der Festplatte erhalten, werden beim Auswerten der aktuellen Serie aber nicht beigemischt. Originale Ergebnisdateien werden nicht verändert.

## Testplanformat v1

JSON-Objekt mit formatVersion=1, kind="irogen-test-plan", name, defaults und cases.
Jeder Fall hat name, count, coverRange und options. Options enthält dieselben camelCase-Eigenschaften wie GeneratorOptions; nicht genannte Werte werden aus defaults übernommen. Enum-Werte entsprechen den gespeicherten englischen Namen. Unbekannte Eigenschaften, ungültige Werte, doppelte Fallnamen und mehr als insgesamt 10000 Bilder werden vor der Generierung zurückgewiesen.

Beispiel:

    {
      "formatVersion": 1,
      "kind": "irogen-test-plan",
      "name": "Verdeckung untersuchen",
      "defaults": { "seed": 12345, "width": 1600, "height": 1200 },
      "cases": [
        { "name": "Kontrolle", "count": 3, "coverRange": false, "options": {} },
        { "name": "Verdeckung mittel", "count": 3, "coverRange": false,
          "options": { "occlusion": "Medium" } }
      ]
    }

Gleicher Ausgangsseed und gleiche Farboptionen liefern pro Fall dieselbe Palettenfolge. Dadurch können ungestörte und gestörte Bilder paarweise verglichen werden. coverRange=true verteilt die nominalen Abstände über die bisherigen elf Testbereiche. Die Beispieldatei enthält ungestörte Kontrollen, alle Stufen von Verdeckung, Unschärfe, Bewegungsunschärfe, Schatten, Randabfall, Schleier und Rauschen, sechs Entfernungen, ±1 EV sowie waagerechte Verdeckungsfälle und elf Kontrollen über den Abstandsbereich. Diese Serie ist Entwicklungsdiagnose, kein zurückgehaltener Abnahmesatz.

## Entfernungen

Die Auswahl ist gegenseitig ausschließend: Normal, zu nahe (nahe / näher / ganz nahe), zu weit (weit / weiter / sehr weit).
Simulierte Streifenskalierung: 1,0; 1,35 / 2,0 / 3,4; 0,65 / 0,35 / 0,18. Nahaufnahmen sind zentriert und dürfen angeschnitten sein. Das sind Bildgeometrievarianten, keine kalibrierten Zentimeterangaben und keine zusätzliche Fokusunschärfe. Die bisherigen gespeicherten TooClose-/TooFar-Werte behalten ihre Bedeutung als stärkste Stufe. Versionen 1.0.0 und 1.1.0 bleiben ladbar; neue Exporte nennen 1.2.0.

## Nachgestellter Analysefehler und Korrektur

Mit neu erzeugten Standardbildern (Seed 12345, 1600 × 1200, sieben Felder) erkennt die bisherige Analyse 0.1.0 die braune Abdeckfläche als eigenes Feld. Leichte und mittlere Verdeckung lieferten dafür jeweils etwa 36,63 ΔE00 zur Referenz. Das ist der Messwert der Fremdfläche, nicht die Abweichung zur nominalen Erwartung im ursprünglichen Screenshot. Die alten results.json wurden für diese Untersuchung nicht gelesen.

Analyse 0.2.0 prüft bei mindestens drei gruppierten Flächen die Übereinstimmung beider Quergrenzen mit einer strikten Mehrheit der Flächen. Die technische Versuchstoleranz beträgt max(zwei Suchpixel, 6 % der Streifenbreite); sie ist als MaximumCrossEdgeDeviation in den Analyseoptionen dokumentiert. Abweichende Kandidaten bleiben als gesperrte Flächen mit Verdeckungshinweis erhalten und können nicht als Wandreferenz verwendet werden. Unterschiedliche Feldhöhen und Farben sind kein Ablehnungsgrund.

Bei leichter und mittlerer Verdeckung des beschriebenen Falls bleiben jeweils fünf Felder freigegeben; die Fremdfläche wird gesperrt. Die ungestörte Kontrolle liefert weiterhin sieben Felder. Starke Verdeckung bleibt in diesem Fall mehrdeutig. Die Prüfung erkennt nicht jede Verdeckung: Eine vollständig passende Fremdfläche oder fehlende intakte Vergleichsfelder bleiben Grenzen des Verfahrens. Daraus folgt keine allgemeine Behebung von Schatten-, Schleier- oder Beleuchtungsfehlern.

Der Dialog nennt bei nominalen Abweichungen jetzt tatsächlich gespeicherte Hinweise statt pauschal zu behaupten, dass keine Hinweise vorliegen.

## Prüfungen

- Paketwiederherstellung im Locked-Modus erfolgreich.
- Gesamte Solution in Release erfolgreich gebaut, einschließlich Android; acht XAML-Hinweise zu fehlendem x:DataType in der unveränderten Android-Oberfläche, keine Buildfehler. Die laufende Debug-Instanz von IroGen blieb unangetastet.
- WPF-Vorschau in normaler und minimaler Fenstergröße gerendert; die minimale Ansicht visuell kontrolliert, der neue Testplan-Button bleibt erreichbar.
- Kernprüfungen einschließlich unabhängig gezeichneter Verdeckung mit unterschiedlichen Feldhöhen in beiden Orientierungen: 52 bestanden.
- Generator-/Analyse-/WPF-Prüfungen: 104 bestanden; nach weiterer Verdichtung des Exports die neun betroffenen Testplan-/Exportprüfungen erneut bestanden. Der Testplan durchläuft in einem isolierten Vier-Bild-Test Generierung, Übergabe, Analyse, Einlesen und Berichtserzeugung.
- Historischer Stand der ersten Implementierung (Fassung 1.22): Die 113-Bilder-Serie war noch nicht vollständig durch die WPF-Oberfläche geprüft. Für Fassung 1.23 ist nun der komplette technische Ablauf mit 113 Bildern geprüft; dies ist weiterhin keine fachliche Abnahme aller Bildbefunde.
- Reale Kamera- und Farbgenauigkeit sind nicht Gegenstand dieses Nachweises.

## Korrektur des Auswertungsablaufs (Fassung 1.23)

Der am 22. September gemeldete Ablauf ließ nach dem Laden des Testplans alte Ergebnisse erscheinen. Belegbar ist: Der Auswertungsbutton lud ausschließlich die gespeicherten Läufe, ohne den Bezug zur sichtbaren Bildserie zu prüfen. Zudem blieb das Anzahl-Feld bei 1. Die Screenshots beweisen nicht, an welcher Stelle eine zuvor angeforderte Analyse ausgeblieben ist; diese Ursache wird nicht als sicher behauptet.

Die Korrektur verbindet Auswertung, nötigenfalls Analyse und konkreten Lauf. Nach einer neuen Generierung wird die Zuordnung des alten Laufs aufgehoben. Fehler bei der Übergabe öffnen keine historischen Ersatzresultate. Während der Arbeit ist der Button gesperrt. Ein isolierter WPF-Test erzeugt zunächst einen alten Lauf mit elf Bildern, danach die echte Testplanserie mit 113 Bildern und betätigt direkt den Auswertungsbutton. Er prüft die tatsächlichen Tabellenzeilen, Analyseversion, einzige Laufzuordnung und Exportanzahl im geöffneten Dialog.
Prüfergebnis der Ablaufkorrektur: Alle 104 Generator-/Analyse-/WPF-Tests bestanden, einschließlich des vollständigen 113-Bilder-Ablaufs. Release-Build von IroGen erfolgreich ohne Warnungen oder Fehler. Vorhandene Nutzer-Ergebnisdateien wurden für diese Prüfung nicht eingelesen; Tests liefen in einem isolierten temporären Projekt.
