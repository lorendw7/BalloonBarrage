#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class KyudaisaiGameplayInstaller
{
    private const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    private const string BalloonPrefabPath =
        "Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab";

    private static readonly string[] PopEffectPaths =
    {
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Teal.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Coral.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Gold.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_SkyBlue.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Violet.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Rose.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Lime.prefab",
        "Assets/StudioArtPack/BalloonPopV2/BalloonPop_Tangerine.prefab"
    };

    [MenuItem("Balloon Barrage/Kyudaisai/Apply Gameplay Setup")]
    public static void Apply()
    {
        try
        {
            AssetDatabase.Refresh();
            ConfigureBalloonPrefab();
            ConfigurePrototypeScene();
            AssetDatabase.SaveAssets();
            Debug.Log("KYUDAISAI_GAMEPLAY_SETUP_OK");
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

    private static void ConfigureBalloonPrefab()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(
            BalloonPrefabPath);

        try
        {
            Balloon balloon = root.GetComponent<Balloon>();

            if (balloon == null)
            {
                throw new InvalidOperationException(
                    "The production balloon prefab has no Balloon component.");
            }

            SerializedObject serialized = new SerializedObject(balloon);
            serialized.FindProperty("cleanupDelaySeconds").doubleValue = 5d;
            SerializedProperty variants =
                serialized.FindProperty("popEffectVariants");
            variants.arraySize = PopEffectPaths.Length;

            for (int index = 0; index < PopEffectPaths.Length; index++)
            {
                GameObject effectObject =
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        PopEffectPaths[index]);

                if (effectObject == null)
                {
                    throw new InvalidOperationException(
                        "Missing pop effect: " + PopEffectPaths[index]);
                }

                ParticleSystem effect =
                    effectObject.GetComponent<ParticleSystem>();

                if (effect == null)
                {
                    throw new InvalidOperationException(
                        "Pop effect root has no ParticleSystem: " +
                        PopEffectPaths[index]);
                }

                variants.GetArrayElementAtIndex(index)
                    .objectReferenceValue = effect;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, BalloonPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void ConfigurePrototypeScene()
    {
        Scene scene = EditorSceneManager.OpenScene(
            ScenePath,
            OpenSceneMode.Single);

        RoundController controller =
            UnityEngine.Object.FindFirstObjectByType<RoundController>();

        if (controller == null)
        {
            throw new InvalidOperationException(
                "PrototypeScene has no RoundController.");
        }

        SerializedObject serialized = new SerializedObject(controller);
        SerializedProperty songs = serialized.FindProperty("exhibitionSongs");
        songs.arraySize = 3;

        SetSong(
            songs.GetArrayElementAtIndex(0),
            "stepmania-goin-under-beginner",
            "Goin' Under",
            "NegaRen",
            "Assets/Audio/Music/StepMania/GoinUnder.ogg",
            "Rhythm/Prepared/GoinUnder_Beginner.json",
            9.142857143d,
            91.428571429d,
            82.285714286d,
            0,
            68);

        SetSong(
            songs.GetArrayElementAtIndex(1),
            "stepmania-springtime-beginner",
            "Springtime",
            "Kommisar",
            "Assets/Audio/Music/StepMania/Springtime.mp3",
            "Rhythm/Prepared/Springtime_Beginner.json",
            5.373870435d,
            89.915797397d,
            84.541926962d,
            0,
            147);

        SetSong(songs.GetArrayElementAtIndex(2),
            "stepmania-mecha-tribe-beginner", "Mecha-Tribe Assault", "Kommisar",
            "Assets/Audio/Music/StepMania/MechaTribeAssault.ogg",
            "Rhythm/Prepared/MechaTribeAssault_Beginner.json",
            5.361333333d, 85.361333333d, 80d, 0, 137);

        serialized.ApplyModifiedPropertiesWithoutUndo();

        BalloonSpawner spawner =
            UnityEngine.Object.FindFirstObjectByType<BalloonSpawner>();

        if (spawner != null)
        {
            SerializedObject spawnerSerialized = new SerializedObject(spawner);
            spawnerSerialized.FindProperty("maxBalloons").intValue = 16;
            spawnerSerialized.FindProperty("xRange").vector2Value = new Vector2(-3.3f, 3.3f);
            spawnerSerialized.FindProperty("yRange").vector2Value = new Vector2(-2f, 2f);
            spawnerSerialized.ApplyModifiedPropertiesWithoutUndo();
        }

        RhythmChartReader reader = UnityEngine.Object.FindFirstObjectByType<RhythmChartReader>();
        SerializedObject readerSerialized = new SerializedObject(reader);
        readerSerialized.FindProperty("spawnLeadSeconds").doubleValue = 2.8d;
        readerSerialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void SetSong(
        SerializedProperty song,
        string trackId,
        string title,
        string artist,
        string audioPath,
        string chartPath,
        double cueIn,
        double cueOut,
        double duration,
        int firstEventId,
        int endEventIdExclusive)
    {
        AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);

        if (clip == null)
        {
            throw new InvalidOperationException(
                "Missing exhibition audio: " + audioPath);
        }

        song.FindPropertyRelative("trackId").stringValue = trackId;
        song.FindPropertyRelative("title").stringValue = title;
        song.FindPropertyRelative("artist").stringValue = artist;
        song.FindPropertyRelative("audioClip").objectReferenceValue = clip;
        song.FindPropertyRelative("chartRelativePath").stringValue = chartPath;
        song.FindPropertyRelative("sourceCueInSeconds").doubleValue = cueIn;
        song.FindPropertyRelative("sourceCueOutSeconds").doubleValue = cueOut;
        song.FindPropertyRelative("roundDurationSeconds").doubleValue = duration;
        song.FindPropertyRelative("sourceEventStartId").intValue = firstEventId;
        song.FindPropertyRelative("sourceEventEndIdExclusive").intValue =
            endEventIdExclusive;
    }
}
#endif
