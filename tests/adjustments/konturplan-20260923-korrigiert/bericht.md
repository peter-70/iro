# Iro – kompakter Testbericht

Export UTC: 2026-09-23T09:50:31.0256348Z
Diagnosegrenze: 1 ΔE00. Bilder insgesamt: 10.
Dialogfilter: alle. Der Export berücksichtigt immer ALLE eingelesenen Bilder.
Entwicklungsdiagnose, keine Abnahme. Nominale Materialabstände vor Störungen sind keine geprüften Bild-Sollwerte. Abweisung kann richtig sein. Befunde unten sind Beispiele, keine vollständige Einzelfallliste.

- Vollständig gemessen; nominal unauffällig: 6
- Nominale Abweichung; fachlich zu prüfen: 0
- Teilweise gemessen: 0
- Messung abgewiesen: 4
- Verarbeitungs- oder Lesefehler: 0
- Gemessen ohne nominalen Vergleich: 0

## Zusammenfassung nach Testfall, Optionen und Softwareversion

Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca: completed, verarbeitet 10/10.

Gemeinsame Generatoroptionen: {"width":1600,"height":1200,"fieldCount":3,"stripWidthPercent":28,"stripLengthPercent":70,"gapPercent":6,"marginPercent":6,"shadeStep":5,"matchingField":1,"wallDifference":"Exact","roundedTop":false,"variableFieldHeights":false,"labels":false,"rotationDegrees":0,"distance":"Normal","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","glare":"None","dirt":"None","blur":"None","motionBlur":"None","shadows":"None","texture":"None","vignette":"None","occlusion":"None","haze":"None"}
Analyseprofil 1: {"straighten":true,"profileVersion":"synthetic-trial-1","referenceMode":"SharedAutomaticTrial","detectionLongestSide":720,"regionTolerance":14,"minimumFillRatio":0.77,"minimumFieldSide":40,"minimumFieldArea":2500,"minimumSamples":600,"innerMargin":0.18,"maximumOutlierFraction":0.18,"maximumChannelMad":12,"surfaceMargin":0.05,"maximumSpatialDeltaE":2,"maximumCrossEdgeDeviation":0.06,"minimumEdgeConcentration":0.18}
### 1. Rechtecke abnehmend senkrecht - SOLL messbar ohne Perspektivhinweis
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","fieldWidthFactors":[1,0.8333333333,0.6666666667],"perspective":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-207ad31de06949279ee4559f29f07ad8, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 2. Rechtecke zunehmend senkrecht - SOLL messbar ohne Perspektivhinweis
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","fieldWidthFactors":[0.6666666667,0.8333333333,1],"perspective":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-087d552313974530b6f33e57a6641f96, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 3. Kontrolle senkrecht - SOLL alle drei Felder messbar
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","fieldWidthFactors":null,"perspective":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-007ed5cf5bef41c79448d024a1b4ff01, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 4. Verjuengung senkrecht - SOLL gesperrt mit Frontalhinweis
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; Bisherige Verjüngung: mittel. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","fieldWidthFactors":null,"perspective":"Medium","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-76d7e33003ad46adbff3166369437c5c, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status ungeeignete Geometrie)
  Iro: um 1.0° gerade gerichtet.
  Hinweis: Streifen verjüngt sich deutlich. Kamera möglichst frontal auf Muster und Wand ausrichten.

### 5. Verjuengung dunkel verrauscht senkrecht - SOLL gesperrt mit Frontalhinweis
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht, Bisherige Verjüngung: mittel, Belichtung: -0.7. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","fieldWidthFactors":null,"perspective":"Medium","noise":"Light","exposureStops":-0.7}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-bcbf909249f14c13aad2e008e2174253, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status ungeeignete Geometrie)
  Hinweis: Streifen verjüngt sich deutlich. Kamera möglichst frontal auf Muster und Wand ausrichten.

### 6. Rechtecke abnehmend waagerecht - SOLL messbar ohne Perspektivhinweis
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","fieldWidthFactors":[1,0.8333333333,0.6666666667],"perspective":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-df0d3b28854444e19ae2ea3e4032f1bc, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 7. Rechtecke zunehmend waagerecht - SOLL messbar ohne Perspektivhinweis
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","fieldWidthFactors":[0.6666666667,0.8333333333,1],"perspective":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095028-d0643df859734a9193e5225a9565c07d, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 8. Kontrolle waagerecht - SOLL alle drei Felder messbar
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","fieldWidthFactors":null,"perspective":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095029-871cfa833989467192a03894527f4a1f, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 9. Verjuengung waagerecht - SOLL gesperrt mit Frontalhinweis
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; Bisherige Verjüngung: mittel. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","fieldWidthFactors":null,"perspective":"Medium","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095029-8d32a2799f5c44ceb470451e333aa6f9, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status ungeeignete Geometrie)
  Iro: um 0.9° gerade gerichtet.
  Hinweis: Streifen verjüngt sich deutlich. Kamera möglichst frontal auf Muster und Wand ausrichten.

### 10. Verjuengung dunkel verrauscht waagerecht - SOLL gesperrt mit Frontalhinweis
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht, Bisherige Verjüngung: mittel, Belichtung: -0.7. Generator 1.4.0, Analyse 0.5.3.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","fieldWidthFactors":null,"perspective":"Medium","noise":"Light","exposureStops":-0.7}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8edc49994ff04d039b8d8870ac5926ca, Aufnahme irogen-20260923-095029-789f631432204d3da15a8a89be3a9689, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status ungeeignete Geometrie)
  Iro: um 2.9° gerade gerichtet.
  Hinweis: Streifen verjüngt sich deutlich. Kamera möglichst frontal auf Muster und Wand ausrichten.

