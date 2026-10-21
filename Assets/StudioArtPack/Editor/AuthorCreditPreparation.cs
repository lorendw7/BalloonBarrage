using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
 public static class AuthorCreditPreparation
 {
  const string Prefab = "Assets/StudioArtPack/InterfaceV2/Prefabs/StartScreen.prefab";
  public const string Copy = "何单东 Shandong He";
  const string Heading = "CREATED BY";
  static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }

  static GameObject Element(Transform parent, string name, Type graphic, Vector2 position, Vector2 size)
  {
   var existing = parent.Find(name);
   var obj = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), graphic);
   obj.layer = 5;
   var rect = obj.GetComponent<RectTransform>(); rect.SetParent(parent, false);
   rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1, 0);
   rect.anchoredPosition = position; rect.sizeDelta = size; rect.localScale = Vector3.one;
   rect.localRotation = Quaternion.identity;
   return obj;
  }

  static void SetText(Text text, Font font, string copy, int size, Color color)
  {
   text.font = font; text.fontSize = size; text.fontStyle = FontStyle.Normal;
   text.alignment = TextAnchor.MiddleLeft; text.color = color; text.raycastTarget = false;
   text.supportRichText = false; text.resizeTextForBestFit = false; text.text = copy;
   text.horizontalOverflow = HorizontalWrapMode.Overflow; text.verticalOverflow = VerticalWrapMode.Truncate;
  }

  // Presentation maintenance only. Preserve the existing Text/component IDs and scene callbacks.
  public static void Build()
  {
   Require(Application.isBatchMode, "Prepare in an isolated batch copy.");
   var root = PrefabUtility.LoadPrefabContents(Prefab);
   try
   {
    var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/StudioArtPack/InterfaceV2/Fonts/AuthorCredit/BalloonCreditSans-Regular.otf");
    var rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    Require(font != null && rounded != null, "Missing author font or rounded sprite.");
    var plate = Element(root.transform, "AuthorCreditBackdrop", typeof(Image), new Vector2(-32, 30), new Vector2(320, 82));
    var image = plate.GetComponent<Image>(); image.sprite = rounded; image.type = Image.Type.Sliced;
    image.color = new Color(.98f, .95f, .87f, .97f); image.raycastTarget = false;
    var outline = plate.GetComponent<Outline>() ?? plate.AddComponent<Outline>();
    outline.effectColor = new Color(.36f, .25f, .14f, .5f); outline.effectDistance = new Vector2(1, -1);
    var accent = Element(root.transform, "AuthorCreditAccent", typeof(Image), new Vector2(-331, 51), new Vector2(3, 40));
    accent.GetComponent<Image>().color = new Color(.20f, .39f, .35f, .75f); accent.GetComponent<Image>().raycastTarget = false;
    var heading = Element(root.transform, "AuthorCreditHeading", typeof(Text), new Vector2(-54, 79), new Vector2(260, 18));
    SetText(heading.GetComponent<Text>(), font, Heading, 12, new Color(.40f, .34f, .25f, 1));
    var name = Element(root.transform, "AuthorCredit", typeof(Text), new Vector2(-54, 43), new Vector2(260, 34));
    SetText(name.GetComponent<Text>(), font, Copy, 22, new Color(.12f, .23f, .20f, 1));
    foreach (var shadow in name.GetComponents<Shadow>()) Object.DestroyImmediate(shadow);
    plate.transform.SetAsLastSibling(); accent.transform.SetAsLastSibling();
    heading.transform.SetAsLastSibling(); name.transform.SetAsLastSibling();
    PrefabUtility.SaveAsPrefabAsset(root, Prefab);
   }
   finally { PrefabUtility.UnloadPrefabContents(root); }
   AssetDatabase.SaveAssets();
  }

  public static void Verify()
  {
   Require(Application.isBatchMode, "Verify in an isolated batch copy.");
   foreach (var size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080), new Vector2Int(2560,1440), new Vector2Int(1280,960), new Vector2Int(3440,1440) })
   {
    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    var camera = new GameObject("AuthorCreditPreviewCamera").AddComponent<Camera>(); camera.nearClipPlane = .01f; camera.farClipPlane = 10;
    camera.GetUniversalAdditionalCameraData().renderPostProcessing = false;
    var target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32); target.Create(); camera.targetTexture = target;
    var previous = RenderTexture.active;
    var root = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Prefab));
    try
    {
     root.SetActive(true); var canvas = root.GetComponent<Canvas>(); var scaler = root.GetComponent<CanvasScaler>();
     var reference = scaler.referenceResolution; float match = scaler.matchWidthOrHeight; scaler.enabled = false;
     canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
     canvas.scaleFactor = Mathf.Pow(size.x / reference.x, 1 - match) * Mathf.Pow(size.y / reference.y, match); camera.aspect = (float)size.x / size.y;
     var name = root.transform.Find("AuthorCredit").GetComponent<Text>();
     var heading = root.transform.Find("AuthorCreditHeading").GetComponent<Text>();
     Require(name.text == Copy && heading.text == Heading, "Author copy changed.");
     foreach (var text in new[] { name, heading })
     {
      text.font.RequestCharactersInTexture(text.text, text.fontSize, FontStyle.Normal);
      foreach (char c in text.text.Where(c => !char.IsWhiteSpace(c))) Require(text.font.HasCharacter(c), "Missing author glyph " + c);
     }
     Canvas.ForceUpdateCanvases();
     Rect Bounds(RectTransform rect)
     {
      var corners = new Vector3[4]; rect.GetWorldCorners(corners); var points = corners.Select(root.transform.InverseTransformPoint).ToArray();
      return Rect.MinMaxRect(points.Min(p => p.x), points.Min(p => p.y), points.Max(p => p.x), points.Max(p => p.y));
     }
     var plate = root.transform.Find("AuthorCreditBackdrop").GetComponent<RectTransform>(); var plateBounds = Bounds(plate);
     foreach (var text in new[] { name, heading })
     {
      Require(text.preferredWidth <= text.rectTransform.rect.width + 1 && text.preferredHeight <= text.rectTransform.rect.height + 1, "Text overflow: " + text.name + " at " + size);
      var bounds = Bounds(text.rectTransform); Require(plateBounds.Contains(bounds.min) && plateBounds.Contains(bounds.max), "Text outside plaque.");
     }
     Require(!Bounds(name.rectTransform).Overlaps(Bounds(heading.rectTransform)), "Name and heading overlap.");
     foreach (string element in new[] { "AuthorCreditBackdrop", "AuthorCreditAccent", "AuthorCreditHeading", "AuthorCredit" })
     {
      var graphic = root.transform.Find(element).GetComponent<Graphic>(); Require(!graphic.raycastTarget, "Plaque intercepts input.");
      var corners = new Vector3[4]; graphic.rectTransform.GetWorldCorners(corners);
      foreach (var corner in corners) { var p = camera.WorldToViewportPoint(corner); Require(p.x >= 0 && p.x <= 1 && p.y >= 0 && p.y <= 1, "Credit outside viewport at " + size); }
     }
     foreach (string other in new[] { "Footer", "StartButton", "QuitButton" }) Require(!plateBounds.Overlaps(Bounds((RectTransform)root.transform.Find(other))), "Plaque overlaps " + other + " at " + size);
     Canvas.ForceUpdateCanvases(); camera.Render();
     var request = new UniversalRenderPipeline.SingleCameraRequest { destination = target };
     if (UnityEngine.Rendering.RenderPipeline.SupportsRenderRequest(camera, request)) UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera, request);
     camera.Render();
     if (size.x == 1920 && size.y == 1080)
     {
      var pixels = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
      try { RenderTexture.active = target; pixels.ReadPixels(new Rect(0,0,size.x,size.y),0,0); pixels.Apply(); Directory.CreateDirectory("Assets/StudioArtPack/InterfaceV2/Previews"); File.WriteAllBytes("Assets/StudioArtPack/InterfaceV2/Previews/Start_AuthorCredit_1080p.png", pixels.EncodeToPNG()); }
      finally { Object.DestroyImmediate(pixels); }
     }
     Debug.Log("AUTHOR_CREDIT_LAYOUT_OK: " + size);
    }
    finally { RenderTexture.active = previous; Object.DestroyImmediate(root); camera.targetTexture = null; target.Release(); Object.DestroyImmediate(target); Object.DestroyImmediate(camera.gameObject); }
   }
   EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
   var controller = Object.FindFirstObjectByType<RoundController>(); var so = new SerializedObject(controller);
   var screen = (GameObject)so.FindProperty("startScreen").objectReferenceValue;
   Require(screen.transform.Find("AuthorCreditHeading") != null, "Production scene did not inherit plaque.");
   foreach (string buttonName in new[] { "StartButton", "QuitButton" })
   {
    var button = screen.transform.Find(buttonName).GetComponent<Button>();
    Require(button.onClick.GetPersistentMethodName(0) == (buttonName == "StartButton" ? "BeginRound" : "QuitGame") && button.onClick.GetPersistentTarget(0) == controller, "Button callback changed: " + buttonName);
   }
   Debug.Log("AUTHOR_CREDIT_VERIFY_OK: glyphs, five resolutions, plaque/text fit, raycasts and scene callbacks.");
  }

  public static void RunBatch()
  {
   try { Build(); Verify(); EditorApplication.Exit(0); }
   catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
  }
 }
}