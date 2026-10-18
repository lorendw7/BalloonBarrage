using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    // Adds presentation only. Learner-owned fields, save logic and UI binding are untouched.
    public static class BestScoreUiPreparation
    {
        const string Prefab = "Assets/StudioArtPack/InterfaceV2/Prefabs/ResultsScreen.prefab";
        const string Output = "Assets/StudioArtPack/InterfaceV2/Previews";
        static readonly Vector2Int[] Sizes = {
            new Vector2Int(1280, 720), new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440), new Vector2Int(1280, 960),
            new Vector2Int(3440, 1440)
        };

        static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }

        static RectTransform CreateRect(Transform parent, string name, Vector2 position, Vector2 size)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.layer = 5;
            var rect = obj.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one * .5f;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        static Text Label(Transform parent, string name, string copy, Vector2 position,
            Vector2 size, Font font, int fontSize, Color color, TextAnchor alignment)
        {
            var rect = CreateRect(parent, name, position, size);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.resizeTextForBestFit = false;
            text.supportRichText = false;
            text.text = copy;
            return text;
        }

        [MenuItem("Tools/Balloon Studio/Prepare Best Score Results UI")]
        public static void Build()
        {
            Require(!EditorApplication.isPlayingOrWillChangePlaymode, "Leave Play Mode first.");
            var root = PrefabUtility.LoadPrefabContents(Prefab);
            try
            {
                var dialog = root.transform.Find("Dialog");
                Require(dialog != null, "Missing current results dialog.");
                if (dialog.Find("BestScoreRow") != null)
                {
                    Debug.Log("BEST_SCORE_UI_ALREADY_PREPARED: inspect the existing row before editing it.");
                    return;
                }
                var value = dialog.Find("FinalScorePlaque/FinalScoreValue").GetComponent<Text>();
                var caption = dialog.Find("FinalScorePlaque/ScoreCaption").GetComponent<Text>();
                var snapshots = root.GetComponentsInChildren<Component>(true)
                    .Where(c => c != null && c != dialog).ToDictionary(c => c, c => EditorJsonUtility.ToJson(c));
                var row = CreateRect(dialog, "BestScoreRow", new Vector2(0, -64), new Vector2(350, 26));
                Label(row, "BestScoreCaption", "BEST SCORE", new Vector2(-81, 0),
                    new Vector2(166, 26), value.font, 18, caption.color, TextAnchor.MiddleLeft);
                Label(row, "BestScoreValue", "------", new Vector2(90, 0),
                    new Vector2(146, 26), value.font, 21, value.color, TextAnchor.MiddleRight);
                foreach (var item in snapshots)
                    Require(EditorJsonUtility.ToJson(item.Key) == item.Value,
                        "An existing UI component changed: " + item.Key.name + "/" + item.Key.GetType().Name);
                PrefabUtility.SaveAsPrefabAsset(root, Prefab);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            AssetDatabase.SaveAssets();
            Debug.Log("BEST_SCORE_UI_PREPARED: ResultsScreen/Dialog/BestScoreRow/BestScoreValue; placeholder only.");
        }

        public static void VerifyBatch()
        {
            Require(Application.isBatchMode, "Verify in an isolated batch project.");
            Build();
            Directory.CreateDirectory(Output);
            foreach (var size in Sizes) VerifySize(size);
            // Confirm the production scene inherits the new row without saving or changing callbacks.
            EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity", OpenSceneMode.Single);
            var controller = Object.FindFirstObjectByType<RoundController>();
            var serialized = new SerializedObject(controller);
            var screen = (GameObject)serialized.FindProperty("resultsScreen").objectReferenceValue;
            Require(screen != null && screen.transform.Find("Dialog/BestScoreRow/BestScoreValue") != null,
                "Production scene did not inherit the new best-score label.");
            var button = screen.GetComponentInChildren<Button>(true);
            Require(button.onClick.GetPersistentMethodName(0) == "RestartRound" &&
                button.onClick.GetPersistentTarget(0) == controller, "Saved retry callback changed.");
            Debug.Log("BEST_SCORE_UI_VERIFY_OK: 5 resolutions; text fit and nonoverlap; existing component state preserved; " +
                "production inherited label and retry callback checked; no gameplay binding or PlayerPrefs writes.");
        }

        static void VerifySize(Vector2Int size)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var camera = new GameObject("BestScorePreviewCamera").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.16f, .22f, .20f);
            camera.nearClipPlane = .01f;
            camera.farClipPlane = 10;
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = false;
            var root = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Prefab));
            root.SetActive(true);
            var canvas = root.GetComponent<Canvas>();
            var scaler = root.GetComponent<CanvasScaler>();
            var reference = scaler.referenceResolution;
            float match = scaler.matchWidthOrHeight;
            scaler.enabled = false;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            canvas.scaleFactor = Mathf.Pow(size.x / reference.x, 1 - match) * Mathf.Pow(size.y / reference.y, match);
            camera.aspect = (float)size.x / size.y;
            var dialog = root.transform.Find("Dialog");
            var row = (RectTransform)dialog.Find("BestScoreRow");
            var value = row.Find("BestScoreValue").GetComponent<Text>();
            Require(value.text == "------" && !value.raycastTarget, "Unexpected saved label state.");
            // A representative six-digit record, for layout previews only; never a saved score.
            value.text = "001860";
            Canvas.ForceUpdateCanvases();
            foreach (var text in row.GetComponentsInChildren<Text>())
            {
                Require(text.font.dynamic && !text.raycastTarget, "Font/raycast configuration failed.");
                Require(text.preferredWidth <= text.rectTransform.rect.width + 1 &&
                    text.preferredHeight <= text.rectTransform.rect.height + 1, "Best-score text overflow.");
                var corners = new Vector3[4];
                text.rectTransform.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var p = camera.WorldToViewportPoint(corner);
                    Require(p.x >= 0 && p.x <= 1 && p.y >= 0 && p.y <= 1, "Best score outside viewport.");
                }
            }
            Rect InDialog(RectTransform rect)
            {
                var corners = new Vector3[4]; rect.GetWorldCorners(corners);
                var points = corners.Select(c => dialog.InverseTransformPoint(c)).ToArray();
                return Rect.MinMaxRect(points.Min(p => p.x), points.Min(p => p.y),
                    points.Max(p => p.x), points.Max(p => p.y));
            }
            var rowBounds = InDialog(row);
            Require(!rowBounds.Overlaps(InDialog((RectTransform)dialog.Find("FinalScorePlaque"))) &&
                !rowBounds.Overlaps(InDialog((RectTransform)dialog.Find("PlayAgainButton"))), "Best-score row overlaps existing UI.");
            if (size.x == 1920 && size.y == 1080)
            {
                var target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
                var pixels = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
                var previous = RenderTexture.active;
                camera.targetTexture = target;
                Canvas.ForceUpdateCanvases(); camera.Render();
                var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
                if (UnityEngine.Rendering.RenderPipeline.SupportsRenderRequest(camera, request))
                    UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera, request);
                Canvas.ForceUpdateCanvases(); camera.Render();
                RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0); pixels.Apply();
                File.WriteAllBytes(Output + "/Results_BestScore_1080p.png", pixels.EncodeToPNG());
                RenderTexture.active = previous;
                camera.targetTexture = null;
                target.Release(); Object.DestroyImmediate(target); Object.DestroyImmediate(pixels);
            }
            Object.DestroyImmediate(root);
            Object.DestroyImmediate(camera.gameObject);
        }
    }
}
