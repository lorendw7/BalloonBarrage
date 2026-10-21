using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Presentation-only refinement. Runtime timing and fill integration remain learner-owned.
public static class BeatCuePaintStylePreparation
{
    private const string Cue =
        "Assets/StudioArtPack/InterfaceV2/Prefabs/RhythmCue.prefab";

    private const string BurstSource =
        "Assets/StudioArtPack/SurfacePaint/Patterns/SurfacePaint_Teal_Starburst.prefab";

    public const string Root =
        "Assets/StudioArtPack/BeatCuePaint20261021";

    private static Color Teal => new Color(0.04f, 0.45f, 0.43f, 0.98f);
    private static Color DeepInk => new Color(0.06f, 0.16f, 0.17f, 1f);
    private static Color Cream => new Color(1f, 0.96f, 0.82f, 0.98f);
    private static Color Coral => new Color(0.94f, 0.34f, 0.30f, 1f);
    private static Color Gold => new Color(1f, 0.72f, 0.16f, 1f);

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static GameObject Child(
        Transform parent,
        string name,
        Type component,
        Vector2 position,
        Vector2 size)
    {
        var gameObject = new GameObject(
            name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            component);

        gameObject.layer = 5;

        var rect = (RectTransform)gameObject.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * 0.5f;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        gameObject.GetComponent<Graphic>().raycastTarget = false;
        return gameObject;
    }

    private static Image ImageChild(
        Transform parent,
        string name,
        Vector2 position,
        Vector2 size,
        Color color)
    {
        var image = Child(parent, name, typeof(Image), position, size)
            .GetComponent<Image>();

        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(
            "UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = color;
        return image;
    }

    private static void StyleText(
        Text text,
        string copy,
        int size,
        Color color)
    {
        text.text = copy;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.raycastTarget = false;
        text.supportRichText = false;
        text.resizeTextForBestFit = false;
    }

    public static void Run()
    {
        try
        {
            Require(
                Application.isBatchMode &&
                Application.dataPath.Contains(".local-backups"),
                "Run this preparation only in an isolated batch copy.");

            Require(!Directory.Exists(Root), "Preserve delivered paint-cue assets.");
            Directory.CreateDirectory(Root + "/Previews");
            AssetDatabase.Refresh();

            RefineLiveCue();
            CreateApproachGuide();
            AssetDatabase.SaveAssets();
            VerifyAndCapture();

            File.WriteAllText(
                Root + "/Verification.txt",
                "BEAT_CUE_PAINT_STYLE_OK\n" +
                "RhythmCue GUID and existing BeatCueText binding retained.\n" +
                "Project-owned paint sprite reused as an original teal sticker silhouette.\n" +
                "No learner scripts, judgement values, chart timing or scoring changed.\n" +
                "Separate approach guide remains disconnected from production gameplay.\n" +
                "Layout and non-raycast checks passed at 1280x720, 1920x1080 and 2560x1440.\n" +
                "Preview fill is demonstration data, not a runtime timing test.\n");

            AssetDatabase.Refresh();
            Debug.Log("BEAT_CUE_PAINT_STYLE_OK");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }

    private static void RefineLiveCue()
    {
        var root = PrefabUtility.LoadPrefabContents(Cue);

        try
        {
            Require(
                root.transform.Find("PaintCueCard") == null,
                "The paint-style refinement is already present.");

            var rect = (RectTransform)root.transform;
            rect.anchoredPosition = new Vector2(0f, 96f);
            rect.sizeDelta = new Vector2(480f, 118f);

            var paintSource = AssetDatabase.LoadAssetAtPath<GameObject>(BurstSource);
            Require(paintSource != null, "Paint burst source is missing.");

            var paintSprite = paintSource.GetComponent<SpriteRenderer>()?.sprite;
            Require(paintSprite != null, "Paint burst sprite is missing.");

            var background = root.GetComponent<Image>();
            Require(background != null, "RhythmCue root Image is missing.");
            background.sprite = paintSprite;
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = Teal;
            background.raycastTarget = false;

            var rootOutline = root.GetComponent<Outline>();
            Require(rootOutline != null, "Existing cue outline is missing.");
            rootOutline.effectColor = new Color(0.02f, 0.12f, 0.13f, 0.72f);
            rootOutline.effectDistance = new Vector2(2f, -2f);

            var card = ImageChild(
                root.transform,
                "PaintCueCard",
                new Vector2(0f, -1f),
                new Vector2(388f, 80f),
                Cream);
            card.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -1.4f);
            card.transform.SetAsFirstSibling();

            var cardOutline = card.gameObject.AddComponent<Outline>();
            cardOutline.effectColor = new Color(0.02f, 0.18f, 0.19f, 0.85f);
            cardOutline.effectDistance = new Vector2(2f, -2f);

            var goldSlash = ImageChild(
                root.transform,
                "GoldTimingSlash",
                new Vector2(188f, 0f),
                new Vector2(9f, 78f),
                Gold);
            goldSlash.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 8f);

            var coralDrop = ImageChild(
                root.transform,
                "CoralDrop",
                new Vector2(-196f, 27f),
                new Vector2(18f, 18f),
                Coral);
            coralDrop.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 18f);

            var goldDrop = ImageChild(
                root.transform,
                "GoldDrop",
                new Vector2(-210f, 3f),
                new Vector2(11f, 11f),
                Gold);

            var heading = root.transform.Find("BeatCueHeading")?.GetComponent<Text>();
            var cueText = root.transform.Find("BeatCueText")?.GetComponent<Text>();
            Require(heading != null && cueText != null, "Existing cue text is missing.");

            heading.rectTransform.anchoredPosition = new Vector2(-14f, 23f);
            heading.rectTransform.sizeDelta = new Vector2(330f, 20f);
            StyleText(heading, "OPTIONAL  BEAT  BONUS", 13, Coral);

            cueText.rectTransform.anchoredPosition = new Vector2(-14f, -10f);
            cueText.rectTransform.sizeDelta = new Vector2(340f, 56f);
            StyleText(cueText, "READY", 34, DeepInk);
            cueText.transform.SetAsLastSibling();
            heading.transform.SetAsLastSibling();

            PrefabUtility.SaveAsPrefabAsset(root, Cue);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void CreateApproachGuide()
    {
        var cueAsset = AssetDatabase.LoadAssetAtPath<GameObject>(Cue);
        var guide = Object.Instantiate(cueAsset);
        guide.name = "PaintBeatApproachGuide";

        try
        {
            Object.DestroyImmediate(guide.transform.Find("BeatCueText").gameObject);

            var track = ImageChild(
                guide.transform,
                "ApproachTrack",
                new Vector2(-14f, -8f),
                new Vector2(316f, 18f),
                new Color(0.82f, 0.85f, 0.75f, 1f));

            var fill = ImageChild(
                track.transform,
                "BeatApproachFill",
                Vector2.zero,
                new Vector2(316f, 18f),
                Teal);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0f;

            var gate = ImageChild(
                guide.transform,
                "TimingGate",
                new Vector2(145f, -8f),
                new Vector2(7f, 42f),
                Gold);
            gate.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 5f);

            var hint = Child(
                guide.transform,
                "TimingHint",
                typeof(Text),
                new Vector2(-14f, -38f),
                new Vector2(350f, 18f)).GetComponent<Text>();
            hint.font = guide.transform.Find("BeatCueHeading").GetComponent<Text>().font;
            StyleText(hint, "FIRE WHEN THE PAINT HITS GOLD", 10, DeepInk);

            PrefabUtility.SaveAsPrefabAsset(
                guide,
                Root + "/PaintBeatApproachGuide.prefab");
        }
        finally
        {
            Object.DestroyImmediate(guide);
        }
    }

    private static void VerifyAndCapture()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var host = new GameObject(
            "PreviewCanvas",
            typeof(Canvas),
            typeof(CanvasScaler));

        var canvas = host.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        var camera = new GameObject("PreviewCamera", typeof(Camera))
            .GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.17f, 0.24f, 0.22f);
        canvas.worldCamera = camera;

        var scaler = host.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        var paths = new[]
        {
            Cue,
            Root + "/PaintBeatApproachGuide.prefab"
        };

        foreach (var path in paths)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Require(asset != null, "Prepared cue asset is missing: " + path);

            var instance = Object.Instantiate(asset, host.transform, false);
            Require(
                instance.transform.Find("PaintCueCard") != null,
                "Paint cue card is missing.");

            foreach (var size in new[]
            {
                new Vector2Int(1280, 720),
                new Vector2Int(1920, 1080),
                new Vector2Int(2560, 1440)
            })
            {
                var target = new RenderTexture(size.x, size.y, 24);
                target.Create();
                Require(
                    target.IsCreated(),
                    $"Preview render target could not be created at {size.x}x{size.y}.");
                camera.targetTexture = target;
                canvas.planeDistance = 1f;

                var cueText = instance.transform.Find("BeatCueText")?.GetComponent<Text>();
                if (cueText != null)
                {
                    cueText.text = "NEXT 0.8s";
                }

                var fill = instance.transform.Find(
                    "ApproachTrack/BeatApproachFill")?.GetComponent<Image>();
                if (fill != null)
                {
                    fill.fillAmount = 0.72f;
                }

                Canvas.ForceUpdateCanvases();

                foreach (var text in instance.GetComponentsInChildren<Text>())
                {
                    Require(text.font != null, "Cue font is missing: " + text.name);
                    Require(text.font.HasCharacter('0'), "Cue font lacks digits.");
                    Require(
                        text.preferredWidth <= text.rectTransform.rect.width + 1f &&
                        text.preferredHeight <= text.rectTransform.rect.height + 1f,
                        "Text overflow: " + text.name);
                }

                if (cueText != null)
                {
                    var textMesh = cueText.canvasRenderer.GetMesh();
                    Require(
                        textMesh != null && textMesh.vertexCount > 0,
                        "Live cue text produced no render geometry.");
                }

                Require(
                    instance.GetComponentsInChildren<Graphic>()
                        .All(graphic => !graphic.raycastTarget),
                    "Cue UI intercepts pointer input.");

                camera.Render();

                if (size.x == 1920)
                {
                    var previous = RenderTexture.active;
                    RenderTexture.active = target;
                    var image = new Texture2D(
                        size.x,
                        size.y,
                        TextureFormat.RGB24,
                        false);
                    image.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
                    image.Apply();

                    var name = path == Cue
                        ? "PaintCueLive"
                        : "PaintApproachPrepared";
                    File.WriteAllBytes(
                        Root + "/Previews/" + name + ".png",
                        image.EncodeToPNG());

                    Object.DestroyImmediate(image);
                    RenderTexture.active = previous;
                }

                camera.targetTexture = null;
                target.Release();
                Object.DestroyImmediate(target);
            }

            Object.DestroyImmediate(instance);
        }

        Object.DestroyImmediate(host);
        Object.DestroyImmediate(camera.gameObject);

        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        var reader = Object.FindFirstObjectByType<RhythmChartReader>();
        Require(reader != null, "Production RhythmChartReader is missing.");

        var serializedReader = new SerializedObject(reader);
        var cue = (Text)serializedReader.FindProperty("beatCueText")
            .objectReferenceValue;
        Require(cue != null, "Production beatCueText binding is missing.");
        Require(
            cue.transform.parent.Find("PaintCueCard") != null,
            "Production scene did not inherit the refined cue.");
    }
}
