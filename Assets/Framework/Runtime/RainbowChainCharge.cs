namespace BalloonBarrage.Foundation
{
    /// <summary>One banked burst, consumed by the next direct balloon hit.</summary>
    public sealed class RainbowChainCharge
    {
        public bool IsReady { get; private set; }
        public int BurstCount { get; private set; }

        public void Grant() { IsReady = true; }

        public bool TryConsume()
        {
            if (!IsReady) return false;
            IsReady = false;
            BurstCount++;
            return true;
        }

        public void Reset()
        {
            IsReady = false;
            BurstCount = 0;
        }
    }
}
