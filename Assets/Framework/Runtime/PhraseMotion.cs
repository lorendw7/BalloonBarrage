using UnityEngine;

namespace BalloonBarrage.Foundation
{
    /// <summary>Small, periodic offsets; the chart's round-relative DSP time owns phase.</summary>
    public static class PhraseMotion
    {
        public static Vector2 Offset(double phase, int trajectory)
        {
            float angle = (float)((phase - System.Math.Floor(phase)) * System.Math.PI * 2d);
            if (trajectory == 1) return new Vector2(.18f * Mathf.Sin(angle), .12f * (1f - Mathf.Cos(angle)) * .5f);
            if (trajectory == 2) return new Vector2(.18f * Mathf.Sin(angle), .12f * Mathf.Sin(angle * 2f));
            return Vector2.zero;
        }
    }
}
