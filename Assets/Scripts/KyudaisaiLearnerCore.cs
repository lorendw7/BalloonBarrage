using UnityEngine;

public enum BarragePattern
{
    LateralSweep,
    ShallowFan,
    StaggeredCluster
}

/// <summary>
/// Three deliberately small gameplay decisions reserved for the learner.
/// The surrounding song, timing, spawning, UI and VFX infrastructure is ready.
/// </summary>
public static class KyudaisaiLearnerCore
{
    public static BarragePattern SelectBarragePattern(double roundProgress)
    {
        if (roundProgress < 1.0 / 3.0)
        {
            return BarragePattern.LateralSweep;
        }

        if (roundProgress < 2.0 / 3.0)
        {
            return BarragePattern.ShallowFan;
        }

        return BarragePattern.StaggeredCluster;
    }

    public static Vector2 CalculatePatternOffset(
        BarragePattern pattern,
        int placementSlot,
        int patternStep)
    {
        switch (pattern)
        {
            case BarragePattern.LateralSweep:
                return new Vector2(
                    placementSlot * 0.5f,
                    0f);

            case BarragePattern.ShallowFan:
                return new Vector2(
                    placementSlot * 0.45f,
                    0.12f + Mathf.Abs(placementSlot) * 0.16f);

            case BarragePattern.StaggeredCluster:
                float height = patternStep % 2 == 0
                    ? 0.28f
                    : -0.28f;

                return new Vector2(
                    placementSlot * 0.28f,
                    height);

            default:
                return Vector2.zero;
        }
    }

    public static int SelectExplosionVariant(
        int balloonColorIndex,
        int variantCount)
    {
        if (variantCount <= 0)
        {
            return 0;
        }

        return Mathf.Max(0, balloonColorIndex) % variantCount;
    }
}
