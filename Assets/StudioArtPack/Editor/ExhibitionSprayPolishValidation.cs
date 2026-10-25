using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    /// <summary>
    /// Verifies the bounded exhibition spray-polish configuration without
    /// changing gameplay assets or saving the production scene.
    /// </summary>
    public static class ExhibitionSprayPolishValidation
    {
        private const string GunPrefabPath =
            "Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab";

        private const string ScenePath =
            "Assets/Scenes/PrototypeScene.unity";

        public static void RunBatch()
        {
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "Run the exhibition spray validation in batch mode.");
            }

            try
            {
                ValidateParticlePrefab();
                ValidateProductionScene();
                Debug.Log(
                    "EXHIBITION_SPRAY_POLISH_OK: concentrated two-frame stream, " +
                    "subtle noise, bounded particles, foreground sorting, scene " +
                    "reference and softened focus-wall graffiti passed.");
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void ValidateParticlePrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                GunPrefabPath);

            Require(prefab != null, "Missing paint gun prefab.");

            Transform sprayTransform = prefab.transform.Find(
                "Muzzle/MuzzleSpray");

            Require(sprayTransform != null, "Missing Muzzle/MuzzleSpray.");
            RequireClose(sprayTransform.localPosition.x, 0f,
                "Unexpected lateral visual-emitter offset.");
            RequireClose(sprayTransform.localPosition.y, 0f,
                "Unexpected vertical visual-emitter offset.");
            RequireClose(sprayTransform.localPosition.z, 0.14f,
                "Visual emitter must remain in front of the logical muzzle.");

            ParticleSystem particleSystem =
                sprayTransform.GetComponent<ParticleSystem>();
            ParticleSystemRenderer renderer =
                sprayTransform.GetComponent<ParticleSystemRenderer>();

            Require(particleSystem != null, "Missing spray particle system.");
            Require(renderer != null, "Missing spray particle renderer.");

            ParticleSystem.MainModule main = particleSystem.main;
            Require(main.loop && !main.playOnAwake,
                "Spray must loop only when explicitly played.");
            Require(main.maxParticles == 288,
                "Unexpected spray particle cap.");
            RequireClose(main.startSpeed.constant, 32f,
                "Unexpected visual speed.");
            RequireClose(main.startSize.constantMin, 0.1f,
                "Unexpected minimum stream size.");
            RequireClose(main.startSize.constantMax, 0.2f,
                "Unexpected maximum stream size.");

            ParticleSystem.EmissionModule emission = particleSystem.emission;
            RequireClose(emission.rateOverTime.constant, 260f,
                "Unexpected stream density.");

            ParticleSystem.ShapeModule shape = particleSystem.shape;
            Require(shape.enabled, "Spray cone is disabled.");
            RequireClose(shape.angle, 0.65f,
                "Unexpected spray cone angle.");

            ParticleSystem.NoiseModule noise = particleSystem.noise;
            Require(noise.enabled, "Organic stream noise is disabled.");
            RequireClose(noise.strength.constant, 0.025f,
                "Unexpected spray noise strength.");
            RequireClose(noise.frequency, 1.6f,
                "Unexpected spray noise frequency.");
            RequireClose(noise.scrollSpeed.constant, 0.15f,
                "Unexpected spray noise motion.");

            ParticleSystem.TextureSheetAnimationModule sheet =
                particleSystem.textureSheetAnimation;
            Require(sheet.enabled, "Texture sheet animation is disabled.");
            Require(sheet.numTilesX == 2 && sheet.numTilesY == 2,
                "Unexpected spray atlas layout.");
            RequireClose(sheet.startFrame.constantMin, 0f,
                "Unexpected first stream frame.");
            RequireClose(sheet.startFrame.constantMax, 0.4999f,
                "Unexpected second stream-frame range.");

            Require(renderer.sortingOrder == 10000,
                "Spray no longer renders above transparent graffiti.");
            RequireClose(renderer.velocityScale, 0.01f,
                "Unexpected stretched-particle velocity scale.");
            RequireClose(renderer.lengthScale, 1.35f,
                "Unexpected stretched-particle length.");

            GameObject instance = Object.Instantiate(prefab);
            try
            {
                ParticleSystem live = instance.transform
                    .Find("Muzzle/MuzzleSpray")
                    .GetComponent<ParticleSystem>();

                live.Simulate(0.25f, true, true, true);
                Require(live.particleCount > 0,
                    "Spray configuration emitted no particles in simulation.");
                Require(live.particleCount <= main.maxParticles,
                    "Spray simulation exceeded its particle cap.");
            }
            finally
            {
                Object.DestroyImmediate(instance);
            }
        }

        private static void ValidateProductionScene()
        {
            EditorSceneManager.OpenScene(ScenePath);

            PlayerShooter shooter = Object.FindObjectsByType<PlayerShooter>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Single(component =>
                    component.gameObject.scene ==
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            SerializedObject serializedShooter =
                new SerializedObject(shooter);
            ParticleSystem sceneSpray = serializedShooter
                .FindProperty("muzzleSpray")
                .objectReferenceValue as ParticleSystem;

            Require(sceneSpray != null,
                "PlayerShooter lost its muzzle-spray reference.");
            Require(sceneSpray.GetComponent<ParticleSystemRenderer>()
                    .sortingOrder == 10000,
                "Scene spray no longer uses foreground sorting.");

            ValidateGraffitiAlpha("Graffiti_Red_Dots", 0.78f);
            ValidateGraffitiAlpha("Graffiti_Yellow_Brush", 0.82f);
            ValidateGraffitiAlpha("Graffiti_Cyan_Back", 0.82f);
        }

        private static void ValidateGraffitiAlpha(
            string objectName,
            float expectedAlpha)
        {
            SpriteRenderer renderer = Object
                .FindObjectsByType<SpriteRenderer>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Single(candidate => candidate.name == objectName);

            RequireClose(renderer.color.a, expectedAlpha,
                objectName + " alpha changed.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        private static void RequireClose(
            float actual,
            float expected,
            string message)
        {
            if (!Mathf.Approximately(actual, expected))
            {
                throw new InvalidOperationException(
                    $"{message} Expected {expected}, got {actual}.");
            }
        }
    }
}
