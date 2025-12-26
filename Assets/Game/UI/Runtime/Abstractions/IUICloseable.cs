using System;

namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// 可关闭 UI 接口。
    /// 实现此接口的 UI 组件可以通过触发 CloseRequested 事件请求关闭，
    /// UIFactory 会自动监听并处理关闭流程（动画、归还对象池等）。
    /// </summary>
    public interface IUICloseable
    {
        /// <summary>
        /// UI 请求关闭事件（由按钮点击等触发）。
        /// 触发此事件后，UIFactory 会自动执行关闭流程。
        /// </summary>
        event Action CloseRequested;

        /// <summary>
        /// 通知 UI 即将被外部关闭（如 ESC 键、堆栈管理器等）。
        /// UI 可以在此方法中完成必要的清理和状态设置。
        /// </summary>
        /// <remarks>
        /// 此方法由 <see cref="IUIStackManager"/> 在 Pop 时调用，
        /// 确保 UI 组件能在外部关闭前正确完成异步操作（如设置对话框结果）。
        /// </remarks>
        void OnExternalClose();
    }
}
