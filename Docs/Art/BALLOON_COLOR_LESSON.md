# Lesson: three balloon colors with matching bursts

Updated: October 17, 2026. The creator reported MainWall paint and eight-second cleanup passed, then selected balloon colors as the next small increment. [Roadmap](../Design/ROADMAP.md) owns the queue; the [surface-paint lesson](SURFACE_PAINT_LESSON.md) retains the paint lifetime and optional later fade/round-cleanup work.

## Prepared prefab

`Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab` is prepared from the current `Assets/Prefabs/PlayableBalloon.prefab`, not the older V12 multi-shape prefab. All existing source components and their settings are preserved. It adds the existing cosmetic `StudioBalloonAppearance` component with:

- The current body MeshFilter and MeshRenderer references.
- One silhouette: the current original mesh, preserving collider alignment.
- Three opaque latex materials: current SurfacesV6 TealLatex, BalloonColors CoralLatex and GoldLatex.

The source prefab and production scene are not automatically reassigned. No learner gameplay code, model, generated image or external asset is added by this preparation. Existing original-model attribution and material provenance remain applicable.

## Current hookup step

1. Stop Play Mode and open PrototypeScene.
2. In Hierarchy, select **GameManager**. Find its **Balloon Spawner (Script)** component.
3. Drag `PlayableBalloon_ThreeColors.prefab` from Project into **Balloon Prefab**. Save the scene. Other spawner fields retain their existing settings.
4. Start and play long enough to see several new balloons. Colors are selected independently; consecutive balloons may share a color. Hit each color and verify its paint burst matches. Ordinary hits should remain 10 points and matching on-beat hits 20, with the separate golden glow retained.

Current source/reference review confirms the production spawner uses Balloon Prefab GUID `ee2d7674776bb93438672c562a35bc5b`. The creator reports all three balloon colors, matching bursts and ordinary 10/on-beat 20-point playtests passed. The next core lesson is [Local best score](../Design/BEST_SCORE_LESSON.md).

## Why it works

`StudioBalloonAppearance.Awake()` selects a pigment once when an instance initializes. It uses its own System.Random instance, independent of the existing Unity spawn/movement random stream. A single silhouette entry keeps the shape fixed. `ApplyVariant` assigns the chosen shared material to that instance's renderer rather than editing the material asset.

Balloon.Pop already calls `GetComponent<StudioBalloonAppearance>()?.ColorizeBurst(effect.gameObject)`. The new component makes that existing optional hook active; StudioPaintTint carries its pigment into the independent burst while retaining alpha/timing/emission settings. The separately instantiated on-beat glow keeps its own color.

No per-frame color cycling is introduced. A balloon keeps the color selected at spawn. More palettes, changing gun/surface-paint color and patterned bursts are separate increments.

## Verification and acceptance limits

Unity 6000.0.77f1 compiled the asset-preparation tool in an ignored isolated project. Preparation compared every existing component's serialized state before adding appearance selection. The focused method-driven check passed all three body-material/burst-pigment matches, 24 valid calls to the existing appearance initialization, unchanged body mesh and unchanged Collider/Rigidbody settings. It confirmed current base points 10, expiry delay 3.1 seconds, judgement window 0.1 seconds, paint burst and golden-glow references. The original source prefab, production scene and learner source hashes were preserved.

This is asset/configuration verification, not a physical full-song color/scoring or performance playtest. The hookup and listed color/scoring behavior now have creator feedback and a saved reference check; no new independent full-song or performance acceptance is inferred. The public v0.1.0-demo.1 ZIP is unchanged. Logs remain ignored under `.local-backups/balloon-color-20261017`.
