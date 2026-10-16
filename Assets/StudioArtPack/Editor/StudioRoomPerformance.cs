using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

namespace BalloonBarrage.EditorTools
{
    // Preparation and controlled camera comparison; no runtime gameplay changes.
    public static class StudioRoomPerformance
    {
        private const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
        private const string Output = ".local-backups/room-performance";

        [MenuItem("Tools/BalloonBarrage/Prepare Static Room Batching")]
        public static void PrepareScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play Mode first.");
            var scene = SceneManager.GetActiveScene();
            if (scene.path != ScenePath) throw new InvalidOperationException("Open PrototypeScene first.");
            if (All<StudioRoomBatching>(scene).Length != 0) throw new InvalidOperationException("Room batching already prepared; preserve its settings.");
            var candidates = All<MeshRenderer>(scene).Where(Eligible).ToArray();
            if (candidates.Length < 20) throw new InvalidOperationException("Unexpectedly few fixed room meshes: " + candidates.Length);
            var root = new GameObject("StudioRoomBatching");
            SceneManager.MoveGameObjectToScene(root, scene);
            Undo.RegisterCreatedObjectUndo(root, "Prepare room batching");
            var batching = Undo.AddComponent<StudioRoomBatching>(root);
            var serialized = new SerializedObject(batching);
            var array = serialized.FindProperty("surfaces");
            array.arraySize = candidates.Length;
            for (int i = 0; i < candidates.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = candidates[i];
            serialized.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("ROOM_BATCHING_PREPARED: " + candidates.Length + " explicit fixed opaque room renderers. Save the scene.");
        }

        private static bool Eligible(MeshRenderer renderer)
        {
            if (!renderer.enabled || !renderer.gameObject.activeInHierarchy || renderer.HasPropertyBlock() ||
                (GameObjectUtility.GetStaticEditorFlags(renderer.gameObject) & StaticEditorFlags.ContributeGI) == 0 ||
                renderer.GetComponentInParent<Camera>() != null || renderer.GetComponentInParent<Rigidbody>() != null ||
                renderer.GetComponentInParent<Canvas>() != null || renderer.GetComponentInParent<LODGroup>() != null ||
                renderer.GetComponentsInParent<MonoBehaviour>(true).Any()) return false;
            var filter = renderer.GetComponent<MeshFilter>();
            return filter != null && filter.sharedMesh != null && filter.sharedMesh.isReadable &&
                filter.sharedMesh.vertexCount > 0 && renderer.sharedMaterials.Length > 0 &&
                renderer.sharedMaterials.All(m => m != null && m.renderQueue <= 2500 &&
                    m.GetTag("DisableBatching", false, "False") != "True");
        }

        public static void PrepareAndCompareBatch()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated project.");
            Directory.CreateDirectory(Output);
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var protectedObjects = All<Component>(scene).Where(c => c is Transform || c is Collider || c is Rigidbody ||
                c is Camera || c is Light || c is AudioSource || c is MonoBehaviour).ToArray();
            var snapshots = protectedObjects.Select(EditorJsonUtility.ToJson).ToArray();
            PrepareScene();
            if (!snapshots.SequenceEqual(protectedObjects.Select(EditorJsonUtility.ToJson)))
                throw new InvalidOperationException("Existing component serialization changed.");
            // Save only the whitelist/helper. Never serialize runtime combined meshes back to the scene.
            EditorSceneManager.SaveScene(scene);
            scene = EditorSceneManager.OpenScene(ScenePath);
            var batching = All<StudioRoomBatching>(scene).Single();
            foreach (var canvas in All<Canvas>(scene)) canvas.gameObject.SetActive(false);
            var camera = Camera.main;
            var shooter = All<PlayerShooter>(scene).Single();
            shooter.transform.LookAt(new Vector3(0, 0, 8));
            var target = RenderTexture.GetTemporary(2560, 1440, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = target;
            camera.aspect = 16f / 9f;
            var poses = All<Transform>(scene).Select(t => t.localToWorldMatrix).ToArray();
            var physics = All<Collider>(scene).Select(EditorJsonUtility.ToJson).ToArray();
            var lights = All<Light>(scene).Select(EditorJsonUtility.ToJson).ToArray();
            var before = Capture(camera, target, "Before");
            batching.PrepareOnce();
            if (batching.BatchedRendererCount < 20) throw new InvalidOperationException("Too few meshes actually batched: " + batching.BatchedRendererCount);
            var after = Capture(camera, target, "After");
            if (!poses.SequenceEqual(All<Transform>(scene).Select(t => t.localToWorldMatrix)) ||
                !physics.SequenceEqual(All<Collider>(scene).Select(EditorJsonUtility.ToJson)) ||
                !lights.SequenceEqual(All<Light>(scene).Select(EditorJsonUtility.ToJson)))
                throw new InvalidOperationException("Batching changed transforms, physics or lights.");
            camera.targetTexture = null;
            RenderTexture.ReleaseTemporary(target);
            string report = "ROOM_BATCHING_COMPARE_OK: " + batching.BatchedRendererCount + " static room renderers batched.\n" +
                "Before: " + before + "\nAfter: " + after + "\n" +
                "2560x1440 camera-only edit-mode comparison; CPU submission timings are not player FPS or GPU timings. " +
                "Existing components preserved while preparing; transforms/colliders/lights unchanged by runtime batching. " +
                "No core gameplay source, camera, image quality, lighting or physics setting changed. Human performance acceptance pending.\n";
            File.WriteAllText(Output + "/Verification.txt", report);
            Debug.Log(report);
        }

        private static string Capture(Camera camera, RenderTexture target, string label)
        {
            for (int i = 0; i < 12; i++) camera.Render();
            var watch = new Stopwatch();
            var times = new double[30];
            for (int i = 0; i < times.Length; i++)
            {
                watch.Restart(); camera.Render(); watch.Stop(); times[i] = watch.Elapsed.TotalMilliseconds;
            }
            Array.Sort(times);
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            var pixels = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
            pixels.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            pixels.Apply();
            File.WriteAllBytes(Output + "/" + label + ".png", pixels.EncodeToPNG());
            Object.DestroyImmediate(pixels);
            RenderTexture.active = previous;
            // UnityStats returns zero for this synchronous batch render; don't report it as a workload count.
            return $"median Camera.Render CPU submission={times[15]:F3} ms; UnityStats counters unavailable in this batch pass";
        }

        private static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects()
            .SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    }
}
