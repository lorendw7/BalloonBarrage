using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BalloonFloat : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.6f;

    [SerializeField, Min(0f)]
    private float driftDelaySeconds = 3f;

    [SerializeField]
    private Vector2 xRange = new Vector2(-3.5f, 3.5f);

    [SerializeField]
    private Vector2 yRange = new Vector2(-2.5f, 2.5f);

    private Rigidbody body;
    private bool hasStartedMoving;
    private float driftElapsedSeconds;
    private RigidbodyConstraints originalConstraints;
    private bool holdingPosition;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        originalConstraints = body.constraints;
    }

    private void OnEnable()
    {
        if (hasStartedMoving || driftDelaySeconds <= 0f) return;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.constraints = originalConstraints | RigidbodyConstraints.FreezePosition;
        holdingPosition = true;
    }

    private void OnDisable()
    {
        ReleasePosition();
    }

    private void ReleasePosition()
    {
        if (!holdingPosition) return;
        body.constraints = originalConstraints;
        holdingPosition = false;
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
            // Count only active play so pause preserves the initial formation.
            driftElapsedSeconds += Time.fixedDeltaTime;
            if (driftElapsedSeconds < driftDelaySeconds)
            {
                return;
            }

            ReleasePosition();
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
