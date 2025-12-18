using System;
using System.Threading;
using Core.Feature.AssetManagement.Abstractions;
using Core.Feature.Logging.Abstractions;
using Core.Feature.SceneManagement.Abstractions;
using Core.Feature.Loading.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 负责按需加载主菜单屏幕（Addressable Key: UI/Screens/MainMenu），并手动装配 MainMenuPresenter。
    /// 场景中的 Canvas 为空挂点，由此类实例化 UI。
    /// </summary>
    public sealed class MainMenuScreenLoader : IStartable, IDisposable
    {
        private const string AddressKey = "UI/Screens/MainMenu";

        private readonly IObjectResolver _resolver;
        private readonly IAssetProvider _assetProvider;
        private readonly ILogService _logService;

        private GameObject _instance;
        private AsyncOperationHandle<GameObject>? _handle;
        private MainMenuPresenter _presenter;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        [Inject]
        public MainMenuScreenLoader(
            IObjectResolver resolver,
            IAssetProvider assetProvider,
            ILogService logService)
        {
            _resolver = resolver;
            _assetProvider = assetProvider;
            _logService = logService;
        }

        public void Start()
        {
            LoadAsync(_cts.Token).Forget();
        }

        private async UniTask LoadAsync(CancellationToken token)
        {
            try
            {
                var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    _logService?.Error(LogCategory.Menu, "[主菜单] 场景中未找到 Canvas，无法实例化主菜单 UI");
                    return;
                }

                _logService?.Information(LogCategory.Menu, "[主菜单] 加载 UI 屏幕：{0}", AddressKey);
                var handle = _assetProvider.LoadAssetAsync<GameObject>(AddressKey);
                _handle = handle;
                var prefab = await handle.ToUniTask(token);

                _instance = UnityEngine.Object.Instantiate(prefab, canvas.transform);
                _instance.name = "MainMenuScreen";

                var view = _instance.GetComponentInChildren<MainMenuView>();
                if (view == null)
                {
                    _logService?.Error(LogCategory.Menu, "[主菜单] MainMenuView 未找到，无法装配 Presenter");
                    return;
                }

                // 手动装配 Presenter（直接从容器解析依赖）。
                var sceneService = _resolver.Resolve<ISceneService>();
                var loadingService = _resolver.Resolve<ILoadingService>();
                var navigationService = _resolver.Resolve<IMenuNavigationService>();
                var quitHandler = _resolver.Resolve<IQuitHandler>();

                _presenter = new MainMenuPresenter(
                    view,
                    sceneService,
                    loadingService,
                    navigationService,
                    quitHandler,
                    _logService);
                _presenter.Initialize();
            }
            catch (OperationCanceledException)
            {
                _logService?.Warning(LogCategory.Menu, "[主菜单] 加载被取消");
            }
            catch (Exception ex)
            {
                _logService?.Error(LogCategory.Menu, $"[主菜单] 加载 UI 失败：{ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;

            _presenter?.Dispose();
            _presenter = null;

            if (_instance != null)
            {
                UnityEngine.Object.Destroy(_instance);
                _instance = null;
            }

            if (_handle.HasValue)
            {
                _assetProvider.Release(_handle.Value);
                _handle = null;
            }
        }
    }
}

