using System;
using UnityEngine;

/// <summary>One appearance selection per spawn. Never changes collision, movement or points.</summary>
public sealed class StudioBalloonAppearance : MonoBehaviour
{
    public MeshFilter body;
    public Renderer latex;
    public Mesh[] silhouettes = Array.Empty<Mesh>();
    public Material[] pigments = Array.Empty<Material>();
    public int ColorIndex { get; private set; }
    public int ShapeIndex { get; private set; }
    public Color Pigment => pigments[ColorIndex].GetColor("_BaseColor");

    void Awake()
    {
        // Keep art randomness independent of the learner's spawn/movement random stream.
        var random = new System.Random(Guid.NewGuid().GetHashCode());
        if (pigments.Length > 0 && silhouettes.Length > 0)
            ApplyVariant(random.Next(pigments.Length), random.Next(silhouettes.Length));
    }

    public void ApplyVariant(int colorIndex, int shapeIndex)
    {
        if (body == null || latex == null || pigments.Length == 0 || silhouettes.Length == 0) return;
        ColorIndex = Mathf.Clamp(colorIndex, 0, pigments.Length - 1);
        ShapeIndex = Mathf.Clamp(shapeIndex, 0, silhouettes.Length - 1);
        body.sharedMesh = silhouettes[ShapeIndex];
        latex.sharedMaterial = pigments[ColorIndex];
    }

    public void ColorizeBurst(GameObject burst) => StudioPaintTint.Effect(burst, Pigment);
}
