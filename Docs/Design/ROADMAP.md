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
| After festival | Song selection, accessibility/calibration settings, controller/co-op, macOS packaging lesson | Separate small milestones; no assumed festival port |
| Exploratory only | Roguelite runs, upgrades, armored/splitter/chain targets | Reassess against the relaxing rhythm identity before committing; not the default product promise |

Round-persistent paint remains required. The creator declined a count cap: no timed fade, oldest-first removal or silent deletion. If measured cost is high, reduce new decorative effects or evaluate paint-preserving rendering changes.

## Remaining schedule and gates

Dates are targets, not completion claims. Do not borrow the testing buffer to finish optional features.

| Date | Main outcome | Evidence / cut rule |
|---|---|---|
| Oct 20 | Rebaseline the plan; play one complete current-source round; check ordinary hits, tapping/held fire, dense paint and the new gun | Record concrete comfort/readability issues; confirm booth hardware/audio logistics. Do not add more major art work |
| Oct 21 | Fix the largest input/music/readability issue; teach a successful-hit counter only after the core flow works | Keep one DSP origin and existing scoring. A wall shot or duplicate contact must not count |
| Oct 22 | Add on-beat subset and a minimal positive result summary if stable; tune existing ordinary/on-beat sound and visual contrast | Both casual held fire and optional timed taps must feel worthwhile. If accounting is unreliable, retain score/best and a completion message |
| Oct 23 | Japanese-first instructions/buttons/result text; verify fonts; optionally one short visual-child animation | Prefer readable instructions over squash or combo. No new timing rule or mode |
| Oct 24 | Observe three unfamiliar players if available, including someone who rarely plays rhythm games; make small fixes | **Freeze new features at day's end.** Remove optional effects that obscure targets or feel stressful; if participants are unavailable, record the missing evidence |
| Oct 25 | Exhibition-PC full-song/retry session, pause/focus checks and frame/memory capture | At least 30 minutes of actual play and ten consecutive retries; record how many were full-song runs. Callback loops are not substitutes |
| Oct 26 | Fix only observed blockers; build/extract a clean candidate; rehearse offline/audio/display/save/retry | Repeat affected checks after each fix; no new art systems or gameplay rules |
| Oct 27 | Accept one candidate and a tested fallback | New immutable versioned ZIP/hash/notices, accurate release page/download. Use a reduced feature set if necessary; demo.2 is not automatically hardware-accepted |
| Oct 28–29 | Rehearse newcomer instructions, player handoff, volume, restart and backup recovery | Main/fallback launch from fresh extraction. Patch only demonstrated blockers; use new versions |
| Oct 30 | Final equipment/package rehearsal and distribution freeze | Verify executable/hash, mouse surface, display, power, audio and operator sheet. No speculative last-minute polish |
| Oct 31 | Exhibit the rehearsed build and collect short anonymous observations | Staff handles handoff/recovery; no untested booth-time changes |
| Nov 1 | Continue only if participation is confirmed | Same tested package; no assumed second-day commitment |

Five remaining implementation/tuning dates (October 20–24) provide at most 20–30 hours if fully available; today's elapsed work is not extra capacity. Reserve October 25–26's 8–12 hours for verification/fixes. In a four-hour session, use about 2.5 hours for one small change, one hour for real play/comparison and 30 minutes for review/checkpoint. Extra time goes to evidence, not automatic scope expansion.

## Immediate teaching queue

The learner owns Assets/Scripts. Explain one field/helper/call site at a time, let the learner write it, then review saved code before continuing. The creator deferred further paint-scale parameter lessons.

1. Run the [continuous-fire and paint-loop lesson](CONTINUOUS_FIRE_LESSON.md). Confirm the existing held input has no resource or forced downtime and report sound/readability/performance observations before changing its cadence.
2. Address beat-cue clarity. Follow the [beat-cue handoff](../Art/BEAT_CUE_HANDOFF.md): improve existing cue readability, then teach one serialized graphic field. The approach graphic is prepared separately; dynamic timing and target association remain learner work.
3. If stable, teach the current-round successful-hit field and once-only accepted-impact increment. Verify reset and wall/duplicate exclusions.
4. Add the on-beat subset through the existing matched-event bonus path. Verify five hits including two on-beat hits give 70 points, not seven hits.
5. Bind only the accepted counters to results, with a friendly completion line and clear Retry. Keep the painted room visible.
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

The Windows packaging lesson is now documented in [Packaging a Windows demo](../Project/PACKAGING_LESSON.md). It separates the Unity build, fresh-extraction acceptance, immutable ZIP/hash creation and publication. macOS modules, architecture, signing/notarization and real-Mac testing remain a later lesson, not a festival commitment.

## Cut order and later direction

Cut streak/milestones, extra bob and result ornament first; then entrance/squash or new-record flourish. If new counters are not trustworthy, ship the proven score/best plus welcoming completion copy. Never cut the complete song, reliable input/reset, readable cue, original notices or the October 25–27 verification window.

Do not widen timing windows or change reward values silently to appear more casual: first test the existing generous base-hit path and cue clarity; a judged-rule change needs an explicit measured decision and record-version treatment. Do not add pooling, delete paint or lower resolution speculatively.

After the festival, prioritize approachable song/chart choices, audio/display calibration and comfort options, then device support/co-op and the packaging lesson. More surfaces and content follow measured demand. Roguelite/upgrade experiments are optional branches that must preserve the casual standard-song mode, not an obligation to make the game harder.
