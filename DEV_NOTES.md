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
- ✅ 修复了 `Parameter hides field` 警告

**目的**：统一代码风格，提升可读性

#### 2. 架构文档完善
- ✅ 为 `CoreLifetimeScope` 添加详细注释
  - 列出了 9 个待实现的基础设施服务（场景管理、对象池、配置等）
- ✅ 为 `GameLifetimeScope` 添加注释
  - 列出了 10 个游戏域服务（玩家数据、经济、音频等）
- ✅ 为 `GameplayLifetimeScope` 添加注释
  - 列出了 11 个玩法场景服务（关卡、顾客、订单、烹饪等）
- ✅ 为 `MenuLifetimeScope` 添加注释
  - 列出了 11 个菜单场景服务（导航、Presenter、UI 工厂等）

**目的**：明确每层的职责和未来实施方向

#### 3. 项目规划
- ✅ 创建 `ROADMAP.md` 开发路线图
  - 分 5 个阶段（Core/Game/Menu/Gameplay/测试优化）
  - 定义 3 个里程碑（MVP/Alpha/Beta）
  - 为每个功能添加详细清单

**目的**：追踪进度，规划开发节奏

### 技术讨论
- 讨论了 LifetimeScope 的层级关系（Core → Game → Scene）
- 确认了通过 Unity Inspector 配置父子容器的设计合理性
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
- [ ] 在 Unity 中验证 CoreServicesTest
- [ ] 检查 Scene 中的 LifetimeScope 父子容器配置
- [ ] 开始实现场景管理系统（ISceneService）

---

## 使用说明

### 每日记录格式
```markdown
## YYYY-MM-DD（地点 设备）

### 完成的工作
- ✅ 具体任务描述

### 遇到的问题
- ⚠️ 问题描述及解决方案

### 下一步计划
- [ ] 待办事项
```

### 提交时别忘了
1. 更新本文件
2. 更新 ROADMAP.md 勾选完成的任务
3. 写清楚 Git commit message
