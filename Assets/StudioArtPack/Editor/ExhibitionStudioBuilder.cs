using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// Editor-only environment assembly. Gameplay remains learner-owned.
public static class ExhibitionStudioBuilder
{
    private const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    private const string Root = "Assets/StudioArtPack/ExhibitionV1";
    private const string GroupName = "ExhibitionStudioV1";
    private static string PreviewDir => Path.GetFullPath(Path.Combine(Application.dataPath, "../.local-backups/ExhibitionStudioPreview"));

    [MenuItem("Tools/Balloon Studio/Build Exhibition Studio V1")]
    public static void BuildMenu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { Debug.LogWarning("Stop Play Mode before assembling the studio."); return; }
        if (SceneManager.GetActiveScene().path != ScenePath)
        { Debug.LogWarning("Open PrototypeScene before assembling the studio."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build(SceneManager.GetActiveScene());
    }

    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use the menu in interactive Unity.");
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Directory.CreateDirectory(PreviewDir);
        Capture(scene, "before.png");
        Build(scene);
        Capture(scene, "after.png");
        CaptureTargetLayout(scene);
        ValidateAim(scene);
        Debug.Log("EXHIBITION_STUDIO_OK: scene saved; environment, gun, references and aim previews checked.");
    }

    private static void Build(Scene scene)
    {
        if (scene.GetRootGameObjects().Any(go => go.name == GroupName))
            throw new InvalidOperationException("Studio V1 already exists; preserve manual edits and adjust its objects directly.");
        string backup = Path.GetFullPath(Path.Combine(Application.dataPath, "../.local-backups/scene-before-exhibition-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".unity"));
        Directory.CreateDirectory(Path.GetDirectoryName(backup));
        File.Copy(ScenePath, backup, false);
        File.Copy(ScenePath + ".meta", backup + ".meta", false);
        Directory.CreateDirectory(Root + "/Materials");
        Directory.CreateDirectory(Root + "/Prefabs");
        AssetDatabase.Refresh();

        var camera = Find<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var gun = Find<PlayerShooter>(scene).Single().transform;
        if (gun.parent != camera.transform) throw new InvalidOperationException("Expected the gun to remain a direct camera child.");
        gun.localPosition = new Vector3(.42f, -.35f, 1.15f);
        gun.localScale = Vector3.one * .65f;
        gun.LookAt(camera.transform.TransformPoint(new Vector3(0, 0, 16)));
        PrefabUtility.RecordPrefabInstancePropertyModifications(gun);

        var wood = AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/HoneyWood.mat");
        var floor = AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/WarmConcrete.mat");
        var cream = AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/Cream.mat");
        var dark = AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/Charcoal.mat");
        var gold = AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/Gold.mat");
        if (new[] { wood, floor, cream, dark, gold }.Any(m => m == null)) throw new InvalidOperationException("Existing studio materials are missing.");
        foreach (var renderer in Find<MeshRenderer>(scene))
        {
            if (new[] { "TopBeam", "LeftColumn", "RightColumn", "Stage" }.Contains(renderer.name)) renderer.sharedMaterial = wood;
            if (renderer.name == "Floor") renderer.sharedMaterial = floor;
        }

        var decor = new GameObject(GroupName).transform;
        SceneManager.MoveGameObjectToScene(decor.gameObject, scene);
        var windowMat = PictureMaterial("WindowView", "WindowCourtyard_v1.png");
        var posterMat = PictureMaterial("BalloonPoster", "BalloonPoster_v1.png");
        var window = new GameObject("CourtyardWindow").transform;
        Part(window, "OutsideView", PrimitiveType.Quad, Vector3.zero, new Vector3(3.3f, 4.4f, 1), windowMat);
        Part(window, "LeftFrame", PrimitiveType.Cube, new Vector3(-1.73f, 0, -.05f), new Vector3(.16f, 4.7f, .16f), wood);
        Part(window, "RightFrame", PrimitiveType.Cube, new Vector3(1.73f, 0, -.05f), new Vector3(.16f, 4.7f, .16f), wood);
        Part(window, "TopFrame", PrimitiveType.Cube, new Vector3(0, 2.27f, -.05f), new Vector3(3.6f, .16f, .16f), wood);
        Part(window, "BottomFrame", PrimitiveType.Cube, new Vector3(0, -2.27f, -.05f), new Vector3(3.6f, .16f, .16f), wood);
        Part(window, "VerticalMullion", PrimitiveType.Cube, new Vector3(0, 0, -.09f), new Vector3(.055f, 4.4f, .08f), dark);
        Part(window, "HorizontalMullion", PrimitiveType.Cube, new Vector3(0, 0, -.09f), new Vector3(3.3f, .055f, .08f), dark);
        SavePlace(window.gameObject, decor, new Vector3(-6.93f, .05f, 4.8f), Quaternion.Euler(0, -90, 0));

        var poster = new GameObject("FramedBalloonPoster").transform;
        Part(poster, "Artwork", PrimitiveType.Quad, new Vector3(0, 0, -.055f), new Vector3(1.15f, 1.15f, 1), posterMat);
        Part(poster, "Backing", PrimitiveType.Cube, Vector3.zero, new Vector3(1.29f, 1.29f, .08f), wood);
        SavePlace(poster.gameObject, decor, new Vector3(5.15f, 1.05f, 9.8f), Quaternion.identity);

        var table = new GameObject("PaintWorkbench").transform;
        Part(table, "Tabletop", PrimitiveType.Cube, new Vector3(0, 1.55f, 0), new Vector3(2.5f, .16f, 1.05f), wood);
        foreach (float x in new[] { -.95f, .95f }) foreach (float z in new[] { -.35f, .35f })
            Part(table, "Leg", PrimitiveType.Cube, new Vector3(x, .75f, z), new Vector3(.12f, 1.5f, .12f), wood);
        Part(table, "LowerShelf", PrimitiveType.Cube, new Vector3(0, .45f, 0), new Vector3(2.15f, .1f, .9f), wood);
        SavePlace(table.gameObject, decor, new Vector3(-5.25f, -4f, 6f), Quaternion.identity);
        PlaceExisting("PaintCans", decor, new Vector3(-5.25f, -2.36f, 6f));
        PlaceExisting("PaintCans", decor, new Vector3(4.75f, -4f, 7.3f));
        PlaceExisting("PottedPlant", decor, new Vector3(5.25f, -4f, 6.8f));
        Part(decor, "LeftSpeakerShelf", PrimitiveType.Cube, new Vector3(-5.1f, -.9f, 9.42f), new Vector3(1.4f, .14f, .9f), wood);
        PlaceExisting("Speaker", decor, new Vector3(-5.1f, -.83f, 9.42f));
        PlaceExisting("PottedPlant", decor, new Vector3(5.15f, -2.8f, 9f), .7f);
        foreach (float x in new[] { -4.1f, 4.1f })
        {
            Part(decor, "RoofBeam", PrimitiveType.Cube, new Vector3(x, 3.7f, 4.5f), new Vector3(.23f, .38f, 10f), wood);
            var fixture = Part(decor, "LampShade", PrimitiveType.Cylinder, new Vector3(x, 2.88f, 9.1f), new Vector3(.32f, .15f, .32f), dark);
            fixture.localRotation = Quaternion.Euler(25, 0, 0);
            Part(decor, "LampLens", PrimitiveType.Sphere, new Vector3(x, 2.74f, 9.05f), new Vector3(.22f, .07f, .22f), gold);
        }

        foreach (var collider in decor.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(collider);
        AddMusic(decor);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed.");
        Debug.Log("GUN_PLACEMENT position=" + gun.localPosition.ToString("F3") + " scale=" + gun.localScale.ToString("F3"));
    }

    private static Material PictureMaterial(string name, string image)
    {
        string imagePath = Root + "/Textures/" + image;
        var importer = AssetImporter.GetAtPath(imagePath) as TextureImporter;
        if (importer == null) throw new IOException("Missing generated image: " + imagePath);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = true;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.SaveAndReimport();
        var shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) throw new InvalidOperationException("Missing URP Unlit shader.");
        var material = new Material(shader) { name = name };
        material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath));
        material.SetColor("_BaseColor", Color.white);
        material.SetFloat("_Cull", 0);
        AssetDatabase.CreateAsset(material, Root + "/Materials/" + name + ".mat");
        return material;
    }

    private static void AddMusic(Transform parent)
    {
        const string musicRoot = "Assets/Audio/Music";
        foreach (string path in Directory.GetFiles(musicRoot, "*.wav"))
        {
            var importer = (AudioImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            bool looping = Path.GetFileNameWithoutExtension(path).EndsWith("_Loop", StringComparison.Ordinal);
            var settings = importer.defaultSampleSettings;
            settings.loadType = looping ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat = looping ? AudioCompressionFormat.Vorbis : AudioCompressionFormat.PCM;
            settings.quality = .7f;
            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            importer.defaultSampleSettings = settings;
            importer.forceToMono = false;
            importer.loadInBackground = looping;
            importer.SaveAndReimport();
        }
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(musicRoot + "/StudioDaylight_Loop.wav");
        if (clip == null) throw new InvalidOperationException("The original background music is missing.");
        var go = new GameObject("StudioBackgroundMusic");
        var audio = go.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.loop = true;
        audio.playOnAwake = true;
        audio.spatialBlend = 0;
        audio.volume = .14f;
        audio.priority = 180;
        SavePlace(go, parent, Vector3.zero, Quaternion.identity);
    }

    private static Transform Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        var renderer = go.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = true;
        return go.transform;
    }

    private static void SavePlace(GameObject source, Transform parent, Vector3 position, Quaternion rotation)
    {
        var prefab = PrefabUtility.SaveAsPrefabAsset(source, Root + "/Prefabs/" + source.name + ".prefab");
        UnityEngine.Object.DestroyImmediate(source);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.transform.localPosition = position;
        instance.transform.localRotation = rotation;
    }

    private static void PlaceExisting(string name, Transform parent, Vector3 position, float scale = 1)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StudioArtPack/Generated/" + name + ".prefab");
        if (prefab == null) throw new IOException("Missing existing decor prefab: " + name);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.transform.localPosition = position;
        instance.transform.localScale = Vector3.one * scale;
    }

    private static T[] Find<T>(Scene scene) where T : Component => scene.GetRootGameObjects()
        .SelectMany(go => go.GetComponentsInChildren<T>(true)).ToArray();

    internal static void Capture(Scene scene, string name)
    {
        var camera = Find<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;
        float previousAspect = camera.aspect;
        bool previousAsync = ShaderUtil.allowAsyncCompilation;
        var target = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGB32);
        var image = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        try
        {
            ShaderUtil.allowAsyncCompilation = false;
            target.Create();
            camera.targetTexture = target;
            camera.aspect = 16f / 9f;
            // Warm up the pipeline before submitting a specific camera request.
            camera.Render();
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
            if (RenderPipeline.SupportsRenderRequest(camera, request)) RenderPipeline.SubmitRenderRequest(camera, request);
            else camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0);
            image.Apply();
            File.WriteAllBytes(Path.Combine(PreviewDir, name), image.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = previousTarget;
            camera.aspect = previousAspect;
            RenderTexture.active = previousActive;
            ShaderUtil.allowAsyncCompilation = previousAsync;
            target.Release();
            UnityEngine.Object.DestroyImmediate(target);
            UnityEngine.Object.DestroyImmediate(image);
        }
    }

    internal static void ValidateAim(Scene scene)
    {
        var shooter = Find<PlayerShooter>(scene).Single();
        var gun = shooter.transform;
        var camera = Find<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var serialized = new SerializedObject(shooter);
        foreach (string field in new[] { "muzzle", "paintProjectilePrefab", "muzzleSpray" })
            if (serialized.FindProperty(field)?.objectReferenceValue == null) throw new InvalidOperationException("Missing shooter reference: " + field);
        if (gun.GetComponentsInChildren<Collider>(true).Length != 0) throw new InvalidOperationException("Gun must not intercept shot queries.");
        if (scene.GetRootGameObjects().Single(go => go.name == GroupName).GetComponentsInChildren<Collider>(true).Length != 0)
            throw new InvalidOperationException("Decor must not intercept shot queries.");
        var music = Find<AudioSource>(scene).Single(source => source.name == "StudioBackgroundMusic");
        if (!music.loop || music.clip == null || music.spatialBlend != 0 || !music.playOnAwake)
            throw new InvalidOperationException("Background music wiring is invalid.");
        var originalRotation = gun.rotation;
        try
        {
            foreach (var point in new[] { new Vector3(-3.3f, -1.2f, 8), new Vector3(-3.3f, 1.4f, 8), new Vector3(3.3f, -1.2f, 8), new Vector3(3.3f, 1.4f, 8) })
            {
                gun.LookAt(point);
                var muzzle = (Transform)serialized.FindProperty("muzzle").objectReferenceValue;
                if (camera.WorldToViewportPoint(muzzle.position).z < camera.nearClipPlane) throw new InvalidOperationException("Muzzle crossed the camera near plane.");
                Capture(scene, "aim-" + point.x.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "-" + point.y.ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + ".png");
            }
        }
        finally { gun.rotation = originalRotation; }
        int missing = Find<Transform>(scene).Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
        if (missing != 0) throw new InvalidOperationException("Missing scene scripts: " + missing);
    }

    internal static void CaptureTargetLayout(Scene scene)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayableBalloon.prefab");
        var targets = new System.Collections.Generic.List<GameObject>();
        var positions = new[] { new Vector3(-1.8f, -.4f, 8), new Vector3(0, .8f, 8), new Vector3(1.8f, -.4f, 8) };
        var colors = new[] { "Teal", "Coral", "Gold" };
        try
        {
            for (int i = 0; i < positions.Length; i++)
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                targets.Add(target);
                target.SetActive(true);
                target.transform.position = positions[i];
                var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/" + colors[i] + ".mat");
                foreach (var renderer in target.GetComponentsInChildren<MeshRenderer>(true))
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(_ => material).ToArray();
            }
            Capture(scene, "target-layout-preview.png");
        }
        finally { foreach (var target in targets) UnityEngine.Object.DestroyImmediate(target); }
    }
}
