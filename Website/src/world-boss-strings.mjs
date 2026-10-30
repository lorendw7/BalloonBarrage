// Public proposal copy. GAME_DESIGN.md owns behavior; ROADMAP.md owns delivery gates.
export const bossStrings = {
  "zh-Hans": {
    nav: "Boss 混斗", teaser: "新增后续计划：和 Boss＋AI 小队直接交战 →",
    title: "边走位、边瞄准，和 Boss 正面交锋。",
    intro: "已选方向：单人对抗一名 Boss 和它的 AI 小队，双方用颜料武器直接攻击彼此。比如你进入街区旁的竞技场，绕过掩体，与一台原创“扩音器机甲”交战；机甲只是造型示例，还未制作。",
    distinction: "这个模式的目标是击败敌人，不是打气球计分。普通街区的定点音游保留；Boss 战是你主动进入的另一种挑战，尚未实装。",
    steps: [
      ["01 / 找位置，互相进攻", "你边移动边瞄准 Boss 或小兵，对方也会向你射击。掩体、侧路和攻击距离让你决定往哪里走。"],
      ["02 / 看预告，避开攻击", "Boss 先抬起武器、标出攻击范围，再发动招式。你可以绕到掩体后，或离开危险区域；提示要靠形状和声音共同表达。"],
      ["03 / 抓住空当，打它本体", "Boss 一轮攻击后露出本体弱点，你选择集中攻击，或先处理靠近的小兵。音乐随阶段变化，跟拍有表现反馈，但开火不必等待节拍。"]
    ],
    outcome: "目标是完成一场交战后返回广场。音乐或装饰奖励的条件仍待设计；输了可退出或重试，不封锁逛街路线。先研究一名 Boss＋一名小兵，再考虑更多敌人。",
    rulesTitle: "展开：先把哪些规则说清楚？",
    rules: [
      ["战斗镜头", "Boss 战允许移动与瞄准同时进行，单独验证第三人称战斗镜头；普通音游舞台仍采用稳定视角。"],
      ["受击与难度", "生命、护盾、受击反馈、失败条件和敌人数还未确定。先保证攻击有预告、能躲开、能看懂谁在打你。"],
      ["颜料与音乐", "保留按住持续射击的方向。地面颜料是否影响移动或占点另行试验；音乐烘托战斗阶段，不在交战中偷偷改出招规则。"],
      ["模式边界", "这是单人对抗游戏 AI。联网多人、本地双人 Boss 战和三方混战都不是首个研究范围。"]
    ],
    techTitle: "展开：新技术怎样让交战更有趣？",
    techIntro: "先用明确规则做出可玩的对手，再比较技术是否真的改善体验。以下都是研究方向，没有接入当前试玩。",
    tech: [
      ["规则 AI：先会打、会停、会让路", "先设计巡逻、发现玩家、瞄准、开火、躲掩体和恢复的步骤，调出公平的反应时间。"],
      ["小队协同：有配合，也有空当", "让小兵分工、轮流压制，限制同时攻击的人数。敌人根据视线和听到的动静行动，不提前知道你的按键。"],
      ["强化学习：研究更自然的走位", "用 Unity ML-Agents 在离线练习场训练选掩体和移动策略，再与规则 AI 比较。只有表现稳定、速度足够时才考虑采用，并保留规则 AI 后备。"],
      ["受约束的场地变化", "后期尝试在审核过的模板里组合掩体和入口。先检查能走通、有躲避空间、目标看得清，再作为候选场地。"]
    ],
    artTitle: "先让 Boss、小兵和攻击提示一眼分清。",
    art: "后续准备原创 Boss 轮廓、武器、掩体、街区道具、贴图和音效。延续阳光音乐节与手作小镇的风格，用不同身形、动作和图标区分敌人；这些素材仍待制作。",
    order: "开发顺序：先完成基础小镇体验 → 一个可移动交战的试验场 → 一名 Boss＋一名小兵 → 难度与可读性验证 → 可选学习型 AI。没有发布日期。",
    references: "参考方法与技术依据", inspiration: "借鉴 Salmon Run 的敌群压力、分工与大敌阶段感，制作自己的战斗规则、角色和场景。"
  },
  ja: {
    nav: "ボス乱戦", teaser: "新しい将来案：ボスとAI小隊との直接対決 →",
    title: "動いて狙い、ボスと撃ち合う。",
    intro: "選択した方向は、一人でボスとそのAI小隊に挑み、絵の具の武器で互いを直接攻撃するモードです。例えば街区の脇の闘技場で、遮蔽物を回りながら独自の「拡声器メカ」と対決。この姿はまだ制作前の例です。",
    distinction: "目的は敵を倒すことで、風船の得点競争ではありません。街区の定点リズム遊びも残し、自分で入る別の挑戦として考えます。まだ未実装です。",
    steps: [
      ["01 / 場所を選び、撃ち合う", "移動しながらボスや小兵を狙い、相手も撃ち返します。遮蔽物、脇道、射程を見て進む場所を選びます。"],
      ["02 / 予告を見て、攻撃を避ける", "ボスが武器を上げ、攻撃範囲を示してから技を出します。遮蔽物へ隠れるか危険範囲を離れます。形と音の両方で予告します。"],
      ["03 / 隙を見て、本体を狙う", "攻撃後に本体の弱点が現れたら集中攻撃するか、近くの小兵を先に対処するか選択。曲は段階に合わせて変わり、発射は拍を待つ必要がありません。"]
    ],
    outcome: "一戦を終えて広場へ戻ることが目標です。音楽や飾りの報酬条件は未定。負けても退出や再挑戦ができ、散策の道は閉ざしません。最初はボス一体と小兵一体から研究します。",
    rulesTitle: "最初に決めるルールを見る",
    rules: [
      ["戦闘の視点", "移動と照準を同時に行う三人称の戦闘カメラを別途検証。通常の音楽ステージは安定した視点を保ちます。"],
      ["被弾と難しさ", "体力、盾、被弾の表現、失敗条件、敵の数は未定。まず予告が見え、避けられ、誰が撃っているかわかることを重視します。"],
      ["絵の具と音楽", "長押しで撃ち続ける方向を維持。床の絵の具が移動や陣地に影響するかは別に検討し、音楽は戦闘段階を演出します。途中で技のルールをこっそり変えません。"],
      ["最初の範囲", "一人でゲームAIに挑む案です。オンライン対戦、ローカル二人のボス戦、三陣営の乱戦は初回の研究範囲に含みません。"]
    ],
    techTitle: "新しい技術をどう遊びに役立てる？",
    techIntro: "明確なルールで遊べる相手を先に作り、技術で体験が改善するか比較します。以下は研究案で、現在の体験版には入っていません。",
    tech: [
      ["ルールAI：撃つ、休む、道を空ける", "巡回、発見、照準、射撃、遮蔽物への移動、休止の手順から、公平な反応時間を調整します。"],
      ["小隊の連携：役割と隙を作る", "小兵が役割を分けて交代で攻撃し、同時攻撃数を制限。視線や音で判断し、プレイヤーの次の入力は知りません。"],
      ["強化学習：自然な移動を研究", "Unity ML-Agentsでオフラインの練習場を使い、遮蔽物選びや移動を学習。ルールAIと比較し、安定性と速度を確認できた場合だけ採用を検討します。"],
      ["制約付きの場面変化", "後期には確認済みの型から遮蔽物や入口を組み合わせます。通路、回避空間、目標の見やすさを確認して候補にします。"]
    ],
    artTitle: "ボス、小兵、攻撃の合図を見分けやすく。",
    art: "独自のボスの姿、武器、遮蔽物、街の道具、テクスチャと効果音を今後準備します。陽光の音楽祭と手作りの街を引き継ぎ、体形、動き、アイコンで敵を区別。素材はまだ制作前です。",
    order: "順序：基本の街の体験 → 動きながら戦える試験場 → ボス一体と小兵一体 → 難度と見やすさの確認 → 任意の学習型AI。公開日は未定です。",
    references: "参考にする考え方と技術", inspiration: "Salmon Runの敵群への対処、役割分担、大きな敵との段階的な対決から学び、独自のルール、キャラクター、景色を作ります。"
  },
  en: {
    nav: "Boss brawl", teaser: "New later plan: direct combat against a Boss and AI squad →",
    title: "Move, aim and trade shots with a Boss.",
    intro: "Selected direction: one player directly fights a Boss and its AI squad with paint weapons. Imagine entering an arena beside a district, moving around cover and confronting an original loudspeaker mech. That design is an example, not a finished asset.",
    distinction: "The objective is to defeat enemies, not score balloon hits. Fixed music stages remain available; this is a separate encounter you choose to enter. It is not implemented.",
    steps: [
      ["01 / Take a position and trade shots", "Move while aiming at the Boss or its minions, who fire back. Cover, side routes and range help you choose where to go."],
      ["02 / Read the warning and evade", "The Boss raises a weapon and marks its attack area before striking. Move behind cover or leave the danger zone. Shapes and sound both communicate the warning."],
      ["03 / Use the opening to hit its body", "After an attack, a weak point on the Boss opens. Focus it or deal with nearby minions first. Music follows battle phases; firing never requires waiting for the beat."]
    ],
    outcome: "Finish an encounter and return to the plaza. Musical or decorative rewards still need conditions. A loss allows exit or retry without blocking exploration. Study one Boss and one minion before adding more enemies.",
    rulesTitle: "Explore the first rules to settle",
    rules: [
      ["Combat camera", "Test a separate third-person camera for simultaneous movement and aiming. Ordinary rhythm stages keep their stable view."],
      ["Hits and difficulty", "Health, shields, hit feedback, loss conditions and enemy counts remain open. Prioritize visible warnings, avoidable attacks and clear attacker identity."],
      ["Paint and music", "Keep held continuous fire as the direction. Ground paint affecting movement or territory needs a separate study. Music supports phases without secretly changing attack rules mid-fight."],
      ["Initial scope", "One player fights game-controlled AI. Online multiplayer, local two-player Boss combat and three-way faction battles are outside the first study."]
    ],
    techTitle: "How could new technology improve combat?",
    techIntro: "Build an enjoyable opponent with clear rules first, then compare whether a technique improves the experience. These are research ideas, not current demo features.",
    tech: [
      ["Rule-based AI: fight, rest and leave space", "Start with patrol, detection, aiming, firing, cover and recovery steps, then tune fair reaction times."],
      ["Squad coordination: roles with openings", "Assign roles and rotate pressure while limiting simultaneous attackers. Enemies use sight and sound rather than knowing future player input."],
      ["Reinforcement learning: study natural movement", "Use an offline Unity ML-Agents training arena to study cover and movement choices. Compare against rule-based AI; consider adoption only with stable behavior and acceptable speed, keeping a rules fallback."],
      ["Constrained arena variation", "Later, combine cover and entrances from reviewed templates. Check reachable routes, evasion space and target visibility before considering a layout."]
    ],
    artTitle: "Make the Boss, minions and warnings easy to distinguish.",
    art: "Prepare original Boss silhouettes, weapons, cover, town props, textures and sound effects later. Carry forward the sunny festival and handmade town through distinct shapes, movements and icons. These assets have not been made yet.",
    order: "Order: accept the basic town visit → a movement-and-combat test arena → one Boss and one minion → difficulty and readability checks → optional learned AI. No release date is committed.",
    references: "Design and technical references", inspiration: "Learn from Salmon Run's group pressure, roles and large-enemy encounters while creating original combat rules, characters and environments."
  }
};
