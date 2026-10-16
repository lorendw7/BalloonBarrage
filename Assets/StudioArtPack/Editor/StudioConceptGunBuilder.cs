using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

/// <summary>Build a rounded toy paint gun aligned with the original firing helpers.</summary>
public static class StudioConceptGunBuilder
{
    static string Root => StudioConceptVariantsPass.Root;
    static Vector2 V(float radius, float height) => new Vector2(radius, height);

    public static Shader AuthorGunShader()
    {
        string path = Root + "/Shaders/StudioGunLit.shader";
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(Root + "/Shaders");
            var package = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.render-pipelines.universal/Shaders/Lit.shader");
            string sourcePath = Path.Combine(package.resolvedPath, "Shaders/Lit.shader");
            string source = File.ReadAllText(sourcePath);
            string derived = source.Replace("Shader \"Universal Render Pipeline/Lit\"", "Shader \"BalloonBarrage/Studio Gun Lit\"")
                .Replace("#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION", "// Held gun omits screen-space AO sampling; room AO and physical lighting remain enabled.");
            File.WriteAllText(path, "// Derived from Unity URP " + package.version + " Shaders/Lit.shader; see UnityCompanionLicense.md and Provenance.txt.\n" + derived);
            File.Copy(Path.Combine(package.resolvedPath, "LICENSE.md"), Root + "/Shaders/UnityCompanionLicense.md");
            using (var hash = SHA256.Create())
                File.WriteAllText(Root + "/Shaders/Provenance.txt", "Source: Unity installed com.unity.render-pipelines.universal " + package.version + "/Shaders/Lit.shader\nSHA-256: " + BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(sourcePath))).Replace("-", "").ToLowerInvariant() + "\nChanges: shader name; forward-pass SSAO variant omitted for held weapon only. Remaining URP lighting, shadows, probes and passes retained. License copied verbatim.\n");
            AssetDatabase.Refresh();
        }
        return AssetDatabase.LoadAssetAtPath<Shader>(path);
    }

    public static StudioGunAppearance.Region[] Build(PlayerShooter shooter)
    {
        var old = shooter.transform.Find("RefinedGunVisual"); old.gameObject.SetActive(false);
        PrefabUtility.RecordPrefabInstancePropertyModifications(old.gameObject);
        var root = new GameObject("ConceptPaintGun").transform; root.SetParent(shooter.transform, false);
        var materials = new[] { "WeaponCream", "WeaponTealGrip", "WeaponCoralMuzzle", "WeaponYellowStructure" }.Select(n => new Material(AssetDatabase.LoadAssetAtPath<Material>(StudioMaterialAppearancePass.Root + "/Materials/" + n + ".mat")) { name = "Concept_" + n }).ToArray();
        var shader = AuthorGunShader();
        foreach (var material in materials) { material.shader = shader; material.SetTexture("_BaseMap", null); material.SetTexture("_BumpMap", null); material.DisableKeyword("_NORMALMAP"); material.SetFloat("_Smoothness", .40f); AssetDatabase.CreateAsset(material, Root + "/Materials/" + material.name + ".mat"); }
        var glass = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "ReservoirGlass" };
        glass.SetColor("_BaseColor", new Color(.88f, .96f, 1, .19f)); glass.SetFloat("_Surface", 1); glass.SetFloat("_Blend", 0); glass.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); glass.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha); glass.SetFloat("_ZWrite", 0); glass.SetFloat("_Smoothness", .72f); glass.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); glass.SetOverrideTag("RenderType", "Transparent"); glass.renderQueue = (int)RenderQueue.Transparent; glass.SetShaderPassEnabled("ShadowCaster", false);
        AssetDatabase.CreateAsset(glass, Root + "/Materials/ReservoirGlass.mat");
        Transform Part(string name, Mesh mesh, Vector3 position, Material material, Quaternion? rotation = null)
        {
            var go = new GameObject(name); go.transform.SetParent(root, false); go.transform.localPosition = position; go.transform.localRotation = rotation ?? Quaternion.identity;
            go.AddComponent<MeshFilter>().sharedMesh = mesh; go.AddComponent<MeshRenderer>().sharedMaterial = material; return go.transform;
        }
        Transform Box(string name, Vector3 position, Vector3 size, float radius, int role, Quaternion? rotation = null) => Part(name, StudioConceptGeometry.RoundedBox(size, radius), position, materials[role], rotation);
        var barrelRotation = Quaternion.Euler(90, 0, 0);
        Box("SoftReceiver", new Vector3(0, .063f, -.065f), new Vector3(.174f, .156f, .30f), .030f, 0);
        Box("UpperCradle", new Vector3(0, .15f, -.064f), new Vector3(.14f, .034f, .23f), .016f, 3);
        Box("TealGrip", new Vector3(0, -.084f, -.156f), new Vector3(.088f, .218f, .095f), .025f, 1, Quaternion.Euler(-16, 0, 0));
        Box("CreamGripHeel", new Vector3(0, -.180f, -.127f), new Vector3(.107f, .035f, .118f), .015f, 0);
        Box("TriggerGuardBase", new Vector3(0, -.090f, -.058f), new Vector3(.028f, .026f, .119f), .012f, 0);
        Box("TriggerGuardFront", new Vector3(0, -.052f, -.010f), new Vector3(.028f, .103f, .028f), .012f, 0);
        Box("CoralTrigger", new Vector3(0, -.041f, -.069f), new Vector3(.022f, .057f, .028f), .010f, 2, Quaternion.Euler(-14, 0, 0));
        // Open thick muzzle ends at .342, before the learner-owned Muzzle helper at .345.
        var barrel = StudioConceptGeometry.Lathe(new[] { V(.040f, -.090f), V(.063f, -.087f), V(.074f, -.066f), V(.082f, .076f), V(.080f, .100f), V(.072f, .107f), V(.052f, .107f), V(.044f, .094f), V(.040f, -.090f) });
        Part("CoralOpenMuzzle", barrel, new Vector3(0, .085f, .235f), materials[2], barrelRotation);
        var collar = StudioConceptGeometry.Lathe(new[] { V(.051f, -.010f), V(.061f, -.007f), V(.061f, .007f), V(.051f, .010f), V(.044f, .010f), V(.044f, -.010f), V(.051f, -.010f) });
        Part("GoldBarrelCollar", collar, new Vector3(0, .085f, .145f), materials[3], barrelRotation);
        var chamber = StudioConceptGeometry.Lathe(new[] { V(0, -.143f), V(.047f, -.138f), V(.067f, -.124f), V(.075f, -.097f), V(.075f, .087f), V(.067f, .111f), V(.047f, .125f), V(0, .129f) });
        var glassPart = Part("ClearHorizontalReservoir", chamber, new Vector3(0, .238f, -.065f), glass, barrelRotation);
        glassPart.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
        Box("PigmentInsideReservoir", new Vector3(0, .220f, -.069f), new Vector3(.132f, .081f, .239f), .035f, 1);
        var strap = StudioConceptGeometry.Lathe(new[] { V(.074f, -.012f), V(.081f, -.009f), V(.081f, .009f), V(.074f, .012f), V(.071f, .012f), V(.071f, -.012f), V(.074f, -.012f) });
        Part("GoldReservoirStrap", strap, new Vector3(0, .238f, -.122f), materials[3], barrelRotation);
        Part("GoldReservoirBack", strap, new Vector3(0, .238f, -.194f), materials[3], barrelRotation);
        var lid = StudioConceptGeometry.Lathe(new[] { V(0, -.009f), V(.046f, -.009f), V(.052f, -.004f), V(.052f, .015f), V(.045f, .021f), V(0, .021f) });
        Part("GoldReservoirLid", lid, new Vector3(0, .311f, -.135f), materials[3]);
        for (int i = 0; i < 12; i++)
        {
            float a = i * Mathf.PI / 6;
            Box("LidKnurl", new Vector3(Mathf.Cos(a) * .049f, .316f, -.135f + Mathf.Sin(a) * .049f), new Vector3(.005f, .014f, .007f), .0015f, 3, Quaternion.Euler(0, -i * 30, 0));
        }
        // Gold round buttons, molded grip ribs and pigmented flecks retain the friendly toy language.
        var button = StudioConceptGeometry.Lathe(new[] { V(0, -.003f), V(.024f, -.003f), V(.030f, 0), V(.030f, .008f), V(.025f, .012f), V(0, .012f) });
        var sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); var sphere = sphereObject.GetComponent<MeshFilter>().sharedMesh; Object.DestroyImmediate(sphereObject);
        foreach (int side in new[] { -1, 1 })
        {
            Part("GoldCheekButton", button, new Vector3(side * .086f, .065f, -.050f), materials[3], Quaternion.Euler(0, 0, -side * 90));
            for (int i = 0; i < 4; i++)
            {
                var fleck = Part("PigmentFleck", sphere, new Vector3(side * .087f, .067f + i * .011f, -.126f + i * .008f), materials[1]);
                fleck.localScale = new Vector3(.0025f, i == 0 ? .027f : .010f, i == 0 ? .032f : .009f);
            }
        }
        for (int i = 0; i < 3; i++) Box("MoldedGripRib", new Vector3(0, -.057f - i * .035f, -.210f + i * .010f), new Vector3(.061f, .009f, .009f), .004f, 1);
        var regions = new List<StudioGunAppearance.Region>();
        foreach (var material in materials.Concat(new[] { glass }))
        {
            var renderers = root.GetComponentsInChildren<MeshRenderer>().Where(r => r.sharedMaterial == material).ToArray();
            var combine = renderers.Select(r => new CombineInstance { mesh = r.GetComponent<MeshFilter>().sharedMesh, transform = root.worldToLocalMatrix * r.transform.localToWorldMatrix }).ToArray();
            var mesh = new Mesh { name = "ConceptGun_" + material.name }; mesh.CombineMeshes(combine, true, true); mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh, Root + "/Meshes/" + mesh.name + ".asset");
            foreach (var renderer in renderers) Object.DestroyImmediate(renderer.gameObject);
            var part = Part(mesh.name, mesh, Vector3.zero, material); var finalRenderer = part.GetComponent<MeshRenderer>();
            if (material == glass) finalRenderer.shadowCastingMode = ShadowCastingMode.Off;
            else regions.Add(new StudioGunAppearance.Region { renderer = finalRenderer, materialIndex = 0, role = Array.IndexOf(materials, material) });
        }
        return regions.ToArray();
    }
}
