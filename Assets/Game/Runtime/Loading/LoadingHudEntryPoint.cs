using System;
using Core.Feature.Loading.Abstractions;
using Game.UI.Runtime.Abstractions;
using UnityEngine;
using VContainer.Unity;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// 负责在游戏域内创建并挂接 Loading HUD。
    /// 配置建议：
    /// - LoadingHud 样式/延迟：使用 LoadingHudConfig（推荐放 Assets/Game/Configs/LoadingHudConfig.asset 并打 Addressable）。
    /// - UI 层级/排序：使用 UIHierarchyConfig（推荐放 Assets/Game/Configs/UIHierarchyConfig.asset 并打 Addressable）。
    /// - 首场景/过渡/全局配置：保持 StartupConfig、SceneTransitionConfig 放在 Assets/Core/Configs/，由 Core 层加载。
    /// </summary>
    public sealed class LoadingHudEntryPoint : IStartable, IDisposable
    {
        private readonly ILoadingService _loadingService;
        private readonly LoadingHudConfig _config;
        private readonly IUIRootService _uiRootService;
        private GameObject _hudObject;
        private Canvas _loadingCanvas;

        public LoadingHudEntryPoint(ILoadingService loadingService, IUIRootService uiRootService, LoadingHudConfig config = null)
        {
            _loadingService = loadingService;
            _config = config;
            _uiRootService = uiRootService;
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

            _loadingCanvas = null;
        }

        private void EnsureLoadingCanvas()
        {
            if (_loadingCanvas != null)
            {
                return;
            }

            _uiRootService.EnsureInitialized();
            _loadingCanvas = _uiRootService.GetLayerCanvas(UILayer.Loading);
        }
    }
}
