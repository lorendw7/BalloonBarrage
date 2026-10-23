# Development roadmap

Updated: October 23, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior; [Validation](../Project/VALIDATION.md) owns evidence; [README](../../README.md) is the entry point.

## Exhibition product direction

**A casual, relaxing rhythm game about popping balloons and painting a sunlit studio.** The player promise is: aim freely, hold fire for as long as desired, enjoy the visible color trail and catch the beat when it feels good. Rhythm is an audible and visible invitation, not permission to shoot or a requirement to finish.

The creator has approximately 4–6 hours per day. Prioritize comfortable input, a readable musical cue, satisfying ordinary hits, an audible on-beat accent and a welcoming finish. On October 22 the creator promoted successful-hit count, on-beat-hit count, a forgiving streak, Japanese/English switching and a bounded original ink-spray feedback pass into the pre-festival commitment. The streak remains feedback, not a score multiplier or failure condition.

The [official festival site](https://kyudaisai.jp/79th/), checked October 22, lists October 31–November 1, 2026, 09:00–19:00, at Kyushu University Ito Campus. Those are festival-wide hours, not confirmation of this project's booth. The creator plans October 31; booth location, project operating hours, PC count, operator coverage and November 1 participation remain unconfirmed. Revised internal gates: feature freeze October 27, candidate acceptance October 28, distribution freeze October 30. This leaves less verification buffer than the earlier plan, so every implementation day ends with its focused checks.

## October 23 decision board

The next decision is **whether the current build is dependable**, not how many more features can fit. Work from top to bottom; stop optional work when a higher item is open.

| Order | Required outcome | Current status | Next evidence |
|---|---|---|---|
| 1 | Complete-song core flow on the current source | Creator reports no apparent issue; exact revision/hardware details were not supplied | Preserve this baseline while adding one increment at a time; repeat the focused path after each increment |
| 2 | Successful-hit and on-beat-hit counters | Saved in current source at accepted balloon impact; positive existing bonus forms the on-beat subset | Verify 5 hits / 2 on-beat / 70 points, duplicate protection, wall/empty shots and Retry reset in Play Mode |
| 3 | Forgiving current/maximum streak | Saved in current source; accepted hits increment and unresolved target expiry resets current only | Verify expiry/max behavior, pause/wall/empty-shot exclusions and Retry reset in the natural round flow |
| 4 | Existing ResultsScreen summary binding | Saved scene reference and three-line legacy Text layout; creator visually confirmed the populated 2K natural-result screen | Verify Retry, focused values, other supported layouts and the exact package without adding a second result screen |
| 5 | Japanese/English runtime UI | M+ Japanese/Latin font, license and pinned provenance prepared; translation data exists; runtime switching is not connected | One shared UI, persistent language choice, all static and dynamic labels refreshed, glyph/layout checks at exhibition resolutions |
| 6 | Original energetic ink-spray feedback | Existing projectile, persistent paint, burst, recoil and on-beat accent form the base | Finish a bounded custom muzzle/trail/impact/streak presentation set without copied assets, UI, fonts, weapon shapes or territory rules |
| 7 | Feature freeze | Due October 27 | All committed items must pass focused checks; cut only extra variants, milestones and ornament |
| 8 | Exact exhibition-PC package acceptance | Open | Fresh extraction, 30 minutes of real play, at least ten Retries, pause/focus/audio/quit and recorded hardware details |
| 9 | Primary plus fallback rehearsal | Open | Record immutable filenames, hashes and which machine passed; rehearse operator recovery offline |

The beat-approach fill is saved and connected to the chart/song clock. The existing results screen now receives final score, local best score, successful hits, on-beat hits and maximum streak, and retains Retry. The new accounting and layout are saved source, not accepted gameplay: focused count, expiry, natural-end, Retry and resolution checks remain open. Accuracy, Miss count and grade remain out of scope.

## Current baseline: do not confuse source, previews and release

| Area | Saved or observed evidence | Still required |
|---|---|---|
| Public package | GitHub prerelease v0.1.0-demo.3 contains locally built Windows x64 and unsigned/unnotarized macOS Universal ZIPs; compilation, structure and hashes are recorded | Windows target-PC core-flow/soak acceptance; real-Mac launch/audio/input/complete-song acceptance. Publication is not acceptance |
| Current source | Eight pigments, matching pop/projectile marks, six pattern families, random pattern/rotation/scale | Latest combined-source complete-song and clean-package checks; configurable-angle revision has source review only |
| Current art | Brighter side walls, six-color murals, plaster/mineral floor, multicolor reservoir, author plaque; isolated native renders and component checks | In-play target contrast, latest combined-build load and actual-machine performance; indirect lighting/reflections have not been rebaked |
| Input/feedback | Saved pointer, firing audio, bounded recoil, on-beat feedback and chart-clock-driven approach fill; creator-reported earlier spot checks | Tapping versus held-fire listening test, cue clarity, pause/focus/Retry on final hardware |
| Statistics/results | Successful-hit/on-beat counters, current/maximum streak, reset paths and ResultsScreen binding are saved; creator visually confirmed the populated 2K result layout | Focused 5/2/70, duplicate/expiry, Retry, other-layout and exact-package acceptance before freeze; no missed-event denominator yet |
| Performance | Earlier instrumented full song with 610 marks; ten diagnostic reloads; earlier Editor snapshots | At least 30 minutes of real full-song/retry use, memory/object trends, target-PC frame times |
| Website | Trilingual site publishes the demo.3 Windows/macOS prerelease links and platform limitations; offline build and link checks passed | Keep prerelease/test limitations accurate; web checks are not Unity acceptance |

October 19–20 time included art and learner paint work. Earlier scheduled counter/streak sessions are not treated as completed. Do not compress missed scope into the remaining days.

## Scope and cuts

| Priority | Exhibition outcome | Boundary |
|---|---|---|
| P0: finishable and welcoming | One complete song; free aim and unrestricted-duration held fire; ordinary hits reward; pause/retry/quit reliable | No health, ammunition, reload, heat, elimination, score subtraction, restart-on-miss or color-matching obligation; retain a finite technical projectile cadence |
| P0: music and comfort | First-time players understand the action, hear the track and perceive the beat bonus | Preserve existing chart/0.1-second window/3.1-second cleanup/10–20 scoring until a separate measured tuning decision; do not infer accuracy from held fire |
| P0: delivery and language | Japanese/English switch on one shared UI, verified glyphs/layouts, clean offline package and rehearsed fallback | Persist the choice; no duplicated screens or dependency on network/system fonts; Simplified Chinese runtime hookup remains later |
| P1: trusted result summary | Existing score/best plus successful hits, on-beat hits, maximum streak, visible painted room and clear Retry | Commit each chart event once; no grades, red Miss total, mandatory ranking, score multiplier or automatic restart |
| P1: forgiving streak | Current and maximum streak derived from accepted event outcomes | Accepted target expiry resets; empty shots, wall paint, held fire and pause do not; a quiet reset must not feel punitive |
| P1: original spray feedback | Coherent custom muzzle spray, projectile liquid motion, impact splat/pop and clearer on-beat/streak accents | Use project-created/attributed assets only. Do not copy Splatoon assets, UI, fonts, weapons, characters, sounds or territory-control rules |
| Optional: cut first | Extra spray variants, milestone 5/10/20 flourishes, extra bob, new-record animation, further art/reflection bake | Cut ornament before any committed counter, language path or baseline reliability check |
| Parallel delivery experiment | A local macOS Universal demo.3 candidate is published beside Windows from the same commit | It must not displace Windows festival acceptance; unsigned/unnotarized output and real-Mac testing remain explicit limits |
| After festival | Song selection, accessibility/calibration settings, multi-game design study, an explorable 3D-map teaching track, controller/co-op, macOS signing/notarization and broader support | Separate small milestones; preserve the accepted fixed-camera mode and make no assumed festival-ready Mac port |
| Exploratory only | Roguelite runs, upgrades, armored/splitter/chain targets | Reassess against the relaxing rhythm identity before committing; not the default product promise |

Round-persistent paint remains required. The creator declined a count cap: no timed fade, oldest-first removal or silent deletion. If measured cost is high, reduce new decorative effects or evaluate paint-preserving rendering changes.

## Remaining schedule and gates

Dates are targets, not completion claims. The creator explicitly accepted a narrower test buffer by expanding the pre-festival commitment. Do not let extra variants or ornament consume October 28–30.

Work through the gates in order. A later calendar date does not waive an earlier gate: if the current-source playtest is not recorded, fix or describe that blocker before adding counters; if feature-freeze acceptance is incomplete, cut optional work rather than moving it into the packaging window.

Current status: the learner has advanced the source portions of Gates B–D together through the existing result-page binding. This does not waive their evidence requirements; complete the focused accounting, expiry, natural-end, Retry and layout checks before treating any of those gates as accepted.

| Date | Main outcome | Evidence / cut rule |
|---|---|---|
| Oct 22 | **Gate A — baseline and interfaces.** Record the creator's no-apparent-issue report with its limits; prepare the licensed Japanese/Latin font and lock the Japanese/English string list | No gameplay change. Confirm one shared UI and persistent language choice as the implementation design |
| Oct 23 | **Gate B — successful-hit accounting.** Teach the event outcome state and one successful-hit increment at the accepted balloon impact | One event cannot hit twice; wall hits and empty shots do not count; Retry returns zero; five ordinary accepted hits score 50 and count five |
| Oct 24 | **Gate C — on-beat subset.** Reuse the existing matched-event judgement and expose an on-beat count only after the hit is accepted | Five accepted hits including two on-beat hits produce 70 points, five successful hits and two on-beat hits; pause and duplicate callbacks change nothing |
| Oct 25 | **Gate D — forgiving streak and result binding.** Add current/maximum streak, then bind the three trusted statistics to the existing ResultsScreen | Expiry resets current, not maximum; wall/empty shots do not reset; Retry clears all round values. Do not create a second result screen |
| Oct 26 | **Gate E — bilingual UI and original spray feedback.** Finish Japanese/English switching, dynamic cue/result strings, glyph/layout checks and the minimum custom muzzle/trail/impact/on-beat/streak presentation set | No copied Splatoon content or territory rules. Preserve collider/root timing; cap transient effects and keep targets/music/cue readable |
| Oct 27 | Observe three unfamiliar players, fix only demonstrated clarity/comfort problems, run complete-song regression and **freeze features** | All committed features need focused checks. Cut extra variants/milestones/ornament first; record missing participant evidence rather than inventing it |
| Oct 28 | Exhibition-PC full-song/retry soak, frame/memory capture, build/extract one immutable candidate and retain the last accepted fallback | At least 30 minutes of actual play and ten consecutive Retries; verify both languages, statistics identities, effects, offline notices and exact hash |
| Oct 29 | Rehearse newcomer instructions, player handoff, volume, restart and backup recovery | Main/fallback launch from fresh extraction. Patch only demonstrated blockers and create a new version if bytes change |
| Oct 30 | Final equipment/package rehearsal and distribution freeze | Verify executable/hash, mouse surface, display, power, audio and operator sheet. No speculative last-minute polish |
| Oct 31 | Exhibit the rehearsed build and collect short anonymous observations | Staff handles handoff/recovery; no untested booth-time changes |
| Nov 1 | Continue only if participation is confirmed | Same tested package; no assumed second-day commitment |

Six implementation/integration dates (October 22–27) provide at most 24–36 hours if fully available, followed by only two candidate/rehearsal days before the October 30 freeze. In a four-hour session, use about 2.5 hours for one small change, one hour for focused play/comparison and 30 minutes for review/checkpoint. Do not combine counters, language and effects into one unreviewed code pass.

### Parallel production lanes through feature freeze

| Lane | October 22–27 outcome | Owner / stop rule |
|---|---|---|
| Core learning | Successful hits → on-beat subset → forgiving streak → existing result-page bindings, one reviewed increment at a time | Learner writes gameplay; stop on the first unresolved accounting/reset regression |
| Presentation | Japanese/English font/copy/layout and one bounded original spray-feedback set | Maintainer prepares assets/tools; learner connects runtime behavior; preserve visual readability and original provenance |
| Delivery | Package preflight, operator runbook, primary/fallback rehearsal and evidence templates | Maintainer prepares; creator accepts the exact package/hardware |
| Verification | Daily focused checks, Oct 27 feature freeze, Oct 28 soak/candidate, Oct 29 rehearsal, Oct 30 distribution freeze | Extra variants and ornament may not borrow this time |

## Post-festival teaching track

This track starts no earlier than November 2, after the Kyudaisai package and exhibition evidence are preserved. It is exploratory rather than a commitment to replace the accepted game. [Game design](GAME_DESIGN.md#inspiration-and-originality-boundary) owns the originality boundary and the condition that exploration must support the relaxing rhythm-and-paint identity.

Teach and review one small system at a time:

1. **Preserve the baseline and study references.** Keep the accepted fixed-camera scene and package recoverable. Compare several games through a short observation matrix covering player goal, movement, camera, spatial guidance, paint feedback and pacing; do not import or imitate protected content.
2. **Build a separate 3D graybox.** Create an experimental scene from simple original primitives. Teach scale, colliders, landmarks and a short navigable route before adding decorative assets or modifying the production scene.
3. **Add movement and camera comfort.** Introduce one movement axis or camera behavior per lesson, with motion-comfort, collision and recovery checks. Do not combine movement, combat and a large map in one pass.
4. **Define paintable-world contracts.** Extend surface detection deliberately, preserve pigment/contact correctness and measure mark growth. General 3D projection, curved surfaces and optimization remain separate experiments.
5. **Place the rhythm loop in space.** Prototype how a complete song, optional timed rewards and readable targets work within one small area. Exploration must not turn missed beats into punishment or obscure the next musical action.
6. **Evaluate one vertical slice.** Compare the experiment with the fixed-camera mode for clarity, comfort, performance and enjoyment. Only an accepted result may inform a later production roadmap; otherwise retain it as a learning scene.

Later lessons may study co-op, upgrades or broader maps only after this slice establishes reliable movement, camera, paint and rhythm behavior. Reference research produces design notes and original prototypes, not a clone of any one game.

## Immediate teaching status

The learner owns Assets/Scripts. Explain one field/helper/call site at a time, let the learner write it, then review saved code before continuing. The creator deferred further paint-scale parameter lessons.

The saved source now contains the first four learner increments: accepted-hit counting, the on-beat subset, forgiving current/maximum streak and a three-line binding on the existing ResultsScreen. Static review confirms the scene reference and prefab geometry. The earlier creator report of no apparent issue predates this combined checkpoint and does not accept it. No new results screen is needed.

1. **Accept the saved statistics increment.** Run the 5-hit/2-on-beat/70-point case, duplicate and wall/empty-shot exclusions, expiry current/max case, natural results and Retry reset. Record the exact revision and device.
2. **Connect Japanese/English switching.** Use the shared translation keys and prepared Japanese/Latin font. Refresh static labels and dynamic cue/feedback/results, persist the choice, then check both languages at target resolutions.
3. **Integrate the original spray-feedback minimum.** Review muzzle spray, liquid projectile motion, impact splat/pop, on-beat accent and one restrained streak pulse as separate increments. Keep logical roots/colliders unchanged.
4. **Run delivery gates.** Complete newcomer, full-song, Retry, performance and exact-package checks before publication.

Milestone flourishes at streak 5/10/20 remain optional even though the base streak is committed. Asset/layout preparation can proceed independently but does not replace the learner's integration or manual judgement.

## Experience acceptance

Use the [short creator playtest and reply template](../Project/EXHIBITION_PLAYTEST.md) for the first current-source report. A Japanese-first printable control card is linked there; font/print acceptance on exhibition hardware remains open.

These are planning targets for a small formative test, not statistical or medical claims.

| Question | Procedure | Acceptance direction |
|---|---|---|
| Is it easy to start? | Three unfamiliar players read the short card without an oral walkthrough | Aim/fire within 30 seconds; explain that shooting can already paint during the original intro |
| Is ordinary play satisfying? | Let a novice hold fire and ignore the bonus | Can finish the song, enjoys pops/colors, no perceived need to restart after misses |
| Is it still a rhythm game? | Invite optional timed taps; ask what the cue and extra accent mean | At least two of three identify that timing earns an extra reward; if not, simplify cues/mix before adding combo |
| Is it relaxing? | Ask for comfort/fun and perceived pressure on 1–5 scales, then ask what caused the answer | Seek comfort/fun at least 4 and pressure at most 2 for most participants; any discomfort or perceived punishment triggers review, not an averaged-away pass |
| Is audio comfortable? | Listen during the densest held-fire section on the intended device | Music remains audible; no piercing or startling repeated accent; agree an appropriate venue volume |
| Is it readable? | Compare early and late paint-heavy passages | Reticle, balloons and timing cue remain distinguishable; no persistent flicker or distracting flash |
| Is the ending welcoming? | Observe results without prompting | Player can find Retry/exit, see their painted room, and describe an enjoyable moment; voluntary retry is a useful signal, not a quota |
| Are statistics correct? | Run Game design's event-accounting and streak cases | Accepted hits counted once, on-beat subset valid, streak/reset correct; no score multiplier or misleading accuracy |
| Is the build dependable? | Fresh extraction, offline launch, full song, pause/focus/retry, record persistence, quit | No exceptions/soft locks/ghost fire/cross-round accumulation; tested fallback available |
| Is it smooth? | Measure clean-player CPU/GPU frame times, long frames, memory, live paint/effects and voices | 2K/120 FPS remains a target. Use and document a tested fallback if needed; no claim from a single Editor screenshot |

## Exhibit session and recovery

Plan a 3–4 minute turn: a brief explanation, the complete 123.429-second song and a short results/paint moment. Do not crop/speed the song or add an upgrade menu to manage the line. Let casual players simply enjoy it; an operator can mention the optional beat reward after they understand aiming.

Default to Japanese and expose an obvious English switch. The Japanese-first card covers mouse aim, held fire, optional countdown-timed bonus, Esc pause and Retry. The prepared M+ 1c font has pinned OFL provenance but still needs exact-glyph rendering and layout checks in the connected UI. The existing name-credit font remains limited to the exact name and ASCII. No account entry, public name leaderboard or compulsory score comparison.

Confirm booth/PC/display/power/audio rules and an operator before rehearsal. Keep the accepted ZIP, rehearsal-tested fallback, hashes/notices and short recovery sheet locally and on backup storage. A prior public build is a fallback candidate until tested on the actual machine.

## Ownership and evidence

The learner owns event outcomes/counters, trigger calls, animation integration and result bindings in Assets/Scripts. The assistant prepares/reviews art, editor tools, fonts/copy/layout, meaningful checks and packaging when requested. Source changes, automated checks, creator reports, unfamiliar-player observations and final-device acceptance are distinct evidence.

This planning request establishes the committed teaching scope but does not authorize the assistant to bypass the learner and write core gameplay. Preserve concurrent work, GUIDs/meta, original audio/attribution/restrictions and recorded image prompts. Documentation is English; player-facing website and game translations are separate. Append commits normally; never rewrite history.

The [desktop release runbook](../Project/PACKAGING_LESSON.md) makes local Windows/macOS builds, immutable ZIP/hash creation, Draft Release staging, target-platform acceptance and publication separate gates. GitHub Actions is a private Windows fallback after the Linux-hosted macOS activation failure; real-Mac testing and any signing/notarization remain separate work and are not a festival commitment.

## Cut order and later direction

Cut extra spray variants, milestone flourishes, extra bob, new-record animation and result ornament first. The base successful/on-beat counters, forgiving streak, Japanese/English switch and minimum custom spray-feedback set remain committed; if any fails correctness at freeze, record the blocker and retain the last accepted package rather than publishing misleading data. Never cut the complete song, reliable input/reset, readable cue, original notices or the October 28–30 verification window.

Do not widen timing windows or change reward values silently to appear more casual: first test the existing generous base-hit path and cue clarity; a judged-rule change needs an explicit measured decision and record-version treatment. Do not add pooling, delete paint or lower resolution speculatively.

After the festival, prioritize approachable song/chart choices, audio/display calibration and comfort options, then device support/co-op and the packaging lesson. More surfaces and content follow measured demand. Roguelite/upgrade experiments are optional branches that must preserve the casual standard-song mode, not an obligation to make the game harder.
