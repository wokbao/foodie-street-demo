using Core.Feature.Loading.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// 简易加载 HUD（纯代码生成 UI），用于临时展示进度、描述和旋转动画。
    /// </summary>
    public sealed class LoadingHud : MonoBehaviour
    {
        private ILoadingService _loadingService;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        private bool _usingExternalCanvas;
        private Slider _progressBar;
        private Text _descriptionText;
        private RectTransform _spinner;

        public void Initialize(ILoadingService loadingService, Canvas externalCanvas = null)
        {
            _loadingService = loadingService;
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
        }

        private void Update()
        {
            if (_spinner != null && _spinner.gameObject.activeSelf)
            {
                _spinner.Rotate(Vector3.forward, -180f * Time.unscaledDeltaTime);
            }
        }

        private void OnStateChanged(LoadingState state)
        {
            var active = state.IsLoading;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = active ? 1f : 0f;
                _canvasGroup.blocksRaycasts = active;
                _canvasGroup.interactable = active;
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
                _spinner.gameObject.SetActive(active);
            }
        }

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
                _canvas.sortingOrder = 5000;

                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                gameObject.AddComponent<GraphicRaycaster>();
                parentTransform = transform;
            }

            var overlay = CreatePanel(parentTransform, "Overlay", new Color(0f, 0f, 0f, 0.55f));
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
            rect.anchorMin = new Vector2(0.25f, 0.15f);
            rect.anchorMax = new Vector2(0.75f, 0.2f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var background = go.AddComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0.15f);

            var fillArea = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillArea.transform.SetParent(go.transform, false);
            var fillRect = fillArea.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var fillImage = fillArea.GetComponent<Image>();
            fillImage.color = new Color(0.2f, 0.7f, 1f, 0.9f);

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
            rect.anchorMin = new Vector2(0.2f, 0.22f);
            rect.anchorMax = new Vector2(0.8f, 0.28f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var text = go.GetComponent<Text>();
            // Unity 6+ 不再提供 Arial.ttf 内置字体，改用 LegacyRuntime.ttf
            var builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.font = builtinFont;
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 1f, 1f, 0.92f);
            text.text = "正在加载...";

            return text;
        }

        private RectTransform CreateSpinner(RectTransform parent)
        {
            var go = new GameObject("Spinner", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(40f, 40f);
            rect.anchorMin = new Vector2(0.5f, 0.65f);
            rect.anchorMax = new Vector2(0.5f, 0.65f);
            rect.anchoredPosition = Vector2.zero;

            var image = go.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.9f);

            return rect;
        }
    }
}
