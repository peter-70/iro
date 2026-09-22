# Android-Analysehinweise – 21. September 2026

Planfassung 1.21. `AnalysisPresentation` verbindet Analyseergebnisse mit einer gemeinsamen MAUI-Anzeige: Überschrift, konkreter Analysehinweis und Feldwerte/-hinweise. Nicht freigegebene Werte erscheinen als Gedankenstrich. Ein neuer Test entfernt alte Werte sofort; Ladefehler, Abbruch sowie Seiten-/Hintergrundwechsel entfernen sie ebenfalls. Ergebnisse überholter Aufgaben dürfen die aktuelle Anzeige nicht überschreiben.

Die Android-Startseite bietet **Klares Testbild** und **Unscharfes Testbild**. Die gespeicherten PNGs werden über den Android-Bitmapdecoder in RGB24 überführt und tatsächlich vom gemeinsamen `ImageAnalyzer` ausgewertet. Es werden keine vorgefertigten Ergebniszahlen oder Meldungen anhand des Bildnamens angezeigt. Testmodus und fehlende Live-Kameraanbindung sind ausdrücklich beschriftet. Eine Kameraberechtigung ist für die Tests nicht erforderlich.

Die beiden festen Entwicklungsbilder unter `src/iro.app/Resources/Raw/` stammen aus den Nutzerläufen:

| Datei | Herkunft | SHA-256 |
|---|---|---|
| `test-normal.png` | `iro-run-27fbeb04a17d4744a68e2c4b41352b9a` | `9857d23492e799174ce3b1fd9db7cae479f126762a9df744b2fe6d452472a984` |
| `test-blur.png` | `iro-run-e96672920cee46fda9b0e7997daa16ab` | `18c4dd2028fbf765366431b784453092cd0082b155b517cd95f1c71ae35fdeea` |

Beide: IroGen, Seed 12345, 1600×1200, sieben Felder, rechts/vertikal, Wand identisch mit Feld 3. Einziger Unterschied ist starke Unschärfe im zweiten Bild. Entwicklungsfälle, keine unabhängigen Abnahmebilder.

## Automatisierte Prüfung

Drei neue Integrationstests bestanden: echtes klares PNG → sieben Messwerte → echtes unscharfes PNG → Hinweis **„Bild unscharf. Kamera ruhig halten und neu fokussieren.“** ohne freigegebene Werte → klares PNG wieder mit Werten; überholtes/abgebrochenes Bild überschreibt kein neueres; Ladefehler entfernt vorherige Zahlen. Zusätzlich alle 50 bisherigen Kernprüfungen bestanden.

```powershell
dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore --artifacts-path artifacts/analysis-build --filter FullyQualifiedName~AndroidPresentationTests
dotnet build src/iro.app/Iro.App.csproj --no-restore --artifacts-path artifacts/analysis-build -p:EmbedAssembliesIntoApk=true -m:1
```

## Grenzen

Dies verbindet die Ergebnisanzeige im Android-Testmodus, nicht die Live-Kamera. Kamera-Pufferzuordnung, endgültige Haupt-/Einstellungsoberfläche, zeitliche Messstabilität und die vollständige technische Abnahme bleiben offen. Die bekannte falsch gewählte Wandreferenz des Unschärfefalls wurde in diesem Schritt nicht korrigiert. Die bereits vorhandene Unschärfesperre verhindert in diesem Fall weiterhin jeden Messwert; ein Hinweis allein behebt keinen Erkennungsfehler.

## Android-Emulatorprüfung

Auf dem vorhandenen Pixel-7-Emulator (Android 36, Software-Grafik) installiert und ohne Kameraberechtigung ausgeführt. Der echte Button **Klares Testbild** liefert die normalen ΔE00-Werte. Anschließend **Unscharfes Testbild**: Überschrift „Keine zuverlässige Messung“, exakter Unschärfehinweis sichtbar, Feldwert „–“, keine bisherigen Zahlen mehr. Rückwechsel zum klaren Bild liefert wieder Werte. Home-Taste und Rückkehr zur App: „Analyse angehalten“ und entfernte Messwerte. UI-Hierarchien wurden automatisiert geprüft; Screenshots und XML liegen unter `artifacts/android-hints/`.

Android-Build mit eingebetteten Assemblies erfolgreich, 0 Warnungen und 0 Fehler. Die Kamera wurde für diese Prüfung weder verwendet noch freigegeben. Die Schwäche bei der Wandreferenz bleibt unabhängig von der nachgewiesenen Anzeige bestehen.

Die finale APK-Fassung wurde erneut installiert; Unschärfehinweis, verständliche Feldbezeichnung und gesperrter Wert wurden per UI-Hierarchie bestätigt und im Screenshot visuell geprüft. Der ausschließlich für diese Prüfung gestartete Hintergrundemulator wurde anschließend beendet.

