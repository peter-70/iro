# Gleichmäßige Reflexüberlagerung: Grenze der Einzelbildanalyse

Stand: 27. September 2026. Planfassung 1.53. Analyse 0.5.15. Dieser Befund untersucht die zwölf bekannten Fehlfreigaben bei einem gleichmäßigen hellen Schleier. Er ändert den produktiven Analyzer nicht und erklärt die Fehlfreigaben nicht für zulässig.

## Fragestellung und Ergebnis

Zu klären war, ob Iro einen gleichmäßigen Reflexschleier allein aus den Pixeln eines einzelnen Bildes zuverlässig von echten hellen oder homogenen Wand- und Feldfarben unterscheiden kann.

Für die untersuchte Klasse ist das unmöglich. Der Beweis ist konstruktiv: Zu jedem der zwölf Schleierbilder wurde eine zweite, saubere Szene erzeugt, deren Wand- und Feldmaterialien bereits genau die nach der Überlagerung sichtbaren Farben besitzen. Diese saubere Szene zeichnet keinen Reflex. Beide Erzeugungswege liefern bei senkrechtem und waagerechtem Streifen, bei Reflex auf Feld, Wand oder beiden sowie ohne und mit dem festgelegten Rauschen dieselben Breiten, Höhen und **bytegenau dieselben RGB-Pixel**.

Ein Einzelbild-Analyzer erhält in beiden Fällen somit exakt dieselbe Eingabe. Er kann das Reflexbild nicht ablehnen und das saubere Bild zugleich freigeben. Eine Helligkeits-, Sättigungs- oder Farbabstandsschwelle würde nur eine willkürliche Teilmenge echter Materialien aussortieren. Generatoroptionen oder ursprüngliche Materialfarben standen dem Analyzer nicht zur Verfügung.

Die zwölf Bilder bleiben fachlich unbrauchbare Aufnahmen und bekannte Fehlfreigaben. Der Nachweis bedeutet ausschließlich: Die geforderte automatische Unterscheidung kann für eine räumlich vollständig gleichmäßige, spurlos in plausible Materialfarben übergehende Überlagerung nicht aus diesem einen Bild gewonnen werden. Es wurde deshalb keine Scheinsperre und keine Farbkorrektur eingebaut.

## Zulässige Strategien, über die der Nutzer entscheiden muss

1. **Aufnahmevorgabe mit physischer Reflexvermeidung.** Blitz ausschalten, Kamera/Licht nicht spiegelnd ausrichten und bei glänzenden Flächen einen Polarisationsfilter beziehungsweise kreuzpolarisierte Beleuchtung verwenden. Vorteil: Das Problem wird vor der Analyse vermindert und das Einzelbildprinzip bleibt erhalten. Nachteil: Bedien- oder Hardwareaufwand; die App kann bei einem vollkommen gleichmäßigen Restschleier weiterhin nicht beweisen, dass die Vorgabe eingehalten wurde.
2. **Bekannte physische Qualitätsreferenz im Bild.** Ein zusätzlicher, fest definierter Referenzbereich mit bekanntem Reflexionsverhalten könnte als Aufnahmeindikator dienen. Vorteil: Zusätzliche Information im selben Bild, ohne Farben zu rekonstruieren. Nachteil: Streifen und Aufnahmevertrag ändern sich; Kamera, Beleuchtung und Grenzwerte müssen am Gerät kalibriert werden. Auch dieser Ansatz benötigt reale Gegenbeispiele und darf echte Materialien nicht pauschal sperren.
3. **Zweite Aufnahme mit gezielt verändertem Winkel oder Licht.** Ein spiegelnder Anteil verändert sich typischerweise zwischen zwei kontrollierten Aufnahmen. Vorteil: Die bisher fehlende Zusatzinformation kann sichtbar werden. Nachteil: Dies widerspricht der aktuellen verbindlichen Regel „ein Bild, eine Analyse, eine Auswertung“, erfordert Ausrichtung und Vergleich zweier Bilder und wäre eine ausdrückliche Produktänderung. Diffuse oder unverändert bleibende Störungen sind dadurch nicht automatisch gelöst.
4. **Manuelle Wiederholung als Bedienentscheidung.** Die App kann vor jeder Messung allgemein zur reflexarmen Aufnahme auffordern oder dem Nutzer eine Wiederholung anbieten. Vorteil: Keine unbelegte automatische Diagnose. Nachteil: Der konkrete gleichmäßige Schleier wird weiterhin nicht automatisch erkannt; eine unbemerkte Fehlfreigabe bleibt möglich.

Keine dieser Strategien wurde als Produktverhalten festgelegt. Die Auswahl ist eine Produktentscheidung des Nutzers. Bis dahin bleibt der dokumentierte technische Grenzfall offen.

## Reproduzierbare Prüfungen

Werkzeuge: .NET SDK 10.0.401, PowerShell 7.6.5, xUnit.net VSTest Adapter 3.1.4.

Erster Lauf nach Ergänzung des Beweistests:

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionEvidenceTests --logger "console;verbosity=normal"
```

Schlusszeile: `Gesamtzahl Tests: 23` mit 11 bestanden und 12 fehlgeschlagen. Die Pixelgleichheit und die Messbarkeit waren bereits erfüllt; fehlgeschlagen war ausschließlich der ungeeignete direkte Vergleich zweier Ergebnisobjekte mit enthaltenen Listen. Die Assertion wurde anschließend auf den inhaltlichen Vergleich von Status, Hinweis, Feldgrenzen, Freigabe, Messwert und Kennzeichnung des nächsten Treffers korrigiert.

Wiederholung desselben gezielten Satzes mit minimaler Konsolenausgabe:

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionEvidenceTests --logger "console;verbosity=minimal"
```

Tatsächliche Schlusszeile: `Bestanden! : Fehler: 0, erfolgreich: 23, übersprungen: 0, gesamt: 23, Dauer: 13 s`.

Bestehender PNG-Produktpfad für alle 108 Reflexbilder:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~ReflectionPngTests --logger "console;verbosity=minimal"
```

Tatsächliche Schlusszeile: `Bestanden! : Fehler: 0, erfolgreich: 4, übersprungen: 0, gesamt: 4, Dauer: 15 s`.

Verpflichtender Gegencheck mit sauberem Bild und bekannter leichter Rauschstörung:

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~TwoImageCheckpointTests --logger "console;verbosity=minimal"
```

Tatsächliche Schlusszeile: `Bestanden! : Fehler: 0, erfolgreich: 1, übersprungen: 0, gesamt: 1, Dauer: 881 ms`. Der erzeugte Bericht liegt unter [iro-run-ef0ed1017024411880f193b013c54cf6](../tests/adjustments/zwischenkontrolle-20260923/iro-run-ef0ed1017024411880f193b013c54cf6/bericht.md).

Bewusst nicht ausgeführt: vollständige Kern- und Generator-Testprojekte, Restore, Solution-Build, Android-Build, Abschluss-Sammellauf und Gerätetests. Der Produktcode wurde nicht geändert; die vollständigen Läufe bleiben für den vereinbarten Abschluss vorgesehen.
