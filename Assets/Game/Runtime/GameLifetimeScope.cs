using VContainer;
using VContainer.Unity;

namespace Game.Runtime
{
    /// <summary>
    /// 游戏域共享 Scope：承接 Core 注册的基础设施，集中注册玩家进度、经济、音频、资源加载等跨场景服务，供 Gameplay/Menu 等子 Scope 复用。
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 注册游戏层面的共享服务（PlayerProgress、EconomyService、AudioBus、AddressablesProvider 等）。
        }
    }
}
