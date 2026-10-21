using UnityEngine;



public class PaintProjectile : MonoBehaviour
{
    // Cosmetic selection must not consume the spawn/movement random stream.
    private static readonly System.Random pigmentRandom = new System.Random();

    [SerializeField] private float speed = 35f;

    [SerializeField] private float radius = 0.08f;

    [SerializeField] private float lifetime = 2f;

    [SerializeField] private GameObject paintMarkPrefab; 

    [SerializeField] private Material[] pigments;

    [SerializeField] private GameObject[] paintMarkVariants;

    [SerializeField] private float paintRotationRange = 20f;

    [System.Serializable]
    private class PaintPatternGroup
    {
        public GameObject[] prefabs;
    }

    [SerializeField]
    private PaintPatternGroup[] paintPatternGroups;

    private const int FirstPaintSortingOrder = 100;
    private static int nextPaintSortingOrder = FirstPaintSortingOrder;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ResetPaintSortingOrder()
    {
        nextPaintSortingOrder = FirstPaintSortingOrder;
    }

    private void Awake()
    {
        int index = pigmentRandom.Next(pigments.Length);

        Color color = pigments[index].GetColor("_BaseColor");

        StudioPaintTint.Effect(gameObject, color);

        GameObject[] patterns = paintPatternGroups[index].prefabs;

        int patternIndex = pigmentRandom.Next(patterns.Length);

        paintMarkPrefab = patterns[patternIndex];
    }

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
            else if (paintMarkPrefab != null &&
                    (hit.collider.name == "MainWall" ||
             hit.collider.name == "Floor" ||
             hit.collider.name == "LeftWall" ||
             hit.collider.name == "RightWall"))
            {
                Vector3 position = hit.point + hit.normal * 0.005f;

                Quaternion rotation = Quaternion.LookRotation(hit.normal);

                GameObject mark = Instantiate(paintMarkPrefab, position, rotation);

                int sortingOrder = nextPaintSortingOrder++;

                foreach(SpriteRenderer spriteRenderer
                    in mark.GetComponentsInChildren<SpriteRenderer>(true)) {
                    spriteRenderer.sortingOrder = sortingOrder;
                }

                float angle = ((float)pigmentRandom.NextDouble() * 2f - 1f) * paintRotationRange;

                mark.transform.Rotate(0f, 0f, angle, Space.Self);

                float scaleMultiplier = 0.8f + (float)pigmentRandom.NextDouble() * 0.4f;

                mark.transform.localScale *= scaleMultiplier;

            }

            Destroy(gameObject);
            return;

        }

        transform.position += transform.forward * distance;
    }
}
