# Material and model appearance study V11

Open `Assets/Scenes/StudioMaterialAppearance.unity` in Unity **6000.0.77f1**. This independent study continues [camera and lighting V10](CAMERA_LIGHTING_V10.md). Its camera, light settings and postprocessing are retained. The original prototype, V10 scene, source materials, source meshes and playable prefab remain available for recovery. See [README](../../README.md), [game design](../Design/GAME_DESIGN.md), and [roadmap](../Design/ROADMAP.md) for project scope.

## Sample-first review

The first saved comparison used **one existing balloon mesh and one principal environment material, the oak floor**, through the tagged game camera at 1600 × 900. `Before_Sample.png` and `After_Sample.png` use the same position, camera, lights and room. The balloon sample was inspected at smoothness 0.42, then refined to 0.48 to retain a visible broad highlight before promotion. The floor sample retains the original oak artwork and changes its physical scale and surface response. [SampleReview.txt](../../Assets/StudioArtPack/MaterialAppearanceV11/SampleReview.txt) records the actual visual review.

Only after inspecting these renders were the sample materials promoted into a copied playable balloon prefab and the room study. The final material and mesh corrections were rendered and reloaded again. No generated concept or composited image is used as implementation evidence.

## Delivered appearance

| Element | Material and model changes |
|---|---|
| Balloon | Existing smooth pear-shaped mesh, knot and string retained. URP/Lit, opaque dielectric, Metallic 0, Smoothness 0.48 instead of 0.64, teal `#249DA6`. Broader highlights replace the small sharp reflection spots. |
| Wood floor | Existing `OakPlanks_v7.png` reused. Texture scale 1.75 on the existing world-scaled floor UVs: eight rows per 1.6 m repeat, approximately 20 cm board width. A separate copy of that image is imported as a height-derived normal, aligned with the albedo; normal strength 0.16. The existing oak packed surface map supplies varying roughness, approximately 0.11–0.29 effective smoothness. Metallic remains 0. |
| Walls | Pale cream `#F6EBDD`, Smoothness 0.10 and weak existing plaster normals at strength 0.085. A new restrained, nonperiodic 1024 px albedo uses less than 1% tone variation. Cloned wall meshes have UV0 spanning a room-sized clamped map, preserving geometry and UV2. |
| Wall paint | Original cyan, coral and yellow atlas artwork retained. Three planar SpriteRenderer marks receive separate decorative relief meshes with a 128-cell grid, smooth alpha contours and physical side rims. Paint depth varies approximately 2–12 mm. URP/Lit, Metallic 0, Smoothness 0.50; side rims use solid matching paint materials. The original sprite components and assets are retained, with their renderers disabled in this study. |
| Other paint | Existing floor paint, dots, drips, bucket surfaces and easel swatches receive copied nonmetallic paint materials, Smoothness 0.48 and brighter pigment colors. Existing geometry remains. This does not change particle effects or implement new impact decals. |
| Weapon | Cream body `#F5EBD9`, teal grip/rear trim and tank, coral muzzle `#F07865`, yellow structural parts `#F4C75B`. Painted parts have Metallic 0 and Smoothness 0.24–0.32. Existing rounded receiver, open barrel, collars, trigger guard and lid are retained. Copied barrel/lip geometry is 20% wider radially; tank/lid/label geometry is approximately 12% wider. Barrel depth, muzzle transform, controller transform and collision configuration remain intact. |

The existing combined weapon meshes are split into material regions where needed. Small remaining metal fittings retain their existing materials. The grip uses a continuous, untextured teal material on its original rounded geometry; this avoids a dark triangle-level material boundary and overly busy microstructure in the enlarged review. The first trial's local grip stretch was discarded after visual inspection. All final mesh assets preserve the established rounded normals where geometry is reused.

The balloon is an **opaque thin-latex appearance approximation**. Stock URP/Lit does not provide an authored thin-film transmission or subsurface thickness model here. Physical light passing through a balloon would require a separately justified shader/thickness workflow; lowering Metallic or adding transparency alone does not establish it. This pass uses the installed URP 17.0.4 Lit features rather than assuming HDRP or an external plugin. See Unity's [URP Lit material reference](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/lit-shader.html).

## What requires geometry, rather than material tuning

- A flat paint sprite cannot have a side silhouette or cast a thickness-dependent contact shadow. The study adds actual relief/rim geometry; a normal map alone would not solve that problem.
- Muzzle diameter, open-mouth wall thickness and tank silhouette require mesh edits. Their existing open and rounded structures were widened in copied meshes; the muzzle helper was not moved.
- Small sharp hardware, ventilation details and overlapping part clearances need deliberate mesh topology or physical separation if they must hold up in close views. Color and roughness cannot create a missing edge, hollow cavity or gap. The enlarged receiver view still shows a local dark/noisy contact area beside the barrel mount; a dedicated close-view mesh/normal and near-field shadow diagnosis is needed before claiming a final weapon asset.
- The existing beam lightmap/UV edge seams are visible in the room and are outside this material study. They need UV/mesh correction and rebaking, rather than a brighter material or Bloom.
- The unchanged gameplay framing crops much of the grip. The saved mesh is reviewed in an additional inspection pose, but making the complete grip visible during normal aiming would require a separate composition/held-weapon placement decision.

The existing ACES volume and Bloom intensity **0.12** remain unchanged. No new Bloom, depth of field, motion blur, exposure adjustment or camera/light change is used to conceal these limits.

## Captures and evidence

All captures under `Assets/StudioArtPack/MaterialAppearanceV11/Previews` are real Unity renders from the same tagged game camera, 1600 × 900, excluding Screen Space Overlay UI.

| Files | Meaning |
|---|---|
| `Before_Sample.png`, `After_Sample.png` | One fixed balloon, before promotion; sampled floor/material response |
| `Before_Room.png`, `After_Room.png` | Same fixed three-target layout, baseline and completed study |
| `Before_Gun.png`, `After_Gun.png` | Normal held-weapon transform, temporarily aimed at the same central point |
| `Before_WeaponInspection.png`, `After_WeaponInspection.png` | Weapon temporarily brought forward, enlarged and rotated identically for inspection; this pose is restored and is not saved gameplay placement |
| `Before_Runtime.png`, `After_Runtime.png` | Three naturally spawned targets in the isolated Play Mode comparison; seed held constant, capture frames not synchronized |

Unity compiled the current learner sources and reloaded the study with one saved lightmap pair and 75 target-region probes. Material changes and new paint geometry require recomputing bounce: lighting settings are copied with the same values, and new lightmaps are saved under `Assets/Scenes/StudioMaterialAppearance`. A separate Custom reflection cubemap records the changed room materials with the held gun hidden. These are material-dependent bake updates, not a new lighting design.

The creation checks retain camera/light serialization, learner component fields and scene colliders, allowing only the spawner's reference to the copied visual balloon prefab. The copied prefab preserves its scripts, colliders, Rigidbody, movement parameters and mesh. [Verification.txt](../../Assets/StudioArtPack/MaterialAppearanceV11/Verification.txt) and [PlayCheck.txt](../../Assets/StudioArtPack/MaterialAppearanceV11/PlayCheck.txt) state the final automated results and limitations. They do not establish a human mouse playtest, full round, performance acceptance or a playable release.

The concept-image attachments were not accessible in this chat. Existing studio assets and the user's written warm-workshop and material brief were the available references. Exact concept matching remains unverified.

## Tune and recover

1. Open the study scene in a 16:9 Game view. Enter Play Mode, click Start and manually check aiming, projectile hits, three-target refresh, score and retry.
2. Edit the named copied materials under `MaterialAppearanceV11/Materials`. The study's spawner uses `MaterialAppearanceV11/Prefabs/PlayableBalloonAppearance.prefab`; the original playable prefab is unchanged.
3. Preserve the study scene, its paired metadata, its lighting folder and the complete `MaterialAppearanceV11` folder together. Changing static material albedo, wall UVs or relief geometry requires rebaking the study and refreshing its reflection. Do not bake into the baseline scene's lighting data.
4. Open `StudioCameraLighting.unity` to recover the preceding material/model appearance. The study is appended to Build Settings for the existing build-index-based retry; the original prototype remains first.
5. The editor-only utility `StudioMaterialAppearancePass` includes sample creation, review refinement, promotion and saved-scene verification. Existing-study guards prevent rerunning one-time promotion over manual changes. Its `SampleReview.txt` gate requires an actual render review record; it does not require a new user permission request.

## Provenance

Original images, attribution and recorded image-generation prompts are untouched. Floor normals derive from a byte-for-byte copy of the existing oak artwork via Unity's importer; plaster variation is deterministic procedural data. Relief geometry derives from the existing graffiti atlas alpha; weapon meshes reuse the existing geometry and rounded normals. New lightmaps, reflections and previews are Unity outputs. No external download, replacement concept image or new AI raster was introduced. See [Credits](../../CREDITS.md).
