using UnityEngine;
using UnityEngine.UI;

/// <summary>Local, cosmetic audience reactions. Reads completed successes; never changes gameplay.</summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(200)]
public sealed class AudienceCelebration : MonoBehaviour
{
    public const int MaximumReactions = 8;
    private const float AudioCooldown = 1.6f;
    private static readonly string[] EmojiNames = { "emoji_u1f44d", "emoji_u1f44f", "emoji_u1f389", "emoji_u1f929" };
    private sealed class Reaction
    {
        public RectTransform rect;
        public CanvasGroup group;
        public RawImage emoji;
        public Text text;
        public float age, duration;
        public int side, lane;
        public bool active;
    }

    private readonly Reaction[] reactions = new Reaction[MaximumReactions];
    private readonly Texture2D[] emojiTextures = new Texture2D[4];
    private RhythmSequenceGuide sequence;
    private TricolorRelay relay;
    private RainbowChain chain;
    private LocalizedText fontTemplate;
    private Font fallbackFont;
    private GameObject root;
    private RectTransform canvasRect;
    private AudioSource cheer;
    private AudioClip yay, crowd;
    private int lastSequence, lastRelay, lastChain, nextSlot;
    private float cooldown, soundRemaining, soundDuration;
    private bool hasFocus = true;
    [SerializeField, Range(0f, 1f)] private float cheerVolume = .48f;

    public int VisibleReactionCount { get; private set; }
    public int ShownCelebrationCount { get; private set; }
    public int CheerStartCount { get; private set; }

    private void OnEnable() { CacheSources(); Snapshot(); }
    private void CacheSources()
    {
        if (sequence == null) sequence = GetComponent<RhythmSequenceGuide>();
        if (relay == null) relay = GetComponent<TricolorRelay>();
        if (chain == null) chain = GetComponent<RainbowChain>();
    }
    private void Snapshot()
    {
        lastSequence = sequence != null ? sequence.CelebrationCount : 0;
        lastRelay = relay != null ? relay.CompletedCount : 0;
        lastChain = chain != null ? chain.BurstCount : 0;
    }

    private void LateUpdate()
    {
        CacheSources();
        if (!RoundController.IsPlaying || RoundController.IsPaused || !hasFocus)
        { Clear(); Snapshot(); return; }

        bool ordered = sequence != null && sequence.CelebrationCount > lastSequence;
        bool collected = relay != null && relay.CompletedCount > lastRelay;
        bool linked = chain != null && chain.BurstCount > lastChain && chain.LastNeighborCount > 0;
        Snapshot();
        // A single hit can finish several successes. Combine them into one bounded burst.
        if (linked) Celebrate("audience.chain", true, chain.LastNeighborCount, chain.LastExtraPoints);
        else if (ordered) Celebrate("audience.sequence", false, 0, 0);
        else if (collected) Celebrate("audience.relay", false, 0, 0);
        Animate(Time.deltaTime);
    }

    private void Celebrate(string key, bool big, int neighbors, int points)
    {
        EnsureUi();
        root.SetActive(true);
        ShownCelebrationCount++;
        // Refresh the applause together: a second success must not stack unreadable rows.
        foreach (Reaction item in reactions)
        { item.active = false; item.rect.gameObject.SetActive(false); }
        for (int pair = 0; pair < 4; pair++)
        {
            for (int side = 0; side < 2; side++)
            {
                Reaction item = reactions[nextSlot];
                nextSlot = (nextSlot + 1) % MaximumReactions;
                item.side = side; item.lane = pair;
                item.age = -pair * .14f;
                item.duration = 2.2f + pair * .12f;
                item.active = true;
                item.emoji.texture = emojiTextures[(pair + side) % emojiTextures.Length];
                item.text.font = fontTemplate != null ? fontTemplate.CurrentFont : fallbackFont;
                item.text.text = pair == 0 ? LocalizationManager.Format(key, neighbors, points)
                    : LocalizationManager.GetText(pair == 1 ? "audience.like" : pair == 2 ? "audience.cheer" : "audience.encore");
                item.group.alpha = 0f;
                item.rect.gameObject.SetActive(true);
            }
        }
        if (cooldown > 0f || cheer == null) return;
        cheer.Stop();
        cheer.clip = big ? crowd : yay;
        if (cheer.clip == null) return;
        soundDuration = Mathf.Min(big ? 2.6f : 1.8f, cheer.clip.length);
        soundRemaining = soundDuration;
        cheer.volume = cheerVolume;
        cheer.Play(); // One 2D voice, subject to the normal AudioListener master volume/pause.
        CheerStartCount++;
        cooldown = Mathf.Max(AudioCooldown, soundDuration + .3f);
    }

    private void Animate(float delta)
    {
        cooldown = Mathf.Max(0f, cooldown - delta);
        if (cheer != null && soundRemaining > 0f)
        {
            soundRemaining = Mathf.Max(0f, soundRemaining - delta);
            cheer.volume = cheerVolume * Mathf.Clamp01(soundRemaining / .35f);
            if (soundRemaining <= 0f) cheer.Stop();
        }
        VisibleReactionCount = 0;
        if (root == null) return;
        float width = Mathf.Min(290f, canvasRect.rect.width * .18f);
        for (int i = 0; i < reactions.Length; i++)
        {
            Reaction item = reactions[i];
            if (!item.active) continue;
            item.age += delta;
            if (item.age >= item.duration)
            { item.active = false; item.rect.gameObject.SetActive(false); continue; }
            if (item.age < 0f) continue;
            VisibleReactionCount++;
            float progress = item.age / item.duration;
            item.group.alpha = Mathf.Min(Mathf.Clamp01(item.age / .14f), Mathf.Clamp01((item.duration - item.age) / .45f));
            item.rect.anchorMin = item.rect.anchorMax = item.rect.pivot = new Vector2(item.side, 0f);
            item.rect.sizeDelta = new Vector2(width, 78f);
            float inset = 14f + 7f * Mathf.Sin(progress * Mathf.PI);
            item.rect.anchoredPosition = new Vector2(item.side == 0 ? inset : -inset,
                canvasRect.rect.height * .25f + item.lane * 86f + progress * canvasRect.rect.height * .19f);
            item.rect.localScale = Vector3.one * (1f + .10f * Mathf.Sin(Mathf.Clamp01(item.age / .3f) * Mathf.PI));
            float iconSize = Mathf.Min(60f, width * .7f);
            item.emoji.rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            item.text.gameObject.SetActive(width >= 150f);
        }
        if (VisibleReactionCount == 0 && soundRemaining <= 0f)
        {
            bool pending = false;
            foreach (Reaction item in reactions) pending |= item.active;
            if (!pending) root.SetActive(false);
        }
    }

    private void EnsureUi()
    {
        if (root != null) return;
        fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
        fallbackFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        for (int i = 0; i < emojiTextures.Length; i++) emojiTextures[i] = Resources.Load<Texture2D>("Audience/" + EmojiNames[i]);
        yay = Resources.Load<AudioClip>("Audience/CheerYay");
        crowd = Resources.Load<AudioClip>("Audience/CheerCrowd");
        cheer = gameObject.AddComponent<AudioSource>();
        cheer.playOnAwake = false; cheer.loop = false; cheer.spatialBlend = 0f;
        cheer.priority = 160; cheer.ignoreListenerPause = false;
        root = new GameObject("AudienceCelebrationOverlay", typeof(RectTransform), typeof(Canvas),
            typeof(CanvasScaler), typeof(CanvasGroup), typeof(BalloonBarrage.Foundation.CanvasTextResolutionRefresh));
        canvasRect = root.GetComponent<RectTransform>();
        Canvas canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 27;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
        CanvasGroup group = root.GetComponent<CanvasGroup>(); group.blocksRaycasts = false; group.interactable = false;
        for (int i = 0; i < reactions.Length; i++)
        {
            var row = new GameObject("AudienceReaction" + i, typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            row.transform.SetParent(root.transform, false);
            Image plate = row.GetComponent<Image>(); plate.color = new Color(.025f, .075f, .12f, .84f); plate.raycastTarget = false;
            Reaction item = new Reaction { rect = row.GetComponent<RectTransform>(), group = row.GetComponent<CanvasGroup>() };
            var icon = new GameObject("PublicEmoji", typeof(RectTransform), typeof(RawImage)); icon.transform.SetParent(row.transform, false);
            item.emoji = icon.GetComponent<RawImage>(); item.emoji.raycastTarget = false;
            item.emoji.rectTransform.anchorMin = item.emoji.rectTransform.anchorMax = new Vector2(0f, .5f);
            item.emoji.rectTransform.pivot = new Vector2(0f, .5f); item.emoji.rectTransform.anchoredPosition = new Vector2(8f, 0f);
            var label = new GameObject("AudienceMessage", typeof(RectTransform), typeof(Text)); label.transform.SetParent(row.transform, false);
            item.text = label.GetComponent<Text>(); item.text.raycastTarget = false;
            item.text.color = new Color(1f, .95f, .75f); item.text.fontSize = 22; item.text.fontStyle = FontStyle.Bold;
            item.text.alignment = TextAnchor.MiddleLeft; item.text.resizeTextForBestFit = true;
            item.text.resizeTextMinSize = 14; item.text.resizeTextMaxSize = 22;
            item.text.rectTransform.anchorMin = Vector2.zero; item.text.rectTransform.anchorMax = Vector2.one;
            item.text.rectTransform.offsetMin = new Vector2(76f, 4f); item.text.rectTransform.offsetMax = new Vector2(-8f, -4f);
            reactions[i] = item; row.SetActive(false);
        }
        var credit = new GameObject("CelebrationAssetCredit", typeof(RectTransform), typeof(Text)); credit.transform.SetParent(root.transform, false);
        Text attribution = credit.GetComponent<Text>(); attribution.font = fallbackFont; attribution.fontSize = 14;
        attribution.text = "Cheer: OtoLogic (CC BY 4.0)  |  Emoji: Noto (Apache 2.0)";
        attribution.alignment = TextAnchor.MiddleCenter; attribution.color = new Color(1f, .97f, .88f, .85f); attribution.raycastTarget = false;
        attribution.rectTransform.anchorMin = attribution.rectTransform.anchorMax = new Vector2(.5f, 0f);
        attribution.rectTransform.pivot = new Vector2(.5f, 0f); attribution.rectTransform.anchoredPosition = new Vector2(0f, 5f);
        attribution.rectTransform.sizeDelta = new Vector2(650f, 24f);
        Shadow creditShadow = credit.AddComponent<Shadow>();
        creditShadow.effectColor = new Color(.02f, .035f, .05f, 1f); creditShadow.effectDistance = new Vector2(1f, -1f);
    }

    private void Clear()
    {
        if (root != null) root.SetActive(false);
        foreach (Reaction item in reactions)
            if (item != null) { item.active = false; item.rect.gameObject.SetActive(false); }
        if (cheer != null) cheer.Stop();
        soundRemaining = 0f; cooldown = 0f; VisibleReactionCount = 0;
    }
    private void OnApplicationFocus(bool focused) { hasFocus = focused; if (!focused) { Clear(); Snapshot(); } }
    private void OnDisable() { Clear(); Snapshot(); }
    private void OnDestroy()
    {
        if (root != null) Destroy(root);
        if (cheer != null) Destroy(cheer);
    }
}
