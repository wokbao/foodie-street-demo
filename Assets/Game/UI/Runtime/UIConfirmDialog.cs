using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用确认弹窗：遮罩 + 面板 + 确认/取消按钮，使用 UIDialogAnimator 提供动画。
    /// </summary>
    public sealed class UIConfirmDialog : MonoBehaviour
    {
        [SerializeField] private UIDialogAnimator _animator;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

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

            _animator?.HideInstant();
        }

        private void OnDestroy()
        {
            _confirmButton?.onClick.RemoveAllListeners();
            _cancelButton?.onClick.RemoveAllListeners();
        }

        public void Show(string message, Action onConfirm, Action onCancel = null)
        {
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            if (_message != null)
            {
                _message.text = message;
            }

            _animator?.Show();
        }

        public void Close()
        {
            _animator?.Hide();
        }

        private void OnConfirmClicked()
        {
            _onConfirm?.Invoke();
            Close();
        }

        private void OnCancelClicked()
        {
            _onCancel?.Invoke();
            Close();
        }

#if UNITY_EDITOR
        public void EditorWireUp(UIDialogAnimator animator, TextMeshProUGUI message, Button confirmButton, Button cancelButton)
        {
            _animator = animator;
            _message = message;
            _confirmButton = confirmButton;
            _cancelButton = cancelButton;
        }
#endif
    }
}
