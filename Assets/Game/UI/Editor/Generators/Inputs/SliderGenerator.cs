using Game.UI.Editor.Shared;
using Game.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Generators.Inputs
{
    /// <summary>
    /// 滑块生成器
    /// </summary>
    public static class SliderGenerator
    {
        private const string Dir = "Assets/Game/UI/Common/Inputs";
        private const string FileName = "UI_Slider.prefab";

        [MenuItem("Tools/Game UI/Generators/Inputs/生成滑块")]
        public static void Generate()
        {
            UIGeneratorUtility.EnsureFolder(Dir);

            var path = $"{Dir}/{FileName}";
            var root = new GameObject("UI_Slider", typeof(RectTransform), typeof(Slider));
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400f, 32f);

            // Background
            var bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(root.transform, false);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.5f);
            bgRect.anchorMax = new Vector2(1f, 0.5f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.sizeDelta = new Vector2(0f, 8f);

            var bgImage = bgObj.GetComponent<Image>();
            bgImage.color = UIGeneratorUtility.Hex("#E0E0E0");

            // Fill Area
            var fillAreaObj = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaObj.transform.SetParent(root.transform, false);
            var fillAreaRect = fillAreaObj.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.5f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.5f);
            fillAreaRect.pivot = new Vector2(0.5f, 0.5f);
            fillAreaRect.sizeDelta = new Vector2(0f, 8f);

            // Fill
            var fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var fillImage = fillObj.GetComponent<Image>();
            fillImage.color = UIGeneratorUtility.ColorPrimary;

            // Handle Slide Area
            var handleAreaObj = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleAreaObj.transform.SetParent(root.transform, false);
            var handleAreaRect = handleAreaObj.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(16f, 0f);
            handleAreaRect.offsetMax = new Vector2(-16f, 0f);

            // Handle
            var handleObj = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handleObj.transform.SetParent(handleAreaObj.transform, false);
            var handleRect = handleObj.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(24f, 24f);

            var handleImage = handleObj.GetComponent<Image>();
            handleImage.color = UIGeneratorUtility.ColorPrimary;

            var slider = root.GetComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.wholeNumbers = false;
            slider.value = 50f;

            UIGeneratorUtility.SavePrefab(path, root);
            UIGeneratorUtility.MarkAddressable(path, UIKeys.Common.Slider);

            AssetDatabase.SaveAssets();
            Debug.Log($"[UI生成器] ✅ 已生成：{UIKeys.Common.Slider}");
        }
    }
}
