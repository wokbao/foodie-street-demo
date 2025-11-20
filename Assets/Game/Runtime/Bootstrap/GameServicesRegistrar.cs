using VContainer;
using FoodStreet.Core.Bootstrap;

namespace FoodStreet.Game.Bootstrap
{
    /// <summary>
    /// 游戏域常驻服务注册（进度、静态数据、音频、全局 UI 等），挂到 CoreBootstrap 物体上。
    /// </summary>
    public sealed class GameServicesRegistrar : LifetimeScopeRegistrar
    {
        public override void Register(IContainerBuilder builder)
        {
            // builder.Register<GameProgressService>(Lifetime.Singleton);
            // builder.Register<IAssetProvider, AddressableAssetProvider>(Lifetime.Singleton);
            // builder.Register<IAudioBus, AudioBus>(Lifetime.Singleton);
            // builder.Register<IGlobalUiService, GlobalUiService>(Lifetime.Singleton);
        }
    }
}
