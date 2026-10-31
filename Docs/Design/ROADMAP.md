# Development roadmap

Updated: October 31, 2026 (Asia/Tokyo). This is the delivery plan. [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns completed evidence, and [README](../../README.md) is the entry point.

## Current decision

The creator-authorized [room-wide ink revision](FULL_SCENE_PAINT_ARCHITECTURE.md) is published in demo.17 for Windows x64 and macOS Universal, with the updated trilingual website. It covers 404 prepared static receivers, continuous strokes, dense original wet ink and bounded GPU storage. The targeted corner/curve fixture passes 171 checks and the full studio GPU/physics fixture passes 1,256 assertions; both clean builds have zero errors/warnings and archive integrity checks pass. The creator selected automatic checks/publication after desktop application authorization timed out. Visible Windows UI, physical controllers, venue audio and real-Mac acceptance remain separate in [Validation](../Project/VALIDATION.md). Preserve learner/art working edits, immutable archives, music originals, source/GUIDs and verification records.


The creator-authorized [sequence guidance, celebrations, A+B+C Kyudaisai presentation and gamepad feel](GAME_DESIGN.md#sequence-guidance-and-celebrations--source-revision) remain in the published baseline. Controlled checks cover read-only guidance, ordinary/beat/chain reward boundaries and modal/input lifecycle; physical Xbox/Beitong comfort remains a creator playtest. Open-world gameplay remains outside this authorization.

The October 30 planning request authorizes documenting the selected open-world direction, updating the official website and pushing main. It does **not** authorize implementing open-world gameplay. The selected topology, encounter structure and first AI role are in [Game design](GAME_DESIGN.md#open-world-direction--planning-only); the [open-world track](#open-world-development-track--planning-only) below is the future learner/assistant development sequence. Exhibition and package work retain their existing evidence boundaries.

The published baseline retains the creator's completed co-op decisions, all twenty v4 song/mode profiles and the [optional tricolor relay](GAME_DESIGN.md#optional-tricolor-relay). Preserve learner method bodies, historical chart/record identities, music originals and asset GUIDs. Source checks, package integrity, physical controllers, native UI and real-Mac acceptance remain separate in [Validation](../Project/VALIDATION.md). Postcards/export and paint ribbon remain deferred.

| Current work | Completion boundary |
|---|---|
| Demo.15 desktop candidates | Preserve the verified immutable ZIPs; keep native Windows, physical-controller and real-Mac acceptance separate from passed controlled checks and headless startup |
| Trilingual official website | Present published demo.16 downloads with optional cheer playback, explain the three cue meanings and Kyudaisai presentation, and retain the confirmed Room 2407 meeting place; future packages need their own release |
| Local disk reclamation | Remove only verified recoverable extractions or reproducible build output; preserve release archives, checksums, evidence, source, asset GUIDs, notices and music originals |

Unity checks, exact-package observations, screenshots and remaining human/device checks belong to [Validation](../Project/VALIDATION.md#demo15-desktop-publication--october-31). Keep real-Mac, native keyboard/focus, sustained-spray and venue acceptance distinct from the completed checks. Demo.15 remains the previous desktop candidate, and demo.6 remains the complete-song fallback. Published packages are immutable; historical natural rounds and newer controlled diagnostics retain their original revision boundaries.

- **Festival:** 60–75 seconds per song, preferably about 65 seconds, with a clear musical start, contrast and ending. Default to this mode for the booth.
- **Standard:** each complete recording, with a separate versioned chart/profile identity. Do not overwrite original audio or existing profiles.

Use authored sweep/fan/staggered sections, readable recurring motifs and a brief listen-then-answer interaction. Existing free aiming, unrestricted-duration held fire, 10/20 scoring, timing window and persistent paint remain the baseline. The creator explicitly chose to **defer the long-note paint ribbon**: keep its design as an optional later experiment, with no runtime implementation or exhibition dependency.

All ten songs receive both profiles; this replaces the earlier two-song-only preparation scope. The operator may recommend one spacious song and one livelier song without removing the rest. Do not describe automated audio analysis as human listening, a generated beat grid as a professionally calibrated chart, or new source as a tested package.

## Preserved baseline

The October 29 content update adds seven recordings and production studio art, documented in the [expansion guide](../Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md). Its demo.7 build/publication status belongs to its own evidence. **demo.6** remains the known published complete-song fallback; do not replace or overwrite its immutable ZIPs. Keep any subsequently verified demo.7 package as an additional fallback rather than changing its contents.

Saved source has twenty v4 profiles: ten roughly 64–65-second Festival rounds and ten full Standard recordings. The learner functions remain saved. Current constraints are a finite six-sample firing cadence, 24-target capacity, twelve-second post-note cleanup, eight pigments and 404 explicitly prepared opaque static paint receivers. One fixed atlas preserves accumulated pigment without a count limit or oldest-first removal.

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
| Runtime | Codex: authorized all-song, gamepad and co-op support; learner: two reserved P2 decisions | Preserve learner functions, scoring, hit cadence, contact-aware pops, existing paint receivers and persistence |
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

The current learner step is [co-op lesson 1](LOCAL_COOP_LESSON.md): implement only `CoopLearnerCore.IsSecondShotReady`, check early/equal/late times, then request review. Leave `SecondPlayerBeatBonus` untouched until that first step is verified. The assistant owns support wiring and checks; the learner owns these two decisions. Preserve the three existing `KyudaisaiLearnerCore` solutions. New co-op packaging follows completion/review of the lesson rather than presenting placeholders as accepted gameplay.

## Post-festival milestones

Begin after preserving the exhibition package and any observations. Select one gameplay milestone at a time.

| Milestone | Smallest deliverable | Acceptance / decision |
|---|---|---|
| M0 — review | Preserve source, package hashes, limits and versioned observations; rank one player problem | A reproducible issue or recorded observation has a measurable next step |
| M1 — comfort | One audio/display/signed timing-calibration setting in the existing solo loop | Defaults preserve rules; persistence, pause/Retry/relaunch and localized layout are checked |
| M2 — musical refinement | Review one song's offset/phrases through listening and one listen-then-answer interaction through observation | Keep, revise or remove the interaction based on musical clarity and comfort; repeat song by song |
| M2 optional — paint ribbon | Only if later selected: one sustained phrase invites a guided stroke on an existing eligible wall, initially without tracking score | It adds enjoyable musical expression beyond free painting. Otherwise retain free paint and archive the experiment |
| M3 — paint rendering | Maintain the saved 404-receiver atlas from [paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md); add new geometry only after explicit preparation/contact review | Correct contact/pigment/overlap, retained artwork, clean Retry and measured object/memory cost |
| M4 — explorable studio | A separate original graybox route and one learner movement/camera increment | Navigation supports comfortable aiming/rhythm; preserve fixed-camera mode if exploration weakens it |
| M5 — local co-op | Stable solo loop; P1 mouse plus explicitly paired P2 gamepad, Xbox first | Independent reticles/release guards, single target resolution, shared clock and reliable reconnect; Beitong follows separately |
| M6 — wider distribution | Real-Mac access and evidence of demand | Launch/signing/notarization and broader hardware support each have tested packages and stated limits |

The selected open-world track below refines M4 into small teaching steps. M0 review and demonstrated comfort problems take priority; M1–M3 are not automatic prerequisites when unrelated to the first graybox question. Co-op, new paint rendering and roguelite upgrades remain separate decisions. Study references at the level of player problems and create original answers, following the [originality boundary](GAME_DESIGN.md#inspiration-and-originality-boundary).

## Open-world development track — planning only

No release date or calendar estimate is committed. The three districts are a long-term design envelope, not a commitment to build them together. [Game design](GAME_DESIGN.md#confirmed-detail-choices--c--a--b) records the creator-confirmed **C / A / B** gameplay direction and [A + C art direction](GAME_DESIGN.md#confirmed-art-direction--a--c): a sunny street music festival with handmade art-town details. Night festivals are deferred beyond the accepted daytime slice. This update is planning and website work only. Scope changes and additional choices must remain explicit.

| Gate | Smallest deliverable and dependency | Evidence required before expansion |
|---|---|---|
| OW0 — decide and sketch | Gameplay C / A / B and art A + C confirmed; preserve the studio baseline, sketch plaza → first stage → return and define a daylight palette/material brief | Route, troupe roles, camera transitions and layer rewards still need design; distinguish landmark/decoration/scoring balloons by shape as well as color; choices alone do not complete a graybox or playtest |
| OW1 — walk comfortably | After OW0: separate third-person graybox with plaza edge, one path and one stage; learner writes one movement/camera increment | Observe navigation, camera collision/occlusion, motion comfort, return-path clarity and pause/focus behavior; record human observations separately from automated checks |
| OW2 — enter and leave one song | After OW1: explicitly enter a stable stage view using one existing profile; return after results or exit | Audio ownership/clock, input handoff, Retry, exit, records and target cleanup verified; free exploration never starts a chart unintentionally |
| OW3a — one troupe member | After OW2: telegraph one target, bounded aim/fire/recovery, then one complete encounter as the troupe foundation | Single target consumption, simultaneous-hit rule, line of sight and reaction delay checked; player understands the member's intent; solo records/rules preserved |
| OW3b — a coordinated pair | After OW3a: add a second member with distinguishable intent and bounded turn-taking; test one proposed interruption separately | No unreadable simultaneous threats, blocked routes, shifted chart timing or hidden scoring targets; record actual performance and player comprehension |
| OW4 — one complete visit | After OW3b: plaza → pier troupe challenge → result → first musical layer → plaza ensemble; apply A + C to this slice with one visible completion change | Observe an unaided 5–8-minute target visit; verify no duplicate reward on Retry, phrase-aligned audio and clean mode handoff; check glossy targets against matte/paper/wood scenery, bounded effects, comfort and performance |
| OW5 — expand only what works | After OW4 acceptance: one district/layer at a time; separately study short one-rule challenge variants, optional detours, rival roles and cosmetic identity | Distinct experiences, readable recovery/intent cues, audible layer combinations and verified provenance; optional pursuits stop during stage play; measure scene, paint, AI and audio costs before further expansion |

The learner owns future gameplay code. The assistant prepares a small exercise, documentation, original assets or tools as needed, then reviews and verifies the saved result. Direct gameplay implementation needs an explicit request. The selected troupe is included through the one-member and coordinated-pair gates; a larger simultaneous group is beyond the first slice. Beat Teaser, Balloon Porter and Stage Leader are candidate studies, not three required first-slice characters. Persistent world saves, procedural maps, cloud-generated dialogue/music, online multiplayer, damage systems and a seamless city remain outside the first slice. Prepare compatible, appropriately licensed music layers rather than assuming existing recordings provide remixable stems.

The night music festival is a later optional expansion, not a scheduled gate or an automatic OW5 deliverable. Revisit it only after accepting the daylight slice, with a separate lighting, target-readability, performance and music-transition review; a day/night cycle is not implied.

If walking and aiming compete for attention, keep the fixed stage camera. If rivalry undermines the relaxing experience, offer the same stage solo and reduce target contention. If the first route is confusing, shorten it before building a second district. Keep measured runtime/package evidence in [Validation](../Project/VALIDATION.md), separate from site checks and proposed acceptance criteria.

### Boss combat research track — planning only

The October 31 choice is **single-player direct combat against a Boss and its AI squad**, not a balloon scoring contest. [Game design](GAME_DESIGN.md#optional-boss-brawl--direct-combat) owns the encounter rules and [research/asset responsibilities](GAME_DESIGN.md#boss-ai-research-and-asset-responsibility). This track follows acceptance of the basic daytime town visit (OW4); it is not a dependency for the current festival build, a delivery date, or a promise to complete every research idea.

| Gate | Smallest deliverable | Evidence before the next step |
|---|---|---|
| B0 — combat brief and art | One paper arena and direct-combat walkthrough; assistant prepares original silhouette, material, cover and warning references | Creator/learner can explain who attacks whom, what the weak point is, how to avoid an attack and how to exit; health/defeat/camera questions explicitly recorded |
| B1 — moving combat baseline | A separate small test arena, simultaneous movement/aiming, one rule-based enemy and one telegraphed attack | Contact, finite held-fire cadence, line of sight, warning readability, evasion, camera comfort, hit/defeat feedback and retry/exit checked; no change to existing rhythm scores |
| B2 — one Boss and one minion | One Boss phase with recovery/weak-point opening; one minion and bounded coordination | No unavoidable combined attack, blocked return path or unreadable off-screen pressure; record human comprehension separately from automated checks |
| B3 — optional learned decisions | Offline cover/reposition policy, evaluated against B2 with identical constraints and a rule-based fallback | Measured comfort/fairness, generalization, stalls, frame time, memory and fallback behavior justify adoption; otherwise retain B2 |
| B4 — accepted encounter integration | Optional district arena entry/exit, explicitly designed reward and original approved assets; later template variations only if useful | Clean town/combat/audio transitions, isolated records, no duplicate rewards, preserved exploration access, provenance and runtime budget |

The learner writes gameplay one step at a time; the assistant prepares assets/tools/docs and reviews saved code. Asset preparation is assigned to the assistant. No trained model, combat script, new Unity asset import or playable Boss scene is delivered by this website/planning update. Online or local multiplayer Boss modes, additional factions and night festivals require separate scope decisions.

## Change and cut rules

- Fix reproduced timing, contact, crash, visibility or localization issues before adding ornament. Cut extra variants and milestone effects first.
- Keep unrestricted held fire, finite cadence, existing rewards/window and round-persistent paint. No timed fade, paint-count cap or oldest-first deletion.
- Preserve source recordings and original charts/notices. New profiles/derived charts use explicit identities; standard and festival records cannot mix. Retain noncommercial StepMania restrictions and BY-SA attribution/licensing for the affected derivatives.
- Do not declare a playable release from a plan, website check, asset import or camera render. Report controlled-clock and natural runtime coverage separately; exact-package startup is not full standalone-song acceptance.
- Use the [release runbook](../Project/PACKAGING_LESSON.md), preserve immutable assets and append normal commits. Push `main` without rewriting history. Never overwrite concurrent learner work.
