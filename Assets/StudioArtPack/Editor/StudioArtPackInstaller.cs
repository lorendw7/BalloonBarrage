// Editor-only art assembly; gameplay scripts remain learner-owned.
// 仅在编辑器组装美术，保留用户玩法脚本。
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class StudioArtPackInstaller
{
    const string Root = "Assets/StudioArtPack";
    const string Output = Root + "/Generated";
    const string Preview = Output + "/StudioArtPreview.unity";
    static StudioArtPackInstaller() { EditorApplication.delayCall += AutoInstall; }

    static void AutoInstall()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
        if (!File.Exists(Root + "/InstallRequested.txt") || File.Exists(Output + "/Installed.txt")) return;
        if (!File.Exists("Assets/Scenes/PrototypeScene.unity")) return;
        Build();
    }

    [MenuItem("Tools/Balloon Studio/Build Art Preview")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        { Debug.LogWarning("Stop Play before installing art / 请先停止运行。"); return; }
        if (File.Exists(Preview))
        { Debug.Log("Art preview already exists / 美术预览已存在: " + Preview); return; }
        Directory.CreateDirectory(Output);
        AssetDatabase.Refresh();
        Scene original = SceneManager.GetActiveScene();
        Scene preview = default;
        try
        {
            // Copy saved scene first; capture unsaved active prototype through saveAsCopy.
            if (original.path == "Assets/Scenes/PrototypeScene.unity")
            {
                if (!EditorSceneManager.SaveScene(original, Preview, true))
                    throw new Exception("Could not create preview scene copy.");
            }
            else if (!AssetDatabase.CopyAsset("Assets/Scenes/PrototypeScene.unity", Preview))
                throw new Exception("PrototypeScene missing or copy failed.");
            AssetDatabase.Refresh();
            preview = EditorSceneManager.OpenScene(Preview, OpenSceneMode.Additive);
            SceneManager.SetActiveScene(preview);
            var objects = preview.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
            var wood = Mat("HoneyWood", Color.white, .18f, "HoneyWood");
            var floor = Mat("WarmConcrete", Color.white, .12f, "WarmConcrete");
            var dark = Mat("Charcoal", new Color(.09f,.12f,.17f), .22f);
            var cream = Mat("Cream", new Color(.94f,.85f,.69f), .2f);
            var coral = Mat("Coral", new Color(1f,.23f,.22f), .58f);
            var teal = Mat("Teal", new Color(.025f,.52f,.56f), .58f);
            var gold = Mat("Gold", new Color(1f,.64f,.12f), .5f);
            var leaf = Mat("Leaf", new Color(.18f,.43f,.29f), .22f);
            var clay = Mat("Terracotta", new Color(.65f,.29f,.17f), .22f);
            foreach (var t in objects)
            {
                var r = t.GetComponent<MeshRenderer>();
                if (r == null) continue;
                if (new[]{"TopBeam","LeftColumn","RightColumn","Stage"}.Contains(t.name)) r.sharedMaterial=wood;
                if (t.name=="Floor") r.sharedMaterial=floor;
            }
            // Keep the user's existing wall, camera, lights, and colorful graffiti.
            var decor = new GameObject("StudioDecor");
            var speaker = new GameObject("Speaker");
            Part(speaker.transform,"Cabinet",PrimitiveType.Cube,new Vector3(0,.7f,0),new Vector3(.8f,1.4f,.55f),dark);
            Part(speaker.transform,"Woofer",PrimitiveType.Cylinder,new Vector3(0,.5f,-.29f),new Vector3(.55f,.035f,.55f),cream).localRotation=Quaternion.Euler(90,0,0);
            Part(speaker.transform,"Cone",PrimitiveType.Cylinder,new Vector3(0,.5f,-.34f),new Vector3(.4f,.03f,.4f),dark).localRotation=Quaternion.Euler(90,0,0);
            Part(speaker.transform,"Tweeter",PrimitiveType.Sphere,new Vector3(0,1.07f,-.29f),new Vector3(.2f,.2f,.08f),gold);
            SavePlace(speaker,decor.transform,new Vector3(-5.25f,-4f,7f));
            var plant = new GameObject("PottedPlant");
            Part(plant.transform,"Pot",PrimitiveType.Cylinder,new Vector3(0,.26f,0),new Vector3(.65f,.26f,.65f),clay);
            Part(plant.transform,"Stem",PrimitiveType.Cylinder,new Vector3(0,.9f,0),new Vector3(.055f,.65f,.055f),leaf);
            for(int i=0;i<6;i++)
            {
                float a=i*60f*Mathf.Deg2Rad;
                var l=Part(plant.transform,"Leaf"+i,PrimitiveType.Sphere,new Vector3(Mathf.Cos(a)*.22f,.8f+i*.14f,Mathf.Sin(a)*.17f),new Vector3(.22f,.65f,.1f),leaf);
                l.localRotation=Quaternion.Euler(0,-i*60f, i%2==0?35:-35);
            }
            SavePlace(plant,decor.transform,new Vector3(5.1f,-4f,7f));
            var cans = new GameObject("PaintCans");
            var paints=new[]{coral,teal,gold};
            for(int i=0;i<3;i++)
            {
                var x=(i-1)*.43f;
                Part(cans.transform,"Can"+i,PrimitiveType.Cylinder,new Vector3(x,.22f,i%2*.18f),new Vector3(.34f,.22f,.34f),paints[i]);
                Part(cans.transform,"Lid"+i,PrimitiveType.Cylinder,new Vector3(x,.455f,i%2*.18f),new Vector3(.36f,.015f,.36f),cream);
            }
            SavePlace(cans,decor.transform,new Vector3(-4.2f,-4f,5.8f));

            string[] names={"Coral","Teal","Gold"};
            for(int i=0;i<3;i++)
            {
                var template=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PlayableBalloon.prefab");
                if(template==null) throw new Exception("PlayableBalloon prefab missing");
                var balloon=(GameObject)PrefabUtility.InstantiatePrefab(template);
                balloon.name="Balloon_"+names[i];
                balloon.SetActive(true);
                foreach(var r in balloon.GetComponentsInChildren<MeshRenderer>(true))
                    r.sharedMaterials=r.sharedMaterials.Select(_=>paints[i]).ToArray();
                PrefabUtility.SaveAsPrefabAsset(balloon,Output+"/"+balloon.name+".prefab");
                UnityEngine.Object.DestroyImmediate(balloon);
            }
            foreach(var t in objects)
            {
                var spawner=t.GetComponent<BalloonSpawner>();
                if(spawner==null) continue;
                var so=new SerializedObject(spawner);
                so.FindProperty("balloonPrefab").objectReferenceValue=AssetDatabase.LoadAssetAtPath<GameObject>(Output+"/Balloon_Coral.prefab");
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Prepared UI prefab only: no fake score/timer or unimplemented buttons.
            var panel=new GameObject("PaperPanel",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));
            panel.GetComponent<RawImage>().texture=Texture("PaperPanel");
            panel.GetComponent<RawImage>().raycastTarget=false;
            panel.GetComponent<RectTransform>().sizeDelta=new Vector2(300,200);
            PrefabUtility.SaveAsPrefabAsset(panel,Output+"/PaperPanel.prefab");
            UnityEngine.Object.DestroyImmediate(panel);

            var burst=new GameObject("PaintBurst");
            var ps=burst.AddComponent<ParticleSystem>();
            ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=ps.main;
            main.loop=false; main.playOnAwake=true; main.duration=.6f;
            main.startLifetime=.7f; main.startSpeed=2.2f; main.startSize=.18f;
            main.startColor=coral.color; main.maxParticles=24;
            main.gravityModifier=.25f;
            var emission=ps.emission; emission.rateOverTime=0;
            emission.SetBursts(new[]{new ParticleSystem.Burst(0,18)});
            var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Sphere; shape.radius=.1f;
            var particleMat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            particleMat.name="PaintParticle";
            AssetDatabase.CreateAsset(particleMat,Output+"/PaintParticle.mat");
            burst.GetComponent<ParticleSystemRenderer>().sharedMaterial=particleMat;
            PrefabUtility.SaveAsPrefabAsset(burst,Output+"/PaintBurst.prefab");
            UnityEngine.Object.DestroyImmediate(burst);

            EditorSceneManager.MarkSceneDirty(preview);
            if(!EditorSceneManager.SaveScene(preview)) throw new Exception("Preview save failed");
            AssetDatabase.SaveAssets();
            File.WriteAllText(Output+"/Installed.txt",
                "Art preview installed / 美术预览已生成\n"+Preview+
                "\nOriginal scene and gameplay scripts preserved.\n"+
                "UI panel and burst are prepared prefabs; runtime score/effect hooks are not implemented.\n");
            Debug.Log("STUDIO_ART_INSTALLED: "+Preview);
        }
        catch(Exception e) { Debug.LogException(e); }
        finally
        {
            if(preview.IsValid() && preview.isLoaded) EditorSceneManager.CloseScene(preview,true);
            if(original.IsValid() && original.isLoaded) SceneManager.SetActiveScene(original);
            AssetDatabase.Refresh();
        }
    }

    static Texture2D Texture(string name)
    {
        string path=Root+"/Textures/"+name+".png";
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);
        if(importer==null) throw new Exception("Missing texture "+path);
        importer.textureType=TextureImporterType.Default; importer.sRGBTexture=true;
        importer.maxTextureSize=2048; importer.mipmapEnabled=name!="PaperPanel";
        importer.wrapMode=name=="PaperPanel"?TextureWrapMode.Clamp:TextureWrapMode.Repeat;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
    static Material Mat(string name,Color color,float smoothness,string texture=null)
    {
        var mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.name=name; mat.SetColor("_BaseColor",color);
        mat.SetFloat("_Metallic",0); mat.SetFloat("_Smoothness",smoothness);
        if(texture!=null) mat.SetTexture("_BaseMap",Texture(texture));
        AssetDatabase.CreateAsset(mat,Output+"/"+name+".mat");
        return mat;
    }
    static Transform Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Material material)
    {
        var go=GameObject.CreatePrimitive(type);
        go.name=name; go.transform.SetParent(parent,false);
        go.transform.localPosition=position; go.transform.localScale=scale;
        go.GetComponent<Renderer>().sharedMaterial=material;
        UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        return go.transform;
    }
    static void SavePlace(GameObject go,Transform parent,Vector3 position)
    {
        var prefab=PrefabUtility.SaveAsPrefabAsset(go,Output+"/"+go.name+".prefab");
        UnityEngine.Object.DestroyImmediate(go);
        var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab,parent);
        instance.transform.localPosition=position;
    }
}
