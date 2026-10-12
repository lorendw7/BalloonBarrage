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

// One-time environment art pass. Run against a saved, isolated project copy.
public static class StudioCoverFinisher
{
    const string Root = "Assets/StudioArtPack/CoverFinishV9";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    static int meshIndex;
    static Material oak, forest, olive, stem, brass, teal;
    static Mesh leaf;

    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
        if (Directory.Exists(Root)) throw new InvalidOperationException("V9 exists; preserve delivered art and manual edits.");
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var gameplay = All<MonoBehaviour>(scene).Where(IsLearner).ToArray();
        var gameplayBefore = gameplay.Select(EditorJsonUtility.ToJson).ToArray();
        var colliders = All<Collider>(scene);
        var collidersBefore = colliders.Select(EditorJsonUtility.ToJson).ToArray();
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var cameraTransform = EditorJsonUtility.ToJson(camera.transform);
        foreach (var dir in new[] { "Materials", "Meshes", "Textures", "Reflections", "Previews" }) Directory.CreateDirectory(Root + "/" + dir);
        Directory.CreateDirectory(".local-backups/ExhibitionStudioPreview");
        AssetDatabase.Refresh();
        Capture(scene, "Room_Before_v9");
        var group = new GameObject("StudioCoverFinishV9").transform;
        SceneManager.MoveGameObjectToScene(group.gameObject, scene);

        CreateSurfaces();
        RefineExisting(scene);
        BuildCabinet(group);
        BuildVines(group);
        RefineLighting(scene, group);
        RefreshReflection(scene);

        if (!gameplayBefore.SequenceEqual(gameplay.Select(EditorJsonUtility.ToJson))) throw new InvalidOperationException("Learner serialization changed.");
        if (colliders.Length != All<Collider>(scene).Length || !collidersBefore.SequenceEqual(colliders.Select(EditorJsonUtility.ToJson))) throw new InvalidOperationException("Collider settings changed.");
        if (cameraTransform != EditorJsonUtility.ToJson(camera.transform)) throw new InvalidOperationException("Camera framing changed.");
        if (group.GetComponentsInChildren<Collider>(true).Length != 0) throw new InvalidOperationException("Decorative collider found.");
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed.");
        Capture(scene, "Room_GameCamera_v9");
        CaptureTargets(scene);
        ExhibitionStudioBuilder.ValidateAim(scene);
        Debug.Log("COVER_V9_OK: learner components, camera transform and colliders preserved; surfaces, joinery, foliage, lighting and reflection saved.");
    }

    static bool IsLearner(MonoBehaviour c) => c != null && AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(c)).StartsWith("Assets/Scripts/", StringComparison.Ordinal);

    public static void BakeBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use isolated batch mode.");
        var scene = EditorSceneManager.OpenScene(ScenePath);
        string giRoot = Root + "/BakedLighting";
        if (Directory.Exists(giRoot)) throw new InvalidOperationException("Baked lighting exists; preserve the delivered snapshot.");
        Directory.CreateDirectory(giRoot); AssetDatabase.Refresh();
        File.Copy(ScenePath, ".local-backups/v9-before-bake.unity", false);
        var sourceMeshes = new Dictionary<Mesh, Mesh>();
        var environment = All<MeshRenderer>(scene).Where(r => r.enabled && r.gameObject.activeInHierarchy && r.GetComponentInParent<PlayerShooter>() == null).ToArray();
        foreach (var r in environment)
        {
            var filter = r.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null) continue;
            var original = filter.sharedMesh;
            if (!sourceMeshes.TryGetValue(original, out var mesh))
            {
                mesh = UnityEngine.Object.Instantiate(original); mesh.name = original.name + "_LightmapUV";
                if (!Unwrapping.GenerateSecondaryUVSet(mesh)) throw new InvalidOperationException("Lightmap UV generation failed: " + original.name);
                AssetDatabase.CreateAsset(mesh, giRoot + "/" + sourceMeshes.Count.ToString("D3") + "_LightmapMesh.asset");
                sourceMeshes[original] = mesh;
            }
            filter.sharedMesh = mesh; Record(filter);
            GameObjectUtility.SetStaticEditorFlags(r.gameObject, GameObjectUtility.GetStaticEditorFlags(r.gameObject) | StaticEditorFlags.ContributeGI);
            r.receiveGI = ReceiveGI.Lightmaps; Record(r);
        }
        var settings = new LightingSettings { name = "ArtisanStudioLighting" };
        settings.realtimeGI = false; settings.bakedGI = true;
        settings.lightmapper = LightingSettings.Lightmapper.ProgressiveCPU;
        settings.lightmapResolution = 12; settings.lightmapMaxSize = 1024;
        settings.directSampleCount = 32; settings.indirectSampleCount = 128; settings.environmentSampleCount = 64;
        settings.maxBounces = 3; settings.ao = true; settings.aoMaxDistance = .4f;
        settings.aoExponentIndirect = .65f; settings.aoExponentDirect = .15f;
        AssetDatabase.CreateAsset(settings, giRoot + "/ArtisanStudioLighting.lighting"); Lightmapping.lightingSettings = settings;
        var group = scene.GetRootGameObjects().Single(g => g.name == "StudioCoverFinishV9").transform;
        BakedArea(group, "BakedWindowSoftbox", new Vector3(-6.5f, 1.0f, 5.3f), new Vector3(0, -1, 7), new Vector2(3.2f, 4.2f), Hex("FFF0D8"), 4);
        BakedArea(group, "BakedSkylightBounce", new Vector3(0, 3.8f, 4.4f), new Vector3(0, -4, 4.4f), new Vector2(4, 4), Hex("EDF4FA"), 2.5f);
        EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
        Debug.Log("COVER_V9_BAKE_BEGIN: " + environment.Length + " environment renderers, " + sourceMeshes.Count + " lightmap mesh copies.");
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Offline studio lighting bake failed.");
        if (LightmapSettings.lightmaps.Length == 0) throw new InvalidOperationException("No lightmaps produced.");
        RefreshReflection(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        Capture(scene, "Room_GameCamera_v9"); CaptureTargets(scene);
        Debug.Log("COVER_V9_BAKE_OK: " + LightmapSettings.lightmaps.Length + " lightmaps; two offline area lights, no extra realtime lights.");
    }

    static void BakedArea(Transform parent, string name, Vector3 position, Vector3 target, Vector2 size, Color color, float intensity)
    {
        var light = new GameObject(name).AddComponent<Light>(); light.transform.SetParent(parent, false);
        light.transform.position = position; light.transform.LookAt(target); light.type = LightType.Rectangle;
        light.areaSize = size; light.color = color; light.intensity = intensity; light.lightmapBakeType = LightmapBakeType.Baked;
    }
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static T Load<T>(string path) where T : UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new FileNotFoundException(path);
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var color); return color; }
    static Material Clone(string source, string name, string tint)
    {
        var mat = new Material(Load<Material>(source)) { name = name };
        mat.SetColor("_BaseColor", Hex(tint));
        AssetDatabase.CreateAsset(mat, Root + "/Materials/" + name + ".mat");
        return mat;
    }

    static void CreateSurfaces()
    {
        oak = Clone("Assets/StudioArtPack/SurfacesV6/Materials/SatinOak.mat", "NaturalJoinery", "D9BD99");
        oak.SetFloat("_BumpScale", .14f); oak.mainTextureScale = new Vector2(.8f, .8f);
        brass = Clone("Assets/StudioArtPack/SurfacesV6/Materials/SatinBrass.mat", "AgedBrass", "BA9862");
        teal = Clone("Assets/StudioArtPack/SurfacesV6/Materials/TealEnamel.mat", "CabinetTeal", "376D68");
        var plaster = Clone("Assets/StudioArtPack/RoomFinishV7/Materials/CreamPlaster.mat", "MineralCreamPlaster", "F4EADB");
        var heights = new float[512 * 512]; var albedo = new Color[heights.Length]; var normals = new Color[heights.Length];
        for (int y = 0; y < 512; y++) for (int x = 0; x < 512; x++)
        {
            float u = x / 512f, v = y / 512f, tau = Mathf.PI * 2;
            float broad = Mathf.Sin(tau * (u * 3 + .35f * Mathf.Sin(v * tau * 2))) * Mathf.Cos(v * tau * 4);
            float fine = Mathf.Sin(tau * (u * 43 + v * 29)) * Mathf.Sin(tau * (v * 67 - u * 31));
            heights[y * 512 + x] = broad * .17f + fine * .018f;
            float tone = .995f + .0025f * broad + .0015f * fine;
            albedo[y * 512 + x] = new Color(tone, tone, tone, 1);
        }
        for (int y = 0; y < 512; y++) for (int x = 0; x < 512; x++)
        {
            float dx = heights[y * 512 + (x + 1) % 512] - heights[y * 512 + (x + 511) % 512];
            float dy = heights[((y + 1) % 512) * 512 + x] - heights[((y + 511) % 512) * 512 + x];
            var n = new Vector3(-dx * .65f, -dy * .65f, 1).normalized;
            normals[y * 512 + x] = new Color(n.x * .5f + .5f, n.y * .5f + .5f, n.z * .5f + .5f, 1);
        }
        plaster.SetTexture("_BaseMap", Texture("MineralPlaster", albedo, false));
        plaster.SetTexture("_BumpMap", Texture("MineralPlasterNormal", normals, true));
        plaster.SetFloat("_BumpScale", .16f); plaster.EnableKeyword("_NORMALMAP");
        forest = Clone("Assets/StudioArtPack/ReferenceV4/Materials/LeafForest.mat", "FoliageForest", "355E3C");
        olive = Clone("Assets/StudioArtPack/ReferenceV4/Materials/LeafOlive.mat", "FoliageSage", "758A4B");
        stem = Clone("Assets/StudioArtPack/ReferenceV4/Materials/LeafForest.mat", "VineStems", "637345");
        // Fine ribs vary across the blade, with a restrained central vein.
        var leafPixels = new Color[512 * 512];
        for (int y = 0; y < 512; y++) for (int x = 0; x < 512; x++)
        {
            float u = x / 511f, v = y / 511f;
            float rib = Mathf.Pow(Mathf.Max(0, Mathf.Cos((v + Mathf.Abs(u - .5f) * .45f) * Mathf.PI * 36)), 10);
            float vein = Mathf.Exp(-Mathf.Pow((u - .5f) * 100, 2));
            float tone = .86f + .09f * rib + .16f * vein;
            leafPixels[y * 512 + x] = new Color(tone, tone, tone, 1);
        }
        var leafTexture = Texture("LeafVeins", leafPixels, false);
        foreach (var mat in new[] { forest, olive }) { mat.SetTexture("_BaseMap", leafTexture); mat.SetFloat("_Smoothness", .32f); mat.SetFloat("_Cull", 0); }
        leaf = SaveMesh(LeafMesh(false), "BotanicalLeaf");
    }

    static Texture2D Texture(string name, Color[] pixels, bool normal)
    {
        var data = new Texture2D(512, 512, TextureFormat.RGBA32, false, normal);
        data.SetPixels(pixels); data.Apply();
        string path = Root + "/Textures/" + name + ".png";
        File.WriteAllBytes(path, data.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(data);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
        importer.sRGBTexture = !normal; importer.wrapMode = TextureWrapMode.Repeat;
        importer.mipmapEnabled = true; importer.filterMode = FilterMode.Trilinear; importer.anisoLevel = 8;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
        importer.SaveAndReimport(); return Load<Texture2D>(path);
    }

    static void RefineExisting(Scene scene)
    {
        var plaster = Load<Material>(Root + "/Materials/MineralCreamPlaster.mat");
        var broadLeaf = SaveMesh(LeafMesh(true), "SplitPhilodendronLeaf");
        int leaves = 0;
        var joineryNames = new HashSet<string> { "TopBeam", "RoofBeam", "SkylightBeam", "SkylightCrossbar", "SkylightRim", "WindowShelf", "WindowPlantLedge", "NicheShelf", "NicheSill" };
        foreach (var r in All<MeshRenderer>(scene))
        {
            if (r.GetComponentInParent<PlayerShooter>() != null) continue;
            if (r.sharedMaterial != null && r.sharedMaterial.name == "CreamPlaster")
            {
                r.sharedMaterial = plaster;
                var mf = r.GetComponent<MeshFilter>();
                var m = UnityEngine.Object.Instantiate(mf.sharedMesh);
                var vertices = m.vertices; var uv = new Vector2[vertices.Length]; var normals = m.normals;
                for (int i = 0; i < vertices.Length; i++)
                {
                    var p = mf.transform.TransformPoint(vertices[i]); var n = mf.transform.TransformDirection(normals[i]);
                    uv[i] = (Mathf.Abs(n.x) > .5f ? new Vector2(p.z, p.y) : Mathf.Abs(n.y) > .5f ? new Vector2(p.x, p.z) : new Vector2(p.x, p.y)) / 2.4f;
                }
                m.uv = uv; mf.sharedMesh = SaveMesh(m, "PlasterWorldUV"); Record(mf);
            }
            if (joineryNames.Contains(r.name))
            {
                var mf = r.GetComponent<MeshFilter>(); var b = mf.sharedMesh.bounds;
                var s = r.transform.lossyScale; s = new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z));
                var m = RoundedBox(Vector3.Scale(b.size, s), .025f);
                var vertices = m.vertices; var normals = m.normals;
                for (int i = 0; i < vertices.Length; i++) { vertices[i] = new Vector3(vertices[i].x / s.x, vertices[i].y / s.y, vertices[i].z / s.z) + b.center; normals[i] = Vector3.Scale(normals[i], s).normalized; }
                m.vertices = vertices; m.normals = normals; m.RecalculateBounds();
                mf.sharedMesh = SaveMesh(m, "SoftJoinery"); r.sharedMaterial = oak; Record(mf);
            }
            if (r.name == "HangingLeaf" || r.name == "FernLeaf" || r.name.StartsWith("Leaf", StringComparison.Ordinal))
            {
                var mf = r.GetComponent<MeshFilter>();
                if (mf != null) { mf.sharedMesh = r.name == "HangingLeaf" ? leaf : broadLeaf; Record(mf); }
                r.sharedMaterial = leaves++ % 4 == 0 ? olive : forest;
            }
            if (r.name == "RugBase" || r.name == "RugStripe")
            {
                string name = "Woven_" + (r.sharedMaterial == null ? "Rug" : r.sharedMaterial.name);
                var path = Root + "/Materials/" + name + ".mat";
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null)
                {
                    mat = new Material(Load<Material>("Assets/StudioArtPack/SurfacesV6/Materials/WovenOatLinen.mat")) { name = name };
                    Color tint = r.sharedMaterial.GetColor("_BaseColor");
                    mat.SetColor("_BaseColor", Color.Lerp(tint, Hex("BDAF97"), .45f)); mat.mainTextureScale = new Vector2(14, 5);
                    AssetDatabase.CreateAsset(mat, path);
                }
                r.sharedMaterial = mat;
            }
            Record(r);
        }
    }

    static void BuildCabinet(Transform parent)
    {
        var cabinet = new GameObject("ArtisanSideboard").transform; cabinet.SetParent(parent, false);
        cabinet.localPosition = new Vector3(5.8f, -3.92f, 6.65f);
        Box(cabinet, "TealCarcass", new Vector3(0, .63f, 0), new Vector3(1.38f, 1.12f, .82f), teal);
        Box(cabinet, "OakWorktop", new Vector3(0, 1.24f, 0), new Vector3(1.53f, .14f, .96f), oak);
        for (int i = 0; i < 2; i++)
        {
            float x = (i - .5f) * .66f;
            Box(cabinet, "InsetDoor", new Vector3(x, .54f, -.445f), new Vector3(.59f, .77f, .045f), teal);
            Box(cabinet, "UpperDrawer", new Vector3(x, 1.01f, -.445f), new Vector3(.59f, .16f, .045f), teal);
            foreach (float y in new[] { .71f, 1.02f })
            {
                var knob = GameObject.CreatePrimitive(PrimitiveType.Sphere); UnityEngine.Object.DestroyImmediate(knob.GetComponent<Collider>());
                knob.name = "BrassPull"; knob.transform.SetParent(cabinet, false); knob.transform.localPosition = new Vector3(x, y, -.51f); knob.transform.localScale = Vector3.one * .075f; knob.GetComponent<MeshRenderer>().sharedMaterial = brass;
            }
        }
        foreach (float x in new[] { -.55f, .55f }) foreach (float z in new[] { -.29f, .29f }) Box(cabinet, "OakFoot", new Vector3(x, .08f, z), new Vector3(.1f, .16f, .1f), oak);
        Combine(cabinet, "Sideboard");
    }

    static void BuildVines(Transform parent)
    {
        var vines = new GameObject("TrailingStudioGreenery").transform; vines.SetParent(parent, false);
        for (int strand = 0; strand < 7; strand++)
        {
            bool left = strand < 4; float x = left ? -5.87f + strand * .13f : 5.15f + (strand - 4) * .12f;
            float z = left ? 5.5f : 9.13f; float top = left ? 2.65f : 2.27f;
            for (int i = 0; i < 12; i++)
            {
                float y = top - i * .16f;
                float sway = Mathf.Sin(i * .68f + strand) * .11f;
                var blade = new GameObject("VineLeaf").transform; blade.SetParent(vines, false);
                blade.localPosition = new Vector3(x + sway + (i % 2 == 0 ? -.10f : .10f), y, z - .06f);
                blade.localRotation = Quaternion.Euler(15 + strand * 5, left ? 35 : -15, i % 2 == 0 ? 57 : -57);
                blade.localScale = new Vector3(.19f, .25f, .25f);
                blade.gameObject.AddComponent<MeshFilter>().sharedMesh = leaf;
                blade.gameObject.AddComponent<MeshRenderer>().sharedMaterial = (i + strand) % 4 == 0 ? olive : forest;
                if (i < 11) Box(vines, "VineStem", new Vector3(x + sway, y - .08f, z), new Vector3(.012f, .18f, .012f), stem);
            }
        }
        Combine(vines, "TrailingGreenery");
    }

    static void RefineLighting(Scene scene, Transform group)
    {
        RenderSettings.ambientSkyColor = new Color(.63f, .69f, .74f);
        RenderSettings.ambientEquatorColor = new Color(.50f, .48f, .44f);
        RenderSettings.ambientGroundColor = new Color(.30f, .32f, .33f);
        var sun = All<Light>(scene).Single(l => l.type == LightType.Directional);
        sun.intensity = 1.4f; sun.shadowStrength = .50f; sun.color = Hex("FFF0DA");
        var fill = All<Light>(scene).Single(l => l.name == "FrontFillLight"); fill.intensity = 7; fill.color = Hex("E5EFF5");
        var window = All<Light>(scene).Single(l => l.name == "WindowDaylight");
        window.intensity = 135; window.shadowStrength = .48f; window.shadowNormalBias = .08f;
        window.shadowBias = .018f;
        var skylight = All<MeshRenderer>(scene).Single(r => r.name == "SkylightGlass");
        var glass = new Material(skylight.sharedMaterial) { name = "LuminousSkylight" };
        glass.SetColor("_EmissionColor", Hex("FFF0D8") * 1.65f); glass.EnableKeyword("_EMISSION");
        AssetDatabase.CreateAsset(glass, Root + "/Materials/LuminousSkylight.mat"); skylight.sharedMaterial = glass; Record(skylight);
        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        AssetDatabase.CreateAsset(profile, Root + "/StudioColorGrade.asset");
        var tone = profile.Add<Tonemapping>(true); tone.mode.Override(TonemappingMode.ACES);
        var color = profile.Add<ColorAdjustments>(true); color.postExposure.Override(.2f); color.contrast.Override(3); color.saturation.Override(-6);
        var bloom = profile.Add<Bloom>(true); bloom.threshold.Override(1.25f); bloom.intensity.Override(.12f); bloom.scatter.Override(.55f);
        var vignette = profile.Add<Vignette>(true); vignette.intensity.Override(.10f); vignette.smoothness.Override(.35f);
        foreach (var component in profile.components) AssetDatabase.AddObjectToAsset(component, profile);
        var volume = group.gameObject.AddComponent<Volume>(); volume.isGlobal = true; volume.priority = 20; volume.sharedProfile = profile;
        // The existing PC renderer already has SSAO. Tune its saved feature without adding a pass.
        var renderer = Load<UniversalRendererData>("Assets/Settings/PC_Renderer.asset");
        var ao = renderer.rendererFeatures.Single(f => f.GetType().Name == "ScreenSpaceAmbientOcclusion");
        var serialized = new SerializedObject(ao);
        serialized.FindProperty("m_Settings.Intensity").floatValue = .70f;
        serialized.FindProperty("m_Settings.Radius").floatValue = .4f;
        serialized.FindProperty("m_Settings.DirectLightingStrength").floatValue = .25f;
        serialized.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(ao);
        renderer.SetDirty(); EditorUtility.SetDirty(profile);
    }

    static void RefreshReflection(Scene scene)
    {
        var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection");
        var gun = All<PlayerShooter>(scene).Single().gameObject; bool active = gun.activeSelf;
        string path = Root + "/Reflections/ArtisanStudio.exr";
        try { gun.SetActive(false); if (!Lightmapping.BakeReflectionProbe(probe, path)) throw new InvalidOperationException("Reflection bake failed."); }
        finally { gun.SetActive(active); }
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        probe.mode = ReflectionProbeMode.Custom; probe.customBakedTexture = Load<Cubemap>(path);
    }

    static Mesh LeafMesh(bool split)
    {
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        const int rows = 40, columns = 8;
        for (int y = 0; y <= rows; y++) for (int x = 0; x <= columns; x++)
        {
            float t = y / (float)rows, s = x / (float)columns * 2 - 1;
            float width = Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * Mathf.PI)), .72f) * .44f;
            if (split) width *= 1 - .40f * Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * Mathf.PI * 12)), 8);
            vertices.Add(new Vector3(s * width, t - .5f, .12f * s * s * Mathf.Sin(t * Mathf.PI) - .065f * Mathf.Sin(t * Mathf.PI) + .12f * t * t));
            uv.Add(new Vector2(x / (float)columns, t));
            if (y < rows && x < columns) { int a = y * (columns + 1) + x; triangles.AddRange(new[] { a, a + columns + 1, a + 1, a + 1, a + columns + 1, a + columns + 2 }); }
        }
        var m = new Mesh(); m.SetVertices(vertices); m.SetUVs(0, uv); m.SetTriangles(triangles, 0); m.RecalculateNormals(); m.RecalculateTangents(); m.RecalculateBounds(); return m;
    }

    static Mesh RoundedBox(Vector3 size, float radius)
    {
        var vertices = new List<Vector3>(); var normals = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        var half = size * .5f; radius = Mathf.Min(radius, Mathf.Min(half.x, Mathf.Min(half.y, half.z)) * .65f); var inner = half - Vector3.one * radius;
        foreach (var normal in new[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back })
        {
            var u = Mathf.Abs(normal.y) > .5f ? Vector3.right : Vector3.up; var v = Vector3.Cross(normal, u);
            float hu = Mathf.Abs(Vector3.Dot(half, u)), hv = Mathf.Abs(Vector3.Dot(half, v));
            var xs = Steps(hu, radius); var ys = Steps(hv, radius); int start = vertices.Count;
            for (int y = 0; y < ys.Length; y++) for (int x = 0; x < xs.Length; x++)
            {
                var q = Vector3.Scale(normal, half) + u * xs[x] + v * ys[y];
                var closest = new Vector3(Mathf.Clamp(q.x, -inner.x, inner.x), Mathf.Clamp(q.y, -inner.y, inner.y), Mathf.Clamp(q.z, -inner.z, inner.z));
                var n = (q - closest).normalized; vertices.Add(closest + n * radius); normals.Add(n); uv.Add(new Vector2(xs[x] + hu, ys[y] + hv));
            }
            for (int y = 0; y < ys.Length - 1; y++) for (int x = 0; x < xs.Length - 1; x++) { int a = start + y * xs.Length + x; triangles.AddRange(new[] { a, a + 1, a + xs.Length, a + 1, a + xs.Length + 1, a + xs.Length }); }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0); mesh.RecalculateTangents(); mesh.RecalculateBounds(); return mesh;
    }
    static float[] Steps(float half, float radius) => new[] { -half, -half + radius * .293f, -half + radius, half - radius, half - radius * .293f, half };
    static void Box(Transform parent, string name, Vector3 position, Vector3 size, Material material)
    {
        var g = new GameObject(name); g.transform.SetParent(parent, false); g.transform.localPosition = position;
        g.AddComponent<MeshFilter>().sharedMesh = RoundedBox(size, .018f); g.AddComponent<MeshRenderer>().sharedMaterial = material;
    }
    static Mesh SaveMesh(Mesh mesh, string name)
    {
        mesh.name = name; mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + (meshIndex++).ToString("D3") + "_" + name + ".asset"); return mesh;
    }
    static void Combine(Transform root, string name)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>();
        foreach (var materialGroup in renderers.GroupBy(r => r.sharedMaterial))
        {
            var instances = materialGroup.Select(r => new CombineInstance { mesh = r.GetComponent<MeshFilter>().sharedMesh, transform = root.worldToLocalMatrix * r.transform.localToWorldMatrix }).ToArray();
            var m = new Mesh { indexFormat = IndexFormat.UInt32 }; m.CombineMeshes(instances, true, true);
            var g = new GameObject(name + "_" + materialGroup.Key.name); g.transform.SetParent(root, false);
            g.AddComponent<MeshFilter>().sharedMesh = SaveMesh(m, g.name); g.AddComponent<MeshRenderer>().sharedMaterial = materialGroup.Key;
        }
        foreach (var r in renderers) UnityEngine.Object.DestroyImmediate(r.gameObject);
    }
    static void Record(UnityEngine.Object value) { if (PrefabUtility.IsPartOfPrefabInstance(value)) PrefabUtility.RecordPrefabInstancePropertyModifications(value); }
    static void Capture(Scene scene, string name)
    {
        ExhibitionStudioBuilder.Capture(scene, name + ".png");
        File.Copy(".local-backups/ExhibitionStudioPreview/" + name + ".png", Root + "/Previews/" + name + ".png", true);
    }
    public static void CaptureLoadedGame()
    {
        if (!Application.isPlaying) throw new InvalidOperationException("Capture the running scene only.");
        Capture(SceneManager.GetActiveScene(), "Gameplay_Runtime_v9");
    }

    public static void VerifyAndCaptureBatch()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);
        if (All<Transform>(scene).Any(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) > 0)) throw new InvalidOperationException("Missing scene script.");
        var group = scene.GetRootGameObjects().Single(g => g.name == "StudioCoverFinishV9");
        if (group.GetComponentsInChildren<Collider>(true).Length != 0) throw new InvalidOperationException("Art collider found.");
        if (LightmapSettings.lightmaps.Length != 1 || LightmapSettings.lightmaps[0].lightmapColor == null || LightmapSettings.lightmaps[0].lightmapDir == null) throw new InvalidOperationException("Saved lightmaps failed to reload.");
        var area = group.GetComponentsInChildren<Light>();
        if (area.Length != 2 || area.Any(l => l.lightmapBakeType != LightmapBakeType.Baked)) throw new InvalidOperationException("Offline lights changed.");
        var meshes = All<MeshFilter>(scene).Where(f => f.sharedMesh != null).Select(f => f.sharedMesh).Distinct();
        foreach (var mesh in meshes)
            if (mesh.isReadable && mesh.vertices.Any(v => !float.IsFinite(v.x) || !float.IsFinite(v.y) || !float.IsFinite(v.z))) throw new InvalidOperationException("Invalid mesh coordinates.");
        Capture(scene, "Room_GameCamera_v9");
        ExhibitionStudioBuilder.ValidateAim(scene);
        AssetDatabase.Refresh();
        Debug.Log("COVER_V9_RELOAD_OK: lightmaps, offline lights, mesh coordinates, scene scripts and four-corner aiming verified.");
    }
    static void CaptureTargets(Scene scene)
    {
        var targets = new List<GameObject>(); var materials = new List<Material>();
        try
        {
            for (int i = 0; i < 3; i++)
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(Load<GameObject>("Assets/Prefabs/PlayableBalloon.prefab"), scene); targets.Add(target);
                target.transform.position = new Vector3((i - 1) * 2.2f, i == 1 ? 1.0f : -.45f, 8);
                var mat = new Material(Load<Material>("Assets/StudioArtPack/SurfacesV6/Materials/TealLatex.mat")); materials.Add(mat);
                mat.SetColor("_BaseColor", Hex(new[] { "2198A2", "E97761", "E8BB58" }[i]));
                foreach (var r in target.GetComponentsInChildren<MeshRenderer>().Where(r => r.name == "SmoothBalloonSurface" || r.sharedMaterials.Any(m => m != null && m.name == "TealLatex"))) r.sharedMaterial = mat;
            }
            Capture(scene, "Targets_Illustrative_v9");
        }
        finally { foreach (var target in targets) UnityEngine.Object.DestroyImmediate(target); foreach (var mat in materials) UnityEngine.Object.DestroyImmediate(mat); }
    }
}
