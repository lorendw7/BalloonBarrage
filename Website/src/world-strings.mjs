// Public-facing translations. Design decisions live in Docs/Design/GAME_DESIGN.md.
export const worldStrings = {
  "zh-Hans": {
    nav: "开放世界计划", title: "开放世界计划｜BalloonBarrage 气球弹幕",
    description: "从一间画室走向音乐街区：中心广场、自由探索、定点音游挑战与对手 AI。未来开发规划，尚未实装。",
    home: "返回当前试玩", skip: "跳到正文", language: "语言", navigation: "规划导航",
    eyebrow: "未来版本 · 共同开发手记 / 01", headline: "把一首歌，\n变成一条想探索的街。",
    intro: "从温暖画室出发，走进有气球路标、音乐舞台和顽皮对手的彩色小镇。先做好一个街角，再让世界慢慢长大。",
    status: "规划中 · 尚未实装 · 暂无发布日期", updated: "方向确认于 2026.10.30",
    mapNav: "地图草案", loopNav: "玩法连接", aiNav: "对手 AI", planNav: "开发顺序", choicesNav: "已确认细节",
    selected: "已选方向", decisions: ["中心广场＋音乐街区", "自由探索＋定点音乐挑战", "C · 对手：捣蛋小队", "A · 相机：第三人称探索＋稳定舞台视角", "B · 奖励：收集音乐层，让小镇合奏"],
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
    loopIntro: "已选第三人称自由探索，主动进入挑战时切换到稳定舞台视角。演奏结束后回到探索，让移动与瞄准各自清楚。",
    loop: [
      ["01 / 找到舞台", "气球塔、形状路牌和远处舞台引导探索；颜色之外也提供形状和轮廓线索。"],
      ["02 / 主动开始", "确认后淡出环境音乐，开始一首固定谱面的挑战。路过不会自动开局，正式谱面使用统一音乐时钟。"],
      ["03 / 选择气球", "按住仍可持续射击。AI 先预告目标，你可以抢同一颗，也可以打另一颗；合拍奖励保持可选。"],
      ["04 / 带回音乐", "结算后收集音乐层，让广场逐渐合奏。具体获得条件仍待设计；输给 AI 不封锁唯一通路。"]
    ],
    rule: "路标气球、装饰气球和计分气球须有明确区别。现有画室的局内颜料保留，不等于开放世界已经支持跨场景或重启存档。",
    aiTitle: "一支捣蛋小队，各有各的招。", aiIntro: "已选多名对手组成的小队。先验证一名成员，再加入第二名的分工与轮流行动，最后扩充队伍；抢球、佯动和舞台道具干扰是待细化的角色提案。采用离线行为规则，不依赖云端大模型。",
    aiStates: ["巡逻", "接受挑战", "预告目标", "瞄准射击", "短暂恢复"],
    aiRules: [
      ["先预告，再配合", "每名成员用清晰意图标记预告行动，保留反应延迟与空当。小队干扰不改变谱面时间、不遮住计分目标，也不堵住唯一道路。"],
      ["同一气球只结算一次", "双方共享固定谱面。命中归属和同时命中要有明确规则；不穿墙、不瞬间必中。"],
      ["轻竞争，不堵路", "先不加伤害、击退和失败重开。保留单人演奏，AI 对决的记录与现有单人分数分开。"]
    ],
    planTitle: "每一步，都有一个可以验证的问题。", planIntro: "后续一起开发：先讲清一个小步骤，由你写核心玩法，再一起检查。完成并观察一个环节后，才扩建下一块。",
    stages: [
      ["OW0", "画出第一条路", "C／A／B 方向已确认；细化路线、小队分工和音乐层获得条件，保留画室基线。", "方向确认不代表灰盒或试玩已经完成。"],
      ["OW1", "走得舒服", "独立第三人称灰盒，只做广场边缘、一条路和一个舞台。", "观察：不迷路、视角舒适、相机不穿墙。"],
      ["OW2", "走进一首歌", "主动进入稳定舞台视角，复用一个现有歌曲配置，再退出。", "检查：音乐时钟、输入切换、结算与重试清理。"],
      ["OW3a", "认识一名小队成员", "从预告一颗气球开始，逐步加入瞄准、抢球和恢复。", "检查：命中唯一结算；玩家看得懂 AI 的意图。"],
      ["OW3b", "两名成员配合", "加入第二名成员，验证分工、轮流行动和一种可读的干扰。", "检查：不同时制造难以辨认的威胁、不改判定时间、不堵路。"],
      ["OW4", "带回第一层音乐", "串起码头小队挑战、结果、音乐层收集和广场合奏。", "检查：重试不重复领奖、乐句边界接入、环境音乐与挑战切换。"],
      ["OW5", "让街区与合奏生长", "逐个扩展街区、兼容的音乐层和小队角色。", "确认：体验有区别、合奏清楚，并重新检查负载与可读性。"]
    ],
    deferred: "小队按一名成员、两人配合逐步验证，更大规模同时参战留到后续。第一块不做无缝大城市、程序生成地图、联网多人或云端生成音乐。阶段是验证顺序，不是发布日期。",
    choicesTitle: "C／A／B，已经确定的小镇方向。", choicesIntro: "创作者已确认以下选择，本次仅更新规划与官网。小队人数与分工、相机参数、音乐层获得条件和存档范围仍待一起细化。",
    choices: [
      ["C · 对手：捣蛋小队", "多名 AI 用不同招式争抢气球与制造可读的干扰。先验证单个成员，再形成小队配合；具体人数尚未确定。"],
      ["A · 相机：第三人称＋稳定舞台", "探索时能看见角色与道路；主动开局后切换到稳定舞台视角，结束再返回。重点验证遮挡、转场和输入交接。"],
      ["B · 奖励：音乐层收集", "让每次探索为小镇增加一个声部。可考虑码头鼓点、花园旋律和屋顶低音；需准备授权清楚、节拍调性兼容的音乐层，具体分配仍是提案。"]
    ],
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
    mapNav: "Map sketch", loopNav: "Play loop", aiNav: "Rival AI", planNav: "Development steps", choicesNav: "Confirmed details",
    selected: "Selected direction", decisions: ["Central plaza + music districts", "Free exploration + fixed-location music challenges", "C · Opponents: mischievous troupe", "A · Camera: third-person exploration + stable stage view", "B · Rewards: musical layers for the town ensemble"],
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
    loopIntro: "Third-person exploration is selected. Opt into a stable stage view for challenges, then return to exploration after results so walking and aiming each stay readable.",
    loop: [
      ["01 / Find a stage", "Balloon towers, shaped signs and distant stages guide exploration. Pair color with shapes and distinct silhouettes."],
      ["02 / Choose to begin", "Fade out ambience after confirmation, then start an authored chart. Walking past never starts a round; one music clock owns judgement."],
      ["03 / Pick a balloon", "Held fire stays available. The rival telegraphs a target: contest it or choose another. On-beat rewards remain optional."],
      ["04 / Bring music home", "Collect musical layers after challenges and grow the plaza ensemble. Exact reward conditions remain to be designed; losing never blocks the only route."]
    ],
    rule: "Landmark, decoration and scoring balloons need distinct presentation. Existing round-persistent studio paint does not imply world saves across scenes or application restarts.",
    aiTitle: "A mischievous troupe, each with a different trick.", aiIntro: "Multiple rivals are selected. Validate one member, then a coordinated pair with distinct roles and turn-taking before expanding. Balloon contests, feints and stage-prop interruptions are candidate roles. Use offline behavior rules; no cloud model is required.",
    aiStates: ["Patrol", "Accept challenge", "Show intent", "Aim & fire", "Recover"],
    aiRules: [
      ["Telegraph before coordinating", "Give each member clear intent cues, reaction delays and recovery openings. Interruptions must not shift chart times, hide scoring targets or block the only route."],
      ["Resolve each balloon once", "Share one authored chart. Define ownership and simultaneous hits; no firing through walls or instant perfect shots."],
      ["Compete without blocking exploration", "Start without damage, knockback or forced restarts. Keep solo stages available and rival records separate from existing solo scores."]
    ],
    planTitle: "One testable question at every step.", planIntro: "Develop together later: explain one small step, let the learner write the core gameplay, then review it together. Observe a working increment before expanding.",
    stages: [
      ["OW0", "Sketch the first route", "C / A / B is confirmed; refine the route, troupe roles and layer rewards, preserving the studio baseline.", "Confirmed direction does not mean a graybox or playtest is complete."],
      ["OW1", "Walk comfortably", "A separate third-person graybox: plaza edge, one path, one stage.", "Observe wayfinding, camera comfort and camera collision."],
      ["OW2", "Walk into a song", "Opt into a stable stage view, reuse one existing profile, then leave.", "Verify music clock, input handoff, results and Retry cleanup."],
      ["OW3a", "Meet one troupe member", "Begin with one telegraphed balloon; add aiming, contesting and recovery.", "Verify single-hit resolution and whether players understand the intent."],
      ["OW3b", "Coordinate a pair", "Add a second member, distinct roles, turn-taking and one readable interruption.", "No unreadable simultaneous threats, shifted judgement times or blocked routes."],
      ["OW4", "Bring back the first layer", "Connect pier troupe challenge, results, a musical layer and the plaza ensemble.", "Check reward deduplication on Retry, phrase-aligned entry and ambience/challenge handoff."],
      ["OW5", "Grow the town ensemble", "Expand one district, compatible music layer and troupe role at a time.", "Confirm distinct experiences and an audible ensemble; recheck load and readability."]
    ],
    deferred: "Validate the troupe as one member, then a coordinated pair; larger simultaneous groups come later. Seamless cities, procedural maps, online multiplayer and cloud-generated music stay outside the first slice. These are gates, not release dates.",
    choicesTitle: "C / A / B: the town's direction is confirmed.", choicesIntro: "The creator has confirmed these choices. This update covers planning and the website only. Troupe size and roles, camera settings, reward conditions and save scope still need joint design.",
    choices: [
      ["C · Opponents: mischievous troupe", "Multiple rivals contest balloons and create readable interruptions. Validate individual behavior before group coordination; exact team size remains open."],
      ["A · Camera: third person + stable stage", "See the character and route while exploring; explicitly enter a stable stage view, then return after results. Validate occlusion, transitions and input handoff."],
      ["B · Rewards: collect musical layers", "Each visit can add a voice to the town. Pier percussion, garden melody and rooftop bass are examples. Prepare appropriately licensed, tempo/key-compatible layers; district assignments remain proposals."]
    ],
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
    mapNav: "マップ案", loopNav: "遊びの流れ", aiNav: "ライバルAI", planNav: "開発の順序", choicesNav: "確定した詳細",
    selected: "選択済みの方向性", decisions: ["中央広場＋音楽の街区", "自由な探索＋定点の音楽チャレンジ", "C · 相手：いたずら集団", "A · カメラ：三人称探索＋安定した舞台視点", "B · 報酬：音楽レイヤーを集めて街を合奏に"],
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
    loopIntro: "三人称での自由な探索を選択済み。自分で挑戦を始めると安定した舞台視点へ切り替わり、結果表示後は探索へ戻ります。",
    loop: [
      ["01 / ステージを探す", "風船の塔、形の違う案内板、遠くの舞台が目印。色だけでなく形や輪郭でも道を示します。"],
      ["02 / 自分で開始する", "確認後に環境音楽をフェードアウトして譜面を開始。通り過ぎるだけでは始まらず、判定は一つの音楽時計に従います。"],
      ["03 / 風船を選ぶ", "長押しで撃ち続けられます。AIの狙いを見て同じ風船を競うか、別の風船を選択。リズム一致の報酬は任意です。"],
      ["04 / 音楽を持ち帰る", "音楽レイヤーを集め、広場の合奏を育てます。具体的な獲得条件は今後設計し、AIに負けても唯一の道は閉ざしません。"]
    ],
    rule: "目印、装飾、得点用の風船は明確に区別します。現在のアトリエのラウンド内ペイント保持は、シーン移動や再起動後のワールド保存を意味しません。",
    aiTitle: "それぞれの技を持つ、いたずら集団。", aiIntro: "複数のライバルを選択済み。まず一体を検証し、次に二体の役割分担と交代行動を確かめてから拡張します。風船の取り合い、フェイント、舞台装置のいたずらは役割案です。オフラインの行動ルールを使います。",
    aiStates: ["巡回", "挑戦を受ける", "狙いを予告", "狙って撃つ", "ひと休み"],
    aiRules: [
      ["連携する前に予告する", "各メンバーが意図を明確に示し、反応時間と休止の隙を設けます。妨害で譜面時刻を変えたり、得点用の風船や唯一の道を塞いだりしません。"],
      ["風船一つにつき一度だけ判定", "同じ固定譜面を共有し、命中の帰属と同時命中のルールを定義。壁越しの射撃や即座の必中は行いません。"],
      ["探索を止めない競争", "最初はダメージ、ノックバック、強制やり直しを入れません。ソロ演奏を残し、対決記録と既存のソロ得点を分けます。"]
    ],
    planTitle: "一段階に、一つの確かめる問い。", planIntro: "今後は小さな手順を説明し、学習者がコアのコードを書き、一緒にレビューします。動く一歩を観察してから次へ進みます。",
    stages: [
      ["OW0", "最初の道を描く", "C／A／Bは確定。道、集団の役割、音楽レイヤーの獲得条件を詰め、アトリエ版を保存。", "方向の確定は試作やプレイテストの完了を意味しません。"],
      ["OW1", "心地よく歩く", "別の三人称グレーボックスで広場の端、道一本、舞台一つ。", "観察：道のわかりやすさ、視点の快適さ、カメラの衝突。"],
      ["OW2", "一曲へ入る", "自分で安定した舞台視点へ入り、既存の曲設定一つを使って退出。", "検証：音楽時計、入力切替、結果とリトライ時の後処理。"],
      ["OW3a", "一体のメンバーに会う", "風船一つの予告から、照準、取り合い、休止へ。", "検証：命中の一意な処理と、相手の意図の伝わり方。"],
      ["OW3b", "二体の連携を作る", "二体目を加え、役割分担、交代行動、読める妨害を一つ検討。", "検証：読めない同時攻撃、判定時刻の変更、通路封鎖を避ける。"],
      ["OW4", "最初の音楽を持ち帰る", "埠頭の集団チャレンジ、結果、レイヤー収集、広場合奏をつなぐ。", "検証：リトライ時の報酬重複防止、楽句境界での合流、音楽切替。"],
      ["OW5", "街と合奏を育てる", "街区、調和する音楽レイヤー、集団の役割を一つずつ追加。", "確認：体験の違い、合奏の聞きやすさ、負荷と視認性。"]
    ],
    deferred: "集団は一体、次に二体の連携から検証し、大人数の同時行動は後に回します。最初の試作に大都市、自動生成マップ、オンライン対戦、クラウド生成音楽は含めません。段階は公開日ではありません。",
    choicesTitle: "C／A／B、街の方向性が決まりました。", choicesIntro: "制作者が以下を確認しました。今回は計画とウェブサイトのみ更新します。人数と役割、カメラ設定、報酬条件、保存範囲は今後一緒に設計します。",
    choices: [
      ["C · 相手：いたずら集団", "複数のAIが異なる技で風船を競い、読める妨害を行う構想。一体の行動から集団の連携へ進め、人数は今後決めます。"],
      ["A · カメラ：三人称＋安定した舞台", "探索ではキャラクターと道を見渡し、挑戦開始後は安定した舞台視点へ。終了後に戻し、遮蔽、遷移、入力の受け渡しを検証します。"],
      ["B · 報酬：音楽レイヤー収集", "訪れるたびに街へ声部を追加。埠頭の打楽器、庭園の旋律、屋根街の低音などを検討します。適切な許諾とテンポ・調性の合う素材を用意し、割り当ては今後決めます。"]
    ],
    sourcesTitle: "将来の計画と、現在の体験版。", sourceDesign: "ゲーム設計全文（英語）", sourceRoadmap: "開発ロードマップ（英語）",
    originality: "視覚の方向性は、鮮やかな色面、遊び心のある輪郭、ペイントの反応と暖かな手作りの街。『スプラトゥーン』などから抽象的な見やすさを学び、マップ、キャラクター、UI、美術、音楽は独自に設計します。",
    footer: "また歩きたくなる、小さな道から。"
  }
};
