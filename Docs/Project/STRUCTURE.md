# 工程结构与迁移 / Structure and migration

## ADR-001：框架与核心分工 / Foundation and core ownership

状态 / Status: Accepted。日期 / Date: 2026-10-08。决策依据 / Decider: 用户要求助手搭框架、自己写核心 / User requests assistant-owned foundations and learner-owned core.

背景 / Context：加快教学，保留可运行原型；最新方向为桌面优先、手机后置，不整体代写核心。 / Accelerate learning while preserving the prototype; desktop now first, mobile deferred, without rewriting gameplay.

决定 / Decision：Assets/Framework/Runtime 保存无玩法副作用的数据类型，以 BalloonBarrage.Foundation 程序集隔离；Assets/Framework/Editor 保存只读辅助窗口；Assets/Framework/Tests/Editor 保存配置测试。Assets/Settings/Gameplay 保存可调资产。现有 Assets/Scripts 保持学习者核心，不移动 GUID 或自动接入玩法。
Keep side-effect-free settings types in Assets/Framework/Runtime under the BalloonBarrage.Foundation assembly, read-only tools in Editor, configuration tests in Tests/Editor, and assets in Assets/Settings/Gameplay. Preserve learner-owned Assets/Scripts and GUIDs; do not automatically integrate gameplay.

比较 / Options：仅讲解、不搭工具，改动少但重复接线慢；完整玩法框架代写，速度快但削弱理解；本次采用渐进数据/工具框架，减少机械操作而保留规则实现。 / Explanation-only minimizes changes but repeats setup; a fully authored gameplay framework is fast but reduces learner ownership; incremental data/tools remove mechanical work while leaving rules to the learner.

后果 / Consequences：新参数不会自动改变旧脚本行为；必须由学习者读取并验收。检查窗口只检查当前原型中启用的发射器和生成器及其字段，不证明命中、计时或入场正确。测试只覆盖配置默认值、负延迟保护和资产导入，不覆盖核心玩法。
New settings do not change old behavior until learner integration and acceptance. The workbench checks enabled prototype shooters/spawners and fields, not hit/timing/entrance correctness. Tests cover defaults, negative-delay protection, and asset import, not core gameplay.

下一步 / Action items：学习者已接入配置、等待和入场。按 2026-10-31 九大祭展示倒排，优先多球、连射、90 秒计分回合、日文操作和重开；10/27 展示候选包，详见 [路线](../Design/ROADMAP.md)。使用 Workbench 检查接线，在 Test Runner 的 EditMode 页运行配置测试；漂浮后置。
Settings, delay, and entrance are integrated. For the October 31 Kyudai-sai showcase, prioritize multiple targets, held fire, 90-second score rounds, Japanese controls, and retry, targeting an October 27 candidate; see the roadmap. Use Workbench and EditMode configuration tests; defer float polish.

唯一工作根目录 D:/CS/Code/BalloonBarrage；远端 BalloonBarrage。Assets、Packages、ProjectSettings 必须留在 Unity 项目根下。
Working root: D:/CS/Code/BalloonBarrage; remote: BalloonBarrage. Keep Assets, Packages, ProjectSettings at the Unity root.

## 迁移 / Migration
以实际编辑项目的已保存 Assets/Packages/ProjectSettings 为准，原仓库独有美术保留。首次清单：198 新增、59 同路径相同、21 同路径不同；冲突先备份再更新，.meta 成对迁移。
Saved editor assets/packages/settings take precedence; repository-only art is retained. Initial inventory: 198 additions, 59 identical paths, 21 differing paths. Back up conflicts before replacement and preserve metadata pairs.

首次备份位于 .local-backups/before-consolidation-20260918-233127/，含 repository-before.zip 与 editor-project-before.zip。旧文档也在备份中。旧 Unity 路径不再作为活动目录；恢复以本地快照为准。
Initial snapshots contain repository-before.zip and editor-project-before.zip, including old docs. The former Unity path is no longer active; use local snapshots for recovery.

## 分类与去重 / Classification and deduplication
Docs/Design 管设计与路线，Docs/Art 管美术与提示词，Docs/Project 管结构和验证。根 README 做导航，CREDITS 管许可。旧 PLAN、DESIGN_V2 与重复下载清单退出活动目录，避免冲突规范。
Docs/Design owns design/roadmap, Docs/Art owns art/prompts, Docs/Project owns structure/verification. README is navigation, CREDITS is provenance. Retire old conflicting plans and duplicate inventory documents.

保留下载模型包目录，避免拆散 colormap 与模型；StudioArtPack 是自包含生成包。TutorialInfo 是 Unity 附带模板，不是新建教学文档。Prefab 颜色变体与原型/预览场景用途不同，保留。
Preserve downloaded package structure and palette associations. StudioArtPack is a self-contained generated pack. TutorialInfo is Unity template content, not new lesson documentation. Color variants and prototype/preview scenes serve distinct purposes.

初次 PNG/JPG/OGG/WAV/FBX/OBJ 内容哈希扫描未发现重复；不能仅凭外观或同名删除资源，GUID 和 Sprite 切片可能不同。
Initial binary hash scan found no duplicate PNG/JPG/OGG/WAV/FBX/OBJ files. Do not delete by visual similarity or names: GUIDs and sprite slices may differ.

旧 SampleScene 与实际 PrototypeScene 使用同一个 GUID；已将旧 SampleScene 及 .meta 移至本地备份的 retired-files，消除冲突。原型 GUID 和构建入口保留。已迁到 Docs 的美术说明，其旧 .meta 也归入备份。
Old SampleScene shared the prototype GUID; its scene/metadata were moved to retired-files in the local backup. Prototype GUID/build entry are preserved. Retired art-document metadata is also archived.

新增资源先查素材索引。移动已引用资源需保留 .meta 并验证引用。不复制 Library、Logs、Temp、UserSettings、嵌套 .git，不提交本地备份或助手设置。
Check inventory before adding art. Preserve metadata and validate references when moving assets. Do not copy caches, nested Git repositories, local backups, or assistant settings into version control.

## 2026-10-03 整理 / Consolidation

再次检查旧目录，保留学习者后来保存的 Balloon.cs、PaintGunVfx.cs、PlayableBalloon.prefab 和 PrototypeScene.unity；已有脚本/场景 GUID 保留，新增脚本连同 .meta 同步。旧目录中的重复说明不重新导入，当前说明只维护在 Docs。
Synchronize the learner's later Balloon.cs, PaintGunVfx.cs, PlayableBalloon.prefab, and PrototypeScene.unity. Preserve existing script/scene GUIDs and copy new script metadata. Do not reimport duplicate old-package documentation; Docs remains the documentation source.

补齐新资源的元数据，资源生成工具使用相对项目位置输出预览。下载枪包、生成美术包、玩法、翻译、项目设置保持各自目录，避免复制模型造成重复 GUID。
Complete new-asset metadata and make preview output relative to the project. Keep downloaded guns, generated art, gameplay, localization, and settings in their respective folders rather than duplicating models and GUIDs.

根据用户要求，本次发布以整理后的目录创建新的根提交并替换远端 main 历史，清理旧计划分支。重建前的完整 Git 历史和工作文件保存在本地 `.local-backups/before-history-rebuild-20261003/`，不上传。
That history rebuild was completed on 2026-10-03. Subsequent publications append commits; do not rewrite history again. Pre-rebuild files/history stay in the ignored local backup.

以上重建记录是 2026-10-03 已完成的历史操作；后续正常追加提交，不再次重写历史。

## 2026-10-08 官网与目录职责 / Website and folder ownership

Website/ 独立保存静态官网：src/ 是页面/样式/三语，scripts/ 是构建预览，tests/ 是验证。网站说明集中到 Docs/Project/WEBSITE_AND_RELEASES.md，不创建重复计划或教学文档。
Website/ holds the static site: src/ for pages/styles/translations, scripts/ for build/preview, tests/ for verification. Its guide lives in Docs/Project/WEBSITE_AND_RELEASES.md, not another competing plan or lesson document.

Website/assets.json 只维护原图路径，构建复制到忽略的 Website/dist/assets；Assets 仍只有一份源图片，Unity .meta/GUID 不移动。Pages 只上传 Website/dist，不上传缓存、备份或整个工程。游戏 ZIP 放 Releases，不提交主干。
Website/assets.json references canonical images, copied only to ignored output. Assets retains one source image with unchanged Unity metadata/GUIDs. Upload only Website/dist to Pages, not caches/backups/the whole project. Keep game ZIPs in Releases, not main.
