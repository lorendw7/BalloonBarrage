using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

// Cosmetic scene authoring only. Run in an isolated project, then copy reviewed assets back.
public static class StudioColorRefresh
{
    public const string Root = "Assets/StudioArtPack/ColorRefresh20261020";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    static readonly string[] Hues = { "26B6BB", "F17C65", "E8BC4C", "9C80CC", "78BE99", "E89EB8" };
    static readonly string[] Names = { "Turquoise", "Coral", "Marigold", "Lilac", "Mint", "Rose" };
    static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    static Color Hex(string hex) { ColorUtility.TryParseHtmlString("#" + hex, out var c); return c; }
    static void Record(Object value) { EditorUtility.SetDirty(value); if (PrefabUtility.IsPartOfPrefabInstance(value)) PrefabUtility.RecordPrefabInstancePropertyModifications(value); }

    public static void Run()
    {
        EditorSceneManager.OpenScene(ScenePath);
        int frames = 0;
        EditorApplication.CallbackFunction start = null;
        start = () => { if (++frames < 30) return; EditorApplication.update -= start; Build(); };
        EditorApplication.update += start;
    }

    static void Build()
    {
        try
        {
            Require(Application.isBatchMode && Application.dataPath.Contains(".local-backups"), "Use an isolated batch project under .local-backups.");
            Require(!Directory.Exists(Root), "Preserve existing manual art edits; output already exists.");
            foreach (var folder in new[] { "Materials", "Meshes", "Previews" }) Directory.CreateDirectory(Root + "/" + folder);
            AssetDatabase.Refresh();
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var protectedObjects = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Component>(true))
                .Where(c => c is Collider || c is Rigidbody || c is Camera || c is Canvas || c is MonoBehaviour && !(c is Volume)).ToArray();
            var before = protectedObjects.Select(EditorJsonUtility.ToJson).ToArray();
            var poses = protectedObjects.Select(c => EditorJsonUtility.ToJson(c.transform)).ToArray();
            Capture("Before");
            var root = new GameObject("ColorfulSideWalls_20261020");
            var materials = Hues.Select((h, i) =>
            {
                var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "WallPigment_" + Names[i] };
                m.SetColor("_BaseColor", Hex(h)); m.SetFloat("_Smoothness", .16f); m.SetFloat("_Cull", 0);
                m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", Hex(h) * .10f);
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                AssetDatabase.CreateAsset(m, Root + "/Materials/" + m.name + ".mat"); return m;
            }).ToArray();
            BuildWall(root.transform, -1, materials); BuildWall(root.transform, 1, materials);
            var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            var fill = lights.Single(l => l.name == "FrontFillLight"); fill.intensity = 21; Record(fill);
            var sun = lights.Single(l => l.type == LightType.Directional); sun.intensity = 1.4f; sun.shadowStrength = .36f; Record(sun);
            var window = lights.Single(l => l.name == "WindowDaylight"); window.intensity = 90; Record(window);
            RenderSettings.ambientSkyColor = Hex("BFCCD6");
            RenderSettings.ambientEquatorColor = Hex("AAA397");
            RenderSettings.ambientGroundColor = Hex("737A7D");
            var volume = Object.FindObjectsByType<Volume>(FindObjectsSortMode.None).Single(v => v.isGlobal && AssetDatabase.GetAssetPath(v.sharedProfile) == "Assets/StudioArtPack/CoverFinishV9/StudioColorGrade.asset");
            string profilePath = Root + "/BrightStudioGrade.asset";
            Require(AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(volume.sharedProfile), profilePath), "Copy the grade without changing shared baseline assets.");
            volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath); Record(volume);
            if (volume.sharedProfile.TryGet<ColorAdjustments>(out var grade))
            { grade.postExposure.Override(.30f); grade.saturation.Override(3); grade.contrast.Override(1); EditorUtility.SetDirty(grade); }
            if (volume.sharedProfile.TryGet<Vignette>(out var vignette)) { vignette.intensity.Override(.025f); EditorUtility.SetDirty(vignette); }
            Require(before.SequenceEqual(protectedObjects.Select(EditorJsonUtility.ToJson)) && poses.SequenceEqual(protectedObjects.Select(c => EditorJsonUtility.ToJson(c.transform))), "Gameplay, physics, UI or camera changed.");
            Require(root.GetComponentsInChildren<Collider>().Length == 0 && root.GetComponentsInChildren<MonoBehaviour>().Length == 0, "Decoration must not intercept shots or add gameplay.");
            AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            // Reload saved data before capture; wait for shader uploads.
            EditorSceneManager.OpenScene(ScenePath);
            int frames = 0;
            EditorApplication.CallbackFunction render = null;
            render = () =>
            {
                if (++frames < 30) return;
                EditorApplication.update -= render;
                try
                {
                    Capture("After");
                    File.WriteAllText(Root + "/Verification.txt", "Saved scene reloaded and rendered at 1920x1080.\nExisting gameplay/physics/UI/camera components and their transforms unchanged.\nTwo decorative meshes, six pigment materials, no added colliders/scripts/lights.\nExisting baked lightmaps retained; realtime fill, ambient colors and independent grade adjusted.\nNot a standalone release or interactive playtest.\n");
                    AssetDatabase.Refresh(); Debug.Log("STUDIO_COLOR_REFRESH_OK"); EditorApplication.Exit(0);
                }
                catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
            };
            EditorApplication.update += render;
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }

    // Mesh coordinates are (distance along wall, height); no textures or third-party artwork.
    sealed class Drawing
    {
        public readonly List<Vector3> vertices = new List<Vector3>();
        public readonly List<int>[] triangles = Enumerable.Range(0, 6).Select(_ => new List<int>()).ToArray();
        readonly int side;
        int layers;
        public Drawing(int side) { this.side = side; }
        Vector3 Point(Vector2 p, float depth) => new Vector3(side * (6.975f - depth), p.y, p.x);
        public void Polygon(List<Vector2> points, int color)
        {
            float area = 0;
            for (int i = 0; i < points.Count; i++) { var a = points[i]; var b = points[(i + 1) % points.Count]; area += a.x * b.y - b.x * a.y; }
            if (Mathf.Sign(area) != side) points.Reverse(); // Interior-facing normals on both walls.
            float depth = (++layers) * .00012f; int start = vertices.Count;
            Vector2 center = points.Aggregate(Vector2.zero, (a, b) => a + b) / points.Count;
            vertices.Add(Point(center, depth)); vertices.AddRange(points.Select(p => Point(p, depth)));
            for (int i = 0; i < points.Count; i++) triangles[color].AddRange(new[] { start, start + 1 + i, start + 1 + (i + 1) % points.Count });
        }
        public void Dot(float z, float y, float radius, int color, float squash = 1)
        {
            var p = new List<Vector2>();
            for (int i = 0; i < 40; i++) { float a = i * Mathf.PI / 20; p.Add(new Vector2(z + Mathf.Cos(a) * radius, y + Mathf.Sin(a) * radius * squash)); }
            Polygon(p, color);
        }
        public void Stroke(Vector2[] path, float width, int color)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                var delta = (path[i + 1] - path[i]).normalized; var n = new Vector2(-delta.y, delta.x) * width * .5f;
                Polygon(new List<Vector2> { path[i] - n, path[i] + n, path[i + 1] + n, path[i + 1] - n }, color);
            }
            foreach (var p in path) Dot(p.x, p.y, width * .5f, color);
        }
        public void Splat(float z, float y, float r, int color, int seed)
        {
            var random = new System.Random(seed); var p = new List<Vector2>();
            for (int i = 0; i < 128; i++)
            {
                float a = i * Mathf.PI / 64;
                float spike = Mathf.Pow(Mathf.Max(0, Mathf.Sin(a * 11 + seed)), 16) * .55f;
                float radius = r * (.83f + .16f * Mathf.Sin(a * 5 + seed) + .10f * Mathf.Cos(a * 9) + spike + (float)random.NextDouble() * .07f);
                p.Add(new Vector2(z + Mathf.Cos(a) * radius, y + Mathf.Sin(a) * radius));
            }
            Polygon(p, color);
            for (int i = 0; i < 24; i++) { float a = (float)random.NextDouble() * Mathf.PI * 2; float d = r * (1.2f + (float)random.NextDouble() * 1.15f); Dot(z + Mathf.Cos(a) * d, y + Mathf.Sin(a) * d, r * (.018f + (float)random.NextDouble() * .08f), color, .65f + (float)random.NextDouble()); }
            for (int i = 0; i < 3; i++) { float dz = (i - 1) * r * .45f; Stroke(new[] { new Vector2(z + dz, y - r * .5f), new Vector2(z + dz, y - r * (1.4f + i * .18f)) }, r * .09f, color); }
        }
    }
    static void BuildWall(Transform parent, int side, Material[] materials)
    {
        var d = new Drawing(side);
        // Asymmetric pigment clusters echo the concept's painted studio rather than decorative striping.
        if (side < 0)
        {
            d.Splat(.7f, .65f, .47f, 0, 19); d.Splat(1.8f, -.55f, .34f, 1, 23);
            d.Splat(8.55f, .8f, .30f, 3, 44);
            d.Splat(1.9f, 2.75f, .31f, 2, 71);
            Brush(d, .6f, -1.75f, 1.0f, 2);
        }
        else
        {
            d.Splat(1.1f, .1f, .54f, 1, 32); d.Splat(2.05f, 1.2f, .32f, 3, 57);
            d.Splat(8.75f, -.6f, .32f, 0, 61);
            d.Splat(3.15f, 2.75f, .39f, 0, 89);
            Brush(d, .8f, -1.55f, 1.15f, 4);
            Brush(d, 8.5f, 1.85f, .55f, 2);
        }
        d.Splat(side < 0 ? 1.5f : 2.2f, -2.75f, .21f, 5, 101);
        var mesh = new Mesh { name = side < 0 ? "LeftWallMural" : "RightWallMural", indexFormat = IndexFormat.UInt32 };
        mesh.SetVertices(d.vertices); mesh.subMeshCount = 6;
        for (int i = 0; i < 6; i++) mesh.SetTriangles(d.triangles[i], i);
        mesh.RecalculateNormals(); mesh.RecalculateBounds(); AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + mesh.name + ".asset");
        var go = new GameObject(mesh.name); go.transform.SetParent(parent, false);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = go.AddComponent<MeshRenderer>(); renderer.sharedMaterials = materials;
        renderer.shadowCastingMode = ShadowCastingMode.Off; renderer.receiveShadows = true;
        renderer.lightProbeUsage = LightProbeUsage.BlendProbes;
    }
    static void Brush(Drawing d, float z, float y, float length, int color)
    {
        for (int i = 0; i < 13; i++)
        {
            float offset = (i - 6) * .024f;
            float start = -.5f * length + .08f * Mathf.Sin(i * 3.4f);
            float end = .5f * length + .09f * Mathf.Cos(i * 1.7f);
            d.Stroke(new[] { new Vector2(z + start, y + start * .28f + offset), new Vector2(z, y + offset), new Vector2(z + end, y + end * .28f + offset) }, .023f, color);
        }
    }
    static void Capture(string name)
    {
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var states = canvases.Select(c => c.gameObject.activeSelf).ToArray();
        foreach (var c in canvases) c.gameObject.SetActive(false);
        var camera = Camera.main; var old = camera.targetTexture; float aspect = camera.aspect;
        var rt = RenderTexture.GetTemporary(1920, 1080, 24, RenderTextureFormat.ARGB32); var previous = RenderTexture.active;
        bool async = ShaderUtil.allowAsyncCompilation; ShaderUtil.allowAsyncCompilation = false;
        try
        {
            camera.targetTexture = rt; camera.aspect = 16f / 9;
            RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = rt });
            RenderTexture.active = rt; var image = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0); image.Apply();
            File.WriteAllBytes(Root + "/Previews/" + name + ".png", image.EncodeToPNG()); Object.DestroyImmediate(image);
        }
        finally
        {
            camera.targetTexture = old; camera.aspect = aspect; RenderTexture.active = previous; RenderTexture.ReleaseTemporary(rt); ShaderUtil.allowAsyncCompilation = async;
            for (int i = 0; i < canvases.Length; i++) canvases[i].gameObject.SetActive(states[i]);
        }
    }
}
