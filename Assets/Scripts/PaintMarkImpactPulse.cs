using UnityEngine;

/// <summary>
/// Gives a newly deposited paint mark a brief liquid impact swell, then removes
/// itself while leaving the settled mark intact.
/// </summary>
public sealed class PaintMarkImpactPulse : MonoBehaviour
{
    private const float Duration = 0.16f;
    private const float StartScale = 0.48f;
    private const float OvershootScale = 1.14f;
    private const float OvershootTime = 0.58f;

    private Vector3 settledScale;
    private float elapsed;
    private bool initialized;

    public void Initialize(Vector3 targetScale)
    {
        settledScale = targetScale;
        transform.localScale = settledScale * StartScale;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / Duration);
        float scale;

        if (progress < OvershootTime)
        {
            float growProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress / OvershootTime);

            scale = Mathf.Lerp(
                StartScale,
                OvershootScale,
                growProgress);
        }
        else
        {
            float settleProgress = Mathf.SmoothStep(
                0f,
                1f,
                (progress - OvershootTime) / (1f - OvershootTime));

            scale = Mathf.Lerp(
                OvershootScale,
                1f,
                settleProgress);
        }

        transform.localScale = settledScale * scale;

        if (progress >= 1f)
        {
            transform.localScale = settledScale;
            Destroy(this);
        }
    }
}
