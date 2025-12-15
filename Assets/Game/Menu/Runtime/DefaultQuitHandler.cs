using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using UnityEngine;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 默认退出处理：直接退出应用，可替换为确认弹窗或平台特定逻辑。
    /// </summary>
    public sealed class DefaultQuitHandler : IQuitHandler
    {
        private readonly ILogService _logService;
        private readonly UIConfirmDialog _dialog;

        public DefaultQuitHandler(ILogService logService, UIConfirmDialog dialog)
        {
            _logService = logService;
            _dialog = dialog;
        }

        public UniTask RequestQuitAsync()
        {
            if (_dialog == null)
            {
                _logService?.Information(LogCategory.Menu, "[退出] 直接退出应用（未找到确认弹窗）");
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                return UniTask.CompletedTask;
            }

            _logService?.Information(LogCategory.Menu, "[退出] 弹出确认弹窗");
            _dialog.Show(
                "确定要退出游戏吗？",
                onConfirm: () =>
                {
                    _logService?.Information(LogCategory.Menu, "[退出] 用户确认退出");
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                },
                onCancel: () =>
                {
                    _logService?.Information(LogCategory.Menu, "[退出] 用户取消退出");
                    _dialog.Close();
                });

            return UniTask.CompletedTask;
        }
    }
}
