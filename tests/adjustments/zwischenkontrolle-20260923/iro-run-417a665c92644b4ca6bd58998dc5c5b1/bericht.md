# Iro – kompakter Testbericht

Export UTC: 2026-09-24T12:26:11.5489207Z
Diagnosegrenze: 1 ΔE00. Bilder insgesamt: 2.
Dialogfilter: alle. Der Export berücksichtigt immer ALLE eingelesenen Bilder.
Entwicklungsdiagnose, keine Abnahme. Nominale Materialabstände vor Störungen sind keine geprüften Bild-Sollwerte. Abweisung kann richtig sein. Befunde unten sind Beispiele, keine vollständige Einzelfallliste.

Verhaltenserwartungen: 2 erfüllt; 0 nicht erfüllt; 0 nicht bewertet; 0 Prüffehler.
Diese Prüfung bewertet Freigabe, freigegebene Feldanzahl und fachlichen Hinweis. Sie ist keine Farbgenauigkeits- oder Nutzerabnahme.

| Testfall / Aufnahme | Sollverhalten | Soll-Ist-Ergebnis | Befund | Grundlage |
|---|---|---|---|---|
| Sauberes Kontrollbild - drei korrekte Felder / irogen-20260924-122610-cd6506358adf465aacb9c2e81421a344 | Messfreigabe; 3 freigegebene Felder; Hinweis erforderlich: kein Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 3 Felder freigegeben. Keine Aussage zur Farbgenauigkeit. | Saubere Geometriekontrollen und Pixelrauschen bis ±6 in der Messstabilitätsuntersuchung bestanden. Drei homogene Felder, feste Originalpalette. Ergänzende numerische Gegenprüfung im Zwei-Bilder-Regressionstest. |
| Leichtes Pixelrauschen - drei korrekte Felder / irogen-20260924-122611-b6f78307d4f64814b5aa53f409bb2eb0 | Messfreigabe; 3 freigegebene Felder; Hinweis erforderlich: kein Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 3 Felder freigegeben. Keine Aussage zur Farbgenauigkeit. | Saubere Geometriekontrollen und Pixelrauschen bis ±6 in der Messstabilitätsuntersuchung bestanden. Drei homogene Felder, feste Originalpalette. Ergänzende numerische Gegenprüfung im Zwei-Bilder-Regressionstest. |

- Vollständig gemessen; nominal unauffällig: 2
- Nominale Abweichung; fachlich zu prüfen: 0
- Teilweise gemessen: 0
- Messung abgewiesen: 0
- Verarbeitungs- oder Lesefehler: 0
- Gemessen ohne nominalen Vergleich: 0

## Zusammenfassung nach Testfall, Optionen und Softwareversion

Lauf iro-run-417a665c92644b4ca6bd58998dc5c5b1: completed, verarbeitet 2/2.

Gemeinsame Generatoroptionen: {"width":1600,"height":1200,"position":"Right","orientation":"Vertical","fieldCount":3,"stripWidthPercent":28,"stripLengthPercent":70,"gapPercent":6,"marginPercent":6,"shadeStep":5,"matchingField":1,"wallDifference":"Exact","roundedTop":false,"variableFieldHeights":false,"fieldWidthFactors":null,"labels":false,"rotationDegrees":0,"distance":"Normal","perspective":"None","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","glare":"None","dirt":"None","blur":"None","motionBlur":"None","shadows":"None","texture":"None","vignette":"None","occlusion":"None","haze":"None","exposureStops":0}
Analyseprofil 1: {"straighten":true,"profileVersion":"synthetic-trial-1","referenceMode":"SharedAutomaticTrial","detectionLongestSide":720,"regionTolerance":14,"minimumFillRatio":0.77,"minimumFieldSide":40,"minimumFieldArea":2500,"minimumSamples":600,"innerMargin":0.18,"maximumOutlierFraction":0.18,"maximumChannelMad":12,"surfaceMargin":0.05,"maximumSpatialDeltaE":2,"maximumCrossEdgeDeviation":0.06,"minimumEdgeConcentration":0.18}
### 1. Sauberes Kontrollbild - drei korrekte Felder
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.5.0, Analyse 0.5.9.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"noise":"None"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-417a665c92644b4ca6bd58998dc5c5b1, Aufnahme irogen-20260924-122610-cd6506358adf465aacb9c2e81421a344, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 2. Leichtes Pixelrauschen - drei korrekte Felder
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht. Generator 1.5.0, Analyse 0.5.9.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.02 ΔE00.
Abweichende Generatoroptionen: {"noise":"Light"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-417a665c92644b4ca6bd58998dc5c5b1, Aufnahme irogen-20260924-122611-b6f78307d4f64814b5aa53f409bb2eb0, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,02 ΔE00
  Hinweis: 


Zusätzliche automatisierte Regression: drei zugeordnete Felder, keine Fremdflächen, Überdeckung mindestens 90 %, gleiche nächste Farbe; nominale Abweichung sauber höchstens 0,01 / leichtes Rauschen höchstens 0,15 ΔE00. Keine Gerätegrenze.