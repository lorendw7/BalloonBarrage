# Development roadmap

Updated: October 12, 2026 (Asia/Tokyo). This is the single delivery plan; [game design](GAME_DESIGN.md) defines intended behavior.

## Current baseline and next lesson

Saved implementation includes continuous aim, held fire (five shots/second), flying paint with swept collision, one-time scoring, target tracking/refill (three targets), configured respawn delay, and background music. Balloons now use random initial XY rigidbody motion, collision bounce and rectangle bounds; the learner has saved the upper-boundary correction. The old entrance animation is disabled. RoundController is attached with a saved 90-second duration; shooting, refill, and scoring check its play state.

Results, Play Again, Esc pause/resume, Resume, Quit and start/instructions are saved. The learner reports results/retry, pause-panel interaction and editor Quit passed manual testing. Review corrected one initialization value: IsPlaying is false while waiting for Start. Latest isolated checks pass for waiting-state timer/spawn/held-fire/score gates, BeginRound, pause/resume, end cleanup and retry returning to the start menu. These checks drive methods and simulated mouse state; visual Start/Resume/Quit interaction still needs manual acceptance. No beat judgement, wave director, upgrades or controller pairing is implemented.

Next, at the learner's request: integrate live score/time HUD, then Canvas results and Japanese instructions. StartScreen, PauseScreen, their Start/Resume/Quit callbacks and an Input System EventSystem are saved; method-driven start/pause/resume/retry checks pass, and the learner reports a successful pause playtest. PlayerShooter's taught release/pointer safeguards are still missing and must be accepted before menu integration is complete. Follow the [interface lessons](../Art/INTERFACE_V2.md). Projectile-created paint on MainWall, capped at 40 with retry cleanup, follows UI; the saved PaintPop_Teal burst connection is already checked. Spawn-spacing work remains deferred unless playtests show that overlap blocks the exhibition experience. Build a fresh Windows package after accepting this flow; the existing development ZIP retains its older five-second test scene.

## Exhibition schedule

The project records October 31 as the creator's intended Qpic showcase date. On October 12, this leaves 19 calendar days, with a candidate due October 27. This is a project planning assumption, not independently verified festival scheduling; booth and hours are pending. Dates below are delivery targets, not completed-feature claims.

| Dates in 2026 | Deliverable | Acceptance |
|---|---|---|
| October 11–13 | Paint-pop feedback and bounded wall paint | One burst/score per hit; effects clear; marks have a fixed cap and reset each round |
| October 14–16 | Canvas start/help, HUD, pause/results, Japanese font | Readable score/time; correct panel transitions; no menu click-through; glyphs render |
| October 17–20 | Fresh Windows build and complete solo loop | New player starts without coaching; ten retries; pause/resume/quit; offline launch |
| October 21–24 | Playtests and measured fixes | Creator arranges Qpic playtests; ten rounds and a 30-minute soak on exhibition PC; record frame times/object counts |
| October 25–27 | Candidate package | Full ZIP, version, controls, known issues, SHA-256, offline startup, correct website download |
| October 28–30 | Exhibition-PC validation and backup | Verify audio, window, input, exit and backup ZIP; fix blockers only |
| October 31 | Showcase | Use verified build and collect feedback |

Keep the existing scene and freeze broad art changes after the four-wall pass. If behind, reduce target/effect counts and optional polish while protecting results, retry, readable instructions, and stable play. If the solo package is already accepted by October 20, trial only a small original-track beat bonus with a separate regression pass; otherwise rhythm follows the showcase. Waves, upgrades, external chart import, language switching and controllers/co-op stay outside the exhibition minimum. Do not replace the engine or broaden level scope.

Budget approximately 6–8 remaining small lessons for the exhibition package: boundary correction and paint acceptance 1, bounded projectile paint 1, Canvas integration 1–2, Japanese font/instructions 1, packaging 1, playtests/fixes 1–2. Most sessions can target 45–90 minutes, but acceptance rather than lesson count controls readiness.

## Rhythm and roguelite milestones

| Order | Small increment | Exit criterion |
|---|---|---|
| 1 | Original-track clock and beat grid | Stable beat phase across the loop; explicit BPM/start/offset data; pause/retry reset together |
| 2 | Beat feedback and shot judgement | Manual taps and held shots produce explainable early/on-time/late results at different frame rates |
| 3 | Hit bonus and calibration | Judge on fire, award once on hit; distance does not shift judgement; hardware offset can be adjusted |
| 4 | Three to five waves | Bounded spawning, completion and restart without leaked objects |
| 5 | Three eligible upgrades | One-of-three selection, caps, correct reset and bounded interactions |
| 6 | Integrated demo | One track, complete run/results/retry, stable Windows package and accurate documentation |

Use the [downloaded chart references](MUSIC_AND_CHARTS.md) to study phrasing, density, tempo and offset conventions. Do not make a general-purpose importer a prerequisite for the first rhythm demo.

After the exhibition baseline, estimate another 7–11 small lessons for the first one-track rhythm/roguelite demo: clock/judgement/bonus/calibration 3–4, waves/outcomes 1–2, upgrades/selection/reset 2–3, integrated playtests/fixes 1–2. This excludes additional songs, chart import, co-op and release polish. Adjust scope after acceptance; it is not a completion-date promise.

## Working and release gates

The learner writes core gameplay; the assistant maintains tools, configuration, assets, tests, documentation and website. Teach one playable increment at a time. Publishing a saved checkpoint does not authorize implementing missing gameplay.

- Complete solo start/play/results/retry and pause/quit.
- Verify target Windows hardware, Japanese glyphs, sound, input and offline launch.
- Investigate inherited rendering-reference failures recorded in [Validation](../Project/VALIDATION.md).
- Resolve asset licenses and retain attribution before release.
- Package all runtime files and publish genuine download links through the [release workflow](../Project/WEBSITE_AND_RELEASES.md).
