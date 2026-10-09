using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Physical materials and daylight art pass. No learner gameplay changes.
public static class StudioAtmospherePolisher
{
    const string Root="Assets/StudioArtPack/AtmosphereV3";
    const string ScenePath="Assets/Scenes/PrototypeScene.unity";
    static readonly Dictionary<Material,Material> replacements=new Dictionary<Material,Material>();
    static Material wood,plaster,floor,teal,ink;

    public static void BuildBatch()
    {
        var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        ExhibitionStudioBuilder.Capture(scene,"atmosphere-before.png");
        Apply(scene);
        ExhibitionStudioBuilder.Capture(scene,"atmosphere-after.png");
        ExhibitionStudioBuilder.CaptureTargetLayout(scene);
        ExhibitionStudioBuilder.ValidateAim(scene);
        if(All<Camera>(scene).Single(c=>c.CompareTag("MainCamera")).fieldOfView!=45f) throw new Exception("Camera framing mismatch");
        Debug.Log("ATMOSPHERE_STUDIO_OK: physical materials, framing, daylight and music validated.");
    }
    [MenuItem("Tools/Balloon Studio/Apply Daylight Atmosphere V3")]
    public static void Menu()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode||SceneManager.GetActiveScene().path!=ScenePath) return;
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) Apply(SceneManager.GetActiveScene());
    }
    public static void RefreshPreviewBatch()
    {
        var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        if(!scene.GetRootGameObjects().Any(g=>g.name=="StudioAtmosphereV3")) throw new Exception("Apply V3 first.");
        RefineLightBalance(scene);
        AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        ExhibitionStudioBuilder.Capture(scene,"atmosphere-after.png");
        ExhibitionStudioBuilder.CaptureTargetLayout(scene);ExhibitionStudioBuilder.ValidateAim(scene);
        Debug.Log("ATMOSPHERE_STUDIO_OK: final light balance and framing validated.");
    }
    static void Apply(Scene scene)
    {
        if(scene.GetRootGameObjects().Any(g=>g.name=="StudioAtmosphereV3")) throw new Exception("Atmosphere already applied; edit the scene directly.");
        Directory.CreateDirectory(".local-backups");
        File.Copy(ScenePath,".local-backups/scene-before-atmosphere-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".unity",false);
        Directory.CreateDirectory(Root+"/Materials"); Directory.CreateDirectory(Root+"/Meshes");
        AssetDatabase.Refresh();replacements.Clear();
        wood=CloneExisting("HoneyWood","WalnutWood",new Color(.64f,.51f,.39f),.3f);
        wood.mainTextureScale=new Vector2(1,2.5f);
        plaster=CloneExisting("WarmConcrete","WarmPlaster",Hex("BFB4A0"),.10f);
        plaster.mainTextureScale=new Vector2(2,2);
        floor=CloneExisting("WarmConcrete","SatinConcrete",Hex("B9ADA0"),.32f);
        floor.mainTextureScale=new Vector2(4,4);
        teal=Lit("DeepTeal","38696A",.28f);ink=Lit("Graphite","30373A",.38f);
        foreach(var renderer in All<MeshRenderer>(scene))
        {
            if(renderer.transform.GetComponentInParent<PlayerShooter>()!=null) continue;
            var mats=renderer.sharedMaterials;
            for(int i=0;i<mats.Length;i++)
            {
                var old=mats[i];if(old==null||old.shader.name!="BalloonStudio/SoftToon") continue;
                if(!replacements.TryGetValue(old,out var material))
                {
                    string name=old.name.Replace("Cartoon","");
                    string hex=name=="Cream"?"C9BDA6":name=="Mint"?"7B9890":name=="Teal"?"397B79":name=="Coral"?"BA6857":
                        name=="Gold"?"C4A04D":name=="Lavender"?"8B9394":name=="Indigo"?"555C70":name=="Leaf"?"4C7952":"434947";
                    material=name=="Honey"?wood:Lit(name+"Paint",hex,name=="Leaf"?.34f:.27f);
                    replacements[old]=material;
                }
                mats[i]=material;
            }
            renderer.sharedMaterials=mats;
            switch(renderer.name)
            {
                case "TopBeam":case "LeftColumn":case "RightColumn":case "Stage":case "RoofBeam":
                case "Tabletop":case "Leg":case "LowerShelf":case "LeftSpeakerShelf":case "RightSpeakerShelf":
                case "WindowShelf":case "CrateBody":case "CrateSlat":case "LeftLeg":case "RightLeg":case "CanvasRail":
                case "LeftFrame":case "RightFrame":case "TopFrame":case "BottomFrame": renderer.sharedMaterial=wood;break;
                case "Floor":renderer.sharedMaterial=floor;break;
                case "LeftWall":case "RightWall":case "Ceiling": renderer.sharedMaterial=plaster;break;
                case "LeftWainscot":case "RightWainscot":renderer.sharedMaterial=teal;break;
                case "LampShade":case "LeftLightHousing":case "RightLightHousing":renderer.sharedMaterial=ink;break;
                case "LidRim":renderer.sharedMaterial=LitOnce("BrushedMetal","AAA69B",.5f,.55f);break;
                case "LeftLightStrip":renderer.sharedMaterial=Emissive("TealTube","48B9B2",1.4f);break;
                case "RightLightStrip":renderer.sharedMaterial=Emissive("CoralTube","EA9275",1.15f);break;
            }
            // Transparent window view and the room shell do not block the daylight.
            bool shell=new[]{"LeftWall","RightWall","Ceiling","MainWall","OutsideView","FloorPaint","PaintDot","RugBase","RugStripe"}.Contains(renderer.name);
            renderer.shadowCastingMode=shell?ShadowCastingMode.Off:ShadowCastingMode.On;
            renderer.receiveShadows=true;
            var filter=renderer.GetComponent<MeshFilter>();
            if(filter!=null&&filter.sharedMesh!=null&&filter.sharedMesh.uv.Length==0&&renderer.sharedMaterial.mainTexture!=null)
            {
                var mesh=UnityEngine.Object.Instantiate(filter.sharedMesh);var vertices=mesh.vertices;var normals=mesh.normals;
                var uv=new Vector2[vertices.Length];
                for(int i=0;i<vertices.Length;i++)
                {
                    var p=vertices[i];var n=normals[i];
                    uv[i]=Mathf.Abs(n.y)>.5f?new Vector2(p.x,p.z):Mathf.Abs(n.x)>.5f?new Vector2(p.z,p.y):new Vector2(p.x,p.y);
                }
                mesh.uv=uv;string path=AssetDatabase.GenerateUniqueAssetPath(Root+"/Meshes/"+renderer.name+"UV.asset");
                AssetDatabase.CreateAsset(mesh,path);filter.sharedMesh=mesh;PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
        }
        var group=new GameObject("StudioAtmosphereV3").transform;
        SceneManager.MoveGameObjectToScene(group.gameObject,scene);
        var flags=All<Transform>(scene).FirstOrDefault(t=>t.name=="FestivalPennants");
        if(flags!=null) flags.gameObject.SetActive(false);
        foreach(var t in All<Transform>(scene))
        {
            if(t.name=="PaintFloorAccents") t.position+=Vector3.down*.11f;
            if(t.name=="PaintBucket"&&t.position.y < -3.8f){var p=t.position;p.y=-4;t.position=p;}
            if(t.name=="PaintEasel"){var p=t.position;p.y=-4;t.position=p;}
        }
        // Recessed skylight supplies the large warm/cool planes of the website image.
        var sky=Emissive("SkylightGlass","C7DCE0",.22f);
        Part(group,"SkylightRecess",new Vector3(0,3.91f,4.4f),new Vector3(4.8f,.10f,5.8f),ink);
        Part(group,"SkylightGlass",new Vector3(0,3.84f,4.4f),new Vector3(4.5f,.055f,5.5f),sky);
        foreach(float x in new[]{-2.3f,0,2.3f}) Part(group,"SkylightBeam",new Vector3(x,3.70f,4.4f),new Vector3(.14f,.21f,5.65f),wood);
        foreach(float z in new[]{1.6f,4.4f,7.2f}) Part(group,"SkylightCrossbar",new Vector3(0,3.7f,z),new Vector3(4.75f,.21f,.14f),wood);

        RenderSettings.ambientMode=AmbientMode.Trilight;
        RenderSettings.ambientSkyColor=new Color(.50f,.56f,.60f);
        RenderSettings.ambientEquatorColor=new Color(.38f,.34f,.29f);
        RenderSettings.ambientGroundColor=new Color(.20f,.22f,.23f);
        var sun=All<Light>(scene).Single(l=>l.type==LightType.Directional);
        sun.transform.rotation=Quaternion.Euler(43,35,0);sun.color=Hex("FFE5C2");sun.intensity=1.55f;
        sun.shadows=LightShadows.Soft;sun.shadowStrength=.78f;sun.shadowBias=.035f;sun.shadowNormalBias=.18f;
        RenderSettings.sun=sun;
        var fill=All<Light>(scene).FirstOrDefault(l=>l.name=="FrontFillLight");
        if(fill!=null){fill.color=Hex("D9E8EF");fill.intensity=1.3f;fill.range=20;}
        var window=new GameObject("WindowDaylight").AddComponent<Light>();window.transform.SetParent(group,false);
        window.transform.position=new Vector3(-6.2f,2.2f,3.0f);window.transform.LookAt(new Vector3(1.4f,-4f,6.1f));
        window.type=LightType.Spot;window.color=Hex("FFE7BC");window.intensity=65;window.range=22;window.spotAngle=65;
        window.innerSpotAngle=55;window.shadows=LightShadows.Soft;window.shadowStrength=.65f;
        window.shadowBias=.025f;window.shadowNormalBias=.18f;window.cookie=WindowCookie();
        var camera=All<Camera>(scene).Single(c=>c.CompareTag("MainCamera"));
        camera.transform.position=new Vector3(0,.10f,-8.8f);camera.transform.rotation=Quaternion.Euler(2,0,0);camera.fieldOfView=45;
        var gun=All<PlayerShooter>(scene).Single().transform;
        gun.localPosition=new Vector3(.47f,-.51f,1.35f);gun.localScale=Vector3.one*.50f;gun.LookAt(new Vector3(0,0,8));
        PrefabUtility.RecordPrefabInstancePropertyModifications(gun);
        var music=All<AudioSource>(scene).Single(a=>a.name=="StudioBackgroundMusic");music.volume=.14f;
        PrefabUtility.RecordPrefabInstancePropertyModifications(music);
        RefineLightBalance(scene);
        if(group.GetComponentsInChildren<Collider>(true).Length!=0) throw new Exception("New decor collider found.");
        AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed");
    }
    static Color Hex(string hex){ColorUtility.TryParseHtmlString("#"+hex,out var c);return c;}
    static void RefineLightBalance(Scene scene)
    {
        // Keep depth without crushing the shaded walls or the ceiling.
        RenderSettings.ambientSkyColor=new Color(.62f,.67f,.70f);
        RenderSettings.ambientEquatorColor=new Color(.50f,.46f,.40f);
        RenderSettings.ambientGroundColor=new Color(.32f,.34f,.35f);
        var sun=All<Light>(scene).Single(l=>l.type==LightType.Directional);
        sun.color=Hex("FFEBD5");sun.intensity=1.35f;sun.shadowStrength=.55f;
        foreach(var item in new[]{("WarmPlaster","D9CEB9"),("SatinConcrete","C8BCAF")})
        {
            var mat=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/"+item.Item1+".mat");
            mat.SetColor("_BaseColor",Hex(item.Item2));EditorUtility.SetDirty(mat);
        }
        var glass=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/SkylightGlass.mat");
        glass.SetColor("_EmissionColor",Hex("C7DCE0")*.95f);EditorUtility.SetDirty(glass);
    }
    static T[] All<T>(Scene scene) where T:Component=>scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
    static Material Lit(string name,string hex,float smooth,float metal=0)
    {
        var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",Hex(hex));m.SetFloat("_Smoothness",smooth);m.SetFloat("_Metallic",metal);
        AssetDatabase.CreateAsset(m,Root+"/Materials/"+name+".mat");return m;
    }
    static Material LitOnce(string name,string hex,float smooth,float metal=0)=>AssetDatabase.LoadAssetAtPath<Material>(Root+"/Materials/"+name+".mat")??Lit(name,hex,smooth,metal);
    static Material Emissive(string name,string hex,float strength)
    {
        var m=LitOnce(name,hex,.35f);m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",Hex(hex)*strength);return m;
    }
    static Material CloneExisting(string source,string name,Color color,float smooth)
    {
        var m=new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/Generated/"+source+".mat")){name=name};
        m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",smooth);AssetDatabase.CreateAsset(m,Root+"/Materials/"+name+".mat");return m;
    }
    static void Part(Transform parent,string name,Vector3 position,Vector3 scale,Material mat)
    {
        var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        go.transform.SetParent(parent,false);go.transform.localPosition=position;go.transform.localScale=scale;
        go.GetComponent<MeshRenderer>().sharedMaterial=mat;
        if(name=="SkylightGlass"||name=="SkylightRecess")go.GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;
    }
    static Texture2D WindowCookie()
    {
        // Native light mask, not artwork: nine softly edged panes cast window light.
        var t=new Texture2D(128,128,TextureFormat.RGBA32,false,true){name="WindowLightMask",wrapMode=TextureWrapMode.Clamp};
        var pixels=new Color[128*128];
        for(int y=0;y<128;y++)for(int x=0;x<128;x++)
        {
            float u=x/127f,v=y/127f;
            float edge=Mathf.Min(Mathf.Min(u,1-u),Mathf.Min(v,1-v));
            float bars=Mathf.Min(Mathf.Abs(u-.333f),Mathf.Abs(u-.667f));
            bars=Mathf.Min(bars,Mathf.Min(Mathf.Abs(v-.333f),Mathf.Abs(v-.667f)));
            float a=Mathf.SmoothStep(0,1,Mathf.Clamp01((edge-.055f)/.04f))*Mathf.SmoothStep(0,1,Mathf.Clamp01((bars-.016f)/.008f));
            pixels[y*128+x]=new Color(a,a,a,a);
        }
        t.SetPixels(pixels);t.Apply();AssetDatabase.CreateAsset(t,Root+"/WindowLightMask.asset");return t;
    }
}
