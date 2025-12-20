using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 战斗流程管理器：统一管理战斗的完整流程
/// </summary>
public class BattleFlowManager : MonoSingleton<BattleFlowManager>
{
    [Header("场景名称")]
    public string battleSceneName = "BattleScene";
    public string levelSelectSceneName = "LevelSelectScene";
    public string startSceneName = "StartScene";
    
    /// <summary>
    /// 当前战斗的遭遇数据
    /// </summary>
    public EncounterData CurrentEncounter { get; private set; }
    
    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle(EncounterData encounter)
    {
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("🎮 【关卡进入】开始进入新关卡");
        Debug.Log("═══════════════════════════════════════════════════════════");
        
        if (encounter == null)
        {
            Debug.LogError("❌ BattleFlowManager: 遭遇数据为空，无法开始战斗！");
            return;
        }
        
        CurrentEncounter = encounter;
        
        // 显示当前关卡信息
        if (GameManager.Instance != null)
        {
            Debug.Log($"📊 关卡信息:");
            Debug.Log($"   - 关卡索引: {GameManager.Instance.CurrentStageIndex}");
            Debug.Log($"   - 遭遇类型: {encounter.encounterType}");
            Debug.Log($"   - 遭遇名称: {encounter.encounterName}");
            Debug.Log($"   - 遭遇描述: {encounter.description}");
        }
        
        // 从遭遇中选择敌人
        List<EnemyData> enemies = encounter.SelectEnemies();
        
        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogError($"❌ BattleFlowManager: 遭遇 '{encounter.name}' 没有选择到敌人！");
            return;
        }
        
        // 显示敌人信息
        Debug.Log($"👹 敌人配置:");
        Debug.Log($"   - 敌人数量: {enemies.Count}");
        for (int i = 0; i < enemies.Count; i++)
        {
            Debug.Log($"   - 敌人 {i + 1}: {enemies[i].name}");
        }
        
        // 保存到 GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextEnemies = enemies;
            GameManager.Instance.IsEliteBattle = encounter.encounterType == EncounterType.Elite;
            Debug.Log($"✅ 战斗配置完成: {encounter.encounterType} - {encounter.encounterName}, 敌人数量: {enemies.Count}");
        }
        else
        {
            Debug.LogError("❌ GameManager.Instance 为空！");
        }
        
        // 加载战斗场景
        Debug.Log($"🚀 正在加载战斗场景: {battleSceneName}");
        SceneManager.sceneLoaded += OnBattleSceneLoaded;
        SceneManager.LoadScene(battleSceneName);
    }
    
    /// <summary>
    /// 战斗胜利后的处理
    /// </summary>
    public void OnBattleWin()
    {
        Debug.Log("🎉 战斗胜利！");
        // 战斗胜利后的逻辑由 GameManager.OnBattleResult 处理
        // 这里可以添加额外的胜利逻辑
    }
    
    /// <summary>
    /// 战斗失败后的处理
    /// </summary>
    public void OnBattleLose()
    {
        Debug.Log("💀 战斗失败！");
        // 返回主菜单或重试
        if (GameManager.Instance != null)
        {
            // 可以添加失败处理逻辑，比如返回主菜单
            // SceneManager.LoadScene("StartScene");
        }
    }
    
    /// <summary>
    /// 返回关卡选择界面
    /// </summary>
    public void ReturnToLevelSelect()
    {
        Debug.Log($"🔙 返回关卡选择界面: {levelSelectSceneName}");
        SceneManager.LoadScene(levelSelectSceneName);
    }
    
    /// <summary>
    /// 返回开始界面（游戏完成或失败）
    /// </summary>
    public void GoToStartScene()
    {
        Debug.Log($"🏠 返回开始界面: {startSceneName}");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(startSceneName);
    }
    
    /// <summary>
    /// 战斗场景加载完成后的回调
    /// </summary>
    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnBattleSceneLoaded;
        // 重置 MouseUtil 的 Camera 引用
        MouseUtil.ResetCamera();
        
        Debug.Log("═══════════════════════════════════════════════════════════");
        Debug.Log("✅ 【关卡进入完成】战斗场景加载完成");
        Debug.Log($"   - 场景名称: {scene.name}");
        Debug.Log($"   - 已重置 MouseUtil Camera");
        if (CurrentEncounter != null)
        {
            Debug.Log($"   - 当前遭遇: {CurrentEncounter.encounterName}");
        }
        if (GameManager.Instance != null)
        {
            Debug.Log($"   - 关卡索引: {GameManager.Instance.CurrentStageIndex}");
            Debug.Log($"   - 玩家血量: {GameManager.Instance.CurrentHealth}/{GameManager.Instance.MaxHealth}");
            Debug.Log($"   - 牌库数量: {GameManager.Instance.MasterDeck.Count}");
        }
        Debug.Log("═══════════════════════════════════════════════════════════");
    }
    
    /// <summary>
    /// 场景加载完成后的回调
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        // 重置 MouseUtil 的 Camera 引用
        MouseUtil.ResetCamera();
        Debug.Log("✅ 场景加载完成，已重置 MouseUtil Camera");
    }
}
