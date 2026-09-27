#requires -Version 7.0
$ErrorActionPreference = 'Stop'
Write-Output "PowerShell $($PSVersionTable.PSVersion)"
$projectRoot = Split-Path (Split-Path $PSScriptRoot)
$planSchema = Join-Path $PSScriptRoot 'irogen-testplan-v2.schema.json'
foreach ($name in @('feldzuordnung-und-messwerte.json','zwischenkontrolle-zwei-bilder.json','perspektivkorrektur-konturpruefung.json','unschaerfe-strenge-ablehnung.json','beleuchtung-schutzpruefung.json','rangfolge-und-gleichstaende.json')) {
    $json = Get-Content (Join-Path $projectRoot "iro-gen/testplans/$name") -Raw
    if (-not (Test-Json -Json $json -SchemaFile $planSchema)) { throw "Ungültiger Plan: $name" }
}
$plan = Get-Content (Join-Path $projectRoot 'iro-gen/testplans/feldzuordnung-und-messwerte.json') -Raw | ConvertFrom-Json -AsHashtable
$plan.cases[0].expected.formatVersion = 1
if (Test-Json -Json ($plan | ConvertTo-Json -Depth 100) -SchemaFile $planSchema -ErrorAction SilentlyContinue) { throw 'Altes Format darf keine Felderwartungen aufnehmen.' }
$plan.cases[0].expected.formatVersion = 2
$plan.cases[0].expected.fields[0].minimumIntersectionOverUnion = 1.1
if (Test-Json -Json ($plan | ConvertTo-Json -Depth 100) -SchemaFile $planSchema -ErrorAction SilentlyContinue) { throw 'Ungültige Überdeckung akzeptiert.' }
$evidenceRoot = Join-Path $projectRoot 'tests/adjustments/felderwartungen-20260927'
$latest = Get-ChildItem $evidenceRoot -Directory | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $latest) { throw 'Zuerst FieldExpectationTests ausführen.' }
& (Join-Path $projectRoot 'tests/iro.gen.tests/Test-AnalysisRuns.ps1') -Directory $latest.FullName
$rankingPlan = Get-Content (Join-Path $projectRoot 'iro-gen/testplans/rangfolge-und-gleichstaende.json') -Raw | ConvertFrom-Json -AsHashtable
$rankingPlan.cases[0].expected.formatVersion = 2
if (Test-Json -Json ($rankingPlan | ConvertTo-Json -Depth 100) -SchemaFile $planSchema -ErrorAction SilentlyContinue) { throw 'Rangfolge in alter Erwartungsversion akzeptiert.' }
$rankingPlan.cases[0].expected.formatVersion = 3
$rankingPlan.cases[0].expected.ranking.tieTolerance = -1
if (Test-Json -Json ($rankingPlan | ConvertTo-Json -Depth 100) -SchemaFile $planSchema -ErrorAction SilentlyContinue) { throw 'Negative Gleichstandstoleranz akzeptiert.' }
$rankingRoot = Join-Path $projectRoot 'tests/adjustments/rangfolge-20260927'
$latestRanking = Get-ChildItem $rankingRoot -Directory | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $latestRanking) { throw 'Zuerst FieldExpectationTests ausführen.' }
& (Join-Path $projectRoot 'tests/iro.gen.tests/Test-AnalysisRuns.ps1') -Directory $latestRanking.FullName
Write-Output 'Sechs Pläne, vier ungültige Gegenbeispiele und zwei aktuelle Ergebnisläufe schema-geprüft.' 
