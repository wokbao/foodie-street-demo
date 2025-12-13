using System;
using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单导航服务默认实现：当前仅记录日志，预留给后续 UI 路由/设置面板接入。
    /// </summary>
    public sealed class MenuNavigationService : IMenuNavigationService
    {
        private readonly ILogService _logService;

        public MenuNavigationService(ILogService logService)
        {
            _logService = logService;
        }

        public UniTask ShowSettingsAsync()
        {
            _logService?.Information(LogCategory.Menu, "[菜单导航] 打开设置页（待接入实际 UI）");
            return UniTask.CompletedTask;
        }

        public UniTask ShowMainMenuAsync()
        {
            _logService?.Information(LogCategory.Menu, "[菜单导航] 返回主菜单（待接入实际 UI）");
            return UniTask.CompletedTask;
        }
    }
}
