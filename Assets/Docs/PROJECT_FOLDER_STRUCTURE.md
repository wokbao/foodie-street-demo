# PROJECT_FOLDER_STRUCTURE.md
项目目录结构规范（Core / Game）
Version: 1.0
Status: Active
Owner: 自己

> 本文档定义 Core 与 Game 仓库的统一目录结构与命名规范。  
> 目标：零思考找到文件、零歧义命名、可扩展、适合大型项目。

# 1. 总览
两个仓库均遵循统一结构：

/ProjectRoot
  ├── Assets/
  │    ├── Core/ or Game/
  │    ├── ThirdParty/
  │    └── Plugins/
  ├── ProjectSettings/
  ├── Packages/
  └── README.md

# 2. Assets/Core 目录结构

Assets/Core
  ├── Domain/
  │    └── (业务逻辑抽象，无 Unity 依赖)
  │
  ├── Feature/
  │    └── (按系统或模块拆包，例如：Level / Shop / Customer)
  │
  ├── Abstractions/
  │    └── (接口、DTO、事件、配置结构)
  │
  ├── Runtime/
  │    ├── Systems/
  │    ├── Services/
  │    ├── Components/
  │    └── Bootstrap/
  │
  ├── ScriptableObjects/
  ├── Prefabs/
  ├── Scenes/
  │    └── Core_Bootstrap.unity
  │
  ├── Resources/
  ├── Editor/
  └── Tests/
       ├── EditMode/
       └── PlayMode/

## 命名规则（Core）
接口：IXXX  
配置 SO：XXXConfig  
事件：XXXEvent  
服务：XXXService  
系统：XXXSystem  
组件：XXXComponent  
目录：PascalCase


# 3. Assets/Game 目录结构

Assets/Game
  ├── Menu/
  │    ├── Prefabs/
  │    ├── UI/
  │    └── Scenes/
  │         └── Game_Menu_Main.unity
  │
  ├── Gameplay/
  │    ├── Prefabs/
  │    ├── UI/
  │    ├── Level/
  │    │    ├── LevelConfig/
  │    │    └── LevelLoader/
  │    ├── Environment/
  │    │    └── Env_Street_Default.prefab
  │    ├── Shops/
  │    ├── Customers/
  │    └── Scenes/
  │         └── Game_Street_Gameplay.unity
  │
  ├── Sandbox/
       └── Game_Dev_Sandbox.unity

  ├── ScriptableObjects/
  ├── Resources/
  ├── Animations/
  ├── Sprites/
  ├── Audio/
  ├── Effects/
  └── Editor/


## 命名规则（Game）
环境：Env_XXX.prefab  
店铺：Shop_XXX.prefab  
顾客：Customer_XXX.prefab  
UI：UI_XXX.prefab  
场景：Game_XXX.unity  
插槽：Slot_XXX  
节点：Node_XXX


# 4. Feature 目录结构规范（Core 与 Game 通用）

Feature/
  └── FeatureName/
        ├── Domain/
        ├── Abstractions/
        ├── Runtime/
        ├── ScriptableObjects/
        ├── Prefabs/
        ├── Editor/
        └── Tests/

示例（Shop 系统）：

Feature/Shop
  ├── Domain/
  ├── Abstractions/
  │     └── IShop.cs
  ├── Runtime/
  │     ├── ShopManager.cs
  │     ├── ShopBase.cs
  │     └── Shop_Ramen.cs
  ├── ScriptableObjects/
  │     └── ShopConfig.asset
  ├── Prefabs/
  │     └── Shop_Ramen.prefab
  └── Tests/


# 5. ScriptableObjects 规则

目录固定：

Assets/*/ScriptableObjects/<Feature>/<SOType>

示例：

ScriptableObjects/Level/LevelConfig/
ScriptableObjects/Shop/ShopConfig/
ScriptableObjects/Customer/CustomerPrototype/

说明：配置类型（接口、字段结构）在 Core 定义；配置资产实例放在 Game（如 `Assets/Game/Gameplay/Level/LevelConfig/`）。


# 6. 资源命名规则（Textures / Audio / FX）

格式必须为：

前缀_模块名_语义名  

示例：

Tex_UI_ButtonNormal.png  
Sfx_Customer_Enter.wav  
Fx_Shop_Smoke.prefab


# 7. 场景命名规则

Core_Bootstrap.unity  
Game_Menu_Main.unity  
Game_Street_Gameplay.unity  
Game_Dev_Sandbox.unity


# 8. Prefab 命名规则

<Category>_<Module>_<Name>.prefab

示例：

Env_Street_Default.prefab  
Shop_Ramen.prefab  
Customer_Normal.prefab  
UI_SatisfactionBar.prefab


# 9. 禁止事项（强制）

禁止无语义目录（New Folder、Temp、Test、Utils 等）  
禁止随意命名（shop、Shop1、Prefab_xxx）  
禁止场景混放  
禁止 Prefab 四处散落  
禁止 SO 混放  
禁止资源命名无前缀  


# 10. 附录：可选初始化

（可选）后续可生成目录初始化脚本、模板工程等。
