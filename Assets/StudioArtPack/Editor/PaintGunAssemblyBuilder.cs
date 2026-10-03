using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Editor art tooling only. Never edits the original model or gameplay scripts.
public static class PaintGunAssemblyBuilder
{
    const string Source="Assets/Art/BlasterKit/blaster-m.fbx";
    static string PreviewDir => Path.GetFullPath(Path.Combine(Application.dataPath,"../.local-backups/PaintGunPreview"));
    const string Root="Assets/StudioArtPack/PaintBlaster";
    [MenuItem("Tools/Balloon Studio/Assemble Blaster M")]
    public static void Build()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Exit Play Mode before assembling art.");
        if(!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
        Directory.CreateDirectory(Root+"/Assembled");AssetDatabase.Refresh();
        var source=AssetDatabase.LoadAssetAtPath<GameObject>(Source);
        var tankSource=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/PaintCanister_Teal.prefab");
        var teal=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/Teal.mat");
        if(source==null||tankSource==null||teal==null)throw new Exception("Missing existing model or paint supplements");
        string prefabPath=Root+"/Assembled/PaintGun_M.prefab";
        if(File.Exists(prefabPath))throw new Exception("Assembly exists; preserve manual edits, use a new version for regeneration.");
        var gun=new GameObject("PaintGun_M");
        var model=(GameObject)PrefabUtility.InstantiatePrefab(source);
        model.name="Original_Blaster_M";model.transform.SetParent(gun.transform,false);
        model.transform.localRotation=Quaternion.Euler(0,180,0);
        var tank=(GameObject)PrefabUtility.InstantiatePrefab(tankSource);
        tank.name="PaintCanister";tank.transform.SetParent(gun.transform,false);
        tank.transform.localScale=Vector3.one*.4f;
        tank.transform.localPosition=new Vector3(0,.22f,-.045f);
        var mount=GameObject.CreatePrimitive(PrimitiveType.Cube);
        mount.name="CanisterMount";mount.transform.SetParent(gun.transform,false);
        mount.transform.localPosition=new Vector3(0,.135f,-.045f);
        mount.transform.localScale=new Vector3(.1f,.055f,.1f);
        mount.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/Gold.mat");
        var nozzle=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        nozzle.name="PaintNozzle";nozzle.transform.SetParent(gun.transform,false);
        nozzle.transform.localPosition=new Vector3(0,.085f,.316f);
        nozzle.transform.localRotation=Quaternion.Euler(90,0,0);
        nozzle.transform.localScale=new Vector3(.046f,.016f,.046f);
        UnityEngine.Object.DestroyImmediate(nozzle.GetComponent<Collider>());
        nozzle.GetComponent<Renderer>().sharedMaterial=teal;
        var muzzle=new GameObject("Muzzle");muzzle.transform.SetParent(gun.transform,false);
        muzzle.transform.localPosition=new Vector3(0,.085f,.345f);
        var grip=new GameObject("GripPoint");grip.transform.SetParent(gun.transform,false);
        grip.transform.localPosition=new Vector3(0,-.08f,-.16f);
        var material=PaintMaterial();
        var spray=MakeSpray(material);
        spray.transform.SetParent(muzzle.transform,false);
        foreach(var c in gun.GetComponentsInChildren<Collider>())UnityEngine.Object.DestroyImmediate(c);
        PrefabUtility.SaveAsPrefabAsset(gun,prefabPath);
        Render(gun,"assembled-m.png",new Vector3(-1,.45f,1));
        Render(gun,"assembled-front.png",new Vector3(0,.15f,1));
        Render(gun,"assembled-m.png",new Vector3(-1,.45f,1));
        UnityEngine.Object.DestroyImmediate(gun);
        CreatePreview(prefabPath);
        AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        var saved=AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if(saved.transform.Find("Muzzle/MuzzleSpray")==null || saved.GetComponentsInChildren<Collider>().Length!=0)
            throw new Exception("Assembly verification failed");
        Debug.Log("PAINT_GUN_ASSEMBLY_OK: original blaster-m, reservoir, muzzle, optional spray, preview scene");
    }
    static Material PaintMaterial()
    {
        string path=Root+"/Assembled/PaintParticles.mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(mat!=null)return mat;
        var shader=Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if(shader==null)throw new Exception("Particle shader missing");
        mat=new Material(shader);mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/PaintFX_Atlas_v1.png"));
        mat.SetColor("_BaseColor",Color.white);mat.SetFloat("_Surface",1);
        mat.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);mat.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_SrcBlendAlpha",1);mat.SetFloat("_DstBlendAlpha",(float)BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite",0);mat.SetFloat("_Cull",0);mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.SetOverrideTag("RenderType","Transparent");mat.renderQueue=3000;
        AssetDatabase.CreateAsset(mat,path);return mat;
    }
    static GameObject MakeSpray(Material material)
    {
        var go=new GameObject("MuzzleSpray");var ps=go.AddComponent<ParticleSystem>();
        ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        var main=ps.main;main.loop=false;main.playOnAwake=false;main.duration=.25f;
        main.startLifetime=new ParticleSystem.MinMaxCurve(.1f,.2f);
        main.startSpeed=new ParticleSystem.MinMaxCurve(1.5f,2.5f);
        main.startSize=new ParticleSystem.MinMaxCurve(.025f,.05f);
        main.startColor=new Color(.025f,.65f,.72f);main.maxParticles=16;
        main.simulationSpace=ParticleSystemSimulationSpace.World;
        var emission=ps.emission;emission.rateOverTime=0;emission.SetBursts(new[]{new ParticleSystem.Burst(0,6)});
        var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Cone;shape.angle=6;shape.radius=.009f;
        var sheet=ps.textureSheetAnimation;sheet.enabled=true;sheet.numTilesX=2;sheet.numTilesY=2;
        sheet.frameOverTime=new ParticleSystem.MinMaxCurve(0);sheet.startFrame=0;
        var renderer=go.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;
        renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
        return go;
    }
    static void CreatePreview(string prefabPath)
    {
        string scenePath=Root+"/Assembled/PaintGun_M_Preview.unity";
        if(File.Exists(scenePath))return;
        var previous=SceneManager.GetActiveScene();
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,Application.isBatchMode ? NewSceneMode.Single : NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        try
        {
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
        var cameraObject=new GameObject("PreviewCamera");var camera=cameraObject.AddComponent<Camera>();
        cameraObject.tag="MainCamera";cameraObject.AddComponent<AudioListener>();
        camera.transform.position=new Vector3(-.9f,.55f,1.1f);camera.transform.LookAt(new Vector3(0,.06f,0));
        camera.orthographic=true;camera.orthographicSize=.46f;camera.nearClipPlane=.01f;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.85f,.8f,.7f);
        var light=new GameObject("KeyLight").AddComponent<Light>();light.type=LightType.Directional;
        light.intensity=1.5f;light.transform.rotation=Quaternion.Euler(35,-30,0);
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.5f,.5f,.5f);
        EditorSceneManager.SaveScene(scene,scenePath);
        }
        finally
        {
            if(previous.IsValid())SceneManager.SetActiveScene(previous);
            if(!Application.isBatchMode)EditorSceneManager.CloseScene(scene,true);
        }
    }
    public static void Inspect()
    {
        var source=AssetDatabase.LoadAssetAtPath<GameObject>(Source);
        if(source==null)throw new Exception("Missing downloaded blaster-m");
        var go=UnityEngine.Object.Instantiate(source);
        try
        {
            var bounds=BoundsOf(go);
            Debug.Log("GUN_BOUNDS center="+bounds.center.ToString("F3")+" size="+bounds.size.ToString("F3"));
            Render(go,"m-side.png",new Vector3(1,.3f,.25f));
            Render(go,"m-other.png",new Vector3(-1,.3f,-.25f));
        }
        finally{UnityEngine.Object.DestroyImmediate(go);}
    }
    static Bounds BoundsOf(GameObject go)
    {
        var rs=go.GetComponentsInChildren<Renderer>();
        if(rs.Length==0)throw new Exception("No gun renderers");
        var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);return b;
    }
    static void Render(GameObject go,string name,Vector3 direction)
    {
        Directory.CreateDirectory(PreviewDir);
        var utility=new PreviewRenderUtility();
        bool asyncCompilation=ShaderUtil.allowAsyncCompilation;
        ShaderUtil.allowAsyncCompilation=false;
        try
        {
            var clone=UnityEngine.Object.Instantiate(go);utility.AddSingleGO(clone);
            var b=BoundsOf(clone);
            utility.camera.orthographic=true;
            utility.camera.orthographicSize=b.size.magnitude*.64f;
            utility.camera.transform.position=b.center+direction.normalized*b.size.magnitude*3;
            utility.camera.transform.LookAt(b.center);
            utility.camera.nearClipPlane=.01f;utility.camera.farClipPlane=100;
            utility.camera.clearFlags=CameraClearFlags.Color;
            utility.camera.backgroundColor=new Color(.83f,.79f,.7f);
            utility.lights[0].intensity=1.3f;
            utility.lights[0].transform.rotation=Quaternion.Euler(35,25,0);
            utility.lights[1].intensity=.7f;
            utility.ambientColor=Color.gray;
            utility.BeginStaticPreview(new Rect(0,0,900,700));
            utility.Render(true);
            var image=utility.EndStaticPreview();
            File.WriteAllBytes(PreviewDir+"/"+name,image.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(image);
        }
        finally{utility.Cleanup();ShaderUtil.allowAsyncCompilation=asyncCompilation;}
    }
}
