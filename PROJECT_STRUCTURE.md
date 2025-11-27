# 项目结构说明

## 🏗️ 仓库架构

本项目使用 **Git Submodule** 管理核心框架，分为两个独立仓库：

### 1. 主仓库：`foodie-street-demo`
- **用途**：游戏业务逻辑和场景
- **包含内容**：
  - `Assets/Game/` - 游戏业务代码（Menu、Gameplay 等）
  - `Assets/Core/` - **Git Submodule**（指向 unity-core-framework）
  - `DEV_NOTES.md` - 开发日志
  - `ROADMAP.md` - 开发路线图
  - Unity 项目配置（ProjectSettings、Packages 等）

### 2. 子模块：`unity-core-framework`
- **仓库地址**：`https://github.com/wokbao/unity-core-framework.git`
- **本地路径**：`Assets/Core/`
- **用途**：跨项目复用的核心框架
- **包含内容**：
  - 基础设施服务（日志、资源管理、场景管理等）
  - 通用工具类
  - VContainer 容器配置
  - 独立于具体游戏业务的代码

---

## 📝 提交代码流程

### 修改了 `Assets/Core/` 内的代码

```bash
# 1. 进入子模块目录
cd Assets/Core

# 2. 提交到 unity-core-framework
git add .
git commit -m "feat: 添加新功能"
git push origin main  # 或你的分支名

# 3. 返回主仓库
cd ../..

# 4. 更新主仓库的子模块引用
git add Assets/Core
git commit -m "chore: 更新 Core 子模块"
git push
```

### 修改了 `Assets/Game/` 或其他文件

```bash
# 直接在主仓库提交
git add .
git commit -m "feat: 添加游戏功能"
git push
```

---

## 🔄 子模块常用命令

### 初次克隆项目
```bash
git clone <主仓库地址>
cd foodie-street-demo

# 初始化并拉取子模块
git submodule init
git submodule update
```

### 拉取最新代码（包含子模块）
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

1. **代码复用**：Core 框架可以在多个项目中使用
2. **独立维护**：框架代码和业务代码分开管理
3. **版本控制**：主仓库可以锁定特定版本的 Core 框架
4. **团队协作**：不同团队可以独立维护不同仓库

---

## ⚠️ 注意事项

1. **修改 Core 代码后别忘了两次提交**（子模块 + 主仓库）
2. **拉取代码后记得更新子模块** `git submodule update`
3. **子模块默认处于分离头指针状态**，提交前确保在正确分支
4. **主仓库只记录子模块的 commit hash**，不包含实际代码

---

**最后更新**：2025-11-28
