using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using Game.UI.Runtime.Abstractions;
using UnityEngine;

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
        private GameObject _settingsPanelInstance;
        private bool _isSettingsOpen;

        public MenuNavigationService(
            ILogService logService,
            IUIFactory uiFactory,
            IUIRootService uiRootService)
        {
            _logService = logService;
            _uiFactory = uiFactory;
            _uiRootService = uiRootService;
        }

        public bool IsSettingsOpen => _isSettingsOpen;

        public async UniTask ShowSettingsAsync()
        {
            if (_isSettingsOpen)
            {
                _logService?.Information(LogCategory.Menu, "设置面板已打开，忽略重复请求");
                return;
            }

            if (_uiFactory == null)
            {
                _logService?.Warning(LogCategory.Menu, "UIFactory 未注册，无法打开设置面板");
                return;
            }

            _uiRootService.EnsureInitialized();
            var parent = _uiRootService.GetLayer(UILayer.Overlay);
            if (parent == null)
            {
                _logService?.Warning(LogCategory.Menu, "未找到 Overlay 层，无法打开设置面板");
                return;
            }

            _logService?.Information(LogCategory.Menu, "打开设置页");
            _isSettingsOpen = true;

            // 使用 ShowDialogAsync 自动管理堆栈（ESC 和关闭按钮都会自动关闭）
            _settingsPanelInstance = await _uiFactory.ShowDialogAsync(
                UIKeys.Common.PanelHeader,
                parent,
                onClose: () =>
                {
                    _logService?.Debug(LogCategory.Menu, "设置面板已关闭");
                    _isSettingsOpen = false;
                    _settingsPanel = null;
                    _settingsPanelInstance = null;
                });

            if (_settingsPanelInstance == null)
            {
                _logService?.Warning(LogCategory.Menu, "实例化设置面板失败");
                _isSettingsOpen = false;
                return;
            }

            _settingsPanel = _settingsPanelInstance.GetComponentInChildren<UIPanelWithHeader>(true);
            if (_settingsPanel == null)
            {
                _logService?.Warning(LogCategory.Menu, "预制体上未找到 UIPanelWithHeader");
                _isSettingsOpen = false;
                Object.Destroy(_settingsPanelInstance);
                _settingsPanelInstance = null;
                return;
            }

            _settingsPanel.SetTitle("设置");
            // 不需要手动订阅 CloseRequested，UIFactory 会自动处理
        }

        public UniTask ShowMainMenuAsync()
        {
            _logService?.Information(LogCategory.Menu, "返回主菜单");
            _isSettingsOpen = false;
            return UniTask.CompletedTask;
        }

        public bool TryHandleBack()
        {
            // 由 UIStackManager 统一处理返回键
            return false;
        }
    }
}



