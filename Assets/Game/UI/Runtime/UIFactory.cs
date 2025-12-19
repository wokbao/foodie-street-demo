using System;
using System.Collections.Generic;
using System.Threading;
using Core.Feature.AssetManagement.Runtime;
using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.UI.Runtime.Abstractions;
using UnityEngine;

namespace Game.UI.Runtime
{
    /// <summary>
    /// 简单的 UI 工厂：按 Key 加载 Addressable 预制体并实例化，带缓存与释放接口。
    /// </summary>
    public sealed class UIFactory : IUIFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly ILogService _logService;
        private readonly Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();

        public UIFactory(IAssetProvider assetProvider, ILogService logService)
        {
            _assetProvider = assetProvider;
            _logService = logService;
        }

        public UniTask PreloadAsync(string key, CancellationToken ct = default)
        {
            return LoadPrefabAsync(key, ct);
        }

        public async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, CancellationToken ct = default)
        {
            var prefab = await LoadPrefabAsync(key, ct);
            if (prefab == null)
            {
                _logService?.Error(LogCategory.Menu, $"[UIFactory] 预制体加载失败：{key}");
                return null;
            }

            var instance = UnityEngine.Object.Instantiate(prefab, parent);
            instance.name = prefab.name;
            return instance;
        }

        public void Release(string key)
        {
            if (_prefabCache.Remove(key))
            {
                _assetProvider.Release(key);
                _logService?.Information(LogCategory.Menu, $"[UIFactory] 释放 UI 资源：{key}");
            }
        }

        public void ReleaseAll()
        {
            foreach (var key in _prefabCache.Keys)
            {
                _assetProvider.Release(key);
            }

            _prefabCache.Clear();
            _logService?.Information(LogCategory.Menu, "[UIFactory] 已释放所有 UI 资源缓存");
        }

        private async UniTask<GameObject> LoadPrefabAsync(string key, CancellationToken ct)
        {
            if (_prefabCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            try
            {
                var prefab = await _assetProvider.LoadAssetAsync<GameObject>(key, ct);
                if (prefab != null)
                {
                    _prefabCache[key] = prefab;
                    _logService?.Information(LogCategory.Menu, $"[UIFactory] 已加载并缓存：{key}");
                }

                return prefab;
            }
            catch (OperationCanceledException)
            {
                _logService?.Warning(LogCategory.Menu, $"[UIFactory] 预加载取消：{key}");
                return null;
            }
            catch (Exception ex)
            {
                _logService?.Error(LogCategory.Menu, $"[UIFactory] 加载失败：{key} - {ex.Message}", ex);
                return null;
            }
        }
    }
}
