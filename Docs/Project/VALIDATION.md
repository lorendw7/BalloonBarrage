# Validation record

Updated: October 12, 2026 (Asia/Tokyo). Historical detail remains in Git history; prior checks do not automatically validate new gameplay.

## Cover finish V9 and concurrent learner scene

- Unity 6000.0.77f1 compiled the editor-only art utility and the learner sources in an isolated project. The actual scene now contains the [V9 environment finish](../Art/COVER_FINISH_V9.md), six added decorative material batches and two offline area lights, with no new colliders or realtime lights.
- One CPU lighting bake succeeded: 389 active environment renderers, 99 copied lightmap-ready meshes and one directional lightmap pair. Saved lightmaps reloaded successfully. The refreshed Custom reflection, subtle plaster/vein textures, rounded joinery, sideboard and color grade were rendered and inspected. An initial plaster pattern and shadow treatment were too pronounced and were reduced before delivery; a deprecated light-type enum was corrected before the successful bake.
- The art builder verified unchanged learner-component and collider serialization and unchanged camera transform. A reload check verified finite readable mesh coordinates, no missing scene scripts, both area lights remaining Baked and four-corner muzzle framing.
- During preparation the learner saved additional RoundController, Score and scene changes. A Unity-object-ID merge preserved all six changed learner scene blocks while adding the art changes. The latest gameplay sources were copied into the test project and were not edited by this art pass. The scene and new assets were copied back only after checking for concurrent destination changes.
- Two existing acceptance coroutines passed against the merged scene: saved Start/pause/Resume/retry callbacks and timer gates; and balloon startup, collision reversal, pause/resume, duplicate-hit scoring, saved paint-effect cleanup and end-round cleanup. The Start coroutine also verified saved lightmaps in Play Mode. A later repeat of that one coroutine passed while collecting a runtime-camera capture.
- Room_GameCamera_v9 is a saved environment capture; Gameplay_Runtime_v9 includes a normally spawned target. Both exclude overlay UI. Targets_Illustrative_v9 temporarily positions and recolors three targets solely for art comparison. No screenshot establishes equivalence with the generated cover, target-PC performance, physical mouse clicks or a full graphical round. No Windows package was rebuilt.
- Static metadata/document-link audit passes with 709 unique GUIDs and eighteen documentation files. Original metadata GUIDs and attribution are preserved.

## Earlier saved pause Canvas checkpoint

- The learner reports the pause playtest passed. Saved RoundController initializes PauseScreen hidden, prevents pausing outside a round, toggles its visibility with IsPaused, and exposes ResumeRound with a playing/paused guard. PrototypeScene references the pause root and saves ResumeButton/ResumeRound and QuitButton/QuitGame callbacks to the scene controller. The old OnGUI pause drawing call is removed.
- Unity 6000.0.77f1 compiled the saved sources in an isolated copy. One Play Mode acceptance coroutine passed: saved start and pause callback targets, hidden initial pause panel, pause displaying the panel and freezing the timer, the saved Resume event restoring timeScale and hiding the panel, repeated Resume not pausing again, and retry restoring the start screen, zero score and hidden pause panel. The test invokes Button events and controller methods; physical Esc/mouse input and executing Quit are outside this automated check.
- At this checkpoint, GameplayHUD and its live Text references were still the next learner lesson. The learner has since saved a HUD root, score/time references and a LateUpdate display method in the current scene/controller. The V9 art pass preserves that saved integration; its acceptance coroutines do not assert every displayed HUD value. Canvas results and menu-input isolation remain separate follow-ups.
- PlayerShooter has its EventSystems import and waitingForFireRelease declaration but still lacks the taught release/pointer checks in Update. This remains a separate input issue; no canonical gameplay source or scene was changed by the assistant during this review. Static metadata/link audit passes with 545 GUIDs and seventeen documents.

## Earlier start Canvas review

- The learner has saved public BeginRound/QuitGame actions, the startScreen scene reference, the StartButton/BeginRound and QuitButton/QuitGame scene callbacks, and one Input System EventSystem with UI actions. The old start-menu drawing call is removed. BalloonFloat's upper-boundary line is now corrected in the canonical source.
- Unity 6000.0.77f1 compiled the current sources in an isolated copy. One Play Mode acceptance coroutine passed: positive runtime Canvas/button geometry, expected persistent callback targets/methods, assigned pointer/click actions, waiting-state spawn/score gates, Start callback invocation hiding the panel, repeated Start preserving score, pause holding the timer, resume restoring timeScale, and retry restoring the start screen and zero score. This invokes the saved Button event directly; it does not validate physical mouse clicks or execute Quit.
- The start checkpoint originally preceded the saved pause Canvas above. HUD and Canvas results remain pending, and menu-input isolation is not accepted. No canonical gameplay source or scene was changed by the assistant during that review.

## Earlier boundary review and subsequent saved correction

- The saved BalloonFloat now checks a root-position rectangle of X [-3.5, 3.5] and Y [-2.5, 2.5] after its one-time startup. The scene still spawns within X [-3, 3], Y [-2, 2], Z 8. The prefab has not yet serialized the new range fields; the script defaults were observed in the isolated boundary tests.
- The earlier review found an upper-boundary error at BalloonFloat.cs line 81: `velocity.y -= Mathf.Abs(velocity.y)` subtracts the upward velocity from itself, producing zero. The learner has since saved `velocity.y = -Mathf.Abs(velocity.y)`. No canonical gameplay file was changed by the assistant.
- Six isolated Unity 6000.0.77f1 method-driven cases against the saved code produced four passes and two failures: top-edge and top-right-corner inward velocity failed. Applying only the suggested one-line correction in the ignored test copy made all six boundary cases pass. Cases cover four edges, one corner and unchanged motion inside the rectangle; they do not prove that the entire visual mesh stays inside the rectangle.
- A corrected-copy regression run passed ten tests: six boundary cases, three existing spawn-setting tests, and one Play Mode motion/collision/pause/hit/effect/end-cleanup coroutine. The motion harness initially assumed two runner yields guaranteed physics startup; it now waits for the actual startup flag with a timeout. Those passes cover the earlier corrected test snapshot. Manual full-round play and a fresh Windows build remain pending.
- Projectile-created paint on MainWall, capped at 40 marks and cleared by retry scene reload, has been taught as an example but remains deferred while the learner integrates Canvas UI. No PaintSurface component is currently present.

## Saved balloon physics and paint-effect connection

- Reviewed the learner's saved BalloonFloat, PlayableBalloon, BalloonBounce and physics settings. BalloonFloat now assigns one random XY velocity after the round starts, allowing subsequent physics contacts to change its velocity. The float component is enabled, BalloonEntrance is disabled, gravity and kinematic mode are off, and Z position plus all rotation axes are constrained. The active capsule uses the zero-friction, 0.6-bounciness material; Bounce Threshold is 0.1.
- Unity 6000.0.77f1 compiled the current sources in an isolated project copy. One acceptance coroutine passed: no initial push before Start, initial speed of 0.6 with no Z velocity, opposing balloons reversing after contact without scoring, pause freezing position, resume restoring movement, duplicate Hit calls awarding ten points once, and EndRound removing moving balloons. The collision test assigns opposing velocities after verifying the saved random startup behavior; it does not rely on random balloons meeting naturally.
- The saved production prefab now references PaintPop_Teal. The same test verified that Hit creates that effect independently and that it is removed after unpaused cleanup time. Hit and controller methods were invoked directly; manual aiming, hardware input, GUI clicks and a natural full 90-second round remain outside this test.
- Startup speed is assigned once. Restitution can reduce speed after contacts, and the script does not keep balloons inside a designated gameplay rectangle. Spawn spacing, movement bounds and projectile-created surface paint remain pending learner work. No learner gameplay source was edited during this review and no new standalone package was built.
- Static metadata/document-link audit passes with 545 unique GUIDs and seventeen documentation files. Git whitespace checking reports Unity-generated empty YAML values in PlayableBalloon; these are formatting warnings, not gameplay failures.

## Four-wall scene and prepared paint-pop lesson

- Isolated Unity compilation and camera rendering succeeded for ExhibitionV8. MainWall, LeftWall, RightWall, BackWall and the two side returns share CreamPlaster. The new back wall is behind the fixed camera; eight decorative shell/skirting renderers add no colliders. Existing learner-component and collider serialization were checked unchanged. The existing fill light and reflection were updated.
- YAML block comparison against the previous working scene found no removed object/component IDs. Only the existing reflection probe, fill light and scene-root list changed; 34 new blocks describe the decorative group. The original learner source hashes and production balloon prefab are unchanged.
- Three prepared effect prefabs each emitted 32 particles at a simulated 0.18-second snapshot and had zero visible particles after two simulated seconds. An initial assertion incorrectly used IsAlive on editor-paused emitters; the corrected editor check verifies particle count/non-looping, and a separate Play Mode test verifies actual cleanup.
- One isolated Play Mode acceptance coroutine passed for all three variants. It temporarily assigned each prefab to a test balloon instance, invoked Hit twice, checked one ten-point award, verified particles paused/resumed with the round and that the effect root/child were destroyed after unpaused time. It also checked all six wall renderers' material assignments and the back wall's position behind the camera.
- These are method-driven tests, not manual aiming or actual GUI clicks. This earlier art checkpoint tested temporary instance assignments; the saved production connection is now verified in the physics checkpoint above. Visual playtest, bounded surface marks and a fresh Windows package remain pending; see the [lesson](../Art/PAINT_POP_LESSON.md).
- Static metadata/link audit passes with 544 unique GUIDs and seventeen documentation files. Game-camera, wall-direction and particle snapshots are saved under ExhibitionV8/Previews; no new runtime script or external/generated image was introduced.

## Interface V2 and cream/oak room finish

- An isolated Unity 6000.0.77f1 import and editor compilation completed successfully. The original generated UI sheet imported as nine named sprites, with nine-slice borders on buttons, panels and plaques. Five editable Canvas presentation prefabs and a gallery scene were saved.
- Unity rendered all five screens at 1280 × 720 and 1920 × 1080, plus the start screen at 1280 × 960. Text-fit assertions passed after shortening one instruction line that overflowed in the first 4:3 check. The start, pause, results, help and HUD layouts were visually inspected. Preview score/time values are examples.
- The V7 checkpoint moved its three wall splashes toward the edges and applied the oak floor, cream walls/columns and ivory ceiling. The floor mesh changes UVs only, with a 2.8-unit repeat; the Custom reflection cubemap was rebaked. That checkpoint's scene diff contained only three decorative transforms, seven material references, the floor mesh reference and the reflection reference.
- The room builder verified identical serialized learner components and collider settings before/after, and unchanged renderer/collider counts. Assets/Scripts has no changes from this art pass. A gameplay-camera room render was inspected; this is visual verification, not a performance benchmark.
- Static audit passes with 523 unique Unity metadata GUIDs and sixteen documentation files; git diff --check passes. Generated image originals, exact prompts, prior asset GUIDs and attribution are retained.
- New Canvas buttons have empty gameplay events. They are not installed over the existing OnGUI runtime menus. [Interface V2](../Art/INTERFACE_V2.md) teaches wiring to the learner controller and live score/time values. Runtime clicks, state transitions, translated fonts and a fresh standalone build remain unverified for these new layouts.

## Current 90-second start and pause flow

- Reviewed the learner's start/help, pause/resume, Quit, results/retry and associated gameplay gates. Corrected the waiting-state initialization from IsPlaying=true to false so the start menu blocks timer, spawning, firing and scoring. No additional gameplay feature was authored in this review.
- The current saved scene duration is 90 seconds. Current gameplay sources compile with Unity's bundled compiler and generated project references; the isolated Unity test project also compiled them successfully.
- One isolated acceptance coroutine passed: the start menu keeps the timer at 90, spawns no balloons, blocks held-fire input and score changes; BeginRound enables counting and bounded replenishment; pause freezes the timer/audio state; resume restores them; end captures the score and cleans balloons; retry resets score/time and returns to the waiting menu with no targets.
- Methods were invoked directly and mouse state simulated. This does not validate actual GUI clicks, keyboard hardware, audible music, or a natural full 90-second expiry. The learner previously reported results/retry, pause/Resume and editor Quit passed manual testing; the current Start interface still requires manual acceptance.
- Static metadata/document-link audit and all sixteen original bundled-file hashes pass. Source snapshots match the reviewed gameplay and scene. No new Windows ZIP was built during this publication; the development ZIP below is an older snapshot.

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

RoundController is attached with a saved 90-second duration. Waiting, playing, paused and finished states control the start/pause/results displays. Shooting, refill and scoring use the play/pause gates. The newest acceptance checks are described above; target-PC and full graphical-flow acceptance remain pending.

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
