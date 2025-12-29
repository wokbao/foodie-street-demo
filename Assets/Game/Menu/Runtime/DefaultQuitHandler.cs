using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
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

            // 使用 ShowConfirmDialogAsync 获取用户选择，UI 关闭后才会返回结果
            var result = await _uiFactory.ShowConfirmDialogAsync("确定要退出游戏吗？", parent);

            _isDialogOpen = false;

            // 根据用户选择执行对应逻辑（此时 UI 已安全关闭，所有服务仍可用）
            if (result == DialogResult.Confirmed)
            {
                _logService?.Information(LogCategory.Menu, "用户确认退出");
                QuitApplication();
            }
            else
            {
                _logService?.Information(LogCategory.Menu, $"用户取消退出（结果：{result}）");
            }
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




