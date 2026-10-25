# Full-scene paint architecture

Status: bounded pre-festival stretch design, requested October 25. This document defines the technical direction; it is not saved gameplay, a release promise or evidence that arbitrary surfaces are paintable. Before feature freeze, “full scene” means every deliberately prepared, opaque, static receiver in the current studio, not glass, UI, balloons, particles, moving roots or arbitrary unprepared meshes. The existing four flat receivers remain the tested fallback. Delivery order and the October 27 stop gate live in [ROADMAP.md](ROADMAP.md), while player-facing behavior and the originality boundary remain in [GAME_DESIGN.md](GAME_DESIGN.md).

## Goal and constraints

The long-term experiment is a room or small explorable scene where firing visibly accumulates paint across eligible walls, floors and selected props. It should preserve the project's relaxing rhythm identity and borrow only the broad idea that player input changes the environment. It must not copy Splatoon assets, characters, UI, typography, weapon designs, sounds, maps or territory-control rules.

Required properties:

- Paint persists for the current round and clears on Retry or a new round.
- Eight existing pigments remain coordinated across stream, impact and accumulated surface color.
- Surface paint does not require one permanent GameObject or SpriteRenderer per hit.
- Runtime memory is bounded by prepared render textures, not by deleting older player paint.
- The fixed-camera exhibition scene remains recoverable while the system is developed in a separate graybox.
- The performance target remains 2560 x 1440 at 120 FPS on the exhibition PC, with an 8.33 ms frame budget subject to measurement.

Not included in the first slice: territory scoring, swimming in paint, team ownership, network replication, save-across-launches, arbitrary transparent receivers, moving receivers or a large map.

## Proposed data flow

```text
accepted paint contact
        |
        v
PaintSurface lookup ---- reject non-paintable receiver
        |
        v
world contact + normal -> surface UV / planar coordinates
        |
        v
reused PaintStamp command queue
        |
        v
one batched GPU paint pass per affected atlas tile
        |
        v
shared paint RenderTexture sampled by receiver materials
```

The current SphereCast remains authoritative for target hits and rhythm scoring. A paint contact produces a visual stamp command only after receiver eligibility is known. Painting must never create a second damage or score path.

## Components and contracts

| Component | Responsibility | Important boundary |
|---|---|---|
| `PaintSurface` | Registers one renderer, projection mode and atlas tile | No per-frame work when no stamp arrives |
| `PaintSurfaceRegistry` | Resolves a hit collider to its surface | Use references/layers, not fragile object-name checks |
| `PaintStamp` | Value-type command containing surface id, coordinate, radius, rotation, pigment and pattern | No Unity object allocation per hit |
| `PaintSurfaceSystem` | Reuses a command list, groups stamps by tile and submits draw commands | Commands are transient; accumulated pixels persist |
| Paint accumulation shader | Blends a neutral mask and pigment into the selected atlas region | Original/attributed masks only; no gameplay logic |
| Receiver material variant | Composites base material with the shared paint atlas | Preserve lighting, normals and existing base texture |

## Surface mapping

The first slice supports explicit projection modes rather than claiming arbitrary geometry:

1. `PlanarXY`, `PlanarXZ` and `PlanarYZ` map a world contact through the receiver's local bounds. These cover the room's walls and floor without replacing their colliders with MeshColliders.
2. `MeshUV0` uses `RaycastHit.textureCoord` only on prepared meshes with verified non-overlapping UVs and compatible colliders.
3. Curved props enter one at a time after UV seams, stretching and contact normals pass a visual check.

Transparent glass, particles, UI, balloons and moving gameplay roots are initially ineligible. Small unprepared props may keep a bounded transient impact effect without receiving persistent paint.

## Rendering and memory baseline

Start with one 2048 x 2048 ARGB32 paint atlas with no depth buffer. Its color storage is approximately 16 MiB before platform-specific overhead. Divide it into prepared tiles for the main wall, side walls and floor. Submit reusable quads into the selected tile with normal alpha blending; do not copy the whole atlas for each stamp. A 4096 atlas or per-surface textures require measured evidence because they can multiply memory and bandwidth.

The current SpriteRenderer marks remain the recoverable baseline until the texture path reproduces their contact position, pigment and round persistence. Do not run both permanent systems in production after migration; temporary A/B comparison belongs in the separate graybox.

Performance rules:

- Reuse command lists, MaterialPropertyBlocks and command buffers.
- Perform no atlas work in frames with no new stamp.
- Batch all stamps received in one frame by surface tile.
- Keep transient nozzle and impact particles capped independently of persistent paint.
- Clear the atlas once on Retry/new round; never clear and rebuild it every frame.
- Record CPU/GPU frame time, GC allocations, atlas memory and submitted stamp count in the diagnostic build.

## Delivery slices

1. **Learner contract prototype:** one copied graybox wall, one pigment, planar mapping, clear-on-Retry and a diagnostic stamp counter. The learner writes the runtime components; the maintainer prepares art, materials, validation and review.
2. **Performance proof:** 1,000 scripted contacts without growing GameObject count; compare 1024 and 2048 atlas resolution at 2K.
3. **Production-room slice:** main wall and floor, eight pigments, three original mask families and current scoring unchanged.
4. **Prepared-room migration:** move the two side walls, then add only opaque static furniture with an explicit projection or verified UV mapping. Reject unsuitable props rather than silently smearing paint.
5. **Feature-freeze decision:** by October 27, retain the prepared-room path only if contact, Retry, color agreement and performance pass. Otherwise keep the four-receiver baseline for Kyudaisai and continue the bounded atlas migration after the event.
6. **Post-festival expansion:** evaluate curved props, larger environments and an explorable graybox only after the exhibition baseline is preserved.

## Acceptance gates

- A contact appears at the sampled position and does not float, mirror or cross a UV seam unexpectedly.
- Earlier paint remains through the song, pause and results; Retry clears it exactly once.
- Pigment agrees with the stream/impact source and never changes score.
- After warm-up, ordinary firing frames add no managed allocation from the paint accumulation path.
- 1,000 contacts do not add persistent scene objects or increase atlas memory.
- A clean standalone 2K run records p95 frame time within the accepted budget, no repeated long-frame pattern and stable memory through ten Retries.
- Balloons, reticle, beat cue and Simplified Chinese/Japanese/English UI remain readable over dense paint.

## Trade-offs and later review

Render-texture accumulation gives bounded object count and immediate persistence, but resolution is finite and every receiver needs deliberate coordinate mapping. Decals are easier for a few arbitrary meshes but preserve a per-mark object/draw-management problem. Runtime mesh painting offers physical surface detail but is too complex for the first slice. Revisit sparse virtual textures, compute-based stamping, save serialization and territory analysis only after the small-room slice is measured and accepted.
