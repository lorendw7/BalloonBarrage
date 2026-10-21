# Packaging a Windows demo

This guide packages a tested BalloonBarrage build; it does not decide whether gameplay is ready. [Game design](../Design/GAME_DESIGN.md) owns behavior, the [Roadmap](../Design/ROADMAP.md) owns delivery gates, and [Validation](VALIDATION.md) records evidence.

## The four separate outcomes

1. **Build:** Unity writes a complete Windows player to a new directory.
2. **Accept:** the creator tests that exact directory on the intended PC.
3. **Package:** the release tool copies the accepted files into an immutable ZIP, adds notices and writes SHA-256.
4. **Publish:** GitHub Releases hosts the ZIP; the website reads the published release separately.

A successful website build does not accept the Unity player. A ZIP made from untested output is not a release candidate.

## Before building

- Use Unity `6000.0.77f1`, the version recorded in `ProjectSettings/ProjectVersion.txt`.
- Preserve the current working tree. For a release candidate, build from a named commit in an isolated copy so active learner edits and diagnostic probes cannot change during the build.
- Choose a new version such as `v0.1.0-demo.3`. Never overwrite an existing published ZIP or checksum.
- Create a new empty output directory outside any earlier build. Example: `Builds/Windows-x64-v0.1.0-demo.3`.
- Confirm only `Assets/Scenes/PrototypeScene.unity` is enabled for the player.
- Confirm Windows x86-64 and a non-Development build for a release candidate. Development builds are diagnostics, not final candidates.
- Confirm the bundled StepMania notice and source manifest remain unchanged. The songs are authorized only for noncommercial development packages and must not be described as commercially cleared.

## Build in the Unity editor

1. Exit Play Mode and save the intended scene.
2. Open **File > Build Profiles**.
3. Select and activate **Windows**. Install the Windows Build Support module through Unity Hub if Unity reports it missing.
4. In the active scene list, enable only `Assets/Scenes/PrototypeScene.unity` at index 0.
5. Set the architecture to Intel 64-bit / x86-64.
6. Leave **Development Build** disabled for a release candidate.
7. Click **Build** and save the player as `BalloonShooter.exe` inside the new empty versioned directory. The current packaging tool checks that exact executable name.
8. Do not rename, remove or separate the executable, `UnityPlayer.dll`, data directory or other generated runtime files.

The repository also contains `BalloonBarrage.EditorTools.WindowsReleaseBuild.Build` for isolated batch builds. Its saved version is currently pinned to `0.1.0-demo.2`, so do not use it for a new version until the learner/maintainer intentionally updates and reviews that tool. A manual candidate build avoids silently publishing the wrong embedded version.

## Accept the exact build directory

From the new build directory, test before creating the ZIP:

- launch offline;
- title/instructions and Start;
- unrestricted-duration held fire, ordinary hits and on-beat hits;
- complete-song settlement;
- pause, resume, focus loss and release-before-fire behavior;
- retry with cleared score, balloons, projectiles and paint;
- best-score persistence after relaunch;
- Quit;
- music/SFX balance and readable targets/cue during dense paint;
- intended resolution and at least the planned target-PC soak.

Record failures and the environment in [Validation](VALIDATION.md). Fixes require a new build directory and a repeated affected check. Do not patch files inside an already accepted directory.

## Create release metadata

Create an ignored local JSON file, for example `.local-reference/release-v0.1.0-demo.3.json`:

```json
{
  "date": "2026-10-27",
  "features": "Short factual list of what is saved in this build.",
  "verification": "Exact creator playtests and automated checks completed for this build.",
  "limits": "Known unverified behavior, unsupported devices and noncommercial music restriction."
}
```

Do not describe a planned feature as implemented, an automated check as a physical playtest, or an Editor screenshot as sustained player performance.

## Package without modifying runtime files

Run from the repository root, replacing the example version and paths:

```powershell
python Tools/Release/package_windows_demo.py `
  --source Builds/Windows-x64-v0.1.0-demo.3 `
  --output Builds/BalloonBarrage-Windows-x64-v0.1.0-demo.3.zip `
  --tag v0.1.0-demo.3 `
  --release-info .local-reference/release-v0.1.0-demo.3.json
```

The tool refuses to overwrite an existing ZIP or checksum, excludes `*_DoNotShip`, embeds credits/notices and a per-runtime-file manifest, verifies every archived entry, detects concurrent source-build changes and writes a `.sha256` sidecar.

Check the printed file count and digest. Then extract the ZIP into a fresh folder and repeat the short launch/start/fire/pause/results/retry/quit smoke test. The full acceptance remains attached to the original identical runtime manifest; extraction confirms the package is usable.

## Publish and update the website

1. Create the matching Git tag and GitHub prerelease/release from the accepted commit.
2. Upload the ZIP and `.sha256`; do not upload only GitHub's source archives.
3. Keep earlier accepted packages immutable as fallbacks.
4. Dispatch or observe `.github/workflows/website.yml` from `main`. The site selects a nonempty Windows-labelled ZIP from this repository's trusted GitHub Release URL.
5. Verify the live Chinese, Japanese and English routes, version, size, download URL and release notes.

If a package is wrong, withdraw the attachment, fix from a new versioned build and publish a new version. If only the site is wrong, append a website fix or revert commit and let Pages redeploy. Never rewrite repository history for release recovery.
