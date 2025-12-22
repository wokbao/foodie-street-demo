using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// UI 动画接口，用于弹窗的打开/关闭动画。
    /// </summary>
    public interface IUIAnimator
    {
        /// <summary>
        /// 播放打开动画（淡入 + 缩放）
        /// </summary>
        UniTask PlayShowAsync(GameObject ui);

        /// <summary>
        /// 播放关闭动画（淡出 + 缩小）
        /// </summary>
        UniTask PlayHideAsync(GameObject ui);
    }
}
