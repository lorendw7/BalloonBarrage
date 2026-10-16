# Game design

Updated: October 16, 2026. Delivery order, ownership and acceptance live in the [roadmap](ROADMAP.md); source and download details live in [Music and charts](MUSIC_AND_CHARTS.md).

## Confirmed direction

BalloonBarrage combines free shooting, balloon crowds, beat bonuses, and roguelite variety in a warm graffiti studio. Players can fire at any time, including held fire. Accurate rhythm earns an additional score or damage benefit; it does not unlock permission to shoot. The learner implements gameplay.

Windows solo is the first acceptance baseline. A fixed camera and shared gallery avoid movement and split-screen complexity. The creator selected P1 keyboard/mouse plus P2 one gamepad, and owns one Xbox and one Beitong controller. Prepare input support alongside the rhythm lessons; integrate shared-screen co-op after the solo rhythm checkpoint. Mobile, online multiplayer, independent dual mice and split-screen are outside current scope.

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

The required October showcase is a playable rhythm shooting demo using downloaded music and a prepared chart. Studio music belongs to interfaces; gameplay uses one complete non-looping song. Start with Goin' Under / Beginner, about 123.429 seconds. Switching, full-song DSP timing, 0.8-second chart previews, target binding/cleanup, solo firing-time judgement and matched-target impact rewards are saved. The learner reports cleanup and ordinary/on-beat 10/20-point playtests passed; no independent Unity check of these latest increments is recorded. The separate-index countdown and 35-percent enlarged on-beat paint burst are also saved, with learner-reported playtests passed. Golden ring/sparkle and temporary reward Text are now connected with learner-reported display/automatic-hide acceptance. The creator selected a 1.45-second display duration, matching the saved timer; full-song calibration and independent acceptance remain pending. Keep free aiming/held fire, Japanese instructions, pause/resume, results, quit and retry. Reset score, event judgements, audio origin and targets/projectiles each round. The older local Windows ZIP still uses its five-second test scene.

The saved teal latex-and-paint burst is connected. Additional tactile feedback follows acceptance of the rhythm loop: independent gentle visual bobbing, a short entrance expansion, a brief hit squash, bounded gun recoil and fading projectile-created surface paint. Their intended behavior is specified below. Delivery order and cutoffs live in the roadmap.

The demo is accepted only when music, beat cue, shot judgement, scored impact, pause and retry work together in a Windows build. Roguelite upgrades and additional tracks follow this exhibition milestone. Do not advertise planned rhythm features as saved implementation.

## Required rhythm loop

Retain StudioDaylight_Loop.wav as interface music. Start gameplay with the downloaded Goin' Under by NegaRen and its dance-single Beginner chart by Fraxtil: constant 210 BPM, source OFFSET 0, 93 tap-row events containing 95 original lane taps. The complete audio is 5,443,200 samples at 44,100 Hz (123.428571 seconds), without looping or a 90-second crop. The first chart event is at beat 32 / about 9.143 seconds; the last is at beat 416 / about 118.857 seconds. Preserve the intro and ending, and settle only at the song's end.

Use the prepared importable audio, non-looping AudioSource prefab and derived event JSON documented in [Music and charts](MUSIC_AND_CHARTS.md). First teach menu/game playback switching, then scheduled audio with one DSP origin for chart events, timer and settlement. Keep first-beat offset and signed input calibration explicit. The [first audio lesson](RHYTHM_FIRST_LESSON.md) is a foundation exercise, not chart judgement. Do not substitute an arbitrary constant beat grid for the actual chart.

Saved solo defaults: a hit awards 10 points; an accepted shot within 100 milliseconds of its aimed target's chart event can add 10 points if it hits that event's balloon. PlayerShooter reads song time after its fire gates; InitializeShot carries the judgement result and target event id. Collision checks the on-beat flag, nonnegative id and matching target id before passing the bonus to Balloon. Its single-hit guard resolves base plus bonus once. Explicit player identity is still required before co-op integration. Off-time or mismatched-target hits retain base points. Window and reward tuning remain subject to listening/playtests; user-reported 10/20 scoring is not complete-song acceptance.

Show a small beat cue and a timing ring/countdown on each chart-driven target, plus readable timing feedback. Preview the next event sufficiently early to aim; the saved initial lead is 0.8 seconds, subject to playtest tuning. The creator requested removing the fixed three-target cap. Continuous random refill is disabled; each prepared event calls Spawn once, with no active-count gate. Saved cleanup retains unanswered targets until the event time plus a configurable delay, default 3.1 seconds at the creator's request, then deactivates/destroys without scoring. This retention is separate from the proposed timing window and does not yet count a formal Miss. Bound concurrent targets through lifecycle cleanup rather than skipping chart events at a count cap. Merge simultaneous source lanes into one target event; lane metadata is provenance, not automatic world coordinates. Keep each event playable once and mark unanswered events missed after the accepted window. Judge when a shot passes input/cadence gates and fires, so travel distance does not shift judgement; retain the matched target/event identity until impact. A timed shot already in flight needs a short impact-resolution grace bounded by projectile lifetime, rather than expiring its target before arrival. Define firstBeatOffset relative to scheduled start and positive inputCalibration as an addition to shot time. Source StepMania offset is converted with the opposite sign.

Held-fire cadence can repeatedly miss the beat even when the player starts accurately. Compare manual taps with held fire during playtests before choosing bonus and combo rules. Keep baseline free shooting useful, provide visible timing feedback, and tune accessibility independently of damage.

Pause/resume and retry must restore song, timer and chart scheduling together. Derive all gameplay timing from scheduled audio origin; stop the song and show results at the complete audio duration, then return to interface music. Measure latency on the exhibition PC. This prepared constant-tempo chart is the first supported subset; a general SSC importer, tempo changes, holds and mines are later work.

## Feedback and aiming targets

The pointer-following reticle, cursor lifecycle and matching paint-shot audio are saved, with creator-reported spot playtests passed. Cosmetic recoil is also saved: cache the resting local pose in Awake, apply a bounded 0.035 m backward displacement and 6-degree local pitch per accepted shot, and recover in LateUpdate with exponential smoothing at rate 18. Pause holds recovery. Explicit pose restoration on disable/end is not yet implemented, and physical-input recoil acceptance remains pending. Reticle visibility follows active/unpaused play while the system pointer returns for menus, pause, results and focus loss. Short shot audio plays below the music only after accepted firing; bound overlapping voices. Recoil moves only an explicit weapon visual child and restores its resting pose, preserving the camera, aiming root, muzzle and projectile direction. Verify held fire, pause and retry before including these in the exhibition build. See the roadmap for teaching order.

The additional balloon animation and surface-paint behaviors below remain planned, beyond the connected burst, saved recoil and current physics motion. Keep Rigidbody motion and collision on the root. Cosmetic transforms belong to explicit visual children; never restore a root transform-position oscillator over the existing dynamic Rigidbody.

| Effect | Proposed implementation and initial tuning | Acceptance |
|---|---|---|
| Gentle balloon motion | Reduce initial root speed from 0.6 toward 0.25 for an aiming trial. Later give each visual child a random sine phase and about 0.03–0.05 world-unit bob amplitude at 0.5–0.7 Hz, converting to parent local units as needed. | Three targets remain easy to aim at; visual/collider alignment stays inside the measured hit margin; contacts still bounce. |
| Entrance expansion | Animate only the visible model from about 85% scale to 103%, then settle at 100% over roughly 0.2 seconds. The existing root-scaling BalloonEntrance is disabled; adapt it before reuse. | No changing Rigidbody/collider scale, abrupt overlap or drifting final scale. |
| Hit squash and burst | Lock the existing hit guard and disable colliders immediately; show a roughly 0.06-second visual squash before destroying the target. Carry the projectile's hit point so the independent burst appears at impact. | Score resolves once; the squash is visible before destruction; the burst survives the target and pauses/cleans up correctly. |
| Gun recoil | Displace a visual model child about 0.02–0.03 units backward and rotate it about 1–2 degrees, returning over 0.1 seconds. Clamp repeated impulses. | Aim root, projectile direction and logical muzzle stay stable; held fire does not accumulate displacement or shake the camera. |
| Surface paint | Spawn existing spray sprites on eligible projectile hits. Fade each from full opacity toward 0.35 over about eight seconds; globally cap at 40 marks, remove the oldest and clear on retry. Start with MainWall. | Marks have no colliders, do not obscure targets and do not accumulate across rounds. |

Existing PaintPop variants emit at most 32 particles per burst, have short non-looping lifetimes and a two-second fallback cleanup. Retain bounded firing/refill, measure concurrent effects and only add pooling or further effects if measured object counts/frame times justify them. If behind schedule, defer entrance/hit squash, extra surfaces and additional art while protecting the playable rhythm loop.

## Post-festival rhythm/roguelite prototype

Proposed first slice: about three complete-song stages, initially reusing one approved song if necessary, a small pool that can offer three eligible upgrades, and a complete run-results/retry loop. Start with normal one-hit balloons, then introduce one clearly signaled armored or bounded splitter/chain rule at a time. A larger library, elite encounters and additional combinations follow playtesting rather than defining the minimum prototype.

Musical encounter phases may follow authored phrases within a song, but the audio and chart clock continue uninterrupted. Show one-of-three upgrade selection between complete-song stages. Do not pause a song mid-phrase to offer an upgrade menu or treat randomly inserted targets as original chart notes. The exhibition standard-song mode remains available without run upgrades.

Candidate upgrades include cadence, spread, piercing, chains, area burst, freeze, critical hits, and shields. Define caps, eligible offers, ownership, and chain termination before adding combinations. Reset run upgrades on retry; initially persist only settings and best scores. A recorded random seed may aid debugging without promising cross-version determinism.

The initial run can end after its authored stages with a final score; health, escaped-target pressure and elimination are later design proposals, not existing timed-challenge rules. Secondary projectiles/chains must not duplicate the primary event's rhythm reward. Define co-op ownership and shared/global effect budgets before applying upgrades to both players.

## Songs, results and local progression

A proposed compact content target is two or three rights-checked songs with approachable and standard chart choices. Reuse the accepted track while testing the selection workflow. Song, chart, difficulty and rules version need stable identities so the audio, event list, attribution and saved record always agree. Preserve complete-song intros/endings and the existing timing subset until additional timing/note types are explicitly supported.

Later results can distinguish ordinary hits, on-beat hits and unanswered chart events, with each event resolved once. A missed projectile and an unanswered chart event are different statistics. Current cleanup is not a formal Miss counter, and current scoring does not implement combo or accuracy. Define their denominators, ordering and thresholds before displaying them; judge firing time first but commit a successful reward only after the matching impact. Recheck those rules for target travel, co-op and chain/piercing upgrades.

Persist only intended settings and records, separated by song/chart, difficulty, solo/co-op/run mode and scoring rules. Missing or damaged save data must fall back safely. Unlocks, currency and permanent upgrades are optional future decisions; they must not block replaying the small accepted content set. Commercial song clearance remains separate from availability in the authorized noncommercial development bundle.

## Co-op and localization

The selected first co-op mode is P1 keyboard/mouse plus P2 one gamepad. Verify the Xbox first, then Beitong as an alternate; two-gamepad play is deferred. Explicitly pair each player to devices; never use Gamepad.current to identify P2 when both pads are connected. Each player owns a reticle, cadence, release guard and projectile identity. Use shape plus labels: coral ring/P1 and teal diamond/P2. Share score, music, timer, targets, results and the beat clock. No friendly fire, movement, split-screen, upgrades or second audio source is needed for this mode.

P1 aims with the mouse and fires with the left button. P2 moves a bounded reticle with the right stick and fires with RT; Xbox Menu/Start or keyboard Esc can pause. Menu navigation has a designated owner and must not fire into the scene. Pause on the paired pad's disconnect; reconnect that player's device or return to the title to choose solo. Retain release-before-fire behavior independently on start, resume and reconnect. Resolve simultaneous target hits once through the existing hit guard. Rumble is deferred. See [controller preparation](CONTROLLER_SETUP.md) for the supplied configuration and hardware checks; gameplay integration is still planned.

Game UI targets Chinese, Japanese, and English. Text data exists; runtime switching and font coverage are pending. Exhibition readability prioritizes Japanese. See [localization](../Art/LOCALIZATION.md).

## Planned paint and color expansion

The creator requests projectile paint across the studio, multiple balloon colors and patterned explosions. The explicitly authorized [concept variants V12](../Art/CONCEPT_VARIANTS_V12.md) scene now implements cosmetic balloon colors/silhouettes, matching burst pigment, randomly initialized gun themes and matching spray/flying-paint color. Surface impact placement, projected patterns and fading remain planned. Rhythm and input checkpoints still govern delivery.

- Start with the existing coral, teal and gold pop variants, then the six spray sprites. A balloon's chosen color should select its visible material, pop color and pattern as one entry; avoid independent random selections that disagree. Color is cosmetic initially and does not add new target rules or score values.
- Projectile hits should place paint at the actual hit position and surface normal on walls, floor, furniture and other eligible scene geometry. Verify normals, depth bias and clipping on curved/slanted objects; a flat sprite is the first wall exercise, while projected decals are the intended general surface solution. [Unity's URP decal documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/renderer-feature-decal.html) supports projection around meshes but excludes transparent receivers; begin with opaque studio surfaces and separately scope any glass fallback. Do not recolor an entire shared material on every hit.
- A patterned pop should emit a brief independent burst and select a matching splat/brush/drip shape for eligible nearby surfaces. Determine contact through short, bounded surface queries; patterns must not float in empty space. Keep the original short particle lifetime and one-time hit guard.
- Use a round-owned global paint budget (initially 40 marks shared by both players), fade toward 0.35 opacity over eight seconds, remove the oldest and clear on retry. Marks have no colliders. Measure readability and object/frame counts before increasing this budget.

Retain source artwork and attribution. V12's authorized cosmetic selection is isolated in art components with two optional tint hooks; remaining gameplay impact/pattern integration belongs to the learner. This expansion must not delay the accepted festival rhythm demo.

## Performance approach

The creator's current performance target is 2560 x 1440 at 120 FPS on the exhibition PC, an 8.33 ms frame budget, subject to sustained measurement. Distinguish Editor Game-view observations from standalone-player CPU/GPU timings and confirm dense passages, held fire, repeated effects and frame-time spikes. October 16 creator screenshots show 36.7 FPS before fixed-room batching and 452.9 FPS afterward at 2K; the latest snapshot exceeds the target, while whole-song stability and standalone acceptance remain pending. Higher-count trials are experiments, not proven capacity; removing the fixed count cap does not prove performance or implement cleanup.

Record CPU/GPU frame time, allocations, live object counts, audio voices, and particle/decal counts. Cap targets and feedback first; add pooling only when a measured bottleneck justifies it. Pooling must reset scale, collider state, pop flags, and ownership. Resolve simultaneous hits once and bound chains. Decorative strings and props should not add gameplay colliders or unnecessary realtime shadows.
