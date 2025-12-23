## ⚠️ **重要提示：Splash 场景配置说明**

创建 Splash 场景后，必须进行以下配置：

### **CoreLifetimeScope 配置**

在 Splash 场景中的 `CoreLifetimeScope` GameObject 上：

1. **禁用 Auto Run**  
   在 Inspector 中，找到 `LifetimeScope` 组件  
   **取消勾选 `Auto Run`** ✅

   ```
   CoreLifetimeScope (Script)
   ├─ Parent Reference: (empty)
   ├─ Auto Run: [ ] ← 必须取消勾选！
   └─ ...
   ```

2. **原因说明**
   - CoreLifetimeScope 需要在**配置加载完成后**才能构建
   - SplashBootstrapper 会在加载完配置后手动调用 `Build()`
   - 如果 Auto Run = true，会在 Awake 时自动构建，此时配置还没加载

---

### **完整配置检查清单**

创建 Splash 场景后，确保：

- [ ] CoreLifetimeScope 的 **Auto Run = false**
- [ ] CoreLifetimeScope 的 **Core Config Manifest** 已设置
- [ ] SplashBootstrapper 的 **Core Config Manifest** 已设置
- [ ] SplashBootstrapper 的 **Game Config Manifest** 已设置
- [ ] SplashBootstrapper 的 **Next Scene Name = "Game_Menu_Main"**

---

完成后，从 Splash 场景启动游戏即可！
