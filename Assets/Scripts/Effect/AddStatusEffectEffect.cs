using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffectEffect : Effect
{
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField] private int stackCount;
    
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        Debug.Log($"效果：添加状态 {statusEffectType} 层数 {stackCount}");
        return new AddStatusEffectGA(statusEffectType, stackCount, targets);
    }
}
