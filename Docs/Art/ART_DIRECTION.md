# Art direction

The current studio uses cream-grey plaster, warm oak/walnut, deep teal and restrained rust/brass accents, physically lit materials, window daylight and soft shadows. [Cover finish V9](COVER_FINISH_V9.md) adds subtle mineral/leaf surfaces, rounded joinery and saved offline interior lighting. Preserve the existing vibrant graffiti and keep props at the edges. The current scene supersedes the earlier pastel/toon-only concept.

The latest [concept studio detail V13](CONCEPT_DETAIL_V13.md) scene uses the website's original studio concept as its room reference: warm gray mineral floor, warm wood furniture, lighter teal niches, coral/cyan strip cores, coordinated pigment and woven-rug accents, rolled paint tins, crates, brushes and leaned canvas studies. It inherits [V12's](CONCEPT_VARIANTS_V12.md) six runtime balloon/gun pigments, three silhouettes and rounded gun with a clear horizontal reservoir. Earlier scenes remain baselines; each art scene has separate baked lighting and actual Unity captures.

Visual priority is reticles and balloons, hit feedback, HUD, then environment. Preserve recognizable pear silhouettes, knots, short strings and broad highlights. Reduce background interference before making targets smaller. Decorative ropes and props should not intercept gameplay collision queries.

Palette references: cream `#F4E6D0`, coral `#FF6B6B`, teal `#3EC6C0`, yellow `#FFD166`, indigo `#5B5F97`, charcoal `#30343F`. Future co-op reticles also use distinct shapes and player labels.

Judge composition in the 16:9 Game view, not Scene zoom. The current camera uses a 45-degree field of view. Keep the gun low and right with clear central target space. Validate UI at 720p and 1080p; mobile safe-area work is deferred.

Offset wall splats slightly to prevent depth flicker. Bound particles, decals and audio concurrency. Use text-free art with live localized labels. Concepts are visual references, not model assets or gameplay screenshots.

The requested gentle bobbing, entrance expansion, hit squash, small recoil and fading paint are defined as planned feedback in [Game design](../Design/GAME_DESIGN.md). Preserve root Rigidbody/aim/collider behavior and animate explicit visual children. The mandatory festival rhythm checkpoint precedes cosmetic expansion; use [Roadmap](../Design/ROADMAP.md) for delivery order.

Consult the [catalog](ASSET_CATALOG.md), [studio implementation](STUDIO_ART_PACK.md), [localization](LOCALIZATION.md), and [credits](../../CREDITS.md) before adding or replacing assets.
