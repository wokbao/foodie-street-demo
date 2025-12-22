using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Inputs
{
    /// <summary>
    /// 文本输入框生成器
    /// </summary>
    public static class InputFieldGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Inputs";
        private const string FileName = "UI_InputField.prefab";

        [MenuItem("Tools/Game UI/Generators/Inputs/生成输入框")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_InputField", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400f, 56f);

            var image = root.GetComponent<Image>();
            image.color = Color.white;

            // Text Area
            var textAreaObj = new GameObject("TextArea", typeof(RectTransform), typeof(RectMask2D));
            textAreaObj.transform.SetParent(root.transform, false);
            var textAreaRect = textAreaObj.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(12f, 12f);
            textAreaRect.offsetMax = new Vector2(-12f, -12f);

            // Placeholder
            var placeholderObj = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
            placeholderObj.transform.SetParent(textAreaObj.transform, false);
            var placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            var placeholder = placeholderObj.GetComponent<TextMeshProUGUI>();
            placeholder.text = "请输入...";
            placeholder.alignment = TextAlignmentOptions.MidlineLeft;
            placeholder.color = UIGeneratorUtility.Hex("#999999");
            placeholder.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(placeholder);

            // Text
            var textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(textAreaObj.transform, false);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "";
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.color = UIGeneratorUtility.ColorText;
            text.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(text);

            var inputField = root.GetComponent<TMP_InputField>();
            inputField.textViewport = textAreaRect;
            inputField.textComponent = text;
            inputField.placeholder = placeholder;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.InputField);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.InputField}");
        }
    }
}
