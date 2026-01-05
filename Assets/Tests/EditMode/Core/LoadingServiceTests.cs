using System;
using NUnit.Framework;
using Core.Feature.Loading.Runtime;
using Core.Feature.Loading.Abstractions;

namespace Tests.EditMode.Core
{
    /// <summary>
    /// LoadingService 单元测试
    /// </summary>
    public class LoadingServiceTests
    {
        private LoadingService _loadingService;
        private MockTelemetry _mockTelemetry;

        [SetUp]
        public void SetUp()
        {
            _mockTelemetry = new MockTelemetry();
            // 传入 null 作为 ILogService，LoadingService 应该能处理
            _loadingService = new LoadingService(_mockTelemetry, null);
        }

        [TearDown]
        public void TearDown()
        {
            _loadingService?.Dispose();
        }

        // ==============================================
        // 测试 1：验证 Begin/Dispose 计数器
        // ==============================================
        [Test]
        public void Begin_IncrementsActiveOperations()
        {
            // Arrange
            Assert.AreEqual(0, _loadingService.ActiveOperations);

            // Act
            var scope = _loadingService.Begin("测试加载");

            // Assert
            Assert.AreEqual(1, _loadingService.ActiveOperations);
            Assert.IsTrue(_loadingService.IsLoading);

            scope.Dispose();
            Assert.AreEqual(0, _loadingService.ActiveOperations);
            Assert.IsFalse(_loadingService.IsLoading);
        }

        // ==============================================
        // 测试 2：验证嵌套加载计数
        // ==============================================
        [Test]
        public void Begin_Nested_CountsCorrectly()
        {
            // Arrange & Act
            var scope1 = _loadingService.Begin("加载 1");
            Assert.AreEqual(1, _loadingService.ActiveOperations);

            var scope2 = _loadingService.Begin("加载 2");
            Assert.AreEqual(2, _loadingService.ActiveOperations);

            var scope3 = _loadingService.Begin("加载 3");
            Assert.AreEqual(3, _loadingService.ActiveOperations);

            // Dispose in reverse order
            scope3.Dispose();
            Assert.AreEqual(2, _loadingService.ActiveOperations);

            scope2.Dispose();
            Assert.AreEqual(1, _loadingService.ActiveOperations);

            scope1.Dispose();
            Assert.AreEqual(0, _loadingService.ActiveOperations);
        }

        // ==============================================
        // 测试 3：验证进度报告
        // ==============================================
        [Test]
        public void ReportProgress_UpdatesState()
        {
            // Arrange
            var scope = _loadingService.Begin();

            // Act
            _loadingService.ReportProgress(0.5f, "加载中 50%");

            // Assert
            Assert.AreEqual(0.5f, _loadingService.Progress, 0.001f);
            Assert.AreEqual("加载中 50%", _loadingService.Description);

            scope.Dispose();
        }

        // ==============================================
        // 测试 4：验证阶段管理
        // ==============================================
        [Test]
        public void BeginPhase_EndPhase_UpdatesCurrentPhase()
        {
            // Arrange
            var scope = _loadingService.Begin();

            // Act
            _loadingService.BeginPhase("加载资源");

            // Assert
            Assert.AreEqual("加载资源", _loadingService.CurrentPhase);

            // Act
            _loadingService.EndPhase("加载资源");

            // Assert
            Assert.IsNull(_loadingService.CurrentPhase);

            scope.Dispose();
        }

        // ==============================================
        // 测试 5：验证事件触发
        // ==============================================
        [Test]
        public void Begin_Dispose_TriggersEvents()
        {
            // Arrange
            bool startedFired = false;
            bool completedFired = false;

            _loadingService.OnLoadingStarted += () => startedFired = true;
            _loadingService.OnLoadingCompleted += () => completedFired = true;

            // Act
            var scope = _loadingService.Begin();
            Assert.IsTrue(startedFired, "OnLoadingStarted 应该被触发");
            Assert.IsFalse(completedFired, "OnLoadingCompleted 不应该被触发");

            scope.Dispose();
            Assert.IsTrue(completedFired, "OnLoadingCompleted 应该被触发");
        }

        // ==============================================
        // 测试 6：验证前台/后台加载模式
        // ==============================================
        [Test]
        public void Begin_ForegroundMode_ShowsUI()
        {
            // Arrange & Act
            var foregroundScope = _loadingService.Begin(mode: LoadingMode.Foreground);
            Assert.IsTrue(_loadingService.ShouldShowUi, "Foreground 模式应该显示 UI");
            Assert.AreEqual(1, _loadingService.ActiveForegroundOperations);

            foregroundScope.Dispose();
            Assert.IsFalse(_loadingService.ShouldShowUi, "Dispose 后不应该显示 UI");
        }

        [Test]
        public void Begin_BackgroundMode_DoesNotShowUI()
        {
            // Arrange & Act
            var backgroundScope = _loadingService.Begin(mode: LoadingMode.Background);

            // Assert
            Assert.IsTrue(_loadingService.IsLoading, "应该在加载中");
            Assert.IsFalse(_loadingService.ShouldShowUi, "Background 模式不应该显示 UI");
            Assert.AreEqual(0, _loadingService.ActiveForegroundOperations);

            backgroundScope.Dispose();
        }

        // ==============================================
        // Mock 实现
        // ==============================================
        private class MockTelemetry : ILoadingTelemetry
        {
            public void RecordLoadingStart(string operationId, string description) { }
            public void RecordLoadingEnd(string operationId, float durationSeconds) { }
            public void RecordPhaseStart(string phaseName) { }
            public void RecordPhaseEnd(string phaseName, float durationSeconds) { }
            public LoadingMetrics GetMetrics() => default;
            public void Reset() { }
        }
    }
}
