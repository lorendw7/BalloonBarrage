# Development roadmap

Updated: October 21, 2026 (Asia/Tokyo). This is the single delivery plan. [Game design](GAME_DESIGN.md) owns behavior; [Validation](../Project/VALIDATION.md) owns evidence; [README](../../README.md) is the entry point.

## Exhibition product direction

**A casual, relaxing rhythm game about popping balloons and painting a sunlit studio.** The player promise is: aim freely, hold fire for as long as desired, enjoy the visible color trail and catch the beat when it feels good. Rhythm is an audible and visible invitation, not permission to shoot or a requirement to finish.

The creator has approximately 4–6 hours per day. Prioritize comfortable input, a readable musical cue, satisfying ordinary hits, an audible on-beat accent and a welcoming finish. A score chase is optional. Do not make combo maintenance, accuracy, upgrades or content volume the exhibition's main attraction.

The [official festival page](https://kyudaisai.jp/79th/en/about/), checked October 20, lists October 31–November 1, 2026 at Kyushu University Ito Campus. The creator plans October 31; booth, hours, PC count and November 1 participation remain unconfirmed. Internal gates: feature freeze October 24, candidate October 27, distribution freeze October 30.

## Current baseline: do not confuse source, previews and release

| Area | Saved or observed evidence | Still required |
|---|---|---|
| Public package | Windows x64 non-Development v0.1.0-demo.2; complete 123.429-second song, 93 events, 10/20 rewards, three-color paint, local best score | Fresh-extraction core-flow and listening acceptance on the exhibition PC |
| Current source | Eight pigments, matching pop/projectile marks, six pattern families, random pattern/rotation/scale | Latest combined-source complete-song and clean-package checks; configurable-angle revision has source review only |
| Current art | Brighter side walls, six-color murals, plaster/mineral floor, multicolor reservoir, author plaque; isolated native renders and component checks | In-play target contrast, latest combined-build load and actual-machine performance; indirect lighting/reflections have not been rebaked |
| Input/feedback | Saved pointer, firing audio, bounded recoil and on-beat cue; creator-reported earlier spot checks | Tapping versus held-fire listening test, pause/focus/retry on final hardware |
| Statistics/results | Score and local best score saved; no hit/on-beat counters or streak implementation found in current source review | Teach only the minimum useful addition; do not promise counters or combo as shipped |
| Performance | Earlier instrumented full song with 610 marks; ten diagnostic reloads; earlier Editor snapshots | At least 30 minutes of real full-song/retry use, memory/object trends, target-PC frame times |
| Website | Latest native room/gun previews published separately from demo.2 | Publish only tested package claims; web checks are not Unity acceptance |

October 19–20 time included art and learner paint work. Earlier scheduled counter/streak sessions are not treated as completed. Do not compress missed scope into the remaining days.

## Scope and cuts

| Priority | Exhibition outcome | Boundary |
|---|---|---|
| P0: finishable and welcoming | One complete song; free aim and unrestricted-duration held fire; ordinary hits reward; pause/retry/quit reliable | No health, ammunition, reload, heat, elimination, score subtraction, restart-on-miss or color-matching obligation; retain a finite technical projectile cadence |
| P0: music and comfort | First-time players understand the action, hear the track and perceive the beat bonus | Preserve existing chart/0.1-second window/3.1-second cleanup/10–20 scoring until a separate measured tuning decision; do not infer accuracy from held fire |
| P0: delivery | Short Japanese instructions, verified glyphs, clean offline package and rehearsed fallback | Static Japanese exhibit language is enough; no new language-switching system |
| P1: warm ending | Existing score/best score plus a positive completion line and visible painted room; clear Retry | No grades, red Miss total, mandatory ranking or automatic restart; do not block exit or handoff |
| P1: simple success summary | Successful-hit count, then on-beat subset if the once-only outcome/reset checks pass | Results-first, not another live HUD requirement; internal unanswered-event accounting is diagnostic, not a public failure measure |
| P2: one tactile refinement | Tune existing pop/ring/sound first; add one visual-child entrance or squash only if time permits | No root/collider animation, global hit stop, mandatory camera shake or competing effect labels |
| Optional: cut first | Forgiving streak, milestone flourishes, extra bob, new-record animation, further art/reflection bake | Only after P0/P1 acceptance; none is required for the festival |
| Parallel delivery experiment | A local macOS Universal candidate may be built and staged beside Windows from the same commit | It must not displace Windows festival acceptance; unsigned/unnotarized output and real-Mac testing remain explicit limits |
| After festival | Song selection, accessibility/calibration settings, controller/co-op, macOS signing/notarization and broader support | Separate small milestones; no assumed festival-ready Mac port |
| Exploratory only | Roguelite runs, upgrades, armored/splitter/chain targets | Reassess against the relaxing rhythm identity before committing; not the default product promise |

Round-persistent paint remains required. The creator declined a count cap: no timed fade, oldest-first removal or silent deletion. If measured cost is high, reduce new decorative effects or evaluate paint-preserving rendering changes.

## Remaining schedule and gates

Dates are targets, not completion claims. Do not borrow the testing buffer to finish optional features.

Work through the gates in order. A later calendar date does not waive an earlier gate: if the current-source playtest is not recorded, fix or describe that blocker before adding counters; if feature-freeze acceptance is incomplete, cut optional work rather than moving it into the packaging window.

| Date | Main outcome | Evidence / cut rule |
|---|---|---|
| Oct 21 | **Gate A — lock the current core.** Accept 6 shots per second, choose 6 or 8 muzzle particles, verify new paint covers old paint/background art, then finish one complete song with pause/resume and Retry | Paint-order source compiles in isolation; visual overwrite and complete-song behavior still require the creator's report. Fix only the strongest input/music/readability failure. Do not add counters until this gate is clean |
| Oct 22 | **Gate B — smallest results improvement.** Teach a successful-hit counter; add the on-beat subset and a positive result line only if each once-only/reset check passes | Keep one DSP origin and existing scoring. Wall shots and duplicate contacts must not count. If accounting is unreliable, ship score/best plus the completion line |
| Oct 23 | Japanese-first instructions/buttons/result text; verify fonts; optionally one short visual-child animation | Prefer readable instructions over squash or combo. No new timing rule or mode |
| Oct 24 | Observe three unfamiliar players if available, including someone who rarely plays rhythm games; make small fixes | **Freeze new features at day's end.** Remove optional effects that obscure targets or feel stressful; if participants are unavailable, record the missing evidence |
| Oct 25 | Exhibition-PC full-song/retry session, pause/focus checks and frame/memory capture | At least 30 minutes of actual play and ten consecutive retries; record how many were full-song runs. Callback loops are not substitutes |
| Oct 26 | Fix only observed blockers; build/extract a clean candidate; rehearse offline/audio/display/save/retry | Repeat affected checks after each fix; no new art systems or gameplay rules |
| Oct 27 | Accept one candidate and a tested fallback | New immutable versioned ZIP/hash/notices, accurate release page/download. Use a reduced feature set if necessary; demo.2 is not automatically hardware-accepted |
| Oct 28–29 | Rehearse newcomer instructions, player handoff, volume, restart and backup recovery | Main/fallback launch from fresh extraction. Patch only demonstrated blockers; use new versions |
| Oct 30 | Final equipment/package rehearsal and distribution freeze | Verify executable/hash, mouse surface, display, power, audio and operator sheet. No speculative last-minute polish |
| Oct 31 | Exhibit the rehearsed build and collect short anonymous observations | Staff handles handoff/recovery; no untested booth-time changes |
| Nov 1 | Continue only if participation is confirmed | Same tested package; no assumed second-day commitment |

Four remaining implementation/tuning dates (October 21–24) provide at most 16–24 hours if fully available. Reserve October 25–26's 8–12 hours for verification/fixes. In a four-hour session, use about 2.5 hours for one small change, one hour for real play/comparison and 30 minutes for review/checkpoint. Extra time goes to evidence, not automatic scope expansion.

### Parallel production lanes through feature freeze

| Lane | October 21–24 outcome | Owner / stop rule |
|---|---|---|
| Core learning | Paint overwrite acceptance, full-song baseline, then at most one results or beat-cue increment at a time | Learner writes gameplay; stop on the first unresolved regression |
| Presentation | Japanese-first static copy/font checks and only already-prepared cue/art hookups that survive the baseline | Maintainer prepares assets/tools; no new rule or mode |
| Delivery | Package preflight, operator runbook, primary/fallback rehearsal and evidence templates | Maintainer prepares; creator accepts the exact package/hardware |
| Verification | Oct 24 feature freeze, Oct 25 soak/retries, Oct 26 blocker fixes, Oct 27 immutable candidate | No optional feature may borrow this time |

## Immediate teaching queue

The learner owns Assets/Scripts. Explain one field/helper/call site at a time, let the learner write it, then review saved code before continuing. The creator deferred further paint-scale parameter lessons.

October 21 direction report: the creator noticed no obvious baseline issue and requested a clearer paint-sticker beat cue plus a more energetic spray feel. The live cue's presentation-only asset pass is prepared. The creator subsequently reported the firing rate decided, and the saved PrototypeScene confirms 6 shots per second; this is user-reported tuning acceptance rather than an automated playtest. The learner has now saved monotonic Sprite sorting for direct-impact marks plus an `AfterSceneLoad` reset for Retry, and an isolated compile has zero errors. Visual acceptance of later paint over earlier paint/background graffiti and the short full-song baseline remain pending; compilation is not visual acceptance. The creator next requested result-page and beat-cue improvements. Extend the already-bound `ResultsScreen` rather than creating a duplicate, and integrate the prepared cue graphic one learner increment at a time.

1. **Observe before editing.** Run the [continuous-fire and paint-loop lesson](CONTINUOUS_FIRE_LESSON.md), then continue to the end of the same song using the [short exhibition playtest](../Project/EXHIBITION_PLAYTEST.md). Report the strongest issue; do not change several Inspector values during the observation.
2. **Connect the prepared cue safely.** If held fire, music, pause or Retry fails, diagnose that single failure first. Otherwise use the [beat-cue handoff](../Art/BEAT_CUE_HANDOFF.md) and teach one serialized graphic field before any scene hookup or timing formula.
3. **Count accepted hits once.** Teach one current-round successful-hit field and increment it only at the accepted balloon outcome. Verify reset plus wall-shot and duplicate-contact exclusions before adding another statistic.
4. **Count the on-beat subset.** Reuse the existing matched-event bonus path. Verify five hits including two on-beat hits give 70 points, five successful hits and two on-beat hits—not seven successful hits.
5. **Extend the existing result page with accepted data.** Bind the verified counters to the current score/best-score/Retry screen with a friendly completion line. Keep the painted room visible. If either counter is unreliable, omit it rather than show misleading data; do not build a second result screen.
6. Tune the existing ordinary/on-beat burst and sound; add at most one visual-child animation if it improves feel.
7. Complete Japanese copy, unfamiliar-player trials and package gates.

A streak is no longer a prerequisite or the next automatic lesson. If requested after these checks, use the optional rules in Game design; defer it if the remaining schedule is tight. Asset/layout preparation can proceed independently but does not replace the learner's integration or manual judgement.

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
| Are statistics correct if included? | Run Game design's event-accounting cases | Accepted hits counted once, on-beat subset valid, reset correct; no misleading accuracy |
| Is the build dependable? | Fresh extraction, offline launch, full song, pause/focus/retry, record persistence, quit | No exceptions/soft locks/ghost fire/cross-round accumulation; tested fallback available |
| Is it smooth? | Measure clean-player CPU/GPU frame times, long frames, memory, live paint/effects and voices | 2K/120 FPS remains a target. Use and document a tested fallback if needed; no claim from a single Editor screenshot |

## Exhibit session and recovery

Plan a 3–4 minute turn: a brief explanation, the complete 123.429-second song and a short results/paint moment. Do not crop/speed the song or add an upgrade menu to manage the line. Let casual players simply enjoy it; an operator can mention the optional beat reward after they understand aiming.

Use a Japanese-first card with mouse aim, held fire, optional countdown-timed bonus, Esc pause and Retry. Check a Japanese-capable font: the existing name-credit font only covers the exact name and ASCII. No account entry, public name leaderboard or compulsory score comparison.

Confirm booth/PC/display/power/audio rules and an operator before rehearsal. Keep the accepted ZIP, rehearsal-tested fallback, hashes/notices and short recovery sheet locally and on backup storage. A prior public build is a fallback candidate until tested on the actual machine.

## Ownership and evidence

The learner owns event outcomes/counters, trigger calls, animation integration and result bindings in Assets/Scripts. The assistant prepares/reviews art, editor tools, fonts/copy/layout, meaningful checks and packaging when requested. Source changes, automated checks, creator reports, unfamiliar-player observations and final-device acceptance are distinct evidence.

No new gameplay is authorized by this planning request. Preserve concurrent work, GUIDs/meta, original audio/attribution/restrictions and recorded image prompts. Documentation is English; player-facing website and game translations are separate. Append commits normally; never rewrite history.

The [desktop release runbook](../Project/PACKAGING_LESSON.md) makes local Windows/macOS builds, immutable ZIP/hash creation, Draft Release staging, target-platform acceptance and publication separate gates. GitHub Actions is a private Windows fallback after the Linux-hosted macOS activation failure; real-Mac testing and any signing/notarization remain separate work and are not a festival commitment.

## Cut order and later direction

Cut streak/milestones, extra bob and result ornament first; then entrance/squash or new-record flourish. If new counters are not trustworthy, ship the proven score/best plus welcoming completion copy. Never cut the complete song, reliable input/reset, readable cue, original notices or the October 25–27 verification window.

Do not widen timing windows or change reward values silently to appear more casual: first test the existing generous base-hit path and cue clarity; a judged-rule change needs an explicit measured decision and record-version treatment. Do not add pooling, delete paint or lower resolution speculatively.

After the festival, prioritize approachable song/chart choices, audio/display calibration and comfort options, then device support/co-op and the packaging lesson. More surfaces and content follow measured demand. Roguelite/upgrade experiments are optional branches that must preserve the casual standard-song mode, not an obligation to make the game harder.
