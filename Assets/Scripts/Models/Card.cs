using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 给生成的卡牌赋予信息的类
/// </summary>
public class Card
{
    private readonly CardData data;

    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Effect ManualTargetEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;
    public int Mana { get; private set; }
    
    /// <summary>
    /// CardData是ScriptableObject类，构造时读取信息
    /// </summary>
    /// <param name="cardData"></param>
    public Card(CardData cardData)
    {
        data = cardData;
        Mana = cardData.Mana;
        Debug.Log($"创建卡牌数据：{data.name} 法力消耗 {Mana}");
    }
}
