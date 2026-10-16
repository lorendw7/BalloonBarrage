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

// Opt-in art study. Original materials, meshes, prefabs, camera/light settings and learner code stay intact.
public static class StudioMaterialAppearancePass
{
    public const string Source = "Assets/Scenes/StudioCameraLighting.unity";
    public const string Destination = "Assets/Scenes/StudioMaterialAppearance.unity";
    public const string Root = "Assets/StudioArtPack/MaterialAppearanceV11";
    const string BalloonSource = "Assets/Prefabs/PlayableBalloon.prefab";
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects()
        .SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static T Load<T>(string path) where T : UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new FileNotFoundException(path);
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var c); return c; }
    static void Record(UnityEngine.Object obj)
    {
        EditorUtility.SetDirty(obj);
        if (PrefabUtility.IsPartOfPrefabInstance(obj)) PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
    }
    static Material Clone(string source, string name, string tint, float smoothness)
    {
        var material = new Material(Load<Material>(source)) { name = name };
        material.SetColor("_BaseColor", Hex(tint)); material.SetFloat("_Metallic", 0);
        material.SetFloat("_Smoothness", smoothness);
        material.SetColor("_EmissionColor", Color.black); material.DisableKeyword("_EMISSION");
        AssetDatabase.CreateAsset(material, Root + "/Materials/" + name + ".mat"); return material;
    }

    [MenuItem("BalloonBarrage/Art/Create balloon and floor material samples")]
    public static void CreateSamples()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (File.Exists(Destination) || Directory.Exists(Root)) throw new InvalidOperationException("Study exists; preserve manual edits.");
        if (!(GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)) throw new InvalidOperationException("Installed URP required.");
        foreach (string folder in new[] { "Materials", "Textures", "Meshes", "Prefabs", "Previews", "Reflections" }) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh();
        var source = EditorSceneManager.OpenScene(Source);
        WriteAudit(source);
        CaptureTargetLayout(source, "Before_Room", null, false);
        CaptureTargetLayout(source, "Before_Sample", null, true);
        CaptureGunPose(source, "Before_Gun", false);
        var latex = Clone("Assets/StudioArtPack/SurfacesV6/Materials/TealLatex.mat", "SoftTealLatex", "249DA6", .42f);
        var floor = Clone("Assets/StudioArtPack/RoomFinishV7/Materials/NaturalOakFloor.mat", "DrySatinOakFloor", "FFFFFF", .75f);
        floor.SetTextureScale("_BaseMap", Vector2.one * 1.75f);
        // Reuse the existing oak alpha smoothness map: .75 * its .15-.39 alpha gives restrained .11-.29 gloss.
        floor.SetTexture("_MetallicGlossMap", Load<Texture2D>("Assets/StudioArtPack/SurfacesV6/Textures/OakSurface.png"));
        floor.SetTextureScale("_MetallicGlossMap", Vector2.one * 1.75f);
        floor.EnableKeyword("_METALLICSPECGLOSSMAP");
        string normalPath = Root + "/Textures/FloorHeightSource.png";
        File.Copy("Assets/StudioArtPack/RoomFinishV7/Textures/OakPlanks_v7.png", normalPath, false);
        AssetDatabase.ImportAsset(normalPath, ImportAssetOptions.ForceSynchronousImport);
        var normalImporter = (TextureImporter)AssetImporter.GetAtPath(normalPath);
        normalImporter.textureType = TextureImporterType.NormalMap;
        normalImporter.convertToNormalmap = true; normalImporter.heightmapScale = .025f;
        normalImporter.sRGBTexture = false; normalImporter.wrapMode = TextureWrapMode.Repeat;
        normalImporter.mipmapEnabled = true; normalImporter.anisoLevel = 8;
        normalImporter.SaveAndReimport();
        floor.SetTexture("_BumpMap", Load<Texture2D>(normalPath));
        floor.SetTextureScale("_BumpMap", Vector2.one * 1.75f);
        floor.SetFloat("_BumpScale", .16f); floor.EnableKeyword("_NORMALMAP");
        if (!AssetDatabase.CopyAsset(Source, Destination)) throw new IOException("Scene copy failed.");
        var scene = EditorSceneManager.OpenScene(Destination);
        foreach (var r in All<MeshRenderer>(scene).Where(r => r.name == "Floor" || r.name == "FloorReturn"))
        { r.sharedMaterial = floor; Record(r); }
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        CaptureTargetLayout(scene, "After_Sample", latex, true);
        AssetDatabase.Refresh();
        Debug.Log("MATERIAL_SAMPLES_OK: one unchanged balloon mesh, metallic 0/smoothness .42; existing oak, 20 cm boards, aligned normal map and restrained gloss. Camera/lights/postprocessing unchanged.");
    }

    static void WriteAudit(Scene scene)
    {
        var lines = new List<string> { "Unity " + Application.unityVersion, "Source " + Source,
            "Retain camera, lighting and postprocessing. Original asset bytes are not modified." };
        foreach (var r in All<Renderer>(scene))
        {
            if (r is ParticleSystemRenderer) continue;
            if (r is SpriteRenderer sprite) lines.Add("Sprite " + r.name + " | sprite " + sprite.sprite?.name + " | position " + r.transform.position + " | bounds " + r.bounds);
            else if (r.name.Contains("Floor") || r.name.Contains("Wall") || r.GetComponentInParent<PlayerShooter>() != null)
            {
                var mesh = r.GetComponent<MeshFilter>()?.sharedMesh;
                lines.Add("Mesh " + r.name + " | enabled " + r.enabled + " | mesh " + mesh?.name + " | vertices " + mesh?.vertexCount +
                    " | local bounds " + mesh?.bounds + " | world scale " + r.transform.lossyScale);
            }
            lines.AddRange(r.sharedMaterials.Where(m => m != null).Select(m => "Material " + m.name + " | " + m.shader.name +
                " | metallic " + (m.HasProperty("_Metallic") ? m.GetFloat("_Metallic") : -1) +
                " | smoothness " + (m.HasProperty("_Smoothness") ? m.GetFloat("_Smoothness") : -1)));
        }
        File.WriteAllLines(Root + "/SourceAudit.txt", lines.Distinct());
    }

    public static void RefineLatexSample()
    {
        var material = Load<Material>(Root + "/Materials/SoftTealLatex.mat");
        material.SetFloat("_Smoothness", .48f); EditorUtility.SetDirty(material); AssetDatabase.SaveAssets();
        CaptureTargetLayout(EditorSceneManager.OpenScene(Destination), "After_Sample", material, true);
        AssetDatabase.Refresh(); Debug.Log("MATERIAL_SAMPLE_REFINED_OK: .48 soft latex gloss, camera/light settings unchanged.");
    }

    [MenuItem("BalloonBarrage/Art/Promote reviewed material samples")]
    public static void Promote()
    {
        if (!File.Exists(Root + "/SampleReview.txt")) throw new InvalidOperationException("Inspect the game-camera sample images and record SampleReview.txt first.");
        if (File.Exists(Root + "/Promoted.txt")) throw new InvalidOperationException("Pass already promoted; preserve manual edits.");
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var scene = EditorSceneManager.OpenScene(Destination);
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        string cameraBefore = EditorJsonUtility.ToJson(camera) + EditorJsonUtility.ToJson(camera.transform);
        var lights = All<Light>(scene); var lightBefore = lights.Select(l => EditorJsonUtility.ToJson(l) + EditorJsonUtility.ToJson(l.transform)).ToArray();
        var colliders = All<Collider>(scene); var colliderBefore = colliders.Select(c => EditorJsonUtility.ToJson(c)).ToArray();
        var gameplay = All<MonoBehaviour>(scene).Where(c => c != null && AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(c)).StartsWith("Assets/Scripts/", StringComparison.Ordinal)).ToArray();
        var gameplayBefore = gameplay.Select(c => EditorJsonUtility.ToJson(c)).ToArray();
        var spawner = All<BalloonSpawner>(scene).Single(); var spawnerSo = new SerializedObject(spawner);
        var oldPrefab = spawnerSo.FindProperty("balloonPrefab").objectReferenceValue;
        CaptureGunPose(scene, "Before_WeaponInspection", true);
        PromoteBalloon(spawner);
        RefineWalls(scene);
        RefinePaint(scene);
        RefineWeapon(scene);
        // Validate all learner fields against the baseline, allowing only the explicitly cloned visual prefab reference.
        var newPrefab = new SerializedObject(spawner).FindProperty("balloonPrefab").objectReferenceValue;
        SetBalloonPrefab(spawner, oldPrefab);
        if (!gameplayBefore.SequenceEqual(gameplay.Select(c => EditorJsonUtility.ToJson(c)))) throw new InvalidOperationException("Learner serialization changed.");
        SetBalloonPrefab(spawner, newPrefab);
        if (colliders.Length != All<Collider>(scene).Length || !colliderBefore.SequenceEqual(colliders.Select(c => EditorJsonUtility.ToJson(c)))) throw new InvalidOperationException("Scene collision changed.");
        if (cameraBefore != EditorJsonUtility.ToJson(camera) + EditorJsonUtility.ToJson(camera.transform) ||
            !lightBefore.SequenceEqual(lights.Select(l => EditorJsonUtility.ToJson(l) + EditorJsonUtility.ToJson(l.transform))))
            throw new InvalidOperationException("Camera or lights changed.");
        var settings = UnityEngine.Object.Instantiate(Lightmapping.lightingSettings);
        settings.name = "MaterialAppearanceLighting";
        AssetDatabase.CreateAsset(settings, Root + "/MaterialAppearanceLighting.lighting");
        Lightmapping.lightingSettings = settings; Lightmapping.lightingDataAsset = null;
        LightmapSettings.lightmaps = Array.Empty<LightmapData>();
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        // Recompute material bounce and new relief geometry under the unchanged V10 lighting parameters.
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Material study bake failed.");
        var gun = All<PlayerShooter>(scene).Single().gameObject; bool active = gun.activeSelf;
        var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection");
        string reflection = Root + "/Reflections/MaterialStudio.exr";
        try { gun.SetActive(false); if (!Lightmapping.BakeReflectionProbe(probe, reflection)) throw new IOException("Reflection bake failed."); }
        finally { gun.SetActive(active); }
        AssetDatabase.ImportAsset(reflection, ImportAssetOptions.ForceSynchronousImport);
        probe.customBakedTexture = Load<Cubemap>(reflection); Record(probe);
        if (!EditorBuildSettings.scenes.Any(s => s.path == Destination))
            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(Destination, true) }).ToArray();
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        File.WriteAllText(Root + "/Promoted.txt", "Reviewed balloon/floor samples promoted. Original assets preserved; cloned playable visual prefab only. Camera, lights, postprocessing, learner fields and colliders retained.\n");
        VerifySaved();
    }

    static void SetBalloonPrefab(BalloonSpawner spawner, UnityEngine.Object prefab)
    {
        var so = new SerializedObject(spawner); so.FindProperty("balloonPrefab").objectReferenceValue = prefab;
        so.ApplyModifiedPropertiesWithoutUndo(); Record(spawner);
    }
    static void PromoteBalloon(BalloonSpawner spawner)
    {
        string path = Root + "/Prefabs/PlayableBalloonAppearance.prefab";
        if (!AssetDatabase.CopyAsset(BalloonSource, path)) throw new IOException("Balloon visual copy failed.");
        var root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            var protectedComponents = root.GetComponentsInChildren<Component>(true).Where(c => c is Collider || c is Rigidbody || c is MonoBehaviour).ToArray();
            var before = protectedComponents.Select(c => EditorJsonUtility.ToJson(c)).ToArray();
            foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>())
            { renderer.sharedMaterial = Load<Material>(Root + "/Materials/SoftTealLatex.mat"); Record(renderer); }
            if (!before.SequenceEqual(protectedComponents.Select(c => EditorJsonUtility.ToJson(c)))) throw new InvalidOperationException("Balloon physics/gameplay changed.");
            PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
        SetBalloonPrefab(spawner, Load<GameObject>(path));
    }

    static void RefineWalls(Scene scene)
    {
        var plaster = Clone("Assets/StudioArtPack/CoverFinishV9/Materials/MineralCreamPlaster.mat", "QuietCreamPlaster", "F6EBDD", .10f);
        const int n = 1024;
        var colors = new Color[n * n];
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float noise = Mathf.PerlinNoise(x / (float)n * 7.3f + 17.1f, y / (float)n * 4.8f + 9.2f);
            float fine = Mathf.PerlinNoise(x / (float)n * 113.7f, y / (float)n * 89.3f);
            float tint = .992f + .008f * noise + .002f * (fine - .5f);
            colors[y * n + x] = new Color(tint, tint, tint, 1);
        }
        var texture = SaveTexture("QuietPlaster", n, n, colors, false, TextureWrapMode.Clamp);
        plaster.SetTexture("_BaseMap", texture); plaster.SetTextureScale("_BaseMap", Vector2.one);
        plaster.SetFloat("_BumpScale", .085f); plaster.SetTextureScale("_BumpMap", new Vector2(.61f, .83f));
        foreach (var r in All<MeshRenderer>(scene).Where(r => r.sharedMaterial != null && r.sharedMaterial.name == "MineralCreamPlaster"))
        {
            r.sharedMaterial = plaster;
            var filter = r.GetComponent<MeshFilter>(); var mesh = UnityEngine.Object.Instantiate(filter.sharedMesh);
            var vertices = mesh.vertices; var normals = mesh.normals; var uv = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                var p = filter.transform.TransformPoint(vertices[i]); var normal = filter.transform.TransformDirection(normals[i]);
                uv[i] = Mathf.Abs(normal.x) > .5f ? new Vector2((p.z + 10) / 20f, (p.y + 4.2f) / 8.4f) :
                    Mathf.Abs(normal.z) > .5f ? new Vector2((p.x + 7.2f) / 14.4f, (p.y + 4.2f) / 8.4f) : new Vector2((p.x + 7.2f) / 14.4f, (p.z + 10) / 20f);
            }
            mesh.uv = uv; mesh.RecalculateTangents(); mesh.name = r.name + "_NonRepeatingPlasterUV";
            AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + mesh.name + ".asset");
            filter.sharedMesh = mesh; Record(filter); Record(r);
        }
    }

    static Texture2D SaveTexture(string name, int width, int height, Color[] colors, bool linear, TextureWrapMode wrap)
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, linear);
        texture.SetPixels(colors); texture.Apply(); string path = Root + "/Textures/" + name + ".png";
        File.WriteAllBytes(path, texture.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path); importer.sRGBTexture = !linear;
        importer.wrapMode = wrap; importer.mipmapEnabled = true; importer.filterMode = FilterMode.Trilinear; importer.anisoLevel = 8;
        importer.SaveAndReimport(); return Load<Texture2D>(path);
    }

    static void RefineWeapon(Scene scene)
    {
        string mats = "Assets/StudioArtPack/SurfacesV6/Materials/";
        var cream = Clone(mats + "CreamEnamel.mat", "WeaponCream", "F5EBD9", .28f);
        var teal = Clone(mats + "TealEnamel.mat", "WeaponTealGrip", "278F87", .24f);
        var coral = Clone(mats + "CoralEnamel.mat", "WeaponCoralMuzzle", "F07865", .32f);
        var yellow = Clone(mats + "OchreEnamel.mat", "WeaponYellowStructure", "F4C75B", .30f);
        foreach (var m in new[] { cream, teal, coral, yellow })
        {
            m.SetTexture("_MetallicGlossMap", null); m.DisableKeyword("_METALLICSPECGLOSSMAP");
            m.SetFloat("_BumpScale", .06f);
        }
        var visual = All<PlayerShooter>(scene).Single().transform.Find("RefinedGunVisual");
        if (visual == null) throw new InvalidOperationException("Existing refined weapon missing.");
        foreach (var r in visual.GetComponentsInChildren<MeshRenderer>())
        {
            var filter = r.GetComponent<MeshFilter>(); var original = filter.sharedMesh;
            var mesh = UnityEngine.Object.Instantiate(original); var originalVertices = original.vertices;
            var vertices = (Vector3[])originalVertices.Clone(); var normals = original.normals;
            string name = r.sharedMaterial.name;
            for (int i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i]; var normal = normals[i];
                if ((name == "BrushedSteel" && v.z > .14f) || (name == "SatinBrass" && v.z > .29f))
                { v.x *= 1.20f; v.y = .085f + (v.y - .085f) * 1.20f; normal.x /= 1.20f; normal.y /= 1.20f; }
                if (v.y > .18f || ((name == "BrushedSteel" || name == "SatinBrass") && v.y > .138f))
                { v.x *= 1.12f; v.z = -.045f + (v.z + .045f) * 1.12f; normal.x /= 1.12f; normal.z /= 1.12f; }
                vertices[i] = v; normals[i] = normal.normalized;
            }
            var materialList = new List<Material>(); var groups = new List<List<int>>();
            var triangles = original.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                var p = (originalVertices[triangles[i]] + originalVertices[triangles[i + 1]] + originalVertices[triangles[i + 2]]) / 3;
                Material m = name == "CreamEnamel" ? cream : name == "TealEnamel" ? teal : name == "CoralEnamel" ? coral :
                    name == "OchreEnamel" ? yellow : name == "SatinBrass" ? (p.z > .29f ? coral : yellow) :
                    name == "BrushedSteel" ? (p.z > .13f ? coral : r.sharedMaterial) :
                    name == "CharcoalRubber" && p.y < -.015f && p.z < -.105f ? teal : r.sharedMaterial;
                int index = materialList.IndexOf(m);
                if (index < 0) { index = materialList.Count; materialList.Add(m); groups.Add(new List<int>()); }
                groups[index].AddRange(new[] { triangles[i], triangles[i + 1], triangles[i + 2] });
            }
            mesh.vertices = vertices; mesh.subMeshCount = materialList.Count;
            for (int i = 0; i < groups.Count; i++) mesh.SetTriangles(groups[i], i);
            mesh.normals = normals; mesh.RecalculateTangents(); mesh.RecalculateBounds();
            mesh.name = "Weapon_" + r.name; AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + mesh.name + ".asset");
            filter.sharedMesh = mesh; r.sharedMaterials = materialList.ToArray(); Record(filter); Record(r);
        }
    }

    static void RefinePaint(Scene scene)
    {
        var paints = new Dictionary<Material, Material>();
        foreach (var r in All<MeshRenderer>(scene).Where(r => new[] { "FloorPaint", "PaintDot", "PaintDrip", "LidPaint", "PaintSwatch" }.Contains(r.name)))
        {
            var old = r.sharedMaterial;
            if (!paints.TryGetValue(old, out var paint))
            {
                var tint = old.GetColor("_BaseColor"); Color.RGBToHSV(tint, out float h, out float s, out float v);
                string color = s < .3f ? "F3E7D1" : h > .35f && h < .65f ? "28B9AE" : h > .05f && h < .2f ? "FFD259" : "F67D69";
                paint = Clone(AssetDatabase.GetAssetPath(old), "ThickPaint_" + old.name, color, .48f);
                paint.SetTexture("_MetallicGlossMap", null); paint.DisableKeyword("_METALLICSPECGLOSSMAP");
                paint.SetTexture("_BaseMap", null); paint.SetTexture("_BumpMap", null); paint.DisableKeyword("_NORMALMAP");
                paints.Add(old, paint);
            }
            r.sharedMaterial = paint; Record(r);
        }
        foreach (var sprite in All<SpriteRenderer>(scene).Where(r => r.name.StartsWith("Graffiti_", StringComparison.Ordinal))) CreatePaintRelief(sprite);
    }

    static void CreatePaintRelief(SpriteRenderer sprite)
    {
        // Read existing atlas through the GPU without changing its importer or recorded artwork.
        var source = sprite.sprite.texture; var rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        var active = RenderTexture.active; var pixels = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        try { Graphics.Blit(source, rt); RenderTexture.active = rt; pixels.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0); pixels.Apply(); }
        finally { RenderTexture.active = active; RenderTexture.ReleaseTemporary(rt); }
        const int n = 128; var rect = sprite.sprite.rect; var bounds = sprite.sprite.bounds;
        var vertices = new List<Vector3>(); var uvs = new List<Vector2>(); var triangles = new List<int>(); var sides = new List<int>();
        var alpha = new float[(n + 1) * (n + 1)];
        for (int y = 0; y <= n; y++) for (int x = 0; x <= n; x++)
        {
            float u = x / (float)n, v = y / (float)n;
            var uv = new Vector2((rect.x + rect.width * u) / source.width, (rect.y + rect.height * v) / source.height);
            float a = pixels.GetPixelBilinear(uv.x, uv.y).a; alpha[y * (n + 1) + x] = a;
            float height = (.002f + .010f * Mathf.SmoothStep(0, 1, a)) / Mathf.Abs(sprite.transform.lossyScale.z);
            vertices.Add(new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, u), Mathf.Lerp(bounds.min.y, bounds.max.y, v), -height));
            uvs.Add(uv);
        }
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            int a = y * (n + 1) + x, b = a + 1, c = a + n + 1, d = c + 1;
            if (Mathf.Max(alpha[a], alpha[b], alpha[c], alpha[d]) < .25f) continue;
            // Let the original alpha texture define the top contour, rather than eroding it to whole grid cells.
            triangles.AddRange(new[] { a, c, b, b, c, d });
            var crosses = new List<Vector3>(); var crossUvs = new List<Vector2>();
            void Cross(int first, int second)
            {
                if ((alpha[first] >= .25f) == (alpha[second] >= .25f)) return;
                float t = (.25f - alpha[first]) / (alpha[second] - alpha[first]);
                crosses.Add(Vector3.Lerp(vertices[first], vertices[second], t)); crossUvs.Add(Vector2.Lerp(uvs[first], uvs[second], t));
            }
            Cross(a, b); Cross(b, d); Cross(d, c); Cross(c, a);
            for (int i = 0; i + 1 < crosses.Count; i += 2)
            {
                var va = crosses[i]; var vb = crosses[i + 1];
                var gradient = new Vector3(alpha[b] + alpha[d] - alpha[a] - alpha[c], alpha[c] + alpha[d] - alpha[a] - alpha[b], 0);
                if (Vector3.Dot(Vector3.Cross(vb - va, new Vector3(0, 0, -va.z)), -gradient) < 0) { var swap = va; va = vb; vb = swap; }
                int start = vertices.Count;
                vertices.Add(va); vertices.Add(vb); vertices.Add(new Vector3(va.x, va.y, 0)); vertices.Add(new Vector3(vb.x, vb.y, 0));
                uvs.AddRange(new[] { crossUvs[i], crossUvs[i + 1], crossUvs[i], crossUvs[i + 1] });
                sides.AddRange(new[] { start, start + 1, start + 2, start + 1, start + 3, start + 2 });
            }
        }
        UnityEngine.Object.DestroyImmediate(pixels);
        var mesh = new Mesh { name = sprite.name + "_RaisedPaint", indexFormat = IndexFormat.UInt32 };
        mesh.SetVertices(vertices); mesh.SetUVs(0, uvs); mesh.subMeshCount = 2; mesh.SetTriangles(triangles, 0); mesh.SetTriangles(sides, 1); mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds();
        if (!Unwrapping.GenerateSecondaryUVSet(mesh)) throw new InvalidOperationException("Paint UV bake preparation failed.");
        mesh = Persist(mesh, Root + "/Meshes/" + mesh.name + ".asset");
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = sprite.name + "_Paint" };
        material.SetTexture("_BaseMap", source); material.SetColor("_BaseColor", Color.white); material.SetFloat("_Metallic", 0); material.SetFloat("_Smoothness", .50f);
        material.SetFloat("_AlphaClip", 1); material.SetFloat("_Cutoff", .25f); material.EnableKeyword("_ALPHATEST_ON"); material.SetOverrideTag("RenderType", "TransparentCutout"); material.renderQueue = (int)RenderQueue.AlphaTest;
        material = Persist(material, Root + "/Materials/" + material.name + ".mat");
        var edge = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = sprite.name + "_PaintEdge" };
        edge.SetColor("_BaseColor", Hex(sprite.name.Contains("Cyan") ? "38AEAF" : sprite.name.Contains("Yellow") ? "EDC741" : "DD7161"));
        edge.SetFloat("_Metallic", 0); edge.SetFloat("_Smoothness", .45f);
        edge = Persist(edge, Root + "/Materials/" + edge.name + ".mat");
        var child = sprite.transform.Find("RaisedPaintVisual");
        var relief = child == null ? new GameObject("RaisedPaintVisual") : child.gameObject;
        if (child == null) relief.transform.SetParent(sprite.transform, false);
        var filter = relief.GetComponent<MeshFilter>() ?? relief.AddComponent<MeshFilter>(); filter.sharedMesh = mesh;
        var renderer = relief.GetComponent<MeshRenderer>() ?? relief.AddComponent<MeshRenderer>(); renderer.sharedMaterials = new[] { material, edge };
        renderer.shadowCastingMode = ShadowCastingMode.On; renderer.receiveShadows = true;
        GameObjectUtility.SetStaticEditorFlags(relief, GameObjectUtility.GetStaticEditorFlags(sprite.gameObject) | StaticEditorFlags.ContributeGI);
        sprite.enabled = false; Record(sprite);
    }

    static T Persist<T>(T obj, string path) where T : UnityEngine.Object
    {
        var existing = AssetDatabase.LoadAssetAtPath<T>(path);
        if (existing == null) { AssetDatabase.CreateAsset(obj, path); return obj; }
        EditorUtility.CopySerialized(obj, existing); EditorUtility.SetDirty(existing); UnityEngine.Object.DestroyImmediate(obj); return existing;
    }

    public static void RefineModelReview()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        var originalGun = Load<GameObject>("Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab");
        var originalGrip = originalGun.transform.Find("RefinedGunVisual/CharcoalRubber").GetComponent<MeshFilter>().sharedMesh;
        var grip = All<PlayerShooter>(scene).Single().transform.Find("RefinedGunVisual/CharcoalRubber").GetComponent<MeshFilter>().sharedMesh;
        grip.vertices = originalGrip.vertices; grip.normals = originalGrip.normals; grip.RecalculateTangents(); grip.RecalculateBounds(); EditorUtility.SetDirty(grip);
        foreach (var sprite in All<SpriteRenderer>(scene).Where(r => r.name.StartsWith("Graffiti_", StringComparison.Ordinal))) CreatePaintRelief(sprite);
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Refined paint bake failed.");
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        VerifySaved();
        Debug.Log("MATERIAL_MODEL_REVIEW_OK: smooth alpha contours with physical paint rims; original rounded grip geometry retained with teal color; no blur/Bloom change.");
    }

    public static void PolishGrip()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        var material = new Material(Load<Material>(Root + "/Materials/WeaponTealGrip.mat")) { name = "WeaponGripSoft" };
        material.SetTexture("_BaseMap", null); material.SetTexture("_BumpMap", null); material.DisableKeyword("_NORMALMAP");
        material.SetFloat("_Smoothness", .26f);
        material = Persist(material, Root + "/Materials/WeaponGripSoft.mat");
        var renderer = All<PlayerShooter>(scene).Single().transform.Find("RefinedGunVisual/CharcoalRubber").GetComponent<MeshRenderer>();
        renderer.sharedMaterials = Enumerable.Repeat(material, renderer.sharedMaterials.Length).ToArray(); Record(renderer);
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene); VerifySaved();
        Debug.Log("MATERIAL_GRIP_POLISH_OK: continuous teal grip/rear trim; no triangle-level dark material boundary or exaggerated micro-normal.");
    }

    public static void VerifySaved()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        if (LightmapSettings.lightmaps.Length == 0 || LightmapSettings.lightProbes == null || LightmapSettings.lightProbes.count != 75)
            throw new InvalidOperationException("Copied study bake/probes not saved.");
        int missing = All<Transform>(scene).Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
        if (missing != 0) throw new InvalidOperationException("Missing scripts.");
        var aimCheck = typeof(StudioCameraLightingPass).GetMethod("ValidateAim", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (aimCheck == null) throw new InvalidOperationException("Existing camera/shot-path checker missing.");
        aimCheck.Invoke(null, new object[] { scene });
        var latex = Load<Material>(Root + "/Materials/SoftTealLatex.mat");
        CaptureTargetLayout(scene, "After_Room", latex, false);
        CaptureGunPose(scene, "After_Gun", false); CaptureGunPose(scene, "After_WeaponInspection", true);
        AssetDatabase.Refresh();
        Debug.Log("MATERIAL_APPEARANCE_SAVED_OK: reloaded study; unchanged camera/lights/postprocessing, 75 probes, cloned latex prefab, cream plaster, dry oak, three raised paint meshes and four-color weapon.");
    }

    public static void FinalizeReview()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        var gun = All<PlayerShooter>(scene).Single().gameObject; bool active = gun.activeSelf;
        var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection");
        string path = Root + "/Reflections/MaterialStudio.exr";
        try { gun.SetActive(false); if (!Lightmapping.BakeReflectionProbe(probe, path)) throw new IOException("Final reflection capture failed."); }
        finally { gun.SetActive(active); }
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        probe.customBakedTexture = Load<Cubemap>(path); Record(probe); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        VerifySaved();
        Debug.Log("MATERIAL_FINAL_OK: final reflection, saved scene, complete target motion bounds and nine camera/muzzle sphere-cast paths checked.");
    }

    static void CaptureTargetLayout(Scene scene, string name, Material latex, bool single)
    {
        var prefab = Load<GameObject>(BalloonSource);
        var targets = new List<GameObject>();
        var positions = single ? new[] { new Vector3(0, .3f, 8) } :
            new[] { new Vector3(-2, -.5f, 8), new Vector3(0, .9f, 8), new Vector3(2, -.5f, 8) };
        try
        {
            foreach (var position in positions)
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                targets.Add(target); target.transform.position = position;
                if (latex != null) foreach (var r in target.GetComponentsInChildren<MeshRenderer>()) r.sharedMaterial = latex;
            }
            Capture(scene, name);
        }
        finally { foreach (var target in targets) UnityEngine.Object.DestroyImmediate(target); }
    }

    static void CaptureGunPose(Scene scene, string name, bool inspection)
    {
        var gun = All<PlayerShooter>(scene).Single().transform;
        var localPosition = gun.localPosition; var localRotation = gun.localRotation; var localScale = gun.localScale;
        try
        {
            if (inspection)
            {
                var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
                gun.position = camera.transform.TransformPoint(new Vector3(0, -.03f, 1.8f));
                gun.rotation = camera.transform.rotation * Quaternion.Euler(0, 120, 0);
                gun.localScale = localScale * 3.4f;
            }
            else gun.LookAt(new Vector3(0, 0, 8));
            Capture(scene, name);
        }
        finally { gun.localPosition = localPosition; gun.localRotation = localRotation; gun.localScale = localScale; }
    }

    public static void Capture(Scene scene, string name)
    {
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active; float aspect = camera.aspect;
        bool async = ShaderUtil.allowAsyncCompilation;
        var target = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        try
        {
            ShaderUtil.allowAsyncCompilation = false; target.Create(); camera.targetTexture = target; camera.aspect = 16f / 9f;
            camera.Render(); var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
            if (RenderPipeline.SupportsRenderRequest(camera, request)) RenderPipeline.SubmitRenderRequest(camera, request); else camera.Render();
            RenderTexture.active = target; pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); pixels.Apply();
            File.WriteAllBytes(Root + "/Previews/" + name + ".png", pixels.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = oldTarget; camera.aspect = aspect; RenderTexture.active = oldActive;
            ShaderUtil.allowAsyncCompilation = async; target.Release(); UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(pixels);
        }
    }
}
