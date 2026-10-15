# Concept scene and cosmetic variants V12

Open `Assets/Scenes/StudioConceptVariants.unity` in Unity **6000.0.77f1** and enter Play Mode. Start the existing round normally. This scene continues [material appearance V11](MATERIAL_APPEARANCE_V11.md) and is appended to Build Settings so the existing retry reloads it. The original prototype and V10/V11 scenes remain available. [README](../../README.md), [game design](../Design/GAME_DESIGN.md) and [roadmap](../Design/ROADMAP.md) remain the project entry point, behavior source and delivery plan.

For the latest room detail and color pass, open [concept studio detail V13](CONCEPT_DETAIL_V13.md). V12 remains the oak-floor baseline and cosmetic implementation source.

October 14 music configuration sync: this scene now assigns interface/game music for the learner's shared song-clock controller. A short isolated title/start/music-switch check passes; the full natural song check was run in PrototypeScene. See [Validation](../Project/VALIDATION.md). No gameplay code or art settings were changed by this sync.

The creator explicitly requested this cosmetic implementation on October 13, 2026. It uses the three original concept images referenced by `Website/assets.json`: `CozyGraffitiStudio_Concept.png`, `Balloon_Readability_v2.png` and `PaintBlaster_Concept_v1.png`. Concepts guide the colors, rounded shapes and studio composition; they are not implementation screenshots.

## Saved appearance

| Element | Implementation |
|---|---|
| Balloons | Six opaque latex pigments: teal, coral, gold, indigo, mint and rose. Three silhouettes: the original pear, a compact round body and a slender body. Each spawn selects one color and one silhouette once. The original knot, string, colliders, Rigidbody, movement and ten-point reward are retained. |
| Balloon burst | The selected pigment tints the existing latex/paint particle effect before playback. Alpha envelopes, emission counts, lifetime, cleanup and the one-time hit guard remain intact. No projected surface pattern is added. |
| Gun model | A new rounded cream receiver, colored molded grip, thick hollow muzzle, gold collars/button/strap/cap, cream trigger guard, colored trigger and small raised paint flecks. A clear horizontal reservoir reveals a separate pigment insert. Four opaque material batches and one transparent batch render the model. The previous visual is retained but disabled in this scene. |
| Gun initialization | Six coordinated themes randomly initialize the reservoir, grip, muzzle, trigger and lightly tinted cream body. Gold hardware stays readable. The chosen theme remains stable through firing and pause; scene reload/retry initializes a new random choice, which may repeat the preceding choice. |
| Spray and flying paint | The selected gun pigment colors its existing muzzle spray and every existing projectile visual immediately after creation. Each gun supplies its own shot color; there is no global shared paint-color variable. |
| Studio paint | Larger raised teal, coral and gold splats reuse the original spray artwork/alpha, with derived neutral texture copies for pigment tinting. Existing peripheral floor paint, swatches and bucket details receive the six-color palette. These marks are saved decoration, not new projectile-created decals. |
| Woodwork | Copied beam meshes have native rounded-box topology, regenerated lightmap UVs and wood coordinates scaled to their dimensions. Earlier beam meshes/materials are retained. |

The camera, camera placement, lights and postprocessing match V11. New static paint and woodwork have their own saved lightmaps and reflection under the inherited settings. Props remain peripheral; the runtime three-target budget is unchanged. The gun's muzzle helper remains at its original location, beyond the new barrel lip.

The reservoir is an alpha-blended glass approximation with a solid pigment insert. It has no refraction, simulated liquid level or fluid motion. Latex remains opaque. The normal first-person framing still crops the grip; gallery images use a temporary inspection pose that is restored after capture.

An actual-render comparison identified the enlarged gun's dark stippling as screen-space ambient occlusion: changing realtime shadow reception alone did not resolve it. Its opaque materials use a local derivative of installed URP Lit that omits the forward SSAO variant. Room SSAO, scene camera/lights/postprocessing, physical gun lighting, shadows and probe/reflection response remain enabled. The derivative retains the package license, source version/hash and exact changes under `ConceptVariantsV12/Shaders`; revisit it when upgrading URP.

## Runtime ownership and tuning

`StudioBalloonAppearance`, `StudioGunAppearance` and `StudioPaintTint` live under `Assets/StudioArtPack/Runtime`. Appearance initialization uses independent `System.Random` instances, preserving the gameplay random stream. Shared materials are never recolored at runtime: balloon instances select shared pigment assets; guns/projectiles use per-renderer material property blocks; particles use instance modules.

Two narrow learner-source hooks are authorized by this request: PlayerShooter passes a newly created projectile to the optional gun appearance component; Balloon passes its newly created burst to the optional balloon appearance component. Earlier scenes without these components retain their existing appearance. Spawn scheduling, aim, firing gates/cadence, collision, scoring, round duration and music are unchanged by the hooks.

Tune `StudioGunAppearance.themes` and its region bindings in the scene. Tune `StudioBalloonAppearance.pigments` and `silhouettes` in `ConceptVariantsV12/Prefabs/PlayableBalloonVariants.prefab`. Do not scale the physics root when authoring a cosmetic silhouette. Keep the complete scene, its lighting folder, the V12 asset folder and paired metadata together. `StudioConceptVariantsPass.Build` refuses to regenerate over an existing scene/asset folder.

## Evidence and limits

All previews under `Assets/StudioArtPack/ConceptVariantsV12/Previews` are actual 1600 × 900 Unity game-camera renders. `Before_Room` and `After_Room` use the same fixed three-target positions. `Balloon_Gallery` shows six fixed cosmetic examples; it is not the gameplay target count. `Gun_*` images temporarily enlarge/rotate the gun for review. `Runtime_Variants` shows naturally spawned targets during the isolated automated Play Mode run. Screen Space Overlay UI is excluded from camera captures.

[Verification.txt](../../Assets/StudioArtPack/ConceptVariantsV12/Verification.txt) records scene reload, saved lightmaps, missing-script checks and nine camera/muzzle ray and swept-shot paths across the motion rectangle. Creation compares learner component settings (allowing only the visual balloon prefab reference), scene colliders and camera/light settings with V11.

[PlayCheck.txt](../../Assets/StudioArtPack/ConceptVariantsV12/PlayCheck.txt) records 48 balloon initializations, all 18 color/shape combinations, 48 gun initializations, all six theme bindings, shared-material/random-state preservation, a method-driven synthetic shot with matching projectile/muzzle color, actual hit/+10 score, natural three-target refill, matching burst pigment, duplicate-hit protection and retry restoring Start/zero score/no targets. These checks do not accept physical mouse input, a complete natural round, sound, frame times or a playable release. Solo rhythm and controller acceptance remain governed by the roadmap.

Final tests used the concurrently saved learner scripts and covered particles already emitted on Awake, preserving their count, alpha and remaining lifetime. The repository metadata/link audit passed. The October 14 [renderer-reference repair](../Project/VALIDATION.md#render-references-repaired) resolved all eleven inherited pointers: the full Unity audit now reports zero missing scripts, zero unsupported Volume Profile records and zero unresolved GUIDs. Before/after room and gun renders show no visible regression; scene, camera, lighting, SSAO parameters and learner-source hashes were preserved.

## Provenance

No new AI raster or external download is introduced. The wall paint derives from existing `GraffitiSpray_Atlas_v3.png` strokes and alpha; their original files and recorded prompts remain untouched. New gun meshes, body variants and rounded beams are native procedural geometry. The pear mesh/knot retains its original attribution, and the preserved earlier weapon retains its Kenney provenance. The gun shader derives from the installed Unity URP package with its Unity Companion License preserved. Lightmaps, reflection and previews are Unity outputs. See [Credits](../../CREDITS.md).
