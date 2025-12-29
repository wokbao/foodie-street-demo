using System.Collections.Generic;
using Game.UI.Runtime.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 统一管理 GlobalUIRoot 与各 UI 层（Main/HUD/Overlay/Loading/Transition）。
    /// 目标：
    /// - 将“建根、建层、加 Canvas/Scaler、设 sortingOrder”等一次性初始化逻辑移出业务点击路径
    /// - 业务侧只关心“我要把 UI 挂在哪一层”
    /// </summary>
    public sealed class UIRootService : IUIRootService
    {
        private readonly UIHierarchyConfig _config;

        private GameObject _root;
        private readonly Dictionary<UILayer, Canvas> _layerCanvases = new();

        public UIRootService(UIHierarchyConfig config = null)
        {
            _config = config != null ? config : UIHierarchyConfig.Default;
        }

        public GameObject Root
        {
            get
            {
                EnsureInitialized();
                return _root;
            }
        }

        public void EnsureInitialized()
        {
            if (_root != null && !_root.Equals(null))
            {
                if (_layerCanvases.Count > 0)
                {
                    return;
                }
            }

            CreateOrFindRoot();
            EnsureLayers();
        }

        public Transform GetLayer(UILayer layer)
        {
            EnsureInitialized();
            return _layerCanvases.TryGetValue(layer, out var canvas) ? canvas.transform : null;
        }

        public Canvas GetLayerCanvas(UILayer layer)
        {
            EnsureInitialized();
            return _layerCanvases.TryGetValue(layer, out var canvas) ? canvas : null;
        }

        private void CreateOrFindRoot()
        {
            var rootName = string.IsNullOrWhiteSpace(_config.RootName) ? "GlobalUIRoot" : _config.RootName;

            _root = GameObject.Find(rootName);
            if (_root == null)
            {
                _root = new GameObject(rootName);
                UnityEngine.Object.DontDestroyOnLoad(_root);
            }

            var rootCanvas = _root.GetComponent<Canvas>();
            if (rootCanvas == null)
            {
                rootCanvas = _root.AddComponent<Canvas>();
            }
            rootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rootCanvas.overrideSorting = true;
            rootCanvas.sortingOrder = _config.RootSortingOrder;

            var scaler = _root.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = _root.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (_root.GetComponent<GraphicRaycaster>() == null)
            {
                _root.AddComponent<GraphicRaycaster>();
            }
        }

        private void EnsureLayers()
        {
            _layerCanvases.Clear();

            var rootScaler = _root.GetComponent<CanvasScaler>();

            EnsureLayer(UILayer.Main, _config.MainLayerName, _config.MainSortingOrder, rootScaler);
            EnsureLayer(UILayer.Hud, _config.HudLayerName, _config.HudSortingOrder, rootScaler);
            EnsureLayer(UILayer.Overlay, _config.OverlayLayerName, _config.OverlaySortingOrder, rootScaler);
            EnsureLayer(UILayer.Loading, _config.LoadingLayerName, _config.LoadingSortingOrder, rootScaler);
            EnsureLayer(UILayer.Transition, _config.TransitionLayerName, _config.TransitionSortingOrder, rootScaler);
        }

        private void EnsureLayer(UILayer layer, string name, int sortingOrder, CanvasScaler rootScaler)
        {
            var layerName = string.IsNullOrWhiteSpace(name) ? $"Layer_{layer}" : name;
            var layerObject = FindOrCreateLayer(_root.transform, layerName);

            var canvas = layerObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = layerObject.AddComponent<Canvas>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            var scaler = layerObject.GetComponent<CanvasScaler>();
            if (scaler == null && rootScaler != null)
            {
                scaler = layerObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = rootScaler.uiScaleMode;
                scaler.referenceResolution = rootScaler.referenceResolution;
                scaler.screenMatchMode = rootScaler.screenMatchMode;
                scaler.matchWidthOrHeight = rootScaler.matchWidthOrHeight;
            }

            if (layerObject.GetComponent<GraphicRaycaster>() == null)
            {
                layerObject.AddComponent<GraphicRaycaster>();
            }

            _layerCanvases[layer] = canvas;
        }

        private static GameObject FindOrCreateLayer(Transform parent, string name)
        {
            var child = parent.Find(name);
            if (child != null)
            {
                return child.gameObject;
            }

            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return go;
        }
    }
}

