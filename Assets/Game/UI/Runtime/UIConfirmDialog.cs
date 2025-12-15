using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用确认弹窗：遮罩 + 面板 + 确认/取消按钮，内置淡入淡出与缩放动画。
    /// </summary>
    public sealed class UIConfirmDialog : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;

        [Header("Animation")]
        [SerializeField] private float _fadeSeconds = 0.12f;
        [SerializeField] private float _scaleFrom = 0.96f;

        private Coroutine _routine;
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

            SetVisible(false, instant: true);
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

            SetVisible(true, instant: false);
        }

        public void Close()
        {
            SetVisible(false, instant: false);
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

        private void SetVisible(bool visible, bool instant)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            if (instant)
            {
                ApplyState(visible, 1f);
                return;
            }

            _routine = StartCoroutine(AnimateVisible(visible));
        }

        private IEnumerator AnimateVisible(bool visible)
        {
            ApplyState(visible, 0f);

            var duration = Mathf.Max(0.01f, _fadeSeconds);
            var t = 0f;

            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                ApplyState(visible, Mathf.Clamp01(t));
                yield return null;
            }

            ApplyState(visible, 1f);
            _routine = null;
        }

        private void ApplyState(bool visible, float progress)
        {
            if (_canvasGroup == null)
            {
                return;
            }

            var alpha = visible ? progress : (1f - progress);
            _canvasGroup.alpha = alpha;
            _canvasGroup.blocksRaycasts = visible && alpha > 0.99f;
            _canvasGroup.interactable = visible && alpha > 0.99f;

            if (_panel != null)
            {
                var from = Vector3.one * _scaleFrom;
                _panel.localScale = visible
                    ? Vector3.Lerp(from, Vector3.one, progress)
                    : Vector3.Lerp(Vector3.one, from, progress);
            }

            gameObject.SetActive(visible || alpha > 0.001f);
        }

#if UNITY_EDITOR
        public void EditorWireUp(CanvasGroup canvasGroup, RectTransform panel, TextMeshProUGUI message, Button confirmButton, Button cancelButton)
        {
            _canvasGroup = canvasGroup;
            _panel = panel;
            _message = message;
            _confirmButton = confirmButton;
            _cancelButton = cancelButton;
        }
#endif
    }
}
