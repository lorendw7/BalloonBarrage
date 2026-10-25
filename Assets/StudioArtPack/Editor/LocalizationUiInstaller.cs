using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace BalloonBarrage.EditorTools
{
    /// <summary>
    /// Installs localization bindings into the studio UI prefabs while leaving
    /// the learner's gameplay scene and gameplay scripts untouched.
    /// </summary>
    public static class LocalizationUiInstaller
    {
        private const string PrefabFolder =
            "Assets/StudioArtPack/InterfaceV2/Prefabs/";
        private const string StartScreenPath =
            PrefabFolder + "StartScreen.prefab";
        private const string OnBeatFeedbackPath =
            "Assets/StudioArtPack/RhythmFeedback/OnBeatFeedback.prefab";
        private const string TablePath =
            "Assets/Localization/UIStrings.json";
        private const string ChineseFontPath =
            "Assets/StudioArtPack/InterfaceV2/Fonts/Chinese/" +
            "NotoSansSC-Medium.ttf";
        private const string JapaneseFontPath =
            "Assets/StudioArtPack/InterfaceV2/Fonts/Japanese/" +
            "MPlus1C-Regular.ttf";
        private const string SelectorName = "LanguageSelector";

        private sealed class Binding
        {
            public readonly string PrefabPath;
            public readonly string ObjectName;
            public readonly string ParentName;
            public readonly string Key;
            public readonly Color? SimplifiedChineseColor;

            public Binding(
                string prefabName,
                string objectName,
                string parentName,
                string key,
                Color? simplifiedChineseColor = null)
            {
                PrefabPath = PrefabFolder + prefabName;
                ObjectName = objectName;
                ParentName = parentName;
                Key = key;
                SimplifiedChineseColor = simplifiedChineseColor;
            }
        }

        private sealed class FontBinding
        {
            public readonly string PrefabPath;
            public readonly string ObjectName;

            public FontBinding(string prefabPath, string objectName)
            {
                PrefabPath = prefabPath;
                ObjectName = objectName;
            }
        }

        private static readonly Binding[] Bindings =
        {
            new Binding("StartScreen.prefab", "ControlsTitle", null,
                "menu.howToPlay", new Color(0.13f, 0.17f, 0.16f, 1f)),
            new Binding("StartScreen.prefab", "ControlsText", null,
                "instructions.startSummary",
                new Color(0.13f, 0.17f, 0.16f, 1f)),
            new Binding("StartScreen.prefab", "Eyebrow", null,
                "menu.studio", new Color(0.25f, 0.21f, 0.18f, 1f)),
            new Binding("StartScreen.prefab", "Subtitle", null,
                "menu.subtitle", new Color(0.25f, 0.21f, 0.18f, 1f)),
            new Binding("StartScreen.prefab", "Footer", null,
                "menu.tagline", new Color(0.25f, 0.21f, 0.18f, 1f)),
            new Binding("StartScreen.prefab", "Label", "StartButton",
                "menu.start"),
            new Binding("StartScreen.prefab", "Label", "QuitButton",
                "menu.quit"),
            new Binding("StartScreen.prefab", "AuthorCreditHeading", null,
                "menu.createdBy", new Color(0.25f, 0.18f, 0.12f, 1f)),

            new Binding("GameplayHUD.prefab", "ScoreCaption", null,
                "hud.scoreLabel"),
            new Binding("GameplayHUD.prefab", "TimerCaption", null,
                "hud.timeLeft"),
            new Binding("GameplayHUD.prefab", "PauseHint", null,
                "hud.pauseHint"),

            new Binding("PauseScreen.prefab", "Title", null,
                "pause.title"),
            new Binding("PauseScreen.prefab", "Description", null,
                "pause.message"),
            new Binding("PauseScreen.prefab", "Label", "ResumeButton",
                "pause.resume"),
            new Binding("PauseScreen.prefab", "Label", "QuitButton",
                "menu.quit"),

            new Binding("ResultsScreen.prefab", "Eyebrow", null,
                "result.complete"),
            new Binding("ResultsScreen.prefab", "Title", null,
                "result.colorWellSpent"),
            new Binding("ResultsScreen.prefab", "ScoreCaption", null,
                "result.finalScore"),
            new Binding("ResultsScreen.prefab", "BestScoreCaption", null,
                "result.bestScore"),
            new Binding("ResultsScreen.prefab", "Label", "PlayAgainButton",
                "result.playAgain"),
            new Binding("ResultsScreen.prefab", "ReplayHint", null,
                "result.returnHint"),

            new Binding("InstructionsScreen.prefab", "Title", null,
                "menu.howToPlay"),
            new Binding("InstructionsScreen.prefab", "Goal", null,
                "instructions.goal"),
            new Binding("InstructionsScreen.prefab", "StepCopy0", null,
                "instructions.aim"),
            new Binding("InstructionsScreen.prefab", "StepCopy1", null,
                "instructions.fire"),
            new Binding("InstructionsScreen.prefab", "StepCopy2", null,
                "instructions.pause"),
            new Binding("InstructionsScreen.prefab", "Step0", null,
                "instructions.stepAim"),
            new Binding("InstructionsScreen.prefab", "Step1", null,
                "instructions.stepPaint"),
            new Binding("InstructionsScreen.prefab", "Step2", null,
                "instructions.stepPause"),
            new Binding("InstructionsScreen.prefab", "Label", "BackButton",
                "menu.back"),

            new Binding("RhythmCue.prefab", "BeatCueHeading", null,
                "cue.optionalBonus")
        };

        private static readonly FontBinding[] DynamicFontBindings =
        {
            new FontBinding(
                PrefabFolder + "RhythmCue.prefab", "BeatCueText"),
            new FontBinding(
                PrefabFolder + "ResultsScreen.prefab", "ResultStatsText"),
            new FontBinding(OnBeatFeedbackPath, "OnBeatFeedback")
        };

        [MenuItem("Tools/Balloon Barrage/Install Localization UI")]
        public static void InstallFromMenu()
        {
            Install();
        }

        public static void RunBatch()
        {
            try
            {
                Install();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void Install()
        {
            TextAsset table = AssetDatabase.LoadAssetAtPath<TextAsset>(TablePath);
            Font chineseFont = AssetDatabase.LoadAssetAtPath<Font>(
                ChineseFontPath);
            Font japaneseFont = AssetDatabase.LoadAssetAtPath<Font>(
                JapaneseFontPath);

            Require(table != null, "Missing localization table.");
            Require(chineseFont != null, "Missing Simplified Chinese font.");
            Require(japaneseFont != null, "Missing Japanese font.");

            IEnumerable<string> prefabPaths = Bindings
                .Select(binding => binding.PrefabPath)
                .Concat(DynamicFontBindings.Select(binding =>
                    binding.PrefabPath))
                .Distinct();

            foreach (string prefabPath in prefabPaths)
            {
                GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);

                try
                {
                    foreach (Binding binding in Bindings.Where(binding =>
                                 binding.PrefabPath == prefabPath))
                    {
                        ConfigureBinding(
                            root, binding, chineseFont, japaneseFont);
                    }

                    foreach (FontBinding binding in
                             DynamicFontBindings.Where(binding =>
                                 binding.PrefabPath == prefabPath))
                    {
                        ConfigureDynamicFont(
                            root, binding, chineseFont, japaneseFont);
                    }

                    if (prefabPath == StartScreenPath)
                    {
                        ConfigureManager(root, table);
                        ConfigureLanguageSelector(root, chineseFont);
                    }

                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }

            AssetDatabase.SaveAssets();
            ValidateSavedAssets();
            Debug.Log(
                $"LOCALIZATION_UI_INSTALL_OK: {Bindings.Length} static " +
                $"labels, {DynamicFontBindings.Length} dynamic fonts, " +
                "runtime manager and three language buttons installed.");
        }

        private static void ConfigureBinding(
            GameObject root,
            Binding binding,
            Font chineseFont,
            Font japaneseFont)
        {
            Transform target = FindTarget(root, binding);
            Require(
                target != null,
                $"Missing {binding.ObjectName} in {binding.PrefabPath}.");
            Require(
                target.GetComponent<Text>() != null,
                $"{binding.ObjectName} has no legacy Text component.");

            LocalizedText localized =
                target.GetComponent<LocalizedText>() ??
                target.gameObject.AddComponent<LocalizedText>();
            SerializedObject serialized = new SerializedObject(localized);
            serialized.FindProperty("localizationKey").stringValue = binding.Key;
            serialized.FindProperty("simplifiedChineseFont").objectReferenceValue =
                chineseFont;
            serialized.FindProperty("japaneseFont").objectReferenceValue =
                japaneseFont;
            serialized.FindProperty("useSimplifiedChineseColor").boolValue =
                binding.SimplifiedChineseColor.HasValue;
            if (binding.SimplifiedChineseColor.HasValue)
            {
                serialized.FindProperty("simplifiedChineseColor").colorValue =
                    binding.SimplifiedChineseColor.Value;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(localized);
        }

        private static void ConfigureDynamicFont(
            GameObject root,
            FontBinding binding,
            Font chineseFont,
            Font japaneseFont)
        {
            Transform target = root
                .GetComponentsInChildren<Transform>(true)
                .SingleOrDefault(transform =>
                    transform.name == binding.ObjectName);
            Require(
                target != null,
                $"Missing {binding.ObjectName} in {binding.PrefabPath}.");
            Require(
                target.GetComponent<Text>() != null ||
                target.GetComponent<TMPro.TMP_Text>() != null,
                $"{binding.ObjectName} has no supported text component.");

            LocalizedFont localizedFont =
                target.GetComponent<LocalizedFont>() ??
                target.gameObject.AddComponent<LocalizedFont>();
            SerializedObject serialized = new SerializedObject(localizedFont);
            serialized.FindProperty("simplifiedChineseFont")
                .objectReferenceValue = chineseFont;
            serialized.FindProperty("japaneseFont")
                .objectReferenceValue = japaneseFont;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(localizedFont);
        }

        private static void ConfigureManager(GameObject root, TextAsset table)
        {
            LocalizationManager manager =
                root.GetComponent<LocalizationManager>() ??
                root.AddComponent<LocalizationManager>();
            SerializedObject serialized = new SerializedObject(manager);
            serialized.FindProperty("stringTableAsset").objectReferenceValue =
                table;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(manager);
        }

        private static void ConfigureLanguageSelector(
            GameObject root,
            Font labelFont)
        {
            Transform existing = root.transform.Find(SelectorName);
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
            }

            GameObject selector = new GameObject(
                SelectorName,
                typeof(RectTransform),
                typeof(HorizontalLayoutGroup));
            selector.layer = 5;
            selector.transform.SetParent(root.transform, false);

            RectTransform selectorRect = selector.GetComponent<RectTransform>();
            selectorRect.anchorMin = new Vector2(1f, 1f);
            selectorRect.anchorMax = new Vector2(1f, 1f);
            selectorRect.pivot = new Vector2(1f, 1f);
            selectorRect.anchoredPosition = new Vector2(-42f, -34f);
            selectorRect.sizeDelta = new Vector2(250f, 40f);

            HorizontalLayoutGroup layout =
                selector.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            CreateLanguageButton(
                selector.transform,
                "ChineseButton",
                "中文",
                72f,
                UILanguage.SimplifiedChinese,
                labelFont);
            CreateLanguageButton(
                selector.transform,
                "JapaneseButton",
                "日本語",
                88f,
                UILanguage.Japanese,
                labelFont);
            CreateLanguageButton(
                selector.transform,
                "EnglishButton",
                "EN",
                64f,
                UILanguage.English,
                labelFont);
        }

        private static void CreateLanguageButton(
            Transform parent,
            string name,
            string caption,
            float width,
            UILanguage language,
            Font labelFont)
        {
            GameObject buttonObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button),
                typeof(LanguageButton));
            buttonObject.layer = 5;
            buttonObject.transform.SetParent(parent, false);

            RectTransform buttonRect =
                buttonObject.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(width, 38f);

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(
                "UI/Skin/UISprite.psd");
            image.type = Image.Type.Sliced;
            image.color = new Color(0.035f, 0.32f, 0.29f, 0.94f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.88f, 0.55f, 1f);
            colors.pressedColor = new Color(0.78f, 0.92f, 0.88f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            LanguageButton languageButton =
                buttonObject.GetComponent<LanguageButton>();
            SerializedObject serialized = new SerializedObject(languageButton);
            serialized.FindProperty("language").enumValueIndex = (int)language;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            UnityEventTools.AddPersistentListener(
                button.onClick, languageButton.SelectLanguage);

            GameObject labelObject = new GameObject(
                "Label",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text));
            labelObject.layer = 5;
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(6f, 2f);
            labelRect.offsetMax = new Vector2(-6f, -2f);

            Text label = labelObject.GetComponent<Text>();
            label.text = caption;
            label.font = labelFont;
            label.fontSize = 17;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.98f, 0.96f, 0.87f, 1f);
            label.raycastTarget = false;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 12;
            label.resizeTextMaxSize = 17;
        }

        private static Transform FindTarget(GameObject root, Binding binding)
        {
            return root
                .GetComponentsInChildren<Transform>(true)
                .SingleOrDefault(transform =>
                    transform.name == binding.ObjectName &&
                    (binding.ParentName == null ||
                     (transform.parent != null &&
                      transform.parent.name == binding.ParentName)));
        }

        private static void ValidateSavedAssets()
        {
            foreach (Binding binding in Bindings)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    binding.PrefabPath);
                Require(prefab != null, "Missing saved prefab.");
                Transform target = FindTarget(prefab, binding);
                Require(target != null, "Missing saved localization target.");
                LocalizedText localized = target.GetComponent<LocalizedText>();
                Require(localized != null, "Missing saved LocalizedText.");
                SerializedObject serialized = new SerializedObject(localized);
                Require(
                    serialized.FindProperty("localizationKey").stringValue ==
                    binding.Key,
                    $"Wrong localization key on {binding.ObjectName}.");
            }

            foreach (FontBinding binding in DynamicFontBindings)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    binding.PrefabPath);
                Require(prefab != null, "Missing saved dynamic text prefab.");
                Transform target = prefab
                    .GetComponentsInChildren<Transform>(true)
                    .SingleOrDefault(transform =>
                        transform.name == binding.ObjectName);
                Require(target != null, "Missing saved dynamic text target.");
                Require(
                    target.GetComponent<LocalizedFont>() != null,
                    $"Missing LocalizedFont on {binding.ObjectName}.");
            }

            GameObject start = AssetDatabase.LoadAssetAtPath<GameObject>(
                StartScreenPath);
            Require(start != null, "Missing start screen prefab.");
            LocalizationManager manager =
                start.GetComponent<LocalizationManager>();
            Require(manager != null, "Start screen lacks LocalizationManager.");
            Require(
                start.GetComponentsInChildren<LanguageButton>(true).Length == 3,
                "Start screen must contain three language buttons.");
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
