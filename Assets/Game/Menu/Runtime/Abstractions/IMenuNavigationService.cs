using Cysharp.Threading.Tasks;

namespace Game.Menu.Runtime.Abstractions
{
    /// <summary>
    /// 菜单导航服务接口，用于打开设置、回到主菜单等操作。
    /// 具体实现可连接 UI 面板或路由系统。
    /// </summary>
    public interface IMenuNavigationService
    {
        /// <summary>
        /// 当前是否有设置面板打开（或导航中的页面处于打开状态）。
        /// </summary>
        bool IsSettingsOpen { get; }

        /// <summary>
        /// 打开设置界面（或跳转到设置页面）。
        /// </summary>
        UniTask ShowSettingsAsync();

        /// <summary>
        /// 回到主菜单（用于从子页面返回）。
        /// </summary>
        UniTask ShowMainMenuAsync();

        /// <summary>
        /// 处理返回/取消输入（如 Esc/手柄 B），如果有可关闭的页面则关闭并返回 true。
        /// </summary>
        bool TryHandleBack();
    }
}
