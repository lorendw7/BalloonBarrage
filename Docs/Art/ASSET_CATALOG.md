# Asset catalog

Canonical assets stay in Unity directories. `Website/assets.json` references selected originals; generated website copies stay ignored. Asset presence alone does not prove runtime integration.

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
| Assets/StudioArtPack/InterfaceV2 | New menu background, nine sliced sprites, five UI presentation prefabs and Unity previews |
| Assets/StudioArtPack/RoomFinishV7 | Oak floor texture and world-scale UV mesh, cream walls, ivory ceiling and prior room reflection |
| Assets/StudioArtPack/ExhibitionV8 | Four-wall closure, refreshed reflection, prepared paint-pop variants and camera previews |
| Assets/StudioArtPack/CoverFinishV9 | Current plaster, rounded joinery, botanical foliage, sideboard, color grade, lightmap-ready mesh copies and game-camera captures |
| Assets/Scenes/PrototypeScene | Saved V9 lighting data, directional lightmap pair and bake-generated reflection |
| Assets/Localization | 25 UI keys in three languages; runtime switching pending |

## Integration guides

- [Studio art pack](STUDIO_ART_PACK.md): current V6 surfaces, earlier dependencies, music and recorded prompts.
- [Cover finish V9](COVER_FINISH_V9.md): current environment finish, offline lighting, tuning and capture evidence.
- [Paint blaster](PAINT_BLASTER.md): assembled gun, projectile visuals and muzzle effects.
- [Balloon pop](BALLOON_POP_V2.md): latex-fragment particles.
- [Paint-pop lesson](PAINT_POP_LESSON.md): connect combined fragments and paint droplets through the existing learner hook.
- [Spray assets](SPRAY_ASSETS.md): wall decoration atlas and prefabs.
- [Interface V2](INTERFACE_V2.md): current UI prefabs, score/timer display and learner integration steps.
- [Menu assets](MENU_ASSETS.md): original V1 background, slices and preserved prompts.
- [Localization](LOCALIZATION.md): text, font and runtime requirements.
- [Music and charts](../Design/MUSIC_AND_CHARTS.md): external study downloads.

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
