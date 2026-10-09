using UnityEngine;
using System.Collections.Generic;
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

    [SerializeField, Min(1)]
    private int maxBalloons = 3;

    private List<GameObject> activeBalloons = new List<GameObject>();

    private void Start()
    {
        if (spawnSettings == null || balloonPrefab == null)
        {
            Debug.LogError("请连接生成配置和气球预制体");
            enabled = false;
            return;
        }

        for (int i = 0; i < maxBalloons; i++)
        {
            Spawn();
        }
        remainingDelay = spawnSettings.RespawnDelay;
    }

    private void Update()
    {
        activeBalloons.RemoveAll(balloon => balloon == null);

        if (activeBalloons.Count >= maxBalloons)
        {
            remainingDelay = spawnSettings.RespawnDelay;
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

        GameObject newBalloon = Instantiate(
            balloonPrefab,
            position,
            balloonPrefab.transform.rotation
        );

        activeBalloons.Add(newBalloon);
    }    
}
