# Website and desktop releases

## October 21 image-clarity refresh

The website now keeps prepared WebP images at their source resolution up to 2560 × 1440 and raises WebP quality from 85 to 92. This removes the previous 1600 × 1200 preparation ceiling for the 1920 × 1080 studio and interface previews while retaining lazy loading below the opening. The 1672 × 941 opening artwork is no longer enlarged beyond its native width, and the primary gameplay and studio figures stop growing at 1600 CSS pixels. The image dialog may use nearly the full viewport instead of stopping at 1280 pixels. Canonical PNG/JPEG files remain byte-for-byte unchanged; `Website/media/provenance.json` records every regenerated derivative and setting.

The current design update keeps held fire available for the whole round without ammunition, reload or overheat, while retaining a finite technical projectile cadence for clarity and performance. The trilingual website describes this as continuous held fire and visible paint accumulation; it does not claim a new downloadable build. Follow [Packaging desktop demos](PACKAGING_LESSON.md) for the build → acceptance → ZIP/hash → publication sequence.

## October 21 desktop download and automation refresh

The download section now presents independent Windows and macOS cards in Chinese, Japanese and English. Windows continues to resolve the established public ZIP. macOS remains an honest unavailable state until a matching real ZIP exists; when present, it carries the unsigned, unnotarized and real-hardware-testing notice. The release selector accepts only nonempty uploaded platform-labelled ZIPs from this repository's trusted GitHub Release URLs.

The offline build and all 12 site checks passed, including both platform states and the local preview/path-safety test. `.github/workflows/desktop-build.yml`, `DesktopReleaseBuild.cs` and `package_desktop_demo.py` prepare the two candidates without making physical-acceptance claims. The Actions workflow still requires the repository's Unity activation secrets before its first run. The locally built Windows `v0.1.0-demo.3` ZIP is an unplayed candidate and is not described as a public release.

## October 21 continuous-fire positioning refresh

Chinese, Japanese and English hero/gameplay/next-step copy now explains unrestricted-duration held fire, the absence of ammo/reload/overheat systems and the finite technical cadence. The public-download copy remains tied to `v0.1.0-demo.2`; this content update does not announce a new Unity package or completed playtest.

The offline build generated all three language routes and 28 project images. Ten website tests passed after local-port access was enabled, covering translation parity, the continuous-fire wording contract, release selection, output escaping, canonical/share metadata, recorded media hashes, local links/assets, content-addressed CSS/JS and preview route/path safety. Desktop visual inspection covered Chinese, Japanese and English hero/gameplay copy. These checks validate the static site only, not the Unity game, a narrow-screen layout or a live GitHub Pages deployment.

## October 20 scene and mixed-pigment previews

The trilingual studio-notes section now includes a large native Unity room preview with plaster walls, a mineral floor, side-wall paint and the updated gun. The gun card uses the latest enlarged mixed-pigment reservoir render. Copy distinguishes static marbled pigment from fluid simulation or next-projectile prediction; these previews are separate from the unchanged public `v0.1.0-demo.2` Windows package.

Canonical PNGs are `MixedPigment20261020/Previews/After.png` and `After_Model.png` under `Assets/StudioArtPack/PaintBlaster`. Their quality-92 WebP derivatives are 211,392 and 186,388 bytes, with hashes and native-render attribution in `Website/media/provenance.json`. `prepare-images.py --only` updates selected derivatives without rewriting unrelated media. Original concepts, prompts and the creator-supplied gameplay capture remain intact.

Nine website checks passed. Browser inspection at 390-pixel width found no horizontal overflow in Chinese, Japanese or English. Website publication includes these preview images and their metadata, not pending Unity gameplay, scene or model implementation.

## Distribution

The static website uses GitHub Pages; playable desktop packages use GitHub Releases. The site has Chinese, Japanese and English pages. Project documentation is English only. Windows and macOS have separate download cards and trusted asset selection; an absent macOS package is shown as unavailable rather than inferred from source archives.

The October 18 prerelease **v0.1.0-demo.2** packages the new non-Development Windows solo build with persistent four-surface paint, three-color balloons, local records, author credit and HUD formatting optimization. Its instrumented diagnostic checks and clean-package integrity checks are separate from physical acceptance; final clean-package graphical smoke testing was interrupted. The creator explicitly requested pushing it with those limits recorded. The earlier **v0.1.0-demo.1** preserves the creator-tested rollback checkpoint. Both remain noncommercial development demos, separate from final exhibition acceptance. [Roadmap](../Design/ROADMAP.md) owns scope; [Validation](VALIDATION.md) owns evidence and package hashes.

For the current package, run `Tools/Release/package_windows_demo.py` with the versioned source/output/tag and `--release-info` pointing to a JSON object containing nonempty `date`, `features`, `verification` and `limits` strings. The tool includes the author-font OFL/provenance when this metadata is supplied. `WindowsReleaseBuild.Build` requires an empty absolute `BALLOON_BUILD_OUTPUT`, builds only PrototypeScene with options None, and writes an optional `BALLOON_BUILD_REPORT`. Run it in an isolated copy; diagnostic probes must be removed for the distributed build. Keep published packages immutable and use a new version for changes.

## Build and preview

### October 20 presentation refresh

The existing illustrated opening now summarizes round length, platform and controls, with a clearer download link in navigation. The desktop palette panel uses a compact text/image layout. A trilingual Studio notes section follows downloads, showing the 48-pattern asset sheet, the enlarged paint-blaster inspection render and the refreshed start-screen creator plaque. Each image is explicitly labelled as a development asset, model or interface preview outside the current three-color demo. It does not announce a new game package or accepted exhibition build. The footer includes the original creator name in a matching small plaque.

The three previews use lazy-loaded quality-92 WebP derivatives (approximately 111, 190 and 281 KB), full-image enlargement and recorded source/output SHA-256 values and attribution in `Website/media/provenance.json`. Original image bytes, paired Unity metadata and existing image-generation prompts are retained. The gallery/model source files are included as website reference images; this website commit does not publish the unrelated pending gameplay or model implementation. Only the opening background is preloaded; below-the-fold gameplay and preview images load lazily.

Local validation uses the existing nine site checks, a current release snapshot and browser inspection of language navigation, desktop/mobile layout and the image dialog. Site checks do not validate Unity gameplay. After pushing main, verify the matching Pages workflow and the live trilingual preview/download routes. Keep the previous immutable demo ZIP and normal Git history.

Use Node.js 22 or newer; no dependency installation is required. From the repository root:

```powershell
node Website/scripts/build.mjs
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Preview at http://127.0.0.1:4173/. Add `--offline` to the build command when release lookup is unavailable. Offline/API-failure status is unknown, not proof that no release exists. Generated `Website/dist` stays ignored.

`Website/assets.json` selects canonical project artwork and prepared web derivatives. The gameplay section uses the **creator-supplied development screenshot received October 19**, with its original 1347 × 753 PNG available on enlargement. It shows multicolor wall splatters and brush marks, but its exact build and capture environment are unverified. All three languages label it as a development preview, separate from the downloadable three-color v0.1.0-demo.2 package. The source is preserved byte-for-byte as `Assets/Art/Previews/Gameplay_Development_20261019.png`, with a new paired Unity metadata file. The October 16 Editor Stats capture remains separately labelled in the expandable performance record; its readings are instantaneous observations, not sustained standalone claims. Website strings live in `Website/src/strings.mjs`, separately from game strings.

The opening retains the original warm balloon-studio menu illustration, with a centered title and demo entry point. Actual gameplay appears in its own figure in the following gameplay section. Content widths align, and downloads precede exhibition information and the compact concept gallery. Longer development/performance records remain expandable. All three languages use the same structure and real build-time release lookup, including explicit unknown/empty states.

Stylesheets and scripts now use content-addressed filenames. The user-reported oversized screenshot over the old background was reproduced by combining current markup and the earlier stylesheet. New pages request their matching CSS/JS versions, preventing that mixed-layout failure. Stable filenames remain available for older documents; newly built pages do not reference them.

The development gameplay image uses a quality-92 WebP derivative below 200 KB, preserving the entire frame and its native aspect ratio without enlargement. The original illustrated background and concept thumbnails also use WebP; concept thumbnails load lazily. Full-resolution originals, attributions and exact generation prompts remain unchanged. `python Website/scripts/prepare-images.py` regenerates the committed web derivatives with Pillow at source resolution up to 2560 × 1440; `Website/media/provenance.json` records source/output hashes and preparation settings. The October 19 capture uses format conversion and compression only, without generative editing. CI requires no imaging dependency. The image validator checks PNG, JPEG and WebP signatures. Canonical language URLs and Open Graph metadata use a dated 1200 × 675 JPEG of the development capture, fitted with minimal cream padding to retain the full HUD without cropping or distortion. Social clients may cache or ignore this metadata.

For this refresh, local build/tests and browser checks precede the main-branch Pages deployment. Verify the live first screen, three languages, image dialog and demo ZIP URL afterward. If the page, images or release entry fails, append a revert commit for this website refresh and let the same Pages workflow redeploy; do not reset or rewrite history. These checks do not accept Unity gameplay or exhibition performance.

## Publication and download selection

`.github/workflows/website.yml` builds, tests and publishes on configured main-branch paths and manual dispatch from main. Release events request a fresh main-branch run through `workflow_dispatch`; the tag job does not deploy directly. The refresh job alone has `actions: write`, and deployment retains the existing main-only Pages environment rules. This avoids a release tag being rejected by those rules without weakening environment protection. Use GitHub Actions as the Pages source. Build-time release lookup produces a public, token-free snapshot; visitors do not call the GitHub API. For an older tag that predates this workflow change, dispatch `website.yml` from main after editing or withdrawing its attachments.

Accept nonempty uploaded Windows- or macOS-labelled ZIPs from this repository's HTTPS `releases/download` URLs. Select each platform independently, reject drafts, prefer stable releases and otherwise label prereleases. Lookup examines the latest 100 releases. Source-code archives are not playable packages. Preserve the release-list link for empty or unknown states.

`.github/workflows/desktop-build.yml` prepares Windows x64 and unsigned, unnotarized macOS Universal candidates on manual dispatch. It requires the three Unity activation secrets documented in [Packaging desktop demos](PACKAGING_LESSON.md). Workflow artifacts are not public releases unless the dispatch explicitly enables prerelease publication, and neither cloud compilation nor packaging is physical acceptance.

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

## Wide-screen layout

On desktop widths above 1000 pixels, the illustrated hero fits its original 1672:941 aspect ratio with contain sizing instead of cropping into a fixed-height strip and is capped at its native 1672-pixel width. The header and content retain a 1760-pixel maximum width; primary gameplay and studio imagery is capped at 1600 CSS pixels to avoid browser upscaling. At 1600 pixels and above the base text size is 18 pixels. The existing narrow-screen layout and original source artwork remain unchanged. CSS filenames continue to change with their content.
