# Original paint-spray style candidate

`Assets/StudioArtPack/PaintSprayStyle20261021/Textures/PaintSprayStyle_Atlas_v2.png` is a newly generated transparent 2-by-2 sprite atlas for an energetic paint-shooter presentation. It borrows only broad visual qualities: saturated runtime color, chunky liquid silhouettes, directional motion and immediate hit readability. It does not reproduce another game's assets, characters, UI, typography, weapons, named iconography or territory rules.

The four neutral white/gray cells are prepared for runtime tinting:

| Cell | Sprite name | Intended visual role |
|---|---|---|
| Top left | `NozzleJetTopLeft` | brief accepted-shot nozzle flash |
| Top right | `ViscousStreakTopRight` | optional fast directional accent |
| Bottom left | `DropletClusterBottomLeft` | small airborne secondary droplets |
| Bottom right | `ImpactSplashBottomRight` | short non-persistent hit accent |

The production `PrototypeScene` saves a prefab-instance override that assigns the dedicated transparent `PaintNozzleJet` material to `MuzzleSpray` and selects the atlas's `ViscousStreakTopRight` sprite through Texture Sheet Animation. The looping system emits 160 particles per second with no burst, using a concentrated 0.8-degree cone and 0.006-unit emitter radius, 0.22-to-0.32-second lifetime, 28-to-36-unit speed, 0.14-to-0.22-unit size and a 256-particle cap. A size-over-lifetime envelope starts each streak at 65%, swells quickly to 108%, then contracts to 40%; color-over-lifetime holds full opacity through roughly 76% of particle life before fading to zero. Its stretched-billboard renderer uses a restrained 0.008 speed scale and 1.2 length scale: adjacent droplets overlap into a dense liquid stream without producing long needle-like streaks during fast aim changes.

## Continuous spray presentation

The learner-saved `PlayerShooter` implementation no longer instantiates a flying projectile for accepted shots. Held fire keeps the production six-samples-per-second cadence and performs an immediate 0.3-radius SphereCast from the muzzle over a 30-unit range. This forgiving logical volume is deliberately wider than the enabled 0.13-radius balloon collider. Balloon hits retain the saved rhythm bonus path; non-balloon hits delegate to the existing paint-pattern data on `PaintDrop3D_Teal.prefab` to create static surface marks. At scene startup, the shooter chooses one of the eight saved pigment groups and applies the corresponding material color to the particle system. The stream and deposited marks therefore keep one coordinated color for the whole play session while mark shape, angle and scale continue to vary. The legacy prefab and GUID remain because they store the pigment and pattern references, not because the shooter launches the prefab.

Each deposited surface mark receives a visual-only `PaintMarkImpactPulse`. It begins at 55% of its randomized settled scale, swells to 112%, and settles to 100% over 0.14 seconds. The short-lived component then destroys itself without removing or continually updating the persistent mark.

The stream is visual-only particle motion in World simulation space. At the saved speed and lifetime ranges, particles travel approximately 6.16 to 11.52 world units. Its visual reach is intentionally shorter than the forgiving 30-unit hit volume. Maximum particle residency is approximately 52 particles at the 160-per-second rate, below the saved 256-particle cap.

The earlier mesh-head and dual-TrailRenderer projectile presentation remains stored on the legacy prefab but is not used by the current shooter path. This is saved asset and learner-code configuration, not Play Mode, complete-song or performance acceptance. Confirm visual/logical range alignment, target and beat-cue readability, wall sweeps, rapid aim changes, pause/resume and Retry before accepting the presentation.

The exact prompt, generator mode, source path and SHA-256 are stored beside the image in `Provenance.json`. Static alpha/import checks are recorded in `Verification.txt`; they are not a Unity import test or playtest.
