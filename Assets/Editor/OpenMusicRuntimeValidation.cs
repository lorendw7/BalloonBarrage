#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Real-time, automated Editor evidence. Does not simulate human listening or aim.
[InitializeOnLoad]
public static class OpenMusicRuntimeValidation
{
    const string Key = "OpenMusicRuntimeValidation.Active";
    static double deadline, stageTime, pausedTime, lastSongTime;
    static int stage, round, peak;
    static readonly int[] Rounds = { 3, 7 };
    static string[] identities;
    static OpenMusicRuntimeValidation() { if (SessionState.GetBool(Key, false)) EditorApplication.update += Tick; }
    static void Require(bool ok, string reason) { if (!ok) throw new InvalidOperationException(reason); }
    public static void Run()
    {
        Require(Application.isBatchMode, "Run batch mode without -quit.");
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        SessionState.SetBool(Key, true); EditorApplication.update -= Tick; EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }
    static RhythmChartData Chart(RhythmChartReader reader) => (RhythmChartData)typeof(RhythmChartReader).GetField("chart", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(reader);
    static void Capture(string name)
    {
        typeof(KyudaisaiRuntimeSmokeCheck).GetMethod("Capture", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { name });
        File.Copy("Logs/" + name + ".png", OpenExpansionInstaller.Root + "/Previews/" + name + ".png", true);
    }
    static void Tick()
    {
        if (!SessionState.GetBool(Key, false) || !EditorApplication.isPlaying) return;
        try
        {
            if (deadline == 0) { deadline = EditorApplication.timeSinceStartup + 290; stageTime = EditorApplication.timeSinceStartup; }
            Require(EditorApplication.timeSinceStartup < deadline, "Music validation timed out.");
            var controller = Object.FindFirstObjectByType<RoundController>();
            if (controller == null || Time.frameCount < 8) return;
            var reader = Object.FindFirstObjectByType<RhythmChartReader>();
            if (stage == 0)
            {
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (var keyboard in InputSystem.devices.OfType<Keyboard>().ToArray()) InputSystem.DisableDevice(keyboard);
                controller.BeginRound();
                var list = GameObject.Find("SongList").GetComponent<Dropdown>();
                Require(list.options.Count == 10, "Expected ten song options.");
                identities = new string[10];
                var music = (AudioSource)new SerializedObject(controller).FindProperty("gameMusic").objectReferenceValue;
                for (int i = 0; i < 10; i++)
                {
                    list.value = i; controller.SelectSong(i);
                    var profile = controller.SelectedSong; identities[i] = profile.trackId;
                    Require(reader.IsReady && Chart(reader).title == profile.title, "Wrong chart: " + profile.title);
                    Require(music.clip == profile.audioClip && profile.audioClip.length + .06 >= profile.sourceCueOutSeconds, "Wrong/truncated clip: " + profile.title);
                    Require(Chart(reader).events.Length == profile.sourceEventEndIdExclusive - profile.sourceEventStartId, "Wrong event range: " + profile.title);
                    Require(Chart(reader).events.Last().timeSeconds < profile.roundDurationSeconds, "Event beyond round end.");
                    Debug.Log("OPEN_MUSIC_PROFILE_OK: " + profile.title + ", events=" + Chart(reader).events.Length + ", seconds=" + profile.roundDurationSeconds);
                }
                Require(identities.Distinct().Count() == 10, "Record identities collide.");
                controller.CancelSongSelection(); Require(!controller.IsChoosingSong && !RoundController.IsPlaying, "Back failed.");
                stage = 1; stageTime = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 1)
            {
                if (EditorApplication.timeSinceStartup - stageTime < 1) return;
                controller.BeginRound();
                stage = 6; stageTime = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 6)
            {
                if (EditorApplication.timeSinceStartup - stageTime < .5) return;
                if (round == 0)
                {
                    var list = GameObject.Find("SongList").GetComponent<Dropdown>();
                    list.value = 7; Capture("SongSelectionTen"); list.Show();
                    Require(GameObject.Find("Dropdown List") != null, "Dropdown did not open.");
                    stage = 7; stageTime = EditorApplication.timeSinceStartup; return;
                }
                StartRound(controller); return;
            }
            if (stage == 7)
            {
                if (EditorApplication.timeSinceStartup - stageTime < .5) return;
                Capture("SongListTen"); GameObject.Find("SongList").GetComponent<Dropdown>().Hide();
                StartRound(controller); return;
            }
            if (stage == 2)
            {
                int count = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).Count(b => b.gameObject.activeInHierarchy);
                peak = Math.Max(peak, count); Require(count <= 16, "Balloon limit exceeded.");
                if (controller.SongTimeSeconds < 8) return;
                Require(peak > 0, "No natural targets spawned.");
                Capture(round == 0 ? "GameplayCozy" : "GameplayClassic");
                typeof(RoundController).GetMethod("TogglePause", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(controller, null);
                pausedTime = controller.SongTimeSeconds; Require(RoundController.IsPaused, "Pause failed.");
                stageTime = EditorApplication.timeSinceStartup; stage = 3; return;
            }
            if (stage == 3)
            {
                if (EditorApplication.timeSinceStartup - stageTime < 1) return;
                Require(Math.Abs(controller.SongTimeSeconds - pausedTime) < .02, "Paused song clock moved.");
                controller.ResumeRound(); Require(!RoundController.IsPaused, "Resume failed."); stage = 4; return;
            }
            if (stage == 4)
            {
                if (RoundController.IsPlaying) { lastSongTime = controller.SongTimeSeconds; return; }
                var profile = controller.SelectedSong;
                Require(lastSongTime >= profile.roundDurationSeconds - .25, "Early settlement.");
                var results = (GameObject)new SerializedObject(controller).FindProperty("resultsScreen").objectReferenceValue;
                Require(results != null && results.activeInHierarchy, "Missing natural results screen.");
                Debug.Log("OPEN_MUSIC_NATURAL_ROUND_OK: " + profile.title + ", lastPlayingSeconds=" + lastSongTime + ", peak=" + peak + "; pause/resume and results passed.");
                round++; controller.RestartRound(); stageTime = EditorApplication.timeSinceStartup; stage = 5; return;
            }
            if (stage == 5)
            {
                if (EditorApplication.timeSinceStartup - stageTime < 2) return;
                Require(!RoundController.IsPlaying && !RoundController.IsPaused, "Retry did not reset state.");
                if (round < Rounds.Length) { stage = 1; return; }
                File.WriteAllText(OpenExpansionInstaller.Root + "/RuntimeVerification.txt", "OPEN_MUSIC_RUNTIME_OK\nAll ten dropdown profiles matched audio, charts, duration, event ranges and unique record identities.\nCozy Puzzle In-Game 1 (81.356s) and Chaoz Fantasy (133.904s) completed at real-time natural settlement with spawned targets, pause/resume, results and Retry.\nOther eight profiles: selection/data checks only; no full-round claim. No physical aiming/listening, timing calibration, real-Mac check or performance benchmark.\n");
                SessionState.SetBool(Key, false); Debug.Log("OPEN_MUSIC_RUNTIME_OK"); EditorApplication.Exit(0);
            }
        }
        catch (Exception error) { SessionState.SetBool(Key, false); Debug.LogException(error); EditorApplication.Exit(1); }
    }
    static void StartRound(RoundController controller)
    {
        controller.SelectSong(Rounds[round]); controller.StartSelectedSong();
        Require(RoundController.IsPlaying, "Round did not start.");
        stageTime = EditorApplication.timeSinceStartup; peak = 0; lastSongTime = 0; stage = 2;
    }
}
#endif
