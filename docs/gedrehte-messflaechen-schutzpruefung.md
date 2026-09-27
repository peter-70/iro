# Gedrehte Messflächen, Druck und konkurrierende Muster

Stand: 27. September 2026; Analyse 0.5.12, Planfassung 1.47. Teilbeitrag zum Arbeitsschritt „Geraderichten und innere Messflächen absichern“.

## Unabhängige Grenzfälle

Die neue Kern-Testreihe erzeugt 55 Rasterbilder mit analytisch bekannten Materialrechtecken. Die Bildpunkte werden über eine unabhängige inverse Drehung mit Materialfarbe, weißem Rand oder Druckfarbe belegt. Keine Iro-Erkennungsmasken oder Sollfarben gehen in den Analyzer ein. Acht Winkel: −80°, −45°, −27°, 0°, 13°, 37°, 44,5°, 80°. Unterschiedliche Achsenlagen sind damit enthalten.

- 32 geeignete Fälle: acht Winkel × zwei Feldgrößen (140×110 und 64×52 Pixel) × ohne/mit Randdruck. Weiße drei Pixel breite Ränder, 18 Pixel Feldabstand. Alle drei Felder müssen freigegeben werden.
- 16 Fälle mit einem schmalen dunklen Druckstrich innerhalb der Messfläche: acht Winkel × zwei Größen. Zulässig sind nur unverfälschte Messwerte oder Ablehnung. Die Fixture ist eine vereinfachte Druckstörung, keine umfassende Schriftartenprüfung.
- Vier zu kleine Muster (26×22 Pixel): keine freigegebenen Werte oder nächsten Treffer.
- Drei konkurrierende Muster: ein gedrehter Streifen und eine unabhängige waagerechte Reihe. Erwartet: Mehrdeutigkeit und keinerlei Messwerte.

Für jedes freigegebene Feld wird dessen inneres Messpolygon unabhängig in die bekannte ursprüngliche Materialgeometrie zurückgerechnet. Jede Ecke muss mindestens zwei Pixel innerhalb des wahren Materialrechtecks liegen. Ein echtes Feld darf nicht mehrfach freigegeben werden. Zusätzlich werden Feldfarbe, tatsächlich gemessene Wandfarbe und daraus berechneter Farbabstand mit den digitalen Materialwerten verglichen. Toleranz 1e-8 ΔE00 ist ausschließlich numerische Rundung in diesen unverrauschten Rasterfixtures, keine Kamera-Genauigkeitszusage. Gesperrte Felder dürfen weder einen Abstand noch einen nächsten Treffer ausgeben.

## Gefundene Schutzlücke

Vor Änderung bestanden 21 von 23 neuen Testfällen; zwei scheiterten: Bei −27° und 37° blendete die gedrehte Detektionsansicht die zweite waagerechte Musterreihe aus. Iro veröffentlichte den gewählten Streifen ohne Mehrdeutigkeitshinweis. Die Fälle zeigten eine unzulässige eindeutige Auswahl, nicht bereits verfälschte Werte des ausgewählten Streifens. Bei 0° erkannte der bestehende Detektor bereits beide Muster und lehnte ab.

Analyse 0.5.12 ergänzt bei tatsächlich gedrehten, zunächst eindeutigen Detektionen eine geometrische Gegenprüfung auf dem Originalbild. Bereits im Original erkannte Mehrdeutigkeit oder mindestens zwei dort erkannte Feldmittelpunkte außerhalb der gewählten gedrehten Felder verhindern die Freigabe. Beide Prüfungen verwenden dieselben unveränderten Originalpixel. Keine photometrische Korrektur oder zweite Messgrundlage eingeführt.

Die erste Umsetzung verdrängte bei einem bestehenden starken Unschärfefall den passenden Unschärfehinweis. Diese Regression wurde behoben: vorhandene aufnahmeweite Qualitätsgründe behalten Vorrang; die zusätzliche Mustersperre folgt vor Rangfolge und endgültiger Freigabe. Beide neuen Fälle verlangen jetzt ausdrücklich den verständlichen Hinweis „Mehrere mögliche Muster“.

## Nachweise und Grenzen

- 117 gezielte Kernprüfungen bestanden: neue Messflächenfälle, Originalpixelvertrag, Geometrie, bisherige Pixelanalyse, Beschnitt, Reflexe und Unschärfe. Anschließend alle 23 neuen Testfälle mit zusätzlich präzisiertem Ablehnungsgrund erneut bestanden; keine weiteren Produktänderungen danach.
- 23 Integrationsprüfungen bestanden: vorhandene Geraderichten-Fälle, PNG-Analyse, App-Anzeige und 108 Reflex-PNGs.
- Anschließender Zwei-Bilder-Gegencheck bestanden: sauberes und bekannt leicht gestörtes Bild einschließlich Feldgeometrie, Werten und Rangfolge. Keine übersprungenen Tests.

Die geeigneten Messflächen benötigten in diesen Fällen keine Änderung des Innenabstands oder der robusten Farbstatistik. Es wurde ausschließlich die nachgewiesene Mehrdeutigkeitslücke korrigiert. Weitere Schriftformen, Randbreiten, Bildauflösungen, Perspektive und reale Kamerabilder sind damit nicht umfassend geprüft. Die Gegenprüfung erkennt nur Muster, die der bestehende Detektor in mindestens einer der beiden Ansichten erfassen kann; beliebige konkurrierende Winkel bleiben eine offene Grenze. Eine zweite Detektion bei gedrehten Aufnahmen kostet zusätzliche Laufzeit, die am Gerät zu prüfen ist.

Der begrenzte Teil ist umgesetzt und technisch geprüft. Der gesamte Arbeitsschritt, Schluss-Sammellauf, Geräteprüfung und ausdrückliche Abnahme bleiben offen.

## Reproduktion

```powershell
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --filter FullyQualifiedName~RotatedInteriorSafetyTests
```

Die Fixtures liegen im Testcode; dies ist kein neuer ladbarer IroGen-JSON-Plan.

Android-Release-Build zur Messflächen- und Mehrdeutigkeitsabsicherung am 27. September 2026 bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Kein Gerätetest oder Schluss-Sammellauf.
