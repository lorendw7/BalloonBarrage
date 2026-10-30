using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Read-only snapshot of the exact bounded planner used by the next burst.</summary>
[DisallowMultipleComponent]
public sealed class RainbowChainPreview : MonoBehaviour
{
    private RainbowChain chain;
    private PlayerShooter shooter;
    private RoundController round;
    private LocalPlayerInput localPlayers;
    private GameObject root;
    private Text label;
    private LocalizedText fontTemplate;
    private readonly RectTransform[] outlines = new RectTransform[3];
    private readonly List<Balloon> neighbors = new List<Balloon>(2);
    public Balloon PrimaryTarget { get; private set; }
    public int NeighborCount => neighbors.Count;
    public int VisibleMarkerCount { get; private set; }

    public void Configure(RainbowChain owner)
    {
        chain = owner;
        shooter = FindFirstObjectByType<PlayerShooter>();
        round = FindFirstObjectByType<RoundController>();
        localPlayers = round != null ? round.GetComponent<LocalPlayerInput>() : null;
    }

    private void LateUpdate()
    {
        if (localPlayers != null) shooter = localPlayers.LastAimingPlayer == 1 ? localPlayers.Secondary : localPlayers.Primary;
        if (chain == null || shooter == null || !chain.IsSelected || !chain.IsReady ||
            !RoundController.IsPlaying || RoundController.IsPaused || !shooter.TryGetAimedBalloon(out Balloon target))
        { Hide(); return; }
        Show(target);
    }

    private void Show(Balloon primary)
    {
        if (Camera.main == null || chain == null || !chain.IsReady || !RoundController.IsPlaying || RoundController.IsPaused)
        { Hide(); return; }
        chain.GetPreviewTargets(primary, neighbors);
        if (primary == null || round == null || !primary.CanReceiveChain(round.SongTimeSeconds))
        { Hide(); return; }
        EnsureUi();
        root.SetActive(true);
        PrimaryTarget = primary;
        VisibleMarkerCount = 0;
        label.text = LocalizationManager.Format("chain.preview", neighbors.Count);
        if (fontTemplate != null) label.font = fontTemplate.CurrentFont;
        for (int index = 0; index < outlines.Length; index++)
        {
            Balloon target = index == 0 ? primary : index <= neighbors.Count ? neighbors[index - 1] : null;
            bool visible = target != null && target.gameObject.activeSelf;
            outlines[index].gameObject.SetActive(visible);
            if (!visible) continue;
            Vector3 viewport = Camera.main.WorldToViewportPoint(target.GuidePosition);
            if (viewport.z <= 0f) { outlines[index].gameObject.SetActive(false); continue; }
            outlines[index].anchorMin = outlines[index].anchorMax = new Vector2(viewport.x, viewport.y);
            outlines[index].anchoredPosition = Vector2.zero;
            VisibleMarkerCount++;
        }
    }

    private void EnsureUi()
    {
        if (root != null) return;
        fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
        root = new GameObject("RainbowChainPreviewOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 26;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
        CanvasGroup group = root.GetComponent<CanvasGroup>(); group.blocksRaycasts = false; group.interactable = false;
        var title = new GameObject("ChainPreviewLabel", typeof(RectTransform), typeof(Text), typeof(Shadow));
        title.transform.SetParent(root.transform, false);
        label = title.GetComponent<Text>();
        label.font = fontTemplate != null ? fontTemplate.CurrentFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 24; label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
        label.color = new Color(1f, .94f, .62f);
        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(.5f, .84f);
        label.rectTransform.sizeDelta = new Vector2(760, 48);
        for (int index = 0; index < outlines.Length; index++)
        {
            var outline = new GameObject("ChainTarget" + index, typeof(RectTransform));
            outline.transform.SetParent(root.transform, false);
            outlines[index] = outline.GetComponent<RectTransform>();
            for (int side = 0; side < 4; side++)
            {
                var edge = new GameObject("Edge", typeof(RectTransform), typeof(Image));
                edge.transform.SetParent(outline.transform, false);
                Image image = edge.GetComponent<Image>(); image.raycastTarget = false;
                image.color = index == 0 ? new Color(1f, .86f, .3f, .9f) : new Color(.5f, 1f, .88f, .7f);
                RectTransform rect = edge.GetComponent<RectTransform>();
                bool horizontal = side < 2;
                rect.sizeDelta = horizontal ? new Vector2(88, 2) : new Vector2(2, 110);
                rect.anchoredPosition = horizontal ? new Vector2(0, side == 0 ? -55 : 55) : new Vector2(side == 2 ? -44 : 44, 0);
            }
        }
    }

    private void Hide()
    {
        PrimaryTarget = null; VisibleMarkerCount = 0; neighbors.Clear();
        if (root != null) root.SetActive(false);
    }
    private void OnDisable() { Hide(); }
    private void OnDestroy() { if (root != null) Destroy(root); }
}
