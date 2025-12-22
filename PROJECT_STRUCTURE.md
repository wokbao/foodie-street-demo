# 项目结构说明

## 📦 仓库架构

项目使用 **Git Submodule** 管理核心框架，分为两部分：

### 1. 主仓库：`foodie-street-demo`
- **用途**：游戏业务逻辑与场景
- **包含**：
  - `Assets/Game/` - 业务代码（Menu、Gameplay 等）
  - `Assets/Core/` - **Git Submodule**（指向 `unity-core-framework`）
  - `DEV_NOTES.md` - 开发日志
  - `ROADMAP.md` - 开发路线图
  - Unity 配置（ProjectSettings、Packages 等）

### 2. 子模块：`unity-core-framework`
- **仓库地址**：`https://github.com/wokbao/unity-core-framework.git`
- **本地路径**：`Assets/Core/`
- **用途**：跨项目复用的核心框架
- **包含**：
  - 基础设施服务（日志、资源管理、场景管理等）
  - 通用工具类
  - VContainer 容器配置
  - 独立于业务的公共代码

---

## 📝 提交代码流程

### 修改 `Assets/Core/`（子模块）
> 优先使用脚本：`scripts/submodule_commit.sh <子模块路径> "<子模块提交信息>" ["<父仓库提交信息>"]`，一键完成“子模块提交 + 父仓库指针更新”。
```bash
# 1. 进入子模块
cd Assets/Core

# 2. 在子模块提交
git add .
git commit -m "feat: 添加新功能"
git push origin main  # 或你的分支

# 3. 返回主仓库
cd ../..

# 4. 更新主仓库的子模块指针
git add Assets/Core
git commit -m "chore: 更新 Core 子模块指针"
git push
```

### 修改 `Assets/Game/` 或其他文件
```bash
git add .
git commit -m "feat: 添加游戏功能"
git push
```

---

## 🛠 子模块常用命令

### 初次克隆项目
```bash
git clone <主仓库地址>
cd foodie-street-demo

# 初始化并拉取子模块
git submodule init
git submodule update
```

### 拉取最新代码（含子模块）
```bash
git pull
git submodule update --remote
```

### 查看子模块状态
```bash
git submodule status
```

---

## 🎯 为什么使用 Submodule？
1. **代码复用**：Core 框架可在多个项目中使用
2. **独立维护**：框架代码与业务代码分开管理
3. **版本控制**：主仓库可锁定特定版本的 Core 框架
4. **团队协作**：不同团队可独立维护不同仓库

---

## ⚠️ 注意事项
1. **修改 Core 代码后需两次提交**（子模块 + 主仓库）
2. **拉取代码后请更新子模块**：`git submodule update`
3. **子模块默认分离头指针**，提交前确保在正确分支
4. **主仓库仅记录子模块的 commit hash**，不包含实际代码
5. **中文文档编码**：所有 Markdown 统一使用 UTF-8；查看/编辑前优先确保终端/编辑器为 UTF-8（PowerShell 可先执行 `chcp 65001`）。一旦发现中文乱码，直接用正确 UTF-8 正文重写覆盖，切勿尝试 ISO-8859-1/GBK 等编码回转，以免二次污染。
6. **PowerShell 默认编码提醒**：Windows PowerShell 文件类命令默认非 UTF-8，建议在个人 Profile 设置：
   ```powershell
   [Console]::OutputEncoding = [System.Text.Encoding]::UTF8
   [Console]::InputEncoding  = [System.Text.Encoding]::UTF8
   chcp 65001 > $null
   $PSDefaultParameterValues['*:Encoding'] = 'utf8'
   ```
   这样无需每次加 `-Encoding UTF8`。

---

## 🏗 运行时代码架构（LifetimeScope 层级）

- **CoreLifetimeScope**：全局父容器（放在首场景且可常驻），负责加载 Core ConfigManifest、注册核心服务。
- **GameLifetimeScope**：常驻 Game 容器（Parent = CoreLifetimeScope，建议命名 `GameLifetimeScope_Root` 并 DontDestroyOnLoad）。首个场景加载 `GameConfigManifest`，通过 `ConfigLoader + ConfigRegistry` 注册 Game 配置（如 `LoadingHudConfig`），之后子场景复用。
- **MenuLifetimeScope / GameplayLifetimeScope**：场景级子容器，Parent 指向常驻的 GameLifetimeScope，不再继承 Game 类。仅注册本场景服务/组件，避免重复加载配置。

使用提示：
- 只在一个场景放置 GameLifetimeScope，并在 Inspector 引用 `GameConfigManifest`（含 `LoadingHudConfig` Addressable Key 等）。
- 各场景的 LifetimeScope（Menu/Gameplay）在 Inspector 将 Parent 设置为常驻的 GameLifetimeScope，即可访问 Core/Game 服务。
- LoadingHudConfig 等配置由 Manifest 自动加载注册，无需在子场景手动引用。

---

## 🎨 UI 风格基线（可爱 2D）
- **色板**：主色 #FF9FB3，辅色 #FFD166，强调 #7BDFF2，中性 #F7F7F7/#E6E6E6，文本主色 #333333。
- **字体**：统一 TMP 字体资源（圆润无衬线），标题加粗，正文常规；字号建议：标题 24/28，正文 16/18。
- **组件**：圆角 12px；按钮/卡片内边距 12~16px，行间距 8px；弹窗半透明遮罩（#000000, 40%），简单渐隐/缩放动画。
- **层级**：全局 Canvas（GlobalUIRoot）+ 分层：HUD / Overlay / Popup；排序规则固定，避免多 Canvas 抢层级。
- **资产**：通用按钮/弹窗/列表项 Prefab 放 `Assets/Game/UI/` 并 Addressable 化；图标保持线条圆润、粗细一致。

---

## 📰 编码规范

### 命名规范
- **私有字段**：使用 `_camelCase`（`.editorconfig` 已强制）
  ```csharp
  private ILogService _logService;
  private int _currentLevel;
  ```

- **命名空间**：从 `Assets/` 下一级开始（已禁用 ReSharper 命名空间检查）
  ```csharp
  // 文件路径: Assets/Game/UI/Runtime/UIFactory.cs
  // 命名空间: Game.UI.Runtime（而非 Game.UI.Assets.Game.UI.Runtime）
  namespace Game.UI.Runtime
  {
      public class UIFactory { }
  }
  ```

### VContainer 依赖注入规范
统一使用链式注册：
```csharp
// 推荐：链式注册
builder.Register<ConcreteClass>(Lifetime.Singleton)
    .As<IInterface>();

// 避免：单行注册（除非有特殊原因）
builder.Register<IInterface, ConcreteClass>(Lifetime.Singleton);
```
优点：
1. 风格统一，便于维护
2. 易于追加配置（`.WithParameter()`、`.AsSelf()` 等）
3. 清晰区分具体实现类与接口

示例：
```csharp
// 基础注册
builder.Register<LogService>(Lifetime.Singleton)
    .As<ILogService>();

// 带参数配置
builder.Register<GameConfig>(Lifetime.Singleton)
    .WithParameter("configPath", "Configs/game.json")
    .As<IGameConfig>();

// 多接口映射
builder.Register<AudioService>(Lifetime.Singleton)
    .As<IAudioService>()
    .As<IMusicPlayer>();
```

### Git 提交规范
**所有 Git 提交信息必须使用中文**：
```bash
# 正确示例
git commit -m "feat(core): 实现场景管理系统和核心服务启动器"
git commit -m "fix(logging): 修复日志配置加载失败的问题"
git commit -m "docs: 更新开发日志和路线图"

# 错误示例
git commit -m "feat(core): implement scene management system"
git commit -m "fix: fixed bug"
```
格式：`<type>(<scope>): <description>`  
常用 type：`feat`、`fix`、`docs`、`refactor`、`test`  
`scope` 可选，用于描述影响的模块；`description` 必须用中文简洁说明。
- 提交必须包含标题（subject）和正文（body）；正文用列表概述主要改动，示例：
```bash
git commit -m "feat(core): 增强调度" \
  -m "- 支持场景过渡动画" \
  -m "- 增加进度回调"
```

### 日志和异常信息规范
**所有日志消息和异常信息必须使用中文**：
```csharp
// 正确示例
_logService.Information(LogCategory.Core, "开始加载场景");
Debug.LogWarning("Addressables 加载配置失败");
throw new Exception("场景加载失败");

// 错误示例
_logService.Information(LogCategory.Core, "Loading scene...");
Debug.LogWarning("Failed to load config");
throw new Exception("Scene load failed");
```
- 包括所有 `ILogService` 的日志调用
- 包括所有 `Debug.Log/LogWarning/LogError` 调用
- 包括所有异常消息 `throw new Exception(...)`
- 注释可以用英文，但面向用户的信息必须中文

### 文档同步规范
**每次代码修改后，必须同步更新以下文档**：
1. **DEV_NOTES.md** - 记录当日工作内容和技术决策
2. **ROADMAP.md** - 勾选完成的功能任务（如适用）
3. **PROJECT_STRUCTURE.md** - 更新编码规范或架构说明（如适用）
- **新开对话时的文档更新规则**：先打开最新仓库版本（避免覆盖本地未合并的内容），按照原有格式追加/勾选，不重写历史记录；如遇冲突，优先保留仓库版本再追加本次更新。
- **文档修改纪律**：
  - 严禁整篇替换/重写，仅在原有基础上增改；改前先 `git status`/`git diff` 确认范围。
  - 全程使用 UTF-8 读写；如发现乱码，直接用正确 UTF-8 正文覆盖，不做编码互转。
  - 修改后自查 `git diff`，确认仅改预期段落和格式。

---

**最后更新**：2025-12-14
