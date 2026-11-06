# BalloonBarrage

A music-and-paint game growing from a playable studio prototype toward an explorable, handmade music town. The next development focus is a small plaza → pier stage → return route. The open world is **planning only**; the published studio demo remains the playable baseline.

[Official website](https://lorendw7.github.io/BalloonBarrage/en/) · [Open-world plan](https://lorendw7.github.io/BalloonBarrage/en/open-world/) · [Desktop releases](https://github.com/lorendw7/BalloonBarrage/releases)

## Start the next development step

Read [Game design](Docs/Design/GAME_DESIGN.md#open-world-direction--planning-only) for the selected experience, then [Roadmap](Docs/Design/ROADMAP.md#immediate-lesson-queue) for the current task and acceptance questions. First prepare the route sketch, exploration/stage camera handoff and a small original asset brief. The learner writes one gameplay increment at a time; the assistant prepares assets, tools and documentation, then reviews saved code.

Keep the sunny music-town style, fixed-stage rhythm challenges and musical-layer rewards. The optional later Boss mode is direct combat against a Boss and its AI squad. Exact decisions and deferred research belong to the design and roadmap, not a second checklist here.

The November 6 cleanup removes event promotion, venue details and survey/QR entry points from the website. It does not implement an open-world scene or modify saved gameplay, historical package evidence or asset attribution.

## Play or open the project

The current published desktop candidate is **[v0.1.0-demo.17](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.17)** for Windows x64 and macOS Universal. Published downloads are listed in [Desktop releases](https://github.com/lorendw7/BalloonBarrage/releases); [Validation](Docs/Project/VALIDATION.md) records each exact build/package. Extract the complete platform ZIP before running **BalloonShooter.exe** or opening **BalloonBarrage.app**. The Mac candidate is unsigned, unnotarized and untested on a real Mac. The mixed development bundle remains noncommercial because of its original StepMania content. **[demo.16](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.16)** remains available as the previous candidate; **[demo.6](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.6)** remains the complete-song fallback. Source archives are not playable packages.

To open the source:

1. Install Git LFS, clone this repository, and run `git lfs pull`.
2. Open the repository root in Unity **6000.0.77f1**.
3. Open `Assets/Scenes/PrototypeScene.unity` and enter Play Mode.
4. Read the instructions and select Start. Aim with the mouse and hold the left button to fire. Press C during a round to cycle the eight pigments. Esc pauses; Resume continues and Quit exits.

The existing studio scene offers short sessions (labelled Festival in the saved game; ten rounds of about 64–65 seconds) and Standard (all ten complete recordings), with preview → Play → results. One opening sequence demonstrates three noninteractive outline markers before matching real balloons. Hold fire throughout or add optional timed shots; C cycles eight pigments and Esc pauses. Existing scoring, contact-aware effects, persistent paint and the learner's three functions remain. The paint ribbon is deferred. [Game design](Docs/Design/GAME_DESIGN.md) defines the experience; [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md#festival-and-complete-song-phrase-catalog) records durations, preparation and remaining listening/calibration limits.

## Current baseline and evidence

The published demo.17 addresses continuous spray across corners, curves and wall/floor boundaries. [Coverage verification](Docs/Project/VALIDATION.md#continuous-spray-corners-and-curved-coverage--october-31) records the 171 targeted checks and 1,256 full-studio GPU/physics assertions. [Paint architecture](Docs/Design/FULL_SCENE_PAINT_ARCHITECTURE.md) defines the 404 prepared receivers and bounded storage.

The saved studio includes twenty song/mode profiles, optional relay/chain rewards, local co-op/gamepads and audience feedback. The creator's two co-op learner decisions are completed; [the lesson](Docs/Design/LOCAL_COOP_LESSON.md) remains a reference, not the next unfinished assignment. [Validation](Docs/Project/VALIDATION.md) distinguishes implementation, controlled checks, immutable packages and outstanding human/device acceptance.

## Music and attribution

Three pinned StepMania packs remain included for authorized noncommercial development packages with original notices. Four MintoDog recordings are CC0 1.0; Chaoz Fantasy, Chaoz Airflow and Chaoz Impact by ParagonX9 are CC BY-SA 3.0, with original MP3s, attribution and same-license practice charts. These licenses remain separate; the complete mixed package remains noncommercial because of StepMania content. See [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md), [expansion provenance](Docs/Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md) and [Credits](CREDITS.md).

Preserve Unity GUIDs and paired `.meta` files, original asset attribution and exact recorded generation prompts. New scene props and image studies are not evidence of new gameplay integration.

## Documentation

Project documentation is in English; game and website translations are separate.

- [Game design](Docs/Design/GAME_DESIGN.md): player promise, saved behavior and proposed systems.
- [Development roadmap](Docs/Design/ROADMAP.md): current priorities, owners, milestones and acceptance boundaries.
- [Structure and contribution rules](Docs/Project/STRUCTURE.md): repository layout and ownership.
- [Validation](Docs/Project/VALIDATION.md): saved implementation, automated checks and creator reports.
- [Art direction](Docs/Art/ART_DIRECTION.md), [asset catalog](Docs/Art/ASSET_CATALOG.md) and [studio expansion](Docs/Art/STUDIO_EXPANSION_20261028.md): visual language, assets and specialist guides.
- [Localization](Docs/Art/LOCALIZATION.md) and [Interface V2](Docs/Art/INTERFACE_V2.md): shared UI, fonts, layouts and integration notes.
- [Website and releases](Docs/Project/WEBSITE_AND_RELEASES.md): architecture, release discovery and publication checks.
- [Desktop release runbook](Docs/Project/PACKAGING_LESSON.md): immutable build/package workflow.
- [Immediate lesson queue](Docs/Design/ROADMAP.md#immediate-lesson-queue): one learner-owned increment at a time.

## Website preview

With Node.js 22 or newer, run from the repository root:

```powershell
node Website/scripts/build.mjs --offline
node --test Website/tests/*.test.mjs
node Website/scripts/serve.mjs
```

Open http://127.0.0.1:4173/. Omit `--offline` to query current GitHub release availability. Website checks do not validate the Unity game.
