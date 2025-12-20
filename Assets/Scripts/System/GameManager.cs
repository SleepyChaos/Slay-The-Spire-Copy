using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{

    [Header("流程控制")]
    public int CurrentStageIndex = 0; // 当前第几关 (0开始)
    public const int BOSS_STAGE_INDEX = 9; // 第10关是BOSS (索引9，即完成9次闯关后)

    [Header("全卡牌图鉴 (请在编辑器里把所有能抽到的卡拖进去)")]
    public List<CardData> AllCardsLibrary; 

    [Header("全局状态")]
    public int CurrentHealth;
    public int MaxHealth;
    public List<CardData> MasterDeck = new List<CardData>();
    public List<PerkData> Relics = new List<PerkData>(); // 获得的遗物

    // 下一场战斗的配置
    public List<EnemyData> NextEnemies;
    public bool IsEliteBattle; // 标记是不是精英战

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun(HeroData heroData)
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎮 【开始新游戏】GameManager.StartNewRun 被调用");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        CurrentStageIndex = 0; // 重置关卡
        MaxHealth = heroData.Health;
        CurrentHealth = heroData.Health;
        
        MasterDeck.Clear();
        MasterDeck.AddRange(heroData.Deck);
        
        Relics.Clear();
        if(heroData.StartingPerks != null) Relics.AddRange(heroData.StartingPerks);
        
        Debug.Log($"📋 游戏初始化完成:");
        Debug.Log($"   - 关卡索引: {CurrentStageIndex}");
        Debug.Log($"   - 玩家血量: {CurrentHealth}/{MaxHealth}");
        Debug.Log($"   - 牌库数量: {MasterDeck.Count}");
        Debug.Log($"   - 遗物数量: {Relics.Count}");

        // 【关键修复】检查当前场景，只有在 StartScene 中才自动开始游戏
        // 如果是从其他场景调用（比如返回主菜单后），不应该自动开始
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool isInStartScene = currentSceneName == "StartScene" || 
                             (BattleFlowManager.Instance != null && 
                              currentSceneName == BattleFlowManager.Instance.startSceneName);
        
        Debug.Log($"   - 当前场景: {currentSceneName}");
        Debug.Log($"   - 是否在开始场景: {isInStartScene}");
        
        // 检查是否使用固定顺序流程
        if (EncounterManager.Instance != null && EncounterManager.Instance.useFixedSequence)
        {
            // 【关键修复】只有在 StartScene 中才自动开始游戏
            // 如果是从其他场景调用（比如返回主菜单后），不应该自动开始
            if (!isInStartScene)
            {
                Debug.LogWarning("⚠️ 不在 StartScene 中，跳过自动开始游戏");
                Debug.LogWarning("   - 这是正常的，返回主菜单时不应该自动开始游戏");
                return;
            }
            
            Debug.Log("🎯 检测到固定顺序流程，直接进入第一关战斗");
            
            // 检查 EncounterManager 是否配置了遭遇序列
            if (EncounterManager.Instance.encounterSequence == null || EncounterManager.Instance.encounterSequence.Count == 0)
            {
                Debug.LogError("❌ EncounterManager.encounterSequence 为空！无法开始游戏。");
                Debug.LogError("💡 请在 EncounterManager 的 Inspector 中配置 Encounter Sequence 列表。");
                return;
            }
            
            // 获取第一关的遭遇（索引 0）
            EncounterData firstEncounter = EncounterManager.Instance.GetEncounterByIndex(0);
            
            if (firstEncounter == null)
            {
                Debug.LogError("❌ 无法获取第一关的遭遇数据！");
                Debug.LogError("💡 请确保 Encounter Sequence 的第一个元素已正确设置。");
                return;
            }
            
            Debug.Log($"✅ 第一关遭遇: {firstEncounter.encounterName} ({firstEncounter.encounterType})");
            
            // 直接开始第一关战斗
            if (BattleFlowManager.Instance != null)
            {
                Debug.Log("🚀 准备开始第一关战斗...");
                BattleFlowManager.Instance.StartBattle(firstEncounter);
            }
            else
            {
                Debug.LogError("❌ BattleFlowManager.Instance 为空！无法开始战斗。");
                Debug.LogError("💡 请确保场景中有 BattleFlowManager 组件。");
            }
        }
        else
        {
            // 随机选关模式：加载关卡选择界面
            Debug.Log("🎲 随机选关模式，加载关卡选择界面");
            string levelSelectSceneName = "LevelSelectScene";
            if (BattleFlowManager.Instance != null && !string.IsNullOrEmpty(BattleFlowManager.Instance.levelSelectSceneName))
            {
                levelSelectSceneName = BattleFlowManager.Instance.levelSelectSceneName;
            }
            
            Debug.Log($"🚀 正在加载关卡选择场景: {levelSelectSceneName}");
            SceneManager.LoadScene(levelSelectSceneName);
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }

    public void AdvanceStage()
    {
        int previousStage = CurrentStageIndex;
        CurrentStageIndex++;
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("📈 【关卡进度更新】进入下一关");
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log($"   - 上一关索引: {previousStage}");
        Debug.Log($"   - 当前关索引: {CurrentStageIndex}");
        Debug.Log($"   - 玩家血量: {CurrentHealth}/{MaxHealth}");
        Debug.Log($"   - 牌库数量: {MasterDeck.Count}");
        
        // 检查是否使用固定顺序流程
        if (EncounterManager.Instance != null && EncounterManager.Instance.useFixedSequence)
        {
            // 固定顺序流程：不加载 LevelSelectScene，直接由 RewardUI 处理下一关
            bool hasNext = EncounterManager.Instance.HasNextEncounter(CurrentStageIndex - 1);
            Debug.Log($"   - 固定顺序流程: {(hasNext ? "还有下一关" : "所有关卡已完成")}");
            if (hasNext)
            {
                EncounterData nextEncounter = EncounterManager.Instance.GetEncounterByIndex(CurrentStageIndex);
                if (nextEncounter != null)
                {
                    Debug.Log($"   - 下一关遭遇: {nextEncounter.encounterName} ({nextEncounter.encounterType})");
                }
            }
            Debug.Log($"✅ 关卡索引已更新，等待 RewardUI 处理下一关");
            return; // RewardUI.GoToNextLevel() 会处理场景切换
        }
        else
        {
            // 随机选关模式：返回关卡选择界面
            Debug.Log($"   - 随机选关模式: 将返回选关界面");
            Debug.Log($"🔙 返回关卡选择界面");
            if (BattleFlowManager.Instance != null)
            {
                BattleFlowManager.Instance.ReturnToLevelSelect();
            }
            else
            {
                SceneManager.LoadScene("LevelSelectScene");
            }
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }

    // ... 在 GameManager.cs 中

    private void OnEnable()
    {
        Debug.Log("GameManager 开始监听 BattleResultGA");
        // 使用 POST 时机，确保战斗结果完全处理后再显示奖励UI
        ActionSystem.SubscribeReaction<BattleResultGA>(OnBattleResult, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<BattleResultGA>(OnBattleResult, ReactionTiming.POST);
    }

    private void OnBattleResult(BattleResultGA result)
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🏁 【关卡结束】战斗结果已确定");
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log($"📊 战斗结果: {(result.IsWin ? "✅ 胜利" : "❌ 失败")}");
        Debug.Log($"   - 关卡索引: {CurrentStageIndex}");

        if (result.IsWin)
        {
            // 保存血量
            if (HeroSystem.Instance != null)
            {
                int previousHealth = CurrentHealth;
                CurrentHealth = HeroSystem.Instance.HeroView.CurrentHealth;
                Debug.Log($"   - 战斗前血量: {previousHealth}/{MaxHealth}");
                Debug.Log($"   - 战斗后血量: {CurrentHealth}/{MaxHealth}");
                Debug.Log($"   - 损失血量: {previousHealth - CurrentHealth}");
            }

            // 【关键】检查是否是Boss战斗胜利
            bool isBossBattle = false;
            if (BattleFlowManager.Instance != null && BattleFlowManager.Instance.CurrentEncounter != null)
            {
                isBossBattle = BattleFlowManager.Instance.CurrentEncounter.encounterType == EncounterType.Boss;
                Debug.Log($"   - 遭遇类型: {BattleFlowManager.Instance.CurrentEncounter.encounterType}");
            }
            
            // 【关键修复】检查是否是最后一个关卡
            bool isLastStage = false;
            if (EncounterManager.Instance != null && EncounterManager.Instance.useFixedSequence)
            {
                int totalStages = EncounterManager.Instance.encounterSequence != null ? EncounterManager.Instance.encounterSequence.Count : 0;
                // 当前关卡索引是 CurrentStageIndex，如果 CurrentStageIndex + 1 >= totalStages，说明这是最后一关
                isLastStage = (CurrentStageIndex + 1) >= totalStages;
                Debug.Log($"   - 当前关卡索引: {CurrentStageIndex}");
                Debug.Log($"   - 总关卡数: {totalStages}");
                Debug.Log($"   - 是否是最后一关: {isLastStage}");
            }
            
            // 检查是否还有下一关
            bool hasNext = false;
            if (EncounterManager.Instance != null && EncounterManager.Instance.useFixedSequence)
            {
                hasNext = EncounterManager.Instance.HasNextEncounter(CurrentStageIndex);
                Debug.Log($"   - 固定顺序流程: {(hasNext ? "还有下一关" : "所有关卡已完成")}");
            }
            else
            {
                Debug.Log($"   - 随机选关模式: 将返回选关界面");
            }

            // 【关键修复】如果是最后一个关卡（Boss或非Boss），直接显示胜利界面，不显示奖励界面
            if (isLastStage)
            {
                Debug.Log("🎉 最后一关完成，显示胜利界面（跳过奖励界面）");
                VictoryUI victoryUI = FindObjectOfType<VictoryUI>(true);
                if (victoryUI != null)
                {
                    Debug.Log($"✅ 找到 VictoryUI: {victoryUI.gameObject.name}");
                    victoryUI.Show();
                }
                else
                {
                    Debug.LogError("❌ 场景里找不到 VictoryUI 脚本！无法显示胜利界面。");
                    Debug.LogError("💡 请确保场景中有 VictoryUI 组件，或者直接返回主菜单。");
                    // 保底逻辑：直接返回主菜单
                    string startSceneName = "StartScene";
                    SceneManager.LoadScene(startSceneName);
                }
                return; // 不显示奖励界面
            }

            // 【关键】如果是Boss战斗胜利且没有下一关，显示胜利界面
            if (isBossBattle && !hasNext)
            {
                Debug.Log("🎉 Boss战斗胜利且所有关卡已完成，显示胜利界面");
                VictoryUI victoryUI = FindObjectOfType<VictoryUI>(true);
                if (victoryUI != null)
                {
                    Debug.Log($"✅ 找到 VictoryUI: {victoryUI.gameObject.name}");
                    victoryUI.Show();
                }
                else
                {
                    Debug.LogError("❌ 场景里找不到 VictoryUI 脚本！无法显示胜利界面。");
                    Debug.LogError("💡 请确保场景中有 VictoryUI 组件，或者直接返回主菜单。");
                    // 保底逻辑：直接返回主菜单
                    string startSceneName = "StartScene";
                    SceneManager.LoadScene(startSceneName);
                }
                return; // 不显示奖励界面
            }

            // 寻找并打开 UI
            Debug.Log("🔍 开始查找 RewardUI...");
            RewardUI rewardUI = FindObjectOfType<RewardUI>(true);
            
            if (rewardUI != null)
            {
                Debug.Log($"✅ 找到 RewardUI: {rewardUI.gameObject.name}, 当前状态: Active={rewardUI.gameObject.activeSelf}");
                Debug.Log("🎁 准备显示奖励选择界面...");
                rewardUI.Show();
            }
            else
            {
                Debug.LogError("❌ 场景里找不到 RewardUI 脚本！无法显示奖励。");
                Debug.LogError("💡 请确保 RewardPanel 上有 RewardUI 组件，并且场景中确实存在该对象。");
                // 找不到 UI 时的保底逻辑：直接下一关
                // AdvanceStage(); 
            }
        }
        else
        {
            Debug.Log("💀 战斗失败，游戏结束");
            Debug.Log("   - 显示死亡界面");
            
            // 显示死亡界面
            DeathUI deathUI = FindObjectOfType<DeathUI>(true);
            if (deathUI != null)
            {
                Debug.Log($"✅ 找到 DeathUI: {deathUI.gameObject.name}");
                deathUI.Show();
            }
            else
            {
                Debug.LogError("❌ 场景里找不到 DeathUI 脚本！无法显示死亡界面。");
                Debug.LogError("💡 请确保场景中有 DeathUI 组件，或者直接返回主菜单。");
                // 保底逻辑：直接返回主菜单
                string startSceneName = "StartScene";
                if (BattleFlowManager.Instance != null && !string.IsNullOrEmpty(BattleFlowManager.Instance.startSceneName))
                {
                    startSceneName = BattleFlowManager.Instance.startSceneName;
                }
                SceneManager.LoadScene(startSceneName);
            }
        }
        
        Debug.Log("═══════════════════════════════════════════════════════════");
    }
}