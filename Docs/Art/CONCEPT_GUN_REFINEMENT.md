# Concept-style paint gun refinement

The October 19 refinement brings the production gun in `Assets/Scenes/PrototypeScene.unity` closer to the website's existing `PaintBlaster_Concept_v1.png`. Open that scene for the saved first-person placement. This is a cosmetic model update; the public v0.1.0-demo.2 Windows ZIP and website screenshot remain separate, unchanged artifacts.

The [README](../../README.md) remains the entry point, [game design](../Design/GAME_DESIGN.md) owns behavior, and [roadmap](../Design/ROADMAP.md) owns delivery. The exact concept-generation prompt remains in [Paint blaster](PAINT_BLASTER.md#recorded-generation-prompts).

## Saved appearance

- Rounded warm cream receiver, teal angled grip and cream heel/trigger guard.
- Short, flared coral nozzle with a recessed open bore, molded pads and honey-gold collar.
- Horizontal transparent reservoir with a separate opaque multicolor marbled fill, flat upper liquid surface and small cream calibration marks on both sides.
- Gold reservoir bands, fluted cap, curved rear return hose and circular side dials.
- Small raised teal/coral pigment flecks replacing the earlier dark vents and fasteners.

The original shell uses six combined mesh/material batches under `Assets/StudioArtPack/PaintBlaster/ConceptRefinement20261019`. The October 20 reservoir update uses eight batches: the original teal body batch excludes its liquid triangles, and separate mixed-liquid and calibration batches are added. The new assets live under `Assets/StudioArtPack/PaintBlaster/MixedPigment20261020`. Glass uses alpha blending; the liquid is static geometry without refraction or fluid animation. No runtime theme selection was added.

The reservoir's eight-hue marbling visually explains the gun's multicolor pigment supply. It does not predict the next projectile color, simulate mixing, consume ammunition or change the learner's projectile palette logic. The transparent glass, gold bands, receiver and grip keep their existing materials.

## Integration

`PaintBlaster/Assembled/PaintGun_M.prefab` keeps its GUID and the existing `RefinedGunVisual` transform/file ID. The new `WebsiteConceptShell_20261019` child sits inside that recoil root. Earlier cosmetic mesh children remain present but inactive, preserving their assets and identifiers. The original nested Kenney weapon is retained.

The scene's gun pose, scale, aim root, `gunVisual` binding, logical `Muzzle/MuzzleSpray` branch, audio, projectile reference and learner scripts are unchanged. New geometry ends before the logical muzzle at local Z 0.345. The shell contains only Transform, MeshFilter and MeshRenderer components. Other scenes sharing this visual inherit it unless they disable or override the branch; V12/V13 retain their separate concept shell.

## Verification and previews

`ConceptRefinement20261019/Verification.txt` records the original isolated Unity **6000.0.77f1** checks of the six-batch shell. `MixedPigment20261020/Verification.txt` records the subsequent eight-batch reservoir update: verified extraction of the original 896 liquid triangles, unchanged gameplay component data and existing local poses, saved recoil/muzzle bindings, material validity and camera near-plane clearance across nine aim/aspect combinations (16:9, 16:10 and 4:3). The isolated run completed with `MIXED_PIGMENT_OK` and exit code 0. Source hashes separately confirm that learner scripts and PrototypeScene remain unchanged during this pass.

`Previews/Before_FirstPerson.png` and `After_FirstPerson.png` are actual 1600 × 1000 Unity camera renders under the saved scene's lighting, with Canvas temporarily hidden. `Before_Model.png` and `After_Model.png` use the same temporary enlarged three-quarter inspection pose, restored without saving the scene. These are inspection renders, not recorded gameplay or generated concept imagery.

These checks do not establish physical mouse input, recoil feel during play, a complete song, sustained performance or a new playable release. Confirm the silhouette and firing feedback in normal Play Mode before exhibition acceptance.

The current reservoir previews are `MixedPigment20261020/Previews/Before.png` and `After.png` (1920 x 1080 saved game camera) and `Before_Model.png` / `After_Model.png` (1600 x 1000 temporary enlarged inspection pose). All use native Unity rendering, with UI hidden temporarily and scene poses restored without saving.

## Authoring and provenance

`Assets/StudioArtPack/Editor/ConceptGunRefinement.cs` builds in an isolated batch project under `.local-reference` and refuses to overwrite existing output. Preserve manual asset edits rather than regenerating over finished output. Rounded boxes, lathed surfaces, the clipped liquid volume and swept hose are native procedural geometry, with no new raster generation or external download.

Opaque materials reuse the licensed `ConceptVariantsV12/Shaders/StudioGunLit.shader`; its source/version/hash and Unity Companion License remain in that original folder. Glass derives from the existing V12 reservoir material. `Provenance.json` records source and output hashes. Original asset attribution remains in [Credits](../../CREDITS.md).

`Assets/StudioArtPack/Editor/MixedPigmentReservoir.cs` authors the reservoir update in an isolated `.local-backups` project. Its deterministic original 512 x 512 marbled texture uses turquoise, blue, violet, rose, coral, gold, lime and mint bands with softly mixed boundaries. It requires no external image, download or generated-image prompt. The original combined mesh is preserved; only the prefab's body mesh reference changes, with separate liquid UVs and calibration geometry saved as new assets. Preserve the existing prefab GUID and paired metadata.
