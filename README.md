# BalloonBarrage / 气球弹幕

暖色涂鸦工作室中的卡通油漆枪射击游戏。**桌面优先，Windows 单人先行**；大量气球、波次、肉鸽强化和本地双人是计划功能，不是当前已完成内容。暂不走应用商店，GitHub Pages 托管官网，GitHub Releases 分发游戏包。
A cartoon paint-blaster game in a cozy graffiti studio. **Desktop first, starting with Windows solo.** Balloon crowds, waves, roguelite upgrades, and local co-op are planned, not complete. No store release is planned: GitHub Pages hosts the site and GitHub Releases distributes builds.

- 官网 / Website: [中文](https://lorendw7.github.io/BalloonBarrage/) · [日本語](https://lorendw7.github.io/BalloonBarrage/ja/) · [English](https://lorendw7.github.io/BalloonBarrage/en/).
- 下载 / Downloads: [GitHub Releases](https://github.com/lorendw7/BalloonBarrage/releases). 当前尚无公开 Windows 包；源代码 ZIP 不是可运行游戏。 / No public Windows build exists yet; a source ZIP is not a playable game.
- 仓库 / Repository: [lorendw7/BalloonBarrage](https://github.com/lorendw7/BalloonBarrage).

## 九大祭展示 / Kyudai-sai showcase

作者是 [Qpic（九州大学物理研究部）](https://www.qpic.jp/) 成员，计划在 **2026-10-31** 的九大祭展示本作。[第 79 回九大祭](https://kyudaisai.jp/79th/) 的官方活动日期为 10 月 31 日至 11 月 1 日；本作展位与试玩时段待确认。
The creator is a [Qpic](https://www.qpic.jp/) member and plans to exhibit this game on **October 31, 2026**. The [79th Kyudai-sai](https://kyudaisai.jp/79th/) runs October 31–November 1; this game's booth and demo hours are pending.

近期目标：90 秒一局的 Windows 键鼠单人试玩，多球、连射、计时计分、日文提示、结算重开。**10 月 27 日完成展示候选包，28–30 日验证现场电脑**。波次、肉鸽强化、手柄和双人放到展示后；完整排期与验收见 [路线](Docs/Design/ROADMAP.md)。这是开发目标，尚非已完成能力。
Near-term target: a 90-second Windows solo demo with multiple balloons, held fire, timer/score, Japanese instructions, results, and retry. **Candidate build by October 27; exhibition-PC checks October 28–30.** Waves, roguelite upgrades, controllers, and co-op follow the showcase. See the [roadmap](Docs/Design/ROADMAP.md). These are development targets, not implemented features.

## 打开项目 / Open the project

安装 Git LFS，克隆后执行 git lfs pull，使用 Unity **6000.0.77f1** 打开包含 Assets、Packages、ProjectSettings 的仓库根。本机唯一活动目录是 `D:/CS/Code/BalloonShooter`；不要打开旧的 `D:/CS/Code/Unity/BalloonShooter`。
Install Git LFS and run git lfs pull after cloning. Open the repository root containing Assets, Packages, ProjectSettings with Unity **6000.0.77f1**. The canonical local checkout is `D:/CS/Code/BalloonShooter`, not the retired path containing another Unity directory.

- 原型 / Prototype: `Assets/Scenes/PrototypeScene.unity`.
- 气球 / Balloon: `Assets/Prefabs/PlayableBalloon.prefab`.
- 油漆枪 / Paint blaster: [PaintGun_M.prefab](Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab).
- 环境预览 / Art preview: `Assets/StudioArtPack/Generated/StudioArtPreview.unity`.
- 枪预览 / Blaster preview: `Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M_Preview.unity`.
- 配置检查 / Wiring helper: Unity 菜单 / menu `Tools → BalloonBarrage → Learning Workbench`.

## 当前状态 / Current state

2026-10-08 保存代码检查：已有持续鼠标瞄准、单击发射飞行颜料弹、路径碰撞、一次击破计分、音效和独立碎片、单目标随机生成及配置等待、平滑入场。保存预制体启用胶囊并关闭两个旧球形命中体。漂浮脚本有代码，但未挂到保存玩法预制体。
Saved-source inspection on 2026-10-08 shows continuous mouse aim, click-fired moving paint, swept collision, single scoring, audio/fragments, single-target spawning with delay, and smooth entrance. The prefab enables a capsule and disables both old spheres. Float code exists but is not attached to the saved prefab.

多球课已在会话提供，保存生成器仍管理一个目标。连射、波次、完整回合、强化、对象池、菜单事件、游戏内三语切换和手柄/双人未实现。官网三语不等于游戏内语言已接入。
The multi-target lesson has been supplied in chat, but the saved spawner still tracks one target. Held fire, waves, rounds, upgrades, pooling, menu actions, in-game localization, controllers, and co-op remain pending. Website languages do not imply in-game localization.

学习者写核心，助手维护配置、工具、测试、美术、文档和官网。本轮保留学习者保存改动，不代写核心或移动 Unity GUID。Windows 构建和运行验收仍需完成。
The learner writes core gameplay; the assistant maintains foundations, tools, tests, art, docs, and the site. Preserve saved learner changes without authoring core or moving Unity GUIDs. Windows builds and runtime acceptance remain pending.

## 官网 / Website

Website/ 在 Unity Assets 之外。Node.js 22+，无需安装依赖，在仓库根运行：
Website/ is outside Unity Assets. With Node.js 22+, run at the repository root; no dependencies are needed:

```text
node Website/scripts/build.mjs
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

预览 / Preview: http://127.0.0.1:4173/. 离线构建加 --offline；无法确认发布状态时不假称没有包。规则见 [官网与桌面发行](Docs/Project/WEBSITE_AND_RELEASES.md)。
Append --offline for offline builds; an unknown release status is not reported as no release. See the [distribution guide](Docs/Project/WEBSITE_AND_RELEASES.md).

## 导航 / Navigation

- [游戏设计 / Game design](Docs/Design/GAME_DESIGN.md)
- [开发路线 / Roadmap](Docs/Design/ROADMAP.md)
- [美术规范 / Art direction](Docs/Art/ART_DIRECTION.md)
- [素材索引 / Asset catalog](Docs/Art/ASSET_CATALOG.md)
- [菜单素材 / Menu assets](Docs/Art/MENU_ASSETS.md)
- [中日英游戏文案 / Game strings](Docs/Art/LOCALIZATION.md)
- [结构与迁移 / Structure](Docs/Project/STRUCTURE.md)
- [官网与桌面发行 / Distribution](Docs/Project/WEBSITE_AND_RELEASES.md)
- [验证记录 / Validation](Docs/Project/VALIDATION.md)
- [素材署名 / Credits](CREDITS.md)

## 分类 / Layout

```text
Assets/                    Unity 素材及元数据 / Unity art and metadata
  Art/                     源模型、纹理、概念 / Models, textures, concepts
  Audio/                   音效 / Audio
  Localization/            三语游戏文案 / Game translations
  Prefabs/                 玩法预制体 / Gameplay prefabs
  Scenes/                  玩法场景 / Scenes
  Scripts/                 学习者核心 / Learner-owned core
  Framework/               助手配置、工具、测试 / Foundation, tools, tests
  Settings/                渲染、输入、Gameplay 配置 / Render, input, gameplay
  StudioArtPack/            生成美术、菜单、组装枪、预览 / Generated art, menu, blaster, previews
Docs/Design/               唯一设计与路线 / Authoritative design and roadmap
Docs/Art/                  美术索引与说明 / Art catalog and guides
Docs/Project/              工程维护与发行 / Maintenance and distribution
Tools/ProjectMaintenance/  维护工具 / Maintenance tools
Website/                   官网源码与测试 / Website source and tests
.github/workflows/         官网自动发布 / Website publication
Packages/                  Unity 依赖 / Dependencies
ProjectSettings/           Unity 配置 / Settings
```

不提交缓存、备份、网站输出或游戏二进制。游戏 ZIP 放 Releases。网站从原始素材构建，不提交第二份原图。
Exclude caches, backups, site output, and game binaries. Put ZIPs in Releases. Build website images from canonical art without another committed source copy.
