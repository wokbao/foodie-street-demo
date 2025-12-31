namespace Game.Audio.Runtime
{
    /// <summary>
    /// 音频通道枚举
    /// 
    /// <para><b>用途</b>：</para>
    /// <list type="bullet">
    ///   <item>区分不同类型的音频（BGM、音效、UI、语音）</item>
    ///   <item>支持分通道音量控制</item>
    ///   <item>便于 AudioMixer 映射（后续扩展）</item>
    /// </list>
    /// </summary>
    public enum AudioChannel
    {
        /// <summary>主音量，控制所有通道</summary>
        Master = 0,

        /// <summary>背景音乐</summary>
        BGM = 1,

        /// <summary>游戏音效（战斗、交互等）</summary>
        SFX = 2,

        /// <summary>UI 音效（按钮点击、弹窗等）</summary>
        UI = 3,

        /// <summary>语音/对话</summary>
        Voice = 4
    }
}
