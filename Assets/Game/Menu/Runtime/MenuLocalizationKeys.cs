namespace Game.Menu.Runtime
{
    /// <summary>
    /// 菜单模块本地化 Key 常量
    /// </summary>
    /// <remarks>
    /// <para><b>使用规范</b>：</para>
    /// <list type="bullet">
    ///   <item>所有菜单相关的本地化 Key 必须在此定义</item>
    ///   <item>禁止在代码中使用硬编码的用户可见文本</item>
    ///   <item>Key 格式：Menu_{功能}_{描述}</item>
    /// </list>
    /// </remarks>
    public static class MenuLocalizationKeys
    {
        /// <summary>
        /// 加载相关
        /// </summary>
        public static class Loading
        {
            /// <summary>
            /// 进入游戏加载描述 - "进入游戏"
            /// </summary>
            public const string EnterGame = "Menu_Loading_EnterGame";

            /// <summary>
            /// 加载场景进度描述 - "加载场景"
            /// </summary>
            public const string LoadingScene = "Menu_Loading_Scene";
        }
    }
}
