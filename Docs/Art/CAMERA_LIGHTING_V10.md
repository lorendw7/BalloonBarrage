# Camera and lighting study V10

Open `Assets/Scenes/StudioCameraLighting.unity` in Unity **6000.0.77f1**. This is an independent camera/lighting study of the current saved prototype. `PrototypeScene.unity` remains the recovery baseline and default first build scene. See [README](../../README.md), [game design](../Design/GAME_DESIGN.md), and [roadmap](../Design/ROADMAP.md) for project scope.

## Inspected setup

- Unity 6000.0.77f1, URP 17.0.4, Linear color space. The active PC asset is `Assets/Settings/PC_RPAsset.asset`, using the existing deferred renderer with SSAO, HDR, main/additional soft shadows and legacy Light Probe Groups. APV is not enabled.
- The source uses one perspective MainCamera, 45 degrees, at `(0, 0.1, -8.8)`, pitched down 2 degrees. The held gun is a child of this camera.
- There are five realtime lights: directional sun, window spotlight, frontal point fill and two niche spotlights. Two rectangular Baked lights and one saved directional lightmap pair already supply static bounce. No target Light Probe Group was present.
- Existing plaster, oak, wood, latex and gun surfaces use URP/Lit; particle effects include Unlit shaders. The existing ACES volume, bloom, material tints, textures and shared renderer settings are retained. The full material/light inventory is [SourceAudit.txt](../../Assets/StudioArtPack/CameraLightingV10/SourceAudit.txt).
- `PlayerShooter` aims using `Camera.main.ScreenPointToRay`; the projectile uses a sphere cast. The saved spawner caps targets at three. This study does not modify their code, references, prefab materials, spawn settings or colliders.

## Changes in the copied scene

| Setting | Source | Study |
|---|---|---|
| Camera position | `(0, 0.1, -8.8)` | `(0, -0.4, -7.2)` |
| Downward pitch / vertical FOV | 2 degrees / 45 degrees | 0.6 degrees / 38 degrees |
| Sun | 1.4 intensity, tinted color plus 4500 K | 1.45, white color plus 5000 K |
| Sun travel direction | Existing diagonal | Normalized `(0.55, -0.55, 0.62)`, from upper left toward the room |
| Window spotlight | 135, strongly warm | 105, `#FFF3E0`, existing cookie preserved |
| Front fill | Point, 7, at `(0, 1, -2)` | Broad skylight spot, 18, at `(0, 3.6, 4.4)`, aimed at `(0, -0.8, 8)` |
| Baked window / skylight bounce | 4 / 2.5 | 4.4 / 2.6, skylight `#FFF6E8` |
| Ambient sky / equator / ground | `(0.63,0.69,0.74)` / `(0.50,0.48,0.44)` / `(0.30,0.32,0.33)` | `(0.68,0.70,0.71)` / `(0.58,0.55,0.50)` / `(0.34,0.32,0.28)` |

The camera enlarges the target wall and balloons while cropping peripheral furniture and most of the skylight. The window remains the warm key; the skylight gives a weaker, broad fill without a second competing shadow. Main/window shadow strengths are 0.48/0.45 with URP High soft-shadow filtering and restrained bias. This is filtered realtime shadowing, not physical area-light penumbrae. The held gun retains its local placement; its lower body is more cropped in the tighter view.

Static geometry uses its existing secondary UVs and GI flags. The study has independent CPU-baked lighting settings: 12 texels/unit, 1024 atlas, 32 direct/128 indirect/64 environment samples, three bounces. One lightmap pair is saved under `Assets/Scenes/StudioCameraLighting`. A 5 × 5 × 3 Light Probe Group encloses the target motion region: x ±4.2, y ±3.2, z 6.6–9.3. Moving latex surfaces receive baked bounce through their existing Blend Probes setting. The Custom reflection probe points to a fresh study cubemap captured with the gun hidden. No runtime light is added; the frontal light is reused.

This follows the project's configured probe system rather than switching the shared pipeline to APV. Unity's [URP light reference](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/light-component.html) describes the supported Baked area-light workflow.

## Captures and verification

All PNGs below are 1600 × 900 renders from the existing tagged game camera with the same capture settings. Screen Space Overlay UI is excluded.

| Capture in `Assets/StudioArtPack/CameraLightingV10/Previews` | Evidence |
|---|---|
| `Before_Room.png`, `After_Room.png` | Source and reloaded study room, without targets |
| `Before_Targets.png`, `After_Targets.png` | Same three fixed positions using the unmodified playable prefab; editor comparison, not natural spawning |
| `Before_Runtime.png`, `After_Runtime.png` | Play Mode, three normally spawned targets after invoking the existing BeginRound callback; random seed held constant, capture times are not frame synchronized |

The current playable prefab produces teal targets. These captures preserve that material instead of recoloring targets for the comparison.

Unity compiled the current learner scripts and reloaded the saved study. Camera projection checks at nine positions spanning the full balloon motion range preserve complete target silhouettes with a viewport margin. Camera rays hit the enabled balloon collider; sphere casts from the existing muzzle reach that target, and the muzzle remains ahead of the near plane. Protected scene gameplay, collision and Canvas serialization was checked during creation. Concurrent learner changes to the gun/UI prefab overrides were merged into the study, then the scene was reloaded and checked again.

In both source and study, an isolated Play Mode check verified three normal spawns, a programmatically launched instance of the unchanged projectile hitting/popping a target and awarding 10 points, normal respawn restoring three targets, and the existing retry callback restoring Start, score zero and no targets. The test attempted synthetic mouse input first but did not obtain a successful shot in either scene. This does **not** establish a real mouse-input regression or a successful end-to-end mouse test. Read [PlayCheck.txt](../../Assets/StudioArtPack/CameraLightingV10/PlayCheck.txt) and [Verification.txt](../../Assets/StudioArtPack/CameraLightingV10/Verification.txt) for exact evidence. Physical mouse aiming/firing, HUD layout at 720p/1080p, a full round, mobile rendering and target-device frame rate still need human acceptance.

## Open, tune and recover

1. Open the study scene and use a 16:9 Game view. Enter Play Mode, click Start, shoot targets near each motion boundary, confirm score/respawn, and exercise Pause/Resume/Retry.
2. Tune the existing MainCamera and lights directly in this scene. Keep the MainCamera tag, gun hierarchy and gameplay references intact. The object named `FrontFillLight` now supplies skylight fill; its name is retained for tooling.
3. After changing baked lights, ambient illumination or static geometry, save the study and run **BalloonBarrage → Art → Rebake camera and lighting scene copy**. Rebaking also updates its reflection and camera captures. This command writes only the study's bake assets.
4. Keep the study scene, its `.meta`, its generated lighting folder, and `CameraLightingV10` together. The scene is appended to Build Settings so the learner's build-index-based Retry works; the original remains first. Remove this appended entry when retiring the study.
5. Recover the baseline by reopening `PrototypeScene.unity`. Do not rerun the one-time creation command over an existing study. The editor scripts remain available for another saved project without the study assets.

For an isolated batch project, use `-executeMethod StudioCameraLightingPass.Create` to create/bake/capture, `StudioCameraLightingPass.VerifySaved` to reload/check/capture, or `StudioCameraLightingPlayCheck.RunBatch` for the Play Mode check. The Play Mode command must omit `-quit`; it exits when finished. Use the installed matching Unity version and a working graphics device, not `-nographics`.

## Remaining art gaps

The two requested attachments were not available in this chat; the existing saved studio and the written warm-workshop brief served as references. Exact concept matching remains unverified.

Lighting cannot supply hand-crafted prop detail, replace coarse plant/window silhouettes, fix existing beam/lightmap edge seams, or provide more varied plaster/wood surface detail. Those need mesh/UV/material work in a later authorized asset pass. The bright fixed teal balloons can still visually overlap the teal splat; a future balloon material palette and quieter splat layout would improve separation at every position. This pass deliberately retains those assets. A physically broad moving shadow or more elaborate volumetric sun shafts would require a separate supported rendering/asset plan, rather than assuming HDRP features or a new plugin.

All new lightmaps, cubemaps and captures are Unity renders. This pass adds no external art, downloads or image-generation prompts; existing attribution remains intact.
