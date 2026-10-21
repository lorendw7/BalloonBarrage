# Interface V2: assets and learner integration

## October 19 author credit

StartScreen now presents the bottom-right author credit as a rounded warm-cream plaque with a fine brown outline, a muted teal accent, a small `CREATED BY` heading and a larger `何单东 Shandong He` name. At the Canvas reference size, the plaque is 320 × 82 with 32 units of right margin and 30 units of bottom margin. All four presentation objects ignore raycasts. Existing author Text/component IDs and the prefab GUID are preserved. A small renamed Noto CJK font subset covers printable ASCII and the exact name characters; its complete OFL notice and pinned provenance are bundled in `Fonts/AuthorCredit`. This is author-credit coverage only, not general Chinese/Japanese localization.

Isolated Unity 6000.0.77f1 native checks passed glyph availability, both text blocks fitting within the plaque, viewport bounds, no overlap with the footer/buttons, and disabled raycast targets at 720p, 1080p, 1440p, 4:3 and ultrawide. Production-scene inheritance and the retained Start/Quit callbacks also passed. `Previews/Start_AuthorCredit_1080p.png` is the visually reviewed native layout preview. Evidence is ignored under `.local-backups/author-credit-polish-20261019/unity.log`. Preparation never saves the production scene; learner gameplay source hashes remained unchanged. This is layout acceptance, not a standalone gameplay or release test.

The new pack is in `Assets/StudioArtPack/InterfaceV2`. Open `InterfaceGallery.unity` to inspect the layouts. The production gameplay scene is still `Assets/Scenes/PrototypeScene.unity`.

**Status:** StartScreen, PauseScreen, GameplayHUD and ResultsScreen are connected in PrototypeScene. Start/Resume/Quit/Play Again callbacks and live score/time/final-score Text values are saved; the old OnGUI display is removed. Isolated checks cover the Canvas flow, and the learner reports pause, HUD and new results playtests passed. PlayerShooter now has release/pointer guards, and the scene disables legacy PaintGunVfx. Final physical-input acceptance remains pending; prepared gamepad actions are not connected to gameplay. See the [validation record](../Project/VALIDATION.md) for evidence and limits. Do not add the gallery scene to the game build as a playable level.

## Aim reticle handoff

Prepared on October 16: `Assets/StudioArtPack/InterfaceV2/Reticle/AimReticle.prefab` is now a hidden child of PrototypeScene's GameplayHUD. It has centered anchors/pivot, local scale (1, 1, 1), position (0, 0), and a 36 x 36 reference rectangle. Four small cream UI Image bars with dark teal Outline components leave the center open. All Raycast Target flags are off. There is no texture, new Canvas, collider, camera, runtime aiming script or AI-generated bitmap; the shapes are original native Unity UI geometry and remain resolution-independent. Keep the existing overlay HUD/scaler configuration.

The learner's PlayerShooter aims with Mouse.current.position and ScreenPointToRay. Its serialized RectTransform field `aimReticle`, state gating and screen-position following are now saved and connected, with learner-reported following/pause-hide acceptance. This presentation block runs at the start of the existing Update, before its mouse early return. With this Screen Space Overlay HUD, Transform.position accepts the mouse pixel coordinates directly; do not assign them to anchoredPosition or divide by the scaler. A future camera/world-space HUD would need explicit coordinate conversion.

After saving/compiling the field, use **Tools > BalloonBarrage > Connect Aim Reticle** and save the scene. The editor helper assigns only the actual scene RectTransform, preserves an existing different reference, and supports Undo through SerializedObject. It logs a warning if the learner field does not exist and does not write gameplay. Do not drag the Project prefab into a scene-object field. Reload the externally prepared scene first; preserve any unsaved work before reloading.

Following, pause hiding and system-pointer hiding/restoration have creator-reported acceptance. Saved code assigns Cursor.visible = !showReticle outside the nullable-reference block and retains the guarded active/position update. RestorePointer shows the system pointer and hides the reticle on disable and application-focus loss. Normal Update restores gameplay state after focus returns. Add `aimReticle != null` to showReticle as a missing-reference fallback. Do not lock the cursor because this mode uses absolute mouse coordinates. Independent standalone title/pause/results/retry, resize, focus and quit acceptance remains part of the build gate. Static authoring previews show sample HUD numbers rather than a live song/playtest: [1080p](../../Assets/StudioArtPack/InterfaceV2/Reticle/Previews/Reticle_1080p.png), [1440p](../../Assets/StudioArtPack/InterfaceV2/Reticle/Previews/Reticle_1440p.png).

`StudioReticleBuilder` builds missing assets only. Its scene setup/render checks require an isolated batch project; the production menu does not automatically open/save scenes. No existing gameplay source or original UI prefab was replaced.

## On-beat feedback handoff

The October 15 feedback assets are in `Assets/StudioArtPack/RhythmFeedback`. `OnBeatGlow.prefab` combines an expanding golden ring with twelve small sparkles. It uses an original procedural URP additive shader, without a texture download, realtime light, collider or runtime script. Visible particles last at most 0.42 seconds; emission is non-looping and uses scaled time so pause freezes the effect. Bloom is not required. Asset rendering and particle checks do not establish exhibition performance or gameplay integration.

The saved Balloon onBeatGlowPrefab reference is connected on the actual PlayableBalloon used by the spawner. Its positive-bonus impact branch instantiates the glow at effectPosition and schedules destruction after one second. Keep the existing pigment burst reference. This branch follows the accepted shot/target-id check and the balloon's one-hit guard; firing alone must not show the reward effect. Current nesting also requires the existing paint-burst reference to remain assigned.

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

`Prefabs/RhythmCue.prefab` is the passive UI plaque used by the saved countdown. It reuses the existing cream/wood sprite and bundled Lato Bold, with a centered `BeatCueText` child initially displaying `READY`. The root is anchored bottom-center at (0, 80), size 400 x 84 in the existing reference canvas; text is 360 x 56 at 30 points. Both graphics have Raycast Target off, so the plaque does not block shooting. It has no scripts or Canvas of its own and introduces no judgement, timer or hit logic.

The saved instance is connected under GameplayHUD at (0, 80), with bottom-center anchors/pivot and unit scale. RhythmChartReader.UpdateBeatCue uses a separate nextCueEventIndex driven by original chart event times: NEXT shows remaining song-clock seconds with one decimal, and ENDING covers the song tail. Keep the cue index separate from the preview-spawn index, which advances 0.8 seconds early. The creator reports countdown acceptance; latest whole-song/pause/retry regression remains pending. See [Validation](../Project/VALIDATION.md).

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

## Maintain the connected interface

Use the connection map above to repair a missing reference; do not add duplicate Canvas roots, EventSystems or button listeners. Assign objects from the scene Hierarchy rather than their Project prefab assets. Start/Quit, Resume/Quit and Play Again already target the scene controller.

Keep score and time as live Text values: score/final score use `ToString("D6")`, while time displays ceiling-rounded remaining seconds as minutes and seconds. The initial timer comes from the complete song (about 02:04 when rounded), rather than the superseded 90-second lesson. EndRound captures the score before writing ResultsScreen. Retry reloads the scene, returns to Start and resets score, song origin and display state. Do not restore the removed OnGUI labels.

For visual adjustments, open InterfaceGallery or Prefab Mode, edit Label/ControlsText rather than the PNG, and retain sliced button frames. Gallery buttons have no production callbacks. Verify text/layout at supported aspect ratios before saving changes to the game scene.

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


## October 17 best-score results row

The existing ResultsScreen prefab now includes Dialog/BestScoreRow with a BEST SCORE caption and a BestScoreValue legacy Text. It uses the existing dynamic Lato Bold font, a centered 350 × 26 row at (0, -64), right-aligned size-21 record value and nonblocking raycast settings. The saved `------` is a placeholder. The learner's best-score read/higher-only save are saved; the new Text field and display assignment remain learner work in the [Best-score lesson](../Design/BEST_SCORE_LESSON.md).

The [native preview](../../Assets/StudioArtPack/InterfaceV2/Previews/Results_BestScore_1080p.png) uses illustrative scores. Five resolution checks passed fit, viewport and nonoverlap, and the production scene retained its retry callback. Original prefab object ids, controls and references remain intact; only the Dialog child list and new presentation objects were added. No new raster generation, font or external download was introduced, and the original recorded artwork prompts and font license remain unchanged.
