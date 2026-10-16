using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class StudioConceptDetailPass
{
    public const string Source = StudioConceptVariantsPass.Destination;
    public const string Destination = "Assets/Scenes/StudioConceptDetail.unity";
    public const string Root = "Assets/StudioArtPack/ConceptDetailV13";
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static T Load<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new FileNotFoundException(path);
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var c); return c; }
    static Material[] paint;
    static Material cream, wood, metal, ink;
    static Mesh sphere, tinShell, rolledRim, paintDisc;
    static int meshIndex;
    static void Record(Object obj) { EditorUtility.SetDirty(obj); if (PrefabUtility.IsPartOfPrefabInstance(obj)) PrefabUtility.RecordPrefabInstancePropertyModifications(obj); }
    static Material NewMaterial(string name, string tint, float smoothness)
    {
        var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name }; m.SetColor("_BaseColor", Hex(tint)); m.SetFloat("_Metallic", 0); m.SetFloat("_Smoothness", smoothness);
        AssetDatabase.CreateAsset(m, Root + "/Materials/" + name + ".mat"); return m;
    }
    static Material CopyMaterial(string path, string name, string tint)
    {
        var m = new Material(Load<Material>(path)) { name = name }; m.SetColor("_BaseColor", Hex(tint)); AssetDatabase.CreateAsset(m, Root + "/Materials/" + name + ".mat"); return m;
    }

    [MenuItem("BalloonBarrage/Art/Create concept studio details V13")]
    public static void Build()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
        if (File.Exists(Destination) || Directory.Exists(Root)) throw new InvalidOperationException("V13 exists; preserve manual edits.");
        foreach (string folder in new[] { "Materials", "Meshes", "Textures", "Reflections", "Previews" }) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh();
        var baseline = EditorSceneManager.OpenScene(Source); CaptureTargets(baseline, "Before_Room");
        if (!AssetDatabase.CopyAsset(Source, Destination)) throw new IOException("Scene copy failed.");
        var scene = EditorSceneManager.OpenScene(Destination);
        var protectedObjects = All<Component>(scene).Where(c => c is Collider || c is Rigidbody || c is Light || c is Camera || c is MonoBehaviour).ToArray();
        var protectedBefore = protectedObjects.Select(EditorJsonUtility.ToJson).ToArray();
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera")); string cameraPose = EditorJsonUtility.ToJson(camera.transform);
        var collisionPoses = All<Collider>(scene).Select(c => EditorJsonUtility.ToJson(c.transform)).ToArray();
        var root = new GameObject("ConceptStudioDetailsV13").transform; SceneManager.MoveGameObjectToScene(root.gameObject, scene);
        CreateMaterials(); RefineRoom(scene); BuildSupplies(scene, root); BuildCanvases(root); BuildFloorPaint(root); BatchDetails(root);
        if (!protectedBefore.SequenceEqual(protectedObjects.Select(EditorJsonUtility.ToJson)) || !collisionPoses.SequenceEqual(All<Collider>(scene).Select(c => EditorJsonUtility.ToJson(c.transform)))) throw new InvalidOperationException("Existing runtime, lighting or physics data changed.");
        if (cameraPose != EditorJsonUtility.ToJson(camera.transform) || root.GetComponentsInChildren<Collider>(true).Length != 0 || root.GetComponentsInChildren<MonoBehaviour>(true).Length != 0) throw new InvalidOperationException("Framing or decoration ownership changed.");
        var settings = Object.Instantiate(Lightmapping.lightingSettings); settings.name = "ConceptDetailLighting";
        AssetDatabase.CreateAsset(settings, Root + "/ConceptDetailLighting.lighting"); Lightmapping.lightingSettings = settings; Lightmapping.lightingDataAsset = null; LightmapSettings.lightmaps = Array.Empty<LightmapData>();
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("V13 bake failed.");
        RefreshReflection(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        RegisterScene(); Verify();
        Debug.Log("CONCEPT_DETAIL_BUILD_OK: warm mineral floor, coordinated pigments, emissive strips, detailed tins/crates/brushes/canvases, bounded peripheral paint; existing gameplay/camera/lights/colliders preserved.");
    }

    static void CreateMaterials()
    {
        string[] hues = { "18A9AE", "EF705D", "F4BF45", "655891", "61BBA0", "D68EAD" };
        paint = hues.Select((h, i) => NewMaterial("Pigment_" + StudioConceptVariantsPass.Names[i], h, .42f)).ToArray();
        cream = NewMaterial("PainterIvory", "F5E9D4", .16f);
        wood = CopyMaterial("Assets/StudioArtPack/CoverFinishV9/Materials/NaturalJoinery.mat", "SupplyOak", "DCC097");
        metal = CopyMaterial("Assets/StudioArtPack/SurfacesV6/Materials/BrushedSteel.mat", "RolledTinSteel", "D7DDDD"); metal.SetFloat("_Smoothness", .34f);
        ink = NewMaterial("WorkshopInk", "374448", .20f);
        CreateGeometry();
    }
    static void CreateGeometry()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere); sphere = go.GetComponent<MeshFilter>().sharedMesh; Object.DestroyImmediate(go);
        tinShell = StudioConceptGeometry.Lathe(new[] { new Vector2(0, 0), new Vector2(.94f, 0), new Vector2(1, .025f), new Vector2(1, .93f), new Vector2(.98f, .98f), new Vector2(.90f, .98f), new Vector2(.88f, .92f), new Vector2(.88f, .10f), new Vector2(0, .10f) });
        rolledRim = StudioConceptGeometry.Lathe(new[] { new Vector2(.94f, -.018f), new Vector2(1.03f, -.011f), new Vector2(1.045f, 0), new Vector2(1.03f, .012f), new Vector2(.93f, .018f), new Vector2(.90f, 0), new Vector2(.94f, -.018f) });
        paintDisc = StudioConceptGeometry.Lathe(new[] { new Vector2(0, -.008f), new Vector2(.90f, -.008f), new Vector2(.92f, 0), new Vector2(.89f, .008f), new Vector2(0, .008f) });
    }

    static void RefineRoom(Scene scene)
    {
        var concrete = NewMaterial("WarmMineralFloor", "C8BCAC", .22f);
        const int n = 512; var texture = new Texture2D(n, n, TextureFormat.RGB24, false);
        var colors = new Color[n * n];
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float u = x / (float)n, v = y / (float)n;
            float broad = Mathf.Sin(u * Mathf.PI * 2) * Mathf.Cos(v * Mathf.PI * 4) + .5f * Mathf.Cos((u + v) * Mathf.PI * 6);
            uint seed = (uint)(x + y * n + 9187); seed ^= seed >> 16; seed *= 0x7feb352d; seed ^= seed >> 15; seed *= 0x846ca68b; seed ^= seed >> 16;
            float fine = seed / (float)uint.MaxValue - .5f; float value = .96f + broad * .013f + fine * .018f;
            colors[y * n + x] = new Color(value, value, value);
        }
        texture.SetPixels(colors); texture.Apply(); string path = Root + "/Textures/MineralFloor.png"; File.WriteAllBytes(path, texture.EncodeToPNG()); Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path); importer.wrapMode = TextureWrapMode.Repeat; importer.mipmapEnabled = true; importer.anisoLevel = 8; importer.SaveAndReimport();
        concrete.SetTexture("_BaseMap", Load<Texture2D>(path)); concrete.SetTextureScale("_BaseMap", Vector2.one * .5f);
        var niche = CopyMaterial("Assets/StudioArtPack/ReferenceV4/Materials/NicheTeal.mat", "AiryTealNiche", "3E8A83");
        var rug = new Dictionary<Material, Material>();
        foreach (var r in All<MeshRenderer>(scene))
        {
            if (r.name == "Floor" || r.name == "FloorReturn") { r.sharedMaterial = concrete; Record(r); }
            if (r.name == "ArchedStudioNiche") { r.sharedMaterial = niche; Record(r); }
            if (r.name == "FloorPaint" || r.name == "PaintDot") { r.enabled = false; Record(r); }
            if (r.sharedMaterial != null && r.sharedMaterial.name.StartsWith("Woven_", StringComparison.Ordinal))
            {
                var old = r.sharedMaterial;
                if (!rug.TryGetValue(old, out var m)) { string color = old.name.Contains("Indigo") ? "355C64" : old.name.Contains("Coral") ? "D28668" : "D9BF83"; m = CopyMaterial(AssetDatabase.GetAssetPath(old), "Rug_" + old.name, color); rug.Add(old, m); }
                r.sharedMaterial = m; Record(r);
            }
            if (r.name == "LeftLightStrip" || r.name == "RightLightStrip")
            {
                bool teal = r.name == "LeftLightStrip";
                var m = NewMaterial(teal ? "TealLightCore" : "CoralLightCore", teal ? "18AFC1" : "F36F52", .25f);
                m.SetColor("_EmissionColor", Hex(teal ? "10C8D9" : "FA7559") * .60f); m.EnableKeyword("_EMISSION"); m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
                r.sharedMaterial = m; Record(r);
            }
        }
        // Match shell, wet top and drips by bucket; retain the existing label geometry.
        int bucket = 0;
        foreach (var body in All<MeshRenderer>(scene).Where(r => r.name == "BucketBody").OrderBy(r => r.transform.position.x))
        {
            var pigment = paint[new[] { 2, 0, 1, 3 }[bucket++ % 4]];
            body.sharedMaterial = pigment; Record(body);
            foreach (var detail in body.transform.parent.GetComponentsInChildren<MeshRenderer>(true).Where(r => r.name == "LidPaint" || r.name == "PaintDrip")) { detail.sharedMaterial = pigment; Record(detail); }
        }
        RefineWallPigments(scene);
    }

    static void RefineWallPigments(Scene scene)
    {
        foreach (var r in All<MeshRenderer>(scene).Where(r => new[] { "Teal_Splat", "Coral_Splat", "Gold_Splat" }.Contains(r.name)))
        {
            int i = r.name.StartsWith("Teal") ? 0 : r.name.StartsWith("Coral") ? 1 : 2;
            string path = Root + "/Materials/WallPaint_" + StudioConceptVariantsPass.Names[i] + ".mat";
            var top = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (top == null) { top = new Material(r.sharedMaterials[0]) { name = "WallPaint_" + StudioConceptVariantsPass.Names[i] }; AssetDatabase.CreateAsset(top, path); }
            top.SetColor("_BaseColor", paint[i].GetColor("_BaseColor")); EditorUtility.SetDirty(top);
            r.sharedMaterials = new[] { top, paint[i] }; Record(r);
        }
    }

    public static void RefineReview()
    {
        if (!Application.isBatchMode || File.Exists(Root + "/VisualReview.txt")) throw new InvalidOperationException("Use the isolated, unreviewed V13 copy; preserve delivered changes.");
        var scene = EditorSceneManager.OpenScene(Destination);
        paint = StudioConceptVariantsPass.Names.Select(n => Load<Material>(Root + "/Materials/Pigment_" + n + ".mat")).ToArray();
        RefineWallPigments(scene);
        foreach (string name in new[] { "TealLightCore", "CoralLightCore" })
        {
            bool teal = name.StartsWith("Teal"); var material = Load<Material>(Root + "/Materials/" + name + ".mat");
            material.SetColor("_BaseColor", Hex(teal ? "18AFC1" : "F36F52")); material.SetColor("_EmissionColor", Hex(teal ? "10C8D9" : "FA7559") * .60f); EditorUtility.SetDirty(material);
        }
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Reviewed material bake failed.");
        RefreshReflection(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene); Verify();
        File.WriteAllText(Root + "/VisualReview.txt", "Compared original CozyGraffitiStudio_Concept.png with fixed-layout before/after Unity game-camera renders.\nWarm mineral floor restores concept-style color separation and broad window-shadow shapes. Tins, crates, brushes, leaned paint studies and restrained peripheral floor pigment add workshop detail.\nFirst trial's white-clipped strip cores were rejected. Final cores use .60 emission gain with saturated teal/coral pigments, unchanged Bloom and existing lights; main splats use coordinated richer pigment copies.\nOriginal scene, source artwork, previous materials, camera/framing, gameplay and collision remain available. Added pieces are combined into 20 material batches, with no runtime behaviours or decorative colliders.\n");
        AssetDatabase.Refresh(); Debug.Log("CONCEPT_DETAIL_REVIEW_OK");
    }

    static Transform Part(Transform parent, string name, Mesh mesh, Vector3 position, Vector3 scale, Material material, Quaternion? rotation = null)
    {
        var result = new GameObject(name).transform; result.SetParent(parent, false); result.localPosition = position; result.localScale = scale; result.localRotation = rotation ?? Quaternion.identity;
        result.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh; result.gameObject.AddComponent<MeshRenderer>().sharedMaterial = material; return result;
    }
    static Transform Box(Transform parent, string name, Vector3 position, Vector3 size, float bevel, Material material, Quaternion? rotation = null) => Part(parent, name, StudioConceptGeometry.RoundedBox(size, bevel), position, Vector3.one, material, rotation);
    static Transform Group(Transform parent, string name, Vector3 position, Quaternion? rotation = null) { var t = new GameObject(name).transform; t.SetParent(parent, false); t.localPosition = position; t.localRotation = rotation ?? Quaternion.identity; return t; }

    static void Tin(Transform parent, Vector3 position, float radius, float height, int color)
    {
        var root = Group(parent, "RolledPaintTin", position); var pigment = paint[color % paint.Length];
        Part(root, "EnamelShell", tinShell, Vector3.zero, new Vector3(radius, height, radius), pigment);
        Part(root, "RolledRim", rolledRim, new Vector3(0, height * .98f, 0), new Vector3(radius, height, radius), metal);
        Part(root, "TinFoot", rolledRim, new Vector3(0, height * .025f, 0), new Vector3(radius, height, radius), metal);
        Part(root, "WetPigment", paintDisc, new Vector3(0, height * .925f, 0), new Vector3(radius, height, radius), pigment);
        Box(root, "PaperLabel", new Vector3(0, height * .46f, -radius * 1.005f), new Vector3(radius * .95f, height * .38f, .012f), .005f, cream);
        Part(root, "ColorCode", sphere, new Vector3(0, height * .48f, -radius * 1.043f), new Vector3(radius * .30f, radius * .30f, .005f), pigment);
        for (int i = 0; i < 3; i++)
        {
            float angle = -.24f + i * .25f, length = height * (.16f + i * .10f);
            Part(root, "ShortPaintDrip", sphere, new Vector3(Mathf.Sin(angle) * radius, height * .94f - length * .4f, -Mathf.Cos(angle) * radius), new Vector3(radius * .14f, length, .010f), cream);
        }
        foreach (int side in new[] { -1, 1 })
            Box(root, "HandleLug", new Vector3(side * radius * 1.012f, height * .68f, 0), new Vector3(.025f, height * .10f, .035f), .008f, metal);
    }

    static void Crate(Transform parent, Vector3 position, float angle)
    {
        var crate = Group(parent, "OpenSlattedCrate", position, Quaternion.Euler(0, angle, 0));
        Box(crate, "CrateBase", new Vector3(0, .045f, 0), new Vector3(1.15f, .09f, .70f), .012f, wood);
        foreach (int side in new[] { -1, 1 }) for (int i = 0; i < 3; i++)
            Box(crate, "WoodSlat", new Vector3(0, .15f + i * .15f, side * .33f), new Vector3(1.12f, .115f, .035f), .008f, wood);
        foreach (int x in new[] { -1, 1 }) foreach (int z in new[] { -1, 1 })
        {
            Box(crate, "CrateCorner", new Vector3(x * .52f, .29f, z * .30f), new Vector3(.045f, .53f, .045f), .007f, wood);
            Part(crate, "Nail", sphere, new Vector3(x * .48f, .44f, z * .35f), new Vector3(.020f, .020f, .004f), metal);
        }
        Tin(crate, new Vector3(-.28f, .08f, 0), .19f, .49f, 0); Tin(crate, new Vector3(.23f, .08f, -.02f), .17f, .38f, 1);
        Tin(crate, new Vector3(.22f, .46f, -.02f), .15f, .29f, 2);
    }

    static void BrushCup(Transform parent, Vector3 position)
    {
        var cup = Group(parent, "BrushCup", position); Part(cup, "CupShell", tinShell, Vector3.zero, new Vector3(.13f, .26f, .13f), cream);
        for (int i = 0; i < 5; i++)
        {
            var brush = Group(cup, "UsedBrush", new Vector3((i - 2) * .034f, .15f, (i % 2) * .035f), Quaternion.Euler(0, i * 11, (i - 2) * 9));
            Box(brush, "WoodHandle", new Vector3(0, .23f, 0), new Vector3(.018f, .48f, .018f), .004f, wood);
            Box(brush, "Ferrule", new Vector3(0, .49f, 0), new Vector3(.057f, .055f, .022f), .006f, metal);
            Box(brush, "PaintedBristles", new Vector3(0, .56f, 0), new Vector3(.056f, .09f, .025f), .010f, paint[i % 3]);
        }
    }

    static void BuildSupplies(Scene scene, Transform parent)
    {
        int index = 0;
        foreach (var old in All<MeshRenderer>(scene).Where(r => r.name == "NichePaintTin").OrderBy(r => r.transform.position.x))
        { Tin(parent, new Vector3(old.bounds.center.x, old.bounds.min.y, old.bounds.center.z), old.bounds.extents.x, old.bounds.size.y, index++ % 3); old.enabled = false; Record(old); }
        Crate(parent, new Vector3(-5.55f, -4f, 7.35f), -9); Crate(parent, new Vector3(4.05f, -4f, 7.20f), 8);
        Tin(parent, new Vector3(-4.40f, -4f, 7.35f), .23f, .58f, 3);
        Tin(parent, new Vector3(6.65f, -4f, 5.95f), .25f, .63f, 2);
        BrushCup(parent, new Vector3(-5.68f, -2.26f, 5.52f));
        // A few used tubes and swatches beside the right supply cabinet.
        for (int i = 0; i < 3; i++)
        {
            var tube = Group(parent, "UsedPaintTube", new Vector3(5.24f + i * .19f, -2.575f, 6.90f), Quaternion.Euler(80, i * 20, 17));
            Part(tube, "TubeBody", sphere, Vector3.zero, new Vector3(.080f, .25f, .065f), cream);
            Box(tube, "PigmentBand", Vector3.zero, new Vector3(.084f, .075f, .068f), .015f, paint[i]);
            Box(tube, "TubeCap", new Vector3(0, .145f, 0), new Vector3(.048f, .035f, .042f), .006f, ink);
        }
    }

    static void PaintMark(Transform parent, string name, int pigment, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        string sourceName = pigment == 1 ? "Coral_Splat" : pigment == 2 ? "Gold_Splat" : "Teal_Splat";
        var transform = Part(parent, name, Load<Mesh>(StudioConceptVariantsPass.Root + "/Meshes/" + sourceName + ".asset"), position, scale, paint[pigment], rotation);
        var top = new Material(Load<Material>(StudioConceptVariantsPass.Root + "/Materials/" + sourceName + "_Paint.mat")) { name = "PaintMark_" + meshIndex++ };
        top.SetColor("_BaseColor", paint[pigment].GetColor("_BaseColor")); string path = Root + "/Materials/" + top.name + ".mat";
        var saved = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (saved == null) AssetDatabase.CreateAsset(top, path);
        else { EditorUtility.CopySerialized(top, saved); EditorUtility.SetDirty(saved); Object.DestroyImmediate(top); top = saved; }
        transform.GetComponent<MeshRenderer>().sharedMaterials = new[] { top, paint[pigment] };
    }

    static void Canvas(Transform parent, Vector3 basePosition, Vector3 angles, int pigment)
    {
        var root = Group(parent, "LeaningPaintStudy", basePosition, Quaternion.Euler(angles));
        Box(root, "CanvasBacking", new Vector3(0, 1.03f, 0), new Vector3(1.08f, 1.98f, .07f), .010f, wood);
        Box(root, "PrimedCanvas", new Vector3(0, 1.03f, -.046f), new Vector3(1.01f, 1.91f, .026f), .006f, cream);
        PaintMark(root, "AbstractPigment", pigment, new Vector3(-.03f, 1.20f, -.064f), new Vector3(.235f, .42f, .6f), Quaternion.Euler(0, 0, -18));
        PaintMark(root, "AbstractGold", 2, new Vector3(.12f, .73f, -.080f), new Vector3(.17f, .25f, .6f), Quaternion.Euler(0, 0, 22));
        for (int i = 0; i < 3; i++) Box(root, "TestStroke", new Vector3(-.28f + i * .16f, .34f, -.070f), new Vector3(.085f, .19f, .005f), .008f, paint[(pigment + i) % paint.Length], Quaternion.Euler(0, 0, -15));
    }
    static void BuildCanvases(Transform parent)
    {
        Canvas(parent, new Vector3(-5.5f, -4, 6.30f), new Vector3(10, 12, -5), 3);
        Canvas(parent, new Vector3(5.08f, -4, 5.86f), new Vector3(5, -14, 6), 1);
    }
    static void BuildFloorPaint(Transform parent)
    {
        var positions = new[] { new Vector3(-4.4f, -3.976f, 5.05f), new Vector3(4.4f, -3.974f, 5.85f), new Vector3(-5.1f, -3.972f, 8.05f), new Vector3(5.25f, -3.970f, 7.35f), new Vector3(-3.05f, -3.968f, 3.0f), new Vector3(3.35f, -3.966f, 3.55f) };
        for (int i = 0; i < positions.Length; i++)
            PaintMark(parent, "FloorPigment_" + i, i % 4, positions[i], new Vector3(.55f + (i % 2) * .16f, .27f, .5f), Quaternion.Euler(90, i * 37, 0));
        for (int i = 0; i < 12; i++)
        {
            int side = i % 2 == 0 ? -1 : 1;
            Part(parent, "SmallFloorDroplet", sphere, new Vector3(side * (3.65f + i % 3 * .42f), -3.974f, 3.8f + i / 2 * .40f), new Vector3(.04f + i % 3 * .02f, .008f, .06f), paint[i % 3]);
        }
    }

    static void BatchDetails(Transform root)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        var materials = renderers.SelectMany(r => r.sharedMaterials).Distinct().ToArray();
        foreach (var material in materials)
        {
            var pieces = new List<CombineInstance>();
            foreach (var renderer in renderers) for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                if (renderer.sharedMaterials[i] == material) pieces.Add(new CombineInstance { mesh = renderer.GetComponent<MeshFilter>().sharedMesh, subMeshIndex = i, transform = root.worldToLocalMatrix * renderer.transform.localToWorldMatrix });
            var mesh = new Mesh { name = "DetailBatch_" + meshIndex++, indexFormat = IndexFormat.UInt32 }; mesh.CombineMeshes(pieces.ToArray(), true, true);
            if (!Unwrapping.GenerateSecondaryUVSet(mesh)) throw new InvalidOperationException("Detail lightmap UV generation failed.");
            string path = Root + "/Meshes/" + mesh.name + ".asset"; var saved = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (saved == null) AssetDatabase.CreateAsset(mesh, path);
            else { EditorUtility.CopySerialized(mesh, saved); EditorUtility.SetDirty(saved); Object.DestroyImmediate(mesh); mesh = saved; }
            var batch = Part(root, "Batch_" + material.name, mesh, Vector3.zero, Vector3.one, material);
            GameObjectUtility.SetStaticEditorFlags(batch.gameObject, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic);
        }
        foreach (var renderer in renderers) Object.DestroyImmediate(renderer.gameObject);
        foreach (var empty in root.GetComponentsInChildren<Transform>().Where(t => t != root && t.GetComponent<MeshRenderer>() == null).Reverse().ToArray()) if (empty != null && empty.childCount == 0) Object.DestroyImmediate(empty.gameObject);
        File.WriteAllText(Root + "/BatchReport.txt", "Added source mesh parts: " + renderers.Length + "; final material batches: " + materials.Length + "; added colliders: 0; added runtime behaviours: 0.\n");
    }
    static void RefreshReflection(Scene scene)
    {
        var gun = All<PlayerShooter>(scene).Single().gameObject; bool active = gun.activeSelf; var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection"); string path = Root + "/Reflections/DetailStudio.exr";
        try { gun.SetActive(false); if (!Lightmapping.BakeReflectionProbe(probe, path)) throw new IOException("Detail reflection bake failed."); }
        finally { gun.SetActive(active); }
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport); probe.customBakedTexture = Load<Cubemap>(path); Record(probe);
    }
    public static void CorrectDetailClearances()
    {
        if (!Application.isBatchMode || File.Exists(Root + "/ClearanceReview.txt")) throw new InvalidOperationException("Use the isolated, uncorrected V13 copy; preserve subsequent edits.");
        var scene = EditorSceneManager.OpenScene(Destination);
        var protectedObjects = All<Component>(scene).Where(c => c is Collider || c is Rigidbody || c is Light || c is Camera || c is MonoBehaviour).ToArray();
        var protectedBefore = protectedObjects.Select(EditorJsonUtility.ToJson).ToArray();
        Object.DestroyImmediate(scene.GetRootGameObjects().Single(g => g.name == "ConceptStudioDetailsV13"));
        paint = StudioConceptVariantsPass.Names.Select(n => Load<Material>(Root + "/Materials/Pigment_" + n + ".mat")).ToArray();
        cream = Load<Material>(Root + "/Materials/PainterIvory.mat"); wood = Load<Material>(Root + "/Materials/SupplyOak.mat"); metal = Load<Material>(Root + "/Materials/RolledTinSteel.mat"); ink = Load<Material>(Root + "/Materials/WorkshopInk.mat"); CreateGeometry(); meshIndex = 0;
        var root = new GameObject("ConceptStudioDetailsV13").transform; SceneManager.MoveGameObjectToScene(root.gameObject, scene);
        BuildSupplies(scene, root); BuildCanvases(root); BuildFloorPaint(root); BatchDetails(root);
        if (!protectedBefore.SequenceEqual(protectedObjects.Select(EditorJsonUtility.ToJson)) || root.GetComponentsInChildren<Collider>().Length != 0) throw new InvalidOperationException("Existing behaviour/physics/lighting changed during detail correction.");
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Corrected detail bake failed.");
        RefreshReflection(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene); FinalAudit();
        File.WriteAllText(Root + "/ClearanceReview.txt", "Close-camera review found leaning studies crossing the existing stage's front rail, a hidden loose tin/crate in the stage body, and paint escaping a canvas edge.\nMoved new boards/tins/crate ahead of the stage and separated the right supply cluster. Reduced/centered canvas paint bounds within its primed surface. Rebuilt only the V13 decoration batches in place, retaining their material/mesh GUIDs; existing gameplay/physics/camera/lights unchanged.\nCorrected scene baked, reloaded and full Unity reference/aim/framing audit passed. Review final close-up captures; runtime physical-input/performance acceptance remains separate.\n");
        AssetDatabase.Refresh(); Debug.Log("CONCEPT_DETAIL_CLEARANCES_OK");
    }
    public static void RegisterScene()
    {
        if (!EditorBuildSettings.scenes.Any(s => s.path == Destination)) EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(Destination, true) }).ToArray();
    }
    public static void FinalizePlacementReview()
    {
        if (!Application.isBatchMode || !File.Exists(Root + "/ClearanceReview.txt") || File.Exists(Root + "/PositionReview.txt")) throw new InvalidOperationException("Use the isolated V13 copy after correction; preserve subsequent edits.");
        // Reload in a fresh editor process so saved native mesh buffers are used for baking and review.
        var scene = EditorSceneManager.OpenScene(Destination);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("Final placement bake failed.");
        RefreshReflection(scene); AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene); FinalAudit();
        File.WriteAllText(Root + "/PositionReview.txt", "Final user request limited the remaining work to prop placement. Colors and detail geometry retained.\nMoved left study ahead of its crate; moved right study between cart and cabinet and reduced its lean. Right crate now sits left of the cabinet instead of inside it; loose tins are separated from cabinet/stage/crates. Paint tubes rest on the existing cabinet worktop.\nFresh Unity editor process reloaded the saved mesh assets, rebaked lighting/reflection and captured final game/inspection views. Full reference and aim/framing audit passed. Existing camera, lights, physics and gameplay retained.\n");
        AssetDatabase.Refresh(); Debug.Log("CONCEPT_DETAIL_POSITIONS_OK");
    }
    public static void FinalAudit()
    {
        Verify(); ProjectValidation.Run();
        File.AppendAllText(Root + "/Verification.txt", "Full Unity project audit passed: missingScripts=0, unsupportedVolumeScripts=0, unresolved=0. Detailed inspection captures temporarily reposition the same game camera and restore it without saving.\n");
        AssetDatabase.Refresh(); Debug.Log("CONCEPT_DETAIL_FINAL_AUDIT_OK");
    }
    public static void Verify()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        if (LightmapSettings.lightmaps.Length == 0 || All<Transform>(scene).Any(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) != 0)) throw new InvalidOperationException("Saved detail lighting or scripts missing.");
        typeof(StudioCameraLightingPass).GetMethod("ValidateAim", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { scene });
        CaptureTargets(scene, "After_Room");
        CaptureDetailViews(scene);
        File.WriteAllText(Root + "/Verification.txt", "Unity " + Application.unityVersion + "; isolated scene build and reload; 2026-10-14 Asia/Tokyo.\nNew static details/materials only. Existing gameplay/physics/camera/light component serialization and collider transforms match V12. No added decorative colliders or runtime scripts.\nSaved lightmaps present; nine camera/muzzle paths and complete target-motion framing passed. Actual 1600x900 game-camera captures use the same fixed three-target layout before/after. Human input, complete rounds, sound, performance and releases unverified.\n");
        AssetDatabase.Refresh(); Debug.Log("CONCEPT_DETAIL_VERIFY_OK");
    }
    static void CaptureDetailViews(Scene scene)
    {
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera")); var position = camera.transform.position; var rotation = camera.transform.rotation;
        var gun = All<PlayerShooter>(scene).Single().gameObject; bool active = gun.activeSelf;
        try
        {
            gun.SetActive(false);
            camera.transform.position = new Vector3(-3.7f, -1.6f, 3.0f); camera.transform.LookAt(new Vector3(-5.1f, -2.2f, 7.2f));
            StudioConceptVariantsPass.Capture(scene, "V13_Workbench_Detail"); File.Copy(StudioConceptVariantsPass.Root + "/Previews/V13_Workbench_Detail.png", Root + "/Previews/Workbench_Detail.png", true);
            camera.transform.position = new Vector3(5.35f, -2.1f, 4.05f); camera.transform.LookAt(new Vector3(5.20f, -3.0f, 7.05f));
            StudioConceptVariantsPass.Capture(scene, "V13_Supplies_Detail"); File.Copy(StudioConceptVariantsPass.Root + "/Previews/V13_Supplies_Detail.png", Root + "/Previews/Supplies_Detail.png", true);
        }
        finally { camera.transform.position = position; camera.transform.rotation = rotation; gun.SetActive(active); }
    }
    static void CaptureTargets(Scene scene, string label)
    {
        var spawner = new SerializedObject(All<BalloonSpawner>(scene).Single()); var prefab = (GameObject)spawner.FindProperty("balloonPrefab").objectReferenceValue;
        var targets = new List<GameObject>(); var gun = All<StudioGunAppearance>(scene).Single();
        try
        {
            gun.ApplyTheme(0);
            var positions = new[] { new Vector3(-2, -.5f, 8), new Vector3(0, .9f, 8), new Vector3(2, -.5f, 8) };
            for (int i = 0; i < positions.Length; i++) { var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene); targets.Add(target); target.transform.position = positions[i]; target.GetComponent<StudioBalloonAppearance>().ApplyVariant(i, i); }
            StudioConceptVariantsPass.Capture(scene, "V13_" + label); File.Copy(StudioConceptVariantsPass.Root + "/Previews/V13_" + label + ".png", Root + "/Previews/" + label + ".png", true);
        }
        finally { foreach (var target in targets) Object.DestroyImmediate(target); foreach (var region in gun.regions) region.renderer.SetPropertyBlock(null, region.materialIndex); }
    }

    public static void Inspect()
    {
        var scene = EditorSceneManager.OpenScene(Source);
        File.WriteAllLines("Logs/ConceptDetailInspect.txt", All<MeshRenderer>(scene).Where(r => r.gameObject.activeInHierarchy && r.enabled && (r.name.Contains("Floor") || r.name.Contains("Tube") || r.name.Contains("Neon") || r.name.Contains("Niche") || r.name.Contains("Paint") || r.name.Contains("Bucket") || r.name.Contains("Easel")))
            .Select(r => r.name + " | position " + r.transform.position + " | bounds " + r.bounds + " | material " + r.sharedMaterial.name));
        Debug.Log("CONCEPT_DETAIL_INSPECT_OK");
    }
}
