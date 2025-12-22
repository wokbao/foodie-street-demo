using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Buttons
{
    /// <summary>
    /// 文本按钮生成器（无背景，纯文字）
    /// </summary>
    public static class ButtonTextGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Buttons";
        private const string FileName = "UI_ButtonText.prefab";

        [MenuItem("Tools/Game UI/Generators/Buttons/生成文本按钮")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_ButtonText", typeof(RectTransform), typeof(Button));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160f, 48f);

            var button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;

            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "文本按钮";
            label.alignment = TextAlignmentOptions.Center;
            label.color = UIGeneratorUtility.ColorAccent;
            label.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            button.targetGraphic = label;
            button.colors = new ColorBlock
            {
                normalColor = UIGeneratorUtility.ColorAccent,
                highlightedColor = UIGeneratorUtility.Tint(UIGeneratorUtility.ColorAccent, 0.8f),
                pressedColor = UIGeneratorUtility.Tint(UIGeneratorUtility.ColorAccent, 0.6f),
                selectedColor = UIGeneratorUtility.ColorAccent,
                disabledColor = UIGeneratorUtility.Hex("#CCCCCC"),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.ButtonText);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.ButtonText}");
        }
    }
}
