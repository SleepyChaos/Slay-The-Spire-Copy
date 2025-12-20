# 固定顺序关卡流程配置指南

## 📋 概述

本指南将帮助你配置一个简单的固定顺序游戏流程：
- **关卡顺序**：2个普通战斗 → 1个精英战斗 → 1个Boss战斗（共4关）
- **流程**：每过一关自动进入下一关，每关胜利后可以选择一次奖励卡

---

## 🎯 第一步：配置遭遇序列

### 1.1 在 Unity 编辑器中找到 EncounterManager

1. **打开关卡选择场景**（`LevelSelectScene` 或你的主场景）
2. **在 Hierarchy 中找到 `EncounterManager`**（应该在 `--- SYSTEMS ---` 分类下）
3. **选中 `EncounterManager`**，在 Inspector 面板中查看组件

### 1.2 配置固定顺序

在 Inspector 的 `Encounter Manager` 组件中：

1. **勾选 `Use Fixed Sequence`** ✅
   - 这会启用固定顺序流程，禁用随机选关

2. **配置 `Encounter Sequence` 列表**：
   - 点击 `Encounter Sequence` 列表的 `+` 号，添加 4 个元素
   - **Element 0**: 拖入第一个普通战斗遭遇（例如：`红色史莱姆_normal`）
   - **Element 1**: 拖入第二个普通战斗遭遇（例如：`普通史莱两只_normal`）
   - **Element 2**: 拖入精英战斗遭遇（例如：`史莱姆精英战_elite`）
   - **Element 3**: 拖入Boss战斗遭遇（例如：`蓝色史莱姆BOSS_boss`）

   **配置示例**：
   ```
   Encounter Sequence (Size: 4)
     Element 0: 红色史莱姆_normal
     Element 1: 普通史莱两只_normal
     Element 2: 史莱姆精英战_elite
     Element 3: 蓝色史莱姆BOSS_boss
   ```

---

## ✅ 第二步：验证配置

### 2.1 检查遭遇数据

确保你拖入的遭遇数据都正确配置了：

1. **双击打开每个遭遇数据**（在 `Assets/Data/Encounter/` 文件夹中）
2. **检查以下内容**：
   - ✅ `Enemy Pool` 不为空（至少有一个敌人）
   - ✅ `Enemy Count` 正确（通常是 1 或 2）
   - ✅ `Encounter Type` 正确（Normal/Elite/Boss）

### 2.2 验证配置（可选）

1. **在 Inspector 中选中 `EncounterManager`**
2. **右键点击组件标题** → 选择 `验证配置`
3. **查看 Console**，确认没有警告

---

## 🎮 第三步：测试流程

### 3.1 启动游戏

1. **运行游戏**（点击 Play 按钮）
2. **游戏流程应该是**：
   - 游戏开始 → 直接进入第1关战斗（不显示选关界面）
   - 战斗胜利 → 显示奖励卡选择界面
   - 选择奖励（或跳过）→ 直接进入第2关战斗
   - 重复直到完成所有4关
   - 完成所有关卡后 → 返回开始界面（或显示胜利界面）

### 3.2 检查 Console 日志

游戏运行时，Console 应该显示类似以下日志：

```
🎯 固定顺序流程：直接进入关卡 0
✅ 从固定序列获取遭遇 [0]: 红色史莱姆_normal
🚀 直接开始战斗: 红色史莱姆_normal
...
🔔 GameManager 收到战斗结果: 胜利
🎁 【RewardUI】Show 被调用！准备显示奖励...
...
🚀 准备进入下一关...
🎯 固定顺序流程：直接进入下一关战斗 [1]: 普通史莱两只_normal
```

---

## 🔧 常见问题

### Q1: 游戏开始后没有自动进入战斗

**可能原因**：
- `EncounterManager` 的 `Use Fixed Sequence` 未勾选
- `Encounter Sequence` 列表为空或元素为空

**解决方法**：
- 检查 `EncounterManager` 配置
- 确保序列中的每个元素都拖入了有效的遭遇数据

### Q2: 选择奖励后没有进入下一关

**可能原因**：
- `BattleFlowManager` 未正确配置
- 场景名称不匹配

**解决方法**：
- 检查 `BattleFlowManager` 的 `Battle Scene Name` 是否正确
- 确保战斗场景名称与你的实际场景名称一致

### Q3: 所有关卡完成后没有返回开始界面

**可能原因**：
- `BattleFlowManager` 的 `Start Scene Name` 未设置或错误

**解决方法**：
- 检查 `BattleFlowManager` 的 `Start Scene Name` 字段
- 确保场景名称与你的实际开始场景名称一致

---

## 📝 切换回随机选关模式

如果你想切换回随机选关模式：

1. **在 Inspector 中选中 `EncounterManager`**
2. **取消勾选 `Use Fixed Sequence`** ❌
3. **保存场景**

这样游戏就会恢复为随机选关模式（显示3个选项供玩家选择）。

---

## 🎯 总结

配置完成后，你的游戏流程应该是：

```
开始游戏
  ↓
第1关（普通）→ 战斗 → 奖励选择 → 
第2关（普通）→ 战斗 → 奖励选择 → 
第3关（精英）→ 战斗 → 奖励选择 → 
第4关（Boss）→ 战斗 → 奖励选择 → 
完成！
```

每关之间**不会显示选关界面**，而是**自动进入下一关战斗**。

祝你测试顺利！🎮


