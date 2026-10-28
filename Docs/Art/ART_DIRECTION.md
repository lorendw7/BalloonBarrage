# Art direction

Updated: October 28, 2026. [Asset catalog](ASSET_CATALOG.md) owns asset locations; [Roadmap](../Design/ROADMAP.md) owns production promotion and delivery.

The current studio uses cream-grey plaster, warm oak/walnut, deep teal and restrained rust/brass accents, physically lit materials, window daylight and soft shadows. [Cover finish V9](COVER_FINISH_V9.md) adds subtle mineral/leaf surfaces, rounded joinery and saved offline interior lighting. Preserve the existing vibrant graffiti and keep props at the edges. The current scene supersedes the earlier pastel/toon-only concept.

The production [October 20 color and surface refresh](STUDIO_COLOR_REFRESH.md) adds asymmetric pigment splashes and dry-brush marks to both side walls, increases shadow-side fill, reduces the window hotspot and uses an independent, lighter color grade. World-scale limewashed plaster and a warm gray mineral floor bring the room closer to the original website studio concept while preserving the central aiming area and timber furnishings. A new indirect-light/reflection bake remains a follow-up proposal.

The earlier [concept studio detail V13](CONCEPT_DETAIL_V13.md) scene uses the website's original studio concept as its room reference: warm gray mineral floor, warm wood furniture, lighter teal niches, coral/cyan strip cores, coordinated pigment and woven-rug accents, rolled paint tins, crates, brushes and leaned canvas studies. It inherits [V12's](CONCEPT_VARIANTS_V12.md) six runtime balloon/gun pigments, three silhouettes and rounded gun with a clear horizontal reservoir. These scenes remain separate from production and retain their recorded lighting/capture evidence.

The [October 28 studio expansion](STUDIO_EXPANSION_20261028.md) continues the same warm studio language through a separate art study, reusable original props and text-free artwork. Use larger quiet shapes around the central aiming area, with detail concentrated at the room's edges. Pair a clear focal artwork with supporting supplies rather than filling every surface. Its guide records exact contents and verification; it does not replace demo.6 or establish new gameplay support.

Visual priority is reticles and balloons, hit feedback, HUD, then environment. Preserve recognizable pear silhouettes, knots, short strings and broad highlights. Reduce background interference before making targets smaller. Decorative ropes and props should not intercept gameplay collision queries.

Palette references: cream `#F4E6D0`, coral `#FF6B6B`, teal `#3EC6C0`, yellow `#FFD166`, indigo `#5B5F97`, charcoal `#30343F`. Future co-op reticles also use distinct shapes and player labels.

Judge composition in the 16:9 Game view, not Scene zoom. The current camera uses a 45-degree field of view. Keep the gun low and right with clear central target space. Compare art changes at the same camera and resolution; check early and paint-heavy frames before promoting an environment. Validate affected UI at 720p, 1080p and 1440p and preserve the existing aspect-ratio support; mobile safe-area work is deferred.

Offset wall splats slightly to prevent depth flicker. Bound particles, decals and audio concurrency. Use text-free art with live localized labels. Concepts are visual references, not model assets or gameplay screenshots.

Bounded recoil and round-persistent paint are saved; additional gentle bobbing, entrance expansion and hit squash remain proposed in [Game design](../Design/GAME_DESIGN.md). Paint does not fade or disappear because of a count cap. Preserve root Rigidbody/aim/collider behavior and animate explicit visual children. New decorative props have no gameplay colliders and are not automatically paint receivers. Use shared materials and restrained shadow casting, then measure before claiming a performance improvement.

Keep original textures and exact image-generation prompts with provenance. Generated concepts are references; native Unity captures show a specific scene; released gameplay captures show a specific executable. Website captions must retain these distinctions. Art promotion follows the roadmap's focused native comparison and verification steps before any new desktop package.

Consult the [catalog](ASSET_CATALOG.md), [studio implementation](STUDIO_ART_PACK.md), [localization](LOCALIZATION.md), and [credits](../../CREDITS.md) before adding or replacing assets.
