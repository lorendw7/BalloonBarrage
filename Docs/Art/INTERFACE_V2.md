# Interface V2: assets and learner integration

The new pack is in `Assets/StudioArtPack/InterfaceV2`. Open `InterfaceGallery.unity` to inspect the layouts. The production gameplay scene is still `Assets/Scenes/PrototypeScene.unity`.

**Status:** StartScreen, PauseScreen, GameplayHUD and ResultsScreen are connected in PrototypeScene. Start/Resume/Quit/Play Again callbacks and live score/time/final-score Text values are saved; the old OnGUI display is removed. Isolated checks cover the Canvas flow, and the learner reports pause, HUD and new results playtests passed. PlayerShooter now has release/pointer guards, and the scene disables legacy PaintGunVfx. Final physical-input acceptance remains pending; prepared gamepad actions are not connected to gameplay. See the [validation record](../Project/VALIDATION.md) for evidence and limits. Do not add the gallery scene to the game build as a playable level.

## On-beat feedback handoff

The October 15 feedback assets are in `Assets/StudioArtPack/RhythmFeedback`. `OnBeatGlow.prefab` combines an expanding golden ring with twelve small sparkles. It uses an original procedural URP additive shader, without a texture download, realtime light, collider or runtime script. Visible particles last at most 0.42 seconds; emission is non-looping and uses scaled time so pause freezes the effect. Bloom is not required. Asset rendering and particle checks do not establish exhibition performance or gameplay integration.

The learner owns the gameplay connection. In Balloon, add a serialized ParticleSystem reference named `onBeatGlowPrefab`. Inside the existing `bonusPoints > 0` impact branch, instantiate it at `effectPosition` with identity rotation and schedule its object for destruction after one second. Assign the new prefab on the actual PlayableBalloon prefab used by the spawner. Keep the existing pigment burst reference. This branch follows the accepted shot/target-id check and the balloon's one-hit guard; firing alone must not show the reward effect. Current nesting also requires the existing paint-burst reference to remain assigned.

`OnBeatFeedback.prefab` is a separate, initially disabled UnityEngine.UI.Text component with the literal label `ON BEAT +20`, bundled Lato Bold and Raycast Target off. Its root stays active; the saved learner receiver shows the Text component after a rewarded impact, hides it after the creator-selected 1.45 seconds of active play, freezes its timer during pause, and resets it on retry. The asset does not implement that behavior. Parent it to GameplayHUD with bottom-center anchors, local scale (1, 1, 1), anchored position (0, 168), and size 440 x 64. Keep the countdown at (0, 80) and do not reuse its Text for reward feedback.

`RhythmFeedbackBuilder` prepares missing assets only and refuses to overwrite existing feedback prefabs. Its scene-render verification is restricted to batch mode in an isolated project. The new shader/materials and particle settings were authored in code, with no new image-generation prompt or third-party image. Existing Lato licensing remains in Fonts/OFL.txt and [Credits](../../CREDITS.md). Production scene and learner source remain unchanged by asset preparation.

October 16 connection checkpoint: the learner's event receiver and pause-aware timer are saved in RhythmChartReader; the creator has confirmed its saved 1.45-second duration. The assistant added the reward prefab instance to PrototypeScene's GameplayHUD with the rectangle above and connected onBeatFeedbackText to that scene Text. A direct reference to the Project prefab asset would not display the scene label. The component is hidden initially by design; inspect its Rect Transform through the Hierarchy instance, and confirm visible behavior with an on-beat impact after reloading the updated scene. See [Validation](../Project/VALIDATION.md) for check limits and backup details.

## October 14 clarity and resolution support

The five delivered prefabs now use bundled, dynamic Lato Regular/Bold fonts. A small CanvasTextResolutionRefresh presentation component requests appropriately sized glyphs and dirties text when the canvas pixel size/scale changes or a panel is re-enabled. Existing UnityEngine.UI.Text components and their IDs remain, so the controller's score/time/final-score references and button callbacks are compatible. No learner gameplay source or scene was changed in this pass.

The reported screenshot shows soft text while the UI artwork remains sharper. An isolated 2560 x 1440 baseline render with the previous font was already clear: the exact live-editor artifact was not reproduced, and the old font was dynamic, not a fixed bitmap. This pass supplies deterministic embedded fonts and explicit resolution-change refreshing; the creator must check the originally affected Game view after import/restart.

Keep one layout with Scale With Screen Size, reference 1600 x 900 and Match Width Or Height 0.5. Reference Resolution defines design coordinates, not the maximum rendering resolution. The full-screen background uses Envelope Parent, score/time retain upper-corner anchors, and central panels retain center anchors.

- Checked render sizes: 1280 x 720, 1920 x 1080, 2560 x 1440, 1280 x 960 (4:3) and 3440 x 1440 (ultrawide). All five prefabs pass text-fit and viewport checks at each size; previews use the equivalent Canvas Scaler calculation in a camera render.
- Windows defaults now allow window resizing and use a 1920 x 1080 windowed fallback. Existing full-screen-window mode remains, using the display's native desktop size. An in-game resolution picker is not implemented.
- Editor Game-view fixed resolution and Scale are separate. 1x displays original pixels and may show scrollbars when the target is larger than the pane; use a fit zoom or Play Maximized to inspect the whole layout. Raising the target resolution does not add detail to the original background/button raster artwork.
- An isolated Play Mode check passed glyph growth at scale factors 0.8, 1.2 and 1.6, plus saved Start/HUD/Resume/results/retry behavior. A [2K preview](../../Assets/StudioArtPack/InterfaceV2/Previews/StartScreen_1440p_clarity.png) is an actual render, not a live screenshot of the creator's Editor.

Font license, pinned source URLs and hashes are in Fonts/OFL.txt and Fonts/provenance.json, with [Credits](../../CREDITS.md). Lato covers this Latin UI; Japanese/Chinese font coverage remains separate. [Unity Canvas Scaler](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/script-CanvasScaler.html) describes reference resolution and scaling.

## Rhythm cue handoff

`Prefabs/RhythmCue.prefab` is a passive UI plaque prepared for the next learner hookup. It reuses the existing cream/wood sprite and bundled Lato Bold, with a centered `BeatCueText` child initially displaying `READY`. The root is anchored bottom-center at (0, 80), size 400 x 84 in the existing reference canvas; text is 360 x 56 at 30 points. Both graphics have Raycast Target off, so the plaque does not block shooting. It has no scripts or Canvas of its own and introduces no judgement, timer or hit logic.

In Edit Mode, drag this prefab under the existing GameplayHUD root, without adding another Canvas. After parenting, explicitly set the RhythmCue instance's anchor min/max to (0.5, 0), pivot (0.5, 0.5), anchored position (0, 80), local Z 0, scale (1, 1, 1) and size 400 x 84. Hierarchy dragging can leave instance transform overrides; do not assume the prefab defaults are still effective. Add `using UnityEngine.UI;` and a serialized `Text beatCueText` field in RhythmChartReader, then drag the plaque's BeatCueText child into that field. Save the scene. At this handoff the visible READY text is a placeholder; a later learner increment drives it from the original chart event time and the same song clock. Maintain a separate cue-event index: nextEventIndex already advances at the preview-spawn time, 0.8 seconds before the beat.

Source-asset checks cover the prefab's eight object/component records, internal file references, reused sprite/font GUIDs, disabled raycasts and paired new metadata. The new plaque has not been rendered or independently imported in Unity in this pass; check READY placement in Play Mode before adding timing code. No existing HUD prefab, scene or Assets/Scripts file was changed to create it. Distinct on-beat text/flash feedback remains a later hookup from accepted impact, not from the firing log.

The creator's subsequent Game-view screenshot showed the cue clipped at the lower left. PrototypeScene had the correct GameplayHUD parent and anchors but instance overrides at anchored X -800, Y 50 and scale 0.625. At the creator's explicit request, only these five values were corrected to X 0, Y 80 and scale 1; the full pre-edit scene is backed up under ignored .local-backups. Scene comparison confirmed all other lines unchanged. Reference-canvas calculations place the corrected rectangle inside 720p/1080p/1440p, 4:3 and ultrawide viewports; this is a geometry check, not a new Unity render or live Play Mode verification. Reload the saved scene after leaving Play Mode to see the disk correction.

The subsequent learner source now implements UpdateBeatCue and calls it after the reader's pause/data guards. Its separate nextCueEventIndex advances on original event times; NEXT shows remaining song-clock seconds with one decimal, and ENDING covers the song tail after the last event. The Text reference is saved. The earlier READY-only description records the initial handoff; dynamic countdown code is now present, with live countdown/pause/retry confirmation still pending.

## Current connection map

| Scene object | Saved connection | Remaining work |
|---|---|---|
| StartScreen | startScreen; StartButton → BeginRound; QuitButton → QuitGame | Final physical-input acceptance |
| PauseScreen | pauseScreen; ResumeButton → ResumeRound; QuitButton → QuitGame | Final physical-input acceptance |
| GameplayHUD | gameplayHUD; ScoreValue → scoreText; TimeValue → timeText | Final graphical/full-round acceptance |
| ResultsScreen | resultsScreen; FinalScoreValue → finalScoreText; hidden in Start and shown/written in EndRound | Final graphical/full-round acceptance |
| ResultsScreen/Dialog/PlayAgainButton | RestartRound callback to the scene controller | Final physical-input acceptance |

The finished state now shows ResultsScreen and its captured final score. The asset's sample 001250 is replaced by EndRound before the panel appears. Play Again reloads the scene and returns to Start. Unused DrawStartMenu/DrawPauseMenu methods remain in the learner source but do not render by themselves.

There is one EventSystem and one controller. The four screen roots own separate canvases; no wrapper Canvas is needed. Keep RoundController outside UI roots that can be disabled. HUD raycast targets are off. The saved shooter uses its release flag and UI-pointer guard.

The older PaintGunVfx remains in the gun prefab but is disabled on the production scene instance. Saved PlayerShooter guards control accepted projectiles and muzzle feedback. Do not add another raw-input effect listener.

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

Labels use bundled Lato Regular/Bold through UnityEngine.UI.Text; the clarity pass preserves existing Text component references. The earlier LegacyRuntime-font implementation is superseded. A Chinese/Japanese version still requires font coverage and layout checks; see [Localization](LOCALIZATION.md).

## Lesson 1: inspect and customize the start screen

1. Save your current scene, then open InterfaceGallery.
2. Select StartScreen/StartButton/Label. Change the **Text** field, not the PNG.
3. Select StartButton. Inspect **Source Image**, **Image Type: Sliced**, and Button **Transition: Color Tint**.
4. Inspect StartScreen/ControlsPanel/ControlsText. Instructions are live text.
5. Preview at 1280 × 720 and 1920 × 1080. Keep the title inside the center region.
6. Open the StartScreen prefab in Prefab Mode to make a reusable change, or use a prefab variant for experiments.

Acceptance: title and instructions remain readable; Start and Quit retain their outlines when resized. Buttons have no callbacks at this stage.

## Lesson 2: connect the existing start and quit actions

The Start/Quit baseline below is already saved in PrototypeScene. Use these steps as reference when a connection is missing; do not add duplicate roots or listeners.

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

PauseScreen, GameplayHUD and ResultsScreen are connected. The following table describes their saved visibility rules. Use the remaining lesson steps as reference rather than adding duplicate roots or listeners. Initially enable StartScreen only.

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

