// Public-facing translations. Design decisions live in Docs/Design/GAME_DESIGN.md.
export const worldStrings = {
  "zh-Hans": {
    nav: "开放世界计划", title: "开放世界计划｜BalloonBarrage 气球弹幕",
    description: "从一间画室走向音乐街区：中心广场、自由探索、定点音游挑战与对手 AI。未来开发规划，尚未实装。",
    home: "返回当前试玩", skip: "跳到正文", language: "语言", navigation: "规划导航",
    eyebrow: "未来版本 · 共同开发手记 / 01", headline: "把一首歌，\n变成一条想探索的街。",
    intro: "从温暖画室出发，走进有气球路标、音乐舞台和顽皮对手的彩色小镇。先做好一个街角，再让世界慢慢长大。",
    status: "规划中 · 尚未实装 · 暂无发布日期", updated: "方向确认于 2026.10.30",
    mapNav: "地图草案", loopNav: "玩法连接", aiNav: "对手 AI", planNav: "开发顺序", choicesNav: "一起选择",
    selected: "已选方向", decisions: ["中心广场＋音乐街区", "自由探索＋定点音乐挑战", "对手 AI：巡逻、抢气球、音乐对决"],
    boundary: "本页是未来设计提案。当前下载仍是画室音游；地图、移动、AI 与奖励系统均未由本次更新实现。",
    mapTitle: "一个中心，三种音乐性格。", mapIntro: "长远地图采用短环路连接广场；第一块只做“广场边缘 → 鼓点码头 → 返回”。每条路都能看见明确的归途标志。",
    mapLabel: "地图拓扑示意，非游戏截图，也不代表实际尺寸。广场分别连接三个街区。点击地点查看提案。",
    districtNames: ["气球广场", "鼓点码头", "旋律花园", "切分屋顶"],
    districtTags: ["安全中心", "第一块候选", "后续扩展", "后续研究"],
    districts: [
      "气球塔作为回家地标。练习墙、路线牌和画室入口围绕广场布置；出生点不进行抢夺对战。",
      "宽阔平路通往鼓棚舞台。用清晰鼓点、留白气球阵列和一个 AI 对手验证核心体验；暂不加入水上移动。",
      "以拱门和植物组成安静环路。用旋律问答点亮花园装饰，探索较舒缓的音乐表现。",
      "以错落楼顶表现更活泼的节奏。升空气球、升降装置与高低差仅作后续设想，先验证相机与遮挡。"
    ],
    scale: "设计目标，尚未实测：第一次走到舞台约 30–60 秒；探索、练习、一场挑战和返回合计约 5–8 分钟。用体验决定地图大小。",
    loopTitle: "气球是目标，节奏是规则，AI 是可读懂的对手。",
    loopIntro: "走路时自由看风景；想演奏时才进入舞台。推荐先使用稳定的舞台视角，让移动与瞄准各自清楚。",
    loop: [
      ["01 / 找到舞台", "气球塔、形状路牌和远处舞台引导探索；颜色之外也提供形状和轮廓线索。"],
      ["02 / 主动开始", "确认后淡出环境音乐，开始一首固定谱面的挑战。路过不会自动开局，正式谱面使用统一音乐时钟。"],
      ["03 / 选择气球", "按住仍可持续射击。AI 先预告目标，你可以抢同一颗，也可以打另一颗；合拍奖励保持可选。"],
      ["04 / 带回色彩", "结算后回到街区。提议用装饰、配色或音乐层奖励探索；输给 AI 不封锁唯一通路。"]
    ],
    rule: "路标气球、装饰气球和计分气球须有明确区别。现有画室的局内颜料保留，不等于开放世界已经支持跨场景或重启存档。",
    aiTitle: "先让一个对手，玩得明白。", aiIntro: "第一版 AI 的乐趣来自可预测的行动与抢球选择。先用离线行为规则完成一个对手，不依赖云端大模型。",
    aiStates: ["巡逻", "接受挑战", "预告目标", "瞄准射击", "短暂恢复"],
    aiRules: [
      ["先预告，再争抢", "转身、瞄准线或意图标记让你看懂下一步；保留反应延迟、瞄准误差和空当。"],
      ["同一气球只结算一次", "双方共享固定谱面。命中归属和同时命中要有明确规则；不穿墙、不瞬间必中。"],
      ["轻竞争，不堵路", "先不加伤害、击退和失败重开。保留单人演奏，AI 对决的记录与现有单人分数分开。"]
    ],
    planTitle: "每一步，都有一个可以验证的问题。", planIntro: "后续一起开发：先讲清一个小步骤，由你写核心玩法，再一起检查。完成并观察一个环节后，才扩建下一块。",
    stages: [
      ["OW0", "画出第一条路", "选择相机、对手性格和奖励方向，保留画室基线。", "确认：一条来回路线和一个首要体验问题。"],
      ["OW1", "走得舒服", "独立灰盒场景，只做广场边缘、一条路和一个舞台。", "观察：不迷路、视角舒适、相机不穿墙。"],
      ["OW2", "走进一首歌", "主动进入稳定舞台视角，复用一个现有歌曲配置，再退出。", "检查：音乐时钟、输入切换、结算与重试清理。"],
      ["OW3", "遇见一个对手", "从预告一颗气球开始，逐步加入瞄准、抢球和恢复。", "检查：命中唯一结算；玩家看得懂 AI 的意图。"],
      ["OW4", "完成一次小镇之旅", "串起码头挑战、结果、一项装饰和返回广场。", "观察：第一次游玩能独立完成；测量舒适度和性能。"],
      ["OW5", "再长出一个街区", "第一个完整体验成立后，再扩展花园或屋顶。", "确认：新增体验确有区别，并重新检查负载与可读性。"]
    ],
    deferred: "第一块暂不做：无缝大城市、程序生成地图、联网多人、多个同时参战 AI、云端对话或实时生成音乐。阶段是验证顺序，不是发布日期。",
    choicesTitle: "下一轮，你来定小镇的性格。", choicesIntro: "上面三个大方向已确认；下面是尚未选定的细节。可以组合试选，再把编号带回聊天讨论。这里不会提交投票或更改正式计划。",
    groups: [
      ["对手性格", ["A · 顽皮竞争者｜推荐：预告明显、空当多", "B · 均衡决斗者｜抢球更密集", "C · 捣蛋小队｜多个对手，范围更大"]],
      ["探索相机", ["A · 第三人称行走＋稳定舞台视角｜推荐研究", "B · 第一人称行走与瞄准", "C · 俯视探索＋独立舞台视角"]],
      ["成长奖励", ["A · 装饰与修复广场｜推荐", "B · 收集音乐层，让小镇逐渐合奏", "C · 解锁路线与移动玩具｜依赖更多系统"]]
    ],
    selectionLabel: "你的讨论草案", unset: "待选", selectionHint: "仅显示当前页面的试选；不保存、不上传。", reset: "清空试选", noScript: "可直接阅读选项，把喜欢的编号发回聊天；启用 JavaScript 可查看组合摘要。",
    sourcesTitle: "规划与当前试玩分开看。", sourceDesign: "完整游戏设计（英文）", sourceRoadmap: "开发路线图（英文）",
    originality: "视觉方向：鲜明色块、活泼轮廓、喷绘反馈与温暖的手工小镇。以《斯普拉顿》等作品的可读性为抽象参考，地图、角色、界面、美术和音乐独立设计。",
    footer: "先做一段值得反复走的小路。"
  },
  en: {
    nav: "Open-world plans", title: "Open-world plans | BalloonBarrage",
    description: "A plaza, music districts, opt-in rhythm stages and rival AI. A future BalloonBarrage design proposal, not implemented gameplay.",
    home: "Back to the current demo", skip: "Skip to content", language: "Language", navigation: "Plan navigation",
    eyebrow: "Future edition · Development notebook / 01", headline: "Turn a song into\na street worth exploring.",
    intro: "Step beyond the warm studio into a colorful town of balloon landmarks, music stages and playful rivals. Make one corner work, then let the world grow.",
    status: "Planning only · Not implemented · No release date", updated: "Direction confirmed 2026.10.30",
    mapNav: "Map sketch", loopNav: "Play loop", aiNav: "Rival AI", planNav: "Development steps", choicesNav: "Choose together",
    selected: "Selected direction", decisions: ["Central plaza + music districts", "Free exploration + fixed-location music challenges", "Rival AI: patrols, balloon contests and musical duels"],
    boundary: "This page describes future design. Downloads remain the studio rhythm game; this update implements no world map, movement, AI or reward system.",
    mapTitle: "One center. Three musical personalities.", mapIntro: "Short loops connect each proposed district to the plaza. Build only the plaza edge → Percussion Pier → return first. Every route needs a clear way home.",
    mapLabel: "Conceptual topology, not a game screenshot or a scale drawing. The plaza connects to three districts. Select a place to read its proposal.",
    districtNames: ["Balloon Plaza", "Percussion Pier", "Melody Garden", "Syncopation Rooftops"],
    districtTags: ["Safe hub", "First slice candidate", "Later expansion", "Later study"],
    districts: [
      "A balloon tower marks home. A practice wall, route signs and the studio entrance frame the plaza. No active rival contest in the arrival area.",
      "A wide, flat path leads to a drum-canopy stage. Spacious percussion patterns and one rival test the core experience, without water traversal.",
      "Arches and plants form a sheltered loop. Melodic call-and-response could illuminate garden decorations for a quieter musical experience.",
      "Stepped rooftops suggest livelier rhythms. Lift balloons, elevators and vertical routes remain later experiments, after camera and occlusion studies."
    ],
    scale: "Unmeasured design targets: about 30–60 seconds to the first stage; about 5–8 minutes for exploration, practice, one challenge and return. Let the experience set the map size.",
    loopTitle: "Balloons are targets. Rhythm sets the rules. Rivals show their intent.",
    loopIntro: "Explore freely, then enter a stage when you want to play. Start by studying a stable stage view so walking and aiming each stay readable.",
    loop: [
      ["01 / Find a stage", "Balloon towers, shaped signs and distant stages guide exploration. Pair color with shapes and distinct silhouettes."],
      ["02 / Choose to begin", "Fade out ambience after confirmation, then start an authored chart. Walking past never starts a round; one music clock owns judgement."],
      ["03 / Pick a balloon", "Held fire stays available. The rival telegraphs a target: contest it or choose another. On-beat rewards remain optional."],
      ["04 / Bring color home", "Return to the district after results. Decorations, color arrangements or musical layers are proposed rewards; losing never blocks the only route."]
    ],
    rule: "Landmark, decoration and scoring balloons need distinct presentation. Existing round-persistent studio paint does not imply world saves across scenes or application restarts.",
    aiTitle: "Make one rival understandable first.", aiIntro: "Predictable actions and target choices create the first AI encounter. Begin with offline behavior rules for one rival; no cloud model is required.",
    aiStates: ["Patrol", "Accept challenge", "Show intent", "Aim & fire", "Recover"],
    aiRules: [
      ["Telegraph before contesting", "Turning, aim cues or intent markers reveal the next action. Keep reaction delay, aim error and recovery openings."],
      ["Resolve each balloon once", "Share one authored chart. Define ownership and simultaneous hits; no firing through walls or instant perfect shots."],
      ["Compete without blocking exploration", "Start without damage, knockback or forced restarts. Keep solo stages available and rival records separate from existing solo scores."]
    ],
    planTitle: "One testable question at every step.", planIntro: "Develop together later: explain one small step, let the learner write the core gameplay, then review it together. Observe a working increment before expanding.",
    stages: [
      ["OW0", "Sketch the first route", "Choose camera, rival temperament and rewards; preserve the studio baseline.", "Agree on one return loop and one player-experience question."],
      ["OW1", "Walk comfortably", "A separate graybox: plaza edge, one path, one stage.", "Observe wayfinding, camera comfort and camera collision."],
      ["OW2", "Walk into a song", "Opt into a stable stage view, reuse one existing profile, then leave.", "Verify music clock, input handoff, results and Retry cleanup."],
      ["OW3", "Meet one rival", "Begin with one telegraphed balloon; add aiming, contesting and recovery.", "Verify single-hit resolution and whether players understand the intent."],
      ["OW4", "Complete one town visit", "Connect pier challenge, results, one decoration and the return to the plaza.", "Observe an unaided first visit; measure comfort and performance."],
      ["OW5", "Grow one more district", "Expand to the garden or rooftops only after the first full experience works.", "Confirm a distinct experience and recheck load and readability."]
    ],
    deferred: "Outside the first slice: seamless cities, procedural maps, online multiplayer, multiple active rivals, cloud dialogue and live generated music. These are gates, not release dates.",
    choicesTitle: "Choose the town's personality next.", choicesIntro: "The three broad directions above are confirmed. These details are still open. Try a combination and bring its letters back to the chat; this does not submit a vote or change the plan.",
    groups: [
      ["Rival temperament", ["A · Playful competitor | Recommended: clear cues, generous openings", "B · Balanced duelist | More contested targets", "C · Mischievous troupe | Multiple rivals, larger scope"]],
      ["Exploration camera", ["A · Third-person walk + stable stage view | Recommended study", "B · First-person walking and aiming", "C · Elevated exploration + separate stage view"]],
      ["Reward focus", ["A · Decorate and restore the plaza | Recommended", "B · Collect musical layers for a town ensemble", "C · Unlock routes and traversal toys | More dependencies"]]
    ],
    selectionLabel: "Your discussion draft", unset: "Undecided", selectionHint: "Current-page choices only; nothing is saved or uploaded.", reset: "Clear choices", noScript: "Read the options and send your preferred letters in chat; enable JavaScript for a combination summary.",
    sourcesTitle: "Keep plans separate from the current demo.", sourceDesign: "Full game design", sourceRoadmap: "Development roadmap",
    originality: "Visual direction: bold color blocks, playful silhouettes, paint feedback and a warm handmade town. Study readability in references such as Splatoon at an abstract level; design original maps, characters, interfaces, artwork and music.",
    footer: "Start with a little route worth walking again."
  },
  ja: {
    nav: "オープンワールド計画", title: "オープンワールド計画 | BalloonBarrage",
    description: "広場と音楽の街区、自由な探索、ステージ型リズムチャレンジ、ライバルAI。未実装の将来構想です。",
    home: "現在の体験版へ", skip: "本文へ移動", language: "言語", navigation: "計画のナビゲーション",
    eyebrow: "将来のバージョン · 開発ノート / 01", headline: "一曲の音楽を、\n歩きたくなる街に。",
    intro: "暖かなアトリエから、風船の目印、音楽ステージ、遊び好きなライバルのいる街へ。まず一つの街角を作り、少しずつ広げます。",
    status: "計画段階 · 未実装 · 公開日未定", updated: "方向性の確認日 2026.10.30",
    mapNav: "マップ案", loopNav: "遊びの流れ", aiNav: "ライバルAI", planNav: "開発の順序", choicesNav: "一緒に選ぶ",
    selected: "選択済みの方向性", decisions: ["中央広場＋音楽の街区", "自由な探索＋定点の音楽チャレンジ", "ライバルAI：巡回、風船の取り合い、音楽対決"],
    boundary: "このページは将来の設計案です。配布中の作品はアトリエのリズムゲームで、今回の更新ではマップ、移動、AI、報酬を実装していません。",
    mapTitle: "一つの広場、三つの音楽の表情。", mapIntro: "各街区と広場を短い周回路で結ぶ構想です。最初は「広場の端 → リズム埠頭 → 帰還」だけ。どの道にも帰り道の目印を置きます。",
    mapLabel: "接続関係の概念図です。ゲーム画面や縮尺図ではありません。広場から三つの街区へ接続します。場所を選ぶと提案を読めます。",
    districtNames: ["風船広場", "リズム埠頭", "メロディ庭園", "シンコペ屋根街"],
    districtTags: ["安全な拠点", "最初の試作候補", "後の拡張", "後の研究"],
    districts: [
      "風船の塔を帰還の目印に。練習壁、案内板、アトリエ入口を配置し、到着地点では取り合いを行いません。",
      "広く平らな道から、太鼓の屋根があるステージへ。見やすい打楽器のパターンと一体のAIで基本体験を確認します。水上移動は含めません。",
      "アーチと植物に囲まれた穏やかな周回路。メロディの問いかけと応答で庭の装飾が光る、静かな体験を検討します。",
      "段差のある屋根で、弾むリズムを表現。浮上風船、昇降機、高低差の移動は、カメラと遮蔽の検証後に検討します。"
    ],
    scale: "未計測の設計目標：最初のステージまで約30〜60秒。探索、練習、一回の挑戦と帰還で約5〜8分。体験を基準に広さを決めます。",
    loopTitle: "風船が目標、リズムがルール、AIは意図の見える相手。",
    loopIntro: "歩くときは自由に探索し、演奏したくなったらステージへ。最初は安定したステージ視点を試し、移動と照準をわかりやすくします。",
    loop: [
      ["01 / ステージを探す", "風船の塔、形の違う案内板、遠くの舞台が目印。色だけでなく形や輪郭でも道を示します。"],
      ["02 / 自分で開始する", "確認後に環境音楽をフェードアウトして譜面を開始。通り過ぎるだけでは始まらず、判定は一つの音楽時計に従います。"],
      ["03 / 風船を選ぶ", "長押しで撃ち続けられます。AIの狙いを見て同じ風船を競うか、別の風船を選択。リズム一致の報酬は任意です。"],
      ["04 / 色を持ち帰る", "結果表示後は街区へ戻ります。装飾、配色、音楽レイヤーを報酬として検討。AIに負けても唯一の道は閉ざしません。"]
    ],
    rule: "目印、装飾、得点用の風船は明確に区別します。現在のアトリエのラウンド内ペイント保持は、シーン移動や再起動後のワールド保存を意味しません。",
    aiTitle: "まず一体の行動を、わかりやすく。", aiIntro: "予測できる行動と狙う風船の選択が最初の楽しさです。オフラインの行動ルールから始め、クラウドモデルは必要としません。",
    aiStates: ["巡回", "挑戦を受ける", "狙いを予告", "狙って撃つ", "ひと休み"],
    aiRules: [
      ["競う前に予告する", "向き、照準の合図、意図マーカーで次の行動を示します。反応時間、狙いの誤差、休止の隙を設けます。"],
      ["風船一つにつき一度だけ判定", "同じ固定譜面を共有し、命中の帰属と同時命中のルールを定義。壁越しの射撃や即座の必中は行いません。"],
      ["探索を止めない競争", "最初はダメージ、ノックバック、強制やり直しを入れません。ソロ演奏を残し、対決記録と既存のソロ得点を分けます。"]
    ],
    planTitle: "一段階に、一つの確かめる問い。", planIntro: "今後は小さな手順を説明し、学習者がコアのコードを書き、一緒にレビューします。動く一歩を観察してから次へ進みます。",
    stages: [
      ["OW0", "最初の道を描く", "カメラ、相手の性格、報酬を選び、アトリエ版を保存。", "確認：一つの往復路と、最初に解く体験の課題。"],
      ["OW1", "心地よく歩く", "別のグレーボックスで広場の端、道一本、舞台一つ。", "観察：道のわかりやすさ、視点の快適さ、カメラの衝突。"],
      ["OW2", "一曲へ入る", "自分で安定した舞台視点へ入り、既存の曲設定一つを使って退出。", "検証：音楽時計、入力切替、結果とリトライ時の後処理。"],
      ["OW3", "一体の相手に会う", "風船一つの予告から、照準、取り合い、休止へ。", "検証：命中の一意な処理と、相手の意図の伝わり方。"],
      ["OW4", "一度の街歩きを完成", "埠頭の挑戦、結果、装飾一つ、広場への帰還をつなぐ。", "観察：初見での完了、快適さ、実機性能を測定。"],
      ["OW5", "街区を一つ広げる", "最初の体験が成立してから庭園や屋根街へ。", "確認：新しい遊びの違いと、負荷・見やすさ。"]
    ],
    deferred: "最初の試作には、シームレスな大都市、自動生成マップ、オンライン対戦、複数の同時対戦AI、クラウド会話、リアルタイム生成音楽を含めません。段階は検証順であり、公開日ではありません。",
    choicesTitle: "次は、街の性格を一緒に選ぶ。", choicesIntro: "上の三つの方向性は確認済みです。下の詳細は未決定。組み合わせを試して、記号をチャットに持ち帰れます。投票の送信や計画の変更は行いません。",
    groups: [
      ["相手の性格", ["A · 遊び好きな競争相手｜推奨：明確な予告と大きな隙", "B · 均衡した決闘相手｜取り合いが多め", "C · いたずら集団｜複数の相手で規模が大きい"]],
      ["探索カメラ", ["A · 三人称の歩行＋安定した舞台視点｜検討推奨", "B · 一人称の歩行と照準", "C · 見下ろし探索＋別の舞台視点"]],
      ["報酬の中心", ["A · 広場の装飾と修復｜推奨", "B · 音楽レイヤーを集めて街を合奏に", "C · 道と移動用の道具を解放｜依存する機能が多い"]]
    ],
    selectionLabel: "相談用の組み合わせ", unset: "未選択", selectionHint: "現在のページ内だけの試選択です。保存・送信はしません。", reset: "選択をクリア", noScript: "選択肢を読み、好きな記号をチャットへ。JavaScriptを有効にすると組み合わせを表示できます。",
    sourcesTitle: "将来の計画と、現在の体験版。", sourceDesign: "ゲーム設計全文（英語）", sourceRoadmap: "開発ロードマップ（英語）",
    originality: "視覚の方向性は、鮮やかな色面、遊び心のある輪郭、ペイントの反応と暖かな手作りの街。『スプラトゥーン』などから抽象的な見やすさを学び、マップ、キャラクター、UI、美術、音楽は独自に設計します。",
    footer: "また歩きたくなる、小さな道から。"
  }
};
