# Unity 编辑器配置详细指南

## 📋 准备工作

在开始之前，请确保：
- Unity 编辑器已打开项目
- 你已经有一些 `EnemyData` 资源（如果没有，需要先创建）
- 了解基本的 Unity UI 操作

---

## 🎯 第一步：创建遭遇数据（EncounterData）

### 1.1 创建普通遭遇

1. **在 Project 窗口**：
   - 找到 `Assets/Scripts/Data/` 文件夹（如果没有，创建一个）
   - 右键点击该文件夹
   - 选择 `Create → Data → Encounter`
   - 命名为 `Encounter_Normal_01`（或其他名称）

2. **配置遭遇数据**（在 Inspector 中）：
   ```
   Encounter Type: Normal
   Encounter Name: "普通战斗"
   Description: "一场普通的战斗"（可选）
   
   Enemy Pool: 
     - 点击 "+" 添加敌人
     - 从 Project 窗口拖入你的 EnemyData 资源
     - 可以添加多个敌人（系统会随机选择）
   
   Enemy Count: 1（同时出现的敌人数量）
   Selection Mode: Random（随机选择）
   
   Min Stage Index: 0（从第1关开始可以出现）
   Max Stage Index: -1（-1表示无限制）
   Weight: 10（随机权重，数字越大越容易被选中）
   ```

3. **重复创建多个普通遭遇**：
   - `Encounter_Normal_02`
   - `Encounter_Normal_03`
   - 等等...

### 1.2 创建精英遭遇

1. **创建精英遭遇**（步骤同上）：
   - 命名为 `Encounter_Elite_01`
   - **Encounter Type**: 选择 `Elite`
   - **Encounter Name**: "精英战斗"
   - 配置更强的敌人或更多敌人
   - **Weight**: 可以设置小一点（比如 5），让精英出现频率低一些

### 1.3 创建 Boss 遭遇

1. **创建 Boss 遭遇**：
   - 命名为 `Encounter_Boss_01`
   - **Encounter Type**: 选择 `Boss`
   - **Encounter Name**: "Boss 战斗"
   - 配置 Boss 敌人
   - **Enemy Count**: 通常为 1（单个 Boss）

---

## 🎮 第二步：配置场景管理器

### 2.1 创建或找到关卡选择场景

1. **创建关卡选择场景**（如果还没有）：
   - `File → New Scene`
   - 选择 `Basic (Built-in)` 或 `2D`
   - 保存为 `Assets/Scenes/LevelSelectScene.unity`

2. **或者使用现有场景**：
   - 打开你的关卡选择场景

### 2.2 配置 EncounterManager

1. **创建管理器对象**：
   - 在 Hierarchy 窗口右键
   - `Create Empty`
   - 命名为 `EncounterManager`

2. **添加组件**：
   - 选中 `EncounterManager` 对象
   - 在 Inspector 点击 `Add Component`
   - 搜索 `Encounter Manager` 并添加

3. **配置遭遇池**：
   ```
   Normal Encounters:
     - Size: 设置为你创建的普通遭遇数量（比如 3）
     - 依次拖入你创建的所有普通遭遇
   
   Elite Encounters:
     - Size: 设置为你创建的精英遭遇数量（比如 2）
     - 依次拖入你创建的所有精英遭遇
   
   Boss Encounters:
     - Size: 设置为你创建的Boss遭遇数量（比如 1）
     - 拖入你创建的Boss遭遇
   ```

4. **设置为 DontDestroyOnLoad**（可选但推荐）：
   - 在 Inspector 中找到 `EncounterManager` 脚本
   - 确保该对象不会被场景切换销毁
   - 或者将其放在一个不会被销毁的父对象下

### 2.3 配置 BattleFlowManager

1. **创建管理器对象**：
   - 在 Hierarchy 窗口右键
   - `Create Empty`
   - 命名为 `BattleFlowManager`

2. **添加组件**：
   - 选中 `BattleFlowManager` 对象
   - 在 Inspector 点击 `Add Component`
   - 搜索 `Battle Flow Manager` 并添加

3. **配置场景名称**：
   ```
   Battle Scene Name: "BattleScene"（你的战斗场景名称）
   Level Select Scene Name: "LevelSelectScene"（关卡选择场景名称）
   ```

---

## 🎨 第三步：创建关卡选择 UI

### 3.1 创建 UI Canvas

1. **创建 Canvas**（如果场景中没有）：
   - Hierarchy 右键 → `UI → Canvas`
   - 命名为 `LevelSelectCanvas`

2. **配置 Canvas**：
   - **Render Mode**: `Screen Space - Camera`（推荐）
   - **Render Camera**: 拖入场景中的 Main Camera
   - **Sort Order**: 100（确保显示在最上层）

### 3.2 创建关卡选择 Panel

1. **创建 Panel**：
   - 选中 `LevelSelectCanvas`
   - 右键 → `UI → Panel`
   - 命名为 `LevelSelectPanel`

2. **配置 Panel**：
   - **Rect Transform**: 
     - Anchor: 设置为 `Stretch Stretch`（全屏）
     - 或者设置为合适的大小和位置
   - **Image**:
     - Color: 可以设置为半透明黑色（用于遮挡背景）
     - Alpha: 0.5-0.8（根据你的需求）

3. **添加 LevelSelectUI 组件**：
   - 选中 `LevelSelectPanel`
   - 在 Inspector 点击 `Add Component`
   - 搜索 `Level Select UI` 并添加

### 3.3 创建节点容器

1. **创建容器**：
   - 选中 `LevelSelectPanel`
   - 右键 → `UI → Empty`（或 `GameObject → Create Empty`）
   - 命名为 `NodeContainer`

2. **配置容器**：
   - **Rect Transform**:
     - Anchor: `Center`
     - Width: 1200（或根据你的需求）
     - Height: 400
     - Position: (0, 0, 0)
   - 添加 `Horizontal Layout Group` 组件（用于水平排列）：
     - Spacing: 50
     - Child Alignment: `Middle Center`
     - Child Force Expand: 取消勾选 Width 和 Height

3. **在 LevelSelectUI 中配置**：
   ```
   Content: 拖入 LevelSelectPanel
   Node Container: 拖入 NodeContainer
   Node Prefab: （稍后创建）
   Skip Button: （可选，可以留空）
   ```

### 3.4 创建关卡节点预制体

1. **创建节点对象**：
   - 在 Hierarchy 中（任意位置）
   - 右键 → `UI → Button`
   - 命名为 `LevelNode`

2. **配置节点外观**：
   ```
   Rect Transform:
     - Width: 300
     - Height: 400
     - Position: (0, 0, 0)
   
   Image (Button 组件自带):
     - Color: 白色或你喜欢的颜色
     - Source Image: 可以设置一个背景图片（可选）
   ```

3. **添加子元素**：

   **a. 标题文本**：
   - 选中 `LevelNode`
   - 右键 → `UI → Text - TextMeshPro`（如果没有，用普通 Text）
   - 命名为 `TitleText`
   - 配置：
     ```
     Rect Transform:
       - Anchor: Top Center
       - Pos Y: -30
       - Width: 280
       - Height: 50
     Text:
       - Text: "关卡标题"
       - Font Size: 36
       - Alignment: Center
       - Color: 黑色或白色
     ```

   **b. 描述文本**：
   - 选中 `LevelNode`
   - 右键 → `UI → Text - TextMeshPro`
   - 命名为 `DescriptionText`
   - 配置：
     ```
     Rect Transform:
       - Anchor: Middle Center
       - Pos Y: -50
       - Width: 280
       - Height: 100
     Text:
       - Text: "关卡描述"
       - Font Size: 24
       - Alignment: Center
       - Color: 灰色
     ```

   **c. 图标（可选）**：
   - 选中 `LevelNode`
   - 右键 → `UI → Image`
   - 命名为 `IconImage`
   - 配置：
     ```
     Rect Transform:
       - Anchor: Top Center
       - Pos Y: -100
       - Width: 150
       - Height: 150
     Image:
       - Color: 白色
       - Source Image: （留空，代码会自动设置）
     ```

   **d. 精英标记（可选）**：
   - 选中 `LevelNode`
   - 右键 → `UI → Image` 或 `UI → Text - TextMeshPro`
   - 命名为 `EliteBadge`
   - 配置：
     ```
     Rect Transform:
       - Anchor: Top Right
       - Pos X: -10
       - Pos Y: -10
     Text/Image:
       - 显示 "ELITE" 或红色标记
     ```
   - 默认设置为 `Inactive`（不激活）

   **e. Boss 标记（可选）**：
   - 同上，命名为 `BossBadge`
   - 显示 "BOSS" 或特殊标记

4. **添加 LevelNodeAdapter 组件**：
   - 选中 `LevelNode`
   - 在 Inspector 点击 `Add Component`
   - 搜索 `Level Node Adapter` 并添加

5. **配置 LevelNodeAdapter**：
   ```
   Click Button: 拖入 LevelNode（Button 组件）
   Icon Image: 拖入 IconImage（如果有）
   Title Text: 拖入 TitleText
   Description Text: 拖入 DescriptionText
   Elite Badge: 拖入 EliteBadge（如果有）
   Boss Badge: 拖入 BossBadge（如果有）
   ```

6. **保存为预制体**：
   - 从 Hierarchy 拖拽 `LevelNode` 到 `Assets/Prefabs/` 文件夹
   - 命名为 `LevelNodePrefab`
   - 删除 Hierarchy 中的 `LevelNode`（因为已经保存为预制体）

7. **在 LevelSelectUI 中配置**：
   - 选中 `LevelSelectPanel`
   - 在 Inspector 的 `Level Select UI` 组件中
   - **Node Prefab**: 拖入刚才创建的 `LevelNodePrefab`

---

## 🔗 第四步：配置 LevelSelectController

1. **创建或找到控制器对象**：
   - 在 Hierarchy 中创建空对象
   - 命名为 `LevelSelectController`

2. **添加组件**：
   - 选中 `LevelSelectController`
   - 在 Inspector 点击 `Add Component`
   - 搜索 `Level Select Controller` 并添加

3. **配置引用**：
   ```
   Level Select UI: 拖入 LevelSelectPanel（有 LevelSelectUI 组件的对象）
   ```

---

## ⚔️ 第五步：配置战斗场景

### 5.1 确保战斗场景存在

1. **创建或打开战斗场景**：
   - 如果还没有，创建 `Assets/Scenes/BattleScene.unity`
   - 或者使用你现有的战斗场景

### 5.2 配置 MatchSetupSystem

1. **找到或创建 MatchSetupSystem**：
   - 在战斗场景中找到 `MatchSetupSystem` 对象
   - 如果没有，创建一个并添加 `Match Setup System` 组件

2. **配置 MatchSetupSystem**：
   - **Hero Data**: 拖入你的英雄数据
   - **Enemy Datas**: （可以留空，系统会从 GameManager 读取）
   - 其他配置保持不变

---

## 🎮 第六步：测试流程

### 6.1 检查场景设置

1. **确保场景在 Build Settings 中**：
   - `File → Build Settings`
   - 确保 `LevelSelectScene` 和 `BattleScene` 都在场景列表中
   - 如果没有，点击 `Add Open Scenes` 添加

### 6.2 测试关卡选择

1. **运行游戏**：
   - 点击 Play 按钮
   - 或者通过代码调用 `GameManager.StartNewRun()`

2. **检查 Console**：
   - 查看是否有错误信息
   - 应该看到 "生成关卡选项" 的日志

3. **检查 UI**：
   - 应该看到 3 个关卡选项卡片
   - 点击应该能进入战斗

### 6.3 常见问题排查

**问题1：看不到关卡选择UI**
- 检查 Canvas 是否激活
- 检查 LevelSelectPanel 是否激活
- 检查 Canvas 的 Render Mode 和 Camera 设置

**问题2：点击关卡没有反应**
- 检查 LevelNodeAdapter 的 Click Button 引用是否正确
- 检查 BattleFlowManager 是否存在
- 查看 Console 是否有错误

**问题3：没有生成关卡选项**
- 检查 EncounterManager 是否配置了遭遇池
- 检查是否有可用的遭遇数据
- 查看 Console 的错误信息

---

## 📝 快速检查清单

在开始测试前，确保：

- [ ] 创建了至少 3 个普通遭遇数据
- [ ] 创建了至少 1 个精英遭遇数据
- [ ] 创建了至少 1 个 Boss 遭遇数据
- [ ] EncounterManager 配置了所有遭遇池
- [ ] BattleFlowManager 配置了场景名称
- [ ] 创建了关卡选择 UI（Canvas、Panel、Container）
- [ ] 创建了关卡节点预制体并配置了所有引用
- [ ] LevelSelectUI 配置了所有引用
- [ ] LevelSelectController 配置了 UI 引用
- [ ] 场景已添加到 Build Settings

---

## 🎯 完成！

现在你的遭遇系统应该可以正常工作了。如果遇到问题，请查看 Console 的日志信息，它们会帮助你定位问题所在。
