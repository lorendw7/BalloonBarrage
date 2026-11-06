# Development roadmap

Updated: November 6, 2026 (Asia/Tokyo). [Game design](GAME_DESIGN.md) owns behavior, [Validation](../Project/VALIDATION.md) owns evidence, and [README](../../README.md) is the entry point.

## Current decision

Development now focuses on the first small open-world route. Event promotion, venue scheduling and survey/QR recruitment are retired from the website. Preserve the published demo.17 studio baseline and all existing source, records, asset GUIDs, music notices and concurrent learner edits. The website update does not establish that any open-world gate has passed.

The selected direction is a central plaza with music districts, free exploration, opt-in fixed-stage rhythm challenges, a readable rival troupe and musical layers brought home. Original sunny, handmade town assets are prepared by the assistant. The learner owns future gameplay; each exercise stays small and is reviewed before the next. The previous authorization for studio implementation does not authorize writing new open-world gameplay directly.

## Immediate lesson queue

Current gate: **OW0 — route and asset preparation**, not OW1 implementation.

1. Sketch one plaza corner, one readable path and one pier stage, with a clear return route. Use the [map design](GAME_DESIGN.md#open-world-direction--planning-only); the garden and rooftops remain future branches.
2. Mark the spawn/home landmark, first sight of the stage, entry confirmation, exploration-to-stage camera handoff, exit and return. Walk through the sketch on paper and record confusing turns or occlusion. Target travel times remain design targets until measured.
3. The assistant prepares a small original asset brief: daylight palette, paper/wood/fabric materials, a plaza landmark, route sign and modular stage/cover silhouettes. Record dimensions, intended role, provenance and exact generation prompts when assets are produced. No new art is claimed by this documentation pass.
4. Once the route and camera questions are settled, explain one movement/camera exercise in a separate graybox scene. Let the learner write the gameplay, review the saved result and check navigation/return-path comfort before adding a song.

Ready for OW1 when the creator can describe the whole out-and-back trip, identify where control changes, and distinguish a navigation landmark from a rhythm target. Do not require Boss combat, learned AI, additional districts or finished art before this first walking question.

The two co-op learner decisions are already complete; [Local co-op lesson](LOCAL_COOP_LESSON.md) is a reference. Do not reassign it as unfinished work. Fix a reproduced baseline issue if it blocks development; broader calibration and hardware acceptance remain separately recorded rather than silently marked complete.

## Ownership and validation

| Work | Owner | Acceptance boundary |
|---|---|---|
| Route, camera and encounter decisions | Creator with assistant design support | A paper walkthrough and explicit open questions |
| Original models, props, textures and sound candidates | Assistant, with creator art direction | Small reviewed set, source/licensing records, preserved GUIDs/meta on import |
| Gameplay | Learner; assistant explains, reviews and verifies | One saved increment at a time; direct assistant implementation needs an explicit request |
| Tools, documentation, website and publication | Assistant | Appropriate checks and ordinary commits; static checks never imply a Unity playtest |

Use a named committed revision in a separate checkout for future release builds. Do not overwrite concurrent script/shader edits or immutable public packages. Music licensing remains unchanged.

## Preserved baseline

The published studio baseline is demo.17. The [music/studio expansion guide](../Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md) records content provenance, and [Validation](../Project/VALIDATION.md) records each immutable package. Demo.16 remains the preceding candidate and demo.6 the complete-song fallback; do not overwrite their ZIPs.

Saved source has twenty v4 profiles: ten roughly 64–65-second Festival rounds and ten full Standard recordings. The learner functions remain saved. Current constraints are a finite six-sample firing cadence, 24-target capacity, twelve-second post-note cleanup, eight pigments and 404 explicitly prepared opaque static paint receivers. One fixed atlas preserves accumulated pigment without a count limit or oldest-first removal.

Historical automated checks, creator reports, anonymous feedback about uniform pacing/offset effects, exact package hashes and remaining limits stay in [Validation](../Project/VALIDATION.md). Preserve those revision boundaries. Physical feel, venue listening, unfamiliar-player observation and real-Mac launch are separate observations and are not silently reinstated as blockers for the authorized automated pass.

## Open-world development track — planning only

No release date or calendar estimate is committed. The three districts are a long-term design envelope, not a commitment to build them together. [Game design](GAME_DESIGN.md#confirmed-detail-choices--c--a--b) records the creator-confirmed **C / A / B** gameplay direction and [A + C art direction](GAME_DESIGN.md#confirmed-art-direction--a--c): a sunny street music festival with handmade art-town details. Night festivals are deferred beyond the accepted daytime slice. The November 6 preparation update changes documentation and the website only. Scope changes and additional choices must remain explicit.

| Gate | Smallest deliverable and dependency | Evidence required before expansion |
|---|---|---|
| OW0 — decide and sketch | Gameplay C / A / B and art A + C confirmed; preserve the studio baseline, sketch plaza → first stage → return and define a daylight palette/material brief | Route, troupe roles, camera transitions and layer rewards still need design; distinguish landmark/decoration/scoring balloons by shape as well as color; choices alone do not complete a graybox or playtest |
| OW1 — walk comfortably | After OW0: separate third-person graybox with plaza edge, one path and one stage; learner writes one movement/camera increment | Observe navigation, camera collision/occlusion, motion comfort, return-path clarity and pause/focus behavior; record human observations separately from automated checks |
| OW2 — enter and leave one song | After OW1: explicitly enter a stable stage view using one existing profile; return after results or exit | Audio ownership/clock, input handoff, Retry, exit, records and target cleanup verified; free exploration never starts a chart unintentionally |
| OW3a — one troupe member | After OW2: telegraph one target, bounded aim/fire/recovery, then one complete encounter as the troupe foundation | Single target consumption, simultaneous-hit rule, line of sight and reaction delay checked; player understands the member's intent; solo records/rules preserved |
| OW3b — a coordinated pair | After OW3a: add a second member with distinguishable intent and bounded turn-taking; test one proposed interruption separately | No unreadable simultaneous threats, blocked routes, shifted chart timing or hidden scoring targets; record actual performance and player comprehension |
| OW4 — one complete visit | After OW3b: plaza → pier troupe challenge → result → first musical layer → plaza ensemble; apply A + C to this slice with one visible completion change | Observe an unaided 5–8-minute target visit; verify no duplicate reward on Retry, phrase-aligned audio and clean mode handoff; check glossy targets against matte/paper/wood scenery, bounded effects, comfort and performance |
| OW5 — expand only what works | After OW4 acceptance: one district/layer at a time; separately study short one-rule challenge variants, optional detours, rival roles and cosmetic identity | Distinct experiences, readable recovery/intent cues, audible layer combinations and verified provenance; optional pursuits stop during stage play; measure scene, paint, AI and audio costs before further expansion |

The learner owns future gameplay code. The assistant prepares a small exercise, documentation, original assets or tools as needed, then reviews and verifies the saved result. Direct gameplay implementation needs an explicit request. The selected troupe is included through the one-member and coordinated-pair gates; a larger simultaneous group is beyond the first slice. Beat Teaser, Balloon Porter and Stage Leader are candidate studies, not three required first-slice characters. Persistent world saves, procedural maps, cloud-generated dialogue/music, online multiplayer, damage systems and a seamless city remain outside the first slice. Prepare compatible, appropriately licensed music layers rather than assuming existing recordings provide remixable stems.

The night music festival is a later optional expansion, not a scheduled gate or an automatic OW5 deliverable. Revisit it only after accepting the daylight slice, with a separate lighting, target-readability, performance and music-transition review; a day/night cycle is not implied.

If walking and aiming compete for attention, keep the fixed stage camera. If rivalry undermines the relaxing experience, offer the same stage solo and reduce target contention. If the first route is confusing, shorten it before building a second district. Keep measured runtime/package evidence in [Validation](../Project/VALIDATION.md), separate from site checks and proposed acceptance criteria.

### Boss combat research track — planning only

The October 31 choice is **single-player direct combat against a Boss and its AI squad**, not a balloon scoring contest. [Game design](GAME_DESIGN.md#optional-boss-brawl--direct-combat) owns the encounter rules and [research/asset responsibilities](GAME_DESIGN.md#boss-ai-research-and-asset-responsibility). This track follows acceptance of the basic daytime town visit (OW4); it is not a dependency for the published studio baseline, a delivery date, or a promise to complete every research idea.

| Gate | Smallest deliverable | Evidence before the next step |
|---|---|---|
| B0 — combat brief and art | One paper arena and direct-combat walkthrough; assistant prepares original silhouette, material, cover and warning references | Creator/learner can explain who attacks whom, what the weak point is, how to avoid an attack and how to exit; health/defeat/camera questions explicitly recorded |
| B1 — moving combat baseline | A separate small test arena, simultaneous movement/aiming, one rule-based enemy and one telegraphed attack | Contact, finite held-fire cadence, line of sight, warning readability, evasion, camera comfort, hit/defeat feedback and retry/exit checked; no change to existing rhythm scores |
| B2 — one Boss and one minion | One Boss phase with recovery/weak-point opening; one minion and bounded coordination | No unavoidable combined attack, blocked return path or unreadable off-screen pressure; record human comprehension separately from automated checks |
| B3 — optional learned decisions | Offline cover/reposition policy, evaluated against B2 with identical constraints and a rule-based fallback | Measured comfort/fairness, generalization, stalls, frame time, memory and fallback behavior justify adoption; otherwise retain B2 |
| B4 — accepted encounter integration | Optional district arena entry/exit, explicitly designed reward and original approved assets; later template variations only if useful | Clean town/combat/audio transitions, isolated records, no duplicate rewards, preserved exploration access, provenance and runtime budget |

The learner writes gameplay one step at a time; the assistant prepares assets/tools/docs and reviews saved code. Asset preparation is assigned to the assistant. No trained model, combat script, new Unity asset import or playable Boss scene is delivered by this website/planning update. Online or local multiplayer Boss modes, additional factions and night festivals require separate scope decisions.

## Change and cut rules

- Fix reproduced timing, contact, crash, visibility or localization issues before adding ornament. Cut extra variants and milestone effects first.
- Keep unrestricted held fire, finite cadence, existing rewards/window and round-persistent paint. No timed fade, paint-count cap or oldest-first deletion.
- Preserve source recordings and original charts/notices. New profiles/derived charts use explicit identities; standard and festival records cannot mix. Retain noncommercial StepMania restrictions and BY-SA attribution/licensing for the affected derivatives.
- Do not declare a playable release from a plan, website check, asset import or camera render. Report controlled-clock and natural runtime coverage separately; exact-package startup is not full standalone-song acceptance.
- Use the [release runbook](../Project/PACKAGING_LESSON.md), preserve immutable assets and append normal commits. Push `main` without rewriting history. Never overwrite concurrent learner work.
