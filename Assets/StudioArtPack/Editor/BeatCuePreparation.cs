using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// UI asset preparation only. Timing-driven fill remains learner integration.
public static class BeatCuePreparation
{
    const string Cue = "Assets/StudioArtPack/InterfaceV2/Prefabs/RhythmCue.prefab";
    public const string Root = "Assets/StudioArtPack/BeatCue20261020";
    static Color Teal => new Color(.08f, .43f, .43f);
    static void Require(bool c, string m) { if (!c) throw new InvalidOperationException(m); }
    static GameObject Child(Transform parent, string name, Type component, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), component); go.layer = 5;
        var r = (RectTransform)go.transform; r.SetParent(parent, false); r.anchorMin = r.anchorMax = r.pivot = Vector2.one * .5f; r.anchoredPosition = pos; r.sizeDelta = size;
        go.GetComponent<Graphic>().raycastTarget = false; return go;
    }
    static void TextStyle(Text text, Font font, string copy, int size)
    {
        text.font = font; text.text = copy; text.fontSize = size; text.fontStyle = FontStyle.Bold; text.alignment = TextAnchor.MiddleCenter;
        text.color = Teal; text.raycastTarget = false; text.supportRichText = false; text.resizeTextForBestFit = false;
    }
    public static void Run()
    {
        try
        {
            Require(Application.isBatchMode && Application.dataPath.Contains(".local-backups"), "Use an isolated batch copy.");
            Require(!Directory.Exists(Root), "Preserve delivered UI assets."); Directory.CreateDirectory(Root + "/Previews"); AssetDatabase.Refresh();
            var root = PrefabUtility.LoadPrefabContents(Cue);
            try
            {
                Require(root.transform.Find("BeatCueHeading") == null, "Already refined.");
                var text = root.transform.Find("BeatCueText").GetComponent<Text>(); var font = text.font;
                var panel = root.GetComponent<Image>(); panel.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"); panel.type = Image.Type.Sliced;
                panel.color = new Color(1, .97f, .89f, .98f); panel.raycastTarget = false;
                var border = root.AddComponent<Outline>(); border.effectColor = new Color(.1f,.35f,.32f,.7f); border.effectDistance = new Vector2(1.5f,-1.5f);
                text.rectTransform.anchoredPosition = new Vector2(0,-9); text.rectTransform.sizeDelta = new Vector2(360,38);
                TextStyle(text,font,"READY",30);
                TextStyle(Child(root.transform,"BeatCueHeading",typeof(Text),new Vector2(0,24),new Vector2(360,20)).GetComponent<Text>(),font,"BEAT BONUS  /  OPTIONAL",13);
                PrefabUtility.SaveAsPrefabAsset(root,Cue);
                // A separate, unconnected graphic prototype. No static bar is inserted into live gameplay.
                var guide = Object.Instantiate(root); guide.name = "BeatApproachGuide";
                try
                {
                    Object.DestroyImmediate(guide.transform.Find("BeatCueText").gameObject);
                    var track = Child(guide.transform,"ApproachTrack",typeof(Image),new Vector2(-10,-7),new Vector2(300,12)).GetComponent<Image>(); track.color = new Color(.78f,.83f,.78f);
                    var fill = Child(track.transform,"BeatApproachFill",typeof(Image),Vector2.zero,new Vector2(300,12)).GetComponent<Image>();
                    fill.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"); fill.type = Image.Type.Filled; fill.fillMethod = Image.FillMethod.Horizontal; fill.fillOrigin = 0; fill.fillAmount = 0; fill.color = Teal;
                    var gate = Child(guide.transform,"TimingGate",typeof(Image),new Vector2(140,-7),new Vector2(4,30)).GetComponent<Image>(); gate.color = new Color(.86f,.59f,.14f);
                    TextStyle(Child(guide.transform,"TimingHint",typeof(Text),new Vector2(0,-28),new Vector2(350,17)).GetComponent<Text>(),font,"FIRE AS THE BAR REACHES THE GOLD LINE",10);
                    PrefabUtility.SaveAsPrefabAsset(guide,Root+"/BeatApproachGuide.prefab");
                }
                finally { Object.DestroyImmediate(guide); }
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            AssetDatabase.SaveAssets(); Verify();
            File.WriteAllText(Root+"/Verification.txt","BEAT_CUE_ASSETS_OK\nExisting cue Text/file ID retained; no learner scripts or timing/scoring changed.\nLive cue contrast and heading polished. Separate approach guide is not connected to gameplay.\nLayout checked at 1280x720, 1920x1080 and 2560x1440.\nPreview fill and text values are static demonstration data, not a timed runtime test.\n");
            AssetDatabase.Refresh(); Debug.Log("BEAT_CUE_ASSETS_OK"); EditorApplication.Exit(0);
        }
        catch(Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
    static void Verify()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        var host = new GameObject("PreviewCanvas",typeof(Canvas),typeof(CanvasScaler)); var canvas = host.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceCamera;
        var camera = new GameObject("PreviewCamera",typeof(Camera)).GetComponent<Camera>(); camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.17f,.24f,.22f); canvas.worldCamera = camera;
        host.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; host.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920,1080);
        foreach(string path in new[]{Cue,Root+"/BeatApproachGuide.prefab"})
        {
            var go = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path),host.transform,false);
            foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080),new Vector2Int(2560,1440)})
            {
                var target = new RenderTexture(size.x,size.y,24); target.Create(); camera.targetTexture = target; canvas.planeDistance = 1;
                foreach(var text in go.GetComponentsInChildren<Text>())
                {
                    if(text.name=="BeatCueText") text.text="NEXT 0.8s";
                    Require(text.font.HasCharacter('0'),"Font unavailable.");
                }
                var fill = go.transform.Find("ApproachTrack/BeatApproachFill"); if(fill!=null) fill.GetComponent<Image>().fillAmount=.72f;
                Canvas.ForceUpdateCanvases();
                foreach(var text in go.GetComponentsInChildren<Text>()) Require(text.preferredWidth<=text.rectTransform.rect.width+1 && text.preferredHeight<=text.rectTransform.rect.height+1,"Text overflow: "+text.name);
                Require(go.GetComponentsInChildren<Graphic>().All(g=>!g.raycastTarget),"UI intercepts mouse.");
                camera.Render();
                if(size.x==1920)
                {
                    var old=RenderTexture.active; RenderTexture.active=target; var image=new Texture2D(size.x,size.y,TextureFormat.RGB24,false); image.ReadPixels(new Rect(0,0,size.x,size.y),0,0); image.Apply(); File.WriteAllBytes(Root+"/Previews/"+(path==Cue?"CurrentCue":"PreparedApproach")+".png",image.EncodeToPNG()); Object.DestroyImmediate(image); RenderTexture.active=old;
                }
                camera.targetTexture=null; target.Release(); Object.DestroyImmediate(target);
            }
            Object.DestroyImmediate(go);
        }
        Object.DestroyImmediate(host); Object.DestroyImmediate(camera.gameObject);
        EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
        var reader=Object.FindFirstObjectByType<RhythmChartReader>(); var so=new SerializedObject(reader); var cue=(Text)so.FindProperty("beatCueText").objectReferenceValue;
        Require(cue!=null && cue.transform.parent.Find("BeatCueHeading")!=null,"Live scene binding lost.");
    }
}
