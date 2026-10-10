# 美术素材清单 / Art asset catalog
喷漆枪复用与补充 / Existing guns and paint supplements: [说明 / Details](PAINT_BLASTER.md).
新增橡胶碎片爆破包 / New latex pop pack: [说明 / Details](BALLOON_POP_V2.md).
更新日期 / Updated: 2026-10-10

官网复用原型运行截图、工作室/油漆枪概念与气球参考；Website/assets.json 只维护原图路径，输出不提交。原图留在 Assets 分类，概念图标为非实机，不复制模型/图集包到网站。 / The site references a gameplay camera capture and canonical concept art via a manifest, with generated copies ignored. Preserve Assets originals and label concepts as non-gameplay; do not duplicate model/atlas packages.

| 文件 / File | 用途 / Use | 状态 / Status |
|---|---|---|
| Assets/Art/Concepts/CozyGraffitiStudio_Concept.png | 原环境参考 / Original environment reference | 既有素材保留 / Existing, retained |
| Assets/Art/Textures/GraffitiSplats_Atlas.png | 4×2 喷溅图集 / 4×2 splat atlas | 既有素材保留 / Existing, retained |
| Assets/Art/Textures/GraffitiWall_Quiet_v2.png | 奶油底边缘涂鸦 / Cream wall with edge paint | 新增、不透明、1536×1024 / New, opaque, 1536×1024 |
| Assets/Art/Concepts/Balloon_Readability_v2.png | 三色气球造型参考 / Three-color balloon reference | 新增、不透明、1536×1024 / New, opaque, 1536×1024 |

## 导入约定 / Import contract
新墙面作为 Default 纹理、sRGB 开启、Clamp、Bilinear、最大尺寸 2048、有 Mip Maps。它不是无缝纹理；在 3:2 独立面板上使用一次，避免 Repeat 拉伸或重复图案。
Import the wall as Default, sRGB on, Clamp, Bilinear, max size 2048, with mipmaps. It is not seamless; use once on a 3:2 panel rather than repeating or stretching.

创建独立 URP/Lit 材质：Opaque、Base Color 白色、Metallic 0、Smoothness 约 0.15，Base Map 使用新墙面。面板放在原墙前少许且木框后，移除装饰面板 Collider，避免拦截点击。不要覆盖原墙材质。
Use a separate URP/Lit material: Opaque, white Base Color, Metallic 0, Smoothness about 0.15, and the new Base Map. Place the panel slightly in front of the wall but behind the frame, without a collider that intercepts clicks. Preserve the original material.

旧图集按 Grid By Cell Count 切成 4 列 2 行，不再假定每格固定 448 像素；实际尺寸以导入器为准。新气球参考图有底色，不切成游戏 Sprite。
Slice the existing atlas by cell count: four columns, two rows. Do not assume a fixed 448-pixel cell; use actual import dimensions. The new balloon reference is opaque and should not be sliced into gameplay sprites.

场景和 Prefab 已从实际 Unity 项目同步到统一目录，保留其 .meta。原型与美术预览是不同入口；资源存在不代表全部已接入玩法。
Scenes and prefabs have been consolidated from the editor project with metadata preserved. Prototype and art preview are separate entry points; asset presence does not imply gameplay integration.

## 来源与生成 / Provenance
下载来源记录见根目录 [CREDITS](../../CREDITS.md)，保留作者声明并待补具体下载页面。展示场景新图片的原始提示词、配乐用途与制作方式见 [工作室美术包](STUDIO_ART_PACK.md)。
See root CREDITS for recorded third-party attribution; retain declarations and add exact download URLs later.

## 统一资源分类 / Unified inventory
| 路径 / Path | 数量与用途 / Count and use |
|---|---|
| Assets/Art/Balloon | 1 个源气球模型 / One source balloon model |
| Assets/Art/BlasterKit | 40 FBX：18 枪、4 泡沫弹、2 弹夹、3 箱子、2 手榴弹造型、3 瞄具、2 枪口、1 烟雾网格、5 靶及碎片 / 40 models: 18 blasters, 4 darts, 2 clips, 3 crates, 2 grenade props, 3 scopes, 2 muzzle parts, 1 smoke mesh, 5 targets/fragments |
| Assets/Audio | 5 普通+5 玻璃轻碰撞音 / Five generic and five glass impacts |
| Assets/Audio/Music | 3 原创循环曲、开场与结算短音；主曲已接入原型 / Three original loops and two cues; main groove assigned in PrototypeScene |
| Assets/Prefabs | 当前玩法气球 / Gameplay balloon |
| Assets/StudioArtPack/Textures | 生成的墙面装饰、木纹、地面、纸张、喷漆 / Generated environment and spray art |
| Assets/StudioArtPack/Generated | 材质、气球颜色变体、装饰、粒子与预览 / Materials, variants, props, particles, preview |
| Assets/StudioArtPack/ExhibitionV1 | 2 生成图片、2 材质、窗/画框/工作台/背景音乐 4 Prefab，已布置到原型 / Two generated images, two materials and four prefabs placed in PrototypeScene |
| Assets/StudioArtPack/CartoonV2 | 卡通材质、圆角网格、颜料桶/木箱/画架/画笔/地面装饰/旗串组合 / Toon materials, rounded meshes and reusable bucket/crate/easel/brush/floor/pennant assemblies |
| Assets/StudioArtPack/AtmosphereV3 | 当前受光材质、带 UV 的圆角网格、窗格光遮罩；深色工作室风格 / Current physical materials, UV meshes and native window-light mask |
| Assets/StudioArtPack/ReferenceV4 | 新细橡木纹理、壁龛/叶片网格、细化材质 / Fine oak texture, alcove/leaf meshes and refined materials |
| Assets/StudioArtPack/DetailsV5 | 窗边、画材、家具与墙面细节；8 Prefab、6 网格、5 材质，已加入原型 / Window, art-tool, furniture and wall details; eight prefabs, six meshes and five materials placed in PrototypeScene |
| Assets/Art/Previews/PrototypeScene_20261010.png | 当前 V5 原型运行相机图，正常生成三球，不含界面 / Current V5 gameplay camera capture with three normally spawned targets, excluding UI |
| Assets/StudioArtPack/SprayPrefabs | 6 喷漆预制体 / Six spray prefabs |
| Assets/StudioArtPack/Menu | 新菜单背景与 6 图形图集 / New menu background and six-element UI atlas |
| Assets/StudioArtPack/PaintBlaster/Assembled | blaster-m 喷漆枪组装、枪口粒子与独立预览场景 / Assembled blaster-m, muzzle particles and independent preview scene |
| Assets/StudioArtPack/PaintBlaster/Prefabs | 2 颜料罐、2 三维颜料弹视觉、8 静态喷漆 Sprite / Two reservoirs, two 3D paint-projectile visuals, eight static paint sprites |
| Assets/StudioArtPack/BalloonPopV2 | 3 橡胶碎片粒子颜色、共用纹理和材质 / Three latex-particle colors sharing atlas and material |
| Assets/Localization | 25 键中日英界面文案，无运行时切换 / 25 Chinese/Japanese/English UI keys; runtime switching pending |

下载包保留 colormap 与模型关系；smoke.fbx 是网格，不是粒子系统。音效已由用户接入 Balloon.cs；不同预制体是否赋值仍需检查。没有因内容相似删除任何运行素材。
Preserve palette/model associations; smoke.fbx is a mesh, not particles. The learner has added audio playback to Balloon.cs; inspect each prefab's assignment separately. No runtime art was removed based on visual similarity.

生成包说明：[工作室](STUDIO_ART_PACK.md)、[喷漆](SPRAY_ASSETS.md)、[菜单](MENU_ASSETS.md)。
Generated-pack details: studio, spray, and menu documents linked above.

早期墙面与气球参考图使用内置 imagegen 生成；下面保留原始提示词。旧概念图和首版喷溅来自前序迭代，缺少原始提示词，不补写为已知。新菜单和喷漆提示词见对应说明。
The earlier wall/balloon reference art used built-in imagegen; original prompts follow. Older concepts and the first splat atlas lack recorded prompts. New menu/spray prompts are in their respective documents.

以下保留实际英文提示词用于复现；中文用途描述见上表。生成结果不保证逐像素复现。
The exact English prompts below support reproduction; Chinese descriptions appear above. Generation is not pixel-deterministic.

### Wall prompt / 墙面提示词
```text
Use case: stylized-concept. Asset type: production game wall albedo texture for BalloonShooter, a cozy desktop balloon popping graffiti studio. Create a flat front-facing rectangular warm cream plaster wall painting, landscape 3:2. Very subtle paper/plaster grain. Sparse hand painted muted teal, coral and ochre brush swashes confined to outermost 15% corners and edges; central 70% almost blank cream for clear moving balloon targets. Low saturation and low contrast. No balloons, no strings, no text, no frame, no room, no perspective, no shadows, no lighting baked into texture, no watermark. Opaque background. This is a directly usable wall texture, not a room concept.
```
### Balloon prompt / 气球提示词
```text
Use case: stylized-concept. Asset type: balloon visual development reference sheet for a cozy Unity 3D graffiti balloon-popping game, not a runtime sprite atlas. Landscape sheet, warm cream plain background. Exactly three isolated balloons equally spaced in a single row: coral red, deep teal, muted indigo. Each balloon has instantly recognizable rounded pear silhouette, a visible small tied triangular knot, one short gently curved charcoal string, satin rubber surface with broad soft off-white highlight on upper left and clear darker edge. Balloon body occupies 65% of sheet height, short string 15%. Hand-painted polished stylized 3D aesthetic, rounded friendly shapes. No paint splats, no room, no props, no letters, no text, no grids, no watermark, no ground shadow. Clear silhouette and warm-cool contrast usable as modeling and material reference.
```
