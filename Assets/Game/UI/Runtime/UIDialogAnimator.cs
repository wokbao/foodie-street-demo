using System.Collections;
using UnityEngine;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 通用 UI 动画器：淡入淡出 + 缩放动画，供 Dialog/Panel 复用。
    /// </summary>
    public sealed class UIDialogAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;

        [Header("Animation")]
        [SerializeField] private float _fadeSeconds = 0.35f;
        [SerializeField] private float _scaleFrom = 0.85f;

        private Coroutine _routine;

        /// <summary>
        /// 显示 UI（带动画）。
        /// </summary>
        public void Show()
        {
            SetVisible(true, instant: false);
        }

        /// <summary>
        /// 隐藏 UI（带动画）。
        /// </summary>
        public void Hide()
        {
            SetVisible(false, instant: false);
        }

        /// <summary>
        /// 立即显示 UI（无动画）。
        /// </summary>
        public void ShowInstant()
        {
            SetVisible(true, instant: true);
        }

        /// <summary>
        /// 立即隐藏 UI（无动画）。
        /// </summary>
        public void HideInstant()
        {
            SetVisible(false, instant: true);
        }

        private void SetVisible(bool visible, bool instant)
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            if (visible && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            // 如果脚本还未启用或对象仍未激活，直接同步状态，避免协程报错。
            if (!isActiveAndEnabled)
            {
                ApplyState(visible, 1f);
                return;
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
        public void EditorWireUp(CanvasGroup canvasGroup, RectTransform panel)
        {
            _canvasGroup = canvasGroup;
            _panel = panel;
        }
#endif
    }
}
