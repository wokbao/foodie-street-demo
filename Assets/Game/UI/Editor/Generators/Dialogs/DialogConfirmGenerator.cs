using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Dialogs
{
    /// <summary>
    /// 确认弹窗生成器
    /// </summary>
    public static class DialogConfirmGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Dialogs";
        private const string FileName = "UI_DialogConfirm.prefab";

        [MenuItem("Tools/Game UI/Generators/Dialogs/生成确认弹窗")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            
            // 创建根节点
            var root = new GameObject("UI_DialogConfirm", typeof(RectTransform));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // 添加 Animator 容器
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

            // 遮罩
            var overlayObj = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
            overlayObj.transform.SetParent(animatorRoot.transform, false);
            var overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayObj.GetComponent<Image>().color = UIGeneratorUtility.ColorOverlay;

            // 面板
            var panelObj = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(animatorRoot.transform, false);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(820f, 420f);
            panelRect.anchoredPosition = Vector2.zero;
            panelObj.GetComponent<Image>().color = UIGeneratorUtility.ColorSecondary;

            // 消息文本
            var messageObj = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
            messageObj.transform.SetParent(panelObj.transform, false);
            var messageRect = messageObj.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0f, 0f);
            messageRect.anchorMax = new Vector2(1f, 1f);
            messageRect.offsetMin = new Vector2(32f, 120f);
            messageRect.offsetMax = new Vector2(-32f, -120f);

            var message = messageObj.GetComponent<TextMeshProUGUI>();
            message.text = "确定要执行此操作吗？";
            message.alignment = TextAlignmentOptions.Center;
            message.color = UIGeneratorUtility.ColorText;
            message.fontSize = 20f;
            UIGeneratorUtility.TryAssignDefaultFont(message);

            // 按钮行
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

            // 取消按钮
            var cancelButton = CreateButton("CancelButton", "取消", false);
            cancelButton.transform.SetParent(buttonRow.transform, false);

            // 确认按钮
            var confirmButton = CreateButton("ConfirmButton", "确认", false);
            confirmButton.transform.SetParent(buttonRow.transform, false);

            // 添加 Dialog 组件
            var dialog = root.AddComponent<UIConfirmDialog>();
            dialog.EditorWireUp(message, confirmButton.GetComponent<Button>(), cancelButton.GetComponent<Button>());

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.DialogConfirm);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.DialogConfirm}");
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
