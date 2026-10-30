#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>Controlled-clock Editor acceptance; this does not replace human aiming/listening.</summary>
[InitializeOnLoad]
public static class PhraseShowcaseValidation
{
    private const string Active = "PhraseShowcaseValidation.Active";
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int stage, assertions;
    private static double deadline, waitUntil;
    private static UILanguage language;
    private static string record;
    private static int? savedRecord;
    private static readonly List<string> errors = new List<string>();
    private static MelodyTrajectory[] movers;
    private static Vector3[] pausedPositions;
    private static double pausedSongTime;

    static PhraseShowcaseValidation()
    {
        if (SessionState.GetBool(Active, false)) EditorApplication.update += Tick;
    }

    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use a batch Editor.");
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying || !SessionState.GetBool(Active, false)) return;
        try
        {
            RoundController round = Object.FindFirstObjectByType<RoundController>();
            if (round == null || Time.frameCount < 8) return;
            if (stage == 0)
            {
                deadline = EditorApplication.timeSinceStartup + 120;
                Application.logMessageReceived += OnLog;
                language = LanguageSettings.Current;
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (InputDevice device in InputSystem.devices)
                    if (device is Mouse || device is Keyboard) InputSystem.DisableDevice(device);
                round.BeginRound();
                record = round.SelectedBestScoreKey;
                savedRecord = PlayerPrefs.HasKey(record) ? PlayerPrefs.GetInt(record) : (int?)null;
                Require(round.SelectedSong.trackId.EndsWith("-showcase-v3"), "Default profile is not the versioned showcase.");
                Require(record.EndsWith(".Rhythm.Showcase.v3"), "Showcase shares the old record.");
                foreach (UILanguage locale in Enum.GetValues(typeof(UILanguage)))
                {
                    LanguageSettings.SetLanguage(locale);
                    foreach (string key in new[] { "cue.mirror.watch", "cue.mirror.respond", "cue.finale", "chain.preview", "showcase.tag" })
                        Require(LocalizationManager.GetText(key) != key, "Missing translation: " + key);
                }
                LanguageSettings.SetLanguage(UILanguage.Japanese);
                stage = 1; return;
            }
            if (EditorApplication.timeSinceStartup > deadline) throw new InvalidOperationException("Fixture timed out.");
            if (errors.Count > 0) throw new InvalidOperationException(string.Join(" | ", errors));
            if (EditorApplication.timeSinceStartup < waitUntil) return;
            var reader = Object.FindFirstObjectByType<RhythmChartReader>();
            var cue = Object.FindFirstObjectByType<FestivalDemonstrationCue>();
            if (stage == 1)
            {
                if (round.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                round.StartSelectedSong();
                Require(RoundController.IsPlaying && reader.LoadedChart.events.Length == 45, "Showcase failed to load.");
                Seek(round, reader, 24d);
                Call(cue, "LateUpdate");
                Require(cue.IsMirrored && cue.IsDemonstrating && cue.VisibleMarkerCount == 3, "Middle demonstration is missing.");
                Require(Score.Value == 0, "Watch markers scored.");
                foreach (Graphic graphic in GameObject.Find("FestivalDemonstrationOverlay").GetComponentsInChildren<Graphic>(true))
                    Require(!graphic.raycastTarget, "Guide blocks aiming.");
                Capture("ShowcaseMirrorWatch-ja");
                Seek(round, reader, 27.6d);
                Call(cue, "LateUpdate");
                Require(cue.IsMirrored && cue.IsResponding && cue.VisibleMarkerCount == 3, "Answer outlines did not persist.");
                Require(reader.LoadedChart.events[15].placementSlot == 1 && reader.LoadedChart.events[17].placementSlot == -1,
                    "Middle answer did not reverse columns.");
                Capture("ShowcaseMirrorAnswer-ja");
                Balloon answer = FindBalloon(15);
                Require(answer != null, "First mirror answer target is missing.");
                answer.Hit();
                Call(cue, "LateUpdate");
                Require(cue.VisibleMarkerCount == 2 && Score.Value == 10, "Resolved answer marker or score mismatch.");
                Seek(round, reader, 36.6d);
                movers = Object.FindObjectsByType<MelodyTrajectory>(FindObjectsSortMode.None);
                Require(movers.Length > 0, "Authored motion did not attach.");
                var before = new Vector3[movers.Length];
                for (int i = 0; i < movers.Length; i++) { Call(movers[i], "FixedUpdate"); before[i] = movers[i].GetComponent<Rigidbody>().position; }
                Seek(round, reader, 37.6d);
                bool moved = false;
                for (int i = 0; i < movers.Length; i++)
                {
                    Call(movers[i], "FixedUpdate");
                    Vector3 current = movers[i].GetComponent<Rigidbody>().position;
                    moved |= (current - before[i]).sqrMagnitude > .00001f;
                    Vector3 origin = (Vector3)typeof(MelodyTrajectory).GetField("origin", Private).GetValue(movers[i]);
                    Require(Mathf.Abs(current.x - origin.x) <= .181f && Mathf.Abs(current.y - origin.y) <= .121f,
                        "Motion left its authored amplitude.");
                    var owner = Object.FindFirstObjectByType<BalloonSpawner>();
                    Require((owner.ClampTargetPosition(current) - current).sqrMagnitude < .000001f,
                        "Moving target left the playable bounds.");
                    Balloon logical = movers[i].GetComponent<Balloon>();
                    Require(Math.Abs(logical.EventTimeSeconds - reader.LoadedChart.events[logical.EventId].timeSeconds) < .000001,
                        "Motion altered the judgement timestamp.");
                }
                Require(moved, "Motion never changed a logical target position.");
                Capture("ShowcaseMelody-ja");
                Call(round, "TogglePause");
                pausedPositions = new Vector3[movers.Length];
                for (int i = 0; i < movers.Length; i++) pausedPositions[i] = movers[i].GetComponent<Rigidbody>().position;
                pausedSongTime = round.SongTimeSeconds;
                waitUntil = EditorApplication.timeSinceStartup + .3;
                stage = 2; return;
            }
            if (stage == 2)
            {
                Require(RoundController.IsPaused && Math.Abs(round.SongTimeSeconds - pausedSongTime) < .001, "Pause advanced the song clock.");
                for (int i = 0; i < movers.Length; i++)
                {
                    Call(movers[i], "FixedUpdate");
                    Require((movers[i].GetComponent<Rigidbody>().position - pausedPositions[i]).sqrMagnitude < .000001f,
                        "Paused motion drifted.");
                }
                Call(round, "TogglePause");
                var owner = Object.FindFirstObjectByType<BalloonSpawner>();
                Require(owner.Spawn(new RhythmEventData { id = 999998, timeSeconds = 80d, trajectory = 1,
                    motionStartSeconds = 36d, motionEndSeconds = 45.714285714d, motionPeriodSeconds = 4d }),
                    "Restoration probe failed to spawn.");
                MelodyTrajectory restorationProbe = FindBalloon(999998).GetComponent<MelodyTrajectory>();
                Seek(round, reader, 45.8d);
                Call(restorationProbe, "FixedUpdate");
                Require(!restorationProbe.IsActive && restorationProbe.GetComponent<BalloonFloat>().enabled &&
                    !restorationProbe.GetComponent<Rigidbody>().isKinematic, "End of phrase did not restore float/physics.");
                foreach (MelodyTrajectory mover in movers)
                {
                    if (mover == null || !mover.gameObject.activeSelf) continue;
                    Call(mover, "FixedUpdate");
                    Require(!mover.IsActive && mover.GetComponent<BalloonFloat>().enabled && !mover.GetComponent<Rigidbody>().isKinematic,
                        "Motion did not restore ordinary target behaviour.");
                }
                Seek(round, reader, 61d);
                Call(cue, "LateUpdate");
                Require(cue.IsFinale && cue.VisibleMarkerCount == 3, "Final route is missing.");
                Capture("ShowcaseFinaleRoute-ja");
                Balloon accent = FindBalloon(44);
                Require(accent != null && accent.IsFinaleAccent, "Final accent target is missing.");
                int beforeScore = Score.Value, beforeHits = Score.SuccessfulHits;
                accent.Hit();
                Require(accent.FinaleEffectCount == 3 && Score.Value == beforeScore + 10 && Score.SuccessfulHits == beforeHits + 1,
                    "Finale duplicated score or omitted its multicolour effect.");
                accent.Hit();
                Require(accent.FinaleEffectCount == 3 && Score.Value == beforeScore + 10, "Repeated final hit duplicated the flourish.");
                var spawner = Object.FindFirstObjectByType<BalloonSpawner>();
                Require(spawner.Spawn(new RhythmEventData { id = 999999, timeSeconds = -10d, finaleAccent = true }), "Expiry probe failed to spawn.");
                Balloon expired = FindBalloon(999999);
                expired.CheckExpiry(round.SongTimeSeconds);
                Require(expired.FinaleEffectCount == 0 && Score.Value == beforeScore + 10, "Unhit finale expiry scored or exploded.");
                Call(cue, "LateUpdate");
                Require(cue.VisibleMarkerCount == 2, "Resolved finale guide remained visible.");
                waitUntil = EditorApplication.timeSinceStartup + .1;
                stage = 21; return;
            }
            if (stage == 21)
            {
                Capture("ShowcaseFinaleBurst-ja");
                Call(round, "EndRound");
                Call(cue, "LateUpdate");
                Require(!cue.IsFinale && cue.VisibleMarkerCount == 0, "Settlement retained the route.");
                round.RestartRound();
                waitUntil = EditorApplication.timeSinceStartup + .5;
                stage = 3; return;
            }
            if (stage == 3)
            {
                Require(!RoundController.IsPlaying && !RoundController.IsPaused && Score.Value == 0, "Retry did not reset the showcase.");
                Finish(null);
            }
        }
        catch (Exception error) { Finish(error); }
    }

    private static void Seek(RoundController round, RhythmChartReader reader, double time)
    {
        double paused = (double)typeof(RoundController).GetField("totalPausedDspSeconds", Private).GetValue(round);
        typeof(RoundController).GetField("songStartDspTime", Private).SetValue(round, AudioSettings.dspTime - paused - time);
        Call(reader, "Update"); Call(reader, "Update");
    }
    private static Balloon FindBalloon(int id) => Array.Find(Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None), b => b.EventId == id);
    private static void Call(object target, string method) => target.GetType().GetMethod(method, Private).Invoke(target, null);
    private static void Capture(string name)
    {
        Call(Object.FindFirstObjectByType<RoundController>(), "LateUpdate");
        Canvas.ForceUpdateCanvases();
        typeof(KyudaisaiRuntimeSmokeCheck).GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, new object[] { name });
    }
    private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); assertions++; }
    private static void OnLog(string message, string trace, LogType type)
    { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(message); }
    private static void Finish(Exception error)
    {
        SessionState.SetBool(Active, false); Application.logMessageReceived -= OnLog;
        if (record != null) { if (savedRecord.HasValue) PlayerPrefs.SetInt(record, savedRecord.Value); else PlayerPrefs.DeleteKey(record); }
        LanguageSettings.SetLanguage(language); PlayerPrefs.Save();
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/PhraseShowcaseRuntime.json", JsonUtility.ToJson(new Evidence {
            status = error == null ? "passed" : "failed", error = error?.ToString(), assertions = assertions,
            finishedUtc = DateTime.UtcNow.ToString("o"), unityVersion = Application.unityVersion }, true));
        if (error == null) Debug.Log("PHRASE_SHOWCASE_RUNTIME_OK: " + assertions); else Debug.LogException(error);
        EditorApplication.Exit(error == null ? 0 : 1);
    }
    [Serializable] private sealed class Evidence
    {
        public string status, error, finishedUtc, unityVersion;
        public int assertions;
        public string scope = "Controlled-clock Editor mirror guide/reverse answer, bounded logical motion/pause/restoration, final route/single-score flourish/expiry, locales/record isolation and Retry.";
        public string limits = "No natural full-song mouse play, standalone, real Mac, human aim comfort or musical/venue acceptance.";
    }
}
#endif
