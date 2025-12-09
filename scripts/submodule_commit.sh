#!/usr/bin/env bash
set -euo pipefail

# 子模块提交助手脚本 - 自动化提交子模块并更新父仓库引用
# 功能：一键完成子模块的提交和父仓库引用的更新
# 作者：项目维护团队
# 创建时间：2024年

# 使用方法：
#   scripts/submodule_commit.sh <子模块路径> <子模块提交信息> [父仓库提交信息]
# 示例：
#   scripts/submodule_commit.sh Assets/Core/Feature/ObjectPooling "feat: 添加新功能"
#   scripts/submodule_commit.sh Assets/Core/Feature/ObjectPooling "feat: 添加新功能" "chore: 更新子模块引用"

# 参数验证：检查是否提供了足够的参数
if [[ $# -lt 2 ]]; then
  echo "错误：参数不足！" >&2
  echo "使用方法：$0 <子模块路径> <子模块提交信息> [父仓库提交信息]" >&2
  echo "示例：$0 Assets/Core/Feature/ObjectPooling \"feat: 添加新功能\"" >&2
  exit 1
fi

# 参数解析
SUBMODULE_PATH="$1"           # 子模块路径
SUBMODULE_MSG="$2"            # 子模块提交信息
PARENT_MSG="${3:-chore: bump $(basename "$SUBMODULE_PATH") submodule}"  # 父仓库提交信息（可选，有默认值）

# 子模块有效性检查：确保指定的路径是有效的子模块
if [[ ! -d "$SUBMODULE_PATH/.git" ]]; then
  echo "错误：$SUBMODULE_PATH 不是一个有效的子模块路径（缺少 .git 目录）。" >&2
  exit 1
fi

# 第一步：提交子模块
echo "=== 开始提交子模块：$SUBMODULE_PATH ==="

# 显示子模块状态（简洁格式）
echo "子模块状态："
git -C "$SUBMODULE_PATH" status -sb

# 添加所有更改到子模块暂存区
echo "添加所有更改到子模块暂存区..."
git -C "$SUBMODULE_PATH" add -A

# 提交子模块更改
echo "提交子模块更改..."
git -C "$SUBMODULE_PATH" commit -m "$SUBMODULE_MSG"

# 推送子模块到远程仓库
echo "推送子模块到远程仓库..."
git -C "$SUBMODULE_PATH" push

echo "✓ 子模块提交完成"

# 第二步：更新父仓库引用
echo "=== 更新父仓库引用 ==="

# 将子模块更改添加到父仓库暂存区
echo "更新父仓库对子模块的引用..."
git add "$SUBMODULE_PATH"

# 检查是否有实际更改需要提交
if git diff --cached --quiet; then
  echo "ℹ️  父仓库没有需要提交的更改。"
else
  # 提交父仓库的更改
  echo "提交父仓库的更改..."
  git commit -m "$PARENT_MSG"
  
  # 推送父仓库到远程
  echo "推送父仓库到远程..."
  git push
  
  echo "✓ 父仓库更新完成"
fi

echo "✅ 子模块提交流程全部完成！"
echo "子模块：$SUBMODULE_PATH"
echo "子模块提交信息：$SUBMODULE_MSG"
echo "父仓库提交信息：$PARENT_MSG"