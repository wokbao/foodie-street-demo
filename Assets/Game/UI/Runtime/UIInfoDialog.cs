using System;
using Game.UI.Runtime.Abstractions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用单按钮提示弹窗。
    /// 实现 IUICloseable，按钮点击后自动触发关闭事件。
    /// </summary>
    public sealed class UIInfoDialog : MonoBehaviour, IUICloseable
    {
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _okButton;

        private Action _onOk;

        /// <inheritdoc/>
        public event Action CloseRequested;

        private void Awake()
        {
            if (_okButton != null)
            {
                _okButton.onClick.AddListener(OnOkClicked);
            }
        }

        private void OnDestroy()
        {
            _okButton?.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 显示提示弹窗。
        /// </summary>
        /// <param name="message">提示消息</param>
        /// <param name="onOk">确认回调（可选）</param>
        public void Show(string message, Action onOk = null)
        {
            _onOk = onOk;

            if (_message != null)
            {
                _message.text = message;
            }
        }

        private void OnOkClicked()
        {
            _onOk?.Invoke();
            // 触发关闭请求，由 UIFactory 处理实际关闭流程
            CloseRequested?.Invoke();
        }

        /// <inheritdoc/>
        public void OnExternalClose()
        {
            // UIInfoDialog 是同步的单按钮弹窗，无需特殊清理
            // 外部关闭时不调用 _onOk 回调，与点击确认按钮区分
        }


#if UNITY_EDITOR
        public void EditorWireUp(TextMeshProUGUI message, Button okButton)
        {
            _message = message;
            _okButton = okButton;
        }
#endif
    }
}


