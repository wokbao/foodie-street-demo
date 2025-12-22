using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Panels
{
    /// <summary>
    /// 卡片面板生成器（圆角、阴影）
    /// </summary>
    public static class PanelCardGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Panels";
        private const string FileName = "UI_PanelCard.prefab";

        [MenuItem("Tools/Game UI/Generators/Panels/生成卡片面板")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_PanelCard", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400f, 300f);

            var image = root.GetComponent<Image>();
            image.color = UIGeneratorUtility.ColorSecondary;
            image.type = Image.Type.Sliced; // 支持圆角（需要Nine-Sliced Sprite）

            var contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(root.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(24f, 24f);
            contentRect.offsetMax = new Vector2(-24f, -24f);

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.PanelCard);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.PanelCard}");
        }
    }
}
