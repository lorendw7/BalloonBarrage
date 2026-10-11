# Paint blaster assets

The source BlasterKit contains eighteen gun variants. The assembled `Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab` references blaster-m, a reservoir, nozzle and muzzle particles. Preserve the original FBX and `colormap.png` relationship. The current V6 visible shell is described in the [studio guide](STUDIO_ART_PACK.md).

| Directory | Contents |
|---|---|
| Concepts | PaintBlaster_Concept_v1.png, a styling reference rather than mesh/UV data |
| Textures | PaintFX_Atlas_v1.png, four transparent paint elements |
| Materials | Teal, coral and gold accessory materials |
| Prefabs | Two reservoirs, two 3D droplets and eight static sprite visuals |
| Assembled | Gun prefab and independent PaintGun_M_Preview.unity |

Local +Z is forward. The original nested model rotates 180 degrees without modifying its FBX. `Muzzle/MuzzleSpray` is a six-particle burst with Play On Awake disabled. `GripPoint` is a placement reference, not hand IK. Gun decoration has no colliders.

The current gameplay includes paint-projectile motion, swept collision and balloon hits. Standalone visual templates do not acquire gameplay automatically: inspect each prefab's components and bindings. Sprite visuals do not fit ParticleSystem fields. Flat streak/spurt sprites point right and need camera/direction alignment; 3D droplets face +Z.

Reuse existing wall splats and latex fragments. Paint impact and balloon rupture are separate effects. Existing builders refuse to overwrite assembled output; do not regenerate over manual adjustments. Concept/atlas images use built-in image generation, and supplemental models use Unity geometry. Retain original model attribution in [Credits](../../CREDITS.md).

Recorded prompts below describe the concept, then the effects atlas.


## Recorded generation prompts

```text
Use case: stylized-concept. Asset type: concept design sheet for a cozy 3D game paint-spraying toy gun, not an actual 3D model or UV map. Landscape sheet on warm ivory paper, one large three-quarter view and one smaller clean side view of the SAME compact playful paint blaster. Chunky cream plastic body, teal grip, coral wide circular nozzle, mustard fittings, small translucent paint reservoir on top visibly filled with turquoise paint. Rounded friendly forms, soft satin highlights, tasteful paint smudges, plausible ergonomic silhouette. No military styling, no bullets, no flames, no real weapon branding. Show a short glossy turquoise paint jet and 3 detached viscous droplets exiting the large view nozzle, and a small matching paint tank accessory. Polished stylized 3D hand-painted game-art style consistent with honey-wood and cream graffiti studio. Clear negative space, no words, no letters, no arrows, no watermark. This is a design reference, not a UI screenshot.
```

```text
Use case: stylized-concept. Asset type: production transparent game VFX sprite atlas. Square image exactly 2 columns by 2 rows of four independent WHITE neutral glossy acrylic PAINT elements for runtime tinting. True transparent alpha background, generous transparent margins and gutters. Top-left: one plump round paint droplet with small trailing tail pointing left, traveling right. Top-right: one elongated horizontal viscous paint streak with rounded leading end at right and short uneven tail at left. Bottom-left: a compact paint nozzle spurt traveling right, 3 connected small lobes and 2 detached tiny droplets, no broad cloud. Bottom-right: a frontal wet irregular paint impact splat with rounded lobes, thick glossy center and few satellite droplets, NOT sharp explosion star. Thin soft gray internal shading and white specular highlights retain liquid volume. All four completely contained in their own cells, centered, cell edges blank, no shared elements, no overlap. No colored pigment: neutral white/gray ready for tint. No text, no checkerboard, no frame, no smoke, fire, metal, balloon fragments. Polished stylized game art; these are independent VFX variants, not sequential animation.
```
