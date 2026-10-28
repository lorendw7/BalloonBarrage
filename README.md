# BalloonBarrage

A casual, relaxing rhythm-game prototype set in a warm graffiti studio. Aim freely, hold fire, pop balloons and leave colorful paint; timed hits earn an optional extra reward. Windows keyboard/mouse solo is the exhibition focus. The [game design](Docs/Design/GAME_DESIGN.md) defines behavior, and the [roadmap](Docs/Design/ROADMAP.md) defines delivery priorities.

[Official website](https://lorendw7.github.io/BalloonBarrage/en/) · [Desktop releases](https://github.com/lorendw7/BalloonBarrage/releases) · [Development roadmap](Docs/Design/ROADMAP.md)

## Play or open the project

The current desktop prerelease is **[v0.1.0-demo.6](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.6)**, built from `afdf4da` for Windows x64 and macOS Universal. Extract the complete platform ZIP before running **BalloonShooter.exe** or opening **BalloonBarrage.app**. The Mac candidate is unsigned and unnotarized. This is a free noncommercial development demo with the original song restrictions and notices. **[demo.5](https://github.com/lorendw7/BalloonBarrage/releases/tag/v0.1.0-demo.5)** remains the rollback. GitHub source archives are not playable packages.

To open the source:

1. Install Git LFS, clone this repository, and run `git lfs pull`.
2. Open the repository root in Unity **6000.0.77f1**.
3. Open `Assets/Scenes/PrototypeScene.unity` and enter Play Mode.
4. Read the instructions and select Start. Aim with the mouse and hold the left button to fire. Press C during a round to cycle the eight pigments. Esc pauses; Resume continues and Quit exits.

The saved prototype connects one complete-song chart, continuous spray, eight coordinated pigments, persistent marks on nine prepared opaque static receivers, score/best/statistics, and one shared Simplified Chinese/Japanese/English UI. The learner owns gameplay in `Assets/Scripts`; the assistant maintains supporting assets, tools, checks, documentation and the website. See [working agreements](AGENTS.md).

## Current work and evidence

The [October 28 studio expansion](Docs/Art/STUDIO_EXPANSION_20261028.md) adds a separate art-study scene and reusable art resources, with provenance and verification recorded in its guide. It is development material outside the immutable demo.6 packages. The official website distinguishes generated artwork, native Unity previews and released gameplay captures. The [art catalog](Docs/Art/ASSET_CATALOG.md) is the entry point for individual assets and earlier studies.

The October 27 automated Unity fixture passed 72 receiver/pigment contacts, pause persistence, 1,000 retained marks, natural complete-song settlement and ten Retry resets. demo.6 also passed compilation, exact archive/manifest/hash checks and a fresh Windows-package startup smoke. A startup smoke is not a standalone full-song playtest; Editor frame-time measurements are not guaranteed player FPS. Physical feel, venue audio, unfamiliar-player observation and real-Mac launch were excluded from that automated pass. [Validation](Docs/Project/VALIDATION.md) records the exact checks, revision boundaries and remaining limits.

The [roadmap](Docs/Design/ROADMAP.md) now covers October 28–31 delivery and dependency-gated work after the festival: comfort/settings, song selection, a measured paint-rendering study, an original explorable studio study and later controller/co-op support. New artwork and website publication do not automatically promote a scene or produce a new desktop release.

## Music and attribution

Three pinned StepMania song/chart packs are included under `Assets/StreamingAssets/Rhythm/StepMania` for authorized noncommercial development packages, with original credits, manifest and notices. PrototypeScene uses original studio music on interfaces and the separate imported Goin' Under copy during its complete 93-event Beginner chart. The songs are not represented as commercially cleared. See [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md) and [Credits](CREDITS.md).

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
