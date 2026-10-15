# Development roadmap

Updated: October 16, 2026 (Asia/Tokyo). This is the single delivery plan; [game design](GAME_DESIGN.md) defines behavior and feedback tuning.

## Current baseline and next lesson

Saved implementation includes continuous aim, held fire (five shots/second), swept projectile collision and one-time scoring. The old three-target refill loop is disabled; prepared chart events now call Spawn, which has no active-count gate. Unused refill delay/count fields remain for later cleanup. Balloons use XY rigidbody motion, bounce and corrected bounds; the old entrance animation is disabled. RoundController now switches interface/game audio and derives the complete timer from a scheduled DSP origin; the fixed 90-second field is removed. Shooting, chart scheduling and scoring check its play state.

All four Canvas screens, live score/time, final score and Start/Resume/Quit/Play Again callbacks are saved. The learner reports pause, HUD and results playtests passed; isolated flow checks cover the saved transitions and retry. The new release/pointer checks are saved in PlayerShooter and the scene disables the legacy PaintGunVfx component. Solo firing-time judgement and matched-target impact rewards are now saved; the learner reports ordinary hits score 10 and on-beat hits score 20. These latest rewards have source review and user playtest evidence, not an independent Unity test. Physical input and audible output still need final exhibition acceptance. The song-clock countdown and enlarged on-beat pigment burst are saved, with learner-reported playtests passed. Golden glow and temporary ON BEAT reward Text are now connected; the learner reports glow, text display and automatic hiding passed. The creator has confirmed 1.45 seconds as the intended reward-text duration, matching saved source; pause freezes the timer and another rewarded hit restarts it. Full-song calibration, independent end-to-end regression, performance and packaging remain pending. Waves and upgrades are deferred.

The creator requires interface music separate from downloaded gameplay songs, with one complete song per round. Goin' Under / Beginner (about 123.429 seconds), an imported clip, non-looping prefab and 93 events are prepared. The reader loads events and schedules previews 0.8 seconds early on the saved song clock. Spawn initializes Balloon event id/time; active-frame cleanup retains unanswered targets until event time plus the configured delay, default 3.1 seconds. PlayerShooter compares the accepted firing time with the aimed target's original event time, then initializes the projectile with the result and target id. Impact requires a matching nonnegative id and an on-beat result for the additional 10 points. Balloon's existing guard resolves base plus bonus once. The 3.1-second cleanup delay is separate from the timing window or formal Miss statistics. The connected RhythmCue now counts toward chart-event time using a cue index separate from the preview-spawn index. The prepared [glow and temporary reward Text](../Art/INTERFACE_V2.md#on-beat-feedback-handoff) are connected with user-reported display/hide acceptance. Next validate the complete song and final event, pause near event times, repeated hits, settlement and retry; then measure audible/input timing before adding calibration code. The older local five-second Windows ZIP must be rebuilt; no public Windows release was listed when checked on October 15.

## Exhibition schedule

The [official 79th Kyudaisai site](https://kyudaisai.jp/79th/), checked October 16, lists October 31–November 1, 2026. There are 15 calendar days until opening and 11 until the October 27 internal candidate deadline as of October 16. Reserve October 28–30 for the exhibition PC and backup. The creator's booth/location remains unconfirmed. Dates below are targets, not completion claims.

| Dates in 2026 | Deliverable | Acceptance |
|---|---|---|
| October 13–14 checkpoint | Interface/game music switch and scheduled song clock | Saved implementation and isolated complete-song checks exist; physical/audio acceptance is still required |
| October 15–16 | Target event identity, preview and missed-target lifecycle | Pass the original event id/time to each target; preserve the 0.8-second preview; expire unanswered events without skipping scheduled notes; pause holds all event timing |
| October 17–19 | Shot judgement, impact reward and visible timing feedback | Accepted firing carries event/player identity and judgement; qualifying hits award once regardless of distance; timing cue uses the same song clock |
| October 20 | Solo rhythm checkpoint and first fresh Windows demo | Complete natural song, final chart event, song-end settlement, pause/resume and retry stay aligned; standalone works offline; capture the first performance baseline |
| October 21–23 | Conditional controller/co-op, Japanese instructions and early measured fixes | Accept one-gamepad input first; P1 mouse plus P2 pad only after solo passes; verify simultaneous hits, disconnect/reconnect and retry by October 23 |
| October 24–26 | Performance lesson, playtests and measured fixes | Profile a Windows build on the exhibition PC; apply measured fixes; ten rounds and a 30-minute soak; record frame times/object counts and audio/input alignment |
| October 27 | Candidate package | Full ZIP, version, controls, known issues, SHA-256, offline startup and accurate download link |
| October 28–30 | Exhibition-PC validation and backup | Verify audio, window, input, exit and backup ZIP; fix blockers only |
| October 31–November 1 | Festival | Use the verified build and collect feedback |

Freeze broad environment/art expansion now. The October 20 gate must include audible downloaded music, actual chart-driven targets/cues, firing-time judgement and hit rewards with the accepted UI loop. If timing work slips, defer cosmetics while continuing the rhythm work. Prepare controller configuration/hardware checks alongside lessons. Co-op may enter the candidate only if solo rhythm passes by October 20 and shared-screen input, simultaneous hits, pause/disconnect and retry pass by October 23; otherwise retain the accepted solo song mode. Waves, upgrades, general chart import, song selection, two-gamepad play and language switching follow the festival. Use the prepared single constant-tempo Beginner chart; a general importer is not a prerequisite.

Target identity/lifecycle, solo firing-time judgement and matched impact rewards are saved with user-reported cleanup and 10/20-point playtests. The main solo feature hookup is complete with incremental user-reported playtests; this is not a release acceptance claim. Remaining required work is the pointer-following gameplay reticle, complete-song synchronization/calibration, independent regression checks, Japanese readability, performance acceptance and packaging. Teach each core increment in small edits, then review it as one working checkpoint; a tiny snippet is not a complete feature or a promised lesson count. Use focused 45–90-minute work sessions where possible. Optional co-op adds device pairing, independent aiming/firing, shared outcomes and disconnect recovery; these must not consume the required solo acceptance window.

## Lesson order and acceptance

The next small learner lesson is a pointer-following gameplay reticle. The assistant prepares its transparent UI artwork; the learner connects its screen position and cursor state. Keep the existing mouse aim and shot judgement. Show the reticle and hide the system pointer only during active, unpaused play; restore the pointer on menus, pause, results, disable and focus loss. The Image must not intercept UI raycasts. Verify screen alignment at the supported resolutions and restore visibility across retry. A separate player-colored gamepad reticle belongs to the conditional input/co-op stage, not this presentation increment.

The following two small lessons add requested firing feedback before the complete-song regression: a short paint/air shot sound below music volume, then restrained recoil of an explicit weapon visual child. Trigger both only after an accepted shot actually spawns. Do not move the gameplay camera, aiming root, muzzle or projectile direction for cosmetic recoil. Recover toward a cached resting local pose, bound repeated kicks and reset on pause/disable/retry. Prepare licensed audio with provenance and cap simultaneous shot voices so five-shots-per-second held fire does not mask timing cues. These are requested plans, not saved implementation; complete-song acceptance and the October 20 gate still take priority if integration causes regressions.

| Order | Small increment | Exit criterion |
|---|---|---|
| 1 | Menu/game switch and scheduled song clock | Interface studio loop; Start schedules downloaded song; the same origin drives full duration, pause and settlement; retry resets it |
| 2 | Chart loading and cue on the song timeline | Prepared event times and cue use that origin; first-beat offset is explicit; no judgement before audio start |
| 3 | Prepared chart previews/targets and accepted-shot judgement | Replace random refill with bounded chart scheduling; intro has no invented notes; UI/cadence rejections are not judged |
| 4 | Carry event judgement to impact | Existing hit guard resolves once; one bonus per player per event; projectile distance does not shift judgement |
| 5 | Song-end, pause, retry and calibration | Play the complete 123.429-second song and final event; pause around an event, retry repeatedly and measure signed calibration |
| 6 | Selected feedback and Japanese readability | Apply only the approved small visual changes; demonstrate readable controls/timing and bounded effects |
| 7 | Fresh Windows build | Complete song/chart round, offline audio/input and working pause/results/retry/quit |
| 8 | Performance lesson, exhibition acceptance and candidate | Profile the standalone build, verify measured fixes, new-player tests, ten retries, 30-minute soak, versioned package and backup |

Use the [prepared downloaded chart](MUSIC_AND_CHARTS.md) for the first runtime song. The preprocessing tool validates the pinned constant-tempo tap subset; unsupported timing/note types are rejected.

Performance teaching is required before candidate packaging. Follow the [performance approach](GAME_DESIGN.md#performance-approach): measure a standalone build during a complete song first, distinguish CPU/GPU or allocation spikes, then teach one measured fix at a time and compare again. Check effect cleanup and bounded object counts across retries. Teach shadow/effect/render-cost tuning when GPU measurements justify it, and pooling only when repeated creation/destruction is a measured bottleneck. Keep 1080p/60 FPS as the exhibition target; 1440p is optional pending actual hardware measurements. Resolution compatibility checks do not prove sustained frame rate.

Run [controller hardware preparation](CONTROLLER_SETUP.md) alongside the core lessons. After solo rhythm acceptance, teach one-player gamepad reticle/fire, then the second player with explicit pairing and shared outcomes. The October 23 acceptance gate, rather than a promised two-lesson implementation, determines whether co-op ships.

The creator explicitly authorized the [concept variants V12](../Art/CONCEPT_VARIANTS_V12.md) cosmetic pass: multiple balloon colors/forms, matching particle pigment, random initial gun palettes and matching spray/flying paint are saved in the new scene. Projected/patterned surface impacts and fading remain learner follow-ups. See [game design](GAME_DESIGN.md#planned-paint-and-color-expansion) for behavior. Broad surface coverage and new cosmetic effects do not take priority over rhythm or controller acceptance.

After the festival, extend the accepted foundation with general surface painting, remaining co-op work, waves, eligible upgrades and additional songs. These require separate acceptance.

## Working and release gates

The learner writes core gameplay; the assistant maintains tools, configuration, assets, tests, documentation and website. Teach one playable increment at a time. Publishing a saved checkpoint does not authorize implementing missing gameplay.

| Owner | Work |
|---|---|
| Learner | Assets/Scripts gameplay: connect the pointer-following reticle with gameplay/menu cursor visibility, complete full-song/pause/retry verification, finish measured synchronization/calibration and optional input/pairing |
| Assistant | Prepare UI/presentation assets and Japanese instruction text, preserve scene/configuration references, maintain input-action assets and editor/build tools, review saved increments, automate relevant checks, maintain English documentation, publish the website and package an accepted candidate |
| Learner on actual hardware, assisted by checklists | Audible timing, physical Xbox/Beitong input, exhibition PC performance and new-player playtests |

Reduce switching costs: one lesson changes one small core area, then the assistant checks the saved version and updates evidence. Reuse the four accepted Canvas screens and bound chart targets through their event lifecycle; keep original music for interfaces and the prepared downloaded song for gameplay. Stop broad art revisions until the candidate is stable. No gameplay is silently added by preparation tools.

For each handoff, the learner saves the script and scene and reports the observed result. The assistant checks the saved implementation, supplies the next small edit and prepares non-gameplay work that does not depend on unfinished core logic. Hookup instructions must name the component/field and expected output. Record source review, user playtest and automated checks separately; website publication never marks a gameplay gate passed. Preserve all concurrent learner/art changes and publish only the intended website files for a website-only update.

The required exhibition package is one full song, solo mouse/keyboard aim/fire, chart previews, target expiry, visible timing feedback, firing-time rhythm bonus resolved once on impact, working title/instructions/HUD/pause/results/retry/quit, Japanese controls, and accepted offline Windows operation. Measured performance, ten retries and a 30-minute soak are release gates. Additional room polish, arbitrary-surface painting/patterns, waves, upgrades, multiple songs and in-game language switching are deferred. Cosmetic entrance/recoil/squash effects ship only if already stable and regression-free.

- Complete a solo rhythm start/play/results/retry loop and pause/quit, with accepted firing-time judgement and hit rewards.
- Verify audible cue/chart alignment, full-song end, duplicate-hit protection and synchronized reset at different target distances/frame rates.
- Verify target Windows hardware, Japanese glyphs, sound, input and offline launch.
- Investigate inherited rendering-reference failures recorded in [Validation](../Project/VALIDATION.md).
- Resolve asset licenses and retain attribution before release.
- Package all runtime files and publish genuine download links through the [release workflow](../Project/WEBSITE_AND_RELEASES.md).
