#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// <summary>Synthetic rules/physics/UI checks, separate from standalone and human acceptance.</summary>
[InitializeOnLoad]
public static class RainbowChainValidation
{
    private const string ActiveKey = "RainbowChainValidation.Active";
    private const BindingFlags Private = BindingFlags.NonPublic | BindingFlags.Instance;
    private static int stage, assertions, nextId = 900000;
    private static double deadline, waitUntil;
    private static UILanguage originalLanguage;
    private static readonly List<string> errors = new List<string>();
    private static readonly Dictionary<string, int?> preferences = new Dictionary<string, int?>();
    private static GameObject blocker;
    private static int pausedScore;
    private static Balloon[] previewFixture;

    static RainbowChainValidation()
    {
        if (SessionState.GetBool(ActiveKey, false)) EditorApplication.update += Tick;
    }

    public static void Run()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run in a batch Editor.");
        SessionState.SetBool(ActiveKey, true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }

    private static void Tick()
    {
        if (!SessionState.GetBool(ActiveKey, false) || !EditorApplication.isPlaying) return;
        try
        {
            var round = Object.FindFirstObjectByType<RoundController>();
            if (round == null || Time.frameCount < 8) return;
            if (stage == 0)
            {
                deadline = EditorApplication.timeSinceStartup + 120;
                Application.logMessageReceived += OnLog;
                originalLanguage = LanguageSettings.Current;
                InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
                foreach (InputDevice device in InputSystem.devices)
                    if (device is Keyboard || device is Mouse) InputSystem.DisableDevice(device);
                round.BeginRound();
                Require(!round.RainbowChainEnabled, "Classic must be the default.");
                string classicKey = round.SelectedBestScoreKey;
                string legacyKey = (string)typeof(RoundController).GetMethod("GetBestScoreKey",
                    BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { round.SelectedSong.trackId });
                Require(classicKey == legacyKey, "Classic record identity changed.");
                SavePreference(classicKey);
                round.SetRainbowChainEnabled(true);
                Require(round.SelectedBestScoreKey == classicKey + ".RainbowChain.v1", "Chain record is not isolated.");
                SavePreference(round.SelectedBestScoreKey);
                string festivalKey = round.SelectedBestScoreKey;
                round.SetSongMode("standard");
                Require(round.RainbowChainEnabled && round.SelectedBestScoreKey != festivalKey,
                    "Rules did not survive length selection or records overlap.");
                round.SetSongMode("festival");
                Require(round.SelectedBestScoreKey == festivalKey, "Returning to Festival changed its record key.");
                foreach (UILanguage language in Enum.GetValues(typeof(UILanguage)))
                {
                    LanguageSettings.SetLanguage(language);
                    Call(round, "RefreshSongSelectionText");
                    foreach (string key in new[] { "rules.classic", "rules.chain", "rules.chainHint", "chain.ready", "chain.burst", "result.chainBursts" })
                        Require(LocalizationManager.GetText(key) != key, "Missing key: " + key);
                    Dropdown rules = GameObject.Find("SongRules").GetComponent<Dropdown>();
                    Require(rules.value == 1 && rules.captionText.text == LocalizationManager.GetText("rules.chain"), "Rules selector mismatch.");
                    Capture("RainbowSelection-" + LanguageSettings.CurrentCode);
                }
                LanguageSettings.SetLanguage(UILanguage.Japanese);
                stage = 1;
                return;
            }
            if (EditorApplication.timeSinceStartup > deadline) throw new InvalidOperationException("Fixture timed out.");
            if (errors.Count > 0) throw new InvalidOperationException(string.Join(" | ", errors));
            if (EditorApplication.timeSinceStartup < waitUntil) return;
            var chain = round.GetComponent<RainbowChain>();
            var relay = round.GetComponent<TricolorRelay>();
            if (stage == 1)
            {
                if (round.SelectedSong.audioClip.loadState == AudioDataLoadState.Loading) return;
                round.StartSelectedSong();
                Require(RoundController.IsPlaying, "Round failed to start.");
                Object.FindFirstObjectByType<RhythmChartReader>().enabled = false;
                round.SetRainbowChainEnabled(false);
                Require(round.RainbowChainEnabled, "Rules changed during a round.");
                Require(!chain.IsReady && chain.BurstCount == 0, "Dirty initial charge.");
                Pop(0); Pop(1); Pop(2);
                Require(chain.IsReady && chain.BurstCount == 0, "Third color must charge the following hit, not itself.");
                Require(Score.Value == 30 && relay.CompletedCount == 1, "Charge altered scoring or collection.");
                ClearTargets();
                waitUntil = EditorApplication.timeSinceStartup + .1;
                stage = 2;
                return;
            }
            if (stage == 2)
            {
                Call(round, "LateUpdate");
                Call(relay, "LateUpdate");
                Capture("RainbowReady-ja");
                Balloon primary = Spawn(3, new Vector3(0f, 0f, 8f));
                Balloon left = Spawn(4, new Vector3(-.9f, 0f, 8f));
                Balloon right = Spawn(5, new Vector3(.9f, 0f, 8f));
                Balloon extra = Spawn(6, new Vector3(0f, 1.4f, 8f));
                Balloon far = Spawn(7, new Vector3(0f, -2.2f, 8f));
                previewFixture = new[] { primary, left, right, extra, far };
                waitUntil = EditorApplication.timeSinceStartup + .1;
                stage = 19;
                return;
            }
            if (stage == 19)
            {
                Balloon primary = previewFixture[0], left = previewFixture[1], right = previewFixture[2],
                    extra = previewFixture[3], far = previewFixture[4];
                Physics.SyncTransforms();
                var predicted = new List<Balloon>();
                Require(chain.GetPreviewTargets(primary, predicted) == 2 && predicted.Contains(left) && predicted.Contains(right),
                    "Preview planner differs from the expected nearest two.");
                Require(chain.IsReady && Score.Value == 30, "Preview consumed charge or scored.");
                var preview = round.GetComponent<RainbowChainPreview>();
                PlayerShooter shooter = Object.FindFirstObjectByType<PlayerShooter>();
                Transform muzzle = (Transform)typeof(PlayerShooter).GetField("muzzle", Private).GetValue(shooter);
                // Imported mesh/collider center is offset from the logical chart root.
                Collider aimCollider = Array.Find(primary.GetComponentsInChildren<Collider>(), c => c.enabled && !c.isTrigger);
                muzzle.position = aimCollider.bounds.center - Vector3.forward * 2f;
                typeof(PlayerShooter).GetField("lastAimDirection", Private).SetValue(shooter, Vector3.forward);
                typeof(PlayerShooter).GetField("aimValid", Private).SetValue(shooter, true);
                bool foundAim = shooter.TryGetAimedBalloon(out Balloon aimed);
                Physics.SphereCast(muzzle.position, .3f, Vector3.forward, out RaycastHit diagnostic, 30f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                Require(foundAim && aimed == primary, "Preview did not use the shooter SphereCast. Hit: " +
                    (diagnostic.collider == null ? "none" : diagnostic.collider.name) + "; target bounds " + aimCollider.bounds);
                Call(preview, "LateUpdate");
                Require(preview.PrimaryTarget == primary && preview.NeighborCount == 2 && preview.VisibleMarkerCount == 3 && preview.VisibleLinkCount == 2,
                    "Aim preview markers/count mismatch.");
                var overlay = GameObject.Find("RainbowChainPreviewOverlay");
                Require(overlay.GetComponentsInChildren<Collider>(true).Length == 0, "Preview acquired colliders.");
                foreach (Graphic graphic in overlay.GetComponentsInChildren<Graphic>(true))
                    Require(!graphic.raycastTarget, "Preview blocks pointer input.");
                Capture("RainbowPreview-ja");
                Call(shooter, "CancelFireInput");
                Call(preview, "LateUpdate");
                Require(preview.PrimaryTarget == null && preview.VisibleMarkerCount == 0 && chain.IsReady,
                    "Focus cancellation did not hide preview or preserved charge incorrectly.");
                primary.HitAt(primary.transform.position, 10);
                Require(!left.CanReceiveChain(round.SongTimeSeconds) && !right.CanReceiveChain(round.SongTimeSeconds), "Nearest two were not resolved.");
                Require(extra.CanReceiveChain(round.SongTimeSeconds) && far.CanReceiveChain(round.SongTimeSeconds), "Burst exceeded two targets or radius.");
                Require(chain.LastNeighborCount == 2 && chain.BurstCount == 1 && !chain.IsReady, "Burst/charge accounting mismatch.");
                Require(Score.Value == 70 && Score.SuccessfulHits == 6 && Score.OnBeatHits == 1,
                    "Secondary targets duplicated score or gained rhythm bonus.");
                Require(relay.CollectedCount == 1 && relay.CompletedCount == 1, "Secondary targets refilled the relay.");
                primary.Hit(10); left.Hit(10);
                Require(Score.Value == 70 && chain.BurstCount == 1, "A resolved target scored twice or recursed.");
                waitUntil = EditorApplication.timeSinceStartup + .1;
                stage = 20;
                return;
            }
            if (stage == 20)
            {
                Call(round, "LateUpdate");
                Call(relay, "LateUpdate");
                Capture("RainbowBurst-ja");
                chain.GrantCharge();
                chain.GrantCharge();
                Call(round, "TogglePause");
                pausedScore = Score.Value;
                Spawn(0, new Vector3(-2f, 0f, 8f)).Hit();
                Require(chain.IsReady && chain.BurstCount == 1 && Score.Value == pausedScore, "Pause consumed charge or scored.");
                waitUntil = EditorApplication.timeSinceStartup + .35;
                stage = 3;
                return;
            }
            if (stage == 3)
            {
                Require(RoundController.IsPaused && chain.IsReady && Score.Value == pausedScore, "Pause failed to preserve state.");
                round.ResumeRound();
                ClearTargets();
                Balloon primary = Spawn(0, new Vector3(0f, 0f, 8f));
                Balloon target = Spawn(1, new Vector3(1.2f, 0f, 8f));
                blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blocker.name = "RainbowFixtureWall";
                blocker.transform.position = new Vector3(.6f, 0f, 8f);
                blocker.transform.localScale = new Vector3(.1f, 2f, 2f);
                Physics.SyncTransforms();
                primary.Hit();
                Require(target.CanReceiveChain(round.SongTimeSeconds) && chain.LastNeighborCount == 0, "Burst passed through a wall.");
                Require(!chain.IsReady && chain.BurstCount == 2, "Repeated grants stacked charges.");
                blocker.SetActive(false); Object.Destroy(blocker); blocker = null;
                ClearTargets();
                waitUntil = EditorApplication.timeSinceStartup + .1;
                stage = 4;
                return;
            }
            if (stage == 4)
            {
                chain.GrantCharge();
                Balloon primary = Spawn(0, new Vector3(0f, 0f, 8f));
                Balloon valid = Spawn(1, new Vector3(.9f, 0f, 8f));
                Balloon expired = Spawn(2, new Vector3(-.9f, 0f, 8f), -100d);
                Balloon inactive = Spawn(3, new Vector3(0f, .9f, 8f));
                inactive.gameObject.SetActive(false);
                Balloon decoration = Spawn(4, new Vector3(0f, -.9f, 8f));
                decoration.InitializeRhythm(new RhythmEventData { id = -1, timeSeconds = 100d });
                Physics.SyncTransforms();
                primary.Hit();
                Require(chain.LastNeighborCount == 1 && !valid.CanReceiveChain(round.SongTimeSeconds), "Valid nearby chart target not resolved.");
                Require(expired.gameObject.activeSelf && !expired.CanReceiveChain(round.SongTimeSeconds), "Expired target resolved by chain.");
                Require(!inactive.gameObject.activeSelf && decoration.gameObject.activeSelf, "Inactive/decorative target resolved.");
                ClearTargets();
                chain.GrantCharge();
                chain.enabled = false;
                chain.enabled = true;
                int before = chain.BurstCount;
                Pop(5);
                Require(chain.BurstCount == before + 1 && chain.LastNeighborCount == 0 && chain.IsReady,
                    "A direct third color must bank a new charge after spending the old one.");
                Pop(5);
                Require(chain.BurstCount == before + 2 && chain.LastNeighborCount == 0 && !chain.IsReady,
                    "Empty-neighbor burst or event subscription accounting failed.");
                Call(relay, "LateUpdate");
                Call(round, "EndRound");
                Call(relay, "LateUpdate");
                Text results = (Text)typeof(RoundController).GetField("resultStatsText", Private).GetValue(round);
                Require(results.text.Contains(LocalizationManager.Format("result.chainBursts", chain.BurstCount)), "Result omits burst count.");
                foreach (UILanguage language in Enum.GetValues(typeof(UILanguage)))
                {
                    LanguageSettings.SetLanguage(language);
                    Call(round, "RefreshResultStats");
                    Canvas.ForceUpdateCanvases();
                    Capture("RainbowResults-" + LanguageSettings.CurrentCode);
                    Require(results.cachedTextGenerator.characterCountVisible >= results.text.Replace("\n", "").Length,
                        "Result statistics clipped: " + language);
                }
                string chainKey = round.SelectedBestScoreKey;
                Require(PlayerPrefs.GetInt(chainKey) == Score.Value, "Chain result saved to wrong key.");
                foreach (var pair in preferences)
                    if (!pair.Key.EndsWith(".RainbowChain.v1", StringComparison.Ordinal))
                        Require(PlayerPrefs.HasKey(pair.Key) == pair.Value.HasValue &&
                            (!pair.Value.HasValue || PlayerPrefs.GetInt(pair.Key) == pair.Value.Value), "Classic record overwritten.");
                int count = chain.BurstCount;
                chain.GrantCharge();
                Require(!chain.IsReady && chain.BurstCount == count, "Post-round charge granted.");
                round.RestartRound();
                waitUntil = EditorApplication.timeSinceStartup + .75;
                stage = 5;
                return;
            }
            if (stage == 5)
            {
                Require(!RoundController.IsPlaying && !RoundController.IsPaused && !round.RainbowChainEnabled, "Retry failed to restore classic title.");
                Require(!chain.IsReady && !chain.IsFlashing && chain.BurstCount == 0 && relay.CompletedCount == 0,
                    "Retry retained chain/relay state.");
                Require(Score.Value == 0 && GameObject.Find("TricolorRelayOverlay") == null, "Retry retained score/overlay.");
                Finish(null);
            }
        }
        catch (Exception error) { Finish(error); }
    }

    private static Balloon Spawn(int color, Vector3 position, double beat = 100d)
    {
        int id = nextId++;
        Require(Object.FindFirstObjectByType<BalloonSpawner>().Spawn(new RhythmEventData { id = id, timeSeconds = beat }), "Fixture could not spawn target.");
        foreach (Balloon balloon in Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None))
        {
            if (balloon.EventId != id) continue;
            balloon.GetComponent<BalloonFloat>().enabled = false;
            balloon.GetComponent<BalloonEntrance>().enabled = false;
            balloon.GetComponent<Rigidbody>().isKinematic = true;
            balloon.transform.position = position;
            balloon.GetComponent<StudioBalloonAppearance>().ApplyVariant(color, 0);
            return balloon;
        }
        throw new InvalidOperationException("Spawned target missing.");
    }

    private static void Pop(int color) => Spawn(color, new Vector3(-2f, 0f, 8f)).Hit();
    private static void ClearTargets()
    {
        foreach (Balloon balloon in Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None))
        {
            balloon.gameObject.SetActive(false);
            Object.Destroy(balloon.gameObject);
        }
    }
    private static void SavePreference(string key) => preferences[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
    private static void Call(object target, string method) => target.GetType().GetMethod(method, Private).Invoke(target, null);
    private static void Capture(string name) => typeof(KyudaisaiRuntimeSmokeCheck)
        .GetMethod("Capture", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { name });
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
        assertions++;
    }
    private static void OnLog(string message, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) errors.Add(message);
    }
    private static void Finish(Exception error)
    {
        SessionState.SetBool(ActiveKey, false);
        Application.logMessageReceived -= OnLog;
        foreach (var pair in preferences)
            if (pair.Value.HasValue) PlayerPrefs.SetInt(pair.Key, pair.Value.Value); else PlayerPrefs.DeleteKey(pair.Key);
        LanguageSettings.SetLanguage(originalLanguage);
        PlayerPrefs.Save();
        if (blocker != null) { blocker.SetActive(false); Object.Destroy(blocker); }
        Directory.CreateDirectory("Logs");
        File.WriteAllText("Logs/RainbowChainRuntime.json", JsonUtility.ToJson(new Evidence {
            status = error == null ? "passed" : "failed", error = error?.ToString(), assertions = assertions,
            unityVersion = Application.unityVersion, finishedUtc = DateTime.UtcNow.ToString("o") }, true));
        if (error == null) Debug.Log("RAINBOW_CHAIN_RUNTIME_OK: " + assertions + " assertions"); else Debug.LogException(error);
        EditorApplication.Exit(error == null ? 0 : 1);
    }
    [Serializable] private sealed class Evidence
    {
        public string status, error, unityVersion, finishedUtc;
        public int assertions;
        public string scope = "Synthetic Editor shared aim SphereCast, read-only preview/burst parity and input cancellation, charge order, chain cap/radius/occlusion, ordinary secondary scoring, no relay recursion, pause, record isolation, three-language selection/results and Retry.";
        public string limits = "No mouse-input, natural full-song, standalone release, real-Mac, human feel/listening or venue acceptance.";
    }
}
#endif
