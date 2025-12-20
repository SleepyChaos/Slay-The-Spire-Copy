# 开始界面设置指南

## 📋 概述

开始界面（StartUI）是玩家进入游戏和死亡后返回的界面，包含一个"开始游戏"按钮。

---

## 🎨 Unity 编辑器设置步骤

### 第一步：创建 StartScene 场景（如果还没有）

1. **创建新场景**：
   - `File` → `New Scene`
   - 选择 `Basic (Built-in)` 或 `2D` 模板
   - 保存为 `Assets/Scenes/StartScene.unity`

2. **添加到 Build Settings**：
   - `File` → `Build Settings`
   - 点击 `Add Open Scenes` 或拖拽 `StartScene.unity` 到列表
   - 确保 `StartScene` 在列表的第一位（索引 0），这样它就是游戏的启动场景

---

### 第二步：创建开始界面 UI 结构

1. **创建 Canvas**：
   - 右键 Hierarchy → `UI` → `Canvas`
   - 命名为 `Canvas`
   - 设置：
     - `Render Mode`: `Screen Space - Overlay`
     - `Sort Order`: 0（默认）

2. **在 Canvas 下创建开始界面 Panel**：
   - 右键 Canvas → `UI` → `Panel`
   - 命名为 `StartPanel`
   - 设置 `RectTransform`：
     - `Anchor`: `Stretch Stretch`（全屏）
     - `Left/Right/Top/Bottom`: 0（填满整个屏幕）

3. **配置 StartPanel 的 Image 组件**：
   - `Color`: 可以设置为背景色（如深色或半透明）
   - `Raycast Target`: ✅ 勾选（用于遮挡点击）

4. **在 StartPanel 下创建子对象**：

   **a. 标题文本（可选）**：
   - 右键 StartPanel → `UI` → `Text - TextMeshPro`
   - 命名为 `TitleText`
   - 设置：
     - `Text`: "Slay The Spire" 或你的游戏名称
     - `Font Size`: 72-96
     - `Alignment`: 居中
     - `Color`: 白色或其他醒目颜色
     - 位置：屏幕中央偏上

   **b. 副标题文本（可选）**：
   - 右键 StartPanel → `UI` → `Text - TextMeshPro`
   - 命名为 `SubtitleText`
   - 设置：
     - `Text`: "点击开始游戏" 或 "Press to Start"
     - `Font Size`: 36-48
     - `Alignment`: 居中
     - `Color`: 灰色或白色
     - 位置：标题下方

   **c. 开始游戏按钮**：
   - 右键 StartPanel → `UI` → `Button - TextMeshPro`
   - 命名为 `StartGameButton`
   - 设置：
     - `Text`: "开始游戏" 或 "Start Game"
     - `Font Size`: 48
     - 位置：屏幕中央

---

### 第三步：添加 StartUI 脚本

1. **选中 StartPanel**（根对象）

2. **添加 StartUI 组件**：
   - 在 Inspector 中点击 `Add Component`
   - 搜索 `Start UI` 并添加

3. **配置 StartUI 组件引用**：
   - **Content**: 拖入 `StartPanel` 自身（或留空，脚本会自动使用 `gameObject`）
   - **Start Game Button**: 拖入 `StartGameButton`
   - **Title Text**: 拖入 `TitleText`（可选）
   - **Subtitle Text**: 拖入 `SubtitleText`（可选）

4. **配置游戏设置**：
   - **Default Hero Data**: 拖入你的默认英雄数据（如 `Assets/Data/Heros/DefaultHero.asset`）

---

### 第四步：确保 GameManager 存在

1. **在 StartScene 中创建 GameManager**：
   - 右键 Hierarchy → `Create Empty`
   - 命名为 `GameManager`
   - 添加 `Game Manager` 组件

2. **配置 GameManager**：
   - **All Cards Library**: 拖入所有可用的卡牌数据

3. **注意**：GameManager 使用 `DontDestroyOnLoad`，所以它会在场景切换时保留

---

### 第五步：设置启动场景

1. **打开 Build Settings**：
   - `File` → `Build Settings`

2. **确保 StartScene 在列表的第一位**：
   - 拖拽 `StartScene` 到列表顶部
   - 或者选中 `StartScene`，点击 `Add Open Scenes`

3. **验证**：
   - 运行游戏时，应该首先进入 `StartScene`

---

## 🎯 推荐的 UI 结构

```
StartScene
├── Canvas (Sort Order: 0)
│   └── StartPanel (Panel, 全屏)
│       ├── TitleText (TextMeshPro, "游戏标题")
│       ├── SubtitleText (TextMeshPro, "副标题", 可选)
│       └── StartGameButton (Button, "开始游戏")
└── GameManager (GameObject, DontDestroyOnLoad)
    └── Game Manager (Script)
```

---

## 🔧 代码集成

开始界面已经集成到游戏流程中：

- **游戏启动**：Unity 自动加载 `StartScene`（如果设置为启动场景）
- **开始游戏**：点击按钮 → `StartUI.OnStartGameClicked()` → `GameManager.StartNewRun()` → 加载 `LevelSelectScene`
- **死亡返回**：`DeathUI.OnReturnToMenuClicked()` → `BattleFlowManager.GoToStartScene()` → 加载 `StartScene`

---

## ✅ 验证清单

- [ ] `StartScene` 已创建并添加到 Build Settings
- [ ] `StartScene` 在 Build Settings 列表的第一位（索引 0）
- [ ] Canvas 和 StartPanel 已创建
- [ ] StartGameButton 已创建并配置
- [ ] StartUI 脚本已添加到 StartPanel
- [ ] StartUI 的所有引用已正确设置（Button, HeroData）
- [ ] GameManager 在场景中存在
- [ ] 测试：运行游戏，确认进入 StartScene
- [ ] 测试：点击开始游戏按钮，确认进入 LevelSelectScene
- [ ] 测试：死亡后返回，确认回到 StartScene

---

## 🐛 常见问题

### 问题1：游戏启动时没有进入 StartScene

**可能原因**：
- `StartScene` 未添加到 Build Settings
- `StartScene` 不在列表的第一位

**解决方法**：
- 打开 Build Settings
- 确保 `StartScene` 在列表顶部（索引 0）

### 问题2：点击开始游戏按钮没有反应

**可能原因**：
- `StartGameButton` 引用未设置
- `Default Hero Data` 未设置
- `GameManager` 不存在

**解决方法**：
- 检查 StartUI 组件的 Inspector 设置
- 确保所有引用都已正确设置
- 检查 Console 是否有错误日志

### 问题3：GameManager 为空

**可能原因**：
- 场景中没有 GameManager GameObject
- GameManager 组件未添加

**解决方法**：
- 在 StartScene 中创建 GameManager
- 添加 `Game Manager` 组件

---

## 📝 总结

开始界面系统已经完成，包括：
- ✅ StartUI 脚本
- ✅ 开始游戏按钮功能
- ✅ GameManager 集成
- ✅ 死亡后返回功能

按照上述步骤在 Unity 编辑器中设置 UI 结构，即可使用开始界面功能！


