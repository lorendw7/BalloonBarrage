using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    public static class SurfacePaintCoverageSetupTool
    {
        private const string Root =
            "Assets/StudioArtPack/SurfacePaintCoverage20261027";
        private const string AtlasPath =
            Root + "/Textures/PaintCoverage_Atlas_v1.png";
        private const string PrefabDirectory = Root + "/Prefabs";
        private const string ProjectilePath =
            "Assets/StudioArtPack/PaintBlaster/Prefabs/PaintDrop3D_Teal.prefab";
        private const string BalloonPath =
            "Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab";
        private const string ScenePath =
            "Assets/Scenes/PrototypeScene.unity";

        private static readonly string[] Colors =
        {
            "Teal", "Coral", "Gold", "SkyBlue",
            "Violet", "Rose", "Lime", "Tangerine"
        };

        private static readonly (string name, string sprite)[] Shapes =
        {
            ("Puddle", "CoveragePuddleTopLeft"),
            ("Smear", "CoverageSmearTopRight"),
            ("Burst", "CoverageBurstBottomLeft"),
            ("Bridge", "CoverageBridgeBottomRight")
        };

        [MenuItem("BalloonBarrage/Art/Setup Broad Coverage Paint")]
        public static void Setup()
        {
            ConfigureAtlas();

            Dictionary<string, Sprite> sprites = AssetDatabase
                .LoadAllAssetsAtPath(AtlasPath)
                .OfType<Sprite>()
                .ToDictionary(sprite => sprite.name, sprite => sprite);

            Require(sprites.Count == Shapes.Length,
                "Coverage atlas must contain exactly four named sprites.");

            EnsureFolder(Root, "Prefabs");

            for (int colorIndex = 0;
                 colorIndex < Colors.Length;
                 colorIndex++)
            {
                string color = Colors[colorIndex];
                Material material = MaterialFor(color);

                foreach ((string shape, string spriteName) in Shapes)
                {
                    Require(sprites.TryGetValue(spriteName, out Sprite sprite),
                        "Missing atlas sprite " + spriteName + ".");
                    SaveMarkPrefab(color, shape, sprite, material);
                }
            }

            ConnectProjectile();
            ConnectBalloon();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Validate();

            Debug.Log(
                "SURFACE_PAINT_COVERAGE_SETUP_OK: four broad marks across " +
                "eight pigments, direct deposits, legacy deposits and " +
                "balloon-pop deposits connected.");
        }

        public static void RunBatch()
        {
            if (!Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "Run coverage setup in batch mode.");
            }

            try
            {
                Setup();
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        private static void ConfigureAtlas()
        {
            TextureImporter importer = AssetImporter.GetAtPath(AtlasPath)
                as TextureImporter;
            Require(importer != null, "Missing coverage-atlas importer.");

            bool changed =
                importer.textureType != TextureImporterType.Sprite ||
                importer.spriteImportMode != SpriteImportMode.Multiple ||
                importer.maxTextureSize != 1024 ||
                importer.mipmapEnabled ||
                importer.spritePixelsPerUnit != 250f ||
                importer.wrapMode != TextureWrapMode.Clamp ||
                importer.filterMode != FilterMode.Bilinear ||
                !importer.alphaIsTransparency;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.maxTextureSize = 1024;
            importer.mipmapEnabled = false;
            importer.spritePixelsPerUnit = 250f;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.crunchedCompression = false;

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }

        private static Material MaterialFor(string color)
        {
            string sourcePath =
                "Assets/StudioArtPack/SurfacePaint/Patterns/" +
                "SurfacePaint_" + color + "_Spray.prefab";
            GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(
                sourcePath);
            Require(source != null,
                "Missing existing pigment source " + sourcePath + ".");

            SpriteRenderer renderer = source.GetComponent<SpriteRenderer>();
            Require(renderer != null && renderer.sharedMaterial != null,
                "Existing pigment source lost its material: " + sourcePath);
            return renderer.sharedMaterial;
        }

        private static string MarkPath(string color, string shape)
        {
            return PrefabDirectory + "/SurfacePaintCoverage_" +
                color + "_" + shape + ".prefab";
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static GameObject LoadMark(string color, string shape)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                MarkPath(color, shape));
            Require(prefab != null,
                "Missing coverage prefab " + MarkPath(color, shape) + ".");
            return prefab;
        }

        private static void SaveMarkPrefab(
            string color,
            string shape,
            Sprite sprite,
            Material material)
        {
            string name = "SurfacePaintCoverage_" + color + "_" + shape;
            GameObject root = new GameObject(name)
            {
                layer = LayerMask.NameToLayer("Ignore Raycast")
            };

            try
            {
                root.transform.localScale = Vector3.one * 0.18f;
                SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sharedMaterial = material;
                renderer.color = new Color(1f, 1f, 1f, 0.94f);
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.lightProbeUsage = LightProbeUsage.Off;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                renderer.motionVectorGenerationMode =
                    MotionVectorGenerationMode.ForceNoMotion;
                renderer.allowOcclusionWhenDynamic = false;

                GameObject saved = PrefabUtility.SaveAsPrefabAsset(
                    root,
                    MarkPath(color, shape));
                Require(saved != null, "Failed to save " + name + ".");
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }

        private static void ConnectProjectile()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(ProjectilePath);
            try
            {
                PaintProjectile projectile = root.GetComponent<PaintProjectile>();
                Require(projectile != null,
                    "Paint projectile prefab lost PaintProjectile.");

                SerializedObject serialized = new SerializedObject(projectile);
                SerializedProperty groups = serialized.FindProperty(
                    "paintPatternGroups");
                Require(groups != null && groups.arraySize == Colors.Length,
                    "Paint projectile must retain eight pigment groups.");

                SerializedProperty legacy = serialized.FindProperty(
                    "paintMarkVariants");
                Require(legacy != null,
                    "Paint projectile lost legacy paintMarkVariants.");
                legacy.arraySize = Colors.Length;

                for (int i = 0; i < Colors.Length; i++)
                {
                    SerializedProperty prefabs = groups
                        .GetArrayElementAtIndex(i)
                        .FindPropertyRelative("prefabs");
                    Require(prefabs != null && prefabs.arraySize >= 6,
                        "Pigment group " + Colors[i] +
                        " must retain six pattern slots.");

                    prefabs.GetArrayElementAtIndex(0).objectReferenceValue =
                        LoadMark(Colors[i], "Puddle");
                    prefabs.GetArrayElementAtIndex(1).objectReferenceValue =
                        LoadMark(Colors[i], "Burst");
                    prefabs.GetArrayElementAtIndex(2).objectReferenceValue =
                        LoadMark(Colors[i], "Bridge");
                    legacy.GetArrayElementAtIndex(i).objectReferenceValue =
                        LoadMark(Colors[i], "Puddle");
                }

                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, ProjectilePath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ConnectBalloon()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(BalloonPath);
            try
            {
                Balloon balloon = root.GetComponent<Balloon>();
                Require(balloon != null,
                    "Playable balloon prefab lost Balloon.");
                SerializedObject serialized = new SerializedObject(balloon);
                SerializedProperty marks = serialized.FindProperty(
                    "wallPaintPrefabs");
                Require(marks != null,
                    "Balloon lost wallPaintPrefabs.");
                marks.arraySize = Colors.Length;

                for (int i = 0; i < Colors.Length; i++)
                {
                    marks.GetArrayElementAtIndex(i).objectReferenceValue =
                        LoadMark(Colors[i], "Burst");
                }

                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, BalloonPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void Validate()
        {
            TextureImporter importer = AssetImporter.GetAtPath(AtlasPath)
                as TextureImporter;
            Require(importer != null && importer.maxTextureSize == 1024,
                "Coverage atlas must remain capped at 1024 pixels.");
            Require(!importer.mipmapEnabled && importer.alphaIsTransparency,
                "Coverage atlas must keep alpha and disable mipmaps.");

            foreach (string color in Colors)
            {
                Material expectedMaterial = MaterialFor(color);
                foreach ((string shape, _) in Shapes)
                {
                    GameObject prefab = LoadMark(color, shape);
                    SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
                    Require(renderer != null && renderer.sprite != null,
                        prefab.name + " is missing its sprite.");
                    Require(renderer.sharedMaterial == expectedMaterial,
                        prefab.name + " uses the wrong pigment material.");
                    Require(prefab.GetComponentsInChildren<Collider>(true).Length == 0,
                        prefab.name + " must not contain a collider.");
                    Require(prefab.GetComponentsInChildren<Rigidbody>(true).Length == 0,
                        prefab.name + " must not contain a Rigidbody.");
                    Require(prefab.GetComponentsInChildren<MonoBehaviour>(true).Length == 0,
                        prefab.name + " must not contain a runtime script.");
                    Require(renderer.shadowCastingMode == ShadowCastingMode.Off &&
                            !renderer.receiveShadows &&
                            renderer.lightProbeUsage == LightProbeUsage.Off &&
                            renderer.reflectionProbeUsage == ReflectionProbeUsage.Off &&
                            renderer.motionVectorGenerationMode ==
                                MotionVectorGenerationMode.ForceNoMotion,
                        prefab.name + " restored unnecessary render work.");
                }
            }

            GameObject projectileRoot = AssetDatabase.LoadAssetAtPath<GameObject>(
                ProjectilePath);
            PaintProjectile projectile = projectileRoot.GetComponent<PaintProjectile>();
            SerializedObject projectileSerialized = new SerializedObject(projectile);
            SerializedProperty groups = projectileSerialized.FindProperty(
                "paintPatternGroups");
            Require(groups.arraySize == Colors.Length,
                "Integrated projectile lost an eight-color group.");

            for (int i = 0; i < Colors.Length; i++)
            {
                SerializedProperty prefabs = groups
                    .GetArrayElementAtIndex(i)
                    .FindPropertyRelative("prefabs");
                Require(prefabs.GetArrayElementAtIndex(0).objectReferenceValue ==
                        LoadMark(Colors[i], "Puddle") &&
                        prefabs.GetArrayElementAtIndex(1).objectReferenceValue ==
                        LoadMark(Colors[i], "Burst") &&
                        prefabs.GetArrayElementAtIndex(2).objectReferenceValue ==
                        LoadMark(Colors[i], "Bridge"),
                    Colors[i] + " direct marks are not connected.");
            }

            GameObject balloonRoot = AssetDatabase.LoadAssetAtPath<GameObject>(
                BalloonPath);
            SerializedProperty balloonMarks = new SerializedObject(
                    balloonRoot.GetComponent<Balloon>())
                .FindProperty("wallPaintPrefabs");
            Require(balloonMarks.arraySize == Colors.Length,
                "Integrated balloon lost an eight-color wall mark.");
            for (int i = 0; i < Colors.Length; i++)
            {
                Require(balloonMarks.GetArrayElementAtIndex(i)
                        .objectReferenceValue == LoadMark(Colors[i], "Burst"),
                    Colors[i] + " balloon deposit is not connected.");
            }

            EditorSceneManager.OpenScene(ScenePath);
            PaintSurface[] surfaces = Object.FindObjectsByType<PaintSurface>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            Require(surfaces.Length == 9,
                "Production scene must retain nine prepared paint surfaces.");
            foreach (PaintSurface surface in surfaces)
            {
                SerializedProperty target = new SerializedObject(surface)
                    .FindProperty("targetRenderer");
                Require(target != null && target.objectReferenceValue != null,
                    surface.name + " lost its target renderer.");
            }
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
