using UnityEngine;

namespace Game.UI.Runtime
{
    /// <summary>
    /// UI 层级与排序配置，便于统一管理各类 UI 层（主界面/HUD/Overlay/Loading/过渡）。
    /// </summary>
    [CreateAssetMenu(fileName = "UIHierarchyConfig", menuName = "Game/UI/UI Hierarchy Config")]
    public sealed class UIHierarchyConfig : ScriptableObject
    {
        [Header("根节点")]
        [SerializeField] private string _rootName = "GlobalUIRoot";
        [SerializeField] private int _rootSortingOrder = 0;

        [Header("主界面层")]
        [SerializeField] private string _mainLayerName = "Layer_Main";
        [SerializeField] private int _mainSortingOrder = 0;

        [Header("HUD 层")]
        [SerializeField] private string _hudLayerName = "Layer_HUD";
        [SerializeField] private int _hudSortingOrder = 200;

        [Header("弹窗层")]
        [SerializeField] private string _overlayLayerName = "Layer_Overlay";
        [SerializeField] private int _overlaySortingOrder = 600;

        [Header("Loading 层")]
        [SerializeField] private string _loadingLayerName = "Layer_Loading";
        [SerializeField] private int _loadingSortingOrder = 8000;

        [Header("过渡层")]
        [SerializeField] private string _transitionLayerName = "Layer_Transition";
        [SerializeField] private int _transitionSortingOrder = 9999;

        public string RootName => _rootName;
        public int RootSortingOrder => _rootSortingOrder;
        public string MainLayerName => _mainLayerName;
        public int MainSortingOrder => _mainSortingOrder;
        public string HudLayerName => _hudLayerName;
        public int HudSortingOrder => _hudSortingOrder;
        public string OverlayLayerName => _overlayLayerName;
        public int OverlaySortingOrder => _overlaySortingOrder;
        public string LoadingLayerName => _loadingLayerName;
        public int LoadingSortingOrder => _loadingSortingOrder;
        public string TransitionLayerName => _transitionLayerName;
        public int TransitionSortingOrder => _transitionSortingOrder;

        public static UIHierarchyConfig Default
        {
            get
            {
                var cfg = CreateInstance<UIHierarchyConfig>();
                return cfg;
            }
        }
    }
}

