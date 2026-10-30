// Visitor-facing translations; survey operation is documented in Docs/Project/FESTIVAL_SURVEY.md.
export const surveyStrings = {
  "zh-Hans": {
    nav: "九大祭投票", title: "下一段音乐旅程，由你来提建议。",
    teaser: "先看懂未来玩法选项，再投票；试玩报名自愿参加。",
    intro: "面向九大祭同学的未来方向问卷，预计约 1–2 分钟。阳光音乐节＋手作小镇已经确定，夜间音乐节留到后面；投票帮助安排后续探索，不代表功能或发布日期承诺。",
    pending: "投票尚未开放", pendingBody: "Google 表单正在准备。下面可预览题目，目前没有收集昵称、邮箱或选票。",
    open: "前往 Google 表单投票", openBody: "在 Google 表单填写并提交后，回复才会被记录。本页只介绍问卷，不直接接收个人信息。",
    questionsTitle: "四道选择题，每题选一项", unsure: "还没想好",
    detailsTitle: "选项是什么意思？看小例子",
    questionHelp: "展开每题的说明再比较。例子是未来构想，不是现有功能；没有偏好可选“还没想好”，无需猜答案。",
    choiceExplanations: [
      ["重点是发现：沿街寻找气球线索和隐藏舞台，自己决定先逛哪里，再停下来演奏。", "重点是对手互动：到固定舞台听捣蛋 AI 的乐句，再用节奏回应，体验有角色参与的音乐挑战。", "“声部”就是合奏里的一层声音，例如鼓点或低音；挑战后带回广场，让小镇音乐越来越丰富。"],
      ["安静的花园与回环小路，侧重听旋律、接奏短句，适合想慢慢逛、慢慢听的同学。", "层叠屋顶与更跳跃的节奏，侧重错落拍点的音乐挑战；跳跃和升降移动仍需另外验证。", "先把现有中心广场做得更丰富，例如更多短活动与合奏变化，再扩大地图。"],
      ["它先示范一小段气球节奏，你再接奏；乐趣来自听懂并回应，而不是追着敌人跑。", "观察它搬运气球的路线，选一条近路相遇；这是可选探索任务，舞台演奏时暂停追逐。", "开局前告诉你本轮的特别规则，例如延长音气球；规则不会在演奏中突然改变。", "优先打磨没有对手打扰的演奏体验，再考虑 AI；适合更在意节奏手感的同学。"],
      ["像学生亲手张贴的活动海报和画出的店招，用纸张与笔触表现街区个性。", "像大家一起搭建的小型祭典，用布旗、木板与气球营造温暖热闹的舞台。", "把乐器轮廓融入街边道具，让小乐队成为街区标志，突出“音乐小镇”的印象。"]
    ],
    questions: [
      ["你最希望优先完善哪种体验？", ["探索街区、发现隐藏舞台", "与捣蛋 AI 进行音乐对决", "收集声部，让广场合奏"]],
      ["完成首个鼓点码头后，你更想去哪里？", ["安静的旋律花园", "活泼的切分屋顶", "活动更多的中心广场"]],
      ["哪种候选对手最有趣？", ["抢拍手：示范乐句，你来接奏", "气球搬运工：观察路线、选择近路", "舞台领队：开局前宣布挑战规则", "更想先完善单人演奏"]],
      ["你更想看到哪种手作细节？", ["纸海报与涂鸦招牌", "布旗、木舞台与气球装饰", "乐器造型道具与街头乐队"]]
    ],
    participationTitle: "昵称必填，试玩报名可选", participation: "使用昵称即可，无需真实姓名、学号或电话。投票末尾可选择是否接收后续试玩邀请：选“愿意”才填写邮箱，选“仅投票”可直接提交。可选留言请不要填写其他人的个人信息。",
    privacyTitle: "个人信息怎么使用？", privacy: "昵称用于整理反馈；自愿留下的邮箱仅用于后续试玩邀请，并在报名后 90 天内由作者手动删除。可通过 Google 表单底部的“Contact form owner（联系表单所有者）”申请撤回或删除。回复由作者管理，不公开昵称、邮箱或原始留言；若分享结果，只发布汇总票数。报名不保证名额或日期。",
    note: "问卷说明预览；正式填写入口开放前不接收报名。",
    back: "查看已确认的开放世界计划", thanks: "谢谢你一起决定小镇的下一步。"
  },
  ja: {
    nav: "九大祭アンケート", title: "次の音楽の旅に、あなたのアイデアを。",
    teaser: "遊びの候補を読んでから投票。次回テストへの登録は任意です。",
    intro: "九大祭の来場者向け、約1〜2分の将来構想アンケートです。陽光の音楽祭＋手作りの街は決定済みで、夜の音楽祭は後の構想。回答は検討の参考とし、機能や公開日を約束するものではありません。",
    pending: "投票はまだ始まっていません", pendingBody: "Googleフォームを準備中です。以下は質問のプレビューで、現在はニックネーム、メールアドレス、票を収集していません。",
    open: "Googleフォームで回答する", openBody: "Googleフォームで送信すると回答が記録されます。このページは案内のみで、個人情報を直接受け取りません。",
    questionsTitle: "四つの質問、それぞれ一つ選択", unsure: "まだわからない",
    detailsTitle: "選択肢の意味と小さな例",
    questionHelp: "説明を開いて比べてみてください。例は将来の構想で、実装済み機能ではありません。迷ったら「まだわからない」を選べます。",
    choiceExplanations: [
      ["発見を重視。風船の手がかりや隠れた舞台を探し、自分で散策順を決めてから立ち止まって演奏します。", "相手とのやり取りを重視。固定の舞台でいたずらAIの楽句を聞き、リズムで応える音楽チャレンジです。", "「声部」は合奏の音の層。打楽器や低音などを広場へ持ち帰り、街の音楽を少しずつ豊かにする構想です。"],
      ["静かな庭園と回遊路。旋律を聞いて短い楽句に応え、ゆっくり散策と音楽を楽しむ方向です。", "段差のある屋根の景色と弾むリズム。拍のずれを楽しむ課題が中心で、ジャンプや昇降移動は別途検証します。", "地図を広げる前に中央広場を充実。短い遊びや合奏の変化など、戻ってきたくなる場所を目指します。"],
      ["短い風船のリズムを先に演奏し、あなたが応答。追いかけっこより、聞いて返す楽しさを重視します。", "風船を運ぶ経路を見て近道を選ぶ、任意の探索課題。舞台で演奏している間は追跡を止めます。", "長く鳴らす風船など、その回のルールを開始前に告知。演奏中に突然ルールを変えることはありません。", "相手に邪魔されない演奏の手触りを先に磨き、AIは後で検討する選択です。"],
      ["学生が手で貼った催しのポスターや描いた店の看板。紙や筆跡で街の個性を表します。", "みんなで組み立てた小さなお祭り。布旗、木の板、風船で温かくにぎやかな舞台を作ります。", "楽器の形を街の道具に取り入れ、小さなバンドを街区の目印にして、音楽の街らしさを強めます。"]
    ],
    questions: [
      ["どの体験を優先して充実させたいですか？", ["街を探索して隠れた舞台を発見", "いたずらAIとの音楽対決", "声部を集めて広場の合奏を育てる"]],
      ["最初のリズム埠頭の後、どこへ行きたいですか？", ["穏やかなメロディ庭園", "にぎやかなシンコペ屋根街", "遊びの増えた中央広場"]],
      ["どのライバル候補が楽しそうですか？", ["リズムの挑発役：手本の楽句に応答", "風船の運び屋：経路を見て近道を選ぶ", "舞台リーダー：開始前にルールを告知", "まずソロ演奏を充実させたい"]],
      ["どの手作りの表現を見たいですか？", ["紙ポスターと落書き看板", "布旗、木の舞台、風船の飾り", "楽器の形の道具と街のバンド"]]
    ],
    participationTitle: "ニックネームは必須、テスト登録は任意", participation: "本名、学籍番号、電話番号は不要です。最後に次回テストの案内を希望するか選び、希望者だけメールアドレスを入力します。投票のみでも送信できます。自由記述に他の方の個人情報を書かないでください。",
    privacyTitle: "個人情報の使い道", privacy: "ニックネームは回答整理に、任意のメールは今後のテスト案内だけに使用し、登録から90日以内に作者が手動で削除します。撤回・削除はGoogleフォーム下部の「Contact form owner」から依頼できます。回答は作者が管理し、名前、メール、自由記述の原文は非公開。紹介する結果は集計票数のみで、登録は参加枠や日程を保証しません。",
    note: "アンケートの案内プレビューです。正式な回答受付の開始までは登録できません。",
    back: "決定済みのオープンワールド計画へ", thanks: "街の次の一歩を一緒に考えてくれて、ありがとう。"
  },
  en: {
    nav: "Festival survey", title: "Help shape the next musical journey.",
    teaser: "Read what each option means, then vote. Playtest signup is optional.",
    intro: "A 1–2 minute future-direction survey for Kyudaisai visitors. The sunny festival and handmade town are already selected; a night festival comes later. Votes guide exploration, not promised features or release dates.",
    pending: "Voting is not open yet", pendingBody: "The Google Form is being prepared. Preview the questions below; no nicknames, email addresses or votes are being collected yet.",
    open: "Vote in Google Forms", openBody: "Your response is recorded only when you submit the Google Form. This page explains the survey and does not receive personal information directly.",
    questionsTitle: "Four questions, one choice each", unsure: "Not sure yet",
    detailsTitle: "What the choices mean, with examples",
    questionHelp: "Open each explanation to compare. These examples describe future ideas, not existing features. Choose “Not sure” if you have no preference.",
    choiceExplanations: [
      ["Focus on discovery: follow balloon clues to hidden stages, choose where to wander, then stop to perform.", "Focus on a rival interaction: hear a mischievous AI's phrase at a fixed stage and answer it through rhythm.", "A musical part is one layer of the ensemble, such as drums or bass. Bring parts back from challenges so the plaza's music grows richer."],
      ["A quiet garden with looping paths, focused on listening to melodies and answering short phrases at a relaxed pace.", "Stepped rooftop scenery and livelier offbeat challenges. Jumping and lift movement still need separate investigation.", "Add short activities and ensemble changes to the existing central plaza before expanding the map."],
      ["The rival demonstrates a short balloon rhythm and you answer. The appeal is listening and responding rather than chasing an enemy.", "Watch a balloon carrier's route and take a shortcut to meet it. This exploration task is optional and pursuit pauses during stage play.", "Announces a special rule, such as a sustained balloon, before the round. Rules do not suddenly change during a performance.", "Polish the feel of performing without a rival first, then consider AI later."],
      ["Student-made event posters and hand-painted shop signs use paper textures and brush marks to give each street character.", "A small festival built together: fabric flags, wooden boards and balloons make stages feel warm and lively.", "Instrument-shaped street props and small local bands become district landmarks, making the town's musical identity visible."]
    ],
    questions: [
      ["Which experience should we develop first?", ["Explore streets and discover hidden stages", "Challenge mischievous AI in musical duels", "Collect musical parts for the plaza ensemble"]],
      ["After the first Percussion Pier, where would you like to go?", ["A peaceful Melody Garden", "Lively Syncopation Rooftops", "A central plaza with more activities"]],
      ["Which candidate rival sounds most fun?", ["Beat Teaser: answer a demonstrated phrase", "Balloon Porter: observe a route and take a shortcut", "Stage Leader: announces a rule before the round", "Develop solo performances first"]],
      ["Which handmade details would you like to see?", ["Paper posters and painted signs", "Fabric bunting, wooden stages and balloon decorations", "Instrument-shaped props and local bands"]]
    ],
    participationTitle: "Nickname required. Playtest signup optional.", participation: "Use a nickname; no real name, student ID or phone number is needed. At the end, choose whether to receive future playtest invitations. Only those who opt in enter an email; voting alone is welcome. Do not include other people's personal details in the optional comment.",
    privacyTitle: "How your information will be used", privacy: "Nicknames organize feedback; optional emails are only for future playtest invitations and will be manually deleted by the creator within 90 days of signup. Use “Contact form owner” at the bottom of the Google Form to withdraw or request deletion. The creator manages responses privately; only aggregate vote counts may be shared, never names, emails or raw comments. Signup does not guarantee a place or date.",
    note: "Survey information preview. Signup begins only when the official response link opens.",
    back: "Read the confirmed open-world plans", thanks: "Thank you for helping choose the town's next step."
  }
};
