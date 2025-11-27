using Core.Feature.Logging.Abstractions;
using Core.Feature.Logging.Runtime;
using Game.Runtime;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Runtime
{
    /// <summary>
    /// 玩法场景 LifetimeScope
    /// 
    /// <para><b>职责范围</b>：</para>
    /// 注册与玩法场景（Gameplay）相关的服务，这些服务：
    /// <list type="bullet">
    ///   <item>仅在玩法场景中使用，不被其他场景（如 Menu）共享</item>
    ///   <item>管理关卡逻辑、顾客系统、订单系统、评分等</item>
    ///   <item>生命周期绑定到场景，场景卸载时自动销毁</item>
    ///   <item>可以访问 Game 层和 Core 层的所有服务</item>
    /// </list>
    /// 
    /// <para><b>与其他 LifetimeScope 的关系</b>：</para>
    /// <list type="bullet">
    ///   <item>继承：GameLifetimeScope（代码继承，自动获取 Game 层服务）</item>
    ///   <item>可访问：Core 层的所有基础设施服务</item>
    /// </list>
    /// </summary>
    public sealed class GameplayLifetimeScope : GameLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 先调用父类注册，确保 Game 层服务可用
            base.Configure(builder);
            
            // ========================================
            // 已注册的玩法场景服务
            // ========================================
            
            // （当前为空，待实现）
            
            
            // ========================================
            // TODO: 待补充的玩法场景服务
            // ========================================
            
            // TODO: 关卡配置加载（LevelConfigLoader / LevelDataService）
            // - 加载关卡配置数据
            // - 解析关卡参数（难度、食谱、顾客数量等）
            // - 提供关卡数据访问接口
            // builder.Register<ILevelConfigLoader, LevelConfigLoader>(Lifetime.Scoped);
            
            // TODO: 关卡管理器（LevelManager / GameplayController）
            // - 关卡初始化与清理
            // - 关卡流程控制（开始、暂停、结束）
            // - 关卡胜利/失败判定
            // builder.Register<ILevelManager, LevelManager>(Lifetime.Scoped);
            
            // TODO: 顾客生成系统（CustomerSpawner / CustomerManager）
            // - 顾客生成逻辑（时间、数量、类型）
            // - 顾客队列管理
            // - 顾客行为控制
            // builder.Register<ICustomerSpawner, CustomerSpawner>(Lifetime.Scoped);
            
            // TODO: 订单系统（OrderService / OrderManager）
            // - 订单生成与分配
            // - 订单状态管理（待处理、制作中、完成）
            // - 订单验证与结算
            // builder.Register<IOrderService, OrderService>(Lifetime.Scoped);
            
            // TODO: 烹饪系统（CookingService / RecipeManager）
            // - 食谱管理
            // - 烹饪流程控制（准备、烹饪、装盘）
            // - 食材组合验证
            // builder.Register<ICookingService, CookingService>(Lifetime.Scoped);
            
            // TODO: 评分系统（ScoringService / RatingManager）
            // - 计算订单评分（速度、准确度、质量）
            // - 累计总分
            // - 星级评定
            // builder.Register<IScoringService, ScoringService>(Lifetime.Scoped);
            
            // TODO: 满意度系统（SatisfactionService）
            // - 顾客满意度计算
            // - 满意度影响因素（等待时间、食物质量）
            // - 满意度奖惩机制
            // builder.Register<ISatisfactionService, SatisfactionService>(Lifetime.Scoped);
            
            // TODO: 时间管理（GameplayTimeManager）
            // - 关卡倒计时
            // - 加速/慢速时间特效
            // - 时间暂停
            // builder.Register<IGameplayTimeManager, GameplayTimeManager>(Lifetime.Scoped);
            
            // TODO: 特效管理（VFXManager）
            // - 烹饪特效
            // - UI 特效（评分、连击）
            // - 粒子特效池化
            // builder.Register<IVFXManager, VFXManager>(Lifetime.Scoped);
            
            // TODO: 玩法 UI 控制器（GameplayUIController）
            // - 关卡 UI 显示与更新
            // - HUD 管理（分数、时间、生命值）
            // - 暂停菜单、结算界面
            // builder.Register<IGameplayUIController, GameplayUIController>(Lifetime.Scoped);
            
            // TODO: 教程系统（TutorialService）- 如果需要
            // - 新手引导流程
            // - 提示系统
            // - 高亮与遮罩
            // builder.Register<ITutorialService, TutorialService>(Lifetime.Scoped);
            
            // ========================================
            // 注意事项
            // ========================================
            // 1. 使用 Lifetime.Scoped，服务会在场景卸载时自动释放
            // 2. 场景特定的 MonoBehaviour 可以通过 RegisterComponentInHierarchy 注册
            // 3. 需要跨场景保存的数据应该写入 Game 层的服务，而不是保存在这里
            // 4. 避免直接依赖 Unity 的静态 API，优先通过接口注入
        }
    }
}
