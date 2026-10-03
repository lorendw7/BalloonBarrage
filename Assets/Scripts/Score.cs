using UnityEngine;

public class Score : MonoBehaviour
{
    private static int value = 0;

    public static void Add(int amount)
    {
        value += amount;
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(20, 20, 200, 40),
            $"Score: {value}"
        );
    }
}
