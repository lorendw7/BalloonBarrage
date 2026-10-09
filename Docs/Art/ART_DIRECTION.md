# 美术规范 / Art direction

当前以官网暖光工作室概念为方向：奶油灰墙、胡桃木色结构、深青绿与少量赭红/黄铜，使用真实受光材质、木纹、微水泥和柔和阴影。保留既有涂鸦，避免大面积浅紫和糖果色平涂。2026-10-09 用户明确要求调整相机与枪以欣赏场景；当前 V3 参数见 [工作室美术包](STUDIO_ART_PACK.md)。新版玩法参考 [游戏设计](../Design/GAME_DESIGN.md)。
Current direction follows the website's warm studio concept: cream-grey plaster, walnut structure, deep teal and restrained rust/brass, with physical materials and soft shadows. Preserve graffiti. The user explicitly requested camera/gun reframing; current V3 settings are in the studio guide.

2026-10-09 用户要求增加色彩、层次与卡通感：V2 保留蜂蜜框架与原涂鸦，以更平整的卡通色块替代大面积重复木纹；侧墙与道具采用薄荷、淡紫、青绿、珊瑚和黄色，并增加圆角与边缘装饰。说明见 [工作室美术包](STUDIO_ART_PACK.md)。
The requested V2 pass adds richer cartoon colors and layers, preserving honey framing and graffiti while simplifying broad wood patterns. Mint/lavender walls, teal/coral/yellow props, rounded forms and peripheral decor follow the website concept.

## 可读性 / Readability
视觉优先级：准星与气球 → 击破反馈 → HUD → 环境。气球保留梨形、结口、短绳和宽高光，不能与静态涂鸦圆点混淆。密集波次先减少背景局部干扰，不把气球缩成无法瞄准的小点。
Priority: reticles/targets, hit feedback, HUD, environment. Preserve pear silhouettes, knots, short strings, broad highlights; distinguish targets from painted dots. Reduce local background competition before shrinking targets beyond readability.

固定 16:9 相机使用 V3 的 FOV 45° 与略后移构图；Scene 缩放不等于 Game 构图。UI 用屏幕 Canvas 与锚点，适配 720p、1080p、手机横屏和安全区。密集目标的最小尺寸需用真实手柄与触控实测。
Use V3's 45-degree 16:9 framing; Scene zoom is not Game composition. Use screen-space UI anchors and validate future input-device readability.

## 配色 / Palette
奶油底 #F4E6D0，珊瑚 #FF6B6B，青绿 #3EC6C0，黄 #FFD166，靛蓝 #5B5F97，炭灰文字 #30343F。双人准星同时使用颜色、形状和 P1/P2 标号。
Cream, coral, teal, yellow, indigo, charcoal form the palette. Co-op reticles combine color, shape, and P1/P2 labels.

## 环境和效果 / Environment and effects
木框、道具留在边缘；绳子使用局部坐标对准结口，不带碰撞体。贴花略离墙面避免闪烁，不能依赖 Order in Layer 穿透三维墙。喷漆/粒子设预算，背景噪声不能无限累计。
Keep frames/props peripheral; use local-coordinate strings aligned to knots without colliders. Offset decals slightly from surfaces; sorting order does not bypass 3D walls. Cap splats/particles and background clutter.

单个主光塑形，环境光和无阴影补光提亮；避免每个气球实时阴影。提供降低闪光、震屏、震动与音效密度的设置，连锁反馈不做整屏高频白闪。
Use a key light with ambient/shadowless fill; avoid per-balloon realtime shadows. Offer reduced flashes, shake, rumble, and sound density; no high-frequency full-screen white flashes.

## 菜单 / Menus
背景中间留白，文字用 Unity 文本而非烘焙图片。开始/继续可用珊瑚主按钮，设置用青绿，返回用奶油。暂停与结算复用纸张面板。手机隐藏双人入口；桌面双人加入页预留两个设备卡位。
Keep background centers quiet and render localized text in Unity. Coral primary, teal settings, cream back; reuse paper panels for pause/results. Hide co-op entry on mobile; reserve two device cards in desktop join UI.

现有菜单资源不含玩家卡、升级卡插画、独立准星和按钮点击逻辑，这些按功能开发逐步补齐，不称为完整 UI。
Current menu art does not include player cards, upgrade illustrations, reticles, or button logic. Add them with features rather than claiming a complete UI.
