using BalloonBarrage.Foundation;
using UnityEngine;

/// <summary>One authored phrase moves logical target/collider together, without shifting judgement.</summary>
[DisallowMultipleComponent]
public sealed class MelodyTrajectory : MonoBehaviour
{
    private Balloon balloon;
    private RoundController round;
    private BalloonSpawner spawner;
    private BalloonFloat floating;
    private Rigidbody body;
    private Vector3 origin;
    private int trajectory;
    private double start, end, period;
    private bool originalKinematic, originalFloat;
    public bool IsActive { get; private set; }

    public void Configure(RhythmEventData data, BalloonSpawner owner)
    {
        balloon = GetComponent<Balloon>();
        round = FindFirstObjectByType<RoundController>();
        spawner = owner;
        body = GetComponent<Rigidbody>();
        floating = GetComponent<BalloonFloat>();
        origin = transform.position;
        trajectory = data.trajectory;
        start = data.motionStartSeconds; end = data.motionEndSeconds; period = data.motionPeriodSeconds;
        if (trajectory < 1 || trajectory > 2 || period <= 0d || end <= start || body == null || round == null)
        { enabled = false; return; }
        originalKinematic = body.isKinematic;
        originalFloat = floating != null && floating.enabled;
        if (floating != null) floating.enabled = false;
        body.isKinematic = true;
        IsActive = true;
    }

    private void FixedUpdate()
    {
        if (!IsActive || !RoundController.IsPlaying || RoundController.IsPaused) return;
        double now = round.SongTimeSeconds;
        if (now >= end) { Release(); enabled = false; return; }
        Vector2 offset = PhraseMotion.Offset(System.Math.Max(0d, now - start) / period, trajectory);
        Vector3 candidate = origin + new Vector3(offset.x, offset.y, 0f);
        candidate = spawner.ClampTargetPosition(candidate);
        if (spawner.CanMoveTarget(balloon, candidate)) body.position = candidate;
    }

    private void Release()
    {
        if (!IsActive) return;
        IsActive = false;
        if (body != null) body.isKinematic = originalKinematic;
        if (floating != null) floating.enabled = originalFloat;
    }
    private void OnDisable() { Release(); }
}
