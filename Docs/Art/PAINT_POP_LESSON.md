# Lesson: paint bursts when balloons pop

This lesson connects a short paint burst using the effect hook the learner already wrote. The saved teal connection is now verified; manual acceptance remains below. Delivery dates and later feature scope live in the [roadmap](../Design/ROADMAP.md).

## Current implementation and prepared assets

`Assets/Scripts/Balloon.cs` already instantiates `popEffectPrefab`, plays it, and schedules its independent GameObject for destruction after two scaled seconds. The `isPoping` guard prevents repeated scoring/effects. The saved `Assets/Prefabs/PlayableBalloon.prefab` now points at `PaintPop_Teal`, which produces latex fragments and paint drops. Its Pop Point is unassigned, so the code uses the balloon root position.

New assets are prepared in `Assets/StudioArtPack/ExhibitionV8/Prefabs`:

| Prefab | Appearance |
|---|---|
| PaintPop_Teal | Teal fragments and paint drops; matches the current balloon |
| PaintPop_Coral | Coral variant for a future matching target |
| PaintPop_Gold | Gold variant for a future matching target |

Each prefab has one root latex ParticleSystem and one PaintDroplets child. A single burst emits ten scraps and twenty-two drops. Both systems are non-looping and use scaled time. Drops shrink away within 0.58 seconds; scraps use the existing 0.38–0.68-second lifetime. The root retains Stop Action Destroy; the existing two-second cleanup remains a fallback. There are no colliders, particle collision callbacks, runtime scripts or extra lights on these effect assets.

The latex atlas/material are reused from [BalloonPopV2](BALLOON_POP_V2.md), with their original provenance retained. Drops use Unity's sphere mesh and a particle material; no new generated image is involved. `Previews/PaintPop_Peak_v8.png` shows three manually simulated bursts through the gameplay camera. It is an art preview, not three recorded hits.

The earlier isolated art test temporarily assigned all three variants to test instances. A subsequent physics acceptance test used the saved production prefab without replacing its effect reference and verified the teal burst, one score award for repeated Hit calls, and cleanup. See the [validation record](../Project/VALIDATION.md). These method-driven tests do not replace the following manual playtest.

## Connect one effect (about 15–25 minutes)

1. Stop Play Mode and save your current work.
2. Open PrototypeScene. Select the object with BalloonSpawner. Inspect its **Balloon Prefab** reference; it currently points to `Assets/Prefabs/PlayableBalloon.prefab`.
3. Open that prefab in Prefab Mode. Select the root containing the **Balloon** component.
4. Find **Pop Effect Prefab**. Confirm it contains `Assets/StudioArtPack/ExhibitionV8/Prefabs/PaintPop_Teal.prefab`. If it is missing or you are repeating this lesson on another prefab, drag that asset into the field. Unity uses its root ParticleSystem.
5. Leave **Pop Point** as it is for the first test. A missing Pop Point is supported by the existing position fallback. If the burst looks offset, create a PopPoint child at the visible balloon center and assign it in a separate small adjustment.
6. Save the prefab and return to PrototypeScene. Do not add a second Instantiate call and do not place a permanent PaintPop instance in the scene.
7. Enter Play Mode, click Start and shoot a balloon.

Expected: one sound, one ten-point award, latex scraps and teal paint droplets at the pop position, then a clean disappearance. The effect remains visible briefly after the balloon is destroyed because it was instantiated independently.

## Understand the existing code

The current flow is:

`Hit → duplicate-hit guard → Pop → disable colliders → instantiate effect → play → timed cleanup → destroy balloon`.

`effect.Play()` plays child particle systems by default, so the PaintDroplets child is triggered without adding a second gameplay effect field. Do not parent the spawned effect under the balloon: destroying the balloon would also remove the burst. Do not enable Looping to make the burst more visible; adjust burst size/count modestly instead.

This lesson is Inspector wiring and acceptance, because the underlying code is already written. Keep `Assets/Scripts` unchanged unless a reproducible issue requires a learner-authored fix.

## Acceptance before moving on

- Pop one balloon: score increases exactly once and exactly one root PaintPop instance appears.
- Use held fire against the same target: no repeated score or duplicate burst from that target.
- Pause immediately after a hit: the paint freezes with the round. Resume: it finishes.
- Inspect the Hierarchy after two seconds of unpaused time: the effect root and child are gone.
- Play ten pops and restart: no steadily growing collection of PaintPop objects.
- Check at 720p and 1080p. Drops should read as paint without covering the next target.

If nothing appears, inspect the prefab actually referenced by BalloonSpawner, the Balloon component's effect assignment, root ParticleSystem and Console. Editing an inactive scene template or a different color variant will not change what the spawner instantiates. If effects vanish too early, check that they were not parented to the balloon. If they persist while paused, that is expected scaled-time behavior; count cleanup time after resuming.

## Next paint lesson: projectile hits leave surface marks

Flying particles and persistent wall marks are separate systems. These particles do not paint walls automatically.

The requested next paint increment will reuse one existing [spray prefab](SPRAY_ASSETS.md) when a projectile hits an eligible wall or floor. Use the projectile's existing hit point and surface normal to place a small decorative mark just clear of that surface, then register it with a round-owned collection. Balloon hits still trigger the burst above. Start with a maximum of 40 visible surface marks (remove the oldest), and clear all marks on restart. Keep marks low contrast and without colliders. Surface marks are planned learner work; the current PaintProjectile only pops balloons and destroys itself on a hit. Do not add render-texture painting or particle collision painting before this bounded version passes.

After that, continue the [Canvas integration lessons](INTERFACE_V2.md), Japanese readability and a fresh Windows build. Do not wait for rhythm judgement or roguelite upgrades to validate this exhibition loop.
