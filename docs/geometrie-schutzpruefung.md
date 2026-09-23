# Korrektur der Zwei-Feld-Schutzlücke – aktueller Stand

23. September 2026 · Analyse 0.5.3 · Plan 1.32.

Im unveränderten IroGen-Fall „Verjuengung dunkel verrauscht waagerecht“ enthielt die erkannte Streifengruppe nur zwei Felder. Deren Querbreiten betrugen 136 und 114 Pixel; die inneren Konturänderungen betrugen ungefähr −9,94 % und −7,11 %. Die Schutzprüfung kehrte bei weniger als drei Feldern dennoch sofort zurück. Dies ist die unmittelbar nachgewiesene Ursache der Freigabe trotz vorhandener Verjüngungsbelege. Die Erkennung aller ursprünglichen Felder wird durch diese Korrektur nicht behauptet.

Gezielte Änderung: Auch zwei erkannte Felder werden geprüft, wobei beide Konturen gleichgerichtete Verjüngung belegen müssen. Breitenänderungs-, Kontur- und Anpassungsschwellen bleiben unverändert. Bei zwei Punkten ist eine lineare Anpassung allein kein Beleg; deshalb bleiben beide unabhängigen Konturbedingungen zwingend. Originalpixel-Farbpfad und Generatorplan sind unverändert.

Sechs unabhängige neue Kernfälle: zwei verjüngte Zwei-Feld-Streifen, zwei Rechteckkontrollen mit unterschiedlichen Breiten und zwei Fälle mit nur einer verjüngten Kontur. Vor Korrektur versagten beide Verjüngungssperren bei bestandenen Rechteckkontrollen. Danach bestehen alle 28 gezielten Geometriefälle. Im Ende-zu-Ende-Test entfällt die bisherige Ausnahme für kombinierte Störungen: Alle zehn Erwartungen werden nun zwingend geprüft.

[Unveränderter JSON-Plan](../iro-gen/testplans/perspektivkorrektur-konturpruefung.json): **10/10 Erwartungen erfüllt**, sechs Bilder mit jeweils drei freigegebenen Feldern, vier Bilder vollständig gesperrt mit Frontalhinweis. [Neuer Bericht](../tests/adjustments/konturplan-20260923-korrigiert/bericht.md) und [Soll-Ist-Prüfung](../tests/adjustments/konturplan-20260923-korrigiert/soll-ist.md). Frühere Berichte bleiben erhalten.

Abschließend nach der Codekorrektur: 96 Kern- und 123 Generator-/Integrationstests bestanden, keine übersprungen. Schemaprüfung mit zwei gültigen und fünf ungültigen Beispielen bestanden; 34 Exportpakete einzeln sowie gespeicherte Analyseläufe erfolgreich auf Vertragskonsistenz geprüft. Ein anfänglicher Sammelaufruf über alle Aufträge meldete mehrfach verwendete Aufnahme-IDs zwischen separaten Paketen; korrekt paketweise geprüft ohne Fehler. Die Vertragsprüfung alter Läufe ist keine erneute Analyse alter Bilder. Der abschließende Release-Build wird separat im Arbeitsplan dokumentiert.

Keine Geräte-/Emulatorprüfung oder Nutzerabnahme. Die allgemeine Perspektiverkennung, einschließlich gleichmäßiger Verkürzung ohne bekannte Feldform, bleibt offen.

## Historische Entwicklungsstände bis Analyse 0.5.2

Die folgenden Befunde beschreiben frühere Stufen. Ihre Mindestanzahl von drei Feldern ist durch die oben beschriebene Regel ersetzt.
# Geometrieschutz: Konturen statt bloßer Feldbreiten

Stand: 22. September 2026. Aktuell: Analyse 0.5.2, Plan 1.30.
Teilfortschritt; keine vollständige Perspektivprüfung oder Nutzerabnahme.

## Aktueller Nachweis und Korrektur

Vier neue Gegenbeispiele mit rechteckigen, unterschiedlich breiten Feldern wurden unter 0.5.1 fälschlich wegen Perspektive gesperrt. Die Erwartung „kein Perspektivhinweis ohne entsprechende Kontur“ wurde vor der Korrektur formuliert und in beiden Streifen- und Breitenrichtungen reproduziert. Zwei weitere schmale, rechteckige Streifen blieben bereits unter 0.5.1 messbar.

Die Sperre verlangt nun zusätzlich zum bisherigen Breitenverlauf eine gleichgerichtete Verjüngung innerhalb mindestens zweier und mindestens der Hälfte der erkannten Felder (bei ungerader Anzahl aufgerundet). Dazu werden die Querbreiten der segmentierten Komponenten in zwei inneren Bändern entlang der Feldachse verglichen: zwischen einem Sechstel und einem Drittel sowie zwischen zwei Dritteln und fünf Sechsteln der Feldlänge. Die mittlere Änderung muss mindestens zwei Pixel des Erkennungsrasters und mindestens 2,5 % der größeren mittleren Breite betragen. Diese zentral versionierten Zahlen sind Versuchswerte. Sie ersetzen keinen Nachweis eines physikalischen Kamerawinkels.

Nach der Korrektur bestehen alle 22 Geometrieprüfungen: zwölf bisherige Fälle, vier neue Gegenbeispiele mit unterschiedlichen rechteckigen Feldbreiten, zwei schmale Streifen und vier deutlich verjüngte Streifen mit gemeinsamer Abdunklung und deterministischen Störpixeln. Die letztgenannte Kombination ist eine synthetische Robustheitsprüfung, kein kalibriertes Sensorauschen oder Belichtungsmodell.

**Erkenntnis zur gleichmäßigen Verkürzung:** Dasselbe Pixelbild schmaler Rechtecke kann sowohl schmale frontal fotografierte Felder als auch gleichmäßig verkürzte breitere Felder darstellen. Ohne zusätzliche bekannte Geometrie lässt sich aus diesem Merkmal allein keine der beiden Ursachen bestimmen. Daher keine neue Seitenverhältnis- oder Winkelgrenze erfinden. Die starke-Perspektive-Anforderung bleibt ausdrücklich offen, ebenso ein allgemein sicherer Umgang mit solcher Mehrdeutigkeit. Die schmalen Kontrollbilder belegen nur das aktuelle Verhalten, keine Eignung beliebiger schräger Aufnahmen.

Die Konturergänzung behebt die reproduzierte Fehlablehnung; weitere unregelmäßige Umrisse, Drehungen mit Rastereffekten und kombinierte Störungen sind weiterhin zu prüfen. Bestehende Schutzprüfungen bleiben aktiv. Die Messfarben stammen unverändert aus dem gemeinsamen Originalbild.

Prüfstand: 90 Kerntests und 121 IroGen-/Integrationstests bestanden, keine übersprungen. Vollständiger Release-Build einschließlich Android bestanden: keine Fehler, acht bekannte XAML-Bindungswarnungen (XC0022). Keine Geräte-/Emulatorlaufzeitprüfung. Diese Entwicklungsfälle sind kein zurückgehaltener Abnahmesatz.

**Abnahme in Klartext, weiterhin offen:** „Unterschiedliche rechteckige Feldbreiten erzeugen in den geprüften Fällen keinen falschen Perspektivhinweis. Deutlich zusammenlaufende Konturen werden auch in den geprüften Kombinationen mit Abdunklung und Störpixeln gesperrt. Die allgemeine Erkennung stark schräger Aufnahmen ist noch nicht abgeschlossen.“

## Historischer Nachweis der vorigen Entwicklungsstufe

Die folgenden Angaben betreffen ausdrücklich Analyse 0.5.1 und Plan 1.29. Ihre Breitenprüfung wurde durch die oben beschriebene Konturbedingung ergänzt.
### Kleine Randreste und deutliche Verjüngung

Stand: 22. September 2026. Analyse 0.5.1, verbindlicher Plan 1.29.
Teilfortschritt der vorrangigen Sicherheitsarbeiten; keine vollständige Perspektivprüfung oder Nutzerabnahme.

## Vorab formulierte Erwartungen und Gegenprobe

Zwölf deterministische RGB-Testbilder in [GeometrySafetyTests.cs](../tests/iro.core.tests/GeometrySafetyTests.cs) werden unabhängig von IroGen aufgebaut. Die Analyse erhält ausschließlich Pixel und Standardoptionen, keine Sollgeometrie oder Generatorstufe. Die Erwartungen standen vor der Korrektur fest:

| Aufnahme | Fälle | Erwartung |
|---|---:|---|
| Angeschnittenes Feld mit nur 6 oder 16 Pixeln sichtbarer Länge, senkrecht/waagerecht | 4 | Gesamte Messung sperren, vollständige Aufnahme verlangen |
| Unabhängiger kleiner Fleck am Bildrand neben vollständigem Streifen | 2 | Beide vollständigen Felder messen |
| Drei trapezförmige Felder auf deutlich zusammenlaufendem Streifen, beide Richtungen und beide Verjüngungsrichtungen | 4 | Gesamte Messung sperren, frontalere Aufnahme verlangen |
| Vollständige Felder unterschiedlicher Länge mit geringer Breitenänderung, beide Richtungen | 2 | Alle vier Felder messen |

Vor der Korrektur in Analyse 0.5.0: acht Schutzfälle fehlgeschlagen, vier geeignete Gegenbeispiele bestanden. Nach der Korrektur: alle zwölf bestanden. Dies ist ein Entwicklungssatz, kein zurückgehaltener Abnahmesatz.

## Änderungen

- Randkomponenten bleiben vor der Prüfung der Mindestmessgröße als geometrische Hinweise erhalten. Auch ein selbst zu kleiner Feldrest kann dadurch die unvollständige Streifenaufnahme erkennen lassen. Bestehende Form- und Zuordnungskriterien bleiben aktiv.
- Ab mindestens drei erkannten Feldern prüft Iro die Querbreite entlang der Streifenachse. Ein linearer Trend mit Bestimmtheitsmaß mindestens 0,90 und geschätzter Breitenänderung über 15 % der maximalen Feldbreite sperrt die Aufnahme vor der Farbmessung.
- Der Nutzerhinweis lautet: „Streifen verjüngt sich deutlich. Kamera möglichst frontal auf Muster und Wand ausrichten.“
- Die Zahlen sind zentral versionierte technische Versuchswerte, keine physikalischen Winkelgrenzen. Unterschiedliche Feldlängen sind weiterhin zulässig. Der gemeinsame Originalpixel-Farbpfad wird nicht verändert.

## Prüfung

- Kern: 80 Tests bestanden, keine übersprungen.
- IroGen und Integration: 121 Tests bestanden, keine übersprungen; bestehende räumliche und Bildqualitätsregressionen eingeschlossen.
- Vollständiger Release-Build der Solution einschließlich Android bestanden: keine Fehler, acht bereits bekannte XAML-Bindungswarnungen (XC0022).
- Keine neue Geräte-/Emulatorlaufzeitprüfung; keine Aussage realer Farbgenauigkeit.

## Grenzen und nächster Schritt

Gleichmäßige perspektivische Verkürzung ohne Breitenverlauf wird damit nicht zuverlässig erkannt. Weniger als drei Felder reichen für diese neue Trendprüfung nicht aus. Verschiedene tatsächliche Feldbreiten können einen ähnlichen Trend erzeugen; eine allgemeine Unterscheidung von Streifenform und Perspektive ist damit nicht bewiesen. Kleine Randreste müssen weiterhin die bestehenden Komponenten- und Zuordnungskriterien erfüllen. Vollständigkeit ist daher nicht allgemein garantiert.

Als Nächstes die unabhängigen Eignungsfälle um gleichmäßige Verkürzung, wechselnde Feldbreiten, Drehung und kombinierte Störungen erweitern und die verbleibenden Grenzen anhand dieser Fälle bearbeiten. Die allgemeine Perspektivanforderung bleibt offen. Die Ausführung verifizierter Erwartungen in IroGen ist ebenfalls noch offen.

**Abnahme in Klartext, noch offen:** „Auch kleine sichtbare Reste eines angeschnittenen Streifens verhindern in den geprüften Fällen die Messung. Deutlich zusammenlaufende Streifen werden abgewiesen; die geprüften vollständigen Gegenbeispiele bleiben messbar. Die beschriebenen Grenzen sind bekannt.“
