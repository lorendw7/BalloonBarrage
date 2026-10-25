# Game design

Updated: October 25, 2026. Delivery order, ownership and acceptance live in the [roadmap](ROADMAP.md); source and download details live in [Music and charts](MUSIC_AND_CHARTS.md). Current source and public prerelease v0.1.0-demo.5 include the first statistics/streak/result-summary increment, the continuous-spray checkpoint and shared Simplified Chinese/Japanese/English UI. Publication does not imply target-hardware acceptance.

## Confirmed direction

BalloonBarrage is a **casual, relaxing rhythm game** about popping balloons and painting a warm studio. Free aiming, tactile pops, colorful marks and an inviting musical pulse are the core identity. Players can fire at any time, including held fire; ordinary hits remain satisfying and timed hits earn an optional score bonus. Missing the beat does not block shooting, reduce score, eliminate the player or require a restart. The learner implements gameplay.

The festival promise is a comfortable complete-song experience, not an accuracy exam or compulsory combo chase. Rhythm must remain legible through the existing chart-driven appearances, timing cue and audible/visual bonus; relaxation does not mean replacing the chart with unrelated random targets. Roguelite upgrades are exploratory post-festival ideas, not a defining feature or current commitment.

Windows solo is the first acceptance baseline. A fixed camera and shared gallery avoid movement and split-screen complexity. The selected later co-op remains P1 keyboard/mouse plus P2 one gamepad; the creator owns one Xbox and one Beitong controller. Controller/co-op integration remains deferred beyond the exhibition experience work. Mobile, online multiplayer, independent dual mice and split-screen are outside current scope.

## Inspiration and originality boundary

Splatoon is a useful reference for three abstract qualities: firing paint visibly changes the space, color makes input consequences immediately readable, and shooting the environment can be pleasurable even without defeating an opponent. BalloonBarrage translates those qualities into its own fixed-camera rhythm studio: the room becomes a record of the player's two-minute performance, balloons provide readable focal targets, and the beat adds an optional accent. The official Splatoon reference is [Nintendo's gameplay overview](https://splatoon.nintendo.com/en/gameplay/); it is design research, not a source asset or a feature checklist.

Do not copy Splatoon characters, names, UI, maps, weapon silhouettes, sound, music, team rules, turf scoring, ink-swimming movement or visual assets. BalloonBarrage's identity remains balloons, a warm indoor art studio, a complete-song rhythm structure, persistent personal paint and a calm solo exhibition experience. No Nintendo material is included or implied to be licensed.

After Kyudaisai, design study may compare multiple games, including Splatoon, at the level of abstract mechanics, interaction clarity, pacing, camera language and environmental feedback. Each lesson must identify what player problem a reference solves, then create and test a BalloonBarrage-specific answer with original code, layouts, art and audio. A reference is evidence for a design question, not permission to reproduce its content or combine feature lists without a player-experience reason.

An explorable 3D map is a post-festival teaching direction, not an exhibition promise. The first experiment should live in a separate graybox scene and preserve the accepted fixed-camera complete-song mode as a working baseline. It may investigate movement, camera control, landmarks, navigable studio spaces and paintable surfaces one system at a time. It should advance only if testing shows that exploration supports the relaxing rhythm-and-paint identity without making aiming, musical timing, comfort or performance less readable.

## Continuous-fire contract

“Unrestricted continuous fire” means the player may hold the fire input for the whole playable round. There is no ammunition pool, magazine, reload, heat meter, stamina cost, rhythm permission gate or forced downtime. Empty shots and surface painting do not punish the player. Releasing and pressing again is a choice, not a resource-reset requirement.

The simulation must still accept a finite, tuned number of hit samples per second. The saved `PlayerShooter` exposes `shotsPerSecond` with a default of 5, while `PrototypeScene` saves 6, and repeats while the mouse button is held. Each accepted sample performs an immediate bounded SphereCast; the visible stream is a separate looping particle presentation rather than a flying gameplay projectile. This cadence controls feedback clarity, audio overlap, collision load and persistent-paint growth; it is not an in-world firing restriction. Tune the one serialized value through playtests rather than deleting the gate or querying collision every frame. Menus, pause and focus changes may require the button to be released once before firing resumes so UI clicks do not become accidental shots.

The first learner exercise is [Continuous-fire and paint-loop lesson](CONTINUOUS_FIRE_LESSON.md). It begins with observation rather than a code rewrite because the saved input path already satisfies the no-ammo/no-reload contract.

## Whole-game loop and mode boundaries

The accepted foundation is title/instructions → one complete song → results → retry/title. Keep free aiming, unrestricted held fire, musical bonuses and visible paint feedback readable throughout that loop. The intended later game adds song/difficulty selection and local records; a separate run mode can connect complete-song stages through upgrade choices. These additions remain proposals until implemented and accepted through the roadmap.

| Mode | Intended experience | State and scoring boundary |
|---|---|---|
| Exhibition / standard song | One selected complete song, initially the prepared Goin' Under chart | Existing base/on-beat score rules; no run upgrades; complete-song end owns settlement |
| Local co-op | Two independently controlled reticles in the same studio and song | Shared score and one consumed target outcome; explicit input/player identity supports later contribution statistics |
| Roguelite run (exploratory) | A possible later branch, only if it supports the relaxing core experience | Not a committed milestone; keep standard songs available without upgrades or pressure |
| Practice / free paint | Possible later place to learn aim or decorate without score pressure | Separate from chart accuracy and competitive records; not a prerequisite for the first release |

Color, sound, light and paint should make an accepted action easier to understand. They must not silently alter timing rules, move the logical muzzle through the environment or obscure the next target. Settings, localization and records support the accepted loop rather than requiring an online service. The [roadmap](ROADMAP.md) owns milestones, dependencies, priorities and release gates.

## Exhibition slice

The exhibition foundation is one complete-song rhythm shooting demo using Goin' Under / Beginner, about 123.429 seconds and 93 prepared events. Studio music belongs to interfaces; gameplay uses the full non-looping song. Public v0.1.0-demo.5 includes DSP timing, 0.8-second previews, target binding/cleanup, firing-time matched-target 10/20 rewards, the separate-index localized NEXT cue, enlarged same-color burst, golden accent and selected 1.45-second reward text. Reticle, audio, bounded recoil, pause/results/retry, four-surface persistent paint, local records and result statistics are saved. Detailed evidence and limitations live in Validation; clean exhibition-PC listening, readability and stability remain open. Preserve complete-song end and reset score, event state, audio origin and round objects on retry.

The saved teal latex-and-paint burst is connected. Additional tactile feedback follows acceptance of the rhythm loop: independent gentle visual bobbing, a short entrance expansion, a brief hit squash, bounded gun recoil and round-persistent surface paint from sampled spray impacts and balloon pops. Their intended behavior is specified below. Delivery order and cutoffs live in the roadmap.

The demo is accepted only when music, beat cue, shot judgement, scored impact, pause and retry work together in a Windows build. Roguelite upgrades and additional tracks follow this exhibition milestone. Do not advertise planned rhythm features as saved implementation.

## Required rhythm loop

Retain StudioDaylight_Loop.wav as interface music. Start gameplay with the downloaded Goin' Under by NegaRen and its dance-single Beginner chart by Fraxtil: constant 210 BPM, source OFFSET 0, 93 tap-row events containing 95 original lane taps. The complete audio is 5,443,200 samples at 44,100 Hz (123.428571 seconds), without looping or a 90-second crop. The first chart event is at beat 32 / about 9.143 seconds; the last is at beat 416 / about 118.857 seconds. Preserve the intro and ending, and settle only at the song's end.

Use the prepared importable audio, non-looping AudioSource prefab and derived event JSON documented in [Music and charts](MUSIC_AND_CHARTS.md). First teach menu/game playback switching, then scheduled audio with one DSP origin for chart events, timer and settlement. Keep first-beat offset and signed input calibration explicit. The [first audio lesson](RHYTHM_FIRST_LESSON.md) is a foundation exercise, not chart judgement. Do not substitute an arbitrary constant beat grid for the actual chart.

Saved solo defaults: a hit awards 10 points; an accepted sample within 100 milliseconds of its aimed target's chart event can add 10 points when the SphereCast reaches that balloon. `PlayerShooter` reads song time after its input/cadence gates, performs the bounded cast from the muzzle and evaluates `Balloon.IsOnBeat` immediately before calling `Balloon.Hit`. The balloon's single-hit guard resolves base plus bonus once. Explicit player identity is still required before co-op integration. Off-time hits retain base points. Window and reward tuning remain subject to listening/playtests; user-reported 10/20 scoring is not complete-song acceptance.

The saved cue is the global numeric NEXT countdown in RhythmChartReader, plus accepted on-beat feedback. Per-target timing rings/countdowns are proposed, not implemented; the global cue does not identify a particular balloon. A graphical approach indicator is the next presentation exercise, with target association requiring a separate learner increment. Preview the next event sufficiently early to aim; the saved initial lead is 0.8 seconds, subject to playtest tuning. The creator requested removing the fixed three-target cap. Continuous random refill is disabled; each prepared event calls Spawn once, with no active-count gate. Saved cleanup retains unanswered targets until the event time plus a configurable delay, default 3.1 seconds at the creator's request, then deactivates/destroys without scoring. This retention is separate from the proposed timing window and does not yet count a formal Miss. Bound concurrent targets through lifecycle cleanup rather than skipping chart events at a count cap. Merge simultaneous source lanes into one target event; lane metadata is provenance, not automatic world coordinates. Keep each event playable once and mark unanswered events missed after the accepted window. Judge when a sample passes input/cadence gates; the immediate cast has no projectile-travel delay or in-flight grace period. Define firstBeatOffset relative to scheduled start and positive inputCalibration as an addition to shot time. Source StepMania offset is converted with the opposite sign.

Held-fire cadence can repeatedly miss the beat even when the player starts accurately. Compare manual taps with held fire during playtests before choosing bonus and combo rules. Keep baseline free shooting useful, provide visible timing feedback, and tune accessibility independently of damage. Do not solve this tension with ammo, reload, heat, forced tapping or a permission-to-fire window.

Pause/resume and retry must restore song, timer and chart scheduling together. Derive all gameplay timing from scheduled audio origin; stop the song and show results at the complete audio duration, then return to interface music. Measure latency on the exhibition PC. This prepared constant-tempo chart is the first supported subset; a general SSC importer, tempo changes, holds and mines are later work.

October 19 established eight balloon pigments, group-local selection across 48 persistent-mark patterns, configurable in-plane rotation and uniform instance scale. The October 24 continuous-spray checkpoint reuses those indexed resources without instantiating a flying projectile. Production defaults to the teal group so the gun accent, particle stream and direct surface marks read as one paint source; the serialized group index preserves a controlled future theme choice. Direct spray deposits use the spray, burst and drip families at 1.0–1.35 scale, while repeated broad swoosh/dry-brush/starburst marks are excluded from this path. The local-space visual stream derives its length from the cursor contact and sorts above transparent graffiti and persistent marks; authoritative hits still come from the finite immediate SphereCast cadence. Wall marks add a short visual-only scale pulse. Color matching is not a scoring condition. Focused static and synthetic Play Mode checks cover the corrected stream origin, bounded live particles and one persistent teal mark; physical feel, complete-song behavior and performance remain open. See the [palette guide](../Art/BALLOON_COLOR_LESSON.md) for indexed assets, provenance and verification limits. Public v0.1.0-demo.5 packages this checkpoint.

The October 19 pattern-art follow-up prepares six silhouette families per pigment (48 collider-free prefab resources) and varies the eight already-connected defaults. Group-local pattern selection, configurable rotation and uniform random scale are now learner-written and saved, with the verification limits stated above; see the [palette guide](../Art/BALLOON_COLOR_LESSON.md#six-pattern-families-prepared-for-learner-integration). Asset preparation itself did not implement learner gameplay.

## Exhibition experience design

**Implementation status, October 24:** the creator confirms a casual, relaxing rhythm-game direction, unrestricted held fire and approximately 4–6 hours per day. The first successful-hit, on-beat-hit, forgiving-streak and result-summary increment is saved in current source. A bounded original continuous-spray pass is also saved with a concentrated looping stream, immediate forgiving hit volume, coordinated pigment marks and short impact pulse. Focused gameplay, Retry, layout, performance and package acceptance remain open. Japanese/English switching remains a pre-festival commitment. Milestone flourishes and extra variants remain optional. Calendar, cuts and ownership live in the [roadmap](ROADMAP.md).

### Player promise and pacing

The intended rhythm is aim → fire → immediate accepted hit → pigment burst/paint → readable success/streak response → next target. On-beat success has a recognizable extra accent; ordinary hits still feel worthwhile. Preserve the unshortened song and roughly nine-second first-event intro. Players can already shoot/paint during the intro; explain this rather than adding off-chart tutorial targets.

After the song, keep the painted studio visible behind a compact, friendly summary. Lead with a positive completion line, then score, best score, successful hits, on-beat hits and maximum streak. Retry is clear but optional. Do not foreground unanswered targets, accuracy grades, rankings or a broken combo. No health, ammunition, elimination, score subtraction, score multipliers, mid-song upgrades, random target rules or online leaderboard is required.

Exhibition copy should invite play without promising a medical stress-relief effect. Proposed short Japanese copy: "音楽に合わせて、気ままに風船を割ろう。" Controls should explicitly allow held fire and explain timed shots as an extra reward, with no need to hit every beat. Verify all glyphs before integration; this text is not yet saved game UI.

### Event outcomes and statistics — partially saved

Use the loaded chart's valid event identities as the denominator: 93 for this chart, not shots, paint marks or original lane taps. Each event starts unresolved and commits one terminal outcome: ordinary hit, on-beat hit or missed/unanswered event. A shared accepted resolution keeps score, counters and streak consistent and retains the existing target hit guard. Unknown/unbound ids do not contribute to chart statistics; duplicate outcomes cannot emit another success/effect.

The October 23 source checkpoint increments successful hits and current/maximum streak only after `Balloon.Hit` passes the balloon's existing one-pop guard; the saved positive bonus selects the on-beat subset. An unresolved balloon reaching the existing cleanup deadline resets current streak, and `Score.ResetScore` clears all round values. This does not yet implement the proposed missed-event total, a separate event-outcome registry or final settlement accounting. The focused cases below and natural Results/Retry flow remain required before acceptance.

| Value | Definition |
|---|---|
| Successful hits | Ordinary plus on-beat hits, counted only after an actual accepted scored impact |
| On-beat hits | A subset of successful hits: the existing firing-time judgement reaches its matching event and earns the saved bonus |
| Ordinary hits | Successful chart hits without that matching bonus, including off-time/mismatched-event shots |
| Missed events | Unresolved targets reaching the existing cleanup deadline; at natural settlement finalize any remaining chart events once before showing the summary |
| Maximum streak | Highest current-round hit streak; initially not a separate persistent record |

Preserve complete-song end, 3.1-second target cleanup and the finite six-samples-per-second production cadence. A sampled input is not yet a successful hit unless its immediate cast reaches a balloon. Surface and empty samples are not missed chart events. Quit is not a completed round and must not fabricate full-song statistics. Pause commits no new outcomes. Retry resets round statistics/outcomes while retaining the best-score key/value. Do not extend settlement or retune timing as part of these counters.

At a natural completed round require `successfulHits + missedEvents = chartEventCount`, `onBeatHits <= successfulHits`, and `ordinaryHits = successfulHits - onBeatHits`. With unchanged rewards, `finalScore = 10 * successfulHits + 10 * onBeatHits`. Five hits including two on-beat hits mean three ordinary hits and 70 points, not seven hits. No accuracy percentage or S/A/B grade is required.

### Forgiving hit streak — committed for the exhibition

- Start current and maximum streak at zero. Accepted ordinary/on-beat event hits increment current and update maximum, in actual resolution order rather than trigger-pull order.
- Accepted unanswered-target expiry resets current streak. Merely passing the scheduled beat does not: preserve the saved cleanup/grace behavior.
- Empty shots, surface paint, ordinary off-beat hits, held fire and pause do not independently break a streak.
- Duplicate Hit, expiry and settlement callbacks cannot advance/reset an already resolved event again. Results show maximum streak rather than a misleading current streak after final missed-event resolution.
- No streak score multiplier: existing best-score rules/key remain comparable. Any later reward/window change needs a separate scoring-version decision.
- If trialled, a reset is quiet: no red warning, buzzer, camera jolt or lost accumulated score. Hide or omit the live streak if it encourages anxious monitoring. Do not name this field a rhythm combo: ordinary hits also extend it.

Acceptance example: three hits give current/max 3; an unanswered-event expiry gives current 0/max 3; the next hit gives current 1/max 3. Duplicate Hit and surface-only shots do not change these outcomes twice. Natural results/retry must satisfy the count identities above.

### Feedback hierarchy and trial tuning — planned

These are starting trials, not accepted final values. Improve silhouette, timing and contrast using existing assets before increasing counts.

| Moment | Intended response | Guardrail |
|---|---|---|
| Accepted fire | Existing reticle, sound and bounded gun recoil | No compulsory camera shake or feedback on rejected input |
| Arrival | Visual child about 85% → 103% → 100% scale over 0.20 seconds | Fixed root Rigidbody/collider; verify visual/hit alignment and remove overshoot if needed |
| Ordinary hit | Immediate outcome/score lock and collider disable, then about 0.06-second visual-child squash; independent pigment burst/deposits | Trigger once; delayed visual destruction cannot leave a live target or duplicate score |
| On-beat hit | Matching pigment plus clearer small gold ring/sparkle and an optional brief audio accent | Initially preserve existing 35% burst enlargement and selected 1.45-second reward text; no full-screen flash or extra paint queries |
| Streak | One small count with a restrained approximately 0.12-second scale pulse | No text object spawned per hit; keep aim/next-event cue visible; no score multiplier |
| Milestone | Trial accents at streak 5, 10 and 20 using existing palette/effects | Replace/retrigger one slot, never queue labels or overlapping accent voices; optional polish is cut first |
| Results/new record | Positive completion line, visible painted studio, score/best, reliable hit/on-beat counts and maximum streak | No failure-focused Miss count or grade. Save strict record improvement once; ties/lower results still get a welcoming finish |

Animate explicit visual children. Do not animate the dynamic root or stop global time for hit stop. Logical muzzle, aim, collisions and music remain independent. Moving the burst to the exact sampled spray hit point is a later refinement if it needs another API/call-site change; it must not delay accepting a simpler squash.

Audition sound against music during dense held fire. Keep the accepted base shot audible without masking the track. A new on-beat accent is brief with bounded overlap; do not change music speed/pitch or duck the whole track by default. Retain at most 32 particles per pop and the two-second transient fallback until a measured revision is accepted.

Keep persistent paint through results, without a count cap, timed fade or oldest-first removal. Preserve pigment and actual contact on the four accepted flat receivers. The October 25 stretch target expands coverage to deliberately prepared opaque static room receivers before the festival; glass, UI, balloons, particles, moving roots and unprepared meshes remain ineligible. The pre-festival spray pass may strengthen the custom continuous stream, impact splat/pop and on-beat/streak accents, but must use original or attributed project assets. It must not reproduce Splatoon assets, UI, fonts, weapon/character designs, sounds or territory-control rules. Fix flicker/floating marks/target contrast before adding extra variants or curved projection. Extra bob, richer milestone effects and result ornaments are optional cuts.

### Exhibit communication and acceptance — planned

Use concise Japanese-first instructions with checked glyphs: mouse aim, held fire, fire near countdown zero for a matching-target bonus, Esc pause and Retry. The exhibition UI defaults to Japanese and offers Simplified Chinese, Japanese and English from the same screen hierarchy, with a persisted choice and refreshed dynamic labels. The small author-credit font does not cover general UI copy; bundled M+ 1c and Noto Sans SC assets require connected Unity glyph/layout acceptance.

Observe unfamiliar players before accepting the new effects, including someone who rarely plays rhythm games. Ask whether ordinary popping feels good, timing adds a recognizable bonus, targets remain visible and anything feels tiring or punishing. A relaxed completed round matters more than a higher score or a forced retry. Change one parameter at a time. Native layout checks, source/reference checks, synthetic outcomes, manual feel reports and clean-player measurements remain distinct evidence. Saved statistics/streak/result bindings are not accepted until their focused Play Mode cases pass; language switching and new effect increments remain unimplemented.

## Feedback and aiming targets

The pointer-following reticle, cursor lifecycle and matching paint-shot audio are saved, with creator-reported spot playtests passed. Cosmetic recoil is also saved: cache the resting local pose in Awake, apply a bounded 0.035 m backward displacement and 6-degree local pitch per accepted sample, and recover in LateUpdate with exponential smoothing at rate 18. Pause holds recovery. ResetGunPose restores the cached visual pose immediately while not playing and on disable. The creator reports recoil and settlement/retry reset passed, followed by no apparent issue in the earlier start/pause/resume spot check. These reports predate the final continuous-spray tuning and do not establish latest full-song or standalone acceptance. Reticle visibility follows active/unpaused play while the system pointer returns for menus, pause, results and focus loss. Short shot audio plays below the music only after accepted firing; bound overlapping voices. Recoil moves only an explicit weapon visual child and restores its resting pose, preserving the camera, aiming root, muzzle and spray direction. Verify held fire, pause and retry before including these in the exhibition build. See the roadmap for teaching order.

Projectile marks and matching pop deposits are saved on MainWall, Floor, LeftWall and RightWall, persisting through results and clearing on retry. Isolated geometry/lifecycle checks and an instrumented full-song trial support this checkpoint; human feel/readability and exhibition-PC acceptance remain separate. No paint-count limit is planned. Entrance/hit animation and the experience additions above remain planned. Keep Rigidbody/collision on the root and cosmetic transforms on explicit visual children.

| Effect | Proposed implementation and initial tuning | Acceptance |
|---|---|---|
| Gentle balloon motion | Reduce initial root speed from 0.6 toward 0.25 for an aiming trial. Later give each visual child a random sine phase and about 0.03–0.05 world-unit bob amplitude at 0.5–0.7 Hz, converting to parent local units as needed. | Three targets remain easy to aim at; visual/collider alignment stays inside the measured hit margin; contacts still bounce. |
| Entrance expansion | Animate only the visible model from about 85% scale to 103%, then settle at 100% over roughly 0.2 seconds. The existing root-scaling BalloonEntrance is disabled; adapt it before reuse. | No changing Rigidbody/collider scale, abrupt overlap or drifting final scale. |
| Hit squash and burst | Lock the existing hit guard and disable colliders immediately; show a roughly 0.06-second visual squash before destroying the target. Carry the sampled hit point so the independent burst appears at impact. | Score resolves once; the squash is visible before destruction; the burst survives the target and pauses/cleans up correctly. |
| Gun recoil (saved tuning) | Apply 0.035 units backward and -6 degrees local pitch from the cached visual rest pose; recover with exponential smoothing rate 18. Reset immediately outside play/on disable. | Aim root, spray direction and logical muzzle stay stable; held fire does not accumulate displacement or shake the camera. |
| Surface paint | Saved spray/pop deposits on four accepted flat receivers persist through results and clear on scene retry. A future title-return route must also clear them. No count cap or timed fading. | Correct contact, no mark colliders, readable targets and no cross-round accumulation; human/target-PC acceptance remains open. |

Existing PaintPop variants emit at most 32 particles per burst, have short non-looping lifetimes and a two-second fallback cleanup. Retain bounded firing/refill, measure concurrent effects and only add pooling or further effects if measured object counts/frame times justify them. If behind schedule, defer entrance/hit squash, extra surfaces and additional art while protecting the playable rhythm loop.

## Optional post-festival rhythm/roguelite exploration

This is an exploratory branch, not the next committed release or the game's main positioning. Reassess whether upgrades or tougher targets add enjoyable variety without undermining the relaxing standard-song mode. If selected later, a possible slice is three complete-song stages and a small between-song upgrade pool; define and test one additional rule at a time. A larger library, elite encounters and combinations do not define the exhibition or minimum game.

Musical encounter phases may follow authored phrases within a song, but the audio and chart clock continue uninterrupted. Show one-of-three upgrade selection between complete-song stages. Do not pause a song mid-phrase to offer an upgrade menu or treat randomly inserted targets as original chart notes. The exhibition standard-song mode remains available without run upgrades.

Candidate upgrades include cadence, spread, piercing, chains, area burst, freeze, critical hits, and shields. Define caps, eligible offers, ownership, and chain termination before adding combinations. Reset run upgrades on retry; initially persist only settings and best scores. A recorded random seed may aid debugging without promising cross-version determinism.

The initial run can end after its authored stages with a final score; health, escaped-target pressure and elimination are later design proposals, not existing timed-challenge rules. Secondary projectiles/chains must not duplicate the primary event's rhythm reward. Define co-op ownership and shared/global effect budgets before applying upgrades to both players.

## Songs, results and local progression

A proposed compact content target is two or three rights-checked songs with approachable and standard chart choices. Reuse the accepted track while testing the selection workflow. Song, chart, difficulty and rules version need stable identities so the audio, event list, attribution and saved record always agree. Preserve complete-song intros/endings and the existing timing subset until additional timing/note types are explicitly supported.

The current source extends the small result summary with successful hits, on-beat hits and maximum streak as defined in [Exhibition experience design](#exhibition-experience-design). The values use the accepted balloon impact and existing cleanup path, but saved cleanup is not yet a formal event-outcome registry and no missed-event total or accuracy exists. Unanswered-event totals support future internal correctness checks, not a failure-focused results screen. Judge firing time first but commit success only after matching impact. Accuracy, grades and more elaborate statistics require separate design/testing.

Local best score for Goin' Under / Beginner / solo / the current scoring-rules version is saved in v0.1.0-demo.2 with isolated higher/lower/retry/relaunch/display checks. Missing data defaults to zero; persist only a strict completed-round improvement and retain it on retry. Exhibition-PC acceptance remains separate. The new-record flourish above is planned; v0.1.0-demo.1 does not contain this record feature.

Persist only intended settings and records, separated by song/chart, difficulty, solo/co-op/run mode and scoring rules. Missing or damaged save data must fall back safely. Unlocks, currency and permanent upgrades are optional future decisions; they must not block replaying the small accepted content set. Commercial song clearance remains separate from availability in the authorized noncommercial development bundle.

## Co-op and localization

Controller/co-op work is deferred at the creator’s request; the roadmap owns its later scheduling. The selected first co-op mode remains P1 keyboard/mouse plus P2 one gamepad. Verify the Xbox first, then Beitong as an alternate; two-gamepad play is deferred. Explicitly pair each player to devices; never use Gamepad.current to identify P2 when both pads are connected. Each player owns a reticle, cadence, release guard and spray-sample identity. Use shape plus labels: coral ring/P1 and teal diamond/P2. Share score, music, timer, targets, results and the beat clock. No friendly fire, movement, split-screen, upgrades or second audio source is needed for this mode.

P1 aims with the mouse and fires with the left button. P2 moves a bounded reticle with the right stick and fires with RT; Xbox Menu/Start or keyboard Esc can pause. Menu navigation has a designated owner and must not fire into the scene. Pause on the paired pad's disconnect; reconnect that player's device or return to the title to choose solo. Retain release-before-fire behavior independently on start, resume and reconnect. Resolve simultaneous target hits once through the existing hit guard. Rumble is deferred. See [controller preparation](CONTROLLER_SETUP.md) for the supplied configuration and hardware checks; gameplay integration is still planned.

Translation data targets Simplified Chinese, Japanese and English. The festival runtime exposes all three, defaults to Japanese when no preference exists, and persists switching without duplicating screens. M+ 1c covers prepared Japanese/Latin copy and Noto Sans SC covers prepared Simplified Chinese copy. Static labels plus the runtime-written beat countdown, on-beat award and result statistics are connected to the shared table. Unity editor validation and Japanese 2K/1080p flow checks pass; three-language standalone-player acceptance remains pending. See [localization](../Art/LOCALIZATION.md).

## Planned paint and color expansion

The creator requests paint across the studio, multiple balloon colors and patterned explosions. The explicitly authorized [concept variants V12](../Art/CONCEPT_VARIANTS_V12.md) scene implements cosmetic balloon colors/silhouettes, matching burst pigment, randomly initialized gun themes and matching spray color. Current production defaults to teal while retaining eight indexed pigment groups, and matching balloon-pop deposits exist on MainWall, Floor, LeftWall and RightWall via bounded surface queries. The next learner-owned increment is an explicit color switch that updates stream, impact/deposit selection and gun accent from one shared index; random independent colors are not acceptable. See the [palette guide](../Art/BALLOON_COLOR_LESSON.md). Earlier focused geometry/lifecycle checks covered the persistent resources; the October 24 immediate-spray integration still needs full-song coverage/readability and standalone-load checks. Prepared opaque-room projection is now a pre-festival stretch with the existing four receivers as fallback. Rhythm and input checkpoints still govern delivery.

- Start with the existing coral, teal and gold pop variants, then the six spray sprites. A balloon's chosen color should select its visible material, pop color and pattern as one entry; avoid independent random selections that disagree. Color is cosmetic initially and does not add new target rules or score values.
- Spray samples should place paint at the actual hit position and surface normal on walls, floor, furniture and other eligible scene geometry. Verify normals, depth bias and clipping on curved/slanted objects; a flat sprite is the first wall exercise, while projected decals are the intended general surface solution. [Unity's URP decal documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/renderer-feature-decal.html) supports projection around meshes but excludes transparent receivers; begin with opaque studio surfaces and separately scope any glass fallback. Do not recolor an entire shared material on every hit.
- A balloon pop has two visual layers: a brief independent flying-paint burst and matching persistent splat/brush/drip deposits on eligible nearby surfaces. Determine contact through short, bounded surface queries; deposits must not float at the former balloon position. Match pigment to the balloon, keep the original short particle lifetime and one-time hit guard, and do not create extra damage or score. A larger or distinct on-beat deposit is an optional visual trial after ordinary deposits pass readability checks.
- The October 17 plan adopts round persistence for surface paint from both spray impacts and balloon pops: retain deposits without timed fading/removal throughout the song, pause and results; clear them on retry, a new round or return to title. The learner removed paint-mark timed destruction on October 18; focused lifecycle checks passed, while full-song load/performance remains open. Saving artwork across application restarts is a separate future feature, outside this increment.
- The creator explicitly declined a paint-count limit. Do not add a simultaneous-mark cap or oldest-first count removal. Marks have no colliders. Start with the four saved flat receivers, then expand only to deliberately prepared opaque static surfaces. Measure full-song coverage, standalone frame times, memory and live objects before expanding deposit density; if individual marks are too costly, use the paint-preserving [full-scene paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md) rather than silently deleting old work. Pooling transient bursts alone does not solve persistent-mark growth. Prepared-room coverage is an October 25 pre-festival stretch; arbitrary curved, moving or transparent projection remains post-festival work.

Retain source artwork and attribution. V12's authorized cosmetic selection is isolated in art components with two optional tint hooks; remaining gameplay impact/pattern integration belongs to the learner. This expansion must not delay the accepted festival rhythm demo.

## Performance approach

The creator's current performance target is 2560 x 1440 at 120 FPS on the exhibition PC, an 8.33 ms frame budget, subject to sustained measurement. Distinguish Editor Game-view observations from standalone-player CPU/GPU timings and confirm dense passages, held fire, repeated effects and frame-time spikes. October 16 creator screenshots show 36.7 FPS before fixed-room batching and 452.9 FPS afterward at 2K; the latest snapshot exceeds the target, while whole-song stability and standalone acceptance remain pending. Higher-count trials are experiments, not proven capacity; removing the fixed count cap does not prove performance or implement cleanup.

Record CPU/GPU frame time, allocations, live object counts, audio voices, and particle/decal counts. Cap targets and feedback first; add pooling only when a measured bottleneck justifies it. Pooling must reset scale, collider state, pop flags, and ownership. Resolve simultaneous hits once and bound chains. Decorative strings and props should not add gameplay colliders or unnecessary realtime shadows.
