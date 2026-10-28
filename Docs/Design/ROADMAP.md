# Development roadmap

Updated: October 28, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns evidence, and [README](../../README.md) is the entry point.

## Current decision

Preserve the complete-song **v0.1.0-demo.6** Windows package as the current packaged Kyudaisai fallback while preparing a separately versioned, feedback-driven candidate. Demo.6 packages commit `afdf4da`; **demo.5** remains the rollback. The candidate is not accepted merely because its plan, content or assets exist. It must pass its own source, Unity, exact-package and physical readability checks before replacing the booth default.

Recent anonymous trial feedback said the game felt carefully made, but that the two-minute play stayed mechanically similar and that the balloon-hit effect sometimes appeared offset. The creator chose this response: investigate alignment, shorten the exhibition round, add a second selectable song, author several balloon-barrage patterns and add bounded pop-effect variations. [Validation](../Project/VALIDATION.md#external-playtest-feedback-received-october-28) records the evidence boundary; [game design](GAME_DESIGN.md#kyudaisai-feedback-revision--source-candidate) owns intended behavior.

The product remains a casual, relaxing rhythm game: free aim, unrestricted-duration held fire, persistent colorful paint and an optional beat reward. [Game design](GAME_DESIGN.md) defines these rules. The creator explicitly authorized the assistant to save the Kyudaisai runtime scaffold, then reserved three short decisions for the learner in `KyudaisaiLearnerCore.cs`. Review those functions one at a time before candidate acceptance; maintainers continue to own assets, song/chart data, tools, documentation, website and checks.

The festival dates previously checked on the [official site](https://kyudaisai.jp/79th/) on October 22 are October 31–November 1, 2026. The creator plans October 31; booth, operating hours, equipment, operator coverage and November 1 participation are unconfirmed. Do not treat festival-wide hours as this project's schedule.

## Baseline and evidence boundaries

| Area | Recorded baseline | Limit / next decision |
|---|---|---|
| Desktop delivery | demo.6 Windows x64 and unsigned/unnotarized macOS Universal builds from `afdf4da`; exact ZIP hashes, manifests and notices checked; fresh Windows extraction passed a 20-second startup/player-log smoke | Startup is not a standalone complete-song playtest. No new package is required for a separate art study or website edit |
| Paint and rhythm | Eight coordinated pigments, C-key switching, continuous spray, nine prepared opaque static receivers and 32 broad-coverage mark prefabs are saved | Preserve existing timing, scoring, cadence, contact eligibility and round persistence; arbitrary curved, moving and transparent receivers remain future work |
| Automated Unity evidence | October 27 fixture passed 72 receiver/pigment contacts, pause persistence, 1,000 retained marks, natural song settlement and ten Retry resets | Evidence belongs to that fixture/revision. Re-run affected checks if production assets or references change |
| Statistics and UI | Successful/on-beat hits, current/maximum streak, existing ResultsScreen bindings, global approach fill and one shared Chinese/Japanese/English UI are saved | Focused 5-hit/2-on-beat/70-point and expiry/duplicate cases are not established by a website check; no formal Miss total, accuracy or grade is implemented |
| Performance | The October 27 2K Editor fixture measured 3.733 ms mean / 6.584 ms p95 with 1,000 marks | This is an Editor diagnostic, not guaranteed standalone 120 FPS or capacity for more objects |
| New art and website | The [October 28 studio expansion](../Art/STUDIO_EXPANSION_20261028.md) develops reusable studio assets, a separate composition study and a clearer trilingual art showcase | Label concepts, native scene previews and released gameplay separately. Use the new pack's own verification record for its status |
| Recent player feedback | Anonymous responses reported uniform pacing and an occasional apparently offset balloon-hit effect | Exact build/hardware are unknown. Treat these as prioritization evidence; reproduce alignment and validate a shorter varied candidate rather than claiming a confirmed root cause |
| Candidate source integration | Two short profiles, two runtime song buttons, chart filtering, isolated record keys, deterministic placement slots, contact-point burst handoff and eight serialized fragment prefabs are saved | Three learner functions still use safe defaults. No observed full candidate round or new package exists; segmented playback permission, physical readability, timing/listening, pattern variety and effect variation remain unaccepted |

Physical spray feel, venue-volume listening, unfamiliar-player observations and real-Mac launch were explicitly excluded from the automated release pass. They remain optional future observations, **not failed gates and not passes**. Do not reinstate them as prerequisites for completing the already authorized automated work. Exact evidence, archive hashes and historical creator reports stay in [Validation](../Project/VALIDATION.md).

## October 28–31 delivery plan

Dates are targets. A later date does not imply an earlier task passed. The creator's approximate 4–6 available hours per day favor one bounded gameplay change plus review. Do not combine duration, selection, patterns and effects into one unreviewable script rewrite.

| Target | Deliverable | Owner and completion rule |
|---|---|---|
| Oct 28 | Record feedback and save candidate scaffold | Saved: two 75–90-second windows, second tap-only song, source selection UI, timing/profile wiring, contact-point handoff, deterministic slots and eight effect references. Static Unity and preprocessing checks pass; this does not prove a playable candidate |
| Oct 29 | Learner core and one observed short round | Learner writes pattern phase selection, pattern offsets and effect mapping one function at a time. Then pass center/edge hit alignment, pause, natural settlement, selection and Retry for one profile before testing both songs |
| Oct 29–30 | Candidate playtest and authored-variety review | Complete two observed rounds per song, exercise sweep/fan/cluster and all intended effect mappings, confirm isolated records/credits, and tune only after recording the strongest readability issue |
| Oct 30 | Candidate freeze and exact-package decision | Build a new immutable version only if selected-song audio/chart/credits, round timing, patterns, effect alignment, cleanup, results/Retry, trilingual UI and exact-package startup pass. Preserve ZIPs, checksums and notices. Otherwise retain demo.6 as the booth primary |
| Oct 31 | Exhibition, if logistics are confirmed | Use the new candidate only if its gates pass; otherwise use demo.6. Follow the documented handoff/recovery path and retain observations with exact version, song/profile and hardware context |
| Nov 1 | Optional second day | Continue only if participation is confirmed; retain the same package unless a separately verified fix is needed |

No new executable, renamed replacement ZIP, feature-complete claim or performance promise follows automatically from a plan, content import or art/website push. A production-scene promotion uses a separate review: native before/after at the same camera, target/HUD visibility, unchanged gameplay references, focused Unity checks and then a new immutable package only when requested. A lighting/reflection rebake is its own measured task because it can change readability and load.

## Kyudaisai gameplay revision

Work in this order. A lower row does not start merely because its assets are ready.

| Priority | Smallest reviewable increment | Completion evidence |
|---|---|---|
| P0 — hit-effect alignment | **Source saved:** both hit paths pass sampled contact to the independent burst without changing scoring | Static call-site check passes; center/edge attachment, one-hit appearance and transient cleanup still need Play Mode observation |
| P0 — shorter session | **Source saved:** Goin' Under 82.285714 s and Springtime 84.541927 s profiles seek/filter from unchanged source audio at pitch 1 | Static cue/event/duration checks pass; audio, visible timer, natural results, pause and Retry still need observed rounds |
| P1 — three song choices | **Source saved:** Start opens a dropdown with preview; Play confirms matching clip/chart/profile/record key; Back returns to title | Current-source verification is recorded in Validation; physical listening, complete rounds, Retry and exact-package identity remain open |
| P1 — authored barrages | **Source saved:** learner chooses sweep/fan/cluster; denser scene uses bounded active count and alternate rows | All three remain readable in full-round observation; no silent event loss or per-frame random spawning |
| P1 — pop variations | **Source saved:** learner selects among eight serialized effects by color index | Visual variants respect the one-hit guard and particle budget, preserve next-target/music clarity and clean up on Retry/title |

All three pinned songs now have short profiles: Goin' Under (82.286 s / 68 events), Springtime (84.542 s / 147 events) and Mecha-Tribe Assault (80 s / 137 events). The last uses an explicit head-only balloon adaptation that omits sustain tails and mines. The updated scene uses a 16-target cap, 2.8-second spawn lead, five-second post-beat cleanup and free-row placement. Keep originals, hashes, attribution and notices; the noncommercial/segmented-playback packaging boundary remains in Music and Charts.

Candidate acceptance requires at least two observed complete rounds per song/profile, including pause/resume and Retry, plus center/edge hit checks during every pattern family. Record exact source/package and hardware. Automated checks, a website build or a startup smoke cannot replace those observations.

## Parallel work and asset acceptance

| Lane | Concrete output | Acceptance boundary |
|---|---|---|
| Scene composition | Modular edge-of-room props, framed art and a separately saved studio study | Keep the central aiming area clear; decorative props have no gameplay colliders or unnecessary realtime shadows; preserve source scene and lighting assets |
| Art production | Original text-free poster/window art, reusable native geometry/materials and selected web derivatives | Record each exact image prompt, source path, hash, intended use and restrictions; pair Unity assets with `.meta` files; do not call a reference image a model or gameplay capture |
| Official website | Responsive trilingual showcase with legible art/release distinctions | Preserve real uploaded platform downloads, keyboard access, reduced-motion support and local media provenance; run build/tests and report browser coverage accurately |
| Delivery and docs | Current roadmap, catalog links and traceable verification | One source of truth for design/plan/evidence; no duplicated release history or unverified completion claims |

Useful art is art that can be reused and evaluated. Prefer a small coherent pack over disconnected variants: a quiet large-area surface, a focal artwork and supporting modular props. Each piece needs a named intended use. Keep poster/window content unlit or appropriately materialized for its use; do not bake accidental directional shadows into reusable albedo. New props are not automatically new paint receivers.

## Immediate lesson queue

For each item, explain **one small function**, let the learner write it, then review and verify the saved result. The creator explicitly authorized the surrounding source scaffold; no other core rewrite is needed for this lesson sequence.

1. In `KyudaisaiLearnerCore.SelectBarragePattern`, split normalized round progress into three sections and return lateral sweep, shallow fan and staggered cluster.
2. In `KyudaisaiLearnerCore.CalculatePatternOffset`, map the supplied slot to normalized x and give fan/cluster distinct, bounded y behavior. Review camera readability before adding complexity.
3. In `KyudaisaiLearnerCore.SelectExplosionVariant`, return a valid palette index, initially by wrapping/clamping the balloon color index. Verify ordinary/on-beat cleanup and dense-passage visibility.
4. Run the focused static validation, then observe center/edge hits, pause, natural results, song switching and Retry. Do not package until both songs complete twice and the rights gate is resolved.

## Post-festival milestones

Start no earlier than November 2 after preserving the package and any exhibition observations. Windows solo remains the reference mode. These are ordered, reviewable milestones rather than promises of a large November feature set; select one active gameplay milestone at a time.

| Milestone | Dependency and smallest deliverable | Done when / decision |
|---|---|---|
| M0 — review and preserve | Archive release hashes, current source, known limits and any versioned observations; choose the most important player problem | One ranked backlog item has a reproduction or observation and a measurable acceptance condition; keep demo.6 recoverable |
| M1 — comfort and settings | M0; teach one audio, display or signed timing-calibration setting in the existing solo loop | Defaults preserve current behavior; value persists; pause/retry/relaunch and trilingual layout are checked. No silent timing-window/reward changes |
| M2 — song and encounter library | Complete or continue the feedback-driven candidate; stable song/chart/round/pattern/rules identities before more content | Two or three rights-checked songs load matching audio/chart/credits, complete and short profiles isolate records, and authored pattern/effect sets remain readable. Add one content item at a time |
| M3 — paint rendering study | A measured paint/object cost or a specific receiver requirement; follow [full-scene paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md) on one planar receiver in a separate scene | Correct pigment/contact/overlap, persistence through results, clean Retry and measured object/memory behavior. Compare against the current sprites before deciding whether to promote |
| M4 — explorable studio learning slice | M0 plus explicit choice to explore movement; original graybox with a short route and landmarks, then one movement/camera lesson | One small area is comfortable to traverse and supports readable aim/rhythm/paint. Compare it with fixed-camera solo; retain it as a study if it weakens the core experience |
| M5 — local co-op | Stable solo loop and selected input lesson; P1 mouse plus explicitly paired P2 gamepad, Xbox first | Independent reticles/release guards, single target resolution, shared clock and reliable pause/disconnect/reconnect. Beitong follows as a separate device check |
| M6 — broader distribution | Evidence of demand and target hardware access | Real-Mac launch, signing/notarization and any broader support have their own tested packages and stated limits; availability of a Universal ZIP alone is insufficient |

M3 and M4 are separate experiments, not prerequisites to enjoy the fixed-camera game. Do not combine movement, camera, new paint rendering, song selection and co-op into one lesson. Art/website work may continue in parallel without consuming the current gameplay milestone's acceptance time.

### Original 3D-map study

For M4, first compare several games at the level of player problem, camera, landmarks, pacing and feedback. Record an original answer for BalloonBarrage rather than importing another game's visual identity. Then build a graybox route, add one movement/camera behavior, prepare one eligible paint surface and finally test a complete-song rhythm encounter in that small area. [Game design](GAME_DESIGN.md#inspiration-and-originality-boundary) owns the originality boundary.

After one reviewable slice, decide to promote, revise or archive it. A larger map, extra target classes, roguelite upgrades and run progression remain exploratory and have no scheduled delivery commitment. Keep the casual standard-song mode available without those systems.

## Exhibit session and recovery

For an accepted short candidate, plan a 2–2.5 minute turn: short instructions, a 75–90-second song window and a brief result/paint moment. Preserve original source files and do not change playback speed/pitch; cue windows require recorded metadata and appropriate permission. If the candidate is not accepted, demo.6 remains a 3–4 minute turn with the complete 123.429-second song. Use Japanese-first instructions with the existing Chinese/Japanese/English selector; no account entry, public name leaderboard or compulsory score comparison is needed.

The [operator runbook](../Project/EXHIBITION_RUNBOOK.md) owns setup, handoff and recovery. The optional [creator playtest template](../Project/EXHIBITION_PLAYTEST.md) can record comfort, timing clarity, dense-paint readability and audio observations if performed. Preserve the tested version/hardware context instead of converting a report into general acceptance of later code.

## Change and cut rules

- Fix a reproduced reliability, contact, readability or localization blocker before adding ornament. Defer milestone flourishes, extra bob, new-record animation, extra variants and reflection work first.
- Preserve demo.6's complete-song fallback, existing rewards/window, finite hit cadence, round-persistent paint and original notices. A candidate short profile may change only its explicit start/end/event window; do not overwrite source audio, change pitch/speed or mix records across profiles. No timed fading, oldest-first removal or silent paint cap; optimize only from measured cost.
- Keep new art studies separate from production promotion. Website build, asset import, native render, Play Mode fixture and exact-package launch each prove different things.
- Use the [desktop release runbook](../Project/PACKAGING_LESSON.md) for a requested new package. Never overwrite an existing public release ZIP or remove the rollback while validating its successor.
- Preserve concurrent learner edits, GUIDs and paired `.meta` files. Keep project documentation in English, record exact image prompts and attribution, and append normal commits without rewriting history.
