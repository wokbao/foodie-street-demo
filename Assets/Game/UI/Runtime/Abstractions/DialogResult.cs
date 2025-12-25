namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// 对话框用户选择结果。
    /// 用于表示用户在确认对话框中的选择，确保类型安全的结果判断。
    /// </summary>
    /// <remarks>
    /// 此枚举用于 <see cref="IUIFactory.ShowConfirmDialogAsync"/> 等方法的返回值，
    /// 替代传统的布尔值或字符串判断，提供更清晰的语义和编译时检查。
    /// </remarks>
    /// <example>
    /// 典型使用场景：
    /// <code>
    /// var result = await _uiFactory.ShowConfirmDialogAsync("确定要退出吗？", parent);
    /// if (result == DialogResult.Confirmed)
    /// {
    ///     Application.Quit();
    /// }
    /// </code>
    /// </example>
    public enum DialogResult
    {
        /// <summary>
        /// 未选择或对话框被程序关闭。
        /// 通常表示异常情况，例如对话框被强制销毁或加载失败。
        /// </summary>
        None = 0,

        /// <summary>
        /// 用户点击了确认按钮。
        /// 表示用户明确同意执行操作。
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// 用户点击了取消按钮或按 ESC 键关闭。
        /// 表示用户拒绝执行操作或主动关闭对话框。
        /// </summary>
        Cancelled = 2
    }
}
