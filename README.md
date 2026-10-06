# BalloonBarrage / 气球弹幕

暖色涂鸦工作室中的爽快气球射击肉鸽。Windows 支持单人及本地同屏双人，移动端以单人为主。双人、海量气球和随机强化是下一阶段目标，不是当前已完成功能。
A cozy graffiti-studio balloon-shooting roguelite. Windows targets solo/local two-player co-op; mobile focuses on solo play. Co-op, hordes, and upgrades are planned, not implemented.

开发优先级：共用单人核心 → 手机单人 → 本地双人。Windows 场景继续作为教学验证入口，手机单人优先于双人功能。
Development priority: shared solo core → mobile solo → local co-op. The Windows scene remains the teaching testbed; mobile solo takes priority over co-op.

## 唯一工作目录 / Single working directory
使用 Unity **6000.0.77f1** 打开 `D:/CS/Code/BalloonShooter`。远端为 [lorendw7/BalloonBarrage](https://github.com/lorendw7/BalloonBarrage)，本地目录按用户确认保留原名。
Open `D:/CS/Code/BalloonShooter` with Unity **6000.0.77f1**. The remote is BalloonBarrage; the local folder intentionally retains its confirmed name.

原型入口 / Prototype: `Assets/Scenes/PrototypeScene.unity`。
美术预览 / Art preview: `Assets/StudioArtPack/Generated/StudioArtPreview.unity`。
喷漆枪预制体 / Paint gun: [PaintGun_M.prefab](Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab)。
喷漆枪预览 / Paint-gun preview: `Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M_Preview.unity`。

如果 Unity 里没有 PaintBlaster 文件夹，在 Unity Hub 从磁盘添加上面的唯一工作目录。旧路径多一层 `Unity/`，两者不是同一项目。
If PaintBlaster is missing, add the working directory above in Unity Hub. The old path contains an extra `Unity/` folder and is a separate project.

不要再使用旧路径 `D:/CS/Code/Unity/BalloonShooter`。当前所有资源和学习者代码已统一到上面的工作目录；恢复文件保存在 `.local-backups/`，不提交 Git。
Do not use the old `D:/CS/Code/Unity/BalloonShooter` path. Assets and learner code are consolidated in the working directory above; recovery snapshots stay in the ignored `.local-backups/` folder.

## 当前状态 / Current state
- 已有：玩家发射器、飞行颜料弹与路径碰撞、计分、单目标连续生成、立即爆裂与独立碎片、击破音效、工作室场景和美术。用户反馈射击流程可以运行；尚非完整一局游戏。
  Present: player shooter, moving paint projectiles with swept collision, score, single-target respawning, immediate pop with independent fragments, audio, studio scenes and art. The learner reports the shooting flow works; this is not yet a complete round.
- 枪口特效和 PlayerShooter 三个引用已连接。学习者已删除提前退出的点击门控，并在缺失引用警告后补上 return；持续瞄准需试玩验收，枪的画面遮挡仍待调整。
  Muzzle VFX and the three PlayerShooter references are wired. The learner removed the early click gate and added the missing-reference return; continuous aiming awaits playtest acceptance, and gun framing still needs adjustment.
- 菜单图形已制作，按钮事件未连接；BalloonFloat.cs 仍是空模板。新气球尚无生成等待或入场动画。
  Menu graphics exist, but button events are not wired. BalloonFloat.cs is empty; respawn delay and entrance animation are pending.
- 未完成：双人输入、连射、对象池、随机强化、完整局流程、手机适配。
  Pending: co-op, automatic fire, pooling, upgrades, run flow, and mobile adaptation.
- 玩法由用户亲自编写，教学在会话中；辅助工具只处理编辑器资源。
  The learner writes gameplay; lessons stay in conversation. Utilities prepare editor assets only.

## 导航 / Navigation
- [游戏设计 / Game design](Docs/Design/GAME_DESIGN.md)
- [开发路线 / Roadmap](Docs/Design/ROADMAP.md)
- [美术规范 / Art direction](Docs/Art/ART_DIRECTION.md)
- [素材索引 / Asset catalog](Docs/Art/ASSET_CATALOG.md)
- [菜单素材 / Menu assets](Docs/Art/MENU_ASSETS.md)
- [中日英界面资源 / Chinese, Japanese, English UI](Docs/Art/LOCALIZATION.md)
- [工程结构与迁移 / Structure and migration](Docs/Project/STRUCTURE.md)
- [验证结果与待处理项 / Validation and outstanding items](Docs/Project/VALIDATION.md)
- [作者与许可 / Credits](CREDITS.md)

## 目录 / Layout
```text
Assets/                 Unity 资源及 .meta / Assets and metadata
  Art/                  模型、纹理、材质、概念图 / Models, textures, materials, concepts
  Audio/                下载音效 / Downloaded audio
  Localization/         中日英文案资源 / Chinese, Japanese, English strings
  Prefabs/              当前玩法预制体 / Gameplay prefabs
  Scenes/               当前场景 / Scenes
  Scripts/              用户玩法代码 / Learner gameplay
  StudioArtPack/        生成美术、UI、预览和工具 / Generated art, UI, preview, tools
  Settings/             渲染和输入配置 / Rendering and input settings
  TutorialInfo/         Unity 模板资源 / Unity template resources
Docs/
  Design/               玩法和计划 / Design and roadmap
  Art/                  美术规范、清单、提示词 / Art guides, inventory, prompts
  Project/              结构和验证 / Structure and validation
Tools/ProjectMaintenance/ 工程维护工具 / Maintenance tools
Packages/               依赖 / Dependencies
ProjectSettings/        Unity 配置 / Unity settings
```
