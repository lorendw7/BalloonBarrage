# Website and desktop releases

The static website uses GitHub Pages; playable Windows packages use GitHub Releases. The site has Chinese, Japanese and English pages. Project documentation is English only.

The October 16 source publication includes all saved gameplay, scenes, assets, tools, English documentation and the trilingual website authorized by the creator. It does not create or accept a new Windows release. The site separates saved implementation, creator-reported playtests, Editor performance observations and delivery targets. [Roadmap](../Design/ROADMAP.md) owns scope and deadlines; [Validation](VALIDATION.md) owns evidence.

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

`.github/workflows/website.yml` builds, tests and publishes on configured main-branch paths, release events and manual dispatch. Use GitHub Actions as the Pages source. Build-time release lookup produces a public, token-free snapshot; visitors do not call the GitHub API.

Accept a nonempty uploaded Windows-labelled ZIP from this repository's HTTPS `releases/download` URLs. Reject drafts; prefer stable releases, otherwise label prereleases. Lookup examines the latest 100 releases. Source-code archives are not playable packages. Preserve the release-list link for empty or unknown states.

Read [GitHub Releases](https://github.com/lorendw7/BalloonBarrage/releases) for live availability; documentation is not a live release listing.

## Current local Windows checkpoint

The October 16 manual build is in `Builds/Windows-x64-Solo-20261016`: run **BalloonShooter.exe** and keep **BalloonShooter_Data** plus every accompanying runtime file. The filename reflects the current Unity product name. Only PrototypeScene is enabled; four art-study scenes remain saved but excluded. A byte-verified local review ZIP was prepared on October 17 with English README/credits, excluding Burst DoNotShip diagnostics. Build/cache/file checks and recorded startup are separate from complete graphical standalone acceptance. See [Validation](VALIDATION.md#windows-package-review-october-17) for hashes, build mode and limitations. Neither the folder nor ZIP is committed, and no public Release attachment was created.

For the creator's first standalone test: start a round, fire, pause/resume, let the complete song settle, use Play Again, start a second round and quit through the game button. Report issues separately from Editor playtests. Retain original song notices and mark review packages as noncommercial development content.

## Windows package checklist

For the first manual build in Unity 6000.0.77f1, exit Play Mode, save the scene and open **File > Build Profiles**. Select Windows and activate it if needed. In the active scene list, include only `Assets/Scenes/PrototypeScene.unity` at index zero; uncheck the four art-study scenes without deleting their assets. A custom profile can use **Override Global Scene List** to keep a separate scene selection. Target Intel/AMD 64-bit. Check these settings before the build-options/output-folder lesson. See [Unity Build Profiles](https://docs.unity3d.com/6000.0/Documentation/Manual/build-profiles-reference.html) and [scene selection](https://docs.unity3d.com/6000.0/Documentation/Manual/build-profile-scene-list.html).

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
