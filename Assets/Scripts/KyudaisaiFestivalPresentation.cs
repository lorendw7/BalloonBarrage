using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>Original presentation inspired by the 79th Kyudaisai theme. No scoring or saved unlocks.</summary>
[DisallowMultipleComponent]
public sealed class KyudaisaiFestivalPresentation : MonoBehaviour
{
    private RoundController round;
    private GameObject titleScreen, resultsScreen, titleRoot, resultsRoot, secretRoot;
    private FestivalPresentationGraphic titleArt, resultArt, secretArt;
    private LocalizedText fontTemplate;
    private Text titleCaption, resultCaption, secretTitle, secretMessage, closeLabel, badgeLabel;
    private Button badge, closeButton;
    private GameObject previousSelection;
    private CanvasGroup titleInteraction;
    private bool previousInteractable, previousRaycasts;
    private float titleAge, resultAge, secretAge, tapDeadline;
    private int taps;
    private bool wasResults;
    public bool IsSecretOpen => secretRoot != null && secretRoot.activeSelf;
    public int SecretTapCount => taps;
    public bool IsResultCelebrating => resultsRoot != null && resultsRoot.activeSelf;

    public void Configure(RoundController owner, GameObject title, GameObject results)
    {
        if (titleRoot != null || resultsRoot != null) return;
        round = owner; titleScreen = title; resultsScreen = results;
        if (titleScreen != null)
        {
            titleInteraction = titleScreen.GetComponent<CanvasGroup>();
            if (titleInteraction == null) titleInteraction = titleScreen.AddComponent<CanvasGroup>();
            fontTemplate = titleScreen.GetComponentInChildren<LocalizedText>(true);
            titleRoot = CreateLayer(titleScreen.transform, "AkatsukiTitlePresentation", out titleArt);
            // The original background remains first; controls and credits draw above decoration.
            titleRoot.transform.SetSiblingIndex(Mathf.Min(1, titleScreen.transform.childCount - 1));
            titleArt.Mode = FestivalPresentationGraphic.PresentationMode.Dawn;
            titleCaption = CreateText(titleRoot.transform, "FestivalHeading", new Vector2(.5f, .935f),
                new Vector2(620, 64), 22);
            titleCaption.color = new Color(1f, .94f, .77f);
            badge = CreateButton(titleScreen.transform, "Hidden79Badge", new Vector2(.065f, .925f),
                new Vector2(76, 62), out badgeLabel);
            badgeLabel.text = "79"; badgeLabel.fontSize = 30;
            badge.onClick.AddListener(TapBadge);
            BuildSecret();
        }
        if (resultsScreen != null)
        {
            resultsRoot = CreateLayer(resultsScreen.transform, "NineLightsCelebration", out resultArt);
            resultsRoot.transform.SetSiblingIndex(Mathf.Min(1, resultsScreen.transform.childCount - 1));
            resultArt.Mode = FestivalPresentationGraphic.PresentationMode.Lanterns;
            resultCaption = CreateText(resultsRoot.transform, "LanternThanks", new Vector2(.5f, .94f),
                new Vector2(900, 65), 23);
            resultsRoot.SetActive(false);
        }
        RefreshText();
    }

    private void OnEnable() { LanguageSettings.LanguageChanged += OnLanguageChanged; }
    private void OnDisable()
    {
        LanguageSettings.LanguageChanged -= OnLanguageChanged;
        CloseSecret();
        if (titleRoot != null) titleRoot.SetActive(false);
        if (badge != null) badge.gameObject.SetActive(false);
        if (resultsRoot != null) resultsRoot.SetActive(false);
        wasResults = false; taps = 0;
    }
    private void OnLanguageChanged(UILanguage language) { RefreshText(); }

    private void LateUpdate()
    {
        bool titleVisible = titleScreen != null && titleScreen.activeInHierarchy &&
            !RoundController.IsPlaying && round != null && !round.IsChoosingSong;
        if (titleRoot != null) titleRoot.SetActive(titleVisible);
        if (badge != null) badge.gameObject.SetActive(titleVisible);
        if (!titleVisible)
        {
            CloseSecret(); taps = 0; titleAge = 0f;
        }
        else
        {
            titleAge += Time.unscaledDeltaTime;
            titleArt.SetAge(titleAge);
            if (taps > 0 && Time.unscaledTime > tapDeadline) { taps = 0; badgeLabel.text = "79"; }
            if (IsSecretOpen)
            {
                secretAge += Time.unscaledDeltaTime;
                secretArt.SetAge(secretAge);
                if ((Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
                    (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)) CloseSecret();
            }
        }
        bool resultVisible = resultsScreen != null && resultsScreen.activeInHierarchy && !RoundController.IsPlaying;
        if (resultsRoot != null)
        {
            resultsRoot.SetActive(resultVisible);
            if (!resultVisible || !wasResults) resultAge = 0f;
            if (resultVisible) { resultAge += Time.unscaledDeltaTime; resultArt.SetAge(resultAge); }
        }
        wasResults = resultVisible;
    }

    public void TapBadge()
    {
        if (!isActiveAndEnabled || titleScreen == null || !titleScreen.activeInHierarchy ||
            RoundController.IsPlaying || round == null || round.IsChoosingSong || IsSecretOpen) return;
        if (Time.unscaledTime > tapDeadline) taps = 0;
        tapDeadline = Time.unscaledTime + 6f;
        taps++;
        badgeLabel.text = taps == 1 ? "79 ·" : taps == 2 ? "79 ··" : "79";
        if (taps < 3) return;
        taps = 0; secretAge = 0f;
        previousSelection = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        previousInteractable = titleInteraction.interactable; previousRaycasts = titleInteraction.blocksRaycasts;
        titleInteraction.interactable = false; titleInteraction.blocksRaycasts = false;
        secretRoot.SetActive(true);
        secretRoot.transform.SetAsLastSibling();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
    }

    public void CloseSecret()
    {
        if (!IsSecretOpen) return;
        secretRoot.SetActive(false);
        if (titleInteraction != null)
        { titleInteraction.interactable = previousInteractable; titleInteraction.blocksRaycasts = previousRaycasts; }
        taps = 0;
        if (badgeLabel != null) badgeLabel.text = "79";
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(previousSelection != null && previousSelection.activeInHierarchy
                ? previousSelection : null);
        previousSelection = null;
    }

    private void BuildSecret()
    {
        secretRoot = CreateLayer(titleScreen.transform, "AkatsukiHiddenConstellation", out secretArt);
        secretArt.Mode = FestivalPresentationGraphic.PresentationMode.Constellation;
        // A modal backdrop consumes clicks; the decoration itself never receives them.
        var blocker = secretRoot.AddComponent<Image>();
        blocker.color = new Color(.025f, .055f, .13f, 1f);
        blocker.raycastTarget = true;
        secretArt.transform.SetAsLastSibling();
        CanvasGroup group = secretRoot.GetComponent<CanvasGroup>();
        group.blocksRaycasts = true; group.interactable = true; group.ignoreParentGroups = true;
        secretTitle = CreateText(secretRoot.transform, "SecretTitle", new Vector2(.5f, .83f), new Vector2(800, 72), 36);
        secretMessage = CreateText(secretRoot.transform, "SecretBlessing", new Vector2(.5f, .25f), new Vector2(850, 100), 25);
        closeButton = CreateButton(secretRoot.transform, "CloseSecret", new Vector2(.5f, .115f),
            new Vector2(300, 64), out closeLabel);
        closeButton.onClick.AddListener(CloseSecret);
        secretRoot.SetActive(false);
    }

    private void RefreshText()
    {
        SetText(titleCaption, "festival.heading");
        SetText(resultCaption, "festival.thanks");
        SetText(secretTitle, "festival.secret.title");
        SetText(secretMessage, "festival.secret.message");
        SetText(closeLabel, "festival.secret.close");
        if (badgeLabel != null) badgeLabel.font = CurrentFont;
    }
    private Font CurrentFont => fontTemplate != null ? fontTemplate.CurrentFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    private void SetText(Text label, string key)
    {
        if (label == null) return;
        label.font = CurrentFont; label.text = LocalizationManager.GetText(key);
    }

    private static GameObject CreateLayer(Transform parent, string name, out FestivalPresentationGraphic art)
    {
        var layer = new GameObject(name, typeof(RectTransform), typeof(CanvasGroup));
        layer.transform.SetParent(parent, false);
        Stretch(layer.GetComponent<RectTransform>());
        var strokes = new GameObject("OriginalFestivalLightwork", typeof(RectTransform), typeof(CanvasRenderer), typeof(FestivalPresentationGraphic));
        strokes.transform.SetParent(layer.transform, false);
        art = strokes.GetComponent<FestivalPresentationGraphic>(); art.raycastTarget = false; Stretch(art.rectTransform);
        CanvasGroup group = layer.GetComponent<CanvasGroup>(); group.blocksRaycasts = false; group.interactable = false;
        return layer;
    }
    private static void Stretch(RectTransform rect)
    { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }

    private Text CreateText(Transform parent, string name, Vector2 anchor, Vector2 size, int fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(Shadow));
        go.transform.SetParent(parent, false);
        Text text = go.GetComponent<Text>(); text.font = CurrentFont; text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter; text.color = new Color(1f, .93f, .73f);
        text.raycastTarget = false; text.resizeTextForBestFit = true; text.resizeTextMinSize = 16; text.resizeTextMaxSize = fontSize;
        text.rectTransform.anchorMin = text.rectTransform.anchorMax = anchor;
        text.rectTransform.sizeDelta = size;
        go.GetComponent<Shadow>().effectColor = new Color(.025f, .05f, .09f, .9f);
        return text;
    }
    private Button CreateButton(Transform parent, string name, Vector2 anchor, Vector2 size, out Text label)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(Outline));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = anchor; rect.sizeDelta = size;
        Image image = go.GetComponent<Image>(); image.color = new Color(.08f, .14f, .23f, .95f);
        go.GetComponent<Outline>().effectColor = new Color(1f, .73f, .35f, .8f);
        Button button = go.GetComponent<Button>(); button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = colors.selectedColor = new Color(1.4f, 1.25f, 1f);
        colors.pressedColor = new Color(.8f, .7f, .6f); button.colors = colors;
        label = CreateText(go.transform, "Label", new Vector2(.5f, .5f), size - new Vector2(10f, 4f), 23);
        return button;
    }
    private void OnDestroy()
    {
        if (titleRoot != null) Destroy(titleRoot);
        if (resultsRoot != null) Destroy(resultsRoot);
        if (secretRoot != null) Destroy(secretRoot);
        if (badge != null) Destroy(badge.gameObject);
    }
}
