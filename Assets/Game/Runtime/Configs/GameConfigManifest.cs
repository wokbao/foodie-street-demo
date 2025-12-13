using System.Collections.Generic;
using Core.Runtime.Configuration;
using UnityEngine;

namespace Game.Runtime.Configs
{
    /// <summary>
    /// Game 层配置清单，存放 Addressables Key。
    /// Core 可复用时无需依赖这些配置；本清单仅服务于本项目的 Game 层。
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfigManifest", menuName = "Game/Config Manifest")]
    public sealed class GameConfigManifest : ScriptableObject, IConfigManifest
    {
        [Header("配置条目")]
        [Tooltip("所有需要加载的 Game 配置")]
        [SerializeField] private List<ConfigManifest.ConfigEntry> _entries = new();

        public IReadOnlyList<ConfigManifest.ConfigEntry> Entries => _entries;
    }
}
