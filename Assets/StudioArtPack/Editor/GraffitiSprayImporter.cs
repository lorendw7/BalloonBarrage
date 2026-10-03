using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GraffitiSprayImporter
{
    const string Root="Assets/StudioArtPack";
    const string Atlas=Root+"/Textures/GraffitiSpray_Atlas_v3.png";
    const string Output=Root+"/SprayPrefabs";
    static GraffitiSprayImporter() { EditorApplication.delayCall += Build; }
    [MenuItem("Tools/Balloon Studio/Import Spray Sprites")]
    public static void Build()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling ||
            !File.Exists(Atlas) || File.Exists(Output+"/Ready.txt")) return;
        var importer=AssetImporter.GetAtPath(Atlas) as TextureImporter;
        if(importer==null) return;
        importer.GetSourceTextureWidthAndHeight(out int sourceWidth, out int sourceHeight);
        float w=sourceWidth/3f, h=sourceHeight/2f;
        importer.npotScale=TextureImporterNPOTScale.None;
        importer.textureType=TextureImporterType.Sprite;
        importer.spriteImportMode=SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit=200;
        importer.alphaSource=TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency=true;
        importer.mipmapEnabled=false;
        importer.wrapMode=TextureWrapMode.Clamp;
        importer.filterMode=FilterMode.Bilinear;
        importer.maxTextureSize=2048;
        importer.textureCompression=TextureImporterCompression.Uncompressed;
        string[] names={"Coral_Burst","Teal_Spray","Yellow_Drip","Indigo_Swoosh","Pink_Brush","Turquoise_Burst"};
        var slices=new SpriteMetaData[6];
        for(int i=0;i<6;i++)
            slices[i]=new SpriteMetaData {name=names[i],rect=new Rect((i%3)*w,(1-i/3)*h,w,h),alignment=9,pivot=new Vector2(.5f,.5f)};
#pragma warning disable 0618
        importer.spritesheet=slices;
#pragma warning restore 0618
        importer.SaveAndReimport();
        Directory.CreateDirectory(Output);
        AssetDatabase.Refresh();
        var shader=Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if(shader==null) shader=Shader.Find("Sprites/Default");
        var mat=AssetDatabase.LoadAssetAtPath<Material>(Output+"/SprayUnlit.mat");
        if(mat==null) { mat=new Material(shader); AssetDatabase.CreateAsset(mat,Output+"/SprayUnlit.mat"); }
        int count=0;
        foreach(var asset in AssetDatabase.LoadAllAssetsAtPath(Atlas))
        {
            if(!(asset is Sprite sprite)) continue;
            var go=new GameObject(sprite.name);
            var renderer=go.AddComponent<SpriteRenderer>();
            renderer.sprite=sprite;
            renderer.sharedMaterial=mat;
            renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows=false;
            PrefabUtility.SaveAsPrefabAsset(go,Output+"/"+sprite.name+".prefab");
            Object.DestroyImmediate(go);
            count++;
        }
        if(count!=6) throw new System.Exception("Expected six spray sprites, got "+count);
        AssetDatabase.SaveAssets();
        File.WriteAllText(Output+"/Ready.txt","Six transparent spray sprites and prefabs imported / 六个透明喷漆 Sprite 和 Prefab 已导入。");
        AssetDatabase.Refresh();
        Debug.Log("SPRAY_IMPORT_OK: 6");
    }
}
