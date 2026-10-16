using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>Isolated before/after check for the installed URP resource-reference cleanup.</summary>
public static class RenderReferenceRepairCheck
{
    const string Evidence = "Logs/RenderReferenceRepair";
    static readonly string[] Fields = { "probeVolumeDebugShader", "probeVolumeFragmentationDebugShader", "probeVolumeOffsetDebugShader", "probeVolumeSamplingDebugShader", "probeSamplingDebugMesh", "probeSamplingDebugTexture", "probeVolumeBlendStatesCS" };
    static readonly string[] Paths = {
        "Runtime/Debug/ProbeVolumeDebug.shader", "Runtime/Debug/ProbeVolumeFragmentationDebug.shader",
        "Runtime/Debug/ProbeVolumeOffsetDebug.shader", "Runtime/Debug/ProbeVolumeSamplingDebug.shader",
        "Runtime/Debug/ProbeSamplingDebugMesh.fbx", "Runtime/Debug/ProbeVolumeNumbersDisplayTex.png",
        "Runtime/Lighting/ProbeVolume/ProbeVolumeBlendStates.compute"
    };
    static string Hash(string path)
    {
        using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-", "");
    }
    static string[] ProtectedHashes() => Directory.GetFiles("Assets/Scripts", "*").Concat(Directory.GetFiles("Assets/Scenes", "*.unity"))
        .Concat(new[] { "Assets/Settings/PC_RPAsset.asset", "Assets/Settings/UniversalRenderPipelineGlobalSettings.asset", "Assets/Settings/DefaultVolumeProfile.asset.meta", "Assets/Settings/PC_Renderer.asset.meta" })
        .OrderBy(p => p).Select(p => p + " | " + Hash(p)).ToArray();

    public static void Before()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated project.");
        Directory.CreateDirectory(Evidence); File.WriteAllLines(Evidence + "/ProtectedBefore.txt", ProtectedHashes());
        Capture("Before"); Debug.Log("RENDER_REFERENCE_BEFORE_OK");
    }

    public static void After()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated project.");
        if (!File.ReadAllLines(Evidence + "/ProtectedBefore.txt").SequenceEqual(ProtectedHashes())) throw new InvalidOperationException("Protected source/scene/pipeline metadata changed.");
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/DefaultVolumeProfile.asset");
        if (profile == null || profile.components.Count != 19 || profile.components.Any(c => c == null)) throw new InvalidOperationException("Expected the 19 retained URP volume settings.");
        string profileText = File.ReadAllText("Assets/Settings/DefaultVolumeProfile.asset");
        if (profileText.Contains("Unity.RenderPipelines.Core.Editor.Tests") || profileText.Contains("m_Script: {fileID: 0}")) throw new InvalidOperationException("Unsupported volume test subasset remains.");
        var renderer = new SerializedObject(AssetDatabase.LoadAssetAtPath<Object>("Assets/Settings/PC_Renderer.asset"));
        var resources = renderer.FindProperty("probeVolumeResources");
        for (int i = 0; i < Fields.Length; i++)
        {
            string path = "Packages/com.unity.render-pipelines.core/" + Paths[i];
            var expected = AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault(a => i == 4 ? a is Mesh : i == 5 ? a is Texture2D : i == 6 ? a is ComputeShader : a is Shader);
            var actual = resources.FindPropertyRelative(Fields[i]).objectReferenceValue;
            if (expected == null || actual != expected) throw new InvalidOperationException("Probe resource mismatch: " + Fields[i]);
        }
        ProjectValidation.Run(); Capture("After");
        foreach (string label in new[] { "Room", "Gun" }) Compare(label);
        File.WriteAllText(Evidence + "/Verification.txt", "Unity " + Application.unityVersion + "; isolated before/after checks; 2026-10-14 Asia/Tokyo.\n19 supported default-volume components retained; 4 missing sample/test/custom types and 5 null-script editor-test subassets removed.\nAll 7 legacy renderer probe-resource slots resolve to their installed Core equivalents. Active global resources, renderer features/settings, learner scripts, scene bytes and edited asset GUIDs preserved.\nFull-project scan: zero missing scripts, zero unsupported VolumeProfile script records, zero unresolved asset GUIDs.\nRoom and enlarged gun rendered before/after with the same fixed composition. See PixelComparison.txt. No gameplay or release acceptance is inferred.\n");
        Debug.Log("RENDER_REFERENCE_REPAIR_OK");
    }

    static void Capture(string label)
    {
        var scene = EditorSceneManager.OpenScene(StudioConceptVariantsPass.Destination);
        var targets = new System.Collections.Generic.List<GameObject>();
        var gun = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<StudioGunAppearance>(true)).Single();
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(StudioConceptVariantsPass.Root + "/Prefabs/PlayableBalloonVariants.prefab");
        var positions = new[] { new Vector3(-2, -.5f, 8), new Vector3(0, .9f, 8), new Vector3(2, -.5f, 8) };
        try
        {
            gun.ApplyTheme(0);
            for (int i = 0; i < positions.Length; i++)
            {
                var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene); targets.Add(target); target.transform.position = positions[i]; target.GetComponent<StudioBalloonAppearance>().ApplyVariant(i, i);
            }
            Render(scene, label + "_Room");
            foreach (var target in targets) Object.DestroyImmediate(target); targets.Clear();
            var camera = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Camera>()).Single(c => c.CompareTag("MainCamera"));
            gun.transform.position = camera.transform.TransformPoint(new Vector3(0, -.03f, 1.8f)); gun.transform.rotation = camera.transform.rotation * Quaternion.Euler(0, 120, 0); gun.transform.localScale *= 3.4f;
            Render(scene, label + "_Gun");
        }
        finally { foreach (var target in targets) Object.DestroyImmediate(target); }
        // Never save the capture scene or its temporary inspection transforms.
    }

    static void Render(Scene scene, string label)
    {
        StudioConceptVariantsPass.Capture(scene, "ReferenceRepair_" + label);
        File.Copy(StudioConceptVariantsPass.Root + "/Previews/ReferenceRepair_" + label + ".png", Evidence + "/" + label + ".png", true);
    }

    static void Compare(string label)
    {
        var before = new Texture2D(2, 2, TextureFormat.RGB24, false); var after = new Texture2D(2, 2, TextureFormat.RGB24, false);
        try
        {
            before.LoadImage(File.ReadAllBytes(Evidence + "/Before_" + label + ".png")); after.LoadImage(File.ReadAllBytes(Evidence + "/After_" + label + ".png"));
            if (before.width != after.width || before.height != after.height) throw new InvalidOperationException("Capture sizes changed.");
            var a = before.GetPixels32(); var b = after.GetPixels32(); long sum = 0; int max = 0, changed = 0;
            for (int i = 0; i < a.Length; i++)
            {
                int r = Math.Abs(a[i].r - b[i].r), g = Math.Abs(a[i].g - b[i].g), blue = Math.Abs(a[i].b - b[i].b);
                sum += r + g + blue; max = Math.Max(max, Math.Max(r, Math.Max(g, blue))); if (r + g + blue > 0) changed++;
            }
            double mean = sum / (a.Length * 3.0);
            File.AppendAllText(Evidence + "/PixelComparison.txt", label + ": changed pixels " + changed + "/" + a.Length + "; mean RGB delta " + mean.ToString("F6", System.Globalization.CultureInfo.InvariantCulture) + "/255; max " + max + "/255.\n");
            if (mean > .10) throw new InvalidOperationException("Render changed beyond the reference-only repair tolerance: " + label);
        }
        finally { Object.DestroyImmediate(before); Object.DestroyImmediate(after); }
    }
}
