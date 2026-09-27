# Iro – kompakter Testbericht

Export UTC: 2026-09-27T06:56:49.2231116Z
Diagnosegrenze: 1 ΔE00. Bilder insgesamt: 8.
Dialogfilter: alle. Der Export berücksichtigt immer ALLE eingelesenen Bilder.
Entwicklungsdiagnose, keine Abnahme. Nominale Materialabstände vor Störungen sind keine geprüften Bild-Sollwerte. Abweisung kann richtig sein. Befunde unten sind Beispiele, keine vollständige Einzelfallliste.

Verhaltenserwartungen: 8 erfüllt; 0 nicht erfüllt; 0 nicht bewertet; 0 Prüffehler.
Diese Prüfung bewertet Freigabe, freigegebene Feldanzahl und fachlichen Hinweis. Sie ist keine Farbgenauigkeits- oder Nutzerabnahme.

| Testfall / Aufnahme | Sollverhalten | Soll-Ist-Ergebnis | Befund | Grundlage |
|---|---|---|---|---|
| Vertical - Schatten None - Rauschen None / irogen-20260927-065646-c6ec664ae55b4f1bb7d8e1c65c4e85b8 | Messfreigabe; 3 freigegebene Felder; Hinweis erforderlich: kein Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 3 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. |
| Vertical - Schatten Strong - Rauschen None / irogen-20260927-065647-f7e7819641724d6b82901692c048d5c1 | Vollständige Sperre; 0 freigegebene Felder; Hinweis erforderlich: anderer Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 0 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. Präzisierung 27. September 2026: Dieser Fall scheitert an fehlender geeigneter Wandreferenz; kein belegter Code für räumliche Ungleichmäßigkeit. |
| Vertical - Schatten None - Rauschen Light / irogen-20260927-065647-8e8645d6cdc747eeba0f76c68a59fb82 | Messfreigabe; 3 freigegebene Felder; Hinweis erforderlich: kein Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 3 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. |
| Vertical - Schatten Strong - Rauschen Light / irogen-20260927-065647-b330b08786dc49759a9f979790549908 | Vollständige Sperre; 0 freigegebene Felder; Hinweis erforderlich: anderer Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 0 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. Präzisierung 27. September 2026: Dieser Fall scheitert an fehlender geeigneter Wandreferenz; kein belegter Code für räumliche Ungleichmäßigkeit. |
| Horizontal - Schatten None - Rauschen None / irogen-20260927-065647-a7256048a3bf491d8db9f6d5b932791b | Messfreigabe; 3 freigegebene Felder; Hinweis erforderlich: kein Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 3 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. |
| Horizontal - Schatten Strong - Rauschen None / irogen-20260927-065648-738eadae307b47af9b752c233082a342 | Vollständige Sperre; 0 freigegebene Felder; Hinweis erforderlich: Messfläche räumlich ungleichmäßig; Ursache nicht bestimmt | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 0 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. |
| Horizontal - Schatten None - Rauschen Light / irogen-20260927-065648-71b0adf9f9a74d38865a0bd41ee562be | Messfreigabe; 3 freigegebene Felder; Hinweis erforderlich: kein Qualitätshinweis | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 3 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. |
| Horizontal - Schatten Strong - Rauschen Light / irogen-20260927-065648-2c35eca8d6f4410880e13e0a94dc28ce | Vollständige Sperre; 0 freigegebene Felder; Hinweis erforderlich: Messfläche räumlich ungleichmäßig; Ursache nicht bestimmt | Erwartung erfüllt | Alle hinterlegten Verhaltenserwartungen erfüllt; 0 Felder freigegeben.  Keine Aussage zur Farbgenauigkeit. | Vorab festgelegte Erwartung für diese Geometrie: homogene scharfe Kontrollen mit bekanntem leichtem Rauschen messbar; starker modellierter Schattenübergang durch Messflächen sperrt sämtliche Vergleiche. Vorwärtsmodell SceneGenerator (linearer Lichtfaktor bis 0,34). Keine Behauptung einer eindeutigen Schattenursache aus Pixeln. Unabhängige Gegenproben: IlluminationEvidenceTests; docs/beleuchtung-untersuchung.md. |

- Vollständig gemessen; nominal unauffällig: 4
- Nominale Abweichung; fachlich zu prüfen: 0
- Teilweise gemessen: 0
- Messung abgewiesen: 4
- Verarbeitungs- oder Lesefehler: 0
- Gemessen ohne nominalen Vergleich: 0

## Zusammenfassung nach Testfall, Optionen und Softwareversion

Lauf iro-run-8ef76059cfd34b5b8307f346810480bd: completed, verarbeitet 8/8.

Gemeinsame Generatoroptionen: {"width":1600,"height":1200,"fieldCount":3,"stripWidthPercent":28,"stripLengthPercent":70,"gapPercent":6,"marginPercent":6,"shadeStep":5,"matchingField":1,"wallDifference":"Exact","roundedTop":false,"variableFieldHeights":false,"fieldWidthFactors":null,"labels":false,"rotationDegrees":0,"distance":"Normal","perspective":"None","randomPlacement":false,"sideView":"None","verticalView":"None","verticalDirection":"Random","wallGap":"None","glare":"None","dirt":"None","blur":"None","motionBlur":"None","texture":"None","vignette":"None","occlusion":"None","haze":"None","exposureStops":0}
Analyseprofil 1: {"straighten":true,"profileVersion":"synthetic-trial-1","referenceMode":"SharedAutomaticTrial","detectionLongestSide":720,"regionTolerance":14,"minimumFillRatio":0.77,"minimumFieldSide":40,"minimumFieldArea":2500,"minimumSamples":600,"innerMargin":0.18,"maximumOutlierFraction":0.18,"maximumChannelMad":12,"surfaceMargin":0.05,"maximumSpatialDeltaE":2,"maximumCrossEdgeDeviation":0.06,"minimumEdgeConcentration":0.18}
### 1. Vertical - Schatten None - Rauschen None
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","shadows":"None","noise":"None"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065646-c6ec664ae55b4f1bb7d8e1c65c4e85b8, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 2. Vertical - Schatten Strong - Rauschen None
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; Schatten: stark. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","shadows":"Strong","noise":"None"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065647-f7e7819641724d6b82901692c048d5c1, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Referenzfläche ungeeignet)
  Hinweis: Zu wenig freie Wandfläche. Muster und Wand vollständig ins Bild nehmen.

### 3. Vertical - Schatten None - Rauschen Light
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.02 ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","shadows":"None","noise":"Light"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065647-8e8645d6cdc747eeba0f76c68a59fb82, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,02 ΔE00
  Hinweis: 

### 4. Vertical - Schatten Strong - Rauschen Light
senkrecht, Streifen rechts, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht, Schatten: stark. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Right","orientation":"Vertical","shadows":"Strong","noise":"Light"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065647-b330b08786dc49759a9f979790549908, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Referenzfläche ungeeignet)
  Hinweis: Zu wenig freie Wandfläche. Muster und Wand vollständig ins Bild nehmen.

### 5. Horizontal - Schatten None - Rauschen None
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; keine Störung. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.00 ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","shadows":"None","noise":"None"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065647-a7256048a3bf491d8db9f6d5b932791b, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,00 ΔE00
  Hinweis: 

### 6. Horizontal - Schatten Strong - Rauschen None
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; Schatten: stark. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","shadows":"Strong","noise":"None"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065648-738eadae307b47af9b752c233082a342, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Messflächen ungeeignet)
  Hinweis: Messflächen sind zu ungleichmäßig für einen zuverlässigen Vergleich. Bitte für gleichmäßige Beleuchtung und einheitliche Messflächen sorgen und erneut aufnehmen. / Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden.

### 7. Horizontal - Schatten None - Rauschen Light
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 1; abweichend: 0; teilweise: 0; abgewiesen: 0; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 3/3; zusätzliche Flächen: 0; größte nominale Abweichung: 0.01 ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","shadows":"None","noise":"Light"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065648-71b0adf9f9a74d38865a0bd41ee562be, Seed 12345: Vollständig gemessen; nominal unauffällig; 3 von 3 Feldern gemessen, größte Abweichung 0,01 ΔE00
  Hinweis: 

### 8. Horizontal - Schatten Strong - Rauschen Light
waagerecht, Streifen unten, 3 Felder, Wand gleich Bezugsfeld; Rauschen: leicht, Schatten: stark. Generator 1.5.0, Analyse 0.5.15.
Bilder: 1; vollständig/nominal unauffällig: 0; abweichend: 0; teilweise: 0; abgewiesen: 1; Lesefehler: 0; ohne nominalen Vergleich: 0.
Gemessene/erwartete Felder: 0/3; zusätzliche Flächen: 0; größte nominale Abweichung: – ΔE00.
Abweichende Generatoroptionen: {"position":"Bottom","orientation":"Horizontal","shadows":"Strong","noise":"Light"}; Analyseprofil 1.

Bis zu drei unterschiedliche Befunde (jeweils ungünstigstes Beispiel):
- Lauf iro-run-8ef76059cfd34b5b8307f346810480bd, Aufnahme irogen-20260927-065648-2c35eca8d6f4410880e13e0a94dc28ce, Seed 12345: Messung abgewiesen; Kein Farbfeld gemessen (Status Messflächen ungeeignet)
  Hinweis: Messflächen sind zu ungleichmäßig für einen zuverlässigen Vergleich. Bitte für gleichmäßige Beleuchtung und einheitliche Messflächen sorgen und erneut aufnehmen. / Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden.

