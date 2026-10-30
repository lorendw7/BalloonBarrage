using UnityEngine;
using System.Collections.Generic;
using BalloonBarrage.Foundation;

public class BalloonSpawner : MonoBehaviour
{
    private static readonly int[] LateralSweepSlots =
        { -2, -1, 0, 1, 2, 1, 0, -1 };

    private static readonly int[] ShallowFanSlots =
        { -2, 0, 2, -1, 1, 0 };

    private static readonly int[] StaggeredClusterSlots =
        { -1, 1, 0, 2, -2, 0 };
    private static readonly float[] PlacementRows = { 0f, 0.85f, -0.85f, 1.7f, -1.7f };

    [SerializeField] private GameObject balloonPrefab;

    [SerializeField] private BalloonSpawnSettings spawnSettings;

    [SerializeField]
    private Vector2 xRange = new Vector2(
        -3.3f, 3.3f);

    [SerializeField]
    private Vector2 yRange = new Vector2(
        -1.2f, 1.4f);

    [SerializeField]
    private float zPosition = 8f;

    [SerializeField, Min(1)]
    private int maxBalloons = 16;
    public int ActiveCount => activeBalloons.Count;
    public IReadOnlyList<Balloon> ActiveBalloons => activeBalloons;

    private readonly List<Balloon> activeBalloons =
        new List<Balloon>();

    private void Start()
    {
        if (spawnSettings == null || balloonPrefab == null)
        {
            Debug.LogError("请连接生成配置和气球预制体");
            enabled = false;
            return;
        }

    }

    public void CheckExpiredBalloons(double songTime)
    {
        for (int index = activeBalloons.Count - 1;
            index >= 0;
            index--)
        {
            Balloon balloon = activeBalloons[index];

            if (balloon == null || !balloon.gameObject.activeSelf)
            {
                activeBalloons.RemoveAt(index);
                continue;
            }

            balloon.CheckExpiry(songTime);
        }
    }


    public bool Spawn(RhythmEventData eventData)
    {
        activeBalloons.RemoveAll(balloon => balloon == null || !balloon.gameObject.activeSelf);
        if (activeBalloons.Count >= maxBalloons) return false;
        int placementSlot = eventData.authoredPlacement
            ? Mathf.Clamp(eventData.placementSlot, -2, 2)
            : GetPlacementSlot(eventData.pattern, eventData.patternStep);

        Vector3 position = GetPatternPosition(eventData.pattern, placementSlot, eventData.patternStep);

        if (!TryFindOpenPosition(position, out position)) return false;

        GameObject newBalloon = Instantiate(
            balloonPrefab,
            position,
            balloonPrefab.transform.rotation
        );
        Balloon balloon = newBalloon.GetComponent<Balloon>();
        balloon.InitializeRhythm( eventData );
        if (eventData.trajectory != 0)
            newBalloon.AddComponent<MelodyTrajectory>().Configure(eventData, this);
        Debug.Log($"Initialized balloon {balloon.EventId}; beat at {balloon.EventTimeSeconds:F3}s.");

        activeBalloons.Add(balloon);
        return true;
    }

    private bool TryFindOpenPosition(Vector3 preferred, out Vector3 position)
    {
        // Preserve the authored column whenever a readable row is available.
        foreach (float row in PlacementRows)
        {
            Vector3 candidate = preferred;
            candidate.y = Mathf.Clamp(preferred.y + row, yRange.x, yRange.y);
            if (IsPositionOpen(candidate))
            {
                position = candidate;
                return true;
            }
        }

        // Search the bounded area for the closest free alternative. Failure
        // leaves the chart event pending instead of creating an overlap.
        int columns = Mathf.Clamp(Mathf.CeilToInt((xRange.y - xRange.x) / 0.425f), 1, 32);
        int rows = Mathf.Clamp(Mathf.CeilToInt((yRange.y - yRange.x) / 0.425f), 1, 32);
        float closestDistance = float.PositiveInfinity;
        position = preferred;
        bool found = false;
        for (int column = 0; column <= columns; column++)
        {
            for (int row = 0; row <= rows; row++)
            {
                Vector3 candidate = new Vector3(
                    Mathf.Lerp(xRange.x, xRange.y, (float)column / columns),
                    Mathf.Lerp(yRange.x, yRange.y, (float)row / rows), zPosition);
                float distance = (candidate - preferred).sqrMagnitude;
                if (distance >= closestDistance || !IsPositionOpen(candidate)) continue;
                position = candidate;
                closestDistance = distance;
                found = true;
            }
        }
        return found;
    }

    private bool IsPositionOpen(Vector3 candidate)
    {
        return CanMoveTarget(null, candidate);
    }

    public Vector3 ClampTargetPosition(Vector3 candidate) => new Vector3(
        Mathf.Clamp(candidate.x, xRange.x, xRange.y), Mathf.Clamp(candidate.y, yRange.x, yRange.y), zPosition);

    public bool CanMoveTarget(Balloon target, Vector3 candidate)
    {
        return !activeBalloons.Exists(other => other != target && other != null && other.gameObject.activeSelf &&
            (other.transform.position - candidate).sqrMagnitude < 0.75f * 0.75f);
    }

    public Vector3 GetPatternPosition(BarragePattern pattern, int placementSlot, int patternStep)
    {
        Vector2 normalizedOffset =
            KyudaisaiLearnerCore.CalculatePatternOffset(
                pattern,
                placementSlot,
                patternStep);

        return new Vector3(
            Mathf.Lerp(xRange.x, xRange.y,
                Mathf.Clamp01(normalizedOffset.x * 0.5f + 0.5f)),
            Mathf.Lerp(yRange.x, yRange.y,
                Mathf.Clamp01(normalizedOffset.y * 0.5f + 0.5f)),
            zPosition);

    }

    private static int GetPlacementSlot(
        BarragePattern pattern,
        int patternStep)
    {
        int[] slots;

        switch (pattern)
        {
            case BarragePattern.ShallowFan:
                slots = ShallowFanSlots;
                break;
            case BarragePattern.StaggeredCluster:
                slots = StaggeredClusterSlots;
                break;
            default:
                slots = LateralSweepSlots;
                break;
        }

        return slots[Mathf.Abs(patternStep) % slots.Length];
    }
}
