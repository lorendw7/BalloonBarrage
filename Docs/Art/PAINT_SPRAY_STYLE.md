# Original paint-spray style candidate

`Assets/StudioArtPack/PaintSprayStyle20261021/Textures/PaintSprayStyle_Atlas_v2.png` is a newly generated transparent 2-by-2 sprite atlas for an energetic paint-shooter presentation. It borrows only broad visual qualities: saturated runtime color, chunky liquid silhouettes, directional motion and immediate hit readability. It does not reproduce another game's assets, characters, UI, typography, weapons, named iconography or territory rules.

The four neutral white/gray cells are prepared for runtime tinting:

| Cell | Sprite name | Intended visual role |
|---|---|---|
| Top left | `NozzleJetTopLeft` | brief accepted-shot nozzle flash |
| Top right | `ViscousStreakTopRight` | optional fast directional accent |
| Bottom left | `DropletClusterBottomLeft` | small airborne secondary droplets |
| Bottom right | `ImpactSplashBottomRight` | short non-persistent hit accent |

This is candidate art, not saved gameplay. Nothing in the atlas is connected to `MuzzleSpray`, projectiles, scoring, collision or surface marking. Keep the current cadence comparison isolated. After the learner accepts one firing rate, compare only one cosmetic change, beginning with the existing particle burst count if desired. A later asset-integration lesson can test the top-left sprite without changing projectile count or the game's paint-mark rules.

The exact prompt, generator mode, source path and SHA-256 are stored beside the image in `Provenance.json`. Static alpha/import checks are recorded in `Verification.txt`; they are not a Unity import test or playtest.
