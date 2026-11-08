// Labels for saved scenery; combat and traversal remain future learner work.
export const harborStrings = {
  "zh-Hans": {
    title: "更明显的起伏，更自然的绕行",
    intro: "港湾继续细化地形：高处与低地之间有更明显的起伏，弯曲小径顺着坡面展开。房屋之间留出巷道，独立掩体周围留出绕行空间，让街区不只是一条笔直的路。",
    tag: "西部港湾 / 街区与地形", mapLabel: "西部港湾",
    overviewAlt: "Unity 原生港湾全景：更明显的高低起伏、弯曲小径、房屋间巷道和可绕行掩体",
    surfaceTitle: "从屋顶到街面，让材质说话",
    surfaceBody: "Kenney City Kit 的住宅与商业建筑保留原有模型和色板贴图。Poly Haven 的灰泥、屋瓦、木板和石铺素材，与项目原创的程序化表面纹理共同增加近看细节。模型、下载贴图与原创纹理分别记录来源。",
    scope: "这是静态地形与路线布局准备。坡面、小径、房屋间巷道和掩体绕行空间已布置；尚未实现角色移动、战斗、敌人 AI 或破坏。支撑、净空与连通检查不等于角色操控、战斗平衡或人工游玩验收。",
    sourceLabel: "建筑与表面素材来源",
    terrainTitle: "从山脊，到巷道与掩体之间",
    terrainIntro: "不同宽度的路径连接坡地与房屋，每组掩体两侧都留出自然地面的绕行空间。",
    terrainMetricLabels: ["港湾地表高差", "房屋间巷道", "掩体绕行路线"],
    terrainViews: [
      ["山脊与草坡", "更强的地形起伏", "更高的山脊与低地之间，草坡以连续起伏连接；小径沿坡面转折，保留与街路的连接。", "港湾山脊近景，连续草坡与弯曲小径形成明显高低变化"],
      ["院落之间的巷道", "建筑间的侧向通路", "房屋之间的外部巷道连接街路与院落边缘，让建筑周围有更多方向的通路。房屋内部仍未开放。", "两排港湾建筑之间的院落巷道及与街路相接的路径"],
      ["掩体两侧的绕行", "分段掩体 / 独立侧路", "分段石墙与岩石之间留出空隙，两侧的空地提供绕行空间。这是静态空间布局，尚未加入敌人、武器或战斗规则。", "石墙和岩石掩体旁的两侧绕行路径，通道保持可辨识"]
    ],
    views: [
      ["港湾街道", "屋檐 / 店面 / 宽阔街路", "不同屋顶、窗户与门廊延续街道的建筑变化。房屋之间的巷道接入街路，为未来探索保留侧向通道；不代表室内空间已经开放。", "港湾街道近景，不同轮廓的住宅与商业建筑沿宽路排列"],
      ["北岸高台", "抬升地形 / 缓坡连接", "更明显的地势起伏与沿坡弯曲的小径形成新的景深。高处与低地保持路径连接；这里展示静态几何，角色移动尚未实现，坡面通行也未经人工游玩验证。", "高台街道、坡道和有表面细节的建筑形成北岸高低变化"],
      ["下沉花园", "低地 / 掩体 / 多向通路", "干燥的低地盆地与周围坡面形成静态布局研究。独立掩体两侧与后方留出通行空间，曲线小径提供绕行方向；尚未验证战斗与敌人行为。", "下沉花园与周围坡面、低矮掩体和不同方向的通路"],
      ["连接海岸的桥", "两条接入路线 / 海面视野", "新的桥梁将西岸接回原有海岸，从主街与南部花园两侧进入街区，让地图结构保持连贯。", "跨过青绿海面的新桥与西岸建筑，桥边护栏和路线清楚可见"]
    ]
  },
  ja: {
    title: "起伏を深く、道をしなやかに",
    intro: "港の高低差をよりはっきりさせ、斜面に沿う曲がった小道を整えています。建物の間には路地を、独立した遮蔽物の周囲には回り道を設け、一本の直線だけではない街並みにしました。",
    tag: "西の港 / 街並みと地形", mapLabel: "西の港",
    overviewAlt: "Unity の港の全景。はっきりした高低差、曲がる小道、建物の間の路地、回り道のある遮蔽物",
    surfaceTitle: "屋根から街路まで、素材の表情を",
    surfaceBody: "Kenney City Kit の住宅と商業建物は、元の形状とパレットテクスチャを保持しています。Poly Haven の漆喰、屋根瓦、板材、石畳に、プロジェクト独自のプロシージャル表面模様を組み合わせました。モデル、取得した画像、オリジナルの模様は別々に出典を記録しています。",
    scope: "現在は静的な地形と経路の配置準備です。斜面、小道、建物の間の路地、遮蔽物の回り道を配置していますが、キャラクター移動、戦闘、敵AI、破壊は未実装です。支持面・通行空間・接続の確認は、操作感や戦闘バランス、人によるプレイの検証ではありません。",
    sourceLabel: "建物と表面素材の出典",
    terrainTitle: "尾根から、路地と遮蔽物の間へ",
    terrainIntro: "幅の異なる道が斜面と建物をつなぎ、どの遮蔽物の両側にも、自然の地面を通って回り込める空間を残しています。",
    terrainMetricLabels: ["港の地表の高低差", "建物の間の路地", "遮蔽物の迂回路"],
    terrainViews: [
      ["尾根と草の斜面", "はっきりした起伏", "高い尾根から低地へ、草の斜面が連続した起伏をつくります。小道は斜面に沿って曲がり、街路への接続を保ちます。", "港の尾根の近景。草の斜面と曲がる小道に明瞭な高低差が見える"],
      ["中庭の間の路地", "建物の間の側道", "建物の外側の路地が街路と中庭の縁につながり、周囲に複数方向の通路を残します。建物内部への入場は未実装です。", "港の建物に挟まれた中庭の路地と街路への接続"],
      ["遮蔽物の両側を回る道", "分かれた遮蔽物 / 側道", "石壁と岩の間に隙間を残し、両側に回り込むための空間を設けました。静的な空間の構成で、敵、武器、戦闘ルールはまだありません。", "石壁と岩の遮蔽物の両側に、区別できる回り道が配置されている"]
    ],
    views: [
      ["港の通り", "軒先 / 店先 / 広い道", "屋根、窓、玄関の違いが街並みに変化をつくります。建物の間の路地を通りにつなぎ、横から回れる空間を残しています。建物内部への入場を示すものではありません。", "広い港の通りに異なる形の住宅と商業建物が並ぶ近景"],
      ["北岸の台地", "高い地形 / 緩い坂", "はっきりした起伏と斜面に沿う曲がった小道が奥行きをつくります。高所と低地は道でつながりますが、静的な形状の表示で、キャラクター移動は未実装、坂道の通行もプレイによる検証は未実施です。", "台地の道、坂、細かな表面を持つ建物がつくる北岸の高低差"],
      ["低地の庭園", "低地 / 遮蔽物 / 複数の道", "水のない低地の盆地と周囲の斜面で静的な配置を検討しています。独立した遮蔽物の両側と後ろに通行空間を残し、曲がる小道で回り込める形にしました。戦闘や敵の振る舞いは未検証です。", "低地の庭園、周囲の斜面、低い遮蔽物、複数方向の通路"],
      ["海岸をつなぐ橋", "二つの接続路 / 海の眺め", "新しい橋が西岸と既存の海岸をつなぎます。大通りと南の庭園側から入れる構成で、地図のつながりを保ちます。", "青緑の海を渡る橋と西岸の建物。手すりと通路が見える"]
    ]
  },
  en: {
    title: "Higher ground. Curving paths. More ways around.",
    intro: "The harbor gains more pronounced rises and dips, with footpaths curving along the slopes. Alleys between buildings and space around freestanding cover give the street layout more than one straight approach.",
    tag: "WESTERN HARBOR / STREETS & TERRAIN", mapLabel: "Western Harbor",
    overviewAlt: "Native Unity harbor overview with stronger rises and dips, curved footpaths, building alleys and space around cover",
    surfaceTitle: "More to see, from rooftops to street level",
    surfaceBody: "Kenney City Kit homes and commercial buildings retain their source geometry and palette atlases. Poly Haven plaster, roof tiles, planks and paving join original procedural surface patterns. Models, downloaded textures and original patterns keep separate provenance.",
    scope: "This is static terrain and route-layout preparation. Slopes, footpaths, building alleys and space around cover are placed; character movement, combat, enemy AI and destruction are not implemented. Support, clearance and connectivity checks do not establish control feel, combat balance or human playtest acceptance.",
    sourceLabel: "Building and surface sources",
    terrainTitle: "From the ridge to the spaces between",
    terrainIntro: "Paths of different widths connect slopes and buildings, with open ground on both sides of every cover group for routes around it.",
    terrainMetricLabels: ["HARBOR GROUND HEIGHT SPAN", "BUILDING ALLEYS", "Paths around cover"],
    terrainViews: [
      ["Ridgeline and grassy slopes", "MORE PRONOUNCED CONTOURS", "Continuous grassy slopes connect the higher ridge to lower ground. Footpaths curve with the land while retaining connections to the streets.", "Native harbor ridgeline close-up with pronounced grassy contours and curving paths"],
      ["Courtyard alleys", "SIDE ROUTES BETWEEN BUILDINGS", "Exterior passages connect streets to courtyard edges, leaving approaches around the buildings. Interiors remain unavailable.", "Courtyard alley between harbor buildings with a path connecting to the street"],
      ["Passages around cover", "INTERRUPTED COVER / SIDE ROUTES", "Gaps divide masonry and rock cover, with open ground along both sides. This is static spatial preparation; enemies, weapons and combat rules are not implemented.", "Distinct side paths around interrupted masonry and rock cover in the harbor"]
    ],
    views: [
      ["Harbor street", "EAVES / SHOPFRONTS / BROAD ROUTES", "Different roofs, windows and porches retain distinct silhouettes. Alleys between buildings connect to the streets, leaving side approaches for future exploration. These are exterior routes, not accessible interiors.", "Native harbor street close-up with varied homes and commercial buildings along a broad avenue"],
      ["Northern terrace", "RAISED LAND / SLOPED APPROACH", "Stronger rises and curving paths along the slopes add depth. Routes connect higher and lower ground in the saved geometry; character movement is unimplemented, and the slopes have not been playtested.", "Raised terrace streets, slopes and textured buildings on the northern harbor"],
      ["Sunken garden", "LOW GROUND / COVER / MULTIPLE ROUTES", "A dry low basin and surrounding slopes form a static layout study. Space beside and behind freestanding cover, linked by curving paths, provides alternate approaches. Combat and enemy behavior have not been verified.", "Sunken garden with surrounding slopes, low cover and paths from several directions"],
      ["Bridges to the coast", "TWO APPROACHES / OPEN WATER", "New bridges connect the west shore to the existing coast. Approaches from the avenue and southern garden keep the expanded map connected.", "New bridge across turquoise water toward western buildings, with clear railings and a broad path"]
    ]
  }
};
