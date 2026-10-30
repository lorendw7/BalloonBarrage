# BalloonBarrage

A casual, relaxing rhythm-game prototype set in a warm graffiti studio. Aim freely, hold fire, pop balloons and leave colorful paint; timed hits earn an optional extra reward. Windows keyboard/mouse solo is the exhibition focus. The [game design](Docs/Design/GAME_DESIGN.md) defines behavior, and the [roadmap](Docs/Design/ROADMAP.md) defines delivery priorities.

[Official website](https://lorendw7.github.io/BalloonBarrage/en/) · [Desktop releases](https://github.com/lorendw7/BalloonBarrage/releases) · [Development roadmap](Docs/Design/ROADMAP.md)

## Play or open the project

The current published desktop candidate is **[v0.1.0-demo.13](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.13)** for Windows x64 and macOS Universal. Published downloads are listed in [Desktop releases](https://github.com/lorendw7/BalloonBarrage/releases); [Validation](Docs/Project/VALIDATION.md) records each exact build/package. Extract the complete platform ZIP before running **BalloonShooter.exe** or opening **BalloonBarrage.app**. The Mac candidate is unsigned, unnotarized and untested on a real Mac. The mixed development bundle remains noncommercial because of its original StepMania content. **[demo.10](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.10)** remains available as the previous candidate; **[demo.6](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.6)** remains the complete-song fallback. Source archives are not playable packages.

To open the source:

1. Install Git LFS, clone this repository, and run `git lfs pull`.
2. Open the repository root in Unity **6000.0.77f1**.
3. Open `Assets/Scenes/PrototypeScene.unity` and enter Play Mode.
4. Read the instructions and select Start. Aim with the mouse and hold the left button to fire. Press C during a round to cycle the eight pigments. Esc pauses; Resume continues and Quit exits.

The revised scene offers Festival (ten rounds of about 64–65 seconds) and Standard (all ten complete recordings), with preview → Play → results. One opening sequence demonstrates three noninteractive outline markers before matching real balloons. Hold fire throughout or add optional timed shots; C cycles eight pigments and Esc pauses. Existing scoring, contact-aware effects, persistent paint and the learner's three functions remain. The paint ribbon is deferred. [Game design](Docs/Design/GAME_DESIGN.md) defines the experience; [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md#festival-and-complete-song-phrase-catalog) records durations, preparation and remaining listening/calibration limits.

## Current work and evidence

The [open-world planning page](https://lorendw7.github.io/BalloonBarrage/en/open-world/) introduces the future music town. Confirmed direction combines **C / A / B gameplay** with **A + C art: a sunny street music festival with handmade town details**; the night festival is deferred. This is design work only, with no new gameplay or package. [Game design](Docs/Design/GAME_DESIGN.md#open-world-direction--planning-only) owns decisions and remaining details; the [roadmap](Docs/Design/ROADMAP.md#open-world-development-track--planning-only) defines learner-led development gates.

The current source extends [mirror echo, melody trajectories and the luminous finale](Docs/Design/GAME_DESIGN.md#all-song-showcase--v4) to **all ten songs in Festival and Standard**. V4 records preserve historical scores, note times and audio. [Rainbow chain](Docs/Design/GAME_DESIGN.md#optional-rainbow-chain--source-candidate) previews nearby targets with frames, links and counts. [Local co-op/gamepads](Docs/Design/GAME_DESIGN.md#local-players-and-gamepads) include the creator's completed P2 firing and beat-bonus functions; [the completed lesson](Docs/Design/LOCAL_COOP_LESSON.md) explains them. Published demo.13 includes colored pop glows, a twelve-petal finale and a brief afterglow. Postcards/export remain deferred. [Validation](Docs/Project/VALIDATION.md) distinguishes source checks, immutable packages and physical-device acceptance.

The October 30 demo.11 candidates include an optional [tricolor relay](Docs/Design/GAME_DESIGN.md#optional-tricolor-relay): collect any three different balloon colors, celebrate and count completed sets separately from score. The creator explicitly authorized this code addition; the learner's three functions are preserved. Demo.10 remains the earlier candidate without the relay. [Validation](Docs/Project/VALIDATION.md) owns the exact checks and remaining limits.

The planned October 31 exhibition meeting place is **Room 2407, Center Building 2 (センター２号館), Ito Campus**. Demo operating hours remain unconfirmed. The [official website](https://lorendw7.github.io/BalloonBarrage/en/#festival) carries the translated visitor information.

The [October 29 open music/studio update](Docs/Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md) adds seven recordings and promotes seven current studio prefabs plus five downloaded Kenney CC0 decorations into production. The [October 28 study](Docs/Art/STUDIO_EXPANSION_20261028.md) remains available. Website media distinguishes generated artwork, Unity previews and standalone evidence. The [art catalog](Docs/Art/ASSET_CATALOG.md) links individual assets and earlier studies.

The creator authorized this festival implementation by parallel Codex agents. Derived section/placement data and a brief demonstration extend the saved rhythm loop while retaining learner-owned pattern/effect functions. Source-data tests, Unity runtime checks, native captures and exact-package checks are recorded separately in Validation.

The October 27 automated Unity fixture passed 72 receiver/pigment contacts, pause persistence, 1,000 retained marks, natural complete-song settlement and ten Retry resets. demo.6 also passed compilation, exact archive/manifest/hash checks and a fresh Windows-package startup smoke. A startup smoke is not a standalone full-song playtest; Editor frame-time measurements are not guaranteed player FPS. Physical feel, venue audio, unfamiliar-player observation and real-Mac launch were excluded from that automated pass. [Validation](Docs/Project/VALIDATION.md) records the exact checks, revision boundaries and remaining limits.

The [roadmap](Docs/Design/ROADMAP.md) tracks all-ten-song delivery, automated validation, immutable packaging and the deferred paint-ribbon experiment. [Music-led festival variety](Docs/Design/GAME_DESIGN.md#music-led-festival-variety) owns the interaction rules. Human listening, physical aiming, venue hardware and real-Mac observations remain distinct from automated checks.

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
- [Exhibition operator runbook](Docs/Project/EXHIBITION_RUNBOOK.md): offline setup, handoff and recovery.
- [Immediate lesson queue](Docs/Design/ROADMAP.md#immediate-lesson-queue): one learner-owned increment at a time.

## Website preview

With Node.js 22 or newer, run from the repository root:

```powershell
node Website/scripts/build.mjs --offline
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Open http://127.0.0.1:4173/. Omit `--offline` to query current GitHub release availability. Website checks do not validate the Unity game.
