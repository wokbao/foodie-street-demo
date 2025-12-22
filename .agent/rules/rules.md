---
trigger: always_on
---

# 工作区项目规则

## 🎯 项目架构
Unity + VContainer DI，Core/Game 子模块分离。

## 📝 编码约束

### C# 规范
- 遵循 C# 标准编码规范（私有字段 `_camelCase`，.editorconfig 强制）
- 命名空间从 `Assets` 下级开始：`Game.Gameplay.Runtime`
- **所有日志/异常必须中文**，包含具体参数值
- 禁止魔法数字，使用常量或 ScriptableObject 配置

### 依赖注入 (VContainer)
- 多接口/参数注入必须链式注册，简单单接口可单行
- **禁止构造函数异步逻辑**，改用 `IStartable` / `IInitializable`

### 异常处理
- 使用语义化类型（`ArgumentNullException`, `InvalidOperationException`）
- 避免空 catch，必须记录日志
- 重新抛出用 `throw;` 保留堆栈

### Unity 生命周期
- **优先 Plain C# 类** + VContainer，避免过度使用 MonoBehaviour
- 需要 Unity 生命周期才用 MonoBehaviour（Transform、Collider 等）
- **禁止协程**，全部改用 UniTask

## ⚡ 技术栈

### UniTask (异步)
- **强制使用**，禁止 `System.Threading.Tasks.Task`
- 所有异步 API 必须支持 `CancellationToken`
- Fire-and-forget 必须 `.Forget()`

### R3 (响应式)
- 所有订阅必须 Dispose，推荐 `.AddTo(destroyCancellationToken)`
- 避免高频流闭包（GC 优化）

### Addressables
- **仅通过 `IAssetProvider` 访问**，禁止直接调用 Addressables API
- 句柄必须缓存，失败/取消时释放
- 加载 API 必须接收 `CancellationToken`
- `Addressables.Instantiate` 实例需 `ReleaseInstance`

## 🏗 架构层级

### LifetimeScope 层级
1. **CoreLifetimeScope**: 全局，注册核心服务（日志、资源、场景）
2. **GameLifetimeScope**: DontDestroyOnLoad，加载 `GameConfigManifest`
3. **场景 Scope**: MenuLifetimeScope / GameplayLifetimeScope，Parent 指向 Game

### 代码组织
- 结构：`Domain / Feature / Abstractions / Runtime`
- 配置：ScriptableObject
- Core 层极其谨慎，影响所有项目

### UI 架构
- **层级**: Main / HUD / Overlay / Loading / Transition（严格分层）
- **访问**: 统一通过 `IUIRootService.GetLayer(UILayer.Xxx)`
- **资源**: 通过 `IUIFactory` 加载，禁止直接访问 Addressables

## 📦 Git 工作流

### 提交规范
- 格式：`<type>(<scope>): <description>`（中文）
- 类型：feat / fix / refactor / perf / docs / style / test / chore
- 示例：`feat(core): 实现场景加载系统`

### 子模块
- Core 修改需两次提交（子模块 + 主仓库）
- 提交前确保正确分支（非分离头指针）
- 拉取后：`git submodule update --init --recursive`

### 编码
- UTF-8，PowerShell 需 `chcp 65001`

## 📚 文档同步（AI 核心职责）

### 必更新文档
- **DEV_NOTES.md**: 当日工作内容（每次代码修改必更新）
- **ROADMAP.md**: 勾选完成任务
- **PROJECT_STRUCTURE.md**: 架构变更时更新

### 修改纪律
- **仅增量修改**，禁止整篇重写
- 修改前 `git diff` 确认范围，修改后验证
- 发现 UTF-8 乱码直接覆盖，不做编码转换

## 🧪 代码质量

### 必须遵守
- 公共 API 含 XML 文档注释
- 避免 LINQ / foreach 闭包（GC 优化）
- 关键功能须有 EditMode 单元测试

### 日志规范
- Information: 正常流程关键节点
- Warning: 可恢复异常、降级逻辑
- Error: 不可恢复错误 + Exception 对象

## 🚨 关键提醒

1. **Core 层修改需极其谨慎**，影响所有子项目
2. 子模块默认分离头指针，提交前必检查
3. 新对话先 `git pull` + `git submodule update`
4. Markdown 统一 UTF-8