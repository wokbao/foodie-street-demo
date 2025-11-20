# PROJECT_GAME_FOOD_STREET_DESIGN.md
美食街（Food Street）Demo — 游戏策划设计文档  
Version: 0.2  
Status: Active  

## 1. 游戏概述
- 类型：轻度模拟经营 + 关卡制  
- 特点：固定玩法场景 + 数据驱动关卡（LevelConfig）

## 2. 核心循环
1. 关卡准备：选择店铺  
2. 运营：顾客来 → 选择 → 吃 or 走  
3. 结算：满意度判定星级

## 3. 场景结构（强制）
- Core_Bootstrap.unity  
- Game_Menu_Main.unity  
- Game_Street_Gameplay.unity（唯一玩法场景）  
- Game_Dev_Sandbox.unity（可选，调试）

## 4. 环境 Prefab
- Env_Street_Default.prefab  
包含：
- Tilemap  
- ShopSlot_01~03  
- PathNodes（Start/Mid/End）  
- Decorations/

## 5. 关卡配置（LevelConfig）
字段包括：
- levelId  
- durationSeconds  
- targetSatisfaction thresholds  
- environmentPrefab  
- customerWaves  
- shopSlots  

关卡加载流程：
Menu → Gameplay → LevelLoader → EnvRoot → Shop → 顾客 → 结算

## 6. 店铺系统
三种店铺：
- Shop_Ramen  
- Shop_BBQ  
- Shop_Dessert  

属性：
- FoodType  
- 服务时间：3 秒

## 7. 顾客系统
行为：
- Walk → DetectShop → Eat → Exit  

满意度：
- 吃到喜欢：+10  
- 找不到：-5  

## 8. 时间 & 结算
- 倒计时：默认 180 秒  
- 达成阈值 → 星级  
- UI：满意度 / 时间 / 结算界面  

## 9. 不做范围（Demo）
- 座位系统  
- 导航 AI  
- 复杂经济  
- 多关卡  
- 完整存档  
