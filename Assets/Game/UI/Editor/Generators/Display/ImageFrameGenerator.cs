using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Display
{
    /// <summary>
    /// 图片框生成器
    /// </summary>
    public static class ImageFrameGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Display";
        private const string FileName = "UI_ImageFrame.prefab";

        [MenuItem("Tools/Game UI/Generators/Display/生成图片框")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_ImageFrame", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200f, 200f);

            var image = root.GetComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.ImageFrame);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.ImageFrame}");
        }
    }
}
