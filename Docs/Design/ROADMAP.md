# Development roadmap

Updated: October 11, 2026 (Asia/Tokyo). This is the single delivery plan; [game design](GAME_DESIGN.md) defines intended behavior.

## Current baseline and next lesson

Saved implementation includes continuous aim, held fire (five shots/second), flying paint with swept collision, one-time scoring, target tracking/refill (three targets), configured respawn delay, entrance animation, and background music. RoundController is attached with a saved 90-second duration; shooting, refill, and scoring check its play state.

Results, Play Again, Esc pause/resume, Resume, Quit and start/instructions are saved. The learner reports results/retry, pause-panel interaction and editor Quit passed manual testing. Review corrected one initialization value: IsPlaying is false while waiting for Start. Latest isolated checks pass for waiting-state timer/spawn/held-fire/score gates, BeginRound, pause/resume, end cleanup and retry returning to the start menu. These checks drive methods and simulated mouse state; visual Start/Resume/Quit interaction still needs manual acceptance. No beat judgement, wave director, upgrades or controller pairing is implemented.

Next: manually accept the start/instructions flow, then proceed directly to the original-track audio clock, beat judgement and hit bonuses, followed by waves and upgrades. Spawn-spacing work remains deferred at the learner's request. The existing Windows development ZIP predates pause/start-menu changes and retains its five-second test scene; rebuild after accepting the new flow. External song selection and chart judgement remain future lessons.

## Exhibition schedule

The creator plans to showcase the game on October 31 as a Qpic member. Booth and hours are pending. Dates below are delivery targets, not completed-feature claims.

| Dates in 2026 | Deliverable | Acceptance |
|---|---|---|
| October 11–16 | Complete round, results, retry, spawn spacing | End stops combat; no leftover targets/projectiles; score/time reset; ten clean retries |
| October 17–20 | Japanese start/help, pause/quit, first Windows build | New player can start; no menu click-through; complete a round outside Unity |
| October 21–24 | Playtests and measured fixes | Creator arranges Qpic playtests; ten rounds and a 30-minute soak on exhibition PC; record frame times/object counts |
| October 25–27 | Candidate package | Full ZIP, version, controls, known issues, SHA-256, offline startup, correct website download |
| October 28–30 | Exhibition-PC validation and backup | Verify audio, window, input, exit and backup ZIP; fix blockers only |
| October 31 | Showcase | Use verified build and collect feedback |

Keep the existing scene. If behind, reduce target/effect counts and optional polish while protecting results, retry, readable instructions, and stable play. Rhythm, waves, upgrades, language switching, controllers/co-op, and float polish follow the showcase unless the minimum build is already accepted. Do not replace the engine or broaden level scope.

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

From the accepted round/pause/quit baseline, estimate 8–12 remaining small lessons of roughly 60–90 minutes for the first one-track solo rhythm/roguelite demo: start/help 1, audio clock/judgement/bonus/calibration 3–4, waves/outcomes 1–2, upgrades/selection/reset 2–3, integrated playtests/fixes 1–2. The estimate includes the current start/help lesson and excludes additional songs, external chart import, co-op and full release polish. Adjust scope and lesson size after acceptance; this is not a completion-date promise.

## Working and release gates

The learner writes core gameplay; the assistant maintains tools, configuration, assets, tests, documentation and website. Teach one playable increment at a time. Publishing a saved checkpoint does not authorize implementing missing gameplay.

- Complete solo start/play/results/retry and pause/quit.
- Verify target Windows hardware, Japanese glyphs, sound, input and offline launch.
- Investigate inherited rendering-reference failures recorded in [Validation](../Project/VALIDATION.md).
- Resolve asset licenses and retain attribution before release.
- Package all runtime files and publish genuine download links through the [release workflow](../Project/WEBSITE_AND_RELEASES.md).
