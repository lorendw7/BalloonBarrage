using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using BalloonBarrage.Foundation;

// Batch asset maintenance. Preserves Text component IDs and gameplay/UI callback interfaces.
public static class UiResolutionClarityPass
{
    const string Root = "Assets/StudioArtPack/InterfaceV2";
    const string Output = "Logs/UiClarity";
    static readonly string[] Screens = { "StartScreen", "PauseScreen", "ResultsScreen", "InstructionsScreen", "GameplayHUD" };
    static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080), new Vector2Int(2560, 1440), new Vector2Int(1280, 960), new Vector2Int(3440, 1440) };

    public static void RunBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run in an isolated batch copy.");
        Directory.CreateDirectory(Output);
        var legacy = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        Debug.Log("UI_FONT_BASELINE: name=" + legacy.name + ", dynamic=" + legacy.dynamic + ", rasterSize=" + legacy.fontSize);
        if (!File.Exists(Output + "/Before_Start_1440p.png"))
            Capture("StartScreen", new Vector2Int(2560, 1440), "Before_Start_1440p");

        foreach (string name in new[] { "Lato-Regular", "Lato-Bold" })
        {
            string path = Root + "/Fonts/" + name + ".ttf";
            var importer = (TrueTypeFontImporter)AssetImporter.GetAtPath(path);
            importer.fontTextureCase = FontTextureCase.Dynamic;
            importer.includeFontData = true;
            importer.SaveAndReimport();
        }
        var regular = AssetDatabase.LoadAssetAtPath<Font>(Root + "/Fonts/Lato-Regular.ttf");
        var bold = AssetDatabase.LoadAssetAtPath<Font>(Root + "/Fonts/Lato-Bold.ttf");
        if (regular == null || bold == null || !regular.dynamic || !bold.dynamic)
            throw new InvalidOperationException("Expected included dynamic TrueType fonts.");
        foreach (string name in Screens)
        {
            string path = Root + "/Prefabs/" + name + ".prefab";
            var prefab = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (prefab.GetComponent<CanvasTextResolutionRefresh>() == null)
                    prefab.AddComponent<CanvasTextResolutionRefresh>();
                foreach (var text in prefab.GetComponentsInChildren<Text>(true))
                {
                    bool wantsBold = text.font == bold || text.fontStyle == FontStyle.Bold || text.fontStyle == FontStyle.BoldAndItalic;
                    text.font = wantsBold ? bold : regular;
                    if (wantsBold) text.fontStyle = text.fontStyle == FontStyle.BoldAndItalic ? FontStyle.Italic : FontStyle.Normal;
                    text.resizeTextForBestFit = false;
                }
                PrefabUtility.SaveAsPrefabAsset(prefab, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }
        foreach (string name in Screens)
            foreach (var size in Sizes) Capture(name, size, name + "_" + size.x + "x" + size.y);
        AssetDatabase.SaveAssets();
        Debug.Log("UI_CLARITY_OK: 5 prefabs, dynamic fonts; 25 resolution/text-fit renders.");
    }

    static void Capture(string name, Vector2Int size, string label)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("UiPreviewCamera").AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.2f, .25f, .23f);
        camera.nearClipPlane = .01f; camera.farClipPlane = 10;
        var data = camera.GetUniversalAdditionalCameraData(); data.renderPostProcessing = false;
        var target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
        var pixels = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
        RenderTexture previous = RenderTexture.active;
        bool previousAsync = ShaderUtil.allowAsyncCompilation;
        GameObject root = null;
        try
        {
            ShaderUtil.allowAsyncCompilation = false;
            target.Create(); camera.targetTexture = target; camera.aspect = (float)size.x / size.y;
            root = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/" + name + ".prefab"));
            root.SetActive(true);
            var canvas = root.GetComponent<Canvas>();
            var scaler = root.GetComponent<CanvasScaler>();
            if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                throw new InvalidOperationException("Expected adaptive Canvas Scaler: " + name);
            var reference = scaler.referenceResolution;
            float match = scaler.matchWidthOrHeight;
            scaler.enabled = false;
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
            canvas.scaleFactor = Mathf.Pow(size.x / reference.x, 1 - match) * Mathf.Pow(size.y / reference.y, match);
            Canvas.ForceUpdateCanvases();
            foreach (var text in root.GetComponentsInChildren<Text>(true))
            {
                if (label.StartsWith("Before_")) continue;
                if (text.preferredHeight > text.rectTransform.rect.height + 1 || text.preferredWidth > text.rectTransform.rect.width + 1)
                    throw new InvalidOperationException("Text overflow: " + label + "/" + text.name + " preferred=" + text.preferredWidth + "x" + text.preferredHeight + " available=" + text.rectTransform.rect.size);
                if (!text.font.dynamic) throw new InvalidOperationException("Non-dynamic text font: " + text.name);
                var corners = new Vector3[4]; text.rectTransform.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    Vector3 point = camera.WorldToViewportPoint(corner);
                    if (point.x < -.002 || point.x > 1.002 || point.y < -.002 || point.y > 1.002)
                        throw new InvalidOperationException("Text outside viewport: " + label + "/" + text.name);
                }
            }
            Canvas.ForceUpdateCanvases(); camera.Render();
            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
            if (RenderPipeline.SupportsRenderRequest(camera, request)) RenderPipeline.SubmitRenderRequest(camera, request);
            Canvas.ForceUpdateCanvases(); camera.Render();
            RenderTexture.active = target;
            pixels.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0); pixels.Apply();
            File.WriteAllBytes(Output + "/" + label + ".png", pixels.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previous; camera.targetTexture = null;
            ShaderUtil.allowAsyncCompilation = previousAsync;
            if (root != null) Object.DestroyImmediate(root);
            Object.DestroyImmediate(camera.gameObject);
            target.Release(); Object.DestroyImmediate(target); Object.DestroyImmediate(pixels);
        }
    }
}
