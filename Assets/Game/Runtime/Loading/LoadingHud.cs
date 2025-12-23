using System;
using System.Threading;
using Core.Feature.Loading.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Runtime.Loading.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// 简易加载 HUD（纯代码生成 UI），实现 ILoadingView 接口。
    /// 用于临时展示进度、描述和旋转动画。
    /// 如需美术版样式，可创建实现 ILoadingView 的预制体版本。
    /// </summary>
    public sealed class LoadingHud : MonoBehaviour, ILoadingView
    {
        private const int DefaultSortingOrder = 8000;

        private LoadingHudConfig _config;
        private ILoadingService _loadingService;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        private bool _usingExternalCanvas;
        private Slider _progressBar;
        private Text _descriptionText;
        private RectTransform _spinner;
        private CancellationTokenSource _showCts;
        private CancellationTokenSource _fadeCts;
        private bool _isVisible;

        public bool IsVisible => _isVisible;

        public void Initialize(ILoadingService loadingService, LoadingHudConfig config = null, Canvas externalCanvas = null)
        {
            _loadingService = loadingService ?? throw new ArgumentNullException(nameof(loadingService), "加载服务不能为 null");
            _config = config;
            _usingExternalCanvas = externalCanvas != null;
            _canvas = externalCanvas;

            BuildVisualsIfNeeded();

            _loadingService.OnStateChanged += OnStateChanged;
            OnStateChanged(_loadingService.State);
        }

        private void OnDestroy()
        {
            if (_loadingService != null)
            {
                _loadingService.OnStateChanged -= OnStateChanged;
            }

            _showCts?.Cancel();
            _showCts?.Dispose();
            _showCts = null;

            _fadeCts?.Cancel();
            _fadeCts?.Dispose();
            _fadeCts = null;

            HideInstant();
        }

        private void Update()
        {
            if (_spinner != null && _spinner.gameObject.activeSelf)
            {
                float rotationSpeed = _config?.SpinnerRotationSpeed ?? 180f;
                _spinner.Rotate(Vector3.forward, -rotationSpeed * Time.unscaledDeltaTime);
            }
        }

        private void OnStateChanged(LoadingState state)
        {
            // 只在 ShouldShowUi 为 true (有前台阻塞任务) 时才显示 HUD
            var active = state.ShouldShowUi;

            if (active)
            {
                if (_showCts == null || _showCts.IsCancellationRequested)
                {
                    _showCts?.Dispose();
                    _showCts = new CancellationTokenSource();
                    ShowWithDelayAsync(_showCts.Token).Forget();
                }
            }
            else
            {
                _showCts?.Cancel();
                _showCts?.Dispose();
                _showCts = null;

                HideWithFadeAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }

            if (_progressBar != null)
            {
                _progressBar.value = state.Progress;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = string.IsNullOrEmpty(state.Description)
                    ? "正在加载..."
                    : state.Description;
            }

            if (_spinner != null)
            {
                _spinner.gameObject.SetActive(active && (_canvasGroup?.alpha ?? 0f) > 0f);
            }
        }

        #region ILoadingView Implementation

        public void SetProgress(float progress)
        {
            if (_progressBar != null)
            {
                _progressBar.value = Mathf.Clamp01(progress);
            }
        }

        public void SetDescription(string description)
        {
            if (_descriptionText != null)
            {
                _descriptionText.text = string.IsNullOrEmpty(description) ? "正在加载..." : description;
            }
        }

        public void Show()
        {
            if (_showCts == null || _showCts.IsCancellationRequested)
            {
                _showCts?.Dispose();
                _showCts = new CancellationTokenSource();
                ShowWithDelayAsync(_showCts.Token).Forget();
            }
        }

        public void Hide()
        {
            _showCts?.Cancel();
            _showCts?.Dispose();
            _showCts = null;

            HideWithFadeAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void Dispose()
        {
            if (_loadingService != null)
            {
                _loadingService.OnStateChanged -= OnStateChanged;
            }

            _showCts?.Cancel();
            _showCts?.Dispose();
            _showCts = null;

            _fadeCts?.Cancel();
            _fadeCts?.Dispose();
            _fadeCts = null;

            HideInstant();

            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        #endregion


        private void BuildVisualsIfNeeded()
        {
            if (_canvasGroup != null)
            {
                return;
            }

            Transform parentTransform;

            if (_usingExternalCanvas && _canvas != null)
            {
                parentTransform = _canvas.transform;
            }
            else
            {
                _canvas = gameObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = DefaultSortingOrder;

                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                gameObject.AddComponent<GraphicRaycaster>();
                parentTransform = transform;
            }

            var overlayColor = _config?.OverlayColor ?? new Color(0f, 0f, 0f, 0.55f);
            var overlay = CreatePanel(parentTransform, "Overlay", overlayColor);
            _canvasGroup = overlay.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            var root = overlay.GetComponent<RectTransform>();

            _spinner = CreateSpinner(root);
            _descriptionText = CreateDescription(root);
            _progressBar = CreateProgressBar(root);
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);

            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = panel.GetComponent<Image>();
            image.color = color;

            return panel;
        }

        private Slider CreateProgressBar(RectTransform parent)
        {
            var go = new GameObject("ProgressBar", typeof(RectTransform), typeof(Slider));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = _config?.ProgressBarAnchorMin ?? new Vector2(0.25f, 0.15f);
            rect.anchorMax = _config?.ProgressBarAnchorMax ?? new Vector2(0.75f, 0.2f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var background = go.AddComponent<Image>();
            background.color = _config?.ProgressBarBackgroundColor ?? new Color(1f, 1f, 1f, 0.15f);

            var fillArea = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillArea.transform.SetParent(go.transform, false);
            var fillRect = fillArea.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var fillImage = fillArea.GetComponent<Image>();
            fillImage.color = _config?.ProgressBarFillColor ?? new Color(0.2f, 0.7f, 1f, 0.9f);

            var slider = go.GetComponent<Slider>();
            slider.targetGraphic = background;
            slider.fillRect = fillRect;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;

            return slider;
        }

        private Text CreateDescription(RectTransform parent)
        {
            var go = new GameObject("Description", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = _config?.DescriptionAnchorMin ?? new Vector2(0.2f, 0.22f);
            rect.anchorMax = _config?.DescriptionAnchorMax ?? new Vector2(0.8f, 0.28f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var text = go.GetComponent<Text>();
            // Unity 6+ 不再提供 Arial.ttf 内置字体，改用 LegacyRuntime.ttf
            var builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.font = builtinFont;
            text.fontSize = _config?.DescriptionFontSize ?? 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = _config?.DescriptionTextColor ?? new Color(1f, 1f, 1f, 0.92f);
            text.text = "正在加载...";

            return text;
        }

        private RectTransform CreateSpinner(RectTransform parent)
        {
            var go = new GameObject("Spinner", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = _config?.SpinnerSize ?? new Vector2(40f, 40f);
            var anchor = _config?.SpinnerAnchor ?? new Vector2(0.5f, 0.65f);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.anchoredPosition = Vector2.zero;

            var image = go.GetComponent<Image>();
            image.color = _config?.SpinnerColor ?? new Color(1f, 1f, 1f, 0.9f);

            return rect;
        }

        private async UniTask ShowWithDelayAsync(CancellationToken ct)
        {
            try
            {
                // 默认 0.1 秒，避免加载过快时UI闪烁，同时确保能显示
                float delay = _config?.ShowDelaySeconds ?? 0.1f;
                await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: true, cancellationToken: ct);

                if (ct.IsCancellationRequested) return;
                if (_loadingService != null && !_loadingService.State.IsLoading) return;

                await FadeToAsync(1f, _config?.FadeInDuration ?? 0.3f, ct);

                if (ct.IsCancellationRequested) return;

                if (_canvasGroup != null)
                {
                    _canvasGroup.blocksRaycasts = true;
                    _canvasGroup.interactable = true;
                }

                if (_spinner != null)
                {
                    _spinner.gameObject.SetActive(true);
                }

                _isVisible = true;
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不记录错误
                _isVisible = false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"显示加载 HUD 时发生错误：{ex.Message}");
                _isVisible = false;
            }
        }

        private async UniTask HideWithFadeAsync(CancellationToken ct)
        {
            try
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.blocksRaycasts = false;
                    _canvasGroup.interactable = false;
                }

                await FadeToAsync(0f, _config?.FadeOutDuration ?? 0.2f, ct);

                if (ct.IsCancellationRequested) return;

                if (_spinner != null)
                {
                    _spinner.gameObject.SetActive(false);
                }

                _isVisible = false;
            }
            catch (OperationCanceledException)
            {
                // 正常取消，立即隐藏
                HideInstant();
                _isVisible = false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"隐藏加载 HUD 时发生错误：{ex.Message}");
                HideInstant();
                _isVisible = false;
            }
        }

        private async UniTask FadeToAsync(float targetAlpha, float duration, CancellationToken ct)
        {
            if (_canvasGroup == null) return;
            if (duration <= 0f)
            {
                _canvasGroup.alpha = targetAlpha;
                return;
            }

            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (ct.IsCancellationRequested) return;

                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            _canvasGroup.alpha = targetAlpha;
        }

        private void HideInstant()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }

            if (_spinner != null)
            {
                _spinner.gameObject.SetActive(false);
            }

            _isVisible = false;
        }
    }
}
