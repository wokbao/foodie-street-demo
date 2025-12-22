using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using Game.UI.Runtime.Abstractions;
using UnityEngine;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 默认退出处理：弹出确认弹窗，可替换为平台特定逻辑。
    /// </summary>
    public sealed class DefaultQuitHandler : IQuitHandler
    {
        private readonly ILogService _logService;
        private readonly IUIFactory _uiFactory;
        private readonly IUIRootService _uiRootService;

        private bool _isDialogOpen;

        public DefaultQuitHandler(
            ILogService logService,
            IUIFactory uiFactory,
            IUIRootService uiRootService)
        {
            _logService = logService;
            _uiFactory = uiFactory;
            _uiRootService = uiRootService;
        }

        public async UniTask RequestQuitAsync()
        {
            // 防止重复打开
            if (_isDialogOpen)
            {
                _logService?.Debug(LogCategory.Menu, "退出弹窗已打开，忽略重复请求");
                return;
            }

            if (_uiFactory == null)
            {
                _logService?.Warning(LogCategory.Menu, "UIFactory 未注册，直接退出");
                QuitApplication();
                return;
            }

            _uiRootService.EnsureInitialized();
            var parent = _uiRootService.GetLayer(UILayer.Overlay);
            if (parent == null)
            {
                _logService?.Warning(LogCategory.Menu, "未找到 Overlay 层，直接退出");
                QuitApplication();
                return;
            }

            _isDialogOpen = true;
            _logService?.Information(LogCategory.Menu, "弹出退出确认弹窗");

            // 使用 ShowDialogAsync 自动管理堆栈（ESC 和按钮点击都会自动关闭）
            var instance = await _uiFactory.ShowDialogAsync(
                UIKeys.Common.DialogConfirm,
                parent,
                onClose: () =>
                {
                    _logService?.Debug(LogCategory.Menu, "退出弹窗已关闭");
                    _isDialogOpen = false;
                });

            if (instance == null)
            {
                _logService?.Warning(LogCategory.Menu, "实例化确认弹窗失败，直接退出");
                _isDialogOpen = false;
                QuitApplication();
                return;
            }

            var dialog = instance.GetComponentInChildren<UIConfirmDialog>(true);
            if (dialog == null)
            {
                _logService?.Warning(LogCategory.Menu, "预制体上未找到 UIConfirmDialog，直接退出");
                _isDialogOpen = false;
                Object.Destroy(instance);
                QuitApplication();
                return;
            }

            // 只关心业务逻辑，不关心关闭细节（框架自动处理）
            dialog.Show(
                "确定要退出游戏吗？",
                onConfirm: () =>
                {
                    _logService?.Information(LogCategory.Menu, "用户确认退出");
                    QuitApplication();
                },
                onCancel: () =>
                {
                    _logService?.Information(LogCategory.Menu, "用户取消退出");
                    // 不需要手动关闭，IUICloseable 会自动触发关闭流程
                });
        }

        private void QuitApplication()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}




