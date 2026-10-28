# Hit-effect contact handoff lesson

Updated: October 28, 2026. The creator explicitly authorized the contact-handoff scaffold after the external report that a balloon-hit effect sometimes appeared offset. [Game design](GAME_DESIGN.md#kyudaisai-feedback-revision--source-candidate) owns the intended behavior, the [roadmap](ROADMAP.md#immediate-lesson-queue) owns the remaining learner functions, and [Validation](../Project/VALIDATION.md#kyudaisai-source-integration-october-28) records the evidence boundary.

## Current evidence

`PlayerShooter` and `PaintProjectile` receive an accepted `RaycastHit`/`SphereCast` contact and now pass its point into `Balloon.HitAt`. `Balloon.Pop` uses that point for the independent fragment effect and on-beat glow. The compatibility `Hit(int)` path retains `popPoint`/root fallback behavior. Static source validation passes; the reported offset has not been reproduced and no visual fix is accepted yet.

## Saved implementation

The saved implementation adds `HitAt(Vector3 hitPoint, int bonusPoints = 0)`, preserves `Hit(int bonusPoints = 0)` as a compatibility entry point, and threads an explicit/fallback choice into `Pop`. Both production callers supply the sampled point. Scoring, the one-hit guard, collider disabling, wall-paint queries, on-beat scaling, audio and cleanup remain on their existing paths.

## Review boundary

Unity compilation and the focused static validation confirm that both production call sites reference `HitAt` and that the eight connected variants use world simulation. This source/API evidence does not prove visual alignment. In Play Mode, hit the center and visible edges from several screen positions, then pause/resume and Retry. Record any remaining offset with the exact song, pattern, screen area and source revision before adding an intentional normal offset.
