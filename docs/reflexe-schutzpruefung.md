# Reflexe und Glanzstellen: Untersuchung und begrenzte Absicherung

Stand: 24. September 2026. Analyse 0.5.10, Planfassung 1.45 (Untersuchungsstand aus Fassung 1.44). Grundlage: Nutzerauftrag, breit gefächerte Reflexfälle zu prüfen und verfälschte Messfreigaben zu verhindern. Der Gesamtpunkt bleibt offen.

## Bilder und vorab festgelegte Auswertung

108 Störungsbilder: neun Formen × drei Ziele (mittleres Feld, Wand, beide) × zwei Streifenrichtungen × ohne/mit Rauschen ±2, Seed 924137. Formen: einzelner Glanzpunkt, mehrere Punkte, kleiner/großer harter Fleck, weicher Glanz, Lichtband, farbiger Reflex, Reflex bis zum Kanalanschlag und gleichmäßige Überlagerung. Dazu acht saubere beziehungsweise helle Kontrollen und zwei echte Rechteckstreifen ohne konkurrierende Fortsetzung.

Die Fixture mischt Material- und Reflexlicht in linearem RGB, quantisiert anschließend nach sRGB und fügt gegebenenfalls Rauschen hinzu. Das ist ein synthetisches Vorwärtsmodell, kein validiertes Kameramodell. Die Originalmaterialfarben dienen ausschließlich der externen Auswertung; Iro erhält nur das Bild. Keine Bildkorrektur ist hinzugekommen.

Bei freigegebenen Feldern werden sowohl die gemessene Feldfarbe als auch die tatsächlich gewählte Wandreferenz gegen die bekannten Materialfarben geprüft. Ein Fehler über 1 ΔE00 oder eine Geometrieüberdeckung unter 80 % markiert eine offene Fehlfreigabe. Dies sind Untersuchungskriterien für deutliche Fehler, keine Freigabegrenzen oder Genauigkeitszusage der App. Saubere Kontrollen verlangen alle drei Felder und höchstens 0,15 ΔE00 bei Rauschen, ohne Rauschen lediglich Rechenrundung. Kleine Glanzpunkte verlangen weiterhin alle drei Felder. Abgelehnte Felder dürfen weder Zahlen noch einen nächsten Treffer liefern.

- [Vorheriger Befund mit Analyse 0.5.9](../tests/adjustments/reflexe-20260924/untersuchung-0.5.9.md): 16 offene Fehlfreigaben, 64 Sperren, 28 übrige Freigaben.
- [Aktueller Befund mit Analyse 0.5.10](../tests/adjustments/reflexe-20260924/untersuchung-0.5.10.md): zwölf offene Fehlfreigaben, 68 Sperren, 28 übrige Freigaben innerhalb digitaler Prüftoleranz.
- [Alle 108 PNG-Bilder](../tests/adjustments/reflexe-20260924/bilder).
- [Beispiel des korrigierten Lichtbands](../tests/adjustments/reflexe-20260924/bilder/Lichtband-Feld-senkrecht-rauschen-0.png).

Die Untersuchung behauptet keine richtige Ablehnung jedes gesperrten Falls. Insbesondere ein kleiner, kompakter Fleck wird teilweise konservativ gesperrt; die mögliche Auswertung anderer geeigneter Innenflächen ist nicht allgemein nachgewiesen. Die übrigen Freigaben sind nicht automatisch vollständig oder gerätegeeignet. Die erzeugten PNGs sind unabhängige Fixtures; ein ladbarer IroGen-JSON-Plan mit allen neun exakten Formen wurde hier nicht implementiert.

## Reproduzierter Fehler und gezielte Korrektur

Ein Lichtband zerlegt das mittlere Feld in drei nebeneinanderliegende Rechtecke. Iro erkannte diese als querliegenden Streifen und verwendete ein ignoriertes echtes Farbfeld als vermeintliche Wandreferenz. Vier Regressionen (beide Richtungen, mit/ohne Rauschen) scheiterten vor der Änderung.

Der Detektor prüft jetzt, ob außerhalb der gewählten Gruppe liegende Rechtecke eine quer dazu verlaufende Fortsetzung ihres gemeinsamen Umrisses bilden. Dafür gelten dieselben bestehenden Nachbarschaftsregeln: höchstens zwei Pixel Überlappung, Abstand höchstens 0,85 der größeren Querbreite, Breitenverhältnis mindestens 0,70 und Mittelpunktsversatz höchstens 0,13 der kleineren Querbreite. Die widersprüchliche Zuordnung wird als mehrdeutiges Muster ohne Messwerte abgelehnt. Es wird weder anhand heller Farbe auf einen Reflex geschlossen noch ein Farbwert rekonstruiert.

Zwei zusätzliche Gegenproben mit denselben drei Rechtecken ohne konkurrierende Fortsetzung bleiben vollständig messbar. Frühere Gegenproben für echte unterschiedliche Feldbreiten bleiben bestehen. Die Sperre ist kein allgemeiner Reflexdetektor: Fehlen sichtbare konkurrierende Konturen, ist diese konkrete Absicherung nicht anwendbar.

## Verbleibende Grenze – keine vollständige Schutzfreigabe

**Nutzerentscheidung vom 24. September 2026, Fassung 1.45:** Gleichmäßige Reflexüberlagerung ist kein zulässiger Messzustand: Aufnahme vollständig ablehnen, keine Messwerte und keinen nächsten Treffer anzeigen; Hinweis sinngemäß „Bild unbrauchbar. Bitte Reflexionen vermeiden und erneut aufnehmen.“ Keine rechnerische Rettung. Diese gewünschte Ablehnung setzt belastbare Erkennung voraus; für die zwölf homogenen Überlagerungsfälle fehlt sie weiterhin. Ihre Freigaben bleiben unerfüllte Schutzanforderungen, nicht akzeptierte Ausnahmen. Keine pauschale Sperre echter heller oder homogener Materialfarben und keine Generator-Sollwerte als versteckte Erkennungshilfe einführen. Der Nutzer lässt die bisherige Teilkorrektur vorerst bestehen; daraus folgt keine Abnahme oder Behauptung, die offene Sperre sei implementiert.

Zwölf Fälle gleichmäßiger Reflexüberlagerung geben weiterhin verfälschte Werte frei. In diesem Modell werden ganze homogene Materialflächen verändert, ohne einen innerhalb der Fläche erkennbaren Reflexrand. Die resultierenden Bildfarben können auch echte, andersfarbige Materialien beschreiben. Die unbekannte ursprüngliche Materialfarbe darf Iro nicht aus Testwissen rekonstruieren. Diese Fälle werden im Bericht weiterhin ausdrücklich als offene Fehlfreigaben geführt und nicht durch einen grünen Untersuchungstest für gelöst erklärt.

**Nachtrag vom 27. September 2026:** Die fehlende Unterscheidbarkeit ist inzwischen für alle zwölf Fälle durch bytegleiche saubere Gegenbilder konstruktiv belegt. Daraus folgt weiterhin keine Zulässigkeit der verfälschten Messung; vielmehr ist für eine sichere Sperre zusätzliche Aufnahmeinformation oder eine geänderte Produktstrategie erforderlich. [Beweis und Entscheidungsoptionen](reflexe-einzelbildgrenze-2026-09-27.md).

Die Schutzregression verlangt, dass keine weitere Form der aktuellen Matrix solche Fehlfreigaben zeigt. Die unverändert offene Klasse wird separat ausgewiesen. Das ist keine Abnahme ihrer Freigabe. Weitere Positionen, Deckungsanteile, spektrale Verläufe, Materialstrukturen, Perspektive, Kameraverarbeitung und reale Reflexe sind noch nicht umfassend abgedeckt.

## Ausgeführte Prüfungen

- 138 gezielte Kernprüfungen bestanden: Reflexuntersuchung und Regressionen, Geometrie, Beschnitt, Messregeln, Beleuchtung und Unschärfe. Darin elf Reflex-Testfälle mit mehrfachen Bildanalysen.
- 27 gezielte Integrationsprüfungen bestanden. Vier davon prüfen alle 108 Reflexbilder über echte PNG-Kodierung und den produktiven Analyseadapter, einschließlich Status, Geometrie, Freigabe, ΔE00, nächstem Treffer und Anzeige bei der korrigierten Fehlzuordnung.
- Anschließend Zwei-Bilder-Gegencheck bestanden: saubere Aufnahme und bekannte leichte Störung, einschließlich Geometrie, Messwerten und Rangfolge. Keine übersprungenen Tests.
- Schluss-Sammellauf, reale Gerätetests und ausdrückliche Abnahme bleiben ausstehend.

## Reproduktion

Vom Projektstamm aus:

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionEvidenceTests
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionPngTests
```

Die Untersuchung schreibt einen versionsbezogenen Markdown-Befund; die PNG-Prüfung erzeugt die Bilddateien erneut. Die Material-Sollwerte werden weiterhin ausschließlich im Test verwendet.

Android-Release-Build zur Reflex-Teilkorrektur am 24. September 2026 erfolgreich: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest oder Schluss-Sammellauf.
