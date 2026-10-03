# 菜单美术 / Menu art
资源目录 / Asset folder: `Assets/StudioArtPack/Menu`.

内置 imagegen 生成；未替换场景或修改玩法。按钮没有文字，供 Unity 叠加简体中文、日文、英文文本。文案见 [三语资源](LOCALIZATION.md)。
Generated with built-in imagegen. No scene replacement or gameplay changes. Add localized live text in Unity; labels are not baked into images.

| 文件或 Sprite / File or sprite | 用途 / Purpose |
|---|---|
| MenuBackground_v1.png | 主菜单背景，约 16:9 / Main menu background, approximately 16:9 |
| Button_Coral | 开始或重试 / Start or retry |
| Button_Teal | 设置或继续 / Settings or resume |
| Button_Yellow | 次要操作 / Secondary action |
| Button_Cream | 返回或退出 / Back or quit |
| Dialog_Paper | 暂停、设置、结算底板 / Pause, settings, results panel |
| Score_Plaque | 分数或计时底板 / Score or timer plaque |

MenuUI_Atlas_v1.png 含透明通道，已配置六个紧边界 Sprite。建议 UI Image 使用 Simple 并保持比例；尚未设置九宫格边界。颜色代表用途建议，不是同一按钮的精确状态帧。当前仅为美术素材，未连接按钮事件或制作菜单场景。
MenuUI_Atlas_v1.png has alpha and six tightly bounded sprite slices. Use Simple UI Images with preserved aspect ratio; nine-slice borders are not configured. Colors suggest roles, not pixel-matched interaction states. Art only: no button events or menu scene are included.

## 生成提示词 / Exact generation prompts

### 背景 / Background
Use case: stylized-concept. Asset type: production game main-menu background, wide 16:9 landscape. Create a polished cozy balloon-popping art studio game background, warm cream plaster wall, honey oak frame surrounding a large blank ivory paper central board, subtle paper grain and hand-painted gouache texture. Coral, turquoise and mustard spray-paint splashes confined to outer corners; cheerful shiny teal and coral balloons with thin strings grouped along left and right margins. Straight-on camera, gentle warm light, charming clean indie-game art, no people or weapons. Central 55% of image must be quiet pale empty space for live menu title and three buttons, generous negative space. Frame and decorations remain inside safe margins. This is the actual background texture, not a screenshot or UI mockup. No text, no letters, no buttons, no watermarks. Opaque full-bleed background.

### 图集 / Atlas
Use case: stylized-concept. Asset type: production transparent game UI sprite atlas. A perfectly aligned 2-column by 3-row grid of SIX separated hand-painted cozy studio UI assets, equal square cells, generous transparent gutters. Real transparent alpha background, no checkerboard drawn. Row1 left: wide rounded coral painted wooden button; row1 right: same sized warm teal painted wooden button. Row2 left: same sized mustard yellow painted wooden button; row2 right: same sized muted cream-gray painted wooden button. Row3 left: square cream paper dialog panel with thin honey oak wooden frame, empty center; row3 right: wide cream paper score plaque with small coral and teal spray accents at its corners. All assets front-on flat orthographic, centered in own cells, cohesive warm cream/honey oak/coral/teal/mustard palette, soft handcrafted gouache grain, subtle inner bevel, crisp readable silhouettes, no cast shadow outside asset. Buttons have quiet EMPTY centers for Unity text overlay, identical proportions across first four cells. No text, no numbers, no icons, no labels, no watermark. Transparent gaps between every asset. Keep all elements fully within own cells.
