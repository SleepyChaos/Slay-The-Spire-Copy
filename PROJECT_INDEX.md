# Slay The Spire Copy - 项目索引

## 项目概述
这是一个基于Unity开发的卡牌游戏项目，模仿《杀戮尖塔》(Slay the Spire)的游戏机制。项目使用事件驱动架构，通过ActionSystem实现系统间的解耦通信。

## 核心架构

### 1. 事件系统 (ActionSystem)
**位置**: `Assets/Scripts/General/ActionSystem/`

- **ActionSystem.cs**: 核心事件调度系统，使用单例模式
  - 三阶段事件处理流程：PreReactions → Perform → PostReactions
  - 支持事件订阅/取消订阅机制
  - 支持事件执行器注册（Performer）
  
- **GameAction.cs**: 所有游戏事件的基类
  - 包含三个反应列表：PreReactions, PerformReactions, PostReactions
  
- **ReactionTiming.cs**: 定义事件订阅时机（PRE/POST）

### 2. 数据层 (Data)
**位置**: `Assets/Scripts/Data/`

- **CardData.cs**: 卡牌数据（ScriptableObject）
  - 包含：描述、法力消耗、图片、手动目标效果、自动目标效果列表
  
- **HeroData.cs**: 英雄数据（ScriptableObject）
  - 包含：图片、生命值、初始牌组、起始遗物
  
- **EnemyData.cs**: 敌人数据（ScriptableObject）
  - 包含：图片、生命值、攻击力
  
- **PerkData.cs**: 遗物/天赋数据（ScriptableObject）

### 3. 系统层 (System)
**位置**: `Assets/Scripts/System/`

#### 核心管理系统
- **GameManager.cs**: 游戏流程管理
  - 管理关卡进度、全局状态（血量、牌组、遗物）
  - 处理战斗结果，场景切换
  - 监听 BattleResultGA 事件

- **HeroSystem.cs**: 英雄系统
  - 管理英雄视图和状态
  - 监听敌人回合事件（PRE/POST）
  - 处理燃烧状态伤害，抽卡逻辑

- **EnemySystem.cs**: 敌人系统
  - 管理敌人列表和回合逻辑
  - 处理敌人攻击、击杀
  - 监听 EnemyTurnGA, AttackHeroGA, KillEnemyGA

- **CardSystem.cs**: 卡牌系统
  - 管理抽牌堆、弃牌堆、手牌
  - 处理抽卡、弃牌、出牌逻辑
  - 监听 DrawCardsGA, DiscardAllCardGA, PlayCardGA

- **ManaSystem.cs**: 法力系统
  - 管理法力值（最大3点）
  - 处理法力消耗和恢复
  - 监听 SpendManaGA, RefillManaGA

#### 战斗系统
- **DamageSystem.cs**: 伤害系统
  - 处理伤害计算和视觉效果
  - 监听 DealDamageGA
  - 触发死亡检查和战斗结果

- **HealSystem.cs**: 治疗系统
  - 处理治疗逻辑
  - 监听 HealGA

- **LifestealSystem.cs**: 生命偷取系统
  - 处理生命偷取效果
  - 监听 LifestealGA

- **StatusEffectSystem.cs**: 状态效果系统
  - 处理状态效果添加（护甲、燃烧等）
  - 监听 AddStatusEffectGA

- **BurnSystem.cs**: 燃烧系统
  - 处理燃烧伤害
  - 监听 ApplyBurnGA

#### 其他系统
- **EffectSystem.cs**: 效果系统
  - 执行卡牌效果
  - 监听 PerformEffectGA

- **PerkSystem.cs**: 遗物系统
  - 管理遗物的添加和移除

- **StatSystem.cs**: 属性系统
  - 处理属性增减
  - 监听 IncreaseStatsGA

- **MatchSetupSystem.cs**: 战斗设置系统
  - 初始化战斗场景

- **LevelSelectController.cs**: 关卡选择控制器

- **Interactions.cs**: 交互系统

- **ManualTargetSystem.cs**: 手动目标选择系统

- **CardViewHoverSystem.cs**: 卡牌悬停系统

### 4. 游戏事件 (GameActions)
**位置**: `Assets/Scripts/GameActions/`

所有游戏事件都继承自 `GameAction`：

- **AddStatusEffectGA**: 添加状态效果
- **ApplyBurnGA**: 应用燃烧伤害
- **AttackHeroGA**: 敌人攻击英雄
- **BattleResultGA**: 战斗结果（胜利/失败）
- **DealDamageGA**: 造成伤害
- **DiscardAllCardGA**: 丢弃所有手牌
- **DrawCardsGA**: 抽卡
- **EnemyTurnGA**: 敌人回合
- **HealGA**: 治疗
- **IncreaseStatsGA**: 增加属性
- **KillEnemyGA**: 击杀敌人
- **LifestealGA**: 生命偷取
- **PerformEffectGA**: 执行效果
- **PlayCardGA**: 打出卡牌
- **RefillManaGA**: 恢复法力
- **SpendManaGA**: 消耗法力

### 5. 效果系统 (Effect)
**位置**: `Assets/Scripts/Effect/`

所有效果都继承自 `Effect` 基类：

- **AddStatusEffectEffect**: 添加状态效果
- **DealDamageEffect**: 造成伤害
- **DrawCardEffect**: 抽卡
- **LifestealEffect**: 生命偷取

### 6. 模型层 (Models)
**位置**: `Assets/Scripts/Models/`

- **Card.cs**: 卡牌运行时模型
- **Effect.cs**: 效果基类
- **Perk.cs**: 遗物模型
- **AutoTargetEffect.cs**: 自动目标效果
- **TargetMode.cs**: 目标选择模式

### 7. 视图层 (Views)
**位置**: `Assets/Scripts/Views/`

- **HeroView.cs**: 英雄视图
- **EnemyView.cs**: 敌人视图
- **CardView.cs**: 卡牌视图
- **CombatantView.cs**: 战斗单位视图基类

### 8. UI层
**位置**: `Assets/Scripts/UI/`

包含各种UI组件，如奖励UI、法力UI、遗物UI等

### 9. 工具类 (General)
**位置**: `Assets/Scripts/General/`

- **MonoSingleton.cs**: MonoBehaviour单例基类
- **Singleton.cs**: 普通单例基类
- **ListExtensions.cs**: 列表扩展方法（如Draw随机抽取）

## 游戏流程

1. **游戏开始**: GameManager.StartNewRun() 初始化英雄数据
2. **战斗开始**: MatchSetupSystem 设置战斗场景
3. **玩家回合**: 玩家抽卡、出牌
4. **敌人回合**: EnemySystem 执行敌人行动
5. **战斗结束**: 根据结果触发 BattleResultGA
6. **奖励阶段**: 显示奖励UI，选择下一关

## 关键设计模式

1. **单例模式**: 所有系统都使用 MonoSingleton
2. **事件驱动**: 通过 ActionSystem 实现系统解耦
3. **观察者模式**: 通过 SubscribeReaction 订阅事件
4. **策略模式**: Effect 系统使用策略模式实现不同效果
5. **工厂模式**: CardViewCreator, EnemyViewCreator 创建视图

## 依赖关系

- Unity 版本: 根据项目设置确定
- PrimeTween: 用于动画效果
- SerializeReferenceEditor: 用于序列化引用编辑

## 注意事项

- 所有系统在 OnEnable/OnDisable 中注册/注销事件监听
- 事件执行使用协程，支持异步操作
- 战斗单位通过 CombatantView 统一接口
- 卡牌效果分为手动目标效果和自动目标效果

