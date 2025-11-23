using Core.Feature.Logging.Abstractions;
using Core.Feature.Logging.Runtime;
using Game.Runtime;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Runtime
{
    /// <summary>
    /// 玩法场景 Scope：注册关卡装配、顾客/订单系统、评分与满意度等场景专属依赖，解析 GameLifetimeScope 暴露的共享服务。
    /// </summary>
    public sealed class GameplayLifetimeScope : GameLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // 在此注册 LevelConfig、LevelLoader、CustomerSpawner、SatisfactionService 等玩法层依赖。
            base.Configure(builder);
            // builder.Register<LogService>(Lifetime.Singleton)
            //     .As<ILogService>();
        }
    }
}
