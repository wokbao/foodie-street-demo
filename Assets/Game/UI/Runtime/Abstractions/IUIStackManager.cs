using System;
using UnityEngine;

namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// UI 堆栈管理器接口，用于管理弹窗的显示顺序和关闭逻辑。
    /// 支持 ESC/返回键关闭最上层弹窗。
    /// </summary>
    public interface IUIStackManager
    {
        /// <summary>
        /// 将 UI 压入堆栈（打开弹窗时调用）
        /// </summary>
        /// <param name="ui">要压入的 UI 对象</param>
        /// <param name="onClose">关闭时的回调（可选）</param>
        void Push(GameObject ui, Action onClose = null);

        /// <summary>
        /// 弹出并关闭栈顶 UI
        /// </summary>
        /// <returns>是否成功弹出</returns>
        bool Pop();

        /// <summary>
        /// 查看栈顶 UI（不弹出）
        /// </summary>
        /// <returns>栈顶 UI，若为空则返回 null</returns>
        GameObject Peek();

        /// <summary>
        /// 清空所有堆栈中的 UI
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取当前堆栈深度
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 是否启用 ESC 键关闭功能
        /// </summary>
        bool EscapeEnabled { get; set; }
    }
}
