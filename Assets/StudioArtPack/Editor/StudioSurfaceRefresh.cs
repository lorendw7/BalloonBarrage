using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

// Deterministic native surface authoring. Never changes learner gameplay or shared baseline materials.
public static class StudioSurfaceRefresh
{
    public const string Root = "Assets/StudioArtPack/SurfaceRefresh20261020";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    const int Size = 1024;
    static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    static void Record(Object obj) { EditorUtility.SetDirty(obj); if (PrefabUtility.IsPartOfPrefabInstance(obj)) PrefabUtility.RecordPrefabInstancePropertyModifications(obj); }
    static Color Hex(string text) { ColorUtility.TryParseHtmlString("#" + text, out var c); return c; }
    static void Later(Action work)
    {
        int frames = 0; EditorApplication.CallbackFunction callback = null;
        callback = () => { if (++frames < 32) return; EditorApplication.update -= callback; try { work(); } catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); } };
        EditorApplication.update += callback;
    }
    public static void Run()
    {
        Require(Application.isBatchMode && Application.dataPath.Contains(".local-backups"), "Use an isolated batch project.");
        Require(!Directory.Exists(Root), "Preserve existing output.");
        EditorSceneManager.OpenScene(ScenePath); Later(Build);
    }
    public static void Refine()
    {
        Require(Application.isBatchMode && Application.dataPath.Contains(".local-backups"), "Use an isolated batch project.");
        EditorSceneManager.OpenScene(ScenePath);
        var floor = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Materials/WarmMineralFloor.mat");
        var wall = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Materials/LimewashedPlaster.mat");
        floor.SetColor("_BaseColor", Hex("E2E5E9")); wall.SetFloat("_BumpScale", .42f);
        EditorUtility.SetDirty(floor); EditorUtility.SetDirty(wall); AssetDatabase.SaveAssets();
        Later(() => { Capture("After"); AssetDatabase.Refresh(); Debug.Log("SURFACE_REFINEMENT_OK"); EditorApplication.Exit(0); });
    }
    static void Build()
    {
        foreach (string folder in new[] { "Textures", "Materials", "Meshes", "Previews" }) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh(); Capture("Before");
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var protectedObjects = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Component>(true))
            .Where(c => c is Collider || c is Rigidbody || c is Camera || c is Light || c is Canvas || c is MonoBehaviour).ToArray();
        var snapshots = protectedObjects.Select(EditorJsonUtility.ToJson).ToArray();
        var poses = protectedObjects.Select(c => EditorJsonUtility.ToJson(c.transform)).ToArray();
        var floor = Surface("WarmMineralFloor", true); var wall = Surface("LimewashedPlaster", false);
        string[] wallNames = { "LeftWall", "RightWall", "MainWall", "BackWall", "LeftWallReturn", "RightWallReturn", "LeftColumn", "RightColumn" };
        int count = 0;
        foreach (var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            bool isFloor = r.name == "Floor" || r.name == "FloorReturn";
            if (!isFloor && !wallNames.Contains(r.name)) continue;
            var filter = r.GetComponent<MeshFilter>(); Require(filter != null && filter.sharedMesh != null, "Expected surface mesh.");
            var source = filter.sharedMesh; var mesh = Object.Instantiate(source); mesh.name = r.name + "_SurfaceUV";
            var vertices = mesh.vertices; var normals = mesh.normals; var uv = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                var p = filter.transform.TransformPoint(vertices[i]); var n = filter.transform.TransformDirection(normals[i]);
                uv[i] = (Mathf.Abs(n.y) > .5f ? new Vector2(p.x, p.z) : Mathf.Abs(n.x) > .5f ? new Vector2(p.z, p.y) : new Vector2(p.x, p.y)) / (isFloor ? 4.8f : 3.2f);
            }
            mesh.uv = uv; mesh.RecalculateTangents();
            Require(mesh.vertices.SequenceEqual(source.vertices) && mesh.triangles.SequenceEqual(source.triangles) && mesh.uv2.SequenceEqual(source.uv2), "Preserve geometry and baked-light UVs.");
            AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + mesh.name + ".asset");
            filter.sharedMesh = mesh; r.sharedMaterial = isFloor ? floor : wall; Record(filter); Record(r); count++;
        }
        Require(count == 10, "Expected eight plaster surfaces and two floor surfaces, got " + count);
        Require(snapshots.SequenceEqual(protectedObjects.Select(EditorJsonUtility.ToJson)) && poses.SequenceEqual(protectedObjects.Select(c => EditorJsonUtility.ToJson(c.transform))), "Protected scene components changed.");
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        EditorSceneManager.OpenScene(ScenePath);
        Later(() =>
        {
            Capture("After");
            File.WriteAllText(Root + "/Verification.txt", "SURFACE_REFRESH_OK\nTen saved surface assignments: eight plaster, two mineral floor.\nNative seamless 1024px albedo, tangent-normal and metallic/smoothness maps.\nWorld-scale UV0 and tangents updated; geometry, triangle topology and lightmap UV2 retained.\nExisting gameplay, collider, camera, UI, light and MonoBehaviour component data and transforms unchanged.\nSaved scene reloaded and rendered at 1920x1080 with UI temporarily hidden.\nExisting baked lighting and reflections retained; no rebake, standalone test or performance benchmark.\n");
            AssetDatabase.Refresh(); Debug.Log("SURFACE_REFRESH_OK"); EditorApplication.Exit(0);
        });
    }
    static float Hash(int x, int y, int seed)
    {
        unchecked { uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1447); h = (h ^ (h >> 13)) * 1274126177; return (h ^ (h >> 16)) / (float)uint.MaxValue; }
    }
    static float Noise(float u, float v, int cells, int seed)
    {
        float x = u * cells, y = v * cells; int ix = Mathf.FloorToInt(x), iy = Mathf.FloorToInt(y);
        float tx = x - ix, ty = y - iy; tx = tx * tx * (3 - 2 * tx); ty = ty * ty * (3 - 2 * ty);
        int Wrap(int a) => (a % cells + cells) % cells;
        return Mathf.Lerp(Mathf.Lerp(Hash(Wrap(ix), Wrap(iy), seed), Hash(Wrap(ix + 1), Wrap(iy), seed), tx), Mathf.Lerp(Hash(Wrap(ix), Wrap(iy + 1), seed), Hash(Wrap(ix + 1), Wrap(iy + 1), seed), tx), ty) - .5f;
    }
    static Material Surface(string name, bool floor)
    {
        int n = Size; var albedo = new Color[n * n]; var normal = new Color[n * n]; var mask = new Color[n * n]; var height = new float[n * n];
        Color baseColor = Hex(floor ? "B5B0A5" : "E9E2D5");
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float u = x / (float)n, v = y / (float)n;
            float broad = Noise(u, v, 4, 11), medium = Noise(u, v, 18, 29), grain = Noise(u, v, 110, 71);
            float trowel = Noise(u + .018f * Mathf.Sin(v * Mathf.PI * 8), v, 38, 47);
            float fine = Hash(x, y, 83) - .5f;
            float value = 1 + broad * (floor ? .16f : .065f) + medium * .035f + grain * .016f + fine * .009f;
            int i = y * n + x; albedo[i] = new Color(baseColor.r * value, baseColor.g * value, baseColor.b * value, 1);
            height[i] = medium * .28f + trowel * .24f + grain * .11f + fine * .018f;
            float smooth = floor ? .26f + broad * .10f + medium * .08f : .13f + medium * .08f;
            mask[i] = new Color(0, 0, 0, Mathf.Clamp01(smooth));
        }
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float dx = height[y * n + (x + 1) % n] - height[y * n + (x + n - 1) % n];
            float dy = height[((y + 1) % n) * n + x] - height[((y + n - 1) % n) * n + x];
            var direction = new Vector3(-dx * (floor ? 3 : 5), -dy * (floor ? 3 : 5), 1).normalized;
            normal[y * n + x] = new Color(direction.x * .5f + .5f, direction.y * .5f + .5f, direction.z * .5f + .5f, 1);
        }
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
        m.SetColor("_BaseColor", floor ? Hex("E2E5E9") : Color.white); m.SetTexture("_BaseMap", Texture(name + "_Albedo", albedo, false, true));
        m.SetTexture("_BumpMap", Texture(name + "_Normal", normal, true, false)); m.SetFloat("_BumpScale", floor ? .55f : .42f); m.EnableKeyword("_NORMALMAP");
        m.SetTexture("_MetallicGlossMap", Texture(name + "_Smoothness", mask, false, false)); m.SetFloat("_Metallic", 0); m.SetFloat("_Smoothness", 1); m.EnableKeyword("_METALLICSPECGLOSSMAP");
        m.SetFloat("_SmoothnessTextureChannel", 0); m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
        AssetDatabase.CreateAsset(m, Root + "/Materials/" + name + ".mat"); return m;
    }
    static Texture2D Texture(string name, Color[] pixels, bool normal, bool srgb)
    {
        var image = new Texture2D(Size, Size, TextureFormat.RGBA32, false, !srgb); image.SetPixels(pixels); image.Apply();
        string path = Root + "/Textures/" + name + ".png"; File.WriteAllBytes(path, image.EncodeToPNG()); Object.DestroyImmediate(image);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport); var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default; importer.sRGBTexture = srgb;
        importer.wrapMode = TextureWrapMode.Repeat; importer.mipmapEnabled = true; importer.anisoLevel = 8; importer.filterMode = FilterMode.Trilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ; importer.maxTextureSize = Size; importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static void Capture(string name)
    {
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None); var states = canvases.Select(c => c.gameObject.activeSelf).ToArray();
        foreach (var c in canvases) c.gameObject.SetActive(false);
        var camera = Camera.main; var old = camera.targetTexture; float aspect = camera.aspect; var previous = RenderTexture.active;
        var rt = RenderTexture.GetTemporary(1920, 1080, 24, RenderTextureFormat.ARGB32); bool async = ShaderUtil.allowAsyncCompilation; ShaderUtil.allowAsyncCompilation = false;
        try
        {
            camera.targetTexture = rt; camera.aspect = 16f / 9;
            RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = rt });
            RenderTexture.active = rt; var image = new Texture2D(1920, 1080, TextureFormat.RGB24, false); image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0); image.Apply();
            File.WriteAllBytes(Root + "/Previews/" + name + ".png", image.EncodeToPNG()); Object.DestroyImmediate(image);
        }
        finally { camera.targetTexture = old; camera.aspect = aspect; RenderTexture.active = previous; RenderTexture.ReleaseTemporary(rt); ShaderUtil.allowAsyncCompilation = async; for (int i = 0; i < canvases.Length; i++) canvases[i].gameObject.SetActive(states[i]); }
    }
}
