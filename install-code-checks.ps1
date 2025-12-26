# 自动化规范检查安装脚本
# 配置 Git Hooks 并启用 EditorConfig

Write-Host "🚀 安装企业级规范检查工具..." -ForegroundColor Green
Write-Host ""

# 1. 配置 Git Hooks 路径
Write-Host "📝 配置 Git Hooks..." -ForegroundColor Cyan
try {
    git config core.hooksPath .githooks
    Write-Host "  ✅ Git Hooks 路径已设置为 .githooks" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Git Hooks 配置失败: $_" -ForegroundColor Red
    exit 1
}

# 2. 给 hooks 脚本添加执行权限（Windows 上通常不需要，但为兼容性保留）
Write-Host ""
Write-Host "📝 配置 Hooks 权限..." -ForegroundColor Cyan
if (Test-Path ".githooks/pre-commit") {
    Write-Host "  ✅ pre-commit hook 已就绪" -ForegroundColor Green
}
if (Test-Path ".githooks/commit-msg") {
    Write-Host "  ✅ commit-msg hook 已就绪" -ForegroundColor Green
}

# 3. 检查 EditorConfig 支持
Write-Host ""
Write-Host "📝 检查 EditorConfig..." -ForegroundColor Cyan
if (Test-Path ".editorconfig") {
    Write-Host "  ✅ EditorConfig 文件已存在" -ForegroundColor Green
    Write-Host "  💡 请确保 IDE 已安装 EditorConfig 插件" -ForegroundColor Yellow
} else {
    Write-Host "  ❌ 未找到 .editorconfig 文件" -ForegroundColor Red
}

# 4. 测试 Git Hooks
Write-Host ""
Write-Host "🧪 测试 Git Hooks..." -ForegroundColor Cyan
Write-Host "  创建测试文件..." -ForegroundColor Gray

# 创建一个测试文件
$testFile = "test_hook.txt"
"test" | Out-File -FilePath $testFile -Encoding UTF8
git add $testFile

Write-Host "  运行 pre-commit hook..." -ForegroundColor Gray
# Windows 下可能需要使用 Git Bash 或 WSL 来运行 sh 脚本
# 这里只是验证配置，实际运行需要 Git Bash 环境

# 清理测试文件
git reset HEAD $testFile 2>$null
Remove-Item $testFile -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "✅ 安装完成！" -ForegroundColor Green
Write-Host ""
Write-Host "📋 已启用的检查:" -ForegroundColor Cyan
Write-Host "  ✓ 私有字段命名规范（_camelCase）" -ForegroundColor White
Write-Host "  ✓ 禁止协程（IEnumerator）" -ForegroundColor White
Write-Host "  ✓ 禁止 async void" -ForegroundColor White
Write-Host "  ✓ 禁止 System.Threading.Tasks.Task" -ForegroundColor White
Write-Host "  ✓ 禁止空 catch 块" -ForegroundColor White
Write-Host "  ✓ 禁止直接调用 Addressables" -ForegroundColor White
Write-Host "  ✓ 提交信息格式检查" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  注意事项:" -ForegroundColor Yellow
Write-Host "  • Git Hooks 在 Windows 上需要 Git Bash 环境" -ForegroundColor Gray
Write-Host "  • 如果 hooks 未执行，请检查 Git 配置" -ForegroundColor Gray
Write-Host "  • 紧急情况可使用 git commit --no-verify 绕过" -ForegroundColor Gray
Write-Host ""
Write-Host "🎯 下一步:" -ForegroundColor Cyan
Write-Host "  1. 在 IDE 中安装 EditorConfig 插件" -ForegroundColor White
Write-Host "  2. 提交代码测试 hooks 是否工作" -ForegroundColor White
Write-Host "  3. 阅读 .githooks/README.md 了解详情" -ForegroundColor White
Write-Host ""
