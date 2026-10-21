# Studio color refresh

The October 20 pass targets the production `Assets/Scenes/PrototypeScene.unity`, using the original `Assets/Art/Concepts/CozyGraffitiStudio_Concept.png` as a composition and lighting reference. See [art direction](ART_DIRECTION.md) for visual priorities and [asset catalog](ASSET_CATALOG.md) for existing assets.

## Reference comparison

The concept has soft daylight, readable shadow-side walls, irregular pigment clusters, plaster microstructure and a restrained mineral floor. Before these passes, the production room had a darker left wall, stronger localized sunlight, cleaner side surfaces and an orange wooden floor. Increasing all light intensities would amplify the existing hotspot rather than address this imbalance.

The current pass adds bounded, text-free color to both side walls and balances existing realtime fill and window light. It retains the camera, central aiming space, furniture, collision geometry and baked lightmaps. An independent volume profile lifts exposure slightly, reduces vignette and restores some pigment saturation. New murals are native procedural meshes using six rough pigment materials; they introduce no downloaded or generated raster artwork and no new gameplay logic.

## Follow-up priorities

1. Refine bevels, contact shadows and hanging foliage around the perimeter; retain gameplay collider and camera poses.
2. Review a new indirect-light/reflection bake only after material and room changes settle. Preserve previous bake assets until the new result has been compared.

These are proposed follow-ups, not implemented features. The concept is an art target, not proof of a playable scene or guaranteed visual parity.

## Plaster and mineral-floor materials

The subsequent surface pass replaces the production floor and floor return with a warm gray mineral material, and assigns limewashed plaster to eight walls/columns. Existing timber furniture, beams, skirting and stage remain wood. The central target area keeps low-contrast surface variation; existing paint decorations remain separate.

`Assets/StudioArtPack/Editor/StudioSurfaceRefresh.cs` deterministically generates original seamless 1024 x 1024 albedo, tangent-normal and smoothness maps for both materials, with no external downloads or image-generation prompts. The assets are in `Assets/StudioArtPack/SurfaceRefresh20261020`. Metallic stays zero; floor smoothness varies around 0.26 and plaster around 0.13. Final normal strengths are 0.55 for the floor and 0.42 for plaster. A slightly cooler floor tint limits the existing warm sunlight cast.

Cloned render meshes use world-scale UV0: a 4.8 m floor repeat and 3.2 m wall repeat. Tangents are recalculated for the new UVs. Vertex positions, triangle topology and lightmap UV2 are checked against the original meshes. Existing shared mesh assets and material GUIDs remain intact; only scene references point to the new assets. Textures repeat with mipmaps, trilinear filtering, 8x anisotropy and high-quality compression.

The isolated Unity build checks ten surface assignments and unchanged gameplay, collision, UI, camera, lighting and MonoBehaviour data/transforms. The scene is saved, reopened and rendered at 1920 x 1080; final material tuning is captured separately. This is native scene inspection, not interactive gameplay acceptance. Baked lighting and reflections retain the earlier room's indirect color and have not been regenerated for the new materials.

## Authoring and evidence

`Assets/StudioArtPack/Editor/StudioColorRefresh.cs` authors the pass in an isolated project. Assets and before/after native Unity captures live in `Assets/StudioArtPack/ColorRefresh20261020`. The tool checks that existing gameplay, physics, UI and camera components and their transforms are unchanged, reloads the saved scene and captures it at 1920 x 1080 with UI temporarily hidden. Captures are scene inspection renders, not an interactive playtest or release build.

The final isolated Unity run passed with `STUDIO_COLOR_REFRESH_OK` and process exit code 0. Source gameplay file hashes remained unchanged. The reviewed scene and new assets were copied back after checking that the production scene had not changed concurrently. Existing scene and asset GUIDs were retained. The final production-scene diff consists of new mural objects, three ambient colors, window intensity (135 to 90), frontal fill (7 to 21), sun shadow strength (0.5 to 0.36), and the copied volume-profile reference. Sun intensity stays at 1.4. The copied grade uses exposure 0.30, saturation 3, contrast 1 and vignette 0.025.

Native screenshots show lighter shadow-side surfaces while retaining the existing room composition. Fixed image-space luminance samples increased from 110.1 to 140.2 on the left wall and 88.9 to 112.8 on the ceiling (8-bit grayscale); these are image statistics, not physical light measurements or a player visibility test. No new lightmap bake, standalone package or performance benchmark was performed.

Original concept attribution and recorded image-generation prompts remain in [Credits](../../CREDITS.md) and the catalog. The new wall geometry is deterministic project-authored decoration; it does not alter those source images.
