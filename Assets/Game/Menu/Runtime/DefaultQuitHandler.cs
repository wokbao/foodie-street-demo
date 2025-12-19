using System;
using Core.Feature.Logging.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using Game.UI.Runtime;
using Game.UI.Runtime.Abstractions;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 默认退出处理：直接退出应用，可替换为确认弹窗或平台特定逻辑。
    /// </summary>
    public sealed class DefaultQuitHandler : IQuitHandler
    {
        private readonly ILogService _logService;
        private readonly IUIFactory _uiFactory;
        private readonly UIHierarchyConfig _uiHierarchyConfig;

        private UIConfirmDialog _dialog;

        public DefaultQuitHandler(
            ILogService logService,
            IObjectResolver resolver,
            IUIFactory uiFactory,
            UIHierarchyConfig uiHierarchyConfig = null)
        {
            _logService = logService;
            _uiFactory = uiFactory;
            _uiHierarchyConfig = uiHierarchyConfig != null ? uiHierarchyConfig : UIHierarchyConfig.Default;
            resolver.TryResolve(out _dialog);
        }

        public async UniTask RequestQuitAsync()
        {
            if (_dialog == null)
            {
                await EnsureDialogLoadedAsync();
            }

            if (_dialog == null)
            {
                _logService?.Information(LogCategory.Menu, "[退出] 直接退出应用（未找到确认弹窗）");
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                return;
            }

            _logService?.Information(LogCategory.Menu, "[退出] 弹出确认弹窗");
            _dialog.Show(
                "确定要退出游戏吗？",
                onConfirm: () =>
                {
                    _logService?.Information(LogCategory.Menu, "[退出] 用户确认退出");
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                },
                onCancel: () =>
                {
                    _logService?.Information(LogCategory.Menu, "[退出] 用户取消退出");
                    _dialog.Close();
                });

            return;
        }

        private async UniTask EnsureDialogLoadedAsync()
        {
            if (_uiFactory == null)
            {
                _logService?.Warning(LogCategory.Menu, "[退出] UIFactory 未注册，无法实例化确认弹窗");
                return;
            }

            var parent = FindOverlayLayer();
            if (parent == null)
            {
                _logService?.Warning(LogCategory.Menu, "[退出] 未找到 Overlay 层，无法实例化确认弹窗");
                return;
            }

            var instance = await _uiFactory.InstantiateAsync(UIKeys.Common.DialogConfirm, parent);
            if (instance == null)
            {
                _logService?.Warning(LogCategory.Menu, "[退出] 实例化确认弹窗失败，Key={0}", UIKeys.Common.DialogConfirm);
                return;
            }

            _dialog = instance.GetComponentInChildren<UIConfirmDialog>(true);
            if (_dialog == null)
            {
                _logService?.Warning(LogCategory.Menu, "[退出] 预制体上未找到 UIConfirmDialog");
                UnityEngine.Object.Destroy(instance);
            }
        }

        private Transform FindOverlayLayer()
        {
            var rootName = string.IsNullOrWhiteSpace(_uiHierarchyConfig.RootName)
                ? "GlobalUIRoot"
                : _uiHierarchyConfig.RootName;
            var root = GameObject.Find(rootName);
            if (root == null || root.Equals(null))
            {
                return null;
            }

            var layerName = string.IsNullOrWhiteSpace(_uiHierarchyConfig.OverlayLayerName)
                ? "Layer_Overlay"
                : _uiHierarchyConfig.OverlayLayerName;

            var layer = FindOrCreateLayer(root.transform, layerName);
            return IsDestroyed(layer) ? null : layer;
        }

        private static Transform FindOrCreateLayer(Transform parent, string name)
        {
            if (IsDestroyed(parent))
            {
                return null;
            }

            Transform layer;
            try
            {
                layer = parent.Find(name);
            }
            catch (MissingReferenceException)
            {
                return null;
            }

            if (IsDestroyed(layer))
            {
                var go = new GameObject(name);
                layer = go.transform;
                layer.SetParent(parent, false);
            }

            return layer;
        }

        private static bool IsDestroyed(UnityEngine.Object obj) => obj == null || obj.Equals(null);
    }

}
