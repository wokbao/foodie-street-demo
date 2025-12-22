using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Dialogs
{
    /// <summary>
    /// 输入弹窗生成器（带输入框的对话框）
    /// </summary>
    public static class DialogInputGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Dialogs";
        private const string FileName = "UI_DialogInput.prefab";

        [MenuItem("Tools/Game UI/Generators/Dialogs/生成输入弹窗")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";

            var root = new GameObject("UI_DialogInput", typeof(RectTransform));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var animatorRoot = new GameObject("Animator", typeof(RectTransform), typeof(CanvasGroup));
            animatorRoot.transform.SetParent(root.transform, false);
            var animatorRect = animatorRoot.GetComponent<RectTransform>();
            animatorRect.anchorMin = Vector2.zero;
            animatorRect.anchorMax = Vector2.one;
            animatorRect.offsetMin = Vector2.zero;
            animatorRect.offsetMax = Vector2.zero;

            var canvasGroup = animatorRoot.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;

            var overlayObj = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
            overlayObj.transform.SetParent(animatorRoot.transform, false);
            var overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayObj.GetComponent<Image>().color = UIGeneratorUtility.ColorOverlay;

            var panelObj = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(animatorRoot.transform, false);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(820f, 480f);
            panelRect.anchoredPosition = Vector2.zero;
            panelObj.GetComponent<Image>().color = UIGeneratorUtility.ColorSecondary;

            // Title
            var titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(panelObj.transform, false);
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(0f, 64f);
            titleRect.anchoredPosition = new Vector2(0f, -24f);

            var title = titleObj.GetComponent<TextMeshProUGUI>();
            title.text = "请输入";
            title.alignment = TextAlignmentOptions.Center;
            title.color = UIGeneratorUtility.ColorText;
            title.fontSize = 28f;
            UIGeneratorUtility.TryAssignDefaultFont(title);

            // Input Field Placeholder
            var inputObj = new GameObject("InputField", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            inputObj.transform.SetParent(panelObj.transform, false);
            var inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0.5f, 0.5f);
            inputRect.anchorMax = new Vector2(0.5f, 0.5f);
            inputRect.pivot = new Vector2(0.5f, 0.5f);
            inputRect.sizeDelta = new Vector2(720f, 80f);
            inputRect.anchoredPosition = new Vector2(0f, 0f);

            var inputImage = inputObj.GetComponent<Image>();
            inputImage.color = Color.white;

            // Input Text Area
            var textAreaObj = new GameObject("Text Area", typeof(RectTransform));
            textAreaObj.transform.SetParent(inputObj.transform, false);
            var textAreaRect = textAreaObj.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(16f, 16f);
            textAreaRect.offsetMax = new Vector2(-16f, -16f);

            var textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(textAreaObj.transform, false);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textObj.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.color = UIGeneratorUtility.ColorText;
            text.fontSize = 20f;
            UIGeneratorUtility.TryAssignDefaultFont(text);

            var inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.textViewport = textAreaRect;
            inputField.textComponent = text;

            // Buttons
            var buttonRow = new GameObject("Buttons", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            buttonRow.transform.SetParent(panelObj.transform, false);
            var rowRect = buttonRow.GetComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0f);
            rowRect.anchorMax = new Vector2(0.5f, 0f);
            rowRect.pivot = new Vector2(0.5f, 0f);
            rowRect.sizeDelta = new Vector2(720f, 88f);
            rowRect.anchoredPosition = new Vector2(0f, 32f);

            var layout = buttonRow.GetComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 16f;
            layout.childControlHeight = true;
            layout.childControlWidth = false;

            var cancelButton = CreateButton("CancelButton", "取消", false);
            cancelButton.transform.SetParent(buttonRow.transform, false);

            var confirmButton = CreateButton("ConfirmButton", "确认", false);
            confirmButton.transform.SetParent(buttonRow.transform, false);

            // Note: 需要自定义 UIInputDialog 组件来处理逻辑
            // 这里仅生成基础结构

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.DialogInput);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.DialogInput}");
        }

        private static GameObject CreateButton(string name, string text, bool isPrimary)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            var image = root.GetComponent<Image>();
            image.color = isPrimary ? UIGeneratorUtility.ColorPrimary : UIGeneratorUtility.ColorSecondary;

            var button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = image;
            button.colors = UIGeneratorUtility.BuildButtonColors(isPrimary);

            root.AddComponent<UIButtonScaleFeedback>();

            var layout = root.GetComponent<LayoutElement>();
            layout.preferredWidth = 320f;
            layout.preferredHeight = 88f;

            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 12f);
            labelRect.offsetMax = new Vector2(-16f, -12f);

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = text;
            label.alignment = TextAlignmentOptions.Center;
            label.color = isPrimary ? Color.white : UIGeneratorUtility.ColorText;
            label.fontSize = 22f;
            UIGeneratorUtility.TryAssignDefaultFont(label);

            return root;
        }
    }
}
