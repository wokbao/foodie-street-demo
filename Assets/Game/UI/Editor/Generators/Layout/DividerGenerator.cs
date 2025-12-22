using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Layout
{
    /// <summary>
    /// 分割线生成器
    /// </summary>
    public static class DividerGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Layout";
        private const string FileName = "UI_Divider.prefab";

        [MenuItem("Tools/Game UI/Generators/Layout/生成分割线")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_Divider", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400f, 2f);

            var image = root.GetComponent<Image>();
            image.color = UIGeneratorUtility.Hex("#E0E0E0");

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.Divider);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.Divider}");
        }
    }
}
