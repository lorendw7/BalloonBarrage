using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Editor-only art pass: leaves the learner's gameplay and audio wiring intact.
public static class CartoonStudioPolisher
{
    const string Root = "Assets/StudioArtPack/CartoonV2";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    static int meshIndex;
    static Material cream, mint, teal, coral, gold, lavender, indigo, wood, dark, green;

    [MenuItem("Tools/Balloon Studio/Apply Cartoon Studio V2")]
    public static void Menu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().path != ScenePath)
        { Debug.LogWarning("Stop Play and open PrototypeScene first."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Apply(SceneManager.GetActiveScene());
    }

    public static void BuildBatch()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ExhibitionStudioBuilder.Capture(scene, "cartoon-before.png");
        Apply(scene);
        ExhibitionStudioBuilder.Capture(scene, "cartoon-after.png");
        ExhibitionStudioBuilder.CaptureTargetLayout(scene);
        ExhibitionStudioBuilder.ValidateAim(scene);
        Debug.Log("CARTOON_STUDIO_OK: scene saved, toon shader compiled, decor and gameplay references checked.");
    }

    static void Apply(Scene scene)
    {
        if (scene.GetRootGameObjects().Any(g => g.name == "CartoonStudioV2"))
            throw new InvalidOperationException("Cartoon V2 already exists; adjust its objects directly to preserve manual edits.");
        if (!scene.GetRootGameObjects().Any(g => g.name == "ExhibitionStudioV1"))
            throw new InvalidOperationException("Apply the exhibition studio first.");
        string backup = ".local-backups/scene-before-cartoon-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".unity";
        Directory.CreateDirectory(".local-backups");
        File.Copy(ScenePath, backup, false);
        File.Copy(ScenePath + ".meta", backup + ".meta", false);
        Directory.CreateDirectory(Root + "/Materials");
        Directory.CreateDirectory(Root + "/Meshes");
        Directory.CreateDirectory(Root + "/Prefabs");
        AssetDatabase.Refresh();
        meshIndex = 0;
        cream = Paint("Cream", "F4E6D0"); mint = Paint("Mint", "B5DCCC");
        teal = Paint("Teal", "3EC6C0"); coral = Paint("Coral", "FF897B");
        gold = Paint("Gold", "FFD166"); lavender = Paint("Lavender", "C6C2E1");
        indigo = Paint("Indigo", "777CAF"); wood = Paint("Honey", "DEA55E");
        dark = Paint("Ink", "394153"); green = Paint("Leaf", "73B68A");
        foreach (var renderer in All<MeshRenderer>(scene))
        {
            Material replacement = null;
            switch (renderer.name)
            {
                case "TopBeam": case "LeftColumn": case "RightColumn": case "Stage": replacement = wood; break;
                case "Floor": replacement = cream; break;
                case "LeftWall": replacement = mint; break;
                case "RightWall": replacement = lavender; break;
                case "Ceiling": replacement = cream; break;
                case "RoofBeam": replacement = wood; break;
                case "Tabletop": replacement = gold; break;
                case "Leg": replacement = coral; break;
                case "LowerShelf": replacement = teal; break;
                case "LeftSpeakerShelf": replacement = teal; break;
                case "LeftFrame": case "RightFrame": case "TopFrame": case "BottomFrame": replacement = wood; break;
                case "Backing": replacement = coral; break;
                case "LampShade": replacement = indigo; break;
                case "LampLens": replacement = gold; break;
            }
            if (replacement == null) continue;
            renderer.sharedMaterial = replacement;
            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
            if (new[] { "Tabletop", "Leg", "LowerShelf", "RoofBeam", "LeftSpeakerShelf" }.Contains(renderer.name))
            {
                var filter = renderer.GetComponent<MeshFilter>();
                filter.sharedMesh = RoundedMesh(renderer.name, renderer.transform.localScale, .08f);
                renderer.transform.localScale = Vector3.one;
                PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
                PrefabUtility.RecordPrefabInstancePropertyModifications(renderer.transform);
            }
        }
        var group = new GameObject("CartoonStudioV2").transform;
        SceneManager.MoveGameObjectToScene(group.gameObject, scene);

        // Layered frame, side lights and low panelling echo the website concept.
        Box(group, "TealInnerTrim", new Vector3(-5.91f,.2f,9.65f), new Vector3(.12f,5.9f,.16f), teal);
        Box(group, "CoralInnerTrim", new Vector3(5.91f,.2f,9.65f), new Vector3(.12f,5.9f,.16f), coral);
        Box(group, "GoldTopTrim", new Vector3(0,3.2f,9.62f), new Vector3(11.9f,.10f,.16f), gold);
        Box(group, "TealStageFront", new Vector3(0,-3.04f,7.86f), new Vector3(12.5f,.22f,.12f), teal);
        Box(group, "LavenderSkirting", new Vector3(0,-3.65f,9.85f), new Vector3(13f,.26f,.13f), lavender);
        Box(group, "LeftWainscot", new Vector3(-6.82f,-2.85f,4.5f), new Vector3(.15f,2.1f,10), teal);
        Box(group, "RightWainscot", new Vector3(6.82f,-2.85f,4.5f), new Vector3(.15f,2.1f,10), indigo);
        Box(group, "LeftLightHousing", new Vector3(-6.25f,.3f,8.9f), new Vector3(.24f,3.5f,.24f), dark);
        Box(group, "LeftLightStrip", new Vector3(-6.25f,.3f,8.74f), new Vector3(.14f,3.25f,.12f), teal);
        Box(group, "RightLightHousing", new Vector3(6.25f,.3f,8.9f), new Vector3(.24f,3.5f,.24f), dark);
        Box(group, "RightLightStrip", new Vector3(6.25f,.3f,8.74f), new Vector3(.14f,3.25f,.12f), coral);

        var crates = new GameObject("PaintCrates").transform;
        crates.SetParent(group, false);
        Crate(crates, new Vector3(-5.3f,-3.47f,8.5f), new Vector3(1.35f,1,1), wood);
        Crate(crates, new Vector3(5.2f,-3.47f,8.5f), new Vector3(1.25f,1,1), coral);
        SavePrefab(crates.gameObject, "PaintCrates");
        var buckets = new GameObject("PaintBuckets").transform;
        buckets.SetParent(group, false);
        Bucket(buckets, new Vector3(-5.55f,-3.94f,3.2f), gold, .8f);
        Bucket(buckets, new Vector3(-4.55f,-3.94f,4.0f), teal, 1);
        Bucket(buckets, new Vector3(5.35f,-2.94f,8.5f), gold, .58f);
        Bucket(buckets, new Vector3(-5.6f,-2.94f,8.5f), coral, .60f);
        SavePrefab(buckets.gameObject, "PaintBuckets");

        var easel = new GameObject("PaintEasel").transform;
        easel.SetParent(group, false);
        easel.localPosition = new Vector3(6.0f,-3.9f,5.6f);
        easel.localRotation = Quaternion.Euler(0,-22,0);
        var leftLeg = Box(easel,"LeftLeg",new Vector3(-.44f,1,0),new Vector3(.13f,2,.15f),wood);
        leftLeg.localRotation = Quaternion.Euler(0,0,-9);
        var rightLeg = Box(easel,"RightLeg",new Vector3(.44f,1,0),new Vector3(.13f,2,.15f),wood);
        rightLeg.localRotation = Quaternion.Euler(0,0,9);
        Box(easel,"CanvasRim",new Vector3(0,1.65f,-.07f),new Vector3(1.3f,1.6f,.12f),indigo);
        Box(easel,"BlankCanvas",new Vector3(0,1.65f,-.15f),new Vector3(1.12f,1.42f,.055f),cream);
        Box(easel,"CanvasRail",new Vector3(0,.84f,-.16f),new Vector3(1.5f,.13f,.27f),gold);
        for (int i=0;i<3;i++)
            Primitive(easel,"PaintSwatch",PrimitiveType.Sphere,new Vector3((i-1)*.29f,1.6f+(i%2)*.25f,-.20f),new Vector3(.39f,.62f,.06f),new[]{teal,coral,gold}[i]);
        SavePrefab(easel.gameObject, "PaintEasel");

        // Two stereo shelves, books, brushes and vines add medium/small detail.
        Box(group,"RightSpeakerShelf",new Vector3(5.05f,-.9f,9.42f),new Vector3(1.4f,.14f,.9f),coral);
        Existing("Speaker",group,new Vector3(5.05f,-.83f,9.42f),.85f);
        Box(group,"WindowShelf",new Vector3(-6.38f,1.8f,6.9f),new Vector3(.8f,.12f,1.7f),gold);
        Existing("PottedPlant",group,new Vector3(-6.35f,1.86f,6.8f),.48f);
        Vine(group,new Vector3(-6.10f,2.25f,5.5f),11);
        Vine(group,new Vector3(5.68f,2.5f,9.2f),9);
        var brushCup = new GameObject("BrushCup").transform;
        brushCup.SetParent(group,false); brushCup.localPosition=new Vector3(-4.5f,-2.35f,6.1f);
        Primitive(brushCup,"Cup",PrimitiveType.Cylinder,new Vector3(0,.18f,0),new Vector3(.30f,.18f,.3f),indigo);
        for(int i=0;i<4;i++)
        {
            var brush = Box(brushCup,"BrushHandle",new Vector3((i-1.5f)*.06f,.58f,0),new Vector3(.035f,.55f,.035f),wood);
            brush.localRotation = Quaternion.Euler(0,0,(i-1.5f)*9);
            Box(brushCup,"BrushTip",new Vector3((i-1.5f)*.13f,.9f,0),new Vector3(.065f,.15f,.065f),new[]{teal,coral,gold,cream}[i]);
        }
        SavePrefab(brushCup.gameObject,"BrushCup");
        for(int i=0;i<4;i++)
            Box(group,"ArtBook",new Vector3(-5.9f+i*.18f,-2.2f,6.15f),new Vector3(.14f,.34f+i*.07f,.43f),new[]{teal,coral,indigo,cream}[i]);

        // Warm floor swatches give depth without filling the central target wall.
        var rugs = new GameObject("PaintFloorAccents").transform; rugs.SetParent(group,false);
        Primitive(rugs,"RugBase",PrimitiveType.Cube,new Vector3(0,-3.885f,4.2f),new Vector3(6.7f,.018f,1.2f),indigo);
        for(int i=0;i<9;i++) Box(rugs,"RugStripe",new Vector3(-3.12f+i*.78f,-3.87f,4.2f),new Vector3(.3f,.012f,1.17f),i%2==0?coral:gold);
        foreach(var p in new[]{new Vector3(-3.8f,-3.88f,6.6f),new Vector3(4.5f,-3.88f,5.5f),new Vector3(-5,-3.88f,2)})
        {
            Primitive(rugs,"FloorPaint",PrimitiveType.Sphere,p,new Vector3(.7f,.014f,.95f),p.x<0?teal:coral);
            Primitive(rugs,"PaintDot",PrimitiveType.Sphere,p+new Vector3(.53f,0,.32f),new Vector3(.14f,.014f,.18f),gold);
        }
        SavePrefab(rugs.gameObject,"PaintFloorAccents");
        Pennants(group);
        AssignAnimeMusic(scene);
        foreach(var c in group.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(c);
        if(group.GetComponentsInChildren<Collider>(true).Length!=0) throw new InvalidOperationException("Decor collider remains.");
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        if(!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed.");
        var shader = Shader.Find("BalloonStudio/SoftToon");
        if(ShaderUtil.ShaderHasError(shader)) throw new InvalidOperationException("Toon shader compilation failed.");
    }

    static Material Paint(string name,string hex)
    {
        var shader=Shader.Find("BalloonStudio/SoftToon");
        if(shader==null) throw new InvalidOperationException("Missing toon shader.");
        ColorUtility.TryParseHtmlString("#"+hex,out var color);
        var mat=new Material(shader){name="Cartoon"+name}; mat.SetColor("_BaseColor",color);
        mat.SetColor("_ShadeColor",new Color(.78f,.82f,.97f));
        AssetDatabase.CreateAsset(mat,Root+"/Materials/"+name+".mat"); return mat;
    }
    static void AssignAnimeMusic(Scene scene)
    {
        foreach(string path in Directory.GetFiles("Assets/Audio/Music","*.wav"))
        {
            var importer=(AudioImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
            bool loop=Path.GetFileNameWithoutExtension(path).EndsWith("_Loop",StringComparison.Ordinal);
            var settings=importer.defaultSampleSettings;
            settings.loadType=loop?AudioClipLoadType.Streaming:AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat=loop?AudioCompressionFormat.Vorbis:AudioCompressionFormat.PCM;
            settings.quality=.7f;settings.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;
            importer.defaultSampleSettings=settings;importer.forceToMono=false;
            importer.loadInBackground=loop;importer.SaveAndReimport();
        }
        var clip=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music/StudioDaylight_Loop.wav");
        if(clip==null) throw new InvalidOperationException("Missing new anime-style music.");
        var source=All<AudioSource>(scene).Single(a=>a.name=="StudioBackgroundMusic");
        source.clip=clip;source.volume=.14f;
        PrefabUtility.RecordPrefabInstancePropertyModifications(source);
        var musicPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StudioArtPack/ExhibitionV1/Prefabs/StudioBackgroundMusic.prefab");
        var contents=PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(musicPrefab));
        try
        {
            contents.GetComponent<AudioSource>().clip=clip;contents.GetComponent<AudioSource>().volume=.14f;
            PrefabUtility.SaveAsPrefabAsset(contents,AssetDatabase.GetAssetPath(musicPrefab));
        }
        finally{PrefabUtility.UnloadPrefabContents(contents);}
    }
    static T[] All<T>(Scene scene) where T:Component => scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
    static Transform Box(Transform parent,string name,Vector3 p,Vector3 size,Material mat)
    {
        var go=new GameObject(name); go.transform.SetParent(parent,false); go.transform.localPosition=p;
        go.AddComponent<MeshFilter>().sharedMesh=RoundedMesh(name,size,.06f);
        var r=go.AddComponent<MeshRenderer>(); r.sharedMaterial=mat; r.shadowCastingMode=ShadowCastingMode.Off;
        return go.transform;
    }
    static Transform Primitive(Transform parent,string name,PrimitiveType type,Vector3 p,Vector3 size,Material mat)
    {
        var go=GameObject.CreatePrimitive(type); UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        go.name=name; go.transform.SetParent(parent,false); go.transform.localPosition=p; go.transform.localScale=size;
        var r=go.GetComponent<MeshRenderer>(); r.sharedMaterial=mat; r.shadowCastingMode=ShadowCastingMode.Off;
        return go.transform;
    }
    static Mesh RoundedMesh(string name,Vector3 size,float radius)
    {
        var h=size*.5f; radius=Mathf.Min(radius,Mathf.Min(h.x,Mathf.Min(h.y,h.z))*.7f);
        var inner=h-Vector3.one*radius; var vertices=new List<Vector3>();var normals=new List<Vector3>();var triangles=new List<int>();
        // Cube-face grid projected onto the rounded inner box.
        var faces=new[]{Vector3.right,Vector3.left,Vector3.up,Vector3.down,Vector3.forward,Vector3.back};
        const int steps=6;
        foreach(var normal in faces)
        {
            var u=Mathf.Abs(normal.y)>.5f?Vector3.right:Vector3.up;
            var v=Vector3.Cross(normal,u);int start=vertices.Count;
            for(int y=0;y<=steps;y++)for(int x=0;x<=steps;x++)
            {
                var q=Vector3.Scale(normal+u*(x*2f/steps-1)+v*(y*2f/steps-1),h);
                var nearest=new Vector3(Mathf.Clamp(q.x,-inner.x,inner.x),Mathf.Clamp(q.y,-inner.y,inner.y),Mathf.Clamp(q.z,-inner.z,inner.z));
                var n=(q-nearest).normalized;vertices.Add(nearest+n*radius);normals.Add(n);
            }
            for(int y=0;y<steps;y++)for(int x=0;x<steps;x++)
            {
                int a=start+y*(steps+1)+x,b=a+1,c=a+steps+1,d=c+1;
                triangles.AddRange(new[]{a,b,c,b,d,c});
            }
        }
        var mesh=new Mesh{name=name+"Rounded"};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();
        AssetDatabase.CreateAsset(mesh,Root+"/Meshes/"+(meshIndex++).ToString("D3")+"_"+name+".asset");return mesh;
    }
    static void SavePrefab(GameObject go,string name)
    { PrefabUtility.SaveAsPrefabAsset(go,Root+"/Prefabs/"+name+".prefab"); }
    static void Existing(string name,Transform parent,Vector3 p,float scale)
    {
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/StudioArtPack/Generated/"+name+".prefab");
        var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab,parent);go.transform.localPosition=p;go.transform.localScale=Vector3.one*scale;
    }
    static void Crate(Transform parent,Vector3 p,Vector3 size,Material mat)
    {
        Box(parent,"CrateBody",p,size,mat);
        for(int i=0;i<3;i++) Box(parent,"CrateSlat",p+new Vector3(0,(i-1)*.27f,-size.z*.51f),new Vector3(size.x+.04f,.08f,.05f),cream);
    }
    static void Bucket(Transform parent,Vector3 p,Material mat,float scale)
    {
        var go=new GameObject("PaintBucket").transform;go.SetParent(parent,false);go.localPosition=p;go.localScale=Vector3.one*scale;
        Primitive(go,"BucketBody",PrimitiveType.Cylinder,new Vector3(0,.42f,0),new Vector3(.82f,.42f,.82f),mat);
        Primitive(go,"LidRim",PrimitiveType.Cylinder,new Vector3(0,.88f,0),new Vector3(.92f,.035f,.92f),cream);
        Primitive(go,"LidPaint",PrimitiveType.Cylinder,new Vector3(0,.92f,0),new Vector3(.78f,.012f,.78f),mat);
        Box(go,"BucketLabel",new Vector3(0,.40f,-.408f),new Vector3(.34f,.25f,.024f),cream);
        for(int i=0;i<3;i++)Primitive(go,"PaintDrip",PrimitiveType.Capsule,new Vector3((i-1)*.19f,.64f,-.41f),new Vector3(.065f,.11f+i*.025f,.032f),cream);
    }
    static void Vine(Transform parent,Vector3 p,int count)
    {
        for(int i=0;i<count;i++)
        {
            var leaf=Primitive(parent,"HangingLeaf",PrimitiveType.Sphere,p+new Vector3(Mathf.Sin(i*1.8f)*.15f,-i*.16f,0),new Vector3(.24f,.30f,.10f),i%3==0?mint:green);
            leaf.localRotation=Quaternion.Euler(0,0,i%2==0?35:-35);
        }
    }
    static void Pennants(Transform parent)
    {
        var go=new GameObject("FestivalPennants").transform;go.SetParent(parent,false);
        for(int i=0;i<9;i++)
        {
            float x=-4.4f+i*1.1f,y=2.8f-Mathf.Sin(i/8f*Mathf.PI)*.30f;
            Box(go,"FlagCord",new Vector3(x,y+.13f,9.3f),new Vector3(1.16f,.018f,.018f),dark);
            var mesh=new Mesh{name="Pennant"};mesh.vertices=new[]{new Vector3(-.27f,0,0),new Vector3(.27f,0,0),new Vector3(0,-.42f,0)};
            mesh.triangles=new[]{0,1,2};mesh.RecalculateNormals();mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh,Root+"/Meshes/Flag"+i+".asset");
            var flag=new GameObject("Flag");flag.transform.SetParent(go,false);flag.transform.localPosition=new Vector3(x,y,9.27f);
            flag.AddComponent<MeshFilter>().sharedMesh=mesh;var r=flag.AddComponent<MeshRenderer>();r.sharedMaterial=new[]{coral,teal,gold,lavender}[i%4];r.shadowCastingMode=ShadowCastingMode.Off;
        }
        SavePrefab(go.gameObject,"FestivalPennants");
    }
}
