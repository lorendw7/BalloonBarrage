# 三语界面资源 / Trilingual UI resources

目标语言：简体中文 zh-Hans、日文 ja、英文 en。资源文件为 Assets/Localization/UIStrings.json；25 个稳定键，每键三种文案。此 JSON 不是 Unity Localization 自动识别的 String Table，也没有接入运行时。后续教学实现读取，或迁入正式 String Table。
Target locales: Simplified Chinese, Japanese, English. UIStrings.json contains 25 stable keys in three languages. It is not an automatically imported Unity Localization String Table and has no runtime wiring yet. Teach a loader or migrate to formal tables later.

菜单图片不烘焙文字，三语共用同一套背景与按钮。保持键稳定，例如 menu.start；只换显示值。{0} 为数字占位符，三语必须保持一致。语言切换需保存设置并刷新当前 UI，不能只改变后续生成的文字。
Share text-free art across locales. Keep keys stable and switch values. Preserve numeric placeholders such as {0}. Later language switching must persist settings and refresh existing UI.

需导入授权清楚且覆盖中文、日文假名、汉字和拉丁字符的字体，并制作 TextMeshPro 字体及回退方案；本轮未下载字体，不能宣称已解决缺字。按钮给长文案留空间，设备断开提示允许换行，不把整句硬塞进小按钮。
Import appropriately licensed CJK/Latin fonts and configure TMP font assets/fallbacks. No font is downloaded this round, so glyph coverage is not yet verified. Leave space for longer labels and wrap device messages.

桌面显示本地双人入口，手机以单人为主；隐藏功能入口由后续代码控制，不通过删除翻译实现。文档继续中英双语，游戏文案支持三语。
Show local co-op on desktop; mobile focuses on solo. Future code controls visibility rather than removing translations. Engineering docs remain bilingual; player-facing text targets three languages.

爆破效果使用无文字粒子/颜料，三语共用。当前 PaintBurst 是碎片喷散，不是气球模型破裂模拟；接线步骤留在会话中，由用户完成。
Text-free particles and paint are shared across locales. PaintBurst is a fragment burst, not mesh-fracture simulation. Wiring is taught in conversation and completed by the learner.

本轮提升 PaintBurst 粒子可见度：寿命 0.7 秒、速度 2.2、大小 0.18，保留单次 18 粒子的预算。尚未在 Game 视图试玩，不代表接线完成；后续高密度模式需对象池与并发限制。
This round increases lifetime to 0.7 seconds, speed to 2.2, and size to 0.18, retaining 18 particles per burst. Game-view playtesting and gameplay wiring are pending; dense gameplay later needs pooling and concurrency caps.
