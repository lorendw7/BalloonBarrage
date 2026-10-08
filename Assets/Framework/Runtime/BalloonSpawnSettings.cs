using UnityEngine;

namespace BalloonBarrage.Foundation
{
    // Data only: learner scripts decide when and how balloons spawn.
    // 只保存参数：生成时机和入场行为由学习者的脚本决定。
    [CreateAssetMenu(menuName = "BalloonBarrage/Balloon Spawn Settings", fileName = "BalloonSpawnSettings")]
    public sealed class BalloonSpawnSettings : ScriptableObject
    {
        [Header("Spawn area / 生成区域")]
        [SerializeField] private Vector2 xRange = new Vector2(-3f, 3f);
        [SerializeField] private Vector2 yRange = new Vector2(-1.2f, 1.4f);
        [SerializeField] private float zPosition = 8f;

        [Header("Pacing / 节奏")]
        [SerializeField, Min(0f)] private float respawnDelay = 0.9f;

        [Header("Entrance (next lesson) / 入场（后续课）")]
        [SerializeField, Min(0.01f)] private float entranceDuration = 0.35f;
        [SerializeField, Range(0.01f, 1f)] private float entranceStartScale = 0.15f;

        public Vector2 XRange => new Vector2(Mathf.Min(xRange.x, xRange.y), Mathf.Max(xRange.x, xRange.y));
        public Vector2 YRange => new Vector2(Mathf.Min(yRange.x, yRange.y), Mathf.Max(yRange.x, yRange.y));
        public float ZPosition => zPosition;
        public float RespawnDelay => Mathf.Max(0f, respawnDelay);
        public float EntranceDuration => Mathf.Max(0.01f, entranceDuration);
        public float EntranceStartScale => Mathf.Clamp(entranceStartScale, 0.01f, 1f);
    }
}
