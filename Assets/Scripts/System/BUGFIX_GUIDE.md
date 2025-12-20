# 问题修复指南

## 🔧 已修复的问题

### 问题1：模拟战斗依然存在 ✅

**原因**：`MatchSetupSystem` 优先检查 `GameManager.Instance.NextEnemies`，如果为空就使用默认的 `enemyDatas`。

**修复**：修改了 `MatchSetupSystem.cs`，现在优先使用 `BattleFlowManager.Instance.CurrentEncounter` 的敌人配置。

**结果**：现在会优先使用配置好的遭遇序列中的敌人，而不是默认的测试敌人。

---

### 问题2：场景加载错误 ✅

**原因**：
- `GameManager.AdvanceStage()` 直接加载 `LevelSelectScene`，但这个场景可能不存在或未添加到 Build Settings
- 在固定顺序流程中，不应该返回选关界面

**修复**：
1. 修改了 `GameManager.AdvanceStage()`，在固定顺序流程中不再加载 `LevelSelectScene`
2. `RewardUI.GoToNextLevel()` 已经正确处理了固定顺序流程

**结果**：固定顺序流程中，选择奖励后直接进入下一关战斗，不会尝试加载不存在的场景。

---

### 问题3：Camera 被销毁导致无法打出卡牌 ✅

**原因**：`MouseUtil.camera` 是静态变量，在场景切换时 `Camera.main` 被销毁，但引用仍然指向已销毁的对象。

**修复**：
1. 修改了 `MouseUtil.cs`，使用属性自动重新获取 Camera
2. 在 `BattleFlowManager` 的场景加载回调中重置 Camera 引用

**结果**：场景切换后，`MouseUtil` 会自动重新获取新的 Camera，卡牌可以正常拖拽。

---

## 📝 需要在 Unity 中配置的内容

### 1. 配置 BattleFlowManager 的场景名称

**重要**：你的战斗场景是 `SampleScene`，不是 `BattleScene`！

1. **在 Hierarchy 中找到 `BattleFlowManager`**（在 `--- SYSTEMS ---` 下）
2. **在 Inspector 中配置**：
   ```
   Battle Scene Name: "SampleScene"  ← 改为你的实际场景名称
   Level Select Scene Name: "LevelSelectScene"  ← 如果不存在，可以留空或填 "SampleScene"
   Start Scene Name: "StartScene"  ← 如果不存在，可以留空或填 "SampleScene"
   ```

### 2. 确保场景已添加到 Build Settings（可选）

如果你确实有 `LevelSelectScene` 和 `StartScene`：

1. **打开 Build Settings**：`File → Build Settings`
2. **添加场景**：
   - 点击 `Add Open Scenes` 添加当前打开的场景
   - 或者拖拽场景文件到列表中
3. **确保场景顺序正确**（通常不重要，但建议按游戏流程排序）

---

## ✅ 测试步骤

1. **配置 BattleFlowManager**：
   - 将 `Battle Scene Name` 改为 `"SampleScene"`

2. **运行游戏**：
   - 应该直接进入第1关战斗（不再有模拟战斗）
   - 战斗胜利后显示奖励选择
   - 选择奖励后直接进入第2关战斗
   - 卡牌可以正常拖拽和使用

3. **检查 Console**：
   - 应该看到类似以下日志：
     ```
     ✅ 从 BattleFlowManager 读取遭遇 'xxx' 的敌人配置
     ✅ 战斗场景加载完成，已重置 MouseUtil Camera
     ```

---

## 🐛 如果还有问题

### 问题：仍然出现模拟战斗

**检查**：
- `EncounterManager` 的 `Use Fixed Sequence` 是否已勾选
- `Encounter Sequence` 列表是否已配置
- `BattleFlowManager` 的 `Battle Scene Name` 是否正确

### 问题：仍然无法打出卡牌

**检查**：
- 场景中是否有 `Main Camera` 或任何 `Camera` 组件
- Console 是否有 `MouseUtil: 无法找到 Camera` 的错误

### 问题：场景加载错误

**检查**：
- `BattleFlowManager` 的场景名称是否正确
- 如果场景不存在，确保场景名称与你的实际场景名称一致

---

## 📌 总结

所有代码修复已完成，你只需要：

1. ✅ **配置 BattleFlowManager**：将 `Battle Scene Name` 改为 `"SampleScene"`
2. ✅ **测试游戏流程**：应该可以正常进行固定顺序的关卡流程

祝你测试顺利！🎮

