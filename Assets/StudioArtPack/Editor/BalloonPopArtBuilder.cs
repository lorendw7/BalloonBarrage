using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// Editor-only asset creation. Existing prefabs and learner scripts are not changed.
public static class BalloonPopArtBuilder
{
    const string Root = "Assets/StudioArtPack/BalloonPopV2";
    [MenuItem("Tools/Balloon Studio/Build Latex Pop Art")]
    public static void Build()
    {
        AssetDatabase.Refresh();
        string path = Root + "/LatexFragments_v1.png";
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new Exception("Missing latex atlas");
        importer.textureType = TextureImporterType.Default;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "/LatexFragments.mat");
        if (material == null)
        {
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null) throw new Exception("URP particle shader not available");
            material = new Material(shader);
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", Color.white);
            material.SetFloat("_Surface", 1);
            material.SetFloat("_Blend", 0);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_SrcBlendAlpha", (float)BlendMode.One);
            material.SetFloat("_DstBlendAlpha", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0);
            material.SetFloat("_Cull", (float)CullMode.Off);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)RenderQueue.Transparent;
            AssetDatabase.CreateAsset(material, Root + "/LatexFragments.mat");
        }
        Create("BalloonPop_Teal", new Color(.04f,.58f,.7f), material);
        Create("BalloonPop_Coral", new Color(1f,.3f,.25f), material);
        Create("BalloonPop_Gold", new Color(1f,.73f,.12f), material);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("LATEX_POP_ART_OK: 3 prefabs, shared transparent atlas/material");
    }

    static void Create(string name, Color color, Material material)
    {
        string path = Root + "/" + name + ".prefab";
        if (File.Exists(path)) return; // Never overwrite manually tuned art.
        var go = new GameObject(name);
        try
        {
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main;
            main.loop = false;
            main.duration = .7f;
            main.playOnAwake = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(.38f,.68f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(2.4f,4f);
            main.startSize = new ParticleSystem.MinMaxCurve(.22f,.4f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0,Mathf.PI*2);
            main.startColor = color;
            main.gravityModifier = .65f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 16;
            main.stopAction = ParticleSystemStopAction.Destroy;
            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0,12) });
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = .06f;
            var sheet = ps.textureSheetAnimation;
            sheet.enabled = true;
            sheet.numTilesX = 2;
            sheet.numTilesY = 2;
            sheet.animation = ParticleSystemAnimationType.WholeSheet;
            sheet.frameOverTime = new ParticleSystem.MinMaxCurve(0);
            sheet.startFrame = new ParticleSystem.MinMaxCurve(0,1);
            var rotation = ps.rotationOverLifetime;
            rotation.enabled = true;
            rotation.z = new ParticleSystem.MinMaxCurve(-9f,9f);
            var fade = ps.colorOverLifetime;
            fade.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(new[] {new GradientColorKey(Color.white,0),new GradientColorKey(Color.white,1)},
                new[] {new GradientAlphaKey(1,0),new GradientAlphaKey(1,.65f),new GradientAlphaKey(0,1)});
            fade.color = gradient;
            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            PrefabUtility.SaveAsPrefabAsset(go,path);
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
    }
}
