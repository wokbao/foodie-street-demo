using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.UI.Runtime.Abstractions;
using VContainer.Unity;

namespace Game.UI.Runtime
{
    /// <summary>
    /// UI 预加载器：游戏启动时预加载常用 UI 资源。
    /// 实现 IStartable，在 VContainer 容器启动时自动执行。
    /// </summary>
    public sealed class UIPreloader : IStartable
    {
        private readonly IUIFactory _uiFactory;
        private readonly ILogService _log;

        /// <summary>
        /// 需要预加载的 UI Key 列表
        /// </summary>
        private static readonly string[] PreloadKeys =
        {
            UIKeys.Common.DialogConfirm,
            UIKeys.Common.PanelHeader,
        };

        public UIPreloader(IUIFactory uiFactory, ILogService log)
        {
            _uiFactory = uiFactory;
            _log = log;
        }

        public void Start()
        {
            PreloadAllAsync().Forget();
        }

        private async UniTaskVoid PreloadAllAsync()
        {
            _log.Information(LogCategory.UI, $"开始预加载 UI 资源，共 {PreloadKeys.Length} 个");

            foreach (var key in PreloadKeys)
            {
                try
                {
                    await _uiFactory.PreloadAsync(key);
                    _log.Debug(LogCategory.UI, $"预加载完成：{key}");
                }
                catch (System.Exception ex)
                {
                    _log.Warning(LogCategory.UI, $"预加载失败：{key}，错误：{ex.Message}");
                }
            }

            _log.Information(LogCategory.UI, "UI 资源预加载完成");
        }
    }
}
