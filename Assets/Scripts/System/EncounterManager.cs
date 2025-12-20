using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 遭遇管理器：管理遭遇池，根据关卡选择遭遇
/// </summary>
public class EncounterManager : MonoSingleton<EncounterManager>
{
    [Header("遭遇池配置")]
    [Tooltip("所有普通遭遇")]
    public List<EncounterData> normalEncounters = new List<EncounterData>();
    
    [Tooltip("所有精英遭遇")]
    public List<EncounterData> eliteEncounters = new List<EncounterData>();
    
    [Tooltip("所有Boss遭遇")]
    public List<EncounterData> bossEncounters = new List<EncounterData>();
    
    [Header("固定顺序流程（用于测试）")]
    [Tooltip("按顺序排列的遭遇序列。如果设置了此列表，将使用固定顺序而不是随机选择。")]
    public List<EncounterData> encounterSequence = new List<EncounterData>();
    
    [Tooltip("是否使用固定顺序流程")]
    public bool useFixedSequence = false;
    
    /// <summary>
    /// 根据关卡索引和类型获取可用的遭遇列表
    /// </summary>
    public List<EncounterData> GetAvailableEncounters(EncounterType type, int stageIndex)
    {
        List<EncounterData> pool = GetEncounterPool(type);
        return pool.Where(e => e != null && e.CanAppearAtStage(stageIndex)).ToList();
    }
    
    /// <summary>
    /// 从可用遭遇中随机选择一个
    /// </summary>
    public EncounterData GetRandomEncounter(EncounterType type, int stageIndex)
    {
        List<EncounterData> available = GetAvailableEncounters(type, stageIndex);
        if (available.Count == 0)
        {
            Debug.LogWarning($"⚠️ 关卡 {stageIndex} 没有可用的 {type} 遭遇！");
            return null;
        }
        
        // 加权随机选择
        float totalWeight = available.Sum(e => e.weight);
        float random = Random.Range(0f, totalWeight);
        
        float currentWeight = 0f;
        foreach (var encounter in available)
        {
            currentWeight += encounter.weight;
            if (random <= currentWeight)
            {
                return encounter;
            }
        }
        
        // 如果加权随机失败，返回第一个
        return available[0];
    }
    
    /// <summary>
    /// 从可用遭遇中随机选择多个（用于关卡选择界面）
    /// </summary>
    public List<EncounterData> GetRandomEncounters(EncounterType type, int stageIndex, int count)
    {
        List<EncounterData> available = GetAvailableEncounters(type, stageIndex);
        if (available.Count == 0)
        {
            Debug.LogWarning($"⚠️ 关卡 {stageIndex} 没有可用的 {type} 遭遇！");
            return new List<EncounterData>();
        }
        
        // 如果可用遭遇少于需要的数量，返回所有可用遭遇
        if (available.Count <= count)
        {
            return new List<EncounterData>(available);
        }
        
        // 随机选择，不允许重复
        List<EncounterData> selected = new List<EncounterData>();
        List<EncounterData> pool = new List<EncounterData>(available);
        
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, pool.Count);
            selected.Add(pool[index]);
            pool.RemoveAt(index);
        }
        
        return selected;
    }
    
    /// <summary>
    /// 获取指定类型的遭遇池
    /// </summary>
    private List<EncounterData> GetEncounterPool(EncounterType type)
    {
        switch (type)
        {
            case EncounterType.Normal:
                return normalEncounters;
            case EncounterType.Elite:
                return eliteEncounters;
            case EncounterType.Boss:
                return bossEncounters;
            default:
                return new List<EncounterData>();
        }
    }
    
    /// <summary>
    /// 根据关卡索引获取固定顺序的遭遇（用于测试流程）
    /// </summary>
    public EncounterData GetEncounterByIndex(int stageIndex)
    {
        if (!useFixedSequence || encounterSequence == null || encounterSequence.Count == 0)
        {
            Debug.LogWarning("⚠️ 未启用固定顺序流程或遭遇序列为空！");
            return null;
        }
        
        if (stageIndex < 0 || stageIndex >= encounterSequence.Count)
        {
            Debug.LogWarning($"⚠️ 关卡索引 {stageIndex} 超出遭遇序列范围 (0-{encounterSequence.Count - 1})！");
            return null;
        }
        
        EncounterData encounter = encounterSequence[stageIndex];
        if (encounter == null)
        {
            Debug.LogWarning($"⚠️ 关卡索引 {stageIndex} 的遭遇数据为空！");
            return null;
        }
        
        Debug.Log($"✅ 从固定序列获取遭遇 [{stageIndex}]: {encounter.encounterName}");
        return encounter;
    }
    
    /// <summary>
    /// 检查是否还有下一关
    /// </summary>
    public bool HasNextEncounter(int currentStageIndex)
    {
        if (!useFixedSequence || encounterSequence == null)
            return false;
        
        return currentStageIndex + 1 < encounterSequence.Count;
    }
    
    /// <summary>
    /// 验证配置（在编辑器中调用）
    /// </summary>
    [ContextMenu("验证配置")]
    public void ValidateConfiguration()
    {
        Debug.Log("🔍 验证遭遇配置...");
        
        int normalCount = normalEncounters.Count(e => e != null);
        int eliteCount = eliteEncounters.Count(e => e != null);
        int bossCount = bossEncounters.Count(e => e != null);
        
        Debug.Log($"  - 普通遭遇: {normalCount}");
        Debug.Log($"  - 精英遭遇: {eliteCount}");
        Debug.Log($"  - Boss遭遇: {bossCount}");
        
        if (useFixedSequence)
        {
            int sequenceCount = encounterSequence.Count(e => e != null);
            Debug.Log($"  - 固定序列遭遇: {sequenceCount}");
            if (sequenceCount == 0)
                Debug.LogWarning("⚠️ 启用了固定序列但序列为空！");
        }
        
        if (normalCount == 0)
            Debug.LogWarning("⚠️ 没有配置普通遭遇！");
        if (eliteCount == 0)
            Debug.LogWarning("⚠️ 没有配置精英遭遇！");
        if (bossCount == 0)
            Debug.LogWarning("⚠️ 没有配置Boss遭遇！");
    }
}
