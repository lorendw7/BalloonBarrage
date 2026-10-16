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

        remainingDelay = 0f;
    }

    private void Update()
    {
        activeBalloons.RemoveAll(balloon => balloon == null);
    }

    public void CheckExpiredBalloons(double songTime)
    {
        foreach (GameObject balloonObject in activeBalloons)
        {
            if(balloonObject == null)
            {
                continue;
            }

            Balloon balloon = balloonObject.GetComponent<Balloon>();
            balloon.CheckExpiry(songTime);
        }
    }


    public void Spawn(RhythmEventData eventData)
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
        Balloon balloon = newBalloon.GetComponent<Balloon>();
        balloon.InitializeRhythm( eventData );
        Debug.Log($"Initialized balloon {balloon.EventId}; beat at {balloon.EventTimeSeconds:F3}s.");

        activeBalloons.Add(newBalloon);
    }    
}
