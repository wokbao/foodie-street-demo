using System;
using System.Threading;
using Core.Feature.Localization.Abstractions;
using Core.Feature.Logging.Abstractions;
using Core.Feature.Loading.Abstractions;
using Core.Feature.SceneManagement.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Audio.Runtime;
using Game.Audio.Runtime.Abstractions;
using Game.Menu.Runtime.Abstractions;
using VContainer;
using VContainer.Unity;
#if UNITY_EDITOR
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
        private readonly IMenuNavigationService _navigationService;
        private readonly IQuitHandler _quitHandler;
        private readonly ILogService _logService;
        private readonly IAudioService _audioService;
        private readonly ILocalizationService _localizationService;

        private bool _isLoading;

        [Inject]
        public MainMenuPresenter(
            IMainMenuView view,
            ISceneService sceneService,
            ILoadingService loadingService,
            IMenuNavigationService navigationService,
            IQuitHandler quitHandler,
            ILogService logService,
            IAudioService audioService,
            ILocalizationService localizationService)
        {
            _view = view;
            _sceneService = sceneService;
            _loadingService = loadingService;
            _navigationService = navigationService;
            _quitHandler = quitHandler;
            _logService = logService;
            _audioService = audioService;
            _localizationService = localizationService;
        }

        public void Initialize()
        {
            _view.PlayClicked += OnPlayClicked;
            _view.SettingsClicked += OnSettingsClicked;
            _view.QuitClicked += OnQuitClicked;

            // 播放主菜单 BGM
            PlayMenuBGMAsync().Forget();
        }

        private async UniTaskVoid PlayMenuBGMAsync()
        {
            try
            {
                await _audioService.PlayBGMAsync(AudioKeys.BGM.Menu, fadeInDuration: 1.0f);
                _logService?.Information(LogCategory.Menu, "[主菜单] BGM 开始播放");
            }
            catch (Exception ex)
            {
                _logService?.Warning(LogCategory.Menu, $"[主菜单] BGM 播放失败：{ex.Message}");
            }
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

            // 点击音效由 UIButtonSound 组件处理
            _view.SetInteractable(false);
            _view.ShowLoadingIndicator(true);

            StartGameAsync(_view.DestroyCancellationToken).Forget();
        }

        private void OnSettingsClicked()
        {
            // 点击音效由 UIButtonSound 组件处理
            _navigationService.ShowSettingsAsync().Forget();
        }

        private void OnQuitClicked()
        {
            // 点击音效由 UIButtonSound 组件处理
            _quitHandler.RequestQuitAsync().Forget();
        }

        private async UniTask StartGameAsync(CancellationToken token)
        {
            var enterGameText = _localizationService.GetText(MenuLocalizationKeys.Loading.EnterGame);
            using var scope = _loadingService?.Begin(enterGameText);

            try
            {
                _logService?.Information(LogCategory.Menu, $"[主菜单] 开始加载场景：{_view.StartSceneKey}");
                var loadingSceneText = _localizationService.GetText(MenuLocalizationKeys.Loading.LoadingScene);
                var progress = _loadingService?.CreateProgressReporter(loadingSceneText);
                await _sceneService.LoadSceneAsync(_view.StartSceneKey, _view.UseLoadingScreen, progress, token);
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
