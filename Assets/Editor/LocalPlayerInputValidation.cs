#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>Virtual gamepad integration only. Checks the saved learner firing and bonus through actual shooter sampling.</summary>
[InitializeOnLoad]
public static class LocalPlayerInputValidation
{
    private const string Active = "LocalPlayerInputValidation.Active";
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int stage, assertions;
    private static double deadline, wait;
    private static Gamepad first, second, spare;
    private static string record;
    private static int? savedRecord;
    private static string soloRecord;
    private static int? savedSoloRecord;
    private static UILanguage language;
    private static readonly List<string> errors = new List<string>();
    static LocalPlayerInputValidation() { if (SessionState.GetBool(Active, false)) EditorApplication.update += Tick; }
    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use a batch Editor.");
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        EditorApplication.update -= Tick; EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }
    [MenuItem("Balloon Barrage/Lessons/Check P2 shot gate")]
    public static void CheckLesson1()
    {
        bool ready = !CoopLearnerCore.IsSecondShotReady(0f, 1f) && CoopLearnerCore.IsSecondShotReady(1f, 1f) &&
            CoopLearnerCore.IsSecondShotReady(1.2f, 1f);
        if (ready) Debug.Log("COOP_LESSON_1_READY: early/equal/late cases pass; request review before the next lesson.");
        else Debug.LogWarning("COOP_LESSON_1_PENDING: edit only IsSecondShotReady in Assets/Scripts/CoopLearnerCore.cs.");
    }
    private static void Tick()
    {
        if (!EditorApplication.isPlaying || !SessionState.GetBool(Active, false)) return;
        try
        {
            var round = Object.FindFirstObjectByType<RoundController>();
            if (round == null || Time.frameCount < 8) return;
            var input = round.GetComponent<LocalPlayerInput>();
            if (stage == 0)
            {
                deadline = EditorApplication.timeSinceStartup + 100;
                Require(!CoopLearnerCore.IsSecondShotReady(.8f, 1f) && CoopLearnerCore.IsSecondShotReady(1f, 1f) &&
                    CoopLearnerCore.IsSecondShotReady(1.2f, 1f), "Learner shot boundary cases failed.");
                Require(CoopLearnerCore.SecondPlayerBeatBonus(true) == 10 && CoopLearnerCore.SecondPlayerBeatBonus(false) == 0,
                    "Learner beat bonus cases failed.");
                Application.logMessageReceived += OnLog;
                language = LanguageSettings.Current;
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (InputDevice device in InputSystem.devices)
                    if (device is Keyboard || device is Mouse || device is Gamepad) InputSystem.DisableDevice(device);
                round.BeginRound();
                string solo = round.SelectedBestScoreKey;
                soloRecord = solo; savedSoloRecord = PlayerPrefs.HasKey(solo) ? PlayerPrefs.GetInt(solo) : (int?)null;
                Require(input.Layout == LocalPlayerLayout.MouseSolo && !input.Secondary.gameObject.activeSelf, "Default solo changed.");
                round.SetLocalPlayerLayout(3);
                Require(!input.CanStart && !GameObject.Find("ConfirmSongButton").GetComponent<Button>().interactable, "Missing controllers allowed Play.");
                first = InputSystem.AddDevice<Gamepad>();
                round.SetLocalPlayerLayout(2);
                Require(input.CanStart && input.RequiredGamepads == 1, "Mouse/pad layout did not accept one gamepad.");
                Require(input.PrepareRound() && input.AssignedDeviceId(0) == -1 && input.AssignedDeviceId(1) == first.deviceId,
                    "Mouse/pad layout assigned the gamepad to the wrong player.");
                Require(round.SelectedBestScoreKey == solo + ".LocalCoop.v1", "Co-op record is not isolated.");
                round.SetLocalPlayerLayout(1);
                Require(input.CanStart && round.SelectedBestScoreKey == solo, "Gamepad solo changed scoring identity.");
                round.SetLocalPlayerLayout(3);
                Require(!input.CanStart, "One pad was shared by two players.");
                second = InputSystem.AddDevice<Gamepad>();
                Require(input.CanStart, "Two-pad layout did not become ready.");
                record = round.SelectedBestScoreKey; savedRecord = PlayerPrefs.HasKey(record) ? PlayerPrefs.GetInt(record) : (int?)null;
                foreach (UILanguage locale in Enum.GetValues(typeof(UILanguage)))
                {
                    LanguageSettings.SetLanguage(locale); Call(round, "RefreshSongSelectionText");
                    foreach (string key in new[] { "players.mouse", "players.pad", "players.mousePad", "players.twoPads", "players.missing", "players.lesson", "players.padControls", "players.reconnect" })
                        Require(LocalizationManager.GetText(key) != key, "Missing input translation: " + key);
                    Require(GameObject.Find("LocalPlayerLayout").GetComponent<Dropdown>().options.Count == 4, "Layout selector lost options.");
                }
                LanguageSettings.SetLanguage(UILanguage.Japanese);
                stage = 1; return;
            }
            if (EditorApplication.timeSinceStartup > deadline) throw new InvalidOperationException("Fixture timed out.");
            if (errors.Count > 0) throw new InvalidOperationException(string.Join(" | ", errors));
            if (EditorApplication.timeSinceStartup < wait) return;
            if (stage == 1)
            {
                if (round.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                round.StartSelectedSong();
                Require(RoundController.IsPlaying && input.Secondary.gameObject.activeSelf, "Co-op infrastructure did not start.");
                Object.FindFirstObjectByType<RhythmChartReader>().enabled = false;
                Write(input, "hasFocus", true); Write(input.Primary, "hasInputFocus", true); Write(input.Secondary, "hasInputFocus", true);
                Require(input.AssignedDeviceId(0) == first.deviceId && input.AssignedDeviceId(1) == second.deviceId, "Players share a controller.");
                round.SetLocalPlayerLayout(0);
                Require(input.Layout == LocalPlayerLayout.TwoGamepads, "Layout changed mid-round.");
                spare = InputSystem.AddDevice<Gamepad>();
                Require(input.AssignedDeviceId(0) == first.deviceId && input.AssignedDeviceId(1) == second.deviceId, "Joining pad stole an assignment.");
                stage = 10; wait = EditorApplication.timeSinceStartup + .1; return;
            }
            if (stage == 10)
            {
                Vector2 p1 = input.ViewportAim(0), p2 = input.ViewportAim(1);
                State(first, new GamepadState()); State(second, new GamepadState { leftStick = Vector2.right });
                Call(input, "Update");
                Require(input.ViewportAim(0) == p1 && input.ViewportAim(1).x > p2.x,
                    "Independent aiming failed: " + p1 + "/" + input.ViewportAim(0) + "; " + p2 + "/" + input.ViewportAim(1) +
                    "; stick=" + second.leftStick.ReadValue() + "; dt=" + Time.unscaledDeltaTime + "; paused=" + RoundController.IsPaused);
                Require(input.LastAimingPlayer == 1, "Chain preview did not follow the active aim owner.");
                State(second, new GamepadState());
                Call(input.Primary, "Update"); Call(input.Secondary, "Update");
                int shots = input.Primary.ShotCount;
                State(first, new GamepadState { rightTrigger = 1f });
                Call(input.Primary, "Update");
                Require(input.Primary.ShotCount == shots + 1, "Solo/P1 gamepad RT did not fire.");
                Call(input.Primary, "Update");
                Require(input.Primary.ShotCount == shots + 1, "Gamepad ignored the existing shot interval.");
                State(first, new GamepadState());
                Balloon timedTarget = SpawnAimed(round, input, true);
                int scoreBefore = Score.Value, onBeatBefore = Score.OnBeatHits;
                State(second, new GamepadState { rightTrigger = 1f });
                Call(input.Secondary, "Update");
                Require(input.Secondary.ShotCount == 1 && Score.Value == scoreBefore + 20 && Score.OnBeatHits == onBeatBefore + 1,
                    "P2 timed shot did not award exactly 10 base + 10 learner bonus.");
                Call(input.Secondary, "Update");
                Require(input.Secondary.ShotCount == 1, "P2 bypassed the learner cooldown.");
                Write(input.Primary, "nextShotTime", 0f);
                State(first, new GamepadState { rightTrigger = 1f }); Call(input.Primary, "Update");
                timedTarget.HitAt(timedTarget.transform.position, 10);
                Require(Score.Value == scoreBefore + 20, "Shared target scored twice.");
                State(first, new GamepadState()); State(second, new GamepadState());
                SpawnAimed(round, input, false); Write(input.Secondary, "nextShotTime", 0f);
                State(second, new GamepadState { rightTrigger = 1f }); Call(input.Secondary, "Update");
                Require(input.Secondary.ShotCount == 2 && Score.Value == scoreBefore + 30 && Score.OnBeatHits == onBeatBefore + 1,
                    "P2 ordinary shot did not award exactly base points.");
                State(second, new GamepadState());
                InputSystem.DisableDevice(second);
                Require(RoundController.IsPaused && !input.DevicesReady, "Disconnected controller did not pause safely.");
                Call(input, "LateUpdate");
                Require(GameObject.Find("ControllerReconnectOverlay") != null, "Reconnect guidance is missing.");
                round.ResumeRound(); Require(RoundController.IsPaused, "Missing pad allowed Resume.");
                InputSystem.EnableDevice(second);
                Require(input.DevicesReady, "Returning assigned controller was not recognized.");
                round.ResumeRound(); Require(!RoundController.IsPaused, "Reconnected round could not resume.");
                int before = input.Primary.ShotCount;
                State(first, new GamepadState { rightTrigger = 1f });
                Call(input.Primary, "Update");
                Require(input.Primary.ShotCount == before, "Reconnect resumed a held trigger without release.");
                State(first, new GamepadState());
                Call(input.Primary, "Update");
                Call(round, "EndRound");
                State(first, new GamepadState { rightTrigger = 1f }); Call(input.Primary, "Update");
                Require(input.Primary.ShotCount == before, "Controller fired after settlement.");
                round.RestartRound(); stage = 2; wait = EditorApplication.timeSinceStartup + .5; return;
            }
            if (stage == 2)
            {
                Require(!RoundController.IsPlaying && !RoundController.IsPaused && input.Layout == LocalPlayerLayout.MouseSolo &&
                    !input.Secondary.gameObject.activeSelf, "Retry retained local player state.");
                round.BeginRound(); round.SetLocalPlayerLayout(1);
                stage = 3; return;
            }
            if (stage == 3)
            {
                if (round.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                State(first, new GamepadState { rightTrigger = 1f });
                round.StartSelectedSong();
                Write(input, "hasFocus", true); Write(input.Primary, "hasInputFocus", true);
                Require(input.Layout == LocalPlayerLayout.GamepadSolo && !input.Secondary.gameObject.activeSelf,
                    "Gamepad solo retained a second shooter.");
                Call(input.Primary, "Update");
                Require(input.Primary.ShotCount == 0, "Gamepad solo fired a trigger held across Play.");
                State(first, new GamepadState()); Call(input.Primary, "Update");
                State(first, new GamepadState { rightTrigger = 1f }); Call(input.Primary, "Update");
                Require(input.Primary.ShotCount == 1, "Gamepad solo could not fire after release.");
                Call(round, "EndRound"); round.RestartRound();
                stage = 4; wait = EditorApplication.timeSinceStartup + .5; return;
            }
            if (stage == 4)
            {
                round.BeginRound(); round.SetLocalPlayerLayout(2); stage = 5; return;
            }
            if (stage == 5)
            {
                if (round.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                round.StartSelectedSong(); Object.FindFirstObjectByType<RhythmChartReader>().enabled = false;
                Write(input, "hasFocus", true); Write(input.Secondary, "hasInputFocus", true);
                State(first, new GamepadState()); Call(input.Secondary, "Update");
                SpawnAimed(round, input, true);
                int before = Score.Value;
                State(first, new GamepadState { rightTrigger = 1f }); Call(input.Secondary, "Update");
                Require(input.AssignedDeviceId(1) == first.deviceId && input.Secondary.ShotCount == 1 && Score.Value == before + 20,
                    "Mouse + gamepad P2 did not fire/score from its assigned device.");
                Call(round, "EndRound"); Finish(null);
            }
        }
        catch (Exception error) { Finish(error); }
    }
    private static int nextTarget = 600000;
    private static Balloon SpawnAimed(RoundController round, LocalPlayerInput input, bool onBeat)
    {
        int id = nextTarget++;
        Require(Object.FindFirstObjectByType<BalloonSpawner>().Spawn(new RhythmEventData {
            id = id, timeSeconds = round.SongTimeSeconds + (onBeat ? 0d : 2d) }), "Scoring target failed to spawn.");
        Balloon target = Array.Find(Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None), b => b.EventId == id);
        target.GetComponent<BalloonFloat>().enabled = false; target.GetComponent<BalloonEntrance>().enabled = false;
        target.GetComponent<Rigidbody>().isKinematic = true; target.transform.position = new Vector3(0f, 0f, 8f);
        Physics.SyncTransforms();
        Collider body = Array.Find(target.GetComponentsInChildren<Collider>(), c => c.enabled && !c.isTrigger);
        Vector3 screen = Camera.main.WorldToScreenPoint(body.bounds.center);
        var aim = (Vector2[])typeof(LocalPlayerInput).GetField("aims", Private).GetValue(input);
        aim[0] = aim[1] = new Vector2(screen.x / Screen.width, screen.y / Screen.height);
        return target;
    }
    private static void State(Gamepad pad, GamepadState value)
    {
        // EditorApplication.update reads the Editor buffer, not a queued Dynamic update.
        InputState.Change(pad, value, InputState.currentUpdateType);
    }
    private static void Call(object target, string method) => target.GetType().GetMethod(method, Private).Invoke(target, null);
    private static void Write(object target, string name, object value) => target.GetType().GetField(name, Private).SetValue(target, value);
    private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); assertions++; }
    private static void OnLog(string message, string trace, LogType type) { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(message); }
    private static void Finish(Exception error)
    {
        SessionState.SetBool(Active, false); Application.logMessageReceived -= OnLog;
        if (record != null) { if (savedRecord.HasValue) PlayerPrefs.SetInt(record, savedRecord.Value); else PlayerPrefs.DeleteKey(record); }
        if (soloRecord != null) { if (savedSoloRecord.HasValue) PlayerPrefs.SetInt(soloRecord, savedSoloRecord.Value); else PlayerPrefs.DeleteKey(soloRecord); }
        LanguageSettings.SetLanguage(language); PlayerPrefs.Save();
        foreach (Gamepad pad in new[] { first, second, spare }) if (pad != null && pad.added) InputSystem.RemoveDevice(pad);
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/LocalPlayerInputRuntime.json", JsonUtility.ToJson(new Evidence { status = error == null ? "passed" : "failed",
            error = error?.ToString(), assertions = assertions, finishedUtc = DateTime.UtcNow.ToString("o") }, true));
        if (error == null) Debug.Log("LOCAL_PLAYER_INPUT_RUNTIME_OK: " + assertions); else Debug.LogException(error);
        EditorApplication.Exit(error == null ? 0 : 1);
    }
    [Serializable] private sealed class Evidence
    {
        public string status, error, finishedUtc;
        public int assertions;
        public string scope = "Directly injected virtual Input System device states: four layouts, device ownership, independent bounded aim, P1 RT/cadence, missing-device/reconnect pause and held-trigger protection, records/locales and Retry. No native event/physical-controller acceptance.";
        public string learnerStatus = "Saved learner methods pass early/equal/late and bonus cases; actual P2 shooter samples verify ordinary/timed scoring in both co-op layouts and one-time shared resolution.";
        public string limits = "No physical controller, native standalone, real-Mac or human comfort acceptance.";
    }
}
#endif
