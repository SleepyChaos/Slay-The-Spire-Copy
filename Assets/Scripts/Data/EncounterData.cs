using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 遭遇数据：定义一场战斗的配置
/// </summary>
[CreateAssetMenu(menuName = "Data/Encounter")]
public class EncounterData : ScriptableObject
{
    [Header("基础信息")]
    [Tooltip("遭遇类型")]
    public EncounterType encounterType = EncounterType.Normal;
    
    [Tooltip("遭遇名称（用于显示）")]
    public string encounterName;
    
    [Tooltip("遭遇描述（可选）")]
    [TextArea(2, 4)]
    public string description;
    
    [Header("敌人配置")]
    [Tooltip("该遭遇可能出现的敌人池")]
    public List<EnemyData> enemyPool = new List<EnemyData>();
    
    [Tooltip("同时出现的敌人数量")]
    [Range(1, 5)]
    public int enemyCount = 1;
    
    [Tooltip("敌人选择模式")]
    public EnemySelectionMode selectionMode = EnemySelectionMode.Random;
    
    [Header("关卡限制")]
    [Tooltip("最早出现的关卡索引（0开始）")]
    public int minStageIndex = 0;
    
    [Tooltip("最晚出现的关卡索引（-1表示无限制）")]
    public int maxStageIndex = -1;
    
    [Tooltip("随机权重（用于加权随机选择）")]
    [Range(0, 100)]
    public float weight = 10f;
    
    /// <summary>
    /// 从敌人池中选择敌人列表
    /// </summary>
    public List<EnemyData> SelectEnemies()
    {
        if (enemyPool == null || enemyPool.Count == 0)
        {
            Debug.LogError($"❌ EncounterData '{name}' 的敌人池为空！");
            return new List<EnemyData>();
        }
        
        List<EnemyData> selected = new List<EnemyData>();
        
        switch (selectionMode)
        {
            case EnemySelectionMode.Random:
                // 随机选择，允许重复
                for (int i = 0; i < enemyCount; i++)
                {
                    selected.Add(enemyPool[Random.Range(0, enemyPool.Count)]);
                }
                break;
                
            case EnemySelectionMode.Unique:
                // 随机选择，不允许重复
                List<EnemyData> available = new List<EnemyData>(enemyPool);
                int count = Mathf.Min(enemyCount, available.Count);
                for (int i = 0; i < count; i++)
                {
                    int index = Random.Range(0, available.Count);
                    selected.Add(available[index]);
                    available.RemoveAt(index);
                }
                break;
                
            case EnemySelectionMode.Sequential:
                // 按顺序选择
                for (int i = 0; i < Mathf.Min(enemyCount, enemyPool.Count); i++)
                {
                    selected.Add(enemyPool[i]);
                }
                break;
        }
        
        return selected;
    }
    
    /// <summary>
    /// 检查该遭遇是否可以在指定关卡出现
    /// </summary>
    public bool CanAppearAtStage(int stageIndex)
    {
        if (stageIndex < minStageIndex) return false;
        if (maxStageIndex >= 0 && stageIndex > maxStageIndex) return false;
        return true;
    }
}

/// <summary>
/// 遭遇类型
/// </summary>
public enum EncounterType
{
    Normal,  // 普通战斗
    Elite,   // 精英战斗
    Boss     // Boss战斗
}

/// <summary>
/// 敌人选择模式
/// </summary>
public enum EnemySelectionMode
{
    Random,    // 随机选择（允许重复）
    Unique,    // 随机选择（不允许重复）
    Sequential // 按顺序选择
}
