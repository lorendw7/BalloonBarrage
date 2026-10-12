using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoundController : MonoBehaviour
{
    [SerializeField, Min(1f)]
    private float roundDuration = 90f;

    private float remainingTime;

    private int finalScore;

    private bool hasStarted;

    public static bool IsPlaying { get; private set; }

    public static bool IsPaused { get; private set; }

    [SerializeField]
    private GameObject startScreen;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject gameplayHUD;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text timeText;


    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Score.ResetScore();
        remainingTime = roundDuration;
        IsPlaying = false;
        hasStarted = false;

        if (startScreen != null)
        {
            startScreen.SetActive(true);
        }
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (gameplayHUD != null)
        {
            gameplayHUD.SetActive(IsPlaying);
        }

        if (scoreText != null)
        {
            scoreText.text = Score.Value.ToString("D6");
        }

        if (timeText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);

            timeText.text = $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }
    public void BeginRound()
    {
        if (hasStarted)
        {
            return;
        }

        Score.ResetScore();
        remainingTime = roundDuration;
        hasStarted = true;
        IsPlaying = true;

        if (startScreen != null)
        {
            startScreen.SetActive(false);
        }
    }

    private void TogglePause()
    {
        if (!IsPlaying)
        {
            return;
        }

        IsPaused = !IsPaused;

        Time.timeScale = IsPaused ? 0f : 1f;
        AudioListener.pause = IsPaused;

        if (pauseScreen != null)
        {
            pauseScreen.SetActive(IsPaused);
        }
    }

    public void ResumeRound()
    {
        if (IsPlaying && IsPaused)
        {
            TogglePause();
        }
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
        if (IsPaused)
        {
            return;
        }

        if (!hasStarted)
        {
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

    public void QuitGame()
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
