using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 关卡节点适配器：用于在关卡选择界面显示单个关卡选项
/// 复用 RewardCardAdapter 的设计思路，但显示关卡信息而不是卡牌
/// </summary>
public class LevelNodeAdapter : MonoBehaviour
{
    [Header("UI组件")]
    public Button clickButton;
    public Image iconImage;          // 关卡图标
    public TMP_Text titleText;       // 关卡标题
    public TMP_Text descriptionText; // 关卡描述
    public GameObject eliteBadge;    // 精英标记（可选）
    public GameObject bossBadge;     // Boss标记（可选）
    
    private EncounterData encounterData;
    private Action<EncounterData> onClickCallback;
    
    /// <summary>
    /// 设置关卡节点数据
    /// </summary>
    public void Setup(EncounterData encounter, Action<EncounterData> callback)
    {
        if (encounter == null)
        {
            Debug.LogError("❌ LevelNodeAdapter: EncounterData 为空！");
            return;
        }
        
        encounterData = encounter;
        onClickCallback = callback;
        
        // 设置标题
        if (titleText != null)
        {
            titleText.text = string.IsNullOrEmpty(encounter.encounterName) 
                ? GetDefaultName(encounter.encounterType) 
                : encounter.encounterName;
        }
        
        // 设置描述
        if (descriptionText != null)
        {
            if (!string.IsNullOrEmpty(encounter.description))
            {
                descriptionText.text = encounter.description;
            }
            else
            {
                // 生成默认描述
                string enemyInfo = "";
                if (encounter.enemyPool != null && encounter.enemyPool.Count > 0)
                {
                    enemyInfo = $"敌人: {encounter.enemyCount}个";
                }
                descriptionText.text = enemyInfo;
            }
        }
        
        // 显示/隐藏标记
        if (eliteBadge != null)
        {
            eliteBadge.SetActive(encounter.encounterType == EncounterType.Elite);
        }
        
        if (bossBadge != null)
        {
            bossBadge.SetActive(encounter.encounterType == EncounterType.Boss);
        }
        
        // 设置图标（如果有）
        if (iconImage != null && encounter.enemyPool != null && encounter.enemyPool.Count > 0)
        {
            // 使用第一个敌人的图片作为图标
            if (encounter.enemyPool[0].Image != null)
            {
                iconImage.sprite = encounter.enemyPool[0].Image;
                iconImage.enabled = true;
            }
        }
        
        // 绑定点击事件
        if (clickButton != null)
        {
            clickButton.onClick.RemoveAllListeners();
            clickButton.onClick.AddListener(OnClick);
            clickButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("❌ LevelNodeAdapter: clickButton 未设置！无法点击关卡。");
        }
        
        Debug.Log($"✅ LevelNodeAdapter 设置完成: {encounter.encounterName}");
    }
    
    private string GetDefaultName(EncounterType type)
    {
        switch (type)
        {
            case EncounterType.Normal:
                return "普通战斗";
            case EncounterType.Elite:
                return "精英战斗";
            case EncounterType.Boss:
                return "Boss战斗";
            default:
                return "未知关卡";
        }
    }
    
    private void OnClick()
    {
        Debug.Log($"🃏 点击了关卡: {encounterData?.encounterName ?? "未知"}");
        if (onClickCallback != null && encounterData != null)
        {
            onClickCallback.Invoke(encounterData);
        }
        else
        {
            Debug.LogWarning("⚠️ LevelNodeAdapter: 回调函数或遭遇数据为空！");
        }
    }
}
