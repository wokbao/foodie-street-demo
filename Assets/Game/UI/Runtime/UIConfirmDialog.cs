using System;
using Game.UI.Runtime.Abstractions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用确认弹窗：遮罩 + 面板 + 确认/取消按钮。
    /// 实现 IUICloseable，按钮点击后自动触发关闭事件。
    /// </summary>
    public sealed class UIConfirmDialog : MonoBehaviour, IUICloseable
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

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
        }

        /// <summary>
        /// 显示确认弹窗。
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="onConfirm">确认回调</param>
        /// <param name="onCancel">取消回调（可选）</param>
        public void Show(string message, Action onConfirm, Action onCancel = null)
        {
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            if (_message != null)
            {
                _message.text = message;
            }
        }

        private void OnConfirmClicked()
        {
            _onConfirm?.Invoke();
            // 触发关闭请求，由 UIFactory 处理实际关闭流程
            CloseRequested?.Invoke();
        }

        private void OnCancelClicked()
        {
            _onCancel?.Invoke();
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


