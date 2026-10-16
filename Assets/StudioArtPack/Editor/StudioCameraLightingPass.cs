using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// Editor-only, opt-in scene copy. Never edits learner scripts or shared art/pipeline assets.
public static class StudioCameraLightingPass
{
    public const string Source = "Assets/Scenes/PrototypeScene.unity";
    public const string Destination = "Assets/Scenes/StudioCameraLighting.unity";
    public const string Root = "Assets/StudioArtPack/CameraLightingV10";
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects()
        .SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static Camera GameCamera(Scene scene) => All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
    static Color Hex(string text) { ColorUtility.TryParseHtmlString("#" + text, out var c); return c; }
    static void Record(UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
        if (PrefabUtility.IsPartOfPrefabInstance(obj)) PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
    }

    [MenuItem("BalloonBarrage/Art/Create camera and lighting scene copy")]
    public static void Create()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (File.Exists(Destination) || Directory.Exists(Root))
            throw new InvalidOperationException("Pass already exists. Open its scene; do not overwrite manual changes.");
        if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset pipeline))
            throw new InvalidOperationException("This pass requires the project's installed URP pipeline.");
        var source = EditorSceneManager.OpenScene(Source);
        Directory.CreateDirectory(Root + "/Previews");
        Directory.CreateDirectory(Root + "/Reflections");
        AssetDatabase.Refresh();
        WriteAudit(source, pipeline);
        Capture(source, "Before_Room");
        CaptureTargets(source, "Before_Targets");
        if (!AssetDatabase.CopyAsset(Source, Destination)) throw new IOException("Scene copy failed.");
        var scene = EditorSceneManager.OpenScene(Destination);
        var protectedComponents = All<Component>(scene).Where(c => c != null &&
            (c is Collider || c is Rigidbody || c is Canvas ||
             c is MonoBehaviour mb && AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(mb))
                 .StartsWith("Assets/Scripts/", StringComparison.Ordinal))).ToArray();
        var snapshot = protectedComponents.Select(c => EditorJsonUtility.ToJson(c)).ToArray();
        var camera = GameCamera(scene);
        camera.transform.position = new Vector3(0, -.4f, -7.2f);
        camera.transform.rotation = Quaternion.Euler(.6f, 0, 0);
        camera.fieldOfView = 38;
        Record(camera); Record(camera.transform);

        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(.68f, .70f, .71f);
        RenderSettings.ambientEquatorColor = new Color(.58f, .55f, .50f);
        RenderSettings.ambientGroundColor = new Color(.34f, .32f, .28f);
        RenderSettings.fog = false;
        var sun = All<Light>(scene).Single(l => l.type == LightType.Directional);
        sun.transform.rotation = Quaternion.LookRotation(new Vector3(.55f, -.55f, .62f));
        sun.color = Color.white; sun.useColorTemperature = true; sun.colorTemperature = 5000;
        sun.intensity = 1.45f; sun.shadows = LightShadows.Soft; sun.shadowStrength = .48f;
        sun.shadowBias = .025f; sun.shadowNormalBias = .12f;
        sun.GetUniversalAdditionalLightData().softShadowQuality = SoftShadowQuality.High;
        RenderSettings.sun = sun; Record(sun); Record(sun.transform);
        var window = All<Light>(scene).Single(l => l.name == "WindowDaylight");
        window.color = Hex("FFF3E0"); window.useColorTemperature = false;
        window.intensity = 105; window.shadowStrength = .45f;
        window.GetUniversalAdditionalLightData().softShadowQuality = SoftShadowQuality.High;
        Record(window);
        // Reuse the former frontal point fill as a broad skylight source, avoiding another light.
        var fill = All<Light>(scene).Single(l => l.name == "FrontFillLight");
        fill.type = LightType.Spot; fill.transform.position = new Vector3(0, 3.6f, 4.4f);
        fill.transform.LookAt(new Vector3(0, -.8f, 8));
        fill.spotAngle = 125; fill.innerSpotAngle = 95; fill.range = 18;
        fill.color = Hex("FFF3E5"); fill.useColorTemperature = false;
        fill.intensity = 18; fill.shadows = LightShadows.None;
        Record(fill); Record(fill.transform);
        All<Light>(scene).Single(l => l.name == "BakedWindowSoftbox").intensity = 4.4f;
        var skyBounce = All<Light>(scene).Single(l => l.name == "BakedSkylightBounce");
        skyBounce.intensity = 2.6f; skyBounce.color = Hex("FFF6E8");

        var probes = new GameObject("CameraLighting_TargetLightProbes").AddComponent<LightProbeGroup>();
        SceneManager.MoveGameObjectToScene(probes.gameObject, scene);
        var positions = new List<Vector3>();
        foreach (float z in new[] { 6.6f, 8f, 9.3f })
            foreach (float y in new[] { -3.2f, -1.6f, 0f, 1.6f, 3.2f })
                foreach (float x in new[] { -4.2f, -2.1f, 0f, 2.1f, 4.2f }) positions.Add(new Vector3(x, y, z));
        probes.probePositions = positions.ToArray();
        // Independent lighting settings/data: no writes to the source scene's bake assets.
        var settings = UnityEngine.Object.Instantiate(Lightmapping.lightingSettings);
        settings.name = "CameraLightingV10"; settings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
        settings.lightmapResolution = 12; settings.directSampleCount = 32;
        settings.indirectSampleCount = 128; settings.environmentSampleCount = 64;
        settings.maxBounces = 3; settings.realtimeGI = false; settings.bakedGI = true;
        AssetDatabase.CreateAsset(settings, Root + "/CameraLightingV10.lighting");
        Lightmapping.lightingSettings = settings;
        Lightmapping.lightingDataAsset = null;
        LightmapSettings.lightmaps = Array.Empty<LightmapData>();
        if (!snapshot.SequenceEqual(protectedComponents.Select(c => EditorJsonUtility.ToJson(c))))
            throw new InvalidOperationException("Protected gameplay/UI/collision serialization changed.");
        ValidateAim(scene);
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Could not save copied scene.");
        // The learner's retry uses the active scene's build index; append without reordering existing entries.
        if (!EditorBuildSettings.scenes.Any(s => s.path == Destination))
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(Destination, true) }).ToArray();
        Debug.Log("CAMERA_LIGHTING_COPY_OK: gameplay/UI/colliders preserved; independent bake required.");
        BakeAndCapture();
    }

    [MenuItem("BalloonBarrage/Art/Rebake camera and lighting scene copy")]
    public static void BakeAndCapture()
    {
        if (Application.isPlaying) throw new InvalidOperationException("Stop Play Mode before baking.");
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene(Destination);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Copied-scene lightmap bake failed.");
        if (LightmapSettings.lightmaps.Length == 0 || LightmapSettings.lightProbes == null ||
            LightmapSettings.lightProbes.count < 75) throw new InvalidOperationException("Lightmaps/probes missing.");
        var gun = All<PlayerShooter>(scene).Single().gameObject;
        bool active = gun.activeSelf;
        var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection");
        string reflection = Root + "/Reflections/Studio.exr";
        try
        {
            gun.SetActive(false);
            if (!Lightmapping.BakeReflectionProbe(probe, reflection)) throw new IOException("Reflection bake failed.");
        }
        finally { gun.SetActive(active); }
        AssetDatabase.ImportAsset(reflection, ImportAssetOptions.ForceSynchronousImport);
        probe.mode = ReflectionProbeMode.Custom;
        probe.customBakedTexture = AssetDatabase.LoadAssetAtPath<Cubemap>(reflection);
        Record(probe);
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        VerifySaved();
    }

    public static void VerifySaved()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        ValidateAim(scene);
        int missing = All<Transform>(scene).Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
        if (missing != 0 || LightmapSettings.lightmaps.Length == 0 || LightmapSettings.lightProbes == null ||
            LightmapSettings.lightProbes.count < 75) throw new InvalidOperationException("Saved scene validation failed.");
        Capture(scene, "After_Room"); CaptureTargets(scene, "After_Targets");
        AssetDatabase.Refresh();
        Debug.Log("CAMERA_LIGHTING_SAVED_OK: reloaded scene; " + LightmapSettings.lightmaps.Length +
            " lightmaps; " + LightmapSettings.lightProbes.count + " probes; aim projection and shot paths checked.");
    }

    public static void RegisterCopiedScene()
    {
        if (!File.Exists(Destination)) throw new FileNotFoundException(Destination);
        if (!EditorBuildSettings.scenes.Any(s => s.path == Destination))
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(Destination, true) }).ToArray();
    }

    static void WriteAudit(Scene scene, UniversalRenderPipelineAsset pipeline)
    {
        var camera = GameCamera(scene);
        var lines = new List<string> { "Unity: " + Application.unityVersion, "URP asset: " + AssetDatabase.GetAssetPath(pipeline),
            "Color space: " + QualitySettings.activeColorSpace, "Camera: " + camera.transform.position +
            ", Euler " + camera.transform.eulerAngles + ", FOV " + camera.fieldOfView,
            "Existing lightmaps: " + LightmapSettings.lightmaps.Length,
            "Existing probes: " + (LightmapSettings.lightProbes == null ? 0 : LightmapSettings.lightProbes.count) };
        lines.AddRange(All<Light>(scene).Select(l => $"Light: {l.name}, {l.type}, {l.lightmapBakeType}, intensity {l.intensity}, shadows {l.shadows}, position {l.transform.position}"));
        lines.AddRange(All<Renderer>(scene).SelectMany(r => r.sharedMaterials).Where(m => m != null).Distinct()
            .Select(m => "Material: " + AssetDatabase.GetAssetPath(m) + " | shader " + m.shader.name));
        File.WriteAllLines(Root + "/SourceAudit.txt", lines);
    }

    public static void Capture(Scene scene, string name)
    {
        var camera = GameCamera(scene); var oldTarget = camera.targetTexture;
        var oldActive = RenderTexture.active; float oldAspect = camera.aspect;
        bool async = ShaderUtil.allowAsyncCompilation;
        var target = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        try
        {
            ShaderUtil.allowAsyncCompilation = false; target.Create(); camera.targetTexture = target;
            camera.aspect = 16f / 9f; camera.Render();
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
            if (RenderPipeline.SupportsRenderRequest(camera, request)) RenderPipeline.SubmitRenderRequest(camera, request);
            else camera.Render();
            RenderTexture.active = target; pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); pixels.Apply();
            File.WriteAllBytes(Root + "/Previews/" + name + ".png", pixels.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = oldTarget; camera.aspect = oldAspect; RenderTexture.active = oldActive;
            ShaderUtil.allowAsyncCompilation = async; target.Release();
            UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(pixels);
        }
    }

    static void CaptureTargets(Scene scene, string name)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayableBalloon.prefab");
        var targets = new List<GameObject>();
        try
        {
            foreach (var position in new[] { new Vector3(-2, -.5f, 8), new Vector3(0, .9f, 8), new Vector3(2, -.5f, 8) })
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                targets.Add(target); target.transform.position = position; target.SetActive(true);
            }
            Capture(scene, name);
        }
        finally { foreach (var target in targets) UnityEngine.Object.DestroyImmediate(target); }
    }

    static void ValidateAim(Scene scene)
    {
        var camera = GameCamera(scene); float aspect = camera.aspect; camera.aspect = 16f / 9f;
        var shooter = All<PlayerShooter>(scene).Single(); var so = new SerializedObject(shooter);
        var muzzle = so.FindProperty("muzzle").objectReferenceValue as Transform;
        var projectile = so.FindProperty("paintProjectilePrefab").objectReferenceValue as PaintProjectile;
        if (muzzle == null || projectile == null || so.FindProperty("muzzleSpray").objectReferenceValue == null)
            throw new InvalidOperationException("Shooter references missing.");
        var spawner = new SerializedObject(All<BalloonSpawner>(scene).Single());
        if (spawner.FindProperty("maxBalloons").intValue != 3) throw new InvalidOperationException("Expected three targets.");
        var prefab = spawner.FindProperty("balloonPrefab").objectReferenceValue as GameObject;
        var motion = new SerializedObject(prefab.GetComponent<BalloonFloat>());
        var xs = motion.FindProperty("xRange").vector2Value; var ys = motion.FindProperty("yRange").vector2Value;
        float z = spawner.FindProperty("zPosition").floatValue;
        Quaternion rotation = shooter.transform.localRotation;
        var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        try
        {
            foreach (float x in new[] { xs.x, 0, xs.y }) foreach (float y in new[] { ys.x, 0, ys.y })
            {
                target.transform.position = new Vector3(x, y, z); Physics.SyncTransforms();
                var body = target.GetComponentsInChildren<Collider>().First(c => !c.isTrigger && c.enabled);
                var point = body.bounds.center;
                var bounds = target.GetComponentsInChildren<Renderer>().Aggregate(new Bounds(point, Vector3.zero),
                    (b, r) => { b.Encapsulate(r.bounds); return b; });
                foreach (int sx in new[] { -1, 1 }) foreach (int sy in new[] { -1, 1 })
                {
                    var p = camera.WorldToViewportPoint(bounds.center + Vector3.Scale(bounds.extents, new Vector3(sx, sy, -1)));
                    if (p.z <= camera.nearClipPlane || p.x < .025f || p.x > .975f || p.y < .025f || p.y > .975f)
                        throw new InvalidOperationException("Target silhouette outside framing at " + target.transform.position + " viewport " + p);
                }
                var screen = camera.WorldToScreenPoint(point); var ray = camera.ScreenPointToRay(screen);
                if (!Physics.Raycast(ray, out var aimHit, 100, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
                    aimHit.collider.GetComponentInParent<Balloon>() != target.GetComponent<Balloon>())
                    throw new InvalidOperationException("Camera aim blocked at " + point);
                shooter.transform.LookAt(aimHit.point);
                if (camera.WorldToViewportPoint(muzzle.position).z < camera.nearClipPlane)
                    throw new InvalidOperationException("Muzzle behind near clip plane.");
                float radius = new SerializedObject(projectile).FindProperty("radius").floatValue;
                Vector3 direction = (aimHit.point - muzzle.position).normalized;
                if (!Physics.SphereCast(muzzle.position, radius, direction, out var shotHit, 100,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
                    shotHit.collider.GetComponentInParent<Balloon>() != target.GetComponent<Balloon>())
                    throw new InvalidOperationException("Projectile path blocked at " + point);
            }
        }
        finally { shooter.transform.localRotation = rotation; camera.aspect = aspect; UnityEngine.Object.DestroyImmediate(target); }
    }
}
