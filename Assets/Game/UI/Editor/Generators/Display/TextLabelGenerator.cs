using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.UI.Editor.Generators.Display
{
    /// <summary>
    /// 文本标签生成器
    /// </summary>
    public static class TextLabelGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Display";
        private const string FileName = "UI_TextLabel.prefab";

        [MenuItem("Tools/Game UI/Generators/Display/生成文本标签")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_TextLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300f, 40f);

            var text = root.GetComponent<TextMeshProUGUI>();
            text.text = "文本标签";
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.color = UIGeneratorUtility.ColorText;
            text.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(text);

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.TextLabel);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.TextLabel}");
        }
    }
}
