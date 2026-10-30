using BalloonBarrage.Foundation;
using NUnit.Framework;

namespace BalloonBarrage.Foundation.Tests
{
    public sealed class RainbowChainChargeTests
    {
        [Test]
        public void ChargeCannotFireBeforeItIsEarnedOrTwiceAfterOneGrant()
        {
            var charge = new RainbowChainCharge();
            Assert.That(charge.TryConsume(), Is.False);
            charge.Grant();
            Assert.That(charge.TryConsume(), Is.True);
            Assert.That(charge.TryConsume(), Is.False);
            Assert.That(charge.BurstCount, Is.EqualTo(1));
        }

        [Test]
        public void RepeatedGrantsBankOnlyOneBurst()
        {
            var charge = new RainbowChainCharge();
            charge.Grant(); charge.Grant(); charge.Grant();
            Assert.That(charge.TryConsume(), Is.True);
            Assert.That(charge.TryConsume(), Is.False);
            charge.Grant();
            Assert.That(charge.TryConsume(), Is.True);
            Assert.That(charge.BurstCount, Is.EqualTo(2));
        }

        [Test]
        public void RetryClearsChargeAndBurstCount()
        {
            var charge = new RainbowChainCharge();
            charge.Grant(); charge.TryConsume(); charge.Grant();
            charge.Reset();
            Assert.That(charge.IsReady, Is.False);
            Assert.That(charge.BurstCount, Is.Zero);
            Assert.That(charge.TryConsume(), Is.False);
        }
    }
}
