# Cover-aligned studio finish V9

Open `Assets/Scenes/PrototypeScene.unity` for the saved game environment. This pass improves the real 3D room toward the cover's warm artisan-studio reference. The generated cover remains a visual target; this checkpoint does not establish equivalent detail or rendering quality.

## Delivered environment

- Cream plaster has a restrained, periodic mineral albedo and normal map with world-scaled UVs. The central aiming area remains quiet.
- Selected wood beams, skylight rails and shelves have approximately 2.5 cm rounded edges and physical-size texture coordinates. Original transforms and collision shapes are retained.
- Existing plants use more detailed curved blades, split leaf silhouettes and subtle vein textures. Additional trailing greenery is combined into three material batches.
- A teal sideboard with inset fronts, an oak worktop and brass pulls fills the lower right edge. Its three material batches and the three greenery batches add six decorative MeshRenderers, without colliders.
- Existing rug strips use woven cloth surfaces and muted tints. Original rug geometry remains.
- Window/sun/fill balance, restrained ACES color grading, bloom and the existing PC renderer's ambient-occlusion settings provide more depth. The new global Volume has priority 20 and no motion blur or depth-of-field override.

The new materials, meshes, textures, color profile, reflection and captures are under `Assets/StudioArtPack/CoverFinishV9`. Existing assets and GUIDs remain available. The PC renderer's existing SSAO feature changes intensity from 0.4 to 0.7 and radius from 0.3 to 0.4; this affects scenes using that renderer. Its direct-lighting strength remains 0.25.

## Saved offline lighting

Two rectangular area lights are **Baked**, with no additional realtime lights. One warm source models broad window bounce; the second provides cooler skylight illumination. The environment uses copied meshes with secondary UVs, leaving imported source meshes intact.

The CPU bake uses 12 texels per world unit, a 1024 atlas, 32 direct samples, 128 indirect samples, 64 environment samples and three bounces. One directional lightmap pair and `LightingData.asset` are saved in `Assets/Scenes/PrototypeScene`, with their original generated metadata. The lighting settings and 99 prepared mesh copies are in `CoverFinishV9/BakedLighting`. Preserve the complete lighting folder: the scene and baked data reference its assets.

The Custom reflection probe references `CoverFinishV9/Reflections/ArtisanStudio.exr`, captured with the held gun hidden. Old reflection assets are retained for prior scenes and checkpoints.

To tune color, edit `StudioColorGrade.asset`; to tune plaster or joinery, open the named materials from Mesh Renderer. Moving static props or changing geometry requires rebaking the lighting. Realtime light adjustments can be inspected immediately, but changes to baked area lights require a new bake. Do not rerun the one-time builder over this delivered scene.

## Captures and evidence

| Capture in CoverFinishV9/Previews | Meaning |
|---|---|
| Room_Before_v9.png | Saved room before this environment pass |
| Room_GameCamera_v9.png | Reloaded saved environment through the game camera, excluding overlay UI |
| Targets_Illustrative_v9.png | Three temporarily positioned/recolored targets for art comparison; not gameplay evidence |
| Gameplay_Runtime_v9.png | A normally spawned target during the isolated Play Mode start-flow check, excluding overlay UI |

Unity 6000.0.77f1 compiled and reloaded the merged scene, verified saved lightmap references, finite mesh coordinates, missing-script absence and four-corner muzzle framing. The existing start/pause/retry and balloon-motion/effect acceptance coroutines passed in the isolated project. These invoke callbacks/methods; they do not establish physical mouse input, target-PC frame rate or a completed graphical round. See [Validation](../Project/VALIDATION.md) for the checked snapshot and limitations.

Learner gameplay, prefab connections, camera framing and collider serialization were preserved. Concurrent saved scene changes were merged by Unity object ID before checking the current sources. The art utility is editor-only; it adds no runtime script.

## Provenance

New plaster and vein PNGs are deterministic mathematical textures, and new geometry is procedural. Existing wood and woven source materials retain their attribution. Lightmaps, reflections and preview PNGs are Unity renders. This pass adds no external download or AI-generated raster, and does not replace any recorded image-generation prompt. See [Credits](../../CREDITS.md) and the [studio guide](STUDIO_ART_PACK.md).
