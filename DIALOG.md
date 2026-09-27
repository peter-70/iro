# Arbeitsprotokoll – 27. September 2026

## Implementierter Punkt: Unterbelichtung ohne Clipping am relativen Vergleich absichern

Der bisherige 16-Bilder-Unterbelichtungsplan enthielt vier geprüfte dunkle Originalfarbkontrollen und zwölf bewusst unbewertete Fälle mit ganzbildweiter Abdunklung. Für diese zwölf Fälle sind jetzt geprüfte Erwartungen hinterlegt: drei geometrisch sicher zugeordnete und freigegebene Felder sowie die relative Reihenfolge `field-3 < field-2 < field-1`. Enthalten sind −1, −2 und −3 EV, beide Streifenrichtungen sowie jeweils kein und leichtes Rauschen.

Der Integrationstest prüft alle 16 Bilder auf fehlerfreie Verarbeitung, vollständige Feldzuordnung, mindestens 90 % geometrische Überdeckung, fehlende Fremdflächen, Freigabe aller drei Felder, erwartete Rangfolge und richtige Kennzeichnung des ähnlichsten Feldes. Absolute Materialwerte vor der Belichtung werden ausdrücklich nicht als Sollwerte verwendet.

Es wurde keine Helligkeitssperre, Bildkorrektur, Produktlogik oder EV-Grenze ergänzt. Planfassung 1.57 hält fest: Absolute Dunkelheit allein ist kein Fehler. Eine Sperre benötigt belegten Informationsverlust oder eine unzuverlässige relative Wand-/Farbfeldbeziehung.

## Roter Erstlauf und Korrektur der Testannahme

Der erste verschärfte Lauf war rot: 12 von 16 Erwartungen wurden als verletzt gemeldet. Ursache war keine Iro-Fehlmessung, sondern eine falsche neue Testerwartung. Der Plan setzt die Wand ausdrücklich auf sRGB 140 und die Felder auf 60, 100 und 160. Deshalb ist `field-3` und nicht `field-1` am nächsten. Die Erwartung wurde anhand dieser festgelegten Farben auf `field-3 < field-2 < field-1` korrigiert. Iro und seine Messlogik wurden nicht an den Test angepasst.

## Konkret ausgeführte Prüfungen

    dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~UnderexposurePlanTests --logger "console;verbosity=minimal"

Erster Lauf: Fehler 1, erfolgreich 0, gesamt 1; 12 von 16 Bild-Erwartungen rot wegen der falschen Annahme zur Wandfarbe.

Erneuter Lauf nach Korrektur der Erwartung: Fehler 0, erfolgreich 1, übersprungen 0, gesamt 1, Dauer 5 s. Der Test erzeugte und analysierte 16 PNG-Bilder: 16 Erwartungen erfüllt, 0 nicht erfüllt, 0 nicht bewertet, 0 Prüffehler.

    dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~UnderexposureEvidenceTests --logger "console;verbosity=minimal"

Ergebnis: Fehler 0, erfolgreich 8, übersprungen 0, gesamt 8, Dauer 191 ms. Dunkle geeignete Originalfarben blieben messbar; möglicher Quantisierungsverlust ohne exakten Kanalendpunkt blieb als nicht allgemein aus dem Einzelbild unterscheidbare Grenze belegt.

    dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter "FullyQualifiedName~UnderexposurePlanTests|FullyQualifiedName~RankingExpectationTests|FullyQualifiedName~TestPlanTests" --logger "console;verbosity=minimal"

Ergebnis: Fehler 0, erfolgreich 22, übersprungen 0, gesamt 22, Dauer 5 s. Unterbelichtungsplan, Rangfolgevertrag und allgemeiner Testplanvertrag bestanden gemeinsam.

    dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --logger "console;verbosity=minimal"

Ergebnis: Fehler 0, erfolgreich 1, übersprungen 0, gesamt 1, Dauer 877 ms. Sauberes Bild und bekannte leichte Rauschstörung bestanden Freigabe, Feldzuordnung, Messwerte und Rangfolge.

Zusätzliche Strukturprüfung: 16 Testfälle, 16 geprüfte Erwartungen, davon 12 mit geprüfter relativer Rangfolge; `git diff --check` ohne Formatfehler. Vollständige Testprojekte, Solution-/Android-Build, Abschluss-Sammellauf und reale Gerätetests wurden bewusst noch nicht vorgezogen.

Bewertung nach dem Grundprinzip: In den zwölf Belichtungsfällen wurden Wand und Farbstreifen durch dieselbe Ganzbildtransformation beeinflusst. Der relative Vergleich funktionierte bis einschließlich der untersuchten synthetischen −3 EV in allen Fällen. Eine unterschiedliche Beeinflussung trat nicht auf. Daraus folgt keine Korrektur oder Sperre. Die Kerngegenprobe zeigt zugleich, dass extreme Quantisierung Information auch ohne Codewert 0 vernichten kann; eine allgemeine sichere Grenze lässt sich daraus im Einzelbild nicht ableiten und bleibt für reale Gerätetests offen.

## Nächster offener Punkt: Kanalanschlags- und Clipping-Sperre relativ auditieren

Als Nächstes wird die bestehende aufnahmeweite Sperre bei relevanten Pixelanteilen am Kanalendpunkt geprüft. Zu belegen ist, wann Endpunktpixel tatsächlich relative Farbinformation vernichten. Gleichzeitig muss gesichert bleiben, dass echte sehr dunkle, helle oder gesättigte Materialfarben nicht allein wegen ihres absoluten Farbwerts gesperrt werden. Die vorhandene Zwei-Prozent-Versuchsgrenze wird ohne belastbare Gegenfälle weder gelockert noch verschärft.
## Code-/Planabgleich vom 27. September 2026 – Prüfbericht, kein neuer Arbeitsauftrag

Auf ausdrücklichen Nutzerauftrag Code von Iro und IroGen gegen Planfassung 1.57 geprüft; Befunde mit unveränderter Produktlogik in Planfassung 1.58 und [Auditbericht](docs/code-plan-abgleich-2026-09-27.md) dokumentiert.

Aktuelle Prüfungen: 79 ausgewählte Kerntests, davon 73 bestanden und sechs fehlgeschlagen, null übersprungen. Alle sechs Fehler betreffen vorhandene positive Gegenfälle mit echten homogenen Kanalendpunktfarben. Andere vorhandene Tests erwarten für dieselbe Farbe noch eine Sperre. 25 ausgewählte Generator-/Integrationsprüfungen bestanden, einschließlich Unterbelichtung, Feld-/Rangerwartungen, Anzeigewechsel und Zwei-Bilder-Gegencheck. Inventarvorschau scheitert an veralteten Unterbelichtungszahlen; auch die alte Reflexschleier-Sperrforderung im Inventar ist überholt. Maschinenlesbare Ergebnisse unter tests/adjustments/codeaudit-20260927.

Relative Bewertung: Die Endpunkt-Gegenfälle enthalten keine simulierte unterschiedliche Aufnahmebeeinflussung von Wand und Streifen; die pauschale Sperre verhindert den Vergleich. Gleichmäßige Abdunklung und bildweites leichtes Rauschen erhalten in den geprüften Integrationsfällen die relative Zuordnung und Rangfolge. Unbrauchbare Unschärfe vermischt Farben über Grenzen und wird in der Anzeigegegenprobe korrekt gesperrt. Keine neue Korrektur, Schwelle oder pauschale Freigabe von Endpunktbildern abgeleitet.

Nächster empfohlener Arbeitspunkt bleibt die fachlich konsistente Kanalendpunkt-/Clipping-Prüfung samt widersprüchlichen Tests. Danach Inventar synchronisieren und vollständigen Android-Testmodus mit Overlay, sichtbarer Referenz, nächstem Treffer, Einstellungen und Testquelle fertigstellen. Kamera erst nach bestandenem technischem Sammellauf. Dies dokumentiert Empfehlungen des Audits; Umsetzung, Sammellauf, Geräteprüfung und ausdrückliche Abnahmen wurden nicht vorgezogen.
