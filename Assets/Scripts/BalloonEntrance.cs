using UnityEngine;
using BalloonBarrage.Foundation;
using UnityEngine.Rendering;

public class BalloonEntrance : MonoBehaviour
{
    [SerializeField]
    private BalloonSpawnSettings spawnSettings;

    private Vector3 targetScale;
    private Vector3 startScale;
    private float elapsedTime;


    void Start()
    {
        if (spawnSettings == null)
        {
            Debug.LogWarning("气球没有连接生成配置。", this);
            enabled = false;
            return;
        }

        targetScale = transform.localScale;

        startScale = 
            targetScale * spawnSettings.EntranceStartScale;

        elapsedTime = 0f;

        transform.localScale = startScale;

    }


    void Update()
    {
        elapsedTime += Time.deltaTime;

        float progress = Mathf.Clamp01(
            elapsedTime / spawnSettings.EntranceDuration
        );

        float smoothProgress =
            Mathf.SmoothStep(0f, 1f, progress);

        transform.localScale = Vector3.Lerp(
            startScale, targetScale, smoothProgress );

        if (progress >= 1f)
        {
            transform.localScale = targetScale;
            enabled = false;
        }

        
    }
}
