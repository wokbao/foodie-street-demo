using System;
using NUnit.Framework;
using Core.Feature.SceneManagement.Abstractions;

namespace Tests.EditMode.Core
{
    /// <summary>
    /// SceneService 单元测试（仅测试同步逻辑）
    /// 注意：场景加载需要 PlayMode 测试，这里只测试枚举和基础逻辑
    /// </summary>
    public class SceneServiceTests
    {
        // ==============================================
        // 测试 1：验证场景过渡模式枚举
        // ==============================================
        [Test]
        public void SceneTransitionMode_HasExpectedValues()
        {
            // Assert - 验证枚举值存在
            Assert.IsTrue(Enum.IsDefined(typeof(SceneTransitionMode), SceneTransitionMode.Cinematic));
            Assert.IsTrue(Enum.IsDefined(typeof(SceneTransitionMode), SceneTransitionMode.Fade));
            Assert.IsTrue(Enum.IsDefined(typeof(SceneTransitionMode), SceneTransitionMode.None));
        }

        // ==============================================
        // 测试 2：验证 SceneTransitionMode 枚举值数量
        // ==============================================
        [Test]
        public void SceneTransitionMode_HasCorrectEnumCount()
        {
            // Act
            var values = Enum.GetValues(typeof(SceneTransitionMode));

            // Assert - 应该有多种过渡模式
            Assert.GreaterOrEqual(values.Length, 3, "应该至少有 3 种过渡模式");
        }

        // ==============================================
        // 测试 3：验证 SceneTransitionMode 默认值
        // ==============================================
        [Test]
        public void SceneTransitionMode_DefaultIsCinematic()
        {
            // Arrange
            var defaultMode = default(SceneTransitionMode);

            // Assert - 默认值应该是 Cinematic (0)
            Assert.AreEqual(SceneTransitionMode.Cinematic, defaultMode);
        }
    }
}
