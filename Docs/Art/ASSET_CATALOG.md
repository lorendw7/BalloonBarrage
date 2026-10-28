# Asset catalog

Canonical assets stay in Unity directories. `Website/assets.json` references selected originals and tracked web derivatives; generated deployment copies stay ignored. Asset presence alone does not prove runtime integration.

Updated: October 28, 2026. New art studies remain separate from the demo.6 desktop package. The [studio expansion guide](STUDIO_EXPANSION_20261028.md) records the new pack's contents, prompts and verification; [Roadmap](../Design/ROADMAP.md) owns any production promotion.

| Location | Contents |
|---|---|
| Assets/Art/Balloon | Original balloon model |
| Assets/Art/BlasterKit | 40 FBX models including 18 blasters; keep the original palette |
| Assets/Art/Concepts | Studio and balloon visual references |
| Assets/Art/Textures | Original splat atlas and quiet graffiti wall |
| Assets/Art/Previews | PrototypeScene_20261010 and gun/workbench/balloon closeups |
| Assets/Audio | Five generic and five glass impact sounds |
| Assets/Audio/Music | Three original loops, two cues and music metadata |
| Assets/Prefabs | Playable balloon |
| Assets/StudioArtPack | Generated textures, props, materials, VFX and preview scenes |
| Assets/StudioArtPack/StudioExpansion20261028 | Reusable studio props, materials and generated artwork for the separate October 28 composition study; see its [handoff and evidence](STUDIO_EXPANSION_20261028.md) |
| Assets/StudioArtPack/InterfaceV2 | New menu background, nine sliced sprites, five UI presentation prefabs and Unity previews |
| Assets/StudioArtPack/RoomFinishV7 | Oak floor texture and world-scale UV mesh, cream walls, ivory ceiling and prior room reflection |
| Assets/StudioArtPack/ColorRefresh20261020 | Two native side-wall mural meshes, six pigment materials, independent brighter color grade and before/after Unity inspection renders; see [color refresh](STUDIO_COLOR_REFRESH.md) |
| Assets/StudioArtPack/SurfaceRefresh20261020 | Original procedural plaster/mineral-floor PBR maps, two materials, ten render-mesh UV variants and native Unity before/after inspection renders; see [surface pass](STUDIO_COLOR_REFRESH.md#plaster-and-mineral-floor-materials) |
| Assets/StudioArtPack/ExhibitionV8 | Four-wall closure, refreshed reflection, prepared paint-pop variants and camera previews |
| Assets/StudioArtPack/BalloonColors | Eight indexed balloon/projectile pigments and matching persistent marks connected in PrototypeScene; focused isolated checks are recorded separately from full-song readability/load |
| Assets/StudioArtPack/ConceptVariantsV12 | Six latex pigments, three balloon forms, copied playable balloon, new rounded gun meshes, wall-paint derivatives, reflection and Unity previews |
| Assets/StudioArtPack/PaintBlaster/ConceptRefinement20261019 | Native production gun shell, six mesh/material batches, concept-style horizontal reservoir, source hashes and Unity before/after inspection renders |
| Assets/StudioArtPack/PaintBlaster/MixedPigment20261020 | Production multicolor marbled liquid, separated teal body mesh, reservoir calibration marks and native Unity previews; eight cosmetic batches in the updated gun |
| Assets/StudioArtPack/PaintSprayStyle20261025 | Original transparent four-cell neutral VFX atlas prepared for runtime tinting; exact built-in generation prompt, source path, hash and importer settings are stored beside the PNG; candidate art is not connected to gameplay |
| Assets/StudioArtPack/SurfacePaint | Prepared small teal projectile wall-mark prefab/material, existing atlas provenance and native close-up preview; first-wall/eight-second integration saved with creator-reported pass |
| Assets/StudioArtPack/SurfacePaintCoverage20261027 | Original four-cell neutral persistent-mark atlas with 32 integrated eight-pigment prefabs, broader connected centers and shorter lobes; exact prompt, source path, hash, importer settings, automated nine-surface/1,000-mark validation metrics and native Unity preview are stored beside the PNG |
| Assets/StudioArtPack/Runtime | Cosmetic selection/tint components and one-time explicit static-room batching; no new score, movement or rhythm rules |
| Assets/Scenes/StudioConceptVariants.unity | V12 cosmetic variant scene; paired lighting folder and appended Build Settings entry |
| Assets/StudioArtPack/ConceptDetailV13 | Copied room pigments/strip/rug materials, procedural mineral floor, batched art supplies/paint studies, reflection and actual Unity previews |
| Assets/Scenes/StudioConceptDetail.unity | Earlier V13 room-detail scene; paired lighting folder and appended Build Settings entry |
| Assets/Settings/Gameplay/ExhibitionControls.inputactions | Prepared mouse/gamepad actions; runtime controller/co-op support is not connected |
| Assets/StudioArtPack/CoverFinishV9 | Current plaster, rounded joinery, botanical foliage, sideboard, color grade, lightmap-ready mesh copies and game-camera captures |
| Assets/Scenes/PrototypeScene | Saved V9 lighting data, directional lightmap pair and bake-generated reflection |
| Assets/Localization | Shared Simplified Chinese/Japanese/English UI table; persistent runtime switching and dynamic labels are connected; see [localization](LOCALIZATION.md) for glyph/layout and package evidence |

## Integration guides

- [Studio expansion, October 28](STUDIO_EXPANSION_20261028.md): separate scene study, original reusable art, exact prompts and verification limits.
- [Balloon color lesson](BALLOON_COLOR_LESSON.md): current-source eight-color prefab, indexed projectile/mark hookup, matched bursts and verification limits.

- [Surface paint lesson](SURFACE_PAINT_LESSON.md): earlier wall/contact learning steps; the current nine-receiver broad-coverage checkpoint is recorded in [paint-spray style](PAINT_SPRAY_STYLE.md) and [Validation](../Project/VALIDATION.md).

- [Studio art pack](STUDIO_ART_PACK.md): current V6 surfaces, earlier dependencies, music and recorded prompts.
- [Cover finish V9](COVER_FINISH_V9.md): current environment finish, offline lighting, tuning and capture evidence.
- [Concept variants V12](CONCEPT_VARIANTS_V12.md): random cosmetic initialization, gun model and verified Unity capture/Play Mode scope.
- [Concept studio detail V13](CONCEPT_DETAIL_V13.md): earlier room color/detail study, batched native supplies, mineral floor and comparison evidence.
- [Paint blaster](PAINT_BLASTER.md): assembled gun, projectile visuals, muzzle effects and the prepared paint-shot audio/framing handoff.
- [Concept-style gun refinement](CONCEPT_GUN_REFINEMENT.md): latest production shell, preserved recoil/muzzle integration and inspection evidence.
- [Balloon pop](BALLOON_POP_V2.md): eight bounded latex-fragment particle prefabs are serialized on the production balloon and contact-point handoff is saved; the short variant-selection function and physical alignment check remain learner/playtest work.
- [Paint-pop lesson](PAINT_POP_LESSON.md): connect combined fragments and paint droplets through the existing learner hook.
- [Spray assets](SPRAY_ASSETS.md): wall decoration atlas and prefabs.
- [Interface V2](INTERFACE_V2.md): current UI prefabs, score/timer display and learner integration steps.
- [Aim reticle](INTERFACE_V2.md#aim-reticle-handoff): original native-UI prefab and connection handoff; runtime following is now saved, with evidence in Validation.
- [On-beat feedback](INTERFACE_V2.md#on-beat-feedback-handoff): bounded golden ring/sparkle prefab and a separate hidden reward Text for learner integration.
- [Menu assets](MENU_ASSETS.md): original V1 background, slices and preserved prompts.
- [Localization](LOCALIZATION.md): text, font and runtime requirements.
- [Music and charts](../Design/MUSIC_AND_CHARTS.md): external study downloads.

## Current website captures

The website keeps released gameplay, development captures, native art-study renders and generated references separately captioned. `Assets/Art/Previews/Gameplay_v0.1.0-demo.2_20261018.jpg` is the preserved 2560 × 1440 released-demo capture; it must retain its demo.2 date/version even while current downloads advance. `CreatorStats_20261016.png` retains its Editor label, and the October 27 dense-paint frame remains an automated Unity fixture capture. New studio artwork is not a screenshot of demo.6.

`Website/assets.json` selects displayed sources. Smaller web derivatives and their hashes/settings are maintained by `Website/scripts/prepare-images.py` and `Website/media/provenance.json`; generated deployment copies live in ignored `Website/dist`. Existing concept artwork and recorded prompts remain unchanged; see [Credits](../../CREDITS.md).

## Wall and reference import

`GraffitiWall_Quiet_v2.png` and `Balloon_Readability_v2.png` are opaque 1536×1024 generated images. The wall is a nonseamless 3:2 panel: Default texture, sRGB, Clamp, Bilinear, maximum size 2048 and mipmaps. Use a separate opaque URP/Lit material, white base color, metallic zero and smoothness around 0.15. Position in front of the old wall and behind the frame without a collider.

Slice `GraffitiSplats_Atlas.png` as four columns by two rows using actual import dimensions. The opaque balloon reference is not a gameplay sprite atlas. Preserve model/palette relationships and `.meta` files. `smoke.fbx` is a mesh, not a ParticleSystem.

See [Credits](../../CREDITS.md) for third-party provenance. Original prompts for the older studio concept and first splat atlas are unavailable; do not invent them. The following recorded prompts are retained verbatim; image generation is not pixel-deterministic.


## Recorded generation prompts

```text
Use case: stylized-concept. Asset type: production game wall albedo texture for BalloonShooter, a cozy desktop balloon popping graffiti studio. Create a flat front-facing rectangular warm cream plaster wall painting, landscape 3:2. Very subtle paper/plaster grain. Sparse hand painted muted teal, coral and ochre brush swashes confined to outermost 15% corners and edges; central 70% almost blank cream for clear moving balloon targets. Low saturation and low contrast. No balloons, no strings, no text, no frame, no room, no perspective, no shadows, no lighting baked into texture, no watermark. Opaque background. This is a directly usable wall texture, not a room concept.
```

```text
Use case: stylized-concept. Asset type: balloon visual development reference sheet for a cozy Unity 3D graffiti balloon-popping game, not a runtime sprite atlas. Landscape sheet, warm cream plain background. Exactly three isolated balloons equally spaced in a single row: coral red, deep teal, muted indigo. Each balloon has instantly recognizable rounded pear silhouette, a visible small tied triangular knot, one short gently curved charcoal string, satin rubber surface with broad soft off-white highlight on upper left and clear darker edge. Balloon body occupies 65% of sheet height, short string 15%. Hand-painted polished stylized 3D aesthetic, rounded friendly shapes. No paint splats, no room, no props, no letters, no text, no grids, no watermark, no ground shadow. Clear silhouette and warm-cool contrast usable as modeling and material reference.
```
