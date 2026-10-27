#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static class ImpactSprayV3SetupTool
{
    private const string SetupLabel =
        "BalloonBarrageImpactSprayV3-v3";

    private const string PrefabFolder =
        "Assets/StudioArtPack/PaintSprayStyle20261025/Prefabs";

    private const string PrefabPath =
        PrefabFolder + "/ImpactSprayV3.prefab";

    private const string MaterialPath =
        "Assets/StudioArtPack/PaintSprayStyle20261025/Materials/PaintVfx_V3.mat";

    private const string AtlasPath =
        "Assets/StudioArtPack/PaintSprayStyle20261025/Textures/PaintSprayVfx_Atlas_v3.png";

    [InitializeOnLoadMethod]
    private static void ScheduleSetup()
    {
        EditorApplication.delayCall += SetupWhenReady;
    }

    [MenuItem("BalloonBarrage/Art/Setup Impact Spray V3")]
    private static void SetupWhenReady()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Scene scene = SceneManager.GetActiveScene();

        if (!scene.IsValid() || !scene.isLoaded || scene.name != "PrototypeScene")
        {
            return;
        }

        PlayerShooter shooter = Object.FindFirstObjectByType<PlayerShooter>(
            FindObjectsInactive.Include);

        if (shooter == null)
        {
            return;
        }

        ParticleSystem prefab = LoadImpactPrefab();
        SerializedObject shooterObject = new SerializedObject(shooter);
        SerializedProperty impactProperty =
            shooterObject.FindProperty("impactSprayPrefab");
        SerializedProperty canisterProperty =
            shooterObject.FindProperty("paintCanisterRenderer");

        MeshRenderer canisterRenderer = shooter
            .GetComponentsInChildren<MeshRenderer>(true)
            .FirstOrDefault(renderer =>
                renderer.gameObject.name == "PigmentTeal");

        bool prefabIsCurrent = prefab != null &&
            AssetDatabase.GetLabels(prefab.gameObject)
                .Contains(SetupLabel);

        bool referenceIsCurrent = prefab != null &&
            impactProperty != null &&
            impactProperty.objectReferenceValue == prefab;

        bool canisterReferenceIsCurrent =
            canisterProperty != null &&
            canisterRenderer != null &&
            canisterProperty.objectReferenceValue == canisterRenderer;

        if (prefabIsCurrent &&
            referenceIsCurrent &&
            canisterReferenceIsCurrent)
        {
            return;
        }

        if (!prefabIsCurrent)
        {
            prefab = CreateOrUpdateImpactPrefab();
        }

        if (prefab == null ||
            impactProperty == null ||
            canisterProperty == null ||
            canisterRenderer == null)
        {
            Debug.LogError(
                "Art setup could not assign the impact prefab or paint canister renderer.");
            return;
        }

        impactProperty.objectReferenceValue = prefab;
        canisterProperty.objectReferenceValue = canisterRenderer;
        shooterObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(shooter);

        RemoveTemporarySceneObject(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log(
            "Impact Spray V3 and PigmentTeal renderer connected to PlayerShooter.");
    }

    private static ParticleSystem LoadImpactPrefab()
    {
        GameObject prefabObject =
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        return prefabObject != null
            ? prefabObject.GetComponent<ParticleSystem>()
            : null;
    }

    private static ParticleSystem CreateOrUpdateImpactPrefab()
    {
        EnsurePrefabFolder();

        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        Sprite impactSprite = AssetDatabase
            .LoadAllAssetsAtPath(AtlasPath)
            .OfType<Sprite>()
            .FirstOrDefault(sprite =>
                sprite.name == "ImpactSplashV3BottomRight");

        Sprite dropletSprite = AssetDatabase
            .LoadAllAssetsAtPath(AtlasPath)
            .OfType<Sprite>()
            .FirstOrDefault(sprite =>
                sprite.name == "DropletClusterV3BottomLeft");

        if (material == null ||
            impactSprite == null ||
            dropletSprite == null)
        {
            Debug.LogError("Impact Spray V3 material or sprites are missing.");
            return null;
        }

        bool prefabAlreadyExists =
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath) != null;

        GameObject source = prefabAlreadyExists
            ? PrefabUtility.LoadPrefabContents(PrefabPath)
            : new GameObject("ImpactSprayV3");

        ParticleSystem particles = source.GetComponent<ParticleSystem>();

        if (particles == null)
        {
            particles = source.AddComponent<ParticleSystem>();
        }

        ConfigureMain(particles);
        ConfigureEmission(particles);
        ConfigureShape(particles);
        ConfigureFade(particles);
        ConfigureRenderer(particles, material, impactSprite);
        ConfigureDroplets(source, material, dropletSprite);

        GameObject prefabObject = PrefabUtility.SaveAsPrefabAsset(
            source,
            PrefabPath);

        if (prefabAlreadyExists)
        {
            PrefabUtility.UnloadPrefabContents(source);
        }
        else
        {
            Object.DestroyImmediate(source);
        }

        if (prefabObject != null)
        {
            string[] labels = AssetDatabase.GetLabels(prefabObject)
                .Where(label =>
                    !label.StartsWith("BalloonBarrageImpactSprayV3-"))
                .Append(SetupLabel)
                .ToArray();

            AssetDatabase.SetLabels(prefabObject, labels);
        }

        return prefabObject != null
            ? prefabObject.GetComponent<ParticleSystem>()
            : null;
    }

    private static void ConfigureMain(ParticleSystem particles)
    {
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = 0f;
        main.startSize = 1.1f;
        main.startColor = Color.white;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.maxParticles = 4;
    }

    private static void ConfigureEmission(ParticleSystem particles)
    {
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)1)
        });
    }

    private static void ConfigureShape(ParticleSystem particles)
    {
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.enabled = false;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 38f;
        shape.radius = 0.02f;
        shape.length = 0.05f;
    }

    private static void ConfigureFade(ParticleSystem particles)
    {
        Gradient fade = new Gradient();
        fade.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            });

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime =
            particles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fade);

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime =
            particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.65f),
                new Keyframe(0.18f, 1f),
                new Keyframe(1f, 0.25f)));
    }

    private static void ConfigureRenderer(
        ParticleSystem particles,
        Material material,
        Sprite impactSprite)
    {
        ParticleSystemRenderer renderer =
            particles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.View;
        renderer.material = material;
        renderer.sortingOrder = 10001;

        ParticleSystem.TextureSheetAnimationModule textureSheet =
            particles.textureSheetAnimation;
        textureSheet.enabled = true;
        textureSheet.mode = ParticleSystemAnimationMode.Sprites;

        while (textureSheet.spriteCount > 0)
        {
            textureSheet.RemoveSprite(0);
        }

        textureSheet.AddSprite(impactSprite);
    }

    private static void ConfigureDroplets(
        GameObject root,
        Material material,
        Sprite dropletSprite)
    {
        Transform existing = root.transform.Find("ImpactDropletsV3");
        GameObject dropletObject;

        if (existing != null)
        {
            dropletObject = existing.gameObject;
        }
        else
        {
            dropletObject = new GameObject("ImpactDropletsV3");
            dropletObject.transform.SetParent(root.transform, false);
        }

        dropletObject.transform.localPosition = Vector3.zero;
        dropletObject.transform.localRotation = Quaternion.identity;
        dropletObject.transform.localScale = Vector3.one;

        ParticleSystem droplets =
            dropletObject.GetComponent<ParticleSystem>();

        if (droplets == null)
        {
            droplets = dropletObject.AddComponent<ParticleSystem>();
        }

        ParticleSystem.MainModule main = droplets.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.28f, 0.5f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.2f, 2.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.28f);
        main.startRotation = new ParticleSystem.MinMaxCurve(
            0f,
            Mathf.PI * 2f);
        main.startColor = Color.white;
        main.gravityModifier = 0.25f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.maxParticles = 20;

        ParticleSystem.EmissionModule emission = droplets.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[]
        {
            new ParticleSystem.Burst(0f, (short)7)
        });

        ParticleSystem.ShapeModule shape = droplets.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 55f;
        shape.radius = 0.02f;
        shape.length = 0.05f;

        Gradient fade = new Gradient();
        fade.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            });

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime =
            droplets.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fade);

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime =
            droplets.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            new AnimationCurve(
                new Keyframe(0f, 0.9f),
                new Keyframe(1f, 0.15f)));

        ParticleSystemRenderer renderer =
            droplets.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.View;
        renderer.material = material;
        renderer.sortingOrder = 10002;

        ParticleSystem.TextureSheetAnimationModule textureSheet =
            droplets.textureSheetAnimation;
        textureSheet.enabled = true;
        textureSheet.mode = ParticleSystemAnimationMode.Sprites;

        while (textureSheet.spriteCount > 0)
        {
            textureSheet.RemoveSprite(0);
        }

        textureSheet.AddSprite(dropletSprite);
    }

    private static void EnsurePrefabFolder()
    {
        const string parent =
            "Assets/StudioArtPack/PaintSprayStyle20261025";

        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder(parent, "Prefabs");
        }
    }

    private static void RemoveTemporarySceneObject(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "ImpactSprayV3" &&
                !PrefabUtility.IsPartOfPrefabInstance(root))
            {
                Object.DestroyImmediate(root);
            }
        }
    }
}
#endif
