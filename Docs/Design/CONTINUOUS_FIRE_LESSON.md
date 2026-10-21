# Continuous-fire and paint-loop lesson

This is the first learner checkpoint for the October 21 design reset. It tests the existing implementation before adding or rewriting gameplay code. The authoritative behavior is in [Game design](GAME_DESIGN.md); delivery order is in the [Roadmap](ROADMAP.md).

## Design goal

Holding the left mouse button should let a player fire for the whole round without ammunition, reload, heat, stamina or a beat gate. The room should visibly accumulate color, while balloons, reticle, music and the timing cue remain readable.

Unlimited duration does not mean unlimited projectiles per frame. A finite cadence is required for understandable recoil and sound, reliable collision work and measurable persistent-paint cost. `PlayerShooter` already uses a serialized `shotsPerSecond` value, defaulting to 5, while `Mouse.current.leftButton.isPressed` remains true.

## One small step: observe the saved baseline

Do not edit code yet.

1. Open `Assets/Scenes/PrototypeScene.unity` in Unity 6000.0.77f1.
2. Enter Play Mode, start the round and hold the left mouse button for 30 seconds. Sweep between empty walls and balloons; do not try to tap on the beat.
3. Confirm there is no ammunition, reload, overheat or forced stop. Pause once while holding the button, resume, release it, then hold again. Confirm the menu click does not immediately fire.
4. Record only these observations: whether firing ever stopped, whether the repeated sound masked the music, whether balloons and the NEXT cue stayed readable, and whether the Editor showed a visible hitch as paint accumulated.

This is a creator playtest, not automated acceptance or a clean-player performance measurement.

## Reply template

```text
Continuous fire for 30 seconds: PASS / FAIL
Stopped or forced downtime: none / describe
Music under held fire: clear / partly masked / masked
Targets and NEXT cue: clear / partly obscured / obscured
Pause-resume-release-fire: PASS / FAIL
Visible hitch or error: none / describe
Preferred feel at the current cadence: too slow / comfortable / too busy
```

## Next lesson, after the report

If the baseline works, change only the `shotsPerSecond` value and compare one alternative against the saved value. Do not remove the cadence check and do not add ammo or an overheat system. The learner makes that gameplay change; the assistant reviews the saved diff and the same observation set before another parameter changes.

If the baseline fails, diagnose that single failure first. Do not combine a firing-rate change with beat-cue, scoring, paint-rendering or result-screen work.
