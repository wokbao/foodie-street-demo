using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Panels
{
    /// <summary>
    /// 基础面板生成器
    /// </summary>
    public static class PanelBaseGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Panels";
        private const string FileName = "UI_PanelBase.prefab";

        [MenuItem("Tools/Game UI/Generators/Panels/生成基础面板")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_PanelBase", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(900f, 600f);

            var image = root.GetComponent<Image>();
            image.color = UIGeneratorUtility.ColorSecondary;

            var contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(root.transform, false);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(16f, 16f);
            contentRect.offsetMax = new Vector2(-16f, -16f);

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.PanelBase);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.PanelBase}");
        }
    }
}
