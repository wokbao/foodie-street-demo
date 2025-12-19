using UnityEngine;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// Loading HUD 的可配置参数，便于按平台/主题调整。
    /// 建议放置于 Assets/Game/Configs/ 并打上 Addressable，之后在 GameLifetimeScope Inspector 引用。
    /// </summary>
    [CreateAssetMenu(fileName = "LoadingHudConfig", menuName = "Game/Loading/Loading Hud Config")]
    public sealed class LoadingHudConfig : ScriptableObject
    {
        [Header("显示控制")]
        [Tooltip("加载持续超过该延迟后才显示 HUD，避免短加载闪屏。")]
        [SerializeField] private float _showDelaySeconds = 2f;

        [Header("样式")]
        [Tooltip("遮罩颜色（默认半透明黑）。")]
        [SerializeField] private Color _overlayColor = new Color(0f, 0f, 0f, 0.55f);

        [Tooltip("Spinner 颜色。")]
        [SerializeField] private Color _spinnerColor = new Color(1f, 1f, 1f, 0.9f);

        public float ShowDelaySeconds => _showDelaySeconds;
        public Color OverlayColor => _overlayColor;
        public Color SpinnerColor => _spinnerColor;
    }
}
