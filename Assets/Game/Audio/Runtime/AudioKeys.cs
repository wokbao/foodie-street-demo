namespace Game.Audio.Runtime
{
    /// <summary>
    /// 音频资源 Key 常量
    /// 
    /// <para><b>用途</b>：</para>
    /// <list type="bullet">
    ///   <item>消除魔法字符串</item>
    ///   <item>统一管理 Addressables 音频资源地址</item>
    ///   <item>便于重构和 IDE 重命名</item>
    /// </list>
    /// 
    /// <para><b>注意</b>：Key 必须与 Addressables 中配置的地址一致</para>
    /// </summary>
    public static class AudioKeys
    {
        /// <summary>
        /// 背景音乐 Key
        /// </summary>
        public static class BGM
        {
            public const string Menu = "Audio/BGM/bgm_menu";
            public const string Gameplay = "Audio/BGM/bgm_gameplay";
        }

        /// <summary>
        /// 音效 Key
        /// </summary>
        public static class SFX
        {
            /// <summary>UI 点击音效</summary>
            public const string Click = "Audio/SFX/sfx_click";
            
            /// <summary>成功提示音</summary>
            public const string Success = "Audio/SFX/sfx_success";
            
            /// <summary>失败/错误提示音</summary>
            public const string Error = "Audio/SFX/sfx_error";
        }

        /// <summary>
        /// UI 专用音效 Key
        /// </summary>
        public static class UI
        {
            /// <summary>按钮点击</summary>
            public const string ButtonClick = "Audio/SFX/sfx_click";
            
            /// <summary>弹窗打开</summary>
            public const string PopupOpen = "Audio/SFX/sfx_popup_open";
            
            /// <summary>弹窗关闭</summary>
            public const string PopupClose = "Audio/SFX/sfx_popup_close";
        }
    }
}
