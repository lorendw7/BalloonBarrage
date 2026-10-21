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

// Cosmetic prefab pass; colored projectile selection remains learner-owned.
public static class MixedPigmentReservoir
{
    public const string Root = "Assets/StudioArtPack/PaintBlaster/MixedPigment20261020";
    const string Prefab = "Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab";
    const string Scene = "Assets/Scenes/PrototypeScene.unity";
    static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    static Color Hex(string text) { ColorUtility.TryParseHtmlString("#" + text, out var c); return c; }
    static void Later(Action action)
    {
        int frames = 0; EditorApplication.CallbackFunction callback = null;
        callback = () => { if (++frames < 32) return; EditorApplication.update -= callback; try { action(); } catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); } };
        EditorApplication.update += callback;
    }
    public static void Run()
    {
        Require(Application.isBatchMode && Application.dataPath.Contains(".local-backups"), "Use an isolated batch project.");
        Require(!Directory.Exists(Root), "Preserve existing artwork.");
        foreach (var d in new[] { "Materials", "Meshes", "Textures", "Previews" }) Directory.CreateDirectory(Root + "/" + d);
        AssetDatabase.Refresh(); EditorSceneManager.OpenScene(Scene);
        Later(() => { Capture("Before", false); Capture("Before_Model", true); Build(); EditorSceneManager.OpenScene(Scene); Later(() => { Verify(); Capture("After", false); Capture("After_Model", true); AssetDatabase.Refresh(); Debug.Log("MIXED_PIGMENT_OK"); EditorApplication.Exit(0); }); });
    }
    static void Build()
    {
        var gun = PrefabUtility.LoadPrefabContents(Prefab);
        try
        {
            var protectedComponents = gun.GetComponentsInChildren<Component>(true).Where(c => c is MonoBehaviour || c is Collider || c is Rigidbody).ToArray();
            var before = protectedComponents.Select(EditorJsonUtility.ToJson).ToArray();
            var transforms = gun.GetComponentsInChildren<Transform>(true); var poses = transforms.Select(EditorJsonUtility.ToJson).ToArray();
            var root = gun.transform.Find("RefinedGunVisual/WebsiteConceptShell_20261019"); Require(root != null, "Expected current concept shell.");
            var teal = root.Find("PigmentTeal").GetComponent<MeshFilter>(); var source = teal.sharedMesh;
            var points = source.vertices; var indices = source.triangles; var body = new List<int>(); var liquid = new List<int>();
            for (int i = 0; i < indices.Length; i += 3)
            {
                bool fill = points[indices[i]].y > .17f && points[indices[i + 1]].y > .17f && points[indices[i + 2]].y > .17f;
                (fill ? liquid : body).AddRange(new[] { indices[i], indices[i + 1], indices[i + 2] });
            }
            Require(liquid.Count == 7 * 64 * 6, "Unexpected liquid geometry; do not remove unrelated gun parts.");
            var bodyMesh = Object.Instantiate(source); bodyMesh.name = "TealBodyWithoutLiquid"; bodyMesh.SetTriangles(body, 0); bodyMesh.RecalculateBounds();
            AssetDatabase.CreateAsset(bodyMesh, Root + "/Meshes/" + bodyMesh.name + ".asset"); teal.sharedMesh = bodyMesh;
            var selected = liquid.Distinct().ToArray(); var map = selected.Select((old, index) => new { old, index }).ToDictionary(p => p.old, p => p.index);
            var liquidMesh = new Mesh { name = "MarbledLiquidFill" }; liquidMesh.vertices = selected.Select(i => points[i]).ToArray();
            liquidMesh.normals = selected.Select(i => source.normals[i]).ToArray();
            liquidMesh.uv = liquidMesh.vertices.Select(p => new Vector2((p.z + .219f) / .290f, (p.y - .174f + p.x * .32f) / .105f)).ToArray();
            liquidMesh.triangles = liquid.Select(i => map[i]).ToArray(); liquidMesh.RecalculateTangents(); liquidMesh.RecalculateBounds();
            AssetDatabase.CreateAsset(liquidMesh, Root + "/Meshes/" + liquidMesh.name + ".asset");
            var shader = teal.GetComponent<Renderer>().sharedMaterial.shader;
            var material = new Material(shader) { name = "MixedPigmentMarble" }; material.SetColor("_BaseColor", Color.white); material.SetColor("_Color", Color.white);
            material.SetTexture("_BaseMap", MakeMarble()); material.SetFloat("_Smoothness", .62f); material.SetFloat("_Metallic", 0);
            AssetDatabase.CreateAsset(material, Root + "/Materials/MixedPigmentMarble.mat");
            var fillObject = new GameObject("MixedColorLiquid"); fillObject.transform.SetParent(root, false); fillObject.AddComponent<MeshFilter>().sharedMesh = liquidMesh;
            var renderer = fillObject.AddComponent<MeshRenderer>(); renderer.sharedMaterial = material; renderer.shadowCastingMode = ShadowCastingMode.Off;
            // Raised small cream calibration marks on both tank sides; no text/font dependency.
            var ticks = new List<CombineInstance>();
            for (int side = -1; side <= 1; side += 2) for (int i = 0; i < 5; i++)
            {
                var mesh = StudioConceptGeometry.RoundedBox(new Vector3(.002f, i % 2 == 0 ? .014f : .008f, .003f), .0007f);
                ticks.Add(new CombineInstance { mesh = mesh, transform = Matrix4x4.TRS(new Vector3(side * .079f, .239f, -.161f + i * .035f), Quaternion.identity, Vector3.one) });
            }
            var tickMesh = new Mesh { name = "TankCalibrationTicks" }; tickMesh.CombineMeshes(ticks.ToArray());
            AssetDatabase.CreateAsset(tickMesh, Root + "/Meshes/TankCalibrationTicks.asset"); foreach (var tick in ticks) Object.DestroyImmediate(tick.mesh);
            var marks = new GameObject("ReservoirCalibration"); marks.transform.SetParent(root, false); marks.AddComponent<MeshFilter>().sharedMesh = tickMesh;
            var mr = marks.AddComponent<MeshRenderer>(); mr.sharedMaterial = root.Find("WarmCream").GetComponent<Renderer>().sharedMaterial; mr.shadowCastingMode = ShadowCastingMode.Off;
            Require(before.SequenceEqual(protectedComponents.Select(EditorJsonUtility.ToJson)), "Gameplay component data changed.");
            // Existing child lists on the shell root grow; existing local poses and all other transforms remain unchanged.
            for (int i = 0; i < transforms.Length; i++) if (transforms[i] != root) Require(poses[i] == EditorJsonUtility.ToJson(transforms[i]), "Existing transform changed.");
            Require(root.GetComponentsInChildren<Component>().All(c => c is Transform || c is MeshFilter || c is MeshRenderer), "Cosmetic-only hierarchy required.");
            PrefabUtility.SaveAsPrefabAsset(gun, Prefab); AssetDatabase.SaveAssets();
        }
        finally { PrefabUtility.UnloadPrefabContents(gun); }
    }
    static Texture2D MakeMarble()
    {
        const int n = 512; var palette = new[] { "22A9AE", "63B9DF", "9975C5", "E98FAF", "EE7964", "F0BB4D", "9FCB65", "50B898" }.Select(Hex).ToArray();
        var texture = new Texture2D(n, n, TextureFormat.RGB24, false); var pixels = new Color[n * n];
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float u = x / (float)n, v = y / (float)n;
            float flow = u * 2.1f + v * .7f + .20f * Mathf.Sin(v * 10 + u * 9) + .075f * Mathf.Sin(v * 24 - u * 12);
            float p = Mathf.Repeat(flow, 1) * palette.Length; int index = Mathf.FloorToInt(p); float f = p - index;
            float blend = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.72f, 1, f));
            Color color = Color.Lerp(palette[index], palette[(index + 1) % palette.Length], blend);
            float streak = Mathf.Pow(.5f + .5f * Mathf.Sin(flow * Mathf.PI * 64 + v * 3), 18) * .13f;
            pixels[y * n + x] = Color.Lerp(color, new Color(1, .96f, .87f), streak);
        }
        texture.SetPixels(pixels); texture.Apply(); string path = Root + "/Textures/MixedPigmentMarble.png"; File.WriteAllBytes(path, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport); var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.sRGBTexture = true; importer.wrapMode = TextureWrapMode.Clamp; importer.mipmapEnabled = true; importer.filterMode = FilterMode.Trilinear; importer.anisoLevel = 4; importer.maxTextureSize = n; importer.textureCompression = TextureImporterCompression.CompressedHQ; importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static void Verify()
    {
        var shooter = Object.FindObjectsByType<PlayerShooter>(FindObjectsSortMode.None).Single(); var camera = Camera.main;
        var root = shooter.transform.Find("RefinedGunVisual/WebsiteConceptShell_20261019"); var serialized = new SerializedObject(shooter);
        Require(serialized.FindProperty("gunVisual").objectReferenceValue == root.parent, "Recoil binding lost.");
        var muzzle = (Transform)serialized.FindProperty("muzzle").objectReferenceValue;
        Require(muzzle.localPosition == new Vector3(0, .085f, .345f) && !muzzle.IsChildOf(root.parent), "Logical muzzle changed.");
        Require(root.GetComponentsInChildren<Renderer>().Length == 8, "Expected eight cosmetic batches.");
        var fill = root.Find("MixedColorLiquid").GetComponent<Renderer>(); Require(fill.sharedMaterial.mainTexture != null && !ShaderUtil.ShaderHasError(fill.sharedMaterial.shader), "Liquid material invalid.");
        var rotation = shooter.transform.localRotation; float aspect = camera.aspect; int checks = 0;
        foreach (float ratio in new[] { 16f / 9, 16f / 10, 4f / 3 }) foreach (var point in new[] { new Vector3(-3, -1.6f, 8), new Vector3(0, 0, 8), new Vector3(3, 1.6f, 8) })
        {
            camera.aspect = ratio; shooter.transform.LookAt(point);
            foreach (var filter in root.GetComponentsInChildren<MeshFilter>()) foreach (var vertex in filter.sharedMesh.vertices) Require(camera.WorldToViewportPoint(filter.transform.TransformPoint(vertex)).z > camera.nearClipPlane, "Gun clips camera near plane.");
            checks++;
        }
        shooter.transform.localRotation = rotation; camera.aspect = aspect;
        File.WriteAllText(Root + "/Verification.txt", "MIXED_PIGMENT_OK\nExisting combined teal liquid separated by verified triangle count; body retained.\nEight cosmetic mesh batches; no new runtime scripts or colliders.\nSaved gameplay component data, existing local poses, muzzle and recoil references preserved.\n" + checks + " aspect/aim near-plane checks passed.\nNative scene and enlarged inspection renders; not a live fluid simulation, projectile-color preview, playtest or release build.\n");
    }
    static void Capture(string name, bool inspection)
    {
        var shooter = Object.FindObjectsByType<PlayerShooter>(FindObjectsSortMode.None).Single().transform; var camera = Camera.main;
        var pos = shooter.localPosition; var rot = shooter.localRotation; var scale = shooter.localScale;
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None); var states = canvases.Select(c => c.gameObject.activeSelf).ToArray(); foreach (var c in canvases) c.gameObject.SetActive(false);
        if (inspection) { shooter.position = camera.transform.TransformPoint(new Vector3(-.04f, -.04f, 1.8f)); shooter.rotation = camera.transform.rotation * Quaternion.Euler(0, 145, 0); shooter.localScale = Vector3.one * 1.8f; }
        int w = inspection ? 1600 : 1920, h = inspection ? 1000 : 1080;
        var rt = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32); var old = camera.targetTexture; var previous = RenderTexture.active; float aspect = camera.aspect; bool async = ShaderUtil.allowAsyncCompilation; ShaderUtil.allowAsyncCompilation = false;
        try
        {
            camera.targetTexture = rt; camera.aspect = w / (float)h; RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = rt });
            RenderTexture.active = rt; var image = new Texture2D(w, h, TextureFormat.RGB24, false); image.ReadPixels(new Rect(0, 0, w, h), 0, 0); image.Apply(); File.WriteAllBytes(Root + "/Previews/" + name + ".png", image.EncodeToPNG()); Object.DestroyImmediate(image);
        }
        finally { camera.targetTexture = old; camera.aspect = aspect; RenderTexture.active = previous; RenderTexture.ReleaseTemporary(rt); ShaderUtil.allowAsyncCompilation = async; shooter.localPosition = pos; shooter.localRotation = rot; shooter.localScale = scale; for (int i = 0; i < canvases.Length; i++) canvases[i].gameObject.SetActive(states[i]); }
    }
}
