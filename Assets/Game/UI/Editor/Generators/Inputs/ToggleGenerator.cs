using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Inputs
{
    /// <summary>
    /// 开关生成器
    /// </summary>
    public static class ToggleGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Inputs";
        private const string FileName = "UI_Toggle.prefab";

        [MenuItem("Tools/Game UI/Generators/Inputs/生成开关")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_Toggle", typeof(RectTransform), typeof(Toggle));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200f, 40f);

            // Background
            var bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(root.transform, false);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.5f);
            bgRect.anchorMax = new Vector2(0f, 0.5f);
            bgRect.pivot = new Vector2(0f, 0.5f);
            bgRect.sizeDelta = new Vector2(40f, 40f);

            var bgImage = bgObj.GetComponent<Image>();
            bgImage.color = Color.white;

            // Checkmark
            var checkmarkObj = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            checkmarkObj.transform.SetParent(bgObj.transform, false);
            var checkmarkRect = checkmarkObj.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            checkmarkRect.offsetMin = new Vector2(8f, 8f);
            checkmarkRect.offsetMax = new Vector2(-8f, -8f);

            var checkmarkImage = checkmarkObj.GetComponent<Image>();
            checkmarkImage.color = UIGeneratorUtility.ColorPrimary;

            // Label
            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(56f, 0f);
            labelRect.offsetMax = Vector2.zero;

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "选项";
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = UIGeneratorUtility.ColorText;
            label.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            var toggle = root.GetComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.graphic = checkmarkImage;
            toggle.isOn = false;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.Toggle);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.Toggle}");
        }
    }
}
