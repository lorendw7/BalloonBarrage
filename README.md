# BalloonBarrage

A Unity paint-blaster prototype set in a warm graffiti studio. The intended game combines unrestricted shooting, on-beat bonuses, and roguelite upgrades. Windows keyboard/mouse solo comes first; rhythm judging, waves, upgrades, and local co-op are planned.

[Website](https://lorendw7.github.io/BalloonBarrage/en/) · [Game releases](https://github.com/lorendw7/BalloonBarrage/releases) · [Development roadmap](Docs/Design/ROADMAP.md)

## Run the prototype

1. Install Git LFS, clone this repository, and run `git lfs pull`.
2. Open the repository root in Unity **6000.0.77f1**.
3. Open `Assets/Scenes/PrototypeScene.unity` and enter Play Mode.
4. Read the instructions and click Start. Aim with the mouse and hold the left button to fire. Esc pauses; Resume continues and Quit exits.

The saved scene uses a **90-second round**. Start/instructions, pause/resume, Quit, results, cleanup and score reset are implemented. Before Start, countdown, target spawning, firing and scoring are gated. Play Again reloads the scene and returns to the start menu. Automated flow checks pass; current GUI interaction still needs a manual start-menu pass. The prototype includes multiple targets, held fire, projectile collision and original music playback. Beat judging, waves and upgrades are pending. See the [validation record](Docs/Project/VALIDATION.md) for evidence and limitations.

## Music and charts

Three public StepMania song/chart packs are included under `Assets/StreamingAssets/Rhythm/StepMania` for noncommercial development packages, with original credits, source manifest and a usage notice. The current prototype still plays its original synthesized music; song selection, chart loading and rhythm judgement are not implemented. Read the [music and chart guide](Docs/Design/MUSIC_AND_CHARTS.md) for contents and restrictions.

## Documentation

All project documentation is maintained in English. The website and planned game UI have separate Chinese, Japanese, and English translations.

- [Game design](Docs/Design/GAME_DESIGN.md): scope and proposed rhythm rules.
- [Roadmap](Docs/Design/ROADMAP.md): priorities, exhibition schedule, and acceptance criteria.
- [Structure and contribution rules](Docs/Project/STRUCTURE.md): ownership and repository layout.
- [Validation](Docs/Project/VALIDATION.md): completed checks and outstanding gates.
- [Website and releases](Docs/Project/WEBSITE_AND_RELEASES.md): build, download, and packaging workflow.
- [Art direction](Docs/Art/ART_DIRECTION.md) and [asset catalog](Docs/Art/ASSET_CATALOG.md): current art and specialist guides.
- [Credits](CREDITS.md): attribution and provenance.

The creator plans an October 31, 2026 showcase as a Qpic member. The roadmap targets an October 27 candidate; booth and hours remain unconfirmed. A source archive is not a playable game. Check Releases for actual Windows packages.

## Website preview

With Node.js 22 or newer, run from the repository root:

```powershell
node Website/scripts/build.mjs --offline
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Open http://127.0.0.1:4173/. Omit `--offline` to query current GitHub release availability. Website checks do not validate the Unity game.
