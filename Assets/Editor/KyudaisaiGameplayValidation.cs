#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class KyudaisaiGameplayValidation
{
    [MenuItem("Balloon Barrage/Kyudaisai/Validate Gameplay Setup")]
    public static void Run()
    {
        try
        {
            ValidateSceneProfiles();
            ValidatePopEffects();
            ValidateContactHandoff();
            ValidateLearnerExercise();
            Debug.Log("KYUDAISAI_GAMEPLAY_VALIDATION_OK");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }

            throw;
        }

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(0);
        }
    }

    private static void ValidateSceneProfiles()
    {
        EditorSceneManager.OpenScene(
            "Assets/Scenes/PrototypeScene.unity",
            OpenSceneMode.Single);

        RoundController controller =
            UnityEngine.Object.FindFirstObjectByType<RoundController>();
        Require(controller != null, "RoundController is missing.");

        SerializedObject serialized = new SerializedObject(controller);
        SerializedProperty songs = serialized.FindProperty("exhibitionSongs");
        Require(songs.arraySize == 3, "Three songs must be configured.");

        for (int index = 0; index < songs.arraySize; index++)
        {
            SerializedProperty song = songs.GetArrayElementAtIndex(index);
            string chartPath = song.FindPropertyRelative("chartRelativePath")
                .stringValue;
            double cueIn = song.FindPropertyRelative("sourceCueInSeconds")
                .doubleValue;
            double cueOut = song.FindPropertyRelative("sourceCueOutSeconds")
                .doubleValue;
            double duration = song.FindPropertyRelative("roundDurationSeconds")
                .doubleValue;
            int firstId = song.FindPropertyRelative("sourceEventStartId")
                .intValue;
            int endId = song.FindPropertyRelative("sourceEventEndIdExclusive")
                .intValue;

            Require(
                song.FindPropertyRelative("audioClip").objectReferenceValue != null,
                $"Song {index} has no audio clip.");
            Require(duration >= 75d && duration <= 90d,
                $"Song {index} duration is outside the short-round target.");
            Require(Math.Abs((cueOut - cueIn) - duration) < 0.00001d,
                $"Song {index} cue window and duration disagree.");

            string absoluteChartPath = Path.Combine(
                Application.streamingAssetsPath,
                chartPath);
            Require(File.Exists(absoluteChartPath),
                $"Song {index} chart is missing: {chartPath}");

            RhythmChartData chart = JsonUtility.FromJson<RhythmChartData>(
                File.ReadAllText(absoluteChartPath));
            Require(chart != null && chart.events != null,
                $"Song {index} chart could not be parsed.");

            int selectedCount = 0;
            double firstTime = double.MaxValue;
            double lastTime = double.MinValue;

            foreach (RhythmEventData chartEvent in chart.events)
            {
                if (chartEvent.id < firstId || chartEvent.id >= endId)
                {
                    continue;
                }

                selectedCount++;
                firstTime = Math.Min(firstTime, chartEvent.timeSeconds);
                lastTime = Math.Max(lastTime, chartEvent.timeSeconds);
            }

            Require(selectedCount == endId - firstId,
                $"Song {index} selected event ids are not contiguous.");
            Require(Math.Abs(firstTime - cueIn) < 0.00001d,
                $"Song {index} first event does not start at cue-in.");
            Require(lastTime < cueOut,
                $"Song {index} last selected event reaches beyond cue-out.");
        }
    }

    private static void ValidatePopEffects()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(
            "Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab");

        try
        {
            Balloon balloon = root.GetComponent<Balloon>();
            Require(balloon != null, "Production balloon component is missing.");

            SerializedObject serialized = new SerializedObject(balloon);
            SerializedProperty variants =
                serialized.FindProperty("popEffectVariants");
            Require(variants.arraySize == 8,
                "Production balloon must reference eight pop variants.");

            for (int index = 0; index < variants.arraySize; index++)
            {
                ParticleSystem effect = variants.GetArrayElementAtIndex(index)
                    .objectReferenceValue as ParticleSystem;
                Require(effect != null, $"Pop variant {index} is missing.");
                Require(effect.main.simulationSpace ==
                    ParticleSystemSimulationSpace.World,
                    $"Pop variant {index} must use world simulation.");
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ValidateContactHandoff()
    {
        Require(
            typeof(Balloon).GetMethod("HitAt") != null,
            "Balloon.HitAt is missing.");

        string shooter = File.ReadAllText("Assets/Scripts/PlayerShooter.cs");
        string projectile = File.ReadAllText("Assets/Scripts/PaintProjectile.cs");
        Require(shooter.Contains("sprayedBalloon.HitAt("),
            "PlayerShooter does not pass the sampled hit point.");
        Require(projectile.Contains("balloon.HitAt(hit.point"),
            "PaintProjectile does not pass the sampled hit point.");
    }

    private static void ValidateLearnerExercise()
    {
        Require(KyudaisaiLearnerCore.SelectBarragePattern(0d) == BarragePattern.LateralSweep &&
            KyudaisaiLearnerCore.SelectBarragePattern(0.5d) == BarragePattern.ShallowFan &&
            KyudaisaiLearnerCore.SelectBarragePattern(0.9d) == BarragePattern.StaggeredCluster,
            "The saved learner barrage phases are incorrect.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
#endif
