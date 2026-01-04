# 🥞 Foodie Street 开发路线图

> 按架构层级追踪任务进度。`- [ ]` 待办，`- [x]` 已完成。

---

## 📊 总体进度

- [x] 项目架构设计
- [x] LifetimeScope 层级搭建（Core 常驻 → Game 常驻 → Menu/Gameplay 子容器）
- [x] 基础日志系统
- [x] 资源管理系统（Addressables）
- [ ] Core 层基础设施完善
- [ ] Game 层业务服务实现
- [ ] 场景功能开发

---

## 🏗 Phase 1: Core 层基础设施

### 场景管理（优先级：高）
- [x] 定义 `ISceneService` 接口
- [x] 实现 `SceneService`
  - [x] 异步场景加载
  - [x] 场景卸载
  - [x] 加载进度追踪
  - [x] 场景过渡效果支持（`ISceneTransition` + 默认淡入淡出）
- [x] 在 `CoreLifetimeScope` 中注册
- [x] 场景过渡参数配置化（ScriptableObject，可供美术策划调节）
- [x] 场景过渡可插拔策略（Cinematic/Fade/Shutter/Noise/None，带安全回退）
- [x] 场景切换事件钩子（开始/完成，便于 UI/音频订阅）
- [ ] （可选）过渡配置远程/环境覆盖：加入 ConfigManifest 可选项，`IValidatableConfig` 校验，加载失败回退本地默认；保留 Inspector 直引用兜底
- [x] 编写单元测试（SceneServiceTest.cs）
- [x] 修复首次场景加载卸载错误（Additive 模式回退）

### 国际化系统 / Localization（优先级：高）
- [x] 定义 `ILocalizationService` 接口
- [x] 实现基础 `LocalizationService` (透传模式)
- [x] 在 `CoreLifetimeScope` 中注册
- [ ] 接入真实本地化数据源 (CSV/ScriptableObject)
- [ ] 文本替换逻辑 (string.Format)

### 加载系统 / Loading System（优先级：高）✅ 企业级重构完成
- [x] 定义 `ILoadingService` 接口
- [x] 实现加载进度追踪
  - [x] 进度更新接口
  - [x] 加载描述文本
  - [x] 支持嵌套加载（计数器）
  - [x] 生命周期事件（OnLoadingStarted/Completed/Error）
  - [x] 加载阶段管理（BeginPhase/EndPhase/OnPhaseChanged）
  - [x] 错误处理（ReportError）
- [x] 性能遥测系统
  - [x] ILoadingTelemetry 接口
  - [x] LoadingTelemetry 实现
  - [x] 自动记录操作和阶段耗时
- [x] 在 `GameLifetimeScope` 中实现 UI
  - [x] 进度条 UI
  - [x] 加载提示文字
  - [x] 旋转图标/动画
  - [x] 平滑淡入淡出动画
  - [x] 实现 ILoadingView 接口（UI 抽象化）
- [x] Loading HUD 配置化（LoadingHudConfig SO，GameConfigManifest 统一加载）
  - [x] 从 3 个配置项扩展到 18 个（动画、布局、样式、调试）
  - [x] 移除所有硬编码魔法数字
- [x] 移除协程，改用 UniTask（符合项目规范）
- [x] 集成于 SceneService
  - [x] 场景加载时显示进度
  - [x] 异步资源加载进度
  - [x] 细粒度加载阶段（4 个：卸载、转场淡出、加载资源、转场淡入）
- [x] 引导场景 Loading HUD（首场景加载耗时 >2s 再启用，统一全局排序层）
- [x] **已完成 P0**：启动加载优化（创建 Splash 场景，异步加载配置，解决白屏问题）
- [x] **已完成 P1**：UI 预加载优化（集成 LoadingService，显示进度）
- [ ] **待改进 P2**：关卡加载优化（批量预加载资源，细粒度进度）


### 对象池（优先级：高）
- [x] 定义 `IObjectPoolManager` 接口
- [x] 实现 GameObject 池化
- [x] 实现普通对象池化
- [ ] 自动扩容和收缩机制
- [x] 在 `CoreLifetimeScope` 中注册

### 配置管理（优先级：高）✅
- [x] 高规格配置管理体系
  - [x] ConfigManifest（声明式配置清单）
  - [x] ConfigLoader（反射驱动的加载器）
  - [x] ConfigRegistry（DI 自动注册）
  - [x] IValidatableConfig（配置验证接口）
- [x] 零代码扩展能力
- [x] 环境隔离（Dev/Production）
- [x] 失败降级策略
- [ ] 配置热更新机制（预留）
- [ ] 配置清单一致性校验脚本：比对 ConfigManifest 中的 Key/路径与 Addressables 资源地址，确保标记正确

### 事件总线（优先级：中）
- [x] 定义 `IEventBus` 接口
- [x] 事件发布/订阅
- [x] 事件优先级支持
- [ ] 事件过滤机制
- [x] 在 `CoreLifetimeScope` 中注册

### 输入管理（优先级：低）
- [ ] 定义 `IInputService` 接口
- [ ] 键盘/鼠标输入封装
- [ ] 触摸输入支持
- [ ] 输入映射系统
- [ ] 在 `CoreLifetimeScope` 中注册

### 持久化系统（优先级：低）
- [ ] 定义 `ISaveService` 接口
- [ ] 本地存储实现
- [ ] 数据序列化/反序列化
- [ ] 存档管理
- [ ] 在 `CoreLifetimeScope` 中注册

### 时间管理（优先级：低）
- [ ] 定义 `ITimeManager` 接口
- [ ] 时间缩放功能
- [ ] 计时器系统
- [ ] 在 `CoreLifetimeScope` 中注册

---

## 🎮 Phase 2: Game 层业务服务
### 音频管理（优先级：高）✅
- [x] 定义 `IAudioService` 接口
- [x] BGM 播放系统（淡入淡出支持）
- [x] 音效系统（2D/3D SFX）
- [x] 音量控制（分通道、响应式）
- [x] AudioSource 管理与对象池复用
- [x] 在 `GameLifetimeScope` 中注册
- [x] AudioKeys 常量类（消除魔法字符串）
- [x] UIButtonSound 组件（UI 音效解耦）

### 玩家数据管理（优先级：高）
- [ ] 定义 `IPlayerDataService` 接口
- [ ] 玩家等级系统
- [ ] 经验值管理
- [ ] 解锁内容追踪
- [ ] 数据持久化集成
- [ ] 在 `GameLifetimeScope` 中注册

### 经济系统（优先级：高）
- [ ] 定义 `IEconomyService` 接口
- [ ] 货币管理（金币、钻石）
- [ ] 交易系统
- [ ] 奖励发放
- [ ] 防作弊机制
- [ ] 在 `GameLifetimeScope` 中注册

### 游戏状态机（优先级：中）
- [ ] 定义 `IGameStateMachine` 接口
- [ ] 游戏状态定义（启动、菜单、游戏中、暂停）
- [ ] 状态转换逻辑
- [ ] 在 `GameLifetimeScope` 中注册

### 成就系统（优先级：中）
- [ ] 定义 `IAchievementService` 接口
- [ ] 成就解锁逻辑
- [ ] 成就进度追踪
- [ ] 成就奖励发放
- [ ] 在 `GameLifetimeScope` 中注册

### 商店系统（优先级：低）
- [ ] 定义 `IShopService` 接口
- [ ] 商品数据管理
- [ ] 购买流程
- [ ] 库存管理
- [ ] 在 `GameLifetimeScope` 中注册

### 解锁系统（优先级：低）
- [ ] 定义 `IUnlockService` 接口
- [ ] 食谱解锁
- [ ] 装备解锁
- [ ] 功能解锁
- [ ] 在 `GameLifetimeScope` 中注册

---

## 📜 Phase 3: Menu 场景功能

### 主菜单（优先级：高）
- [ ] 设计主菜单 UI
- [ ] 实现 `MainMenuPresenter`
  - [x] 主菜单 Presenter/视图框架（Play/Settings/Quit 按钮事件 + 场景加载钩子）
- [x] 开始游戏按钮（调用 SceneService，加载中禁用交互）
- [ ] 设置按钮（待接入设置/导航服务）
- [x] 退出按钮（直接退出，后续可接确认弹窗/平台逻辑）
- [ ] 在 `MenuLifetimeScope` 中注册

### 关卡选择（优先级：高）
- [ ] 设计关卡选择 UI
- [ ] 实现 `LevelSelectionPresenter`
- [ ] 关卡列表展示
- [ ] 关卡锁定/解锁状态
- [ ] 星级显示
- [ ] 在 `MenuLifetimeScope` 中注册

### 设置界面（优先级：中）
- [ ] 设计设置 UI
- [ ] 实现 `SettingsPresenter`
- [ ] 音量调节
- [ ] 画质设置
- [ ] 语言切换
- [ ] 在 `MenuLifetimeScope` 中注册
- [x] 预制化通用弹窗/面板（UGUI+TMP），统一样式/动画（遮罩、渐隐/位移）
- [x] Settings/Quit 弹窗接入 `IMenuNavigationService` / `IQuitHandler`，复用统一 UI 资产
- [x] **UI 对话框系统企业级重构（2025-12-25 完成）** ✅
  - [x] 创建 `DialogResult` 枚举（类型安全的结果判断）
  - [x] 重构 `UIConfirmDialog` 为基于结果返回的 API（`ShowAsync` 方法）
  - [x] 扩展 `IUIFactory` 接口（新增 `ShowConfirmDialogAsync` 方法）
  - [x] 修复异步完成顺序问题（`closeCompletionSource` 确保 UI 完全关闭后返回）
  - [x] 简化业务代码（`DefaultQuitHandler` 从 70 行减至 45 行）
  - [x] 添加完善的 XML 文档注释（含详细说明、示例和执行流程）
  - [ ] **未来可选优化（路线图）**：
    - [ ] 泛型对话框 API（当有 3+ 种对话框时实施）
    - [ ] 完善取消令牌支持（当需要超时/场景切换时实施）
- [ ] UI 目录规范化：`Common/` 基础件、`Screens/MainMenu/` 组合件、`Styles/` 主题/字体/色板、`Fonts/` 字体资产
- [x] UI 工厂 + Addressables 按需实例化（弹窗/设置等从 Addressables 加载，带释放/缓存策略）
- [x] 导航栈/状态管理：防重复打开，支持 Push/Pop、Overlay 弹窗
- [x] 统一进出场动画参数：淡入+缩放+遮罩渐隐，配置化（SO/常量）
- [ ] 输入/适配：键盘/手柄导航、Esc/取消关闭、Safe Area、对比度/字体可读性检查
- [ ] 设置数据流：接入配置服务（音量/语言等），提供持久化接口（可先 stub）
- [ ] Addressables 打包策略：CommonUI/Screens 暂时 Pack Together，后续按改动频次拆分优化

### 菜单导航（优先级：中）
- [ ] 定义 `IMenuNavigationService` 接口
- [ ] 页面跳转逻辑
- [ ] 返回栈管理
- [ ] 转场动画
- [ ] 在 `MenuLifetimeScope` 中注册

### 图鉴/收藏（优先级：低）
- [ ] 设计图鉴 UI
- [ ] 实现 `CollectionPresenter`
- [ ] 食谱图鉴展示
- [ ] 成就展示
- [ ] 在 `MenuLifetimeScope` 中注册

---

## 🎯 Phase 4: Gameplay 场景功能

### 关卡管理（优先级：高）
- [ ] 定义 `ILevelManager` 接口
- [ ] 关卡初始化
- [ ] 关卡流程控制
- [ ] 胜利/失败判定
- [ ] 在 `GameplayLifetimeScope` 中注册

### 关卡配置（优先级：高）
- [ ] 定义 `ILevelConfigLoader` 接口
- [ ] 创建关卡配置数据结构
- [ ] 关卡配置加载
- [ ] 难度参数解析
- [ ] 在 `GameplayLifetimeScope` 中注册

### 顾客系统（优先级：高）
- [ ] 定义 `ICustomerSpawner` 接口
- [ ] 顾客生成逻辑
- [ ] 顾客队列管理
- [ ] 顾客行为 AI
- [ ] 在 `GameplayLifetimeScope` 中注册

### 订单系统（优先级：高）
- [ ] 定义 `IOrderService` 接口
- [ ] 订单生成
- [ ] 订单状态管理
- [ ] 订单验证
- [ ] 在 `GameplayLifetimeScope` 中注册

### 烹饪系统（优先级：高）
- [ ] 定义 `ICookingService` 接口
- [ ] 食谱管理
- [ ] 烹饪流程控制
- [ ] 食材组合验证
- [ ] 在 `GameplayLifetimeScope` 中注册

### 评分系统（优先级：中）
- [ ] 定义 `IScoringService` 接口
- [ ] 订单评分计算
- [ ] 累计总分
- [ ] 星级评定
- [ ] 在 `GameplayLifetimeScope` 中注册

### 满意度系统（优先级：中）
- [ ] 定义 `ISatisfactionService` 接口
- [ ] 满意度计算
- [ ] 影响因素分析
- [ ] 奖惩机制
- [ ] 在 `GameplayLifetimeScope` 中注册

### 玩法 UI（优先级：中）
- [ ] 定义 `IGameplayUIController` 接口
- [ ] HUD 显示（分数、时间）
- [ ] 暂停菜单
- [ ] 结算界面
- [ ] 在 `GameplayLifetimeScope` 中注册
- [ ] 建立通用 UI 资产库（按钮/弹窗/列表项），Addressable 化，挂载到 UI 工厂/池

### 特效管理（优先级：低）
- [ ] 定义 `IVFXManager` 接口
- [ ] 烹饪特效
- [ ] UI 特效
- [ ] 粒子特效池化
- [ ] 在 `GameplayLifetimeScope` 中注册

---

## 🧪 Phase 5: 测试与优化

### 单元测试
- [ ] Core 层服务测试
- [ ] Game 层服务测试
- [ ] 依赖注入测试

### 集成测试
- [ ] 场景切换流程测试
- [ ] 完整游戏流程测试

### 性能优化
- [ ] 内存优化
- [ ] GC 优化
- [ ] 渲染优化
- [ ] 加载时间优化

---

## 📌 使用说明

### 更新任务状态
```markdown
- [ ] 待办任务
- [x] 已完成任务
```

### 添加备注
在任务下方使用缩进添加详细信息：
```markdown
- [ ] 实现音频系统
  - 使用 Unity AudioSource
  - 考虑使用 FMOD（可选）
  - 参考文件：AudioService.cs
```

### 追踪阻碍
使用引用块标记阻碍：
```markdown
- [ ] 实现网络功能
  > ⚠️ 等待后端 API 完成
```

---

## 🔥 当前优先级（本周目标）
1. ✅ Core 基础架构搭建
2. 🔧 场景管理系统完善（过渡策略、事件钩子）
3. 🧭 主菜单界面开发
4. 🧭 关卡选择界面开发

---

## 📅 里程碑
- **v0.1.0 - MVP** (目标：2025-XX-XX)
  - [x] 架构搭建完成
  - [ ] 基础场景切换
  - [ ] 简单的主菜单
  - [ ] 一个可玩的关卡原型

- **v0.2.0 - Alpha** (目标：2025-XX-XX)
  - [ ] 完整的菜单系统
  - [ ] 3-5 个关卡
  - [ ] 基础音效 + BGM
  - [ ] 存档系统

- **v0.3.0 - Beta** (目标：2025-XX-XX)
  - [ ] 完整的玩法系统
  - [ ] 成就系统
  - [ ] 商店系统
  - [ ] 10+ 关卡

---

**最后更新**：2026-01-04  
**当前版本**：v0.1.0-dev
