# Game design

Updated: October 18, 2026. Delivery order, ownership and acceptance live in the [roadmap](ROADMAP.md); source and download details live in [Music and charts](MUSIC_AND_CHARTS.md). Exhibition experience additions below are planned, not implemented features of v0.1.0-demo.2.

## Confirmed direction

BalloonBarrage combines free shooting, balloon crowds, beat bonuses, and roguelite variety in a warm graffiti studio. Players can fire at any time, including held fire. Accurate rhythm earns an additional score or damage benefit; it does not unlock permission to shoot. The learner implements gameplay.

Windows solo is the first acceptance baseline. A fixed camera and shared gallery avoid movement and split-screen complexity. The selected later co-op remains P1 keyboard/mouse plus P2 one gamepad; the creator owns one Xbox and one Beitong controller. Controller/co-op integration remains deferred beyond the exhibition experience work. Mobile, online multiplayer, independent dual mice and split-screen are outside current scope.

## Whole-game loop and mode boundaries

The accepted foundation is title/instructions → one complete song → results → retry/title. Keep free aiming, optional held fire, musical bonuses and visible paint feedback readable throughout that loop. The intended later game adds song/difficulty selection and local records; a separate run mode can connect complete-song stages through upgrade choices. These additions remain proposals until implemented and accepted through the roadmap.

| Mode | Intended experience | State and scoring boundary |
|---|---|---|
| Exhibition / standard song | One selected complete song, initially the prepared Goin' Under chart | Existing base/on-beat score rules; no run upgrades; complete-song end owns settlement |
| Local co-op | Two independently controlled reticles in the same studio and song | Shared score and one consumed target outcome; projectile player identity supports later contribution statistics |
| Roguelite run | Short sequence of complete-song stages with a small upgrade pool | Upgrade choices between songs; run state resets on retry; records separated from standard songs |
| Practice / free paint | Possible later place to learn aim or decorate without score pressure | Separate from chart accuracy and competitive records; not a prerequisite for the first release |

Color, sound, light and paint should make an accepted action easier to understand. They must not silently alter timing rules, move the logical muzzle through the environment or obscure the next target. Settings, localization and records support the accepted loop rather than requiring an online service. The [roadmap](ROADMAP.md) owns milestones, dependencies, priorities and release gates.

## Exhibition slice

The exhibition foundation is one complete-song rhythm shooting demo using Goin' Under / Beginner, about 123.429 seconds and 93 prepared events. Studio music belongs to interfaces; gameplay uses the full non-looping song. Saved v0.1.0-demo.2 includes DSP timing, 0.8-second previews, target binding/cleanup, firing-time matched-target 10/20 rewards, the separate-index NEXT cue, enlarged same-color burst, golden accent and selected 1.45-second reward text. Reticle, audio, bounded recoil, pause/results/retry, four-surface persistent paint and local records are saved. Detailed evidence and limitations live in Validation; clean exhibition-PC listening, readability and stability remain open. Preserve complete-song end and reset score, event state, audio origin and round objects on retry.

The saved teal latex-and-paint burst is connected. Additional tactile feedback follows acceptance of the rhythm loop: independent gentle visual bobbing, a short entrance expansion, a brief hit squash, bounded gun recoil and round-persistent surface paint from projectiles and balloon pops. Their intended behavior is specified below. Delivery order and cutoffs live in the roadmap.

The demo is accepted only when music, beat cue, shot judgement, scored impact, pause and retry work together in a Windows build. Roguelite upgrades and additional tracks follow this exhibition milestone. Do not advertise planned rhythm features as saved implementation.

## Required rhythm loop

Retain StudioDaylight_Loop.wav as interface music. Start gameplay with the downloaded Goin' Under by NegaRen and its dance-single Beginner chart by Fraxtil: constant 210 BPM, source OFFSET 0, 93 tap-row events containing 95 original lane taps. The complete audio is 5,443,200 samples at 44,100 Hz (123.428571 seconds), without looping or a 90-second crop. The first chart event is at beat 32 / about 9.143 seconds; the last is at beat 416 / about 118.857 seconds. Preserve the intro and ending, and settle only at the song's end.

Use the prepared importable audio, non-looping AudioSource prefab and derived event JSON documented in [Music and charts](MUSIC_AND_CHARTS.md). First teach menu/game playback switching, then scheduled audio with one DSP origin for chart events, timer and settlement. Keep first-beat offset and signed input calibration explicit. The [first audio lesson](RHYTHM_FIRST_LESSON.md) is a foundation exercise, not chart judgement. Do not substitute an arbitrary constant beat grid for the actual chart.

Saved solo defaults: a hit awards 10 points; an accepted shot within 100 milliseconds of its aimed target's chart event can add 10 points if it hits that event's balloon. PlayerShooter reads song time after its fire gates; InitializeShot carries the judgement result and target event id. Collision checks the on-beat flag, nonnegative id and matching target id before passing the bonus to Balloon. Its single-hit guard resolves base plus bonus once. Explicit player identity is still required before co-op integration. Off-time or mismatched-target hits retain base points. Window and reward tuning remain subject to listening/playtests; user-reported 10/20 scoring is not complete-song acceptance.

Show a small beat cue and a timing ring/countdown on each chart-driven target, plus readable timing feedback. Preview the next event sufficiently early to aim; the saved initial lead is 0.8 seconds, subject to playtest tuning. The creator requested removing the fixed three-target cap. Continuous random refill is disabled; each prepared event calls Spawn once, with no active-count gate. Saved cleanup retains unanswered targets until the event time plus a configurable delay, default 3.1 seconds at the creator's request, then deactivates/destroys without scoring. This retention is separate from the proposed timing window and does not yet count a formal Miss. Bound concurrent targets through lifecycle cleanup rather than skipping chart events at a count cap. Merge simultaneous source lanes into one target event; lane metadata is provenance, not automatic world coordinates. Keep each event playable once and mark unanswered events missed after the accepted window. Judge when a shot passes input/cadence gates and fires, so travel distance does not shift judgement; retain the matched target/event identity until impact. A timed shot already in flight needs a short impact-resolution grace bounded by projectile lifetime, rather than expiring its target before arrival. Define firstBeatOffset relative to scheduled start and positive inputCalibration as an addition to shot time. Source StepMania offset is converted with the opposite sign.

Held-fire cadence can repeatedly miss the beat even when the player starts accurately. Compare manual taps with held fire during playtests before choosing bonus and combo rules. Keep baseline free shooting useful, provide visible timing feedback, and tune accessibility independently of damage.

Pause/resume and retry must restore song, timer and chart scheduling together. Derive all gameplay timing from scheduled audio origin; stop the song and show results at the complete audio duration, then return to interface music. Measure latency on the exhibition PC. This prepared constant-tempo chart is the first supported subset; a general SSC importer, tempo changes, holds and mines are later work.

October 19 saved palette expansion: eight balloon pigments, eight random per-shot projectile pigments and matching surface marks are now configured. Balloon explosions retain the balloon pigment; direct surface impacts retain the projectile pigment. Color matching is not a scoring condition. See the [palette guide](../Art/BALLOON_COLOR_LESSON.md) for indexed assets, provenance and verification limits. This expansion is not in the public v0.1.0-demo.2 package.

## Exhibition experience design

**Planning status, October 18:** the creator prioritizes gameplay feel and effects and can spend approximately 4–6 hours per day. Deepen the existing solo loop with a reason to sustain a good run and replay. Calendar, cuts and ownership live in the [roadmap](ROADMAP.md). This section defines proposed behavior, not saved code.

### Player promise and pacing

The intended rhythm is aim → fire → immediate accepted hit → pigment burst/paint → readable success/streak response → next target. On-beat success has a recognizable extra accent; ordinary hits still feel worthwhile. Preserve the unshortened song and roughly nine-second first-event intro. Players can already shoot/paint during the intro; explain this rather than adding off-chart tutorial targets.

After the song, show a concrete hit/streak/record result that suggests another attempt. Do not add health, ammunition, elimination, score multipliers, mid-song upgrades, random target rules or an online leaderboard in this slice.

### Event outcomes and statistics — planned

Use the loaded chart's valid event identities as the denominator: 93 for this chart, not shots, paint marks or original lane taps. Each event starts unresolved and commits one terminal outcome: ordinary hit, on-beat hit or missed/unanswered event. A shared accepted resolution keeps score, counters and streak consistent and retains the existing target hit guard. Unknown/unbound ids do not contribute to chart statistics; duplicate outcomes cannot emit another success/effect.

| Value | Definition |
|---|---|
| Successful hits | Ordinary plus on-beat hits, counted only after an actual accepted scored impact |
| On-beat hits | A subset of successful hits: the existing firing-time judgement reaches its matching event and earns the saved bonus |
| Ordinary hits | Successful chart hits without that matching bonus, including off-time/mismatched-event shots |
| Missed events | Unresolved targets reaching the existing cleanup deadline; at natural settlement finalize any remaining chart events once before showing the summary |
| Maximum streak | Highest current-round hit streak; initially not a separate persistent record |

Preserve complete-song end, 3.1-second cleanup and two-second projectile lifetime. A shot fired on time is not yet a successful hit. Surface hits/missed projectiles are not missed chart events. Quit is not a completed round and must not fabricate full-song statistics. Pause commits no new outcomes. Retry resets round statistics/outcomes while retaining the best-score key/value. Do not extend settlement or retune timing as part of these counters.

At a natural completed round require `successfulHits + missedEvents = chartEventCount`, `onBeatHits <= successfulHits`, and `ordinaryHits = successfulHits - onBeatHits`. With unchanged rewards, `finalScore = 10 * successfulHits + 10 * onBeatHits`. Five hits including two on-beat hits mean three ordinary hits and 70 points, not seven hits. No accuracy percentage or S/A/B grade is required.

### Forgiving hit streak — planned

- Start current and maximum streak at zero. Accepted ordinary/on-beat event hits increment current and update maximum, in actual resolution order rather than trigger-pull order.
- Accepted unanswered-target expiry resets current streak. Merely passing the scheduled beat does not: preserve the saved cleanup/grace behavior.
- Empty shots, surface paint, ordinary off-beat hits, held fire and pause do not independently break a streak.
- Duplicate Hit, expiry and settlement callbacks cannot advance/reset an already resolved event again. Results show maximum streak rather than a misleading current streak after final missed-event resolution.
- No streak score multiplier: existing best-score rules/key remain comparable. Any later reward/window change needs a separate scoring-version decision.

Acceptance example: three hits give current/max 3; an unanswered-event expiry gives current 0/max 3; the next hit gives current 1/max 3. Duplicate Hit and surface-only shots do not change these outcomes twice. Natural results/retry must satisfy the count identities above.

### Feedback hierarchy and trial tuning — planned

These are starting trials, not accepted final values. Improve silhouette, timing and contrast using existing assets before increasing counts.

| Moment | Intended response | Guardrail |
|---|---|---|
| Accepted fire | Existing reticle, sound and bounded gun recoil | No compulsory camera shake or feedback on rejected input |
| Arrival | Visual child about 85% → 103% → 100% scale over 0.20 seconds | Fixed root Rigidbody/collider; verify visual/hit alignment and remove overshoot if needed |
| Ordinary hit | Immediate outcome/score lock and collider disable, then about 0.06-second visual-child squash; independent pigment burst/deposits | Trigger once; delayed visual destruction cannot leave a live target or duplicate score |
| On-beat hit | Matching pigment plus clearer small gold ring/sparkle and an optional brief audio accent | Initially preserve existing 35% burst enlargement and selected 1.45-second reward text; no full-screen flash or extra paint queries |
| Streak | One small count with a restrained approximately 0.12-second scale pulse | No text object spawned per hit; keep aim/next-event cue visible |
| Milestone | Trial accents at streak 5, 10 and 20 using existing palette/effects | Replace/retrigger one slot, never queue labels or overlapping accent voices; optional polish is cut first |
| Results/new record | Final score, best score, hits/event total, on-beat subset, maximum streak; one strict-improvement emphasis | Determine new-record status before updating storage; save once at settlement; ties/lower results are not new records |

Animate explicit visual children. Do not animate the dynamic root or stop global time for hit stop. Logical muzzle, aim, collisions and music remain independent. Moving the burst to the exact projectile hit point is a later refinement if it needs another API/call-site change; it must not delay accepting a simpler squash.

Audition sound against music during dense held fire. Keep the accepted base shot audible without masking the track. A new on-beat accent is brief with bounded overlap; do not change music speed/pitch or duck the whole track by default. Retain at most 32 particles per pop and the two-second transient fallback until a measured revision is accepted.

Keep persistent paint through results, without a count cap, timed fade or oldest-first removal. Preserve pigment and actual contact on the four accepted flat receivers. Fix flicker/floating marks/target contrast before adding furniture or curved projection. Extra bob, richer milestone effects and result ornaments are optional cuts.

### Exhibit communication and acceptance — planned

Use concise Japanese-first instructions with checked glyphs: mouse aim, held fire, fire near countdown zero for a matching-target bonus, Esc pause and Retry. Static exhibit copy is sufficient; runtime language selection is not required. The small author-credit font does not cover Japanese instructions.

Observe unfamiliar players before adding more effects. Ask whether the bonus is recognizable, the next target stays visible against paint, and a concrete replay goal is apparent. Change one parameter at a time. Native layout checks, synthetic outcomes, manual feel reports and clean-player measurements remain distinct evidence. All statistics, streaks, entrance/squash and new-record flourishes in this section remain unimplemented until saved and accepted.

## Feedback and aiming targets

The pointer-following reticle, cursor lifecycle and matching paint-shot audio are saved, with creator-reported spot playtests passed. Cosmetic recoil is also saved: cache the resting local pose in Awake, apply a bounded 0.035 m backward displacement and 6-degree local pitch per accepted shot, and recover in LateUpdate with exponential smoothing at rate 18. Pause holds recovery. ResetGunPose restores the cached visual pose immediately while not playing and on disable. The creator reports recoil and settlement/retry reset passed, followed by no apparent issue in the latest start/pause/resume spot check. These reports do not establish latest full-song or standalone acceptance. Reticle visibility follows active/unpaused play while the system pointer returns for menus, pause, results and focus loss. Short shot audio plays below the music only after accepted firing; bound overlapping voices. Recoil moves only an explicit weapon visual child and restores its resting pose, preserving the camera, aiming root, muzzle and projectile direction. Verify held fire, pause and retry before including these in the exhibition build. See the roadmap for teaching order.

Projectile marks and matching pop deposits are saved on MainWall, Floor, LeftWall and RightWall, persisting through results and clearing on retry. Isolated geometry/lifecycle checks and an instrumented full-song trial support this checkpoint; human feel/readability and exhibition-PC acceptance remain separate. No paint-count limit is planned. Entrance/hit animation and the experience additions above remain planned. Keep Rigidbody/collision on the root and cosmetic transforms on explicit visual children.

| Effect | Proposed implementation and initial tuning | Acceptance |
|---|---|---|
| Gentle balloon motion | Reduce initial root speed from 0.6 toward 0.25 for an aiming trial. Later give each visual child a random sine phase and about 0.03–0.05 world-unit bob amplitude at 0.5–0.7 Hz, converting to parent local units as needed. | Three targets remain easy to aim at; visual/collider alignment stays inside the measured hit margin; contacts still bounce. |
| Entrance expansion | Animate only the visible model from about 85% scale to 103%, then settle at 100% over roughly 0.2 seconds. The existing root-scaling BalloonEntrance is disabled; adapt it before reuse. | No changing Rigidbody/collider scale, abrupt overlap or drifting final scale. |
| Hit squash and burst | Lock the existing hit guard and disable colliders immediately; show a roughly 0.06-second visual squash before destroying the target. Carry the projectile's hit point so the independent burst appears at impact. | Score resolves once; the squash is visible before destruction; the burst survives the target and pauses/cleans up correctly. |
| Gun recoil (saved tuning) | Apply 0.035 units backward and -6 degrees local pitch from the cached visual rest pose; recover with exponential smoothing rate 18. Reset immediately outside play/on disable. | Aim root, projectile direction and logical muzzle stay stable; held fire does not accumulate displacement or shake the camera. |
| Surface paint | Saved projectile/pop deposits on four accepted flat receivers persist through results and clear on scene retry. A future title-return route must also clear them. No count cap or timed fading. | Correct contact, no mark colliders, readable targets and no cross-round accumulation; human/target-PC acceptance remains open. |

Existing PaintPop variants emit at most 32 particles per burst, have short non-looping lifetimes and a two-second fallback cleanup. Retain bounded firing/refill, measure concurrent effects and only add pooling or further effects if measured object counts/frame times justify them. If behind schedule, defer entrance/hit squash, extra surfaces and additional art while protecting the playable rhythm loop.

## Post-festival rhythm/roguelite prototype

Proposed first slice: about three complete-song stages, initially reusing one approved song if necessary, a small pool that can offer three eligible upgrades, and a complete run-results/retry loop. Start with normal one-hit balloons, then introduce one clearly signaled armored or bounded splitter/chain rule at a time. A larger library, elite encounters and additional combinations follow playtesting rather than defining the minimum prototype.

Musical encounter phases may follow authored phrases within a song, but the audio and chart clock continue uninterrupted. Show one-of-three upgrade selection between complete-song stages. Do not pause a song mid-phrase to offer an upgrade menu or treat randomly inserted targets as original chart notes. The exhibition standard-song mode remains available without run upgrades.

Candidate upgrades include cadence, spread, piercing, chains, area burst, freeze, critical hits, and shields. Define caps, eligible offers, ownership, and chain termination before adding combinations. Reset run upgrades on retry; initially persist only settings and best scores. A recorded random seed may aid debugging without promising cross-version determinism.

The initial run can end after its authored stages with a final score; health, escaped-target pressure and elimination are later design proposals, not existing timed-challenge rules. Secondary projectiles/chains must not duplicate the primary event's rhythm reward. Define co-op ownership and shared/global effect budgets before applying upgrades to both players.

## Songs, results and local progression

A proposed compact content target is two or three rights-checked songs with approachable and standard chart choices. Reuse the accepted track while testing the selection workflow. Song, chart, difficulty and rules version need stable identities so the audio, event list, attribution and saved record always agree. Preserve complete-song intros/endings and the existing timing subset until additional timing/note types are explicitly supported.

The exhibition plan pulls forward the event outcomes, forgiving hit streak and result summary defined in [Exhibition experience design](#exhibition-experience-design). These remain planned: saved cleanup is not a formal Miss counter, and current scoring has no streak or accuracy. A missed projectile and an unanswered event are different statistics. Judge firing time first but commit success only after matching impact. More elaborate accuracy, grades, co-op and chain/piercing statistics require separate design/testing.

Local best score for Goin' Under / Beginner / solo / the current scoring-rules version is saved in v0.1.0-demo.2 with isolated higher/lower/retry/relaunch/display checks. Missing data defaults to zero; persist only a strict completed-round improvement and retain it on retry. Exhibition-PC acceptance remains separate. The new-record flourish above is planned; v0.1.0-demo.1 does not contain this record feature.

Persist only intended settings and records, separated by song/chart, difficulty, solo/co-op/run mode and scoring rules. Missing or damaged save data must fall back safely. Unlocks, currency and permanent upgrades are optional future decisions; they must not block replaying the small accepted content set. Commercial song clearance remains separate from availability in the authorized noncommercial development bundle.

## Co-op and localization

Controller/co-op work is deferred at the creator’s request; the roadmap owns its later scheduling. The selected first co-op mode remains P1 keyboard/mouse plus P2 one gamepad. Verify the Xbox first, then Beitong as an alternate; two-gamepad play is deferred. Explicitly pair each player to devices; never use Gamepad.current to identify P2 when both pads are connected. Each player owns a reticle, cadence, release guard and projectile identity. Use shape plus labels: coral ring/P1 and teal diamond/P2. Share score, music, timer, targets, results and the beat clock. No friendly fire, movement, split-screen, upgrades or second audio source is needed for this mode.

P1 aims with the mouse and fires with the left button. P2 moves a bounded reticle with the right stick and fires with RT; Xbox Menu/Start or keyboard Esc can pause. Menu navigation has a designated owner and must not fire into the scene. Pause on the paired pad's disconnect; reconnect that player's device or return to the title to choose solo. Retain release-before-fire behavior independently on start, resume and reconnect. Resolve simultaneous target hits once through the existing hit guard. Rumble is deferred. See [controller preparation](CONTROLLER_SETUP.md) for the supplied configuration and hardware checks; gameplay integration is still planned.

Game UI targets Chinese, Japanese, and English. Text data exists; runtime switching and font coverage are pending. Exhibition readability prioritizes Japanese. See [localization](../Art/LOCALIZATION.md).

## Planned paint and color expansion

The creator requests projectile paint across the studio, multiple balloon colors and patterned explosions. The explicitly authorized [concept variants V12](../Art/CONCEPT_VARIANTS_V12.md) scene implements cosmetic balloon colors/silhouettes, matching burst pigment, randomly initialized gun themes and matching spray/flying-paint color. Production now has eight-color projectiles and matching balloon-pop deposits on MainWall, Floor, LeftWall and RightWall via the existing bounded surface queries. See the [palette guide](../Art/BALLOON_COLOR_LESSON.md). Focused October 18 geometry/lifecycle checks passed; full-song load/readability remains open. Direct projectile marks on the same four receivers also pass isolated actual-collision checks. General surface projection remains planned; full-song coverage/readability and standalone load remain open. Rhythm and input checkpoints still govern delivery.

- Start with the existing coral, teal and gold pop variants, then the six spray sprites. A balloon's chosen color should select its visible material, pop color and pattern as one entry; avoid independent random selections that disagree. Color is cosmetic initially and does not add new target rules or score values.
- Projectile hits should place paint at the actual hit position and surface normal on walls, floor, furniture and other eligible scene geometry. Verify normals, depth bias and clipping on curved/slanted objects; a flat sprite is the first wall exercise, while projected decals are the intended general surface solution. [Unity's URP decal documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/renderer-feature-decal.html) supports projection around meshes but excludes transparent receivers; begin with opaque studio surfaces and separately scope any glass fallback. Do not recolor an entire shared material on every hit.
- A balloon pop has two visual layers: a brief independent flying-paint burst and matching persistent splat/brush/drip deposits on eligible nearby surfaces. Determine contact through short, bounded surface queries; deposits must not float at the former balloon position. Match pigment to the balloon, keep the original short particle lifetime and one-time hit guard, and do not create extra damage or score. A larger or distinct on-beat deposit is an optional visual trial after ordinary deposits pass readability checks.
- The October 17 plan adopts round persistence for surface paint from both projectile impacts and balloon pops: retain deposits without timed fading/removal throughout the song, pause and results; clear them on retry, a new round or return to title. The learner removed projectile-mark timed destruction on October 18; focused lifecycle checks passed, while full-song load/performance remains open. Saving artwork across application restarts is a separate future feature, outside this increment.
- The creator explicitly declined a paint-count limit. Do not add a simultaneous-mark cap or oldest-first count removal. Marks have no colliders. Start with the existing wall receiver, then expand to eligible opaque surfaces. Measure full-song coverage, standalone frame times, memory and live objects before expanding deposit density; if individual marks are too costly, evaluate batching or surface-texture accumulation that preserves earlier paint. Pooling transient bursts alone does not solve persistent-mark growth. The four flat receivers are saved; more general projection is later M2 work and is not an exhibition requirement.

Retain source artwork and attribution. V12's authorized cosmetic selection is isolated in art components with two optional tint hooks; remaining gameplay impact/pattern integration belongs to the learner. This expansion must not delay the accepted festival rhythm demo.

## Performance approach

The creator's current performance target is 2560 x 1440 at 120 FPS on the exhibition PC, an 8.33 ms frame budget, subject to sustained measurement. Distinguish Editor Game-view observations from standalone-player CPU/GPU timings and confirm dense passages, held fire, repeated effects and frame-time spikes. October 16 creator screenshots show 36.7 FPS before fixed-room batching and 452.9 FPS afterward at 2K; the latest snapshot exceeds the target, while whole-song stability and standalone acceptance remain pending. Higher-count trials are experiments, not proven capacity; removing the fixed count cap does not prove performance or implement cleanup.

Record CPU/GPU frame time, allocations, live object counts, audio voices, and particle/decal counts. Cap targets and feedback first; add pooling only when a measured bottleneck justifies it. Pooling must reset scale, collider state, pop flags, and ownership. Resolve simultaneous hits once and bound chains. Decorative strings and props should not add gameplay colliders or unnecessary realtime shadows.
