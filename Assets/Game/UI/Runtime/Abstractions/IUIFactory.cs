using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// UI 资源加载与实例化工厂，统一管理 Addressable UI 的加载/缓存/释放。
    /// </summary>
    public interface IUIFactory
    {
        /// <summary>
        /// 预加载指定 Key 的 UI 资源（仅加载，不实例化），可提前填充缓存。
        /// </summary>
        UniTask PreloadAsync(string key, CancellationToken ct = default);

        /// <summary>
        /// 按 Key 加载并实例化 UI 预制体。
        /// </summary>
        UniTask<GameObject> InstantiateAsync(string key, Transform parent = null, IProgress<float> progress = null, CancellationToken ct = default);

        /// <summary>
        /// 释放指定 Key 的已加载资源（会清理缓存），如需彻底卸载请确保实例已销毁。
        /// </summary>
        void Release(string key);

        /// <summary>
        /// 释放所有已加载的 UI 资源缓存。
        /// </summary>
        void ReleaseAll();

        /// <summary>
        /// 当前已缓存的 UI 资源 Key 列表（诊断用）。
        /// </summary>
        IReadOnlyCollection<string> CachedKeys { get; }
    }
}
