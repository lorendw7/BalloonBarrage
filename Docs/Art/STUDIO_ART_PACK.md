# StudioArtPack / 工作室美术包
更新 / Updated: 2026-10-10

## 当前 V5：工作室细节 / Current V5 studio details

在 V4 暖光、橡木与米灰墙面的基础上补充使用痕迹，保持中央靶区与相机位置。新增：窗边折叠帘、帘杆与窗扣；带拇指孔和颜料点的调色盘、笔刷、杯子与颜料管；工作台下层速写本与空心纸卷；右墙工具挂板、色样纸、滚筒与装饰时钟；带脚轮的画材推车、备用画布和折叠布；圆木凳、左侧壁龛书架及窄木踢脚线。
V5 adds signs of use while retaining V4 lighting, oak, plaster, framing and the clear target area: gathered curtains and hardware; a thumbhole palette, brushes, mug and paint tubes; sketchbooks and hollow paper rolls; a tool pegboard, color study, roller and decorative clock; a wheeled canvas cart, folded cloth, wooden stool, niche books and narrow skirting.

资源位于 `Assets/StudioArtPack/DetailsV5`，场景统一收在 **StudioDetailsV5**。包含五个新材质、六个共享网格和八个可复用 Prefab；窗帘褶皱、杯子的内壁、纸卷开口、调色盘拇指孔与挂板孔列均为实际网格。其余复用既有橡木、颜料和金属材质，不新增图片、外部模型、实时灯光或装饰碰撞体。时钟为静态装饰，窗帘没有布料模拟。
DetailsV5 contains five materials, six shared meshes and eight connected reusable prefabs under StudioDetailsV5. Cloth folds, cup interiors, tube openings, the palette hole and pegboard perforation marks are geometry. Existing oak, pigment and metal materials are reused; there are no new images, external models, realtime lights or decor colliders. The clock is decorative and curtains do not simulate cloth.

`Tools → Balloon Studio → Add Studio Details V5` 为一次性添加入口；当前 PrototypeScene 已应用，不必重复执行。工具要求现有 V4，备份场景并拒绝覆盖已有 V5；之后直接调整场景组或 Prefab。八个 Prefab 为 WindowDetails、WorkbenchObjects、WorkbenchPaperShelf、PainterPegboard、StudioWallClock、CanvasSupplyCart、PainterStool、NicheBookShelf。新增几何共 155 个 MeshRenderer、16,218 顶点；这是资源统计，不是帧率测量。
The one-time editor menu requires V4, backs up the scene and refuses to overwrite V5. The current PrototypeScene already includes it; edit its group or prefabs directly. The eight prefabs cover window, desktop, lower shelf, pegboard, clock, cart, stool and niche books. Added geometry has 155 MeshRenderers and 16,218 vertices; these counts are not FPS measurements.

独立 Unity 副本中完成构图、四角瞄准与引用检查，以及多球/连射回归。最终图由正常运行生成三个气球后截取相机，保存到 `Assets/Art/Previews/PrototypeScene_20261010.png`，官网复用此图。同步时核对原有玩法、Prefab、配乐和场景 GUID，清理 Unity 渲染初始化产生的额外灯光数据；除新增场景根列表外，原有场景组件内容保持一致。
An isolated Unity copy validates composition, four-corner aiming, references and target/fire regression. The canonical preview captures three normally spawned runtime targets and is reused on the site. Sync checks preserve original gameplay, prefabs, music and scene GUID; render-initialization light metadata is excluded. Existing serialized scene components remain unchanged except for the expanded root list.

## V4：参考图与材质基础 / V4 reference and material foundation

继续向官网的 CozyGraffitiStudio 参考图靠拢：新细纹橡木替换粗木纹，两侧设置青绿色拱形壁龛、储物搁板与小型暖光；整片下墙色块隐藏，保留米灰墙面。植物换为带曲面的尖叶，并增加窗边陶盆；天窗改为更明亮的天空色，减轻大块网格阴影对靶区的干扰。V3 的宽镜头与低位枪布局保留。
The current pass adds finer oak, arched teal niches with warm shelf lights, curved foliage and window planters. Large lower-wall panels are hidden and the skylight reads brighter. V3 framing remains. The website image is a visual reference; this does not claim identical concept-art rendering.

资源位于 `Assets/StudioArtPack/ReferenceV4`，新增物体在场景 **StudioReferenceV4** 下。新木纹通过内置 imagegen 制作，成品为 `Textures/FineOak_v4.png`；已目视检查，不宣称边缘经过逐像素无缝验证。壁龛、叶片、灯光和材质在 Unity 内制作，不是把参考图作为游戏背景替代三维场景。
Assets live in ReferenceV4 and scene additions under StudioReferenceV4. FineOak_v4.png uses built-in imagegen and was visually inspected, without claiming pixel-tested seamless edges. Alcoves, leaves, lighting and materials are native Unity assets.

音乐保留当前文件名与 GUID，主曲仍为 108 BPM。键盘音色加入三弦微失谐、非整数倍泛音、衰减和弱击弦瞬态，替换持续的电子主音；和弦加入轻微力度/时序差异与低音量和声铺底。仍是原创合成作品，不是钢琴实录；五段文件均更新，实际听感需试听确认。
Music preserves filenames/GUIDs and tempo. A softer synthesized piano model replaces the sustained electronic lead, with restrained voicing, small timing/velocity variations and a quiet harmonic bed. It is original synthesis, not a recorded piano performance.

### FineOak_v4.png 原始提示词 / Exact prompt
```text
Production seamless albedo texture for a warmly lit semi-realistic stylized 3D artist studio, matching high-end cozy game environment rendering. Entire square is natural honey oak, fine straight vertical grain, narrow delicate grain lines with gentle variation, subtle wood pores, soft golden tan warm medium-light brown. Restrained realistic grain, low contrast, satin unfinished timber. Flat front-on orthographic diffuse texture, perfectly even illumination, no baked light, no shadows, no specular highlights, no knots, no dramatic swirling grain, no boards, no seams, no objects, no frame, no labels, no text. Full bleed single tileable texture, not a room render or material sphere.
```

## V3：暖光与镜头基础 / V3 daylight and framing foundation

按用户最新反馈，场景从浅色平涂卡通转向官网参考图的暖光材质风格。当前 PrototypeScene 使用奶油灰抹灰墙、深青绿下墙、胡桃木色梁架/工作台和少量赭红、黄铜色道具；恢复木纹、微水泥、粗糙度、接触阴影与斜向窗格日光，并增加天窗。V2 道具保留，旗串在场景中隐藏，卡通材质已由场景材质覆盖。
The latest request replaces flat pastel shading with physical materials, walnut framing, cream-grey plaster, deep teal panels and restrained rust/brass accents. Daylight, shadows, concrete/wood textures and a skylight follow the website reference. V2 props remain; pennants are hidden and scene material overrides replace toon shading.

镜头位置 **(0, 0.10, -8.8)**，俯角 **2°**，FOV **45°**，用 16:9 Game 视图验收。枪的相机局部位置 **(0.47, -0.51, 1.35)**，统一缩放 **0.50**，留出中央和地面供欣赏场景。新增材质、UV 网格与窗光遮罩位于 `Assets/StudioArtPack/AtmosphereV3`，天窗和窗光位于场景 **StudioAtmosphereV3**。
Camera framing is wider and slightly farther back; the gun is smaller and lower. AtmosphereV3 contains physical materials, UV-enabled meshes and the native Unity light mask. This matches the reference's direction, not a claim of pixel-identical concept rendering.

配乐同步柔化：主曲 **108 BPM / 71.111 秒**，菜单 **84 BPM / 22.857 秒**。减少旋律密度、改用圆润的键盘音色，弱化底鼓、改为轻边鼓与刷镲，场景播放音量 **0.14**。只保留五段当前版本，重命名时保留 .meta GUID，避免断开场景与 Prefab 引用。
Music is softer and slower, with restrained percussion, rounder keyboard tones and more space between phrases. Five current tracks retain their GUIDs during renaming; scene playback volume is 0.14.

## V2 道具搭建记录 / V2 prop construction history

按官网的 CozyGraffitiStudio 概念图补充真实三维道具：滴漆颜料桶、带板条木箱、画架与抽象画布、画笔杯、艺术书本、左右音箱、窗边小搁板、垂挂绿植、彩色旗串、条纹地毯与少量地面漆点。主框架保留蜂蜜色，侧墙加入薄荷与淡紫，下墙和边框用青绿/珊瑚形成前后层次；原墙涂鸦保留。
Real 3D decor follows the website concept: paint buckets, crates, easel, brushes, books, stereo shelves, vines, pennants, rug and floor accents. Honey framing, mint/lavender walls and teal/coral trims add depth while preserving the existing graffiti.

圆角网格与 SoftToon 材质由编辑器生成，使用柔和的三档明暗塑形，不新增实时灯光。资源在 `Assets/StudioArtPack/CartoonV2`，场景内新增装饰位于 **CartoonStudioV2**；所有新装饰不带 Collider。材质与网格独立保存，不覆盖旧美术文件或修改玩法。
Rounded meshes and a soft three-band URP toon shader are editor-generated; no new realtime lights are added. New assets live in CartoonV2; new scene decor is grouped under CartoonStudioV2 and has no colliders. Existing art files and gameplay remain intact.

`Tools → Balloon Studio → Apply Cartoon Studio V2` 可在尚未升级的原型执行；当前场景已应用时，不必再次运行，工具会拒绝覆盖已有组。先备份场景，然后直接调整组内物体；道具组合也保留为可复用 Prefab。概念图是美术参考，最终画面以 Unity 相机渲染为准。
The editor menu applies V2 once and refuses to overwrite an existing group. Adjust grouped objects directly afterwards. Reusable decor prefabs are included; the concept image is a reference, not a gameplay screenshot.

## 10/31 展示场景与配乐 / Exhibition studio and music

当前入口是 `Assets/Scenes/PrototypeScene.unity`，已直接应用这一版场景。旧 `Generated/StudioArtPreview.unity` 保留为历史预览，不用于当前试玩。
Open PrototypeScene for the current game; the older StudioArtPreview is a historical art preview.

- 枪是 Main Camera 的子物体，当前镜头和枪参数见上方 V3；位置、缩放、旋转是不同设置，不要把缩放值填到 Rotation。
- V1 建立窗景、画框、工作台、颜料罐、音箱、盆栽与装饰灯罩；V3 已按最新要求调整材质、相机和灯光。物体集中在边缘，中央保留靶区，原涂鸦保留。
- `ExhibitionV1/Prefabs` 包含 CourtyardWindow、FramedBalloonPoster、PaintWorkbench 和 StudioBackgroundMusic。场景内新增物体统一放在 **ExhibitionStudioV1** 下，后续可直接移动或替换。装饰无 Collider，避免挡住射击。
- 建场景时，先在 Game 中选择 16:9，再从相机视角调整边缘道具。左墙放窗与工作台，右侧放盆栽和画框，上方放横梁；不要只看 Scene 视图判断遮挡。构图预览独立放置三球，当前运行生成器也已恢复默认 3 球的列表和补充逻辑。
- `Tools → Balloon Studio → Build Exhibition Studio V1` 是编辑器组装工具；当前场景已经生成，无需再执行。工具拒绝覆盖已有 ExhibitionStudioV1，防止丢失手动调整，首次生成会备份原场景。

The gun is smaller and sits at the lower right. New props are grouped under ExhibitionStudioV1 and have no colliders. Four reusable prefabs are provided. Judge placement in a 16:9 Game view; three preview balloons illustrate composition only. The editor tool backs up the scene and refuses to overwrite an existing group.

### 音乐使用 / Music use

以下均为原创合成配乐，44.1 kHz、双声道、16-bit WAV，不使用录音或外部采样。制作源文件为 `Tools/Audio/compose_studio_music.py`（Python + NumPy）；时间与电平记录在 `Assets/Audio/Music/MusicManifest.json`。

| 文件 / File | 用途 / Use | 长度 / Duration |
|---|---|---|
| StudioDaylight_Loop.wav | 108 BPM 温暖键盘、轻鼓与留白旋律 / Warm keys and restrained percussion | 32 小节，71.111 秒 |
| StudioDaylight_Drums_Loop.wav | 柔和的纯鼓替代版；不要与主配乐叠放 / Soft drum-only alternative | 32 小节，71.111 秒 |
| StudioDaylight_Menu_Loop.wav | 84 BPM 柔和菜单循环 / Gentle menu loop | 8 小节，22.857 秒 |
| RoundStart_Soft.wav | 轻柔四拍提示 / Soft four-beat count-in | 2.222 秒 |
| RoundComplete_Soft.wav | 低八度、圆润的结算短音 / Rounded completion sting | 2.222 秒 |

场景的 **ExhibitionStudioV1 → StudioBackgroundMusic → Audio Source** 已赋值当前主配乐，Loop 与 Play On Awake 开启，Spatial Blend = 0（2D），Volume = 0.14。回 Unity 停止 Play、刷新并重新打开 PrototypeScene，然后 Play 试听；在该组件调整音量。循环曲使用 Streaming/Vorbis，短音效使用 PCM/Decompress On Load。按用户要求只保留当前五段，场景与音乐 Prefab 共用主曲。
The current groove is assigned to the scene and reusable looping 2D AudioSource at volume 0.14. Refresh and reopen PrototypeScene, then press Play to audition. Only the five current tracks remain. Loops stream as Vorbis; cues use decompressed PCM.

菜单音乐和开场/结算短音已备好，尚未绑定回合事件；等学习者完成菜单与计时流程再接入。此轮未修改核心玩法。循环按完整小节制作，音尾跨边界延续，接口采用 3 毫秒平滑收尾；WAV 无削波，最终听感与 Unity 内循环仍需试听验收。
Menu music and cues are prepared but not connected to future events. Note tails wrap across full bars; a 3 ms seam taper removes noise discontinuities. WAV clipping checks passed; listening and in-game loop acceptance remain required.

### 新图片来源 / New image provenance

两张图片使用内置 imagegen 生成，成品在 `Assets/StudioArtPack/ExhibitionV1/Textures`。窗景是虚构的校园风格庭院，不是九州大学实景或 Qpic 官方图像；气球装饰画不含文字或标识。使用独立 URP/Unlit 材质、Clamp、Bilinear、Mip Maps、最大尺寸 2048，配合窗框/画框预制体，不作为无缝纹理平铺。
Both images were generated with built-in imagegen. The fictional courtyard does not depict Kyushu University or official Qpic imagery. Separate unlit materials use clamped, mipmapped textures, not repeating tiles.

#### WindowCourtyard_v1.png — 原始提示词 / Exact prompt
```text
Use case: stylized-concept. Asset type: production opaque texture for a tall window panel in a cozy Unity 3D graffiti studio. Primary request: a peaceful stylized courtyard view, entirely outdoors, as seen through a window but WITHOUT the window frame or any interior. Composition: portrait 3:4 artwork, full-bleed landscape with pale apricot afternoon sky in upper half, soft green trees framing distant simple cream campus-like buildings, a quiet stone walkway at bottom. Style: polished hand-painted low-poly game illustration, broad simple shapes, low visual noise, soft warm sunlight. Palette: warm cream #F4E6D0, honey wood browns, muted teal #3EC6C0, green foliage, small coral accents. Constraints: image fills all edges; no frame, glass reflection, room, gun, balloons, people, text, letters, signs, logos, watermark or UI. This is a directly usable window-view texture, not a concept sheet and not a photographic depiction of any real campus.
```

#### BalloonPoster_v1.png — 原始提示词 / Exact prompt
```text
Use case: stylized-concept. Asset type: production opaque artwork for a square framed poster inside a cozy Unity graffiti studio. Primary request: playful abstract balloon-and-paint artwork. Composition: square full-bleed warm ivory handmade paper; three stylized rounded pear-shaped balloon forms, coral #FF6B6B, teal #3EC6C0, mustard #FFD166, clustered asymmetrically with small indigo #5B5F97 paint droplets and loose charcoal curved strings. Big shapes, generous breathing space, subtle paper grain. Style: tasteful screen-printed illustration with slightly imperfect ink edges, friendly and modern, matching a warm honey-wood game studio. Constraints: just the artwork, flat front-on, fills the square; no physical frame, wall, room, perspective, shadows, text, lettering, numbers, logos, badges, watermark, gun, people or UI. This is a directly usable poster texture, not a reference sheet.
```

## 早期风格记录（由上方 V3 更新） / Earlier direction, superseded by V3
保留用户认可的鲜艳青色、黄色、珊瑚涂鸦与浅色背景组合。本次不降低旧涂鸦透明度，不改相机和已有灯光。
Preserve the approved vibrant teal/yellow/coral graffiti and pale background. This pack does not reduce existing splat opacity or alter the camera and lights.

## 资源 / Assets
- Textures/HoneyWood.png：手绘蜂蜜木纹，木框与台阶 / Painted honey wood for frame and stage.
- Textures/WarmConcrete.png：低对比微水泥地面 / Quiet microcement floor.
- Textures/PaperPanel.png：无文字界面纸张面板 / Blank paper UI panel.
- Generated：编辑器生成 10 个材质、3 个气球颜色 Prefab、音箱、盆栽、颜料罐、纸张 UI 面板和 PaintBurst 粒子 Prefab。
  Editor output: ten materials, three balloon-color prefabs, speaker, plant, paint cans, UI panel, and PaintBurst particle prefab.
- Generated/StudioArtPreview.unity：实际 PrototypeScene 的副本，应用木纹/地面，增加边缘装饰，生成器使用珊瑚气球。保留现有涂鸦、相机、灯光及玩法代码。
  Copy of the actual PrototypeScene with wood/floor, edge props, and a coral spawner prefab, preserving existing graffiti, camera, lights, and gameplay.

## 早期预览的导入与打开 / Historical preview import
统一项目路径 / Unified editor project: D:/CS/Code/BalloonBarrage.
回到 Unity，执行 Assets > Refresh，等待编译。首次导入自动生成预览；如正在 Play，停止后执行 Tools > Balloon Studio > Build Art Preview。
Return to Unity, use Assets > Refresh, and wait for compilation. First import creates the preview automatically; if playing, stop then use Tools > Balloon Studio > Build Art Preview.

打开 Generated/StudioArtPreview.unity 后 Play。原始 PrototypeScene 不被替换。已存在预览时安装器不会覆盖；新一版应使用新的输出目录。
Open Generated/StudioArtPreview.unity and press Play. The original scene is preserved. Existing previews are not overwritten; future versions should use a new output directory.

UI 面板是可复用 RawImage Prefab，不覆盖 Score.OnGUI。PaintBurst 未接入命中；气球音效代码已由用户完成，正式菜单逻辑仍待开发。10/09 主配乐已接入 PrototypeScene，见上方。三色气球为独立 Prefab，没有随机换色逻辑。菜单美术见 [菜单素材](MENU_ASSETS.md)。
The reusable RawImage does not replace Score.OnGUI. PaintBurst is not wired; pop-audio code exists, while menu logic remains pending. The October 9 main music is connected in PrototypeScene. Color prefabs are separate, without runtime randomization.

## 制作方式 / Production
纹理使用内置 image_gen，三维装饰使用 Unity 基础网格组合，材质及 Prefab 使用 Editor 工具生成。生成器只在编辑器运行，不改用户玩法脚本。
Textures use built-in image_gen; 3D props are Unity primitive assemblies; materials and prefabs are generated by an editor-only tool without changing learner gameplay.

生成纹理已目视检查；不宣称无缝边缘经过像素级测试。最终布局、命中体验、UI 接线需在 Game 中验收。
Textures were visually inspected; seamless edges are not claimed pixel-tested. Validate final composition, clicking, and later UI integration in Game view.

### 木纹提示词 / Wood prompt
```text
Use case: stylized-concept. Production square seamless albedo texture for cozy graffiti game wooden stage beams. Honey oak warm mid brown, wide subtle flowing wood grain vertically oriented, hand-painted stylized game art. Flat even color, low contrast, no knots resembling eyes, no boards seams, no perspective, no lighting, no shadows, no text, no frame. Entire square filled with texture, tileable edges.
```

### 地面提示词 / Floor prompt
```text
Use case: stylized-concept. Production seamless square floor albedo texture for a cozy graffiti studio game. Warm light grey beige matte microcement, extremely subtle tiny mineral flecks and cloudy plaster variations. Even neutral lighting, low contrast, no cracks, no tiles, no seams, no perspective, no cast shadows, no objects, no text, no graffiti. Tileable edges. Hand-painted game material, visually quiet.
```

### UI 提示词 / UI prompt
```text
Use case: stylized-concept. Asset type: game UI background panel single asset. Wide landscape 3:2 image completely filled by warm ivory handmade paper texture with a rounded hand-drawn dark charcoal border inset 5 percent, tiny teal and coral paint flecks only in corners. Large completely blank center for live score/settings text, no words no icons no typography no watermark. Flat front view, no perspective, no external shadow, subtle texture, cozy graffiti studio game. Opaque image suitable for rectangular UI panel, designed to remain legible when scaled small.
```
