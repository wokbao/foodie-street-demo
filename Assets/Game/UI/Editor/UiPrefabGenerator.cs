using System;
using System.Globalization;
using Game.UI.Runtime;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor
{
    public static class UiPrefabGenerator
    {
        private const string ButtonsDir = "Assets/Game/UI/Prefabs/Buttons";
        private const string PanelsDir = "Assets/Game/UI/Prefabs/Panels";
        private const string DialogsDir = "Assets/Game/UI/Prefabs/Dialogs";

        private static readonly Color ColorPrimary = Hex("#FF9FB3");
        private static readonly Color ColorSecondary = Hex("#F7F7F7");
        private static readonly Color ColorAccent = Hex("#7BDFF2");
        private static readonly Color ColorText = Hex("#333333");
        private static readonly Color ColorOverlay = new Color(0f, 0f, 0f, 0.4f);

        [MenuItem("Tools/Game UI/生成基础 UI Prefab（Addressables）")]
        public static void Generate()
        {
            EnsureFolder(ButtonsDir);
            EnsureFolder(PanelsDir);
            EnsureFolder(DialogsDir);

            var buttonPrimaryPath = $"{ButtonsDir}/UI_ButtonPrimary.prefab";
            var buttonSecondaryPath = $"{ButtonsDir}/UI_ButtonSecondary.prefab";
            var panelBasePath = $"{PanelsDir}/UI_PanelBase.prefab";
            var panelWithHeaderPath = $"{PanelsDir}/UI_PanelWithHeader.prefab";
            var dialogConfirmPath = $"{DialogsDir}/UI_DialogConfirm.prefab";
            var dialogInfoPath = $"{DialogsDir}/UI_DialogInfo.prefab";

            CreateButtonPrefab(buttonPrimaryPath, "UI_ButtonPrimary", isPrimary: true);
            CreateButtonPrefab(buttonSecondaryPath, "UI_ButtonSecondary", isPrimary: false);
            CreatePanelBasePrefab(panelBasePath);
            CreatePanelWithHeaderPrefab(panelWithHeaderPath);
            CreateConfirmDialogPrefab(dialogConfirmPath);
            CreateInfoDialogPrefab(dialogInfoPath);

            MarkAddressable(buttonPrimaryPath, "UI/ButtonPrimary");
            MarkAddressable(buttonSecondaryPath, "UI/ButtonSecondary");
            MarkAddressable(panelBasePath, "UI/Panel/Base");
            MarkAddressable(panelWithHeaderPath, "UI/Panel/Header");
            MarkAddressable(dialogConfirmPath, "UI/Dialog/Confirm");
            MarkAddressable(dialogInfoPath, "UI/Dialog/Info");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateButtonPrefab(string path, string rootName, bool isPrimary)
        {
            var root = new GameObject(rootName, typeof(RectTransform), typeof(Image), typeof(Button));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(320f, 88f);

            var image = root.GetComponent<Image>();
            image.color = isPrimary ? ColorPrimary : ColorSecondary;

            var button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = image;
            button.colors = BuildButtonColors(isPrimary);

            root.AddComponent<UIButtonScaleFeedback>();

            var labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelObj.transform.SetParent(root.transform, false);
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(16f, 12f);
            labelRect.offsetMax = new Vector2(-16f, -12f);

            var label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = isPrimary ? "主按钮" : "次按钮";
            label.alignment = TextAlignmentOptions.Center;
            label.color = isPrimary ? Color.white : ColorText;
            label.fontSize = 24f;
            TryAssignDefaultFont(label);

            SavePrefab(path, root);
        }

        private static void CreatePanelBasePrefab(string path)
        {
            var root = new GameObject("UI_PanelBase", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(900f, 600f);

            var image = root.GetComponent<Image>();
            image.color = ColorSecondary;

            var contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(root.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(16f, 16f);
            contentRect.offsetMax = new Vector2(-16f, -16f);

            SavePrefab(path, root);
        }

        private static void CreatePanelWithHeaderPrefab(string path)
        {
            var root = new GameObject("UI_PanelWithHeader", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(900f, 650f);

            var image = root.GetComponent<Image>();
            image.color = ColorSecondary;

            var headerObj = new GameObject("Header", typeof(RectTransform), typeof(Image));
            headerObj.transform.SetParent(root.transform, false);
            var headerRect = headerObj.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.sizeDelta = new Vector2(0f, 96f);
            headerRect.anchoredPosition = Vector2.zero;

            var headerImage = headerObj.GetComponent<Image>();
            headerImage.color = ColorAccent;

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
            title.color = ColorText;
            title.fontSize = 28f;
            TryAssignDefaultFont(title);

            var closeButton = CreateButtonObject("CloseButton", "关闭", isPrimary: false);
            closeButton.transform.SetParent(headerObj.transform, false);
            var closeRect = closeButton.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1f, 0.5f);
            closeRect.anchorMax = new Vector2(1f, 0.5f);
            closeRect.pivot = new Vector2(1f, 0.5f);
            closeRect.sizeDelta = new Vector2(96f, 56f);
            closeRect.anchoredPosition = new Vector2(-24f, 0f);

            var contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(root.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(16f, 16f);
            contentRect.offsetMax = new Vector2(-16f, -112f);

            var panel = root.AddComponent<UIPanelWithHeader>();
            panel.EditorWireUp(titleObj.GetComponent<TextMeshProUGUI>(), closeButton.GetComponent<Button>(), contentRect);

            SavePrefab(path, root);
        }

        private static void CreateConfirmDialogPrefab(string path)
        {
            var root = new GameObject("UI_DialogConfirm", typeof(RectTransform), typeof(CanvasGroup));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var canvasGroup = root.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            var overlayObj = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
            overlayObj.transform.SetParent(root.transform, false);
            var overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayObj.GetComponent<Image>().color = ColorOverlay;

            var panelObj = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(root.transform, false);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(820f, 420f);
            panelRect.anchoredPosition = Vector2.zero;
            panelObj.GetComponent<Image>().color = ColorSecondary;

            var titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(panelObj.transform, false);
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(0f, 64f);
            titleRect.anchoredPosition = new Vector2(0f, -24f);

            var title = titleObj.GetComponent<TextMeshProUGUI>();
            title.text = "确认退出";
            title.alignment = TextAlignmentOptions.Center;
            title.color = ColorText;
            title.fontSize = 28f;
            TryAssignDefaultFont(title);

            var messageObj = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
            messageObj.transform.SetParent(panelObj.transform, false);
            var messageRect = messageObj.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0f, 0f);
            messageRect.anchorMax = new Vector2(1f, 1f);
            messageRect.offsetMin = new Vector2(32f, 120f);
            messageRect.offsetMax = new Vector2(-32f, -120f);

            var message = messageObj.GetComponent<TextMeshProUGUI>();
            message.text = "确定要退出游戏吗？";
            message.alignment = TextAlignmentOptions.Center;
            message.color = ColorText;
            message.fontSize = 20f;
            TryAssignDefaultFont(message);

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
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            var cancelButton = CreateButtonObject("CancelButton", "取消", isPrimary: false);
            cancelButton.transform.SetParent(buttonRow.transform, false);
            cancelButton.GetComponent<RectTransform>().sizeDelta = new Vector2(320f, 88f);

            var confirmButton = CreateButtonObject("ConfirmButton", "确认", isPrimary: true);
            confirmButton.transform.SetParent(buttonRow.transform, false);
            confirmButton.GetComponent<RectTransform>().sizeDelta = new Vector2(320f, 88f);

            var dialog = root.AddComponent<UIConfirmDialog>();
            dialog.EditorWireUp(canvasGroup, panelRect, message, confirmButton.GetComponent<Button>(), cancelButton.GetComponent<Button>());

            SavePrefab(path, root);
        }

        private static void CreateInfoDialogPrefab(string path)
        {
            var root = new GameObject("UI_DialogInfo", typeof(RectTransform), typeof(CanvasGroup));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var canvasGroup = root.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            var overlayObj = new GameObject("Overlay", typeof(RectTransform), typeof(Image));
            overlayObj.transform.SetParent(root.transform, false);
            var overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayObj.GetComponent<Image>().color = ColorOverlay;

            var panelObj = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(root.transform, false);
            var panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(820f, 360f);
            panelRect.anchoredPosition = Vector2.zero;
            panelObj.GetComponent<Image>().color = ColorSecondary;

            var messageObj = new GameObject("Message", typeof(RectTransform), typeof(TextMeshProUGUI));
            messageObj.transform.SetParent(panelObj.transform, false);
            var messageRect = messageObj.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0f, 0f);
            messageRect.anchorMax = new Vector2(1f, 1f);
            messageRect.offsetMin = new Vector2(32f, 120f);
            messageRect.offsetMax = new Vector2(-32f, -120f);

            var message = messageObj.GetComponent<TextMeshProUGUI>();
            message.text = "提示内容";
            message.alignment = TextAlignmentOptions.Center;
            message.color = ColorText;
            message.fontSize = 20f;
            TryAssignDefaultFont(message);

            var okButton = CreateButtonObject("OkButton", "知道了", isPrimary: true);
            okButton.transform.SetParent(panelObj.transform, false);
            var okRect = okButton.GetComponent<RectTransform>();
            okRect.anchorMin = new Vector2(0.5f, 0f);
            okRect.anchorMax = new Vector2(0.5f, 0f);
            okRect.pivot = new Vector2(0.5f, 0f);
            okRect.sizeDelta = new Vector2(320f, 88f);
            okRect.anchoredPosition = new Vector2(0f, 32f);

            var dialog = root.AddComponent<UIInfoDialog>();
            dialog.EditorWireUp(canvasGroup, panelRect, message, okButton.GetComponent<Button>());

            SavePrefab(path, root);
        }

        private static GameObject CreateButtonObject(string name, string text, bool isPrimary)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            var image = root.GetComponent<Image>();
            image.color = isPrimary ? ColorPrimary : ColorSecondary;

            var button = root.GetComponent<Button>();
            button.transition = Selectable.Transition.ColorTint;
            button.targetGraphic = image;
            button.colors = BuildButtonColors(isPrimary);

            root.AddComponent<UIButtonScaleFeedback>();

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
            label.color = isPrimary ? Color.white : ColorText;
            label.fontSize = 22f;
            TryAssignDefaultFont(label);

            return root;
        }

        private static ColorBlock BuildButtonColors(bool isPrimary)
        {
            var normal = isPrimary ? ColorPrimary : ColorSecondary;
            var highlighted = isPrimary ? Tint(ColorPrimary, 1.08f) : Tint(ColorSecondary, 0.95f);
            var pressed = isPrimary ? Tint(ColorPrimary, 0.92f) : Tint(ColorSecondary, 0.9f);
            var disabled = Hex("#E6E6E6");

            return new ColorBlock
            {
                normalColor = normal,
                highlightedColor = highlighted,
                pressedColor = pressed,
                selectedColor = highlighted,
                disabledColor = disabled,
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };
        }

        private static void SavePrefab(string path, GameObject root)
        {
            try
            {
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/");
            var name = System.IO.Path.GetFileName(path);

            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException($"无效路径：{path}");
            }

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, name);
        }

        private static void MarkAddressable(string assetPath, string address)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("[UI] 未找到 Addressables Settings，跳过 Addressable 标记。");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrWhiteSpace(guid))
            {
                Debug.LogWarning($"[UI] 未找到资源 GUID：{assetPath}");
                return;
            }

            var group = settings.FindGroup("GameUI");
            if (group == null)
            {
                var schemas = settings.DefaultGroup != null ? settings.DefaultGroup.Schemas : null;
                group = settings.CreateGroup("GameUI", false, false, false, schemas);
            }
            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address;
            entry.labels.Add("UI");
        }

        private static void TryAssignDefaultFont(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return;
            }

            if (TMP_Settings.defaultFontAsset != null)
            {
                text.font = TMP_Settings.defaultFontAsset;
            }
        }

        private static Color Tint(Color color, float factor)
        {
            return new Color(
                Mathf.Clamp01(color.r * factor),
                Mathf.Clamp01(color.g * factor),
                Mathf.Clamp01(color.b * factor),
                color.a);
        }

        private static Color Hex(string hex)
        {
            var raw = hex.Trim().TrimStart('#');
            if (raw.Length != 6)
            {
                return Color.magenta;
            }

            var r = byte.Parse(raw.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(raw.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(raw.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
