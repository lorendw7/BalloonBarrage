using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    // Reuses the existing cosmetic component; never edits learner gameplay or a scene.
    public static class BalloonColorLessonPreparation
    {
        const string Source = "Assets/Prefabs/PlayableBalloon.prefab";
        const string Folder = "Assets/StudioArtPack/BalloonColors/Prefabs";
        const string Output = Folder + "/LessonBalloon_ThreeColors.prefab";

        static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }

        [MenuItem("Tools/Balloon Studio/Prepare Three-Color Balloon Lesson")]
        public static void Build()
        {
            Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Leave Play Mode first.");
            if (File.Exists(Output))
            {
                Debug.Log("THREE_COLOR_LESSON_PRESERVED: " + Output);
                return;
            }
            Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();
            var root = PrefabUtility.LoadPrefabContents(Source);
            try
            {
                var latex = root.GetComponentsInChildren<MeshRenderer>(true).Single();
                var body = latex.GetComponent<MeshFilter>();
                Require(body != null && body.sharedMesh != null, "Missing current balloon mesh.");
                var originalComponents = root.GetComponentsInChildren<Component>(true)
                    .Where(c => c != null).ToDictionary(c => c, c => EditorJsonUtility.ToJson(c));
                var appearance = root.GetComponent<StudioBalloonAppearance>();
                Require(appearance == null, "Production prefab already has appearance selection; review before rebuilding.");
                appearance = root.AddComponent<StudioBalloonAppearance>();
                appearance.body = body;
                appearance.latex = latex;
                // One existing mesh: color selection must not change shape or collider alignment.
                appearance.silhouettes = new[] { body.sharedMesh };
                appearance.pigments = new[] {
                    latex.sharedMaterial,
                    AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/BalloonColors/CoralLatex.mat"),
                    AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/BalloonColors/GoldLatex.mat")
                };
                Require(appearance.pigments.All(m => m != null && m.HasProperty("_BaseColor")),
                    "Invalid pigment material.");
                foreach (var item in originalComponents)
                    Require(EditorJsonUtility.ToJson(item.Key) == item.Value,
                        "Preparation changed an existing component: " + item.Key.GetType().Name);
                PrefabUtility.SaveAsPrefabAsset(root, Output);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            AssetDatabase.SaveAssets();
            Debug.Log("THREE_COLOR_BALLOON_PREPARED: " + Output);
        }

        public static void VerifyBatch()
        {
            Require(Application.isBatchMode, "Verify in an isolated batch project.");
            Build();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Output);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                var appearance = instance.GetComponent<StudioBalloonAppearance>();
                var balloon = instance.GetComponent<Balloon>();
                var serialized = new SerializedObject(balloon);
                Require(balloon.points == 10, "Base score changed.");
                var source = new SerializedObject(AssetDatabase.LoadAssetAtPath<GameObject>(Source).GetComponent<Balloon>());
                Require(Math.Abs(serialized.FindProperty("cleanupDelaySeconds").doubleValue -
                    source.FindProperty("cleanupDelaySeconds").doubleValue) < .0001,
                    "Expiry tuning changed.");
                Require(Math.Abs(serialized.FindProperty("rhythmWindowsSeconds").doubleValue - .1) < .0001,
                    "Judgement window changed.");
                var pop = serialized.FindProperty("popEffectPrefab").objectReferenceValue as ParticleSystem;
                Require(pop != null && serialized.FindProperty("onBeatGlowPrefab").objectReferenceValue != null,
                    "Lost burst or on-beat glow.");
                var mesh = appearance.body.sharedMesh;
                var physics = instance.GetComponentsInChildren<Collider>(true).Cast<Component>()
                    .Concat(instance.GetComponentsInChildren<Rigidbody>(true)).ToDictionary(c => c, c => EditorJsonUtility.ToJson(c));
                for (int i = 0; i < 3; i++)
                {
                    appearance.ApplyVariant(i, 0);
                    Require(appearance.latex.sharedMaterial == appearance.pigments[i], "Wrong body material.");
                    Require(appearance.body.sharedMesh == mesh, "Shape changed.");
                    var effect = Object.Instantiate(pop);
                    try
                    {
                        appearance.ColorizeBurst(effect.gameObject);
                        foreach (var part in effect.GetComponentsInChildren<ParticleSystem>(true))
                        {
                            var color = part.main.startColor;
                            var observed = color.mode == ParticleSystemGradientMode.Color ? color.color :
                                color.mode == ParticleSystemGradientMode.TwoColors ? color.colorMin :
                                color.mode == ParticleSystemGradientMode.TwoGradients ? color.gradientMin.Evaluate(.5f) :
                                color.gradient.Evaluate(.5f);
                            Require(Vector3.Distance(new Vector3(observed.r, observed.g, observed.b),
                                new Vector3(appearance.Pigment.r, appearance.Pigment.g, appearance.Pigment.b)) < .002f,
                                "Burst pigment mismatch.");
                        }
                    }
                    finally { Object.DestroyImmediate(effect.gameObject); }
                }
                // Exercise the existing spawn appearance selection without calling gameplay Awake methods.
                var awake = typeof(StudioBalloonAppearance).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
                for (int i = 0; i < 24; i++)
                {
                    awake.Invoke(appearance, null);
                    Require(appearance.ColorIndex >= 0 && appearance.ColorIndex < 3 && appearance.ShapeIndex == 0,
                        "Invalid spawn selection.");
                }
                foreach (var item in physics)
                    Require(EditorJsonUtility.ToJson(item.Key) == item.Value, "Physics configuration changed.");
                Debug.Log("THREE_COLOR_BALLOON_VERIFY_OK: current source components preserved; 3 pigment/burst matches; " +
                    "24 valid spawn selections; one unchanged mesh; source expiry, score 10, window .1 and glow preserved; separate lesson asset, no scene hookup.");
            }
            finally { Object.DestroyImmediate(instance); }
        }
    }
}
