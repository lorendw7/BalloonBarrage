# Development roadmap

Updated: October 30, 2026 (Asia/Tokyo). This is the delivery plan. [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns completed evidence, and [README](../../README.md) is the entry point.

## Current decision

The October 30 planning request authorizes documenting the selected open-world direction, updating the official website and pushing main. It does **not** authorize implementing open-world gameplay. The selected topology, encounter structure and first AI role are in [Game design](GAME_DESIGN.md#open-world-direction--planning-only); the [open-world track](#open-world-development-track--planning-only) below is the future learner/assistant development sequence. Exhibition and package work retain their existing evidence boundaries.

The creator explicitly authorized implementing the four [showcase additions](GAME_DESIGN.md#goin-under-festival-showcase--v3), testing them, packaging Windows/macOS candidates and pushing main. The bounded Goin' Under Festival mirror, motion phrase and finale, plus the shared-planner Rainbow chain preview, are implemented and checked. Source `ee2ef85` is pushed to main, and both local demo.12 candidates passed builds and exact ZIP checks. Windows interface acceptance is pending because the desktop was locked; real-Mac acceptance is also pending. Preserve all original event times/audio and the nineteen other profiles, isolate showcase records, then verify source behavior and exact packages. Keep packages/evidence immutable and remove recoverable temporary build files after verification. Published demo.11 downloads remain separate until a new publication is requested. Postcards/export and the paint ribbon remain deferred.

The creator explicitly authorized the tricolor relay and its new Windows/macOS packages, tests, publication, website download update and temporary-file cleanup. Preserve the learner's three saved functions in `KyudaisaiLearnerCore.cs` and retain learner ownership for future lessons. [Optional tricolor relay](GAME_DESIGN.md#optional-tricolor-relay) owns the rules. Demo.11 adds the relay; demo.10 remains immutable.

| Current work | Completion boundary |
|---|---|
| Demo.12 four-interaction candidates | Independent clean-source Windows/macOS builds, immutable ZIPs, exact per-file/provenance/license checks, Windows package observations and clearly labelled remaining Mac/human acceptance |
| Trilingual official website | Continue presenting published demo.11 downloads and the confirmed meeting place, Room 2407 in Center Building 2 (センター２号館); a new public download needs a separate release |
| Local disk reclamation | Remove only verified recoverable extractions or reproducible build output; preserve release archives, checksums, evidence, source, asset GUIDs, notices and music originals |

Unity checks, exact-package observations, screenshots and remaining human/device checks belong to [Validation](../Project/VALIDATION.md#demo11-tricolor-relay-release--october-30). Keep real-Mac, native keyboard/focus, sustained-spray and venue acceptance distinct from the completed checks.

The current **v0.1.0-demo.11** Windows/macOS candidates include the tricolor relay and the earlier input protection, balloon placement, protected drift wait, retention and NEXT repairs. Both non-Development builds have zero errors/warnings. Both exact ZIPs passed structural/hash/provenance checks; a fresh Windows extraction completed a natural short round with one relay, results, Retry and normal Quit. Real-Mac, native keyboard/focus, held-fire soak, human listening and exhibition-machine acceptance remain separate.

Demo.10 remains the previous desktop candidate, and demo.6 remains the complete-song fallback. Published packages are immutable. Demo.8's natural all-profile Editor rounds and the newer focused/synthetic diagnostics remain distinct evidence in [Validation](../Project/VALIDATION.md).

- **Festival:** 60–75 seconds per song, preferably about 65 seconds, with a clear musical start, contrast and ending. Default to this mode for the booth.
- **Standard:** each complete recording, with a separate versioned chart/profile identity. Do not overwrite original audio or existing profiles.

Use authored sweep/fan/staggered sections, readable recurring motifs and a brief listen-then-answer interaction. Existing free aiming, unrestricted-duration held fire, 10/20 scoring, timing window and persistent paint remain the baseline. The creator explicitly chose to **defer the long-note paint ribbon**: keep its design as an optional later experiment, with no runtime implementation or exhibition dependency.

All ten songs receive both profiles; this replaces the earlier two-song-only preparation scope. The operator may recommend one spacious song and one livelier song without removing the rest. Do not describe automated audio analysis as human listening, a generated beat grid as a professionally calibrated chart, or new source as a tested package.

## Preserved baseline

The October 29 content update adds seven recordings and production studio art, documented in the [expansion guide](../Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md). Its demo.7 build/publication status belongs to its own evidence. **demo.6** remains the known published complete-song fallback; do not replace or overwrite its immutable ZIPs. Keep any subsequently verified demo.7 package as an additional fallback rather than changing its contents.

Saved source before this revision has ten selectable profiles: seven windows of about 80–87 seconds and three complete classics of about 116–254 seconds. The three learner functions are saved. The scene has a finite six-sample firing cadence, 16-target capacity, 2.8-second previews, five-second unanswered-target cleanup, eight pigments and nine deliberately prepared paint receivers. These values are constraints, not permission to increase density or remove old paint.

Historical automated checks, creator reports, anonymous feedback about uniform pacing/offset effects, exact package hashes and remaining limits stay in [Validation](../Project/VALIDATION.md). Preserve those revision boundaries. Physical feel, venue listening, unfamiliar-player observation and real-Mac launch are separate observations and are not silently reinstated as blockers for the authorized automated pass.

## Festival implementation order

| Priority | Deliverable | Completion condition |
|---|---|---|
| P0 | Reproducible ten-song catalog with festival and standard profiles | Twenty unique versioned record identities, matching clips/charts, valid source windows/events, preserved hashes/licenses and complete source duration in standard mode |
| P0 | Short, readable song pacing | Each festival profile is 60–75 seconds at unchanged pitch/speed; reduce events rather than compressing the old count. Record whether boundaries/offsets are inferred or listened to |
| P1 | Explicit musical sections and recurring motifs | Source/round times convert correctly; sections select existing pattern families. Direction/spacing remain possible at the saved cadence, with no skipped events or shifted judgement times |
| P1 | Brief listen-then-answer | Three noninteractive outline markers demonstrate a route; distinct real chart events answer at the same positions. Music never waits for input. Demo markers cannot collide, score, obscure answer previews or become a memory requirement |
| P1 | Clear mode selection | Default festival mode, complete-song standard mode, readable durations and Chinese/Japanese/English labels; switching selects matching audio/chart/window/record |
| P0 | Runtime and package verification | Affected source/data checks, all-profile automated runtime checks, natural-round evidence explicitly identified, immutable desktop builds, ZIP/hash/manifest checks and fresh Windows extraction/startup |
| Deferred | Long-note paint ribbon | Design only. No guide, tracking score, new note type or additional exhibition controls in this version |

The [music-led design](GAME_DESIGN.md#music-led-festival-variety) defines the interaction, timing and fairness rules. Keep one target per scoring event; ordinary early hits consume their event normally. Demonstrations and other decoration never create rewards. Do not force tapping, add color-match requirements or change timing/reward values during this slice.

## Implementation ownership and integration

| Lane | Owner and output | Boundary |
|---|---|---|
| Catalog preparation | Preserve the saved catalog, sections, demonstration metadata and provenance | Original recordings/charts/notices remain unchanged; human listening remains separately labelled |
| Runtime | Codex: the explicitly authorized relay, Rainbow chain and four showcase additions | Preserve learner functions, scoring, hit cadence, contact-aware pops, existing paint receivers and persistence |
| Verification | Codex: focused collection tests and runtime lifecycle/scoring/layout checks | Controlled-clock traversal, natural real-time rounds, screenshots and physical play are different evidence |
| Integration and delivery | Codex: source review, native visual inspection and documentation/site publication | No force-push, overwritten public assets, concurrent editor shutdown or unearned acceptance claims |

Keep Unity integration sequential and avoid concurrent edits. The active learner directory is not the release-build source. Preserve GUIDs and paired `.meta` files; use a named committed revision in a separate checkout for release builds.

## October 29–31 delivery targets

Dates are targets; a later date never implies an earlier check passed.

| Target | Work | Decision |
|---|---|---|
| Oct 29 | Generate all twenty profiles and integrate sections/demo/mode UI | Run fast all-profile checks first; correct timing/placement/lifecycle errors before long runs |
| Oct 29–30 | Run natural automated rounds across the catalog and inspect native selection/demo/answer captures | Record the exact tested profiles, revision and elapsed time. Human aiming/listening and hardware observations remain separate |
| Oct 30 freeze | Build immutable Windows x64 and unsigned macOS Universal candidates from the named source revision | Verify exact archives, notices, checksums, manifests and fresh Windows launch; state Mac testing limits and preserve the fallback |
| Oct 31 | Planned exhibition; meeting place confirmed as Center Building 2, Room 2407 | Use default festival mode and the verified package. Operating hours remain unconfirmed. Operator may recommend the easiest two tracks; no additional feature work at the booth |
| Nov 1 | Optional second day | Participation, booth and operating hours remain unconfirmed; keep the same package unless a separately verified fix is needed |

The creator previously planned October 31 at Kyudaisai. The [official event site](https://kyudaisai.jp/79th/) is the event source; festival-wide dates/hours are not the project's confirmed booth schedule.

## Exhibit session and recovery

Plan a **90–120-second visitor turn**: roughly 10–20 seconds for instruction/selection, **60–75 seconds of gameplay** and 10–20 seconds for the painted result/handoff. Recommend the simplest short chart first. Complete songs remain available in standard mode and have their actual durations displayed; never present them as short queue rounds.

Use concise Japanese-first controls with the existing language selector: aim, hold fire, optional beat bonus, C for pigment and Esc for pause. Explain a demonstration as “watch the outline, then pop the balloons”; no special input is needed. No account, name entry, grade, public leaderboard or compulsory retry is required.

The [operator runbook](../Project/EXHIBITION_RUNBOOK.md) owns setup and recovery. The optional [playtest template](../Project/EXHIBITION_PLAYTEST.md) records unfamiliar-player comfort, musical recognition and visibility if performed. If a new interaction proves confusing or unstable, fall back to the preserved verified package/profile and state its actual duration.

## Immediate lesson queue

The October 30 relay, Rainbow chain and four showcase additions were explicitly authorized for direct Codex implementation. Subsequent core gameplay lessons return to the working agreement: explain one small step, let the learner write it, then review and verify. Do not restart the three saved learner functions as unfinished work. Select the next lesson from an observed player problem rather than adding unrelated mechanics.

## Post-festival milestones

Begin after preserving the exhibition package and any observations. Select one gameplay milestone at a time.

| Milestone | Smallest deliverable | Acceptance / decision |
|---|---|---|
| M0 — review | Preserve source, package hashes, limits and versioned observations; rank one player problem | A reproducible issue or recorded observation has a measurable next step |
| M1 — comfort | One audio/display/signed timing-calibration setting in the existing solo loop | Defaults preserve rules; persistence, pause/Retry/relaunch and localized layout are checked |
| M2 — musical refinement | Review one song's offset/phrases through listening and one listen-then-answer interaction through observation | Keep, revise or remove the interaction based on musical clarity and comfort; repeat song by song |
| M2 optional — paint ribbon | Only if later selected: one sustained phrase invites a guided stroke on an existing eligible wall, initially without tracking score | It adds enjoyable musical expression beyond free painting. Otherwise retain free paint and archive the experiment |
| M3 — paint rendering | One measured planar receiver study from [paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md) | Correct contact/pigment/overlap, retained artwork, clean Retry and measured object/memory cost |
| M4 — explorable studio | A separate original graybox route and one learner movement/camera increment | Navigation supports comfortable aiming/rhythm; preserve fixed-camera mode if exploration weakens it |
| M5 — local co-op | Stable solo loop; P1 mouse plus explicitly paired P2 gamepad, Xbox first | Independent reticles/release guards, single target resolution, shared clock and reliable reconnect; Beitong follows separately |
| M6 — wider distribution | Real-Mac access and evidence of demand | Launch/signing/notarization and broader hardware support each have tested packages and stated limits |

The selected open-world track below refines M4 into small teaching steps. M0 review and demonstrated comfort problems take priority; M1–M3 are not automatic prerequisites when unrelated to the first graybox question. Co-op, new paint rendering and roguelite upgrades remain separate decisions. Study references at the level of player problems and create original answers, following the [originality boundary](GAME_DESIGN.md#inspiration-and-originality-boundary).

## Open-world development track — planning only

No release date or calendar estimate is committed. The three districts are a long-term design envelope, not a commitment to build them together. [Game design](GAME_DESIGN.md#open-world-direction--planning-only) owns the selected central plaza, fixed-location challenges and rival AI rules. Scope changes and additional choices must remain explicit.

| Gate | Smallest deliverable and dependency | Evidence required before expansion |
|---|---|---|
| OW0 — decide and sketch | Preserve the studio baseline; choose camera, rival temperament and reward emphasis; sketch plaza → first stage → return | Creator agrees on one route and first question. Website is labelled planning; no claim of a playable world |
| OW1 — walk comfortably | After OW0: separate graybox with plaza edge, one path and one stage; learner writes one movement/camera increment | Observe navigation, camera collision, motion comfort, return-path clarity and pause/focus behavior; record human observations separately from automated checks |
| OW2 — enter and leave one song | After OW1: explicitly enter a stable stage view using one existing profile; return after results or exit | Audio ownership/clock, input handoff, Retry, exit, records and target cleanup verified; free exploration never starts a chart unintentionally |
| OW3 — one readable rival | After OW2: telegraph one target, bounded aim/fire/recovery, then one complete rival encounter | Single target consumption, simultaneous-hit rule, line of sight and reaction delay checked; player can explain what the rival will do; no change to solo records/rules |
| OW4 — one complete visit | After OW3: plaza → pier stage → result → one proposed decoration → plaza | Observe a first-time player completing the 5–8-minute target visit without forced retry; measure actual time, comfort, target visibility and performance on a named machine |
| OW5 — expand only what works | After OW4 acceptance: one additional district at a time, using the established encounter contract | Each district adds a distinct musical or spatial experience; measure loaded scene, paint, targets and audio costs before adding streaming, vertical traversal or further AI |

The learner owns future gameplay code. The assistant prepares a small exercise, documentation, original assets or tools as needed, then reviews and verifies the saved result. Direct gameplay implementation needs an explicit request. Persistent world saves, procedural maps, cloud-generated dialogue/music, online multiplayer, several simultaneous rivals, damage systems and a seamless city are outside the first slice.

If walking and aiming compete for attention, keep the fixed stage camera. If rivalry undermines the relaxing experience, offer the same stage solo and reduce target contention. If the first route is confusing, shorten it before building a second district. Keep measured runtime/package evidence in [Validation](../Project/VALIDATION.md), separate from site checks and proposed acceptance criteria.

## Change and cut rules

- Fix reproduced timing, contact, crash, visibility or localization issues before adding ornament. Cut extra variants and milestone effects first.
- Keep unrestricted held fire, finite cadence, existing rewards/window and round-persistent paint. No timed fade, paint-count cap or oldest-first deletion.
- Preserve source recordings and original charts/notices. New profiles/derived charts use explicit identities; standard and festival records cannot mix. Retain noncommercial StepMania restrictions and BY-SA attribution/licensing for the affected derivatives.
- Do not declare a playable release from a plan, website check, asset import or camera render. Report controlled-clock and natural runtime coverage separately; exact-package startup is not full standalone-song acceptance.
- Use the [release runbook](../Project/PACKAGING_LESSON.md), preserve immutable assets and append normal commits. Push `main` without rewriting history. Never overwrite concurrent learner work.
