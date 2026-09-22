$ErrorActionPreference = 'Stop'
$schemaPath = Join-Path $PSScriptRoot 'aufnahme-v1.schema.json'

function Read-CaptureDescription([string]$Json) {
    if (-not (Test-Json -Json $Json -SchemaFile $schemaPath -ErrorAction SilentlyContinue)) {
        throw 'Ungültige Aufnahmebeschreibung.'
    }
    return $Json | ConvertFrom-Json -AsHashtable
}

foreach ($name in @('generator', 'kamera')) {
    $json = Get-Content (Join-Path $PSScriptRoot "examples/$name.json") -Raw
    $capture = Read-CaptureDescription $json
    if ($capture.formatVersion -ne 1) { throw 'Unerwartete Formatversion.' }
    Write-Output "Gültig eingelesen: $name"
}

$original = Get-Content (Join-Path $PSScriptRoot 'examples/generator.json') -Raw
$mutations = @(
    { param($d) $d.Remove('captureId') | Out-Null },
    { param($d) $d.width = 0 },
    { param($d) $d.formatVersion = 2 },
    { param($d) $d.actualResult = @{ deltaE00 = 12 } },
    { param($d) $d.source = 'camera' }
)
foreach ($mutate in $mutations) {
    $data = $original | ConvertFrom-Json -AsHashtable
    & $mutate $data
    $rejected = $false
    try { $null = Read-CaptureDescription ($data | ConvertTo-Json -Depth 30) }
    catch { $rejected = $true }
    if (-not $rejected) { throw 'Ungültiges Gegenbeispiel wurde akzeptiert.' }
}
Write-Output 'Fünf ungültige Gegenbeispiele korrekt abgewiesen.'
