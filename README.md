# BalloonBarrage

A Unity paint-blaster prototype set in a warm graffiti studio. The intended game combines unrestricted shooting, on-beat bonuses, and roguelite upgrades. Windows keyboard/mouse solo comes first; rhythm judging, waves, upgrades, and local co-op are planned.

[Website](https://lorendw7.github.io/BalloonBarrage/en/) · [Game releases](https://github.com/lorendw7/BalloonBarrage/releases) · [Development roadmap](Docs/Design/ROADMAP.md)

## Run the prototype

1. Install Git LFS, clone this repository, and run `git lfs pull`.
2. Open the repository root in Unity **6000.0.77f1**.
3. Open `Assets/Scenes/PrototypeScene.unity` and enter Play Mode.
4. Aim with the mouse and hold the left button to fire.

The saved scene uses a **five-second round for testing**. The exhibition target is 90 seconds after results and retry pass acceptance. The prototype has multiple targets, held fire, projectile collision, scoring, music playback, and a timer that gates combat. Round-end cleanup, score reset, and reload support are saved; the results/retry flow is still being completed and needs runtime acceptance. Start/pause menus and beat judging are pending. See the [validation record](Docs/Project/VALIDATION.md) for evidence and limitations.

## Music and charts

Three public StepMania song/chart packs have been collected for local study. The repository contains a pinned download manifest and checksum-verifying downloader; downloaded songs stay outside Unity assets and version control. Read the [music and chart guide](Docs/Design/MUSIC_AND_CHARTS.md) for sources, restrictions, and the proposed rhythm workflow. The current prototype uses original synthesized music.

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
