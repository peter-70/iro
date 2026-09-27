#requires -Version 7.0
[CmdletBinding()]
param([switch]$Execute)

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot
$inventoryPath = Join-Path $PSScriptRoot 'abschluss-pruefbestand.json'
$schemaPath = Join-Path $PSScriptRoot 'schemas/abschluss-pruefbestand-v1.schema.json'
$inventoryJson = Get-Content -LiteralPath $inventoryPath -Raw

if (-not (Test-Json -Json $inventoryJson -SchemaFile $schemaPath)) {
    throw 'Der technische Prüfbestand entspricht nicht seinem Schema.'
}

$inventory = $inventoryJson | ConvertFrom-Json -AsHashtable
$listed = @($inventory.plans.path | Sort-Object)
$found = @(Get-ChildItem -LiteralPath (Join-Path $projectRoot 'iro-gen/testplans') -File -Filter '*.json' |
    ForEach-Object { [IO.Path]::GetRelativePath($projectRoot, $_.FullName).Replace('\', '/') } | Sort-Object)
if (Compare-Object $listed $found) {
    throw 'Der Prüfbestand enthält nicht genau alle vorhandenen IroGen-JSON-Testpläne.'
}

$imageTotal = 0
$verifiedImageTotal = 0
$unassessedImageTotal = 0
foreach ($entry in $inventory.plans) {
    $path = Join-Path $projectRoot $entry.path
    $planJson = Get-Content -LiteralPath $path -Raw
    $plan = $planJson | ConvertFrom-Json -AsHashtable
    if ($plan.formatVersion -eq 2) {
        $planSchema = Join-Path $PSScriptRoot 'schemas/irogen-testplan-v2.schema.json'
        if (-not (Test-Json -Json $planJson -SchemaFile $planSchema)) { throw "Ungültiger Testplan: $($entry.path)" }
    }
    elseif ($plan.formatVersion -ne 1) { throw "Unbekannte Testplanversion: $($entry.path)" }

    $cases = @($plan.cases)
    $images = ($cases | Measure-Object -Property count -Sum).Sum
    $verifiedCases = @($cases | Where-Object { $_.expected.verification -eq 'verified' }).Count
    $verifiedImages = ($cases | Where-Object { $_.expected.verification -eq 'verified' } |
        Measure-Object -Property count -Sum).Sum
    if ($null -eq $verifiedImages) { $verifiedImages = 0 }
    $unassessedImages = $images - $verifiedImages
    if ($images -ne $entry.images -or $cases.Count -ne $entry.cases -or
        $verifiedCases -ne $entry.verifiedCases -or $unassessedImages -ne $entry.unassessedImages) {
        throw "Inventarangaben passen nicht zum Testplan: $($entry.path)"
    }
    $imageTotal += $images
    $verifiedImageTotal += $verifiedImages
    $unassessedImageTotal += $unassessedImages
}

if ($imageTotal -ne $inventory.plannedImages -or $verifiedImageTotal -ne $inventory.verifiedExpectationImages -or
    $unassessedImageTotal -ne $inventory.diagnosticOrUnassessedImages -or $imageTotal -ne $verifiedImageTotal + $unassessedImageTotal) {
    throw 'Die Summen des Prüfbestands widersprechen den Einzelplänen.'
}
if (@($inventory.knownGaps | Where-Object { $_.id -eq 'uniform-reflection-overlay' -and $_.text -match 'Zwölf' }).Count -ne 1) {
    throw 'Die zwölf bekannten Reflex-Fehlfreigaben fehlen in der Lückenliste.'
}

Write-Output "PowerShell $($PSVersionTable.PSVersion)"
Write-Output "Prüfbestand gültig: $($inventory.plans.Count) Pläne, $imageTotal Bilder; $verifiedImageTotal mit geprüfter Erwartung, $unassessedImageTotal diagnostisch oder unbewertet."
Write-Output "Bekannte Lücken: $($inventory.knownGaps.Count)."
Write-Output 'Vorgesehene Befehle:'
$inventory.commands | ForEach-Object { Write-Output "  [$($_.id)] $($_.command)" }

if (-not $Execute) {
    Write-Output 'Vorschau abgeschlossen. Kein Build und kein Testlauf gestartet. Für den später autorisierten Sammellauf -Execute angeben.'
    return
}

Push-Location $projectRoot
try {
    foreach ($step in $inventory.commands) {
        Write-Output "Starte [$($step.id)] ..."
        switch ($step.id) {
            'restore' { & dotnet restore iro.slnx --locked-mode }
            'build' { & dotnet build iro.slnx --no-restore -c Release }
            'core-tests' { & dotnet test tests/iro.core.tests/Iro.Core.Tests.csproj --no-restore -c Release --verbosity minimal }
            'generator-tests' { & dotnet test tests/iro.gen.tests/IroGen.Tests.csproj --no-restore -c Release --verbosity minimal }
            'schema-tests' { & tests/schemas/Invoke-FieldExpectations.ps1 }
            'android-release' { & dotnet build src/iro.app/Iro.App.csproj -f net10.0-android --no-restore -c Release --verbosity minimal }
            default { throw "Unbekannter fest codierter Prüfschritt: $($step.id)" }
        }
        if ($LASTEXITCODE -ne 0) { throw "Prüfschritt fehlgeschlagen: $($step.id), Exitcode $LASTEXITCODE" }
    }
}
finally { Pop-Location }
