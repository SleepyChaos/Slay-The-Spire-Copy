# Unity 配置指南 - 可视化版

## 📍 基于你的项目结构的具体操作

根据你的项目截图，这里是具体的操作步骤：

---

## 🎯 第一步：检查并创建遭遇数据

### 1.1 检查现有遭遇数据

**在 Project 面板中：**
- 路径：`Assets/Data/Encounter/`
- 我看到你已经有了：
  - `史莱姆精英战` ✅
  - `蓝色史莱姆BOSS` ✅

**需要做的：**
1. **检查这些遭遇数据的配置**：
   - 双击打开 `史莱姆精英战`
   - 在 Inspector 中检查：
     - `Encounter Type` 应该是 `Elite`
     - `Enemy Pool` 应该有敌人数据
     - `Enemy Count` 应该是 1 或更多
   
2. **创建更多普通遭遇**（如果还没有）：
   - 在 `Assets/Data/Encounter/` 文件夹中
   - 右键 → `Create → Data → Encounter`
   - 命名为 `普通战斗_01`、`普通战斗_02` 等
   - 配置：
     ```
     Encounter Type: Normal
     Encounter Name: "普通战斗"
     Enemy Pool: 拖入 `Assets/Data/Enemies/史莱姆` 或其他敌人
     Enemy Count: 1
     Selection Mode: Random
     Min Stage Index: 0
     Max Stage Index: -1
     Weight: 10
     ```

---

## 🎮 第二步：在 Hierarchy 中添加管理器

### 2.1 添加 EncounterManager

**在 Hierarchy 面板中：**
- 找到 `--- SYSTEMS ---` 分类（在右侧）
- 在该分类下：

1. **创建新对象**：
   - 右键点击 `--- SYSTEMS ---`
   - `Create Empty`
   - 命名为 `EncounterManager`

2. **添加组件**：
   - 选中 `EncounterManager`
   - Inspector 面板 → `Add Component`
   - 搜索 `Encounter Manager` 并添加

3. **配置遭遇池**：
   ```
   Normal Encounters:
     - Size: 2（或你创建的普通遭遇数量）
     - Element 0: 拖入 `Assets/Data/Encounter/普通战斗_01`
     - Element 1: 拖入 `Assets/Data/Encounter/普通战斗_02`
   
   Elite Encounters:
     - Size: 1
     - Element 0: 拖入 `Assets/Data/Encounter/史莱姆精英战`
   
   Boss Encounters:
     - Size: 1
     - Element 0: 拖入 `Assets/Data/Encounter/蓝色史莱姆BOSS`
   ```

### 2.2 添加 BattleFlowManager

**同样在 `--- SYSTEMS ---` 分类下：**

1. **创建新对象**：
   - 右键点击 `--- SYSTEMS ---`
   - `Create Empty`
   - 命名为 `BattleFlowManager`

2. **添加组件**：
   - 选中 `BattleFlowManager`
   - Inspector → `Add Component`
   - 搜索 `Battle Flow Manager` 并添加

3. **配置场景名称**：
   ```
   Battle Scene Name: "BattleScene"（或你的战斗场景名称）
   Level Select Scene Name: "LevelSelectScene"（或你的关卡选择场景名称）
   ```

---

## 🎨 第三步：创建关卡选择 UI（参考 RewardPanel）

### 3.1 在 UI 分类下创建关卡选择面板

**在 Hierarchy 面板中：**
- 找到 `--- UI ---` 分类
- 参考 `RewardPanel` 的结构

1. **创建 Panel**：
   - 右键点击 `Canvas`（在 `--- UI ---` 下）
   - `UI → Panel`
   - 命名为 `LevelSelectPanel`

2. **配置 Panel**：
   ```
   Rect Transform:
     - Anchor: Stretch Stretch（全屏）
     - Left, Right, Top, Bottom: 0
   
   Image:
     - Color: (0, 0, 0, 200) 或半透明黑色
   ```

3. **添加 LevelSelectUI 组件**：
   - 选中 `LevelSelectPanel`
   - Inspector → `Add Component`
   - 搜索 `Level Select UI` 并添加

### 3.2 创建节点容器（参考 Container）

**在 `LevelSelectPanel` 下：**

1. **创建容器**：
   - 右键点击 `LevelSelectPanel`
   - `UI → Empty` 或 `GameObject → Create Empty`
   - 命名为 `LevelNodeContainer`

2. **配置容器**：
   ```
   Rect Transform:
     - Anchor: Middle Center
     - Width: 1200
     - Height: 400
     - Pos X: 0, Pos Y: 0
   
   添加 Horizontal Layout Group 组件:
     - Spacing: 50
     - Child Alignment: Middle Center
   ```

3. **在 LevelSelectUI 中配置**：
   - 选中 `LevelSelectPanel`
   - Inspector 的 `Level Select UI` 组件：
     ```
     Content: 拖入 LevelSelectPanel（自己）
     Node Container: 拖入 LevelNodeContainer
     Node Prefab: （稍后创建）
     ```

### 3.3 创建关卡节点预制体（参考 RewardCardContainer）

**在 Project 面板中：**
- 路径：`Assets/Prefabs/`
- 我看到你已经有了 `RewardCardContainer`，我们可以参考它的结构

1. **在 Hierarchy 中创建临时对象**：
   - 右键点击 `Canvas`（或任意位置）
   - `UI → Button`
   - 命名为 `LevelNode`（临时，稍后保存为预制体）

2. **配置按钮外观**：
   ```
   Rect Transform:
     - Width: 300
     - Height: 400
     - Position: (0, 0, 0)
   
   Image (Button 自带):
     - Source Image: 可以留空或设置背景
     - Color: 白色或浅灰色
   ```

3. **添加子元素**：

   **a. 标题文本**：
   - 选中 `LevelNode`
   - 右键 → `UI → Text - TextMeshPro`
   - 命名为 `TitleText`
   - 配置：
     ```
     Rect Transform:
       - Anchor: Top Center
       - Pos Y: -30
       - Width: 280
       - Height: 50
     TextMeshPro:
       - Text: "关卡标题"
       - Font Size: 36
       - Alignment: Center
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
     TextMeshPro:
       - Text: "关卡描述"
       - Font Size: 24
       - Alignment: Center
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
     ```

4. **添加 LevelNodeAdapter 组件**：
   - 选中 `LevelNode`
   - Inspector → `Add Component`
   - 搜索 `Level Node Adapter` 并添加

5. **配置 LevelNodeAdapter**：
   ```
   Click Button: 拖入 LevelNode（Button 组件）
   Icon Image: 拖入 IconImage
   Title Text: 拖入 TitleText
   Description Text: 拖入 DescriptionText
   Elite Badge: （可选，留空）
   Boss Badge: （可选，留空）
   ```

6. **保存为预制体**：
   - 从 Hierarchy 拖拽 `LevelNode` 到 `Assets/Prefabs/` 文件夹
   - 命名为 `LevelNodePrefab`
   - 删除 Hierarchy 中的 `LevelNode`（因为已经是预制体了）

7. **在 LevelSelectUI 中配置预制体**：
   - 选中 `LevelSelectPanel`
   - Inspector 的 `Level Select UI` 组件
   - **Node Prefab**: 拖入 `Assets/Prefabs/LevelNodePrefab`

---

## 🔗 第四步：添加 LevelSelectController

**在 Hierarchy 面板中：**
- 可以在 `--- SYSTEMS ---` 或 `--- CREATORS ---` 分类下

1. **创建控制器对象**：
   - 右键点击 `--- SYSTEMS ---`
   - `Create Empty`
   - 命名为 `LevelSelectController`

2. **添加组件**：
   - 选中 `LevelSelectController`
   - Inspector → `Add Component`
   - 搜索 `Level Select Controller` 并添加

3. **配置引用**：
   ```
   Level Select UI: 拖入 LevelSelectPanel（在 --- UI --- 下）
   ```

---

## ✅ 第五步：检查配置

### 检查清单：

**在 Hierarchy 中应该看到：**
- [ ] `--- SYSTEMS ---` 下有 `EncounterManager`
- [ ] `--- SYSTEMS ---` 下有 `BattleFlowManager`
- [ ] `--- SYSTEMS ---` 下有 `LevelSelectController`
- [ ] `--- UI ---` 下有 `LevelSelectPanel`
- [ ] `LevelSelectPanel` 下有 `LevelNodeContainer`

**在 Project 中应该看到：**
- [ ] `Assets/Data/Encounter/` 下有至少 3 个遭遇数据
- [ ] `Assets/Prefabs/` 下有 `LevelNodePrefab`

**在 Inspector 中检查：**
- [ ] `EncounterManager` 的遭遇池都已配置
- [ ] `BattleFlowManager` 的场景名称正确
- [ ] `LevelSelectUI` 的所有引用都已拖入
- [ ] `LevelNodeAdapter` 的所有引用都已拖入
- [ ] `LevelSelectController` 的 UI 引用已拖入

---

## 🎮 第六步：测试

1. **运行游戏**：
   - 点击 Play 按钮
   - 或者通过代码调用 `GameManager.StartNewRun()`

2. **查看 Console**：
   - 应该看到 "生成关卡选项" 的日志
   - 如果有错误，根据错误信息调整

3. **检查 UI**：
   - 应该看到 3 个关卡选项卡片
   - 点击应该能进入战斗

---

## 📝 快速操作总结

1. **Project 面板**：
   - `Assets/Data/Encounter/` → 创建遭遇数据
   - `Assets/Prefabs/` → 保存关卡节点预制体

2. **Hierarchy 面板**：
   - `--- SYSTEMS ---` → 添加 `EncounterManager`、`BattleFlowManager`、`LevelSelectController`
   - `--- UI ---` → 添加 `LevelSelectPanel` 和 `LevelNodeContainer`

3. **Inspector 面板**：
   - 配置所有组件的引用（拖拽操作）

---

## ⚠️ 注意事项

1. **场景名称**：确保 `BattleFlowManager` 中的场景名称与你的实际场景名称一致
   - 如果战斗场景叫 `SampleScene`，就填 `SampleScene`
   - 如果关卡选择场景不存在，需要先创建

2. **Canvas 设置**：确保 `Canvas` 的 Render Mode 正确
   - 如果使用 3D 元素（如 CardView），建议用 `Screen Space - Camera`

3. **预制体引用**：所有引用都要从 Hierarchy 或 Project 拖入，不要手动输入路径

现在按照这个指南操作，应该就能完成配置了！
