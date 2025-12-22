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

        [Header("动画")]
        [Tooltip("淡入动画时长（秒）。")]
        [SerializeField] private float _fadeInDuration = 0.3f;

        [Tooltip("淡出动画时长（秒）。")]
        [SerializeField] private float _fadeOutDuration = 0.2f;

        [Tooltip("Spinner 旋转速度（度/秒）。")]
        [SerializeField] private float _spinnerRotationSpeed = 180f;

        [Header("样式")]
        [Tooltip("遮罩颜色（默认半透明黑）。")]
        [SerializeField] private Color _overlayColor = new Color(0f, 0f, 0f, 0.55f);

        [Tooltip("Spinner 颜色。")]
        [SerializeField] private Color _spinnerColor = new Color(1f, 1f, 1f, 0.9f);

        [Tooltip("进度条填充颜色。")]
        [SerializeField] private Color _progressBarFillColor = new Color(0.2f, 0.7f, 1f, 0.9f);

        [Tooltip("进度条背景颜色。")]
        [SerializeField] private Color _progressBarBackgroundColor = new Color(1f, 1f, 1f, 0.15f);

        [Header("布局 - 进度条")]
        [Tooltip("进度条锚点最小值（相对父容器 0-1）。")]
        [SerializeField] private Vector2 _progressBarAnchorMin = new Vector2(0.25f, 0.15f);

        [Tooltip("进度条锚点最大值（相对父容器 0-1）。")]
        [SerializeField] private Vector2 _progressBarAnchorMax = new Vector2(0.75f, 0.2f);

        [Header("布局 - 描述文字")]
        [Tooltip("描述文字锚点最小值（相对父容器 0-1）。")]
        [SerializeField] private Vector2 _descriptionAnchorMin = new Vector2(0.2f, 0.22f);

        [Tooltip("描述文字锚点最大值（相对父容器 0-1）。")]
        [SerializeField] private Vector2 _descriptionAnchorMax = new Vector2(0.8f, 0.28f);

        [Tooltip("描述文字字号。")]
        [SerializeField] private int _descriptionFontSize = 20;

        [Tooltip("描述文字颜色。")]
        [SerializeField] private Color _descriptionTextColor = new Color(1f, 1f, 1f, 0.92f);

        [Header("布局 - Spinner")]
        [Tooltip("Spinner 尺寸（像素）。")]
        [SerializeField] private Vector2 _spinnerSize = new Vector2(40f, 40f);

        [Tooltip("Spinner 锚点位置（相对父容器 0-1）。")]
        [SerializeField] private Vector2 _spinnerAnchor = new Vector2(0.5f, 0.65f);

        [Header("调试")]
        [Tooltip("显示调试信息（FPS、加载时间等）。")]
        [SerializeField] private bool _showDebugInfo = false;

        // 显示控制
        public float ShowDelaySeconds => _showDelaySeconds;

        // 动画
        public float FadeInDuration => _fadeInDuration;
        public float FadeOutDuration => _fadeOutDuration;
        public float SpinnerRotationSpeed => _spinnerRotationSpeed;

        // 样式
        public Color OverlayColor => _overlayColor;
        public Color SpinnerColor => _spinnerColor;
        public Color ProgressBarFillColor => _progressBarFillColor;
        public Color ProgressBarBackgroundColor => _progressBarBackgroundColor;

        // 布局 - 进度条
        public Vector2 ProgressBarAnchorMin => _progressBarAnchorMin;
        public Vector2 ProgressBarAnchorMax => _progressBarAnchorMax;

        // 布局 - 描述文字
        public Vector2 DescriptionAnchorMin => _descriptionAnchorMin;
        public Vector2 DescriptionAnchorMax => _descriptionAnchorMax;
        public int DescriptionFontSize => _descriptionFontSize;
        public Color DescriptionTextColor => _descriptionTextColor;

        // 布局 - Spinner
        public Vector2 SpinnerSize => _spinnerSize;
        public Vector2 SpinnerAnchor => _spinnerAnchor;

        // 调试
        public bool ShowDebugInfo => _showDebugInfo;
    }
}
