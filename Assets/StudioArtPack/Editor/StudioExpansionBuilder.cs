using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// Additive art authoring only. Never writes the production scene or build settings.
public static class StudioExpansionBuilder
{
    public const string Root = "Assets/StudioArtPack/StudioExpansion20261028";
    public const string ScenePath = "Assets/Scenes/StudioExpansion20261028.unity";
    public const string GalleryPath = Root + "/StudioPropGallery.unity";
    const string Production = "Assets/Scenes/PrototypeScene.unity";
    static Material oak, ivory, teal, coral, gold, ink, steel, lilac, mint;
    static readonly List<string> report = new List<string>();
    static string productionHash;
    static int sourceParts;
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static void Require(bool condition, string reason) { if (!condition) throw new InvalidOperationException(reason); }
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var color); return color; }
    static string Hash(string path) { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant(); }

    public static void Run()
    {
        Require(Application.isBatchMode, "Run in batch mode with the interactive editor closed.");
        AfterFrames(() =>
        {
            Require(!File.Exists(ScenePath) && !File.Exists(GalleryPath) && !Directory.Exists(Root + "/Prefabs"), "Preserve delivered art; this builder creates a new pack once.");
            productionHash = Hash(Production);
            foreach (var folder in new[] { "Materials", "Meshes", "Prefabs", "Previews" }) Directory.CreateDirectory(Root + "/" + folder);
            AssetDatabase.Refresh();
            MakeMaterials();
            PrepareCourtyardMaterial();
            SavePrefab("SupplyTrolley", Trolley);
            SavePrefab("RollerTray", Tray);
            SavePrefab("CanvasEasel", Easel);
            SavePrefab("PigmentSwatchRail", Swatches);
            SavePrefab("CanvasDryingRack", Rack);
            SavePrefab("FramedRhythmPrint", t => Print(t, "StudioRhythmPrint"));
            SavePrefab("FramedBotanicalPrint", t => Print(t, "BotanicalPigmentPrint"));
            AssetDatabase.SaveAssets();
            BuildStudio();
            AfterFrames(() =>
            {
                var scene = EditorSceneManager.OpenScene(ScenePath);
                Capture(scene, "StudioExpansion", 1920, 1080);
                CaptureDetails(scene);
                BuildGallery();
                AfterFrames(() =>
                {
                    var gallery = EditorSceneManager.OpenScene(GalleryPath);
                    Capture(gallery, "PropCollection", 1920, 1200);
                    Verify();
                    File.WriteAllLines(Root + "/Verification.txt", report);
                    AssetDatabase.Refresh();
                    Debug.Log("STUDIO_EXPANSION_OK: seven native prop prefabs, two saved art scenes, protected production scene and no added runtime behaviours.");
                    EditorApplication.Exit(0);
                });
            });
        });
    }

    static void AfterFrames(Action action)
    {
        int frames = 0;
        EditorApplication.CallbackFunction callback = null;
        callback = () =>
        {
            if (++frames < 30) return;
            EditorApplication.update -= callback;
            try { action(); } catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
        };
        EditorApplication.update += callback;
    }

    static Material Mat(string name, string hex, float smoothness = .22f)
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
        material.SetColor("_BaseColor", Hex(hex)); material.SetFloat("_Smoothness", smoothness);
        AssetDatabase.CreateAsset(material, Root + "/Materials/" + name + ".mat"); return material;
    }
    static void MakeMaterials()
    {
        oak = Mat("HoneyJoinery", "BE956D"); ivory = Mat("WarmCanvas", "F0E4CC", .12f);
        teal = Mat("EnamelTeal", "299B9C", .35f); coral = Mat("PigmentCoral", "EB7B68", .3f);
        gold = Mat("PigmentOchre", "E6B74C", .3f); ink = Mat("WorkshopCharcoal", "35494A");
        steel = Mat("BrushedSteel", "B7C3BF", .35f); lilac = Mat("PigmentLilac", "9B84BB", .3f); mint = Mat("PigmentSage", "89B49B", .3f);
        steel.SetFloat("_Metallic", .5f);
    }
    static void PrepareCourtyardMaterial()
    {
        string texturePath = Root + "/Textures/CourtyardAutumnView.png";
        Require(File.Exists(texturePath), "Missing courtyard source artwork.");
        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath);
        importer.wrapMode = TextureWrapMode.Clamp; importer.mipmapEnabled = true; importer.maxTextureSize = 2048; importer.sRGBTexture = true; importer.SaveAndReimport();
        string path = Root + "/Materials/CourtyardAutumnView.mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) return;
        var material = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { name = "CourtyardAutumnView" };
        material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath));
        material.SetColor("_BaseColor", Color.white); AssetDatabase.CreateAsset(material, path);
    }
    static Transform Group(Transform parent, string name)
    {
        var t = new GameObject(name).transform; if (parent != null) t.SetParent(parent, false); return t;
    }
    static Transform Part(Transform parent, string name, Mesh mesh, Vector3 position, Vector3 scale, Material material, Quaternion? rotation = null)
    {
        var t = Group(parent, name); t.localPosition = position; t.localScale = scale; t.localRotation = rotation ?? Quaternion.identity;
        t.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = t.gameObject.AddComponent<MeshRenderer>(); renderer.sharedMaterial = material;
        renderer.lightProbeUsage = LightProbeUsage.BlendProbes; renderer.shadowCastingMode = ShadowCastingMode.On;
        return t;
    }
    static Transform Box(Transform parent, string name, Vector3 position, Vector3 size, Material material, Quaternion? rotation = null)
        => Part(parent, name, StudioConceptGeometry.RoundedBox(size, Mathf.Min(.025f, Mathf.Min(size.x, Mathf.Min(size.y, size.z)) * .2f)), position, Vector3.one, material, rotation);
    static Transform Cylinder(Transform parent, string name, Vector3 position, float radius, float height, Material material, Quaternion? rotation = null)
    {
        var mesh = StudioConceptGeometry.Lathe(new[] { new Vector2(0, -.5f), new Vector2(.97f, -.5f), new Vector2(1, -.46f), new Vector2(1, .46f), new Vector2(.97f, .5f), new Vector2(0, .5f) });
        return Part(parent, name, mesh, position, new Vector3(radius, height, radius), material, rotation);
    }
    static void Beam(Transform parent, string name, Vector3 a, Vector3 b, float width, Material material)
        => Box(parent, name, (a + b) / 2, new Vector3(width, (b - a).magnitude, width), material, Quaternion.FromToRotation(Vector3.up, b - a));
    static void Tin(Transform parent, Vector3 bottom, Material pigment, float radius = .14f, float height = .30f)
    {
        Cylinder(parent, "PaintTin", bottom + Vector3.up * height / 2, radius, height, pigment);
        Cylinder(parent, "TinLid", bottom + Vector3.up * (height + .012f), radius * 1.04f, .024f, steel);
        Cylinder(parent, "LidPigment", bottom + Vector3.up * (height + .026f), radius * .80f, .008f, pigment);
        Box(parent, "TinLabel", bottom + new Vector3(0, height * .53f, -radius - .002f), new Vector3(radius * 1.15f, height * .44f, .008f), ivory);
    }
    static void Trolley(Transform root)
    {
        foreach (float y in new[] { .29f, 1.04f })
        {
            Box(root, "Shelf", new Vector3(0, y, 0), new Vector3(1.5f, .065f, .8f), teal);
            foreach (float z in new[] { -.40f, .40f }) Box(root, "ShelfLip", new Vector3(0, y + .08f, z), new Vector3(1.5f, .12f, .045f), teal);
            foreach (float x in new[] { -.75f, .75f }) Box(root, "ShelfLip", new Vector3(x, y + .08f, 0), new Vector3(.045f, .12f, .8f), teal);
        }
        foreach (float x in new[] { -.67f, .67f }) foreach (float z in new[] { -.31f, .31f })
        {
            Box(root, "Upright", new Vector3(x, .72f, z), new Vector3(.06f, 1.16f, .06f), steel);
            Cylinder(root, "Caster", new Vector3(x, .12f, z), .12f, .085f, ink, Quaternion.Euler(0, 0, 90));
        }
        Beam(root, "Handle", new Vector3(.67f, 1.32f, -.31f), new Vector3(.67f, 1.32f, .31f), .075f, oak);
        Tin(root, new Vector3(-.38f, 1.085f, 0), coral); Tin(root, new Vector3(-.03f, 1.085f, 0), gold); Tin(root, new Vector3(.32f, 1.085f, 0), teal);
        Box(root, "Sketchbook", new Vector3(-.22f, .40f, .02f), new Vector3(.65f, .1f, .5f), ivory, Quaternion.Euler(0, -8, 0));
        Box(root, "FoldedCloth", new Vector3(.40f, .43f, .02f), new Vector3(.35f, .17f, .53f), mint);
    }
    static void Tray(Transform root)
    {
        Box(root, "TrayBase", new Vector3(0, .035f, 0), new Vector3(.88f, .07f, 1.10f), ink);
        foreach (float x in new[] { -.44f, .44f }) Box(root, "TrayRim", new Vector3(x, .09f, 0), new Vector3(.045f, .12f, 1.10f), ink);
        foreach (float z in new[] { -.55f, .55f }) Box(root, "TrayRim", new Vector3(0, .09f, z), new Vector3(.88f, .12f, .045f), ink);
        Box(root, "PigmentPool", new Vector3(0, .08f, .25f), new Vector3(.78f, .012f, .47f), coral);
        for (int i = 0; i < 6; i++) Box(root, "LoadingRidge", new Vector3(0, .086f + i * .006f, -.40f + i * .075f), new Vector3(.74f, .025f, .018f), steel);
        Cylinder(root, "RollerNap", new Vector3(0, .23f, -.04f), .095f, .58f, ivory, Quaternion.Euler(0, 0, 90));
        Beam(root, "RollerArm", new Vector3(.33f, .23f, -.04f), new Vector3(.33f, .23f, -.37f), .025f, steel);
        Beam(root, "RollerArm", new Vector3(.33f, .23f, -.37f), new Vector3(0, .23f, -.37f), .025f, steel);
        Cylinder(root, "RollerGrip", new Vector3(0, .23f, -.58f), .055f, .36f, teal, Quaternion.Euler(90, 0, 0));
    }
    static void Easel(Transform root)
    {
        Beam(root, "LeftLeg", new Vector3(-.55f, 0, 0), new Vector3(-.18f, 2.35f, .16f), .085f, oak);
        Beam(root, "RightLeg", new Vector3(.55f, 0, 0), new Vector3(.18f, 2.35f, .16f), .085f, oak);
        Beam(root, "RearLeg", new Vector3(0, 0, .65f), new Vector3(0, 2.25f, .18f), .09f, oak);
        Box(root, "LowerBrace", new Vector3(0, .36f, .04f), new Vector3(.95f, .075f, .09f), oak);
        Box(root, "CanvasShelf", new Vector3(0, .72f, -.075f), new Vector3(1.30f, .09f, .30f), oak);
        Box(root, "CanvasBacking", new Vector3(0, 1.41f, .03f), new Vector3(1.12f, 1.28f, .075f), ivory);
        // Raised, original pigment studies, not a gameplay target or rhythm indicator.
        Cylinder(root, "CoralStudy", new Vector3(-.18f, 1.66f, -.017f), .21f, .008f, coral, Quaternion.Euler(90, 0, 0));
        Cylinder(root, "TealStudy", new Vector3(.17f, 1.38f, -.024f), .24f, .008f, teal, Quaternion.Euler(90, 0, 0));
        Cylinder(root, "OchreStudy", new Vector3(-.20f, 1.15f, -.030f), .13f, .008f, gold, Quaternion.Euler(90, 0, 0));
        Box(root, "TopClamp", new Vector3(0, 2.10f, -.02f), new Vector3(.32f, .07f, .18f), oak);
    }
    static void Swatches(Transform root)
    {
        Box(root, "Rail", new Vector3(0, .52f, .025f), new Vector3(1.75f, .07f, .075f), oak);
        var pigments = new[] { teal, coral, gold, lilac, mint };
        for (int i = 0; i < pigments.Length; i++)
        {
            float x = (i - 2) * .32f;
            Box(root, "CanvasTag", new Vector3(x, .27f, 0), new Vector3(.25f, .40f, .025f), ivory, Quaternion.Euler(0, 0, (i % 2 == 0 ? -3 : 3)));
            Box(root, "PigmentSwatch", new Vector3(x, .25f, -.018f), new Vector3(.19f, .22f, .008f), pigments[i]);
            Box(root, "BrassClip", new Vector3(x, .49f, -.026f), new Vector3(.055f, .10f, .045f), gold);
        }
    }
    static void Rack(Transform root)
    {
        foreach (float x in new[] { -.61f, .61f }) foreach (float z in new[] { -.28f, .28f })
            Box(root, "Upright", new Vector3(x, .88f, z), new Vector3(.065f, 1.76f, .065f), oak);
        for (int i = 0; i < 5; i++)
        {
            float y = .18f + i * .32f;
            Box(root, "DryingShelf", new Vector3(0, y, 0), new Vector3(1.28f, .04f, .65f), steel);
            Box(root, "CanvasStack", new Vector3(-.03f, y + .06f, -.015f), new Vector3(1.10f, .07f, .54f), ivory, Quaternion.Euler(0, i % 2 == 0 ? -3 : 3, 0));
            Box(root, "ColoredEdge", new Vector3(.16f, y + .10f, -.29f), new Vector3(.5f, .024f, .018f), new[] { teal, coral, gold, mint, lilac }[i]);
        }
        Box(root, "TopRail", new Vector3(0, 1.76f, .28f), new Vector3(1.28f, .065f, .065f), oak);
    }
    static void Print(Transform root, string textureName)
    {
        string path = Root + "/Textures/" + textureName + ".png";
        Require(File.Exists(path), "Generated print is required: " + path);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.wrapMode = TextureWrapMode.Clamp; importer.mipmapEnabled = true; importer.maxTextureSize = 2048; importer.sRGBTexture = true; importer.SaveAndReimport();
        var material = Mat(textureName, "FFFFFF", .10f); material.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(path));
        Box(root, "Backing", new Vector3(0, .7f, .035f), new Vector3(1.4f, 1.4f, .07f), ivory);
        foreach (float x in new[] { -.68f, .68f }) Box(root, "Frame", new Vector3(x, .7f, -.025f), new Vector3(.065f, 1.43f, .09f), oak);
        foreach (float y in new[] { .02f, 1.38f }) Box(root, "Frame", new Vector3(0, y, -.025f), new Vector3(1.3f, .065f, .09f), oak);
        var quad = new Mesh(); quad.vertices = new[] { new Vector3(-.5f, -.5f, 0), new Vector3(-.5f, .5f, 0), new Vector3(.5f, .5f, 0), new Vector3(.5f, -.5f, 0) };
        quad.uv = new[] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right }; quad.triangles = new[] { 0, 1, 2, 0, 2, 3 }; quad.RecalculateNormals(); quad.RecalculateBounds();
        Part(root, "OriginalArtwork", quad, new Vector3(0, .7f, -.012f), new Vector3(1.28f, 1.28f, 1), material);
    }

    static void SavePrefab(string name, Action<Transform> build)
    {
        var root = Group(null, name); build(root);
        var renderers = root.GetComponentsInChildren<MeshRenderer>(); sourceParts += renderers.Length;
        var materials = renderers.Select(r => r.sharedMaterial).Distinct().ToArray();
        var submeshes = new List<Mesh>();
        foreach (var material in materials)
        {
            var entries = renderers.Where(r => r.sharedMaterial == material).Select(r => new CombineInstance { mesh = r.GetComponent<MeshFilter>().sharedMesh, transform = root.worldToLocalMatrix * r.transform.localToWorldMatrix }).ToArray();
            var mesh = new Mesh { indexFormat = IndexFormat.UInt32 }; mesh.CombineMeshes(entries, true, true); submeshes.Add(mesh);
        }
        var combined = new Mesh { name = name, indexFormat = IndexFormat.UInt32 };
        combined.CombineMeshes(submeshes.Select(m => new CombineInstance { mesh = m, transform = Matrix4x4.identity }).ToArray(), false, false);
        combined.RecalculateBounds();
        AssetDatabase.CreateAsset(combined, Root + "/Meshes/" + name + ".asset");
        foreach (var renderer in renderers) Object.DestroyImmediate(renderer.gameObject);
        foreach (var mesh in submeshes) Object.DestroyImmediate(mesh);
        root.gameObject.AddComponent<MeshFilter>().sharedMesh = combined;
        var final = root.gameObject.AddComponent<MeshRenderer>(); final.sharedMaterials = materials; final.lightProbeUsage = LightProbeUsage.BlendProbes;
        PrefabUtility.SaveAsPrefabAsset(root.gameObject, Root + "/Prefabs/" + name + ".prefab");
        report.Add(name + ": " + renderers.Length + " source pieces -> 1 renderer / " + materials.Length + " material submeshes; " + combined.vertexCount + " vertices; " + combined.triangles.Length / 3 + " triangles.");
        Object.DestroyImmediate(root.gameObject);
    }
    static Transform Place(Transform parent, string name, Vector3 position, float rotation = 0, float scale = 1)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/" + name + ".prefab");
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.gameObject.scene);
        go.transform.SetParent(parent, false); go.transform.localPosition = position; go.transform.localRotation = Quaternion.Euler(0, rotation, 0); go.transform.localScale = Vector3.one * scale;
        return go.transform;
    }
    static void BuildStudio()
    {
        var baseline = EditorSceneManager.OpenScene(Production);
        Capture(baseline, "Before_Production", 1920, 1080);
        Require(AssetDatabase.CopyAsset(Production, ScenePath), "Scene copy failed.");
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var protectedComponents = All<Component>(scene).Where(c => c is Transform || c is MonoBehaviour || c is Collider || c is Rigidbody || c is Camera || c is Light || c is AudioSource).ToArray();
        var before = protectedComponents.Select(EditorJsonUtility.ToJson).ToArray();
        var root = Group(null, "StudioExpansion20261028_ArtStudy");
        Place(root, "SupplyTrolley", new Vector3(5.85f, -4f, 5.30f), -18, 1.12f);
        Place(root, "RollerTray", new Vector3(4.3f, -3.98f, 3.5f), 18, .9f);
        Place(root, "CanvasEasel", new Vector3(-5.9f, -4f, 7.00f), 12, 1.0f);
        Place(root, "CanvasDryingRack", new Vector3(-6.05f, -4f, 2.9f), 16, .90f);
        Place(root, "PigmentSwatchRail", new Vector3(6.86f, .70f, 2.4f), 90, 1.05f);
        Place(root, "FramedRhythmPrint", new Vector3(6.86f, 1.6f, 2.4f), 90, .90f);
        Place(root, "FramedBotanicalPrint", new Vector3(-6.86f, 2.48f, 4.6f), -90, .80f);
        Require(before.SequenceEqual(protectedComponents.Select(EditorJsonUtility.ToJson)), "Existing runtime, camera, transform, audio, physics or lighting data changed.");
        Require(root.GetComponentsInChildren<Collider>(true).Length == 0 && root.GetComponentsInChildren<MonoBehaviour>(true).Length == 0, "Art must not intercept shots or add gameplay.");
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        report.Add("Copied PrototypeScene into independent StudioExpansion20261028; preserved all existing transform/runtime/camera/audio/physics/light serialization. No build-settings change.");
    }
    static void BuildGallery()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientMode = AmbientMode.Trilight; RenderSettings.ambientSkyColor = Hex("D5E2DD"); RenderSettings.ambientEquatorColor = Hex("B0AAA0"); RenderSettings.ambientGroundColor = Hex("7B7366");
        var root = Group(null, "StudioPropCollection");
        Place(root, "SupplyTrolley", new Vector3(0, 0, -.2f), -8);
        Place(root, "CanvasEasel", new Vector3(-3.6f, 0, .65f), 8);
        Place(root, "CanvasDryingRack", new Vector3(3.2f, 0, .5f), -12);
        Place(root, "RollerTray", new Vector3(-1.8f, 0, -.8f), -12, 1.12f);
        Place(root, "PigmentSwatchRail", new Vector3(1.8f, .03f, -.9f), -6, 1.0f);
        Place(root, "FramedRhythmPrint", new Vector3(-1.65f, 1.35f, 1.7f), 0, 1.15f);
        Place(root, "FramedBotanicalPrint", new Vector3(.6f, 1.35f, 1.7f), 0, 1.15f);
        // Native presentation surfaces use built-in meshes; the gallery is excluded from the player build list.
        GalleryCube("GalleryFloor", new Vector3(0, -.10f, .0f), new Vector3(12, .2f, 8), ivory);
        GalleryCube("GalleryWall", new Vector3(0, 2, 1.88f), new Vector3(12, 4.2f, .12f), mint);
        var camera = Group(null, "GalleryCamera").gameObject.AddComponent<Camera>(); camera.tag = "MainCamera";
        camera.transform.position = new Vector3(5.2f, 4.3f, -10f); camera.transform.LookAt(new Vector3(0, 1.15f, .35f));
        camera.orthographic = true; camera.orthographicSize = 3.2f; camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Hex("E5DBCA"); camera.nearClipPlane = .1f; camera.farClipPlane = 100;
        camera.gameObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;
        var light = Group(null, "GalleryKeyLight").gameObject.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1.8f; light.color = Hex("FFF3DA"); light.shadows = LightShadows.Soft; light.transform.rotation = Quaternion.Euler(45, -35, 0);
        EditorSceneManager.SaveScene(scene, GalleryPath);
    }
    static void GalleryCube(string name, Vector3 position, Vector3 scale, Material material)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; Object.DestroyImmediate(go.GetComponent<Collider>());
        go.transform.position = position; go.transform.localScale = scale; go.GetComponent<MeshRenderer>().sharedMaterial = material;
    }
    static void CaptureDetails(Scene scene)
    {
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera")); var position = camera.transform.position; var rotation = camera.transform.rotation;
        var children = camera.transform.Cast<Transform>().Select(t => t.gameObject).ToArray(); var states = children.Select(g => g.activeSelf).ToArray();
        try
        {
            foreach (var child in children) child.SetActive(false);
            camera.transform.position = new Vector3(3.8f, -1.3f, .6f); camera.transform.LookAt(new Vector3(6.0f, -.9f, 6f)); Capture(scene, "StudioExpansion_Detail", 1600, 1200);
        }
        finally { camera.transform.position = position; camera.transform.rotation = rotation; for (int i = 0; i < children.Length; i++) children[i].SetActive(states[i]); }
    }
    static void Capture(Scene scene, string name, int width, int height)
    {
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var canvases = All<Canvas>(scene); var states = canvases.Select(c => c.gameObject.activeSelf).ToArray();
        foreach (var canvas in canvases) canvas.gameObject.SetActive(false);
        var old = camera.targetTexture; var active = RenderTexture.active; float aspect = camera.aspect; bool async = ShaderUtil.allowAsyncCompilation;
        var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32); var pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
        try
        {
            ShaderUtil.allowAsyncCompilation = false; target.Create(); camera.targetTexture = target; camera.aspect = width / (float)height;
            for (int i = 0; i < 3; i++) { camera.Render(); RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = target }); }
            RenderTexture.active = target; pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0); pixels.Apply();
            File.WriteAllBytes(Root + "/Previews/" + name + ".png", pixels.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = old; camera.aspect = aspect; RenderTexture.active = active; ShaderUtil.allowAsyncCompilation = async;
            target.Release(); Object.DestroyImmediate(target); Object.DestroyImmediate(pixels);
            for (int i = 0; i < canvases.Length; i++) canvases[i].gameObject.SetActive(states[i]);
        }
    }
    static void Verify()
    {
        Require(Hash(Production) == productionHash, "Production scene bytes changed.");
        foreach (var path in Directory.GetFiles(Root + "/Prefabs", "*.prefab"))
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Require(prefab != null && prefab.GetComponentsInChildren<Collider>(true).Length == 0 && prefab.GetComponentsInChildren<MonoBehaviour>(true).Length == 0, "Invalid prefab: " + path);
            foreach (var filter in prefab.GetComponentsInChildren<MeshFilter>()) Require(filter.sharedMesh != null && filter.sharedMesh.vertexCount > 0, "Missing mesh: " + path);
            foreach (var renderer in prefab.GetComponentsInChildren<MeshRenderer>()) Require(renderer.sharedMaterials.All(m => m != null && m.shader != null && m.shader.isSupported), "Missing or unsupported material: " + path);
        }
        foreach (var path in new[] { ScenePath, GalleryPath })
        {
            var scene = EditorSceneManager.OpenScene(path);
            Require(All<Transform>(scene).Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)) == 0, "Missing scene script: " + path);
        }
        report.Add("Production scene SHA-256 unchanged: " + productionHash);
        report.Add(sourceParts + " authoring parts combined into seven prefab renderers. This is a renderer-count reduction, not a measured FPS result.");
        report.Add("All seven prefabs reloaded with nonempty meshes, supported materials, zero colliders and zero runtime scripts. Both saved scenes reloaded with no missing scripts.");
        report.Add("Previews are Unity 6000.0.77f1 Edit Mode camera renders. No physical playtest, new gameplay, standalone build or release acceptance is claimed. Existing baked lighting was retained; new props use probes/realtime lighting.");
    }

    // One bounded review correction for the first delivered draft; never regenerates prefab GUIDs.
    public static void FinishPlacementReview()
    {
        Require(Application.isBatchMode && !File.Exists(Root + "/PositionReview.txt"), "Preserve subsequent manual scene edits.");
        AfterFrames(() =>
        {
            productionHash = Hash(Production);
            PrepareCourtyardMaterial();
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var root = scene.GetRootGameObjects().Single(g => g.name == "StudioExpansion20261028_ArtStudy").transform;
            void Move(string name, Vector3 position, float rotation, float scale)
            {
                var t = root.Find(name); Require(t != null, "Missing art placement: " + name);
                t.localPosition = position; t.localRotation = Quaternion.Euler(0, rotation, 0); t.localScale = Vector3.one * scale;
                PrefabUtility.RecordPrefabInstancePropertyModifications(t);
            }
            Move("PigmentSwatchRail", new Vector3(6.86f, .70f, 2.4f), 90, 1.05f);
            Move("FramedRhythmPrint", new Vector3(6.86f, 1.6f, 2.4f), 90, .90f);
            Move("FramedBotanicalPrint", new Vector3(-6.86f, 2.48f, 4.6f), -90, .80f);
            AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
            AfterFrames(() =>
            {
                scene = EditorSceneManager.OpenScene(ScenePath); Capture(scene, "StudioExpansion", 1920, 1080); CaptureDetails(scene);
                Require(Hash(Production) == productionHash, "Production scene bytes changed.");
                File.WriteAllText(Root + "/PositionReview.txt", "Corrected both frame normals to face inward. Moved the rhythm print/swatch rail forward of the existing right pegboard and the botanical print above the left window, preserving the central target area.\nBoth final scene captures were regenerated from the reloaded saved scene. Gallery capture and all prefab GUIDs remain unchanged.\nCourtyardAutumnView is a prepared, unused optional URP/Unlit material with sRGB, Clamp and mipmapped texture import. Production window is unchanged.\nProduction scene SHA-256 unchanged: " + productionHash + "\n");
                AssetDatabase.Refresh(); Debug.Log("STUDIO_EXPANSION_PLACEMENT_REVIEW_OK"); EditorApplication.Exit(0);
            });
        });
    }
}
