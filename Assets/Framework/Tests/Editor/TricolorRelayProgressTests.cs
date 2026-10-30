using BalloonBarrage.Foundation;
using NUnit.Framework;

namespace BalloonBarrage.Foundation.Tests
{
    public sealed class TricolorRelayProgressTests
    {
        [Test]
        public void RepeatedAndInvalidColorsPreservePartialCollection()
        {
            var progress = new TricolorRelayProgress();
            Assert.That(progress.Collect(0), Is.True);
            Assert.That(progress.Collect(0), Is.False);
            Assert.That(progress.Collect(-1), Is.False);
            Assert.That(progress.Collect(8), Is.False);
            Assert.That(progress.CollectedCount, Is.EqualTo(1));
            Assert.That(progress.CompletedCount, Is.Zero);
            Assert.That(progress.Contains(0), Is.True);
        }

        [Test]
        public void AnyThreeDistinctColorsCompleteAndBeginAnotherCollection()
        {
            var progress = new TricolorRelayProgress();
            foreach (int color in new[] { 7, 0, 4, 4, 1, 7 })
                Assert.That(progress.Collect(color), Is.True);
            Assert.That(progress.CompletedCount, Is.EqualTo(2));
            Assert.That(progress.CollectedCount, Is.Zero);
            Assert.That(progress.Contains(7), Is.False);
        }

        [Test]
        public void RetryClearsCompletedAndPartialCollections()
        {
            var progress = new TricolorRelayProgress();
            foreach (int color in new[] { 0, 1, 2, 7 }) progress.Collect(color);
            progress.Reset();
            Assert.That(progress.CompletedCount, Is.Zero);
            Assert.That(progress.CollectedCount, Is.Zero);
            Assert.That(progress.Collect(7), Is.True);
        }
    }
}
