// Public-facing translations. Design decisions live in Docs/Design/GAME_DESIGN.md.
export const worldStrings = {
  "zh-Hans": {
    nav: "开放世界计划", title: "开放世界计划｜BalloonBarrage 气球弹幕",
    description: "从一间画室走向音乐街区：中心广场、自由探索、定点音游挑战与对手 AI。未来开发规划，尚未实装。",
    home: "返回当前试玩", skip: "跳到正文", language: "语言", navigation: "规划导航",
    eyebrow: "未来版本 · 音乐小镇计划", headline: "先逛一段街，\n再玩一首歌。",
    intro: "设想一座能自由散步的音乐小镇：你从广场出发，找到喜欢的舞台，停下来射气球、玩节奏挑战，再带一段音乐回家。第一步只做广场、一条路和码头舞台。",
    status: "规划中 · 尚未实装 · 暂无发布日期", updated: "方向确认于 2026.10.30",
    mapNav: "小镇有哪些地方", loopNav: "一局怎么玩", aiNav: "捣蛋对手", planNav: "开发顺序", choicesNav: "已选定的玩法",
    selected: "想象你第一次来到小镇", decisions: ["在广场认识道路，试着打几颗练习气球。", "沿路标走到码头，看见一座小舞台。", "自己选择开始，镜头停稳，音乐响起。", "和捣蛋角色选气球、抢目标，完成一首歌。", "回到广场，听见新带回的鼓点加入合奏。"],
    boundary: "这是未来的一次游玩示例，尚未做成。当前可下载的是画室里的气球音游；音乐小镇还不能试玩。奖励条件也仍待设计。",
    mapTitle: "广场像家，街区像三条出门的路。", mapIntro: "每次从中心广场出发，去一个街区玩，再回广场。远期想做码头、花园、屋顶三个方向；第一步只打通“广场 → 码头舞台 → 返回”。",
    mapLabel: "这张草图只说明地点怎样相连，不是游戏画面或实际尺寸。点击地点看看在那里做什么。",
    districtNames: ["气球广场", "鼓点码头", "旋律花园", "切分屋顶"],
    districtTags: ["安全中心", "第一块候选", "后续扩展", "后续研究"],
    districts: [
      "气球塔作为回家地标。练习墙、路线牌和画室入口围绕广场布置；出生点不进行抢夺对战。",
      "宽阔平路通往鼓棚舞台。用清晰鼓点、留白气球阵列和一个 AI 对手验证核心体验；暂不加入水上移动。",
      "以拱门和植物组成安静环路。用旋律问答点亮花园装饰，探索较舒缓的音乐表现。",
      "以错落楼顶表现更活泼的节奏。升空气球、升降装置与高低差仅作后续设想，先验证相机与遮挡。"
    ],
    scale: "设计目标，尚未实测：第一次走到舞台约 30–60 秒；探索、练习、一场挑战和返回合计约 5–8 分钟。用体验决定地图大小。",
    loopTitle: "到码头玩一局，会发生什么？",
    loopIntro: "散步时自由走、没有节奏评分；到了舞台，自己确认才开始射气球的音乐挑战。结束后恢复走路，可以回广场，也可以留下再练一次。",
    loop: [
      ["01 / 顺着路标走","广场的气球塔告诉你家在哪，路牌带你去码头。第一版先走一条清楚的小路，隐藏路线以后再加。"],
      ["02 / 站定，选择开始","你确认开局，镜头对准舞台，街上的背景音乐让位给挑战歌曲。路过舞台不会突然开始比赛。"],
      ["03 / 看目标，射气球","例如对手先用标记指向一颗气球，你可以抢它，也可以选另一颗。按住能持续射击；跟上拍子有额外奖励，没合拍也能继续玩。"],
      ["04 / 听见小镇变化","例如完成码头挑战后，带回一段鼓点，广场原来的旋律多了一层伴奏。具体获得条件待设计，输了也能继续逛小镇。"]
    ],
    rule: "街上的气球有三种用途：指路、装饰、舞台上的射击目标；外形会帮助你区分。收集进度能保存多久仍未决定。",
    aiTitle: "AI 就是游戏控制的捣蛋对手。", aiIntro: "它会按事先设计的规则行动。先做一名会指目标、抢气球、停下来喘口气的角色，再考虑两人轮流配合。你可以观察它的提示来选下一颗目标。",
    aiStates: ["巡逻", "接受挑战", "预告目标", "瞄准射击", "短暂恢复"],
    aiRules: [
      ["先让你看懂它想干什么", "先转身、瞄准、显示目标，再开枪，留时间让你反应。两名角色也要轮流提示，避免屏幕上同时挤满信息。"],
      ["它也要遵守规则", "一颗气球只能算一次命中。对手会有失误和休息，不能隔墙射击或一出现就必中；同时打中的归属还需测试。"],
      ["输了也能继续逛", "初步设计不扣生命、不把你打飞，也不强迫重开。仍可选单人演奏；对决成绩与单人记录分开。"]
    ],
    planTitle: "每一步，都有一个可以验证的问题。", planIntro: "后续一起开发：先讲清一个小步骤，由你写核心玩法，再一起检查。完成并观察一个环节后，才扩建下一块。",
    stages: [
      ["OW0","画出第一条路","玩法 C／A／B 与美术 A＋C 已确认；画出白天路线，确定底色、材质与气球分类。","方向确认不代表灰盒或试玩已经完成。"],
      ["OW1","走得舒服","独立第三人称灰盒，只做广场边缘、一条路和一个舞台。","观察：不迷路、视角舒适、相机不穿墙。"],
      ["OW2","走进一首歌","主动进入稳定舞台视角，复用一个现有歌曲配置，再退出。","检查：音乐时钟、输入切换、结算与重试清理。"],
      ["OW3a","认识一名小队成员","从预告一颗气球开始，逐步加入瞄准、抢球和恢复。","检查：命中唯一结算；玩家看得懂 AI 的意图。"],
      ["OW3b","两名成员配合","加入第二名成员，验证分工、轮流行动和一种可读的干扰。","检查：不同时制造难以辨认的威胁、不改判定时间、不堵路。"],
      ["OW4","带回第一层音乐","串起码头挑战、音乐层与广场合奏；加入手作场景和一处可见的完成反馈。","检查：重试不重复领奖、乐句边界接入、环境音乐与挑战切换。"],
      ["OW5","让街区与合奏生长","白天体验通过后，逐个研究街区、单规则短挑战、可选绕路与个性装饰。","确认：体验有区别、合奏清楚，并重新检查负载与可读性。"]
    ],
    deferred: "先完成白天的一条路、一座舞台和基础小队体验。夜间音乐节、大规模同场对手、无缝城市、程序地图、联网多人和云端生成音乐留到后续。阶段是验证顺序，不是发布日期。",
    choicesTitle: "三个说法，换成实际游玩来理解。", choicesIntro: "这些是已选定的方向。对手人数、具体奖励条件和进度保存方式，还要在小规模试做中一起确定。",
    choices: [
      ["捣蛋小队：会与你抢气球的角色", "由游戏控制的对手先示意要打哪颗气球，你决定跟它争抢，还是换个目标。先做一名，再逐步加入配合。"],
      ["两种镜头：逛街看路，演奏看目标", "“第三人称”就是从角色身后看到自己和道路。开局后镜头稳定地对着舞台，让你专心瞄准气球，结束再回到散步。"],
      ["音乐层：给广场合奏添一种声音", "把一首合奏想成鼓点、旋律、低音叠在一起。每带回一层，广场就多一种声音；例如先有鼓点，再加入花园旋律。具体分配仍是提案。"]
    ],
    roleTitle: "展开：三个候选角色",
    roleIntro: "角色名与规则均为提案，不代表首版要做三名。先完成一个基础对手，再逐项研究。",
    roles: [
      ["抢拍手","展示短气球乐句，让玩家接奏。需单独制作问答谱面，不在演奏中临时改谱。"],
      ["气球搬运工","在可选探索任务中搬运气球，玩家观察路线、选择捷径。进入舞台后暂停追逐。"],
      ["舞台领队","开局前公布一种规则变化，例如长按气球。属于后续谱面与输入研究，不改变正在进行的挑战。"]
    ],
    artNav: "美术方向",
    artTitle: "阳光音乐节，手作的小镇。",
    artIntro: "A＋C 已确认：延续温暖画室，把气球、音乐和街区生活连成一体。先用于广场一角、一条路和一座舞台；本页是设计简报，尚非游戏场景。",
    artRules: [
      ["暖底色，鲜明目标","奶油色建筑与日光打底，青绿、珊瑚橙点缀。背景保持安静，交互点与气球用对比色、形状和图标共同提示。"],
      ["有光泽，也有手作感","亮面气球搭配哑光地面、纸海报、纸板招牌、布旗与木舞台。鼓胀、回弹、泄气反馈短促清楚，不遮住下一颗目标。"],
      ["让音乐成为街区生活","用打气筒、颜料桶、鼓槌和节拍器设计原创道具。各街区拥有自己的乐队与海报；气球花纹、服饰和演出动作留作后续个性表达。"]
    ],
    artLater: "夜间音乐节后置：先验证白天街区，再单独研究夜景灯光、目标清晰度与音乐切换。暂不承诺昼夜循环或上线日期。",
    sourcesTitle: "规划与当前试玩分开看。", sourceDesign: "完整游戏设计（英文）", sourceRoadmap: "开发路线图（英文）",
    originality: "参考《斯普拉顿》把玩法、音乐和生活文化统一起来的方法。地图、角色、界面、美术与音乐独立设计；完整设计文档保留官方参考来源。",
    footer: "先做一段值得反复走的小路。"
  },
  en: {
    nav: "Open-world plans", title: "Open-world plans | BalloonBarrage",
    description: "A plaza, music districts, opt-in rhythm stages and rival AI. A future BalloonBarrage design proposal, not implemented gameplay.",
    home: "Back to the current demo", skip: "Skip to content", language: "Language", navigation: "Plan navigation",
    eyebrow: "Future edition · A musical town", headline: "Wander a street.\nStop to play a song.",
    intro: "Imagine a small town you can explore freely: leave the plaza, find a stage, stop to shoot balloons in a rhythm challenge, then bring some music home. Start with just the plaza, one path and the pier stage.",
    status: "Planning only · Not implemented · No release date", updated: "Direction confirmed 2026.10.30",
    mapNav: "Places to visit", loopNav: "Playing a round", aiNav: "Rivals", planNav: "Development steps", choicesNav: "Selected play ideas",
    selected: "Imagine your first visit", decisions: ["Get your bearings in the plaza and try some practice balloons.", "Follow the signs to a small stage on the pier.", "Choose to start: the camera settles and the music begins.", "Pick balloons and contest targets with a mischievous rival.", "Return to the plaza and hear your new drum part join the ensemble."],
    boundary: "This is an example of a future visit, not a playable town. Current downloads are the studio balloon rhythm game. Reward conditions still need design.",
    mapTitle: "The plaza is home. Three paths lead to music.", mapIntro: "Leave the central plaza for one district, play there, then return. The longer-term idea includes a pier, garden and rooftops; first build only plaza → pier stage → home.",
    mapLabel: "This sketch shows how places connect, not a game screen or actual scale. Select a place to see what you might do there.",
    districtNames: ["Balloon Plaza", "Percussion Pier", "Melody Garden", "Syncopation Rooftops"],
    districtTags: ["Safe hub", "First slice candidate", "Later expansion", "Later study"],
    districts: [
      "A balloon tower marks home. A practice wall, route signs and the studio entrance frame the plaza. No active rival contest in the arrival area.",
      "A wide, flat path leads to a drum-canopy stage. Spacious percussion patterns and one rival test the core experience, without water traversal.",
      "Arches and plants form a sheltered loop. Melodic call-and-response could illuminate garden decorations for a quieter musical experience.",
      "Stepped rooftops suggest livelier rhythms. Lift balloons, elevators and vertical routes remain later experiments, after camera and occlusion studies."
    ],
    scale: "Unmeasured design targets: about 30–60 seconds to the first stage; about 5–8 minutes for exploration, practice, one challenge and return. Let the experience set the map size.",
    loopTitle: "What would one round at the pier feel like?",
    loopIntro: "Walk freely without rhythm grading. At a stage, choose to begin a balloon-shooting music challenge. Afterwards you can walk home or stay for another try.",
    loop: [
      ["01 / Follow the signs","The plaza's balloon tower marks home and signs lead to the pier. Make one clear path first; hidden routes can come later."],
      ["02 / Stop and choose to start","The camera faces the stage and street music gives way to the challenge song. Simply walking past a stage does not start a contest."],
      ["03 / Choose and shoot","A marker shows which balloon the rival will shoot. Contest it or pick another. Hold to keep firing; hitting the beat adds a bonus, while other shots remain valid."],
      ["04 / Hear the town change","A pier challenge might bring back drums to accompany the plaza's melody. Exact reward conditions remain open; losing still lets you explore."]
    ],
    rule: "Balloons have three jobs: mark routes, decorate the town, or serve as stage targets. Their appearance should help you tell them apart. How collected progress is saved remains undecided.",
    aiTitle: "An AI rival is a character controlled by the game.", aiIntro: "It follows designed behavior rules. Start with one character that points at a target, contests a balloon and pauses to recover; then consider a pair taking turns. Read its cues to choose your next target.",
    aiStates: ["Patrol", "Accept challenge", "Show intent", "Aim & fire", "Recover"],
    aiRules: [
      ["Let you see its next move", "It turns, aims and shows a target before firing, leaving time to react. A pair should take turns giving cues so the screen stays understandable."],
      ["The rival follows rules too", "A balloon counts once. Rivals make mistakes and rest; they cannot fire through walls or land an instant perfect shot. Simultaneous hits still need testing."],
      ["Keep exploring after a loss", "The initial design has no health damage, knockback or forced restart. Solo play remains available and rival scores stay separate from solo records."]
    ],
    planTitle: "One testable question at every step.", planIntro: "Develop together later: explain one small step, let the learner write the core gameplay, then review it together. Observe a working increment before expanding.",
    stages: [
      ["OW0","Sketch the first route","Gameplay C / A / B and art A + C are confirmed; sketch the daylight route, palette, materials and balloon categories.","Confirmed direction does not mean a graybox or playtest is complete."],
      ["OW1","Walk comfortably","A separate third-person graybox: plaza edge, one path, one stage.","Observe wayfinding, camera comfort and camera collision."],
      ["OW2","Walk into a song","Opt into a stable stage view, reuse one existing profile, then leave.","Verify music clock, input handoff, results and Retry cleanup."],
      ["OW3a","Meet one troupe member","Begin with one telegraphed balloon; add aiming, contesting and recovery.","Verify single-hit resolution and whether players understand the intent."],
      ["OW3b","Coordinate a pair","Add a second member, distinct roles, turn-taking and one readable interruption.","No unreadable simultaneous threats, shifted judgement times or blocked routes."],
      ["OW4","Bring back the first layer","Connect the pier challenge, musical layer and plaza ensemble; add handmade scenery and one visible completion change.","Check reward deduplication on Retry, phrase-aligned entry and ambience/challenge handoff."],
      ["OW5","Grow the town ensemble","After the daytime slice passes, study districts, short one-rule challenges, optional detours and cosmetic identity individually.","Confirm distinct experiences and an audible ensemble; recheck load and readability."]
    ],
    deferred: "Finish one daytime route, stage and baseline troupe experience first. Night festivals, larger simultaneous groups, seamless cities, procedural maps, online multiplayer and cloud-generated music come later. These are gates, not release dates.",
    choicesTitle: "Three ideas, explained through play.", choicesIntro: "These directions are selected. Rival numbers, exact reward conditions and progress saving still need to be worked out in small prototypes.",
    choices: [
      ["Mischievous troupe: characters contesting balloons", "Game-controlled rivals signal which balloon they want. You decide whether to race them for it or choose another. Start with one, then build cooperation."],
      ["Two camera views: see the path, then the targets", "Third person means seeing your character and the path from behind. Starting a round steadies the view on the stage so you can aim, then returns you to walking."],
      ["Musical layers: add a sound to the plaza", "Think of drums, melody and bass playing together. Each collected layer adds one sound to the plaza: perhaps drums first, then a garden melody. Exact assignments are still proposals."]
    ],
    roleTitle: "Explore three candidate roles",
    roleIntro: "Names and rules are proposals, not three required first-slice characters. Validate one baseline rival before each later study.",
    roles: [
      ["Beat Teaser","Demonstrates a short balloon phrase for the player to answer. Requires a separate authored call-and-response chart, never a mid-song chart change."],
      ["Balloon Porter","Carries a balloon in an optional exploration task; observe the route and choose a shortcut. Pause pursuit during stage play."],
      ["Stage Leader","Announces one modifier before a round, such as a sustained balloon. A later chart/input study, never a change to a running challenge."]
    ],
    artNav: "Art direction",
    artTitle: "A sunny festival. A handmade town.",
    artIntro: "A + C is confirmed: extend the warm studio into a town where balloons, music and daily life belong together. Apply it to one plaza corner, path and stage first. This is a design brief, not a game scene.",
    artRules: [
      ["Warm scenery, clear targets","Cream buildings and daylight with teal and coral accents. Keep backgrounds calm; use contrast, shapes and icons together for balloons and interaction cues."],
      ["Gloss meets handmade texture","Glossy balloons against matte ground, paper posters, cardboard signs, fabric bunting and wooden stages. Brief inflation, springy hits and deflation never hide the next target."],
      ["Music as local culture","Design original props from pumps, paint buckets, drumsticks and metronomes. Give districts local bands and posters; balloon patterns, clothing and performance gestures are later identity options."]
    ],
    artLater: "Night festival deferred: accept the daytime slice first, then study night lighting, target readability and music transitions separately. No day/night cycle or release date is promised.",
    sourcesTitle: "Keep plans separate from the current demo.", sourceDesign: "Full game design", sourceRoadmap: "Development roadmap",
    originality: "Study how Splatoon connects play, music and local culture. Maps, characters, interfaces, artwork and music remain original; the full design document records official references.",
    footer: "Start with a little route worth walking again."
  },
  ja: {
    nav: "オープンワールド計画", title: "オープンワールド計画 | BalloonBarrage",
    description: "広場と音楽の街区、自由な探索、ステージ型リズムチャレンジ、ライバルAI。未実装の将来構想です。",
    home: "現在の体験版へ", skip: "本文へ移動", language: "言語", navigation: "計画のナビゲーション",
    eyebrow: "将来の版 · 音楽の街の計画", headline: "街を歩いて、\n立ち止まって一曲。",
    intro: "自由に散歩できる小さな音楽の街を想像してください。広場から舞台を探し、立ち止まって風船を撃つリズム遊びを楽しみ、音楽を持ち帰ります。まずは広場、道一本、埠頭の舞台だけを作る計画です。",
    status: "計画段階 · 未実装 · 公開日未定", updated: "方向性の確認日 2026.10.30",
    mapNav: "街の場所", loopNav: "一回の遊び方", aiNav: "いたずらな相手", planNav: "開発の順序", choicesNav: "選択済みの遊び",
    selected: "初めての訪問を想像すると", decisions: ["広場で道を覚え、練習用の風船を撃ってみる。", "案内板をたどると、埠頭に小さな舞台がある。", "自分で開始を選ぶと、視点が落ち着いて曲が始まる。", "いたずらな相手と風船を選び、取り合いながら一曲遊ぶ。", "広場へ帰ると、持ち帰った太鼓の音が合奏に加わる。"],
    boundary: "これは将来の遊び方の例で、街はまだ遊べません。現在配布中なのはアトリエの風船リズムゲームです。報酬の条件も今後決めます。",
    mapTitle: "広場は帰る場所。三つの道の先に音楽。", mapIntro: "中央広場から街区へ出かけて遊び、また広場へ戻ります。長期的には埠頭、庭園、屋根街の三方向を考え、まず「広場 → 埠頭の舞台 → 帰還」だけを作ります。",
    mapLabel: "場所同士のつながりを示す草図です。ゲーム画面や実際の広さではありません。場所を選ぶと、そこで何をするか読めます。",
    districtNames: ["風船広場", "リズム埠頭", "メロディ庭園", "シンコペ屋根街"],
    districtTags: ["安全な拠点", "最初の試作候補", "後の拡張", "後の研究"],
    districts: [
      "風船の塔を帰還の目印に。練習壁、案内板、アトリエ入口を配置し、到着地点では取り合いを行いません。",
      "広く平らな道から、太鼓の屋根があるステージへ。見やすい打楽器のパターンと一体のAIで基本体験を確認します。水上移動は含めません。",
      "アーチと植物に囲まれた穏やかな周回路。メロディの問いかけと応答で庭の装飾が光る、静かな体験を検討します。",
      "段差のある屋根で、弾むリズムを表現。浮上風船、昇降機、高低差の移動は、カメラと遮蔽の検証後に検討します。"
    ],
    scale: "未計測の設計目標：最初のステージまで約30〜60秒。探索、練習、一回の挑戦と帰還で約5〜8分。体験を基準に広さを決めます。",
    loopTitle: "埠頭で一回遊ぶと、どうなる？",
    loopIntro: "散歩中は自由に歩き、リズムの採点はありません。舞台で自分から開始すると、風船を撃つ音楽チャレンジへ。終わったら広場へ帰っても、もう一度練習しても構いません。",
    loop: [
      ["01 / 案内板をたどる","広場の風船の塔が帰る場所の目印。案内板で埠頭へ進みます。まず道一本をわかりやすく作り、隠し道は後に検討します。"],
      ["02 / 立ち止まって開始する","自分で開始を選ぶと視点が舞台に向き、街の音楽から挑戦の曲へ。通り過ぎただけでは始まりません。"],
      ["03 / 狙う風船を選ぶ","例えば相手のマークが次の狙いを示します。同じ風船を競っても、別の風船を狙っても大丈夫。長押しで撃ち続けられ、拍に合うと追加報酬があります。"],
      ["04 / 街の変化を聞く","例えば埠頭から太鼓の音を持ち帰ると、広場の旋律に伴奏が加わります。獲得条件は今後決めますが、負けても散歩を続けられます。"]
    ],
    rule: "風船の役目は道案内、街の飾り、舞台の射撃目標の三つ。見た目で区別できるようにします。集めた進捗の保存方法は未定です。",
    aiTitle: "AIは、ゲームが動かすいたずらな相手。", aiIntro: "あらかじめ決めた行動ルールで動きます。まず狙いを示し、風船を取り合い、ひと休みする一体から。その後に二体の交代行動を考えます。相手の合図を見て次の目標を選べます。",
    aiStates: ["巡回", "挑戦を受ける", "狙いを予告", "狙って撃つ", "ひと休み"],
    aiRules: [
      ["次に何をするか、先に見せる", "振り向き、狙い、目標を示してから撃ち、反応する時間を残します。二体でも順番に合図し、画面をわかりやすく保ちます。"],
      ["相手もルールを守る", "風船の命中は一度だけ数えます。相手も外したり休んだりし、壁越しや登場直後の必中はありません。同時命中の扱いは今後検証します。"],
      ["負けても散歩は続く", "初期案では体力ダメージ、吹き飛ばし、強制やり直しを入れません。ソロ演奏も選べて、対決の得点はソロ記録と分けます。"]
    ],
    planTitle: "一段階に、一つの確かめる問い。", planIntro: "今後は小さな手順を説明し、学習者がコアのコードを書き、一緒にレビューします。動く一歩を観察してから次へ進みます。",
    stages: [
      ["OW0","最初の道を描く","遊びのC／A／Bと美術のA＋Cは確定。昼の道、配色、素材、風船の区別を描きます。","方向の確定は試作やプレイテストの完了を意味しません。"],
      ["OW1","心地よく歩く","別の三人称グレーボックスで広場の端、道一本、舞台一つ。","観察：道のわかりやすさ、視点の快適さ、カメラの衝突。"],
      ["OW2","一曲へ入る","自分で安定した舞台視点へ入り、既存の曲設定一つを使って退出。","検証：音楽時計、入力切替、結果とリトライ時の後処理。"],
      ["OW3a","一体のメンバーに会う","風船一つの予告から、照準、取り合い、休止へ。","検証：命中の一意な処理と、相手の意図の伝わり方。"],
      ["OW3b","二体の連携を作る","二体目を加え、役割分担、交代行動、読める妨害を一つ検討。","検証：読めない同時攻撃、判定時刻の変更、通路封鎖を避ける。"],
      ["OW4","最初の音楽を持ち帰る","埠頭の挑戦、音楽レイヤー、広場合奏に、手作りの景色と一つの目に見える達成表現を加えます。","検証：リトライ時の報酬重複防止、楽句境界での合流、音楽切替。"],
      ["OW5","街と合奏を育てる","昼の体験を確認後、街区、一つのルールの短い挑戦、任意の寄り道、個性の装飾を個別に研究。","確認：体験の違い、合奏の聞きやすさ、負荷と視認性。"]
    ],
    deferred: "まず昼の道一本、舞台一つ、集団の基本体験を完成させます。夜の音楽祭、大人数の同時行動、大都市、自動生成マップ、オンライン対戦、クラウド生成音楽は後の構想です。段階は公開日ではありません。",
    choicesTitle: "三つの言葉を、実際の遊びで説明。", choicesIntro: "以下の方向を選択済みです。相手の人数、報酬の条件、進捗の保存方法は、小さな試作を通して一緒に決めます。",
    choices: [
      ["いたずら集団：風船を競うキャラクター", "ゲームが動かす相手が、次に狙う風船を合図。同じ風船を競うか別の目標にするか、自分で選べます。一体から始め、徐々に連携を作ります。"],
      ["二つの視点：散歩では道、演奏では目標", "三人称とは、自分のキャラクターと道を後ろから見る視点。開始後は舞台を安定して映し、風船を狙うことに集中。終わると散歩へ戻ります。"],
      ["音楽レイヤー：広場に音を一つ足す", "太鼓、旋律、低音が重なる合奏を想像してください。一層持ち帰るたびに音が増え、例えば太鼓の次に庭園の旋律が加わります。割り当ては今後決めます。"]
    ],
    roleTitle: "三つの役割候補を見る",
    roleIntro: "名前とルールは提案で、初版に三体を作る約束ではありません。基本の一体を検証してから各案を研究します。",
    roles: [
      ["リズムの挑発役","短い風船の楽句を見せ、プレイヤーが応答します。専用の問答譜面が必要で、演奏途中に譜面を変えません。"],
      ["風船の運び屋","任意の探索課題で風船を運び、プレイヤーは経路を観察して近道を選びます。舞台中は追跡を止めます。"],
      ["舞台リーダー","長押し風船など、一つの変更ルールを開始前に告知。将来の譜面・入力研究であり、進行中の挑戦は変えません。"]
    ],
    artNav: "美術の方向性",
    artTitle: "陽光の音楽祭、手作りの街。",
    artIntro: "A＋Cが確定。暖かなアトリエを、風船と音楽が暮らしにつながる街へ広げます。まず広場の一角、道一本、舞台一つに適用。設計案であり、実装済みの景色ではありません。",
    artRules: [
      ["暖かな背景、明確な目標","クリーム色の建物と日光に、青緑とコーラルの差し色。背景を穏やかに保ち、風船や操作の目印を色の対比、形、アイコンで伝えます。"],
      ["つやと手作りの質感","つやのある風船に、マットな地面、紙ポスター、段ボール看板、布旗、木の舞台。膨らみ、弾み、しぼみの反応は短く、次の目標を隠しません。"],
      ["音楽が街の文化になる","空気入れ、絵の具バケツ、ばち、メトロノームから独自の道具を設計。街区ごとにバンドやポスターを考え、風船の模様、服、演奏動作は後の個性表現に。"]
    ],
    artLater: "夜の音楽祭は後に検討。まず昼の街区を確認し、夜景の照明、目標の見やすさ、音楽の切替を別途研究します。昼夜サイクルや公開日は未定です。",
    sourcesTitle: "将来の計画と、現在の体験版。", sourceDesign: "ゲーム設計全文（英語）", sourceRoadmap: "開発ロードマップ（英語）",
    originality: "『スプラトゥーン』から、遊びと音楽、暮らしの文化をつなぐ方法を学びます。マップ、キャラクター、UI、美術、音楽は独自に設計し、設計文書に公式の参考資料を記録します。",
    footer: "また歩きたくなる、小さな道から。"
  }
};
