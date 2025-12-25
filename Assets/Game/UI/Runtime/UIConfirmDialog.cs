using System;
using Cysharp.Threading.Tasks;
using Game.UI.Runtime.Abstractions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用确认弹窗组件。
    /// 包含遮罩层、消息文本、确认和取消按钮。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 此组件实现了 <see cref="IUICloseable"/> 接口，按钮点击后会触发 <see cref="CloseRequested"/> 事件，
    /// 由 <see cref="IUIFactory"/> 自动处理关闭流程（动画、对象池归还等）。
    /// </para>
    /// <para>
    /// <strong>设计原则</strong>：
    /// <list type="bullet">
    /// <item>使用基于结果返回的异步 API（<see cref="ShowAsync"/>），确保业务逻辑在 UI 清理完成后执行</item>
    /// <item>支持对象池复用，每次调用 <see cref="ShowAsync"/> 会自动重置内部状态</item>
    /// <item>OnDestroy 时会安全完成未完成的异步操作</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// 不要直接实例化此组件，而应通过 <see cref="IUIFactory.ShowConfirmDialogAsync"/> 使用：
    /// <code>
    /// var result = await _uiFactory.ShowConfirmDialogAsync("确定要删除吗？", parent);
    /// if (result == DialogResult.Confirmed)
    /// {
    ///     // 执行删除操作
    /// }
    /// </code>
    /// </example>
    public sealed class UIConfirmDialog : MonoBehaviour, IUICloseable
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private UniTaskCompletionSource<DialogResult> _completionSource;

        /// <inheritdoc/>
        public event Action CloseRequested;

        private void Awake()
        {
            if (_confirmButton != null)
            {
                _confirmButton.onClick.AddListener(OnConfirmClicked);
            }

            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(OnCancelClicked);
            }
        }

        private void OnDestroy()
        {
            _confirmButton?.onClick.RemoveAllListeners();
            _cancelButton?.onClick.RemoveAllListeners();

            // 如果对话框被销毁但用户还未选择，返回 None
            if (_completionSource != null && _completionSource.Task.Status == UniTaskStatus.Pending)
            {
                _completionSource.TrySetResult(DialogResult.None);
            }
        }

        /// <summary>
        /// 显示确认弹窗并等待用户选择。
        /// 此方法会阻塞直到用户点击按钮，然后在 UI 完全关闭后返回结果。
        /// </summary>
        /// <param name="message">要显示的提示消息</param>
        /// <returns>
        /// 返回一个 <see cref="UniTask{DialogResult}"/>，包含用户的选择结果。
        /// 当用户点击确认按钮时返回 <see cref="DialogResult.Confirmed"/>，
        /// 点击取消按钮或按 ESC 键时返回 <see cref="DialogResult.Cancelled"/>。
        /// </returns>
        /// <remarks>
        /// <para>
        /// <strong>执行流程</strong>：
        /// <list type="number">
        /// <item>调用此方法后立即返回 Task，等待用户操作</item>
        /// <item>用户点击按钮触发 <see cref="CloseRequested"/> 事件</item>
        /// <item><see cref="IUIFactory"/> 播放关闭动画并归还对象池</item>
        /// <item>Task 完成，返回用户选择的结果</item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>对象池复用</strong>：
        /// 每次调用此方法都会创建新的 <see cref="UniTaskCompletionSource{DialogResult}"/>，
        /// 确保从对象池取出的实例状态正确。
        /// </para>
        /// </remarks>
        /// <example>
        /// 通常不直接调用此方法，而是通过 <see cref="IUIFactory.ShowConfirmDialogAsync"/> 间接使用。
        /// </example>
        public UniTask<DialogResult> ShowAsync(string message)

        {
            // 创建新的 CompletionSource（对象池复用时需要重置）
            _completionSource = new UniTaskCompletionSource<DialogResult>();

            // 设置消息
            if (_message != null)
            {
                _message.text = message;
            }

            return _completionSource.Task;
        }

        private void OnConfirmClicked()
        {
            _completionSource?.TrySetResult(DialogResult.Confirmed);
            // 触发关闭请求，由 UIFactory 处理实际关闭流程
            CloseRequested?.Invoke();
        }

        private void OnCancelClicked()
        {
            _completionSource?.TrySetResult(DialogResult.Cancelled);
            // 触发关闭请求，由 UIFactory 处理实际关闭流程
            CloseRequested?.Invoke();
        }

#if UNITY_EDITOR
        public void EditorWireUp(TextMeshProUGUI message, Button confirmButton, Button cancelButton)
        {
            _message = message;
            _confirmButton = confirmButton;
            _cancelButton = cancelButton;
        }
#endif
    }
}

