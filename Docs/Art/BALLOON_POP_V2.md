# Latex-fragment balloon pop

For the prepared combined latex-and-paint burst and the current wiring exercise, see the [paint-pop lesson](PAINT_POP_LESSON.md). The existing gameplay prefab still references the original teal fragment effect until the learner connects the new version.

Folder: `Assets/StudioArtPack/BalloonPopV2`. The generated `LatexFragments_v1.png` is a transparent 2×2 atlas of neutral latex scraps, tinted through a shared unlit material. Teal, Coral and Gold particle prefabs share the texture.

Each burst emits twelve particles with 0.38–0.68-second lifetime, size 0.22–0.4, speed 2.4–4, gravity, random spin and late fade. Each particle retains one randomly selected cell; the atlas is not sequential animation. XY emission suits the current front camera and does not simulate general 3D mesh fracture.

Assign the matching particle prefab to the spawned balloon's Pop Effect Prefab and retain Pop Point. Its root ParticleSystem fits that field. Keep spawned effects independent of the balloon so destroying the target does not remove the burst early. Stop action destroys the effect; existing timed cleanup remains compatible.

Learner code controls shrinking/hiding and one-time scoring. A short shrink may still look like deflation; texture changes cannot implement rupture timing. Preserve the older PaintBurst asset where referenced. Check runtime appearance with the actual camera and target density.


## Recorded generation prompts

```text
Use case: stylized-concept. Production game VFX texture atlas, square, exactly 2 columns by 2 rows, FOUR isolated individual torn LATEX BALLOON FRAGMENTS, one centered in each equal cell. True transparent alpha background including generous empty gutters and margins. Each fragment is a thin flexible piece of popped rubber, irregular soft torn outline, curling edges, a subtle folded crease and soft satin highlight. Top left a curled triangular scrap, top right a folded crescent scrap, bottom left a stretched ribbon scrap, bottom right a small crumpled balloon-neck scrap. All in neutral ivory WHITE with soft light gray shading, ready to tint any color in a particle shader. Each fragment fits within central 60 percent of its cell, equally scaled. Believable thin rubber material, polished stylized 3D appearance matching a cozy hand-painted balloon game. Flat orthographic isolated sprite assets, no scene, no text, no grid, no checkerboard painted, no shadows cast on background, no glow, no smoke, no fire, no paint splashes, no intact balloons. Not a sequential animation: four independent fragment variations.
```
