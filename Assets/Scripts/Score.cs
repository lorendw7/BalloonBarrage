using UnityEngine;

public class Score : MonoBehaviour
{
    private static int value = 0;

    public static int Value => value;

    private static int successfulHits = 0;

    public static int SuccessfulHits => successfulHits;

    private static int onBeatHits = 0;

    public static int OnBeatHits => onBeatHits;

    private static int currentStreak = 0;

    public static int CurrentStreak => currentStreak;

    private static int maximumStreak = 0;

    public static int MaximumStreak => maximumStreak;


    public static void ResetScore()
    {
        value = 0;

        successfulHits = 0;
        onBeatHits = 0;
        currentStreak = 0;
        maximumStreak = 0;
    }

    public static void RegisterSuccessfulHit(bool isOnBeat)
    {

        if (!RoundController.IsPlaying || RoundController.IsPaused)
        {
            return;
        }

        successfulHits++;

        if (isOnBeat)
        {
            onBeatHits++;
        }

        currentStreak++;
        if (currentStreak > maximumStreak)
        {
            maximumStreak = currentStreak;
        }
    }

    public static void RegisterExpiredTarget()
    {
        if (!RoundController.IsPlaying || RoundController.IsPaused)
        {
            return;
        }

        currentStreak = 0;
    }

    public static void Add(int amount)
    {
        if (!RoundController.IsPlaying ||
    RoundController.IsPaused)
        {
            return;
        }
        value += amount;
    }
}
