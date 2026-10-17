# Development roadmap

Updated: October 17, 2026 (Asia/Tokyo). This is the single delivery plan for BalloonBarrage, from the exhibition demo to a fuller game. [Game design](GAME_DESIGN.md) owns gameplay rules and tuning; [Validation](../Project/VALIDATION.md) owns evidence; [README](../../README.md) is the project entry point.

## Direction and scope

Build an approachable paint-blaster rhythm game in a warm studio: free aiming and firing remain useful, while accurate musical timing earns a bonus. Expand from one dependable solo song to shared-screen co-op, expressive surface paint, a small song library and a bounded roguelite run. Preserve readable targets and musical timing as content grows.

The exhibition demo and the later game are separate milestones. Adding an item to this roadmap does not implement it, accept it or authorize the assistant to write learner gameplay. Post-festival content counts are planning targets, subject to playtests, music rights and measured capacity. They are not public release promises.

| Horizon | Deliverable | Boundary |
|---|---|---|
| M0: exhibition | One complete song, dependable solo controls, rhythm rewards, accepted UI loop and offline Windows package | Required by the October 27 candidate target; controller/co-op work is deferred |
| M1: stable controls and co-op | Accepted one-pad play, then P1 keyboard/mouse plus P2 one gamepad | Deferred at the creator’s request; reschedule after solo polish and performance acceptance |
| M2: painting and readable variety | Projectile-created surface marks, coordinated balloon colors and patterned pops | General opaque-surface support first; cosmetic paint does not change target timing |
| M3: song and score progression | Small cleared song library, selection/difficulty flow, results breakdown and local records | Add songs through validated data and attribution, not hardcoded scene duplicates |
| M4: roguelite vertical slice | A short run with musical encounter phases, a few target types and three bounded upgrade choices | Build on accepted solo rhythm/paint foundations; co-op upgrades follow later; choices occur between songs |
| M5: compact first release | Integrated modes, settings/accessibility, localization, performance and distribution quality | Ship a small accepted feature set before expanding content |

No post-festival calendar date is committed yet. Review one milestone after its predecessor passes and schedule the next one from actual progress. Online play, independent dual mice, split-screen, mobile, user accounts and an online leaderboard are outside this roadmap's initial release.

## Current checkpoint

This publication includes all saved gameplay, scenes, art, tools, documentation and website progress authorized by the creator. A committed source checkpoint is separate from a tested Windows package. Read the validation record before treating an earlier automated check as acceptance of newer code.

| Area | Evidence available | Remaining gate |
|---|---|---|
| Title, pause, HUD, results and retry | Saved; isolated natural-song/results/retry checks passed; creator reports Windows start/shoot/pause/results/retry/Quit passed October 17 | Measure audible alignment, fresh extraction/offline behavior and repeated play |
| Song switch and complete-song clock | Saved; an earlier isolated natural-song/pause/retry run passed | Audible alignment and latest chart/reward integration on the exhibition PC |
| Chart previews, event identity and unanswered-target cleanup | Saved; source review and creator-reported cleanup pass | Dense/unanswered passages, final event and pause near scheduled events |
| Ordinary/on-beat rewards and countdown | Saved; source review and creator-reported 10/20-point and countdown passes | Different distances/frame rates, held fire versus taps, duplicate-resolution checks |
| Paint burst, golden glow and temporary reward text | Saved; creator-reported display/hide passes; isolated asset/layout/reference checks | Confirm complete-song behavior, pause, repeated hits and retry with the selected 1.45-second text setting |
| Reticle, firing sound and visual recoil | Creator reports reticle/audio/recoil/reset and Windows core input flow passed; isolated full-song/last-hit/retry check passed | Dense passages and target-PC performance |
| Xbox/Beitong and co-op | Input actions prepared; Beitong hardware preparation reported successful; deferred October 17 | Reschedule after solo work; no current hardware-check or exhibition deadline |
| Surface paint, patterned pops, song selection, runs and upgrades | Planned; cosmetic variants exist in separate art scenes | Separate gameplay milestones below |
| Windows performance and current demo package | Public Windows solo prerelease v0.1.0-demo.1 preserves the tested build; file/report/chart/archive checks passed; creator reports the standalone core flow passed. Editor batching evidence remains separate | Sustained frame times, ten retries, fresh-extraction/offline checks, soak and final non-development candidate |

Do not assign a percentage or fixed remaining lesson count to this checkpoint. Connected features, creator playtests, automated checks and release acceptance are different forms of evidence.

## Exhibition calendar and decisions

The [official 79th Kyudaisai site](https://kyudaisai.jp/79th/), checked October 16, lists October 31–November 1, 2026. The creator plans to exhibit on October 31; booth and demo hours remain unconfirmed. As of October 17 there are 14 calendar days to opening and 10 to the October 27 internal candidate target.

| Target | Learner's core work | Assistant's parallel preparation | Exit criterion |
|---|---|---|---|
| October 16–19 | Reticle, short firing feedback, then complete-song rhythm regression | Reticle artwork, original synthesized firing sound, hierarchy/reference checks, Japanese instructions and test preparation | No blocked UI, drifting weapon, duplicate bonus or timing regression; finish the natural song |
| October 20 | Resolve solo rhythm blockers and measured timing issues | Build/package the accepted checkpoint, verify bundled audio/chart and capture first performance baseline | Solo title → complete song → results → retry works offline in a fresh Windows build |
| October 21–23 | Finish the small solo results increment if stable; address measured solo issues | Japanese instructions/fonts, new-player readability, standalone performance capture and regression checks | Clear solo instructions; best score persists if included; core song loop remains accepted |
| October 24–26 | Measured fixes and new-player playtests | Profiling setup, repeatable stress checks, cleanup/reference checks and package preparation | Target-PC performance, ten retries and a 30-minute soak accepted |
| October 27 | Confirm gameplay and known issues | Versioned ZIP, complete runtime files, SHA-256, controls, notices and verified download | A reproducible candidate that launches offline |
| October 28–30 | Exhibition-PC rehearsal and blocker fixes | Backup ZIP, recovery instructions and accurate website/download information | Fresh extraction, sound, display, devices, quit and fallback all verified |
| October 31–November 1 | Use the verified build and observe players | Maintain release information and consolidate feedback | Record observed problems without introducing untested booth-time changes |

The creator deferred controller and local co-op work on October 17. The previous October 23 co-op decision gate is retired; the exhibition plan now prioritizes keyboard/mouse solo. Reschedule device preparation and M1 only after solo polish and performance acceptance, with no new date promised. If a small increment causes regression, return to the accepted solo checkpoint. Preserve the performance/package window and defer additional modes or major environment changes.

## Immediate lesson queue

Teach each row through small edits, normally one field/method or one hookup at a time, then review the saved checkpoint. A row may need several exchanges; a snippet is not a finished feature.

| Order | Increment | Acceptance before moving on |
|---|---|---|
| 1 | Local solo best score — next learner lesson, planned | Current song/chart/solo/rules-specific record defaults to zero; a higher completed-round score persists, a lower score cannot overwrite it; retry resets only the round score; confirm quit/relaunch persistence |
| 2 | Exhibition instructions and readability — assistant preparation alongside lesson 1 | Short Japanese instructions and suitable font coverage; a new player understands aim, fire, rhythm bonus, pause and retry; readable title/HUD/results |
| 3 | Measured smoothness and timing | Capture standalone CPU/GPU frame times and signed audible/input error; teach one supported optimization at a time and compare again; 2K/120 FPS remains a target |
| 4 | Repeated-play and package regression | Ten retries and a 30-minute soak; counts/memory remain bounded; fresh extraction and offline start/audio/results/retry/Quit pass on the exhibition PC |
| 5 | Exhibition candidate and rehearsal | Intended non-development build, versioned ZIP/hash/notices, verified website download and offline backup; target October 27, rehearse October 28–30 |

Start the best-score lesson with one persistent key and a zero-default read, then teach the higher-score comparison at settlement, persistence and a separate results label through small changes. The assistant prepares the label/layout and checks; the learner writes gameplay. This is planned work, not a shipped feature or permission for the assistant to implement it. It is a small optional M3 slice: defer it if it threatens core-flow or performance acceptance. Do not add combo/accuracy statistics or additional songs to this increment.

The latest isolated natural-song check covered all 93 events, last-event reward, settlement, retry and the second round's first hit. On October 17 the creator reported Windows start, shooting, pause/resume, whole-song results, retry and Quit passed. Public prerelease v0.1.0-demo.1 preserves that tested runtime; see [Validation](../Project/VALIDATION.md#public-windows-demo-v010-demo1). It does not yet include best-score saving, Japanese UI or controller gameplay. A public development prerelease does not accept the final exhibition candidate.

## Workstreams, ownership and dependencies

| Workstream | Learner owns | Assistant prepares and maintains | Dependency / acceptance |
|---|---|---|---|
| Rhythm and scoring | Scheduling, target lifecycle, firing judgement, collision resolution, later combo/stat rules | Prepared chart validation, focused regression checks, review and documentation | Shared song clock and once-only event resolution |
| Aim and input | Reticle movement, fire/release gates, player/device pairing | Input actions, reticle art, menu glyphs and setup checks | Solo accepted before P2; logical aim independent of cosmetic recoil |
| Weapon feel and effects | Accepted-shot triggers, visual recoil, impact behavior | Licensed/generated assets, audio editing, material/prefab setup and bounded particle assets | No phantom feedback on rejected input; preserve aim and musical cues |
| Painting and target variety | Eligible hit placement, fade/budget lifecycle, target rules | Decal/splat materials, patterns, palette entries and receiver checks | Correct impact position/normal, bounded counts and readable targets |
| Content and progression | Selection flow, run state, upgrades and local records | Chart conversion checks, attribution, data/configuration scaffolding and UI assets | Rights and timing subset verified before a song is offered |
| UI and localization | State transitions and data bindings | Layout, fonts, translations, resolution checks and scene-reference repair | UI never fires into the game; no duplicate clocks or scores |
| Performance and shipping | Fixes that touch gameplay; physical playtests | Profiling guidance, build tools, meaningful automated checks, packaging and website | Measured improvement on the target PC, complete offline package |

The learner retains Assets/Scripts gameplay. Preparing a helper, prefab, test or settings asset does not silently complete its gameplay integration. The creator uses the real mouse, Xbox/Beitong, audio output and exhibition PC for physical acceptance; synthetic tests support but do not replace it. Existing Unity GUIDs, paired metadata, original song files/notices and recorded image prompts remain intact.

Dependency chain: accepted song clock → event lifecycle and once-only rewards → solo controls/feedback → complete-song Windows baseline → solo results/readability/performance → bounded impact paint and content progression → run/upgrades → integrated release checks. Deferred co-op is a later branch, not a prerequisite for solo improvements. Asset preparation, translation and test tools can proceed alongside the relevant learner step without adding unrequested gameplay.

## M0: exhibition acceptance

Required scope is one complete prepared song, solo keyboard/mouse shooting, chart previews, target expiry, visible beat/reward feedback, firing-time bonus resolved once on the matching impact, accepted title/instructions/HUD/pause/results/retry/quit, Japanese readability and an offline Windows package. New painting systems, extra songs, formal combo statistics, upgrades, language switching and broad environment revisions are not M0 requirements.

| Gate | Required evidence |
|---|---|
| Musical round | Play the complete unshortened song, including intro, final event and ending; listen for cue/judgement alignment |
| Input and scoring | Ordinary and on-beat hits, misses/mismatched targets, held input after UI/resume, duplicate contacts, different distances and focus changes |
| State reset | Ten consecutive retries with fresh score, chart index, song origin, targets, projectiles, feedback, cursor and weapon pose |
| Readability | Unfamiliar player understands the short instructions; Japanese fonts and HUD remain legible at the selected window size |
| Performance | Target 2560 x 1440/120 FPS on the exhibition PC (8.33 ms budget); record standalone CPU/GPU frame times, hitches, allocations, live objects, particles/paint and audio voices |
| Stability | 30-minute repeated-play soak including dense unanswered passages; counts and memory do not grow without bound |
| Package | Fresh extraction on the target PC, full runtime dependencies, notices, SHA-256 and offline start/audio/retry/quit |
| Public status | Real package/version link verified; distinguish local progress, creator reports, automated checks and shipped features |

Use a Development build to investigate a measured problem, then confirm the intended player package as well. Resolution layout checks are not frame-rate measurements. Pool targets/projectiles/effects only when profiling identifies repeated creation/destruction as a bottleneck; reset event id, scoring guard, collider, pigment, scale, audio and ownership when reusing an object.

## M1: dependable input and shared-screen co-op

Deferred at the creator’s request on October 17. Remove this work from the immediate lesson queue and exhibition calendar; resume only when rescheduled after solo polish/performance. Preserve the accepted solo build. Use [Controller preparation](CONTROLLER_SETUP.md) rather than duplicating device setup here.

| Increment | Deliverable / exit criterion |
|---|---|
| One pad, one player | Explicitly paired pad, bounded right-stick reticle, useful sensitivity/deadzone, RT fire and release-after-menu; verify Xbox and Beitong |
| Player state | Independent reticle, cadence, release guard and projectile player identity; one shared camera, music source, round controller and event timeline |
| Shared outcomes | Team score first; existing balloon hit guard awards once even for simultaneous contacts; one shared target cannot be consumed twice |
| Menus and recovery | Designated menu owner; pause on paired-pad disconnect; reconnect without ghost shots or return safely to solo; spare pad is ignored |
| Presentation | Distinct reticle shape plus P1/P2 label, coordinated gun/paint colors; no reliance on color alone |
| Acceptance | Complete a natural song with both devices, pauses, reconnect, settlement and repeated retries on the actual Windows PC |

Individual contribution statistics can follow shared-score acceptance. Two-gamepad play and rumble remain later extensions. Co-op upgrades must preserve explicit player ownership and shared/global effect budgets.

## M2: paint the studio and add visual variety

Cosmetic balloon/gun palette work exists in separate art variants; it does not implement projectile-created paint on scene surfaces. Follow the behavior/budgets in [Planned paint and color expansion](GAME_DESIGN.md#planned-paint-and-color-expansion).

| Increment | Deliverable / exit criterion |
|---|---|
| First wall mark | Actual projectile impact position/normal, small depth offset, no mark collider; no mark from a UI click or a flying projectile |
| Eligible scene surfaces | Walls, floor and opaque furniture; verify sloped/curved/thin geometry and material receivers; glass/transparent receivers need a separately scoped fallback |
| Coordinated patterns | One palette entry selects balloon appearance, pigment and burst pattern; projected marks contact eligible surfaces rather than floating in space |
| Lifecycle and readability | Round-owned global paint budget, oldest-first removal, fade and retry clear; both players share the budget; targets remain readable |
| Presentation polish | Add visual-child entrance/bob/squash only with collider alignment and bounded cleanup; compare repeated/dense effects before increasing counts |
| Acceptance | Dense passages and two-player fire, then pause/retry/soak; correct placement, stable counts and measured render cost |

Preserve the cozy room's composition. More paint must not turn the center into an opaque overlay, change physics layers accidentally or recolor entire shared materials on impact.

## M3: music, results and local progression

Planning target: a small selection of two or three rights-checked songs and a gentle/standard difficulty path, not an unrestricted user importer. Reuse the current approved track for early workflow tests; each additional song/chart needs its own provenance and usage decision. A downloaded file is not automatic commercial clearance.

| Increment | Deliverable / exit criterion |
|---|---|
| Track catalog | Stable song/chart ids, title/artist/author, duration, difficulty and rights/provenance; one selected track loads its matching audio/chart |
| Song selection | Selection → controls/ready → selected full song → results → replay or selection; switching restores cursor, audio and chart state |
| Difficulty | Distinct validated chart data/preview/motion tuning; do not silently change the music speed or scoring window to fake difficulty |
| Event statistics | Explicit once-only ordinary/on-beat/expired outcomes; distinguish missed shots from unanswered chart events and scored impacts |
| Results | Score and an understandable timing/hit breakdown; combo/accuracy definitions approved before display; proposed thresholds tuned by listening/playtests |
| Local records | Best scores keyed by song, chart, mode, difficulty and rules version; settings persist, current round/run state resets; corrupt/missing data has a safe default |
| Tools | Chart preview/validation, clear rejection of unsupported timing/note types, source hashes/notices and regression fixtures |

Do not compare solo, co-op and upgraded-run scores as if their rules were identical. Calibration settings must document sign and audio/output context. Preserve the existing narrow supported chart subset until tempo changes/holds/stops are explicitly designed and tested.

## M4: a small rhythm/roguelite run

This is a post-festival design proposal. Build one playable run before adding many target types or upgrade combinations. See [Post-festival rhythm/roguelite prototype](GAME_DESIGN.md#post-festival-rhythmroguelite-prototype) for the mode boundary.

| Increment | Planning target / acceptance |
|---|---|
| Run skeleton | About three complete-song stages, initially allowed to reuse one approved song; title → stage → results/choice → next stage → run results/retry |
| Musical encounters | Wave/encounter changes follow authored song phrases; chart timing and uninterrupted song playback remain authoritative |
| Upgrade choice | A small eligible pool, initially three useful choices; select between songs, with maximum levels, exclusions and ownership shown clearly |
| First target additions | Add one rule at a time, such as an armored target, then a bounded splitter/chain variant; explain its visual cue and how rhythm interacts with it |
| Combination rules | Limit projectile/chain counts, terminate recursive effects, resolve damage/score once and prevent secondary effects from inventing duplicate rhythm bonuses |
| Run lifecycle | Retry removes run upgrades, temporary objects and player ownership state; retain only intended settings/records |
| Balance | New players can finish a short run, identify upgrades' effects and understand failure/success without dense instructions |

Do not interrupt a song for an upgrade menu or add random off-chart targets while claiming they are authored rhythm notes. A later dedicated free-paint/practice mode can explore relaxed decoration separately from scored song challenges.

## M5: integrated first release

Select the stable subset of M1–M4 rather than requiring every proposed extension. A compact release target is solo plus accepted local co-op, a small song selection, coordinated bounded paint, useful settings/results and one short optional run mode. Cut an unstable optional mode before weakening core rhythm or offline play.

| Area | Release work |
|---|---|
| Settings and accessibility | Independent music/effect volume, aim sensitivity/deadzone, resolution/window mode and measured quality presets; readable contrast, shape/labels and reduced-flash/recoil options |
| Localization and tutorial | Japanese first for exhibition, then complete Chinese/Japanese/English strings and glyph coverage; short practice/onboarding that demonstrates free fire versus rhythm bonus |
| Content finish | Consistent balloon/gun/pattern palettes, restrained scene lighting, readable HUD and transitions, clean audio mix |
| Robustness | Missing/invalid chart handling, unavailable audio/device recovery, save defaults/migration, focus/pause behavior and clean quit |
| Performance | Compare CPU/GPU and allocation captures on supported hardware, verify effect/paint/audio budgets and recheck dense sections after each content increase |
| Distribution | Versioned Windows package, control/device matrix, system notes based on tests, license/credit inventory, hashes and rollback/backup |
| Public communication | Website/screenshots match the shipped revision; real download states; known issues and patch notes tied to version |

No online services, analytics, monetization or broad platform port is a dependency for this release.

## Risks, cut lines and decisions

| Risk or unresolved decision | Response / cut line |
|---|---|
| Timing feels inconsistent despite numerical judgement | Measure physical audio/input latency, compare taps and held fire, inspect one DSP origin and target-distance independence before tuning windows |
| Extra feedback makes the song harder to read/hear | Lower shot audio and effect density, reduce recoil/flash and keep cosmetic transforms separate; return to accepted feedback if necessary |
| Co-op disrupts solo input or duplicate-hit protection | Preserve a solo entry/fallback, explicit device pairing and one consumed-target outcome; keep M1 deferred and protect the accepted solo path |
| Paint obscures targets or costs too much | Start on one wall, use a shared budget/fade, reject unsuitable receivers and profile before expanding |
| Song rights or timing format are unsuitable | Keep the accepted song and notices; replace/defer the content, never imply commercial clearance |
| Too many upgrades or target behaviors | Finish one short run and a tiny eligible pool first; add combinations only after bounded-effects and scoring tests |
| Standalone performance unverified | Treat 2K/120 as a target, not a claim; compare against the initial 36.7 FPS Editor observation, retain profiling/soak time and discuss any necessary quality tradeoff from measured data |
| Learning overload | One small core change per handoff, one next action, explain code and keep a runnable checkpoint; prepare assets/docs/tests in parallel |
| Exact combo, accuracy, difficulty and progression rules | Decide during M3 from observed play; keep proposals out of advertised current features |
| Which post-festival feature comes first | Finish solo polish/performance first; M2/M3 can proceed while M1 is deferred. Reschedule co-op from creator priorities and actual capacity |

## Working agreement and evidence

Each increment has one owner, an asset/configuration handoff, a concrete playtest and an evidence entry. Record saved implementation, creator report, automated result and unresolved checks separately. Keep screenshots/audio samples from the revision actually tested; an old camera render or website test cannot accept gameplay.

Keep README as entry point, game design as behavior source and this roadmap as delivery plan. Detailed import rights live in [Music and charts](MUSIC_AND_CHARTS.md), controller steps in the controller guide, art provenance in [Credits](../../CREDITS.md), and build/download procedures in [Website and releases](../Project/WEBSITE_AND_RELEASES.md). Update those sources rather than copying their full instructions here. Publish ordinary commits without rewriting history, and inspect concurrent learner work before every edit.
