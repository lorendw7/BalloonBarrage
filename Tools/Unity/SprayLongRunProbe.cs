// Diagnostic player only: copy into an isolated project's Assets folder.
// Launch with -sprayProbe -probeOutput <new directory> at 2560x1440.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

[DefaultExecutionOrder(-2000)]
public sealed class SprayLongRunProbe : MonoBehaviour
{
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly FieldInfo NextShot = typeof(PlayerShooter).GetField("nextShotTime", Private);
    private static readonly Func<PlayerShooter, bool> HasFocus = CreateFocusReader();
    private RoundController controller;
    private PlayerShooter shooter;
    private Mouse mouse;
    private Keyboard keyboard;
    private ParticleSystem spray;
    private int stage, selected, lastPigmentStep = -1;
    private double stageStarted, nextSnapshot;
    private string output;
    private Report report;
    private ProfilerRecorder draws, batches, gc;
    private readonly FrameTiming[] timings = new FrameTiming[1];
    private readonly List<double> early = new List<double>(5000);
    private readonly List<double> late = new List<double>(5000);

    [Serializable] public sealed class Snapshot
    {
        public double songSeconds, frameMilliseconds, cpuMainMilliseconds, gpuMilliseconds;
        public long allocatedBytes, managedUsedBytes, drawCalls, batches, gcAllocatedInFrame;
        public int persistentMarks, liveParticles, transientImpactSystems;
    }
    [Serializable] public sealed class Report
    {
        public string status = "running", unityVersion, gpu, cpu, title, trackId, output;
        public string scope = "Non-Development diagnostic Windows player; natural DSP longest song; synthetic held mouse/periodic pigment changes; method-driven focus and suspension assertions, then replayed focused/released input if external tools interrupt the performance workload. Not physical Alt+Tab, human aiming/listening or venue acceptance.";
        public int width, height, requestedFps = 120, focusAssertions, peakMarks, performanceFocusReplays;
        public double durationSeconds, completedAtSongSeconds, wallSeconds, earlyFrameP50, earlyFrameP95, lateFrameP50, lateFrameP95;
        public bool results, paintSurvivesResults, retryClearsPaint, zeroResultsParticles;
        public bool drawCounterAvailable, batchesCounterAvailable, gcCounterAvailable;
        public List<Snapshot> samples = new List<Snapshot>();
        public List<string> errors = new List<string>();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Application.isEditor) return;
        if (!Environment.GetCommandLineArgs().Contains("-sprayProbe") && Application.productName != "SprayLongRunProbe") return;
        var root = new GameObject("SprayLongRunDiagnostic");
        DontDestroyOnLoad(root);
        root.AddComponent<SprayLongRunProbe>();
    }

    private void Awake()
    {
        string[] args = Environment.GetCommandLineArgs();
        int outputIndex = Array.IndexOf(args, "-probeOutput");
        output = outputIndex >= 0
            ? Path.GetFullPath(args[outputIndex + 1])
            : Path.GetFullPath(Path.Combine(Application.dataPath, "../probe-reports", DateTime.UtcNow.ToString("yyyyMMddTHHmmss")));
        Directory.CreateDirectory(output);
        report = new Report { unityVersion = Application.unityVersion, gpu = SystemInfo.graphicsDeviceName,
            cpu = SystemInfo.processorType, output = output };
        Application.logMessageReceived += OnLog;
        Application.runInBackground = true;
        Screen.SetResolution(2560, 1440, false);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
        foreach (var device in InputSystem.devices.Where(d => d is Mouse || d is Keyboard).ToArray()) InputSystem.DisableDevice(device);
        mouse = InputSystem.AddDevice<Mouse>("SprayProbeMouse"); mouse.MakeCurrent();
        keyboard = InputSystem.AddDevice<Keyboard>("SprayProbeKeyboard"); keyboard.MakeCurrent();
        stageStarted = Time.realtimeSinceStartupAsDouble;
    }

    private void Update()
    {
        try
        {
            double now = Time.realtimeSinceStartupAsDouble;
            Require(now - stageStarted < 330d, "Diagnostic timed out.");
            Require(report.errors.Count == 0, "Unity runtime error: " + string.Join(" | ", report.errors));
            if (stage == 0)
            {
                if (now - stageStarted < 1d) return;
                controller = Object.FindFirstObjectByType<RoundController>();
                shooter = Object.FindFirstObjectByType<PlayerShooter>();
                Require(controller != null && shooter != null, "Production controller/shooter missing.");
                selected = Enumerable.Range(0, controller.SongProfiles.Count)
                    .Where(i => controller.SongProfiles[i].mode == "standard")
                    .OrderByDescending(i => controller.SongProfiles[i].roundDurationSeconds).First();
                controller.SelectSong(selected);
                report.title = controller.SelectedSong.title; report.trackId = controller.SelectedSong.trackId;
                report.durationSeconds = controller.SelectedSong.roundDurationSeconds;
                report.width = Screen.width; report.height = Screen.height;
                Require(report.width == 2560 && report.height == 1440, "Diagnostic must render at 2560x1440.");
                spray = (ParticleSystem)Read(shooter, "muzzleSpray");
                stage = 1; return;
            }
            if (stage == 1)
            {
                var clip = controller.SelectedSong.audioClip;
                if (clip.loadState == AudioDataLoadState.Unloaded) { clip.LoadAudioData(); return; }
                if (clip.loadState == AudioDataLoadState.Loading) return;
                Require(clip.loadState == AudioDataLoadState.Loaded, "Longest song failed to load.");
                controller.StartSelectedSong();
                Require(RoundController.IsPlaying, "Longest song did not start.");
                VerifyFocus();
                draws = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1);
                batches = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count", 1);
                gc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 1);
                report.drawCounterAvailable = draws.Valid; report.batchesCounterAvailable = batches.Valid; report.gcCounterAvailable = gc.Valid;
                nextSnapshot = 2d; stage = 2; Save(); return;
            }
            if (stage == 2)
            {
                if (!RoundController.IsPlaying)
                {
                    Require(report.completedAtSongSeconds >= report.durationSeconds - .5d, "Round settled early.");
                    report.results = ((GameObject)Read(controller, "resultsScreen")).activeInHierarchy;
                    Require(report.results, "Natural Results did not appear.");
                    Send(0); stage = 3; stageStarted = now; return;
                }
                double songTime = controller.SongTimeSeconds;
                report.completedAtSongSeconds = songTime;
                // Test driver only: keep the workload steady after the focus assertions.
                // The private getter is compiled once to avoid per-frame reflection allocations.
                if (!HasFocus(shooter))
                {
                    Call(shooter, "OnApplicationFocus", true);
                    Send(0); Call(shooter, "Update");
                    report.performanceFocusReplays++;
                }
                int pigmentStep = (int)(songTime / 8d);
                Send(songTime < .3d ? (ushort)0 : (ushort)1,
                    new Vector2((.5f + .24f * Mathf.Sin((float)songTime * .14f)) * Screen.width,
                        (.5f + .15f * Mathf.Cos((float)songTime * .19f)) * Screen.height),
                    pigmentStep != lastPigmentStep);
                lastPigmentStep = pigmentStep;
                if (songTime >= 10d && songTime < 40d) early.Add(Time.unscaledDeltaTime * 1000d);
                if (songTime >= report.durationSeconds - 35d && songTime < report.durationSeconds - 5d) late.Add(Time.unscaledDeltaTime * 1000d);
                FrameTimingManager.CaptureFrameTimings();
                if (songTime >= nextSnapshot)
                {
                    TakeSnapshot(songTime); nextSnapshot += 15d; Save();
                }
                return;
            }
            if (stage == 3)
            {
                if (now - stageStarted < 2.3d) return;
                int marks = MarkCount();
                report.paintSurvivesResults = marks >= report.peakMarks && marks > 500;
                report.zeroResultsParticles = spray.particleCount == 0;
                Require(report.paintSurvivesResults && report.zeroResultsParticles, "Results lost paint or retained the nozzle stream.");
                TakeSnapshot(report.durationSeconds);
                ScreenCapture.CaptureScreenshot(Path.Combine(output, "painted-results.png"));
                stage = 4; stageStarted = now; return;
            }
            if (stage == 4)
            {
                if (now - stageStarted < 1d) return;
                controller.RestartRound(); stage = 5; stageStarted = now; return;
            }
            if (stage == 5)
            {
                if (now - stageStarted < 1d) return;
                report.retryClearsPaint = MarkCount() == 0 && !RoundController.IsPlaying && Score.Value == 0;
                Require(report.retryClearsPaint, "Retry retained paint or round state.");
                report.earlyFrameP50 = Percentile(early, .5); report.earlyFrameP95 = Percentile(early, .95);
                report.lateFrameP50 = Percentile(late, .5); report.lateFrameP95 = Percentile(late, .95);
                report.status = "passed"; Save(); Application.Quit(0);
            }
        }
        catch (Exception error)
        {
            report.status = "failed"; report.errors.Add(error.ToString()); Save(); Debug.LogError(error); Application.Quit(1);
        }
    }

    private void VerifyFocus()
    {
        Call(shooter, "OnApplicationFocus", true);
        Send(0); Call(shooter, "Update");
        Send(1); Write(shooter, "nextShotTime", 0f); Call(shooter, "Update");
        Check((float)NextShot.GetValue(shooter) > Time.time, "Focused input did not fire.");
        Call(shooter, "SetSprayVisual", true, 10f); spray.Emit(4);
        var audio = (AudioSource)Read(shooter, "shotAudioSource"); audio.Play();
        Call(shooter, "OnApplicationFocus", false);
        Check(!spray.isPlaying && spray.particleCount == 0 && !audio.isPlaying, "Focus loss retained spray or shot audio.");
        Check(Cursor.visible && !((RectTransform)Read(shooter, "aimReticle")).gameObject.activeSelf, "Focus loss did not restore pointer.");
        Quaternion rotation = shooter.transform.rotation;
        int pigment = (int)Read(shooter, "sprayPigmentIndex");
        Send(1, new Vector2(150f, 200f), true); Write(shooter, "nextShotTime", 0f); Call(shooter, "Update");
        Check((float)NextShot.GetValue(shooter) == 0f && (int)Read(shooter, "sprayPigmentIndex") == pigment &&
            Quaternion.Angle(rotation, shooter.transform.rotation) < .001f, "Unfocused input fired, aimed or changed pigment.");
        Call(shooter, "OnApplicationFocus", true);
        CheckHeldBlocked("Focus regain fired a held button."); ReleaseAndFire();
        Call(shooter, "OnApplicationPause", true);
        CheckHeldBlocked("Application suspension accepted fire.");
        Call(shooter, "OnApplicationPause", false);
        CheckHeldBlocked("Application resume fired a held button."); ReleaseAndFire();
        shooter.enabled = false; shooter.enabled = true;
        Check((bool)Read(shooter, "hasInputFocus") == Application.isFocused, "Re-enable retained stale focus state.");
        Call(shooter, "OnApplicationFocus", true);
        CheckHeldBlocked("Re-enabled shooter fired a held button."); ReleaseAndFire();
        controller.GetType().GetMethod("TogglePause", Private).Invoke(controller, null);
        CheckHeldBlocked("Paused round accepted fire."); controller.ResumeRound();
        CheckHeldBlocked("Round resume fired a held button."); ReleaseAndFire();
        Send(0); Call(shooter, "Update");
    }
    private void CheckHeldBlocked(string message)
    {
        Send(1); Write(shooter, "nextShotTime", 0f); Call(shooter, "Update");
        Check((float)NextShot.GetValue(shooter) == 0f && !spray.isPlaying && spray.particleCount == 0, message);
    }
    private void ReleaseAndFire()
    {
        Send(0); Call(shooter, "Update"); Send(1); Write(shooter, "nextShotTime", 0f); Call(shooter, "Update");
        Check((float)NextShot.GetValue(shooter) > Time.time, "Release and new press did not restore fire.");
    }
    private void Check(bool condition, string message) { Require(condition, message); report.focusAssertions++; }
    private void Send(ushort buttons, Vector2? position = null, bool changePigment = false)
    {
        InputSystem.QueueStateEvent(mouse, new MouseState { position = position ?? new Vector2(Screen.width * .7f, Screen.height * .55f), buttons = buttons });
        InputSystem.QueueStateEvent(keyboard, changePigment ? new KeyboardState(Key.C) : new KeyboardState());
        InputSystem.Update();
    }
    private static int MarkCount() => Object.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Count(r => r.name.StartsWith("SurfacePaint", StringComparison.Ordinal));
    private void TakeSnapshot(double songTime)
    {
        int count = MarkCount(); report.peakMarks = Mathf.Max(report.peakMarks, count);
        FrameTimingManager.GetLatestTimings(1, timings);
        report.samples.Add(new Snapshot { songSeconds = songTime, frameMilliseconds = Time.unscaledDeltaTime * 1000d,
            cpuMainMilliseconds = timings[0].cpuMainThreadFrameTime, gpuMilliseconds = timings[0].gpuFrameTime,
            allocatedBytes = Profiler.GetTotalAllocatedMemoryLong(), managedUsedBytes = Profiler.GetMonoUsedSizeLong(),
            drawCalls = draws.Valid ? draws.LastValue : -1, batches = batches.Valid ? batches.LastValue : -1,
            gcAllocatedInFrame = gc.Valid ? gc.LastValue : -1, persistentMarks = count,
            liveParticles = spray != null ? spray.particleCount : 0,
            transientImpactSystems = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None).Count(p => p.name.Contains("Clone")) });
    }
    private static double Percentile(List<double> values, double p) { values.Sort(); return values[Math.Min(values.Count - 1, (int)(values.Count * p))]; }
    private static Func<PlayerShooter, bool> CreateFocusReader()
    {
        var target = Expression.Parameter(typeof(PlayerShooter), "shooter");
        return Expression.Lambda<Func<PlayerShooter, bool>>(
            Expression.Field(target, typeof(PlayerShooter).GetField("hasInputFocus", Private)), target).Compile();
    }
    private static object Read(object target, string name) => target.GetType().GetField(name, Private).GetValue(target);
    private static void Write(object target, string name, object value) => target.GetType().GetField(name, Private).SetValue(target, value);
    private static void Call(object target, string name, params object[] args) => target.GetType().GetMethod(name, Private).Invoke(target, args);
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private void OnLog(string message, string trace, LogType type) { if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) report.errors.Add(message); }
    private void Save() { report.wallSeconds = Time.realtimeSinceStartupAsDouble; File.WriteAllText(Path.Combine(output, "report.json"), JsonUtility.ToJson(report, true)); }
    private void OnDestroy() { draws.Dispose(); batches.Dispose(); gc.Dispose(); Application.logMessageReceived -= OnLog; }
}
