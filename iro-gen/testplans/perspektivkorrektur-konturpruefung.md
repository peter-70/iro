> **Neu: automatische Soll-Ist-Prüfung.** Den JSON-Plan erneut laden und an Iro senden, mit **IroGen 1.5.0 / Analyse 0.5.4**. Erwartet werden **10 erfüllt, 0 nicht erfüllt, 0 nicht bewertet, 0 Prüffehler**. Bildoptionen unverändert; der Plan verwendet jetzt Format 2 mit geprüften Erwartungen. Die nachfolgenden Angaben zu älteren Versionen und manueller Auswertung sind historisch. [Aktuelle Anleitung](../../docs/verhaltenserwartungen.md).
> **Aktueller Stand, 23. September 2026:** Die bekannte kombinierte Fehlfreigabe ist in Analyse **0.5.3** gezielt behoben. Derselbe unveränderte JSON-Plan erfüllt im neuen Probelauf **10/10 Erwartungen**. IroGen 1.4.0 neu starten und auf Analyseversion 0.5.3 im Bericht achten. [Neuer Soll-Ist-Nachweis](../../tests/adjustments/konturplan-20260923-korrigiert/soll-ist.md). Die unten beschriebenen 9/10 und der offene Fehler sind historische Befunde unter 0.5.2. Eine Nutzerabnahme steht weiterhin aus.
# IroGen: JSON-Testplan für die korrigierte Perspektivprüfung

Aktualisiert: 23. September 2026. Dieser Abschnitt entspricht dem klargestellten Nutzerauftrag: Bilder in IroGen erzeugen, an Iro senden und den exportierten Bericht prüfen. Die frühere PowerShell-Anleitung unten bleibt als ergänzender Entwicklertest erhalten.

## Datei und Ablauf

1. Die neu gebaute IroGen-Version **1.4.0** starten. Eine bereits offene ältere Instanz schließen; sie kennt die neue Feldbreitenoption noch nicht. Der geprüfte Build liegt unter iro-gen/bin/Release/net10.0-windows/IroGen.exe.
2. Über **„Testplan laden“** die Datei [perspektivkorrektur-konturpruefung.json](perspektivkorrektur-konturpruefung.json) auswählen.
3. Erzeugung aller **10 Bilder** abwarten. Die Anzahl stammt aus dem Plan; das Eingabefeld „Anzahl Bilder“ ist dafür nicht maßgeblich.
4. **„An Iro-Tests senden“** anklicken und Analyseabschluss abwarten.
5. **„Testergebnisse auswerten“** öffnen, den neuen Lauf mit 10 Bildern sowie Generator 1.4.0 / Analyse 0.5.2 kontrollieren. „Nur Läufe mit Problemen“ ausschalten, damit auch passende Kontrollen sichtbar sind.
6. Den Markdown-Bericht exportieren und hier bereitstellen.

## Sollverhalten für die zehn Bilder

| Fälle | Anzahl | Vorab festgelegte Erwartung |
|---|---:|---|
| Rechtecke mit abnehmender bzw. zunehmender Breite, senkrecht und waagerecht | 4 | Alle drei Felder freigegeben; kein Hinweis auf frontalere Aufnahme. |
| Gleich breite Kontrollfelder, senkrecht und waagerecht | 2 | Alle drei Felder freigegeben. |
| Deutliche Verjüngung, senkrecht und waagerecht | 2 | Keine Messfreigabe, Hinweis auf frontalere Aufnahme. |
| Deutliche Verjüngung mit −0,7 Blenden und leichtem Rauschen, senkrecht und waagerecht | 2 | Keine Messfreigabe, Hinweis auf frontalere Aufnahme. |

Die Namen der Testfälle enthalten das Sollverhalten und erscheinen im Bericht. Die bisherige nominale Bewertung des Auswertungsdialogs ist **keine automatische Soll-Ist-Prüfung dieser Erwartungen**. Wir vergleichen Freigaben und Hinweise anhand der Tabelle. Eine korrekte Abweisung ist hier ein Erfolg, eine nominal unauffällige Messung allein kein Beleg der Geometrieeignung.

## Eigener Probelauf und offene Schutzlücke

Der vollständige Weg über PNG-Erzeugung, Testübergabe, Analyse und Markdown-Export wurde am 23. September 2026 ausgeführt. **Neun von zehn Erwartungen erfüllt.** Alle vier unterschiedlichen Rechteckbreiten werden richtig gemessen, ebenso beide Kontrollen. Beide ungestörten Verjüngungen und die dunkle/verrauschte senkrechte Verjüngung werden gesperrt.

**Offener Fehler:** Bei „Verjuengung dunkel verrauscht waagerecht“ gibt Analyse 0.5.2 zwei von drei Feldern frei. Erwartet ist die vollständige Sperre mit Frontalhinweis. Der Fall bleibt unverändert im Plan. Kein vollständiger Erfolgsnachweis und keine Abnahme.

[Exportierter Probelauf](../../tests/adjustments/konturplan-20260923/bericht.md) · [Expliziter Soll-Ist-Vergleich](../../tests/adjustments/konturplan-20260923/soll-ist.md).

Die Generator-Verjüngung ist ein synthetisches Modell, keine universelle physikalische Winkelgrenze. Rauschen und Belichtung unterscheiden sich bewusst von den einfachen RGB-Kerntests. Damit prüfen wir den tatsächlichen Generatorweg zusätzlich; wir behaupten keine identischen Bildbedingungen.

## Frühere ergänzende Entwickleranleitung

Die folgende Anleitung betrifft die 22 Kerntests und ersetzt den oben beschriebenen IroGen-Lauf nicht.
# Testplan: Perspektivprüfung anhand der Feldkonturen

Datum: 23. September 2026 · Ziel: Analyse 0.5.2 oder neuer mit dieser Korrektur  
Grundlage: [verbindlicher Plan, Fassung 1.30](../../IRO-KONSOLIDIERTER-PLAN.md), [Arbeitsplan](../../docs/arbeitsplan-bildoptimierung.md), [Geometrienachweis](../../docs/geometrie-schutzpruefung.md).

## Ziel und Umfang

Unterschiedlich breite rechteckige Felder dürfen allein wegen ihrer Breitenunterschiede keinen Perspektivhinweis auslösen. Deutlich zusammenlaufende Feldkonturen müssen weiterhin die ganze Messung sperren. Passende und ungeeignete Gegenbeispiele werden gemeinsam geprüft, damit pauschales Ablehnen nicht als Verbesserung gilt.

**Diese Markdown-Datei ist eine Prüfanleitung, keine in IroGen ladbare Konfiguration.** Die exakten Gegenbeispiele erzeugen bereits vorhandene Kerntests automatisch. Sie prüfen direkt die Kernanalyse von Iro. Bilder oder Generatoroptionen müssen dafür nicht von Hand erstellt werden.

Dieser Plan prüft die begrenzte Konturkorrektur und benachbarte Schutzregeln. Er ersetzt keine allgemeine Perspektiv-, Kundenoberflächen- oder Farbgenauigkeitsabnahme.

## Durchführung für den Nutzer

1. PowerShell öffnen.
2. Den folgenden Block ausführen. Er baut die aktuelle Kernanalyse und führt ausschließlich die gezielten Geometriefälle aus.
3. Erwartet werden **22 erfolgreich, 0 fehlgeschlagen, 0 übersprungen**. Keine gefundenen Tests zählen nicht als Erfolg.
4. Die erzeugte Datei **testausgabe.txt** zur Auswertung bereitstellen. Eine große results.json ist nicht erforderlich.

~~~powershell
Set-Location -LiteralPath 'D:\Source\iro'
$pruefordner = Join-Path $PWD ('tests/runs/perspektivkorrektur-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $pruefordner -Force | Out-Null

dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter 'FullyQualifiedName~GeometrySafetyTests' --logger 'console;verbosity=normal' --logger 'trx;LogFileName=geometrie.trx' --results-directory $pruefordner 2>&1 |
    Tee-Object -FilePath (Join-Path $pruefordner 'testausgabe.txt')

$testergebnis = $LASTEXITCODE
Write-Host "Exitcode: $testergebnis"
Write-Host "Bericht: $(Join-Path $pruefordner 'testausgabe.txt')"
~~~

Bei fehlenden wiederhergestellten Paketen zunächst im Projektstamm ausführen, dann den Testblock wiederholen:

~~~powershell
dotnet restore iro.slnx --locked-mode
~~~

Ein fehlgeschlagener Restore oder Build ist ein technisches Hindernis, kein Bildbefund. Keine Paketversionen dafür ändern. Bei inzwischen veränderter Testanzahl erst den Unterschied prüfen, nicht einfach die Erwartung anpassen.

## Testfälle und Sollverhalten

Die Bilder sind deterministische synthetische RGB-Puffer: senkrecht 800 × 600 Pixel, waagerecht um 90 Grad gedreht. Standardoptionen einschließlich Geraderichten bleiben aktiv. Die Analyse bekommt keine Sollgeometrie oder Generatorparameter.

| Verhalten | Ausführung | Fälle | Erwartung |
|---|---|---:|---|
| Rechteckige Felder unterschiedlicher Breite | Drei Felder, Breiten 180/150/120 Pixel, jeweils 110 Pixel lang; senkrecht/waagerecht und breiteres Ende jeweils auf beiden Seiten | 4 | Kein Hinweis auf frontalere Aufnahme, keine Ablehnung wegen ungeeigneter Geometrie, mindestens ein Feld freigegeben. Die vollständige Freigabe aller drei Felder wird vom bestehenden Test nicht ausdrücklich nachgewiesen. |
| Tatsächlich verjüngte Konturen | Drei trapezförmige Felder mit kontinuierlich abnehmender Breite; beide Streifenrichtungen und beide Verjüngungsrichtungen | 4 | Ganze Aufnahme wegen Geometrie sperren, frontalere Aufnahme verlangen, kein Feld freigeben. |
| Verjüngung mit Abdunklung und Störpixeln | Dieselben vier Fälle; gesamtes Bild auf 65 % der Kanalwerte skalieren, deterministische Abweichungen von −2 bis +2 Codewerten hinzufügen | 4 | Dieselbe vollständige Geometriesperre und derselbe sinngemäße Hinweis. Kein Nachweis realer Sensor- oder Belichtungsgrenzen. |
| Schmale rechteckige Felder | Drei Felder mit 60 × 110 Pixeln; beide Richtungen | 2 | Alle drei Felder messbar, kein Perspektivhinweis. Dies beweist keine Eignung realer stark schräger Fotos. |
| Unterschiedliche Feldlängen und geringe Breitenunterschiede | Längen 60/140/90/150 Pixel, Breiten 170/168/166/164 Pixel; beide Richtungen | 2 | Alle vier Felder messbar. |
| Kleine Reste eines angeschnittenen Feldes | Sichtbarer Feldrest von 6 oder 16 Pixeln am Rand, zwei ganze Felder dahinter; beide Richtungen | 4 | Ganze Aufnahme wegen Geometrie sperren, vollständigen Streifen verlangen, kein Feld freigeben. |
| Unabhängiger Randfleck | Kleiner Fleck am Bildrand, räumlich neben vollständigem Streifen; beide Richtungen | 2 | Beide vollständigen Felder messbar. |
| **Gesamt** | | **22** | **Alle Erwartungen erfüllt.** |

Die vier Fälle mit unterschiedlichen rechteckigen Feldbreiten schlugen unter Analyse 0.5.1 fehl. Dieser historische Vorher-Befund ist im Geometrienachweis dokumentiert. Für die aktuelle Prüfung muss kein alter Arbeitsstand wiederhergestellt werden.

Die exakte Bildkonstruktion und die automatischen Prüfungen stehen in [GeometrySafetyTests.cs](../../tests/iro.core.tests/GeometrySafetyTests.cs). Diese Fälle sind Entwicklungsregressionen, kein zurückgehaltener unabhängiger Abnahmesatz.

## Fehlermeldungen verständlich zuordnen

| Technischer Name in der Ausgabe | Bedeutung |
|---|---|
| RectangularFieldsWithDifferentWidthsDoNotProvePerspective | Rechteckige Felder verschiedener Breite dürfen keinen falschen Perspektivhinweis auslösen. |
| StrongCoherentTaperStopsTheWholeStrip | Verjüngte Konturen müssen die ganze Aufnahme sperren; Parameter unterscheiden Richtung und zusätzliche Abdunklung/Störpixel. |
| NarrowRectanglesAloneDoNotProvePhysicalCameraAngle | Schmale Rechtecke beweisen keinen schrägen Kamerawinkel. |
| DifferentFieldLengthsAndSlightTaperRemainMeasurable | Verschiedene Feldlängen und geringe Breitenunterschiede bleiben zulässig. |
| SmallVisibleRemnantOfCroppedFieldStillStopsMeasurement | Auch kleine passende Randreste müssen Streifenbeschnitt erkennen lassen. |
| UnrelatedSmallBorderPatchDoesNotInvalidateCompleteStrip | Ein unabhängiger Randfleck darf den vollständigen Streifen nicht ungültig machen. |

Bei Fehlern den Textbericht vollständig übermitteln. Erwartung und tatsächlichen Befund getrennt notieren; Testschwellen nicht ändern, um einen grünen Lauf zu erzielen.

## Ergänzende Prüfung über IroGen

Optional den vorhandenen JSON-Testplan [Raumlage, Perspektive und Wandabstand](raumlage-perspektive-und-wandabstand.json) auf weitere Regressionen prüfen. Er ersetzt die exakten Gegenbeispiele oben nicht.

Aktuelle IroGen-Version starten → „Testplan laden“ und die JSON-Datei auswählen → Erzeugung abwarten → **„An Iro-Tests senden“** → Analyseabschluss abwarten → „Testergebnisse auswerten“ → Datum und Analyseversion des neuen Laufs prüfen → Markdown-Bericht exportieren.

Nominale Farbabstände bleiben Entwicklungsdiagnosen. Nominal unauffällige Ergebnisse beweisen weder korrekte Perspektiverkennung noch reale Farbgenauigkeit. Für diesen ergänzenden Lauf wird hier keine pauschale Sollzahl freigegebener Bilder festgelegt.

## Ergebnis und Abnahme

Nach dem neuen Lauf ausfüllen; frühere erfolgreiche Läufe zählen hier nicht als aktuelle Durchführung.

- Prüfdatum und prüfende Person:
- Geprüfter Commit oder Arbeitsstand einschließlich lokaler Änderungen:
- Analyseversion:
- Pfad des Textberichts:
- Erfolgreiche / fehlgeschlagene / übersprungene Fälle:
- Auffälligkeiten:

| Prüfschritt in Klartext | Ergebnis: bestanden / fehlgeschlagen / nicht geprüft | Bemerkung |
|---|---|---|
| Unterschiedliche rechteckige Feldbreiten lösen keinen falschen Perspektivhinweis aus. | nicht geprüft | |
| Deutlich verjüngte Konturen sperren die gesamte Messung. | nicht geprüft | |
| Die Sperre bleibt bei den geprüften Kombinationen mit Abdunklung und Störpixeln wirksam. | nicht geprüft | |
| Geeignete schmale Felder und unterschiedliche Feldlängen bleiben im beschriebenen Umfang messbar. | nicht geprüft | |
| Beschnittschutz und Abgrenzung unabhängiger Randflecken bleiben erhalten. | nicht geprüft | |

- [x] Testplan erstellt und mit vorhandenen Testfällen abgeglichen.
- [ ] Neuer gezielter Testlauf mit allen 22 Fällen bestanden und Bericht gesichert.
- [ ] Nutzerabnahme ausdrücklich erteilt.
- [ ] Begrenzte Korrektur erledigt: Prüfung und Nutzerabnahme liegen beide vor.

**Abnahmetext:** „Ich nehme die Korrektur gegen falsche Perspektivablehnung im hier geprüften Umfang ab. Die geeigneten Gegenbeispiele bleiben im beschriebenen Umfang messbar; die geprüften deutlich verjüngten und angeschnittenen Streifen werden gesperrt.“

**Weiterhin offen:** allgemeine Perspektiverkennung, besonders gleichmäßige Verkürzung bei unbekannter realer Feldform; beliebige Drehungen und weitere kombinierte Störungen; Kundenoberfläche und reale Geräteprüfung; reale Farbgenauigkeit. Eine Abnahme dieser begrenzten Korrektur schließt diese Punkte nicht.
