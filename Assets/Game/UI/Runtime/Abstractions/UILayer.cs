namespace Game.UI.Runtime.Abstractions
{
    /// <summary>
    /// UI 层类型：用于统一获取 UI 父节点并控制排序层级。
    /// </summary>
    public enum UILayer
    {
        Main = 0,
        Hud = 1,
        Overlay = 2,
        Loading = 3,
        Transition = 4
    }
}

