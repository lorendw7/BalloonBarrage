using UnityEngine;
using UnityEngine.UI;

/// <summary>Bounded UI mesh: hollow target rings, shaded curves and directional light trails.</summary>
public sealed class RhythmSequenceGraphic : MaskableGraphic
{
    public static readonly Color FocusColor = new Color(1f, .86f, .32f, 1f);
    public static readonly Color FollowColor = new Color(.58f, 1f, .91f, .9f);
    private static readonly Color Shade = new Color(.025f, .065f, .08f, .85f);
    private readonly Vector2[] centers = new Vector2[3];
    private readonly float[] sizes = new float[3];
    private int count;
    private float approach, flow;
    private const int RingSegments = 64, CurveSegments = 32;
    public int VisibleLinkCount { get; private set; }
    private Vector2 celebrationCenter;
    private float celebration;
    public void SetCelebration(Vector2 center, float remaining)
    { celebrationCenter = center; celebration = Mathf.Clamp01(remaining); SetVerticesDirty(); }

    public void SetFrame(Vector2[] positions, float[] radii, int visible, float progress, float phase)
    {
        count = Mathf.Clamp(visible, 0, centers.Length);
        for (int i = 0; i < count; i++) { centers[i] = positions[i]; sizes[i] = radii[i]; }
        approach = progress; flow = phase;
        VisibleLinkCount = 0;
        for (int i = 0; i < count - 1; i++)
            if (Vector2.Distance(centers[i], centers[i + 1]) > sizes[i] * (i == 0 ? 1.55f : 1f) + sizes[i + 1] + 48f)
                VisibleLinkCount++;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear();
        // Links terminate outside the larger approach circle, leaving balloon centers clear.
        for (int i = 0; i < count - 1; i++) DrawLink(mesh, i);
        for (int i = 0; i < count; i++)
        {
            Color tint = i == 0 ? FocusColor : FollowColor;
            Ring(mesh, centers[i], sizes[i], 9f, Shade);
            Ring(mesh, centers[i], sizes[i], 17f, Alpha(tint, i == 0 ? .12f : .05f));
            Ring(mesh, centers[i], sizes[i], i == 0 ? 3.5f : 2f, Alpha(tint, i == 0 ? 1f : .6f));
            if (i != 0) continue;
            float radius = sizes[i] * Mathf.Lerp(1.55f, 1f, approach);
            Ring(mesh, centers[i], radius, 7f, Alpha(Shade, .65f));
            Ring(mesh, centers[i], radius, 2f, Alpha(tint, .8f));
        }
        if (celebration > 0f)
        {
            float radius = Mathf.Lerp(150f, 45f, celebration);
            Ring(mesh, celebrationCenter, radius, 3f, Alpha(FocusColor, celebration * .7f));
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Stroke(mesh, celebrationCenter + direction * (radius + 8f),
                    celebrationCenter + direction * (radius + 20f), 3.5f, Alpha(FocusColor, celebration));
            }
        }
    }

    private void DrawLink(VertexHelper mesh, int index)
    {
        Vector2 delta = centers[index + 1] - centers[index];
        float distance = delta.magnitude;
        float startRadius = sizes[index] * (index == 0 ? 1.55f : 1f) + 12f;
        float endRadius = sizes[index + 1] + 12f;
        if (distance <= startRadius + endRadius + 24f) return;
        Vector2 direction = delta / distance;
        Vector2 start = centers[index] + direction * startRadius;
        Vector2 end = centers[index + 1] - direction * endRadius;
        Vector2 control = (start + end) * .5f + Vector2.up * Mathf.Min(45f, distance * .12f);
        Color tint = index == 0 ? FocusColor : FollowColor;
        for (int s = 0; s < CurveSegments; s++)
        {
            float t = s / (float)CurveSegments, next = (s + 1f) / CurveSegments;
            Vector2 a = Curve(start, control, end, t), b = Curve(start, control, end, next);
            Stroke(mesh, a + Vector2.down * 2f, b + Vector2.down * 2f, 9f, Shade);
            Stroke(mesh, a, b, 15f, Alpha(tint, .08f));
            Stroke(mesh, a, b, 2.5f, Alpha(tint, index == 0 ? .65f : .3f));
            float trail = Mathf.Clamp01(1f - Mathf.Abs(next - flow) / .14f);
            if (trail > 0f) Stroke(mesh, a, b, 5f, Alpha(tint, trail * .95f));
        }
        // A fixed arrowhead makes the order readable even without motion or color.
        Vector2 tip = Curve(start, control, end, .72f);
        Vector2 tangent = (Curve(start, control, end, .74f) - tip).normalized;
        Vector2 normal = new Vector2(-tangent.y, tangent.x);
        Stroke(mesh, tip - tangent * 12f + normal * 6f, tip, 4f, tint);
        Stroke(mesh, tip - tangent * 12f - normal * 6f, tip, 4f, tint);
    }

    private static Vector2 Curve(Vector2 a, Vector2 b, Vector2 c, float t)
        => (1f - t) * (1f - t) * a + 2f * (1f - t) * t * b + t * t * c;
    private static Color Alpha(Color tint, float alpha) { tint.a = alpha; return tint; }

    private static void Ring(VertexHelper mesh, Vector2 center, float radius, float width, Color tint)
    {
        for (int i = 0; i < RingSegments; i++)
        {
            float a = i * (Mathf.PI * 2f / RingSegments), b = (i + 1) * (Mathf.PI * 2f / RingSegments);
            Vector2 u = new Vector2(Mathf.Cos(a), Mathf.Sin(a)), v = new Vector2(Mathf.Cos(b), Mathf.Sin(b));
            Quad(mesh, center + u * (radius - width * .5f), center + u * (radius + width * .5f),
                center + v * (radius + width * .5f), center + v * (radius - width * .5f), tint);
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
