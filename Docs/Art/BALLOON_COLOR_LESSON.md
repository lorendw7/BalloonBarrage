# Balloon and projectile palette

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

The production `PaintDrop3D_Teal.prefab` retains its legacy name and GUID. `PaintProjectile.Awake()` selects one random palette index from a separate System.Random stream, applies the corresponding material's `_BaseColor` through `StudioPaintTint` to the flying projectile, and selects that index's persistent mark prefab. Repeated colors are allowed. Unconfigured legacy projectile prefabs retain their existing behavior. The production shooter has no gun-theme component overriding this tint.

Balloon hits use the balloon's pigment for its explosion/deposits; directly hitting an eligible flat receiver uses the projectile's pigment. This adds no color-matching scoring rule. Timing, speed, collision queries, ordinary/on-beat points and persistent-mark lifetime are unchanged. The four receivers remain MainWall, Floor, LeftWall and RightWall; retry clears marks.

## Art provenance

The five new opaque latex materials copy existing CoralLatex settings and change only their name and pigment. Their sRGB inputs are SkyBlue `#489DEC`, Violet `#9A6DE0`, Rose `#EB71A6`, Lime `#91C94F` and Tangerine `#EF903C`.

The five new surface prefabs reuse the existing Teal_Spray sprite, opacity, scale, Ignore Raycast layer and collider-free setup. `PalettePaint.shader` preserves the sprite alpha and takes its brightest RGB channel as monochrome shading before applying the pigment. This avoids multiplying new colors by the original teal hue. Each new pigment has one shared material; no per-shot material copies or texture downloads are introduced. Original model/atlas attribution and exact image-generation prompts remain applicable and unchanged; see [spray assets](SPRAY_ASSETS.md#recorded-generation-prompts).

## Verification

Unity 6000.0.77f1 compiled the saved expansion. Isolated Editor Play Mode checks passed eight pigments on each of four production receivers for both balloon-pop deposits and actual SphereCast projectile collisions (32 combinations per path). Projectile checks sampled 256 random spawns, reached all eight pigments and compared each renderer tint with its selected mark. Checks retained ordinary/on-beat 10/20 scoring, rejected ineligible surfaces, preserved marks beyond eight seconds through pause/results and cleared them on retry. The native Unity preview rendered all eight balloon materials, projectile tints and mark shaders after allowing material buffers to update. Logs, probes and the preview remain ignored under `.local-backups/palette-20261019`. The public v0.1.0-demo.2 package does not contain it. Human readability, full-song load and exhibition-machine acceptance remain separate.

## How the projectile selects its color

The Inspector saves two equally sized arrays: `pigments` contains latex Materials, and `paintMarkVariants` contains the corresponding persistent mark prefabs. `Awake` runs when the projectile initializes. It first rejects missing/empty/misaligned arrays, then selects one index with `pigmentRandom.Next(pigments.Length)`. The upper bound is excluded, so eight entries produce indices 0 through 7. Null entries retain the original fallback instead of throwing.

`pigments[index].GetColor("_BaseColor")` reads a color from the selected material. `StudioPaintTint.Effect` applies that color to this projectile's renderers without changing shared material assets. Assigning `paintMarkPrefab = paintMarkVariants[index]` selects the template used later by the existing collision branch; it does not instantiate a mark yet. Both operations use the same index, which keeps the flying paint and its impact mark aligned. The separate System.Random instance avoids consuming Unity's spawn/movement random sequence.

## Adding another color later

The creator reaffirmed on October 19 that future core gameplay should be learner-written. Explain one small increment, let the learner implement it, and review/verify before advancing. The palette code already saved here is not a template for silently implementing future gameplay.

Create a latex material and a matching persistent mark. Append the latex to both the balloon and projectile pigment arrays. Append the mark at the same index in both `Balloon.wallPaintPrefabs` and `PaintProjectile.paintMarkVariants`. Preserve the existing order and prefab GUIDs. Inspect several spawns, explosions and direct surface impacts; matching balloon and bullet colors is not required to score.
