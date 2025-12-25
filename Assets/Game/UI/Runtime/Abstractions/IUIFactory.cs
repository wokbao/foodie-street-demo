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
        /// 显示确认对话框并等待用户选择。
        /// UI 完全关闭后返回用户的选择结果，确保业务逻辑在所有资源释放后执行。
        /// </summary>
        /// <param name="message">要显示给用户的提示消息</param>
        /// <param name="parent">
        /// 对话框的父节点，通常为 UI 层级节点（如 <see cref="UILayer.Overlay"/>）。
        /// 如果为 null，将使用默认父节点。
        /// </param>
        /// <param name="ct">
        /// 取消令牌，用于在对话框加载阶段取消操作。
        /// 注意：对话框显示后，用户必须点击按钮才能关闭，无法通过取消令牌强制关闭。
        /// </param>
        /// <returns>
        /// 返回一个 <see cref="UniTask{DialogResult}"/>，在对话框完全关闭后完成。
        /// <list type="table">
        /// <item>
        /// <term><see cref="DialogResult.Confirmed"/></term>
        /// <description>用户点击了确认按钮</description>
        /// </item>
        /// <item>
        /// <term><see cref="DialogResult.Cancelled"/></term>
        /// <description>用户点击了取消按钮或按 ESC 键</description>
        /// </item>
        /// <item>
        /// <term><see cref="DialogResult.None"/></term>
        /// <description>加载失败或发生异常</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para>
        /// <strong>执行顺序保证</strong>：
        /// 此方法确保以下严格的执行顺序：
        /// <list type="number">
        /// <item>加载并显示对话框</item>
        /// <item>等待用户点击按钮</item>
        /// <item>播放关闭动画</item>
        /// <item>归还对象池并释放资源</item>
        /// <item>返回用户选择结果</item>
        /// </list>
        /// 因此，在 <c>await</c> 之后执行的代码可以安全地假设所有 UI 资源已释放，
        /// 即使执行破坏性操作（如 <c>Application.Quit()</c>）也不会导致异常。
        /// </para>
        /// </remarks>
        /// <example>
        /// 典型用法（退出确认）：
        /// <code>
        /// var parent = _uiRootService.GetLayer(UILayer.Overlay);
        /// var result = await _uiFactory.ShowConfirmDialogAsync("确定要退出游戏吗？", parent);
        /// 
        /// if (result == DialogResult.Confirmed)
        /// {
        ///     Application.Quit(); // 安全执行，UI 已完全关闭
        /// }
        /// </code>
        /// </example>
        UniTask<DialogResult> ShowConfirmDialogAsync(string message, Transform parent = null, CancellationToken ct = default);


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
