# Development roadmap

Updated: October 29, 2026 (Asia/Tokyo). This is the delivery plan. [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns completed evidence, and [README](../../README.md) is the entry point.

## Current decision

The creator authorized parallel Codex implementation of the festival revision, all-ten-song preparation, updated desktop packages and an ordinary push to `main`. This authorizes the bounded implementation described here; preserve the learner's existing three functions in `KyudaisaiLearnerCore.cs` and retain learner ownership for future lessons.

Prepare **v0.1.0-demo.8** with ten songs in two modes:

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

## Parallel ownership and integration

| Lane | Owner and output | Boundary |
|---|---|---|
| Catalog preparation | Codex catalog agent: derived charts, sections, demonstration metadata, provenance and meaningful preparation tests | Original recordings/charts/notices remain unchanged; human listening remains separately labelled |
| Runtime | Codex runtime agent: phrase/placement consumption, noninteractive demonstration and mode selection | Preserve learner functions, scoring, hit cadence, contact-aware pops, existing paint receivers and persistence |
| Verification | Codex verification agent: all-profile data/runtime checks, lifecycle/score/capacity cases and structured evidence | Controlled-clock traversal, natural real-time rounds, screenshots and physical play are different evidence |
| Integration and delivery | Primary Codex agent: source review, native visual inspection, documentation/site updates, isolated release build and main-branch push | No force-push, overwritten public assets, concurrent editor shutdown or unearned acceptance claims |

Only the primary agent runs Unity integration and release commands. Agents share the workspace and own disjoint files. The active learner directory is not the release-build source. Preserve GUIDs and paired `.meta` files; use a named committed revision in a separate checkout for release builds.

## October 29–31 delivery targets

Dates are targets; a later date never implies an earlier check passed.

| Target | Work | Decision |
|---|---|---|
| Oct 29 | Generate all twenty profiles and integrate sections/demo/mode UI | Run fast all-profile checks first; correct timing/placement/lifecycle errors before long runs |
| Oct 29–30 | Run natural automated rounds across the catalog and inspect native selection/demo/answer captures | Record the exact tested profiles, revision and elapsed time. Human aiming/listening and hardware observations remain separate |
| Oct 30 freeze | Build immutable Windows x64 and unsigned macOS Universal candidates from the named source revision | Verify exact archives, notices, checksums, manifests and fresh Windows launch; state Mac testing limits and preserve the fallback |
| Oct 31 | Planned exhibition if logistics are confirmed | Use default festival mode and the verified package. Operator may recommend the easiest two tracks; no additional feature work at the booth |
| Nov 1 | Optional second day | Participation, booth and operating hours remain unconfirmed; keep the same package unless a separately verified fix is needed |

The creator previously planned October 31 at Kyudaisai. The [official event site](https://kyudaisai.jp/79th/) is the event source; festival-wide dates/hours are not the project's confirmed booth schedule.

## Exhibit session and recovery

Plan a **90–120-second visitor turn**: roughly 10–20 seconds for instruction/selection, **60–75 seconds of gameplay** and 10–20 seconds for the painted result/handoff. Recommend the simplest short chart first. Complete songs remain available in standard mode and have their actual durations displayed; never present them as short queue rounds.

Use concise Japanese-first controls with the existing language selector: aim, hold fire, optional beat bonus, C for pigment and Esc for pause. Explain a demonstration as “watch the outline, then pop the balloons”; no special input is needed. No account, name entry, grade, public leaderboard or compulsory retry is required.

The [operator runbook](../Project/EXHIBITION_RUNBOOK.md) owns setup and recovery. The optional [playtest template](../Project/EXHIBITION_PLAYTEST.md) records unfamiliar-player comfort, musical recognition and visibility if performed. If a new interaction proves confusing or unstable, fall back to the preserved verified package/profile and state its actual duration.

## Immediate lesson queue

The authorized festival implementation is handled by Codex for this revision. Subsequent core gameplay lessons return to the working agreement: explain one small step, let the learner write it, then review and verify. Do not restart the three saved learner functions as unfinished work. Select the next lesson from an observed player problem rather than adding unrelated mechanics.

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

Co-op, movement, new paint rendering and roguelite upgrades are separate decisions. Study reference games at the level of player problems and create original answers, following the [originality boundary](GAME_DESIGN.md#inspiration-and-originality-boundary).

## Change and cut rules

- Fix reproduced timing, contact, crash, visibility or localization issues before adding ornament. Cut extra variants and milestone effects first.
- Keep unrestricted held fire, finite cadence, existing rewards/window and round-persistent paint. No timed fade, paint-count cap or oldest-first deletion.
- Preserve source recordings and original charts/notices. New profiles/derived charts use explicit identities; standard and festival records cannot mix. Retain noncommercial StepMania restrictions and BY-SA attribution/licensing for the affected derivatives.
- Do not declare a playable release from a plan, website check, asset import or camera render. Report controlled-clock and natural runtime coverage separately; exact-package startup is not full standalone-song acceptance.
- Use the [release runbook](../Project/PACKAGING_LESSON.md), preserve immutable assets and append normal commits. Push `main` without rewriting history. Never overwrite concurrent learner work.
