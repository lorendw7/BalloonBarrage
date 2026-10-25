using System;
using UnityEngine;

public enum UILanguage
{
    SimplifiedChinese,
    Japanese,
    English
}


public static class LanguageSettings
{
    private const string LanguageKey = "ui.language.v1";

    public static event Action<UILanguage> LanguageChanged;

    public static UILanguage Current { get; private set; } = LoadLanguage();

    public static string CurrentCode =>
    GetCode(Current);

    public static void SetLanguage(UILanguage language)
    {
        if (Current == language)
        {
            return;
        }

        Current = language;

        PlayerPrefs.SetString(
            LanguageKey, CurrentCode);

        PlayerPrefs.Save();

        LanguageChanged?.Invoke(Current);
    }

    private static UILanguage LoadLanguage()
    {
        string savedCode = PlayerPrefs.GetString(
            LanguageKey, "ja");

        switch (savedCode)
        {
            case "zh-Hans":
                return UILanguage.SimplifiedChinese;
            case "en":
                return UILanguage.English;

            default:
                return UILanguage.Japanese;
        }
    }

    private static string GetCode(UILanguage language)
    {
        switch (language)
        {
            case UILanguage.SimplifiedChinese:
                return "zh-Hans";
            case UILanguage.English:
                return "en";
            default:
                return "ja";
        }
    }
}
