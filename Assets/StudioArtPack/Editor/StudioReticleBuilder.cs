using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    // Presentation assets and explicit editor hookup only; no runtime aiming code.
    public static class StudioReticleBuilder
    {
        private const string Folder = "Assets/StudioArtPack/InterfaceV2/Reticle";
        private const string PrefabPath = Folder + "/AimReticle.prefab";

        [MenuItem("Tools/BalloonBarrage/Build Missing Aim Reticle Asset")]
        public static void BuildMissingAsset()
        {
            if (AssetDatabase.LoadMainAssetAtPath(PrefabPath) != null)
                throw new InvalidOperationException("Reticle already exists; preserve manual edits.");
            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets/StudioArtPack/InterfaceV2", "Reticle");
            var root = new GameObject("AimReticle", typeof(RectTransform));
            root.layer = 5;
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(36f, 36f);
            AddTick(rect, "Top", new Vector2(0f, 11f), new Vector2(2.5f, 7f));
            AddTick(rect, "Bottom", new Vector2(0f, -11f), new Vector2(2.5f, 7f));
            AddTick(rect, "Left", new Vector2(-11f, 0f), new Vector2(7f, 2.5f));
            AddTick(rect, "Right", new Vector2(11f, 0f), new Vector2(7f, 2.5f));
            root.SetActive(false);
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            AssetDatabase.SaveAssets();
        }

        private static void AddTick(RectTransform parent, string name, Vector2 position, Vector2 size)
        {
            var tick = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Outline));
            tick.layer = 5;
            var rect = tick.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            var image = tick.GetComponent<Image>();
            image.color = new Color(.98f, .94f, .83f, 1f);
            image.raycastTarget = false;
            var outline = tick.GetComponent<Outline>();
            outline.effectColor = new Color(.08f, .18f, .17f, 1f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);
            outline.useGraphicAlpha = true;
        }

        [MenuItem("Tools/BalloonBarrage/Connect Aim Reticle")]
        public static void ConnectCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            RectTransform reticle = FindReticle();
            if (reticle == null || reticle.gameObject.scene != scene)
            {
                Debug.LogWarning("Open the prepared PrototypeScene with its AimReticle scene instance.");
                return;
            }
            MonoBehaviour shooter = null;
            foreach (var behaviour in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (behaviour == null || behaviour.gameObject.scene != scene || behaviour.GetType().Name != "PlayerShooter") continue;
                if (shooter != null) throw new InvalidOperationException("Multiple shooters: select the intended solo scene first.");
                shooter = behaviour;
            }
            if (shooter == null) throw new InvalidOperationException("PlayerShooter is missing.");
            var serialized = new SerializedObject(shooter);
            var reference = serialized.FindProperty("aimReticle");
            if (reference == null)
            {
                Debug.LogWarning("First save and compile the learner's [SerializeField] private RectTransform aimReticle; field.");
                return;
            }
            if (reference.objectReferenceValue != null && reference.objectReferenceValue != reticle)
                throw new InvalidOperationException("Preserve the existing different reticle reference.");
            reference.objectReferenceValue = reticle;
            serialized.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeObject = reticle.gameObject;
            Debug.Log("AIM_RETICLE_CONNECTED: scene RectTransform assigned. Save the scene; runtime following remains learner code.");
        }

        // The production editor never automatically opens or saves a scene.
        public static void BuildAndPrepareBatch()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
            BuildMissingAsset();
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
            if (FindReticle() != null) throw new InvalidOperationException("Reticle scene instance already exists.");
            Canvas hud = null;
            foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (canvas.name == "GameplayHUD") hud = canvas;
            if (hud == null || hud.renderMode != RenderMode.ScreenSpaceOverlay)
                throw new InvalidOperationException("Expected the current Overlay GameplayHUD.");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            instance.transform.SetParent(hud.transform, false);
            instance.transform.SetAsLastSibling();
            var rect = instance.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            VerifyAndCaptureBatch();
        }

        private static RectTransform FindReticle()
        {
            RectTransform found = null;
            foreach (var rect in Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (rect.name != "AimReticle") continue;
                if (found != null) throw new InvalidOperationException("Duplicate AimReticle scene instance.");
                found = rect;
            }
            return found;
        }

        public static void VerifyAndCaptureBatch()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
            EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
            var rect = FindReticle();
            if (rect == null || rect.parent == null || rect.parent.name != "GameplayHUD" ||
                rect.gameObject.activeSelf || rect.localScale != Vector3.one ||
                rect.anchorMin != new Vector2(.5f, .5f) || rect.anchorMax != rect.anchorMin ||
                rect.anchoredPosition != Vector2.zero || rect.sizeDelta != new Vector2(36f, 36f))
                throw new InvalidOperationException("Unexpected initial reticle layout/state.");
            var images = rect.GetComponentsInChildren<Image>(true);
            if (images.Length != 4 || rect.GetComponentsInChildren<Outline>(true).Length != 4 ||
                rect.GetComponentsInChildren<Canvas>(true).Length != 0)
                throw new InvalidOperationException("Unexpected reticle hierarchy.");
            foreach (var image in images)
                if (image.raycastTarget || image.sprite != null) throw new InvalidOperationException("Expected nonblocking native UI shapes.");
            foreach (var behaviour in rect.GetComponentsInChildren<MonoBehaviour>(true))
                if (!(behaviour is Graphic) && !(behaviour is BaseMeshEffect))
                    throw new InvalidOperationException("Reticle must contain presentation components only.");
            Canvas hud = rect.GetComponentInParent<Canvas>(true);
            foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                canvas.gameObject.SetActive(canvas == hud);
            hud.gameObject.SetActive(true);
            rect.gameObject.SetActive(true);
            var camera = Camera.main;
            if (camera == null) throw new InvalidOperationException("Main camera missing.");
            string output = Folder + "/Previews";
            if (!AssetDatabase.IsValidFolder(output)) AssetDatabase.CreateFolder(Folder, "Previews");
            Capture(camera, hud, rect, 1920, 1080, output + "/Reticle_1080p.png");
            Capture(camera, hud, rect, 2560, 1440, output + "/Reticle_1440p.png");
            AssetDatabase.Refresh();
            Debug.Log("RETICLE_ASSETS_OK: hidden scene instance under Overlay HUD; four native outlined Images, no raycasts/runtime behaviour; 1080p/1440p render alignment. Following and cursor lifecycle are not integrated.");
        }

        private static void Capture(Camera camera, Canvas hud, RectTransform rect, int width, int height, string path)
        {
            var scaler = hud.GetComponent<CanvasScaler>();
            var target = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            var previous = RenderTexture.active;
            var previousTarget = camera.targetTexture;
            camera.targetTexture = target;
            camera.aspect = (float)width / height;
            var reference = scaler.referenceResolution;
            float match = scaler.matchWidthOrHeight;
            scaler.enabled = false;
            hud.renderMode = RenderMode.ScreenSpaceCamera;
            hud.worldCamera = camera;
            hud.planeDistance = 1f;
            hud.scaleFactor = Mathf.Pow(width / reference.x, 1f - match) * Mathf.Pow(height / reference.y, match);
            Canvas.ForceUpdateCanvases();
            // In this temporary camera preview, test the same screen point using projection.
            rect.position = camera.ScreenToWorldPoint(new Vector3(width * .5f, height * .5f, hud.planeDistance));
            Canvas.ForceUpdateCanvases();
            Vector3 screen = camera.WorldToScreenPoint(rect.position);
            if (Vector2.Distance(screen, new Vector2(width * .5f, height * .5f)) > .5f)
                throw new InvalidOperationException("Reticle preview center is misaligned.");
            camera.Render();
            RenderTexture.active = target;
            var pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
            pixels.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
            pixels.Apply();
            File.WriteAllBytes(path, pixels.EncodeToPNG());
            Object.DestroyImmediate(pixels);
            camera.targetTexture = previousTarget;
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(target);
        }
    }
}
