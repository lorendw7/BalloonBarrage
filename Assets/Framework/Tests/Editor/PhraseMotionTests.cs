using BalloonBarrage.Foundation;
using NUnit.Framework;
using UnityEngine;

namespace BalloonBarrage.Foundation.Tests
{
    public sealed class PhraseMotionTests
    {
        [Test]
        public void AuthoredPathsStaySmallAndCloseAtTheirPeriod()
        {
            for (int kind = 1; kind <= 2; kind++)
            {
                Assert.That(PhraseMotion.Offset(0, kind).sqrMagnitude, Is.LessThan(0.000001f));
                Assert.That(PhraseMotion.Offset(1, kind).sqrMagnitude, Is.LessThan(0.000001f));
                for (int sample = 0; sample <= 100; sample++)
                {
                    Vector2 offset = PhraseMotion.Offset(sample / 100d, kind);
                    Assert.That(Mathf.Abs(offset.x), Is.LessThanOrEqualTo(.181f));
                    Assert.That(Mathf.Abs(offset.y), Is.LessThanOrEqualTo(.121f));
                }
            }
        }

        [Test]
        public void PathsArePeriodicAndUnknownTypeIsStationary()
        {
            Assert.That((PhraseMotion.Offset(.125, 2) - PhraseMotion.Offset(3.125, 2)).sqrMagnitude, Is.LessThan(.000001f));
            Assert.That(PhraseMotion.Offset(.25, 0), Is.EqualTo(Vector2.zero));
            Assert.That(PhraseMotion.Offset(.25, 1).y, Is.GreaterThan(0f));
            Assert.That(PhraseMotion.Offset(.375, 2).y, Is.LessThan(0f));
        }
    }
}
