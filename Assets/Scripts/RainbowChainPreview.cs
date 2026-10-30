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
    private RectTransform canvasRect;
    private readonly Image[] links = new Image[2];
    private readonly Text[] badges = new Text[3];
    public int VisibleLinkCount { get; private set; }
    private LocalizedText fontTemplate;
    private readonly RectTransform[] outlines = new RectTransform[3];
    private readonly List<Balloon> neighbors = new List<Balloon>(2);
    public Balloon PrimaryTarget { get; private set; }
    public int NeighborCount => neighbors.Count;
    public int VisibleMarkerCount { get; private set; }
    private readonly Vector3[] shownPositions = new Vector3[3];
    private readonly int[] shownPoints = new int[3];
    private float burstRemaining;
    public bool IsShowingBurst => burstRemaining > 0f;

    public void ShowBurst(Vector3 primary, Vector3[] positions, int[] points, int count)
    {
        shownPositions[0] = primary;
        for (int i = 0; i < Mathf.Min(count, 2); i++)
        { shownPositions[i + 1] = positions[i]; shownPoints[i + 1] = points[i]; }
        burstRemaining = .85f;
        burstCount = Mathf.Clamp(count, 0, 2);
    }
    private int burstCount;

    public void Configure(RainbowChain owner)
    {
        chain = owner;
        shooter = FindFirstObjectByType<PlayerShooter>();
        round = FindFirstObjectByType<RoundController>();
        localPlayers = round != null ? round.GetComponent<LocalPlayerInput>() : null;
    }

    private void LateUpdate()
    {
        if (!RoundController.IsPlaying) { burstRemaining = 0f; Hide(); return; }
        if (RoundController.IsPaused) { Hide(); return; }
        if (Camera.main == null || chain == null) { burstRemaining = 0f; Hide(); return; }
        if (burstRemaining > 0f)
        {
            burstRemaining = Mathf.Max(0f, burstRemaining - Time.deltaTime);
            EnsureUi();
            label.text = burstCount > 0
                ? LocalizationManager.Format("chain.burst.result", burstCount, chain.LastExtraPoints)
                : LocalizationManager.GetText("chain.burst.empty");
            RenderPattern(burstCount + 1, true);
            return;
        }
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
        PrimaryTarget = primary;
        shownPositions[0] = primary.GuidePosition;
        for (int i = 0; i < neighbors.Count; i++)
        { shownPositions[i + 1] = neighbors[i].GuidePosition; shownPoints[i + 1] = neighbors[i].points; }
        label.text = neighbors.Count > 0
            ? LocalizationManager.Format("chain.preview.shot", neighbors.Count + 1)
            : LocalizationManager.GetText("chain.preview.empty");
        RenderPattern(neighbors.Count + 1, false);
    }

    private void RenderPattern(int count, bool burst)
    {
        root.SetActive(true);
        VisibleMarkerCount = 0;
        if (fontTemplate != null) label.font = fontTemplate.CurrentFont;
        for (int index = 0; index < outlines.Length; index++)
        {
            bool visible = index < count;
            outlines[index].gameObject.SetActive(visible);
            if (!visible) continue;
            Vector3 viewport = Camera.main.WorldToViewportPoint(shownPositions[index]);
            if (viewport.z <= 0f) { outlines[index].gameObject.SetActive(false); continue; }
            outlines[index].anchorMin = outlines[index].anchorMax = new Vector2(viewport.x, viewport.y);
            outlines[index].anchoredPosition = Vector2.zero;
            outlines[index].localScale = Vector3.one * (burst ? 1f + .3f * (1f - burstRemaining / .85f) : 1f);
            badges[index].text = index == 0 ? LocalizationManager.GetText(burst ? "chain.marker.hit" : "chain.marker.aim")
                : "+" + shownPoints[index].ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (fontTemplate != null) badges[index].font = fontTemplate.CurrentFont;
            VisibleMarkerCount++;
        }
        VisibleLinkCount = 0;
        Vector3 primaryUv = Camera.main.WorldToViewportPoint(shownPositions[0]);
        for (int index = 0; index < links.Length; index++)
        {
            bool visible = index < count - 1 && primaryUv.z > 0f;
            links[index].gameObject.SetActive(visible);
            if (!visible) continue;
            Vector3 targetUv = Camera.main.WorldToViewportPoint(shownPositions[index + 1]);
            if (targetUv.z <= 0f) { links[index].gameObject.SetActive(false); continue; }
            Vector2 delta = Vector2.Scale(new Vector2(targetUv.x - primaryUv.x, targetUv.y - primaryUv.y), canvasRect.rect.size);
            RectTransform line = links[index].rectTransform;
            line.anchorMin = line.anchorMax = new Vector2(primaryUv.x, primaryUv.y);
            line.anchoredPosition = Vector2.zero; line.pivot = new Vector2(0f, .5f);
            float travel = burst ? Mathf.Clamp01((.85f - burstRemaining) / .12f) : 1f;
            line.sizeDelta = new Vector2(delta.magnitude * travel, burst ? 9f : 4f);
            links[index].color = burst ? new Color(1f, .88f, .32f, Mathf.Clamp01(burstRemaining * 3f)) : new Color(.5f, 1f, .88f, .9f);
            line.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            VisibleLinkCount++;
        }
    }

    private void EnsureUi()
    {
        if (root != null) return;
        fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
        root = new GameObject("RainbowChainPreviewOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
            typeof(CanvasGroup), typeof(BalloonBarrage.Foundation.CanvasTextResolutionRefresh));
        canvasRect = root.GetComponent<RectTransform>();
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 26;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
        CanvasGroup group = root.GetComponent<CanvasGroup>(); group.blocksRaycasts = false; group.interactable = false;
        for (int index = 0; index < links.Length; index++)
        {
            var line = new GameObject("ChainConnection" + index, typeof(RectTransform), typeof(Image), typeof(Shadow));
            line.transform.SetParent(root.transform, false); links[index] = line.GetComponent<Image>();
            links[index].raycastTarget = false; links[index].color = new Color(.5f, 1f, .88f, .9f);
            line.GetComponent<Shadow>().effectColor = new Color(.02f, .1f, .08f, .95f);
        }
        var header = new GameObject("ChainPreviewPlate", typeof(RectTransform), typeof(Image));
        header.transform.SetParent(root.transform, false);
        header.GetComponent<Image>().color = new Color(.025f, .12f, .1f, .9f); header.GetComponent<Image>().raycastTarget = false;
        var plate = header.GetComponent<RectTransform>(); plate.anchorMin = plate.anchorMax = new Vector2(.5f, .84f); plate.sizeDelta = new Vector2(760, 66);
        var title = new GameObject("ChainPreviewLabel", typeof(RectTransform), typeof(Text), typeof(Shadow));
        title.transform.SetParent(header.transform, false);
        label = title.GetComponent<Text>();
        label.font = fontTemplate != null ? fontTemplate.CurrentFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 30; label.alignment = TextAnchor.MiddleCenter; label.raycastTarget = false;
        label.color = new Color(1f, .94f, .62f);
        label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(.5f, .5f);
        label.rectTransform.sizeDelta = new Vector2(740, 60);
        for (int index = 0; index < outlines.Length; index++)
        {
            var outline = new GameObject("ChainTarget" + index, typeof(RectTransform));
            outline.transform.SetParent(root.transform, false);
            outlines[index] = outline.GetComponent<RectTransform>();
            var badge = new GameObject("TargetNumber", typeof(RectTransform), typeof(Text), typeof(Outline));
            badge.transform.SetParent(outline.transform, false); badges[index] = badge.GetComponent<Text>();
            badges[index].font = label.font; badges[index].fontSize = 24;
            badges[index].text = index == 0 ? "1" : "+" + index;
            badges[index].alignment = TextAnchor.MiddleCenter; badges[index].color = index == 0 ? new Color(1f, .88f, .35f) : new Color(.5f, 1f, .88f);
            badges[index].raycastTarget = false; badges[index].rectTransform.anchoredPosition = new Vector2(0, 77);
            badges[index].rectTransform.sizeDelta = new Vector2(118, 34);
            badge.GetComponent<Outline>().effectColor = new Color(.02f, .1f, .08f, 1f);
            for (int side = 0; side < 4; side++)
            {
                var edge = new GameObject("Edge", typeof(RectTransform), typeof(Image), typeof(Shadow));
                edge.transform.SetParent(outline.transform, false);
                Image image = edge.GetComponent<Image>(); image.raycastTarget = false;
                image.color = index == 0 ? new Color(1f, .86f, .3f, .9f) : new Color(.5f, 1f, .88f, .7f);
                RectTransform rect = edge.GetComponent<RectTransform>();
                bool horizontal = side < 2;
                rect.sizeDelta = horizontal ? new Vector2(100, 4) : new Vector2(4, 128);
                rect.anchoredPosition = horizontal ? new Vector2(0, side == 0 ? -64 : 64) : new Vector2(side == 2 ? -50 : 50, 0);
            }
        }
    }

    private void Hide()
    {
        PrimaryTarget = null; VisibleMarkerCount = 0; VisibleLinkCount = 0; neighbors.Clear();
        if (root != null) root.SetActive(false);
    }
    private void OnDisable() { burstRemaining = 0f; Hide(); }
    private void OnDestroy() { if (root != null) Destroy(root); }
}
