using UnityEngine;

public class BalloonFloat : MonoBehaviour
{

    [SerializeField]
    private float floatAmplitude = 0.12f;

    [SerializeField]
    private float floatFrequency = 0.6f;

    private Vector3 basePosition;
    private float elapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basePosition = transform.position;
        elapsedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        float wave = Mathf.Sin(
            elapsedTime * floatFrequency * Mathf.PI * 2f);

        float offsetY = wave * floatAmplitude;

        transform.position = 
            basePosition + Vector3.up * offsetY;
    }
}
