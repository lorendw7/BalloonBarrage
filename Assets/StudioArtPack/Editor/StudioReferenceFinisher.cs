using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Art-only refinement toward the website's cozy studio reference.
public static class StudioReferenceFinisher
{
    const string Root="Assets/StudioArtPack/ReferenceV4";
    const string ScenePath="Assets/Scenes/PrototypeScene.unity";
    public static void BuildBatch()
    {
        var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        ExhibitionStudioBuilder.Capture(scene,"reference-before.png");
        Apply(scene);
        ExhibitionStudioBuilder.Capture(scene,"reference-after.png");
        ExhibitionStudioBuilder.CaptureTargetLayout(scene);ExhibitionStudioBuilder.ValidateAim(scene);
        Debug.Log("REFERENCE_STUDIO_OK: materials, alcoves, foliage, light and framing validated.");
    }
    public static void RefreshFoliageBatch()
    {
        var scene=EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
        var current=AssetDatabase.LoadAssetAtPath<Mesh>(Root+"/Meshes/CurvedLeaf.asset");
        if(current==null)throw new Exception("Missing leaf mesh.");
        var corrected=LeafMesh();current.Clear();current.vertices=corrected.vertices;current.normals=corrected.normals;
        current.uv=corrected.uv;current.triangles=corrected.triangles;current.RecalculateBounds();current.UploadMeshData(false);
        UnityEngine.Object.DestroyImmediate(corrected);
        foreach(var filter in All<MeshFilter>(scene).Where(f=>f.sharedMesh==current))
        {filter.sharedMesh=null;filter.sharedMesh=current;filter.GetComponent<Renderer>().ResetBounds();filter.GetComponent<Renderer>().ResetLocalBounds();}
        Debug.Log("LEAF_GEOMETRY: "+current.vertexCount+" vertices; bounds "+current.bounds);
        EditorUtility.SetDirty(current);AssetDatabase.SaveAssets();
        ExhibitionStudioBuilder.Capture(scene,"reference-after.png");ExhibitionStudioBuilder.CaptureTargetLayout(scene);ExhibitionStudioBuilder.ValidateAim(scene);
        Debug.Log("REFERENCE_STUDIO_OK: finite foliage geometry and final render validated.");
    }
    static void Apply(Scene scene)
    {
        if(scene.GetRootGameObjects().Any(g=>g.name=="StudioReferenceV4"))throw new Exception("V4 already exists; preserve manual edits.");
        Directory.CreateDirectory(".local-backups");
        File.Copy(ScenePath,".local-backups/scene-before-reference-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".unity",false);
        Directory.CreateDirectory(Root+"/Materials");Directory.CreateDirectory(Root+"/Meshes");AssetDatabase.Refresh();
        var group=new GameObject("StudioReferenceV4").transform;
        var oak=Lit("FineOak","C5A17B",.27f);
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/FineOak_v4.png");
        if(texture==null)throw new Exception("Missing generated fine oak texture.");
        var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
        importer.wrapMode=TextureWrapMode.Repeat;importer.mipmapEnabled=true;importer.anisoLevel=4;importer.SaveAndReimport();
        oak.SetTexture("_BaseMap",texture);oak.mainTextureScale=new Vector2(1.5f,2);
        var cream=Lit("LimePlaster","D8CDB7",.13f);
        cream.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/StudioArtPack/Textures/WarmConcrete.png"));
        cream.mainTextureScale=new Vector2(3,3);
        var niche=Lit("NicheTeal","315E5C",.22f);
        var leafDark=Lit("LeafForest","365B35",.30f);leafDark.SetFloat("_Cull",0);
        var leafLight=Lit("LeafOlive","697A43",.28f);leafLight.SetFloat("_Cull",0);
        var arch=ArchMesh();AssetDatabase.CreateAsset(arch,Root+"/Meshes/NicheArch.asset");
        var leaf=LeafMesh();AssetDatabase.CreateAsset(leaf,Root+"/Meshes/CurvedLeaf.asset");
        int leafIndex=0;
        foreach(var r in All<MeshRenderer>(scene))
        {
            if(r.GetComponentInParent<PlayerShooter>()!=null)continue;
            var mats=r.sharedMaterials;
            for(int i=0;i<mats.Length;i++)if(mats[i]!=null&&mats[i].name=="WalnutWood")mats[i]=oak;
            r.sharedMaterials=mats;
            if(new[]{"LeftColumn","RightColumn","LeftWall","RightWall","Ceiling"}.Contains(r.name))r.sharedMaterial=cream;
            if(new[]{"LeftWainscot","RightWainscot"}.Contains(r.name))r.gameObject.SetActive(false);
            if(r.name=="HangingLeaf"||r.name.StartsWith("Leaf",StringComparison.Ordinal))
            {
                var mf=r.GetComponent<MeshFilter>();if(mf!=null){mf.sharedMesh=leaf;PrefabUtility.RecordPrefabInstancePropertyModifications(mf);}
                r.sharedMaterial=leafIndex%3==0?leafLight:leafDark;
                r.transform.localScale=r.name=="HangingLeaf"?new Vector3(.32f,.48f,.65f):new Vector3(.46f,.88f,.8f);
                r.transform.localRotation*=Quaternion.Euler((leafIndex%3-1)*17,(leafIndex%4-1.5f)*21,0);
                PrefabUtility.RecordPrefabInstancePropertyModifications(r.transform);leafIndex++;
            }
            // The skylight should read as illumination, not a giant dark grid on the target wall.
            if(r.name=="SkylightBeam"||r.name=="SkylightCrossbar")r.shadowCastingMode=ShadowCastingMode.Off;
            PrefabUtility.RecordPrefabInstancePropertyModifications(r);
        }
        foreach(float x in new[]{-5.1f,5.1f})
        {
            var go=new GameObject("ArchedStudioNiche");go.transform.SetParent(group,false);
            go.transform.localPosition=new Vector3(x,1.65f,9.77f);
            go.AddComponent<MeshFilter>().sharedMesh=arch;go.AddComponent<MeshRenderer>().sharedMaterial=niche;
            Part(group,"NicheShelf",new Vector3(x,-1.9f,9.34f),new Vector3(1.5f,.13f,.94f),oak);
            Part(group,"NicheSill",new Vector3(x,-2.76f,9.48f),new Vector3(1.65f,.17f,.72f),oak);
            for(int i=0;i<3;i++)
            {
                var can=GameObject.CreatePrimitive(PrimitiveType.Cylinder);can.name="NichePaintTin";UnityEngine.Object.DestroyImmediate(can.GetComponent<Collider>());
                can.transform.SetParent(group,false);can.transform.localPosition=new Vector3(x+(i-1)*.34f,-1.63f,9.26f);can.transform.localScale=new Vector3(.29f,.20f,.29f);
                can.GetComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/AtmosphereV3/Materials/"+new[]{"TealPaint","GoldPaint","CoralPaint"}[i]+".mat");
            }
        }
        // A pair of side-lit ceramic planters gives scale and depth at the window.
        var pot=Lit("GlazedTerracotta","AA674C",.32f);
        for(int side=0;side<2;side++)
        {
            var plant=new GameObject("WindowFern").transform;plant.SetParent(group,false);
            plant.localPosition=side==0?new Vector3(-6.15f,-2.35f,5.95f):new Vector3(-6.4f,.15f,6.7f);
            if(side==1)Part(group,"WindowPlantLedge",new Vector3(-6.38f,.09f,6.7f),new Vector3(1.1f,.12f,1.4f),oak);
            var basePot=GameObject.CreatePrimitive(PrimitiveType.Cylinder);UnityEngine.Object.DestroyImmediate(basePot.GetComponent<Collider>());
            basePot.name="CeramicPot";basePot.transform.SetParent(plant,false);basePot.transform.localPosition=new Vector3(0,.20f,0);basePot.transform.localScale=new Vector3(.55f,.20f,.55f);basePot.GetComponent<MeshRenderer>().sharedMaterial=pot;
            for(int i=0;i<9;i++)
            {
                var branch=new GameObject("FernLeaf");branch.transform.SetParent(plant,false);
                float angle=i*137.5f;branch.transform.localPosition=new Vector3(Mathf.Sin(angle*Mathf.Deg2Rad)*.22f,.74f+ (i%3)*.12f,Mathf.Cos(angle*Mathf.Deg2Rad)*.2f);
                branch.transform.localRotation=Quaternion.Euler(20+(i%3)*15,angle,30);branch.transform.localScale=new Vector3(.42f,.95f,.9f);
                branch.AddComponent<MeshFilter>().sharedMesh=leaf;branch.AddComponent<MeshRenderer>().sharedMaterial=i%3==0?leafLight:leafDark;
            }
        }
        // Balanced window light and local shelf pools, without flattening the whole room.
        var sun=All<Light>(scene).Single(l=>l.type==LightType.Directional);sun.intensity=1.2f;sun.shadowStrength=.40f;
        sun.color=Hex("FFF0DC");
        RenderSettings.ambientSkyColor=new Color(.66f,.70f,.72f);
        RenderSettings.ambientEquatorColor=new Color(.53f,.48f,.41f);
        var window=All<Light>(scene).Single(l=>l.name=="WindowDaylight");window.intensity=100;window.shadowStrength=.45f;
        foreach(float x in new[]{-5.1f,5.1f})
        {
            var lamp=new GameObject("NicheWarmLight").AddComponent<Light>();lamp.transform.SetParent(group,false);
            lamp.type=LightType.Spot;lamp.transform.localPosition=new Vector3(x,2.3f,9.15f);lamp.transform.LookAt(new Vector3(x,-1,9.55f));
            lamp.color=Hex("FFE0A8");lamp.intensity=12;lamp.range=5;lamp.spotAngle=75;lamp.innerSpotAngle=50;lamp.shadows=LightShadows.None;
        }
        var skylight=new Material(Shader.Find("Universal Render Pipeline/Unlit")){name="DaylightSky"};skylight.SetColor("_BaseColor",Hex("BFD8DC"));
        AssetDatabase.CreateAsset(skylight,Root+"/Materials/DaylightSky.mat");
        foreach(var r in All<MeshRenderer>(scene).Where(r=>r.name=="SkylightGlass"))r.sharedMaterial=skylight;
        if(group.GetComponentsInChildren<Collider>(true).Length!=0)throw new Exception("New decor intercepts aiming.");
        AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);if(!EditorSceneManager.SaveScene(scene))throw new IOException("Scene save failed");
    }
    static Color Hex(string value){ColorUtility.TryParseHtmlString("#"+value,out var color);return color;}
    static T[] All<T>(Scene scene)where T:Component=>scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
    static Material Lit(string name,string color,float smooth)
    {var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};m.SetColor("_BaseColor",Hex(color));m.SetFloat("_Smoothness",smooth);AssetDatabase.CreateAsset(m,Root+"/Materials/"+name+".mat");return m;}
    static void Part(Transform parent,string name,Vector3 p,Vector3 size,Material mat)
    {var go=GameObject.CreatePrimitive(PrimitiveType.Cube);UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=size;go.GetComponent<MeshRenderer>().sharedMaterial=mat;}
    static Mesh ArchMesh()
    {
        var vertices=new List<Vector3>{new Vector3(0,-1.6f,0),new Vector3(-.88f,-4.35f,0),new Vector3(.88f,-4.35f,0)};
        for(int i=0;i<=24;i++){float a=i*Mathf.PI/24;vertices.Add(new Vector3(Mathf.Cos(a)*.88f,Mathf.Sin(a)*.88f,0));}
        var triangles=new List<int>();for(int i=1;i<vertices.Count;i++)triangles.AddRange(new[]{0,i==vertices.Count-1?1:i+1,i});
        var mesh=new Mesh{name="ArchedNiche"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
    }
    static Mesh LeafMesh()
    {
        var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
        for(int i=0;i<=12;i++)
        {
            float t=i/12f,w=Mathf.Pow(Mathf.Max(0,Mathf.Sin(t*Mathf.PI)),.8f)*.5f;
            vertices.Add(new Vector3(-w,t-.5f,Mathf.Sin(t*Mathf.PI)*.16f));
            vertices.Add(new Vector3(0,t-.5f,Mathf.Sin(t*Mathf.PI)*-.06f));
            vertices.Add(new Vector3(w,t-.5f,Mathf.Sin(t*Mathf.PI)*.16f));
            uv.Add(new Vector2(0,t));uv.Add(new Vector2(.5f,t));uv.Add(new Vector2(1,t));
            if(i<12)for(int j=0;j<2;j++){int a=i*3+j;triangles.AddRange(new[]{a,a+3,a+1,a+1,a+3,a+4});}
        }
        if(vertices.Any(v=>float.IsNaN(v.x)||float.IsNaN(v.y)||float.IsNaN(v.z)))throw new Exception("Invalid leaf vertex");
        var mesh=new Mesh{name="CurvedPointedLeaf"};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
    }
}
