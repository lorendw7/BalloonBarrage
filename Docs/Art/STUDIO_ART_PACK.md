# Studio art pack

Open `Assets/Scenes/PrototypeScene.unity` for the current game. `Assets/StudioArtPack/Generated/StudioArtPreview.unity` is an older art preview, not the current gameplay scene. Existing generated assets are already applied; one-time builders refuse to overwrite their output. Edit current prefabs and scene groups directly rather than regenerating them over manual work.

## Current composition and surfaces

The October 12 [cover finish V9](COVER_FINISH_V9.md) adds restrained mineral plaster, rounded joinery, botanical leaf surfaces, trailing greenery and a teal sideboard. It applies saved offline window/skylight bounce lighting and a room color grade. Six decorative material batches add no colliders; two baked area lights add no realtime lights. The latest saved-room and runtime-camera captures are in `CoverFinishV9/Previews`.

The October 11 interface pass moves and scales the three wall splashes toward the edges of the central board, preserving a quieter aiming area. It adds no renderers, materials, lights or colliders to the gameplay scene. [Interface V2](INTERFACE_V2.md) supplies separate presentation prefabs and a learner guide; the prototype's OnGUI controls remain in place until that integration is completed.

The V6 gun has a rounded shell, inset panels, metal nozzle, ribbed grip, reservoir and lid, combined into seven material batches. Original gun imports remain; the visible source mesh is disabled. Muzzle and firing references are retained. The smooth balloon surface retains the original knot and a rebuilt curved rope, with existing gameplay components.

`SurfacesV6` contains thirteen materials, seventeen meshes, eleven native 512-square surface maps and a 256-resolution HDR room reflection cubemap. Oak, cloth, metal, enamel, ceramic, rubber and latex have distinct surfaces. Linear normal/parameter maps use mipmaps, trilinear filtering and anisotropy eight. A Custom reflection probe explicitly references the offline cubemap, captured without the held gun. No extra realtime lights or decorative colliders are required.

Camera reference: position `(0, 0.10, -8.8)`, pitch 2 degrees, field of view 45 degrees. Gun camera-local position `(0.47, -0.51, 1.35)`, uniform scale `0.50`. Treat saved Inspector values as authoritative if adjusted. Verify framing and corner aiming in a 16:9 Game view.

## Cream walls and oak floor (RoomFinishV7)

RoomFinishV7 supplies these base room materials. NaturalOakFloor and IvoryCeiling remain applied; V9 replaces the walls' CreamPlaster assignment with MineralCreamPlaster:

| Material | Applied to | Settings |
|---|---|---|
| NaturalOakFloor | Floor | Generated OakPlanks_v7 base map, metallic 0, smoothness 0.26 |
| CreamPlaster | LeftWall, RightWall, MainWall, LeftColumn, RightColumn | Base color #F4EADB, metallic 0, smoothness 0.12 |
| IvoryCeiling | Ceiling | Base color #F6F1E7, metallic 0, smoothness 0.10 |

To tune the finish, select the named object in the Hierarchy and open its material from Mesh Renderer. Adjust Base Map tint and Smoothness, then check Game view. The current plaster is shared across the walls and columns; duplicate the material first if only one surface should change. V9 uses a restrained mineral texture rather than the older mottled concrete texture. Lighting still creates natural shaded areas.

The floor keeps its original transform, geometry shape and BoxCollider. A mesh copy changes only UV coordinates: one texture repeat spans 2.8 world units, giving eight approximately 35 cm board rows. The 1254-square source image retains its original pixels and uses Repeat, mipmaps, trilinear filtering and anisotropy 8. Material tiling (1,1) is the baseline; (2,2) halves the apparent plank dimensions. The plank pattern was inspected from the gameplay camera; generation does not guarantee pixel-perfect tile edges or invisible repetition from every viewpoint.

`Reflections/CreamOakStudio.exr` is the refreshed Custom probe cubemap, baked with the held gun temporarily hidden. Original surface assets remain for other props and older previews. `StudioRoomFinishBuilder.BuildBatch` is a one-time isolated-project art utility that refuses to overwrite an existing V7 finish. It checks that learner component serialization, collider settings and renderer counts stay unchanged. Edit the delivered materials for later tuning.

The V7 room and screen renders remain in `Assets/StudioArtPack/InterfaceV2/Previews`. They are presentation captures, not runtime gameplay acceptance. Current V9 captures are linked above. The floor texture uses built-in image generation; its exact prompt is retained below.

## Four cream sides and exhibition feedback (ExhibitionV8)

The original three-sided room stopped at z=-2, while the fixed camera sits at z=-8.8. `StudioRoomClosureV8` completes a fourth wall at z=-9.9, behind the camera, with matching left/right returns and floor/ceiling extensions. V9 applies MineralCreamPlaster (#F4EADB) to MainWall, LeftWall, RightWall, BackWall and the two side returns. Arches, wooden trim and artwork retain their accent colors. Shadows naturally change the displayed shade of the cream material.

The group adds eight simple renderers: five shell surfaces and three skirting pieces. They have no colliders and cast no realtime shadows, preserving the existing shooting path. Original floor/wall collision shapes, camera and learner components are unchanged. Its earlier fill-light and reflection settings are superseded by the [V9 lighting finish](COVER_FINISH_V9.md).

The V8 gameplay-camera and four-direction wall captures are in `Assets/StudioArtPack/ExhibitionV8/Previews`. They precede the current V9 surface and lighting finish. Earlier InterfaceV2 previews retain their V7 room backdrop. The one-time `ExhibitionPolishBuilder.BuildBatch` refuses to rebuild an existing closure. Tune delivered materials and objects directly; avoid putting a wall at the original open edge, which would block the fixed camera.

The same pack supplies three prepared latex-and-paint particle prefabs. Follow the [paint-pop lesson](PAINT_POP_LESSON.md) to connect one; no production balloon reference was changed by this art pass. Wall marks are the following learner lesson.

## Dependencies retained in the scene

| Directory | Role |
|---|---|
| Generated | Base materials, balloon variants, props, PaintBurst and preview |
| ExhibitionV1 | Courtyard/poster images, frames, workbench and reusable music source |
| CartoonV2 | Rounded prop meshes, reusable assemblies and historical toon materials |
| AtmosphereV3 | Physical materials, UV meshes and window-light mask |
| ReferenceV4 | Fine oak, alcoves, curved leaves and planters |
| DetailsV5 | Eight prop prefabs: curtains, desk tools, lower shelf, pegboard, clock, cart, stool and niche books |
| SurfacesV6 | Current closeup geometry and prop surfaces; original room reflection |
| RoomFinishV7 | Current floor UVs and oak texture, cream/ivory room materials, refreshed room reflection |
| ExhibitionV8 | Four-wall closure, prior room reflection, prepared paint-pop variants and earlier room/effect previews |
| CoverFinishV9 | Current plaster/joinery/foliage surfaces, sideboard, color grade, offline lighting mesh copies, refreshed reflection and latest room/runtime previews |

Earlier directory names do not make their contents disposable. V5 props retain 155 renderers and, after V6 refinements, 27,606 vertices; these counts are not frame-rate measurements. Curtains and the clock are decorative, without cloth or clock simulation.

## Original music

Files are in `Assets/Audio/Music`, generated by `Tools/Audio/compose_studio_music.py` without external recordings or samples.

| File | Tempo | Structure and use |
|---|---|---|
| StudioDaylight_Loop.wav | 108 BPM | 32 bars, 71.111 seconds; current warm piano-like loop |
| StudioDaylight_Drums_Loop.wav | 108 BPM | 32 bars, 71.111 seconds; alternative, do not layer over the full mix |
| StudioDaylight_Menu_Loop.wav | 84 BPM | 8 bars, 22.857 seconds; prepared menu loop |
| RoundStart_Soft.wav | 108 BPM | 2.222-second count-in cue |
| RoundComplete_Soft.wav | 108 BPM | 2.222-second completion cue |

The main loop uses a 2D AudioSource at volume 0.14. Loops stream as Vorbis; cues use decompressed PCM. Menu/cue events are not wired. Current files use original piano-like synthesis and a quiet harmonic bed. File-level clipping/seam checks do not replace listening or in-game loop acceptance. Rhythm integration is planned in [Music and charts](../Design/MUSIC_AND_CHARTS.md).

## Provenance and previews

Texture images use built-in image generation; meshes, props, light masks and surface maps use Unity primitives or procedural geometry. The courtyard is fictional, not campus photography or official Qpic art. Courtyard/poster textures are clamped rather than tiled. Fine-oak and older texture edges were visually inspected, not proven pixel-seamless.

The October 10 prototype preview captures three normally spawned targets through the runtime camera and excludes UI. Other preview scenes may contain illustrative art arrangements; do not describe them as gameplay evidence. See [Validation](../Project/VALIDATION.md) and [Credits](../../CREDITS.md).

The original prompts below appear in this order: fine oak, courtyard, poster, older wood, former concrete floor, and UI panel. The new V7 oak-plank prompt follows them.


## Recorded generation prompts

```text
Production seamless albedo texture for a warmly lit semi-realistic stylized 3D artist studio, matching high-end cozy game environment rendering. Entire square is natural honey oak, fine straight vertical grain, narrow delicate grain lines with gentle variation, subtle wood pores, soft golden tan warm medium-light brown. Restrained realistic grain, low contrast, satin unfinished timber. Flat front-on orthographic diffuse texture, perfectly even illumination, no baked light, no shadows, no specular highlights, no knots, no dramatic swirling grain, no boards, no seams, no objects, no frame, no labels, no text. Full bleed single tileable texture, not a room render or material sphere.
```

```text
Use case: stylized-concept. Asset type: production opaque texture for a tall window panel in a cozy Unity 3D graffiti studio. Primary request: a peaceful stylized courtyard view, entirely outdoors, as seen through a window but WITHOUT the window frame or any interior. Composition: portrait 3:4 artwork, full-bleed landscape with pale apricot afternoon sky in upper half, soft green trees framing distant simple cream campus-like buildings, a quiet stone walkway at bottom. Style: polished hand-painted low-poly game illustration, broad simple shapes, low visual noise, soft warm sunlight. Palette: warm cream #F4E6D0, honey wood browns, muted teal #3EC6C0, green foliage, small coral accents. Constraints: image fills all edges; no frame, glass reflection, room, gun, balloons, people, text, letters, signs, logos, watermark or UI. This is a directly usable window-view texture, not a concept sheet and not a photographic depiction of any real campus.
```

```text
Use case: stylized-concept. Asset type: production opaque artwork for a square framed poster inside a cozy Unity graffiti studio. Primary request: playful abstract balloon-and-paint artwork. Composition: square full-bleed warm ivory handmade paper; three stylized rounded pear-shaped balloon forms, coral #FF6B6B, teal #3EC6C0, mustard #FFD166, clustered asymmetrically with small indigo #5B5F97 paint droplets and loose charcoal curved strings. Big shapes, generous breathing space, subtle paper grain. Style: tasteful screen-printed illustration with slightly imperfect ink edges, friendly and modern, matching a warm honey-wood game studio. Constraints: just the artwork, flat front-on, fills the square; no physical frame, wall, room, perspective, shadows, text, lettering, numbers, logos, badges, watermark, gun, people or UI. This is a directly usable poster texture, not a reference sheet.
```

```text
Use case: stylized-concept. Production square seamless albedo texture for cozy graffiti game wooden stage beams. Honey oak warm mid brown, wide subtle flowing wood grain vertically oriented, hand-painted stylized game art. Flat even color, low contrast, no knots resembling eyes, no boards seams, no perspective, no lighting, no shadows, no text, no frame. Entire square filled with texture, tileable edges.
```

```text
Use case: stylized-concept. Production seamless square floor albedo texture for a cozy graffiti studio game. Warm light grey beige matte microcement, extremely subtle tiny mineral flecks and cloudy plaster variations. Even neutral lighting, low contrast, no cracks, no tiles, no seams, no perspective, no cast shadows, no objects, no text, no graffiti. Tileable edges. Hand-painted game material, visually quiet.
```

```text
Use case: stylized-concept. Asset type: game UI background panel single asset. Wide landscape 3:2 image completely filled by warm ivory handmade paper texture with a rounded hand-drawn dark charcoal border inset 5 percent, tiny teal and coral paint flecks only in corners. Large completely blank center for live score/settings text, no words no icons no typography no watermark. Flat front view, no perspective, no external shadow, subtle texture, cozy graffiti studio game. Opaque image suitable for rectangular UI panel, designed to remain legible when scaled small.
```

### OakPlanks_v7 — October 11, 2026

Built-in image generation; original 1254 × 1254 PNG. SHA-256: `15691c13d9ab4854cb209c1a737df65586f8a0f44d9f22fccec215663219b34d`.

```text
Use case: stylized-concept. Asset type: square seamless game environment albedo texture, warm natural oak plank flooring for a cozy cream-and-teal artisan studio. Strict orthographic top-down material scan, the entire image is wood flooring, no room or perspective. Eight parallel horizontal rows of long medium-width oak planks. Realistic fine straight oak grain running horizontally, occasional small subtle knots, staggered short-end seams; mostly long boards about four to six times longer than they are wide. Restrained warm honey-beige natural oak, soft brown hairline joints, very small brightness variation between boards, clean matte satin finish, elegant and lightly worn but no scratches or dirt. Tile seamlessly on all four edges: horizontal plank rows align at top and bottom, staggered board pattern repeats without a border. Even flat diffuse lighting with no cast shadows, no directional highlights, no vignetting, no baked reflections. Soft fine-grained texture, not orange, not parquet, no herringbone, no nails, no furniture, no objects, no text, no watermark, no frame. Opaque production base-color texture, square composition.
```
