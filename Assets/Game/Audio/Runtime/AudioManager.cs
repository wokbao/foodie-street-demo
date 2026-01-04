namespace Game.Audio.Runtime
{
    /// <summary>
    /// 音频服务静态访问器
    /// 
    /// <para><b>用途</b>：</para>
    /// <list type="bullet">
    ///   <item>为无法通过 DI 注入的 MonoBehaviour 组件（如 UIButtonSound）提供 IAudioService 的访问点</item>
    ///   <item>遵循「DI 优先，静态兜底」原则</item>
    /// </list>
    /// 
    /// <para><b>注意</b>：</para>
    /// <list type="bullet">
    ///   <item>业务代码（Presenter/Service）应优先使用构造函数注入</item>
    ///   <item>仅 UI 组件和无法 DI 的场景使用此访问器</item>
    /// </list>
    /// 
    /// <para><b>生命周期</b>：由 <see cref="AudioService.Start"/> 注册，<see cref="AudioService.Dispose"/> 取消注册</para>
    /// </summary>
    public static class AudioManager
    {
        /// <summary>
        /// 全局音频服务实例
        /// </summary>
        /// <remarks>
        /// 在 AudioService 启动后可用，Dispose 后为 null。
        /// 使用前应检查是否为 null。
        /// </remarks>
        public static IAudioService Instance { get; private set; }

        /// <summary>
        /// 注册音频服务实例（由 AudioService 内部调用）
        /// </summary>
        /// <param name="service">音频服务实例</param>
        internal static void Register(IAudioService service)
        {
            Instance = service;
        }

        /// <summary>
        /// 取消注册音频服务实例（由 AudioService 内部调用）
        /// </summary>
        internal static void Unregister()
        {
            Instance = null;
        }

        /// <summary>
        /// 设置音频服务实例（仅用于单元测试 Mock）
        /// </summary>
        /// <param name="mock">Mock 音频服务实例</param>
        public static void SetInstance(IAudioService mock)
        {
            Instance = mock;
        }
    }
}
