using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Isolated, one-time scene/art preparation. Learner gameplay and prefab wiring stay intact.
public static class ExhibitionPolishBuilder
{
    const string Root = "Assets/StudioArtPack/ExhibitionV8";
    const string PreviewSource = ".local-backups/ExhibitionStudioPreview";
    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use an isolated batch project.");
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        if (scene.GetRootGameObjects().Any(g => g.name == "StudioRoomClosureV8")) throw new InvalidOperationException("V8 exists; preserve manual work.");
        foreach (string dir in new[]{"Meshes", "Materials", "Prefabs", "Previews", "Reflections"}) Directory.CreateDirectory(Root+"/"+dir);
        Directory.CreateDirectory(PreviewSource); AssetDatabase.Refresh();
        var gameplay = All<MonoBehaviour>(scene).Where(c => c != null && AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(c)).StartsWith("Assets/Scripts/",StringComparison.Ordinal)).ToArray();
        var beforeGameplay = gameplay.Select(EditorJsonUtility.ToJson).ToArray();
        var colliders = All<Collider>(scene); var beforeColliders = colliders.Select(EditorJsonUtility.ToJson).ToArray();
        var cream = Load<Material>("Assets/StudioArtPack/RoomFinishV7/Materials/CreamPlaster.mat");
        var floor = Load<Material>("Assets/StudioArtPack/RoomFinishV7/Materials/NaturalOakFloor.mat");
        var ceiling = Load<Material>("Assets/StudioArtPack/RoomFinishV7/Materials/IvoryCeiling.mat");
        var oak = Load<Material>("Assets/StudioArtPack/SurfacesV6/Materials/SatinOak.mat");
        foreach (string name in new[]{"MainWall","LeftWall","RightWall","LeftColumn","RightColumn"}) All<MeshRenderer>(scene).Single(r=>r.name==name).sharedMaterial=cream;
        var group = new GameObject("StudioRoomClosureV8").transform;
        // The original room ends at z=-2; the fixed camera is at z=-8.8.
        // Complete the shell behind that camera, rather than placing a wall across its view.
        Part(group,"BackWall",new Vector3(0,0,-9.9f),new Vector3(14,8,.2f),cream);
        Part(group,"LeftWallReturn",new Vector3(-7.1f,0,-5.95f),new Vector3(.2f,8,7.9f),cream);
        Part(group,"RightWallReturn",new Vector3(7.1f,0,-5.95f),new Vector3(.2f,8,7.9f),cream);
        Part(group,"CeilingReturn",new Vector3(0,4.1f,-5.95f),new Vector3(14,.2f,7.9f),ceiling);
        var extension = Part(group,"FloorReturn",new Vector3(0,-4.1f,-5.95f),new Vector3(14,.2f,7.9f),floor);
        var mesh=UnityEngine.Object.Instantiate(extension.GetComponent<MeshFilter>().sharedMesh);mesh.name="FloorReturnWorldUV";
        var uv=new Vector2[mesh.vertexCount];var vertices=mesh.vertices;var normals=mesh.normals;
        for(int i=0;i<uv.Length;i++) {
            var p=extension.TransformPoint(vertices[i]);var n=normals[i];
            uv[i]=(Mathf.Abs(n.y)>.5f?new Vector2(p.x,p.z):Mathf.Abs(n.x)>.5f?new Vector2(p.z,p.y):new Vector2(p.x,p.y))/2.8f;
        }
        mesh.uv=uv;AssetDatabase.CreateAsset(mesh,Root+"/Meshes/FloorReturnWorldUV.asset");extension.GetComponent<MeshFilter>().sharedMesh=mesh;
        Part(group,"BackSkirting",new Vector3(0,-3.83f,-9.74f),new Vector3(14,.3f,.12f),oak);
        foreach(float x in new[]{-6.96f,6.96f}) Part(group,"SideReturnSkirting",new Vector3(x,-3.83f,-5.95f),new Vector3(.12f,.3f,7.9f),oak);
        // Reuse the existing shadowless fill; no extra realtime light or shadow map.
        var fill=All<Light>(scene).Single(l=>l.name=="FrontFillLight");fill.intensity=12;fill.color=new Color(1,.965f,.90f);
        var probe=All<ReflectionProbe>(scene).Single(p=>p.name=="BakedStudioReflection");
        var gun=All<PlayerShooter>(scene).Single().gameObject;bool active=gun.activeSelf;
        try {
            gun.SetActive(false);string path=Root+"/Reflections/ClosedCreamStudio.exr";
            if(!Lightmapping.BakeReflectionProbe(probe,path))throw new InvalidOperationException("Reflection bake failed.");
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);probe.customBakedTexture=Load<Cubemap>(path);
        } finally {gun.SetActive(active);}
        if(group.GetComponentsInChildren<Collider>(true).Length!=0 || colliders.Length!=All<Collider>(scene).Length ||
           !beforeColliders.SequenceEqual(colliders.Select(EditorJsonUtility.ToJson)) || !beforeGameplay.SequenceEqual(gameplay.Select(EditorJsonUtility.ToJson)))
            throw new InvalidOperationException("Gameplay/collider state changed.");
        AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
        BuildPaintBurst("Teal",new Color(.04f,.65f,.68f));
        BuildPaintBurst("Coral",new Color(.95f,.31f,.21f));
        BuildPaintBurst("Gold",new Color(1,.69f,.06f));
        CaptureRoomAndEffects(scene);
        AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        Debug.Log("EXHIBITION_V8_OK: four cream sides; 8 collider-free shell/skirting renderers; 3 prepared paint-pop prefabs; gameplay unchanged.");
    }

    static Transform Part(Transform parent,string name,Vector3 position,Vector3 scale,Material material)
    {
        var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        go.transform.SetParent(parent,false);go.transform.position=position;go.transform.localScale=scale;
        var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;
        return go.transform;
    }

    static void BuildPaintBurst(string colorName,Color color)
    {
        string path=Root+"/Prefabs/PaintPop_"+colorName+".prefab";
        if(File.Exists(path))throw new InvalidOperationException("Refusing to overwrite "+path);
        var go=UnityEngine.Object.Instantiate(Load<GameObject>("Assets/StudioArtPack/BalloonPopV2/BalloonPop_"+colorName+".prefab"));
        go.name="PaintPop_"+colorName;
        try {
            var root=go.GetComponent<ParticleSystem>();root.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            root.useAutoRandomSeed=false;root.randomSeed=173;
            var main=root.main;main.stopAction=ParticleSystemStopAction.Destroy;
            var emission=root.emission;emission.SetBursts(new[]{new ParticleSystem.Burst(0,10)});
            var drops=new GameObject("PaintDroplets").AddComponent<ParticleSystem>();drops.transform.SetParent(go.transform,false);
            drops.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);drops.useAutoRandomSeed=false;drops.randomSeed=317;
            var m=drops.main;m.loop=false;m.duration=.65f;m.startLifetime=new ParticleSystem.MinMaxCurve(.38f,.58f);
            m.startSpeed=new ParticleSystem.MinMaxCurve(2.4f,5.2f);m.startSize=new ParticleSystem.MinMaxCurve(.07f,.16f);
            m.startColor=color;m.gravityModifier=.45f;m.simulationSpace=ParticleSystemSimulationSpace.World;m.maxParticles=24;m.playOnAwake=true;
            m.stopAction=ParticleSystemStopAction.None;
            var e=drops.emission;e.rateOverTime=0;e.SetBursts(new[]{new ParticleSystem.Burst(0,22)});
            var shape=drops.shape;shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=.05f;
            var size=drops.sizeOverLifetime;size.enabled=true;size.size=new ParticleSystem.MinMaxCurve(1,AnimationCurve.EaseInOut(0,1,1,.05f));
            var renderer=drops.GetComponent<ParticleSystemRenderer>();renderer.renderMode=ParticleSystemRenderMode.Mesh;
            var sphere=GameObject.CreatePrimitive(PrimitiveType.Sphere);renderer.mesh=sphere.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(sphere);
            var material=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit")){name="PaintDroplets_"+colorName};
            material.SetColor("_BaseColor",Color.white);AssetDatabase.CreateAsset(material,Root+"/Materials/"+material.name+".mat");
            renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
            // All visible children end before the root's original 0.7 s duration; Balloon's 2 s cleanup remains valid.
            if(go.GetComponentsInChildren<Collider>().Length!=0 || go.GetComponentsInChildren<MonoBehaviour>().Length!=0)throw new InvalidOperationException("Unexpected runtime behavior on art prefab.");
            PrefabUtility.SaveAsPrefabAsset(go,path);
        } finally {UnityEngine.Object.DestroyImmediate(go);}
    }

    static void CaptureRoomAndEffects(Scene scene)
    {
        Capture(scene,"Room_GameCamera_v8");
        var camera=All<Camera>(scene).Single(c=>c.CompareTag("MainCamera"));var position=camera.transform.position;var rotation=camera.transform.rotation;
        var gun=All<PlayerShooter>(scene).Single().gameObject;bool active=gun.activeSelf;
        try {
            gun.SetActive(false);camera.transform.position=new Vector3(0,0,1);
            foreach(var side in new[]{("Front",0f),("Right",90f),("Back",180f),("Left",270f)}) {
                camera.transform.rotation=Quaternion.Euler(0,side.Item2,0);Capture(scene,"Wall_"+side.Item1+"_v8");
            }
        } finally {camera.transform.SetPositionAndRotation(position,rotation);gun.SetActive(active);}
        var effects=new GameObject[3];
        try {
            var colors=new[]{"Teal","Coral","Gold"};
            for(int i=0;i<3;i++) {
                effects[i]=UnityEngine.Object.Instantiate(Load<GameObject>(Root+"/Prefabs/PaintPop_"+colors[i]+".prefab"),new Vector3((i-1)*2.8f,.25f,8),Quaternion.identity);
                effects[i].GetComponent<ParticleSystem>().Simulate(.18f,true,true,true);
                int count=effects[i].GetComponentsInChildren<ParticleSystem>().Sum(p=>p.particleCount);
                if(count!=32)throw new InvalidOperationException("Expected 32 particles at burst peak, got "+count);
            }
            Capture(scene,"PaintPop_Peak_v8");
            foreach(var effect in effects) {
                var ps=effect.GetComponent<ParticleSystem>();ps.Simulate(2,true,true,true);
                // Simulate pauses the systems; IsAlive can remain true for a paused emitter with zero particles.
                // This verifies visible lifetime only. Destruction and pause behavior need the Play Mode acceptance check.
                if(effect.GetComponentsInChildren<ParticleSystem>().Any(p=>p.particleCount!=0 || p.main.loop))
                    throw new InvalidOperationException("Paint particles remained after 2 s simulation.");
            }
        } finally {foreach(var effect in effects)if(effect!=null)UnityEngine.Object.DestroyImmediate(effect);}
    }
    static void Capture(Scene scene,string name)
    {
        ExhibitionStudioBuilder.Capture(scene,name+".png");File.Copy(PreviewSource+"/"+name+".png",Root+"/Previews/"+name+".png",true);
    }
    public static void ValidateAndCaptureBatch()
    {
        if(!Application.isBatchMode)throw new InvalidOperationException("Use isolated batch mode.");
        CaptureRoomAndEffects(EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity"));
        AssetDatabase.Refresh();Debug.Log("EXHIBITION_V8_CAPTURE_OK");
    }
    static T Load<T>(string path) where T:UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new FileNotFoundException(path);
    static T[] All<T>(Scene scene) where T:Component => scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
}
