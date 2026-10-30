// Copy into Assets/Editor in an isolated project, then run -batchmode
// -executeMethod BalloonReliabilityValidation.Run without -quit.
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BalloonBarrage.EditorTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class BalloonReliabilityValidation
{
    private const string Key = "BalloonReliabilityValidation.Active";
    private const string Production = "Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab";
    private const string Lesson = "Assets/StudioArtPack/BalloonColors/Prefabs/LessonBalloon_ThreeColors.prefab";
    private const string Report = "Logs/balloon-reliability-validation.txt";
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    private static double started;

    static BalloonReliabilityValidation()
    {
        if (SessionState.GetBool(Key, false)) EditorApplication.update += Tick;
    }

    public static void Run()
    {
        Require(Application.isBatchMode, "Use an isolated batch project.");
        Directory.CreateDirectory("Logs");
        File.WriteAllText(Report, "Automated Editor checks; no human aiming/listening or desktop package acceptance.\n");
        try
        {
            VerifyPreparation();
            EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
            SessionState.SetBool(Key, true);
            EditorApplication.EnterPlaymode();
        }
        catch (Exception error) { Finish(1, "FAIL: " + error); }
    }

    private static void VerifyPreparation()
    {
        string[] paths = { Production, Production + ".meta", "Assets/Scenes/PrototypeScene.unity" };
        var before = paths.ToDictionary(path => path, File.ReadAllBytes);
        if (File.Exists(Lesson)) BalloonColorLessonPreparation.Build();
        else BalloonColorLessonPreparation.VerifyBatch();
        Require(File.Exists(Lesson), "Lesson asset was not created separately.");
        byte[] originalLesson = File.ReadAllBytes(Lesson);
        var root = PrefabUtility.LoadPrefabContents(Lesson);
        try
        {
            var data = new SerializedObject(root.GetComponent<Balloon>());
            data.FindProperty("cleanupDelaySeconds").doubleValue = 9.25d;
            data.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, Lesson);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
        byte[] editedLesson = File.ReadAllBytes(Lesson);
        byte[] lessonMeta = File.ReadAllBytes(Lesson + ".meta");
        BalloonColorLessonPreparation.Build();
        Require(editedLesson.SequenceEqual(File.ReadAllBytes(Lesson)), "Repeated preparation overwrote a lesson edit.");
        Require(lessonMeta.SequenceEqual(File.ReadAllBytes(Lesson + ".meta")), "Lesson GUID changed.");
        foreach (string path in paths)
            Require(before[path].SequenceEqual(File.ReadAllBytes(path)), "Preparation modified production: " + path);
        File.WriteAllBytes(Lesson, originalLesson);
        AssetDatabase.ImportAsset(Lesson);
        Record("PASS: preparation creates a separate lesson; production prefab/GUID/scene bytes preserved; repeated invocation preserves lesson edits/GUID.");
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (started == 0d) { started = EditorApplication.timeSinceStartup; return; }
            double elapsed = EditorApplication.timeSinceStartup - started;
            Require(elapsed < 30d, "Runtime validation timed out.");
            var controller = Object.FindFirstObjectByType<RoundController>();
            if (elapsed < .7 || controller == null) return;
            var reader = Object.FindFirstObjectByType<RhythmChartReader>();
            Require(reader.IsReady, "Production chart failed to load.");
            controller.StartSelectedSong();
            Require(RoundController.IsPlaying, "Round failed to start.");
            var spawner = Object.FindFirstObjectByType<BalloonSpawner>();
            reader.enabled = false;
            VerifySpacing(spawner);
            VerifyPendingEvent(reader, spawner);
            VerifyCue(reader, spawner);
            Finish(0, "PASS: all balloon reliability checks.");
        }
        catch (Exception error) { Finish(1, "FAIL: " + error); }
    }

    private static RhythmEventData Event(int id, double time = 100d) =>
        new RhythmEventData { id = id, timeSeconds = time, authoredPlacement = true, placementSlot = 0 };

    private static void VerifySpacing(BalloonSpawner spawner)
    {
        ClearBalloons();
        int capacity = (int)Read(spawner, "maxBalloons");
        for (int index = 0; index < capacity; index++)
            Require(spawner.Spawn(Event(10000 + index)), "Could not admit the configured capacity from one crowded column.");
        var balloons = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None);
        Require(balloons.Length == capacity && !spawner.Spawn(Event(10000 + capacity)), "Capacity check failed.");
        var motion = balloons[0].GetComponent<BalloonFloat>();
        var body = balloons[0].GetComponent<Rigidbody>();
        var original = (RigidbodyConstraints)Read(motion, "originalConstraints");
        motion.enabled = false;
        Require(body.constraints == original, "Disabling the hold leaked frozen constraints.");
        motion.enabled = true;
        Require((body.constraints & RigidbodyConstraints.FreezePosition) == RigidbodyConstraints.FreezePosition,
            "Re-enabling the waiting target failed to restore its hold.");
        Vector2 xRange = (Vector2)Read(spawner, "xRange"), yRange = (Vector2)Read(spawner, "yRange");
        Physics.SyncTransforms();
        for (int i = 0; i < balloons.Length; i++)
        {
            Vector3 position = balloons[i].transform.position;
            Require(position.x >= xRange.x && position.x <= xRange.y && position.y >= yRange.x && position.y <= yRange.y,
                "Fallback position left the spawn area.");
            Require(balloons[i].EventTimeSeconds == 100d, "Placement changed event judgement time.");
            for (int j = i + 1; j < balloons.Length; j++)
            {
                Require(Vector3.Distance(position, balloons[j].transform.position) >= .7499f, "Fallback overlapped target centers.");
                foreach (Collider a in balloons[i].GetComponentsInChildren<Collider>().Where(c => c.enabled))
                foreach (Collider b in balloons[j].GetComponentsInChildren<Collider>().Where(c => c.enabled))
                    Require(!Physics.ComputePenetration(a, a.transform.position, a.transform.rotation,
                        b, b.transform.position, b.transform.rotation, out _, out _), "Target colliders overlap at spawn.");
            }
        }
        Record("PASS: " + capacity + " repeated-column targets admitted inside bounds, separated centers/colliders; next target blocked; original event times retained.");
        ClearBalloons();
    }

    private static void VerifyPendingEvent(RhythmChartReader reader, BalloonSpawner spawner)
    {
        Vector2 xRange = (Vector2)Read(spawner, "xRange"), yRange = (Vector2)Read(spawner, "yRange");
        Write(spawner, "xRange", Vector2.zero); Write(spawner, "yRange", Vector2.zero);
        Require(spawner.Spawn(Event(20000)), "Initial occupied target failed.");
        var pending = Event(20001, 0d);
        Write(reader, "chart", new RhythmChartData { events = new[] { pending } });
        Write(reader, "nextEventIndex", 0); Write(reader, "nextCueEventIndex", 0);
        Call(reader, "Update");
        Require((int)Read(reader, "nextEventIndex") == 0, "Blocked placement consumed a pending event.");
        Require(Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).Length == 1, "Blocked area produced an overlap.");
        ClearBalloons();
        Call(reader, "Update");
        var admitted = Object.FindFirstObjectByType<Balloon>();
        Require((int)Read(reader, "nextEventIndex") == 1 && admitted.EventId == pending.id && admitted.EventTimeSeconds == 0d,
            "Freed position did not admit the original pending event exactly once.");
        Write(spawner, "xRange", xRange); Write(spawner, "yRange", yRange);
        ClearBalloons(); Call(reader, "LoadSelectedChart");
        Record("PASS: physically saturated area blocks without overlap or event loss; freeing a position admits the same event/time once.");
    }

    private static void VerifyCue(RhythmChartReader reader, BalloonSpawner spawner)
    {
        reader.enabled = true;
        var events = reader.LoadedChart.events;
        Require(events.Length >= 4, "Cue test requires four events.");
        for (int i = 0; i < 3; i++) Require(spawner.Spawn(events[i]), "Cue target failed to spawn.");
        var balloons = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).OrderBy(b => b.EventTimeSeconds).ToArray();
        Write(reader, "currentSongTime", events[0].timeSeconds - .1d);
        Call(reader, "UpdateBeatCue");
        Require((int)Read(reader, "nextCueEventIndex") == 0, "Initial cue is wrong.");
        balloons[1].Hit(); Call(reader, "UpdateBeatCue");
        Require((int)Read(reader, "nextCueEventIndex") == 0, "Out-of-order hit skipped an unresolved earlier target.");
        balloons[0].Hit(); int score = Score.Value;
        balloons[0].Hit(); Call(reader, "UpdateBeatCue");
        Require(Score.Value == score && (int)Read(reader, "nextCueEventIndex") == 2,
            "Early/duplicate hits did not skip exactly the resolved events.");
        var cueText = (UnityEngine.UI.Text)Read(reader, "beatCueText");
        string remaining = (Mathf.RoundToInt((float)((events[2].timeSeconds - (double)Read(reader, "currentSongTime")) * 10d)) / 10f)
            .ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        Require(cueText.text == LocalizationManager.Format("cue.next", remaining), "Displayed countdown still refers to a popped target.");
        double retention = new SerializedObject(balloons[2]).FindProperty("cleanupDelaySeconds").doubleValue;
        balloons[2].CheckExpiry(events[2].timeSeconds + retention + .01d);
        Call(reader, "UpdateBeatCue");
        Require((int)Read(reader, "nextCueEventIndex") == 3, "Expiry did not resolve its cue.");
        Require(Score.Value == score, "Expiry changed the score.");
        Call(reader, "LoadSelectedChart");
        Require(((HashSet<int>)Read(reader, "resolvedEventIds")).Count == 0, "Chart reload retained resolved ids.");
        reader.enabled = false;
        ClearBalloons();
        Record("PASS: early/out-of-order hits, duplicate guard, displayed NEXT time, expiry and chart reload keep cue resolution synchronized.");
    }

    private static void ClearBalloons()
    {
        foreach (Balloon balloon in Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None)) Object.DestroyImmediate(balloon.gameObject);
    }
    private static object Read(object target, string name) => target.GetType().GetField(name, Private).GetValue(target);
    private static void Write(object target, string name, object value) => target.GetType().GetField(name, Private).SetValue(target, value);
    private static void Call(object target, string name) => target.GetType().GetMethod(name, Private).Invoke(target, null);
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Record(string message) { File.AppendAllText(Report, message + "\n"); Debug.Log(message); }
    private static void Finish(int code, string message)
    {
        Record(message); SessionState.SetBool(Key, false); EditorApplication.update -= Tick; EditorApplication.Exit(code);
    }
}
#endif
