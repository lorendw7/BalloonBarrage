# Menu assets

For the latest screen layouts, nine-sliced controls, score/timer plaques and integration lessons, use [Interface V2](INTERFACE_V2.md). This page preserves the original V1 asset reference and exact prompts.

Folder: `Assets/StudioArtPack/Menu`. Generated art is shared across locales; overlay live text using the [localization data](LOCALIZATION.md).

| Asset or sprite | Suggested use |
|---|---|
| MenuBackground_v1.png | Approximately 16:9 menu background |
| Button_Coral | Start/retry |
| Button_Teal | Settings/resume |
| Button_Yellow | Secondary action |
| Button_Cream | Back/quit |
| Dialog_Paper | Pause/settings/results panel |
| Score_Plaque | Score/timer backing |

`MenuUI_Atlas_v1.png` has transparency and six tightly bounded sprite slices. Use Simple UI Images with preserved aspect ratio; nine-slice borders are not configured. Colors suggest roles, not exact interaction-state frames.

These assets do not implement button events or a menu scene. Start, pause, results and retry integration require learner work and runtime acceptance. The prompts below describe the background, then the atlas.


## Recorded generation prompts

```text
Use case: stylized-concept. Asset type: production game main-menu background, wide 16:9 landscape. Create a polished cozy balloon-popping art studio game background, warm cream plaster wall, honey oak frame surrounding a large blank ivory paper central board, subtle paper grain and hand-painted gouache texture. Coral, turquoise and mustard spray-paint splashes confined to outer corners; cheerful shiny teal and coral balloons with thin strings grouped along left and right margins. Straight-on camera, gentle warm light, charming clean indie-game art, no people or weapons. Central 55% of image must be quiet pale empty space for live menu title and three buttons, generous negative space. Frame and decorations remain inside safe margins. This is the actual background texture, not a screenshot or UI mockup. No text, no letters, no buttons, no watermarks. Opaque full-bleed background.
```

```text
Use case: stylized-concept. Asset type: production transparent game UI sprite atlas. A perfectly aligned 2-column by 3-row grid of SIX separated hand-painted cozy studio UI assets, equal square cells, generous transparent gutters. Real transparent alpha background, no checkerboard drawn. Row1 left: wide rounded coral painted wooden button; row1 right: same sized warm teal painted wooden button. Row2 left: same sized mustard yellow painted wooden button; row2 right: same sized muted cream-gray painted wooden button. Row3 left: square cream paper dialog panel with thin honey oak wooden frame, empty center; row3 right: wide cream paper score plaque with small coral and teal spray accents at its corners. All assets front-on flat orthographic, centered in own cells, cohesive warm cream/honey oak/coral/teal/mustard palette, soft handcrafted gouache grain, subtle inner bevel, crisp readable silhouettes, no cast shadow outside asset. Buttons have quiet EMPTY centers for Unity text overlay, identical proportions across first four cells. No text, no numbers, no icons, no labels, no watermark. Transparent gaps between every asset. Keep all elements fully within own cells.
```
