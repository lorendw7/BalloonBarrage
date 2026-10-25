using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string localizationKey;

    [SerializeField]
    private Font simplifiedChineseFont;

    [SerializeField]
    private Font japaneseFont;

    [SerializeField]
    private bool useSimplifiedChineseColor;

    [SerializeField]
    private Color simplifiedChineseColor;

    private TMP_Text tmpLabel;
    private Text legacyLabel;
    private Font originalLegacyFont;
    private Color originalLegacyColor;

    private void Awake()
    {
        tmpLabel = GetComponent<TMP_Text>();
        legacyLabel = GetComponent<Text>();

        if (legacyLabel != null)
        {
            originalLegacyFont = legacyLabel.font;
            originalLegacyColor = legacyLabel.color;
        }

        if (tmpLabel == null && legacyLabel == null)
        {
            Debug.LogError(
                "LocalizedText requires TMP_Text or UI.Text.", this);
        }
    }

    private void OnEnable()
    {
        LanguageSettings.LanguageChanged += OnLanguageChanged;
        RefreshText();
    }

    private void OnDisable()
    {
        LanguageSettings.LanguageChanged -= OnLanguageChanged;
    }

    private void RefreshText()
    {
        string translatedText =
            LocalizationManager.GetText(localizationKey);

        if (tmpLabel != null)
        {
            tmpLabel.text = translatedText;
        }

        if (legacyLabel != null)
        {
            legacyLabel.text = translatedText;
            legacyLabel.font = ResolveLegacyFont();
            legacyLabel.color = ResolveLegacyColor();
        }
    }

    private void OnLanguageChanged(UILanguage language)
    {
        RefreshText();
    }

    private Font ResolveLegacyFont()
    {
        switch (LanguageSettings.Current)
        {
            case UILanguage.SimplifiedChinese:
                return simplifiedChineseFont != null
                    ? simplifiedChineseFont
                    : originalLegacyFont;

            case UILanguage.Japanese:
                return japaneseFont != null
                    ? japaneseFont
                    : originalLegacyFont;

            default:
                return originalLegacyFont;
        }
    }

    private Color ResolveLegacyColor()
    {
        if (LanguageSettings.Current == UILanguage.SimplifiedChinese &&
            useSimplifiedChineseColor)
        {
            return simplifiedChineseColor;
        }

        return originalLegacyColor;
    }
}
