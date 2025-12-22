using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.UI.Runtime;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Game.UI.Editor
{
    /// <summary>
    /// UIKeys 验证工具：检查 UIKeys 中定义的 Key 是否在 Addressables 中存在。
    /// </summary>
    public static class UIKeysValidator
    {
        [MenuItem("Tools/UI/验证 UIKeys 与 Addressables 对应关系")]
        public static void ValidateUIKeys()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("[UIKeys验证] 未找到 Addressables 设置");
                return;
            }

            // 获取所有 Addressable 条目的 Key
            var allAddressableKeys = new HashSet<string>();
            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                foreach (var entry in group.entries)
                {
                    allAddressableKeys.Add(entry.address);
                }
            }

            // 获取 UIKeys 中所有定义的常量
            var uiKeys = GetAllUIKeys();
            
            var missingKeys = new List<string>();
            var validKeys = new List<string>();

            foreach (var key in uiKeys)
            {
                if (allAddressableKeys.Contains(key))
                {
                    validKeys.Add(key);
                }
                else
                {
                    missingKeys.Add(key);
                }
            }

            // 输出结果
            Debug.Log($"[UIKeys验证] ✅ 有效 Key：{validKeys.Count} 个");
            
            if (missingKeys.Count > 0)
            {
                Debug.LogWarning($"[UIKeys验证] ⚠️ 缺失 Key：{missingKeys.Count} 个");
                foreach (var key in missingKeys)
                {
                    Debug.LogWarning($"  - {key}");
                }
            }
            else
            {
                Debug.Log("[UIKeys验证] ✅ 所有 UIKeys 都在 Addressables 中找到对应条目");
            }
        }

        /// <summary>
        /// 通过反射获取 UIKeys 中所有定义的常量字符串
        /// </summary>
        private static List<string> GetAllUIKeys()
        {
            var keys = new List<string>();
            var uiKeysType = typeof(UIKeys);

            // 获取嵌套类（Common, Screens 等）
            var nestedTypes = uiKeysType.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
            
            foreach (var nestedType in nestedTypes)
            {
                // 获取该嵌套类中的所有常量字段
                var fields = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

                foreach (var field in fields)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        keys.Add(value);
                    }
                }
            }

            return keys;
        }
    }
}
