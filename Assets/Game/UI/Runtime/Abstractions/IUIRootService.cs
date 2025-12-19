using UnityEngine;

namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// 全局 UI Root 管理：负责创建 GlobalUIRoot 与各 UI 层 Canvas，并提供稳定的层级访问入口。
    /// </summary>
    public interface IUIRootService
    {
        GameObject Root { get; }

        void EnsureInitialized();

        Transform GetLayer(UILayer layer);

        Canvas GetLayerCanvas(UILayer layer);
    }
}

