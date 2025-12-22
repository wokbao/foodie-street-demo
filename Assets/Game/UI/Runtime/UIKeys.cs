namespace Game.UI.Runtime
{
    /// <summary>
    /// Addressable Key 常量，统一管理 UI 资源命名。
    /// </summary>
    public static class UIKeys
    {
        /// <summary>
        /// 通用基础组件（可复用）
        /// </summary>
        public static class Common
        {
            // 按钮类
            public const string ButtonPrimary = "UI/ButtonPrimary";
            public const string ButtonSecondary = "UI/ButtonSecondary";
            public const string ButtonIcon = "UI/ButtonIcon";
            public const string ButtonText = "UI/ButtonText";
            public const string ButtonClose = "UI/ButtonClose";

            // 面板类
            public const string PanelBase = "UI/Panel/Base";
            public const string PanelHeader = "UI/Panel/Header";
            public const string PanelCard = "UI/Panel/Card";

            // 对话框类
            public const string DialogConfirm = "UI/Dialog/Confirm";
            public const string DialogInfo = "UI/Dialog/Info";
            public const string DialogInput = "UI/Dialog/Input";

            // 输入组件
            public const string InputField = "UI/Input/Field";
            public const string Slider = "UI/Input/Slider";
            public const string Toggle = "UI/Input/Toggle";
            public const string Dropdown = "UI/Input/Dropdown";

            // 显示组件
            public const string TextLabel = "UI/Display/Label";
            public const string ProgressBar = "UI/Display/ProgressBar";
            public const string ImageFrame = "UI/Display/ImageFrame";

            // 布局组件
            public const string ScrollView = "UI/Layout/ScrollView";
            public const string Divider = "UI/Layout/Divider";
        }

        /// <summary>
        /// 完整界面（场景级）
        /// </summary>
        public static class Screens
        {
            public const string MainMenu = "UI/Screens/MainMenu";
            // 后续屏幕：Gameplay HUD、Pause、Result 等
        }
    }
}
