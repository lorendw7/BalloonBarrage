using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

// Editor-only art assembly. No shooting, damage, movement, or scene changes.
public static class PaintBlasterArtBuilder
{
    const string Root="Assets/StudioArtPack/PaintBlaster";
    [MenuItem("Tools/Balloon Studio/Build Paint Blaster Art")]
    public static void Build()
    {
        Directory.CreateDirectory(Root+"/Materials");
        Directory.CreateDirectory(Root+"/Prefabs");
        AssetDatabase.Refresh();
        var gold=Mat("Gold",new Color(1,.64f,.12f));
        var teal=Mat("Teal",new Color(.025f,.62f,.69f));
        var coral=Mat("Coral",new Color(1,.29f,.23f));
        Canister("PaintCanister_Teal",teal,gold);
        Canister("PaintCanister_Coral",coral,gold);
        Projectile("PaintDrop3D_Teal",teal);
        Projectile("PaintDrop3D_Coral",coral);
        ImportSprites(teal.color,coral.color);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("PAINT_BLASTER_ART_OK: 2 canisters, 2 projectile visuals, 8 sprite visuals; existing guns untouched");
    }
    static Material Mat(string name,Color color)
    {
        string path=Root+"/Materials/"+name+".mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat!=null)return mat;
        var shader=Shader.Find("Universal Render Pipeline/Lit");
        if(shader==null)throw new Exception("URP/Lit missing");
        mat=new Material(shader);mat.SetColor("_BaseColor",color);
        mat.SetFloat("_Smoothness",.48f);mat.SetFloat("_Metallic",0);
        AssetDatabase.CreateAsset(mat,path);return mat;
    }
    static void Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Vector3 rotation,Material mat)
    {
        var go=GameObject.CreatePrimitive(type);go.name=name;
        go.transform.SetParent(parent,false);go.transform.localPosition=pos;
        go.transform.localScale=scale;go.transform.localEulerAngles=rotation;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().sharedMaterial=mat;
    }
    static void Save(GameObject go,string name)
    {
        try {PrefabUtility.SaveAsPrefabAsset(go,Root+"/Prefabs/"+name+".prefab");}
        finally {UnityEngine.Object.DestroyImmediate(go);}
    }
    static bool Exists(string name)=>File.Exists(Root+"/Prefabs/"+name+".prefab");
    static void Canister(string name,Material paint,Material gold)
    {
        if(Exists(name))return;var go=new GameObject(name);
        Part(go.transform,"PaintReservoir",PrimitiveType.Capsule,Vector3.zero,new Vector3(.3f,.23f,.3f),Vector3.zero,paint);
        Part(go.transform,"Lid",PrimitiveType.Cylinder,new Vector3(0,.24f,0),new Vector3(.28f,.035f,.28f),Vector3.zero,gold);
        Save(go,name);
    }
    static void Projectile(string name,Material paint)
    {
        if(Exists(name))return;var go=new GameObject(name);
        Part(go.transform,"Droplet",PrimitiveType.Sphere,Vector3.zero,new Vector3(.16f,.16f,.24f),Vector3.zero,paint);
        Part(go.transform,"Tail",PrimitiveType.Sphere,new Vector3(0,0,-.13f),new Vector3(.065f,.065f,.18f),Vector3.zero,paint);
        Save(go,name);
    }
    static void ImportSprites(Color teal,Color coral)
    {
        string path=Root+"/Textures/PaintFX_Atlas_v1.png";
        var importer=AssetImporter.GetAtPath(path) as TextureImporter;
        if(importer==null)throw new Exception("Missing paint atlas");
        importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Multiple;
        importer.npotScale=TextureImporterNPOTScale.None;importer.alphaIsTransparency=true;
        importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.mipmapEnabled=false;
        importer.wrapMode=TextureWrapMode.Clamp;importer.maxTextureSize=2048;
        importer.spritePixelsPerUnit=500;importer.textureCompression=TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();importer.GetSourceTextureWidthAndHeight(out int w,out int h);
        var f=new SpriteDataProviderFactories();f.Init();var provider=f.GetSpriteEditorDataProviderFromObject(importer);
        provider.InitSpriteEditorDataProvider();var previous=provider.GetSpriteRects();
        string[] names={"PaintDrop","PaintStreak","NozzleSpurt","WetImpact"};
        var rects=new SpriteRect[4];
        for(int i=0;i<4;i++)rects[i]=new SpriteRect{name=names[i],rect=new Rect((i%2)*w/2f,(1-i/2)*h/2f,w/2f,h/2f),pivot=new Vector2(.5f,.5f),alignment=SpriteAlignment.Center,spriteID=previous.FirstOrDefault(r=>r.name==names[i])?.spriteID??GUID.Generate()};
        provider.SetSpriteRects(rects);provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(rects.Select(r=>new SpriteNameFileIdPair(r.name,r.spriteID)));
        provider.Apply();importer.SaveAndReimport();
        var sprites=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        if(sprites.Length!=4)throw new Exception("Expected four paint sprites");
        foreach(var sprite in sprites)for(int i=0;i<2;i++)
        {
            string name=sprite.name+(i==0?"_Teal":"_Coral");if(Exists(name))continue;
            var go=new GameObject(name);var renderer=go.AddComponent<SpriteRenderer>();
            renderer.sprite=sprite;renderer.color=i==0?teal:coral;
            // Default SpriteRenderer material supports built-in sprite tint/alpha.
            Save(go,name);
        }
    }
}
