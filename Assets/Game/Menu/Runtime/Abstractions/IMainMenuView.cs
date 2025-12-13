using System;
using System.Threading;

namespace Game.Menu.Runtime.Abstractions
{
    /// <summary>
    /// 主菜单 UI 视图接口。
    /// 由 Unity 场景中的 MainMenuView 组件实现，向 Presenter 提供按钮事件与状态控制。
    /// </summary>
    public interface IMainMenuView
    {
        /// <summary>
        /// “开始游戏”按钮点击事件。
        /// </summary>
        event Action PlayClicked;

        /// <summary>
        /// “设置”按钮点击事件。
        /// </summary>
        event Action SettingsClicked;

        /// <summary>
        /// “退出”按钮点击事件。
        /// </summary>
        event Action QuitClicked;

        /// <summary>
        /// 开始游戏时要加载的场景 Key（Addressables 或 Build Settings 名称）。
        /// 可在 Inspector 配置。
        /// </summary>
        string StartSceneKey { get; }

        /// <summary>
        /// 进入游戏时是否使用加载界面。
        /// 可在 Inspector 配置。
        /// </summary>
        bool UseLoadingScreen { get; }

        /// <summary>
        /// 场景卸载时用于取消异步操作的 Token。
        /// </summary>
        CancellationToken DestroyCancellationToken { get; }

        /// <summary>
        /// 控制交互状态（在加载中禁用按钮）。
        /// </summary>
        void SetInteractable(bool interactable);

        /// <summary>
        /// 控制加载指示器显示/隐藏。
        /// </summary>
        void ShowLoadingIndicator(bool visible);
    }
}
