using Game.Menu.Runtime.Abstractions;
using VContainer;
using VContainer.Unity;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单场景作用域：仅注册 Menu 场景专属的服务与入口点（生命周期跟随场景）。
    /// </summary>
    public sealed class MenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IMenuNavigationService, MenuNavigationService>(Lifetime.Scoped);
            builder.Register<IQuitHandler, DefaultQuitHandler>(Lifetime.Scoped);

            builder.RegisterEntryPoint<MainMenuScreenLoader>(Lifetime.Scoped);
        }
    }
}

