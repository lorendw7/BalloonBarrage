# 工程结构与迁移 / Structure and migration

唯一工作根目录 D:/CS/Code/BalloonShooter；远端 BalloonBarrage。Assets、Packages、ProjectSettings 必须留在 Unity 项目根下。
Working root: D:/CS/Code/BalloonShooter; remote: BalloonBarrage. Keep Assets, Packages, ProjectSettings at the Unity root.

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
At the user's request, publish the consolidated tree as a new root commit replacing main history and remove the obsolete planning branch. Pre-rebuild Git history and working files are backed up locally under `.local-backups/before-history-rebuild-20261003/` and are not uploaded.
