using Cysharp.Threading.Tasks;
using Game.UI.Runtime.Abstractions;
using UnityEngine;

namespace Game.UI.Runtime
{
    /// <summary>
    /// UI 动画实现：简单的淡入淡出 + 缩放动画。
    /// 使用 CanvasGroup 控制透明度，RectTransform 控制缩放。
    /// </summary>
    public sealed class UIAnimator : IUIAnimator
    {
        private const float AnimationDuration = 0.2f;
        private const float StartScale = 0.8f;

        /// <inheritdoc/>
        public async UniTask PlayShowAsync(GameObject ui)
        {
            if (ui == null) return;

            var cg = ui.GetComponent<CanvasGroup>();
            var rt = ui.GetComponent<RectTransform>();

            // 添加 CanvasGroup（如果没有）
            if (cg == null)
            {
                cg = ui.AddComponent<CanvasGroup>();
            }

            // 初始状态
            cg.alpha = 0f;
            if (rt != null) rt.localScale = Vector3.one * StartScale;

            // 动画
            var elapsed = 0f;
            while (elapsed < AnimationDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / AnimationDuration);
                var easeT = EaseOutBack(t);

                cg.alpha = t;
                if (rt != null) rt.localScale = Vector3.Lerp(Vector3.one * StartScale, Vector3.one, easeT);

                await UniTask.Yield();
            }

            // 确保最终状态
            cg.alpha = 1f;
            if (rt != null) rt.localScale = Vector3.one;
        }

        /// <inheritdoc/>
        public async UniTask PlayHideAsync(GameObject ui)
        {
            if (ui == null) return;

            var cg = ui.GetComponent<CanvasGroup>();
            var rt = ui.GetComponent<RectTransform>();

            if (cg == null) return; // 没有 CanvasGroup 就直接隐藏

            // 动画
            var elapsed = 0f;
            while (elapsed < AnimationDuration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / AnimationDuration);

                cg.alpha = 1f - t;
                if (rt != null) rt.localScale = Vector3.Lerp(Vector3.one, Vector3.one * StartScale, t);

                await UniTask.Yield();
            }

            // 确保最终状态
            cg.alpha = 0f;
            if (rt != null) rt.localScale = Vector3.one * StartScale;
        }

        /// <summary>
        /// Ease Out Back 缓动函数，产生轻微的回弹效果
        /// </summary>
        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
