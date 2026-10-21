using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    /// <summary>Author native cosmetic meshes, preserving the existing recoil root and firing helpers.</summary>
    public static class ConceptGunRefinement
    {
        public const string Root = "Assets/StudioArtPack/PaintBlaster/ConceptRefinement20261019";
        const string Prefab = "Assets/StudioArtPack/PaintBlaster/Assembled/PaintGun_M.prefab";
        const string Scene = "Assets/Scenes/PrototypeScene.unity";
        static readonly List<Mesh> Temporary = new List<Mesh>();
        static Vector2 P(float r, float z) => new Vector2(r, z);
        static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }

        public static void Run()
        {
            Require(Application.isBatchMode && Application.dataPath.Contains(".local-reference"), "Use an isolated batch project under .local-reference.");
            try
            {
                Require(!Directory.Exists(Root), "Output already exists; preserve manual asset edits.");
                Directory.CreateDirectory(Root + "/Meshes"); Directory.CreateDirectory(Root + "/Materials"); Directory.CreateDirectory(Root + "/Previews");
                AssetDatabase.Refresh();
                EditorSceneManager.OpenScene(Scene);
                CaptureScene("Before_FirstPerson"); CaptureInspection("Before_Model");
                Build();
                AssetDatabase.SaveAssets();
                EditorSceneManager.OpenScene(Scene);
                Verify();
                // Give the renderer a few editor frames to upload freshly imported meshes/materials.
                int frames = 0;
                EditorApplication.CallbackFunction render = null;
                render = () =>
                {
                    if (++frames < 24) return;
                    EditorApplication.update -= render;
                    try
                    {
                        CaptureScene("After_FirstPerson"); CaptureInspection("After_Model");
                        AssetDatabase.Refresh();
                        Debug.Log("CONCEPT_GUN_REFINEMENT_OK"); EditorApplication.Exit(0);
                    }
                    catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
                };
                EditorApplication.update += render;
            }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
        }

        static Material Material(string name, string hex, float smooth, float metal = 0)
        {
            var shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/StudioArtPack/ConceptVariantsV12/Shaders/StudioGunLit.shader");
            Require(shader != null && !ShaderUtil.ShaderHasError(shader), "Existing licensed held-gun shader is unavailable.");
            var material = new Material(shader) { name = name };
            ColorUtility.TryParseHtmlString("#" + hex, out var color);
            material.SetColor("_BaseColor", color); material.SetColor("_Color", color);
            material.SetFloat("_Smoothness", smooth); material.SetFloat("_Metallic", metal);
            AssetDatabase.CreateAsset(material, Root + "/Materials/" + name + ".mat");
            return material;
        }

        static void Build()
        {
            var gun = PrefabUtility.LoadPrefabContents(Prefab);
            try
            {
                var visual = gun.transform.Find("RefinedGunVisual");
                Require(visual != null && visual.localPosition == Vector3.zero && visual.localRotation == Quaternion.identity, "Unexpected recoil root; preserve custom pose.");
                string muzzle = EditorJsonUtility.ToJson(gun.transform.Find("Muzzle"));
                foreach (Transform child in visual) child.gameObject.SetActive(false);
                var root = new GameObject("WebsiteConceptShell_20261019").transform; root.SetParent(visual, false);
                var cream = Material("WarmCream", "F5E8CF", .42f);
                var teal = Material("PigmentTeal", "159A9E", .49f);
                var coral = Material("CoralNozzle", "F27D68", .40f);
                var gold = Material("HoneyGold", "EAB44D", .48f, .20f);
                var dark = Material("NozzleInterior", "763F37", .29f);
                var glass = new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/StudioArtPack/ConceptVariantsV12/Materials/ReservoirGlass.mat")) { name = "ClearReservoir" };
                glass.SetColor("_BaseColor", new Color(.88f, .97f, 1, .13f));
                glass.SetFloat("_Smoothness", .83f); glass.SetFloat("_Cull", 2);
                AssetDatabase.CreateAsset(glass, Root + "/Materials/ClearReservoir.mat");
                Transform Part(string name, Mesh mesh, Vector3 position, Material material, Quaternion? rotation = null, Vector3? scale = null)
                {
                    var go = new GameObject(name); go.transform.SetParent(root, false); go.transform.localPosition = position;
                    go.transform.localRotation = rotation ?? Quaternion.identity; go.transform.localScale = scale ?? Vector3.one;
                    go.AddComponent<MeshFilter>().sharedMesh = mesh; go.AddComponent<MeshRenderer>().sharedMaterial = material;
                    return go.transform;
                }
                Mesh Box(Vector3 size, float bevel) { var mesh = StudioConceptGeometry.RoundedBox(size, bevel); Temporary.Add(mesh); return mesh; }
                Mesh Lathe(params Vector2[] profile) { var mesh = StudioConceptGeometry.Lathe(profile); Temporary.Add(mesh); return mesh; }
                void B(string name, Vector3 pos, Vector3 size, float bevel, Material mat, Quaternion? rot = null) => Part(name, Box(size, bevel), pos, mat, rot);
                var axial = Quaternion.Euler(90, 0, 0);
                B("SoftReceiver", new Vector3(0, .065f, -.065f), new Vector3(.177f, .158f, .315f), .035f, cream);
                B("ReservoirSaddle", new Vector3(0, .155f, -.072f), new Vector3(.132f, .032f, .25f), .015f, cream);
                B("ErgonomicGrip", new Vector3(0, -.083f, -.157f), new Vector3(.091f, .216f, .102f), .027f, teal, Quaternion.Euler(-16, 0, 0));
                B("CreamGripHeel", new Vector3(0, -.181f, -.130f), new Vector3(.109f, .034f, .126f), .016f, cream);
                B("GuardBase", new Vector3(0, -.094f, -.054f), new Vector3(.034f, .025f, .120f), .011f, cream);
                B("GuardFront", new Vector3(0, -.054f, -.004f), new Vector3(.034f, .102f, .031f), .014f, cream, Quaternion.Euler(10, 0, 0));
                B("CoralTrigger", new Vector3(0, -.040f, -.067f), new Vector3(.024f, .060f, .032f), .012f, coral, Quaternion.Euler(-17, 0, 0));
                // Bell profile folds into a genuinely recessed open bore; frontmost z=.341 < logical muzzle z=.345.
                Part("CreamBarrelNeck", Lathe(P(0,-.037f),P(.040f,-.037f),P(.047f,-.025f),P(.047f,.032f),P(.040f,.037f),P(0,.037f)), new Vector3(0,.085f,.152f), cream, axial);
                Part("CoralBellNozzle", Lathe(P(.048f,-.073f),P(.062f,-.069f),P(.077f,-.046f),P(.092f,.039f),P(.094f,.057f),P(.089f,.072f),P(.082f,.077f),P(.066f,.077f),P(.060f,.070f),P(.054f,.045f),P(.039f,-.061f),P(.048f,-.073f)), new Vector3(0,.085f,.264f), coral, axial);
                Part("DarkRecessedBore", Lathe(P(.037f,-.045f),P(.053f,.038f),P(.054f,.048f),P(.048f,.050f),P(.024f,-.045f),P(.037f,-.045f)), new Vector3(0,.085f,.264f), dark, axial);
                Part("GoldNozzleCollar", Lathe(P(.046f,-.012f),P(.061f,-.008f),P(.062f,.008f),P(.055f,.014f),P(.042f,.014f),P(.042f,-.012f),P(.046f,-.012f)), new Vector3(0,.085f,.190f), gold, axial);
                // Raised narrow nozzle pads, kept within the original muzzle end plane.
                for (int i=0; i<8; i++)
                {
                    float a=i*Mathf.PI/4;
                    B("NozzleMoldedPad", new Vector3(Mathf.Cos(a)*.091f,.085f+Mathf.Sin(a)*.091f,.310f),new Vector3(.014f,.007f,.021f),.003f,coral,Quaternion.Euler(0,0,i*45-90));
                }
                var tankProfile = new[] {P(0,-.160f),P(.045f,-.155f),P(.067f,-.140f),P(.079f,-.116f),P(.079f,.106f),P(.070f,.132f),P(.050f,.146f),P(0,.151f)};
                Part("TransparentHorizontalTank", Lathe(tankProfile), new Vector3(0,.246f,-.069f), glass, axial);
                var liquid = Liquid(); Temporary.Add(liquid);
                Part("VisibleTealFill",liquid,new Vector3(0,.246f,-.069f),teal);
                Mesh band = Lathe(P(.075f,-.011f),P(.084f,-.007f),P(.084f,.007f),P(.078f,.012f),P(.074f,.012f),P(.074f,-.011f),P(.075f,-.011f));
                Part("TankGoldStrap",band,new Vector3(0,.246f,-.136f),gold,axial);
                Part("RearTankCollar",band,new Vector3(0,.246f,-.205f),gold,axial);
                Mesh lid=Lathe(P(0,-.010f),P(.047f,-.010f),P(.056f,-.003f),P(.056f,.016f),P(.049f,.024f),P(0,.024f));
                Part("GoldKnurledCap",lid,new Vector3(0,.326f,-.139f),gold);
                for(int i=0;i<16;i++)
                {
                    float a=i*Mathf.PI/8;
                    B("CapFlute",new Vector3(Mathf.Cos(a)*.053f,.333f,-.139f+Mathf.Sin(a)*.053f),new Vector3(.007f,.019f,.007f),.003f,gold,Quaternion.Euler(0,-i*22.5f,0));
                }
                var hose=Hose(); Temporary.Add(hose); Part("RearGoldReturnHose",hose,Vector3.zero,gold);
                var button=Lathe(P(0,-.004f),P(.029f,-.004f),P(.036f,.001f),P(.036f,.012f),P(.030f,.018f),P(0,.018f));
                var primitive=GameObject.CreatePrimitive(PrimitiveType.Sphere); var sphere=primitive.GetComponent<MeshFilter>().sharedMesh; Object.DestroyImmediate(primitive);
                foreach(int side in new[]{-1,1})
                {
                    Part("GoldRoundDial",button,new Vector3(side*.087f,.068f,-.060f),gold,Quaternion.Euler(0,0,-side*90));
                    B("DialInset",new Vector3(side*.107f,.069f,-.060f),new Vector3(.003f,.007f,.039f),.001f,gold,Quaternion.Euler(30,0,0));
                    // A few small modeled pigment marks replace military vents/fasteners.
                    for(int i=0;i<8;i++)
                    {
                        float y=.045f+(i%3)*.018f,z=i<4?-.151f+i*.007f:.016f+(i-4)*.008f;
                        Part("PaintFleck",sphere,new Vector3(side*.089f,y,z),i<4?teal:coral,null,new Vector3(.003f,i%3==0?.023f:.009f,.008f+(i%2)*.007f));
                    }
                    for(int i=0;i<3;i++) B("SoftGripRib",new Vector3(side*.044f,-.056f-i*.035f,-.169f+i*.010f),new Vector3(.006f,.010f,.071f),.003f,teal,Quaternion.Euler(-16,0,0));
                }
                // Static material batches; no new scripts, colliders, audio or particles.
                foreach(var mat in new[]{cream,teal,coral,gold,dark,glass})
                {
                    var renderers=root.GetComponentsInChildren<MeshRenderer>().Where(r=>r.sharedMaterial==mat).ToArray();
                    var combine=renderers.Select(r=>new CombineInstance{mesh=r.GetComponent<MeshFilter>().sharedMesh,transform=root.worldToLocalMatrix*r.transform.localToWorldMatrix}).ToArray();
                    var mesh=new Mesh{name="ConceptGun_"+mat.name}; mesh.indexFormat=IndexFormat.UInt32; mesh.CombineMeshes(combine,true,true); mesh.RecalculateBounds();
                    AssetDatabase.CreateAsset(mesh,Root+"/Meshes/"+mesh.name+".asset");
                    foreach(var renderer in renderers) Object.DestroyImmediate(renderer.gameObject);
                    var part=Part(mat.name,mesh,Vector3.zero,mat); var final=part.GetComponent<MeshRenderer>();
                    final.shadowCastingMode=ShadowCastingMode.Off;
                    if(mat==glass) final.receiveShadows=false;
                }
                Require(EditorJsonUtility.ToJson(gun.transform.Find("Muzzle"))==muzzle,"Logical muzzle changed.");
                Require(root.GetComponentsInChildren<Collider>(true).Length==0,"Cosmetic shell must have no collision.");
                PrefabUtility.SaveAsPrefabAsset(gun,Prefab);
            }
            finally { PrefabUtility.UnloadPrefabContents(gun); foreach(var mesh in Temporary) Object.DestroyImmediate(mesh); Temporary.Clear(); }
        }

        static Mesh Liquid()
        {
            var profile=new[]{P(0,-.150f),P(.040f,-.146f),P(.060f,-.131f),P(.072f,-.109f),P(.072f,.100f),P(.061f,.122f),P(.043f,.137f),P(0,.140f)};
            var vertices=new List<Vector3>(); var indices=new List<int>(); const int sides=64;
            for(int z=0;z<profile.Length;z++) for(int i=0;i<=sides;i++)
            {
                float a=i*Mathf.PI*2/sides;
                vertices.Add(new Vector3(profile[z].x*Mathf.Cos(a),Mathf.Min(.014f,profile[z].x*Mathf.Sin(a)),profile[z].y));
                if(z<profile.Length-1&&i<sides) {int j=z*(sides+1)+i; indices.AddRange(new[]{j,j+1,j+sides+1,j+1,j+sides+2,j+sides+1});}
            }
            var mesh=new Mesh{name="ClippedLiquidFill"};mesh.SetVertices(vertices);mesh.SetTriangles(indices,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }

        static Mesh Hose()
        {
            var vertices=new List<Vector3>();var indices=new List<int>();const int rings=28,sides=12;
            for(int i=0;i<=rings;i++)
            {
                float a=-Mathf.PI/2+i*Mathf.PI/rings;
                var center=new Vector3(0,.163f+Mathf.Sin(a)*.081f,-.219f-Mathf.Cos(a)*.085f);
                var radial=new Vector3(0,Mathf.Sin(a),-Mathf.Cos(a));
                for(int j=0;j<=sides;j++)
                {
                    float b=j*Mathf.PI*2/sides;vertices.Add(center+(Vector3.right*Mathf.Cos(b)+radial*Mathf.Sin(b))*.017f);
                    if(i<rings&&j<sides){int k=i*(sides+1)+j;indices.AddRange(new[]{k,k+1,k+sides+1,k+1,k+sides+2,k+sides+1});}
                }
            }
            var mesh=new Mesh{name="CurvedReturnHose"};mesh.SetVertices(vertices);mesh.SetTriangles(indices,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
        }

        static PlayerShooter Shooter() => Object.FindObjectsByType<PlayerShooter>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single();
        static void Verify()
        {
            var shooter=Shooter();var visual=shooter.transform.Find("RefinedGunVisual");var shell=visual.Find("WebsiteConceptShell_20261019");
            var serialized=new SerializedObject(shooter);var muzzle=(Transform)serialized.FindProperty("muzzle").objectReferenceValue;
            Require(serialized.FindProperty("gunVisual").objectReferenceValue==visual,"Saved recoil binding lost.");
            Require(!muzzle.IsChildOf(visual)&&muzzle.localPosition==new Vector3(0,.085f,.345f),"Muzzle helper moved into cosmetic hierarchy.");
            Require(shell.GetComponentsInChildren<MeshRenderer>().Length==6,"Expected six material batches.");
            Require(shell.GetComponentsInChildren<Component>().All(c=>c is Transform||c is MeshFilter||c is MeshRenderer),"Unexpected nonvisual shell component.");
            int vertices=0,triangles=0;
            foreach(var filter in shell.GetComponentsInChildren<MeshFilter>())
            {
                var mesh=filter.sharedMesh;Require(mesh!=null&&mesh.bounds.max.z<.345f,"Mesh exceeds logical muzzle.");
                Require(!ShaderUtil.ShaderHasError(filter.GetComponent<Renderer>().sharedMaterial.shader),"Shader compile failure.");
                vertices+=mesh.vertexCount;triangles+=mesh.triangles.Length/3;
            }
            var camera=Camera.main;var pose=shooter.transform.localRotation;int paths=0;float top=0;
            foreach(float aspect in new[]{16f/9,16f/10,4f/3})
            {
                camera.aspect=aspect;
                foreach(var target in new[]{new Vector3(-3,-1.6f,8),new Vector3(0,0,8),new Vector3(3,1.6f,8)})
                {
                    shooter.transform.LookAt(target);var screen=camera.WorldToViewportPoint(muzzle.position);
                    Require(screen.z>camera.nearClipPlane,"Muzzle behind near plane.");
                    foreach(var filter in shell.GetComponentsInChildren<MeshFilter>()) foreach(var vertex in filter.sharedMesh.vertices)
                    {
                        var p=camera.WorldToViewportPoint(filter.transform.TransformPoint(vertex));
                        Require(p.z>camera.nearClipPlane,"Mesh crosses camera near plane.");top=Mathf.Max(top,p.y);
                    }
                    paths++;
                }
            }
            shooter.transform.localRotation=pose;camera.aspect=16f/9;
            File.WriteAllText(Root+"/Verification.txt",$"Unity {Application.unityVersion}; isolated edit-mode verification.\n6 mesh/material batches; {vertices} vertices; {triangles} triangles.\nSaved recoil root/binding and muzzle pose preserved. Cosmetic hierarchy has only Transform/MeshFilter/MeshRenderer.\n{paths} aim/aspect combinations checked for camera near-plane clearance; maximum top viewport {top:F4}.\nNative camera renders are model/scene inspections, not physical input, full-song playtests, performance benchmarks or a new Windows release.\n");
        }

        static void CaptureScene(string name)
        {
            foreach(var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include,FindObjectsSortMode.None)) canvas.gameObject.SetActive(false);
            Capture(Camera.main,name);
        }
        static void CaptureInspection(string name)
        {
            // The real scene's lights remain in place; temporarily rotate/enlarge only the cosmetic shell.
            var shooter=Shooter().transform;var camera=Camera.main;var pos=shooter.localPosition;var rot=shooter.localRotation;var scale=shooter.localScale;
            shooter.position=camera.transform.TransformPoint(new Vector3(-.04f,-.04f,1.8f));shooter.rotation=camera.transform.rotation*Quaternion.Euler(0,145,0);shooter.localScale=Vector3.one*1.8f;
            Capture(camera,name);shooter.localPosition=pos;shooter.localRotation=rot;shooter.localScale=scale;
        }
        static void Capture(Camera camera,string name)
        {
            bool async=ShaderUtil.allowAsyncCompilation;ShaderUtil.allowAsyncCompilation=false;
            var target=RenderTexture.GetTemporary(1600,1000,24,RenderTextureFormat.ARGB32);var previous=RenderTexture.active;var old=camera.targetTexture;float aspect=camera.aspect;
            try
            {
                camera.targetTexture=target;camera.aspect=1.6f;
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                RenderTexture.active=target;var pixels=new Texture2D(1600,1000,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,1600,1000),0,0);pixels.Apply();
                File.WriteAllBytes(Root+"/Previews/"+name+".png",pixels.EncodeToPNG());Object.DestroyImmediate(pixels);
            }
            finally{camera.targetTexture=old;camera.aspect=aspect;RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);ShaderUtil.allowAsyncCompilation=async;}
        }
    }
}
