---
trigger: always_on
---

# 项目规则 (Project Rules)

> **System Instruction**: You MUST strictly adhere to these rules. Deviating from the "Forbidden" rules is considered a critical error.

## 🎯 项目架构
- **Unity + VContainer DI**
- **Core / Game** 子模块严格分离

## 📝 编码约束
### C# 规范
- 私有字段使用 `_camelCase`（通过 `.editorconfig` 强制）
- 命名空间从 `Assets` 下级开始，如 `Game.Gameplay.Runtime`
- **日志/异常必须中文**，并包含关键参数值（ids, names, counts）
- 禁止魔法数字，统一使用 `const` 或 `ScriptableObject` 配置

### 🌍 国际化与常量 (Localization & Constants)
- **严禁硬编码 UI 文本**：核心逻辑代码中（特别是 Service/ViewModel 层）**严禁**出现用户可见的硬编码文本。
- **接入本地化服务**：UI 显示的文本必须通过 `ILocalizationService` 获取。
- **常量定义**：状态名、阶段名、Key 等标识性字符串必须定义为 `public const string`（如 `const string Phase_Unload = "Phase_Unload"`），**禁止**使用魔法字符串（Magic Strings）。

### 📖 文档与注释 (Documentation)
- **接口注释**：所有公共接口（Interface/Public Method）必须包含标准的 XML 文档注释。
- **复杂逻辑说明**：对于复杂的业务逻辑（如状态切换、异步流程控制），必须在代码中使用 `<remarks>` 标签或详细的行内注释进行说明。

### 依赖注入 (VContainer)
- 多接口/参数注入采用链式注册 `builder.Register<T>(Lifetime.Scoped).AsImplementedInterfaces();`
- **构造函数禁止异步逻辑**，必须使用 `IStartable.Start()` 或 `IInitializable`

### 异常处理
- 使用语义化异常类型（`ArgumentNullException`、`InvalidOperationException`）
- **禁止空 catch**，必须记录日志后 `throw;`（保留堆栈）
- **OperationCanceledException**：在资源清理/取消流程中应被捕获并记录为 Info/Debug（不作为错误），除非是必须中断的关键流程。

### Unity 生命周期
- 优先使用 **Plain C# 类 + VContainer**，仅 View/Collider 组件继承 `MonoBehaviour`
- **禁止协程 (IEnumerator)**，全局统一改为 **UniTask**

## ⚡ 技术栈
### UniTask (异步与取消)
- **强制使用**，禁止 `System.Threading.Tasks.Task`
- **统一取消模式**：
  - 所有持有异步状态的类（Service/Controller）必须维护 `CancellationTokenSource _cts`。
  - `Dispose()` 中必须调用 `_cts.Cancel()` 和 `_cts.Dispose()`。
- **禁止保护性检查**：
  - 禁止在 `Dispose` 或异步连续调用中使用 `if (!Application.isPlaying)` 或 `if (go == null)` 来掩盖生命周期问题。
  - 必须使用 `CancellationToken` 提前退出或处理 `OperationCanceledException`。
- **链接令牌**：
  - 接受外部 `CancellationToken` 的方法，通常应使用 `CancellationTokenSource.CreateLinkedTokenSource(externalCt, _internalCts.Token)` 链接内部生命周期。
- Fire‑and‑Forget 必须调用 `.Forget()` 扩展方法

### R3 (响应式)
- 所有订阅 (`.Subscribe`) 必须 `Dispose`（推荐 `.AddTo(destroyCancellationToken)`）
- 避免在 Update 中进行高频 LINQ/闭包分配

### Addressables
- **禁止直接调用 `Addressables.Load*`**，必须注入并使用 `IAssetProvider`
- 句柄需缓存，失败/取消时及时释放
- `Addressables.InstantiateAsync` 的实例必须通过 `ReleaseInstance` 销毁

## 🧪 测试规范 (Testing Specifications)
- **Core 层单元测试**：Core 层的所有服务（Service）必须包含对应的单元测试。
- **覆盖率要求**：单元测试覆盖率必须 **> 80%**。

## 🏗 架构层级
1. **CoreLifetimeScope** – 全局单例，注册日志、资源、场景服务
2. **GameLifetimeScope** – 游戏主循环，加载 `GameConfigManifest`
3. **场景 Scope** – `MenuLifetimeScope` / `GameplayLifetimeScope`

### UI 架构
- 层级：`Main → HUD → Overlay → Loading → Transition`
- **访问**：统一通过 `IUIRootService.GetLayer(UILayer)`
- **资源**：通过 `IUIFactory` 加载

## 📦 Git 工作流
- **提交规范**：`<type>(<scope>): <description>`（中文）
- **子模块**：Core 变更需 **两次提交**（Submodule commit + Main repo commit）
- **编码**：文件统一 **UTF-8**

---

## 🧩 核心模式范例 (Pattern Examples)

### ✅ VContainer 初始化 (VS Monobehaviour)
```csharp
// [Good]
public class GameInitializer : IStartable {
    private readonly IAssetProvider _assets;
    public GameInitializer(IAssetProvider assets) => _assets = assets; // 仅注入
    
    public void Start() {
        InitializeAsync().Forget(); // Fire-and-forget
    }
}

// [Bad]
public class BadManager : MonoBehaviour {
    IEnumerator Start() { ... } // 禁止协程
    async void Start() { ... }  // 禁止 async void
}
```

### ✅ 资源加载与取消 (Safe Asset Loading & Cancellation)
```csharp
// [Good]
public async UniTask DoWorkAsync(CancellationToken externalCt) {
    // 链接外部取消请求与内部生命周期
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalCt, _cts.Token);
    var ct = linkedCts.Token;

    try {
        ct.ThrowIfCancellationRequested(); // 早期检查
        var prefab = await _assetProvider.LoadAssetAsync<GameObject>("Hero", ct);
    } catch (OperationCanceledException) {
        // 优雅退出，不报 Error
        _logService.Debug("操作已取消");
    }
}

// [Bad]
public async UniTask DoWorkAsync() {
    // 禁止依赖 isPlaying 检查
    if (!Application.isPlaying) return; 
    
    var prefab = await Addressables.LoadAssetAsync<GameObject>("Hero"); // 禁止直接调用
}
```

### ✅ 响应式订阅 (R3 Safety)
```csharp
// [Good]
_playerHealth.Current
    .Where(h => h < 10)
    .Subscribe(h => ShowLowHealthWarning(h))
    .AddTo(this.GetCancellationTokenOnDestroy()); // 必防止泄漏

// [Bad]
_playerHealth.Current.Subscribe(...); // 缺少 Dispose 管理
```# 项目规则 (Project Rules)

> **System Instruction**: You MUST strictly adhere to these rules. Deviating from the "Forbidden" rules is considered a critical error.

## 🎯 项目架构
- **Unity + VContainer DI**
- **Core / Game** 子模块严格分离

## 📝 编码约束
### C# 规范
- 私有字段使用 `_camelCase`（通过 `.editorconfig` 强制）
- 命名空间从 `Assets` 下级开始，如 `Game.Gameplay.Runtime`
- **日志/异常必须中文**，并包含关键参数值（ids, names, counts）
- 禁止魔法数字，统一使用 `const` 或 `ScriptableObject` 配置

### 🌍 国际化与常量 (Localization & Constants)
- **严禁硬编码 UI 文本**：核心逻辑代码中（特别是 Service/ViewModel 层）**严禁**出现用户可见的硬编码文本。
- **接入本地化服务**：UI 显示的文本必须通过 `ILocalizationService` 获取。
- **常量定义**：状态名、阶段名、Key 等标识性字符串必须定义为 `public const string`（如 `const string Phase_Unload = "Phase_Unload"`），**禁止**使用魔法字符串（Magic Strings）。

### 📖 文档与注释 (Documentation)
- **接口注释**：所有公共接口（Interface/Public Method）必须包含标准的 XML 文档注释。
- **复杂逻辑说明**：对于复杂的业务逻辑（如状态切换、异步流程控制），必须在代码中使用 `<remarks>` 标签或详细的行内注释进行说明。

### 依赖注入 (VContainer)
- 多接口/参数注入采用链式注册 `builder.Register<T>(Lifetime.Scoped).AsImplementedInterfaces();`
- **构造函数禁止异步逻辑**，必须使用 `IStartable.Start()` 或 `IInitializable`

### 异常处理
- 使用语义化异常类型（`ArgumentNullException`、`InvalidOperationException`）
- **禁止空 catch**，必须记录日志后 `throw;`（保留堆栈）
- **OperationCanceledException**：在资源清理/取消流程中应被捕获并记录为 Info/Debug（不作为错误），除非是必须中断的关键流程。

### Unity 生命周期
- 优先使用 **Plain C# 类 + VContainer**，仅 View/Collider 组件继承 `MonoBehaviour`
- **禁止协程 (IEnumerator)**，全局统一改为 **UniTask**

## ⚡ 技术栈
### UniTask (异步与取消)
- **强制使用**，禁止 `System.Threading.Tasks.Task`
- **统一取消模式**：
  - 所有持有异步状态的类（Service/Controller）必须维护 `CancellationTokenSource _cts`。
  - `Dispose()` 中必须调用 `_cts.Cancel()` 和 `_cts.Dispose()`。
- **禁止保护性检查**：
  - 禁止在 `Dispose` 或异步连续调用中使用 `if (!Application.isPlaying)` 或 `if (go == null)` 来掩盖生命周期问题。
  - 必须使用 `CancellationToken` 提前退出或处理 `OperationCanceledException`。
- **链接令牌**：
  - 接受外部 `CancellationToken` 的方法，通常应使用 `CancellationTokenSource.CreateLinkedTokenSource(externalCt, _internalCts.Token)` 链接内部生命周期。
- Fire‑and‑Forget 必须调用 `.Forget()` 扩展方法

### R3 (响应式)
- 所有订阅 (`.Subscribe`) 必须 `Dispose`（推荐 `.AddTo(destroyCancellationToken)`）
- 避免在 Update 中进行高频 LINQ/闭包分配

### Addressables
- **禁止直接调用 `Addressables.Load*`**，必须注入并使用 `IAssetProvider`
- 句柄需缓存，失败/取消时及时释放
- `Addressables.InstantiateAsync` 的实例必须通过 `ReleaseInstance` 销毁

## 🧪 测试规范 (Testing Specifications)
- **Core 层单元测试**：Core 层的所有服务（Service）必须包含对应的单元测试。
- **覆盖率要求**：单元测试覆盖率必须 **> 80%**。

## 🏗 架构层级
1. **CoreLifetimeScope** – 全局单例，注册日志、资源、场景服务
2. **GameLifetimeScope** – 游戏主循环，加载 `GameConfigManifest`
3. **场景 Scope** – `MenuLifetimeScope` / `GameplayLifetimeScope`

### UI 架构
- 层级：`Main → HUD → Overlay → Loading → Transition`
- **访问**：统一通过 `IUIRootService.GetLayer(UILayer)`
- **资源**：通过 `IUIFactory` 加载

## 📦 Git 工作流
- **提交规范**：`<type>(<scope>): <description>`（中文）
- **子模块**：Core 变更需 **两次提交**（Submodule commit + Main repo commit）
- **编码**：文件统一 **UTF-8**

---

## 🧩 核心模式范例 (Pattern Examples)

### ✅ VContainer 初始化 (VS Monobehaviour)
```csharp
// [Good]
public class GameInitializer : IStartable {
    private readonly IAssetProvider _assets;
    public GameInitializer(IAssetProvider assets) => _assets = assets; // 仅注入
    
    public void Start() {
        InitializeAsync().Forget(); // Fire-and-forget
    }
}

// [Bad]
public class BadManager : MonoBehaviour {
    IEnumerator Start() { ... } // 禁止协程
    async void Start() { ... }  // 禁止 async void
}
```

### ✅ 资源加载与取消 (Safe Asset Loading & Cancellation)
```csharp
// [Good]
public async UniTask DoWorkAsync(CancellationToken externalCt) {
    // 链接外部取消请求与内部生命周期
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalCt, _cts.Token);
    var ct = linkedCts.Token;

    try {
        ct.ThrowIfCancellationRequested(); // 早期检查
        var prefab = await _assetProvider.LoadAssetAsync<GameObject>("Hero", ct);
    } catch (OperationCanceledException) {
        // 优雅退出，不报 Error
        _logService.Debug("操作已取消");
    }
}

// [Bad]
public async UniTask DoWorkAsync() {
    // 禁止依赖 isPlaying 检查
    if (!Application.isPlaying) return; 
    
    var prefab = await Addressables.LoadAssetAsync<GameObject>("Hero"); // 禁止直接调用
}
```

### ✅ 响应式订阅 (R3 Safety)
```csharp
// [Good]
_playerHealth.Current
    .Where(h => h < 10)
    .Subscribe(h => ShowLowHealthWarning(h))
    .AddTo(this.GetCancellationTokenOnDestroy()); // 必防止泄漏

// [Bad]
_playerHealth.Current.Subscribe(...); // 缺少 Dispose 管理
```