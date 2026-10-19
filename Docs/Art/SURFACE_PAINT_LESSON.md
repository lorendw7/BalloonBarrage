# Lesson: projectile impacts leave wall paint

Updated: October 19, 2026. Persistent balloon-pop marks and direct projectile marks pass isolated checks on MainWall, Floor, LeftWall and RightWall. The saved palette now has eight colors; the [palette guide](BALLOON_COLOR_LESSON.md) owns their indexed references, art provenance and verification status. The next learner step checks complete-song coverage and readability before measured load. Controller/co-op remains deferred. [Roadmap](../Design/ROADMAP.md) owns scheduling and [game design](../Design/GAME_DESIGN.md#planned-paint-and-color-expansion) owns surface eligibility and lifecycle policy.

## Reviewed starting point

`Assets/Scripts/PaintProjectile.cs` SphereCasts each movement segment. Balloon hits retain the event-matched 10/20-point award. The non-balloon MainWall branch instantiates an independent mark at `hit.point + hit.normal * 0.005f` with `Quaternion.LookRotation(hit.normal)`, then reaches the original projectile destroy/return. The learner removed the delayed mark destruction. An isolated Play Mode check passed actual projectile collision/creation, retention beyond eight scaled seconds, pause, settlement-callback retention and scene-retry cleanup. Direct ordinary/bonus hit checks awarded 10/20 once. This is lifecycle acceptance, not natural full-song or persistent-paint performance acceptance; see [Validation](../Project/VALIDATION.md#persistent-mainwall-paint-october-18).

The creator also reports [three-color balloons](BALLOON_COLOR_LESSON.md), matching bursts and 10/20-point scoring passed. [Local best score](../Design/BEST_SCORE_LESSON.md) is saved and independently checked. The creator requested assistant-led acceptance and continued gameplay teaching. There is no paint-count cap or oldest-first removal plan. The public package remains unchanged; persistent paint needs full-song load regression before exhibition packaging.

PrototypeScene's front target wall is **MainWall**, with an enabled, non-trigger BoxCollider. Do not confuse it with BackWall behind the camera. The saved shooter references `Assets/StudioArtPack/PaintBlaster/Prefabs/PaintDrop3D_Teal.prefab` (GUID `1503380901ff47c44b942dfabf1913a2`). Modify its root PaintProjectile reference when the learner adds the new field, rather than an in-flight clone or decorative paint object.

## Prepared art

| Asset | Purpose |
|---|---|
| `Assets/StudioArtPack/SurfacePaint/SurfacePaint_Teal.prefab` | Small flat impact mark; SpriteRenderer only, no collider, Rigidbody or runtime script |
| `Assets/StudioArtPack/SurfacePaint/SurfacePaint_Coral.prefab` | Same safe mark settings with the existing Coral_Burst atlas sprite |
| `Assets/StudioArtPack/SurfacePaint/SurfacePaint_Gold.prefab` | Same safe mark settings with the existing Yellow_Drip atlas sprite |
| `Assets/StudioArtPack/SurfacePaint/SurfacePaint_Teal.mat` | Dedicated copy of the existing transparent URP sprite-unlit material; does not modify shared decorative marks |
| `Assets/StudioArtPack/Textures/GraffitiSpray_Atlas_v3.png` | Existing transparent atlas, Teal_Spray slice; no new image generation or download |
| `Assets/StudioArtPack/SurfacePaint/Previews/WallMark_Detail.png` | Native Unity close-up on a wall; asset preview, not a recorded projectile hit |

The 512 × 512 source slice has a centered pivot, 200 pixels/unit and prefab scale 0.18: its full square footprint is 0.4608 world units. Sprite alpha is 0.90. The root uses Ignore Raycast, has no collider and casts/receives no shadows. The supplied shader uses alpha blending, Cull Off and ZWrite Off. The original atlas and exact recorded prompt remain unchanged; see [Spray assets](SPRAY_ASSETS.md#recorded-generation-prompts). `Provenance.json` records source hashes and preparation settings.

This is a flat mark, not a projector or surface texture rewrite. Both pop deposits and projectile impacts now accept MainWall, Floor, LeftWall and RightWall. Curved furniture, edge clipping and additional material receivers need separate work. The learner connected the production palette.

## First learner increment: expose and connect the prefab

In PaintProjectile, directly below the existing lifetime field and inside the class, add:

```csharp
[SerializeField]
private GameObject paintMarkPrefab;
```

`SerializeField` lets Unity save/show the private field in the Inspector. `GameObject` accepts the entire paint prefab. The variable is a template reference; declaring it does not instantiate paint.

Save and wait for compilation. In Project, select `PaintDrop3D_Teal.prefab`, select its root, and find **Paint Mark Prefab** on PaintProjectile. Drag `SurfacePaint_Teal.prefab` into that field and save. Do not drag it into Hierarchy or add a second collider. Confirm the reference before the next small code edit. The assistant prepares assets and checks; the learner writes and connects gameplay.

## Subsequent small increments

1. Recognize a non-balloon hit on MainWall, preserving balloon scoring.
2. Use `hit.point` for the contact position and `hit.normal` for the outward surface direction. Place the mark at `hit.point + hit.normal * 0.005f` and align its local forward with `Quaternion.LookRotation(hit.normal)`. The small offset prevents coplanar flicker; do not use the projectile center as the wall contact point.
3. Instantiate the referenced prefab before the existing projectile destruction. Keep its supplied scale and do not parent it to the projectile. First confirm one shot produces one mark while balloon hits retain their award/effects.
4. Round persistence is now saved and passed the focused lifecycle check below. No maximum-count or oldest-first-removal step is planned.
5. Teach matching balloon-pop wall deposits before general surfaces, without changing shared scene materials. Preserve the existing short-lived particle burst.

## Accepted learner increment: keep wall marks for the round

In the existing MainWall branch of `PaintProjectile.Update`, replace these statements:

```csharp
GameObject mark = Instantiate(paintMarkPrefab, position, rotation);
Destroy(mark, 8f);
```

with:

```csharp
Instantiate(paintMarkPrefab, position, rotation);
```

Instantiation creates an independent scene object. Removing the mark's delayed destruction leaves it present during the song, pause and results. Keep `Destroy(gameObject)` below the branch: that destroys the projectile, not the independent mark. The current `RestartRound` reloads the scene, which removes these scene-owned marks; no persistent object or `DontDestroyOnLoad` is needed. A future return-to-title flow must also clear them explicitly or reload the scene.

The focused check passed retention after more than eight seconds, during pause and after invoking the saved settlement callback. Retry removed old marks and reset score, and a new round began cleanly. Natural full-song paint coverage and persistent-mark performance remain unverified. Keep the change separate from an accepted exhibition build until those checks pass.

## Saved learner increment: expose the pop mark palette

In `Balloon`, below the existing `popEffectPrefab` field, add:

```csharp
[SerializeField]
private GameObject[] wallPaintPrefabs;
```

An array keeps one prefab per existing appearance color index. After compilation, select `Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab`, set **Wall Paint Prefabs** size to 3, and connect:

| Element | Prefab | Existing appearance pigment |
|---|---|---|
| 0 | SurfacePaint_Teal | TealLatex |
| 1 | SurfacePaint_Coral | CoralLatex |
| 2 | SurfacePaint_Gold | GoldLatex |

The order matches `StudioBalloonAppearance.ColorIndex`; do not randomize a second color. Coral and gold reuse the atlas's existing Coral_Burst and Yellow_Drip slices, with the original prompt/provenance retained. They are prepared art only; no production balloon reference has been connected automatically.

October 18 source/reference review confirms the serialized array and all three saved GUID/fileID references in this order. PrototypeScene's spawner points to this checked three-color prefab. Subsequent Play Mode checks now also cover pop-created marks on all four eligible surfaces, as recorded below.

## Next learner increment: find a wall contact and leave a matching mark

Checkpoint: the helper and its Pop call are saved. The six-direction version now passes twelve surface/color combinations on the actual MainWall/Floor/LeftWall/RightWall colliders, with blocker/range/score/lifecycle checks. See [Validation](../Project/VALIDATION.md#six-direction-balloon-pop-receivers-october-18). The following paragraph describes the original first-wall exercise; current saved behavior extends it as described afterward.

Teach one bounded contact query first: from the balloon root, cast along world positive Z toward MainWall, with a three-unit maximum range and trigger hits ignored. Only accept the first hit if its collider is MainWall; do not paint through another obstacle. This matches the current front-wall setup, not every surrounding surface.

Select the mark using the existing StudioBalloonAppearance.ColorIndex, guarding missing appearance, missing array, out-of-range index and missing prefab. Instantiate at the ray's actual hit point plus 0.005 units along its normal; align forward with that normal. The mark stays independent and uses the accepted round-persistent lifecycle.

Call the helper once from Pop after the existing collider-disabling loop, outside the optional particle-effect branch. This prevents the balloon from blocking its own query and preserves the one-time hit guard, existing score and short-lived particle cleanup. Do not place a mark at the floating balloon position or recolor a shared material. Keep projectile behavior unchanged.

The saved helper checks six world-axis directions with a three-unit limit and first-hit rejection. Misses and ineligible receivers use `continue`. Only MainWall, Floor, LeftWall and RightWall accept marks. Full-song paint density, visual readability and standalone load still need acceptance.

## Next learner increment: direct projectile paint on floor and side walls

In PaintProjectile.Update, extend the existing non-balloon branch's MainWall predicate to a parenthesized OR group for MainWall, Floor, LeftWall and RightWall. Keep the paintMarkPrefab non-null guard outside that group. Keep the existing balloon branch first and retain its event-matched bonus logic; preserve contact point/normal placement, independent mark instantiation and the common projectile destruction/return. Do not add additional raycasts: the projectile already has a SphereCast hit.

This increment is now saved and passes isolated actual-projectile collision checks on all four surfaces, no surface-only score, matching/mismatched balloon 10/20 behavior, once-only marks/projectile cleanup, ineligible-receiver rejection and retry clearing. It uses the projectile's existing assigned teal mark prefab; it does not implement a new projectile pigment-selection system. See [Validation](../Project/VALIDATION.md#direct-projectile-floor-and-side-wall-paint-october-18).

## Next learner increment: full-song paint coverage and readability

Run one complete original song without changing duration or clock speed. Place recognizable early projectile marks on reachable approved surfaces, then play normally with held fire and balloon pops. Pause/resume once. At natural results, confirm early marks remain, later marks do not flicker or float, and accumulated paint has not made balloons/countdowns unreadable. Retry and confirm old marks are gone and current score resets while best score remains. Record placement/clarity failures separately from object counts and frame times. Editor observations are not standalone performance acceptance; capture standalone CPU/GPU frame-time data, repeated retries and soak according to the roadmap before packaging. Do not add timed fading or count-cap removal as an unmeasured workaround.

## Verified asset evidence and limits

Unity 6000.0.77f1 compiled the preparation tool in an ignored isolated copy. The method-driven check loaded PrototypeScene, hit MainWall at approximately (0, 0.99, 9.90) with normal (0, 0, -1), placed a temporary mark 0.005 units outward, and verified parallel alignment and the unchanged subsequent wall ray. It checked the sprite/material/shader, collider/script/shadow exclusions, expected size and asset reload. A close-up render was visually inspected. Temporary objects and preview camera were not saved to the scene.

The diagnostic log is ignored under `.local-backups/surface-paint-20261017`; a redundant diagnostic launch was refused by the already-open isolated project, after which the active verification completed successfully. This is asset/placement verification, not acceptance of a new projectile branch, lifetime, fade, score regression or Windows performance. Production gameplay and scene hashes remain unchanged by preparation. Public v0.1.0-demo.1 does not contain runtime surface painting.
