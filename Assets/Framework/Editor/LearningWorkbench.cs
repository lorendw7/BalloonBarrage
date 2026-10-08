using System.Collections.Generic;
using BalloonBarrage.Foundation;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BalloonBarrage.EditorTools
{
    // Read-only diagnostics. Never rewires objects or saves scenes.
    // 只读诊断：不自动接线、不修改或保存场景。
    public sealed class LearningWorkbench : EditorWindow
    {
        private const string SettingsPath = "Assets/Settings/Gameplay/BalloonSpawnSettings.asset";
        private readonly List<string> results = new List<string>();
        private Vector2 scroll;

        [MenuItem("Tools/BalloonBarrage/Learning Workbench")]
        private static void Open()
        {
            GetWindow<LearningWorkbench>("Learning / 学习");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("框架助手 / Framework helper", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "只检查接线；绿色配置不代表玩法已完成。核心逻辑由你写。\n" +
                "Checks wiring only; valid settings do not mean finished gameplay. You author the core.",
                MessageType.Info);

            if (GUILayout.Button("选择生成配置 / Select spawn settings"))
                SelectAsset(SettingsPath);
            if (GUILayout.Button("打开核心生成脚本 / Open learner spawner"))
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Scripts/BalloonSpawner.cs");
                if (script != null) AssetDatabase.OpenAsset(script);
            }
            if (GUILayout.Button("检查当前原型 / Check current prototype")) CheckCurrentPrototype();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var result in results) EditorGUILayout.LabelField(result, EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndScrollView();

            EditorGUILayout.HelpBox(
                "本课验收：击破后等待约 0.9 秒，只生成一个新球；调成 2 秒后等待变长。\n" +
                "Acceptance: wait about 0.9 s after a pop, spawn exactly one balloon; a 2 s setting lengthens the wait.",
                MessageType.None);
        }

        private static void SelectAsset(string path)
        {
            var asset = AssetDatabase.LoadMainAssetAtPath(path);
            if (asset == null) return;
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        private void CheckCurrentPrototype()
        {
            results.Clear();
            var settings = AssetDatabase.LoadAssetAtPath<BalloonSpawnSettings>(SettingsPath);
            results.Add(settings != null
                ? "[OK] 生成配置存在 / Spawn settings exist (not automatically connected)."
                : "[!] 生成配置缺失 / Missing spawn settings.");

            if (SceneManager.GetActiveScene().path != "Assets/Scenes/PrototypeScene.unity")
            {
                results.Add("[!] 请先打开 PrototypeScene / Open PrototypeScene first.");
                return;
            }
            if (EditorApplication.isPlaying)
            {
                results.Add("[!] 请停止运行后检查保存接线 / Stop Play Mode to inspect scene wiring.");
                return;
            }

            var scene = SceneManager.GetActiveScene();
            var shooters = InScene<PlayerShooter>(scene);
            results.Add(shooters.Count == 1
                ? "[OK] 一个玩家发射器 / One player shooter."
                : "[!] 发射器数量 / Shooter count: " + shooters.Count);
            foreach (var shooter in shooters)
            {
                CheckReference(shooter, "muzzle");
                CheckReference(shooter, "paintProjectilePrefab");
                CheckReference(shooter, "muzzleSpray");
                var oldVfx = shooter.GetComponent<PaintGunVfx>();
                if (oldVfx != null && oldVfx.enabled)
                    results.Add("[!] 旧 PaintGunVfx 仍启用；可能重复播放 / Old VFX input is still enabled.");
            }

            var spawners = InScene<BalloonSpawner>(scene);
            results.Add(spawners.Count == 1
                ? "[OK] 一个气球生成器 / One balloon spawner."
                : "[!] 生成器数量 / Spawner count: " + spawners.Count);
            foreach (var spawner in spawners)
            {
                CheckReference(spawner, "balloonPrefab");
                CheckReference(spawner, "spawnSettings");
            }
            results.Add("[待试玩 / Playtest] 延迟、命中和入场仍需运行验证 / Timing, hits, and entrance require Play Mode.");
        }

        private static List<T> InScene<T>(Scene scene) where T : Component
        {
            var found = new List<T>();
            foreach (var component in Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (component.gameObject.scene == scene && component.gameObject.activeInHierarchy
                    && (!(component is Behaviour behaviour) || behaviour.enabled)) found.Add(component);
            return found;
        }

        private void CheckReference(Object target, string field)
        {
            var property = new SerializedObject(target).FindProperty(field);
            if (property == null)
            {
                results.Add("[待接入 / Pending] " + field + " 字段尚未添加 / Field not added yet.");
                return;
            }
            bool assigned = property.propertyType == SerializedPropertyType.ObjectReference
                && property.objectReferenceValue != null;
            results.Add((assigned ? "[OK] " : "[!] ") + target.name + ": " + field
                + (assigned ? " 已连接 / assigned" : " 未连接 / unassigned"));
        }
    }
}
