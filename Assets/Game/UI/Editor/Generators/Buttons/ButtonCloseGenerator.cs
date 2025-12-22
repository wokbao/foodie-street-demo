using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Buttons
{
    /// <summary>
    /// 关闭按钮生成器（圆形，"×"符号）
    /// </summary>
    public static class ButtonCloseGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Buttons";
        private const string FileName = "UI_ButtonClose.prefab";

        [MenuItem("Tools/Game UI/Generators/Buttons/生成关闭按钮")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_ButtonClose", typeof(RectTransform), typeof(Image), typeof(Button));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(48f, 48f);

            var image = root.GetComponent<Image>();
            image.color = UIGeneratorUtility.ColorSecondary;
            image.type = Image.Type.Sliced;

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
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "×";
            label.alignment = TextAlignmentOptions.Center;
            label.color = UIGeneratorUtility.ColorText;
            label.fontSize = 32f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.ButtonClose);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.ButtonClose}");
        }
    }
}
