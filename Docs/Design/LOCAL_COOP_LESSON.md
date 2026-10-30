# Local co-op and gamepad lesson

[README](../../README.md) is the entry point; [Game design](GAME_DESIGN.md#local-players-and-gamepads) owns behavior and [Roadmap](ROADMAP.md) owns delivery. This is an explicitly requested learner exercise. Existing `KyudaisaiLearnerCore.cs` solutions remain untouched.

## Completed learner code

The creator saved both functions and requested review, tests and packaging. The authored method bodies are preserved.

```csharp
public static bool IsSecondShotReady(float now, float nextAllowedShotTime)
{
    return now >= nextAllowedShotTime;
}

public static int SecondPlayerBeatBonus(bool isOnBeat)
{
    if (isOnBeat) return 10;
    return 0;
}
```

The comparison returns false before the deadline, true at equality and true afterward. It does not create a second clock: the surrounding shooter schedules the next allowed time and handles input, pause and trigger release. The bonus returns only the additional ten points. `Balloon` already supplies the base ten, so a timed hit totals twenty and an ordinary hit totals ten. Returning twenty here would incorrectly total thirty.

The actual shooter samples both functions. Controlled virtual-device tests exercise early/equal/late gate values, ordinary/timed P2 hits, cadence and single target resolution across players. Direct state injection in an Editor callback does not certify native device events, a physical controller or real-Mac operation. [Validation](../Project/VALIDATION.md) owns executed evidence; [controller setup](CONTROLLER_SETUP.md) owns the hardware check sequence.

Future gameplay lessons should introduce one small learner-owned change at a time. This completed exercise does not authorize the assistant to fill future core functions automatically.
