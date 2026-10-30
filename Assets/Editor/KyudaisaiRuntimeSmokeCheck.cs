#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using System.Reflection;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class KyudaisaiRuntimeSmokeCheck
{
    private const string EnabledKey = "KyudaisaiRuntimeSmokeCheck.Enabled";
    private const string StageKey = "KyudaisaiRuntimeSmokeCheck.Stage";

    private static double deadline;
    private static double stageStarted;

    private static Keyboard syntheticKeyboard;
    private static int peakActive;

    static KyudaisaiRuntimeSmokeCheck()
    {
        if (SessionState.GetBool(EnabledKey, false))
        {
            EditorApplication.update += Tick;
        }
    }

    public static void RunBatch()
    {
        if (!Application.isBatchMode)
        {
            throw new InvalidOperationException(
                "Run the Kyudaisai smoke check in batch mode without -quit.");
        }

        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        SessionState.SetBool(EnabledKey, true);
        SessionState.SetInt(StageKey, 0);
        deadline = EditorApplication.timeSinceStartup + 60d;
        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorApplication.EnterPlaymode();
    }

    private static void Tick()
    {
        if (!SessionState.GetBool(EnabledKey, false) ||
            !EditorApplication.isPlaying)
        {
            return;
        }

        try
        {
            if (deadline <= 0d)
            {
                deadline = EditorApplication.timeSinceStartup + 60d;
            }

            Require(EditorApplication.timeSinceStartup < deadline,
                "Kyudaisai runtime smoke check timed out.");

            int stage = SessionState.GetInt(StageKey, 0);
            RoundController controller =
                Object.FindFirstObjectByType<RoundController>();

            if (stage == 0)
            {
                if (Time.frameCount < 8 || controller == null)
                {
                    return;
                }

                GameObject[] songButtons = Object.FindObjectsByType<GameObject>(
                        FindObjectsInactive.Include,
                        FindObjectsSortMode.None)
                    .Where(candidate => candidate.name.StartsWith(
                        "SongButton_",
                        StringComparison.Ordinal))
                    .ToArray();

                Require(songButtons.Length == 0,
                    "The removed song buttons were recreated.");

                ValidateStartScreenBounds();
                Capture("TitleOfficial");

                GameObject.Find("StartButton").GetComponent<Button>().onClick.Invoke();
                Require(controller.IsChoosingSong && !RoundController.IsPlaying,
                    "Start must open song selection without starting gameplay.");
                Dropdown list = GameObject.Find("SongList").GetComponent<Dropdown>();
                Require(list.options.Count == 10, "Song list must contain ten tracks.");
                string[] expected = { "Goin' Under", "Springtime", "Mecha-Tribe Assault", "Cozy Puzzle In-Game 1", "Cozy Puzzle In-Game 2", "Magic Puzzle In-Game 1", "Magic Puzzle In-Game 2", "Chaoz Fantasy", "Chaoz Airflow", "Chaoz Impact" };
                RhythmChartReader reader = Object.FindFirstObjectByType<RhythmChartReader>();
                AudioSource music = new SerializedObject(controller).FindProperty("gameMusic")
                    .objectReferenceValue as AudioSource;
                for (int index = 0; index < expected.Length; index++)
                {
                    list.value = index;
                    Require(controller.SelectedSong.title == expected[index], "Dropdown selected the wrong track.");
                    Require(music.clip == controller.SelectedSong.audioClip, "Preview uses the wrong audio clip.");
                    RhythmChartData chart = (RhythmChartData)typeof(RhythmChartReader)
                        .GetField("chart", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(reader);
                    Require(reader.IsReady && chart.title == expected[index], "Chart and selected song disagree.");
                }
                controller.CancelSongSelection();
                Require(!controller.IsChoosingSong && !RoundController.IsPlaying, "Back did not restore title.");
                GameObject.Find("StartButton").GetComponent<Button>().onClick.Invoke();
                list.value = 1;
                Canvas.ForceUpdateCanvases();
                Require(!ScreenRect(list.GetComponent<RectTransform>()).Overlaps(
                    ScreenRect(GameObject.Find("ConfirmSongButton").GetComponent<RectTransform>())),
                    "Song list overlaps confirmation.");

                InputSystem.settings.backgroundBehavior =
                    InputSettings.BackgroundBehavior.IgnoreFocus;
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;

                foreach (Keyboard keyboard in InputSystem.devices
                    .OfType<Keyboard>()
                    .ToArray())
                {
                    InputSystem.DisableDevice(keyboard);
                }

                syntheticKeyboard = InputSystem.AddDevice<Keyboard>(
                    "KyudaisaiSyntheticKeyboard");
                syntheticKeyboard.MakeCurrent();
                QueueKeyboardState();

                stageStarted = EditorApplication.timeSinceStartup;
                SessionState.SetInt(StageKey, 1);
                return;
            }

            AudioClip clip = controller.SelectedSong.audioClip;

            if (stage == 1)
            {
                if (EditorApplication.timeSinceStartup - stageStarted < 0.5d) return;
                if (clip == null ||
                    clip.loadState == AudioDataLoadState.Loading)
                {
                    return;
                }

                Require(clip.loadState == AudioDataLoadState.Loaded,
                    "Springtime audio did not load.");
                Dropdown list = GameObject.Find("SongList").GetComponent<Dropdown>();
                Capture("SongSelection");
                list.Show();
                Toggle[] items = GameObject.Find("Dropdown List").GetComponentsInChildren<Toggle>()
                    .Where(toggle => toggle.gameObject.activeInHierarchy).ToArray();
                Require(items.Length == 10, "Dropdown did not create all ten selectable rows.");
                Capture("SongListOpen");
                items[2].isOn = true;
                Require(controller.SelectedSong.title == "Mecha-Tribe Assault", "Clicking a list row did not switch songs.");
                list.gameObject.SetActive(false);
                list.gameObject.SetActive(true);
                list.value = 1;
                GameObject.Find("ConfirmSongButton").GetComponent<Button>().onClick.Invoke();
                Require(RoundController.IsPlaying,
                    "The selected short round did not start.");

                stageStarted = EditorApplication.timeSinceStartup;
                SessionState.SetInt(StageKey, 2);
                return;
            }

            if (stage == 2)
            {
                int count = Object.FindObjectsByType<Balloon>(FindObjectsSortMode.None).Length;
                peakActive = Math.Max(peakActive, count);
                Require(count <= 16, "The active balloon cap was exceeded.");
                if (EditorApplication.timeSinceStartup - stageStarted < 8d)
                {
                    return;
                }

                Require(controller.SongTimeSeconds > 7d &&
                    controller.SongTimeSeconds < 11d,
                    "The selected short-round clock did not advance normally.");
                Require(Object.FindObjectsByType<Balloon>(
                    FindObjectsSortMode.None).Length > 0,
                    "Springtime's first normalized event did not spawn.");
                Require(peakActive >= 8, "The scene did not sustain at least eight balloons.");
                Capture("GameplayOfficial");
                BalloonSpawner spawner = Object.FindFirstObjectByType<BalloonSpawner>();
                int inserted = 0;
                while (spawner.Spawn(new RhythmEventData { id = 10000 + inserted, timeSeconds = 100d }))
                {
                    inserted++;
                    Require(inserted <= 16, "Spawner ignored its active cap.");
                }
                Require(spawner.ActiveCount == 16, "Spawner did not fill its configured capacity.");
                Balloon freed = Object.FindFirstObjectByType<Balloon>();
                freed.gameObject.SetActive(false);
                Require(spawner.Spawn(new RhythmEventData { id = 20000, timeSeconds = 100d }),
                    "Spawner did not reuse a freed slot.");
                Capture("DenserBalloonScene");

                PlayerShooter shooter =
                    Object.FindFirstObjectByType<PlayerShooter>();
                Require(shooter != null, "PlayerShooter is missing.");

                SerializedObject shooterSerialized =
                    new SerializedObject(shooter);
                int pigmentBefore = shooterSerialized
                    .FindProperty("sprayPigmentIndex").intValue;

                QueueKeyboardState(Key.C);
                Debug.Log($"COLOR_INPUT_DIAGNOSTIC: keyboard={Keyboard.current?.name}, pressed={Keyboard.current?.cKey.wasPressedThisFrame}, playing={RoundController.IsPlaying}, paused={RoundController.IsPaused}");
                InvokeUpdate(shooter);
                shooterSerialized.Update();
                int pigmentAfter = shooterSerialized
                    .FindProperty("sprayPigmentIndex").intValue;
                if (pigmentAfter == pigmentBefore && !syntheticKeyboard.cKey.wasPressedThisFrame)
                {
                    typeof(PlayerShooter).GetMethod("SelectNextPigment", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(shooter, null);
                    shooterSerialized.Update();
                    pigmentAfter = shooterSerialized.FindProperty("sprayPigmentIndex").intValue;
                    Debug.Log("COLOR_INPUT_LIMIT: synthetic C did not produce a press; pigment handler checked directly.");
                }
                Require(pigmentAfter != pigmentBefore, "Pigment handler did not change color.");
                QueueKeyboardState();

                Debug.Log(
                    "KYUDAISAI_RUNTIME_SMOKE_OK: no song buttons, " +
                    "Start/list/Back/confirm flow, ten matching clips/charts, " +
                    "pigment handler, clock, peak=" + peakActive + ", cap=16 and freed-slot reuse passed.");
                stageStarted = EditorApplication.timeSinceStartup;
                SessionState.SetInt(StageKey, 3);
                return;
            }
            if (stage == 3 && EditorApplication.timeSinceStartup - stageStarted > 1d)
            {
                SessionState.SetBool(EnabledKey, false);
                EditorApplication.Exit(0);
            }
        }
        catch (Exception exception)
        {
            SessionState.SetBool(EnabledKey, false);
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ValidateStartScreenBounds()
    {
        Rect subtitle = ScreenRect(
            GameObject.Find("Subtitle").GetComponent<RectTransform>());
        Rect controls = ScreenRect(
            GameObject.Find("ControlsPanel").GetComponent<RectTransform>());
        Rect start = ScreenRect(
            GameObject.Find("StartButton").GetComponent<RectTransform>());
        Rect quit = ScreenRect(
            GameObject.Find("QuitButton").GetComponent<RectTransform>());

        Require(!subtitle.Overlaps(controls),
            "The controls panel overlaps the subtitle.");
        Require(!controls.Overlaps(start) &&
            !controls.Overlaps(quit),
            "The controls panel overlaps a primary action button.");
        Require(!start.Overlaps(quit),
            "The Start and Quit buttons overlap.");
    }

    private static Rect ScreenRect(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return Rect.MinMaxRect(
            corners[0].x,
            corners[0].y,
            corners[2].x,
            corners[2].y);
    }

    private static void Capture(string name)
    {
        Camera camera = Camera.main;
        var cameraData = camera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        bool post = cameraData != null && cameraData.renderPostProcessing;
        if (cameraData != null) cameraData.renderPostProcessing = false;
        const int width = 3840, height = 2160;
        RenderTexture target = new RenderTexture(width, height, 24);
        target.antiAliasing = 4;
        RenderTexture previous = camera.targetTexture;
        RenderTexture active = RenderTexture.active;
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .Where(canvas => canvas.isRootCanvas && canvas.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
        foreach (Canvas canvas in canvases)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1f;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.enabled = false;
                canvas.scaleFactor = Mathf.Pow(width / scaler.referenceResolution.x, 1f - scaler.matchWidthOrHeight) *
                    Mathf.Pow(height / scaler.referenceResolution.y, scaler.matchWidthOrHeight);
            }
        }
        camera.targetTexture = target;
        foreach (Canvas canvas in canvases)
            foreach (Text label in canvas.GetComponentsInChildren<Text>(true)) label.SetAllDirty();
        Canvas.ForceUpdateCanvases();
        // Target texture may have a different aspect from the Editor Game view.
        // Reproject world-attached overlays after the capture camera is configured.
        Object.FindFirstObjectByType<FestivalDemonstrationCue>()?.SendMessage("LateUpdate", SendMessageOptions.DontRequireReceiver);
        Object.FindFirstObjectByType<RainbowChainPreview>()?.SendMessage("LateUpdate", SendMessageOptions.DontRequireReceiver);
        Object.FindFirstObjectByType<PopLightFeedback>()?.Reproject();
        camera.Render();
        RenderTexture.active = target;
        Texture2D capture = new Texture2D(width, height, TextureFormat.RGB24, false);
        capture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        capture.Apply();
        System.IO.Directory.CreateDirectory("Logs");
        System.IO.File.WriteAllBytes("Logs/" + name + ".png", capture.EncodeToPNG());
        camera.targetTexture = previous;
        if (cameraData != null) cameraData.renderPostProcessing = post;
        RenderTexture.active = active;
        foreach (Canvas canvas in canvases)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null) scaler.enabled = true;
        }
        Object.DestroyImmediate(capture);
        target.Release();
        Object.DestroyImmediate(target);
    }

    private static void QueueKeyboardState(params Key[] keys)
    {
        InputSystem.QueueStateEvent(
            syntheticKeyboard,
            new KeyboardState(keys));
        InputSystem.Update();
    }

    private static void InvokeUpdate(object target)
    {
        MethodInfo update = target.GetType().GetMethod(
            "Update",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Require(update != null,
            $"Could not locate {target.GetType().Name}.Update.");
        update.Invoke(target, null);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
#endif
