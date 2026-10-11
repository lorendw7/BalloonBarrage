# Validation record

Updated: October 11, 2026 (Asia/Tokyo). Historical detail remains in Git history; prior checks do not automatically validate new gameplay.

## Windows rhythm-data development snapshot

- Reviewed the saved RoundController, Score, BalloonSpawner, PlayerShooter, PaintProjectile and Balloon behavior. The learner's results/Play Again UI is present, including the corrected Final Score label. No new gameplay was authored in this packaging pass.
- Unity 6000.0.77f1 built Windows x64 successfully in an isolated copy with Development enabled. The five-second scene setting is retained. The generated package includes original songs/charts as data; it does not implement external-song playback, selection or beat judging.
- All sixteen original files match the pinned source hashes in both StreamingAssets and the freshly extracted ZIP; provenance and noncommercial notices are included.
- An isolated Unity acceptance coroutine passed: natural timer expiry, no post-end scoring, final score capture, immediate projectile deactivation, target/projectile cleanup, and ten method-driven scene reloads restoring zero score and three targets. An initial test-harness WaitForSeconds yield was unsupported by the EditMode runner; the corrected explicit frame/timeout wait passed. GUI clicks and audiovisual appearance are not covered.
- The extracted Windows executable launched headlessly and reached round end. It logged an internal temporary-allocation lifetime warning after startup; no gameplay exception was observed. Investigate in a normal graphical run before release. The smoke process was stopped after observation; clean user-driven exit was not tested.
- ZIP: `Builds/BalloonBarrage-Windows-x64-rhythm-dev-20261011.zip` (71,395,088 bytes), SHA-256 `af691f5e4fb031ae3c5cd8814fe21cbc32ea367dd07a99eeb66b984b8fe4fe25`. It remains local and ignored; no GitHub binary release has been published.
- Static audit passes with 486 unique metadata GUIDs and fifteen checked documentation files. The separate package verifier checks original bytes and required notices.

## Earlier documentation checks

- Repository static audit passed: 459 unique Unity metadata GUIDs, no missing/orphaned metadata, and valid local links across README and Docs (15 files).
- Sixteen pinned StepMania reference files initially downloaded and passed SHA-256 verification. The later user-authorized bundling pass is recorded above.
- The website online build generated three language pages from five canonical artworks and reported no eligible public Windows package at lookup time.
- All six website tests passed: translation keys, release selection, empty/failure states, local links, canonical artwork and preview HTTP routing. The first sandboxed run could not connect to localhost; rerunning with networking enabled passed.
- Documentation was consolidated into English, retaining fourteen recorded image-generation prompts, credits and unresolved issues. Gameplay changes are saved learner work; this cleanup did not author new gameplay.
- A snapshot of the saved gameplay sources compiled with Unity's bundled compiler and the project's generated assembly references. This is a source compilation check, not a Unity Test Runner run or Windows build.

## Gameplay checkpoint

RoundController is attached with a five-second test duration. Shooting, refill and scoring use its play state. Results/retry are saved and method-driven checks now pass as described above; the 90-second exhibition setting and manual interface acceptance remain pending.

## Prior evidence

The October 10 V6 record reports isolated Unity compilation/rendering and five PlayMode checks covering target refill/caps, held fire at five and two shots per second, refined assets/capture, and an actual projectile hit with one score award. Closeups and four-corner aiming were inspected. The room-reflection reload reference was fixed with an explicit Custom cubemap.

Those art checks predate the round-control changes. A Windows development build now exists, but target-PC performance and exhibition soak testing remain pending.

## Outstanding gates

- The inherited strict scan reported four unresolved GUIDs in DefaultVolumeProfile.asset and seven in PC_Renderer.asset. Investigate in Unity; successful rendering does not clear the full reference audit. Do not delete settings blindly.
- Manually verify results/retry buttons, visual cleanup and repeated play on the target PC.
- Verify Japanese start/help, pause/quit, font coverage and menu click handling.
- Measure a 30-minute exhibition-PC run and test graphics/audio/input/exit in the extracted Windows package.
- Finish existing model/font license verification before public release. The bundled songs retain upstream noncommercial restrictions; commercial distribution is not cleared.
- Rhythm, waves, upgrades, runtime language switching and controller/co-op tests await implementation.

## Repeatable checks

Run from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File Tools/ProjectMaintenance/Verify-Project.ps1
powershell -ExecutionPolicy Bypass -File Tools/Rhythm/Download-References.ps1
node Website/scripts/build.mjs --offline
node --test Website/tests/site.test.mjs
```

Use Unity Test Runner for foundation EditMode tests. ProjectValidation.Run performs deeper scene/prefab/YAML checks in an isolated Unity project; it previously failed on the inherited references. Metadata and website tests do not replace runtime/build acceptance. Keep generated logs and verification copies outside the committed tree.
