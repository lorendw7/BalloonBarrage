param([switch]$Apply)
$ErrorActionPreference = 'Stop'
$sourceRoot = 'D:\CS\Code\Unity\BalloonShooter'
$targetRoot = 'D:\CS\Code\BalloonShooter'
if ($Apply -and (Test-Path -LiteralPath (Join-Path $targetRoot 'Docs\Project\STRUCTURE.md'))) {
    throw 'This project is already consolidated. Run without -Apply for an audit; do not overwrite newer work from the retired project.'
}
$folders = @('Assets','Packages','ProjectSettings')
$records = @()
foreach ($folder in $folders) {
    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $sourceRoot $folder) -File -Recurse) {
        $relative = $file.FullName.Substring($sourceRoot.Length + 1)
        $destination = Join-Path $targetRoot $relative
        $action = 'Add'
        if (Test-Path -LiteralPath $destination) {
            $action = if ((Get-FileHash -LiteralPath $file.FullName).Hash -eq (Get-FileHash -LiteralPath $destination).Hash) {'Identical'} else {'ReplaceWithSavedUnityVersion'}
        }
        $records += [pscustomobject]@{Path=$relative;Action=$action;Source=$file.FullName;Target=$destination}
    }
}
$records | Group-Object Action | Select-Object Name,Count | Format-Table
$records | Where-Object Action -eq 'ReplaceWithSavedUnityVersion' | Select-Object Path | Format-Table
if (-not $Apply) { return }
$backupRoot = Join-Path $targetRoot ('.local-backups\before-consolidation-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
# Snapshot both saved projects before replacing anything; omit generated caches.
$targetPaths = @('Assets','Packages','ProjectSettings','Docs','README.md','PLAN.md','.gitignore','.gitattributes') | ForEach-Object {Join-Path $targetRoot $_} | Where-Object {Test-Path -LiteralPath $_}
$sourcePaths = @('Assets','Packages','ProjectSettings','Docs','README.md','CREDITS.md','.gitignore','.gitattributes') | ForEach-Object {Join-Path $sourceRoot $_} | Where-Object {Test-Path -LiteralPath $_}
Compress-Archive -LiteralPath $targetPaths -DestinationPath (Join-Path $backupRoot 'repository-before.zip')
Compress-Archive -LiteralPath $sourcePaths -DestinationPath (Join-Path $backupRoot 'editor-project-before.zip')
foreach ($record in $records | Where-Object Action -ne 'Identical') {
    New-Item -ItemType Directory -Path (Split-Path -Parent $record.Target) -Force | Out-Null
    Copy-Item -LiteralPath $record.Source -Destination $record.Target -Force
}
Copy-Item -LiteralPath (Join-Path $sourceRoot 'CREDITS.md') -Destination (Join-Path $targetRoot 'CREDITS.md')
Write-Output "BACKUP: $backupRoot"
Write-Output 'Saved Unity assets/settings merged. Source project and both Git histories untouched.'
