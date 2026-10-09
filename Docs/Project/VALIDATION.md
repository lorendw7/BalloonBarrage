# 验证记录 / Validation record
更新 / Updated: 2026-10-09 (Asia/Tokyo)

## 2026-10-09 Qpic 与展示计划 / Qpic and showcase planning

- 官网中日英加入作者的 Qpic 成员身份、Qpic 与九大祭官网链接，以及用户确认的 2026-10-31 计划展示日期。展位和时段保留待确认状态。开发路线收敛为 90 秒单人试玩，目标 10/27 展示候选包；目标不是完成声明。
  All site languages identify the creator as a Qpic member, link Qpic/the official festival site, and give the user-confirmed planned showcase date of October 31. Booth/hours remain pending. The roadmap targets a 90-second solo demo and an October 27 candidate, without marking those features complete.
- 三语构建与五项现有网站测试通过；在线构建查询的公开游戏包为空。静态审计通过：195 个唯一资源 GUID、14 份文档，无本轮 Unity 资源移动。
  The trilingual build and five existing website tests pass. The online build found no public game package. Static auditing passes with 195 unique asset GUIDs and fourteen documents; no Unity assets were moved.
- 本轮未修改学习者核心、场景或预制体，保存代码仍为单球、单击发射。尚未完成展示版玩法、Windows 构建或现场电脑试玩；教学代码须由学习者输入并运行验收。
  Learner gameplay, scenes and prefabs are unchanged. Saved code still has a single target and click firing. The exhibition gameplay, Windows build and exhibition-PC playtests remain unfinished; lesson code needs learner entry and runtime acceptance.

## 2026-10-08 桌面官网检查 / Desktop website checks

- [GitHub Pages 发布](https://github.com/lorendw7/BalloonBarrage/actions/runs/37793811426) 构建、测试与部署全部成功；[线上官网](https://lorendw7.github.io/BalloonBarrage/) 的三个语言页面、样式、脚本、图标、发布状态和四张图片共 11 个资源返回 HTTP 200。
  The [Pages workflow](https://github.com/lorendw7/BalloonBarrage/actions/runs/37793811426) built, tested, and deployed successfully. Eleven [live-site](https://lorendw7.github.io/BalloonBarrage/) resources return HTTP 200: all language pages, styles, script, favicon, release snapshot, and four artworks.
- 浏览器实测中文→日文→英文→中文，英文大图打开/关闭、中文下载区“试玩包准备中”均正确；桌面画面及 390×844 小屏检查通过，中英文无横向溢出，未观察到浏览器错误/警告。没有下载或测试游戏二进制。
  Browser checks passed for Chinese → Japanese → English → Chinese, the English artwork dialog, and the Chinese "demo pending" panel. Desktop and 390×844 layouts were inspected, with no horizontal overflow in Chinese/English and no observed browser errors/warnings. No game binary was downloaded or tested.
- Website/ 构建中日英三个静态页面，复用四张原始美术；五项 Node 测试通过，覆盖翻译键、真实/空/失败下载状态、链接与原图字节一致性。公开 Release 查询为空，未创建游戏包。
  Website/ builds three static language pages from four canonical artworks. Five Node tests pass for translation keys, ready/empty/failure download states, local links, and exact asset bytes. The public release list is empty; no game package was created.
- 静态审计通过：195 个唯一元数据 GUID、14 份文档；67 个图片/音效/模型未发现相同内容的重复。官网输出与本地检查文件被忽略，Unity 原图和 .meta 不移动。
  Static audits pass with 195 unique metadata GUIDs and fourteen documents; 67 art/audio/model files contain no exact duplicates. Website output and local checks are ignored; Unity source assets and metadata are not moved.
- 当前一个框架源文件与八个玩法源文件使用 Unity 编译器独立编译通过；CS0649 是 Inspector 赋值字段警告。这不是 Unity Test Runner、完整导入、试玩或 Windows 构建验证。
  One foundation and eight gameplay sources compile independently with Unity's compiler. CS0649 warnings concern Inspector-assigned fields. This is not Unity Test Runner, full import, playtesting, or Windows build validation.
- 保存的生成参数接线、延迟、入场和碰撞体调整随本次提交保留；本轮没有代写核心脚本。以下较早框架检查的“尚未接入”已被此检查点取代。桌面单人优先，主流程和发行包仍待实现/验收；既有渲染引用问题仍保留。
  This commit preserves saved settings wiring, delay/entrance work and collider changes without rewriting core scripts. Earlier "not integrated" notes are superseded by this checkpoint. Desktop solo is first; full flow and a distributable build still need implementation/acceptance. Inherited rendering-reference issues remain.

## 2026-10-08 框架检查 / Foundation checks

- 参数程序集、现有核心脚本、LearningWorkbench 编辑器工具及三个 NUnit 测试源文件，使用 Unity 附带编译器和真实程序集引用分别编译通过。
  The settings assembly, existing core scripts, LearningWorkbench, and three NUnit test sources compile separately with Unity's bundled compiler and actual assembly references.
- 静态检查通过：194 个唯一资源元数据 GUID、13 份文档；新增文件元数据完整。Assets/Scripts、Scenes、Prefabs 无本轮修改。
  Static checks pass with 194 unique metadata GUIDs and thirteen documents; new metadata is complete. This change does not modify Assets/Scripts, Scenes, or Prefabs.
- 尚未在 Unity Test Runner 执行新测试或试玩辅助窗口，编译测试源文件不等于测试运行通过。生成配置未接入核心，延迟/入场逻辑仍由学习者实现。此前渲染设置引用问题仍待处理。
  New tests have not run in Unity Test Runner, and the workbench has not been exercised in the editor. Compiling test sources is not a passing test run. Settings are not integrated; delay/entrance remain learner work. Inherited rendering-reference issues remain outstanding.

## 2026-10-06 射击学习检查点 / Shooting learning checkpoint

后续更新：学习者已移除提前点击门控并补充引用保护 return；当前源文件重新独立编译通过，元数据检查通过。场景枪局部位置保存为 (0.55, -0.35, 1)，Rotation 三轴约 0.65，Scale 仍继承 (1, 1, 1)。保留该场景改动，提醒将缩放值填到 Scale；持续瞄准和镜头构图尚需试玩。
Follow-up: the learner removed the early click gate and added the guard return; current runtime sources compile again and metadata checks pass. Saved gun-local position is (0.55, -0.35, 1), Rotation is approximately 0.65 on each axis, and Scale remains inherited (1, 1, 1). Preserve the scene edit and advise entering scaling values under Scale. Continuous aim and framing still need playtesting.

下列重复门控待修条目记录的是本日较早检查点，并非最新代码状态。
The unresolved click-gate entries below describe the earlier checkpoint today, not the latest code state.

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
