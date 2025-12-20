using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// 这个脚本挂在新的 UI 卡牌 Prefab 上
public class RewardCardDisplay : MonoBehaviour
{
    [Header("UI 组件引用 (需要手动拖拽)")]
    public Image cardArtImage;      // 卡牌立绘
    public TextMeshProUGUI manaText;   // 费用文本
    public TextMeshProUGUI nameText;   // 名称文本
    public TextMeshProUGUI descText;   // 描述文本
    public Button clickButton;         // 自身挂载的 Button 组件

    // 保存当前卡牌数据
    private CardData myCardData;
    // 点击时的回调函数
    private Action<CardData> onClickCallback;

    /// <summary>
    /// 初始化卡牌显示
    /// </summary>
    /// <param name="data">卡牌数据</param>
    /// <param name="callback">点击时要执行的方法</param>
    public void Setup(CardData data, Action<CardData> callback)
    {
        myCardData = data;
        onClickCallback = callback;

        // 1. 填充显示数据
        if (cardArtImage != null) cardArtImage.sprite = data.Image;
     //   if (manaText != null) manaText.text = data.ManaCost.ToString();
        if (nameText != null) nameText.text = data.name; // 或者 data.CardName 如果你有这个字段
        if (descText != null) descText.text = data.Description;

        // 2. 设置按钮点击事件
        clickButton.onClick.RemoveAllListeners(); // 清除旧事件
        clickButton.onClick.AddListener(OnClick);
    }

    // 当按钮被点击时触发
    private void OnClick()
    {
        if (onClickCallback != null && myCardData != null)
        {
            // 通知 RewardUI 我被点击了，并把我的数据传过去
            onClickCallback.Invoke(myCardData);
        }
    }
}