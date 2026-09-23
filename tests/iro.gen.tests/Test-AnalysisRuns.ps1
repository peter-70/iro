param([Parameter(Mandatory)][string]$Directory)
$ErrorActionPreference = 'Stop'
$schema = Join-Path $PSScriptRoot '../schemas/analyse-lauf-v1.schema.json'
$files = @(Get-ChildItem -LiteralPath $Directory -Recurse -File -Filter results.json)
if ($files.Count -eq 0) { throw 'Keine Analyseläufe gefunden.' }
foreach ($file in $files) {
    $json = Get-Content -LiteralPath $file.FullName -Raw
    $format = ($json | ConvertFrom-Json).formatVersion
    if ($format -notin @(1,2)) { throw 'Unbekanntes Analyseformat.' }
    $schema = Join-Path $PSScriptRoot "../schemas/analyse-lauf-v$format.schema.json"
    if (-not (Test-Json -Json $json -SchemaFile $schema)) { throw "Ungültiger Analyselauf: $($file.FullName)" }
    $run = $json | ConvertFrom-Json
    if ($run.processedCount -ne $run.captures.Count -or $run.processedCount -gt $run.requestedCount) { throw 'Ungültige Bildanzahl.' }
    if ($run.status -ne 'cancelled' -and $run.processedCount -ne $run.requestedCount) { throw 'Unvollständiger Lauf meldet Abschluss.' }
    $ids = [Collections.Generic.HashSet[string]]::new()
    $measured = 0; $images = 0
    foreach ($capture in $run.captures) {
        if (-not $ids.Add($capture.captureId)) { throw 'Doppelte Aufnahme-ID.' }
        $valid = 0
        if ($null -ne $capture.analysis) {
            foreach ($field in $capture.analysis.fields) {
                foreach ($rect in @($field.bounds, $field.innerBounds, $field.reference.bounds) | Where-Object { $null -ne $_ }) {
                    if ($rect.right -ne $rect.x + $rect.width -or $rect.bottom -ne $rect.y + $rect.height -or $rect.area -ne $rect.width * $rect.height -or $rect.right -gt $capture.analysis.width -or $rect.bottom -gt $capture.analysis.height) { throw 'Widersprüchliche Messgeometrie.' }
                }
                if ($field.measurementAllowed) { $valid++ }
            }
        }
        if ($valid -gt 0) { $images++ }; $measured += $valid
    }
    if ($images -ne $run.imagesWithMeasurements -or $measured -ne $run.measuredFields) { throw 'Zusammenfassung widerspricht Einzelbefunden.' }
    $errors = @($run.captures | Where-Object { $null -ne $_.error }).Count
    if ($run.status -eq 'completed' -and $errors -gt 0) { throw 'Dateifehler trotz fehlerfrei gemeldetem Lauf.' }
    Write-Output "Analyseformat und Zusammenfassung gültig: $($run.processedCount) Bilder; $images mit Messwerten; $measured gültige Felder."
}
