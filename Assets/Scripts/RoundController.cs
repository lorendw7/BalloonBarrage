using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoundController : MonoBehaviour
{
    private float songDuration;
    private double songStartDspTime;

    private float remainingTime;

    private int finalScore;

    private const string BestScoreKey = "BestScore.GoinUnder.Beginner.Solo.v1";

    private int bestScore;

    private int displayedScore = int.MinValue;
    private int displayedSeconds = int.MinValue;


    private bool hasStarted;

    public static bool IsPlaying { get; private set; }

    public static bool IsPaused { get; private set; }

    public double SongTimeSeconds
    {
        get
        {
            if (!IsPlaying)
            {
                return 0d;
            }
            return System.Math.Max(0d, AudioSettings.dspTime - songStartDspTime);
        }
    }
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

    [SerializeField]
    private GameObject resultsScreen;

    [SerializeField]
    private Text finalScoreText;

    [SerializeField]
    private Text bestScoreText;

    [SerializeField]
    private AudioSource menuMusic;

    [SerializeField]
    private AudioSource gameMusic;

    [SerializeField]
    private Text resultStatsText;

    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Score.ResetScore();

        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        Debug.Log($"Best score loaded: {bestScore}");

        songDuration = gameMusic != null && gameMusic.clip != null 
            ? gameMusic.clip.length : 0f;
        remainingTime = songDuration;

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

        if (resultsScreen != null)
        {
            resultsScreen.SetActive(false);
        }

        if (gameMusic != null)
        {
            gameMusic.Stop();
        }

        if (menuMusic != null)
        {
            menuMusic.Stop();
            menuMusic.Play();
        }
    }

    private void LateUpdate()
    {
        if (gameplayHUD != null && gameplayHUD.activeSelf != IsPlaying)
        {
            gameplayHUD.SetActive(IsPlaying);
        }

        int currentScore = Score.Value;
        if (scoreText != null && displayedScore != currentScore)
        {
            displayedScore = currentScore;
            scoreText.text = currentScore.ToString("D6");
        }

        if (timeText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);

            if (displayedSeconds != seconds)
            {
                displayedSeconds = seconds;
                timeText.text = $"{seconds / 60:00}:{seconds % 60:00}";
            }
        }
    }
    public void BeginRound()
    {
        if (hasStarted)
        {
            return;
        }

        if (gameMusic == null || gameMusic.clip == null ||
            gameMusic.clip.loadState != AudioDataLoadState.Loaded)
        {
            Debug.LogWarning("Game music is missing or not loaded yet.");
            return;
        }

        Score.ResetScore();

        songDuration = gameMusic.clip.length;
        remainingTime = songDuration;

        hasStarted = true;
        IsPlaying = true;

        if (startScreen != null)
        {
            startScreen.SetActive(false);
        }
        if (menuMusic != null)
        {
            menuMusic.Stop();
        }

        gameMusic.Stop();

        songStartDspTime = AudioSettings.dspTime + 0.2d;
        gameMusic.PlayScheduled(songStartDspTime);
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

        float elapsed = Mathf.Max(
            0f, (float)(AudioSettings.dspTime - songStartDspTime));

        remainingTime = Mathf.Max(
            0f, songDuration - elapsed);

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

        if(finalScore > bestScore)
        {
            bestScore = finalScore;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.Save();
        }

        if (gameMusic != null)
        {
            gameMusic.Stop();
        }

        if (menuMusic != null)
        {
            menuMusic.Play();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = finalScore.ToString("D6");
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = bestScore.ToString("D6");
        }

        if (resultStatsText != null)
        {
            resultStatsText.text =
                $"Hits: {Score.SuccessfulHits}\n" +
                $"On-beat Hits: {Score.OnBeatHits}\n" +
                $"Max Streak: {Score.MaximumStreak}";
        }

        if (resultsScreen != null)
        {
            resultsScreen.SetActive(true);
        }

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

    public void RestartRound()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex);
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
