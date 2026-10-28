# Studio expansion — October 28, 2026

This additive pack prepares seven reusable studio prefabs and three original text-free images. Two saved Unity scenes let the new art be inspected without changing `PrototypeScene` or the demo.6 desktop packages. [Art direction](ART_DIRECTION.md) owns the visual language; [Roadmap](../Design/ROADMAP.md) owns any later production promotion.

The [October 29 production update](OPEN_MUSIC_STUDIO_UPDATE_20261029.md) subsequently places these seven prefabs into `PrototypeScene`. The original study and verification below describe their initial delivery; immutable demo.6 packages remain unchanged.

## Open the delivered scenes

- [Studio expansion](../../Assets/Scenes/StudioExpansion20261028.unity): a copy of the production room with supplies and artwork staged around the perimeter. Existing gameplay components remain inherited; this pass verifies art authoring, not Play Mode.
- [Prop gallery](../../Assets/StudioArtPack/StudioExpansion20261028/StudioPropGallery.unity): a dedicated inspection arrangement with all seven prefabs and neutral gallery lighting.
- [Prefab folder](../../Assets/StudioArtPack/StudioExpansion20261028/Prefabs): drag individual prefabs into an art-study scene for reuse.

Both scenes are excluded from the player build list. The production scene, gameplay scripts and existing asset GUIDs are preserved.

## Reusable resources

| Asset | Intended use |
|---|---|
| `SupplyTrolley.prefab` | Teal two-tier cart with paint tins, cloth and a sketchbook |
| `RollerTray.prefab` | Compact tray, pigment pool and roller for a workbench or floor edge |
| `CanvasEasel.prefab` | Honey-wood easel with an original raised geometric pigment study |
| `PigmentSwatchRail.prefab` | Five clipped swatches for workshop dressing; not a control or UI |
| `CanvasDryingRack.prefab` | Repeated open shelves and stacked canvases |
| `FramedRhythmPrint.prefab` | Framed original balloon-and-brush print |
| `FramedBotanicalPrint.prefab` | Framed original botanical monoprint |

All seven prefabs are collider-free and contain no runtime scripts. Each combines its authoring geometry into one mesh renderer, retaining separate material submeshes. The 107 source pieces become seven renderers across the collection; this reduces renderer count, **not a measured claim about draw calls or FPS**. The prefabs have seven saved meshes and shared warm wood, cream, teal, coral, ochre, sage, lilac, charcoal and metal materials.

New props are decorative and do not automatically become paint receivers. Reuse should preserve target/HUD visibility and the existing eligible-surface contract.

## Generated artwork and exact prompts

The three PNGs were created using **OpenAI built-in image generation**, then copied byte-for-byte into [Textures](../../Assets/StudioArtPack/StudioExpansion20261028/Textures). No external source image or downloaded artwork was supplied. Exact prompts, source-output identifiers, dimensions and SHA-256 values are in [Textures/Provenance.json](../../Assets/StudioArtPack/StudioExpansion20261028/Textures/Provenance.json).

| Image | Dimensions | Intended use |
|---|---|---|
| `StudioRhythmPrint.png` | 1254 × 1254 | Square print already used by the framed-rhythm prefab |
| `BotanicalPigmentPrint.png` | 1254 × 1254 | Square print already used by the framed-botanical prefab |
| `CourtyardAutumnView.png` | 1086 × 1448 | Prepared portrait window-view candidate with a separate URP/Unlit material, not a production-window replacement |

The courtyard is fictional and is not official campus imagery. Its texture uses sRGB, Clamp and mipmaps; the optional material remains unused in the delivered scenes. Prints remain opaque and text-free, with localized captions supplied separately by the website. Existing model, font and noncommercial music attribution remains in [Credits](../../CREDITS.md).

## Authoring and verification

[StudioExpansionBuilder.cs](../../Assets/StudioArtPack/Editor/StudioExpansionBuilder.cs) is an Editor-only authoring tool. Its initial `Run` entry point requires batch mode and refuses to overwrite an existing delivered pack. Use the saved assets directly; authoring another version should use fresh paths and preserve the current GUIDs.

The builder creates native geometry, imports the generated prints, saves both scenes, reloads prefabs/scenes and writes [Verification.txt](../../Assets/StudioArtPack/StudioExpansion20261028/Verification.txt). It checks nonempty meshes, supported materials, no added colliders/runtime behaviours, no missing scene scripts, unchanged production-scene bytes, and unchanged existing component data while creating the study. Unity 6000.0.77f1 completed the authoring run with exit code 0 and `STUDIO_EXPANSION_OK`.

The visual review corrected the wall-facing directions and moved the prints clear of the window and pegboard. The bounded `FinishPlacementReview` pass preserved prefab GUIDs and regenerated the scene captures; it completed with exit code 0 and `STUDIO_EXPANSION_PLACEMENT_REVIEW_OK`. [PositionReview.txt](../../Assets/StudioArtPack/StudioExpansion20261028/PositionReview.txt) records the final placement and optional courtyard-material preparation. The source production-scene SHA-256 remains `1860d0bd12a3074a3506e1b763fd7a38b824c6d0c1b6efb0236dcecc57199355`.

The [preview folder](../../Assets/StudioArtPack/StudioExpansion20261028/Previews) contains a same-camera before/after pair, a close inspection and the prop collection. These are Unity Edit Mode camera renders with UI hidden, not generated gameplay images or standalone captures. Existing room lightmaps and reflections are retained; new props use the available probes/realtime lighting. No new bake, gameplay test, frame-time benchmark or desktop build is implied.

The website uses selected compressed derivatives with independently recorded source/output hashes in `Website/media/provenance.json`. A source-art change requires regenerating its derivative before the next website build. Exact automated results and any browser limits are summarized in [Validation](../Project/VALIDATION.md).
