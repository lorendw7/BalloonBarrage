# 验证记录 / Validation record
更新 / Updated: 2026-10-06 (Asia/Tokyo)

## 2026-10-06 射击学习检查点 / Shooting learning checkpoint

- 静态检查通过：182 个资源元数据 GUID 无重复，13 份文档链接有效，无缺失或孤立 .meta。
  Static checks passed: 182 unique metadata GUIDs, valid links in thirteen documents, and no missing/orphaned metadata.
- 使用 Unity 附带 Roslyn 及当前工程程序集引用，独立编译 8 个运行时源文件通过；输出保留在忽略的本地检查目录。CS0649 警告涉及 Inspector 赋值字段；这不是完整 Unity 导入、Windows 构建或运行时测试。
  Eight runtime source files compile with Unity's bundled Roslyn and current project references; output stays in an ignored local check directory. CS0649 warnings concern Inspector-assigned fields. This is not a full Unity import, Windows build, or runtime test.
- 已检查保存的 Muzzle、PaintDrop3D_Teal、MuzzleSpray 引用非空，弹丸预制体已挂脚本；用户反馈发射可以运行。枪遮挡和点击才转向仍未修复，本轮保留学习者玩法代码。
  Saved muzzle, projectile, and spray references are non-null; the projectile prefab has its script. The learner reports firing works. Gun occlusion and click-only rotation remain; learner gameplay files are preserved.
- 定位了重复点击门控、引用警告后的缺失 return。按学习检查点提交，下一课由学习者修复和试玩；编译通过不代表这两个问题已修复。
  Inspection identified the duplicate click gate and missing return after the reference warning. This is a learning checkpoint; learner fixes/playtesting are next. Compilation does not imply these defects are resolved.
- diff 检查报告 Unity YAML 空字段和 Balloon.cs 的行末空白，不是编译错误，未改写学习者文件。此前 11 个渲染设置未解析引用仍待处理，本轮未重新执行完整 Unity 引用扫描。
  Diff checking reports trailing whitespace in Unity YAML empty fields and Balloon.cs, not compilation errors; learner files are unchanged. Eleven inherited unresolved rendering references remain; the complete Unity reference scan was not rerun.

## 2026-10-03 历史验证 / Historical validation

## 已验证 / Verified
- 静态检查通过：180 个资源元数据 GUID 无重复，13 份文档链接有效；没有缺失或孤立 .meta。
  Static checks passed: 180 unique asset metadata GUIDs, valid links in thirteen documents, and no missing or orphaned metadata.
- 二进制内容扫描检查 67 个 PNG/JPG/OGG/WAV/FBX/OBJ，未发现相同内容的重复文件；下载模型与调色板关系保留。
  Hash scans of 67 image/audio/model files found no exact duplicates; downloaded models retain palette associations.
- Unity 6000.0.77f1 在独立验证副本中完成当前脚本编译，三个场景及全部 Prefab 扫描的缺失 MonoBehaviour 数量为 0。
  Unity compiled current scripts in an isolated copy; all three scenes and all prefabs contain zero missing MonoBehaviour scripts.
- Balloon.cs、PaintGunVfx.cs、PlayableBalloon.prefab、PrototypeScene.unity 与学习者旧项目已保存文件内容一致，新增脚本元数据保留。
  Balloon.cs, PaintGunVfx.cs, PlayableBalloon.prefab, and PrototypeScene.unity match the learner's saved old-project files; new script metadata is preserved.
- 当时喷漆枪组装资源完整；PaintGunVfx 已存在，但组件挂载和字段接线未自动完成。此状态已由 2026-10-06 检查点取代。
  At that checkpoint assembly assets and PaintGunVfx existed, but attachment and wiring were pending. The 2026-10-06 checkpoint supersedes that status.

## 待处理 / Outstanding
严格引用扫描发现 DefaultVolumeProfile.asset 中 4 个、PC_Renderer.asset 中 7 个无法解析的 GUID。两文件 SHA-256 与旧 Unity 项目完全相同，属于迁入的既有设置，不是本轮生成菜单的引用。未自动删除或重建渲染设置，避免改变现有画面；完整引用验证因此未通过。
Strict reference scanning found four unresolved GUIDs in DefaultVolumeProfile.asset and seven in PC_Renderer.asset. Both files are byte-identical to the old Unity project, so these are inherited settings, not menu references. Rendering settings were not deleted/rebuilt; the full reference audit therefore did not pass.

尚未完成 Game 视图人工试玩、Windows 可执行构建、双人/手机功能测试。导入通过不等于这些玩法已实现。应先在新目录打开原型确认画面和音效，再独立调查上述渲染设置。
Game-view playtesting, Windows builds, and co-op/mobile tests are not completed. Import success does not imply those features exist. Open the consolidated prototype to check visuals/audio, then investigate the inherited render settings separately.

## 可重复检查 / Repeatable checks
Tools/ProjectMaintenance/Verify-Project.ps1 检查缺失/孤立 .meta、重复 GUID 与文档链接。ProjectValidation.Run 在独立批处理项目中检查全部场景、Prefab 和 YAML 引用，不重新生成美术，也不保存扫描场景；本次因上述 11 个既有引用失败，不能称完整验证通过。
The PowerShell verifier checks missing/orphaned metadata, duplicate GUIDs, and doc links. ProjectValidation.Run checks all scenes, prefabs, and YAML references in an isolated batch project without rebuilding art or saving inspected scenes. It fails on the eleven inherited references above; the complete audit has not passed.

本次发布按用户要求重建为新的首次提交。Git LFS 管理模型、图片和音效；Library、Temp、Logs、UserSettings、本地备份不在发布树中。完整旧历史与发布前文件备份保留在 `.local-backups/before-history-rebuild-20261003/`。
This publication rebuilds history as a new initial commit at the user's request. Git LFS manages models, images, and audio. Caches and local backups are excluded. Old history and pre-publication files remain in `.local-backups/before-history-rebuild-20261003/`.
