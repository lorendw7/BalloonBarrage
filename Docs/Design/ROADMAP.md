# Development roadmap

Updated: October 27, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior; [Validation](../Project/VALIDATION.md) owns evidence; [README](../../README.md) is the entry point.

## Exhibition product direction

**A casual, relaxing rhythm game about popping balloons and painting a sunlit studio.** The player promise is: aim freely, hold fire for as long as desired, enjoy the visible color trail and catch the beat when it feels good. Rhythm is an audible and visible invitation, not permission to shoot or a requirement to finish.

The creator has approximately 4–6 hours per day. Prioritize comfortable input, a readable musical cue, satisfying ordinary hits, an audible on-beat accent and a welcoming finish. On October 24 the creator expanded the language commitment to Simplified Chinese, Japanese and English. On October 25 the creator requested explicit spray-color switching and paint across all deliberately prepared opaque static studio surfaces before the festival, while retaining ownership of core code. These are bounded stretch targets behind the complete-song and language gates; the existing four flat receivers remain the fallback. Successful-hit count, on-beat-hit count, a forgiving streak and a bounded original ink-spray feedback pass remain in the pre-festival commitment. The streak remains feedback, not a score multiplier or failure condition.

The [official festival site](https://kyudaisai.jp/79th/), checked October 22, lists October 31–November 1, 2026, 09:00–19:00, at Kyushu University Ito Campus. Those are festival-wide hours, not confirmation of this project's booth. The creator plans October 31; booth location, project operating hours, PC count, operator coverage and November 1 participation remain unconfirmed. Revised internal gates: feature freeze October 27, candidate acceptance October 28, distribution freeze October 30. This leaves less verification buffer than the earlier plan, so every implementation day ends with its focused checks.

## October 24 decision board

The next decision is **whether the current build is dependable**, not how many more features can fit. Work from top to bottom; stop optional work when a higher item is open.

| Order | Required outcome | Current status | Next evidence |
|---|---|---|---|
| 1 | Complete-song core flow on the current source | Creator reports no apparent issue; exact revision/hardware details were not supplied | Preserve this baseline while adding one increment at a time; repeat the focused path after each increment |
| 2 | Successful-hit and on-beat-hit counters | Saved in current source at accepted balloon impact; positive existing bonus forms the on-beat subset | Verify 5 hits / 2 on-beat / 70 points, duplicate protection, wall/empty shots and Retry reset in Play Mode |
| 3 | Forgiving current/maximum streak | Saved in current source; accepted hits increment and unresolved target expiry resets current only | Verify expiry/max behavior, pause/wall/empty-shot exclusions and Retry reset in the natural round flow |
| 4 | Existing ResultsScreen summary binding | Saved scene reference and three-line legacy Text layout; creator visually confirmed the populated 2K natural-result screen | Verify Retry, focused values, other supported layouts and the exact package without adding a second result screen |
| 5 | Simplified Chinese/Japanese/English runtime UI | One shared persistent selector, 31 static bindings and three dynamic labels are connected; exact glyph, runtime-format and 1080p/2K Chinese contrast/layout checks pass | Repeat all three connected languages with real pause input, nonzero results/on-beat feedback and Retry in the exact standalone candidate |
| 6 | Original energetic ink-spray feedback | Continuous particle stream, immediate bounded hit sampling, coordinated pigment marks and a short impact pulse are saved | Play Mode-check visual/logical alignment, target readability, wall sweeps, fast aim changes, pause/resume and Retry; fix only demonstrated blockers |
| 7 | Shared-index color switching and prepared-room paint | Shared color selection is saved. Nine prepared static opaque receivers carry the explicit `PaintSurface` contract; 32 broad-coverage prefabs provide four silhouettes across eight pigments | Automated 72-contact, pause, 1,000-mark, natural-song and ten-Retry checks passed; retain the four-receiver/legacy-mark fallback data |
| 8 | Feature freeze | Automated Gate F passed October 27 | Build and test one immutable candidate; do not reopen optional gameplay or ornament |
| 9 | Exact desktop candidate checks | Next | Fresh extraction, archive/manifest/hash verification and a Windows startup/player-log smoke; real-play feel and venue audio are excluded |
| 10 | Primary plus fallback record | Next | Record immutable filenames and hashes and preserve demo.5 as rollback; on-site operator rehearsal is outside this automated pass |

The beat-approach fill is saved and connected to the chart/song clock. The existing results screen now receives final score, local best score, successful hits, on-beat hits and maximum streak, and retains Retry. The new accounting and layout are saved source, not accepted gameplay: focused count, expiry, natural-end, Retry and resolution checks remain open. Accuracy, Miss count and grade remain out of scope.

## Current baseline: do not confuse source, previews and release

| Area | Saved or observed evidence | Still required |
|---|---|---|
| Public package | GitHub prerelease v0.1.0-demo.5 contains Windows x64 and unsigned/unnotarized macOS Universal ZIPs from `0c5db59`; both compiled with zero errors and passed CRC/manifest/font-notice/hash checks; a fresh Windows extraction passed startup and Chinese start/HUD smoke | Preserve as rollback until the new immutable candidate passes automated build/archive/Windows-startup checks |
| Current source | Eight indexed pigments, shared color switching, three broad direct-deposit families, continuous spray without spawned gameplay projectiles, cursor-contact visual length, foreground transparent sorting, forgiving SphereCast hits, short mark-impact pulse and nine explicitly prepared static receivers | Automated Play Mode contact/pause/1,000-mark/natural-song/ten-Retry acceptance passed; clean desktop build, archive and Windows-package smoke remain |
| Current art | Brighter side walls, six-color murals, plaster/mineral floor, multicolor reservoir, author plaque; isolated native renders and component checks | In-play target contrast, latest combined-build load and actual-machine performance; indirect lighting/reflections have not been rebaked |
| Input/feedback | Saved pointer, firing audio, bounded recoil, on-beat feedback, chart-clock-driven approach fill and concentrated continuous spray; creator-reported earlier spot checks predate the final spray envelope/pulse | Tapping versus held-fire listening test, stream/target readability, pause/focus/Retry and wall-sweep checks on final hardware |
| Statistics/results | Successful-hit/on-beat counters, current/maximum streak, reset paths and ResultsScreen binding are saved; creator visually confirmed the populated 2K result layout | Focused 5/2/70, duplicate/expiry, Retry, other-layout and exact-package acceptance before freeze; no missed-event denominator yet |
| Performance | Current automated Editor fixture retained 1,000 marks; forced 2K rendering measured 3.733 ms mean / 6.584 ms p95 and about 5.4 MB additional reported allocation on an RTX 5070 Ti Laptop GPU | Treat as Editor diagnostic evidence, not standalone FPS; physical soak and venue-hardware judgement are excluded from this pass |
| Website | Trilingual source and deployed Pages site describe connected localization, use the current 2K Simplified Chinese Unity render and expose verified demo.5 Windows/macOS download cards | Preserve truthful platform limits and update only from real uploaded release assets |

October 19–20 time included art and learner paint work. Earlier scheduled counter/streak sessions are not treated as completed. Do not compress missed scope into the remaining days.

## Scope and cuts

| Priority | Exhibition outcome | Boundary |
|---|---|---|
| P0: finishable and welcoming | One complete song; free aim and unrestricted-duration held fire; ordinary hits reward; pause/retry/quit reliable | No health, ammunition, reload, heat, elimination, score subtraction, restart-on-miss or color-matching obligation; retain a finite technical hit-sampling cadence |
| P0: music and comfort | First-time players understand the action, hear the track and perceive the beat bonus | Preserve existing chart/0.1-second window/3.1-second cleanup/10–20 scoring until a separate measured tuning decision; do not infer accuracy from held fire |
| P0: delivery and language | Simplified Chinese/Japanese/English switch on one shared UI, verified glyphs/layouts, clean offline package and rehearsed fallback | Persist the choice; no duplicated screens or dependency on network/system fonts |
| P1: trusted result summary | Existing score/best plus successful hits, on-beat hits, maximum streak, visible painted room and clear Retry | Commit each chart event once; no grades, red Miss total, mandatory ranking, score multiplier or automatic restart |
| P1: forgiving streak | Current and maximum streak derived from accepted event outcomes | Accepted target expiry resets; empty shots, wall paint, held fire and pause do not; a quiet reset must not feel punitive |
| P1: original spray feedback | Coherent custom continuous stream, persistent impact splat/pop and clearer on-beat/streak accents | Use project-created/attributed assets only. Do not copy Splatoon assets, UI, fonts, weapons, characters, sounds or territory-control rules |
| P1 stretch: coordinated color and prepared-room paint | One deliberate color choice drives stream, impact/deposits and gun accent; every prepared opaque static studio receiver can accumulate paint | Learner writes runtime code. Exclude glass, UI, balloons, particles, moving roots and unprepared meshes; retain the four-receiver baseline if the stretch misses the October 27 gate |
| Optional: cut first | Extra spray variants, milestone 5/10/20 flourishes, extra bob, new-record animation, further art/reflection bake | Cut ornament before any committed counter, language path or baseline reliability check |
| Parallel delivery experiment | A local macOS Universal demo.5 candidate is published beside Windows from the same commit | It must not displace Windows festival acceptance; unsigned/unnotarized output and real-Mac testing remain explicit limits |
| After festival | Song selection, accessibility/calibration settings, multi-game design study, an explorable 3D-map teaching track, controller/co-op, macOS signing/notarization and broader support | Separate small milestones; preserve the accepted fixed-camera mode and make no assumed festival-ready Mac port |
| Exploratory only | Roguelite runs, upgrades, armored/splitter/chain targets | Reassess against the relaxing rhythm identity before committing; not the default product promise |

Round-persistent paint remains required. The creator declined a count cap: no timed fade, oldest-first removal or silent deletion. If measured cost is high, reduce new decorative effects or evaluate paint-preserving rendering changes.

## Remaining schedule and gates

Dates are targets, not completion claims. The creator explicitly accepted a narrower test buffer by expanding the pre-festival commitment. Do not let extra variants or ornament consume October 28–30.

Work through the gates in order. A later calendar date does not waive an earlier gate: if the current-source playtest is not recorded, fix or describe that blocker before adding counters; if feature-freeze acceptance is incomplete, cut optional work rather than moving it into the packaging window.

Current status: the learner has advanced the source portions of Gates B–E through shared pigment selection and the existing result-page binding. The maintainer connected the shared trilingual table, all nine prepared static opaque receivers and the original eight-pigment broad-coverage art. Automated Gate F passed all 72 surface/pigment contacts, pause persistence, a 1,000-mark 2K Editor diagnostic, natural complete-song settlement and ten Retry resets. The next deliverable is one immutable desktop candidate. Human feel, venue-volume, unfamiliar-player and real-Mac checks were removed from this pass at the creator's request and are explicitly excluded rather than passed.

| Date | Main outcome | Evidence / cut rule |
|---|---|---|
| Oct 22 | **Gate A — baseline and interfaces.** Record the creator's no-apparent-issue report with its limits; prepare licensed Japanese/Latin and Simplified Chinese fonts and lock the initial string list | No gameplay change. Confirm one shared UI and persistent language choice as the implementation design |
| Oct 23 | **Gate B — successful-hit accounting.** Teach the event outcome state and one successful-hit increment at the accepted balloon impact | One event cannot hit twice; wall hits and empty shots do not count; Retry returns zero; five ordinary accepted hits score 50 and count five |
| Oct 24 | **Gate C — on-beat subset.** Reuse the existing matched-event judgement and expose an on-beat count only after the hit is accepted | Five accepted hits including two on-beat hits produce 70 points, five successful hits and two on-beat hits; pause and duplicate callbacks change nothing |
| Oct 25 | **Gate D — forgiving streak/result binding plus presentation repair.** Preserve the learner statistics work; separate the visual spray emitter from the logical muzzle and prepare original tintable VFX art without connecting gameplay | Expiry resets current, not maximum; wall/empty shots do not reset; Retry clears all round values. Held fire must no longer appear to pass through the gun; do not create a second result screen |
| Oct 26 | **Gate E — trilingual UI and coordinated color.** Finish Simplified Chinese/Japanese/English switching, then let one learner-owned selected pigment index update stream, impact/deposits and gun accent | No copied game content or territory rules. Preserve collider/root timing; switching color changes no score or rhythm state; check all three language layouts |
| Oct 27 | **Gate F — prepared-room paint and feature freeze.** Expand only through explicit opaque static receivers and run automated contact/Retry/performance regression | Passed: nine receivers × eight pigments, pause persistence, 1,000 retained marks, natural song end and ten Retry resets. Human observation is excluded at the creator's request; arbitrary curved, moving and transparent surfaces remain post-festival |
| Oct 28 | Exhibition-PC full-song/retry soak, frame/memory capture, build/extract one immutable candidate and retain the last accepted fallback | At least 30 minutes of actual play and ten consecutive Retries; verify both languages, statistics identities, effects, offline notices and exact hash |
| Oct 29 | Rehearse newcomer instructions, player handoff, volume, restart and backup recovery | Main/fallback launch from fresh extraction. Patch only demonstrated blockers and create a new version if bytes change |
| Oct 30 | Final equipment/package rehearsal and distribution freeze | Verify executable/hash, mouse surface, display, power, audio and operator sheet. No speculative last-minute polish |
| Oct 31 | Exhibit the rehearsed build and collect short anonymous observations | Staff handles handoff/recovery; no untested booth-time changes |
| Nov 1 | Continue only if participation is confirmed | Same tested package; no assumed second-day commitment |

Six implementation/integration dates (October 22–27) provide at most 24–36 hours if fully available, followed by only two candidate/rehearsal days before the October 30 freeze. In a four-hour session, use about 2.5 hours for one small change, one hour for focused play/comparison and 30 minutes for review/checkpoint. Do not combine counters, language and effects into one unreviewed code pass.

### Parallel production lanes through feature freeze

| Lane | October 22–27 outcome | Owner / stop rule |
|---|---|---|
| Core learning | Successful hits → on-beat subset → forgiving streak → existing result-page bindings → localization guard/parser → shared pigment index → explicit paint-surface contract, one reviewed increment at a time | Learner writes gameplay; stop on the first unresolved accounting/reset regression |
| Presentation | Simplified Chinese/Japanese/English font/copy/layout, original tintable VFX, gun/muzzle alignment and prepared receiver materials | Maintainer prepares assets/tools; learner connects runtime behavior; preserve visual readability and original provenance |
| Delivery | Package preflight, operator runbook, primary/fallback rehearsal and evidence templates | Maintainer prepares; creator accepts the exact package/hardware |
| Verification | Daily focused checks, Oct 27 feature freeze, Oct 28 soak/candidate, Oct 29 rehearsal, Oct 30 distribution freeze | Extra variants and ornament may not borrow this time |

## Post-festival teaching track

This track starts no earlier than November 2, after the Kyudaisai package and exhibition evidence are preserved. It is exploratory rather than a commitment to replace the accepted game. [Game design](GAME_DESIGN.md#inspiration-and-originality-boundary) owns the originality boundary and the condition that exploration must support the relaxing rhythm-and-paint identity.

Teach and review one small system at a time:

1. **Preserve the baseline and study references.** Keep the accepted fixed-camera scene and package recoverable. Compare several games through a short observation matrix covering player goal, movement, camera, spatial guidance, paint feedback and pacing; do not import or imitate protected content.
2. **Build a separate 3D graybox.** Create an experimental scene from simple original primitives. Teach scale, colliders, landmarks and a short navigable route before adding decorative assets or modifying the production scene.
3. **Add movement and camera comfort.** Introduce one movement axis or camera behavior per lesson, with motion-comfort, collision and recovery checks. Do not combine movement, combat and a large map in one pass.
4. **Define paintable-world contracts.** Follow the [full-scene paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md): prototype one planar RenderTexture receiver, preserve pigment/contact correctness and prove bounded object/memory growth before adding surfaces. Curved receivers remain separate experiments.
5. **Place the rhythm loop in space.** Prototype how a complete song, optional timed rewards and readable targets work within one small area. Exploration must not turn missed beats into punishment or obscure the next musical action.
6. **Evaluate one vertical slice.** Compare the experiment with the fixed-camera mode for clarity, comfort, performance and enjoyment. Only an accepted result may inform a later production roadmap; otherwise retain it as a learning scene.

Later lessons may study co-op, upgrades or broader maps only after this slice establishes reliable movement, camera, paint and rhythm behavior. Reference research produces design notes and original prototypes, not a clone of any one game.

## Immediate teaching status

The learner owns Assets/Scripts and reconfirmed that ownership on October 25. Explain one field/helper/call site at a time, let the learner write it, then review saved code before continuing. The creator deferred further paint-scale parameter lessons.

The saved source contains the learner's statistics, shared pigment and continuous-spray work. The maintained asset layer now supplies an original broad-coverage atlas, 32 eight-pigment mark prefabs and the nine prepared receivers without changing learner-owned scoring, timing or hit cadence. Automated Unity evidence covers all 72 receiver/pigment contacts, pause persistence, 1,000 retained marks, natural complete-song settlement and ten Retry resets. No new results screen or gameplay rule is needed before packaging.

1. **Freeze the saved gameplay.** Do not add optional rules, variants or ornament while packaging.
2. **Build one immutable desktop candidate.** Use a clean worktree from one pushed commit for Windows x64 and macOS Universal.
3. **Verify the exact archives.** Check CRC, runtime manifest, notices, prohibited-file exclusions, launcher/data layout, hashes and a fresh Windows-package startup/player log.
4. **Refresh the trilingual website truthfully.** Show the integrated eight-color, nine-surface automated evidence and preserve the explicit Editor/standalone and unsigned-Mac limits.
5. **Preserve rollback.** Keep demo.5 available until the new prerelease and website have been verified.

Milestone flourishes at streak 5/10/20 remain optional even though the base streak is committed. Asset/layout preparation can proceed independently but does not replace the learner's integration or manual judgement.

## Optional future human acceptance — excluded from this automated pass

The creator asked to cancel checks this environment cannot perform. The following physical-feel, unfamiliar-player, venue-audio and booth-hardware procedures are therefore advisory only and do not block the current automated package pass. They must never be recorded as passed without a real observation. The [short creator playtest and reply template](../Project/EXHIBITION_PLAYTEST.md) remains available if a later on-site session is desired.

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

Default to Japanese and expose obvious Simplified Chinese, Japanese and English controls. The Japanese-first card covers mouse aim, held fire, optional countdown-timed bonus, Esc pause and Retry. Prepared M+ 1c and Noto Sans SC fonts have pinned OFL provenance and source-level coverage checks, but still need exact-glyph rendering and layout checks in the connected UI. The existing name-credit font remains limited to the exact name and ASCII. No account entry, public name leaderboard or compulsory score comparison.

Confirm booth/PC/display/power/audio rules and an operator before rehearsal. Keep the accepted ZIP, rehearsal-tested fallback, hashes/notices and short recovery sheet locally and on backup storage. A prior public build is a fallback candidate until tested on the actual machine.

## Ownership and evidence

The learner owns event outcomes/counters, trigger calls, animation integration and result bindings in Assets/Scripts. The assistant prepares/reviews art, editor tools, fonts/copy/layout, meaningful checks and packaging when requested. Source changes, automated checks, creator reports, unfamiliar-player observations and final-device acceptance are distinct evidence.

This planning request establishes the committed teaching scope but does not authorize the assistant to bypass the learner and write core gameplay. Preserve concurrent work, GUIDs/meta, original audio/attribution/restrictions and recorded image prompts. Documentation is English; player-facing website and game translations are separate. Append commits normally; never rewrite history.

The [desktop release runbook](../Project/PACKAGING_LESSON.md) makes local Windows/macOS builds, immutable ZIP/hash creation, Draft Release staging, target-platform acceptance and publication separate gates. GitHub Actions is a private Windows fallback after the Linux-hosted macOS activation failure; real-Mac testing and any signing/notarization remain separate work and are not a festival commitment.

## Cut order and later direction

Cut extra spray variants, milestone flourishes, extra bob, new-record animation and result ornament first. The base successful/on-beat counters, forgiving streak, trilingual switch and minimum custom spray-feedback set remain committed; if any fails correctness at freeze, record the blocker and retain the last accepted package rather than publishing misleading data. Never cut the complete song, reliable input/reset, readable cue, original notices or the October 28–30 verification window.

Do not widen timing windows or change reward values silently to appear more casual: first test the existing generous base-hit path and cue clarity; a judged-rule change needs an explicit measured decision and record-version treatment. Do not add pooling, delete paint or lower resolution speculatively.

After the festival, prioritize approachable song/chart choices, audio/display calibration and comfort options, then device support/co-op and the packaging lesson. More surfaces and content follow measured demand. Roguelite/upgrade experiments are optional branches that must preserve the casual standard-song mode, not an obligation to make the game harder.
