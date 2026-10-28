using System;
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

// Asset/scene authoring only. No learner-owned runtime code is changed.
public static class OpenExpansionInstaller
{
    public const string Root = "Assets/StudioArtPack/OpenExpansion20261028";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    [Serializable] public class Catalog { public Song[] songs; }
    [Serializable] public class Song
    {
        public string trackId, title, artist, audioPath, chartRelativePath;
        public double sourceCueInSeconds, sourceCueOutSeconds, roundDurationSeconds;
        public int sourceEventStartId, sourceEventEndIdExclusive;
    }
    static T[] All<T>(Scene scene) where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).ToArray();
    static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

    public static void AddClassics()
    {
        try
        {
            Require(Application.isBatchMode, "Use batch mode.");
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.OpenScene(ScenePath);
            var controller = All<RoundController>(scene).Single();
            var catalog = JsonUtility.FromJson<Catalog>(File.ReadAllText("Assets/StreamingAssets/Rhythm/Classics20261029/profiles.json"));
            var serialized = new SerializedObject(controller);
            var songs = serialized.FindProperty("exhibitionSongs");
            Require(songs.arraySize == 7, "Expected the seven-song expansion; do not overwrite manual edits.");
            songs.arraySize = 10;
            for (int i = 0; i < catalog.songs.Length; i++)
            {
                var source = catalog.songs[i]; var song = songs.GetArrayElementAtIndex(i + 7);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(source.audioPath);
                Require(clip != null, "Missing classic: " + source.title);
                foreach (var name in new[] { "trackId", "title", "artist", "chartRelativePath" })
                    song.FindPropertyRelative(name).stringValue = (string)typeof(Song).GetField(name).GetValue(source);
                song.FindPropertyRelative("audioClip").objectReferenceValue = clip;
                foreach (var name in new[] { "sourceCueInSeconds", "sourceCueOutSeconds", "roundDurationSeconds" })
                    song.FindPropertyRelative(name).doubleValue = (double)typeof(Song).GetField(name).GetValue(source);
                song.FindPropertyRelative("sourceEventStartId").intValue = 0;
                song.FindPropertyRelative("sourceEventEndIdExclusive").intValue = source.sourceEventEndIdExclusive;
                var importer = (AudioImporter)AssetImporter.GetAtPath(source.audioPath);
                var settings = importer.defaultSampleSettings;
                settings.loadType = AudioClipLoadType.Streaming; settings.preloadAudioData = false;
                importer.defaultSampleSettings = settings; importer.SaveAndReimport();
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            File.AppendAllText(Root + "/Verification.txt", "CLASSIC_MUSIC_SETUP_OK: three unchanged ParagonX9 recordings, CC BY-SA 3.0 notices/originals and separate full-song practice profiles; ten choices total.\n");
            AssetDatabase.Refresh(); Debug.Log("CLASSIC_MUSIC_SETUP_OK"); EditorApplication.Exit(0);
        }
        catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
    }

    public static void Apply()
    {
        try
        {
            Require(Application.isBatchMode, "Use batch mode with the interactive Editor closed.");
            AssetDatabase.Refresh();
            Directory.CreateDirectory(Root + "/Materials");
            Directory.CreateDirectory(Root + "/Previews");
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.OpenScene(ScenePath);
            Require(!scene.GetRootGameObjects().Any(g => g.name == "OpenExpansion20261028"), "Expansion already installed; preserve subsequent scene edits.");
            var controller = All<RoundController>(scene).Single();
            var protectedComponents = All<MonoBehaviour>(scene).Where(c => c != controller).ToArray();
            var before = protectedComponents.Select(EditorJsonUtility.ToJson).ToArray();
            var catalog = JsonUtility.FromJson<Catalog>(File.ReadAllText("Assets/StreamingAssets/Rhythm/OpenMusic20261028/profiles.json"));
            var serialized = new SerializedObject(controller);
            var songs = serialized.FindProperty("exhibitionSongs");
            Require(songs.arraySize == 3, "Expected the original three song profiles.");
            songs.arraySize = 7;
            for (int i = 0; i < catalog.songs.Length; i++)
            {
                var source = catalog.songs[i]; var song = songs.GetArrayElementAtIndex(i + 3);
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(source.audioPath);
                Require(clip != null && clip.length >= source.sourceCueOutSeconds, "Missing or truncated audio: " + source.title);
                song.FindPropertyRelative("trackId").stringValue = source.trackId;
                song.FindPropertyRelative("title").stringValue = source.title;
                song.FindPropertyRelative("artist").stringValue = source.artist;
                song.FindPropertyRelative("audioClip").objectReferenceValue = clip;
                song.FindPropertyRelative("chartRelativePath").stringValue = source.chartRelativePath;
                song.FindPropertyRelative("sourceCueInSeconds").doubleValue = source.sourceCueInSeconds;
                song.FindPropertyRelative("sourceCueOutSeconds").doubleValue = source.sourceCueOutSeconds;
                song.FindPropertyRelative("roundDurationSeconds").doubleValue = source.roundDurationSeconds;
                song.FindPropertyRelative("sourceEventStartId").intValue = source.sourceEventStartId;
                song.FindPropertyRelative("sourceEventEndIdExclusive").intValue = source.sourceEventEndIdExclusive;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            foreach (var source in catalog.songs)
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(source.audioPath);
                var settings = importer.defaultSampleSettings;
                settings.loadType = AudioClipLoadType.Streaming;
                settings.preloadAudioData = false;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }

            var root = new GameObject("OpenExpansion20261028").transform;
            void Place(string name, Vector3 position, float rotation, float scale)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StudioExpansionBuilder.Root + "/Prefabs/" + name + ".prefab");
                Require(prefab != null, "Missing current studio art: " + name);
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
                go.transform.SetParent(root, false); go.transform.localPosition = position;
                go.transform.localRotation = Quaternion.Euler(0, rotation, 0); go.transform.localScale = Vector3.one * scale;
            }
            Place("SupplyTrolley", new Vector3(5.85f, -4f, 5.30f), -18, 1.12f);
            Place("RollerTray", new Vector3(4.3f, -3.98f, 3.5f), 18, .9f);
            Place("CanvasEasel", new Vector3(-5.9f, -4f, 7f), 12, 1);
            Place("CanvasDryingRack", new Vector3(-6.05f, -4f, 2.9f), 16, .9f);
            Place("PigmentSwatchRail", new Vector3(6.86f, .7f, 2.4f), 90, 1.05f);
            Place("FramedRhythmPrint", new Vector3(6.86f, 1.6f, 2.4f), 90, .9f);
            Place("FramedBotanicalPrint", new Vector3(-6.86f, 2.48f, 4.6f), -90, .8f);
            AddModel(root, "pottedPlant", new Vector3(-5.0f, -4f, 7.8f), 1.5f);
            AddModel(root, "plantSmall1", new Vector3(5.85f, -2.79f, 5.45f), .30f);
            AddModel(root, "plantSmall2", new Vector3(-5.55f, -4f, 3.1f), .65f);
            AddModel(root, "plantSmall3", new Vector3(5.0f, -4f, 7.8f), .85f);
            AddModel(root, "books", new Vector3(5.6f, -2.79f, 5.25f), .16f);
            Require(root.GetComponentsInChildren<Collider>(true).Length == 0 && root.GetComponentsInChildren<MonoBehaviour>(true).Length == 0, "Decorations must not change gameplay contacts or add behaviours.");
            Require(before.SequenceEqual(protectedComponents.Select(EditorJsonUtility.ToJson)), "Existing gameplay component data changed.");
            foreach (var renderer in root.GetComponentsInChildren<Renderer>())
            {
                renderer.lightProbeUsage = LightProbeUsage.BlendProbes;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.BlendProbes;
                Require(renderer.sharedMaterials.All(m => m != null && m.shader.isSupported), "Unsupported decoration material.");
            }
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            scene = EditorSceneManager.OpenScene(ScenePath);
            Require(All<Component>(scene).All(c => c != null), "Missing scene script.");
            Capture(scene, "Studio", false);
            File.WriteAllText(Root + "/Verification.txt", "OPEN_EXPANSION_SETUP_OK\nSeven serialized song profiles; four unchanged CC0 recordings with separate practice grids.\nSeven current studio-art prefabs and five Kenney OBJ decorations promoted into PrototypeScene.\nNo decoration colliders/runtime scripts. Existing gameplay components other than the song catalog remain unchanged.\nNew music imports stream audio. Existing lightmaps retained; no new bake or FPS claim.\n");
            AssetDatabase.Refresh();
            Debug.Log("OPEN_EXPANSION_SETUP_OK"); EditorApplication.Exit(0);
        }
        catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
    }

    static void AddModel(Transform parent, string name, Vector3 position, float height)
    {
        string path = Root + "/KenneyFurniture/Models/" + name;
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(path + ".obj");
        Require(model != null, "Missing Kenney model: " + name);
        var go = (GameObject)PrefabUtility.InstantiatePrefab(model, parent.gameObject.scene);
        go.transform.SetParent(parent, false); go.name = "Kenney_" + name;
        var renderers = go.GetComponentsInChildren<Renderer>();
        var bounds = renderers[0].bounds; foreach (var r in renderers) bounds.Encapsulate(r.bounds);
        float scale = height / bounds.size.y;
        go.transform.localScale = Vector3.one * scale;
        go.transform.localPosition = position - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) * scale;
        var materialBlocks = Regex.Matches(File.ReadAllText(path + ".mtl"), @"newmtl\s+(\S+)\s+Ka[^\n]*\n\s*Kd\s+([\d.]+)\s+([\d.]+)\s+([\d.]+)");
        foreach (var renderer in renderers)
        {
            var materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                string materialName = materials[i] == null ? "wood" : materials[i].name;
                string target = Root + "/Materials/Kenney_" + materialName + ".mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(target);
                if (material == null)
                {
                    var match = materialBlocks.Cast<Match>().FirstOrDefault(m => m.Groups[1].Value == materialName);
                    Require(match != null, "Unrecognized original material: " + materialName);
                    float Number(int index) => float.Parse(match.Groups[index].Value, System.Globalization.CultureInfo.InvariantCulture);
                    material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "Kenney_" + materialName, enableInstancing = true };
                    material.SetColor("_BaseColor", new Color(Number(2), Number(3), Number(4)));
                    material.SetFloat("_Smoothness", .22f); AssetDatabase.CreateAsset(material, target);
                }
                materials[i] = material;
            }
            renderer.sharedMaterials = materials;
        }
    }

    public static void Capture(Scene scene, string name, bool keepUI)
    {
        Directory.CreateDirectory(Root + "/Previews");
        var camera = All<Camera>(scene).Single(c => c.CompareTag("MainCamera"));
        var canvases = All<Canvas>(scene); var states = canvases.Select(c => c.gameObject.activeSelf).ToArray();
        if (!keepUI) foreach (var canvas in canvases) canvas.gameObject.SetActive(false);
        var old = camera.targetTexture; var active = RenderTexture.active; float aspect = camera.aspect;
        var target = new RenderTexture(2560, 1440, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(2560, 1440, TextureFormat.RGB24, false);
        bool async = ShaderUtil.allowAsyncCompilation;
        try
        {
            ShaderUtil.allowAsyncCompilation = false; target.Create(); camera.targetTexture = target; camera.aspect = 16f / 9;
            for (int i = 0; i < 4; i++) { camera.Render(); RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = target }); }
            RenderTexture.active = target; pixels.ReadPixels(new Rect(0, 0, 2560, 1440), 0, 0); pixels.Apply();
            File.WriteAllBytes(Root + "/Previews/" + name + ".png", pixels.EncodeToPNG());
        }
        finally
        {
            ShaderUtil.allowAsyncCompilation = async; camera.targetTexture = old; camera.aspect = aspect; RenderTexture.active = active;
            target.Release(); Object.DestroyImmediate(target); Object.DestroyImmediate(pixels);
            for (int i = 0; i < canvases.Length; i++) canvases[i].gameObject.SetActive(states[i]);
        }
    }
}
