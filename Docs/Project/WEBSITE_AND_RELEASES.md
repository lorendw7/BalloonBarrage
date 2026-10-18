# Website and desktop releases

The static website uses GitHub Pages; playable Windows packages use GitHub Releases. The site has Chinese, Japanese and English pages. Project documentation is English only.

The October 18 prerelease **v0.1.0-demo.2** packages the new non-Development Windows solo build with persistent four-surface paint, three-color balloons, local records, author credit and HUD formatting optimization. Its instrumented diagnostic checks and clean-package integrity checks are separate from physical acceptance; final clean-package graphical smoke testing was interrupted. The creator explicitly requested pushing it with those limits recorded. The earlier **v0.1.0-demo.1** preserves the creator-tested rollback checkpoint. Both remain noncommercial development demos, separate from final exhibition acceptance. [Roadmap](../Design/ROADMAP.md) owns scope; [Validation](VALIDATION.md) owns evidence and package hashes.

For the current package, run `Tools/Release/package_windows_demo.py` with the versioned source/output/tag and `--release-info` pointing to a JSON object containing nonempty `date`, `features`, `verification` and `limits` strings. The tool includes the author-font OFL/provenance when this metadata is supplied. `WindowsReleaseBuild.Build` requires an empty absolute `BALLOON_BUILD_OUTPUT`, builds only PrototypeScene with options None, and writes an optional `BALLOON_BUILD_REPORT`. Run it in an isolated copy; diagnostic probes must be removed for the distributed build. Keep published packages immutable and use a new version for changes.

## Build and preview

Use Node.js 22 or newer; no dependency installation is required. From the repository root:

```powershell
node Website/scripts/build.mjs
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Preview at http://127.0.0.1:4173/. Add `--offline` to the build command when release lookup is unavailable. Offline/API-failure status is unknown, not proof that no release exists. Generated `Website/dist` stays ignored.

`Website/assets.json` selects canonical project artwork, including the October 16 2K studio camera render and creator-provided Game-view Stats screenshot. The build rejects LFS pointers and copies images into output without adding another committed source copy. Label concepts as concepts; the studio render excludes UI and balloons, while the expandable Game-view screenshot includes the actual HUD and Stats. FPS numbers are individual Editor observations, not sustained standalone claims. Website strings live in `Website/src/strings.mjs`, separately from game strings.

## Publication and download selection

`.github/workflows/website.yml` builds, tests and publishes on configured main-branch paths and manual dispatch from main. Release events request a fresh main-branch run through `workflow_dispatch`; the tag job does not deploy directly. The refresh job alone has `actions: write`, and deployment retains the existing main-only Pages environment rules. This avoids a release tag being rejected by those rules without weakening environment protection. Use GitHub Actions as the Pages source. Build-time release lookup produces a public, token-free snapshot; visitors do not call the GitHub API. For an older tag that predates this workflow change, dispatch `website.yml` from main after editing or withdrawing its attachments.

Accept a nonempty uploaded Windows-labelled ZIP from this repository's HTTPS `releases/download` URLs. Reject drafts; prefer stable releases, otherwise label prereleases. Lookup examines the latest 100 releases. Source-code archives are not playable packages. Preserve the release-list link for empty or unknown states.

Read [GitHub Releases](https://github.com/lorendw7/BalloonBarrage/releases) for live availability; documentation is not a live release listing.

## Published Windows checkpoint

[v0.1.0-demo.1](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.1) uses the October 16 build in ignored `Builds/Windows-x64-Solo-20261016`. Run **BalloonShooter.exe** and keep **BalloonShooter_Data** plus every runtime file. Only PrototypeScene is enabled; art-study scenes are excluded. The ZIP preserves all 299 distributable runtime/source files, excluding the Burst DoNotShip directory. English controls, linked credits, a per-file runtime hash manifest, full font/model/CC0 notices and the Unity shader license accompany it. Original StepMania files/notices remain unchanged.

The creator reported the complete Windows core-flow test passed on October 17. This is physical-playtest feedback, not independent graphical automation or sustained performance acceptance. The reviewed build report includes Development; this prerelease is a development snapshot. Japanese UI, gamepad/co-op, performance capture, fresh-extraction/offline confirmation, ten retries and a 30-minute soak remain separate gates.

To reproduce packaging without rebuilding or modifying runtime files:

```powershell
python Tools/Release/package_windows_demo.py --source Builds/Windows-x64-Solo-20261016 --output Builds/BalloonBarrage-Windows-x64-v0.1.0-demo.1.zip --tag v0.1.0-demo.1
```

The tool refuses existing outputs, verifies every archive entry and checks runtime hashes against the source again after packaging. Upload the ZIP and its `.sha256` sidecar to the matching prerelease. Do not overwrite an already published artifact; use the next demo version for a changed package. Package evidence and hashes are in [Validation](VALIDATION.md#public-windows-demo-v010-demo1).

## Windows package checklist

For the first manual build in Unity 6000.0.77f1, exit Play Mode, save the scene and open **File > Build Profiles**. Select Windows and activate it if needed. In the active scene list, include only `Assets/Scenes/PrototypeScene.unity` at index zero; uncheck the four art-study scenes without deleting their assets. A custom profile can use **Override Global Scene List** to keep a separate scene selection. Target Intel/AMD 64-bit. Check these settings before the build-options/output-folder lesson. See [Unity Build Profiles](https://docs.unity3d.com/6000.0/Documentation/Manual/build-profiles-reference.html) and [scene selection](https://docs.unity3d.com/6000.0/Documentation/Manual/build-profile-scene-list.html).

For the final exhibition candidate, beyond the current development prerelease:

1. Complete the [roadmap gates](../Design/ROADMAP.md), including asset clearance and target-PC playtests.
2. Build Windows x86_64 without Development Build, including the prototype scene, into ignored `Builds/Windows`.
3. Extract a fresh package outside the editor and test start, shooting, results, retry, pause, quit, audio and every advertised language/device.
4. ZIP the executable, `_Data`, UnityPlayer.dll and all other generated runtime files together. Suggested name: `BalloonBarrage-Windows-x64-v0.1.0.zip`.
5. Record SHA-256, version, controls, tested system, known issues and release status. Do not invent minimum hardware requirements.
6. Upload to Releases, use prerelease for early demos, and verify the rebuilt website links the actual asset.

The user-authorized StepMania collection is now included through StreamingAssets. Packages containing it are noncommercial development snapshots; retain its NOTICE and upstream credits. Commercial distribution requires separate clearance. See [Music and charts](../Design/MUSIC_AND_CHARTS.md).

For a development snapshot, set `BALLOON_BUILD_OUTPUT` to an empty absolute directory and run Unity in an isolated project copy with `-batchmode -quit -executeMethod BalloonBarrage.EditorTools.WindowsDevelopmentBuild.Build`. This builds Windows x64 with Development enabled and includes an English package README and credits. It is distinct from the non-development release checklist above. Verify the output with `Tools/Rhythm/Verify-Bundle.ps1 -BundlePath <output>/BalloonBarrage_Data/StreamingAssets/Rhythm/StepMania`, then ZIP the entire output directory.

## Recovery

Revert a faulty website commit or redeploy a known good revision. Withdraw broken game attachments and rebuild the website snapshot. Preserve normal main history. Keep an offline backup of the accepted exhibition ZIP.
