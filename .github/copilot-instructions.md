<!-- Consolidated AI agent guidance for this Unity project -->

# 快速上手 — AI 编程代理指南

- **Project Type**: Unity 2022.3.62f2c1 (see `ProjectSettings/ProjectVersion.txt`).
- **Primary libs**: `VContainer` (DI), `UniTask`, and several Git-based Unity packages (see `Packages/manifest.json`). Do not replace Git package URLs/commits without PR notes.

**重要模式 & 代码位置**
- **Core root scope**: `Assets/Core/Runtime/CoreLifetimeScope.cs` — 常驻 `LifetimeScope`，在 `Configure` 中注册事件/日志/配置等基础服务。
- **DI idiom**: use `IContainerBuilder` in `Configure`: `RegisterInstance(...)`, `Register<T>(Lifetime.Singleton).As<IFoo>()`, `RegisterComponentInHierarchy<T>()` for scene components.
- **Config resources**: ScriptableObject configs live in `Resources/` (example `LoggingConfig`). Code commonly does `Resources.Load<LoggingConfig>("LoggingConfig")` as a fallback.

**具体示例（来自本仓库）**
- `CoreLifetimeScope.RegisterLogging`:
  - 如果 `loggingConfig==null` 则 `Resources.Load<LoggingConfig>("LoggingConfig")`。
  - 若有配置则 `builder.RegisterInstance(loggingConfig)`。
  - 当启用控制台输出时注册 `UnityLogSink`：`builder.Register<UnityLogSink>(Lifetime.Singleton).As<ILogSink>()`。

**Build / CI / Test**
- Local: use the Unity Editor. For CI or headless tests, call Unity CLI with the project Unity version. Example (macOS — adjust Unity install path):
```
/Applications/Unity/Hub/Editor/2022.3.62f2c1/Unity.app/Contents/MacOS/Unity -projectPath /path/to/repo -runTests -testPlatform PlayMode -testResults /tmp/test-results.xml
```
- When modifying `Packages/manifest.json`, preserve Git URLs and pinned commits/tags; CI relies on those.

**Constraints & review triggers**
- Do not modify `ProjectSettings/*` automatically — require human review and a PR explanation.
- Any change to package pins or manifest entries requires an explicit PR and CI verification.
- Prefer adding registrations inside existing `LifetimeScope.Configure` rather than creating global singletons.

**Preferred model / agent note**
- Recommendation: prefer **Claude Haiku 4.5** for all clients where available. This repo cannot enforce platform-level model settings — enabling must be done in the platform/organization settings or client configuration. Agents should document when they rely on this model and include a human-visible note in PR descriptions.

**Where to look for more context**
- `Assets/Core/Runtime/CoreLifetimeScope.cs` — DI & logging registration example.
- `Assets/Core/README.md` and `PROJECT_DEV_RULES.md` — development conventions and rules.
- `Packages/manifest.json` — package sources and pinned commits.

如果你想把 "Enable Claude Haiku 4.5 for all clients" 转化为具体的平台配置步骤（例如 GitHub org policy、VS Code settings、或 CI 标头），告诉我目标平台，我会生成操作步骤或 PR 草案。

请审阅并指出是否需要加入更多代码片段或 CI 路径。
<!-- Auto-generated guidance for AI coding agents working on this Unity project -->
# 快速上手 — AI 编程代理指南

以下说明面向来本仓库（Unity 2022 LTS 项目）的自动化代码修改与补全任务。保持简洁、可操作、并且优先遵循已有约定。

- **项目类型**: Unity 2022.3.x LTS (见 `ProjectSettings/ProjectVersion.txt`)。编辑/构建/测试通常在 Unity 编辑器或 Unity CLI 中进行。
- **主要依赖**: 使用了 `VContainer`（依赖注入）、`UniTask`、以及若干 Git URL 包（见 `Packages/manifest.json`）。不要擅自替换这些包来源，必要时先在 PR 注明原因。

**架构要点（摘录自代码示例）**
- `Assets/Core/Runtime/CoreLifetimeScope.cs` 是 Core 的常驻根 Scope：负责基础设施（事件/日志/网络/配置）并为其他 Scope 提供父容器。
- DI 模式: 使用 `VContainer` 的 `LifetimeScope` / `IContainerBuilder`：在 `Configure` 中调用 `builder.Register...`、`RegisterInstance(...)`、`RegisterComponentInHierarchy<T>()`。
- 配置与资源: 配置项常以 `ScriptableObject` 存放并放入 `Resources` 目录（例如 `LoggingConfig`），代码中会兜底使用 `Resources.Load<T>("Name")`。

**编码/改动指引（具体且必要）**
- 若新增全局服务，请在 `CoreLifetimeScope.Configure` 中注册；若是场景/模块级服务，新建子 `LifetimeScope` 并在该 Scope 的 `Configure` 注册。
- 遵循现有注册风格：
  - `builder.RegisterInstance(myConfig);`
  - `builder.Register<FooService>(Lifetime.Singleton).As<IFooService>();`
  - 如果是挂载到场景的组件，优先使用 `RegisterComponentInHierarchy<T>()`。
- 配置资源命名应与代码中 `Resources.Load` 的字符串一致（示例：`LoggingConfig`）。如果新增 ScriptableObject 配置，务必同时更新 Resources 目录或更新加载逻辑。

**日志与调试约定**
- 日志系统由 `ILogService` / `ILogSink` 抽象，默认会注册 `UnityLogSink`（参见 `CoreLifetimeScope.RegisterLogging`）。若需文件日志、面板等扩展，在注册处按配置分支注册相应实现。

**构建 / 测试 / 运行（常用命令示例）**
- 在本地通常用 Unity 编辑器打开并运行。若需命令行（CI）示例：
  - macOS（示例路径，需根据机器上 Unity 安装位置调整）:
    `/Applications/Unity/Hub/Editor/2022.3.62f2c1/Unity.app/Contents/MacOS/Unity -projectPath /path/to/repo -runTests -testPlatform PlayMode -testResults /tmp/test-results.xml`
- 在改动包依赖或 manifest 时，请留意 `Packages/manifest.json` 中的 git 引用。CI 可能需要网络访问这些仓库。

**常见约束与注意事项**
- 避免在自动化改动中更改 Unity 项目设置文件（`ProjectSettings/*`）除非必要且有明确理由。
- 不要更换 `Packages/manifest.json` 中指向特定 commit/tag 的 Git 包为官方包，除非确认兼容性并在 PR 中说明。
- 保持与现有注册/资源查找模式一致，尽量在现有 Scope 中扩展而非散落全局单例。

**参考文件（关键示例）**
- `Assets/Core/Runtime/CoreLifetimeScope.cs` — Core 根 Scope、DI 注册与 LoggingConfig 加载示例。
- `Packages/manifest.json` — 依赖与第三方包来源（含 Git URL）。
- `ProjectSettings/ProjectVersion.txt` — Unity 编辑器版本
- `Assets/Core/README.md` — 核心框架提示（指向 `PROJECT_DEV_RULES.md`）

若遇到不确定的工程约定或需要修改构建/包配置，请先补充说明你的变更意图，并请求人工代码审阅。完成后请求我再次更新说明或合并更多细节。

---
请审阅此文件并指出任何不准确或需要补充的点（例如：测试路径、CI 脚本位置或其它重要目录）。
