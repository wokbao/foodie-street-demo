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
    }
}
