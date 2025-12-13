using Game.Menu.Runtime.Abstractions;
using Game.Runtime;
using VContainer;
using VContainer.Unity;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单场景 LifetimeScope
    /// 
    /// <para><b>职责范围</b>：</para>
    /// 注册与菜单场景（Menu）相关的服务，这些服务：
    /// <list type="bullet">
    ///   <item>仅在菜单场景中使用，不被其他场景（如 Gameplay）共享</item>
    ///   <item>管理主菜单、设置、商店、图鉴等 UI 模块</item>
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
    public sealed class MenuLifetimeScope : GameLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 先调用父类注册，确保 Game 层服务可用
            base.Configure(builder);
            
            // ========================================
            // 已注册的菜单场景服务
            // ========================================
            
            builder.RegisterComponentInHierarchy<MainMenuView>()
                .As<IMainMenuView>();
            
            builder.Register<MainMenuPresenter>(Lifetime.Scoped)
                .AsSelf()
                .AsImplementedInterfaces();
            
            
            // ========================================
            // TODO: 待补充的菜单场景服务
            // ========================================
            
            // TODO: 菜单导航服务（MenuNavigationService）
            // - 菜单界面间的跳转逻辑
            // - 返回栈管理
            // - 页面转场动画控制
            // builder.Register<IMenuNavigationService, MenuNavigationService>(Lifetime.Scoped);
            
            // TODO: 主菜单 Presenter（MainMenuPresenter）
            // - 主界面按钮事件处理
            // - 开始游戏、设置、退出等功能
            // - 新手引导触发
            // builder.Register<MainMenuPresenter>(Lifetime.Scoped);
            
            // TODO: 关卡选择 Presenter（LevelSelectionPresenter）
            // - 关卡列表展示
            // - 关卡锁定/解锁状态
            // - 关卡星级显示
            // - 关卡难度选择
            // builder.Register<LevelSelectionPresenter>(Lifetime.Scoped);
            
            // TODO: 商店 Presenter（ShopPresenter / StorePresenter）
            // - 商品列表展示
            // - 购买流程 UI
            // - 货币显示更新
            // builder.Register<ShopPresenter>(Lifetime.Scoped);
            
            // TODO: 设置 Presenter（SettingsPresenter）
            // - 音量调节
            // - 画质设置
            // - 语言切换
            // - 账号管理
            // builder.Register<SettingsPresenter>(Lifetime.Scoped);
            
            // TODO: 图鉴/收藏 Presenter（CollectionPresenter）
            // - 食谱图鉴展示
            // - 成就展示
            // - 解锁内容预览
            // builder.Register<CollectionPresenter>(Lifetime.Scoped);
            
            // TODO: 活动/每日任务 Presenter（EventPresenter / DailyQuestPresenter）
            // - 限时活动展示
            // - 每日任务列表
            // - 奖励领取
            // builder.Register<EventPresenter>(Lifetime.Scoped);
            
            // TODO: 排行榜 Presenter（LeaderboardPresenter）
            // - 全球排行榜
            // - 好友排行榜
            // - 榜单刷新
            // builder.Register<LeaderboardPresenter>(Lifetime.Scoped);
            
            // TODO: 菜单 UI 工厂（MenuUIFactory）
            // - 动态创建 UI 元素（商品项、关卡项等）
            // - UI 对象池管理
            // builder.Register<IMenuUIFactory, MenuUIFactory>(Lifetime.Scoped);
            
            // TODO: 菜单数据适配器（MenuDataAdapter）
            // - 将游戏数据转换为 UI 显示格式
            // - 数据绑定与同步
            // builder.Register<IMenuDataAdapter, MenuDataAdapter>(Lifetime.Scoped);
            
            // TODO: 菜单音效控制器（MenuAudioController）
            // - 菜单 BGM 播放
            // - 按钮音效
            // - 场景转换音效
            // builder.Register<IMenuAudioController, MenuAudioController>(Lifetime.Scoped);
            
            // ========================================
            // 注意事项
            // ========================================
            // 1. 使用 Lifetime.Scoped，服务会在场景卸载时自动释放
            // 2. Presenter 负责 UI 逻辑，ViewModel 负责数据，保持职责分离
            // 3. 需要持久化的设置应该通过 Game 层的服务保存
            // 4. UI 元素通过 RegisterComponentInHierarchy 注册
        }
    }
}
