using System;
using Core.Feature.Loading.Abstractions;
using Game.UI.Runtime;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// 负责在游戏域内创建并挂接 Loading HUD。
    /// 配置建议：
    /// - LoadingHud 样式/排序/延迟：做成 ScriptableObject（推荐放 Assets/Game/Configs/LoadingHudConfig.asset 并打 Addressable），在构造函数或 DI 注入替换下方常量。
    /// - 首场景/过渡/全局配置：保持 StartupConfig、SceneTransitionConfig 放在 Assets/Core/Configs/，由 Core 层加载。
    /// </summary>
    public sealed class LoadingHudEntryPoint : IStartable, IDisposable
    {
        private readonly ILoadingService _loadingService;
        private readonly LoadingHudConfig _config;
        private readonly UIHierarchyConfig _uiHierarchyConfig;
        private GameObject _hudObject;
        private GameObject _globalCanvasRoot;
        private Canvas _loadingCanvas;

        public LoadingHudEntryPoint(ILoadingService loadingService, LoadingHudConfig config = null, UIHierarchyConfig uiHierarchyConfig = null)
        {
            _loadingService = loadingService;
            _config = config;
            _uiHierarchyConfig = uiHierarchyConfig != null ? uiHierarchyConfig : UIHierarchyConfig.Default;
        }

        public void Start()
        {
            EnsureLoadingCanvas();

            _hudObject = new GameObject("LoadingOverlay");
            _hudObject.transform.SetParent(_loadingCanvas.transform, false);

            var hud = _hudObject.AddComponent<LoadingHud>();
            hud.Initialize(_loadingService, _config, _loadingCanvas);
        }

        public void Dispose()
        {
            if (_hudObject != null)
            {
                UnityEngine.Object.Destroy(_hudObject);
                _hudObject = null;
            }

            _globalCanvasRoot = null;
            _loadingCanvas = null;
        }

        private void EnsureLoadingCanvas()
        {
            if (_globalCanvasRoot != null && _loadingCanvas != null)
            {
                return;
            }

            var rootName = string.IsNullOrWhiteSpace(_uiHierarchyConfig.RootName)
                ? "GlobalUIRoot"
                : _uiHierarchyConfig.RootName;

            _globalCanvasRoot = GameObject.Find(rootName);
            if (_globalCanvasRoot == null)
            {
                _globalCanvasRoot = new GameObject(rootName);
                UnityEngine.Object.DontDestroyOnLoad(_globalCanvasRoot);
            }

            // 确保根有 CanvasScaler，但根 Canvas 排序使用配置的 RootSortingOrder。
            var rootCanvas = _globalCanvasRoot.GetComponent<Canvas>();
            if (rootCanvas == null)
            {
                rootCanvas = _globalCanvasRoot.AddComponent<Canvas>();
                rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                rootCanvas.sortingOrder = _uiHierarchyConfig.RootSortingOrder;
            }
            else
            {
                rootCanvas.sortingOrder = _uiHierarchyConfig.RootSortingOrder;
            }

            var scaler = _globalCanvasRoot.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = _globalCanvasRoot.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (_globalCanvasRoot.GetComponent<GraphicRaycaster>() == null)
            {
                _globalCanvasRoot.AddComponent<GraphicRaycaster>();
            }

            // 统一创建/确保各 UI 层 Canvas（每层一个 Canvas 以便独立 sortingOrder；并复制 CanvasScaler 保持缩放一致）。
            EnsureLayerCanvas(_globalCanvasRoot.transform, scaler, _uiHierarchyConfig.MainLayerName, _uiHierarchyConfig.MainSortingOrder);
            EnsureLayerCanvas(_globalCanvasRoot.transform, scaler, _uiHierarchyConfig.HudLayerName, _uiHierarchyConfig.HudSortingOrder);
            EnsureLayerCanvas(_globalCanvasRoot.transform, scaler, _uiHierarchyConfig.OverlayLayerName, _uiHierarchyConfig.OverlaySortingOrder);
            EnsureLayerCanvas(_globalCanvasRoot.transform, scaler, _uiHierarchyConfig.TransitionLayerName, _uiHierarchyConfig.TransitionSortingOrder);

            var loadingLayer = EnsureLayerCanvas(_globalCanvasRoot.transform, scaler, _uiHierarchyConfig.LoadingLayerName, _uiHierarchyConfig.LoadingSortingOrder);
            _loadingCanvas = loadingLayer != null ? loadingLayer.GetComponent<Canvas>() : null;
        }

        private static GameObject EnsureLayerCanvas(Transform root, CanvasScaler rootScaler, string layerName, int sortingOrder)
        {
            var name = string.IsNullOrWhiteSpace(layerName) ? "Layer" : layerName;
            var layer = FindOrCreateLayer(root, name);

            var canvas = layer.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = layer.AddComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            // 每层单独 Canvas 时也需要 CanvasScaler 才能与根节点保持一致缩放。
            var scaler = layer.GetComponent<CanvasScaler>();
            if (scaler == null && rootScaler != null)
            {
                scaler = layer.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = rootScaler.uiScaleMode;
                scaler.referenceResolution = rootScaler.referenceResolution;
                scaler.screenMatchMode = rootScaler.screenMatchMode;
                scaler.matchWidthOrHeight = rootScaler.matchWidthOrHeight;
            }

            if (layer.GetComponent<GraphicRaycaster>() == null)
            {
                layer.AddComponent<GraphicRaycaster>();
            }

            return layer;
        }

        private static GameObject FindOrCreateLayer(Transform parent, string name)
        {
            var child = parent.Find(name);
            if (child != null)
            {
                return child.gameObject;
            }

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go;
        }
    }
}
