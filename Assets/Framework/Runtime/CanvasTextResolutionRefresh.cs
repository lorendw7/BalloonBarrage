using UnityEngine;
using UnityEngine.UI;

namespace BalloonBarrage.Foundation
{
    // Presentation only: refresh rasterized text after a canvas resolution/scale change.
    [RequireComponent(typeof(Canvas))]
    [DefaultExecutionOrder(100)]
    public sealed class CanvasTextResolutionRefresh : MonoBehaviour
    {
        private Canvas canvas;
        private Vector2 previousSize;
        private float previousScale = -1f;

        private void OnEnable()
        {
            canvas = GetComponent<Canvas>();
            previousSize = Vector2.zero;
            previousScale = -1f;
        }

        private void LateUpdate()
        {
            Vector2 size = canvas.pixelRect.size;
            float scale = canvas.scaleFactor;
            if (size == previousSize && Mathf.Approximately(scale, previousScale)) return;
            previousSize = size;
            previousScale = scale;
            foreach (var text in GetComponentsInChildren<Text>(false))
            {
                if (text.font != null && text.font.dynamic)
                    text.font.RequestCharactersInTexture(text.text,
                        Mathf.Max(1, Mathf.RoundToInt(text.fontSize * text.pixelsPerUnit)), text.fontStyle);
                text.SetAllDirty();
            }
        }
    }
}
