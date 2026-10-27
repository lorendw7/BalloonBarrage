using UnityEngine;

[DisallowMultipleComponent]
public class PaintSurface : MonoBehaviour
{
    [SerializeField]
    private Renderer targetRenderer;

    public Renderer TargetRenderer => targetRenderer;

    private void Reset()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    public static bool CanReceivePaint(Collider collider)
    {
        if (collider == null)
        {
            return false;
        }

        PaintSurface paintSurface =
            collider.GetComponentInParent<PaintSurface>();

        if (paintSurface != null)
        {
            return true;
        }

        string surfaceName = collider.name;

        return surfaceName == "MainWall" ||
            surfaceName == "Floor" ||
            surfaceName == "LeftWall" ||
            surfaceName == "RightWall";
    }


    public bool TryGetPaintUV(Vector3 worldPoint, out Vector2 uv)
    {
        uv = default;
        
        if (targetRenderer == null)
        {
            return false;
        }

        Bounds bounds = targetRenderer.localBounds;

        if (bounds.size.x <= Mathf.Epsilon ||
            bounds.size.y <= Mathf.Epsilon)
        {
            return false;
        }

        Vector3 localPoint = targetRenderer.transform.InverseTransformPoint(worldPoint);

        bool isInside =
        localPoint.x >= bounds.min.x &&
        localPoint.x <= bounds.max.x &&
        localPoint.y >= bounds.min.y &&
        localPoint.y <= bounds.max.y;

        uv = new Vector2(
            Mathf.InverseLerp(
                bounds.min.x,
                bounds.max.x,
                localPoint.x),
            Mathf.InverseLerp(
                bounds.min.y,
                bounds.max.y,
                localPoint.y));

        return isInside;
    }
}
