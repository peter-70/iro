# Entwicklungsumgebung

Historischer Einrichtungsstand der Android-App: 18. September 2026. Ergänzung zu IroGen: 19. September 2026.

## Geprüfte lokale Installation

- Windows x64, .NET SDK 10.0.401, MSBuild 18.9.
- MAUI SDK und Microsoft.Maui.Controls 10.0.20.
- Android-Workload 36.1.69, Visual-Studio-verwaltet.
- Android SDK: `C:\Program Files (x86)\Android\android-sdk`.
- OpenJDK 21.0.8 unter `C:\Program Files\Android\openjdk\jdk-21.0.8`.
- Vorhandener Emulator: `pixel_7_-_api_36`, Android 36, x86_64.
- App-Ziel: `net10.0-android`; vorläufige Mindestversion Android API 24.
- Entwicklungskennung: `de.example.iro`, Anzeigename: Iro.

Maschinenabhängige SDK-Pfade sind nicht in die Projektdateien eingebaut. Die vorhandene Installation baut die Android-App ohne zusätzliche Workloadinstallation.

## Umfang dieses Schritts

Android-Solution mit MAUI-App, plattformunabhängiger Bibliothek, xUnit-Projekt und Testgenerator-Gerüst. Keine iOS-, MacCatalyst- oder Windows-App-Ziele. Kameraberechtigung ist deklariert und über die Startseite anforderbar. Android-App-Einstellungen sind erreichbar; bei Rückkehr wird der Status neu geprüft. Noch keine Kameravorschau, Kameraautomatiken, Farbauswertung oder Testaufzeichnung implementiert.

Die App fordert keine Speicher-, Mikrofon- oder Standortberechtigung an. Debug-Werkzeuge können zusätzliche Berechtigungen für ihre Laufzeitverbindung ergänzen.

## Prüfstand

- Vollständiger Debug-Build der Solution: erfolgreich, keine Warnungen oder Fehler.
- Testprojekt baut und Testadapter startet; derzeit **keine fachlichen Tests vorhanden**. Das ist kein Nachweis der späteren Farbberechnung.
- Testgenerator-Gerüst startet und benennt seinen noch nicht implementierten Umfang.
- Emulatorprüfung wird nach Abschluss unten protokolliert.
- Visual-Studio-Debugger und echtes Motorola noch nicht geprüft.

## Nächster Entwicklungsschritt

Gemäß Planfassung 1.16 zunächst Datenverträge und Testeingabequelle vervollständigen sowie Hauptoberfläche, Einstellungen und Bildanalyse im Testmodus verbinden. Technische Gesamtabnahme mit generierten Bildern und Bildfolgen im Emulator. Erst danach native Kameravorschau, Framezugriff und reale Gerätetests. Der derzeitige Berechtigungsbildschirm ist nur das Grundgerüst; der künftige Testmodus benötigt keinen Kamerazugriff.

## Dokumentation

- [Microsoft: Kameraberechtigung in MAUI](https://learn.microsoft.com/dotnet/maui/platform-integration/appmodel/permissions?view=net-maui-10.0)
- [Microsoft: Android Device Manager](https://learn.microsoft.com/dotnet/maui/android/emulator/device-manager?view=net-maui-10.0)

## Git auf diesem Rechner

Das Repository wurde initialisiert, noch kein Commit erstellt. Weil die Sandbox das Git-Verzeichnis unter einem anderen Windows-Benutzer erzeugt hat, wurde für genau `D:/Source/iro` ein `safe.directory`-Eintrag in der Git-Benutzerkonfiguration gesetzt. Keine pauschale Vertrauensfreigabe für andere Verzeichnisse.

## Emulatorprüfung

Iro wurde auf dem vorhandenen Pixel-7-Emulator mit Android 36 installiert und gestartet. Kameraberechtigung abgelehnt, erneut angefordert und erteilt; die Oberfläche zeigte jeweils den passenden Status. Keine AndroidRuntime-Absturzmeldung bei diesen Prüfungen. Ein abschließender Screenshot bestätigte die Darstellung im Software-Grafikmodus.

Der vorhandene Grafikmodus `auto` erzeugte im Hintergrundtest OpenGL-Fehler und schwarze Screenshots. Ein vorübergehender Start mit `-gpu swiftshader_indirect -no-snapshot` lieferte eine sichtbare Startseite. Die gespeicherte Emulator-Konfiguration wurde nicht verändert. Im Android Device Manager bei schwarzem Fenster auf Software-Grafik umstellen und einen Kaltstart ausführen.

Alternativ vor dem Start aus Visual Studio in PowerShell:
```powershell
& 'C:\Program Files (x86)\Android\android-sdk\emulator\emulator.exe' -avd pixel_7_-_api_36 -gpu swiftshader_indirect -no-snapshot
```

Danach in Visual Studio den bereits laufenden Emulator als Ziel wählen. Die erste vollständige Installation benötigt etwas Zeit. Für den unabhängigen Test wurde das APK mit `-p:EmbedAssembliesIntoApk=true` gebaut und mit `adb install --no-incremental -r` vollständig installiert. Der Hintergrund-Testemulator wurde anschließend beendet.

Nicht geprüft: Visual-Studio-Debugger-Anbindung, Einstellungsrückkehr nach externem Berechtigungswechsel und echte Kamerafunktion. Diese bleiben nächste Integrationsprüfungen.

## IroGen – WPF-Testbildgenerator, 19. September 2026

Zusätzlich zur Android-App besteht jetzt die eigenständige Windows-Entwicklungsanwendung `iro-gen/IroGen.csproj` mit eigener `IroGen.slnx`. Ziel: `net10.0-windows`, WPF, SDK 10.0.401. Visual Studio Community 2026 18.9.3 und Windows-Desktop-Runtime 10.0.12 wurden lokal bestätigt. Das ist ein Entwicklerwerkzeug, kein zusätzliches Plattformziel der Iro-App.

Die Generatoroberfläche erzeugt Wand-/Streifenbilder mit Optionen, nominalem Sollwertaufdruck und PNG-/JSON-Export. 61 Generatorprüfungen bestanden; Gesamtbuild mit serieller Projektverarbeitung erfolgreich. Einzelheiten und Grenzen: [IroGen-Prüfstand](irogen-pruefung.md), [Bedienung](../iro-gen/README.md). Die oben dokumentierten früheren Android-Befunde bleiben historisch; Kamera- und Farbauswertung der Android-App wurden durch diesen Schritt nicht implementiert.

### Historischer Prüfstand: IroGen-Serienerweiterung, Planfassung 1.18

IroGen 1.1.0 unterstützt Bildserien, systematische nominale ΔE00-Abdeckung und lokale Iro-Testaufträge. Aktueller Generator-Prüfstand: 70 bestandene Tests; vollständige Serien mit 100 und 500 Bildern erzeugt und schema-validiert. Keine neue Paket-/SDK-Installation. Die Analyse in Iro und die spätere Anzeige zurückgelieferter Ergebnisse sind weiterhin nicht implementiert; die Übergabe zeigt ausdrücklich „Analyse noch nicht verfügbar“.

### Aktuell: Einzelbildanalyse und IroGen-API, Planfassung 1.19

Plattformunabhängige Pixelanalyse in Iro.Core sowie Windows-PNG-Adapter und lokaler Serienläufer in Iro.Analysis ergänzt. IroGen zeigt tatsächliche Istwerte und Diagnosen. SDK und NuGet-Paketversionen bleiben unverändert; Projektverweise und Lockdateien wurden angepasst. 50 Kernprüfungen sowie 88 Generator-/Integrationsprüfungen bestanden. Gesamtbuild einschließlich Android mit 0 Warnungen/Fehlern. Die bisher geöffnete IroGen-Instanz blieb erhalten; aktuelle Binaries liegen unter `artifacts/analysis-build/bin/IroGen/debug/`. Details und tatsächliche Erkennungslücken: [Analyse-Prüfstand](analyse-pruefung.md). Dies implementiert noch keine Kameraanalyse in der Android-Oberfläche.

### Android-Hinweisanbindung, 21. September 2026, Planfassung 1.21

Die Android-Startseite besitzt jetzt eine gemeinsame Analyseanzeige und einen ausdrücklich gekennzeichneten Testmodus mit zwei gespeicherten Originalbildern. Echte Pixelanalyse läuft im Hintergrund; Hinweise, Messwertsperren und Zustandswechsel wurden im Emulator ohne Kameraberechtigung geprüft. Drei neue Integrationstests und die bisherigen 50 Kernprüfungen bestanden. Android-Build ohne Warnungen/Fehler. Einzelheiten: [Android-Hinweise](android-hinweise-pruefung.md). Die bisherigen Aussagen über eine reine Berechtigungs-Startseite sind historisch; Live-Kamera und endgültige Oberfläche fehlen weiterhin.
