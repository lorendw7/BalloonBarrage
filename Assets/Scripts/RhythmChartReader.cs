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
    public bool authoredPlacement;
    public int placementSlot;
    public int trajectory;
    public double motionStartSeconds;
    public double motionEndSeconds;
    public double motionPeriodSeconds;
    public bool finaleAccent;
    [System.NonSerialized] public BarragePattern pattern;
    [System.NonSerialized] public int patternStep;
    [System.NonSerialized] public double previewNotBeforeSeconds;
}

[Serializable]
public class RhythmSectionData
{
    public double startSeconds;
    public double endSeconds;
    public int pattern;
    public int direction = 1;
}

[Serializable]
public class RhythmDemonstrationData
{
    public double startSeconds;
    public double stepSeconds;
    public double answerStartSeconds;
    public double answerEndSeconds;
    public int[] slots;
    public int[] answerSlots;
    public int[] answerEventIds;
    public bool keepAnswerGuide;
}

[Serializable]
public class RhythmFinaleData
{
    public int[] eventIds;
    public int[] slots;
}

[Serializable]
public class RhythmChartData
{
    public string title;
    public float bpm;
    public double durationSeconds;
    public RhythmEventData[] events;
    public RhythmSectionData[] sections;
    public RhythmDemonstrationData[] demonstrations;
    public RhythmFinaleData finale;
}


public class RhythmChartReader : MonoBehaviour
{
    private RhythmChartData chart;
    public bool IsReady => chart != null && chart.events != null && chart.events.Length > 0;
    public RhythmChartData LoadedChart => chart;
    public bool IsResolved(int eventId) => resolvedEventIds.Contains(eventId);

    private int nextEventIndex = 0;

    private int nextCueEventIndex = 0;
    private readonly HashSet<int> resolvedEventIds = new HashSet<int>();

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

    private void OnTargetResolved(int eventId)
    {
        resolvedEventIds.Add(eventId);
        displayedCueTenths = int.MinValue;
    }

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
        Balloon.OnResolved += OnTargetResolved;
        LanguageSettings.LanguageChanged += OnLanguageChanged;

        if (roundController != null)
        {
            roundController.SongSelectionChanged += LoadSelectedChart;
        }
    }

    private void OnDisable()
    {
        Balloon.OnBeatHit -= ShownOnBeatFeedback;
        Balloon.OnResolved -= OnTargetResolved;
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
        resolvedEventIds.Clear();
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

            BarragePattern pattern = KyudaisaiLearnerCore.SelectBarragePattern(
                System.Math.Max(0d, System.Math.Min(1d, roundProgress)));
            int direction = 1;
            if (sourceChart.sections != null)
            {
                foreach (RhythmSectionData section in sourceChart.sections)
                {
                    if (normalizedTime >= section.startSeconds && normalizedTime < section.endSeconds)
                    {
                        pattern = (BarragePattern)Mathf.Clamp(section.pattern, 0, 2);
                        direction = section.direction < 0 ? -1 : 1;
                        break;
                    }
                }
            }

            var roundEvent = new RhythmEventData
            {
                id = sourceEvent.id,
                timeSeconds = System.Math.Max(0d, normalizedTime),
                authoredPlacement = sourceEvent.authoredPlacement,
                placementSlot = Mathf.Clamp(sourceEvent.placementSlot, -2, 2) * direction,
                pattern = pattern,
                trajectory = sourceEvent.trajectory,
                motionStartSeconds = sourceEvent.motionStartSeconds,
                motionEndSeconds = sourceEvent.motionEndSeconds,
                motionPeriodSeconds = sourceEvent.motionPeriodSeconds,
                finaleAccent = sourceEvent.finaleAccent,
                patternStep = shortRoundEvents.Count
            };
            // Answer targets become hittable only after the demonstration has finished.
            // Their musical judgement times and ordinary/on-beat rewards stay unchanged.
            if (sourceChart.demonstrations != null)
            {
                foreach (RhythmDemonstrationData demonstration in sourceChart.demonstrations)
                {
                    if (demonstration.slots == null || demonstration.stepSeconds <= 0d) continue;
                    for (int index = 0; index < demonstration.slots.Length; index++)
                    {
                        double answerTime = demonstration.answerStartSeconds + index * demonstration.stepSeconds;
                        bool explicitIds = demonstration.answerEventIds != null &&
                            demonstration.answerEventIds.Length == demonstration.slots.Length;
                        if (explicitIds ? demonstration.answerEventIds[index] != roundEvent.id :
                            Math.Abs(normalizedTime - answerTime) > 0.02d) continue;
                        roundEvent.authoredPlacement = true;
                        int[] answerSlots = demonstration.answerSlots != null &&
                            demonstration.answerSlots.Length == demonstration.slots.Length
                            ? demonstration.answerSlots : demonstration.slots;
                        roundEvent.placementSlot = Mathf.Clamp(answerSlots[index], -2, 2);
                        roundEvent.pattern = BarragePattern.LateralSweep;
                        roundEvent.previewNotBeforeSeconds = demonstration.startSeconds +
                            demonstration.slots.Length * demonstration.stepSeconds;
                    }
                }
            }
            if (sourceChart.finale != null && sourceChart.finale.eventIds != null &&
                sourceChart.finale.slots != null)
            {
                for (int index = 0; index < Math.Min(sourceChart.finale.eventIds.Length, sourceChart.finale.slots.Length); index++)
                    if (sourceChart.finale.eventIds[index] == roundEvent.id)
                    {
                        roundEvent.authoredPlacement = true;
                        roundEvent.pattern = BarragePattern.LateralSweep;
                        roundEvent.placementSlot = sourceChart.finale.slots[index];
                    }
            }
            shortRoundEvents.Add(roundEvent);
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

    public double GetSpawnTime(RhythmEventData eventData)
    {
        return Math.Max(eventData.previewNotBeforeSeconds,
            eventData.timeSeconds - spawnLeadSeconds);
    }

    private void UpdateBeatCue()
    {
        if (beatCueText == null)
        {
            return;
        }

        while (nextCueEventIndex < chart.events.Length &&
            (currentSongTime > chart.events[nextCueEventIndex].timeSeconds ||
             resolvedEventIds.Contains(chart.events[nextCueEventIndex].id)))
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
            GetSpawnTime(chart.events[nextEventIndex]))
        {
            RhythmEventData currentEvent = chart.events[nextEventIndex];

            Debug.Log($"Chart event {currentEvent.id} reached.");
            if (!balloonSpawner.Spawn(currentEvent)) break;
            nextEventIndex++;
        }
    }
}
