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
    public bool IsMirrored { get; private set; }
    public bool IsFinale { get; private set; }

    private GameObject uiRoot;
    private Text instruction;
    private readonly RectTransform[] markers = new RectTransform[3];
    private readonly Image[][] markerStrokes = new Image[3][];
    private readonly Text[] markerNumbers = new Text[3];

    private void LateUpdate()
    {
        IsDemonstrating = false;
        IsResponding = false;
        ActiveStep = -1;
        VisibleMarkerCount = 0;
        IsMirrored = false;
        IsFinale = false;
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
                double end = demonstration.answerEndSeconds > demonstration.answerStartSeconds
                    ? demonstration.answerEndSeconds
                    : demonstration.answerStartSeconds + demonstration.slots.Length * demonstration.stepSeconds;
                if (now >= demonstration.startSeconds && now < end)
                {
                    active = demonstration;
                    break;
                }
            }
        }
        if (active == null)
        {
            if (chart != null && chart.finale != null && chart.finale.eventIds != null &&
                chart.finale.eventIds.Length > 0)
            {
                RhythmEventData first = System.Array.Find(chart.events, e => e.id == chart.finale.eventIds[0]);
                if (first != null && now >= chartReader.GetSpawnTime(first))
                { ShowFinale(chart); return; }
            }
            Hide(); return;
        }

        EnsureUi();
        uiRoot.SetActive(true);
        double watchEnd = active.startSeconds + active.slots.Length * active.stepSeconds;
        IsDemonstrating = now < watchEnd;
        IsResponding = !IsDemonstrating;
        IsMirrored = active.answerSlots != null && active.answerSlots.Length == active.slots.Length;
        ActiveStep = IsDemonstrating
            ? Mathf.Clamp((int)((now - active.startSeconds) / active.stepSeconds), 0, active.slots.Length - 1)
            : -1;

        instruction.fontSize = 24;
        instruction.text = LocalizationManager.GetText(IsMirrored
            ? (IsDemonstrating ? "cue.mirror.watch" : "cue.mirror.respond")
            : (IsDemonstrating ? "cue.demonstration.watch" : "cue.demonstration.respond"));
        LocalizedText localizedTemplate = textTemplate != null ? textTemplate.GetComponent<LocalizedText>() : null;
        if (localizedTemplate != null) instruction.font = localizedTemplate.CurrentFont;

        int count = Mathf.Min(markers.Length, active.slots.Length);
        for (int index = 0; index < markers.Length; index++)
        {
            int eventId = active.answerEventIds != null && index < active.answerEventIds.Length
                ? active.answerEventIds[index] : -1;
            bool visible = index < count && (IsDemonstrating || active.keepAnswerGuide) &&
                (IsDemonstrating || eventId < 0 || !chartReader.IsResolved(eventId));
            markers[index].gameObject.SetActive(visible);
            if (!visible) continue;
            int slot = !IsDemonstrating && IsMirrored ? active.answerSlots[index] : active.slots[index];
            Vector3 world = TargetPosition(eventId, slot, index);
            Vector3 viewport = Camera.main.WorldToViewportPoint(world);
            if (viewport.z <= 0f)
            {
                markers[index].gameObject.SetActive(false);
                continue;
            }
            markers[index].anchorMin = markers[index].anchorMax = new Vector2(viewport.x, viewport.y);
            markers[index].anchoredPosition = Vector2.zero;
            bool highlighted = index == ActiveStep;
            markers[index].localScale = Vector3.one * (highlighted ? 1.15f : 1f);
            Color color = highlighted ? new Color(1f, 0.9f, 0.45f, 1f) : new Color(0.9f, 1f, 0.96f, 0.34f);
            foreach (Image stroke in markerStrokes[index]) stroke.color = color;
            markerNumbers[index].font = instruction.font;
            markerNumbers[index].color = color;
            VisibleMarkerCount++;
        }
    }

    private Vector3 TargetPosition(int eventId, int slot, int step)
    {
        foreach (Balloon balloon in balloonSpawner.ActiveBalloons)
            if (balloon != null && balloon.EventId == eventId && balloon.gameObject.activeSelf)
                return balloon.GuidePosition;
        return balloonSpawner.GetPatternPosition(BarragePattern.LateralSweep, Mathf.Clamp(slot, -2, 2), step);
    }

    private void ShowFinale(RhythmChartData chart)
    {
        EnsureUi();
        uiRoot.SetActive(true);
        IsFinale = true;
        instruction.fontSize = 30;
        LocalizedText template = textTemplate != null ? textTemplate.GetComponent<LocalizedText>() : null;
        if (template != null) instruction.font = template.CurrentFont;
        for (int index = 0; index < markers.Length; index++)
        {
            bool visible = index < chart.finale.eventIds.Length && chart.finale.slots != null &&
                index < chart.finale.slots.Length && !chartReader.IsResolved(chart.finale.eventIds[index]);
            markers[index].gameObject.SetActive(visible);
            if (!visible) continue;
            Vector3 viewport = Camera.main.WorldToViewportPoint(TargetPosition(
                chart.finale.eventIds[index], chart.finale.slots[index], index));
            if (viewport.z <= 0f) { markers[index].gameObject.SetActive(false); continue; }
            markers[index].anchorMin = markers[index].anchorMax = new Vector2(viewport.x, viewport.y);
            markers[index].anchoredPosition = Vector2.zero;
            markers[index].localScale = Vector3.one * (1.75f + .07f * Mathf.Sin(Time.unscaledTime * 3f));
            foreach (Image stroke in markerStrokes[index]) stroke.color = new Color(1f, .9f, .45f, .9f);
            markerNumbers[index].font = instruction.font;
            markerNumbers[index].color = new Color(1f, .96f, .7f, 1f);
            VisibleMarkerCount++;
        }
        instruction.text = LocalizationManager.Format("cue.finale", VisibleMarkerCount);
        if (VisibleMarkerCount == 0) { IsFinale = false; Hide(); }
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
            var number = new GameObject("Order", typeof(RectTransform), typeof(Text));
            number.transform.SetParent(marker.transform, false);
            markerNumbers[index] = number.GetComponent<Text>();
            markerNumbers[index].text = (index + 1).ToString();
            markerNumbers[index].font = instruction.font;
            markerNumbers[index].fontSize = 18;
            markerNumbers[index].alignment = TextAnchor.MiddleCenter;
            markerNumbers[index].raycastTarget = false;
            markerNumbers[index].rectTransform.sizeDelta = new Vector2(36f, 36f);
            markerNumbers[index].rectTransform.localRotation = Quaternion.Euler(0f, 0f, -45f);
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
