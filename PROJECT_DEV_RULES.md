# 项目开发规则（Core/Game 分离 + AAA 流程）

本规则适用于 Core 独立仓库及依赖它的 Game 仓库，目标是即便是小项目也遵循 3A 项目的工程与质量标准。

## 目标与范围
- Core 仓库：通用框架、基础系统、工具与可复用资产（DI 组合、事件总线、时间系统、存档、配置、战斗/关卡通用模块等）。
- Game 仓库：具体玩法逻辑与美术资源；引用 Core 作为子模块或包，仅在需要时扩展不在 Core 中的垂直能力。
- 所有跨仓库协议（接口、数据格式、事件、配置）必须在 Core 定义并文档化；禁止在 Game 中定义 Core 需要再回滚的协议。

## 技术栈基线
- Unity：使用最新 LTS 版本（无特殊情况不跨大版本），启用增量 GC，验证安卓/iOS/PC 主平台的 Player Settings 一致性。
- 语言：C# 10/ .NET Standard 2.1（保持包兼容）；分析器使用 Roslyn/StyleCop/FxCop，警告视为错误。
- DI：VContainer（见下方规范）；反射注入仅限启动阶段，运行期不做动态组装。
- 响应式：UniRx（强制使用 CompositeDisposable 管理生命周期）；事件驱动优先，避免手写 Update 轮询。
- 异步：UniTask（禁止裸 Thread/Task.Run，跨帧/IO 统一用 UniTask）；异步 API 默认可取消和有超时。

## 仓库与分支策略
- 双仓库：Game 将 Core 作为子模块或 UPM 包引用；版本推进采用语义化版本号（Core: MAJOR.MINOR.PATCH）。
- 分支：主分支 `main`（随时可发布），功能分支 `feature/<module>`，预发布 `release/<version>`，紧急修复 `hotfix/<issue>`。
- 合并：必须通过 PR，至少 1 名核心审核 + CI 绿灯；禁止直接向 `main` 推送。
- 版本标签：每次上线打 `vX.Y.Z` tag；Core 更新后同步在 Game 仓库更新依赖记录与变更日志。

## 目录与资源规范
- 代码：`Assets/Core/Scripts/<Domain>/<Feature>`；接口/抽象放 `Abstractions`，实现放 `Runtime`，编辑器工具放 `Editor`。
- 预制体/资源：`Assets/Core/Prefabs/<System>`、`Assets/Core/ArtPlaceholders`（核心不存放最终美术）；命名使用 `PascalCase`，带功能后缀。
- 配置：ScriptableObject 统筹存放 `Assets/Core/Configs`，JSON/表格需有导入管线；配置结构在 Core 定义，Game 仅扩展字段。
- 场景：Core 仅提供系统级引导或测试场景（如 `Core_Bootstrap.unity`），游戏正式场景只在 Game 仓库。

## 编码与风格
- C# 风格：PascalCase（类型/方法/属性），camelCase（字段/局部变量），私有字段前缀 `_`；接口以 `I` 开头；禁止缩写和魔法数字。
- 设计：遵循 SOLID/清晰边界；领域服务和平台服务通过接口注入；跨域通信首选事件流或消息管道，避免双向硬依赖。
- 文档：公共 API、协议、脚本化配置字段必须含 XML 注释；复杂类写类内使用示例或时序说明。
- 日志：使用统一 Logger/Wrapper（支持分类与采样）；日志等级默认 Info，上线 Build 只保留 Warning+Error。

## VContainer 规范
- 组合根：每个场景（或子场景）有独立 Composition Root；Game 启动从 `CoreBootstrap` 注册公共服务，再组装 Game 层服务。
- 生命周期：默认 `Transient`；状态管理/单例服务使用 `Singleton`；避免未释放的 `Scoped`；禁止在构造函数中触发异步/IO。
- 绑定：接口优先，避免直接绑定具体类；跨域接口在 Core 定义并由 Game 提供实现时，使用占位默认实现避免 Null。
- 调试：在开发环境启用验证（Validate）和循环依赖检测；容器树写单元测试保证绑定完整与无泄漏。

## UniRx 规范
- 生命周期：组件创建时注册到 `CompositeDisposable`，在 `OnDestroy`/`Dispose` 中统一释放；避免静态 Subject。
- 事件分层：Core 定义 `Message/Signal` 集合；使用 `IObservable<T>` 暴露事件，`Subject` 只在内部；场景范围的消息使用 `MessageBroker`。
- 性能：使用 `ObserveOnMainThread` 控制主线程回调；高频流合并/节流；避免在热路径频繁生成闭包和分配。

## UniTask 规范
- API 设计：公开异步方法返回 `UniTask` 或 `UniTask<T>`；提供 `CancellationToken` 参数，默认超时配置在 Core 常量。
- Fire-and-forget：禁止裸调用，需 `Forget(Debug.LogException)` 或统一的安全包装；确保任务池一致。
- 同步上下文：默认在主线程恢复；IO/CPU 密集使用 `UniTask.SwitchToThreadPool`；跨帧等待使用 `UniTask.Yield`/`NextFrame`。
- 异常：统一走错误上报与用户可见提示流；不要吞异常。

## 测试与质量门
- 单元测试：核心逻辑、DI 组合、配置解析、数据契约必须有 EditMode 测试；覆盖率基线 60%（核心模块 80%）。
- PlayMode/集成：关键系统（加载、存档、输入、网络、UI 流程）需 PlayMode 自动化用例；Game 层新增系统需附带至少 1 条冒烟用例。
- 静态检查：StyleCop/Analyzer 无警告；序列化字段、可空性、异步 API 使用规则通过 CI 检查。
- 资源校验：CI 验证命名、引用完整（无 Missing Prefab/Script），Addressables/Scenes 构建校验。
- QA 阶段：预发布版跑完整冒烟、性能快照、内存占用、加载时间、线上崩溃率监控基线；上线需通过回归清单。

## CI/CD
- 流水线阶段：Lint → EditMode → PlayMode（可裁剪）→ 打包（Dev/Release 配置）→ 构建报告与工件上传。
- 构建一致性：使用确定化构建（同版本同配置输出一致）；版本号、渠道、Player Settings 写脚本化配置。
- 产物：符号文件、性能报告、变更日志、兼容性清单必须随构建归档；标记 Core 版本。

## 性能与内存基线（3A 标准）
- 帧时间：目标 60 FPS（<16.6ms）；重场景允许 30 FPS（<33ms），需记录并告知；禁止长帧 >50ms。
- GC：帧内分配趋近 0；关键循环避免装箱与 Linq；定期 Profile（Profiler/Memory Profiler）并提交报告。
- 资源：纹理/网格/音频使用导入预设；Bundle/Addressables 有依赖与冗余检测；加载时间（主菜单 <3s，场景切换 <8s）需跟踪。
- 设备：至少验证中端安卓、iOS 近两年主力机、PC 中端配置；记录性能基线与异常。

## 错误处理与监控
- 统一的错误码与异常包装；玩家可见错误需对应提示文案与恢复策略。
- 日志上传/崩溃上报集成（开发环境可选，发布环境必开）；再现步骤与栈信息规范记录到 Issue。

## 提交、评审与文档
- 提交信息：`<type>(<scope>): <summary>`，附 Issue/Task 链接；禁止堆砌式提交。
- PR 模板：包含目的、变更点、风险、测试结果（附复现步骤）；无测试 = 不可合并。
- 文档：新增系统需在 Core 文档补充使用方式、扩展点、限制；协议/数据结构更新后同步 Game 团队。

## 配置管理与环境
- 机密信息使用环境变量/密钥库；禁止将密钥直接写入脚本或资源。
- 平台差异：平台相关实现用接口隔离，尽量在 Core 层提供抽象（存储、网络、输入、文件系统）。

## 3A 额外标准与发布闸门
- 可用性：关键流程（启动、登录、战斗开始、结算）需无障碍路径，支持断线重连与容错。
- 可移植性：多语言/多地区预留（文本表、时区、货币格式）。
- 可访问性：基础无障碍（色盲方案、可调 UI 字号/音量、多输入设备支持）。
- 安全：输入校验、防作弊/防篡改（关键数据校验、存档签名）、网络请求签名或加密。
- 上线清单：性能与稳定性达标、崩溃率阈值、硬件兼容清单、法务/合规（隐私、版权、GDPR/本地法规）通过后方可发布。

## 变更与例外
- 本规则为默认红线；如需例外（性能/上线紧急）必须在 PR 中写明理由、风险与补救计划，并在版本后补足。
