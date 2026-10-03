$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$assets = Join-Path $root 'Assets'
$missingMetadata = @(Get-ChildItem -LiteralPath $assets -Recurse | Where-Object {
    $_.Extension -ne '.meta' -and -not $_.Name.StartsWith('.') -and
    -not (Test-Path -LiteralPath ($_.FullName + '.meta'))
})
if ($missingMetadata.Count) { $missingMetadata.FullName; throw 'Missing asset or folder metadata' }
$orphanMetadata = @(Get-ChildItem -LiteralPath $assets -Recurse -Filter '*.meta' -File | Where-Object {
    -not (Test-Path -LiteralPath $_.FullName.Substring(0, $_.FullName.Length - 5))
})
if ($orphanMetadata.Count) { $orphanMetadata.FullName; throw 'Metadata without its asset or folder' }
$metas = Get-ChildItem -LiteralPath (Join-Path $root 'Assets') -Recurse -Filter '*.meta' -File | ForEach-Object {
    $guid = Select-String -LiteralPath $_.FullName -Pattern '^guid: (.+)$'
    if ($guid) { [pscustomobject]@{Path=$_.FullName;Guid=$guid.Matches[0].Groups[1].Value} }
}
$duplicates = @($metas | Group-Object Guid | Where-Object Count -gt 1)
if ($duplicates.Count) { $duplicates | ForEach-Object {$_.Group} | Format-Table; throw 'Duplicate asset GUIDs' }
$broken = @()
$docs = @(Get-Item -LiteralPath (Join-Path $root 'README.md')) + @(Get-ChildItem -LiteralPath (Join-Path $root 'Docs') -Recurse -Filter '*.md' -File)
foreach ($doc in $docs) {
    foreach ($match in [regex]::Matches((Get-Content -LiteralPath $doc.FullName -Raw), '\]\(([^)]+)\)')) {
        $link = $match.Groups[1].Value
        if ($link -match '^(https?://|#)') { continue }
        $destination = [IO.Path]::GetFullPath((Join-Path $doc.DirectoryName ($link.Split('#')[0])))
        if (-not (Test-Path -LiteralPath $destination)) { $broken += "$($doc.Name): $link" }
    }
}
if ($broken.Count) { $broken; throw 'Broken documentation links' }
Write-Output "STATIC_AUDIT_OK: $($metas.Count) unique metadata GUIDs; $($docs.Count) documentation files checked."
