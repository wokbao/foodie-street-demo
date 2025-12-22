using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Buttons
{
    /// <summary>
    /// 图标按钮生成器（无背景，仅图标+悬停效果）
    /// </summary>
    public static class ButtonIconGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Buttons";
        private const string FileName = "UI_ButtonIcon.prefab";

        [MenuItem("Tools/Game UI/Generators/Buttons/生成图标按钮")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_ButtonIcon", typeof(RectTransform), typeof(Image), typeof(Button));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(64f, 64f);

            var image = root.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0f); // 透明背景

            var button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.None;

            root.AddComponent<UIButtonScaleFeedback>();

            // Icon placeholder
            var iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(root.transform, false);
            var iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(8f, 8f);
            iconRect.offsetMax = new Vector2(-8f, -8f);

            var iconImage = iconObj.GetComponent<Image>();
            iconImage.color = UIGeneratorUtility.ColorText;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.ButtonIcon);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.ButtonIcon}");
        }
    }
}
