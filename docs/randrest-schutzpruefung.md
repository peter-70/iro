# Gestörte Randreste erkennen und Teilmessungen kennzeichnen

24. September 2026; Analyse 0.5.8; Planfassung 1.42. Gezielte Absicherung, keine allgemeine Beschnittfreigabe.

## Ursache und Korrektur

Der Farbverlauf eines angeschnittenen Farbfelds wurde bei der Regionssuche in mehrere schmale Farbkomponenten zerlegt. Jede einzelne war schmaler als eine geometrisch passende Streifenfortsetzung. Dadurch übersprang die bisherige Beschnittprüfung den realen Randrest und gab die zwei übrigen Felder mit Status Measured frei.

Vor Änderung 16 Fehlfreigaben reproduziert: Resthöhen 6/16/40/110 Pixel an allen vier Bildseiten, jeweils zwei statt null Freigaben. Vier Gegenbilder mit einem unabhängigen gleichartigen Randobjekt bestanden bereits.

Die Schutzprüfung berücksichtigt jetzt zusätzlich die gemeinsame Geometrie direkt benachbarter Randkomponenten. Zusammenführung ausschließlich quer zur Streifenrichtung, bei passenden Anfangs-/Endkoordinaten entlang des Streifens (höchstens zwei Pixel Differenz) und einem Querspalt beziehungsweise einer Rasterüberlappung von höchstens zwei Pixeln. Die bisherigen Kriterien für Breite, Abstand und Ausrichtung zur erkannten Feldreihe bleiben erhalten. Einzelkomponenten werden weiterhin ebenfalls geprüft. Keine Bildpixel oder Messfarben werden zusammengeführt; die Vereinigung der Rechtecke dient ausschließlich der Sicherheitsentscheidung. Die Rastertoleranz ist kein neuer Freigabe- oder Beschnittgrenzwert.

Nach Änderung alle 16 Fehlfälle vollständig gesperrt; unabhängige Randobjekte weiterhin messbar. Hinweis:

> Angeschnittener Streifen: sichere Auswertung nicht gewährleistet. Muster vollständig ins Bild nehmen.

Auch die 16 Verlaufsfälle in der bisherigen 64-Bilder-Untersuchung werden nun ausdrücklich auf ausbleibende Freigabe, ΔE00-Werte und nächste Treffer geprüft. [Vorher](../tests/adjustments/beschnitt-20260924/vorher-0.5.7.md), [aktueller Bericht](../tests/adjustments/beschnitt-20260924/bericht.md).

## Kennzeichnung zulässiger Teilmessungen

Bereits zulässige Teilmessungen, beispielsweise bei einer isoliert ungeeigneten erkannten Fläche ohne aufnahmeweite Sperre, nennen nun die Anzahl auswertbarer und erkannter Felder. Beispiel:

> 2 von 3 erkannten Feldern auswertbar. Vergleich und ähnlichster Treffer gelten nur für die auswertbaren sichtbaren Felder; kein vollständiger Streifenvergleich.

Die Anzahl erkannter Felder wird nicht als wahre Gesamtzahl des ursprünglichen Streifens ausgegeben. Der bestehende PartiallyMeasured-Status und die Anzeigeüberschrift Teilweise auswertbar bleiben bestehen. Ungeeignete Felder haben keinen ΔE00-Wert und keinen nächsten Treffer. Der Test verwendet ein tatsächlich analysiertes verunreinigtes Feld und prüft den Wechsel von vollständig zu teilweise und zurück einschließlich angezeigter Werte und Hinweise. Der Resultathinweis steht auch für den vorhandenen Berichtsweg bereit; kein neues Schema oder künstlicher Messwert erforderlich.

## Prüfungen

- 108 gezielte Kernprüfungen bestanden: Randreste, Beschnittuntersuchung, Geometrie, Pixelanalyse, Messregeln, Beleuchtung und Unschärfe.
- 19 gezielte Integrationsprüfungen bestanden: PNG-Analyse, Android-Anzeigeanbindung, Geraderichten sowie Beleuchtungs- und Unschärfepläne. Zwei neue unabhängige PNG-Gegenproben mit senkrechtem/waagerechtem Randrest sperren über den produktiven Adapter und zeigen keine Werte.
- PNG-Beispiele und kurze Befunde: [senkrecht](../tests/adjustments/randrest-20260924/senkrecht.md), [waagerecht](../tests/adjustments/randrest-20260924/waagerecht.md). Die entsprechenden PNG-Dateien liegen daneben.
- Abschließender Zwei-Bilder-Gegencheck bestanden: sauberes Bild und bewährte leichte Rauschstörung, einschließlich Freigabe, Feldzuordnung, Messwerten und Rangfolge.

Keine übersprungenen Tests. Kein Schluss-Sammellauf und keine Geräteausführung.

## Grenzen und nächster Schritt

Die Nachweise betreffen die reproduzierten geometrisch anschließenden Verlaufsfragmente, die geprüften Größen und Richtungen. Beliebige schräg beschnittene, stark verrauschte, verdeckte oder vollständig unsichtbare Felder sind damit nicht zuverlässig erkennbar. Ein unsichtbares besser passendes Feld bleibt prinzipiell möglich. Die bedingte Freigabe geeigneter Beschnitte ist noch nicht implementiert und bleibt gemäß Fassung 1.41 zu untersuchen; bestehende konservative Sperren sind nicht gelockert. Als Nächstes den nachweisbaren Freigabebereich mit tatsächlich erkannter Geometrie statt vorgegebener Fixture-Maske entwickeln und gegen unabhängige Fälle prüfen.

Umsetzung und gezielte Prüfung der hier reproduzierten Lücke belegt; übergeordneter Planpunkt, Schlussprüfungen und ausdrückliche Abnahme bleiben offen.

Android-Release-Build nach der Korrektur bestanden: 0 Fehler, 8 bekannte XC0022-Warnungen. Keine Geräteausführung.

## Fortschreibung mit Analyse 0.5.9

Die zuvor offene begrenzte Endbeschnittfreigabe ist nun innerhalb eines synthetisch geprüften Bereichs implementiert; [Regeln und Nachweise](endbeschnitt-freigabe.md). Ungeeignete und zusätzliche schwache Randreste bleiben Sperrindizien. Allgemeine Beschnittfreigabe und Abnahme bleiben offen.
