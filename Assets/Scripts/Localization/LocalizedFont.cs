using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LocalizedFont : MonoBehaviour
{
    [SerializeField]
    private Font simplifiedChineseFont;

    [SerializeField]
    private Font japaneseFont;

    private Text legacyLabel;
    private Font originalLegacyFont;

    private void Awake()
    {
        legacyLabel = GetComponent<Text>();

        if (legacyLabel != null)
        {
            originalLegacyFont = legacyLabel.font;
        }
        else
        {
            Debug.LogError("LocalizedFont requires UI.Text.", this);
        }
    }

    private void OnEnable()
    {
        LanguageSettings.LanguageChanged += OnLanguageChanged;
        ApplyFont(LanguageSettings.Current);
    }

    private void OnDisable()
    {
        LanguageSettings.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(UILanguage language)
    {
        ApplyFont(language);
    }

    private void ApplyFont(UILanguage language)
    {
        if (legacyLabel == null)
        {
            return;
        }

        switch (language)
        {
            case UILanguage.SimplifiedChinese:
                legacyLabel.font = simplifiedChineseFont != null
                    ? simplifiedChineseFont
                    : originalLegacyFont;
                break;

            case UILanguage.Japanese:
                legacyLabel.font = japaneseFont != null
                    ? japaneseFont
                    : originalLegacyFont;
                break;

            default:
                legacyLabel.font = originalLegacyFont;
                break;
        }
    }
}
