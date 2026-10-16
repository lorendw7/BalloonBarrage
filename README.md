# BalloonBarrage

A Unity paint-blaster prototype set in a warm graffiti studio. The intended game combines unrestricted shooting, on-beat bonuses, and roguelite upgrades. Windows keyboard/mouse solo comes first; solo rhythm judging is saved, while waves, upgrades, and local co-op are planned.

[Website](https://lorendw7.github.io/BalloonBarrage/en/) · [Game releases](https://github.com/lorendw7/BalloonBarrage/releases) · [Development roadmap](Docs/Design/ROADMAP.md)

## Run the prototype

1. Install Git LFS, clone this repository, and run `git lfs pull`.
2. Open the repository root in Unity **6000.0.77f1**.
3. Open `Assets/Scenes/PrototypeScene.unity` and enter Play Mode.
4. Read the instructions and click Start. Aim with the mouse and hold the left button to fire. Esc pauses; Resume continues and Quit exits.

For the latest concept-inspired studio, open `Assets/Scenes/StudioConceptDetail.unity` instead. It adds a warm mineral floor, coordinated color accents, detailed paint supplies, leaning canvases and peripheral pigment marks. It retains V12's six balloon pigments/three silhouettes, rounded gun, random initial themes and matching paint effects. See [Concept studio detail V13](Docs/Art/CONCEPT_DETAIL_V13.md) for previews, tuning and automated-check limits.

The current source checkpoint includes one complete **Goin' Under** song (about 123.429 seconds), 93 chart events, target cleanup, 10/20-point rhythm rewards and the start/pause/HUD/results/retry loop. Reticle tracking, pointer restoration, clearer shot audio and on-beat feedback have creator-reported spot playtest passes. Cosmetic recoil, smooth recovery and immediate pose reset on settlement/disable are saved, with creator-reported recoil and settlement/retry reset passes. The latest start/pause/resume spot check was reported normal; natural song-end and second-round integration remain the next checks.

Fixed-room batching is connected. The creator's latest 2560 × 1440 Editor Stats screenshot reports 452.9 FPS / 285 batches, compared with 36.7 FPS / 1,671 batches before the change. This exceeds the **2K/120 FPS target in that snapshot**; sustained whole-song and standalone performance remain unverified. No fresh Windows release is established by this source publication. Read [Validation](Docs/Project/VALIDATION.md) for evidence and [Roadmap](Docs/Design/ROADMAP.md) for the next acceptance gates.

## Music and charts

Three public StepMania song/chart packs are included under `Assets/StreamingAssets/Rhythm/StepMania` for noncommercial development packages, with original credits, manifest and notices. PrototypeScene plays original studio music on interfaces and the separate imported Goin' Under copy during a round. Its 93-event Beginner timeline drives advance spawning in the saved source; event lifecycle and solo rhythm bonuses are saved with user-reported playtests. See the validation record for independent-check limits. Read the [music and chart guide](Docs/Design/MUSIC_AND_CHARTS.md) for preparation and restrictions.

## Documentation

All project documentation is maintained in English. The website and planned game UI have separate Chinese, Japanese, and English translations.

- [Game design](Docs/Design/GAME_DESIGN.md): scope and proposed rhythm rules.
- [Roadmap](Docs/Design/ROADMAP.md): priorities, exhibition schedule, and acceptance criteria.
- [Structure and contribution rules](Docs/Project/STRUCTURE.md): ownership and repository layout.
- [Validation](Docs/Project/VALIDATION.md): completed checks and outstanding gates.
- [Website and releases](Docs/Project/WEBSITE_AND_RELEASES.md): build, download, and packaging workflow.
- [Art direction](Docs/Art/ART_DIRECTION.md) and [asset catalog](Docs/Art/ASSET_CATALOG.md): current art and specialist guides.
- [Cover finish V9](Docs/Art/COVER_FINISH_V9.md): latest actual-scene finish, saved lighting and runtime-camera preview.
- [Camera and lighting V10](Docs/Art/CAMERA_LIGHTING_V10.md): independent scene study, before/after game-camera renders, baked target probes and verification limits.
- [Material and model appearance V11](Docs/Art/MATERIAL_APPEARANCE_V11.md): reviewed balloon/floor samples, copied surface and weapon assets, raised paint, and before/after renders under the same camera and lights.
- [Concept variants V12](Docs/Art/CONCEPT_VARIANTS_V12.md): six-color/three-silhouette balloons, new concept gun, random coordinated gun/paint themes and Unity preview evidence.
- [Concept studio detail V13](Docs/Art/CONCEPT_DETAIL_V13.md): latest room, mineral floor, richer pigment/strip/rug palette, modeled studio supplies and fixed-camera comparison.
- [Interface V2](Docs/Art/INTERFACE_V2.md): new UI assets, preview gallery and lessons for wiring Start, Quit, Resume and score displays.
- [UI clarity and resolution support](Docs/Art/INTERFACE_V2.md#october-14-clarity-and-resolution-support): bundled fonts, text refresh and checked 720p/1080p/1440p, 4:3 and ultrawide layouts.
- [First rhythm lesson](Docs/Design/RHYTHM_FIRST_LESSON.md): switch interface/game music and initialize the round from the full downloaded song before adding its DSP chart timeline.
- [Chart-reading lesson](Docs/Design/RHYTHM_CHART_LESSON.md): load the prepared 93-event timeline and verify its first event before scheduling targets.
- [Controller preparation](Docs/Design/CONTROLLER_SETUP.md): prepared input actions and Xbox/Beitong checks for the planned P1 mouse plus P2 gamepad mode.
- [Paint-pop lesson](Docs/Art/PAINT_POP_LESSON.md): connected burst assets and their manual acceptance checklist; later surface paint follows the rhythm checkpoint.
- [Credits](CREDITS.md): attribution and provenance.

The [official Kyudaisai site](https://kyudaisai.jp/79th/) lists October 31–November 1, 2026. The roadmap targets an October 27 rhythm-demo candidate; the creator's booth/location remains unconfirmed. A source archive is not a playable game. Check Releases for actual Windows packages.

## Website preview

With Node.js 22 or newer, run from the repository root:

```powershell
node Website/scripts/build.mjs --offline
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Open http://127.0.0.1:4173/. Omit `--offline` to query current GitHub release availability. Website checks do not validate the Unity game.
