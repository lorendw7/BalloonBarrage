# 官网与桌面发行 / Website and desktop distribution

更新 / Updated: 2026-10-09 (Asia/Tokyo).

## 职责 / Ownership

官网在 Website/，Unity 保持根目录。Pages 是网站，不是浏览器游戏；Windows ZIP 放 GitHub Releases，不走应用商店。官网三语不代表游戏内切换已完成。
Website/ is separate from Unity. Pages hosts a site, not a browser game; distribute Windows ZIPs through Releases, not stores. Website localization is not implemented in-game switching.

官网 / Site: https://lorendw7.github.io/BalloonBarrage/. 三语 / Languages: root 中文, ja/ 日本語, en/ English (under /BalloonBarrage/).
下载 / Releases: https://github.com/lorendw7/BalloonBarrage/releases.

## Qpic 与九大祭 / Qpic and Kyudai-sai

用户确认自己是 [Qpic](https://www.qpic.jp/) 成员、本作计划 2026-10-31 展示。三语首页的展示区提供 Qpic 与 [九大祭官网](https://kyudaisai.jp/79th/) 链接。使用作者第一人称介绍成员作品与计划展示，不声明 Qpic 官方联合开发或背书；不使用未提供的社团标志。具体展位、试玩时段待确认后补充，活动整体的 10/31–11/01 不等于本作两天均参展。
The user confirmed Qpic membership and a planned October 31 showcase. The exhibition section links to Qpic and the festival site. Use the creator's first-person voice without claiming official co-development or endorsement or adding an unprovided club logo. Add booth/hours once confirmed; the festival's two-day schedule does not establish two-day attendance for this game.

展示版目标与排期以 [路线](../Design/ROADMAP.md) 为准。多球补充与持按连射已恢复并通过 PlayMode 检查，官网操作说明与此一致。90 秒完整回合、结算与重开仍待完成，试玩包未公开。现场操作以日文为先，官网分享优先使用 [日文页](https://lorendw7.github.io/BalloonBarrage/ja/)。
The roadmap owns demo scope and dates. Multiple targets and held fire have been restored and checked in PlayMode; the website describes those controls. Complete 90-second rounds, results and retry remain unfinished, and no public demo is available. Prioritize Japanese in the exhibition UI and share the Japanese site with visitors.

## 本地构建 / Local build

Node.js 22+，无需依赖安装。先 git lfs pull，仓库根运行： / Node.js 22+, no dependencies. Fetch LFS art, run at the root:

```text
node Website/scripts/build.mjs
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

预览 http://127.0.0.1:4173。--offline 构建显示未知发布状态，保留发布列表链接，不虚构下载。dist 是忽略输出。官网无广告、统计、外部字体；大图弹窗以外的浏览、语言、下载不依赖 JavaScript。
Preview at http://127.0.0.1:4173. --offline reports unknown status while retaining release-list links. dist is ignored. No ads, analytics, external fonts; browsing/languages/downloads work without JS, except the optional image dialog.

## 素材与文案 / Assets and strings

Website/assets.json 引用五张原图：菜单背景、工作室/油漆枪概念、气球造型，以及 Assets/Art/Previews/PrototypeScene_20261010.png。新增画面在独立 Unity 副本的 PlayMode 中加载当前场景，等待正常生成三个气球后截取相机；不含界面，也没有用构图示意球冒充运行画面。构建拒绝 LFS 指针，仅复制选定图片到输出，网站目录不提交第二份原图。概念图单独标明。[素材索引](../Art/ASSET_CATALOG.md) 与 [署名](../../CREDITS.md) 是来源记录。
The manifest references five canonical images, including the October 10 prototype capture. In an isolated Unity copy, PlayMode loads the current scene and waits for three normal runtime targets before capturing the camera. It excludes UI and uses no staged layout balloons. Reject LFS pointers and copy selected images only to output; keep no duplicate source art in Website. Label concepts separately. See [catalog](../Art/ASSET_CATALOG.md) and [credits](../../CREDITS.md).

2026-10-10 官网审核：去掉重复宣传口号、编号特性/路线卡片、贴纸与倾斜描边，保留暖色工作室风格。内容围绕当前玩法、开发近况、展示计划、美术草案与真实发布状态；三语使用自然表达，素材来源保留 AI 辅助生成说明。初版 Windows 预览根目录曾因尾部分隔符返回 403，现已规范根路径，并增加实际 HTTP 检查。
The October 10 editorial pass removes repeated slogans, numbered feature/roadmap cards, stickers and tilted borders while retaining the warm studio palette. The page covers current gameplay, development notes, exhibition plans, sketches and genuine release status. All three languages use direct phrasing, with AI-assisted art disclosed in credits. Preview roots are normalized to fix Windows directory-root 403s and covered by an HTTP integration check.

Website/src/strings.mjs 保存网站三语，Assets/Localization/UIStrings.json 保存游戏三语，用途不同。网站每个语言静态预渲染，翻译测试验证相同键集和非空值；语言通过独立 URL 分享。
Website strings and game strings are separate surfaces. Render each site language statically, verify identical nonempty keys, and share independent language URLs.

## 自动发布 / Automated publication

.github/workflows/website.yml 在 main 的网站/选定美术改动、发布事件和手动触发时构建、测试，用 LFS checkout 后上传 Website/dist，发布到 github-pages。Pages 设置 Actions 发布源（build_type: workflow）。构建仅 contents: read；部署 pages: write、id-token: write。
Build/test on main site/selected-art changes, release events, or manual dispatch. LFS checkout, upload Website/dist, deploy to github-pages. Choose the Actions publishing source (build_type: workflow). Build uses contents: read; deployment pages: write and id-token: write.

附件修改后等待工作流；若未触发则手动运行 Publish official website。访客浏览器不调用 GitHub API，下载状态在构建时获取并写入无 token 的 release.json。
Wait for the workflow after asset changes; manually dispatch if necessary. Fetch releases at build time, not in visitor browsers; release.json is a public token-free snapshot.

参考 / References: [Pages workflows](https://docs.github.com/en/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages), [Pages API](https://docs.github.com/en/rest/pages/pages), [Releases API](https://docs.github.com/en/rest/releases/releases).

## 下载规则 / Download rules

- 排除草稿；必须非空、已上传 ZIP，名含 Windows、win64 或 win-x64。建议 BalloonBarrage-Windows-x64-v0.1.0.zip。 / Reject drafts; require a nonempty uploaded Windows-labelled ZIP.
- 只接受本仓库 releases/download HTTPS URL；优先稳定包，否则明确预发布。查最近 100 个发布。 / Accept this repo's HTTPS asset URLs, prefer stable then labelled prerelease, inspect latest 100.
- 无包为准备中，API 失败为无法确认；源码 ZIP 不作游戏包，不链接不存在的 latest。 / Distinguish empty from API failure; source ZIPs are not games; no nonexistent latest link.

## Windows 包 / Windows package

目前无公开包；本轮不自动创建未验收游戏 Release。完整单人实现后： / No public build exists; do not create an untested game release in this publication. After solo completion:

1. Unity Build Profiles 选 Windows x86_64、非 Development Build，包含原型场景，输出到忽略的 Builds/Windows/。 / Build non-development Windows x64 into ignored Builds/Windows/.
2. 独立解压运行，验收开始、射击、结算、重开、暂停/退出及声称的语言和设备支持。 / Extract independently and test full flow plus declared language/device support.
3. ZIP 包含 EXE、_Data、UnityPlayer.dll 和其他实际生成文件，不能只压 EXE。 / Include the executable, data, runtime DLL and all generated files.
4. 核对许可，生成 SHA-256，Release 写明状态、操作、已知问题、测试系统，不虚构最低配置。 / Audit licenses, hash the ZIP, document scope/controls/issues/tested systems without invented specs.
5. 上传规范命名 ZIP；早期试玩设 prerelease。官网重建后自动显示真实链接。 / Upload a named ZIP; use prerelease for early demos. Rebuild exposes the genuine link.

## 回滚 / Rollback

网站使用正常 revert 或重新部署已知良好提交，不重写 main。撤下问题游戏附件后刷新网站。自定义域名另行配置，本轮默认 github.io。
Revert/redeploy known good commits without rewriting main. Refresh after withdrawing broken game assets. Custom domains are separate; use default github.io now.
