# Concept studio detail V13

Open `Assets/Scenes/StudioConceptDetail.unity` in Unity **6000.0.77f1**. This scene continues [concept variants V12](CONCEPT_VARIANTS_V12.md), using the original `CozyGraffitiStudio_Concept.png` as its room reference. The earlier scenes and their lighting remain available. [README](../../README.md), [game design](../Design/GAME_DESIGN.md) and [roadmap](../Design/ROADMAP.md) remain the entry point, behavior source and delivery plan.

## Saved scene

October 14 music configuration sync: the existing interface source and added prepared game source are assigned to the learner's song-clock controller. A short isolated title/start/music-switch check passes; natural complete-song acceptance was run in PrototypeScene. See [Validation](../Project/VALIDATION.md). Art/camera/light/collider settings and gameplay code were preserved.

- A warm gray mineral floor replaces oak in this scene. Its restrained, deterministic procedural albedo separates the colored paint from the warm wood furniture and shows the existing window-shadow shapes. Geometry and collision are retained; it is an appearance approximation, not a scanned concrete surface.
- Teal niches are lighter; wall splats use richer copied teal/coral/gold pigments. Bucket bodies, wet tops and drips now share coordinated pigment colors. The rug uses teal, rust and ochre woven material copies.
- Teal and coral strip cores retain their colors under the inherited ACES/Bloom settings. The first white-clipped trial was corrected to a 0.60 emission gain. Existing light components are unchanged; their material-dependent bake is refreshed.
- Six shelf tins are rebuilt visually with rolled metal rims, wet paint tops, feet, paper labels, pigment badges, drips and handle lugs. Additional supplies include two open slatted crates, stacked tins, a used brush cup, paint tubes and two leaning abstract paint studies.
- The final placement-only adjustment separates the new studies, crates and loose tins from the stage, cabinet and cart. Paint tubes now rest on the cabinet worktop. The right study has a shallower lean; the game camera and existing furniture stay in place.
- Six irregular peripheral floor marks and twelve small droplets reuse the existing paint artwork/geometry. Their placement is saved decoration; no new projectile decal behavior is introduced. Large details stay near the edges and the three-target play area remains clear.

The 241 added mesh parts are combined into **20 material batches**. That is an authoring/draw-organization count, not a measured performance result. Added decoration has no colliders or runtime behaviours. Existing physics, learner components, input/UI configuration, camera framing and light component settings match V12. No gameplay source was edited for this pass. Six-color balloons, three silhouettes, coordinated gun/paint themes and their existing optional tint hooks are inherited from V12.

## Evidence

`ConceptDetailV13/Previews/Before_Room.png` and `After_Room.png` are actual 1600 × 900 Unity renders with the same fixed three-target layout and game camera. `Runtime_Variants.png` captures natural spawning during the final isolated Play Mode check. `Workbench_Detail.png` and `Supplies_Detail.png` temporarily reposition the same camera to inspect props; these views are restored and are not gameplay camera changes. Screen Space Overlay UI is excluded.

[Verification.txt](../../Assets/StudioArtPack/ConceptDetailV13/Verification.txt), [VisualReview.txt](../../Assets/StudioArtPack/ConceptDetailV13/VisualReview.txt), [ClearanceReview.txt](../../Assets/StudioArtPack/ConceptDetailV13/ClearanceReview.txt) and [PlayCheck.txt](../../Assets/StudioArtPack/ConceptDetailV13/PlayCheck.txt) distinguish the saved scene, visual/clearance review and automated runtime checks. Close inspection corrected studies crossing the stage front, a hidden tin/crate and canvas paint escaping its edge; combined mesh/material writes explicitly retain their existing GUIDs and mark the assets dirty before saving. Scene reload, saved lightmaps, all nine camera/muzzle paths across the complete target-motion rectangle and the full Unity reference audit passed. Play Mode checks cover cosmetic initialization/material/random-state preservation, an accepted method-driven synthetic shot, matching pigment, hit/+10 score, natural refill, burst color/duplicate-hit protection and retry restoring Start/zero score/no targets in this scene.

Human mouse input, a natural complete round, audio, frame times, exhibition-PC acceptance and a playable release remain unverified. The previous renderer-reference repair remains in place; no validation exclusions or ignored GUIDs are added.

[PositionReview.txt](../../Assets/StudioArtPack/ConceptDetailV13/PositionReview.txt) records the final placement-only request. Final lightmaps/reflection and previews were generated after reloading the saved meshes in a fresh Unity process, avoiding stale native render buffers during in-place mesh updates.

## Tune and preserve

Tune copied materials under `ConceptDetailV13/Materials`, including `WarmMineralFloor`, `AiryTealNiche`, `Pigment_*`, `WallPaint_*` and the two `*LightCore` materials. Static color/emission or geometry edits require rebaking this scene and refreshing its reflection. Keep its scene, paired lighting folder and complete V13 asset folder/metadata together. The new scene is appended to Build Settings for the existing retry callback; the prototype remains first.

`StudioConceptDetailPass.Build` is an isolated-project editor utility and refuses to regenerate over existing V13 output. `RefineReview` is guarded after the saved review. The shared cosmetic Play Mode harness accepts this scene through `RunDetailBatch`; it does not modify gameplay. Open V12 to return to the preceding oak-floor room.

## Provenance

No new AI image or external download was used. The mineral texture is native deterministic procedural data. Supplies use procedural rounded boxes/lathed meshes and Unity's sphere; wall/floor/canvas paint reuses the existing V12 relief and spray-alpha derivatives. Existing wood, woven surfaces, original concepts, attribution, licenses and exact recorded image-generation prompts are retained. The V12 gun shader and its Unity Companion License remain unchanged. Lightmaps, reflection and previews are Unity outputs. See [Credits](../../CREDITS.md).
