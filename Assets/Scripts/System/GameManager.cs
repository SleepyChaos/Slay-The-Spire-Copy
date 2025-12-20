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
        CurrentStageIndex = 0; // 重置关卡
        MaxHealth = heroData.Health;
        CurrentHealth = heroData.Health;
        
        MasterDeck.Clear();
        MasterDeck.AddRange(heroData.Deck);
        
        Relics.Clear();
        if(heroData.StartingPerks != null) Relics.AddRange(heroData.StartingPerks);

        SceneManager.LoadScene("LevelSelectScene"); // 去选关页面
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