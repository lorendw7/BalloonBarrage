using UnityEngine;
using UnityEngine.UI;

/// <summary>Cosmetic pulses and a bounded pool of three shadow-free colored lights.</summary>
public sealed class PopLightFeedback : MonoBehaviour
{
    public static PopLightFeedback Instance { get; private set; }
    public const int MaximumPulses = 12, MaximumLights = 3;
    public int TotalPulseCount { get; private set; }
    public int FinalePulseCount { get; private set; }
    public int ActiveLightCount { get; private set; }
    public int VisiblePetalCount { get; private set; }
    public float FinaleAge { get; private set; } = 10f;
    private sealed class Pulse
    {
        public RectTransform root;
        public Image halo, ring, core;
        public readonly Image[] petals = new Image[12];
        public Vector3 world;
        public Color pigment;
        public float age = 10f, duration, strength;
        public bool finale;
    }
    private readonly Pulse[] pulses = new Pulse[MaximumPulses];
    private readonly Light[] lights = new Light[MaximumLights];
    private readonly float[] lightAges = new float[MaximumLights], lightDurations = new float[MaximumLights];
    private readonly Color[] lightColors = new Color[MaximumLights];
    private readonly bool[] finaleLights = new bool[MaximumLights];
    private readonly Texture2D[] textures = new Texture2D[3];
    private readonly Sprite[] sprites = new Sprite[3];
    private GameObject canvasRoot;
    private Text caption;
    private LocalizedText fontTemplate;
    private int nextPulse;
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < MaximumLights; i++)
        {
            var go = new GameObject("PaintPopLight" + i); go.transform.SetParent(transform, false);
            lights[i] = go.AddComponent<Light>(); lights[i].type = LightType.Point;
            lights[i].shadows = LightShadows.None; lights[i].enabled = false; lightAges[i] = 10f;
        }
    }
    public void Show(Vector3 world, Color pigment, bool onBeat, bool finale)
    {
        if (!RoundController.IsPlaying || RoundController.IsPaused || Camera.main == null) return;
        EnsureUi();
        for (int tries = 0; tries < MaximumPulses && pulses[nextPulse].finale && pulses[nextPulse].age < pulses[nextPulse].duration; tries++)
            nextPulse = (nextPulse + 1) % MaximumPulses;
        Pulse p = pulses[nextPulse]; nextPulse = (nextPulse + 1) % MaximumPulses;
        p.world = world; p.pigment = pigment; p.age = 0f; p.finale = finale;
        p.duration = finale ? 1.5f : onBeat ? .65f : .48f;
        p.strength = finale ? 2.3f : onBeat ? 1.2f : 1f;
        TotalPulseCount++;
        if (finale) { FinalePulseCount++; FinaleAge = 0f; }
        int selected = -1; float oldest = -1f;
        for (int i = 0; i < MaximumLights; i++)
        {
            if (finaleLights[i] && lightAges[i] < lightDurations[i] && !finale) continue;
            if (lightAges[i] > oldest) { oldest = lightAges[i]; selected = i; }
        }
        if (selected >= 0)
        {
            lights[selected].transform.position = world - Camera.main.transform.forward * .7f;
            lightAges[selected] = 0f; lightDurations[selected] = finale ? 1.5f : .3f;
            lightColors[selected] = pigment; finaleLights[selected] = finale;
            lights[selected].range = finale ? 7f : 3.5f;
        }
        Reproject();
    }
    private void LateUpdate()
    {
        if (RoundController.IsPaused)
        {
            if (canvasRoot != null) canvasRoot.SetActive(false);
            foreach (Light light in lights) if (light != null) light.enabled = false;
            ActiveLightCount = 0; VisiblePetalCount = 0; return;
        }
        float delta = Time.deltaTime; FinaleAge += delta;
        foreach (Pulse p in pulses) if (p != null) p.age += delta;
        ActiveLightCount = 0;
        for (int i = 0; i < MaximumLights; i++)
        {
            lightAges[i] += delta; bool active = lightAges[i] < lightDurations[i]; lights[i].enabled = active;
            if (!active) continue;
            float t = lightAges[i] / lightDurations[i];
            lights[i].color = finaleLights[i] ? Color.HSVToRGB(Mathf.Repeat(t * 1.7f + .05f, 1f), .65f, 1f) : lightColors[i];
            lights[i].intensity = (finaleLights[i] ? 5f : 3f) * (1f - t) * (1f - t); ActiveLightCount++;
        }
        Reproject();
    }
    public void Reproject()
    {
        if (canvasRoot == null || Camera.main == null) return;
        bool any = false; VisiblePetalCount = 0;
        foreach (Pulse p in pulses)
        {
            bool visible = p.age < p.duration && !RoundController.IsPaused; p.root.gameObject.SetActive(visible);
            if (!visible) continue;
            Vector3 uv = Camera.main.WorldToViewportPoint(p.world);
            if (uv.z <= 0f) { p.root.gameObject.SetActive(false); continue; }
            any = true; p.root.anchorMin = p.root.anchorMax = new Vector2(uv.x, uv.y); p.root.anchoredPosition = Vector2.zero;
            float t = Mathf.Clamp01(p.age / p.duration), fade = (1f - t) * (1f - t);
            float size = p.strength * Mathf.Lerp(90f, 205f, Mathf.Sqrt(t));
            p.halo.rectTransform.sizeDelta = Vector2.one * size; p.ring.rectTransform.sizeDelta = Vector2.one * size * 1.15f;
            p.core.rectTransform.sizeDelta = Vector2.one * Mathf.Lerp(36f, 10f, t) * p.strength;
            p.halo.color = new Color(p.pigment.r, p.pigment.g, p.pigment.b, .78f * fade);
            p.ring.color = Color.Lerp(p.pigment, Color.white, .4f) * new Color(1, 1, 1, .95f * fade);
            p.core.color = new Color(1f, .98f, .88f, .85f * fade);
            for (int k = 0; k < 12; k++)
            {
                Image petal = p.petals[k]; petal.gameObject.SetActive(p.finale); if (!p.finale) continue;
                petal.rectTransform.sizeDelta = new Vector2(42f * (1f - .4f * t), Mathf.Lerp(65f, 205f, Mathf.Sqrt(t)));
                petal.rectTransform.localRotation = Quaternion.Euler(0, 0, k * 30f + t * 20f);
                Color color = Color.HSVToRGB(k / 12f, .75f, 1f); color.a = .85f * (1f - t); petal.color = color; VisiblePetalCount++;
            }
        }
        bool finaleVisible = FinaleAge < 1.5f && !RoundController.IsPaused; caption.gameObject.SetActive(finaleVisible);
        if (finaleVisible)
        {
            caption.text = LocalizationManager.GetText("cue.finale.burst");
            if (fontTemplate != null) caption.font = fontTemplate.CurrentFont;
            caption.color = new Color(1f, .93f, .6f, Mathf.Clamp01((1.5f - FinaleAge) * 2f)); any = true;
        }
        canvasRoot.SetActive(any && !RoundController.IsPaused);
    }
    private void EnsureUi()
    {
        if (canvasRoot != null) return;
        for (int k = 0; k < 3; k++)
        {
            textures[k] = MakeTexture(k);
            sprites[k] = Sprite.Create(textures[k], new Rect(0, 0, 128, 128), new Vector2(.5f, .5f), 100, 0, SpriteMeshType.FullRect);
        }
        canvasRoot = new GameObject("PaintPopLightOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
        canvasRoot.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay; canvasRoot.GetComponent<Canvas>().sortingOrder = 18;
        var scaler = canvasRoot.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
        var group = canvasRoot.GetComponent<CanvasGroup>(); group.blocksRaycasts = group.interactable = false;
        for (int i = 0; i < MaximumPulses; i++)
        {
            var go = new GameObject("PopPulse" + i, typeof(RectTransform)); go.transform.SetParent(canvasRoot.transform, false);
            Pulse p = pulses[i] = new Pulse { root = go.GetComponent<RectTransform>() };
            p.halo = Image(p.root, "Halo", sprites[0]);
            for (int k = 0; k < 12; k++) { p.petals[k] = Image(p.root, "PaintPetal", sprites[2]); p.petals[k].rectTransform.pivot = new Vector2(.5f, 0f); }
            p.ring = Image(p.root, "ExpansionRing", sprites[1]); p.core = Image(p.root, "BrightCore", sprites[0]);
        }
        var title = new GameObject("FinaleLightCaption", typeof(RectTransform), typeof(Text), typeof(Shadow)); title.transform.SetParent(canvasRoot.transform, false);
        caption = title.GetComponent<Text>(); fontTemplate = FindFirstObjectByType<LocalizedText>(FindObjectsInactive.Include);
        caption.font = fontTemplate != null ? fontTemplate.CurrentFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        caption.fontSize = 32; caption.alignment = TextAnchor.MiddleCenter; caption.raycastTarget = false;
        caption.rectTransform.anchorMin = caption.rectTransform.anchorMax = new Vector2(.5f, .72f); caption.rectTransform.sizeDelta = new Vector2(1050, 60);
        title.GetComponent<Shadow>().effectColor = new Color(.02f, .06f, .06f, .95f); title.GetComponent<Shadow>().effectDistance = new Vector2(2, -2);
    }
    private static Image Image(Transform parent, string name, Sprite sprite)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>(); image.sprite = sprite; image.raycastTarget = false; return image;
    }
    private static Texture2D MakeTexture(int kind)
    {
        var texture = new Texture2D(128, 128, TextureFormat.RGBA32, false) { name = "RuntimePaintLight" + kind, filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp };
        var pixels = new Color[128 * 128];
        for (int y = 0; y < 128; y++) for (int x = 0; x < 128; x++)
        {
            float px = (x + .5f) / 64f - 1f, py = (y + .5f) / 64f - 1f, r = Mathf.Sqrt(px * px + py * py);
            float a = kind == 0 ? Mathf.Exp(-r * r * 4f) * Mathf.Clamp01((1f - r) * 4f) : kind == 1 ? Mathf.Exp(-Mathf.Pow((r - .76f) / .055f, 2f))
                : Mathf.Clamp01((1f - px * px / .5f - py * py) * 5f) * (.85f + .15f * Mathf.Sin(y * .4f));
            pixels[y * 128 + x] = new Color(1, 1, 1, a);
        }
        texture.SetPixels(pixels); texture.Apply(false, true); return texture;
    }
    private void OnDestroy()
    {
        if (Instance == this) Instance = null; if (canvasRoot != null) Destroy(canvasRoot);
        foreach (Sprite sprite in sprites) if (sprite != null) Destroy(sprite);
        foreach (Texture2D texture in textures) if (texture != null) Destroy(texture);
    }
}
