# 教学与开发路线 / Learning and development roadmap

更新 / Updated: 2026-10-09 (Asia/Tokyo).

## 方向 / Direction

桌面优先，Windows 键鼠单人先完成，再做手柄和本地同屏双人。暂不走应用商店；官网用 GitHub Pages，游戏包用 GitHub Releases。手机后置，不是桌面发行门槛。
Desktop first: Windows keyboard/mouse solo, then controllers and shared-screen co-op. No app-store release is planned; use Pages for the site and Releases for builds. Mobile is deferred and is not a desktop release gate.

## 10 月 31 日展示目标 / October 31 showcase

作者确认是 [Qpic](https://www.qpic.jp/) 成员，并计划在 **2026-10-31** 展示气球弹幕。[第 79 回九大祭](https://kyudaisai.jp/79th/) 官方活动日期为 10/31–11/01；本作具体展位与时段待确认。以下为按截止日期倒排的工作目标，不是已经完成的功能或固定课时承诺。
The creator confirmed Qpic membership and plans to exhibit BalloonBarrage on **October 31, 2026**. The festival runs October 31–November 1; this game's booth/hours are pending. The schedule below is a delivery target, not a claim of implemented features or guaranteed lesson duration.

**最小展示版：日文开始/操作提示 → 90 秒鼠标射击 → 分数结算 → 一键重开。** 同时具备多球补充、按住连射、暂停/退出、每局清零和离线 Windows 包。90 秒可按试玩反馈调整。先在现有场景完成闭环，再调整美术，不新增关卡或更换引擎。
**Minimum showcase: Japanese start/instructions → 90 seconds of mouse shooting → score/results → quick retry.** Include replenishing targets, held fire, pause/quit, clean resets, and an offline Windows package. Tune round length after playtests; complete the loop in the existing scene before polishing art.

| 日期 / Dates | 交付目标 / Target | 通过条件 / Acceptance |
|---|---|---|
| 10/09–10/12 | 多球与连射 / Multiple targets and held fire | 先 3 球验收，再试 8–12 球；数量不超上限，松手停止生成子弹 / Validate 3 targets, then trial 8–12; obey caps and stop new shots on release |
| 10/13–10/16 | 完整一局 / Complete round | 90 秒结束；停止生成/计分；清理目标与弹丸；重开分数和计时归零 / Time up stops spawning/scoring; clean targets/projectiles and reset score/time on retry |
| 10/17–10/20 | 现场操作与首次 Windows 包 / Exhibition UI and first Windows build | 日文开始、说明、暂停/退出和结果；脱离 Unity 可玩一局 / Japanese start/help/pause/quit/results; one complete round outside Unity |
| 10/21–10/24 | 试玩和性能修复 / Playtests and performance fixes | 找 Qpic 同伴试玩；初见能理解操作；现场电脑连续十局、30 分钟稳定 / Creator arranges Qpic playtests; understandable controls, ten clean rounds and a stable 30-minute soak |
| 10/25–10/27 | 展示候选包 / Showcase candidate | 完整 ZIP、版本/操作/已知问题与校验值；离线启动；日文官网入口 / Complete ZIP, version/controls/issues/checksum, offline launch and Japanese website entry |
| 10/28–10/30 | 现场电脑验证和备份 / Exhibition-PC checks and backup | 仅修阻塞问题；确认音量、窗口、输入、退出、备用 ZIP / Fix blockers only; verify audio/window/input/quit and retain a backup ZIP |
| 10/31 | 展示 / Showcase | 使用已验证包，收集试玩反馈 / Use the verified package and collect feedback |

若进度落后，优先减少目标数量和特效，保留计时、结算、重开与稳定运行。波次、漏球失败、肉鸽强化、语言切换、手柄/双人、漂浮绳子和对象池默认在展示后；性能实测需要时再提前优化。官网已提供三语，游戏内展示版先保证日文可读。
If behind, reduce target/effect counts while preserving timer/results/retry and stable execution. Defer waves, escape failure, upgrades, language switching, controllers/co-op, float/string polish and pooling unless profiling makes optimization necessary. The website is trilingual; ensure Japanese readability in the exhibition build first.

## 保存代码基线 / Saved-code baseline

- 已接入：持续瞄准、单击发射、飞行颜料弹、路径碰撞、一次计分、音效碎片、单目标生成、配置等待与平滑入场。
  Integrated: aim, click firing, flying paint, swept collision, single scoring, pop audio/fragments, one target, settings delay, smooth entrance.
- 胶囊已启用，两个旧球形体关闭。漂浮有代码但未挂到保存预制体，后置处理。
  Capsule enabled, two old spheres disabled. Float exists but is not attached to the saved prefab; defer polish.
- 多球名单在会话提供，但保存生成器仍管理一个目标。会话示例不等于已实现。
  Multi-target tracking was supplied in chat; saved code still tracks one target. Examples are not implementation.
- 官网三语已制作；Unity 菜单事件、游戏语言切换、连射、波次、完整回合、强化和双人尚待实现。
  The website is trilingual; game menu actions, localization, held fire, waves, full rounds, upgrades, and co-op are pending.

## 展示前教学 / Pre-showcase lessons

| 顺序 / Order | 学习者核心 / Learner core | 助手准备 / Assistant foundation | 验收 / Acceptance |
|---|---|---|---|
| 1 | 多球名单与存活上限 / Target tracking and cap | 生成配置与检查 / Settings and checks | 打掉后补充，不无限生成 / Refill without unbounded growth |
| 2 | 连射与按秒射速 / Held fire and cadence | 核对已有输入 API 与接线 / Verify existing input API and wiring | 按秒限制射速，松手停止发射；基础版本不承诺低帧率精确补发 / Time-based shot limit, stop on release; basic version does not catch up missed low-FPS shots |
| 3 | 计时与回合状态 / Timer and round state | 后续准备时长配置与显示 / Prepare duration settings and display next | 时间到停止生成、开火和加分 / Stop spawning, shooting and scoring at time up |
| 4 | 结算与重开 / Results and retry | 后续准备结果界面 / Prepare results UI next | 清理弹丸/气球，静态分数显式归零，连续重开十次正常 / Clean projectiles/targets, explicitly reset static score, retry ten times |
| 5 | 日文开始、暂停与退出 / Japanese start, pause and quit | 复用现有菜单素材与文案，检查字形 / Reuse menu art/strings and check glyphs | 初见能开始；暂停不战斗；菜单点击不穿透 / First-time start, paused combat, no menu click-through |
| 6 | Windows 打包与现场试玩 / Windows package and exhibition playtest | 检查包、官网、发行说明 / Verify package, website and release notes | ZIP 完整解压，离线完成一局，现场电脑稳定 / Extract full ZIP, finish a round offline, stable exhibition-PC play |

当前约六个教学单元，可按理解程度拆分；每次完成一个可玩的增量。上一课多球示例尚未保存，本次继续讲不依赖多球的连射；10/12 前两项都要由学习者输入并试玩验收。之后先完成计时、结算、重开，不插入美术细节课。
There are about six teaching units, split as needed, each producing a playable increment. The prior multiple-target example is not saved yet; the next held-fire lesson is independent, and both need learner implementation/playtesting by October 12. Then complete timer/results/retry before art polish.

## 展示后扩展 / After the showcase

依次加入波次/预算/胜负、三选一强化、游戏内三语切换、按实测需要的对象池、手柄和本地双人。完整设计仍见 [游戏设计](GAME_DESIGN.md)，展示版计时挑战不会自动被宣称具备这些功能。
Expand into waves/budgets/outcomes, upgrade choices, in-game localization, measured pooling needs, controllers and local co-op. The full design remains in GAME_DESIGN; none of these are implied by the timed exhibition demo.

## 教学约定 / Lesson contract

助手维护配置、工具、测试、美术、文档和官网；学习者写 Assets/Scripts 核心。每课给短代码、解释变量和分支，学习者输入、Inspector 接线、试玩。除明确授权，不自动代写射击、生成、计分、强化或胜负；提交请求只发布保存代码，不补写未完成规则。
The assistant maintains foundations, tools, tests, art, docs, and the site; the learner writes Assets/Scripts core. Each lesson gives small examples, explains variables/branches, then learner entry, wiring, and playtest. Without explicit authorization, do not author gameplay; commit requests publish saved code without implementing missing rules.

## 桌面发行门槛 / Desktop release gates

- [ ] 完整单人开始、游玩、结算、重开与暂停/退出。 / Complete solo flow and pause/quit.
- [ ] Windows x64 非开发构建在目标机器验证声音、窗口、输入。 / Non-development x64 build tested on target hardware.
- [ ] 核对模型、音效、字体许可，保留署名。 / Audit licenses and preserve attribution.
- [ ] 调查既有渲染引用，验收游戏语言；官网测试不能替代游戏测试。 / Investigate render references and verify in-game language; site tests are not gameplay tests.
- [ ] ZIP 包含完整运行文件，Release 提供版本、操作、已知问题和 SHA-256。 / Complete runtime package and release notes/checksum.
- [ ] 官网只链接真实上传包，概念图注明非实机。 / Genuine package links and labelled concept art.

下一课：连射与射速；同时完成上一课多球名单，然后进入 90 秒计时与回合状态。 / Next: held fire and cadence; finish the prior multiple-target lesson, then implement the 90-second timer and round state.
