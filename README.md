# Iro

Android-App mit .NET MAUI zum kamerabasierten Farbvergleich. Ein erster lokaler Einzelbild-Analyseweg für IroGen ist implementiert; die Android-Kameravorschau folgt gemäß [Entwicklungsplan](IRO-KONSOLIDIERTER-PLAN.md).

## In Visual Studio starten

1. `iro.slnx` in Visual Studio öffnen.
2. `Iro.App` als Startprojekt festlegen.
3. Konfiguration **Debug** und den Emulator **pixel_7_-_api_36** wählen.
4. Mit **F5** starten. Beim ersten Build werden NuGet-Pakete wiederhergestellt.
5. **Klares Testbild** oder **Unscharfes Testbild** wählen. Die App analysiert das gespeicherte PNG und zeigt Messwerte beziehungsweise den Analysehinweis. Dafür ist keine Kameraberechtigung nötig.

Die App zeigt jetzt eine gemeinsame Ergebnis-/Hinweisanzeige für die Einzelbildanalyse. Im gekennzeichneten Testmodus werden zwei gespeicherte Originalbilder ausgewertet. Die Live-Kamera ist noch nicht angebunden; reale Farb- und Kameratests folgen später. [Prüfung der Android-Hinweise](docs/android-hinweise-pruefung.md).

## Projekte

- `src/iro.app`: Android-Oberfläche und später die native Camera2-Anbindung.
- `src/iro.core`: Plattformunabhängige Einzelbildanalyse: Felderkennung, Messflächen, sRGB/Lab und ΔE00.
- `src/iro.analysis`: Lokale Windows-PNG-API und Serienläufer. [Schnittstelle und Grenzen](docs/analyse-api.md).
- `tests/iro.core.tests`: Unabhängige Referenz-, Pixel- und Qualitätsprüfungen des Iro-Kerns.
- `iro-gen`: WPF-Testbildgenerator **IroGen** mit Optionen, Bildserien, ΔE00-Abdeckung, Gesamtexport und lokaler Iro-Analyse und vorläufiger Soll/Ist-Diagnose. [Start und Bedienung](iro-gen/README.md).
- `tests/iro.gen.tests`: Fachliche Farb-, Bild-, Export- und WPF-Tests für IroGen.
- `tools/iro.testgen`: Historisches Konsolengerüst; die Bildgenerierung erfolgt jetzt mit IroGen.

## Kommandozeile

```powershell
dotnet restore iro.slnx --locked-mode
dotnet build iro.slnx
dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj
dotnet run --project tools/iro.testgen/Iro.TestGen.csproj
```

Für einen bereits laufenden Emulator:
```powershell
dotnet build src/iro.app/Iro.App.csproj -t:Run -f net10.0-android
```

SDK und Paketversionen sind über `global.json`, Projektdateien und Paket-Lockdateien festgelegt. Bei absichtlichen Paketänderungen Lockdateien mit aktualisieren.

Weitere Informationen: [Umgebung und Prüfstand](docs/entwicklungsumgebung.md).

**Emulatorhinweis:** Die vorhandene automatische Grafikbeschleunigung lieferte im Hintergrundtest schwarze Bilder. Mit Software-Grafik konnte die Startseite geprüft werden. Falls dein Emulator schwarz bleibt, folge dem dokumentierten [Software-Grafik-Startweg](docs/entwicklungsumgebung.md#emulatorprüfung).





## Verbindliche Bildoptimierung

Die beschlossene gemeinsame Messgrundlage und zulässigen Optimierungen stehen im [Entwicklungsplan](IRO-KONSOLIDIERTER-PLAN.md#64-verbindliche-bildoptimierung-und-gemeinsame-messgrundlage). Der [Arbeitsplan zur Bildoptimierung](docs/arbeitsplan-bildoptimierung.md) führt Umsetzung, Prüfung und Nutzerabnahme für jeden Schritt getrennt. Das [vollständige Entscheidungsprotokoll](docs/entscheidung-bildoptimierung-2026-09-22.md) bleibt als unveränderte Quelle erhalten.

Aktueller [Entwicklungs- und Regelaudit](docs/regelaudit-2026-09-22.md): Analyse 0.5.0 behebt nachgewiesene Freigabefehler; weitere Schutzprüfungen und die Kunden-App bleiben unvollständig. Sicherheits- und Nachweislücken haben vor neuen Bildoptimierungen Vorrang.
