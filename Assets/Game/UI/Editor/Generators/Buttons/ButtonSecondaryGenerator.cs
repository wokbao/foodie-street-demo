using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Buttons
{
    /// <summary>
    /// 次按钮生成器
    /// </summary>
    public static class ButtonSecondaryGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Buttons";
        private const string FileName = "UI_ButtonSecondary.prefab";

        [MenuItem("Tools/Game UI/Generators/Buttons/生成次按钮")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_ButtonSecondary", typeof(RectTransform), typeof(Image), typeof(Button));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(320f, 88f);

            var image = root.GetComponent<Image>();
            image.color = UIGeneratorUtility.ColorSecondary;

            var button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = image;
            button.colors = UIGeneratorUtility.BuildButtonColors(isPrimary: false);

            root.AddComponent<UIButtonScaleFeedback>();

            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 12f);
            labelRect.offsetMax = new Vector2(-16f, -12f);

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "次按钮";
            label.alignment = TextAlignmentOptions.Center;
            label.color = UIGeneratorUtility.ColorText;
            label.fontSize = 24f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.ButtonSecondary);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.ButtonSecondary}");
        }
    }
}
