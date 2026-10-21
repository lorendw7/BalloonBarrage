# Beat cue presentation handoff

The creator selected beat-cue clarity as the next comfort issue on October 20. Current saved gameplay shows the global `NEXT` countdown via `RhythmChartReader.UpdateBeatCue`; it does not implement a target-attached timing ring. The existing firing-time matched-event judgement stays unchanged.

## October 21 paint-sticker refinement

The live `RhythmCue.prefab` now uses an original paint-game presentation: a project-owned teal starburst silhouette behind a slightly angled cream card, coral optional-bonus heading, dark countdown, small paint drops and a gold timing slash. This borrows only broad qualities such as energetic paint shapes, strong contrast and a playful offset layout. It does not copy Splatoon characters, weapon art, typography, icons, interface layout, sound or turf rules.

`BeatCuePaintStylePreparation.cs` produced and verified the pass in an isolated Unity 6000.0.77f1 project. The existing prefab GUID, `BeatCueText` component and production-scene binding are retained. Layout, text geometry, required digits and non-raycast behavior passed at 1280 × 720, 1920 × 1080 and 2560 × 1440. Native previews and the verification record live in `Assets/StudioArtPack/BeatCuePaint20261021`. This is asset/layout evidence, not a physical-input or complete-song acceptance.

`PaintBeatApproachGuide.prefab` is the matching unconnected approach-bar prototype. Its teal fill and gold endpoint communicate motion plus a fixed goal instead of relying on color alone. Its saved preview amount is demonstration data; production gameplay still shows only the text countdown until the learner completes the increments below.

![Paint-sticker live cue](../../Assets/StudioArtPack/BeatCuePaint20261021/Previews/PaintCueLive.png)

`BeatCuePreparation.cs` prepares two UI assets in an isolated project:

- The existing `InterfaceV2/Prefabs/RhythmCue.prefab` retains its BeatCueText component and GUID, with a high-opacity cream background, teal text/border and a small optional-bonus heading. Its runtime numeric text and scene binding are preserved.
- `BeatCue20261020/BeatApproachGuide.prefab` is a separate graphic prototype: a horizontal teal fill approaching a gold timing line. It is not instantiated or driven by production gameplay. Preview fill is a manually assigned demonstration, not a timing test.

After the one-variable firing-rate comparison in [Spray feel and cadence lesson](../Design/SPRAY_FEEL_LESSON.md), the learner's first cue-code increment is only a serialized `UnityEngine.UI.Image` field named `beatApproachFill` next to `beatCueText` in RhythmChartReader. Review saved compilation before adding a scene hookup or update formula. Later increments connect the graphic, drive normalized progress from the existing song clock/next-event time, and handle the intro, event transition, pause, ending and retry. Do not use an independent animation loop or a metronome unrelated to the actual chart events.

This global graphic communicates *when*, not *which balloon*. Overlapping targets can still make the target association unclear; evaluate a per-target cue as a separate learner-owned increment after the simple graphic is accepted. Colors supplement motion and the fixed endpoint, rather than serving as the only timing signal. Ordinary hits remain rewarding without following this cue.

See [Game design](../Design/GAME_DESIGN.md) for judgement rules and [Roadmap](../Design/ROADMAP.md) for scope. The control card must describe the cue that is actually integrated; it must not tell players to follow a ring that does not exist.
