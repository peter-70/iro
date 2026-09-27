# Robustheit der Feldzuordnung bei gleichmäßigem Reflexschleier

Stand: 27. September 2026. Planfassung 1.54. IroGen 1.6.0, Analyse 0.5.15.

## Auftrag und richtige Antwort

Der Anwender nimmt genau ein Bild auf und kennt keine Farbwerte oder Codes. Für diese Untersuchung gilt eine Antwort als vollständig richtig, wenn Iro genau das tatsächlich zur Wand identische Farbfeld als ähnlichsten freigegebenen Treffer markiert. Jede zusätzliche Markierung zählt als falscher Treffer; fehlt die Markierung des passenden Feldes, zählt dies als übersehener Treffer. Numerische ΔE00-Abweichungen werden getrennt betrachtet und waren nicht das Erfolgskriterium dieses Auftrags.

IroGen besitzt dafür nun eine reproduzierbare, ganzbildweite Testskala von 0 bis 10. Stufe 0 verändert das Bild nicht. Jede weitere Stufe mischt vier Prozentpunkte Weiß in alle kodierten sRGB-Farbkanäle des gesamten Bildes; Stufe 10 entspricht 40 %. Das ist ein synthetisches Vorwärtsmodell, keine Kamera- oder Materialsimulation und kein Produktgrenzwert. Der Stufenwert gelangt niemals in den Analyzer.

Zusätzlich stehen vier reproduzierbare Farbprofile bereit: allgemein, hell, blass sowie gering gesättigt. Die Profile beeinflussen ausschließlich die Testfarbenerzeugung. Alte IroGen-Aufnahmen und Optionsdateien der Versionen 1.0.0 bis 1.5.0 bleiben lesbar.

## Matrix und Ergebnis

Je Stufe wurden 80 verschiedene PNG-Bilder ausgewertet: vier Farbprofile mit je 20 Seeds. Insgesamt waren es 880 Einzelbilder. Feldgeometrie und das richtige Bezugsfeld wurden erst nach der reinen PNG-Analyse zur Bewertung herangezogen. Iro erhielt weder Stufe noch Profil, Seed, Sollfarben, Feldmasken oder erwartete Antwort.

| Stufe | Weißmischung | vollständig richtig | falsche Treffer | übersehene Treffer | 97-%-Ziel |
|---:|---:|---:|---:|---:|---|
| 0 | 0 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 1 | 4 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 2 | 8 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 3 | 12 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 4 | 16 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 5 | 20 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 6 | 24 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 7 | 28 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 8 | 32 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 9 | 36 % | 80/80 (100 %) | 0 | 0 | erreicht |
| 10 | 40 % | 80/80 (100 %) | 0 | 0 | erreicht |

Das Mindestziel von 97 % ist damit bis einschließlich Stufe 2 erfüllt und wurde in dieser Matrix auch für Stufe 3 sowie alle höheren untersuchten Stufen erfüllt. Jedes einzelne Farbprofil erreichte auf jeder Stufe 20 von 20 vollständig richtige Antworten. Es traten keine Verarbeitungsfehler auf.

Da keine falsche oder übersehene Antwort auftrat, gab es gemäß Auftrag keine fehlerhaften Stufen, für die ein Warnmerkmal gesucht werden musste. Es wurde keine Sperre, Warnung, Reflexerkennung oder sonstige Produktlogik ergänzt.

Der frühere Nachweis bleibt bestehen: Ein gleichmäßiger Schleier kann bytegleich zu einer sauberen Szene mit anderen echten Materialfarben sein. Die neue Matrix beantwortet eine andere, für die App zentrale Frage: Die relative Wahl des tatsächlich passenden Feldes blieb trotz der gemeinsamen Überlagerung stabil. Sie beweist weder physikalische Farbgenauigkeit noch Zuverlässigkeit bei räumlich ungleichmäßigen Reflexen oder realen Kamerabildern. Die numerischen Abstände können durch den Schleier verändert sein, obwohl der richtige Treffer erhalten bleibt.

Der vollständige automatisch erzeugte Tabellenbericht mit allen Farbprofilen liegt unter [robustheit-0.5.15.md](../tests/adjustments/reflexschleier-20260927/robustheit-0.5.15.md).

## Ausgeführte Prüfungen

Werkzeuge: .NET SDK 10.0.401, PowerShell 7.6.5, xUnit.net VSTest Adapter 3.1.4.

Gezielte Skalenprüfung:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionVeilRobustnessTests.ScaleIsReproducibleWholeImageAndRejectsInvalidLevels --logger "console;verbosity=minimal"
```

Schlusszeile: `Bestanden! : Fehler: 0, erfolgreich: 1, übersprungen: 0, gesamt: 1, Dauer: 263 ms`.

880-Bilder-Untersuchung:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionVeilRobustnessTests.MeasuresMatchingFieldRobustnessAcrossElevenVeilLevels --logger "console;verbosity=minimal"
```

Schlusszeile: `Bestanden! : Fehler: 0, erfolgreich: 1, übersprungen: 0, gesamt: 1, Dauer: 45 s`.

Abschließender gezielter Satz aus neuer Matrix sowie angrenzenden Generator-, Testplan- und Berichtsfunktionen:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter "FullyQualifiedName~ReflectionVeilRobustnessTests|FullyQualifiedName~GeneratorTests|FullyQualifiedName~TestPlanTests|FullyQualifiedName~ReviewDiagnosticsTests" --logger "console;verbosity=minimal"
```

Schlusszeile: `Bestanden! : Fehler: 0, erfolgreich: 76, übersprungen: 0, gesamt: 76, Dauer: 45 s`.

Verpflichtender Gegencheck mit sauberem Bild und bekannter leichter Rauschstörung:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --logger "console;verbosity=minimal"
```

Schlusszeile: Bestanden – Fehler: 0, erfolgreich: 1, übersprungen: 0, gesamt: 1, Dauer: 808 ms. [Erzeugter Zwei-Bilder-Bericht](../tests/adjustments/zwischenkontrolle-20260923/iro-run-5ae5c2655670460ba03d29d0c3172fb6/bericht.md).

Bewusst nicht ausgeführt: vollständige Kern- und Generator-Testprojekte, Restore, Solution-Build, Android-Build, Abschluss-Sammellauf und reale Gerätetests. Der Iro-Produktcode wurde nicht geändert; die vollständigen Läufe bleiben für den vereinbarten Abschluss vorgesehen.

## Verbindliche Entscheidung auf Grundlage des relativen Vergleichs

Iro ist kein absolutes Farbmessgerät. Wand und Farbreferenz werden im selben Foto unter denselben Aufnahmebedingungen relativ verglichen. Daher rechtfertigt eine gleichmäßig auf beide wirkende globale Veränderung keine Sperre, solange der relative Vergleich nachweislich zuverlässig bleibt.

Für die synthetische 0–10-Matrix ist die maßgebliche Aussage belegt: Genau das tatsächlich zur Wand identische Feld bleibt der einzige ähnlichste freigegebene Treffer. Der automatisierte Test ist ab Planfassung 1.56 ein echter Regressionsvertrag und muss bei jedem falschen oder übersehenen passendsten Feld sowie bei jedem Verarbeitungsfehler fehlschlagen.

Die Entscheidung behauptet keine unveränderten numerischen ΔE00-Werte, keine absolute Materialfarbe und keine allgemeine Unbedenklichkeit realer Reflexe. Numerische Werte beschreiben den relativen Abstand in der aktuellen Aufnahme. Räumlich ungleiche Reflexe, Clipping, verlorene Information oder eine nachgewiesene Änderung der relativen Feldbeziehung bleiben Gründe für Warnung oder Sperre. Es wurde keine Bildkorrektur, Reflexerkennung oder Produktschwelle ergänzt.
## Erneute Vertragsprüfung nach der Entscheidung in Fassung 1.56

Der Matrixlauf schreibt nicht mehr nur einen Bericht. Er schlägt nun bei jedem falschen oder übersehenen passendsten Feld sowie bei jedem Verarbeitungsfehler fehl.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionVeilRobustnessTests --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 2, übersprungen 0, gesamt 2, Dauer 26 s. Enthalten sind die reproduzierbare Ganzbildtransformation und 880 PNG-Einzelbildanalysen.

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionEvidenceTests --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 23, übersprungen 0, gesamt 23, Dauer 13 s.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionPngTests --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 4, übersprungen 0, gesamt 4, Dauer 15 s. Die beiden letzten Läufe sichern bestehende Schutzfälle mit räumlich ungleicher Wirkung; die relative Entscheidung lockert sie nicht.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --logger "console;verbosity=minimal"
```

Ergebnis: Fehler 0, erfolgreich 1, übersprungen 0, gesamt 1, Dauer 865 ms. Sauberes Bild und bekannte leichte Rauschstörung bestanden Freigabe, Feldzuordnung, Messwerte und Rangfolge.

Bewertung nach dem Grundprinzip: Beim synthetischen Reflexschleier wurden Wand und Farbstreifen gleich beeinflusst; der relative Vergleich blieb in allen 880 Bildern korrekt. Bei den räumlich ungleichen Reflexgegenfällen war die gemeinsame Beziehung potenziell verfälscht; die vorhandenen Schutzprüfungen blieben grün. Daraus folgt keine Bildkorrektur und keine pauschale globale Sperre.