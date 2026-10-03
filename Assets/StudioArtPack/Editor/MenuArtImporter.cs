using System;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.Linq;
using UnityEngine;

// Editor-only asset preparation. Does not modify scenes or gameplay.
public static class MenuArtImporter
{
    const string Root = "Assets/StudioArtPack/Menu";

    [MenuItem("Tools/Balloon Studio/Import Menu Art")]
    public static void Build()
    {
        AssetDatabase.Refresh();
        Configure(Root + "/MenuBackground_v1.png", false);
        var atlas = Configure(Root + "/MenuUI_Atlas_v1.png", true);
        atlas.GetSourceTextureWidthAndHeight(out int width, out int height);
        if (width != 1536 || height != 1024)
            throw new InvalidOperationException("Menu atlas must be 1536 x 1024.");
        string[] names = { "Button_Coral", "Button_Teal", "Button_Yellow", "Button_Cream", "Dialog_Paper", "Score_Plaque" };
        // Tight alpha bounds plus padding, in top-left image coordinates.
        var bounds = new RectInt[] {
            new RectInt(52,50,695,238), new RectInt(783,51,703,238),
            new RectInt(51,327,698,233), new RectInt(784,327,702,233),
            new RectInt(173,571,428,413), new RectInt(717,611,690,324)
        };
        var factories = new SpriteDataProviderFactories();
        factories.Init();
        var provider = factories.GetSpriteEditorDataProviderFromObject(atlas);
        provider.InitSpriteEditorDataProvider();
        var previous = provider.GetSpriteRects();
        var slices = new SpriteRect[6];
        for (int i = 0; i < slices.Length; i++)
        {
            var b = bounds[i];
            var existing = previous.FirstOrDefault(s => s.name == names[i]);
            slices[i] = new SpriteRect {
                name = names[i], rect = new Rect(b.x, height-b.y-b.height, b.width, b.height),
                alignment = SpriteAlignment.Center, pivot = new Vector2(.5f,.5f),
                spriteID = existing != null ? existing.spriteID : GUID.Generate()
            };
        }
        provider.SetSpriteRects(slices);
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(
            slices.Select(s => new SpriteNameFileIdPair(s.name, s.spriteID)));
        provider.Apply();
        atlas.SaveAndReimport();
        int count = 0;
        foreach (var item in AssetDatabase.LoadAllAssetsAtPath(Root + "/MenuUI_Atlas_v1.png"))
            if (item is Sprite) count++;
        if (count != 6) throw new InvalidOperationException("Expected six menu sprites, got " + count);
        AssetDatabase.SaveAssets();
        Debug.Log("MENU_ART_OK: background plus 6 UI sprites");
    }

    static TextureImporter Configure(string path, bool multiple)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("Missing art: " + path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = multiple ? SpriteImportMode.Multiple : SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = multiple;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.maxTextureSize = 2048;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        return importer;
    }
}
