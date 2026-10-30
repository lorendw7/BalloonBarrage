using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace BalloonBarrage.EditorTools
{
    /// <summary>Prepares unique, seam-padded paint UVs without changing original meshes or materials.</summary>
    public static class RoomPaintPreparation
    {
        private const string Root = "Assets/StudioArtPack/RoomPaint20261031";
        [MenuItem("BalloonBarrage/Art/Prepare Full Room Paint")]
        public static void Run()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/PrototypeScene.unity");
            Directory.CreateDirectory(Root + "/Meshes"); AssetDatabase.Refresh();
            MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(Eligible).OrderByDescending(r => TileSize(r)).ThenBy(r => PathFor(r.transform), StringComparer.Ordinal).ToArray();
            if (renderers.Length == 0) throw new InvalidOperationException("No static opaque room receivers.");
            int size = 2048;
            Dictionary<MeshRenderer, RectInt> tiles;
            while (!Pack(renderers, size, out tiles))
            { size *= 2; if (size > 4096) throw new InvalidOperationException("Room exceeds the explicit 4096 paint-atlas budget."); }
            Shader coatShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Art/PaintSystem/RoomPaintCoat.shader");
            Shader stampShader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/Art/PaintSystem/RoomPaintAccumulate.shader");
            if (coatShader == null || stampShader == null) throw new InvalidOperationException("Missing room paint shaders.");
            Material coat = AssetDatabase.LoadAssetAtPath<Material>(Root + "/RoomPaintCoat.mat");
            if (coat == null) { coat = new Material(coatShader); AssetDatabase.CreateAsset(coat, Root + "/RoomPaintCoat.mat"); }
            coat.shader = coatShader;
            coat.enableInstancing = true;
            var prepared = new List<PaintSurface>(renderers.Length);
            var coatMeshes = new List<CombineInstance>(renderers.Length);
            var records = new List<string>();
            AssetDatabase.StartAssetEditing();
            try
            {
            foreach (MeshRenderer renderer in renderers)
            {
                Mesh original = renderer.GetComponent<MeshFilter>().sharedMesh;
                RectInt tile = tiles[renderer];
                string key = Hash(PathFor(renderer.transform));
                BuildMeshes(original, tile, size, out Mesh visibleMesh, out Mesh stampMesh);
                stampMesh = SaveMesh(stampMesh, Root + "/Meshes/" + key + "-stamp.asset");
                Transform child = renderer.transform.Find("__RoomPaintCoat");
                if (child != null) Object.DestroyImmediate(child.gameObject);
                coatMeshes.Add(new CombineInstance { mesh = visibleMesh, transform = renderer.transform.localToWorldMatrix });
                PaintSurface surface = renderer.GetComponent<PaintSurface>();
                if (surface == null) surface = renderer.gameObject.AddComponent<PaintSurface>();
                var serialized = new SerializedObject(surface);
                serialized.FindProperty("targetRenderer").objectReferenceValue = renderer;
                serialized.FindProperty("stampMesh").objectReferenceValue = stampMesh;
                serialized.FindProperty("atlasPixels").rectValue = new Rect(tile.x, tile.y, tile.width, tile.height);
                serialized.ApplyModifiedPropertiesWithoutUndo();
                // Paint must hit visible geometry, not a loose prop bounding box. Trigger contracts remain intact.
                MeshCollider contactCollider = renderer.GetComponents<MeshCollider>().FirstOrDefault(c => !c.isTrigger);
                if (contactCollider == null) contactCollider = renderer.gameObject.AddComponent<MeshCollider>();
                contactCollider.sharedMesh = original; contactCollider.convex = false; contactCollider.enabled = true;
                foreach (Collider other in renderer.GetComponents<Collider>())
                    if (other != contactCollider && !other.isTrigger) other.enabled = false;
                if (renderer.gameObject.layer == 2) renderer.gameObject.layer = 0;
                prepared.Add(surface);
                records.Add(JsonUtility.ToJson(new ReceiverRecord { path = PathFor(renderer.transform),
                    sourceMesh = AssetDatabase.GetAssetPath(original), coatMesh = Root + "/CombinedRoomCoat.asset",
                    stampMesh = AssetDatabase.GetAssetPath(stampMesh), tile = new Vector4(tile.x, tile.y, tile.width, tile.height),
                    sourceVertices = original.vertexCount, preparedVertices = visibleMesh.vertexCount }));
            }
            }
            finally { AssetDatabase.StopAssetEditing(); }
            RoomPaintSystem system = Object.FindFirstObjectByType<RoomPaintSystem>();
            if (system == null) system = new GameObject("PreparedRoomPaintSystem").AddComponent<RoomPaintSystem>();
            Mesh combined = new Mesh { name = "PreparedRoomPaintCoat", indexFormat = IndexFormat.UInt32 };
            combined.CombineMeshes(coatMeshes.ToArray(), true, true, false); combined.RecalculateBounds();
            WeldCoat(combined);
            combined = SaveMesh(combined, Root + "/CombinedRoomCoat.asset");
            foreach (CombineInstance entry in coatMeshes) Object.DestroyImmediate(entry.mesh);
            Transform combinedNode = system.transform.Find("__RoomPaintCoat");
            if (combinedNode == null) { combinedNode = new GameObject("__RoomPaintCoat", typeof(MeshFilter), typeof(MeshRenderer)).transform; combinedNode.SetParent(system.transform, false); }
            combinedNode.GetComponent<MeshFilter>().sharedMesh = combined;
            MeshRenderer overlay = combinedNode.GetComponent<MeshRenderer>(); overlay.sharedMaterial = coat;
            overlay.shadowCastingMode = ShadowCastingMode.Off; overlay.receiveShadows = false;
            overlay.lightProbeUsage = LightProbeUsage.Off; overlay.reflectionProbeUsage = ReflectionProbeUsage.Off;
            overlay.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            overlay.sortingOrder = 50; overlay.enabled = false;
            foreach (PaintSurface surface in prepared)
            {
                var mapping = new SerializedObject(surface);
                mapping.FindProperty("coatRenderer").objectReferenceValue = overlay;
                mapping.ApplyModifiedPropertiesWithoutUndo();
            }
            var settings = new SerializedObject(system);
            settings.FindProperty("accumulationShader").objectReferenceValue = stampShader;
            settings.FindProperty("atlasSize").intValue = size;
            SerializedProperty list = settings.FindProperty("surfaces"); list.arraySize = prepared.Count;
            for (int i = 0; i < prepared.Count; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = prepared[i];
            settings.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
            File.WriteAllText(Root + "/Receivers.json", "{\n  \"atlasSize\": " + size + ",\n  \"colorBytes\": " + ((long)size * size * 4) +
                ",\n  \"scope\": \"Prepared opaque static studio meshes. Original geometry/materials and paint artwork retained; generated coat/stamp UVs.\",\n  \"receivers\": [\n" + string.Join(",\n", records) + "\n]}\n");
            AssetDatabase.Refresh();
            Debug.Log("ROOM_PAINT_PREPARED: " + prepared.Count + " static receivers, " + size + " atlas; no permanent per-hit objects.");
        }

        [Serializable] private sealed class ReceiverRecord
        { public string path, sourceMesh, coatMesh, stampMesh; public Vector4 tile; public int sourceVertices, preparedVertices; }
        private static bool Eligible(MeshRenderer renderer)
        {
            if (!renderer.gameObject.activeInHierarchy || renderer.name == "__RoomPaintCoat" || renderer.GetComponentInParent<Camera>() != null ||
                renderer.GetComponentInParent<PlayerShooter>() != null || renderer.GetComponentInParent<Balloon>() != null ||
                renderer.GetComponentInParent<Rigidbody>() != null || renderer.GetComponentInParent<Animator>() != null ||
                renderer.GetComponentInParent<Canvas>() != null) return false;
            MeshFilter filter = renderer.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null || filter.sharedMesh.vertexCount < 3 || !renderer.enabled) return false;
            if (renderer.sharedMaterials.Length == 0) return false;
            return renderer.sharedMaterials.All(m => m != null && m.renderQueue <= 2500 &&
                (!m.HasProperty("_Surface") || m.GetFloat("_Surface") < .5f) &&
                (!m.HasProperty("_AlphaClip") || m.GetFloat("_AlphaClip") < .5f) &&
                !m.IsKeywordEnabled("_ALPHATEST_ON"));
        }
        private static int TileSize(Renderer renderer)
        {
            if (renderer.name == "MainWall" || renderer.name == "Floor") return 1024;
            float length = renderer.bounds.size.magnitude;
            return length > 8f ? 512 : length > 3f ? 256 : length > 1f ? 128 : 64;
        }
        private static bool Pack(MeshRenderer[] renderers, int size, out Dictionary<MeshRenderer, RectInt> result)
        {
            result = new Dictionary<MeshRenderer, RectInt>(); int x = 0, y = 0, rowHeight = 0;
            foreach (MeshRenderer renderer in renderers)
            {
                int tile = TileSize(renderer);
                if (x + tile > size) { x = 0; y += rowHeight; rowHeight = 0; }
                if (y + tile > size) return false;
                result.Add(renderer, new RectInt(x, y, tile, tile)); x += tile; rowHeight = Mathf.Max(rowHeight, tile);
            }
            return true;
        }
        private static Mesh SaveMesh(Mesh mesh, string path)
        {
            Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing == null) { AssetDatabase.CreateAsset(mesh, path); return mesh; }
            EditorUtility.CopySerialized(mesh, existing); Object.DestroyImmediate(mesh); return existing;
        }
        private readonly struct CoatVertex : IEquatable<CoatVertex>
        {
            public readonly Vector3 position, normal; public readonly Vector2 uv;
            public CoatVertex(Vector3 p, Vector3 n, Vector2 u) { position=p; normal=n; uv=u; }
            public bool Equals(CoatVertex other) => position.Equals(other.position) && normal.Equals(other.normal) && uv.Equals(other.uv);
            public override bool Equals(object value) => value is CoatVertex other && Equals(other);
            public override int GetHashCode() => unchecked((position.GetHashCode()*397 ^ normal.GetHashCode())*397 ^ uv.GetHashCode());
        }
        private static void WeldCoat(Mesh mesh)
        {
            Vector3[] points=mesh.vertices, normals=mesh.normals; Vector2[] uv=mesh.uv4; int[] indices=mesh.triangles;
            var lookup=new Dictionary<CoatVertex,int>(points.Length);
            var keptPoints=new List<Vector3>(); var keptNormals=new List<Vector3>(); var keptUVs=new List<Vector2>();
            var remap=new int[points.Length];
            for (int i=0;i<points.Length;i++)
            {
                var key=new CoatVertex(points[i],normals[i],uv[i]);
                if (!lookup.TryGetValue(key,out int at)) { at=keptPoints.Count; lookup.Add(key,at); keptPoints.Add(points[i]); keptNormals.Add(normals[i]); keptUVs.Add(uv[i]); }
                remap[i]=at;
            }
            for (int i=0;i<indices.Length;i++) indices[i]=remap[indices[i]];
            mesh.Clear(); mesh.indexFormat=IndexFormat.UInt32; mesh.SetVertices(keptPoints); mesh.SetNormals(keptNormals);
            mesh.SetUVs(3,keptUVs); mesh.triangles=indices; mesh.RecalculateBounds(); mesh.Optimize();
        }
        private static string PathFor(Transform node) => node.parent == null ? node.name + "[" + node.GetSiblingIndex() + "]"
            : PathFor(node.parent) + "/" + node.name + "[" + node.GetSiblingIndex() + "]";
        private static string Hash(string value)
        { using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").Substring(0, 16).ToLowerInvariant(); }

        private static void BuildMeshes(Mesh source, RectInt tile, int atlas, out Mesh coat, out Mesh stamp)
        {
            UnwrapParam.SetDefaults(out UnwrapParam unwrap); unwrap.packMargin = 4f / tile.width;
            int[] indices = source.triangles;
            Vector2[] unwrapped = Unwrapping.GeneratePerTriangleUV(source, unwrap);
            if (unwrapped.Length != indices.Length) throw new InvalidOperationException("Unexpected generated UV count: " + source.name);
            Vector3[] sourcePositions = source.vertices, sourceNormals = source.normals;
            var positions = new Vector3[indices.Length]; var normals = new Vector3[indices.Length];
            var paintUVs = new Vector2[indices.Length]; var paddedPositions = new Vector3[indices.Length]; var paddedUVs = new Vector2[indices.Length];
            var triangles = new int[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                triangles[i] = i; positions[i] = sourcePositions[indices[i]];
                normals[i] = sourceNormals.Length == sourcePositions.Length ? sourceNormals[indices[i]] : Vector3.up;
                paintUVs[i] = new Vector2((tile.x + 2f + unwrapped[i].x * (tile.width - 4f)) / atlas,
                    (tile.y + 2f + unwrapped[i].y * (tile.height - 4f)) / atlas);
            }
            // Expand only the accumulation geometry, preserving its world/UV affine map. The visible
            // geometry stays exact; a one-texel gutter prevents bilinear cracks along UV seams.
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector2 centerUV = (paintUVs[i] + paintUVs[i+1] + paintUVs[i+2]) / 3f;
                Vector3 centerWorld = (positions[i] + positions[i+1] + positions[i+2]) / 3f;
                float area = Mathf.Abs((paintUVs[i+1].x-paintUVs[i].x)*(paintUVs[i+2].y-paintUVs[i].y) -
                    (paintUVs[i+1].y-paintUVs[i].y)*(paintUVs[i+2].x-paintUVs[i].x));
                float edge = Mathf.Max((paintUVs[i+1]-paintUVs[i]).magnitude,
                    Mathf.Max((paintUVs[i+2]-paintUVs[i+1]).magnitude, (paintUVs[i]-paintUVs[i+2]).magnitude));
                float expansion = 1f + 3.6f / atlas / Mathf.Max(area / Mathf.Max(edge, .000001f), .000001f);
                for (int j = 0; j < 3; j++)
                { paddedUVs[i+j] = centerUV + (paintUVs[i+j]-centerUV)*expansion; paddedPositions[i+j] = centerWorld + (positions[i+j]-centerWorld)*expansion; }
            }
            coat = new Mesh { name = source.name + "-paint-coat", indexFormat = IndexFormat.UInt32 };
            coat.vertices = positions; coat.normals = normals; coat.SetUVs(3, paintUVs); coat.triangles = triangles; coat.RecalculateBounds();
            stamp = new Mesh { name = source.name + "-paint-stamp", indexFormat = IndexFormat.UInt32 };
            stamp.vertices = paddedPositions; stamp.normals = normals; stamp.SetUVs(3, paddedUVs); stamp.triangles = triangles; stamp.RecalculateBounds();
        }
    }
}
