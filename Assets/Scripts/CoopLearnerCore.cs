/// <summary>Two small decisions reserved for the creator. Infrastructure owns input and safety.</summary>
public static class CoopLearnerCore
{
    /// <summary>Lesson 1: is now at or beyond the next permitted shot time?</summary>
    public static bool IsSecondShotReady(float now, float nextAllowedShotTime)
    {
        // TODO (learner): return the result of comparing the two times.
        return false;
    }

    /// <summary>Lesson 2: ordinary hit adds zero; on-beat hit adds ten bonus points.</summary>
    public static int SecondPlayerBeatBonus(bool isOnBeat)
    {
        // TODO (learner): choose the bonus using isOnBeat.
        return 0;
    }
}
