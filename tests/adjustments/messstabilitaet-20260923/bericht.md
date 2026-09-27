# Messstabilität: kontrollierte Bildfolgen

Analyse 0.5.13; 24 Folgen × 12 Frames = 288 Einzelbildanalysen. Keine Kamera-, Genauigkeits- oder Abnahmeprüfung.
Pro Frame werden Wand und Feld gemeinsam vom produktiven Analyzer gemessen. Feldzuordnung nur für diese feste Geometrie nach Lage; kein allgemeines Tracking.
Median, unskalierte MAD und Spannweite beziehen sich auf ΔE00 je Feld über zwölf Frames. Basisdifferenz = Betrag der Differenz zum Median derselben unveränderten digitalen Szene, kein realer Farbfehler.
Alle Felder aller Frames müssen freigegeben sein, sonst schlägt dieser begrenzte Versuch fehl; fehlende Werte werden weder als Null ersetzt noch still aussortiert. Keine universellen Freigabeschwellen.

| Folge | Richtung | Wand RGB | Feld | Werte | Median ΔE00 | MAD | Spannweite | Basisdifferenz |
|---|---|---:|---:|---:|---:|---:|---:|---:|
| Unverändert | senkrecht | 10 | 1 | 12 | 25.029111 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 10 | 2 | 12 | 33.996120 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 10 | 3 | 12 | 44.533543 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | senkrecht | 10 | 1 | 12 | 25.028598 | 0.003345 | 0.020032 | 0.000513 |
| Pixelrauschen ±2 | senkrecht | 10 | 2 | 12 | 33.998264 | 0.001563 | 0.009122 | 0.002144 |
| Pixelrauschen ±2 | senkrecht | 10 | 3 | 12 | 44.533575 | 0.004445 | 0.016276 | 0.000032 |
| Pixelrauschen ±6 | senkrecht | 10 | 1 | 12 | 25.025682 | 0.010398 | 0.056627 | 0.003429 |
| Pixelrauschen ±6 | senkrecht | 10 | 2 | 12 | 34.000203 | 0.004871 | 0.020798 | 0.004083 |
| Pixelrauschen ±6 | senkrecht | 10 | 3 | 12 | 44.532197 | 0.012683 | 0.043123 | 0.001346 |
| Wechselnder Offset ±6 | senkrecht | 10 | 1 | 12 | 24.977258 | 0.750746 | 1.501492 | 0.051854 |
| Wechselnder Offset ±6 | senkrecht | 10 | 2 | 12 | 33.942868 | 1.045963 | 2.091927 | 0.053252 |
| Wechselnder Offset ±6 | senkrecht | 10 | 3 | 12 | 44.490274 | 1.360659 | 2.721319 | 0.043268 |
| Fester Offset +6 | senkrecht | 10 | 1 | 12 | 25.728004 | 0.000000 | 0.000000 | 0.698893 |
| Fester Offset +6 | senkrecht | 10 | 2 | 12 | 34.988831 | 0.000000 | 0.000000 | 0.992711 |
| Fester Offset +6 | senkrecht | 10 | 3 | 12 | 45.850934 | 0.000000 | 0.000000 | 1.317391 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 10 | 1 | 12 | 18.159549 | 3.143155 | 11.655922 | 6.869562 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 10 | 2 | 12 | 24.041017 | 4.436914 | 16.973512 | 9.955103 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 10 | 3 | 12 | 30.998480 | 5.865174 | 22.881368 | 13.535062 |
| Unverändert | senkrecht | 140 | 1 | 12 | 27.333704 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 140 | 2 | 12 | 17.970132 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 140 | 3 | 12 | 12.897875 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | senkrecht | 140 | 1 | 12 | 27.337659 | 0.004843 | 0.026002 | 0.003955 |
| Pixelrauschen ±2 | senkrecht | 140 | 2 | 12 | 17.971808 | 0.007350 | 0.049733 | 0.001676 |
| Pixelrauschen ±2 | senkrecht | 140 | 3 | 12 | 12.904328 | 0.006906 | 0.027273 | 0.006453 |
| Pixelrauschen ±6 | senkrecht | 140 | 1 | 12 | 27.338794 | 0.014432 | 0.077750 | 0.005090 |
| Pixelrauschen ±6 | senkrecht | 140 | 2 | 12 | 17.970637 | 0.017486 | 0.134335 | 0.000505 |
| Pixelrauschen ±6 | senkrecht | 140 | 3 | 12 | 12.910910 | 0.019277 | 0.078602 | 0.013035 |
| Wechselnder Offset ±6 | senkrecht | 140 | 1 | 12 | 27.293222 | 0.471646 | 0.943291 | 0.040482 |
| Wechselnder Offset ±6 | senkrecht | 140 | 2 | 12 | 17.897003 | 0.301568 | 0.603136 | 0.073129 |
| Wechselnder Offset ±6 | senkrecht | 140 | 3 | 12 | 12.899080 | 0.062630 | 0.125261 | 0.001205 |
| Fester Offset +6 | senkrecht | 140 | 1 | 12 | 27.764868 | 0.000000 | 0.000000 | 0.431164 |
| Fester Offset +6 | senkrecht | 140 | 2 | 12 | 17.595435 | 0.000000 | 0.000000 | 0.374697 |
| Fester Offset +6 | senkrecht | 140 | 3 | 12 | 12.836450 | 0.000000 | 0.000000 | 0.061425 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 140 | 1 | 12 | 19.320571 | 3.377655 | 12.727026 | 8.013132 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 140 | 2 | 12 | 13.948744 | 1.932426 | 7.285893 | 4.021388 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 140 | 3 | 12 | 11.009936 | 1.001204 | 3.750253 | 1.887939 |
| Unverändert | waagerecht | 10 | 1 | 12 | 25.029111 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 10 | 2 | 12 | 33.996120 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 10 | 3 | 12 | 44.533543 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | waagerecht | 10 | 1 | 12 | 25.028598 | 0.003345 | 0.020032 | 0.000513 |
| Pixelrauschen ±2 | waagerecht | 10 | 2 | 12 | 33.998264 | 0.001563 | 0.009122 | 0.002144 |
| Pixelrauschen ±2 | waagerecht | 10 | 3 | 12 | 44.533575 | 0.004445 | 0.016276 | 0.000032 |
| Pixelrauschen ±6 | waagerecht | 10 | 1 | 12 | 25.025682 | 0.010398 | 0.056627 | 0.003429 |
| Pixelrauschen ±6 | waagerecht | 10 | 2 | 12 | 34.000203 | 0.004871 | 0.020798 | 0.004083 |
| Pixelrauschen ±6 | waagerecht | 10 | 3 | 12 | 44.532197 | 0.012683 | 0.043123 | 0.001346 |
| Wechselnder Offset ±6 | waagerecht | 10 | 1 | 12 | 24.977258 | 0.750746 | 1.501492 | 0.051854 |
| Wechselnder Offset ±6 | waagerecht | 10 | 2 | 12 | 33.942868 | 1.045963 | 2.091927 | 0.053252 |
| Wechselnder Offset ±6 | waagerecht | 10 | 3 | 12 | 44.490274 | 1.360659 | 2.721319 | 0.043268 |
| Fester Offset +6 | waagerecht | 10 | 1 | 12 | 25.728004 | 0.000000 | 0.000000 | 0.698893 |
| Fester Offset +6 | waagerecht | 10 | 2 | 12 | 34.988831 | 0.000000 | 0.000000 | 0.992711 |
| Fester Offset +6 | waagerecht | 10 | 3 | 12 | 45.850934 | 0.000000 | 0.000000 | 1.317391 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 10 | 1 | 12 | 18.159549 | 3.143155 | 11.655922 | 6.869562 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 10 | 2 | 12 | 24.041017 | 4.436914 | 16.973512 | 9.955103 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 10 | 3 | 12 | 30.998480 | 5.865174 | 22.881368 | 13.535062 |
| Unverändert | waagerecht | 140 | 1 | 12 | 27.333704 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 140 | 2 | 12 | 17.970132 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 140 | 3 | 12 | 12.897875 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | waagerecht | 140 | 1 | 12 | 27.337659 | 0.004843 | 0.026002 | 0.003955 |
| Pixelrauschen ±2 | waagerecht | 140 | 2 | 12 | 17.971808 | 0.007350 | 0.049733 | 0.001676 |
| Pixelrauschen ±2 | waagerecht | 140 | 3 | 12 | 12.904328 | 0.006906 | 0.027273 | 0.006453 |
| Pixelrauschen ±6 | waagerecht | 140 | 1 | 12 | 27.338794 | 0.014432 | 0.077750 | 0.005090 |
| Pixelrauschen ±6 | waagerecht | 140 | 2 | 12 | 17.970637 | 0.017486 | 0.134335 | 0.000505 |
| Pixelrauschen ±6 | waagerecht | 140 | 3 | 12 | 12.910910 | 0.019277 | 0.078602 | 0.013035 |
| Wechselnder Offset ±6 | waagerecht | 140 | 1 | 12 | 27.293222 | 0.471646 | 0.943291 | 0.040482 |
| Wechselnder Offset ±6 | waagerecht | 140 | 2 | 12 | 17.897003 | 0.301568 | 0.603136 | 0.073129 |
| Wechselnder Offset ±6 | waagerecht | 140 | 3 | 12 | 12.899080 | 0.062630 | 0.125261 | 0.001205 |
| Fester Offset +6 | waagerecht | 140 | 1 | 12 | 27.764868 | 0.000000 | 0.000000 | 0.431164 |
| Fester Offset +6 | waagerecht | 140 | 2 | 12 | 17.595435 | 0.000000 | 0.000000 | 0.374697 |
| Fester Offset +6 | waagerecht | 140 | 3 | 12 | 12.836450 | 0.000000 | 0.000000 | 0.061425 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 140 | 1 | 12 | 19.320571 | 3.377655 | 12.727026 | 8.013132 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 140 | 2 | 12 | 13.948744 | 1.932426 | 7.285893 | 4.021388 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 140 | 3 | 12 | 11.009936 | 1.001204 | 3.750253 | 1.887939 |

Feste gemeinsame Offsets sind zeitlich vollkommen stabil, verändern aber die Vergleiche. Gemeinsame Veränderungen heben sich in ΔE00 nicht allgemein auf.
Die Störmodelle sind kontrollierte digitale Eingaben: unabhängiges gleichverteiltes RGB-Codewertrauschen, gemeinsame additive Offsets bzw. lineare Belichtungsänderung. Keine kalibrierten Sensor-/ISP-Modelle.

## Fünfermedian als Offline-Versuch

Acht vollständige Fenster pro Folge, Ausgabe ab Frame 5. Vergleich mit den Rohwerten derselben Frames 5–12; kein Vorfüllen. Kein produktiver Glätter oder Freigabekriterium.

| Folge | Richtung | Wand RGB | Feld | Roh-MAD ab Frame 5 | Median-MAD | Maximale Abweichung vom aktuellen Rohwert |
|---|---|---:|---:|---:|---:|---:|
| Unverändert | senkrecht | 10 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 10 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 10 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | senkrecht | 10 | 1 | 0.005646 | 0.001678 | 0.013711 |
| Pixelrauschen ±2 | senkrecht | 10 | 2 | 0.002005 | 0.000567 | 0.005214 |
| Pixelrauschen ±2 | senkrecht | 10 | 3 | 0.005306 | 0.003263 | 0.009713 |
| Pixelrauschen ±6 | senkrecht | 10 | 1 | 0.017090 | 0.009068 | 0.034180 |
| Pixelrauschen ±6 | senkrecht | 10 | 2 | 0.003998 | 0.000364 | 0.013963 |
| Pixelrauschen ±6 | senkrecht | 10 | 3 | 0.010579 | 0.007633 | 0.023126 |
| Wechselnder Offset ±6 | senkrecht | 10 | 1 | 0.750746 | 0.750746 | 0.000000 |
| Wechselnder Offset ±6 | senkrecht | 10 | 2 | 1.045963 | 1.045963 | 0.000000 |
| Wechselnder Offset ±6 | senkrecht | 10 | 3 | 1.360659 | 1.360659 | 0.000000 |
| Fester Offset +6 | senkrecht | 10 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | senkrecht | 10 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | senkrecht | 10 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 10 | 1 | 1.864360 | 2.214491 | 2.459619 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 10 | 2 | 2.556884 | 3.028857 | 3.510170 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 10 | 3 | 3.383897 | 3.958704 | 4.848595 |
| Unverändert | senkrecht | 140 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 140 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | senkrecht | 140 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | senkrecht | 140 | 1 | 0.001873 | 0.001028 | 0.012564 |
| Pixelrauschen ±2 | senkrecht | 140 | 2 | 0.004610 | 0.003690 | 0.041347 |
| Pixelrauschen ±2 | senkrecht | 140 | 3 | 0.007936 | 0.002720 | 0.011449 |
| Pixelrauschen ±6 | senkrecht | 140 | 1 | 0.006067 | 0.004997 | 0.031949 |
| Pixelrauschen ±6 | senkrecht | 140 | 2 | 0.011458 | 0.002313 | 0.101813 |
| Pixelrauschen ±6 | senkrecht | 140 | 3 | 0.015816 | 0.003850 | 0.042129 |
| Wechselnder Offset ±6 | senkrecht | 140 | 1 | 0.471646 | 0.471646 | 0.000000 |
| Wechselnder Offset ±6 | senkrecht | 140 | 2 | 0.301568 | 0.301568 | 0.000000 |
| Wechselnder Offset ±6 | senkrecht | 140 | 3 | 0.062630 | 0.062630 | 0.000000 |
| Fester Offset +6 | senkrecht | 140 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | senkrecht | 140 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | senkrecht | 140 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 140 | 1 | 1.895679 | 2.300315 | 2.824365 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 140 | 2 | 1.078621 | 1.154707 | 1.707609 |
| Belichtungswechsel 0 bis −2 EV | senkrecht | 140 | 3 | 0.601360 | 0.761385 | 1.171041 |
| Unverändert | waagerecht | 10 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 10 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 10 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | waagerecht | 10 | 1 | 0.005646 | 0.001678 | 0.013711 |
| Pixelrauschen ±2 | waagerecht | 10 | 2 | 0.002005 | 0.000567 | 0.005214 |
| Pixelrauschen ±2 | waagerecht | 10 | 3 | 0.005306 | 0.003263 | 0.009713 |
| Pixelrauschen ±6 | waagerecht | 10 | 1 | 0.017090 | 0.009068 | 0.034180 |
| Pixelrauschen ±6 | waagerecht | 10 | 2 | 0.003998 | 0.000364 | 0.013963 |
| Pixelrauschen ±6 | waagerecht | 10 | 3 | 0.010579 | 0.007633 | 0.023126 |
| Wechselnder Offset ±6 | waagerecht | 10 | 1 | 0.750746 | 0.750746 | 0.000000 |
| Wechselnder Offset ±6 | waagerecht | 10 | 2 | 1.045963 | 1.045963 | 0.000000 |
| Wechselnder Offset ±6 | waagerecht | 10 | 3 | 1.360659 | 1.360659 | 0.000000 |
| Fester Offset +6 | waagerecht | 10 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | waagerecht | 10 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | waagerecht | 10 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 10 | 1 | 1.864360 | 2.214491 | 2.459619 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 10 | 2 | 2.556884 | 3.028857 | 3.510170 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 10 | 3 | 3.383897 | 3.958704 | 4.848595 |
| Unverändert | waagerecht | 140 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 140 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Unverändert | waagerecht | 140 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Pixelrauschen ±2 | waagerecht | 140 | 1 | 0.001873 | 0.001028 | 0.012564 |
| Pixelrauschen ±2 | waagerecht | 140 | 2 | 0.004610 | 0.003690 | 0.041347 |
| Pixelrauschen ±2 | waagerecht | 140 | 3 | 0.007936 | 0.002720 | 0.011449 |
| Pixelrauschen ±6 | waagerecht | 140 | 1 | 0.006067 | 0.004997 | 0.031949 |
| Pixelrauschen ±6 | waagerecht | 140 | 2 | 0.011458 | 0.002313 | 0.101813 |
| Pixelrauschen ±6 | waagerecht | 140 | 3 | 0.015816 | 0.003850 | 0.042129 |
| Wechselnder Offset ±6 | waagerecht | 140 | 1 | 0.471646 | 0.471646 | 0.000000 |
| Wechselnder Offset ±6 | waagerecht | 140 | 2 | 0.301568 | 0.301568 | 0.000000 |
| Wechselnder Offset ±6 | waagerecht | 140 | 3 | 0.062630 | 0.062630 | 0.000000 |
| Fester Offset +6 | waagerecht | 140 | 1 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | waagerecht | 140 | 2 | 0.000000 | 0.000000 | 0.000000 |
| Fester Offset +6 | waagerecht | 140 | 3 | 0.000000 | 0.000000 | 0.000000 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 140 | 1 | 1.895679 | 2.300315 | 2.824365 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 140 | 2 | 1.078621 | 1.154707 | 1.707609 |
| Belichtungswechsel 0 bis −2 EV | waagerecht | 140 | 3 | 0.601360 | 0.761385 | 1.171041 |

Der Median beseitigt den festen Offset nicht. Bei Belichtungsänderung weicht sein Ergebnis vom aktuellen Frame ab; diese Verzögerung darf nicht als erfolgreiche Stabilisierung gewertet werden. Änderungen tatsächlicher Aufnahmeparameter brauchen weiterhin einen Historienwechsel.
