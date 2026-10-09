# BalloonBarrage 设计 / Game design
更新 / Updated: 2026-10-09. 本文取代旧节奏游戏计划，是当前唯一玩法规范。
This is the current specification, replacing the former rhythm-game plan.

## 已确认与提案 / Confirmed scope and proposals
已确认：大量气球、爽快射击、肉鸽变化；桌面单人/本地同屏双人，键鼠+手柄或双手柄；暖色工作室与鲜艳涂鸦；用户亲自写核心。桌面优先，Windows 首发，手机后置，暂不走应用商店。
Confirmed: balloon hordes, satisfying shooting, roguelite variety, desktop solo/co-op with paired controllers, cozy graffiti, learner-owned core. Desktop first, Windows first release; mobile deferred, no app store currently planned.

开发顺序为 Windows 单人核心、完整桌面试玩、手柄和本地双人。中日英官网复用项目美术，通过 Pages 发布；游戏 ZIP 用 Releases 分发。教学与验收见 [路线](ROADMAP.md)，发行见 [官网与桌面发行](../Project/WEBSITE_AND_RELEASES.md)。
Develop Windows solo, a complete demo, then controllers/co-op. A trilingual site reuses project art on Pages; Releases distributes ZIPs. See the [roadmap](ROADMAP.md) and [distribution guide](../Project/WEBSITE_AND_RELEASES.md).

## 九大祭展示版本 / Kyudai-sai demonstration

用户于 2026-10-09 确认自己是 [Qpic](https://www.qpic.jp/) 成员，计划 10 月 31 日展示本作。[第 79 回九大祭官方日程](https://kyudaisai.jp/79th/) 为 2026-10-31 至 11-01。近期按 10 月 27 日展示候选包倒排，具体展位和时段待确认。官网中日英介绍作者与 Qpic 的关系并提供链接。
On October 9 the creator confirmed Qpic membership and an October 31 showcase. The official festival dates are October 31–November 1. Target a demo candidate on October 27; booth and hours remain pending. All three website languages link Qpic and describe the creator's membership.

展示版先做固定靶场、90 秒计分挑战：日文开始/操作提示 → 鼠标瞄准、按住连射、多气球补充 → 时间到停止生成与计分 → 显示分数 → 一键重开。支持暂停/返回标题/退出；打包后离线可玩。90 秒是初始试玩参数，可按现场轮换调整。
The exhibition build is a fixed-gallery, 90-second score challenge: Japanese start/instructions → mouse aim, held fire and replenishing targets → time up stops spawning and scoring → results → quick retry. Include pause, return to title and quit, with offline packaged play. Ninety seconds is an initial playtest setting.

展示前必需：分数和计时每局复位；结束时清理飞行弹丸和目标；日文字体无缺字；展示电脑上连续十局和 30 分钟运行无卡死或持续对象增长。波次/漏球失败、强化、对象池、语言切换、手柄/双人和漂浮细节移至展示后；若性能实测不达标，先降低同时目标与特效上限，再决定是否提前引入对象池。排期和逐课验收维护在 [路线](ROADMAP.md)。
Before the exhibition: reset score/time each round, clear projectiles/targets at the end, verify Japanese glyphs, and run ten consecutive rounds plus a 30-minute soak on the exhibition PC without lockups or growing object counts. Defer waves/escape failure, upgrades, pooling, language switching, controllers/co-op and float polish. Reduce target/effect caps first if profiling misses the target; bring pooling forward only if measurements require it. See the roadmap for dates and lesson acceptance.

以下局长、胜负规则、强化和性能预算为展示后版本提案，待试玩修订。不承诺联网、双鼠标或移动双人。音乐服务射击，不强制按拍开火。
Run length, outcomes, upgrades, and performance budgets below are post-showcase proposals to playtest. Online, two independent mice, and mobile co-op are out of scope. Music supports shooting without enforcing timing.

## 展示后核心循环 / Post-showcase core loop
菜单 → 人数/设备加入 → 波次射击 → 暂停战斗、三选一强化 → 更密集波次 → 精英气球 → 结算/再来一局。
Menu → player/device join → wave shooting → pause for one-of-three upgrade → denser waves → elite balloon → results/retry.

先做固定相机靶场，无角色移动或分屏。首版 5 波约 3–5 分钟，之后扩展到约 10 分钟。爽感来自连续击破、穿透和连锁组合，不靠无限堆对象。
Begin with a fixed-camera gallery, no avatar movement or split screen. Start with five waves over 3–5 minutes, later ten-minute runs. Satisfaction comes from rapid pops, piercing, and chains—not unlimited entities.

## 气球和胜负 / Targets and outcomes
普通球一击爆，护甲球多次命中，分裂球生成少量小球，连锁球触发邻近爆破，精英球使用大轮廓与弱点。先实现普通球，再逐类加入。
Normal targets pop in one hit; armored targets take several; splitters spawn a few smaller targets; chain targets burst neighbors; elites use large silhouettes and weak points. Add types incrementally.

提案：漏球积累团队共享压力，满值失败；清完最终波胜利。另留无失败练习模式。双人无友伤、不抢经验；个人命中统计与团队总分并存。
Proposal: escaped targets fill shared pressure; a full meter loses, final-wave completion wins. Keep no-fail practice. No friendly fire or XP competition; track individual contribution and shared score.

## 双人输入 / Co-op input
界面目标语言：简体中文、日文、英文。三语文案资源已准备，运行时切换与字体覆盖尚待实现。
UI targets Simplified Chinese, Japanese, and English. Text resources are prepared; runtime switching and font coverage remain pending.
每人独立编号、设备、准星、射速和升级。P1 珊瑚圆环，P2 青色菱形，同时标号，不能只靠颜色。
Each player owns ID, devices, reticle, cadence, and upgrades. P1 has a coral ring, P2 a teal diamond; number both rather than relying on color alone.

| 操作 / Action | 键鼠 / Keyboard-mouse | 手柄 / Gamepad | 手机单人 / Mobile solo |
|---|---|---|---|
| 瞄准 / Aim | 鼠标位置 / Cursor | 右摇杆移动准星 / Right-stick reticle | 拖动瞄准区 / Drag aim zone |
| 射击 / Fire | 按住左键 / Hold left button | 按住 RT/R2 / Hold trigger | 按住瞄准区自动射击 / Hold aim zone to fire |
| 技能 / Skill | 空格 / Space | 面部按钮 / Face button | 独立技能按钮 / Skill button |
| 暂停 / Pause | Esc | Start/Menu | 屏幕按钮 / UI button |

计划使用 PlayerInput/设备配对，保持共享相机。单人 Mouse.current 已在 PlayerShooter，不在气球；双人时按玩家配对，不用全局 Gamepad.current 区分两人。断线暂停并提示重连或继续单人，不自动串线。
Plan PlayerInput pairing with a shared camera. Solo Mouse.current is already in PlayerShooter, not balloons; co-op needs per-player devices, not global Gamepad.current. Pause on disconnect with reconnect/solo choices without cross-control.

升级时暂停，两人各自选择后继续；公共暂停菜单仅由发起者导航。手机 UI 触点不触发场景开火，处理触点 ID、安全区与横屏布局，不实现移动双人。
Pause for upgrades until both choose; the initiator owns shared pause navigation. Mobile UI touches must not shoot the scene. Handle touch IDs, safe areas, and landscape; no mobile co-op.

## 肉鸽强化 / Roguelite upgrades
首批 6–8 项：射速、散射、穿透、连锁、范围爆破、冻结、暴击、护盾。设等级上限、触发规则与连锁终止条件，候选池过滤无效选项。
Start with 6–8 upgrades: cadence, spread, piercing, chains, area burst, freeze, critical hits, shields. Define caps, triggers, chain termination, and eligible offers.

局内强化和分数重开清零；局外首版仅保存设置和最佳成绩，后续可做横向武器解锁。记录随机种子便于排错，不承诺跨版本逐帧确定性。
Reset run stats on retry; initially persist settings and best scores only, with sidegrade unlocks later. Record random seeds for debugging without cross-version deterministic promises.

## 未来代码分工 / Planned responsibilities
- PlayerSession/Input：加入、设备、暂停、准星 / Join, devices, pause, reticles.
- Weapon/HitResolver：射速、命中、玩家归属 / Cadence, hits, attribution.
- Balloon/Pool：生命、一次结算、复位复用 / Health, single resolution, reset/reuse.
- WaveDirector：波次预算、存活上限、出生区 / Wave budget, active caps, spawn area.
- RunState/Upgrade：流程、压力、强化 / Flow, pressure, upgrades.
- Feedback：音效、粒子、喷漆、震动配额 / Audio, particle, splat, rumble budgets.

这不是已新增脚本。对象池复用时必须重置现有缩放、碰撞开关、isPoping 等状态；同帧两人击中同球只结算一次。
These classes are not implemented. Pooling must reset scale, collider, isPoping, and other state; simultaneous hits resolve only once.

## 性能与验收 / Performance and acceptance
PC 按 30→60→100 个同时可交互目标逐级测试；手机先测 15→30。这些是试验预算，不是实测容量。一局数百次击破不要求同时数百个高成本对象。
Measure desktop tiers of 30, 60, 100 active targets; begin mobile at 15–30. These are trial budgets, not measured capacity. Hundreds of pops per run need not mean hundreds of simultaneous expensive objects.

计划对象池、按玩家射击检测、音效并发上限、喷漆上限和粒子降级。绳子/装饰不挂碰撞体，默认不为每球做实时阴影。PC 1080p/60 FPS 是目标；手机选定真机再定预算，每级记录 CPU/GPU 时间和 GC。
Plan pooling, per-player shot queries, audio/decal caps, and scalable particles. No string/decoration colliders or default per-target realtime shadows. Target PC 1080p/60 FPS; define mobile budgets on real devices and measure CPU/GPU time and allocations.

- [ ] 单人、键鼠+手柄、双手柄无串线 / No cross-control in all input combinations.
- [ ] 暂停、断线、升级恢复正确，无菜单穿透 / Correct recovery and no UI click-through.
- [ ] 同球只结算一次，连锁有终止 / Single resolution and bounded chains.
- [ ] 重开清空状态，连续复用无残留 / Clean retry and pooled state.
- [ ] Windows 完整 ZIP 与真实下载入口 / Complete Windows ZIP and genuine downloads.
- [ ] 手机后置，不阻塞桌面发行 / Mobile deferred, not a desktop release gate.

参考 / Reference: [Unity PlayerInputManager](https://docs.unity.cn/Packages/com.unity.inputsystem%401.9/manual/PlayerInputManager.html). 项目锁定 1.19.0，实际实现以随包文档核对 API。Verify APIs against the installed 1.19.0 package during implementation.
