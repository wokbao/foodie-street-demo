using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 为 UGUI 按钮提供轻量的悬停/按下缩放反馈，避免每个按钮单独配置动画。
    /// </summary>
    public sealed class UIButtonScaleFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _hoverScale = 1.02f;
        [SerializeField] private float _pressedScale = 0.98f;
        [SerializeField] private float _durationSeconds = 0.08f;

        private Vector3 _baseScale;
        private Coroutine _routine;
        private bool _hovering;
        private bool _pressed;

        private void Awake()
        {
            _baseScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hovering = true;
            UpdateTarget();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovering = false;
            _pressed = false;
            UpdateTarget();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
            UpdateTarget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
            UpdateTarget();
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            transform.localScale = _baseScale;
            _hovering = false;
            _pressed = false;
        }

        private void UpdateTarget()
        {
            var factor = _pressed ? _pressedScale : (_hovering ? _hoverScale : 1f);
            var target = _baseScale * factor;

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(TweenScale(target));
        }

        private IEnumerator TweenScale(Vector3 target)
        {
            var start = transform.localScale;
            var t = 0f;
            var duration = Mathf.Max(0.01f, _durationSeconds);

            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                transform.localScale = Vector3.Lerp(start, target, Mathf.Clamp01(t));
                yield return null;
            }

            transform.localScale = target;
            _routine = null;
        }
    }
}
