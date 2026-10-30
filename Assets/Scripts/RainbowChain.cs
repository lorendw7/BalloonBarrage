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
    private RoundController round;
    private BalloonSpawner spawner;
    private Vector3 burstOrigin;
    private float flashRemaining;

    public bool IsReady => charge.IsReady;
    public int BurstCount => charge.BurstCount;
    public int LastNeighborCount { get; private set; }
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

        burstOrigin = primary.transform.position;
        LastNeighborCount = 0;
        candidates.Clear();
        double now = round.SongTimeSeconds;
        foreach (Balloon target in spawner.ActiveBalloons)
        {
            if (target == primary || target == null || !target.CanReceiveChain(now)) continue;
            if ((target.transform.position - burstOrigin).sqrMagnitude <= Radius * Radius)
                candidates.Add(target);
        }
        candidates.Sort(CompareNearest);
        foreach (Balloon target in candidates)
        {
            if (LastNeighborCount == MaximumNeighbors) break;
            if (target == null || !target.CanReceiveChain(now)) continue;
            Vector3 destination = target.transform.position;
            // Primary colliders are disabled before this callback. Another balloon can
            // block a farther one until it is resolved; walls always block the burst.
            if (Physics.Linecast(burstOrigin, destination, out RaycastHit obstruction,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) &&
                obstruction.collider.GetComponentInParent<Balloon>() != target) continue;
            target.HitFromChain();
            LastNeighborCount++;
        }
        flashRemaining = 1.4f;
        candidates.Clear();
    }

    private int CompareNearest(Balloon left, Balloon right)
    {
        int distance = (left.transform.position - burstOrigin).sqrMagnitude.CompareTo(
            (right.transform.position - burstOrigin).sqrMagnitude);
        return distance != 0 ? distance : left.EventId.CompareTo(right.EventId);
    }
}
