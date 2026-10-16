using System.Collections.Generic;
using UnityEngine;

/// <summary>Native mesh authoring for the concept pass; no imported source meshes are edited.</summary>
public static class StudioConceptGeometry
{
    public static Mesh RoundedBox(Vector3 size, float radius)
    {
        var vertices = new List<Vector3>(); var normals = new List<Vector3>(); var uvs = new List<Vector2>(); var indices = new List<int>();
        var half = size * .5f; radius = Mathf.Min(radius, Mathf.Min(half.x, Mathf.Min(half.y, half.z)) * .95f); var core = half - Vector3.one * radius;
        float[] Steps(float h) => new[] { -h, -h + radius * .293f, -h + radius, h - radius, h - radius * .293f, h };
        foreach (var normal in new[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back })
        {
            var u = Mathf.Abs(normal.y) > .5f ? Vector3.right : Vector3.up; var v = Vector3.Cross(normal, u);
            float hu = Mathf.Abs(Vector3.Dot(half, u)), hv = Mathf.Abs(Vector3.Dot(half, v)), hn = Mathf.Abs(Vector3.Dot(half, normal));
            var xs = Steps(hu); var ys = Steps(hv); int start = vertices.Count;
            for (int y = 0; y < ys.Length; y++) for (int x = 0; x < xs.Length; x++)
            {
                var p = normal * hn + u * xs[x] + v * ys[y];
                var closest = new Vector3(Mathf.Clamp(p.x, -core.x, core.x), Mathf.Clamp(p.y, -core.y, core.y), Mathf.Clamp(p.z, -core.z, core.z));
                var n = (p - closest).normalized; vertices.Add(closest + n * radius); normals.Add(n); uvs.Add(new Vector2(xs[x] + hu, ys[y] + hv));
            }
            for (int y = 0; y < ys.Length - 1; y++) for (int x = 0; x < xs.Length - 1; x++)
            { int i = start + y * xs.Length + x; indices.AddRange(new[] { i, i + 1, i + xs.Length, i + 1, i + xs.Length + 1, i + xs.Length }); }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetUVs(0, uvs); mesh.SetTriangles(indices, 0); mesh.RecalculateTangents(); mesh.RecalculateBounds(); return mesh;
    }

    public static Mesh Lathe(Vector2[] profile)
    {
        const int sides = 64;
        var vertices = new List<Vector3>(); var normals = new List<Vector3>(); var uvs = new List<Vector2>(); var indices = new List<int>();
        for (int i = 0; i <= sides; i++) for (int j = 0; j < profile.Length; j++)
        {
            float angle = i * Mathf.PI * 2 / sides; var p = profile[j]; var d = profile[Mathf.Min(j + 1, profile.Length - 1)] - profile[Mathf.Max(j - 1, 0)];
            vertices.Add(new Vector3(p.x * Mathf.Cos(angle), p.y, p.x * Mathf.Sin(angle))); normals.Add(new Vector3(d.y * Mathf.Cos(angle), -d.x, d.y * Mathf.Sin(angle)).normalized); uvs.Add(new Vector2(i / (float)sides, j / (float)(profile.Length - 1)));
            if (i < sides && j < profile.Length - 1) { int a = i * profile.Length + j; indices.AddRange(new[] { a, a + 1, a + profile.Length, a + 1, a + profile.Length + 1, a + profile.Length }); }
        }
        var mesh = new Mesh(); mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetUVs(0, uvs); mesh.SetTriangles(indices, 0); mesh.RecalculateTangents(); mesh.RecalculateBounds(); return mesh;
    }
}
