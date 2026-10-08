# 教学与开发路线 / Learning and development roadmap

更新 / Updated: 2026-10-08 (Asia/Tokyo).

## 方向 / Direction

桌面优先，Windows 键鼠单人先完成，再做手柄和本地同屏双人。暂不走应用商店；官网用 GitHub Pages，游戏包用 GitHub Releases。手机后置，不是桌面发行门槛。
Desktop first: Windows keyboard/mouse solo, then controllers and shared-screen co-op. No app-store release is planned; use Pages for the site and Releases for builds. Mobile is deferred and is not a desktop release gate.

## 保存代码基线 / Saved-code baseline

- 已接入：持续瞄准、单击发射、飞行颜料弹、路径碰撞、一次计分、音效碎片、单目标生成、配置等待与平滑入场。
  Integrated: aim, click firing, flying paint, swept collision, single scoring, pop audio/fragments, one target, settings delay, smooth entrance.
- 胶囊已启用，两个旧球形体关闭。漂浮有代码但未挂到保存预制体，后置处理。
  Capsule enabled, two old spheres disabled. Float exists but is not attached to the saved prefab; defer polish.
- 多球名单在会话提供，但保存生成器仍管理一个目标。会话示例不等于已实现。
  Multi-target tracking was supplied in chat; saved code still tracks one target. Examples are not implementation.
- 官网三语已制作；Unity 菜单事件、游戏语言切换、连射、波次、完整回合、强化和双人尚待实现。
  The website is trilingual; game menu actions, localization, held fire, waves, full rounds, upgrades, and co-op are pending.

## 主功能教学 / Main-feature lessons

| 顺序 / Order | 学习者核心 / Learner core | 助手准备 / Assistant foundation | 验收 / Acceptance |
|---|---|---|---|
| 1 | 多球名单与存活上限 / Target tracking and cap | 生成配置与检查 / Settings and checks | 打掉后补充，不无限生成 / Refill without unbounded growth |
| 2 | 连射与按秒射速 / Held fire and cadence | 武器配置 / Weapon settings | 帧率不改变射速 / Frame-independent cadence |
| 3 | 波次、预算、胜负 / Waves, budgets, outcomes | 波次数据 / Wave data | 生成有界，最终波能结束 / Bounded spawning, final wave ends |
| 4 | 开始、暂停、结算、重开 / Start, pause, results, retry | UI 层级与接线 / UI structure | 新局清零，暂停停止战斗 / Reset on retry, pause combat |
| 5 | 三选一强化 / Upgrade choices | 数据、图标、说明 / Data, icons, descriptions | 强化有效且重开恢复 / Effective choices, clean reset |
| 6 | 菜单与游戏中日英切换 / Menus and localization | 字体、文案、美术 / Fonts, strings, art | 完整导航与字形 / Navigation and glyph coverage |
| 7 | 回收复位与性能预算 / Reuse and budgets | 性能记录、池接口 / Profiling, pool interfaces | 无旧状态、弹丸碎片有上限 / Clean state, bounded VFX |
| 8 | Windows 完整验收 / Desktop regression | 官网与发行说明 / Website and release guide | ZIP 解压后能完整玩一局 / Extracted ZIP supports full round |
| 9 | 手柄和本地双人 / Controllers and co-op | 设备加入与双 HUD / Device join, dual HUD | 两种设备组合不串输入 / Independent device ownership |

漂浮、绳子、后坐力、喷漆细节统一留到主流程后。课程按理解程度拆分，不承诺固定课数或商业发布日期。
Defer float, strings, recoil, and paint polish until the core flow works. Split lessons according to understanding; no fixed session count or release date is promised.

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

下一课：多球和连射，先验证桌面单人；不再先做手机输入。 / Next: multiple targets and held fire for desktop solo, not touch input.
