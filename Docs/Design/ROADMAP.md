# Development roadmap

Updated: October 28, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns evidence, and [README](../../README.md) is the entry point.

## Current decision

Keep the complete-song Windows solo experience stable through the exhibition. The October 27 automated release scope is complete: **v0.1.0-demo.6** packages commit `afdf4da`; **demo.5** remains the rollback. New scene studies, reusable art and website improvements can advance alongside that immutable package. Adding art to the repository or publishing the website does not update either desktop ZIP.

The product remains a casual, relaxing rhythm game: free aim, unrestricted-duration held fire, persistent colorful paint and an optional beat reward. [Game design](GAME_DESIGN.md) defines these rules. The learner owns core gameplay in `Assets/Scripts`; maintainers prepare assets, tools, documentation, website and checks. New movement, target rules, scoring, controllers or mode systems require separate learner increments.

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

Physical spray feel, venue-volume listening, unfamiliar-player observations and real-Mac launch were explicitly excluded from the automated release pass. They remain optional future observations, **not failed gates and not passes**. Do not reinstate them as prerequisites for completing the already authorized automated work. Exact evidence, archive hashes and historical creator reports stay in [Validation](../Project/VALIDATION.md).

## October 28–31 delivery plan

Dates are targets. A later date does not imply an earlier task passed. The creator's approximate 4–6 available hours per day favor one bounded change plus review; avoid simultaneous gameplay experiments during this window.

| Target | Deliverable | Owner and completion rule |
|---|---|---|
| Oct 28 | Additive scene/art pack and official-site refresh | Maintainer: preserve existing GUIDs and production baseline; save usable materials/meshes/prefabs, exact generation prompts and provenance; run applicable import/reference/website checks. New art stays a separate study until deliberately promoted |
| Oct 28 | Update the current plan and release presentation | Maintainer: replace stale implementation to-do lists with evidence-based status; keep demo.6 downloads and demo.5 rollback; publish repository/website changes through normal commits |
| Oct 29 | Prepare operator handoff and recovery materials | Maintainer prepares the [operator runbook](../Project/EXHIBITION_RUNBOOK.md), control card and local primary/fallback copies. Creator confirms logistics if available; any physical rehearsal is recorded separately |
| Oct 30 | Distribution freeze | Preserve exact versioned ZIPs, checksums and notices. Change only a demonstrated blocker; changed package bytes require a new version and affected checks |
| Oct 31 | Exhibition, if logistics are confirmed | Use the selected package, short instructions and the documented recovery path; retain any observations with version/hardware context |
| Nov 1 | Optional second day | Continue only if participation is confirmed; retain the same package unless a separately verified fix is needed |

No new executable, renamed replacement ZIP, feature-complete claim or performance promise follows automatically from an art/website push. A production-scene promotion uses a separate review: native before/after at the same camera, target/HUD visibility, unchanged gameplay references, focused Unity checks and then a new immutable package only when requested. A lighting/reflection rebake is its own measured task because it can change readability and load.

## Parallel work and asset acceptance

| Lane | Concrete output | Acceptance boundary |
|---|---|---|
| Scene composition | Modular edge-of-room props, framed art and a separately saved studio study | Keep the central aiming area clear; decorative props have no gameplay colliders or unnecessary realtime shadows; preserve source scene and lighting assets |
| Art production | Original text-free poster/window art, reusable native geometry/materials and selected web derivatives | Record each exact image prompt, source path, hash, intended use and restrictions; pair Unity assets with `.meta` files; do not call a reference image a model or gameplay capture |
| Official website | Responsive trilingual showcase with legible art/release distinctions | Preserve real uploaded platform downloads, keyboard access, reduced-motion support and local media provenance; run build/tests and report browser coverage accurately |
| Delivery and docs | Current roadmap, catalog links and traceable verification | One source of truth for design/plan/evidence; no duplicated release history or unverified completion claims |

Useful art is art that can be reused and evaluated. Prefer a small coherent pack over disconnected variants: a quiet large-area surface, a focal artwork and supporting modular props. Each piece needs a named intended use. Keep poster/window content unlit or appropriately materialized for its use; do not bake accidental directional shadows into reusable albedo. New props are not automatically new paint receivers.

## Immediate lesson queue

No new core gameplay is needed to publish this art/website update. For the next gameplay request, explain **one small field, helper or call site**, let the learner write it, then review and verify the saved result. The creator previously deferred more paint-scale parameter lessons.

The first useful future lesson is to inspect one existing behavior and its evidence before adding a system: for example, review the five-hit/two-on-beat score identity or a Retry reset. If the learner chooses spray tuning, use the existing [cadence lesson](SPRAY_FEEL_LESSON.md) with one variable at a time. Neither activity authorizes an assistant rewrite of `Assets/Scripts`.

## Post-festival milestones

Start no earlier than November 2 after preserving the package and any exhibition observations. Windows solo remains the reference mode. These are ordered, reviewable milestones rather than promises of a large November feature set; select one active gameplay milestone at a time.

| Milestone | Dependency and smallest deliverable | Done when / decision |
|---|---|---|
| M0 — review and preserve | Archive release hashes, current source, known limits and any versioned observations; choose the most important player problem | One ranked backlog item has a reproduction or observation and a measurable acceptance condition; keep demo.6 recoverable |
| M1 — comfort and settings | M0; teach one audio, display or signed timing-calibration setting in the existing solo loop | Defaults preserve current behavior; value persists; pause/retry/relaunch and trilingual layout are checked. No silent timing-window/reward changes |
| M2 — song/chart selection | M1 or a documented decision to prioritize content; first select the existing song via stable song/chart/rules identities | Selection loads matching audio/chart/credits, settles at full duration and isolates records. Add a second rights-checked song only after this passes |
| M3 — paint rendering study | A measured paint/object cost or a specific receiver requirement; follow [full-scene paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md) on one planar receiver in a separate scene | Correct pigment/contact/overlap, persistence through results, clean Retry and measured object/memory behavior. Compare against the current sprites before deciding whether to promote |
| M4 — explorable studio learning slice | M0 plus explicit choice to explore movement; original graybox with a short route and landmarks, then one movement/camera lesson | One small area is comfortable to traverse and supports readable aim/rhythm/paint. Compare it with fixed-camera solo; retain it as a study if it weakens the core experience |
| M5 — local co-op | Stable solo loop and selected input lesson; P1 mouse plus explicitly paired P2 gamepad, Xbox first | Independent reticles/release guards, single target resolution, shared clock and reliable pause/disconnect/reconnect. Beitong follows as a separate device check |
| M6 — broader distribution | Evidence of demand and target hardware access | Real-Mac launch, signing/notarization and any broader support have their own tested packages and stated limits; availability of a Universal ZIP alone is insufficient |

M3 and M4 are separate experiments, not prerequisites to enjoy the fixed-camera game. Do not combine movement, camera, new paint rendering, song selection and co-op into one lesson. Art/website work may continue in parallel without consuming the current gameplay milestone's acceptance time.

### Original 3D-map study

For M4, first compare several games at the level of player problem, camera, landmarks, pacing and feedback. Record an original answer for BalloonBarrage rather than importing another game's visual identity. Then build a graybox route, add one movement/camera behavior, prepare one eligible paint surface and finally test a complete-song rhythm encounter in that small area. [Game design](GAME_DESIGN.md#inspiration-and-originality-boundary) owns the originality boundary.

After one reviewable slice, decide to promote, revise or archive it. A larger map, extra target classes, roguelite upgrades and run progression remain exploratory and have no scheduled delivery commitment. Keep the casual standard-song mode available without those systems.

## Exhibit session and recovery

Plan a 3–4 minute turn: short instructions, the complete 123.429-second song and a brief result/paint moment. Do not crop or speed the track to manage a queue. Use Japanese-first instructions with the existing Chinese/Japanese/English selector; no account entry, public name leaderboard or compulsory score comparison is needed.

The [operator runbook](../Project/EXHIBITION_RUNBOOK.md) owns setup, handoff and recovery. The optional [creator playtest template](../Project/EXHIBITION_PLAYTEST.md) can record comfort, timing clarity, dense-paint readability and audio observations if performed. Preserve the tested version/hardware context instead of converting a report into general acceptance of later code.

## Change and cut rules

- Fix a reproduced reliability, contact, readability or localization blocker before adding ornament. Defer milestone flourishes, extra bob, new-record animation, extra variants and reflection work first.
- Preserve the complete song, existing rewards/window, finite hit cadence, round-persistent paint and original notices. No timed fading, oldest-first removal or silent paint cap; optimize only from measured cost.
- Keep new art studies separate from production promotion. Website build, asset import, native render, Play Mode fixture and exact-package launch each prove different things.
- Use the [desktop release runbook](../Project/PACKAGING_LESSON.md) for a requested new package. Never overwrite an existing public release ZIP or remove the rollback while validating its successor.
- Preserve concurrent learner edits, GUIDs and paired `.meta` files. Keep project documentation in English, record exact image prompts and attribution, and append normal commits without rewriting history.
