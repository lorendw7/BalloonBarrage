# Desktop release runbook

This is the operational source for building, staging and publishing BalloonBarrage desktop packages. [Game design](../Design/GAME_DESIGN.md) owns behavior, the [Roadmap](../Design/ROADMAP.md) owns delivery gates, [Validation](VALIDATION.md) records evidence, and [Website and releases](WEBSITE_AND_RELEASES.md) explains how published assets reach the site.

## Release model

Keep these outcomes separate:

1. **Build:** Unity writes a Windows x64 player or macOS Universal application.
2. **Package:** the release tool verifies the structure, adds notices, creates an immutable ZIP and writes SHA-256.
3. **Stage:** a GitHub Draft Release stores both candidates without exposing them on the website.
4. **Accept:** the exact extracted ZIP is tested on its target operating system and the evidence is recorded.
5. **Publish:** the draft becomes a prerelease or release; the website then discovers its platform-labelled assets.

A successful compilation, ZIP check or website test is not a physical playtest. Never replace an existing published asset: rebuild under a new version.

## Primary path: local dual-platform build

Local Unity builds are the default because they use the creator's activated Unity Hub installation and avoid the unresolved Linux-container login failure recorded in [Validation](VALIDATION.md#desktop-delivery-pipeline-checkpoint-october-21). GitHub Actions remains a Windows-only fallback and never publishes a release automatically.

### Prerequisites

- Unity Editor **6000.0.77f1**, activated through Unity Hub.
- **Mac Build Support (Mono)** added to that exact Editor installation.
- Windows support included with the Windows Editor. **Windows Build Support (IL2CPP)** is optional unless the project explicitly switches to IL2CPP.
- Git LFS content downloaded, Python 3 available, and GitHub CLI authenticated before upload.
- A named commit pushed to the repository and a separate clean checkout of that exact commit. Do not build a release from the learner's active working directory.
- Enough free space for both Unity outputs, package ZIPs and a second project Library.

The macOS output is Universal (Intel 64-bit plus Apple silicon), unsigned and unnotarized. It remains a candidate until tested after fresh extraction on a real Mac.

### Prepare an isolated checkout

Choose the version before building, for example `0.1.0-demo.3`. Confirm the active repository is clean, then create a detached worktree outside it from the intended commit:

```powershell
$commit = "REPLACE_WITH_COMMIT_SHA"
git status --short
git worktree add --detach D:\CS\Build\BalloonBarrage-v0.1.0-demo.3 $commit
git -C D:\CS\Build\BalloonBarrage-v0.1.0-demo.3 lfs pull
```

Keep the worktree and build logs until both packages are accepted or rejected. Do not copy uncommitted gameplay files into it.

### Build both players

Close Unity Editor instances using the isolated checkout. Run the following from PowerShell, changing only the version and paths. Every output directory must be new or empty.

```powershell
$version = "0.1.0-demo.3"
$unity = "D:\Software\Unity Editor\6000.0.77f1\Editor\Unity.exe"
$project = "D:\CS\Build\BalloonBarrage-v$version"
$output = "D:\CS\Build\BalloonBarrage-v$version-output"
New-Item -ItemType Directory -Path $output

& $unity -batchmode -quit `
  -projectPath $project `
  -buildTarget StandaloneWindows64 `
  -executeMethod BalloonBarrage.EditorTools.DesktopReleaseBuild.Build `
  -balloonTarget windows `
  -balloonVersion $version `
  -balloonOutput "$output\windows" `
  -logFile "$output\windows-build.log"
if ($LASTEXITCODE -ne 0) { throw "Windows build failed with exit code $LASTEXITCODE" }

& $unity -batchmode -quit `
  -projectPath $project `
  -buildTarget StandaloneOSX `
  -executeMethod BalloonBarrage.EditorTools.DesktopReleaseBuild.Build `
  -balloonTarget macos `
  -balloonVersion $version `
  -balloonOutput "$output\macos" `
  -logFile "$output\macos-build.log"
if ($LASTEXITCODE -ne 0) { throw "macOS build failed with exit code $LASTEXITCODE" }
```

`DesktopReleaseBuild.Build` includes only `Assets/Scenes/PrototypeScene.unity`, disables Development Build and refuses a nonempty output. The macOS architecture is set to Universal for the build and restored afterward. Do not rename or separate generated runtime files.

Inspect each `BuildReport.txt` and investigate its build-step warnings before packaging. A successful build with missing-script warnings is a rejected candidate. If a target switch or reused Library produces those warnings, retry the same named source revision in a fresh per-platform checkout with a new Library; keep the rejected output and diagnostics separate. The demo.9 release investigation in [Validation](VALIDATION.md) records this case. Neither a build success nor the presence of a managed DLL proves that its scene/asset script references were retained.

### Create release evidence

Write an ignored JSON file such as `D:\CS\Build\BalloonBarrage-v0.1.0-demo.3-output\release-info.json`:

```json
{
  "date": "2026-10-21",
  "features": "Short factual list of saved behavior in this commit.",
  "verification": "Compilation and checks completed for these exact outputs.",
  "limits": "Physical playtests still required; macOS is unsigned and unnotarized; bundled music is noncommercial only."
}
```

Record only completed evidence. Planned behavior, Editor screenshots and static website tests are not standalone-player acceptance.

### Package immutable candidates

Run the packager from the isolated checkout:

```powershell
python "$project\Tools\Release\package_desktop_demo.py" `
  --source "$output\windows" `
  --output "$output\BalloonBarrage-Windows-x64-v$version.zip" `
  --tag "v$version" `
  --platform windows `
  --release-info "$output\release-info.json"

python "$project\Tools\Release\package_desktop_demo.py" `
  --source "$output\macos" `
  --output "$output\BalloonBarrage-macOS-Universal-v$version.zip" `
  --tag "v$version" `
  --platform macos `
  --release-info "$output\release-info.json"
```

The tool refuses existing outputs, checks required player paths, excludes Unity `DoNotShip` and backup payloads, embeds notices and a runtime manifest, and writes a `.zip.sha256` sidecar. For a macOS player produced on Windows, it detects Mach-O binaries and writes their executable permissions into the ZIP.

Run the focused packager tests whenever the tool changes:

```powershell
python Tools/Release/test_package_desktop_demo.py
```

### Stage a Draft Release

Do not publish directly from a build command. Upload both ZIPs and checksums to a draft so the website continues showing only previously published packages:

```powershell
$tag = "v$version"
$commit = "REPLACE_WITH_COMMIT_SHA"
$notes = "D:\CS\Build\release-notes-v$version.md"
gh release create $tag `
  "$output\BalloonBarrage-Windows-x64-v$version.zip" `
  "$output\BalloonBarrage-Windows-x64-v$version.zip.sha256" `
  "$output\BalloonBarrage-macOS-Universal-v$version.zip" `
  "$output\BalloonBarrage-macOS-Universal-v$version.zip.sha256" `
  --repo lorendw7/BalloonBarrage `
  --target $commit `
  --draft `
  --prerelease `
  --title "BalloonBarrage v$version desktop candidates" `
  --notes-file $notes
```

Drafts are private staging records, not playable public releases. Keep the noncommercial music restriction and macOS signing/testing limits in the notes.

## Acceptance of the exact ZIPs

Verify the checksum before testing, extract to a fresh directory and run the packaged copy rather than the uncompressed build output.

For Windows, test on the intended exhibition PC:

- offline launch and readable title/instructions;
- complete-song start, held fire, ordinary/on-beat hits and settlement;
- pause, resume, focus loss, retry, best-score persistence and quit;
- readable targets/cue during dense paint, music/SFX balance and intended resolution;
- repeated sessions and the planned exhibition soak.

For macOS, test the exact ZIP on at least an available Apple-silicon Mac; add an Intel Mac when practical. Fully extract it, then Control-click **BalloonBarrage.app** and choose **Open** if Gatekeeper warns about the unsigned application. Verify launch, audio, input, complete-song results, Retry and Quit. Record the macOS version and hardware. Signing/notarization is a later distribution task, not implied by a successful unsigned test.

Record all completed checks and failures in [Validation](VALIDATION.md). A fix requires a new versioned build and package.

## Publish and refresh the website

After the required platform checks pass, edit the release notes to match the evidence and publish the draft. Use a prerelease for development candidates. Publication triggers `.github/workflows/website.yml`, which rebuilds the static site from `main` and discovers the uploaded Windows/macOS assets independently.

Verify afterward:

1. The release contains both intended ZIPs and matching `.sha256` files.
2. The Chinese, Japanese and English website routes display the correct version, size and platform link.
3. macOS retains its unsigned/unnotarized warning.
4. The previous accepted package remains available as a rollback.

If a package is wrong, return the release to draft or remove the bad attachment, build a new version and republish only after acceptance. Never rewrite Git history or replace an already accepted ZIP in place.

## GitHub Actions fallback

`.github/workflows/desktop-build.yml` is a manually dispatched **Windows-only fallback**. It uses `game-ci/unity-builder`, requires `UNITY_LICENSE`, `UNITY_EMAIL` and `UNITY_PASSWORD`, runs the packager tests, and uploads a private workflow artifact. It has no publication job.

`.github/workflows/release-tools.yml` runs the focused Python packager tests when release tooling changes. This checks packaging behavior only; it does not compile or launch Unity.

The previous Linux-hosted macOS matrix path is intentionally disabled because Unity account activation returned HTTP 400 before compilation. Historical logs and the exact Windows artifact hash remain in [Validation](VALIDATION.md). Reintroduce cloud macOS building only after a verified licensing fix or a persistent licensed macOS runner; do not treat a workflow retry as a substitute for real-Mac acceptance.

## Recovery and cleanup

- Preserve the accepted ZIPs, checksums, release notes and validation record on backup storage.
- Preserve build logs until the version is accepted or abandoned.
- Keep the prior public release available during rollout.
- Remove the isolated worktree only after confirming its commit and evidence are retained; use its exact path with `git worktree remove`.
- For website-only failures, append a fix or revert commit and let Pages redeploy. Do not rebuild a game package for a static-site defect.
