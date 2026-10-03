# 美术规范 / Art direction

延续用户认可的暖奶油墙、蜂蜜木框、鲜艳青绿/黄色/珊瑚涂鸦组合。不统一降低已调好的涂鸦透明度，不重置相机或灯光。新版玩法参考 [游戏设计](../Design/GAME_DESIGN.md)。
Preserve the approved cream wall, honey wood, and vibrant teal/yellow/coral graffiti. Do not globally dim approved splats or reset the camera/lights. See the current game design.

## 可读性 / Readability
视觉优先级：准星与气球 → 击破反馈 → HUD → 环境。气球保留梨形、结口、短绳和宽高光，不能与静态涂鸦圆点混淆。密集波次先减少背景局部干扰，不把气球缩成无法瞄准的小点。
Priority: reticles/targets, hit feedback, HUD, environment. Preserve pear silhouettes, knots, short strings, broad highlights; distinguish targets from painted dots. Reduce local background competition before shrinking targets beyond readability.

固定 16:9 相机沿用当前调校；Scene 缩放不等于 Game 构图。UI 用屏幕 Canvas 与锚点，适配 720p、1080p、手机横屏和安全区。密集目标的最小尺寸需用真实手柄与触控实测。
Keep the tuned 16:9 camera; Scene zoom is not Game composition. Use screen-space UI/anchors for 720p, 1080p, landscape mobile and safe areas. Validate minimum target size on gamepads and touch devices.

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
