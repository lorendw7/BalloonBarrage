using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace BalloonBarrage.EditorTools
{
    // Asset preparation only. Does not connect or implement gameplay.
    public static class RhythmFeedbackBuilder
    {
        private const string Folder = "Assets/StudioArtPack/RhythmFeedback";
        private const string GlowPath = Folder + "/OnBeatGlow.prefab";
        private const string TextPath = Folder + "/OnBeatFeedback.prefab";

        [MenuItem("Tools/BalloonBarrage/Build Missing Rhythm Feedback Assets")]
        public static void Build()
        {
            if (AssetDatabase.LoadMainAssetAtPath(GlowPath) != null ||
                AssetDatabase.LoadMainAssetAtPath(TextPath) != null)
                throw new InvalidOperationException("Feedback assets already exist; preserve and inspect them before rebuilding.");

            var shader = AssetDatabase.LoadAssetAtPath<Shader>(Folder + "/OnBeatGlow.shader");
            if (shader == null) throw new InvalidOperationException("OnBeatGlow shader is missing.");
            var ringMaterial = new Material(shader) { name = "OnBeatRing" };
            ringMaterial.SetColor("_Tint", new Color(2.6f, 1.8f, 0.6f, 1f));
            ringMaterial.SetFloat("_Shape", 0f);
            AssetDatabase.CreateAsset(ringMaterial, Folder + "/OnBeatRing.mat");
            var sparkMaterial = new Material(shader) { name = "OnBeatSparks" };
            sparkMaterial.SetColor("_Tint", new Color(3f, 2.3f, 1f, 1f));
            sparkMaterial.SetFloat("_Shape", 1f);
            AssetDatabase.CreateAsset(sparkMaterial, Folder + "/OnBeatSparks.mat");

            var root = new GameObject("OnBeatGlow");
            var ring = Configure(root, ringMaterial, 1, 0.38f, 0.38f, 0f, 0f, 1.3f, 1.3f);
            var rootMain = ring.main;
            rootMain.stopAction = ParticleSystemStopAction.Destroy;
            var ringSize = ring.sizeOverLifetime;
            ringSize.enabled = true;
            ringSize.size = new ParticleSystem.MinMaxCurve(1f,
                new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(0.45f, 1.1f), new Keyframe(1f, 1.65f)));

            var sparksObject = new GameObject("OnBeatSparkles");
            sparksObject.transform.SetParent(root.transform, false);
            var sparks = Configure(sparksObject, sparkMaterial, 12, 0.22f, 0.42f, 1.5f, 3.2f, 0.06f, 0.13f);
            var shape = sparks.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.06f;
            var sparkSize = sparks.sizeOverLifetime;
            sparkSize.enabled = true;
            sparkSize.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));
            PrefabUtility.SaveAsPrefabAsset(root, GlowPath);
            UnityEngine.Object.DestroyImmediate(root);

            var labelObject = new GameObject("OnBeatFeedback", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            labelObject.layer = 5;
            var rect = labelObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 168f);
            rect.sizeDelta = new Vector2(440f, 64f);
            var label = labelObject.GetComponent<Text>();
            label.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/StudioArtPack/InterfaceV2/Fonts/Lato-Bold.ttf");
            if (label.font == null) throw new InvalidOperationException("Bundled Lato Bold font is missing.");
            label.fontSize = 32;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(0.48f, 0.20f, 0.04f, 1f);
            label.text = "ON BEAT +20";
            label.raycastTarget = false;
            label.enabled = false;
            PrefabUtility.SaveAsPrefabAsset(labelObject, TextPath);
            UnityEngine.Object.DestroyImmediate(labelObject);
            AssetDatabase.SaveAssets();
            if (Application.isBatchMode) VerifyAndPreview();
        }

        private static ParticleSystem Configure(GameObject owner, Material material, int count,
            float minLife, float maxLife, float minSpeed, float maxSpeed, float minSize, float maxSize)
        {
            var particles = owner.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.useAutoRandomSeed = false;
            particles.randomSeed = (uint)(count + 317);
            var main = particles.main;
            main.duration = 0.5f;
            main.loop = false;
            main.playOnAwake = true;
            main.useUnscaledTime = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Local;
            main.maxParticles = count;
            main.startLifetime = new ParticleSystem.MinMaxCurve(minLife, maxLife);
            main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
            main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
            main.startColor = Color.white;
            var emission = particles.emission;
            emission.rateOverTime = 0f;
            emission.rateOverDistance = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });
            var shape = particles.shape;
            shape.enabled = false;
            var color = particles.colorOverLifetime;
            color.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(1f, 0.12f), new GradientAlphaKey(0f, 1f) });
            color.color = gradient;
            owner.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
            return particles;
        }

        public static void VerifyAndPreview()
        {
            if (!Application.isBatchMode)
                throw new InvalidOperationException("Run scene previews in an isolated batch project to preserve open scenes.");
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GlowPath);
            var systems = prefab.GetComponentsInChildren<ParticleSystem>(true);
            if (systems.Length != 2 || prefab.GetComponentsInChildren<Light>(true).Length != 0 ||
                prefab.GetComponentsInChildren<Collider>(true).Length != 0 ||
                prefab.GetComponentsInChildren<MonoBehaviour>(true).Length != 0)
                throw new InvalidOperationException("Unexpected glow hierarchy/components.");
            int budget = 0;
            foreach (var particles in systems)
            {
                if (particles.main.loop || particles.main.useUnscaledTime || particles.emission.rateOverTime.constant != 0f)
                    throw new InvalidOperationException("Feedback must be a bounded, scaled-time burst.");
                budget += particles.main.maxParticles;
                var material = particles.GetComponent<ParticleSystemRenderer>().sharedMaterial;
                if (material == null || !material.shader.isSupported || ShaderUtil.ShaderHasError(material.shader))
                    throw new InvalidOperationException("Unsupported feedback material/shader.");
            }
            if (budget != 13) throw new InvalidOperationException("Expected a 13-particle maximum.");
            var label = AssetDatabase.LoadAssetAtPath<GameObject>(TextPath).GetComponent<Text>();
            if (label == null || label.enabled || label.raycastTarget || label.font == null)
                throw new InvalidOperationException("Expected a hidden, nonblocking text label.");

            EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
            foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                canvas.gameObject.SetActive(false);
            var camera = Camera.main;
            if (camera == null) throw new InvalidOperationException("Preview camera is missing.");
            var glow = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            glow.transform.position = new Vector3(0f, 0f, 8f);
            var root = glow.GetComponent<ParticleSystem>();
            string previewFolder = Path.Combine(Folder, "Previews");
            if (!AssetDatabase.IsValidFolder(previewFolder)) AssetDatabase.CreateFolder(Folder, "Previews");
            Render(camera, Path.Combine(previewFolder, "Studio_Before.png"), glow, root, -1f);
            Render(camera, Path.Combine(previewFolder, "OnBeatGlow_080ms.png"), glow, root, 0.08f);
            Render(camera, Path.Combine(previewFolder, "OnBeatGlow_180ms.png"), glow, root, 0.18f);
            Render(camera, Path.Combine(previewFolder, "OnBeatGlow_320ms.png"), glow, root, 0.32f);
            root.Simulate(0.75f, true, true, false);
            foreach (var particles in glow.GetComponentsInChildren<ParticleSystem>())
                if (particles.particleCount != 0)
                    throw new InvalidOperationException("Visible particles remain after the intended burst: " + particles.name);
            // Manual edit-mode simulation checks visible particles, not runtime destruction.
            // The gameplay handoff also schedules destruction after one second.
            UnityEngine.Object.DestroyImmediate(glow);
            AssetDatabase.Refresh();
            Debug.Log("RHYTHM_FEEDBACK_ASSETS_OK: two systems, 13 particles max, no light/collider/runtime script, hidden nonblocking text; preview only, gameplay not connected.");
        }

        private static void Render(Camera camera, string path, GameObject glow, ParticleSystem root, float time)
        {
            glow.SetActive(time >= 0f);
            if (time >= 0f) root.Simulate(time, true, true, false);
            var previous = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var target = RenderTexture.GetTemporary(1280, 720, 24, RenderTextureFormat.ARGB32);
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            File.WriteAllBytes(path, image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
            camera.targetTexture = previous;
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(target);
        }
    }
}
