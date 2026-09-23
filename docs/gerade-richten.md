# Gerade richten vor der Farbfeldsuche

Stand: 22. September 2026. Analyse 0.4.0, Planfassung 1.26.

## Festgelegtes Verhalten

Iro schätzt zu Beginn der Einzelbildanalyse die gemeinsame Richtung deutlicher, rechtwinkliger Bildkanten. Ist die Richtung eindeutig, richtet Iro sie auf die nächstgelegene waagerechte beziehungsweise senkrechte Achse aus. Die maximale Korrektur beträgt 45°. Dadurch sind alle Drehungen des Streifens in der Bildebene abgedeckt: Eine Drehung um beispielsweise 80° benötigt nur eine Korrektur um 10° zur anderen Achse.

Das Geraderichten erfolgt vor der Suche nach zusammenhängenden Farbregionen und vor der Gruppierung zu einem Streifen. Bei uneindeutiger Richtung bleibt das Bild unverändert. Es wird nicht anhand von Generatoroptionen oder Sollfarben entschieden.

## Farbmessung und Koordinaten

Die geradgerichtete Ansicht dient der Geometriesuche. Farbmittelwerte, Gleichmäßigkeitsprüfung und Qualitätsmerkmale werden aus den unveränderten Pixeln des Originalbilds entnommen. Dadurch entstehen durch die Drehung keine interpolierten Farben.

Erkannte Feld-, Innen- und Referenzflächen werden als Polygone in das ursprüngliche Bildkoordinatensystem zurückgerechnet. Umhüllungsrechtecke bleiben für ältere Verbraucher erhalten. Die nachgelagerte Testauswertung ordnet Soll- und Istfelder anhand der Polygonüberlappung zu, wenn beide Polygone vorhanden sind. Gedrehte leere Bildränder gelten weder als Farbfeld noch als Wandreferenz.

Der Ergebnisdatensatz enthält den angewandten Korrekturwinkel. Der kompakte Bericht nennt ihn bei den jeweiligen Beispielen. Positive Werte bedeuten die auf das Originalbild angewandte Drehung.

## Verfahren und Grenzen

Die Winkelschätzung arbeitet auf einer auf höchstens 480 Pixel Kantenlänge verkleinerten Suchansicht. Sie bildet farbkanalweise Kanten, stimmt über Winkel von −45° bis unter +45° ab und verlangt zwei rechtwinklige Kantenfamilien. Eine konkurrierende Richtung muss ausreichend schwächer sein. Diese Bedingungen sollen einzelne Schattenkanten und unstrukturierte Flächen nicht als Streifenrichtung ausgeben.

Geraderichten korrigiert ausschließlich Drehung in der Bildebene. Starke Perspektive, Verdeckung, Unschärfe, zu kleine Flächen und ungeeignete Beleuchtung bleiben eigene Qualitätsprobleme. Perspektivische Kanten können eine kleine zusätzliche Drehung nahelegen; das ist keine vollständige perspektivische Entzerrung.

## Reproduzierbare Prüfung

- 22 klare Generatorbilder: vertikale und horizontale Grundform bei −80°, −60°, −45°, −30°, −10°, 0°, 10°, 30°, 45°, 60° und 80°. Alle sieben Felder jedes Bildes wurden erkannt und gemessen; die Winkelabweichung lag höchstens 0,6° und die nominale ΔE00-Abweichung höchstens 0,15.
- Im 108-Bilder-Plan wurden die zwölf ausschließlich zufällig positionierten und gedrehten Bilder vor der Korrektur einmal teilweise und elfmal gar nicht gemessen. Mit Korrektur wurden neun vollständig und drei teilweise gemessen. Alle zugeordneten Werte blieben höchstens 0,15 ΔE00 vom nominalen Wert entfernt.
- Alle 49 zuvor vollständig und innerhalb der Diagnosegrenze gemessenen Bilder blieben in diesem Zustand.
- Die strengere Prüfung einer einzelnen klar verschmierten Feldkante reduzierte die ohne Geraderichten freigegebenen nominal auffälligen Bilder im selben Prüfstand von fünf auf drei. Im Gesamtlauf mit Geraderichten wurden 58 Bilder vollständig/genau, 22 teilweise und 20 gar nicht gemessen; acht Bilder hatten nominale Abweichungen über 1 ΔE00. Fünf zusätzliche Auffälligkeiten stammen aus sehr dunklen, bewegungsunscharfen Perspektivkombinationen, die durch die verbesserte Geometriesuche überhaupt erst erkannt wurden. Da die Nominalwerte Materialfarben vor Störungen beschreiben, ist dies eine Entwicklungsdiagnose und kein alleiniger Beleg eines falschen App-Messwerts.
- Starke zufällige Freihandkombinationen mit Perspektive, Wandabstand, wenig Licht und Unschärfe blieben vollständig abgewiesen.

Reale Kameraaufnahmen, Gerätegeschwindigkeit und Overlaydarstellung sind damit noch nicht abgenommen. Die künstlichen Bilder belegen die Geometriefunktion und ihre Rückabbildung.