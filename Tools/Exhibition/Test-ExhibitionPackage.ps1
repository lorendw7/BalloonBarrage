[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$BuildPath,

    [string]$ExecutableName = "BalloonShooter.exe",

    [switch]$ReleasePackage
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$resolvedBuild = Resolve-Path -LiteralPath $BuildPath
$root = Get-Item -LiteralPath $resolvedBuild
if (-not $root.PSIsContainer) {
    throw "BuildPath must be a directory: $($root.FullName)"
}

$checks = [System.Collections.Generic.List[object]]::new()
function Add-Check {
    param(
        [string]$Name,
        [bool]$Passed,
        [string]$Detail
    )

    $checks.Add([pscustomobject]@{
        Check = $Name
        Result = if ($Passed) { "PASS" } else { "FAIL" }
        Detail = $Detail
    })
}

$exePath = Join-Path $root.FullName $ExecutableName
$playerName = [System.IO.Path]::GetFileNameWithoutExtension($ExecutableName)
$dataPath = Join-Path $root.FullName ($playerName + "_Data")
$unityPlayerPath = Join-Path $root.FullName "UnityPlayer.dll"
$crashHandlerPath = Join-Path $root.FullName "UnityCrashHandler64.exe"
$monoPath = Join-Path $root.FullName "MonoBleedingEdge"
$gameAssemblyPath = Join-Path $root.FullName "GameAssembly.dll"

Add-Check "Executable" ((Test-Path -LiteralPath $exePath -PathType Leaf) -and ((Get-Item -LiteralPath $exePath -ErrorAction SilentlyContinue).Length -gt 0)) $exePath
Add-Check "Player data" (Test-Path -LiteralPath $dataPath -PathType Container) $dataPath
Add-Check "Unity runtime" (Test-Path -LiteralPath $unityPlayerPath -PathType Leaf) $unityPlayerPath
Add-Check "Crash handler" (Test-Path -LiteralPath $crashHandlerPath -PathType Leaf) $crashHandlerPath
Add-Check "Scripting runtime" ((Test-Path -LiteralPath $monoPath -PathType Container) -or (Test-Path -LiteralPath $gameAssemblyPath -PathType Leaf)) "MonoBleedingEdge or GameAssembly.dll"

$stepManiaPath = Join-Path $dataPath "StreamingAssets\Rhythm\StepMania"
$noticePath = Join-Path $stepManiaPath "NOTICE.txt"
$manifestPath = Join-Path $stepManiaPath "source-manifest.json"
Add-Check "StepMania notice" (Test-Path -LiteralPath $noticePath -PathType Leaf) $noticePath
Add-Check "StepMania manifest" (Test-Path -LiteralPath $manifestPath -PathType Leaf) $manifestPath

$doNotShip = @(Get-ChildItem -LiteralPath $root.FullName -Force -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -like "*_DoNotShip" -or $_.Name -like "*BackUpThisFolder_ButDontShipItWithYourGame*" })
Add-Check "No Unity backup payload" ($doNotShip.Count -eq 0) $(if ($doNotShip.Count -eq 0) { "none" } else { ($doNotShip.Name -join ", ") })

if ($ReleasePackage) {
    $creditsPath = Join-Path $root.FullName "CREDITS.md"
    $readmePath = Join-Path $root.FullName "README.txt"
    Add-Check "Packaged credits" (Test-Path -LiteralPath $creditsPath -PathType Leaf) $creditsPath
    Add-Check "Packaged readme" (Test-Path -LiteralPath $readmePath -PathType Leaf) $readmePath
}

$files = @(Get-ChildItem -LiteralPath $root.FullName -Recurse -File -Force)
$totalBytes = ($files | Measure-Object -Property Length -Sum).Sum
$exeHash = if (Test-Path -LiteralPath $exePath -PathType Leaf) {
    (Get-FileHash -LiteralPath $exePath -Algorithm SHA256).Hash
} else {
    "unavailable"
}

$checks | Format-Table -AutoSize
Write-Host ""
Write-Host ("Files: {0}" -f $files.Count)
Write-Host ("Bytes: {0}" -f $totalBytes)
Write-Host ("Executable SHA-256: {0}" -f $exeHash)

$failures = @($checks | Where-Object { $_.Result -eq "FAIL" })
if ($failures.Count -gt 0) {
    Write-Error ("Exhibition package preflight failed: {0} check(s)." -f $failures.Count)
    exit 1
}

Write-Host "Exhibition package preflight passed. This does not replace launching and playing the exact package."
exit 0
