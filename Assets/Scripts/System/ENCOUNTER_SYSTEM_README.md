# 遭遇系统使用说明

## 概述

新的遭遇系统将战斗流程模块化，支持：
- 数据驱动的遭遇配置
- 可复用的战斗流程
- 关卡选择界面（复用奖励卡UI）
- 9次闯关后进入Boss关

## 系统架构

### 1. 数据层
- **EncounterData**: ScriptableObject，定义一场战斗的配置
  - 遭遇类型（Normal/Elite/Boss）
  - 敌人池和选择模式
  - 关卡限制和权重

### 2. 管理层
- **EncounterManager**: 管理遭遇池，根据关卡选择遭遇
- **BattleFlowManager**: 统一管理战斗流程（开始战斗、战斗结束等）

### 3. UI层
- **LevelSelectUI**: 关卡选择界面，显示3个关卡选项
- **LevelNodeAdapter**: 单个关卡节点的UI适配器

## 使用步骤

### 步骤1：创建遭遇数据（EncounterData）

1. 在 Project 窗口右键 → `Create → Data → Encounter`
2. 配置遭遇：
   - **Encounter Type**: Normal/Elite/Boss
   - **Encounter Name**: 遭遇名称（用于显示）
   - **Enemy Pool**: 该遭遇可能出现的敌人列表
   - **Enemy Count**: 同时出现的敌人数量
   - **Selection Mode**: 敌人选择模式（Random/Unique/Sequential）
   - **Min/Max Stage Index**: 最早/最晚出现的关卡
   - **Weight**: 随机权重

### 步骤2：配置 EncounterManager

1. 在场景中创建空对象，添加 `EncounterManager` 组件
2. 在 Inspector 中配置：
   - **Normal Encounters**: 拖入所有普通遭遇
   - **Elite Encounters**: 拖入所有精英遭遇
   - **Boss Encounters**: 拖入所有Boss遭遇

### 步骤3：配置 BattleFlowManager

1. 在场景中创建空对象，添加 `BattleFlowManager` 组件
2. 配置场景名称：
   - **Battle Scene Name**: "BattleScene"
   - **Level Select Scene Name**: "LevelSelectScene"

### 步骤4：创建关卡选择UI

1. 创建关卡选择界面（类似 RewardPanel）：
   - 创建 Canvas（如果还没有）
   - 创建 Panel 作为容器
   - 添加 `LevelSelectUI` 组件

2. 创建关卡节点预制体：
   - 创建 UI 元素（Image、Text等）
   - 添加 `LevelNodeAdapter` 组件
   - 配置引用：
     - **Click Button**: 点击按钮
     - **Icon Image**: 图标（可选）
     - **Title Text**: 标题文本
     - **Description Text**: 描述文本
     - **Elite Badge**: 精英标记（可选）
     - **Boss Badge**: Boss标记（可选）

3. 在 `LevelSelectUI` 中配置：
   - **Content**: Panel 对象
   - **Node Container**: 节点排列的父节点
   - **Node Prefab**: 关卡节点预制体

### 步骤5：配置 LevelSelectController

1. 在关卡选择场景中，找到或创建 `LevelSelectController`
2. 配置：
   - **Level Select UI**: 拖入 `LevelSelectUI` 组件

## 游戏流程

### 完整流程

1. **游戏开始** → `GameManager.StartNewRun()` → 加载 `LevelSelectScene`

2. **关卡选择** → `LevelSelectController` 生成3个随机遭遇选项 → `LevelSelectUI` 显示

3. **玩家选择** → `LevelSelectUI.OnLevelSelected()` → `BattleFlowManager.StartBattle()` → 加载 `BattleScene`

4. **战斗** → `MatchSetupSystem` 初始化战斗 → 玩家战斗

5. **战斗胜利** → `GameManager.OnBattleResult()` → `RewardUI.Show()` → 显示奖励卡

6. **选择奖励** → `RewardUI.PickCard()` → `RewardUI.GoToNextLevel()` → `GameManager.AdvanceStage()` → 返回 `LevelSelectScene`

7. **重复步骤2-6**，直到完成9次闯关

8. **Boss关** → 第10关（索引9）自动显示Boss选项 → 战斗Boss

## 关卡进度

- **CurrentStageIndex**: 当前关卡索引（0开始）
- **BOSS_STAGE_INDEX**: 9（完成9次闯关后，第10关是Boss）
- 每次战斗胜利后，`GameManager.AdvanceStage()` 会增加关卡索引

## 注意事项

1. **EncounterManager** 需要在场景中存在，建议放在 DontDestroyOnLoad 对象上
2. **BattleFlowManager** 同样建议放在 DontDestroyOnLoad 对象上
3. 确保所有遭遇数据都正确配置了敌人池
4. 关卡选择UI需要正确配置 Canvas 和渲染模式
5. 如果某个关卡没有可用的遭遇，系统会输出警告

## 扩展

未来可以扩展：
- 事件节点（Event）
- 休息节点（Rest）
- 商店节点（Shop）
- 更复杂的遭遇选择逻辑
