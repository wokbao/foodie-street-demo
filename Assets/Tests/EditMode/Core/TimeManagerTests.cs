using System;
using NUnit.Framework;
using Core.Feature.TimeManagement.Abstractions;
using Core.Feature.TimeManagement.Runtime;

namespace Tests.EditMode.Core
{
    /// <summary>
    /// TimeManager 单元测试
    /// </summary>
    public class TimeManagerTests
    {
        private TimeManager _timeManager;

        [SetUp]
        public void SetUp()
        {
            // 传入 null 作为 ILogService，TimeManager 应该能处理
            _timeManager = new TimeManager(null);
        }

        [TearDown]
        public void TearDown()
        {
            _timeManager?.Dispose();
            // 恢复默认时间缩放
            UnityEngine.Time.timeScale = 1f;
        }

        // ==============================================
        // 测试 1：验证暂停功能
        // ==============================================
        [Test]
        public void Pause_SetsTimeScaleToZero()
        {
            // Arrange
            UnityEngine.Time.timeScale = 1f;
            Assert.IsFalse(_timeManager.IsPaused);

            // Act
            _timeManager.Pause();

            // Assert
            Assert.IsTrue(_timeManager.IsPaused);
            Assert.AreEqual(0f, UnityEngine.Time.timeScale, 0.001f);
        }

        // ==============================================
        // 测试 2：验证恢复功能
        // ==============================================
        [Test]
        public void Resume_RestoresOriginalTimeScale()
        {
            // Arrange
            UnityEngine.Time.timeScale = 0.5f;
            _timeManager.Pause();
            Assert.IsTrue(_timeManager.IsPaused);

            // Act
            _timeManager.Resume();

            // Assert
            Assert.IsFalse(_timeManager.IsPaused);
            Assert.AreEqual(0.5f, UnityEngine.Time.timeScale, 0.001f);
        }

        // ==============================================
        // 测试 3：验证重复暂停不会改变保存的时间缩放
        // ==============================================
        [Test]
        public void Pause_MultipleCalls_DoesNotChangeStoredScale()
        {
            // Arrange
            UnityEngine.Time.timeScale = 2f;

            // Act
            _timeManager.Pause();
            _timeManager.Pause(); // 第二次调用应该被忽略

            // Assert
            _timeManager.Resume();
            Assert.AreEqual(2f, UnityEngine.Time.timeScale, 0.001f);
        }

        // ==============================================
        // 测试 4：验证创建计时器参数校验
        // ==============================================
        [Test]
        public void CreateTimer_WithZeroDuration_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _timeManager.CreateTimer(0f, () => { });
            });
        }

        [Test]
        public void CreateTimer_WithNegativeDuration_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _timeManager.CreateTimer(-1f, () => { });
            });
        }

        // ==============================================
        // 测试 5：验证计时器创建返回有效实例
        // ==============================================
        [Test]
        public void CreateTimer_ReturnsValidTimer()
        {
            // Act
            var timer = _timeManager.CreateTimer(1f, () => { });

            // Assert
            Assert.IsNotNull(timer);
            Assert.IsTrue(timer.IsRunning);
            Assert.IsFalse(timer.IsCompleted);
            Assert.IsFalse(timer.IsRepeating);
        }

        // ==============================================
        // 测试 6：验证重复计时器创建
        // ==============================================
        [Test]
        public void CreateRepeatingTimer_ReturnsRepeatingTimer()
        {
            // Act
            var timer = _timeManager.CreateRepeatingTimer(1f, () => { });

            // Assert
            Assert.IsNotNull(timer);
            Assert.IsTrue(timer.IsRepeating);
        }

        // ==============================================
        // 测试 7：验证取消所有计时器
        // ==============================================
        [Test]
        public void CancelAllTimers_CancelsAllActiveTimers()
        {
            // Arrange
            var timer1 = _timeManager.CreateTimer(1f, () => { });
            var timer2 = _timeManager.CreateTimer(2f, () => { });

            // Act
            _timeManager.CancelAllTimers();

            // Assert - 计时器应该被标记为不运行
            Assert.IsFalse(timer1.IsRunning);
            Assert.IsFalse(timer2.IsRunning);
        }

        // ==============================================
        // 测试 8：验证事件触发
        // ==============================================
        [Test]
        public void Pause_TriggersOnPauseChangedEvent()
        {
            // Arrange
            bool eventFired = false;
            bool pauseState = false;
            _timeManager.OnPauseChanged += (isPaused) =>
            {
                eventFired = true;
                pauseState = isPaused;
            };

            // Act
            _timeManager.Pause();

            // Assert
            Assert.IsTrue(eventFired);
            Assert.IsTrue(pauseState);
        }
    }
}
