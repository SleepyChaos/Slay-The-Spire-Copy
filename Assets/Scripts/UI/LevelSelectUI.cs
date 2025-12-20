using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡选择UI：复用奖励卡UI的设计，显示3个关卡选项供玩家选择
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [Header("界面组件")]
    public GameObject content;           // 整个 Panel
    public Transform nodeContainer;      // 关卡节点排列的父节点
    public Button skipButton;            // 跳过按钮（可选，通常不需要）
    
    [Header("预制体引用")]
    public LevelNodeAdapter nodePrefab;  // 关卡节点预制体
    
    private void Start()
    {
        HideUI();
        
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(OnSkipClicked);
        }
    }
    
    /// <summary>
    /// 显示关卡选择界面
    /// </summary>
    public void Show(List<EncounterData> encounters)
    {
        if (encounters == null || encounters.Count == 0)
        {
            Debug.LogError("❌ LevelSelectUI: 遭遇列表为空！");
            return;
        }
        
        Debug.Log($"🎯 【LevelSelectUI】显示关卡选择，共 {encounters.Count} 个选项");
        
        // 确保父对象激活
        Transform parent = transform.parent;
        while (parent != null)
        {
            if (!parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
            }
            parent = parent.parent;
        }
        
        // 检查Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            if (!canvas.gameObject.activeSelf)
            {
                canvas.gameObject.SetActive(true);
            }
            if (canvas.sortingOrder < 100)
            {
                canvas.sortingOrder = 100;
            }
        }
        
        // 显示UI
        gameObject.SetActive(true);
        if (content != null)
        {
            content.SetActive(true);
        }
        
        // 生成关卡节点
        GenerateLevelNodes(encounters);
        
        // 强制刷新Canvas
        Canvas.ForceUpdateCanvases();
        
        Debug.Log("✅ 关卡选择UI显示完成！");
    }
    
    /// <summary>
    /// 生成关卡节点
    /// </summary>
    private void GenerateLevelNodes(List<EncounterData> encounters)
    {
        if (nodeContainer == null)
        {
            Debug.LogError("❌ LevelSelectUI: nodeContainer 引用为空！");
            return;
        }
        
        if (nodePrefab == null)
        {
            Debug.LogError("❌ LevelSelectUI: nodePrefab 引用为空！");
            return;
        }
        
        // 清理旧节点
        foreach (Transform child in nodeContainer)
        {
            if (child != null)
                Destroy(child.gameObject);
        }
        
        // 生成新节点
        for (int i = 0; i < encounters.Count; i++)
        {
            EncounterData encounter = encounters[i];
            if (encounter == null)
            {
                Debug.LogWarning($"⚠️ 遭遇数据 [{i}] 为空，跳过");
                continue;
            }
            
            LevelNodeAdapter adapter = Instantiate(nodePrefab, nodeContainer);
            adapter.Setup(encounter, OnLevelSelected);
            Debug.Log($"✅ 生成关卡节点 [{i}]: {encounter.encounterName}");
        }
    }
    
    /// <summary>
    /// 玩家选择了某个关卡
    /// </summary>
    private void OnLevelSelected(EncounterData encounter)
    {
        Debug.Log($"✅ 玩家选择了关卡: {encounter.encounterName}");
        
        // 隐藏UI
        HideUI();
        
        // 开始战斗
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
    /// 跳过按钮点击（通常不需要，但保留接口）
    /// </summary>
    private void OnSkipClicked()
    {
        Debug.Log("⏩ 玩家选择跳过（不应该出现）");
        // 通常关卡选择不能跳过，但保留接口以防需要
    }
    
    /// <summary>
    /// 隐藏UI
    /// </summary>
    public void HideUI()
    {
        if (content != null)
            content.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
