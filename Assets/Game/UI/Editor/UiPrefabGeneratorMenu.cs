using Game.UI.Editor.Generators.Buttons;
using Game.UI.Editor.Generators.Dialogs;
using Game.UI.Editor.Generators.Display;
using Game.UI.Editor.Generators.Inputs;
using Game.UI.Editor.Generators.Layout;
using Game.UI.Editor.Generators.Panels;
using UnityEditor;
using UnityEngine;

namespace Game.UI.Editor
{
    /// <summary>
    /// UI Prefab 生成器统一菜单入口
    /// </summary>
    public static class UiPrefabGeneratorMenu
    {
        [MenuItem("Tools/Game UI/生成所有 UI 组件")]
        public static void GenerateAll()
        {
            // 按钮类 (5)
            ButtonPrimaryGenerator.Generate();
            ButtonSecondaryGenerator.Generate();
            ButtonIconGenerator.Generate();
            ButtonTextGenerator.Generate();
            ButtonCloseGenerator.Generate();

            // 面板类 (3)
            PanelBaseGenerator.Generate();
            PanelHeaderGenerator.Generate();
            PanelCardGenerator.Generate();

            // 对话框类 (3)
            DialogConfirmGenerator.Generate();
            DialogInfoGenerator.Generate();
            DialogInputGenerator.Generate();

            // 输入组件 (4)
            InputFieldGenerator.Generate();
            SliderGenerator.Generate();
            ToggleGenerator.Generate();
            DropdownGenerator.Generate();

            // 显示组件 (3)
            TextLabelGenerator.Generate();
            ProgressBarGenerator.Generate();
            ImageFrameGenerator.Generate();

            // 布局组件 (2)
            ScrollViewGenerator.Generate();
            DividerGenerator.Generate();

            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成全部 20 个通用 UI 组件！");
        }

        [MenuItem("Tools/Game UI/按分类生成/按钮类")]
        public static void GenerateButtons()
        {
            ButtonPrimaryGenerator.Generate();
            ButtonSecondaryGenerator.Generate();
            ButtonIconGenerator.Generate();
            ButtonTextGenerator.Generate();
            ButtonCloseGenerator.Generate();
            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成 5 个按钮组件");
        }

        [MenuItem("Tools/Game UI/按分类生成/面板类")]
        public static void GeneratePanels()
        {
            PanelBaseGenerator.Generate();
            PanelHeaderGenerator.Generate();
            PanelCardGenerator.Generate();
            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成 3 个面板组件");
        }

        [MenuItem("Tools/Game UI/按分类生成/对话框类")]
        public static void GenerateDialogs()
        {
            DialogConfirmGenerator.Generate();
            DialogInfoGenerator.Generate();
            DialogInputGenerator.Generate();
            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成 3 个对话框组件");
        }

        [MenuItem("Tools/Game UI/按分类生成/输入组件")]
        public static void GenerateInputs()
        {
            InputFieldGenerator.Generate();
            SliderGenerator.Generate();
            ToggleGenerator.Generate();
            DropdownGenerator.Generate();
            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成 4 个输入组件");
        }

        [MenuItem("Tools/Game UI/按分类生成/显示组件")]
        public static void GenerateDisplays()
        {
            TextLabelGenerator.Generate();
            ProgressBarGenerator.Generate();
            ImageFrameGenerator.Generate();
            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成 3 个显示组件");
        }

        [MenuItem("Tools/Game UI/按分类生成/布局组件")]
        public static void GenerateLayouts()
        {
            ScrollViewGenerator.Generate();
            DividerGenerator.Generate();
            AssetDatabase.Refresh();
            Debug.Log("[UI生成器] ✅ 已生成 2 个布局组件");
        }
    }
}
