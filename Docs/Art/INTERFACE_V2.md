# Interface V2: assets and learner integration

The new pack is in `Assets/StudioArtPack/InterfaceV2`. Open `InterfaceGallery.unity` to inspect the layouts. The production gameplay scene is still `Assets/Scenes/PrototypeScene.unity`.

**Status:** StartScreen and PauseScreen are instantiated in PrototypeScene with saved controller references and Start, Resume and Quit callbacks. Isolated method-driven checks pass for start, pause/resume and retry; the learner also reports a successful pause playtest. Results, score and time still use OnGUI. GameplayHUD and ResultsScreen remain presentation assets with sample text; their gameplay integration is pending. PlayerShooter's release/pointer checks are not saved. Do not add the gallery scene to the game build as a playable level.

## What changed

Three wall decorations in PrototypeScene are smaller and closer to the outer edges, leaving the central aim area quieter. The subsequent RoomFinishV7 pass adds warm oak flooring, cream walls and an ivory ceiling; see the material instructions in [Studio art pack](STUDIO_ART_PACK.md). Camera, lights, target placement, collisions, round duration and learner scripts were not changed in this art pass.

The V2 art uses cream paper, walnut frames, coral primary actions and teal resume actions. Every label is editable Unity text. The original V1 assets remain available; their exact prompts are retained in [Menu assets](MENU_ASSETS.md).

## Ready-to-use files

| File | Purpose |
|---|---|
| Textures/StartBackground_v2.png | Opaque 1672 × 941 generated background; fills the start screen |
| Textures/InterfaceAtlas_v2.png | Original transparent 1254 × 1254 generated sheet, nine named sprites |
| Prefabs/StartScreen.prefab | Title, short instructions, Start and Quit |
| Prefabs/PauseScreen.prefab | Dimmed room, Resume and Quit |
| Prefabs/ResultsScreen.prefab | Final score and Play Again |
| Prefabs/InstructionsScreen.prefab | Optional separate help layout with Back; navigation is not implemented |
| Prefabs/GameplayHUD.prefab | Top-left score and top-right remaining time |
| InterfaceGallery.unity | Presentation sandbox; only StartScreen is initially active |
| Previews | Unity camera renders at 720p and 1080p, plus start at 4:3 |

In the gallery Hierarchy, enable one screen at a time. Pause/results/help show a plain dark background there. Their saved previews show the actual studio behind them. In Play Mode the buttons provide hover/pressed feedback, but their clicks have no game action until you wire them.

## Sprite roles and scaling

Expand InterfaceAtlas_v2 in the Project window to see the slices.

| Sprite | Use |
|---|---|
| Button_Coral | Start and Play Again |
| Button_Teal | Resume |
| Button_Cream | Quit and Back |
| Panel_Paper | Help, pause and results containers |
| Plaque_Score | Score and final score |
| Plaque_Timer | Remaining time |
| Icon_Play, Icon_Pause, Icon_Quit | Optional button or heading decoration |

Buttons, panels and plaques have nine-slice borders: set an Image's Type to **Sliced** and resize its RectTransform. Keep icons **Simple**, with Preserve Aspect enabled. A tiny icon should accompany a text label rather than replacing it.

Import settings are already saved: Sprite, Multiple for the atlas, input alpha, no mipmaps, Clamp, maximum 2048, uncompressed. The actual generated dimensions differ from the requested dimensions; slicing uses the actual pixels. Keep each PNG with its .meta file. Do not run a grid slicer over the atlas: asset heights differ.

Every prefab has a Screen Space Overlay Canvas, Canvas Scaler set to Scale With Screen Size, reference 1600 × 900 and match 0.5. The start background uses Envelope Parent to fill other aspect ratios. HUD plaques anchor to the upper corners. Most decorative Images and all Text objects have Raycast Target off; buttons and modal dimmers retain it.

Labels currently use Unity's built-in LegacyRuntime font through `UnityEngine.UI.Text`. This is deliberate so the examples need no font download or TMP import. A Chinese/Japanese version needs a suitable font and a fresh layout check; see [Localization](LOCALIZATION.md).

## Lesson 1: inspect and customize the start screen

1. Save your current scene, then open InterfaceGallery.
2. Select StartScreen/StartButton/Label. Change the **Text** field, not the PNG.
3. Select StartButton. Inspect **Source Image**, **Image Type: Sliced**, and Button **Transition: Color Tint**.
4. Inspect StartScreen/ControlsPanel/ControlsText. Instructions are live text.
5. Preview at 1280 × 720 and 1920 × 1080. Keep the title inside the center region.
6. Open the StartScreen prefab in Prefab Mode to make a reusable change, or use a prefab variant for experiments.

Acceptance: title and instructions remain readable; Start and Quit retain their outlines when resized. Buttons have no callbacks at this stage.

## Lesson 2: connect the existing start and quit actions

Do this in a duplicate of PrototypeScene first. The following steps are learner work; they have not been applied to Assets/Scripts.

1. Drag StartScreen.prefab into the scene as a root object. It already contains a Canvas: do not put it inside another Canvas.
2. Create one EventSystem if none exists. Use **Input System UI Input Module**, with its default UI actions assigned. Keep only one active EventSystem.
3. Expose the existing no-argument BeginRound and QuitGame methods for the Inspector. Change their access modifier from private to public; retain their existing bodies. Guard BeginRound against a second start while a round is already active.
4. On StartScreen/StartButton, add one **On Click()** listener. Drag the scene object containing RoundController, then choose **RoundController → BeginRound()**.
5. Wire QuitButton to **RoundController → QuitGame()**. Quit leaves Play Mode in the Editor and exits the standalone application.
6. Add a serialized GameObject reference for startScreen to RoundController. Assign the scene's StartScreen instance. Hide it when BeginRound succeeds; show it when the scene initializes.
7. Remove the old start-menu drawing branch from OnGUI only after the Canvas version is connected. Do not render both menus at once.

Private methods do not appear in Button's event dropdown. Drag the **scene component**, not the C# file. A Button listener does not automatically hide a Canvas or advance a round.

Acceptance: the countdown, spawning and scoring wait for Start; the start screen disappears after one click; Quit works; Play Again returns to the start screen as it already does. Check that the click used on a UI button does not fire a projectile into the room. When introducing Canvas UI, block gameplay mouse input while the pointer is over an interactive UI element (EventSystem.current.IsPointerOverGameObject), while preserving the existing play/pause gates.

## Lesson 3: wire pause and results

Add PauseScreen, ResultsScreen and GameplayHUD as separate root prefabs. Initially enable StartScreen only. Wire references to the actual scene instances.

| State | Start | HUD | Pause | Results |
|---|---|---|---|---|
| Waiting for Start | On | Off | Off | Off |
| Playing | Off | On | Off | Off |
| Paused | Off | On | On | Off |
| Finished | Off | Off | Off | On |

- Keep the existing Esc handling and TogglePause logic. Update panel visibility whenever the state changes.
- PauseScreen/Dialog/ResumeButton should call a public ResumeRound wrapper that calls TogglePause only when IsPlaying and IsPaused are both true. Repeated clicks should not pause again.
- PauseScreen/Dialog/QuitButton uses the same QuitGame method.
- ResultsScreen/PlayAgainButton calls the existing RestartRound method after making it public. It reloads to the start screen; it does not immediately begin another round.
- InstructionsScreen is an optional visual layout. Its BackButton has no corresponding existing controller action. Keep it inactive until you implement help navigation. The StartScreen already includes the essential instructions.
- After each replacement is working, remove the corresponding old OnGUI branch to avoid duplicate controls.

Do not put RoundController on an object that is disabled to hide a panel. Hide the UI roots only. Time.timeScale must remain zero during pause, but Unity UI clicks must still work.

## Lesson 4: show live score and time

These are **Text** components, so use `using UnityEngine.UI;` in your learner-owned script. Introduce serialized Text references and assign:

| Controller reference | Hierarchy object |
|---|---|
| scoreText | GameplayHUD/ScorePlaque/ScoreValue |
| timeText | GameplayHUD/TimerPlaque/TimeValue |
| finalScoreText | ResultsScreen/Dialog/FinalScorePlaque/FinalScoreValue |

Read the existing values; do not create a second score counter:

```csharp
scoreText.text = Score.Value.ToString("D6");
int seconds = Mathf.CeilToInt(remainingTime);
timeText.text = $"{seconds / 60:00}:{seconds % 60:00}";
finalScoreText.text = finalScore.ToString("D6");
```

Put final-score assignment in EndRound **after** finalScore is captured. Refresh the HUD at initialization and while playing; the existing pause early return naturally holds the display. Do not place the first HUD refresh only after a playing-state early return, or the initial values may retain the sample text. Replace or remove Score.OnGUI and the timer GUI.Label once the new display works. Keep the score limit/layout in mind if later gameplay can exceed six digits.

For the current incremental HUD lesson, the learner can use a single LateUpdate method in RoundController: set GameplayHUD active from IsPlaying and refresh score/time from the existing values every frame. Start has already initialized those values before LateUpdate. Pause holds remainingTime and blocks score changes, so the display stays stable. Keep OnGUI results until the separate results lesson.

Acceptance: new game displays 000000 and 01:30; a hit changes score once; pause freezes time; the result equals the captured score; replay resets score and returns to Start.

## Visual checks before calling integration complete

- No duplicate OnGUI and Canvas labels; only the intended state panels are visible.
- Check pointer clicks, keyboard navigation, focus, held-fire input and pause at both target resolutions.
- Menu controls remain clickable at timeScale zero.
- Read the room through the paused overlay; the aiming area stays unobstructed during gameplay.
- Verify start/help at a narrower 4:3 window if you support resizing. Wider layouts and translated fonts need their own checks.
- Verify Quit in a standalone development build as well as the Editor.

The delivered camera renders and text-fit checks verify presentation. They do not prove these integration steps or a new playable build. See [Validation](../Project/VALIDATION.md) and [Roadmap](../Design/ROADMAP.md).

## Maintenance and provenance

`Assets/StudioArtPack/Editor/StudioInterfaceBuilder.cs` imports the sheet and builds the initial prefabs/gallery. Its Build command refuses to replace existing prefabs. Edit delivered assets or create variants to preserve manual work. CaptureBatch is an isolated-project batch utility; it opens PrototypeScene and creates presentation previews without saving changes to that scene.

Both PNGs were produced with the built-in image generation tool on October 11, 2026. They are project-created AI artwork. Original bytes and alpha are retained; only Unity import metadata defines slices. The previews are rendered from actual Unity Canvas objects, not generated mockups. [Credits](../../CREDITS.md) retains separate third-party attribution.

### Exact generation prompt: InterfaceAtlas_v2

```text
Use case: stylized-concept. Asset type: production game UI sprite atlas on transparent alpha. Create exactly NINE separate handcrafted studio-game interface assets in a strict 3-column by 3-row grid on a square 1536x1536 canvas, each 512x512 cell separated by clear transparent gutters. Every asset completely within its own cell with at least 40 pixels padding. Row 1: coral red wide rounded rectangular button, deep teal wide rounded rectangular button, ivory wide rounded rectangular button. All three identical shape and size, about 420x140 pixels, centered in cell, subtle bevel and painted enamel on wood, quiet empty center. Row 2: square ivory paper dialog with thin dark walnut frame (420x420), wide cream score plaque with a tiny teal paint dab at left edge (420x160), wide cream timer plaque with a tiny golden paint dab at left edge (420x160). Row 3: ivory play triangle icon, ivory pause double-bar icon, ivory exit-door with right arrow icon; each centered, crisp solid silhouette about 180x180, subtle dark teal outline, legible at small size. Warm contemporary artisan studio style, restrained fine grain, refined tactile materials, shallow edge highlights. Palette cream #F4E6D0, coral #CE6B58, deep teal #287E80, walnut #70503A. Absolutely no text, letters, numbers, logos or watermark. No background, no ambient colored haze, no drop shadows outside silhouettes, no checkerboard drawn. Orthographic front view, no perspective. This is a usable separated sprite sheet, not a mockup. Leave centers blank for live Unity text.
```

### Exact generation prompt: StartBackground_v2

```text
Use case: stylized-concept. Asset type: full-bleed landscape 16:9 production main-menu background for BalloonBarrage, no interface baked in. An inviting contemporary artisan balloon-painting studio, refined hand-painted 3D illustration, warm daylight from a tall left window, matte cream plaster, honey oak and walnut furniture, restrained deep teal cabinets and coral paint accents. Straight-on camera, credible material depth and soft contact shadows. A few shiny teal and coral pear-shaped balloons on the far left, thin strings; brushes, ceramic paint pots and folded linen on a low left desk; a narrow plant and framed abstract balloon print at the far right. Delicate painted splashes only at peripheral corners. The middle 60 percent is a calm softly illuminated pale plaster wall, entirely unobstructed for a live title, instructions and two buttons. Keep central empty space tall and wide from 12 to 88 percent height. Decorative props hug the bottom and outer side margins. Sophisticated cozy indie game atmosphere, strong clean silhouettes, low noise, subtle grain, not childish, no heavy outlines. Palette ivory #F4E6D0, walnut #70503A, teal #287E80, muted coral #CE6B58, tiny brass highlights. NO text, letters, numbers, typography, logos, watermarks, GUI, button shapes, frames around the canvas, or people. Opaque background. This is artwork to use directly behind editable Unity UI.
```

