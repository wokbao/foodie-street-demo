using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Inputs
{
    /// <summary>
    /// 下拉菜单生成器
    /// </summary>
    public static class DropdownGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Inputs";
        private const string FileName = "UI_Dropdown.prefab";

        [MenuItem("Tools/Game UI/Generators/Inputs/生成下拉菜单")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_Dropdown", typeof(RectTransform), typeof(Image), typeof(TMP_Dropdown));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300f, 48f);

            var image = root.GetComponent<Image>();
            image.color = Color.white;

            // Label
            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 8f);
            labelRect.offsetMax = new Vector2(-40f, -8f);

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "选项";
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = UIGeneratorUtility.ColorText;
            label.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            // Arrow
            var arrowObj = new GameObject("Arrow", typeof(RectTransform), typeof(Image));
            arrowObj.transform.SetParent(root.transform, false);
            var arrowRect = arrowObj.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1f, 0.5f);
            arrowRect.anchorMax = new Vector2(1f, 0.5f);
            arrowRect.pivot = new Vector2(1f, 0.5f);
            arrowRect.sizeDelta = new Vector2(24f, 24f);
            arrowRect.anchoredPosition = new Vector2(-12f, 0f);

            var arrowImage = arrowObj.GetComponent<Image>();
            arrowImage.color = UIGeneratorUtility.ColorText;

            // Template (Dropdown List)
            var templateObj = new GameObject("Template", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
            templateObj.transform.SetParent(root.transform, false);
            templateObj.SetActive(false);
            var templateRect = templateObj.GetComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0f, 0f);
            templateRect.anchorMax = new Vector2(1f, 0f);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.sizeDelta = new Vector2(0f, 200f);
            templateRect.anchoredPosition = new Vector2(0f, 2f);

            var templateImage = templateObj.GetComponent<Image>();
            templateImage.color = Color.white;

            // Viewport
            var viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewportObj.transform.SetParent(templateObj.transform, false);
            var viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            viewportObj.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            // Content
            var contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(viewportObj.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = Vector2.one;
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = new Vector2(0f, 48f);

            // Item
            var itemObj = new GameObject("Item", typeof(RectTransform), typeof(Toggle));
            itemObj.transform.SetParent(contentObj.transform, false);
            var itemRect = itemObj.GetComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0f, 1f);
            itemRect.anchorMax = Vector2.one;
            itemRect.pivot = new Vector2(0.5f, 1f);
            itemRect.sizeDelta = new Vector2(0f, 48f);

            // Item Background
            var itemBgObj = new GameObject("Item Background", typeof(RectTransform), typeof(Image));
            itemBgObj.transform.SetParent(itemObj.transform, false);
            var itemBgRect = itemBgObj.GetComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.offsetMin = Vector2.zero;
            itemBgRect.offsetMax = Vector2.zero;

            var itemBgImage = itemBgObj.GetComponent<Image>();
            itemBgImage.color = UIGeneratorUtility.ColorSecondary;

            // Item Label
            var itemLabelObj = new GameObject("Item Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            itemLabelObj.transform.SetParent(itemObj.transform, false);
            var itemLabelRect = itemLabelObj.GetComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.offsetMin = new Vector2(16f, 0f);
            itemLabelRect.offsetMax = new Vector2(-16f, 0f);

            var itemLabel = itemLabelObj.GetComponent<TextMeshProUGUI>();
            itemLabel.text = "选项";
            itemLabel.alignment = TextAlignmentOptions.MidlineLeft;
            itemLabel.color = UIGeneratorUtility.ColorText;
            itemLabel.fontSize = 18f;
            UIGeneratorUtility.TryAssignDefaultFont(itemLabel);

            var itemToggle = itemObj.GetComponent<Toggle>();
            itemToggle.targetGraphic = itemBgImage;

            var scrollRect = templateObj.GetComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            var dropdown = root.GetComponent<TMP_Dropdown>();
            dropdown.targetGraphic = image;
            dropdown.template = templateRect;
            dropdown.captionText = label;
            dropdown.itemText = itemLabel;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.Dropdown);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.Dropdown}");
        }
    }
}
