using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BalloonFloat : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.6f;

    [SerializeField]
    private Vector2 xRange = new Vector2(-3.5f, 3.5f);

    [SerializeField]
    private Vector2 yRange = new Vector2(-2.5f, 2.5f);

    private Rigidbody body;
    private bool hasStartedMoving;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!RoundController.IsPlaying ||
            RoundController.IsPaused)
        {
            return;
        }

        if (!hasStartedMoving)
        {
            StartMoving();
            hasStartedMoving = true;
        }

        KeepInsideBounds();
    }

    private void StartMoving()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        Vector3 direction = new Vector3(
            Mathf.Cos(angle),
            Mathf.Sin(angle),
            0f);

        body.linearVelocity = direction * moveSpeed;
    }

    private void KeepInsideBounds()
    {
        Vector3 position = body.position;
        Vector3 velocity = body.linearVelocity;

        bool crossedBoundary = false;

        if (position.x < xRange.x)
        {
            position.x = xRange.x;
            velocity.x = Mathf.Abs(velocity.x);
            crossedBoundary = true;
        }
        else if (position.x > xRange.y)
        {
            position.x = xRange.y;
            velocity.x = -Mathf.Abs(velocity.x);
            crossedBoundary = true;
        }

        if (position.y < yRange.x)
        {
            position.y = yRange.x;
            velocity.y = Mathf.Abs(velocity.y);
            crossedBoundary = true;
        }
        else if (position.y > yRange.y)
        {
            position.y = yRange.y;
            velocity.y = -Mathf.Abs(velocity.y);
            crossedBoundary = true;
        }

        if (crossedBoundary)
        {
            body.position = position;
            body.linearVelocity = velocity;
        }
    }
}
