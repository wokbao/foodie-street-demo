using System;
using System.Threading;
using Core.Feature.Logging.Abstractions;
using Core.Feature.SceneManagement.Abstractions;
using Core.Feature.Loading.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using Game.UI.Runtime.Abstractions;
using UnityEngine;
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
        private readonly IUIFactory _uiFactory;
        private readonly ILogService _logService;
        private readonly UIHierarchyConfig _uiHierarchyConfig;

        private GameObject _instance;
        private MainMenuPresenter _presenter;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        [Inject]
        public MainMenuScreenLoader(
            IObjectResolver resolver,
            IUIFactory uiFactory,
            ILogService logService,
            UIHierarchyConfig uiHierarchyConfig = null)
        {
            _resolver = resolver;
            _uiFactory = uiFactory;
            _logService = logService;
            _uiHierarchyConfig = uiHierarchyConfig != null ? uiHierarchyConfig : UIHierarchyConfig.Default;
        }

        public void Start()
        {
            LoadAsync(_cts.Token).Forget();
        }

        private async UniTask LoadAsync(CancellationToken token)
        {
            try
            {
                var parent = FindTargetLayer();
                if (parent == null)
                {
                    _logService?.Error(LogCategory.Menu, "[主菜单] 未找到 UI 根节点，无法实例化主菜单 UI");
                    return;
                }

                _logService?.Information(LogCategory.Menu, "[主菜单] 加载 UI 屏幕：{0}", AddressKey);
                _instance = await _uiFactory.InstantiateAsync(AddressKey, parent, null, token);

                var view = _instance != null
                    ? _instance.GetComponentInChildren<MainMenuView>()
                    : null;
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
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            _presenter?.Dispose();
            _presenter = null;

            if (_instance != null)
            {
                UnityEngine.Object.Destroy(_instance);
                _instance = null;
            }

            _uiFactory.Release(AddressKey);
        }

        private Transform FindTargetLayer()
        {
            var rootName = string.IsNullOrWhiteSpace(_uiHierarchyConfig.RootName)
                ? "GlobalUIRoot"
                : _uiHierarchyConfig.RootName;

            var root = GameObject.Find(rootName);
            if (root == null)
            {
                return null;
            }

            var layerName = string.IsNullOrWhiteSpace(_uiHierarchyConfig.MainLayerName)
                ? "Layer_Main"
                : _uiHierarchyConfig.MainLayerName;

            var layer = root.transform.Find(layerName);
            if (layer == null)
            {
                var go = new GameObject(layerName);
                layer = go.transform;
                layer.SetParent(root.transform, false);
            }

            return layer;
        }
    }
}
