using UnityEngine;
using BalloonBarrage.Foundation;

public class BalloonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject balloonPrefab;

    [SerializeField] private BalloonSpawnSettings spawnSettings;

    private float remainingDelay;

    [SerializeField]
    private Vector2 xRange = new Vector2(
        -3.3f, 3.3f);

    [SerializeField]
    private Vector2 yRange = new Vector2(
        -1.2f, 1.4f);

    [SerializeField]
    private float zPosition = 8f;

    private GameObject currentBallon;

    private void Start()
    {
        if (spawnSettings == null || balloonPrefab == null)
        {
            Debug.LogError("请连接生成配置和气球预制体");
            enabled = false;
            return;
        }

        Spawn();
        remainingDelay = spawnSettings.RespawnDelay;
    }

    private void Update()
    {
        if (currentBallon != null)
        {
            return;
        }

        remainingDelay -= Time.deltaTime;

        if (remainingDelay > 0f)
        {
            return;
        }

        Spawn();
        remainingDelay = spawnSettings.RespawnDelay;
    }

    private void Spawn()
    {
        Vector3 position = new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            zPosition
        );

        currentBallon = Instantiate(
            balloonPrefab,
            position,
            balloonPrefab.transform.rotation
        );
    }    
}
