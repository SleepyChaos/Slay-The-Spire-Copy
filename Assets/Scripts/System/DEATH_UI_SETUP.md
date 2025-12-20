# 死亡界面设置指南

## 📋 概述

死亡界面（DeathUI）用于在角色死亡时显示全屏遮挡的死亡画面，提供返回主菜单和重试选项。

---

## 🎨 Unity 编辑器设置步骤

### 第一步：创建死亡界面 UI 结构

1. **在战斗场景中找到 Canvas**（或创建一个新的 Canvas）
   - 如果使用现有 Canvas，确保它的 `Sort Order` 足够高（建议 200+）
   - 如果创建新 Canvas，设置：
     - `Render Mode`: `Screen Space - Overlay` 或 `Screen Space - Camera`
     - `Sort Order`: 200（确保显示在所有其他 UI 之上）

2. **在 Canvas 下创建死亡界面对象**：
   - 右键 Canvas → `UI` → `Panel`
   - 命名为 `DeathPanel`
   - 设置 `RectTransform`：
     - `Anchor`: `Stretch Stretch`（全屏）
     - `Left/Right/Top/Bottom`: 0（填满整个屏幕）

3. **配置 DeathPanel 的 Image 组件**：
   - `Color`: 黑色，Alpha 设为 200-255（半透明到不透明）
   - `Raycast Target`: ✅ 勾选（用于遮挡点击）

4. **在 DeathPanel 下创建子对象**：

   **a. 死亡标题文本**：
   - 右键 DeathPanel → `UI` → `Text - TextMeshPro`
   - 命名为 `DeathTitleText`
   - 设置：
     - `Text`: "你死了" 或 "GAME OVER"
     - `Font Size`: 72-96
     - `Alignment`: 居中
     - `Color`: 红色或白色
     - 位置：屏幕中央偏上

   **b. 死亡信息文本（可选）**：
   - 右键 DeathPanel → `UI` → `Text - TextMeshPro`
   - 命名为 `DeathMessageText`
   - 设置：
     - `Text`: "游戏结束" 或 "你到达了第 X 关"
     - `Font Size`: 36-48
     - `Alignment`: 居中
     - `Color`: 灰色或白色
     - 位置：标题下方

   **c. 返回主菜单按钮**：
   - 右键 DeathPanel → `UI` → `Button - TextMeshPro`
   - 命名为 `ReturnToMenuButton`
   - 设置：
     - `Text`: "返回主菜单"
     - `Font Size`: 36
     - 位置：屏幕中央偏下

   **d. 重试按钮（可选）**：
   - 右键 DeathPanel → `UI` → `Button - TextMeshPro`
   - 命名为 `RetryButton`
   - 设置：
     - `Text`: "重试"
     - `Font Size`: 36
     - 位置：返回主菜单按钮旁边或下方

---

### 第二步：添加 DeathUI 脚本

1. **选中 DeathPanel**（根对象）

2. **添加 DeathUI 组件**：
   - 在 Inspector 中点击 `Add Component`
   - 搜索 `Death UI` 并添加

3. **配置 DeathUI 组件引用**：
   - **Content**: 拖入 `DeathPanel` 自身（或留空，脚本会自动使用 `gameObject`）
   - **Background Image**: 拖入 `DeathPanel` 的 `Image` 组件
   - **Death Title Text**: 拖入 `DeathTitleText`
   - **Death Message Text**: 拖入 `DeathMessageText`（可选）
   - **Return To Menu Button**: 拖入 `ReturnToMenuButton`
   - **Retry Button**: 拖入 `RetryButton`（可选）

4. **配置设置**：
   - **Button Enable Delay**: 1（按钮延迟启用时间，秒）
   - **Use Fade In Animation**: ✅ 勾选（使用淡入动画）
   - **Fade In Duration**: 0.5（淡入动画时长，秒）

---

### 第三步：初始状态设置

1. **确保 DeathPanel 初始状态为隐藏**：
   - 在 Hierarchy 中选中 `DeathPanel`
   - 在 Inspector 中取消勾选 `Active`（或确保 `DeathUI` 脚本的 `Start()` 会隐藏它）

2. **测试设置**：
   - 运行游戏
   - 在代码中临时调用 `DeathUI.Show()` 测试显示效果

---

## 🎯 推荐的 UI 结构

```
Canvas (Sort Order: 200)
└── DeathPanel (Panel, 全屏)
    ├── Background (Image, 半透明黑色)
    ├── DeathTitleText (TextMeshPro, "你死了")
    ├── DeathMessageText (TextMeshPro, "游戏结束", 可选)
    ├── ReturnToMenuButton (Button, "返回主菜单")
    └── RetryButton (Button, "重试", 可选)
```

---

## 🔧 代码集成

死亡界面已经集成到 `GameManager.OnBattleResult()` 中：

- 当 `BattleResultGA(false)` 被触发时（玩家死亡）
- `GameManager` 会自动查找场景中的 `DeathUI`
- 如果找到，调用 `DeathUI.Show()` 显示死亡界面
- 如果未找到，会直接返回主菜单（保底逻辑）

---

## 🎨 自定义选项

### 修改死亡文本

在 Unity 编辑器中：
- 选中 `DeathTitleText`，修改 `Text` 字段
- 选中 `DeathMessageText`，修改 `Text` 字段

或在代码中：
```csharp
deathUI.deathTitleText.text = "自定义标题";
deathUI.deathMessageText.text = "自定义信息";
```

### 修改背景颜色

在 Unity 编辑器中：
- 选中 `DeathPanel` → `Image` 组件
- 修改 `Color`（Alpha 值控制透明度）

### 添加死亡动画

可以在 `DeathUI.Show()` 中添加更多动画效果，例如：
- 缩放动画
- 旋转动画
- 粒子特效

---

## ✅ 验证清单

- [ ] DeathPanel 已创建并设置为全屏
- [ ] DeathUI 脚本已添加到 DeathPanel
- [ ] 所有引用已正确设置（Background Image, Text, Buttons）
- [ ] DeathPanel 初始状态为隐藏
- [ ] Canvas Sort Order 设置为 200+
- [ ] 测试：运行游戏，触发死亡，确认界面正确显示
- [ ] 测试：点击返回主菜单按钮，确认正确跳转
- [ ] 测试：点击重试按钮（如果有），确认正确跳转

---

## 🐛 常见问题

### 问题1：死亡界面不显示

**可能原因**：
- DeathPanel 未激活
- Canvas Sort Order 太低，被其他 UI 遮挡
- DeathUI 脚本引用未设置

**解决方法**：
- 检查 DeathPanel 的 Active 状态
- 提高 Canvas Sort Order
- 检查 Inspector 中的引用设置

### 问题2：按钮无法点击

**可能原因**：
- 按钮的 `Interactable` 未启用
- 有其他 UI 遮挡按钮
- Canvas 的 `GraphicRaycaster` 未启用

**解决方法**：
- 检查按钮的 `Interactable` 状态
- 检查 Canvas 的 `GraphicRaycaster` 组件
- 确保没有其他 UI 在按钮上方

### 问题3：背景不遮挡屏幕

**可能原因**：
- Background Image 的 RectTransform 未设置为全屏
- Image 组件的 `Raycast Target` 未勾选

**解决方法**：
- 设置 RectTransform 的 Anchor 为 `Stretch Stretch`
- 勾选 Image 组件的 `Raycast Target`

---

## 📝 总结

死亡界面系统已经完成，包括：
- ✅ DeathUI 脚本
- ✅ GameManager 集成
- ✅ 淡入动画支持
- ✅ 按钮延迟启用
- ✅ 返回主菜单功能
- ✅ 重试功能（可选）

按照上述步骤在 Unity 编辑器中设置 UI 结构，即可使用死亡界面功能！



