using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单导航服务：控制设置面板的开关。
    /// </summary>
    public sealed class MenuNavigationService : IMenuNavigationService
    {
        private readonly ILogService _logService;
        private readonly UIPanelWithHeader _settingsPanel;

        public MenuNavigationService(ILogService logService, UIPanelWithHeader settingsPanel)
        {
            _logService = logService;
            _settingsPanel = settingsPanel;

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

            _logService?.Information(LogCategory.Menu, "[菜单导航] 打开设置页");
            _settingsPanel.SetTitle("设置");
            _settingsPanel.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public UniTask ShowMainMenuAsync()
        {
            if (_settingsPanel != null)
            {
                _settingsPanel.gameObject.SetActive(false);
            }

            _logService?.Information(LogCategory.Menu, "[菜单导航] 返回主菜单");
            return UniTask.CompletedTask;
        }

        private void OnCloseRequested()
        {
            ShowMainMenuAsync().Forget();
        }
    }
}
