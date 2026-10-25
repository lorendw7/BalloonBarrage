using System;
using UnityEngine;


[Serializable]
public class LocalizationEntry
{
    public string key;
    public string zhHans;
    public string ja;
    public string en;

    public string GetText(UILanguage language)
    {
        switch (language)
        {
            case UILanguage.SimplifiedChinese:
                return zhHans;

            case UILanguage.English:
                return en;

            default:
                return ja;
        }
    }
}

[Serializable]

public class LocalizationTable
{
    public string[] locales;
    public LocalizationEntry[] entries;
    
}
