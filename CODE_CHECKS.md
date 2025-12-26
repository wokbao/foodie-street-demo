# 企业级规范自动化检查系统

本项目已配置自动化代码规范检查，包括 EditorConfig 和 Git Hooks。

## 🚀 快速开始

### 1. 安装检查工具

```powershell
# Windows PowerShell
.\install-code-checks.ps1
```

### 2. 配置 IDE

**Rider / Visual Studio**:
- EditorConfig 支持已内置，无需额外配置

**VS Code**:
1. 安装扩展：`EditorConfig for VS Code`
2. 重启 VS Code

## 📋 自动检查项目

### EditorConfig 检查（编辑时）
- ✅ 私有字段命名：`_camelCase`（**强制**）
- ✅ 缩进：4 空格
- ✅ 文件编码：UTF-8 CRLF
- ✅ 异步方法未 await（编译错误）

### Git Pre-commit 检查（提交前）
- ✅ 禁止协程（`IEnumerator`）
- ✅ 禁止 `async void`
- ✅ 禁止 `System.Threading.Tasks.Task`
- ✅ 禁止空 `catch` 块
- ✅ 禁止直接调用 `Addressables.Load*`
- ✅ 文件编码检查

### Git Commit-msg 检查（提交时）
- ✅ 提交信息格式：`<type>(<scope>): <description>`
- ✅ 建议使用中文描述

## 🔧 使用示例

### 正确的提交流程

```bash
# 1. 修改代码（IDE 会自动提示命名错误）
# 2. 提交代码
git add .
git commit -m "refactor(ui): 修复按钮缩放动画"

# ✅ 自动运行所有检查
# 🔍 运行代码规范检查...
#   ✓ 检查文件编码
#   ✓ 检查禁止的代码模式
#   ✓ 检查 Addressables 使用规范
# ✅ 代码规范检查通过
# ✅ 提交信息格式检查通过
```

### 违规示例

```bash
# ❌ 使用了协程
git commit -m "fix: 修复动画"
# ❌ 错误: UIButton.cs 使用了协程（IEnumerator）
#   规范要求：禁止协程，全局统一改为 UniTask

# ❌ 提交信息格式错误
git commit -m "修复了一个bug"
# ❌ 错误: 提交信息格式不正确
# 📝 正确格式: <type>(<scope>): <description>
```

## 🛑 紧急绕过（仅紧急情况）

```bash
# 绕过所有检查（不推荐）
git commit --no-verify -m "emergency fix"
```

## 📁 文件结构

```
.
├── .editorconfig           # EditorConfig 配置
├── .githooks/              # Git Hooks 目录
│   ├── pre-commit         # 提交前检查
│   ├── commit-msg         # 提交信息检查
│   └── README.md          # Hooks 说明
└── install-code-checks.ps1 # 安装脚本
```

## 🔍 检查规则详情

### 1. 命名规范
```csharp
// ✅ 正确
private int _count;
private string _userName;

// ❌ 错误（IDE 会红线提示）
private int count;
private string userName;
```

### 2. 异步编程
```csharp
// ✅ 正确
private void Start()
{
    LoadAsync(cancellationToken).Forget();
}

private async UniTask LoadAsync(CancellationToken ct)
{
    await UniTask.Delay(1000, cancellationToken: ct);
}

// ❌ 错误（提交时被拦截）
private async void Start()  // async void
{
    await Task.Delay(1000);  // System.Threading.Tasks.Task
}

private IEnumerator LoadCoroutine()  // 协程
{
    yield return new WaitForSeconds(1);
}
```

### 3. 资源加载
```csharp
// ✅ 正确
public class MyService
{
    private readonly IAssetProvider _assetProvider;
    
    public async UniTask<T> LoadAsync<T>(string key, CancellationToken ct)
    {
        return await _assetProvider.LoadAssetAsync<T>(key, ct);
    }
}

// ❌ 错误（提交时被拦截）
var handle = Addressables.LoadAssetAsync<Sprite>("icon");
```

### 4. 异常处理
```csharp
// ✅ 正确
try
{
    await LoadAsync(ct);
}
catch (OperationCanceledException)
{
    _logger.Warning("加载被取消");
}
catch (Exception ex)
{
    _logger.Error("加载失败", ex);
    throw;
}

// ❌ 错误（提交时被拦截）
try
{
    await LoadAsync(ct);
}
catch (Exception)
{
    // 空 catch
}
```

## 🎯 豁免机制

某些文件需要豁免规范检查：

### EditorConfig 豁免
```csharp
#pragma warning disable IDE0051
private void OnValidate() { }  // Unity 方法
#pragma warning restore IDE0051
```

### Git Hook 豁免
以下文件/目录会被自动排除：
- `*Test*.cs` - 测试文件
- `*Editor*.cs` - 编辑器脚本
- `*AssetProvider*.cs` - 资源提供者实现
- `ConfigLoader.cs` - 配置加载器（已豁免）

## 💡 故障排除

### Hooks 未执行
```bash
# 检查 Git 配置
git config core.hooksPath
# 应该输出：.githooks

# 重新配置
git config core.hooksPath .githooks
```

### Windows 下 Hooks 失败
- 确保安装了 Git Bash
- 或使用 WSL（Windows Subsystem for Linux）

### EditorConfig 不生效
- 检查 IDE 是否安装 EditorConfig 插件
- 重启 IDE
- 检查 `.editorconfig` 文件是否存在

## 📚 参考资料

- [EditorConfig 文档](https://editorconfig.org/)
- [Git Hooks 文档](https://git-scm.com/book/zh/v2/自定义-Git-Git-钩子)
- [项目规范](./PROJECT_RULES.md)

## 🤝 贡献

如需添加新的检查规则：
1. 编辑 `.editorconfig` 或 `.githooks/pre-commit`
2. 更新本文档
3. 提交 PR
