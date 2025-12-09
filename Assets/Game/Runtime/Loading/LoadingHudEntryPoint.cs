using System;
using Core.Feature.Loading.Abstractions;
using UnityEngine;
using VContainer.Unity;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// 负责在游戏域内创建并挂接 Loading HUD。
    /// </summary>
    public sealed class LoadingHudEntryPoint : IStartable, IDisposable
    {
        private readonly ILoadingService _loadingService;
        private GameObject _hudObject;

        public LoadingHudEntryPoint(ILoadingService loadingService)
        {
            _loadingService = loadingService;
        }

        public void Start()
        {
            _hudObject = new GameObject("LoadingOverlay");
            UnityEngine.Object.DontDestroyOnLoad(_hudObject);

            var hud = _hudObject.AddComponent<LoadingHud>();
            hud.Initialize(_loadingService);
        }

        public void Dispose()
        {
            if (_hudObject != null)
            {
                UnityEngine.Object.Destroy(_hudObject);
                _hudObject = null;
            }
        }
    }
}
