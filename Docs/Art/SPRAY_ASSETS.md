# Spray assets

`Assets/StudioArtPack/Textures/GraffitiSpray_Atlas_v3.png` is a generated transparent 1536×1024 atlas: three columns, two rows, 512-square cells. Prefabs live in `Assets/StudioArtPack/SprayPrefabs`.

| Prefab | Appearance |
|---|---|
| Coral_Burst | Radial coral splash |
| Teal_Spray | Round teal spray |
| Yellow_Drip | Dripping yellow paint |
| Indigo_Swoosh | Curved indigo stroke |
| Pink_Brush | Broad pink stroke |
| Turquoise_Burst | Turquoise starburst |

Place a prefab beneath `Environment/Graffiti`, align it to the wall, and preserve existing graffiti. At 200 pixels per unit each cell spans about 2.56 Unity units; scale 0.6–1 is a layout starting point. Offset from the wall to avoid coplanar flicker and rotate around Z for variation.

The prefabs have no colliders. These are decorative visuals and possible future impact assets; they do not automatically paint the wall. Transparency was checked at asset preparation time; verify final rendering in the game camera.


## Recorded generation prompts

```text
Use case: stylized-concept. Asset type: transparent PNG paint splat sprite atlas for cozy balloon popping graffiti game. Exactly 6 isolated paint marks arranged in a precise evenly spaced 3 column by 2 row grid, landscape 3:2 aspect. Each mark fully contained within its own cell with generous transparent gutters and no overlaps. Top row: coral-red radial splash with rounded droplets; teal round spray-paint blot with scattered droplets; golden-yellow dripping paint blot. Bottom row: indigo curved broad brush swoosh; coral-pink short horizontal dry-brush stroke; turquoise starburst paint splat. Friendly joyful acrylic and aerosol paint, rich saturated colors with hand-painted pigment texture, no blood-like appearance. Actual transparent background alpha, no painted checkerboard, no background, no frame, no text, no labels, no shadows, no balloons or objects. Crisp readable silhouettes with soft speckled outer edges. Production sprite sheet, flat front view.
```
