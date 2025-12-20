using UnityEngine;

public class PlayCardGA : GameAction
{
    public EnemyView ManualTarget {get; private set;}
    public Card Card {get; set;}
    
    /// <summary>
    /// 存储当前想要打出的牌
    /// </summary>
    /// <param name="card"></param>
    public PlayCardGA(Card card)
    {
        Card = card;
        ManualTarget = null;
        Debug.Log($"创建打出卡牌事件：{card.Title}");
    }

    public PlayCardGA(Card card, EnemyView target)
    {
        Card = card;
        ManualTarget = target;
        Debug.Log($"创建打出卡牌事件（手动目标）：{card.Title} -> {target.name}");
    }
}
