# 教学与开发路线 / Learning and development roadmap

更新 / Updated: 2026-10-06 (Asia/Tokyo)。目标是让初学者亲自完成一个可玩的 BalloonBarrage 学习原型。每课只增加一个可以在 Unity 中立即验证的结果；玩法代码由学习者输入，仓库工具只整理资源、美术和文档。
The goal is a playable BalloonBarrage learning prototype authored by the learner. Every lesson adds one immediately testable Unity result. The learner types gameplay code; repository tools only organize assets, art, and documentation.

## 当前基线 / Current baseline

已接入：固定相机工作室、单个气球随机生成、玩家发射器、飞行颜料弹的 SphereCast 路径碰撞、Balloon.Hit 一次结算、计分、击破音效、立即爆裂和独立青色碎片、美术环境及喷漆枪。用户反馈发射流程可以运行；尚非完整一局游戏。
Integrated: fixed-camera studio, single-target spawning, player shooter, moving projectiles with SphereCast path checks, one-time Balloon.Hit resolution, score, audio, immediate pop with independent teal fragments, environment art, and paint gun. The learner reports firing works; this is not a complete round.

PlayerShooter 已连接 Muzzle、PaintDrop3D_Teal 和 MuzzleSpray，弹丸预制体已挂脚本，保存参数为速度 35、半径 0.08、寿命 2 秒。下一课先修复重复的点击门控、缺失引用警告后的 return，以及枪挡住画面的构图，不把点击转向描述为持续瞄准已完成。
PlayerShooter references Muzzle, PaintDrop3D_Teal, and MuzzleSpray. The projectile prefab has its script with speed 35, radius 0.08, and lifetime 2 seconds. Fix the duplicate early click gate, missing return after the reference warning, and gun framing next; continuous aiming is not complete.

尚未完成：BalloonFloat 仍为空；生成器立即替换旧球，没有等待和入场动画。Canvas 菜单、运行时三语切换、波次、对象池、双人输入、肉鸽强化和手机触控未完成。三语文案和菜单美术存在不等于玩法接入。
Pending: BalloonFloat is empty; immediate respawning has no delay or entrance. Canvas menus, runtime localization, waves, pooling, co-op, upgrades, and touch remain. Existing trilingual strings and menu art do not imply integration.

当前课 / Current lesson: **持续瞄准与镜头构图 / Continuous aim and gun framing**。

## 分阶段课程 / Course stages

| 阶段 / Stage | 课程范围 / Lessons | 可验证结果 / Exit condition | 预计课次 / Sessions |
|---|---|---|---:|
| 1. 单人 Windows 原型 / Solo Windows prototype | 下列十个教学单元 / Ten units below | 从菜单完整玩一局、结算、重开；完成基础强化与性能测试 / Complete round, retry, basic upgrades, and profiling | 10–14 |
| 2. 本地双人 / Local co-op | 玩家设备归属、双准星、分数归属、键鼠+手柄、双手柄、断线 / Device ownership, reticles, score attribution, device pairs, disconnect | 两人不串输入，同球一次结算，断线提示明确 / Independent input, single resolution, clear disconnect handling | 4–6 |
| 3. 手机单人 / Mobile solo | 触控、安全区、自适应 UI、效果预算、真机与构建 / Touch, safe area, responsive UI, budgets, device tests, builds | 横屏真机完整单人游玩 / Complete landscape-device solo run | 4–6 |

收敛到最小可玩原型后，剩余预计 **18–26 次**，每次 45–90 分钟，另需练习和排错。每周 3 次约 6–9 周授课，实际完成可能更长；不是商业发布工期。复杂单元拆课；高级连锁、特殊气球、更多武器和关卡留作原型后的扩展。
With scope narrowed to a minimal prototype, estimate **18–26 sessions** of 45–90 minutes plus practice/debugging: roughly 6–9 teaching weeks at three weekly sessions, potentially longer in practice. This is not a commercial release estimate. Split complex units; advanced chains, special balloons, extra weapons, and levels are post-prototype extensions.

## 单人原型的十个单元 / Ten solo-prototype units

1. **持续瞄准与构图 / Continuous aim and framing**：不点击也转向；空引用安全退出；枪不挡住中心。 / Aim without clicking, exit safely on missing references, keep the center clear.
2. **气球入场 / Balloon entrance**：生成延迟、平滑入场、漂浮；一次击破只安排一次生成，两个动画不争夺位置。 / Delay, entrance, float; one replacement per pop and no competing movement scripts.
3. **爽快连射 / Held fire**：按秒计时的射速、枪口效果单一输入归属、表现后坐力。 / Time-based rate, one muzzle input owner, visual recoil.
4. **颜料命中 / Paint impacts**：墙面痕迹和气球反馈；不穿墙；痕迹数量与寿命上限。 / Wall marks and balloon feedback without through-wall paint; bounded count/lifetime.
5. **多球与波次 / Targets and waves**：合法出生位置、数量、压力、胜负和一次计分。 / Valid spawn positions, counts, pressure, win/loss, single scoring.
6. **HUD 与三语 / HUD and localization**：Canvas 自适应、分数/波次、中日英字体和切换。 / Responsive Canvas score/wave display, Chinese/Japanese/English fonts and switching.
7. **完整局流程 / Round flow**：开始、暂停、计时、结算、重开；菜单不发射，暂停不生成，新局清零。 / Start, pause, timer, results, retry; no menu firing or paused spawns; reset run state.
8. **最小肉鸽 / Minimal roguelite**：波后三选一，至少射速、散射、伤害三类；新局重置。 / Post-wave choices among rate, spread, and damage; reset each run.
9. **对象池与预算 / Pooling and budgets**：回收重置；限制粒子和弹丸；记录 30/60/100 目标性能。 / Correct reuse/reset, bounded particles/projectiles, profiling 30/60/100 targets.
10. **Windows 验收 / Windows acceptance**：声音/画质/语言设置、独立构建、从菜单到重开回归。 / Audio/quality/language settings, standalone build, full menu-to-retry regression.

现阶段相机射线只选瞄准点；真实命中由颜料弹移动路径检查决定，不回退到点击即伤害，也不模拟真实流体。双人仍为本地键鼠+手柄或双手柄；手机单人优先。
The camera ray selects an aim point only; projectile path checks resolve hits. Do not revert to instant click damage or simulate physical fluids. Co-op targets local keyboard/mouse plus gamepad or dual gamepads; mobile prioritizes solo.

## 每次教学约定 / Lesson contract

每课遵循：说明目标 → 展示完整短代码 → 逐句解释 → 由学习者输入 → Inspector 连接 → Play Mode 验收 → 常见错误排查。除非用户明确要求，助手不代写 `Assets/Scripts` 下的玩法代码。
Each lesson follows: goal, short complete code, line-by-line explanation, learner typing, Inspector wiring, Play Mode verification, and common-failure checks. Gameplay scripts under `Assets/Scripts` are not authored by the assistant unless explicitly requested.

完成当前课前不提前引入双人、对象池或移动端；美术资源存在不代表玩法已经接入。联网、双鼠标和移动双人暂不排期。
Do not introduce co-op, pooling, or mobile before the current lesson passes. Existing art does not imply gameplay integration. Online play, dual mice, and mobile co-op remain out of scope.
