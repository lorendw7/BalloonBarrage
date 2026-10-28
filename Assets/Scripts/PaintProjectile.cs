using UnityEngine;



public class PaintProjectile : MonoBehaviour
{
    private static readonly int[] WallSprayPatternIndices =
    {
        0, 0, 0, 0, // Spray: broad wet coverage
        1, 1,       // Burst: smaller impact punctuation
        2           // Drip: occasional vertical gravity cue
    };

    private static readonly int[] FlatSprayPatternIndices =
    {
        0, 0, 0, // Spray
        1, 1     // Burst; no wall-style drips on floors
    };

    // Cosmetic selection must not consume the spawn/movement random stream.
    private static readonly System.Random pigmentRandom = new System.Random();

    [SerializeField] private float speed = 35f;

    [SerializeField] private float radius = 0.08f;

    [SerializeField] private float lifetime = 2f;

    [SerializeField] private GameObject paintMarkPrefab; 

    [SerializeField] private Material[] pigments;

    [SerializeField] private GameObject[] paintMarkVariants;

    [SerializeField] private float paintRotationRange = 20f;

    [SerializeField, Min(0.1f)]
    private float paintCoverageMultiplier = 2f;

    [SerializeField, Min(1f)]
    private float wallPaintWidthMultiplier = 1.35f;

    [System.Serializable]
    private class PaintPatternGroup
    {
        public GameObject[] prefabs;
    }

    [SerializeField]
    private PaintPatternGroup[] paintPatternGroups;

    private const int FirstPaintSortingOrder = 100;
    private static int nextPaintSortingOrder = FirstPaintSortingOrder;

    public int PigmentCount => Mathf.Min(
        pigments?.Length ?? 0,
        paintPatternGroups?.Length ?? 0);

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

    public Color GetPigmentColor(int pigmentIndex)
    {
        if (PigmentCount <= 0)
        {
            return new Color(0.025f, 0.62f, 0.69f, 1f);
        }

        int safeIndex = Mathf.Clamp(
            pigmentIndex,
            0,
            PigmentCount - 1);

        Material pigment = pigments[safeIndex];

        if (pigment != null && pigment.HasProperty("_BaseColor"))
        {
            return pigment.GetColor("_BaseColor");
        }

        return Color.white;
    }

    public void SpawnPaintMark(RaycastHit hit)
    {
        SpawnPaintMark(hit, 0);
    }

    public void SpawnPaintMark(RaycastHit hit, int pigmentIndex)
    {

        if (!PaintSurface.CanReceivePaint(hit.collider) ||
            paintPatternGroups == null ||
            paintPatternGroups.Length == 0)
        {
            return;
        }

        int groupIndex = Mathf.Clamp(
            pigmentIndex,
            0,
            paintPatternGroups.Length - 1);

        GameObject[] patterns = paintPatternGroups[groupIndex].prefabs;

        if (patterns == null || patterns.Length == 0) { return; }

        int patternIndex = SelectDirectSprayPattern(
            patterns.Length,
            hit.normal);

        GameObject selectedPrefab = patterns[patternIndex];

        if(selectedPrefab == null) { return; }

        Vector3 position = hit.point + hit.normal * 0.005f;

        Quaternion rotation =
            Quaternion.FromToRotation(
                Vector3.forward,
                hit.normal);

        GameObject mark = Instantiate(
            selectedPrefab, position, rotation);

        int sortingOrder = nextPaintSortingOrder++;

        SpriteRenderer spriteRenderer =
            mark.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }

        float angle = ((float)pigmentRandom.NextDouble() * 2f - 1f) * paintRotationRange;

        mark.transform.Rotate(
            0f,
            0f,
            angle,
            Space.Self);

        float scaleMultiplier = GetDirectSprayScale(patternIndex);

        Vector3 targetScale = mark.transform.localScale * scaleMultiplier;

        bool isVerticalSurface =
            Mathf.Abs(hit.normal.y) < 0.5f;

        if (isVerticalSurface)
        {
            targetScale.x *= wallPaintWidthMultiplier;
        }

        mark.transform.localScale = targetScale;

        PaintMarkImpactPulse impactPulse =
            mark.AddComponent<PaintMarkImpactPulse>();

        impactPulse.Initialize(mark.transform.localScale);
    }

    private static int SelectDirectSprayPattern(
        int patternCount,
        Vector3 surfaceNormal)
    {
        if (patternCount < 6)
        {
            return pigmentRandom.Next(patternCount);
        }

        bool isVerticalSurface = Mathf.Abs(surfaceNormal.y) < 0.5f;

        int[] weightedPatterns = isVerticalSurface
            ? WallSprayPatternIndices
            : FlatSprayPatternIndices;

        return weightedPatterns[
            pigmentRandom.Next(weightedPatterns.Length)];
    }

    private float GetDirectSprayScale(int patternIndex)
    {
        float randomValue = (float)pigmentRandom.NextDouble();

        switch (patternIndex)
        {
            case 0: // Spray
                return paintCoverageMultiplier *
                    (0.95f + randomValue * 0.45f);

            case 1: // Burst
                return paintCoverageMultiplier *
                    (0.8f + randomValue * 0.35f);

            case 2: // Drip
                return paintCoverageMultiplier *
                    (0.9f + randomValue * 0.25f);

            default:
                return 1f;
        }
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

                balloon.HitAt(hit.point, bonusPoints);
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

                PaintMarkImpactPulse impactPulse =
                    mark.AddComponent<PaintMarkImpactPulse>();

                impactPulse.Initialize(mark.transform.localScale);

            }

            Destroy(gameObject);
            return;

        }

        transform.position += transform.forward * distance;
    }
}
