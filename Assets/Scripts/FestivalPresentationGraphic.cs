using UnityEngine;
using UnityEngine.UI;

/// <summary>Original procedural artwork; no official logos, mascot art, textures or real-time lights.</summary>
public sealed class FestivalPresentationGraphic : MaskableGraphic
{
    public enum PresentationMode { Dawn, Lanterns, Constellation }
    public PresentationMode Mode { get; set; }
    private float age;
    private static readonly Color Gold = new Color(1f, .76f, .36f, 1f);
    private static readonly Color Coral = new Color(1f, .43f, .37f, 1f);
    private static readonly Color Indigo = new Color(.055f, .09f, .24f, .82f);
    public int LitLampCount => Mode == PresentationMode.Lanterns ? Mathf.Clamp(1 + (int)(age / .22f), 0, 9) : 0;
    public void SetAge(float value) { age = Mathf.Max(0f, value); SetVerticesDirty(); }

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        Rect area = rectTransform.rect;
        if (area.width <= 0 || area.height <= 0) return;
        if (Mode == PresentationMode.Dawn)
        {
            // Transparent center preserves the original studio artwork and all controls.
            GradientBand(mesh, area, .68f, 1f, Clear(Indigo), Indigo);
            GradientBand(mesh, area, 0f, .18f, new Color(.9f, .35f, .21f, .32f), Clear(Coral));
            Vector2 sun = Point(area, .16f, .825f);
            for (int i = 0; i < 4; i++)
                Arc(mesh, sun, 42f + i * 13f, 0f, Mathf.PI, 2f, WithAlpha(Gold, .5f - i * .09f));
            Stroke(mesh, Point(area, .045f, .825f), Point(area, .275f, .825f), 1.5f, WithAlpha(Gold, .5f));
            for (int i = 0; i < 6; i++)
            {
                float cycle = Mathf.Repeat(age * .024f + i * .163f, 1f);
                float x = i % 2 == 0 ? .035f + (i / 2) * .033f : .965f - (i / 2) * .033f;
                Balloon(mesh, Point(area, x, .14f + cycle * .65f), 12f + i % 3 * 3f,
                    WithAlpha(i % 2 == 0 ? Coral : Gold, Mathf.Sin(cycle * Mathf.PI) * .65f));
            }
            // A slow sunray stays in the title band rather than crossing instructions.
            float sweep = .32f + Mathf.PingPong(age * .018f, .36f);
            Quad(mesh, Point(area, sweep, .7f), Point(area, sweep + .08f, .7f),
                Point(area, sweep + .28f, 1f), Point(area, sweep + .2f, 1f),
                new Color(1f, .87f, .55f, .055f));
        }
        else if (Mode == PresentationMode.Lanterns)
        {
            GradientBand(mesh, area, 0f, .18f, new Color(.1f, .055f, .14f, .72f), Clear(Indigo));
            for (int i = 0; i < 9; i++)
            {
                float reveal = Mathf.Clamp01((age - i * .22f) * 2f);
                Vector2 center = Point(area, .1f + i * .1f, .055f);
                Arc(mesh, center, 13f, 0, Mathf.PI * 2f, 2f, WithAlpha(Gold, .15f + reveal * .55f));
                Disc(mesh, center, 19f, WithAlpha(Coral, reveal * .1f));
                Disc(mesh, center, 7f, WithAlpha(Gold, reveal * .8f));
                Disc(mesh, center + Vector2.up * 2, 3f, WithAlpha(Color.white, reveal * .9f));
            }
            for (int i = 0; i < 4; i++)
            {
                float travel = Mathf.Clamp01((age - .6f - i * .35f) / 7f);
                float opacity = Mathf.Sin(travel * Mathf.PI) * .85f;
                float x = i < 2 ? .07f + i * .09f : .84f + (i - 2) * .09f;
                Balloon(mesh, Point(area, x + Mathf.Sin(travel * 4f + i) * .008f, .12f + travel * .7f),
                    23f, WithAlpha(i % 2 == 0 ? Gold : Coral, opacity));
            }
        }
        else
        {
            Vector2 center = Point(area, .5f, .54f);
            float radius = Mathf.Min(area.width * .27f, area.height * .22f);
            Vector2 first = Vector2.zero, previous = Vector2.zero;
            for (int i = 0; i < 9; i++)
            {
                float angle = Mathf.PI * .5f - i * Mathf.PI * 2f / 9f;
                Vector2 position = center + new Vector2(Mathf.Cos(angle) * radius * 1.35f, Mathf.Sin(angle) * radius * .7f);
                Color tint = Color.Lerp(Coral, Gold, i / 8f);
                float reveal = Mathf.Clamp01(age * 1.5f - i * .12f);
                if (i > 0) Stroke(mesh, previous, position, 2f, WithAlpha(tint, reveal * .5f));
                else first = position;
                Balloon(mesh, position, 16f, WithAlpha(tint, reveal));
                previous = position;
            }
            Stroke(mesh, previous, first, 2f, WithAlpha(Gold, Mathf.Clamp01(age - .8f) * .5f));
            Arc(mesh, center - Vector2.up * 10f, 35f, 0, Mathf.PI, 3f, Gold);
            Stroke(mesh, center + new Vector2(-60, -10), center + new Vector2(60, -10), 2f, Coral);
        }
    }

    private static Vector2 Point(Rect area, float x, float y) => new Vector2(area.xMin + area.width * x, area.yMin + area.height * y);
    private static Color Clear(Color color) => WithAlpha(color, 0f);
    private static Color WithAlpha(Color color, float alpha) { color.a = alpha; return color; }
    private static void Balloon(VertexHelper mesh, Vector2 center, float radius, Color tint)
    {
        Disc(mesh, center, radius * 1.7f, WithAlpha(tint, tint.a * .07f));
        Arc(mesh, center, radius, 0, Mathf.PI * 2f, 2.5f, tint);
        Arc(mesh, center + new Vector2(-radius * .2f, radius * .1f), radius * .55f, Mathf.PI * .45f, Mathf.PI * .85f, 2f, tint);
        Stroke(mesh, center - Vector2.up * radius, center - Vector2.up * (radius + 20f), 1.2f, tint);
    }
    private static void GradientBand(VertexHelper mesh, Rect area, float bottom, float top, Color lower, Color upper)
    {
        int at = mesh.currentVertCount;
        mesh.AddVert(Point(area, 0, bottom), lower, Vector2.zero); mesh.AddVert(Point(area, 1, bottom), lower, Vector2.zero);
        mesh.AddVert(Point(area, 1, top), upper, Vector2.zero); mesh.AddVert(Point(area, 0, top), upper, Vector2.zero);
        mesh.AddTriangle(at, at + 1, at + 2); mesh.AddTriangle(at, at + 2, at + 3);
    }
    private static void Disc(VertexHelper mesh, Vector2 center, float radius, Color tint)
    {
        for (int i = 0; i < 32; i++)
        {
            float a = i * Mathf.PI * 2f / 32f, b = (i + 1) * Mathf.PI * 2f / 32f;
            int at = mesh.currentVertCount;
            mesh.AddVert(center, tint, Vector2.zero);
            mesh.AddVert(center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius, Clear(tint), Vector2.zero);
            mesh.AddVert(center + new Vector2(Mathf.Cos(b), Mathf.Sin(b)) * radius, Clear(tint), Vector2.zero);
            mesh.AddTriangle(at, at + 1, at + 2);
        }
    }
    private static void Arc(VertexHelper mesh, Vector2 center, float radius, float from, float to, float width, Color tint)
    {
        for (int i = 0; i < 48; i++)
        {
            float a = Mathf.Lerp(from, to, i / 48f), b = Mathf.Lerp(from, to, (i + 1) / 48f);
            Stroke(mesh, center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius,
                center + new Vector2(Mathf.Cos(b), Mathf.Sin(b)) * radius, width, tint);
        }
    }
    private static void Stroke(VertexHelper mesh, Vector2 a, Vector2 b, float width, Color tint)
    {
        Vector2 direction = (b - a).normalized;
        Vector2 side = new Vector2(-direction.y, direction.x) * width * .5f;
        Quad(mesh, a - side, a + side, b + side, b - side, tint);
    }
    private static void Quad(VertexHelper mesh, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color tint)
    {
        int at = mesh.currentVertCount;
        mesh.AddVert(a, tint, Vector2.zero); mesh.AddVert(b, tint, Vector2.zero);
        mesh.AddVert(c, tint, Vector2.zero); mesh.AddVert(d, tint, Vector2.zero);
        mesh.AddTriangle(at, at + 1, at + 2); mesh.AddTriangle(at, at + 2, at + 3);
    }
}
