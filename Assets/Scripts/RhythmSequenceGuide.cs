using UnityEngine;
using UnityEngine.UI;

/// <summary>Read-only guidance for the next three spawned, unresolved chart targets.</summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(100)]
public sealed class RhythmSequenceGuide : MonoBehaviour
{
    public const int MaximumTargets = 3;
    private const double TimingGrace = .1d;
    private const float PreviewSeconds = 2.8f;
    private readonly Balloon[] targets = new Balloon[MaximumTargets];
    private readonly Vector2[] positions = new Vector2[MaximumTargets];
    private readonly float[] radii = new float[MaximumTargets];
    private readonly Text[] numbers = new Text[MaximumTargets];
    private RoundController round;
    private BalloonSpawner spawner;
    private RhythmChartReader reader;
    private FestivalDemonstrationCue demonstration;
    private RainbowChainPreview chainPreview;
    private Camera view;
    private GameObject root;
    private RhythmSequenceGraphic graphic;
    private RectTransform canvasRect;
    private Font numberFont;
    private LocalizedText fontTemplate;
    private Text instruction;
    private int expectedEvent = -1, sequenceProgress;
    private float celebrationRemaining;
    private Vector3 celebrationWorld;
    public int CelebrationCount { get; private set; }

    private void OnEnable() { Balloon.OnDirectPop += DirectPop; }
    private void DirectPop(Balloon balloon)
    {
        if (!RoundController.IsPlaying || RoundController.IsPaused || VisibleTargetCount == 0) return;
        if (balloon.EventId != expectedEvent) { sequenceProgress = 0; return; }
        expectedEvent = -1;
        if (++sequenceProgress < 3) return;
        sequenceProgress = 0; celebrationRemaining = 1f; celebrationWorld = balloon.GuidePosition;
        CelebrationCount++;
    }
    public int VisibleTargetCount { get; private set; }
    public int VisibleLinkCount => VisibleTargetCount > 0 && graphic != null ? graphic.VisibleLinkCount : 0;
    public int FocusEventId => VisibleTargetCount > 0 ? targets[0].EventId : -1;

    private void Start()
    {
        round = GetComponent<RoundController>();
        spawner = FindFirstObjectByType<BalloonSpawner>();
        reader = FindFirstObjectByType<RhythmChartReader>();
        demonstration = FindFirstObjectByType<FestivalDemonstrationCue>();
        chainPreview = GetComponent<RainbowChainPreview>();
        view = Camera.main;
        numberFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
    }

    private void LateUpdate()
    {
        if (!RoundController.IsPlaying)
        { sequenceProgress = 0; expectedEvent = -1; celebrationRemaining = 0f; Hide(); return; }
        if (round == null || spawner == null || reader == null || view == null ||
            !RoundController.IsPlaying || RoundController.IsPaused ||
            (demonstration != null && (demonstration.IsDemonstrating || demonstration.IsResponding || demonstration.IsFinale)) ||
            (chainPreview != null && chainPreview.VisibleMarkerCount > 0))
        { Hide(); return; }

        celebrationRemaining = Mathf.Max(0f, celebrationRemaining - Time.deltaTime);
        double now = round.SongTimeSeconds;
        SelectTargets(now);
        int nextFocus = FocusEventId;
        if (expectedEvent >= 0 && expectedEvent != nextFocus) sequenceProgress = 0;
        expectedEvent = nextFocus;
        if (VisibleTargetCount == 0 && celebrationRemaining <= 0f) { Hide(); return; }
        EnsureUi();
        root.SetActive(true);
        instruction.font = fontTemplate != null ? fontTemplate.CurrentFont : numberFont;
        instruction.text = celebrationRemaining > 0f ? LocalizationManager.GetText("cue.sequence.complete")
            : LocalizationManager.Format("cue.sequence.hint", sequenceProgress);
        for (int i = 0; i < MaximumTargets; i++)
        {
            numbers[i].gameObject.SetActive(i < VisibleTargetCount);
            if (i >= VisibleTargetCount) continue;
            Vector3 world = targets[i].GuidePosition;
            Vector3 uv = view.WorldToViewportPoint(world);
            positions[i] = Vector2.Scale(new Vector2(uv.x - .5f, uv.y - .5f), canvasRect.rect.size);
            // Project the actual balloon body size, including entrance and depth changes.
            StudioBalloonAppearance appearance = targets[i].GetComponent<StudioBalloonAppearance>();
            float bodyRadius = appearance != null && appearance.latex != null
                ? appearance.latex.bounds.extents.magnitude : .42f * targets[i].transform.lossyScale.x;
            Vector3 side = view.WorldToViewportPoint(world + view.transform.right * bodyRadius);
            radii[i] = Mathf.Clamp(Mathf.Abs(side.x - uv.x) * canvasRect.rect.width + 12f, 30f, 115f);
            numbers[i].rectTransform.anchoredPosition = positions[i] + Vector2.up * (radii[i] + 24f);
            numbers[i].text = (sequenceProgress + i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
            numbers[i].color = i == 0 ? RhythmSequenceGraphic.FocusColor : RhythmSequenceGraphic.FollowColor;
        }
        float approach = VisibleTargetCount > 0 ? Mathf.Clamp01(1f - (float)(targets[0].EventTimeSeconds - now) / PreviewSeconds) : 0f;
        double interval = VisibleTargetCount > 1 ? targets[1].EventTimeSeconds - targets[0].EventTimeSeconds : 1d;
        // Directional flow uses the local chart interval, never an unrelated wall clock.
        float flow = VisibleTargetCount > 0 ? Mathf.Repeat((float)((now - targets[0].EventTimeSeconds) / System.Math.Max(.25d, interval)), 1f) : 0f;
        graphic.SetFrame(positions, radii, VisibleTargetCount, approach, flow);
        Vector3 celebrationUv = view.WorldToViewportPoint(celebrationWorld);
        graphic.SetCelebration(Vector2.Scale(new Vector2(celebrationUv.x - .5f, celebrationUv.y - .5f), canvasRect.rect.size),
            celebrationUv.z > 0f ? celebrationRemaining : 0f);
    }

    private void SelectTargets(double now)
    {
        System.Array.Clear(targets, 0, targets.Length);
        VisibleTargetCount = 0;
        int capacity = MaximumTargets - sequenceProgress;
        foreach (Balloon candidate in spawner.ActiveBalloons)
        {
            if (candidate == null || !candidate.CanReceiveChain(now) || reader.IsResolved(candidate.EventId) ||
                candidate.EventTimeSeconds < now - TimingGrace) continue;
            Vector3 uv = view.WorldToViewportPoint(candidate.GuidePosition);
            if (uv.z <= 0f || uv.x < .025f || uv.x > .975f || uv.y < .04f || uv.y > .96f) continue;
            int at = 0;
            while (at < VisibleTargetCount && Compare(targets[at], candidate) <= 0) at++;
            if (at >= capacity) continue;
            for (int j = Mathf.Min(VisibleTargetCount, capacity - 1); j > at; j--) targets[j] = targets[j - 1];
            targets[at] = candidate;
            VisibleTargetCount = Mathf.Min(VisibleTargetCount + 1, capacity);
        }
    }

    private static int Compare(Balloon a, Balloon b)
    {
        int time = a.EventTimeSeconds.CompareTo(b.EventTimeSeconds);
        return time != 0 ? time : a.EventId.CompareTo(b.EventId);
    }

    private void EnsureUi()
    {
        if (root != null) return;
        root = new GameObject("RhythmSequenceOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
            typeof(CanvasGroup), typeof(BalloonBarrage.Foundation.CanvasTextResolutionRefresh));
        canvasRect = root.GetComponent<RectTransform>();
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 24;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = .5f;
        CanvasGroup group = root.GetComponent<CanvasGroup>();
        group.blocksRaycasts = false; group.interactable = false;
        var strokes = new GameObject("SequenceLightAndShadow", typeof(RectTransform), typeof(CanvasRenderer), typeof(RhythmSequenceGraphic));
        strokes.transform.SetParent(root.transform, false);
        graphic = strokes.GetComponent<RhythmSequenceGraphic>();
        graphic.raycastTarget = false;
        graphic.rectTransform.anchorMin = Vector2.zero; graphic.rectTransform.anchorMax = Vector2.one;
        graphic.rectTransform.sizeDelta = Vector2.zero;
        for (int i = 0; i < MaximumTargets; i++)
        {
            var badge = new GameObject("SequenceNumber" + (i + 1), typeof(RectTransform), typeof(Text), typeof(Outline));
            badge.transform.SetParent(root.transform, false);
            numbers[i] = badge.GetComponent<Text>();
            numbers[i].font = numberFont; numbers[i].fontSize = 28; numbers[i].fontStyle = FontStyle.Bold;
            numbers[i].text = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
            numbers[i].alignment = TextAnchor.MiddleCenter; numbers[i].raycastTarget = false;
            numbers[i].rectTransform.sizeDelta = new Vector2(48, 40);
            Outline outline = badge.GetComponent<Outline>();
            outline.effectColor = new Color(.035f, .075f, .08f, .95f);
            outline.effectDistance = new Vector2(2, -2);
        }
        var plate = new GameObject("SequenceMeaning", typeof(RectTransform), typeof(Image));
        plate.transform.SetParent(root.transform, false);
        var rect = plate.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.5f, .2f);
        rect.sizeDelta = new Vector2(700, 66);
        plate.GetComponent<Image>().color = new Color(.035f, .09f, .12f, .86f); plate.GetComponent<Image>().raycastTarget = false;
        var label = new GameObject("SequenceInstruction", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(plate.transform, false); instruction = label.GetComponent<Text>();
        instruction.fontSize = 22; instruction.alignment = TextAnchor.MiddleCenter;
        instruction.color = new Color(1f, .94f, .75f); instruction.raycastTarget = false;
        instruction.rectTransform.sizeDelta = new Vector2(680, 62);
    }

    private void Hide()
    {
        VisibleTargetCount = 0;
        System.Array.Clear(targets, 0, targets.Length);
        if (root != null) root.SetActive(false);
    }
    private void OnDisable()
    { Balloon.OnDirectPop -= DirectPop; sequenceProgress = 0; expectedEvent = -1; celebrationRemaining = 0f; Hide(); }
    private void OnDestroy() { if (root != null) Destroy(root); }
}
