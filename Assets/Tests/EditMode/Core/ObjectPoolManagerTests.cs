using NUnit.Framework;
using Core.Feature.ObjectPooling.Abstractions;

namespace Tests.EditMode.Core
{
    /// <summary>
    /// ObjectPoolManager 单元测试
    /// 注意：完整测试需要 PlayMode（因为需要 IAssetProvider 等依赖）
    /// 这里只测试可独立运行的逻辑
    /// </summary>
    public class ObjectPoolManagerTests
    {
        // ==============================================
        // 测试 1：验证 PoolStatistics 默认值
        // ==============================================
        [Test]
        public void PoolStatistics_DefaultValues_AreZero()
        {
            // Arrange
            var stats = new PoolStatistics();

            // Assert
            Assert.AreEqual(0, stats.TotalCreated);
            Assert.AreEqual(0, stats.ActiveCount);
            Assert.AreEqual(0, stats.IdleCount);
            Assert.AreEqual(0, stats.RentCount);
            Assert.AreEqual(0, stats.ReturnCount);
        }

        // ==============================================
        // 测试 2：验证 PoolStatistics 赋值
        // ==============================================
        [Test]
        public void PoolStatistics_CanBeAssigned()
        {
            // Arrange & Act
            var stats = new PoolStatistics
            {
                TotalCreated = 10,
                ActiveCount = 5,
                IdleCount = 5,
                RentCount = 15,
                ReturnCount = 10
            };

            // Assert
            Assert.AreEqual(10, stats.TotalCreated);
            Assert.AreEqual(5, stats.ActiveCount);
            Assert.AreEqual(5, stats.IdleCount);
            Assert.AreEqual(15, stats.RentCount);
            Assert.AreEqual(10, stats.ReturnCount);
        }

        // ==============================================
        // 测试 3：验证 HitRate 计算
        // ==============================================
        [Test]
        public void PoolStatistics_HitRate_CalculatesCorrectly()
        {
            // Arrange
            var stats = new PoolStatistics
            {
                TotalCreated = 5,
                RentCount = 20
            };

            // Act
            var hitRate = stats.HitRate;

            // Assert - 命中率 = (20 - 5) / 20 = 0.75
            Assert.AreEqual(0.75f, hitRate, 0.001f);
        }

        [Test]
        public void PoolStatistics_HitRate_ZeroRentCount_ReturnsZero()
        {
            // Arrange
            var stats = new PoolStatistics { RentCount = 0 };

            // Act
            var hitRate = stats.HitRate;

            // Assert
            Assert.AreEqual(0f, hitRate);
        }
    }
}
