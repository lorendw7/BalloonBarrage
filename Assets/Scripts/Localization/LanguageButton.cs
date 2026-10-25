using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    [SerializeField]
    private UILanguage language;

    public void SelectLanguage()
    {
        LanguageSettings.SetLanguage(language);
    }
}