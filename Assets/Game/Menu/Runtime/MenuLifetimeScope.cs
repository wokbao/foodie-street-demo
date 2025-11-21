using VContainer;
using VContainer.Unity;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单场景 Scope：负责注册主菜单/商店/活动等 UI Presenter、ViewModel、数据适配器，并复用 GameLifetimeScope 的共享服务。
    /// </summary>
    public sealed class MenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 在此注册菜单相关的 Presenter、ViewModel、导航服务、数据源等依赖。
        }
    }
}
