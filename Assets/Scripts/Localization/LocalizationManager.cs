using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LocalizationManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset stringTableAsset;

    private static readonly Dictionary<string, LocalizationEntry>
        Entries =
            new Dictionary<string, LocalizationEntry>();

    public static bool IsReady { get; private set; }

    public static string GetText(string key)
    {
        if (!IsReady)
        {
            return key;
        }

        if (!Entries.TryGetValue(key, out LocalizationEntry entry))
        {
            Debug.LogWarning("Missing localization key: " + key);
            return key;
        }

        return entry.GetText(LanguageSettings.Current);
    }

    public static string Format(string key, params object[] arguments)
    {
        string format = GetText(key);

        try
        {
            return string.Format(format, arguments);
        }
        catch (System.FormatException)
        {
            Debug.LogWarning("Invalid localization format: " + key);
            return format;
        }
    }

    private void Awake()
    {
        LoadTable();
    }

    private void LoadTable()
    {
        IsReady = false;

        if (stringTableAsset == null)
        {
            Debug.LogError("Localization string table is not assigned.");
            return;
        }

        LocalizationTable table = JsonUtility.FromJson<LocalizationTable>(stringTableAsset.text);

        if (table == null) {
            Debug.LogError("Localization string table could not be parsed.");
            return;
        }

        if (table.entries == null || table.entries.Length == 0)
        {
            Debug.LogError("Localization string table has no entries.");
            return;
        }

        Entries.Clear();
        foreach (LocalizationEntry entry in table.entries)
        {
            Entries[entry.key] = entry;
        }

        if (Entries.Count != table.entries.Length)
        {
            Debug.LogError("Localization string table contains duplicate keys.");
            Entries.Clear();
            return;
        }

        IsReady  = true;
    }
}
