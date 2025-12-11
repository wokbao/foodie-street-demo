# 🍪 Foodie Street 开发日志

## 2025-12-14 (周日)

### 完成的工作
- ✅ 提供场景过渡接口 `ISceneTransition` 与默认实现 `SceneFadeTransition`（黑场淡入淡出，DontDestroyOnLoad Canvas，排序 8000）
- ✅ 新增影院式 `SceneCinematicTransition`（上下黑条闭合 + 淡入淡出），默认在 `CoreLifetimeScope` 启用；保留 `SceneFadeTransition` 备用注册（注释掉）以便随时切换
- ✅ `SceneService` 集成过渡流程：加载前淡出、加载后淡入，可与 LoadingService 并行
- ✅ 场景过渡配置化：新增 `SceneTransitionConfig`（SO），`CoreLifetimeScope` Inspector 可选引用；支持影院式/黑场/禁用三种模式，未配置时回退默认；增加 `NoSceneTransition` 作为关闭过渡的实现
- ✅ Loading HUD 全局化：`LoadingHudEntryPoint` 自动创建/复用全局 `GlobalUIRoot`（Canvas + CanvasScaler + GraphicRaycaster，DontDestroyOnLoad），`LoadingHud` 可复用外部 Canvas，避免各场景单独配置缩放

**动机**
- 以高品质过渡体验控制场景切换曝光：避免闪烁和未准备好内容露出，为后续美术过渡效果留扩展点

**下一步计划**
- [ ] 扩展过渡效果主题（快门/镜头光圈/噪点），支持可插拔策略
- [ ] 与 Loading HUD 美术稿打通：共享排序层级、首场景耗时>2s 再显示
- [ ] 增加场景切换开始/完成钩子，便于 UI/音频联动
- [ ] ScriptableObject 过渡参数配置化（暂不做，待美术/策划有需求时再开启）

---

## 2025-12-13 (周六)

### 完成的工作
- ✅ EventBus 整合：定义 `IEventBus`、实现 `EventBus`（支持优先级顺序），注册到 `CoreLifetimeScope` 并由 `CoreBootstrapper` 初始化，添加 `EventBusTest` 验证注入与发布
- ✅ 对象池初始化流程：`ObjectPoolManager` 纳入 `CoreBootstrapper` 初始化顺序，默认记录日志

**动机**
- 整合基础服务流程，确保事件传递与池化管理默认启用

**下一步计划**
- [ ] 事件过滤/错误防护与触发链核心算法
- [ ] 事件流用例：简易 UI 响应或场景通知
- [ ] 对象池自动扩容/限制策略与超时回收

---

## 2025-12-12 (周五)

### 精简子模块提交流程
- ✅ 新增脚本 `scripts/submodule_commit.sh`：一键完成子模块 add/commit/push + 主仓指针提交，减少重复命令
- ✅ 默认父仓提交信息 `chore: bump <submodule> submodule`，可在第三个参数覆盖
- ⚠️ 使用方法：`scripts/submodule_commit.sh Assets/Core "<子模块提交信息>" "<可选父仓提交信息>"`，需保证子模块改动已准备好 add/commit

**动机**
- 频繁提交子模块时需要连续执行 add/commit/push，再回到主仓更新指针，脚本化减少机械步骤与遗漏风险

**下一步计划**
- 观察脚本是否覆盖日常场景，必要时补充参数（如跳过 push、或支持多个子模块）

---

## 2025-12-11 (周四)

### 完成启动配置化与加载体验修正
- ✅ 新增 `StartupConfig` + `StartupRunner`：配置首场景 Key 与 Loading 开关，启动后自动用 `SceneService` 加载首个业务场景
- ✅ 增加 `DontDestroyOnLoadHelper`，确保 `CoreLifetimeScope` 常驻
- ✅ 修复 Loading HUD 字体（Unity 6+ 使用 `LegacyRuntime.ttf`，避免内置 Arial 异常）
- ✅ Addressables：为 StartupConfig/场景添加 Addressable 条目（Configs/Scenes 组）

**动机**
- 核心/业务解耦，可配置首场景与加载策略；避免内置字体失效导致 HUD 报错

**下一步计划**
- 视需要在引导场景补充轻量 UGUI HUD（监听 `ILoadingService`）以覆盖首场景加载
- 跟进对象池实际落地（特效/弹窗）或事件总线更丰富的用例

---

## 2025-12-10 (周三)

### 完成对象池管理器接入
- ✅ 定义并实现 `IObjectPoolManager`，支持普通对象与 GameObject 租借/归还、`WarmUp` 预热、清理
- ✅ 在 `CoreLifetimeScope` 注册对象池服务，`SceneService`/其他模块可直接注入使用
- ✅ `CoreBootstrapper` 注入对象池并预留初始化入口
- ✅ 新增 `ObjectPoolTest` 便于在场景中验证注入与基础租借/归还
- 🔧 可扩展容量上限、超时回收、组件重置回调等

**动机**
- 为频繁实例化的对象提供复用机制，降低 GC 和实例化开销

**下一步计划**
- 验证在实际场景（特效/弹窗）接入池化的效果
- 继续推进事件总线或 Loading HUD 美化

---

## 2025-12-09 (周二)

### 完成加载系统 & HUD 接入
- ✅ 定义 `ILoadingService` + `LoadingState`，支持嵌套计数、进度、描述文本
- ✅ 默认实现 `LoadingService`，提供 Begin/Dispose 作用域与进度上报，支持外部 `IProgress<float>` 透传
- ✅ `SceneService` 加载流程接入 LoadingService，统一进度回调
- ✅ Game 层挂载临时 Loading HUD（代码生成 Canvas/进度条/描述/旋转），通过 EntryPoint 常驻
- ✅ `CoreLifetimeScope` 注册 LoadingService，`GameLifetimeScope` 注册 HUD EntryPoint
- 🔧 HUD 可替换为美术版 Prefab，并接入过渡动画或可取消/错误提示

**动机**
- 为场景加载提供统一的进度状态汇聚，便于 UI 展示和后续扩展

**下一步计划**
- 高优先：对象池（`IObjectPoolManager`）或事件总线（`IEventBus`），择一先做
- Loading UI 美化：改为 Prefab + Addressables，补充可取消/错误提示

---

## 2025-12-07 (周日)

### 实现企业级配置管理系统

**背景**
- 原有的手动 Addressables 加载方式不够规范
- 每添加新配置都需要修改代码
- 需要一个可扩展的配置管理方案

**实现内容**
1. **ConfigManifest（配置清单）**
   - 声明式配置管理
   - Inspector 可视化编辑
   - 支持优先级、环境隔离
2. **ConfigLoader（配置加载器）**
   - 基于反射的类型安全加载
   - 批量加载优化
   - 失败降级策略
   - 详细加载日志
3. **ConfigRegistry（配置注册器）**
   - 自动注册到 DI 容器
   - 反射实现动态注册
4. **IValidatableConfig（验证接口）**
   - 配置自我验证协议
   - 支持多错误消息

**架构特点**
- 零代码扩展：添加新配置仅需在 Inspector 中操作
- 类型安全：反射 + 泛型保证正确性
- 分层设计：配置加载层不依赖服务层，避免循环依赖
- 环境隔离：支持开发/生产环境配置

**测试验证**
- LoggingConfig 成功加载并注册
- LogService 正确注入配置
- 完整启动流程验证通过

**目录结构**
```
Assets/Core/
├── Runtime/Configuration/    # 配置管理代码
│   ├── ConfigManifest.cs
│   ├── ConfigLoader.cs
│   ├── ConfigRegistry.cs
│   └── IValidatableConfig.cs
└── Configs/                  # 配置资产目录
    ├── CoreConfigManifest.asset
    └── LoggingConfig.asset
```

**其他改进**
- 重构 `CoreLifetimeScope` 使用新配置系统
- 添加详细代码注释说明加载顺序
- 移除废弃的 LoggingInstaller 相关代码
- 更新 Git 作者信息为 GitHub noreply 邮箱

**下一步计划**
- Loading System（加载进度条系统）
- Object Pool System（对象池系统）

---

# 开发日志

> 记录每日开发进度，方便跨设备工作时快速同步上下文

---

## 2025-11-27（公司 Windows）✅

### 完成的工作

#### 1. 代码规范重构
- ✅ 创建 `.editorconfig` 强制执行 `_camelCase` 私有字段命名
- ✅ 重构 `LogPanel.cs` 所有私有字段
- ✅ 重构 `LogService.cs` 所有私有字段
- ✅ 重构 `AddressablesAssetProvider.cs` 所有私有字段
- ✅ 修复多个 `Parameter hides field` 警告

**目的**：统一代码风格，提升可读性

#### 2. 架构文档完善
- ✅ 在 `CoreLifetimeScope` 添加详细注释
  - 列出 9 个待实现的基础设施服务（场景管理、对象池、配置等）
- ✅ 在 `GameLifetimeScope` 添加注释
  - 列出 10 个游戏域服务（玩家数据、经济、音频等）
- ✅ 在 `GameplayLifetimeScope` 添加注释
  - 列出 11 个玩法场景服务（关卡、顾客、订单、烹饪等）
- ✅ 在 `MenuLifetimeScope` 添加注释
  - 列出 11 个菜单场景服务（导航、Presenter、UI 工厂等）

**目的**：明确每层的职责和未来实现方向

#### 3. 项目规划
- ✅ 创建 `ROADMAP.md` 开发路线图
  - 规划 5 个阶段（Core/Game/Menu/Gameplay/测试优化）
  - 定义 3 个里程碑（MVP/Alpha/Beta）
  - 为每个功能添加详细清单

**目的**：追踪进度，规划开发节奏

### 技术讨论
- 讨论了 LifetimeScope 的层级关系（Core ← Game ← Scene）
- 确认通过 Unity Inspector 配置父子容器的设计合理性
- 讨论了项目管理工具选型（最终选择 ROADMAP.md + Git）

### 待处理问题
- 无

---

## 下一步计划

### 优先级排序
1. **Core 层 - 场景管理系统**（高优先级）
   - 定义 `ISceneService` 接口
   - 实现异步场景加载
   - 加载进度追踪
2. **Menu 场景 - 主菜单**（高优先级）
   - 实现 `MainMenuPresenter`
   - 创建主菜单 UI
3. **Core 层 - 对象池**（中优先级）
   - GameObject 池化
   - 提升性能

### 技术债务
- 无

---

## 2025-11-28（家里 Mac）✅

### 完成的工作

#### 1. 简化依赖注入配置
- ✅ 移除 `LoggingInstaller` 模式，改为直接在 `CoreLifetimeScope` 中注册服务
- ✅ 添加 `Core.Feature.Logging.Abstractions` using 语句
- ✅ 直接注册 `UnityLogSink` 和 `LogService`
- ✅ 创建 `CoreServicesTest.cs` 验证依赖注入是否正常工作

**目的**：简化 DI 配置，移除不必要的抽象层，使代码更易理解

### 技术讨论
- 讨论了 VContainer 的依赖注入机制
- 确认了父子容器的配置方式（需要在 Inspector 中设置 Parent 引用）
- 理解了 Installer 模式的局限性（注册后不会自动调用 Install 方法）

### 下一步计划
- [ ] 在 Unity 中验证 `CoreServicesTest`
- [ ] 检查 Scene 中的 LifetimeScope 父子容器配置
- [ ] 开始实现场景管理系统（`ISceneService`）

---

## 2025-11-28（公司 Windows）✅

### 完成的工作

#### 1. 统一依赖注入注册风格
- ✅ 将 `CoreLifetimeScope` 中的所有服务注册统一为链式风格
- ✅ 修改 `AddressablesAssetProvider` 注册方式
  - 由 `builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Singleton)`
  - 改为 `builder.Register<AddressablesAssetProvider>(Lifetime.Singleton).As<IAssetProvider>()`
- ✅ 与日志系统注册保持一致

**目的**：统一代码风格，便于未来添加额外配置（如 `.WithParameter()`）

### 技术讨论
- 讨论了两种 VContainer 注册方式的区别
- 确认统一使用链式注册风格，提升代码一致性和可扩展性

---

## 2025-11-28（家里 Mac 晚上）✅

### 完成的工作

#### 1. 实现场景管理系统 (Scene Management System)
- ✅ 定义 `ISceneService` 接口（异步加载/卸载、进度追踪）
- ✅ 实现 `SceneService` 类（Addressables + UniTask）
- ✅ 在 `CoreLifetimeScope` 中注册服务
- ✅ 创建 `SceneServiceTest.cs` 测试脚本
- ✅ 修复 `CoreServicesTest.cs` 编译错误（LogCategory.System ← Core）

#### 2. 创建核心服务初始化器 (CoreBootstrapper)
- ✅ 实现 `CoreBootstrapper` 类（使用 VContainer 的 `IStartable`）
- ✅ 在 `CoreLifetimeScope` 中注册为 EntryPoint
- ✅ 统一管理所有核心服务的初始化流程
- ✅ 添加初始化日志记录

**目的**：提供统一的场景加载接口，支持异步操作和进度追踪；统一管理核心服务的初始化

#### 3. 配置管理系统 (Config Management)
- ✅ 重构 `CoreLifetimeScope` 以支持 Addressables 配置加载
- ✅ 实现 `LoggingConfig` 的同步加载 (`WaitForCompletion`)
- ✅ 确保在服务初始化前配置已就绪
- ✅ 移除 Inspector 中的硬引用，解耦资源依赖

**目的**：实现“先加载配置，后初始化服务”的框架思路，支持动态配置加载

### 技术讨论
- 使用 Addressables 管理场景资源
- UniTask 替代 Unity AsyncOperation 实现更好的异步体验
- 单场景架构设计（Single Scene Mode）
- 启动时同步加载关键配置的必要性权衡

### 下一步计划
- [ ] 在 Unity 中测试场景加载功能
- [ ] 实现对象池系统

---

## 使用说明

### 每日记录格式
```markdown
## YYYY-MM-DD（所在地/设备）

### 完成的工作
- ✅ 具体任务描述

### 遇到的问题
- ⚠️ 问题描述及解决方案

### 下一步计划
- [ ] 待办事项
```

### 提交时别忘了
1. 更新本文档
2. 更新 ROADMAP.md 勾选完成的任务
3. 写清楚 Git commit message
