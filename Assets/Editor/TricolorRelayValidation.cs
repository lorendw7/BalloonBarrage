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

/// <summary>Synthetic Editor integration checks; no claim of mouse, listening or package acceptance.</summary>
[InitializeOnLoad]
public static class TricolorRelayValidation
{
    private const string ActiveKey = "TricolorRelayValidation.Active";
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int stage, assertions, eventId = 800000;
    private static double deadline, waitUntil;
    private static readonly List<string> errors = new List<string>();
    private static UILanguage originalLanguage;
    private static string bestKey;
    private static bool hadBest;
    private static int savedBest;

    static TricolorRelayValidation()
    {
        if (SessionState.GetBool(ActiveKey, false)) EditorApplication.update += Tick;
    }

    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run this fixture in a batch Editor.");
        SessionState.SetBool(ActiveKey, true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying || !SessionState.GetBool(ActiveKey, false)) return;
        try
        {
            RoundController round = Object.FindFirstObjectByType<RoundController>();
            if (round == null || Time.frameCount < 8) return;
            if (stage == 0)
            {
                deadline = EditorApplication.timeSinceStartup + 120;
                Application.logMessageReceived += OnLog;
                originalLanguage = LanguageSettings.Current;
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (InputDevice device in InputSystem.devices)
                    if (device is Keyboard || device is Mouse) InputSystem.DisableDevice(device);
                round.BeginRound();
                round.SetSongMode("festival");
                round.SelectSong(0);
                bestKey = (string)typeof(RoundController).GetMethod("GetBestScoreKey",
                    BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { round.SelectedSong.trackId });
                hadBest = PlayerPrefs.HasKey(bestKey);
                savedBest = PlayerPrefs.GetInt(bestKey);
                stage = 1;
                return;
            }
            if (EditorApplication.timeSinceStartup >= deadline) throw new InvalidOperationException("Fixture timed out.");
            if (errors.Count > 0) throw new InvalidOperationException(string.Join("\n", errors));
            TricolorRelay relay = round.GetComponent<TricolorRelay>();
            if (stage == 1)
            {
                if (round.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                round.StartSelectedSong();
                Require(RoundController.IsPlaying, "Round did not start.");
                Object.FindFirstObjectByType<RhythmChartReader>().enabled = false;
                Require(relay != null && relay.CompletedCount == 0 && relay.CollectedCount == 0, "Fresh state is dirty.");
                Pop(0); Pop(0); Pop(5);
                Require(relay.CollectedCount == 2 && relay.CompletedCount == 0, "Repeated color consumed progress.");
                Require(Score.Value == 30 && Score.SuccessfulHits == 3, "Ordinary scores changed.");
                Call(round, "TogglePause");
                Pop(2);
                Require(relay.CollectedCount == 2 && Score.Value == 30, "Paused hit advanced progress/score.");
                waitUntil = EditorApplication.timeSinceStartup + .35;
                stage = 2;
                return;
            }
            if (EditorApplication.timeSinceStartup < waitUntil) return;
            if (stage == 2)
            {
                Require(RoundController.IsPaused && relay.CollectedCount == 2, "Pause discarded progress.");
                round.ResumeRound();
                LanguageSettings.SetLanguage(UILanguage.Japanese);
                Call(relay, "LateUpdate");
                Require(GameObject.Find("TricolorRelayOverlay") != null, "Relay HUD missing.");
                Capture("TricolorRelayProgress-ja");
                Pop(2, 10);
                Require(relay.CompletedCount == 1 && relay.CollectedCount == 0, "Three colors did not complete a relay.");
                Require(Score.Value == 50 && Score.OnBeatHits == 1, "Relay changed the 10/20 point contract.");
                Require(relay.IsCelebrating, "Completion feedback missing.");
                waitUntil = EditorApplication.timeSinceStartup + .1;
                stage = 3;
                return;
            }
            if (stage == 3)
            {
                Call(relay, "LateUpdate");
                Capture("TricolorRelayComplete-ja");
                foreach (UILanguage language in Enum.GetValues(typeof(UILanguage)))
                {
                    LanguageSettings.SetLanguage(language);
                    Call(relay, "LateUpdate");
                    foreach (string key in new[] { "relay.progress", "relay.complete", "relay.hint", "result.tricolorRelays" })
                        Require(LocalizationManager.GetText(key) != key, "Untranslated relay key: " + key);
                    Require(!GameObject.Find("Progress").GetComponent<Text>().text.Contains("{0}"), "HUD format was not expanded.");
                    Capture("TricolorRelayComplete-" + LanguageSettings.CurrentCode);
                }
                Pop(2); Pop(7);
                Require(relay.CollectedCount == 2 && relay.CompletedCount == 1, "New set did not begin independently.");
                Balloon expired = Spawn(6);
                expired.CheckExpiry(1000);
                Require(relay.CollectedCount == 2 && relay.CompletedCount == 1, "Miss discarded pigment collection.");
                Pop(1);
                Require(relay.CompletedCount == 2, "Second relay not counted.");
                relay.enabled = false;
                Pop(4);
                relay.enabled = true;
                Pop(4);
                Require(relay.CollectedCount == 1, "Enable/disable duplicated or retained event subscriptions.");
                LanguageSettings.SetLanguage(UILanguage.Japanese);
                Call(round, "EndRound");
                Call(relay, "LateUpdate");
                Require(!RoundController.IsPlaying && relay.CompletedCount == 2, "Settlement lost relay count.");
                Text result = (Text)typeof(RoundController).GetField("resultStatsText", Private).GetValue(round);
                Require(result.text.Contains(LocalizationManager.Format("result.tricolorRelays", 2)), "Result omits relays.");
                Require(GameObject.Find("TricolorRelayOverlay") == null, "HUD covers results.");
                Canvas.ForceUpdateCanvases();
                Capture("TricolorRelayResults-ja");
                Require(result.cachedTextGenerator.characterCountVisible >= result.text.Replace("\n", "").Length,
                    "Result statistics do not fit in the existing panel: " + result.cachedTextGenerator.characterCountVisible + "/" + result.text.Length);
                int before = Score.Value;
                Pop(0);
                Require(Score.Value == before && relay.CollectedCount == 1, "Post-round hit advanced state.");
                round.RestartRound();
                waitUntil = EditorApplication.timeSinceStartup + .75;
                stage = 4;
                return;
            }
            if (stage == 4)
            {
                Require(!RoundController.IsPlaying && !RoundController.IsPaused, "Retry did not return to title.");
                Require(relay.CompletedCount == 0 && relay.CollectedCount == 0 && !relay.IsCelebrating, "Retry retained relay state.");
                Require(Score.Value == 0, "Retry retained score.");
                Require(GameObject.Find("TricolorRelayOverlay") == null, "Retry leaked overlay.");
                Finish(null);
            }
        }
        catch (Exception exception) { Finish(exception); }
    }

    private static Balloon Spawn(int color)
    {
        BalloonSpawner spawner = Object.FindFirstObjectByType<BalloonSpawner>();
        int id = eventId++;
        Require(spawner.Spawn(new RhythmEventData { id = id, timeSeconds = 100d }), "Fixture target could not spawn.");
        foreach (Balloon balloon in Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None))
            if (balloon.EventId == id)
            {
                balloon.GetComponent<StudioBalloonAppearance>().ApplyVariant(color, 0);
                return balloon;
            }
        throw new InvalidOperationException("Spawned balloon missing.");
    }

    private static void Pop(int color, int bonus = 0)
    {
        Balloon balloon = Spawn(color);
        balloon.HitAt(balloon.transform.position, bonus);
        balloon.Hit(bonus); // The same target must never count twice.
    }
    private static void Call(object target, string method) => target.GetType().GetMethod(method, Private).Invoke(target, null);
    private static void Capture(string name) => typeof(KyudaisaiRuntimeSmokeCheck)
        .GetMethod("Capture", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { name });
    private static void OnLog(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(message);
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        assertions++;
    }
    private static void Finish(Exception error)
    {
        SessionState.SetBool(ActiveKey, false);
        Application.logMessageReceived -= OnLog;
        if (bestKey != null)
        {
            if (hadBest) PlayerPrefs.SetInt(bestKey, savedBest); else PlayerPrefs.DeleteKey(bestKey);
            LanguageSettings.SetLanguage(originalLanguage);
            PlayerPrefs.Save();
        }
        Directory.CreateDirectory("Logs");
        string status = error == null ? "passed" : "failed";
        File.WriteAllText("Logs/TricolorRelayRuntime.json", JsonUtility.ToJson(new Evidence {
            status = status, assertions = assertions, unityVersion = Application.unityVersion,
            error = error?.ToString(), finishedUtc = DateTime.UtcNow.ToString("o") }, true));
        if (error != null) Debug.LogException(error);
        else Debug.Log("TRICOLOR_RELAY_RUNTIME_OK: " + assertions + " assertions");
        EditorApplication.Exit(error == null ? 0 : 1);
    }
    [Serializable] private sealed class Evidence
    {
        public string status, unityVersion, finishedUtc, error;
        public int assertions;
        public string scope = "Synthetic Editor pop/scoring, pause, three UI languages, result layout, event lifecycle and scene Retry.";
        public string limits = "No mouse-input playtest, natural full-song run, human listening, standalone release or venue-hardware acceptance.";
    }
}
#endif
