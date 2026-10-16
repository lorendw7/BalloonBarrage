using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>Opt-in concept pass. Build in an isolated project; keep all earlier scenes/assets.</summary>
public static class StudioConceptVariantsPass
{
    public const string Root = "Assets/StudioArtPack/ConceptVariantsV12";
    public const string Destination = "Assets/Scenes/StudioConceptVariants.unity";
    public static readonly string[] Names = { "Teal", "Coral", "Gold", "Indigo", "Mint", "Rose" };
    public static readonly string[] Colors = { "29B8B6", "F77769", "FFD05B", "7976BC", "67C9AB", "DFA0C1" };
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static T Load<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new FileNotFoundException(path);
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#" + value, out var color); return color; }
    static void Record(Object obj)
    {
        EditorUtility.SetDirty(obj);
        if (PrefabUtility.IsPartOfPrefabInstance(obj)) PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
    }
    static Material Material(string name, Color color, float gloss)
    {
        var result = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
        result.SetColor("_BaseColor", color); result.SetFloat("_Metallic", 0); result.SetFloat("_Smoothness", gloss);
        AssetDatabase.CreateAsset(result, Root + "/Materials/" + name + ".mat"); return result;
    }
    static string[] CollisionAudit(Scene scene) => All<Collider>(scene).Select(EditorJsonUtility.ToJson).ToArray();
    static string[] LearnerAudit(Scene scene) => All<MonoBehaviour>(scene).Where(c => c != null && AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(c)).StartsWith("Assets/Scripts/", StringComparison.Ordinal)).Select(EditorJsonUtility.ToJson).ToArray();

    [MenuItem("BalloonBarrage/Art/Create concept variants V12 scene")]
    public static void Build()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (File.Exists(Destination) || Directory.Exists(Root)) throw new InvalidOperationException("V12 already exists; preserve manual edits.");
        foreach (var folder in new[] { "Materials", "Meshes", "Textures", "Prefabs", "Previews", "Reflections", "Shaders" }) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh();
        var baseline = EditorSceneManager.OpenScene(StudioMaterialAppearancePass.Destination);
        CaptureLayout(baseline, "Before_Room", false);
        if (!AssetDatabase.CopyAsset(StudioMaterialAppearancePass.Destination, Destination)) throw new IOException("Scene copy failed.");
        var scene = EditorSceneManager.OpenScene(Destination);
        var collisionBefore = CollisionAudit(scene); var learnerBefore = LearnerAudit(scene);
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        string cameraBefore = EditorJsonUtility.ToJson(camera) + EditorJsonUtility.ToJson(camera.transform);
        var lightBefore = All<Light>(scene).Select(l => EditorJsonUtility.ToJson(l) + EditorJsonUtility.ToJson(l.transform)).ToArray();
        var spawner = new SerializedObject(All<BalloonSpawner>(scene).Single());
        var originalBalloon = spawner.FindProperty("balloonPrefab").objectReferenceValue;
        BuildBalloon(spawner);
        BuildGun(scene);
        BuildWallPaint(scene);
        FinishJoinery(scene);
        // Only the copied visual prefab reference is allowed to differ in learner serialization.
        var newBalloon = spawner.FindProperty("balloonPrefab").objectReferenceValue;
        spawner.FindProperty("balloonPrefab").objectReferenceValue = originalBalloon; spawner.ApplyModifiedPropertiesWithoutUndo();
        if (!learnerBefore.SequenceEqual(LearnerAudit(scene))) throw new InvalidOperationException("Learner settings changed.");
        spawner.FindProperty("balloonPrefab").objectReferenceValue = newBalloon; spawner.ApplyModifiedPropertiesWithoutUndo();
        if (!collisionBefore.SequenceEqual(CollisionAudit(scene))) throw new InvalidOperationException("Collision changed.");
        if (cameraBefore != EditorJsonUtility.ToJson(camera) + EditorJsonUtility.ToJson(camera.transform) || !lightBefore.SequenceEqual(All<Light>(scene).Select(l => EditorJsonUtility.ToJson(l) + EditorJsonUtility.ToJson(l.transform)))) throw new InvalidOperationException("Camera/light settings changed.");
        var settings = Object.Instantiate(Lightmapping.lightingSettings); settings.name = "ConceptVariantsLighting";
        AssetDatabase.CreateAsset(settings, Root + "/ConceptVariantsLighting.lighting"); Lightmapping.lightingSettings = settings;
        Lightmapping.lightingDataAsset = null; LightmapSettings.lightmaps = Array.Empty<LightmapData>();
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        if (!Lightmapping.Bake()) throw new InvalidOperationException("V12 bake failed.");
        var gun = All<PlayerShooter>(scene).Single().gameObject;
        var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection");
        string reflection = Root + "/Reflections/ConceptStudio.exr";
        bool active = gun.activeSelf;
        try { gun.SetActive(false); if (!Lightmapping.BakeReflectionProbe(probe, reflection)) throw new IOException("Reflection bake failed."); }
        finally { gun.SetActive(active); }
        AssetDatabase.ImportAsset(reflection, ImportAssetOptions.ForceSynchronousImport); probe.customBakedTexture = Load<Cubemap>(reflection); Record(probe);
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        RegisterScene();
        VerifyAndCapture();
        Debug.Log("CONCEPT_VARIANTS_BUILD_OK: 6 balloon pigments, 3 silhouettes, 6 gun/paint themes, raised wall splats, remapped wood; learner camera/lights/physics/settings preserved.");
    }

    static void BuildBalloon(SerializedObject spawner)
    {
        var source = Load<GameObject>(StudioMaterialAppearancePass.Root + "/Prefabs/PlayableBalloonAppearance.prefab");
        string path = Root + "/Prefabs/PlayableBalloonVariants.prefab";
        if (!AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(source), path)) throw new IOException("Balloon copy failed.");
        var root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            var protectedBefore = root.GetComponentsInChildren<Component>(true).Where(c => c is Collider || c is Rigidbody || c is MonoBehaviour).Select(EditorJsonUtility.ToJson).ToArray();
            var filter = root.GetComponentsInChildren<MeshFilter>(true).Single();
            var sourceMesh = filter.sharedMesh;
            var forms = new List<Mesh> { sourceMesh };
            foreach (string shape in new[] { "Round", "Slender" })
            {
                var mesh = Object.Instantiate(sourceMesh); mesh.name = "Latex_" + shape;
                var positions = mesh.vertices;
                // Existing first 49 x 97 vertices are the smooth body; retain the authored knot verbatim.
                const int rings = 48, sides = 96;
                for (int y = 0; y <= rings; y++) for (int x = 0; x <= sides; x++)
                {
                    int i = y * (sides + 1) + x; var p = positions[i];
                    float t = y * Mathf.PI / rings;
                    if (shape == "Round")
                    {
                        // A compact round body with its lower neck meeting the unchanged knot.
                        float oldY = .1755f * Mathf.Cos(t) + .006f;
                        p.y = oldY - .035f * (1 + Mathf.Cos(t));
                        float factor = (1 - .12f * Mathf.Cos(t)) / (1 + .08f * Mathf.Cos(t));
                        p.x *= factor; p.z *= factor;
                    }
                    else { p.x *= .84f; p.z *= .84f; }
                    positions[i] = p;
                }
                mesh.vertices = positions;
                // Compute body normals on the parametric grid; weld the UV seam explicitly.
                var normals = mesh.normals;
                for (int y = 0; y <= rings; y++) for (int x = 0; x <= sides; x++)
                {
                    int i = y * (sides + 1) + x;
                    int left = y * (sides + 1) + (x == 0 || x == sides ? sides - 1 : x - 1);
                    int right = y * (sides + 1) + (x == sides ? 1 : x + 1);
                    var tangent = positions[right] - positions[left];
                    var down = positions[Mathf.Min(y + 1, rings) * (sides + 1) + x] - positions[Mathf.Max(y - 1, 0) * (sides + 1) + x];
                    normals[i] = y == 0 ? Vector3.up : y == rings ? Vector3.down : Vector3.Cross(tangent, down).normalized;
                }
                mesh.normals = normals; mesh.RecalculateTangents(); mesh.RecalculateBounds();
                AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + mesh.name + ".asset"); forms.Add(mesh);
            }
            var pigments = Names.Select((name, i) => Material("Latex_" + name, Hex(Colors[i]), .48f)).ToArray();
            var appearance = root.AddComponent<StudioBalloonAppearance>();
            appearance.body = filter; appearance.latex = filter.GetComponent<Renderer>(); appearance.silhouettes = forms.ToArray(); appearance.pigments = pigments;
            var protectedAfter = root.GetComponentsInChildren<Component>(true).Where(c => c is Collider || c is Rigidbody || (c is MonoBehaviour && !(c is StudioBalloonAppearance))).Select(EditorJsonUtility.ToJson).ToArray();
            if (!protectedBefore.SequenceEqual(protectedAfter)) throw new InvalidOperationException("Balloon physics/learner data changed.");
            PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
        spawner.FindProperty("balloonPrefab").objectReferenceValue = Load<GameObject>(path); spawner.ApplyModifiedPropertiesWithoutUndo();
    }

    static void BuildGun(Scene scene)
    {
        var shooter = All<PlayerShooter>(scene).Single();
        var gun = shooter.gameObject.AddComponent<StudioGunAppearance>();
        gun.muzzleSpray = (ParticleSystem)new SerializedObject(shooter).FindProperty("muzzleSpray").objectReferenceValue;
        var cream = Hex("F5EBD9");
        gun.themes = Names.Select((name, i) => new StudioGunAppearance.Theme
        {
            name = name + " paint", body = Color.Lerp(cream, Hex(Colors[i]), .09f), paint = Hex(Colors[i]),
            muzzle = Hex(Colors[(i + 1) % Colors.Length]), hardware = Hex("F4C75B")
        }).ToArray();
        gun.regions = StudioConceptGunBuilder.Build(shooter); Record(gun);
    }

    static void BuildWallPaint(Scene scene)
    {
        foreach (var sprite in All<SpriteRenderer>(scene).Where(r => r.name.StartsWith("Graffiti_", StringComparison.Ordinal))) { sprite.gameObject.SetActive(false); Record(sprite.gameObject); }
        var parent = new GameObject("ConceptWallPaint"); SceneManager.MoveGameObjectToScene(parent, scene);
        var sprites = AssetDatabase.LoadAllAssetsAtPath("Assets/StudioArtPack/Textures/GraffitiSpray_Atlas_v3.png").OfType<Sprite>().ToArray();
        CreateRelief(parent.transform, sprites.Single(s => s.name == "Teal_Spray"), "Teal_Splat", Hex(Colors[0]), new Vector3(-2.25f, .15f, 9.855f), 1.55f);
        CreateRelief(parent.transform, sprites.Single(s => s.name == "Coral_Burst"), "Coral_Splat", Hex(Colors[1]), new Vector3(2.35f, 1.0f, 9.845f), 1.35f);
        CreateRelief(parent.transform, sprites.Single(s => s.name == "Yellow_Drip"), "Gold_Splat", Hex(Colors[2]), new Vector3(.2f, -1.25f, 9.835f), 1.05f);
        // Spread restrained pigment colors through the existing peripheral paint/buckets.
        var paints = Names.Select((name, i) => Material("StudioPaint_" + name, Hex(Colors[i]), .44f)).ToArray();
        int index = 0;
        foreach (var r in All<MeshRenderer>(scene).Where(r => new[] { "FloorPaint", "PaintDot", "PaintDrip", "LidPaint", "PaintSwatch" }.Contains(r.name)).OrderBy(r => r.transform.position.x).ThenBy(r => r.transform.position.z))
        { r.sharedMaterial = paints[index++ % paints.Length]; Record(r); }
    }

    static void CreateRelief(Transform parent, Sprite sprite, string name, Color color, Vector3 position, float scale)
    {
        var source = sprite.texture; var rt = RenderTexture.GetTemporary(source.width, source.height, 0); var old = RenderTexture.active;
        var sourcePixels = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        try { Graphics.Blit(source, rt); RenderTexture.active = rt; sourcePixels.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0); sourcePixels.Apply(); }
        finally { RenderTexture.active = old; RenderTexture.ReleaseTemporary(rt); }
        var rect = sprite.rect; int width = (int)rect.width, height = (int)rect.height;
        var pixels = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
        {
            var p = sourcePixels.GetPixel((int)rect.x + x, (int)rect.y + y);
            // Preserve the original stroke texture and alpha, recolor the existing art without red/teal bias.
            float value = .80f + .20f * Mathf.Max(p.r, Mathf.Max(p.g, p.b));
            pixels.SetPixel(x, y, new Color(value, value, value, p.a));
        }
        pixels.Apply(); Object.DestroyImmediate(sourcePixels);
        string texturePath = Root + "/Textures/" + name + "_SourceAlpha.png";
        File.WriteAllBytes(texturePath, pixels.EncodeToPNG()); AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceSynchronousImport);
        var importer = (TextureImporter)AssetImporter.GetAtPath(texturePath); importer.alphaIsTransparency = true; importer.wrapMode = TextureWrapMode.Clamp; importer.mipmapEnabled = true; importer.SaveAndReimport();
        const int n = 128;
        var verts = new List<Vector3>(); var uvs = new List<Vector2>(); var top = new List<int>(); var sides = new List<int>(); var alpha = new float[(n + 1) * (n + 1)];
        float size = width / sprite.pixelsPerUnit;
        for (int y = 0; y <= n; y++) for (int x = 0; x <= n; x++)
        {
            var uv = new Vector2(x / (float)n, y / (float)n); float a = pixels.GetPixelBilinear(uv.x, uv.y).a;
            alpha[y * (n + 1) + x] = a;
            verts.Add(new Vector3((uv.x - .5f) * size, (uv.y - .5f) * size, -(.003f + .005f * a) / scale)); uvs.Add(uv);
        }
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            int a = y * (n + 1) + x, b = a + 1, c = a + n + 1, d = c + 1;
            if (Mathf.Max(alpha[a], alpha[b], alpha[c], alpha[d]) < .25f) continue;
            top.AddRange(new[] { a, c, b, b, c, d });
            var crosses = new List<Vector3>(); var crossUvs = new List<Vector2>();
            void Cross(int first, int second)
            {
                if ((alpha[first] >= .25f) == (alpha[second] >= .25f)) return;
                float t = (.25f - alpha[first]) / (alpha[second] - alpha[first]);
                crosses.Add(Vector3.Lerp(verts[first], verts[second], t)); crossUvs.Add(Vector2.Lerp(uvs[first], uvs[second], t));
            }
            Cross(a, b); Cross(b, d); Cross(d, c); Cross(c, a);
            for (int i = 0; i + 1 < crosses.Count; i += 2)
            {
                var va = crosses[i]; var vb = crosses[i + 1];
                var gradient = new Vector3(alpha[b] + alpha[d] - alpha[a] - alpha[c], alpha[c] + alpha[d] - alpha[a] - alpha[b], 0);
                if (Vector3.Dot(Vector3.Cross(vb - va, new Vector3(0, 0, -va.z)), -gradient) < 0) { var swap = va; va = vb; vb = swap; }
                int start = verts.Count;
                verts.AddRange(new[] { va, vb, new Vector3(va.x, va.y, 0), new Vector3(vb.x, vb.y, 0) });
                uvs.AddRange(new[] { crossUvs[i], crossUvs[i + 1], crossUvs[i], crossUvs[i + 1] });
                sides.AddRange(new[] { start, start + 1, start + 2, start + 1, start + 3, start + 2 });
            }
        }
        Object.DestroyImmediate(pixels);
        var mesh = new Mesh { name = name, indexFormat = IndexFormat.UInt32 }; mesh.SetVertices(verts); mesh.SetUVs(0, uvs); mesh.subMeshCount = 2; mesh.SetTriangles(top, 0); mesh.SetTriangles(sides, 1); mesh.RecalculateNormals(); mesh.RecalculateTangents(); mesh.RecalculateBounds();
        if (!Unwrapping.GenerateSecondaryUVSet(mesh)) throw new InvalidOperationException("Paint UV generation failed.");
        AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + name + ".asset");
        var material = Material(name + "_Paint", color, .48f); material.SetTexture("_BaseMap", Load<Texture2D>(texturePath));
        material.SetFloat("_AlphaClip", 1); material.SetFloat("_Cutoff", .25f); material.EnableKeyword("_ALPHATEST_ON"); material.SetOverrideTag("RenderType", "TransparentCutout"); material.renderQueue = (int)RenderQueue.AlphaTest;
        var edge = Material(name + "_Rim", color, .45f);
        var go = new GameObject(name); go.transform.SetParent(parent, false); go.transform.position = position; go.transform.localScale = Vector3.one * scale;
        go.AddComponent<MeshFilter>().sharedMesh = mesh; go.AddComponent<MeshRenderer>().sharedMaterials = new[] { material, edge };
        GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic);
    }

    static void FinishJoinery(Scene scene)
    {
        var source = Load<Material>("Assets/StudioArtPack/CoverFinishV9/Materials/NaturalJoinery.mat");
        var material = new Material(source) { name = "QuietOakJoinery" }; material.SetFloat("_BumpScale", .055f); material.SetFloat("_Smoothness", .20f);
        string materialPath = Root + "/Materials/QuietOakJoinery.mat";
        var savedMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (savedMaterial == null) AssetDatabase.CreateAsset(material, materialPath);
        else { EditorUtility.CopySerialized(material, savedMaterial); Object.DestroyImmediate(material); material = savedMaterial; }
        int index = 0;
        foreach (var r in All<MeshRenderer>(scene).Where(r => new[] { "TopBeam", "RoofBeam", "SkylightBeam", "SkylightCrossbar", "SkylightRim" }.Contains(r.name)))
        {
            var f = r.GetComponent<MeshFilter>(); var bounds = f.sharedMesh.bounds; var s = r.transform.lossyScale;
            var size = Vector3.Scale(bounds.size, new Vector3(Mathf.Abs(s.x), Mathf.Abs(s.y), Mathf.Abs(s.z)));
            var mesh = StudioConceptGeometry.RoundedBox(size, .018f); mesh.name = "JoineryUV_" + index++;
            var localVertices = mesh.vertices; var localNormals = mesh.normals;
            for (int i = 0; i < localVertices.Length; i++)
            { localVertices[i] = bounds.center + new Vector3(localVertices[i].x / s.x, localVertices[i].y / s.y, localVertices[i].z / s.z); localNormals[i] = Vector3.Scale(localNormals[i], s).normalized; }
            // Offset only the copied front-beam visual; retain its existing collision transform/data.
            if (r.name == "TopBeam") for (int i = 0; i < localVertices.Length; i++) localVertices[i].z -= .22f / s.z;
            mesh.vertices = localVertices; mesh.normals = localNormals; mesh.RecalculateBounds();
            var vertices = mesh.vertices; var normals = mesh.normals; var uv = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                var p = Vector3.Scale(vertices[i], r.transform.lossyScale); var n = normals[i];
                uv[i] = Mathf.Abs(n.y) > .5f ? new Vector2(p.z / 3f, p.x / 1.6f) : Mathf.Abs(n.x) > .5f ? new Vector2(p.z / 3f, p.y / 1.6f) : new Vector2(p.x / 3f, p.y / 1.6f);
            }
            mesh.uv = uv; mesh.RecalculateTangents();
            if (!Unwrapping.GenerateSecondaryUVSet(mesh)) throw new InvalidOperationException("Joinery UV generation failed.");
            string meshPath = Root + "/Meshes/" + mesh.name + ".asset";
            var savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
            if (savedMesh == null) AssetDatabase.CreateAsset(mesh, meshPath);
            else { EditorUtility.CopySerialized(mesh, savedMesh); Object.DestroyImmediate(mesh); mesh = savedMesh; }
            f.sharedMesh = mesh; r.sharedMaterial = material; Record(f); Record(r);
        }
    }

    public static void RegisterScene()
    {
        if (!EditorBuildSettings.scenes.Any(s => s.path == Destination)) EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(new[] { new EditorBuildSettingsScene(Destination, true) }).ToArray();
    }

    public static void AuditReferences()
    {
        var paths = Directory.GetFiles(Root, "*", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles("Assets/Scenes/StudioConceptVariants", "*", SearchOption.AllDirectories)).Concat(new[] { Destination });
        int references = 0;
        foreach (var path in paths.Where(p => new[] { ".asset", ".mat", ".prefab", ".unity" }.Contains(Path.GetExtension(p))))
        {
            string content = File.ReadAllText(path); if (!content.StartsWith("%YAML")) continue;
            foreach (Match match in Regex.Matches(content, @"guid: ([0-9a-f]{32})"))
            {
                string guid = match.Groups[1].Value; if (guid.StartsWith("0000000000000000")) continue;
                if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid))) throw new InvalidOperationException("V12 unresolved reference: " + path + " | " + guid);
                references++;
            }
        }
        File.AppendAllText(Root + "/Verification.txt", "V12 direct YAML asset references: " + references + " checked, zero unresolved.\n");
        Debug.Log("CONCEPT_VARIANTS_REFERENCES_OK: " + references);
    }

    public static void VerifyAndCapture()
    {
        var scene = EditorSceneManager.OpenScene(Destination);
        if (All<Transform>(scene).Any(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject) != 0)) throw new InvalidOperationException("Missing script.");
        if (LightmapSettings.lightmaps.Length == 0) throw new InvalidOperationException("Saved lightmaps missing.");
        typeof(StudioCameraLightingPass).GetMethod("ValidateAim", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, new object[] { scene });
        var appearance = All<StudioGunAppearance>(scene).Single();
        if (appearance.themes.Length != 6 || appearance.regions.Length < 4) throw new InvalidOperationException("Gun themes/regions missing.");
        CaptureLayout(scene, "After_Room", true);
        CaptureBalloonGallery(scene);
        CaptureGunGallery(scene);
        File.WriteAllText(Root + "/Verification.txt", "Unity " + Application.unityVersion + "; isolated project.\nSaved scene reloaded; lightmaps present; no missing scripts; nine camera/muzzle paths and complete motion framing passed.\nSix pigments / three silhouettes; six gun themes. Fixed visual layouts are real 1600x900 Unity game-camera renders, not gameplay captures.\nCreation compares learner settings (one visual prefab-reference exception), scene colliders and camera/light settings to V11. Human input, complete rounds, performance and a playable release remain unverified.\n");
        AssetDatabase.Refresh(); Debug.Log("CONCEPT_VARIANTS_VERIFY_OK");
    }

    static void CaptureLayout(Scene scene, string label, bool variants)
    {
        var spawner = new SerializedObject(All<BalloonSpawner>(scene).Single()); var prefab = (GameObject)spawner.FindProperty("balloonPrefab").objectReferenceValue;
        var targets = new List<GameObject>(); var gun = All<StudioGunAppearance>(scene).FirstOrDefault();
        try
        {
            if (gun != null) gun.ApplyTheme(0);
            var positions = new[] { new Vector3(-2, -.5f, 8), new Vector3(0, .9f, 8), new Vector3(2, -.5f, 8) };
            for (int i = 0; i < positions.Length; i++)
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene); targets.Add(target); target.transform.position = positions[i];
                if (variants) target.GetComponent<StudioBalloonAppearance>().ApplyVariant(new[] { 0, 1, 2 }[i], i);
            }
            Capture(scene, label);
        }
        finally { foreach (var target in targets) Object.DestroyImmediate(target); ClearGunBlocks(gun); }
    }

    static void CaptureBalloonGallery(Scene scene)
    {
        var prefab = Load<GameObject>(Root + "/Prefabs/PlayableBalloonVariants.prefab"); var targets = new List<GameObject>();
        var gun = All<PlayerShooter>(scene).Single().gameObject; bool active = gun.activeSelf;
        try
        {
            gun.SetActive(false);
            for (int i = 0; i < 6; i++)
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene); targets.Add(target);
                target.transform.position = new Vector3(-2.8f + i % 3 * 2.8f, i < 3 ? 1.2f : -.9f, 8);
                target.GetComponent<StudioBalloonAppearance>().ApplyVariant(i, i % 3);
            }
            Capture(scene, "Balloon_Gallery");
        }
        finally { foreach (var target in targets) Object.DestroyImmediate(target); gun.SetActive(active); }
    }

    static void CaptureGunGallery(Scene scene)
    {
        var gun = All<StudioGunAppearance>(scene).Single(); var transform = gun.transform; var pos = transform.localPosition; var rot = transform.localRotation; var scale = transform.localScale;
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        try
        {
            transform.position = camera.transform.TransformPoint(new Vector3(0, -.03f, 1.8f)); transform.rotation = camera.transform.rotation * Quaternion.Euler(0, 120, 0); transform.localScale = scale * 3.4f;
            for (int i = 0; i < gun.themes.Length; i++) { gun.ApplyTheme(i); Capture(scene, "Gun_" + Names[i]); }
        }
        finally { transform.localPosition = pos; transform.localRotation = rot; transform.localScale = scale; ClearGunBlocks(gun); }
    }

    static void ClearGunBlocks(StudioGunAppearance gun)
    {
        if (gun == null) return;
        foreach (var region in gun.regions) region.renderer.SetPropertyBlock(null, region.materialIndex);
        // Particle overrides are temporary in the capture scene; never save after gallery capture.
    }

    public static void Capture(Scene scene, string label)
    {
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera")); var oldTarget = camera.targetTexture; var oldActive = RenderTexture.active; float aspect = camera.aspect; bool async = ShaderUtil.allowAsyncCompilation;
        var rt = new RenderTexture(1600, 900, 24, RenderTextureFormat.ARGB32); var pixels = new Texture2D(1600, 900, TextureFormat.RGB24, false);
        try
        {
            ShaderUtil.allowAsyncCompilation = false; rt.Create(); camera.targetTexture = rt; camera.aspect = 16f / 9f; camera.Render();
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = rt }; if (RenderPipeline.SupportsRenderRequest(camera, request)) RenderPipeline.SubmitRenderRequest(camera, request); else camera.Render();
            RenderTexture.active = rt; pixels.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0); pixels.Apply(); File.WriteAllBytes(Root + "/Previews/" + label + ".png", pixels.EncodeToPNG());
        }
        finally { camera.targetTexture = oldTarget; camera.aspect = aspect; RenderTexture.active = oldActive; ShaderUtil.allowAsyncCompilation = async; rt.Release(); Object.DestroyImmediate(rt); Object.DestroyImmediate(pixels); }
    }
}
