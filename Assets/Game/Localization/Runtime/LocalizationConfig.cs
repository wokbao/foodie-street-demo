using UnityEngine;

namespace Game.Localization.Runtime
{
    /// <summary>
    /// 本地化配置 ScriptableObject
    /// </summary>
    /// <remarks>
    /// <para><b>用途</b>：配置默认表名、启动语言等设置</para>
    /// <para><b>位置</b>：Assets/Game/Localization/Settings/</para>
    /// </remarks>
    [CreateAssetMenu(fileName = "LocalizationConfig", menuName = "Game/Localization/Config")]
    public class LocalizationConfig : ScriptableObject
    {
        [Header("默认设置")]
        [Tooltip("默认 String Table 名称")]
        [SerializeField] private string _defaultTableName = LocalizationTableKeys.UI;

        [Tooltip("Fallback 语言代码（当翻译缺失时）")]
        [SerializeField] private string _fallbackLocaleCode = "en";

        [Header("启动设置")]
        [Tooltip("是否在启动时自动初始化")]
        [SerializeField] private bool _initializeOnStartup = true;

        [Tooltip("是否记住用户语言选择")]
        [SerializeField] private bool _rememberUserSelection = true;

        /// <summary>
        /// 默认 String Table 名称
        /// </summary>
        public string DefaultTableName => _defaultTableName;

        /// <summary>
        /// Fallback 语言代码
        /// </summary>
        public string FallbackLocaleCode => _fallbackLocaleCode;

        /// <summary>
        /// 是否在启动时自动初始化
        /// </summary>
        public bool InitializeOnStartup => _initializeOnStartup;

        /// <summary>
        /// 是否记住用户语言选择
        /// </summary>
        public bool RememberUserSelection => _rememberUserSelection;
    }
}
