using System;
using System.Threading;
using Core.Feature.Logging.Abstractions;
using Core.Feature.SceneManagement.Abstractions;
using Core.Feature.Loading.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Audio.Runtime.Abstractions;
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
    public sealed class MainMenuScreenLoader : IStartable, IDisposable, ISceneReadyHandler
    {
        private const string AddressKey = UIKeys.Screens.MainMenu;

        private readonly IObjectResolver _resolver;
        private readonly IUIFactory _uiFactory;
        private readonly IUIRootService _uiRootService;
        private readonly ILogService _logService;

        private GameObject _instance;
        private MainMenuPresenter _presenter;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private UniTaskCompletionSource _sceneReadyTcs = new UniTaskCompletionSource();

        [Inject]
        public MainMenuScreenLoader(
            IObjectResolver resolver,
            IUIFactory uiFactory,
            IUIRootService uiRootService,
            ILogService logService)
        {
            _resolver = resolver;
            _uiFactory = uiFactory;
            _uiRootService = uiRootService;
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
                var audioService = _resolver.Resolve<IAudioService>();

                _presenter = new MainMenuPresenter(
                    view,
                    sceneService,
                    loadingService,
                    navigationService,
                    quitHandler,
                    _logService,
                    audioService);
                _presenter.Initialize();

                // 关键：等待几帧，确保 Canvas 完成重建和渲染
                // 1帧可能不够（Layout Rebuild -> Graphic Rebuild -> Batching -> Render）
                UnityEngine.Debug.Log($"[MainMenuScreenLoader] [帧:{UnityEngine.Time.frameCount}] UI 实例化完成，开始等待渲染帧...");

                // 等待至少 2-3 帧
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                // 标记场景准备就绪
                _sceneReadyTcs.TrySetResult();
                _logService?.Information(LogCategory.Menu, $"[主菜单] [帧:{UnityEngine.Time.frameCount}] 视觉准备就绪 (Wait Finished)");
            }
            catch (OperationCanceledException)
            {
                _logService?.Warning(LogCategory.Menu, "[主菜单] 加载被取消");
                _sceneReadyTcs.TrySetCanceled();
            }
            catch (Exception ex)
            {
                _logService?.Error(LogCategory.Menu, $"[主菜单] 加载 UI 失败：{ex.Message}", ex);
                // 即使失败也标记完成，避免无限卡住 Loading
                _sceneReadyTcs.TrySetResult();
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
            _uiRootService.EnsureInitialized();
            return _uiRootService.GetLayer(UILayer.Main);
        }
        public UniTask WaitForSceneReadyAsync()
        {
            return _sceneReadyTcs.Task;
        }
    }
}
