using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Layout
{
    /// <summary>
    /// 滚动视图生成器
    /// </summary>
    public static class ScrollViewGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Layout";
        private const string FileName = "UI_ScrollView.prefab";

        [MenuItem("Tools/Game UI/Generators/Layout/生成滚动视图")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_ScrollView", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600f, 400f);

            var image = root.GetComponent<Image>();
            image.color = Color.white;

            // Viewport
            var viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewportObj.transform.SetParent(root.transform, false);
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
            contentRect.sizeDelta = new Vector2(0f, 800f);

            var scrollRect = root.GetComponent<ScrollRect>();
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.ScrollView);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.ScrollView}");
        }
    }
}
