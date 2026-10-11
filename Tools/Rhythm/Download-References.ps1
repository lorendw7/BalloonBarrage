$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'references.json') -Raw | ConvertFrom-Json
$destination = Join-Path $root '.local-reference/Rhythm/StepMania'
foreach ($entry in $manifest.files) {
    $target = [IO.Path]::GetFullPath((Join-Path $destination $entry.path))
    if (-not $target.StartsWith($destination + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw 'Reference path escapes the download directory.'
    }
    New-Item -ItemType Directory -Force (Split-Path $target) | Out-Null
    if (Test-Path -LiteralPath $target) {
        if ((Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash -eq $entry.sha256) { continue }
        throw "Existing reference differs from the manifest: $target"
    }
    $temporary = $target + '.download'
    Invoke-WebRequest -Uri $entry.url -OutFile $temporary
    if ((Get-FileHash -LiteralPath $temporary -Algorithm SHA256).Hash -ne $entry.sha256) {
        throw "Download checksum mismatch: $temporary"
    }
    Move-Item -LiteralPath $temporary -Destination $target
}
Write-Output "Verified $($manifest.files.Count) reference files in $destination"
