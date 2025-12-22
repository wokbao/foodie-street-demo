using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Panels
{
    /// <summary>
    /// 带标题栏面板生成器
    /// </summary>
    public static class PanelHeaderGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Panels";
        private const string FileName = "UI_PanelWithHeader.prefab";

        [MenuItem("Tools/Game UI/Generators/Panels/生成带标题面板")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_PanelWithHeader", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(900f, 650f);

            var image = root.GetComponent<Image>();
            image.color = UIGeneratorUtility.ColorSecondary;

            // Header
            var headerObj = new GameObject("Header", typeof(RectTransform), typeof(Image));
            headerObj.transform.SetParent(root.transform, false);
            var headerRect = headerObj.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.sizeDelta = new Vector2(0f, 96f);
            headerRect.anchoredPosition = Vector2.zero;

            var headerImage = headerObj.GetComponent<Image>();
            headerImage.color = UIGeneratorUtility.ColorAccent;

            // Title
            var titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(headerObj.transform, false);
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(24f, 0f);
            titleRect.offsetMax = new Vector2(-120f, 0f);

            var title = titleObj.GetComponent<TextMeshProUGUI>();
            title.text = "设置";
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.color = UIGeneratorUtility.ColorText;
            title.fontSize = 28f;
            UIGeneratorUtility.TryAssignDefaultFont(title);

            // Close Button Placeholder
            var closeButton = CreateCloseButton();
            closeButton.transform.SetParent(headerObj.transform, false);
            var closeRect = closeButton.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1f, 0.5f);
            closeRect.anchorMax = new Vector2(1f, 0.5f);
            closeRect.pivot = new Vector2(1f, 0.5f);
            closeRect.sizeDelta = new Vector2(96f, 56f);
            closeRect.anchoredPosition = new Vector2(-24f, 0f);

            // Content
            var contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(root.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(16f, 16f);
            contentRect.offsetMax = new Vector2(-16f, -112f);

            var panel = root.AddComponent<UIPanelWithHeader>();
            panel.EditorWireUp(title, closeButton.GetComponent<Button>(), contentRect);

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.PanelHeader);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.PanelHeader}");
        }

        private static GameObject CreateCloseButton()
        {
            var root = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
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
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "关闭";
            label.alignment = TextAlignmentOptions.Center;
            label.color = UIGeneratorUtility.ColorText;
            label.fontSize = 20f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            return root;
        }
    }
}
