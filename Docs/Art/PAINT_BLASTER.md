# Paint blaster assets

The source BlasterKit contains eighteen gun variants. The assembled `Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab` retains blaster-m and its original helpers. Its production visual uses the [October 19 concept-style refinement](CONCEPT_GUN_REFINEMENT.md), with a horizontal clear reservoir, cream body and coral bell nozzle. Preserve the original FBX and `colormap.png` relationship. The earlier V6 shell remains as inactive cosmetic children; its authoring is described in the [studio guide](STUDIO_ART_PACK.md).

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


## Paint-shot sound and held-gun framing

The October 16 handoff supplies `Assets/Audio/SFX/PaintShot_AirPuff.wav`: an original 175 ms mono PCM16/44.1 kHz trigger-and-paint sound, currently revision 3. `Tools/Audio/synthesize_paint_shot.py` records its fixed seed, processing, level measurements and SHA-256 in the accompanying `PaintShot_Provenance.json`, retaining earlier revision hashes. It uses no external recordings or song samples. This is a firing sound; balloon rupture and surface-impact sounds remain separate.

In PrototypeScene, `PaintGun_M/PaintShotAudio` is a dedicated AudioSource with the clip assigned: volume 0.50, pitch 1, Spatial Blend 0, Doppler 0, reverb mix 0, Play On Awake off, Loop off, and Ignore Listener Pause off. Import uses PCM, Decompress On Load and preloaded audio. One short source with `Play()` bounds this lesson's playback to one voice at the existing five shots per second. Listening against the actual song and physical-input acceptance remain required.

Revision 3 adds a brief 1.75/2.87 kHz mechanism click, a descending wet body, a short pressure resonance, filtered air and three liquid transients. A 0.6 ms attack keeps the onset immediate; a 22 ms end fade leaves a 25 ms gap at five shots per second. Soft saturation gives the body presence while retaining headroom: -14.33 dBFS RMS and -2.16 dBFS peak, with source volume still 0.50. The same filename/GUID retains existing references. Local single/burst and song-mix previews live under the ignored `.local-reference/PaintShotV3` directory; they are development auditions, not listening acceptance. Confirm clarity against the actual song on the exhibition output device before accepting the mix.

The saved camera-child gun pose is local position `(0.40, -0.32, 1.30)`, uniform scale `0.65`. The October 16 follow-up lowers only the scene gun root by 0.02 local units from Y -0.30; X/Z, rotation, scale and the muzzle child pose remain unchanged. This preserves the cosmetic rest-pose cache and keeps the saved recoil relative to the same visual child. The saved cosmetic recoil is described below.

PlayerShooter's `shotAudioSource` references the scene PaintShotAudio child. Its Play call follows projectile creation and rhythm initialization, after all play/pause/UI/release/cadence guards. **Tools > BalloonBarrage > Connect Paint Shot Audio** can repair a missing scene reference after compilation; it does not write gameplay. Do not add a second cadence timer or raw-input audio listener. The creator reports the revised firing sound passed; whole-song/exhibition-device balance and fresh-package acceptance remain separate gates in [Validation](../Project/VALIDATION.md).

## Cosmetic recoil handoff

PrototypeScene's assembled gun has a separate active `RefinedGunVisual` mesh child. The logical `Muzzle/MuzzleSpray` branch and shot audio are outside it. **Tools > BalloonBarrage > Connect Gun Recoil Visual** connects the saved gunVisual field to that scene child and marks the scene dirty for explicit saving. It refuses a conflicting reference or an unsafe hierarchy and does not create gameplay fields or animation.

The learner has saved the visual reference, Awake rest-pose cache, accepted-shot KickGun and LateUpdate recovery. Current tuning is 0.035 m backward, -6 degrees pitch and smoothing rate 18; pause holds recovery. Each kick derives from the cached rest pose, so held fire does not accumulate offsets. Only the cosmetic child moves; the aiming root, camera and logical muzzle remain independent. ResetGunPose is now called outside active play and from OnDisable, immediately restoring the cached visual pose. The creator reports recoil and settlement/retry reset passed; the lower gun position still needs a fresh visual/physical-input confirmation; see [Validation](../Project/VALIDATION.md) and the [lesson queue](../Design/ROADMAP.md#immediate-lesson-queue).

## Recorded generation prompts

```text
Use case: stylized-concept. Asset type: concept design sheet for a cozy 3D game paint-spraying toy gun, not an actual 3D model or UV map. Landscape sheet on warm ivory paper, one large three-quarter view and one smaller clean side view of the SAME compact playful paint blaster. Chunky cream plastic body, teal grip, coral wide circular nozzle, mustard fittings, small translucent paint reservoir on top visibly filled with turquoise paint. Rounded friendly forms, soft satin highlights, tasteful paint smudges, plausible ergonomic silhouette. No military styling, no bullets, no flames, no real weapon branding. Show a short glossy turquoise paint jet and 3 detached viscous droplets exiting the large view nozzle, and a small matching paint tank accessory. Polished stylized 3D hand-painted game-art style consistent with honey-wood and cream graffiti studio. Clear negative space, no words, no letters, no arrows, no watermark. This is a design reference, not a UI screenshot.
```

```text
Use case: stylized-concept. Asset type: production transparent game VFX sprite atlas. Square image exactly 2 columns by 2 rows of four independent WHITE neutral glossy acrylic PAINT elements for runtime tinting. True transparent alpha background, generous transparent margins and gutters. Top-left: one plump round paint droplet with small trailing tail pointing left, traveling right. Top-right: one elongated horizontal viscous paint streak with rounded leading end at right and short uneven tail at left. Bottom-left: a compact paint nozzle spurt traveling right, 3 connected small lobes and 2 detached tiny droplets, no broad cloud. Bottom-right: a frontal wet irregular paint impact splat with rounded lobes, thick glossy center and few satellite droplets, NOT sharp explosion star. Thin soft gray internal shading and white specular highlights retain liquid volume. All four completely contained in their own cells, centered, cell edges blank, no shared elements, no overlap. No colored pigment: neutral white/gray ready for tint. No text, no checkerboard, no frame, no smoke, fire, metal, balloon fragments. Polished stylized game art; these are independent VFX variants, not sequential animation.
```
