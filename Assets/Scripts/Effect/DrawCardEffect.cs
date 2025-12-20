using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽卡卡牌    
/// </summary>
public class DrawCardEffect : Effect
{
    [SerializeField] private int drawAmount;
    
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        Debug.Log($"效果：抽牌 {drawAmount}");
        DrawCardsGA drawCardsGA = new DrawCardsGA(drawAmount);
        return drawCardsGA; 
    }
}
