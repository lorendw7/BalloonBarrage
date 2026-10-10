using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Editor-only visual refinement. Imported source art and gameplay components remain intact.
public static class StudioSurfaceFinisher
{
    const string Root = "Assets/StudioArtPack/SurfacesV6";
    const string ScenePath = "Assets/Scenes/PrototypeScene.unity";
    const string GunPath = "Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab";
    const string BalloonPath = "Assets/Prefabs/PlayableBalloon.prefab";
    static Material oak, linen, steel, brass, ceramic, rubber, teal, cream, ochre, coral, latex, coffee, canvas;
    static Mesh ring, cup, tube;
    static int meshNumber;
    static bool buildingGun;
    static readonly List<Mesh> temporaryGunMeshes = new List<Mesh>();
    static readonly Dictionary<string, Mesh> boxes = new Dictionary<string, Mesh>();

    [MenuItem("Tools/Balloon Studio/Refine Models and Surfaces V6")]
    public static void Menu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || SceneManager.GetActiveScene().path != ScenePath)
        { Debug.LogWarning("Stop Play Mode and open PrototypeScene first."); return; }
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) Apply(SceneManager.GetActiveScene());
    }

    public static void BuildBatch()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Use the editor menu outside batch mode.");
        var scene = EditorSceneManager.OpenScene(ScenePath);
        Apply(scene);
        ExhibitionStudioBuilder.Capture(scene, "surfaces-room.png");
        ExhibitionStudioBuilder.ValidateAim(scene);
        CaptureCloseups(scene);
        Debug.Log("STUDIO_SURFACES_OK: refined gun, smooth balloon, bevels, distinct surfaces and room reflections.");
    }

    static void Apply(Scene scene)
    {
        if (!scene.GetRootGameObjects().Any(g => g.name == "StudioDetailsV5")) throw new InvalidOperationException("V5 scene required.");
        if (Directory.Exists(Root) || scene.GetRootGameObjects().Any(g => g.name == "StudioSurfacesV6"))
            throw new InvalidOperationException("V6 exists; edit its assets directly to preserve manual changes.");
        Directory.CreateDirectory(".local-backups/ExhibitionStudioPreview");
        File.Copy(ScenePath, ".local-backups/scene-before-surfaces-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".unity", false);
        foreach (string folder in new[] { "Materials", "Meshes", "Textures", "Reflections" }) Directory.CreateDirectory(Root + "/" + folder);
        AssetDatabase.Refresh(); boxes.Clear(); meshNumber = 0;
        CreateMaterials();
        ring = SaveMesh(Torus(.43f, .055f, 64, 16), "RoundedRing");
        cup = SaveMesh(Lathe(new[] { V(0,-.5f), V(.38f,-.5f), V(.43f,-.47f), V(.46f,-.40f), V(.50f,.42f), V(.50f,.47f), V(.48f,.5f), V(.43f,.5f), V(.41f,.47f), V(.40f,.40f), V(.36f,-.36f), V(.32f,-.40f), V(0,-.40f) }), "RolledCeramicCup");
        tube = SaveMesh(Lathe(new[] { V(.47f,-.5f), V(.5f,-.47f), V(.5f,.47f), V(.47f,.5f), V(.39f,.5f), V(.37f,.47f), V(.37f,-.47f), V(.39f,-.5f), V(.47f,-.5f) }), "RoundedPaperTube");
        RefineGun();
        RefineBalloon();
        // Prefab sources keep their GUIDs, while scene overrides are deliberately updated below.
        foreach (string path in Directory.GetFiles("Assets/StudioArtPack/DetailsV5/Prefabs", "*.prefab"))
        {
            var contents = PrefabUtility.LoadPrefabContents(path);
            try { RefineProps(contents.GetComponentsInChildren<MeshRenderer>(true)); PrefabUtility.SaveAsPrefabAsset(contents, path); }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
        }
        RefineProps(All<MeshRenderer>(scene).Where(r => r.GetComponentInParent<PlayerShooter>() == null));
        var gun = All<PlayerShooter>(scene).Single().transform;
        foreach (var r in gun.GetComponentsInChildren<MeshRenderer>(true))
        {
            r.enabled = r.transform.IsChildOf(gun.Find("RefinedGunVisual"));
            PrefabUtility.RecordPrefabInstancePropertyModifications(r);
        }
        var group = new GameObject("StudioSurfacesV6"); SceneManager.MoveGameObjectToScene(group, scene);
        var probe = new GameObject("BakedStudioReflection").AddComponent<ReflectionProbe>();
        probe.transform.SetParent(group.transform, false); probe.transform.position = new Vector3(0, 0, 4.8f);
        probe.mode = ReflectionProbeMode.Baked; probe.boxProjection = true; probe.size = new Vector3(13.5f, 8, 12);
        probe.resolution = 256; probe.nearClipPlane = .15f; probe.farClipPlane = 35; probe.intensity = .75f;
        probe.clearFlags = ReflectionProbeClearFlags.Skybox; probe.backgroundColor = new Color(.45f,.42f,.36f);
        probe.hdr = true;
        bool gunActive = gun.gameObject.activeSelf;
        try
        {
            gun.gameObject.SetActive(false);
            if (!Lightmapping.BakeReflectionProbe(probe, Root + "/Reflections/StudioInterior.exr")) throw new InvalidOperationException("Room reflection bake failed.");
        }
        finally { gun.gameObject.SetActive(gunActive); }
        AssetDatabase.ImportAsset(Root + "/Reflections/StudioInterior.exr", ImportAssetOptions.ForceSynchronousImport);
        // Custom mode persists an explicit asset reference without depending on a scene LightingDataAsset.
        probe.mode = ReflectionProbeMode.Custom;
        probe.customBakedTexture = AssetDatabase.LoadAssetAtPath<Cubemap>(Root + "/Reflections/StudioInterior.exr");
        if (probe.customBakedTexture == null) throw new InvalidOperationException("Reflection cubemap missing.");
        Validate(scene);
        AssetDatabase.SaveAssets(); EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed.");
    }

    static void CreateMaterials()
    {
        var woodMaps = SurfaceMaps("Oak", 0, .27f, .12f, 0);
        var clothMaps = SurfaceMaps("Linen", 1, .10f, .05f, 0);
        var metalMaps = SurfaceMaps("BrushedMetal", 2, .57f, .04f, .90f);
        var rubberMaps = SurfaceMaps("Rubber", 3, .12f, .035f, 0);
        var enamelMaps = SurfaceMaps("Enamel", 4, .40f, .05f, 0);
        oak = new Material(Load<Material>("Assets/StudioArtPack/ReferenceV4/Materials/FineOak.mat")) { name = "SatinOak" };
        Maps(oak, woodMaps, .20f); AssetDatabase.CreateAsset(oak, Root + "/Materials/SatinOak.mat");
        linen = Lit("WovenOatLinen", "DCCFB7", .10f); Maps(linen, clothMaps, .38f);
        linen.SetTexture("_BaseMap", WovenAlbedo()); linen.mainTextureScale = new Vector2(2, 2);
        steel = Lit("BrushedSteel", "B6BFC2", .57f, .9f); Maps(steel, metalMaps, .035f); steel.mainTextureScale = new Vector2(3, 3);
        brass = Lit("SatinBrass", "D1AA60", .57f, .9f); Maps(brass, metalMaps, .035f); brass.mainTextureScale = new Vector2(3, 3);
        ceramic = Lit("IvoryGlaze", "EDE2CD", .68f);
        rubber = Lit("CharcoalRubber", "293336", .12f); Maps(rubber, rubberMaps, .22f);
        teal = Lit("TealEnamel", "287E80", .4f); Maps(teal, enamelMaps, .12f);
        cream = Lit("CreamEnamel", "E5D9BD", .4f); Maps(cream, enamelMaps, .12f);
        ochre = Lit("OchreEnamel", "C8A34E", .4f); Maps(ochre, enamelMaps, .12f);
        coral = Lit("CoralEnamel", "BA6857", .4f); Maps(coral, enamelMaps, .12f);
        latex = Lit("TealLatex", "2198A2", .64f);
        coffee = Lit("CoffeeSurface", "362319", .76f);
        canvas = Lit("UnprimedCanvas", "E8DFC8", .10f); Maps(canvas, clothMaps, .26f); canvas.mainTextureScale = new Vector2(3, 3);
    }

    // Native, periodic surface maps describe physical microstructure; no external artwork is used.
    static Texture2D[] SurfaceMaps(string name, int kind, float smooth, float variation, float metallic)
    {
        const int n = 512;
        var heights = new float[n * n]; var normal = new Color[n * n]; var packed = new Color[n * n];
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float u = x/(float)n, v = y/(float)n, tau = Mathf.PI*2;
            float grain = Mathf.Sin(tau*(u*18 + .24f*Mathf.Sin(v*tau*2) + .05f*Mathf.Sin(v*tau*7)));
            float small = Mathf.Sin(tau*(u*73 + v*19))*Mathf.Sin(tau*(v*53-u*17));
            float weave = (Mathf.Cos(tau*u*64) + Mathf.Cos(tau*v*64))*.5f;
            float h = kind==0 ? grain*.16f + small*.025f : kind==1 ? weave*.13f + small*.015f : kind==2 ? Mathf.Sin(tau*v*96)*.04f + small*.015f : small*.035f;
            heights[y*n+x] = h;
            packed[y*n+x] = new Color(metallic, 0, 0, Mathf.Clamp01(smooth + variation*(kind==0?grain:small)));
        }
        for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
        {
            float dx = heights[y*n+(x+1)%n]-heights[y*n+(x+n-1)%n];
            float dy = heights[((y+1)%n)*n+x]-heights[((y+n-1)%n)*n+x];
            var direction = new Vector3(-dx*2.5f, -dy*2.5f, 1).normalized;
            normal[y*n+x] = new Color(direction.x*.5f+.5f, direction.y*.5f+.5f, direction.z*.5f+.5f, 1);
        }
        return new[] { Texture(name+"Normal", normal, n, true, false), Texture(name+"Surface", packed, n, false, false) };
    }
    static Texture2D WovenAlbedo()
    {
        const int n=512; var pixels=new Color[n*n];
        for(int y=0;y<n;y++)for(int x=0;x<n;x++)
        {
            float warp = Mathf.Cos(x*Mathf.PI/4), weft = Mathf.Cos(y*Mathf.PI/4);
            float tone = .93f + .035f*(warp+weft) + .012f*Mathf.Sin((x+y)*Mathf.PI*14/n);
            pixels[y*n+x]=new Color(tone,tone,tone,1);
        }
        return Texture("LinenWeave",pixels,n,false,true);
    }
    static Texture2D Texture(string name, Color[] pixels, int n, bool isNormal, bool srgb)
    {
        var data=new Texture2D(n,n,TextureFormat.RGBA32,false,!srgb);data.SetPixels(pixels);data.Apply();
        string path=Root+"/Textures/"+name+".png"; File.WriteAllBytes(path,data.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(data);
        AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType=isNormal?TextureImporterType.NormalMap:TextureImporterType.Default;importer.sRGBTexture=srgb;
        importer.convertToNormalmap=false;importer.wrapMode=TextureWrapMode.Repeat;importer.mipmapEnabled=true;importer.anisoLevel=8;
        importer.filterMode=FilterMode.Trilinear;importer.maxTextureSize=n;importer.textureCompression=TextureImporterCompression.CompressedHQ;
        importer.compressionQuality=90;importer.alphaSource=isNormal?TextureImporterAlphaSource.None:TextureImporterAlphaSource.FromInput;
        importer.SaveAndReimport();return Load<Texture2D>(path);
    }
    static void Maps(Material material, Texture2D[] maps, float bump)
    {
        material.SetTexture("_BumpMap",maps[0]);material.SetFloat("_BumpScale",bump);material.EnableKeyword("_NORMALMAP");
        material.SetTexture("_MetallicGlossMap",maps[1]);material.EnableKeyword("_METALLICSPECGLOSSMAP");
        material.SetFloat("_Smoothness",1);material.SetFloat("_SmoothnessTextureChannel",0);
    }
    static Material Lit(string name,string hex,float smooth,float metallic=0)
    {
        var shader=Shader.Find("Universal Render Pipeline/Lit")??throw new InvalidOperationException("URP Lit missing.");
        var material=new Material(shader){name=name};ColorUtility.TryParseHtmlString("#"+hex,out var color);
        material.SetColor("_BaseColor",color);material.SetFloat("_Smoothness",smooth);material.SetFloat("_Metallic",metallic);
        AssetDatabase.CreateAsset(material,Root+"/Materials/"+name+".mat");return material;
    }

    static void RefineGun()
    {
        var gun=PrefabUtility.LoadPrefabContents(GunPath);
        try
        {
            if(gun.transform.Find("RefinedGunVisual")!=null)throw new InvalidOperationException("Gun already refined.");
            buildingGun=true;
            foreach(var r in gun.GetComponentsInChildren<MeshRenderer>(true))r.enabled=false;
            var root=new GameObject("RefinedGunVisual").transform;root.SetParent(gun.transform,false);
            Box(root,"RoundedReceiver",new Vector3(0,.055f,-.07f),new Vector3(.137f,.151f,.32f),.017f,cream);
            Box(root,"RearCap",new Vector3(0,.052f,-.236f),new Vector3(.128f,.132f,.025f),.012f,rubber);
            Box(root,"UpperRail",new Vector3(0,.139f,-.056f),new Vector3(.075f,.026f,.26f),.008f,brass);
            var grip=Box(root,"RoundedGrip",new Vector3(0,-.075f,-.177f),new Vector3(.075f,.21f,.09f),.016f,rubber);
            grip.localRotation=Quaternion.Euler(-13,0,0);
            for(int i=0;i<5;i++)Box(grip,"GripRib",new Vector3(0,-.064f+i*.028f,-.045f),new Vector3(.066f,.009f,.008f),.003f,rubber);
            Box(root,"GripHeel",new Vector3(0,-.18f,-.151f),new Vector3(.085f,.025f,.103f),.010f,brass);
            var barrel=SaveMesh(Lathe(new[]{V(0,-.11f),V(.037f,-.11f),V(.044f,-.10f),V(.044f,-.065f),V(.033f,-.055f),V(.033f,.082f),V(.04f,.089f),V(.04f,.105f),V(.033f,.111f),V(.025f,.111f),V(.024f,.096f),V(.024f,-.08f),V(0,-.08f)}),"BarrelWithOpenMouth");
            Part(root,"PaintBarrel",barrel,new Vector3(0,.085f,.229f),Vector3.one,steel,Quaternion.Euler(90,0,0));
            Ring(root,"BarrelCollar",new Vector3(0,.085f,.163f),new Vector3(.092f,.014f,.092f),brass,Quaternion.Euler(90,0,0));
            var nozzle=SaveMesh(Lathe(new[]{V(.026f,-.006f),V(.028f,-.003f),V(.028f,.003f),V(.025f,.006f),V(.019f,.006f),V(.018f,.002f),V(.018f,-.006f),V(.026f,-.006f)}),"NozzleLip");
            Part(root,"NozzleLip",nozzle,new Vector3(0,.085f,.338f),Vector3.one,brass,Quaternion.Euler(90,0,0));
            // All geometry ends before the original Muzzle at z=.345.
            Box(root,"Trigger",new Vector3(0,-.034f,-.085f),new Vector3(.017f,.055f,.018f),.006f,brass);
            Box(root,"TriggerGuardBottom",new Vector3(0,-.070f,-.074f),new Vector3(.02f,.014f,.108f),.005f,steel);
            Box(root,"TriggerGuardFront",new Vector3(0,-.038f,-.02f),new Vector3(.02f,.067f,.014f),.005f,steel);
            foreach(float side in new[]{-1f,1f})
            {
                Box(root,"InsetSidePanel",new Vector3(side*.069f,.053f,-.078f),new Vector3(.006f,.087f,.18f),.002f,teal);
                Box(root,"MakerPlate",new Vector3(side*.073f,.065f,-.118f),new Vector3(.004f,.027f,.06f),.0015f,brass);
                for(int i=0;i<3;i++)Box(root,"PanelVent",new Vector3(side*.074f,.035f,.014f-i*.022f),new Vector3(.003f,.031f,.006f),.001f,rubber);
                foreach(float z in new[]{-.184f,.037f})
                {
                    Ring(root,"FlushFastener",new Vector3(side*.073f,.096f,z),new Vector3(.024f,.005f,.024f),steel,Quaternion.Euler(0,0,90));
                    Box(root,"FastenerSlot",new Vector3(side*.0755f,.096f,z),new Vector3(.001f,.0025f,.010f),.0004f,rubber);
                }
            }
            var tank=SaveMesh(Lathe(new[]{V(0,-.091f),V(.041f,-.091f),V(.052f,-.086f),V(.060f,-.067f),V(.061f,.06f),V(.055f,.081f),V(.045f,.089f),V(0,.089f)}),"PaintReservoirShell");
            Part(root,"TealReservoir",tank,new Vector3(0,.22f,-.045f),Vector3.one,teal);
            Ring(root,"ReservoirFoot",new Vector3(0,.143f,-.045f),new Vector3(.117f,.017f,.117f),steel);
            var lid=SaveMesh(Lathe(new[]{V(0,-.007f),V(.052f,-.007f),V(.058f,-.003f),V(.058f,.008f),V(.054f,.013f),V(0,.013f)}),"ReservoirLid");
            Part(root,"BrassReservoirLid",lid,new Vector3(0,.309f,-.045f),Vector3.one,brass);
            for(int i=0;i<12;i++)
            {
                float a=i*Mathf.PI/6;
                Box(root,"LidKnurl",new Vector3(Mathf.Cos(a)*.057f,.313f,-.045f+Mathf.Sin(a)*.057f),new Vector3(.004f,.010f,.003f),.001f,brass,Quaternion.Euler(0,-i*30,0));
            }
            Box(root,"ReservoirLabel",new Vector3(0,.221f,-.107f),new Vector3(.046f,.046f,.003f),.001f,cream);
            for(int i=0;i<3;i++)Box(root,"LabelPaintMark",new Vector3(-.013f+i*.013f,.221f,-.109f),new Vector3(.006f,.026f-i*.005f,.0015f),.0006f,i==1?ochre:coral);
            CombineGun(root);
            if(gun.GetComponentsInChildren<Collider>(true).Length!=0)throw new InvalidOperationException("Gun collider changed.");
            if(gun.transform.Find("Muzzle").localPosition!=new Vector3(0,.085f,.345f))throw new InvalidOperationException("Muzzle moved.");
            PrefabUtility.SaveAsPrefabAsset(gun,GunPath);
        }
        finally
        {
            buildingGun=false;
            PrefabUtility.UnloadPrefabContents(gun);
            foreach(var mesh in temporaryGunMeshes)UnityEngine.Object.DestroyImmediate(mesh);
            temporaryGunMeshes.Clear();boxes.Clear();
        }
    }
    static void CombineGun(Transform root)
    {
        buildingGun=false;
        // Bake static shell pieces by material to avoid dozens of extra draw calls on the held gun.
        var renderers=root.GetComponentsInChildren<MeshRenderer>();
        foreach(var material in renderers.Select(r=>r.sharedMaterial).Distinct())
        {
            var pieces=renderers.Where(r=>r.sharedMaterial==material).Select(r=>new CombineInstance{mesh=r.GetComponent<MeshFilter>().sharedMesh,transform=root.worldToLocalMatrix*r.transform.localToWorldMatrix}).ToArray();
            var mesh=new Mesh();mesh.CombineMeshes(pieces,true,true);mesh=SaveMesh(mesh,"Gun_"+material.name);
            Part(root,material.name,mesh,Vector3.zero,Vector3.one,material);
        }
        foreach(var child in renderers.Select(r=>r.transform).Where(t=>t.parent==root).ToArray())UnityEngine.Object.DestroyImmediate(child.gameObject);
    }

    static void RefineBalloon()
    {
        var balloon=PrefabUtility.LoadPrefabContents(BalloonPath);
        try
        {
            var filter=balloon.GetComponentsInChildren<MeshFilter>(true).Single();
            var source=filter.sharedMesh;
            var body=BalloonMesh(source);filter.sharedMesh=SaveMesh(body,"SmoothBalloonWithOriginalTie");
            filter.GetComponent<MeshRenderer>().sharedMaterial=latex;
            RefineString(balloon,filter);
            PrefabUtility.RecordPrefabInstancePropertyModifications(filter);PrefabUtility.RecordPrefabInstancePropertyModifications(filter.GetComponent<MeshRenderer>());
            PrefabUtility.SaveAsPrefabAsset(balloon,BalloonPath);
        }
        finally{PrefabUtility.UnloadPrefabContents(balloon);}
    }
    static Mesh BalloonMesh(Mesh source)
    {
        var verts=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
        const int sides=96,rings=48;
        for(int y=0;y<=rings;y++)for(int x=0;x<=sides;x++)
        {
            float t=y*Mathf.PI/rings,a=x*Mathf.PI*2/sides;
            float r=.1324f*Mathf.Sin(t)*(1+.08f*Mathf.Cos(t));
            float h=.006f+.1755f*Mathf.Cos(t);
            float dr=.1324f*(Mathf.Cos(t)*(1+.08f*Mathf.Cos(t))-.08f*Mathf.Sin(t)*Mathf.Sin(t));
            float dh=-.1755f*Mathf.Sin(t);
            verts.Add(new Vector3(r*Mathf.Cos(a),h,r*Mathf.Sin(a)));
            normals.Add(new Vector3(-dh*Mathf.Cos(a),dr,-dh*Mathf.Sin(a)).normalized);uv.Add(new Vector2(x/(float)sides,y/(float)rings));
            if(y<rings&&x<sides){int k=y*(sides+1)+x;triangles.AddRange(new[]{k,k+1,k+sides+1,k+1,k+sides+2,k+sides+1});}
        }
        // Retain the original knot, replacing its trailing wire with the dedicated rope renderer.
        var sourceV=source.vertices;var sourceN=source.normals;var sourceUv=source.uv;var remap=new Dictionary<int,int>();
        var original=source.triangles;
        for(int i=0;i<original.Length;i+=3)
        {
            float high=Mathf.Max(sourceV[original[i]].y,Mathf.Max(sourceV[original[i+1]].y,sourceV[original[i+2]].y));
            float low=Mathf.Min(sourceV[original[i]].y,Mathf.Min(sourceV[original[i+1]].y,sourceV[original[i+2]].y));
            if(high>-.160f||low<-.196f)continue;
            for(int j=0;j<3;j++)
            {
                int id=original[i+j];if(!remap.TryGetValue(id,out int index))
                {index=verts.Count;remap[id]=index;verts.Add(sourceV[id]);normals.Add(sourceN[id]);uv.Add(sourceUv.Length==sourceV.Length?sourceUv[id]:Vector2.zero);}
                triangles.Add(index);
            }
        }
        var mesh=new Mesh();mesh.SetVertices(verts);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);return mesh;
    }

    static void RefineString(GameObject balloon,MeshFilter body)
    {
        var line=balloon.GetComponentsInChildren<LineRenderer>(true).Single();
        const int count=25;var points=new Vector3[count];
        for(int i=0;i<count;i++)
        {
            float t=i/(float)(count-1);
            var local=new Vector3(.008f+Mathf.Sin(t*Mathf.PI*2)*.025f*t,-.191f-t*.30f,.009f+Mathf.Sin(t*Mathf.PI)*.008f);
            points[i]=line.transform.InverseTransformPoint(body.transform.TransformPoint(local));
        }
        line.useWorldSpace=false;line.positionCount=count;line.SetPositions(points);
        line.widthMultiplier=.009f;line.widthCurve=AnimationCurve.Constant(0,1,1);line.numCornerVertices=4;line.numCapVertices=4;
        line.shadowCastingMode=ShadowCastingMode.Off;
        PrefabUtility.RecordPrefabInstancePropertyModifications(line);
    }

    static void RefineProps(IEnumerable<MeshRenderer> input)
    {
        var bevelNames=new HashSet<string>{"Tabletop","LowerShelf","Leg","OakShelf","UpperShelf","Tray","CanvasRail","WorkbenchTop","TopShelf","BottomShelf"};
        foreach(var r in input.ToArray())
        {
            if(r.sharedMaterial==null)continue;
            var materials=r.sharedMaterials;
            for(int i=0;i<materials.Length;i++)
            {
                if(materials[i]==null)continue;
                switch(materials[i].name)
                {
                    case "FineOak":materials[i]=oak;break;
                    case "OatLinen":materials[i]=linen;break;
                    case "IvoryCeramic":materials[i]=ceramic;break;
                    case "BrushedMetal":materials[i]=steel;break;
                    case "TealPaint":materials[i]=teal;break;
                    case "GoldPaint":materials[i]=ochre;break;
                    case "CoralPaint":materials[i]=coral;break;
                }
            }
            r.sharedMaterials=materials;
            if(r.name=="Coffee")r.sharedMaterial=coffee;
            if(r.name.Contains("Wheel")||r.name=="Caster"||r.name=="FootCap")r.sharedMaterial=rubber;
            if(r.name=="FoldedCloth"||r.name=="ClothFold")r.sharedMaterial=linen;
            if(r.name=="BlankCanvas"||r.name=="CanvasFace")r.sharedMaterial=canvas;
            var f=r.GetComponent<MeshFilter>();
            if(f==null)continue;
            if(f.sharedMesh.name=="HollowCup")f.sharedMesh=cup;
            if(f.sharedMesh.name=="SmallRing")f.sharedMesh=ring;
            if(f.sharedMesh.name=="PaperTube")f.sharedMesh=tube;
            if(bevelNames.Contains(r.name)&&r.sharedMaterial==oak)
            {
                var b=f.sharedMesh.bounds;
                f.sharedMesh=BoxMesh(b.size,Mathf.Min(b.size.x,Mathf.Min(b.size.y,b.size.z))*.14f,b.center);
            }
            if(r.name=="RoundOakSeat")
            {
                // Built-in cylinders have unit radius .5 and full height 2.
                f.sharedMesh=SaveMesh(Lathe(new[]{V(0,-1),V(.44f,-1),V(.48f,-.92f),V(.5f,-.7f),V(.5f,.7f),V(.48f,.92f),V(.44f,1),V(0,1)}),"StoolSeat"+(meshNumber++));
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(r);PrefabUtility.RecordPrefabInstancePropertyModifications(f);
        }
    }

    static Transform Box(Transform parent,string name,Vector3 p,Vector3 size,float radius,Material material,Quaternion? rotation=null)
        => Part(parent,name,BoxMesh(size,radius,Vector3.zero),p,Vector3.one,material,rotation);
    static Mesh BoxMesh(Vector3 size,float radius,Vector3 center)
    {
        string key=size.ToString("F5")+radius.ToString("F5")+center.ToString("F5");
        if(boxes.TryGetValue(key,out var cached))return cached;
        var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var indices=new List<int>();
        var half=size*.5f;radius=Mathf.Min(radius,Mathf.Min(half.x,Mathf.Min(half.y,half.z))*.7f);var inner=half-Vector3.one*radius;
        foreach(var normal in new[]{Vector3.right,Vector3.left,Vector3.up,Vector3.down,Vector3.forward,Vector3.back})
        {
            var u=Mathf.Abs(normal.y)>.5f?Vector3.right:Vector3.up;var v=Vector3.Cross(normal,u);
            float hu=Mathf.Abs(Vector3.Dot(half,u)),hv=Mathf.Abs(Vector3.Dot(half,v));
            var xs=BevelSteps(hu,radius);var ys=BevelSteps(hv,radius);int start=vertices.Count;
            for(int y=0;y<ys.Length;y++)for(int x=0;x<xs.Length;x++)
            {
                var q=Vector3.Scale(normal,half)+u*xs[x]+v*ys[y];
                var closest=new Vector3(Mathf.Clamp(q.x,-inner.x,inner.x),Mathf.Clamp(q.y,-inner.y,inner.y),Mathf.Clamp(q.z,-inner.z,inner.z));
                var n=(q-closest).normalized;vertices.Add(center+closest+n*radius);normals.Add(n);
                uv.Add(new Vector2(xs[x]+hu,ys[y]+hv));
            }
            for(int y=0;y<ys.Length-1;y++)for(int x=0;x<xs.Length-1;x++)
            {int a=start+y*xs.Length+x;indices.AddRange(new[]{a,a+1,a+xs.Length,a+1,a+xs.Length+1,a+xs.Length});}
        }
        var mesh=new Mesh();mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);
        mesh=SaveMesh(mesh,"BeveledBox"+(meshNumber++).ToString("D3"));boxes[key]=mesh;return mesh;
    }
    static float[] BevelSteps(float half,float radius)
    {
        float core=half-radius;
        return new[]{-half,-core-radius*.92388f,-core-radius*.707107f,-core-radius*.382683f,-core,0,core,core+radius*.382683f,core+radius*.707107f,core+radius*.92388f,half};
    }
    static Mesh Lathe(Vector2[] profile)
    {
        const int sides=64;var verts=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var indices=new List<int>();
        for(int i=0;i<=sides;i++)for(int j=0;j<profile.Length;j++)
        {
            float angle=i*Mathf.PI*2/sides;var p=profile[j];
            var d=profile[Mathf.Min(j+1,profile.Length-1)]-profile[Mathf.Max(j-1,0)];
            verts.Add(new Vector3(p.x*Mathf.Cos(angle),p.y,p.x*Mathf.Sin(angle)));
            normals.Add(new Vector3(d.y*Mathf.Cos(angle),-d.x,d.y*Mathf.Sin(angle)).normalized);
            uv.Add(new Vector2(i/(float)sides,j/(float)(profile.Length-1)));
            if(i<sides&&j<profile.Length-1){int a=i*profile.Length+j;indices.AddRange(new[]{a,a+1,a+profile.Length,a+1,a+profile.Length+1,a+profile.Length});}
        }
        var mesh=new Mesh();mesh.SetVertices(verts);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);return mesh;
    }
    static Mesh Torus(float radius,float thickness,int sides,int tubeSides)
    {
        var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var indices=new List<int>();
        for(int i=0;i<=sides;i++)for(int j=0;j<=tubeSides;j++)
        {
            float u=i*Mathf.PI*2/sides,v=j*Mathf.PI*2/tubeSides;
            var normal=new Vector3(Mathf.Cos(v)*Mathf.Cos(u),Mathf.Sin(v),Mathf.Cos(v)*Mathf.Sin(u));
            vertices.Add(new Vector3(radius*Mathf.Cos(u),0,radius*Mathf.Sin(u))+normal*thickness);normals.Add(normal);uv.Add(new Vector2(i/(float)sides,j/(float)tubeSides));
            if(i<sides&&j<tubeSides){int a=i*(tubeSides+1)+j;indices.AddRange(new[]{a,a+1,a+tubeSides+1,a+1,a+tubeSides+2,a+tubeSides+1});}
        }
        var mesh=new Mesh();mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(indices,0);return mesh;
    }
    static Transform Part(Transform parent,string name,Mesh mesh,Vector3 position,Vector3 scale,Material material,Quaternion? rotation=null)
    {
        var result=new GameObject(name).transform;result.SetParent(parent,false);result.localPosition=position;result.localScale=scale;result.localRotation=rotation??Quaternion.identity;
        result.gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;result.gameObject.AddComponent<MeshRenderer>().sharedMaterial=material;return result;
    }
    static void Ring(Transform parent,string name,Vector3 position,Vector3 size,Material material,Quaternion? rotation=null)
    {
        // Stored torus is a y-axis ring with outside diameter .97 and tube thickness .11.
        Part(parent,name,ring,position,new Vector3(size.x/.97f,size.y/.11f,size.z/.97f),material,rotation);
    }
    static Mesh SaveMesh(Mesh mesh,string name)
    {
        mesh.name=name;mesh.RecalculateBounds();mesh.RecalculateTangents();
        if(mesh.vertexCount==0||mesh.vertices.Any(v=>!Finite(v.x)||!Finite(v.y)||!Finite(v.z))||mesh.normals.Any(v=>!Finite(v.x)||!Finite(v.y)||!Finite(v.z))||mesh.tangents.Any(v=>!Finite(v.x)||!Finite(v.y)||!Finite(v.z)||!Finite(v.w)))
            throw new InvalidOperationException("Invalid mesh: "+name);
        if(buildingGun){temporaryGunMeshes.Add(mesh);return mesh;}
        AssetDatabase.CreateAsset(mesh,Root+"/Meshes/"+name+".asset");return mesh;
    }
    static bool Finite(float v)=>!float.IsNaN(v)&&!float.IsInfinity(v);
    static Vector2 V(float radius,float height)=>new Vector2(radius,height);
    static T Load<T>(string path)where T:UnityEngine.Object=>AssetDatabase.LoadAssetAtPath<T>(path)??throw new IOException("Missing asset: "+path);
    static T[] All<T>(Scene scene)where T:Component=>scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();

    public static void Validate(Scene scene)
    {
        var gun=All<PlayerShooter>(scene).Single();var visual=gun.transform.Find("RefinedGunVisual");
        if(visual==null||visual.GetComponentsInChildren<MeshRenderer>().Length>8)throw new InvalidOperationException("Refined gun missing or unbatched.");
        if(gun.GetComponentsInChildren<Collider>(true).Length!=0)throw new InvalidOperationException("Gun intercepts aiming.");
        foreach(var r in All<MeshRenderer>(scene).Where(r=>r.enabled))
        {
            if(r.sharedMaterials.Any(m=>m==null))throw new InvalidOperationException("Missing material: "+r.name);
            var mesh=r.GetComponent<MeshFilter>()?.sharedMesh;if(mesh==null)throw new InvalidOperationException("Missing mesh: "+r.name);
            if(mesh.vertices.Any(v=>!Finite(v.x)||!Finite(v.y)||!Finite(v.z)))throw new InvalidOperationException("Non-finite vertices: "+r.name);
        }
        var balloon=Load<GameObject>(BalloonPath);
        if(balloon.GetComponentsInChildren<MeshFilter>(true).Single().sharedMesh.vertexCount<4000)throw new InvalidOperationException("Smooth balloon mesh missing.");
        if(balloon.GetComponentsInChildren<CapsuleCollider>(true).Count(c=>c.enabled)!=1)throw new InvalidOperationException("Playable balloon collider changed.");
        var probe=All<ReflectionProbe>(scene).Single(p=>p.name=="BakedStudioReflection");
        if(probe.mode!=ReflectionProbeMode.Custom||probe.customBakedTexture==null)throw new InvalidOperationException("Reflection missing.");
        Debug.Log("SURFACE_VALIDATION_OK: gun uses "+visual.GetComponentsInChildren<MeshRenderer>().Length+" material batches; balloon geometry and collision intact.");
    }
    static void CaptureCloseups(Scene scene)
    {
        var camera=All<Camera>(scene).Single(c=>c.CompareTag("MainCamera"));
        var gun=All<PlayerShooter>(scene).Single().transform;var previousPosition=camera.transform.position;var previousRotation=camera.transform.rotation;float fov=camera.fieldOfView;
        // Held gun is a camera child; detach temporarily so camera movement does not move the subject.
        var parent=gun.parent;var gunPosition=gun.position;var gunRotation=gun.rotation;
        try
        {
            gun.SetParent(null,true);gun.position=new Vector3(0,-.6f,4.8f);gun.rotation=Quaternion.identity;
            var center=gun.TransformPoint(new Vector3(0,.055f,0));
            camera.transform.position=center+new Vector3(-.46f,.24f,-.65f);camera.transform.LookAt(center);camera.fieldOfView=35;
            ExhibitionStudioBuilder.Capture(scene,"surfaces-gun-closeup.png");
            camera.transform.position=new Vector3(-3.1f,-1.25f,2.5f);camera.transform.LookAt(new Vector3(-5.3f,-2.05f,6));camera.fieldOfView=33;
            gun.gameObject.SetActive(false);ExhibitionStudioBuilder.Capture(scene,"surfaces-workbench-closeup.png");
        }
        finally
        {
            gun.gameObject.SetActive(true);camera.transform.position=previousPosition;camera.transform.rotation=previousRotation;camera.fieldOfView=fov;
            gun.SetParent(parent,true);gun.position=gunPosition;gun.rotation=gunRotation;
        }
    }
}
