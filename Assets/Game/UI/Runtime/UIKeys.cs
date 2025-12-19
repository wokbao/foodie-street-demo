namespace Game.UI.Runtime
{
    /// <summary>
    /// Addressable Key 常量，统一管理 UI 资源命名。
    /// </summary>
    public static class UIKeys
    {
        public static class Common
        {
            public const string ButtonPrimary = "UI/ButtonPrimary";
            public const string ButtonSecondary = "UI/ButtonSecondary";
            public const string PanelBase = "UI/Panel/Base";
            public const string PanelHeader = "UI/Panel/Header";
            public const string DialogConfirm = "UI/Dialog/Confirm";
            public const string DialogInfo = "UI/Dialog/Info";
        }

        public static class Screens
        {
            public const string MainMenu = "UI/Screens/MainMenu";
            // 后续屏幕：如 Gameplay HUD、Pause、Result 等可在此添加
        }
    }
}

