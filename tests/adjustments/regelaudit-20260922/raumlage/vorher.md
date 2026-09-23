# Iro – kompakter Testbericht

Export UTC: 2026-09-22T18:09:03.2998631Z
Diagnosegrenze: 1 ΔE00. Bilder insgesamt: 108.
Dialogfilter: alle. Der Export berücksichtigt immer ALLE eingelesenen Bilder.
Entwicklungsdiagnose, keine Abnahme. Nominale Materialabstände vor Störungen sind keine geprüften Bild-Sollwerte. Abweisung kann richtig sein. Befunde unten sind Beispiele, keine vollständige Einzelfallliste.

- Vollständig gemessen; nominal unauffällig: 49
- Nominale Abweichung; fachlich zu prüfen: 3
- Teilweise gemessen: 12
- Messung abgewiesen: 44
- Verarbeitungs- oder Lesefehler: 0
- Gemessen ohne nominalen Vergleich: 0

## Zusammenfassung nach Testfall, Optionen und Softwareversion

Lauf iro-run-74fd16bdd2404378ac833d23163c448d: completed, verarbeitet 108/108.

Gemeinsame Generatoroptionen: {"width":1600,"height":1200,"position":"Center","fieldCount":7,"stripWidthPercent":23,"stripLengthPercent":82,"gapPercent":3,"marginPercent":6,"shadeStep":5,"matchingField":3,"wallDifference":"Exact","roundedTop":true,"variableFieldHeights":false,"labels":true,"rotationDegrees":0,"distance":"Normal","perspective":"None","glare":"None","dirt":"None","blur":"None","shadows":"None","texture":"None","vignette":"None","occlusion":"None","haze":"None"}
Analyseprofil 1: {"straighten":false,"profileVersion":"synthetic-trial-1","referenceMode":"SharedAutomaticTrial","detectionLongestSide":720,"regionTolerance":14,"minimumFillRatio":0.77,"minimumFieldSide":40,"minimumFieldArea":2500,"minimumSamples":600,"innerMargin":0.18,"maximumOutlierFraction":0.18,"maximumChannelMad":12,"surfaceMargin":0.05,"maximumSpatialDeltaE":2,"maximumCrossEdgeDeviation":0.06,"minimumEdgeConcentration":0.18}
### 1. Kontrolle senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180833-4361ab9de5514a918a8e5e614592da44, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 2. Kontrolle waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180833-dc54e8c5d7354820ae82143d0e2557dd, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 3. Zufällige Lage senkrechter Streifen
Grundform senkrecht, Streifen zufällig positioniert und gedreht, 7 Felder, Wand gleich Bezugsfeld; Zufällige Position und Drehung: aktiv. Generator 1.3.0, Analyse 0.5.0.
Bilder: 6; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 1; abgewiesen: 5; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 5/42; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":true,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180834-bc25d08dc6164937a8852c9529493ddd, Seed -1640519182: Teilweise gemessen; 5 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 93,8°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: Feldgrenzen passen nicht zum Streifen. Mögliche Verdeckung; Muster vollständig sichtbar halten. / Einzelne Messflächen ungeeignet; gültige Felder bleiben auswertbar.
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180834-14a1c45c434d45849cbbbc8e0d6b2672, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung -122,3°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 4. Zufällige Lage waagerechter Streifen
Grundform waagerecht, Streifen zufällig positioniert und gedreht, 7 Felder, Wand gleich Bezugsfeld; Zufällige Position und Drehung: aktiv. Generator 1.3.0, Analyse 0.5.0.
Bilder: 6; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 6; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/42; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":true,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180834-495e7784655a4df5a2b5d9a2897f439f, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung -122,3°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 5. Seitenblick leicht senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von der Seite: leicht. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"Light","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180835-667b8fd256514c50abc34ae5ed937057, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 15° (links), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: 

### 6. Oben oder unten leicht senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: leicht, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"Light","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180835-04baa29e9a014a95b3a84d3da2c486f9, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 15°, Wandabstand 0 cm.
  Hinweis: 

### 7. Seitenblick stark senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von der Seite: stark. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"Strong","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180835-60f701dc6f62443f9ddb4c6b9b38d1c9, Seed -1640519182: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 45° (rechts), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: 

### 8. Oben oder unten stark senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: stark, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"Strong","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180836-12edaaddf55945c4b0e9bd1ee91c4a2a, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 45°, Wandabstand 0 cm.
  Hinweis: 

### 9. Seitenblick sehr stark senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von der Seite: sehr stark. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 3; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 9/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"VeryStrong","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180836-b3ca8178f11e4c76add8f220f484ac3b, Seed 12345: Teilweise gemessen; 3 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 70° (links), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: Farbabstand ΔE00 · kleiner = ähnlicher

### 10. Oben oder unten sehr stark senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: sehr stark, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 2; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 19/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"VeryStrong","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180836-a436fff86c8148e7a680b08c68cc8737, Seed 12345: Teilweise gemessen; 6 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 70°, Wandabstand 0 cm.
  Hinweis: Farbabstand ΔE00 · kleiner = ähnlicher
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180836-d84df97d7fc5410989ff7029f01b2581, Seed -1640519182: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von unten 70°, Wandabstand 0 cm.
  Hinweis: 

### 11. Wandabstand klein senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Abstand zur Wand: < 10 cm. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"Small","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180837-908d7e22b00744e093ce4b9f5f20a157, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 5 cm.
  Hinweis: 

### 12. Wandabstand größer senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Abstand zur Wand: 10–< 20 cm. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"Greater","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180837-ff98e4a8d6c64292a9a9e0a098e0d29e, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 15 cm.
  Hinweis: 

### 13. Wandabstand groß senkrecht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Abstand zur Wand: > 20 cm. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"Large","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180838-0f15760b46d54a21bea29a742c57d56b, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status ungeeignete Geometrie)
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 30 cm.
  Hinweis: Muster vollständig ins Bild nehmen. Angeschnittener Streifen ist nicht messfähig.

### 14. Seitenblick leicht waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von der Seite: leicht. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"Light","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180838-71ea6757ea6143709a45b5a9da886334, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 15° (links), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: 

### 15. Oben oder unten leicht waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: leicht, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"Light","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180838-c1cc06e57b384aaf8ebcfc43b1456391, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 15°, Wandabstand 0 cm.
  Hinweis: 

### 16. Seitenblick stark waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von der Seite: stark. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"Strong","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180839-3b7162946f0e4a01838dad44fed6dd39, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 45° (links), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: 

### 17. Oben oder unten stark waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: stark, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 3; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 15/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"Strong","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180839-e2017964071c4f3db59de384655178b9, Seed 12345: Teilweise gemessen; 5 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 45°, Wandabstand 0 cm.
  Hinweis: Farbabstand ΔE00 · kleiner = ähnlicher

### 18. Seitenblick sehr stark waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von der Seite: sehr stark. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"VeryStrong","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180839-2ed9a7c8570c4cc096981cabf805f262, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 70° (links), Blick frontal 0°, Wandabstand 0 cm.
  Hinweis: 

### 19. Oben oder unten sehr stark waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: sehr stark, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 3; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 9/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"VeryStrong","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180839-50018c22c9c94a62a745bc4e69de8b16, Seed 12345: Teilweise gemessen; 3 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 70°, Wandabstand 0 cm.
  Hinweis: Farbabstand ΔE00 · kleiner = ähnlicher

### 20. Wandabstand klein waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Abstand zur Wand: < 10 cm. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"Small","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180840-65b80131efec4073a224a54df562477e, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 5 cm.
  Hinweis: 

### 21. Wandabstand größer waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Abstand zur Wand: 10–< 20 cm. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"Greater","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180840-ce716698ceaa4631889ec0e2dd2840fe, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 15 cm.
  Hinweis: 

### 22. Wandabstand groß waagerecht
waagerecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Abstand zur Wand: > 20 cm. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Horizontal","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"Large","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180841-2b874288e9f44d32932b3da964a448e2, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status ungeeignete Geometrie)
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick frontal 0°, Wandabstand 30 cm.
  Hinweis: Muster vollständig ins Bild nehmen. Angeschnittener Streifen ist nicht messfähig.

### 23. Kontrolle Blick von oben
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: stark, Blickrichtung: von oben. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"Strong","verticalDirection":"FromAbove","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180841-a4bd9b02bf584d1298e400b1922dd574, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 45°, Wandabstand 0 cm.
  Hinweis: 

### 24. Kontrolle Blick von unten
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Blick von oben/unten: stark, Blickrichtung: von unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 3; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 21/21; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"Strong","verticalDirection":"FromBelow","wallGap":"None","motionBlur":"None","noise":"None","exposureStops":0}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180841-2139824522854108885bb03f7d49276e, Seed 12345: Vollständig gemessen; nominal unauffällig; 7 von 7 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von unten 45°, Wandabstand 0 cm.
  Hinweis: 

### 25. Hochhalten leicht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: leicht, Rauschen: leicht, Blick von oben/unten: leicht, Abstand zur Wand: < 10 cm, Belichtung: −1 Blende, Blickrichtung: von unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"Light","verticalDirection":"FromBelow","wallGap":"Small","motionBlur":"Light","noise":"Light","exposureStops":-1}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180841-9bc7dccfe6ce4dc2b8f969c4544ebc83, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Messflächen ungeeignet)
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von unten 15°, Wandabstand 5 cm.
  Hinweis: Bild unscharf. Kamera ruhig halten und neu fokussieren.

### 26. Hochhalten stark
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: mittel, Rauschen: mittel, Blick von der Seite: leicht, Blick von oben/unten: stark, Abstand zur Wand: 10–< 20 cm, Belichtung: -2, Blickrichtung: von unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"Light","verticalView":"Strong","verticalDirection":"FromBelow","wallGap":"Greater","motionBlur":"Medium","noise":"Medium","exposureStops":-2}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180843-86b660ff7aab4523bc32bcf80ff5000b, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung 0,0°, Seitenblick 15° (links), Blick von unten 45°, Wandabstand 15 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 27. Hochhalten sehr stark
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: stark, Rauschen: mittel, Blick von der Seite: stark, Blick von oben/unten: sehr stark, Abstand zur Wand: > 20 cm, Belichtung: -2.5, Blickrichtung: von unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"Strong","verticalView":"VeryStrong","verticalDirection":"FromBelow","wallGap":"Large","motionBlur":"Strong","noise":"Medium","exposureStops":-2.5}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180844-f00a2ec39a694312852d54d57a8e524e, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung 0,0°, Seitenblick 45° (links), Blick von unten 70°, Wandabstand 30 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 28. Bücken leicht
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: leicht, Rauschen: leicht, Blick von oben/unten: leicht, Abstand zur Wand: < 10 cm, Belichtung: −1 Blende, Blickrichtung: von oben. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"Light","verticalDirection":"FromAbove","wallGap":"Small","motionBlur":"Light","noise":"Light","exposureStops":-1}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180845-d38c1cd8622e4a4ab119974bb77a3da9, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Messflächen ungeeignet)
  Simulierte Lage: Drehung 0,0°, Seitenblick 0° (frontal), Blick von oben 15°, Wandabstand 5 cm.
  Hinweis: Bild unscharf. Kamera ruhig halten und neu fokussieren.

### 29. Bücken stark
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: mittel, Rauschen: mittel, Blick von der Seite: leicht, Blick von oben/unten: stark, Abstand zur Wand: 10–< 20 cm, Belichtung: -2, Blickrichtung: von oben. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"Light","verticalView":"Strong","verticalDirection":"FromAbove","wallGap":"Greater","motionBlur":"Medium","noise":"Medium","exposureStops":-2}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180846-c0456052dc5e4378b51f2fe1f4e490e1, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung 0,0°, Seitenblick 15° (links), Blick von oben 45°, Wandabstand 15 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 30. Bücken sehr stark
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: stark, Rauschen: mittel, Blick von der Seite: stark, Blick von oben/unten: sehr stark, Abstand zur Wand: > 20 cm, Belichtung: -2.5, Blickrichtung: von oben. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"Strong","verticalView":"VeryStrong","verticalDirection":"FromAbove","wallGap":"Large","motionBlur":"Strong","noise":"Medium","exposureStops":-2.5}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180847-0b54c0aebcf042f48e4b7e49dccb9622, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung 0,0°, Seitenblick 45° (links), Blick von oben 70°, Wandabstand 30 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 31. Freihand kombiniert zufällig
Grundform senkrecht, Streifen zufällig positioniert und gedreht, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: leicht, Rauschen: leicht, Zufällige Position und Drehung: aktiv, Blick von der Seite: stark, Blick von oben/unten: stark, Abstand zur Wand: 10–< 20 cm, Belichtung: −1 Blende, Blickrichtung: zufällig oben/unten. Generator 1.3.0, Analyse 0.5.0.
Bilder: 6; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 6; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/42; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":true,"sideView":"Strong","verticalView":"Strong","verticalDirection":"Random","wallGap":"Greater","motionBlur":"Light","noise":"Light","exposureStops":-1}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180848-c178324a68ef412592b0743d4da14086, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status kein Muster erkannt)
  Simulierte Lage: Drehung -122,3°, Seitenblick 45° (rechts), Blick von oben 45°, Wandabstand 15 cm.
  Hinweis: Vergleichsmuster nicht erkannt.

### 32. Vergleich wenig Licht frontal
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Rauschen: mittel, Belichtung: -2. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 3; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 20/21; zusätzliche Flächen: 0; größte nominale Abweichung: 4.74 ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"None","noise":"Medium","exposureStops":-2}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180850-f2555a3b10e2458ba1e268f998c763e8, Seed 12345: Nominale Abweichung – fachlich zu prüfen; 7 von 7 Feldern gemessen, größte Abweichung 4,74 ΔE00
  Hinweis: Freigegebene Werte weichen bis zu 4,7 ΔE00 vom nominalen Materialabstand ab (Diagnosegrenze 1,0). Das allein belegt keinen Messfehler. App: Farbabstand ΔE00 · kleiner = ähnlicher 

### 33. Vergleich wenig Licht und Zittern frontal
senkrecht, Streifen mittig, 7 Felder, Wand gleich Bezugsfeld; Bewegungsunschärfe: mittel, Rauschen: mittel, Belichtung: -2. Generator 1.3.0, Analyse 0.5.0.
Bilder: 3; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 3; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/21; zusätzliche Flächen: 1; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"orientation":"Vertical","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","motionBlur":"Medium","noise":"Medium","exposureStops":-2}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-74fd16bdd2404378ac833d23163c448d, Aufnahme irogen-20260922-180851-6d7ef6328dc349f7b45edc916fb5d3f3, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Messflächen ungeeignet)
  Hinweis: Bild unscharf. Kamera ruhig halten und neu fokussieren.

