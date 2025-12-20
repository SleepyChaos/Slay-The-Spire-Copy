# Interactions 组件设置指南

## 📋 概述

`Interactions.cs` 是一个 `MonoSingleton` 脚本，用于管理玩家的交互状态（如是否允许拖拽卡牌、UI是否打开等）。它需要挂载到场景中的 GameObject 上。

---

## 🎯 Unity 编辑器设置步骤

### 方法1：在现有 SYSTEMS 组下创建（推荐）

1. **打开战斗场景**（如 `SampleScene` 或 `BattleScene`）

2. **在 Hierarchy 中找到 `--- SYSTEMS ---` 组**
   - 如果不存在，创建一个空 GameObject，命名为 `--- SYSTEMS ---`

3. **在 `--- SYSTEMS ---` 下创建 Interactions 对象**：
   - 右键 `--- SYSTEMS ---` → `Create Empty`
   - 命名为 `Interactions`

4. **添加 Interactions 组件**：
   - 选中 `Interactions` GameObject
   - 在 Inspector 中点击 `Add Component`
   - 搜索 `Interactions` 并添加

5. **验证设置**：
   - 运行游戏
   - 在 Console 中应该看到：`创建单例实例：Interactions`

---

### 方法2：自动创建（如果场景中没有）

`MonoSingleton` 有自动创建功能：
- 如果场景中没有 `Interactions` 组件，代码会自动创建一个 GameObject
- 但**建议手动创建**，以便更好地组织场景结构

---

## 📁 推荐的 Hierarchy 结构

```
SampleScene (或 BattleScene)
├── --- SYSTEMS ---
│   ├── GameManager
│   ├── Interactions          ← 这里！
│   ├── HeroSystem
│   ├── CardSystem
│   ├── EnemySystem
│   ├── ActionSystem
│   └── ... (其他系统)
├── Canvas
│   ├── RewardPanel
│   ├── DeathPanel
│   └── ... (其他UI)
└── ... (其他对象)
```

---

## ✅ 验证清单

- [ ] `Interactions` GameObject 已创建在 `--- SYSTEMS ---` 下
- [ ] `Interactions` 组件已添加到 GameObject
- [ ] 运行游戏，Console 显示 `创建单例实例：Interactions`
- [ ] 没有 `MissingReferenceException` 错误

---

## 🔍 如何检查是否已正确设置

### 方法1：在 Hierarchy 中查找
- 搜索 `Interactions`
- 应该找到一个 GameObject，上面有 `Interactions` 组件

### 方法2：在代码中检查
运行游戏后，在 Console 中应该看到：
```
创建单例实例：Interactions
```

### 方法3：运行时检查
在代码中临时添加：
```csharp
if (Interactions.Instance != null)
{
    Debug.Log("✅ Interactions 已正确设置");
}
else
{
    Debug.LogError("❌ Interactions 未找到！");
}
```

---

## 🐛 常见问题

### 问题1：找不到 Interactions.Instance

**可能原因**：
- 场景中没有 `Interactions` GameObject
- `Interactions` 组件未添加到 GameObject

**解决方法**：
- 按照上述步骤创建并添加组件
- 或者让代码自动创建（不推荐，不利于场景管理）

### 问题2：有多个 Interactions 实例

**可能原因**：
- 场景中有多个 `Interactions` GameObject
- 或者代码自动创建了新的实例

**解决方法**：
- 删除多余的 `Interactions` GameObject
- 确保只有一个 `Interactions` 组件在场景中

### 问题3：Interactions 在场景切换时被销毁

**说明**：
- `Interactions` 默认不会在场景切换时保留（没有 `DontDestroyOnLoad`）
- 如果需要跨场景保留，可以在 `Interactions.cs` 的 `Awake()` 中添加：
  ```csharp
  protected override void Awake()
  {
      base.Awake();
      DontDestroyOnLoad(gameObject);
  }
  ```
- 但通常**不需要**跨场景保留，因为每个场景都应该有自己的 `Interactions` 实例

---

## 📝 总结

**设置步骤**：
1. ✅ 在 `--- SYSTEMS ---` 下创建 `Interactions` GameObject
2. ✅ 添加 `Interactions` 组件
3. ✅ 运行游戏验证

**位置**：
- Hierarchy: `--- SYSTEMS ---` → `Interactions`
- 组件: `Interactions` (Script)

设置完成后，`Interactions.Instance` 就可以正常使用了！


