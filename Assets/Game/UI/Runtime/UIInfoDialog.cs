using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用单按钮提示弹窗，使用 UIDialogAnimator 提供动画。
    /// </summary>
    public sealed class UIInfoDialog : MonoBehaviour
    {
        [SerializeField] private UIDialogAnimator _animator;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _okButton;

        private Action _onOk;

        private void Awake()
        {
            if (_okButton != null)
            {
                _okButton.onClick.AddListener(OnOkClicked);
            }

            _animator?.HideInstant();
        }

        private void OnDestroy()
        {
            _okButton?.onClick.RemoveAllListeners();
        }

        public void Show(string message, Action onOk = null)
        {
            _onOk = onOk;

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

        private void OnOkClicked()
        {
            _onOk?.Invoke();
            Close();
        }

#if UNITY_EDITOR
        public void EditorWireUp(UIDialogAnimator animator, TextMeshProUGUI message, Button okButton)
        {
            _animator = animator;
            _message = message;
            _okButton = okButton;
        }
#endif
    }
}
