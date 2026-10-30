namespace BalloonBarrage.Foundation
{
    /// <summary>Optional per-round collection; independent of score and rhythm judgement.</summary>
    public sealed class TricolorRelayProgress
    {
        public const int RequiredColors = 3;
        public const int PaletteSize = 8;
        private int collectedMask;
        public int CollectedCount { get; private set; }
        public int CompletedCount { get; private set; }

        public bool Contains(int colorIndex) => colorIndex >= 0 && colorIndex < PaletteSize &&
            (collectedMask & (1 << colorIndex)) != 0;

        // Returns true only for a newly collected pigment. A repeated color preserves progress.
        public bool Collect(int colorIndex)
        {
            if (colorIndex < 0 || colorIndex >= PaletteSize || Contains(colorIndex)) return false;
            collectedMask |= 1 << colorIndex;
            CollectedCount++;
            if (CollectedCount == RequiredColors)
            {
                CompletedCount++;
                CollectedCount = 0;
                collectedMask = 0;
            }
            return true;
        }

        public void Reset()
        {
            collectedMask = 0;
            CollectedCount = 0;
            CompletedCount = 0;
        }
    }
}
