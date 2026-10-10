using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Additive, editor-only prop pass. Never rewrites existing scene objects or gameplay assets.
public static class StudioDetailFinisher
{
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    const string Root = "Assets/StudioArtPack/DetailsV5";
    const string GroupName = "StudioDetailsV5";
    static Material oak, ink, metal, teal, coral, gold, paper, paperEdge, linen, cork, ceramic;
    static Mesh ring, cup, paperTube, curtain, palette, holes;

    [MenuItem("Tools/Balloon Studio/Add Studio Details V5")]
    public static void Menu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().path != ScenePath)
        { Debug.LogWarning("Stop Play Mode and open PrototypeScene first."); return; }
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) Apply(SceneManager.GetActiveScene());
    }

    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use the editor menu outside batch mode.");
        Directory.CreateDirectory(".local-backups/ExhibitionStudioPreview");
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ExhibitionStudioBuilder.Capture(scene, "details-before.png");
        Apply(scene);
        ExhibitionStudioBuilder.Capture(scene, "details-after.png");
        ExhibitionStudioBuilder.CaptureTargetLayout(scene);
        ExhibitionStudioBuilder.ValidateAim(scene);
        Debug.Log("STUDIO_DETAILS_OK: additive details, clear targets, materials and shooting references checked.");
    }

    static void Apply(Scene scene)
    {
        if (scene.GetRootGameObjects().Any(g => g.name == GroupName))
            throw new InvalidOperationException("Details V5 already exists; edit the group directly to preserve manual changes.");
        if (!scene.GetRootGameObjects().Any(g => g.name == "StudioReferenceV4"))
            throw new InvalidOperationException("This detail pass requires the existing V4 studio.");
        Directory.CreateDirectory(".local-backups");
        File.Copy(ScenePath, ".local-backups/scene-before-details-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".unity", false);
        foreach (string folder in new[] { "Materials", "Meshes", "Prefabs" }) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh();
        oak = Existing("Assets/StudioArtPack/ReferenceV4/Materials/FineOak.mat");
        ink = Existing("Assets/StudioArtPack/AtmosphereV3/Materials/Graphite.mat");
        metal = Existing("Assets/StudioArtPack/AtmosphereV3/Materials/BrushedMetal.mat");
        teal = Existing("Assets/StudioArtPack/AtmosphereV3/Materials/TealPaint.mat");
        coral = Existing("Assets/StudioArtPack/AtmosphereV3/Materials/CoralPaint.mat");
        gold = Existing("Assets/StudioArtPack/AtmosphereV3/Materials/GoldPaint.mat");
        paper = Lit("DrawingPaper", "EEE5D1", .12f);
        paperEdge = Lit("PaperEdges", "CFC5AF", .08f);
        linen = Lit("OatLinen", "DFD0B6", .09f);
        cork = Lit("WarmCork", "997354", .08f);
        ceramic = Lit("IvoryCeramic", "E4D8BD", .42f);
        ring = SaveMesh(Torus(), "SmallRing");
        cup = SaveMesh(Cup(), "HollowCup");
        paperTube = SaveMesh(Lathe(new[] { new Vector2(.50f, -.5f), new Vector2(.50f, .5f), new Vector2(.38f, .5f), new Vector2(.38f, -.5f), new Vector2(.50f, -.5f) }), "PaperTube");
        curtain = SaveMesh(Curtain(), "GatheredCurtain");
        palette = SaveMesh(Palette(), "ThumbholePalette");
        holes = SaveMesh(PegHoles(), "PegboardHoles");

        var root = new GameObject(GroupName).transform;
        SceneManager.MoveGameObjectToScene(root.gameObject, scene);
        var window = Group(root, "WindowDetails", new Vector3(-6.77f, .05f, 4.8f), Quaternion.Euler(0, -90, 0));
        Window(window);
        SavePrefab(window);
        var desk = Group(root, "WorkbenchObjects", new Vector3(-5.25f, -2.29f, 6));
        Workbench(desk);
        SavePrefab(desk);
        var shelf = Group(root, "WorkbenchPaperShelf", new Vector3(-5.25f, -3.47f, 6));
        Book(shelf, new Vector3(-.48f, .06f, -.06f), new Vector3(.85f, .09f, .55f), teal);
        Book(shelf, new Vector3(-.44f, .16f, -.03f), new Vector3(.76f, .09f, .50f), paper);
        for (int i = 0; i < 2; i++)
            Roll(shelf, new Vector3(.30f + i * .23f, .12f, 0), new Vector3(.17f, .17f, .79f));
        SavePrefab(shelf);
        var board = Group(root, "PainterPegboard", new Vector3(6.72f, .25f, 4.65f), Quaternion.Euler(0, 90, 0));
        Pegboard(board);
        SavePrefab(board);
        var clock = Group(root, "StudioWallClock", new Vector3(6.72f, 2.5f, 5.95f), Quaternion.Euler(0, 90, 0));
        Clock(clock);
        SavePrefab(clock);
        var cart = Group(root, "CanvasSupplyCart", new Vector3(4.15f, -4, 5.2f), Quaternion.Euler(0, -8, 0));
        Cart(cart);
        SavePrefab(cart);
        var stool = Group(root, "PainterStool", new Vector3(-3.95f, -4, 5.15f), Quaternion.Euler(0, 15, 0));
        Stool(stool);
        SavePrefab(stool);
        var niche = Group(root, "NicheBookShelf", new Vector3(-5.1f, 1.1f, 9.20f));
        Box(niche, "OakShelf", Vector3.zero, new Vector3(1.45f, .09f, .7f), oak);
        for (int i = 0; i < 4; i++)
        {
            var book = Group(niche, "Sketchbook", new Vector3(-.48f + i * .19f, .27f + i * .025f, .02f));
            var size = new Vector3(.14f, .43f + i * .05f, .34f);
            var cover = new[] { teal, paper, coral, linen }[i];
            Box(book, "PageBlock", new Vector3(0, 0, .015f), new Vector3(size.x - .025f, size.y - .025f, size.z - .03f), paperEdge);
            foreach (float x in new[] { -size.x / 2, size.x / 2 })
                Box(book, "BookCover", new Vector3(x, 0, 0), new Vector3(.012f, size.y, size.z), cover);
            Box(book, "BookSpine", new Vector3(0, 0, -size.z / 2), new Vector3(size.x, size.y, .02f), cover);
            Box(book, "SpineBand", new Vector3(0, .11f, -size.z / 2 - .012f), new Vector3(.085f, .022f, .004f), paper);
        }
        Cylinder(niche, "SmallCeramicVase", new Vector3(.43f, .19f, .02f), new Vector3(.22f, .31f, .22f), ceramic);
        SavePrefab(niche);
        // Narrow skirting supplies a finished wall edge without adding a broad colored panel.
        foreach (float x in new[] { -6.77f, 6.77f })
            Box(root, "OakWallSkirting", new Vector3(x, -3.84f, 4.6f), new Vector3(.12f, .24f, 10), oak);

        ValidateDetails(root);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Could not save the detailed scene.");
    }

    static void Window(Transform parent)
    {
        foreach (float side in new[] { -1f, 1f })
        {
            MeshPart(parent, "GatheredLinen", curtain, new Vector3(side * 1.40f, 0, -.10f), Vector3.one, linen);
            for (int i = 0; i < 3; i++)
                Ring(parent, "CurtainRing", new Vector3(side * 1.40f + (i - 1) * .10f, 2.16f, -.10f), new Vector3(.08f, .08f, .08f), metal, Quaternion.Euler(90, 0, 0));
            Box(parent, "CurtainTie", new Vector3(side * 1.40f, -.28f, -.18f), new Vector3(.49f, .09f, .055f), linen);
            foreach (float y in new[] { -1.43f, 1.43f })
                Box(parent, "WindowHinge", new Vector3(side * 1.62f, y, -.13f), new Vector3(.09f, .19f, .045f), metal);
        }
        Bar(parent, "CurtainRod", new Vector3(-1.76f, 2.18f, -.10f), new Vector3(1.76f, 2.18f, -.10f), .045f, metal);
        Box(parent, "WindowLatch", new Vector3(.08f, -.10f, -.19f), new Vector3(.07f, .22f, .05f), metal);
    }

    static void Workbench(Transform parent)
    {
        var pal = Group(parent, "UsedPalette", new Vector3(-.58f, .045f, -.32f), Quaternion.Euler(0, -15, 0));
        MeshPart(pal, "PaletteWithThumbhole", palette, Vector3.zero, Vector3.one, oak);
        for (int i = 0; i < 4; i++)
            Sphere(pal, "PaintDab", new Vector3(-.23f + (i % 2) * .22f, .035f, -.12f + (i / 2) * .22f), new Vector3(.14f, .026f, .11f), new[] { teal, coral, gold, paper }[i]);
        Brush(parent, new Vector3(.01f, .05f, -.33f), Quaternion.Euler(0, 32, 82), .53f, teal);
        var mug = Group(parent, "PainterMug", new Vector3(.70f, 0, -.35f), Quaternion.Euler(0, 25, 0));
        MeshPart(mug, "HollowCeramicMug", cup, new Vector3(0, .145f, 0), new Vector3(.29f, .29f, .29f), ceramic);
        Cylinder(mug, "Coffee", new Vector3(0, .22f, 0), new Vector3(.23f, .007f, .23f), ink);
        Ring(mug, "MugHandle", new Vector3(.17f, .15f, 0), new Vector3(.20f, .20f, .20f), ceramic, Quaternion.Euler(90, 0, 0));
        Cylinder(mug, "CorkCoaster", new Vector3(0, .012f, 0), new Vector3(.37f, .02f, .37f), cork);
        for (int i = 0; i < 2; i++)
        {
            var tube = Group(parent, "PaintTube", new Vector3(.08f + i * .23f, .055f, -.18f), Quaternion.Euler(90, 0, 12 + i * 24));
            Cylinder(tube, "TubeBody", Vector3.zero, new Vector3(.11f, .24f, .11f), ceramic);
            Cylinder(tube, "PigmentBand", Vector3.zero, new Vector3(.113f, .075f, .113f), i == 0 ? coral : teal);
            Cylinder(tube, "TubeCap", new Vector3(0, .15f, 0), new Vector3(.068f, .06f, .068f), ink);
        }
    }

    static void Pegboard(Transform parent)
    {
        Box(parent, "CorkBoard", Vector3.zero, new Vector3(2.4f, 2.35f, .08f), cork);
        MeshPart(parent, "Perforations", holes, new Vector3(0, 0, -.043f), Vector3.one, ink, false);
        foreach (float x in new[] { -1.24f, 1.24f }) Box(parent, "SideRail", new Vector3(x, 0, -.02f), new Vector3(.10f, 2.55f, .12f), oak);
        foreach (float y in new[] { -1.23f, 1.23f }) Box(parent, "EndRail", new Vector3(0, y, -.02f), new Vector3(2.5f, .10f, .12f), oak);
        for (int i = 0; i < 3; i++)
        {
            float x = -.82f + i * .35f;
            Bar(parent, "BrushHook", new Vector3(x, .72f, -.055f), new Vector3(x, .72f, -.17f), .028f, metal);
            Brush(parent, new Vector3(x, .30f, -.18f), Quaternion.Euler(0, 0, 180), .73f, new[] { teal, coral, gold }[i]);
        }
        var sheet = Group(parent, "PinnedColorStudy", new Vector3(.64f, .40f, -.07f), Quaternion.Euler(0, 0, -5));
        Box(sheet, "StudyPaper", Vector3.zero, new Vector3(.60f, .77f, .013f), paper);
        for (int i = 0; i < 3; i++)
            Sphere(sheet, "PigmentSample", new Vector3((i - 1) * .17f, .03f, -.014f), new Vector3(.13f, .24f, .009f), new[] { teal, coral, gold }[i]);
        Cylinder(sheet, "Pin", new Vector3(0, .32f, -.032f), new Vector3(.045f, .028f, .045f), metal).localRotation = Quaternion.Euler(90, 0, 0);
        Box(parent, "ToolTray", new Vector3(0, -.94f, -.22f), new Vector3(2.1f, .10f, .40f), oak);
        // A roller hangs below the tray; its metal frame is made from three thin segments.
        Bar(parent, "RollerHandle", new Vector3(.54f, -.33f, -.18f), new Vector3(.54f, -.72f, -.18f), .09f, teal);
        Bar(parent, "RollerStem", new Vector3(.54f, -.32f, -.18f), new Vector3(.54f, -.05f, -.18f), .025f, metal);
        Bar(parent, "RollerArm", new Vector3(.54f, -.05f, -.18f), new Vector3(.95f, -.05f, -.18f), .025f, metal);
        var roller = Cylinder(parent, "UsedPaintRoller", new Vector3(.94f, .15f, -.18f), new Vector3(.17f, .42f, .17f), linen);
        roller.localRotation = Quaternion.identity;
    }

    static void Clock(Transform parent)
    {
        Cylinder(parent, "ClockRim", Vector3.zero, new Vector3(.72f, .10f, .72f), oak).localRotation = Quaternion.Euler(90, 0, 0);
        Cylinder(parent, "ClockFace", new Vector3(0, 0, -.062f), new Vector3(.62f, .012f, .62f), paper).localRotation = Quaternion.Euler(90, 0, 0);
        for (int i = 0; i < 12; i++)
        {
            float angle = i * Mathf.PI / 6;
            var tick = Box(parent, "HourMark", new Vector3(Mathf.Sin(angle) * .257f, Mathf.Cos(angle) * .257f, -.072f), new Vector3(.012f, .04f, .008f), ink);
            tick.localRotation = Quaternion.Euler(0, 0, -i * 30);
        }
        Bar(parent, "HourHand", new Vector3(0, 0, -.085f), new Vector3(-.11f, .08f, -.085f), .021f, ink);
        Bar(parent, "MinuteHand", new Vector3(0, 0, -.085f), new Vector3(.17f, .11f, -.085f), .013f, ink);
        Sphere(parent, "ClockPin", new Vector3(0, 0, -.085f), new Vector3(.035f, .035f, .020f), metal);
    }

    static void Cart(Transform parent)
    {
        for (int level = 0; level < 2; level++)
        {
            float y = .24f + level * .82f;
            Box(parent, "TrayFloor", new Vector3(0, y, 0), new Vector3(1.15f, .07f, .67f), oak);
            foreach (float x in new[] { -.57f, .57f }) Box(parent, "TrayRim", new Vector3(x, y + .09f, 0), new Vector3(.065f, .17f, .67f), teal);
            Box(parent, "TrayBack", new Vector3(0, y + .09f, .31f), new Vector3(1.15f, .17f, .06f), teal);
        }
        foreach (float x in new[] { -.48f, .48f }) foreach (float z in new[] { -.25f, .25f })
        {
            Bar(parent, "CartUpright", new Vector3(x, .17f, z), new Vector3(x, 1.32f, z), .045f, metal);
            var wheel = Cylinder(parent, "Caster", new Vector3(x, .10f, z), new Vector3(.17f, .06f, .17f), ink);
            wheel.localRotation = Quaternion.Euler(0, 0, 90);
        }
        Bar(parent, "PushHandle", new Vector3(-.48f, 1.32f, .25f), new Vector3(.48f, 1.32f, .25f), .065f, oak);
        Book(parent, new Vector3(-.16f, .37f, -.02f), new Vector3(.60f, .16f, .50f), paper);
        Book(parent, new Vector3(-.12f, .50f, -.02f), new Vector3(.52f, .09f, .44f), coral);
        for (int i = 0; i < 2; i++)
        {
            var canvas = Group(parent, "SpareCanvas", new Vector3(-.23f + i * .23f, 1.35f, .08f + i * .05f), Quaternion.Euler(0, -12 + i * 7, 7));
            Box(canvas, "Stretcher", Vector3.zero, new Vector3(.37f, .58f, .06f), oak);
            Box(canvas, "BlankCanvas", new Vector3(0, 0, -.037f), new Vector3(.34f, .55f, .012f), paper);
        }
        Box(parent, "FoldedCloth", new Vector3(.25f, 1.13f, -.12f), new Vector3(.39f, .07f, .28f), linen);
    }

    static void Stool(Transform parent)
    {
        Cylinder(parent, "RoundOakSeat", new Vector3(0, 1.05f, 0), new Vector3(.82f, .10f, .82f), oak);
        foreach (float x in new[] { -.24f, .24f }) foreach (float z in new[] { -.24f, .24f })
        {
            Bar(parent, "StoolLeg", new Vector3(x * 1.23f, .045f, z * 1.23f), new Vector3(x, 1.03f, z), .075f, oak);
            Sphere(parent, "FootCap", new Vector3(x * 1.23f, .05f, z * 1.23f), new Vector3(.085f, .09f, .085f), ink);
        }
        Ring(parent, "Footrest", new Vector3(0, .36f, 0), new Vector3(.72f, .72f, .72f), metal, Quaternion.identity);
    }

    static void Brush(Transform parent, Vector3 position, Quaternion rotation, float height, Material pigment)
    {
        var brush = Group(parent, "Paintbrush", position, rotation);
        Bar(brush, "WoodHandle", new Vector3(0, -height * .40f, 0), new Vector3(0, height * .23f, 0), .035f, oak);
        Box(brush, "Ferrule", new Vector3(0, height * .27f, 0), new Vector3(.075f, height * .14f, .045f), metal);
        Box(brush, "Bristles", new Vector3(0, height * .42f, 0), new Vector3(.073f, height * .18f, .044f), pigment);
    }

    static void Book(Transform parent, Vector3 position, Vector3 size, Material cover)
    {
        var book = Group(parent, "BoundSketchbook", position);
        Box(book, "PageBlock", Vector3.zero, size * .94f, paperEdge);
        foreach (float y in new[] { -size.y / 2, size.y / 2 })
            Box(book, "BookCover", new Vector3(0, y, 0), new Vector3(size.x, .014f, size.z), cover);
        Box(book, "Binding", new Vector3(-size.x / 2, 0, 0), new Vector3(.026f, size.y, size.z), cover);
    }

    static void Roll(Transform parent, Vector3 position, Vector3 size)
    {
        var roll = MeshPart(parent, "RolledDrawingPaper", paperTube, position, new Vector3(size.x, size.z, size.y), paper);
        roll.localRotation = Quaternion.Euler(90, 0, 0);
        Ring(parent, "PaperRollTie", position, new Vector3(size.x * 1.06f, size.x * 1.06f, size.x * 1.06f), linen, Quaternion.Euler(90, 0, 0));
    }

    static Transform Group(Transform parent, string name, Vector3 position, Quaternion? rotation = null)
    {
        var result = new GameObject(name).transform;
        result.SetParent(parent, false);
        result.localPosition = position;
        result.localRotation = rotation ?? Quaternion.identity;
        return result;
    }

    static Transform Box(Transform parent, string name, Vector3 position, Vector3 size, Material material) => Primitive(parent, name, PrimitiveType.Cube, position, size, material);
    static Transform Sphere(Transform parent, string name, Vector3 position, Vector3 size, Material material) => Primitive(parent, name, PrimitiveType.Sphere, position, size, material);
    // Here size.y is the full height, unlike the built-in cylinder's two-unit height.
    static Transform Cylinder(Transform parent, string name, Vector3 position, Vector3 size, Material material) => Primitive(parent, name, PrimitiveType.Cylinder, position, new Vector3(size.x, size.y / 2, size.z), material);
    static Transform Primitive(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 size, Material material)
    {
        var result = GameObject.CreatePrimitive(type);
        UnityEngine.Object.DestroyImmediate(result.GetComponent<Collider>());
        result.name = name;
        result.transform.SetParent(parent, false);
        result.transform.localPosition = position;
        result.transform.localScale = size;
        result.GetComponent<MeshRenderer>().sharedMaterial = material;
        return result.transform;
    }

    static Transform MeshPart(Transform parent, string name, Mesh mesh, Vector3 position, Vector3 size, Material material, bool shadows = true)
    {
        var result = Group(parent, name, position);
        result.localScale = size;
        result.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = result.gameObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = shadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
        return result;
    }

    static void Ring(Transform parent, string name, Vector3 position, Vector3 size, Material material, Quaternion rotation)
    { MeshPart(parent, name, ring, position, size, material).localRotation = rotation; }
    static void Bar(Transform parent, string name, Vector3 a, Vector3 b, float diameter, Material material)
    {
        var result = Cylinder(parent, name, (a + b) / 2, new Vector3(diameter, (b - a).magnitude, diameter), material);
        result.localRotation = Quaternion.FromToRotation(Vector3.up, (b - a).normalized);
    }
    static Material Existing(string path) => AssetDatabase.LoadAssetAtPath<Material>(path) ?? throw new IOException("Missing studio material: " + path);
    static Material Lit(string name, string hex, float smooth)
    {
        if (AssetDatabase.LoadAssetAtPath<Material>(Root + "/Materials/" + name + ".mat") != null)
            throw new InvalidOperationException("Preserve existing detail assets: " + name);
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? throw new InvalidOperationException("URP Lit is missing.");
        var result = new Material(shader) { name = name };
        ColorUtility.TryParseHtmlString("#" + hex, out var color);
        result.SetColor("_BaseColor", color);
        result.SetFloat("_Smoothness", smooth);
        result.SetFloat("_Metallic", 0);
        AssetDatabase.CreateAsset(result, Root + "/Materials/" + name + ".mat");
        return result;
    }
    static Mesh SaveMesh(Mesh mesh, string name)
    {
        mesh.name = name;
        mesh.RecalculateNormals(); mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + name + ".asset");
        return mesh;
    }
    static void SavePrefab(Transform group)
    {
        var position = group.localPosition; var rotation = group.localRotation;
        try
        {
            group.localPosition = Vector3.zero; group.localRotation = Quaternion.identity;
            if (PrefabUtility.SaveAsPrefabAssetAndConnect(group.gameObject, Root + "/Prefabs/" + group.name + ".prefab", InteractionMode.AutomatedAction) == null)
                throw new IOException("Prefab save failed.");
        }
        finally
        {
            group.localPosition = position; group.localRotation = rotation;
            PrefabUtility.RecordPrefabInstancePropertyModifications(group);
        }
    }

    static void ValidateDetails(Transform root)
    {
        if (root.GetComponentsInChildren<Collider>(true).Length != 0 || root.GetComponentsInChildren<Light>(true).Length != 0)
            throw new InvalidOperationException("Detail props must not affect shot queries or add realtime lights.");
        // Saved spawning ranges are x +/-3, y +/-2 at z=8; include the balloon body and string margin.
        var targetArea = new Bounds(new Vector3(0, 0, 8), new Vector3(7.2f, 5.2f, 1.8f));
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var renderer in renderers)
        {
            if (renderer.sharedMaterial == null || renderer.sharedMaterial.shader == null)
                throw new InvalidOperationException("Missing material: " + renderer.name);
            if (renderer.bounds.Intersects(targetArea)) throw new InvalidOperationException("Detail enters target area: " + renderer.name);
            var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null || mesh.vertices.Any(v => float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z)))
                throw new InvalidOperationException("Invalid mesh: " + renderer.name);
        }
        int vertices = renderers.Sum(r => r.GetComponent<MeshFilter>().sharedMesh.vertexCount);
        if (renderers.Length > 220 || vertices > 80000) throw new InvalidOperationException("Detail geometry budget exceeded.");
        Debug.Log("DETAIL_BUDGET: " + renderers.Length + " renderers, " + vertices + " vertices; no colliders or new lights.");
    }

    static Mesh Torus()
    {
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        for (int i = 0; i <= 24; i++) for (int j = 0; j <= 8; j++)
        {
            float u = i * Mathf.PI * 2 / 24, v = j * Mathf.PI * 2 / 8;
            float radius = .43f + .055f * Mathf.Cos(v);
            vertices.Add(new Vector3(radius * Mathf.Cos(u), .055f * Mathf.Sin(v), radius * Mathf.Sin(u)));
            uv.Add(new Vector2(i / 24f, j / 8f));
            if (i < 24 && j < 8) { int a = i * 9 + j; triangles.AddRange(new[] { a, a + 1, a + 9, a + 1, a + 10, a + 9 }); }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0); return mesh;
    }

    static Mesh Cup()
    {
        // Revolved closed-bottom profile with a real open mouth and inside wall.
        return Lathe(new[] { new Vector2(0, -.5f), new Vector2(.42f, -.5f), new Vector2(.50f, .48f), new Vector2(.49f, .5f), new Vector2(.41f, .5f), new Vector2(.36f, -.40f), new Vector2(0, -.40f) });
    }

    static Mesh Lathe(Vector2[] profile)
    {
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        for (int i = 0; i <= 32; i++) for (int j = 0; j < profile.Length; j++)
        {
            float angle = i * Mathf.PI * 2 / 32;
            vertices.Add(new Vector3(Mathf.Cos(angle) * profile[j].x, profile[j].y, Mathf.Sin(angle) * profile[j].x));
            uv.Add(new Vector2(i / 32f, j / (float)(profile.Length - 1)));
            if (i < 32 && j < profile.Length - 1)
            { int a = i * profile.Length + j; int next = a + profile.Length; triangles.AddRange(new[] { a, a + 1, next, a + 1, next + 1, next }); }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0); return mesh;
    }

    static Mesh Curtain()
    {
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        for (int y = 0; y <= 14; y++) for (int x = 0; x <= 24; x++)
        {
            float u = x / 24f, v = y / 14f;
            float gathered = 1 - .27f * Mathf.Exp(-Mathf.Pow((v - .44f) * 6, 2));
            vertices.Add(new Vector3((u - .5f) * .46f * gathered, (v - .5f) * 4.12f - .025f * Mathf.Sin(u * Mathf.PI * 8) * (1 - v), Mathf.Sin(u * Mathf.PI * 8) * .038f - .025f * Mathf.Sin(v * Mathf.PI)));
            uv.Add(new Vector2(u, v));
            if (y < 14 && x < 24) { int a = y * 25 + x; triangles.AddRange(new[] { a, a + 25, a + 1, a + 1, a + 25, a + 26 }); }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0); return mesh;
    }

    static Mesh Palette()
    {
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var triangles = new List<int>();
        for (int i = 0; i <= 40; i++)
        {
            float a = i * Mathf.PI * 2 / 40;
            foreach (float y in new[] { -.025f, .025f })
            {
                vertices.Add(new Vector3(Mathf.Cos(a) * .47f, y, Mathf.Sin(a) * .29f));
                vertices.Add(new Vector3(.27f + Mathf.Cos(a) * .075f, y, Mathf.Sin(a) * .075f));
                uv.Add(new Vector2(Mathf.Cos(a) * .5f + .5f, Mathf.Sin(a) * .5f + .5f));
                uv.Add(new Vector2(.77f + Mathf.Cos(a) * .08f, Mathf.Sin(a) * .08f + .5f));
            }
            if (i < 40)
            {
                int n = i * 4;
                triangles.AddRange(new[] { n, n + 4, n + 1, n + 1, n + 4, n + 5,
                    n + 2, n + 3, n + 6, n + 3, n + 7, n + 6,
                    n, n + 2, n + 4, n + 2, n + 6, n + 4,
                    n + 1, n + 5, n + 3, n + 3, n + 5, n + 7 });
            }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetUVs(0, uv); mesh.SetTriangles(triangles, 0); return mesh;
    }

    static Mesh PegHoles()
    {
        var vertices = new List<Vector3>(); var triangles = new List<int>();
        for (int y = 0; y < 9; y++) for (int x = 0; x < 9; x++)
        {
            var center = new Vector3((x - 4) * .25f, (y - 4) * .25f, 0);
            int start = vertices.Count; vertices.Add(center);
            for (int i = 0; i < 8; i++)
            { float a = i * Mathf.PI / 4; vertices.Add(center + new Vector3(Mathf.Cos(a) * .016f, Mathf.Sin(a) * .016f, 0)); }
            for (int i = 0; i < 8; i++) triangles.AddRange(new[] { start, start + (i + 1) % 8 + 1, start + i + 1 });
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); return mesh;
    }
}
