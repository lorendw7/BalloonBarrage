# 教学与开发路线 / Learning and development roadmap

更新 / Updated: 2026-10-03。目标是让初学者亲自完成一个可玩的 BalloonBarrage 学习原型。每课只增加一个可以在 Unity 中立即验证的结果；玩法代码由学习者输入，仓库工具只整理资源、美术和文档。
The goal is a playable BalloonBarrage learning prototype authored by the learner. Every lesson adds one immediately testable Unity result. The learner types gameplay code; repository tools only organize assets, art, and documentation.

## 当前基线 / Current baseline

已完成：固定相机工作室、单个气球随机生成、鼠标点击射线、计分、缩小消失、击破音效、美术环境、涂鸦、菜单图形、橡胶碎片素材，以及基于 `blaster-m` 的喷漆枪组装预制体。
Complete: fixed-camera studio, one-at-a-time random balloon spawning, mouse ray click, score, shrink pop, audio, environment art, graffiti, menu graphics, latex-fragment art, and an assembled `blaster-m` paint-gun prefab.

爆破粒子代码和基础预制体绑定已经从学习者保存的旧项目同步，橡胶碎片升级和视觉验收仍待完成。PaintGunVfx 脚本也已同步，下一步在枪上挂组件并连接 MuzzleSpray。
The learner's pop-particle code and basic prefab binding have been synchronized; latex-fragment upgrades and visual checks remain. PaintGunVfx is present; attach it to the gun and wire MuzzleSpray next.

尚未完成：独立玩家射击器、喷漆弹、Canvas 菜单、波次、对象池、双人输入、肉鸽强化和手机触控。
Pending: player-owned shooting, paint projectiles, Canvas menus, waves, pooling, co-op input, roguelite upgrades, and mobile touch.

当前课 / Current lesson: **1.1 按鼠标播放枪口喷漆粒子 / Play muzzle paint on mouse press**。

## 分阶段课程 / Course stages

| 阶段 / Stage | 课程范围 / Lessons | 可验证结果 / Exit condition | 预计课次 / Sessions |
|---|---|---|---:|
| 1. 喷漆射击闭环 / Paint-shooting slice | 枪口粒子、玩家射击器、射线命中、爆破特效、颜料痕迹 / Muzzle VFX, shooter, hit ray, pop VFX, paint mark | 枪口、命中点和爆破位置一致；气球不再自己读取鼠标 / Muzzle, hit and pop align; balloons no longer read input | 5 |
| 2. 一局游戏 / Playable round | Canvas 分数、开始、暂停、倒计时、结算、重开 / Canvas score, start, pause, timer, results, retry | 能从菜单完整玩完并重开一局 / Complete and retry a round | 4–6 |
| 3. 大量气球 / Balloon hordes | 多目标生成、波次、逃逸压力、对象池、反馈数量限制 / Multiple targets, waves, pressure, pooling, feedback budgets | PC 依次测试 30/60/100 个目标，无重复计分 / Test 30/60/100 targets without duplicate scoring | 5–7 |
| 4. 本地双人 / Local co-op | Input System、准星、玩家归属、键鼠+手柄、双手柄、断线 / Player input, reticles, ownership, device pairs, disconnect | 两名玩家无串线、同球只结算一次 / No cross-control; one resolution per balloon | 5–7 |
| 5. 肉鸽构筑 / Roguelite loop | 三选一、射速、散射、穿透、连锁、特殊气球、随机种子 / Upgrade choices, modifiers, special balloons, seeds | 五波内形成可辨识构筑并正常结算 / Distinct build across five waves | 6–8 |
| 6. PC 打磨 / PC polish | 三语切换、音画反馈、菜单美术、设置、构建和性能检查 / Localization, feedback, UI art, settings, build, profiling | Windows 1080p/60 FPS 目标和完整构建 / Complete Windows build targeting 1080p/60 FPS | 3–5 |
| 7. 手机单人 / Mobile solo | 触控、安全区、自适应 UI、粒子降级、真机性能 / Touch, safe area, responsive UI, VFX scaling, device profiling | 横屏真机可完整单人游玩 / Complete solo run on a landscape device | 5–8 |

剩余预计 **33–46 次**，每次约 45–90 分钟，另需自主练习和排错时间。每周 3–4 次，学习原型约需 2–4 个月；这不是商业发布工期。
Estimated remaining work: **33–46 sessions** of 45–90 minutes plus practice and debugging. At 3–4 lessons weekly, expect roughly 2–4 months for a learning prototype, not a commercial release.

## 第一阶段的五课 / Stage 1 lesson sequence

1. **枪口会喷 / Muzzle plays**：点击鼠标时调用现有 `MuzzleSpray.Play()`，只验证视觉。
2. **输入归玩家 / Player owns input**：新建 `PlayerShooter`，把射线检测从 `Balloon.Update()` 移出。
3. **命中有来源 / Explicit hit API**：`Balloon.Hit()` 负责一次结算，防止输入、分数和表现耦合。
4. **气球真爆破 / Pop particles**：实例化橡胶碎片粒子，并解释 Prefab、引用和生命周期。
5. **墙上留漆 / Paint impact**：仅在有效表面生成受数量限制的颜料痕迹，完成第一阶段验收。

阶段 1 先使用射线命中，画面可显示短促喷漆；暂不模拟真实液体弹道。后续如玩法需要，再把视觉弹丸与命中判定分离。这样先学清输入、职责和命中，再增加移动与对象池。
Stage 1 uses ray hits with a short visible paint burst rather than physical fluid ballistics. A visual projectile may be separated from hit resolution later. This teaches input, ownership, and hits before movement and pooling.

## 每次教学约定 / Lesson contract

每课遵循：说明目标 → 展示完整短代码 → 逐句解释 → 由学习者输入 → Inspector 连接 → Play Mode 验收 → 常见错误排查。除非用户明确要求，助手不代写 `Assets/Scripts` 下的玩法代码。
Each lesson follows: goal, short complete code, line-by-line explanation, learner typing, Inspector wiring, Play Mode verification, and common-failure checks. Gameplay scripts under `Assets/Scripts` are not authored by the assistant unless explicitly requested.

完成当前课前不提前引入双人、对象池或移动端；美术资源存在不代表玩法已经接入。联网、双鼠标和移动双人暂不排期。
Do not introduce co-op, pooling, or mobile before the current lesson passes. Existing art does not imply gameplay integration. Online play, dual mice, and mobile co-op remain out of scope.
