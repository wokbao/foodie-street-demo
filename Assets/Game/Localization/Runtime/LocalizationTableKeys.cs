using Core.Feature.Localization.Abstractions;

namespace Game.Localization.Runtime
{
    /// <summary>
    /// 本地化表名常量定义
    /// </summary>
    /// <remarks>
    /// <para><b>使用规范</b>：</para>
    /// <list type="bullet">
    ///   <item>所有表名必须在此定义，禁止魔法字符串</item>
    ///   <item>表名应与 Unity Localization Table Collection 名称一致</item>
    /// </list>
    /// </remarks>
    public static class LocalizationTableKeys
    {
        /// <summary>
        /// 通用文本（确认、取消、返回等）
        /// </summary>
        public const string Shared = DefaultLocalizationOptions.DefaultTable;

        /// <summary>
        /// UI 文本（按钮、标签、菜单等）
        /// </summary>
        public const string UI = "UIStrings";

        /// <summary>
        /// 玩法文本（关卡、道具、任务等）
        /// </summary>
        public const string Gameplay = "GameplayStrings";

        /// <summary>
        /// 对话文本（故事、NPC 对话等）
        /// </summary>
        public const string Dialogs = "DialogStrings";
    }
}
