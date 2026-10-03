# 喷漆枪补充素材 / Paint blaster supplements

枪体复用 Assets/Art/BlasterKit/blaster-a … blaster-r.fbx，共 18 款。本轮选用 blaster-m，已组装颜料罐、连接座、枪口和喷漆粒子。没有替换原枪体，也没有拆散下载包与 colormap.png。
The pack contains 18 downloaded BlasterKit guns. This assembly selects blaster-m and adds a reservoir, mount, nozzle and paint particles. Original gun meshes and palette associations remain intact.

新增目录 / New folder: `Assets/StudioArtPack/PaintBlaster`.

| 分类 / Category | 文件 / Files | 边界 / Scope |
|---|---|---|
| Concepts | PaintBlaster_Concept_v1.png | 配色与配件参考，不是模型或 UV / Styling reference, not mesh or UV |
| Textures | PaintFX_Atlas_v1.png | 4 格透明液态颜料：水滴、拖尾、枪口喷溅、命中斑 / Four transparent liquid-paint elements |
| Materials | Teal, Coral, Gold | 基础三维配件材质 / 3D accessory materials |
| Prefabs | PaintCanister_Teal/Coral | 独立颜料罐基础模型，无透明容器模拟 / Simple separate reservoirs, no transparent-fluid simulation |
| Prefabs | PaintDrop3D_Teal/Coral | 三维颜料弹视觉，无移动和碰撞 / 3D projectile visuals without movement/collision |
| Prefabs | PaintDrop, PaintStreak, NozzleSpurt, WetImpact，各 Teal/Coral | 8 个静态 Sprite 视觉模板，无自动动画或生命周期 / Eight static sprite visuals, no automatic animation/lifecycle |

补充模型用 Unity 基础网格组装，并非概念图级精细模型。原枪模无需复制到这个文件夹；通过 Prefab 引用原模型即可。已有喷漆墙贴花和气球橡胶碎片继续复用，不重建。
Supplemental models use Unity primitives, not concept-quality modeling. Reference original guns rather than duplicating FBXs. Reuse existing wall splats and latex-fragment pop effects.

## 接入边界 / Integration boundary
素材工具只准备美术。学习者已编写 `Assets/Scripts/PaintGunVfx.cs`，在鼠标按下时调用粒子 Play；尚待挂到枪上并连接 Muzzle Spray 字段。射击命中、颜料弹碰撞、后坐力和切枪仍待教学实现。三维弹沿本地 +Z 朝前；平面水滴/喷射图朝右，需按相机和发射方向旋转。Sprite 不等同 ParticleSystem，不能拖入气球的 Pop Effect Prefab 字段。
Art tools prepare visuals. The learner's PaintGunVfx.cs calls particle Play on mouse press; attaching it to the gun and wiring Muzzle Spray remain pending. Hit resolution, projectile collision, recoil, and switching remain lesson tasks. 3D droplets face local +Z; flat streak/spurt images point right and need alignment. Sprites are not ParticleSystems and do not fit the balloon's Pop Effect Prefab field.

后续教学：摆放玩家武器 → 触发枪口粒子 → 颜料飞行 → 命中调用气球爆破。颜料命中痕迹与气球破裂是两个效果，不互相替代。
Next lessons: place the player weapon, trigger muzzle particles, animate paint, then trigger balloon pop on hit. Paint impact and latex popping remain separate effects.

## 已组装喷漆枪 / Assembled paint gun

- `Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab`：可拖入场景的美术根预制体；嵌套引用原枪和颜料罐。 / Drop-in art prefab referencing the original gun and reservoir.
- 同目录 `PaintGun_M_Preview.unity`：独立静态预览，不替换原型场景。 / Independent static preview; does not replace the prototype.
- `Muzzle`：根物体本地 +Z 为喷射方向；原模型在包装内旋转 180°，不修改 FBX。 / Local +Z is forward; the nested source rotates 180° without changing the FBX.
- `Muzzle/MuzzleSpray`：真实 ParticleSystem，单次 6 粒，关闭 Play On Awake；编辑器选中后通过粒子预览播放。未接入开火输入。 / Real six-particle burst with Play On Awake disabled; preview using the particle controls. No firing input is wired.
- `GripPoint`：握持位置参考，不是手部绑定或 IK。 / Grip reference only, not hand rigging or IK.

所有枪体装饰不带 Collider，不阻挡既有气球点击射线。预览与组装工具可编译；正式游戏相机、手柄输入、命中与粒子触发仍需接入和运行验收。不要重复运行生成工具覆盖手工修改；工具检测到已有预制体会停止。
Decorative gun objects have no colliders and do not intercept existing balloon click rays. Assembly tooling compiles; gameplay camera, controller input, hits and particle triggering still require integration and runtime testing. The builder refuses to overwrite an existing assembled prefab.

## 来源与验证 / Provenance and validation
概念图和图集通过内置 imagegen 生成，透明通道已检查；编辑器工具生成附件，运行时视觉仍待用户相机验收。原枪模作者记录见 CREDITS.md。图片无文字，中日英共用。
Concept/atlas generated with built-in imagegen; alpha checked. Editor tooling generates accessories; runtime visuals need camera testing. Original model credits remain in CREDITS.md. Text-free art is shared across Chinese/Japanese/English.

## 原始提示词 / Exact prompts
### 概念 / Concept
Use case: stylized-concept. Asset type: concept design sheet for a cozy 3D game paint-spraying toy gun, not an actual 3D model or UV map. Landscape sheet on warm ivory paper, one large three-quarter view and one smaller clean side view of the SAME compact playful paint blaster. Chunky cream plastic body, teal grip, coral wide circular nozzle, mustard fittings, small translucent paint reservoir on top visibly filled with turquoise paint. Rounded friendly forms, soft satin highlights, tasteful paint smudges, plausible ergonomic silhouette. No military styling, no bullets, no flames, no real weapon branding. Show a short glossy turquoise paint jet and 3 detached viscous droplets exiting the large view nozzle, and a small matching paint tank accessory. Polished stylized 3D hand-painted game-art style consistent with honey-wood and cream graffiti studio. Clear negative space, no words, no letters, no arrows, no watermark. This is a design reference, not a UI screenshot.
### 特效 / Effects
Use case: stylized-concept. Asset type: production transparent game VFX sprite atlas. Square image exactly 2 columns by 2 rows of four independent WHITE neutral glossy acrylic PAINT elements for runtime tinting. True transparent alpha background, generous transparent margins and gutters. Top-left: one plump round paint droplet with small trailing tail pointing left, traveling right. Top-right: one elongated horizontal viscous paint streak with rounded leading end at right and short uneven tail at left. Bottom-left: a compact paint nozzle spurt traveling right, 3 connected small lobes and 2 detached tiny droplets, no broad cloud. Bottom-right: a frontal wet irregular paint impact splat with rounded lobes, thick glossy center and few satellite droplets, NOT sharp explosion star. Thin soft gray internal shading and white specular highlights retain liquid volume. All four completely contained in their own cells, centered, cell edges blank, no shared elements, no overlap. No colored pigment: neutral white/gray ready for tint. No text, no checkerboard, no frame, no smoke, fire, metal, balloon fragments. Polished stylized game art; these are independent VFX variants, not sequential animation.
