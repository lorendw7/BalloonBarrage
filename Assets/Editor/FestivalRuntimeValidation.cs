#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>
/// Automated Editor integration evidence. Full mode uses the natural DSP clock;
/// fast mode deliberately advances its origin and is never a listening/playtest.
/// Run batch mode without -quit, using RunFull or RunFast as executeMethod.
/// </summary>
[InitializeOnLoad]
public static class FestivalRuntimeValidation
{
    private const string ActiveKey = "FestivalRuntimeValidation.Active";
    private const string ModeKey = "FestivalRuntimeValidation.Mode";
    private const string ShardIndexKey = "FestivalRuntimeValidation.ShardIndex";
    private const string ShardCountKey = "FestivalRuntimeValidation.ShardCount";
    private const string CatalogPath = "Assets/StreamingAssets/Rhythm/Festival20261029/catalog.json";
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private static Evidence evidence;
    private static Catalog catalog;
    private static int[] rounds;
    private static int stage, round, lastAdvancedFrame = -1;
    private static double deadline, stageStarted, pausedTime, advancedTime;
    private static int pausedScore, demonstrationScore;
    private static bool wasDemonstrating, demoCaptured, answerCaptured;
    private static double runStarted, roundStarted;
    private static int castDiagnostics;
    private static bool colliderSelectionLogged;
    private static readonly Dictionary<int, int> SpawnedIds = new Dictionary<int, int>();
    private static readonly Dictionary<string, SavedPreference> Preferences = new Dictionary<string, SavedPreference>();
    private static readonly List<string> RuntimeErrors = new List<string>();
    private static UILanguage originalLanguage;

    [Serializable] private sealed class Catalog { public Profile[] songs; }
    [Serializable] private sealed class Profile
    {
        public string trackId, title, mode, chartRelativePath, audioAssetPath;
        public double sourceCueInSeconds, sourceCueOutSeconds, roundDurationSeconds;
        public int sourceEventStartId, sourceEventEndIdExclusive;
    }
    [Serializable] private sealed class Chart
    {
        public Event[] events;
        public Section[] sections;
        public Demonstration[] demonstrations;
    }
    [Serializable] private sealed class Event
    {
        public int id, placementSlot;
        public double timeSeconds;
        public bool authoredPlacement;
    }
    [Serializable] private sealed class Section
    {
        public double startSeconds, endSeconds;
        public int pattern, direction;
    }
    [Serializable] private sealed class Demonstration
    {
        public double startSeconds, stepSeconds, answerStartSeconds;
        public int[] slots, answerSlots, answerEventIds;
    }
    [Serializable] private sealed class Evidence
    {
        public string status, mode, startedUtc, finishedUtc, unityVersion, sourceRevision;
        public string scope = "Automated Unity Editor: selection, profile data, sampled physics/pop scoring, demonstration, pause/resume, settlement and Retry.";
        public string limits = "No human listening/aiming/calibration, standalone-player full-round acceptance, venue observation, real-Mac check or performance benchmark. Synthetic scoring calls the existing spray SphereCast helper and accepted pop path; it does not simulate mouse input or certify held-fire feel.";
        public bool controlledClockOffsets;
        public int expectedProfiles, selectedProfiles, shardIndex, shardCount;
        public string[] assignedTrackIds;
        public string languageChecks;
        public double wallSeconds;
        public List<RoundEvidence> rounds = new List<RoundEvidence>();
        public List<string> errors = new List<string>();
    }
    [Serializable] private sealed class RoundEvidence
    {
        public string trackId, title, mode, settlement;
        public double durationSeconds, lastPlayingSeconds, wallSeconds;
        public int chartEvents, observedDistinctTargets, peakActiveTargets, successfulHits, onBeatHits, score;
        public bool pauseResume, demonstrationSeen, responseSeen, sampleScoring70, results, retry;
    }
    private sealed class SavedPreference { public bool existed; public int value; }

    static FestivalRuntimeValidation()
    {
        if (SessionState.GetBool(ActiveKey, false)) EditorApplication.update += Tick;
    }

    public static void RunFast() { Begin("fast"); }
    public static void RunFull() { Begin("full"); }
    public static void Run()
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, "-festivalValidationMode");
        Begin(index >= 0 && index + 1 < args.Length ? args[index + 1] : "fast");
    }
    private static void Begin(string mode)
    {
        Require(Application.isBatchMode, "Festival validation requires a separate batch project, without -quit.");
        Require(new[] { "fast", "full", "festival", "standard" }.Contains(mode), "Unknown validation mode: " + mode);
        int shardCount = mode == "full" ? IntegerArgument("-festivalShardCount", 1) : 1;
        int shardIndex = mode == "full" ? IntegerArgument("-festivalShardIndex", 0) : 0;
        Require(shardCount >= 1 && shardCount <= 20 && shardIndex >= 0 && shardIndex < shardCount,
            "Festival shard requires count 1..20 and index 0..count-1.");
        SessionState.SetInt(ShardIndexKey, shardIndex);
        SessionState.SetInt(ShardCountKey, shardCount);
        SessionState.SetString(ModeKey, mode);
        SessionState.SetBool(ActiveKey, true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }
    private static bool Fast => SessionState.GetString(ModeKey, "fast") == "fast";
    private static int ShardIndex => SessionState.GetInt(ShardIndexKey, 0);
    private static int ShardCount => SessionState.GetInt(ShardCountKey, 1);
    private static RoundEvidence Current => evidence.rounds[evidence.rounds.Count - 1];
    private static object Read(object target, string name) => target.GetType().GetField(name, PrivateInstance).GetValue(target);
    private static void Write(object target, string name, object value) => target.GetType().GetField(name, PrivateInstance).SetValue(target, value);
    private static object Call(object target, string name, params object[] args) => target.GetType().GetMethod(name, PrivateInstance).Invoke(target, args);
    private static object Public(object target, string name) => target.GetType().GetProperty(name).GetValue(target);
    private static RhythmChartData Loaded(RhythmChartReader reader) => (RhythmChartData)Read(reader, "chart");

    private static int IntegerArgument(string name, int fallback)
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, name);
        if (index < 0) return fallback;
        Require(index + 1 < args.Length && int.TryParse(args[index + 1], out _), "Invalid integer argument: " + name);
        return int.Parse(args[index + 1]);
    }

    // Longest-processing-time assignment is deterministic; ties use catalog/group order.
    private static int[] DurationBalancedShard(int count, int shard)
    {
        var groups = Enumerable.Range(0, count).Select(_ => new List<int>()).ToArray();
        var totals = new double[count];
        foreach (int index in Enumerable.Range(0, catalog.songs.Length)
            .OrderByDescending(i => catalog.songs[i].roundDurationSeconds).ThenBy(i => i))
        {
            int destination = 0;
            for (int group = 1; group < count; group++)
                if (totals[group] < totals[destination]) destination = group;
            groups[destination].Add(index);
            totals[destination] += catalog.songs[index].roundDurationSeconds;
        }
        return groups[shard].OrderBy(i => i).ToArray();
    }

    private static void Tick()
    {
        if (!SessionState.GetBool(ActiveKey, false) || !EditorApplication.isPlaying) return;
        try
        {
            var controller = Object.FindFirstObjectByType<RoundController>();
            if (controller == null || Time.frameCount < 8) return;
            if (evidence == null) Initialize(controller);
            Require(EditorApplication.timeSinceStartup < deadline, "Festival runtime validation timed out.");
            Require(RuntimeErrors.Count == 0, "Unexpected Unity runtime error: " + string.Join(" | ", RuntimeErrors));
            var reader = Object.FindFirstObjectByType<RhythmChartReader>();
            if (stage == 0)
            {
                ValidateSelection(controller, reader);
                controller.CancelSongSelection();
                Require(!controller.IsChoosingSong && !RoundController.IsPlaying, "Back failed.");
                stage = 1; stageStarted = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 1)
            {
                if (EditorApplication.timeSinceStartup - stageStarted < .35) return;
                PrepareRound(controller, reader); return;
            }
            if (stage == 2)
            {
                Require(controller.SelectedSong.audioClip != null, "Selected clip missing.");
                if (controller.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                Require(controller.SelectedSong.audioClip.loadState == AudioDataLoadState.Loaded, "Selected audio failed to load.");
                controller.StartSelectedSong();
                Require(RoundController.IsPlaying, "Selected round failed to start.");
                stage = 3; stageStarted = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 3)
            {
                InspectPlaying(controller, reader);
                if (controller.SongTimeSeconds < 1.0) { Advance(controller); return; }
                Call(controller, "TogglePause");
                Require(RoundController.IsPaused, "Pause failed.");
                pausedTime = controller.SongTimeSeconds; pausedScore = Score.Value;
                stage = 4; stageStarted = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 4)
            {
                if (EditorApplication.timeSinceStartup - stageStarted < .3) return;
                Require(Math.Abs(controller.SongTimeSeconds - pausedTime) < .01, "Paused DSP song time advanced.");
                Require(Score.Value == pausedScore, "Score changed during pause.");
                controller.ResumeRound(); Require(!RoundController.IsPaused, "Resume failed.");
                Current.pauseResume = true; stage = 5; return;
            }
            if (stage == 5)
            {
                if (RoundController.IsPlaying)
                {
                    InspectPlaying(controller, reader); Advance(controller); return;
                }
                FinishRound(controller, reader);
                controller.RestartRound();
                stage = 6; stageStarted = EditorApplication.timeSinceStartup; return;
            }
            if (stage == 6)
            {
                if (EditorApplication.timeSinceStartup - stageStarted < .6) return;
                Require(!RoundController.IsPlaying && !RoundController.IsPaused, "Retry did not reset round state.");
                Require(Score.Value == 0 && Score.SuccessfulHits == 0 && Score.OnBeatHits == 0, "Retry retained round statistics.");
                Require(Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).Length == 0, "Retry retained balloons.");
                Current.retry = true; Save();
                round++;
                if (round == rounds.Length) { Complete(); return; }
                stage = 1; stageStarted = EditorApplication.timeSinceStartup;
            }
        }
        catch (Exception error) { Fail(error); }
    }

    private static void Initialize(RoundController controller)
    {
        originalLanguage = LanguageSettings.Current;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        foreach (InputDevice device in InputSystem.devices.Where(d => d is Keyboard || d is Mouse).ToArray())
            InputSystem.DisableDevice(device);
        catalog = JsonUtility.FromJson<Catalog>(File.ReadAllText(CatalogPath));
        Require(catalog?.songs != null && catalog.songs.Length == 20, "Expected twenty profiles in festival catalog.");
        string mode = SessionState.GetString(ModeKey, "fast");
        rounds = Enumerable.Range(0, 20).Where(i => mode == "full" || mode == "fast" || catalog.songs[i].mode == mode).ToArray();
        if (mode == "full" && ShardCount > 1) rounds = DurationBalancedShard(ShardCount, ShardIndex);
        Require(rounds.Length > 0, "No profiles selected for validation.");
        evidence = new Evidence
        {
            status = "running", mode = mode, controlledClockOffsets = Fast,
            startedUtc = DateTime.UtcNow.ToString("o"), unityVersion = Application.unityVersion,
            expectedProfiles = rounds.Length,
            shardIndex = ShardIndex, shardCount = ShardCount,
            assignedTrackIds = rounds.Select(i => catalog.songs[i].trackId).ToArray(),
            languageChecks = ShardCount == 1 ? "All three UI languages" : "Current language only; three-language cycle belongs to RunFast to avoid shared preference writes across shards",
            sourceRevision = Environment.GetEnvironmentVariable("BALLOON_SOURCE_REVISION") ?? "unrecorded"
        };
        runStarted = EditorApplication.timeSinceStartup;
        deadline = runStarted + (Fast ? 900 : rounds.Sum(i => catalog.songs[i].roundDurationSeconds) + 300);
        Application.logMessageReceived += OnLog;
        Directory.CreateDirectory("Logs");
        Debug.Log("FESTIVAL_VALIDATION_START: mode=" + mode + "; shard=" + ShardIndex + "of" + ShardCount + "; rounds=" + rounds.Length + "; natural=" + !Fast);
    }
    private static void ValidateSelection(RoundController controller, RhythmChartReader reader)
    {
        controller.BeginRound();
        var ids = new HashSet<string>();
        var keys = new HashSet<string>();
        for (int i = 0; i < catalog.songs.Length; i++)
        {
            Profile expected = catalog.songs[i];
            controller.SetSongMode(expected.mode); controller.SelectSong(i);
            var profile = controller.SelectedSong;
            Require(profile.trackId == expected.trackId && profile.mode == expected.mode, "Wrong profile selected at index " + i);
            Require(ids.Add(profile.trackId), "Track identity collision: " + profile.trackId);
            string key = (string)typeof(RoundController).GetMethod("GetBestScoreKey", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { profile.trackId });
            Require(keys.Add(key), "Best-score identity collision.");
            // Parallel natural shards have disjoint profile keys; never restore another shard's records.
            if (rounds.Contains(i))
                Preferences[key] = new SavedPreference { existed = PlayerPrefs.HasKey(key), value = PlayerPrefs.GetInt(key, 0) };
            Require(reader.IsReady, "Chart reader not ready: " + profile.title);
            Require(profile.audioClip != null && profile.audioClip.length + .12 >= profile.sourceCueOutSeconds, "Wrong/truncated clip: " + profile.title);
            Require(Math.Abs(profile.sourceCueOutSeconds - profile.sourceCueInSeconds - profile.roundDurationSeconds) < .002, "Cue/duration mismatch.");
            Require(profile.mode != "festival" || (profile.roundDurationSeconds >= 60 && profile.roundDurationSeconds <= 75.001), "Festival duration outside 60–75 seconds.");
            Require(profile.mode != "standard" || profile.sourceCueInSeconds == 0, "Standard profile does not start at recording origin.");
            Chart source = JsonUtility.FromJson<Chart>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, profile.chartRelativePath)));
            Event[] included = source.events.Where(e => e.id >= profile.sourceEventStartId && e.id < profile.sourceEventEndIdExclusive).ToArray();
            RhythmChartData loaded = Loaded(reader);
            Require(loaded.events.Length == included.Length && loaded.events.Length >= 5, "Event-range mismatch/empty chart: " + profile.title);
            Require(loaded.events.Select(e => e.id).Distinct().Count() == loaded.events.Length, "Duplicate loaded event ids.");
            Require(loaded.events.All(e => e.timeSeconds >= 0 && e.timeSeconds < profile.roundDurationSeconds), "Event outside selected round.");
            for (int e = 1; e < loaded.events.Length; e++)
                Require(loaded.events[e].timeSeconds > loaded.events[e - 1].timeSeconds, "Non-increasing event timeline.");
            Require(source.sections != null && source.sections.Length >= 3, "Missing authored sections.");
            double previousEnd = 0;
            foreach (Section section in source.sections)
            {
                Require(Math.Abs(section.startSeconds - previousEnd) < .002, "Section gap/overlap.");
                Require(section.endSeconds > section.startSeconds && section.pattern >= 0 && section.pattern <= 2 && Math.Abs(section.direction) == 1, "Invalid section.");
                previousEnd = section.endSeconds;
            }
            Require(Math.Abs(previousEnd - profile.roundDurationSeconds) < .002, "Sections do not cover the selected round.");
            Require(source.demonstrations != null && source.demonstrations.Length >= 1, "Missing demonstration metadata.");
            foreach (Demonstration demo in source.demonstrations)
            {
                Require(demo.slots != null && demo.slots.Length == 3 && demo.stepSeconds >= .2, "Unreadable demonstration motif.");
                Require(demo.startSeconds >= 0 && demo.answerStartSeconds >= demo.startSeconds + demo.stepSeconds * demo.slots.Length, "Demonstration overlaps answer.");
                for (int s = 0; s < demo.slots.Length; s++)
                {
                    double expectedTime = profile.sourceCueInSeconds + demo.answerStartSeconds + s * demo.stepSeconds;
                    Event answer = demo.answerEventIds != null && demo.answerEventIds.Length == demo.slots.Length
                        ? included.SingleOrDefault(e => e.id == demo.answerEventIds[s])
                        : included.SingleOrDefault(e => Math.Abs(e.timeSeconds - expectedTime) < .002);
                    int expectedSlot = demo.answerSlots != null && demo.answerSlots.Length == demo.slots.Length
                        ? demo.answerSlots[s] : demo.slots[s];
                    Require(answer != null && answer.authoredPlacement && answer.placementSlot == expectedSlot, "Missing/mismatched answer target.");
                    RhythmEventData loadedAnswer = loaded.events.SingleOrDefault(e => e.id == answer.id);
                    Require(loadedAnswer != null && Math.Abs(loadedAnswer.timeSeconds - (answer.timeSeconds - profile.sourceCueInSeconds)) < .002,
                        "Answer event identity/time changed while loading.");
                }
            }
            Dropdown list = GameObject.Find("SongList").GetComponent<Dropdown>();
            Require(list.options.Count == 10, "Expected ten songs in each mode, not a mixed twenty-song list.");
            evidence.selectedProfiles++;
            Debug.Log("FESTIVAL_PROFILE_OK: " + profile.trackId + "; mode=" + profile.mode + "; seconds=" + profile.roundDurationSeconds + "; events=" + loaded.events.Length);
        }
        UILanguage[] languages = ShardCount == 1
            ? Enum.GetValues(typeof(UILanguage)).Cast<UILanguage>().ToArray()
            : new[] { originalLanguage };
        foreach (UILanguage language in languages)
        {
            LanguageSettings.SetLanguage(language);
            foreach (string key in new[] { "cue.demonstration.watch", "cue.demonstration.respond", "song.mode.festival", "song.mode.standard" })
                Require(LocalizationManager.GetText(key) != key, "Missing localized festival key: " + key + "/" + language);
        }
        LanguageSettings.SetLanguage(originalLanguage);
        controller.SetSongMode("festival"); controller.SelectSong(0);
        Capture("FestivalSelection"); Save();
    }
    private static void PrepareRound(RoundController controller, RhythmChartReader reader)
    {
        Profile profile = catalog.songs[rounds[round]];
        controller.BeginRound(); controller.SetSongMode(profile.mode); controller.SelectSong(rounds[round]);
        Require(reader.IsReady, "Round chart failed to load.");
        evidence.rounds.Add(new RoundEvidence
        {
            trackId = profile.trackId, title = profile.title, mode = profile.mode,
            durationSeconds = profile.roundDurationSeconds, chartEvents = Loaded(reader).events.Length,
            settlement = Fast ? "controlled-clock-offset; not natural playback" : "natural DSP playback"
        });
        SpawnedIds.Clear(); wasDemonstrating = false; demoCaptured = false; answerCaptured = false;
        advancedTime = 0; lastAdvancedFrame = -1; stage = 2;
        stageStarted = EditorApplication.timeSinceStartup;
        roundStarted = stageStarted;
    }
    private static void InspectPlaying(RoundController controller, RhythmChartReader reader)
    {
        Current.lastPlayingSeconds = controller.SongTimeSeconds;
        Balloon[] balloons = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).Where(b => b.gameObject.activeInHierarchy).ToArray();
        Current.peakActiveTargets = Math.Max(Current.peakActiveTargets, balloons.Length);
        var spawner = Object.FindFirstObjectByType<BalloonSpawner>();
        int capacity = (int)typeof(BalloonSpawner).GetField("maxBalloons", PrivateInstance).GetValue(spawner);
        Require(balloons.Length <= capacity, "Active balloon cap exceeded.");
        foreach (Balloon balloon in balloons)
        {
            int instance = balloon.GetInstanceID();
            if (SpawnedIds.TryGetValue(balloon.EventId, out int prior)) Require(prior == instance, "Scoring event spawned twice: " + balloon.EventId);
            else
            {
                RhythmEventData chartEvent = Loaded(reader).events.First(e => e.id == balloon.EventId);
                Require(controller.SongTimeSeconds - reader.GetSpawnTime(chartEvent) < .5d,
                    "Target admission was delayed by congestion: " + balloon.EventId);
                SpawnedIds[balloon.EventId] = instance;
            }
        }
        Current.observedDistinctTargets = SpawnedIds.Count;
        Component cue = controller.GetComponents<Component>().FirstOrDefault(c => c != null && c.GetType().Name == "FestivalDemonstrationCue");
        if (cue == null)
            cue = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).FirstOrDefault(c => c.GetType().Name == "FestivalDemonstrationCue");
        Require(cue != null, "Demonstration cue component missing.");
        bool demonstrating = (bool)Public(cue, "IsDemonstrating");
        bool responding = (bool)Public(cue, "IsResponding");
        if (demonstrating)
        {
            Current.demonstrationSeen = true;
            Require((int)Public(cue, "VisibleMarkerCount") > 0 && (int)Public(cue, "VisibleMarkerCount") <= 3, "Unexpected marker count.");
            GameObject overlay = GameObject.Find("FestivalDemonstrationOverlay");
            Require(overlay != null, "Demonstration overlay missing while active.");
            Require(overlay.GetComponentsInChildren<Collider>(true).Length == 0, "Demonstration acquired a gameplay collider.");
            Require(overlay.GetComponentsInChildren<Graphic>(true).All(g => !g.raycastTarget), "Demonstration graphic blocks pointer input.");
            CanvasGroup group = overlay.GetComponent<CanvasGroup>();
            Require(group != null && !group.blocksRaycasts && !group.interactable, "Demonstration blocks input.");
            if (wasDemonstrating) Require(Score.Value == demonstrationScore, "Demonstration changed score without a hit.");
            demonstrationScore = Score.Value;
            if (!demoCaptured && round < 2) { Capture("FestivalWatch-" + round); demoCaptured = true; }
        }
        if (responding)
        {
            Current.responseSeen = true;
            if (!answerCaptured && round < 2) { Capture("FestivalAnswer-" + round); answerCaptured = true; }
        }
        wasDemonstrating = demonstrating;
        if (!demonstrating && Score.SuccessfulHits < 5) SampleScoring(controller, balloons);
        Require(Score.OnBeatHits <= Score.SuccessfulHits && Score.Value == 10 * Score.SuccessfulHits + 10 * Score.OnBeatHits, "Score/statistics invariant broken.");
        Current.score = Score.Value; Current.successfulHits = Score.SuccessfulHits; Current.onBeatHits = Score.OnBeatHits;
    }
    private static void SampleScoring(RoundController controller, Balloon[] balloons)
    {
        PlayerShooter shooter = Object.FindFirstObjectByType<PlayerShooter>();
        Require(shooter != null, "Missing shooter.");
        Transform muzzle = (Transform)Read(shooter, "muzzle");
        bool wantBonus = Score.OnBeatHits < 2;
        foreach (Balloon balloon in balloons.OrderBy(b => b.EventTimeSeconds))
        {
            double time = controller.SongTimeSeconds;
            if (balloon.IsOnBeat(time) != wantBonus) continue;
            Physics.SyncTransforms();
            Collider[] candidates = balloon.GetComponentsInChildren<Collider>();
            if (!colliderSelectionLogged)
            {
                colliderSelectionLogged = true;
                Debug.Log("FESTIVAL_COLLIDER_SELECTION: " + string.Join(" | ", candidates.Select(c =>
                    Hierarchy(c.transform) + "; " + c.GetType().Name + "; enabled=" + c.enabled +
                    "; trigger=" + c.isTrigger + "; active=" + c.gameObject.activeInHierarchy)));
            }
            // The prefab deliberately keeps legacy disabled spheres beside the live capsule.
            // Aim at an actual participating collider; never re-enable legacy geometry.
            Collider collider = candidates.Where(c => c.enabled && !c.isTrigger && c.gameObject.activeInHierarchy &&
                (Physics.DefaultRaycastLayers & (1 << c.gameObject.layer)) != 0)
                .OrderByDescending(c => c.bounds.size.sqrMagnitude).FirstOrDefault();
            Require(collider != null, "Scoring balloon has no enabled non-trigger collider.");
            Vector3 direction = (collider.bounds.center - muzzle.position).normalized;
            object[] args = { direction, default(RaycastHit) };
            bool hit = (bool)typeof(PlayerShooter).GetMethod("TryGetSprayHit", PrivateInstance).Invoke(shooter, args);
            if (!hit)
            {
                DiagnoseCast(shooter, muzzle, collider, balloon, time, direction, false, default);
                continue;
            }
            RaycastHit contact = (RaycastHit)args[1];
            Balloon actual = contact.collider.GetComponentInParent<Balloon>();
            if (actual != balloon)
            {
                DiagnoseCast(shooter, muzzle, collider, balloon, time, direction, true, contact);
                continue;
            }
            int before = Score.Value;
            int bonus = actual.IsOnBeat(time) ? 10 : 0;
            actual.HitAt(contact.point, bonus);
            Require(Score.Value == before + 10 + bonus, "Accepted sampled cast did not score once.");
            actual.HitAt(contact.point, bonus);
            Require(Score.Value == before + 10 + bonus, "Duplicate accepted hit scored twice.");
            Require(actual.GetComponentsInChildren<Collider>().All(c => !c.enabled), "Accepted pop retained collider.");
            if (Score.SuccessfulHits == 5)
            {
                Require(Score.OnBeatHits == 2 && Score.Value == 70, "Five-hit/two-on-beat sample should equal 70 points.");
                Current.sampleScoring70 = true;
            }
            break;
        }
    }
    private static void DiagnoseCast(PlayerShooter shooter, Transform muzzle, Collider expected,
        Balloon balloon, double songTime, Vector3 direction, bool hit, RaycastHit contact)
    {
        if (castDiagnostics++ >= 8) return;
        float radius = (float)Read(shooter, "sprayRadius");
        float range = (float)Read(shooter, "sprayRange");
        string actual = hit
            ? Hierarchy(contact.collider.transform) + "; distance=" + contact.distance.ToString("F3") + "; point=" + contact.point.ToString("F3")
            : "none";
        RaycastHit[] all = Physics.SphereCastAll(muzzle.position, radius, direction, range,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore).OrderBy(h => h.distance).Take(6).ToArray();
        Debug.Log("FESTIVAL_CAST_DIAGNOSTIC: event=" + balloon.EventId + "; song=" + songTime.ToString("F6") +
            "; beat=" + balloon.EventTimeSeconds.ToString("F6") + "; onBeat=" + balloon.IsOnBeat(songTime) +
            "; muzzle=" + muzzle.position.ToString("F3") + "; shooter=" + shooter.transform.position.ToString("F3") +
            "; forward=" + shooter.transform.forward.ToString("F3") + "; camera=" + Camera.main.transform.position.ToString("F3") +
            "; target=" + Hierarchy(expected.transform) + "; targetCenter=" + expected.bounds.center.ToString("F3") +
            "; targetSize=" + expected.bounds.size.ToString("F3") + "; trigger=" + expected.isTrigger +
            "; enabled=" + expected.enabled + "; radius=" + radius + "; range=" + range +
            "; targetDistance=" + Vector3.Distance(muzzle.position, expected.bounds.center).ToString("F3") +
            "; actual=" + actual + "; all=" + string.Join(" | ", all.Select(h => Hierarchy(h.collider.transform) + "@" + h.distance.ToString("F3"))));
    }
    private static string Hierarchy(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null) { transform = transform.parent; path = transform.name + "/" + path; }
        return path;
    }
    private static void Advance(RoundController controller)
    {
        if (!Fast || lastAdvancedFrame == Time.frameCount) return;
        lastAdvancedFrame = Time.frameCount;
        double increment = .12;
        // Land on the next on-beat sample while testing the first two accepted hits.
        if (Score.OnBeatHits < 2)
        {
            Balloon next = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None)
                .Where(b => b.EventTimeSeconds > advancedTime).OrderBy(b => b.EventTimeSeconds).FirstOrDefault();
            if (next != null && next.EventTimeSeconds - advancedTime < increment)
                increment = Math.Max(.001, next.EventTimeSeconds - advancedTime);
        }
        advancedTime = Math.Max(advancedTime, controller.SongTimeSeconds) + increment;
        double paused = (double)Read(controller, "totalPausedDspSeconds");
        Write(controller, "songStartDspTime", AudioSettings.dspTime - paused - advancedTime);
    }
    private static void FinishRound(RoundController controller, RhythmChartReader reader)
    {
        Require(Current.lastPlayingSeconds >= Current.durationSeconds - (Fast ? .5 : .3), "Round settled too early.");
        Require((int)Read(reader, "nextEventIndex") == Loaded(reader).events.Length, "Not all chart events reached the spawner.");
        Require(Current.observedDistinctTargets == Current.chartEvents, "Some chart targets were never observed.");
        Require(Current.sampleScoring70, "Did not complete the sampled 70-point physics/pop check.");
        Require(Current.demonstrationSeen && Current.responseSeen, "Demonstration or response never appeared.");
        Current.wallSeconds = EditorApplication.timeSinceStartup - roundStarted;
        GameObject results = (GameObject)Read(controller, "resultsScreen");
        Require(results != null && results.activeInHierarchy, "Natural/controller settlement did not show results.");
        Current.results = true;
        if (round == 0) Capture("FestivalResults");
        Debug.Log("FESTIVAL_ROUND_OK: " + Current.trackId + "; " + Current.settlement + "; last=" + Current.lastPlayingSeconds + "; targets=" + Current.observedDistinctTargets + "; peak=" + Current.peakActiveTargets + "; score=" + Current.score);
        Save();
    }
    private static void Capture(string name)
    {
        // Existing capture helper temporarily converts overlay UI and restores it.
        typeof(KyudaisaiRuntimeSmokeCheck).GetMethod("Capture", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { name });
    }
    private static void OnLog(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            RuntimeErrors.Add(message);
    }
    private static void RestorePreferences()
    {
        foreach (var entry in Preferences)
        {
            if (entry.Value.existed) PlayerPrefs.SetInt(entry.Key, entry.Value.value);
            else PlayerPrefs.DeleteKey(entry.Key);
        }
        LanguageSettings.SetLanguage(originalLanguage);
        PlayerPrefs.Save();
    }
    private static void Save()
    {
        if (evidence == null) return;
        evidence.wallSeconds = EditorApplication.timeSinceStartup - runStarted;
        string suffix = evidence.shardCount > 1 ? "-shard" + evidence.shardIndex + "of" + evidence.shardCount : "";
        File.WriteAllText("Logs/FestivalRuntime-" + evidence.mode + suffix + ".json", JsonUtility.ToJson(evidence, true));
    }
    private static void Complete()
    {
        evidence.status = "passed"; evidence.finishedUtc = DateTime.UtcNow.ToString("o");
        RestorePreferences(); Save();
        SessionState.SetBool(ActiveKey, false); Application.logMessageReceived -= OnLog;
        Debug.Log("FESTIVAL_RUNTIME_OK: " + evidence.mode + "; " + rounds.Length + " completed rounds; controlledClockOffsets=" + Fast);
        EditorApplication.Exit(0);
    }
    private static void Fail(Exception error)
    {
        if (evidence != null)
        {
            evidence.status = "failed"; evidence.finishedUtc = DateTime.UtcNow.ToString("o");
            evidence.errors.Add(error.ToString()); evidence.errors.AddRange(RuntimeErrors);
            RestorePreferences(); Save();
        }
        SessionState.SetBool(ActiveKey, false); Application.logMessageReceived -= OnLog;
        Debug.LogException(error); EditorApplication.Exit(1);
    }
    private static void Require(bool condition, string reason)
    {
        if (!condition) throw new InvalidOperationException(reason);
    }
}
#endif
