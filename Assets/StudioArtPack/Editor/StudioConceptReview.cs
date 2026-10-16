using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class StudioConceptReview
{
    public static void FinalizeModels()
    {
        if (File.Exists(StudioConceptVariantsPass.Root + "/VisualReview.txt")) throw new System.InvalidOperationException("Final geometry already reviewed; preserve manual changes.");
        var scene = EditorSceneManager.OpenScene(StudioConceptVariantsPass.Destination);
        var gun = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<StudioGunAppearance>()).Single();
        var shader = StudioConceptGunBuilder.AuthorGunShader();
        foreach (var material in gun.regions.Select(r => r.renderer.sharedMaterials[r.materialIndex]).Distinct())
        { material.shader = shader; material.SetFloat("_ReceiveShadows", 1); material.DisableKeyword("_RECEIVE_SHADOWS_OFF"); EditorUtility.SetDirty(material); }
        var filters = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<MeshFilter>(true)).Where(f => AssetDatabase.GetAssetPath(f.sharedMesh).StartsWith(StudioConceptVariantsPass.Root + "/Meshes/JoineryUV_")).ToArray();
        foreach (var filter in filters) { filter.sharedMesh.RecalculateBounds(); EditorUtility.SetDirty(filter.sharedMesh); }
        var beam = filters.Single(f => f.name == "TopBeam");
        var vertices = beam.sharedMesh.vertices;
        for (int i = 0; i < vertices.Length; i++) vertices[i].z -= .22f / beam.transform.lossyScale.z;
        beam.sharedMesh.vertices = vertices; beam.sharedMesh.RecalculateBounds(); EditorUtility.SetDirty(beam.sharedMesh);
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        if (!Lightmapping.Bake()) throw new System.InvalidOperationException("Final geometry bake failed.");
        var probe = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<ReflectionProbe>()).Single(p => p.name == "BakedStudioReflection");
        string reflection = StudioConceptVariantsPass.Root + "/Reflections/ConceptStudio.exr";
        gun.gameObject.SetActive(false);
        try { if (!Lightmapping.BakeReflectionProbe(probe, reflection)) throw new System.InvalidOperationException("Reflection bake failed."); }
        finally { gun.gameObject.SetActive(true); }
        AssetDatabase.ImportAsset(reflection, ImportAssetOptions.ForceSynchronousImport); probe.customBakedTexture = AssetDatabase.LoadAssetAtPath<Cubemap>(reflection); EditorUtility.SetDirty(probe); PrefabUtility.RecordPrefabInstancePropertyModifications(probe);
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(scene);
        StudioConceptVariantsPass.VerifyAndCapture();
        File.WriteAllText(StudioConceptVariantsPass.Root + "/VisualReview.txt", "Reviewed original website studio/balloon/gun concept files and actual Unity room/gallery renders.\nNew horizontal clear reservoir, cream receiver, thick open muzzle and gold straps accepted as closer silhouettes.\nRealtime shadow reception toggle did not resolve near-field stippling. A temporary SSAO-off comparison did; final custom URP Lit derivative omits AO sampling for the held gun only, retaining room AO and normal lighting/shadows.\nCopied beam bounds recomputed and front-beam visual placed ahead of cream column caps; original collider fields/transforms retained. Original scenes and assets retained. Glass/latex remain approximations; physical input and performance unverified.\n");
        AssetDatabase.Refresh();
    }
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene(StudioConceptVariantsPass.Destination);
        var renderers = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<MeshRenderer>(true)).Where(r => r.enabled && r.gameObject.activeInHierarchy).ToArray();
        var beam = renderers.Single(r => r.name == "TopBeam");
        File.WriteAllLines("Logs/ConceptBeamAudit.txt", renderers.Where(r => r.bounds.Intersects(beam.bounds)).Select(r => r.name + " | " + r.bounds + " | " + AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh)));
        // Test near-camera SSAO separately from realtime shadow reception.
        var gun = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<StudioGunAppearance>()).Single();
        foreach (var material in gun.regions.Select(r => r.renderer.sharedMaterials[r.materialIndex]).Distinct())
        { material.SetFloat("_ReceiveShadows", 1); material.DisableKeyword("_RECEIVE_SHADOWS_OFF"); EditorUtility.SetDirty(material); }
        AssetDatabase.SaveAssets();
        var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Settings/PC_Renderer.asset");
        var ao = renderer.rendererFeatures.Single(f => f.name == "ScreenSpaceAmbientOcclusion");
        try { ao.SetActive(false); StudioConceptVariantsPass.VerifyAndCapture(); }
        finally { ao.SetActive(true); }
    }
}
