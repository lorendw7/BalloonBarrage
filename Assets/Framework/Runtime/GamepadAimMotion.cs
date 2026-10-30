using UnityEngine;

namespace BalloonBarrage.Foundation
{
    /// <summary>Resolution-independent, immediate-stop stick response. Consumes unprocessed stick values.</summary>
    public static class GamepadAimMotion
    {
        public const float Deadzone = .16f;
        public static Vector2 ViewportDelta(Vector2 raw, float seconds, float aspect, float precisionScale = 1f)
        {
            float length = raw.magnitude;
            if (length <= Deadzone || seconds <= 0f || aspect <= 0f) return Vector2.zero;
            float strength = Mathf.Clamp01((length - Deadzone) / (1f - Deadzone));
            float speed = (.12f * strength + .88f * Mathf.Pow(strength, 1.8f)) * 1.15f;
            Vector2 delta = raw / length * (speed * Mathf.Min(seconds, .05f) * Mathf.Clamp(precisionScale, .35f, 1f));
            return new Vector2(delta.x / aspect, delta.y);
        }
    }
}
