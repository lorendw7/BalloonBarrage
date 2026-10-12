using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// One-time art pass, run in an isolated project copy. No gameplay authoring.
public static class StudioRoomFinishBuilder
{
    const string Root = "Assets/StudioArtPack/RoomFinishV7";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";

    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run this one-time art pass in an isolated batch project.");
        if (File.Exists(Root + "/Materials/CreamPlaster.mat")) throw new InvalidOperationException("V7 already exists. Preserve manual material edits.");
        var scene = EditorSceneManager.OpenScene(ScenePath);
        var renderers = All<MeshRenderer>(scene);
        var gameplay = All<MonoBehaviour>(scene).Where(c => c != null && AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(c)).StartsWith("Assets/Scripts/", StringComparison.Ordinal)).ToArray();
        var beforeGameplay = gameplay.Select(EditorJsonUtility.ToJson).ToArray();
        var colliders = All<Collider>(scene);
        var beforeColliders = colliders.Select(EditorJsonUtility.ToJson).ToArray();
        foreach (string folder in new[] {"Materials", "Meshes", "Reflections"}) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh();
        var path = Root + "/Textures/OakPlanks_v7.png";
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer == null) throw new FileNotFoundException(path);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = true;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Trilinear;
        importer.mipmapEnabled = true;
        importer.anisoLevel = 8;
        importer.maxTextureSize = 2048;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();

        var floor = Material("NaturalOakFloor", "FFFFFF", .26f);
        floor.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(path));
        var wall = Material("CreamPlaster", "F4EADB", .12f);
        var ceiling = Material("IvoryCeiling", "F6F1E7", .10f);
        var floorRenderer = renderers.Single(r => r.name == "Floor");
        floorRenderer.sharedMaterial = floor;
        var filter = floorRenderer.GetComponent<MeshFilter>();
        var mesh = UnityEngine.Object.Instantiate(filter.sharedMesh);
        mesh.name = "FloorWorldScaleUV";
        var vertices = mesh.vertices; var normals = mesh.normals; var uv = new Vector2[vertices.Length];
        // One texture repeat is 2.8 m: eight board rows, approximately 35 cm wide.
        for (int i = 0; i < vertices.Length; i++)
        {
            var p = filter.transform.TransformPoint(vertices[i]);
            var n = filter.transform.TransformDirection(normals[i]);
            uv[i] = (Mathf.Abs(n.y) > .5f ? new Vector2(p.x,p.z) : Mathf.Abs(n.x) > .5f ? new Vector2(p.z,p.y) : new Vector2(p.x,p.y)) / 2.8f;
        }
        mesh.uv = uv;
        AssetDatabase.CreateAsset(mesh, Root + "/Meshes/FloorWorldScaleUV.asset");
        filter.sharedMesh = mesh;
        string[] walls = {"LeftWall", "RightWall", "MainWall", "LeftColumn", "RightColumn"};
        foreach (string name in walls) renderers.Single(r => r.name == name).sharedMaterial = wall;
        renderers.Single(r => r.name == "Ceiling").sharedMaterial = ceiling;
        foreach (var component in renderers.Cast<UnityEngine.Object>().Concat(new UnityEngine.Object[]{filter}))
            if (PrefabUtility.IsPartOfPrefabInstance(component)) PrefabUtility.RecordPrefabInstancePropertyModifications(component);

        // Keep the current room reflection consistent with the new floor and wall colors.
        var probe = All<ReflectionProbe>(scene).Single(p => p.name == "BakedStudioReflection");
        var gun = All<PlayerShooter>(scene).Single().gameObject;
        bool gunActive = gun.activeSelf;
        try {
            gun.SetActive(false);
            string reflection = Root + "/Reflections/CreamOakStudio.exr";
            if (!Lightmapping.BakeReflectionProbe(probe, reflection)) throw new InvalidOperationException("Room reflection bake failed.");
            AssetDatabase.ImportAsset(reflection, ImportAssetOptions.ForceSynchronousImport);
            probe.mode = ReflectionProbeMode.Custom;
            probe.customBakedTexture = AssetDatabase.LoadAssetAtPath<Cubemap>(reflection);
            if (probe.customBakedTexture == null) throw new InvalidOperationException("Missing V7 reflection.");
        } finally {gun.SetActive(gunActive);}

        if (renderers.Length != All<MeshRenderer>(scene).Length || colliders.Length != All<Collider>(scene).Length)
            throw new InvalidOperationException("Art pass unexpectedly changed object counts.");
        if (!beforeGameplay.SequenceEqual(gameplay.Select(EditorJsonUtility.ToJson)) || !beforeColliders.SequenceEqual(colliders.Select(EditorJsonUtility.ToJson)))
            throw new InvalidOperationException("Art pass unexpectedly changed gameplay or collider settings.");
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed.");
        StudioInterfaceBuilder.CaptureBatch();
        Debug.Log("ROOM_FINISH_V7_OK: oak floor with world-scaled UVs, five cream walls/columns, ivory ceiling, refreshed reflection; gameplay and collider settings preserved.");
    }

    static Material Material(string name, string color, float smoothness)
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) throw new InvalidOperationException("URP/Lit missing.");
        var result = new Material(shader) {name = name};
        ColorUtility.TryParseHtmlString("#" + color, out var value);
        result.SetColor("_BaseColor", value); result.SetFloat("_Metallic", 0); result.SetFloat("_Smoothness", smoothness);
        AssetDatabase.CreateAsset(result, Root + "/Materials/" + name + ".mat");
        return result;
    }
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
}
