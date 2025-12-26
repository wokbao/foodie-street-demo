using System;
using System.Collections.Generic;
using Core.Feature.Logging.Abstractions;
using Game.UI.Runtime.Abstractions;
using UnityEngine;
using VContainer.Unity;

namespace Game.UI.Runtime
{
    /// <summary>
    /// UI 堆栈管理器实现，管理弹窗的显示顺序和 ESC 键关闭。
    /// 使用 VContainer 的 ITickable 接口监听按键输入。
    /// </summary>
    public sealed class UIStackManager : IUIStackManager, ITickable
    {
        /// <summary>
        /// 堆栈项，包含 UI 对象和关闭回调
        /// </summary>
        private readonly struct StackEntry
        {
            public readonly GameObject UI;
            public readonly Action OnClose;

            public StackEntry(GameObject ui, Action onClose)
            {
                UI = ui;
                OnClose = onClose;
            }
        }

        private readonly ILogService _log;
        private readonly Stack<StackEntry> _stack = new();

        public UIStackManager(ILogService log)
        {
            _log = log;
        }

        /// <inheritdoc/>
        public int Count => _stack.Count;

        /// <inheritdoc/>
        public bool EscapeEnabled { get; set; } = true;

        /// <inheritdoc/>
        public void Push(GameObject ui, Action onClose = null)
        {
            if (ui == null)
            {
                _log.Warning(LogCategory.UI, "Push 失败：UI 对象为 null");
                return;
            }

            _stack.Push(new StackEntry(ui, onClose));
            _log.Debug(LogCategory.UI, $"堆栈压入：{ui.name}，当前深度：{_stack.Count}");
        }

        /// <inheritdoc/>
        public bool Pop()
        {
            if (_stack.Count == 0)
            {
                _log.Debug(LogCategory.UI, "Pop 失败：堆栈为空");
                return false;
            }

            var entry = _stack.Pop();
            var uiName = entry.UI != null ? entry.UI.name : "(已销毁)";

            _log.Debug(LogCategory.UI, $"堆栈弹出：{uiName}，剩余深度：{_stack.Count}");

            // 在调用 OnClose 之前，先通知可关闭 UI 进行外部关闭处理（基于接口抽象）
            // 这确保了 UI 组件能在外部关闭前完成必要的清理（如设置对话框异步结果）
            if (entry.UI != null)
            {
                var closeable = entry.UI.GetComponentInChildren<IUICloseable>(includeInactive: true);
                closeable?.OnExternalClose();
            }

            // 调用关闭回调（由调用方决定是销毁还是归还对象池）
            entry.OnClose?.Invoke();

            return true;
        }

        /// <inheritdoc/>
        public GameObject Peek()
        {
            if (_stack.Count == 0)
            {
                return null;
            }

            var entry = _stack.Peek();

            // 检查 UI 是否已被外部销毁
            if (entry.UI == null)
            {
                // 清理无效项
                _stack.Pop();
                return Peek(); // 递归查找有效项
            }

            return entry.UI;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _log.Information(LogCategory.UI, $"清空所有弹窗，共 {_stack.Count} 个");

            while (_stack.Count > 0)
            {
                var entry = _stack.Pop();
                // 调用关闭回调（由调用方决定是销毁还是归还对象池）
                entry.OnClose?.Invoke();
            }
        }

        /// <summary>
        /// VContainer ITickable 实现，每帧检测 ESC 键
        /// </summary>
        public void Tick()
        {
            if (!EscapeEnabled) return;
            if (_stack.Count == 0) return;

            // 检测 ESC 键或 Android 返回键
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _log.Debug(LogCategory.UI, "检测到 ESC 键，关闭栈顶弹窗");
                Pop();
            }
        }
    }
}

