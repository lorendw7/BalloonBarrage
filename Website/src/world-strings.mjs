// Public-facing translations. Design decisions live in Docs/Design/GAME_DESIGN.md.
export const worldStrings = {
  "zh-Hans": {
    nav: "阳光音乐小镇", title: "阳光音乐小镇｜BalloonBarrage 气球弹幕",
    description: "查看阳光音乐小镇的 Unity 原生静态场景：西部港湾、南部集市、立体地形、纹理建筑与多样花草。开放世界玩法仍待制作。",
    home: "返回首页", skip: "跳到正文", language: "语言", navigation: "小镇导航",
    eyebrow: "2026.11.08 · 阳光海岸扩展", headline: "更大的小镇，\n更明亮的色彩。",
    intro: "桥梁把北岸小镇、南部海岸和西部港湾连起来。市场、花田、海湾、高台与下沉花园共同丰富地图；这里展示已保存的静态 Unity 场景，角色移动和歌曲入口仍待编写。",
    status: "静态场景已制作 · 开放世界玩法待制作 · 暂无发布日期", updated: "场景与网站更新于 2026.11.08",
    mapNav: "小镇有哪些地方", loopNav: "一局怎么玩", aiNav: "捣蛋对手", planNav: "开发顺序", choicesNav: "已选定的玩法",
    selected: "想象你第一次来到小镇", decisions: ["在广场认识道路，试着打几颗练习气球。", "沿路标走到码头，看见一座小舞台。", "自己选择开始，镜头停稳，音乐响起。", "和捣蛋角色选气球、抢目标，完成一首歌。", "回到广场，听见新带回的鼓点加入合奏。"],
    boundary: "以上是未来玩法示例。小镇已有可编辑的静态场景，但没有玩家移动、对手 AI 或歌曲入口。当前可下载游玩的是画室音游。",
    mapTitle: "从旧小镇，到南岸与西部港湾。", mapIntro: "原有广场、码头、花园和屋顶区保留在北岸，南岸四处地标与新的西部港湾由桥梁和路线连接。高低差与战斗场地布局仍是静态准备，下一步从一小段移动练习开始。",
    mapLabel: "地点连接示意，不代表实际尺寸。上方原生俯视图展示已保存的 Unity 场景；点击地点可了解北岸街区。",
    districtNames: ["气球广场", "鼓点码头", "旋律花园", "切分屋顶"],
    districtTags: [
      "北岸中心",
      "首个玩法练习区",
      "静态花园环路",
      "静态高台区"
    ],
    districts: [
      "折纸灯塔是回到广场的地标，路牌、长椅与画室外立面围绕清晰通路布置。角色模型用于比例参考，练习气球与出生逻辑尚待接入。",
      "宽阔步道通往鼓棚舞台，已有静态地面与边界准备。一个定点歌曲挑战将作为后续小步练习；当前没有水上移动或对手。",
      "拱门、花草和池塘围出安静的步行环路，并与西侧南岸桥相连。旋律问答与点亮花园装饰仍是未来玩法提案。",
      "屋顶平台通过宽坡道连接地面，房屋为不可进入的外立面。高台已经布置；升降、跳跃与屋顶节奏玩法尚未实现。"
    ],
    scale: "当前面积来自已保存场景的静态陆地轮廓。面积与路线支撑检查不等同于实测步行时长；高低差、战斗布局和游玩节奏仍需后续控制器与人工体验验证。",
    loopTitle: "到码头玩一局，会发生什么？",
    loopIntro: "以下是尚待制作的玩法：散步时自由走、没有节奏评分；到了舞台，自己确认才开始射气球的音乐挑战。结束后恢复走路，可以回广场，也可以留下再练一次。",
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
    planTitle: "每一步，都有一个可以验证的问题。", planIntro: "更大的场景、明亮配色与素材已经准备好。先在副本里移动一朵花，熟悉场景；然后由学习者写一个小小的移动步骤，再一起检查。静态地图扩展仍属于 OW0，不代表开放世界已经可玩。",
    stages: [
      ["OW0", "准备场景与路线", "已制作并细化北岸小镇和明亮南岸扩展，港湾使用 CC0 建筑，并继续细化自然起伏、曲线小径、掩体周围通路与房屋间巷道，另有四种原创 3D 花卉；来源分别保留，原生图片展示静态场景。", "下一步先编辑一朵花；静态几何验证不代表移动、相机或玩法通过。"],
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
    artTitle: "青绿海湾，珊瑚屋顶，金色布旗。",
    artIntro: "阳光海岸与西部港湾结合明亮配色、开源建筑、细致地面和墙面、多样花境与高低地形。CC0 原始模型与贴图保留许可和哈希；原创程序化几何与纹理另记来源。",
    artRules: [
      ["鲜明颜色，清楚的路线", "草地、屋顶、水面与花朵带来更鲜艳的色彩，奶油色路径留出安静区域。路牌和地标形状与颜色共同指路。"],
      ["原始素材与派生材质分开", "使用 Kenney CC0 模型和已有 Poly Haven 表面，调色写在独立 Unity 材质中。原始文件与授权记录保留，既有石材与木纹继续提供细节。"],
      ["近景与全景一起检查", "俯视图看地图连接，集市、林地和营地近景看比例与留白。画面来自 Unity 静态场景，不是独立游戏包或玩家实录。"]
    ],
    artLater: "夜间音乐节后置：先验证白天街区，再单独研究夜景灯光、目标清晰度与音乐切换。暂不承诺昼夜循环或上线日期。",
    sourcesTitle: "静态场景、玩法计划与当前试玩分开看。", sourceDesign: "完整游戏设计（英文）", sourceRoadmap: "开发路线图（英文）",
    originality: "学习《斯普拉顿》连接玩法、音乐与生活文化的方法。项目原创场景布局和道具搭配有明确授权的 CC0 Kenney 模型与 Poly Haven 表面；素材署名、原始文件和准确生成提示词分别保留。",
    footer: "小镇已经铺开，下一步从走好一小段路开始。"
  },
  en: {
    nav: "Sunny music town", title: "Sunny music town | BalloonBarrage",
    description: "Explore native static Unity views of a larger sunny town: a western harbor, southern market, varied elevation, textured buildings and planting. Open-world gameplay remains future work.",
    home: "Back to home", skip: "Skip to content", language: "Language", navigation: "Town navigation",
    eyebrow: "2026.11.08 · Sunny coast expansion", headline: "A bigger town.\nA brighter palette.",
    intro: "Bridges connect the northern town, southern coast and western harbor. Markets, meadows, bays, terraces and a sunken garden add variety to the saved static Unity scene. Character movement and song entry remain future work.",
    status: "Static scene prepared · Gameplay still to come · No release date", updated: "Scene and website updated 2026.11.08",
    mapNav: "Places to visit", loopNav: "Playing a round", aiNav: "Rivals", planNav: "Development steps", choicesNav: "Selected play ideas",
    selected: "Imagine your first visit", decisions: ["Get your bearings in the plaza and try some practice balloons.", "Follow the signs to a small stage on the pier.", "Choose to start: the camera settles and the music begins.", "Pick balloons and contest targets with a mischievous rival.", "Return to the plaza and hear your new drum part join the ensemble."],
    boundary: "The visit above describes future play. An editable static town exists, but player movement, rival AI and song entry are not connected. Current playable downloads remain the studio rhythm game.",
    mapTitle: "From the original town to the southern coast and western harbor.", mapIntro: "The plaza, pier, garden and rooftops remain in the north, linked to four southern landmarks and the new western harbor. Elevation and combat layout remain static preparation; the first movement lesson still begins with a short route.",
    mapLabel: "A connection diagram, not a scale map. The native top-down image above shows the saved Unity scene. Select a place to learn about the northern districts.",
    districtNames: ["Balloon Plaza", "Percussion Pier", "Melody Garden", "Syncopation Rooftops"],
    districtTags: [
      "Northern hub",
      "First gameplay exercise",
      "Static garden loop",
      "Static raised terrace"
    ],
    districts: [
      "A folded-paper landmark marks the way home. Signs, benches and studio facades frame clear paths. Character models provide scale references; practice balloons and spawn behavior remain future work.",
      "A broad boardwalk reaches the drum-canopy stage, with static support and boundaries prepared. One fixed song challenge is a later exercise; water traversal and rivals are not present.",
      "Arches, planting and a pond form a quiet return loop connected to the western bridge south. Melodic call-and-response and illuminated garden rewards remain gameplay proposals.",
      "Broad ramps link the raised terrace to ground level, with non-enterable building facades. The setting is placed; lifts, jumping and rooftop rhythm challenges are not implemented."
    ],
    scale: "Area comes from the saved scene’s static land outline. Area and route support checks do not measure walking time; elevation, combat layout and pacing still need a controller and human playtests.",
    loopTitle: "What would one round at the pier feel like?",
    loopIntro: "This is the proposed gameplay: walk freely without rhythm grading, then choose to begin a balloon-shooting music challenge at a stage. Afterwards, walk home or stay for another try.",
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
      ["Keep exploring after a loss", "The initial design has no health damage, knockback or forced restart. The plan keeps a solo option and separates rival scores from solo records."]
    ],
    planTitle: "One testable question at every step.", planIntro: "The larger scene, brighter palette and reusable assets are prepared. First move one flower in a practice copy; then let the learner write one small movement step and review it together. The static expansion remains OW0, not a playable open world.",
    stages: [
      ["OW0", "Prepare the scene and routes", "The northern town and bright southern expansion are saved and refined. The western harbor combines CC0 buildings with stronger natural contours, curved paths, passages around cover and building alleys; four original 3D flowers have separate provenance. Native images show static scenery.", "Next, edit one flower. Static geometry checks do not accept movement, cameras or gameplay."],
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
    artTitle: "Turquoise bays. Coral roofs. Golden bunting.",
    artIntro: "Sunny Coast and the western harbor combine bright colors, open-license buildings, detailed ground and walls, varied flowers and elevation. CC0 models and maps retain licenses and hashes; original procedural geometry and patterns keep separate provenance.",
    artRules: [
      ["Vivid color, readable routes", "Grass, roofs, water and flowers carry stronger color while ivory paths leave quiet space. Signs and landmark silhouettes work with color to explain the route."],
      ["Keep originals separate from prepared materials", "CC0 Kenney models and existing Poly Haven surfaces use separate Unity color treatments. Original files and source licenses remain intact; stone and wood textures retain surface detail."],
      ["Review close views as well as the whole map", "The top-down view shows connections; market, grove and campsite views reveal scale and clear space. These are native static Unity renders, not standalone gameplay or human playtest captures."]
    ],
    artLater: "Night festival deferred: accept the daytime slice first, then study night lighting, target readability and music transitions separately. No day/night cycle or release date is promised.",
    sourcesTitle: "Separate static art, future play and current downloads.", sourceDesign: "Full game design", sourceRoadmap: "Development roadmap",
    originality: "Study how Splatoon connects play, music and local culture. Original project layouts and props accompany licensed CC0 Kenney models and Poly Haven surfaces. Keep attribution, original files and exact image-generation prompts with their separate records.",
    footer: "The town is taking shape. Start by walking one small route well."
  },
  ja: {
    nav: "陽光の音楽の街", title: "陽光の音楽の街 | BalloonBarrage",
    description: "陽光の音楽の町をUnityの静的画像で紹介。西の港、南の市場、高低差、表面の細かな建物、多彩な草花。オープンワールドの遊びはこれから制作します。",
    home: "ホームへ戻る", skip: "本文へ移動", language: "言語", navigation: "街のナビゲーション",
    eyebrow: "2026.11.08 · 陽光の海岸を拡張", headline: "もっと広い街へ。\nもっと明るい色へ。",
    intro: "橋が北の町、南の海岸、西の港をつなぎます。市場、花畑、湾、台地、低地の庭園で保存済みの静的なUnityシーンを広げました。キャラクター移動や曲への入口は未実装です。",
    status: "静的なシーンを準備済み · 遊びは今後制作 · 公開日未定", updated: "シーンとサイトの更新日 2026.11.08",
    mapNav: "街の場所", loopNav: "一回の遊び方", aiNav: "いたずらな相手", planNav: "開発の順序", choicesNav: "選択済みの遊び",
    selected: "初めての訪問を想像すると", decisions: ["広場で道を覚え、練習用の風船を撃ってみる。", "案内板をたどると、埠頭に小さな舞台がある。", "自分で開始を選ぶと、視点が落ち着いて曲が始まる。", "いたずらな相手と風船を選び、取り合いながら一曲遊ぶ。", "広場へ帰ると、持ち帰った太鼓の音が合奏に加わる。"],
    boundary: "上の訪問は将来の遊び方です。編集可能な静的な街はありますが、移動操作、ライバルAI、曲への入口は未接続です。今遊べる配布版はアトリエのリズムゲームです。",
    mapTitle: "元の町から、南岸と西の港へ。", mapIntro: "北岸の広場、埠頭、庭園、屋根街を保ち、南岸の四つの場所と新しい西の港を橋と道でつなぎました。高低差と戦闘場所は静的な準備で、移動の練習は短い道から始めます。",
    mapLabel: "場所のつながりを示す図で、縮尺図ではありません。上のUnity俯瞰画像が保存済みシーンです。場所を選ぶと北岸の街区を読めます。",
    districtNames: ["風船広場", "リズム埠頭", "メロディ庭園", "シンコペ屋根街"],
    districtTags: [
      "北岸の中心",
      "最初の遊びの練習区",
      "静的な庭園ルート",
      "静的な高台"
    ],
    districts: [
      "折り紙の目印が広場への帰り道を示します。案内板、ベンチ、アトリエの外壁を通路の周囲に配置。キャラクターは大きさの参考で、練習風船や出現処理は今後の作業です。",
      "広い遊歩道が太鼓の屋根のステージへ続き、静的な足場と境界を準備済みです。一曲の定点チャレンジは次の練習項目で、水上移動や相手はまだありません。",
      "アーチ、草花、池を巡る静かな道は、西側の橋で南岸へつながります。メロディの問答や庭の装飾を光らせる報酬は、将来の遊びの案です。",
      "広い坂道で地面と高台をつなぎ、入れない建物の外観を配置しました。場所はありますが、昇降機、ジャンプ、屋根のリズム遊びは未実装です。"
    ],
    scale: "面積は保存済みシーンの静的な陸地の輪郭に基づきます。面積や経路の支えの確認から歩行時間は判断できません。高低差、戦闘場所の配置、遊びのテンポは操作と実際のプレイで検証します。",
    loopTitle: "埠頭で一回遊ぶと、どうなる？",
    loopIntro: "以下は今後制作する遊びです。散歩中は自由に歩き、リズム採点はありません。舞台で自分から開始すると風船を撃つ音楽チャレンジへ。終わったら帰っても、もう一度練習しても構いません。",
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
    planTitle: "一段階に、一つの確かめる問い。", planIntro: "広いシーン、明るい配色、再利用できる素材を準備しました。まず練習用コピーで花を一つ動かし、その後、学習者が小さな移動処理を書いて一緒に確認します。静的な拡張はOW0の準備で、遊べる街の完成ではありません。",
    stages: [
      ["OW0", "シーンと道を準備する", "北岸の街と明るい南岸を保存し、植栽を細かく整えました。西の港ではCC0の建物に加え、自然な起伏、曲がる道、遮蔽物の回り道、建物の間の路地を整え、別途4種のオリジナル3D花モデルも用意。出典を分けて記録し、画像は静的な景色です。", "次は花を一つ編集。静的な形状確認は移動、カメラ、遊びの合格ではありません。"],
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
    artTitle: "青緑の湾、コーラルの屋根、金色の旗。",
    artIntro: "サニーコーストと西の港に、明るい配色、オープン素材の建物、細かな地面と壁、多彩な花、高低差を組み合わせました。CC0の原本と許諾・ハッシュ、独自の形状や模様の出典を分けて保存しています。",
    artRules: [
      ["鮮やかな色、わかりやすい道", "草地、屋根、水面、花には鮮やかさを。アイボリーの道には落ち着いた空間を残します。案内板と目印の形も色と一緒に道を伝えます。"],
      ["原本と調整用マテリアルを分ける", "CC0のKenneyモデルと既存のPoly Haven表面を使い、配色は別のUnityマテリアルで調整。元ファイルと許諾は保持し、石や木の質感も残します。"],
      ["全体と近景の両方で確認する", "俯瞰で接続を、市場や林、キャンプ場の近景で大きさと余白を確認します。画像はUnityの静的シーンで、配布ゲームや人によるプレイの記録ではありません。"]
    ],
    artLater: "夜の音楽祭は後に検討。まず昼の街区を確認し、夜景の照明、目標の見やすさ、音楽の切替を別途研究します。昼夜サイクルや公開日は未定です。",
    sourcesTitle: "静的な美術、将来の遊び、現在の体験版。", sourceDesign: "ゲーム設計全文（英語）", sourceRoadmap: "開発ロードマップ（英語）",
    originality: "『スプラトゥーン』から遊び、音楽、街の文化をつなぐ方法を学びます。独自の配置と道具に、許諾されたCC0のKenneyモデルとPoly Haven表面を組み合わせ、署名、原本、正確な画像生成プロンプトを個別に記録します。",
    footer: "街の形はできてきました。まず短い道を心地よく歩くことから。"
  }
};
