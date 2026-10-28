# Development roadmap

Updated: October 29, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns evidence, and [README](../../README.md) is the entry point.

## Current decision

The [October 29 expansion](../Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md) prepares **v0.1.0-demo.7** with ten songs, four CC0 short rounds, three complete ParagonX9 classics under CC BY-SA 3.0 and current studio art promoted into production. Learner-owned gameplay scripts remain unchanged. [Validation](../Project/VALIDATION.md) records the actual build, package, automated runtime and publication results. Full-catalog human listening/play and exhibition hardware remain separate observations.

Preserve the complete-song **v0.1.0-demo.6** Windows package as the current packaged Kyudaisai fallback while preparing a separately versioned, feedback-driven candidate. Demo.6 packages commit `afdf4da`; **demo.5** remains the rollback. A new candidate needs source, affected Unity and exact-package checks before replacing the booth default; plan/content/assets alone are insufficient. Record physical readability separately when observed, without treating automated results as human acceptance.

The October 29 planning revision prioritizes a **60–75-second festival round, preferably about 65 seconds**, with a 90–120-second visitor turn. Start with two curated songs and make musical phrases drive the existing barrage families before introducing new target rules. Existing 80–87-second windows and complete songs remain saved as they are; new shorter profiles are not implemented by this plan. [Music-led festival variety](GAME_DESIGN.md#music-led-festival-variety--proposed) owns pacing, interactions and fairness.

Anonymous trial feedback identified mechanically uniform play and an occasional apparently offset hit effect. Address those observations through readable musical variation and contact checks. [Validation](../Project/VALIDATION.md#external-playtest-feedback-received-october-28) records the evidence boundary; more imported tracks or decorative effects alone do not establish improved play.

The product remains a casual, relaxing rhythm game: free aim, unrestricted-duration held fire, persistent colorful paint and an optional beat reward. [Game design](GAME_DESIGN.md) defines these rules. The authorized Kyudaisai runtime scaffold and the three learner decisions in `KyudaisaiLearnerCore.cs` are saved. Future gameplay increments remain learner-owned; maintainers own assets, song/chart data, tools, documentation, website and checks.

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
| Candidate source integration | Ten profiles through Start → list/preview → Play; seven 80–87-second windows, three complete classics, isolated record keys, deterministic slots, contact-point handoff and eight fragment prefabs are saved | Three learner functions are saved. Phrase-driven selection, 60–75-second profiles and new interactions remain proposals. New practice grids still need listening/calibration; use the October 29 verification for actual automated evidence |

Physical spray feel, venue-volume listening, unfamiliar-player observations and real-Mac launch were explicitly excluded from the automated release pass. They remain optional future observations, **not failed gates and not passes**. Do not reinstate them as prerequisites for completing the already authorized automated work. Exact evidence, archive hashes and historical creator reports stay in [Validation](../Project/VALIDATION.md).

## October 28–31 delivery plan

Dates are targets. A later date does not imply an earlier task passed. The creator's approximate 4–6 available hours per day favor one bounded gameplay change plus review. Do not combine duration, selection, patterns and effects into one unreviewable script rewrite.

| Target | Deliverable | Owner and completion rule |
|---|---|---|
| Oct 28–29 baseline | Preserve existing candidate and content evidence | Scaffold and the learner's three functions are saved; the working catalog now has ten profiles. Preserve concurrent work and demo.6 fallback. Use Validation for actual runtime/package results |
| Oct 29 next | Choose the exhibit pair and prepare one shorter profile | Listen to existing tracks; select a spacious and a livelier option. Prepare one phrase-bounded 60–75-second window with cue/event metadata, ending, attribution and a new profile key; prefer about 65 seconds. This is planned data work, not a claim of calibrated charts |
| Oct 29–30 optional increment | One phrase-driven barrage trial | Learner replaces elapsed-third selection with one explicit phrase lookup in a separate reviewable increment. Maintainer prepares section data and checks; reuse current patterns/rewards. If it is not clear and stable, keep existing pattern behavior |
| Oct 30 freeze | Select the exact booth package and scope | Verify the selected audio/chart/window, profile records, timing, score, alignment, capacity/cleanup, pause, results/Retry and localized UI using affected checks, then an immutable package/startup check if packaging is requested. Compare the new profile's musical readability if a creator observation is available. Freeze without adding another mechanic; unresolved new work falls back to the verified package/profile |
| Oct 31 | Exhibition, if logistics are confirmed | Use the new candidate only if its gates pass; otherwise use demo.6. Follow the documented handoff/recovery path and retain observations with exact version, song/profile and hardware context |
| Nov 1 | Optional second day | Continue only if participation is confirmed; retain the same package unless a separately verified fix is needed |

No new executable, renamed replacement ZIP, feature-complete claim or performance promise follows automatically from a plan, content import or art/website push. A production-scene promotion uses a separate review: native before/after at the same camera, target/HUD visibility, unchanged gameplay references, focused Unity checks and then a new immutable package only when requested. A lighting/reflection rebake is its own measured task because it can change readability and load.

## Kyudaisai gameplay revision

Work in this order. The source status below does not imply completed listening or human acceptance. [Game design](GAME_DESIGN.md#music-led-festival-variety--proposed) owns the interaction rules; do not copy them into a second design specification.

| Priority | Smallest reviewable increment | Completion evidence |
|---|---|---|
| P0 — hit-effect alignment | **Source saved:** both hit paths pass sampled contact to the independent burst without changing scoring | Static call-site check passes; center/edge attachment, one-hit appearance and transient cleanup still need Play Mode observation |
| P0 — shorter musical session | **Planned:** prepare one 60–75-second window, then the second exhibit choice; existing profiles stay intact | Check phrase start/end, source-to-round event conversion, credits, distinct record keys, timer/settlement, pause and Retry. Listen before calling the new window musically authored; never shorten by raising pitch/speed |
| P1 — exhibit curation | **Saved:** ten-song selector. **Planned:** two clearly identified short exhibit choices; full catalog remains standard play | Matching clip/chart/window/record and duration label; avoid presenting a 254-second classic as a short queue round. Curated UI is not yet implemented |
| P1 — phrase-driven barrages | **Saved:** sweep/fan/cluster selected by elapsed thirds. **Planned:** explicit phrase metadata selects direction/spacing and a repeated motif | One learner-owned selection increment; verify boundary times, deterministic placement, target visibility and capacity without moving judgement times or dropping events |
| P2 — target timing cue | **Planned optional trial:** associate one approach cue with the next matching target | Same clock/window/reward; observe ordinary early hits, held fire and overlap. Defer if it competes with target visibility or cannot be verified before freeze |
| Preserve — pop variations | **Source saved:** eight color-indexed fragment effects | Preserve one-hit guard, bounded feedback and Retry cleanup. Extra variants do not substitute for musical interaction |

The saved catalog and provenance are documented in [Music and charts](MUSIC_AND_CHARTS.md) and the [October 29 expansion](../Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md). Ten available tracks are a content library, not ten required exhibit choices. Prioritize one polished short profile and then a contrasting second profile. Do not compress all old events into the shorter window. Keep the saved capacity/cadence, originals, hashes, attribution and notices.

For each newly selected exhibit profile, check start, natural ending, pause/resume, Retry, score identity, event density and center/edge effects. If human comparison is performed, aim for two rounds per selected profile and record whether the phrase changes are recognizable and held fire remains comfortable. Full ten-song listening is separate work. Human observations remain optional and cannot be claimed from automated results; missing observations do not invalidate previously completed automated release work. An unclear experimental interaction stays out of the booth default.

## Parallel work and asset acceptance

| Lane | Concrete output | Acceptance boundary |
|---|---|---|
| Scene composition | Modular edge-of-room props, framed art and a separately saved studio study | Keep the central aiming area clear; decorative props have no gameplay colliders or unnecessary realtime shadows; preserve source scene and lighting assets |
| Art production | Original text-free poster/window art, reusable native geometry/materials and selected web derivatives | Record each exact image prompt, source path, hash, intended use and restrictions; pair Unity assets with `.meta` files; do not call a reference image a model or gameplay capture |
| Official website | Responsive trilingual showcase with legible art/release distinctions | Preserve real uploaded platform downloads, keyboard access, reduced-motion support and local media provenance; run build/tests and report browser coverage accurately |
| Delivery and docs | Current roadmap, catalog links and traceable verification | One source of truth for design/plan/evidence; no duplicated release history or unverified completion claims |

Useful art is art that can be reused and evaluated. Prefer a small coherent pack over disconnected variants: a quiet large-area surface, a focal artwork and supporting modular props. Each piece needs a named intended use. Keep poster/window content unlit or appropriately materialized for its use; do not bake accidental directional shadows into reusable albedo. New props are not automatically new paint receivers.

## Immediate lesson queue

The three original functions are saved; do not restart them as unfinished exercises. For new gameplay, explain **one small function**, let the learner write it, then review and verify. This planning request does not authorize an assistant-written core rewrite.

1. With the current code unchanged, listen to one candidate song and mark a coherent 60–75-second window plus its phrase changes. Maintainer can prepare derived profile/section data and provenance; preserve existing profile identities.
2. Next gameplay lesson, if selected: given a current song time and explicit section ranges, return the matching section's existing pattern, with a safe fallback. Start with one boundary; the learner writes the lookup before any caller/schema integration.
3. Review boundary behavior and pause/Retry against one clock, then integrate one phrase source in a separate learner increment. Keep the saved offset/effect mapping and scoring unchanged.
4. Verify the first short profile, then prepare the contrasting second. Freeze existing behavior if a phrase lookup or target cue remains incomplete; defer the richer interactions below.

## Post-festival milestones

Start no earlier than November 2 after preserving the package and any exhibition observations. Windows solo remains the reference mode. These are ordered, reviewable milestones rather than promises of a large November feature set; select one active gameplay milestone at a time.

| Milestone | Dependency and smallest deliverable | Done when / decision |
|---|---|---|
| M0 — review and preserve | Archive release hashes, current source, known limits and any versioned observations; choose the most important player problem | One ranked backlog item has a reproduction or observation and a measurable acceptance condition; keep demo.6 recoverable |
| M1 — comfort and settings | M0; teach one audio, display or signed timing-calibration setting in the existing solo loop | Defaults preserve current behavior; value persists; pause/retry/relaunch and trilingual layout are checked. No silent timing-window/reward changes |
| M2 — musical interaction slice | M0 plus a stable short solo profile; use the existing ten-track library before importing more. First phrase-authored patterns, then choose one richer interaction from the design | One interaction adds recognizable musical/aiming variety in a comparison round; records, cadence, timing, paint and no-failure play remain coherent. Listen/calibrate one chart at a time; imported grids alone do not satisfy this milestone |
| M3 — paint rendering study | A measured paint/object cost or a specific receiver requirement; follow [full-scene paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md) on one planar receiver in a separate scene | Correct pigment/contact/overlap, persistence through results, clean Retry and measured object/memory behavior. Compare against the current sprites before deciding whether to promote |
| M4 — explorable studio learning slice | M0 plus explicit choice to explore movement; original graybox with a short route and landmarks, then one movement/camera lesson | One small area is comfortable to traverse and supports readable aim/rhythm/paint. Compare it with fixed-camera solo; retain it as a study if it weakens the core experience |
| M5 — local co-op | Stable solo loop and selected input lesson; P1 mouse plus explicitly paired P2 gamepad, Xbox first | Independent reticles/release guards, single target resolution, shared clock and reliable pause/disconnect/reconnect. Beitong follows as a separate device check |
| M6 — broader distribution | Evidence of demand and target hardware access | Real-Mac launch, signing/notarization and any broader support have their own tested packages and stated limits; availability of a Universal ZIP alone is insufficient |

M3 and M4 are separate experiments, not prerequisites to enjoy the fixed-camera game. Do not combine movement, camera, new paint rendering, song selection and co-op into one lesson. Art/website work may continue in parallel without consuming the current gameplay milestone's acceptance time.

### Musical interaction experiments

After the festival, progress within M2 only when the previous slice is useful. Each experiment gets a learner-owned gameplay increment and a keep/revise/defer decision; none is an exhibition commitment.

| Order | Experiment and smallest slice | Evaluation |
|---|---|---|
| 1 | One listen-then-answer motif using visual demonstration markers and a repeated three-position answer | Demonstration cannot be mistaken for hittable balloons; players anticipate the answer without needing a memory test or extra button |
| 2 | One sustained phrase with a visual paint ribbon on an existing eligible planar receiver; no tracking score initially | Held spray follows comfortably; actual contact/pigment and retained marks stay correct, music/targets remain visible, and missing the guide has no penalty |
| 3 | One chorus relay and a final focal event on an already calibrated chart | Aim route remains possible at saved cadence; each event resolves once, ordinary hits still work and no extra scoring targets are fabricated |
| 4 | A quieter painted-studio ending, after the interaction itself is convincing | The result view lets players recognize their own painting. Artwork export/saving is a separate choice; no account, collectible or progression system is required |

Target cues may precede these experiments if observation shows players cannot connect the existing NEXT cue with a balloon. A scoring ribbon, new note type, chord rule or reward formula is a separate later design with versioned records. Co-op, movement and roguelite upgrades retain their own milestones rather than being bundled into this rhythm work.

### Original 3D-map study

For M4, first compare several games at the level of player problem, camera, landmarks, pacing and feedback. Record an original answer for BalloonBarrage rather than importing another game's visual identity. Then build a graybox route, add one movement/camera behavior, prepare one eligible paint surface and finally test a complete-song rhythm encounter in that small area. [Game design](GAME_DESIGN.md#inspiration-and-originality-boundary) owns the originality boundary.

After one reviewable slice, decide to promote, revise or archive it. A larger map, extra target classes, roguelite upgrades and run progression remain exploratory and have no scheduled delivery commitment. Keep the casual standard-song mode available without those systems.

## Exhibit session and recovery

For a newly verified short candidate, plan a **90–120-second visitor turn**: roughly 10–20 seconds for instructions/selection, **60–75 seconds of gameplay (prefer about 65)** and 10–20 seconds for results/paint/handoff. These are planning targets; no current profile is shortened by this document. The complete classics remain standard-play choices. Preserve source files and speed/pitch; cue windows need recorded metadata and appropriate usage rights. If the shorter profile is not ready at freeze, use the verified existing profile/package and state its actual duration; demo.6 still needs roughly a 3–4 minute turn. Use Japanese-first instructions with the Chinese/Japanese/English selector; no account, public name leaderboard or compulsory score comparison is needed.

The [operator runbook](../Project/EXHIBITION_RUNBOOK.md) owns setup, handoff and recovery. The optional [creator playtest template](../Project/EXHIBITION_PLAYTEST.md) can record comfort, timing clarity, dense-paint readability and audio observations if performed. Preserve the tested version/hardware context instead of converting a report into general acceptance of later code.

## Change and cut rules

- Fix a reproduced reliability, contact, readability or localization blocker before adding ornament. Defer milestone flourishes, extra bob, new-record animation, extra variants and reflection work first.
- Freeze the shortest coherent verified profile first. Cut the target cue and new interactions before risking song/clock/results reliability. Never force an abrupt ending or cram the old event count into a 60–75-second window; if necessary choose a different song or retain the existing fallback with its actual duration.
- Preserve demo.6's complete-song fallback, existing rewards/window, finite hit cadence, round-persistent paint and original notices. A candidate short profile may change only its explicit start/end/event window; do not overwrite source audio, change pitch/speed or mix records across profiles. No timed fading, oldest-first removal or silent paint cap; optimize only from measured cost.
- Keep new art studies separate from production promotion. Website build, asset import, native render, Play Mode fixture and exact-package launch each prove different things.
- Use the [desktop release runbook](../Project/PACKAGING_LESSON.md) for a requested new package. Never overwrite an existing public release ZIP or remove the rollback while validating its successor.
- Preserve concurrent learner edits, GUIDs and paired `.meta` files. Keep project documentation in English, record exact image prompts and attribution, and append normal commits without rewriting history.
