# Game design

Updated: October 16, 2026. Delivery order, ownership and acceptance live in the [roadmap](ROADMAP.md); source and download details live in [Music and charts](MUSIC_AND_CHARTS.md).

## Confirmed direction

BalloonBarrage combines free shooting, balloon crowds, beat bonuses, and roguelite variety in a warm graffiti studio. Players can fire at any time, including held fire. Accurate rhythm earns an additional score or damage benefit; it does not unlock permission to shoot. The learner implements gameplay.

Windows solo is the first acceptance baseline. A fixed camera and shared gallery avoid movement and split-screen complexity. The creator selected P1 keyboard/mouse plus P2 one gamepad, and owns one Xbox and one Beitong controller. Prepare input support alongside the rhythm lessons; integrate shared-screen co-op after the solo rhythm checkpoint. Mobile, online multiplayer, independent dual mice and split-screen are outside current scope.

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

The creator requested a pointer-following reticle, matching paint-shot audio and gentle weapon recoil on October 16. These remain planned learner increments. Reticle visibility follows active/unpaused play while the system pointer returns for menus, pause, results and focus loss. Short shot audio plays below the music only after accepted firing; bound overlapping voices. Recoil moves only an explicit weapon visual child and restores its resting pose, preserving the camera, aiming root, muzzle and projectile direction. Verify held fire, pause and retry before including these in the exhibition build. See the roadmap for teaching order.

These are planned learner changes, except the connected burst and current physics motion. Keep Rigidbody motion and collision on the root. Cosmetic transforms belong to explicit visual children; never restore a root transform-position oscillator over the existing dynamic Rigidbody.

| Effect | Proposed implementation and initial tuning | Acceptance |
|---|---|---|
| Gentle balloon motion | Reduce initial root speed from 0.6 toward 0.25 for an aiming trial. Later give each visual child a random sine phase and about 0.03–0.05 world-unit bob amplitude at 0.5–0.7 Hz, converting to parent local units as needed. | Three targets remain easy to aim at; visual/collider alignment stays inside the measured hit margin; contacts still bounce. |
| Entrance expansion | Animate only the visible model from about 85% scale to 103%, then settle at 100% over roughly 0.2 seconds. The existing root-scaling BalloonEntrance is disabled; adapt it before reuse. | No changing Rigidbody/collider scale, abrupt overlap or drifting final scale. |
| Hit squash and burst | Lock the existing hit guard and disable colliders immediately; show a roughly 0.06-second visual squash before destroying the target. Carry the projectile's hit point so the independent burst appears at impact. | Score resolves once; the squash is visible before destruction; the burst survives the target and pauses/cleans up correctly. |
| Gun recoil | Displace a visual model child about 0.02–0.03 units backward and rotate it about 1–2 degrees, returning over 0.1 seconds. Clamp repeated impulses. | Aim root, projectile direction and logical muzzle stay stable; held fire does not accumulate displacement or shake the camera. |
| Surface paint | Spawn existing spray sprites on eligible projectile hits. Fade each from full opacity toward 0.35 over about eight seconds; globally cap at 40 marks, remove the oldest and clear on retry. Start with MainWall. | Marks have no colliders, do not obscure targets and do not accumulate across rounds. |

Existing PaintPop variants emit at most 32 particles per burst, have short non-looping lifetimes and a two-second fallback cleanup. Retain bounded firing/refill, measure concurrent effects and only add pooling or further effects if measured object counts/frame times justify them. If behind schedule, defer entrance/hit squash, extra surfaces and additional art while protecting the playable rhythm loop.

## First rhythm/roguelite demo

Proposed scope: one track, three to five waves, at least three eligible upgrades, and a complete results/retry loop. Between waves, pause combat for one-of-three upgrade selection. Start with normal one-hit balloons; introduce armored, splitter, chain, and elite targets incrementally.

Candidate upgrades include cadence, spread, piercing, chains, area burst, freeze, critical hits, and shields. Define caps, eligible offers, ownership, and chain termination before adding combinations. Reset run upgrades on retry; initially persist only settings and best scores. A recorded random seed may aid debugging without promising cross-version determinism.

Escaped-target pressure and final-wave victory are proposals for later playtests. They are not rules of the timed exhibition challenge.

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

Target 1080p at 60 FPS on the exhibition PC, subject to measurement. Measure concurrent chart-driven targets after event-window expiry is implemented, including dense sections and unanswered notes. Higher-count trials are experiments, not proven capacity; the lesson removing the fixed count cap does not prove performance or implement cleanup.

Record CPU/GPU frame time, allocations, live object counts, audio voices, and particle/decal counts. Cap targets and feedback first; add pooling only when a measured bottleneck justifies it. Pooling must reset scale, collider state, pop flags, and ownership. Resolve simultaneous hits once and bound chains. Decorative strings and props should not add gameplay colliders or unnecessary realtime shadows.
