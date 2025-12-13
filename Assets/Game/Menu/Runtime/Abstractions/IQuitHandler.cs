using Cysharp.Threading.Tasks;

namespace Game.Menu.Runtime.Abstractions
{
    /// <summary>
    /// 退出流程接口，可替换为确认弹窗或平台特定的退出逻辑。
    /// </summary>
    public interface IQuitHandler
    {
        /// <summary>
        /// 请求退出游戏，具体行为由实现决定（确认弹窗/直接退出/切换场景等）。
        /// </summary>
        UniTask RequestQuitAsync();
    }
}
