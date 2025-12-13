using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using UnityEngine;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 默认退出处理：直接退出应用，可替换为确认弹窗或平台特定逻辑。
    /// </summary>
    public sealed class DefaultQuitHandler : IQuitHandler
    {
        private readonly ILogService _logService;

        public DefaultQuitHandler(ILogService logService)
        {
            _logService = logService;
        }

        public UniTask RequestQuitAsync()
        {
            _logService?.Information(LogCategory.Menu, "[退出] 直接退出应用（可替换为确认弹窗）");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            return UniTask.CompletedTask;
        }
    }
}
