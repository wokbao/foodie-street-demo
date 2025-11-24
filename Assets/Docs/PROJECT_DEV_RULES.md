# PROJECT_DEV_RULES.md
项目开发规则（Core/Game 分离 + AAA 工程流程）
Version: 1.0  
Status: Active  
Owner: 团队  

> 本规则适用于 Core 仓库与 Game 仓库。目标：即使是小项目，也要遵守大项目的工程与质量标准。

## 1. 目标与结构
- Core：提供通用框架、基础系统、工具、可复用模块  
- Game：实现具体玩法与资源  
- 所有协议（接口、数据结构、事件、配置格式）必须在 Core 定义；资产实例可放在 Game（如 LevelConfig ScriptableObject）  

## 2. 技术栈
- Unity LTS  
- C# 10  
- DI：VContainer  
- 响应式：R3（替代 UniRx）  
- 异步：UniTask  
- 分析器：Roslyn + StyleCop  
- 警告视为错误（Werror）  

## 3. 仓库与分支
- Core 独立仓库  
- Game 引用 Core（Git 子模块或 UPM）  
- 分支：main / feature / release / hotfix  
- Tag：vX.Y.Z  

## 4. 场景与资源规范
- 代码结构固定：Domain / Feature / Abstractions / Runtime  
- 配置统一使用 ScriptableObject  
- 核心资源不放临时或最终美术  

## 5. 编码规范
- 命名：PascalCase / camelCase / _field  
- 命名空间从 Assets 下一级开始，不包含 Assets 本身，例如 Assets/Game/Gameplay/Runtime → Game.Gameplay.Runtime，Assets/Core/Runtime → Core.Runtime  
- 禁止魔法数字与随意缩写  
- 所有公共 API 必须包含文档注释  

## 6. VContainer
- 每个场景必须有一个 Composition Root  
- 禁止在构造函数中执行异步逻辑  
- 正确选择 Singleton / Transient 生命周期  

## 7. R3（Reactive）
- 所有订阅必须 Dispose  
- 事件使用 MessageBroker 或 IObservable  
- 避免在高频流中创建闭包  

## 8. UniTask
- 所有异步 API 必须支持取消  
- 禁止直接 new Thread / Task.Run  
- Fire-and-forget 必须安全包装  
- 禁止引用 System.Threading.Tasks 作为正式异步返回值，统一返回 UniTask / UniTask<T>（第三方 SDK 适配时需在文档中说明例外）  

## 9. 测试与质量
- EditMode 覆盖率：≥ 60%（Core ≥ 80%）  
- PlayMode：关键功能至少 1 套  
- 静态检查必须全部通过  

## 10. CI/CD
- 包含 Lint、EditMode、PlayMode、打包  
- 构建可重复  
- 产物包含符号、性能日志、ChangeLog  

## 11. 性能基线
- 目标 60 FPS  
- 禁止帧内 GC  
- 主场景加载 < 3s  

## 12. 发布闸门
- 崩溃率达标  
- 性能达标  
- 法务合规（隐私、版权）  

## 13. 例外条款
- 必须在 PR 中说明原因  
- 版本后续必须补齐  

## 14. Addressables 资源管理
- Addressables 访问统一通过 IAssetProvider，业务代码禁止直接引用 Addressables API  
- 句柄需缓存且复用前必须 Await/WaitForCompletion，失败或取消要移出缓存并释放  
- 加载方法必须接收 CancellationToken，取消时释放句柄，禁止遗留悬挂请求  
- Addressables.Instantiate 创建的实例必须登记并在生命周期结束后调用 ReleaseInstance  
- 资源管理相关异步接口统一使用 UniTask，与整体技术栈保持一致  
