# Iro-Testaufträge

IroGen stellt über **An Iro-Tests senden** eine vollständige Kopie der aktuell erzeugten Serie unter einem neuen `iro-request-…`-Ordner bereit. Ein Auftrag enthält `request.json`, ein Serienmanifest und alle PNG-/Aufnahmedateien. Er bleibt unabhängig vom temporären IroGen-Cache erhalten.

Nach der Übergabe führt IroGen den Auftrag über die lokale Iro-API aus und zeigt eine vorläufige Istwertdiagnose. Der Auftrag selbst bleibt unverändert als Eingabesammlung mit Status pending-analysis erhalten; der tatsächliche Laufstatus steht ausschließlich in der separaten Ergebnisdatei. Der erste Analyseweg ist ein begrenzter Einzelbildversuch; siehe [API](../../docs/analyse-api.md).

Verträge: [Testauftrag v2](../schemas/testauftrag-v2.schema.json), [Bildserie v1](../schemas/bildserie-v1.schema.json), [Aufnahme v1](../schemas/aufnahme-v1.schema.json). Der gemeinsame Leser prüft zusätzlich alle referenzierten Dateien und IDs sowie Pfadgrenzen. Unvollständige Ordner mit `.incomplete` am Ende nicht verarbeiten. Analyseergebnisse gehören getrennt nach `tests/runs/`; Eingangsdaten und Erwartungen nicht überschreiben.

Aufträge sind lokale Entwicklungsdaten und werden nicht automatisch in Git aufgenommen. Eine Projektkopie ist kein Backup.

