# Local co-op and gamepad lesson

[README](../../README.md) is the entry point; [Game design](GAME_DESIGN.md#local-players-and-gamepads--development-practice) owns behavior and [Roadmap](ROADMAP.md) owns delivery. This is an explicitly requested learner exercise. Existing `KyudaisaiLearnerCore.cs` solutions remain untouched.

## What is wired

The selection screen offers mouse solo, gamepad solo, mouse + gamepad practice and two-gamepad practice. One shared song clock, target set, paint canvas and score serve both local players; each has a numbered reticle. There is no online play or split screen. Solo gamepad uses either stick to aim, held RT to spray, X/West to change pigment and Start to pause. D-pad/stick, South and East navigate menus through Unity's existing Input System UI actions. Mouse/keyboard controls remain available in their selected layout.

Controller assignments are frozen at Play. An extra controller cannot steal a player. Losing an assigned controller pauses the round, cancels spraying and requires the original controller to return before Resume. Trigger release is required after pause/focus loss/reconnection. Practice co-op uses a separate `.LocalCoop.Dev.v1` score suffix. Retry restores normal mouse solo.

## Deliberately unfinished core

`Assets/Scripts/CoopLearnerCore.cs` belongs to the learner. Both method bodies are intentionally small TODOs. `IsSecondShotReady` currently returns false, so P2 can aim but cannot produce an accepted shot. `SecondPlayerBeatBonus` currently returns zero, so its timed-hit reward remains unfinished. P1 and gamepad-solo behavior use the existing working path. This is a development practice mode, not accepted complete co-op gameplay.

Do not fill both methods at once. The assistant will review each small step and verify it before the next lesson.

## Step 1: decide when P2 may shoot

Edit **only the body of `IsSecondShotReady(float now, float nextAllowedShotTime)`**.

`now` is the current gameplay time. `nextAllowedShotTime` is the earliest time permitted by the previous shot. Return a Boolean that answers whether the first time has reached the second. Equality must count as ready. Use a comparison and return its result; no loop, new timer, target lookup, score change or input handling is needed. The surrounding shooter already schedules the next shot and protects pause, focus, held-trigger release and per-player input.

Check these cases:

| now | nextAllowedShotTime | Expected |
|---:|---:|---|
| 0 | 1 | false |
| 1 | 1 | true |
| 1.2 | 1 | true |

In Unity, choose **Balloon Barrage → Lessons → Check P2 shot gate**. A pending message means one of the three cases is still wrong. Send the edited function for review before touching `SecondPlayerBeatBonus`.

## Verification boundary

Virtual-device Editor checks can verify assignment, cursor independence, P1 trigger/cadence, device-loss safety, layout/record isolation and reset. They do not certify a physical controller, standalone package, real Mac, human comfort or the unfinished P2 core. [Validation](../Project/VALIDATION.md) records exact completed checks and limits.
