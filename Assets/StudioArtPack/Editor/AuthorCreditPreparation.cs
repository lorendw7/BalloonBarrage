using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Object=UnityEngine.Object;
namespace BalloonBarrage.EditorTools {
 public static class AuthorCreditPreparation {
  const string Prefab="Assets/StudioArtPack/InterfaceV2/Prefabs/StartScreen.prefab";
  public const string Copy="Created by 何单东 Shandong He";
  static void Require(bool value,string message){if(!value)throw new Exception(message);}
  public static void Build(){
   Require(Application.isBatchMode,"Prepare in an isolated batch copy.");
   var root=PrefabUtility.LoadPrefabContents(Prefab);
   try{
    var existing=root.transform.Find("AuthorCredit");
    var font=AssetDatabase.LoadAssetAtPath<Font>("Assets/StudioArtPack/InterfaceV2/Fonts/AuthorCredit/BalloonCreditSans-Regular.otf");
    Require(font!=null,"Missing author font");
    var o=existing!=null?existing.gameObject:new GameObject("AuthorCredit",typeof(RectTransform),typeof(CanvasRenderer),typeof(Text));o.layer=5;
    var rect=o.GetComponent<RectTransform>();rect.SetParent(root.transform,false);
    rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(1,0);
    rect.anchoredPosition=new Vector2(-24,24);rect.sizeDelta=new Vector2(600,44);rect.localScale=Vector3.one;
    var text=o.GetComponent<Text>();text.font=font;text.fontSize=20;text.alignment=TextAnchor.MiddleRight;
    text.color=new Color(.12f,.21f,.17f,1);text.raycastTarget=false; var shadow=o.GetComponent<Shadow>();if(shadow!=null)Object.DestroyImmediate(shadow);
    text.supportRichText=false;text.resizeTextForBestFit=false;text.text=Copy; var priorPlate=root.transform.Find("AuthorCreditBackdrop");var plate=priorPlate!=null?priorPlate.gameObject:new GameObject("AuthorCreditBackdrop",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));plate.layer=5;var pr=plate.GetComponent<RectTransform>();pr.SetParent(root.transform,false);pr.anchorMin=pr.anchorMax=pr.pivot=new Vector2(1,0);pr.anchoredPosition=new Vector2(-12,24);pr.sizeDelta=new Vector2(text.preferredWidth+24,44);pr.localScale=Vector3.one;plate.transform.SetSiblingIndex(o.transform.GetSiblingIndex());var image=plate.GetComponent<Image>();image.color=new Color(.98f,.95f,.86f,.96f);image.raycastTarget=false;
    PrefabUtility.SaveAsPrefabAsset(root,Prefab);
   }finally{Object.DestroyImmediate(root);}
   AssetDatabase.SaveAssets();
  }
  public static void Verify(){
   foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080),new Vector2Int(2560,1440),new Vector2Int(1280,960),new Vector2Int(3440,1440)}){
    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
    var camera=new GameObject("AuthorCreditPreviewCamera").AddComponent<Camera>();camera.nearClipPlane=.01f;camera.farClipPlane=10;
    camera.GetUniversalAdditionalCameraData().renderPostProcessing=false; var viewportTarget=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);camera.targetTexture=viewportTarget;
    var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Prefab));
    root.SetActive(true);var canvas=root.GetComponent<Canvas>();var scaler=root.GetComponent<CanvasScaler>();
    var reference=scaler.referenceResolution;float match=scaler.matchWidthOrHeight;scaler.enabled=false;
    canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
    canvas.scaleFactor=Mathf.Pow(size.x/reference.x,1-match)*Mathf.Pow(size.y/reference.y,match);camera.aspect=(float)size.x/size.y;
    var text=root.transform.Find("AuthorCredit").GetComponent<Text>();
    Require(text.text==Copy&&!text.raycastTarget,"Author copy/raycast failed");
    text.font.RequestCharactersInTexture(Copy,20,FontStyle.Normal);
    foreach(char c in Copy.Where(c=>!char.IsWhiteSpace(c)))Require(text.font.HasCharacter(c),"Missing author glyph "+c);
    Canvas.ForceUpdateCanvases();
    Require(text.preferredWidth<=text.rectTransform.rect.width+1&&text.preferredHeight<=text.rectTransform.rect.height+1,"Author credit overflows at "+size+" preferred="+text.preferredWidth+"x"+text.preferredHeight+" rect="+text.rectTransform.rect);
    var corners=new Vector3[4];text.rectTransform.GetWorldCorners(corners);
    foreach(var c in corners){var p=camera.WorldToViewportPoint(c);Require(p.x>=0&&p.x<=1&&p.y>=0&&p.y<=1,"Credit outside viewport");}
    Rect Bounds(RectTransform r){var points=new Vector3[4];r.GetWorldCorners(points);var p=points.Select(c=>root.transform.InverseTransformPoint(c)).ToArray();return Rect.MinMaxRect(p.Min(c=>c.x),p.Min(c=>c.y),p.Max(c=>c.x),p.Max(c=>c.y));}
    foreach(var name in new[]{"Footer","StartButton","QuitButton"})Require(!Bounds(text.rectTransform).Overlaps(Bounds((RectTransform)root.transform.Find(name))),"Credit overlaps "+name+" size="+size+" credit="+Bounds(text.rectTransform)+" other="+Bounds((RectTransform)root.transform.Find(name)));
    if(size.x==1920&&size.y==1080){
     var rt=new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);var pixels=new Texture2D(size.x,size.y,TextureFormat.RGB24,false);
     var previous=RenderTexture.active;camera.targetTexture=rt;Canvas.ForceUpdateCanvases();camera.Render();
     var request=new UniversalRenderPipeline.SingleCameraRequest{destination=rt};if(UnityEngine.Rendering.RenderPipeline.SupportsRenderRequest(camera,request))UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,request);
     camera.Render();RenderTexture.active=rt;pixels.ReadPixels(new Rect(0,0,size.x,size.y),0,0);pixels.Apply();
     Directory.CreateDirectory("Assets/StudioArtPack/InterfaceV2/Previews");
     File.WriteAllBytes("Assets/StudioArtPack/InterfaceV2/Previews/Start_AuthorCredit_1080p.png",pixels.EncodeToPNG());
     RenderTexture.active=previous;camera.targetTexture=null;rt.Release();Object.DestroyImmediate(rt);Object.DestroyImmediate(pixels);
    }
    Object.DestroyImmediate(root);camera.targetTexture=null;viewportTarget.Release();Object.DestroyImmediate(viewportTarget);Object.DestroyImmediate(camera.gameObject);
   }
   EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
   var controller=Object.FindFirstObjectByType<RoundController>();var so=new SerializedObject(controller);
   var screen=(GameObject)so.FindProperty("startScreen").objectReferenceValue;
   Require(screen.transform.Find("AuthorCredit")!=null,"Production scene did not inherit author label");
   var start=screen.transform.Find("StartButton").GetComponent<Button>();
   Require(start.onClick.GetPersistentMethodName(0)=="BeginRound"&&start.onClick.GetPersistentTarget(0)==controller,"Start callback changed");
   Debug.Log("AUTHOR_CREDIT_VERIFY_OK: glyphs, 5 resolutions, fit/nonoverlap, scene inheritance and Start callback.");
  }
 }
}
