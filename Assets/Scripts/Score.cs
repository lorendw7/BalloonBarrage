using UnityEngine;

public class Score : MonoBehaviour
{
    private static int value = 0;

    public static int Value => value;

    public static void ResetScore()
    {
        value = 0;
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
