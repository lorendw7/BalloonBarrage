using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BalloonBarrage.EditorTools
{
    // Configuration migration only; run in an isolated batch copy before copying checked scenes back.
    public static class StudioMusicSceneSetup
    {
        private static readonly string[] Scenes =
        {
            "Assets/Scenes/StudioConceptVariants.unity",
            "Assets/Scenes/StudioConceptDetail.unity"
        };

        private static string OtherControllerFields(MonoBehaviour controller)
        {
            return Regex.Replace(EditorJsonUtility.ToJson(controller),
                "\"(?:menuMusic|gameMusic)\":\\{[^{}]*\\}", "\"musicReference\":{}");
        }

        public static void ApplyBatch()
        {
            if (!Application.isBatchMode)
                throw new InvalidOperationException("Run this migration in an isolated batch project.");
            var gamePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Audio/Music/StepMania/GameMusic_GoinUnder.prefab");
            if (gamePrefab == null) throw new InvalidOperationException("Prepared music prefab is missing.");

            foreach (string path in Scenes)
            {
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                var controllers = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<MonoBehaviour>(true))
                    .Where(component => component != null && component.GetType().Name == "RoundController").ToArray();
                if (controllers.Length != 1) throw new InvalidOperationException("Expected one round controller: " + path);
                var controller = controllers[0];
                string before = OtherControllerFields(controller);
                var sources = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<AudioSource>(true)).ToArray();
                var menu = sources.Single(source => source.name == "StudioBackgroundMusic");
                var game = sources.SingleOrDefault(source => source.name == "GameMusic_GoinUnder");
                if (game == null)
                {
                    var instance = (GameObject)PrefabUtility.InstantiatePrefab(gamePrefab, scene);
                    instance.transform.SetParent(menu.transform.parent, false);
                    game = instance.GetComponent<AudioSource>();
                }
                if (menu.clip == null || game.clip == null)
                    throw new InvalidOperationException("Music clip missing: " + path);

                menu.playOnAwake = false; menu.loop = true; menu.pitch = 1;
                game.playOnAwake = false; game.loop = false; game.pitch = 1;
                PrefabUtility.RecordPrefabInstancePropertyModifications(menu);
                PrefabUtility.RecordPrefabInstancePropertyModifications(game);

                var serialized = new SerializedObject(controller);
                serialized.FindProperty("menuMusic").objectReferenceValue = menu;
                serialized.FindProperty("gameMusic").objectReferenceValue = game;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                if (before != OtherControllerFields(controller))
                    throw new InvalidOperationException("Non-music controller fields changed: " + path);
                if (!EditorSceneManager.SaveScene(scene))
                    throw new InvalidOperationException("Could not save music configuration: " + path);
                Debug.Log("STUDIO_MUSIC_SETUP_OK: " + path);
            }
        }
    }
}
