using UnityEngine;

public class BalloonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject balloonPrefab;

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
        Spawn();
    }

    private void Update()
    {
        if (currentBallon == null)
        {
            Spawn();
        }
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
