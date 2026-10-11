using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RoundController : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float roundDuration = 90f;

    private float remainingTime;

    private int finalScore;

    private bool hasStarted;

    public static bool IsPlaying { get; private set; }

    public static bool IsPaused { get; private set; }

    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Score.ResetScore();
        remainingTime = roundDuration;
        IsPlaying = false;
        hasStarted = false;
    }

    private void BeginRound()
    {
        Score.ResetScore();
        remainingTime = roundDuration;
        hasStarted = true;
        IsPlaying = true;
    }

    private void TogglePause()
    {
        IsPaused = !IsPaused;

        Time.timeScale = IsPaused ? 0f : 1f;
        AudioListener.pause = IsPaused;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPlaying)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        if (IsPaused)
        {
            return;

        }

        remainingTime = Mathf.Max(
            0f, remainingTime - Time.deltaTime);

        if (remainingTime > 0f )
        {
            return;
        }

        EndRound();

    }

    private void OnDestroy()
    {
        IsPlaying = false;
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void EndRound()
    {
        IsPlaying = false;
        finalScore = Score.Value;

        foreach (PaintProjectile projectile in 
            FindObjectsByType<PaintProjectile>(FindObjectsSortMode.None))
        {
            projectile.gameObject.SetActive(false);
            Destroy(projectile.gameObject);
        }

        foreach (Balloon balloon in
    FindObjectsByType<Balloon>(FindObjectsSortMode.None))
        {
            balloon.gameObject.SetActive(false);
            Destroy(balloon.gameObject);
        }

        Debug.Log($"本局结束，最终分数：{finalScore}");
    }

    private void RestartRound()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(20, 55, 200, 40),
            $"Time: {Mathf.CeilToInt(remainingTime)}");

        if (IsPaused)
        {
            DrawPauseMenu();
            return;
        }

        if (!hasStarted)
        {
            DrawStartMenu();
            return;
        }

        if (IsPlaying)
        {
            return;
        }

        float left = (Screen.width - 260f) / 2f;
        float top = (Screen.height - 160f) / 2f;

        GUI.Box(
            new Rect(left, top, 260, 160),
            "Round Finished");

        GUI.Label(
            new Rect(left + 20, top + 45, 220, 30),
            $"Final Score: {finalScore}");

        if (GUI.Button(
            new Rect(left + 20,  top + 95, 220, 40),
            $"Play Again"))
        {
            RestartRound();
        }
    }

    private void DrawPauseMenu()
    {
        float left = (Screen.width - 260f) / 2f;
        float top = (Screen.height - 220f) / 2f;

        GUI.Box(
            new Rect(left, top, 260, 220f),
            "Paused");

        GUI.Label(
            new Rect(left + 20, top + 45, 220, 30),
            "Press Esc or click Resume");

        if (GUI.Button(
            new Rect(left + 20, top + 95, 220, 40),
            "Resume"))
        {
            TogglePause();
        }

        if (GUI.Button(
            new Rect(left + 20, top + 155, 220, 40),
            "Quit"))
        {
            QuitGame();
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void DrawStartMenu()
    {
        float left = (Screen.width - 360f) / 2f;
        float top = (Screen.height - 260f) / 2f;

        GUI.Box(
               new Rect(left, top, 360, 260),
               "BalloonBarrage");

        GUI.Label(
            new Rect(left + 20, top + 40, 320, 100),
            "Aim with the mouse.\n" +
            "Hold left click to fire.\n" +
            "Esc pauses or resumes.\n" +
            "Pop balloons before time runs out.");

        if (GUI.Button(
            new Rect(left + 20, top + 155, 320, 40),
            "Start"))
        {
            BeginRound();
        }

        if (GUI.Button(
            new Rect(left + 20, top + 210, 320, 30),
            "Quit"))
        {
            QuitGame();
        }
    }
}
