using Game.UI.Runtime.Abstractions;
using VContainer.Unity;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 游戏启动时初始化 GlobalUIRoot 与各 UI 层 Canvas，避免业务首次打开 UI 时触发层级/组件竞态问题。
    /// </summary>
    public sealed class UIRootEntryPoint : IStartable
    {
        private readonly IUIRootService _uiRootService;

        public UIRootEntryPoint(IUIRootService uiRootService)
        {
            _uiRootService = uiRootService;
        }

        public void Start()
        {
            _uiRootService.EnsureInitialized();
        }
    }
}

