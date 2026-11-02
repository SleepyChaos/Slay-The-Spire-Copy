# Slay the Spire 风格卡牌战斗学习项目

基于 Unity 的卡牌战斗复现与学习仓库，Fork 自 [ArnoldRedman/Slay-The-Spire-Copy](https://github.com/ArnoldRedman/Slay-The-Spire-Copy)。本仓保留上游来源，不代表《Slay the Spire》官方项目或完整商业游戏。

## 打开项目

1. 使用 Unity Hub 打开仓库根目录；编辑器版本见 [`ProjectSettings/ProjectVersion.txt`](ProjectSettings/ProjectVersion.txt)，当前记录为 **2022.3.62f2c1**。
2. 等待 Unity 按 [`Packages/manifest.json`](Packages/manifest.json) 解析依赖并完成资源导入。
3. 打开 [`Assets/Scenes/SampleScene.unity`](Assets/Scenes/SampleScene.unity)，进入 Play 模式查看场景。

这是带有 `c1` 后缀的编辑器版本记录；使用其他版本时先在副本中确认兼容性。首次导入会生成 `Library/` 等本机缓存，它们不需要提交。

## 代码阅读入口

| 位置 | 内容 |
| --- | --- |
| [`Assets/Scripts/System/MatchSetupSystem.cs`](Assets/Scripts/System/MatchSetupSystem.cs) | 场景启动时初始化角色、卡组、敌人与初始抽牌 |
| [`Assets/Scripts/General/ActionSystem/ActionSystem.cs`](Assets/Scripts/General/ActionSystem/ActionSystem.cs) | 动作与反应的调度入口 |
| [`Assets/Scripts/GameActions/`](Assets/Scripts/GameActions/) | 抽牌、打牌、消耗法力与回合等动作 |
| [`Assets/Scripts/Data/`](Assets/Scripts/Data/) | 卡牌、敌人、角色和增益数据 |
| [`Assets/Scripts/Views/`](Assets/Scripts/Views/) | 卡牌、手牌和战斗单位等显示组件 |

这些入口用于理解现有战斗原型，不代表完整地图、存档或全部卡牌内容均已实现。运行效果以指定场景和实际资源引用为准。

## 来源与资源

个人修改需与上游提交区分；阅读本仓时可通过 Git 历史和上游比较核对具体差异。`Assets/` 含字体、音画素材和第三方插件，其使用范围遵循各自来源和许可，不能用一份个人许可证覆盖全部资源。本仓未提供根许可证文件。

项目没有独立的命令行测试入口。本地验证应检查资源导入、SampleScene 的引用与 Play 模式交互；README 不作为跨版本导入或可发布成品的验证结论。
