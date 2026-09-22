# Testläufe

Ein Ordner je Lauf: Zeitpunkt, Versionen, Eingaben, Einstellungen, tatsächliche Ergebnisse und Abweichungen als JSON sowie ein kurzer Markdown-Bericht.

Laufdaten sind von Git ausgeschlossen. Separate Sicherung vorsehen. Aktuelle Emulator-Protokolle sind nur technische Startdiagnosen, keine fachliche Abnahme.

Der aktuelle Einzelbildläufer schreibt je Lauf eine results.json gemäß [Analyselauf v1](../schemas/analyse-lauf-v1.schema.json). Referenzierte Eingaben bleiben im über die requestId zugeordneten Testauftrag. Ablauf und Grenzen: [lokale API](../../docs/analyse-api.md).
