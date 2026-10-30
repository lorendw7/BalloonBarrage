using System.Collections.Generic;
using BalloonBarrage.Foundation;
using UnityEngine;

/// <summary>Optional scoring rules: one direct hit can resolve at most two nearby chart targets.</summary>
[DisallowMultipleComponent]
public sealed class RainbowChain : MonoBehaviour
{
    public const float Radius = 1.8f;
    public const int MaximumNeighbors = 2;
    private readonly RainbowChainCharge charge = new RainbowChainCharge();
    private readonly List<Balloon> candidates = new List<Balloon>(16);
    private readonly List<Balloon> ordered = new List<Balloon>(16);
    private readonly RaycastHit[] visibilityHits = new RaycastHit[64];
    private RoundController round;
    private BalloonSpawner spawner;
    private Vector3 burstOrigin;
    private float flashRemaining;

    public bool IsReady => charge.IsReady;
    public int BurstCount => charge.BurstCount;
    public int LastNeighborCount { get; private set; }
    public int LastExtraPoints { get; private set; }
    private readonly Vector3[] burstPositions = new Vector3[2];
    private readonly int[] burstPoints = new int[2];
    public bool IsFlashing => flashRemaining > 0f;
    public bool IsSelected => round != null && round.RainbowChainEnabled;

    public void Configure(RoundController controller)
    {
        round = controller;
        spawner = FindFirstObjectByType<BalloonSpawner>();
    }

    private void OnEnable() { Balloon.OnDirectPop += Burst; }
    private void OnDisable() { Balloon.OnDirectPop -= Burst; }

    public void ResetRound()
    {
        charge.Reset();
        flashRemaining = 0f;
        LastNeighborCount = 0;
        LastExtraPoints = 0;
        candidates.Clear();
    }

    public void GrantCharge()
    {
        if (IsSelected && RoundController.IsPlaying && !RoundController.IsPaused) charge.Grant();
    }

    private void Update()
    {
        if (RoundController.IsPlaying && !RoundController.IsPaused)
            flashRemaining = Mathf.Max(0f, flashRemaining - Time.deltaTime);
    }

    private void Burst(Balloon primary)
    {
        if (!IsSelected || !RoundController.IsPlaying || RoundController.IsPaused ||
            primary == null || primary.EventId < 0 || spawner == null || !charge.TryConsume()) return;

        LastNeighborCount = 0;
        LastExtraPoints = 0;
        Vector3 primaryPosition = primary.GuidePosition;
        SelectTargets(primary, candidates);
        foreach (Balloon target in candidates)
        {
            if (target == null || !target.CanReceiveChain(round.SongTimeSeconds)) continue;
            Vector3 position = target.GuidePosition;
            int before = Score.Value;
            target.HitFromChain();
            int earned = Score.Value - before;
            burstPositions[LastNeighborCount] = position;
            burstPoints[LastNeighborCount] = earned;
            LastExtraPoints += earned;
            LastNeighborCount++;
        }
        flashRemaining = 1.4f;
        GetComponent<RainbowChainPreview>()?.ShowBurst(primaryPosition, burstPositions, burstPoints, LastNeighborCount);
        candidates.Clear();
    }

    public int GetPreviewTargets(Balloon primary, List<Balloon> result)
    {
        result.Clear();
        if (!IsSelected || !IsReady || !RoundController.IsPlaying || RoundController.IsPaused ||
            primary == null || round == null || spawner == null || !primary.CanReceiveChain(round.SongTimeSeconds)) return 0;
        SelectTargets(primary, result);
        return result.Count;
    }

    private void SelectTargets(Balloon primary, List<Balloon> result)
    {
        result.Clear();
        ordered.Clear();
        burstOrigin = primary.transform.position;
        double now = round.SongTimeSeconds;
        foreach (Balloon target in spawner.ActiveBalloons)
        {
            if (target == primary || target == null || !target.CanReceiveChain(now)) continue;
            if ((target.transform.position - burstOrigin).sqrMagnitude <= Radius * Radius)
                ordered.Add(target);
        }
        ordered.Sort(CompareNearest);
        foreach (Balloon target in ordered)
        {
            if (result.Count == MaximumNeighbors) break;
            if (VisibleAfterSelected(primary, target, result)) result.Add(target);
        }
        ordered.Clear();
    }

    private bool VisibleAfterSelected(Balloon primary, Balloon target, List<Balloon> selected)
    {
        Vector3 delta = target.transform.position - burstOrigin;
        int count = Physics.RaycastNonAlloc(burstOrigin, delta.normalized, visibilityHits, delta.magnitude,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        if (count == visibilityHits.Length) return false; // Conservative on unexpected collider congestion.
        for (int index = 0; index < count; index++)
        {
            Balloon hit = visibilityHits[index].collider.GetComponentInParent<Balloon>();
            if (hit == primary || hit == target || (hit != null && selected.Contains(hit))) continue;
            return false;
        }
        return true;
    }

    private int CompareNearest(Balloon left, Balloon right)
    {
        int distance = (left.transform.position - burstOrigin).sqrMagnitude.CompareTo(
            (right.transform.position - burstOrigin).sqrMagnitude);
        return distance != 0 ? distance : left.EventId.CompareTo(right.EventId);
    }
}
