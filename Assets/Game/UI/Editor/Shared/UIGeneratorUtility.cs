using System;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Editor.Shared
{
    /// <summary>
    /// UI 生成器共享工具类：颜色常量、保存/标记方法等。
    /// </summary>
    public static class UIGeneratorUtility
    {
        // 颜色常量
        public static readonly Color ColorPrimary = Hex("#FF9FB3");
        public static readonly Color ColorSecondary = Hex("#F7F7F7");
        public static readonly Color ColorAccent = Hex("#7BDFF2");
        public static readonly Color ColorText = Hex("#333333");
        public static readonly Color ColorOverlay = new Color(0f, 0f, 0f, 0.4f);

        /// <summary>
        /// 保存 Prefab 并销毁临时 GameObject。
        /// </summary>
        public static void SavePrefab(string path, GameObject root)
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

        /// <summary>
        /// 确保文件夹存在，递归创建。
        /// </summary>
        public static void EnsureFolder(string path)
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

        /// <summary>
        /// 标记资源为 Addressable。
        /// </summary>
        public static void MarkAddressable(string assetPath, string address)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("[UI生成器] 未找到 Addressables Settings，跳过标记");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrWhiteSpace(guid))
            {
                Debug.LogWarning($"[UI生成器] 未找到资源 GUID：{assetPath}");
                return;
            }

            var group = settings.FindGroup("CommonUI");
            if (group == null)
            {
                var schemas = settings.DefaultGroup != null ? settings.DefaultGroup.Schemas : null;
                group = settings.CreateGroup("CommonUI", false, false, false, schemas);
            }

            var entry = settings.CreateOrMoveEntry(guid, group, false, false);
            entry.address = address;
            entry.labels.Add("UI");
        }

        /// <summary>
        /// 尝试分配默认 TMP 字体。
        /// </summary>
        public static void TryAssignDefaultFont(TMP_Text text)
        {
            if (text == null) return;

            if (TMP_Settings.defaultFontAsset != null)
            {
                text.font = TMP_Settings.defaultFontAsset;
            }
        }

        /// <summary>
        /// 创建按钮颜色块。
        /// </summary>
        public static ColorBlock BuildButtonColors(bool isPrimary)
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

        /// <summary>
        /// 颜色色调调整。
        /// </summary>
        public static Color Tint(Color color, float factor)
        {
            return new Color(
                Mathf.Clamp01(color.r * factor),
                Mathf.Clamp01(color.g * factor),
                Mathf.Clamp01(color.b * factor),
                color.a);
        }

        /// <summary>
        /// 16 进制字符串转颜色。
        /// </summary>
        public static Color Hex(string hex)
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
