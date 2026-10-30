// Copy to Assets/Editor of an isolated project with production runtime scripts,
// the StartScreen/ResultsScreen prefab dependency closure and UIStrings.json.
// Run -batchmode -executeMethod PresentationFeedbackValidation.Run (without -quit).
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BalloonBarrage.Foundation;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class PresentationFeedbackValidation
{
    private const string Key = "PresentationFeedbackValidation.Active";
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    private static string Folder => Path.GetFullPath("../../Logs/AudienceCelebration20261031");
    private static int checks;
    static PresentationFeedbackValidation()
    { if (SessionState.GetBool(Key, false)) EditorApplication.update += Tick; }

    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Directory.CreateDirectory(Folder);
        SessionState.SetBool(Key, true);
        EditorApplication.EnterPlaymode();
    }
    private static void Tick()
    {
        if (!EditorApplication.isPlaying) return;
        EditorApplication.update -= Tick;
        int code = 0;
        try { Verify(); File.WriteAllText(Path.Combine(Folder, "report.txt"), "PRESENTATION_FEEDBACK_OK: " + checks + " controlled assertions; native UI captures.\nNot a full-song, human-input or standalone acceptance.\n"); }
        catch (Exception error) { Debug.LogException(error); File.WriteAllText(Path.Combine(Folder, "report.txt"), error.ToString()); code = 1; }
        SessionState.SetBool(Key, false);
        EditorApplication.Exit(code);
    }

    private static void Verify()
    {
        var camera = new GameObject("DiagnosticCamera", typeof(Camera)).GetComponent<Camera>();
        camera.tag = "MainCamera"; camera.transform.position = new Vector3(0, 0, -8);
        camera.gameObject.AddComponent<AudioListener>();
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.86f, .81f, .7f);
        var events = new GameObject("Events", typeof(EventSystem)).GetComponent<EventSystem>();
        var title = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StudioArtPack/InterfaceV2/Prefabs/StartScreen.prefab"));
        title.SetActive(false);
        var results = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StudioArtPack/InterfaceV2/Prefabs/ResultsScreen.prefab"));
        results.SetActive(false);
        var owner = new GameObject("Round");
        var round = owner.AddComponent<RoundController>(); round.enabled = false;
        var spawner = new GameObject("Spawner").AddComponent<BalloonSpawner>(); spawner.enabled = false;
        var reader = owner.AddComponent<RhythmChartReader>(); reader.enabled = false;
        var guide = owner.AddComponent<RhythmSequenceGuide>(); guide.enabled = false; Call(guide, "Start");
        var active = (List<Balloon>)Read(spawner, "activeBalloons");
        var late = Target(30, 12d, new Vector3(3, .2f, 3), new Color(.96f, .48f, .27f));
        var first = Target(10, 10d, new Vector3(-3, -.4f, 3), new Color(.18f, .62f, .58f));
        var middle = Target(20, 11d, new Vector3(0, 1f, 3), new Color(.95f, .73f, .2f));
        var tied = Target(21, 11d, new Vector3(1, -1, 3), Color.gray);
        active.AddRange(new[] { late, middle, first, tied });
        Play(true); Clock(round, 8d);
        Call(guide, "LateUpdate"); Canvas.ForceUpdateCanvases(); Call(guide, "LateUpdate");
        Check(guide.VisibleTargetCount == 3 && guide.FocusEventId == 10, "Guide sorts by event time, not spawn/list order, and caps at three.");
        var selected = (Balloon[])Read(guide, "targets");
        Check(selected[1].EventId == 20 && selected[2].EventId == 21, "Equal times use stable event-ID order.");
        active.Remove(tied); tied.gameObject.SetActive(false);
        Call(guide, "LateUpdate"); Canvas.ForceUpdateCanvases(); Call(guide, "LateUpdate");
        Check(guide.VisibleLinkCount == 2, "Separated targets produce exactly two links.");
        int score = Score.Value, hits = Score.SuccessfulHits;
        for (int i = 0; i < 15; i++) Call(guide, "LateUpdate");
        Check(Score.Value == score && Score.SuccessfulHits == hits && first.EventTimeSeconds == 10d, "Repeated guidance changes no score, outcomes or note time.");
        Capture(camera, "sequence-controlled.png");
        var graphic = Object.FindFirstObjectByType<RhythmSequenceGraphic>();
        using (var mesh = new VertexHelper())
        {
            typeof(RhythmSequenceGraphic).GetMethod("OnPopulateMesh", Private, null, new[] { typeof(VertexHelper) }, null).Invoke(graphic, new object[] { mesh });
            Check(mesh.currentVertCount > 0 && mesh.currentVertCount < 10000, "Sequence mesh is bounded below 10,000 vertices.");
        }
        GameObject overlay = (GameObject)Read(guide, "root");
        foreach (Graphic item in overlay.GetComponentsInChildren<Graphic>(true)) Check(!item.raycastTarget, "Guide graphics never intercept input.");
        Check(overlay.GetComponentsInChildren<Collider>(true).Length == 0 && overlay.GetComponentsInChildren<Light>(true).Length == 0, "Guide adds no colliders or real-time lights.");
        SetStatic("IsPaused", true); Call(guide, "LateUpdate");
        Check(guide.VisibleTargetCount == 0 && !overlay.activeSelf, "Pause hides guidance.");
        SetStatic("IsPaused", false); Clock(round, 8d); Call(guide, "LateUpdate");
        Check(guide.FocusEventId == 10, "Resume restores the correct target.");
        middle.Hit(); Call(guide, "LateUpdate");
        Check(guide.FocusEventId == 10 && guide.VisibleTargetCount == 2, "Out-of-order early hits remove only that guide.");
        first.Hit(); Call(guide, "LateUpdate");
        Check(guide.FocusEventId == 30 && Score.Value == score + 20, "Ordinary scoring stays unchanged while focus advances.");
        Clock(round, 12.3d); Call(guide, "LateUpdate");
        Check(guide.VisibleTargetCount == 0 && late.CanReceiveChain(12.3d), "Missed timing cues leave remaining balloons freely shootable.");
        Clock(round, 8d); late.transform.position = new Vector3(100, 0, 3); Call(guide, "LateUpdate");
        Check(guide.VisibleTargetCount == 0, "Offscreen targets do not draw a misleading route.");
        late.transform.position = new Vector3(3, 0, 3);
        ((HashSet<int>)Read(reader, "resolvedEventIds")).Add(30); Call(guide, "LateUpdate");
        Check(guide.VisibleTargetCount == 0, "Reader-resolved targets never appear in guidance.");
        ((HashSet<int>)Read(reader, "resolvedEventIds")).Clear();
        var demo = owner.AddComponent<FestivalDemonstrationCue>(); demo.enabled = false;
        Write(guide, "demonstration", demo);
        typeof(FestivalDemonstrationCue).GetField("<IsDemonstrating>k__BackingField", Private).SetValue(demo, true);
        Call(guide, "LateUpdate"); Check(guide.VisibleTargetCount == 0, "Demonstrations take visual priority.");
        typeof(FestivalDemonstrationCue).GetField("<IsDemonstrating>k__BackingField", Private).SetValue(demo, false);
        var preview = owner.AddComponent<RainbowChainPreview>(); preview.enabled = false;
        Write(guide, "chainPreview", preview);
        typeof(RainbowChainPreview).GetField("<VisibleMarkerCount>k__BackingField", Private).SetValue(preview, 3);
        Call(guide, "LateUpdate"); Check(guide.VisibleTargetCount == 0, "Chain previews take visual priority.");
        Play(false); Call(guide, "LateUpdate"); Check(!overlay.activeSelf, "Settlement clears guidance.");
        late.gameObject.SetActive(false);

        // Exercise actual direct-pop callbacks, not a copied sequence model.
        Play(true); Clock(round, 8d); active.Clear();
        var phrase = new[] { Target(40, 10d, new Vector3(-3, 0, 3), Color.cyan),
            Target(41, 11d, new Vector3(0, 0, 3), Color.yellow), Target(42, 12d, new Vector3(3, 0, 3), Color.red) };
        active.AddRange(phrase);
        typeof(RainbowChainPreview).GetField("<VisibleMarkerCount>k__BackingField", Private).SetValue(preview, 0);
        guide.enabled = true; Call(guide, "LateUpdate");
        phrase[0].Hit(); Call(guide, "LateUpdate");
        Check(((Text[])Read(guide, "numbers"))[0].text == "2", "Sequence numbers remain stable after the first hit.");
        phrase[1].Hit(); Call(guide, "LateUpdate"); phrase[2].Hit(); Call(guide, "LateUpdate");
        Check(guide.CelebrationCount == 1 && Score.Value == score + 50, "Three ordered direct pops celebrate once, with ordinary scoring only.");
        Capture(camera, "sequence-celebration-controlled.png");
        guide.enabled = false; Play(false);

        Vector2 small = GamepadAimMotion.ViewportDelta(new Vector2(.25f, 0), .02f, 16f / 9f);
        Vector2 large = GamepadAimMotion.ViewportDelta(Vector2.right, .02f, 16f / 9f);
        Check(small.x > 0 && small.x < large.x * .15f, "Small stick movements provide precise micro-aiming.");
        Check(GamepadAimMotion.ViewportDelta(new Vector2(.1f, 0), .02f, 1f) == Vector2.zero, "Deadzone prevents drift.");
        Check(GamepadAimMotion.ViewportDelta(Vector2.zero, .02f, 1f) == Vector2.zero, "Released stick stops immediately.");
        Check(Vector2.Distance(GamepadAimMotion.ViewportDelta(Vector2.right, .01f, 1f) * 2,
            GamepadAimMotion.ViewportDelta(Vector2.right, .02f, 1f)) < .00001f, "Normal-frame motion is frame-rate independent.");
        Check(Mathf.Abs(GamepadAimMotion.ViewportDelta(Vector2.right, .02f, 2f).x * 2f -
            GamepadAimMotion.ViewportDelta(Vector2.up, .02f, 2f).y) < .00001f, "Aim speed is isotropic in screen-height units.");
        Check(GamepadAimMotion.ViewportDelta(Vector2.right, .02f, 1f, .55f).x <
            GamepadAimMotion.ViewportDelta(Vector2.right, .02f, 1f).x, "Precision slowdown reduces speed without snapping.");

        title.SetActive(true);
        var special = owner.AddComponent<KyudaisaiFestivalPresentation>(); special.enabled = false;
        special.Configure(round, title, results); special.enabled = true;
        Call(special, "LateUpdate");
        var badge = (Button)Read(special, "badge");
        foreach (UILanguage language in Enum.GetValues(typeof(UILanguage)))
        {
            LanguageSettings.SetLanguage(language); Call(special, "RefreshText");
            foreach (string field in new[] { "titleCaption", "resultCaption", "secretTitle", "secretMessage", "closeLabel" })
            {
                Text text = (Text)Read(special, field);
                Check(!text.text.StartsWith("festival."), "Festival strings resolve for " + language + ": " + field);
                foreach (char ch in text.text) if (!char.IsWhiteSpace(ch)) Check(text.font.HasCharacter(ch), "Bundled font covers " + language + " U+" + ((int)ch).ToString("X4"));
            }
            Canvas.ForceUpdateCanvases(); Capture(camera, "title-" + language + ".png");
        }
        badge.onClick.Invoke(); badge.onClick.Invoke();
        Check(!special.IsSecretOpen && special.SecretTapCount == 2, "Two badge activations do not reveal the secret.");
        badge.onClick.Invoke(); Check(special.IsSecretOpen, "The third badge activation reveals the secret.");
        var close = (Button)Read(special, "closeButton");
        Check(events.currentSelectedGameObject == close.gameObject, "Secret gives keyboard/gamepad focus to Close.");
        Check(!badge.IsInteractable() && close.IsInteractable(), "Modal disables underlying controller navigation while keeping Close usable.");
        ((FestivalPresentationGraphic)Read(special, "secretArt")).SetAge(2f);
        Canvas.ForceUpdateCanvases(); Capture(camera, "secret.png");
        close.onClick.Invoke(); Check(!special.IsSecretOpen, "Close immediately exits the secret.");
        Check(badge.IsInteractable(), "Closing the modal restores underlying menu interaction.");
        badge.onClick.Invoke(); Write(special, "tapDeadline", -1f); Call(special, "LateUpdate");
        Check(special.SecretTapCount == 0, "Partial secret taps expire.");
        for (int i = 0; i < 3; i++) badge.onClick.Invoke();
        title.SetActive(false); Play(true); Call(special, "LateUpdate");
        Check(!special.IsSecretOpen && !badge.gameObject.activeSelf, "Starting gameplay closes/hides all title presentation.");
        Play(false); results.SetActive(true); Call(special, "LateUpdate");
        Check(special.IsResultCelebrating, "Results trigger celebration independently of score.");
        var lamps = (FestivalPresentationGraphic)Read(special, "resultArt"); lamps.SetAge(4f);
        Check(lamps.LitLampCount == 9, "Exactly nine lamps light.");
        Canvas.ForceUpdateCanvases(); Capture(camera, "results.png");
        using (var mesh = new VertexHelper())
        {
            typeof(FestivalPresentationGraphic).GetMethod("OnPopulateMesh", Private, null, new[] { typeof(VertexHelper) }, null).Invoke(lamps, new object[] { mesh });
            Check(mesh.currentVertCount > 0 && mesh.currentVertCount < 10000, "Festival mesh is bounded below 10,000 vertices.");
        }
        results.SetActive(false); title.SetActive(true); Call(special, "LateUpdate");
        Check(!special.IsResultCelebrating && !special.IsSecretOpen && special.SecretTapCount == 0, "Return/retry state clears celebration and secret.");
        Check(Score.Value == score + 50, "All title/secret/results presentation preserves scoring.");
        foreach (string name in new[] { "titleRoot", "resultsRoot", "secretRoot" })
        {
            GameObject layer = (GameObject)Read(special, name);
            Check(layer.GetComponentsInChildren<Light>(true).Length == 0 && layer.GetComponentsInChildren<Collider>(true).Length == 0,
                "Festival layer adds no world lights or colliders: " + name);
        }
        title.SetActive(false); results.SetActive(false); Play(true); Clock(round, 8d);
        int chainBase = Score.Value;
        var chain = owner.AddComponent<RainbowChain>(); chain.Configure(round);
        preview.Configure(chain);
        typeof(RoundController).GetField("<RainbowChainEnabled>k__BackingField", Private).SetValue(round, true);
        active.Clear();
        var primary = Target(50, 10d, new Vector3(0, 0, 3), Color.cyan);
        var left = Target(51, 11d, new Vector3(-1.1f, 0, 3), Color.yellow);
        var right = Target(52, 12d, new Vector3(1.1f, 0, 3), Color.red);
        active.AddRange(new[] { primary, left, right }); Physics.SyncTransforms();
        chain.GrantCharge();
        var planned = new List<Balloon>();
        Check(chain.GetPreviewTargets(primary, planned) == 2, "Real chain planner finds two eligible neighbors.");
        typeof(RainbowChainPreview).GetMethod("Show", Private).Invoke(preview, new object[] { primary });
        Capture(camera, "chain-ready-controlled.png");
        primary.Hit(); Call(preview, "LateUpdate");
        Check(chain.LastNeighborCount == 2 && chain.LastExtraPoints == 20 && Score.Value == chainBase + 30,
            "One ordinary direct hit resolves two extras and the celebration reports actual extra points.");
        Check(preview.IsShowingBurst && preview.VisibleLinkCount == 2 && !chain.IsReady, "Spent charge produces two visible burst beams.");
        Write(preview, "burstRemaining", .55f); Call(preview, "LateUpdate"); Capture(camera, "chain-burst-controlled.png");
        float remaining = (float)Read(preview, "burstRemaining");
        SetStatic("IsPaused", true); Call(preview, "LateUpdate");
        Check(preview.VisibleMarkerCount == 0 && (float)Read(preview, "burstRemaining") == remaining, "Pause hides and freezes chain celebration.");
        SetStatic("IsPaused", false); Call(preview, "LateUpdate");
        Check(preview.VisibleMarkerCount == 3, "Resume restores chain celebration.");
        active.Clear(); var lone = Target(60, 12d, new Vector3(0, 0, 3), Color.cyan); active.Add(lone);
        chain.GrantCharge(); lone.Hit(); Call(preview, "LateUpdate");
        Check(chain.LastNeighborCount == 0 && chain.LastExtraPoints == 0 && Score.Value == chainBase + 40,
            "A lone-target burst truthfully reports zero extras, with no invented rewards.");
        Check(((Text)Read(preview, "label")).text == LocalizationManager.GetText("chain.burst.empty"), "Zero extras have a distinct explanation.");
        Play(false); Call(preview, "LateUpdate"); Check(!preview.IsShowingBurst, "Settlement clears burst feedback.");

        var pad = InputSystem.AddDevice<Gamepad>();
        var shooter = new GameObject("DiagnosticShooter").AddComponent<PlayerShooter>(); shooter.enabled = false;
        var local = owner.AddComponent<LocalPlayerInput>(); local.enabled = false; local.Configure(round, shooter);
        local.SelectLayout(1); Check(local.PrepareRound(), "Gamepad layout prepares with a virtual controller.");
        ((Gamepad[])Read(local, "assigned"))[0] = pad; Write(local, "hasFocus", true);
        Play(true); local.NotifyHit(0, true, false);
        Check(((float[])Read(local, "rumbleRemaining"))[0] > 0f, "Timed hit starts feedback only on the assigned virtual controller.");
        SetStatic("IsPaused", true); Call(local, "UpdateRumble");
        Check(((float[])Read(local, "rumbleRemaining"))[0] == 0f, "Pause stops controller vibration immediately.");
        SetStatic("IsPaused", false); Write(local, "hasFocus", false); local.NotifyHit(0, false, true);
        Check(((float[])Read(local, "rumbleRemaining"))[0] == 0f, "Unfocused input cannot start controller vibration.");
        InputSystem.RemoveDevice(pad); Play(false);
        VerifyAudience(owner, round, spawner, guide, chain, camera);
        Object.Destroy(special); Object.Destroy(guide);
        Debug.Log("PRESENTATION_FEEDBACK_OK: " + checks + " assertions.");
    }

    private static Balloon Target(int id, double time, Vector3 position, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere); go.transform.position = position;
        var material = new Material(Shader.Find("Unlit/Color")); material.color = color;
        go.GetComponent<Renderer>().sharedMaterial = material;
        Balloon balloon = go.AddComponent<Balloon>(); balloon.InitializeRhythm(new RhythmEventData { id = id, timeSeconds = time });
        return balloon;
    }
    private static void VerifyAudience(GameObject owner, RoundController round, BalloonSpawner spawner,
        RhythmSequenceGuide guide, RainbowChain chain, Camera camera)
    {
        LanguageSettings.SetLanguage(UILanguage.SimplifiedChinese);
        foreach (var target in Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None)) target.gameObject.SetActive(false);
        var active = (List<Balloon>)Read(spawner, "activeBalloons"); active.Clear();
        var audience = owner.AddComponent<AudienceCelebration>(); audience.enabled = false;
        chain.enabled = false; chain.ResetRound();
        Play(false); Call(audience, "LateUpdate"); Play(true); Clock(round, 8d);
        guide.enabled = true;
        var phrase = new[] { Target(900, 10d, new Vector3(-3, 0, 3), Color.cyan),
            Target(901, 11d, new Vector3(0, 0, 3), Color.yellow), Target(902, 12d, new Vector3(3, 0, 3), Color.red) };
        active.AddRange(phrase); Call(guide, "LateUpdate");
        int score = Score.Value;
        phrase[0].Hit(); Call(guide, "LateUpdate"); Call(audience, "LateUpdate");
        Check(audience.ShownCelebrationCount == 0, "An incomplete ordered phrase does not pretend to succeed.");
        phrase[1].Hit(); Call(guide, "LateUpdate"); phrase[2].Hit(); Call(guide, "LateUpdate"); Call(audience, "LateUpdate");
        AdvanceAudience(audience, .6f);
        Check(audience.ShownCelebrationCount == 1 && audience.VisibleReactionCount == 8, "Actual ordered direct pops show four reaction pairs.");
        Check(audience.CheerStartCount == 1, "Ordered success requests one cheer voice.");
        Check(Score.Value == score + 30, "Audience reactions preserve the three ordinary pop rewards.");
        var layer = (GameObject)Read(audience, "root");
        Check(!layer.GetComponent<CanvasGroup>().blocksRaycasts && !layer.GetComponent<CanvasGroup>().interactable,
            "The audience overlay cannot intercept aiming or UI input.");
        foreach (var graphic in layer.GetComponentsInChildren<Graphic>(true)) Check(!graphic.raycastTarget, "Every audience graphic is passive.");
        foreach (var texture in (Texture2D[])Read(audience, "emojiTextures")) Check(texture != null && texture.width == 128, "Pinned public emoji imports at 128 pixels.");
        foreach (string field in new[] { "yay", "crowd" })
        {
            var clip = (AudioClip)Read(audience, field);
            Check(clip != null && clip.length > .2f && clip.samples > 0, "Real cheer audio imported: " + field);
        }
        var audio = (AudioSource)Read(audience, "cheer");
        Check(!audio.loop && audio.spatialBlend == 0f && !audio.ignoreListenerPause, "A single non-looping 2D voice respects listener pause.");
        foreach (UILanguage language in Enum.GetValues(typeof(UILanguage)))
        {
            LanguageSettings.SetLanguage(language);
            Font font = ((LocalizedText)Read(audience, "fontTemplate")).CurrentFont;
            foreach (string key in new[] { "audience.sequence", "audience.relay", "audience.chain", "audience.like", "audience.cheer", "audience.encore" })
                foreach (char character in LocalizationManager.Format(key, 2, 20))
                    if (!char.IsWhiteSpace(character)) Check(font.HasCharacter(character), "Audience font covers " + language + ": " + character);
        }
        LanguageSettings.SetLanguage(UILanguage.SimplifiedChinese);
        Capture(camera, "audience-sequence-controlled.png");
        Capture(camera, "audience-portrait-controlled.png", 900, 1600);
        Capture(camera, "audience-four-three-controlled.png", 1200, 900);
        Check(audience.VisibleReactionCount <= AudienceCelebration.MaximumReactions, "All aspect ratios retain a bounded pool.");
        SetStatic("IsPaused", true); Call(audience, "LateUpdate");
        Check(!layer.activeSelf && !audio.isPlaying && audience.VisibleReactionCount == 0, "Pause clears reactions and stops cheer playback.");
        SetStatic("IsPaused", false); Call(audience, "LateUpdate");
        Check(audience.ShownCelebrationCount == 1 && !layer.activeSelf, "Resume does not replay an old success.");
        guide.enabled = false; chain.enabled = true;
        foreach (var balloon in phrase) balloon.gameObject.SetActive(false);
        active.Clear();
        var primary = Target(910, 10d, new Vector3(0, 0, 3), Color.cyan);
        var neighbor = Target(911, 11d, new Vector3(1.1f, 0, 3), Color.yellow);
        active.AddRange(new[] { primary, neighbor }); Physics.SyncTransforms(); chain.GrantCharge();
        primary.Hit(); Call(audience, "LateUpdate"); AdvanceAudience(audience, .6f);
        Check(chain.LastNeighborCount == 1 && audience.ShownCelebrationCount == 2, "A real neighboring chain pop gets its own reaction.");
        Check(layer.GetComponentInChildren<Text>().text == LocalizationManager.Format("audience.chain", 1, 10), "The audience reports actual neighbor and point totals.");
        Capture(camera, "audience-chain-controlled.png");
        active.Clear(); var lone = Target(912, 12d, new Vector3(0, 0, 3), Color.cyan); active.Add(lone);
        chain.GrantCharge(); lone.Hit(); Call(audience, "LateUpdate");
        Check(chain.LastNeighborCount == 0 && audience.ShownCelebrationCount == 2, "Zero-extra chains do not get a misleading cheer.");
        var relay = owner.AddComponent<TricolorRelay>(); relay.enabled = false;
        for (int i = 0; i < 3; i++) typeof(TricolorRelay).GetMethod("Collect", Private).Invoke(relay, new object[] { i, Color.cyan });
        int beforeRelay = Score.Value; Call(audience, "LateUpdate");
        Check(audience.ShownCelebrationCount == 3 && Score.Value == beforeRelay, "The real relay callback celebrates without awarding extra score.");
        for (int repeat = 0; repeat < 30; repeat++)
            typeof(AudienceCelebration).GetMethod("Celebrate", Private).Invoke(audience, new object[] { "audience.sequence", false, 0, 0 });
        AdvanceAudience(audience, .6f);
        Check(audience.VisibleReactionCount <= AudienceCelebration.MaximumReactions && layer.transform.childCount == 9,
            "Success storms reuse the fixed eight-row pool rather than allocating more UI.");
        Check(owner.GetComponents<AudioSource>().Length == 1 && audience.CheerStartCount == 2, "Success storms cannot overlap or spam cheer voices.");
        typeof(AudienceCelebration).GetMethod("OnApplicationFocus", Private).Invoke(audience, new object[] { false });
        Check(!layer.activeSelf && !audio.isPlaying, "Losing focus immediately stops all audience presentation.");
        typeof(AudienceCelebration).GetMethod("OnApplicationFocus", Private).Invoke(audience, new object[] { true });
        Play(false); Call(audience, "LateUpdate"); Play(true); Call(audience, "LateUpdate");
        Check(!layer.activeSelf, "Returning from results never replays stale successes.");
        typeof(AudienceCelebration).GetMethod("Celebrate", Private).Invoke(audience, new object[] { "audience.sequence", false, 0, 0 });
        AdvanceAudience(audience, 10f); Check(!layer.activeSelf && !audio.isPlaying, "Expired effects consume no active presentation or looping audio.");
        Play(false); Object.Destroy(audience); relay.enabled = false;
    }
    private static void AdvanceAudience(AudienceCelebration audience, float seconds) =>
        typeof(AudienceCelebration).GetMethod("Animate", Private).Invoke(audience, new object[] { seconds });

    private static void Capture(Camera camera, string name, int width = 1600, int height = 900)
    {
        var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        var modes = new RenderMode[canvases.Length];
        var target = RenderTexture.GetTemporary(width, height, 24);
        var prior = RenderTexture.active;
        try
        {
            camera.targetTexture = target;
            for (int i = 0; i < canvases.Length; i++)
            { modes[i] = canvases[i].renderMode; canvases[i].renderMode = RenderMode.ScreenSpaceCamera; canvases[i].worldCamera = camera; canvases[i].planeDistance = 1f; }
            camera.aspect = (float)width / height;
            Canvas.ForceUpdateCanvases();
            foreach (var scaler in Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None))
                typeof(CanvasScaler).GetMethod("Handle", Private).Invoke(scaler, null);
            Canvas.ForceUpdateCanvases();
            // Destroy takes effect at frame end; this synchronous fixture mirrors that
            // visibility before capturing the accepted-pop snapshot in the same tick.
            foreach (var balloon in Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None))
                if (!balloon.CanReceiveChain(8d))
                    foreach (var renderer in balloon.GetComponentsInChildren<Renderer>()) renderer.enabled = false;
            foreach (var text in Object.FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if (text.font != null && text.font.dynamic) text.font.RequestCharactersInTexture(text.text,
                    Mathf.Max(1, Mathf.RoundToInt(text.fontSize * text.pixelsPerUnit)), text.fontStyle);
                text.SetAllDirty();
            }
            Canvas.ForceUpdateCanvases();
            foreach (var activeGuide in Object.FindObjectsByType<RhythmSequenceGuide>(FindObjectsSortMode.None))
                if (activeGuide.VisibleTargetCount > 0) Call(activeGuide, "LateUpdate");
            foreach (var activeAudience in Object.FindObjectsByType<AudienceCelebration>(FindObjectsSortMode.None))
            {
                AdvanceAudience(activeAudience, 0f);
                var audienceRoot = (GameObject)Read(activeAudience, "root");
                if (audienceRoot == null || !audienceRoot.activeSelf) continue;
                RectTransform parent = audienceRoot.GetComponent<RectTransform>();
                foreach (RectTransform row in parent)
                {
                    if (!row.gameObject.activeSelf || !row.name.StartsWith("AudienceReaction") || row.GetComponent<CanvasGroup>().alpha <= 0f) continue;
                    Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, row);
                    Check(bounds.min.x >= parent.rect.xMin && bounds.max.x <= parent.rect.xMax &&
                        bounds.min.y >= parent.rect.yMin && bounds.max.y <= parent.rect.yMax, "Reaction stays inside the capture viewport.");
                    Check(bounds.max.x <= parent.rect.xMin + parent.rect.width * .24f ||
                        bounds.min.x >= parent.rect.xMax - parent.rect.width * .24f, "Reactions leave the central aiming region clear.");
                }
            }
            Canvas.ForceUpdateCanvases(); camera.Render(); camera.Render(); RenderTexture.active = target;
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0); texture.Apply();
            File.WriteAllBytes(Path.Combine(Folder, name), texture.EncodeToPNG()); Object.Destroy(texture);
        }
        finally
        {
            camera.targetTexture = null; RenderTexture.active = prior; RenderTexture.ReleaseTemporary(target);
            for (int i = 0; i < canvases.Length; i++) { canvases[i].renderMode = modes[i]; canvases[i].worldCamera = null; }
        }
    }
    private static void Clock(RoundController round, double now)
    { Write(round, "songStartDspTime", AudioSettings.dspTime - now); Write(round, "totalPausedDspSeconds", 0d); }
    private static void Play(bool value) { SetStatic("IsPlaying", value); SetStatic("IsPaused", false); }
    private static void SetStatic(string name, bool value) => typeof(RoundController).GetField("<" + name + ">k__BackingField", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, value);
    private static object Read(object target, string name) => target.GetType().GetField(name, Private).GetValue(target);
    private static void Write(object target, string name, object value) => target.GetType().GetField(name, Private).SetValue(target, value);
    private static void Call(object target, string name) => target.GetType().GetMethod(name, Private).Invoke(target, null);
    private static void Check(bool condition, string message)
    { checks++; if (!condition) throw new InvalidOperationException(message); }
}
#endif
