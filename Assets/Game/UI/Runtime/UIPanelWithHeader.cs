using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 带标题栏和关闭按钮的通用面板骨架，用作 Settings 等界面的容器。
    /// </summary>
    public sealed class UIPanelWithHeader : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _closeButton;
        [SerializeField] private RectTransform _contentRoot;

        public event Action CloseRequested;

        public RectTransform ContentRoot => _contentRoot;

        private void Awake()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(() => CloseRequested?.Invoke());
            }
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveAllListeners();
            }
        }

        public void SetTitle(string title)
        {
            if (_title != null)
            {
                _title.text = title;
            }
        }

#if UNITY_EDITOR
        public void EditorWireUp(TextMeshProUGUI title, Button closeButton, RectTransform contentRoot)
        {
            _title = title;
            _closeButton = closeButton;
            _contentRoot = contentRoot;
        }
#endif
    }
}
