# Development roadmap

Updated: October 19, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior and tuning, [Validation](../Project/VALIDATION.md) owns evidence, and [README](../../README.md) is the entry point.

## Exhibition product direction

The creator prioritizes gameplay experience and effects, with approximately **4–6 hours available per day**. Build a short, satisfying paint-blaster performance: popping feels tactile, on-beat hits feel distinctly better, successive successes build anticipation, and the final score/painted studio makes another attempt appealing.

Keep the complete existing song, free aiming and held fire. The festival build should deepen this loop rather than add several shallow modes. The selected experience work is **hit statistics and forgiving hit streaks, visual-child entrance/hit animation, clearer on-beat effects/sound, and meaningful results**. These are planned, not shipped features. Gameplay remains learner-owned; planning does not authorize the assistant to implement it.

The official [79th Kyudaisai date and venue](https://kyudaisai.jp/79th/en/about/), checked October 18, lists October 31–November 1, 2026 at Kyushu University Ito Campus. The creator intends to exhibit October 31. Booth, opening hours, number of PCs and attendance on November 1 remain unconfirmed. The internal candidate target stays October 27.

## Current baseline and evidence

**v0.1.0-demo.2** is the published Windows x64 non-Development prerelease and the starting source checkpoint. v0.1.0-demo.1 remains an earlier rollback option, subject to rehearsal on the actual exhibition machine.

| Area | Current evidence | Still open |
|---|---|---|
| Complete song and solo loop | Saved 123.429-second song, 93 events, 10/20 rewards, title/pause/results/retry, pointer/audio/recoil | Physical input/listening alignment and latest clean-package core-flow acceptance on the exhibition PC |
| Painting and colors | Three-color balloons, same-color pop marks and teal projectile marks on MainWall/Floor/LeftWall/RightWall; isolated actual-collision/lifecycle checks | Human judgement of full-song coverage, target contrast and edge clipping |
| Unreleased color expansion | Eight pigments for balloons and random projectile colors, matching marks and reusable tint buffers; focused Unity checks and native palette preview | New package, complete-song readability and target-PC load |
| Records and credit | Local best score passed synthetic higher/lower/retry/relaunch checks; author credit and exact name glyphs passed native layout checks | Exhibition-machine save/relaunch and unfamiliar-player readability |
| Performance | Authorized HUD cache optimization; instrumented 2K held-fire full song passed, with 610 marks, mean 2.140 ms / p95 3.050 ms, plus frame-time spikes | Clean target-PC measurements, sustained 120 FPS, allocations and soak |
| Repeated play | Ten diagnostic reload/callback retries cleared paint and retained records | Repeated complete-song play and at least 30 minutes on actual hardware |
| Package and website | Published demo.2 ZIP/hash, original notices, verified live download | Final clean-package graphical smoke test was interrupted; fresh extraction/offline rehearsal must be completed |
| New experience features | Statistics, hit streaks, new animation/result feedback are planned | No implementation or acceptance claimed |
| Localization/co-op | Some translation/input preparation exists | Japanese exhibit copy/font coverage; controllers/co-op remain deferred |

The author-credit font covers only the exact name and ASCII. It is not a Japanese UI solution. Detailed checks and limitations live in Validation rather than being repeated here.

## Scope and cuts

| Priority | Deliverable | Boundary |
|---|---|---|
| Core baseline | Dependable complete solo song; existing scoring, persistent paint, records and controls | Preserve song/chart, 0.1-second judgement window, 3.1-second unanswered cleanup, 10/20 rewards and existing 1.45-second reward text unless a separate measured change is accepted |
| Experience priority 1 | Correct hit/on-beat/missed-event counts, forgiving hit streak and maximum streak; small live counter and clear results | Rules in [Exhibition experience design](GAME_DESIGN.md#exhibition-experience-design); no score multiplier, health, currency or permanent upgrades |
| Experience priority 2 | Visual-child entrance and short hit squash; clearer distinction between ordinary/on-beat feedback | Root Rigidbody/colliders and musical clock stay stable; no global hit stop or compulsory camera shake |
| Experience priority 3 | Tune existing pigment burst/ring/audio; one new-record/result emphasis | Reuse existing assets first, retain matching pigment, prevent overlapping feedback and loudness masking |
| Required finishing | Short Japanese instructions/buttons/result labels with verified glyphs; readable aim/timing cues; one-page booth controls | Static exhibition language is sufficient; runtime three-language switching is deferred |
| Optional, cut first | Gentle extra bob, several streak milestone flourishes, additional result decoration | Only after the core experience passes; no extension of the freeze date |
| After the festival | Armored/splitter/chain targets, song selection, upgrades, runs, controllers/co-op, screenshot export, general curved-surface paint | No implementation in the festival window |

The paint-count limit remains explicitly declined. Do not solve coverage/performance by silently deleting old marks, fading them or adding a count cap. Preserve round persistence; reduce new decorative work or choose a measured paint-preserving rendering change if necessary.

## Capacity and daily working method

October 19–24 reserves six main experience sessions, **24–36 learner hours**. October 25–26 reserves **8–12 hours** for regression and targeted fixes. October 27 is the candidate gate; October 28–30 are rehearsal/buffer days. October 18 is planning and baseline handoff, not another assumed full workday.

For a four-hour day, use roughly 2.5 hours for one small learner implementation, one hour for hands-on comparison/regression, and 30 minutes for explanation, review and a runnable checkpoint. Extra time on a six-hour day goes to tuning, observing players and fixing measured issues, not automatically adding features. Assistant asset/layout/test preparation can proceed independently; it does not consume or replace learner acceptance.

Teach one field, helper, call site or Inspector hookup at a time. Review saved code before another step. A daily deliverable may require several exchanges; there is no promised fixed lesson count or completion percentage.

## Calendar and gates

All dates are internal Asia/Tokyo targets. If a task slips, cut optional scope rather than borrowing the final test window.

| Date | Main learner outcome | Assistant preparation / evidence | Cut or exit rule |
|---|---|---|---|
| Oct 18 | Confirm experience scope and save the published baseline | Refresh this plan and design rules; retain demo.2 and demo.1 backups | No new gameplay in this planning task |
| Oct 19 | First 30 minutes: verify clean demo.2 start/fire/pause/results/retry/quit; then implement once-only hit and on-beat counts | Event-outcome fixtures, result-row assets, Japanese copy/font preparation | Counts update only on accepted impacts; new-game state resets |
| Oct 20 | Complete event outcomes and forgiving hit streak / maximum streak | Counter layout and subtle pulse; duplicate/expiry/end-of-song cases | If streak resolution is unreliable by day's end, remove streak from this candidate and retain proven hit/on-beat counts |
| Oct 21 | Add one visual-child entrance, then a short hit squash | Preserve prefab/physics references; prepare animation curve samples | Repeat contacts score once, logical collider/muzzle stay unchanged; cut squash if it destabilizes lifecycle |
| Oct 22 | Tune ordinary versus on-beat pigment/ring/sound contrast | Reuse current assets, check voice/particle overlap and recorded mix | Player can distinguish feedback while still hearing music and seeing the next target |
| Oct 23 | Integrate result counts/max streak and a new-record emphasis; connect prepared Japanese exhibit labels | Layout/glyph checks and readable instruction card | No letter-grade/accuracy percentage without definitions; no unverified glyphs |
| Oct 24 | Three unfamiliar-player trials if available; tune aiming/readability and strongest feedback | Collect observations, compare dense paint before/after, triage issues | **Freeze new features at end of day.** Optional effects are cut if the simpler version reads/plays better |
| Oct 25 | Continuous full-song/retry session on the exhibition PC | Frame-time/object/memory capture and once-only outcome checks | At least 30 minutes and ten retries followed by complete-song play; callback loops alone do not satisfy this gate |
| Oct 26 | Fix observed blockers and run full regression on the intended clean build | Fresh extraction, offline/audio/display/save checks; package candidate | No new target rules or modes; repeat affected checks after each fix |
| Oct 27 | Accept one candidate and a rehearsal-tested fallback | New immutable versioned ZIP/hash/notices, matched website text and verified download | If optional additions fail, package the latest tested reduced scope; demo.2 is not automatically accepted on a new PC |
| Oct 28 | Rehearse player handoff, sound, display, resolution and restart | Local backup/USB copy, short recovery sheet and download QR | Both main and fallback packages launch from fresh extraction |
| Oct 29 | Fix only blockers demonstrated during rehearsal | Recheck complete flow and affected performance/records | Do not overwrite published packages; use a new patch version if needed |
| Oct 30 | Final rehearsal and freeze distribution | Confirm executable/hash, equipment, cables, audio and staff instructions | No speculative polish or untested booth-time build |
| Oct 31 | Exhibit the rehearsed package and observe players | Keep recovery files available; record anonymous feedback | Staff handles retry/next-player handoff; no untested changes |
| Nov 1 | Continue only if the creator confirms participation | Maintain the same tested package and collect feedback | Official festival second day is not assumed creator attendance |

If clean demo.2 fails the opening smoke check, resolve that blocker before attaching new experience work. Keep the feature scope reduced if it cannot be repaired promptly.

## Immediate teaching queue

Rules are defined in Game design; this table owns delivery order.

| Order | Small increment | Acceptance before moving on |
|---|---|---|
| 1 | Current-round successful-hit count | Exactly one valid chart event increments on actual accepted impact; wall shots and duplicate hits do not |
| 2 | On-beat subset and mutually exclusive event outcomes | On-beat hits are part of total hits; expiry/settlement resolve each event once; no new score values |
| 3 | Streak and maximum streak | Ordinary and on-beat successes extend; unresolved-target expiry resets; missed bullets do not; pause/retry behave correctly |
| 4 | Live counter and result bindings | One clear counter, no stacked labels; totals and reset agree with event data |
| 5 | Visual-child entrance, then hit squash | Independent visual scale, once-only score/feedback, no root/collider animation or leaked delayed object |
| 6 | Tune existing ordinary/on-beat effects and sound | Distinct but non-obscuring; existing reward-text duration respected; dense/held-fire comparison passes |
| 7 | Result/new-record emphasis and exhibit copy | No misleading result percentages; record saved once at settlement; verified Japanese glyphs |
| 8 | New-player tuning, freeze, target-PC candidate | Measured stability and complete clean-package flow, then versioned release |

If a later cosmetic step can be prepared independently, the assistant may prepare its assets first. The learner's integration and acceptance still follow the queue.

## Experience playtests and release acceptance

| Question / gate | Procedure | Planning acceptance target |
|---|---|---|
| Does the first interaction make sense? | Observe three new players with the short instructions, without an oral walkthrough | Aim/fire within 30 seconds; the original musical intro is retained, and shooting can already paint during it |
| Is musical success recognizable? | Ask after a round what earned the extra reward | At least two of three can explain the on-beat bonus and distinguish its feedback; otherwise simplify cues before adding effects |
| Is there a reason to retry? | Offer another attempt without pressure; ask what they would improve | At least two can name a concrete goal such as more hits, a streak or a record; voluntary retries are evidence, not a promised conversion rate |
| Is paint still readable? | Compare the first and final dense passage and results | Targets/reticle/timing feedback remain distinguishable; no persistent flicker or floating deposits |
| Are outcomes trustworthy? | Run the exact synthetic and manual cases in Game design | Each of 93 events resolves once; counts, score and reset agree |
| Does it feel synchronized? | Compare tapping and held fire with the exhibition audio device | Listen and record signed timing observations; preserve one DSP origin; tune only from evidence |
| Is it stable? | At least 30 minutes of actual complete-song play and ten consecutive retries; include pause/focus loss | No exceptions, soft locks, ghost fire, accumulating cross-round paint/effects or unexplained record loss |
| Is it smooth? | Capture intended clean player's CPU/GPU frame times, long frames, allocations, live objects, audio voices and paint/particles | 2K/120 FPS remains a target, not an accepted guarantee; fix measured spikes or agree and label a tested fallback setting |
| Is the package ready? | Fresh extraction on the actual PC, offline launch, sound, score, pause, retry and quit | Correct immutable ZIP/hash/notices, verified download, rehearsed fallback and controls |

Do not claim all gates passed from an Editor screenshot, a website test or the instrumented 610-mark run. Ten short callback retries are useful diagnostics, not ten complete-song retries.

## Exhibit session and recovery

Plan around a **3–4 minute turn**: brief controls, one complete 123.429-second song, a short results/paint moment and an existing Retry handoff. This is a staffing estimate, not a promised queue capacity. Do not crop/speed the song or insert an upgrade menu to shorten a line.

Use a Japanese-first control card, optional English support and the author credit. List mouse aim, held fire, countdown-based rhythm bonus, Esc pause and retry. Avoid account entry and a new leaderboard/name-input flow. Keep the anonymous per-round feedback focused on fun, aiming, timing and effect clarity.

The creator confirms booth, PCs/display, power, mouse surface, audio output, venue audio rules and an operator. These unknowns do not block code/asset preparation but must be settled before rehearsal. Staff use the existing menus to pause/retry; keep a rehearsed backup ZIP and a short restart/recovery sheet locally. The earlier public build is a fallback candidate, not an automatic acceptance substitute.

## Ownership and scope control

| Work | Learner owns | Assistant owns/prepares |
|---|---|---|
| Gameplay | Event outcomes, counters/streaks, triggering animation/effects and result bindings in Assets/Scripts | Explanation, source/reference review, meaningful isolated fixtures |
| Presentation | Hookup and manual feel judgement | Visual-child prefab preparation, animation/effect assets, fonts/copy/layout and audio variants |
| Playtesting | Physical mouse/audio and unfamiliar/exhibition-PC players | Capture methods, observation sheet, issue triage and comparisons |
| Shipping | Final gameplay acceptance and choosing tested settings | Build/ZIP/hash/notices, source publication and accurate website updates when requested |

The planning request authorizes design/documentation, not silent implementation of new learner gameplay. Preserve concurrent edits, GUIDs, paired metadata, original song files, restrictions, attribution and exact image prompts. Append normal commits; do not rewrite history.

## Cut lines and escalation

- Scoring correctness, a complete song and a working retry outrank every new effect. An unreliable new streak is removed rather than patched around at the booth.
- If the October 20 outcome/streak gate fails, ship reliable counters plus the existing score and focus the remaining experience days on visual/audio feel.
- If delayed hit animation causes duplicate scores or lifecycle races, restore immediate destruction and retain a better independent burst.
- If cues or sound mask targets/music, remove optional overlays/voices, reduce decorative effect intensity and keep same-color paint readable. No full-screen flash or mandatory camera shake is required.
- If scope slips, cut extra bob/milestones/result ornament first, then new-record emphasis, then the new streak if necessary. Keep a clear hit/on-beat summary when reliable. Do not cut the Oct 25–27 verification window.
- If performance cost is measured, investigate the actual expensive pass/object class. Do not add pooling, lower resolution, delete persistent paint or rewrite rendering speculatively.
- If a new asset/song has unsuitable rights or timing, keep the current authorized noncommercial bundle and notices; do not promise commercial clearance.
- After October 24, only evidenced blocking fixes enter the candidate; cosmetic improvements wait until after the festival.

## Later milestones

| Milestone | Direction after the festival | Boundary |
|---|---|---|
| M1 | Explicitly paired one-pad solo, then keyboard/mouse P1 plus gamepad P2 | Remains deferred; no new controller deadline |
| M2 | Richer eligible paint surfaces and target presentation | Four flat receivers are saved; curved/transparent projection needs its own design and measured capacity |
| M3 | Small rights-checked song/difficulty library, expanded results and local progression | Event statistics/streak slice is pulled forward above; song selection is not |
| M4 | A short run of complete songs with a tiny pool of bounded upgrades/target rules | Upgrade choices between songs; no new festival combat system |
| M5 | Integrate the accepted subset with settings, accessibility, localization and distribution | No online services, monetization or platform port is required |

See [Game design](GAME_DESIGN.md), [music and charts](MUSIC_AND_CHARTS.md), [controller preparation](CONTROLLER_SETUP.md), [credits](../../CREDITS.md) and [website/releases](../Project/WEBSITE_AND_RELEASES.md) for their respective sources. Do not duplicate their detailed rules or treat later planning targets as released features.
