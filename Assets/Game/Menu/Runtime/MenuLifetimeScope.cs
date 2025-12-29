using Game.Menu.Runtime.Abstractions;
using VContainer;
using VContainer.Unity;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单场景作用域：仅注册 Menu 场景专属的服务与入口点（生命周期跟随场景）。
    /// <para>
    /// 实现 ISceneReadyHandler 接口，作为 SceneService 与场景内部逻辑（UI 加载）的桥梁。
    /// SceneService 会找到此 MonoBehaviour 并等待，而此处将等待委托给 MainMenuScreenLoader。
    /// </para>
    /// </summary>
    public sealed class MenuLifetimeScope : LifetimeScope, Core.Feature.SceneManagement.Abstractions.ISceneReadyHandler
    {
        private Core.Feature.SceneManagement.Abstractions.ISceneReadyHandlerRegistry _handlerRegistry;

        protected override void Awake()
        {
            base.Awake();

            // 从父容器解析注册器（通过 DI 而非静态字段）
            if (Parent != null && Parent.Container != null)
            {
                _handlerRegistry = Parent.Container.Resolve<Core.Feature.SceneManagement.Abstractions.ISceneReadyHandlerRegistry>();
                _handlerRegistry?.Register(this);
                UnityEngine.Debug.Log($"[MenuLifetimeScope] [帧:{UnityEngine.Time.frameCount}] 已通过 Registry 注册为 ActiveHandler");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[MenuLifetimeScope] Parent 容器未就绪，无法注册 ISceneReadyHandler");
            }
        }

        protected override void OnDestroy()
        {
            _handlerRegistry?.Unregister(this);
            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MenuNavigationService>(Lifetime.Scoped)
                .As<IMenuNavigationService>();

            builder.Register<DefaultQuitHandler>(Lifetime.Scoped)
                .As<IQuitHandler>();

            // AsSelf() 允许我们后续手动解析它
            builder.RegisterEntryPoint<MainMenuScreenLoader>(Lifetime.Scoped).AsSelf();
        }

        public async Cysharp.Threading.Tasks.UniTask WaitForSceneReadyAsync()
        {
            if (Container == null)
            {
                UnityEngine.Debug.LogWarning("[MenuLifetimeScope] Container is null! cannot resolve MainMenuScreenLoader. Building now...");
                Build(); // 尝试手动构建
            }

            // 确保容器已构建
            if (Container != null)
            {
                // 委托给实际负责加载 UI 的类
                var loader = Container.Resolve<MainMenuScreenLoader>();
                if (loader != null)
                {
                    UnityEngine.Debug.Log("[MenuLifetimeScope] Delegating wait to MainMenuScreenLoader...");
                    await loader.WaitForSceneReadyAsync();
                }
                else
                {
                    UnityEngine.Debug.LogError("[MenuLifetimeScope] Failed to resolve MainMenuScreenLoader!");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("[MenuLifetimeScope] Container is STILL null after attempt to build!");
            }
        }
    }
}

