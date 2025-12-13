using System;
using System.Threading;
using Core.Feature.Logging.Abstractions;
using Core.Feature.Loading.Abstractions;
using Core.Feature.SceneManagement.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 主菜单 Presenter：处理按钮事件，调用场景/加载服务。
    /// 按钮点击由 MainMenuView 触发，通过事件转发到此处。
    /// </summary>
    public sealed class MainMenuPresenter : IInitializable, IDisposable
    {
        private readonly IMainMenuView _view;
        private readonly ISceneService _sceneService;
        private readonly ILoadingService _loadingService;
        private readonly ILogService _logService;

        private bool _isLoading;

        [Inject]
        public MainMenuPresenter(
            IMainMenuView view,
            ISceneService sceneService,
            ILoadingService loadingService,
            ILogService logService)
        {
            _view = view;
            _sceneService = sceneService;
            _loadingService = loadingService;
            _logService = logService;
        }

        public void Initialize()
        {
            _view.PlayClicked += OnPlayClicked;
            _view.SettingsClicked += OnSettingsClicked;
            _view.QuitClicked += OnQuitClicked;
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
            _view.SettingsClicked -= OnSettingsClicked;
            _view.QuitClicked -= OnQuitClicked;
        }

        private void OnPlayClicked()
        {
            if (_isLoading) return;
            _isLoading = true;
            _view.SetInteractable(false);
            _view.ShowLoadingIndicator(true);

            StartGameAsync(_view.DestroyCancellationToken).Forget();
        }

        private void OnSettingsClicked()
        {
            // 预留：打开设置面板或跳转设置页。
            // 可与未来的 MenuNavigationService / SettingsPresenter 对接。
            _logService?.Information(LogCategory.Menu,
                "[主菜单] 点击设置（待接入设置/导航服务）");
        }

        private void OnQuitClicked()
        {
            // 预留：退出提示弹窗，当前直接退出。
            _logService?.Information(LogCategory.Menu, "[主菜单] 点击退出 → Application.Quit()");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private async UniTask StartGameAsync(CancellationToken token)
        {
            using var scope = _loadingService?.Begin("进入游戏");

            try
            {
                _logService?.Information(LogCategory.Menu, $"[主菜单] 开始加载场景：{_view.StartSceneKey}");
                var progress = _loadingService?.CreateProgressReporter("加载场景");
                await _sceneService.LoadSceneAsync(_view.StartSceneKey, _view.UseLoadingScreen, progress);
            }
            catch (Exception ex)
            {
                _logService?.Error(LogCategory.Menu, $"[主菜单] 加载场景失败：{ex.Message}", ex);
                _view.SetInteractable(true);
                _view.ShowLoadingIndicator(false);
                _isLoading = false;
                throw;
            }

            // 切场景后当前 View 会被销毁；但如果仍存在则恢复状态。
            if (!token.IsCancellationRequested)
            {
                _view.SetInteractable(true);
                _view.ShowLoadingIndicator(false);
            }

            _logService?.Information(LogCategory.Menu, "[主菜单] 场景加载完成");
            _isLoading = false;
        }
    }
}
