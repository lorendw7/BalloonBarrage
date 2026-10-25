using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BalloonBarrage.EditorTools
{
    /// <summary>
    /// Validates translation structure and exact bundled-font coverage without
    /// changing the production scene or learner-owned runtime code.
    /// </summary>
    public static class LocalizationTableValidation
    {
        private const string TablePath =
            "Assets/Localization/UIStrings.json";
        private const string JapaneseFontPath =
            "Assets/StudioArtPack/InterfaceV2/Fonts/Japanese/" +
            "MPlus1C-Regular.ttf";
        private const string ChineseFontPath =
            "Assets/StudioArtPack/InterfaceV2/Fonts/Chinese/" +
            "NotoSansSC-Medium.ttf";

        private static readonly string[] ExpectedLocales =
        {
            "zh-Hans",
            "ja",
            "en"
        };

        [MenuItem("Tools/Balloon Barrage/Validate Localization Assets")]
        public static void ValidateMenu()
        {
            Validate();
            Debug.Log(
                "LOCALIZATION_ASSETS_OK: keys, locales, placeholders and " +
                "exact Simplified Chinese/Japanese/English glyph coverage " +
                "passed.");
        }

        public static void RunBatch()
        {
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "Run batch localization validation in batch mode.");
            }

            try
            {
                ValidateMenu();
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
            TextAsset tableAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                TablePath);
            Font japaneseFont = AssetDatabase.LoadAssetAtPath<Font>(
                JapaneseFontPath);
            Font chineseFont = AssetDatabase.LoadAssetAtPath<Font>(
                ChineseFontPath);

            Require(tableAsset != null, "Missing localization table.");
            Require(japaneseFont != null, "Missing Japanese UI font.");
            Require(chineseFont != null, "Missing Simplified Chinese UI font.");

            LocalizationTable table = JsonUtility.FromJson<LocalizationTable>(
                tableAsset.text);

            Require(table != null, "Localization JSON could not be parsed.");
            Require(table.locales != null &&
                    table.locales.SequenceEqual(ExpectedLocales),
                "Locales must remain ordered as zh-Hans, ja and en.");
            Require(table.entries != null && table.entries.Length > 0,
                "Localization table has no entries.");

            HashSet<string> keys = new HashSet<string>(StringComparer.Ordinal);

            foreach (LocalizationEntry entry in table.entries)
            {
                Require(entry != null, "Localization table has a null entry.");
                Require(!string.IsNullOrWhiteSpace(entry.key),
                    "Localization entry has an empty key.");
                Require(keys.Add(entry.key),
                    "Duplicate localization key: " + entry.key);
                Require(!string.IsNullOrWhiteSpace(entry.zhHans),
                    "Missing zh-Hans text for " + entry.key);
                Require(!string.IsNullOrWhiteSpace(entry.ja),
                    "Missing ja text for " + entry.key);
                Require(!string.IsNullOrWhiteSpace(entry.en),
                    "Missing en text for " + entry.key);

                string zhPlaceholders = PlaceholderSignature(entry.zhHans);
                string jaPlaceholders = PlaceholderSignature(entry.ja);
                string enPlaceholders = PlaceholderSignature(entry.en);

                Require(zhPlaceholders == jaPlaceholders &&
                        jaPlaceholders == enPlaceholders,
                    "Placeholder mismatch for " + entry.key);

                ValidateGlyphs(chineseFont, entry.zhHans, entry.key, "zh-Hans");
                ValidateGlyphs(japaneseFont, entry.ja, entry.key, "ja");
                ValidateGlyphs(japaneseFont, entry.en, entry.key, "en");
            }
        }

        private static string PlaceholderSignature(string value)
        {
            return string.Join(
                ",",
                Regex.Matches(value, @"\{\d+\}")
                    .Cast<Match>()
                    .Select(match => match.Value)
                    .Distinct()
                    .OrderBy(placeholder => placeholder));
        }

        private static void ValidateGlyphs(
            Font font,
            string value,
            string key,
            string locale)
        {
            foreach (char character in value.Distinct())
            {
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }

                Require(font.HasCharacter(character),
                    $"Font lacks '{character}' for {locale} key {key}.");
            }
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
