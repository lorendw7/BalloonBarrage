// Copy to an isolated project's Assets/Editor. Run Prepare (-quit), then Run (without -quit).
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public static class PaintCoverageRegression
{
    const string Active="PaintCoverageRegression.Active";
    static string Folder => Path.GetFullPath("../../Logs/PaintRegression20261031");
    static double ready;
    static RoomPaintSystem paint;
    static PaintProjectile projectile;
    static readonly List<string> failures=new List<string>();
    [Serializable] class Sample { public string name; public float minimumAlpha; public int samples; public bool passed; }
    [Serializable] class Report
    {
        public string scope="Isolated prepared graybox, actual MeshCollider contacts and GPU readback. Not a physical input or standalone FPS check.";
        public string status; public int checks; public List<Sample> cases=new List<Sample>(); public string[] failures;
    }
    static Report report;
    static PaintCoverageRegression() { if(SessionState.GetBool(Active,false)) EditorApplication.update+=Tick; }
    public static void Prepare()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var material=new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Directory.CreateDirectory("Assets/PaintRegression17"); AssetDatabase.Refresh();
        AssetDatabase.CreateAsset(material,"Assets/PaintRegression17/Gray.mat");
        Add("TestWall",PrimitiveType.Cube,new Vector3(0,2,2),new Vector3(6,4,.12f),material);
        Add("TestSide",PrimitiveType.Cube,new Vector3(-3.06f,2,0),new Vector3(.12f,4,4),material);
        Add("TestFloor",PrimitiveType.Cube,new Vector3(0,-.06f,0),new Vector3(6,.12f,4),material);
        Add("TestSphere",PrimitiveType.Sphere,new Vector3(8,1,0),Vector3.one*2,material);
        Add("TestCylinder",PrimitiveType.Cylinder,new Vector3(12,1,0),new Vector3(2,1,2),material);
        var camera=new GameObject("Camera",typeof(Camera));camera.tag="MainCamera";
        camera.transform.position=new Vector3(-.4f,2.5f,-5);camera.transform.LookAt(new Vector3(-2,1.2f,1.2f));
        var light=new GameObject("Light",typeof(Light));light.GetComponent<Light>().type=LightType.Directional;
        light.transform.rotation=Quaternion.Euler(40,-25,0);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),"Assets/Scenes/PrototypeScene.unity");
        BalloonBarrage.EditorTools.RoomPaintPreparation.Run();
    }
    static void Add(string name,PrimitiveType type,Vector3 position,Vector3 scale,Material material)
    { var node=GameObject.CreatePrimitive(type);node.name=name;node.transform.SetPositionAndRotation(position,Quaternion.identity);node.transform.localScale=scale;node.GetComponent<Renderer>().sharedMaterial=material; }
    public static void Run()
    {
        Directory.CreateDirectory(Folder);SessionState.SetBool(Active,true);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");EditorApplication.EnterPlaymode();
    }
    static void Tick()
    {
        if(!EditorApplication.isPlaying || !Application.isPlaying)return;
        if(ready==0){ready=EditorApplication.timeSinceStartup;return;}
        if(EditorApplication.timeSinceStartup-ready<1)return;
        report=new Report();
        try { Test(); } catch(Exception error){failures.Add(error.ToString());}
        report.failures=failures.ToArray();report.status=failures.Count==0?"passed":"failed";
        string[] args=Environment.GetCommandLineArgs();int at=Array.IndexOf(args,"-paintRegressionLabel");string label=at>=0?args[at+1]:"result";
        File.WriteAllText(Path.Combine(Folder,label+".json"),JsonUtility.ToJson(report,true));
        SessionState.SetBool(Active,false);EditorApplication.update-=Tick;
        Debug.Log("PAINT_COVERAGE_REGRESSION: "+report.status+"; "+report.checks+" checks.");EditorApplication.Exit(failures.Count==0?0:1);
    }
    static PaintSurface Surface(string name)=>Object.FindObjectsByType<PaintSurface>(FindObjectsSortMode.None).Single(s=>s.name==name);
    static RaycastHit Contact(PaintSurface surface,Vector3 point,Vector3 outward)
    {
        var collider=surface.GetComponents<MeshCollider>().Single(c=>c.enabled&&!c.isTrigger);
        if(!collider.Raycast(new Ray(point+outward*2,-outward),out RaycastHit hit,4))throw new Exception("Contact missing: "+surface.name+" "+point);
        return hit;
    }
    static Vector2 UV(PaintSurface surface,RaycastHit hit)
    {
        var mesh=((MeshCollider)hit.collider).sharedMesh;var original=mesh.vertices;var indices=mesh.triangles;
        var padded=surface.StampMesh.vertices;var uv=surface.StampMesh.uv4;int t=hit.triangleIndex*3;
        float expansion=(padded[t+1]-padded[t]).magnitude/(original[indices[t+1]]-original[indices[t]]).magnitude;
        Vector2 center=(uv[t]+uv[t+1]+uv[t+2])/3;Vector3 b=hit.barycentricCoordinate;
        return center+(uv[t]*b.x+uv[t+1]*b.y+uv[t+2]*b.z-center)/expansion;
    }
    static void Check(bool condition,string message){report.checks++;if(!condition)failures.Add(message);}
    static void Samples(string name,IEnumerable<RaycastHit> contacts,bool expectedPaint=true)
    {
        paint.Flush();var image=new Texture2D(paint.Atlas.width,paint.Atlas.height,TextureFormat.RGBA32,false,true);
        var prior=RenderTexture.active;RenderTexture.active=paint.Atlas;image.ReadPixels(new Rect(0,0,image.width,image.height),0,0);image.Apply();RenderTexture.active=prior;
        float minimum=1,maximum=0;int count=0;
        foreach(var contact in contacts)
        {
            Vector2 uv=UV(contact.collider.GetComponent<PaintSurface>(),contact);float alpha=image.GetPixelBilinear(uv.x,uv.y).a;
            minimum=Mathf.Min(minimum,alpha);maximum=Mathf.Max(maximum,alpha);count++;Check(expectedPaint?alpha>.65f:alpha<.05f,name+" sample "+count+" alpha="+alpha);
        }
        bool passed=expectedPaint?minimum>.65f:maximum<.05f;report.cases.Add(new Sample{name=name,minimumAlpha=minimum,samples=count,passed=passed});Object.DestroyImmediate(image);
    }
    static RaycastHit Arc(PaintSurface surface,float angle)
    {Vector3 n=new Vector3(Mathf.Sin(angle*Mathf.Deg2Rad),0,-Mathf.Cos(angle*Mathf.Deg2Rad));return Contact(surface,surface.transform.position+n,n);}
    static void Test()
    {
        paint=Object.FindFirstObjectByType<RoomPaintSystem>();
        string path=AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("PaintDrop3D_Teal t:Prefab").First());
        projectile=AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<PaintProjectile>();
        var wall=Surface("TestWall");var side=Surface("TestSide");var floor=Surface("TestFloor");
        RaycastHit a=Contact(wall,new Vector3(-1.1f,1.5f,1.94f),Vector3.back),b=Contact(wall,new Vector3(1.1f,1.5f,1.94f),Vector3.back);
        paint.Clear();projectile.SpawnPaintMark(a,0,77);projectile.SpawnPaintMark(b,0,77);
        Samples("flat rapid stroke",Enumerable.Range(0,17).Select(i=>Contact(wall,new Vector3(Mathf.Lerp(-1.1f,1.1f,i/16f),1.5f,1.94f),Vector3.back)));
        paint.Clear();projectile.SpawnPaintMark(a,0,77);RoomPaintSystem.EndStroke(77);projectile.SpawnPaintMark(b,0,77);
        Samples("release must break stroke",new[]{Contact(wall,new Vector3(0,1.5f,1.94f),Vector3.back)},false);
        paint.Clear();projectile.SpawnPaintMark(a,0,77);projectile.SpawnPaintMark(b,0,88);
        Samples("players stay separate",new[]{Contact(wall,new Vector3(0,1.5f,1.94f),Vector3.back)},false);
        paint.Clear();projectile.SpawnPaintMark(a,0,77);projectile.SpawnPaintMark(b,1,77);
        Samples("pigment change breaks stroke",new[]{Contact(wall,new Vector3(0,1.5f,1.94f),Vector3.back)},false);
        paint.Clear();projectile.SpawnPaintMark(Contact(wall,new Vector3(-2.3f,1,1.94f),Vector3.back),0,77);
        projectile.SpawnPaintMark(Contact(side,new Vector3(-3,1,1.24f),Vector3.right),0,77);
        Samples("rapid stroke across wall corner",new[]{Contact(wall,new Vector3(-2.98f,1,1.94f),Vector3.back),Contact(side,new Vector3(-3,1,1.92f),Vector3.right)});
        Capture("corner");
        paint.Clear();projectile.SpawnPaintMark(Contact(wall,new Vector3(1,.04f,1.94f),Vector3.back),0);
        Samples("wall floor adjacent spill",new[]{Contact(floor,new Vector3(1,0,1.90f),Vector3.up)});
        foreach(string name in new[]{"TestSphere","TestCylinder"})
        {
            var curve=Surface(name);paint.Clear();projectile.SpawnPaintMark(Arc(curve,-40),0,77);projectile.SpawnPaintMark(Arc(curve,40),0,77);
            Samples(name+" rapid curved stroke",Enumerable.Range(0,17).Select(i=>Arc(curve,Mathf.Lerp(-35,35,i/16f))));
            Capture(name,curve.transform.position);
            paint.Clear();for(int i=0;i<=28;i++)projectile.SpawnPaintMark(Arc(curve,-70+i*5),0,77);
            Samples(name+" fine sweep and UV seams",Enumerable.Range(0,55).Select(i=>Arc(curve,-67.5f+i*2.5f)));
            Samples(name+" opposite side stays clean",new[]{Arc(curve,180)},false);
        }
        var blocker=GameObject.CreatePrimitive(PrimitiveType.Cube);blocker.name="Unprepared occluder";
        blocker.transform.position=new Vector3(8,1,-1.2f);blocker.transform.localScale=new Vector3(.9f,2,.1f);Physics.SyncTransforms();
        var sphere=Surface("TestSphere");paint.Clear();projectile.SpawnPaintMark(Arc(sphere,-40),0,77);projectile.SpawnPaintMark(Arc(sphere,40),0,77);
        Samples("unrelated occluder blocks interpolated pigment",new[]{Arc(sphere,0)},false);
        Check(Score.Value==0,"Cosmetic paint changed score.");
    }
    static void Capture(string name,Vector3? focus=null)
    {
        Camera camera=Camera.main;Vector3 position=camera.transform.position;Quaternion rotation=camera.transform.rotation;
        if(focus.HasValue){camera.transform.position=focus.Value+Vector3.back*4.5f;camera.transform.LookAt(focus.Value);}
        var target=RenderTexture.GetTemporary(1600,900,24);var prior=RenderTexture.active;var old=camera.targetTexture;
        camera.targetTexture=target;camera.aspect=16f/9f;camera.Render();RenderTexture.active=target;
        var image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();
        string[] args=Environment.GetCommandLineArgs();int at=Array.IndexOf(args,"-paintRegressionLabel");string label=at>=0?args[at+1]:"result";
        File.WriteAllBytes(Path.Combine(Folder,name+"-"+label+".png"),image.EncodeToPNG());Object.DestroyImmediate(image);
        camera.targetTexture=old;RenderTexture.active=prior;RenderTexture.ReleaseTemporary(target);camera.transform.SetPositionAndRotation(position,rotation);
    }
}
#endif
