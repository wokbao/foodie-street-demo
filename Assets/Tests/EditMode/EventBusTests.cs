using NUnit.Framework;
using Core.Feature.EventBus.Runtime;

namespace Tests.EditMode
{
    /// <summary>
    /// EventBus 单元测试
    /// 这是一个真正的单元测试示例！
    /// </summary>
    public class EventBusTests
    {
        // ==============================================
        // 测试 1：验证订阅和发布是否正常工作
        // ==============================================
        [Test]
        public void Subscribe_WhenEventPublished_CallbackIsInvoked()
        {
            // 📝 AAA 模式（Arrange-Act-Assert）

            // 1️⃣ Arrange（准备阶段）：创建测试对象和数据
            var eventBus = new EventBus();  // EventBus 无依赖，直接 new！

            bool wasCallbackInvoked = false;  // 标记回调是否被调用

            // 订阅事件
            eventBus.Subscribe<TestEvent>(evt =>
            {
                wasCallbackInvoked = true;  // 回调被触发时设为 true
            });

            // 2️⃣ Act（执行阶段）：执行要测试的操作
            eventBus.Publish(new TestEvent("测试消息"));

            // 3️⃣ Assert（断言阶段）：验证结果是否符合预期
            Assert.IsTrue(wasCallbackInvoked, "订阅的回调应该被触发");

            // ✅ 如果运行到这里没报错，测试通过！
        }

        // ==============================================
        // 测试 2：验证优先级是否生效
        // ==============================================
        [Test]
        public void Subscribe_WithPriority_HighPriorityExecutesFirst()
        {
            // Arrange
            var eventBus = new EventBus();

            var executionOrder = new System.Collections.Generic.List<int>();

            // 注意：先注册低优先级，再注册高优先级
            eventBus.Subscribe<TestEvent>(evt => executionOrder.Add(1), priority: 0);   // 低优先级
            eventBus.Subscribe<TestEvent>(evt => executionOrder.Add(2), priority: 10);  // 高优先级

            // Act
            eventBus.Publish(new TestEvent("优先级测试"));

            // Assert：验证执行顺序
            Assert.AreEqual(2, executionOrder.Count, "应该执行了 2 个回调");
            Assert.AreEqual(2, executionOrder[0], "高优先级（10）应该先执行");
            Assert.AreEqual(1, executionOrder[1], "低优先级（0）应该后执行");
        }

        // ==============================================
        // 测试 3：验证取消订阅是否生效
        // ==============================================
        [Test]
        public void Unsubscribe_AfterDispose_CallbackNotInvoked()
        {
            // Arrange
            var eventBus = new EventBus();

            int callbackCount = 0;

            var token = eventBus.Subscribe<TestEvent>(evt =>
            {
                callbackCount++;
            });

            // Act：发布一次
            eventBus.Publish(new TestEvent("第一次"));
            Assert.AreEqual(1, callbackCount, "第一次发布应该触发回调");

            // 取消订阅
            token.Dispose();

            // 再次发布
            eventBus.Publish(new TestEvent("第二次"));

            // Assert：取消订阅后不应该再触发
            Assert.AreEqual(1, callbackCount, "取消订阅后不应该再触发回调");
        }

        // ==============================================
        // 辅助类：测试用的事件
        // ==============================================
        private readonly struct TestEvent
        {
            public string Message { get; }
            public TestEvent(string message) => Message = message;
        }
    }
}
