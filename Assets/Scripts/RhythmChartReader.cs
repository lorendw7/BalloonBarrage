using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;



[Serializable]
public class RhythmEventData
{
    public int id;
    public double timeSeconds;
    [System.NonSerialized] public BarragePattern pattern;
    [System.NonSerialized] public int patternStep;
}

[Serializable]
public class RhythmChartData
{
    public string title;
    public float bpm;
    public double durationSeconds;
    public RhythmEventData[] events;
}


public class RhythmChartReader : MonoBehaviour
{
    private RhythmChartData chart;
    public bool IsReady => chart != null && chart.events != null && chart.events.Length > 0;

    private int nextEventIndex = 0;

    private int nextCueEventIndex = 0;

    [SerializeField]
    private RoundController roundController;

    [SerializeField]
    private double currentSongTime;

    [SerializeField]
    private BalloonSpawner balloonSpawner;

    [SerializeField]
    private double spawnLeadSeconds = 2.8d;

    [SerializeField]
    private Text beatCueText;

    [SerializeField]
    private Image beatApproachFill;

    [SerializeField]
    private Text onBeatFeedbackText;

    private float feedbackRemaining;

    private int displayedCueTenths = int.MinValue;

    private bool showingEndingCue;

    private int lastAwardPoints;

    private void ShownOnBeatFeedback(int awardPoints)
    {
        if (onBeatFeedbackText == null ||
            !RoundController.IsPlaying ||
            RoundController.IsPaused)
        {
            return;
        }

        lastAwardPoints = awardPoints;
        onBeatFeedbackText.text = LocalizationManager.Format(
            "feedback.onBeat", awardPoints);
        onBeatFeedbackText.enabled = true;
        feedbackRemaining = 1.45f;
    }

    private void OnEnable()
    {
        Balloon.OnBeatHit += ShownOnBeatFeedback;
        LanguageSettings.LanguageChanged += OnLanguageChanged;

        if (roundController != null)
        {
            roundController.SongSelectionChanged += LoadSelectedChart;
        }
    }

    private void OnDisable()
    {
        Balloon.OnBeatHit -= ShownOnBeatFeedback;
        LanguageSettings.LanguageChanged -= OnLanguageChanged;

        if (roundController != null)
        {
            roundController.SongSelectionChanged -= LoadSelectedChart;
        }

        feedbackRemaining = 0f;

        if (onBeatFeedbackText != null )
        {
            onBeatFeedbackText.enabled = false;
        }
    }

    private void OnLanguageChanged(UILanguage language)
    {
        displayedCueTenths = int.MinValue;
        showingEndingCue = false;

        if (onBeatFeedbackText != null && feedbackRemaining > 0f)
        {
            onBeatFeedbackText.text = LocalizationManager.Format(
                "feedback.onBeat", lastAwardPoints);
        }
    }

    private void LateUpdate()
    {
        if (onBeatFeedbackText == null)
        {
            return;
        }

        if(!RoundController.IsPlaying)
        {
            feedbackRemaining = 0f;
        } else if(!RoundController.IsPaused)
        {
            feedbackRemaining = Mathf.Max(
                0f, feedbackRemaining - Time.deltaTime);
        }

        onBeatFeedbackText.enabled = feedbackRemaining > 0f; 
     }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSelectedChart();
    }

    private void LoadSelectedChart()
    {
        chart = null;
        if (roundController == null || roundController.SelectedSong == null)
        {
            Debug.LogError("No song profile is available for the chart reader.");
            return;
        }

        ExhibitionSongProfile profile = roundController.SelectedSong;

        string path = Path.Combine(
            Application.streamingAssetsPath,
            profile.chartRelativePath);

        if (!File.Exists(path))
        {
            Debug.LogError("Chart file is missing: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        RhythmChartData sourceChart =
            JsonUtility.FromJson<RhythmChartData>(json);

        if (sourceChart == null || sourceChart.events == null ||
            sourceChart.events.Length == 0)
        {
            Debug.LogError("Chart has no events.");
            return;
        }

        List<RhythmEventData> shortRoundEvents =
            new List<RhythmEventData>();

        foreach (RhythmEventData sourceEvent in sourceChart.events)
        {
            if (sourceEvent.id < profile.sourceEventStartId ||
                sourceEvent.id >= profile.sourceEventEndIdExclusive)
            {
                continue;
            }

            double normalizedTime =
                sourceEvent.timeSeconds - profile.sourceCueInSeconds;

            double roundProgress = profile.roundDurationSeconds > 0d
                ? normalizedTime / profile.roundDurationSeconds
                : 0d;

            shortRoundEvents.Add(new RhythmEventData
            {
                id = sourceEvent.id,
                timeSeconds = System.Math.Max(0d, normalizedTime),
                pattern = KyudaisaiLearnerCore.SelectBarragePattern(
                    System.Math.Max(0d, System.Math.Min(1d, roundProgress))),
                patternStep = shortRoundEvents.Count
            });
        }

        chart = sourceChart;
        chart.events = shortRoundEvents.ToArray();
        chart.durationSeconds = profile.roundDurationSeconds;
        nextEventIndex = 0;
        nextCueEventIndex = 0;
        displayedCueTenths = int.MinValue;
        showingEndingCue = false;

        if (chart.events.Length == 0)
        {
            Debug.LogError("Selected short round has no chart events.");
            return;
        }

        Debug.Log($"Loaded {chart.title}: {chart.events.Length} events.");
        Debug.Log("Chart file read successfully.");
        RhythmEventData firstEvent = chart.events[0];
        Debug.Log($"First event: {firstEvent.timeSeconds:F3}s.");
    }

    private void UpdateBeatCue()
    {
        if (beatCueText == null)
        {
            return;
        }

        while (nextCueEventIndex < chart.events.Length &&
            currentSongTime > chart.events[nextCueEventIndex].timeSeconds)
        {
            nextCueEventIndex++;
        }

        if (nextCueEventIndex >= chart.events.Length)
        {
            if (!showingEndingCue)
            {
                beatCueText.text =
                    LocalizationManager.GetText("cue.ending");
                showingEndingCue = true;
                displayedCueTenths = int.MinValue;
            }

            if (beatApproachFill != null)
            {
                beatApproachFill.fillAmount = 0f;
            }
            return;
        }

        double remaining =
            chart.events[nextCueEventIndex].timeSeconds - currentSongTime;

        if(beatApproachFill != null && spawnLeadSeconds > 0d)
        {
            float curProgress = 1f - (float)(remaining / spawnLeadSeconds);

            beatApproachFill.fillAmount = Mathf.Clamp01(curProgress);
        }
        int remainingTenths = Mathf.Max(
            0,
            Mathf.RoundToInt((float)(remaining * 10d)));

        if (showingEndingCue ||
            displayedCueTenths != remainingTenths)
        {
            string remainingText =
                (remainingTenths / 10f).ToString(
                    "F1",
                    System.Globalization.CultureInfo.InvariantCulture);
            beatCueText.text = LocalizationManager.Format(
                "cue.next", remainingText);
            displayedCueTenths = remainingTenths;
            showingEndingCue = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (roundController == null ||
            balloonSpawner == null)
        {
            return;
        }

        currentSongTime = roundController.SongTimeSeconds;

        if (!RoundController.IsPlaying || RoundController.IsPaused ||
            chart == null || chart.events == null || 
            chart.events.Length == 0)
        {
            return;
        }

        UpdateBeatCue();
        balloonSpawner.CheckExpiredBalloons(currentSongTime);
        while (nextEventIndex < chart.events.Length
            && currentSongTime >= 
            chart.events[nextEventIndex].timeSeconds - spawnLeadSeconds)
        {
            RhythmEventData currentEvent = chart.events[nextEventIndex];

            Debug.Log($"Chart event {currentEvent.id} reached.");
            if (!balloonSpawner.Spawn(currentEvent)) break;
            nextEventIndex++;
        }
    }
}
