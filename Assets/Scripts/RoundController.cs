using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public sealed class ExhibitionSongProfile
{
    public string trackId;
    public string title;
    public string artist;
    public AudioClip audioClip;
    public string chartRelativePath;
    public double sourceCueInSeconds;
    public double sourceCueOutSeconds;
    public double roundDurationSeconds;
    public int sourceEventStartId;
    public int sourceEventEndIdExclusive;
}

public class RoundController : MonoBehaviour
{
    private float songDuration;
    private double songStartDspTime;

    private double pausedAtDspTime;

    private double totalPausedDspSeconds;

    private float remainingTime;

    private int finalScore;

    private int bestScore;

    private int displayedScore = int.MinValue;
    private int displayedSeconds = int.MinValue;


    private bool hasStarted;

    private int selectedSongIndex;
    private GameObject songSelectionPanel;
    private Dropdown songDropdown;
    private Text songSelectionTitle;
    private Text songSelectionDetails;
    private Button songConfirmButton;
    private Button songBackButton;
    private bool refreshSelectionText;
    public bool IsChoosingSong => songSelectionPanel != null && songSelectionPanel.activeSelf;

    public event System.Action SongSelectionChanged;

    public static bool IsPlaying { get; private set; }

    public static bool IsPaused { get; private set; }

    public ExhibitionSongProfile SelectedSong
    {
        get
        {
            if (exhibitionSongs == null || exhibitionSongs.Length == 0)
            {
                return null;
            }

            return exhibitionSongs[Mathf.Clamp(
                selectedSongIndex,
                0,
                exhibitionSongs.Length - 1)];
        }
    }

    public double SongTimeSeconds
    {
        get
        {
            if (!IsPlaying)
            {
                return 0d;
            }
            double effectiveDspTime = IsPaused
                ? pausedAtDspTime
                : AudioSettings.dspTime;

            return System.Math.Max(
                0d,
                effectiveDspTime - songStartDspTime -
                totalPausedDspSeconds);
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
    private ExhibitionSongProfile[] exhibitionSongs;

    [SerializeField]
    private Text resultStatsText;

    private void OnEnable()
    {
        LanguageSettings.LanguageChanged += OnLanguageChanged;
    }

    private void OnDisable()
    {
        LanguageSettings.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(UILanguage language)
    {
        refreshSelectionText = true;
        if (resultsScreen != null && resultsScreen.activeSelf)
        {
            RefreshResultStats();
        }
    }

    private void Start()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Score.ResetScore();

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

        EnsureFallbackSongProfile();
        PrepareStartScreenLayout();
        SelectSong(0);
    }

    private void EnsureFallbackSongProfile()
    {
        if (exhibitionSongs != null && exhibitionSongs.Length > 0)
        {
            return;
        }

        exhibitionSongs = new[]
        {
            new ExhibitionSongProfile
            {
                trackId = "stepmania-goin-under-beginner",
                title = "Goin' Under",
                artist = "NegaRen",
                audioClip = gameMusic != null ? gameMusic.clip : null,
                chartRelativePath = "Rhythm/Prepared/GoinUnder_Beginner.json",
                sourceCueInSeconds = 9.142857143d,
                sourceCueOutSeconds = 91.428571429d,
                roundDurationSeconds = 82.285714286d,
                sourceEventStartId = 0,
                sourceEventEndIdExclusive = 68
            }
        };
    }

    private void PrepareStartScreenLayout()
    {
        if (startScreen == null)
        {
            return;
        }

        Transform controlsPanel = FindDescendant(
            startScreen.transform,
            "ControlsPanel");

        if (controlsPanel != null)
        {
            RectTransform panelRect =
                controlsPanel.GetComponent<RectTransform>();

            if (panelRect != null)
            {
                panelRect.anchoredPosition = new Vector2(0f, -40f);
                panelRect.sizeDelta = new Vector2(600f, 200f);
            }
        }

        Transform controlsText = FindDescendant(
            startScreen.transform,
            "ControlsText");

        if (controlsText != null)
        {
            RectTransform textRect =
                controlsText.GetComponent<RectTransform>();
            Text text = controlsText.GetComponent<Text>();

            if (textRect != null)
            {
                textRect.anchoredPosition = new Vector2(0f, -25f);
                textRect.sizeDelta = new Vector2(560f, 125f);
            }

            if (text != null)
            {
                text.fontSize = 19;
                text.lineSpacing = 0.9f;
            }
        }
    }

    private static Transform FindDescendant(
        Transform root,
        string objectName)
    {
        foreach (Transform candidate in
            root.GetComponentsInChildren<Transform>(true))
        {
            if (candidate.name == objectName)
            {
                return candidate;
            }
        }

        return null;
    }

    public void SelectSong(int index)
    {
        if (hasStarted || exhibitionSongs == null ||
            exhibitionSongs.Length == 0)
        {
            return;
        }

        selectedSongIndex = Mathf.Clamp(
            index,
            0,
            exhibitionSongs.Length - 1);

        ExhibitionSongProfile profile = SelectedSong;

        if (gameMusic != null)
        {
            gameMusic.Stop();
            gameMusic.clip = profile.audioClip;
        }

        songDuration = Mathf.Max(
            0f,
            (float)profile.roundDurationSeconds);
        remainingTime = songDuration;
        displayedSeconds = int.MinValue;

        string bestScoreKey = GetBestScoreKey(profile.trackId);
        bestScore = PlayerPrefs.GetInt(bestScoreKey, 0);

        SongSelectionChanged?.Invoke();
        if (IsChoosingSong)
        {
            PreviewSelectedSong();
            RefreshSongSelectionText();
        }

        Debug.Log(
            $"Selected {profile.title} ({songDuration:F1}s short round).");
    }

    private static string GetBestScoreKey(string trackId)
    {
        return $"BestScore.{trackId}.Kyudaisai.Short.v1";
    }

    private void LateUpdate()
    {
        if (refreshSelectionText)
        {
            refreshSelectionText = false;
            RefreshSongSelectionText();
        }
        if (IsChoosingSong && gameMusic != null && gameMusic.isPlaying &&
            gameMusic.time >= Mathf.Min((float)SelectedSong.sourceCueOutSeconds,
                (float)SelectedSong.sourceCueInSeconds + 12f))
        {
            gameMusic.Stop();
        }
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
        if (hasStarted || startScreen == null) return;
        if (songSelectionPanel == null) BuildSongSelectionPanel();
        SetStartActionsVisible(false);
        songSelectionPanel.SetActive(true);
        songDropdown.SetValueWithoutNotify(selectedSongIndex);
        RefreshSongSelectionText();
        PreviewSelectedSong();
    }

    private void SetStartActionsVisible(bool visible)
    {
        foreach (string objectName in new[] { "StartButton", "QuitButton", "ControlsPanel", "Subtitle" })
        {
            Transform item = FindDescendant(startScreen.transform, objectName);
            if (item != null) item.gameObject.SetActive(visible);
        }
    }

    public void CancelSongSelection()
    {
        if (songDropdown != null) songDropdown.Hide();
        if (songSelectionPanel != null) songSelectionPanel.SetActive(false);
        if (gameMusic != null) gameMusic.Stop();
        SetStartActionsVisible(true);
        if (menuMusic != null) menuMusic.Play();
    }

    private void BuildSongSelectionPanel()
    {
        Transform sourcePanel = FindDescendant(startScreen.transform, "ControlsPanel");
        songSelectionPanel = new GameObject("SongSelectionPanel", typeof(RectTransform), typeof(Image));
        songSelectionPanel.transform.SetParent(sourcePanel.parent, false);
        RectTransform panel = songSelectionPanel.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(0f, -55f);
        panel.sizeDelta = new Vector2(680f, 320f);
        Image background = songSelectionPanel.GetComponent<Image>();
        background.sprite = sourcePanel.GetComponent<Image>().sprite;
        background.type = Image.Type.Sliced;
        background.pixelsPerUnitMultiplier = 1.4f;

        Text textTemplate = FindDescendant(startScreen.transform, "ControlsText").GetComponent<Text>();
        songSelectionTitle = CreateSelectionLabel(textTemplate, "SongSelectionTitle", 105f, 38f, 25);
        songSelectionDetails = CreateSelectionLabel(textTemplate, "SongSelectionDetails", -30f, 60f, 18);

        GameObject dropdownObject = DefaultControls.CreateDropdown(new DefaultControls.Resources());
        dropdownObject.name = "SongList";
        dropdownObject.transform.SetParent(panel, false);
        RectTransform dropdownRect = dropdownObject.GetComponent<RectTransform>();
        dropdownRect.anchoredPosition = new Vector2(0f, 40f);
        dropdownRect.sizeDelta = new Vector2(560f, 54f);
        songDropdown = dropdownObject.GetComponent<Dropdown>();
        Color ink = new Color(0.12f, 0.24f, 0.21f);
        Color paper = new Color(1f, 0.96f, 0.85f);
        songDropdown.image.color = new Color(0.12f, 0.29f, 0.25f);
        Outline border = dropdownObject.AddComponent<Outline>();
        border.effectColor = new Color(0.2f, 0.38f, 0.32f, 0.7f);
        border.effectDistance = new Vector2(2f, -2f);
        ColorBlock colors = songDropdown.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.91f, 0.98f, 0.92f);
        colors.selectedColor = colors.highlightedColor;
        colors.pressedColor = new Color(0.77f, 0.9f, 0.81f);
        songDropdown.colors = colors;
        songDropdown.template.GetComponent<Image>().color = new Color(0.8f, 0.87f, 0.75f);
        Outline listBorder = songDropdown.template.gameObject.AddComponent<Outline>();
        listBorder.effectColor = ink;
        listBorder.effectDistance = new Vector2(1.5f, -1.5f);
        Toggle itemToggle = songDropdown.itemText.GetComponentInParent<Toggle>(true);
        itemToggle.colors = colors;
        if (itemToggle.targetGraphic != null) itemToggle.targetGraphic.color = new Color(0.8f, 0.87f, 0.75f);
        if (itemToggle.graphic != null) itemToggle.graphic.color = new Color(0.15f, 0.5f, 0.39f);
        foreach (Scrollbar scrollbar in songDropdown.template.GetComponentsInChildren<Scrollbar>(true))
        {
            scrollbar.GetComponent<Image>().color = new Color(0.67f, 0.77f, 0.62f);
            if (scrollbar.targetGraphic != null) scrollbar.targetGraphic.color = ink;
        }
        songDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();
        foreach (ExhibitionSongProfile song in exhibitionSongs)
            options.Add($"{song.title}  ·  {song.roundDurationSeconds:0} sec");
        songDropdown.AddOptions(options);
        songDropdown.onValueChanged.AddListener(SelectSong);
        // A long list scrolls inside a bounded panel instead of covering the actions.
        songDropdown.template.sizeDelta = new Vector2(0f, 180f);
        RectTransform itemRect = songDropdown.itemText.transform.parent.GetComponent<RectTransform>();
        itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, 36f);
        Transform arrow = dropdownObject.transform.Find("Arrow");
        if (arrow != null)
        {
            arrow.GetComponent<Image>().enabled = false;
            for (int index = 0; index < 2; index++)
            {
                GameObject stroke = new GameObject("Chevron", typeof(RectTransform), typeof(Image));
                stroke.transform.SetParent(arrow, false);
                RectTransform strokeRect = stroke.GetComponent<RectTransform>();
                strokeRect.sizeDelta = new Vector2(10f, 2.5f);
                strokeRect.anchoredPosition = new Vector2(index == 0 ? -3f : 3f, 0f);
                strokeRect.localRotation = Quaternion.Euler(0f, 0f, index == 0 ? -45f : 45f);
                stroke.GetComponent<Image>().color = paper;
                stroke.GetComponent<Image>().raycastTarget = false;
            }
        }
        foreach (Text label in dropdownObject.GetComponentsInChildren<Text>(true))
        {
            label.font = textTemplate.font;
            label.fontSize = 20;
            label.color = new Color(0.12f, 0.19f, 0.17f);
        }
        songDropdown.captionText.color = new Color(1f, 0.93f, 0.73f);
        songDropdown.captionText.resizeTextForBestFit = true;
        songDropdown.captionText.resizeTextMinSize = 15;
        songDropdown.captionText.resizeTextMaxSize = 20;
        songConfirmButton = CreateSelectionButton("ConfirmSongButton", "StartButton", -170f, StartSelectedSong);
        songBackButton = CreateSelectionButton("BackFromSongButton", "QuitButton", 170f, CancelSongSelection);
    }

    private Text CreateSelectionLabel(Text template, string objectName, float y, float height, int size)
    {
        Text label = Instantiate(template, songSelectionPanel.transform);
        label.name = objectName;
        foreach (LocalizedText localized in label.GetComponents<LocalizedText>()) localized.enabled = false;
        label.rectTransform.anchoredPosition = new Vector2(0f, y);
        label.rectTransform.sizeDelta = new Vector2(580f, height);
        label.fontSize = size;
        label.raycastTarget = false;
        return label;
    }

    private Button CreateSelectionButton(string objectName, string templateName, float x,
        UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = Instantiate(FindDescendant(startScreen.transform, templateName).gameObject,
            songSelectionPanel.transform);
        buttonObject.name = objectName;
        buttonObject.SetActive(true);
        foreach (LocalizedText localized in buttonObject.GetComponentsInChildren<LocalizedText>(true))
            localized.enabled = false;
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(x, -105f);
        rect.sizeDelta = new Vector2(280f, 70f);
        Button button = buttonObject.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(action);
        return button;
    }

    private void RefreshSongSelectionText()
    {
        if (songSelectionPanel == null) return;
        Transform sourceText = FindDescendant(startScreen.transform, "ControlsText");
        LocalizedText localizedSource = sourceText.GetComponent<LocalizedText>();
        Font font = localizedSource != null ? localizedSource.CurrentFont : sourceText.GetComponent<Text>().font;
        foreach (Text label in songSelectionPanel.GetComponentsInChildren<Text>(true)) label.font = font;
        songSelectionTitle.text = LocalizationManager.GetText("song.choose");
        songSelectionDetails.text = $"{SelectedSong.artist}  ·  {SelectedSong.roundDurationSeconds:0} sec\n" +
            LocalizationManager.GetText("song.preview");
        songConfirmButton.GetComponentInChildren<Text>(true).text = LocalizationManager.GetText("song.play");
        songBackButton.GetComponentInChildren<Text>(true).text = LocalizationManager.GetText("song.back");
    }

    private void PreviewSelectedSong()
    {
        if (menuMusic != null) menuMusic.Stop();
        if (gameMusic == null || SelectedSong.audioClip == null) return;
        gameMusic.Stop();
        gameMusic.clip = SelectedSong.audioClip;
        gameMusic.time = (float)SelectedSong.sourceCueInSeconds;
        gameMusic.Play();
    }

    public void StartSelectedSong()
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

        ExhibitionSongProfile profile = SelectedSong;

        if (profile == null)
        {
            Debug.LogWarning("No exhibition song is selected.");
            return;
        }
        RhythmChartReader reader = FindFirstObjectByType<RhythmChartReader>();
        if (reader == null || !reader.IsReady)
        {
            Debug.LogWarning("Selected chart is not ready.");
            return;
        }
        if (songDropdown != null) songDropdown.Hide();
        if (songSelectionPanel != null) songSelectionPanel.SetActive(false);

        songDuration = (float)profile.roundDurationSeconds;
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

        gameMusic.time = Mathf.Clamp(
            (float)profile.sourceCueInSeconds,
            0f,
            Mathf.Max(0f, gameMusic.clip.length - 0.01f));

        songStartDspTime = AudioSettings.dspTime + 0.2d;
        pausedAtDspTime = 0d;
        totalPausedDspSeconds = 0d;
        gameMusic.PlayScheduled(songStartDspTime);
    }

    private void TogglePause()
    {
        if (!IsPlaying)
        {
            return;
        }

        bool willPause = !IsPaused;

        if (willPause)
        {
            pausedAtDspTime = AudioSettings.dspTime;
        }
        else
        {
            totalPausedDspSeconds += System.Math.Max(
                0d,
                AudioSettings.dspTime - pausedAtDspTime);
        }

        IsPaused = willPause;

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
        if (IsChoosingSong && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelSongSelection();
            return;
        }

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
            0f, (float)SongTimeSeconds);

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
            PlayerPrefs.SetInt(
                GetBestScoreKey(SelectedSong.trackId),
                bestScore);
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

        RefreshResultStats();

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

    private void RefreshResultStats()
    {
        if (resultStatsText == null)
        {
            return;
        }

        resultStatsText.text =
            LocalizationManager.Format(
                "result.successfulHits", Score.SuccessfulHits) + "\n" +
            LocalizationManager.Format(
                "result.onBeatHits", Score.OnBeatHits) + "\n" +
            LocalizationManager.Format(
                "result.maxStreak", Score.MaximumStreak);
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
