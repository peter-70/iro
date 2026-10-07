# Offene Produktentscheidungen

Diese Datei dokumentiert offene Produktentscheidungen gemäß `MASTERPLAN.md`, Abschnitt 16. Sie definiert keine eigenständigen Produktanforderungen. Entscheidungen trifft ausschließlich der Nutzer.

## O-001 – Nearest vs. Match / No Match

**Status:** Offen  
**Bezug:** `MASTERPLAN.md`, Abschnitt 4.1–4.2 und Phase 6

**Fest:** Iro kann den farblich nächsten Kandidaten bestimmen.

**Frage:** Soll Iro zusätzlich feststellen, dass kein Feld ausreichend passt?

**Warum offen:** Die zusätzliche Match-/No-Match-Aussage ist noch nicht vom Nutzer freigegeben. Die Bestimmung des nächsten Kandidaten allein beantwortet diese Frage nicht.

**Bekannte Optionen:**

* Iro bestimmt ausschließlich den farblich nächsten Kandidaten.
* Iro bewertet zusätzlich, ob ein Feld ausreichend passt, und kann andernfalls „kein Feld passt“ feststellen. Die dafür erforderlichen Kriterien bedürfen einer ausdrücklichen Nutzerentscheidung.

**Messdaten/Befunde:** Für diesen Eintrag liegen keine ausgewerteten Messdaten vor. Gemäß Phase 6 sollen reale und synthetische Messdaten die spätere Entscheidung unterstützen.

**Bis zur Entscheidung ausdrücklich verboten:**

* Eine ΔE-Match-Schwelle festlegen oder verwenden.
* `IsNearest` als „passt“ interpretieren.
* Eine automatische No-Match-Entscheidung vorwegnehmen.

## O-002 – Repräsentativer Farbwert einer Messfläche

**Status:** Offen
**Bezug:** `MASTERPLAN.md`, Abschnitt 2.5 und 2.7; ausdrückliche Nutzerentscheidung zum Median-basierten Repräsentationswert.

**Fest:**

* Messgrundlage sind unveränderte Originalpixel.
* Es soll ein robuster Median-basierter Repräsentationswert verwendet werden.
* Ein fertiger Messwert ist gemäß MASTERPLAN 2.7 unveränderlich.

**Offen:**

* konkrete Medianform,
* genaue Ausreißerselektion,
* Reihenfolge zwischen Medianbildung und möglicher Qualitäts-/Ausreißerprüfung.

**Warum offen:** Die konkrete Messmethode ist noch nicht vom Nutzer entschieden. Abschnitt 2.7 regelt ausschließlich den Lebenszyklus des Messwertes.

**Bekannte Optionen:** Kanalweiser RGB-Median, Median nach Linear-RGB-Transformation oder geometrischer/vektorieller Median im Farbraum; keine dieser Optionen ist freigegeben.

**Messdaten/Befunde:** Der bisherige Sampler bildet nach Median/MAD-Selektion einen finalen Mittelwert. Die konkrete Medianform und Ausreißerselektion sind damit nicht entschieden.

**Bis zur Entscheidung:**

* `RegionSampler` nicht umbauen.
* Keine Medianform eigenmächtig festlegen.
