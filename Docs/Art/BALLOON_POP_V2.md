# 橡胶碎片爆破 / Latex fragment pop

目录 / Folder: `Assets/StudioArtPack/BalloonPopV2`.

内置 imagegen 生成一张 2×2 透明橡胶碎片图集；编辑器工具生成三种粒子预制体，保留旧 PaintBurst，不修改玩法代码或自动替换用户绑定。
Built-in imagegen produces a transparent 2x2 latex-fragment atlas. An editor tool creates three particle prefabs, preserving PaintBurst and learner gameplay/bindings.

| 文件 / File | 用途 / Use |
|---|---|
| LatexFragments_v1.png | 四种中性灰白薄橡胶碎片，由粒子染色 / Four neutral latex fragments tinted by particles |
| LatexFragments.mat | 共享透明无光照材质 / Shared transparent unlit material |
| BalloonPop_Teal.prefab | 青色气球 / Teal balloon |
| BalloonPop_Coral.prefab | 珊瑚红气球 / Coral balloon |
| BalloonPop_Gold.prefab | 黄色气球 / Gold balloon |

每次 12 粒，寿命 0.38–0.68 秒，大小 0.22–0.4，速度 2.4–4，带重力和随机旋转，尾段淡出。2×2 纹理随机选一格并保持，不当作动画连续播放。效果为 XY 平面散开，适合当前正面相机；不是通用三维碎裂。
Twelve particles per burst, 0.38–0.68s lifetime, size 0.22–0.4, speed 2.4–4, gravity, random spin, late fade. Each particle keeps one random atlas cell. XY-plane emission suits the current front camera; this is not general 3D fracture.

把对应颜色预制体赋给实际生成气球的 Pop Effect Prefab，保留 Pop Point。根物体有 ParticleSystem，可直接兼容上一课字段。停止后自动销毁，仍兼容上一课两秒清理代码。不要做气球的子物体，否则会跟随气球提前消失。
Assign the matching prefab to the spawned balloon's Pop Effect Prefab, retaining Pop Point. Root ParticleSystem matches the lesson field. Stop action destroys the effect and remains compatible with the two-second cleanup. Keep spawned effects independent from the balloon.

当前缩小动画仍由用户代码决定，0.12 秒缩小可能仍像收缩；素材不能替代破裂瞬间的隐藏/形变逻辑。该部分留给后续教学。三语共用无文字效果，不另生成重复贴图。
Learner code still controls shrink animation; a 0.12-second shrink can look like deflation. Art does not implement instantaneous hiding/deformation. Teach that separately. All three locales share text-free effects.

验证范围：透明通道与编辑器资源生成；不宣称已通过用户相机下的实时视觉验收。没有加入火焰、烟雾、物理模型撕裂或大量刚体。
Validation covers alpha and editor asset generation, not realtime appearance under the user's camera. No fire, smoke, mesh fracture, or many rigidbodies.

## 原始提示词 / Exact prompt
Use case: stylized-concept. Production game VFX texture atlas, square, exactly 2 columns by 2 rows, FOUR isolated individual torn LATEX BALLOON FRAGMENTS, one centered in each equal cell. True transparent alpha background including generous empty gutters and margins. Each fragment is a thin flexible piece of popped rubber, irregular soft torn outline, curling edges, a subtle folded crease and soft satin highlight. Top left a curled triangular scrap, top right a folded crescent scrap, bottom left a stretched ribbon scrap, bottom right a small crumpled balloon-neck scrap. All in neutral ivory WHITE with soft light gray shading, ready to tint any color in a particle shader. Each fragment fits within central 60 percent of its cell, equally scaled. Believable thin rubber material, polished stylized 3D appearance matching a cozy hand-painted balloon game. Flat orthographic isolated sprite assets, no scene, no text, no grid, no checkerboard painted, no shadows cast on background, no glow, no smoke, no fire, no paint splashes, no intact balloons. Not a sequential animation: four independent fragment variations.
