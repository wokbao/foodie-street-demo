using VContainer;
using VContainer.Unity;

namespace FoodStreet.Game.Menu.Bootstrap
{
    /// <summary>
    /// 菜单场景 Scope，负责注册菜单 UI 相关依赖。
    /// </summary>
    public sealed class MenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 在此注册菜单相关的 Presenter、ViewModel、数据源等。
        }
    }
}
