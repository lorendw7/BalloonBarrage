# 验证记录 / Validation record
更新 / Updated: 2026-10-09 (Asia/Tokyo)

## 2026-10-09 删除后恢复 / Recovery after re-clone

- 从历史编辑记录恢复四个场景生成器、三张原始纹理和配乐源文件，按 V1→V2→V3→V4 顺序重建 PrototypeScene。使用 Unity 6000.0.77f1 独立副本运行，退出 0，日志包含 STUDIO_RECOVERY_OK；目视检查最终 1600×900 相机图。
  Recovered four art generators, three original textures and the composition source from historical edit records. Unity 6000.0.77f1 replayed the art passes in an isolated checkout and exited 0 with STUDIO_RECOVERY_OK; the final camera render was visually inspected.
- 五段 WAV 解码、格式、整小节时长、峰值和循环端点检查通过；主配乐自动播放、循环、2D、音量 0.14。主曲 108 BPM / 71.111 秒，菜单 84 BPM / 22.857 秒，保留柔和版本。菜单与回合提示音事件仍未接线。
  All five WAV tracks pass decoding, format, full-bar duration, peak and loop-endpoint checks. Main music is 2D, autoplay, looped at volume 0.14. Menu/round audio events remain unconnected.
- 静态审计通过：381 个唯一资源 GUID，14 份文档。新增引用可解析；既有渲染设置的 11 个未解析 GUID 仍保留，完整项目引用审计不能称全部通过。没有进行人工听感验收或 Windows 构建。
  Static checks pass with 381 unique GUIDs and fourteen documents. New references resolve; eleven inherited renderer/profile references remain unresolved. Listening acceptance and Windows build validation are not claimed.
- 同步前校验当前场景及八个核心脚本哈希，保留全部原有 GUID；新建资源与元数据配对同步。用户确认曾完成多球与连射，但重新拉取版本缺少未发布实现；本次按请求恢复场景与音乐，未改写学习者核心。旧场景和验证副本留在忽略的 .local-backups。
  Scene and eight gameplay-source hashes were checked before syncing. Existing GUIDs remain; rebuilt assets and metadata travel together. The creator reports previously completing multiple targets and held fire, but their unpublished implementation is absent after re-clone. This requested scene/music recovery preserves gameplay sources. Backups and validation workspaces remain ignored.

## 2026-10-09 官网参考细化 V4 / Reference refinement V4

- 新细木纹、青绿拱形壁龛、储物搁板与无阴影局部暖光、曲面尖叶和窗边盆栽已加入。弱化大块天窗网格阴影，保持当前镜头与枪位置。新木纹使用内置 imagegen，原始提示词及成品路径已记录。
  Fine oak, arched niches, shelf lights and curved foliage refine the website-inspired scene. Current framing remains. Generated texture provenance and exact prompt are recorded.
- Unity 6000.0.77f1 编译/批处理退出 0，日志 REFERENCE_STUDIO_OK。修复叶尖浮点边界导致的无效顶点并重新上传网格，确认 39 个叶片顶点与有限包围盒；目视检查最终 1600×900 渲染。射击引用、四角枪口近裁切和新装饰无碰撞检查通过。
  Unity compilation/batch passes exited 0. Leaf-tip numeric bounds were corrected and mesh data re-uploaded; finite bounds and 39 vertices were checked. The final render was inspected; shooter references, aim depth and collider checks pass.
- 五段配乐继续原位更新，保留文件名和 GUID。采用原创钢琴式合成音、轻微力度/时序差异与低音量铺底；32 小节主曲与 8 小节菜单保持 108/84 BPM。WAV 解码检查通过，无削波，循环端点连续；没有声称完成人工试听。
  Five tracks retain filenames/GUIDs while using piano-like synthesis and restrained variations. Decoded WAV format, full-bar length, peak and loop-boundary checks pass; listening acceptance is not claimed.
- 静态审计通过：380 个唯一 GUID、14 份文档。同步前核对场景/学习者源码哈希，备份原场景；新增资源引用可解析。官网是概念图，当前模型细节与参考渲染仍有差距；Windows 包与现场电脑试玩仍待完成。
  Static auditing passes with 380 GUIDs and fourteen documents; hashes and original scene backup preserve learner work. New references resolve. The realtime scene still differs in model detail from the concept rendering; Windows/exhibition-PC acceptance remains pending.

## 2026-10-09 官网风格 V3、镜头与柔和配乐 / Atmosphere V3, framing and soft music

- 按用户反馈将浅色平涂卡通材质替换为 URP/Lit 受光材质，加入木纹/微水泥、深青绿/胡桃木色、软阴影与窗格日光，补充天窗。镜头改为 (0, 0.10, -8.8)、俯角 2°、FOV 45°；枪局部位置 (0.47, -0.51, 1.35)、统一缩放 0.50，扩大场景展示面积。原核心脚本未改写。
  Physical materials, restrained colors, textures, daylight and shadows replace flat toon shading. Wider camera framing and a lower/smaller gun expose more of the room without rewriting learner gameplay.
- Unity 6000.0.77f1 在隔离项目中编译、生成和最终光影调整均退出 0，日志 ATMOSPHERE_STUDIO_OK。已目视检查 1600×900 最终场景图；射击引用、四角枪口近裁切检查、缺失脚本检查通过，新装饰无 Collider。三球仍仅为渲染构图示意。
  Unity compile/build/refinement passes exited 0. The final camera render was inspected; shooter references, four-corner muzzle depth, missing-script and decor-collider checks passed. Temporary balloons are layout illustrations only.
- 现有五段音乐改为柔和版本并保留资源 GUID：主曲 108 BPM、71.111 秒；菜单 84 BPM、22.857 秒；短音 2.222 秒。主曲均方根电平约从 -17.1 降至 -21.5 dBFS（单声道测量），4 kHz 以上频段能量占比由约 3.81% 降至 0.082%。这些指标说明更低的电平与高频含量，不替代实际听感；尚未人工试听或完成 Play Mode 音频验收。
  Five softer tracks preserve asset GUIDs. Main/menu tempos are 108/84 BPM. Mono RMS dropped about 4.4 dB and energy above 4 kHz fell from 3.81% to 0.082%; these file measurements do not replace listening or Play Mode acceptance.
- 静态审计通过：365 个唯一资源 GUID、14 份文档。同步前核对原场景和学习者源码哈希，备份原场景；新增材质与网格引用可解析，原型无新增无法解析引用。旧 V1/V2 配色、镜头、配乐参数由本节与当前素材说明替代。Windows 构建及现场试玩仍待完成。
  Static auditing passes with 365 unique GUIDs and fourteen documents. Pre-sync hashes and scene backups preserve learner work; new references resolve. Current V3 specifications supersede older V1/V2 notes; Windows/exhibition-PC acceptance remains pending.

## 2026-10-09 卡通 V2 与日系配乐 / Cartoon V2 and game-pop music

- 按用户的新要求丰富配色与边缘装饰，参照官网 CozyGraffitiStudio 概念图。新增圆角工作台构件、画架、木箱、滴漆桶、画笔杯、书本、双音箱、绿植、旗串与地毯；保留相机、灯光、原涂鸦和学习者核心。
  The requested art pass adds richer peripheral props and rounded forms following the website concept while preserving camera, lights, graffiti and learner gameplay.
- Unity 6000.0.77f1 在缓存的隔离项目中编译与批处理执行成功，退出码 0，记录 CARTOON_STUDIO_OK；卡通 shader 无编译错误。渲染 1600×900 前后图、三球构图示意与四角瞄准图；已目视检查最终场景图。三球依旧仅为临时渲染示意，不代表已实现多球生成。
  Unity compiled and completed the cached isolated batch pass with exit 0; the toon shader has no compilation errors. Final scene images were inspected. Temporary three-balloon layout previews do not implement multi-target spawning.
- 主配乐升级为 148 BPM、32 小节、51.892 秒的原创日系游戏风合成曲，有 A/B 乐句变化；菜单曲为 104 BPM、18.462 秒。另有纯鼓循环与两个 1.622 秒短音。主曲在场景和音乐 Prefab 中均已接入，2D 循环，音量 0.16；菜单/回合短音尚未绑定事件。按用户明确要求删除旧版五段 WAV 及元数据，删除前核对已无运行素材引用。
  The new original game-pop groove loops for 32 bars at 148 BPM, with contrasting phrases. Menu, drum-only and short cues are provided. Main music is assigned in both scene and prefab at volume 0.16; future events remain unconnected. The earlier five WAVs/metadata were removed as expressly requested after checking references.
- 新 WAV 为双声道 44.1 kHz、16-bit PCM，峰值 -2.01 至 -1.31 dBFS，无削波。循环接缝有 3 毫秒平滑过渡，解码后端点连续；整小节帧数检查通过。尚未人工试听或进行 Unity Play Mode 音频验收，文件检查不替代听感判断。
  New WAV format, peak/headroom, full-bar lengths and smoothed loop endpoints were checked. Listening and Play Mode audio acceptance remain pending.
- 同步前再次核对场景和学习者源码哈希，保留原场景备份。静态审计通过：316 个唯一资源 GUID、14 份文档；新增引用解析检查通过。既有渲染引用问题与 Windows 构建/现场试玩待办仍保留。下面 V1 配乐规格属于历史记录，已被本节替代。
  Hashes were checked before syncing and the original scene was backed up. Static auditing passes with 316 unique GUIDs and fourteen documents. Inherited render-reference issues and build/playtesting tasks remain. V1 audio specifications below are historical and superseded.

## 2026-10-09 展示场景与原创配乐 / Exhibition studio and original music

- Unity 6000.0.77f1 在隔离的项目副本中完整导入并编译，ExhibitionStudioBuilder.BuildBatch 退出码 0，记录 EXHIBITION_STUDIO_OK。已保存 PrototypeScene；检查枪口/颜料弹/喷漆引用、场景无 Missing Script、装饰与枪无 Collider，以及背景 AudioSource 的 clip/Loop/2D/Play On Awake 配置。
  Unity fully imported and compiled an isolated project copy; the batch builder exited 0. Scene checks covered shooter references, zero missing scripts, collider-free decor/gun, and looping 2D background-audio wiring.
- 相机生成 1600×900 修改前后图、构图示意和四个靶区角落的瞄准图，已检查画面。枪口未穿越相机近裁切面。三球仅为临时渲染示意，已删除且未保存到场景；此检查不是多球玩法完成或 Play Mode 测试。
  Before/after, layout and four-corner aim images were rendered. Muzzle depth stays beyond the near plane; before/after, layout and upper-left aim were visually inspected. Three illustrative balloons were removed before any scene save; this is not multi-target gameplay or a Play Mode test.
- 同步前核对原型与学习者 PlayerShooter 的 SHA256，保存原场景备份；同步时保留活动编辑器已导入的新资源 GUID，并一致重映射生成的引用。学习者核心文件未改写。
  Source hashes were checked before synchronization and the original scene was backed up. Newly imported editor GUIDs were preserved with consistent reference remapping. Learner gameplay was not rewritten.
- 五段原创合成 WAV 为 44.1 kHz/双声道/16-bit PCM，峰值均约 -1.31 dBFS，无削波；主循环 16 小节、112 BPM、34.286 秒，菜单 8 小节、84 BPM、22.857 秒。循环端点跳变量低于 0.001（归一化振幅），制作源文件及指标随素材保存。这是文件级检查，尚未人工试听或验收 Unity 音频循环。
  All five synthesized WAVs use stereo 44.1 kHz 16-bit PCM, with peaks around -1.31 dBFS and no clipping. Main/menu lengths align with full bars; loop endpoint steps are below 0.001 normalized amplitude. Listening and Unity audio-loop acceptance remain pending.
- 静态审计通过：215 个唯一元数据 GUID、14 份文档；全项目既有渲染引用问题仍保留，此次成功渲染不代表其已全部修复。Windows 构建与展会电脑试玩尚未完成。旧 Qpic 计划检查中的“单击发射”描述是较早检查点；当前学习者已保存连射代码。
  同步后的新增材质/预制体 GUID 引用均可解析，原型未新增无法解析的引用；重新解码五段 WAV，格式、整小节帧数、峰值及循环边界检查通过。
  Static metadata/document auditing passed with 215 unique GUIDs and fourteen documents. Inherited rendering-reference issues remain; successful rendering does not claim all were repaired. Windows/exhibition-PC playtests remain pending. The earlier click-firing note below predates the learner's held-fire changes.

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
