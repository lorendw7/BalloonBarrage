#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Explicit, repeatable promotion of the prepared festival catalog.</summary>
public static class FestivalExperienceInstaller
{
    [Serializable] private sealed class Catalog { public Profile[] songs; }
    [Serializable] private sealed class Profile
    {
        public string trackId, title, artist, mode, audioAssetPath, chartRelativePath;
        public double sourceCueInSeconds, sourceCueOutSeconds, roundDurationSeconds;
        public int sourceEventStartId, sourceEventEndIdExclusive;
    }

    [MenuItem("Balloon Barrage/Kyudaisai/Apply Festival Experience")]
    public static void Apply()
    {
        try
        {
            AssetDatabase.Refresh();
            string catalogPath = "Assets/StreamingAssets/Rhythm/Festival20261029/catalog.json";
            Catalog catalog = JsonUtility.FromJson<Catalog>(File.ReadAllText(catalogPath));
            if (catalog == null || catalog.songs == null || catalog.songs.Length != 20)
                throw new InvalidOperationException("Expected twenty prepared festival/standard profiles.");
            if (catalog.songs.Select(song => song.trackId).Distinct().Count() != catalog.songs.Length)
                throw new InvalidOperationException("Prepared profile identities overlap.");
            if (catalog.songs.Count(song => song.mode == "festival") != 10 ||
                catalog.songs.Count(song => song.mode == "standard") != 10)
                throw new InvalidOperationException("Expected ten profiles in each mode.");

            var scene = EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity", OpenSceneMode.Single);
            RoundController controller = UnityEngine.Object.FindFirstObjectByType<RoundController>();
            RhythmChartReader reader = UnityEngine.Object.FindFirstObjectByType<RhythmChartReader>();
            BalloonSpawner spawner = UnityEngine.Object.FindFirstObjectByType<BalloonSpawner>();
            if (controller == null || reader == null || spawner == null)
                throw new InvalidOperationException("Production scene is missing the current rhythm loop.");
            SerializedObject serialized = new SerializedObject(controller);
            SerializedProperty profiles = serialized.FindProperty("exhibitionSongs");
            profiles.arraySize = catalog.songs.Length;
            for (int index = 0; index < catalog.songs.Length; index++)
            {
                Profile source = catalog.songs[index];
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(source.audioAssetPath);
                if (clip == null || !File.Exists(Path.Combine("Assets/StreamingAssets", source.chartRelativePath)))
                    throw new InvalidOperationException("Missing prepared audio/chart: " + source.title);
                if (source.roundDurationSeconds <= 0d || source.sourceCueInSeconds < 0d ||
                    Math.Abs(source.sourceCueOutSeconds - source.sourceCueInSeconds - source.roundDurationSeconds) > 0.001d ||
                    source.sourceCueOutSeconds > clip.length + 0.15d)
                    throw new InvalidOperationException(string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Invalid prepared window: {0}; mode={1}; cueIn={2:R}; cueOut={3:R}; duration={4:R}; clip.length={5:R}; windowDelta={6:R}; overrun={7:R}; audio={8}",
                        source.title, source.mode, source.sourceCueInSeconds, source.sourceCueOutSeconds,
                        source.roundDurationSeconds, clip.length,
                        source.sourceCueOutSeconds - source.sourceCueInSeconds - source.roundDurationSeconds,
                        source.sourceCueOutSeconds - clip.length, source.audioAssetPath));
                if (source.mode == "festival" && (source.roundDurationSeconds < 60d || source.roundDurationSeconds > 75d))
                    throw new InvalidOperationException("Festival profile is outside the short-round target.");
                SerializedProperty destination = profiles.GetArrayElementAtIndex(index);
                destination.FindPropertyRelative("trackId").stringValue = source.trackId;
                destination.FindPropertyRelative("title").stringValue = source.title;
                destination.FindPropertyRelative("artist").stringValue = source.artist;
                destination.FindPropertyRelative("mode").stringValue = source.mode;
                destination.FindPropertyRelative("audioClip").objectReferenceValue = clip;
                destination.FindPropertyRelative("chartRelativePath").stringValue = source.chartRelativePath;
                destination.FindPropertyRelative("sourceCueInSeconds").doubleValue = source.sourceCueInSeconds;
                destination.FindPropertyRelative("sourceCueOutSeconds").doubleValue = source.sourceCueOutSeconds;
                destination.FindPropertyRelative("roundDurationSeconds").doubleValue = source.roundDurationSeconds;
                destination.FindPropertyRelative("sourceEventStartId").intValue = source.sourceEventStartId;
                destination.FindPropertyRelative("sourceEventEndIdExclusive").intValue = source.sourceEventEndIdExclusive;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();

            FestivalDemonstrationCue cue = controller.GetComponent<FestivalDemonstrationCue>();
            if (cue == null) cue = controller.gameObject.AddComponent<FestivalDemonstrationCue>();
            SerializedObject cueData = new SerializedObject(cue);
            cueData.FindProperty("roundController").objectReferenceValue = controller;
            cueData.FindProperty("chartReader").objectReferenceValue = reader;
            cueData.FindProperty("balloonSpawner").objectReferenceValue = spawner;
            cueData.FindProperty("textTemplate").objectReferenceValue = UnityEngine.Object.FindObjectsByType<Text>(
                FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(text => text.name == "ControlsText");
            cueData.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("FESTIVAL_EXPERIENCE_SETUP_OK: twenty profiles; phrase patterns; non-scoring demonstration; no ribbon.");
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }
}
#endif
