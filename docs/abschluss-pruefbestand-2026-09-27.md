# Unabhängiger Prüfbestand vor dem Abschluss-Sammellauf

27. September 2026 · Planfassung 1.52. Diese Zusammenstellung bereitet den späteren technischen Sammel-Testlauf vor. Sie führt ihn ausdrücklich noch nicht aus und ist keine Geräte- oder Nutzerabnahme.

## Ergebnis der Inventur

Alle neun vorhandenen IroGen-JSON-Testpläne sind in [abschluss-pruefbestand.json](../tests/abschluss-pruefbestand.json) genau einmal erfasst:

| Testplan | Bilder | Bewertungsstand | Aussage und Grenze |
|---|---:|---|---|
| Beleuchtungsschutz | 8 | 8 mit geprüfter Erwartung | Konstruiert starke Übergänge und positive Kontrollen; keine allgemeine Lichtursachenerkennung. |
| Bildqualität und Abstand | 113 | vollständig diagnostisch | Historischer Plan ohne vorab geprüfte Erwartungen; keine 113 bestandenen Abnahmefälle. |
| Feldzuordnung und Messwerte | 3 | 3 mit geprüfter Erwartung | Synthetische Feld-/Wertregression, keine Gerätegenauigkeit. |
| Perspektivkorrektur und Konturprüfung | 10 | 10 mit geprüfter Erwartung | Feldbreiten und deutliche Verjüngung; keine allgemeine reale Perspektivfreigabe. |
| Rangfolge und Gleichstände | 5 | 5 mit geprüfter Erwartung | Einzelbild-Rangprüfung; Prüftoleranz ist keine Produktschwelle. |
| Raumlage, Perspektive und Wandabstand | 108 | vollständig diagnostisch | Historischer Plan ohne vorab geprüfte Erwartungen; keine reale Winkel-/Abstandsfreigabe. |
| Strenge Unschärfeablehnung | 12 | 12 mit geprüfter Erwartung | Belegte scharfe Kontrollen und vollständige Sperren; ein Fall bleibt beim früheren Geometriegrund. |
| Unterbelichtung und dunkle Originalfarben | 16 | 4 geprüft, 12 bewusst unbewertet | Dunkle Materialien als positive Gegenfälle; Belichtungsgrenze weiterhin offen. |
| Zwei-Bilder-Zwischenkontrolle | 2 | 2 mit geprüfter Erwartung | Begrenzter Regressionstest zwischen Arbeitspunkten, kein Sammellauf. |

Gesamt: **277 Bilder**. Davon **44 mit geprüfter Verhaltenserwartung** und **233 diagnostisch oder bewusst unbewertet**. Diese Mengen bleiben getrennt. Ein verarbeiteter Diagnosefall ist kein bestandener Fall.

## Ausführbarer Einstieg

Die Datei [Invoke-AbschlussPruefbestand.ps1](../tests/Invoke-AbschlussPruefbestand.ps1) benötigt PowerShell 7 oder neuer. Ohne Schalter validiert sie ausschließlich Schema und Summen des Inventars, die vollständige Erfassung aller Pläne, Fall-/Bild-/Erwartungszahlen, alle Pläne der Version 2, die sichtbare Nennung der zwölf Reflex-Fehlfreigaben und die fest codierte Befehlsliste.

Sichere Vorschau:

```powershell
& 'C:\Users\Peter Breitkopf\.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe' -NoProfile -File tests/Invoke-AbschlussPruefbestand.ps1
```

Der spätere, erst noch zu autorisierende Sammellauf verwendet denselben Aufruf mit `-Execute`. Das Skript führt dann nur die sechs intern fest zugeordneten Schritte aus: Restore im Locked-Modus, Solution-Release-Build, vollständige Kern- und Generator-Testprojekte, Schema-Prüfung und Android-Release-Build. Befehle aus JSON werden nicht dynamisch als Shellcode ausgeführt.

Die beiden historischen Diagnosepläne bleiben auch dann Diagnose: Ein grüner technischer Test belegt ihre Reproduzierbarkeit und Verarbeitung, nicht das fachliche Bestehen sämtlicher Bilder.

## Sichtbare Restlücken

1. **Gleichmäßige Reflexüberlagerung:** Zwölf synthetische Aufnahmen werden weiterhin freigegeben, obwohl die beschlossene Produktregel vollständige Ablehnung verlangt.
2. **Unterbelichtung ohne Kanalanschlag:** Für zwölf Fälle fehlt eine unabhängig belegte Grenze zwischen auswertbar und unbrauchbar. Dunkle echte Materialien dürfen nicht pauschal gesperrt werden.
3. **Jeweils homogene, aber unterschiedliche Beleuchtung:** Starke räumliche Übergänge werden erkannt. Eine allgemeine zuverlässige Unterscheidung homogener Materialfarbe von unterschiedlich beleuchteter Wand und Streifen ist nicht belegt.
4. **Allgemeine Perspektive und Wandabstand:** Leichte synthetische Fälle und deutliche Verjüngung sind gezielt geprüft. Reale kombinierte Winkel-, Abstand- und Freihandgrenzen fehlen.
5. **Allgemeiner Beschnitt:** Nur der dokumentierte begrenzte Endbeschnitt ist synthetisch freigegeben. Weitere Grenzen bleiben für die Gerätephase offen.
6. **Reale Kamera und Farbgenauigkeit:** Sensor, ISP, Beleuchtung, Fokus, Belichtung und reale Materialien können erst nach grünem technischem Sammellauf am Gerät geprüft werden.
7. **Produktgrenze „ähnlich nah“:** Die Anzeigegrenze bleibt eine offene Produktentscheidung. Die Testtoleranz der Rangprüfung darf nicht stillschweigend dafür verwendet werden.

Die maschinenlesbare Lückenliste steht im Inventar und muss in späteren Berichten erhalten bleiben. Erfolgreiche Kontrollen dürfen diese Punkte weder entfernen noch in eine bestandene Quote einrechnen.

## Tatsächlich ausgeführte Prüfung

Ausgeführt wurde nur die sichere Vorschau, nicht `-Execute`:

```text
PowerShell 7.6.5
Prüfbestand gültig: 9 Pläne, 277 Bilder; 44 mit geprüfter Erwartung, 233 diagnostisch oder unbewertet.
Bekannte Lücken: 7.
Vorschau abgeschlossen. Kein Build und kein Testlauf gestartet. Für den später autorisierten Sammellauf -Execute angeben.
```

Zwei ungültige Gegenbeispiele (falscher Summentyp und unbekannter Bewertungsstatus) wurden vom Inventarschema korrekt abgewiesen.

Anschließend ausgeführter Pflicht-Gegencheck:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --verbosity minimal
```

```text
Bestanden!   : Fehler:     0, erfolgreich:     1, übersprungen:     0, gesamt:     1, Dauer: 857 ms - IroGen.Tests.dll (net10.0)
```

[Bericht des neu erzeugten sauberen und leicht verrauschten Kontrollpaars](../tests/adjustments/zwischenkontrolle-20260923/iro-run-31035ec8b94d4756ae22dc50c034da15/bericht.md). Freigabe, Feldzuordnung, Werte und Rangfolge wurden geprüft.

`git diff --check 2>$null` endete mit Exitcode 0 ohne Ausgabe. Vollständige Kern-/Generatorprojekte, Solution-/Android-Build, Emulator, Kamera und Gerät wurden in diesem Vorbereitungsschritt bewusst nicht ausgeführt.

## Nächster offener Punkt

Vor einem Sammellauf ist der konkrete Schutzverstoß bei den zwölf gleichmäßig reflexüberlagerten Fehlfreigaben zu bearbeiten. Dabei dürfen echte helle oder homogene Materialfarben nicht pauschal abgelehnt und keine Generator-Sollwerte in den Analyzer gegeben werden. Zunächst ist belastbar zu klären, ob die Fälle allein aus dem Einzelbild unterscheidbar sind. Falls nicht, muss die Grenze als technisch nicht erkennbar belegt und eine zulässige Aufnahme-/Produktstrategie entschieden werden; eine Scheinsperre ist keine Lösung.
