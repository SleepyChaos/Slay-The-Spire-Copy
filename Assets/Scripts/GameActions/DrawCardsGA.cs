using UnityEngine;

/// <summary>
/// 抽卡事件
/// </summary>
public class DrawCardsGA : GameAction
{
    public int Amount {get; set;}
    
    /// <summary>
    /// 设置抽卡数量
    /// </summary>
    /// <param name="amount">可抽卡数量</param>
    public DrawCardsGA(int amount)
    {
        Amount = amount;
        Debug.Log($"创建抽卡事件：{amount}");
    }
}
