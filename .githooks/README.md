# Git Hooks 自动化检查

本目录包含 Git Hooks，用于在提交代码时自动检查规范。

## 安装

在项目根目录运行：

```bash
# Windows (PowerShell)
git config core.hooksPath .githooks

# macOS/Linux
git config core.hooksPath .githooks
chmod +x .githooks/*
```

## Hooks 说明

### pre-commit
提交前检查，包括：
- ✅ 文件编码检查（UTF-8 without BOM）
- ✅ 禁止协程（IEnumerator）
- ✅ 禁止 async void
- ✅ 禁止 System.Threading.Tasks.Task
- ✅ 禁止空 catch 块
- ✅ 禁止直接调用 Addressables（需通过 IAssetProvider）

### commit-msg
提交信息格式检查：
- ✅ 格式：`<type>(<scope>): <description>`
- ✅ 建议使用中文描述

## 绕过检查（紧急情况）

```bash
# 仅在紧急情况下使用
git commit --no-verify -m "message"
```

## 豁免文件

以下文件类型会被排除检查：
- Test 相关文件
- Editor 相关文件
- Legacy 遗留代码
- AssetProvider 实现
- ConfigLoader（已豁免）
