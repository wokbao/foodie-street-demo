using VContainer;
using VContainer.Unity;

namespace FoodStreet.Game.Gameplay.Bootstrap
{
    /// <summary>
    /// 玩法场景 Scope，负责注册关卡、店铺、顾客等依赖。
    /// </summary>
    public sealed class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 在此注册 LevelConfig、LevelLoader、SatisfactionService 等玩法依赖。
        }
    }
}
