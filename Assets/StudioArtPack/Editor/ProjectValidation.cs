using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Batch validation only. Never saves inspected scenes or regenerates art.
public static class ProjectValidation
{
    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run validation in an isolated batch project.");
        int missingScripts = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            var root = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
        }
        foreach (var path in Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories))
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                    missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
        }
        var unresolved = new System.Collections.Generic.List<string>();
        int unsupportedVolumeScripts = 0;
        foreach (var path in Directory.GetFiles("Assets", "*", SearchOption.AllDirectories))
        {
            if (!new[] { ".unity", ".prefab", ".mat", ".asset" }.Contains(Path.GetExtension(path))) continue;
            string content = File.ReadAllText(path);
            if (!content.StartsWith("%YAML")) continue;
            // VolumeProfile.OnEnable silently prunes null entries; inspect the source too,
            // so unsupported editor-only subassets cannot evade the scene/prefab audit.
            if (Path.GetExtension(path) == ".asset" && AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
            {
                int count = Regex.Matches(content, @"m_Script: \{fileID: 0\}").Count;
                unsupportedVolumeScripts += count;
                if (count > 0) Debug.LogError("UNSUPPORTED_VOLUME_SCRIPTS " + path + ": " + count);
            }
            foreach (Match match in Regex.Matches(content, @"guid: ([0-9a-f]{32})"))
            {
                var guid = match.Groups[1].Value;
                if (guid.StartsWith("0000000000000000")) continue;
                if (string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid))) unresolved.Add(path + ": " + guid);
            }
        }
        foreach (var item in unresolved.Distinct()) Debug.LogError("UNRESOLVED_REFERENCE " + item);
        Debug.Log("PROJECT_AUDIT missingScripts=" + missingScripts + " unsupportedVolumeScripts=" + unsupportedVolumeScripts + " unresolved=" + unresolved.Distinct().Count());
        if (missingScripts > 0 || unsupportedVolumeScripts > 0 || unresolved.Count > 0) throw new Exception("Project reference validation failed; inspect log.");
        Debug.Log("PROJECT_VALIDATION_OK");
    }
}
