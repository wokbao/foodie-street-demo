using Game.Runtime.Loading;
using VContainer;
using VContainer.Unity;

namespace Game.Runtime
{
    /// <summary>
    /// 游戏域共享 LifetimeScope
    /// 
    /// <para><b>职责范围</b>：</para>
    /// 注册与游戏业务相关但跨场景共享的服务，这些服务：
    /// <list type="bullet">
    ///   <item>依赖 Core 层的基础设施（资源、日志等）</item>
    ///   <item>被多个场景（Menu、Gameplay）共同使用</item>
    ///   <item>管理游戏的持久化状态（玩家数据、进度等）</item>
    ///   <item>生命周期通常为 Singleton，在场景切换时保持存活</item>
    /// </list>
    /// 
    /// <para><b>与其他 LifetimeScope 的关系</b>：</para>
    /// <list type="bullet">
    ///   <item>父容器：CoreLifetimeScope（通过 Unity Inspector 配置）</item>
    ///   <item>子容器：GameplayLifetimeScope、MenuLifetimeScope（代码继承）</item>
    /// </list>
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ========================================
            // 已注册的游戏域服务
            // ========================================

            builder.RegisterEntryPoint<LoadingHudEntryPoint>(Lifetime.Singleton);

            // （当前为空，待实现其他游戏域服务）

            // ========================================
            // TODO: 待补充的游戏域服务
            // ========================================

            // TODO: 玩家数据管理（PlayerDataService / PlayerProgressService）
            // - 玩家等级、经验值、解锁内容
            // - 玩家属性、技能、装备
            // - 游戏进度追踪
            // builder.Register<IPlayerDataService, PlayerDataService>(Lifetime.Singleton);

            // TODO: 经济系统（EconomyService / CurrencyManager）
            // - 货币管理（金币、钻石等）
            // - 交易、购买、奖励发放
            // - 经济平衡和防作弊
            // builder.Register<IEconomyService, EconomyService>(Lifetime.Singleton);

            // TODO: 音频管理（AudioService / AudioManager）
            // - BGM 播放与切换
            // - 音效播放与管理
            // - 音量控制、淡入淡出
            // - 音频池化
            // builder.Register<IAudioService, AudioService>(Lifetime.Singleton);

            // TODO: 成就系统（AchievementService）
            // - 成就解锁与追踪
            // - 成就进度管理
            // - 成就奖励发放
            // builder.Register<IAchievementService, AchievementService>(Lifetime.Singleton);

            // TODO: 任务系统（QuestService / MissionManager）
            // - 日常任务、主线任务管理
            // - 任务进度追踪
            // - 任务奖励结算
            // builder.Register<IQuestService, QuestService>(Lifetime.Singleton);

            // TODO: 商店系统（ShopService / StoreManager）
            // - 商品数据管理
            // - 购买流程处理
            // - 库存管理
            // - IAP（内购）集成
            // builder.Register<IShopService, ShopService>(Lifetime.Singleton);

            // TODO: 社交系统（SocialService）- 如果需要
            // - 好友管理
            // - 排行榜
            // - 分享功能
            // builder.Register<ISocialService, SocialService>(Lifetime.Singleton);

            // TODO: 分析统计（AnalyticsService）
            // - 游戏数据埋点
            // - 用户行为追踪
            // - 关卡漏斗分析
            // builder.Register<IAnalyticsService, AnalyticsService>(Lifetime.Singleton);

            // TODO: 游戏状态机（GameStateMachine）
            // - 游戏整体状态管理（启动、主菜单、游戏中、暂停等）
            // - 状态转换逻辑
            // builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);

            // TODO: 解锁系统（UnlockService）
            // - 食谱解锁
            // - 装备解锁
            // - 功能解锁
            // builder.Register<IUnlockService, UnlockService>(Lifetime.Singleton);

            // ========================================
            // 注意事项
            // ========================================
            // 1. 这里注册的服务会在场景切换时保持存活
            // 2. 避免在这里注册场景特定的服务（应放在 Gameplay/Menu Scope）
            // 3. 需要持久化的数据应该通过 Core 层的 SaveService 保存
            // 4. 优先使用接口，便于单元测试和 Mock
        }
    }
}
