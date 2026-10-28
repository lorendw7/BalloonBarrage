# Balloon and projectile palette

The legacy `Tools/Balloon Studio/Prepare Three-Color Balloon Lesson` command now creates a separate `Assets/StudioArtPack/BalloonColors/Prefabs/LessonBalloon_ThreeColors.prefab`. It preserves an existing lesson asset on repeated invocation and never writes the production prefab below or changes scene references. Its isolated verification compares cleanup against the source lesson prefab instead of hard-coding historical timing. Use the production prefab for the current eight-pigment game; the generated three-color asset is a teaching example.

Updated: October 19, 2026. The creator explicitly authorized adding multiple colors to balloons and projectiles. [Game design](../Design/GAME_DESIGN.md) owns behavior and [roadmap](../Design/ROADMAP.md) owns delivery.

## Saved configuration

The production spawner still references `Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab` (GUID `ee2d7674776bb93438672c562a35bc5b`). Its legacy filename is retained to preserve references. Its palette now contains eight entries in this exact order:

| Index | Pigment | Material |
|---|---|---|
| 0 | Teal | SurfacesV6/Materials/TealLatex |
| 1 | Coral | BalloonColors/CoralLatex |
| 2 | Gold | BalloonColors/GoldLatex |
| 3 | Sky blue | BalloonColors/SkyBlueLatex |
| 4 | Violet | BalloonColors/VioletLatex |
| 5 | Rose | BalloonColors/RoseLatex |
| 6 | Lime | BalloonColors/LimeLatex |
| 7 | Tangerine | BalloonColors/TangerineLatex |

`StudioBalloonAppearance` selects one material at initialization and keeps it for that balloon's lifetime. Its single existing silhouette, collider setup and Rigidbody settings are unchanged. The existing burst hook uses the selected pigment. `Balloon.wallPaintPrefabs` has eight matching `SurfacePaint_<Pigment>` entries in the same order.

The production `PaintDrop3D_Teal.prefab` retains its legacy name and GUID. In the learner's current rewrite, `PaintProjectile.Awake()` selects one random palette index from a separate System.Random stream, reads the material's `_BaseColor` into a Color variable and applies it through `StudioPaintTint` to the flying projectile. Repeated colors are allowed. The learner now reads `paintPatternGroups[index].prefabs`, draws a separate group-local pattern index and assigns that selected prefab to paintMarkPrefab. The impact branch keeps that pattern and pigment chosen at initialization. The eight production pigment/mark slots are populated. Defensive array/null guards are not yet restored in this incremental rewrite, so acceptance is limited to that configured production prefab. The production shooter has no gun-theme component overriding its tint.

Balloon hits use the balloon's pigment for its explosion/deposits. Direct surface impacts now use the projectile's pigment again, through the learner-written same-index assignment. This adds no color-matching scoring rule. Timing, speed, collision queries, ordinary/on-beat points and persistent-mark lifetime are unchanged. The four receivers remain MainWall, Floor, LeftWall and RightWall; retry clears marks.

## Art provenance

The five new opaque latex materials copy existing CoralLatex settings and change only their name and pigment. Their sRGB inputs are SkyBlue `#489DEC`, Violet `#9A6DE0`, Rose `#EB71A6`, Lime `#91C94F` and Tangerine `#EF903C`.

Surface marks now reuse six existing atlas slices with the original opacity, scale, Ignore Raycast layer and collider-free setup. `PalettePaint.shader` preserves sprite alpha and takes its brightest RGB channel as monochrome shading before applying the pigment. This avoids multiplying new colors by the original source hue. Each pigment has one shared recolor material; no per-shot material copies or texture downloads are introduced. Original model/atlas attribution and exact image-generation prompts remain applicable and unchanged; see [spray assets](SPRAY_ASSETS.md#recorded-generation-prompts).

## Six pattern families, prepared for learner integration

`Assets/StudioArtPack/SurfacePaint/Patterns` contains 48 prefabs: eight pigments times six styles. `PatternManifest.json` lists every path/GUID in color-major order. Within each color, the style order is Spray, Burst, Drip, Swoosh, Brush, Starburst. These are art resources, not a runtime selector.

| Style | Existing source slice | Character |
|---|---|---|
| Spray | Teal_Spray | Soft speckles and an uneven spray edge |
| Burst | Coral_Burst | Rounded radial splash and detached drops |
| Drip | Yellow_Drip | Downward-running paint |
| Swoosh | Indigo_Swoosh | Sweeping curved brush mark |
| Brush | Pink_Brush | Broad dry-brush stroke |
| Starburst | Turquoise_Burst | Pointed irregular splatter |

The eight already-connected default mark GUIDs are preserved. Their current silhouettes are Teal/Spray, Coral/Burst, Gold/Drip, SkyBlue/Swoosh, Violet/Starburst, Rose/Brush, Lime/Burst and Tangerine/Drip. This immediately varies the silhouettes across randomly selected colors without implementing new gameplay. The first three use new shared pattern materials; the original materials remain available.

Per-projectile independent style selection and random instance rotation/scale are now learner-written and saved. The accepted surface-impact branch rotates the new mark around its local Z axis after aligning it to the contact normal. The saved paintRotationRange field defaults to 20 degrees, and the saved formula maps a random sample to approximately minus that range through plus that range; the Inspector value controls the actual range. Uniform instance scale remains 0.8–1.2 times the prefab scale. The field and revised angle formula have passed source review; the previous full-circle runtime acceptance below does not verify this latest revision. Preview-only angle/size variation does not establish runtime acceptance. The projectile/balloon Inspector palette lengths are unchanged by this asset preparation.

## Verification

Before the pattern-art follow-up, Unity 6000.0.77f1 compiled the saved color expansion. Isolated Editor Play Mode checks passed eight pigments on each of four production receivers for both balloon-pop deposits and actual SphereCast projectile collisions (32 combinations per path). Projectile checks sampled 256 random spawns, reached all eight pigments and compared each renderer tint with its selected mark. Checks retained ordinary/on-beat 10/20 scoring, rejected ineligible surfaces, preserved marks beyond eight seconds through pause/results and cleared them on retry. The native Unity preview rendered all eight balloon materials, projectile tints and mark shaders after allowing material buffers to update. Logs, probes and the preview remain ignored under `.local-backups/palette-20261019`. Public v0.1.0-demo.4 contains the saved eight-color expansion. Human readability, full-song load and exhibition-machine acceptance remain separate.

The later SurfacePatternAcceptance asset-only batch check passed all 48 prefabs: six unique sprites, consistent ordering across eight pigments, matching material pigment, alpha 0.9, scale 0.18, Ignore Raycast, no colliders/runtime scripts and shader compilation. The native preview was visually inspected: [pattern sheet](../../Assets/Art/Previews/SurfacePatterns_20261019.png). All Assets/Scripts source hashes stayed unchanged. This is not per-impact randomization or a new standalone collision/readability acceptance. Logs are ignored under `.local-backups/patterns-20261019`.

## How the projectile selects its color

The earlier full-circle rotation/scale increment compiled and passed isolated Unity 6000.0.77f1 Editor Play Mode checks on October 19, before the configurable-angle revision. The existing fixture sampled 1,024 projectile spawns and exercised all 48 patterns on four production receivers (192 actual SphereCast impacts). Added assertions checked surface-normal alignment, instance scale within 0.8–1.2 of the source prefab, preserved proportions, and variation across impacts. Existing pigment, receiver rejection, 10/20 scoring, projectile expiry, mark persistence and retry checks also passed. All learner gameplay source hashes remained unchanged during review. Evidence is ignored under `.local-backups/paint-variation-review-20261019/unity.log` and `report.txt`. These checks do not establish physical-input, drip readability, full-song or standalone acceptance.

The learner's saved group-local selection passed isolated Unity 6000.0.77f1 Editor Play Mode acceptance. Sampling 1,024 actual projectile instances reached all 48 color/pattern combinations; both renderer tints matched the selected mark pigment. Actual SphereCast fixtures passed all 48 patterns on MainWall, Floor, LeftWall and RightWall (192 combinations), with one independent mark per impact, correct sprite/material, surface position/normal, receiver rejection and no surface-hit score. Ordinary/event-matched on-beat scoring, projectile expiry, persistent marks through pause/results and retry cleanup passed. Fixture patterns were selected by repeated calls to the saved Awake and chart spawning was disabled to avoid unrelated targets obstructing the collision fixtures. The gameplay source hash was unchanged. Evidence is ignored under `.local-backups/pattern-choice-lesson-20261019/final-unity.log` and its report. This is not physical-input, natural full-song, game-render readability or standalone performance acceptance.

The Inspector saves eight pigment Materials and eight paintPatternGroups. Each group contains six same-color prefab references. Awake selects a color index using its separate System.Random stream, reads that material's _BaseColor, and tints the projectile through StudioPaintTint. It then reads that color's group, draws an independent patternIndex bounded by patterns.Length and assigns patterns[patternIndex] to paintMarkPrefab. This keeps the color fixed while independently varying the silhouette. No mark is instantiated until the existing collision branch runs. The older paintMarkVariants field is retained during the incremental rewrite but is no longer used by the saved selector. Array/null guards will be revisited after the basic variation step.

The learner has saved the nested `[System.Serializable] PaintPatternGroup` class with its public GameObject[] prefabs and private serialized PaintPatternGroup[] paintPatternGroups field. At the creator's explicit request, the assistant connected all 48 prefab references in the production projectile asset: eight groups follow the pigment color order, and each has six styles ordered Spray, Burst, Drip, Swoosh, Brush, Starburst. Every group contains only its own color. Existing palette references, prefab GUIDs and gameplay source were preserved.

An isolated Unity batch check loaded the saved production prefab and confirmed eight groups of six non-null object references, exact resource paths/style order, pigment/sprite matches, Ignore Raycast and collider-free assets. The production scene's shooter references that same projectile prefab. This is saved hookup acceptance, not runtime pattern selection. Evidence is ignored under `.local-backups/pattern-hookup-20261019`.

The learner has saved variation only inside the accepted surface-impact branch: capture the instantiated mark, draw an angle with the existing cosmetic System.Random stream, rotate with Space.Self and multiply the instance's original localScale by a bounded random factor. The source preserves the hit point, normal offset, original pigment, collider-free mark and lifetime policy. The learner has also saved the serialized rotation-range field and connected it to the angle formula. The creator has deferred further scale-range lessons and requested a return to the roadmap priorities. Keep the current scale behavior; source review alone does not establish visual readability or standalone acceptance.

## Adding another color later

The creator reaffirmed on October 19 that future core gameplay should be learner-written. Explain one small increment, let the learner implement it, and review/verify before advancing. The palette code already saved here is not a template for silently implementing future gameplay.

Create a latex material and a matching persistent mark. Append the latex to both the balloon and projectile pigment arrays. Append the default mark at the same index in `Balloon.wallPaintPrefabs`, and append a corresponding six-style group in `PaintProjectile.paintPatternGroups`. Preserve the existing order and prefab GUIDs. Inspect several spawns, explosions and direct surface impacts; matching balloon and bullet colors is not required to score.
