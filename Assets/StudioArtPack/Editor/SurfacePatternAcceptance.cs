using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    // Asset-only validation and a temporary native render. No runtime selector or scene save.
    public static class SurfacePatternAcceptance
    {
        static readonly string[] Colors = { "Teal", "Coral", "Gold", "SkyBlue", "Violet", "Rose", "Lime", "Tangerine" };
        static readonly string[] Styles = { "Spray", "Burst", "Drip", "Swoosh", "Brush", "Starburst" };
        static void Require(bool value, string message)
        {
            if (!value) throw new InvalidOperationException(message);
        }
        static void Label(string text, Vector3 position, float size)
        {
            var label = new GameObject(text).AddComponent<TextMesh>();
            label.text = text; label.anchor = TextAnchor.MiddleCenter;
            label.fontSize = 64; label.characterSize = size;
            label.color = new Color(.15f, .23f, .21f);
            label.transform.position = position;
        }
        public static void Run()
        {
            Require(Application.isBatchMode, "Run in an isolated batch project.");
            try
            {
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var camera = new GameObject("PatternPreviewCamera").AddComponent<Camera>();
                camera.transform.position = new Vector3(0, 0, -12);
                camera.orthographic = true; camera.orthographicSize = 6.4f;
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(.97f, .95f, .89f);
                camera.GetUniversalAdditionalCameraData().renderPostProcessing = false;
                Label("SIX PATTERNS / EIGHT PIGMENTS", new Vector3(0, 5.8f, 0), .055f);
                var balloon = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab");
                var pigments = balloon.GetComponent<StudioBalloonAppearance>().pigments;
                var random = new System.Random(20261019);
                var sprites = new Sprite[6];
                for (int color = 0; color < Colors.Length; color++)
                {
                    float x = -5.8f + color * 1.7f;
                    Label(Colors[color], new Vector3(x, 4.95f, 0), .03f);
                    for (int style = 0; style < Styles.Length; style++)
                    {
                        var path = "Assets/StudioArtPack/SurfacePaint/Patterns/SurfacePaint_" + Colors[color] + "_" + Styles[style] + ".prefab";
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        Require(prefab != null, "Missing " + path);
                        Require(prefab.layer == 2 && prefab.GetComponentsInChildren<Collider>(true).Length == 0 &&
                            prefab.GetComponentsInChildren<MonoBehaviour>(true).Length == 0, "Unsafe pattern components: " + path);
                        Require(Vector3.Distance(prefab.transform.localScale, Vector3.one * .18f) < .0001f, "Changed production mark scale.");
                        var sprite = prefab.GetComponent<SpriteRenderer>();
                        Require(sprite != null && sprite.sprite != null && Mathf.Abs(sprite.color.a - .9f) < .0001f, "Sprite/alpha missing.");
                        Require(!ShaderUtil.ShaderHasError(sprite.sharedMaterial.shader), "Paint shader error.");
                        Require(Vector4.Distance(sprite.sharedMaterial.GetColor("_Pigment"), pigments[color].GetColor("_BaseColor")) < .001f,
                            "Pattern pigment mismatch: " + path);
                        if (color == 0) sprites[style] = sprite.sprite;
                        else Require(sprite.sprite == sprites[style], "Pattern ordering mismatch.");
                        var instance = Object.Instantiate(prefab);
                        float y = 3.8f - style * 1.45f;
                        instance.transform.position = new Vector3(x, y, 0);
                        instance.transform.localScale = Vector3.one * (.31f + (float)random.NextDouble() * .045f);
                        float angle = ((float)random.NextDouble() * 2 - 1) * (style == 2 ? 4 : 22);
                        instance.transform.rotation = Quaternion.Euler(0, 0, angle);
                        if (color == 0) Label(Styles[style], new Vector3(-7.25f, y, 0), .027f);
                    }
                }
                for (int i = 0; i < sprites.Length; i++)
                    for (int j = i + 1; j < sprites.Length; j++) Require(sprites[i] != sprites[j], "Duplicate silhouettes.");
                Label("ASSET PREVIEW / RUNTIME RANDOM SELECTION IS NOT IMPLEMENTED", new Vector3(0, -5.25f, 0), .027f);
                int frames = 0;
                EditorApplication.CallbackFunction render = null;
                render = () =>
                {
                    if (++frames < 20) return;
                    EditorApplication.update -= render;
                    try
                    {
                        var target = new RenderTexture(1600, 1280, 24, RenderTextureFormat.ARGB32);
                        camera.targetTexture = target;
                        RenderPipeline.SubmitRenderRequest(camera, new UniversalRenderPipeline.SingleCameraRequest { destination = target });
                        RenderTexture.active = target;
                        var pixels = new Texture2D(1600, 1280, TextureFormat.RGB24, false);
                        pixels.ReadPixels(new Rect(0, 0, 1600, 1280), 0, 0); pixels.Apply();
                        var output = Environment.GetEnvironmentVariable("BALLOON_PATTERN_PREVIEW");
                        Require(!string.IsNullOrEmpty(output), "Set BALLOON_PATTERN_PREVIEW.");
                        File.WriteAllBytes(output, pixels.EncodeToPNG());
                        Debug.Log("SURFACE_PATTERNS_OK: 48 assets, 6 unique silhouettes, matching pigment/alpha/scale, no colliders/scripts; native preview only.");
                        EditorApplication.Exit(0);
                    }
                    catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
                };
                EditorApplication.update += render;
            }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
        }
    }
}
