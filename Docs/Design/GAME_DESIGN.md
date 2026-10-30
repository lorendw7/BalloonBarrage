# Game design

Updated: October 31, 2026. Delivery order, ownership and acceptance live in the [roadmap](ROADMAP.md); source and download details live in [Music and charts](MUSIC_AND_CHARTS.md). The authorized festival revision prepares ten short profiles and ten complete-song profiles, phrase-based patterns and one brief listen-then-answer motif per profile. Source, scene integration, automated runtime and package evidence are recorded separately; demo.6 remains the complete-song fallback. The long-note paint ribbon is explicitly deferred. [Validation](../Project/VALIDATION.md) separates automated evidence from human or target-hardware acceptance.

## Confirmed direction

BalloonBarrage is a **casual, relaxing rhythm game** about popping balloons and painting a warm studio. Free aiming, tactile pops, colorful marks and an inviting musical pulse are the core identity. Players can fire at any time, including held fire; ordinary hits remain satisfying and timed hits earn an optional score bonus. Missing the beat does not block shooting, reduce score, eliminate the player or require a restart. The learner normally implements gameplay; the creator explicitly authorized Codex to implement this bounded festival revision while preserving the three learner functions.

The festival promise is a comfortable short song session with visible phrase-to-phrase variety, not an accuracy exam or compulsory combo chase. The accepted complete-song build remains the fallback and the standard-song reference. Rhythm must remain legible through chart-driven appearances, timing cues and audible/visual bonuses; variety comes from authored song and pattern data rather than unrelated per-frame random targets. Roguelite upgrades are exploratory post-festival ideas, not a defining feature or current commitment.

Windows solo is the first acceptance baseline. A fixed camera and shared gallery avoid movement and split-screen complexity. The creator now requested mouse + gamepad and two-gamepad local practice. [Local players](#local-players-and-gamepads) defines its completed infrastructure and learner-owned unfinished core. The creator owns one Xbox and one Beitong controller; physical device acceptance remains separate. Mobile, online multiplayer, independent dual mice and split-screen are outside current scope.

## Inspiration and originality boundary

Splatoon is a useful reference for three abstract qualities: firing paint visibly changes the space, color makes input consequences immediately readable, and shooting the environment can be pleasurable even without defeating an opponent. BalloonBarrage translates those qualities into its own fixed-camera rhythm studio: the room becomes a record of the player's musical performance, balloons provide readable focal targets, and the beat adds an optional accent. The official Splatoon reference is [Nintendo's gameplay overview](https://splatoon.nintendo.com/en/gameplay/); it is design research, not a source asset or a feature checklist.

Do not copy Splatoon characters, names, UI, maps, weapon silhouettes, sound, music, team rules, turf scoring, ink-swimming movement or visual assets. BalloonBarrage's identity remains balloons, a warm indoor art studio, a song-driven rhythm structure, persistent personal paint and a calm solo exhibition experience. No Nintendo material is included or implied to be licensed.

After Kyudaisai, design study may compare multiple games, including Splatoon, at the level of abstract mechanics, interaction clarity, pacing, camera language and environmental feedback. Each lesson must identify what player problem a reference solves, then create and test a BalloonBarrage-specific answer with original code, layouts, art and audio. A reference is evidence for a design question, not permission to reproduce its content or combine feature lists without a player-experience reason.

The creator selected a post-festival open-world direction on October 30: a central plaza with music districts, free exploration with opt-in fixed-location music challenges, and rival AI as the first AI role. The [open-world design](#open-world-direction--planning-only) defines this branch. It is planning only; preserve the accepted studio mode and begin in a separate graybox scene.

## Open-world direction — planning only

**Selected by the creator on October 30, 2026:** central plaza plus music districts; free exploration plus fixed-location music challenges; rival AI that patrols, competes for balloons and participates in musical duels. The website presents these decisions at [Open-world plans](https://lorendw7.github.io/BalloonBarrage/en/open-world/). No open-world scene, movement, rivals, unlocks or new playable package is delivered by this planning update. The [roadmap](ROADMAP.md#open-world-development-track--planning-only) owns delivery gates. Future core gameplay remains learner-written, one small step at a time, unless direct implementation is explicitly requested.

### Player promise and loop

Explore a bright, handmade festival town in third person, follow balloon landmarks to a small music stage, challenge a mischievous AI troupe from a stable stage view, then bring a musical layer back to the plaza. The creator selected **art direction A + C: a sunny street music festival with handmade art-town details** on October 30, 2026. Keep the studio's warm materials and tactile balloon pops. A night music festival is a later expansion idea, outside the first slice; no day/night system is committed.

Proposed loop: **explore → discover a stage → explicitly start a music challenge → pop and paint against the troupe → see results → collect a musical layer → hear the town ensemble grow**. Exploration has no accuracy grade or time pressure. Entering a district does not automatically start a scoring chart. First prototype: transition from third-person exploration to a comfortable stable stage view after confirmation, reuse one existing song profile unchanged, and return control after results. Ambient music fades out before the chart starts; only the chart's existing clock judges notes. Returning to exploration never carries an unfinished scoring event across modes.

Unrestricted-duration held fire, finite hit cadence and optional on-beat rewards remain the baseline. Missing a beat does not block movement, remove access to a district or force a restart. Rival results are separate from existing solo records. Exploration paint persistence across scenes or application restarts is an undecided storage feature; do not promise it from the current round-persistent studio paint.

### Map concept: one plaza, three distinct districts

The topology is a hub with three short return loops, not a seamless city. Every district entrance should show the plaza's balloon tower or another unambiguous return sign. Color is paired with shapes, signs and distinct silhouettes. Quiet walking paths sit outside challenge sightlines. The first slice includes only the plaza edge, one route and one stage; the rest of the map is a proposal, not production scope.

| Place | Navigation and music role | Balloon interaction proposal | Expansion boundary |
|---|---|---|---|
| Balloon Plaza | Safe arrival, recognizable balloon tower, route signs, practice wall and studio entry | Non-scoring practice balloons and a place to hear collected musical layers together | No active rival contest in the arrival area |
| Percussion Pier | Broad, flat first route; drum-canopy landmark; spacious percussion phrases | Clearly telegraphed targets shared with one rival on a small stage | First graybox candidate; no water traversal or new physics required |
| Melody Garden | Sheltered loop, arch-shaped landmark, quieter melodic call-and-response | Balloon patterns illuminate authored garden decorations | Later content study after the first stage succeeds |
| Syncopation Rooftops | Stepped silhouettes and distant stage; stronger rhythmic contrast | Optional lift-balloon spectacle and trickier authored patterns | Vertical movement, lifts and camera occlusion are separate experiments |

Diagram: Plaza ↔ Pier; Plaza ↔ Garden; Plaza ↔ Rooftops. Optional cross-district shortcuts come later. Initial design targets, not measured results: a roughly 30–60-second walk to the first stage and a 5–8-minute first visit including practice, one challenge, results and return. Scale the map by observed travel comfort rather than kilometres or collectible count. Keep combat targets against calm backgrounds, avoid simultaneous rival threats behind the camera, and provide a direct return path.

### How balloons, rhythm and AI connect

1. **Balloons make the world readable.** Landmark clusters identify routes; stage balloons are the actual chart targets; decoration balloons have a separate appearance and never create competitive rewards.
2. **Rhythm structures the encounter.** Authored phrases determine previews and scoring targets. A good musical hit adds the existing optional bonus and a bounded celebration; free spraying stays valid. Decorative lights may follow the beat without becoming extra notes.
3. **Rival AI creates a readable choice.** Before contesting a target, the rival turns, aims and displays an intent marker. The player can contest that target or choose another visible balloon. The rival must occasionally miss and wait through authored recovery windows; it cannot fire instantly on spawn, act through walls or read future player input.
4. **The selected reward is a growing town ensemble.** Collect musical layers through stage progression and hear them combine in the plaza. An AI loss or low accuracy cannot lock the only exploration route. Exact reward conditions and progression persistence remain design questions; neither is implemented.

### Gameplay principles for the festival town

Use balloons consistently as landmarks, chart targets and stage props, with distinct shapes for each purpose. A completed challenge should leave an authored, readable change: a decoration rises, a light turns on or an instrument joins the plaza ensemble. These visible and audible rewards are planned feedback, not evidence of implemented persistence.

Keep the first route direct. Later optional detours can lead to hidden challenges, while short, replayable stages each introduce one rule. Better performances may unlock variations; exact thresholds remain undecided and access to the main route stays open. Cosmetic balloon patterns, clothes, instruments and performance gestures are later identity options, not first-slice requirements.

The first rival study retains the existing authored chart and target-contest rules. Candidate roles for later studies are:

| Candidate | Proposed encounter | Boundary |
|---|---|---|
| Beat Teaser | Demonstrates a short balloon phrase for the player to answer | Separate authored call-and-response study after the baseline encounter; no runtime chart changes |
| Balloon Porter | Carries an optional task balloon along a readable route; the player chooses an interception path | Opt-in exploration activity, not mandatory pursuit; suspend it during stage play |
| Stage Leader | Announces one challenge modifier, such as a sustained balloon, before the round | Later rule study only; requires its own chart/input design and must not alter an active chart |

These are proposals, not three committed characters or a fixed team size. Actions need advance cues, recovery windows and recoverable mistakes. Pause exploration pursuits during the stable stage view; no off-screen interference with chart reading. Verify one member before a coordinated pair.

### Confirmed art direction — A + C

The selected base is a sunny street music festival with handmade town details. Apply it to the plaza corner, path and single pier stage before extending it to other districts. This is a visual brief, not newly produced art or a playable environment.

| Element | Direction | Readability constraint |
|---|---|---|
| Color and light | Cream buildings, warm daylight, teal and coral accents | Calm backgrounds; reserve strong contrast for targets and interaction cues, paired with shapes/icons |
| Materials | Glossy balloons against matte buildings and ground; paper posters, cardboard signs, fabric bunting and wooden stages | Preserve target edges against busy textures; avoid decorative flashes that resemble scoring cues |
| Silhouette and motion | Rounded balloons, recognizable instrument backpacks; inflation, springy hits and brief deflation | Exaggerated feedback stays bounded and never covers the next target |
| Everyday props | Original combinations of pumps, paint buckets, drumsticks and metronomes | Shared local craft vocabulary; keep prop function legible |
| District culture | Local bands, handmade posters and instrument-shaped landmarks | Pier percussion, garden chimes/keyboards and rooftop street performances are examples, not fixed content assignments |

Learn from Splatoon's integration of a core material, traversal/objectives and a believable local music culture. References: Nintendo's [gameplay overview](https://splatoon.nintendo.com/en/gameplay/), [world and music interview](https://www.nintendo.com/au/news-and-articles/ask-the-developer-vol-7-splatoon-3-chapter-1/) and [everyday-object design interview](https://www.nintendo.com/us/whatsnew/ask-the-developer-vol-7-splatoon-3-part-2/). These inform design principles; maps, characters, interfaces, artwork, fictional bands and music remain original. Preserve attribution and exact generation prompts for any future assets.

Defer the neon/night music festival until the daytime slice is accepted. Treat it as an optional later event with separate lighting, readability and audio checks, not an automatic OW5 deliverable.

### Rival AI: bounded and offline first

The selected opponent is **C — a mischievous troupe**, with multiple original rivals and readable environmental interruptions. Build toward that direction in increments: study one troupe member first, then add a second with coordinated intent and turn-taking before increasing simultaneous pressure. The one-member study is an implementation step, not a replacement for the selected group. The candidate roles above are later studies; exact roles and team size remain undecided. Proposed member states: patrol → notice an invitation → show target intent → aim → fire → recover → return. Use bounded offline behavior rules. In the first slice, patrols outside a stage are presentation only; challenges begin through player consent. Interruptions must not move scoring timestamps, hide required targets or block the only path.

Use the same declared chart and one authoritative target-resolution rule. A balloon can be consumed once. Log target ownership, shot time and resolution; test simultaneous hits explicitly. Define reaction delay, aim error, line-of-sight checks and recovery before tuning difficulty. Rival scoring is a separate experimental mode and must not silently replace the existing 10/20 solo rules. In the first study, disable damage, knockback, hard failure and adaptive difficulty. Later adaptation may select the next encounter tier with the player's consent; it must not change a running chart or secretly weaken the player's input.

### Confirmed detail choices — C / A / B

The creator confirmed these choices on October 30, 2026 and requested planning and website updates only. They are selected future direction, not saved gameplay.

| Decision | Selected direction | Planning consequence |
|---|---|---|
| Rival temperament — C | Mischievous troupe | Multiple rivals with distinct, telegraphed roles; validate one member, then a coordinated pair before a larger group |
| Exploration camera — A | Third-person walking plus stable stage view | Define explicit entry/exit camera transitions, collision/occlusion handling and input handoff; no forced aiming while walking |
| Reward emphasis — B | Collect musical layers for a town ensemble | Each earned layer adds an audible part to the plaza arrangement; first test one layer, then combine compatible parts |

Proposed music progression: percussion at the pier, melody in the garden and bass or rhythmic accents at the rooftops. These layer assignments are examples, not additional confirmed choices. Prepare original or appropriately licensed, tempo/key-compatible stems or loops; do not assume the existing mixed-song catalog contains separable or harmonically compatible stems. Schedule additions at musical phrase boundaries, offer layer mute/level controls and check clipping and phase alignment. The exploration ensemble must yield to the challenge track, with no change to chart timing. Preserve provenance and existing music restrictions. Saving collection progress is a later explicit design task.

Still open: troupe size and exact roles, interruption rules, camera distance/transition settings, layer acquisition conditions, music arrangement and save scope. The initial learner exercise remains a paper route walkthrough and one movement/camera question; the assistant reviews saved learner code before the next gameplay increment. No gameplay implementation is authorized by this planning update.

## Continuous-fire contract

“Unrestricted continuous fire” means the player may hold the fire input for the whole playable round. There is no ammunition pool, magazine, reload, heat meter, stamina cost, rhythm permission gate or forced downtime. Empty shots and surface painting do not punish the player. Releasing and pressing again is a choice, not a resource-reset requirement.

The simulation must still accept a finite, tuned number of hit samples per second. The saved `PlayerShooter` exposes `shotsPerSecond` with a default of 5, while `PrototypeScene` saves 6, and repeats while the mouse button is held. Each accepted sample performs an immediate bounded SphereCast; the visible stream is a separate looping particle presentation rather than a flying gameplay projectile. This cadence controls feedback clarity, audio overlap, collision load and persistent-paint growth; it is not an in-world firing restriction. Tune the one serialized value through playtests rather than deleting the gate or querying collision every frame. Menus, pause and focus changes may require the button to be released once before firing resumes so UI clicks do not become accidental shots.

The first learner exercise is [Continuous-fire and paint-loop lesson](CONTINUOUS_FIRE_LESSON.md). It begins with observation rather than a code rewrite because the saved input path already satisfies the no-ammo/no-reload contract.

## Whole-game loop and mode boundaries

Public demo.6 preserves title/instructions → one complete song → results → retry/title. The revised loop adds a mode selector before the song list: Festival contains ten approximately 64–65-second profiles; Standard contains all ten complete recordings. Both use preview → Play → painted results → retry/title. Separate versioned profile keys keep records apart. The learner's original pattern/effect functions remain intact; explicit section data selects among those pattern families. Source wiring and package acceptance remain distinct; see the roadmap and Validation.

| Mode | Intended experience | State and scoring boundary |
|---|---|---|
| Kyudaisai queue | Ten short profiles, each about 64–65 seconds, within the 60–75-second target | Existing base/on-beat rules; separate v2 profile/chart identities isolate records. Festival is the default mode |
| Standard song / fallback | All ten complete recordings with bounded derived charts; demo.6 retains the original complete Goin' Under reference | Complete recording end owns settlement; standard and festival records cannot mix. Original source charts/audio remain intact |
| Local co-op | Two independently controlled reticles in the same studio and song | Shared score and one consumed target outcome; explicit input/player identity supports later contribution statistics |
| Roguelite run (exploratory) | A possible later branch, only if it supports the relaxing core experience | Not a committed milestone; keep standard songs available without upgrades or pressure |
| Practice / free paint | Possible later place to learn aim or decorate without score pressure | Separate from chart accuracy and competitive records; not a prerequisite for the first release |

Color, sound, light and paint should make an accepted action easier to understand. They must not silently alter timing rules, move the logical muzzle through the environment or obscure the next target. Settings, localization and records support the accepted loop rather than requiring an online service. The [roadmap](ROADMAP.md) owns milestones, dependencies, priorities and release gates.

## Exhibition slice

The saved exhibition foundation is one complete-song rhythm shooting demo using Goin' Under / Beginner, about 123.429 seconds and 93 prepared events. Studio music belongs to interfaces; gameplay uses the full non-looping song. Public v0.1.0-demo.6 includes DSP timing, 0.8-second previews, target binding/cleanup, firing-time matched-target 10/20 rewards, the separate-index localized NEXT cue with a global approach fill, enlarged same-color burst, golden accent and selected 1.45-second reward text. Reticle, audio, bounded recoil, pause/results/retry, nine prepared paint receivers, local records and result statistics are saved. Preserve this package as a fallback; any shorter multi-song candidate is a separately versioned build with its own checks. Listening and physical-readability judgments remain separate from the automated checks in Validation.

The saved teal latex-and-paint burst is connected. Additional tactile feedback follows acceptance of the rhythm loop: independent gentle visual bobbing, a short entrance expansion, a brief hit squash, bounded gun recoil and round-persistent surface paint from sampled spray impacts and balloon pops. Their intended behavior is specified below. Delivery order and cutoffs live in the roadmap.

The intended experience keeps music, beat cue, shot judgement, scored impact, pause and retry working together. The current source saves short-round audio seeking, chart filtering, a post-Start dropdown with preview, isolated record keys, three learner-authored pattern/effect decisions, contact-aware bursts and eight effect references. Validation records the exact automated scope; the roadmap preserves the play/package acceptance boundary.

## Kyudaisai feedback revision — source candidate

Recent anonymous playtest feedback praised the care in the build while identifying two problems: the two-minute round felt mechanically uniform, and the balloon-hit effect sometimes appeared offset. The revised candidate addresses those observations without changing the calm, forgiving scoring contract.

| Area | Candidate behavior | Acceptance boundary |
|---|---|---|
| Session length | New festival windows last 64.000–65.455 seconds, at unchanged speed/pitch; standard mode plays complete recordings | Beat-bounded windows/sections are inferred from source timing and signal analysis. Human listening and phrase quality remain unverified; see Validation for runtime evidence |
| Song choice | Festival/Standard mode selector, then ten matching choices with preview, Play and Back | Clip, chart, duration and versioned key change together. The operator may recommend two easy choices; all ten remain available |
| Balloon barrage | Learner code chooses lateral sweep, shallow fan and staggered cluster; free-row spacing supports up to 16 active targets | Source implementation is saved; complete-round human readability remains unverified |
| Pop response | Eight bounded fragment prefabs are selected by the learner's color-to-variant mapping, with the original effect as fallback | Source implementation is saved; dense-passage visual acceptance remains unverified |
| Effect alignment | `PlayerShooter` and `PaintProjectile` pass the sampled contact to `Balloon.HitAt`, and the independent burst uses that point instead of the balloon root | Static source checks confirm both call sites and the serialized world-space effects. Center/edge hits from several screen positions must still be observed; the user-reported offset has not been reproduced or declared fixed |

Original downloaded audio/chart files and notices remain unchanged. A short round should be represented by explicit cue-in/cue-out and event-window metadata at musical phrase boundaries, not by overwriting source audio or changing pitch/speed. Confirm that the intended noncommercial exhibition use permits the chosen playback window; if that cannot be established before freeze, use an unmodified complete track or the demo.6 fallback. See [Music and charts](MUSIC_AND_CHARTS.md) for provenance and technical constraints.

## Music-led festival variety

The design goal is a short musical performance that leaves a personal painting. More songs, denser targets and extra particles alone do not solve uniform play. Music should change where the player aims, when an optional accent feels satisfying, and when a quiet passage allows free painting. The revision consumes explicit section/placement data while preserving the learner's elapsed-thirds fallback. Section boundaries follow source tempo maps or signal-estimated phase; they are not human-confirmed musical form. The table below separates the implemented slice from later ideas.

### Short-round structure

Target **60–75 seconds of gameplay, preferably about 65 seconds**, plus approximately 10–20 seconds for instructions/selection and 10–20 seconds for results/handoff. Aim for a 90–120-second visitor turn. The prepared festival profiles last about 64–65 seconds and preserve old source/profile files. Complete recordings belong to standard play. Do not accelerate the music or end abruptly on a timer.

The following 68-second example illustrates pacing, not verified cue points for any song. Move boundaries to actual phrases after listening; use the audio/chart's tempo map rather than assuming every phrase lasts the same number of seconds.

| Approximate position | Musical role | Player interaction |
|---|---|---|
| 0–8 s | Clear opening pulse | A few separated targets introduce aim and the existing beat bonus; avoid a long empty intro |
| 8–24 s | First theme | A readable lateral sequence invites following the melody across the studio |
| 24–40 s | Answer / contrasting phrase | Change to a shallow fan or reversed sequence; repeat one recognizable motif so players can anticipate it |
| 40–56 s | Chorus / strongest phrase | A compact staggered sequence adds directional variety without requiring more simultaneous hits |
| 56–68 s | Musical resolution | A short reprise ends on an authored final accent, then exposes the painted studio for results |

Prefer fewer meaningful events to compressing the same event count into less time. Preview the first event early enough for aim; retain the current preview lead unless a separate change is tested. Include a sparse phrase for free painting where the music supports it. If no coherent 60–75-second window exists, choose another prepared song or retain the verified fallback instead of making an awkward cut. The first exhibit pair should contrast a spacious, easy-to-read chart with a livelier sequence; choose by listening and aiming evidence rather than track popularity or BPM alone.

### Interaction palette

This table describes the Classic song rules. The separately selected [rainbow chain](#optional-rainbow-chain--source-candidate) uses its own scoring-record identity; it does not alter Classic or add the deferred paint ribbon.

| Interaction | How music drives play | Rule and scope |
|---|---|---|
| Phrase-authored sweep, fan and staggered sequence | Different phrases change aim direction, spacing and recurrence; the returning theme repeats its recognizable pattern | Implemented source/data slice. Reuse the three learner pattern families, one target per derived chart event and existing rewards; timing/phrase listening remains separate |
| Target-associated approach cue | A shrinking outline or simple shape identifies the target whose beat is next, so the player can connect the sound to the aim | Small follow-up trial. Use the matching event and DSP clock; check overlap, ordinary hits and color-independent readability before adding another HUD layer |
| Listen, then answer | A brief noninteractive shape/light motif previews a left/center/right sequence; the next phrase supplies matching, normally hittable balloons | Implemented source slice: an opening three-position motif, plus the bounded Goin' Under showcase mirror described below. Markers have no collider/raycast blocking/reward or compulsory memory test. Answers are real authored chart events; preview starts after the demonstration, while judgement time remains fixed |
| Long-note paint ribbon | During a sustained musical phrase, a visual path invites following it with held spray across an already eligible wall | Explicitly deferred by the creator; no runtime guide in this version. If later selected, first test visual guidance with ordinary deposits and no extra score. Tracking accuracy, duration rewards or new event types require separate rules and record identities; do not reinterpret imported hold tails silently |
| Chorus relay and final accent | Distinct chart events form a compact readable route, ending with one focal balloon and a restrained musical/visual accent | Implemented only in the all-song showcase below. No simultaneous chord requirement, extra mandatory clicks or boss health. A decorative larger balloon must retain a clear hit silhouette and resolve once |
| Personal painted ending | Players use spare phrases to decorate, then see their own marks behind the result panel | Existing persistence supports this direction. A quieter result composition is planned; image export, artwork saving and themed mural objectives are later features |

Keep new song interactions in separate bounded phrases. Do not add a color-match requirement to C-key switching, shielded targets, hazards, forced rhythm input, or mid-song upgrade choices to the festival slice. Direction, spacing, musical repetition and optional painting provide variety without another control lesson. Show any new invitation with a short localized label and shape, not color alone.

### Phrase data and fairness contract

The versioned derived chart stores sections with round-relative start/end, pattern family and direction; events store source-audio times and authored slots. Demonstrations store round-relative start, step duration, answer start and three slots. The reader subtracts cue-in once, selects the matching section, applies direction once and gates only answer previews until the demonstration ends. The catalog identifies song, mode, audio, chart, window and record key. See [Music and charts](MUSIC_AND_CHARTS.md#festival-and-complete-song-phrase-catalog) for schema and reproduction. BPM/beat boundaries must be converted through the existing timing map. Review cue-in/out, event inclusion and the musical ending together. New practice grids require listening/offset calibration and phrase review before being described as musically authored charts. Source recordings, original charts and notices remain intact; [Music and charts](MUSIC_AND_CHARTS.md) owns provenance and usage boundaries.

Use the existing one-clock, one-event, one-accepted-pop model. An early ordinary hit consumes that event normally; it cannot return at the beat for a second reward. Held fire remains a valid way to play and may earn the existing bonus when its accepted sample reaches the matching target in the saved window. New cues do not automatically award a bonus, widen the window or invent additional scoring events. Decorative guides and echoes never score. Any future scoring mechanic uses a new rules identity and cannot compete against old records.

Author spatial/event spacing against the actual finite firing cadence and aim travel. Do not require a simultaneous chord or intervals shorter than the player can sample and aim; do not increase shot frequency to rescue a dense chart. Keep the saved 16-target limit as a capacity bound, not a density goal. If a chart fills capacity and delays a pending event beyond its intended cue, reduce authored congestion and recheck timing; do not move the judgement time or silently skip events. Ordinary off-time play must still finish comfortably without a failure state.

Before promoting a trial, compare it against the same short profile with the existing patterns: can a player describe at least one musical/aiming change, identify the next target, and finish without a required extra input? Record timing/readability observations separately from automated clock, score, pause, cleanup and Retry checks. These are criteria for choosing a new design, not claims that existing packages passed human testing or a requirement to repeat unrelated release work.

## Optional tricolor relay

On October 30 the creator explicitly authorized Codex to write this additional gameplay. The three learner functions stay intact. The demo.11 candidates include this small, optional collection goal in both existing song modes. Demo.10 remains the immutable earlier package without it.

Pop any three distinct balloon pigments to complete one relay. The lower-left HUD shows numeric progress, three collected swatches and the completed count. Completing a set briefly pulses the three HUD swatches, then collection starts again. A repeated pigment still earns its normal hit score and preserves partial progress. Pigment identity comes from the popped balloon, not the player's selected spray color; C remains optional decoration control. The next successful pigment starts the next set immediately, even during the celebration.

There is no additional deadline, required order, color-match shot, failure, score multiplier or automatic secondary pop. Expired balloons do not erase collection progress. Existing 10/20 rewards, rhythm windows, chart events, target capacity, song durations, retained paint and record keys remain unchanged. Completed relays are a separate per-round keepsake shown as the fourth result statistic, without persistent records or rewards. Paused and post-round pops cannot collect; pause hides the overlay and freezes the brief celebration. Retry resets counts, swatches and celebration state and destroys the old overlay. Both text and numeric progress explain the goal without relying on color alone.

The implementation uses a pure progress model, a successful-pop notification and one bounded HUD. No new art, music, scene copies or world particle systems are needed. [Validation](../Project/VALIDATION.md) records the actual automated evidence and limits; source checks are not a standalone release or a human playtest.

## Optional rainbow chain — source candidate

On October 30 the creator explicitly authorized direct implementation of another gameplay addition after first receiving its scope. The optional Rainbow chain ruleset extends the tricolor relay with a banked burst. It is saved source, not part of the published demo.11 ZIPs. Artwork postcards/export remain deferred at the creator's request.

Before Play, select Classic or Rainbow chain independently of Festival/Standard song length. Classic is the default and retains its original scoring records. The rules selector and short explanation are localized in Chinese, Japanese and English. Rules cannot change after a round starts. Retry returns to the normal title with Classic selected and all transient counters cleared.

Complete a tricolor relay through direct balloon hits to bank one charge. Charges do not stack or expire. Empty shots and ordinary surface painting do not consume it. The next direct hit on a chart balloon spends the charge, even if no eligible neighbor is nearby: choose a cluster to make the burst useful. First spend an existing charge, then collect that direct hit's pigment. A hit that completes a set cannot spend its newly earned charge immediately; a direct hit can legitimately spend an old charge and earn another for the following hit.

One burst resolves at most two additional, already spawned chart balloons within 1.8 world units of the primary balloon. Choose nearest first, using event ID to break distance ties. Ignore resolved, inactive, expired and non-chart targets. A shared bounded nonallocating raycast planner prevents crossing scene geometry and simulates already-selected neighbors disappearing. Secondary hits resolve through the existing one-time pop guard, effects, paint and event cleanup. They receive their ordinary base points and successful-hit/streak accounting, with no rhythm bonus, no pigment collection and no further burst callback. A single direct timed hit can therefore score its normal 20 plus up to two normal 10-point neighbors; it cannot multiply a rhythm reward or create new chart events. Original event times, capacity, audio, cadence and continuous-fire input remain intact.

The existing relay HUD shows a ready instruction and a brief localized burst/neighbor count; the actual neighboring pops use the existing pigment effects. No new asset or world particle system is required. Results add a separate burst count. Charge, flash and collection state freeze during pause, cannot advance after settlement and reset on Retry. Rainbow chain records append `.RainbowChain.v1` to the existing song/profile scoring key; Festival and Standard remain separate. Existing profile records keep their keys; the new showcase profile below has a separate identity.

The first automated trial covers charge order, cap/radius/occlusion, one-time outcomes, secondary-score boundaries, pause, record isolation, localized selection/results and reset. [Validation](../Project/VALIDATION.md) owns exact results and the distinction from natural mouse-input play, musical comfort, packages and venue acceptance.

## All-song showcase — v4

The creator expanded the trial to all ten songs in both Festival and Standard on October 30. Every one of the twenty profiles now uses a separately versioned `-showcase-v4` chart/profile and `.Rhythm.Showcase.v4` record key. Historical v2/v3 files and records remain preserved. Event counts, event IDs/timestamps, audio bytes/speed/pitch, track windows and source attribution are unchanged. Postcards/export and the long-note paint ribbon remain deferred.

Each profile has the following separated interactions, selected deterministically from its actual existing notes:

- **Mirror echo:** near the middle, numbered noninteractive outlines demonstrate left → center → right. Three existing consecutive notes answer right → center → left. Match these by event ID rather than inventing equally spaced judgement times; Springtime's irregular intervals stay intact. The watch step fits the local note gaps, and explicit response end metadata keeps the faint guide visible through the actual final answer. Resolved guides disappear. Holding fire remains valid.
- **Melody trajectories:** a later eight-target phrase uses four shallow arcs and four waves, with at most 0.18 horizontal / 0.12 vertical world-unit displacement. It starts after the mirror response and ends before the final route's previews. Motion period follows each track's original beat-step scale. Logical body, collider and appearance move together on the paused DSP song clock; timestamps remain fixed. The spawner's serialized bounds and minimum 0.75-unit root spacing remain enforced; blocked motion waits. Ordinary physics/float are restored at phrase end or disable.
- **Firework finale:** the last three existing events form left → right → center, with numbered faint outlines following real targets. The final chart event adds three brief existing pigment effects on its one accepted pop. It still scores once; unhit expiry creates no effect or consolation score. The same one-time resolution guard applies to chain pops.
- **Chain preview:** available on every song/mode when Rainbow chain rules are selected. The existing shooter SphereCast identifies the primary; the shared nearest-first/radius/occlusion planner previews up to two neighbors without changing charge, score or timing. Firing recalculates the snapshot. Pause, invalid aim, input cancellation, settlement and spent charge hide it. Co-op follows the most recently moved player's aim; the shared relay still banks one shared charge.

All guidance is nonblocking and localized in Chinese, Japanese and English. Joint phrase selection ensures sparse short charts have room for all three song interactions without overlap. Human listening, aiming comfort and real-device acceptance remain distinct in [Validation](../Project/VALIDATION.md). Demo.13 packages this expansion; earlier packages retain their original content.

## Sequence guidance and celebrations — source revision

The creator's subsequent optimization request raises the production scene's simultaneous target cap from 16 to **24** and extends the production balloon's unhit cleanup from eight to **12 seconds after its chart event**. With the unchanged 2.8-second early appearance, an unhit target may therefore remain for about 14.8 active song seconds. Keep the three-second drift wait, 0.75-unit spawn spacing, bounds, hit timing, audio and score values unchanged. A full/physically blocked field retains the original pending event rather than overlapping or silently discarding it; settlement still clears remaining targets. The original production GUIDs and prefab inheritance remain intact. Packaging and cleanup evidence belong to [Validation](../Project/VALIDATION.md).

On October 30 the creator explicitly requested direct implementation of stronger ordered/rhythm guidance, the A+B+C Kyudaisai presentation below, clearer chain results, celebrations and improved gamepad feel. The revision is included in the demo.14 candidates published October 31 after the official-website request. Preserve the learner's concurrent script/shader changes and existing judgement, cadence, record identities and scoring. [Validation](../Project/VALIDATION.md#demo14-publication-and-official-website--october-31) distinguishes publication from remaining physical/native acceptance.

**Sequence route:** Classic and Rainbow chain can show up to three already spawned, unresolved, onscreen targets, sorted by original event time then ID. Hollow gold/teal rings, numbered badges and shallow curved arrows suggest an order. Numbers remain 1, 2, 3 within a three-hit phrase. A moving highlight indicates direction; it is not a second judgement clock. The brighter outer ring contracts to the fixed body ring at that target's original event time. This means “hit when the rings meet for the existing +10 beat bonus,” not expiry, a required hit or an automatic explosion. Ordinary hits retain their base points. Missed timing cues leave the still-live balloon freely shootable.

Three consecutive direct hits on the suggested current target trigger a one-second “A LITTLE MELODY!” message and expanding gold ring/eight rays. Order adds no score, multiplier or automatic hit. An out-of-order hit or a skipped timing cue quietly restarts this cosmetic three-hit progress without affecting accumulated points. Secondary chain hits do not complete it. Guidance and celebration use a fixed UI mesh and three reusable badges; they add no colliders or world lights. They yield to demonstrations/finales and the actual chain preview/burst; pause hides them and freezes their active duration, and settlement clears them.

**Chain route:** the existing charged rule remains one aimed direct hit plus at most two eligible neighbors. The primary marker says AIM; neighbor markers show their base points, and straight branching connections mean “this one shot will also pop these targets.” A zero-neighbor preview advises aiming at a cluster. An actual burst retains a short snapshot, draws outward gold beams to the accepted secondary positions and reports the actual extra count and extra points. A charge spent without neighbors explicitly reports zero extras; it receives no invented chain reward. The preview and burst remain separate from curved sequence arrows, the beat countdown and the unchanged 10/20 direct-hit reward.

### Kyudaisai 2026 A+B+C presentation

The creator selected A+B+C after reviewing the official 79th Kyudaisai theme **Akatsuki (暁), dawn/new beginnings**, and the candle/sky-lantern ending. Reference sources, checked October 30, 2026: [official theme](https://kyudaisai.jp/79th/theme/) and [opening/ending program](https://kyudaisai.jp/79th/op-ed/). These motivate original procedural graphics; no official mascot, logo, photograph, music or copied website asset is included.

- **A — dawn title:** retain the studio background and existing controls/credits, add indigo-to-clear and coral edge washes, sunrise arcs, a slow sunray, six drifting outline balloons and a localized festival heading. Start and language selection remain available immediately. Song selection/gameplay hides this title decoration.
- **B — nine lights:** entering results lights nine small lamps in sequence and raises four outline balloon lights along the side margins. Scores and Retry remain unobstructed and immediately usable. Celebration has no score threshold and does not alter settlement. This is a presentation inspired by the official ending, not an official event/reward claim.
- **C — hidden 79:** activate the title's small 79 button three times within six seconds. Reveal an original nine-balloon constellation and a localized “See you here again” blessing. A modal backdrop blocks clicks on underlying actions, Close receives controller/keyboard focus, and Close/Esc/gamepad East returns immediately. Partial taps expire; leaving the title clears the modal. No persistent unlock, real-clock restriction or online service is required. The menu treasure hunt option D remains unimplemented.

Festival presentation is decorative and available offline, independently of machine date. Chinese, Japanese and English text uses the existing localization table/fonts. [Validation](../Project/VALIDATION.md#presentation-and-controller-source-revision--october-30) owns actual checks and physical-input limits.

## Local players and gamepads

The selection screen offers mouse solo (default), gamepad solo, mouse + gamepad co-op and two-gamepad co-op. Both players share one camera, song clock, target set, paint surface, score and optional relay/chain state. Numbered reticles identify their aim. This is local play without networking or split screen.

Gamepads use either stick for aim, held RT for spray, X/West for pigment and Start for pause. Existing Input System UI actions provide stick/d-pad navigation, South submit and East cancel. Assign controllers once at Play in stable device-ID order; a new device cannot steal a slot. Require the controller count before Play. Losing an assigned controller pauses/cancels input, requires the original device to return and requires trigger release before spraying again. Co-op records append `.LocalCoop.v1`; gamepad and mouse solo share solo scoring identities. Retry restores mouse solo.

The creator completed both learner-owned `CoopLearnerCore` decisions: `now >= nextAllowedShotTime` permits equality, and on-beat hits add ten bonus points while ordinary hits add zero. Both flow through the real P2 shooter; ordinary hits total 10 and timed hits 20. A resolved target cannot score twice across players. [The completed lesson](LOCAL_COOP_LESSON.md) explains these decisions; future core increments remain learner-owned. Physical-device and real-Mac acceptance remain separate from automated virtual-state checks.

The later October 30 source revision replaces the fixed 800-pixel/second cursor with screen-height-based motion, an explicit 0.16 raw radial deadzone and a gentle response curve for micro-aiming. Either stick works, with right-stick priority; returning to center stops immediately. Near a live target, low/mid deflection slows to 55%; strong deflection escapes at full speed. This changes cursor transport only: no aim snapping, enlarged cast, automatic shot or timing change. Frame steps are capped at 50 ms to prevent a hitch from jumping the cursor. Brief assigned-controller feedback distinguishes ordinary hits, beat hits and actual multi-target bursts; pause, focus loss, suspension, disable and settlement stop outstanding vibration. Motor support and comfortable values on the creator's Xbox/Beitong devices remain physical acceptance work.

## Required rhythm loop

Retain StudioDaylight_Loop.wav as interface music. The packaged reference starts gameplay with the downloaded Goin' Under by NegaRen and its dance-single Beginner chart by Fraxtil: constant 210 BPM, source OFFSET 0, 93 tap-row events containing 95 original lane taps. The complete audio is 5,443,200 samples at 44,100 Hz (123.428571 seconds), without looping or a crop. Its first chart event is at beat 32 / about 9.143 seconds and its last is at beat 416 / about 118.857 seconds. Demo.6 preserves that complete intro/ending and settles at song end. The source candidate defines separate credited cue/event windows without altering this baseline.

Use the prepared importable audio, non-looping AudioSource prefab and derived event JSON documented in [Music and charts](MUSIC_AND_CHARTS.md). First teach menu/game playback switching, then scheduled audio with one DSP origin for chart events, timer and settlement. Keep first-beat offset and signed input calibration explicit. The [first audio lesson](RHYTHM_FIRST_LESSON.md) is a foundation exercise, not chart judgement. Do not substitute an arbitrary constant beat grid for the actual chart.

Saved solo defaults: a hit awards 10 points; an accepted sample within 100 milliseconds of its aimed target's chart event can add 10 points when the SphereCast reaches that balloon. `PlayerShooter` reads song time after its input/cadence gates, performs the bounded cast from the muzzle and evaluates `Balloon.IsOnBeat` immediately before calling `Balloon.HitAt` with the sampled point. The balloon's single-hit guard resolves base plus bonus once. Explicit player identity is still required before co-op integration. Off-time hits retain base points. Window and reward tuning remain subject to listening/playtests; user-reported 10/20 scoring is not complete-song acceptance.

The saved cue is the global NEXT countdown and approach fill plus accepted on-beat feedback. NEXT skips events already resolved by a hit or expiry, including targets popped early or out of order; this cue-only tracking resets when a chart loads and does not implement a new scoring registry. Per-target timing rings/countdowns remain proposed. The current scene uses a 2.8-second spawn lead and keeps unanswered targets for eight seconds after their beat; the on-beat bonus window remains 0.1 seconds. Balloons wait for three seconds of active play after appearing before starting their original 0.6-unit-per-second random drift. Their Rigidbody position is frozen during the wait so collisions cannot scatter the initial formation; the original constraints are restored before drifting or when the component is disabled. Pause does not consume this delay. The spawner enforces 16 active targets, reuses freed slots and tries alternate rows in the authored column, then searches the bounded area for the nearest available fallback with at least 0.75-unit center spacing. If capacity or placement is blocked, the chart reader retains the pending event instead of overlapping targets, advancing silently or changing its judgement time. Simultaneous source lanes share one scoring target; lane metadata is provenance. Expiry resets streak without awarding points. Judge accepted cast time without projectile travel delay. Positive inputCalibration adds to shot time; source StepMania OFFSET is converted with the opposite sign. This source tuning follows demo.8; the published demo.8 packages retain their recorded behavior.

Held-fire cadence can repeatedly miss the beat even when the player starts accurately. Compare manual taps with held fire during playtests before choosing bonus and combo rules. Keep baseline free shooting useful, provide visible timing feedback, and tune accessibility independently of damage. Do not solve this tension with ammo, reload, heat, forced tapping or a permission-to-fire window.

Pause/resume and retry must restore song, timer and chart scheduling together. The source candidate derives short-round time from one scheduled origin, subtracts accumulated pause duration, seeks the selected clip to cue-in, filters/normalizes its event window and stops at the profile duration before returning to interface music. Measure latency and observe pause/Retry on the exhibition PC. Supporting preparation handles tap charts with BPM segments and the runtime selects either prepared path; stops, holds, mines and a general SSC importer remain later work.

The saved pigment index coordinates the gun accent, stream and direct surface deposits; C cycles its eight entries during active play. Balloon colors select their own matching bursts/deposits and never impose a color-matching scoring rule. The October 27 broad-coverage pack supplies four silhouettes across eight pigments: direct deposits use puddle, burst and bridge families, and balloon pops use broad bursts; legacy resources remain available as fallback data. The local-space visual stream derives its length from cursor contact and sorts above transparent graffiti and persistent marks; authoritative hits still come from the finite immediate SphereCast cadence. Wall marks add a short visual-only scale pulse. See the [palette guide](../Art/BALLOON_COLOR_LESSON.md) and [paint-art guide](../Art/PAINT_SPRAY_STYLE.md) for assets and provenance; Validation owns the 72-contact, persistence, natural-song and Retry evidence.

## Exhibition experience design

**Implementation status, October 28:** successful-hit/on-beat counters, forgiving streak, result bindings, concentrated continuous spray, coordinated pigment selection and Chinese/Japanese/English switching are saved and included in demo.6. Automated coverage and package checks are recorded separately in Validation. Formal event-outcome accounting, per-target cue association, milestone flourishes and additional animation remain proposals. Calendar, cuts and ownership live in the [roadmap](ROADMAP.md).

### Player promise and pacing

The intended rhythm is aim → fire → immediate accepted hit → pigment burst/fragments/paint → readable success/streak response → next pattern. On-beat success has a recognizable extra accent; ordinary hits still feel worthwhile. In the planned short profile, begin near a clear musical phrase and surface the first readable target quickly enough for a queue setting. Do not add unrelated tutorial targets or silently desynchronize chart and audio.

After the song, keep the painted studio visible behind a compact, friendly summary. Lead with a positive completion line, then score, best score, successful hits, on-beat hits and maximum streak. Retry is clear but optional. Do not foreground unanswered targets, accuracy grades, rankings or a broken combo. No health, ammunition, elimination, score subtraction, score multipliers, mid-song upgrades, random target rules or online leaderboard is required.

Exhibition copy should invite play without promising a medical stress-relief effect. Proposed short Japanese copy: "音楽に合わせて、気ままに風船を割ろう。" Controls should explicitly allow held fire and explain timed shots as an extra reward, with no need to hit every beat. Verify all glyphs before integration; this text is not yet saved game UI.

### Event outcomes and statistics — partially saved

The saved counters use accepted balloon impacts; a future formal event-outcome registry should use the loaded chart's valid identities as the denominator: 93 for this chart, not shots, paint marks or original lane taps. In that proposed registry, each event starts unresolved and commits one terminal outcome: ordinary hit, on-beat hit or missed/unanswered event. A shared accepted resolution would keep score, counters and streak consistent while retaining the existing target hit guard. Unknown/unbound ids must not contribute to formal chart statistics; duplicate outcomes must not emit another success/effect.

The October 23 source checkpoint increments successful hits and current/maximum streak only after `Balloon.Hit` passes the balloon's existing one-pop guard; the saved positive bonus selects the on-beat subset. An unresolved balloon reaching the existing cleanup deadline resets current streak, and `Score.ResetScore` clears all round values. This does not yet implement the proposed missed-event total, a separate event-outcome registry or final settlement accounting. The focused cases below and natural Results/Retry flow remain required before acceptance.

| Value | Definition |
|---|---|
| Successful hits | Ordinary plus on-beat hits, counted only after an actual accepted scored impact |
| On-beat hits | A subset of successful hits: the existing firing-time judgement reaches its matching event and earns the saved bonus |
| Ordinary hits | Successful chart hits without that matching bonus, including off-time/mismatched-event shots |
| Missed events (proposed, not saved) | Unresolved targets reaching the existing cleanup deadline; at natural settlement finalize any remaining chart events once before showing the summary |
| Maximum streak | Highest current-round hit streak; initially not a separate persistent record |

Preserve selected-profile settlement, the target cleanup defined in [Required rhythm loop](#required-rhythm-loop) and the finite six-samples-per-second production cadence. A sampled input is not a successful hit unless its cast reaches a balloon. Surface and empty samples are not missed chart events. Quit must not fabricate completed-round statistics. Pause commits no new outcomes. Retry resets round statistics while retaining per-profile records. Do not retune timing as part of statistics changes.

The current counter checks require `onBeatHits <= successfulHits` and, with unchanged rewards, `finalScore = 10 * successfulHits + 10 * onBeatHits`. Five hits including two on-beat hits mean three ordinary hits and 70 points, not seven hits. A later formal registry must also establish `successfulHits + missedEvents = chartEventCount` and `ordinaryHits = successfulHits - onBeatHits` at natural settlement. Do not claim that missing registry exists or add it during an art pass. No accuracy percentage or S/A/B grade is required.

### Forgiving hit streak — saved behavior

- Start current and maximum streak at zero. Accepted ordinary/on-beat event hits increment current and update maximum, in actual resolution order rather than trigger-pull order.
- Accepted unanswered-target expiry resets current streak. Merely passing the scheduled beat does not: preserve the saved cleanup/grace behavior.
- Empty shots, surface paint, ordinary off-beat hits, held fire and pause do not independently break a streak.
- Duplicate Hit, expiry and settlement callbacks cannot advance/reset an already resolved event again. Results show maximum streak rather than a misleading current streak after final missed-event resolution.
- No streak score multiplier: existing best-score rules/key remain comparable. Any later reward/window change needs a separate scoring-version decision.
- If trialled, a reset is quiet: no red warning, buzzer, camera jolt or lost accumulated score. Hide or omit the live streak if it encourages anxious monitoring. Do not name this field a rhythm combo: ordinary hits also extend it.

Acceptance example: three hits give current/max 3; an unanswered-event expiry gives current 0/max 3; the next hit gives current 1/max 3. Duplicate Hit and surface-only shots do not change these outcomes twice. Check natural results and Retry against the saved counters; the proposed missed-event identity is a separate future contract.

### Feedback hierarchy and trial tuning — planned

These are starting trials, not accepted final values. Improve silhouette, timing and contrast using existing assets before increasing counts.

| Moment | Intended response | Guardrail |
|---|---|---|
| Accepted fire | Existing reticle, sound and bounded gun recoil | No compulsory camera shake or feedback on rejected input |
| Arrival | Visual child about 85% → 103% → 100% scale over 0.20 seconds | Fixed root Rigidbody/collider; verify visual/hit alignment and remove overshoot if needed |
| Ordinary hit | Immediate outcome/score lock and collider disable, then about 0.06-second visual-child squash; independent contact-anchored pigment puff, small latex-fragment scatter and deposits | Trigger once; delayed visual destruction cannot leave a live target, offset the effect or duplicate score |
| On-beat hit | Matching contact-anchored response plus clearer small gold ring/sparkle and an optional brief audio accent | Initially preserve existing 35% burst enlargement and selected 1.45-second reward text; no full-screen flash or unbounded extra paint queries |
| Streak | One small count with a restrained approximately 0.12-second scale pulse | No text object spawned per hit; keep aim/next-event cue visible; no score multiplier |
| Milestone | Trial accents at streak 5, 10 and 20 using existing palette/effects | Replace/retrigger one slot, never queue labels or overlapping accent voices; optional polish is cut first |
| Results/new record | Positive completion line, visible painted studio, score/best, reliable hit/on-beat counts and maximum streak | No failure-focused Miss count or grade. Save strict record improvement once; ties/lower results still get a welcoming finish |

Animate explicit visual children. Do not animate the dynamic root or stop global time for hit stop. Logical muzzle, aim, collisions and music remain independent. Because a playtester reported an occasional visible offset, carrying the accepted sampled contact into the independent burst is now a correction priority; the effect must survive target destruction without following a stale or animated root.

Audition sound against music during dense held fire. Keep the accepted base shot audible without masking the track. A new on-beat accent is brief with bounded overlap; do not change music speed/pitch or duck the whole track by default. Retain at most 32 particles per pop and the two-second transient fallback until a measured revision is accepted.

Keep persistent paint through results, without a count cap, timed fade or oldest-first removal. Preserve pigment and actual contact on the nine deliberately prepared opaque static receivers; glass, UI, balloons, particles, moving roots and unprepared meshes remain ineligible. Further feedback trials must use original or attributed project assets and preserve the frozen release until separately checked. Do not reproduce Splatoon assets, UI, fonts, weapon/character designs, sounds or territory-control rules. Fix flicker/floating marks/target contrast before adding extra variants or curved projection. Extra bob, richer milestone effects and result ornaments remain optional.

### Exhibit communication and acceptance — planned

Use concise Japanese-first instructions with checked glyphs: mouse aim, held fire, C to cycle pigment, fire near countdown zero for a matching-target bonus, Esc pause and Retry. The saved UI defaults to Japanese and offers Simplified Chinese, Japanese and English from the same screen hierarchy, with a persisted choice and refreshed dynamic labels. The small author-credit font does not cover general UI copy; bundled M+ 1c and Noto Sans SC have connected glyph/layout evidence recorded in Validation.

An optional later observation can include someone who rarely plays rhythm games: ask whether ordinary popping feels good, timing adds a recognizable bonus, targets remain visible and anything feels tiring or punishing. A relaxed completed round matters more than a higher score or a forced retry. Change one parameter at a time. Native layout checks, source/reference checks, synthetic outcomes, manual feel reports and clean-player measurements remain distinct evidence. Human observations were excluded from the October 27 automated pass; do not describe them as passed or silently reinstate them as release blockers.

## Feedback and aiming targets

The October 29 input repair immediately clears the nozzle stream and shot audio on focus loss, application suspension or shooter disable, restores the system pointer and resets recoil. Unfocused or suspended input cannot aim, fire or change pigment. Focus regain, application resume and re-enable require a released mouse button before another press can fire. This protects input without adding automatic pause or changing the music clock. [Validation](../Project/VALIDATION.md#focus-recovery-and-long-song-spray--october-29) distinguishes callback assertions from physical Alt+Tab acceptance and records the sustained spray measurement; this repair is included in the demo.10 desktop candidates.

The pointer-following reticle, cursor lifecycle and matching paint-shot audio are saved, with creator-reported spot playtests passed. Cosmetic recoil is also saved: cache the resting local pose in Awake, apply a bounded 0.035 m backward displacement and 6-degree local pitch per accepted sample, and recover in LateUpdate with exponential smoothing at rate 18. Pause holds recovery. ResetGunPose restores the cached visual pose immediately while not playing and on disable. The creator reports recoil and settlement/retry reset passed, followed by no apparent issue in the earlier start/pause/resume spot check. These reports predate the final continuous-spray tuning and do not establish latest full-song or standalone acceptance. Reticle visibility follows active/unpaused play while the system pointer returns for menus, pause, results and focus loss. Short shot audio plays below the music only after accepted firing; bound overlapping voices. Recoil moves only an explicit weapon visual child and restores its resting pose, preserving the camera, aiming root, muzzle and spray direction. Verify held fire, pause and retry before including these in the exhibition build. See the roadmap for teaching order.

Sampled spray marks and matching pop deposits are saved on nine prepared opaque static receivers, persisting through results and clearing on retry. Automated contact/lifecycle and natural-song checks support the October 27 checkpoint; human feel/readability and exhibition-PC acceptance remain separate. No paint-count limit is planned. Entrance/hit animation remains proposed. Keep Rigidbody/collision on the root and cosmetic transforms on explicit visual children.

| Effect | Proposed implementation and initial tuning | Acceptance |
|---|---|---|
| Gentle balloon motion | Reduce initial root speed from 0.6 toward 0.25 for an aiming trial. Later give each visual child a random sine phase and about 0.03–0.05 world-unit bob amplitude at 0.5–0.7 Hz, converting to parent local units as needed. | Three targets remain easy to aim at; visual/collider alignment stays inside the measured hit margin; contacts still bounce. |
| Entrance expansion | Animate only the visible model from about 85% scale to 103%, then settle at 100% over roughly 0.2 seconds. The existing root-scaling BalloonEntrance is disabled; adapt it before reuse. | No changing Rigidbody/collider scale, abrupt overlap or drifting final scale. |
| Hit squash and burst | Lock the existing hit guard and disable colliders immediately; show a roughly 0.06-second visual squash before destroying the target. Carry the sampled hit point so the independent burst appears at impact. | Score resolves once; the squash is visible before destruction; the burst survives the target and pauses/cleans up correctly. |
| Gun recoil (saved tuning) | Apply 0.035 units backward and -6 degrees local pitch from the cached visual rest pose; recover with exponential smoothing rate 18. Reset immediately outside play/on disable. | Aim root, spray direction and logical muzzle stay stable; held fire does not accumulate displacement or shake the camera. |
| Surface paint | Saved spray/pop deposits on nine prepared opaque static receivers persist through results and clear on scene retry. A future title-return route must also clear them. No count cap or timed fading. | Correct contact, no mark colliders, readable targets and no cross-round accumulation; human/target-PC acceptance remains separate. |

Existing PaintPop variants emit at most 32 particles per burst, have short non-looping lifetimes and a two-second fallback cleanup. Retain bounded firing/refill, measure concurrent effects and only add pooling or further effects if measured object counts/frame times justify them. If behind schedule, defer entrance/hit squash, extra surfaces and additional art while protecting the playable rhythm loop.

## Optional post-festival rhythm/roguelite exploration

This is an exploratory branch, not the next committed release or the game's main positioning. Reassess whether upgrades or tougher targets add enjoyable variety without undermining the relaxing standard-song mode. If selected later, a possible slice is three complete-song stages and a small between-song upgrade pool; define and test one additional rule at a time. A larger library, elite encounters and combinations do not define the exhibition or minimum game.

Musical encounter phases may follow authored phrases within a song, but the audio and chart clock continue uninterrupted. Show one-of-three upgrade selection between complete-song stages. Do not pause a song mid-phrase to offer an upgrade menu or treat randomly inserted targets as original chart notes. The exhibition standard-song mode remains available without run upgrades.

Candidate upgrades include cadence, spread, piercing, chains, area burst, freeze, critical hits, and shields. Define caps, eligible offers, ownership, and chain termination before adding combinations. Reset run upgrades on retry; initially persist only settings and best scores. A recorded random seed may aid debugging without promising cross-version determinism.

The initial run can end after its authored stages with a final score; health, escaped-target pressure and elimination are later design proposals, not existing timed-challenge rules. Secondary projectiles/chains must not duplicate the primary event's rhythm reward. Define co-op ownership and shared/global effect budgets before applying upgrades to both players.

## Songs, results and local progression

The revised scene uses Start → Festival/Standard mode → ten-song list/preview → Play, with Back returning to title. Festival defaults to ten short windows; Standard plays the ten complete recordings. [Music and charts](MUSIC_AND_CHARTS.md#festival-and-complete-song-phrase-catalog) owns durations, provenance and chart preparation. The learner's three pattern/effect functions remain intact. Preview, drift, cleanup and capacity follow [Required rhythm loop](#required-rhythm-loop); answer previews start after the demonstration. Pending events wait for capacity, so derived data is authored below that bound. Runtime evidence and human listening/readability remain separate.

The current source extends the small result summary with successful hits, on-beat hits and maximum streak as defined in [Exhibition experience design](#exhibition-experience-design). The values use the accepted balloon impact and existing cleanup path, but saved cleanup is not yet a formal event-outcome registry and no missed-event total or accuracy exists. Unanswered-event totals support future internal correctness checks, not a failure-focused results screen. Judge firing time first but commit success only after matching impact. Accuracy, grades and more elaborate statistics require separate design/testing.

Local best score for Goin' Under / Beginner / solo / the current scoring-rules version is saved in v0.1.0-demo.2 with isolated higher/lower/retry/relaunch/display checks. Missing data defaults to zero; persist only a strict completed-round improvement and retain it on retry. Exhibition-PC acceptance remains separate. The new-record flourish above is planned; v0.1.0-demo.1 does not contain this record feature.

Persist only intended settings and records, separated by song/chart, difficulty, solo/co-op/run mode and scoring rules. Missing or damaged save data must fall back safely. Unlocks, currency and permanent upgrades are optional future decisions; they must not block replaying the small accepted content set. Commercial song clearance remains separate from availability in the authorized noncommercial development bundle.

## Co-op and localization

Current [local-player support](#local-players-and-gamepads) includes the saved learner decisions. [Controller preparation](CONTROLLER_SETUP.md) owns physical-device checks, which remain distinct from source validation. Rumble and a device-picker/join screen remain future work.

Translation data targets Simplified Chinese, Japanese and English. The festival runtime exposes all three, defaults to Japanese when no preference exists, and persists switching without duplicating screens. M+ 1c covers prepared Japanese/Latin copy and Noto Sans SC covers prepared Simplified Chinese copy. Static labels plus the runtime-written beat countdown, on-beat award and result statistics are connected to the shared table. Unity editor validation and Japanese 2K/1080p flow checks pass; three-language standalone-player acceptance remains pending. See [localization](../Art/LOCALIZATION.md).

## Planned paint and color expansion

The current production path uses eight indexed pigments, an explicit C-key switch for gun/stream/direct deposits and nine prepared opaque static receivers. Balloon pigment independently selects matching pop feedback. Broad-coverage sprites are connected; they are not general projected decals or arbitrary-mesh painting. The [concept variants V12](../Art/CONCEPT_VARIANTS_V12.md) scene remains a separate six-pigment art study. See the [palette guide](../Art/BALLOON_COLOR_LESSON.md), [paint-art guide](../Art/PAINT_SPRAY_STYLE.md) and Validation for resource/evidence details. The rules below constrain any further expansion.

- Keep a balloon's visible material, pop color and pattern coordinated as one entry; avoid independent random selections that disagree. Classic retains its existing target/point rules. The separately selected rainbow-chain rules use the existing pigment collection to bank bursts, without requiring the spray color to match a target.
- Spray samples place paint at the actual hit position and surface normal on eligible scene geometry. New props are not automatically receivers. Verify normals, depth bias and clipping before accepting another surface. The [full-scene paint architecture](FULL_SCENE_PAINT_ARCHITECTURE.md) proposes a planar paint-rendering study before broader geometry; evaluate projected decals or a separate glass treatment only for a demonstrated need. Do not recolor an entire shared material on every hit.
- A balloon pop has three coordinated visual layers: a brief independent contact-anchored pigment puff, a small radial latex-fragment scatter and matching persistent splat/brush/drip deposits on eligible nearby surfaces. Determine surface deposits through short, bounded queries; neither fragments nor deposits may float at the balloon root when the accepted contact differs. Match pigment to the balloon, keep short particle lifetimes and the one-time hit guard, and do not create extra damage or score. The on-beat layer adds a restrained gold accent rather than multiplying queries or covering the screen.
- Retain deposits without timed fading/removal throughout the song, pause and results; clear them on retry, a new round or a future return-to-title path. The October 27 automated fixture checked pause, 1,000 retained marks, natural song settlement and ten Retry resets. Saving artwork across application restarts is a separate future feature.
- The creator explicitly declined a paint-count limit. Do not add a simultaneous-mark cap or oldest-first count removal. Marks have no colliders. Expand beyond the nine prepared receivers only through an explicit eligibility/contact review. Measure standalone frame times, memory and live objects before increasing deposit density; if individual marks are too costly, test the paint-preserving architecture rather than silently deleting old work. Pooling transient bursts alone does not solve persistent-mark growth. Arbitrary curved, moving or transparent projection remains post-festival work.

Retain source artwork and attribution. New standalone studio assets and concept images belong to the art-study lane until deliberately promoted; their presence does not authorize runtime behavior or additional receiver contracts. The creator explicitly authorized the saved candidate scaffold; the three focused decisions in `KyudaisaiLearnerCore.cs` remain learner-owned. The roadmap governs promotion and release boundaries.

## Performance approach

The creator's performance target is 2560 x 1440 at 120 FPS on the exhibition PC, an 8.33 ms frame budget, subject to sustained measurement. Distinguish Editor Game-view observations from standalone-player CPU/GPU timings and include dense passages, held fire, repeated effects and frame-time spikes. The October 27 automated 2K Editor fixture measured 3.733 ms mean / 6.584 ms p95 with 1,000 retained marks on its recorded GPU; this is not a standalone FPS guarantee. Earlier measurements and their workload differences live in Validation. Higher-count trials remain experiments, not proven capacity.

Record CPU/GPU frame time, allocations, live object counts, audio voices, and particle/decal counts. Bound transient feedback first and preserve chart-driven target lifecycle without skipping events; add pooling only when a measured bottleneck justifies it. Pooling must reset scale, collider state, pop flags and ownership. Resolve simultaneous hits once and bound any future chains. Decorative strings and props should not add gameplay colliders or unnecessary realtime shadows.

## Pop light and finale presentation — demo.13

Every accepted pop creates a pigment-colored halo, expanding ring and small bright core. On-beat hits have a slightly larger, longer pulse. The finale adds twelve rainbow radial paint petals, larger existing particles, a localized closing caption and a 1.5-second afterglow; its route uses large numbered markers and a remaining-count instruction. Rainbow chain previews use thick frames, connecting lines, numbered increments and a three-pop caption when two neighbors qualify.

`PopLightFeedback` owns cosmetic rendering only: twelve reusable overlay slots, three shadow-free point lights and three generated runtime textures. No collider, input target, score, timing-window or sound is added. Ordinary pulses cannot replace an active finale. Pause hides lights/overlay and freezes their lifetime; resume continues the fade, including a brief settlement afterglow. Retry destroys the textures, sprites, canvas and light objects. No full-screen flash or camera shake is introduced. Human brightness/aim comfort remains an exhibition-device check.
