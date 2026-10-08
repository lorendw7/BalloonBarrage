using BalloonBarrage.Foundation;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace BalloonBarrage.Foundation.Tests
{
    public sealed class BalloonSpawnSettingsTests
    {
        [Test]
        public void NewSettingsHaveUsableDefaults()
        {
            var settings = ScriptableObject.CreateInstance<BalloonSpawnSettings>();
            try
            {
                Assert.That(settings.RespawnDelay, Is.EqualTo(0.9f).Within(0.0001f));
                Assert.That(settings.EntranceDuration, Is.GreaterThan(0f));
                Assert.That(settings.XRange.x, Is.LessThanOrEqualTo(settings.XRange.y));
                Assert.That(settings.YRange.x, Is.LessThanOrEqualTo(settings.YRange.y));
            }
            finally { Object.DestroyImmediate(settings); }
        }

        [Test]
        public void NegativeDelayReadsAsZero()
        {
            var settings = ScriptableObject.CreateInstance<BalloonSpawnSettings>();
            try
            {
                var serialized = new SerializedObject(settings);
                serialized.FindProperty("respawnDelay").floatValue = -1f;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                Assert.That(settings.RespawnDelay, Is.Zero);
            }
            finally { Object.DestroyImmediate(settings); }
        }

        [Test]
        public void StarterAssetLoadsWithValidEntranceSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<BalloonSpawnSettings>(
                "Assets/Settings/Gameplay/BalloonSpawnSettings.asset");
            Assert.That(settings, Is.Not.Null, "Import the starter asset first / 请先导入配置资产。");
            Assert.That(settings.EntranceDuration, Is.GreaterThan(0f));
            Assert.That(settings.EntranceStartScale, Is.InRange(0.01f, 1f));
        }
    }
}
