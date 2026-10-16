using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Presentation assets only. Buttons deliberately have no gameplay callbacks.
public static class StudioInterfaceBuilder
{
    const string Root = "Assets/StudioArtPack/InterfaceV2";
    const string Atlas = Root + "/Textures/InterfaceAtlas_v2.png";
    static readonly string[] Screens = { "StartScreen", "PauseScreen", "ResultsScreen", "InstructionsScreen", "GameplayHUD" };
    static readonly string[] SpriteNames = { "Button_Coral", "Button_Teal", "Button_Cream", "Panel_Paper", "Plaque_Score", "Plaque_Timer", "Icon_Play", "Icon_Pause", "Icon_Quit" };
    static Color Ink => Hex("303C39");
    static Color Muted => Hex("62574B");
    static Font Font => AssetDatabase.LoadAssetAtPath<Font>(Root + "/Fonts/Lato-Regular.ttf");
    static Font BoldFont => AssetDatabase.LoadAssetAtPath<Font>(Root + "/Fonts/Lato-Bold.ttf");

    [MenuItem("Tools/Balloon Studio/Build Interface V2 Assets")]
    public static void Build()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play Mode first.");
        if (Directory.Exists(Root + "/Prefabs") && Directory.GetFiles(Root + "/Prefabs", "*.prefab").Length != 0)
            throw new InvalidOperationException("Interface V2 already exists. Edit its prefabs instead of regenerating them.");
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try
        {
            ImportSprites();
            Directory.CreateDirectory(Root + "/Prefabs");
            Directory.CreateDirectory(Root + "/Previews");
            AssetDatabase.Refresh();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            foreach (string screen in Screens)
            {
                var canvas = BuildScreen(screen);
                PrefabUtility.SaveAsPrefabAsset(canvas.gameObject, Root + "/Prefabs/" + screen + ".prefab");
                UnityEngine.Object.DestroyImmediate(canvas.gameObject);
            }
            var camera = new GameObject("PreviewCamera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Hex("394642");
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule))
                .GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            foreach (string screen in Screens)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "/Prefabs/" + screen + ".prefab"));
                go.SetActive(screen == "StartScreen");
            }
            EditorSceneManager.SaveScene(scene, Root + "/InterfaceGallery.unity");
            AssetDatabase.SaveAssets();
            CaptureBatch();
            Debug.Log("INTERFACE_V2_OK: 9 sprites, 5 presentation prefabs, gallery, 720p/1080p renders. No gameplay callbacks installed.");
        }
        finally { if (!Application.isBatchMode) EditorSceneManager.RestoreSceneManagerSetup(setup); }
    }

    static void ImportSprites()
    {
        AssetDatabase.Refresh();
        Configure(Root + "/Textures/StartBackground_v2.png", false);
        var importer = Configure(Atlas, true);
        importer.GetSourceTextureWidthAndHeight(out int w, out int h);
        if (w != 1254 || h != 1254) throw new InvalidOperationException("Expected original 1254 x 1254 generated atlas.");
        // Bounds include transparent padding; measured from the original alpha, top-left coordinates.
        var bounds = new[] {
            new RectInt(30,150,386,155), new RectInt(435,150,386,155), new RectInt(839,150,385,155),
            new RectInt(43,444,365,347), new RectInt(436,544,385,147), new RectInt(836,544,386,147),
            new RectInt(134,906,205,236), new RectInt(521,897,213,250), new RectInt(910,897,273,261)
        };
        var factories = new SpriteDataProviderFactories(); factories.Init();
        var provider = factories.GetSpriteEditorDataProviderFromObject(importer); provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        var sprites = bounds.Select((b, i) => new SpriteRect {
            name = SpriteNames[i], rect = new Rect(b.x, h-b.y-b.height, b.width, b.height),
            alignment = SpriteAlignment.Center, pivot = Vector2.one * .5f,
            border = i < 3 ? new Vector4(64,45,64,45) : i == 3 ? new Vector4(65,65,65,65) : i < 6 ? new Vector4(80,42,56,42) : Vector4.zero,
            spriteID = previous.FirstOrDefault(s => s.name == SpriteNames[i])?.spriteID ?? GUID.Generate()
        }).ToArray();
        provider.SetSpriteRects(sprites);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(sprites.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply(); importer.SaveAndReimport();
        if (AssetDatabase.LoadAllAssetsAtPath(Atlas).OfType<Sprite>().Count() != 9) throw new InvalidOperationException("Atlas slicing failed.");
    }

    static TextureImporter Configure(string path, bool multiple)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new FileNotFoundException(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = multiple ? SpriteImportMode.Multiple : SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaIsTransparency = multiple;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport(); return importer;
    }

    static Canvas BuildScreen(string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = name == "GameplayHUD" ? 10 : 20;
        var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1600,900); scaler.matchWidthOrHeight = .5f;
        var root = go.GetComponent<RectTransform>();
        if (name == "StartScreen")
        {
            var background = Picture(root, "Background", AssetDatabase.LoadAssetAtPath<Sprite>(Root + "/Textures/StartBackground_v2.png"), 0,0,1600,900);
            Stretch(background.rectTransform);
            var fit = background.gameObject.AddComponent<AspectRatioFitter>(); fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; fit.aspectRatio = 1672f/941f;
            Label(root, "Eyebrow", "THE PAINT STUDIO", 0,-260,640,30,20,Muted);
            Label(root, "Title", "BalloonBarrage", 0,-180,880,100,64,Ink,FontStyle.Bold);
            Label(root, "Subtitle", "A little color. A lot of popping.", 0,-101,780,50,25,Muted);
            var panel = Picture(root,"ControlsPanel", Sprite("Panel_Paper"),0,60,600,200,true);
            Label(panel.rectTransform,"ControlsTitle","HOW TO PLAY",0,-53,480,32,20,Ink,FontStyle.Bold);
            Label(panel.rectTransform,"ControlsText","Mouse to aim  /  Hold left click to fire\nEsc to pause or resume\nPop balloons before time runs out.",0,15,490,102,23,Ink);
            Action(root,"StartButton","Start", "Button_Coral",-170,258,310,84);
            Action(root,"QuitButton","Quit", "Button_Cream",170,258,310,84);
            Label(root,"Footer","Make your mark.",0,345,600,32,20,Muted);
        }
        else if (name == "GameplayHUD")
        {
            var score = Picture(root,"ScorePlaque",Sprite("Plaque_Score"),158,78,252,98,true); TopLeft(score.rectTransform,158,78);
            Label(score.rectTransform,"ScoreCaption","SCORE",0,-23,180,24,16,Muted,FontStyle.Bold);
            Label(score.rectTransform,"ScoreValue","001250",0,12,180,45,34,Ink,FontStyle.Bold);
            var timer = Picture(root,"TimerPlaque",Sprite("Plaque_Timer"),0,78,224,98,true);
            timer.rectTransform.anchorMin = timer.rectTransform.anchorMax = new Vector2(1,1); timer.rectTransform.anchoredPosition = new Vector2(-144,-78);
            Label(timer.rectTransform,"TimerCaption","TIME LEFT",0,-23,170,24,16,Muted,FontStyle.Bold);
            Label(timer.rectTransform,"TimeValue","01:30",0,12,170,45,34,Ink,FontStyle.Bold);
            var hint = Label(root,"PauseHint","ESC  /  PAUSE",0,145,224,30,18,Color.white);
            hint.rectTransform.anchorMin = hint.rectTransform.anchorMax = new Vector2(1,1); hint.rectTransform.anchoredPosition = new Vector2(-144,-145);
            // Decorative graphics never consume UI raycasts.
        }
        else
        {
            var dim = Picture(root,"Dimmer",null,0,0,1600,900); dim.color = new Color(.045f,.075f,.07f,.72f); Stretch(dim.rectTransform); dim.raycastTarget = true;
            var panel = Picture(root,"Dialog",Sprite("Panel_Paper"),0,0,640,620,true);
            var p = panel.rectTransform;
            if (name == "PauseScreen")
            {
                Picture(p,"PauseIcon",Sprite("Icon_Pause"),0,-195,54,64);
                Label(p,"Title","Take a breather",0,-117,540,65,42,Ink,FontStyle.Bold);
                Label(p,"Description","Your round is paused.\nPress Esc or choose Resume.",0,-35,500,78,24,Muted);
                Action(p,"ResumeButton","Resume","Button_Teal",0,85,360,84);
                Action(p,"QuitButton","Quit","Button_Cream",0,195,360,84);
            }
            else if (name == "ResultsScreen")
            {
                Label(p,"Eyebrow","ROUND COMPLETE",0,-220,520,35,20,Muted,FontStyle.Bold);
                Label(p,"Title","Color well spent!",0,-151,550,70,42,Ink,FontStyle.Bold);
                var plaque = Picture(p,"FinalScorePlaque",Sprite("Plaque_Score"),0,-21,390,132,true);
                Label(plaque.rectTransform,"ScoreCaption","FINAL SCORE",0,-31,280,28,18,Muted,FontStyle.Bold);
                Label(plaque.rectTransform,"FinalScoreValue","001250",0,18,270,60,46,Ink,FontStyle.Bold);
                Action(p,"PlayAgainButton","Play Again","Button_Coral",0,125,360,84);
                Label(p,"ReplayHint","Returns to the start screen.",0,223,500,40,21,Muted);
            }
            else
            {
                Label(p,"Title","How to play",0,-220,520,70,46,Ink,FontStyle.Bold);
                string[] heading = {"01   AIM", "02   PAINT", "03   PAUSE"};
                string[] copy = {"Move your mouse to aim the blaster.","Hold the left mouse button to fire.","Press Esc to pause or resume."};
                for (int i=0;i<3;i++) {
                    Label(p,"Step"+i,heading[i],0,-132+i*96,500,34,22,Hex("287E80"),FontStyle.Bold);
                    Label(p,"StepCopy"+i,copy[i],0,-92+i*96,510,42,22,Ink);
                }
                Label(p,"Goal","Pop balloons before the time runs out.",0,163,520,42,22,Muted);
                // Optional teaching layout: no new navigation behavior is implied.
                Action(p,"BackButton","Back","Button_Cream",0,238,330,72);
            }
        }
        return canvas;
    }

    static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
    {
        var go = new GameObject(name,typeof(RectTransform)); go.layer = LayerMask.NameToLayer("UI");
        var rt = go.GetComponent<RectTransform>(); rt.SetParent(parent,false); rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.one*.5f;
        rt.sizeDelta = new Vector2(w,h); rt.anchoredPosition = new Vector2(x,-y); return rt;
    }
    static Image Picture(Transform parent,string name,Sprite sprite,float x,float y,float w,float h,bool sliced=false)
    {
        var rt=Rect(parent,name,x,y,w,h); var image=rt.gameObject.AddComponent<Image>(); image.sprite=sprite; image.raycastTarget=false;
        image.type=sliced?Image.Type.Sliced:Image.Type.Simple; image.preserveAspect=!sliced && sprite!=null; image.pixelsPerUnitMultiplier=1.4f; return image;
    }
    static Text Label(Transform parent,string name,string value,float x,float y,float w,float h,int size,Color color,FontStyle style=FontStyle.Normal)
    {
        bool bold = style == FontStyle.Bold || style == FontStyle.BoldAndItalic;
        var text=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Text>(); text.text=value; text.font=bold?BoldFont:Font; text.fontSize=size;
        text.color=color; text.fontStyle=bold?(style==FontStyle.BoldAndItalic?FontStyle.Italic:FontStyle.Normal):style; text.alignment=TextAnchor.MiddleCenter; text.raycastTarget=false;
        text.horizontalOverflow=HorizontalWrapMode.Wrap; text.verticalOverflow=VerticalWrapMode.Truncate; return text;
    }
    static void Action(Transform parent,string name,string title,string sprite,float x,float y,float w,float h)
    {
        var image=Picture(parent,name,Sprite(sprite),x,y,w,h,true); image.raycastTarget=true;
        var button=image.gameObject.AddComponent<Button>(); button.targetGraphic=image;
        var colors=button.colors; colors.normalColor=Color.white; colors.highlightedColor=new Color(1.08f,1.08f,1.08f); colors.pressedColor=new Color(.74f,.78f,.76f);
        colors.selectedColor=new Color(.88f,1f,.95f); colors.disabledColor=new Color(.55f,.55f,.55f,.65f); colors.fadeDuration=.1f; button.colors=colors;
        Label(image.rectTransform,"Label",title,0,-2,w-68,h-24,28,sprite=="Button_Teal"?Hex("FFF4DF"):Ink,FontStyle.Bold);
    }
    static void Stretch(RectTransform rt) {rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;}
    static void TopLeft(RectTransform rt,float x,float y) {rt.anchorMin=rt.anchorMax=new Vector2(0,1);rt.anchoredPosition=new Vector2(x,-y);}
    static Sprite Sprite(string name) => AssetDatabase.LoadAllAssetsAtPath(Atlas).OfType<Sprite>().Single(s=>s.name==name);
    static Color Hex(string html) {ColorUtility.TryParseHtmlString("#"+html,out var c);return c;}

    public static void CaptureBatch()
    {
        if (!Application.isBatchMode) return;
        Directory.CreateDirectory(Root+"/Previews");
        var scene=EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        var camera=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Camera>()).Single(c=>c.CompareTag("MainCamera"));
        Render(camera,null,1600,900,"StudioScene_v2");
        foreach(string screen in Screens)
        {
            var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/"+screen+".prefab"));
            try {
                var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=.5f;
                foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)}) Render(camera,canvas,size.x,size.y,screen+"_"+size.y+"p");
                if(screen=="StartScreen") Render(camera,canvas,1280,960,screen+"_4x3");
            } finally {UnityEngine.Object.DestroyImmediate(go);}
        }
        AssetDatabase.Refresh();
        foreach (var path in Directory.GetFiles(Root+"/Previews","*.png")) {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.SaveAndReimport();
        }
    }

    static void Render(Camera camera,Canvas canvas,int width,int height,string name)
    {
        var previous=camera.targetTexture;var active=RenderTexture.active;float aspect=camera.aspect;bool async=ShaderUtil.allowAsyncCompilation;
        var target=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32);var pixels=new Texture2D(width,height,TextureFormat.RGB24,false);
        try {
            ShaderUtil.allowAsyncCompilation=false;target.Create();camera.targetTexture=target;camera.aspect=(float)width/height;
            Canvas.ForceUpdateCanvases();
            if(canvas!=null) {
                canvas.GetComponent<CanvasScaler>().enabled=false;
                canvas.scaleFactor=Mathf.Sqrt((width/1600f)*(height/900f));
                Canvas.ForceUpdateCanvases();
                foreach(var text in canvas.GetComponentsInChildren<Text>())
                    if(text.preferredHeight>text.rectTransform.rect.height+1 || text.preferredWidth>text.rectTransform.rect.width+1)
                        throw new InvalidOperationException("Text overflows: "+name+"/"+text.name);
            }
            camera.Render();var request=new UniversalRenderPipeline.SingleCameraRequest {destination=target};
            if(RenderPipeline.SupportsRenderRequest(camera,request))RenderPipeline.SubmitRenderRequest(camera,request);else camera.Render();
            RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,width,height),0,0);pixels.Apply();
            File.WriteAllBytes(Root+"/Previews/"+name+".png",pixels.EncodeToPNG());
        } finally {camera.targetTexture=previous;camera.aspect=aspect;RenderTexture.active=active;ShaderUtil.allowAsyncCompilation=async;target.Release();UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(pixels);}
    }
}
