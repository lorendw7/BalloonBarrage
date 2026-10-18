using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace BalloonBarrage.EditorTools
{
    // Asset preparation only. No projectile callback or production-scene edits.
    public static class SurfacePaintLessonPreparation
    {
        const string Root = "Assets/StudioArtPack/SurfacePaint";
        const string Source = "Assets/StudioArtPack/SprayPrefabs/Teal_Spray.prefab";
        const string MaterialPath = Root + "/SurfacePaint_Teal.mat";
        const string PrefabPath = Root + "/SurfacePaint_Teal.prefab";

        [MenuItem("Tools/Balloon Studio/Prepare Surface Paint Lesson")]
        public static void Build()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Leave Play Mode before preparing assets.");
            Directory.CreateDirectory(Root + "/Previews");
            AssetDatabase.Refresh();
            var original = AssetDatabase.LoadAssetAtPath<GameObject>(Source);
            if (original == null) throw new InvalidOperationException("Missing original spray prefab.");
            var sourceRenderer = original.GetComponent<SpriteRenderer>();
            var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material == null)
            {
                material = new Material(sourceRenderer.sharedMaterial);
                material.name = "SurfacePaint_Teal";
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            var mark = new GameObject("SurfacePaint_Teal");
            try
            {
                mark.layer = LayerMask.NameToLayer("Ignore Raycast");
                mark.transform.localScale = Vector3.one * .18f;
                var sprite = mark.AddComponent<SpriteRenderer>();
                sprite.sprite = sourceRenderer.sprite;
                sprite.sharedMaterial = material;
                sprite.color = new Color(1f, 1f, 1f, .9f);
                sprite.shadowCastingMode = ShadowCastingMode.Off;
                sprite.receiveShadows = false;
                sprite.lightProbeUsage = LightProbeUsage.Off;
                sprite.reflectionProbeUsage = ReflectionProbeUsage.Off;
                PrefabUtility.SaveAsPrefabAsset(mark, PrefabPath);
            }
            finally { UnityEngine.Object.DestroyImmediate(mark); }
            AssetDatabase.SaveAssets();
            Debug.Log("SURFACE_PAINT_PREPARED: " + PrefabPath);
        }

        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        static void Capture(Camera camera, string path, int width, int height)
        {
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            var pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                pixels.Apply();
                File.WriteAllBytes(path, pixels.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                target.Release();
                UnityEngine.Object.DestroyImmediate(target);
                UnityEngine.Object.DestroyImmediate(pixels);
            }
        }

        // Run in the ignored isolated copy. Preview objects are never saved to a scene.
        public static void VerifyBatch()
        {
            Require(Application.isBatchMode, "Use an isolated batch project for verification.");
            Build();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Require(prefab.GetComponentsInChildren<Collider>(true).Length == 0, "Mark has a collider.");
            Require(prefab.GetComponentsInChildren<MonoBehaviour>(true).Length == 0, "Mark has runtime scripts.");
            Require(prefab.layer == 2, "Mark must use Ignore Raycast.");
            var sprite = prefab.GetComponent<SpriteRenderer>();
            Require(sprite.sprite != null && sprite.sharedMaterial != null, "Missing sprite/material.");
            Require(sprite.sharedMaterial.shader.isSupported, "Unsupported paint shader.");
            Require(!ShaderUtil.ShaderHasError(sprite.sharedMaterial.shader), "Paint shader errors.");
            Require(sprite.shadowCastingMode == ShadowCastingMode.Off && !sprite.receiveShadows,
                "Paint must not add shadow work.");
            Require(Vector2.Distance(sprite.sprite.rect.size, new Vector2(512, 512)) < .01f,
                "Unexpected atlas slice size.");

            EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity", OpenSceneMode.Single);
            Physics.SyncTransforms();
            var camera = Camera.main;
            Require(camera != null, "Missing production camera.");
            var canvases = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (var canvas in canvases) canvas.enabled = false;
            var ray = camera.ViewportPointToRay(new Vector3(.5f, .6f, 0));
            Require(Physics.Raycast(ray, out RaycastHit hit, 100f, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore), "Center wall ray missed.");
            Require(hit.collider.name == "MainWall", "Expected MainWall, got " + hit.collider.name);
            Require(!hit.collider.isTrigger, "Wall collider is a trigger.");
            var point = hit.point + hit.normal * .005f;
            var mark = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            mark.transform.SetPositionAndRotation(point, Quaternion.LookRotation(hit.normal));
            Physics.SyncTransforms();
            Require(Physics.Raycast(ray, out RaycastHit after, 100f, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore) && after.collider == hit.collider &&
                Mathf.Abs(after.distance - hit.distance) < .0001f, "Paint blocked a subsequent ray.");
            Require(Vector3.Dot(mark.transform.forward, hit.normal) > .9999f, "Incorrect surface alignment.");
            var bounds = mark.GetComponent<SpriteRenderer>().bounds;
            Require(bounds.size.magnitude < .7f && bounds.size.magnitude > .4f, "Unexpected mark size.");
            var corners = new[] { new Vector3(-.2304f, -.2304f), new Vector3(-.2304f, .2304f),
                new Vector3(.2304f, -.2304f), new Vector3(.2304f, .2304f) };
            foreach (var corner in corners)
            {
                var onMark = point + mark.transform.rotation * corner;
                Require(Mathf.Abs(Vector3.Dot(onMark - hit.point, hit.normal) - .005f) < .0001f,
                    "Mark is not parallel to the wall.");
            }
            Directory.CreateDirectory(".local-backups/surface-paint-verification");
            Capture(camera, ".local-backups/surface-paint-verification/RoomLayout.png", 1920, 1080);

            var closeup = new GameObject("SurfacePaintPreviewCamera");
            var detail = closeup.AddComponent<Camera>();
            detail.transform.position = point + hit.normal * 1.2f;
            detail.transform.rotation = Quaternion.LookRotation(-hit.normal);
            detail.orthographic = true;
            detail.orthographicSize = .32f;
            detail.nearClipPlane = .01f;
            detail.farClipPlane = 4f;
            detail.clearFlags = CameraClearFlags.SolidColor;
            detail.backgroundColor = new Color(.9f, .85f, .7f);
            Capture(detail, Root + "/Previews/WallMark_Detail.png", 640, 640);
            UnityEngine.Object.DestroyImmediate(closeup);
            UnityEngine.Object.DestroyImmediate(mark);
            foreach (var canvas in canvases) canvas.enabled = true;
            AssetDatabase.Refresh();
            Require(AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null, "Prefab reload failed.");
            Debug.Log("SURFACE_PAINT_VERIFY_OK: atlas slice 512x512; footprint .4608m; MainWall hit=" +
                hit.point + "; normal=" + hit.normal + "; .005m offset; next ray unchanged; no colliders/scripts/shadows; previews only, scene not saved.");
        }
    }
}
