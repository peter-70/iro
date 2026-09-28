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
