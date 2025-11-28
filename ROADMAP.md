# 🍜 Foodie Street 开发路线图

> 本文档追踪项目的实施进度，按照架构层级组织任务。
> 使用 `- [ ]` 标记待办，`- [x]` 标记已完成。

---

## 📊 总体进度

- [x] 项目架构设计
- [x] LifetimeScope 层级搭建
- [x] 基础日志系统
- [x] 资源管理系统（Addressables）
- [ ] Core 层基础设施完善
- [ ] Game 层业务服务实现
- [ ] 场景功能开发

---

## 🏗️ Phase 1: Core 层基础设施

### 场景管理（优先级：高）
- [x] 定义 `ISceneService` 接口
- [x] 实现 `SceneService`
  - [x] 异步场景加载
  - [x] 场景卸载
  - [x] 加载进度追踪
  - [ ] 场景过渡效果支持（预留接口）
- [x] 在 `CoreLifetimeScope` 中注册
- [x] 编写单元测试（SceneServiceTest.cs）

### 对象池（优先级：高）
- [ ] 定义 `IObjectPoolManager` 接口
- [ ] 实现 GameObject 池化
- [ ] 实现普通对象池化
- [ ] 自动扩容和收缩机制
- [ ] 在 `CoreLifetimeScope` 中注册

### 配置管理（优先级：中）
- [ ] 定义 `IConfigManager` 接口
- [ ] 实现配置加载器
- [ ] 支持 ScriptableObject 配置
- [ ] 支持 JSON 配置
- [ ] 配置热更新机制
- [ ] 在 `CoreLifetimeScope` 中注册

### 事件总线（优先级：中）
- [ ] 定义 `IEventBus` 接口
- [ ] 实现事件发布/订阅
- [ ] 事件优先级支持
- [ ] 事件过滤机制
- [ ] 在 `CoreLifetimeScope` 中注册

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

### 音频管理（优先级：高）
- [ ] 定义 `IAudioService` 接口
- [ ] BGM 播放系统
- [ ] 音效系统
- [ ] 音量控制
- [ ] 音频池化
- [ ] 在 `GameLifetimeScope` 中注册

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

## 🖼️ Phase 3: Menu 场景功能

### 主菜单（优先级：高）
- [ ] 设计主菜单 UI
- [ ] 实现 `MainMenuPresenter`
- [ ] 开始游戏按钮
- [ ] 设置按钮
- [ ] 退出按钮
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

## 📝 使用说明

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

## 🎯 当前优先级（本周目标）

1. ✅ ~~Core 基础架构搭建~~
2. 🔄 场景管理系统实现
3. 📋 主菜单界面开发
4. 📋 关卡选择界面开发

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
  - [ ] 基础音效和 BGM
  - [ ] 存档系统

- **v0.3.0 - Beta** (目标：2025-XX-XX)
  - [ ] 完整的玩法系统
  - [ ] 成就系统
  - [ ] 商店系统
  - [ ] 10+ 关卡

---

**最后更新**：2025-11-28  
**当前版本**：v0.1.0-dev
