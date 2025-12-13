using System;
using Core.Feature.Loading.Abstractions;
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
        private const int DefaultSortingOrder = 8000;

        private readonly ILoadingService _loadingService;
        private readonly LoadingHudConfig _config;
        private GameObject _hudObject;
        private GameObject _globalCanvasRoot;
        private Canvas _globalCanvas;

        public LoadingHudEntryPoint(ILoadingService loadingService, LoadingHudConfig config = null)
        {
            _loadingService = loadingService;
            _config = config;
        }

        public void Start()
        {
            EnsureGlobalCanvas();

            _hudObject = new GameObject("LoadingOverlay");
            _hudObject.transform.SetParent(_globalCanvasRoot.transform, false);

            var hud = _hudObject.AddComponent<LoadingHud>();
            hud.Initialize(_loadingService, _config, _globalCanvas);
        }

        public void Dispose()
        {
            if (_hudObject != null)
            {
                UnityEngine.Object.Destroy(_hudObject);
                _hudObject = null;
            }

            if (_globalCanvasRoot != null)
            {
                UnityEngine.Object.Destroy(_globalCanvasRoot);
                _globalCanvasRoot = null;
                _globalCanvas = null;
            }
        }

        private void EnsureGlobalCanvas()
        {
            if (_globalCanvasRoot != null && _globalCanvas != null)
            {
                return;
            }

            _globalCanvasRoot = GameObject.Find("GlobalUIRoot");
            if (_globalCanvasRoot == null)
            {
                _globalCanvasRoot = new GameObject("GlobalUIRoot");
                UnityEngine.Object.DontDestroyOnLoad(_globalCanvasRoot);

                _globalCanvas = _globalCanvasRoot.AddComponent<Canvas>();
                _globalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _globalCanvas.sortingOrder = _config != null ? _config.SortingOrder : DefaultSortingOrder;

                var scaler = _globalCanvasRoot.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;

                _globalCanvasRoot.AddComponent<GraphicRaycaster>();
            }
            else
            {
                _globalCanvas = _globalCanvasRoot.GetComponent<Canvas>();
                if (_globalCanvas == null)
                {
                    _globalCanvas = _globalCanvasRoot.AddComponent<Canvas>();
                    _globalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    _globalCanvas.sortingOrder = _config != null ? _config.SortingOrder : DefaultSortingOrder;
                }
                else
                {
                    _globalCanvas.sortingOrder = _config != null ? _config.SortingOrder : DefaultSortingOrder;
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

                UnityEngine.Object.DontDestroyOnLoad(_globalCanvasRoot);
            }
        }
    }
}
