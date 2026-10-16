using System;
using UnityEngine;

/// <summary>Random coordinated gun/paint palette, held for this gun's lifetime.</summary>
public sealed class StudioGunAppearance : MonoBehaviour
{
    [Serializable]
    public struct Theme
    {
        public string name;
        public Color body, paint, muzzle, hardware;
    }

    [Serializable]
    public struct Region
    {
        public Renderer renderer;
        public int materialIndex;
        [Tooltip("0 body, 1 paint/grip, 2 muzzle, 3 hardware")]
        public int role;
    }

    public Theme[] themes = Array.Empty<Theme>();
    public Region[] regions = Array.Empty<Region>();
    public ParticleSystem muzzleSpray;
    public int ThemeIndex { get; private set; }
    public Color PaintColor => themes.Length == 0 ? Color.white : themes[ThemeIndex].paint;

    void Awake()
    {
        if (themes.Length > 0) ApplyTheme(new System.Random(Guid.NewGuid().GetHashCode()).Next(themes.Length));
    }

    public void ApplyTheme(int index)
    {
        if (themes.Length == 0) return;
        ThemeIndex = Mathf.Clamp(index, 0, themes.Length - 1);
        var theme = themes[ThemeIndex];
        foreach (var region in regions)
        {
            if (region.renderer == null) continue;
            var color = region.role == 0 ? theme.body : region.role == 1 ? theme.paint : region.role == 2 ? theme.muzzle : theme.hardware;
            StudioPaintTint.Set(region.renderer, color, region.materialIndex);
        }
        if (muzzleSpray != null) StudioPaintTint.Effect(muzzleSpray.gameObject, theme.paint);
    }

    public void ColorizeShot(GameObject projectile) => StudioPaintTint.Effect(projectile, PaintColor);
}
