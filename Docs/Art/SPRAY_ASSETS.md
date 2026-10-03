# 喷漆素材 v3 / Spray assets v3
资源目录 / Asset folder: `Assets/StudioArtPack/Textures` and `Assets/StudioArtPack/SprayPrefabs`.
内置 image_gen 生成，透明 PNG，1536×1024；3 列×2 行，每格 512×512。已检测到透明 Alpha；预览中透明区域可能显示底色，Unity 按 Alpha 显示。
Generated with built-in image_gen. Transparent 1536×1024 PNG, 3 columns by 2 rows, 512×512 cells. Alpha transparency was detected; preview background colors may differ from Unity's alpha rendering.

| Sprite / Prefab | 用途 / Use |
|---|---|
| Coral_Burst | 珊瑚放射喷溅 / Coral radial splash |
| Teal_Spray | 青绿圆形喷雾 / Teal round spray |
| Yellow_Drip | 黄色滴落颜料 / Yellow dripping paint |
| Indigo_Swoosh | 靛蓝弧形笔刷 / Indigo curved stroke |
| Pink_Brush | 粉色宽笔刷 / Pink broad brush |
| Turquoise_Burst | 青色星状喷溅 / Turquoise starburst |

纹理在 Textures/GraffitiSpray_Atlas_v3.png，Unity 切分后 Prefab 在 SprayPrefabs。打开实际项目 Assets/StudioArtPack/SprayPrefabs，将一个 Prefab 拖到 Environment/Graffiti 下；按实际墙面位置调整，不覆盖现有涂鸦。它们没有 Collider，不拦截气球点击。
Texture: Textures/GraffitiSpray_Atlas_v3.png. Prefabs: SprayPrefabs after Unity import. Drag one under Environment/Graffiti in the actual project and align to the wall; existing graffiti is preserved. No colliders are added, so they do not intercept balloon clicks.

Pixels Per Unit = 200，完整格宽约 2.56 Unity 单位；Scale 0.6–1 是布局起点。保持 Sprite 比墙面稍靠近相机，避免共面；旋转 Z 改变图案角度。
At 200 pixels per unit, each cell spans about 2.56 Unity units; start with scale 0.6–1. Place slightly in front of the wall to avoid coplanar flicker; rotate Z to angle the mark.

当前仅提供装饰及未来击破效果素材，未改写命中逻辑或自动在墙上喷漆。
These are decoration and future pop-feedback assets; hit logic and automatic wall painting are not modified.

## 原始提示词 / Exact prompt
```text
Use case: stylized-concept. Asset type: transparent PNG paint splat sprite atlas for cozy balloon popping graffiti game. Exactly 6 isolated paint marks arranged in a precise evenly spaced 3 column by 2 row grid, landscape 3:2 aspect. Each mark fully contained within its own cell with generous transparent gutters and no overlaps. Top row: coral-red radial splash with rounded droplets; teal round spray-paint blot with scattered droplets; golden-yellow dripping paint blot. Bottom row: indigo curved broad brush swoosh; coral-pink short horizontal dry-brush stroke; turquoise starburst paint splat. Friendly joyful acrylic and aerosol paint, rich saturated colors with hand-painted pigment texture, no blood-like appearance. Actual transparent background alpha, no painted checkerboard, no background, no frame, no text, no labels, no shadows, no balloons or objects. Crisp readable silhouettes with soft speckled outer edges. Production sprite sheet, flat front view.
```
