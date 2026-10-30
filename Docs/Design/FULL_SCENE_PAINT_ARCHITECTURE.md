# Full-scene paint architecture

Updated October 31, 2026. The creator explicitly authorized the room-wide implementation and cartoon-ink revision. This is saved source; package, performance and human acceptance are recorded separately in [Validation](../Project/VALIDATION.md). [Game design](GAME_DESIGN.md) owns behavior and originality; [Roadmap](ROADMAP.md) owns delivery.

## Prepared studio scope

The preparation tool registers **404 active opaque static mesh renderers**: room walls, floor, ceiling and static furniture/props, including curved meshes. It rejects inactive objects, glass/transparent/cutout materials, the camera/weapon, UI, balloons and moving Rigidbody/Animator roots. A new object becomes paintable after explicit preparation and contact validation. This is not automatic support for arbitrary scenes, transparent glass or moving objects.

Original meshes, materials, artwork and GUIDs remain underneath a separate paint coat. Exact static MeshColliders replace competing non-trigger contact shapes on registered objects; triggers remain. The prepared mesh surfaces are the contact geometry. The existing finite SphereCast and balloon hit/score path remain authoritative.

## Data and rendering

`RoomPaintPreparation` generates unique UV4 charts per receiver and packs them into one **4096 × 4096 linear ARGB32 atlas**. Color storage is **67,108,864 bytes (64 MiB)**, before driver overhead. MainWall and Floor receive 1024-pixel tiles; other surfaces receive 64–512-pixel tiles according to size. Chart resolution is finite, particularly on tiny detailed props.

Each receiver has an accumulation mesh with approximately 1.2-texel seam gutters. World position and UV expansion share the same affine mapping, so padding fills bilinear gaps without expanding visible geometry. The receiver manifest records source assets, generated stamp assets and tile bounds.

A single combined world-space coat mesh samples the atlas above the original opaque scene and decorative sprites. Identical position/normal/UV vertices are welded: **824,082 triangle vertices become 197,469 visible vertices**, a 76% reduction. This is one persistent paint renderer, independent of contact count. Underlying unpainted materials keep their original lighting; painted regions use a separate lit pigment coat, dark soft boundary, wet edge accent and gloss. Coat rendering still has a fixed geometry/fragment cost; one renderer is not a promise of zero cost or 120 FPS.

`RoomPaintSystem` reuses a command list, MaterialPropertyBlock and CommandBuffer. Accepted world contact, normal, radius, pigment and an existing original mask form transient commands. The accumulation shader projects the brush into the receiver UVs, rejects opposite-facing/deep fragments and blends premultiplied pigment. A bounded visibility ray permits nearby prepared corner surfaces. Commands flush once in LateUpdate; a synthetic queue over 4,096 commands flushes rather than dropping accumulated paint. Idle frames submit no paint pass. There is no full-atlas copy per contact and no permanent GameObject per painted spot.

## Continuous strokes and effects

Fast neighboring contacts from the same player and pigment receive visual intermediate samples on the starting/ending prepared surfaces. Normals blend across a turn; a bounded first-hit ray reprojects each sample onto real geometry rather than leaving its center inside a convex curve. A brush-width sweep conservatively rejects an intervening unrelated collider. Adjacent walls can join a stroke; opposite-facing contacts, a release, suspension, pigment change, gap above 0.20 seconds or jump above three units break the connection. Player identities remain separate. Interpolation adds pigment only, never damage, score, targets or rhythm events.

Corner spill probes aim slightly inside the neighboring bounds before requiring that neighbor's actual collider to be the first visible contact. Ending a ray exactly on a shared wall/floor edge could otherwise select the original wall and omit the neighbor. No permanent object, extra atlas or scoring query is introduced.

The original coverage mask atlas combines with an original procedural opaque body, chunky lobes and short vertical drips. Existing eight pigment groups coordinate direct deposits, the stream and balloon-pop paint. A twelve-slot-per-prefab pool reuses short impact particles; older permanent paint is never deleted to satisfy that pool. The nozzle presentation is slightly thicker; hit cadence, range and forgiving hit volume remain unchanged.

## Lifecycle and fallback

GPU storage initializes after the render pipeline starts and is checked before submissions. Paint persists through ordinary pause and results. Starting a new round clears it once; scene Retry destroys the old atlas, command buffer, coat state and effect pool. This does not provide saved paint across launches or recovery after graphics-device loss. New source retains the old sprite fallback for unprepared/unsupported situations; prepared studio contacts are tested to use the atlas exclusively.

## Verification and limits

The isolated Unity fixture uses actual colliders and GPU readback for all receivers and eight pigments, then 10,000 deposits with stable object count and atlas bytes. It checks opposite-face rejection, per-player stroke isolation, release boundaries, idle behavior, pause/results, a natural short-song end and five real scene reloads. Standalone long-song timing, exact package checks and physical/controller/venue/Mac observations remain distinct. See [Validation](../Project/VALIDATION.md) for actual results.

The reference to Splatoon concerns only broad, dense, readable cartoon ink and satisfying environmental color. No Nintendo character, UI, sound, map, weapon design, texture or gameplay rule is imported. Existing project artwork, attribution and exact recorded generation prompts remain unchanged.
