# Rhythm lesson 2: read the prepared chart

Updated: October 16, 2026. Loading, 0.8-second previews, target event initialization, 3.1-second cleanup retention, solo firing-time judgement, matched-target impact rewards and a separate-index song-clock countdown are saved. The learner reports cleanup, 10/20-point scoring, countdown and enlarged on-beat paint-burst playtests passed; latest gameplay has source inspection, not an independent Unity run. The [glow and temporary text assets](../Art/INTERFACE_V2.md#on-beat-feedback-handoff) are connected with learner-reported display/hide playtests passed; the selected text duration is 1.45 seconds. Full-song calibration remains pending. Delivery order lives in [Roadmap](ROADMAP.md).

## What the data means

`Assets/StreamingAssets/Rhythm/Prepared/GoinUnder_Beginner.json` is a complete timeline for the imported Goin' Under clip. It contains 93 events, duration about 123.429 seconds, constant 210 BPM and source attribution. Event zero is at 9.142857 seconds, event one at 10 seconds and event two at 11.428571 seconds. These are times since audio start, excluding the scheduled preparation lead. Source lane metadata is not world positioning.

Unity JsonUtility maps structured JSON to serializable C# fields; the names must match. This Windows-only preparation lesson reads the bundled file from Application.streamingAssetsPath rather than treating it as a TextAsset. [Unity JSON documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/json-serialization.html) explains the mapping. Original source/hashes/adaptation remain in [Music and charts](MUSIC_AND_CHARTS.md).

## Learner edits

The loading increments below are saved in `Assets/Scripts/RhythmChartReader.cs`, followed by event initialization, cleanup, rewards and the countdown. Keep the learner's work and serialized references. UpdateBeatCue uses a separate cue-event index rather than the early-spawn index, so the display counts toward the original event time.

The complete reference below covers later increments too; teach it in small parts rather than replacing the learner's file wholesale:

```csharp
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class RhythmChartData
{
    public string title;
    public float bpm;
    public double durationSeconds;
    public RhythmEventData[] events;
}

[Serializable]
public class RhythmEventData
{
    public int id;
    public double timeSeconds;
}

public class RhythmChartReader : MonoBehaviour
{
    private RhythmChartData chart;

    private void Start()
    {
        string path = Path.Combine(
            Application.streamingAssetsPath,
            "Rhythm/Prepared/GoinUnder_Beginner.json");

        if (!File.Exists(path))
        {
            Debug.LogError("Prepared rhythm chart is missing: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        chart = JsonUtility.FromJson<RhythmChartData>(json);

        if (chart == null || chart.events == null ||
            chart.events.Length == 0)
        {
            Debug.LogError("Prepared rhythm chart has no events.");
            return;
        }

        Debug.Log($"Loaded {chart.title}: {chart.events.Length} events; " +
                  $"first event at {chart.events[0].timeSeconds:F3}s.");
    }
}
```

The original JSON also has beat, lanes, hashes and other metadata. The serializer ignores fields not requested by these classes; it does not delete them from the source file. This deliberately small lesson reads trusted, previously validated prepared data. General user-imported chart validation and cross-platform loading are outside this increment.

## Unity connection and acceptance

1. Save the script and wait for compilation.
2. Select the existing scene RoundController object. Add the RhythmChartReader component once. Keep its existing RoundController component and music references.
3. Save PrototypeScene and enter Play Mode. The chart loads on initialization, even while the title is visible.
4. Expect Console output: `Loaded Goin' Under: 93 events; first event at 9.143s.`
5. Start/pause/results/retry continue to use existing logic. Reloading on retry prints the diagnostic again. The data classes are ordinary C# objects; only RhythmChartReader is a scene component.

If there is no message, check compiler errors and whether the component is attached/enabled. If the missing-file message appears, check that the exact prepared JSON is present and that the project/build includes StreamingAssets/Rhythm/Prepared. Do not move or edit the original StepMania files to address this.

## Next increment

The bounded glow connection is saved and the learner reports its playtest passed. On October 16, source review confirmed Balloon.OnBeatHit and its positive-bonus notification immediately after scoring, independently of optional particles. The learner has also saved a separate onBeatFeedbackText reference, feedbackRemaining float, ShownOnBeatFeedback handler guarded by active/unpaused play, paired OnEnable/OnDisable subscription and a dedicated LateUpdate timer. Each notification shows the awarded total and resets the display timer (1.45 seconds, explicitly confirmed by the creator and matching saved source); LateUpdate subtracts scaled delta time only during active unpaused play, clears outside play, and enables Text only while time remains. OnDisable unsubscribes, clears time and hides the Text. The assistant repaired the scene hookup to a new GameplayHUD child at (0, 168), replacing a direct prefab-asset reference; isolated Unity loading/layout/reference checks passed. The learner reports display and automatic hiding passed. Next connect the planned gameplay reticle and manually verify repeated rewarded hits, ordinary hits, pause/resume, complete-song settlement and retry. This text must not score again or trigger on firing/miss/expiry. The 3.1-second target retention remains separate from the saved 0.1-second timing window and from formal Miss statistics. See the [asset handoff](../Art/INTERFACE_V2.md#on-beat-feedback-handoff) and [validation record](../Project/VALIDATION.md).
