# PROJECT_GAME_FOOD_STREET_IMPLEMENTATION_CHECKLIST.md
美食街 Demo — 实现顺序清单  
Version: 0.1  
Status: Active  

## 阶段 0：Core 基础
- [ ] Core_Bootstrap.unity  
- [ ] VContainer 基础注册  
- [ ] 时间系统  
- [ ] 事件系统  

## 阶段 1：关卡驱动框架
- [ ] LevelConfig  
- [ ] LevelLoader  
- [ ] Game_Menu_Main.unity  
- [ ] Game_Street_Gameplay.unity  
- [ ] 空关卡跑通流程  

## 阶段 2：环境系统
- [ ] Env_Street_Default.prefab  
- Tilemap / PathNodes / ShopSlot  
- [ ] Prefab 自动加载测试  

## 阶段 3：核心玩法
### 店铺
- [ ] ShopManager  
- [ ] ShopBase + 3 店铺  

### 顾客
- [ ] CustomerBase（Walk/Eat/Exit）  
- [ ] CustomerSpawner  

### 满意度
- [ ] SatisfactionService  
- [ ] HUD：满意度 / 时间  

## 阶段 4：完整关卡循环
- [ ] 关卡准备  
- [ ] 关卡流程  
- [ ] 结算 UI  
- [ ] 重玩 / 返回菜单  

## 阶段 5：稳定化
- [ ] 性能优化  
- [ ] UI 打磨  
- [ ] Demo 构建  
