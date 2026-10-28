using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A brief, non-scoring visual preview on the same clock as the chart.
/// Hollow diamonds have no colliders and never change firing or hit judgement.
/// </summary>
[DisallowMultipleComponent]
public sealed class FestivalDemonstrationCue : MonoBehaviour
{
    [SerializeField] private RoundController roundController;
    [SerializeField] private RhythmChartReader chartReader;
    [SerializeField] private BalloonSpawner balloonSpawner;
    [SerializeField] private Text textTemplate;

    public bool IsDemonstrating { get; private set; }
    public bool IsResponding { get; private set; }
    public int ActiveStep { get; private set; } = -1;
    public int VisibleMarkerCount { get; private set; }

    private GameObject uiRoot;
    private RectTransform canvasRect;
    private Text instruction;
    private readonly RectTransform[] markers = new RectTransform[3];
    private readonly Image[][] markerStrokes = new Image[3][];

    private void LateUpdate()
    {
        IsDemonstrating = false;
        IsResponding = false;
        ActiveStep = -1;
        VisibleMarkerCount = 0;
        if (!RoundController.IsPlaying || RoundController.IsPaused || roundController == null ||
            chartReader == null || balloonSpawner == null || Camera.main == null)
        {
            Hide();
            return;
        }

        RhythmChartData chart = chartReader.LoadedChart;
        RhythmDemonstrationData active = null;
        double now = roundController.SongTimeSeconds;
        if (chart != null && chart.demonstrations != null)
        {
            foreach (RhythmDemonstrationData demonstration in chart.demonstrations)
            {
                if (demonstration.slots == null || demonstration.slots.Length == 0 ||
                    demonstration.stepSeconds <= 0d) continue;
                double end = demonstration.answerStartSeconds +
                    demonstration.slots.Length * demonstration.stepSeconds;
                if (now >= demonstration.startSeconds && now < end)
                {
                    active = demonstration;
                    break;
                }
            }
        }
        if (active == null) { Hide(); return; }

        EnsureUi();
        uiRoot.SetActive(true);
        double watchEnd = active.startSeconds + active.slots.Length * active.stepSeconds;
        IsDemonstrating = now < watchEnd;
        IsResponding = !IsDemonstrating;
        ActiveStep = IsDemonstrating
            ? Mathf.Clamp((int)((now - active.startSeconds) / active.stepSeconds), 0, active.slots.Length - 1)
            : -1;

        instruction.text = LocalizationManager.GetText(IsDemonstrating
            ? "cue.demonstration.watch" : "cue.demonstration.respond");
        LocalizedText localizedTemplate = textTemplate != null ? textTemplate.GetComponent<LocalizedText>() : null;
        if (localizedTemplate != null) instruction.font = localizedTemplate.CurrentFont;

        int count = Mathf.Min(markers.Length, active.slots.Length);
        for (int index = 0; index < markers.Length; index++)
        {
            bool visible = IsDemonstrating && index < count;
            markers[index].gameObject.SetActive(visible);
            if (!visible) continue;
            Vector3 world = balloonSpawner.GetPatternPosition(
                BarragePattern.LateralSweep, Mathf.Clamp(active.slots[index], -2, 2), index);
            Vector3 screen = Camera.main.WorldToScreenPoint(world);
            if (screen.z <= 0f)
            {
                markers[index].gameObject.SetActive(false);
                continue;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, null, out Vector2 local);
            markers[index].anchoredPosition = local;
            bool highlighted = index == ActiveStep;
            markers[index].localScale = Vector3.one * (highlighted ? 1.15f : 1f);
            Color color = highlighted ? new Color(1f, 0.9f, 0.45f, 1f) : new Color(0.9f, 1f, 0.96f, 0.34f);
            foreach (Image stroke in markerStrokes[index]) stroke.color = color;
            VisibleMarkerCount++;
        }
    }

    private void EnsureUi()
    {
        if (uiRoot != null) return;
        uiRoot = new GameObject("FestivalDemonstrationOverlay", typeof(RectTransform),
            typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
        Canvas canvas = uiRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        CanvasScaler scaler = uiRoot.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        CanvasGroup group = uiRoot.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
        canvasRect = uiRoot.GetComponent<RectTransform>();

        GameObject label = new GameObject("DemonstrationInstruction", typeof(RectTransform), typeof(Text), typeof(Shadow));
        label.transform.SetParent(uiRoot.transform, false);
        instruction = label.GetComponent<Text>();
        instruction.font = textTemplate != null ? textTemplate.font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instruction.fontSize = 24;
        instruction.alignment = TextAnchor.MiddleCenter;
        instruction.color = new Color(1f, 0.97f, 0.79f);
        instruction.raycastTarget = false;
        instruction.rectTransform.anchorMin = instruction.rectTransform.anchorMax = new Vector2(0.5f, 0.2f);
        instruction.rectTransform.sizeDelta = new Vector2(900f, 64f);
        label.GetComponent<Shadow>().effectColor = new Color(0.04f, 0.1f, 0.08f, 0.95f);
        label.GetComponent<Shadow>().effectDistance = new Vector2(1.5f, -1.5f);

        for (int index = 0; index < markers.Length; index++)
        {
            GameObject marker = new GameObject("PreviewDiamond" + index, typeof(RectTransform));
            marker.transform.SetParent(uiRoot.transform, false);
            markers[index] = marker.GetComponent<RectTransform>();
            markers[index].sizeDelta = new Vector2(38f, 38f);
            markers[index].localRotation = Quaternion.Euler(0f, 0f, 45f);
            markerStrokes[index] = new Image[4];
            for (int side = 0; side < 4; side++)
            {
                GameObject stroke = new GameObject("Edge", typeof(RectTransform), typeof(Image));
                stroke.transform.SetParent(marker.transform, false);
                Image image = stroke.GetComponent<Image>();
                image.raycastTarget = false;
                RectTransform rect = stroke.GetComponent<RectTransform>();
                bool horizontal = side < 2;
                rect.sizeDelta = horizontal ? new Vector2(38f, 3f) : new Vector2(3f, 38f);
                rect.anchoredPosition = horizontal
                    ? new Vector2(0f, side == 0 ? -18f : 18f)
                    : new Vector2(side == 2 ? -18f : 18f, 0f);
                markerStrokes[index][side] = image;
            }
        }
    }

    private void Hide()
    {
        if (uiRoot != null) uiRoot.SetActive(false);
    }

    private void OnDisable() { Hide(); }
    private void OnDestroy()
    {
        if (uiRoot != null) Destroy(uiRoot);
    }
}
