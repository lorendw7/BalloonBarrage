# Original paint-spray style

The current prepared-room rendering contract is [Full-scene paint architecture](../Design/FULL_SCENE_PAINT_ARCHITECTURE.md). See [Game design](../Design/GAME_DESIGN.md) for player behavior and [Validation](../Project/VALIDATION.md) for evidence.

## Source artwork and provenance

The accepted nozzle atlas and transparent `PaintNozzleJet` material remain project-original artwork. `Assets/StudioArtPack/PaintSprayStyle20261025/Textures/PaintSprayVfx_Atlas_v3.png` remains a separate tintable candidate, with its exact generation prompt, source output, hash and import settings in its canonical `Provenance.json` and `Verification.txt`; do not describe it as newly connected gameplay.

Persistent pigment uses the original neutral-white four-shape atlas `Assets/StudioArtPack/SurfacePaintCoverage20261027/Textures/PaintCoverage_Atlas_v1.png`. Its puddle, smear, broad burst and bridge silhouettes retain their original source and exact generation prompt. Thirty-two collider-free eight-pigment prefabs remain source palette/mask data and a recoverable legacy sprite fallback. The prepared studio now accumulates their masks into mesh UVs rather than creating a permanent sprite per contact.

## Dense cartoon ink revision — October 31

The original accumulation shader adds a connected opaque core, short chunky lobes and small downward drips on vertical contacts. Existing neutral masks supply outer irregularity. The surface coat keeps pigment saturated, adds a darker soft rim, a restrained wet edge and light-dependent gloss. Main wall and floor have higher-resolution atlas tiles; UV gutters limit cracks. Curved opaque props use their actual prepared geometry. There is no projected floating sticker plane on these receivers.

Rapid neighboring same-player contacts are visually interpolated, so a sweep reads as a connected stroke. Different players, releases and suspended input cannot join strokes. Nozzle particles are 0.12–0.23 units with a 1.1-degree visual cone. The logical muzzle, six-per-second production cadence, 0.3-radius SphereCast, range, hit judgment and score are unchanged. Physical feel and readability still require observation.

Impact droplets are reused in a twelve-slot-per-effect pool. Deposited pigment has no timed fading or oldest-first removal. The GPU atlas and one combined coat renderer bound persistent paint resource use; the target PC's measured frame timings, rather than the number of renderers alone, determine performance acceptance.

The stream remains local-space, follows the current nozzle, shortens to its existing cursor contact and clears when fire becomes invalid. Its renderer stays above background decorative sprites; particle collision remains disabled. Balloon-pop deposits route through the same eight-pigment atlas path.

This revision studies the broad visual language of dense, playful wet ink. It introduces no Nintendo assets, characters, UI, maps, weapons, audio, territory scoring or movement rules. Original attribution and recorded prompts are preserved without modification.
