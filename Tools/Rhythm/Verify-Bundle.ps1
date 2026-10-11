param([string]$BundlePath)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
if (-not $BundlePath) { $BundlePath = Join-Path $root 'Assets/StreamingAssets/Rhythm/StepMania' }
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'references.json') -Raw | ConvertFrom-Json
foreach ($entry in $manifest.files) {
    $file = Join-Path $BundlePath $entry.path
    if (-not (Test-Path -LiteralPath $file)) { throw "Missing bundled file: $file" }
    if ((Get-Item -LiteralPath $file).Length -ne $entry.bytes -or
        (Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash -ne $entry.sha256) {
        throw "Bundled file differs from upstream: $file"
    }
}
foreach ($notice in @('NOTICE.txt', 'README.md', 'source-manifest.json')) {
    if (-not (Test-Path -LiteralPath (Join-Path $BundlePath $notice))) { throw "Missing provenance: $notice" }
}
Write-Output "RHYTHM_BUNDLE_OK: $($manifest.files.Count) original files and provenance notices in $BundlePath"
