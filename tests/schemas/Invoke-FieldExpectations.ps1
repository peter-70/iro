# Reproducible schema invocation from Windows PowerShell 5.1 or PowerShell 7.
# Test-Json and ConvertFrom-Json -AsHashtable run in the explicitly selected PS7 process.
$ErrorActionPreference = 'Stop'
$bundledShell = Join-Path $env:USERPROFILE '.cache\codex-runtimes\codex-primary-runtime\dependencies\native\powershell\pwsh.exe'
if (-not (Test-Path -LiteralPath $bundledShell)) {
    throw 'Die dokumentierte Codex-PowerShell fehlt. PowerShell 7 verwenden und Test-FieldExpectations.ps1 dort starten.'
}
& $bundledShell -NoProfile -File (Join-Path $PSScriptRoot 'Test-FieldExpectations.ps1')
if ($LASTEXITCODE -ne 0) { throw "Schemaprüfung fehlgeschlagen (Exitcode $LASTEXITCODE)." }
