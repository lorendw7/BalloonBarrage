# Website and desktop releases

The static website uses GitHub Pages; playable Windows packages use GitHub Releases. The site has Chinese, Japanese and English pages. Project documentation is English only.

## Build and preview

Use Node.js 22 or newer; no dependency installation is required. From the repository root:

```powershell
node Website/scripts/build.mjs
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Preview at http://127.0.0.1:4173/. Add `--offline` to the build command when release lookup is unavailable. Offline/API-failure status is unknown, not proof that no release exists. Generated `Website/dist` stays ignored.

`Website/assets.json` selects canonical project artwork, including the October 10 prototype camera capture. The build rejects LFS pointers and copies images into output without adding another committed source copy. Label concepts as concepts; the runtime camera capture excludes UI. Website strings live in `Website/src/strings.mjs`, separately from game strings.

## Publication and download selection

`.github/workflows/website.yml` builds, tests and publishes on configured main-branch paths, release events and manual dispatch. Use GitHub Actions as the Pages source. Build-time release lookup produces a public, token-free snapshot; visitors do not call the GitHub API.

Accept a nonempty uploaded Windows-labelled ZIP from this repository's HTTPS `releases/download` URLs. Reject drafts; prefer stable releases, otherwise label prereleases. Lookup examines the latest 100 releases. Source-code archives are not playable packages. Preserve the release-list link for empty or unknown states.

Read [GitHub Releases](https://github.com/lorendw7/BalloonBarrage/releases) for live availability; documentation is not a live release listing.

## Windows package checklist

1. Complete the [roadmap gates](../Design/ROADMAP.md), including asset clearance and target-PC playtests.
2. Build Windows x86_64 without Development Build, including the prototype scene, into ignored `Builds/Windows`.
3. Extract a fresh package outside the editor and test start, shooting, results, retry, pause, quit, audio and every advertised language/device.
4. ZIP the executable, `_Data`, UnityPlayer.dll and all other generated runtime files together. Suggested name: `BalloonBarrage-Windows-x64-v0.1.0.zip`.
5. Record SHA-256, version, controls, tested system, known issues and release status. Do not invent minimum hardware requirements.
6. Upload to Releases, use prerelease for early demos, and verify the rebuilt website links the actual asset.

Downloaded study songs are excluded from the package. See [Music and charts](../Design/MUSIC_AND_CHARTS.md) before importing third-party content.

## Recovery

Revert a faulty website commit or redeploy a known good revision. Withdraw broken game attachments and rebuild the website snapshot. Preserve normal main history. Keep an offline backup of the accepted exhibition ZIP.
