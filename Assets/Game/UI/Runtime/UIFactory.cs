using System;
using System.Collections.Generic;
using System.Threading;
using Core.Feature.AssetManagement.Runtime;
using Core.Feature.Logging.Abstractions;
using Core.Feature.ObjectPooling.Abstractions;
using Cysharp.Threading.Tasks;
using Game.UI.Runtime.Abstractions;
using UnityEngine;

namespace Game.UI.Runtime
{
    /// <summary>
    /// UI 工厂：按 Key 加载 Addressable 预制体并实例化，支持对象池复用。
    /// </summary>
    public sealed class UIFactory : IUIFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly ILogService _logService;
        private readonly IUIStackManager _uiStackManager;
        private readonly IObjectPoolManager _poolManager;
        private readonly IUIAnimator _animator;

        private readonly Dictionary<string, GameObject> _prefabCache = new();
        // 记录实例对应的 Key，用于归还时找到正确的池
        private readonly Dictionary<GameObject, string> _instanceKeyMap = new();

        public IReadOnlyCollection<string> CachedKeys => _prefabCache.Keys;

        public UIFactory(
            IAssetProvider assetProvider,
            ILogService logService,
            IUIStackManager uiStackManager,
            IObjectPoolManager poolManager,
            IUIAnimator animator)
        {
            _assetProvider = assetProvider;
            _logService = logService;
            _uiStackManager = uiStackManager;
            _poolManager = poolManager;
            _animator = animator;
        }

        public UniTask PreloadAsync(string key, CancellationToken ct = default)
        {
            return LoadPrefabAsync(key, ct);
        }

        public async UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, IProgress<float> progress = null, CancellationToken ct = default)
        {
            progress?.Report(0f);
            var prefab = await LoadPrefabAsync(key, ct);
            if (prefab == null)
            {
                _logService?.Error(LogCategory.UI, $"预制体加载失败：{key}");
                return null;
            }

            // 从对象池获取实例
            var instance = _poolManager.Rent(
                prefab,
                parent,
                worldPositionStays: false,
                onReset: ResetUIInstance,
                maxCapacity: 5);

            instance.name = prefab.name;
            instance.SetActive(true);

            // 记录实例对应的 Key
            _instanceKeyMap[instance] = key;

            progress?.Report(1f);
            _logService?.Debug(LogCategory.UI, $"从对象池获取 UI：{key}");
            return instance;
        }

        public void Release(string key)
        {
            if (_prefabCache.Remove(key))
            {
                TryReleaseSafely(key);
                TryLogInfo($"释放 UI 资源：{key}");
            }
        }

        public async UniTask<GameObject> ShowDialogAsync(string key, Transform parent = null, Action onClose = null, CancellationToken ct = default)
        {
            var instance = await InstantiateAsync(key, parent, null, ct);
            if (instance == null)
            {
                return null;
            }

            // 如果 UI 实现了 IUICloseable 接口，自动订阅关闭事件
            var closeable = instance.GetComponentInChildren<IUICloseable>(true);
            Action closeSubscription = null;

            if (closeable != null)
            {
                // 定义关闭处理委托
                closeSubscription = () =>
                {
                    _uiStackManager.Pop();
                };
                closeable.CloseRequested += closeSubscription;
            }

            // 自动加入堆栈，ESC 关闭时先播放动画再归还对象池
            _uiStackManager.Push(instance, () =>
            {
                // 确保在 UI 关闭（无论是自动还是手动 Pop）时取消订阅，防止对象池复用导致事件残留
                if (closeable != null && closeSubscription != null)
                {
                    closeable.CloseRequested -= closeSubscription;
                }

                HandleDialogCloseAsync(instance, onClose).Forget();
            });

            // 播放打开动画
            await _animator.PlayShowAsync(instance);

            _logService?.Debug(LogCategory.UI, $"弹窗已加入堆栈：{key}");
            return instance;
        }

        /// <summary>
        /// 将 UI 实例归还对象池（隐藏而非销毁）
        /// </summary>
        public void ReturnToPool(GameObject instance)
        {
            if (instance == null) return;

            // 找到对应的 Key
            if (!_instanceKeyMap.TryGetValue(instance, out var key))
            {
                _logService?.Warning(LogCategory.UI, $"归还对象池失败：未找到实例对应的 Key，实例名={instance.name}");
                UnityEngine.Object.Destroy(instance);
                return;
            }

            // 归还对象池
            _poolManager.Return(instance);
            _instanceKeyMap.Remove(instance);
            _logService?.Debug(LogCategory.UI, $"UI 已归还对象池：{key}");
        }

        /// <summary>
        /// 处理弹窗关闭：播放关闭动画后归还对象池
        /// </summary>
        private async UniTask HandleDialogCloseAsync(GameObject instance, Action onClose)
        {
            await _animator.PlayHideAsync(instance);
            onClose?.Invoke();
            ReturnToPool(instance);
        }

        public void ReleaseAll()
        {
            var keys = new List<string>(_prefabCache.Keys);
            foreach (var key in keys)
            {
                TryReleaseSafely(key);
            }

            _prefabCache.Clear();
            _instanceKeyMap.Clear();
            TryLogInfo("已释放所有 UI 资源缓存");
        }

        /// <summary>
        /// UI 实例归还对象池时的重置回调
        /// </summary>
        private void ResetUIInstance(GameObject instance)
        {
            instance.SetActive(false);

            // 重置 RectTransform
            if (instance.TryGetComponent<RectTransform>(out var rt))
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }

            // 重置 CanvasGroup（如果有）
            if (instance.TryGetComponent<CanvasGroup>(out var cg))
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
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
                    _logService?.Information(LogCategory.UI, $"已加载并缓存：{key}");
                }

                return prefab;
            }
            catch (OperationCanceledException)
            {
                _logService?.Warning(LogCategory.UI, $"预加载取消：{key}");
                return null;
            }
            catch (Exception ex)
            {
                _logService?.Error(LogCategory.UI, $"加载失败：{key} - {ex.Message}", ex);
                return null;
            }
        }

        private void TryReleaseSafely(string key)
        {
            try
            {
                _assetProvider.Release(key);
            }
            catch (ObjectDisposedException)
            {
                // 容器/日志通道销毁阶段释放，忽略。
            }
            catch (Exception ex)
            {
                TryLogInfo($"释放 {key} 发生异常：{ex.Message}");
            }
        }

        private void TryLogInfo(string message)
        {
            try
            {
                _logService?.Information(LogCategory.UI, message);
            }
            catch (ObjectDisposedException)
            {
                // 容器销毁阶段日志通道可能已释放，忽略即可。
            }
        }
    }
}

