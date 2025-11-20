# PROJECT_DEV_RULES.md
项目开发规则（Core/Game 分离 + AAA 工程流程）  
Version: 1.0  
Status: Active  
Owner: 自己  

> 本规则适用于 Core 仓库与 Game 仓库。目标：即使是小项目，也要遵守大项目的工程与质量标准。

## 1. 目标与结构
- Core：通用框架、基础系统、工具、可复用模块  
- Game：具体玩法与资源  
- 所有协议（接口、数据结构、事件、配置格式）**必须在 Core 定义**；资产实例可放 Game（如 LevelConfig SO 实例）

## 2. 技术栈
- Unity LTS  
- C#10  
- DI：VContainer  
- 响应式：R3（替代 UniRx）  
- 异步：UniTask  
- 分析器：Roslyn + StyleCop  
- 警告视为错误（Werror）

## 3. 仓库与分支
- Core 独立仓库  
- Game 引用 Core（子模块 or UPM）  
- 分支：main / feature / release / hotfix  
- Tag：vX.Y.Z

## 4. 场景与资源规范
- 代码结构固定：Domain/Feature/Abstractions/Runtime  
- 配置统一使用 SO  
- 核心资源不放正式美术  

## 5. 编码规范
- PascalCase / camelCase / _field  
- 禁止缩写、魔法数字  
- 所有公共 API 必有文档注释  

## 6. VContainer
- 每个场景一个 Composition Root  
- 禁止构造函数做异步  
- 有效使用 Singleton/Transient  

## 7. R3（Reactive）
- 所有订阅必须 Dispose  
- 事件使用 MessageBroker or IObservable  
- 避免在高频流中创建闭包  

## 8. UniTask
- 所有异步 API 必须支持取消  
- 禁止裸 Thread/Task.Run  
- Fire-and-forget 必须安全包装  

## 9. 测试与质量
- EditMode 覆盖率：≥60%（Core ≥80%）  
- PlayMode：关键功能至少 1 条  
- 静态检查必须全绿  

## 10. CI/CD
- Lint → EditMode → PlayMode → 打包  
- 构建可重复性  
- 产物包含符号、性能日志、变更日志  

## 11. 性能基线
- 目标：60FPS  
- 禁止帧内 GC  
- 主场景加载 <3s  

## 12. 发布闸门
- 崩溃率达标  
- 性能达标  
- 法务合规（隐私、版权）

## 13. 例外条款
- 必须在 PR 中说明原因  
- 版本后续必须补齐
