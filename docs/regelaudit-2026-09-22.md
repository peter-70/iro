# Entwicklungs- und Regelaudit: Iro vor weiterer Bildoptimierung

Stand: 22. September 2026. Geprüfter Arbeitsstand: Iro-Analyse vor Korrektur 0.4.0, nach Korrektur 0.5.0; Generator-Bildmodell 1.3.0; verbindlicher Plan nach diesem Audit 1.28. Das Repository enthielt schon vor dieser Prüfung Änderungen aus den vorherigen Arbeiten. Kein Commit oder Zurücksetzen vorgenommen.

## Ergebnis und Auslieferungsstand

**Iro ist noch keine auslieferungsreife Kunden-App.** Es existiert ein gemeinsamer, funktionierender Einzelbildkern und eine Android-Testoberfläche. Der Windows-Testweg verwendet denselben Kern. Das ist eine gute Grundlage, aber keine vollständige Umsetzung des Messprotokolls und keine reale Farbvalidierung.

Die zentrale neue Regel wird im untersuchten Farbpfad eingehalten: Wand und Farbstreifen werden aus demselben Frame mit derselben sRGB-Konvertierung gemessen. Keine getrennte Aufhellung, Neutralisierung, Kontrastkorrektur, Farbkalibrierung oder KI-Rekonstruktion gefunden. Belegte Freigabeverstöße wurden behoben. Weitere notwendige Qualitätsprüfungen bleiben unvollständig; neue Erkennungsoptimierungen werden deshalb zurückgestellt.

## Prüfverfahren und Grenzen

Geprüft wurden Quellcode und Datenfluss von `src/iro.app`, `src/iro.core`, `src/iro.analysis` sowie Generator, Testübergabe, Auswertung und Export in `iro-gen`. Das angenommene Entscheidungsprotokoll und der Fachplan bilden den Maßstab. Existierende umfangreiche Ergebnisdateien wurden nicht für die Analyse eingelesen. Die Regressionstests erzeugen ihre eigenen isolierten Eingaben und Läufe; ausgewertet wurden deren kompakte Berichte.

Codebefund, reproduzierbarer Verhaltensfehler und noch fehlende Funktion sind getrennt zu lesen. Es wurden keine Gerätetests, keine neue Emulator-Bedienabnahme, keine unabhängige physische Farbreferenzprüfung und keine statistisch vollständige Fehlergrenzenbestimmung durchgeführt. Ein bestandener Test belegt nur seinen angegebenen Umfang. Die bestehenden 108-/113-Bilder-Pläne sind Entwicklungsdaten, kein unangetasteter Abnahmesatz.

## Tatsächlicher Stand der Kunden-App

| Bereich | Tatsächlich vorhanden | Noch nicht nachgewiesen oder vorhanden |
|---|---|---|
| Android-Oberfläche | Zwei gebündelte PNG-Testbilder; echte Hintergrundanalyse; Messwertliste und Hinweise; Abbruch/Verwerfen älterer Bildaufträge; Berechtigungsdialog | Native Live-Vorschau, Overlay auf Feldpolygonen, bedienbare Wandreferenz, vollständige Einstellungsseite und Nutzerablauf |
| Kamera | Manifest und Berechtigungsbedienung | Camera2-Sitzung, YUV-Konverter, Aufnahme-/CaptureResult-Paarung, tatsächliche AE/AWB/Fokussteuerung und gerätebezogene Prüfung |
| Analyse | Gemeinsamer Core, CIEDE2000, Streifensuche, Geraderichten, robuste Innenflächen, erste räumliche Qualitätsprüfung | Vollständige Freigabeprüfung aller Protokollbedingungen; belastbarer Umgang mit Perspektive, Unterbelichtung und vergleichbarer Beleuchtung |
| Zeitlicher Ablauf | Schutz vor verspäteten Ergebnissen einzelner Testbildaufträge | Feldtracking, gekoppelte Frame-Aggregation, Alterungsgrenzen echter Live-Werte, tatsächliche Parameterwechsel und Ruheprüfung |
| Kundenaussage | Testmodus sichtbar gekennzeichnet | Keine reale Farbgenauigkeits- oder Produktfreigabe; App-ID weiterhin Entwicklungskennung |

Fundstellen: [Android-Hauptseite](../src/iro.app/MainPage.xaml), [Hauptseitenlogik](../src/iro.app/MainPage.xaml.cs), [gebündelter Bildlader](../src/iro.app/TestImageLoader.cs), [Anzeigezustand](../src/iro.core/Analysis/AnalysisPresentation.cs), [App-Projekt](../src/iro.app/Iro.App.csproj).

Der Android-Bildlader ist auf bekannte gebündelte Test-PNGs ausgelegt. Er besitzt nicht den vollständigen Farbprofil-/PNG-Prüfvertrag des Windows-Adapters. Das ist im aktuellen eingeschränkten Testmodus keine heimliche Farbkorrektur, aber kein fertiger allgemeiner Kamera- oder Importpfad.

## Nachweis der gemeinsamen Messgrundlage

1. `ImageAnalyzer` behält das Original als `source`. Geraderichten erzeugt eine virtuelle geometrische Ansicht mit Rückbezug auf dieses Original.
2. `StripDetector` verwendet eine verkleinerte, gegebenenfalls lokal gemittelte Suchansicht. Diese gemittelten Pixel gelangen nicht als Messfarben an `RegionSampler`.
3. `RgbFrame.Sample` geht bei Drehung über die zugehörigen Originalpixel innerhalb der rückabgebildeten Innenfläche. Keine interpolierten Farben werden gemessen.
4. `RegionSampler` behandelt Wand und Farbfelder mit denselben Optionen: robuste Pixelauswahl, Mittelwert im linearen RGB, dann XYZ/Lab.
5. `FindReference` wählt geometrisch, nicht nach hellerem oder dunklerem Farbwert. Die alternative feldweise Referenz bleibt ein expliziter Versuchsmodus; andere Messregionen im selben Bild sind keine unabhängige photometrische Korrektur. Das endgültige Referenzmodell bleibt offen.
6. `AnalysisRunner` übergibt nur PNG-Pixel und Analyseoptionen an die Analyse. Nominale Generatorfarben und Sollgeometrien werden erst danach zum Diagnosevergleich herangezogen.

Fundstellen: [Bild-/Pixelvertrag](../src/iro.core/Analysis/AnalysisContracts.cs), [Analyzer](../src/iro.core/Analysis/ImageAnalyzer.cs), [Messung](../src/iro.core/Analysis/RegionSampler.cs), [Detektor](../src/iro.core/Analysis/StripDetector.cs), [Farbrechnung](../src/iro.core/Analysis/ColorMath.cs), [Windows-Läufer](../src/iro.analysis/AnalysisRunner.cs).

## Belegte Verstöße und vorgenommene Korrekturen

| Befund vor Korrektur | Auswirkung | Korrektur und Nachweis |
|---|---|---|
| Kanalgrenzwerte wurden nur gezählt, ohne die Freigabe zu beeinflussen | Ein Feld oder die Wand mit einem großflächig auf 255 stehenden Kanal erhielt weiterhin ΔE00-Werte | Relevante Endpunktbelegung der nach robuster Auswahl verbleibenden Messpixel sperrt die gesamte Aufnahme. Feld- und Wandfall unabhängig reproduziert. |
| Bereits als unbrauchbar unscharf erkanntes Feld sperrte nur sich selbst | Andere Felder desselben Bilds blieben freigegeben, entgegen der neuen aufnahmeweiten Regel | Erkannte unbrauchbare Unschärfe wird vor Wertausgabe und Rangmarkierung aufnahmeweit gesperrt. Ein teilweise bewegungsverschmiertes, unabhängig erzeugtes Pixelbild reproduziert den Fehler. |
| Angeschnittene Felder wurden als Randregion ausgesondert, der Rest als vollständiger Fund freigegeben | Ein sichtbar am Bildrand fortgesetzter Streifen erhielt Messwerte | Geometrisch zur Feldfolge passende Randregionen lösen eine Ausschnittsperre aus. Horizontale und vertikale Gegenproben mit vollständig sichtbarem Kontrollstreifen bestehen. |
| IroGen nannte nominale Abweichungen „falscher Messwert“ und kleine Abweichungen „Ziel erreicht“ beziehungsweise „genau“ | Ungeprüfte Material-Sollwerte vor Störungen wurden sprachlich zu einer fachlichen Messbewertung aufgewertet | Dialog, Zusammenfassung und Markdown-Export nennen nominale Auffälligkeit und deren Grenzen. Kein Genauigkeits- oder Abnahmeurteil. |
| Auswertung zählte ausschließlich nominal zugeordnete Messungen | Ohne nominale Zuordnung konnte eine vorhandene Messung als abgewiesen erscheinen | Eigene Kategorie „Gemessen ohne nominalen Vergleich“; tatsächliche Freigabe und verfügbare Vergleiche getrennt. |
| App zeigte zwei Nachkommastellen | Abweichung vom vereinbarten Startansatz der normalen Messanzeige | Eine Nachkommastelle in der App; interne Rechnung und Entwicklerdiagnose bleiben ungerundet beziehungsweise detaillierter. |

Neue Kernprüfungen: [MeasurementRuleTests](../tests/iro.core.tests/MeasurementRuleTests.cs). Neue Auswertungsprüfungen: [ReviewDiagnosticsTests](../tests/iro.gen.tests/ReviewDiagnosticsTests.cs). Korrektur der Freigaben: [MeasurementSafety](../src/iro.core/Analysis/MeasurementSafety.cs), [ImageAnalyzer](../src/iro.core/Analysis/ImageAnalyzer.cs).

**Vorher-Nachweis:** Die ersten acht unabhängigen Prüfungen gegen Analyse 0.4.0 ergaben fünf Fehler: zweimal Beschnitt, zweimal Kanalanschlag und einmal Unschärfe-Teilfreigabe. Drei Gegenproben für helle, dunkle und gesättigte Farben mit auflösbaren Kanälen bestanden bereits. Nach Korrektur bestehen diese sowie drei ergänzende Prüfungen für Endpunkte/kleine ausgeschlossene Druckpixel und Sperre/Wiederfreigabe in der Anzeige.

### Grenzen der neuen Schutzmaßnahmen

Die Kanalgrenzenregel ist eine konservative 8-Bit-Versuchsregel: Mehr als 2 % der **behaltenen** Messpixel mit mindestens einem Kanal exakt 0 oder 255 sperren die Aufnahme. Der Wert ist zentral in `MeasurementSafety` abgelegt und mit Analyse 0.5.0 versioniert, aber nicht als Gerätegrenze abgenommen. Kleine durch robuste Statistik ausgeschlossene Druck- oder Glanzpixel lösen nicht automatisch diese Sperre aus. Werte 1/254 allein zählen nicht als Endpunkt. Auch echtes reines Weiß oder eine exakte Primärfarbe am Codewertanschlag kann gesperrt werden: Ohne weitere Aufnahmeinformation ist ausreichende Kanalreserve nicht belegbar. Die Meldung behauptet deshalb einen Kanal an der Messgrenze, keine sicher erkannte Ursache „zu hell“ oder „zu dunkel“.

Der frühere Weiß-Gegenfall verlangte die Freigabe eines ganzen Feldes mit RGB 255/255/255. Er wurde auf helles RGB 250/250/250 mit auflösbaren Kanälen umgestellt; exakte Endpunkte besitzen jetzt ausdrückliche Sperrtests. Die mathematischen sRGB-/Lab-Referenztests für Weiß, Schwarz und Primärfarben sind unverändert. Die bisherige Messgröße `NearLimitFraction` bleibt eine Diagnose und wurde nicht stillschweigend umdefiniert.

Die Unschärfekorrektur verbreitert die Sperrwirkung eines bereits vorhandenen Befunds. Sie beweist nicht, dass die bisherige kurze Kantenprüfung jede Unschärfe erkennt oder nie einen Fehlalarm erzeugt. Die Beschnittkorrektur erkennt passende sichtbare Randfortsetzungen; sie beweist keine Vollständigkeit bei völlig außerhalb liegenden, verschmolzenen, sehr kleinen oder nicht erkannten Feldern.

## Noch offene Regelabdeckung — vor neuen Optimierungen bearbeiten

| Vorrangiger Befund | Konkreter Nachweis / Bedeutung | Erforderliche nächste Arbeit |
|---|---|---|
| Starke Perspektive hat keine eigenständige zuverlässige Sperre | Der Analyzer gruppiert Rechtecke und misst deren Innenflächen. Im neuen 108-Bilder-Lauf werden „Seitenblick sehr stark waagerecht“ mit 21/21 und „oben/unten sehr stark senkrecht“ mit 19/21 Feldern weiter gemessen. Generatorwinkel sind keine automatisch beschlossenen App-Grenzen. | Geometrische Eignungskriterien und unabhängig geprüfte Bildfälle festlegen; starke Verkürzung sicher sperren. Keine pauschale Gleichsetzung von Generatorstufe und Kamerawinkel. |
| Unterbelichtung ohne Codewertanschlag wird nicht gezielt erkannt | Keine modellierte Signal-/Tonwertgüteprüfung. Drei frontale Bilder bei −2 EV liefern weiter 20/21 Werte, nominal bis 4,74 ΔE00 abweichend. Das ist ein Untersuchungsbefund, kein alleiniger Beweis falscher Farbwerte. | Dunkle reale Farbe von unbrauchbarer Aufnahmeinformation unterscheiden; dafür geeignete Positiv-/Negativfälle und messbare Kriterien aufbauen. Keine blinde Helligkeitsschwelle über sämtliche Farben legen. |
| Vergleichbare Beleuchtung beider Messpartner ist nicht belegt | Räumliche Variation innerhalb jeder Fläche wird geprüft; zwei jeweils homogene Flächen können dennoch unterschiedlich beleuchtet sein. | Vergleichbare Beleuchtungsbedingungen untersuchen und nachweisbare Sperr-/Warnbedingungen entwickeln. Die Produktwahl „Warnung oder Ablehnung“ bleibt gemäß Protokoll vor der betroffenen Umsetzung zu entscheiden. |
| Vollständigkeit und Unschärfeerkennung bleiben lückenhaft | Einzelne homogene Rechtecke gelten als möglicher Streifen; fehlende Felder sind dem Analyzer unbekannt. Beim kombinierten Fall „Bücken stark“ werden noch zwei Felder freigegeben. | Erkennungs-/Geometrie- und Qualitätsbefund gemeinsam absichern; die verbleibenden Fälle anhand des tatsächlichen Bilds bewerten, ohne Sollfeldzahl aus dem Generator einzuspeisen. |
| Verifizierte Abnahmeerwartungen werden nicht vollständig ausgeführt | `SceneExport` erzeugt zu Recht `proposed` und unbekannte Freigaben. `AnalysisRunner.CompareAfterAnalysis` verarbeitet derzeit nominale Generatorabstände, keinen vollständigen geprüften Freigabe-/Hinweis-Sollvergleich. | Verifizierte Erwartungen für Freigabe, Ablehnung, Hinweise und gegebenenfalls Bildfarbwerte aufbauen und auswerten. Bis dahin keine „fachlich bestanden“-Behauptung aus dem Auswertungsdialog. |

**Verbindlicher Vorrang:** Diese offenen Sicherheits- und Nachweislücken bleiben vor Erkennungskopie-Aufhellung, Schärfung, zusätzlichen Korrekturmodellen oder anderen neuen Optimierungen auf der Arbeitsliste. Die Durchführung dieses Audits bedeutet nicht, dass diese Punkte erledigt oder die App abgenommen sind.

Eine unbekannte reale Aufnahme lässt nicht sämtliche Ursachen eindeutig bestimmen. Das erlaubt weder erfundene Diagnosen noch stillschweigende Freigabeversprechen. Grenzen müssen in den jeweiligen Kriterien, Versuchen und der späteren Kundenführung ausdrücklich stehen.

## Vollständiger Abgleich der zwölf Protokollverfahren

| Verfahren | Aktueller Stand |
|---|---|
| Geraderichten | Vorhanden; Originalpixelmessung und Rückabbildung vorhanden, synthetisch geprüft. |
| Perspektivkorrektur | Kein allgemeiner Entzerrer, keine zuverlässige starke-Perspektive-Sperre; offen. |
| Globale Aufhellung | Keine automatische Aufhellung im Iro-Messpfad. Erkennungsverbesserung noch nicht implementiert; kein verbotener Korrekturpfad. |
| Weißabgleich/Kalibrierung | Kein getrennter oder ungeprüfter Kalibrierpfad. Echte Kameraeinstellungen noch nicht implementiert. |
| Kontrastverstärkung | Kein Kontrastfilter im Messpfad gefunden. |
| Entrauschen | Robuste innere Pixelauswahl und linearer Mittelwert; Suchansicht getrennt geglättet. Kein grenzüberschreitender Messbildfilter. |
| Nachschärfen | Kein Nachschärfen im Messpfad gefunden. |
| Bewegungs-/Fokusunschärfe | Keine Rekonstruktion. Erkannter ungeeigneter Befund nun aufnahmeweit gesperrt; Erkennung weiterhin begrenzt. |
| Überbelichtung/Clipping | Neue konservative Endpunktsperre; kein Abdunkeln zur Kaschierung. Keine vollständige Sensor-Clipping-Erkennung aus bereits verarbeiteten RGB-Werten zugesagt. |
| Starke Unterbelichtung | Endpunktfälle werden erfasst; allgemeine Aufnahmegüteprüfung fehlt. |
| Schatten/ungleiche Beleuchtung | Innere räumliche Gleichmäßigkeit vorhanden; gegenseitige Beleuchtungsvergleichbarkeit und abgestufte Reaktion fehlen. |
| Glanz/Reflexe | Kleine Ausreißer werden robust entfernt; räumliche Variation/große Ausreißeranteile sperren. Eine große homogene Reflexfläche kann trotzdem durchkommen; keine Rekonstruktion vorhanden. |

## IroGen: Nutzen und Aussagegrenzen

IroGen besitzt funktionierende Serien, wiederholbare Seeds, testplangesteuerte Optionen, Nah-/Fernstufen, zufällige Lage, Seiten-/Höhenperspektive und Wandabstand. Übergabe, Hintergrundanalyse und kompakter Export sind vorhanden. Die Anzeige ist an die aktuelle Serie gebunden. Farbpalette und nominale Abstände bleiben vom Analyzer getrennt.

Die unterschiedlichen Effekte auf Wand und Streifen im **Generator** sind keine verbotenen Korrekturen der **Messung**. Wandstruktur, Streifenglanz, Abstandsschatten und Verdeckung simulieren absichtlich unterschiedliche reale Aufnahmebedingungen. Sie dürfen nicht entfernt werden, um vermeintlich gemeinsame Photometrie zu erzwingen.

Modellgrenzen: Seiten-/Höhenwinkel stammen aus einer künstlichen Projektionsgeometrie; Wandabstand und Schatten sind Annahmen. Fokus-/Bewegungsblur wird im Generator durch Boxmittelung kodierter Farbwerte erzeugt, nicht als kalibriertes physikalisches Sensormodell. Sensorrauschen, Materialreflexion, Kamera-ISP und spektrales Mischlicht sind nicht vollständig simuliert. Solche Bilder sind nützlich für reproduzierbare Entwicklungsfälle, aber keine physische Farbreferenz. Synthetische Fälle, die zur Abstimmung verwendet wurden, sind keine unabhängigen Abnahmefälle.

## Ausgeführte Prüfungen

- Kern: `dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release` — **68 bestanden**, keine übersprungenen Tests.
- Generator/API/Anzeige: `dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release` — **121 bestanden**, keine übersprungenen Tests. Enthalten sind die beiden bestehenden 108-/113-Bilder-Pläne, unabhängige Pixeltests, die klaren Winkelvarianten und gemeinsame Anzeigeprüfungen.
- Der erste Debug-Integrationsbuild scheiterte an durch die laufenden IroGen-/Visual-Studio-Prozesse gesperrten DLLs. Die Prüfung erfolgte anschließend erfolgreich in Release; Benutzerprozesse wurden nicht beendet.
- Gesamter Release-Build einschließlich Android: `dotnet build iro.slnx --no-restore -c Release` erfolgreich, 0 Fehler; 8 bereits bekannte XAML-Hinweise zu nicht kompilierten Bindings. Ein Build ersetzt keine Bedien- oder Geräteabnahme.

Aktuelle kompakte Berichte: [108 räumliche Fälle](../tests/adjustments/regelaudit-20260922/raumlage/bericht.md), [113 Bildqualitätsfälle](../tests/adjustments/regelaudit-20260922/bildqualitaet.md). Die zusätzliche Datei `raumlage/vorher.md` ist der Kontrolllauf **derselben Analyse 0.5.0 mit deaktiviertem Geraderichten**, kein Lauf der früheren Analyseversion.

| Entwicklungsserie | Vollständig / nominal unauffällig | Nominal auffällig | Teilweise | Abgewiesen | Verarbeitungsfehler |
|---|---:|---:|---:|---:|---:|
| 108 räumliche Fälle, Analyse 0.5.0 | 58 | 4 | 11 | 35 | 0 |
| 113 Bildqualitätsfälle, Analyse 0.5.0 | 33 | 9 | 16 | 55 | 0 |

Zum Vergleich enthielt der vom Nutzer übermittelte [108-Bilder-Bericht mit Analyse 0.4.0](../iro-gen/testplans/iro-testbericht-20260922-135106.md) 58 vollständig/nominal unauffällige, 8 nominal auffällige, 22 teilweise und 20 abgewiesene Aufnahmen. Weniger nominal auffällige Freigaben allein beweisen keine höhere reale Farbgenauigkeit; die zusätzlich abgelehnten Aufnahmen sind Teil des Sicherheitsverhaltens. Alle 17 ungestörten Kontrollen im Bildqualitätsplan bleiben vollständig/nominal unauffällig. Bei den reinen zufälligen Drehfällen sind neun vollständig messbar; drei tatsächlich angeschnittene horizontale Streifen werden jetzt abgewiesen statt teilweise gemessen. Der separate klare Winkeltest mit 22 vollständig sichtbaren Varianten besteht weiterhin.

## Status und Abnahme

Audit erstellt und durch Codeprüfung sowie reproduzierbare Tests gestützt. Die beschriebenen Korrekturen sind umgesetzt und im angegebenen Umfang geprüft. **Keine Nutzerabnahme dieser Korrekturen, keine Erledigt-Markierung offener Schutzprüfungen und keine Kundenfreigabe.**

Die nächsten Arbeiten dienen dem Schließen der oben benannten Sicherheits- und Nachweislücken. Zusätzliche Bildoptimierungen sind bis dahin zurückgestellt. Die Kamera bleibt gemäß bisherigem Beschluss eine spätere Phase nach der technischen Gesamtabnahme.