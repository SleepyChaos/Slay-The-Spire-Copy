using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private PerkData perkData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎯 【战斗初始化】MatchSetupSystem 开始初始化战斗");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        // === 1. 尝试从全局管理器读取数据 ===
        if (GameManager.Instance != null)
        {
            // 如果是新游戏（第一关），MasterDeck 可能为空，需要初始化
            if (GameManager.Instance.MasterDeck == null || GameManager.Instance.MasterDeck.Count == 0)
            {
                Debug.Log("📦 检测到新游戏，初始化 MasterDeck...");
                GameManager.Instance.MasterDeck = new List<CardData>();
                if (heroData.Deck != null && heroData.Deck.Count > 0)
                {
                    GameManager.Instance.MasterDeck.AddRange(heroData.Deck);
                    Debug.Log($"   - 已从 HeroData 初始化牌库: {GameManager.Instance.MasterDeck.Count} 张卡牌");
                }
                else
                {
                    Debug.LogWarning("⚠️ HeroData.Deck 为空！无法初始化牌库。");
                }
            }
            
            Debug.Log("📦 从 GameManager 读取游戏进度...");
            Debug.Log($"   - 关卡索引: {GameManager.Instance.CurrentStageIndex}");
            Debug.Log($"   - 玩家血量: {GameManager.Instance.CurrentHealth}/{GameManager.Instance.MaxHealth}");
            Debug.Log($"   - 牌库数量: {GameManager.Instance.MasterDeck.Count}");
            
            // 初始化英雄视图
            HeroSystem.Instance.Setup(heroData);
            
            // 【关键】检查并初始化血量
            // 如果 MaxHealth 为0，说明是新游戏，需要从 HeroData 初始化
            if (GameManager.Instance.MaxHealth <= 0)
            {
                Debug.LogWarning("⚠️ GameManager.MaxHealth 为0，从 HeroData 初始化血量");
                GameManager.Instance.MaxHealth = heroData.Health;
                GameManager.Instance.CurrentHealth = heroData.Health;
            }
            
            Debug.Log($"   - 血量配置: MaxHealth={GameManager.Instance.MaxHealth}, CurrentHealth={GameManager.Instance.CurrentHealth}");
            
            // 【关键】强制设置血量为上一关剩下的血
            // 注意：SetupBase 会把血设置为 MaxHealth，所以我们需要手动调整
            HeroSystem.Instance.HeroView.SetupBase(GameManager.Instance.MaxHealth, heroData.Image);
            
            // 计算需要设置的血量（如果当前血量小于最大血量）
            int targetHealth = GameManager.Instance.CurrentHealth;
            int currentMaxHealth = GameManager.Instance.MaxHealth;
            
            // 确保目标血量在有效范围内
            targetHealth = Mathf.Clamp(targetHealth, 0, currentMaxHealth);
            
            if (targetHealth < currentMaxHealth)
            {
                Debug.Log($"   - 恢复上一关血量: 当前应为 {targetHealth}/{currentMaxHealth}");
                
                // 使用 SetHealth 方法直接设置血量，而不是调用 Damage（避免触发死亡检查）
                HeroSystem.Instance.HeroView.SetHealth(targetHealth);
            }
            else
            {
                Debug.Log($"   - 血量已满: {targetHealth}/{currentMaxHealth}");
            }
            
            // 验证最终血量
            Debug.Log($"   - 最终血量: {HeroSystem.Instance.HeroView.CurrentHealth}/{HeroSystem.Instance.HeroView.MaxHealth}");
            
            // 同步到 GameManager（确保数据一致）
            GameManager.Instance.CurrentHealth = HeroSystem.Instance.HeroView.CurrentHealth;
            GameManager.Instance.MaxHealth = HeroSystem.Instance.HeroView.MaxHealth;

            // 【关键】使用全局牌库，而不是初始牌库
            Debug.Log($"   - 牌库详情:");
            Debug.Log($"     * 牌库总数: {GameManager.Instance.MasterDeck.Count} 张");
            for (int i = 0; i < GameManager.Instance.MasterDeck.Count; i++)
            {
                Debug.Log($"     * 卡牌 {i + 1}: {GameManager.Instance.MasterDeck[i].name}");
            }
            
            // 验证牌库不为空
            if (GameManager.Instance.MasterDeck.Count == 0)
            {
                Debug.LogError("❌ MasterDeck 为空！无法初始化战斗。");
                Debug.LogError("💡 请确保 HeroData.Deck 已正确配置，或检查 StartNewRun 是否被正确调用。");
            }
            
            CardSystem.Instance.SetUp(GameManager.Instance.MasterDeck);
            Debug.Log($"   - 已设置牌库到 CardSystem: {GameManager.Instance.MasterDeck.Count} 张卡牌");
            
            // 初始化遗物 (这里以后也要改从 GameManager 读)
            if (heroData.StartingPerks != null)
            {
                Debug.Log($"   - 初始化遗物: {heroData.StartingPerks.Count} 个");
                foreach (var perkData in heroData.StartingPerks)
                    PerkSystem.Instance.AddPerk(new Perk(perkData));
            }
        }
        else 
        {
            // === 2. 如果没有 GameManager (比如直接在场景里测试)，走默认流程 ===
            Debug.LogWarning("⚠️ 未找到 GameManager，使用默认配置测试。");
            
            HeroSystem.Instance.Setup(heroData);
            
            if (heroData.Deck != null && heroData.Deck.Count > 0)
            {
                CardSystem.Instance.SetUp(heroData.Deck);
                Debug.Log($"   - 使用默认牌库: {heroData.Deck.Count} 张卡牌");
            }
            else
            {
                Debug.LogError("❌ HeroData.Deck 为空！无法初始化牌库。");
            }
            
            if (heroData.StartingPerks != null)
            {
                Debug.Log($"   - 初始化遗物: {heroData.StartingPerks.Count} 个");
                foreach (var perkData in heroData.StartingPerks)
                    PerkSystem.Instance.AddPerk(new Perk(perkData));
            }
        }

        // 初始化敌人（优先从 BattleFlowManager 读取，然后从 GameManager，最后使用默认配置）
        Debug.Log("👹 初始化敌人...");
        
        bool enemiesInitialized = false;
        
        // 优先级1：从 BattleFlowManager 读取遭遇配置
        if (BattleFlowManager.Instance != null && BattleFlowManager.Instance.CurrentEncounter != null)
        {
            // 优先使用 BattleFlowManager 的遭遇配置
            List<EnemyData> enemies = BattleFlowManager.Instance.CurrentEncounter.SelectEnemies();
            if (enemies != null && enemies.Count > 0)
            {
                Debug.Log($"✅ 从 BattleFlowManager 读取遭遇 '{BattleFlowManager.Instance.CurrentEncounter.encounterName}' 的敌人配置");
                Debug.Log($"   - 敌人数量: {enemies.Count}");
                for (int i = 0; i < enemies.Count; i++)
                {
                    Debug.Log($"   - 敌人 {i + 1}: {enemies[i].name}");
                }
                EnemySystem.Instance.Setup(enemies);
                enemiesInitialized = true;
            }
            else
            {
                Debug.LogError($"❌ 遭遇 '{BattleFlowManager.Instance.CurrentEncounter.encounterName}' 没有选择到敌人！");
            }
        }
        
        // 优先级2：从 GameManager 读取敌人配置
        if (!enemiesInitialized && GameManager.Instance != null && GameManager.Instance.NextEnemies != null && GameManager.Instance.NextEnemies.Count > 0)
        {
            Debug.Log($"✅ 从 GameManager 读取敌人配置");
            Debug.Log($"   - 敌人数量: {GameManager.Instance.NextEnemies.Count}");
            for (int i = 0; i < GameManager.Instance.NextEnemies.Count; i++)
            {
                Debug.Log($"   - 敌人 {i + 1}: {GameManager.Instance.NextEnemies[i].name}");
            }
            EnemySystem.Instance.Setup(GameManager.Instance.NextEnemies);
            enemiesInitialized = true;
        }
        
        // 优先级3：使用默认配置（仅用于测试，不应该在正常游戏流程中使用）
        if (!enemiesInitialized && enemyDatas != null && enemyDatas.Count > 0)
        {
            Debug.LogWarning("═══════════════════════════════════════════════════════════");
            Debug.LogWarning("⚠️ 【警告】未找到敌人配置，使用 MatchSetupSystem 的测试敌人配置！");
            Debug.LogWarning("═══════════════════════════════════════════════════════════");
            Debug.LogWarning("💡 这通常意味着：");
            Debug.LogWarning("   1. BattleFlowManager.CurrentEncounter 为空");
            Debug.LogWarning("   2. GameManager.NextEnemies 为空");
            Debug.LogWarning("   3. 可能是直接运行场景，没有通过正常游戏流程");
            Debug.LogWarning("═══════════════════════════════════════════════════════════");
            Debug.Log($"   - 测试敌人数量: {enemyDatas.Count}");
            for (int i = 0; i < enemyDatas.Count; i++)
            {
                Debug.LogWarning($"   - 测试敌人 {i + 1}: {enemyDatas[i].name}");
            }
            Debug.LogWarning("═══════════════════════════════════════════════════════════");
            EnemySystem.Instance.Setup(enemyDatas);
            enemiesInitialized = true;
        }
        
        // 如果所有方法都失败，报错
        if (!enemiesInitialized)
        {
            Debug.LogError("═══════════════════════════════════════════════════════════");
            Debug.LogError("❌ 【错误】没有可用的敌人配置！");
            Debug.LogError("═══════════════════════════════════════════════════════════");
            Debug.LogError("💡 可能的原因：");
            Debug.LogError("   1. BattleFlowManager 未正确设置 CurrentEncounter");
            Debug.LogError("   2. GameManager.NextEnemies 为空");
            Debug.LogError("   3. MatchSetupSystem.enemyDatas 为空（测试配置）");
            Debug.LogError("═══════════════════════════════════════════════════════════");
        }

        // 初始抽牌
        Debug.Log("🎴 开始初始抽牌...");
        DrawCardsGA drawCardsGA = new DrawCardsGA(5);
        ActionSystem.Instance.Perform(drawCardsGA);
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("✅ 【战斗初始化完成】MatchSetupSystem 初始化完成，战斗开始！");
        Debug.Log("═══════════════════════════════════════════════════════════");
    }

    /// <summary>
    /// 调试用：按 Space 键重新生成测试敌人（仅在编辑器中使用）
    /// </summary>
    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.LogWarning("⚠️ 【调试】按 Space 键重新生成测试敌人");
            if (enemyDatas != null && enemyDatas.Count > 0)
            {
                EnemySystem.Instance.Setup(enemyDatas);
            }
            else
            {
                Debug.LogWarning("⚠️ enemyDatas 为空，无法重新生成测试敌人");
            }
        }
        #endif
    }
}
