# Validation record

Updated: October 16, 2026 (Asia/Tokyo). This record separates saved implementation, creator-reported playtests, automated checks and outstanding release gates. Earlier lesson-by-lesson narrative is superseded by the current evidence below; prior committed versions remain in Git history. [Roadmap](../Design/ROADMAP.md) owns delivery decisions.

## Current source checkpoint

The creator authorized publication of all saved game progress, scenes, assets, tools, documentation and website. Publishing source does not establish a playable Windows release or authorize completing unfinished learner gameplay.

| Area | Saved implementation | Creator-reported acceptance |
|---|---|---|
| Round and interface | Start, pause, Resume, Quit, HUD, captured final score, results and retry; complete-song DSP clock | Incremental pause, score/time, results/retry and editor Quit passes |
| Rhythm | 93 prepared events, 0.8-second previews, target identity, expiry at event time + 3.1 seconds, firing-time ±0.1-second judgement, matching impact bonus | Cleanup, ordinary 10 / on-beat 20 points and countdown passed |
| Hit feedback | Enlarged paint burst, golden ring/sparkles, ON BEAT +20 notification/receiver; selected duration 1.45 seconds | Burst/glow and text display/automatic hiding passed incrementally; no separate full-song acceptance at the selected duration |
| Reticle and pointer | Nonblocking overlay reticle follows mouse; play/pause/focus gate; pointer restored by disable/focus-loss helpers | Reticle following, pause hiding and pointer hiding/restoration passed |
| Shot sound and gun framing | Accepted-shot AudioSource trigger, original 175 ms puff revision 3 at volume 0.50; complete gun framed | Revised clear firing sound passed |
| Cosmetic recoil | Visual-child reference and Awake rest cache; bounded 0.035 m / -6-degree kick; rate-18 recovery; pause hold; immediate nonplaying/disable reset | Creator reports recoil and settlement/retry reset passed; lowered framing needs a new creator check |
| Controller/co-op | Configuration and preparation only | Beitong hardware preparation reported successful; no paired gameplay/co-op acceptance |

The saved recoil visual is `RefinedGunVisual`, scene Transform 2100042000. The logical muzzle, spray and audio are outside that branch. `OnDisable` resets the weapon pose and restores the pointer. LateUpdate resets immediately when IsPlaying is false, before the existing pause/recovery guards. Restart reloads the scene and recreates the cached pose. The optional `aimReticle != null` visibility guard also remains unimplemented. Diagnostic shot/chart/spawn logs and unused spawner fields remain in learner source; no gameplay cleanup was silently applied during publication.

## October 16 publication checks

- Follow-up checkpoint: the creator reports recoil and settlement/retry pose-reset playtests passed, then reported no apparent issue in the start/pause/resume spot check. Natural song-end, final-event and second-round integration are still outstanding; this feedback is not recorded as complete-song acceptance.
- At the creator's request, lowered only PrototypeScene's camera-child gun root local Y from -0.30 to -0.32. X/Z, rotation, scale, the cosmetic child and logical muzzle child pose are unchanged. A pre-write scene hash and gameplay hash snapshot guarded concurrent work; the canonical scene diff contains exactly one scalar change. Learner source was published as saved, without assistant gameplay edits.
- Unity 6000.0.77f1 compiled the latest source in the isolated copy and passed the full reference audit again. Rest-pose mesh visibility and camera-ray/muzzle sphere-cast checks passed all 27 target/aspect combinations (16:9, 16:10 and 4:3). Minimum bottom viewport margin changed from 0.0483 to 0.0281; maximum right coordinate remained 0.8748. The lower rest-pose render was visually reviewed. These geometry/render checks do not accept the new framing during physical play or measure FPS.
- The focused method-driven Play Mode check passed bounded recoil, pause hold, resumed recovery, immediate settlement/disable reset and the recreated rest pose after the saved RestartRound scene reload, retaining gun root Y -0.32. Backups, before/after captures and logs are ignored under `.local-backups/gun-lower-20261016`. Latest whole-song and standalone acceptance remain separate.
- The updated trilingual progress copy built successfully with a fresh release lookup reporting no eligible public Windows attachment; all six website tests passed. No new package or release was created.
- Unity 6000.0.77f1 compiled the synchronized saved source in the ignored isolated project. The full asset audit passed: zero missing scripts, zero unsupported Volume Profile records and zero unresolved YAML GUID references across scenes/prefabs/materials/assets.
- A method-driven Play Mode diagnostic passed the saved recoil displacement/angle, repeated-kick nonaccumulation, pause hold, recovery after resume and unchanged logical muzzle local pose. It does not accept normal physical input, end/disable reset, complete-song integration or the feel of the animation. Logs: ignored `.local-backups/publication-20261016/unity.log`.
- The four saved foundation/controller-configuration EditMode tests passed in the isolated project. These tests do not integrate P2 gameplay. Results: ignored `.local-backups/publication-20261016/editmode.xml`.
- Compilation reports two existing CS0414 warnings for unused `BalloonSpawner.remainingDelay` and `maxBalloons`. No compiler errors were found. Learner source remains unchanged by this publication pass.
- All six rhythm preprocessing tests passed: offset sign, subdivision/chord merging, chart timing overrides, unsupported timing rejection, malformed/hold/mine rejection and pinned-track coverage. Bundle verification passed all sixteen original upstream files and provenance notices.
- All six website tests passed: translation parity, download filtering/preference, safe release-state rendering, local links/canonical image copies and preview-server routing/path traversal. The first sandboxed HTTP test was blocked by localhost permissions; rerunning with localhost networking enabled passed.
- The site uses six canonical artwork/capture files and labels the October 16 room render as excluding UI/balloons. The expandable creator Game-view screenshot shows actual HUD and Editor Stats. Download availability is queried independently of source progress; no Windows attachment was created by this pass.
- Local browser checks inspected Chinese, Japanese and English pages, expanded the performance screenshot, and verified the image dialog opens/closes after repairing its assumption that every image link belongs to a figure. A 390 × 844 mobile viewport had no horizontal content overflow. The final online build reported no eligible public Windows package at lookup time.
- Repository metadata/link audit passed: 1,015 unique Unity GUIDs, no missing/orphaned metadata, and valid local links in all 25 documentation files.

## Fixed-room batching and 2K performance

The requested target is **2560 × 1440 at 120 FPS**, an 8.33 ms frame budget. `StudioRoomBatching` performs one Combine call for an explicitly prepared list of 378 fixed room MeshRenderers, with no per-frame scan. Preparation excludes the camera, gun, UI, Rigidbody/scripted ancestors, LODs, unreadable meshes, property blocks and incompatible materials. Promotion preserved all existing scene component records, transforms, colliders and lighting. No resolution, URP, shadow or camera setting was reduced. Runtime combining has startup/memory costs that still need standalone measurement.

| Creator-provided Editor Stats screenshot | Before | After |
|---|---|---|
| Render resolution / preview scale | 2560 × 1440 / 0.53 | 2560 × 1440 / 0.53 |
| FPS / frame time | 36.7 / 27.2 ms | 452.9 / 2.2 ms |
| CPU main / render thread | 27.2 / 4.5 ms | 2.2 / 1.3 ms |
| Batches / saved by batching | 1,671 / 6 | 285 / 1,495 |
| Triangles / vertices | 1.4 million / 974.7 thousand | 1.4 million / 949.1 thousand |
| SetPass / shadow casters | 72 / 851 | 78 / 847 |

These are two instantaneous creator observations, not a controlled sustained benchmark, minimum FPS, GPU timing or standalone result. The latest screenshot exceeds the requested target at capture time. The original before image is retained in ignored `.local-reference/Performance`; the unedited after image is published as `Assets/Art/Previews/CreatorStats_20261016.png`.

Independent isolated diagnostics:

- 2K edit-mode `Camera.Render` median CPU submission changed from 3.756 to 1.478 ms. Transform/collider/light preservation passed; before/after renders were visually reviewed, with SSIM 0.999922. This is not a GPU or FPS measurement.
- A controlled Play Mode comparison forced 2K camera rendering with three fixed initial targets, 60 warm-up and 240 sampled frames per phase. Batches changed 1,663 → 279; draw calls 1,667 → 391. Mean forced-camera frame time changed 3.490 → 3.216 ms; p95 4.315 → 4.296 ms. Hidden-editor timings do not establish standalone performance. An earlier unforced hidden-editor FPS result was discarded because the Game view need not render every update.
- With batching active, a method-driven synthetic accepted-shot/audio check created a projectile and produced nonzero source output peak 0.275760 at volume 0.50. Reports/renders remain in ignored `.local-backups/room-performance-20261016` and adjacent logs.

## Audio, UI and rhythm preparation evidence

- The runtime Goin' Under OGG is byte-identical to the pinned source. The prepared Beginner chart has 93 row events / 95 original lane taps; first event 9.142857 seconds, last 118.857143 seconds. Unity verified 5,443,200 samples at 44,100 Hz, stereo, about 123.4286 seconds, correct non-looping/no-autoplay 2D prefab and preloaded clip. The complete noncommercial bundle retains original notices and hashes; see [Music and charts](../Design/MUSIC_AND_CHARTS.md).
- An earlier isolated natural full-song coroutine passed menu/game switching, the 0.2-second scheduled lead, pause during lead, synchronized remaining time, continued play beyond the former 90-second cutoff, natural settlement, stopped song/resumed menu music, cleanup, captured score and a fresh origin after retry. That run predates the latest chart/reward/recoil code. It does not accept current whole-song rhythm integration, physical clicks or audible latency.
- Earlier method-driven synthetic input checks passed waiting/held-start gates, accepted firing, UI pointer blocking, held input after leaving UI and release required after resume. These are callback checks, not physical-device acceptance.
- Revision-3 shot audio is deterministic original synthesis: mono PCM16/44.1 kHz, 175 ms, -14.33 dBFS RMS, -2.16 dBFS peak, zero endpoints. SHA-256: `aad3a57f3ac7887915c3b6f5545a2145f62a6acdc4b00d5ff723795952d7be56`. The JSON preserves previous revision hashes. A four-second song/shot audition mix peaked at 0.7452 without PCM clipping; it does not cover all sound combinations. The creator's subsequent listening pass is recorded separately above.
- Gun framing and aiming checks passed 27 ray/muzzle sphere-cast paths across 16:9, 16:10 and 4:3. Visible weapon vertices remained in frame (minimum bottom margin 0.0483, maximum right coordinate 0.8748). Reticle geometry/raycast settings and actual scene references were verified. Reward text is bottom-center at (0, 168), unit scale; the beat cue is at (0, 80).
- UI clarity uses attributed Lato and CanvasTextResolutionRefresh. Static font-size checks at 720p/1080p/1440p/4K and an earlier method-driven UI/scale coroutine passed. Windows resizing and a 1920 × 1080 windowed fallback are saved. Lato does not establish Japanese/Chinese font coverage; no resolution picker is implemented.

## Render references repaired

All eleven inherited references were repaired under Unity 6000.0.77f1 / URP 17.0.4. The default Volume Profile lost only unavailable editor-template/test subassets; nineteen supported component records/settings were preserved. Seven legacy Probe Volume resource slots in PC_Renderer now resolve to installed Core-package resources. No package upgrade or gameplay change was introduced. Before/after room/gun renders showed no visible regression; the independent full-project audit reported zero missing scripts, unsupported records or unresolved GUIDs. Raw Volume Profile source is checked because its OnEnable can silently prune invalid entries.

## Art variants and previous package

V10–V13 remain separate scene studies with saved lighting, reflections and previews, documented in the [asset catalog](../Art/ASSET_CATALOG.md). V12 adds six coordinated balloon/gun themes and three balloon silhouettes; V13 adds bounded room detail and supplies. Their earlier isolated creation/reload checks preserved gameplay/physics/camera settings, verified nine aiming paths and checked material/particle behavior. These are art-scene checks, not full-song exhibition acceptance. Exact recorded generation prompts and attribution remain in the specialist guides.

The older ignored Windows development ZIP, `Builds/BalloonBarrage-Windows-x64-rhythm-dev-20261011.zip`, is a **five-second test snapshot** (71,395,088 bytes; SHA-256 `af691f5e4fb031ae3c5cd8814fe21cbc32ea367dd07a99eeb66b984b8fe4fe25`). It is not the current full-song game and was not published as a binary release. Its earlier method-driven expiry/retry checks do not validate newer source.

## Outstanding gates

- Lowered-gun visual/physical-input confirmation, complete-song natural ending/final-event/repeated-hit/cleanup/scoring regression and second-round restart. Creator-reported recoil and settlement/retry reset passes are recorded above; the latest start/pause/resume spot check was reported normal.
- Audible/input timing and signed latency calibration on the actual exhibition setup.
- Japanese instructions and font coverage; new-player readability, UI input and quit in a standalone player.
- A fresh full-song Windows package, offline fresh extraction, ten retries and a 30-minute exhibition-PC soak with CPU/GPU frame times, allocations and object/effect counts.
- Paired controller and co-op integration only after solo acceptance, according to the roadmap cutoff.
- Asset clearance for the intended distribution. Bundled StepMania material remains noncommercial development content; commercial rights are not claimed.

## Repeatable checks

Run from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File Tools/ProjectMaintenance/Verify-Project.ps1
powershell -ExecutionPolicy Bypass -File Tools/Rhythm/Verify-Bundle.ps1 -BundlePath Assets/StreamingAssets/Rhythm/StepMania
python -m unittest discover -s Tools/Rhythm -p "test_*.py"
node Website/scripts/build.mjs --offline
node --test Website/tests/site.test.mjs
```

Use Unity Test Runner for the foundation EditMode tests. `ProjectValidation.Run` checks scenes/prefabs/YAML in an isolated Unity project without saving them. Metadata/site checks never replace runtime/build acceptance. Logs and temporary verification projects stay ignored.
