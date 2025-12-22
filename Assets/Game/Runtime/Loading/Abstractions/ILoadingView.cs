using System;
using Core.Feature.Loading.Abstractions;
using UnityEngine;

namespace Game.Runtime.Loading.Abstractions
{
    /// <summary>
    /// Loading 视图抽象接口，用于解耦加载 UI 实现。
    /// 支持多种实现：代码生成版、预制体版、UI Toolkit 版等。
    /// </summary>
    public interface ILoadingView : IDisposable
    {
        /// <summary>
        /// 初始化视图（应在第一次使用前调用）。
        /// </summary>
        /// <param name="service">加载服务</param>
        /// <param name="config">配置（可选）</param>
        /// <param name="externalCanvas">外部 Canvas（可选，如果提供则不创建独立 Canvas）</param>
        void Initialize(ILoadingService service, LoadingHudConfig config = null, Canvas externalCanvas = null);

        /// <summary>
        /// 设置进度（0-1）。
        /// </summary>
        void SetProgress(float progress);

        /// <summary>
        /// 设置描述文本。
        /// </summary>
        void SetDescription(string description);

        /// <summary>
        /// 显示加载视图。
        /// </summary>
        void Show();

        /// <summary>
        /// 隐藏加载视图。
        /// </summary>
        void Hide();

        /// <summary>
        /// 是否正在显示。
        /// </summary>
        bool IsVisible { get; }
    }
}
