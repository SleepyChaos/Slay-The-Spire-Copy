using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;
    
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        Debug.Log($"效果：造成伤害 {damageAmount}");
        DealDamageGA dealDamageGA = new DealDamageGA(damageAmount,targets,caster);
        return dealDamageGA;
    }
}
