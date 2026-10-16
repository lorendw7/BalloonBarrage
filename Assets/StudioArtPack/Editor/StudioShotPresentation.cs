using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    // Asset preparation and explicit editor wiring only. Runtime shot triggers belong to the learner.
    public static class StudioShotPresentation
    {
        private const string ClipPath = "Assets/Audio/SFX/PaintShot_AirPuff.wav";
        private const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
        private const string Output = ".local-backups/shot-presentation";

        [MenuItem("Tools/BalloonBarrage/Connect Paint Shot Audio")]
        public static void ConnectAudio()
        {
            var shooter = Shooter();
            var audio = shooter.transform.Find("PaintShotAudio")?.GetComponent<AudioSource>();
            if (audio == null) throw new InvalidOperationException("Open the prepared PrototypeScene with PaintShotAudio.");
            var serialized = new SerializedObject(shooter);
            var property = serialized.FindProperty("shotAudioSource");
            if (property == null)
            {
                Debug.LogWarning("First save and compile [SerializeField] private AudioSource shotAudioSource; in PlayerShooter.");
                return;
            }
            if (property.objectReferenceValue != null && property.objectReferenceValue != audio)
                throw new InvalidOperationException("Preserve the existing different shot audio reference.");
            property.objectReferenceValue = audio;
            serialized.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(shooter.gameObject.scene);
            Selection.activeObject = shooter.gameObject;
            Debug.Log("PAINT_SHOT_AUDIO_CONNECTED. Save the scene. Playback remains learner code.");
        }

        [MenuItem("Tools/BalloonBarrage/Connect Gun Recoil Visual")]
        public static void ConnectRecoilVisual()
        {
            var shooter = Shooter();
            var visual = RecoilVisual(shooter);
            var serialized = new SerializedObject(shooter);
            var property = serialized.FindProperty("gunVisual");
            if (property == null)
            {
                Debug.LogWarning("First save and compile [SerializeField] private Transform gunVisual; in PlayerShooter.");
                return;
            }
            if (property.objectReferenceValue != null && property.objectReferenceValue != visual)
                throw new InvalidOperationException("Preserve the existing different gunVisual reference.");
            property.objectReferenceValue = visual;
            serialized.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(shooter.gameObject.scene);
            Selection.activeObject = shooter.gameObject;
            Debug.Log("GUN_RECOIL_VISUAL_CONNECTED: cosmetic child only. Save the scene; recoil animation remains learner code.");
        }

        private static Transform RecoilVisual(PlayerShooter shooter)
        {
            var visual = shooter.transform.Find("RefinedGunVisual");
            var serialized = new SerializedObject(shooter);
            var muzzle = serialized.FindProperty("muzzle").objectReferenceValue as Transform;
            var spray = serialized.FindProperty("muzzleSpray").objectReferenceValue as ParticleSystem;
            if (visual == null || !visual.gameObject.activeInHierarchy || visual.parent != shooter.transform ||
                !visual.GetComponentsInChildren<MeshRenderer>(true).Any(r => r.enabled && r.gameObject.activeInHierarchy) ||
                visual.GetComponentsInChildren<Collider>(true).Length != 0 ||
                visual.GetComponentsInChildren<ParticleSystem>(true).Length != 0 ||
                visual.GetComponentsInChildren<AudioSource>(true).Length != 0 ||
                muzzle == null || muzzle.IsChildOf(visual) || spray == null || spray.transform.IsChildOf(visual))
                throw new InvalidOperationException("Expected separate cosmetic RefinedGunVisual and logical Muzzle/MuzzleSpray siblings.");
            return visual;
        }

        public static void VerifyRecoilHierarchyBatch()
        {
            RequireBatch();
            EditorSceneManager.OpenScene(ScenePath);
            var shooter = Shooter();
            var visual = RecoilVisual(shooter);
            var reference = new SerializedObject(shooter).FindProperty("gunVisual");
            if (reference != null && reference.objectReferenceValue != visual)
                throw new InvalidOperationException("The saved gunVisual field does not reference the cosmetic child.");
            Debug.Log("RECOIL_HIERARCHY_OK: active mesh-only visual child; logical muzzle/particles and audio excluded. " +
                "Rest local position=" + visual.localPosition + ", local rotation=" + visual.localRotation.eulerAngles +
                ". Runtime recoil/cache code has not been implemented by this editor tool.");
        }

        public static void PrepareBatch()
        {
            RequireBatch();
            Directory.CreateDirectory(Output);
            EditorSceneManager.OpenScene(ScenePath);
            var shooter = Shooter();
            var camera = Camera.main;
            HideCanvas();
            shooter.transform.LookAt(new Vector3(0, 0, 8));
            Capture(camera, Output + "/Before.png");
            if (shooter.transform.Find("PaintShotAudio") != null)
                throw new InvalidOperationException("Audio child already exists; preserve manual edits.");
            var importer = (AudioImporter)AssetImporter.GetAtPath(ClipPath);
            var settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat = AudioCompressionFormat.PCM;
            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            settings.preloadAudioData = true;
            importer.defaultSampleSettings = settings;
            // The source is already mono; avoid Unity's downmix normalization.
            importer.forceToMono = false;
            importer.loadInBackground = false;
            importer.SaveAndReimport();
            var child = new GameObject("PaintShotAudio");
            child.transform.SetParent(shooter.transform, false);
            var audio = child.AddComponent<AudioSource>();
            audio.clip = AssetDatabase.LoadAssetAtPath<AudioClip>(ClipPath);
            audio.playOnAwake = false;
            audio.loop = false;
            audio.volume = .50f;
            audio.pitch = 1;
            audio.spatialBlend = 0;
            audio.dopplerLevel = 0;
            audio.reverbZoneMix = 0;
            audio.priority = 100;
            audio.ignoreListenerPause = false;
            shooter.transform.localPosition = new Vector3(.40f, -.30f, 1.30f);
            shooter.transform.localScale = Vector3.one * .65f;
            shooter.transform.LookAt(new Vector3(0, 0, 8));
            // Restore Canvas states by reloading the saved source before changing only the gun.
            var pose = shooter.transform.localPosition;
            var scale = shooter.transform.localScale;
            var rotation = shooter.transform.localRotation;
            string audioJson = EditorJsonUtility.ToJson(audio);
            EditorSceneManager.OpenScene(ScenePath);
            shooter = Shooter();
            shooter.transform.localPosition = pose;
            shooter.transform.localScale = scale;
            shooter.transform.localRotation = rotation;
            child = new GameObject("PaintShotAudio");
            child.transform.SetParent(shooter.transform, false);
            audio = child.AddComponent<AudioSource>();
            EditorJsonUtility.FromJsonOverwrite(audioJson, audio);
            PrefabUtility.RecordPrefabInstancePropertyModifications(shooter.transform);
            EditorSceneManager.SaveScene(shooter.gameObject.scene);
            VerifyBatch();
        }

        public static void VerifyBatch()
        {
            RequireBatch();
            Directory.CreateDirectory(Output);
            EditorSceneManager.OpenScene(ScenePath);
            var shooter = Shooter();
            var audio = shooter.transform.Find("PaintShotAudio")?.GetComponent<AudioSource>();
            var importer = (AudioImporter)AssetImporter.GetAtPath(ClipPath);
            var settings = importer.defaultSampleSettings;
            if (settings.loadType != AudioClipLoadType.DecompressOnLoad ||
                settings.compressionFormat != AudioCompressionFormat.PCM || !settings.preloadAudioData ||
                importer.forceToMono || importer.loadInBackground)
                throw new InvalidOperationException("Unexpected shot import settings.");
            if (audio == null || audio.clip == null || audio.clip.channels != 1 || audio.clip.frequency != 44100 ||
                audio.clip.length > .2f || audio.playOnAwake || audio.loop || audio.spatialBlend != 0 ||
                audio.ignoreListenerPause || audio.volume != .50f)
                throw new InvalidOperationException("Unexpected shot AudioSource/clip settings.");
            var camera = Camera.main;
            var so = new SerializedObject(shooter);
            var muzzle = (Transform)so.FindProperty("muzzle").objectReferenceValue;
            var projectile = (PaintProjectile)so.FindProperty("paintProjectilePrefab").objectReferenceValue;
            float radius = new SerializedObject(projectile).FindProperty("radius").floatValue;
            var spawner = new SerializedObject(Object.FindFirstObjectByType<BalloonSpawner>());
            var prefab = (GameObject)spawner.FindProperty("balloonPrefab").objectReferenceValue;
            var xs = spawner.FindProperty("xRange").vector2Value;
            var ys = spawner.FindProperty("yRange").vector2Value;
            float z = spawner.FindProperty("zPosition").floatValue;
            var target = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            int count = 0;
            float bottom = 1, right = 0;
            try
            {
                foreach (float aspect in new[] { 16f / 9f, 16f / 10f, 4f / 3f })
                {
                    camera.aspect = aspect;
                    foreach (float x in new[] { xs.x, 0, xs.y })
                    foreach (float y in new[] { ys.x, 0, ys.y })
                    {
                        target.transform.position = new Vector3(x, y, z);
                        Physics.SyncTransforms();
                        var collider = target.GetComponentsInChildren<Collider>().First(c => c.enabled && !c.isTrigger);
                        Vector3 point = collider.bounds.center;
                        var ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(point));
                        if (!Physics.Raycast(ray, out var aimHit, 100, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
                            aimHit.collider.GetComponentInParent<Balloon>() != target.GetComponent<Balloon>())
                            throw new InvalidOperationException("Camera aim blocked at " + point);
                        shooter.transform.LookAt(aimHit.point);
                        foreach (var filter in shooter.GetComponentsInChildren<MeshFilter>())
                        {
                            var renderer = filter.GetComponent<MeshRenderer>();
                            if (renderer == null || !renderer.enabled || !filter.gameObject.activeInHierarchy) continue;
                            foreach (Vector3 vertex in filter.sharedMesh.vertices)
                            {
                                Vector3 p = camera.WorldToViewportPoint(filter.transform.TransformPoint(vertex));
                                bottom = Mathf.Min(bottom, p.y); right = Mathf.Max(right, p.x);
                                if (p.z < camera.nearClipPlane || p.x < .02f || p.x > .98f || p.y < .025f || p.y > .98f)
                                    throw new InvalidOperationException("Weapon clipped at " + point + ", viewport " + p + ", mesh " + filter.name);
                            }
                        }
                        if (Physics.CheckSphere(muzzle.position, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
                            !Physics.SphereCast(muzzle.position, radius, (aimHit.point - muzzle.position).normalized, out var shotHit,
                                100, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) ||
                            shotHit.collider.GetComponentInParent<Balloon>() != target.GetComponent<Balloon>())
                            throw new InvalidOperationException("Muzzle/projectile path blocked at " + point);
                        count++;
                    }
                }
            }
            finally { Object.DestroyImmediate(target); }
            camera.aspect = 16f / 9f;
            shooter.transform.LookAt(new Vector3(0, 0, 8));
            HideCanvas();
            Capture(camera, Output + "/After.png");
            string report = $"SHOT_PRESENTATION_OK: clip/source settings; {count} camera/muzzle sphere-cast paths and full visible mesh vertex framing (16:9, 16:10, 4:3). Minimum bottom viewport {bottom:F4}, maximum right {right:F4}.\n" +
                "Isolated edit-mode rendering/physics checks, not an audible listening test, runtime shot-trigger test, human playtest or performance result.\n";
            File.WriteAllText(Output + "/Verification.txt", report);
            Debug.Log(report);
        }

        private static PlayerShooter Shooter() => Object.FindObjectsByType<PlayerShooter>(FindObjectsInactive.Include,
            FindObjectsSortMode.None).Single(s => s.gameObject.scene == SceneManager.GetActiveScene());

        private static void RequireBatch()
        {
            if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
        }

        private static void HideCanvas()
        {
            foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                canvas.gameObject.SetActive(false);
        }

        private static void Capture(Camera camera, string path)
        {
            var target = RenderTexture.GetTemporary(1920, 1080, 24, RenderTextureFormat.ARGB32);
            var previous = RenderTexture.active;
            var previousTarget = camera.targetTexture;
            camera.targetTexture = target;
            camera.aspect = 16f / 9f;
            camera.Render();
            RenderTexture.active = target;
            var pixels = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            pixels.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            pixels.Apply();
            File.WriteAllBytes(path, pixels.EncodeToPNG());
            Object.DestroyImmediate(pixels);
            camera.targetTexture = previousTarget;
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(target);
        }
    }
}
