using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using VContainer;
using VContainer.Unity;
using VContainer.Unity;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单导航服务：控制设置面板的开关。
    /// </summary>
    public sealed class MenuNavigationService : IMenuNavigationService
    {
        private readonly ILogService _logService;
        private readonly UIPanelWithHeader _settingsPanel;
        private bool _isSettingsOpen;

        public MenuNavigationService(
            ILogService logService,
            IObjectResolver resolver)
        {
            _logService = logService;
            resolver.TryResolve(out _settingsPanel);

            if (_settingsPanel != null)
            {
                _settingsPanel.gameObject.SetActive(false);
                _settingsPanel.CloseRequested += OnCloseRequested;
            }
        }

        public UniTask ShowSettingsAsync()
        {
            if (_settingsPanel == null)
            {
                _logService?.Warning(LogCategory.Menu, "[菜单导航] 未找到设置面板");
                return UniTask.CompletedTask;
            }

            if (_isSettingsOpen)
            {
                _logService?.Information(LogCategory.Menu, "[菜单导航] 设置面板已打开，忽略重复请求");
                return UniTask.CompletedTask;
            }

            _logService?.Information(LogCategory.Menu, "[菜单导航] 打开设置页");
            _settingsPanel.SetTitle("设置");
            _settingsPanel.gameObject.SetActive(true);
            _isSettingsOpen = true;
            return UniTask.CompletedTask;
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

        private void OnCloseRequested()
        {
            ShowMainMenuAsync().Forget();
        }
    }
}
