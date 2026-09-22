param([Parameter(Mandatory)][string]$Directory)
$ErrorActionPreference = 'Stop'
$schema = Join-Path $PSScriptRoot '../schemas/aufnahme-v1.schema.json'
Add-Type -AssemblyName PresentationCore
$files = @(Get-ChildItem -LiteralPath $Directory -Recurse -File -Filter '*.json')
if ($files.Count -eq 0) { throw 'Keine Aufnahmebeschreibungen gefunden.' }
$ids = [System.Collections.Generic.HashSet[string]]::new()
$captureCount = 0
foreach ($file in $files) {
    $json = Get-Content -LiteralPath $file.FullName -Raw
    $data = $json | ConvertFrom-Json
    if ($data.kind -eq 'iro-image-series') {
        $seriesSchema = Join-Path $PSScriptRoot '../schemas/bildserie-v1.schema.json'
        if (-not (Test-Json -Json $json -SchemaFile $seriesSchema)) { throw 'Ungültiges Serienmanifest.' }
        if ($data.actualCount -ne $data.requestedCount -or $data.actualCount -ne $data.captures.Count) { throw 'Unvollständige Serie.' }
        foreach ($item in $data.captures) {
            $captureFile = Join-Path $file.DirectoryName $item.descriptionFile
            $description = Get-Content -LiteralPath $captureFile -Raw | ConvertFrom-Json
            if ($description.captureId -ne $item.captureId) { throw 'Aufnahme-ID stimmt nicht mit Manifest überein.' }
            if (-not (Test-Path -LiteralPath (Join-Path $file.DirectoryName $item.imageFile))) { throw 'Bild aus Manifest fehlt.' }
        }
        if ($data.coverRange -and ($data.coverage | Measure-Object -Property count -Sum).Sum -ne $data.actualCount) { throw 'Abstandsverteilung ist unvollständig.' }
        Write-Output "Serienmanifest gültig: $($data.actualCount) Bilder, Delta E $($data.minimumDeltaE00) bis $($data.maximumDeltaE00)."
        continue
    }
    if ($data.kind -eq 'iro-test-request') {
        if ($data.formatVersion -notin @(1, 2)) { throw 'Unbekanntes Testauftragsformat.' }
        $requestSchema = Join-Path $PSScriptRoot "../schemas/testauftrag-v$($data.formatVersion).schema.json"
        if (-not (Test-Json -Json $json -SchemaFile $requestSchema)) { throw 'Ungültiger Testauftrag.' }
        $series = Get-Content -LiteralPath (Join-Path $file.DirectoryName $data.seriesFile) -Raw | ConvertFrom-Json
        if ($series.batchId -ne $data.batchId -or $series.actualCount -ne $data.imageCount) { throw 'Testauftrag und Eingabedaten stimmen nicht überein.' }
        Write-Output "Testauftrag gültig: $($data.imageCount) Bilder; Format $($data.formatVersion)."
        continue
    }
    if (-not (Test-Json -Json $json -SchemaFile $schema)) { throw "Schemafehler: $($file.FullName)" }
    $captureCount++
    $capture = $data
    if (-not $ids.Add($capture.captureId)) { throw "Doppelte Aufnahme-ID: $($capture.captureId)" }
    $imagePath = Join-Path $file.DirectoryName $capture.imageFile
    $stream = [System.IO.File]::OpenRead($imagePath)
    try {
        $decoder = [System.Windows.Media.Imaging.BitmapDecoder]::Create($stream, [System.Windows.Media.Imaging.BitmapCreateOptions]::PreservePixelFormat, [System.Windows.Media.Imaging.BitmapCacheOption]::OnLoad)
        $image = $decoder.Frames[0]
        if ($image.PixelWidth -ne $capture.width -or $image.PixelHeight -ne $capture.height) { throw 'Bildgröße stimmt nicht mit Metadaten überein.' }
    }
    finally { $stream.Dispose() }
    foreach ($field in $capture.expected.fields) {
        if ($null -ne $field.bounds) {
            $b = $field.bounds
            if ($b.x + $b.width -gt $capture.width -or $b.y + $b.height -gt $capture.height) { throw 'Feldbegrenzung liegt außerhalb des Bilds.' }
        }
    }
}
Write-Output "$captureCount PNG-/JSON-Paare geprüft: Schema, eindeutige IDs, Bilddateien, Bildgrößen und Feldbegrenzungen gültig."


