using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    /// <summary>
    /// Renders the saved Simplified Chinese start screen and verifies that the
    /// contrast-specific font and colors are active at exhibition resolutions.
    /// </summary>
    public static class LocalizationPreviewCapture
    {
        private const string PrefabPath =
            "Assets/StudioArtPack/InterfaceV2/Prefabs/StartScreen.prefab";
        private const string OutputFolder =
            "Assets/StudioArtPack/InterfaceV2/Previews";

        public static void RunBatch()
        {
            try
            {
                Capture(new Vector2Int(1920, 1080),
                    "Start_Localized_ZhHans_1080p.png");
                Capture(new Vector2Int(2560, 1440),
                    "Start_Localized_ZhHans_1440p.png");
                AssetDatabase.Refresh();
                Debug.Log(
                    "LOCALIZATION_VISUAL_OK: Simplified Chinese Medium font, " +
                    "contrast colors, 1920x1080 and 2560x1440 renders passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void Capture(Vector2Int size, string fileName)
        {
            GameObject root = null;
            Camera camera = null;
            RenderTexture target = null;
            Texture2D pixels = null;
            RenderTexture previous = RenderTexture.active;
            bool previousAsync = ShaderUtil.allowAsyncCompilation;
            FieldInfo languageField = typeof(LanguageSettings).GetField(
                "<Current>k__BackingField",
                BindingFlags.NonPublic | BindingFlags.Static);
            UILanguage originalLanguage = LanguageSettings.Current;

            try
            {
                Require(languageField != null,
                    "Current-language backing field changed.");
                languageField.SetValue(null, UILanguage.SimplifiedChinese);
                ShaderUtil.allowAsyncCompilation = false;

                camera = new GameObject("LocalizationPreviewCamera")
                    .AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.2f, 0.25f, 0.23f);
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 10f;
                camera.GetUniversalAdditionalCameraData()
                    .renderPostProcessing = false;

                target = new RenderTexture(
                    size.x, size.y, 24, RenderTextureFormat.ARGB32);
                pixels = new Texture2D(
                    size.x, size.y, TextureFormat.RGB24, false);
                target.Create();
                camera.targetTexture = target;
                camera.aspect = (float)size.x / size.y;

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    PrefabPath);
                Require(prefab != null, "Missing start-screen prefab.");
                root = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                root.SetActive(true);
                PrepareLocalization(root);
                ValidateChinesePresentation(root);

                Canvas canvas = root.GetComponent<Canvas>();
                CanvasScaler scaler = root.GetComponent<CanvasScaler>();
                Require(canvas != null && scaler != null,
                    "Start screen requires Canvas and CanvasScaler.");
                Vector2 reference = scaler.referenceResolution;
                float match = scaler.matchWidthOrHeight;
                scaler.enabled = false;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 1f;
                canvas.scaleFactor =
                    Mathf.Pow(size.x / reference.x, 1f - match) *
                    Mathf.Pow(size.y / reference.y, match);

                Canvas.ForceUpdateCanvases();
                foreach (Text label in root.GetComponentsInChildren<Text>(true))
                {
                    Require(
                        label.preferredHeight <=
                        label.rectTransform.rect.height + 1f &&
                        label.preferredWidth <=
                        label.rectTransform.rect.width + 1f,
                        $"Text overflow at {size.x}x{size.y}: {label.name}.");
                }

                camera.Render();
                var request = new UniversalRenderPipeline.SingleCameraRequest
                {
                    destination = target
                };
                if (RenderPipeline.SupportsRenderRequest(camera, request))
                {
                    RenderPipeline.SubmitRenderRequest(camera, request);
                }

                camera.Render();
                RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
                pixels.Apply();
                Directory.CreateDirectory(OutputFolder);
                File.WriteAllBytes(
                    Path.Combine(OutputFolder, fileName), pixels.EncodeToPNG());
            }
            finally
            {
                languageField?.SetValue(null, originalLanguage);
                RenderTexture.active = previous;
                ShaderUtil.allowAsyncCompilation = previousAsync;
                if (camera != null)
                {
                    camera.targetTexture = null;
                }
                if (root != null)
                {
                    Object.DestroyImmediate(root);
                }
                if (camera != null)
                {
                    Object.DestroyImmediate(camera.gameObject);
                }
                if (target != null)
                {
                    target.Release();
                    Object.DestroyImmediate(target);
                }
                if (pixels != null)
                {
                    Object.DestroyImmediate(pixels);
                }
            }
        }

        private static void PrepareLocalization(GameObject root)
        {
            LocalizationManager manager =
                root.GetComponent<LocalizationManager>();
            Require(manager != null, "Start screen lacks LocalizationManager.");
            InvokePrivate(manager, "LoadTable");

            foreach (LocalizedText localized in
                     root.GetComponentsInChildren<LocalizedText>(true))
            {
                InvokePrivate(localized, "Awake");
                InvokePrivate(localized, "RefreshText");
            }
        }

        private static void ValidateChinesePresentation(GameObject root)
        {
            Text body = root.GetComponentsInChildren<Text>(true)
                .Single(label => label.name == "ControlsText");
            Text subtitle = root.GetComponentsInChildren<Text>(true)
                .Single(label => label.name == "Subtitle");

            Require(body.font != null &&
                    body.font.name.Contains("NotoSansSC-Medium"),
                "Start-screen body does not use Noto Sans SC Medium.");
            Require(body.color.r <= 0.14f && body.color.g <= 0.18f,
                "Chinese instructional text is not dark enough.");
            Require(subtitle.color.r <= 0.26f && subtitle.color.g <= 0.22f,
                "Chinese secondary text is not dark enough.");
        }

        private static void InvokePrivate(object target, string methodName)
        {
            MethodInfo method = target.GetType().GetMethod(
                methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            Require(method != null,
                $"Missing {target.GetType().Name}.{methodName}.");
            method.Invoke(target, null);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
