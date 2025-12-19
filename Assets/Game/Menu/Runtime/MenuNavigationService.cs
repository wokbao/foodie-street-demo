using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using Game.UI.Runtime.Abstractions;
using VContainer;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单导航服务：控制设置面板的开关。
    /// </summary>
    public sealed class MenuNavigationService : IMenuNavigationService
    {
        private readonly ILogService _logService;
        private readonly IUIFactory _uiFactory;
        private readonly IUIRootService _uiRootService;

        private UIPanelWithHeader _settingsPanel;
        private bool _isSettingsOpen;

        public MenuNavigationService(
            ILogService logService,
            IObjectResolver resolver,
            IUIFactory uiFactory,
            IUIRootService uiRootService)
        {
            _logService = logService;
            _uiFactory = uiFactory;
            _uiRootService = uiRootService;
            resolver.TryResolve(out _settingsPanel);

            if (_settingsPanel != null)
            {
                _settingsPanel.gameObject.SetActive(false);
                _settingsPanel.CloseRequested += OnCloseRequested;
            }
        }

        public bool IsSettingsOpen => _isSettingsOpen;

        public async UniTask ShowSettingsAsync()
        {
            if (_isSettingsOpen)
            {
                _logService?.Information(LogCategory.Menu, "[菜单导航] 设置面板已打开，忽略重复请求");
                return;
            }

            if (_settingsPanel == null)
            {
                await EnsureSettingsPanelLoadedAsync();
            }

            if (_settingsPanel == null)
            {
                _logService?.Warning(LogCategory.Menu, "[菜单导航] 未找到设置面板");
                return;
            }

            _logService?.Information(LogCategory.Menu, "[菜单导航] 打开设置页");
            _settingsPanel.SetTitle("设置");
            _settingsPanel.gameObject.SetActive(true);
            _isSettingsOpen = true;
        }

        public UniTask ShowMainMenuAsync()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.gameObject.SetActive(false);
            }

            _logService?.Information(LogCategory.Menu, "[菜单导航] 返回主菜单");
            _isSettingsOpen = false;
            return UniTask.CompletedTask;
        }

        public bool TryHandleBack()
        {
            if (_isSettingsOpen)
            {
                _logService?.Information(LogCategory.Menu, "[菜单导航] 处理返回，关闭设置面板");
                ShowMainMenuAsync().Forget();
                return true;
            }

            return false;
        }

        private void OnCloseRequested()
        {
            ShowMainMenuAsync().Forget();
        }

        private async UniTask EnsureSettingsPanelLoadedAsync()
        {
            if (_settingsPanel != null)
            {
                return;
            }

            if (_uiFactory == null)
            {
                _logService?.Warning(LogCategory.Menu, "[菜单导航] UIFactory 未注册，无法实例化设置面板");
                return;
            }

            _uiRootService.EnsureInitialized();
            var parent = _uiRootService.GetLayer(UILayer.Overlay);
            if (parent == null)
            {
                _logService?.Warning(LogCategory.Menu, "[菜单导航] 未找到 Overlay 层，无法实例化设置面板");
                return;
            }

            var instance = await _uiFactory.InstantiateAsync(UIKeys.Common.PanelHeader, parent);
            if (instance == null)
            {
                _logService?.Warning(LogCategory.Menu, "[菜单导航] 实例化设置面板失败，Key={0}", UIKeys.Common.PanelHeader);
                return;
            }

            _settingsPanel = instance.GetComponentInChildren<UIPanelWithHeader>(true);
            if (_settingsPanel == null)
            {
                _logService?.Warning(LogCategory.Menu, "[菜单导航] 预制体上未找到 UIPanelWithHeader");
                UnityEngine.Object.Destroy(instance);
                return;
            }

            _settingsPanel.gameObject.SetActive(false);
            _settingsPanel.CloseRequested += OnCloseRequested;
        }
    }
}
