using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡选择控制器：使用新的遭遇系统生成关卡选项
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    [Header("UI引用")]
    public LevelSelectUI levelSelectUI;
    
    private void Start()
    {
        // 【关键修复】检查当前场景，LevelSelectController 不应该在 StartScene 中运行
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string startSceneName = "StartScene";
        if (BattleFlowManager.Instance != null && !string.IsNullOrEmpty(BattleFlowManager.Instance.startSceneName))
        {
            startSceneName = BattleFlowManager.Instance.startSceneName;
        }
        
        if (currentSceneName == startSceneName)
        {
            Debug.LogWarning($"⚠️ LevelSelectController 在 StartScene ({currentSceneName}) 中，跳过初始化");
            Debug.LogWarning("   - LevelSelectController 应该只在 LevelSelectScene 中使用");
            return;
        }
        
        // 检查是否使用固定顺序流程
        if (EncounterManager.Instance != null && EncounterManager.Instance.useFixedSequence)
        {
            StartFixedSequenceBattle();
        }
        else
        {
            GenerateLevelOptions();
        }
    }

    /// <summary>
    /// 固定顺序流程：直接进入对应关卡战斗
    /// </summary>
    private void StartFixedSequenceBattle()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎮 【固定顺序流程】LevelSelectController 启动");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance 为空！");
            return;
        }
        
        if (EncounterManager.Instance == null)
        {
            Debug.LogError("❌ EncounterManager.Instance 为空！请确保场景中有 EncounterManager 组件。");
            return;
        }
        
        int currentStage = GameManager.Instance.CurrentStageIndex;
        int totalStages = EncounterManager.Instance.encounterSequence.Count;
        
        Debug.Log($"📊 关卡信息:");
        Debug.Log($"   - 当前关卡索引: {currentStage}");
        Debug.Log($"   - 总关卡数量: {totalStages}");
        Debug.Log($"   - 进度: {currentStage + 1}/{totalStages}");
        
        // 从固定序列中获取当前关卡的遭遇
        EncounterData encounter = EncounterManager.Instance.GetEncounterByIndex(currentStage);
        
        if (encounter == null)
        {
            Debug.LogError($"❌ 无法获取关卡 {currentStage} 的遭遇！");
            // 如果所有关卡都完成了，可以返回主菜单或显示胜利界面
            if (currentStage >= totalStages)
            {
                Debug.Log("🎉 所有关卡已完成！");
                // 可以在这里添加胜利逻辑
                return;
            }
            return;
        }
        
        Debug.Log($"📋 遭遇信息:");
        Debug.Log($"   - 遭遇名称: {encounter.encounterName}");
        Debug.Log($"   - 遭遇类型: {encounter.encounterType}");
        Debug.Log($"   - 敌人数量: {encounter.enemyCount}");
        
        // 直接开始战斗，不显示选关界面
        Debug.Log("🚀 准备开始战斗...");
        if (BattleFlowManager.Instance != null)
        {
            BattleFlowManager.Instance.StartBattle(encounter);
        }
        else
        {
            Debug.LogError("❌ BattleFlowManager.Instance 为空！");
        }
    }

    /// <summary>
    /// 生成关卡选项（随机选关模式）
    /// </summary>
    private void GenerateLevelOptions()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance 为空！");
            return;
        }
        
        if (EncounterManager.Instance == null)
        {
            Debug.LogError("❌ EncounterManager.Instance 为空！请确保场景中有 EncounterManager 组件。");
            return;
        }
        
        int currentStage = GameManager.Instance.CurrentStageIndex;
        Debug.Log($"🎯 生成关卡选项，当前关卡: {currentStage}");
        
        List<EncounterData> encounters = new List<EncounterData>();
        
        // 判断是否是 Boss 关（9次闯关后，即第10关是Boss）
        if (currentStage >= GameManager.BOSS_STAGE_INDEX)
        {
            // Boss关：只显示一个Boss选项
            EncounterData bossEncounter = EncounterManager.Instance.GetRandomEncounter(EncounterType.Boss, currentStage);
            if (bossEncounter != null)
            {
                encounters.Add(bossEncounter);
                Debug.Log($"👑 生成Boss关卡: {bossEncounter.encounterName}");
            }
            else
            {
                Debug.LogError("❌ 无法获取Boss遭遇！");
            }
        }
        else
        {
            // 普通关卡：生成3个随机选项（普通+精英混合）
            // 从普通遭遇中随机选择1-2个
            List<EncounterData> normalOptions = EncounterManager.Instance.GetRandomEncounters(
                EncounterType.Normal, currentStage, Random.Range(1, 3));
            encounters.AddRange(normalOptions);
            
            // 从精英遭遇中随机选择0-2个（确保总共3个）
            int remaining = 3 - encounters.Count;
            if (remaining > 0)
            {
                List<EncounterData> eliteOptions = EncounterManager.Instance.GetRandomEncounters(
                    EncounterType.Elite, currentStage, remaining);
                encounters.AddRange(eliteOptions);
            }
            
            // 如果还不够3个，用普通遭遇填充
            while (encounters.Count < 3)
            {
                EncounterData normal = EncounterManager.Instance.GetRandomEncounter(EncounterType.Normal, currentStage);
                if (normal != null && !encounters.Contains(normal))
                {
                    encounters.Add(normal);
                }
                else
                {
                    break; // 避免无限循环
                }
            }
            
            Debug.Log($"✅ 生成了 {encounters.Count} 个关卡选项");
        }
        
        // 显示关卡选择UI
        if (levelSelectUI != null)
        {
            levelSelectUI.Show(encounters);
        }
        else
        {
            Debug.LogError("❌ LevelSelectUI 引用为空！请在 Inspector 中设置。");
        }
    }
}