using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Test
{
    /// <summary>
    /// 测试文件 - 用于验证 EditorConfig 和 Git Hooks
    /// 这个文件包含了多个故意违反规范的示例
    /// </summary>
    public class CodeCheckTest : MonoBehaviour
    {
        // ❌ 测试 1: 私有字段命名错误（缺少下划线）
        // EditorConfig 应该在这里显示警告或错误
        private int testCount;

        // ✅ 正确的命名
        private int _correctCount;

        // ❌ 测试 2: 如果取消注释下面的代码，Git Hook 会拦截
        // private async void Start()
        // {
        //     await Task.Delay(1000);
        // }

        // ✅ 正确的异步方法
        private void Start()
        {
            TestAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask TestAsync(CancellationToken ct)
        {
            await UniTask.Delay(1000, cancellationToken: ct);
        }
    }
}
