# 🍪 Foodie Street 开发日志

## 2026-01-04 (周六)

### 完成的工作
- ✅ **音频系统 3A 标准改进**
  - 创建 `AudioKeys` 常量类，消除音频 Key 魔法字符串
  - 创建 `UIButtonSound` 组件，实现 UI 按钮音效解耦
  - 实现 AudioSource 对象池复用（通过 `IObjectPoolManager`）
  - 更新 `MainMenuPresenter` 使用新的音频常量
- ✅ **场景加载系统修复**
  - 修复首次场景加载时的 "Unloading the last loaded scene" 错误
  - `SceneService` 首次加载使用 Additive 模式，避免卸载唯一场景
  - 加载完成后手动卸载旧场景，保持后续 Single 模式正常工作
- ✅ **代码审查优化**
  - 移除未使用的 `channel` 参数
  - 修复 `AudioService` 注册方式（`RegisterEntryPoint` 确保 `IStartable` 调用）

### 架构改进
- **AudioKeys 常量管理**：所有音频 Key 集中管理，避免魔法字符串
- **UIButtonSound 组件化**：Presenter 无需关心 SFX 播放，组件自动处理
- **场景加载健壮性**：自动检测场景状态，选择正确的加载模式

### 下一步计划
- [ ] 关注 AudioMixer 集成需求
- [ ] 考虑 SFX 变体系统（随机音高/多剪辑）

---

## 2025-12-26 (周五)

### 完成的工作
- ✅ **项目规范落地**：整理并确立企业级开发规范（VContainer + UniTask + R3），产出 `project_rules.md`。
- ✅ **Bug 修复**：修复退出确认弹窗在特定操作序列下无法重新打开的问题。
- ✅ **文档更新**：同步更新开发日志与项目规则。

---

## 2025-12-25 (周三晚上，家里 Mac)

### 完成的工作
- ✅ **UI 确认对话框架构重构**（Game 层企业级改造）
  - **Game 层改动**：
    - 新增 `DialogResult` 枚举（Confirmed/Cancelled/None），替代布尔值判断
    - 重构 `UIConfirmDialog`：移除回调 API，改为基于结果返回的 `ShowAsync` 方法
    - 扩展 `IUIFactory` 接口：新增 `ShowConfirmDialogAsync` 方法
    - 实现 `UIFactory.ShowConfirmDialogAsync`：确保 UI 完全关闭后才返回结果
    - 修复异步完成顺序问题：使用 `closeCompletionSource` 等待关闭流程完成
    - 简化 `DefaultQuitHandler`：从 70 行减少到 45 行，逻辑更清晰
  - **代码质量优化**：
    - 为所有公共 API 添加完善的 XML 文档注释（含详细说明、示例和执行流程）
    - 分析代码复用可能性（结论：fire-and-forget vs 等待完成，模式差异大，不适合提取）
  - **统计数据**：
    - 新增文件：1 个（DialogResult.cs）
    - 修改文件：4 个（UIConfirmDialog.cs, IUIFactory.cs, UIFactory.cs, DefaultQuitHandler.cs）
    - 代码行数：+约 300 行（含注释）

### 架构改进
- **执行顺序保证**：业务逻辑在 UI 清理完成后执行，避免访问已销毁的对象
- **类型安全**：使用枚举代替布尔值，编译时检查，语义清晰
- **文档完善**：XML 注释包含详细说明、使用示例和执行流程
- **向前兼容**：为未来泛型对话框 API 和取消令牌支持预留路线图

### 遇到的问题
- ❌ **初次实现错误**：`ShowAsync` 返回结果和 UI 关闭并发执行，导致 `Application.Quit()` 在动画播放前调用，触发 `MissingReferenceException`
- ✅ **解决方案**：添加 `closeCompletionSource`，等待关闭流程（动画+对象池归还）完全完成后才返回结果

### 下一步计划
- [ ] 关注未来对话框需求，评估是否需要泛型化 API（当有 3+ 种对话框时）
- [ ] 如有超时/场景切换需求，实现完善的取消令牌支持

---


### 完成的工作
- ✅ **Loading 系统企业级重构**（Core 层 + Game 层大幅改进）
  - **Core 层改动（子模块）**：
    - 扩展 `ILoadingService` 接口：添加 4 个生命周期事件（OnLoadingStarted/Completed/Error/PhaseChanged）
    - 扩展 `ILoadingService` 接口：添加加载阶段管理 API（BeginPhase/EndPhase/CurrentPhase）
    - 升级 `LoadingService` 实现：实现生命周期钩子、阶段追踪、错误处理
    - 新增 `ILoadingTelemetry` 接口和 `LoadingTelemetry` 实现：性能监控和遥测系统
    - 修改 `CoreLifetimeScope`：注册 LoadingTelemetry 服务
    - 升级 `SceneService`：添加细粒度加载阶段（4 个阶段：卸载当前场景、播放转场动画、加载场景资源、场景转场完成）
  - **Game 层改动**：
    - 扩展 `LoadingHudConfig`：从 3 个配置项增至 18 个（动画、布局、样式、调试）
    - 重构 `LoadingHud`：移除协程改用 UniTask，实现平滑淡入淡出动画，所有参数配置化
    - 实现 `ILoadingView` 接口：为未来的预制体版本铺路，支持多种 UI 实现
  - **统计数据**：
    - 修改文件：5 个（Core 层 4 个，Game 层 1 个）
    - 新增文件：4 个（Core 层 2 个，Game 层 2 个）
    - 代码行数：+约 700 行
  - ✅ **启动流程优化**：
    - 实现 `ConfigLoader.LoadFromManifestAsync` 异步加载接口
    - 实现 `SplashBootstrapper` 启动脚本
    - 创建 `Game_Splash` 启动场景
    - 实现 `ConfigCache` 机制，支持 Core/Game LifetimeScope 从缓存读取配置
  - ✅ **UI 预加载优化**：
    - `UIPreloader` 集成 `ILoadingService`，显示 "预加载 UI (x/n)" 进度
    - 在 `GameLifetimeScope` 启动时自动执行，用户无感知
  - ✅ **架构规范化 (20:30)**:
    - `SceneService` 解耦：完全移除 `UnityEngine.AddressableAssets` 依赖，改用 `IAssetProvider` 加载场景
    - `IAssetProvider` 升级：集成 `LoadSceneAsync`/`UnloadSceneAsync`
    - 层级标准化：`UIHierarchyConfig` 与 `SceneTransitionConfig` 统一过渡层级为 9999

### 架构改进
- **细粒度进度追踪**：从笼统的 "加载中..." 进化为 4 个清晰的阶段描述
- **性能遥测**：自动记录每个操作和阶段的耗时，便于性能分析和优化
- **配置驱动**：所有 UI 参数外部化，移除硬编码魔法数字
- **UniTask 重构**：符合项目规范，零 GC 分配，正确的 CancellationToken 传递

### 发现的问题
- ⚠️ **启动加载黑屏**：配置文件加载是同步阻塞的（`ConfigLoader.LoadFromManifest`），导致启动时白屏 3-5 秒
- ⚠️ **UI 预加载无进度**：`UIPreloader` 预加载时没有进度反馈
- ⚠️ **关卡加载无细粒度追踪**：未来关卡预加载时需要批量加载资源

### 下一步计划（按优先级）
- [x] **P0 必须**：修复启动加载黑屏问题 ✅
  - 创建 `ConfigLoader.LoadFromManifestAsync()` 异步版本
  - 创建 Splash 启动场景 + `SplashBootstrapper`
  - 修改 LifetimeScope 使用缓存配置
- [x] **P1 建议**：UI 预加载优化（集成 LoadingService，显示进度） ✅
- [ ] **P2 可选**：关卡加载优化（创建 LevelLoader，批量预加载）
- [ ] 提交今天的代码（如果还未提交）

### 技术要点
- Begin/Dispose 控制 HUD 显示/隐藏，BeginPhase/EndPhase 更新细节描述
- 遥测系统使用 `Time.realtimeSinceStartup` 记录时间戳
- SceneService 的 4 个阶段提升了用户体验，避免 "卡住" 的感觉
- VContainer 依赖注入：LoadingService 构造函数需要 ILoadingTelemetry 参数

---

## 2025-12-22 (周日)

### 完成的工作
- ✅ **UI 对象池集成**：`UIFactory` 使用 `IObjectPoolManager` 复用 UI 实例，减少 GC
- ✅ **UI 预加载**：`UIPreloader` 在启动时预加载常用 UI（确认弹窗、设置面板等）
- ✅ **UI 动画系统**：`UIAnimator` 实现淡入淡出 + 缩放动画，集成到 `ShowDialogAsync`
- ✅ **UIKeys 验证工具**：Editor 菜单工具检查 UIKeys 与 Addressables 对应关系
- ✅ **IUICloseable 自动关闭机制**：
  - 创建 `IUICloseable` 接口
  - `UIConfirmDialog` / `UIPanelWithHeader` 实现接口
  - `UIFactory.ShowDialogAsync` 自动订阅 `CloseRequested` 事件
  - 业务层（`DefaultQuitHandler`、`MenuNavigationService`）移除 `IUIStackManager` 依赖
- ✅ **清理旧代码**：移除 `UIDialogAnimator`，更新所有引用

### 架构改进
- 弹窗组件只需实现 `IUICloseable`，框架自动处理关闭流程
- 调用方不需要知道 `IUIStackManager`，只关注业务逻辑

### 下一步计划
- [ ] 处理 lint 警告（命名空间等）
- [ ] Settings 功能填充：音量/语言等占位控件

## 2025-12-15 (周一)

### 完成的工作
- ✅ UI 基础库脚手架：新增 UI Prefab 生成器（UGUI + TMP）与基础交互脚本（按钮缩放反馈、通用确认/提示弹窗、带标题面板骨架）
- ✅ Addressables 规划：统一 Key（`UI/ButtonPrimary`、`UI/ButtonSecondary`、`UI/Panel/Base`、`UI/Panel/Header`、`UI/Dialog/Confirm`、`UI/Dialog/Info`）
- ✅ Quit 弹窗：修复隐藏状态下无法启动协程的问题，保证默认隐藏时也能正常弹出确认窗
- ✅ 菜单接线：Settings 面板可开关（导航服务），Quit 按钮接入确认弹窗（退出/取消）
- ✅ UI 工厂：统一 Addressable 加载/缓存，支持预加载、进度与诊断；主菜单屏幕通过工厂按需加载

### 下一步计划
- [ ] Settings 功能填充：音量/语言等占位控件与数据流
- [ ] 菜单 UI 细化：背景/布局/样式调整，校验 Addressable Prefab 外观
- [ ] Quit 弹窗文案/样式确认（如需）

## 2025-12-14 (周日)

### 完成的工作
- ✅ VContainer 层级梳理：`GameLifetimeScope` 常驻（Parent=Core），`MenuLifetimeScope`/`GameplayLifetimeScope` 改为子容器（Parent 指向常驻 Game），避免重复加载配置/服务
- ✅ 配置加载统一接口：新增 `IConfigManifest`，`ConfigLoader` 支持接口；Game 侧 `GameConfigManifest` 复用 Core Loader/Registry
- ✅ 菜单退出/设置预留：新增 `IMenuNavigationService`、`IQuitHandler` 默认实现，MainMenuPresenter 调用接口
- ✅ Loading HUD 配置化：新增 `LoadingHudConfig`（SO），在 GameManifest 中加载并注册；LoadingHud 支持配置延迟/排序/颜色
- ✅ 常驻 Game Scope 使用清单加载配置，子场景仅引用 Parent，避免多场景重复配置

### 遇到的问题
- Gameplay 场景缺少 LoadingHudConfig 注册导致解析失败 → 通过常驻 Game Scope + Manifest 统一加载解决
- 多个 GameLifetimeScope 并存导致重复加载 → 改为常驻单例 + 子 Scope Parent 机制

### 下一步计划
- [ ] Settings 按钮接入实际设置/导航面板
- [ ] Quit 按钮接入确认弹窗/平台退出逻辑
- [ ] LoadingHudConfig Addressable 流程完善（必要时加清单校验脚本）

---

## 2025-12-13 (周六)

### 完成的工作
- ✅ 主菜单 UI 框架落地：`MainMenuView`（Play/Settings/Quit 按钮、加载指示器、可配置 StartSceneKey/UseLoadingScreen）、`MainMenuPresenter`（事件订阅、调用 `ISceneService.LoadSceneAsync`、接入 `ILogService` 中文日志、加载中禁用交互）
- ✅ Menu 场景接入：`MenuLifetimeScope` 注册视图与 Presenter，场景层级下挂 `MainMenu Root` 即可注入
- ✅ 场景管理健壮性：`SceneService.UnloadCurrentSceneAsync` 在仅有单场景时跳过卸载，避免 “Unloading the last loaded scene” 警告

### 遇到的问题
- 加载首个业务场景时尝试卸载唯一场景触发警告（Addressables 不支持卸载最后一个场景）→ 已在 `SceneService` 跳过单场景卸载

### 下一步计划
- [ ] Settings 按钮接入导航/设置面板 Presenter
- [ ] Quit 按钮接入确认弹窗或平台退出流程
- [ ] 补充主菜单 UI 的 Loading 指示器样式（可用 `_loadingIndicator` 挂任意 GameObject）

---

## 2025-12-12 (周五)

### 完成的工作
- ✅ 新增 `SceneShutterTransition`（占位实现，复用淡入淡出），预留快门风格过渡以便后续替换动画。
- ✅ Loading HUD 细化：默认 2s 延迟显示（短加载不闪屏）、排序层级统一 8000、销毁时清理协程/显隐状态，继续复用全局 `GlobalUIRoot` Canvas。

**动机**
- 减少快速加载场景的 HUD 闪烁；为美术快门动画预留通道。

**下一步计划**
- [ ] 用真实快门动画替换占位实现，按美术方案调整时序与遮罩
- [ ] HUD 转换为可配置 Prefab/Addressables，配合 Loading 主题样式

---

## 2025-12-12 (周五)

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

## 2025-12-12 (周五)

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

## 2025-12-16（Codex 协助）✅

### 完成的工作

#### 1. 主菜单 UI 框架
- ✅ 创建 `IMainMenuView` 接口与 `MainMenuView` 组件（Play/Settings/Quit 按钮事件、加载指示器、StartSceneKey 配置）
- ✅ 创建 `MainMenuPresenter`：处理按钮事件，调用 `ISceneService.LoadSceneAsync`，加载中禁用交互；Settings/Quit 预留钩子
- ✅ 在 `MenuLifetimeScope` 注册 `MainMenuView` 与 `MainMenuPresenter`

### 遇到的问题
- 无

### 下一步计划
- [ ] 在 Unity 场景中放置 Canvas + MainMenuView，绑定 Play/Settings/Quit 按钮与 Loading 指示器
- [ ] 设置 StartSceneKey（Addressables Key 或 Build Settings 名称），验证进入游戏流程
- [ ] 接入 Settings/导航服务（未来的 MenuNavigationService/SettingsPresenter）

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
