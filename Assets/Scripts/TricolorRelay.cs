using BalloonBarrage.Foundation;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Three distinct balloon pigments make a keepsake, without changing score or timing.</summary>
[DisallowMultipleComponent]
public sealed class TricolorRelay : MonoBehaviour
{
    private readonly TricolorRelayProgress progress = new TricolorRelayProgress();
    private readonly Color[] collectedColors = new Color[3];
    private readonly Image[] swatches = new Image[3];
    private GameObject uiRoot;
    private Text label;
    private Text hint;
    private LocalizedText fontTemplate;
    private float celebrationRemaining;
    private RainbowChain chain;

    public int CollectedCount => progress.CollectedCount;
    public int CompletedCount => progress.CompletedCount;
    public bool IsCelebrating => celebrationRemaining > 0f;

    private void OnEnable() { Balloon.OnPigmentPopped += Collect; }
    private void OnDisable()
    {
        Balloon.OnPigmentPopped -= Collect;
        if (uiRoot != null) uiRoot.SetActive(false);
    }

    public void ResetRound()
    {
        progress.Reset();
        celebrationRemaining = 0f;
        System.Array.Clear(collectedColors, 0, collectedColors.Length);
    }

    private void Collect(int colorIndex, Color pigment)
    {
        if (!RoundController.IsPlaying || RoundController.IsPaused) return;
        int slot = progress.CollectedCount;
        int completed = progress.CompletedCount;
        if (!progress.Collect(colorIndex)) return;
        collectedColors[slot] = pigment;
        celebrationRemaining = progress.CompletedCount > completed ? 1.4f : 0f;
        if (progress.CompletedCount > completed)
        {
            chain = GetComponent<RainbowChain>();
            if (chain != null) chain.GrantCharge();
        }
    }

    private void LateUpdate()
    {
        if (!RoundController.IsPlaying || RoundController.IsPaused)
        {
            if (uiRoot != null) uiRoot.SetActive(false);
            return;
        }
        EnsureUi();
        uiRoot.SetActive(true);
        celebrationRemaining = Mathf.Max(0f, celebrationRemaining - Time.deltaTime);
        bool celebrating = IsCelebrating;
        chain = GetComponent<RainbowChain>();
        label.text = chain != null && chain.IsSelected && chain.IsFlashing
            ? LocalizationManager.Format("chain.burst", chain.LastNeighborCount)
            : celebrating
            ? LocalizationManager.Format("relay.complete", CompletedCount)
            : LocalizationManager.Format("relay.progress", CollectedCount, CompletedCount);
        hint.text = LocalizationManager.GetText(chain != null && chain.IsSelected
            ? (chain.IsReady ? "chain.ready" : "chain.hint") : "relay.hint");
        if (fontTemplate != null) label.font = hint.font = fontTemplate.CurrentFont;
        int visibleCount = celebrating ? 3 : CollectedCount;
        for (int i = 0; i < swatches.Length; i++)
        {
            swatches[i].color = i < visibleCount ? collectedColors[i] : new Color(.35f, .4f, .38f, .7f);
            // A bounded HUD pulse stays away from targets; no world flashes or new particles.
            swatches[i].transform.localScale = Vector3.one *
                (celebrating ? 1f + .14f * Mathf.Sin(celebrationRemaining * Mathf.PI * 2f) : 1f);
        }
    }

    private void EnsureUi()
    {
        if (uiRoot != null) return;
        fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
        uiRoot = new GameObject("TricolorRelayOverlay", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(CanvasGroup));
        Canvas canvas = uiRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 25;
        CanvasScaler scaler = uiRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = .5f;
        CanvasGroup group = uiRoot.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        var panel = new GameObject("RelayPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(uiRoot.transform, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(28f, 26f);
        rect.sizeDelta = new Vector2(530f, 116f);
        Image background = panel.GetComponent<Image>();
        background.color = new Color(.04f, .12f, .11f, .9f);
        background.raycastTarget = false;
        label = CreateLabel(panel.transform, "Progress", new Vector2(18f, -10f), new Vector2(494f, 34f), 24);
        hint = CreateLabel(panel.transform, "Hint", new Vector2(18f, -78f), new Vector2(494f, 28f), 18);
        for (int i = 0; i < swatches.Length; i++)
        {
            var chip = new GameObject("Pigment" + (i + 1), typeof(RectTransform), typeof(Image));
            chip.transform.SetParent(panel.transform, false);
            var chipRect = chip.GetComponent<RectTransform>();
            chipRect.anchorMin = chipRect.anchorMax = chipRect.pivot = new Vector2(0f, 1f);
            chipRect.anchoredPosition = new Vector2(18f + i * 54f, -49f);
            chipRect.sizeDelta = new Vector2(42f, 20f);
            swatches[i] = chip.GetComponent<Image>();
            swatches[i].raycastTarget = false;
        }
    }

    private Text CreateLabel(Transform parent, string name, Vector2 position, Vector2 size, int fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        Text text = go.GetComponent<Text>();
        text.font = fontTemplate != null ? fontTemplate.CurrentFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.color = new Color(1f, .97f, .86f);
        text.alignment = TextAnchor.MiddleLeft;
        text.raycastTarget = false;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 14;
        text.resizeTextMaxSize = fontSize;
        text.rectTransform.anchorMin = text.rectTransform.anchorMax = text.rectTransform.pivot = new Vector2(0f, 1f);
        text.rectTransform.anchoredPosition = position;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private void OnDestroy() { if (uiRoot != null) Destroy(uiRoot); }
}
