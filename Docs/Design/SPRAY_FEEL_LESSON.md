# Spray feel and cadence lesson

Updated: October 21, 2026. This lesson learns from the responsiveness of paint-shooter games without copying Splatoon assets, characters, weapon silhouettes, typography, sound, turf scoring or movement rules. BalloonBarrage remains a fixed-camera, complete-song balloon game with optional rhythm rewards.

## What creates a convincing spray

The current shot already coordinates five signals: a finite projectile, six short muzzle particles, the original paint-shot sound, bounded cosmetic recoil and a persistent impact mark. A lively spray feeling comes from those signals agreeing on direction and timing—not from spawning a projectile every frame or filling the screen with particles.

The saved PrototypeScene override now uses `shotsPerSecond = 6`, so accepted held fire waits about 0.167 seconds between shots. The script's new-component default remains 5; the scene override is the value used by this prototype. The cadence is a technical feedback and performance control, not ammunition, reload, heat or forced downtime. The existing `Muzzle/MuzzleSpray` emits six particles per accepted shot with 0.10–0.20 second lifetime, 1.5–2.5 start speed and 0.025–0.05 start size.

## Accepted cadence checkpoint

On October 21 the creator reported that the firing rate was decided, and the saved PrototypeScene confirms the selected value is 6 shots per second. This is user-reported tuning acceptance, not a new automated playtest. Do not edit `PlayerShooter.cs` merely to duplicate the scene override.

## One small step: compare 6 with 8 muzzle particles

Keep the accepted cadence fixed at 6 shots per second. Open the `PaintGun_M` prefab, select `Muzzle/MuzzleSpray`, expand **Particle System > Emission > Bursts**, and change only the first burst's **Count** from 6 to 8. Do not change Rate over Time, lifetime, speed, size, sound, recoil or projectile cadence. This changes only the cosmetic nozzle spray; it must not create extra projectiles, score, collisions or surface marks. At continuous fire, the comparison is approximately 36 versus 48 short-lived muzzle particles per second.

If six particles are already readable at the accepted cadence, keep them and stop. A denser effect is not automatically a better one, especially during two minutes of continuous fire. After the particle comparison, verify ten seconds of held fire, one ordinary balloon hit, one rewarded hit, a wall sweep and pause/resume before continuing to the beat-approach-bar code lesson.

## Reply template

```text
Preferred muzzle burst: 6 / 8 particles
Individual shots: clear / blending together
Targets and beat cue: clear / partly obscured / obscured
Ten-second held fire: smooth / hitch observed
Ordinary hit, rewarded hit, wall mark, pause/resume: pass / describe
Reason in one sentence:
```
