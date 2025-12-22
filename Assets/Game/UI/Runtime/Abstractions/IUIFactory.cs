using System;
using System.Collections.Generic;
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
        /// 加载并显示弹窗/面板，自动加入 UI 堆栈（支持 ESC 关闭）。
        /// </summary>
        /// <param name="key">Addressable Key</param>
        /// <param name="parent">父节点</param>
        /// <param name="onClose">关闭时回调（可选）</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>实例化的 UI 对象</returns>
        UniTask<GameObject> ShowDialogAsync(string key, Transform parent = null, Action onClose = null, CancellationToken ct = default);

        /// <summary>
        /// 将 UI 实例归还对象池（隐藏而非销毁）
        /// </summary>
        void ReturnToPool(GameObject instance);

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
