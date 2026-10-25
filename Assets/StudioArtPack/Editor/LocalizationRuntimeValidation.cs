using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BalloonBarrage.EditorTools
{
    /// <summary>
    /// Exercises the learner-owned localization runtime without changing a
    /// production scene or persisting a language preference.
    /// </summary>
    public static class LocalizationRuntimeValidation
    {
        private const string TablePath =
            "Assets/Localization/UIStrings.json";

        [MenuItem("Tools/Balloon Barrage/Validate Localization Runtime")]
        public static void ValidateFromMenu()
        {
            Validate();
        }

        public static void RunBatch()
        {
            try
            {
                Validate();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void Validate()
        {
            Type managerType = typeof(LocalizationManager);
            FieldInfo entriesField = managerType.GetField(
                "Entries", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo readyField = managerType.GetField(
                "<IsReady>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo assetField = managerType.GetField(
                "stringTableAsset",
                BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo loadMethod = managerType.GetMethod(
                "LoadTable", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo languageField = typeof(LanguageSettings).GetField(
                "<Current>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Static);

            Require(entriesField != null, "Localization entries field changed.");
            Require(readyField != null, "Localization ready field changed.");
            Require(assetField != null, "Localization asset field changed.");
            Require(loadMethod != null, "Localization load method changed.");
            Require(languageField != null, "Current language field changed.");

            var entries =
                (Dictionary<string, LocalizationEntry>)entriesField.GetValue(null);
            var originalEntries =
                new Dictionary<string, LocalizationEntry>(entries);
            bool originalReady = LocalizationManager.IsReady;
            UILanguage originalLanguage = LanguageSettings.Current;
            GameObject host = null;

            try
            {
                readyField.SetValue(null, false);
                Require(
                    LocalizationManager.GetText("menu.start") == "menu.start",
                    "A not-ready manager must return the requested key.");

                TextAsset tableAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                    TablePath);
                Require(tableAsset != null, "Missing localization table.");

                host = new GameObject("__LocalizationRuntimeValidation")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                LocalizationManager manager =
                    host.AddComponent<LocalizationManager>();
                assetField.SetValue(manager, tableAsset);
                loadMethod.Invoke(manager, null);

                Require(
                    LocalizationManager.IsReady,
                    "Localization manager did not become ready.");

                AssertText(
                    languageField,
                    UILanguage.SimplifiedChinese,
                    "menu.start",
                    "开始游戏");
                AssertText(
                    languageField,
                    UILanguage.Japanese,
                    "menu.start",
                    "ゲーム開始");
                AssertText(
                    languageField,
                    UILanguage.English,
                    "menu.start",
                    "Start Game");
                AssertFormat(
                    languageField,
                    UILanguage.SimplifiedChinese,
                    "feedback.onBeat",
                    "节拍命中 +20",
                    20);
                AssertFormat(
                    languageField,
                    UILanguage.Japanese,
                    "cue.next",
                    "あと 5.2 秒",
                    "5.2");
                AssertFormat(
                    languageField,
                    UILanguage.English,
                    "result.maxStreak",
                    "Max Streak: 7",
                    7);

                const string MissingKey = "validation.missing-key";
                Require(
                    LocalizationManager.GetText(MissingKey) == MissingKey,
                    "A missing localization key must return itself.");

                Debug.Log(
                    "LOCALIZATION_RUNTIME_OK: not-ready fallback, " +
                    "three-language lookup, dynamic formatting and " +
                    "missing-key fallback passed.");
            }
            finally
            {
                languageField.SetValue(null, originalLanguage);
                entries.Clear();
                foreach (KeyValuePair<string, LocalizationEntry> pair in
                         originalEntries)
                {
                    entries[pair.Key] = pair.Value;
                }

                readyField.SetValue(null, originalReady);
                if (host != null)
                {
                    UnityEngine.Object.DestroyImmediate(host);
                }
            }
        }

        private static void AssertText(
            FieldInfo languageField,
            UILanguage language,
            string key,
            string expected)
        {
            languageField.SetValue(null, language);
            string actual = LocalizationManager.GetText(key);
            Require(
                actual == expected,
                $"Expected '{expected}' for {language}, got '{actual}'.");
        }

        private static void AssertFormat(
            FieldInfo languageField,
            UILanguage language,
            string key,
            string expected,
            params object[] arguments)
        {
            languageField.SetValue(null, language);
            string actual = LocalizationManager.Format(key, arguments);
            Require(
                actual == expected,
                $"Expected '{expected}' for {language}, got '{actual}'.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
