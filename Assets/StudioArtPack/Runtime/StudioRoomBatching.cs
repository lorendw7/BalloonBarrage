using System.Collections.Generic;
using UnityEngine;

// One-time presentation preparation for an explicit list of immovable room meshes.
// Does not discover, move, hide or batch targets, weapons, UI or gameplay objects.
public sealed class StudioRoomBatching : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] surfaces = new MeshRenderer[0];
    private bool prepared;
    public int BatchedRendererCount { get; private set; }

    private void Start() => PrepareOnce();

    public void PrepareOnce()
    {
        if (prepared) return;
        prepared = true;
        var objects = new List<GameObject>(surfaces.Length);
        foreach (var surface in surfaces)
        {
            if (surface == null || !surface.enabled || !surface.gameObject.activeInHierarchy ||
                surface.isPartOfStaticBatch || surface.HasPropertyBlock() ||
                surface.GetComponentInParent<Rigidbody>() != null) continue;
            var filter = surface.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null || !filter.sharedMesh.isReadable ||
                filter.sharedMesh.vertexCount == 0) continue;
            bool compatible = true;
            foreach (var material in surface.sharedMaterials)
                if (material == null || material.renderQueue > 2500 ||
                    material.GetTag("DisableBatching", false, "False") == "True") compatible = false;
            if (compatible) objects.Add(surface.gameObject);
        }
        if (objects.Count < 2) return;
        StaticBatchingUtility.Combine(objects.ToArray(), gameObject);
        foreach (var surface in surfaces)
            if (surface != null && surface.isPartOfStaticBatch) BatchedRendererCount++;
    }
}
