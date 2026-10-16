using UnityEngine;



public class PaintProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 35f;

    [SerializeField] private float radius = 0.08f;

    [SerializeField] private float lifetime = 2f;

    public bool ShotWasOnBeat {  get; private set; }

    public int TargetEventId { get; private set; } = -1;

    public void InitializeShot(bool isOnBeat, int targetEventId)
    {
        ShotWasOnBeat = isOnBeat;
        TargetEventId = targetEventId;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        float distance = speed * Time.deltaTime;

        bool hasHit = Physics.SphereCast(
         transform.position,
         radius,
         transform.forward,
         out RaycastHit hit,
         distance,
         Physics.DefaultRaycastLayers,
         QueryTriggerInteraction.Ignore
        );

        if (hasHit)
        {
            Balloon balloon =
                hit.collider.GetComponentInParent<Balloon>();

            if (balloon != null)
            {
                int bonusPoints = 0;

                if (ShotWasOnBeat &&
                    TargetEventId >= 0 &&
                    balloon.EventId == TargetEventId)
                {
                    bonusPoints = 10;
                }

                balloon.Hit(bonusPoints);
            }

            Destroy(gameObject);
            return;

        }

        transform.position += transform.forward * distance;
    }
}
