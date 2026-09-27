# Beleuchtung: unabhängige Vorwärtsmodelle

Analyse 0.5.5; 32 Einzelbilder: acht Lichtfelder, zwei Richtungen, ohne/mit Rauschen ±2; Seed 72931. Lineares RGB mal ortsabhängigem Lichtfaktor, dann sRGB und Quantisierung. Alle drei Materialfelder sind homogen. Keine Rekonstruktion, kein Kamera- oder Genauigkeitsnachweis.
Nur gleichmäßige Kontrollen und der konkret sanfte Verlauf besitzen hier eine vorab geforderte vollständige Freigabe. Gestörte Fälle werden ergebnisoffen untersucht; ein bestandener Untersuchungstest bedeutet ausdrücklich NICHT, dass deren Freigaben richtig sind. Detektion und Messung verwenden keinerlei Material- oder Licht-Sollwerte.

| Lichtfeld | Richtung | Rauschen | Erkannte Felder | Freigegeben | Status | Hinweis |
|---|---|---:|---:|---:|---|---|
| Gleichmäßig | senkrecht | 0 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gleichmäßig | senkrecht | 2 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gleichmäßig | waagerecht | 0 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gleichmäßig | waagerecht | 2 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gemeinsam halb so hell | senkrecht | 0 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gemeinsam halb so hell | senkrecht | 2 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gemeinsam halb so hell | waagerecht | 0 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Gemeinsam halb so hell | waagerecht | 2 | 2 | 2 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Sanfter Verlauf | senkrecht | 0 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Sanfter Verlauf | senkrecht | 2 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Sanfter Verlauf | waagerecht | 0 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Sanfter Verlauf | waagerecht | 2 | 3 | 3 | Measured | Farbabstand ΔE00 · kleiner = ähnlicher |
| Starker Querverlauf | senkrecht | 0 | 0 | 0 | AmbiguousPattern | Mehrere mögliche Muster. Nur den gewünschten Streifen ins Bild nehmen. |
| Starker Querverlauf | senkrecht | 2 | 3 | 2 | PartiallyMeasured | Einzelne Messflächen ungeeignet; gültige Felder bleiben auswertbar. |
| Starker Querverlauf | waagerecht | 0 | 3 | 2 | PartiallyMeasured | Einzelne Messflächen ungeeignet; gültige Felder bleiben auswertbar. |
| Starker Querverlauf | waagerecht | 2 | 3 | 2 | PartiallyMeasured | Einzelne Messflächen ungeeignet; gültige Felder bleiben auswertbar. |
| Starker Längsverlauf | senkrecht | 0 | 3 | 0 | InvalidFields | Farbfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Starker Längsverlauf | senkrecht | 2 | 3 | 0 | InvalidFields | Farbfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Starker Längsverlauf | waagerecht | 0 | 3 | 0 | InvalidFields | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Starker Längsverlauf | waagerecht | 2 | 3 | 0 | InvalidFields | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schattenkante durch Messflächen | senkrecht | 0 | 4 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schattenkante durch Messflächen | senkrecht | 2 | 4 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schattenkante durch Messflächen | waagerecht | 0 | 4 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schattenkante durch Messflächen | waagerecht | 2 | 4 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Farbiges Lichtgefälle | senkrecht | 0 | 2 | 0 | InvalidFields | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Farbiges Lichtgefälle | senkrecht | 2 | 2 | 0 | InvalidFields | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Farbiges Lichtgefälle | waagerecht | 0 | 2 | 0 | InvalidFields | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Farbiges Lichtgefälle | waagerecht | 2 | 2 | 0 | InvalidFields | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schatten zwischen Wand und Streifen | senkrecht | 0 | 3 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schatten zwischen Wand und Streifen | senkrecht | 2 | 3 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schatten zwischen Wand und Streifen | waagerecht | 0 | 3 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
| Schatten zwischen Wand und Streifen | waagerecht | 2 | 3 | 0 | InvalidReference | Messfläche räumlich ungleichmäßig. Gleichmäßigeres Licht und eine einheitliche Fläche verwenden. |
