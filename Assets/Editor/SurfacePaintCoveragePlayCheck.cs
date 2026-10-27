using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace BalloonBarrage.EditorTools
{
    [InitializeOnLoad]
    public static class SurfacePaintCoveragePlayCheck
    {
        private const string EnabledKey =
            "SurfacePaintCoveragePlayCheck.Enabled";
        private const string StageKey =
            "SurfacePaintCoveragePlayCheck.Stage";
        private const string RetryKey =
            "SurfacePaintCoveragePlayCheck.Retry";
        private const string ScenePath =
            "Assets/Scenes/PrototypeScene.unity";
        private const int StressMarkCount = 1000;

        private static readonly string OutputDirectory =
            Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "../.local-reference/surface-paint-coverage"));

        private static double stageStarted;
        private static double deadline;
        private static int lastControllerId;
        private static PaintProjectile projectile;
        private static RenderSample baseline;

        [Serializable]
        private struct RenderSample
        {
            public double meanMilliseconds;
            public double p95Milliseconds;
            public long allocatedMemoryBytes;
            public int markCount;
        }

        [Serializable]
        private struct CoverageReport
        {
            public string unityVersion;
            public string graphicsDevice;
            public int surfaceCount;
            public int pigmentCount;
            public int retryCount;
            public RenderSample baselineRender;
            public RenderSample denseRender;
        }

        static SurfacePaintCoveragePlayCheck()
        {
            if (SessionState.GetBool(EnabledKey, false))
            {
                EditorApplication.update += Tick;
            }
        }

        public static void RunBatch()
        {
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "Run the coverage Play Mode check in batch mode without -quit.");
            }

            Directory.CreateDirectory(OutputDirectory);
            EditorSceneManager.OpenScene(ScenePath);
            SessionState.SetBool(EnabledKey, true);
            SessionState.SetInt(StageKey, 0);
            SessionState.SetInt(RetryKey, 0);
            deadline = EditorApplication.timeSinceStartup + 240d;
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            EditorApplication.EnterPlaymode();
        }

        private static void Tick()
        {
            if (!SessionState.GetBool(EnabledKey, false) ||
                !EditorApplication.isPlaying)
            {
                return;
            }

            try
            {
                if (deadline <= 0d)
                {
                    deadline = EditorApplication.timeSinceStartup + 240d;
                }

                Require(EditorApplication.timeSinceStartup < deadline,
                    "Coverage Play Mode check timed out.");

                int stage = SessionState.GetInt(StageKey, 0);
                RoundController controller =
                    Object.FindFirstObjectByType<RoundController>();

                if (stage == 0)
                {
                    if (Time.frameCount < 8 || controller == null)
                    {
                        return;
                    }

                    Camera camera = Camera.main;
                    Require(camera != null, "Missing production camera.");
                    baseline = MeasureRender(camera, 12, 48);

                    PlayerShooter shooter =
                        Object.FindFirstObjectByType<PlayerShooter>();
                    Require(shooter != null, "Missing PlayerShooter.");
                    PaintProjectile source = new SerializedObject(shooter)
                        .FindProperty("paintProjectilePrefab")
                        .objectReferenceValue as PaintProjectile;
                    Require(source != null,
                        "PlayerShooter lost its paint projectile reference.");

                    projectile = Object.Instantiate(source);
                    projectile.enabled = false;
                    SpawnPreparedRoomSet(projectile);

                    Require(CoverageMarks().Length == 72,
                        "Nine surfaces x eight pigments must create 72 marks.");
                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 1);
                    return;
                }

                if (stage == 1)
                {
                    if (EditorApplication.timeSinceStartup - stageStarted < 0.4d)
                    {
                        return;
                    }

                    Require(CoverageMarks().Length == 72,
                        "Coverage marks did not persist after their impact pulse.");
                    Require(Object.FindObjectsByType<PaintMarkImpactPulse>(
                            FindObjectsInactive.Include,
                            FindObjectsSortMode.None).Length == 0,
                        "Impact-pulse helper did not remove itself.");

                    Time.timeScale = 0f;
                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 2);
                    return;
                }

                if (stage == 2)
                {
                    if (EditorApplication.timeSinceStartup - stageStarted < 0.2d)
                    {
                        return;
                    }

                    Require(CoverageMarks().Length == 72,
                        "Pause changed persistent paint.");
                    Time.timeScale = 1f;

                    SpawnStressMarks(projectile, StressMarkCount - 72);
                    SpriteRenderer[] marks = CoverageMarks();
                    Require(marks.Length == StressMarkCount,
                        "Stress fixture must retain exactly 1,000 marks.");
                    ValidateIntegratedMarks(marks);

                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 6);
                    return;
                }

                if (stage == 6)
                {
                    if (EditorApplication.timeSinceStartup - stageStarted < 0.4d)
                    {
                        return;
                    }

                    Require(CoverageMarks().Length == StressMarkCount,
                        "Stress marks did not remain after settling.");
                    Require(Object.FindObjectsByType<PaintMarkImpactPulse>(
                            FindObjectsInactive.Include,
                            FindObjectsSortMode.None).Length == 0,
                        "Stress-mark pulse helpers remained active.");

                    Camera camera = Camera.main;
                    RenderSample dense = MeasureRender(camera, 20, 120);
                    WriteReport(dense);
                    Capture(camera, Path.Combine(
                        OutputDirectory,
                        "DenseCoverage.png"));

                    Object.Destroy(projectile.gameObject);
                    projectile = null;
                    lastControllerId = controller.GetInstanceID();
                    SessionState.SetInt(RetryKey, 1);
                    SessionState.SetInt(StageKey, 3);
                    controller.RestartRound();
                    return;
                }

                if (stage == 3)
                {
                    if (controller == null ||
                        controller.GetInstanceID() == lastControllerId)
                    {
                        return;
                    }

                    Require(CoverageMarks().Length == 0,
                        "Retry retained paint from the previous scene.");

                    SerializedObject serialized = new SerializedObject(controller);
                    AudioSource music = serialized.FindProperty("gameMusic")
                        .objectReferenceValue as AudioSource;
                    if (music == null || music.clip == null ||
                        music.clip.loadState != AudioDataLoadState.Loaded)
                    {
                        return;
                    }

                    controller.BeginRound();
                    Require(RoundController.IsPlaying,
                        "Natural complete-song check could not begin.");
                    stageStarted = EditorApplication.timeSinceStartup;
                    SessionState.SetInt(StageKey, 4);
                    return;
                }

                if (stage == 4)
                {
                    double elapsed =
                        EditorApplication.timeSinceStartup - stageStarted;
                    Require(elapsed < 140d,
                        "Natural complete-song check exceeded 140 seconds.");

                    if (RoundController.IsPlaying || elapsed < 120d)
                    {
                        return;
                    }

                    SerializedProperty results = new SerializedObject(controller)
                        .FindProperty("resultsScreen");
                    GameObject resultsScreen =
                        results.objectReferenceValue as GameObject;
                    Require(resultsScreen != null && resultsScreen.activeSelf,
                        "Natural song ending did not show results.");

                    lastControllerId = controller.GetInstanceID();
                    SessionState.SetInt(RetryKey, 2);
                    SessionState.SetInt(StageKey, 5);
                    controller.RestartRound();
                    return;
                }

                if (stage == 5)
                {
                    if (controller == null ||
                        controller.GetInstanceID() == lastControllerId)
                    {
                        return;
                    }

                    Require(CoverageMarks().Length == 0,
                        "Repeated Retry retained persistent paint.");

                    int retries = SessionState.GetInt(RetryKey, 0);
                    if (retries < 10)
                    {
                        lastControllerId = controller.GetInstanceID();
                        SessionState.SetInt(RetryKey, retries + 1);
                        controller.RestartRound();
                        return;
                    }

                    SessionState.SetBool(EnabledKey, false);
                    Time.timeScale = 1f;
                    Debug.Log(
                        "SURFACE_PAINT_COVERAGE_PLAY_OK: nine prepared " +
                        "surfaces, eight pigments, 1,000 persistent marks, " +
                        "pause, natural complete-song results and ten Retry " +
                        "scene resets passed. Human feel and listening were " +
                        "excluded at the creator's request.");
                    EditorApplication.Exit(0);
                }
            }
            catch (Exception exception)
            {
                SessionState.SetBool(EnabledKey, false);
                Time.timeScale = 1f;
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void SpawnPreparedRoomSet(
            PaintProjectile paintProjectile)
        {
            PaintSurface[] surfaces = Object.FindObjectsByType<PaintSurface>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .OrderBy(surface => surface.name, StringComparer.Ordinal)
                .ToArray();
            Require(surfaces.Length == 9,
                "Production scene must expose nine prepared paint surfaces.");

            for (int surfaceIndex = 0;
                 surfaceIndex < surfaces.Length;
                 surfaceIndex++)
            {
                Collider collider = ColliderFor(surfaces[surfaceIndex]);
                for (int pigment = 0; pigment < 8; pigment++)
                {
                    HashSet<int> before = CoverageMarks()
                        .Select(renderer => renderer.GetInstanceID())
                        .ToHashSet();
                    float u = ((pigment % 4) - 1.5f) / 2f;
                    float v = ((pigment / 4) - 0.5f) * 0.8f;
                    RaycastHit hit = HitFor(collider, u, v);
                    paintProjectile.SpawnPaintMark(hit, pigment);
                    SpriteRenderer[] after = CoverageMarks();
                    Require(after.Length == before.Count + 1,
                        surfaces[surfaceIndex].name +
                        " failed pigment " + pigment + ".");
                    SpriteRenderer created = after.Single(renderer =>
                        !before.Contains(renderer.GetInstanceID()));
                    Require(Vector3.Dot(
                            created.transform.forward,
                            hit.normal) > 0.98f,
                        created.name + " does not face its hit normal.");
                    Require(Vector3.Distance(
                            created.transform.position,
                            hit.point + hit.normal * 0.005f) < 0.001f,
                        created.name + " is not at its sampled contact.");
                }
            }
        }

        private static void SpawnStressMarks(
            PaintProjectile paintProjectile,
            int count)
        {
            PaintSurface[] surfaces = Object.FindObjectsByType<PaintSurface>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .OrderBy(surface => surface.name, StringComparer.Ordinal)
                .ToArray();

            for (int i = 0; i < count; i++)
            {
                Collider collider = ColliderFor(surfaces[i % surfaces.Length]);
                float u = Mathf.Sin(i * 1.618f) * 0.75f;
                float v = Mathf.Cos(i * 2.414f) * 0.75f;
                paintProjectile.SpawnPaintMark(
                    HitFor(collider, u, v),
                    i % 8);
            }
        }

        private static Collider ColliderFor(PaintSurface surface)
        {
            Collider collider = surface.GetComponent<Collider>();
            if (collider == null)
            {
                collider = surface.GetComponentInChildren<Collider>(true);
            }
            Require(collider != null,
                surface.name + " has no paint-contact collider.");
            return collider;
        }

        private static RaycastHit HitFor(
            Collider collider,
            float u,
            float v)
        {
            Bounds bounds = collider.bounds;
            Vector3 size = bounds.size;
            int axis = size.x <= size.y && size.x <= size.z
                ? 0
                : size.y <= size.z ? 1 : 2;

            Vector3 normal = axis == 0 ? Vector3.right :
                axis == 1 ? Vector3.up : Vector3.forward;
            Camera camera = Camera.main;
            if (camera != null && Vector3.Dot(
                    camera.transform.position - bounds.center,
                    normal) < 0f)
            {
                normal = -normal;
            }

            Vector3 tangent = axis == 0 ? Vector3.up : Vector3.right;
            Vector3 bitangent = axis == 2 ? Vector3.up : Vector3.forward;
            float tangentExtent = axis == 0
                ? bounds.extents.y
                : bounds.extents.x;
            float bitangentExtent = axis == 2
                ? bounds.extents.y
                : bounds.extents.z;
            float normalExtent = axis == 0
                ? bounds.extents.x
                : axis == 1 ? bounds.extents.y : bounds.extents.z;

            Vector3 point = bounds.center +
                tangent * (tangentExtent * Mathf.Clamp(u, -0.8f, 0.8f)) +
                bitangent * (bitangentExtent * Mathf.Clamp(v, -0.8f, 0.8f));
            Ray ray = new Ray(
                point + normal * (normalExtent + 0.5f),
                -normal);

            if (!collider.Raycast(ray, out RaycastHit hit,
                    normalExtent * 2f + 1f))
            {
                ray = new Ray(
                    point - normal * (normalExtent + 0.5f),
                    normal);
                Require(collider.Raycast(ray, out hit,
                        normalExtent * 2f + 1f),
                    "Could not raycast prepared surface " + collider.name + ".");
            }

            return hit;
        }

        private static SpriteRenderer[] CoverageMarks()
        {
            return Object.FindObjectsByType<SpriteRenderer>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Where(renderer => renderer.name.StartsWith(
                    "SurfacePaintCoverage_",
                    StringComparison.Ordinal))
                .ToArray();
        }

        private static void ValidateIntegratedMarks(
            IEnumerable<SpriteRenderer> marks)
        {
            HashSet<string> colors = new HashSet<string>();
            foreach (SpriteRenderer renderer in marks)
            {
                string spritePath = AssetDatabase.GetAssetPath(renderer.sprite);
                Require(spritePath.Contains(
                        "SurfacePaintCoverage20261027",
                        StringComparison.Ordinal),
                    renderer.name + " did not use the new coverage atlas.");
                Require(renderer.GetComponent<Collider>() == null,
                    renderer.name + " unexpectedly blocks later paint rays.");

                string[] pieces = renderer.name.Split('_');
                if (pieces.Length >= 2)
                {
                    colors.Add(pieces[1]);
                }
            }

            Require(colors.Count == 8,
                "Stress marks did not exercise all eight pigments.");
        }

        private static RenderSample MeasureRender(
            Camera camera,
            int warmupFrames,
            int sampleFrames)
        {
            Require(camera != null, "Cannot profile without a camera.");
            RenderTexture target = RenderTexture.GetTemporary(
                2560,
                1440,
                24,
                RenderTextureFormat.ARGB32);
            RenderTexture previousTarget = camera.targetTexture;
            List<double> samples = new List<double>(sampleFrames);
            Stopwatch watch = new Stopwatch();

            try
            {
                camera.targetTexture = target;
                camera.aspect = 16f / 9f;
                for (int i = 0; i < warmupFrames; i++)
                {
                    camera.Render();
                }
                for (int i = 0; i < sampleFrames; i++)
                {
                    watch.Restart();
                    camera.Render();
                    watch.Stop();
                    samples.Add(watch.Elapsed.TotalMilliseconds);
                }
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.ReleaseTemporary(target);
            }

            samples.Sort();
            int p95Index = Mathf.Clamp(
                Mathf.CeilToInt(samples.Count * 0.95f) - 1,
                0,
                samples.Count - 1);
            return new RenderSample
            {
                meanMilliseconds = samples.Average(),
                p95Milliseconds = samples[p95Index],
                allocatedMemoryBytes = Profiler.GetTotalAllocatedMemoryLong(),
                markCount = CoverageMarks().Length
            };
        }

        private static void WriteReport(RenderSample dense)
        {
            CoverageReport report = new CoverageReport
            {
                unityVersion = Application.unityVersion,
                graphicsDevice = SystemInfo.graphicsDeviceName,
                surfaceCount = 9,
                pigmentCount = 8,
                retryCount = 10,
                baselineRender = baseline,
                denseRender = dense
            };
            File.WriteAllText(
                Path.Combine(OutputDirectory, "report.json"),
                JsonUtility.ToJson(report, true));
        }

        private static void Capture(Camera camera, string outputPath)
        {
            RenderTexture target = RenderTexture.GetTemporary(
                1920,
                1080,
                24,
                RenderTextureFormat.ARGB32);
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture previousTarget = camera.targetTexture;
            Texture2D pixels = new Texture2D(
                1920,
                1080,
                TextureFormat.RGB24,
                false);

            try
            {
                camera.targetTexture = target;
                camera.aspect = 16f / 9f;
                camera.Render();
                RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0f, 0f, 1920f, 1080f), 0, 0);
                pixels.Apply();
                File.WriteAllBytes(outputPath, pixels.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                RenderTexture.ReleaseTemporary(target);
                Object.DestroyImmediate(pixels);
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
