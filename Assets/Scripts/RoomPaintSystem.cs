using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>Accumulates accepted contacts into prepared mesh UVs. No permanent object per hit.</summary>
[DefaultExecutionOrder(500)]
[DisallowMultipleComponent]
public sealed class RoomPaintSystem : MonoBehaviour
{
    [SerializeField] private Shader accumulationShader;
    [SerializeField] private int atlasSize = 2048;
    [SerializeField] private PaintSurface[] surfaces = new PaintSurface[0];
    private static RoomPaintSystem current;
    private readonly List<Stamp> pending = new List<Stamp>(256);
    private readonly Dictionary<int, Stroke> strokes = new Dictionary<int, Stroke>(2);
    private MaterialPropertyBlock properties;
    private Material brush;
    private CommandBuffer commands;
    public RenderTexture Atlas { get; private set; }
    public int SurfaceCount => surfaces.Length;
    public long SubmittedStampCount { get; private set; }
    public int FlushCount { get; private set; }
    public int PendingCount => pending.Count;
    public long ColorStorageBytes => (long)atlasSize * atlasSize * 4;
    private struct Stamp
    {
        public PaintSurface surface;
        public Vector3 point, normal;
        public Color color;
        public Sprite mask;
        public float radius, rotation;
    }
    private struct Stroke
    {
        public PaintSurface surface;
        public Vector3 point, normal;
        public Color color;
        public float time;
    }

    private void Awake()
    {
        if (accumulationShader == null || !accumulationShader.isSupported || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
        { enabled = false; Debug.LogError("Prepared room paint requires its accumulation shader and render textures."); return; }
        current = this;
        properties = new MaterialPropertyBlock();
        Atlas = new RenderTexture(atlasSize, atlasSize, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
        { name = "RoundPaintAtlas", useMipMap = false, autoGenerateMips = false,
            filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.DontSave };
        brush = new Material(accumulationShader) { hideFlags = HideFlags.HideAndDontSave };
        commands = new CommandBuffer { name = "Batched room paint contacts" };
        Shader.SetGlobalTexture("_RoomPaintAtlas", Atlas);
        Shader.SetGlobalVector("_RoomPaintAtlas_TexelSize", new Vector4(1f / atlasSize, 1f / atlasSize, atlasSize, atlasSize));
        Shader.SetGlobalFloat("_RoomPaintActive", 1f);
        Clear();
        Debug.Log("ROOM_PAINT_READY: " + surfaces.Length + " receivers; " + atlasSize + " atlas descriptor.");
    }

    private System.Collections.IEnumerator Start()
    {
        // Let the pipeline finish its initial render-target setup before reserving storage.
        yield return null; yield return null;
        EnsureStorage();
    }
    private void EnsureStorage()
    {
        if (Atlas == null || Atlas.IsCreated()) return;
        Atlas.Create();
        RenderTexture previous = RenderTexture.active; RenderTexture.active = Atlas;
        GL.Clear(false, true, Color.clear); RenderTexture.active = previous;
    }

    public static void ClearForNewRound() { if (current != null) current.Clear(); }
    public static void EndStroke(int id) { if (current != null) current.strokes.Remove(id); }
    public void Clear()
    {
        pending.Clear(); strokes.Clear(); SubmittedStampCount = 0; FlushCount = 0;
        foreach (PaintSurface surface in surfaces)
            if (surface != null && surface.CoatRenderer != null) surface.CoatRenderer.enabled = false;
        if (Atlas == null || !Atlas.IsCreated()) return;
        RenderTexture before = RenderTexture.active;
        RenderTexture.active = Atlas; GL.Clear(false, true, Color.clear); RenderTexture.active = before;
    }

    public static bool TryPaint(RaycastHit hit, Color color, Sprite mask, float radius, int strokeId = 0)
    {
        if (current == null || !current.isActiveAndEnabled || mask == null || hit.collider == null) return false;
        PaintSurface surface = hit.collider.GetComponentInParent<PaintSurface>();
        if (surface == null || !surface.HasPreparedPaint) return false;
        current.EnsureStorage();
        current.Paint(surface, hit.point, hit.normal, color, mask, radius, strokeId);
        return true;
    }

    private void Paint(PaintSurface surface, Vector3 point, Vector3 normal, Color color, Sprite mask, float radius, int strokeId)
    {
        radius = Mathf.Clamp(radius, .06f, 1.5f);
        // Interpolation adds pigment, never authoritative hits, score, or new targets.
        if (strokeId != 0 && strokes.TryGetValue(strokeId, out Stroke previous) && previous.surface == surface &&
            Time.unscaledTime - previous.time <= .20f && previous.color == color && Vector3.Dot(previous.normal, normal) > .65f)
        {
            float distance = Vector3.Distance(previous.point, point);
            if (distance <= 3f)
            {
                int steps = Mathf.CeilToInt(distance / (radius * .55f));
                for (int i = 1; i < steps; i++)
                    QueueContact(surface, Vector3.Lerp(previous.point, point, (float)i / steps), normal, color, mask, radius);
            }
        }
        QueueContact(surface, point, normal, color, mask, radius);
        if (strokeId != 0) strokes[strokeId] = new Stroke { surface = surface, point = point, normal = normal, color = color, time = Time.unscaledTime };
    }

    private void QueueContact(PaintSurface primary, Vector3 point, Vector3 normal, Color color, Sprite mask, float radius)
    {
        Queue(primary, point, normal, color, mask, radius);
        // A contact near a corner can reach an adjacent prepared surface. Geometry, visibility
        // and the fragment's world distance/normal reject floating and through-wall pigment.
        foreach (PaintSurface neighbor in surfaces)
        {
            if (neighbor == null || neighbor == primary || !neighbor.HasPreparedPaint ||
                neighbor.TargetRenderer.bounds.SqrDistance(point) > radius * radius) continue;
            Vector3 closest = neighbor.TargetRenderer.bounds.ClosestPoint(point);
            Vector3 origin = point + normal * .035f;
            Vector3 delta = closest - origin;
            if (delta.sqrMagnitude < .000001f) continue;
            if (Physics.Raycast(origin, delta.normalized, out RaycastHit contact, delta.magnitude + .04f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) &&
                contact.collider.GetComponentInParent<PaintSurface>() == neighbor)
                Queue(neighbor, point, normal, color, mask, radius);
        }
    }

    private void Queue(PaintSurface surface, Vector3 point, Vector3 normal, Color color, Sprite mask, float radius)
    {
        // Flush a transient batch rather than losing paint if a diagnostic supplies a burst.
        if (pending.Count >= 4096) Flush();
        pending.Add(new Stamp { surface = surface, point = point, normal = normal.normalized,
            color = color, mask = mask, radius = radius, rotation = (SubmittedStampCount % 11) * .41f });
        SubmittedStampCount++;
    }

    private void LateUpdate() { if (pending.Count != 0) Flush(); }
    public void Flush()
    {
        if (pending.Count == 0 || Atlas == null) return;
        EnsureStorage();
        commands.Clear();
        commands.SetRenderTarget(Atlas, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
        commands.SetViewport(new Rect(0, 0, atlasSize, atlasSize));
        foreach (Stamp stamp in pending)
        {
            if (stamp.surface == null || !stamp.surface.HasPreparedPaint) continue;
            if (stamp.surface.CoatRenderer != null) stamp.surface.CoatRenderer.enabled = true;
            Mesh mesh = stamp.surface.StampMesh;
            properties.Clear();
            Matrix4x4 matrix = stamp.surface.TargetRenderer.transform.localToWorldMatrix;
            properties.SetMatrix("_PaintWorldFromLocal", matrix);
            properties.SetMatrix("_PaintNormalToWorld", matrix.inverse.transpose);
            properties.SetVector("_PaintCenterRadius", new Vector4(stamp.point.x, stamp.point.y, stamp.point.z, stamp.radius));
            properties.SetVector("_PaintContactNormal", stamp.normal);
            properties.SetColor("_PaintPigment", stamp.color);
            properties.SetFloat("_PaintRotation", stamp.rotation);
            properties.SetTexture("_PaintMask", stamp.mask.texture);
            Rect source = stamp.mask.textureRect;
            properties.SetVector("_PaintMaskRect", new Vector4(source.x / stamp.mask.texture.width, source.y / stamp.mask.texture.height,
                source.width / stamp.mask.texture.width, source.height / stamp.mask.texture.height));
            commands.EnableScissorRect(stamp.surface.AtlasPixels);
            for (int submesh = 0; submesh < mesh.subMeshCount; submesh++)
                commands.DrawMesh(mesh, Matrix4x4.identity, brush, submesh, 0, properties);
        }
        commands.DisableScissorRect();
        RenderTexture previous = RenderTexture.active;
        Graphics.ExecuteCommandBuffer(commands);
        RenderTexture.active = previous;
        pending.Clear(); FlushCount++;
    }

    private void OnDestroy()
    {
        if (current == this) { current = null; Shader.SetGlobalFloat("_RoomPaintActive", 0f); }
        pending.Clear(); strokes.Clear(); commands?.Release();
        if (brush != null) Destroy(brush);
        if (Atlas != null) { Atlas.Release(); Destroy(Atlas); }
    }
}
